using LTRLib.Extensions;
using LTRLib.IO;
using LTRLib.LTRGeneric;
using RTools_NTS.Util;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace StripeBlockSwap;

public static class Program
{
    public static async Task<int> Main(params string[] args)
    {
        try
        {
            using var tokenSource = new CancellationTokenSource();

            Console.CancelKeyPress += (sender, e) =>
            {
                tokenSource.Cancel();
                e.Cancel = true;
            };

            var token = tokenSource.Token;

            return await MainAsync(args, token).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(ex.JoinMessages());
            Console.ResetColor();
            return ex.HResult;
        }
    }

    public static async Task<int> MainAsync(string[] args, CancellationToken cancellationToken)
    {
        var cmds = StringSupport.ParseCommandLine(args, StringComparer.Ordinal);

        var blockSize = 0L;
        var inoffset = 0L;
        var outoffset = 0L;

        string? infile = null;
        string? outfile = null;

        foreach (var cmd in cmds)
        {
            if (cmd.Key == "bsize" && cmd.Value.Length == 1 &&
                StringSupport.TryParseSuffixedSize(cmd.Value[0], out blockSize))
            {
            }
            else if (cmd.Key == "inoffset" && cmd.Value.Length == 1 &&
                StringSupport.TryParseSuffixedSize(cmd.Value[0], out inoffset))
            {
            }
            else if (cmd.Key == "outoffset" && cmd.Value.Length == 1 &&
                StringSupport.TryParseSuffixedSize(cmd.Value[0], out outoffset))
            {
            }
            else if (cmd.Key == "in" && cmd.Value.Length == 1)
            {
                infile = cmd.Value[0];
            }
            else if (cmd.Key == "out" && cmd.Value.Length == 1)
            {
                outfile = cmd.Value[0];
            }
            else
            {
                Console.WriteLine(@"Syntax:
StripeBlockSwap --bsize=blocksize [--inoffset=inputoffset] [--outoffset=outputoffset] [--in=inputfile] [--out=outputfile]");

                return 100;
            }
        }

        if (blockSize <= 0)
        {
            Console.WriteLine(@"Syntax:
StripeBlockSwap --bsize=blocksize [--inoffset=inputoffset] [--outoffset=outputoffset] [--in=inputfile] [--out=outputfile]");

            return 100;
        }

        cancellationToken.ThrowIfCancellationRequested();

        var buffer = new byte[blockSize];

        Console.Error.WriteLine($"Using block size {blockSize} bytes");

        Console.Error.WriteLine($"Opening input file {(infile is not null and not "-" ? infile : "stdin")}");
        
        using var input = infile is null or "-"
            ? Console.OpenStandardInput()
            : OperatingSystem.IsWindows() && infile.StartsWith(@"\\?\", StringComparison.Ordinal)
            ? new DiskStream(infile, FileAccess.Read)
            : File.OpenRead(infile);

        if (inoffset != 0)
        {
            Console.Error.WriteLine($"Starting at input offset {inoffset}");
            input.Seek(inoffset, SeekOrigin.Current);
        }

        cancellationToken.ThrowIfCancellationRequested();

        Console.Error.WriteLine($"Opening output file {(outfile is not null and not "-" ? outfile : "stdout")}");

        using var output = outfile is null or "-"
            ? Console.OpenStandardOutput()
            : OperatingSystem.IsWindows() && outfile.StartsWith(@"\\?\", StringComparison.Ordinal)
            ? new DiskStream(outfile, FileAccess.ReadWrite)
            : outoffset == 0
            ? File.Create(outfile)
            : File.OpenWrite(outfile);

        if (outoffset != 0)
        {
            Console.Error.WriteLine($"Starting at output offset {outoffset}");
        }

        for (var blockCount = 0L; ; blockCount++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Console.Error.Write($"Reading block {blockCount}...\r");

            var length = await input.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);

            if (length == 0)
            {
                Console.Error.WriteLine();
                Console.Error.WriteLine($"Done, wrote {blockCount * blockSize} bytes.");
                break;
            }

            if ((blockCount & 1) == 0)
            {
                output.Position = outoffset + blockSize * (blockCount + 1);
            }
            else
            {
                output.Position = outoffset + blockSize * (blockCount - 1);
            }

            cancellationToken.ThrowIfCancellationRequested();

            Console.Error.Write($"Writing block {blockCount}...\r");

            await output.WriteAsync(buffer.AsMemory(0, length), cancellationToken).ConfigureAwait(false);

            if (length < blockSize)
            {
                Console.Error.WriteLine();
                Console.Error.WriteLine($"Done, wrote {blockCount * blockSize + length} bytes.");
                break;
            }
        }

        return 0;
    }
}