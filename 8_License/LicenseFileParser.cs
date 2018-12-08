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

            for (int i = 0; i < licenseInput.Length; i++)
            {

            }

            Console.WriteLine("");
            Console.WriteLine("Done");
            Console.ReadKey();
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

    class Node
    {
        char Id { get; }

        int NumChildren { get; }

        int NumMetadataNodes { get; }
    }
}
