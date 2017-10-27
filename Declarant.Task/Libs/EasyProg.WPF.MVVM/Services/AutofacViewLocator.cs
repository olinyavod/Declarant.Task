using System;
using System.Windows;
using Autofac;
using DevExpress.Mvvm.UI;

namespace EasyProg.WPF.MVVM.Services
{
	public class AutofacViewLocator : IViewLocator
	{
		private readonly IContainer _container;

		public AutofacViewLocator(IContainer container)
		{
			_container = container;
		}
		public object ResolveView(string name)
		{
			return _container.ResolveNamed<DependencyObject>(name);
		}

		public Type ResolveViewType(string name)
		{
			return _container.ResolveNamed<ViewInfo>(name).ViewType;
		}

		public string GetViewTypeName(Type type)
		{
			return _container.ResolveKeyed<ViewInfo>(type).ViewName;
		}
	}
}