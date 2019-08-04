using SantasToolbox;
using System.Collections.Generic;
using System.Linq;

namespace _17_Water
{
    class World : IWorld
    {
        private readonly List<Tile> tiles;
        public IEnumerable<IWorldObject> WorldObjects => this.tiles.ToList();

        public World(IEnumerable<Tile> initialTiles)
        {
            this.tiles = initialTiles.ToList();
        }
    }
}
