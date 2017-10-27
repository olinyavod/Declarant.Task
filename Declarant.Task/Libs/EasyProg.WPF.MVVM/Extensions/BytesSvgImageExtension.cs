using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

namespace EasyProg.WPF.MVVM.Extensions
{
	[MarkupExtensionReturnType(typeof(DrawingImage))]
	public class BytesSvgImageExtension : MarkupExtension
	{
		public bool TextAsGeometry { get; set; }

		public bool IncludeRuntime { get; set; }

		public bool OptimizePath { get; set; }

		public byte[] Source { get; set; }

		public CultureInfo Culture { get; set; }

		public Size? Size { get; set; }

		public BytesSvgImageExtension(byte[] source) : this()
		{
			Source = source;
			
		}

		public BytesSvgImageExtension()
		{
			TextAsGeometry = false;
			IncludeRuntime = true;
			OptimizePath = true;
			Culture = CultureInfo.CurrentUICulture;
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			var settings = new WpfDrawingSettings
			{
				IncludeRuntime = IncludeRuntime,
				TextAsGeometry = TextAsGeometry,
				OptimizePath = OptimizePath
			};
			if (Culture != null)
				settings.CultureInfo = Culture;
			using (FileSvgReader fileSvgReader = new FileSvgReader(settings))
			{
				using (var svgStream = new MemoryStream(Source))
				{
					var drawingGroup = fileSvgReader.Read(svgStream);

					if (Size.HasValue)
					{
						var rect = drawingGroup.Bounds;
						drawingGroup.Transform = new ScaleTransform(Size.Value.Width / rect.Width, Size.Value.Height / rect.Height);
					}
					if (drawingGroup != null)
						return new DrawingImage(drawingGroup);
				}
			}
			return null;
		}
	}
}
