using Chat.Server.Domain.Commands;
using System;
using System.Threading.Tasks;

namespace Chat.Server.Communicator.Delegates
{
	public delegate Task ClientSendCommandDelegate(Guid connectionUid, Command command);
}
