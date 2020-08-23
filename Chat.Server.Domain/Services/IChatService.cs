using Chat.Server.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Chat.Server.Domain
{
	public interface IChatService
	{
		Task ConnectAsync(Guid connectionUid);
		Task SendPrivateMessageAsync(Guid connectionUid, string theTargetedUser, IMessageContent theMessageContent);
		Task SendPublicMessageAsync(Guid connectionUid, IMessageContent theMessageContent);
		Task SendPublicTargetedMessageAsync(Guid connectionUid, string theTargetedUser, IMessageContent theMessageContent);
		Task SendNoticeMessageAsync(Guid connectionUid, string message);
		Task UpdateNicknameAsync(Guid connectionUid, string theNickname);
		Task UpdateRoomAsync(Guid connectionUid, string room);
		Task DisconnectAsync(Guid connectionUid);
	}
}