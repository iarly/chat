namespace Chat.Server.Domain.Commands
{
	public class SetNicknameCommand : Command
	{
		public string Nickname { get; set; }
	}
}
