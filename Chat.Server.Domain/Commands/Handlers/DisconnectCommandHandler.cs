using System.Threading.Tasks;

namespace Chat.Server.Domain.Commands.Handlers
{
	public class DisconnectCommandHandler : CommandHandler<DisconnectCommand>
	{
		public DisconnectCommandHandler(IChatService chatService)
		{
			ChatService = chatService;
		}

		public IChatService ChatService { get; }

		protected override Task InternalProcessAsync(DisconnectCommand command)
		{
			return ChatService.DisconnectAsync(command.ConnectionUid);
		}
	}
}