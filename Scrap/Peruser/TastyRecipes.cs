using OpenQA.Selenium;
using Scrap.Model;
using Scrap.Peruser.Utilities;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Scrap.Peruser
{
    // BASED on https://cookieandkate.com/best-carrot-cake-recipe/#tasty-recipes-33706
    // AND https://cookiesandcups.com/perfect-snickerdoodles/
    // ALSO https://pinchofyum.com/sweet-potato-peanut-soup
    class TastyRecipes : IPeruser
    {
        protected Puller puller;
        public TastyRecipes()
        {
            this.puller = new Puller();
        }

        IWebElement IPeruser.FindContainerElement(IWebDriver driver)
        {
            return this.puller.GetOne(driver, "tasty-recipes");
        }

        string IPeruser.GetName(IWebElement container)
        {
            return this.puller.GetAttribute(container, "h2", "innerHTML", PullType.BY_TAG);
        }

        string IPeruser.GetNotes(IWebElement container)
        {
            return this.puller.GetText(container, "tasty-recipes-notes");
        }

        string IPeruser.GetSummary(IWebElement container)
        {
            return this.puller.GetText(container, "tasty-recipes-description");
        }

        List<Tag> IPeruser.GetTags(IWebElement container)
        {
            var tags = new List<Tag>();
            var categoryTag = new Tag
            {
                Label = "Category",
                Value = this.puller.GetText(container, "tasty-recipes-category")
            };
            var methodTag = new Tag
            {
                Label = "Method",
                Value = this.puller.GetText(container, "tasty-recipes-method")
            };
            var cuisineTag = new Tag
            {
                Label = "Cuisine",
                Value = this.puller.GetText(container, "tasty-recipes-cuisine")
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
            return this.puller.CleanText(this.puller.GetText(container, "tasty-recipes-yield").Replace("1x",""));
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
                Amount = this.puller.GetText(container, "tasty-recipes-prep-time"),
                Unit = ""
            };
            var cookTime = new Time
            {
                Label = "Cook Time",
                Amount = this.puller.GetText(container, "tasty-recipes-cook-time"),
                Unit = ""
            };
            var totalTime = new Time
            {
                Label = "Total Time",
                Amount = this.puller.GetText(container, "tasty-recipes-total-time"),
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
            var directionGroupElements = this.puller.GetMany(container, "tasty-recipe-instructions");
            if(directionGroupElements.Count == 0)
            {
                directionGroupElements = this.puller.GetMany(container, "tasty-recipes-instructions");
            }
            foreach (var groupElement in directionGroupElements)
            {
                var label = "";

                var directionElements = this.puller.GetMany(groupElement, "li", PullType.BY_TAG);
                var directions = new List<Direction>();
                foreach (var element in directionElements)
                {
                    directions.Add(new Direction { Text = this.puller.CleanText(element.GetAttribute("innerHTML")) });
                }
                directionGroups.Add(new DirectionGroup { Label = label, Directions = directions });
            }

            return directionGroups;
        }

        List<IngredientGroup> IPeruser.GetIngredientGroups(IWebElement container)
        {
            var ingredientContainer = this.puller.GetOne(container, "tasty-recipe-ingredients");
            if (ingredientContainer == null)
            {
                ingredientContainer = this.puller.GetOne(container, "tasty-recipes-ingredients");
            }

            var ingredientGroups = new List<IngredientGroup>();
            var ingredientGroupElements = this.puller.GetMany(ingredientContainer, "ul", PullType.BY_TAG);
            if (ingredientGroupElements.Count == 0)
            {
                ingredientGroupElements = this.puller.GetMany(ingredientContainer, "ol", PullType.BY_TAG);
            }
            foreach (var groupElement in ingredientGroupElements)
            {
                var label = GuessIngredientGroupLabel(ingredientContainer, groupElement);

                var ingredientElements = this.puller.GetMany(groupElement, "li", PullType.BY_TAG);
                var ingredients = new List<Ingredient>();
                foreach (var element in ingredientElements)
                {
                    var span = this.puller.GetOne(element, "span", PullType.BY_TAG);
                    var name = element.Text;
                    if(span != null)
                    {
                        var regex = new Regex(Regex.Escape(span.Text));
                        name = regex.Replace(name, "", 1);
                    }
                    ingredients.Add(new Ingredient
                    {
                        Amount = span?.GetAttribute("data-amount") ?? "",
                        Unit = span?.GetAttribute("data-unit") ?? "",
                        Name = this.puller.CleanText(name),
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
            var children = this.puller.GetMany(ingredientContainer, "*", PullType.BY_XPATH);
            foreach (var child in children)
            {
                if (child.TagName == "h4")
                {
                    // Found a header!
                    label = this.puller.CleanText(child.Text);
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
