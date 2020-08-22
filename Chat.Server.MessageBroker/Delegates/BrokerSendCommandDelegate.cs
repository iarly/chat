using Chat.Server.Domain.Commands;
using System.Threading.Tasks;

namespace Chat.Server.MessageBroker.Delegates
{
	public delegate Task BrokerSendCommandDelegate(Command command);
}
