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

            for (int round = 0; ; round++)
            {
                world.MakeRound();

                printer.Print(world);
                Console.ReadKey();
            }

            Console.WriteLine();
            Console.ReadKey();
        }
    }
}
