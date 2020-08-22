using Chat.Server.Domain.Entities;
using Chat.Server.Domain.Exceptions;
using Chat.Server.Domain.Factories;
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

		[Test]
		public async Task Should_Not_Update_Room_When_The_User_Is_Already_At_Any_Room()
		{
			// arrage
			string expectedRoom = "secret-room";
			Guid theConnectionUid = Guid.NewGuid();
			string theNickname = "Carlton";
			Client storedClient = new Client
			{
				Nickname = null,
				Room = expectedRoom
			};

			ClientRepositoryMock.Setup(mock => mock.GetByUidAsync(theConnectionUid)).Returns(Task.FromResult(storedClient));

			// act
			await ChatFacade.UpdateNicknameAsync(theConnectionUid, theNickname);

			// assert
			Assert.AreEqual(expectedRoom, storedClient.Room);
		}

		[Test]
		public void Should_Return_Error_When_User_Has_Not_Set_Room()
		{
			// arrage
			Guid theConnectionUid = Guid.NewGuid();
			IMessageContent theMessageContent = Mock.Of<IMessageContent>();
			Client storedClient = new Client
			{
				Nickname = "me",
				Room = ""
			};

			ClientRepositoryMock.Setup(mock => mock.GetByUidAsync(theConnectionUid)).Returns(Task.FromResult(storedClient));

			// act & assert
			Assert.ThrowsAsync<UserHasNotSetTheRoomException>(async () => await ChatFacade.SendPublicMessageAsync(theConnectionUid, theMessageContent));
		}

		[Test]
		public void Should_Return_Error_When_User_Has_Not_Set_Nickname()
		{
			// arrage
			Guid theConnectionUid = Guid.NewGuid();
			IMessageContent theMessageContent = Mock.Of<IMessageContent>();
			Client storedClient = new Client
			{
				Room = ""
			};

			ClientRepositoryMock.Setup(mock => mock.GetByUidAsync(theConnectionUid)).Returns(Task.FromResult(storedClient));

			// act & assert
			Assert.ThrowsAsync<UserHasNotsetTheNicknameException>(async () => await ChatFacade.SendPublicMessageAsync(theConnectionUid, theMessageContent));
		}

		[Test]
		public async Task Should_Broadcast_Message_When_User_Sents_A_Public_Message()
		{
			// arrage
			string expectedRoom = "secret-room";
			Guid theConnectionUid = Guid.NewGuid();
			IMessageContent theMessageContent = Mock.Of<IMessageContent>();

			Client storedClient = new Client
			{
				Nickname = "Will",
				Room = expectedRoom
			};

			ClientRepositoryMock.Setup(mock => mock.GetByUidAsync(theConnectionUid)).Returns(Task.FromResult(storedClient));

			string theMessageDestinationRoom = null;
			Message theSentMessage = null;

			ChatFacade.OnUserSentMessage += (room, message) =>
			{
				theMessageDestinationRoom = room;
				theSentMessage = message;
			};

			// act
			await ChatFacade.SendPublicMessageAsync(theConnectionUid, theMessageContent);

			// assert
			Assert.AreEqual(expectedRoom, theMessageDestinationRoom);
			Assert.AreEqual(theMessageContent, theSentMessage.Content);
			Assert.AreEqual(storedClient, theSentMessage.Sender);
		}

		[Test]
		public async Task Should_Broadcast_A_Targeted_Message_When_User_Sents_A_Targeted_Public_Message()
		{
			// arrage
			string expectedRoom = "secret-room";

			string theTargetedUser = "Carlton";
			Guid theConnectionUid = Guid.NewGuid();
			IMessageContent theMessageContent = Mock.Of<IMessageContent>();

			Client senderClient = new Client
			{
				Nickname = "Will",
				Room = expectedRoom
			};

			Client targetedClient = new Client
			{
				Nickname = "Carlton",
				Room = expectedRoom
			};

			ClientRepositoryMock.Setup(mock => mock.FindByNicknameAsync(theTargetedUser)).Returns(Task.FromResult(targetedClient));

			ClientRepositoryMock.Setup(mock => mock.GetByUidAsync(theConnectionUid)).Returns(Task.FromResult(senderClient));

			string theMessageDestinationRoom = null;
			TargetedMessage theSentMessage = null;

			ChatFacade.OnUserSentMessage += (room, message) =>
			{
				theMessageDestinationRoom = room;
				theSentMessage = message as TargetedMessage;
			};

			// act
			await ChatFacade.SendPublicTargetedMessageAsync(theConnectionUid, theTargetedUser, theMessageContent);

			// assert
			Assert.AreEqual(expectedRoom, theMessageDestinationRoom);
			Assert.NotNull(theSentMessage);

			Assert.AreEqual(theMessageContent, theSentMessage.Content);
			Assert.AreEqual(senderClient, theSentMessage.Sender);
			Assert.AreEqual(targetedClient, theSentMessage.Target);
		}
	}
}