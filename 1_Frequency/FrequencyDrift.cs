using System;
using System.Collections.Generic;
using SantasToolbox;

namespace _1_Frequency
{
    class FrequencyDrift
    {
        static void Main(string[] args)
        {
            var changeProvider = new InputProvider<int>(@"Input.txt", StringToTConverter);
            
            int currentFrequency = 0;

            HashSet<int> alreadySeenFrequencies = new HashSet<int>();
            bool foundRepeatedValue = false;

            while (!foundRepeatedValue)
            {
                for (; changeProvider.MoveNext(); currentFrequency += changeProvider.Current)
                {
                    Console.WriteLine($"Resulting frequency: {currentFrequency}");

                    if (alreadySeenFrequencies.Contains(currentFrequency))
                    {
                        foundRepeatedValue = true;
                        break;
                    }
                    else
                    {
                        alreadySeenFrequencies.Add(currentFrequency);
                    }
                }
                changeProvider.Reset();
            }; 

            Console.WriteLine();
            Console.WriteLine($"Resulting frequency: {currentFrequency}");
            Console.WriteLine("Press any key to continue;");
            Console.ReadKey();
        }

        private static bool StringToTConverter(string input, out int result)
        {
            return int.TryParse(input, out result);
        }
    }
}
