using System.Drawing;

namespace _6_Areas
{
    class IdentifiablePoint
    {
        private Point point;
        private static int idCounter = 0;

        public IdentifiablePoint(Point point)
        {
            this.point = point;
            this.Id = idCounter++;
        }

        public int Id { get; }

        public int X => this.point.X;

        public int Y => this.point.Y;
    }
}
