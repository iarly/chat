﻿using Chat.Server.Domain.Commands;
using Chat.Server.Domain.Commands.Handlers;
using Chat.Server.Domain.Exceptions;

namespace Chat.Server.Domain.Factories
{
	public class CommandHandlerFactory : ICommandHandlerFactory
	{
		public IChatService ChatService { get; }

		public CommandHandlerFactory(IChatService chatService)
		{
			ChatService = chatService;
		}

		public ICommandHandler GetHandler(Command command)
		{
			if (command.GetType() == typeof(SendMessageCommand))
				return new SendMessageCommandHandler(ChatService);

			if (command.GetType() == typeof(SetNicknameCommand))
				return new SetNicknameCommandHandler(ChatService);

			if (command.GetType() == typeof(ConnectCommand))
				return new ConnectCommandHandler(ChatService);

			if (command.GetType() == typeof(DisconnectCommand))
				return new DisconnectCommandHandler(ChatService);

			if (command.GetType() == typeof(ExitCommand))
				return new ExitCommandHandler(ChatService);

			if (command.GetType() == typeof(SetRoomCommand))
				return new SetRoomCommandHandler(ChatService);

			throw new CommandHandlerDoesNotExistsException();
		}
	}
}
