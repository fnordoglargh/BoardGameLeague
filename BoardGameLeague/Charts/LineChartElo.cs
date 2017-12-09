using System;
using System.Windows.Controls;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;

namespace BoardGameLeagueUI.Charts
{
    public partial class LineChartElo
    {
        public LineChartElo()
        {
            EloProgression = new SeriesCollection();
            Labels = val => new DateTime((long)val).ToString("yyyy"); 
            YFormatter = value => value.ToString();
        }

        public SeriesCollection EloProgression { get; set; }
        public Func<double, string> Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }
    }
}
