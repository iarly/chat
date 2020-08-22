using Chat.Server.Domain.Delegates;
using Chat.Server.Domain.Entities;
using System;

namespace Chat.Server.Domain
{
	public interface IDomainEvents
	{
		event RequestNicknameDelegate OnRequestNickname;
		event UserConnectsAtRoomDelegate OnUserConnectsAtRoom;
		event UserSentMessageDelegate OnUserSentMessage;
		event UserSentPrivateMessageDelegate OnUserSentPrivateMessage;

		void InvokeOnUserConnectsAtRoomEvent(Guid theConnectionUid, Client client);
		void InvokeOnUserSentMessage(string room, Message message);
		void InvokeOnUserSentPrivateMessageEvent(Client client, TargetedMessage message);
		void InvokeRequestNicknameEvent(Guid theConnectionUidOfConnectedClient);
	}
}