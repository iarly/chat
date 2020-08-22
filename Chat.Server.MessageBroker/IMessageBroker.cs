using Chat.Server.Domain.Commands;
using Chat.Server.MessageBroker.Delegates;
using System;
using System.Threading.Tasks;

namespace Chat.Server.Application.Contracts
{
	public interface IMessageBroker
	{
		Task PublishAsync(Command command);

		Task SubscribeAsync(Guid connectionUid, BrokerSendCommandDelegate onReceiveCommand);
	}
}
