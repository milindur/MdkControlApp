namespace MDKControl.Core.Services
{
	public interface IMoCoBusProtocolService
	{
        IMoCoBusProtocolMainService Main { get; }
        IMoCoBusProtocolCameraService Camera { get; }
        IMoCoBusProtocolMotorService Motor1 { get; }
        IMoCoBusProtocolMotorService Motor2 { get; }
        IMoCoBusProtocolMotorService Motor3 { get; }
	}
}
