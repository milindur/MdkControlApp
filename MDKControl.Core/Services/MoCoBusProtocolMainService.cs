using System;
using System.Linq;
using System.Threading.Tasks;
using MDKControl.Core.Models;

namespace MDKControl.Core.Services
{
    public class MoCoBusProtocolMainService : IMoCoBusProtocolMainService
    {
        private readonly IMoCoBusCommService _commService;
        private readonly byte _address;

        public MoCoBusProtocolMainService(IMoCoBusCommService commService, byte address)
        {
            _commService = commService;
            _address = address;
        }

        public async Task Start()
        {
            await _commService
                .SendAndReceiveAsync(new MoCoBusMainCommandFrame(_address, MoCoBusMainCommand.Start, null))
                .ConfigureAwait(false);
        }

        public async Task Stop()
        {
            await _commService
                .SendAndReceiveAsync(new MoCoBusMainCommandFrame(_address, MoCoBusMainCommand.Stop, null))
                .ConfigureAwait(false);
        }

        public Task SetProgramMode(MoCoBusProgramMode mode)
        {
            throw new NotImplementedException();
        }

        public Task SendAllMotorsToHome()
        {
            throw new NotImplementedException();
        }

        public Task SendAllMotorsToProgramStart()
        {
            throw new NotImplementedException();
        }

        public Task SetJoystickMode(bool enable)
        {
            throw new NotImplementedException();
        }

        public async Task SetJoystickWatchdog(bool enable)
        {
            await _commService
                .SendAndReceiveAsync(new MoCoBusMainCommandFrame(_address, MoCoBusMainCommand.SetJoystickMode, null))
                .ConfigureAwait(false);
        }

        public Task SetProgramStartPoint()
        {
            throw new NotImplementedException();
        }

        public Task SetProgramStopPoint()
        {
            throw new NotImplementedException();
        }

        public Task ReverseAllMotorsStartStopPoints()
        {
            throw new NotImplementedException();
        }

        public async Task<int> GetFirmwareVersion()
        {
            var response = await _commService
                .SendAndReceiveAsync(new MoCoBusMainCommandFrame(_address, MoCoBusMainCommand.GetFirmwareVersion, null))
                .ConfigureAwait(false);

            return MoCoBusHelper.ParseStatus<int>(response);
        }

        public Task<MoCoBusProgramMode> GetProgramMode()
        {
            throw new NotImplementedException();
        }

        public Task<MoCoBusRunStatus> GetRunStatus()
        {
            throw new NotImplementedException();
        }

        public Task<TimeSpan> GetRunTime()
        {
            throw new NotImplementedException();
        }

        public Task<TimeSpan> GetTotalProgramRunTime()
        {
            throw new NotImplementedException();
        }

        public Task<float> GetProgramPercentComplete()
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsProgramComplete()
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetJoystickMode()
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetJoystickWatchdogStatus()
        {
            throw new NotImplementedException();
        }
    }
}
