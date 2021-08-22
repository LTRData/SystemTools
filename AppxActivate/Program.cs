using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Linq;
using System.Collections.Generic;

namespace Win8
{

    [ComImport, Guid("2e941141-7f97-4756-ba1d-9decde894a3d"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IApplicationActivationManager
    {
        IntPtr ActivateApplication(string appUserModelId, string arguments, uint options, out uint processId);
    }

    [ComImport, Guid("45BA127D-10A8-46EA-8AB7-56EA9078943C")]//Application Activation Manager
    public class ApplicationActivationManager : IApplicationActivationManager
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)/*, PreserveSig*/]
        public extern IntPtr ActivateApplication(string appUserModelId, string arguments, uint options, out uint processId);

    }

    public static class Program
    {
        private static IEnumerable<string> EnumerateMessages(this Exception ex)
        {
            while (ex != null)
            {
                yield return ex.Message;
                ex = ex.InnerException;
            }
        }

        public static int Main(params string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Package name required.");
                return -1;
            }

            try
            {
                var appman = new ApplicationActivationManager();

                string app_args = null;

                if (args.Length > 2)
                {
                    app_args = string.Join(" ", args.Skip(1).Select(p => $"\"{p}\""));
                }

                appman.ActivateApplication(args[0],
                    app_args,
                    0,
                    out var pid);

                return unchecked((int)pid);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    string.Join(" -> ", ex.EnumerateMessages()));

                return ex.HResult;
            }
        }
    }
}
