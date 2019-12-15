using OpenQA.Selenium;
using Scrap.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Scrap.Peruser
{
    // https://demo.wpzoom.com/recipe-card-blocks/2019/03/26/new-style-design/
    // https://demo.wpzoom.com/recipe-card-blocks/2019/03/26/kid-friendly-oil-free-vegan-pancakes/
    // https://demo.wpzoom.com/recipe-card-blocks/2019/02/06/recipe-card-classic-style/
    class RecipeCardBlocks : IPeruser
    {
        IWebElement IPeruser.FindContainerElement(IWebDriver driver)
        {
            return driver.FindElement(By.ClassName("wp-block-wpzoom-recipe-card-block-recipe-card"));
        }

        string IPeruser.GetName(IWebElement container)
        {
            return GetGuts(container, "recipe-card-title");
        }

        string IPeruser.GetNotes(IWebElement container)
        {
            return GetGuts(container, "recipe-card-notes-list");
        }

        string IPeruser.GetServingSize(IWebElement container)
        {
            var detailsItemElements = container.FindElement(By.ClassName("details-items")).FindElements(By.ClassName("detail-item"));
            foreach(var detailElement in detailsItemElements)
            {
                if (detailElement.FindElement(By.ClassName("detail-item-label")).Text.Contains("Serving"))
                {
                    var number = detailElement.FindElement(By.ClassName("detail-item-adjustable-servings")).GetAttribute("value");
                    return CleanText(number + " " + GetGuts(detailElement, "detail-item-unit"));
                }
            }

            return "";
        }

        string IPeruser.GetSummary(IWebElement container)
        {
            return GetGuts(container, "recipe-card-summary");
        }

        List<Tag> IPeruser.GetTags(IWebElement container)
        {
            var courseTag = new Tag
            {
                Label = "Course",
                Value = CleanText(container.FindElements(By.ClassName("recipe-card-course")).FirstOrDefault()?.FindElement(By.TagName("mark"))?.Text ?? "")
            };
            var cuisineTag = new Tag
            {
                Label = "Cuisine",
                Value = CleanText(container.FindElements(By.ClassName("recipe-card-cuisine")).FirstOrDefault()?.FindElement(By.TagName("mark"))?.Text ?? "")
            };
            var difficulyTag = new Tag
            {
                Label = "Difficulty",
                Value = CleanText(container.FindElements(By.ClassName("recipe-card-difficulty")).FirstOrDefault()?.FindElement(By.TagName("mark"))?.Text ?? "")
            };

            var tags = new List<Tag>();
            if(courseTag.Value != "")
            {
                tags.Add(courseTag);
            }
            if (cuisineTag.Value != "")
            {
                tags.Add(cuisineTag);
            }
            if (difficulyTag.Value != "")
            {
                tags.Add(difficulyTag);
            }

            return tags;
        }

        TimeGroup IPeruser.GetTimeGroup(IWebElement container)
        {
            var timeGroup = new TimeGroup
            {
                Times = new List<Time>()
            };

            var detailsItemElements = container.FindElement(By.ClassName("details-items")).FindElements(By.ClassName("detail-item"));
            foreach (var detailElement in detailsItemElements)
            {
                var labelElement = detailElement.FindElement(By.ClassName("detail-item-label"));
                if (labelElement.Text.Contains("time") || labelElement.Text.Contains("Time"))
                {
                    timeGroup.Times.Add(new Time
                    {
                        Label = CleanText(labelElement.Text),
                        Amount = GetGuts(detailElement, "detail-item-value"),
                        Unit = GetGuts(detailElement, "detail-item-unit")
                    });
                }
            }

            return timeGroup;
        }

        List<DirectionGroup> IPeruser.GetDirectionGroups(IWebElement container)
        {
            var directionGroup = new DirectionGroup
            {
                Label = "",
                Directions = new List<Direction>()
            };

            var directionElements = container.FindElements(By.ClassName("direction-step"));
            foreach(var element in directionElements)
            {
                directionGroup.Directions.Add(new Direction
                {
                    Text = CleanText(element.Text)
                });
            }

            return new List<DirectionGroup>
            {
                directionGroup
            };
        }

        List<IngredientGroup> IPeruser.GetIngredientGroups(IWebElement container)
        {
            var ingredientGroupContainer = container.FindElement(By.ClassName("ingredients-list"));
            var allElements = ingredientGroupContainer.FindElements(By.XPath("*"));

            var groups = new List<IngredientGroup>();

            IngredientGroup currentGroup = null;
            foreach (var element in allElements)
            {
                if (element.GetAttribute("class").Contains("ingredient-item-group"))
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
                    if (currentGroup == null)
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
                        Amount = CleanText(element.FindElements(By.ClassName("wpzoom-rcb-ingredient-amount")).FirstOrDefault()?.Text ?? ""),
                        Unit = CleanText(element.FindElements(By.ClassName("wpzoom-rcb-ingredient-unit")).FirstOrDefault()?.Text ?? ""),
                        Name = CleanText(element.FindElements(By.ClassName("wpzoom-rcb-ingredient-name")).FirstOrDefault()?.Text ?? ""),
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
