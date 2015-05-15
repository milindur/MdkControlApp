namespace MDKControl.Core.Models
{
    public class MoCoBusCameraCommandFrame : MoCoBusFrame
    {
        public MoCoBusCameraCommandFrame(byte address, MoCoBusCameraCommand command, byte[] data)
            : base(address, 4, (byte)command, data)
        {
        }

        public new MoCoBusCameraCommand Command
        {
            get { return (MoCoBusCameraCommand)base.Command; }
            set { base.Command = (byte)value; }
        }
    }
}