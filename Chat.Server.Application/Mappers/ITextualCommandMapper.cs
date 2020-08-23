using Chat.Server.Application.Enumerators;
using Chat.Server.Domain.Commands;
using System;

namespace Chat.Server.Application.Mappers
{
	public interface ITextualCommandMapper
	{
		Command ToCommand(Guid connectionUid, ClientState currentState, string message);
		string ToString(Command command);
	}
}