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
				string str = A_0.Substring("\\\\?\\UNC\\".Length, A_0.Length - "\\\\?\\UNC\\".Length);
				return "\\\\" + str;
			}
			if (A_0.StartsWithIgnoreCase("\\\\?\\"))
			{
				return A_0.Substring("\\\\?\\".Length, A_0.Length - "\\\\?\\".Length);
			}
			return A_0;
		}

		internal static string UNCPath(string A_0)
		{
			if (string.IsNullOrEmpty(A_0))
			{
				throw new ArgumentException("filename cannot be an empty string", "filename");
			}
			StringBuilder stringBuilder = new StringBuilder(260);
			int fullPathName = NativeMethods.GetFullPathName(A_0, stringBuilder.Capacity, stringBuilder, null);
			if (stringBuilder.Length < fullPathName)
			{
				stringBuilder.EnsureCapacity(fullPathName);
				fullPathName = NativeMethods.GetFullPathName(A_0, stringBuilder.Capacity, stringBuilder, null);
			}
			string text = stringBuilder.ToString();
			if (!text.StartsWithIgnoreCase("\\\\.\\") && !text.StartsWithIgnoreCase("\\\\?\\") && !text.StartsWithIgnoreCase("\\\\?\\\\\\?\\UNC\\"))
			{
				if (text.StartsWithIgnoreCase("\\\\"))
				{
					text = "\\\\?\\UNC\\" + text.TrimStart(new char[]
					{
						'\\'
					});
				}
				else
				{
					text = "\\\\?\\" + text;
				}
			}
			return text;
		}

		internal static bool Exists(string A_0, SystemType A_1)
		{
			string lpFileName = LongPathIO.UNCPath(A_0);
			uint fileAttributes;
			try
			{
				fileAttributes = NativeMethods.GetFileAttributes(lpFileName);
			}
			catch
			{
				throw new FormatException(string.Format("LongPathIO::Exists: Could not get the attributes of file '{0}'", A_0));
			}
			if (A_1 == LongPathIO.SystemType.File)
			{
				return LongPathIO.IsFile((FileAttributes)fileAttributes);
			}
			return LongPathIO.IsDirectory((FileAttributes)fileAttributes);
		}

		internal static bool IsDirectory(string A_0)
		{
			string lpFileName = LongPathIO.UNCPath(A_0);
			uint fileAttributes;
			try
			{
				fileAttributes = NativeMethods.GetFileAttributes(lpFileName);
			}
			catch
			{
				throw new FormatException(string.Format("LongPathIO::IsDirectory: Could not get the attributes of file '{0}'", A_0));
			}
			return LongPathIO.IsDirectory((FileAttributes)fileAttributes);
		}

		internal static bool IsDirectory(FileAttributes A_0)
		{
			return (A_0 & FileAttributes.Directory) == FileAttributes.Directory && A_0 != (FileAttributes)(-1);
		}

		internal static bool IsFile(FileAttributes A_0)
		{
			return (A_0 & FileAttributes.Directory) != FileAttributes.Directory && A_0 != (FileAttributes)(-1);
		}

		internal static bool ValidHandle(IntPtr A_0)
		{
			return A_0 != LongPathIO.NullHandleValue && A_0 != LongPathIO.InvalidHandleValue;
		}

		internal static IntPtr InvalidHandleValue = new IntPtr(-1);

		internal static IntPtr NullHandleValue = new IntPtr(0);

		internal enum SystemType
		{
			File,
			Directory
		}
	}
}
