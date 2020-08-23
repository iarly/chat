using Chat.Server.Domain.Enumerators;
using System;

namespace Chat.Server.Domain.Entities
{
	public class Client
	{
		public Guid ConnectionUid { get; set; }
		public string Nickname { get; set; }
		public string Room { get; set; }
		public bool HasNickname => !string.IsNullOrEmpty(Nickname);
		public ClientState State => HasNickname ? ClientState.ReadyToConversation : ClientState.WaitingNickname;
	}
}
