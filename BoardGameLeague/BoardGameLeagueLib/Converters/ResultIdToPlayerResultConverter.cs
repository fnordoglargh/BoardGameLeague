using BoardGameLeagueLib.DbClasses;
using System;
using System.Globalization;
using System.Windows.Data;

namespace BoardGameLeagueLib.Converters
{
    public class ResultIdToPlayerResultConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Guid v_Id;

            if(value is Guid)
            {
                v_Id = (Guid)value;
            }
            else
            {
                v_Id = new Guid();
            }

            DbHelper v_DbLoaderInstance = DbHelper.Instance;

            if (v_Id != new Guid() && v_DbLoaderInstance != null)
            {
                return v_DbLoaderInstance.LiveBglDb.PlayersById[v_Id];
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Player v_ActualPlayer = ((Player)value);

            return v_ActualPlayer.Id;
        }
    }
}
