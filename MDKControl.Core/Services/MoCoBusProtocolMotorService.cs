using System;
using System.Linq;
using System.Threading.Tasks;
using MDKControl.Core.Models;
using System.Diagnostics;

namespace MDKControl.Core.Services
{
    public class MoCoBusProtocolMotorService : IMoCoBusProtocolMotorService
    {
        private readonly IMoCoBusCommService _commService;
        private readonly byte _address;
        private readonly byte _motor;

        public MoCoBusProtocolMotorService(IMoCoBusCommService commService, byte address, byte motor)
        {
            _commService = commService;
            _address = address;
            _motor = motor;
        }

        public async Task SetContinuousSpeed(float speed)
        {
            Debug.WriteLine("MoCoBusProtocolMotorService ({0}): SetContinuousSpeed({1})", _motor, speed);

            speed = speed * 5000f / 100f;

            await _commService
                .SendAndReceiveAsync(new MoCoBusMotorCommandFrame(_address, _motor, MoCoBusMotorCommand.SetContinuousSpeed, BitConverter.GetBytes(speed).Reverse().ToArray()))
                .ConfigureAwait(false);
        }

        public async Task SetStartHere()
        {
            await _commService
                .SendAndReceiveAsync(new MoCoBusMotorCommandFrame(_address, _motor, MoCoBusMotorCommand.SetStartHere, null))
                .ConfigureAwait(false);
        }

        public async Task SetStopHere()
        {
            await _commService
                .SendAndReceiveAsync(new MoCoBusMotorCommandFrame(_address, _motor, MoCoBusMotorCommand.SetStopHere, null))
                .ConfigureAwait(false);
        }

        public async Task<int> GetProgramStartPoint()
        {
            var response = await _commService
                .SendAndReceiveAsync(new MoCoBusMotorCommandFrame(_address, _motor, MoCoBusMotorCommand.GetProgramStartPoint, null))
                .ConfigureAwait(false);

            return MoCoBusHelper.ParseStatus<int>(response);
        }

        public async Task<int> GetProgramStopPoint()
        {
            var response = await _commService
                .SendAndReceiveAsync(new MoCoBusMotorCommandFrame(_address, _motor, MoCoBusMotorCommand.GetProgramStopPoint, null))
                .ConfigureAwait(false);

            return MoCoBusHelper.ParseStatus<int>(response);
        }

        public async Task<uint> GetTravelShots()
        {
            var response = await _commService
                .SendAndReceiveAsync(new MoCoBusMotorCommandFrame(_address, _motor, MoCoBusMotorCommand.GetTravelShotsOrTime, null))
                .ConfigureAwait(false);

            return MoCoBusHelper.ParseStatus<uint>(response);
        }
    }
}
