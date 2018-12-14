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
            const int minRecepiesToGenerate = input + 10;

            var recepies = new LinkedList<int>();
            var firstElf = recepies.AddLast(3);
            var secondElf = recepies.AddLast(7);

            var result = new StringBuilder(10);

            while (recepies.Count < minRecepiesToGenerate)
            {
                var newRecipeSum = firstElf.Value + secondElf.Value;

                foreach (var newRecipe in GetDigitsOfNum(newRecipeSum).Reverse())
                {
                    recepies.AddLast(newRecipe);

                    if (recepies.Count > input && recepies.Count <= minRecepiesToGenerate)
                    {
                        result.Append(newRecipe.ToString());
                    }
                }

                firstElf = GetNextRecipe(firstElf);
                secondElf = GetNextRecipe(secondElf);
            }
            
            Console.WriteLine();
            Console.WriteLine($"Part1: {result}");
            Console.ReadKey();

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
