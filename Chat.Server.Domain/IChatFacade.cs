using Chat.Server.Domain.Commands;
using System.Threading.Tasks;

namespace Chat.Server.Domain
{
	public interface IChatFacade
	{
		Task ProcessMessageAsync(Command command);
	}
}