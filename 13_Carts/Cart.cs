using System;
using System.Drawing;

namespace _13_Carts
{
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
}
