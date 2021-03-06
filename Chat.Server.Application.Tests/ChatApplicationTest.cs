using Chat.Server.Application.Contracts;
using Chat.Server.Application.Mappers;
using Chat.Server.Communicator;
using Chat.Server.Domain;
using Chat.Server.Domain.Commands;
using Chat.Server.Domain.Entities;
using Chat.Server.Domain.Enumerators;
using Chat.Server.MessageBroker.Delegates;
using Moq;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.Server.Application.Tests
{
	public class ChatApplicationTest
	{
		ChatApplication ChatApplication;
		Mock<ITextualCommandMapper> TextuaCommandMapperMock;
		Mock<IChatFacade> ChatFacadeMock;
		Mock<IMessageBroker> MessageBrokerMock;
		Mock<ICommunicator<string>> CommunicatorMock;
		Mock<IDomainEvents> DomainEventsMock;

		[SetUp]
		public void Setup()
		{
			TextuaCommandMapperMock = new Mock<ITextualCommandMapper>();
			ChatFacadeMock = new Mock<IChatFacade>();
			MessageBrokerMock = new Mock<IMessageBroker>();
			CommunicatorMock = new Mock<ICommunicator<string>>();
			DomainEventsMock = new Mock<IDomainEvents>();

			ChatApplication = new ChatApplication(ChatFacadeMock.Object,
				TextuaCommandMapperMock.Object,
				MessageBrokerMock.Object,
				CommunicatorMock.Object,
				DomainEventsMock.Object);
		}

		[Test]
		public async Task Should_Listen_Communicator_When_Start_Application()
		{
			await ChatApplication.StartAsync();

			CommunicatorMock.Verify(mock => mock.ListenAsync(It.IsAny<CancellationToken>()));
		}

		[Test]
		public void Should_ProcessCommand_And_Subcribe_To_MassageBroker_When_Client_Connects()
		{
			Guid expectedConnectionUid = Guid.NewGuid();

			CommunicatorMock.Raise(mock => mock.OnClientConnected += null, expectedConnectionUid);

			ChatFacadeMock.Verify(mock => mock.ProcessMessageAsync(It.IsAny<ConnectCommand>()));

			MessageBrokerMock.Verify(mock => mock.SubscribeAsync(expectedConnectionUid, It.IsAny<BrokerSendCommandDelegate>()));
		}

		[Test]
		public void Should_Process_The_Command_When_Client_Sends_Command()
		{
			Guid expectedConnectionUid = Guid.NewGuid();
			string expectedTextualCommand = "/t Jude Hey!";
			Command expectedCommand = new SendMessageCommand();
			Client client = new Client() { Nickname = "IAmReadyToConversation" };

			ChatFacadeMock.Setup(mock => mock.GetClientByUidAsync(expectedConnectionUid)).Returns(Task.FromResult(client));

			TextuaCommandMapperMock.Setup(mock => mock.ToCommand(expectedConnectionUid, ClientState.ReadyToConversation, expectedTextualCommand)).Returns(expectedCommand);

			CommunicatorMock.Raise(mock => mock.OnClientSendCommand += null, expectedConnectionUid, expectedTextualCommand);

			ChatFacadeMock.Verify(mock => mock.ProcessMessageAsync(expectedCommand));
		}

		[Test]
		public void Should_ProcessCommand_When_Client_Disconnects()
		{
			Guid expectedConnectionUid = Guid.NewGuid();

			CommunicatorMock.Raise(mock => mock.OnClientDisconnected += null, expectedConnectionUid);

			ChatFacadeMock.Verify(mock => mock.ProcessMessageAsync(It.IsAny<DisconnectCommand>()));
		}

		[Test]
		public void Should_Forward_Message_To_MessageBroken_When_A_Message_Is_Received_From_Domain()
		{
			Guid expectedConnectionUid = Guid.NewGuid();

			Client expectedDestination = new Client();
			Command expectedCommand = new SendMessageCommand();

			DomainEventsMock.Raise(mock => mock.OnCommand += null, expectedDestination, expectedCommand);

			MessageBrokerMock.Verify(mock => mock.PublishAsync(expectedCommand));
		}
	}
}