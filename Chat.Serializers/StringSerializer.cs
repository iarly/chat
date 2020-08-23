using Chat.Server.Domain.Services;

namespace Chat.Serializers
{
	public class StringSerializer : ISerializer<string>
	{
		public StringSerializer()
		{
		}

		public string Deserialize(string text)
		{
			return text;
		}

		public string Serializer(string command)
		{
			return command;
		}
	}
}
