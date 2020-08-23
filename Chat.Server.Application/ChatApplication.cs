using Chat.Server.Application.Contracts;
using Chat.Server.Communicator;
using Chat.Server.Domain;
using Chat.Server.Domain.Commands;
using Chat.Server.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Chat.Server.Application
{
	public class ChatApplication
	{
		public IChatFacade ChatFacade { get; }
		public IMessageBroker MessageBroker { get; }
		public ICommunicator<Command> Communicator { get; }
		public IDomainEvents DomainEvents { get; }

		public ChatApplication(IChatFacade chatFacade,
			IMessageBroker messageBroker,
			ICommunicator<Command> communicator,
			IDomainEvents domainEvents)
		{
			ChatFacade = chatFacade;
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

		public Task StartAsync()
		{
			return Communicator.ListenAsync(new System.Threading.CancellationToken());
		}

		public void BindDomainEvents()
		{
			DomainEvents.OnCommand += DomainEvents_OnCommand;
		}

		private async Task Communicator_OnClientConnected(System.Guid connectionUid)
		{
			await ChatFacade.ProcessMessageAsync(new ConnectCommand()
			{
				ConnectionUid = connectionUid
			});

			await MessageBroker.SubscribeAsync(connectionUid, MessageBroker_OnCommand);
		}

		private async Task Communicator_OnClientSendCommand(Guid connectionUid, Command command)
		{
			command.ConnectionUid = connectionUid;

			await ChatFacade.ProcessMessageAsync(command);
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
			await Communicator.PublishAsync(command.ConnectionUid, command);
		}

		private async Task DomainEvents_OnCommand(Client target, Command command)
		{
			await MessageBroker.PublishAsync(command);
		}
	}
}
