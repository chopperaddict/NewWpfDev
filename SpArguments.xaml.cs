using System;
using System . Collections . Generic;
using System . ComponentModel;
using System . Text;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Data;
using System . Windows . Documents;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Media . Imaging;
using System . Windows . Shapes;

namespace NewWpfDev
{
    /// <summary>
    /// Interaction logic for SpArguments.xaml
    /// </summary>
    public partial class SpArguments : Window
    {
        TextBox Parent = null;
        public SpArguments ( TextBox parent )
        {
            InitializeComponent ( );
            Parent = parent;
        }

        private void Button_Click ( object sender , RoutedEventArgs e )
        {
            this . Close ( );
        }

        private void Paste_Click ( object sender , RoutedEventArgs e )
        {
            string tmp = "";
            string buff = "";
            string [ ] parts = SPHeaderblock . Text . Split ( "\n" );
            for ( int x = 1 ; x < parts . Length ; x++ )
            {
                if ( parts [ x ] . ToUpper ( ) . Contains ( "@ARG" ) )
                {
                    // Handle any line that  contains @Args
                    int offset = parts [ x ] . ToUpper ( ) . IndexOf ( "@ARG" );
                    tmp = parts [ x ] . Substring ( offset );
                    string [ ] arg = tmp . Split ( "]" );
                    if ( arg . Length > 0 )
                    {
                        for ( int y = 0 ; y < arg . Length ; y++ )
                        {
                            if ( arg [ y ] . Contains ( "varchar" ) )
                            {
                                string [ ] element = arg [ y ] . Split ( ' ' );
                                for ( int z = 0 ; z < element . Length ; z++ )
                                {
                                    buff = $"{CheckForVarchar ( element [ z ] . Trim ( ) )}";
                                    //if ( element [ z ] . Contains ( "varchar" ) )
                                    //    buff = buff . Trim ( ) + $" string ";
                                    //else
                                    //    buff = buff . Trim ( ) + $" {element [ z ]} ";
                                }
                            }
                            else
                            {
                                if ( arg [ y ] . Contains ( "--" ) )
                                {
                                    // remove comments
                                    offset = arg [ y ] . IndexOf ( "--" );
                                    if ( x > 1 )
                                        tmp = $", {arg [ y ] . Substring ( 0 , offset )} ";
                                    else
                                        tmp = $"{arg [ y ] . Substring ( 0 , offset )} ";
                                    buff = buff.Trim() + $"{CheckForVarchar ( tmp . Trim ( ) )}";
                                }
                                else
                                {
                                    if ( arg [ y ] . Length > 0 )
                                        buff =buff.Trim() +  $"{CheckForVarchar ( arg [ y ] . Trim ( ) )}";
                                    //buff = buff . Trim ( ) + $", {arg [ y ]} ";
                                }
                            }
                        }
                    }
                    else
                    {
                        buff = buff . Trim ( ) + $", {tmp} ";
                    }
                }
                else
                {
                    // Handle any line that does NOT contain @Args
                    string outbuff = "";
                    
                    // check for lines that are only comments
                    if ( parts [ x ] . Trim ( ) . StartsWith ( "--" ) )
                        continue;

                    string [ ] arg = parts [ x ] . Split ( "[" );
                    if ( arg . Length > 1 )
                    {
                        tmp = arg [ 1 ] . Trim ( );
                        arg = tmp . Split ( " " );
                        int index = 0;
                        for ( int y = 0 ; y < arg . Length ; y++ )
                        {
                            outbuff += $"{CheckForVarchar ( arg [ y ] . Trim ( ) )} ";
                            //if ( arg [ y ] . Contains ( "varchar" ) )
                            //    outbuff += $"  string ";
                            //else
                            //    outbuff += $"{arg [ y ]} ";
                        }
                    }
                    else
                    {
                        if ( arg [ 0 ] . Length > 1 )
                        {
                            outbuff += $", {CheckForVarchar ( arg [ 0 ] . Trim ( ) )} ";
                            //outbuff += $", {arg [ 0 ]} ";
                        }
                    }
                    outbuff = outbuff . Trim ( );
                    //if ( outbuff.Trim() . Length > 1 )
                    //    buff += $", {outbuff . Trim ( ) . Substring ( 0 , outbuff . Length - 1 )} ";
                    buff += $", {outbuff . Trim ( )} ";
                }
            }
            Clipboard . SetText ( buff );
            Parent . Text = buff;
            NewWpfDev . Utils . DoSuccessBeep ( );
            this . Close ( );
        }

        /// <summary>
        /// Routine that checks for varchar or nvarchar or max in SP arguments
        /// and replaces them with the more legal 'string' nomenclature for the arguments string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string CheckForVarchar ( string input )
        {
            string output = "";
            string [ ] parts = input . Split ( ' ' );
            if ( parts . Length == 1 )
            {
                if ( input . ToUpper ( ) . Contains ( "NVARCHAR" ) || input . ToUpper ( ) . Contains ( "SYSNAME" ) || input . ToUpper ( ) . Contains ( "(MAX)" ) )
                    output += $" string ";
                else
                {
                    if ( input . ToUpper ( ) . Contains ( "VARCHAR" ) )
                        output += $" string ";
                    else
                        output += $"{input} ";
                }
            }
            else
            {
                for ( int x = 0 ; x < parts . Length ; x++ )
                {
//                        if ( parts [ x ] . ToUpper ( ) . Contains ( "NVARCHAR" ) )
                    if ( parts [x] . ToUpper ( ) . Contains ( "NVARCHAR" ) || parts [ x ] . ToUpper ( ) . Contains ( "SYSNAME" ) || parts [ x ] . ToUpper ( ) . Contains ( "(MAX)" ) )
                        output += $" string ";
                    else
                    {
                        if ( parts [ x ] . ToUpper ( ) . Contains ( "VARCHAR" ) )
                            output += $" string ";
                        else
                            output += $"{parts [ x ]} ";
                    }
                }
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
    }
}
