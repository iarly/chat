using System;
using System.Threading.Tasks;

namespace Chat.Server.Domain.Commands.Handlers
{
	public class ConnectCommandHandler : CommandHandler<ConnectCommand>
	{
		public ConnectCommandHandler(IChatService chatService)
		{
			ChatService = chatService;
		}

		public IChatService ChatService { get; }

		protected override async Task InternalProcessAsync(ConnectCommand command)
		{
			await ChatService.ConnectAsync(command.ConnectionUid);
		}
	}
}
