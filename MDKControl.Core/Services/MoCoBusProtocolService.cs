namespace MDKControl.Core.Services
{
    public class MoCoBusProtocolService : IMoCoBusProtocolService
    {
        public MoCoBusProtocolService(IMoCoBusCommService commService, byte address)
        {
            Main = new MoCoBusProtocolMainService(commService, address);
            Camera = new MoCoBusProtocolCameraService(commService, address);
            MotorSlider = new MoCoBusProtocolMotorService(commService, address, 1);
            MotorPan = new MoCoBusProtocolMotorService(commService, address, 2);
            MotorTilt = new MoCoBusProtocolMotorService(commService, address, 3);
        }

        public IMoCoBusProtocolMainService Main { get; }
        public IMoCoBusProtocolCameraService Camera { get; }
        public IMoCoBusProtocolMotorService MotorSlider { get; }
        public IMoCoBusProtocolMotorService MotorPan { get; }
        public IMoCoBusProtocolMotorService MotorTilt { get; }
    }
}
