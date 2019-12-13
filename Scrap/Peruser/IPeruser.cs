using OpenQA.Selenium;

namespace Scrap.Peruser
{
    interface IPeruser
    {
        public static bool CanPeruse(IWebDriver driver)
        {
            return false;
        }

        public Model.Recipe Peruse(IWebDriver driver);
    }
}
