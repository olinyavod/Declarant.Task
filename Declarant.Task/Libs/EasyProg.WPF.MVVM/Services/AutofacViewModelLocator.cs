using System;
using Autofac;
using DevExpress.Mvvm;

namespace EasyProg.WPF.MVVM.Services
{
	public class AutofacViewModelLocator : IViewModelLocator
	{
		private readonly IContainer _container;

		public AutofacViewModelLocator(IContainer container)
		{
			_container = container;
		}

		public object ResolveViewModel(string name)
		{
			return _container.ResolveNamed<object>(name);
		}

		public Type ResolveViewModelType(string name)
		{
			return _container.ResolveNamed<ViewModelInfo>(name).ViewModelType;
		}

		public string GetViewModelTypeName(Type type)
		{
			return _container.ResolveKeyed<ViewModelInfo>(type).ViewModelName;
		}
	}
}