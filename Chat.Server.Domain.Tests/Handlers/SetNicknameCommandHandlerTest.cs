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
	public class SetNicknameCommandHandlerTest
	{
		Mock<IChatService> ChatServiceMock;
		SetNicknameCommandHandler SetNicknameCommandHandler;

		[SetUp]
		public void SetUp()
		{
			ChatServiceMock = new Mock<IChatService>();
			SetNicknameCommandHandler = new SetNicknameCommandHandler(ChatServiceMock.Object);
		}

		[Test]
		public void Should_UpdateNicknameAsync()
		{
			var connectionUid = Guid.NewGuid();
			var nickname = "Will";

			var command = new SetNicknameCommand()
			{
				ConnectionUid = connectionUid,				
				Nickname = nickname
			};

			SetNicknameCommandHandler.ProcessAsync(command);

			ChatServiceMock.Verify(mock => mock.UpdateNicknameAsync(connectionUid, nickname));
		}
	}
}
