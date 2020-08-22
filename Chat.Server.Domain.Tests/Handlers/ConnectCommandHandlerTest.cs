using Chat.Server.Domain.Commands;
using Chat.Server.Domain.Commands.Handlers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chat.Server.Domain.Tests.Handlers
{
	public class ConnectCommandHandlerTest
	{
		Mock<IChatService> ChatServiceMock;
		ConnectCommandHandler ConnectCommandHandler;

		[SetUp]
		public void SetUp()
		{
			ChatServiceMock = new Mock<IChatService>();
			ConnectCommandHandler = new ConnectCommandHandler(ChatServiceMock.Object);
		}

		[Test]
		public void Should_Invoke_Client_Connection()
		{
			var connectionUid = Guid.NewGuid();
			var command = new ConnectCommand()
			{
				ConnectionUid = connectionUid
			};

			ConnectCommandHandler.ProcessAsync(command);

			ChatServiceMock.Verify(mock => mock.ConnectAsync(connectionUid));
		}
	}
}
