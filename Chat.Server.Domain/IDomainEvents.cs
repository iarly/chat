using Chat.Server.Domain.Commands;
using Chat.Server.Domain.Delegates;
using Chat.Server.Domain.Entities;

namespace Chat.Server.Domain
{
	public interface IDomainEvents
	{
		event CommandDelegate OnCommand;

		void SendCommand(Client destination, Command command);
	}
}