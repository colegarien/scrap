using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Recipefier.Persuement.Peruser.Utilities
{

    enum PullType
    {
        BY_CLASS = 0,
        BY_NAME = 1,
        BY_ID = 2,
        BY_TAG = 3,
        BY_XPATH = 4,
    }

    class Puller
    {
        public IWebElement GetOne(ISearchContext element, string key, PullType type = PullType.BY_CLASS)
        {
            return GetMany(element, key, type).LastOrDefault();
        }

        public List<IWebElement> GetMany(ISearchContext element, string key, PullType type = PullType.BY_CLASS)
        {
            return element?.FindElements(DetermineBy(type, key))?.ToList() ?? new List<IWebElement>();
        }

        private By DetermineBy(PullType type, string key)
        {
            switch (type)
            {
                case PullType.BY_TAG:
                    return By.TagName(key);
                case PullType.BY_NAME:
                    return By.Name(key);
                case PullType.BY_ID:
                    return By.Id(key);
                case PullType.BY_XPATH:
                    return By.XPath(key);
                case PullType.BY_CLASS:
                default:
                    return By.ClassName(key);
            }
        }

        public string GetAttribute(IWebElement element, string key, string attribute, PullType type = PullType.BY_CLASS)
        {
            return GetOne(element, key, type)?.GetAttribute(attribute) ?? "";
        }

        public string GetText(IWebElement element, string key, PullType type = PullType.BY_CLASS)
        {
            var firstElement = GetOne(element, key, type);
            var text = "";
            if (firstElement != null)
            {
                text = GetOne(firstElement, key, type)?.Text
                    ?? firstElement.Text;
            }

            return CleanText(text);
        }

        public string CleanText(string text)
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
                .Replace("<em>", "")
                .Replace("</em>", "")
                .Replace("  ", " ")
                .Replace("  ", " ")
                .Trim();

            // remove links
            text = Regex.Replace(text, "<a .*>", "");
            text = Regex.Replace(text, "<span .*>", "");

            return text.Replace("  ", " ").Trim();
        }
    }
}
