using Chat.Server.Application;
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

			await InjectionConfig.Get<ChatApplication>().StartAsync();
		}

		private static IConfiguration CreateConfiguration(string[] args)
		{
			IConfiguration Configuration = new ConfigurationBuilder()
			  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
			  .Build();

			return Configuration;
		}
	}
}
