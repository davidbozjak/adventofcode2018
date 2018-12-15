using SantasToolbox;
using System;

namespace _15_FightClub
{
    class FightClub
    {
        static void Main(string[] args)
        {
            var factory = new WorldFactory();
            var world = factory.GetInitialState();
            var printer = new WorldPrinter();

            printer.Print(world);
            Console.ReadKey();

            for (int round = 0; world.MakeRound(); round++)
            {
                printer.Print(world);
                Console.WriteLine($"After round {round}");
                Console.ReadKey();
            }

            Console.WriteLine();
            Console.ReadKey();
        }
    }
}
