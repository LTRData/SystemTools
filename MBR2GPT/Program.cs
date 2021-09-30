using DiscUtils;
using System;
using System.IO;
using System.Linq;
using LTRLib.Extensions;
using DiscUtils.Partitions;
using System.Collections.Generic;
using LTRLib.IO.Interop;
using LTRLib.IO;
using DiscUtils.Streams;
using DiscUtils.Raw;

namespace MBR2GPT
{
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

        public static void UnsafeMain(params string[] args)
        {
            foreach (var arg in args)
            {
                Console.WriteLine($"Opening '{arg}'...");

                using var disk = OpenVirtualDisk(arg, FileAccess.ReadWrite);

                var partition_table = disk.Partitions;

                if (partition_table is null)
                {
                    throw new NotSupportedException("No partitions detected.");
                }

                Console.WriteLine($"Current partition table: {partition_table.GetType().Name}");

                var partitions = partition_table.Partitions;

                Console.WriteLine($"Number of partitions: {partitions.Count}");

                var extents = partitions.Select((p, i) => (i + 1, p.FirstSector, p.LastSector, p.SectorCount)).ToArray();

                foreach (var (i, FirstSector, LastSector, SectorCount) in extents)
                {
                    Console.WriteLine($"Partition {i}, offset sector {FirstSector}, number of sectors {SectorCount} ({CppFormatting.FormatBytes(SectorCount * disk.Geometry.BytesPerSector)})");
                }

                Console.WriteLine("Do you want to replace the current partition table with a new GPT partition table? (y/N)");
                var keychar = Console.ReadKey().KeyChar;
                Console.WriteLine();
                if (keychar != 'y')
                {
                    Console.WriteLine("Cancelled");
                    return;
                }

                Console.WriteLine("Creating new partition table...");
                var new_table = GuidPartitionTable.Initialize(disk);
                foreach (var (i, FirstSector, LastSector, SectorCount) in extents)
                {
                    Console.WriteLine($"Creating partition {i}, offset sector {FirstSector}, number of sectors {SectorCount}");
                    new_table.Create(FirstSector, LastSector, GuidPartitionTypes.WindowsBasicData, 0, null);
                }

                Console.WriteLine("Done. The disk now has a GPT partition table.");
            }
        }

        public static VirtualDisk OpenVirtualDisk(string path, FileAccess access)
        {
            if (path.StartsWith(@"\\?\", StringComparison.Ordinal) ||
                path.StartsWith(@"\\.\", StringComparison.Ordinal))
            {
                var disk = new DiskStream(path, access);
                try
                {
                    var geometry = disk.Geometry;
                    if (geometry.HasValue)
                    {
                        return new Disk(disk, Ownership.Dispose, new Geometry(
                            geometry.Value.Cylinders, geometry.Value.TracksPerCylinder, geometry.Value.SectorsPerTrack, geometry.Value.BytesPerSector));
                    }
                    else
                    {
                        return new Disk(disk, Ownership.Dispose);
                    }
                }
                catch (Exception ex)
                {
                    disk.Close();
                    throw new IOException($"Error opening disk '{path}'", ex);
                }
            }

            return VirtualDisk.OpenDisk(path, access) ?? new Disk(path, access);
        }
    }
}
