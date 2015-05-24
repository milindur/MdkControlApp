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

    }
}
