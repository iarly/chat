using Chat.Client.Tests.Stubs;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Moq;
using NUnit.Framework;
using System.Threading;

namespace Chat.Client.Tests
{
	public class ChatClientTest
	{
		private Mock<IConfiguration> ConfigurationMock;
		private ChatClient ChatClient;

		public ServerCommunicator Server { get; private set; }
		public CancellationTokenSource CancellationTokenSource { get; private set; }

		[SetUp]
		public void Setup()
		{
			ConfigurationMock = new Mock<IConfiguration>();
			ChatClient = new ChatClient(ConfigurationMock.Object);
			Server = new ServerCommunicator("localhost", 22000);
			CancellationTokenSource = new CancellationTokenSource();
		}

		[TearDown]
		public void Clean()
		{
			CancellationTokenSource.Cancel();
			Server.Dispose();
		}

		public void Should_Notify_When_Disconnect_From_Server()
		{
			ManualResetEvent connectEvent = new ManualResetEvent(false);
			ManualResetEvent disconnectEvent = new ManualResetEvent(false);

			bool connected = false;

			Server.OnClientConnected += () =>
			{
				connected = true;
				connectEvent.Set();
			};

			ChatClient.OnDisconnected += () =>
			{
				connected = false;
				disconnectEvent.Set();
			};

			ChatClient.ConnectTo("localhost", 22000, CancellationTokenSource.Token);

			connectEvent.WaitOne(4000);

			Server.Dispose();

			disconnectEvent.WaitOne(4000);

			Assert.IsFalse(connected);
		}

		[Test]
		public void Should_Connect_To_Server()
		{
			ManualResetEvent manualResetEvent = new ManualResetEvent(false);
			bool connected = false;

			Server.OnClientConnected += () =>
			{
				connected = true;
				manualResetEvent.Set();
			};

			ChatClient.ConnectTo("localhost", 22000, CancellationTokenSource.Token);

			manualResetEvent.WaitOne(4000);

			Assert.IsTrue(connected);
		}

		[Test]
		public void Should_Send_Text_To_The_Server()
		{
			ManualResetEvent manualResetEvent = new ManualResetEvent(false);
			string expectedMessage = "Hi Server!";

			Server.OnClientConnected += () =>
			{
				manualResetEvent.Set();
			};

			ChatClient.ConnectTo("localhost", 22000, CancellationTokenSource.Token);

			manualResetEvent.WaitOne(4000);

			ChatClient.SendMessage(expectedMessage);

			string actualMessage = Server.Receive();

			Assert.AreEqual(expectedMessage + ChatClient.EOF, actualMessage);
		}

		[Test]
		public void Shoud_Receive_Text_From_Server()
		{
			ManualResetEvent connectionDone = new ManualResetEvent(false);
			ManualResetEvent messageReceived = new ManualResetEvent(false);

			string expectedMessage = "Hi Server!";
			string actualMessage = null;

			Server.OnClientConnected += () =>
			{
				connectionDone.Set();
			};

			ChatClient.OnReceiveMessage += (text) =>
			{
				actualMessage = text;
				messageReceived.Set();
			};

			ChatClient.ConnectTo("localhost", 22000, CancellationTokenSource.Token);

			connectionDone.WaitOne(4000);

			Server.Send(expectedMessage);

			messageReceived.WaitOne(4000);

			Assert.AreEqual(expectedMessage, actualMessage);
		}
	}
}