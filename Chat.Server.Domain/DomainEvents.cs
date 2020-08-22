using Chat.Server.Domain.Delegates;
using Chat.Server.Domain.Entities;
using System;

namespace Chat.Server.Domain
{
	public class DomainEvents : IDomainEvents
	{
		public event RequestNicknameDelegate OnRequestNickname;

		public event UserConnectsAtRoomDelegate OnUserConnectsAtRoom;

		public event UserSendMessageDelegate OnUserSendMessage;

		public void InvokeOnUserSendMessage(Client destination, Message message)
		{
			OnUserSendMessage.Invoke(destination, message);
		}

		public void InvokeRequestNicknameEvent(Guid theConnectionUidOfConnectedClient)
		{
			if (OnRequestNickname != null)
			{
				OnRequestNickname.Invoke(theConnectionUidOfConnectedClient);
			}
		}

		public void InvokeOnUserConnectsAtRoomEvent(Guid theConnectionUid, Client client)
		{
			if (OnUserConnectsAtRoom != null)
			{
				OnUserConnectsAtRoom.Invoke(theConnectionUid, client);
			}
		}
	}
}
