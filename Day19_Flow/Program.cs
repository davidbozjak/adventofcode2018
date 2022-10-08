
// Assmebler code straight rewritten to C#:
// For demonstration running part 1 (with lower, part 1, target)

long reg5 = 931;
//long reg5 = 10551331;
long reg0 = 0;

for (long reg1 = 1; reg1 <= reg5; reg1++)
{
    for (long reg4 = 1; reg4 <= reg5; reg4++)
    {
        if (reg1 * reg4 == reg5)
        {
            reg0 += reg1;
            Console.WriteLine($"Reg1: {reg1} Reg4: {reg4}, new sum: {reg0}");
        }
    }
}

Console.WriteLine($"Part 1: {reg0} (computed with original algorithm)");

// What the algorithm does, finds all deviders of target number and sums them.
// Very simple algorithm for this in C#:

//long target = 931;
long target = 10551331;
var deviders = new List<long>();

for (long i = 1; i <= target; i++)
{
    if (target % i == 0)
    {
        deviders.Add(i);
    }
}

Console.WriteLine(String.Join(" ,", deviders));
Console.WriteLine($"Part 2: {deviders.Sum()}");

//Executing the actual input computer code:

var computer = GetComputerFromInputData();
computer.Run();

Console.WriteLine($"Part 1: {computer.GetRegisterValue("0")} (computed running actual input)");

// Don't try executing this for part 2, it would take forever

//computer = GetComputerFromInputData();
//computer.SetRegisterValue("0", 1);

//computer.Run();

//Console.WriteLine($"Part 2: {computer.GetRegisterValue("0")}");

static Computer GetComputerFromInputData()
{
    var input = new StringInputProvider("Input.txt");
    return new Computer(input, "2");    //"2" is part of input data, avoiding parsing that for simplicity
}