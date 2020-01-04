using OpenQA.Selenium;
using Recipefier.Domain.Model;
using System.Collections.Generic;

namespace Recipefier.Persuement.Peruser
{
    // https://demo.ziprecipes.net/best-guacamole-ever/
    // https://demo.ziprecipes.net/elegant-and-delicious-dessert/
    class ZipRecipeZip : ZipRecipesZL, IPeruser
    {
        IWebElement IPeruser.FindContainerElement(IWebDriver driver)
        {
            return puller.GetOne(driver, "zip-recipes");
        }

        string IPeruser.GetName(IWebElement container)
        {
            return puller.GetText(container, "zip-recipe-title");
        }

        string IPeruser.GetNotes(IWebElement container)
        {
            return puller.GetText(puller.GetOne(container, "notes_block"), "instructions-list");
        }
        string IPeruser.GetSummary(IWebElement container)
        {
            return puller.GetText(container, "zip-recipe-summary");
        }


        List<DirectionGroup> IPeruser.GetDirectionGroups(IWebElement container)
        {
            var directionContainer = puller.GetOne(container, "instructions_block")
                                    ?? puller.GetOne(container, "instructions-list");
            var directionElements = puller.GetMany(directionContainer, "ingredient");
            var directions = new List<Direction>();
            foreach (var element in directionElements)
            {
                directions.Add(new Direction { Text = puller.CleanText(element.Text) });
            }
            return new List<DirectionGroup>() { new DirectionGroup { Label = "", Directions = directions } };
        }

        List<IngredientGroup> IPeruser.GetIngredientGroups(IWebElement container)
        {
            var ingredientContainer = puller.GetOne(container, "ingredients_block")
                                    ?? puller.GetOne(container, "ingredients-list");
            var ingredientElements = puller.GetMany(ingredientContainer, "ingredient");
            var ingredients = new List<Ingredient>();
            foreach (var element in ingredientElements)
            {
                ingredients.Add(new Ingredient { Name = puller.CleanText(element.Text) });
            }
            return new List<IngredientGroup>() { new IngredientGroup { Label = "", Ingredients = ingredients } };
        }

    }
}
