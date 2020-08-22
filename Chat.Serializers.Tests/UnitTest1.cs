using Chat.Server.Domain.Commands;
using Chat.Server.Domain.Services;
using NUnit.Framework;
using System;

namespace Chat.Serializers.Tests
{
	public class Tests
	{
		CommandSerializer CommandSerializer;

		[SetUp]
		public void Setup()
		{
			CommandSerializer = new CommandSerializer();
		}

		[Test]
		public void Should_Serialize()
		{
			var expectedText = "{\"$type\":\"Chat.Server.Domain.Commands.SendMessageCommand, Chat.Server.Domain\",\"TargetClientNickname\":null,\"Content\":null,\"Private\":false,\"IsTargeted\":false,\"ConnectionUid\":\"00000000-0000-0000-0000-000000000000\"}";
			var text = CommandSerializer.Serializer(new SendMessageCommand());

			Assert.AreEqual(expectedText, text);
		}

		[Test]
		public void Should_Deserialize()
		{
			var expectedConnectionUid = Guid.NewGuid();
			var text = "{\"$type\":\"Chat.Server.Domain.Commands.SendMessageCommand, Chat.Server.Domain\",\"TargetClientNickname\":null,\"Content\":null,\"Private\":false,\"IsTargeted\":false,\"ConnectionUid\":\"" + expectedConnectionUid.ToString() + "\"}";

			var command = CommandSerializer.Deserialize(text);

			Assert.IsAssignableFrom<SendMessageCommand>(command);
			Assert.AreEqual(expectedConnectionUid, command.ConnectionUid);
		}
	}
}