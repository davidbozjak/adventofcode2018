using System.Drawing;

var input = new StringInputProvider("Input.txt").First()[1..^1];

//var input = "ENWWW(NEEE|SSE(EE|N))";
//var input = "WSSEESWWWNW(S|NENNEEEENN(ESSSSW(NWSW|SSEN)|WSWWN(E|WWS(E|SS))))";

var world = new TileWorld();
var startTile = world.AddKnownRoom(0, 0);

WalkThroughString(input, startTile, world);

var maxDistance = int.MinValue;
var distances = new Dictionary<Tile, int>();

foreach (var room in world.WorldObjects.Cast<Tile>())
{
    var path = AStarPathfinder.FindPath(startTile, room, t => t.Cost, t => t.KnownNeighbours);

    distances.Add(room, path.Count - 1);
}

Console.WriteLine($"Part 1: {distances.Values.Max()}");
Console.WriteLine($"Part 2: {distances.Values.Count(w => w >= 1000)}");

static void WalkThroughString(string str, Tile currentTile, TileWorld world)
{
    for (int i = 0; i < str.Length; i++)
    {
        char c = str[i];

        if (c == '(')
        {
            // find ) on same level
            int end = -1;
            int startOfOption = i + 1;
            List<string> options = new();

            for (int j = i + 1, level = 0; j < str.Length; j++)
            {
                if (str[j] == ')')
                {
                    if (level == 0)
                    {
                        options.Add(str[startOfOption..j]);
                        end = j;
                        break;
                    }
                    else
                    {
                        level--;
                    }
                }

                if (level == 0 && str[j] == '|')
                {
                    options.Add(str[startOfOption..j]);
                    startOfOption = j + 1;
                }

                if (str[j] == '(')
                {
                    level++;
                }
            }
            if (end == -1) throw new Exception();

            foreach (var option in options)
            {
                WalkThroughString(option, currentTile, world);
            }
            
            i = end;
        }
        else
        {
            int x = currentTile.Position.X;
            int y = currentTile.Position.Y;

            if (c == 'N') y -= 1;
            else if (c == 'S') y += 1;
            else if (c == 'W') x -= 1;
            else if (c == 'E') x += 1;
            else throw new Exception();

            var t = world.AddKnownRoom(x, y);
            currentTile.AddKnownNeighbour(t);
            currentTile = t;
        }
    }
}

class Tile : IWorldObject, INode, IEquatable<Tile>
{
    public Point Position { get; }

    public virtual char CharRepresentation => '#';

    public int Z => 0;

    private readonly HashSet<Tile> knownNeighbours = new();

    public IEnumerable<Tile> KnownNeighbours => this.knownNeighbours;

    public int Cost => 1;

    public Tile(int x, int y)
    {
        Position = new Point(x, y);
    }

    public void AddKnownNeighbour(Tile t)
    {
        knownNeighbours.Add(t);
    }

    public bool Equals(Tile? other)
    {
        if (other == null) return false;
        return base.Equals(other);
    }
}

class TileWorld : IWorld
{
    private readonly UniqueFactory<(int, int), Tile> Rooms;

    public IEnumerable<IWorldObject> WorldObjects => this.Rooms.AllCreatedInstances;

    public TileWorld()
    {
        this.Rooms = new UniqueFactory<(int, int), Tile>(w => new Tile(w.Item1, w.Item2));
    }

    public Tile AddKnownRoom(int x, int y)
    {
        return this.Rooms.GetOrCreateInstance((x, y));
    }
}