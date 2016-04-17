namespace MDKControl.Core.Services
{
	public interface IMoCoBusProtocolService
	{
        IMoCoBusProtocolMainService Main { get; }
        IMoCoBusProtocolCameraService Camera { get; }
        IMoCoBusProtocolMotorService MotorSlider { get; }
        IMoCoBusProtocolMotorService MotorPan { get; }
        IMoCoBusProtocolMotorService MotorTilt { get; }
	}
}
