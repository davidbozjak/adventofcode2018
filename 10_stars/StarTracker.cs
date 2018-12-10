using SantasToolbox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace _10_stars
{
    class StarTracker
    {
        static void Main(string[] args)
        {
            var observationProvider = new InputProvider<Star>("Input.txt", GetStar);
            var stars = new List<Star>();

            while (observationProvider.MoveNext())
            {
                stars.Add(observationProvider.Current);
            }

            for (int step = 0; true; step++)
            {
                Console.Clear();
                Console.WriteLine($"Step {step}:\n");

                var minX = stars.Min(w => w.Position.X);
                var maxX = stars.Max(w => w.Position.X);
                VisualizeSky(new EnumerableReader<Star>(stars.OrderBy(w => w.Position.Y).ToArray()), minX, maxX);

                Console.ReadKey();
                stars.ForEach(star => star.MakeStep());
            }

            Console.WriteLine();
            Console.WriteLine($"Done. Num of stars: {stars.Count}");
            Console.ReadKey();
        }

        private static void VisualizeSky(EnumerableReader<Star> stars, int minX, int maxX)
        {
            for (Star star = stars.TakeFirst(), nextStar = null; star != null; star = nextStar)
            {
                var builder = new StringBuilder(new string(Enumerable.Repeat('.', 1 + maxX - minX).ToArray()));
                int y = star.Position.Y;
                builder[star.Position.X - minX] = '#';

                for (nextStar = stars.TakeFirst(); nextStar != null && nextStar.Position.Y == star.Position.Y; nextStar = stars.TakeFirst())
                {
                    builder[nextStar.Position.X - minX] = '#';
                }

                Console.WriteLine(builder);

                if (nextStar != null)
                {
                    for (int i = star.Position.Y + 1; i < nextStar.Position.Y; i++)
                    {
                        builder = new StringBuilder(new string(Enumerable.Repeat('.', 1 + maxX - minX).ToArray()));
                        Console.WriteLine(builder);
                    }
                }
            }
        }

        private static bool GetStar(string value, out Star result)
        {
            try
            {
                result = new Star(value);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }
    }

    class Star
    {
        public Star(string observation)
        {
            var numRegex = new Regex(@"-?\d+");
            var matches = numRegex.Matches(observation).Select(w => int.Parse(w.Value)).ToArray();

            this.Position = new Point(matches[0], matches[1]);
            this.Velocity = new Point(matches[2], matches[3]);
        }

        public Point Position { get; private set; }

        public Point Velocity { get; }

        public void MakeStep()
        {
            this.Position = new Point(this.Position.X + this.Velocity.X, this.Position.Y + this.Velocity.Y);
        }
    }
}
