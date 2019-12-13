using System;
using System.Collections.Generic;
using System.Text;

namespace Scrap.Model
{

    class DirectionGroup
    {
        public string Label { get; set; }
        public List<Direction> Directions { get; set; }
    }

    class Direction
    {
        public string Text { get; set; }
    }
}
