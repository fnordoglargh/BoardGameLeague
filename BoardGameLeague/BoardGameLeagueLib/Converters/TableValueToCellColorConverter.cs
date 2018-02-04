using BoardGameLeagueLib.DbClasses;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace BoardGameLeagueUI.BoardGameLeagueLib.Converters
{
    public class TableValueToCellColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            DataGridCell v_Cell = values[0] as DataGridCell;

            if (v_Cell == null) return Brushes.Transparent;

            int v_RowIndex = CellIndexer.Instance.RowIndex;

            ObservableCollection<Property> v_Row = values[1] as ObservableCollection<Property>;

            if (v_Row == null) return Brushes.Transparent;

            int v_ActualCellValue = -1;

            if (int.TryParse(v_Row[v_RowIndex].Value.ToString(), out v_ActualCellValue))
            {
                if (v_ActualCellValue == 0)
                {
                    return Brushes.Transparent;
                }
                else
                {
                    return Brushes.LightSalmon;
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
