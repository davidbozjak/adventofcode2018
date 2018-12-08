using SantasToolbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _7_Steps
{
    class StepsFinder
    {
        static void Main(string[] args)
        {
            var constraints = GetConstraints();

            var nodes = new Dictionary<char, Node>();

            foreach (var constraint in constraints)
            {
                var first = CreateOrReturnNode(constraint.First);
                var after = CreateOrReturnNode(constraint.After);

                after.AddParent(first);
            }

            PrintResultForPart1(nodes);
            Console.WriteLine("");
            PrintResultForPart2(nodes);

            Console.WriteLine("");
            Console.WriteLine("Done");
            Console.ReadKey();

            Node CreateOrReturnNode(char nodeId)
            {
                if (!nodes.ContainsKey(nodeId))
                {
                    nodes.Add(nodeId, new Node(nodeId));
                }

                return nodes[nodeId];
            }
        }

        private static void PrintResultForPart1(Dictionary<char, Node> nodes)
        {
            var stepPlan = new List<char>();
            var stepsToUse = nodes.Values.ToList();

            while (stepsToUse.Count > 0)
            {
                var unblockedStep = stepsToUse
                    .Where(w => w.Parents.All(parent => stepPlan.Contains(parent.Id)))
                    .OrderBy(w => w.Id)
                    .First();

                stepPlan.Add(unblockedStep.Id);
                stepsToUse.Remove(unblockedStep);
            }

            Console.WriteLine($"Part 1: Steps: {new string(stepPlan.ToArray())}");
        }

        private static void PrintResultForPart2(Dictionary<char, Node> nodes)
        {
            var stepPlan = new List<char>();
            var stepsToUse = nodes.Values.ToList();

            const int numOfWorkers = 5;
            var workerAssignment = new Node[numOfWorkers];
            int now = 0;

            while (stepsToUse.Count > 0)
            {
                while (workerAssignment.Contains(null))
                {
                    var workerIndex = GetIndexOfNextAvaliableWorkerSlot();

                    var unblockedStep = stepsToUse
                        .Where(w => w.Parents.All(parent => stepPlan.Contains(parent.Id)))
                        .OrderBy(w => w.Id)
                        .FirstOrDefault();

                    if (unblockedStep == null)
                    {
                        break;
                    }

                    unblockedStep.Start(now);
                    stepsToUse.Remove(unblockedStep);

                    workerAssignment[workerIndex] = unblockedStep;
                }

                var completedIndex = GetIndexOfWorkerDoneFirst();
                var compeltedStep = workerAssignment[completedIndex];
                workerAssignment[completedIndex] = null;

                stepPlan.Add(compeltedStep.Id);
                now = compeltedStep.CompletedAt;
            }
            
            Console.WriteLine($"Part 2: Steps: {new string(stepPlan.ToArray())} Completed at: {now}");

            int GetIndexOfNextAvaliableWorkerSlot()
            {

                for (int i = 0; i < workerAssignment.Length; i++)
                {
                    if (workerAssignment[i] == null)
                    {
                        return i;
                    }
                }

                return -1;
            }

            int GetIndexOfWorkerDoneFirst()
            {
                int firstCompletedAt = int.MaxValue;
                int firstCompleted = -1;

                for (int i = 0; i < workerAssignment.Length; i++)
                {
                    if (workerAssignment[i] != null && workerAssignment[i].CompletedAt < firstCompletedAt)
                    {
                        firstCompletedAt = workerAssignment[i].CompletedAt;
                        firstCompleted = i;
                    }
                }

                return firstCompleted;
            }
        }

        private static List<Constraint> GetConstraints()
        {
            var coordinatesProvider = new InputProvider<Constraint>("Input.txt", GetConstraint);

            var points = new List<Constraint>();

            while (coordinatesProvider.MoveNext())
            {
                points.Add(coordinatesProvider.Current);
            }

            return points;
        }

        private static bool GetConstraint(string instruction, out Constraint result)
        {
            try
            {
                result = new Constraint(instruction);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }
    }
}
