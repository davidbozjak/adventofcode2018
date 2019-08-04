using SantasToolbox;
using System;

namespace _17_Water
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new WorldFactory();
            var world = factory.GetWorld();
            var printer = new WorldPrinter();
            printer.Print(world);

            while (world.MakeStep(o => ObserveWorld(printer, world, o))) ;
        }

        private static void ObserveWorld(WorldPrinter printer, IWorld world, IWorldObject objectOfInterest)
        {
            printer.Print(world, objectOfInterest);
            Console.ReadKey();
        }
    }
}
