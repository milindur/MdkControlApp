using System;
using System.Threading.Tasks;
using MDKControl.Core.Models;

namespace MDKControl.Core.Services
{
    public interface IMoCoBusProtocolMotorService
    {
        Task SetContinuousSpeed(float speed);
    }
}
