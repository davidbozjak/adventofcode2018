using System;
using System.Drawing;

namespace SantasToolbox
{
    public static class PointExtensions
    {
        public static int Distance(this Point point, Point other)
        {
            return Math.Abs(point.X - other.X) + Math.Abs(point.Y - other.Y);
        }

        public static int ReadingOrder(this Point point)
        {
            return point.Y * 1000 + point.X;
        }
    }
}
