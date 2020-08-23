using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Chat.Client
{
	public class ChatClient
	{
		public const string EOF = "\n\r";
		private IConfiguration Configuration;
		private Socket Socket;

		public ChatClient(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public Socket Client { get; private set; }

		public void ConnectTo(string host, int port)
		{
			IPHostEntry ipHostInfo = Dns.GetHostEntry(host);
			IPAddress ipAddress = ipHostInfo.AddressList[0];
			IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

			Client = new Socket(ipAddress.AddressFamily,
				SocketType.Stream, ProtocolType.Tcp);

			Client.Connect(remoteEP);
		}

		public void SendMessage(string expectedMessage)
		{
			byte[] byteData = Encoding.ASCII.GetBytes(expectedMessage + EOF);
			Client.Send(byteData, 0, byteData.Length, SocketFlags.None);
		}
	}
}
