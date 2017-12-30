using LiveCharts;
using System;

namespace BoardGameLeagueUI.Charts
{
    public partial class LineChart
    {
        public LineChart()
        {
            Progression = new SeriesCollection();
            XFormatter = val => new DateTime((long)val).ToString("yyyy-MM");
            YFormatter = value => value.ToString();
        }

        public SeriesCollection Progression { get; set; }
        public Func<double, string> XFormatter { get; set; }
        public Func<double, string> YFormatter { get; set; }
    }
}
