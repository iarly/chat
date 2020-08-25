using Chat.Server.Domain.Entities;
using Chat.Server.Domain.Repositories;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chat.Server.Data.MongoDb
{
	public class ClientRepository : IClientRepository
	{
		protected readonly IMongoCollection<Client> DbSet;
		protected MongoClient MongoClient { get; }
		protected IMongoDatabase Database { get; }

		public ClientRepository(string mongoDbConnection, string mongoDbDatabase)
		{
			MongoClient = new MongoClient(mongoDbConnection);
			Database = MongoClient.GetDatabase(mongoDbDatabase);

			BsonClassMap.RegisterClassMap<Client>(cm =>
			{
				cm.AutoMap();
				cm.MapIdField(client => client.ConnectionUid);
			});

			DbSet = Database.GetCollection<Client>(nameof(Client));
		}

		public async Task<Client> FindByNicknameAsync(string nickname)
		{
			var documents = await DbSet.FindAsync(client => client.Nickname == nickname);

			var document = documents.FirstOrDefault();

			return document;
		}

		public async Task<IEnumerable<Client>> GetAllClientInTheRoomAsync(string room)
		{
			var documents = await DbSet.FindAsync(client => client.Room == room);

			return documents.ToEnumerable();
		}

		public async Task<Client> GetByUidAsync(Guid theConnectionUid)
		{
			var documents = await DbSet.FindAsync(client => client.ConnectionUid == theConnectionUid);

			var document = documents.FirstOrDefault();

			return document;
		}

		public Task StoreAsync(Client client)
		{
			return DbSet.InsertOneAsync(client);
		}

		public Task UpdateAsync(Client updatedClient)
		{
			return DbSet.ReplaceOneAsync(client => client.ConnectionUid == updatedClient.ConnectionUid, updatedClient);
		}
	}
}
