namespace Chat.Server.Domain.Entities
{
	public class Message
	{
		public Message(Client sender, string text)
		{
			Sender = sender;
			Text = text;
		}

		public Client Sender { get; set; }
		public string Text { get; set; }
	}
}
