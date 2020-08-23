using Chat.Server.Communicator.Delegates;
using Chat.Server.Domain.Commands;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.Server.Communicator
{
	public interface ICommunicator
	{
		event ClientConnectedDelegate OnClientConnected;
		event ClientDisconnectedDelegate OnClientDisconnected;
		event ClientSendCommandDelegate OnClientSendCommand;

		Task ListenAsync(CancellationToken cancellationToken);

		Task PublishAsync(Command command);
	}
}
