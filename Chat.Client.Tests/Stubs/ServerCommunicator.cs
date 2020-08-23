using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Chat.Client.Tests.Stubs
{
	public delegate void ClientConnected();

	public class ServerCommunicator : IDisposable
	{
		private const string EOF = "\n\r";
		private Socket Server;
		private Socket ClientSocket;

		public event ClientConnected OnClientConnected;

		public ServerCommunicator(string host, int port)
		{
			IPHostEntry ipHostInfo = Dns.GetHostEntry(host);
			IPAddress ipAddress = ipHostInfo.AddressList[0];
			IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

			Server = new Socket(ipAddress.AddressFamily,
				SocketType.Stream, ProtocolType.Tcp);

			Server.Bind(remoteEP);

			Server.Listen(10);

			Server.BeginAccept(new AsyncCallback(Server_BeginAccept), null);
		}

		private void Server_BeginAccept(IAsyncResult ar)
		{
			ClientSocket = Server.EndAccept(ar);
			OnClientConnected();
		}

		public void Send(string data)
		{
			byte[] byteData = Encoding.ASCII.GetBytes(data + EOF);
			ClientSocket.Send(byteData, 0, byteData.Length, SocketFlags.None);
		}

		public string Receive()
		{
			byte[] bytes = new byte[1024];
			int bytesRec = ClientSocket.Receive(bytes);
			return Encoding.ASCII.GetString(bytes, 0, bytesRec);
		}

		public void Dispose()
		{
			Server.Close();
			Server.Dispose();
		}
	}
}
