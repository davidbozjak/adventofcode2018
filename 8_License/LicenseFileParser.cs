using SantasToolbox;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace _8_License
{
    class LicenseFileParser
    {
        static void Main(string[] args)
        {
            var licenseInput = GetLicenseInput();
            var licenseReader = new EnumerableReader<int>(licenseInput);

            var rootNode = GetNode(licenseReader);

            Console.WriteLine($"Part 1: Sum of metadata: {rootNode.MetadataSum}");
            Console.WriteLine("");
            Console.WriteLine($"Part 2: Value of root node: {rootNode.Value}");

            Console.WriteLine("");
            Console.WriteLine("Done");
            Console.ReadKey();
        }

        private static LicenseNode GetNode(EnumerableReader<int> input)
        {
            var numChildren = input.TakeFirst();
            var numMetadata = input.TakeFirst();

            var children = new LicenseNode[numChildren];

            for (int i = 0; i < numChildren; i++)
            {
                children[i] = GetNode(input);
            }

            return new LicenseNode(children, input.TakeN(numMetadata));
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
