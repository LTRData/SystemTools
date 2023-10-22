using LTRData.Extensions.Formatting;
using LTRData.Extensions.Split;
using LTRLib.Net;
using System;
using System.Net;
using System.Net.Sockets;

namespace ipcalc;

public static class Program
{
    public static int Main(params string[] args)
    {
        foreach (var arg in args)
        {
            try
            {
                CalculateAddress(arg.AsSpan());
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine(ex.JoinMessages());
                Console.ResetColor();
            }
        }

        return 0;
    }

#if NETCOREAPP
    private static IPAddress ParseIP(ReadOnlySpan<char> address) => IPAddress.Parse(address);

    private static byte ParseByte(ReadOnlySpan<char> chars) => byte.Parse(chars);
#else
    private static IPAddress ParseIP(ReadOnlySpan<char> address) => IPAddress.Parse(address.ToString());

    private static byte ParseByte(ReadOnlySpan<char> chars) => byte.Parse(chars.ToString());
#endif

    public static void CalculateAddress(ReadOnlySpan<char> arg)
    {
        var startAddress = arg.Split('-').ElementAtOrDefault(0);
        var endAddress = arg.Split('-').ElementAtOrDefault(1);
        var netAddress = arg.Split('/').ElementAtOrDefault(0);
        var bitCount = arg.Split('/').ElementAtOrDefault(1);

        var ranges = new IPAddressRanges(AddressFamily.InterNetwork);

        (IPAddress Network, IPAddress Mask, IPAddress Broadcast, byte BitCount) network;

        if (!startAddress.IsEmpty && !endAddress.IsEmpty && bitCount.IsEmpty)
        {
             network = ranges.CalculateNetwork(ParseIP(startAddress), ParseIP(endAddress));
        }
        else if (!netAddress.IsEmpty && !bitCount.IsEmpty && endAddress.IsEmpty)
        {
            network = ranges.CalculateNetwork(ParseIP(netAddress), ParseByte(bitCount));
        }
        else
        {
            throw new ArgumentException("Invalid address range syntax");
        }

        Console.WriteLine(@$"Network: {network.Network}/{network.BitCount} - {network.Broadcast} mask {network.Mask}");
    }
}
