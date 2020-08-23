using Chat.Server.Application.Enumerators;
using Chat.Server.Application.Mappers;
using Chat.Server.Application.Models;
using Chat.Server.Domain.Commands;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chat.Server.Application.Tests.Mappers
{
	public class TextualCommandMapperTest
	{
		private TextualCommandMapper Mapper;

		[SetUp]
		public void Setup()
		{
			Mapper = new TextualCommandMapper();
		}

		[Test]
		public void Should_Convert_Message_To_SetNicknameCommand_When_WaitingNickaname()
		{
			string messageSentByClient = "Joey";
			ClientState currentStateOfClient = ClientState.WaitingNickname;
			Guid connectionUid = Guid.NewGuid();

			Command command = Mapper.ToCommand(connectionUid, currentStateOfClient, messageSentByClient);

			Assert.IsAssignableFrom<SetNicknameCommand>(command);
			Assert.AreEqual(messageSentByClient, ((SetNicknameCommand)command).Nickname);
			Assert.AreEqual(connectionUid, ((SetNicknameCommand)command).ConnectionUid);
		}

		[Test]
		public void Should_Convert_Message_To_SendMessageCommand_When_ReadyToConversation()
		{
			string messageSentByClient = "Hi Joey";
			ClientState currentStateOfClient = ClientState.ReadyToConversation;
			Guid connectionUid = Guid.NewGuid();

			Command command = Mapper.ToCommand(connectionUid, currentStateOfClient, messageSentByClient);

			Assert.IsAssignableFrom<SendMessageCommand>(command);
			Assert.AreEqual(messageSentByClient, ((TextMessageContent)((SendMessageCommand)command).Content).Text);
			Assert.AreEqual(connectionUid, ((SendMessageCommand)command).ConnectionUid);
			Assert.AreEqual(false, ((SendMessageCommand)command).Private);
			Assert.AreEqual(null, ((SendMessageCommand)command).TargetClientNickname);
		}

		[Test]
		public void Should_Convert_TargetedMessage_To_SendMessageCommand_When_ReadyToConversation()
		{
			string messageSentByClient = "/t Joey Hi Joey";
			string targetClientNickname = "Joey";
			string expectedMessage = "Hi Joey";
			ClientState currentStateOfClient = ClientState.ReadyToConversation;
			Guid connectionUid = Guid.NewGuid();

			Command command = Mapper.ToCommand(connectionUid, currentStateOfClient, messageSentByClient);

			Assert.IsAssignableFrom<SendMessageCommand>(command);
			Assert.AreEqual(expectedMessage, ((TextMessageContent)((SendMessageCommand)command).Content).Text);
			Assert.AreEqual(connectionUid, ((SendMessageCommand)command).ConnectionUid);
			Assert.AreEqual(false, ((SendMessageCommand)command).Private);
			Assert.AreEqual(targetClientNickname, ((SendMessageCommand)command).TargetClientNickname);
		}

		[Test]
		public void Should_Convert_PrivateMessage_To_SendMessageCommand_When_ReadyToConversation()
		{
			string messageSentByClient = "/p Sylvia Hi my darling.. :)";
			string targetClientNickname = "Sylvia";
			string expectedMessage = "Hi my darling.. :)";
			ClientState currentStateOfClient = ClientState.ReadyToConversation;
			Guid connectionUid = Guid.NewGuid();

			Command command = Mapper.ToCommand(connectionUid, currentStateOfClient, messageSentByClient);

			Assert.IsAssignableFrom<SendMessageCommand>(command);
			Assert.AreEqual(expectedMessage, ((TextMessageContent)((SendMessageCommand)command).Content).Text);
			Assert.AreEqual(connectionUid, ((SendMessageCommand)command).ConnectionUid);
			Assert.AreEqual(true, ((SendMessageCommand)command).Private);
			Assert.AreEqual(targetClientNickname, ((SendMessageCommand)command).TargetClientNickname);
		}
	}
}
