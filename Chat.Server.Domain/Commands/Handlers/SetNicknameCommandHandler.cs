using System.Threading.Tasks;

namespace Chat.Server.Domain.Commands.Handlers
{
	public class SetNicknameCommandHandler : CommandHandler<SetNicknameCommand>
	{
		public SetNicknameCommandHandler(IChatService chatService)
		{
			ChatService = chatService;
		}

		public IChatService ChatService { get; }

		protected override async Task InternalProcessAsync(SetNicknameCommand command)
		{
			await ChatService.UpdateNicknameAsync(command.ConnectionUid, command.Nickname);
		}
	}
}
