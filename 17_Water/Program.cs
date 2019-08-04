using SantasToolbox;
using System;
using System.Threading.Tasks;

namespace _17_Water
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new WorldFactory();
            var world = factory.GetWorld();
            var printer = new WorldPrinter(false, 10);
            printer.Print(world);

            while (world.MakeStep(o => ObserveWorld(printer, world, o))) ;
        }

        private static void ObserveWorld(WorldPrinter printer, World world, IWorldObject objectOfInterest)
        {
            printer.Print(world, objectOfInterest);
            Console.WriteLine($"Wet tiles: {world.NumberOfWetTiles}");
            //Console.ReadKey();
            Task.Delay(50).Wait();
        }
    }
}
