using Chat.Server.Application.Mappers;
using Chat.Server.Application.Models;
using Chat.Server.Domain.Commands;
using Chat.Server.Domain.Enumerators;
using NUnit.Framework;
using System;

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
		public void Should_Convert_SetNicknameCommand_To_Text()
		{
			string expectedText = "Welcome to our chat! Please provide a nickname.";

			SetNicknameCommand messageCommand = new SetNicknameCommand();

			string actualText = Mapper.ToString(messageCommand);

			Assert.AreEqual(expectedText, actualText);
		}

		[Test]
		public void Should_Convert_PropagateMessageCommand_To_Text()
		{
			string expectedMessage = "What's app? No, chat!";
			string expectedSenderNickname = "Joey";
			string expectedText = $"{expectedSenderNickname} says to everybody: {expectedMessage}";

			PropagateMessageCommand messageCommand = new PropagateMessageCommand
			{
				Content = new TextMessageContent(expectedMessage),
				Private = false,
				Sender = new Domain.Entities.Client
				{
					Nickname = expectedSenderNickname
				}
			};

			string actualText = Mapper.ToString(messageCommand);

			Assert.AreEqual(expectedText, actualText);
		}

		[Test]
		public void Should_Convert_PropagateMessageCommand_To_TargetedMessageText()
		{
			string expectedMessage = "Hey, how are you doin?";
			string expectedSenderNickname = "Joey";
			string expectedTargetNickname = "Jospeh";
			string expectedText = $"{expectedSenderNickname} says to {expectedTargetNickname}: {expectedMessage}";

			PropagateMessageCommand messageCommand = new PropagateMessageCommand
			{
				Content = new TextMessageContent(expectedMessage),
				Private = false,
				Sender = new Domain.Entities.Client
				{
					Nickname = expectedSenderNickname
				},
				Target = new Domain.Entities.Client
				{
					Nickname = expectedTargetNickname
				}
			};

			string actualText = Mapper.ToString(messageCommand);

			Assert.AreEqual(expectedText, actualText);
		}


		[Test]
		public void Should_Convert_PropagateMessageCommand_To_PrivateMessageText()
		{
			string expectedMessage = "Hey, how are you doin?";
			string expectedSenderNickname = "Joey";
			string expectedTargetNickname = "Jospeh";
			string expectedText = $"{expectedSenderNickname} says privately to {expectedTargetNickname}: {expectedMessage}";

			PropagateMessageCommand messageCommand = new PropagateMessageCommand
			{
				Content = new TextMessageContent(expectedMessage),
				Private = true,
				Sender = new Domain.Entities.Client
				{
					Nickname = expectedSenderNickname
				},
				Target = new Domain.Entities.Client
				{
					Nickname = expectedTargetNickname
				}
			};

			string actualText = Mapper.ToString(messageCommand);

			Assert.AreEqual(expectedText, actualText);
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
			string messageSentByClient = "/to Joey Hi Joey";
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
			string messageSentByClient = "/private Sylvia Hi my darling.. :)";
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

		[Test]
		public void Should_Convert_To_SetRoomCommand()
		{
			string messageSentByClient = "/room OtherRoom";
			string expectedRoom = "OtherRoom";
			ClientState currentStateOfClient = ClientState.ReadyToConversation;
			Guid connectionUid = Guid.NewGuid();

			Command command = Mapper.ToCommand(connectionUid, currentStateOfClient, messageSentByClient);

			Assert.IsAssignableFrom<SetRoomCommand>(command);
			Assert.AreEqual(expectedRoom, ((SetRoomCommand)command).Room);
		}
	}
}
