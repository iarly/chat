using Chat.Server.Domain.Commands;
using Chat.Server.Domain.Entities;
using System.Threading.Tasks;

namespace Chat.Server.Domain.Delegates
{
	public delegate Task CommandDelegate(Client destination, Command command);
}
