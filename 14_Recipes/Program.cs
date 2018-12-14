using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _14_Recipes
{
    class Program
    {
        static void Main(string[] args)
        {
            const int input = 635041;
            string inputStr = input.ToString();

            var recepies = new LinkedList<int>();
            var firstElf = recepies.AddLast(3);
            var secondElf = recepies.AddLast(7);

            var result = new StringBuilder(input.ToString().Length);

            while (true)
            {
                var newRecipeSum = firstElf.Value + secondElf.Value;

                foreach (var newRecipe in GetDigitsOfNum(newRecipeSum).Reverse())
                {
                    recepies.AddLast(newRecipe);

                    result.Append(newRecipe.ToString());
                    if (result.Length > inputStr.Length)
                    {
                        result.Remove(0, 1);
                    }

                    if (result.ToString() == inputStr)
                    {
                        Console.WriteLine();
                        Console.WriteLine($"Part2: {recepies.Count - inputStr.Length}");
                        Console.ReadKey();
                        return;
                    }
                }

                firstElf = GetNextRecipe(firstElf);
                secondElf = GetNextRecipe(secondElf);
            }
            
            LinkedListNode<int> GetNextRecipe(LinkedListNode<int> recipe)
            {
                for (int i = 0, count = 1 + recipe.Value; i < count; recipe = recipe.Next ?? recepies.First, i++) ;

                return recipe;
            }
        }

        static IEnumerable<int> GetDigitsOfNum(int num)
        {
            do
            {
                yield return num % 10;
                num = num / 10;
            } while (num > 0);
        }
    }
}
