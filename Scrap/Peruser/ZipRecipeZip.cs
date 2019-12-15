using OpenQA.Selenium;
using Scrap.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scrap.Peruser
{
    // https://demo.ziprecipes.net/best-guacamole-ever/
    // https://demo.ziprecipes.net/elegant-and-delicious-dessert/
    class ZipRecipeZip : ZipRecipesZL, IPeruser
    {
        IWebElement IPeruser.FindContainerElement(IWebDriver driver)
        {
            return driver.FindElement(By.ClassName("zip-recipes"));
        }

        string IPeruser.GetName(IWebElement container)
        {
            return CleanText(container.FindElement(By.ClassName("zip-recipe-title")).Text);
        }

        string IPeruser.GetNotes(IWebElement container)
        {
            return CleanText(container.FindElements(By.ClassName("notes_block")).FirstOrDefault()?.FindElement(By.ClassName("instructions-list")).Text ?? "");
        }
        string IPeruser.GetSummary(IWebElement container)
        {
            return GetGuts(container, "zip-recipe-summary");
        }


        List<DirectionGroup> IPeruser.GetDirectionGroups(IWebElement container)
        {
            var directionContainer = container.FindElements(By.ClassName("instructions_block")).FirstOrDefault()
                ?? container.FindElement(By.ClassName("instructions-list"));
            var directionElements = directionContainer.FindElements(By.ClassName("ingredient"));
            var directions = new List<Direction>();
            foreach (var element in directionElements)
            {
                directions.Add(new Direction { Text = CleanText(element.Text) });
            }
            return new List<DirectionGroup>() { new DirectionGroup { Label = "", Directions = directions } };
        }

        List<IngredientGroup> IPeruser.GetIngredientGroups(IWebElement container)
        {
            var ingredientContainer = container.FindElements(By.ClassName("ingredients_block")).FirstOrDefault() 
                ?? container.FindElement(By.ClassName("ingredients-list"));
            var ingredientElements = ingredientContainer.FindElements(By.ClassName("ingredient"));
            var ingredients = new List<Ingredient>();
            foreach (var element in ingredientElements)
            {
                ingredients.Add(new Ingredient { Name = CleanText(element.Text) });
            }
            return new List<IngredientGroup>() { new IngredientGroup { Label = "", Ingredients = ingredients } };
        }

    }
}
