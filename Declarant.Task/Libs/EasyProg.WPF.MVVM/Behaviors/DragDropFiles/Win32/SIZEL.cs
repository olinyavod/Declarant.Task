using System.Runtime.InteropServices;

namespace EasyProg.WPF.MVVM.Behaviors.DragDropFiles.Win32
{
	[StructLayout(LayoutKind.Sequential)]
	internal sealed class SIZEL
	{
		public int cx;
		public int cy;
	}
}