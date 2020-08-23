using Chat.Server.Domain.Commands;
using Chat.Server.Domain.Entities;
using Chat.Server.Domain.Exceptions;
using Chat.Server.Domain.Factories;
using Chat.Server.Domain.Repositories;
using System;
using System.Threading.Tasks;

namespace Chat.Server.Domain
{
	public class ChatFacade : IChatFacade
	{
		private ICommandHandlerFactory CommandHandlerFactory;

		public IClientRepository ClientRepository { get; }

		public ChatFacade(IClientRepository clientRepository, ICommandHandlerFactory commandHandlerFactory)
		{
			ClientRepository = clientRepository;
			CommandHandlerFactory = commandHandlerFactory;
		}

		public async Task<Client> GetClientByUidAsync(Guid connectionUid)
		{
			return await ClientRepository.GetByUidAsync(connectionUid);
		}

		public async Task ProcessMessageAsync(Command command)
		{
			var handler = CommandHandlerFactory.GetHandler(command);

			ThrowsExceptionWhenHandlerIsNull(handler);

			await handler.ProcessAsync(command);
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
