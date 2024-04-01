using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using UtilLux.Core.Json;
using UtilLux.Core.Services.Users;
using UtilLux.Core.Settings;

namespace UtilLux.Core.Services.InitialSetup;

public abstract class OneTimeInitialSetupService(
    IUserProvider userProvider,
    IOptions<GlobalSettings> globalSettings,
    ILogger<OneTimeInitialSetupService> logger) : IInitialSetupService
{

    private readonly FileInfo initialSetupFile =
        new(Environment.ExpandEnvironmentVariables(globalSettings.Value.InitialSetupFilePath));

    public void InitializeUtiLuxSetup()
    {
        if (File.Exists(initialSetupFile.FullName))
        {
            logger.LogInformation("Initial setup already completed");
            return;
        }

        logger.LogInformation("Starting initial setup");

        var currentUser = userProvider.GetCurrentUser();

        if (currentUser is null)
        {
            logger.LogWarning("Could not determine current user - No initial setup will be done");
            return;
        }

        try
        {
            if (OperatingSystem.IsWindows())
            {
                this.InitializeWindowsSetup(currentUser, this.DoInitialSetup);
            }
            else
            {
                this.InitializeNonWindowsSetup(currentUser, this.DoInitialSetup);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during initial setup");
            return;
        }

        logger.LogInformation("Initial setup completed");
    }

    protected abstract void DoInitialSetup(string currentUser);

    private void InitializeWindowsSetup(string currentUser, Action<string> doInitialSetup)
    {
        logger.LogDebug("Performing initial setup for Windows");
        var users = this.ReadUsersFromFile();

        if (!users.Contains(currentUser))
        {
            logger.LogDebug("Retrieved user : {currentUser} from file", currentUser);
            doInitialSetup(currentUser);
            users.Add(currentUser);
            this.WriteUserToFile(users);
        }
    }

    private List<string> ReadUsersFromFile()
    {
        if (!this.initialSetupFile.Exists)
        {
            return [];
        }

        using var fileStream = new BufferedStream(this.initialSetupFile.OpenRead());
        return JsonSerializer.Deserialize(fileStream, CommandSettingsJsonContext.Default.ListString) ?? [];
    }

    private void WriteUserToFile(List<string> users)
    {
        using var fileStream = new BufferedStream(this.initialSetupFile.OpenWrite());
        JsonSerializer.Serialize(fileStream, users, CommandSettingsJsonContext.Default.ListString);
    }

    private void InitializeNonWindowsSetup(string currentUser, Action<string> doInitialSetup)
    {
        bool fileExists = this.initialSetupFile.Exists;

        string? processPath = Environment.ProcessPath;
        bool shouldDoSetup = !fileExists ||
            processPath is not null && this.initialSetupFile.LastWriteTimeUtc < File.GetCreationTimeUtc(processPath);

        if (shouldDoSetup)
        {
            doInitialSetup(currentUser);
        }

        if (!fileExists)
        {
            this.initialSetupFile.Create().Dispose();
        }

        this.initialSetupFile.LastWriteTimeUtc = DateTime.UtcNow;
    }
}
