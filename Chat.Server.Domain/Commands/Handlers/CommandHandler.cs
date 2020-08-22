using System.Threading.Tasks;

namespace Chat.Server.Domain.Commands.Handlers
{
	public abstract class CommandHandler<TCommand> : ICommandHandler
	{
		public Task ProcessAsync(Command command)
		{
			if (command is TCommand myCommand)
			{
				return InternalProcessAsync(myCommand);
			}

			return null;
		}

		protected abstract Task InternalProcessAsync(TCommand command);
	}
}
