using System;

namespace Chat.Server.Domain.Entities
{
	public class Client
	{
		public Guid ConnectionUid { get; set; }
		public string Nickname { get; set; }
		public string Room { get; set; }
	}
}
