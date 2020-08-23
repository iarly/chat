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
		public IDomainEvents DomainEvents { get; }

		public ChatFacade(IClientRepository clientRepository,
			ICommandHandlerFactory commandHandlerFactory,
			IDomainEvents domainEvents)
		{
			ClientRepository = clientRepository;
			CommandHandlerFactory = commandHandlerFactory;
			DomainEvents = domainEvents;
		}

		public async Task<Client> GetClientByUidAsync(Guid connectionUid)
		{
			return await ClientRepository.GetByUidAsync(connectionUid);
		}

		public async Task ProcessMessageAsync(Command command)
		{
			try
			{
				var handler = CommandHandlerFactory.GetHandler(command);

				ThrowsExceptionWhenHandlerIsNull(handler);

				await handler.ProcessAsync(command);
			}
			catch (Exception exception)
			{
				await HandleException(exception);
			}
		}

		private async Task HandleException(Exception exception)
		{
			var exceptionCommand = new ExceptionCommand(exception);
			var exceptionHandler = CommandHandlerFactory.GetHandler(exceptionCommand);
			await exceptionHandler.ProcessAsync(exceptionCommand);
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
