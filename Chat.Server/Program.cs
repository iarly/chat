using Chat.Server.Application;
using Chat.Server.Communicator;
using Chat.Server.IoC;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Chat.Server
{
	class Program
	{
		static async Task Main(string[] args)
		{
			IConfiguration configuration = CreateConfiguration(args);

			InjectionConfig.ConfigureServices(configuration);

			InjectionConfig.Get<ICommunicator<string>>().OnClientConnected += Communicator_OnClientConnected;
			InjectionConfig.Get<ICommunicator<string>>().OnClientDisconnected += Communicator_OnClientDisconnected;
			InjectionConfig.Get<ICommunicator<string>>().OnClientSendCommand += Communicator_OnClientSendCommand;

			await InjectionConfig.Get<ChatApplication>().StartAsync();
		}

		private static Task Communicator_OnClientSendCommand(Guid connectionUid, string command)
		{
			Console.WriteLine($"{connectionUid} > {command}");
			return Task.CompletedTask;
		}

		private static Task Communicator_OnClientDisconnected(Guid connectionUid)
		{
			Console.WriteLine($"{connectionUid} > disconnected");
			return Task.CompletedTask;
		}

		private static Task Communicator_OnClientConnected(Guid connectionUid)
		{
			Console.WriteLine($"{connectionUid} > connected");
			return Task.CompletedTask;
		}

		private static IConfiguration CreateConfiguration(string[] args)
		{
			IConfiguration Configuration = new ConfigurationBuilder()
			  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
			  .AddCommandLine(args)
			  .Build();

			return Configuration;
		}
	}
}
