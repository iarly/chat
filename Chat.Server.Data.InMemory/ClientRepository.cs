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
			lock (Dataset)
			{
				return Task.FromResult(Dataset.FirstOrDefault(client => client.Nickname == nickname));
			}
		}

		public Task<IEnumerable<Client>> GetAllClientInTheRoomAsync(string room)
		{
			lock (Dataset)
			{
				return Task.FromResult(Dataset.Where(client => client.Room == room).ToList().AsEnumerable());
			}
		}

		public Task<Client> GetByUidAsync(Guid theConnectionUid)
		{
			lock (Dataset)
			{
				return Task.FromResult(Dataset.FirstOrDefault(client => client.ConnectionUid == theConnectionUid));
			}
		}

		public Task StoreAsync(Client client)
		{
			lock (Dataset)
			{
				Dataset.Add(client);
				return Task.CompletedTask;
			}
		}

		public Task UpdateAsync(Client storedClient)
		{
			lock (Dataset)
			{
				var existentClient = Dataset.FirstOrDefault(client => client.ConnectionUid == storedClient.ConnectionUid);

				if (existentClient != null)
				{
					Dataset.Remove(existentClient);
				}

				Dataset.Add(storedClient);
			}

			return Task.CompletedTask;
		}
	}
}
