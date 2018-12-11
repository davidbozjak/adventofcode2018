using System;

namespace _11_Fuel
{
    class Program
    {
        static void Main(string[] args)
        {
            const int gridSerialNumber = 2568;
            var fuelCells = new int[300, 300];
            
            for (int x = 1; x <= 300; x++)
            {
                for (int y = 1; y <= 300; y++)
                {
                    int hundrethDigit = GetPowerLevel(gridSerialNumber, x, y);

                    fuelCells[x - 1, y - 1] = hundrethDigit;
                }
            }
            
            var maxInArea = int.MinValue;
            int selectedX = int.MinValue;
            int selectedY = int.MinValue;
            int selectedSize = int.MinValue;

            for (int size = 1; size < 300; size++)
            {
                for (int x = 1; x <= 301 - size; x++)
                {
                    for (int y = 1; y <= 301 - size; y++)
                    {
                        int packLevel = 0;

                        for (int i = 0; i < size; i++)
                        {
                            for (int j = 0; j < size; j++)
                            {
                                packLevel += fuelCells[x + i - 1, y + j - 1];
                            }
                        }

                        if (packLevel > maxInArea)
                        {
                            maxInArea = packLevel;
                            selectedX = x;
                            selectedY = y;
                            selectedSize = size;
                        }
                    }
                }
            }

            Console.WriteLine($"Part2: Max: {maxInArea} at ({selectedX},{selectedY},{selectedSize})");
            Console.ReadKey();
        }

        private static int GetPowerLevel(int gridSerialNumber, int x, int y)
        {
            var rackId = x + 10;
            var powerLevel = rackId * y;
            powerLevel += gridSerialNumber;
            powerLevel *= rackId;

            int hundrethDigit = getHundrethDigit(powerLevel);
            var finalLevel = hundrethDigit - 5;

            return finalLevel;

            int getHundrethDigit(int input)
            {
                var hundreth = (input % 1000) / 100;
                
                return hundreth;
            }
        }
    }
}
