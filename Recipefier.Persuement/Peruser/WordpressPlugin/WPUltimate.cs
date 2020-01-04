using OpenQA.Selenium;
using Recipefier.Domain.Model;
using Recipefier.Persuement.Peruser.Utilities;
using System.Collections.Generic;

namespace Recipefier.Persuement.Peruser.WordpressPlugin
{
    // Based on https://www.wpultimaterecipe.com/docs/demo/
    class WPUltimate : IPeruser
    {
        protected readonly Puller puller;
        public WPUltimate()
        {
            puller = new Puller();
        }

        IWebElement IPeruser.FindContainerElement(IWebDriver driver)
        {
            return puller.GetOne(driver, "wpurp-container");
        }

        string IPeruser.GetName(IWebElement container)
        {
            return puller.GetText(container, "wpurp-recipe-title");
        }

        string IPeruser.GetSummary(IWebElement container)
        {
            return puller.GetText(container, "wpurp-recipe-description");
        }

        List<Tag> IPeruser.GetTags(IWebElement container)
        {
            var tagsContainer = puller.GetOne(container, "wpurp-recipe-tags");
            var tagTables = puller.GetMany(tagsContainer, "table", PullType.BY_TAG);

            var tags = new List<Tag>();
            foreach (var tableElement in tagTables)
            {
                var tag = new Tag
                {
                    Label = puller.GetText(tableElement, "wpurp-recipe-tag-name"),
                    Value = puller.GetText(tableElement, "wpurp-recipe-tag-terms")
                };

                if (tag.Label != "" && tag.Value != "")
                {
                    tags.Add(tag);
                }
            }
            return tags;
        }

        string IPeruser.GetYield(IWebElement container)
        {
            return puller.GetAttribute(container, "advanced-adjust-recipe-servings", "data-start-servings");
        }

        TimeGroup IPeruser.GetTimeGroup(IWebElement container)
        {
            var timeGroup = new TimeGroup
            {
                Times = new List<Time>()
            };

            var prepTime = new Time
            {
                Label = "Prep Time",
                Amount = puller.GetText(container, "wpurp-recipe-prep-time"),
                Unit = puller.GetText(container, "wpurp-recipe-prep-time-text")
            };
            var cookTime = new Time
            {
                Label = "Cook Time",
                Amount = puller.GetText(container, "wpurp-recipe-cook-time"),
                Unit = puller.GetText(container, "wpurp-recipe-cook-time-text")
            };
            var totalTime = new Time
            {
                Label = "Total Time",
                Amount = puller.GetText(container, "wpurp-recipe-total-time"),
                Unit = puller.GetText(container, "wpurp-recipe-total-time-text")
            };

            if (prepTime.Amount != "")
            {
                timeGroup.Times.Add(prepTime);
            }
            if (cookTime.Amount != "")
            {
                timeGroup.Times.Add(cookTime);
            }
            if (totalTime.Amount != "")
            {
                timeGroup.Times.Add(totalTime);
            }

            return timeGroup;
        }

        List<IngredientGroup> IPeruser.GetIngredientGroups(IWebElement container)
        {
            var ingredientGroups = new List<IngredientGroup>();
            var ingredientGroupElements = puller.GetMany(container, "wpurp-recipe-ingredient-group-container");
            foreach (var groupElement in ingredientGroupElements)
            {
                var label = puller.GetText(groupElement, "wpurp-recipe-ingredient-group");

                var ingredientElements = puller.GetMany(groupElement, "wpurp-recipe-ingredient");
                var ingredients = new List<Ingredient>();
                foreach (var element in ingredientElements)
                {
                    var ingredient = new Ingredient
                    {
                        Amount = puller.GetText(element, "wpurp-recipe-ingredient-quantity"),
                        Unit = puller.GetText(element, "wpurp-recipe-ingredient-unit"),
                        Name = puller.GetText(element, "wpurp-recipe-ingredient-name"),
                        Note = puller.GetText(element, "wpurp-recipe-ingredient-notes"),
                    };

                    if (ingredient.Amount != "" || ingredient.Unit != "" || ingredient.Name != "" || ingredient.Note != "")
                    {
                        ingredients.Add(ingredient);
                    }
                }
                if (ingredients.Count > 0)
                {
                    ingredientGroups.Add(new IngredientGroup { Label = label, Ingredients = ingredients });
                }
            }

            return ingredientGroups;
        }

        List<DirectionGroup> IPeruser.GetDirectionGroups(IWebElement container)
        {
            var directionGroups = new List<DirectionGroup>();
            var directionGroupElements = puller.GetMany(container, "wpurp-recipe-instruction-group-container");
            foreach (var groupElement in directionGroupElements)
            {
                var label = puller.GetText(groupElement, "wpurp-recipe-instruction-group");

                var directionElements = puller.GetMany(container, "wpurp-recipe-instruction");
                var directions = new List<Direction>();
                foreach (var element in directionElements)
                {
                    directions.Add(new Direction { Text = puller.GetText(element, "wpurp-recipe-instruction-text") });
                }
                directionGroups.Add(new DirectionGroup { Label = label, Directions = directions });
            }

            return directionGroups;
        }

        string IPeruser.GetNotes(IWebElement container)
        {
            return puller.GetText(container, "wpurp-recipe-notes");
        }
    }
}
