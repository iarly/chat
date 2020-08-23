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
		private IClientRepository ClientRepository { get; }
		public IChatService ChatService { get; }

		public ChatFacade(IClientRepository clientRepository,
			ICommandHandlerFactory commandHandlerFactory,
			IChatService chatService)
		{
			ClientRepository = clientRepository;
			CommandHandlerFactory = commandHandlerFactory;
			ChatService = chatService;
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
				await ChatService.SendNoticeMessageAsync(command.ConnectionUid, exception.Message);
			}
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
