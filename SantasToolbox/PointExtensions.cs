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
    }
}
