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
            else if (WPUltimate.CanPersue(driver))
            {
                return new WPUltimate();
            }

            throw new NotImplementedException();
        }
    }
}
