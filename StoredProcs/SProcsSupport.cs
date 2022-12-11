using System;
using System . Collections . Generic;
using System . Data;
using System . Text;

using Dapper;

namespace NewWpfDev . StoredProcs
{
    public class SProcsSupport
    {
        public static DynamicParameters ParseSqlArguments ( DynamicParameters parameters , string [ ] args , ref bool hasoutput , ref bool hasretval )
        {
            // WORKING CORRECTLY 6/11/2022 ?
            if ( args != null && args . Length > 0 && args [ 0 ] != "-" )
            {
                for ( int x = 0 ; x < args . Length ; x++ )
                {
                    // breakout on first unused array element
                    if ( args [ x ] == "" ) break;
                    if ( args [ x ] . ToUpper ( ) . Contains ( "OUTPUT" ) )
                    {
                        string [ ] splitter = args [ x ] . Split ( " " );
                        parameters . Add ( $"{splitter [ 1 ]}" , splitter [ 1 ] ,
                                           DbType . String ,
                                           ParameterDirection . Output ,
                                           splitter [ 1 ] . Length );
                    }
                    else
                    {
                        parameters . Add ( $"Arg{x + 1}" , args [ x ] ,
                       DbType . String ,
                       ParameterDirection . Input ,
                       args [ x ] . Length );
                    }
                }
            }
            return parameters;
        }

    }
}
