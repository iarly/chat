﻿using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using Terminal.Gui;

namespace Chat.Client.ConsoleApp
{
	class Program
	{
		public static CustomInput MessageText { get; private set; }
		public static ChatClient ChatClient { get; private set; }
		public static Label ChatText { get; private set; }

		static void Main(string[] args)
		{
			IConfiguration configuration = CreateConfiguration(args);

			Application.Init();

			CreateInterface();

			ChatClient = new ChatClient(null);

			ChatClient.OnDisconnected += ChatClient_OnDisconnected;

			ChatClient.OnReceiveMessage += ChatClient_OnReceiveMessage;

			CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

			var host = configuration["host"] ?? "localhost";
			var port = int.Parse(configuration["port"] ?? "33000");

			ChatClient.ConnectTo(host, port, cancellationTokenSource.Token);

			Application.Run();
		}
		private static IConfiguration CreateConfiguration(string[] args)
		{
			IConfiguration Configuration = new ConfigurationBuilder()
			  .AddCommandLine(args)
			  .Build();

			return Configuration;
		}

		private static void ChatClient_OnDisconnected()
		{
			ChatText.Text += "You is disconnected\n";
		}

		private static void ChatClient_OnReceiveMessage(string message)
		{
			ChatText.Text += message + "\n";
			Application.Refresh();
		}

		private static void CreateInterface()
		{
			var top = Application.Top;

			var chatWindow = new Window("Chat")
			{
				X = 0,
				Y = 1,
				Width = Dim.Fill(),
				Height = Dim.Fill(3)
			};

			var messageWindow = new Window("Send message")
			{
				X = 0,
				Y = Pos.Bottom(top) - 3,
				Width = Dim.Fill(),
				Height = 3
			};

			top.Add(chatWindow);
			top.Add(messageWindow);

			MessageText = new CustomInput("")
			{
				X = 0,
				Y = 0,
				Width = Dim.Fill(),
				Height = Dim.Fill(),
			};

			MessageText.OnPressEnter += MessageText_OnPressEnter;

			ChatText = new Label("")
			{
				X = 0,
				Y = 0,
				Width = Dim.Fill() - 3,
				Height = Dim.Fill(),
			};

			messageWindow.Add(MessageText);
			chatWindow.Add(ChatText);

			var menu = new MenuBar(new MenuBarItem[] {
				new MenuBarItem ("_File", new MenuItem [] {
					new MenuItem ("_Quit", "", () => { })
				})
			});

			top.Add(menu);

			top.SetFocus(MessageText);
		}

		private static void MessageText_OnPressEnter()
		{
			ChatClient.SendMessage(MessageText.Text.ToString());
			MessageText.Text = string.Empty;
		}
	}

	public class CustomInput : TextField
	{
		public event Action OnPressEnter;

		public CustomInput(string text) : base(text)
		{
		}

		public override bool ProcessKey(KeyEvent kb)
		{
			if (kb.Key == Key.Enter)
			{
				OnPressEnter?.Invoke();
			}

			return base.ProcessKey(kb);
		}
	}
}
