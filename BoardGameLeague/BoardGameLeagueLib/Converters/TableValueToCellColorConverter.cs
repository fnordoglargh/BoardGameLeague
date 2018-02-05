using BoardGameLeagueLib.DbClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace BoardGameLeagueUI.BoardGameLeagueLib.Converters
{
    public class TableValueToCellColorConverter : IMultiValueConverter
    {
        private static List<SolidColorBrush> m_Colors = new List<SolidColorBrush>
        {
            new SolidColorBrush(Color.FromRgb(255, 210, 198)),
            new SolidColorBrush(Color.FromRgb(255, 188, 178)),
            new SolidColorBrush(Color.FromRgb(255, 159, 153)),
            new SolidColorBrush(Color.FromRgb(255, 132, 132)),
            new SolidColorBrush(Color.FromRgb(255, 104, 109)),
            new SolidColorBrush(Color.FromRgb(255, 81, 90))
        };

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            DataGridCell v_Cell = values[0] as DataGridCell;

            if (v_Cell == null) return Brushes.Transparent;

            int v_RowIndex = CellIndexer.Instance.RowIndex;

            ObservableCollection<GenericProperty> v_Row = values[1] as ObservableCollection<GenericProperty>;

            if (v_Row == null) return Brushes.Transparent;

            int v_ActualCellValue = -1;
            int v_Index;

            if (int.TryParse(v_Row[v_RowIndex].Value.ToString(), out v_ActualCellValue))
            {
                if (v_ActualCellValue == 0)
                {
                    return Brushes.Transparent;
                }
                else
                {
                    v_Index = v_ActualCellValue / 10;
                    if (v_Index >= m_Colors.Count) v_Index = m_Colors.Count - 1;

                    return m_Colors[v_Index];
                }
            }
            else
            {
                return Brushes.Transparent;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
