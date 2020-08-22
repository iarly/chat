using Chat.Server.Domain.Factories;
using NUnit.Framework;
using System;

namespace Chat.Server.Domain.Tests.Factories
{
	public class ClientFactoryTest
	{
		IClientFactory ClientFactory;

		[SetUp]
		public void Setup()
		{
			ClientFactory = new ClientFactory();
		}

		[Test]
		public void Should_Create_Client()
		{
			var connectionUid = Guid.NewGuid();

			var client = ClientFactory.Create(connectionUid);

			Assert.AreEqual(client.ConnectionUid, connectionUid);
		}
	}
}
