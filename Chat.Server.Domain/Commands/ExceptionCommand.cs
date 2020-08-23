using System;

namespace Chat.Server.Domain.Commands
{
	public class ExceptionCommand : Command
	{
		public ExceptionCommand(Exception exception)
		{
			Message = exception.Message;
		}

		public string Message { get; set; }
	}
}
