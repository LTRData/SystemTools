using IWshRuntimeLibrary;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading;
using File = System.IO.File;

namespace RecentCleanup;

public static class Program
{
    private readonly static string RecentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Recent);

#if NETCOREAPP
    [SupportedOSPlatform("windows")]
#endif
    public static void Main(params string[] args)
    {
        if (args is not null
            && args.SingleOrDefault() == "--trace")
        {
            Trace.Listeners.Add(new ConsoleTraceListener(useErrorStream: true));
        }

        var shell = new WshShell();

        using var watcher = new FileSystemWatcher(RecentFolder, "*.lnk");

        for (; ; )
        {
            Thread.Sleep(4_000);

            foreach (var link in Directory.EnumerateFiles(RecentFolder, "*.lnk"))
            {
                try
                {
                    Trace.WriteLine($"{DateTime.UtcNow}: Analyzing shortcut '{link}'");

                    var shortcut = (IWshShortcut)shell.CreateShortcut(link);

                    var targetPath = shortcut.TargetPath;

                    Marshal.ReleaseComObject(shortcut);

                    if (string.IsNullOrWhiteSpace(targetPath))
                    {
                        continue;
                    }

                    try
                    {
                        Trace.WriteLine($"{DateTime.UtcNow}: Checking for target file '{targetPath}'");

                        if (!Directory.Exists(targetPath)
                            && !File.Exists(targetPath))
                        {
                            File.Delete(link);

                            Trace.WriteLine($"{DateTime.UtcNow}: Removed shortcut '{link}'");
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine($"{DateTime.UtcNow}: Failed to remove shortcut '{link}': {ex}");
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"{DateTime.UtcNow}: Failed to check shortcut '{link}': {ex}");
                }
            }

            Trace.WriteLine($"{DateTime.UtcNow}: Waiting for changes to '{RecentFolder}'");

            watcher.WaitForChanged(WatcherChangeTypes.Changed);
        }
    }
}
