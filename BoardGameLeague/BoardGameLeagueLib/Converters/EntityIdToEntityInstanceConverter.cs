using BoardGameLeagueLib.DbClasses;
using System;
using System.Globalization;
using System.Windows.Data;

namespace BoardGameLeagueLib.Converters
{
    public class EntityIdToEntityInstanceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Guid v_Id;

            if (value is Guid)
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
                if (parameter.ToString() == "Game")
                {
                    return v_DbLoaderInstance.LiveBglDb.GamesById[v_Id];
                }
                else if (parameter.ToString() == "Location")
                {
                    return v_DbLoaderInstance.LiveBglDb.LocationsById[v_Id];
                }
                else if (parameter.ToString() == "Player")
                {
                    return v_DbLoaderInstance.LiveBglDb.PlayersById[v_Id];
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter.ToString() == "Game")
            {
                return ((Game)value).Id;
            }
            else if (parameter.ToString() == "Location")
            {
                return ((Location)value).Id;
            }
            else if (parameter.ToString() == "Player")
            {
                return ((Player)value).Id;
            }
            else
            {
                return null;
            }
        }
    }
}
