using Recipefier.Domain.Model;
using Recipefier.Persuement.Exception;
using Recipefier.Persuement.Peruser;

namespace Recipefier.Persuement
{
    public class RecipePersuer
    {
        private readonly WebDriverFactory webDriverFactory;
        private readonly Factory persuerFactory;

        public RecipePersuer()
        {
            this.webDriverFactory = new WebDriverFactory();
            this.persuerFactory = new Factory();
        }

        /// <summary>
        /// Persue recipe at given URL
        /// </summary>
        /// <param name="url">URL for Recipe</param>
        /// <returns>Persued Recipe</returns>
        /// <exception cref="Recipefier.Persuement.Exception.CouldNotPersueException">Thrown when Recipe cannot be Persued</exception>
        public Recipe Persue(string url)
        {
            using (var driver = webDriverFactory.Get())
            {
                GoToSite(driver, url);
                var peruser = persuerFactory.GetPeruser(driver);
                return peruser.Peruse(driver);
            }
        }

        private void GoToSite(OpenQA.Selenium.IWebDriver driver, string url)
        {
            try
            {
                driver.Navigate().GoToUrl(url);
            }
            catch (System.Exception e)
            {
                throw new CouldNotPersueException("URL is invalid", e);
            }
        }
    }
}
