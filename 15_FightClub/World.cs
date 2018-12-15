using SantasToolbox;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace _15_FightClub
{
    class World : IWorld
    {
        public World(IEnumerable<Tile> tiles, IEnumerable<Fighter> fighters)
        {
            this.Tiles = tiles.ToList();
            this.Goblins = fighters.OfType<Goblin>().ToList();
            this.Elves = fighters.OfType<Elf>().ToList();
        }

        public IList<Tile> Tiles { get; }

        public IList<Goblin> Goblins { get; }

        public IList<Elf> Elves { get; }

        public IEnumerable<IWorldObject> WorldObjects
        {
            get
            {
                var list = new List<IWorldObject>();
                list.AddRange(this.Tiles);
                list.AddRange(this.Goblins);
                list.AddRange(this.Elves);
                return list;
            }
        }
    }
    
    class Tile : IWorldObject
    {
        public Tile(int x, int y, Fighter fighter = null)
        {
            this.Position = new Point(x, y);
            this.Fighter = fighter;
        }

        public Fighter Fighter { get; }

        public Point Position { get; }

        public virtual char CharRepresentation => ' ';

        public int Z => 0;
    }

    class Wall : Tile
    {
        public Wall(int x, int y) : 
            base(x, y, null)
        {

        }

        public override char CharRepresentation => '#';
    }

    abstract class Fighter : IWorldObject
    {
        public Fighter(int x, int y)
        {
            this.Tile = new Tile(x, y, this);
        }

        public Tile Tile { get; }

        public Point Position => this.Tile.Position;

        public abstract char CharRepresentation { get; }

        public virtual int Z => this.Tile.Z + 1;
    }

    class Goblin : Fighter
    {
        public Goblin(int x, int y) :
            base (x, y)
        {

        }

        public override char CharRepresentation => 'G';
    }

    class Elf : Fighter
    {
        public Elf(int x, int y) :
            base(x, y)
        {

        }

        public override char CharRepresentation => 'E';
    }
}
