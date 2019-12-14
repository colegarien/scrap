using OpenQA.Selenium;
using Scrap.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Scrap.Peruser
{
    // BASED on https://cookieandkate.com/best-carrot-cake-recipe/#tasty-recipes-33706
    // AND https://cookiesandcups.com/perfect-snickerdoodles/
    // ALSO https://pinchofyum.com/sweet-potato-peanut-soup
    class TastyRecipes : IPeruser
    {
        IWebElement IPeruser.FindContainerElement(IWebDriver driver)
        {
            return driver.FindElement(By.ClassName("tasty-recipes"));
        }

        string IPeruser.GetName(IWebElement container)
        {
            return container.FindElement(By.TagName("h2")).GetAttribute("innerHTML");
        }

        string IPeruser.GetNotes(IWebElement container)
        {
            return GetGuts(container, "tasty-recipes-notes");
        }

        string IPeruser.GetSummary(IWebElement container)
        {
            return GetGuts(container, "tasty-recipes-description");
        }

        List<Tag> IPeruser.GetTags(IWebElement container)
        {
            var tags = new List<Tag>();
            var categoryTag = new Tag
            {
                Label = "Category",
                Value = GetGuts(container, "tasty-recipes-category")
            };
            var methodTag = new Tag
            {
                Label = "Method",
                Value = GetGuts(container, "tasty-recipes-method")
            };
            var cuisineTag = new Tag
            {
                Label = "Cuisine",
                Value = GetGuts(container, "tasty-recipes-cuisine")
            };

            if (categoryTag.Value != "")
            {
                tags.Add(categoryTag);
            }
            if (methodTag.Value != "")
            {
                tags.Add(methodTag);
            }
            if (cuisineTag.Value != "")
            {
                tags.Add(cuisineTag);
            }

            return tags;
        }

        string IPeruser.GetServingSize(IWebElement container)
        {
            return GetGuts(container, "tasty-recipes-yield");
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
                Amount = GetGuts(container, "tasty-recipes-prep-time"),
                Unit = ""
            };
            var cookTime = new Time
            {
                Label = "Cook Time",
                Amount = GetGuts(container, "tasty-recipes-cook-time"),
                Unit = ""
            };
            var totalTime = new Time
            {
                Label = "Total Time",
                Amount = GetGuts(container, "tasty-recipes-total-time"),
                Unit = ""
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

        List<DirectionGroup> IPeruser.GetDirectionGroups(IWebElement container)
        {
            var directionGroups = new List<DirectionGroup>();
            var directionGroupElements = container.FindElements(By.ClassName("tasty-recipe-instructions"));
            foreach (var groupElement in directionGroupElements)
            {
                var label = "";

                var directionElements = groupElement.FindElements(By.TagName("li"));
                var directions = new List<Direction>();
                foreach (var element in directionElements)
                {
                    directions.Add(new Direction { Text = CleanText(element.GetAttribute("innerHTML")) });
                }
                directionGroups.Add(new DirectionGroup { Label = label, Directions = directions });
            }

            return directionGroups;
        }

        List<IngredientGroup> IPeruser.GetIngredientGroups(IWebElement container)
        {
            var ingredientGroups = new List<IngredientGroup>();
            var ingredientGroupElements = container.FindElements(By.ClassName("tasty-recipe-ingredients"));
            foreach (var groupElement in ingredientGroupElements)
            {
                var label = ""; // TODO figure how to grab h4s

                var ingredientElements = groupElement.FindElements(By.TagName("li"));
                var ingredients = new List<Ingredient>();
                foreach (var element in ingredientElements)
                {
                    var span = element.FindElements(By.TagName("span")).FirstOrDefault();
                    var name = element.GetAttribute("innerHTML");
                    name = CleanText(Regex.Replace(name, "<span .*data.*>.*</span>", ""));
                    ingredients.Add(new Ingredient
                    {
                        Amount = span?.GetAttribute("data-amount") ?? "",
                        Unit = span?.GetAttribute("data-unit") ?? "",
                        Name = name,
                        Note = "",
                    });
                }
                ingredientGroups.Add(new IngredientGroup { Label = label, Ingredients = ingredients });
            }

            return ingredientGroups;
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

            return CleanText(text);
        }

        private string CleanText(string text)
        {
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
            text = Regex.Replace(text, "<span .*>", "");

            return text;
        }
    }
}
