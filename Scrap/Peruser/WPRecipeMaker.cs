﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using Scrap.Model;

namespace Scrap.Peruser
{

    // Based on https://thesaltymarshmallow.com/homemade-belgian-waffle-recipe/
    // Also https://demo.wprecipemaker.com/adjustable-servings/
    class WPRecipeMaker : IPeruser
    {
        IWebElement IPeruser.FindContainerElement(IWebDriver driver)
        {
            return driver.FindElement(By.ClassName("wprm-recipe-container"));
        }

        string IPeruser.GetName(IWebElement container)
        {
            return GetGuts(container, "wprm-recipe-name");
        }

        string IPeruser.GetSummary(IWebElement container)
        {
            return GetGuts(container, "wprm-recipe-summary");
        }

        List<Tag> IPeruser.GetTags(IWebElement container)
        {
            var tags = new List<Tag>();
            var courseTag = new Tag
            {
                Label = "Course",
                Value = GetGuts(container, "wprm-recipe-course")
            };
            var cuisineTag = new Tag
            {
                Label = "Cuisine",
                Value = GetGuts(container, "wprm-recipe-cuisine")
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

        string IPeruser.GetServingSize(IWebElement container)
        {
            return GetGuts(container, "wprm-recipe-servings");
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
                Amount = GetGuts(container, "wprm-recipe-prep_time"),
                Unit = GetGuts(container, "wprm-recipe-prep_time-unit")
            };
            var cookTime = new Time
            {
                Label = "Cook Time",
                Amount = GetGuts(container, "wprm-recipe-cook_time"),
                Unit = GetGuts(container, "wprm-recipe-cook_time-unit")
            };
            var totalTime = new Time
            {
                Label = "Total Time",
                Amount = GetGuts(container, "wprm-recipe-total_time"),
                Unit = GetGuts(container, "wprm-recipe-total_time-unit")
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
            var ingredientGroupElements = container.FindElements(By.ClassName("wprm-recipe-ingredient-group"));
            foreach (var groupElement in ingredientGroupElements)
            {
                var label = GetGuts(groupElement, "wprm-recipe-group-name");

                var ingredientElements = groupElement.FindElements(By.ClassName("wprm-recipe-ingredient"));
                var ingredients = new List<Ingredient>();
                foreach (var element in ingredientElements)
                {
                    ingredients.Add(new Ingredient
                    {
                        Amount = GetGuts(element, "wprm-recipe-ingredient-amount"),
                        Unit = GetGuts(element, "wprm-recipe-ingredient-unit"),
                        Name = GetGuts(element, "wprm-recipe-ingredient-name"),
                        Note = GetGuts(element, "wprm-recipe-ingredient-notes"),
                    });
                }
                ingredientGroups.Add(new IngredientGroup { Label = label, Ingredients = ingredients });
            }

            return ingredientGroups;
        }

        List<DirectionGroup> IPeruser.GetDirectionGroups(IWebElement container)
        {
            var directionGroups = new List<DirectionGroup>();
            var directionGroupElements = container.FindElements(By.ClassName("wprm-recipe-instruction-group"));
            foreach (var groupElement in directionGroupElements)
            {
                var label = GetGuts(groupElement, "wprm-recipe-group-name");

                var directionElements = container.FindElements(By.ClassName("wprm-recipe-instruction"));
                var directions = new List<Direction>();
                foreach (var element in directionElements)
                {
                    directions.Add(new Direction { Text = GetGuts(element, "wprm-recipe-instruction-text") });
                }
                directionGroups.Add(new DirectionGroup { Label = label, Directions = directions });
            }

            return directionGroups;
        }

        string IPeruser.GetNotes(IWebElement container)
        {
            return GetGuts(container, "wprm-recipe-notes");
        }




        private string GetGuts(IWebElement element, string className)
        {
            var firstElement = element.FindElements(By.ClassName(className))
                .FirstOrDefault();
            var text = "";
            if (firstElement != null)
            {
                text = firstElement.FindElements(By.ClassName(className))
                    .FirstOrDefault()?.GetAttribute("innerHTML")
                    ?? firstElement.GetAttribute("innerHTML");
            }

            text = text.Replace("&amp;", "&")
                .Replace("&nbsp;", " ")
                .Replace("<span style=\"display: block;\">", "")
                .Replace("</span>", "")
                .Replace("</a>", "")
                .Replace("<p>", "")
                .Replace("</p>", "")
                .Replace("<br>", "")
                .Replace("</br>", "")
                .Replace("  ", " ")
                .Trim();

            // remove links
            text = Regex.Replace(text, "<a .*>", "");

            return text;
        }
    }
}
