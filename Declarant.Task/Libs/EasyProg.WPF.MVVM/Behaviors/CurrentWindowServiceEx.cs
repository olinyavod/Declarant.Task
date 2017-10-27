using System.Globalization;
using System.Windows;
using System.Windows.Media;
using DevExpress.Mvvm.UI;
using EasyProg.WPF.MVVM.Converters;

namespace EasyProg.WPF.MVVM.Behaviors
{
	public class CurrentWindowServiceEx : CurrentWindowService
	{
		public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
			"Title", typeof(string), typeof(CurrentWindowServiceEx), new PropertyMetadata(default(string), TitleOnPropertyChanged));

		private static void TitleOnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var control = (CurrentWindowServiceEx) sender;
			if (control.ActualWindow != null)
			{
				control.ActualWindow.Title = e.NewValue?.ToString();
			}
		}

		public string Title
		{
			get { return (string) GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}

		public static readonly DependencyProperty IconSourceProperty = DependencyProperty.Register(
			"IconSource", typeof(ImageSource), typeof(CurrentWindowServiceEx), new PropertyMetadata(default(ImageSource), IconSourcePropertyChangedCallback));

		private static void IconSourcePropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var control = (CurrentWindowServiceEx) sender;
			if (control.ActualWindow != null)
			{
				control.ActualWindow.Icon = (ImageSource) e.NewValue;
			}
		}

		public ImageSource IconSource
		{
			get { return (ImageSource) GetValue(IconSourceProperty); }
			set { SetValue(IconSourceProperty, value); }
		}

		public static readonly DependencyProperty IconBytesProperty = DependencyProperty.Register(
			"IconBytes", typeof(byte[]), typeof(CurrentWindowServiceEx), new PropertyMetadata(default(byte[]), IconBytesPropertyChangedCallback));

		private static void IconBytesPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var control = (CurrentWindowServiceEx) sender;
			if (control.ActualWindow != null)
			{
				var converter = new BytesToSvgImageConverter();
				control.ActualWindow.Icon = ((ImageSource) converter.Convert(e.NewValue, typeof(ImageSource), null, CultureInfo.CurrentCulture));
			}
		}

		public byte[] IconBytes
		{
			get { return (byte[]) GetValue(IconBytesProperty); }
			set { SetValue(IconBytesProperty, value); }
		}

		protected override void OnActualWindowChanged(Window oldWindow)
		{
			base.OnActualWindowChanged(oldWindow);
			if (ActualWindow != null)
			{
				if (!string.IsNullOrWhiteSpace(Title))
					ActualWindow.Title = Title;
				if (IconSource != null)
					ActualWindow.Icon = IconSource;
				if (IconBytes != null)
				{
					var converter = new BytesToSvgImageConverter();
					ActualWindow.Icon = ((ImageSource)converter.Convert(IconBytes, typeof(ImageSource), null, CultureInfo.CurrentCulture));
				}
			}
		}
	}
}
