﻿using Chat.Server.Domain.Commands;
using Chat.Server.Domain.Exceptions;
using Chat.Server.Domain.Factories;
using Chat.Server.Domain.Repositories;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Chat.Server.Domain.Tests
{
	public class ChatFacadeTest
	{
		private Mock<IClientRepository> ClientRepositoryMock;
		private Mock<ICommandHandlerFactory> CommandHandlerFactoryMock;

		ChatFacade ChatFacade;

		[SetUp]
		public void Initialize()
		{
			ClientRepositoryMock = new Mock<IClientRepository>();
			CommandHandlerFactoryMock = new Mock<ICommandHandlerFactory>();

			ChatFacade = new ChatFacade(ClientRepositoryMock.Object, 
				CommandHandlerFactoryMock.Object);
		}

		[Test]
		public async Task Should_GetClientByUid()
		{
			Guid connectionUid = Guid.NewGuid();

			await ChatFacade.GetClientByUidAsync(connectionUid);

			ClientRepositoryMock.Verify(mock => mock.GetByUidAsync(connectionUid), Times.Once);
		}

		[Test]
		public async Task Should_Process_Command_When_Command_Exists()
		{
			Command command = Mock.Of<Command>();
			Mock<ICommandHandler> commandHandler = new Mock<ICommandHandler>();

			CommandHandlerFactoryMock.Setup(mock => mock.GetHandler(command)).Returns(commandHandler.Object);

			await ChatFacade.ProcessMessageAsync(command);

			CommandHandlerFactoryMock.Verify(mock => mock.GetHandler(command), Times.Once);

			commandHandler.Verify(mock => mock.ProcessAsync(command), Times.Once);
		}

		[Test]
		public void Should_Throw_NotFoundCommandHandlerException_When_CommandHandler_Does_Not_Exists()
		{
			Command command = Mock.Of<Command>();
			
			Assert.ThrowsAsync<CommandDoesNotExistsException>(async () => await ChatFacade.ProcessMessageAsync(command));
		}
	}
}
