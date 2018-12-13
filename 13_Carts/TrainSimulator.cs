using System;
using System.Collections.Generic;
using System.Drawing;

namespace _13_Carts
{
    class Program
    {
        static void Main(string[] args)
        {
            var initialState = GetInitialState();

            Console.WriteLine();
            Console.ReadKey();
        }

        private static World GetInitialState()
        {

        }

        class Track
        {
            public Point Position { get; }

            public ICollection<Track> NextTrack { get; }
        }

        class Cart
        {
            public Track Position { get; }

            public void Move()
            {

            }
        }

        class World
        {
            public World()
            {

            }

            public ICollection<Track> Tracks { get; }

            public ICollection<Cart> Carts { get; }

            public void MakeStep()
            {

            }
        }
    }
}
