using System.Collections.Generic;

namespace Recipefier.Persuement.Model
{
    public class TimeGroup
    {
        public List<Time> Times { get; set; }
    }

    public class Time
    {
        public string Label { get; set; }
        public string Amount { get; set; }
        public string Unit { get; set; }
    }
}
