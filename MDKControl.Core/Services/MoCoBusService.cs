using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MDKControl.Core.Models;

namespace MDKControl.Core.Services
{
    public class MoCoBusService : IMoCoBusMainService
    {
        private readonly IMoCoBusDeviceService _deviceService;
        private readonly byte _address;

        public MoCoBusService(IMoCoBusDeviceService deviceService, byte address)
        {
            _deviceService = deviceService;
            _address = address;
        }

        public async Task StartAsync()
        {
            await _deviceService
                .SendAndReceiveAsync(new MoCoBusMainCommandFrame(_address, MoCoBusMainCommand.Start, null))
                .ConfigureAwait(false);
        }

        public async Task StopAsync()
        {
            await _deviceService
                .SendAndReceiveAsync(new MoCoBusMainCommandFrame(_address, MoCoBusMainCommand.Stop, null))
                .ConfigureAwait(false);
        }

        public Task SetProgramModeAsync(MoCoBusProgramMode mode)
        {
            throw new NotImplementedException();
        }

        public Task SendAllMotorsToHomeAsync()
        {
            throw new NotImplementedException();
        }

        public Task SendAllMotorsToProgramStartAsync()
        {
            throw new NotImplementedException();
        }

        public Task SetJoystickModeAsync(bool enable)
        {
            throw new NotImplementedException();
        }

        public Task SetJoystickWatchdogAsync(bool enable)
        {
            throw new NotImplementedException();
        }

        public Task SetProgramStartPointAsync()
        {
            throw new NotImplementedException();
        }

        public Task SetProgramStopPointAsync()
        {
            throw new NotImplementedException();
        }

        public Task ReverseAllMotorsStartStopPointsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<uint> GetFirmwareVersionAsync()
        {
            throw new NotImplementedException();
        }

        public Task<MoCoBusProgramMode> GetProgramModeAsync()
        {
            throw new NotImplementedException();
        }

        public Task<MoCoBusRunStatus> GetRunStatusAsync()
        {
            throw new NotImplementedException();
        }

        public Task<TimeSpan> GetRunTimeAsync()
        {
            throw new NotImplementedException();
        }

        public Task<TimeSpan> GetTotalProgramRunTimeAsync()
        {
            throw new NotImplementedException();
        }

        public Task<float> GetProgramPercentCompleteAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsProgramCompleteAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetJoystickModeAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetJoystickWatchdogStatusAsync()
        {
            throw new NotImplementedException();
        }
    }
}
