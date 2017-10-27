using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

namespace EasyProg.WPF.MVVM.Converters
{
	public class BytesToSvgImageConverter : IValueConverter
	{
		private Size? _size;

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var bytes = value as byte[];
			if (bytes == null) return null;

			_size = parameter as Size?;

			return GetImageSource(bytes) ?? parameter;
		}

		public ImageSource GetImageSource(byte[] bytes)
		{
			var settings = new WpfDrawingSettings();

			using (var fileSvgReader = new FileSvgReader(settings))
			{
				using (var svgStream = new MemoryStream(bytes))
				{
					var drawingGroup = fileSvgReader.Read(svgStream);

					if (_size.HasValue)
					{
						var rect = drawingGroup.Bounds;
						drawingGroup.Transform = new ScaleTransform(_size.Value.Width/rect.Width, _size.Value.Height/rect.Height);
					}
					if (drawingGroup != null)
						return new DrawingImage(drawingGroup);
					return null;
				}
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}