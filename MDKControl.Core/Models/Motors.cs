using System;

namespace MDKControl.Core.Models
{
    [Flags]
    public enum Motors : byte
    {
        None = 0,
        MotorSlider = 1,
        MotorPan = 2,
        MotorTilt = 4,
    }
}
