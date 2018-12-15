using SantasToolbox;
using System.Collections.Generic;
using System.Linq;

namespace _15_FightClub
{
    class World : IWorld
    {
        private readonly List<Tile> tiles;
        private readonly List<Fighter> fighters;

        public World(IEnumerable<Tile> tiles, IEnumerable<Fighter> fighters)
        {
            this.tiles = tiles.ToList();
            this.fighters = fighters.ToList();

            this.Tiles = this.tiles.AsReadOnly();
            this.Fighters = this.fighters.AsReadOnly();
        }

        public IList<Tile> Tiles { get; }

        public IList<Fighter> Fighters { get; }

        public IReadOnlyList<Goblin> Goblins => this.Fighters.OfType<Goblin>().ToList();

        public IReadOnlyList<Elf> Elves => this.Fighters.OfType<Elf>().ToList();

        public IEnumerable<IWorldObject> WorldObjects
        {
            get
            {
                var list = new List<IWorldObject>();
                list.AddRange(this.Tiles);
                list.AddRange(this.Fighters);
                return list;
            }
        }

        public void MakeRound()
        {
            var movesLeft = this.Fighters.OrderBy(w => w.Position.Y * 1000 + w.Position.Y).ToList();

            while (movesLeft.Count > 0)
            {
                var fighter = movesLeft[0];
                movesLeft.Remove(fighter);

                fighter.MakeTurn();

                var deadUnits = this.Fighters.Where(w => w.HP <= 0).ToList();

                foreach (var dead in deadUnits)
                {
                    this.Fighters.Remove(dead);
                    movesLeft.Remove(dead);
                }
            }
        }
    }
}
