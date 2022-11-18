using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . Diagnostics;
using System . Printing;
using System . Text;
using System . Text . RegularExpressions;
using System . Windows;
using System . Windows . Media;

using Microsoft . Scripting . Actions;

using NewWpfDev;

using UserControls;

using Views;

namespace StoredProcs
{
    public static class SProcsDataHandling
    {
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
            //int CreatePosition = Arguments . IndexOf ( "CREATE PROCEDURE" );
            //if ( CreatePosition == -1 )
            //{
            //    return "No  Arguments were found in current S.P.......";
            //}
            //int line1 = arguments . IndexOf ( "\n" );
            //Arguments = Arguments . Substring ( CreatePosition + line1 );
            ////          string [ ] strings = Arguments . Split ( "\n" );
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

        public static string GetSpHeaderBlock ( string Arguments )
        {
            string arguments = Arguments;
            int argcount = 0;
            bool found = false;
            int offset = 0;
            string temp = Arguments;
            string tmpbuff = "";
            temp = temp . Trim ( ) . TrimStart ( );
            Arguments = temp . ToUpper ( );

            int CreatePosition = Arguments . IndexOf ( "CREATE PROCEDURE" );
            if ( CreatePosition == -1 )
            {
                return "No  Arguments were found in current S.P.......";
            }

            Arguments = Arguments . Substring ( CreatePosition );

            //call REGEX to find end of header by identifying AS and Begin consecutively
            // and returns the header block in the  buffer
            tmpbuff = GetRegexForHeaderEnd ( Arguments );
            string [ ] parts = tmpbuff . Split ( "\n" );
            string output = "";
            // add top create statement to output buffer
            output = $"{parts [ 0 ]}\n";
            // Sanity check , only CREATE line in header with no args
            if ( parts . Length == 0 )
            {
                MessageBox . Show ( "Unable to find either AS or BEGIN in current script. \nCheck the script in SQL Server Management Studio" , "SQL script Error" );
                return null;
            }
            foreach ( var item in parts )
            {
                string testbuff = "";
                // Bypass create line
                if ( argcount == 0 )
                {
                    argcount++;
                    continue;
                }
                if ( item . Length == 0 )
                    continue;
                testbuff = item;

                // Check for Output arguments
                if ( testbuff . ToUpper ( ) . StartsWith ( '\t' ) )
                    testbuff = testbuff . Substring ( 1 ) . Trim ( );
                if ( testbuff . ToUpper ( ) . StartsWith ( "--" ) )
                    continue;   // its just a comment line
                if ( testbuff . ToUpper ( ) . Contains ( "OUTPUT" ) )
                {
                    string tmpbuff2 = "";
                    if ( testbuff . Contains ( "--" ) )
                    {
                        string [ ] buff2 = testbuff . Split ( "--" );
                        testbuff = buff2 [ 0 ];
                    }
  
                    if ( testbuff . Contains ( '\t' ) )
                    {
                        if ( testbuff . StartsWith ( '\t' ) )
                        {
                            tmpbuff2 = testbuff . Trim ( ) . Substring ( 0 , 2 );
                            if ( tmpbuff2 . Contains ( '\t' ) )
                            {
                                int off = tmpbuff2 . IndexOf ( '\t' );
                                tmpbuff = $"Output : {tmpbuff2 . Trim ( ) . Substring ( 0 , off )}]\n";
                                string tmp2 = CheckforCommas ( tmpbuff );
                                output += $"Output : {tmp2 . Trim ( ) . Substring ( 0 , off )}\n";
                            }
                            else
                            {
                                string tmp2 = CheckforCommas ( item );
                                tmp2 = StripTabs ( tmp2 );
                                tmp2 = CheckforComments ( tmp2 );
                                output += $"Output : {tmp2 . Trim ( )}\n";
                            }
                        }
                        if ( testbuff . Contains ( '\t' ) )
                        {
                            int off = testbuff . IndexOf ( '\t' );
                            string tmp2 = testbuff . Trim ( ) . Substring ( 0 , off );
                            string tmp = CheckforCommas ( tmp2 );
                            output += $"Output : {tmp}";
                        }
                        else
                        {
                            string tmp = CheckforCommas ( item );
                            output += $"Output : {tmp . Trim ( )}\n";
                        }
                    }
                    else
                    {
                        // Normal arguments
                        //Check for leading comment --
                        if ( testbuff . TrimStart ( ) . StartsWith ( "--" ) )
                        {
                            argcount++;
                            continue;
                        }
                        //Check for trailing comment --
                        if ( testbuff . Contains ( "--" ) )
                        {
                            string [ ] tmp = testbuff . Split ( "--" );
                            if ( tmp . Length > 1 )
                            {
                                tmp [ 0 ] = StripTabs ( tmp [ 0 ] );
                                string tmp2 = CheckforCommas ( tmp [ 0 ] );
                                output += $"Input : {tmp2}\n";
                            }
                        }
                        else
                        {
                            if ( testbuff . Contains ( "OUTPUT" ) )
                            {
                                int indx = testbuff . IndexOf ( "OUTPUT" );
                                testbuff = $"Output : {testbuff . Substring ( 0 , indx )}";
                                output += $"{testbuff}\n";
                            }
                            else
                            {
                                string tabs = StripTabs ( testbuff );
                                if ( tabs . Length < 5 )
                                    continue;
                                if ( argcount > 0 && tabs . Length > 0 )
                                {
                                    string tmp = CheckforCommas ( tabs );
                                    output += $"Input : {tmp . Trim ( )}\n";
                                }
                                else if ( tabs . Length > 0 )
                                {
                                    string tmp = CheckforCommas ( tabs );
                                    output += $" {tmp . Trim ( )}\n";
                                }
                            }
                        }
                    }
                }
                else
                {
                    // check normal line for problem characters
                    string tmpbuff2 = "";
                    if ( testbuff . Contains ( '\t' ) )
                    {
                        // Leading tab
                        if ( testbuff . StartsWith ( '\t' ) )
                        {
                            if ( testbuff . Length == 1 )
                                continue;
                            tmpbuff2 = testbuff . Trim ( ) . Substring ( 1 );
                            if ( tmpbuff2 . Contains ( '\t' ) )
                            {
                                int off = tmpbuff2 . IndexOf ( '\t' );
                                tmpbuff = tmpbuff2 . Trim ( ) . Substring ( 0 , off );
                                string tmp = CheckforCommas ( tmpbuff );
                                output += $"Input : {tmp . Trim ( ) . Substring ( 0 , off )}\n";
                            }
                            else
                            {
                                string tmp = CheckforCommas ( item );
                                output += $"Input : {tmp . Trim ( )}\n";
                            }
                        }
                        else
                        {
                            // chek for embedded tab 
                            if ( testbuff . Contains ( '\t' ) )
                            {
                                // conains a tab somewehere
                                int off = testbuff . IndexOf ( '\t' );
                                tmpbuff2 = testbuff . Substring ( 0 , off );
                                // check for spurious comas
                                string tmp = CheckforCommas ( tmpbuff2 );
                                output += $"Input : {tmp . Trim ( )}\n";
                            }
                            else
                            {
                                string tmp = CheckforCommas ( item );
                                output += $"Input : {tmp . Trim ( )}\n";
                            }
                        }
                    }
                    else
                    {
                        // Normal arguments, no \t character
                        // check for leading quote marks
                        if ( testbuff . TrimStart ( ) . StartsWith ( "--" ) )
                        {
                            argcount++;
                            continue;
                        }
                        // check for trailing quote marks
                        if ( testbuff . Contains ( "--" ) )
                        {
                            string [ ] tmp = testbuff . Split ( "--" );
                            if ( tmp . Length > 1 )
                            {
                                tmp [ 0 ] = StripTabs ( tmp [ 0 ] );
                                string tmp2 = CheckforCommas ( tmp [ 0 ] );
                                output += $"Input : {tmp2}\n";
                            }
                        }
                        else
                        {
                            string tabs = StripTabs ( item );
                            if ( tabs . Length < 5 )
                                continue;
                            if ( argcount > 0 && tabs . Length > 0 )
                            {
                                string tmp = CheckforCommas ( tabs );
                                output += $"Input : {tmp . Trim ( )}\n";
                            }
                            else if ( tabs . Length > 0 )
                            {
                                string tmp = CheckforCommas ( tabs );
                                output += $" {tmp . Trim ( )}\n";
                            }
                        }
                    }
                }
                argcount++;
            }
            if ( argcount == 1 )
                output += $"\nEither this Procedure does NOT require any parameters,\nor possibly it contains iNVALID Syntax.....\n\n\nTo check for the most obvious issue ensure the script\n" +
                    $"header contains the AS and BEGIN phrases consecutively.\n\nIt may also contain unterminated string default values\nwith only single quote marks ?";
            return output;
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
                int [ ] offsetBegin = FindWithRegex ( buff , "BEGIN" );
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

    }
}
