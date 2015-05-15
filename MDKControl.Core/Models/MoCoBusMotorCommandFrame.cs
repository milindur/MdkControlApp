namespace MDKControl.Core.Models
{
    public class MoCoBusMotorCommandFrame : MoCoBusFrame
    {
        public MoCoBusMotorCommandFrame(byte address, byte motor, MoCoBusMotorCommand command, byte[] data)
            : base(address, motor, (byte)command, data)
        {
        }

        public new MoCoBusMotorCommand Command
        {
            get { return (MoCoBusMotorCommand)base.Command; }
            set { base.Command = (byte)value; }
        }
    }
}