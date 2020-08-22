using Chat.Server.Domain.Commands;
using Chat.Server.Domain.Factories;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Chat.Server.Domain.Tests
{
	public class ChatFacadeTest
	{
		private Mock<IDomainEvents> DomainEventsMock;
		private Mock<ICommandService> CommandServiceMock;
		private Mock<IChatService> ChatServiceMock;
		private Mock<ICommandHandlerFactory> CommandHandlerFactoryMock;

		ChatFacade ChatFacade;

		[SetUp]
		public void Initialize()
		{
			CommandHandlerFactoryMock = new Mock<ICommandHandlerFactory>();

			ChatFacade = new ChatFacade(CommandHandlerFactoryMock.Object);
		}

		[Test]
		public async Task Should_Process_Command_When_Command_Exists()
		{
			Command command = Mock.Of<Command>();
			Guid connectionUid = Guid.NewGuid();
			Mock<ICommandHandler> commandHandler = new Mock<ICommandHandler>();

			CommandHandlerFactoryMock.Setup(mock => mock.GetHandler(command)).Returns(commandHandler.Object);

			await ChatFacade.ProcessMessageAsync(connectionUid, command);

			CommandHandlerFactoryMock.Verify(mock => mock.GetHandler(command), Times.Once);

			commandHandler.Verify(mock => mock.Process(command), Times.Once);
		}
	}
}
