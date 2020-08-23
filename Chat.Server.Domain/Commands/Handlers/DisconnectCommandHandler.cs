using System.Threading.Tasks;

namespace Chat.Server.Domain.Commands.Handlers
{
	public class DisconnectCommandHandler : CommandHandler<ConnectCommand>
	{
		public DisconnectCommandHandler(IChatService chatService)
		{
			ChatService = chatService;
		}

		public IChatService ChatService { get; }

		protected override Task InternalProcessAsync(ConnectCommand command)
		{
			return ChatService.DisconnectAsync(command.ConnectionUid);
		}
	}
}