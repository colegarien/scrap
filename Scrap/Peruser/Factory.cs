using OpenQA.Selenium;
using System;

namespace Scrap.Peruser
{
    class Factory
    {
        IPeruser[] perusers;
        public Factory()
        {
            perusers = new IPeruser[] { new WPRecipeMaker(), new WPUltimate(), new TastyRecipes(), new ZipRecipesZL(), new ZipRecipeZip() };
        }

        public IPeruser GetPeruser(IWebDriver driver)
        {
            foreach(var peruser in perusers)
            {
                if (peruser.CanPeruse(driver))
                {
                    return peruser;
                }
            }

            throw new NotImplementedException();
        }
    }
}
