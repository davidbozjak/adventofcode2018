using System.Collections.Generic;
using System.Linq;

namespace _7_Steps
{
    class Node
    {
        private readonly List<Node> parents = new List<Node>();

        public Node(char Id)
        {
            this.Id = Id;
            this.Duration = 61 + (Id - 'A');
            this.CompletedAt = int.MinValue;
        }

        public char Id { get; }

        public int Duration { get; }

        public int CompletedAt { get; private set; }

        public IReadOnlyList<Node> Parents => this.parents.AsReadOnly();

        public void AddParent(Node n)
        {
            if (!this.parents.Select(w => w.Id).Any(w => w == n.Id))
            {
                this.parents.Add(n);
            }
        }

        public void Start(int now)
        {
            this.CompletedAt = now + Duration;
        }
    }
}
