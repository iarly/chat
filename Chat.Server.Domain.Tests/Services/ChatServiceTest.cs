using Chat.Server.Domain.Commands;
using Chat.Server.Domain.Entities;
using Chat.Server.Domain.Exceptions;
using Chat.Server.Domain.Factories;
using Chat.Server.Domain.Repositories;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.Server.Domain.Tests
{
	public class ChatServiceTest
	{
		DomainEvents DomainEvents;

		Mock<IClientFactory> ClientFactoryMock;

		Mock<IClientRepository> ClientRepositoryMock;

		ChatService ChatFacade;

		[SetUp]
		public void Setup()
		{
			DomainEvents = new DomainEvents();
			ClientFactoryMock = new Mock<IClientFactory>();
			ClientRepositoryMock = new Mock<IClientRepository>();
			ChatFacade = new ChatService(DomainEvents, ClientFactoryMock.Object, ClientRepositoryMock.Object);
		}

		[Test]
		public async Task Should_Invoke_Disconnect_Domain_Event_On_Disconnect_User()
		{
			// arrange
			Guid expectedConnectionUid = Guid.NewGuid();
			Client expectedClient = new Client() { ConnectionUid = expectedConnectionUid };

			Client actualClient = null;
			DisconnectCommand actualCommand = null;

			ClientRepositoryMock.Setup(mock => mock.GetByUidAsync(expectedConnectionUid)).Returns(Task.FromResult(expectedClient));

			DomainEvents.OnDisconnectCommand += (client, command) =>
			{
				actualClient = client;
				actualCommand = command as DisconnectCommand;
				return Task.CompletedTask;
			};

			// act
			await ChatFacade.DisconnectAsync(expectedConnectionUid);

			// assert
			Assert.IsNotNull(actualCommand);
			Assert.AreEqual(expectedClient, actualClient);
		}

		[Test]
		public void Should_Throw_Error_When_Nickname_Is_Invalid()
		{
			// arrage
			Guid theConnectionUid = Guid.NewGuid();
			string theNickname = "Will Smith";
			Client storedClient = new Client
			{
				ConnectionUid = theConnectionUid,
				Nickname = "Agent J"
			};

			ClientRepositoryMock.Setup(mock => mock.GetByUidAsync(theConnectionUid)).Returns(Task.FromResult(storedClient));

			// act && assert
			Assert.ThrowsAsync<InvalidNicknameException>(async () => await ChatFacade.UpdateNicknameAsync(theConnectionUid, theNickname));
		}

		[Test]
		public void Should_Throw_Error_When_Nickname_Already_Exists()
		{
			// arrage
			Guid theConnectionUid = Guid.NewGuid();
			string theNickname = "Will-Smith";
			Client storedClient = new Client
			{
				ConnectionUid = theConnectionUid,
				Nickname = "Agent J"
			};

			Client existentClient = new Client()
			{
				ConnectionUid = Guid.NewGuid(),
				Nickname = "Will Smith"
			};

			ClientRepositoryMock.Setup(mock => mock.FindByNicknameAsync(theNickname)).Returns(Task.FromResult(existentClient));

			ClientRepositoryMock.Setup(mock => mock.GetByUidAsync(theConnectionUid)).Returns(Task.FromResult(storedClient));

			// act && assert
			Assert.ThrowsAsync<NicknameAlreadyExistsException>(async () => await ChatFacade.UpdateNicknameAsync(theConnectionUid, theNickname));
		}

		[Test]
		public async Task Should_Request_Nickname_When_Connect()
		{
			// arrage
			Guid expectedConnectionUid = Guid.NewGuid();
			Client expectedClient = new Client() { ConnectionUid = expectedConnectionUid };

			Client actualClient = null;
			SetNicknameCommand actualCommand = null;

			DomainEvents.OnCommand += (connectionUid, command) =>
			{
				actualClient = connectionUid;
				actualCommand = command as SetNicknameCommand;
				return Task.CompletedTask;
			};

			ClientFactoryMock.Setup(mock => mock.Create(expectedConnectionUid)).Returns(expectedClient);

			// act
			await ChatFacade.ConnectAsync(expectedConnectionUid);

			// assert
			Assert.AreEqual(expectedConnectionUid, actualClient.ConnectionUid);
			Assert.IsNotNull(actualCommand);
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
		public async Task Should_Update_Clients_Room_When_Updating_The_Current_User_Room()
		{
			// arrage
			Guid connectionUid = Guid.NewGuid();
			string room = "OtherRoom";
			Client storedClient = new Client
			{
				Room = ""
			};

			ClientRepositoryMock.Setup(mock => mock.GetByUidAsync(connectionUid)).Returns(Task.FromResult(storedClient));

			// act
			await ChatFacade.UpdateRoomAsync(connectionUid, room);

			// assert
			Assert.AreEqual(room, storedClient.Room);
			ClientRepositoryMock.Verify(mock => mock.UpdateAsync(storedClient), Times.Once);
		}

		[Test]
		public async Task Should_Notice_Everyone_About_The_Entrant_User_When_Updating_The_Current_User_Room()
		{
			// arrage
			Guid connectionUid = Guid.NewGuid();
			string room = "OtherRoom";
			Client storedClient = new Client
			{
				Room = ""
			};

			NoticeCommand joinNotice = null;

			DomainEvents.OnCommand += (destination, command) =>
			{
				joinNotice = command as NoticeCommand;
				return Task.CompletedTask;
			};

			ClientRepositoryMock.Setup(mock => mock.GetAllClientInTheRoomAsync(room)).Returns(Task.FromResult(new List<Client> { storedClient }.AsEnumerable()));
			ClientRepositoryMock.Setup(mock => mock.GetByUidAsync(connectionUid)).Returns(Task.FromResult(storedClient));

			// act
			await ChatFacade.UpdateRoomAsync(connectionUid, room);

			// assert
			Assert.IsNotNull(joinNotice);
		}

		[Test]
		public async Task Should_Notice_Everyone_About_The_Users_Exit_When_Updating_The_Current_User_Room()
		{
			// arrage
			Guid connectionUid = Guid.NewGuid();
			string theRoomTheUserEntered = "the-new-room";
			string theRoomTheUserHasLeft = "forgotten-room";
			Client storedClient = new Client
			{
				Room = theRoomTheUserHasLeft
			};

			NoticeCommand leftNotice = null;

			DomainEvents.OnCommand += (destination, command) =>
			{
				leftNotice = command as NoticeCommand;
				return Task.CompletedTask;
			};

			ClientRepositoryMock.Setup(mock => mock.GetAllClientInTheRoomAsync(theRoomTheUserHasLeft)).Returns(Task.FromResult(new List<Client> { storedClient }.AsEnumerable()));
			ClientRepositoryMock.Setup(mock => mock.GetByUidAsync(connectionUid)).Returns(Task.FromResult(storedClient));

			// act
			await ChatFacade.UpdateRoomAsync(connectionUid, theRoomTheUserEntered);

			// assert
			Assert.IsNotNull(leftNotice);
		}

		[Test]
		public async Task Should_Update_The_Client_Information_With_They_Nickname_When_UpdateNickname()
		{
			// arrage
			Guid theConnectionUid = Guid.NewGuid();
			string theNickname = "Will-Smith";
			Client storedClient = new Client
			{
				Nickname = "Agent J"
			};

			ClientRepositoryMock.Setup(mock => mock.GetByUidAsync(theConnectionUid)).Returns(Task.FromResult(storedClient));

			// act
			await ChatFacade.UpdateNicknameAsync(theConnectionUid, theNickname);

			// assert
			Assert.AreEqual(theNickname, storedClient.Nickname);
			ClientRepositoryMock.Verify(mock => mock.UpdateAsync(storedClient));
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

			ClientRepositoryMock.Setup(mock => mock.GetByUidAsync(theConnectionUid)).Returns(Task.FromResult(storedClient));

			// act
			await ChatFacade.UpdateNicknameAsync(theConnectionUid, theNickname);

			// assert
			Assert.AreEqual(expectedRoom, storedClient.Room);
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

			Client senderClient = new Client
			{
				Nickname = "Will",
				Room = expectedRoom
			};

			Client destinationClient = new Client();

			ClientRepositoryMock.Setup(mock => mock.GetAllClientInTheRoomAsync(expectedRoom)).Returns(Task.FromResult(new List<Client> { destinationClient }.AsEnumerable()));
			ClientRepositoryMock.Setup(mock => mock.GetByUidAsync(theConnectionUid)).Returns(Task.FromResult(senderClient));

			Client actualDestinationClient = null;
			PropagateMessageCommand actualCommand = null;

			DomainEvents.OnCommand += (destination, command) =>
			{
				actualDestinationClient = destination;
				actualCommand = command as PropagateMessageCommand;
				return Task.CompletedTask;
			};

			// act
			await ChatFacade.SendPublicMessageAsync(theConnectionUid, theMessageContent);

			// assert
			Assert.AreEqual(destinationClient, actualDestinationClient);
			Assert.AreEqual(theMessageContent, actualCommand.Content);
			Assert.AreEqual(senderClient, actualCommand.Sender);
		}

		[Test]
		public async Task Should_Broadcast_A_Targeted_Message_When_User_Sents_A_Targeted_Public_Message()
		{
			// arrage
			string expectedRoom = "secret-room";

			string theTargetedNickname = "Carlton";
			Guid theConnectionUid = Guid.NewGuid();
			IMessageContent theMessageContent = Mock.Of<IMessageContent>();

			Client senderClient = new Client
			{
				Nickname = "Will",
				Room = expectedRoom
			};

			Client targetedClient = new Client
			{
				Nickname = theTargetedNickname,
				Room = expectedRoom
			};

			Client destinationClient = new Client();

			ClientRepositoryMock.Setup(mock => mock.GetAllClientInTheRoomAsync(expectedRoom)).Returns(Task.FromResult(new List<Client> { destinationClient }.AsEnumerable()));

			ClientRepositoryMock.Setup(mock => mock.FindByNicknameAsync(theTargetedNickname)).Returns(Task.FromResult(targetedClient));

			ClientRepositoryMock.Setup(mock => mock.GetByUidAsync(theConnectionUid)).Returns(Task.FromResult(senderClient));

			Client actualDestinationClient = null;
			PropagateMessageCommand actualCommand = null;

			DomainEvents.OnCommand += (destination, command) =>
			{
				actualDestinationClient = destination;
				actualCommand = command as PropagateMessageCommand;
				return Task.CompletedTask;
			};

			// act
			await ChatFacade.SendPublicTargetedMessageAsync(theConnectionUid, theTargetedNickname, theMessageContent);

			// assert
			Assert.AreEqual(destinationClient, actualDestinationClient);
			Assert.NotNull(actualCommand);

			Assert.AreEqual(theMessageContent, actualCommand.Content);
			Assert.AreEqual(senderClient, actualCommand.Sender);
			Assert.AreEqual(targetedClient, actualCommand.Target);
		}

		[Test]
		public void Should_Not_Broadcast_A_Targeted_Message_When_The_Targeted_Client_Does_Not_Exists()
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

			Client targetedClient = null;

			ClientRepositoryMock.Setup(mock => mock.FindByNicknameAsync(theTargetedUser)).Returns(Task.FromResult(targetedClient));

			ClientRepositoryMock.Setup(mock => mock.GetByUidAsync(theConnectionUid)).Returns(Task.FromResult(senderClient));

			// act
			Assert.ThrowsAsync<TargetClientDoesNotExistsException>(async () => await ChatFacade.SendPublicTargetedMessageAsync(theConnectionUid, theTargetedUser, theMessageContent));
		}

		[Test]
		public async Task Should_Broadcast_A_Direct_Message_When_User_Sents_A_Private_Message()
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

			Client actualDestinationClient = null;
			PropagateMessageCommand actualCommand = null;

			DomainEvents.OnCommand += (destination, command) =>
			{
				actualDestinationClient = destination;
				actualCommand = command as PropagateMessageCommand;
				return Task.CompletedTask;
			};

			// act
			await ChatFacade.SendPrivateMessageAsync(theConnectionUid, theTargetedUser, theMessageContent);

			// assert
			Assert.AreEqual(targetedClient, actualDestinationClient);
			Assert.NotNull(actualCommand);

			Assert.AreEqual(theMessageContent, actualCommand.Content);
			Assert.AreEqual(senderClient, actualCommand.Sender);
			Assert.AreEqual(targetedClient, actualCommand.Target);
		}

		[Test]
		public void Should_Not_Broadcast_Direct_Message_When_The_Targeted_Client_Does_Not_Exists()
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

			Client targetedClient = null;

			ClientRepositoryMock.Setup(mock => mock.FindByNicknameAsync(theTargetedUser)).Returns(Task.FromResult(targetedClient));

			ClientRepositoryMock.Setup(mock => mock.GetByUidAsync(theConnectionUid)).Returns(Task.FromResult(senderClient));

			// act
			Assert.ThrowsAsync<TargetClientDoesNotExistsException>(async () => await ChatFacade.SendPublicTargetedMessageAsync(theConnectionUid, theTargetedUser, theMessageContent));
		}

	}
}