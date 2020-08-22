namespace Chat.Server.Domain.Entities
{
	public class TargetedMessage : Message
	{
		public TargetedMessage(Client sender, Client target, IMessageContent content)
			 : base(sender, content)
		{
			Sender = sender;
			Target = target;
			Content = content;
		}

		public Client Target { get; set; }
	}
}
