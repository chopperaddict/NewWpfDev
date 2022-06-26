using NewWpfDev. ViewModels;

using System;
using System . Collections . Generic;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Media;

namespace NewWpfDev
{
	public static class Extensions
	{
	//	public static Brush ToSolidBrush ( this string HexColorString )
	//	{
	//		if ( HexColorString . Length < 9 )
	//		{
	//			//				MessageBox.Show( "The Hex value entered is invalid. It needs to be # + 4 hex pairs\n\neg: [#FF0000FF] = BLUE ");
	//			return null;
	//		}
	//		try
	//		{
	//			if ( HexColorString != null && HexColorString != "" )
	//				return ( Brush ) ( new BrushConverter ( ) . ConvertFrom ( HexColorString ) );
	//			else
	//				return null;
	//		}
	//		catch ( Exception ex )
	//		{
	//			Debug. WriteLine ( $"ToSolidbrush failed - input = {HexColorString}" );
	//			return null;
	//		}
	//	}
	//	public static LinearGradientBrush ToLinearGradientBrush ( this string Colorstring )
	//	{
	//		try
	//		{
	//			return Application . Current . FindResource ( Colorstring ) as LinearGradientBrush;
	//		}
	//		catch ( Exception ex )
	//		{
	//			Debug. WriteLine ( $"ToLinearGradientbrush failed - input = {Colorstring}" );
	//			return null;
	//		}
	//		//return ( LinearGradientBrush ) ( new BrushConverter ( ) . ConvertFrom ( color ) );
	//	}
	//	public static string BrushtoText ( this Brush brush )
	//	{
	//		try
	//		{
	//			if ( brush != null )
	//				return ( string ) brush . ToString ( );
	//			else
	//				return null;
	//		}
	//		catch ( Exception ex )
	//		{
	//			Debug. WriteLine ( $"BrushtoText failed - input = {brush }" );
	//			return null;
	//		}
	//	}
	//	public static string ToBankRecordCommaDelimited ( this BankAccountViewModel record )
	//	{
	//		BankAccountViewModel bvm = new  BankAccountViewModel();
	//		string [] fields ={ "","","","","","","","",""};
	//		fields [ 0 ] = record . Id . ToString ( );
	//		fields [ 1 ] = record . BankNo . ToString ( );
	//		fields [ 2 ] = record . CustNo . ToString ( );
	//		fields [ 3 ] = record . Balance . ToString ( );
	//		fields [ 4 ] = record . IntRate . ToString ( );
	//		fields [ 5 ] = record . AcType . ToString ( );
	//		fields [ 6 ] = record . ODate . ToString ( );
	//		fields [ 7 ] = record . CDate . ToString ( );
	//		return fields [ 0 ] + "," + fields [ 1 ] + "," + fields [ 2 ] + "," + fields [ 3 ] + "," + fields [ 4 ] + "," + fields [ 5 ] + "," + fields [ 6 ] + "," + fields [ 7 ] + "\n";
	//	}
	}

}
