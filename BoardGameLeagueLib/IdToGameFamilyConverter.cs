//using System;
//using System.Globalization;

//namespace BoardGameLeagueLib
//{
//    class IdToGameFamilyConverter : IValueConverter
//    {


//        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            Guid v_id = (Guid)value;
//            GameFamily v_GameFamily = (GameFamily)parameter;

//            Console.WriteLine("Convert value    : " + value);
//            Console.WriteLine("Convert parameter: " + parameter);

//            string retValue = v_id.ToString();
//            return retValue;
//        }

//        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            throw new NotSupportedException();
//        }

//    }
//}
