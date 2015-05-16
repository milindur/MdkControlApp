using System;
using System.Threading.Tasks;
using MDKControl.Core.Models;

namespace MDKControl.Core.Services
{
    public interface IMoCoBusMainService
    {
        Task StartAsync();
        Task StopAsync();

        Task SetProgramModeAsync(MoCoBusProgramMode mode);
        //void SetMotorsMaxStepRateAsync(ushort maxStepRate);

        Task SendAllMotorsToHomeAsync();
        Task SendAllMotorsToProgramStartAsync();

        Task SetJoystickModeAsync(bool enable);
        Task SetJoystickWatchdogAsync(bool enable);

        Task SetProgramStartPointAsync();
        Task SetProgramStopPointAsync();
        Task ReverseAllMotorsStartStopPointsAsync();

        Task<uint> GetFirmwareVersionAsync();

        Task<MoCoBusProgramMode> GetProgramModeAsync();
        //Task<ushort> GetMotorsMaxStepRateAsync();

        Task<MoCoBusRunStatus> GetRunStatusAsync();
        Task<TimeSpan> GetRunTimeAsync();
        Task<TimeSpan> GetTotalProgramRunTimeAsync();
        Task<float> GetProgramPercentCompleteAsync();
        Task<bool> IsProgramCompleteAsync();

        //Task<bool> GetAllMotorsRunStatusAsync();
        //Task<bool> GetSleepStateOfAllMotorsAsync();

        Task<bool> GetJoystickModeAsync();
        Task<bool> GetJoystickWatchdogStatusAsync();
    }
}
