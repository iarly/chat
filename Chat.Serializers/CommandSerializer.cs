using Chat.Server.Domain.Commands;
using Chat.Server.Domain.Services;
using Newtonsoft.Json;

namespace Chat.Serializers
{
	public class CommandSerializer : ISerializer<Command>
	{
		public JsonSerializerSettings JsonSerializerSettings { get; }

		public CommandSerializer()
		{
			JsonSerializerSettings = new JsonSerializerSettings();
			JsonSerializerSettings.TypeNameHandling = TypeNameHandling.Objects;
			JsonSerializerSettings.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;
		}

		public Command Deserialize(string text)
		{
			return JsonConvert.DeserializeObject(text, JsonSerializerSettings) as Command;
		}

		public string Serializer(Command command)
		{
			return JsonConvert.SerializeObject(command, JsonSerializerSettings);
		}
	}
}
