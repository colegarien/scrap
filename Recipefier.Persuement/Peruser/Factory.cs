using OpenQA.Selenium;
using Recipefier.Persuement.Exception;
using Recipefier.Persuement.Peruser.WordpressPlugin;

namespace Recipefier.Persuement.Peruser
{
    class Factory
    {
        private readonly IPeruser[] perusers;

        public Factory()
        {
            perusers = new IPeruser[] {
                new WPRecipeMaker(), 
                new WPUltimate(), 
                new TastyRecipes(), 
                new ZipRecipesZL(), 
                new ZipRecipeZip(), 
                new CookedRecipe(), 
                new RecipeCardBlocks()
            };
        }

        public IPeruser GetPeruser(IWebDriver driver)
        {
            foreach (var peruser in perusers)
            {
                if (peruser.CanPeruse(driver))
                {
                    return peruser;
                }
            }

            throw new CouldNotPersueException("No applicable Persuer");
        }
    }
}
