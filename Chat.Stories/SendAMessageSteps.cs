using Chat.Client;
using Chat.Server.Application;
using Chat.Server.IoC;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Chat.Stories
{
	[Binding]
	public class SendAMessageSteps
	{
		IConfiguration Configuration;
		ScenarioContext ScenarioContext;
		ChatApplication ChatApplication;
		Dictionary<string, ChatClient> Clients;
		Dictionary<string, List<string>> LastMessages;
		CancellationTokenSource CancellationTokenSource;
		string MyChatRoom;
		private string MySentMessage;

		public SendAMessageSteps(ScenarioContext scenarioContext)
		{
			ScenarioContext = scenarioContext;
		}

		[BeforeScenario]
		public void Setup()
		{
			Configuration = Mock.Of<IConfiguration>();
			CancellationTokenSource = new CancellationTokenSource();
			Clients = new Dictionary<string, ChatClient>();
			LastMessages = new Dictionary<string, List<string>>();
			InjectionConfig.ConfigureServices(Configuration);
			ChatApplication = InjectionConfig.Get<ChatApplication>();

			Task.Run(async () =>
			{
				await ChatApplication.StartAsync(CancellationTokenSource.Token);
			});
		}

		[AfterScenario]
		public void CleanUp()
		{
			CancellationTokenSource.Cancel();
		}

		[BeforeScenarioBlock]
		public void WaitForAllMessagesToArrive()
		{
			if (ScenarioContext.CurrentScenarioBlock == ScenarioBlock.Then)
			{
				Thread.Sleep(1000);
			}
		}

		[Given(@"I have connected to the chat server")]
		public void GivenIHaveConnectedToTheChatServer()
		{
			Clients["me"] = CreateChatClient("me");
		}

		[Given(@"I have set my nickname as '(.*)'")]
		public void GivenIHaveSetMyNicknameAs(string nickname)
		{
			Clients["me"].SendMessage(nickname);
		}

		[Given(@"I have connected to '(.*)' chat room")]
		public void GivenIHaveConnectedToChatRoom(string room)
		{
			Clients["me"].SendMessage($"/room {room}");
			MyChatRoom = room;
		}

		[Given(@"'(.*)' has connected to chat room too")]
		public void GivenHasConnectedToChatRoomToo(string friend)
		{
			GetClient(friend).SendMessage($"/room {MyChatRoom}");
		}

		[Given(@"'(.*)' has connected to another chat room")]
		public void GivenHasConnectedToAnotherChatRoom(string person)
		{
			GetClient(person).SendMessage($"/room ANOTHER");
		}

		[When(@"I send a message like '(.*)'")]
		public void WhenISendAMessageLike(string message)
		{
			Clients["me"].SendMessage(message);
			MySentMessage = message;
		}

		[Then(@"I must receive: '(.*)'")]
		public void ThenIMustReceive(string message)
		{
			Assert.IsTrue(LastMessages["me"].Contains(message));
		}

		[Then(@"'(.*)' must receive: '(.*)'")]
		public void ThenMustReceive(string nick, string message)
		{
			Assert.IsTrue(LastMessages[nick].Contains(message));
		}

		[Then(@"'(.*)' mustn't receive my message")]
		public void ThenMustnTReceiveMyMessage(string nick)
		{
			Assert.IsFalse(LastMessages.ContainsKey(MySentMessage));
		}

		private ChatClient CreateChatClient(string nickname)
		{
			var chatClient = new ChatClient(Configuration);
			chatClient.OnReceiveMessage += message =>
			{
				lock (LastMessages)
				{
					if (!LastMessages.ContainsKey(nickname))
						LastMessages[nickname] = new List<string>();
					LastMessages[nickname].Add(message);
				}
			};
			chatClient.ConnectTo("localhost", 33000, CancellationTokenSource.Token);
			return chatClient;
		}

		private ChatClient GetClient(string friend)
		{
			if (!Clients.ContainsKey(friend))
			{
				Clients[friend] = CreateChatClient(friend);
				Clients[friend].SendMessage(friend);
			}
			return Clients[friend];
		}

	}
}
