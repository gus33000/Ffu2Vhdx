using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Microsoft.Composition.ToolBox.IO
{
	public static class DirectoryToolBox
	{
		public static bool Exists(string A_0)
		{
			return LongPathIO.Exists(A_0, LongPathIO.SystemType.Directory) || DirectoryToolBox.IsDriveRoot(A_0);
		}

		public static void CreateDirectory(string A_0)
		{
			DirectoryToolBox.Create(A_0);
		}

		private static bool IsVolumeGuidPath(string A_0)
		{
			int length = "\\\\?\\Volume{205ebf40-0ede-47c6-adc8-3cbf3826b843}".Length;
			int length2 = "\\\\?\\Volume".Length;
			Guid guid;
			return A_0.Length == length && (A_0.StartsWithIgnoreCase("\\\\?\\Volume") || A_0.StartsWithIgnoreCase("\\\\.\\Volume")) && Guid.TryParse(A_0.Substring(length2), out guid);
		}

		public static bool IsDriveRoot(string A_0)
		{
			if (string.IsNullOrWhiteSpace(A_0))
			{
				return false;
			}
			string rootDirectory = DirectoryToolBox.GetRootDirectory(A_0);
			return A_0.CompareTo(rootDirectory) == 0 || A_0.CompareTo(rootDirectory + "\\") == 0;
		}

		public static void Create(string A_0)
		{
			string text = A_0;
			LinkedList<string> linkedList = new LinkedList<string>();
			while (!DirectoryToolBox.Exists(text) && !DirectoryToolBox.IsVolumeGuidPath(text))
			{
				linkedList.AddFirst(text);
				text = DirectoryToolBox.GetDirectoryFromFilePath(text);
			}
			foreach (string text2 in linkedList)
			{
				bool flag = NativeMethods.CreateDirectory(LongPathIO.UNCPath(text2), IntPtr.Zero) != 0;
				int lastWin32Error = Marshal.GetLastWin32Error();
				if (!flag && lastWin32Error != 183)
				{
					throw new Exception(string.Format("Failed to create directory at {0}, GLE = {1}", text2, lastWin32Error));
				}
			}
		}

		public static void Delete(string A_0)
		{
			string text = LongPathIO.UNCPath(A_0);
			if (!DirectoryToolBox.Exists(text))
			{
				return;
			}
			NativeMethods.WIN32_FIND_DATA win32_FIND_DATA = default(NativeMethods.WIN32_FIND_DATA);
			int num;
			IntPtr intPtr = NativeMethods.FindFirstFile(Path.Combine(text, "*.*"), out win32_FIND_DATA, out num);
			try
			{
				if (intPtr == LongPathIO.InvalidHandleValue)
				{
					if (!LongPathIO.IsDirectory(text))
					{
						throw new DirectoryNotFoundException(string.Format("DirectoryToolBox::Delete Target '{0}' is not a directory.", text));
					}
					throw new Exception(string.Format("DirectoryToolBox::Paths Native FindFirstFile failed to load '{0}'. Error={1}", A_0, num));
				}
				else
				{
					do
					{
						if (!win32_FIND_DATA.cFileName.EqualsIgnoreCase(".") && !win32_FIND_DATA.cFileName.EqualsIgnoreCase(".."))
						{
							if (LongPathIO.IsFile(win32_FIND_DATA.dwFileAttributes))
							{
								FileToolBox.Delete(PathToolBox.Combine(A_0, win32_FIND_DATA.cFileName));
							}
							else
							{
								DirectoryToolBox.Delete(PathToolBox.Combine(A_0, win32_FIND_DATA.cFileName));
							}
						}
					}
					while (NativeMethods.FindNextFile(intPtr, out win32_FIND_DATA) != 0);
					string text2 = LongPathIO.UNCPath(text);
					NativeMethods.SetFileAttributes(text2, NativeMethods.EFileAttributes.Normal);
					if (NativeMethods.RemoveDirectory(text2) == 0)
					{
						throw new Exception(string.Format("Kernel32 failed to delete directory '{0}'. Error={1}", text, Marshal.GetLastWin32Error()));
					}
				}
			}
			finally
			{
				if (LongPathIO.ValidHandle(intPtr))
				{
					NativeMethods.FindClose(intPtr);
				}
			}
		}

		public static string GetDirectoryFromFilePath(string A_0)
		{
			if (LongPathIO.IsDirectory(A_0))
			{
				return A_0;
			}
			int num = A_0.LastIndexOfIgnoreCase("\\");
			if (num == -1)
			{
				return string.Empty;
			}
			return A_0.Substring(0, num);
		}

		public static string GetRootDirectory(string A_0)
		{
			string text = LongPathIO.StripUNCPrefix(A_0);
			string str = A_0.Substring(0, A_0.Length - text.Length);
			if (!text.Contains("\\"))
			{
				return str + text;
			}
			string str2 = text.Substring(0, text.IndexOfIgnoreCase("\\"));
			return str + str2;
		}
	}
}
