using LTRLib.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.IO;
using LTRData.Extensions.Formatting;

namespace DiskVolumes;

public static class DiskVolumes
{
    public static int UnsafeMain(params string[] args)
    {
        var argList = new List<string>(args);

        var showContainedMountPoints = false;
        if (argList.RemoveAll(arg => "/S".Equals(arg, StringComparison.OrdinalIgnoreCase)) > 0)
        {
            showContainedMountPoints = true;
        }

        if (argList.Count == 0)
        {
            ListVolumes(showContainedMountPoints);
        }
        else
        {
            argList.ForEach(arg => ListVolume(arg, showContainedMountPoints));
        }

        //if (Debugger.IsAttached)
        //{
        //    Console.ReadKey();
        //}

        return 0;
    }

    public static int ListVolumes(bool showContainedMountPoints)
    {
        foreach (var vol in new VolumeEnumerator())
        {
            Console.WriteLine(vol);

            ListVolume(vol, showContainedMountPoints);

            Console.WriteLine();
        }

        return 0;
    }

    public static int ListVolume(string vol, bool showContainedMountPoints)
    {
        try
        {
            var target = NativeFileIO.QueryDosDevice(vol.Substring(4, 44))?.FirstOrDefault();

            Console.WriteLine($"Device object: {target}");

            var links = NativeFileIO.QueryDosDevice()
                .Select(l => new { l, t = NativeFileIO.QueryDosDevice(l) })
                .Where(o => o.t is not null && o.t.Contains(target, StringComparer.OrdinalIgnoreCase))
                .Select(o => o.l);

            Console.WriteLine("Device object links:");

            foreach (var link in links)
            {
                Console.WriteLine($"  {link}");
            }

            var mnt = NativeFileIO.GetVolumeMountPoints(vol);

            if (mnt.Length == 0)
            {
                Console.WriteLine("No mountpoints");
            }
            else
            {
                Console.WriteLine($"Mounted at:");

                foreach (var m in mnt)
                {
                    Console.WriteLine($"  {m}");
                }
            }

            Console.WriteLine($"Disk extents:");

            using var volobj = NativeFileIO.OpenFileHandle(vol.TrimEnd('\\'), FileMode.Open, 0, FileShare.ReadWrite, false);

            foreach (var ext in NativeFileIO.GetVolumeDiskExtents(volobj))
            {
                Console.WriteLine($"  Disk {ext.DiskNumber} at {ext.StartingOffset}, {ext.ExtentLength} bytes ({SizeFormatting.FormatBytes(ext.ExtentLength)}).");
            }
        }
        catch (Exception ex)
        {
            if (ex is not Win32Exception wex || wex.NativeErrorCode != 1)
            {
                Console.Error.WriteLine(ex.JoinMessages());
            }
        }

        if (showContainedMountPoints)
        {
            try
            {
                foreach (var mnt in new VolumeMountPointEnumerator(vol))
                {
                    var volmnt = vol + mnt;

                    Console.Write($"Contains mount point at {mnt}: ");
                    try
                    {
                        var target = NativeFileIO.GetVolumeNameForVolumeMountPoint(volmnt);
                        Console.Write($"{target}: ");
                        var target_mounts = NativeFileIO.GetVolumeMountPoints(target).Length;
                        if (target_mounts == 0)
                        {
                            Console.WriteLine("(not attached)");
                        }
                        else
                        {
                            Console.WriteLine($"({target_mounts} mount points)");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"Error: {ex.JoinMessages()}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error enumerating contained mount points: {ex.JoinMessages()}");
            }
        }

        return 0;
    }
}
