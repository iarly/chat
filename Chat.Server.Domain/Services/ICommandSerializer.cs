using Chat.Server.Domain.Commands;

namespace Chat.Server.Domain.Services
{
	public interface ICommandSerializer
	{
		string Serializer(Command command);

		Command Deserialize(string text);
	}
}
