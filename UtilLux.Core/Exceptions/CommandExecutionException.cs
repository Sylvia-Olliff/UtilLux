namespace UtilLux.Core.Exceptions;

public class CommandExecutionException : Exception
{
    public readonly string CommandName;

    public CommandExecutionException(string commandName) : base()
    {
        CommandName = commandName;
    }

    public CommandExecutionException(string message, string commandName) : base(message)
    {
        CommandName = commandName;
    }

    public CommandExecutionException(Exception cause, string message, string commandName) : base(message, cause) 
    {
        CommandName = commandName;
    }
}
