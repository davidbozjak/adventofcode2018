﻿using SantasToolbox;
using System.Drawing;

namespace _17_Water
{
    class Tile : IWorldObject
    {
        public Point Position { get; }

        public char CharRepresentation
        {
            get
            {
                if (this.IsClay) return '#';
                else if (this.Water != null) return '~';
                else if (this.HasBeenWet) return '|';
                else return '.';
            }
        }

        public int Z { get; } = 0;

        private IWater water = null;
        public bool HasBeenWet { get; private set; }

        public bool IsClay { get; }

        public IWater Water
        {
            get => this.water;
            set
            {
                this.water = value;
                this.HasBeenWet = this.HasBeenWet || value != null;
            }
        }

        public Tile(Point position, bool isClay = false)
        {
            this.Position = position;
            this.IsClay = isClay;
        }
    }
}
