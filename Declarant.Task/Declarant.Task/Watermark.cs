using System.Windows;

namespace Declarant.Task
{
	public class Watermark
	{
		public static readonly DependencyProperty WatermarkContentProperty = DependencyProperty.RegisterAttached(
			"WatermarkContent", typeof(object), typeof(Watermark), new FrameworkPropertyMetadata(default(object)) { Inherits = true });

		public static void SetWatermarkContent(DependencyObject element, object value)
		{
			element.SetValue(WatermarkContentProperty, value);
		}

		public static object GetWatermarkContent(DependencyObject element)
		{
			return (object)element.GetValue(WatermarkContentProperty);
		}

		public static readonly DependencyProperty WatermarkTemplateProperty = DependencyProperty.RegisterAttached(
			"WatermarkTemplate", typeof(DataTemplate), typeof(Watermark), new FrameworkPropertyMetadata(default(DataTemplate)) { Inherits = true });

		public static void SetWatermarkTemplate(DependencyObject element, DataTemplate value)
		{
			element.SetValue(WatermarkTemplateProperty, value);
		}

		public static DataTemplate GetWatermarkTemplate(DependencyObject element)
		{
			return (DataTemplate)element.GetValue(WatermarkTemplateProperty);
		}
	}
}