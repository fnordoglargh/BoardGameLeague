using BoardGameLeagueLib.DbClasses;
using System;
using System.Globalization;
using System.Windows.Data;

namespace BoardGameLeagueLib
{
    public class ResultIdToPlayerResultConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Guid v_Id = (Guid)value;
            DbHelper v_DbLoaderInstance = DbHelper.Instance;
            Player v_ActualPlayer = v_DbLoaderInstance.LiveBglDb.PlayersById[v_Id];

            return v_ActualPlayer;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Player v_ActualPlayer = ((Player)value);

            return v_ActualPlayer.Id;
        }
    }
}
