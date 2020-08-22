using Chat.Server.Domain.Delegates;
using System;
using System.Threading.Tasks;

namespace Chat.Server.Domain
{
	public class ChatFacade
	{
		public event RequestNicknameDelegate OnRequestNickname;

		public Task ConnectAsync(Guid theConnectionUidOfConnectedClient)
		{
			OnRequestNickname.Invoke(theConnectionUidOfConnectedClient);
			return Task.CompletedTask;
		}
	}
}
