using System.Windows;
using Autofac;
using Autofac.Extras.CommonServiceLocator;
using Declarant.Task.IoCModules;
using DevExpress.Mvvm.UI;
using EasyProg.WPF.MVVM.Services;

namespace Declarant.Task
{
	/// <summary>
	/// Логика взаимодействия для App.xaml
	/// </summary>
	public partial class App
	{
		private IContainer _container;

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			var builder = new ContainerBuilder();
			builder.RegisterModule<DeclarantModule>();

			builder.Register(c =>
			{
				ViewLocator.Default = new AutofacViewLocator(_container);
				return ViewLocator.Default;
			}).SingleInstance();

			builder.Register(c => new RequestLifeTimeProvider(() => _container))
				.As<IRequestLifeTimeProvider>()
				.SingleInstance();

			_container = builder.Build();

			CommonServiceLocator.ServiceLocator.SetLocatorProvider(() => new AutofacServiceLocator(_container));
			
		}
	}

	
}
