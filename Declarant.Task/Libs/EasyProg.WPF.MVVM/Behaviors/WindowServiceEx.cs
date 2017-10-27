using System.Globalization;
using System.Windows;
using System.Windows.Media;
using DevExpress.Mvvm.UI;
using DevExpress.Mvvm.UI.Native;
using EasyProg.WPF.MVVM.Converters;

namespace EasyProg.WPF.MVVM.Behaviors
{
	public class WindowServiceEx : WindowService, IWindowServiceEx
	{
		private Size? _size;
	    private object _icon;
		private SizeToContent _sizeToContent;

		public WindowServiceEx()
		{
			_sizeToContent = SizeToContent.Manual;
		}

		Size? IWindowServiceEx.Size
		{
			get { return _size; }
			set { _size = value; }
		}

	    object IWindowServiceEx.Icon
	    {
	        get { return _icon; }
	        set { _icon = value; }
	    }

		SizeToContent IWindowServiceEx.SizeToContent
		{
			get { return _sizeToContent; }
			set { _sizeToContent = value; }
		}

		protected override IWindowSurrogate CreateWindow(object view)
		{
			var window = base.CreateWindow(view);
			if (_size != null)
			{
				window.RealWindow.SizeToContent = SizeToContent.Manual;
				window.RealWindow.Height = _size.Value.Height;
				window.RealWindow.Width = _size.Value.Width;
				window.RealWindow.SizeToContent = _sizeToContent;
			}
		    if (_icon != null)
		    {
                var converter = new BytesToSvgImageConverter();
		        window.RealWindow.Icon = converter.Convert(_icon, typeof(ImageSource), null, CultureInfo.CurrentCulture) as ImageSource;
		    }
			return window;
		}
	}
}
