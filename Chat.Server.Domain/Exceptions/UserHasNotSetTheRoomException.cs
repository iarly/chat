using System;

namespace Chat.Server.Domain.Exceptions
{
	public class UserHasNotSetTheRoomException : Exception
	{
		public UserHasNotSetTheRoomException()
			   : base("The user hasn't entered at any room")
		{

		}
	}
}
