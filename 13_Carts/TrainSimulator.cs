using SantasToolbox;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace _13_Carts
{
    class TrainSimulator
    {
        static void Main(string[] args)
        {
            var initialState = GetInitialState();

            Console.WriteLine();
            Console.ReadKey();
        }

        private static World GetInitialState()
        {
            var rowProvider = new InputProvider<string>("Input.txt", GetString);

            while (rowProvider.MoveNext())
            {
                var row = rowProvider.Current;
                foreach (var c in row)
                {

                }
            }

            return new World();
        }

        private static bool GetString(string value, out string result)
        {
            result = value;
            return !string.IsNullOrWhiteSpace(value);
        }
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
