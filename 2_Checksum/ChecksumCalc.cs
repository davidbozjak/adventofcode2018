using SantasToolbox;
using System;
using System.Collections.Generic;

namespace _2_Checksum
{
    class ChecksumCalc
    {
        static void Main(string[] args)
        {
            var idProvider = new InputProvider<string>("Input.txt", TrimmedStringProvider);

            int idsWithTwoKeys = 0;
            int idsWithThreeKeys = 0;

            while (idProvider.MoveNext())
            {
                var id = idProvider.Current;

                var letterBreakdown = new Dictionary<char, int>();

                foreach (char c in id)
                {
                    if (!letterBreakdown.ContainsKey(c))
                    {
                        letterBreakdown.Add(c, 0);
                    }

                    letterBreakdown[c]++;
                }

                if (letterBreakdown.ContainsValue(2))
                {
                    idsWithTwoKeys++;
                }

                if (letterBreakdown.ContainsValue(3))
                {
                    idsWithThreeKeys++;
                }
            }

            Console.WriteLine();
            Console.WriteLine($"Resulting checksum: {idsWithTwoKeys * idsWithThreeKeys}");
            Console.WriteLine("Press any key to continue;");
            Console.ReadKey();
        }

        private static bool TrimmedStringProvider(string value, out string result)
        {
            result = value?.ToLower().Trim();
            return !string.IsNullOrWhiteSpace(result);
        }
    }
}
