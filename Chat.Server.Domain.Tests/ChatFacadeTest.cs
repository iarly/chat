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
			ChatFacade = new ChatFacade(ClientFactoryMock.Object, ClientRepositoryMock.Object);
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
			ClientRepositoryMock.Verify(mock => mock.StoreAsync(client), Times.Once);
		}

		[Test]
		public async Task Should_Update_The_Client_Information_With_They_Nickname_When_UpdateNickname()
		{
			// arrage
			Guid theConnectionUid = Guid.NewGuid();
			string theNickname = "Will Smith";
			Client storedClient = new Client
			{
				Nickname = "Agent J"
			};

			ClientRepositoryMock.Setup(mock => mock.GetByUidAsync(theConnectionUid)).Returns(Task.FromResult(storedClient));

			// act
			await ChatFacade.UpdateNicknameAsync(theConnectionUid, theNickname);

			// assert
			Assert.AreEqual(theNickname, storedClient.Nickname);
			ClientRepositoryMock.Verify(mock => mock.UpdateAsync(storedClient), Times.Once);
		}

		[Test]
		public async Task Should_Connect_User_To_The_General_Room_When_UpdateNickname_At_The_First_Time()
		{
			// arrage
			string expectedRoom = "general";
			Guid theConnectionUid = Guid.NewGuid();
			string theNickname = "Carlton";
			Client storedClient = new Client
			{
				Nickname = null
			};

			Guid? theConnectionUidOfConnectedClient = null;
			Client theConnectedClient = null;

			ClientRepositoryMock.Setup(mock => mock.GetByUidAsync(theConnectionUid)).Returns(Task.FromResult(storedClient));

			ChatFacade.OnUserConnectsAtRoom += (connectionUid, client) =>
			{
				theConnectionUidOfConnectedClient = connectionUid;
				theConnectedClient = client;
			};

			// act
			await ChatFacade.UpdateNicknameAsync(theConnectionUid, theNickname);

			// assert
			Assert.AreEqual(expectedRoom, storedClient.Room);
			Assert.AreEqual(theConnectionUid, theConnectionUidOfConnectedClient);
			Assert.AreEqual(storedClient, theConnectedClient);
		}
	}
}