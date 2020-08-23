using Chat.Serializers;
using Chat.Server.Communicator.Sockets.Tests.Stubs;
using Chat.Server.Domain.Commands;
using Chat.Server.Domain.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.Server.Communicator.Sockets.Tests
{
	public class SocketCommunicatorTest
	{
		private TimeSpan ReceiveMessageTimeout = TimeSpan.FromSeconds(5);
		private TimeSpan TestTimeout = TimeSpan.FromSeconds(15);

		private Mock<IConfiguration> ConfigurationMock;
		private CommandSerializer CommandSerializer;
		private SocketCommunicator SocketCommunicator;

		[SetUp]
		public void Setup()
		{
			ConfigurationMock = new Mock<IConfiguration>();
			CommandSerializer = new CommandSerializer();
			SocketCommunicator = new SocketCommunicator(ConfigurationMock.Object, CommandSerializer);
		}

		[Test]
		public async Task Should_Send_Command_To_Client()
		{
			// arrange
			PropagateMessageCommand expectedCommand = new PropagateMessageCommand
			{
				Content = new TextualContent("Welcome to my world!")
			};

			PropagateMessageCommand actualCommand = null;
			ManualResetEvent connectionWaiter = new ManualResetEvent(false);
			CancellationTokenSource cancellationToken = new CancellationTokenSource(TestTimeout);

			TextualContent expectedContent = expectedCommand.Content as TextualContent;
			TextualContent actualContent = null;

			SocketCommunicator.OnClientConnected += (connectionUid) =>
			{
				connectionWaiter.Set();
				return Task.CompletedTask;
			};

			// act when server is ready
			WhenSocketIsReady(cancellationToken, () =>
			{
				ClientConnection client = new ClientConnection("localhost", 33000);

				connectionWaiter.WaitOne(ReceiveMessageTimeout);

				SocketCommunicator.PublishAsync(expectedCommand);

				var receivedText = client.Receive();

				actualCommand = CommandSerializer.Deserialize(receivedText) as PropagateMessageCommand;
				actualContent = actualCommand.Content as TextualContent;
			});

			await SocketCommunicator.ListenAsync(cancellationToken.Token);

			// assert
			Assert.AreEqual(expectedContent.Text, actualContent.Text);
		}

		[Test]
		public async Task Should_Receive_Command_From_Client()
		{
			// arrange
			SendMessageCommand expectedCommand = new SendMessageCommand
			{
				Content = new TextualContent("Hello my world!")
			};

			SendMessageCommand actualCommand = null;
			ManualResetEvent responseWaiter = new ManualResetEvent(false);
			CancellationTokenSource cancellationToken = new CancellationTokenSource(TestTimeout);

			TextualContent expectedContent = expectedCommand.Content as TextualContent;
			TextualContent actualContent = null;

			SocketCommunicator.OnClientSendCommand += (connectionUid, command) =>
			{
				actualCommand = command as SendMessageCommand;
				actualContent = actualCommand.Content as TextualContent;
				responseWaiter.Set();

				return Task.CompletedTask;
			};

			// act when server is ready
			WhenSocketIsReady(cancellationToken, () =>
			{
				ClientConnection client = new ClientConnection("localhost", 33000);

				client.Send(CommandSerializer.Serializer(expectedCommand));

				responseWaiter.WaitOne(ReceiveMessageTimeout);
			});

			await SocketCommunicator.ListenAsync(cancellationToken.Token);

			// assert
			Assert.AreEqual(expectedContent.Text, actualContent.Text);
		}

		[Test]
		public async Task Should_Receive_Two_Commands_From_Client()
		{
			// arrange
			AutoResetEvent responseWaiter = new AutoResetEvent(false);
			CancellationTokenSource cancellationToken = new CancellationTokenSource(TestTimeout);

			TextualContent expectedContent1 = new TextualContent("Hello my world!");
			TextualContent expectedContent2 = new TextualContent("My second message for you");

			TextualContent actualContent1 = null;
			TextualContent actualContent2 = null;

			SocketCommunicator.OnClientSendCommand += (connectionUid, command) =>
			{
				var actualCommand = command as SendMessageCommand;

				if (actualContent1 == null)
				{
					actualContent1 = actualCommand.Content as TextualContent;
				}
				else
				{
					actualContent2 = actualCommand.Content as TextualContent;
					responseWaiter.Set();
				}

				return Task.CompletedTask;
			};

			// act when server is ready
			WhenSocketIsReady(cancellationToken, () =>
			{
				ClientConnection client = new ClientConnection("localhost", 33000);

				client.Send(CommandSerializer.Serializer(new SendMessageCommand { Content = expectedContent1 }));
				client.Send(CommandSerializer.Serializer(new SendMessageCommand { Content = expectedContent2 }));

				responseWaiter.WaitOne(ReceiveMessageTimeout);
			});

			await SocketCommunicator.ListenAsync(cancellationToken.Token);

			// assert
			Assert.AreEqual(expectedContent1.Text, actualContent1.Text);
			Assert.AreEqual(expectedContent2.Text, actualContent2.Text);
		}

		private void WhenSocketIsReady(CancellationTokenSource cancellationToken, Action action)
		{
			SocketCommunicator.OnServerReady += () =>
			{
				Task.Run(() =>
				{
					action();

					cancellationToken.Cancel();
				});
			};
		}

	}
}
