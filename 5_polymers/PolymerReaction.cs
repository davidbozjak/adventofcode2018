using SantasToolbox;
using System;
using System.Collections.Generic;
using System.Text;

namespace _5_polymers
{
    class PolymerReaction
    {
        private const int reactDiff = 'a' - 'A';

        static void Main(string[] args)
        {
            var polymerProvider = new InputProvider<string>("Input.txt", GetString);
            polymerProvider.MoveNext();

            string polymer = polymerProvider.Current;
            
            var polymerLengthAfterInitialReaction = ReactPolymer(polymer);

            Console.WriteLine($"Part 1: polymer length after initial reaction: {polymerLengthAfterInitialReaction}");

            Console.WriteLine("Part 2:");

            var uniqueUnits = GetAllUniqueUnits(polymer);
            int minLength = int.MaxValue;

            foreach (var charToRemove in uniqueUnits)
            {
                var modifiedPolymer = new StringBuilder(polymer)
                    .RemoveAllOccurrencesOfChar((char)charToRemove)
                    .RemoveAllOccurrencesOfChar((char)(charToRemove - reactDiff))
                    .ToString();

                var modifiedPolymerLengthAfterInitialReaction = ReactPolymer(modifiedPolymer);

                if (modifiedPolymerLengthAfterInitialReaction < minLength)
                {
                    minLength = modifiedPolymerLengthAfterInitialReaction;
                    Console.WriteLine($"New min length found: {modifiedPolymerLengthAfterInitialReaction} after removing {(char)charToRemove}");
                }
            }
            
            Console.WriteLine("");
            Console.WriteLine("Done, Press any key co close.");
            Console.ReadKey();
        }

        private static int ReactPolymer(string polymer)
        {
            var polymerBuilder = new StringBuilder(polymer);
            
            for (int i = 1, length = int.MaxValue; length > polymerBuilder.Length; i = Math.Max(1, i - 1))
            {
                length = polymerBuilder.Length;

                for (; i < polymerBuilder.Length; i++)
                {
                    var diff = Math.Abs(polymerBuilder[i] - polymerBuilder[i - 1]);

                    if (diff == reactDiff)
                    {
                        polymerBuilder.Remove(i - 1, 2);
                        break;
                    }
                }
            }

            return polymerBuilder.Length;
        }
        
        private static HashSet<int> GetAllUniqueUnits(string polymer)
        {
            var set = new HashSet<int>();

            for (int i = 0; i < polymer.Length; i++)
            {
                if (polymer[i] < 'a') continue;

                if (!set.Contains(polymer[i]))
                {
                    set.Add(polymer[i]);
                }
            }

            return set;
        }

        private static bool GetString(string value, out string result)
        {
            result = value;
            return !string.IsNullOrWhiteSpace(value);
        }
    }
}
