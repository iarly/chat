using System;
using System.Net.Sockets;
using System.Text;

namespace Chat.Server.Communicator.Sockets.Models
{
	internal class ClientSocket
	{
		public const int BufferSize = 1024;

		public ClientSocket(Socket socket)
		{
			ConnectionUid = new Guid();
			Buffer = new byte[BufferSize];
			StringBuilder = new StringBuilder();
			Socket = socket;
		}

		public Socket Socket { get; internal set; }
		public Guid ConnectionUid { get; internal set; }
		public byte[] Buffer { get; internal set; }
		public StringBuilder StringBuilder { get; internal set; }
	}
}
