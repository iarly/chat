using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Chat.Client
{
	public delegate void ReceiveMessage(string message);

	public delegate void Disconnected();


	public class ChatClient
	{
		public const string EOF = "\n\r";
		private const int BufferSize = 1024;

		private IConfiguration Configuration;
		private Socket Socket;
		private byte[] Buffer = new byte[BufferSize];
		private StringBuilder ReceivedContent = new StringBuilder();

		public ChatClient(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public Socket Client { get; private set; }
		public CancellationToken CancellationToken { get; private set; }

		public event ReceiveMessage OnReceiveMessage;
		public event Disconnected OnDisconnected;

		public void ConnectTo(string host, int port, CancellationToken cancellationToken)
		{
			CancellationToken = cancellationToken;
			IPHostEntry ipHostInfo = Dns.GetHostEntry(host);
			IPAddress ipAddress = ipHostInfo.AddressList[0];
			IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

			Client = new Socket(ipAddress.AddressFamily,
				SocketType.Stream, ProtocolType.Tcp);

			Client.Connect(remoteEP);

			Client.BeginReceive(Buffer, 0, BufferSize, SocketFlags.None, new AsyncCallback(Client_OnReceive), null);
		}

		public void Client_OnReceive(IAsyncResult asyncResult)
		{
			int receivedSize = Client.EndReceive(asyncResult);

			if (receivedSize > 0)
			{
				ReceivedContent.Append(Encoding.ASCII.GetString(Buffer, 0, receivedSize));

				string content = ReceivedContent.ToString();
				int indexOfEndOfFile = content.IndexOf(EOF);

				while (indexOfEndOfFile > -1)
				{
					var currentContent = content.Substring(0, indexOfEndOfFile);

					content = content.Substring(indexOfEndOfFile + EOF.Length);
					ReceivedContent = new StringBuilder(content);

					OnReceiveMessage?.Invoke(currentContent);

					indexOfEndOfFile = content.IndexOf(EOF);
				}
			}

			if (!Client.Connected)
			{
				OnDisconnected?.Invoke();
			}
			else if (!CancellationToken.IsCancellationRequested)
			{
				Client.BeginReceive(Buffer, 0, BufferSize, SocketFlags.None, new AsyncCallback(Client_OnReceive), null);
			}
		}

		public void SendMessage(string expectedMessage)
		{
			byte[] byteData = Encoding.ASCII.GetBytes(expectedMessage + EOF);
			Client.Send(byteData, 0, byteData.Length, SocketFlags.None);
		}
	}
}
