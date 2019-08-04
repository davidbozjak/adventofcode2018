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

        public IEnumerable<IWorldObject> WorldObjects => this.tiles.ToList();

        public World(IEnumerable<Tile> initialTiles)
        {
            this.tiles = initialTiles.ToList();
        }

        public bool MakeStep(Action<IWorldObject> worldObserver)
        {
            var water = this.waterFactory.GetWater();

            Tile tile = GetOrCreateTileAt(this.springPosition);

            if (tile.Water != null)
            {
                return false;
            }

            tile.Water = water;

            // try to move as far down as you can
            while (true)
            {
                worldObserver(tile);
                
                var tileBelow = GetOrCreateTileAt(tile.Position.Down());

                if (!tileBelow.IsClay)
                {
                    if (tileBelow.Water == null)
                    {
                        TransferWater(tileBelow, tile);
                        tile = tileBelow;
                    }
                    else
                    {
                        if (!tileBelow.Water.IsStuck)
                        {
                            var couldPush = PushWaterFromTile(tileBelow);

                            if (couldPush)
                            {
                                TransferWater(tileBelow, tile);
                                return true;
                            }
                            else
                            {
                                tileBelow.Water.IsStuck = true;
                                return false;
                            }
                        }
                    }
                }
                else
                {
                    if (tile.Water == water)
                    {
                        return true;
                    }

                    return false;
                }
            }

            return true;
        }

        private bool PushWaterFromTile(Tile tile)
        {
            var tileBelow = GetOrCreateTileAt(tile.Position.Down());
            if (clearTile(tileBelow, tile)) return true;
            
            var tileLeft = GetOrCreateTileAt(tile.Position.Left());
            if (clearTile(tileLeft, tile)) return true;

            var tileRight = GetOrCreateTileAt(tile.Position.Right());
            if (clearTile(tileRight, tile)) return true;

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
                    if (PushWaterFromTile(tTarget))
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
