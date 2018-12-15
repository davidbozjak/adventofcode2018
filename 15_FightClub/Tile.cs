using System;
using SantasToolbox;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace _15_FightClub
{
    class Tile : IWorldObject
    {
        public Tile(int x, int y, Fighter fighter)
        {
            this.Position = new Point(x, y);
            this.Fighter = fighter;
        }

        public Fighter Fighter { get; private set; }

        public Point Position { get; }

        public bool IsAdjacent(Tile tile)
        {
            var distance = this.Position.Distance(tile.Position);

            return this.Position != tile.Position && distance == 1;
        }

        public IEnumerable<Tile> GetAdjacentTiles(World world)
        {
            return world.Tiles.Where(IsAdjacent);
        }

        public virtual char CharRepresentation => ' ';

        public virtual bool IsAvaliable => this.Fighter == null;

        public virtual void Occupy(Fighter fighter)
        {
            if (fighter != null && this.Fighter != null)
            {
                throw new Exception("Occupied!");
            }

            this.Fighter = fighter;
        }

        public int Z => 0;
    }

    class Wall : Tile
    {
        public Wall(int x, int y) :
            base(x, y, null)
        {

        }

        public override char CharRepresentation => '#';

        public override bool IsAvaliable => false;

        public override void Occupy(Fighter fighter)
        {
            throw new Exception("Can't move into wall!");
        }
    }
}
