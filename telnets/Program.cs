using LTRLib.Extensions;
using LTRLib.IO;
using LTRLib.LTRGeneric;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable IDE0056 // Use index operator

namespace telnets
{
    public static class Program
    {
        public static readonly string telnet_exe = Environment.GetEnvironmentVariable("TELNET") ?? Path.Combine(Environment.SystemDirectory, "telnet.exe");

        public static void WriteError(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(msg);
            Console.ResetColor();
        }

        public static int Main(string[] args)
        {
            var dispobjs = new DisposableList<IDisposable>();

            try
            {
                if (args == null || args.Length < 1)
                {
                    WriteError("Syntax: telnets [options] host [port]");
                    return -1;
                }

                string remote_host;
                int remote_port = 992;
                var options = Enumerable.Empty<string>();

                if (args.Length >= 2)
                {
                    remote_port = int.Parse(args[args.Length - 1]);
                    remote_host = args[args.Length - 2];
                    options = args.Take(args.Length - 2);
                }
                else
                {
                    remote_host = args[args.Length - 1];
                }

                Console.WriteLine($"Connecting to '{remote_host}' at port {remote_port}");

                var remote_socket = IOSupport.OpenTcpIpSocket(remote_host, remote_port);
                remote_socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
                var remote_raw_stream = new NetworkStream(remote_socket);

                dispobjs.Add(remote_raw_stream);

                Console.WriteLine("Connected, applying security");

                var remote_stream = new SslStream(remote_raw_stream);

                dispobjs.Insert(0, remote_stream);

                remote_stream.AuthenticateAsClient(remote_host);

                Console.WriteLine("Setting up local forwarder");

                var local_listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                dispobjs.Add(local_listener);

                local_listener.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                var local_port = (local_listener.LocalEndPoint as IPEndPoint).Port;
                local_listener.Listen(1);

                var telnet_arguments = options.Concat(new[] { IPAddress.Loopback.ToString(), local_port.ToString() }).Join(" ");

                Console.WriteLine($"Starting '{telnet_exe}' with arguments '{telnet_arguments}'");

                var telnet_start_info = new ProcessStartInfo
                {
                    FileName = telnet_exe,
                    Arguments = telnet_arguments,
                    UseShellExecute = false                    
                };

                var telnet_ps = Process.Start(telnet_start_info);

                dispobjs.Add(telnet_ps);

                var local_socket = local_listener.Accept();
                local_listener.Close();
                local_socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);

                var local_stream = new NetworkStream(local_socket);

                dispobjs.Add(local_stream);

                var threads = new List<Thread>
                {
                    CreateForwarder("Inbound", remote_stream, local_stream, () => local_socket.Shutdown(SocketShutdown.Send)),
                    CreateForwarder("Outbound", local_stream, remote_stream, () => remote_socket.Shutdown(SocketShutdown.Send))
                };
                threads.ForEach(t => t.Start());

                threads.ForEach(t => t.Join());

                telnet_ps.WaitForExit();

                return telnet_ps.ExitCode;
            }
            catch (Exception ex)
            {
                WriteError(ex.JoinMessages());
                return 1;
            }
            finally
            {
                dispobjs.Dispose();
            }
        }

        public static Thread CreateForwarder(string name, Stream source_stream, Stream target_stream, Action finalizer)
        {
            return new Thread(() =>
            {
                try
                {
                    source_stream.CopyTo(target_stream);
                }
                catch (IOException)
                {
                }
                catch (ObjectDisposedException)
                {
                }
                finally
                {
                    finalizer?.Invoke();
                }
            })
            {
                Name = name,
                IsBackground = true
            };
        }
    }
}
