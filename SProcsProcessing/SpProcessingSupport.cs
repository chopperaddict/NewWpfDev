using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . Text;

namespace SProcsProcessing
{
    public  class SpProcessingSupport
    {
        //---------------------------------------------------------------------------------------//
        // ceate output string of schema columns info for display from a Generic Collection
        //---------------------------------------------------------------------------------------//
        static public string CreateSchemaReportText ( ObservableCollection<NewWpfDev . GenericClass> gengrid )
        {
            string output = "";
            int paddingsize = 30;
            string underline = $"=========================================";
            bool Finished = false;
            foreach ( NewWpfDev.GenericClass gen in gengrid )
            {
                while ( Finished == false )
                {
                    if ( gen . field1 != null && output == "" )
                    {
                        output += $"SQL Table : {gen . field1 . ToString ( )}\n";
                        output += $"{underline . Substring ( gen . field1 . Length + 12 )}\n";
                    }
                    if ( gen . field2 != null )
                        output += $"Column  :  {gen . field2 . ToString ( )} ";
                    else break;
                    if ( gen . field3 != null )
                        output += $", {gen . field3 . ToString ( )} ";
                    else break;
                    if ( gen . field4 != null && gen . field4 != "0" )
                        output += $", {gen . field4 . ToString ( )} ";
                    else break;
                      if ( gen . field6 != null && gen . field6 != "0" )
                        output += $", {gen . field6 . ToString ( )} ";
                    else break;
                    if ( gen . field7 != null )
                        output += $", {gen . field7 . ToString ( )} ";
                    else break;
                    if ( gen . field8 != null )
                        output += $", {gen . field8 . ToString ( )} ";
                    else break;
                    if ( gen . field9 != null )
                        output += $", {gen . field9 . ToString ( )} ";
                    else break;
                    if ( gen . field10 != null )
                        output += $", {gen . field10 . ToString ( )} ";
                    else break;
                    if ( gen . field11 != null )
                        output += $", {gen . field11 . ToString ( )} ";
                    else break;
                    if ( gen . field12 != null )
                        output += $", {gen . field12 . ToString ( )} ";
                    else break;
                    if ( gen . field13 != null )
                        output += $", {gen . field13 . ToString ( )} ";
                    else break;
                    if ( gen . field14 != null )
                        output += $", {gen . field14 . ToString ( )} ";
                    else break;
                    if ( gen . field15 != null )
                        output += $", {gen . field15 . ToString ( )} ";
                    else break;
                    if ( gen . field16 != null )
                        output += $", {gen . field16 . ToString ( )} ";
                    else break;
                    if ( gen . field17 != null )
                        output += $", {gen . field17 . ToString ( )} ";
                    else break;
                    if ( gen . field18 != null )
                        output += $", {gen . field18 . ToString ( )} ";
                    else break;
                    if ( gen . field19 != null )
                        output += $", {gen . field19 . ToString ( )} ";
                    else break;
                }
                output += "\n";
             }
            return output;
        }
    }
}
