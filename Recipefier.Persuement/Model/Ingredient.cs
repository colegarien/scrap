using System;
using System.Collections.Generic;
using System.Text;

namespace Recipefier.Persuement.Model
{
    public class IngredientGroup
    {
        public string Label { get; set; }
        public List<Ingredient> Ingredients { get; set; }
    }

    public class Ingredient
    {
        public string Amount { get; set; }
        public string Unit { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
    }
}
