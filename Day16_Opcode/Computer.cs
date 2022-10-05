using System.Diagnostics;

class Computer
{
    private readonly UniqueFactory<string, Register> Registers;
    private readonly Dictionary<string, Opcode> InstructionMapping;
    private readonly List<string> Instructions;
    private int instructionIndex;

    public Computer(IEnumerable<string> instructions, Dictionary<string, Opcode> mapping)
    {
        this.Instructions = instructions.ToList();
        this.InstructionMapping = mapping;
        this.Registers = new UniqueFactory<string, Register>(name => new Register(name));
        this.instructionIndex = 0;
    }

    public void SetRegisterValue(string register, long value)
    {
        var r = Registers.GetOrCreateInstance(register);
        r.Value = value;
    }

    public long GetRegisterValue(string register)
    {
        var r = Registers.GetOrCreateInstance(register);
        return r.Value;
    }

    public void Run()
    {
        for (; instructionIndex >= 0 && instructionIndex < Instructions.Count; instructionIndex++)
        {
            var instruction = Instructions[instructionIndex];

            var parts = instruction.Split(" ");

            var operation = InstructionMapping[parts[0]];

            if (operation == Opcode.addr)
            {
                var r = Registers.GetOrCreateInstance(parts[3]);

                r.Value = GetRegisterValue(parts[1]) + GetRegisterValue(parts[2]);
            }
            else if (operation == Opcode.addi)
            {
                var r = Registers.GetOrCreateInstance(parts[3]);

                r.Value = GetRegisterValue(parts[1]) + long.Parse(parts[2]);
            }
            else if (operation == Opcode.mulr)
            {
                var r = Registers.GetOrCreateInstance(parts[3]);

                r.Value = GetRegisterValue(parts[1]) * GetRegisterValue(parts[2]);
            }
            else if (operation == Opcode.muli)
            {
                var r = Registers.GetOrCreateInstance(parts[3]);

                r.Value = GetRegisterValue(parts[1]) * long.Parse(parts[2]);
            }
            else if (operation == Opcode.banr)
            {
                var r = Registers.GetOrCreateInstance(parts[3]);

                r.Value = GetRegisterValue(parts[1]) & GetRegisterValue(parts[2]);
            }
            else if (operation == Opcode.bani)
            {
                var r = Registers.GetOrCreateInstance(parts[3]);

                r.Value = GetRegisterValue(parts[1]) & long.Parse(parts[2]);
            }
            else if (operation == Opcode.borr)
            {
                var r = Registers.GetOrCreateInstance(parts[3]);

                r.Value = GetRegisterValue(parts[1]) | GetRegisterValue(parts[2]);
            }
            else if (operation == Opcode.bori)
            {
                var r = Registers.GetOrCreateInstance(parts[3]);

                r.Value = GetRegisterValue(parts[1]) | long.Parse(parts[2]);
            }
            else if (operation == Opcode.setr)
            {
                var r = Registers.GetOrCreateInstance(parts[3]);

                r.Value = GetRegisterValue(parts[1]);
            }
            else if (operation == Opcode.seti)
            {
                var r = Registers.GetOrCreateInstance(parts[3]);

                r.Value = long.Parse(parts[1]);
            }
            else if (operation == Opcode.gtir)
            {
                var r = Registers.GetOrCreateInstance(parts[3]);

                r.Value = long.Parse(parts[1]) > GetRegisterValue(parts[2]) ? 1 : 0;
            }
            else if (operation == Opcode.gtri)
            {
                var r = Registers.GetOrCreateInstance(parts[3]);

                r.Value = GetRegisterValue(parts[1]) > long.Parse(parts[2]) ? 1 : 0;
            }
            else if (operation == Opcode.gtrr)
            {
                var r = Registers.GetOrCreateInstance(parts[3]);

                r.Value = GetRegisterValue(parts[1]) > GetRegisterValue(parts[2]) ? 1 : 0;
            }
            else if (operation == Opcode.eqir)
            {
                var r = Registers.GetOrCreateInstance(parts[3]);

                r.Value = long.Parse(parts[1]) == GetRegisterValue(parts[2]) ? 1 : 0;
            }
            else if (operation == Opcode.eqri)
            {
                var r = Registers.GetOrCreateInstance(parts[3]);

                r.Value = GetRegisterValue(parts[1]) == long.Parse(parts[2]) ? 1 : 0;
            }
            else if (operation == Opcode.eqrr)
            {
                var r = Registers.GetOrCreateInstance(parts[3]);

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