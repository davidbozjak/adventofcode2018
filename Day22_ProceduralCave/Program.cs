using System.Drawing;


//debug info:
//var world = new World(510, 10, 10);

//puzzle input:
var world = new World(8787, 10, 725);

world.EnsureWholeSquareIsInitialized();

//var printer = new WorldPrinter();
//printer.Print(world);

Console.WriteLine($"Part 1: Risk level: {world.GetRiskLevel()}");

var start = new Spelunker(world.GetRegionAt(0, 0), Tool.Torch, 0, world.GetRegionAt);
var target = new SpelunkerAtTarget(
    world.GetRegionAt(world.TargetPosition.X, world.TargetPosition.Y), 
    Tool.Torch);   // From instructions:
                   // Finally, once you reach the target,
                   // you need the torch equipped before you can find him in the dark.
                   // The target is always in a rocky region, so if you arrive there with climbing gear equipped,
                   // you will need to spend seven minutes switching to your torch.

var path = AStarPathfinder.FindPath(start, target, 
    w => world.TargetPosition.Distance(w.Position), 
    w => w.GetAllPossibleMoves());

if (path == null) throw new Exception();

Console.WriteLine($"Number of times switching tools before the end: {path.Count(w => w.Cost > 1)}");

var totalCost = path.Sum(w => w.Cost);

Console.WriteLine($"Part 2: {totalCost}");

class SpelunkerAtTarget : Spelunker
{
    public SpelunkerAtTarget(Region region, Tool tool) : 
        base(region, tool, 0, (w1, w2) => throw new Exception())
    {
    }

    new public bool Equals(Spelunker? other)
    {
        if (other is null) return false;
        else
        {
            //If they reached us they are at the end and we treat them as OK!
            return this.Position == other.Position && this.CurrentTool == other.CurrentTool;
        }
    }
}

class Spelunker : INode, IWorldObject, IEquatable<Spelunker>
{
    public int Cost { get; }

    public Region CurrentRegion { get; }

    public Tool CurrentTool { get; }

    public Point Position => this.CurrentRegion.Position;

    public char CharRepresentation => 'X';

    public int Z => 1;

    private readonly Func<int, int, Region> getRegionFunc;

    public Spelunker (Region region, Tool tool, int cost, Func<int, int, Region> getRegionFunc)
    {
        this.CurrentRegion = region;
        this.CurrentTool = tool;
        this.Cost = cost;
        this.getRegionFunc = getRegionFunc;
    }

    public IEnumerable<Spelunker> GetAllPossibleMoves()
    {
        var moves = new List<Spelunker>();

        //left, only if not on the edge
        if (this.Position.X > 0)
        {
            moves.AddRange(GetSpelunkerForPosition(this, this.Position.X - 1, this.Position.Y));
        }

        //up, only if not on the edge
        if (this.Position.Y > 0)
        {
            moves.AddRange(GetSpelunkerForPosition(this, this.Position.X, this.Position.Y - 1));
        }

        moves.AddRange(GetSpelunkerForPosition(this, this.Position.X, this.Position.Y + 1));
        moves.AddRange(GetSpelunkerForPosition(this, this.Position.X + 1, this.Position.Y));

        return moves;

        static IEnumerable<Spelunker> GetSpelunkerForPosition(Spelunker current, int x, int y)
        {
            var nextRegion = current.getRegionFunc(x, y);

            foreach (var switchTool in current.CurrentRegion.SupportedTools.Intersect(nextRegion.SupportedTools))
            {
                int cost = switchTool == current.CurrentTool ? 1 : 8;
                yield return new Spelunker(nextRegion, switchTool, cost, current.getRegionFunc);
            }
        }
    }

    public bool Equals(Spelunker? other)
    {
        if (other is null) return false;
        else if (other is SpelunkerAtTarget spelunkerAtTarget)
        {
            return spelunkerAtTarget.Equals(this);
        }
        else return this.Position.Equals(other.Position) && (this.CurrentTool == other.CurrentTool);
    }

    public override bool Equals(object? obj)
    {
        if (obj == null) return false;
        return Equals(obj as Spelunker);
    }

    public override int GetHashCode()
    {
        return this.Position.GetHashCode() * (1 + (int)Math.Pow(777, this.CurrentTool.GetHashCode() + 1));
    }
}

class Region : IWorldObject
{
    public Point Position { get; }

    public char CharRepresentation => Type switch
    {
        RegionType.Rocky => '.',
        RegionType.Wet => '=',
        RegionType.Narrow => '|',
        _ => throw new Exception()
    };

    public int GeologicIndex => this.cachedGeologicIndex.Value;

    public int ErrosionLevel => this.cachedErrosionLevel.Value;

    public int RiskLevel => this.cachedRegionType.Value switch
    {
        RegionType.Rocky => 0,
        RegionType.Wet => 1,
        RegionType.Narrow => 2,
        _ => throw new Exception()
    };

    public IEnumerable<Tool> SupportedTools => this.Type switch
    {
        RegionType.Rocky => new[] { Tool.ClimbingGear, Tool.Torch },
        RegionType.Wet => new[] { Tool.ClimbingGear, Tool.None },
        RegionType.Narrow => new[] { Tool.Torch, Tool.None},
        _ => throw new Exception()
    };

    public int Z => 1;

    public RegionType Type => cachedRegionType.Value;

    private readonly Cached<RegionType> cachedRegionType;
    private readonly Cached<int> cachedGeologicIndex;
    private readonly Cached<int> cachedErrosionLevel;

    public Region(int x, int y, Func<Region, int> getGeologicIndexFunc, Func<Region, int> getErrosionLevelFunc, Func<Region, RegionType> getRegionTypeFunc)
    {
        this.Position = new Point(x, y);
        this.cachedGeologicIndex = new Cached<int>(() => getGeologicIndexFunc(this));
        this.cachedErrosionLevel = new Cached<int>(() => getErrosionLevelFunc(this));
        this.cachedRegionType = new Cached<RegionType>(() => getRegionTypeFunc(this));
    }
}

class World : IWorld
{
    public IEnumerable<IWorldObject> WorldObjects => this.regionFactory.AllCreatedInstances;

    private readonly UniqueFactory<(int, int), Region> regionFactory;

    public int Depth { get; }

    public Point TargetPosition { get; }

    public World(int depth, int targetX, int targetY)
    {
        this.Depth = depth;
        this.TargetPosition = new Point(targetX, targetY);
        regionFactory = new UniqueFactory<(int, int), Region>(w => new Region(w.Item1, w.Item2,
            GetGeologicIndexForPosition,
            GetErrosionLevel,
            GetRegionType));
    }

    public Region GetRegionAt(int x, int y)
    {
        return this.regionFactory.GetOrCreateInstance((x, y));
    }

    public int GetRiskLevel()
    {
        return this.regionFactory.AllCreatedInstances.Sum(w => w.RiskLevel);
    }

    public void EnsureWholeSquareIsInitialized()
    {
        for (int x = 0; x <= TargetPosition.X; x++)
        {
            for (int y = 0; y <= TargetPosition.Y; y++)
            {
                var c = GetRegionAt(x, y).CharRepresentation;
            }
        }
    }

    int GetGeologicIndexForPosition(Region region)
    {
        if (region.Position.X == 0 && region.Position.Y == 0) return 0;
        else if (region.Position == this.TargetPosition) return 0;
        else if (region.Position.Y == 0) return region.Position.X * 16807;
        else if (region.Position.X == 0) return region.Position.Y * 48271;
        else
        {
            var left = GetRegionAt(region.Position.X - 1, region.Position.Y);
            var top = GetRegionAt(region.Position.X, region.Position.Y - 1);

            return left.ErrosionLevel * top.ErrosionLevel;
        }
    }

    int GetErrosionLevel(Region region) =>
        (region.GeologicIndex + this.Depth) % 20183;

    RegionType GetRegionType(Region region) =>
        (region.ErrosionLevel % 3) switch
        {
            0 => RegionType.Rocky,
            1 => RegionType.Wet,
            2 => RegionType.Narrow,
            _ => throw new Exception()
        };
}

public enum RegionType { Rocky, Wet, Narrow};
public enum Tool { Torch, ClimbingGear, None };