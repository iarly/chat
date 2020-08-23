using System;

namespace Chat.Server.Domain.Commands
{
	public class ExitCommand : Command
	{
		public ExitCommand(Guid connectionUid)
		{
			ConnectionUid = connectionUid;
		}
	}
}
