
using System.Collections.Generic;

namespace Scrap.Model
{
    class TimeGroup
    {
        public List<Time> Times { get; set; }
    }

    class Time
    {
        public string Label { get; set; }
        public string Amount { get; set; }
        public string Unit { get; set; }
    }
}
