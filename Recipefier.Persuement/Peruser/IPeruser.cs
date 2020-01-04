using OpenQA.Selenium;
using Recipefier.Domain.Model;
using System.Collections.Generic;
using System.Linq;

namespace Recipefier.Persuement.Peruser
{
    interface IPeruser
    {
        public bool CanPeruse(IWebDriver driver)
        {
            try
            {
                return FindContainerElement(driver) != null;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        protected IWebElement FindContainerElement(IWebDriver driver);

        public Recipe Peruse(IWebDriver driver)
        {
            var container = FindContainerElement(driver);

            return new Recipe
            {
                Source = GetType().Name + " | " + driver.Url,
                Name = GetName(container),
                Summary = GetSummary(container),
                Tags = GetTags(container),
                Yield = GetYield(container),
                TimeGroup = GetTimeGroup(container),
                IngredientGroups = GetIngredientGroups(container),
                DirectionGroups = GetDirectionGroups(container),
                Notes = GetNotes(container)
            };
        }

        protected string GetName(IWebElement container);
        protected string GetSummary(IWebElement container);
        protected List<Tag> GetTags(IWebElement container);
        protected string GetYield(IWebElement container);
        protected TimeGroup GetTimeGroup(IWebElement container);
        protected List<IngredientGroup> GetIngredientGroups(IWebElement container);
        protected List<DirectionGroup> GetDirectionGroups(IWebElement container);
        protected string GetNotes(IWebElement container);

    }
}
