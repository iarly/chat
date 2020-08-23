using Chat.Server.Application.Enumerators;
using Chat.Server.Application.Mappers;
using Chat.Server.Domain.Commands;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chat.Server.Application.Tests.Mappers
{
	public class TextualCommandMapperTest
	{
		private TextualCommandMapper Mapper;

		[SetUp]
		public void Setup()
		{
			Mapper = new TextualCommandMapper();
		}

		[Test]
		public void Should_Convert_Message_To_SetNicknameCommand_When_WaitingNickaname()
		{
			string messageSentByClient = "Joey";
			ClientState currentStateOfClient = ClientState.WaitingNickname;
			Guid connectionUid = Guid.NewGuid();

			Command command = Mapper.ToCommand(connectionUid, currentStateOfClient, messageSentByClient);

			Assert.IsAssignableFrom<SetNicknameCommand>(command);
			Assert.AreEqual(messageSentByClient, ((SetNicknameCommand)command).Nickname);
			Assert.AreEqual(connectionUid, ((SetNicknameCommand)command).ConnectionUid);
		}

	}
}
