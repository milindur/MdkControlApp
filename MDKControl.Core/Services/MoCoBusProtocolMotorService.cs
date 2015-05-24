using System;
using System.Linq;
using System.Threading.Tasks;
using MDKControl.Core.Models;

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
            speed = speed * 5000f / 100f;

            await _commService
                .SendAndReceiveAsync(new MoCoBusMotorCommandFrame(_address, _motor, MoCoBusMotorCommand.SetContinuousSpeed, BitConverter.GetBytes(speed).Reverse().ToArray()))
                .ConfigureAwait(false);
        }
    }
}
