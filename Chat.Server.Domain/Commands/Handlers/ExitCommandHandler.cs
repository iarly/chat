using System.Threading.Tasks;

namespace Chat.Server.Domain.Commands.Handlers
{
	public class ExitCommandHandler : CommandHandler<ExitCommand>
	{
		public ExitCommandHandler(IChatService chatService)
		{
			ChatService = chatService;
		}

		public IChatService ChatService { get; }

		protected override async Task InternalProcessAsync(ExitCommand command)
		{
			await ChatService.DisconnectAsync(command.ConnectionUid);
		}
	}
}
