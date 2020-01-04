using System;
using System.Collections.Generic;
using System.Text;

namespace Recipefier.Persuement.Model
{

    public class DirectionGroup
    {
        public string Label { get; set; }
        public List<Direction> Directions { get; set; }
    }

    public class Direction
    {
        public string Text { get; set; }
    }
}
