using System.Collections.Generic;

namespace iBank.Operator.Desktop.Data
{
    public class Entry
    {
        public List<Formirovanie> People { get; set; } = new List<Formirovanie>();
        public int MonthInt { get; set; }
        public string Month { get; set; } = string.Empty;
        public string Count { get; set; } = string.Empty;
    }
}
