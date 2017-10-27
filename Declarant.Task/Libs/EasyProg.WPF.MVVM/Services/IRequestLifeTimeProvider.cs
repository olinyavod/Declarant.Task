using Autofac;

namespace EasyProg.WPF.MVVM.Services
{
	public interface IRequestLifeTimeProvider
	{
		ILifetimeScope BeginRequest();
	}
}