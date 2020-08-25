using Chat.Serializers;
using Chat.Server.Application;
using Chat.Server.Application.Contracts;
using Chat.Server.Application.Mappers;
using Chat.Server.Communicator;
using Chat.Server.Communicator.Sockets;
using Chat.Server.Data.InMemory;
using Chat.Server.Domain;
using Chat.Server.Domain.Commands;
using Chat.Server.Domain.Entities;
using Chat.Server.Domain.Factories;
using Chat.Server.Domain.Repositories;
using Chat.Server.Domain.Services;
using Chat.Server.MessageBroker.Local;
using Chat.Server.MessageBroker.Redis;
using Microsoft.Extensions.Configuration;
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

		public static void ConfigureServices(IConfiguration configuration)
		{
			var services = new ServiceCollection();

			services.AddSingleton(configuration);

			services.AddSingleton<ChatApplication>();

			services.AddSingleton<IDomainEvents, DomainEvents>();

			services.AddSingleton<IChatFacade, ChatFacade>();
			services.AddSingleton<IChatService, ChatService>();

			services.AddSingleton<IClientFactory, ClientFactory>();
			services.AddSingleton<ICommandHandlerFactory, CommandHandlerFactory>();

			services.AddSingleton<ICommunicator<string>, SocketCommunicator<string>>();

			services.AddSingleton<ISerializer<Command>, CommandSerializer>();
			services.AddSingleton<ISerializer<string>, StringSerializer>();
			services.AddSingleton<ITextualCommandMapper, TextualCommandMapper>();

			ConfigureDatabase(configuration, services);
			ConfigurateMessageBroker(configuration, services);

			ServiceProvider = services.BuildServiceProvider();
		}

		private static void ConfigureDatabase(IConfiguration configuration, ServiceCollection services)
		{
			string mongoDbConnection = configuration["mongoDbConnection"];
			string mongoDbDatabase = configuration["mongoDbDatabase"];

			if (string.IsNullOrEmpty(mongoDbConnection))
			{
				services.AddSingleton<IClientRepository, ClientRepository>();
			}
			else
			{
				services.AddSingleton<IClientRepository>(injector =>
				{
					return new Data.MongoDb.ClientRepository(mongoDbConnection, mongoDbDatabase);
				});
			}
		}

		private static void ConfigurateMessageBroker(IConfiguration configuration, ServiceCollection services)
		{
			string redisConfiguration = configuration["redis"];

			if (string.IsNullOrEmpty(redisConfiguration))
			{
				services.AddSingleton<IMessageBroker, DummyMessageBroker>();
			}
			else
			{
				services.AddSingleton<IMessageBroker>(injector =>
				{
					var serializer = injector.GetService<ISerializer<Command>>();
					return new RedisMessageBroker(redisConfiguration, serializer);
				});
			}
		}
	}
}
