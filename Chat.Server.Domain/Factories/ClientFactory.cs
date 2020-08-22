using Chat.Server.Domain.Entities;
using System;

namespace Chat.Server.Domain.Factories
{
	public class ClientFactory : IClientFactory
	{
		public Client Create(Guid connectionUid)
		{
			return new Client
			{
				ConnectionUid = connectionUid
			};
		}
	}
}
