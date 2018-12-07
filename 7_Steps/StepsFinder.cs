using SantasToolbox;
using System;
using System.Collections.Generic;

namespace _7_Steps
{
    class StepsFinder
    {
        static void Main(string[] args)
        {
            var constraints = GetConstraints();



            Console.WriteLine("Hello World!");
        }

        private static List<Constraint> GetConstraints()
        {
            var coordinatesProvider = new InputProvider<Constraint>("Input.txt", GetConstraint);

            var points = new List<Constraint>();

            while (coordinatesProvider.MoveNext())
            {
                points.Add(coordinatesProvider.Current);
            }

            return points;
        }

        private static bool GetConstraint(string instruction, out Constraint result)
        {
            try
            {
                result = new Constraint(instruction);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }
    }
    
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
