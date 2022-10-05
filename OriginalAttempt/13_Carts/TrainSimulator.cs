using SantasToolbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _13_Carts
{
    class TrainSimulator
    {
        static void Main(string[] args)
        {
            var world = GetInitialState();
            var worldPrinter = new WorldPrinter();

            for (int tick = 0; world.Carts.Count > 1; tick++)
            {
                world.MakeStep();
                worldPrinter.Print(world, world.Carts[0]);
                Console.ReadKey();
            }

            var lastCart = world.Carts[0];

            Console.WriteLine();
            Console.WriteLine($"Last cart at position ({lastCart.Position.X},{lastCart.Position.Y})");
            Console.ReadKey();
        }
        
        private static World GetInitialState()
        {
            var rowProvider = new InputProvider<string>("Input.txt", GetString);

            var tracks = new List<Track>();
            var carts = new List<Cart>();

            for (int y = 0; rowProvider.MoveNext(); y++)
            {
                var row = rowProvider.Current;
                for (int x = 0; x < row.Length; x++)
                {
                    var c = row[x];

                    switch (c)
                    {
                        case ' ':
                            continue;
                        case '|':
                            tracks.Add(new Track(x, y, TrackDirection.Vertical));
                            break;
                        case '-':
                            tracks.Add(new Track(x, y, TrackDirection.Horizontal));
                            break;
                        case '^':
                        case 'v':
                            var vTrack = new Track(x, y, TrackDirection.Vertical);
                            tracks.Add(vTrack);
                            carts.Add(new Cart(vTrack, c == '^' ? Direction.Up : Direction.Down));
                            break;
                        case '<':
                        case '>':
                            var hTrack = new Track(x, y, TrackDirection.Horizontal);
                            tracks.Add(hTrack);
                            carts.Add(new Cart(hTrack, c == '<' ? Direction.Left : Direction.Right));
                            break;
                        case '+':
                            var intersection = new Track(x, y, TrackDirection.Intersection);
                            tracks.Add(intersection);
                            break;
                        case '/':
                            var trTrack = new Track(x, y, TrackDirection.RightTurn);
                            tracks.Add(trTrack);
                            break;
                        case '\\':
                            var tlTrack = new Track(x, y, TrackDirection.LeftTurn);
                            tracks.Add(tlTrack);
                            break;
                    }
                }
            }

            // "wire up" the tracks
            
            foreach (var track in tracks)
            {
                if (track.TrackDirection == TrackDirection.Vertical || track.TrackDirection == TrackDirection.Intersection)
                {
                    track.ConnectedTracs[Direction.Up] = tracks.First(t => t.Position.X == track.Position.X && t.Position.Y == track.Position.Y - 1);
                    track.ConnectedTracs[Direction.Down] = tracks.First(t => t.Position.X == track.Position.X && t.Position.Y == track.Position.Y + 1);
                }

                if (track.TrackDirection == TrackDirection.Horizontal || track.TrackDirection == TrackDirection.Intersection)
                {
                    track.ConnectedTracs[Direction.Left] = tracks.First(t => t.Position.Y == track.Position.Y && t.Position.X == track.Position.X - 1);
                    track.ConnectedTracs[Direction.Right] = tracks.First(t => t.Position.Y == track.Position.Y && t.Position.X == track.Position.X + 1);
                }

                if (track.TrackDirection == TrackDirection.RightTurn || track.TrackDirection == TrackDirection.LeftTurn)
                {
                    track.ConnectedTracs[Direction.Up]    = tracks.FirstOrDefault(t => t.Position.X == track.Position.X && t.Position.Y == track.Position.Y - 1);
                    track.ConnectedTracs[Direction.Down]  = tracks.FirstOrDefault(t => t.Position.X == track.Position.X && t.Position.Y == track.Position.Y + 1);
                    track.ConnectedTracs[Direction.Left]  = tracks.FirstOrDefault(t => t.Position.Y == track.Position.Y && t.Position.X == track.Position.X - 1);
                    track.ConnectedTracs[Direction.Right] = tracks.FirstOrDefault(t => t.Position.Y == track.Position.Y && t.Position.X == track.Position.X + 1);
                }
            }

            return new World(tracks, carts);
        }

        private static bool GetString(string value, out string result)
        {
            result = value;
            return !string.IsNullOrWhiteSpace(value);
        }
    }
}
