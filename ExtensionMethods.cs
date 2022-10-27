#define USECW
#define USETRACK
#undef USETRACK
using System;
using System . Collections . Generic;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows . Threading;
using System . Windows;
using NewWpfDev . ViewModels;
using System . Windows . Media;
using System . Diagnostics;
using System . Runtime . CompilerServices;
using System . Windows . Controls;
using System . IO;

namespace NewWpfDev
{
    public static class ExtensionMethods
    {

        private static Action EmptyDelegate = delegate ( ) { };
        //Snippet = trk
        //[Conditional ( "USECW" )]
        public static void Track (
            this string message ,
            int direction = 0 ,
            int level = 1 ,
            [CallerFilePath] string path = null ,
            [CallerMemberName] string name = null ,
            [CallerLineNumber] int line = -1 )
        {
#if USETRACK
            if ( level == 0 ) return;
            string [ ] tmp = path . Split ( '\\' );
            int len = tmp . Length;
            string filedetails = $"{tmp [ len - 3 ]}/{tmp [ len - 2 ]}/{tmp [ len - 1 ]}";
            if ( MainWindow . LOGTRACK )
            {
                if ( direction == 0 )
                    File . AppendAllText ( $@"C:\users\ianch\Documents\NewWpfDev.Trace.log" , $"** IN  ** : {line} :  {filedetails} : {name . ToUpper ( )}\n" );
                else
                    File . AppendAllText ( $@"C:\users\ianch\Documents\NewWpfDev.Trace.log" , $"** OUT ** : {line} :  {filedetails} : {name . ToUpper ( )}\n" );

            }
            if ( direction == 0 )
                Debug . WriteLine ( $"** TRACK - IN ** : {line} :  {filedetails} : {name . ToUpper ( )}" );
            else
                Debug . WriteLine ( $"** TRACK - OUT ** : {line} :  {filedetails} : {name . ToUpper ( )}" );
#endif
        }
        public static void log (
            this string message ,
            [CallerLineNumber] int line = -1 ,
            [CallerFilePath] string path = null ,
            [CallerMemberName] string name = null )
        {
            string output = "";
            output = path == null ? "No file path" : $"\t{path}  : ";
            output += line < 0 ? "No line  : " : "Line " + $"{line} : ";
            output += name == null ? " No member name" : $"( {name} )";
            Debug . WriteLine ( $"{output}\n{message}" );
        }

        public static void DapperTrace (
            this string message ,
            int level = 1 ,
            [CallerFilePath] string path = null ,
            [CallerMemberName] string name = null ,
            [CallerLineNumber] int line = -1 )
        {
            Debug . WriteLine ( $"\nExecuting : [ {message . ToUpper ( )} ]\n{path}\\{name} : {line}\n" );
        }

        public static void CW (
            this string message ,
            int level = 1 ,
            [CallerFilePath] string path = null ,
            [CallerMemberName] string name = null ,
            [CallerLineNumber] int line = -1 )
        {
            if ( level == 0 ) return;
            string [ ] tmp = path . Split ( '\\' );
            string errmsg = $"\n{name} : {line} ";
            errmsg += $"in {tmp [ tmp . Length - 2 ]}";
            errmsg += $"\\{tmp [ tmp . Length - 1 ]}\n**INFO** = [  {message} ]";
            Debug . WriteLine ( $"\n{errmsg}" );
            if ( MainWindow . LogCWOutput )
                File . AppendAllText ( @"C:\users\ianch\documents\CW.log" , errmsg );
        }
        //-------------------------------------------------------------------------------------------------------//
        //Snippet = cwe
        //[Conditional ( "USECW" )]
        public static void cwerror (
            this string message ,
            int level = 1 ,
            [CallerFilePath] string path = null ,
            [CallerMemberName] string name = null ,
            [CallerLineNumber] int line = -1 )
        {
            if ( level == 0 ) return;
            string [ ] tmp = path . Split ( '\\' );
            Debug . WriteLine ( $"** ERROR ** : {line} ** {message} ** : : {name} (.) in {tmp [ 5 ] + "\\" + tmp [ 6 ]}" );
        }
        //-------------------------------------------------------------------------------------------------------//
        //Snippet = cww
        //[Conditional ( "USECW" )]
        public static void cwwarn (
            this string message ,
            int level = 1 ,
            [CallerFilePath] string path = null ,
            [CallerMemberName] string name = null ,
            [CallerLineNumber] int line = -1 )
        {
            if ( level == 0 ) return;
            string [ ] tmp = path . Split ( '\\' );
            Debug . WriteLine ( $"WARN : {line} ** {message} ** : : {name} (.) in {tmp [ 5 ] + "\\" + tmp [ 6 ]}" );
        }
        //-------------------------------------------------------------------------------------------------------//
        //Snippet = cwi
        //[Conditional ( "USECW" )]
        public static void cwinfo (
            this string message ,
            int level = 1 ,
            [CallerFilePath] string path = null ,
            [CallerMemberName] string name = null ,
            [CallerLineNumber] int line = -1 )
        {
            if ( level == 0 ) return;
            string [ ] tmp = path . Split ( '\\' );
            string namestr = $"{name + " ()" . PadRight ( 25 )}";
            Debug . WriteLine ( $"INFO : {line . ToString ( ) . PadRight ( 6 )} : {namestr} ::** {message . PadRight ( 20 )}  : : File= {tmp [ 5 ] + "\\" + tmp [ 6 ]}" );
        }
        //-------------------------------------------------------------------------------------------------------//

        public static void Refresh ( this UIElement uiElement )
        {
            try
            {
                uiElement . Dispatcher . Invoke ( DispatcherPriority . Render , EmptyDelegate );
            }
            catch ( Exception ex ) { Debug . WriteLine ( $"REFRESH FAILEd !!  {ex . Message}" ); }
        }

        public static void RefreshGrid ( this Control uiElement )
        {
            try
            {
                uiElement . Dispatcher . Invoke ( DispatcherPriority . Render , EmptyDelegate );
            }
            catch ( Exception ex ) { Debug . WriteLine ( $"REFRESH FAILEd !!  {ex . Message}" ); }
        }


        public static Brush ToSolidColorBrush ( this string HexColorString )
        {
            if ( HexColorString . Length != 9 )
            {
                MessageBox . Show ( "The Hex value entered is invalid. It needs to be # + 4 hex pairs\n\neg: [#FF0000FF] = BLUE " );
                return null;
            }
            try
            {
                if ( HexColorString != null && HexColorString != "" )
                    return ( SolidColorBrush ) System . Windows . Application . Current . FindResource ( HexColorString );
                else
                    return null;
            }
            catch ( Exception ex )
            {
                Debug . WriteLine ( $"ToSolidColorBrush failed - input = {HexColorString}" );
                return null;
            }
        }

        public static Brush ToSolidBrush ( this string HexColorString )
        {
            if ( HexColorString . Length < 9 )
            {
                //				MessageBox.Show( "The Hex value entered is invalid. It needs to be # + 4 hex pairs\n\neg: [#FF0000FF] = BLUE ");
                return null;
            }
            try
            {
                if ( HexColorString != null && HexColorString != "" )
                    return ( Brush ) ( new BrushConverter ( ) . ConvertFrom ( HexColorString ) );
                else
                    return null;
            }
            catch ( Exception ex )
            {
                Debug . WriteLine ( $"ToSolidbrush failed - input = {HexColorString}" );
                return null;
            }
        }
        public static LinearGradientBrush ToLinearGradientBrush ( this string Colorstring )
        {
            try
            {
                return Application . Current . FindResource ( Colorstring ) as LinearGradientBrush;
            }
            catch ( Exception ex )
            {
                Debug . WriteLine ( $"ToLinearGradientbrush failed - input = {Colorstring}" );
                return null;
            }
            //return ( LinearGradientBrush ) ( new BrushConverter ( ) . ConvertFrom ( color ) );
        }
        public static string BrushtoText ( this Brush brush )
        {
            try
            {
                if ( brush != null )
                    return ( string ) brush . ToString ( );
                else
                    return null;
            }
            catch ( Exception ex )
            {
                Debug . WriteLine ( $"BrushtoText failed - input = {brush}" );
                return null;
            }
        }
        public static string ToBankRecordCommaDelimited ( this BankAccountViewModel record )
        {
            BankAccountViewModel bvm = new BankAccountViewModel ( );
            string [ ] fields = { "" , "" , "" , "" , "" , "" , "" , "" , "" };
            fields [ 0 ] = record . Id . ToString ( );
            fields [ 1 ] = record . BankNo . ToString ( );
            fields [ 2 ] = record . CustNo . ToString ( );
            fields [ 3 ] = record . Balance . ToString ( );
            fields [ 4 ] = record . IntRate . ToString ( );
            fields [ 5 ] = record . AcType . ToString ( );
            fields [ 6 ] = record . ODate . ToString ( );
            fields [ 7 ] = record . CDate . ToString ( );
            return fields [ 0 ] + "," + fields [ 1 ] + "," + fields [ 2 ] + "," + fields [ 3 ] + "," + fields [ 4 ] + "," + fields [ 5 ] + "," + fields [ 6 ] + "," + fields [ 7 ] + "\n";
        }
    }

}