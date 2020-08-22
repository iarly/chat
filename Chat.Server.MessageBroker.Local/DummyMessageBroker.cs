using Chat.Server.Application.Contracts;
using Chat.Server.Domain.Commands;
using Chat.Server.MessageBroker.Delegates;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chat.Server.MessageBroker.Local
{
	public class DummyMessageBroker : IMessageBroker
	{
		protected static IDictionary<Guid, BrokerSendCommandDelegate> Delegates = new Dictionary<Guid, BrokerSendCommandDelegate>();

		public Task PublishAsync(Command command)
		{
			if (Delegates.TryGetValue(command.ConnectionUid, out BrokerSendCommandDelegate onBrokerSendCommandDelegate))
			{
				onBrokerSendCommandDelegate.Invoke(command);
			}

			return Task.CompletedTask;
		}

		public Task SubscribeAsync(Guid connectionUid, BrokerSendCommandDelegate onReceiveCommand)
		{
			Delegates[connectionUid] = onReceiveCommand;

			return Task.CompletedTask;
		}
	}
}
