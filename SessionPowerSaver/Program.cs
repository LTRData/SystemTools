using LTRData.Extensions.Formatting;
using LTRLib.IO;
using LTRLib.Services.WTS;
using Microsoft.Win32;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace SessionPowerSaver;

public static class Program
{
    private static Process CurrentProcess { get; } = Process.GetCurrentProcess();

    private static WindowsIdentity CurrentUser { get; } = WindowsIdentity.GetCurrent();

    private static readonly StreamWriter log = new(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), $"Session_{CurrentProcess.SessionId}.log"), append: true)
    {
        AutoFlush = true
    };

    private static void LogWrite(string? message)
    {
        message = $"{DateTime.UtcNow:o} Thread {Environment.CurrentManagedThreadId:X5} {message}";

        ThreadPool.QueueUserWorkItem(message =>
        {
            Trace.WriteLine(message);

            lock (log)
            {
                log.WriteLine(message);
            }
        }, message);
    }

    static Program()
    {
        AppDomain.CurrentDomain.UnhandledException += (sender, e) => LogWrite(e.ExceptionObject.ToString());
    }

    public static int Main()
    {
        LogWrite("Starting up SessionPowerSaver");

        var currentProcessTree = new List<int>();
        try
        {
            currentProcessTree.Add(CurrentProcess.Id);
            for (var ppid = CurrentProcess.QueryBasicInformation().ParentProcessId.ToInt32();
                ppid != 0;)
            {
                currentProcessTree.Add(ppid);

                using var p = Process.GetProcessById(ppid);
                if (p.HasExited)
                {
                    break;
                }

                ppid = p.QueryBasicInformation().ParentProcessId.ToInt32();
            }
        }
        catch
        {
        }

        var suspended = new ConcurrentDictionary<int, DateTime>();

        var rc = 0;

        try
        {
            ServiceLoop(currentProcessTree, suspended);
        }
        catch (Exception ex)
        {
            LogWrite(ex.ToString());
            rc = ex.HResult;
        }

        Parallel.ForEach(suspended, item =>
        {
            try
            {
                using var suspended_process = SafeOpenProcess(item.Key);

                if (suspended_process is not null && item.Value == suspended_process.StartTime)
                {
                    LogWrite($"Resuming process {suspended_process.Id} ({suspended_process.ProcessName})");
                    suspended_process.Resume();
                }
            }
            catch (Exception ex)
            {
                LogWrite($"Failed to resume process {item.Key}: {ex}");
            }
        });

        LogWrite($"Exiting SessionPowerSaver 0x{rc:X}");

        return rc;
    }

    private static void ServiceLoop(IReadOnlyList<int> currentProcessTree, ConcurrentDictionary<int, DateTime> suspended)
    {
        using var sync = new ManualResetEvent(initialState: true);

        SystemEvents.SessionSwitch += (_, _) => sync.Set();

        for (; ; )
        {
            var session_state = WTS.CurrentSessionInfo.SessionState;

            sync.Reset();

            var is_connected = session_state <= ConnectState.Connected;

            LogWrite($"Session is {session_state}");

            Parallel.ForEach(WTS.CurrentSessionProcesses
                .TakeWhile(p => !sync.WaitOne(0))
                .Where(p => p.UserSid == CurrentUser.User &&
                    !currentProcessTree.Contains(p.ProcessId) &&
                    p.TotalProcessorTime.TotalMinutes > 6d), p =>
                    {
                        if (sync.WaitOne(0))
                        {
                            return;
                        }

                        if (suspended.TryGetValue(p.ProcessId, out var item))
                        {
                            using var suspended_process = SafeOpenProcess(p.ProcessId);

                            if (suspended_process is null || item != suspended_process.StartTime)
                            {
                                LogWrite($"Previous process {p.ProcessId} with start time {item} no longer exists");
                                suspended.TryRemove(p.ProcessId, out item);
                            }
                            else if (is_connected)
                            {
                                LogWrite($"Resuming process {p.ProcessId} ({p.ProcessName})");
                                suspended_process.Resume();
                                suspended.TryRemove(p.ProcessId, out item);
                                return;
                            }
                            else
                            {
                                return;
                            }
                        }

                        if (is_connected)
                        {
                            return;
                        }

                        using var process = SafeOpenProcess(p.ProcessId);

                        if (process is null ||
                            process.MainWindowHandle == IntPtr.Zero ||
                            !process.Responding)
                        {
                            return;
                        }

                        var process_time = DateTime.Now - process.StartTime;

                        var cpu = p.TotalProcessorTime.TotalMilliseconds / process_time.TotalMilliseconds;

                        if (cpu > 0.1)
                        {
                            LogWrite($"Process {p.ProcessId} ({p.ProcessName}, '{process.MainWindowTitle}') CPU usage is {100 * cpu:0.0}%. Suspending...");

                            process.Suspend();
                            suspended[p.ProcessId] = process.StartTime;
                        }
                    });

            LogWrite("Waiting");

            if (sync.WaitOne(TimeSpan.FromMinutes(60)))
            {
                LogWrite("Session change");
            }
            else
            {
                LogWrite("Timeout");
            }
        }
    }

    private static Process? SafeOpenProcess(int processId)
    {
        try
        {
            return Process.GetProcessById(processId);
        }
        catch (Exception ex)
        {
            LogWrite($"Failed to open process {processId}: {ex.JoinMessages(" -> ")}");
            return null;
        }
    }
}
