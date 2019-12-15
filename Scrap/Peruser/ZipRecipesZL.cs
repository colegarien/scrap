using OpenQA.Selenium;
using Scrap.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Scrap.Peruser
{
    // https://demo.ziprecipes.net/tres-leches/
    class ZipRecipesZL : IPeruser
    {
        IWebElement IPeruser.FindContainerElement(IWebDriver driver)
        {
            return driver.FindElement(By.ClassName("zlrecipe"));
        }

        List<DirectionGroup> IPeruser.GetDirectionGroups(IWebElement container)
        {
            var directionElements = container.FindElements(By.ClassName("instruction"));
            var directions = new List<Direction>();
            foreach (var element in directionElements)
            {
                directions.Add(new Direction { Text = CleanText(element.Text) });
            }
            return new List<DirectionGroup>() { new DirectionGroup { Label = "", Directions = directions } };
        }

        List<IngredientGroup> IPeruser.GetIngredientGroups(IWebElement container)
        {
            var ingredientElements = container.FindElements(By.ClassName("ingredient"));
            var ingredients = new List<Ingredient>();
            foreach (var element in ingredientElements)
            {
                ingredients.Add(new Ingredient { Name = CleanText(element.Text) });
            }
            return new List<IngredientGroup>() { new IngredientGroup { Label = "", Ingredients = ingredients } };
        }

        string IPeruser.GetName(IWebElement container)
        {
            return CleanText(container.FindElement(By.Id("zlrecipe-title")).Text);
        }

        string IPeruser.GetNotes(IWebElement container)
        {
            return CleanText(container.FindElement(By.Id("zlrecipe-notes-list")).Text);
        }

        string IPeruser.GetServingSize(IWebElement container)
        {
            var yieldContainer = container.FindElements(By.ClassName("yield")).FirstOrDefault();
            if(yieldContainer == null)
            {
                return "";
            }

            var amount = yieldContainer.FindElement(By.ClassName("zrdn-serving-adjustment-input")).GetAttribute("value");
            return CleanText(amount + " " + yieldContainer.Text.Replace("Imperial","").Replace("Metric",""));
        }

        string IPeruser.GetSummary(IWebElement container)
        {
            return CleanText(container.FindElement(By.Id("zlrecipe-summary")).Text);
        }

        List<Tag> IPeruser.GetTags(IWebElement container)
        {
            var tags = new List<Tag>();
            var categoryTag = new Tag
            {
                Label = "Category",
                Value = CleanText(container.FindElements(By.Id("zlrecipe-category")).FirstOrDefault()?.Text?.Replace("Category:","")?.Replace("Category", "") ?? "")
            };
            var cuisineTag = new Tag
            {
                Label = "Cuisine",
                Value = CleanText(container.FindElements(By.Id("zlrecipe-cuisine")).FirstOrDefault()?.Text?.Replace("Cuisine:", "")?.Replace("Cuisine", "") ?? "")
            };

            if (categoryTag.Value != "")
            {
                tags.Add(categoryTag);
            }
            if (cuisineTag.Value != "")
            {
                tags.Add(cuisineTag);
            }

            return tags;
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
                Amount = GetGuts(container, "prep_time"),
                Unit = ""
            };
            var cookTime = new Time
            {
                Label = "Cook Time",
                Amount = GetGuts(container, "cook_time"),
                Unit = ""
            };
            var totalTime = new Time
            {
                Label = "Total Time",
                Amount = CleanText(container.FindElements(By.Id("zlrecipe-total-time")).FirstOrDefault()?.Text?.Replace("Total Time:", "") ?? ""),
                Unit = ""
            };
            var otherTotalTime = new Time
            {
                Label = "Total Time",
                Amount = GetGuts(container, "total-time"),
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
            if (otherTotalTime.Amount != "")
            {
                timeGroup.Times.Add(otherTotalTime);
            }

            return timeGroup;
        }



        protected string GetGuts(IWebElement element, string className)
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

        protected string CleanText(string text)
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
