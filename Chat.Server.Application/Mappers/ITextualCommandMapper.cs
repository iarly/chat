using Chat.Server.Domain.Commands;
using Chat.Server.Domain.Enumerators;
using System;

namespace Chat.Server.Application.Mappers
{
	public interface ITextualCommandMapper
	{
		Command ToCommand(Guid connectionUid, ClientState currentState, string message);
		string ToString(Command command);
	}
}