using Chat.Server.Communicator.Delegates;
using Chat.Server.Communicator.Sockets.Delegates;
using Chat.Server.Communicator.Sockets.Models;
using Chat.Server.Domain.Commands;
using Chat.Server.Domain.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.Server.Communicator.Sockets
{
	public class SocketCommunicator : ICommunicator
	{
		public const string EOF = "<\"EOF\">";
		protected IConfiguration Configuration { get; }
		protected ICommandSerializer CommandSerializer { get; }

		internal IDictionary<Guid, ClientSocket> Clients = new Dictionary<Guid, ClientSocket>();

		protected string Host { get; }
		protected int Port { get; }
		protected int MaxRequestsAtTime { get; }
		public CancellationToken CancellationToken { get; private set; }

		public event ServerReadyForConnectionsDelegate OnServerReady;
		public event ClientConnectedDelegate OnClientConnected;
		public event ClientDisconnectedDelegate OnClientDisconnected;
		public event ClientSendCommandDelegate OnClientSendCommand;

		protected EventWaitHandle ConnectionIsDone = new ManualResetEvent(false);

		public SocketCommunicator(IConfiguration configuration, ICommandSerializer commandSerializer)
		{
			Configuration = configuration;
			CommandSerializer = commandSerializer;
			Host = Configuration["host"] ?? "localhost";
			Port = int.Parse(Configuration["port"] ?? "33000");
			MaxRequestsAtTime = int.Parse(Configuration["max-clients"] ?? "100");
		}

		public Task ListenAsync(CancellationToken cancellationToken)
		{
			CancellationToken = cancellationToken;

			var hostEntry = Dns.GetHostEntry(Host);
			var ipAddress = hostEntry.AddressList.First();
			var endpoint = new IPEndPoint(ipAddress, Port);

			try
			{
				using (Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
				{
					listener.Bind(endpoint);

					listener.Listen(MaxRequestsAtTime);

					InvokeServerReadyForConnections();

					while (!cancellationToken.IsCancellationRequested)
					{
						ConnectionIsDone.Reset();
						listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

						WaitForCancelationTokenOrConnectionIsDoneSignal(cancellationToken);
					}

					listener.Close();
				}
			}
			catch
			{
				throw;
			}

			return Task.CompletedTask;
		}

		private void WaitForCancelationTokenOrConnectionIsDoneSignal(CancellationToken cancellationToken)
		{
			WaitHandle.WaitAny(new WaitHandle[] { ConnectionIsDone, cancellationToken.WaitHandle });
		}

		private void InvokeServerReadyForConnections()
		{
			OnServerReady?.Invoke();
		}

		public Task PublishAsync(Command command)
		{
			Socket handler = Clients[command.ConnectionUid].Socket;

			string data = CommandSerializer.Serializer(command);

			byte[] byteData = Encoding.ASCII.GetBytes(data);

			handler.Send(byteData);

			return Task.CompletedTask;
		}

		public void AcceptCallback(IAsyncResult result)
		{
			if (!CancellationToken.IsCancellationRequested)
			{
				ConnectionIsDone.Set();

				Socket listener = (Socket)result.AsyncState;
				Socket handler = listener.EndAccept(result);

				ClientSocket client = new ClientSocket(handler);

				Clients[client.ConnectionUid] = client;

				OnClientConnected?.Invoke(client.ConnectionUid);

				handler.BeginReceive(client.Buffer, 0,
					ClientSocket.BufferSize, 0,
					new AsyncCallback(ReadCallback), client);
			}
		}

		public void ReadCallback(IAsyncResult result)
		{
			ClientSocket client = (ClientSocket)result.AsyncState;
			Socket handler = client.Socket;

			int bytesRead = 0;

			if (!CancellationToken.IsCancellationRequested)
			{
				bytesRead = handler.EndReceive(result);
			}

			if (bytesRead > 0)
			{
				client.StringBuilder.Append(Encoding.ASCII.GetString(client.Buffer, 0, bytesRead));

				string content = client.StringBuilder.ToString();
				int indexOfEndOfFile = content.IndexOf(EOF);

				while (indexOfEndOfFile > -1)
				{
					var currentContent = content.Substring(0, indexOfEndOfFile);

					content = content.Substring(indexOfEndOfFile + EOF.Length);
					client.StringBuilder = new StringBuilder(content);

					OnClientSendCommand?.Invoke(client.ConnectionUid, CommandSerializer.Deserialize(currentContent));

					indexOfEndOfFile = content.IndexOf(EOF);
				}

				if (!CancellationToken.IsCancellationRequested)
				{
					handler.BeginReceive(client.Buffer, 0, ClientSocket.BufferSize, 0,
					new AsyncCallback(ReadCallback), client);
				}
			}
		}

	}
}
