using SantasToolbox;
using System.Collections.Generic;

namespace _15_FightClub
{
    class WorldFactory
    {
        public World GetInitialState()
        {
            var rowProvider = new InputProvider<string>("Input.txt", GetLine);

            var tiles = new List<Tile>();
            var fighters = new List<Fighter>();

            for (int y = 0; rowProvider.MoveNext(); y++)
            {
                for (int x = 0; x < rowProvider.Current.Length; x++)
                {
                    var c = rowProvider.Current[x];

                    Tile tile = null;
                    Fighter fighter = null;

                    switch (c)
                    {
                        case '#':
                            tile = new Wall(x, y);
                            break;
                        case 'G':
                            fighter = new Goblin(x, y);
                            break;
                        case 'E':
                            fighter = new Elf(x, y);
                            break;
                        default:
                            tile = new Tile(x, y);
                            break;
                    }

                    fighters.AddIfNotNull(fighter);
                    tiles.AddIfNotNull(fighter?.Tile ?? tile);
                }
            }

            return new World(tiles, fighters);
        }

        private static bool GetLine(string value, out string result)
        {
            result = value;
            return !string.IsNullOrWhiteSpace(value);
        }
    }
}
