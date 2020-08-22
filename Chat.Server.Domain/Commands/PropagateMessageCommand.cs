using Chat.Server.Domain.Entities;

namespace Chat.Server.Domain.Commands
{
	public class PropagateMessageCommand : Command
	{
		public Client Sender { get; set; }
		public Client Target { get; set; }
		public IMessageContent Content { get; set; }

		public bool Private { get; set; }

		public bool IsTargeted => Target != null;
	}
}
