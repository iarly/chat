namespace Chat.Server.Domain.Entities
{
	public class Message
	{
		public Message(Client sender, IMessageContent content)
		{
			Sender = sender;
			Content = content;
		}

		public Client Sender { get; set; }
		public IMessageContent Content { get; set; }
	}
}
