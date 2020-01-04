using System;
using Recipefier.Domain.Model;
using Recipefier.Persuement;

namespace Recipefier.ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var recipePersuer = new RecipePersuer();
            RenderRecipe(recipePersuer.Persue("https://demo.wprecipemaker.com/recipe-taxonomies/"));
        }


        private static void RenderRecipe(Recipe recipe)
        {
            Console.WriteLine("Recipe: " + recipe.Name);
            Console.WriteLine("Source: " + recipe.Source);
            Console.WriteLine("Summary: " + recipe.Summary);

            foreach (var tag in recipe.Tags)
            {
                Console.WriteLine(tag.Label + ": " + tag.Value);
            }
            Console.WriteLine("Serves: " + recipe.Yield);

            foreach (var time in recipe.TimeGroup.Times)
            {
                Console.WriteLine(time.Label + ": " + time.Amount + " " + time.Unit);
            }

            Console.WriteLine("Ingredients: ");
            foreach (var group in recipe.IngredientGroups)
            {
                if (group.Label != "")
                {
                    Console.WriteLine(" - " + group.Label);
                }
                foreach (var ingredient in group.Ingredients)
                {
                    Console.WriteLine("    " + ingredient.Amount + " " + ingredient.Unit + " " + ingredient.Name);
                }
            }

            Console.WriteLine("Directions: ");
            foreach (var group in recipe.DirectionGroups)
            {
                if (group.Label != "")
                {
                    Console.WriteLine(" - " + group.Label);
                }
                foreach (var direction in group.Directions)
                {
                    Console.WriteLine("    " + direction.Text);
                }
            }

            if (recipe.Notes != "")
            {
                Console.WriteLine("Notes: ");
                Console.WriteLine("    " + recipe.Notes);
            }
        }
    }
}
