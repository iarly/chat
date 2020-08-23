using Chat.Server.Domain.Commands;

namespace Chat.Server.Domain.Services
{
	public interface ISerializer<TMessage>
	{
		string Serializer(TMessage command);

		TMessage Deserialize(string text);
	}
}
