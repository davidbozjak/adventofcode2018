﻿using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

namespace _3_Squares
{
    class Square
    {
        public int Id { get; }

        public Point Position { get; }

        public Size Size { get; }

        public Square(string serializedSquare)
        {
            (this.Id, this.Position, this.Size) = this.Deserialize(serializedSquare);
        }

        private (int Id, Point position, Size size) Deserialize(string serializedSquare)
        {
            var numberRegex = new Regex(@"\d+");

            var numbers = numberRegex.Matches(serializedSquare).Select(w => int.Parse(w.Value)).ToArray();

            var id = numbers[0];
            var position = new Point(numbers[1], numbers[2]);
            var size = new Size(numbers[3], numbers[4]);

            return (id, position, size);
        }

        public bool ContainsPoint(int x, int y)
        {
            bool containsX = x >= this.Position.X && x < (this.Position.X + this.Size.Width);
            bool containsY = y >= this.Position.Y && y < (this.Position.Y + this.Size.Height);

            return containsX && containsY;
        }
    }
}
