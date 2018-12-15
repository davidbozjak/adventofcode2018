﻿using SantasToolbox;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace _13_Carts
{
    class TrainSimulator
    {
        static void Main(string[] args)
        {
            var world = GetInitialState();
            
            for (int tick = 0; world.Carts.Count > 1; tick++)
            {
                world.MakeStep();
            }

            var lastCart = world.Carts[0];

            Console.WriteLine();
            Console.WriteLine($"Last cart at position ({lastCart.Position.X},{lastCart.Position.Y})");
            Console.ReadKey();
        }

        private static void Print(World world)
        {
            int maxX = world.Tracks.Max(w => w.Position.X);
            int maxY = world.Tracks.Max(w => w.Position.Y);

            Print(world, 0, maxX, 0, maxY);
        }

        private static void Print(World world, Cart cart)
        {
            Print(world, cart.Position.X - 5, cart.Position.X + 5, cart.Position.Y - 5, cart.Position.Y + 5);
        }

        private static void Print(World world, int minX, int maxX, int minY, int maxY)
        {
            Console.Clear();
            
            for (int y = minY; y <= maxY; y++)
            {
                var row = new StringBuilder(new string(Enumerable.Repeat(' ', (maxX - minX) + 1).ToArray()));

                foreach (var track in world.Tracks.Where(w => w.Position.Y == y && w.Position.X >= minX && w.Position.X <= maxX))
                {
                    row[track.Position.X - minX] = track.CharRepresentation;
                }

                foreach (var cart in world.Carts.Where(w => w.Position.Y == y && w.Position.X >= minX && w.Position.X <= maxX))
                {
                    row[cart.Position.X - minX] = cart.CharRepresentation;
                }
                if (!string.IsNullOrWhiteSpace(row.ToString()))
                {
                    Console.WriteLine(row);
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

        public char CharRepresentation
        {
            get
            {
                switch (this.TrackDirection)
                {
                    case TrackDirection.Horizontal:
                        return '-';
                    case TrackDirection.Intersection:
                        return '+';
                    case TrackDirection.Vertical:
                        return '|';
                    case TrackDirection.LeftTurn:
                        return '\\';
                    case TrackDirection.RightTurn:
                        return '/';
                    default:
                        throw new Exception();
                }
            }
        }
    }

    class Cart
    {
        private static int idCounter = 1;

        public Cart(Track track, Direction direction)
        {
            this.Track = track;
            this.Direction = direction;
            this.Id = idCounter++;
        }

        public int Id { get; }

        public Track Track { get; private set; }

        public Direction Direction { get; private set; }

        public IntersectionDecision IntersectionDecision { get; private set; } = IntersectionDecision.TurnLeft;

        public Point Position => this.Track.Position;

        public char CharRepresentation
        {
            get
            {
                switch (this.Direction)
                {
                    case Direction.Down:
                        return 'v';
                    case Direction.Up:
                        return '^';
                    case Direction.Left:
                        return '<';
                    case Direction.Right:
                        return '>';
                    default:
                        throw new Exception();
                }
            }
        }

        public void Move()
        {
            if (this.Track.TrackDirection == TrackDirection.LeftTurn)
            {
                this.Direction = TurnLeft();
            }
            else if (this.Track.TrackDirection == TrackDirection.RightTurn)
            {
                this.Direction = TurnRight();
            }
            else if (this.Track.TrackDirection == TrackDirection.Intersection)
            {
                switch (this.IntersectionDecision)
                {
                    case IntersectionDecision.TurnLeft:
                        this.Direction = IntersectionLeft();
                        this.IntersectionDecision = IntersectionDecision.Straight;
                        break;
                    case IntersectionDecision.Straight:
                        this.IntersectionDecision = IntersectionDecision.TurnRight;
                        break;
                    case IntersectionDecision.TurnRight:
                        this.Direction = IntersectionRight();
                        this.IntersectionDecision = IntersectionDecision.TurnLeft;
                        break;
                }
            }
            
            this.Track = this.Track.ConnectedTracs[this.Direction] ?? throw new Exception();

            Direction TurnLeft()
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

            Direction TurnRight()
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

            Direction IntersectionLeft()
            {
                switch (this.Direction)
                {
                    case Direction.Up:
                        return Direction.Left;
                    case Direction.Down:
                        return Direction.Right;
                    case Direction.Left:
                        return Direction.Down;
                    case Direction.Right:
                        return Direction.Up;
                    default:
                        throw new ArgumentException();
                }
            }

            Direction IntersectionRight()
            {
                switch (this.Direction)
                {
                    case Direction.Up:
                        return Direction.Right;
                    case Direction.Down:
                        return Direction.Left;
                    case Direction.Left:
                        return Direction.Up;
                    case Direction.Right:
                        return Direction.Down;
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

        public IList<Cart> Carts { get; }

        public void MakeStep()
        {
            IEnumerable<Cart> CartsInReadingOrder() => this.Carts.OrderBy(w => w.Position.Y * 1000 + w.Position.X);
            var cartsToMove = CartsInReadingOrder().ToList();

            while(cartsToMove.Count > 0)
            {
                var cart = cartsToMove[0];
                cartsToMove.Remove(cart);

                cart.Move();

                foreach(var collidingCart in CartsInReadingOrder())
                {
                    if (collidingCart == cart)
                    {
                        continue;
                    }

                    if (collidingCart.Position.Y > cart.Position.Y)
                    {
                        break;
                    }

                    if (cart.Position.X == collidingCart.Position.X &&
                        cart.Position.Y == collidingCart.Position.Y)
                    {
                        Console.WriteLine($"Collission at ({cart.Position.X}, {cart.Position.Y})");

                        this.Carts.Remove(cart);
                        this.Carts.Remove(collidingCart);
                        cartsToMove.Remove(collidingCart);
                    }
                }
            }
        }
    }
}
