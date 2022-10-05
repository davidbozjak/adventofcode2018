using System.Text.RegularExpressions;

var parser = new MultiLineParser<SnapshotBuilder>(() => new SnapshotBuilder(), (builder, line) => builder.AddLine(line));
var input = new StringInputProvider("Input1.txt") { EndAtEmptyLine = false };

var snapshots = parser.AddRange(input).Select(w => w.Build()).ToList();

var possibleInstructions = snapshots.Select(w => SnapshotInterpreter.GetAllOpcodesThatMatch(w)).ToList();

Console.WriteLine($"Part 1: {possibleInstructions.Count(w => w.Count() > 2)}");

static class SnapshotInterpreter
{
    public enum Opcode
    {
        addr,
        addi,
        mulr,
        muli,
        banr,
        bani,
        borr,
        bori,
        setr,
        seti,
        gtir,
        gtri,
        gtrr,
        eqir,
        eqri,
        eqrr
    }

    public static IEnumerable<Opcode> GetAllOpcodesThatMatch(Snapshot snapshot)
    {
        var list = new List<Opcode?>()
        {
            AddrOrNull(snapshot),
            AddiOrNull(snapshot),
            MulrOrNull(snapshot),
            MuliOrNull(snapshot),
            BanrOrNull(snapshot),
            BaniOrNull(snapshot),
            BorrOrNull(snapshot),
            BoriOrNull(snapshot),
            SetrOrNull(snapshot),
            SetiOrNull(snapshot),
            GtirOrNull(snapshot),
            GtriOrNull(snapshot),
            GtrrOrNull(snapshot),
            EqirOrNull(snapshot),
            EqriOrNull(snapshot),
            EqrrOrNull(snapshot),
        };

        return list.Where(w => w != null).Cast<Opcode>().ToList();
    }

    private static Opcode? AddrOrNull(Snapshot snapshot)
    {
        if (snapshot.After[snapshot.Instruction[3]] == 
            (snapshot.Before[snapshot.Instruction[1]] + snapshot.Before[snapshot.Instruction[2]]))
            return Opcode.addr;
        
        return null;
    }

    private static Opcode? AddiOrNull(Snapshot snapshot)
    {
        if (snapshot.After[snapshot.Instruction[3]] ==
            (snapshot.Before[snapshot.Instruction[1]] + snapshot.Instruction[2]))
            return Opcode.addi;

        return null;
    }

    private static Opcode? MulrOrNull(Snapshot snapshot)
    {
        if (snapshot.After[snapshot.Instruction[3]] ==
            (snapshot.Before[snapshot.Instruction[1]] * snapshot.Before[snapshot.Instruction[2]]))
            return Opcode.mulr;

        return null;
    }

    private static Opcode? MuliOrNull(Snapshot snapshot)
    {
        if (snapshot.After[snapshot.Instruction[3]] ==
            (snapshot.Before[snapshot.Instruction[1]] * snapshot.Instruction[2]))
            return Opcode.muli;

        return null;
    }

    private static Opcode? BanrOrNull(Snapshot snapshot)
    {
        if (snapshot.After[snapshot.Instruction[3]] ==
            (snapshot.Before[snapshot.Instruction[1]] & snapshot.Before[snapshot.Instruction[2]]))
            return Opcode.banr;

        return null;
    }

    private static Opcode? BaniOrNull(Snapshot snapshot)
    {
        if (snapshot.After[snapshot.Instruction[3]] ==
            (snapshot.Before[snapshot.Instruction[1]] & snapshot.Instruction[2]))
            return Opcode.bani;

        return null;
    }

    private static Opcode? BorrOrNull(Snapshot snapshot)
    {
        if (snapshot.After[snapshot.Instruction[3]] ==
            (snapshot.Before[snapshot.Instruction[1]] | snapshot.Before[snapshot.Instruction[2]]))
            return Opcode.borr;

        return null;
    }

    private static Opcode? BoriOrNull(Snapshot snapshot)
    {
        if (snapshot.After[snapshot.Instruction[3]] ==
            (snapshot.Before[snapshot.Instruction[1]] | snapshot.Instruction[2]))
            return Opcode.bori;

        return null;
    }

    private static Opcode? SetrOrNull(Snapshot snapshot)
    {
        if (snapshot.After[snapshot.Instruction[3]] ==
            snapshot.Before[snapshot.Instruction[1]])
            return Opcode.setr;

        return null;
    }

    private static Opcode? SetiOrNull(Snapshot snapshot)
    {
        if (snapshot.After[snapshot.Instruction[3]] ==
            snapshot.Instruction[1])
            return Opcode.seti;

        return null;
    }

    private static Opcode? GtirOrNull(Snapshot snapshot)
    {
        if ((snapshot.After[snapshot.Instruction[3]] == 1 && (snapshot.Instruction[1] > snapshot.Before[snapshot.Instruction[2]]))
            || (snapshot.After[snapshot.Instruction[3]] == 0 && (snapshot.Instruction[1] <= snapshot.Before[snapshot.Instruction[2]])))
            return Opcode.gtir;

        return null;
    }

    private static Opcode? GtriOrNull(Snapshot snapshot)
    {
        if ((snapshot.After[snapshot.Instruction[3]] == 1 && (snapshot.Before[snapshot.Instruction[1]] > snapshot.Instruction[2]))
            || (snapshot.After[snapshot.Instruction[3]] == 0 && (snapshot.Before[snapshot.Instruction[1]] <= snapshot.Instruction[2])))
            return Opcode.gtri;

        return null;
    }

    private static Opcode? GtrrOrNull(Snapshot snapshot)
    {
        if ((snapshot.After[snapshot.Instruction[3]] == 1 && (snapshot.Before[snapshot.Instruction[1]] > snapshot.Before[snapshot.Instruction[2]]))
            || (snapshot.After[snapshot.Instruction[3]] == 0 && (snapshot.Before[snapshot.Instruction[1]] <= snapshot.Before[snapshot.Instruction[2]])))
            return Opcode.gtrr;

        return null;
    }

    private static Opcode? EqirOrNull(Snapshot snapshot)
    {
        if ((snapshot.After[snapshot.Instruction[3]] == 1 && (snapshot.Instruction[1] == snapshot.Before[snapshot.Instruction[2]]))
            || (snapshot.After[snapshot.Instruction[3]] == 0 && (snapshot.Instruction[1] != snapshot.Before[snapshot.Instruction[2]])))
            return Opcode.eqir;

        return null;
    }

    private static Opcode? EqriOrNull(Snapshot snapshot)
    {
        if ((snapshot.After[snapshot.Instruction[3]] == 1 && (snapshot.Before[snapshot.Instruction[1]] == snapshot.Instruction[2]))
            || (snapshot.After[snapshot.Instruction[3]] == 0 && (snapshot.Before[snapshot.Instruction[1]] != snapshot.Instruction[2])))
            return Opcode.eqri;

        return null;
    }

    private static Opcode? EqrrOrNull(Snapshot snapshot)
    {
        if ((snapshot.After[snapshot.Instruction[3]] == 1 && (snapshot.Before[snapshot.Instruction[1]] == snapshot.Before[snapshot.Instruction[2]]))
            || (snapshot.After[snapshot.Instruction[3]] == 0 && (snapshot.Before[snapshot.Instruction[1]] != snapshot.Before[snapshot.Instruction[2]])))
            return Opcode.eqrr;

        return null;
    }
}

record Snapshot(int[] Before, int[] After, int[] Instruction)
{
    
}

class SnapshotBuilder
{
    enum BuilderState { Empty, AddedFirstLine, AddedSecondLine, AddedThirdLine };
    private BuilderState state;
    
    int[]? before;
    int[]? after;
    int[]? instruction;

    private readonly Regex numRegex = new(@"\d+");

    public SnapshotBuilder()
    {
        this.state = BuilderState.Empty;
    }

    public Snapshot Build()
    {
        if (this.state != BuilderState.AddedThirdLine)
            throw new Exception();

        if (this.before == null) throw new Exception();
        if (this.instruction == null) throw new Exception();
        if (this.after == null) throw new Exception();

        return new Snapshot(this.before, this.after, this.instruction);
    }

    public void AddLine(string line)
    {
        if (state == BuilderState.AddedThirdLine) throw new Exception();

        var numbers = numRegex.Matches(line).Select(w => int.Parse(w.Value)).ToArray();

        if (numbers.Length != 4) throw new Exception();

        if (state == BuilderState.Empty)
        {
            this.before = numbers;
            this.state = BuilderState.AddedFirstLine;
        }
        else if (state == BuilderState.AddedFirstLine)
        {
            this.instruction = numbers;
            this.state = BuilderState.AddedSecondLine;
        }
        else if (state == BuilderState.AddedSecondLine)
        {
            this.after = numbers;
            this.state = BuilderState.AddedThirdLine;
        }
    }
}