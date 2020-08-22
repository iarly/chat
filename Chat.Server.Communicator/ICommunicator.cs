using Chat.Server.Communicator.Delegates;
using Chat.Server.Domain.Commands;
using Chat.Server.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Chat.Server.Communicator
{
	public interface ICommunicator
	{
		event ClientConnectedDelegate OnClientConnected;
		event ClientDisconnectedDelegate OnClientDisconnected;
		event ClientSendCommandDelegate OnClientSendCommand;

		Task ListenAsync();

		Task PublishAsync(Command command);
	}
}
