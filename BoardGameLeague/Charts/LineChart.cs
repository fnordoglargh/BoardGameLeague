using LiveCharts;
using System;

namespace BoardGameLeagueUI.Charts
{
    public partial class LineChart
    {
        public LineChart()
        {
            Progression = new SeriesCollection();
            Labels = val => new DateTime((long)val).ToString("yyyy"); 
            YFormatter = value => value.ToString();
        }

        public SeriesCollection Progression { get; set; }
        public Func<double, string> Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }
    }
}
