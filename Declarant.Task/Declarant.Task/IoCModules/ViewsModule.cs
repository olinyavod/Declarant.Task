using Autofac;
using Declarant.Task.Views;
using EasyProg.WPF.MVVM.Services;

namespace Declarant.Task.IoCModules
{
	internal class ViewsModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			builder.RegisterView<EventItemEditorView>();
		}
	}
}