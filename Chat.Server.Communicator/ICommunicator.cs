using Chat.Server.Communicator.Delegates;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.Server.Communicator
{
	public interface ICommunicator<TMessage>
	{
		event ClientConnectedDelegate OnClientConnected;
		event ClientDisconnectedDelegate OnClientDisconnected;
		event ClientSendMessageDelegate<TMessage> OnClientSendCommand;

		Task ListenAsync(CancellationToken cancellationToken);

		Task PublishAsync(Guid connectionUid, TMessage command);
		Task DisconnectAsync(Guid connectionUid);
	}
}
