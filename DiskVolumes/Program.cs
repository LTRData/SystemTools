using System;
using System.Diagnostics;

namespace DiskVolumes
{
    public static class Program
    {
        public static int Main(params string[] args)
        {
            try
            {
                return DiskVolumes.UnsafeMain(args);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.ToString());
                Console.ResetColor();
                return -1;
            }
        }
    }
}
