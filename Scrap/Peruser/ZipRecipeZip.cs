using OpenQA.Selenium;
using Scrap.Model;
using System.Collections.Generic;

namespace Scrap.Peruser
{
    // https://demo.ziprecipes.net/best-guacamole-ever/
    // https://demo.ziprecipes.net/elegant-and-delicious-dessert/
    class ZipRecipeZip : ZipRecipesZL, IPeruser
    {
        IWebElement IPeruser.FindContainerElement(IWebDriver driver)
        {
            return this.puller.GetOne(driver, "zip-recipes");
        }

        string IPeruser.GetName(IWebElement container)
        {
            return this.puller.GetText(container, "zip-recipe-title");
        }

        string IPeruser.GetNotes(IWebElement container)
        {
            return this.puller.GetText(this.puller.GetOne(container, "notes_block"), "instructions-list");
        }
        string IPeruser.GetSummary(IWebElement container)
        {
            return this.puller.GetText(container, "zip-recipe-summary");
        }


        List<DirectionGroup> IPeruser.GetDirectionGroups(IWebElement container)
        {
            var directionContainer = this.puller.GetOne(container, "instructions_block")
                                    ?? this.puller.GetOne(container, "instructions-list");
            var directionElements = this.puller.GetMany(directionContainer, "ingredient");
            var directions = new List<Direction>();
            foreach (var element in directionElements)
            {
                directions.Add(new Direction { Text = this.puller.CleanText(element.Text) });
            }
            return new List<DirectionGroup>() { new DirectionGroup { Label = "", Directions = directions } };
        }

        List<IngredientGroup> IPeruser.GetIngredientGroups(IWebElement container)
        {
            var ingredientContainer = this.puller.GetOne(container, "ingredients_block") 
                                    ?? this.puller.GetOne(container, "ingredients-list");
            var ingredientElements = this.puller.GetMany(ingredientContainer, "ingredient");
            var ingredients = new List<Ingredient>();
            foreach (var element in ingredientElements)
            {
                ingredients.Add(new Ingredient { Name = this.puller.CleanText(element.Text) });
            }
            return new List<IngredientGroup>() { new IngredientGroup { Label = "", Ingredients = ingredients } };
        }

    }
}
