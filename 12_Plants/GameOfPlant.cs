using SantasToolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _12_Plants
{
    class GameOfPlant
    {
        static void Main(string[] args)
        {
            var currentGeneration = GenerateInitialState("##..#.#.#..##..#..##..##..#.#....#.....##.#########...#.#..#..#....#.###.###....#..........###.#.#..");
            VisualizeGeneration(currentGeneration);

            var rules = GetRules();
            var scoreLog = new Dictionary<long, List<long>>();

            const long generationLimit = 50000000000;
            
            for (long generation = 0; generation < generationLimit; generation++)
            {
                var newGeneration = new LinkedList<Pot>();

                newGeneration.AddLast(new Pot(currentGeneration.First.Value.Id - 2, rules, null, null, null, currentGeneration.First.Next?.Value));
                newGeneration.AddLast(new Pot(currentGeneration.First.Value.Id - 1, rules, null, null, currentGeneration.First.Value, currentGeneration.First.Next?.Value));

                for (var pot = currentGeneration.First; pot != null; pot = pot.Next)
                {
                    newGeneration.AddLast(new Pot(pot.Value, rules, pot.Previous?.Previous?.Value, pot.Previous?.Value, pot.Next?.Value, pot.Next?.Next?.Value));
                }

                newGeneration.AddLast(new Pot(currentGeneration.Last.Value.Id + 1, rules, currentGeneration.Last.Previous.Value, currentGeneration.Last.Value, null, null));
                newGeneration.AddLast(new Pot(currentGeneration.Last.Value.Id + 2, rules, currentGeneration.Last.Value, null, null, null));

                while (!newGeneration.First.Value.Filled)
                {
                    newGeneration.RemoveFirst();
                }

                while (!newGeneration.Last.Value.Filled)
                {
                    newGeneration.RemoveLast();
                }

                currentGeneration = newGeneration;

                // Find pattern
                var generationScore = GetSum(pot => 1, currentGeneration);

                if (scoreLog.ContainsKey(generationScore))
                {
                    Console.WriteLine($"Current generation {generation} with value of {generationScore} and Sum {GetSum(pot => pot.Id, currentGeneration)} also repeated on generations ({string.Join(',', scoreLog[generationScore].OrderBy(w => w).Take(5))}, .., {string.Join(',', scoreLog[generationScore].OrderByDescending(w => w).Take(5))})");
                    scoreLog[generationScore].Add(generation);
                    VisualizeGeneration(currentGeneration);
                }
                else
                {
                    scoreLog.Add(generationScore, new List<long>() { generation });
                }
            }
            
            Console.WriteLine();
            Console.WriteLine($"Sum of filled indexes after {generationLimit} generations: {GetSum(pot => pot.Id, currentGeneration)}");

            Console.WriteLine("Done");
            Console.ReadKey();
        }

        private static long GetSum(Func<Pot, long> selector, LinkedList<Pot> currentGeneration)
        {
            long sumOfFilledPlants = 0;

            foreach (var pot in currentGeneration)
            {
                if (pot.Filled)
                {
                    sumOfFilledPlants += selector(pot);
                }
            }

            return sumOfFilledPlants;
        }

        private static void VisualizeGeneration(LinkedList<Pot> generation)
        {
            var output = new StringBuilder();

            foreach (var pot in generation)
            {
                output.Append(pot.Filled ? "#" : ".");
            }

            Console.WriteLine(output);
        }

        private static LinkedList<Pot> GenerateInitialState(string initialState)
        {
            var list = new LinkedList<Pot>();
            int id = 0;

            foreach (var state in initialState)
            {
                list.AddLast(new Pot(id++, state == '#'));
            }

            return list;
        }

        private static ICollection<SpreadingRule> GetRules()
        {
            var rules = new List<SpreadingRule>();
            var ruleProvider = new InputProvider<SpreadingRule>("Input.txt", GetRule);

            while (ruleProvider.MoveNext())
            {
                if (ruleProvider.Current.MapsToTrue)
                {
                    rules.Add(ruleProvider.Current);
                }
            }

            return rules;
        }

        private static bool GetRule(string value, out SpreadingRule result)
        {
            try
            {
                result = new SpreadingRule(value);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }
    }

    class Pot
    {
        public Pot(int id, bool filled)
        {
            this.Id = id;
            this.Filled = filled;
        }

        public Pot(Pot currentPot, IEnumerable<SpreadingRule> rules, Pot left2, Pot left1, Pot right1, Pot right2)
        {
            this.Id = currentPot.Id;

            this.Filled = CalculateFilled(currentPot.Filled, rules, left2, left1, right1, right2);
        }
        
        public Pot(int id, IEnumerable<SpreadingRule> rules, Pot left2, Pot left1, Pot right1, Pot right2)
        {
            this.Id = id;

            this.Filled = CalculateFilled(false, rules, left2, left1, right1, right2);
        }

        public int Id { get; }
        public bool Filled { get; }

        private static bool CalculateFilled(bool currentValue, IEnumerable<SpreadingRule> rules, Pot left2, Pot left1, Pot right1, Pot right2)
        {
            var state = new bool[]
            {
                left2?.Filled == true,
                left1?.Filled == true,
                currentValue,
                right1?.Filled == true,
                right2?.Filled == true
            };

            foreach (var rule in rules)
            {
                if (rule.Matches(state))
                {
                    return true;
                }
            }

            return false;
        }
    }

    class SpreadingRule
    {
        private readonly bool[] matchingPattern;

        public SpreadingRule(string serializedRule)
        {
            this.matchingPattern = new bool[5];

            for (int i = 0; i < 5; i++)
            {
                this.matchingPattern[i] = serializedRule[i] == '#';
            }

            this.MapsToTrue = serializedRule[9] == '#';
        }

        public bool MapsToTrue { get; }

        public bool Matches(bool[] state)
        {
            for (int i = 0; i < 5; i++)
            {
                if (state[i] != matchingPattern[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
