using OpenQA.Selenium;
using Scrap.Model;
using Scrap.Peruser.Utilities;
using System.Collections.Generic;

namespace Scrap.Peruser
{
    // https://demo.ziprecipes.net/tres-leches/
    class ZipRecipesZL : IPeruser
    {
        protected Puller puller;
        public ZipRecipesZL()
        {
            this.puller = new Puller();
        }

        IWebElement IPeruser.FindContainerElement(IWebDriver driver)
        {
            return this.puller.GetOne(driver, "zlrecipe");
        }

        List<DirectionGroup> IPeruser.GetDirectionGroups(IWebElement container)
        {
            var directionElements = this.puller.GetMany(container, "instruction");
            var directions = new List<Direction>();
            foreach (var element in directionElements)
            {
                directions.Add(new Direction { Text = this.puller.CleanText(element.Text) });
            }
            return new List<DirectionGroup>() { new DirectionGroup { Label = "", Directions = directions } };
        }

        List<IngredientGroup> IPeruser.GetIngredientGroups(IWebElement container)
        {
            var ingredientElements = this.puller.GetMany(container, "ingredient");
            var ingredients = new List<Ingredient>();
            foreach (var element in ingredientElements)
            {
                ingredients.Add(new Ingredient { Name = this.puller.CleanText(element.Text) });
            }
            return new List<IngredientGroup>() { new IngredientGroup { Label = "", Ingredients = ingredients } };
        }

        string IPeruser.GetName(IWebElement container)
        {
            return this.puller.GetText(container, "zlrecipe-title", PullType.BY_ID);
        }

        string IPeruser.GetNotes(IWebElement container)
        {
            return this.puller.GetText(container, "zlrecipe-notes-list", PullType.BY_ID);
        }

        string IPeruser.GetServingSize(IWebElement container)
        {
            var yieldContainer = this.puller.GetOne(container, "yield");
            if(yieldContainer == null)
            {
                return "";
            }

            var amount = this.puller.GetAttribute(yieldContainer, "zrdn-serving-adjustment-input", "value");
            return this.puller.CleanText(amount + " " + this.puller.CleanText(yieldContainer.Text.Replace("Imperial","").Replace("Metric","")));
        }

        string IPeruser.GetSummary(IWebElement container)
        {
            return this.puller.GetText(container, "zlrecipe-summary", PullType.BY_ID);
        }

        List<Tag> IPeruser.GetTags(IWebElement container)
        {
            var tags = new List<Tag>();
            var categoryTag = new Tag
            {
                Label = "Category",
                Value = this.puller.CleanText(this.puller.GetText(container, "zlrecipe-category",PullType.BY_ID).Replace("Category:","").Replace("Category", ""))
            };
            var cuisineTag = new Tag
            {
                Label = "Cuisine",
                Value = this.puller.CleanText(this.puller.GetText(container, "zlrecipe-cuisine", PullType.BY_ID).Replace("Cuisine:", "").Replace("Cuisine", ""))
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
                Amount = this.puller.GetText(container, "prep_time"),
                Unit = ""
            };
            var cookTime = new Time
            {
                Label = "Cook Time",
                Amount = this.puller.GetText(container, "cook_time"),
                Unit = ""
            };
            var totalTime = new Time
            {
                Label = "Total Time",
                Amount = this.puller.CleanText(this.puller.GetText(container, "zlrecipe-total-time", PullType.BY_ID).Replace("Total Time:", "").Replace("Total Time", "")),
                Unit = ""
            };
            var otherTotalTime = new Time
            {
                Label = "Total Time",
                Amount = this.puller.GetText(container, "total-time"),
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
    }
}
