using System.Drawing;
using System.Text.RegularExpressions;

var tilesFactory = new UniqueFactory<(int x, int y), Tile>(w => new Tile(w.x, w.y));

var clayLines = new InputProvider<ClayLine?>("Input.txt", GetClayLine).Where(w => w != null).Cast<ClayLine>().ToArray();

foreach (var line in clayLines)
{
    CreateTilesForLine(line);
}

var absoluteMaxY = tilesFactory.AllCreatedInstances.Max(w => w.Position.Y);
int absoluteMinY = tilesFactory.AllCreatedInstances.Min(w => w.Position.Y);

var masterSpring = new Spring(500, 0);

var allSprings = new List<Spring>() { masterSpring };
var springsToProcess = new PriorityQueue<Spring, int>();
springsToProcess.Enqueue(masterSpring, masterSpring.Position.Y);

while (springsToProcess.Count > 0)
{
    var spring = springsToProcess.Dequeue();

    PrintAroundSpring(spring);

    var reservoir = GetReservoirForSpring(spring, tilesFactory.GetOrCreateInstance, absoluteMaxY);

    if (reservoir == null) continue;

    foreach (var newSpring in reservoir.OverflowSprings)
    {
        if (!allSprings.Any(w => w.Position == newSpring.Position))
        {
            allSprings.Add(newSpring);
            springsToProcess.Enqueue(newSpring, newSpring.Position.Y);
        }
    }
}

var worldObjects = new List<IWorldObject>();
worldObjects.AddRange(tilesFactory.AllCreatedInstances);
worldObjects.AddRange(allSprings);

var world = new SimpleWorld<IWorldObject>(worldObjects);
var printer = new WorldPrinter();

printer.PrintToFile(world, "Output_World.txt");

var reachableTiles = tilesFactory.AllCreatedInstances
    .Where(w => w.Position.Y >= absoluteMinY && w.Position.Y <= absoluteMaxY);

Console.WriteLine($"Part 1: {reachableTiles.Count(w => w.State == GroundType.StandingWater || w.State == GroundType.WetSand)}");
Console.WriteLine($"Part 2: {reachableTiles.Count(w => w.State == GroundType.StandingWater)}");

void PrintAroundSpring(Spring spring)
{
    var worldObjects = new List<IWorldObject>();
    worldObjects.AddRange(tilesFactory.AllCreatedInstances);
    worldObjects.AddRange(allSprings);

    var world = new SimpleWorld<IWorldObject>(worldObjects);
    var printer = new WorldPrinter(frameSize: 10);

    printer.Print(world, spring);
}

static Reservoir? GetReservoirForSpring(Spring spring, Func<(int, int), Tile> tileFactory, int maxY)
{
    for (int y = spring.Position.Y, x = spring.Position.X; y <= maxY; y++)
    {
        var tile = tileFactory((x, y));
        
        if (tile.State == GroundType.Clay)
        {
            return new Reservoir(y, spring, tileFactory);
        }
        else
        {
            tile.State = GroundType.WetSand;
        }
    }

    return null;
}

void CreateTilesForLine(ClayLine line)
{
    Func<int, Tile> factory;
    int min, max;
    if (line is VerticalClayLine vl)
    {
        factory = new Func<int, Tile>(w => tilesFactory.GetOrCreateInstance((vl.X, w)));
        min = vl.MinY;
        max = vl.MaxY;
    }
    else if (line is HorizontalClayLine hl)
    {
        factory = new Func<int, Tile>(w => tilesFactory.GetOrCreateInstance((w, hl.Y)));
        min = hl.MinX;
        max = hl.MaxX;
    }
    else throw new Exception();

    for (int i = min; i <= max; i++)
    {
        var tile = factory(i);
        tile.State = GroundType.Clay;
    }
}

static bool GetClayLine(string? input, out ClayLine? value)
{
    value = null;

    if (input == null) return false;

    Regex numRegex = new(@"\d+");

    var numbers = numRegex.Matches(input).Select(w => int.Parse(w.Value)).ToArray();

    value = input[0] == 'y' ?
        new HorizontalClayLine(numbers[0], numbers[1], numbers[2]) :
        new VerticalClayLine(numbers[0], numbers[1], numbers[2]);

    return true;
}

enum GroundType { Sand, WetSand, StandingWater, Clay }

class Tile : IWorldObject
{
    public Point Position { get; }

    public char CharRepresentation => this.State switch
    {
        GroundType.Sand => '.',
        GroundType.WetSand => '|',
        GroundType.StandingWater => '~',
        GroundType.Clay => '#',
        _ => throw new Exception()
    };

    public GroundType State { get; set; }

    public int Z => 1;

    public Tile(int x, int y)
    {
        this.Position = new Point(x, y);
    }
}

class Spring : IWorldObject
{
    public Point Position { get; }

    public char CharRepresentation => '+';

    public int Z => 2;

    public Spring? FeederSpring { get; init; }

    public Reservoir? FeederReservoir { get; init; }

    public Spring(int x, int y)
    {
        this.Position = new Point(x, y);
    }
}

class Reservoir
{
    public IEnumerable<Spring> OverflowSprings { get; }

    public Spring SourceSpring { get; }

    public Reservoir(int bottom, Spring spring, Func<(int, int), Tile> tileFactory)
    {
        this.SourceSpring = spring;

        var springs = new List<Spring>();

        int top = 0;
        for (int y = bottom - 1; springs.Count == 0; y--)
        {
            springs.AddIfNotNull(TryCreateSpring(y, x => x - 1));
            springs.AddIfNotNull(TryCreateSpring(y, x => x + 1));
            top = y;
        }

        this.OverflowSprings = springs;

        SetRowToStandingWater(top, x => x - 1);
        SetRowToStandingWater(top, x => x + 1);

        void SetRowToStandingWater(int y, Func<int, int> incFunc)
        {
            for (int x = incFunc(spring.Position.X); true; x = incFunc(x))
            {
                var tile = tileFactory((x, top));

                if (tile.State != GroundType.StandingWater)
                    break;

                tile.State = GroundType.WetSand;
            }

            tileFactory((spring.Position.X, top)).State = GroundType.WetSand;
        }

        Spring? TryCreateSpring(int y, Func<int, int> incFunc)
        {
            for (int x = spring.Position.X; true; x = incFunc(x))
            {
                var tile = tileFactory((x, y));
                var tileBelow = tileFactory((x, y + 1));

                if (tile.State == GroundType.Clay)
                {
                    break;
                }
                else if (tileBelow.State != GroundType.Clay && tileBelow.State != GroundType.StandingWater)
                {
                    return new Spring(x, y) { FeederReservoir = this, FeederSpring = spring };
                }

                tile.State = GroundType.StandingWater;
            }

            return null;
        }
    }
}

abstract record ClayLine();

record VerticalClayLine(int X, int MinY, int MaxY) : ClayLine;

record HorizontalClayLine(int Y, int MinX, int MaxX) : ClayLine;