using Chat.Server.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Chat.Server.Domain
{
	public interface IChatService
	{
		Task ConnectAsync(Guid theConnectionUidOfConnectedClient);
		Task SendPrivateMessageAsync(Guid theConnectionUid, string theTargetedUser, IMessageContent theMessageContent);
		Task SendPublicMessageAsync(Guid theConnectionUid, IMessageContent theMessageContent);
		Task SendPublicTargetedMessageAsync(Guid theConnectionUid, string theTargetedUser, IMessageContent theMessageContent);
		Task SendNoticeMessageAsync(Guid connectionUid, string message);
		Task UpdateNicknameAsync(Guid theConnectionUid, string theNickname);
	}
}