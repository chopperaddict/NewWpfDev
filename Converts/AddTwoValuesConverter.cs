
using System;
using System . Diagnostics;
using System . Globalization;
using System . Windows . Data;

namespace NewWpfDev . Converts {
    public class AddTwoValuesConverter : IMultiValueConverter {
        public object Convert ( object [ ] values , Type targetType , object parameter , CultureInfo culture ) {
            double dblval1 = 0.0, dblval2 = 0.0;
            object result = null;
            object [  ] objout = new object [3 ] ;
            objout [ 0 ] = values [ 1 ];     // ConverterParameter
            objout [ 1 ] = parameter;     // parameter
            objout [ 2 ] = values [ 0 ];     // main value received (some parent object usually
            //Debug . WriteLine ( $"Converter arguments are :\n{objout [ 0 ] ?. ToString ( )},\n{objout [ 1 ]? . ToString ( )},\n{objout [ 2 ]? . ToString ( )}\n" );
            
            return objout ;

            int intval1 = 0, intval2 = 0;
            dblval1 = System . Convert . ToDouble ( values [ 0 ] . ToString ( ) );
            dblval2 = System . Convert . ToDouble ( values [ 1 ] . ToString ( ) );
            if ( dblval1 > 0.0 && dblval2 > 0.0 ) {
                double resdbl = dblval1 + dblval2;
                return resdbl;
            }
            else {
                intval1 = System . Convert . ToInt32 ( values [ 0 ] . ToString ( ) );
                intval2 = System . Convert . ToInt32 ( values [ 1 ] . ToString ( ) );
                if ( intval1 > 0 && intval2 > 0 ) {
                    int resint = intval1 + intval2;
                    return resint;
                }
            }
            return values [ 0 ];
        }

        public object [ ] ConvertBack ( object value , Type [ ] targetTypes , object parameter , CultureInfo culture ) {
            throw new NotImplementedException ( );
        }
    }
}
