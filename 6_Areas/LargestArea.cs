using SantasToolbox;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

namespace _6_Areas
{
    class LargestArea
    {
        static void Main(string[] args)
        {
            var points = GetPoints();

            int minX = points.Select(w => w.X).Min();
            int maxX = points.Select(w => w.X).Max();
            int minY = points.Select(w => w.Y).Min();
            int maxY = points.Select(w => w.Y).Max();

            var grid = new int[maxX - minX, maxY - minY];
            var areaSizes = new Dictionary<int, int>(points.Select(w => new KeyValuePair<int, int>(w.Id, 0)));
            areaSizes[0] = (maxX - minX) * (maxY - minY);

            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    var closestPoint = GetClosestPoint(x, y, points);
                    areaSizes[grid[x - minX, y - minY]]--;
                    grid[x - minX, y - minY] = closestPoint.Id;
                    areaSizes[closestPoint.Id]++;
                }
            }


            var largestArea = areaSizes.OrderByDescending(w => w.Value).First();
            Console.WriteLine($"Part 1: Largest area: {largestArea.Key} with size: {largestArea.Value}");

            Console.WriteLine("");
            Console.WriteLine("Done");
            Console.ReadKey();
        }

        private static IdentifiablePoint GetClosestPoint(int x, int y, List<IdentifiablePoint> points)
        {
            int minDistance = int.MaxValue;
            IdentifiablePoint closestPoint = null;

            foreach(var point in points)
            {
                var distance = GetDistance(x, y, point.X, point.Y);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestPoint = point;
                }
            }

            return closestPoint;

            int GetDistance(int x1, int y1, int x2, int y2)
            {
                return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
            }
        }

        private static List<IdentifiablePoint> GetPoints()
        {
            var coordinatesProvider = new InputProvider<IdentifiablePoint>("Input.txt", GetPoint);

            var points = new List<IdentifiablePoint>();

            while (coordinatesProvider.MoveNext())
            {
                points.Add(coordinatesProvider.Current);
            }

            return points;
        }

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

        private static bool GetPoint(string value, out IdentifiablePoint result)
        {
            var regex = new Regex(@"\d+");

            try
            {
                var matches = regex.Matches(value);
                result = new IdentifiablePoint(new Point(int.Parse(matches[0].Value), int.Parse(matches[1].Value)));
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }
    }
}
