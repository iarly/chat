using System;

namespace Chat.Server.Domain.Commands
{
	public abstract class Command
	{
		public Guid ConnectionUid { get; set; }
	}
}
