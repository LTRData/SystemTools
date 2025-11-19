using DokanNet;
using DokanNet.Logging;
using System;
using System.Linq;
using System.Management.Automation.Runspaces;

namespace PowerShellFs;

public static class Program
{
    public static void Main(params string[] args)
    {
        using var dokan = new Dokan(new NullLogger());

        RunspaceConnectionInfo? ci = null;

        if (args?.FirstOrDefault() is string host)
        {
            ci = new WSManConnectionInfo
            {
                ComputerName = args[0],
                AuthenticationMechanism = AuthenticationMechanism.Default
            };
        }
        
        using var runspace = ci is not null ? RunspaceFactory.CreateRunspace(ci) : RunspaceFactory.CreateRunspace();

        var fs = new PowerShellFs(runspace);

        Console.CancelKeyPress += (sender, e) =>
        {
            dokan.RemoveMountPoint("Q:");
            e.Cancel = true;
        };

        var builder = new DokanInstanceBuilder(dokan);

        builder.ConfigureOptions(option =>
        {
            option.MountPoint = "Q:";
            option.Options = DokanOptions.WriteProtection;
        });

        using var instance = builder.Build(fs);

        instance.WaitForFileSystemClosed(uint.MaxValue);
    }
}
