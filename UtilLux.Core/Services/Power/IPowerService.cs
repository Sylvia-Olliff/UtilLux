namespace UtilLux.Core.Services.Power;

public interface IPowerService
{
    public Task<bool> SetScreenSleepTime(int minutes);

    public Task<int> GetScreenSleepTime();

}
