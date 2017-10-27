using System;
using System.Runtime.InteropServices;

namespace EasyProg.WPF.MVVM.Behaviors.DragDropFiles.Win32
{
	internal class NativeMethods
	{
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern IntPtr GlobalAlloc(int uFlags, int dwBytes);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern IntPtr GlobalFree(HandleRef handle);

		[DllImport("kernel32.dll")]
		private static extern IntPtr GlobalLock(IntPtr hMem);

		[DllImport("ole32.dll", PreserveSig = false)]
		public static extern ILockBytes CreateILockBytesOnHGlobal(IntPtr hGlobal, bool fDeleteOnRelease);

		[DllImport("OLE32.DLL", CharSet = CharSet.Auto, PreserveSig = false)]
		public static extern IntPtr GetHGlobalFromILockBytes(ILockBytes pLockBytes);

		[DllImport("OLE32.DLL", CharSet = CharSet.Unicode, PreserveSig = false)]
		public static extern IStorage StgCreateDocfileOnILockBytes(ILockBytes plkbyt, uint grfMode, uint reserved);

		// Clipboard formats used for cut/copy/drag operations
		public const string CFSTR_PREFERREDDROPEFFECT = "Preferred DropEffect";
		public const string CFSTR_PERFORMEDDROPEFFECT = "Performed DropEffect";
		public const string CFSTR_FILEDESCRIPTORW = "FileGroupDescriptorW";
		public const string CFSTR_FILEDESCRIPTOR = "FileGroupDescriptor";
		public const string CFSTR_FILECONTENTS = "FileContents";
		public const string CFSTR_FILEDROP = "FileDrop";
		public const string CFSTR_FILENAME = "FileName";
		public const string CFSTR_FILENAMEW = "FileNameW";

		// File Descriptor Flags
		public const Int32 FD_CLSID = 0x00000001;
		public const Int32 FD_SIZEPOINT = 0x00000002;
		public const Int32 FD_ATTRIBUTES = 0x00000004;
		public const Int32 FD_CREATETIME = 0x00000008;
		public const Int32 FD_ACCESSTIME = 0x00000010;
		public const Int32 FD_WRITESTIME = 0x00000020;
		public const Int32 FD_FILESIZE = 0x00000040;
		public const Int32 FD_PROGRESSUI = 0x00004000;
		public const Int32 FD_LINKUI = 0x00008000;

		// Global Memory Flags
		public const Int32 GMEM_MOVEABLE = 0x0002;
		public const Int32 GMEM_ZEROINIT = 0x0040;
		public const Int32 GHND = (GMEM_MOVEABLE | GMEM_ZEROINIT);
		public const Int32 GMEM_DDESHARE = 0x2000;

		// IDataObject constants
		public const Int32 DV_E_TYMED = unchecked((Int32)0x80040069);
	}
}