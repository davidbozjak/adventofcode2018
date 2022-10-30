using System.Text.RegularExpressions;

var points = new InputProvider<Point4?>("Input.txt", GetPoint).Where(w => w != null).Cast<Point4>().ToArray();

var pointsRemaining = points.ToList();

var constellations = new List<List<Point4>>();

// initial
SortPointsIntoConstallations(pointsRemaining, constellations);

int initialNoOfConstallations = constellations.Count;

for (int i = 0; i < constellations.Count; i++)
{
    var constellationToRemove = constellations[i];
    constellations.Remove(constellationToRemove);
    pointsRemaining.AddRange(constellationToRemove);

    SortPointsIntoConstallations(pointsRemaining, constellations);

    if (constellations.Count < initialNoOfConstallations)
    {
        initialNoOfConstallations = constellations.Count;
        i = -1;
    }
}

Console.WriteLine($"Part 1: {constellations.Count}");

static void SortPointsIntoConstallations(List<Point4> pointsRemaining, List<List<Point4>> constellations)
{
    List<Point4> lostPoints = new();
    bool foundHomeForAny = false;

    do
    {
        if (lostPoints.Any())
        {
            if (foundHomeForAny)
            {
                pointsRemaining.AddRange(lostPoints);
            }
            else
            {
                var last = lostPoints.Last();
                constellations.Add(new List<Point4>() { last });
                pointsRemaining.AddRange(lostPoints.Where(w => w != last));
            }

            lostPoints.Clear();
            foundHomeForAny = false;
        }

        while (pointsRemaining.Count > 0)
        {
            var point = pointsRemaining.First();
            pointsRemaining.Remove(point);

            List<Point4>? homeConstalation = null;

            foreach (var constellation in constellations)
            {
                foreach (var c_point in constellation)
                {
                    if (c_point.Distance(point) <= 3)
                    {
                        homeConstalation = constellation;
                        break;
                    }
                }
            }

            if (homeConstalation != null)
            {
                foundHomeForAny = true;
                homeConstalation.Add(point);
            }
            else
            {
                lostPoints.Add(point);
            }
        }
    } while (lostPoints.Count > 0);
}

static bool GetPoint(string? input, out Point4? value)
{
    value = null;

    if (input == null) return false;

    Regex numRegex = new(@"-?\d+");

    var numbers = numRegex.Matches(input).Select(w => int.Parse(w.Value)).ToArray();

    if (numbers.Length != 4)
        throw new Exception();

    value = new Point4(numbers[0], numbers[1], numbers[2], numbers[3]);

    return true;
}

record Point4(int X, int Y, int Z, int W)
{
    public int Distance(Point4 other)
    {
        return Math.Abs(this.X - other.X) +
            Math.Abs(this.Y - other.Y) +
            Math.Abs(this.Z - other.Z) +
            Math.Abs(this.W - other.W);
    }
}