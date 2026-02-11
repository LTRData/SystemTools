using LTRData.Extensions.CommandLine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Automation;

namespace waitwin;

public static partial class Program
{
#if NET7_0_OR_GREATER
    [LibraryImport("user32", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    private static partial nint FindWindowW(string? lpClassName, string lpWindowName);
#else
    [DllImport("user32", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern nint FindWindowW(string? lpClassName, string lpWindowName);
#endif

    public static int Main(params string[] args)
    {
        var cmds = CommandLineParser.ParseCommandLine(args, StringComparer.Ordinal);

        var waitAny = false;

        string[] windows = [];

        foreach (var cmd in cmds)
        {
            switch (cmd.Key)
            {
                case "a":
                    waitAny = true;
                    break;

                case "":
                    windows = cmd.Value;
                    break;

                default:
                    Console.WriteLine($@"Window wait utility - Copyright (c) 2025-2026 Olof Lagerkvist, LTR Data
https://ltr-data.se/opencode.html

Syntax:

waitwin [-a] winTitle1 [winTitle2 ...]

-a      Wait for any of specified windows to close. Otherwise, waits for all
        of specified windows to close.

Windows can be specified as numeric window handles or window title strings.");

                    return -1;
            }
        }

        var waits = new List<EventWaitHandle>();

        foreach (var window in windows)
        {
            if (!int.TryParse(window, out var hwnd))
            {
                var windowTitle = window;
                string? className = null;
                var delim = windowTitle.IndexOf('\\');
                if (delim > 0)
                {
                    className = windowTitle.Substring(0, delim);
                    windowTitle = windowTitle.Substring(delim + 1);
                }

                hwnd = (int)FindWindowW(className, windowTitle);
            }

            if (hwnd == 0)
            {
                Console.WriteLine($"Window '{window}' not found.");
                continue;
            }

            Console.WriteLine($"{hwnd,7} {window}");

            var element = AutomationElement.FromHandle((nint)hwnd);

            var wait = new ManualResetEvent(initialState: false);

            Automation.AddAutomationEventHandler(
                WindowPattern.WindowClosedEvent,
                element,
                TreeScope.Subtree,
                (_, _) => wait.Set());

            waits.Add(wait);
        }

        if (waits.Count == 0)
        {
            return 0;
        }

        if (waits.Count == 1)
        {
            Console.WriteLine("Waiting for close.");
        }
        else if (waitAny)
        {
            Console.WriteLine("Waiting for any to close.");
        }
        else
        {
            Console.WriteLine("Waiting for all to close.");
        }

        var i = 0;

        if (waitAny)
        {
            i = WaitHandle.WaitAny([.. waits]);
        }
        else
        {
            WaitHandle.WaitAll([.. waits]);
        }

        Console.WriteLine("Closed.");

        Automation.RemoveAllEventHandlers();

        waits.ForEach(wait => wait.Close());

        return i;
    }
}
