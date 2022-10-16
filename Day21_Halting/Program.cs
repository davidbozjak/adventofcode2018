var input = new StringInputProvider("Input.txt").ToList();

// To run the simulation for one input:
//var computer = new Computer(input, "1");
//computer.SetRegisterValue("0", 0);
//computer.Run();

// simulating the computer, finding:
// - the first input that the program would accept (part 1)
// - the last input the program would accept before the cylce repeats itself (part 2)

var list = new List<long>();
var possibilities = new HashSet<long>();

long reg2 = 65536;
long reg3 = 1505483;

while (true)
{
    long reg4 = reg2 & 255;
    reg3 = reg3 + reg4;
    reg3 = reg3 & 16777215;
    reg3 = reg3 * 65899;
    reg3 = reg3 & 16777215;

    if (reg2 < 256)
    {
        //Console.WriteLine($"Input that completes: {reg3}");
        
        if (possibilities.Count == 0)
        {
            Console.WriteLine($"Part 1: {reg3}");
        }

        if (possibilities.Contains(reg3))
        {
            Console.WriteLine($"Part 2: {list.Last()}");
            break;
        }
        possibilities.Add(reg3);
        list.Add(reg3);

        reg2 = reg3 | 65536;
        reg3 = 1505483;
    }
    else
    {
        reg2 = reg2 / 256;
    }
}