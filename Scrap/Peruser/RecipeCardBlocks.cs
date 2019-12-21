using OpenQA.Selenium;
using Scrap.Model;
using Scrap.Peruser.Utilities;
using System.Collections.Generic;

namespace Scrap.Peruser
{
    // https://demo.wpzoom.com/recipe-card-blocks/2019/03/26/new-style-design/
    // https://demo.wpzoom.com/recipe-card-blocks/2019/03/26/kid-friendly-oil-free-vegan-pancakes/
    // https://demo.wpzoom.com/recipe-card-blocks/2019/02/06/recipe-card-classic-style/
    class RecipeCardBlocks : IPeruser
    {
        protected Puller puller;
        public RecipeCardBlocks()
        {
            this.puller = new Puller();
        }

        IWebElement IPeruser.FindContainerElement(IWebDriver driver)
        {
            return this.puller.GetOne(driver, "wp-block-wpzoom-recipe-card-block-recipe-card");
        }

        string IPeruser.GetName(IWebElement container)
        {
            return this.puller.GetText(container, "recipe-card-title");
        }

        string IPeruser.GetNotes(IWebElement container)
        {
            return this.puller.GetText(container, "recipe-card-notes-list");
        }

        string IPeruser.GetServingSize(IWebElement container)
        {
            var detailsItemElements = this.puller.GetMany(this.puller.GetOne(container, "details-items"), "detail-item");
            foreach(var detailElement in detailsItemElements)
            {
                if (this.puller.GetText(detailElement, "detail-item-label").Contains("Serving"))
                {
                    var number = this.puller.GetAttribute(detailElement, "detail-item-adjustable-servings", "value");
                    return this.puller.CleanText(number + " " + this.puller.GetText(detailElement, "detail-item-unit"));
                }
            }

            return "";
        }

        string IPeruser.GetSummary(IWebElement container)
        {
            return this.puller.GetText(container, "recipe-card-summary");
        }

        List<Tag> IPeruser.GetTags(IWebElement container)
        {
            var courseTag = new Tag
            {
                Label = "Course",
                Value = this.puller.GetText(this.puller.GetOne(container, "recipe-card-course"), "mark", PullType.BY_TAG)
            };
            var cuisineTag = new Tag
            {
                Label = "Cuisine",
                Value = this.puller.GetText(this.puller.GetOne(container, "recipe-card-cuisine"), "mark", PullType.BY_TAG)
            };
            var difficulyTag = new Tag
            {
                Label = "Difficulty",
                Value = this.puller.GetText(this.puller.GetOne(container, "recipe-card-difficulty"), "mark", PullType.BY_TAG)
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

            var detailsItemElements = this.puller.GetMany(this.puller.GetOne(container, "details-items"), "detail-item");
            foreach (var detailElement in detailsItemElements)
            {
                var labelElement = this.puller.GetOne(detailElement, "detail-item-label");
                var labelText = this.puller.CleanText(labelElement?.Text ?? "");
                if (labelText.Contains("time") || labelText.Contains("Time"))
                {
                    timeGroup.Times.Add(new Time
                    {
                        Label = labelText,
                        Amount = this.puller.GetText(detailElement, "detail-item-value"),
                        Unit = this.puller.GetText(detailElement, "detail-item-unit")
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

            var directionElements = this.puller.GetMany(container, "direction-step");
            foreach(var element in directionElements)
            {
                directionGroup.Directions.Add(new Direction
                {
                    Text = this.puller.CleanText(element.Text)
                });
            }

            return new List<DirectionGroup>
            {
                directionGroup
            };
        }

        List<IngredientGroup> IPeruser.GetIngredientGroups(IWebElement container)
        {
            var ingredientGroupContainer = this.puller.GetOne(container, "ingredients-list");
            var allElements = this.puller.GetMany(ingredientGroupContainer, "*", PullType.BY_XPATH);

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
                        Label = this.puller.CleanText(element.Text),
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
                        Amount = this.puller.GetText(element, "wpzoom-rcb-ingredient-amount"),
                        Unit = this.puller.GetText(element, "wpzoom-rcb-ingredient-unit"),
                        Name = this.puller.GetText(element, "wpzoom-rcb-ingredient-name"),
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
    }
}
