using Chat.Server.Domain.Entities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chat.Server.Domain.Tests
{
	public class DomainEventsTest
	{
		DomainEvents DomainEvents;

		[SetUp]
		public void Setup()
		{
			DomainEvents = new DomainEvents();
		}

		[Test]
		public void Should_Invoke_OnRequestNickname()
		{
			Guid expectedConnectionUid = Guid.NewGuid();
			Guid invokedConnectionUid = Guid.Empty;

			DomainEvents.OnRequestNickname += connectionUid => invokedConnectionUid = connectionUid;

			DomainEvents.InvokeRequestNicknameEvent(expectedConnectionUid);

			Assert.AreEqual(expectedConnectionUid, invokedConnectionUid);
		}

		[Test]
		public void Should_Invoke_OnUserConnectsAtRoom()
		{
			Client expectedClient = new Client();
			Guid expectedConnectionUid = Guid.NewGuid();

			Client invokedClient = null;
			Guid invokedConnectionUid = Guid.Empty;

			DomainEvents.OnUserConnectsAtRoom += (connectionUid, client) =>
			{
				invokedConnectionUid = connectionUid;
				invokedClient = client;
			};

			DomainEvents.InvokeOnUserConnectsAtRoomEvent(expectedConnectionUid, expectedClient);

			Assert.AreEqual(expectedConnectionUid, invokedConnectionUid);
			Assert.AreEqual(expectedClient, invokedClient);
		}

		[Test]
		public void Should_Invoke_OnUserSentMessage()
		{
			string expectedRoomsName = "My Room";
			Message expectedMessage = new Message(null, null);

			string invokedRoomsName = null;
			Message invokedMessage = null;

			DomainEvents.OnUserSentMessage += (roomsName, message) =>
			{
				invokedRoomsName = roomsName;
				invokedMessage = message;
			};

			DomainEvents.InvokeOnUserSentMessage(expectedRoomsName, expectedMessage);

			Assert.AreEqual(expectedRoomsName, invokedRoomsName);
			Assert.AreEqual(expectedMessage, invokedMessage);
		}

		[Test]
		public void Should_Invoke_OnUserSentPrivateMessage()
		{
			Client expectedTarget = new Client();
			TargetedMessage expectedMessage = new TargetedMessage(null, null, null);

			Client invokedTarget = null;
			Message invokedMessage = null;

			DomainEvents.OnUserSentPrivateMessage += (target, message) =>
			{
				invokedTarget = target;
				invokedMessage = message;
			};

			DomainEvents.InvokeOnUserSentPrivateMessageEvent(expectedTarget, expectedMessage);

			Assert.AreEqual(expectedTarget, invokedTarget);
			Assert.AreEqual(expectedMessage, invokedMessage);
		}
	}
}
