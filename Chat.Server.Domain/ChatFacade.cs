using Chat.Server.Domain.Commands;
using Chat.Server.Domain.Exceptions;
using Chat.Server.Domain.Factories;
using System;
using System.Threading.Tasks;

namespace Chat.Server.Domain
{
	public class ChatFacade
	{
		private ICommandHandlerFactory CommandHandlerFactory;

		public ChatFacade(ICommandHandlerFactory commandHandlerFactory)
		{
			CommandHandlerFactory = commandHandlerFactory;
		}

		public async Task ProcessMessageAsync(Guid connectionUid, Command command)
		{
			var handler = CommandHandlerFactory.GetHandler(command);

			ThrowsExceptionWhenHandlerIsNull(handler);

			await handler.ProcessAsync(connectionUid, command);
		}

		private static void ThrowsExceptionWhenHandlerIsNull(ICommandHandler handler)
		{
			if (handler == null)
			{
				throw new CommandDoesNotExistsException();
			}
		}
	}
}
