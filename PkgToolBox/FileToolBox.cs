using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace Microsoft.Composition.ToolBox.IO
{
    public static class FileToolBox
    {
        public static bool Exists(string A_0)
        {
            return LongPathIO.Exists(A_0, LongPathIO.SystemType.File);
        }

        public static void Delete(string A_0)
        {
            if (!Exists(A_0))
            {
                return;
            }
            _ = NativeMethods.SetFileAttributes(LongPathIO.UNCPath(A_0), NativeMethods.EFileAttributes.Normal);
            if (NativeMethods.DeleteFile(LongPathIO.UNCPath(A_0)))
            {
                return;
            }
            int lastWin32Error = Marshal.GetLastWin32Error();
            if (LongPathIO.IsDirectory(A_0))
            {
                throw new Exception(string.Format("FileToolBox::Delete: FileToolBox was used instead of DirectoryToolBox to delete directory '{0}'.", A_0));
            }
            throw new Exception(string.Format("FileToolBox::Delete: Native delete file failed to delete '{0}'. Error={1}", A_0, lastWin32Error));
        }

        internal static bool FileModeRequiresFile(FileMode A_0)
        {
            return A_0 switch
            {
                FileMode.Open => true,
                FileMode.Truncate => true,
                FileMode.Append => true,
                _ => false,
            };
        }

        internal static NativeMethods.ECreationDisposition FileModeToNativeCreationDisposition(FileMode A_0)
        {
            NativeMethods.ECreationDisposition result = A_0 switch
            {
                FileMode.CreateNew => NativeMethods.ECreationDisposition.CreateNew,
                FileMode.Create => NativeMethods.ECreationDisposition.CreateAlways,
                FileMode.Open => NativeMethods.ECreationDisposition.OpenExisting,
                FileMode.OpenOrCreate => NativeMethods.ECreationDisposition.OpenAlways,
                FileMode.Truncate => NativeMethods.ECreationDisposition.OpenAlways,
                FileMode.Append => NativeMethods.ECreationDisposition.OpenExisting,
                _ => throw new InvalidEnumArgumentException(string.Format("Invalid value for mode of type System.IO.FileMode: {0}", A_0)),
            };
            return result;
        }

        public static FileStream Stream(string A_0, FileMode A_1, FileAccess A_2)
        {
            string text = LongPathIO.UNCPath(A_0);
            if (A_1 is FileMode.Append or FileMode.Truncate)
            {
                throw new NotSupportedException(string.Format("FileToolBox::Stream: FileMode: {0} is not supported.", A_1));
            }
            if (FileMode.CreateNew == A_1 && Exists(text))
            {
                throw new IOException("FileToolBox::Stream: File '" + text + "' already exists.");
            }
            if (FileModeRequiresFile(A_1))
            {
                if (!Exists(text))
                {
                    throw new FileNotFoundException("FileToolBox::Stream: File '" + text + "' was not found on disk.");
                }
            }
            else
            {
                DirectoryToolBox.Create(DirectoryToolBox.GetDirectoryFromFilePath(text));
            }
            NativeMethods.EFileShare dwShareMode = NativeMethods.EFileShare.All;
            NativeMethods.EFileAccess efileAccess = NativeMethods.EFileAccess.None;
            NativeMethods.ECreationDisposition dwCreationDisposition = FileModeToNativeCreationDisposition(A_1);
            if (A_2 is FileAccess.Read or FileAccess.ReadWrite)
            {
                efileAccess |= (NativeMethods.EFileAccess)2147483648U;
            }
            if (A_2 is FileAccess.Write or FileAccess.ReadWrite)
            {
                efileAccess |= NativeMethods.EFileAccess.GenericWrite;
            }
            SafeFileHandle safeFileHandle = NativeMethods.CreateFile(text, efileAccess, dwShareMode, nint.Zero, dwCreationDisposition, NativeMethods.EFileAttributes.BackupSemantics, nint.Zero);
            return safeFileHandle.IsInvalid
                ? throw new Win32Exception(Marshal.GetLastWin32Error(), "File: " + text)
                : new FileStream(safeFileHandle, A_2);
        }

        public static FileStream Stream(string A_0)
        {
            return Stream(A_0, FileMode.Open, FileAccess.Read);
        }
    }
}
