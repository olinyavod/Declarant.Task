using System;
using System.Runtime.InteropServices;

namespace EasyProg.WPF.MVVM.Behaviors.DragDropFiles.Win32
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal sealed class FILEDESCRIPTORW
	{
		public uint dwFlags;
		public Guid clsid;
		public SIZEL sizel;
		public POINTL pointl;
		public uint dwFileAttributes;
		public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
		public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
		public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
		public uint nFileSizeHigh;
		public uint nFileSizeLow;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string cFileName;
	}
}