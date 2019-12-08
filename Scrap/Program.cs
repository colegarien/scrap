using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace Scrap
{
    class Program
    {

        static void Main(string[] args)
        {
            var options = new ChromeOptions();
            var serice = ChromeDriverService.CreateDefaultService(@"C:\\Program Files (x86)\\Google\\", "chromedriver.exe");
            IWebDriver driver = new ChromeDriver(serice, options);

            var targetRecipeUrl = "https://thesaltymarshmallow.com/homemade-belgian-waffle-recipe/";
            driver.Navigate().GoToUrl(targetRecipeUrl);

            var persuerFactory = new Peruser.Factory();
            var peruser = persuerFactory.GetPeruser(driver);
            peruser.Peruse(driver);

            driver.Close();
            Console.WriteLine("Complete.");
        }
    }
}
