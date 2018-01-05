using LiveCharts;
using System;

namespace BoardGameLeagueUI.Charts
{
    public partial class LineChart
    {
        public LineChart()
        {
            Progression = new SeriesCollection();
            m_XFormatter = val => new DateTime((long)val).ToString("yyyy-MM");
            YFormatter = value => value.ToString();
        }

        private Func<double, string> m_XFormatter;

        public SeriesCollection Progression { get; set; }
        public Func<double, string> XFormatter
        {
            get { return m_XFormatter; }
            set { m_XFormatter = value; }
        }
        public Func<double, string> YFormatter { get; set; }
    }
}
