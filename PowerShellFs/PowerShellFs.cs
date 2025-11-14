using DiscUtils.Internal;
using DokanNet;
using LTRData.Extensions.Native.Memory;
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

    public int DirectoryListingTimeoutResetIntervalMs { get; }

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
        public byte[]? FileData;
    }

    public NtStatus CreateFile(ReadOnlyNativeMemory<char> fileNamePtr, NativeFileAccess access, FileShare share, FileMode mode, FileOptions options, FileAttributes attributes, ref DokanFileInfo info)
    {
        var path = TranslatePath(fileNamePtr.Span);

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

    public void Cleanup(ReadOnlyNativeMemory<char> fileNamePtr, ref DokanFileInfo info)
    {
        if (info.Context is IDisposable context)
        {
            context.Dispose();
            info.Context = null;
        }
    }

    public void CloseFile(ReadOnlyNativeMemory<char> fileNamePtr, ref DokanFileInfo info)
    {
    }

    public NtStatus ReadFile(ReadOnlyNativeMemory<char> fileNamePtr, NativeMemory<byte> buffer, out int bytesRead, long offset, in DokanFileInfo info)
    {
        var path = TranslatePath(fileNamePtr.Span);

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
                                filedata = [];
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

        filedata.AsSpan((int)offset, size).CopyTo(buffer.Span);
        bytesRead = size;
        return NtStatus.Success;
    }

    public NtStatus WriteFile(ReadOnlyNativeMemory<char> fileNamePtr, ReadOnlyNativeMemory<byte> buffer, out int bytesWritten, long offset, in DokanFileInfo info)
    {
        bytesWritten = 0;
        return NtStatus.NotImplemented;
    }

    public NtStatus FlushFileBuffers(ReadOnlyNativeMemory<char> fileNamePtr, in DokanFileInfo info) => NtStatus.Success;

    public NtStatus GetFileInformation(ReadOnlyNativeMemory<char> fileNamePtr, out ByHandleFileInformation fileInfo, in DokanFileInfo info)
    {
        var path = TranslatePath(fileNamePtr.Span);

        if (string.IsNullOrWhiteSpace(path))
        {
            fileInfo = new ByHandleFileInformation
            {
                Attributes = FileAttributes.Directory | FileAttributes.System
            };

            return NtStatus.Success;
        }

        PSMemberInfoCollection<PSPropertyInfo>? finfo;

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

    public NtStatus FindFiles(ReadOnlyNativeMemory<char> fileNamePtr, out IEnumerable<FindFileInformation> files, in DokanFileInfo info)
        => FindFilesWithPattern(fileNamePtr, "*", out files);

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

    public NtStatus FindFilesWithPattern(ReadOnlyNativeMemory<char> fileNamePtr,
                                         ReadOnlyNativeMemory<char> searchPatternPtr,
                                         out IEnumerable<FindFileInformation> files,
                                         in DokanFileInfo info)
        => FindFilesWithPattern(fileNamePtr, searchPatternPtr.ToString().Replace('<', '*'), out files);

    public NtStatus FindFilesWithPattern(ReadOnlyNativeMemory<char> fileNamePtr,
                                         string searchPattern,
                                         out IEnumerable<FindFileInformation> files)
    {
        var path = TranslatePath(fileNamePtr.Span);

        if (path.AsSpan().IndexOfAny('?', '*') < 0)
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

                if (fileNamePtr.Span.IndexOfAny('?', '*') >= 0)
                {
                    if (Utilities.ConvertWildcardsToRegEx(fileNamePtr.Span.TrimStart('\\').ToString(), ignoreCase: true) is { } regx)
                    {
                        drives = drives.Where(regx);
                    }
                }

                if (searchPattern != "*")
                {
                    if (Utilities.ConvertWildcardsToRegEx(searchPattern, ignoreCase: true) is { } regx)
                    {
                        drives = drives.Where(regx);
                    }
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

    public NtStatus SetFileAttributes(ReadOnlyNativeMemory<char> fileNamePtr, FileAttributes attributes, in DokanFileInfo info) => NtStatus.NotImplemented;

    public NtStatus SetFileTime(ReadOnlyNativeMemory<char> fileNamePtr, DateTime? creationTime, DateTime? lastAccessTime, DateTime? lastWriteTime, in DokanFileInfo info) => NtStatus.NotImplemented;

    public NtStatus DeleteFile(ReadOnlyNativeMemory<char> fileNamePtr, in DokanFileInfo info) => NtStatus.NotImplemented;

    public NtStatus DeleteDirectory(ReadOnlyNativeMemory<char> fileNamePtr, in DokanFileInfo info) => NtStatus.NotImplemented;

    public NtStatus MoveFile(ReadOnlyNativeMemory<char> oldName, ReadOnlyNativeMemory<char> newName, bool replace, ref DokanFileInfo info) => NtStatus.NotImplemented;

    public NtStatus SetEndOfFile(ReadOnlyNativeMemory<char> fileNamePtr, long length, in DokanFileInfo info) => NtStatus.NotImplemented;

    public NtStatus SetAllocationSize(ReadOnlyNativeMemory<char> fileNamePtr, long length, in DokanFileInfo info) => NtStatus.NotImplemented;

    public NtStatus LockFile(ReadOnlyNativeMemory<char> fileNamePtr, long offset, long length, in DokanFileInfo info) => NtStatus.NotImplemented;

    public NtStatus UnlockFile(ReadOnlyNativeMemory<char> fileNamePtr, long offset, long length, in DokanFileInfo info) => NtStatus.NotImplemented;

    public NtStatus GetDiskFreeSpace(out long freeBytesAvailable, out long totalNumberOfBytes, out long totalNumberOfFreeBytes, in DokanFileInfo info)
    {
        freeBytesAvailable = 0;
        totalNumberOfBytes = 0;
        totalNumberOfFreeBytes = 0;
        return NtStatus.Success;
    }

    public NtStatus GetVolumeInformation(NativeMemory<char> volumeLabel, out FileSystemFeatures features, NativeMemory<char> fileSystemName, out uint maximumComponentLength, ref uint volumeSerialNumber, in DokanFileInfo info)
    {
        volumeLabel.SetString("PowerShell");
        features = FileSystemFeatures.ReadOnlyVolume | FileSystemFeatures.UnicodeOnDisk;
        fileSystemName.SetString("PowerShell");
        maximumComponentLength = 260;
        return NtStatus.Success;
    }

    public NtStatus GetFileSecurity(ReadOnlyNativeMemory<char> fileNamePtr, out FileSystemSecurity security, AccessControlSections sections, in DokanFileInfo info)
    {
        security = null!;
        return NtStatus.NotImplemented;
    }

    public NtStatus SetFileSecurity(ReadOnlyNativeMemory<char> fileNamePtr, FileSystemSecurity security, AccessControlSections sections, in DokanFileInfo info) => NtStatus.NotImplemented;

    public NtStatus Mounted(ReadOnlyNativeMemory<char> drive, in DokanFileInfo info) => NtStatus.Success;

    public NtStatus Unmounted(in DokanFileInfo info) => NtStatus.Success;

    public NtStatus FindStreams(ReadOnlyNativeMemory<char> fileNamePtr, out IEnumerable<FindFileInformation> streams, in DokanFileInfo info)
    {
        streams = null!;
        return NtStatus.NotImplemented;
    }
}
