using System;
using System.Linq;
using System.Threading.Tasks;
using MDKControl.Core.Models;

namespace MDKControl.Core.Services
{
    public class MoCoBusProtocolCameraService : IMoCoBusProtocolCameraService
    {
        private readonly IMoCoBusCommService _commService;
        private readonly byte _address;

        public MoCoBusProtocolCameraService(IMoCoBusCommService commService, byte address)
        {
            _commService = commService;
            _address = address;
        }

        public async Task SetFocusTime(ushort time)
        {
            await _commService
                .SendAndReceiveAsync(new MoCoBusCameraCommandFrame(_address, MoCoBusCameraCommand.SetFocusTime, BitConverter.GetBytes(time).Reverse().ToArray()))
                .ConfigureAwait(false);
        }

        public async Task SetTriggerTime(uint time)
        {
            await _commService
                .SendAndReceiveAsync(new MoCoBusCameraCommandFrame(_address, MoCoBusCameraCommand.SetTriggerTime, BitConverter.GetBytes(time).Reverse().ToArray()))
                .ConfigureAwait(false);
        }

        public async Task SetInterval(uint time)
        {
            await _commService
                .SendAndReceiveAsync(new MoCoBusCameraCommandFrame(_address, MoCoBusCameraCommand.SetInterval, BitConverter.GetBytes(time).Reverse().ToArray()))
                .ConfigureAwait(false);
        }

        public async Task SetExposureDelayTime(ushort time)
        {
            await _commService
                .SendAndReceiveAsync(new MoCoBusCameraCommandFrame(_address, MoCoBusCameraCommand.SetExposureDelay, BitConverter.GetBytes(time).Reverse().ToArray()))
                .ConfigureAwait(false);
        }

        public async Task SetMaxShots(ushort shots)
        {
            await _commService
                .SendAndReceiveAsync(new MoCoBusCameraCommandFrame(_address, MoCoBusCameraCommand.SetMaxShots, BitConverter.GetBytes(shots).Reverse().ToArray()))
                .ConfigureAwait(false);
        }

        public async Task<ushort> GetFocusTime()
        {
            var response = await _commService
                .SendAndReceiveAsync(new MoCoBusCameraCommandFrame(_address, MoCoBusCameraCommand.GetFocusTime, null))
                .ConfigureAwait(false);

            return MoCoBusHelper.ParseStatus<ushort>(response);
        }

        public async Task<uint> GetTriggerTime()
        {
            var response = await _commService
                .SendAndReceiveAsync(new MoCoBusCameraCommandFrame(_address, MoCoBusCameraCommand.GetTriggerTime, null))
                .ConfigureAwait(false);

            return (uint)MoCoBusHelper.ParseStatus<int>(response);
        }

        public async Task<ushort> GetExposureDelayTime()
        {
            var response = await _commService
                .SendAndReceiveAsync(new MoCoBusCameraCommandFrame(_address, MoCoBusCameraCommand.GetExposureDelay, null))
                .ConfigureAwait(false);

            return MoCoBusHelper.ParseStatus<ushort>(response);
        }

        public async Task<uint> GetInterval()
        {
            var response = await _commService
                .SendAndReceiveAsync(new MoCoBusCameraCommandFrame(_address, MoCoBusCameraCommand.GetIntervalTime, null))
                .ConfigureAwait(false);

            return (uint)MoCoBusHelper.ParseStatus<int>(response);
        }

        public async Task<ushort> GetMaxShots()
        {
            var response = await _commService
                .SendAndReceiveAsync(new MoCoBusCameraCommandFrame(_address, MoCoBusCameraCommand.GetMaxShots, null))
                .ConfigureAwait(false);

            return (ushort)MoCoBusHelper.ParseStatus<int>(response);
        }

        public async Task<ushort> GetCurrentShots()
        {
            var response = await _commService
                .SendAndReceiveAsync(new MoCoBusCameraCommandFrame(_address, MoCoBusCameraCommand.GetCurrentShots, null))
                .ConfigureAwait(false);

            return MoCoBusHelper.ParseStatus<ushort>(response);
        }
    }
}
