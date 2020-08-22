using Chat.Server.Domain.Entities;
using System;

namespace Chat.Server.Domain.Factories
{
	public interface IClientFactory
	{
		Client Create(Guid theConnectionUidOfConnectedClient);
	}
}
