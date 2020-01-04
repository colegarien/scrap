using OpenQA.Selenium;
using Recipefier.Domain.Model;
using Recipefier.Persuement.Peruser.Utilities;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Recipefier.Persuement.Peruser.WordpressPlugin
{
    // BASED on https://cookieandkate.com/best-carrot-cake-recipe/#tasty-recipes-33706
    // AND https://cookiesandcups.com/perfect-snickerdoodles/
    // ALSO https://pinchofyum.com/sweet-potato-peanut-soup
    class TastyRecipes : IPeruser
    {
        protected readonly Puller puller;
        public TastyRecipes()
        {
            puller = new Puller();
        }

        IWebElement IPeruser.FindContainerElement(IWebDriver driver)
        {
            return puller.GetOne(driver, "tasty-recipes");
        }

        string IPeruser.GetName(IWebElement container)
        {
            return puller.GetAttribute(container, "h2", "innerHTML", PullType.BY_TAG);
        }

        string IPeruser.GetNotes(IWebElement container)
        {
            return puller.GetText(container, "tasty-recipes-notes");
        }

        string IPeruser.GetSummary(IWebElement container)
        {
            return puller.GetText(container, "tasty-recipes-description");
        }

        List<Tag> IPeruser.GetTags(IWebElement container)
        {
            var tags = new List<Tag>();
            var categoryTag = new Tag
            {
                Label = "Category",
                Value = puller.GetText(container, "tasty-recipes-category")
            };
            var methodTag = new Tag
            {
                Label = "Method",
                Value = puller.GetText(container, "tasty-recipes-method")
            };
            var cuisineTag = new Tag
            {
                Label = "Cuisine",
                Value = puller.GetText(container, "tasty-recipes-cuisine")
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

        string IPeruser.GetYield(IWebElement container)
        {
            return puller.CleanText(puller.GetText(container, "tasty-recipes-yield").Replace("1x", ""));
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
                Amount = puller.GetText(container, "tasty-recipes-prep-time"),
                Unit = ""
            };
            var cookTime = new Time
            {
                Label = "Cook Time",
                Amount = puller.GetText(container, "tasty-recipes-cook-time"),
                Unit = ""
            };
            var totalTime = new Time
            {
                Label = "Total Time",
                Amount = puller.GetText(container, "tasty-recipes-total-time"),
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
            var directionGroupElements = puller.GetMany(container, "tasty-recipe-instructions");
            if (directionGroupElements.Count == 0)
            {
                directionGroupElements = puller.GetMany(container, "tasty-recipes-instructions");
            }
            foreach (var groupElement in directionGroupElements)
            {
                var label = "";

                var directionElements = puller.GetMany(groupElement, "li", PullType.BY_TAG);
                var directions = new List<Direction>();
                foreach (var element in directionElements)
                {
                    directions.Add(new Direction { Text = puller.CleanText(element.GetAttribute("innerHTML")) });
                }
                directionGroups.Add(new DirectionGroup { Label = label, Directions = directions });
            }

            return directionGroups;
        }

        List<IngredientGroup> IPeruser.GetIngredientGroups(IWebElement container)
        {
            var ingredientContainer = puller.GetOne(container, "tasty-recipe-ingredients");
            if (ingredientContainer == null)
            {
                ingredientContainer = puller.GetOne(container, "tasty-recipes-ingredients");
            }

            var ingredientGroups = new List<IngredientGroup>();
            var ingredientGroupElements = puller.GetMany(ingredientContainer, "ul", PullType.BY_TAG);
            if (ingredientGroupElements.Count == 0)
            {
                ingredientGroupElements = puller.GetMany(ingredientContainer, "ol", PullType.BY_TAG);
            }
            foreach (var groupElement in ingredientGroupElements)
            {
                var label = GuessIngredientGroupLabel(ingredientContainer, groupElement);

                var ingredientElements = puller.GetMany(groupElement, "li", PullType.BY_TAG);
                var ingredients = new List<Ingredient>();
                foreach (var element in ingredientElements)
                {
                    var span = puller.GetOne(element, "span", PullType.BY_TAG);
                    var name = element.Text;
                    if (span != null)
                    {
                        var regex = new Regex(Regex.Escape(span.Text));
                        name = regex.Replace(name, "", 1);
                    }
                    ingredients.Add(new Ingredient
                    {
                        Amount = span?.GetAttribute("data-amount") ?? "",
                        Unit = span?.GetAttribute("data-unit") ?? "",
                        Name = puller.CleanText(name),
                        Note = "",
                    });
                }
                ingredientGroups.Add(new IngredientGroup { Label = label, Ingredients = ingredients });
            }

            return ingredientGroups;
        }

        private string GuessIngredientGroupLabel(IWebElement ingredientContainer, IWebElement groupElement)
        {
            var label = "";
            // Get all high-level elements in the big ingredient container
            var children = puller.GetMany(ingredientContainer, "*", PullType.BY_XPATH);
            foreach (var child in children)
            {
                if (child.TagName == "h4")
                {
                    // Found a header!
                    label = puller.CleanText(child.Text);
                }
                if (child.Location == groupElement.Location)
                {
                    // Ran into the group in question, we are done looking
                    break;
                }
                else if (child.TagName == "ul" || child.TagName == "ol")
                {
                    // Ran into a ingredient group that isn't the 'groupElement', clear any previusly found header
                    label = "";
                }
            }

            return label;
        }
    }
}
