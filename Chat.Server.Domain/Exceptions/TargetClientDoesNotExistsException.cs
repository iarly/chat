using System;

namespace Chat.Server.Domain.Exceptions
{
	public class TargetClientDoesNotExistsException : Exception
	{
		public TargetClientDoesNotExistsException()
			   : base("The target client doesn't exists")
		{

		}
	}
}
