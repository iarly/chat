using System;

namespace Chat.Server.Domain.Exceptions
{
	public class UserHasNotsetTheNicknameException : Exception
	{
		public UserHasNotsetTheNicknameException()
			   : base("The user hasn't set they nickname")
		{

		}
	}
}
