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
		protected IMongoCollection<Client> _DbSet;
		protected MongoClient MongoClient { get; set; }
		protected IMongoDatabase Database { get; set; }
		protected string MongoDbConnection { get; }
		protected string MongoDbDatabase { get; }

		protected IMongoCollection<Client> DbSet
		{
			get
			{
				if (_DbSet == null)
				{
					MongoClient = new MongoClient(MongoDbConnection);
					Database = MongoClient.GetDatabase(MongoDbDatabase);
					_DbSet = Database.GetCollection<Client>(nameof(Client));
				}

				return _DbSet;
			}
		}

		public ClientRepository(string mongoDbConnection, string mongoDbDatabase)
		{
			BsonClassMap.RegisterClassMap<Client>(cm =>
			{
				cm.AutoMap();
				cm.MapIdField(client => client.ConnectionUid);
			});
			MongoDbConnection = mongoDbConnection;
			MongoDbDatabase = mongoDbDatabase;
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
