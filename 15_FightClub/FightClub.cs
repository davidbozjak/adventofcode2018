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

            while (true)
            {
                printer.Print(world);
                Console.ReadKey();
            }

            Console.WriteLine();
            Console.ReadKey();
        }
    }
}
