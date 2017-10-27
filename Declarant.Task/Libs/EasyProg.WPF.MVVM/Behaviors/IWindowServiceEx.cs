using System.Windows;
using DevExpress.Mvvm;

namespace EasyProg.WPF.MVVM.Behaviors
{
	public interface IWindowServiceEx :IWindowService
	{
		Size? Size { get; set; }
        object Icon { get; set; }
		SizeToContent SizeToContent { get; set; }
	}
}