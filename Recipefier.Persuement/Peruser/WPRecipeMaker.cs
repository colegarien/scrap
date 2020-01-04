using System.Collections.Generic;
using OpenQA.Selenium;
using Recipefier.Persuement.Model;
using Recipefier.Persuement.Peruser.Utilities;

namespace Recipefier.Persuement.Peruser
{

    // Based on https://thesaltymarshmallow.com/homemade-belgian-waffle-recipe/
    // Also https://demo.wprecipemaker.com/adjustable-servings/
    // and https://demo.wprecipemaker.com/recipe-taxonomies/
    class WPRecipeMaker : IPeruser
    {
        protected Puller puller;
        public WPRecipeMaker()
        {
            puller = new Puller();
        }

        IWebElement IPeruser.FindContainerElement(IWebDriver driver)
        {
            return puller.GetOne(driver, "wprm-recipe-container");
        }

        string IPeruser.GetName(IWebElement container)
        {
            return puller.GetText(container, "wprm-recipe-name");
        }

        string IPeruser.GetSummary(IWebElement container)
        {
            return puller.GetText(container, "wprm-recipe-summary");
        }

        List<Tag> IPeruser.GetTags(IWebElement container)
        {
            var tags = new List<Tag>();
            var courseTag = new Tag
            {
                Label = "Course",
                Value = puller.GetText(container, "wprm-recipe-course")
            };
            var cuisineTag = new Tag
            {
                Label = "Cuisine",
                Value = puller.GetText(container, "wprm-recipe-cuisine")
            };

            if (courseTag.Value != "")
            {
                tags.Add(courseTag);
            }
            if (cuisineTag.Value != "")
            {
                tags.Add(cuisineTag);
            }

            return tags;
        }

        string IPeruser.GetYield(IWebElement container)
        {
            return puller.GetText(container, "wprm-recipe-servings");
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
                Amount = puller.GetText(container, "wprm-recipe-prep_time"),
                Unit = puller.GetText(container, "wprm-recipe-prep_time-unit")
            };
            var cookTime = new Time
            {
                Label = "Cook Time",
                Amount = puller.GetText(container, "wprm-recipe-cook_time"),
                Unit = puller.GetText(container, "wprm-recipe-cook_time-unit")
            };
            var totalTime = new Time
            {
                Label = "Total Time",
                Amount = puller.GetText(container, "wprm-recipe-total_time"),
                Unit = puller.GetText(container, "wprm-recipe-total_time-unit")
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
            var ingredientGroupElements = puller.GetMany(container, "wprm-recipe-ingredient-group");
            foreach (var groupElement in ingredientGroupElements)
            {
                var label = puller.GetText(groupElement, "wprm-recipe-group-name");

                var ingredientElements = puller.GetMany(groupElement, "wprm-recipe-ingredient");
                var ingredients = new List<Ingredient>();
                foreach (var element in ingredientElements)
                {
                    ingredients.Add(new Ingredient
                    {
                        Amount = puller.GetText(element, "wprm-recipe-ingredient-amount"),
                        Unit = puller.GetText(element, "wprm-recipe-ingredient-unit"),
                        Name = puller.GetText(element, "wprm-recipe-ingredient-name"),
                        Note = puller.GetText(element, "wprm-recipe-ingredient-notes"),
                    });
                }
                ingredientGroups.Add(new IngredientGroup { Label = label, Ingredients = ingredients });
            }

            return ingredientGroups;
        }

        List<DirectionGroup> IPeruser.GetDirectionGroups(IWebElement container)
        {
            var directionGroups = new List<DirectionGroup>();
            var directionGroupElements = puller.GetMany(container, "wprm-recipe-instruction-group");
            foreach (var groupElement in directionGroupElements)
            {
                var label = puller.GetText(groupElement, "wprm-recipe-group-name");

                var directionElements = puller.GetMany(container, "wprm-recipe-instruction");
                var directions = new List<Direction>();
                foreach (var element in directionElements)
                {
                    directions.Add(new Direction { Text = puller.GetText(element, "wprm-recipe-instruction-text") });
                }
                directionGroups.Add(new DirectionGroup { Label = label, Directions = directions });
            }

            return directionGroups;
        }

        string IPeruser.GetNotes(IWebElement container)
        {
            return puller.GetText(container, "wprm-recipe-notes");
        }
    }
}
