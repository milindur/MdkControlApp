using System.Threading.Tasks;

namespace MDKControl.Core.Services
{
	public interface IMoCoBusProtocolCameraService
	{
        Task SetFocusTime(ushort time);
        Task SetTriggerTime(uint time);
        Task SetExposureDelayTime(ushort time);
        Task SetInterval(uint time);
        Task SetMaxShots(ushort shots);

        Task<ushort> GetFocusTime();
        Task<uint> GetTriggerTime();
        Task<ushort> GetExposureDelayTime();
        Task<uint> GetInterval();
        Task<ushort> GetMaxShots();
        Task<ushort> GetCurrentShots();
	}
}
