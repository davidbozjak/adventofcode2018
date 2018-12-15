using SantasToolbox;
using System;
using System.Drawing;

namespace _13_Carts
{
    class Track : IWorldObject
    {
        public Track(int x, int y, TrackDirection direction)
        {
            this.Position = new Point(x, y);
            this.TrackDirection = direction;
        }

        public Point Position { get; }

        public int Z => 0;

        public TrackDirection TrackDirection { get; }

        public ConnectedTracks ConnectedTracs { get; } = new ConnectedTracks();

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
}
