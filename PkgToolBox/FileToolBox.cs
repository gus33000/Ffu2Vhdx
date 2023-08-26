using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

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
			if (!FileToolBox.Exists(A_0))
			{
				return;
			}
			NativeMethods.SetFileAttributes(LongPathIO.UNCPath(A_0), NativeMethods.EFileAttributes.Normal);
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
			switch (A_0)
			{
			case FileMode.Open:
				return true;
			case FileMode.Truncate:
				return true;
			case FileMode.Append:
				return true;
			}
			return false;
		}

		internal static NativeMethods.ECreationDisposition FileModeToNativeCreationDisposition(FileMode A_0)
		{
			NativeMethods.ECreationDisposition result;
			switch (A_0)
			{
			case FileMode.CreateNew:
				result = NativeMethods.ECreationDisposition.CreateNew;
				break;
			case FileMode.Create:
				result = NativeMethods.ECreationDisposition.CreateAlways;
				break;
			case FileMode.Open:
				result = NativeMethods.ECreationDisposition.OpenExisting;
				break;
			case FileMode.OpenOrCreate:
				result = NativeMethods.ECreationDisposition.OpenAlways;
				break;
			case FileMode.Truncate:
				result = NativeMethods.ECreationDisposition.OpenAlways;
				break;
			case FileMode.Append:
				result = NativeMethods.ECreationDisposition.OpenExisting;
				break;
			default:
				throw new InvalidEnumArgumentException(string.Format("Invalid value for mode of type System.IO.FileMode: {0}", A_0));
			}
			return result;
		}

		public static FileStream Stream(string A_0, FileMode A_1, FileAccess A_2)
		{
			string text = LongPathIO.UNCPath(A_0);
			if (A_1 == FileMode.Append || A_1 == FileMode.Truncate)
			{
				throw new NotSupportedException(string.Format("FileToolBox::Stream: FileMode: {0} is not supported.", A_1));
			}
			if (FileMode.CreateNew == A_1 && FileToolBox.Exists(text))
			{
				throw new IOException("FileToolBox::Stream: File '" + text + "' already exists.");
			}
			if (FileToolBox.FileModeRequiresFile(A_1))
			{
				if (!FileToolBox.Exists(text))
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
			NativeMethods.ECreationDisposition dwCreationDisposition = FileToolBox.FileModeToNativeCreationDisposition(A_1);
			if (A_2 == FileAccess.Read || A_2 == FileAccess.ReadWrite)
			{
				efileAccess |= (NativeMethods.EFileAccess)2147483648U;
			}
			if (A_2 == FileAccess.Write || A_2 == FileAccess.ReadWrite)
			{
				efileAccess |= NativeMethods.EFileAccess.GenericWrite;
			}
			SafeFileHandle safeFileHandle = NativeMethods.CreateFile(text, efileAccess, dwShareMode, IntPtr.Zero, dwCreationDisposition, NativeMethods.EFileAttributes.BackupSemantics, IntPtr.Zero);
			if (safeFileHandle.IsInvalid)
			{
				throw new Win32Exception(Marshal.GetLastWin32Error(), "File: " + text);
			}
			return new FileStream(safeFileHandle, A_2);
		}

		public static FileStream Stream(string A_0)
		{
			return FileToolBox.Stream(A_0, FileMode.Open, FileAccess.Read);
		}
	}
}
