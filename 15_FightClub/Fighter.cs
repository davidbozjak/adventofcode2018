using SantasToolbox;
using System.Drawing;

namespace _15_FightClub
{
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

        public int HP { get; } = 200;

        public int AttackPower => 3;

        public void MakeTurn()
        {

        }
    }

    class Goblin : Fighter
    {
        public Goblin(int x, int y) :
            base(x, y)
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
