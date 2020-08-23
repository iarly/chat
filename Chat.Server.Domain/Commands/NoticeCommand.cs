using System;

namespace Chat.Server.Domain.Commands
{
	public class NoticeCommand : Command
	{
		public NoticeCommand(string message)
		{
			Message = message;
		}

		public string Message { get; set; }
	}
}
