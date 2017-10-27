using Autofac;
using Ploeh.AutoFixture;

namespace EasyProg.BusinessLogic.Tests.Base
{
	public interface IManagerTestConfig
	{
		IContainer Container { get; set; }

		void ConfigureFixture(IContainer container, IFixture fixture);

		object CreateInvalidModel();
		void ConfigureBuilder(ContainerBuilder builder);
		void SetUpDatabase(IContainer container);
	}
}