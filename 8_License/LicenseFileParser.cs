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

        private static Node GetNode(IEnumerable<int> input, out int end)
        {
            var numChildren = input.First();
            var numMetadata = input.Skip(1).First();

            var children = new Node[numChildren];
            var maxLast = 0;

            for (int i = 0; i < numChildren; i++)
            {
                children[i] = GetNode(input.Skip(2 + maxLast), out int childEnd);
                maxLast += childEnd;
            }

            end = 2 + maxLast + numMetadata;

            return new Node(children, input.Skip(2 + maxLast).Take(numMetadata));
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
        private static char IdCounter = 'A';

        public Node(IEnumerable<Node> children, IEnumerable<int> metadata)
        {
            this.Id = IdCounter++;

            this.Children = children.ToList();
            this.Metadata = metadata.ToList();
        }

        public char Id { get; }

        public IReadOnlyCollection<Node> Children;

        public IReadOnlyCollection<int> Metadata;

        public int MetadataSum => this.Children.Sum(w => w.MetadataSum) + this.Metadata.Sum();

        public int Value
        {
            get
            {
                if (this.Children.Count == 0)
                {
                    return this.MetadataSum;
                }
                else
                {
                    int value = 0;

                    foreach (var index in Metadata)
                    {
                        if (index >= 1 && index <= this.Children.Count)
                        {
                            value += this.Children.ElementAt(index - 1).Value;
                        }
                    }

                    return value;
                }
            }
        }
    }
}
