using System;

namespace Chat.Server.Domain.Exceptions
{
	public class InvalidNicknameException : Exception
	{
		public InvalidNicknameException()
			: base("Invalid nickname. Please use only A-Z, a-z, 0-9 or '-' characters.")
		{

		}
	}
}
