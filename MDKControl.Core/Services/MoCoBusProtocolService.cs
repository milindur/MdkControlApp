namespace MDKControl.Core.Services
{
    public class MoCoBusProtocolService : IMoCoBusProtocolService
    {
        public MoCoBusProtocolService(IMoCoBusCommService commService, byte address)
        {
            Main = new MoCoBusProtocolMainService(commService, address);
            Camera = new MoCoBusProtocolCameraService(commService, address);
            Motor1 = new MoCoBusProtocolMotorService(commService, address, 1);
            Motor2 = new MoCoBusProtocolMotorService(commService, address, 2);
            Motor3 = new MoCoBusProtocolMotorService(commService, address, 3);
        }

        public IMoCoBusProtocolMainService Main { get; }
        public IMoCoBusProtocolCameraService Camera { get; }
        public IMoCoBusProtocolMotorService Motor1 { get; }
        public IMoCoBusProtocolMotorService Motor2 { get; }
        public IMoCoBusProtocolMotorService Motor3 { get; }
    }
}
