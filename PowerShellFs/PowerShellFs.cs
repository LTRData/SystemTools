using DiscUtils.Internal;
using DokanNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security.AccessControl;
using System.Text;

namespace PowerShellFs;

public class PowerShellFs : IDokanOperations
{
    public Runspace Runspace { get; }

    private PowerShell GetPS()
    {
        var ps = PowerShell.Create();
        ps.Runspace = Runspace;
        return ps;
    }

    public PowerShellFs(Runspace runspace)
    {
        runspace.Open();
        Runspace = runspace;
    }

    private sealed class FileObject
    {
        public byte[] FileData;
    }

    public NtStatus CreateFile(ReadOnlySpan<char> fileNamePtr, NativeFileAccess access, FileShare share, FileMode mode, FileOptions options, FileAttributes attributes, ref DokanFileInfo info)
    {
        var path = TranslatePath(fileNamePtr);

        if (string.IsNullOrWhiteSpace(path))
        {
            return NtStatus.Success;
        }

        PSObject finfo;

        lock (Runspace)
        {
            using var ps = GetPS();

            finfo = ps.AddCommand("Get-Item").AddParameter("LiteralPath", path).Invoke().FirstOrDefault();
        }

        if (finfo is null && path.EndsWith(@":\", StringComparison.Ordinal))
        {
            lock (Runspace)
            {
                using var ps = GetPS();

                finfo = ps.AddCommand("Get-PSDrive").AddParameter("Name", path.Remove(path.Length - 2)).Invoke().FirstOrDefault();
            }
        }

        if (finfo is null)
        {
            return NtStatus.ObjectNameNotFound;
        }

        if (mode != FileMode.Open &&
            (!Enum.TryParse<FileAttributes>(finfo.Properties["Attributes"]?.Value?.ToString(), out var attr) ||
            (attr.HasFlag(FileAttributes.Directory) ^ info.IsDirectory)))
        {
            return NtStatus.ObjectNameCollision;
        }

        if ((access & (NativeFileAccess.ReadData | NativeFileAccess.GenericRead | NativeFileAccess.WriteData | NativeFileAccess.GenericWrite)) != 0)
        {
            info.Context = new FileObject();
        }

        return NtStatus.Success;
    }

    public void Cleanup(ReadOnlySpan<char> fileNamePtr, ref DokanFileInfo info)
    {
        if (info.Context is IDisposable context)
        {
            context.Dispose();
            info.Context = null;
        }
    }

    public void CloseFile(ReadOnlySpan<char> fileNamePtr, ref DokanFileInfo info)
    {
    }

    public NtStatus ReadFile(ReadOnlySpan<char> fileNamePtr, Span<byte> buffer, out int bytesRead, long offset, in DokanFileInfo info)
    {
        var path = TranslatePath(fileNamePtr);

        bytesRead = 0;

        if (info.Context is not FileObject fileObject)
        {
            return NtStatus.NotImplemented;
        }

        var filedata = fileObject.FileData;

        if (filedata is null)
        {
            lock (fileObject)
            {
                filedata = fileObject.FileData;

                if (filedata is null)
                {
                    lock (Runspace)
                    {
                        using var ps = GetPS();

                        var finfo = ps.AddCommand("Get-Item").AddParameter("LiteralPath", path).Invoke().FirstOrDefault();

                        if (finfo.BaseObject is FileInfo fileInfo)
                        {
                            filedata = File.ReadAllBytes(fileInfo.FullName);
                        }
                        else if (finfo.Properties["RawData"]?.Value is byte[] rawData)
                        {
                            filedata = rawData;
                        }
                        else if (finfo.BaseObject is DictionaryEntry entry)
                        {
                            if (entry.Value is null)
                            {
                                filedata = Array.Empty<byte>();
                            }
                            else if (entry.Value is byte[] byteData)
                            {
                                filedata = byteData;
                            }
                            else
                            {
                                filedata = Encoding.Default.GetBytes(entry.Value.ToString());
                            }
                        }
                    }

                    fileObject.FileData = filedata;
                }
            }
        }

        if (filedata is null)
        {
            return NtStatus.InvalidDeviceRequest;
        }

        if (offset > filedata.Length)
        {
            return NtStatus.Success;
        }

        var size = (int)Math.Min(filedata.Length - offset, buffer.Length);

        filedata.AsSpan((int)offset, size).CopyTo(buffer);
        bytesRead = size;
        return NtStatus.Success;
    }

    public NtStatus WriteFile(ReadOnlySpan<char> fileNamePtr, ReadOnlySpan<byte> buffer, out int bytesWritten, long offset, in DokanFileInfo info)
    {
        bytesWritten = 0;
        return NtStatus.NotImplemented;
    }

    public NtStatus FlushFileBuffers(ReadOnlySpan<char> fileNamePtr, in DokanFileInfo info) => NtStatus.Success;

    public NtStatus GetFileInformation(ReadOnlySpan<char> fileNamePtr, out ByHandleFileInformation fileInfo, in DokanFileInfo info)
    {
        var path = TranslatePath(fileNamePtr);

        if (string.IsNullOrWhiteSpace(path))
        {
            fileInfo = new ByHandleFileInformation
            {
                Attributes = FileAttributes.Directory | FileAttributes.System
            };

            return NtStatus.Success;
        }

        PSMemberInfoCollection<PSPropertyInfo> finfo;

        lock (Runspace)
        {
            using var ps = GetPS();

            finfo = ps.AddCommand("Get-Item").AddParameter("LiteralPath", path).Invoke()
                .FirstOrDefault()?.Properties;
        }

        if (finfo is null && path.EndsWith(@":\", StringComparison.Ordinal))
        {
            lock (Runspace)
            {
                using var ps = GetPS();

                finfo = ps.AddCommand("Get-PSDrive").AddParameter("Name", path.Remove(path.Length - 2)).Invoke()
                    .FirstOrDefault()?.Properties;
            }

            if (finfo is not null)
            {
                fileInfo = new ByHandleFileInformation
                {
                    Attributes = FileAttributes.Directory
                };

                return NtStatus.Success;
            }
        }

        if (finfo is null)
        {
            fileInfo = default;
            return NtStatus.ObjectNameNotFound;
        }

        fileInfo = new ByHandleFileInformation
        {
            Attributes = Enum.TryParse<FileAttributes>(finfo["Attributes"]?.Value?.ToString(), out var attr) ? attr :
                    finfo["PSIsContainer"]?.Value as bool? ?? false ? FileAttributes.Directory : FileAttributes.Normal,
            CreationTime = finfo["CreationTime"]?.Value as DateTime? ?? finfo["NotBefore"]?.Value as DateTime?,
            LastAccessTime = finfo["LastAccessTime"]?.Value as DateTime? ?? finfo["NotBefore"]?.Value as DateTime?,
            LastWriteTime = finfo["LastWriteTime"]?.Value as DateTime? ?? finfo["NotBefore"]?.Value as DateTime?,
            Length = finfo["Length"]?.Value as long? ?? (finfo["RawData"]?.Value as byte[])?.Length ?? (finfo["Value"]?.Value as string)?.Length ?? 0
        };

        return NtStatus.Success;
    }

    public NtStatus FindFiles(ReadOnlySpan<char> fileNamePtr, out IEnumerable<FindFileInformation> files, in DokanFileInfo info) => FindFilesWithPattern(fileNamePtr, "*".AsSpan(), out files, info);

    public static string TranslatePath(ReadOnlySpan<char> path)
    {
        if (path.Length > 0 && path[0] == '\\')
        {
            path = path.Slice(1);
        }

        if (path.IsWhiteSpace())
        {
            return string.Empty;
        }

        var firstDirDelimiter = path.IndexOf('\\');

        if (firstDirDelimiter >= 0)
        {
            return path.ToString().Insert(firstDirDelimiter, ":");
        }
        else
        {
            return path.ToString() + @":\";
        }
    }

    public NtStatus FindFilesWithPattern(ReadOnlySpan<char> fileNamePtr, ReadOnlySpan<char> searchPatternPtr, out IEnumerable<FindFileInformation> files, in DokanFileInfo info)
    {
        var searchPattern = searchPatternPtr.ToString().Replace('<', '*');

        var path = TranslatePath(fileNamePtr);

        if (path.IndexOfAny(new[] { '?', '*' }) < 0)
        {
            path = Path.Combine(path, searchPattern);
        }

        lock (Runspace)
        {
            using var ps = GetPS();

            if (path.IndexOf(@":\", StringComparison.Ordinal) < 0 ||
                path.EndsWith(@":\", StringComparison.Ordinal))
            {
                var drives = ps.AddCommand("Get-PSDrive").Invoke().Select(drive => drive.Properties["Name"].Value as string);

                if (fileNamePtr.IndexOfAny('?', '*') >= 0)
                {
                    var regx = Utilities.ConvertWildcardsToRegEx(fileNamePtr.TrimStart('\\').ToString(), ignoreCase: true);
                    drives = drives.Where(regx);
                }
                if (searchPattern != "*")
                {
                    var regx = Utilities.ConvertWildcardsToRegEx(searchPattern, ignoreCase: true);
                    drives = drives.Where(regx);
                }

                files = drives.Select(drive => new FindFileInformation
                {
                    Attributes = FileAttributes.Directory,
                    FileName = drive.AsMemory()
                });

                return NtStatus.Success;
            }

            var entries = ps.AddCommand("Get-Item").AddParameter("Path", path).Invoke().Select(obj => obj.Properties);

            files = entries
                .Where(obj => obj["PSChildName"]?.Value is string || obj["Name"]?.Value is string)
                .Select(finfo => new FindFileInformation
                {
                    Attributes = Enum.TryParse<FileAttributes>(finfo["Attributes"]?.Value?.ToString(), out var attr) ? attr :
                        finfo["PSIsContainer"]?.Value as bool? ?? false ? FileAttributes.Directory : FileAttributes.Normal,
                    CreationTime = finfo["CreationTime"]?.Value as DateTime? ?? finfo["NotBefore"]?.Value as DateTime?,
                    LastAccessTime = finfo["LastAccessTime"]?.Value as DateTime? ?? finfo["NotBefore"]?.Value as DateTime?,
                    LastWriteTime = finfo["LastWriteTime"]?.Value as DateTime? ?? finfo["NotBefore"]?.Value as DateTime?,
                    Length = finfo["Length"]?.Value as long? ?? (finfo["RawData"]?.Value as byte[])?.Length ?? (finfo["Value"]?.Value as string)?.Length ?? 0,
                    FileName = (finfo["PSChildName"]?.Value as string ?? finfo["Name"]?.Value as string).AsMemory()
                });

            return NtStatus.Success;
        }
    }

    public NtStatus SetFileAttributes(ReadOnlySpan<char> fileNamePtr, FileAttributes attributes, in DokanFileInfo info) => NtStatus.NotImplemented;

    public NtStatus SetFileTime(ReadOnlySpan<char> fileNamePtr, DateTime? creationTime, DateTime? lastAccessTime, DateTime? lastWriteTime, in DokanFileInfo info) => NtStatus.NotImplemented;

    public NtStatus DeleteFile(ReadOnlySpan<char> fileNamePtr, in DokanFileInfo info) => NtStatus.NotImplemented;

    public NtStatus DeleteDirectory(ReadOnlySpan<char> fileNamePtr, in DokanFileInfo info) => NtStatus.NotImplemented;

    public NtStatus MoveFile(ReadOnlySpan<char> oldName, ReadOnlySpan<char> newName, bool replace, ref DokanFileInfo info) => NtStatus.NotImplemented;

    public NtStatus SetEndOfFile(ReadOnlySpan<char> fileNamePtr, long length, in DokanFileInfo info) => NtStatus.NotImplemented;

    public NtStatus SetAllocationSize(ReadOnlySpan<char> fileNamePtr, long length, in DokanFileInfo info) => NtStatus.NotImplemented;

    public NtStatus LockFile(ReadOnlySpan<char> fileNamePtr, long offset, long length, in DokanFileInfo info) => NtStatus.NotImplemented;

    public NtStatus UnlockFile(ReadOnlySpan<char> fileNamePtr, long offset, long length, in DokanFileInfo info) => NtStatus.NotImplemented;

    public NtStatus GetDiskFreeSpace(out long freeBytesAvailable, out long totalNumberOfBytes, out long totalNumberOfFreeBytes, in DokanFileInfo info)
    {
        freeBytesAvailable = 0;
        totalNumberOfBytes = 0;
        totalNumberOfFreeBytes = 0;
        return NtStatus.Success;
    }

    public NtStatus GetVolumeInformation(out string volumeLabel, out FileSystemFeatures features, out string fileSystemName, out uint maximumComponentLength, ref uint volumeSerialNumber, in DokanFileInfo info)
    {
        volumeLabel = "PowerShell";
        features = FileSystemFeatures.ReadOnlyVolume | FileSystemFeatures.UnicodeOnDisk;
        fileSystemName = "PowerShell";
        maximumComponentLength = 260;
        return NtStatus.Success;
    }

    public NtStatus GetFileSecurity(ReadOnlySpan<char> fileNamePtr, out FileSystemSecurity security, AccessControlSections sections, in DokanFileInfo info)
    {
        security = null;
        return NtStatus.NotImplemented;
    }

    public NtStatus SetFileSecurity(ReadOnlySpan<char> fileNamePtr, FileSystemSecurity security, AccessControlSections sections, in DokanFileInfo info) => NtStatus.NotImplemented;

    public NtStatus Mounted(ReadOnlySpan<char> drive, in DokanFileInfo info) => NtStatus.Success;

    public NtStatus Unmounted(in DokanFileInfo info) => NtStatus.Success;

    public NtStatus FindStreams(ReadOnlySpan<char> fileNamePtr, out IEnumerable<FindFileInformation> streams, in DokanFileInfo info)
    {
        streams = null;
        return NtStatus.NotImplemented;
    }
}
