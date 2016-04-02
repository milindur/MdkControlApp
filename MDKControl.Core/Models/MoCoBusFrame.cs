using System;
using System.Linq;

namespace MDKControl.Core.Models
{
    public class MoCoBusFrame
    {
        protected MoCoBusFrame(byte address, byte subAddress, byte command, byte[] data)
        {
            Address = address;
            SubAddress = subAddress;
            Command = command;
            Data = (byte[]) data?.Clone();
        }

        public byte Address { get; set; }
        public byte SubAddress { get; set; }
        public byte Command { get; set; }
        public byte[] Data { get; set; }
        public byte Length => Data != null ? (byte)Data.Length : (byte)0;

        public static bool TryParse(byte[] bytes, out MoCoBusFrame frame)
        {
            frame = null;
            if (bytes.Length < 10) return false;
            if (bytes[0] != 0 || bytes[1] != 0 || bytes[2] != 0 || bytes[3] != 0 || bytes[4] != 0) return false;
            if (bytes[5] != 0xff) return false;
            if (bytes[9] != (bytes.Length - 10)) return false;

            switch (bytes[7])
            {
                case 0:
                    frame = new MoCoBusMainCommandFrame(bytes[6], (MoCoBusMainCommand)bytes[8], bytes.Skip(10).ToArray());
                    return true;
                case 1:
                case 2:
                case 3:
                    frame = new MoCoBusMotorCommandFrame(bytes[6], bytes[7], (MoCoBusMotorCommand)bytes[8], bytes.Skip(10).ToArray());
                    return true;
                case 4:
                    frame = new MoCoBusCameraCommandFrame(bytes[6], (MoCoBusCameraCommand)bytes[8], bytes.Skip(10).ToArray());
                    return true;
                default:
                    frame = new MoCoBusFrame(bytes[6], bytes[7], bytes[8], bytes.Skip(10).ToArray());
                    return true;
            }
        }

        public byte[] ToByteArray()
        {
            var result = new byte[10 + Length];
            result[5] = 0xff;
            result[6] = Address;
            result[7] = SubAddress;
            result[8] = Command;
            result[9] = Length;
            Data?.CopyTo(result, 10);
            return result;
        }

        public override string ToString()
        {
            return $"[MoCoBusFrame: Address={Address}, SubAddress={SubAddress}, Command={Command}, Data={BitConverter.ToString(Data)}]";
        }
    }
}