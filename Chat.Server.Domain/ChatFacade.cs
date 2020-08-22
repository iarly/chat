using Chat.Server.Domain.Delegates;
using Chat.Server.Domain.Entities;
using Chat.Server.Domain.Exceptions;
using Chat.Server.Domain.Factories;
using Chat.Server.Domain.Repositories;
using System;
using System.Threading.Tasks;

namespace Chat.Server.Domain
{
	public class ChatFacade
	{
		public IClientFactory ClientFactory { get; }
		protected IClientRepository ClientRepository { get; }

		public ChatFacade(IClientFactory clientFactory,
			IClientRepository clientRepository)
		{
			ClientFactory = clientFactory;
			ClientRepository = clientRepository;
		}

		public event RequestNicknameDelegate OnRequestNickname;
		public event UserConnectsAtRoomDelegate OnUserConnectsAtRoom;
		public event UserSentMessageDelegate OnUserSentMessage;

		public async Task ConnectAsync(Guid theConnectionUidOfConnectedClient)
		{
			Client client = ClientFactory.Create(theConnectionUidOfConnectedClient);

			await ClientRepository.StoreAsync(client);

			InvokeRequestNicknameEvent(theConnectionUidOfConnectedClient);
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

			SendTheMessageForEverbodyInTheRoom(client.Room, new Message(client, theMessageContent));
		}

		public async Task SendPublicTargetedMessageAsync(Guid theConnectionUid, string theTargetedUser, IMessageContent theMessageContent)
		{
			Client sender = await ClientRepository.GetByUidAsync(theConnectionUid);

			ThrowsErrorWhenNicknameIsNullOrEmpty(sender);

			ThrowsErrorWhenRoomIsNullOrEmpty(sender);

			Client target = await ClientRepository.FindByNicknameAsync(theTargetedUser);

			ThrowsErrorWhenTargetClientDoesNotExists(target);

			SendTheMessageForEverbodyInTheRoom(sender.Room, new TargetedMessage(sender, target, theMessageContent));
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

		private void SendTheMessageForEverbodyInTheRoom(string room, Message message)
		{
			OnUserSentMessage.Invoke(room, message);
		}

		private void UpdateClientRoomWhenRoomIsNotSet(Guid theConnectionUid, Client client)
		{
			if (string.IsNullOrEmpty(client.Room))
			{
				client.Room = "general";
				InvokeOnUserConnectsAtRoomEvent(theConnectionUid, client);
			}
		}

		private void InvokeRequestNicknameEvent(Guid theConnectionUidOfConnectedClient)
		{
			if (OnRequestNickname != null)
			{
				OnRequestNickname.Invoke(theConnectionUidOfConnectedClient);
			}
		}

		private void InvokeOnUserConnectsAtRoomEvent(Guid theConnectionUid, Client client)
		{
			if (OnUserConnectsAtRoom != null)
			{
				OnUserConnectsAtRoom.Invoke(theConnectionUid, client);
			}
		}

	}
}
