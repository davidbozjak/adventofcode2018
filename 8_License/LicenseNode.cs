using System.Collections.Generic;
using System.Linq;

namespace _8_License
{
    class LicenseNode
    {
        private static char IdCounter = 'A';

        public LicenseNode(IEnumerable<LicenseNode> children, IEnumerable<int> metadata)
        {
            this.Id = IdCounter++;

            this.Children = children.ToList();
            this.Metadata = metadata.ToList();
        }

        public char Id { get; }

        public IReadOnlyCollection<LicenseNode> Children;

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
