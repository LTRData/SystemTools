using LTRData.Extensions.CommandLine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace waitps;

public static class Program
{
    public static int Main(params string[] args)
    {
        var cmds = CommandLineParser.ParseCommandLine(args, StringComparer.Ordinal);

        var waitAny = false;

        string[] processes = [];

        foreach (var cmd in cmds)
        {
            switch (cmd.Key)
            {
                case "a":
                    waitAny = true;
                    break;

                case "":
                    processes = cmd.Value;
                    break;

                default:
                    Console.WriteLine($@"Process wait utility - Copyright (c) 2024 Olof Lagerkvist, LTR Data
https://ltr-data.se/opencode.html

Syntax:

waitps [-a] pid1 [pid2 ...]

-a      Wait for any of specified processes to exit. Otherwise, waits for all
        of specified processes to exit.

Processes can be specified as numeric process id or process name.");

                    return -1;
            }
        }

        var pslist = new List<Process>();

		try
		{
            foreach (var process in processes)
			{
				if (int.TryParse(process, out var pid))
				{
					pslist.Add(Process.GetProcessById(pid));
				}
				else
                {
                    pslist.AddRange(Process.GetProcessesByName(process));
                }
            }

            if (pslist.Count == 0)
            {
                Console.WriteLine("No process found");
                return 0;
            }

            foreach (var ps in pslist)
            {
                Console.WriteLine($"{ps.Id,7} {ps.ProcessName}");
            }

            if (pslist.Count == 1)
            {
                Console.WriteLine("Waiting for exit.");
            }
            else if (waitAny)
            {
                Console.WriteLine("Waiting for any to exit.");
            }
            else
            {
                Console.WriteLine("Waiting for all to exit.");
            }

            if (waitAny)
            {
                var tasks = pslist
                    .Select(ps => ps.WaitForExitAsync())
                    .ToArray();

                Task.WaitAny(tasks);
            }
            else
            {
                foreach (var ps in pslist)
                {
                    ps.WaitForExit();
                }
            }

            var psExitCount = 0;

            foreach (var ps in pslist)
            {
                if (ps.HasExited)
                {
                    psExitCount++;
                    
                    Console.WriteLine($"{ps.Id,7} exit");
                }
            }

            return psExitCount;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(ex.GetBaseException().Message);
            Console.ResetColor();

            return ex.HResult;
        }
        finally
		{
			pslist.ForEach(ps => ps.Dispose());
		}
    }

#if !NET5_0_OR_GREATER
    private static Task WaitForExitAsync(this Process ps)
        => Task.Run(ps.WaitForExit);
#endif
}