using DiscUtils.Internal;
using DokanNet;
using Microsoft.PowerShell.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;

namespace PowerShellFs
{
    public class PowerShellFs : IDokanOperations, IDokanOperationsUnsafe
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

        public NtStatus CreateFile(string fileName, NativeFileAccess access, FileShare share, FileMode mode, FileOptions options, FileAttributes attributes, IDokanFileInfo info)
        {
            var path = TranslatePath(fileName);

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

            if (finfo == null && path.EndsWith(@":\", StringComparison.Ordinal))
            {
                lock (Runspace)
                {
                    using var ps = GetPS();

                    finfo = ps.AddCommand("Get-PSDrive").AddParameter("Name", path.Remove(path.Length - 2)).Invoke().FirstOrDefault();
                }
            }

            if (finfo == null)
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

        public void Cleanup(string fileName, IDokanFileInfo info)
        {
            if (info?.Context is IDisposable context)
            {
                context.Dispose();
                info.Context = null;
            }
        }

        public void CloseFile(string fileName, IDokanFileInfo info)
        {
        }

        unsafe public NtStatus ReadFile(string fileName, byte[] buffer, out int bytesRead, long offset, IDokanFileInfo info)
        {
            fixed (byte* ptr = &buffer[0])
            {
                return ReadFile(fileName, new IntPtr(ptr), (uint)buffer.Length, out bytesRead, offset, info);
            }
        }

        public NtStatus ReadFile(string fileName, IntPtr buffer, uint bufferLength, out int bytesRead, long offset, IDokanFileInfo info)
        {
            var path = TranslatePath(fileName);

            bytesRead = 0;

            if (info.Context is not FileObject fileObject)
            {
                return NtStatus.NotImplemented;
            }

            var filedata = fileObject.FileData;

            if (filedata == null)
            {
                lock (fileObject)
                {
                    filedata = fileObject.FileData;

                    if (filedata == null)
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
                                if (entry.Value == null)
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

            if (filedata == null)
            {
                return NtStatus.InvalidDeviceRequest;
            }

            if (offset > filedata.Length)
            {
                return NtStatus.Success;
            }

            var size = (int)Math.Min(filedata.Length - offset, bufferLength);

            Marshal.Copy(filedata, (int)offset, buffer, size);
            bytesRead = size;
            return NtStatus.Success;
        }

        public NtStatus WriteFile(string fileName, IntPtr buffer, uint bufferLength, out int bytesWritten, long offset, IDokanFileInfo info)
        {
            bytesWritten = 0;
            return NtStatus.NotImplemented;
        }

        public NtStatus WriteFile(string fileName, byte[] buffer, out int bytesWritten, long offset, IDokanFileInfo info)
        {
            bytesWritten = 0;
            return NtStatus.NotImplemented;
        }

        public NtStatus FlushFileBuffers(string fileName, IDokanFileInfo info)
        {
            return NtStatus.Success;
        }

        public NtStatus GetFileInformation(string fileName, out ByHandleFileInformation fileInfo, IDokanFileInfo info)
        {
            var path = TranslatePath(fileName);

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

            if (finfo == null && path.EndsWith(@":\", StringComparison.Ordinal))
            {
                lock (Runspace)
                {
                    using var ps = GetPS();

                    finfo = ps.AddCommand("Get-PSDrive").AddParameter("Name", path.Remove(path.Length - 2)).Invoke()
                        .FirstOrDefault()?.Properties;
                }

                if (finfo != null)
                {
                    fileInfo = new ByHandleFileInformation
                    {
                        Attributes = FileAttributes.Directory
                    };

                    return NtStatus.Success;
                }
            }

            if (finfo == null)
            {
                fileInfo = null;
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

        public NtStatus FindFiles(string fileName, out IEnumerable<FindFileInformation> files, IDokanFileInfo info)
        {
            return FindFilesWithPattern(fileName, "*", out files, info);
        }

        public static string TranslatePath(string path)
        {
            if (path != null && path.Length > 0 && path[0] == '\\')
            {
                path = path.Substring(1);
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                return string.Empty;
            }

            var firstDirDelimiter = path.IndexOf('\\');

            if (firstDirDelimiter >= 0)
            {
                path = path.Insert(firstDirDelimiter, ":");
            }
            else
            {
                path += @":\";
            }

            return path;
        }

        public NtStatus FindFilesWithPattern(string fileName, string searchPattern, out IEnumerable<FindFileInformation> files, IDokanFileInfo info)
        {
            searchPattern = searchPattern.Replace('<', '*');

            var path = TranslatePath(fileName);

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

                    if (fileName.IndexOfAny(new[] { '?', '*' }) >= 0)
                    {
                        Func<string, bool> regx = Utilities.ConvertWildcardsToRegEx(fileName.TrimStart('\\')).IsMatch;
                        drives = drives.Where(regx);
                    }
                    if (!"*".Equals(searchPattern, StringComparison.Ordinal))
                    {
                        Func<string, bool> regx = Utilities.ConvertWildcardsToRegEx(searchPattern).IsMatch;
                        drives = drives.Where(regx);
                    }

                    files = drives.Select(drive => new FindFileInformation
                    {
                        Attributes = FileAttributes.Directory,
                        FileName = drive
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
                        FileName = finfo["PSChildName"]?.Value as string ?? finfo["Name"]?.Value as string
                    });

                return NtStatus.Success;
            }
        }

        public NtStatus SetFileAttributes(string fileName, FileAttributes attributes, IDokanFileInfo info)
        {
            return NtStatus.NotImplemented;
        }

        public NtStatus SetFileTime(string fileName, DateTime? creationTime, DateTime? lastAccessTime, DateTime? lastWriteTime, IDokanFileInfo info)
        {
            return NtStatus.NotImplemented;
        }

        public NtStatus DeleteFile(string fileName, IDokanFileInfo info)
        {
            return NtStatus.NotImplemented;
        }

        public NtStatus DeleteDirectory(string fileName, IDokanFileInfo info)
        {
            return NtStatus.NotImplemented;
        }

        public NtStatus MoveFile(string oldName, string newName, bool replace, IDokanFileInfo info)
        {
            return NtStatus.NotImplemented;
        }

        public NtStatus SetEndOfFile(string fileName, long length, IDokanFileInfo info)
        {
            return NtStatus.NotImplemented;
        }

        public NtStatus SetAllocationSize(string fileName, long length, IDokanFileInfo info)
        {
            return NtStatus.NotImplemented;
        }

        public NtStatus LockFile(string fileName, long offset, long length, IDokanFileInfo info)
        {
            return NtStatus.NotImplemented;
        }

        public NtStatus UnlockFile(string fileName, long offset, long length, IDokanFileInfo info)
        {
            return NtStatus.NotImplemented;
        }

        public NtStatus GetDiskFreeSpace(out long freeBytesAvailable, out long totalNumberOfBytes, out long totalNumberOfFreeBytes, IDokanFileInfo info)
        {
            freeBytesAvailable = 0;
            totalNumberOfBytes = 0;
            totalNumberOfFreeBytes = 0;
            return NtStatus.Success;
        }

        public NtStatus GetVolumeInformation(out string volumeLabel, out FileSystemFeatures features, out string fileSystemName, out uint maximumComponentLength, ref uint volumeSerialNumber, IDokanFileInfo info)
        {
            volumeLabel = "PowerShell";
            features = FileSystemFeatures.ReadOnlyVolume | FileSystemFeatures.UnicodeOnDisk;
            fileSystemName = "PowerShell";
            maximumComponentLength = 260;
            return NtStatus.Success;
        }

        public NtStatus GetFileSecurity(string fileName, out FileSystemSecurity security, AccessControlSections sections, IDokanFileInfo info)
        {
            security = null;
            return NtStatus.NotImplemented;
        }

        public NtStatus SetFileSecurity(string fileName, FileSystemSecurity security, AccessControlSections sections, IDokanFileInfo info)
        {
            return NtStatus.NotImplemented;
        }

        public NtStatus Mounted(IDokanFileInfo info)
        {
            return NtStatus.Success;
        }

        public NtStatus Unmounted(IDokanFileInfo info)
        {
            return NtStatus.Success;
        }

        public NtStatus FindStreams(string fileName, out IEnumerable<FindFileInformation> streams, IDokanFileInfo info)
        {
            streams = null;
            return NtStatus.NotImplemented;
        }
    }
}
