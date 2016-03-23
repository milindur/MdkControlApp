namespace MDKControl.Core.Models
{
    public class Point
    {
        public Point(float x, float y)
        {
            X = x;
            Y = y;
        }

        public float X { get; private set; }
        public float Y { get; private set; }

        public override string ToString()
        {
            return string.Format("[Point: X={0}, Y={1}]", X, Y);
        }
    }
}
