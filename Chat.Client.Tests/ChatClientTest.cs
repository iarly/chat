using Chat.Client.Tests.Stubs;
using Microsoft.Extensions.Configuration;
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

		[SetUp]
		public void Setup()
		{
			ConfigurationMock = new Mock<IConfiguration>();
			ChatClient = new ChatClient(ConfigurationMock.Object);
			Server = new ServerCommunicator("localhost", 22000);
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

			ChatClient.ConnectTo("localhost", 22000);

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

			ChatClient.ConnectTo("localhost", 22000);

			manualResetEvent.WaitOne(4000);

			ChatClient.SendMessage(expectedMessage);

			string actualMessage = Server.Receive();

			Assert.AreEqual(expectedMessage + ChatClient.EOF, actualMessage);
		}
	}
}