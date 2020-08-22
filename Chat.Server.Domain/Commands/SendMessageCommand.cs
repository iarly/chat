using Chat.Server.Domain.Entities;

namespace Chat.Server.Domain.Commands
{
	public class SendMessageCommand : Command
	{
		public string TargetClient { get; set; }
		public IMessageContent MessageContent { get; set; }
		public bool Private { get; set; }

		public bool IsTargeted => !string.IsNullOrEmpty(TargetClient);
	}
}
