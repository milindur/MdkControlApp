using System;
using System.Threading.Tasks;
using MDKControl.Core.Models;

namespace MDKControl.Core.Services
{
    public delegate IMoCoBusProtocolMotorService MoCoBusProtocolMotorServiceFactoryDelegate(IMoCoBusCommService commService, byte address, byte motor);
    
    public interface IMoCoBusProtocolMotorService
    {
        Task SetContinuousSpeed(float speed);
        Task SetStartHere();
        Task SetStopHere();

        Task<int> GetProgramStartPoint();
        Task<int> GetProgramStopPoint();
    }
}
