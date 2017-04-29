using System;
using System.Globalization;
using System.Windows.Data;

namespace BoardGameLeagueLib
{
    //class IdToGameFamilyConverter : IMultiValueConverter
    //{


    //    public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        Guid v_id = (Guid)value[0];
    //        DbClass v_TempDatabase = (DbClass)parameter;
    //        GameFamily v_GameFamily = (GameFamily)v_TempDatabase.GameFamiliesById[v_id];

    //        return v_GameFamily;
    //    }

    //    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }

    //}


    public class IdToGameFamilyConverter : IValueConverter
    {


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string retValue = String.Empty;

            if (value != null)
            {

                Guid v_id = (Guid)value;
                GameFamily v_GameFamily = (GameFamily)parameter;

                //Console.WriteLine("Convert value    : " + value);
                //Console.WriteLine("Convert parameter: " + parameter);

                //retValue = v_id.ToString();
            }
            else
            {
            }

            return retValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

    }
}
