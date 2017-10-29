using System.Data.Entity;
using Autofac;
using AutoMapper;
using Declarant.Task.Managers;
using Declarant.Task.Models;
using Declarant.Task.Providers;
using Declarant.Task.Validators;
using DevExpress.Mvvm;
using EasyProg.BusinessLogic.Interfaces;
using FluentValidation;

namespace Declarant.Task.IoCModules
{
	partial class DeclarantModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			builder.RegisterModule<ViewsModule>();

			builder.RegisterType<DeclarantContext>()
				.As<DbContext, DeclarantContext>()
				.InstancePerRequest()
				.InstancePerLifetimeScope();

			builder.RegisterType<EventItemValidator>()
				.As<IValidator<EventItem>>()
				.SingleInstance();

			builder.RegisterType<EventItemsManager>()
				.AsImplementedInterfaces()
				.As<ICrudManager<EventItem, int>>()
				.AsSelf()
				.InstancePerRequest()
				.InstancePerLifetimeScope();

			builder.Register(c => Messenger.Default)
				.SingleInstance();

			builder.Register(c => new MapperConfiguration(cfg =>
				{
					cfg.AddProfile<Mapping>();
				}))
				.SingleInstance();

			builder.Register(c => TSettings.Settings.Default)
				.SingleInstance();

			builder.Register(c => c.Resolve<MapperConfiguration>().CreateMapper());

			builder.RegisterType<ShedulerProvider>()
				.As<IShedulerProvider>()
				.SingleInstance();

		}
	}
}
