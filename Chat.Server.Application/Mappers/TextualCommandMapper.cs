﻿using Chat.Server.Application.Enumerators;
using Chat.Server.Application.Models;
using Chat.Server.Domain.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Chat.Server.Application.Mappers
{
	public class TextualCommandMapper
	{
		public Command ToCommand(Guid connectionUid, ClientState currentState, string message)
		{
			if (currentState == ClientState.WaitingNickname)
			{
				return ConvertToSetNicknameCommand(connectionUid, message);
			}

			if (currentState == ClientState.ReadyToConversation)
			{
				if (message.StartsWith("/t"))
				{
					return ConvertToTargetedMessage(connectionUid, ref message);
				}

				return ConvertToPublicMessage(connectionUid, message);
			}

			return null;
		}

		private static Command ConvertToSetNicknameCommand(Guid connectionUid, string message)
		{
			return new SetNicknameCommand
			{
				ConnectionUid = connectionUid,
				Nickname = message
			};
		}

		private static Command ConvertToPublicMessage(Guid connectionUid, string message)
		{
			return new SendMessageCommand
			{
				ConnectionUid = connectionUid,
				Content = new TextMessageContent(message),
				Private = false,
			};
		}

		private static Command ConvertToTargetedMessage(Guid connectionUid, ref string message)
		{
			Regex targetedMessageRegex = new Regex("/t ([A-z]*) (.*)");
			var match = targetedMessageRegex.Match(message);
			string targetedNickname = match.Groups[1].Value;
			message = match.Groups[2].Value;

			return new SendMessageCommand
			{
				ConnectionUid = connectionUid,
				Content = new TextMessageContent(message),
				TargetClientNickname = targetedNickname,
				Private = false,
			};
		}
	}
}