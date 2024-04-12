using DiscUtils;
using System;
using System.IO;
using System.Linq;
using DiscUtils.Partitions;
using DiscUtils.Raw;
using LTRData.Extensions.Formatting;
using LTRData.Extensions.CommandLine;
using DiscUtils.Streams;
using LTRLib.IO;
using System.Runtime.InteropServices;

namespace ImageMBR2GPT;

public static class Program
{
    static Program()
    {
        var asms = new[]
        {
            typeof(DiscUtils.Vhd.Disk).Assembly,
            typeof(DiscUtils.Vhdx.Disk).Assembly,
            typeof(DiscUtils.Vmdk.Disk).Assembly,
            typeof(DiscUtils.Vdi.Disk).Assembly,
            typeof(DiscUtils.Dmg.Disk).Assembly
        };

        foreach (var asm in asms.Distinct())
        {
            DiscUtils.Setup.SetupHelper.RegisterAssembly(asm);
        }
    }

    public static int Main(params string[] args)
    {
        try
        {
            UnsafeMain(args);
            return 0;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(ex.JoinMessages());
            Console.ResetColor();

            return ex.HResult;
        }
    }

    private enum TargetLayout
    {
        None,
        MBR,
        GPT
    }

    public static void UnsafeMain(params string[] args)
    {
        var targetLayout = TargetLayout.None;
        var access = FileAccess.Read;

        string[]? images = null;

        var cmds = CommandLineParser.ParseCommandLine(args, StringComparer.OrdinalIgnoreCase);

        foreach (var cmd in cmds)
        {
            if (cmd.Key is "g" or "gpt"
                && targetLayout == TargetLayout.None)
            {
                targetLayout = TargetLayout.GPT;
                access = FileAccess.ReadWrite;
            }
            else if (cmd.Key is "m" or "mbr"
                && targetLayout == TargetLayout.None)
            {
                targetLayout = TargetLayout.MBR;
                access = FileAccess.ReadWrite;
            }
            else if (cmd.Key == ""
                && cmd.Value.Length > 0)
            {
                images = cmd.Value;
            }
            else
            { 
                Console.WriteLine(@"ImageMBR2GPT
Copyright (c) 2023 - 2024 Olof Lagerkvist, LTR Data, https://ltr-data.se

Tool for in-place conversion between MBR and GPT partition table formats.
Writes a new partition table with the same partition extents as in existing
partition table, preserving all data within partitions.

Syntax:
ImageMBR2GPT [--gpt | --mbr] imagefile1 [imagefile2 ...]

--gpt           Write a new GPT partition table replacing existing partition
-g              table.

--mbr           Write a new MBR partition table replacing existing partition
-m              table.
");

                return;
            }
        }

        if (images is null || images.Length == 0)
        {
            throw new InvalidOperationException("Missing image file path");
        }

        foreach (var image in images)
        {
            Console.WriteLine($"Opening '{image}'...");

            using var disk = OpenVirtualDisk(image, access);

            var partition_table = disk.Partitions
                ?? throw new NotSupportedException("No partitions detected.");

            Console.WriteLine($"Current partition table: {partition_table.GetType().Name}");

            var partitions = partition_table.Partitions;

            Console.WriteLine($"Number of partitions: {partitions.Count}");

            var extents = partitions.Select((p, i) => (i + 1, p)).ToArray();

            foreach (var (i, p) in extents)
            {
                string? active = null;

                if (p is BiosPartitionInfo bios_part
                    && bios_part.IsActive)
                {
                    active = ", active";
                }

                Console.WriteLine($"Partition {i}, {p.TypeAsString}, offset sector {p.FirstSector}, number of sectors {p.SectorCount} ({SizeFormatting.FormatBytes(p.SectorCount * disk.Geometry.BytesPerSector)}){active}");
            }

            if (targetLayout == TargetLayout.None)
            {
                return;
            }

            if (targetLayout == TargetLayout.MBR
                && extents.Count(extent => extent.p.GuidType != GuidPartitionTypes.MicrosoftReserved) > 4)
            {
                throw new NotSupportedException("Cannot convert disks with more partitions than 4 to MBR layout");
            }

            Console.WriteLine($"Do you want to replace the current partition table with a new {targetLayout} partition table? (y/N)");
            
            var keychar = Console.ReadKey().KeyChar;
            Console.WriteLine();
            if (keychar != 'y')
            {
                Console.WriteLine("Cancelled");
                return;
            }

            Console.WriteLine("Creating new partition table...");

            PartitionTable new_table = targetLayout switch
            {
                TargetLayout.GPT => GuidPartitionTable.Initialize(disk),
                TargetLayout.MBR => BiosPartitionTable.Initialize(disk),
                _ => throw new InvalidOperationException()
            };

            foreach (var (i, p) in extents)
            {
                Console.WriteLine($"Creating partition {i}, offset sector {p.FirstSector}, number of sectors {p.SectorCount}");

                if (new_table is GuidPartitionTable guid_table)
                {
                    var partitionType = GuidPartitionTypes.WindowsBasicData;

                    if (p is GuidPartitionInfo guidPartition)
                    {
                        partitionType = guidPartition.GuidType;
                    }

                    guid_table.Create(p.FirstSector, p.LastSector, partitionType, 0, null);
                }
                else if (new_table is BiosPartitionTable bios_table)
                {
                    if (p.GuidType == GuidPartitionTypes.MicrosoftReserved)
                    {
                        Console.WriteLine("Skipping MSR partition on MBR.");
                        continue;
                    }

                    var partitionType = BiosPartitionTypes.Ntfs;
                    var markActive = false;

                    if (p is BiosPartitionInfo biosPartition)
                    {
                        partitionType = biosPartition.BiosType;
                    }
                    else if (p is GuidPartitionInfo guidPartition
                        && guidPartition.GuidType == GuidPartitionTypes.EfiSystem)
                    {
                        partitionType = BiosPartitionTypes.EfiSystem;
                        markActive = true;
                    }

                    bios_table.CreatePrimaryBySector(p.FirstSector, p.LastSector, partitionType, markActive);
                }
            }

            Console.WriteLine($"Done. The disk now has a {targetLayout} partition table.");
        }
    }

    public static VirtualDisk OpenVirtualDisk(string path, FileAccess access)
    {
#if NETCOREAPP
        if (OperatingSystem.IsWindows())
        {
#endif
            if (path.StartsWith(@"\\?\PhysicalDrive", StringComparison.OrdinalIgnoreCase)
                || path.StartsWith(@"\\.\PhysicalDrive", StringComparison.OrdinalIgnoreCase))
            {
                var stream = new DiskStream(path, access);
                return new Disk(stream, ownsStream: Ownership.Dispose);
            }
#if NETCOREAPP
        }
#endif

        return VirtualDisk.OpenDisk(path, access) ?? new Disk(path, access);
    }
}
