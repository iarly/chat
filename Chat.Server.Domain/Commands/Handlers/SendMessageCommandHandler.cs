using System.Threading.Tasks;

namespace Chat.Server.Domain.Commands.Handlers
{
	public class SendMessageCommandHandler : CommandHandler<SendMessageCommand>
	{
		public SendMessageCommandHandler(IChatService chatService)
		{
			ChatService = chatService;
		}

		public IChatService ChatService { get; }

		protected override async Task InternalProcessAsync(SendMessageCommand command)
		{
			if (command.Private)
			{
				await ChatService.SendPrivateMessageAsync(command.ConnectionUid,
					command.TargetClient,
					command.MessageContent);
			}
			else if (command.IsTargeted)
			{
				await ChatService.SendPublicTargetedMessageAsync(command.ConnectionUid,
					command.TargetClient,
					command.MessageContent);
			}
			else
			{
				await ChatService.SendPublicMessageAsync(command.ConnectionUid,
					command.MessageContent);
			}
		}
	}
}
