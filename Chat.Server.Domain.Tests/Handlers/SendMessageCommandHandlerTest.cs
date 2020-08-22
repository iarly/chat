using Chat.Server.Domain.Commands;
using Chat.Server.Domain.Commands.Handlers;
using Chat.Server.Domain.Entities;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chat.Server.Domain.Tests.Handlers
{
	public class SendMessageCommandHandlerTest
	{
		Mock<IChatService> ChatServiceMock;
		SendMessageCommandHandler SendMessageCommand;

		[SetUp]
		public void SetUp()
		{
			ChatServiceMock = new Mock<IChatService>();
			SendMessageCommand = new SendMessageCommandHandler(ChatServiceMock.Object);
		}

		[Test]
		public void Should_SendPublicMessage_When_Message_Is_Public_And_Not_Targeted()
		{
			var connectionUid = Guid.NewGuid();
			var messageContent = Mock.Of<IMessageContent>();

			var command = new SendMessageCommand()
			{
				ConnectionUid = connectionUid,				
				MessageContent = messageContent,
				Private = false
			};

			SendMessageCommand.ProcessAsync(command);

			ChatServiceMock.Verify(mock => mock.SendPublicMessageAsync(connectionUid, messageContent));
		}

		[Test]
		public void Should_SendPublicMessage_When_Message_Is_Public_And_Targeted()
		{
			var targetedClient = "Jefrey";
			var connectionUid = Guid.NewGuid();
			var messageContent = Mock.Of<IMessageContent>();

			var command = new SendMessageCommand()
			{
				ConnectionUid = connectionUid,
				MessageContent = messageContent,
				Private = false,
				TargetClient = targetedClient
			};

			SendMessageCommand.ProcessAsync(command);

			ChatServiceMock.Verify(mock => mock.SendPublicTargetedMessageAsync(connectionUid, targetedClient, messageContent));
		}

		[Test]
		public void Should_SendPrivateMessageAsync_When_Message_Is_Private()
		{
			var targetedClient = "Jefrey";
			var connectionUid = Guid.NewGuid();
			var messageContent = Mock.Of<IMessageContent>();

			var command = new SendMessageCommand()
			{
				ConnectionUid = connectionUid,
				MessageContent = messageContent,
				TargetClient = targetedClient,
				Private = true
			};

			SendMessageCommand.ProcessAsync(command);

			ChatServiceMock.Verify(mock => mock.SendPrivateMessageAsync(connectionUid, targetedClient, messageContent));
		}
	}
}
