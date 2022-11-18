using System;
using System . Collections . Generic;
using System . ComponentModel;
using System . Configuration;
using System . Diagnostics . Eventing . Reader;
using System . IO;
using System . Printing;
using System . Text;
using System . Threading;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Controls . Ribbon;
using System . Windows . Data;
using System . Windows . Documents;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Media . Imaging;
using System . Windows . Media . TextFormatting;
using System . Windows . Navigation;
using System . Windows . Shapes;
using System . Xml . Linq;

using Newtonsoft . Json . Linq;

using Views;

namespace NewWpfDev
{
    /// <summary>
    /// Interaction logic for SpArguments.xaml
    /// </summary>
    public partial class SpArguments : Window
    {
        TextBox Parent = null;
        public SpResultsViewer Resviewer = null;
        public bool KeepTypes { get; set; } = false;
        public SpArguments ( TextBox parent )
        {
            string [ ] array;
            InitializeComponent ( );
            WpfLib1 . Utils . SetupWindowDrag ( this );

            Parent = parent;
            Window win = new Window ( );
        }
        private void Window_Loaded ( object sender , RoutedEventArgs e )
        {
            KeepTypes = ( bool ) MainWindow . GetSystemSetting ( "AutoCloseSpArgumentsViewer" );
            AutoClose . IsChecked = KeepTypes;
            KeepTypes = ( bool ) MainWindow . GetSystemSetting ( "ShowTypesInSpArgumentsString" );
            ShowTypes . IsChecked = KeepTypes;
        }
        private void Button_Click ( object sender , RoutedEventArgs e )
        {
            this . Close ( );
        }

        private void Paste_Click ( object sender , RoutedEventArgs e )
        {
            string tmp = "";
            string output = "";
            string buff = "";
            string interim = "";
            int arraycount = 0;
            string [ ] arg = null;
            string [ ] element = null;
            int offset = 0;
            string [ ] initial = SPHeaderblock . Text . Split ( "\n" );

            for ( int x = 0 ; x < initial . Length ; x++ )
            {   // Check for empty entries after split
                if ( initial [ x ] == null || initial [ x ] . Length != 0 )
                    arraycount++;
            }

            string [ ] parts = new string [ arraycount ];
            string fullarg = "";
            // clean up the entries first
            for ( int x = 0 ; x < arraycount ; x++ )
            {
                parts [ x ] = initial [ x ] . TrimEnd ( ) . TrimStart ( );
            }
            for ( int x = 0 ; x < parts . Length ; x++ )
            {
                // strip [  Input : ] presention text from each line in parts[]
                if ( parts [ x ] . ToUpper ( ) . Contains ( "INPUT : " ) )
                    parts [ x ] = parts [ x ] . Substring ( 8 );
                if ( parts [ x ] . ToUpper ( ) . Contains ( "OUTPUT : " ) )
                    parts [ x ] = parts [ x ] . Substring ( 9 );
                parts [ x ] = CheckForComment ( parts [ x ] );
            }
            arg = parts;
            for ( int y = 0 ; y < arg . Length ; y++ )
            {
                if ( arg [ y ] . Contains ( "CREATE PROCEDURE" ) )
                    continue;
                // get next argument line
                string input = arg [ y ];
                parts = arg [ y ] . Split ( ' ' );

                //strip leading/Trailing commas
                for ( int x = 0 ; x < parts . Length ; x++ )
                {
                    parts [ x ] = CheckForCommas ( parts [ x ] );
                }
                // now we can parse current phrase out
                for ( int x = 0 ; x < parts . Length ; x++ )
                {
                    // is it an argument name ?
                    if ( parts [ x ] . StartsWith ( "@" ) )
                    {
                        fullarg = parts [ x ];
                        continue;
                    }
                    // check for various data Type identifiers
                    interim = CheckForVarchar ( KeepTypes , parts [ x ] . Trim ( ) );
                    fullarg += $" {interim}";
                    continue;
               }
                buff = fullarg;
                if ( output . Length == 0 )
                    output += $"{buff}";
                else
                    output += $", {buff}";
            }

            Clipboard . SetText ( output );
            Parent . Text = output;
             if ( AutoClose . IsChecked == true )
                this . Close ( );

        }
        public string CheckTypesRequired ( bool KeepTypes , string input )
        {
            if ( KeepTypes == false
                && ( input . ToUpper ( ) . Contains ( "VARCHAR" ) == true
                || input . ToUpper ( ) . Contains ( "BIT" ) == true
                || input . ToUpper ( ) . Contains ( "SYSNAME" ) == true
                || input . ToUpper ( ) . Contains ( "(MAX)" ) == true ) )
                input = "";
            else if ( KeepTypes == true
                && ( input . ToUpper ( ) . Contains ( "VARCHAR" ) == true
                || input . ToUpper ( ) . Contains ( "BIT" ) == true
                || input . ToUpper ( ) . Contains ( "SYSNAME" ) == true
                || input . ToUpper ( ) . Contains ( "(MAX)" ) == true ) )
                input = GetReplacementTypestring ( input );
            else
            {
            }
            return input;
        }
        public string GetReplacementTypestring ( string input )
        {
            string output = "";
            if ( input . ToUpper ( ) . Contains ( "VARCHAR" ) )
                output = " string ";
            else if ( input . ToUpper ( ) . Contains ( "BIT" ) )
                output = " Bit ";
            else if ( input . ToUpper ( ) . Contains ( "SYSNAME" ) )
                output = " string ";
            else if ( input . ToUpper ( ) . Contains ( "(MAX)" ) )
                output = " string ";
            else if ( input . ToUpper ( ) . Contains ( "INT" ) )
                output = " int ";
            else if ( input . ToUpper ( ) . Contains ( "DOUBLE" ) )
                output = " double ";
            else if ( input . ToUpper ( ) . Contains ( "FLOAT" ) )
                output = " float ";
            return output;
        }
        public string CleanEntry ( bool KeepTypes , string input )
        {
            string output = "";
            string tmpbuff = "";
            string [ ] split;
            if ( input . Contains ( ',' ) )
            {
                split = input . Split ( ',' );
                if ( split [ 0 ] . Length > split [ 1 ] . Length )
                    tmpbuff = split [ 0 ];
                else
                    tmpbuff = split [ 1 ];
                input = tmpbuff;
            }
            if ( input . Contains ( ']' ) )
            {
                split = input . Split ( ']' );
                if ( split [ 0 ] . Length > split [ 1 ] . Length )
                    tmpbuff = split [ 0 ];
                else
                    tmpbuff = split [ 1 ];
                input = tmpbuff;
            }
            output = input . Trim ( );
            return output;
        }
        /// <summary>
        /// Routine that checks for varchar or nvarchar or max in SP arguments
        /// and replaces them with the more legal 'string' nomenclature for the arguments string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string CheckForVarchar ( bool KeepTypes , string input )
        {
            string output = "";
            string [ ] parts = input . Split ( ' ' );
            string buff = "";
            string stringbuff = "";
            string outputbuff = "";
            // see if there is a default value in this string
            string defvalue = CheckForDefaultValue ( input );

            if ( input . ToUpper ( ) . Contains ( "OUTPUT" ) )
            {
                int offset = input . IndexOf ( "OUTPUT" );
                input = input . Substring ( 0 , offset ) . Trim ( );
                outputbuff = " Output";
            }
            if ( parts . Length == 1 )
            {
                if ( input . ToUpper ( ) . Contains ( "VARCHAR" )
                || input . ToUpper ( ) . Contains ( "SYSNAME" )
                || input . ToUpper ( ) . Contains ( "(MAX)" ) )
                {
                    //Its a string type
                    if ( KeepTypes )
                        stringbuff = "[string]";

                    //                   if ( KeepTypes == true )
                    buff += $"{defvalue}";
                    output += $"{buff}{stringbuff}";
                }
                else
                    return $"{parts [ 0 ]}";
            }
            else
            {
                string outbuff = "";
                for ( int x = 0 ; x < parts . Length ; x++ )
                {
                    if ( parts [ x ] . Contains ( '@' ) )
                    {
                        buff = parts [ 0 ];
                        buff = CheckForCommas ( parts [ 0 ] );
                        continue;
                    }
                    input = parts [ x ];
                    if ( input . ToUpper ( ) . Contains ( "VARCHAR" )
                         || input . ToUpper ( ) . Contains ( "SYSNAME" )
                         || input . ToUpper ( ) . Contains ( "(MAX)" ) )
                    {
                        //Its a string type
                        if ( KeepTypes ) 
                            stringbuff = " [string]";

                        if ( KeepTypes == true )    // see if we have a default value
                            buff += $"{defvalue}";
                        output += $"{buff}{stringbuff}";
                    }
                    //}
                }
            }
            return $"{output}";
        }

        public string CheckForCommas ( string input )
        {
            string output = input;
            if ( input . Contains ( ',' ) )
            {
                string [ ] tmp = input . Split ( ',' );
                if ( tmp [ 0 ] . Length > tmp [ 1 ] . Length )
                    output = tmp [ 0 ];
                else
                    output = tmp [ 1 ];
            }
            return output;
        }
        public string CheckForDefaultValue ( string input )
        {
            string output = "";
            if ( input . Contains ( '=' ) )
            {
                string [ ] parts = input . Split ( '=' );
                if ( parts . Length > 1 )
                    output = $" = {parts [ 1 ]}";
                if ( output . Contains ( "OUTPUT" ) )
                {
                    string [ ] tmp = output . Split ( "OUTPUT" );
                    if ( tmp [ 1 ] == "OUTPUT" )
                        output = tmp [ 0 ];
                    else
                        output = tmp [ 1 ];
                }
            }
            return output;
        }
        public string CheckForComment ( string input )
        {
            string output = input;
            if ( input . Contains ( "--" ) )
            {
                string [ ] tmp = input . Split ( "--" );
                output = tmp [ 0 ] . Length > tmp [ 1 ] . Length ? tmp [ 0 ] : tmp [ 1 ];
            }
            return output;
        }
        private void PasteButton_KeyDown ( object sender , KeyEventArgs e )
        {
            if ( e . Key == Key . Enter )
                Paste_Click ( sender , null );
            else if ( e . Key == Key . Escape )
                this . Close ( );
        }

        private void AutoClose_Checked ( object sender , RoutedEventArgs e )
        {
            if ( this . IsLoaded )
            {
                CheckBox cb = sender as CheckBox;
                if ( cb . IsChecked == true )
                    if ( Resviewer != null ) Resviewer . CloseArgsViewerOnPaste = true;
                    else
                    if ( Resviewer != null ) Resviewer . CloseArgsViewerOnPaste = false;

                if ( Resviewer != null )
                    MainWindow . SaveSystemSetting ( "AutoCloseSpArgumentsViewer" , Resviewer . CloseArgsViewerOnPaste );
            }
        }

        private void ShowTypes_Checked ( object sender , RoutedEventArgs e )
        {
            //if ( this . IsLoaded )
            //{
            //    CheckBox cb = sender as CheckBox;
            //    if ( cb . IsChecked == true )
            //    {
            //        if ( Resviewer != null ) Resviewer . ShowTypesInArgsViewer = true;
            //        KeepTypes = true;
            //    }
            //    else
            //    {
            //        if ( Resviewer != null ) Resviewer . ShowTypesInArgsViewer = false;
            //        KeepTypes = false;
            //    }
            //    MainWindow . SaveSystemSetting ( "ShowTypesInSpArgumentsString" , KeepTypes );
            //}
        }
        private void ShowTypes_Click ( object sender , RoutedEventArgs e )
        {
            CheckBox cb = sender as CheckBox;
            if ( cb . IsChecked == true )
            {
                if ( Resviewer != null ) Resviewer . ShowTypesInArgsViewer = true;
                KeepTypes = true;
            }
            else
            {
                if ( Resviewer != null ) Resviewer . ShowTypesInArgsViewer = false;
                KeepTypes = false;
            }
            MainWindow . SaveSystemSetting ( "ShowTypesInSpArgumentsString" , KeepTypes );
        }
        private void AutoClose_Loaded ( object sender , RoutedEventArgs e )
        {

        }

        private void ShowTypes_Loaded ( object sender , RoutedEventArgs e )
        {

        }

        private void Window_Closing ( object sender , CancelEventArgs e )
        {
            if ( Resviewer != null ) Resviewer . CloseArgsViewerOnPaste = ( bool ) AutoClose . IsChecked;
            if ( Resviewer != null ) Resviewer . ShowTypesInArgsViewer = ( bool ) ShowTypes . IsChecked;
            MainWindow . SaveSystemSetting ( "ShowTypesInSpArgumentsString" , ( bool ) ShowTypes . IsChecked );
            MainWindow . SaveSystemSetting ( "AutoCloseSpArgumentsViewer" , ( bool ) AutoClose . IsChecked );
            // ConfigurationManager . AppSettings . Add ( "ShowTypesInSpArgumentsString" );
        }

    }
}
