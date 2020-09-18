using System;
using System.Linq;
using Recipefier.Domain.Model;
using Recipefier.Persuement;
using Recipefier.Persuement.Exception;

namespace Recipefier.ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var recipePersuer = new RecipePersuer();

            var urls = new string[]
            {
                "https://demos.boxystudio.com/cooked/recipe/peanut-butter-sandwich-cookies/",
                "https://demos.boxystudio.com/cooked/recipe/brisket-root-vegetables/",
                "https://demos.boxystudio.com/cooked/recipe/sausage-hash-brown-casserole/",
                "https://demo.wpzoom.com/recipe-card-blocks/2019/03/26/new-style-design/",
                "https://demo.wpzoom.com/recipe-card-blocks/2019/03/26/kid-friendly-oil-free-vegan-pancakes/",
                "https://demo.wpzoom.com/recipe-card-blocks/2019/02/06/recipe-card-classic-style/",
                "https://cookieandkate.com/best-carrot-cake-recipe/#tasty-recipes-33706",
                "https://cookiesandcups.com/perfect-snickerdoodles/",
                "https://pinchofyum.com/sweet-potato-peanut-soup",
                "https://thesaltymarshmallow.com/homemade-belgian-waffle-recipe/",
                "https://demo.wprecipemaker.com/adjustable-servings/",
                "https://demo.wprecipemaker.com/recipe-taxonomies/",
                "https://www.wpultimaterecipe.com/docs/demo/",
                "https://demo.ziprecipes.net/tres-leches/",
                "https://demo.ziprecipes.net/best-guacamole-ever/",
                "https://demo.ziprecipes.net/elegant-and-delicious-dessert/",
                "https://toriavey.com/toris-kitchen/falafel/"
            };

            foreach (var url in urls)
            {
                Console.WriteLine("------------ " + url + " ------------");
                try
                {
                    RenderRecipe(recipePersuer.Persue(url));
                }
                catch (CouldNotPersueException e)
                {
                    Console.WriteLine(e.ToString());
                }
                Console.WriteLine("------------ ------------------ ------------");
            }
        }


        private static void RenderRecipe(Recipe recipe, bool verbose = false)
        {
            Console.WriteLine("+----- TEST -----+");
            Console.WriteLine("| Has Name:    "+(recipe.Name.Length > 0 ? "T" : "F")+" |");
            Console.WriteLine("| Directions:  " + (recipe.DirectionGroups.All(g => g.Directions.Count > 0) ? "T" : "F") + " |");
            Console.WriteLine("| Ingredient:  " + (recipe.IngredientGroups.All(g => g.Ingredients.Count > 0) ? "T" : "F") + " |");
            Console.WriteLine("+----- ++++ -----+");

            if (verbose)
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
}
