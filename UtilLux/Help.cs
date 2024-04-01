namespace UtilLux;

public static class Help
{
    private static readonly string _helpText = $"""

        UtilLux

        A keyboard shortcut utility for Windows.

        Usage:
            UtilLux <command> [options]

            Commands:
            run
                Run the UtilLux service.

            stop
                Stop the UtilLux service.

            reload-settings
                Reload the UtilLux settings.

            check-if-running
                Check if the UtilLux service is running.

            show-help
                Show this help text.

        The options can be prefixed with '--', '-', or '/'.

        The following Exit Codes are possible:
            {ExitCode.Success} (0)
                The operation completed successfully.
            {ExitCode.Error} (1)
                An error occurred.
            {ExitCode.IncompatibleSettingsVersion} (2)
                The settings file is incompatible with the current version of UtilLux.
            {ExitCode.UtilLuxNotRunning} (3)
                The UtilLux service is not running.
            {ExitCode.UnknownCommand} (4)
                The command is unknown.
            {ExitCode.SettingsDoNotExist} (5)
                The settings file does not exist.
        """;

    public static ExitCode Show(TextWriter writer, ExitCode code)
    {
        writer.WriteLine(_helpText);
        return code;
    }
}
