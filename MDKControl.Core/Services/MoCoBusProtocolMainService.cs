using System;
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

        public async Task Start(byte arg)
        {
            await _commService
                .SendAndReceiveAsync(new MoCoBusMainCommandFrame(_address, MoCoBusMainCommand.Start, new[] { arg }))
                .ConfigureAwait(false);
        }

        public async Task Start(byte arg1, byte arg2)
        {
            await _commService
                .SendAndReceiveAsync(new MoCoBusMainCommandFrame(_address, MoCoBusMainCommand.Start, new[] { arg1, arg2 }))
                .ConfigureAwait(false);
        }

        public async Task Pause()
        {
            await _commService
                .SendAndReceiveAsync(new MoCoBusMainCommandFrame(_address, MoCoBusMainCommand.Pause, null))
                .ConfigureAwait(false);
        }

        public async Task Stop()
        {
            await _commService
                .SendAndReceiveAsync(new MoCoBusMainCommandFrame(_address, MoCoBusMainCommand.Stop, null))
                .ConfigureAwait(false);
        }

        public async Task SetProgramMode(MoCoBusProgramMode mode)
        {
            await _commService
                .SendAndReceiveAsync(new MoCoBusMainCommandFrame(_address, MoCoBusMainCommand.SetProgramMode, new[] { (byte)mode }))
                .ConfigureAwait(false);
        }

        public async Task SendAllMotorsToHome()
        {
            await _commService
                .SendAndReceiveAsync(new MoCoBusMainCommandFrame(_address, MoCoBusMainCommand.SendAllMotorsToHome, null))
                .ConfigureAwait(false);
        }

        public async Task SendAllMotorsToProgramStart()
        {
            await _commService
                .SendAndReceiveAsync(new MoCoBusMainCommandFrame(_address, MoCoBusMainCommand.SendAllMotorsToProgramStart, null))
                .ConfigureAwait(false);
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

        public async Task SetProgramStartPoint()
        {
            await _commService
                .SendAndReceiveAsync(new MoCoBusMainCommandFrame(_address, MoCoBusMainCommand.SetProgramStartPoint, null))
                .ConfigureAwait(false);
        }

        public async Task SetProgramStopPoint()
        {
            await _commService
                .SendAndReceiveAsync(new MoCoBusMainCommandFrame(_address, MoCoBusMainCommand.SetProgramStopPoint, null))
                .ConfigureAwait(false);
        }

        public async Task ReverseAllMotorsStartStopPoints()
        {
            await _commService
                .SendAndReceiveAsync(new MoCoBusMainCommandFrame(_address, MoCoBusMainCommand.ReverseAllMotorsStartStopPoints, null))
                .ConfigureAwait(false);
        }

        public async Task<int> GetFirmwareVersion()
        {
            var response = await _commService
                .SendAndReceiveAsync(new MoCoBusMainCommandFrame(_address, MoCoBusMainCommand.GetFirmwareVersion, null))
                .ConfigureAwait(false);

            return MoCoBusHelper.ParseStatus<int>(response);
        }

        public async Task<MoCoBusProgramMode> GetProgramMode()
        {
            var response = await _commService
                .SendAndReceiveAsync(new MoCoBusMainCommandFrame(_address, MoCoBusMainCommand.GetProgramMode, null))
                .ConfigureAwait(false);

            return (MoCoBusProgramMode)MoCoBusHelper.ParseStatus<byte>(response);
        }

        public async Task<MoCoBusRunStatus> GetRunStatus()
        {
            var response = await _commService
                .SendAndReceiveAsync(new MoCoBusMainCommandFrame(_address, MoCoBusMainCommand.GetRunStatus, null))
                .ConfigureAwait(false);

            return (MoCoBusRunStatus)MoCoBusHelper.ParseStatus<byte>(response);
        }

        public async Task<TimeSpan> GetRunTime()
        {
            var response = await _commService
                .SendAndReceiveAsync(new MoCoBusMainCommandFrame(_address, MoCoBusMainCommand.GetRunTime, null))
                .ConfigureAwait(false);

            return TimeSpan.FromMilliseconds(MoCoBusHelper.ParseStatus<uint>(response));
        }

        public async Task<TimeSpan> GetTotalProgramRunTime()
        {
            var response = await _commService
                .SendAndReceiveAsync(new MoCoBusMainCommandFrame(_address, MoCoBusMainCommand.GetTotalProgramRunTime, null))
                .ConfigureAwait(false);

            return TimeSpan.FromMilliseconds(MoCoBusHelper.ParseStatus<uint>(response));
        }

        public async Task<float> GetProgramPercentComplete()
        {
            var response = await _commService
                .SendAndReceiveAsync(new MoCoBusMainCommandFrame(_address, MoCoBusMainCommand.GetProgramPercentComplete, null))
                .ConfigureAwait(false);

            return (float)MoCoBusHelper.ParseStatus<byte>(response);
        }

        public async Task<bool> IsProgramComplete()
        {
            var response = await _commService
                .SendAndReceiveAsync(new MoCoBusMainCommandFrame(_address, MoCoBusMainCommand.GetProgramComplete, null))
                .ConfigureAwait(false);

            return MoCoBusHelper.ParseStatus<byte>(response) == 1;
        }

        public Task<bool> GetJoystickMode()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> GetJoystickWatchdogStatus()
        {
            var response = await _commService
                .SendAndReceiveAsync(new MoCoBusMainCommandFrame(_address, MoCoBusMainCommand.GetJoystickWatchdogStatus, null))
                .ConfigureAwait(false);

            return MoCoBusHelper.ParseStatus<byte>(response) == 1;
        }
    }
}
