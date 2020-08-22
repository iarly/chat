using Chat.Server.Domain.Commands;
using Chat.Server.Domain.Commands.Handlers;
using Chat.Server.Domain.Exceptions;
using Chat.Server.Domain.Factories;
using Moq;
using NUnit.Framework;
using System;

namespace Chat.Server.Domain.Tests.Factories
{
	public class CommandHandlerFactoryTest
	{
		ICommandHandlerFactory CommandHandlerFactory;

		[SetUp]
		public void Setup()
		{
			CommandHandlerFactory = new CommandHandlerFactory(chatService: null);
		}

		[Test]
		public void Should_Get_ConnectCommandHandler_When_Command_Is_Connect()
		{
			var command = new ConnectCommand();

			var handler = CommandHandlerFactory.GetHandler(command);

			Assert.IsAssignableFrom<ConnectCommandHandler>(handler);
		}

		[Test]
		public void Should_Get_SendMessageCommandHandler_When_Command_Is_SendMessage()
		{
			var command = new SendMessageCommand();

			var handler = CommandHandlerFactory.GetHandler(command);

			Assert.IsAssignableFrom<SendMessageCommandHandler>(handler);
		}

		[Test]
		public void Should_Get_SetNicknameCommandHandler_When_Command_Is_SetNickname()
		{
			var command = new SetNicknameCommand();

			var handler = CommandHandlerFactory.GetHandler(command);

			Assert.IsAssignableFrom<SetNicknameCommandHandler>(handler);
		}

		[Test]
		public void Should_Throw_CommandHandlerDoesNotExistsException_When_Command_Does_Not_Have_Handler()
		{
			var commandWithoutHandler = Mock.Of<Command>();

			Assert.Throws<CommandHandlerDoesNotExistsException>(() => CommandHandlerFactory.GetHandler(commandWithoutHandler));
		}
	}
}
