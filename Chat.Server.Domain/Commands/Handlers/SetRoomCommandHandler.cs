using System.Threading.Tasks;

namespace Chat.Server.Domain.Commands.Handlers
{
	public class SetRoomCommandHandler : CommandHandler<SetRoomCommand>
	{
		public SetRoomCommandHandler(IChatService chatService)
		{
			ChatService = chatService;
		}

		public IChatService ChatService { get; }

		protected override async Task InternalProcessAsync(SetRoomCommand command)
		{
			await ChatService.UpdateRoomAsync(command.ConnectionUid, command.Room);
		}
	}
}
