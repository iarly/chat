using Chat.Server.Domain.Commands;
using Chat.Server.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Chat.Server.Domain
{
	public interface IChatFacade
	{
		Task<Client> GetClientByUidAsync(Guid connectionUid);
		Task ProcessMessageAsync(Command command);
	}
}