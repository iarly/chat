using System;

namespace Chat.Server.Domain.Exceptions
{
	public class CommandHandlerDoesNotExistsException : Exception
	{
		public CommandHandlerDoesNotExistsException()
			   : base("The command handler doesn't exists")
		{

		}
	}
}
