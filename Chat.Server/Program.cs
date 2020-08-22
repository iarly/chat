using Chat.Server.Application;
using Chat.Server.IoC;
using System.Threading.Tasks;

namespace Chat.Server
{
	class Program
	{
		static async Task Main(string[] args)
		{
			InjectionConfig.ConfigureServices();

			await InjectionConfig.Get<ChatApplication>().StartAsync();
		}
	}
}
