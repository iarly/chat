using System;
using System.Threading.Tasks;

namespace Chat.Server.Communicator.Delegates
{
	public delegate Task ClientDisconnectedDelegate(Guid connectionUid);
}
