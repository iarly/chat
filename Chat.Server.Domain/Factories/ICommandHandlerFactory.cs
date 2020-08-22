using Chat.Server.Domain.Commands;

namespace Chat.Server.Domain.Factories
{
	public interface ICommandHandlerFactory
	{
		ICommandHandler GetHandler(Command command);
	}
}
