using System;
using System.Threading.Tasks;

namespace Chat.Server.Domain.Commands
{
	public interface ICommandHandler
	{
		Task ProcessAsync(Command command);
	}
}
