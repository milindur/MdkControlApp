using System;
using System.Threading.Tasks;
using MDKControl.Core.Models;

namespace MDKControl.Core.Services
{
    public interface IMoCoBusProtocolMainService
    {
        Task Start();
        Task Start(byte arg);
        Task Pause();
        Task Stop();

        Task SetProgramMode(MoCoBusProgramMode mode);
        //void SetMotorsMaxStepRate(ushort maxStepRate);

        Task SendAllMotorsToHome();
        Task SendAllMotorsToProgramStart();

        Task SetJoystickMode(bool enable);
        Task SetJoystickWatchdog(bool enable);

        Task SetProgramStartPoint();
        Task SetProgramStopPoint();
        Task ReverseAllMotorsStartStopPoints();

        Task<int> GetFirmwareVersion();

        Task<MoCoBusProgramMode> GetProgramMode();
        //Task<ushort> GetMotorsMaxStepRate();

        Task<MoCoBusRunStatus> GetRunStatus();
        Task<TimeSpan> GetRunTime();
        Task<TimeSpan> GetTotalProgramRunTime();
        Task<float> GetProgramPercentComplete();
        Task<bool> IsProgramComplete();

        //Task<bool> GetAllMotorsRunStatus();
        //Task<bool> GetSleepStateOfAllMotors();

        Task<bool> GetJoystickMode();
        Task<bool> GetJoystickWatchdogStatus();
    }
}
