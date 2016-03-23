using System;
using System.Linq;
using System.Text;

namespace MDKControl.Core.Models
{
    public static class MoCoBusHelper
    {
        public static dynamic ParseStatus(MoCoBusFrame frame)
        {
            if (frame == null || frame.Length == 0) return null;

            switch ((MoCoBusStatusType)frame.Data[0])
            {
                default:
                    throw new ArgumentException();
                case MoCoBusStatusType.Byte:
                    return frame.Data[1];
                case MoCoBusStatusType.Int:
                    return BitConverter.ToInt16(frame.Data.Skip(1).Take(2).Reverse().ToArray(), 0);
                case MoCoBusStatusType.UInt:
                    return BitConverter.ToUInt16(frame.Data.Skip(1).Take(2).Reverse().ToArray(), 0);
                case MoCoBusStatusType.Long:
                    return BitConverter.ToInt32(frame.Data.Skip(1).Take(4).Reverse().ToArray(), 0);
                case MoCoBusStatusType.ULong:
                    return BitConverter.ToUInt32(frame.Data.Skip(1).Take(4).Reverse().ToArray(), 0);
                case MoCoBusStatusType.Float:
                    return BitConverter.ToInt32(frame.Data.Skip(1).Take(4).Reverse().ToArray(), 0) / 100.0;
                case MoCoBusStatusType.String:
                    return Encoding.UTF8.GetString(frame.Data, 1, frame.Data.Length - 1);
            }
        }

        public static T ParseStatus<T>(MoCoBusFrame frame)
        {
            var status = ParseStatus(frame);

            if (status is T)
            {
                return (T)status;
            }

            return default(T);
            throw new InvalidCastException();
        }
    }
}
