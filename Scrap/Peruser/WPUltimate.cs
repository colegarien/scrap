using OpenQA.Selenium;
using Scrap.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Scrap.Peruser
{
    // Based on https://www.wpultimaterecipe.com/docs/demo/
    class WPUltimate : IPeruser
    {
        IWebElement IPeruser.FindContainerElement(IWebDriver driver)
        {
            return driver.FindElement(By.ClassName("wpurp-container"));
        }

        string IPeruser.GetName(IWebElement container)
        {
            return GetGuts(container, "wpurp-recipe-title");
        }

        string IPeruser.GetSummary(IWebElement container)
        {
            return GetGuts(container, "wpurp-recipe-description");
        }

        List<Tag> IPeruser.GetTags(IWebElement container)
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

        string IPeruser.GetServingSize(IWebElement container)
        {
            return container.FindElements(By.ClassName("advanced-adjust-recipe-servings"))
                    .FirstOrDefault()?.GetAttribute("data-start-servings")
                    ?? "";
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

        List<IngredientGroup> IPeruser.GetIngredientGroups(IWebElement container)
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

        List<DirectionGroup> IPeruser.GetDirectionGroups(IWebElement container)
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

        string IPeruser.GetNotes(IWebElement container)
        {
            return GetGuts(container, "wpurp-recipe-notes");
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
