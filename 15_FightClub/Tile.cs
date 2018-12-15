using System;
using SantasToolbox;
using System.Drawing;

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

        public virtual char CharRepresentation => ' ';

        public virtual bool IsAvaliable => this.Fighter == null;

        public virtual void Occupy(Fighter fighter)
        {
            if (this.Fighter != null)
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
