using LiveCharts;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace BoardGameLeagueUI.Charts
{
    public partial class LineChart : INotifyPropertyChanged
    {
        public LineChart()
        {
            Progression = new SeriesCollection();
            m_XFormatter = val => new DateTime((long)val).ToString("yyyy-MM-dd");
            YFormatter = value => value.ToString();
            ZoomingMode = ZoomingOptions.X;
        }

        private Func<double, string> m_XFormatter;
        private ZoomingOptions m_ZoomingMode;

        public SeriesCollection Progression { get; set; }

        public Func<double, string> XFormatter
        {
            get { return m_XFormatter; }
            set { m_XFormatter = value; }
        }

        public ZoomingOptions ZoomingMode
        {
            get { return m_ZoomingMode; }
            set
            {
                m_ZoomingMode = value;
                NotifyPropertyChanged("ZoomingMode");
            }
        }

        public Func<double, string> YFormatter { get; set; }

        public void ToogleZoomingMode()
        {
            switch (ZoomingMode)
            {
                case ZoomingOptions.None:
                    ZoomingMode = ZoomingOptions.X;
                    break;
                case ZoomingOptions.X:
                    ZoomingMode = ZoomingOptions.Y;
                    break;
                case ZoomingOptions.Y:
                    ZoomingMode = ZoomingOptions.Xy;
                    break;
                case ZoomingOptions.Xy:
                    ZoomingMode = ZoomingOptions.None;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        internal void NotifyPropertyChanged(String a_PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(a_PropertyName));
        }

        #endregion
    }

    public class ZoomingModeCoverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((ZoomingOptions)value)
            {
                case ZoomingOptions.None:
                    return "None";
                case ZoomingOptions.X:
                    return "X";
                case ZoomingOptions.Y:
                    return "Y";
                case ZoomingOptions.Xy:
                    return "XY";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
