using Chat.Server.Domain.Entities;

namespace Chat.Server.Communicator.Sockets.Tests.Stubs
{
	public class TextualContent : IMessageContent
	{
		public TextualContent()
		{

		}
		public TextualContent(string text)
		{
			Text = text;
		}
		public string Text { get; set; }
	}
}
