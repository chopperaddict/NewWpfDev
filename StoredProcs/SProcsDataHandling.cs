using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Diagnostics;
using System . Linq;
using System . Linq . Expressions;
using System . Printing;
using System . Security . RightsManagement;
using System . Text;
using System . Text . RegularExpressions;
using System . Windows;
using System . Windows . Automation . Provider;
using System . Windows . Markup;
using System . Windows . Media;

using IronPython . Runtime . Operations;

using Microsoft . Scripting . Actions;
using Microsoft . Win32;

using NewWpfDev;
using NewWpfDev . Converts;

using UserControls;

using Views;

namespace StoredProcs
{
    public static class SProcsDataHandling
    {
        public static SpResultsViewer spviewer { get; set; }
        public static string GetBareSProcHeader ( string Arguments , string procname , out bool success )
        {
            // save orignal string for testing use only
            string original = Arguments;
            Arguments = original;
            // get stripped down header block
            string arguments = Arguments;
            string sizeprompt = "";
            string Output = "";
            // massage input buffer
            string tmpbuff = "";
            string temp = Arguments;
            temp = temp . Trim ( ) . TrimStart ( );
            Arguments = temp . ToUpper ( );

            success = false;
            tmpbuff = Arguments . ToUpper ( );
            if ( tmpbuff . Length == 0 )
                return "";
            temp = tmpbuff;
            int stringlen = 0;
            string stringlenbuff = "";
            try
            {

                // First get any  items length argument
                string [ ] items = temp . Split ( '(' );
                if ( items != null )
                {
                    if ( items . Length > 1 )
                    {
                        bool found = true;
                        // Check for a valid size clause
                        if ( items [ 1 ] . Contains ( ')' ) )
                        {
                            string test = items [ 1 ] . Substring ( 0 , items [ 1 ] . IndexOf ( ')' ) );
                            if ( test == "MAX" )
                            {
                                sizeprompt = "(MAX argument size)";
                                temp = items [ 0 ];
                            }
                            else
                            {
                                string valid = "0123456789";

                                // check for double/float size
                                if ( test != "" && test . Contains ( ',' ) == true )
                                {
                                    // got ( xx.yy ) value
                                    sizeprompt = $"Double or Float value : {test}";
                                }
                                else
                                {
                                    //it seems it may be an int type value
                                    // See if it contains all digits
                                    for ( int y = 0 ; y < test . Length ; y++ )
                                    {
                                        char validchar = test [ y ];
                                        if ( valid . Contains ( validchar ) == false )
                                        {
                                            found = false;
                                            break;
                                        }
                                    }
                                    if ( found == true )
                                        stringlen = Convert . ToInt32 ( test );
                                    else
                                        sizeprompt = "(Undefined argument size)";
                                }
                            }
                        }
                        // Now parse the strings
                        if ( temp . Contains ( "VARCHAR" ) )
                        {
                            // strip VARCHAR off the string
                            if ( temp . Contains ( "NVARCHAR" ) )
                                tmpbuff = temp . Substring ( 0 , temp . IndexOf ( "NVARCHAR" ) );
                            else
                                tmpbuff = temp . Substring ( 0 , temp . IndexOf ( "VARCHAR" ) );
                            if ( stringlen > 0 )
                            {
                                sizeprompt = $"(string of maximum length {stringlen})";
                            }
                        }
                        else if ( temp . Contains ( "INT" ) || temp . Contains ( "INTEGER" ) )
                        {
                            if ( temp . Contains ( "INT" ) )
                                tmpbuff = temp . Substring ( 0 , temp . IndexOf ( "INT" ) );
                            else if ( temp . Contains ( "INTEGER" ) )
                                tmpbuff = temp . Substring ( 0 , temp . IndexOf ( "INTEGER" ) );
                            if ( stringlen > 0 )
                            {
                                sizeprompt = $"(Integer with max size of {stringlen} digits)";
                            }
                        }
                        else if ( temp . Contains ( "DATE" ) )
                        {
                            tmpbuff = temp . Substring ( 0 , temp . IndexOf ( "DATE" ) );
                            if ( stringlen > 0 )
                            {
                                sizeprompt = $"(Date field [defaullt is 'YYYY/MM/DD'] )";
                            }
                        }
                        else if ( temp . Contains ( "CHAR" ) )
                        {
                            tmpbuff = temp . Substring ( 0 , temp . IndexOf ( "CHAR" ) );
                            if ( stringlen > 0 )
                            {
                                sizeprompt = $"(Character buffer allowing {stringlen} characters)";
                            }
                        }
                        else if ( temp . Contains ( "TEXT" ) )
                        {
                            tmpbuff = temp . Substring ( 0 , temp . IndexOf ( "TEXT" ) );
                            if ( stringlen > 0 )
                            {
                                sizeprompt = $"(string buffer allowing {stringlen} characters)";
                            }
                        }
                        else if ( temp . Contains ( "FLOAT" ) )
                        {
                            tmpbuff = temp . Substring ( 0 , temp . IndexOf ( "FLOAT" ) );
                            if ( stringlen > 0 )
                            {
                                sizeprompt = $"(Float value allowing {stringlenbuff} digits)";
                            }
                        }
                        else if ( temp . Contains ( "DOUBLE" ) )
                        {
                            tmpbuff = temp . Substring ( 0 , temp . IndexOf ( "DOUBLE" ) );
                            if ( stringlen > 0 )
                            {
                                sizeprompt = $"(Double value allowing {stringlenbuff} digits)";
                            }
                        }
                        else if ( temp . Contains ( "DECIMAL" ) )
                        {
                            tmpbuff = temp . Substring ( 0 , temp . IndexOf ( "DECIMAL" ) );
                            if ( stringlen > 0 )
                            {
                                sizeprompt = $"(Decimal value allowing {stringlenbuff} digits)";
                            }
                        }
                        else if ( temp . Contains ( "BIT" ) )
                        {
                            tmpbuff = temp . Substring ( 0 , temp . IndexOf ( "BIT" ) );
                            if ( stringlen > 0 )
                            {
                                sizeprompt = $"(Logical value allowing only Zero or One as the value)";
                            }
                        }
                        if ( sizeprompt != "" )
                            tmpbuff += $" {sizeprompt}";
                        else
                        {
                            string val = stringlen > 0 ? stringlen . ToString ( ) : " ";
                            tmpbuff += $" {val}" . Trim ( );
                        }
                    }
                    if ( tmpbuff . Length > 2 )
                        Output += $"\n{tmpbuff}";
                }
            }
            catch ( Exception ex )
            {
                Debug . WriteLine ( $"{ex . Message}" );
            }
            return Output;
        }

        /// <summary>
        /// Method called when a ddifferent SProc is selected in the listbox
        /// and it parses the header block and provides indicators for the required 
        /// parameters in  the arguments entry field.....
        /// </summary>
        /// <param name="Arguments">The entire S.Proc text</param>
        /// <param name="procname">the name of the S.Procedure</param>
        /// <param name="success">return bool value</param>
        /// <returns></returns>
        public static string CreateSProcArgsList ( string Arguments , string procname , out bool success )
        {
            // save orignal string for testing use only
            string original = Arguments;
            Arguments = original;
            // get stripped down header block
            string arguments = Arguments;
            string sizeprompt = "";
            string Output = "";
            // massage input buffer
            string tmpbuff = "";
            string temp = Arguments;
            temp = temp . Trim ( ) . TrimStart ( );
            Arguments = temp . ToUpper ( );

            success = false;
            tmpbuff = Arguments . ToUpper ( );
            if ( tmpbuff . Length == 0 )
                return "";
            temp = tmpbuff . ToUpper ( );
            int stringlen = 0;
            if ( temp . Contains ( "IT CONTAINS INVALID SYNTAX" ) )
            {
                Output = "WARNING - Header/Script appears to be invalid or corrupted...";
                return Output;
            }
            try
            {
                bool invalid = false;
                // First get any  items length argument
                string [ ] items = temp . Split ( ')' );
                if ( items . Length > 0 )
                {
                    if ( items . Length == 1 )
                    {
                        if ( items [ 0 ] . Contains ( "CREATE PROCEDURE" ) == false )
                        {
                            Output = "WARNING - Header block appears to be invalid or corrupted...";
                            invalid = true;
                        }
                    }
                    if ( invalid == false )
                    {
                        if ( items . Length > 1 )
                        {
                            bool found = true;
                            // Check for a valid size clause
                            if ( items [ 1 ] . Contains ( ')' ) )
                            {
                                string test = items [ 1 ] . Substring ( 0 , items [ 1 ] . IndexOf ( ')' ) );
                                if ( test == "MAX" )
                                {
                                    sizeprompt = "(MAX argument size)";
                                    temp = items [ 0 ];
                                }
                                else
                                {
                                    string valid = "0123456789";

                                    // check for double/float size
                                    if ( test != "" && test . Contains ( ',' ) == true )
                                    {
                                        // got ( xx.yy ) value
                                        sizeprompt = $"Double or Float value : {test}";
                                    }
                                    else
                                    {
                                        //it seems it may be an int type value
                                        // See if it contains all digits
                                        for ( int y = 0 ; y < test . Length ; y++ )
                                        {
                                            char validchar = test [ y ];
                                            if ( valid . Contains ( validchar ) == false )
                                            {
                                                found = false;
                                                break;
                                            }
                                        }
                                        if ( found == true )
                                            stringlen = Convert . ToInt32 ( test );
                                        else
                                            sizeprompt = "(Undefined argument size)";
                                    }
                                }
                            }
                        }
                        // We now have a full header block, so parse the strings
                        string [ ] headerbuff = temp . Split ( '\n' );


                        // default to being in INPUT mode
                        bool outflag = false;
                        Output = "[** Target **] ";


                        // loop thru each the parameters
                        for ( int x = 1 ; x < headerbuff . Length ; x++ )
                        {
                            if ( x > 1 )
                                Output += " : ";

                            string [ ] parts = headerbuff [ x ] . ToUpper ( ) . Split ( ' ' );
                            for ( int y = 0 ; y < parts . Length ; y++ )
                            {
                                if ( parts [ y ] == ":" || parts [ y ] == "" )
                                    continue;
                                if ( parts [ y ] . Contains ( "OUTPUT" ) )
                                {
                                    outflag = true;
                                }
                                if ( !outflag && parts [ y ] . Contains ( "INPUT" ) )
                                    continue;
                                else if ( !outflag && parts [ y ] . Contains ( "@" ) )
                                {
                                    if ( Output != "" )
                                    {
                                        if ( parts [ y ] [ 0 ] == ',' )
                                            Output += $"{parts [ y ] . Substring ( 1 , parts [ y ] . Length - 1 )} ";
                                        else
                                            Output += $"{parts [ y ]} ";
                                    }
                                    else Output += $"{parts [ y ]}";
                                    continue;
                                }
                                else if ( !outflag )
                                {
                                    if ( parts [ y ] . Contains ( "VARCHAR" ) )
                                    {
                                        string [ ] elements = parts [ y ] . Split ( '(' );
                                        if ( elements [ 1 ] . Contains ( ')' ) )
                                        {
                                            string [ ] splitter = elements [ 1 ] . Split ( ')' );
                                            if ( splitter . Length > 0 )
                                                Output += $"STRING ({splitter [ 0 ]})  INPUT";
                                            else
                                                Output += $"STRING ({splitter [ 0 ]})  INPUT";
                                        }
                                        else
                                            Output += $" String ({elements [ 1 ]}) INPUT";
                                    }
                                    else
                                    {
                                        if ( parts [ y ] . Contains ( "SYSNAME" ) || parts [ y ] . Contains ( "MAX" ) )
                                        {
                                            Output += $" String MAX INPUT ";
                                            continue;
                                        }
                                        if ( parts [ y ] == "AS" )
                                            continue;
                                    }
                                    continue;
                                }
                                if ( outflag && parts [ y ] . Contains ( "@" ) )
                                {
                                    if ( parts [ y ] . StartsWith ( "," ) || parts [ y ] . Length < 2 )
                                    {
                                        Output += $"  :  {parts [ y ] . Substring ( 1 )} ";
                                        continue;
                                    }
                                    if ( Output . Length > 0 ) Output += $", {parts [ y ]}";
                                    else Output += $"{parts [ y ]}";
                                }
                                else if ( outflag )
                                {
                                    if ( parts [ y ] . Contains ( "VARCHAR" ) )
                                    {
                                        string [ ] elements = parts [ y ] . Split ( '(' );
                                        Output += $"String ";
                                        // Add in the size !!
                                        if ( elements . Length >= 2 && elements [ 1 ] != "" )
                                            Output += $"({elements [ 1 ] . Substring ( 0 , elements . Length )}) OUTPUT";
                                        else
                                            Output += $")";
                                        break;
                                    }
                                    else if ( parts [ y ] . Contains ( "INT" ) )
                                    {
                                        string [ ] elements = parts [ y ] . Split ( '(' );
                                        Output += $" (Integer OUTPUT";
                                        if ( elements . Length >= 2 && elements [ 1 ] != "" )
                                            Output += $"( {elements [ 1 ]} )";
                                        else
                                            Output += $")";
                                        break;
                                    }
                                    else if ( parts [ y ] . Contains ( "DATE" ) )
                                    {
                                        string [ ] elements = parts [ y ] . Split ( '(' );
                                        Output += $" (Date OUTPUT)";
                                        if ( elements . Length >= 2 && elements [ 1 ] != "" )
                                            Output += $"( {elements [ 1 ]} )";
                                        else
                                            Output += $")";
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else if ( Output == "" )
                        Output = "No arguments required.....";
                }
                else
                {
                    Output = "WARNING - Header/Script appears to be invalid or corrupted...";
                }
            }
            catch ( Exception ex )
            {
                Debug . WriteLine ( $"{ex . Message}" );
            }
            return Output;
        }
        public static string [ ] ReplaceNullsWithBlankString ( string [ ] cleantest )
        {
            for ( int x = 0 ; x < cleantest . Length ; x++ )
            {
                if ( cleantest [ x ] == null )
                    cleantest [ x ] = "";
            }
            return cleantest;
        }

        public static string GetSpHeaderBlock ( string arguments , SpResultsViewer spviewer )
        {
            //Save buffer  for reuse if debugging
            string Arguments = arguments;
            int argcount = 0;
            string defvalue = "";
            int offset = 0;
            string temp = Arguments;
            string tmpbuff = "";
            temp = temp . Trim ( ) . TrimStart ( );
            Arguments = temp . ToUpper ( );
            int CreatePosition = 0;
            string output = "";

            // retrieve header block
            if ( Arguments . Contains ( "CREATE PROCEDURE" ) || Arguments . Contains ( "CREATE  PROCEDURE" ) == true )
            {
                try
                {
                    CreatePosition = Arguments . IndexOf ( "CREATE PROCEDURE" );
                    if ( CreatePosition > 0 )
                    {
                        // Strip out any preamble before the Create Proc line
                        Arguments = Arguments . Substring ( CreatePosition );
                        int offset3 = Arguments . IndexOf ( "\r\n" );
                        Arguments = Arguments . Substring ( offset3 );
                    }
                    // remove Create Proc line


                    // split entire data area by \r\n
                    string [ ] test = Arguments . Split ( "\r\n" );
                    //                   test = Arguments . Split ( "\r\n" );

                    // now get the cleaned up header block alone
                    test = ExtractSpHeaderBlock ( test );
                    if ( test [ 0 ] . StartsWith ( "ERROR -" ) )
                        return test [ 0 ];
                    if ( test . Length == 1 && test [ 0 ] == "" )
                    {
                        spviewer . Parameterstop . Text = $"[No Parameters/Arguments required]";
                        output = "No arguments required, press 'Clear Prompt' button and then select Execute Option.";
                        return output;
                    }

                    if ( test [ 0 ] . StartsWith ( "CREATE PROCEDURE" ) && test . Length == 1 )
                    {
                        spviewer . Parameterstop . Text = $"[No Parameters/Arguments required]";
                        output = "No arguments are required, press 'Clear Prompt' button and then select Execute Option.";
                        return output;
                    }
                    else if ( test [ 0 ] . StartsWith ( "CREATE PROCEDURE" ) && test . Length > 1 )
                    {
                        test [ 0 ] = "";
                    }

                    string currentrow = "";
                    // Sanity check , only CREATE line in header with no args
                    for ( int rows = 0 ; rows < test . Length ; rows++ )
                    {
                        if ( test [ rows ] . Length <= 1 )
                            continue;

                        // remove/ignore any full comment lines
                        if ( test [ rows ] . StartsWith ( "/*") || test [rows].StartsWith ( "*" ) )
                            continue;
                       
                        currentrow = test [ rows ];

                        // check for commented lines
                        if ( currentrow . StartsWith ( "--" ) )
                            continue;
                        string testbuff = CheckAndRemoveBadCharacters ( currentrow ) . Trim ( );
                          // split string into individual items so we can validate them
                        if ( testbuff . Contains ( "MAX" ) || testbuff . Contains ( "SYSNAME" ) )
                        {
                            string [ ] tmp = testbuff . Split ( " " );
                            for ( int x = 0 ; x < tmp . Length ; x++ )
                            {
                                if ( tmp [ x ] == "MAX" || tmp [ x ] == "SYSNAME" )
                                    tmp [ x ] = " 32000";
                            }
                            if ( tmp . Length > 3 )
                                testbuff = $"{tmp [ 0 ]} {tmp [ 1 ]}{tmp [ 2 ]} {tmp [ 3 ]}";
                            else
                                testbuff = $"{tmp [ 0 ]} {tmp [ 1 ]}{tmp [ 2 ]}";
                        }
                        if ( output . Length > 0 )
                            output += " : ";
                        output += testbuff;
                    }
                    //}
                }
                catch ( Exception ex )
                {
                    Console . WriteLine ( $"Parsing error {ex . Message}" );
                    return "";
                }
            }

            if ( output == "" )
            {
                spviewer . Parameterstop . Text = $"[No Parameters/Arguments required]";
                output = "No arguments are required, press 'Clear Prompt' button and then select Execute Option.";
            }
            else
            {
                int [ ] count = new int [ 3 ];
                string [ ] str = output . Split ( " " );
                foreach ( var item in str )
                {
                    if ( item == "" )
                        continue;
                    if ( item . Contains ( "@" ) )
                        count [ 0 ]++;
                    else if ( item . Contains ( "OUTPUT" ) || item . EndsWith ( "OUT" ) )
                    {
                        count [ 1 ]++;
                        count [ 2 ] = count [ 1 ];
                    }
                }

                if ( count [ 0 ] == 0 && count [ 1 ] == 1 )
                    spviewer . Parameterstop . Text = $"[No Parameters but Single Output parameter]";
                else if ( count [ 0 ] == 1 && count [ 1 ] == 0 )
                    spviewer . Parameterstop . Text = $"[Single Target or Input parameter only]";
                else if ( count [ 0 ] == 1 && count [ 1 ] == 1 )
                {
                    if ( count [ 2 ] > 0 )
                        spviewer . Parameterstop . Text = $"[Single Output parameter only]";
                    else
                        spviewer . Parameterstop . Text = $"[Single Target and/or Multiple Input parameters + Single Output parameter]";
                }
                else if ( count [ 0 ] > 1 && count [ 1 ] == 0 && str . Length == 1 )
                    spviewer . Parameterstop . Text = @$"[Single Target plus one input or Multiple Inputs]";
                else if ( count [ 0 ] > 1 && count [ 1 ] == 0 && str . Length > 1 )
                    spviewer . Parameterstop . Text = @$"[Single Target and/or Multiple Inputs]";
                else if ( count [ 0 ] == 1 && count [ 1 ] == 0 && str . Length > 1 )
                    spviewer . Parameterstop . Text = @$"[Single Target or Input parameter]";
                else if ( count [ 0 ] == 0 && count [ 1 ] == 1 )
                    spviewer . Parameterstop . Text = @$"[Single Output parameter only]";
                else if ( count [ 0 ] > 1 && count [ 1 ] == 1 )
                {
                    if ( output . Contains ( ":" ) == false )
                        spviewer . Parameterstop . Text = $"[Single Output parameter only]";
                    else if ( count [ 0 ] - count [ 1 ] == 1 )
                        spviewer . Parameterstop . Text = $"[Single Target or Single Input parameter + Single Output parameter]";
                    else
                        spviewer . Parameterstop . Text = $"[Single Target and/or Multiple Input parameters + Single Output parameter]";
                }
                else if ( count [ 0 ] > 1 && count [ 1 ] >= 1 )
                {
                    if ( count [ 2 ] == count [ 0 ] - 1 )
                        spviewer . Parameterstop . Text = @$"[Single Target or Input + Multiple Output parameters]";
                    else
                        spviewer . Parameterstop . Text = @$"[Single Target and/or Multiple Inputs + Multiple Output parameters]";
                }
                else if ( count [ 0 ] == 0 && count [ 1 ] == 0 )
                {
                    spviewer . SPArguments . Text = @$"[No parameters/Arguments are required]";
                    spviewer . Parameterstop . Text = @$"[No parameters required (or allowed)]";
                    output = "No parameters are required";
                }
                // Single input, single output
                else
                    spviewer . Parameterstop . Text = $"[Input parameter(s) Only ]";
            }
            return output;
        }

        static public string [ ] ExtractSpHeaderBlock ( string [ ] cleantest )
        {
            string Arguments = "";
            int [ ] aspos = new int [ 3 ];
            aspos [ 0 ] = aspos [ 1 ] = -1;
            // clean up any spurious leading characters
            for ( int z = 0 ; z < cleantest . Length ; z++ )
            {
                cleantest [ z ] = cleantest [ z ] . Trim ( );
                if ( cleantest [ z ] == null || cleantest [ z ] == "" )
                    continue;
                if ( cleantest [ z ] == "AS" )
                {
                    aspos [ 0 ] = z;
                    continue;
                }
                if ( cleantest [ z ] == "BEGIN" )
                {
                    aspos [ 1 ] = z;
                    if ( aspos [ 1 ] > aspos [ 0 ] )
                        break;
                }
            }
            if ( aspos [ 0 ] == -1 || aspos [ 1 ] == -1 )
            {
                string [ ] head = new string [ 1 ];
                head [ 0 ] = $"ERROR - Either the \"AS\" or \"BEGIN \" statements are missing";
                return head;
            }

            string [ ] header = new string [ aspos [ 0 ] ];
            if ( aspos [ 1 ] - aspos [ 0 ] >= 1 )
            {
                // got the end of the header block - strip it out into string[]
                for ( int x = 0 ; x < aspos [ 0 ] ; x++ )
                {
                    if ( cleantest [ x ] != "" )
                        header [ x ] = cleantest [ x ];
                    else
                        header [ x ] = "";
                }
            }
            if ( header . Length == 1 && header [ 0 ] == "" )
                return header;

            //int blanklines = 0;
            header = ReplaceNullsWithBlankString ( header );
            // got it - now cleanup the header block
            //by checking  for leading commas, tabs, comments (--)
            // and trailing tabs or comments (--) or \r or \n
            for ( int z = 0 ; z < header . Length ; z++ )
            {
                int blanklines = 0;
                if ( header [ z ] == "" )
                {
                    blanklines++;
                    continue;
                }
                if ( header [ z ] != null && header [ z ] . StartsWith ( '\t' ) )
                {
                    // check for multiple \t
                    header [ z ] = header [ z ] . Substring ( 1 );

                    while ( header [ z ] . Contains ( "\t" ) )
                    {
                        int offset2 = header [ z ] . IndexOf ( '\t' );
                        header [ z ] = header [ z ] . Substring ( 0 , offset2 );
                    }
                    if ( header [ z ] == "" )
                        blanklines++;
                }
                // check for leading ,
                if ( header [ z ] != null && header [ z ] . StartsWith ( ',' ) )
                {
                    header [ z ] = header [ z ] . Substring ( 1 );
                    // check again for \t in case they were in revese order
                    while ( header [ z ] . Contains ( "," ) )
                        header [ z ] = header [ z ] . Substring ( 1 );
                }
                // check for leading --
                if ( header [ z ] != null && header [ z ] . StartsWith ( "-" ) )
                {
                    header [ z ] = header [ z ] . Substring ( 1 );
                    if ( header [ z ] . StartsWith ( "-" ) )
                    {
                        header [ z ] = "";
                        blanklines++;
                    }
                }

                //check for Trailing \t
                if ( header [ z ] . Contains ( "\t" ) )
                {
                    while ( header [ z ] . Contains ( "\t" ) )
                    {
                        int offset2 = header [ z ] . IndexOf ( '\t' );
                        header [ z ] = header [ z ] . Substring ( 0 , offset2 );
                    }
                    if ( header [ z ] == "" )
                        blanklines++;
                }
                // check for Trailing --
                if ( header [ z ] . Contains ( "--" ) )
                {
                    while ( header [ z ] . Contains ( "--" ) )
                    {
                        int offset2 = header [ z ] . IndexOf ( '-' );
                        header [ z ] = header [ z ] . Substring ( 0 , offset2 );
                    }
                    if ( header [ z ] == "" )
                        blanklines++;
                }
                //check for trailing CR
                if ( header [ z ] . Contains ( "\r" ) )
                {
                    while ( header [ z ] . Contains ( "\r" ) )
                    {
                        int offset2 = header [ z ] . IndexOf ( '\r' );
                        header [ z ] = header [ z ] . Substring ( 0 , 1 );
                    }
                    if ( header [ z ] == "" )
                        blanklines++;
                }
                //check for trailing LF
                if ( header [ z ] . Contains ( "\n" ) )
                {
                    while ( header [ z ] . Contains ( "\n" ) )
                    {
                        int offset2 = header [ z ] . IndexOf ( '\n' );
                        header [ z ] = header [ z ] . Substring ( 0 , 1 );
                    }
                    if ( header [ z ] == "" )
                        blanklines++;
                }
                if ( blanklines == header . Length )
                {
                    string [ ] output = new string [ 1 ];
                    output [ 0 ] = "ERROR - No valid Arguments were found in the current Stored P.rocedure......";
                    return output;
                }
                header [ z ] = header [ z ] . Trim ( );
            }
            Arguments = "";
            int newcount = 0;
            for ( int x = 0 ; x < header . Length ; x++ )
            {
                if ( header [ x ] != "" )
                {
                    Arguments += $"{header [ x ]}:";
                    newcount++;
                }
            }
            Arguments = Arguments . Substring ( 0 , Arguments . Length - 1 );

            string [ ] head2 = Arguments . Split ( ":" );
            // we should now have a totally clean string of the entire header block with lines seperated by :
            return head2;
        }


        public static string [ ] ClearStringArray ( string [ ] arry )
        {
            for ( int x = 0 ; x < arry . Length ; x++ )
            {
                arry [ x ] = "";
            }
            return arry;
        }
        public static string CheckAndRemoveBadCharacters ( string testbuff )
        {
            if ( testbuff == null )
                return "";
            // Test for : '\t'
            if ( testbuff . EndsWith ( "," ) )
            {
                testbuff = testbuff . Substring ( 0 , testbuff . Length - 1 );
            }
            // Test for : '\n'
            if ( testbuff . Contains ( "\n" ) )
            {
                string newbuff = "";
                string [ ] tmp = testbuff . Split ( '\n' );
                foreach ( var item in tmp )
                {
                    newbuff += $"{item} ";
                }
                testbuff = newbuff;
            }
            if ( testbuff . Contains ( "\n" ) )
            {
                string newbuff = "";
                string [ ] tmp = testbuff . Split ( '\n' );
                foreach ( var item in tmp )
                {
                    newbuff += $"{item} ";
                }
                testbuff = newbuff;
            }
            // Test for : '\t'
            if ( testbuff . Contains ( "\t" ) )
            {
                string newbuff = "";
                string [ ] tmp = testbuff . Split ( '\t' );
                foreach ( var item in tmp )
                {
                    newbuff += $"{item} ";
                }
                testbuff = newbuff;
            }
            // Test for :  ','
            if ( testbuff . Contains ( "," ) )
            {
                string newbuff = "";
                string [ ] tmp = testbuff . Split ( ',' );
                foreach ( var item in tmp )
                {
                    newbuff += $"{item} ";
                }
                testbuff = newbuff;
            }
            // Test for : '-'
            if ( testbuff . Contains ( "-" ) )
            {
                string newbuff = "";
                string [ ] tmp = testbuff . Split ( "-" );
                newbuff = tmp [ 0 ];
                testbuff = newbuff;
            }
            if ( testbuff . Contains ( "''") || testbuff . Contains ( "' '" ))
            {
                int offset = 0;
                string newbuff = "";
                if ( testbuff . Contains ( "''" ))
                    offset = testbuff . IndexOf ( "''" );
                if(offset == 0)
                    offset = testbuff . IndexOf ( "' '" );
                if ( offset > 0 )
                {
                    testbuff = testbuff . Substring ( 0 , offset );
                    if ( testbuff [testbuff.Length-1] == '=' )
                        testbuff = testbuff . Substring ( 0 , testbuff.Length - 1);
                }//int[] qpos = new int[2];
                 //int q = 0;
                 //for ( int x = 0 ; x < tmp . Length ; x++ )
                 //{
                 //    if ( tmp [ x ] =="" || tmp [x]== " " )
                 //        qpos [ q++] = x;
                 //    if ( tmp [ x ] != "" && tmp [ x ] != " " )
                 //        newbuff += tmp [ x ];
                 //}
                 //if ( qpos [ 0 ] != 0 && qpos [ 1 ] != 0 && qpos [ 1 ]- qpos [ 0 ] >= 1 )
                 //    testbuff = testbuff . Substring ( 0 , qpos [ 0 ] );
                 // testbuff = newbuff;
            }

            //***********************//
            // now parse what is left
            //***********************//
            string [ ] argument = testbuff . Split ( " " );
            for ( int y = 0 ; y < argument . Length ; y++ )
            {
                // Test for : "(xxx)"
                if ( argument [ y ] . Contains ( '(' ) && argument [ y ] . Contains ( ')' ) )
                {

                    //                   string [ ] args = new string [ 5 ];
                    string [ ] tmp2 = new string [ 5 ];
                    tmp2 [ y ] = argument [ y ] . Trim ( );
                    int offset = tmp2 [ y ] . IndexOf ( '(' );
                    // strip out "(xxx)"
                    string [ ] tmp = tmp2 [ y ] . Split ( "(" );
                    tmp2 [ y ] = tmp [ 1 ];
                    offset = tmp2 [ y ] . IndexOf ( ')' );
                    tmp2 [ y ] = tmp2 [ y ] . Substring ( 0 , offset );
                    if ( tmp2 [ y ] == "MAX" || tmp2 [ y ] == "SYSNAME" )
                        tmp2 [ y ] = " STRING 32000";
                    else
                        tmp2 [ y ] = $"STRING {tmp2 [ y ]}";
                    argument [ y ] = tmp2 [ y ];
                }
                if ( argument [ y ] . Contains ( "VARCHAR" ) )
                {
                    argument [ y ] = "STRING";
                }
                // Test for : "MAX" for size
                if ( argument [ y ] == "MAX" || argument [ y ] == "SYSNAME" )
                    argument [ y ] = " STRING 32000";

                // Test for : Sysname as size
                if ( argument [ y ] . Contains ( "SYSNAME" ) )
                {
                    string [ ] args = new string [ 5 ];

                    args = argument [ y ] . Split ( ' ' );
                    if ( args [ 1 ] . Contains ( "SYSNAME" ) )
                        argument [ y ] = " STRING 32000";
                }
            }
            testbuff = "";
            for ( int z = 0 ; z < argument . Length ; z++ )
            {
                testbuff += $"{argument [ z ]} ";
            }
            return testbuff . Trim ( );
        }
        /// <summary>
        /// Method to strip out the header block of any SP and
        /// present it in a presentation format for help system etc
        /// </summary>
        /// <param name="Arguments"></param>

        public static string GetRegexForHeaderEnd ( string argument )
        {
            // use REGEX to find AS and BEGIN so we can get JUST the header block
            int asindex = 0;
            int beginindex = 0;
            string Output = "";
            string tmpbuff = "";
            bool found = false;
            int offset = 0;

            string buff = argument . ToUpper ( );

            try
            {
                int [ ] offsetAs = FindWithRegex ( buff , "AS" );
                // working Regex statement - "\sbegin\s" will match 'begin' starting and ending in space | Cr |Lf characters
                int [ ] offsetBegin = FindWithRegex ( buff , $@"\sBEGIN\s" );
                if ( offsetAs . Length > 0 && offsetBegin . Length > 0 )
                    Output = FindHeaderBlock ( argument , offsetAs , offsetBegin );
            }
            catch ( Exception ex )
            {
                Debug . WriteLine ( $"Regex failure : {ex . Message}" );
            }
            return Output;
        }
        public static int [ ] FindWithRegex ( string buff , string argument )
        {
            string tmpbuff = "";
            bool found = false;
            int offset = 0;
            argument = argument . ToUpper ( );
            buff = buff . ToUpper ( );
            // working Regex statement - "\sbegin\s" will match 'begin' starting and ending in space | Cr |Lf characters
            // or ".begin." will match 'begin' starting and ending in ANY character at  all
            MatchCollection mc = Regex . Matches ( buff , @$"\s{argument}\s" , RegexOptions . IgnoreCase );
            int [ ] offsets = new int [ mc . Count ];
            for ( int x = 0 ; x < mc . Count ; x++ )
            {
                offsets [ x ] = mc [ x ] . Index;
            }
            return offsets;
        }
        public static string FindHeaderBlock ( string buff , int [ ] item1 , int [ ] item2 )
        {
            int beginindex = 0;
            int asindex = 0;
            int diff = 0;
            string tmpbuff = "";
            bool found = false;
            // also works cos begin starts a line but has a space after it in the file
            if ( item1 . Length > 0 && item2 . Length > 0 )
            {
                // iterate thru collection of matches to AS till we find the one closest to our BEGIN statement
                for ( int x = 0 ; x < item1 . Length ; x++ )
                {
                    for ( int y = 0 ; y < item2 . Length ; y++ )
                    {
                        asindex = item1 [ x ];
                        beginindex = item2 [ y ];
                        diff = beginindex - asindex;

                        if ( diff <= 10 )
                        {
                            // GOT IT - AS is within 10 bytes of BEGIN, so we are almost certainly good to go 
                            tmpbuff = buff . Substring ( 0 , asindex );
                            found = true;
                            break;
                        }
                        else if ( diff > 0 )
                        {
                            // begin is too far ahead, so get next As
                            break;
                        }
                    }
                    if ( found )
                        return tmpbuff;
                }
            }
            return "";
        }
        public static string CheckforCommas ( string input )
        {
            string output = "";
            string tmpstr = "";
            string inputbuff = input;
            string [ ] parts;
            int offset = 0;
            if ( input . Contains ( ',' ) )
            {
                while ( true )
                {
                    offset = inputbuff . IndexOf ( ',' );
                    if ( offset == 0 )
                    {
                        if ( output == "" )
                            return input;
                        else
                            return output;
                    }
                    parts = input . Split ( ',' );
                    if ( parts [ 0 ] . Length > parts [ 1 ] . Length )
                        inputbuff = parts [ 0 ];
                    else
                        inputbuff = parts [ 1 ];
                    if ( inputbuff . Contains ( ',' ) == false )
                    {
                        output = inputbuff;
                        break;
                    }
                }
            }
            else
                output = input;
            return output;
        }
        public static string StripTabs ( string input )
        {
            string output = "";
            int offset = -1;
            string [ ] test = new string [ 0 ];
            if ( input . Contains ( '\t' ) )
            {
                offset = input . IndexOf ( '\t' );
                test = input . Split ( '\t' );
                for ( int x = 0 ; x < test . Length ; x++ )
                {
                    if ( test [ x ] . StartsWith ( '\t' ) )
                        test [ x ] = $" {test [ x ] + 1}";
                    output += test [ x ];
                }
            }
            else
                output = input;
            return output;
        }
        public static string CheckforComments ( string input )
        {
            string output = input;
            string [ ] tmp
        ; if ( input . Contains ( "--" ) )
            {
                tmp = input . Split ( "--" );
                if ( tmp . Length == 2 )
                    output = tmp [ 0 ] . Length > tmp [ 1 ] . Length ? tmp [ 0 ] : tmp [ 1 ];
            }
            return output;
        }
        public static List<string> CallStoredProcedure ( List<string> list , string sqlcommand , string [ ] args = null )
        {
            //            List<string> list = new List<string> ( );
            list = NewWpfDev . GenDapperQueries . ProcessUniversalQueryStoredProcedure ( sqlcommand , args , MainWindow . CurrentSqlTableDomain , out string err );
            //This call returns us a List<string>
            // This method is NOT a dynamic method
            return list;
        }
        static public List<string> CreateListFromGenericClass ( ObservableCollection<GenericClass> gengrid )
        {
            // ceate List<string> from Generic Collection
            // Each field is padded  to max of 25 chars in length
            List<string> newlist = new List<string> ( );
            string output = "";
            int paddingsize = 30;
            bool Finished = false;
            foreach ( GenericClass gen in gengrid )
            {
                while ( Finished == false )
                {
                    if ( gen . field1 != null )
                        output = $"{gen . field1 . ToString ( ) . PadRight ( 5 )}";
                    if ( gen . field2 != null && gen . field2 . Length < paddingsize )
                        output += $"{gen . field2 . ToString ( ) . PadRight ( paddingsize - gen . field2 . ToString ( ) . Length )}";
                    else break;
                    if ( gen . field3 != null )
                        output += $"{gen . field3 . ToString ( ) . PadRight ( paddingsize - gen . field3 . ToString ( ) . Length )}";
                    else break;
                    if ( gen . field4 != null )
                        output += $"{gen . field4 . ToString ( ) . PadRight ( paddingsize - gen . field4 . ToString ( ) . Length )}";
                    else break;
                    if ( gen . field5 != null )
                        output += $"{gen . field5 . ToString ( ) . PadRight ( paddingsize - gen . field5 . ToString ( ) . Length )}";
                    else break;
                    if ( gen . field6 != null )
                        output += $"{gen . field6 . ToString ( ) . PadRight ( paddingsize - gen . field6 . ToString ( ) . Length )}";
                    else break;
                    if ( gen . field7 != null )
                        output += $"{gen . field7 . ToString ( ) . PadRight ( paddingsize - gen . field7 . ToString ( ) . Length )}";
                    else break;
                    if ( gen . field8 != null )
                        output += $"{gen . field8 . ToString ( ) . PadRight ( paddingsize - gen . field8 . ToString ( ) . Length )}";
                    else break;
                    if ( gen . field9 != null )
                        output += $"{gen . field9 . ToString ( ) . PadRight ( paddingsize - gen . field9 . ToString ( ) . Length )}";
                    else break;
                    if ( gen . field10 != null )
                        output += $"{gen . field10 . ToString ( ) . PadRight ( paddingsize - gen . field10 . ToString ( ) . Length )}";
                    else break;
                    if ( gen . field11 != null )
                        output += $"{gen . field11 . ToString ( ) . PadRight ( paddingsize - gen . field11 . ToString ( ) . Length )}";
                    else break;
                    if ( gen . field12 != null )
                        output += $"{gen . field12 . ToString ( ) . PadRight ( paddingsize - gen . field12 . ToString ( ) . Length )}";
                    else break;
                    if ( gen . field13 != null )
                        output += $"{gen . field13 . ToString ( ) . PadRight ( paddingsize - gen . field13 . ToString ( ) . Length )}";
                    else break;
                    if ( gen . field14 != null )
                        output += $"{gen . field14 . ToString ( ) . PadRight ( paddingsize - gen . field14 . ToString ( ) . Length )}";
                    else break;
                    if ( gen . field15 != null )
                        output += $"{gen . field15 . ToString ( ) . PadRight ( paddingsize - gen . field15 . ToString ( ) . Length )}";
                    else break;
                    if ( gen . field16 != null )
                        output += $"{gen . field16 . ToString ( ) . PadRight ( paddingsize - gen . field16 . ToString ( ) . Length )}";
                    else break;
                    if ( gen . field17 != null )
                        output += $"{gen . field17 . ToString ( ) . PadRight ( paddingsize - gen . field17 . ToString ( ) . Length )}";
                    else break;
                    if ( gen . field18 != null )
                        output += $"{gen . field18 . ToString ( ) . PadRight ( paddingsize - gen . field18 . ToString ( ) . Length )}";
                    else break;
                    if ( gen . field19 != null )
                        output += $"{gen . field19 . ToString ( ) . PadRight ( paddingsize - gen . field19 . ToString ( ) . Length )}";
                    else break;
                }
                newlist . Add ( output );
            }
            return newlist;
        }

    }
}

