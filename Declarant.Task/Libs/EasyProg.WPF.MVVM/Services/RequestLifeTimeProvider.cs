using System;
using Autofac;
using Autofac.Core.Lifetime;

namespace EasyProg.WPF.MVVM.Services
{
	public class RequestLifeTimeProvider : IRequestLifeTimeProvider
	{
		private readonly Func<IContainer> _getContriner;

		public RequestLifeTimeProvider(Func<IContainer> getContriner)
		{
			_getContriner = getContriner;
		}

		public ILifetimeScope BeginRequest()
		{
			return _getContriner().BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag);
		}
	}
}
