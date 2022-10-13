using System;
using System . Collections . Generic;
using System . Diagnostics;
using System . Globalization;
using System . Linq;
using System . Linq . Expressions;
using System . Text;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Data;

using DapperGenericsLib;

using DocumentFormat . OpenXml . Spreadsheet;

using NewWpfDev . AttachedProperties;

namespace NewWpfDev . Converts
{
    public class ReduceByParamValue : IValueConverter
    {
        /// <summary>
        /// Adds a dependency value received an XPath Converter parameter to move a textbolock downwrds to fit correctly
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert ( object value , Type targetType , object parameter , CultureInfo culture )
        {
            double currentvalue = 0;
            double paramvalue = 0;
            double d = 0;
            try
            {
                if ( value == null )
                    value = 1;
                if ( parameter == null )
                    parameter = 0;
                currentvalue = System . Convert . ToDouble ( value );
                paramvalue = System . Convert . ToDouble ( parameter );
                if ( currentvalue - paramvalue < 0 )
                    return 0;
                //Type t = targetType;
                //			Debug. WriteLine ( $"value = {value}, Parameter = {parameter}, TargetType={targetType}" );
                d = ( double ) currentvalue;
                currentvalue = d - paramvalue;
                $"Reduced = {currentvalue} by parameter value {parameter} to {currentvalue}" . cwinfo ( 0 );
                Debug . WriteLine ( $"ReduceByParamValue : value={value} + Parameter = {parameter}, result = {currentvalue}" );
                return currentvalue;
                //}
                //else
                //{
                //    d = ( double ) currentvalue;
                //    currentvalue = d - ( double ) 35;
                //    $"ReduceByParamValue Converter has returned {currentvalue} from {d} - 35" . cwinfo ( 0 );
                //}
            }
            catch ( Exception ex )
            {
                Debug . WriteLine ( $"ReduceByParamValue ERROR : value={value}, Parameter = {parameter}, Info - {ex . Message}" );
            }
            return currentvalue;
        }

        public object ConvertBack ( object value , Type targetType , object parameter , CultureInfo culture )
        {
            return value;
        }
    }
}
