using SantasToolbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _3_Squares
{
    class OverlappingSquaresFinder
    {
        static void Main(string[] args)
        {
            var squares = GetSquares();

            int fieldX = squares.Min(w => w.Position.X);
            int fieldY = squares.Min(w => w.Position.Y);
            int fieldW = squares.Max(w => w.Position.X + w.Size.Width);
            int fieldH = squares.Max(w => w.Position.Y + w.Size.Height);

            int pointsInTwoOrMorePatches = 0;

            for (int x = fieldX; x < fieldW; x++)
            {
                for (int y = fieldY; y < fieldH; y++)
                {
                    var countPatches = squares.Where(w => w.ContainsPoint(x, y)).Count();

                    if (countPatches >= 2)
                    {
                        pointsInTwoOrMorePatches++;
                    }
                }
            }

            Console.WriteLine($"Points in patches: {pointsInTwoOrMorePatches}");
            Console.ReadKey();
        }

        private static List<Square> GetSquares()
        {
            var idProvider = new InputProvider<Square>("Input.txt", SquareProvider);

            var squares = new List<Square>();

            while (idProvider.MoveNext())
            {
                squares.Add(idProvider.Current);
            }

            return squares;
        }

        private static bool SquareProvider(string value, out Square result)
        {
            try
            {
                result = new Square(value);
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
