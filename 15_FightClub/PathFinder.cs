using SantasToolbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _15_FightClub
{
    // quick and dirty A* implementation
    class PathFinder
    {
        private readonly Tile start;
        private readonly World world;
        private readonly Lazy<List<Solution>> lazySteps;

        public PathFinder(Tile start, Tile goal, World world)
        {
            this.start = start;
            this.Goal = goal;
            this.world = world;
            this.lazySteps = new Lazy<List<Solution>>(AStar);
        }

        public bool IsReachable =>
            this.lazySteps.Value != null;

        public int NumberOfSteps => 
            this.lazySteps.Value[0].NumberOfSteps;

        public Tile NextStep =>
            this.lazySteps.Value.OrderBy(w => w.ReadingOrder).First().NextStep;

        public Tile Goal { get; }

        private List<Solution> AStar()
        {
            var solutions = new List<Solution>();

            // The set of nodes already evaluated
            var closedSet = new List<Tile>();

            // The set of currently discovered nodes that are not evaluated yet.
            // Initially, only the start node is known.
            var openSet = new List<Tile>() { this.start };

            // For each node, which node it can most efficiently be reached from.
            // If a node can be reached from many nodes, cameFrom will eventually contain the
            // most efficient previous step.
            var cameFrom = new Dictionary<Tile, Tile>();

            // For each node, the cost of getting from the start node to that node.
            var gScores = new Dictionary<Tile, int>
            {
                // The cost of going from start to start is zero.
                { start, 0 }
            };

            // For each node, the total cost of getting from the start node to the goal
            // by passing by that node. That value is partly known, partly heuristic.
            var fScore = new Dictionary<Tile, int>
            {
                // For the first node, that value is completely heuristic.
                { this.start, this.start.Position.Distance(this.Goal.Position) }
            };

            while (openSet.Count > 0)
            {
                var current = openSet.OrderBy(w => DictValueOrMax(w, fScore)).First(); //the node in openSet having the lowest fScore[] value

                if (current == this.Goal)
                {
                    var bestFoundSolution = solutions.Count > 0 ? solutions[0].NumberOfSteps : int.MaxValue;
                    var currentSolution = gScores[current];

                    if (currentSolution <= bestFoundSolution)
                    {
                        var steps = new List<Tile>();
                        ReconstructPath(cameFrom, current, steps);
                        solutions.Add(new Solution(steps));
                    }
                    else
                    {
                        return solutions;
                    }

                    openSet.Remove(current);
                    continue;
                }
                else
                {
                    openSet.Remove(current);
                    closedSet.Add(current);
                }
                
                foreach (var neighbor in current.GetAdjacentTiles(this.world).Where(w => w.IsAvaliable))
                {
                    if (closedSet.Contains(neighbor))
                    {
                        continue;       // Ignore the neighbor which is already evaluated.
                    }

                    // The distance from start to a neighbor
                    var tentative_gScore = DictValueOrMax(current, gScores) + 1;
                    var gScore = DictValueOrMax(neighbor, gScores);

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                    else if (tentative_gScore >= gScore)
                    {
                        continue;
                    }

                    // This path is the best until now. Record it!
                    InsertOrUpdate(neighbor, current, cameFrom);
                    InsertOrUpdate(neighbor, tentative_gScore, gScores);

                    var newBestScore = gScore + neighbor.Position.Distance(this.Goal.Position);
                    InsertOrUpdate(neighbor, newBestScore, fScore);
                }
            }

            return solutions.Count > 0 ? solutions : null;
        }

        private void ReconstructPath(Dictionary<Tile, Tile> cameFrom, Tile current, List<Tile> steps)
        {
            if (current == this.start)
            {
                return;
            }

            if (cameFrom.ContainsKey(current))
            {
                ReconstructPath(cameFrom, cameFrom[current], steps);
            }

            steps.Add(current);
        }

        private static int DictValueOrMax<T>(T key, Dictionary<T, int> dict)
        {
            return dict.ContainsKey(key) ? dict[key] : int.MaxValue - 1000;
        }

        private static void InsertOrUpdate<T, K>(T key, K value, Dictionary<T, K> dict)
        {
            if (!dict.ContainsKey(key))
            {
                dict.Add(key, value);
            }
            else
            {
                dict[key] = value;
            }
        }

        private class Solution
        {
            public Solution(List<Tile> steps)
            {
                this.Steps = steps;
            }

            public List<Tile> Steps { get; }

            public int NumberOfSteps => this.Steps.Count;

            public Tile NextStep => this.Steps[0];

            public int ReadingOrder => this.NextStep.Position.ReadingOrder();
        }
    }
}
