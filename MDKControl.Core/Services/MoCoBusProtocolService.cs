using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MDKControl.Core.Models;

namespace MDKControl.Core.Services
{
    public class MoCoBusProtocolService : IMoCoBusProtocolService
    {
        private readonly IMoCoBusCommService _commService;
        private readonly byte _address;
        private readonly IMoCoBusProtocolMainService _mainService;
        private readonly IMoCoBusProtocolCameraService _cameraService;
        private readonly IMoCoBusProtocolMotorService _motorService1;
        private readonly IMoCoBusProtocolMotorService _motorService2;
        private readonly IMoCoBusProtocolMotorService _motorService3;

        public MoCoBusProtocolService(IMoCoBusCommService commService, byte address)
        {
            _commService = commService;
            _address = address;
            _mainService = new MoCoBusProtocolMainService(commService, address);
            _cameraService = new MoCoBusProtocolCameraService(commService, address);
            _motorService1 = new MoCoBusProtocolMotorService(commService, address, 1);
            _motorService2 = new MoCoBusProtocolMotorService(commService, address, 2);
            _motorService3 = new MoCoBusProtocolMotorService(commService, address, 3);
        }

        public IMoCoBusProtocolMainService Main { get { return _mainService; } }
        public IMoCoBusProtocolCameraService Camera { get { return _cameraService; } }
        public IMoCoBusProtocolMotorService Motor1 { get { return _motorService1; } }
        public IMoCoBusProtocolMotorService Motor2 { get { return _motorService2; } }
        public IMoCoBusProtocolMotorService Motor3 { get { return _motorService3; } }
    }
}
