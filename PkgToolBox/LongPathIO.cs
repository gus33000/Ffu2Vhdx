using System;
using System.IO;
using System.Text;

namespace Microsoft.Composition.ToolBox.IO
{
    public static class LongPathIO
    {
        public static string StripUNCPrefix(string A_0)
        {
            if (A_0.StartsWithIgnoreCase("\\\\?\\UNC\\"))
            {
                string str = A_0["\\\\?\\UNC\\".Length..];
                return "\\\\" + str;
            }
            return A_0.StartsWithIgnoreCase("\\\\?\\") ? A_0["\\\\?\\".Length..] : A_0;
        }

        internal static string UNCPath(string A_0)
        {
            if (string.IsNullOrEmpty(A_0))
            {
                throw new ArgumentException("filename cannot be an empty string", "filename");
            }
            StringBuilder stringBuilder = new(260);
            int fullPathName = NativeMethods.GetFullPathName(A_0, stringBuilder.Capacity, stringBuilder, null);
            if (stringBuilder.Length < fullPathName)
            {
                _ = stringBuilder.EnsureCapacity(fullPathName);
                _ = NativeMethods.GetFullPathName(A_0, stringBuilder.Capacity, stringBuilder, null);
            }
            string text = stringBuilder.ToString();
            if (!text.StartsWithIgnoreCase("\\\\.\\") && !text.StartsWithIgnoreCase("\\\\?\\") && !text.StartsWithIgnoreCase("\\\\?\\\\\\?\\UNC\\"))
            {
                text = text.StartsWithIgnoreCase("\\\\")
                    ? "\\\\?\\UNC\\" + text.TrimStart(new char[]
                    {
                        '\\'
                    })
                    : "\\\\?\\" + text;
            }
            return text;
        }

        internal static bool Exists(string A_0, SystemType A_1)
        {
            string lpFileName = UNCPath(A_0);
            uint fileAttributes;
            try
            {
                fileAttributes = NativeMethods.GetFileAttributes(lpFileName);
            }
            catch
            {
                throw new FormatException(string.Format("LongPathIO::Exists: Could not get the attributes of file '{0}'", A_0));
            }
            return A_1 == SystemType.File ? IsFile((FileAttributes)fileAttributes) : IsDirectory((FileAttributes)fileAttributes);
        }

        internal static bool IsDirectory(string A_0)
        {
            string lpFileName = UNCPath(A_0);
            uint fileAttributes;
            try
            {
                fileAttributes = NativeMethods.GetFileAttributes(lpFileName);
            }
            catch
            {
                throw new FormatException(string.Format("LongPathIO::IsDirectory: Could not get the attributes of file '{0}'", A_0));
            }
            return IsDirectory((FileAttributes)fileAttributes);
        }

        internal static bool IsDirectory(FileAttributes A_0)
        {
            return (A_0 & FileAttributes.Directory) == FileAttributes.Directory && A_0 != (FileAttributes)(-1);
        }

        internal static bool IsFile(FileAttributes A_0)
        {
            return (A_0 & FileAttributes.Directory) != FileAttributes.Directory && A_0 != (FileAttributes)(-1);
        }

        internal static bool ValidHandle(nint A_0)
        {
            return A_0 != NullHandleValue && A_0 != InvalidHandleValue;
        }

        internal static nint InvalidHandleValue = new(-1);

        internal static nint NullHandleValue = new(0);

        internal enum SystemType
        {
            File,
            Directory
        }
    }
}
