using Chat.Server.Application;
using Chat.Server.Application.Contracts;
using Chat.Server.Domain;
using Chat.Server.Domain.Entities;
using Chat.Server.Domain.Factories;
using Chat.Server.MessageBroker.Local;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chat.Server.IoC
{
	public class InjectionConfig
	{
		public static ServiceProvider ServiceProvider { get; private set; }

		public static T Get<T>()
		{
			return ServiceProvider.GetService<T>();
		}

		public static void ConfigureServices()
		{
			var services = new ServiceCollection();

			services.AddSingleton<ChatApplication>();
			
			services.AddSingleton<IDomainEvents, DomainEvents>();

			services.AddSingleton<IChatFacade, ChatFacade>();
			services.AddSingleton<IChatService, ChatService>();

			services.AddSingleton<IClientFactory, ClientFactory>();
			services.AddSingleton<ICommandHandlerFactory, CommandHandlerFactory>();

			services.AddSingleton<IMessageBroker, DummyMessageBroker>();

			ServiceProvider = services.BuildServiceProvider();
		}
	}
}
