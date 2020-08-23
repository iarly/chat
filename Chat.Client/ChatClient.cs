using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Sockets;

namespace Chat.Client
{
	public class ChatClient
	{
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
	}
}
