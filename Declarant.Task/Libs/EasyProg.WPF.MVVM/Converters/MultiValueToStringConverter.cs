using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace EasyProg.WPF.MVVM.Converters
{
	public class MultiValueToStringConverter:IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			var separator = " ";
			if (parameter != null) separator = parameter.ToString();
			return string.Join(separator, values.Where(i => !Equals(DependencyProperty.UnsetValue, i)));
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
