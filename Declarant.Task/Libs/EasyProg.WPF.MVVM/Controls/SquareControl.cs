using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace EasyProg.WPF.MVVM.Controls
{
	public class SquareControl : Panel
	{
		protected override Size MeasureOverride(Size constraint)
		{
			var m = Math.Min(constraint.Width, constraint.Height);
			return new Size(m, m);
		}
	}
}
