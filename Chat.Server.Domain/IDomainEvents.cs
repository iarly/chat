using Chat.Server.Domain.Delegates;
using Chat.Server.Domain.Entities;
using System;

namespace Chat.Server.Domain
{
	public interface IDomainEvents
	{
		event RequestNicknameDelegate OnRequestNickname;
		event UserConnectsAtRoomDelegate OnUserConnectsAtRoom;
		event UserSendMessageDelegate OnUserSendMessage;

		void InvokeOnUserSendMessage(Client destination, Message message);
		void InvokeOnUserConnectsAtRoomEvent(Guid theConnectionUid, Client client);
		void InvokeRequestNicknameEvent(Guid theConnectionUidOfConnectedClient);
	}
}