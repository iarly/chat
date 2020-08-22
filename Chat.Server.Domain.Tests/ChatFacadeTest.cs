using Chat.Server.Domain.Entities;
using Chat.Server.Domain.Factories;
using Chat.Server.Domain.Messages;
using Chat.Server.Domain.Repositories;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Chat.Server.Domain.Tests
{
	public class ChatFacadeTest
	{
		Mock<IClientFactory> ClientFactoryMock;

		Mock<IClientRepository> ClientRepositoryMock;

		ChatFacade ChatFacade;

		[SetUp]
		public void Setup()
		{
			ClientFactoryMock = new Mock<IClientFactory>();
			ClientRepositoryMock = new Mock<IClientRepository>();
			ChatFacade = new ChatFacade(ClientRepositoryMock.Object);
		}

		[Test]
		public async Task Should_Request_Nickname_When_Connect()
		{
			// arrage
			Guid theConnectionUidOfConnectedClient = Guid.NewGuid();
			Guid? theConnectionUidFromEvent = null;

			ChatFacade.OnRequestNickname += (connectionUid) =>
			{
				theConnectionUidFromEvent = connectionUid;
			};

			// act
			await ChatFacade.ConnectAsync(theConnectionUidOfConnectedClient);

			// assert
			Assert.AreEqual(theConnectionUidOfConnectedClient, theConnectionUidFromEvent);
		}

		[Test]
		public async Task Should_Store_The_Client_Information_When_Connect()
		{
			// arrage
			Guid theConnectionUidOfConnectedClient = Guid.NewGuid();
			Client client = new Client();

			ClientFactoryMock.Setup(mock => mock.Create(theConnectionUidOfConnectedClient)).Returns(client);

			// act
			await ChatFacade.ConnectAsync(theConnectionUidOfConnectedClient);

			// assert
			ClientRepositoryMock.Verify(mock => mock.Store(client), Times.Once);
		}
	}
}