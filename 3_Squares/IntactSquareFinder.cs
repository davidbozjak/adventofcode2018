using SantasToolbox;
using System;
using System.Collections.Generic;

namespace _3_Squares
{
    class IntactSquareFinder
    {
        static void Main(string[] args)
        {
            var squares = GetSquares();
            var squaresWithNoOverlap = new List<Square>();

            foreach (var square in squares)
            {
                if (!square.OverlapsWithAny(squares))
                {
                    squaresWithNoOverlap.Add(square);
                }
            }

            Console.WriteLine($"Non overlaping patches: {squaresWithNoOverlap.Count}");
            Console.WriteLine($"First overlaping patche id: {squaresWithNoOverlap[0].Id}");
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
