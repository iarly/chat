using Chat.Server.Domain.Entities;

namespace Chat.Server.Application.Models
{
	public class TextMessageContent : IMessageContent
	{
		public TextMessageContent(string text)
		{
			Text = text;
		}

		public string Text { get; set; }
	}
}
