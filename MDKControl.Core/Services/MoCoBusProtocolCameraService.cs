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
    }
}
