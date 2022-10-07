var computer = GetComputerFromInputData();
computer.Run();

Console.WriteLine($"Part 1: {computer.GetRegisterValue("0")}");

static Computer GetComputerFromInputData()
{
    var input = new StringInputProvider("Input.txt");
    return new Computer(input, "2");    //"2" is part of input data, avoiding parsing that for simplicity
}