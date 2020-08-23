using System;

namespace Chat.Server.Domain.Exceptions
{
	public class CommandDoesNotExistsException : Exception
	{
		public CommandDoesNotExistsException()
			: base("The command doesn't exists")
		{
		}
	}
}
