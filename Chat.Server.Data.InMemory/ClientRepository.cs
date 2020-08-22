using Chat.Server.Domain.Entities;
using Chat.Server.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.Server.Data.InMemory
{
	public class ClientRepository : IClientRepository
	{
		public List<Client> Dataset = new List<Client>();

		public Task<Client> FindByNicknameAsync(string nickname)
		{
			return Task.FromResult(Dataset.FirstOrDefault(client => client.Nickname == nickname));
		}

		public Task<IEnumerable<Client>> GetAllClientInTheRoomAsync(string room)
		{
			return Task.FromResult(Dataset.Where(client => client.Room == room));
		}

		public Task<Client> GetByUidAsync(Guid theConnectionUid)
		{
			return Task.FromResult(Dataset.FirstOrDefault(client => client.ConnectionUid == theConnectionUid));
		}

		public Task StoreAsync(Client client)
		{
			Dataset.Add(client);
			return Task.CompletedTask;
		}

		public async Task UpdateAsync(Client storedClient)
		{
			var existentClient = await GetByUidAsync(storedClient.ConnectionUid);
			
			if (existentClient != null)
			{
				Dataset.Remove(existentClient);
			}

			await StoreAsync(storedClient);
		}
	}
}
