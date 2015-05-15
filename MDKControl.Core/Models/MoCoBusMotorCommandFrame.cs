using System;

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

        public override string ToString()
        {
            return string.Format("[MoCoBusMotorCommandFrame: Address={0}, Motor={1}, Command={2}, Data={3}]", Address, SubAddress, Command, BitConverter.ToString(Data));
        }
    }
}