using System;
using System . Globalization;
using System . Windows;
using System . Windows . Data;
//using System . Windows . Media;

namespace NewWpfDev
{
    public class UniversalConvert : IValueConverter
    {
        public object Convert ( object value , Type targetType , object parameter , CultureInfo culture )
        {
            string str = targetType . ToString ( );
            // check if we are requesting a Brush
            if ( str . Contains ( "System.Windows.Media.Brush" ) )
            {
                if ( ( string ) parameter == "" )
                    return    null;

                System . Windows . Media . SolidColorBrush brush = ( System . Windows . Media . SolidColorBrush ) Application . Current . FindResource ( parameter as string );
                return brush;
            }
            return value;
        }

        public object ConvertBack ( object value , Type targetType , object parameter , CultureInfo culture )
        {
            return value;
        }
    }

}
