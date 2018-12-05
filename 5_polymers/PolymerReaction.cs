using SantasToolbox;
using System;
using System.Text;

namespace _5_polymers
{
    class PolymerReaction
    {
        static void Main(string[] args)
        {
            var polymerProvider = new InputProvider<string>("Input.txt", GetString);
            polymerProvider.MoveNext();

            string polymer = polymerProvider.Current;

            var polymerBuilder = new StringBuilder(polymer);

            const int reactDiff = 'a' - 'A';

            for (bool hasReacted = true; hasReacted; )
            {
                hasReacted = false;
                Console.WriteLine($"New loop, polymer length: {polymerBuilder.Length}");

                for (int i = 1; i < polymerBuilder.Length; i++)
                {
                    var diff = Math.Abs(polymerBuilder[i] - polymerBuilder[i - 1]);

                    if (diff == reactDiff)
                    {
                        hasReacted = true;
                        polymerBuilder.Remove(i - 1, 2);
                        break;
                    }
                }
            }

            Console.WriteLine($"No more reactions, final polymer length: {polymerBuilder.Length}");

            Console.WriteLine("");
            Console.WriteLine("Done, Press any key co close.");
            Console.ReadKey();
        }

        private static bool GetString(string value, out string result)
        {
            result = value;
            return !string.IsNullOrWhiteSpace(value);
        }
    }
}
