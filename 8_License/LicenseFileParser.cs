using SantasToolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace _8_License
{
    class LicenseFileParser
    {
        static void Main(string[] args)
        {
            var licenseInput = GetLicenseInput();

            var rootNode = GetNode(licenseInput, out int end);

            Console.WriteLine($"Part 1: Sum of metadata: {rootNode.MetadataSum}");
            Console.WriteLine("");
            Console.WriteLine($"Part 2: Value of root node: {rootNode.Value}");

            Console.WriteLine("");
            Console.WriteLine("Done");
            Console.ReadKey();
        }

        private static LicenseNode GetNode(IEnumerable<int> input, out int length)
        {
            var numChildren = input.First();
            var numMetadata = input.Skip(1).First();

            var children = new LicenseNode[numChildren];
            var lengthOfAllChildren = 0;

            for (int i = 0; i < numChildren; i++)
            {
                children[i] = GetNode(input.Skip(2 + lengthOfAllChildren), out int childLength);
                lengthOfAllChildren += childLength;
            }

            length = 2 + lengthOfAllChildren + numMetadata;

            return new LicenseNode(children, input.Skip(2 + lengthOfAllChildren).Take(numMetadata));
        }

        private static int[] GetLicenseInput()
        {
            var licenseProvider = new InputProvider<int[]>("Input.txt", GetNumbersInLine);
            licenseProvider.MoveNext();
            return licenseProvider.Current;
        }

        private static bool GetNumbersInLine(string value, out int[] result)
        {
            var numRegex = new Regex(@"\d+");
            var matches = numRegex.Matches(value);
            result = matches.Select(w => int.Parse(w.Value)).ToArray();

            return result.Length > 0;
        }
    }
}
