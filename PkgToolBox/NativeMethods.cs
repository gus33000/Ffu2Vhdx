using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Microsoft.Composition.ToolBox.IO
{
    internal class NativeMethods
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.U4)]
        public static extern int GetFullPathName(string inPath, int stringSize, StringBuilder longPath, StringBuilder fileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern uint GetFileAttributes(string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool DeleteFile(string lpExistingFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool SetFileAttributes(string lpExistingFileName, EFileAttributes dwFlagsAndAttributes);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern nint FindFirstFileEx(string lpFileName, int fInfoLevelId, out WIN32_FIND_DATA lpFindFileData, int fSearchOp, nint lpSearchFilter, uint dwAdditionalFlags);

        public static nint FindFirstFile(string lpFileName, out WIN32_FIND_DATA lpFindFileData, out int win32Error)
        {
            nint result = FindFirstFileEx(lpFileName, 1, out lpFindFileData, 0, nint.Zero, 0U);
            win32Error = Marshal.GetLastWin32Error();
            return result;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern int FindNextFile(nint hFindFile, out WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32.dll")]
        public static extern bool FindClose(nint hFindFile);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int CreateDirectory(string pszPath, nint psa);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int RemoveDirectory(string pszPath);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int GetFileAttributesEx(string lpFileName, int fInfoLevelId, out WIN32_FIND_DATA lpFileInformation);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern SafeFileHandle CreateFile(string lpFileName, EFileAccess dwDesiredAccess, EFileShare dwShareMode, nint lpSecurityAttributes, ECreationDisposition dwCreationDisposition, EFileAttributes dwFlagsAndAttributes, nint hTemplateFile);

        internal const int MAX_PATH = 260;

        internal const int COMPRESS_ALGORITHM_MSZIP = 2;

        internal const int COMPRESS_ALGORITHM_XPRESS = 3;

        internal const int COMPRESS_ALGORITHM_XPRESS_HUFF = 4;

        internal const int COMPRESS_ALGORITHM_LZMS = 5;

        internal enum ECreationDisposition : uint
        {
            CreateNew = 1U,
            CreateAlways,
            OpenExisting,
            OpenAlways,
            TruncateExisting
        }

        [Flags]
        internal enum EFileAccess : uint
        {
            None = 0U,
            GenericRead = 2147483648U,
            GenericWrite = 1073741824U,
            GenericExecute = 536870912U,
            GenericAll = 268435456U
        }

        [Flags]
        internal enum EFileAttributes : uint
        {
            None = 0U,
            Readonly = 1U,
            Hidden = 2U,
            System = 4U,
            Directory = 16U,
            Archive = 32U,
            Device = 64U,
            Normal = 128U,
            Temporary = 256U,
            SparseFile = 512U,
            ReparsePoint = 1024U,
            Compressed = 2048U,
            Offline = 4096U,
            NotContentIndexed = 8192U,
            Encrypted = 16384U,
            Write_Through = 2147483648U,
            Overlapped = 1073741824U,
            NoBuffering = 536870912U,
            RandomAccess = 268435456U,
            SequentialScan = 134217728U,
            DeleteOnClose = 67108864U,
            BackupSemantics = 33554432U,
            PosixSemantics = 16777216U,
            OpenReparsePoint = 2097152U,
            OpenNoRecall = 1048576U,
            FirstPipeInstance = 524288U
        }

        [Flags]
        internal enum EFileShare : uint
        {
            None = 0U,
            Read = 1U,
            Write = 2U,
            Delete = 4U,
            All = 7U
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct WIN32_FIND_DATA
        {
            internal FileAttributes dwFileAttributes;

            internal FILETIME ftCreationTime;

            internal FILETIME ftLastAccessTime;

            internal FILETIME ftLastWriteTime;

            internal uint nFileSizeHigh;

            internal uint nFileSizeLow;

            internal int dwReserved0;

            internal int dwReserved1;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            internal string cFileName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            internal string cAlternate;
        }
    }
}