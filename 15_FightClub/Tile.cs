using SantasToolbox;
using System.Drawing;

namespace _15_FightClub
{
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
}
