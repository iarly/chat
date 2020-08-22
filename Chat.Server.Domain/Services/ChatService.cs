using Chat.Server.Domain.Commands;
using Chat.Server.Domain.Delegates;
using Chat.Server.Domain.Entities;
using Chat.Server.Domain.Exceptions;
using Chat.Server.Domain.Factories;
using Chat.Server.Domain.Repositories;
using System;
using System.Threading.Tasks;

namespace Chat.Server.Domain
{
	public class ChatService : IChatService
	{
		protected IDomainEvents DomainEvents { get; }
		protected IClientFactory ClientFactory { get; }
		protected IClientRepository ClientRepository { get; }

		public ChatService(IDomainEvents domainEvents,
			IClientFactory clientFactory,
			IClientRepository clientRepository)
		{
			DomainEvents = domainEvents;
			ClientFactory = clientFactory;
			ClientRepository = clientRepository;
		}

		public async Task ConnectAsync(Guid connectionUid)
		{
			Client client = ClientFactory.Create(connectionUid);

			await ClientRepository.StoreAsync(client);

			DomainEvents.SendCommand(client, new SetNicknameCommand()
			{
				ConnectionUid = connectionUid
			});
		}

		public async Task UpdateNicknameAsync(Guid theConnectionUid, string theNickname)
		{
			Client client = await ClientRepository.GetByUidAsync(theConnectionUid);

			client.Nickname = theNickname;

			UpdateClientRoomWhenRoomIsNotSet(theConnectionUid, client);

			await ClientRepository.UpdateAsync(client);
		}

		public async Task SendPublicMessageAsync(Guid theConnectionUid, IMessageContent theMessageContent)
		{
			Client client = await ClientRepository.GetByUidAsync(theConnectionUid);

			ThrowsErrorWhenNicknameIsNullOrEmpty(client);

			ThrowsErrorWhenRoomIsNullOrEmpty(client);

			await SendTheMessageForEverbodyInTheRoom(client.Room, new Message(client, theMessageContent));
		}

		public async Task SendPublicTargetedMessageAsync(Guid theConnectionUid, string theTargetedUser, IMessageContent theMessageContent)
		{
			Client sender = await ClientRepository.GetByUidAsync(theConnectionUid);

			ThrowsErrorWhenNicknameIsNullOrEmpty(sender);

			ThrowsErrorWhenRoomIsNullOrEmpty(sender);

			Client target = await ClientRepository.FindByNicknameAsync(theTargetedUser);

			ThrowsErrorWhenTargetClientDoesNotExists(target);

			await SendTheMessageForEverbodyInTheRoom(sender.Room, new TargetedMessage(sender, target, theMessageContent));
		}

		public async Task SendPrivateMessageAsync(Guid theConnectionUid, string theTargetedUser, IMessageContent theMessageContent)
		{
			Client sender = await ClientRepository.GetByUidAsync(theConnectionUid);

			ThrowsErrorWhenNicknameIsNullOrEmpty(sender);

			ThrowsErrorWhenRoomIsNullOrEmpty(sender);

			Client target = await ClientRepository.FindByNicknameAsync(theTargetedUser);

			ThrowsErrorWhenTargetClientDoesNotExists(target);

			SendDirectMessageForTargetedClient(target, new TargetedMessage(sender, target, theMessageContent));
		}

		private static void ThrowsErrorWhenTargetClientDoesNotExists(Client target)
		{
			if (target == null)
			{
				throw new TargetClientDoesNotExistsException();
			}
		}

		private static void ThrowsErrorWhenNicknameIsNullOrEmpty(Client client)
		{
			if (string.IsNullOrEmpty(client.Nickname))
			{
				throw new UserHasNotsetTheNicknameException();
			}
		}

		private static void ThrowsErrorWhenRoomIsNullOrEmpty(Client client)
		{
			if (string.IsNullOrEmpty(client.Room))
			{
				throw new UserHasNotSetTheRoomException();
			}
		}

		private void SendDirectMessageForTargetedClient(Client destination, TargetedMessage message)
		{
			DomainEvents.SendCommand(destination, new PropagateMessageCommand()
			{
				ConnectionUid = destination.ConnectionUid,
				Target = destination,
				Sender = message.Sender,
				Content = message.Content,
				Private = true
			});
		}

		private async Task SendTheMessageForEverbodyInTheRoom(string room, Message message)
		{
			var clients = await ClientRepository.GetAllClientInTheRoomAsync(room);
			var targetedMessage = message as TargetedMessage;

			foreach (var client in clients)
			{
				DomainEvents.SendCommand(client, new PropagateMessageCommand()
				{
					ConnectionUid = client.ConnectionUid,
					Target = targetedMessage?.Target,
					Sender = message.Sender,
					Content = message.Content,
					Private = false,
				});
			}
		}

		private void UpdateClientRoomWhenRoomIsNotSet(Guid connectionUid, Client client)
		{
			if (string.IsNullOrEmpty(client.Room))
			{
				client.Room = "general";
			}
		}

	}
}
