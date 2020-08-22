using Chat.Server.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Chat.Server.Domain.Repositories
{
	public interface IClientRepository
	{
		Task StoreAsync(Client client);
		Task UpdateAsync(Client storedClient);
		Task<Client> GetByUidAsync(Guid theConnectionUid);
	}
}
