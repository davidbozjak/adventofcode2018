using System.Drawing;

var input = new StringInputProvider("Input.txt");

var world = new TileWorld(input, true,
    (x, y, c, f) => new TristateTile(x, y, c, f));

var printer = new WorldPrinter();

var tiles = world.WorldObjects.Cast<TristateTile>().ToList(); ;

int minutesToDoPart1 = 10;
int minutesToDoPart2 = 1000000000;

Dictionary<int, int> outputs = new();

for (int minute = 1; minute <= minutesToDoPart2; minute++)
{
    tiles.ForEach(t => t.CalculateNextState());
    tiles.ForEach(t => t.ApplyNextState());

    var numberOfTrees = tiles.Count(w => w.CurrentState == TristateTile.State.Wooded);
    var numberOfLumberyards = tiles.Count(w => w.CurrentState == TristateTile.State.Lumberyard);
    var output = numberOfTrees * numberOfLumberyards;

    if (minute == minutesToDoPart1)
    {
        Console.WriteLine($"Part 1: After {minute} minutes there is {numberOfTrees} wooded tiles and {numberOfLumberyards} lumberyards. Output {numberOfTrees * numberOfLumberyards}");
    }

    if (outputs.ContainsKey(output))
    {
        var lastOccuredMinute = outputs[output];
        var cycleLength = minute - lastOccuredMinute;

        //assuming that any cycles shorter than 10 are a coincidence
        if (cycleLength > 10 && (minutesToDoPart2 - minute) % cycleLength == 0)
        {
            Console.WriteLine($"Matching cycle found, breaking");
            break;
        }
    }

    outputs[output] = minute;
}

var numberOfTreesAtEnd = tiles.Count(w => w.CurrentState == TristateTile.State.Wooded);
var numberOfLumberyardsAtEnd = tiles.Count(w => w.CurrentState == TristateTile.State.Lumberyard);

Console.WriteLine($"Part 2: After {minutesToDoPart2} minutes there is {numberOfTreesAtEnd} wooded tiles and {numberOfLumberyardsAtEnd} lumberyards. Output {numberOfTreesAtEnd * numberOfLumberyardsAtEnd}");

class TristateTile : Tile
{
    public enum State { Open, Wooded, Lumberyard};

    public State CurrentState { get; private set; }

    private State? nextState;

    public override char CharRepresentation => this.CurrentState switch
    {
        State.Open => '.',
        State.Wooded => '|',
        State.Lumberyard => '#',
        _ => throw new Exception()
    }; 

    public TristateTile(int x, int y, char c, Func<Tile, IEnumerable<Tile>> fillNeighboursFunc)
        : base(x, y, true, fillNeighboursFunc)
    {
        this.CurrentState = c switch
        {
            '.' => State.Open,
            '|' => State.Wooded,
            '#' => State.Lumberyard,
            _ => throw new Exception()
        };
    }

    public void CalculateNextState()
    {
        if (this.nextState != null) throw new Exception();

        var numberOfTrees = this.TraversibleNeighbours.Cast<TristateTile>().Count(w => w.CurrentState == State.Wooded);
        var numberOfLumberyards = this.TraversibleNeighbours.Cast<TristateTile>().Count(w => w.CurrentState == State.Lumberyard);

        if (this.CurrentState == State.Open)
        {
            this.nextState = numberOfTrees >= 3 ? State.Wooded : State.Open;
        }
        else if (this.CurrentState == State.Wooded)
        {
            this.nextState = numberOfLumberyards >= 3 ? State.Lumberyard : State.Wooded;
        }
        else if (this.CurrentState == State.Lumberyard)
        {
            this.nextState = (numberOfTrees >= 1 && numberOfLumberyards >= 1) ? State.Lumberyard : State.Open;
        }
        else throw new Exception();

        if (this.nextState == null) throw new Exception();
    }

    public void ApplyNextState()
    {
        if (this.nextState == null) throw new Exception();

        this.CurrentState = this.nextState.Value;
        this.nextState = null;
    }
}