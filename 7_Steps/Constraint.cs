namespace _7_Steps
{
    class Constraint
    {
        public Constraint(string instruction)
        {
            this.OriginalInstruction = instruction;
            this.First = instruction[5];
            this.After = instruction[36];
        }

        public char First { get; }
        public char After { get; }
        public string OriginalInstruction { get; }

    }
}
