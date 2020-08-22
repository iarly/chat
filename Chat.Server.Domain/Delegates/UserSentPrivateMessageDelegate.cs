using Chat.Server.Domain.Entities;

namespace Chat.Server.Domain.Delegates
{
	public delegate void UserSentPrivateMessageDelegate(Client target, Message message);
}
