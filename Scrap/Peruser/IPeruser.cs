﻿using OpenQA.Selenium;
using Scrap.Model;
using System.Collections.Generic;
using System.Linq;

namespace Scrap.Peruser
{
    interface IPeruser
    {
        public bool CanPeruse(IWebDriver driver)
        {
            try
            {
                FindContainerElement(driver);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            return true;
        }

        protected IWebElement FindContainerElement(IWebDriver driver);

        public Recipe Peruse(IWebDriver driver)
        {
            var container = FindContainerElement(driver);

            return new Recipe
            {
                Source = driver.Url,
                Name = GetName(container),
                Summary = GetSummary(container),
                Tags = GetTags(container),
                ServingSize = GetServingSize(container),
                TimeGroup = GetTimeGroup(container),
                IngredientGroups = GetIngredientGroups(container),
                DirectionGroups = GetDirectionGroups(container),
                Notes = GetNotes(container)
            };
        }

        protected string GetName(IWebElement container);
        protected string GetSummary(IWebElement container);
        protected List<Tag> GetTags(IWebElement container);
        protected string GetServingSize(IWebElement container);
        protected TimeGroup GetTimeGroup(IWebElement container);
        protected List<IngredientGroup> GetIngredientGroups(IWebElement container);
        protected List<DirectionGroup> GetDirectionGroups(IWebElement container);
        protected string GetNotes(IWebElement container);

    }
}
