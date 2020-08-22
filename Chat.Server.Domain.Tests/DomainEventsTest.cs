using Chat.Server.Domain.Commands;
using Chat.Server.Domain.Entities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
		public void Should_Invoke_OnCommand()
		{
			Client expectedTarget = new Client();
			Command expectedCommand = new SendMessageCommand();

			Client invokedTarget = null;
			Command invokedCommand = null;

			DomainEvents.OnCommand += (target, command) =>
			{
				invokedTarget = target;
				invokedCommand = command;
				return Task.CompletedTask;
			};

			DomainEvents.SendCommand(expectedTarget, expectedCommand);

			Assert.AreEqual(expectedTarget, invokedTarget);
			Assert.AreEqual(expectedCommand, invokedCommand);
		}
	}
}
