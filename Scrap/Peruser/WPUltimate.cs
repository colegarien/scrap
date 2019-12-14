using OpenQA.Selenium;
using Scrap.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Scrap.Peruser
{
    class WPUltimate : IPeruser
    {
        // Based on https://www.wpultimaterecipe.com/docs/demo/
        public static bool CanPersue(IWebDriver driver)
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
            return driver.FindElement(By.ClassName("wpurp-container"));
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

        public Recipe Peruse(IWebDriver driver)
        {
            var container = FindContainerElement(driver);

            return new Recipe
            {
                Name = GetGuts(container, "wpurp-recipe-title"),
                Source = driver.Url,
                Summary = GetGuts(container, "wpurp-recipe-description"),
                Tags = GetTags(container),
                ServingSize = GetServingSize(container),
                TimeGroup = GetTimeGroup(container),
                IngredientGroups = GetIngredientGroups(container),
                DirectionGroups = GetDirectionGroups(container),
                Notes = GetGuts(container, "wpurp-recipe-notes")
            };
        }

        private string GetServingSize(IWebElement container)
        {
            return container.FindElements(By.ClassName("advanced-adjust-recipe-servings"))
                    .FirstOrDefault()?.GetAttribute("data-start-servings")
                    ?? "";
        }

        private List<Tag> GetTags(IWebElement container)
        {
            var tagsContainer = container.FindElement(By.ClassName("wpurp-recipe-tags"));
            var tagTables = tagsContainer.FindElements(By.TagName("table"));
            
            var tags = new List<Tag>();
            foreach (var tableElement in tagTables)
            {
                var tag = new Tag
                {
                    Label = GetGuts(tableElement, "wpurp-recipe-tag-name"),
                    Value = GetGuts(tableElement, "wpurp-recipe-tag-terms")
                };

                if(tag.Label != "" && tag.Value != "")
                {
                    tags.Add(tag);
                }
            }
            return tags;
        }

        private TimeGroup GetTimeGroup(IWebElement container)
        {
            var timeGroup = new TimeGroup
            {
                Times = new List<Time>()
            };

            var prepTime = new Time
            {
                Label = "Prep Time",
                Amount = GetGuts(container, "wpurp-recipe-prep-time"),
                Unit = GetGuts(container, "wpurp-recipe-prep-time-text")
            };
            var cookTime = new Time
            {
                Label = "Cook Time",
                Amount = GetGuts(container, "wpurp-recipe-cook-time"),
                Unit = GetGuts(container, "wpurp-recipe-cook-time-text")
            };
            var totalTime = new Time
            {
                Label = "Total Time",
                Amount = GetGuts(container, "wpurp-recipe-total-time"),
                Unit = GetGuts(container, "wpurp-recipe-total-time-text")
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

        private List<IngredientGroup> GetIngredientGroups(IWebElement container)
        {
            var ingredientGroups = new List<IngredientGroup>();
            var ingredientGroupElements = container.FindElements(By.ClassName("wpurp-recipe-ingredient-container"));
            foreach (var groupElement in ingredientGroupElements)
            {
                var label = GetGuts(groupElement, "wpurp-recipe-ingredient-group");

                var ingredientElements = groupElement.FindElements(By.ClassName("wpurp-recipe-ingredient"));
                var ingredients = new List<Ingredient>();
                foreach (var element in ingredientElements)
                {
                    ingredients.Add(new Ingredient
                    {
                        Amount = GetGuts(element, "wpurp-recipe-ingredient-quantity"),
                        Unit = GetGuts(element, "wpurp-recipe-ingredient-unit"),
                        Name = GetGuts(element, "wpurp-recipe-ingredient-name"),
                        Note = GetGuts(element, "wpurp-recipe-ingredient-notes"),
                    });
                }
                ingredientGroups.Add(new IngredientGroup { Label = label, Ingredients = ingredients });
            }

            return ingredientGroups;
        }

        private List<DirectionGroup> GetDirectionGroups(IWebElement container)
        {
            var directionGroups = new List<DirectionGroup>();
            var directionGroupElements = container.FindElements(By.ClassName("wpurp-recipe-instruction-group-container"));
            foreach (var groupElement in directionGroupElements)
            {
                var label = GetGuts(groupElement, "wpurp-recipe-instruction-group");

                var directionElements = container.FindElements(By.ClassName("wpurp-recipe-instruction"));
                var directions = new List<Direction>();
                foreach (var element in directionElements)
                {
                    directions.Add(new Direction { Text = GetGuts(element, "wpurp-recipe-instruction-text") });
                }
                directionGroups.Add(new DirectionGroup { Label = label, Directions = directions });
            }

            return directionGroups;
        }
    }
}
