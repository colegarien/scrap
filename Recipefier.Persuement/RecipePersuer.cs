using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Recipefier.Persuement.Model;
using Recipefier.Persuement.Peruser;
using System;

namespace Recipefier.Persuement
{
    public class RecipePersuer
    {
        public Recipe Persue(string url)
        {
            var options = new ChromeOptions();
            options.AddArgument("disable-translate");
            options.AddArgument("disable-infobars");
            options.AddArgument("headless");
            options.AddArgument("disable-gpu");
            options.AddArgument("window-size=1024,768");
            options.AddArgument("log-level=1"); // Warnings and up
            var serice = ChromeDriverService.CreateDefaultService(@"C:\\Program Files (x86)\\Google\\", "chromedriver.exe");
            IWebDriver driver = new ChromeDriver(serice, options);

            var persuerFactory = new Factory();

            var recipe = new Recipe();
            try
            {
                driver.Navigate().GoToUrl(url);
                var peruser = persuerFactory.GetPeruser(driver);
                recipe = peruser.Peruse(driver);
            }
            catch (Exception)
            {
                // hmmm
            }

            driver.Close();
            return recipe;
        }
    }
}
