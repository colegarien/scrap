using OpenQA.Selenium;
using Recipefier.Persuement.Model;
using Recipefier.Persuement.Peruser.Utilities;
using System.Collections.Generic;

namespace Recipefier.Persuement.Peruser
{
    // https://demos.boxystudio.com/cooked/recipe/peanut-butter-sandwich-cookies/
    // https://demos.boxystudio.com/cooked/recipe/brisket-root-vegetables/
    // https://demos.boxystudio.com/cooked/recipe/sausage-hash-brown-casserole/
    class CookedRecipe : IPeruser
    {
        protected Puller puller;
        public CookedRecipe()
        {
            puller = new Puller();
        }

        IWebElement IPeruser.FindContainerElement(IWebDriver driver)
        {
            return puller.GetOne(driver, "cp_recipe");
        }

        string IPeruser.GetName(IWebElement container)
        {
            return puller.GetText(container, "entry-title");
        }

        string IPeruser.GetNotes(IWebElement container)
        {
            return "";
        }

        string IPeruser.GetYield(IWebElement container)
        {
            return puller.GetText(puller.GetOne(container, "cooked-servings"), "a", PullType.BY_TAG);
        }

        string IPeruser.GetSummary(IWebElement container)
        {
            return puller.GetText(container, "cooked-recipe-excerpt");
        }

        List<Tag> IPeruser.GetTags(IWebElement container)
        {
            var tags = new List<Tag>();

            var categoryElement = puller.GetOne(container, "cooked-category");
            if (categoryElement != null)
            {
                tags.Add(new Tag
                {
                    Label = puller.GetText(categoryElement, "cooked-meta-title"),
                    Value = puller.GetText(categoryElement, "a", PullType.BY_TAG),
                });
            }

            var difficultyElement = puller.GetOne(container, "cooked-difficulty-level");
            if (difficultyElement != null)
            {
                tags.Add(new Tag
                {
                    Label = puller.GetText(difficultyElement, "cooked-meta-title"),
                    Value = puller.GetText(difficultyElement, "span", PullType.BY_TAG),
                });
            }

            return tags;
        }

        TimeGroup IPeruser.GetTimeGroup(IWebElement container)
        {
            var times = new List<Time>();

            var timeElements = puller.GetMany(container, "cooked-time");
            foreach (var element in timeElements)
            {
                var label = puller.GetText(element, "cooked-meta-title");
                var text = puller.CleanText(element.Text.Replace(label, ""));
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
            var directionGroupContainer = puller.GetOne(container, "cooked-recipe-directions");
            var allElements = puller.GetMany(directionGroupContainer, "*", PullType.BY_XPATH);

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
                        Label = puller.CleanText(element.Text),
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
                        Text = puller.GetText(element, "cooked-dir-content")
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
            var ingredientGroupContainer = puller.GetOne(container, "cooked-recipe-ingredients");
            var allElements = puller.GetMany(ingredientGroupContainer, "*", PullType.BY_XPATH);

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
                        Amount = puller.GetAttribute(element, "cooked-ing-amount", "data-decimal"),
                        Unit = puller.GetText(element, "cooked-ing-measurement"),
                        Name = puller.GetText(element, "cooked-ing-name"),
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
