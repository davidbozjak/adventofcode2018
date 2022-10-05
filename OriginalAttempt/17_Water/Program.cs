using SantasToolbox;
using System;
using System.Threading.Tasks;

namespace _17_Water
{
    class Program
    {
        static int LastNumber;
        static void Main(string[] args)
        {
            var factory = new WorldFactory();
            var world = factory.GetWorld();
            var printer = new WorldPrinter(false, 10);
            printer.Print(world);

            while (world.MakeStep(o => ObserveWorld(printer, world, o))) ;

            printer.Print(world);
            Console.WriteLine($"Wet tiles: {world.NumberOfWetTiles}");
        }

        private static void ObserveWorld(WorldPrinter printer, World world, IWorldObject objectOfInterest)
        {
            var number = world.NumberOfWetTiles;

            if (number != LastNumber)
            {
                LastNumber = number;
                printer.Print(world, objectOfInterest);
                Console.WriteLine($"Wet tiles: {LastNumber}");
            }

            //printer.Print(world, objectOfInterest);
            
            //Console.ReadKey();
            //Task.Delay(10).Wait();
        }
    }
}
