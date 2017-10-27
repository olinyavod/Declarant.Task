using System.Runtime.InteropServices;

namespace EasyProg.WPF.MVVM.Behaviors.DragDropFiles.Win32
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct FILEGROUPDESCRIPTORW
	{
		public uint cItems;
		public FILEDESCRIPTORW[] fgd;
	}
}