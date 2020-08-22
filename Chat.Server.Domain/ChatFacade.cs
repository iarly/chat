using Chat.Server.Domain.Delegates;
using Chat.Server.Domain.Entities;
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

		public async Task ConnectAsync(Guid theConnectionUidOfConnectedClient)
		{
			Client client = ClientFactory.Create(theConnectionUidOfConnectedClient);

			await ClientRepository.StoreAsync(client);

			if (OnRequestNickname != null)
			{
				OnRequestNickname.Invoke(theConnectionUidOfConnectedClient);
			}
		}

		public async Task UpdateNicknameAsync(Guid theConnectionUid, string theNickname)
		{
			Client client = await ClientRepository.GetByUidAsync(theConnectionUid);

			client.Nickname = theNickname;

			if (string.IsNullOrEmpty(client.Room))
			{
				client.Room = "general";
				OnUserConnectsAtRoom.Invoke(theConnectionUid, client);
			}

			await ClientRepository.UpdateAsync(client);
		}
	}
}
