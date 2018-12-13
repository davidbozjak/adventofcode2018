using SantasToolbox;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace _13_Carts
{
    class TrainSimulator
    {
        static void Main(string[] args)
        {
            var world = GetInitialState();

            for (int tick = 0; ; tick++)
            {
                world.MakeStep();

                for(int i = 0; i < world.Carts.Count; i++)
                {
                    for (int j = i + 1; j < world.Carts.Count; j++)
                    {
                        if (world.Carts[i].Position.X == world.Carts[j].Position.X &&
                            world.Carts[i].Position.Y == world.Carts[j].Position.Y)
                        {
                            Console.WriteLine($"Collision at: ({world.Carts[i].Position.X},{world.Carts[i].Position.Y})");
                            Console.ReadKey();
                            return;
                        }
                    }
                }
            }
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

                if (track.TrackDirection == TrackDirection.RightTurn)
                {
                    var right = tracks.FirstOrDefault(t => t.Position.Y == track.Position.Y && t.Position.X == track.Position.X + 1);

                    if (right != null)
                    {
                        track.ConnectedTracs[Direction.Right] = track.ConnectedTracs[Direction.Left] = right;
                        track.ConnectedTracs[Direction.Down] = track.ConnectedTracs[Direction.Up] = tracks.First(t => t.Position.X == track.Position.X && t.Position.Y == track.Position.Y + 1);
                    }
                    else
                    {
                        track.ConnectedTracs[Direction.Left] = track.ConnectedTracs[Direction.Right] = tracks.First(t => t.Position.Y == track.Position.Y && t.Position.X == track.Position.X - 1);
                        track.ConnectedTracs[Direction.Up] = track.ConnectedTracs[Direction.Down] = tracks.First(t => t.Position.X == track.Position.X && t.Position.Y == track.Position.Y - 1);
                    }
                }

                if (track.TrackDirection == TrackDirection.LeftTurn)
                {
                    var left = tracks.FirstOrDefault(t => t.Position.Y == track.Position.Y && t.Position.X == track.Position.X - 1);

                    if (left != null)
                    {
                        track.ConnectedTracs[Direction.Left] = track.ConnectedTracs[Direction.Right] = left;
                        track.ConnectedTracs[Direction.Down] = track.ConnectedTracs[Direction.Up] = tracks.First(t => t.Position.X == track.Position.X && t.Position.Y == track.Position.Y + 1);
                    }
                    else
                    {
                        track.ConnectedTracs[Direction.Right] = track.ConnectedTracs[Direction.Left] = tracks.First(t => t.Position.Y == track.Position.Y && t.Position.X == track.Position.X + 1);
                        track.ConnectedTracs[Direction.Up] = track.ConnectedTracs[Direction.Down] = tracks.First(t => t.Position.X == track.Position.X && t.Position.Y == track.Position.Y - 1);
                    }
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

    public enum TrackDirection { Vertical, Horizontal, LeftTurn, RightTurn, Intersection }
    public enum IntersectionDecision { TurnLeft, Straight, TurnRight };
    public enum Direction { Up, Down, Left, Right };

    class ConnectedTracs
    {
        private readonly Track[] tracs = new Track[4];
        
        public Track this[Direction index]
        {
            get => this.tracs[(int)index];
            set => this.tracs[(int)index] = value;
        }
    }

    class Track
    {
        public Track(int x, int y, TrackDirection direction)
        {
            this.Position = new Point(x, y);
            this.TrackDirection = direction;
        }

        public Point Position { get; }

        public TrackDirection TrackDirection { get; }

        public ConnectedTracs ConnectedTracs { get; } = new ConnectedTracs();
    }

    class Cart
    {
        public Cart(Track track, Direction direction)
        {
            this.Track = track;
            this.Direction = direction;
        }

        public Track Track { get; private set; }

        public Direction Direction { get; private set; }

        public IntersectionDecision IntersectionDecision { get; private set; } = IntersectionDecision.TurnLeft;

        public Point Position => this.Track.Position;

        public void Move()
        {
            if (this.Track.TrackDirection == TrackDirection.LeftTurn)
            {
                this.Direction = LeftTurn();
            }
            else if (this.Track.TrackDirection == TrackDirection.RightTurn)
            {
                this.Direction = RightTurn();
            }
            else if (this.Track.TrackDirection == TrackDirection.Intersection)
            {
                switch (this.IntersectionDecision)
                {
                    case IntersectionDecision.TurnLeft:
                        this.Direction = LeftTurn();
                        this.IntersectionDecision = IntersectionDecision.Straight;
                        break;
                    case IntersectionDecision.Straight:
                        this.IntersectionDecision = IntersectionDecision.TurnRight;
                        break;
                    case IntersectionDecision.TurnRight:
                        this.Direction = RightTurn();
                        this.IntersectionDecision = IntersectionDecision.TurnLeft;
                        break;
                }
            }
            
            this.Track = this.Track.ConnectedTracs[this.Direction] ?? throw new Exception();

            Direction LeftTurn()
            {
                switch (this.Direction)
                {
                    case Direction.Up:
                        return Direction.Left;
                    case Direction.Down:
                        return Direction.Right;
                    case Direction.Left:
                        return Direction.Up;
                    case Direction.Right:
                        return Direction.Down;
                    default:
                        throw new ArgumentException();
                }
            }

            Direction RightTurn()
            {
                switch (this.Direction)
                {
                    case Direction.Up:
                        return Direction.Right;
                    case Direction.Down:
                        return Direction.Left;
                    case Direction.Left:
                        return Direction.Down;
                    case Direction.Right:
                        return Direction.Up;
                    default:
                        throw new ArgumentException();
                }
            }
        }
    }

    class World
    {
        public World(IEnumerable<Track> tracks, IEnumerable<Cart> carts)
        {
            this.Tracks = tracks.ToList();
            this.Carts = carts.ToList();
        }

        public IReadOnlyList<Track> Tracks { get; }

        public IReadOnlyList<Cart> Carts { get; }

        public void MakeStep()
        {
            var movingOrder = this.Carts.OrderBy(w => w.Position.Y * 1000 + w.Position.X);

            foreach (var cart in movingOrder)
            {
                cart.Move();
            }
        }
    }
}
