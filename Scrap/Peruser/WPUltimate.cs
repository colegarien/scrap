using OpenQA.Selenium;
using Scrap.Model;
using Scrap.Peruser.Utilities;
using System.Collections.Generic;

namespace Scrap.Peruser
{
    // Based on https://www.wpultimaterecipe.com/docs/demo/
    class WPUltimate : IPeruser
    {
        protected Puller puller;
        public WPUltimate()
        {
            this.puller = new Puller();
        }

        IWebElement IPeruser.FindContainerElement(IWebDriver driver)
        {
            return this.puller.GetOne(driver, "wpurp-container");
        }

        string IPeruser.GetName(IWebElement container)
        {
            return this.puller.GetText(container, "wpurp-recipe-title");
        }

        string IPeruser.GetSummary(IWebElement container)
        {
            return this.puller.GetText(container, "wpurp-recipe-description");
        }

        List<Tag> IPeruser.GetTags(IWebElement container)
        {
            var tagsContainer = this.puller.GetOne(container, "wpurp-recipe-tags");
            var tagTables = this.puller.GetMany(tagsContainer, "table", PullType.BY_TAG);
            
            var tags = new List<Tag>();
            foreach (var tableElement in tagTables)
            {
                var tag = new Tag
                {
                    Label = this.puller.GetText(tableElement, "wpurp-recipe-tag-name"),
                    Value = this.puller.GetText(tableElement, "wpurp-recipe-tag-terms")
                };

                if(tag.Label != "" && tag.Value != "")
                {
                    tags.Add(tag);
                }
            }
            return tags;
        }

        string IPeruser.GetYield(IWebElement container)
        {
            return this.puller.GetAttribute(container, "advanced-adjust-recipe-servings", "data-start-servings");
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
                Amount = this.puller.GetText(container, "wpurp-recipe-prep-time"),
                Unit = this.puller.GetText(container, "wpurp-recipe-prep-time-text")
            };
            var cookTime = new Time
            {
                Label = "Cook Time",
                Amount = this.puller.GetText(container, "wpurp-recipe-cook-time"),
                Unit = this.puller.GetText(container, "wpurp-recipe-cook-time-text")
            };
            var totalTime = new Time
            {
                Label = "Total Time",
                Amount = this.puller.GetText(container, "wpurp-recipe-total-time"),
                Unit = this.puller.GetText(container, "wpurp-recipe-total-time-text")
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
            var ingredientGroupElements = this.puller.GetMany(container, "wpurp-recipe-ingredient-group-container");
            foreach (var groupElement in ingredientGroupElements)
            {
                var label = this.puller.GetText(groupElement, "wpurp-recipe-ingredient-group");

                var ingredientElements = this.puller.GetMany(groupElement, "wpurp-recipe-ingredient");
                var ingredients = new List<Ingredient>();
                foreach (var element in ingredientElements)
                {
                    var ingredient = new Ingredient
                    {
                        Amount = this.puller.GetText(element, "wpurp-recipe-ingredient-quantity"),
                        Unit = this.puller.GetText(element, "wpurp-recipe-ingredient-unit"),
                        Name = this.puller.GetText(element, "wpurp-recipe-ingredient-name"),
                        Note = this.puller.GetText(element, "wpurp-recipe-ingredient-notes"),
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
            var directionGroupElements = this.puller.GetMany(container, "wpurp-recipe-instruction-group-container");
            foreach (var groupElement in directionGroupElements)
            {
                var label = this.puller.GetText(groupElement, "wpurp-recipe-instruction-group");

                var directionElements = this.puller.GetMany(container, "wpurp-recipe-instruction");
                var directions = new List<Direction>();
                foreach (var element in directionElements)
                {
                    directions.Add(new Direction { Text = this.puller.GetText(element, "wpurp-recipe-instruction-text") });
                }
                directionGroups.Add(new DirectionGroup { Label = label, Directions = directions });
            }

            return directionGroups;
        }

        string IPeruser.GetNotes(IWebElement container)
        {
            return this.puller.GetText(container, "wpurp-recipe-notes");
        }
    }
}
