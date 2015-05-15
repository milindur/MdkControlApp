namespace MDKControl.Core.Models
{
    public class MoCoBusMainCommandFrame : MoCoBusFrame
    {
        public MoCoBusMainCommandFrame(byte address, MoCoBusMainCommand command, byte[] data)
            : base(address, 0, (byte)command, data)
        {
        }

        public new MoCoBusMainCommand Command
        {
            get { return (MoCoBusMainCommand)base.Command; }
            set { base.Command = (byte)value; }
        }
    }
}