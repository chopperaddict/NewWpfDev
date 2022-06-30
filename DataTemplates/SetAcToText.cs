using System;
using System . Globalization;
using System . Windows . Data;

namespace NewWpfDev . DataTemplates {
    public class SetAcToText : IValueConverter {
        public object Convert ( object value , Type targetType , object parameter , CultureInfo culture ) {
            short arg = 0;
            if ( value == null ) return null;
            arg = short . Parse ( value . ToString ( ) );
            if ( arg == 1 )
                return "Normal";
            else if ( arg == 2 )
                return "Business";
            else if ( arg == 3 )
                return "Savings";
            else if ( arg == 4 )
                return "Deposit";
            else return "";
        }
        public object ConvertBack ( object value , Type targetType , object parameter , CultureInfo culture ) {
            throw new NotImplementedException ( );
        }
    }
}
