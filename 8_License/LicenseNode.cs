using System.Collections.Generic;
using System.Linq;

namespace _8_License
{
    class LicenseNode
    {
        private readonly LicenseNode[] children;
        private readonly int[] metadata;

        public LicenseNode(IEnumerable<LicenseNode> children, IEnumerable<int> metadata)
        {
            this.children = children.ToArray();
            this.metadata = metadata.ToArray();
        }

        public int MetadataSum => this.children.Sum(w => w.MetadataSum) + this.metadata.Sum();

        public int Value
        {
            get
            {
                if (this.children.Length == 0)
                {
                    return this.MetadataSum;
                }
                else
                {
                    int value = 0;

                    foreach (var index in metadata)
                    {
                        if (index >= 1 && index <= this.children.Length)
                        {
                            value += this.children.ElementAt(index - 1).Value;
                        }
                    }

                    return value;
                }
            }
        }
    }
}
