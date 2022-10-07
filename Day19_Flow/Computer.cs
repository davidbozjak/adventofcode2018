using System.Diagnostics;

class Computer
{
    private readonly UniqueFactory<string, Register> registers;
    private readonly List<string> instructions;
    private readonly Register instructionPointerRegister;

    public Computer(IEnumerable<string> instructions, string instructionPointerRegisterName)
    {
        this.instructions = instructions.ToList();
        this.registers = new UniqueFactory<string, Register>(name => new Register(name));
        this.instructionPointerRegister = registers.GetOrCreateInstance(instructionPointerRegisterName);
    }

    public void SetRegisterValue(string register, long value)
    {
        var r = registers.GetOrCreateInstance(register);
        r.Value = value;
    }

    public long GetRegisterValue(string register)
    {
        var r = registers.GetOrCreateInstance(register);
        return r.Value;
    }

    public void Run()
    {
        for (; instructionPointerRegister.Value >= 0 && instructionPointerRegister.Value < instructions.Count; instructionPointerRegister.Value++)
        {
            var instruction = instructions[(int)instructionPointerRegister.Value];

            var parts = instruction.Split(" ");

            var operation = parts[0];

            if (operation == "addr")
            {
                var r = registers.GetOrCreateInstance(parts[3]);

                r.Value = GetRegisterValue(parts[1]) + GetRegisterValue(parts[2]);
            }
            else if (operation == "addi")
            {
                var r = registers.GetOrCreateInstance(parts[3]);

                r.Value = GetRegisterValue(parts[1]) + long.Parse(parts[2]);
            }
            else if (operation == "mulr")
            {
                var r = registers.GetOrCreateInstance(parts[3]);

                r.Value = GetRegisterValue(parts[1]) * GetRegisterValue(parts[2]);
            }
            else if (operation == "muli")
            {
                var r = registers.GetOrCreateInstance(parts[3]);

                r.Value = GetRegisterValue(parts[1]) * long.Parse(parts[2]);
            }
            else if (operation == "banr")
            {
                var r = registers.GetOrCreateInstance(parts[3]);

                r.Value = GetRegisterValue(parts[1]) & GetRegisterValue(parts[2]);
            }
            else if (operation == "bani")
            {
                var r = registers.GetOrCreateInstance(parts[3]);

                r.Value = GetRegisterValue(parts[1]) & long.Parse(parts[2]);
            }
            else if (operation == "borr")
            {
                var r = registers.GetOrCreateInstance(parts[3]);

                r.Value = GetRegisterValue(parts[1]) | GetRegisterValue(parts[2]);
            }
            else if (operation == "bori")
            {
                var r = registers.GetOrCreateInstance(parts[3]);

                r.Value = GetRegisterValue(parts[1]) | long.Parse(parts[2]);
            }
            else if (operation == "setr")
            {
                var r = registers.GetOrCreateInstance(parts[3]);

                r.Value = GetRegisterValue(parts[1]);
            }
            else if (operation == "seti")
            {
                var r = registers.GetOrCreateInstance(parts[3]);

                r.Value = long.Parse(parts[1]);
            }
            else if (operation == "gtir")
            {
                var r = registers.GetOrCreateInstance(parts[3]);

                r.Value = long.Parse(parts[1]) > GetRegisterValue(parts[2]) ? 1 : 0;
            }
            else if (operation == "gtri")
            {
                var r = registers.GetOrCreateInstance(parts[3]);

                r.Value = GetRegisterValue(parts[1]) > long.Parse(parts[2]) ? 1 : 0;
            }
            else if (operation == "gtrr")
            {
                var r = registers.GetOrCreateInstance(parts[3]);

                r.Value = GetRegisterValue(parts[1]) > GetRegisterValue(parts[2]) ? 1 : 0;
            }
            else if (operation == "eqir")
            {
                var r = registers.GetOrCreateInstance(parts[3]);

                r.Value = long.Parse(parts[1]) == GetRegisterValue(parts[2]) ? 1 : 0;
            }
            else if (operation == "eqri")
            {
                var r = registers.GetOrCreateInstance(parts[3]);

                r.Value = GetRegisterValue(parts[1]) == long.Parse(parts[2]) ? 1 : 0;
            }
            else if (operation == "eqrr")
            {
                var r = registers.GetOrCreateInstance(parts[3]);

                r.Value = GetRegisterValue(parts[1]) == GetRegisterValue(parts[2]) ? 1 : 0;
            }
            else throw new Exception("Unknown instruction");
        }
    }

    [DebuggerDisplay("{Name}:{Value}")]
    protected class Register
    {
        public string Name { get; }
        public long Value { get; set; } = 0;

        public Register(string name)
        {
            this.Name = name;
        }
    }
}