﻿using System;
using System . Collections . Generic;
using System . Globalization;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows . Data;

namespace NewWpfDev. Converts
{
	class DoubleToInt : IValueConverter
	{
		public object Convert ( object value , Type targetType , object parameter , CultureInfo culture )
		{
			//			int toBase = 16;
			int val = System . Convert . ToInt32 ( value );
			return val;
		}

		public object ConvertBack ( object value , Type targetType , object parameter , CultureInfo culture )
		{
			double temp = System . Convert . ToDouble ( value );
			//if ( temp <= 255 )
			//	return ( string ) temp . ToString ( "X2" );
			//else if ( temp <= 65535 )
			//	return ( string ) temp . ToString ( "X4" );
			//else if ( temp <= 16777215 )
			//	return ( string ) temp . ToString ( "X6" );

			return ( double ) temp;
		}
	}
}
