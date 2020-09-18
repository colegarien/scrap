using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace Recipefier.Persuement
{
    class WebDriverFactory
    {
        public IWebDriver Get()
        {
            var options = new ChromeOptions();
            options.AddArgument("disable-translate");
            options.AddArgument("disable-infobars");
            options.AddArgument("headless");
            options.AddArgument("disable-gpu");
            options.AddArgument("window-size=1024,768");
            options.AddArgument("log-level=1"); // Warnings and up
            var serice = ChromeDriverService.CreateDefaultService(@"C:\\Program Files (x86)\\Google\\", "chromedriver.exe");
            return new ChromeDriver(serice, options);

            /*var options = new FirefoxOptions();
            options.AddArguments("-headless");

            var service = FirefoxDriverService.CreateDefaultService(@"C:\\Program Files (x86)\\Mozilla Firefox\\", "geckodriver.exe");
            return new FirefoxDriver(service, options);*/
        }
    }
}
