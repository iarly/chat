using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Chat.Server.Communicator.Sockets.Tests.Stubs
{
	public class ClientConnection
	{
		private const string EOF = SocketCommunicator.EOF;
		private Socket Client;

		public ClientConnection(string host, int port)
		{
			IPHostEntry ipHostInfo = Dns.GetHostEntry(host);
			IPAddress ipAddress = ipHostInfo.AddressList[0];
			IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

			Client = new Socket(ipAddress.AddressFamily,
				SocketType.Stream, ProtocolType.Tcp);

			Client.Connect(remoteEP);
		}

		public void Send(string data)
		{
			byte[] byteData = Encoding.ASCII.GetBytes(data + EOF);
			Client.Send(byteData, 0, byteData.Length, SocketFlags.None);
		}

		public string Receive()
		{
			byte[] bytes = new byte[1024];
			int bytesRec = Client.Receive(bytes);
			return Encoding.ASCII.GetString(bytes, 0, bytesRec);
		}
	}
}
