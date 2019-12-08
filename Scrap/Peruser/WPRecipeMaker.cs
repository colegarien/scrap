using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace Scrap.Peruser
{
    class WPRecipeMaker : IPeruser
    {

        // Based on https://thesaltymarshmallow.com/homemade-belgian-waffle-recipe/
        public static bool CanPeruse(IWebDriver driver)
        {
            try
            {
                FindContainerElement(driver);
            } 
            catch (NoSuchElementException)
            {
                return false;
            }

            return true;
        }

        private static IWebElement FindContainerElement(IWebDriver driver)
        {
            return driver.FindElement(By.ClassName("wprm-recipe-container"));
        }

        public void Peruse(IWebDriver driver)
        {
            var container = FindContainerElement(driver);

            var source = driver.Url;
            var name = container.FindElement(By.ClassName("wprm-recipe-name")).GetAttribute("innerHTML").Trim();
            var summary = container.FindElement(By.ClassName("wprm-recipe-summary")).GetAttribute("innerHTML").Trim();
            var notes = container.FindElement(By.ClassName("wprm-recipe-notes")).GetAttribute("innerHTML").Trim();

            var course = container.FindElement(By.ClassName("wprm-recipe-course")).GetAttribute("innerHTML").Trim();
            var cuisine = container.FindElement(By.ClassName("wprm-recipe-cuisine")).GetAttribute("innerHTML").Trim();
            var servingSize = container.FindElement(By.ClassName("wprm-recipe-servings")).GetAttribute("innerHTML").Trim();

            var prepTime = new Model.Time { 
                Amount = container.FindElement(By.ClassName("wprm-recipe-prep_time")).GetAttribute("innerHTML").Trim(),
                Unit = container.FindElement(By.ClassName("wprm-recipe-prep_time-unit")).GetAttribute("innerHTML").Trim()
            };
            var cookTime = new Model.Time
            {
                Amount = container.FindElement(By.ClassName("wprm-recipe-cook_time")).GetAttribute("innerHTML").Trim(),
                Unit = container.FindElement(By.ClassName("wprm-recipe-cook_time-unit")).GetAttribute("innerHTML").Trim()
            };
            var totalTime = new Model.Time
            {
                Amount = container.FindElement(By.ClassName("wprm-recipe-total_time")).GetAttribute("innerHTML").Trim(),
                Unit = container.FindElement(By.ClassName("wprm-recipe-total_time-unit")).GetAttribute("innerHTML").Trim()
            };

            var ingredientElements = container.FindElements(By.ClassName("wprm-recipe-ingredient"));
            var ingredients = new List<Model.Ingredient>();
            foreach (var element in ingredientElements)
            {
                ingredients.Add(new Model.Ingredient
                {
                    Amount = element.FindElement(By.ClassName("wprm-recipe-ingredient-amount")).GetAttribute("innerHTML").Trim(),
                    Unit = element.FindElements(By.ClassName("wprm-recipe-ingredient-unit")).FirstOrDefault()?.GetAttribute("innerHTML").Trim() ?? "",
                    Name = element.FindElement(By.ClassName("wprm-recipe-ingredient-name")).GetAttribute("innerHTML").Trim()
                });
            }

            var directionElements = container.FindElements(By.ClassName("wprm-recipe-instruction"));
            var directions = new List<string>();
            foreach (var element in directionElements)
            {
                directions.Add(element.FindElement(By.ClassName("wprm-recipe-instruction-text")).GetAttribute("innerHTML").Trim());
            }

            Console.WriteLine("Recipe: " + name);
            Console.WriteLine("Source: " + source);
            Console.WriteLine("Summary: " + summary);

            Console.WriteLine("Course: " + course);
            Console.WriteLine("Cuisine: " + cuisine);
            Console.WriteLine("Serving Size: " + servingSize);

            Console.WriteLine("Prep Time: " + prepTime.Amount + " " + prepTime.Unit);
            Console.WriteLine("Cook Time: " + cookTime.Amount + " " + cookTime.Unit);
            Console.WriteLine("Total Time: " + totalTime.Amount + " " + totalTime.Unit);

            Console.WriteLine("Ingredients: ");
            foreach(var ingredient in ingredients)
            {
                Console.WriteLine("    " + ingredient.Amount + " " + ingredient.Unit + " " + ingredient.Name);
            }

            Console.WriteLine("Directions: ");
            foreach (var direction in directions)
            {
                Console.WriteLine("    " + direction);
            }
            Console.WriteLine("Notes: " + notes);

        }
    }
}
