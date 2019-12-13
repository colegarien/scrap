using System;
using System.Collections.Generic;
using System.Text;

namespace Scrap.Model
{
    class IngredientGroup
    {
        public string Label { get; set; }
        public List<Ingredient> Ingredients { get; set; }
    }

    class Ingredient
    {
        public string Amount { get; set; }
        public string Unit { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
    }
}
