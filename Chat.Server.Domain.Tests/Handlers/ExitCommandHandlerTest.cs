using Chat.Server.Domain.Commands;
using Chat.Server.Domain.Commands.Handlers;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Chat.Server.Domain.Tests.Handlers
{
	public class ExitCommandHandlerTest
	{
		Mock<IChatService> ChatServiceMock;
		ExitCommandHandler ExitCommandHandler;

		[SetUp]
		public void SetUp()
		{
			ChatServiceMock = new Mock<IChatService>();
			ExitCommandHandler = new ExitCommandHandler(ChatServiceMock.Object);
		}

		[Test]
		public async Task Should_Disconnect_The_User()
		{
			var command = new ExitCommand(Guid.NewGuid());

			await ExitCommandHandler.ProcessAsync(command);

			ChatServiceMock.Verify(mock => mock.DisconnectAsync(command.ConnectionUid));
		}
	}
}
