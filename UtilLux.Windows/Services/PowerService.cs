using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;
using UtilLux.Core.Services.Power;

namespace UtilLux.Windows.Services;

public class PowerService(ILogger<PowerService> logger) : IPowerService
{
    private readonly ILogger<PowerService> _logger = logger;

    private static Guid GUID_SLEEP_SUBGROUP =
        new("238c9fa8-0aad-41ed-83f4-97be242c8f20");
    private static Guid GUID_SLEEP_AFTER =
        new("29f6c1db-86da-48c5-9fdb-f2b67b1f44da");


    [DllImport("powrprof.dll", CharSet = CharSet.Unicode)]
    static extern uint PowerGetActiveScheme(
    IntPtr UserRootPowerKey,
    ref IntPtr ActivePolicyGuid);

    [DllImport("powrprof.dll", CharSet = CharSet.Unicode)]
    static extern uint PowerReadACValue(
        IntPtr RootPowerKey,
        ref Guid SchemeGuid,
        ref Guid SubGroupOfPowerSettingGuid,
        ref Guid PowerSettingGuid,
        ref int Type,
        ref int Buffer,
        ref uint BufferSize);

    [DllImport("powrprof.dll", CharSet = CharSet.Unicode)]
    static extern UInt32 PowerWriteACValueIndex(IntPtr RootPowerKey,
    [MarshalAs(UnmanagedType.LPStruct)] Guid SchemeGuid,
    [MarshalAs(UnmanagedType.LPStruct)] Guid SubGroupOfPowerSettingsGuid,
    [MarshalAs(UnmanagedType.LPStruct)] Guid PowerSettingGuid,
    int AcValueIndex);

    [DllImport("powrprof.dll", CharSet = CharSet.Unicode)]
    static extern UInt32 PowerSetActiveScheme(IntPtr RootPowerKey,
    [MarshalAs(UnmanagedType.LPStruct)] Guid SchemeGuid);

    public async Task<bool> SetScreenSleepTime(int minutes)
    {
        _logger.LogDebug("Setting Screen sleep time to {sleepTime}", minutes);

        IntPtr activePolicyPtr = IntPtr.Zero;

        PowerGetActiveScheme(IntPtr.Zero, ref activePolicyPtr);

        var activePolicyGuid = Marshal.PtrToStructure<Guid>(activePolicyPtr);

        PowerWriteACValueIndex(IntPtr.Zero, activePolicyGuid, GUID_SLEEP_SUBGROUP, GUID_SLEEP_AFTER, minutes);

        PowerSetActiveScheme(IntPtr.Zero, activePolicyGuid);

        var newSleepTime = await GetScreenSleepTime();
        _logger.LogDebug("Screen sleep time set to {sleepTime}", newSleepTime);
        
        return true;
    }

    public async Task<int> GetScreenSleepTime()
    {
        IntPtr activePolicyPtr = IntPtr.Zero;

        PowerGetActiveScheme(IntPtr.Zero, ref activePolicyPtr);

        var activePolicyGuid = Marshal.PtrToStructure<Guid>(activePolicyPtr);
        var type = 0;
        var value = 0;
        var valueSize = 4u;

        PowerReadACValue(IntPtr.Zero, ref activePolicyGuid, ref GUID_SLEEP_SUBGROUP, ref GUID_SLEEP_AFTER, 
            ref type, ref value, ref valueSize);

        return (int)value;
    }
}
