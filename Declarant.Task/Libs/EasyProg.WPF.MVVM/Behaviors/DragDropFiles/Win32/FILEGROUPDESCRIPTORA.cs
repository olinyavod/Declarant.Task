using System.Runtime.InteropServices;

namespace EasyProg.WPF.MVVM.Behaviors.DragDropFiles.Win32
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	internal sealed class FILEGROUPDESCRIPTORA
	{
		public uint cItems;
		public FILEDESCRIPTORA[] fgd;
	}
}