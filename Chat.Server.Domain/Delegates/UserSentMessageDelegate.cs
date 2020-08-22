using Chat.Server.Domain.Entities;

namespace Chat.Server.Domain.Delegates
{
	public delegate void UserSentMessageDelegate(string room, Message message);
}
