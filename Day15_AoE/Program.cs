using System.Drawing;

Console.WriteLine("Part 1");
RunsWithoutAnyDeaths(3, false);

Console.WriteLine("Part 2");
for (int elfAttack = 4; ; elfAttack++)
{
    if (RunsWithoutAnyDeaths(elfAttack, true))
        break;
}

bool RunsWithoutAnyDeaths(int elvesAttackPower, bool demandNoElfDeaths)
{
    var inputLines = new StringInputProvider("Input.txt").ToList();

    var fighters = new List<Fighter>();

    var world = new TileWorld(inputLines, false, (x, y, c, func) =>
    {
        var tile = new Tile(x, y, c != '#', func);
        if (c == 'E')
        {
            fighters.Add(new Fighter(tile, FigherGroup.Elves));
        }
        else if (c == 'G')
        {
            fighters.Add(new Fighter(tile, FigherGroup.Goblins));
        }
        return tile;
    });

    var elves = fighters.Where(w => w.Group == FigherGroup.Elves).ToList();
    foreach (var elf in elves)
    {
        elf.AttackPower = elvesAttackPower;
    }

    var countOfElvesAtStart = fighters.Count(w => w.Group == FigherGroup.Elves);

    int numberOfFullRounds = RunCombat(fighters, world);

    var countOfElvesAtEnd = fighters.Count(w => w.Group == FigherGroup.Elves);

    if (!demandNoElfDeaths || countOfElvesAtStart == countOfElvesAtEnd)
    {
        PrintCurrentWrold(fighters, world);
        Console.WriteLine($"Result: {numberOfFullRounds * fighters.Sum(w => w.HitPoints)}");
        Console.WriteLine($"Combat ended in round {numberOfFullRounds}");
        Console.WriteLine($"Sum of remaining hit points: {fighters.Sum(w => w.HitPoints)}");

        return true;
    }

    return false;
}

int RunCombat(List<Fighter> fighters, TileWorld world)
{
    for (int roundCount = 0; ; roundCount++)
    {
        var queue = new PriorityQueue<Fighter, int>();
        queue.EnqueueRange(fighters.Select(w => (w, w.Position.ReadingOrder())));

        while (queue.Count > 0)
        {
            var fighter = queue.Dequeue();

            if (!fighters.Any(w => w.Group != fighter.Group))
            {
                if (fighter.HitPoints <= 0)
                    throw new Exception();

                return roundCount;
            }

            if (fighter.HitPoints <= 0)
            {
                if (fighters.Remove(fighter))
                {
                    throw new Exception($"Expecting fighters to always be cleaned up by the {nameof(Fighter.MakeTurn)} method");
                }
            }
            else
            {
                fighter.MakeTurn(fighters, world);
            }
        }

        //PrintCurrentWrold(fighters, world);
        //Console.WriteLine($"End of round {roundCount}");
        //Console.ReadKey();
    }
}

void PrintCurrentWrold(List<Fighter> fighters, TileWorld world)
{
    var printer = new WorldPrinter();

    var allWorldObjects = new List<IWorldObject>();
    allWorldObjects.AddRange(world.WorldObjects);
    allWorldObjects.AddRange(fighters);

    var simpleWorld = new SimpleWorld<IWorldObject>(allWorldObjects);

    printer.Print(simpleWorld);

    Console.WriteLine();
    //Console.WriteLine("Remaining fighters");

    //foreach (var fighter in fighters.OrderBy(w => w.Position.ReadingOrder()))
    //{
    //    Console.WriteLine($"{fighter.CharRepresentation}({fighter.HitPoints}) - {fighter.Position}");
    //}
}

enum FigherGroup { Elves, Goblins };

[System.Diagnostics.DebuggerDisplay("{CharRepresentation}({HitPoints}) - {Position}")]
class Fighter : IWorldObject
{
    public Point Position => this.CurrentTile.Position;

    public Tile CurrentTile { get; private set; }

    public char CharRepresentation => this.Group switch
    {
        FigherGroup.Goblins => 'G',
        FigherGroup.Elves => 'E',
        _ => throw new Exception()
    };

    public int Z => 2;

    public FigherGroup Group { get; }

    public int AttackPower { get; set; } = 3;

    public int HitPoints { get; private set; } = 200;

    public Fighter(Tile initialTile, FigherGroup group)
    {
        this.CurrentTile = initialTile;
        this.Group = group;
    }

    public void MakeTurn(List<Fighter> fighersAlive, TileWorld world)
    {
        if (this.HitPoints <= 0)
            throw new Exception("Expected to be removed already");

        var opponents = fighersAlive.Where(w => w.Group != this.Group).ToList();

        if (!opponents.Any(w => w.Position.IsNeighbour(this.Position)))
        {
            var moveToPosition = FindPlaceToMove(this, opponents, (x, y) =>
            {
                var tile = world.GetTileAt(x, y);
                if (!tile.IsTraversable) return false;
                else return !fighersAlive.Select(w => w.CurrentTile.Position).Any(w => w.X == x && w.Y == y);
            });

            if (moveToPosition != null)
            {
                var moveToTile = world.GetTileAt(moveToPosition.Value.X, moveToPosition.Value.Y);

                if (!moveToTile.Position.IsNeighbour(this.Position))
                    throw new Exception("Can only move to neighbours");

                if (!IsTileEmpty(moveToTile))
                    throw new Exception("Tile is expected to be empty");

                this.CurrentTile = moveToTile;
            }
        }

        //attack
        var target = opponents.Where(w => w.Position.IsNeighbour(this.Position))
                .OrderBy(w => w.HitPoints)
                .ThenBy(w => w.Position.ReadingOrder())
                .FirstOrDefault();

        if (target != null)
        {
            target.HitPoints -= this.AttackPower;

            if (target.HitPoints <= 0)
            {
                fighersAlive.Remove(target);
            }
        }

        bool IsTileEmpty(Tile tile)
            => !fighersAlive.Select(w => w.CurrentTile).Contains(tile);
    }

    private Point? FindPlaceToMove(Fighter u, List<Fighter> targets, Func<int, int, bool> isTileEmptyFunc)
    {
        (int dx, int dy)[] s_neis = { (0, -1), (-1, 0), (1, 0), (0, 1) };

        HashSet<(int x, int y)> inRange = new HashSet<(int x, int y)>();
        foreach (var target in targets)
        {
            foreach ((int dx, int dy) in s_neis)
            {
                (int nx, int ny) = (target.Position.X + dx, target.Position.Y + dy);
                if (isTileEmptyFunc(nx, ny))
                    inRange.Add((nx, ny));
            }
        }

        Queue<(int x, int y)> queue = new Queue<(int x, int y)>();
        Dictionary<(int x, int y), (int px, int py)> prevs = new Dictionary<(int x, int y), (int px, int py)>();
        queue.Enqueue((u.Position.X, u.Position.Y));
        prevs.Add((u.Position.X, u.Position.Y), (-1, -1));
        while (queue.Count > 0)
        {
            (int x, int y) = queue.Dequeue();
            foreach ((int dx, int dy) in s_neis)
            {
                (int x, int y) nei = (x + dx, y + dy);
                if (prevs.ContainsKey(nei) || !isTileEmptyFunc(nei.x, nei.y))
                    continue;

                queue.Enqueue(nei);
                prevs.Add(nei, (x, y));
            }
        }

        List<(int x, int y)>? getPath(int destX, int destY)
        {
            if (!prevs.ContainsKey((destX, destY)))
                return null;
            List<(int x, int y)> path = new List<(int x, int y)>();
            (int x, int y) = (destX, destY);
            while (x != u.Position.X || y != u.Position.Y)
            {
                path.Add((x, y));
                (x, y) = prevs[(x, y)];
            }

            path.Reverse();
            return path;
        }

        List<(int tx, int ty, List<(int x, int y)>? path)> paths =
            inRange
            .Select(t => (t.x, t.y, path: getPath(t.x, t.y)))
            .Where(t => t.path != null)
            .OrderBy(t => t.path.Count)
            .ThenBy(t => t.y)
            .ThenBy(t => t.x)
            .ToList();

        List<(int x, int y)>? bestPath = paths.FirstOrDefault().path;
        if (bestPath != null)
            return new Point(bestPath[0].x, bestPath[0].y);
        else return null;

    }
}