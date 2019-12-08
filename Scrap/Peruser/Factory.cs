using OpenQA.Selenium;
using System;

namespace Scrap.Peruser
{
    class Factory
    {
        public IPeruser GetPeruser(IWebDriver driver)
        {
            if (WPRecipeMaker.CanPeruse(driver))
            {
                return new WPRecipeMaker();
            }

            throw new NotImplementedException();
        }
    }
}
