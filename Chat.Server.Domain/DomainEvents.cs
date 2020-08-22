using Chat.Server.Domain.Commands;
using Chat.Server.Domain.Delegates;
using Chat.Server.Domain.Entities;
using System;

namespace Chat.Server.Domain
{
	public class DomainEvents : IDomainEvents
	{
		public event CommandDelegate OnCommand;

		public void SendCommand(Client destination, Command command)
		{
			if (OnCommand != null)
			{
				OnCommand.Invoke(destination, command);
			}
		}
	}
}
