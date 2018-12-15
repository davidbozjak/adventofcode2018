using SantasToolbox;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace _15_FightClub
{
    abstract class Fighter : IWorldObject
    {
        public Fighter(int x, int y)
        {
            this.Tile = new Tile(x, y, this);
        }

        public Tile Tile { get; private set; }

        public Point Position => this.Tile.Position;

        public abstract char CharRepresentation { get; }

        public virtual int Z => this.Tile.Z + 1;

        public int HP { get; private set; } = 200;

        public int AttackPower => 3;

        public bool MakeTurn(World world)
        {
            var targets = GetTargets(world);

            if (!targets.Any())
            {
                return false;
            }
            
            if (!GetAdjacentTargets(targets).Any())
            {
                var platforms = GetCombatPlatforms(targets, world);

                if (!platforms.Any())
                {
                    return true;
                }

                var paths = platforms.Select(w => new PathFinder(this.Tile, w, world));

                var chosenPath = paths
                    .Where(w => w.IsReachable)
                    .OrderBy(w => w.NumberOfSteps).FirstOrDefault();

                if (chosenPath != null)
                {
                    this.Move(chosenPath.NextStep);
                }
            }

            Attack(targets);

            return true;
        }
        
        protected abstract IEnumerable<Fighter> GetTargets(World world);

        private IEnumerable<Tile> GetCombatPlatforms(IEnumerable<Fighter> targets, World world)
        {
            return world.Tiles
                .Where(w => w.IsAvaliable)
                .Where(tile => targets.Any(target => tile.IsAdjacent(target.Tile)));
        }

        private void Move(Tile tile)
        {
            if (tile == null)
            {
                throw new Exception("Never expecting moving to null");
            }

            if (!tile.IsAdjacent(this.Tile))
            {
                throw new Exception("Can only move to adjacent tiles");
            }

            tile.Occupy(this);
            this.Tile = tile;
        }

        private void Attack(IEnumerable<Fighter> targets)
        {
            var adjacentTargets = GetAdjacentTargets(targets);

            if (!adjacentTargets.Any())
            {
                return;
            }

            var prioTarget = adjacentTargets.OrderBy(w => w.HP * 10000 + w.Position.Y * 1000 + w.Position.X).First();

            prioTarget.HP -= this.AttackPower;
        }

        private IEnumerable<Fighter> GetAdjacentTargets(IEnumerable<Fighter> targets)
        {
            return targets.Where(w => this.Tile.IsAdjacent(w.Tile));
        }
    }

    class Goblin : Fighter
    {
        public Goblin(int x, int y) :
            base(x, y)
        {

        }

        public override char CharRepresentation => 'G';

        protected override IEnumerable<Fighter> GetTargets(World world)
        {
            return world.Elves;
        }
    }

    class Elf : Fighter
    {
        public Elf(int x, int y) :
            base(x, y)
        {

        }

        public override char CharRepresentation => 'E';

        protected override IEnumerable<Fighter> GetTargets(World world)
        {
            return world.Goblins;
        }
    }
}
