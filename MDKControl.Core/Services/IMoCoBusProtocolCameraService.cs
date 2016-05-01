using System.Threading.Tasks;

namespace MDKControl.Core.Services
{
	public interface IMoCoBusProtocolCameraService
	{
        Task SetPreDelayTime(ushort time);
        Task SetFocusTime(ushort time);
        Task SetTriggerTime(uint time);
        Task SetExposureDelayTime(ushort time);
        Task SetInterval(uint time);
        Task SetMaxShots(ushort shots);
        Task SetPanoRepititions(ushort shots);

        Task<ushort> GetPreDelayTime();
        Task<ushort> GetFocusTime();
        Task<uint> GetTriggerTime();
        Task<ushort> GetExposureDelayTime();
        Task<uint> GetInterval();
        Task<ushort> GetMaxShots();
        Task<ushort> GetPanoRepititions();
        Task<ushort> GetCurrentShots();
	}
}
