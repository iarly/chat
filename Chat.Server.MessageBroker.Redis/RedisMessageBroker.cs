using Chat.Server.Application.Contracts;
using Chat.Server.Domain.Commands;
using Chat.Server.Domain.Services;
using Chat.Server.MessageBroker.Delegates;
using StackExchange.Redis;
using System;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.Server.MessageBroker.Redis
{
	public class RedisMessageBroker : IMessageBroker
	{
		private const string Channel = "urn:redissubscriber:command";

		public RedisMessageBroker(string configuration, ISerializer<Command> serializer)
		{
			Redis = ConnectionMultiplexer.Connect(configuration);
			Serializer = serializer;
		}

		public ConnectionMultiplexer Redis { get; }
		public CancellationToken CancellationToken { get; }
		public ISerializer<Command> Serializer { get; }

		public async Task PublishAsync(Command command)
		{
			string textCommand = Serializer.Serializer(command);

			var sub = Redis.GetSubscriber();

			await sub.PublishAsync(Channel, textCommand, CommandFlags.FireAndForget);
		}

		public Task SubscribeAsync(Guid connectionUid, BrokerSendCommandDelegate onReceiveCommand)
		{
			var subscriber = Redis.GetSubscriber();

			subscriber.Subscribe(Channel, (channel, message) =>
			{
				if (string.IsNullOrEmpty(message)) return;
				Command command = Serializer.Deserialize(message);
				onReceiveCommand?.Invoke(command);
			});

			return Task.CompletedTask;
		}
	}
}
