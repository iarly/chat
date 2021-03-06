﻿using Chat.Server.Domain.Entities;

namespace Chat.Server.Domain.Commands
{
	public class SendMessageCommand : Command
	{
		public string TargetClientNickname { get; set; }
		public IMessageContent Content { get; set; }
		public bool Private { get; set; }

		public bool IsTargeted => !string.IsNullOrEmpty(TargetClientNickname);
	}
}
