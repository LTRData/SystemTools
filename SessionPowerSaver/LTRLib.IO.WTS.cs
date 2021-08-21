using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace LTRLib.IO.WTS
{
	// LTRLib.IO.WTS.ConnectState
	public enum ConnectState : uint
	{
		Active,
		Connected,
		ConnectQuery,
		Shadow,
		Disconnected,
		Idle,
		Listen,
		Reset,
		Down,
		Init
	}

	// LTRLib.IO.WTS.SessionFlags
	public enum SessionFlags : uint
	{
		Locked = 0u,
		Unlocked = 1u,
		Unknown = 0xFFFFFFFFu
	}

	internal enum InfoClass : uint
	{
		WTSInitialProgram,
		WTSApplicationName,
		WTSWorkingDirectory,
		WTSOEMId,
		WTSSessionId,
		WTSUserName,
		WTSWinStationName,
		WTSDomainName,
		WTSConnectState,
		WTSClientBuildNumber,
		WTSClientName,
		WTSClientDirectory,
		WTSClientProductId,
		WTSClientHardwareId,
		WTSClientAddress,
		WTSClientDisplay,
		WTSClientProtocolType,
		WTSIdleTime,
		WTSLogonTime,
		WTSIncomingBytes,
		WTSOutgoingBytes,
		WTSIncomingFrames,
		WTSOutgoingFrames,
		WTSClientInfo,
		WTSSessionInfo,
		WTSSessionInfoEx,
		WTSConfigInfo,
		WTSValidationInfo,   // Info Class value used to fetch Validation Information through the WTSQuerySessionInformation
		WTSSessionAddressV4,
		WTSIsRemoteSession
	};

	public delegate bool WTSEnumerateFunc(out SafeWTSBuffer buffer, out int count);

	internal static class UnsafeNativeMethods
	{
		[DllImport("WTSAPI32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool WTSQuerySessionInformation(this SafeWTSHandle Server, uint SessionId, InfoClass WTSInfoClass, out SafeWTSBuffer buffer, out int size);

		[DllImport("WTSAPI32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool WTSEnumerateSessions(this SafeWTSHandle Server, uint Reserved, uint Version, out SafeWTSBuffer buffer, out int count);

		[DllImport("WTSAPI32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool WTSEnumerateServers([MarshalAs(UnmanagedType.LPWStr)] string DomainName, uint Reserved, uint Version, out SafeWTSBuffer buffer, out int count);

		[DllImport("WTSAPI32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool WTSEnumerateListeners(IntPtr Server, IntPtr pReserved, uint Reserved, IntPtr buffer, out int count);

		[DllImport("WTSAPI32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool WTSEnumerateProcesses(this SafeWTSHandle Server, uint Reserved, uint Version, out SafeWTSBuffer buffer, out int count);

		[DllImport("WTSAPI32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool WTSTerminateProcess(this SafeWTSHandle Server, uint processId, uint exitCode);

		[DllImport("WTSAPI32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool WTSLogoffSession(this SafeWTSHandle Server, uint sessionId, [MarshalAs(UnmanagedType.Bool)] bool wait);

		[DllImport("WTSAPI32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool WTSDisconnectSession(this SafeWTSHandle Server, uint sessionId, [MarshalAs(UnmanagedType.Bool)] bool wait);

		[DllImport("WTSAPI32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		internal static extern void WTSFreeMemory(IntPtr buffer);

		[DllImport("WTSAPI32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool WTSGetChildSessionId(out uint ChildSessionId);

		[DllImport("WTSAPI32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool WTSIsChildSessionsEnabled([MarshalAs(UnmanagedType.Bool)] out bool Enabled);

		[DllImport("WTSAPI32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		internal static extern SafeWTSHandle WTSOpenServer(string serverName);

		[DllImport("WTSAPI32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		internal static extern void WTSCloseServer(IntPtr buffer);

		[DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool IsValidSid(IntPtr pNativeData);

		[DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		internal static extern int GetLengthSid(IntPtr pNativeData);

		[DllImport("WTSAPI32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		internal static extern bool WTSQueryListenerConfig(IntPtr hServer, IntPtr pReserved, uint Reserved, string listener, [MarshalAs(UnmanagedType.LPStruct), Out] WTSListenerConfig info);
    }

	public class SafeWTSHandle : SafeHandleMinusOneIsInvalid
	{
		public SafeWTSHandle(IntPtr serverHandle, bool ownsHandle) : base(ownsHandle) => handle = serverHandle;

		protected SafeWTSHandle() : base(ownsHandle: true)
		{
		}

		protected override bool ReleaseHandle()
		{
			UnsafeNativeMethods.WTSCloseServer(handle);
			return true;
		}
	}

	public class SafeWTSBuffer : SafeBuffer
	{
		protected SafeWTSBuffer() : base(ownsHandle: true)
		{
		}

		protected override bool ReleaseHandle()
		{
			UnsafeNativeMethods.WTSFreeMemory(handle);
			return true;
		}
	}

	public class WTS : IDisposable
    {
        private bool disposedValue;

		public static SafeWTSHandle LocalServerHandle { get; } = new(IntPtr.Zero, ownsHandle: false);

		public static WTS LocalServer = new();

        public SafeWTSHandle ServerHandle { get; }

        private WTS() => ServerHandle = LocalServerHandle;

        public WTS(string serverName) => ServerHandle = UnsafeNativeMethods.WTSOpenServer(serverName);

        public static WTSIsRemote IsRemoteSession => SessionInfo<WTSIsRemote>.QuerySession(LocalServerHandle, uint.MaxValue);

		public static WTSSessionAddress CurrentSessionAddress => SessionInfo<WTSSessionAddress>.QuerySession(LocalServerHandle, uint.MaxValue);

		public static WTSConfigInfo CurrentConfigInfo => SessionInfo<WTSConfigInfo>.QuerySession(LocalServerHandle, uint.MaxValue);

		public static WTSClient CurrentClient => SessionInfo<WTSClient>.QuerySession(LocalServerHandle, uint.MaxValue);

		public static WTSInfo CurrentSessionInfo => SessionInfo<WTSInfo>.QuerySession(LocalServerHandle, uint.MaxValue);

		public static WTSInfoEx CurrentSessionInfoEx => SessionInfo<WTSInfoEx>.QuerySession(LocalServerHandle, uint.MaxValue);

		public static uint ChildSession => UnsafeNativeMethods.WTSGetChildSessionId(out var sessionId) ? sessionId : throw new Win32Exception();

		public static bool IsChildSessionsEnabled => UnsafeNativeMethods.WTSIsChildSessionsEnabled(out var enabled) ? enabled : throw new Win32Exception();

		public WTSSessionItem[] Sessions => Enumerate<WTSSessionItem>.Query((out SafeWTSBuffer buf, out int count) => ServerHandle.WTSEnumerateSessions(0, 1, out buf, out count)).ToArray();

		public static WTSServerItem[] GetServers(string domain) => Enumerate<WTSServerItem>.Query((out SafeWTSBuffer buf, out int count) => UnsafeNativeMethods.WTSEnumerateServers(domain, 0, 1, out buf, out count)).ToArray();

		private static readonly int _sizeOfListener = Marshal.SizeOf<WTSListenerItem>();

        public unsafe static WTSListenerItem[] Listeners
        {
            get
            {
                UnsafeNativeMethods.WTSEnumerateListeners(IntPtr.Zero, IntPtr.Zero, 0, IntPtr.Zero, out var count);
                
				var buffer = stackalloc byte[count * _sizeOfListener];
				var ptr = new IntPtr(buffer);
                
				if (!UnsafeNativeMethods.WTSEnumerateListeners(IntPtr.Zero, IntPtr.Zero, 0, ptr, out count))
                {
                    throw new Exception("Listener enumeration failed", new Win32Exception());
                }
                
				var items = new WTSListenerItem[count];
                for (var i = 0; i < count; i++)
                {
                    items[i] = Marshal.PtrToStructure<WTSListenerItem>(ptr + (i * _sizeOfListener));
                }
                
				return items;
            }
        }

        public WTSProcessItem[] Processes => Enumerate<WTSProcessItem.NativeWTSProcessItem>.Query(
			(out SafeWTSBuffer buf, out int count) => ServerHandle.WTSEnumerateProcesses(0, 1, out buf, out count))
			.Select(item => new WTSProcessItem(item))
			.ToArray();

		public T QuerySessionInfo<T>(uint sessionId) => SessionInfo<T>.QuerySession(ServerHandle, sessionId);

		public static WTSListenerConfig GetListenerConfig(string listener)
        {
			var info = new WTSListenerConfig();

			if (!UnsafeNativeMethods.WTSQueryListenerConfig(IntPtr.Zero, IntPtr.Zero, 0, listener, info))
			{
				throw new Exception("Query listener configuration failed", new Win32Exception());
			}

			return info;
		}

		public static void LogoffCurrentSession(bool wait)
		{
			if (!LocalServerHandle.WTSLogoffSession(uint.MaxValue, wait))
			{
				throw new Exception("Current session logoff failed", new Win32Exception());
			}
		}

		public static void DisconnectCurrentSession(bool wait)
		{
			if (!LocalServerHandle.WTSDisconnectSession(uint.MaxValue, wait))
			{
				throw new Exception("Current session disconnect failed", new Win32Exception());
			}
		}

		public void TerminateProcess(uint processId, uint exitCode)
		{
			if (!ServerHandle.WTSTerminateProcess(processId, exitCode))
			{
				throw new Exception("Process termination failed", new Win32Exception());
			}
		}

		public void LogoffSession(uint sessionId, bool wait)
		{
			if (!ServerHandle.WTSLogoffSession(sessionId, wait))
			{
				throw new Exception("Session logoff failed", new Win32Exception());
			}
		}

		public void DisconnectSession(uint sessionId, bool wait)
		{
			if (!ServerHandle.WTSDisconnectSession(sessionId, wait))
			{
				throw new Exception("Session disconnect failed", new Win32Exception());
			}
		}

		protected virtual void Dispose(bool disposing)
        {
            if (ServerHandle != LocalServerHandle && !disposedValue)
            {
                if (disposing)
                {
					// TODO: dispose managed state (managed objects)
					ServerHandle.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer

                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~WTS()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

	internal static class Enumerate<T>
    {
		internal static int DataSize = Marshal.SizeOf<T>();

		public static IEnumerable<T> Query(WTSEnumerateFunc EnumFunc)
        {
			if (!EnumFunc(out var pbuf, out var count))
			{
				throw new Exception($"{EnumFunc.Method.Name} failed", new Win32Exception());
			}

			using (pbuf)
			{
				for (var i = 0; i < count; i++)
				{
					var obj = Marshal.PtrToStructure<T>(pbuf.DangerousGetHandle() + (i * DataSize));

					yield return obj;
				}
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public sealed class WTSListenerConfig
	{
		public uint Version { get; }

		public uint EnableListener { get; }

		public uint MaxConnectionCount { get; }

		public uint PromptForPassword { get; }

		public uint InheritColorDepth { get; }

		public uint ColorDepth { get; }

		public uint InheritBrokenTimeoutSettings { get; }

		public uint BrokenTimeoutSettings { get; }

		public uint DisablePrinterRedirection { get; }

		public uint DisableDriveRedirection { get; }

		public uint DisableComPortRedirection { get; }

		public uint DisableLPTPortRedirection { get; }

		public uint DisableClipboardRedirection { get; }

		public uint DisableAudioRedirection { get; }

		public uint DisablePNPRedirection { get; }

		public uint DisableDefaultMainClientPrinter { get; }

		public uint LanAdapter { get; }

		public uint PortNumber { get; }

		public uint InheritShadowSettings { get; }

		public uint ShadowSettings { get; }

		public uint TimeoutSettingsConnection { get; }

		public uint TimeoutSettingsDisconnection { get; }

		public uint TimeoutSettingsIdle { get; }

		public uint SecurityLayer { get; }

		public uint MinEncryptionLevel { get; }

		public uint UserAuthentication { get; }

		[field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 61)]
		public string Comment { get; }

		[field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
		public string LogonUserName { get; }

		[field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 18)]
		public string LogonDomain { get; }

		[field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 261)]
		public string WorkDirectory { get; }

		[field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 261)]
		public string InitialProgram { get; }

		internal WTSListenerConfig()
        {
        }
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public sealed class WTSSessionItem
	{
		public uint SessionId { get; }

		[field: MarshalAs(UnmanagedType.LPWStr)]
		public string WinStationName { get; }

		public ConnectState State { get; }

		public override string ToString() => $"{SessionId} - {WinStationName} - {State}";

		private WTSSessionItem()
        {
        }
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public sealed class WTSListenerItem
	{
		[field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
		public string ListenerName { get; }

		public override string ToString() => ListenerName;

		private WTSListenerItem()
        {
        }
    }

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public sealed class WTSServerItem
	{
		[field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
		public string ServerName { get; }

		public override string ToString() => ServerName;

		private WTSServerItem()
        {
        }
	}

	public sealed class WTSProcessItem
	{
		internal struct NativeWTSProcessItem
        {
			public uint SessionId { get; }

			public uint ProcessId { get; }

			public IntPtr ProcessName { get; }

			public IntPtr Sid { get; }
		}

		public uint SessionId { get; }

		public uint ProcessId { get; }

		public string ProcessName { get; }

		public SecurityIdentifier Sid { get; }

		public NTAccount Account => Sid?.Translate(typeof(NTAccount)) as NTAccount;

		public override string ToString() => $"{SessionId} - {ProcessId} - {ProcessName} - {Sid}";

        internal WTSProcessItem(NativeWTSProcessItem native)
        {
			SessionId = native.SessionId;
			ProcessId = native.ProcessId;
			ProcessName = Marshal.PtrToStringUni(native.ProcessName);
			if (!UnsafeNativeMethods.IsValidSid(native.Sid))
			{
				return;
			}
			var length = UnsafeNativeMethods.GetLengthSid(native.Sid);
			var bytes = new byte[length];
			Marshal.Copy(native.Sid, bytes, 0, length);
			Sid = new SecurityIdentifier(bytes, 0);
		}
	}

    // LTRLib.IO.WTS.WTS
    public static class SessionInfo<T>
	{
		internal static InfoClass InfoClass { get; } =
			(typeof(T).GetCustomAttributes(typeof(InfoClassAttribute), inherit: false).FirstOrDefault() as InfoClassAttribute)?.InfoClass ??
			throw new Exception($"Attribute {nameof(InfoClassAttribute)} missing on type {typeof(T).FullName}");

		private static int DataSize { get; } = Marshal.SizeOf<T>();

		public static T QuerySession(SafeWTSHandle serverHandle, uint sessionId)
		{
			if (!serverHandle.WTSQuerySessionInformation(sessionId, InfoClass, out var pbuf, out var size))
			{
				throw new Exception("Query session information failed", new Win32Exception());
			}

			using (pbuf)
			{
				if (size < DataSize)
				{
					throw new NotSupportedException($"Unexpected size {size} for type {typeof(T).FullName}. Expected: {DataSize}");
				}

				var obj = Marshal.PtrToStructure<T>(pbuf.DangerousGetHandle());

				return obj;
			}
		}
	}

	internal sealed class InfoClassAttribute : Attribute
    {
        public InfoClass InfoClass { get; set; }
    }

	[InfoClass(InfoClass = InfoClass.WTSIsRemoteSession)]
	public struct WTSIsRemote
    {
		[field: MarshalAs(UnmanagedType.U1)]
		public bool IsRemote { get; }

		public override string ToString() => $"IsRemote: {IsRemote}";
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	[InfoClass(InfoClass = InfoClass.WTSSessionAddressV4)]
	public sealed class WTSSessionAddress
	{
		public uint AddressFamily { get; }

		[field: MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
		public byte[] Address { get; }

		private WTSSessionAddress()
		{
		}
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	[InfoClass(InfoClass = InfoClass.WTSConfigInfo)]
	public sealed class WTSConfigInfo
	{
		public uint Version { get; }

		public uint ConnectClientDrivesAtLogon { get; }

		public uint ConnectPrinterAtLogon { get; }

		public uint DisablePrinterRedirection { get; }

		public uint DisableDefaultMainClientPrinter { get; }

		public uint ShadowSettings { get; }

		[field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
		public string LogonUserName { get; }

		[field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 18)]
		public string LogonDomain { get; }

		[field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 261)]
		public string WorkDirectory { get; }

		[field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 261)]
		public string InitialProgram { get; }

		[field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 261)]
		public string ApplicationName { get; }

		private WTSConfigInfo()
		{
		}
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	[InfoClass(InfoClass = InfoClass.WTSClientInfo)]
	public sealed class WTSClient
	{
		[field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
		public string ClientName { get; }

		[field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 18)]
		public string Domain { get; }

		[field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
		public string UserName { get; }

		[field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 261)]
		public string WorkDirectory { get; }

		[field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 261)]
		public string InitialProgram { get; }

		public byte EncryptionLevel { get; }

		public uint ClientAddressFamily { get; }

		[field: MarshalAs(UnmanagedType.ByValArray, SizeConst = 31)]
		public ushort[] ClientAddress { get; }

		public ushort HRes { get; }

		public ushort VRes { get; }

		public ushort ColorDepth { get; }

		[field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 261)]
		public string ClientDirectory { get; }

		public uint ClientBuildNumber { get; }

		public uint ClientHardwareId { get; }

		public ushort ClientProductId { get; }

		public ushort OutBufCountHost { get; }

		public ushort OutBufCountClient { get; }

		public ushort OutBufLength { get; }

		[field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 261)]
		public string DeviceId { get; }

		private WTSClient()
		{
		}

		public override string ToString() => @$"{ClientName} - {Domain}\{UserName}";
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	[InfoClass(InfoClass = InfoClass.WTSSessionInfo)]
	public sealed class WTSInfo
	{
		public ConnectState SessionState { get; }

		public uint SessionId { get; }

		public uint IncomingBytes { get; }

		public uint OutgoingBytes { get; }

		public uint IncomingFrames { get; }

		public uint OutgoingFrames { get; }

		public uint IncomingCompressedBytes { get; }

		public uint OutgoingCompressedBytes { get; }

		[field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string WinStationName { get; }

		[field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 17)]
		public string DomainName { get; }

		[field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
		public string UserName { get; }

		public long ConnectTime { get; }

		public long DisconnectTime { get; }

		public long LastInputTime { get; }

		public long LogonTime { get; }

		public long CurrentTime { get; }

		private WTSInfo()
		{
		}

		public override string ToString() => @$"{SessionId} - {DomainName}\{UserName} - {SessionState}";
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	[InfoClass(InfoClass = InfoClass.WTSSessionInfoEx)]
	public sealed class WTSInfoEx
	{
		public uint Level { get; }

		public uint Reserved { get; }

		public uint SessionId { get; }

		public ConnectState SessionState { get; }

		public SessionFlags SessionFlags { get; }

		[field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
		public string WinStationName { get; }

		[field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
		public string UserName { get; }

		[field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 18)]
		public string DomainName { get; }

		public long LogonTime { get; }

		public long ConnectTime { get; }

		public long DisconnectTime { get; }

		public long LastInputTime { get; }

		public long CurrentTime { get; }

		public uint IncomingBytes { get; }

		public uint OutgoingBytes { get; }

		public uint IncomingFrames { get; }

		public uint OutgoingFrames { get; }

		public uint IncomingCompressedBytes { get; }

		public uint OutgoingCompressedBytes { get; }

		private WTSInfoEx()
		{
		}

		public override string ToString() => @$"{SessionId} - {DomainName}\{UserName} - {SessionState}";
	}

}