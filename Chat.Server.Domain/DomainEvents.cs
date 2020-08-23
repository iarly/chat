using Chat.Server.Domain.Commands;
using Chat.Server.Domain.Delegates;
using Chat.Server.Domain.Entities;

namespace Chat.Server.Domain
{
	public class DomainEvents : IDomainEvents
	{
		public event CommandDelegate OnCommand;
		public event CommandDelegate OnDisconnectCommand;

		public void SendCommand(Client destination, Command command)
		{
			if (OnCommand != null)
			{
				OnCommand.Invoke(destination, command);
			}
		}

		public void SendDisconnectCommand(Client destination, DisconnectCommand command)
		{
			if (OnDisconnectCommand != null)
			{
				OnDisconnectCommand.Invoke(destination, command);
			}
		}
	}
}
