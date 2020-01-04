using OpenQA.Selenium;
using Recipefier.Domain.Model;
using Recipefier.Persuement.Peruser.Utilities;
using System.Collections.Generic;

namespace Recipefier.Persuement.Peruser
{
    // https://demo.wpzoom.com/recipe-card-blocks/2019/03/26/new-style-design/
    // https://demo.wpzoom.com/recipe-card-blocks/2019/03/26/kid-friendly-oil-free-vegan-pancakes/
    // https://demo.wpzoom.com/recipe-card-blocks/2019/02/06/recipe-card-classic-style/
    class RecipeCardBlocks : IPeruser
    {
        protected Puller puller;
        public RecipeCardBlocks()
        {
            puller = new Puller();
        }

        IWebElement IPeruser.FindContainerElement(IWebDriver driver)
        {
            return puller.GetOne(driver, "wp-block-wpzoom-recipe-card-block-recipe-card");
        }

        string IPeruser.GetName(IWebElement container)
        {
            return puller.GetText(container, "recipe-card-title");
        }

        string IPeruser.GetNotes(IWebElement container)
        {
            return puller.GetText(container, "recipe-card-notes-list");
        }

        string IPeruser.GetYield(IWebElement container)
        {
            var detailsItemElements = puller.GetMany(puller.GetOne(container, "details-items"), "detail-item");
            foreach (var detailElement in detailsItemElements)
            {
                if (puller.GetText(detailElement, "detail-item-label").Contains("Serving"))
                {
                    var number = puller.GetAttribute(detailElement, "detail-item-adjustable-servings", "value");
                    return puller.CleanText(number + " " + puller.GetText(detailElement, "detail-item-unit"));
                }
            }

            return "";
        }

        string IPeruser.GetSummary(IWebElement container)
        {
            return puller.GetText(container, "recipe-card-summary");
        }

        List<Tag> IPeruser.GetTags(IWebElement container)
        {
            var courseTag = new Tag
            {
                Label = "Course",
                Value = puller.GetText(puller.GetOne(container, "recipe-card-course"), "mark", PullType.BY_TAG)
            };
            var cuisineTag = new Tag
            {
                Label = "Cuisine",
                Value = puller.GetText(puller.GetOne(container, "recipe-card-cuisine"), "mark", PullType.BY_TAG)
            };
            var difficulyTag = new Tag
            {
                Label = "Difficulty",
                Value = puller.GetText(puller.GetOne(container, "recipe-card-difficulty"), "mark", PullType.BY_TAG)
            };

            var tags = new List<Tag>();
            if (courseTag.Value != "")
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

            var detailsItemElements = puller.GetMany(puller.GetOne(container, "details-items"), "detail-item");
            foreach (var detailElement in detailsItemElements)
            {
                var labelElement = puller.GetOne(detailElement, "detail-item-label");
                var labelText = puller.CleanText(labelElement?.Text ?? "");
                if (labelText.Contains("time") || labelText.Contains("Time"))
                {
                    timeGroup.Times.Add(new Time
                    {
                        Label = labelText,
                        Amount = puller.GetText(detailElement, "detail-item-value"),
                        Unit = puller.GetText(detailElement, "detail-item-unit")
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

            var directionElements = puller.GetMany(container, "direction-step");
            foreach (var element in directionElements)
            {
                directionGroup.Directions.Add(new Direction
                {
                    Text = puller.CleanText(element.Text)
                });
            }

            return new List<DirectionGroup>
            {
                directionGroup
            };
        }

        List<IngredientGroup> IPeruser.GetIngredientGroups(IWebElement container)
        {
            var ingredientGroupContainer = puller.GetOne(container, "ingredients-list");
            var allElements = puller.GetMany(ingredientGroupContainer, "*", PullType.BY_XPATH);

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
                        Label = puller.CleanText(element.Text),
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
                        Amount = puller.GetText(element, "wpzoom-rcb-ingredient-amount"),
                        Unit = puller.GetText(element, "wpzoom-rcb-ingredient-unit"),
                        Name = puller.GetText(element, "wpzoom-rcb-ingredient-name"),
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
