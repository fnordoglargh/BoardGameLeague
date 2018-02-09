using BoardGameLeagueLib.DbClasses;
using BoardGameLeagueLib.Helpers;
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
        private static List<SolidColorBrush> m_ColorsNegative = new List<SolidColorBrush>
        {
            new SolidColorBrush(Color.FromRgb(255, 210, 198)),
            new SolidColorBrush(Color.FromRgb(255, 188, 178)),
            new SolidColorBrush(Color.FromRgb(255, 159, 153)),
            new SolidColorBrush(Color.FromRgb(255, 132, 132)),
            new SolidColorBrush(Color.FromRgb(255, 104, 109)),
            new SolidColorBrush(Color.FromRgb(255, 81, 90))
        };

        private static List<SolidColorBrush> m_ColorsPostive = new List<SolidColorBrush>
        {
            new SolidColorBrush(Color.FromRgb(206, 255, 226)),
            new SolidColorBrush(Color.FromRgb(186, 255, 199)),
            new SolidColorBrush(Color.FromRgb(137, 255, 149)),
            new SolidColorBrush(Color.FromRgb(112, 255, 119)),
            new SolidColorBrush(Color.FromRgb(73, 255, 86)),
            new SolidColorBrush(Color.FromRgb(48, 255, 58))
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
                    if (v_Index >= m_ColorsNegative.Count) v_Index = m_ColorsNegative.Count - 1;

                    return m_ColorsNegative[v_Index];
                }
            }
            else if (v_Row[v_RowIndex].Value.ToString() == "")
            {
                return Brushes.Black;
            }
            else if (v_Row[v_RowIndex].Value is Standing)
            {
                Standing v_Standing = v_Row[v_RowIndex].Value as Standing;
                v_ActualCellValue = v_Standing.Won - v_Standing.Lost;
                v_Index = v_ActualCellValue / 10;

                if (v_ActualCellValue == 0)
                {
                    return Brushes.Transparent;
                }
                else if (v_ActualCellValue > 0)
                {
                    if (v_Index >= m_ColorsPostive.Count) v_Index = m_ColorsPostive.Count - 1;
                    return m_ColorsPostive[v_Index];
                }
                else
                {
                    v_Index *= -1;
                    if (v_Index >= m_ColorsNegative.Count) v_Index = m_ColorsNegative.Count - 1;
                    return m_ColorsNegative[v_Index];
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
