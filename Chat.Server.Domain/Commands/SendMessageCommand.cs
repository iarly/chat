namespace Chat.Server.Domain.Commands
{
	public class SendMessageCommand : Command
	{
		public string TargetClient { get; set; }
		public string Message { get; set; }
		public bool Private { get; set; }
	}
}
