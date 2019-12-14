﻿using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace Scrap
{
    class Program
    {

        static void Main(string[] args)
        {
            var options = new ChromeOptions();
            options.AddArgument("disable-translate");
            options.AddArgument("disable-infobars");
            options.AddArgument("headless");
            options.AddArgument("disable-gpu");
            options.AddArgument("window-size=1024,768");
            var serice = ChromeDriverService.CreateDefaultService(@"C:\\Program Files (x86)\\Google\\", "chromedriver.exe");
            IWebDriver driver = new ChromeDriver(serice, options);

            var targetRecipeUrl = "https://www.wpultimaterecipe.com/docs/demo/";
            driver.Navigate().GoToUrl(targetRecipeUrl);

            var persuerFactory = new Peruser.Factory();
            var peruser = persuerFactory.GetPeruser(driver);
            
            var recipe = peruser.Peruse(driver);
            RenderRecipe(recipe);

            driver.Close();
        }

        private static void RenderRecipe(Model.Recipe recipe)
        {
            Console.WriteLine("Recipe: " + recipe.Name);
            Console.WriteLine("Source: " + recipe.Source);
            Console.WriteLine("Summary: " + recipe.Summary);

            foreach (var tag in recipe.Tags)
            {
                Console.WriteLine(tag.Label + ": " + tag.Value);
            }
            Console.WriteLine("Serving Size: " + recipe.ServingSize);

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
            Console.WriteLine("Notes: " + recipe.Notes);
        }
    }
}