using SantasToolbox;
using System;
using System.Collections.Generic;

namespace _2_Checksum
{
    class SimilarIdFinder
    {
        static void Main(string[] args)
        {
            var idProvider = new InputProvider<string>("Input.txt", TrimmedStringProvider);
           
            var ids = new List<string>();

            while (idProvider.MoveNext())
            {
                ids.Add(idProvider.Current);
            }

            for (int i = 0, j = 0; i < ids.Count; i++)
            {
                string id = ids[i];

                int numOfDiff = 0;
                for (j = i + 1; j < ids.Count; j++, numOfDiff = 0)
                {
                    var idPair = ids[j];
                    Console.WriteLine($"{Environment.NewLine}New attempt withindexes {i} and {j}: ");
                    for (int pos = 0; pos < id.Length && numOfDiff <= 1; pos++)
                    {
                        if (id[pos] != idPair[pos])
                        {
                            numOfDiff++;
                        }
                        else
                        {
                            Console.Write(id[pos]);
                        }
                    }

                    if (numOfDiff <= 1)
                    {
                        Console.WriteLine();
                        Console.WriteLine($"Success!");
                        Console.WriteLine("Press any key to continue;");
                        Console.ReadKey();
                        return;
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine($"No similar ids found!");
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
