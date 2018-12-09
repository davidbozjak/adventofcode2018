using System;
using System.Collections.Generic;
using System.Linq;

namespace _9_Marbles
{
    class Program
    {
        static void Main(string[] args)
        {
            const int numPlayers = 477;
            const int lastMarble = 70851 * 100;

            var scores = new UInt64[numPlayers];

            var board = new LinkedList<UInt64>();
            var currentMarble = board.AddFirst(0);
            
            for (uint marble = 1, currentPlayer = 0; marble <= lastMarble; marble++, currentPlayer = (currentPlayer + 1) % numPlayers)
            {
                if (marble % 23 == 0)
                {
                    var nodeToRemove = GetNodeToRemove();
                    currentMarble = nodeToRemove.Next ?? board.First;
                    board.Remove(nodeToRemove);
                    
                    scores[currentPlayer] += marble + nodeToRemove.Value;
                    
                    LinkedListNode<UInt64> GetNodeToRemove()
                    {
                        var node = currentMarble;
                        for (int i = 0; i < 7; i++)
                        {
                            node = node.Previous ?? board.Last;
                        }

                        return node;
                    }
                }
                else
                {
                    var addAfter = currentMarble.Next ?? board.First;
                    currentMarble = board.AddAfter(addAfter, marble);
                }
            }

            var highScore = scores.Max();

            Console.WriteLine($"Winning score: {highScore}");
            Console.ReadKey();
        }
    }
}
