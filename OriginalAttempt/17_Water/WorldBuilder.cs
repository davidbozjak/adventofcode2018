using SantasToolbox;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

namespace _17_Water
{
    class WorldFactory
    {
        public World GetWorld()
        {
            var rowProvider = new InputProvider<string>("Input.txt", GetLine);

            var tiles = new List<Tile>();

            foreach (var row in rowProvider)
            {
                tiles.AddRange(GetTilesFromInput(row));
            }

            return new World(tiles);
        }

        private IEnumerable<Tile> GetTilesFromInput(string input)
        {
            if (input[0] == 'x') return GetClayColumn(input);
            else
            {
                if (input[0] != 'y') throw new Exception("unexpected input");
                return GetClayRow(input);
            }
        }

        private IEnumerable<Tile> GetClayRow(string input)
        {
            (int y, int startX, int endX) = GetCoords(input);

            var tiles = new List<Tile>();
            for (int x = startX; x <= endX; x++)
            {
                var position = new Point(x, y);
                tiles.Add(new Tile(position, true));
            }

            return tiles;
        }

        private IEnumerable<Tile> GetClayColumn(string input)
        {
            (int x, int startY, int endY) = GetCoords(input);

            var tiles = new List<Tile>();
            for (int y = startY; y <= endY; y++)
            {
                var position = new Point(x, y);
                tiles.Add(new Tile(position, true));
            }

            return tiles;
        }

        private (int value1, int value2, int value3) GetCoords(string input)
        {
            var numberRegex = new Regex(@"\d+");

            var numbers = numberRegex.Matches(input).Select(w => int.Parse(w.Value)).ToArray();

            return (numbers[0], numbers[1], numbers[2]);
        }

        private bool GetLine(string input, out string result)
        {
            result = input;
            return !string.IsNullOrWhiteSpace(input);
        }
    }
}
