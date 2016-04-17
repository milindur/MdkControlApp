using System;
using System.Threading.Tasks;
using MDKControl.Core.Models;

namespace MDKControl.Core.Services
{
    public interface IMoCoBusProtocolMainService
    {
        Task Start();
        Task Start(byte arg);
        Task Start(byte arg1, byte arg2);
        Task Pause();
        Task Stop();

        Task SetProgramMode(MoCoBusProgramMode mode);
        //void SetMotorsMaxStepRate(ushort maxStepRate);

        Task SendAllMotorsToHome();
        Task SendAllMotorsToProgramStart();

        Task SetJoystickMode(bool enable);
        Task SetJoystickWatchdog(bool enable);

        Task SetProgramStartPoint();
        Task SetProgramStartPoint(Motors motors);
        Task SetProgramStopPoint();
        Task SetProgramStopPoint(Motors motors);
        Task ReverseAllMotorsStartStopPoints();
        Task ReverseAllMotorsStartStopPoints(Motors motors);

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
