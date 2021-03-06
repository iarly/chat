﻿using Chat.Server.Application.Contracts;
using Chat.Server.Application.Mappers;
using Chat.Server.Communicator;
using Chat.Server.Domain;
using Chat.Server.Domain.Commands;
using Chat.Server.Domain.Entities;
using Chat.Server.Domain.Enumerators;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.Server.Application
{
	public class ChatApplication
	{
		public IChatFacade ChatFacade { get; }
		public ITextualCommandMapper TextualCommandMapper { get; }
		public IMessageBroker MessageBroker { get; }
		public ICommunicator<string> Communicator { get; }
		public IDomainEvents DomainEvents { get; }

		public ChatApplication(IChatFacade chatFacade,
			ITextualCommandMapper textualCommandMapper,
			IMessageBroker messageBroker,
			ICommunicator<string> communicator,
			IDomainEvents domainEvents)
		{
			ChatFacade = chatFacade;
			TextualCommandMapper = textualCommandMapper;
			MessageBroker = messageBroker;
			Communicator = communicator;
			DomainEvents = domainEvents;

			BindCommunicatorEvents();
			BindDomainEvents();
		}

		public void BindCommunicatorEvents()
		{
			Communicator.OnClientConnected += Communicator_OnClientConnected;
			Communicator.OnClientDisconnected += Communicator_OnClientDisconnected;
			Communicator.OnClientSendCommand += Communicator_OnClientSendCommand;
		}

		public Task StartAsync(CancellationToken? cancellationToken = null)
		{
			return Communicator.ListenAsync(cancellationToken ?? new CancellationToken());
		}

		public void BindDomainEvents()
		{
			DomainEvents.OnCommand += DomainEvents_OnCommand;
			DomainEvents.OnDisconnectCommand += DomainEvents_OnDisconnectCommand;
		}

		private async Task DomainEvents_OnDisconnectCommand(Client destination, Command command)
		{
			await Communicator.DisconnectAsync(destination.ConnectionUid);
		}

		private async Task Communicator_OnClientConnected(System.Guid connectionUid)
		{
			await MessageBroker.SubscribeAsync(connectionUid, MessageBroker_OnCommand);

			await ChatFacade.ProcessMessageAsync(new ConnectCommand()
			{
				ConnectionUid = connectionUid
			});
		}

		private async Task Communicator_OnClientSendCommand(Guid connectionUid, string textualCommand)
		{
			ClientState clientState = await DiscoverClientStateAsync(connectionUid);

			Command command = TextualCommandMapper.ToCommand(connectionUid, clientState, textualCommand);

			await ChatFacade.ProcessMessageAsync(command);
		}

		private async Task<ClientState> DiscoverClientStateAsync(Guid connectionUid)
		{
			var client = await ChatFacade.GetClientByUidAsync(connectionUid);

			return client.State;
		}

		private async Task Communicator_OnClientDisconnected(Guid connectionUid)
		{
			await ChatFacade.ProcessMessageAsync(new DisconnectCommand()
			{
				ConnectionUid = connectionUid
			});
		}

		private async Task MessageBroker_OnCommand(Command command)
		{
			string textualCommand = TextualCommandMapper.ToString(command);

			await Communicator.PublishAsync(command.ConnectionUid, textualCommand);
		}

		private async Task DomainEvents_OnCommand(Client target, Command command)
		{
			await MessageBroker.PublishAsync(command);
		}
	}
}
