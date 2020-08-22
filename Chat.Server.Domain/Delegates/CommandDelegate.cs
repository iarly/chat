using Chat.Server.Domain.Commands;
using Chat.Server.Domain.Entities;

namespace Chat.Server.Domain.Delegates
{
	public delegate void CommandDelegate(Client destination, Command command);
}
