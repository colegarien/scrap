using OpenQA.Selenium;
using Scrap.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Scrap.Peruser
{
    // https://demos.boxystudio.com/cooked/recipe/peanut-butter-sandwich-cookies/
    // https://demos.boxystudio.com/cooked/recipe/brisket-root-vegetables/
    // https://demos.boxystudio.com/cooked/recipe/sausage-hash-brown-casserole/
    class CookedRecipe : IPeruser
    {
        IWebElement IPeruser.FindContainerElement(IWebDriver driver)
        {
            return driver.FindElement(By.ClassName("cp_recipe"));
        }

        string IPeruser.GetName(IWebElement container)
        {
            return GetGuts(container, "entry-title");
        }

        string IPeruser.GetNotes(IWebElement container)
        {
            return "";
        }

        string IPeruser.GetServingSize(IWebElement container)
        {
            return container.FindElement(By.ClassName("cooked-servings")).FindElement(By.TagName("a")).Text;
        }

        string IPeruser.GetSummary(IWebElement container)
        {
            return GetGuts(container, "cooked-recipe-excerpt");
        }

        List<Tag> IPeruser.GetTags(IWebElement container)
        {
            var tags = new List<Tag>();

            var categoryElement = container.FindElements(By.ClassName("cooked-category")).FirstOrDefault();
            if (categoryElement != null)
            {
                tags.Add(new Tag
                {
                    Label = categoryElement.FindElement(By.ClassName("cooked-meta-title")).Text,
                    Value = categoryElement.FindElement(By.TagName("a")).Text,
                });
            }

            var difficultyElement = container.FindElements(By.ClassName("cooked-difficulty-level")).FirstOrDefault();
            if (difficultyElement != null)
            {
                tags.Add(new Tag
                {
                    Label = difficultyElement.FindElement(By.ClassName("cooked-meta-title")).Text,
                    Value = difficultyElement.FindElement(By.TagName("span")).Text,
                });
            }

            return tags;
        }

        TimeGroup IPeruser.GetTimeGroup(IWebElement container)
        {
            var times = new List<Time>();

            var timeElements = container.FindElements(By.ClassName("cooked-time"));
            foreach(var element in timeElements)
            {
                var label = CleanText(element.FindElement(By.ClassName("cooked-meta-title")).Text);
                var text = CleanText(element.Text.Replace(label, ""));
                times.Add(new Time
                {
                    Label = label,
                    Amount = text,
                    Unit = ""
                });
            }

            return new TimeGroup
            {
                Times = times
            };
        }

        List<DirectionGroup> IPeruser.GetDirectionGroups(IWebElement container)
        {
            var directionGroupContainer = container.FindElement(By.ClassName("cooked-recipe-directions"));
            var allElements = directionGroupContainer.FindElements(By.XPath("*"));

            var groups = new List<DirectionGroup>();

            DirectionGroup currentGroup = null;
            foreach (var element in allElements)
            {
                if (element.GetAttribute("class").Contains("cooked-heading"))
                {
                    if (currentGroup != null)
                    {
                        // We've already setup a group, add it to the list
                        groups.Add(currentGroup);
                    }

                    // Start Next Ingredient Group
                    currentGroup = new DirectionGroup
                    {
                        Label = CleanText(element.Text),
                        Directions = new List<Direction>()
                    };
                }
                else
                {
                    if (currentGroup == null)
                    {
                        // First group didn't have a heading, so initialize it with blank label
                        currentGroup = new DirectionGroup
                        {
                            Label = "",
                            Directions = new List<Direction>()
                        };
                    }

                    currentGroup.Directions.Add(new Direction
                    {
                        Text = GetGuts(element, "cooked-dir-content")
                    });
                }
            }
            if (currentGroup != null)
            {
                // Add last group
                groups.Add(currentGroup);
            }

            return groups;
        }

        List<IngredientGroup> IPeruser.GetIngredientGroups(IWebElement container)
        {
            var ingredientGroupContainer = container.FindElement(By.ClassName("cooked-recipe-ingredients"));
            var allElements = ingredientGroupContainer.FindElements(By.XPath("*"));

            var groups = new List<IngredientGroup>();

            IngredientGroup currentGroup = null;
            foreach (var element in allElements)
            {
                if (element.GetAttribute("class").Contains("cooked-heading"))
                {
                    if (currentGroup != null)
                    {
                        // We've already setup a group, add it to the list
                        groups.Add(currentGroup);
                    }

                    // Start Next Ingredient Group
                    currentGroup = new IngredientGroup
                    {
                        Label = CleanText(element.Text),
                        Ingredients = new List<Ingredient>()
                    };
                }
                else
                {
                    if(currentGroup == null)
                    {
                        // First group didn't have a heading, so initialize it with blank label
                        currentGroup = new IngredientGroup
                        {
                            Label = "",
                            Ingredients = new List<Ingredient>()
                        };
                    }

                    currentGroup.Ingredients.Add(new Ingredient
                    {
                        Amount = CleanText(element.FindElements(By.ClassName("cooked-ing-amount")).FirstOrDefault()?.GetAttribute("data-decimal") ?? ""),
                        Unit = CleanText(element.FindElements(By.ClassName("cooked-ing-measurement")).FirstOrDefault()?.Text ?? ""),
                        Name = CleanText(element.FindElements(By.ClassName("cooked-ing-name")).FirstOrDefault()?.Text ?? ""),
                        Note = ""
                    });
                }
            }
            if (currentGroup != null)
            {
                // Add last group
                groups.Add(currentGroup);
            }

            return groups;
        }





        private string GetGuts(IWebElement element, string className)
        {
            var firstElement = element.FindElements(By.ClassName(className))
                .FirstOrDefault();
            var text = "";
            if (firstElement != null)
            {
                text = firstElement.FindElements(By.ClassName(className))
                    .FirstOrDefault()?.Text
                    ?? firstElement.Text;
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

            return text.Trim();
        }
    }
}
