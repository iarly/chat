﻿using Chat.Server.Application.Enumerators;
using Chat.Server.Application.Models;
using Chat.Server.Domain.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chat.Server.Application.Mappers
{
	public class TextualCommandMapper
	{
		public Command ToCommand(Guid connectionUid, ClientState currentState, string message)
		{
			if (currentState == ClientState.WaitingNickname)
			{
				return new SetNicknameCommand
				{
					ConnectionUid = connectionUid,
					Nickname = message
				};
			}

			if (currentState == ClientState.ReadyToConversation)
			{
				return new SendMessageCommand
				{
					ConnectionUid = connectionUid,
					Content = new TextMessageContent(message),
					Private = false,
				};
			}

			return null;
		}
	}
}
