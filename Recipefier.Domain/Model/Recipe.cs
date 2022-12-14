using System;
using System.Collections.Generic;
using System.Text;

namespace Recipefier.Domain.Model
{
    public class Recipe
    {
        public string Name { get; set; }
        public string Source { get; set; }
        public string Summary { get; set; }

        public List<Tag> Tags { get; set; }
        public string Yield { get; set; }

        public TimeGroup TimeGroup { get; set; }

        public List<IngredientGroup> IngredientGroups { get; set; }
        public List<DirectionGroup> DirectionGroups { get; set; }

        public string Notes { get; set; }
    }
}
