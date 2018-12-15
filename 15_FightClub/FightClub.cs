using SantasToolbox;
using System;
using System.Linq;

namespace _15_FightClub
{
    class FightClub
    {
        static void Main(string[] args)
        {
            var factory = new WorldFactory();
            var world = factory.GetInitialState();
            var printer = new WorldPrinter();

            int numOfFullRounds = -1;

            do
            {
                numOfFullRounds++;
                printer.Print(world);
                Console.WriteLine($"After round {numOfFullRounds}");
                //Console.ReadKey();
            } while (world.MakeRound());
            
            var sumOfHitpoints = world.Fighters.Sum(w => w.HP);
            
            Console.WriteLine();
            Console.WriteLine($"Done. Number of full rounds: {numOfFullRounds} Sum of hitpoints remainging: {sumOfHitpoints}");
            Console.Write($"Part 1: {numOfFullRounds * sumOfHitpoints}");
            Console.ReadKey();
        }
    }
}
