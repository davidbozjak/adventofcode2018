using SantasToolbox;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace _17_Water
{
    class World : IWorld
    {
        private readonly List<Tile> tiles;
        private readonly WaterFactory waterFactory = new WaterFactory();
        private readonly Point springPosition = new Point(500, 0);
        private readonly int worldCutoff;

        public IEnumerable<IWorldObject> WorldObjects => this.tiles.ToList();

        public int NumberOfWetTiles => this.tiles.Where(w => w.HasBeenWet).Count();

        public World(IEnumerable<Tile> initialTiles)
        {
            this.tiles = initialTiles.ToList();
            this.worldCutoff = this.tiles.Max(w => w.Position.Y) + 2;
        }

        public bool MakeStep(Action<IWorldObject> worldObserver)
        {
            var water = this.waterFactory.GetWater();

            bool hasFoundWallLeft = false;

            for (Tile tile = GetOrCreateTileAt(this.springPosition); tile.Position.Y < this.worldCutoff; )
            {
                tile.Water = water;

                worldObserver(tile);

                var tileBelow = GetOrCreateTileAt(tile.Position.Down());

                if (!tileBelow.IsClay && tileBelow.Water == null)
                {
                    TransferWater(tileBelow, tile);
                    tile = tileBelow;
                    hasFoundWallLeft = false;
                    continue;
                }

                if (tileBelow.Water != null)
                {
                    if (PushWaterFromTile(tileBelow))
                    {
                        TransferWater(tileBelow, tile);
                        return true;
                    }
                    else
                    {
                        tileBelow.Water.IsStuck = true;
                    }
                }

                if (WaterCanSettleOn(tile))
                {
                    return true;
                }

                if (!hasFoundWallLeft)
                {
                    var tileLeft = GetOrCreateTileAt(tile.Position.Left());
                    if (tileLeft.IsClay)
                    {
                        hasFoundWallLeft = true;
                    }
                    else
                    {
                        TransferWater(tileLeft, tile);
                        tile = tileLeft;
                        continue;
                    }
                }

                var tileRight = GetOrCreateTileAt(tile.Position.Right());
                TransferWater(tileRight, tile);
                tile = tileRight;
            }

            return false;
        }

        private bool WaterCanSettleOn(Tile tile)
        {
            if (tile.IsClay) throw new Exception("not expecting to try to settle in clay, invalid flow");

            // watter can settle if there if on clay or water below is stuck and there is clay on left and right with levels below filled
            var tileBelow = GetOrCreateTileAt(tile.Position.Down());
            if (!tileBelow.IsClay && !tileBelow.Water?.IsStuck == true) return false;

            var clayOnLeft = this.tiles
                .Where(w => w.IsClay && w.Position.Y == tile.Position.Y && w.Position.X < tile.Position.X)
                .OrderByDescending(w => w.Position.X)
                .FirstOrDefault();
            var clayOnRight = this.tiles
                .Where(w => w.IsClay && w.Position.Y == tile.Position.Y && w.Position.X > tile.Position.X)
                .OrderBy(w => w.Position.X)
                .FirstOrDefault();

            if (clayOnLeft == null || clayOnRight == null)
            {
                return false;
            }

            for (int x = clayOnLeft.Position.X + 1; x < clayOnRight.Position.X; x++)
            {
                var tileToCheck = GetOrCreateTileAt(new Point(x, tile.Position.Y + 1));

                if (!tileToCheck.IsClay && (tileToCheck.Water == null || tileToCheck.Water.IsStuck == false))
                {
                    return false;
                }
            }

            return true;
        }

        private bool PushWaterFromTile(Tile tile, List<Tile> visitedTiles = null)
        {
            visitedTiles = visitedTiles ?? new List<Tile>();

            var tileBelow = GetOrCreateTileAt(tile.Position.Down());

            if (!visitedTiles.Contains(tileBelow))
            {
                visitedTiles.Add(tileBelow);
                if (clearTile(tileBelow, tile)) return true;
            }

            var tileLeft = GetOrCreateTileAt(tile.Position.Left());

            if (!visitedTiles.Contains(tileLeft))
            {
                visitedTiles.Add(tileLeft);
                if (clearTile(tileLeft, tile)) return true;
            }

            var tileRight = GetOrCreateTileAt(tile.Position.Right());

            if (!visitedTiles.Contains(tileRight))
            {
                visitedTiles.Add(tileRight);
                if (clearTile(tileRight, tile)) return true;
            }
            return false;

            bool clearTile(Tile tTarget, Tile tSource)
            {
                if (tTarget.IsClay) return false;
                if (tTarget.Water == null)
                {
                    TransferWater(tTarget, tSource);
                    return true;
                }

                if (!tTarget.Water.IsStuck)
                {
                    if (PushWaterFromTile(tTarget, visitedTiles))
                    {
                        if (tTarget.Water != null) throw new Exception("expecting the tile below to now be empty");

                        TransferWater(tTarget, tSource);
                        return true;
                    }
                    else
                    {
                        tTarget.Water.IsStuck = true;
                        return false;
                    }
                }

                return false;
            }
        }

        private static void TransferWater(Tile tTarget, Tile tSource)
        {
            if (!tTarget.Position.IsNeighbour(tSource.Position))
                throw new Exception("Can only transfer water between neighbour tiles");

            tTarget.Water = tSource.Water;
            tSource.Water = null;
        }

        private Tile GetOrCreateTileAt(Point position)
        {
            var tile = this.tiles.FirstOrDefault(w => w.Position == position);

            if (tile == null)
            {
                tile = new Tile(position);
                this.tiles.Add(tile);
            }

            return tile;
        }
    }
}
