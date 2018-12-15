using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SantasToolbox
{
    public interface IWorldObject
    {
        Point Position { get; }

        char CharRepresentation { get; }

        int Z { get; }
    }

    public interface IWorld
    {
        IList<IWorldObject> WorldObjects { get; }
    }

    public class WorldPrinter
    {
        public void Print(IWorld world)
        {
            int maxX = world.WorldObjects.Max(w => w.Position.X);
            int maxY = world.WorldObjects.Max(w => w.Position.Y);

            Print(world, 0, maxX, 0, maxY);
        }

        public void Print(IWorld world, IWorldObject cart)
        {
            Print(world, cart.Position.X - 5, cart.Position.X + 5, cart.Position.Y - 5, cart.Position.Y + 5);
        }

        public void Print(IWorld world, int minX, int maxX, int minY, int maxY)
        {
            Console.Clear();

            for (int y = minY; y <= maxY; y++)
            {
                var row = new StringBuilder(new string(Enumerable.Repeat(' ', (maxX - minX) + 1).ToArray()));

                foreach (var track in world.WorldObjects.Where(w => w.Position.Y == y && w.Position.X >= minX && w.Position.X <= maxX).OrderBy(w => w.Z))
                {
                    row[track.Position.X - minX] = track.CharRepresentation;
                }
                
                if (!string.IsNullOrWhiteSpace(row.ToString()))
                {
                    Console.WriteLine(row);
                }
            }
        }
    }
}
