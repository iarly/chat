using Chat.Server.Domain.Entities;

namespace Chat.Server.Domain.Delegates
{
	public delegate void UserSendMessageDelegate(Client destination, Message message);
}
