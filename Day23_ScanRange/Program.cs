using System.Text.RegularExpressions;

var scanners = new InputProvider<Scanner?>("Input.txt", GetScanner).Where(w => w != null).Cast<Scanner>().ToList();

var scannersInRange = new Dictionary<Scanner, List<Scanner>>();

foreach (var scanner in scanners)
{
    var scannersInRangeOfScanner = scanners.Where(w => scanner.IsInRange(w)).ToList();
    scannersInRange[scanner] = scannersInRangeOfScanner;
}

Console.WriteLine($"Part 1: {scannersInRange[scannersInRange.Keys.OrderByDescending(w => w.Range).First()].Count}");

var scannersInRangeOfPoint = new Dictionary<Point3, int>();
int maxScoreForPoint = int.MinValue;

//determine bounding box of whole observable universe:
int minX = scanners.Select(w => w.Position.X - w.Range).Min();
int maxX = scanners.Select(w => w.Position.X + w.Range).Max();
int minY = scanners.Select(w => w.Position.Y - w.Range).Min();
int maxY = scanners.Select(w => w.Position.Y + w.Range).Max();
int minZ = scanners.Select(w => w.Position.Z - w.Range).Min();
int maxZ = scanners.Select(w => w.Position.Z + w.Range).Max();

var fullBoundingBox = new Cube(minX, maxX, minY, maxY, minZ, maxZ, null);
var center = new Point3(0, 0, 0);

if (scanners.Count(w => IsCubeWithinRangeOfScanner(fullBoundingBox, w)) != scanners.Count)
    throw new Exception("Something wrong with either bounding-box or cube-in-range-calculation");

var result = DivideAndConquer(fullBoundingBox, scanners);

if (result == null)
    throw new Exception();

foreach (var point in result.Value.Item1.GetPoints())
{
    var count = GetAndCacheScannersThatSeePoint(point);
}

int maxScannersInRangeOfPoint = scannersInRangeOfPoint.Values.Max();
var points = scannersInRangeOfPoint.Where(w => w.Value == maxScannersInRangeOfPoint).OrderBy(w => center.Distance(w.Key));

var closestPoint = points.First().Key;

// basic test if it's truly correct
var closestPointHits = points.First().Value;
var pointCloserToCenter = new Point3(closestPoint.X + (closestPoint.X > 0 ? 1 : -1), closestPoint.Y + (closestPoint.Y > 0 ? 1 : -1), closestPoint.Z + (closestPoint.Z > 0 ? 1 : -1)); //temp, should do a real orientation here

if (center.Distance(closestPoint) >= center.Distance(pointCloserToCenter))
    throw new Exception();

var hitsForCloser = GetAndCacheScannersThatSeePoint(pointCloserToCenter);

if (hitsForCloser >= closestPointHits)
    throw new Exception("You did not find truly the closest");

Console.WriteLine($"Part 2: { center.Distance(closestPoint)}");

static (Cube, int)? DivideAndConquer(Cube cube, IEnumerable<Scanner> scanners)
{
    if (cube.LengthX == 1 || cube.LengthY == 1 || cube.LengthZ == 1)
        return (cube, scanners.Count());

    var dictByScore = new Dictionary<int, PriorityQueue<(Cube, List<Scanner>), int>>();
    foreach(var subCube in cube.GetOctree())
    {
        var scannersInRangeOfCube = scanners.Where(w => IsCubeWithinRangeOfScanner(subCube, w)).ToList();

        if (scannersInRangeOfCube.Count > 0)
        {
            if (!dictByScore.ContainsKey(scannersInRangeOfCube.Count))
                dictByScore[scannersInRangeOfCube.Count] = new PriorityQueue<(Cube, List<Scanner>), int>();

            dictByScore[scannersInRangeOfCube.Count].Enqueue((subCube, scannersInRangeOfCube), new Point3(0, 0, 0).Distance(subCube.Center));
        }
    }

    var queueWithBestPrioScore = dictByScore.OrderByDescending(w => w.Key).First().Value;
    var bestAlternative = queueWithBestPrioScore.Dequeue();
    return DivideAndConquer(bestAlternative.Item1, bestAlternative.Item2);
}

static bool IsCubeWithinRangeOfScanner(Cube cube, Scanner scanner)
{
    if (cube.IsPointInCube(scanner.Position))
        return true;

    int distanceX = Math.Min(Math.Abs(cube.MinX - scanner.Position.X), Math.Abs(cube.MaxX - scanner.Position.X));
    int distanceY = Math.Min(Math.Abs(cube.MinY - scanner.Position.Y), Math.Abs(cube.MaxY - scanner.Position.Y));
    int distanceZ = Math.Min(Math.Abs(cube.MinZ - scanner.Position.Z), Math.Abs(cube.MaxZ - scanner.Position.Z));

    return scanner.Range >= (distanceX + distanceY + distanceZ);
}

int GetAndCacheScannersThatSeePoint(Point3 point)
{
    if (scannersInRangeOfPoint.ContainsKey(point))
        return scannersInRangeOfPoint[point];

    var countScanners = scanners.Count(w => w.IsPointInRange(point));
    scannersInRangeOfPoint[point] = countScanners;

    if (countScanners > maxScoreForPoint)
    {
        maxScoreForPoint = countScanners;
    }

    return countScanners;
}

static bool GetScanner(string? input, out Scanner? value)
{
    value = null;

    if (input == null) return false;

    Regex numRegex = new(@"-?\d+");

    var numbers = numRegex.Matches(input).Select(w => int.Parse(w.Value)).ToArray();

    if (numbers.Length != 4) throw new Exception();

    value = new Scanner(new Point3(numbers[0], numbers[1], numbers[2]), numbers[3]);

    return true;
}

record Scanner (Point3 Position, int Range)
{
    public bool IsPointInRange(Point3 point)
        => IsPointInRange(point.X, point.Y, point.Z);

    public bool IsPointInRange(int x, int y, int z)
        => this.Position.Distance(x, y, z) <= this.Range;

    public bool IsInRange(Scanner other)
        => this.IsPointInRange(other.Position);
}

record Point3(int X, int Y, int Z)
{
    public int Distance(Point3 other)
    {
        return Distance(other.X, other.Y, other.Z);
    }

    public int Distance(int x, int y, int z)
    {
        return Math.Abs(this.X - x) + Math.Abs(this.Y - y) + Math.Abs(this.Z - z);
    }
}

record Cube(int MinX, int MaxX, int MinY, int MaxY, int MinZ, int MaxZ, Cube? Parent)
{
    public int LengthX => this.MaxX - this.MinX;
    public int LengthY => this.MaxY - this.MinY;
    public int LengthZ => this.MaxZ - this.MinZ;

    public int CenterX => this.MinX + this.LengthX / 2;
    public int CenterY => this.MinY + this.LengthY / 2;
    public int CenterZ => this.MinZ + this.LengthZ / 2;

    public Point3 Center => new(CenterX, CenterY, CenterZ);

    public bool IsPointInCube(Point3 point)
    {
        return point.X >= MinX && point.X <= MaxX &&
            point.Y >= MinY && point.Y <= MaxY &&
            point.Z >= MinZ && point.Z <= MaxZ;
    }

    public IEnumerable<Cube> GetOctree()
    {
        var xBoundries = new[] { (this.MinX, this.CenterX), (this.CenterX, this.MaxX) };
        var yBoundries = new[] { (this.MinY, this.CenterY), (this.CenterY, this.MaxY) };
        var zBoundries = new[] { (this.MinZ, this.CenterZ), (this.CenterZ, this.MaxZ) };

        foreach (var x in xBoundries)
        {
            foreach (var y in yBoundries)
            {
                foreach (var z in zBoundries)
                {
                    yield return new Cube(x.Item1, x.Item2, y.Item1, y.Item2, z.Item1, z.Item2, this);
                }
            }
        }
    }

    public IEnumerable<Point3> GetCornerPoints()
    {
        yield return new Point3(this.MaxX, this.MaxY, this.MaxZ);    //position.x + width/2, position.y + height/2, position.z + depth/2
        yield return new Point3(this.MaxX, this.MaxY, this.MinZ);    //position.x + width/2, position.y + height/2, position.z - depth/2
        yield return new Point3(this.MaxX, this.MinY, this.MaxZ);    //position.x + width/2, position.y - height/2, position.z + depth/2
        yield return new Point3(this.MaxX, this.MinY, this.MinZ);    //position.x + width/2, position.y - height/2, position.z - depth/2
        yield return new Point3(this.MinX, this.MaxY, this.MaxZ);    //position.x - width/2, position.y + height/2, position.z + depth/2
        yield return new Point3(this.MinX, this.MaxY, this.MinZ);    //position.x - width/2, position.y + height/2, position.z - depth/2
        yield return new Point3(this.MinX, this.MinY, this.MaxZ);    //position.x - width/2, position.y - height/2, position.z + depth/2
        yield return new Point3(this.MinX, this.MinY, this.MinZ);    //position.x - width/2, position.y - height/2, position.z - depth/2
    }

    public IEnumerable<Point3> GetPoints()
    {
        for (int x = MinX; x <= MaxX; x++)
        {
            for (int y = MinY; y <= MaxY; y++)
            {
                for (int z = MinZ; z <= MaxZ; z++)
                {
                    yield return new Point3(x, y, z);
                }
            }
        }
    }
}