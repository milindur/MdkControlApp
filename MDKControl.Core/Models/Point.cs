using System;

namespace MDKControl.Core.Models
{
    public class Point
    {
        public Point(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public float X { get; private set; }
        public float Y { get; private set; }
        public float Z { get; private set; }

        public override string ToString()
        {
            return string.Format("[Point: X={0}, Y={1}, Z={2}]", X, Y, Z);
        }
    }
}
