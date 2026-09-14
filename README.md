# SystemTools

A collection of .NET utilities from LTR Data for system inspection, disk-image
operations, networking, process/window waits, and Windows session automation.
Each tool is a separate executable project.

## Tools

| Tool | Purpose | Platform and requirements |
| --- | --- | --- |
| [AppxActivate](AppxActivate) | Activate a packaged Windows application by its AppUserModelID. Returns the activated process ID as its exit code. | Windows application-activation COM API; .NET Framework 4.6. |
| [DiskVolumes](DiskVolumes) | List Windows volumes, device-object names and links, mount points, and physical disk extents. `/S` includes contained volume mount points. | Windows native volume APIs. |
| [GetProductKey](GetProductKey) | Display Windows installation details and stored product-key data from a running system, offline installation, or disk/ISO/WIM image. | Live local/remote registry queries require Windows; the local firmware-key query uses CIM. Image parsing uses DiscUtils. |
| [ImageMBR2GPT](ImageMBR2GPT) | Inspect a partition table or replace it in place with an MBR or GPT table using the existing partition extents. | DiscUtils disk images and raw files; also handles Windows physical-drive paths. See conversion limits below. |
| [PowerShellFs](PowerShellFs) | Expose PowerShell drives through a read-only filesystem mounted at `Q:`. Can use a local runspace or a remote host over WSMan. | Windows, .NET Framework 4.8, Windows PowerShell 5, and a Dokany installation compatible with the restored DokanNet package. |
| [RecentCleanup](RecentCleanup) | Repeatedly remove shortcuts from the current user's Recent folder when their targets are not found. | Windows Script Host COM interop; includes an interop assembly in the project. |
| [SessionPowerSaver](SessionPowerSaver) | Suspend selected CPU-consuming GUI processes in a disconnected user session and resume tracked processes when it reconnects. | Windows session/process APIs; runs for the current user and session. |
| [StripeBlockSwap](StripeBlockSwap) | Copy data while exchanging the positions of adjacent blocks of a specified size. | File/stream operations; output must be seekable. Windows device paths also use native disk access. |
| [dssearch](dssearch) | Search Directory Services with LDAP filters and print matching objects' properties, including formatted GUIDs and SIDs. | Windows Directory Services and an accessible directory context; defaults to the current user's parent container. |
| [ipcalc](ipcalc) | Calculate IPv4 network, broadcast, mask, and prefix information from an address range, CIDR prefix, or explicit mask. | Managed networking code using `LTRData.Net`. |
| [telnets](telnets) | Connect an external telnet client to a TLS-protected remote endpoint through a local loopback forwarder. | Requires a telnet executable and a server accepting TLS immediately on connection. Default port: 992. |
| [waitps](waitps) | Wait for existing processes selected by PID or name; wait for all by default, or any with `-a`. | .NET process APIs and permission to inspect the selected processes. |
| [waitwin](waitwin) | Wait for existing windows to close, selected by title or numeric handle; supports `-a` to wait for any. | Windows UI Automation and User32. |

ODBC Dataviewer has moved to
[LTRData/WindowsTools](https://github.com/LTRData/WindowsTools).

## Build and run

Use a .NET 10 SDK for the current .NET 10 targets. For example:

```sh
git clone https://github.com/LTRData/SystemTools.git
cd SystemTools
dotnet build ipcalc/ipcalc.csproj -c Release -f net10.0
dotnet Release/net10.0/ipcalc.dll 192.0.2.25/24
```

All projects except AppxActivate and PowerShellFs declare .NET 8, 9, and 10
targets. SessionPowerSaver and waitwin use the corresponding
`net8.0-windows`, `net9.0-windows`, and `net10.0-windows` names.
AppxActivate targets `net46` only; PowerShellFs targets `net48` only.
Most other tools also retain .NET Framework targets, and ipcalc additionally
declares `netstandard2.1`. Check each project file for its exact target list.

Build individual projects with `-f` to select a declared framework. Windows
tools require Windows at runtime even when their framework name has no
`-windows` suffix. The managed file/network tools have fewer OS dependencies;
this repository does not contain an automated cross-platform test matrix.

[Directory.Build.props](Directory.Build.props) places outputs in the root
`Release/<framework>/` or `Debug/<framework>/` directory.
[SystemTools.slnx](SystemTools.slnx) includes all 13 projects and needs an
IDE/MSBuild version that understands `.slnx`.

For .NET Framework builds, install the relevant reference assemblies. For
Windows desktop targets, use the Windows desktop build components. PowerShellFs
also needs the native Dokany runtime when mounting; restoring its managed NuGet
references alone does not install the driver. Many package references use
floating versions (`*` or a version range), so dependency resolution can change
between restores.

There is currently no test project or CI workflow in this repository.

## Usage examples

After building the corresponding projects for `net10.0`:

```sh
dotnet Release/net10.0/ipcalc.dll 192.0.2.0-192.0.2.255
dotnet Release/net10.0/ipcalc.dll "192.0.2.25%255.255.255.0"
dotnet Release/net10.0/ImageMBR2GPT.dll disk.vhdx
dotnet Release/net10.0/waitps.dll -a 1234 5678
dotnet Release/net10.0/telnets.dll server.example.com 992
```

The ImageMBR2GPT example only displays the existing layout. Replace the example
PIDs and server name with real targets.

On Windows, after building DiskVolumes for `net10.0`:

```powershell
dotnet Release/net10.0/DiskVolumes.dll /S
```

PowerShellFs uses its .NET Framework executable:

```powershell
dotnet build PowerShellFs/PowerShellFs.csproj -c Release -f net48
& .\Release\net48\PowerShellFs.exe
```

With no arguments it uses a local runspace; supplying a host name selects a
remote WSMan runspace and requires working PowerShell remoting. The mount point
is fixed at `Q:`; Ctrl+C removes it. Writes are not implemented, and readable
content depends on the provider/object type.

Options differ by tool. Examples of help commands are `ImageMBR2GPT --help`,
`StripeBlockSwap --help`, `waitps --help`, `waitwin --help`, and
`GetProductKey /?`. Each project's `Program.cs` contains its argument handling;
DiskVolumes delegates to [DiskVolumes.cs](DiskVolumes/DiskVolumes.cs).

## Behavior to know before use

**Disk operations:** ImageMBR2GPT's `--gpt` and `--mbr` options replace the
partition table after an interactive confirmation. It recreates partition
extents but does not migrate boot files or configure Windows/UEFI booting.
Partition types are mapped with limited rules; GPT-to-MBR skips Microsoft
Reserved partitions and rejects more than four remaining partitions. Work on
a copy of an image before converting it.

StripeBlockSwap writes reordered blocks to the destination. A normal output
file is truncated when the output offset is zero; choose a separate destination.
The implementation seeks for every output block, so an ordinary stdout pipe
cannot serve as its output despite the default stdout option.

**Background utilities:** RecentCleanup is a continuing watcher, and missing
targets include paths on unavailable removable or network storage. It deletes
the shortcut, not the target. `--trace` enables console diagnostics.
SessionPowerSaver actually suspends processes. Its selection uses accumulated
CPU time and lifetime-average CPU usage, and it writes
`Session_<session-id>.log` in the user's profile.

**TLS forwarding:** telnets uses `SslStream` with normal certificate validation.
It starts a separate telnet executable, selected through the `TELNET` environment
variable or the platform default (`telnet.exe` in the Windows system directory,
`telnet` elsewhere). Options before the host are passed to that client; when
supplying options, also supply the port. It does not perform STARTTLS negotiation.

**Wait results:** waitps and waitwin find their targets when invoked; they do
not wait for a future process or window to appear. On success, waitps returns
the number of selected processes observed to have exited, so a positive exit
code is expected. With `-a`, waitwin returns the zero-based index of the signaled
wait handle; otherwise its normal result is zero.
