using Chat.Server.Domain.Commands;
using Chat.Server.Domain.Delegates;
using Chat.Server.Domain.Entities;

namespace Chat.Server.Domain
{
	public interface IDomainEvents
	{
		event CommandDelegate OnCommand;
		event CommandDelegate OnDisconnectCommand;

		void SendCommand(Client destination, Command command);
		void SendDisconnectCommand(Client destination, DisconnectCommand command);
	}
}