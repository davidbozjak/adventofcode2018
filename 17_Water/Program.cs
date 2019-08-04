using SantasToolbox;

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
        }
    }
}
