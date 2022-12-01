using System;
using System . Collections . Generic;
using System . Data;
using System . Diagnostics;
using System . Text;
using System . Windows . Input;
using System . Windows;

using Dapper;

using NewWpfDev;
using System . Data . SqlClient;
using NewWpfDev . Views;
using System . Collections;
using System . Linq;
using System . Windows . Documents;
using System . Reflection;
using System . Reflection . Metadata;
using static IronPython . Modules . PythonSocket;
using System . ComponentModel . Design;
using System . Linq . Expressions;

namespace Views
{
    public class StoredprocsProcessing
    {
        //public void SelectSqlPProcessor ( )
        //{
        //    throw new NotImplementedException ( );
        //}

        public static dynamic ProcessGenericDapperStoredProcedure (
            string spCommand ,
            string [ ] args ,
            string CurrDomain ,
            ref string ResultString ,
            ref object Obj ,
            ref Type Objtype ,
            ref int Count ,
            ref string Error ,
            int method = 0 )
        {
            dynamic result = null;
            string Con = "";

            Con = CheckSetSqlDomain ( CurrDomain );
            if ( Con == "" )
            {
                // set to our local definition
                Con = MainWindow . CurrentSqlTableDomain;
                MessageBox . Show ( $"It was not possible to Identify a valid Sql Connection string for \nthe Database [ {CurrDomain . ToUpper ( )} ]\n\n Please report  this error to DB Technical Support" , "Connection Error" );
                return null;
            }

            SqlConnection sqlCon = new SqlConnection ( );
            List<string> queryresults = new List<string> ( );

            "" . Track ( );
            Mouse . OverrideCursor = Cursors . Wait;
            Debug . WriteLine ( $"Running Stored Procedure {spCommand}" );
            using ( sqlCon = new SqlConnection ( Con ) )
            {
                sqlCon . Open ( );
                // Now add record  to SQL table
                var parameters = new DynamicParameters ( );
                parameters = ParseSqlArgs ( parameters , args );
                try
                {
                    $" calling {spCommand} ()" . CW ( );
                    if ( method == 0 )
                    {
                        Debug . WriteLine ( $"ProocessUniversalQueryStoredProcedure() : [ {spCommand . ToUpper ( )} ]" );
                        //**************************************************************************************************************************************************//
                        queryresults = sqlCon . Query<string> ( spCommand , parameters , commandType: CommandType . StoredProcedure ) . ToList ( );
                        //**************************************************************************************************************************************************//
                        //result = sqlCon . Execute ( spCommand , parameters , commandType: CommandType . StoredProcedure );
                        Debug . WriteLine ( $"{spCommand} returned  RESULT = {queryresults . Count}" );
                        ResultString = "SUCCESS";
                        Obj = ( object ) queryresults;
                        Objtype = typeof ( List<string> );
                        Count = queryresults . Count;

                        if ( Objtype == typeof ( List<string> ) )
                            result = ( dynamic ) queryresults;
                        else
                            result = null;
                    }
                }
                catch ( Exception ex )
                {
                    Debug . WriteLine ( $"{ex . Message}" );
                    result = -1;
                    ResultString = "FAIL";
                    Obj = ( object ) queryresults;
                    Objtype = typeof ( List<string> );
                    Count = queryresults . Count;
                    Error = ex . Message;
                    result = ( dynamic ) null;
                }
                Mouse . OverrideCursor = Cursors . Arrow;
                "" . Track ( 1 );
                return result;
            }
        }
        public static string CheckSetSqlDomain ( string domain )
        {
            string ConString = "";
            if ( domain == "" )
                ConString = MainWindow . SqlCurrentConstring;
            else
            {
                ConString = CheckDbDomain ( domain );
                if ( ConString == "" )
                {
                    // set to our local definition
                    ConString = MainWindow . SqlCurrentConstring;
                }
                else
                    MainWindow . SqlCurrentConstring = ConString;
            }
            string [ ] tmp = ConString . Split ( ';' );
            int offset = 0;
            foreach ( var item in tmp )
            {
                if ( item . Contains ( "Catalog" ) )
                {
                    Debug . WriteLine ( $"Sql Domain of {item} confirmed..." );
                    break;
                }
                offset++;
            }
            return ConString;
        }
        public static string CheckDbDomain ( string DbDomain )
        {
            if ( Flags . ConnectionStringsDict == null || Flags . ConnectionStringsDict . Count == 0 )
                Utils . LoadConnectionStrings ( );
            if ( DbDomain == "" )
                DbDomain = MainWindow . CurrentSqlTableDomain;
            Utils . CheckResetDbConnection ( DbDomain , out string constring );
            //         ConnString = constring;
            Flags . CurrentConnectionString = constring;
            MainWindow . SqlCurrentConstring = constring;
            //Debug . WriteLine ( $"Current Domain confirmed as {DbDomain} ..." );
            return constring;
        }
        /// <summary>
        /// Parse a set of string[] to create paramaters.Add(0 statements
        /// where 
        /// [0] = data
        /// [1] = DbType
        /// [2] = Size
        /// [3] = ParameterDirection 
        /// if (direction is INPUT, only args 0-1 are required
        /// if (direction is OUTPUT, args 0-3 are required
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        static public DynamicParameters ParseNewSqlArgs ( DynamicParameters parameters , List<string [ ]> argsbuffer, out string error )
        {
            DynamicParameters pms = new DynamicParameters ( );
            error = "";
            try
            {
                /*
                 order is :
                @name
                Argument
                Type
                Size
                Direction
                 */
                int argcount = 0;
                for ( var i = 0 ; i < argsbuffer . Count ; i++ )
                {
                    DbType argtype = DbType . Object;
                    string [ ] args = new string [ 6 ];
                    args = argsbuffer [ i ];
                    args = PadArgsArray ( args );
                    int y = 0;
                    int [ ] argindx = new int [ 5 ];
                    for ( int z = 0 ; z < 5 ; z++ )
                    {
                        if ( args [ z ] != "" )
                            argindx [ z ] = 1;
                        else
                            argindx [ z ] = 0;
                    }
                    // Got 1st 2 only (default direction)
                    if ( argindx [ 0 ] == 1 && argindx [ 1 ] == 0 && argindx [ 2 ] == 0 && argindx [ 3 ] == 0  )
                    {
                        pms . Add ( args [ 0 ] );
                        argcount++;
                        continue;
                    }
                    if ( argindx [ 0 ] == 1 && argindx [ 1 ] == 1 && argindx [ 2 ] == 0 && argindx [ 3 ] == 0 )
                    {
                        pms . Add ( args [ 0 ]
                            , value: args [ 1 ] );
                        argcount++;
                        continue;
                    }
                    if ( argindx [ 0 ] == 1 && argindx [ 1 ] == 1 && argindx [ 2 ] == 1 && argindx [ 3 ] == 0 && argindx [ 4 ] == 0 )
                    {
                        pms . Add ( args [ 0 ]
                            , value: args [ 1 ]
                            , dbType: GetArgType ( args ) );
                        argcount++;
                        continue;
                    }
                    // Got 1st 2 + arg type + arg size only (default direction)
                    if ( argindx [ 0 ] == 1 && argindx [ 1 ] == 1 && argindx [ 2 ] == 1 && argindx [ 3 ] == 1 && argindx [ 4 ] == 0 )
                    {
                        pms . Add ( args [ 0 ]
                            , value: args [ 1 ]
                             , size: Convert . ToInt32 ( args [ 3 ] ) );
                        argcount++;
                        continue;
                    }
                    // Got 1st 2 + arg direction  (default direction)
                    if ( argindx [ 0 ] == 1 && argindx [ 1 ] == 1 && argindx [ 2 ] == 1 && argindx [ 3 ] == 1 && argindx [ 4 ] == 1 )
                    {
                        pms . Add ( args [ 0 ] ,
                        value: args [ 1 ]
                        , dbType: GetArgType ( args )
                        , direction: GetDirection ( args ) );
                        argcount++;
                        continue;
                    }
                    // Got @ + arg type + direction
                    if ( argindx [ 0 ] == 1 && argindx [ 1 ] == 0 && argindx [ 2 ] == 1 && argindx [ 3 ] == 0 && argindx [ 4 ] == 1 )
                    {
                        pms . Add ( args [ 0 ]
                        , dbType: GetArgType ( args )
                        , direction: GetDirection ( args ) );
                        argcount++;
                        continue;
                    }
                    // Got value only - ILLEGAL
                    if ( argindx [ 0 ] == 1 && argindx [ 1 ] == 0 && argindx [ 2 ] == 0 && argindx [ 3 ] == 0 && argindx [ 4 ] == 0 )
                    {
                        pms . Add ( "" , value: args [ 1 ] );
                        continue;
                    }
                    // Got 1st 2 + size - ILLEGAL
                    if ( argindx [ 0 ] == 1 && argindx [ 1 ] == 1 && argindx [ 2 ] == 0 && argindx [ 3 ] == 1 && argindx [ 4 ] == 0 )
                    {
                        pms . Add ( args [ 0 ]
                            , value: args [ 1 ]
                            , size: Convert . ToInt32 ( args [ 3 ] ) );
                        continue;
                    }
                    // Got 1st 2 + size + direction- ILLEGAL
                    if ( argindx [ 0 ] == 1 && argindx [ 1 ] == 1 && argindx [ 2 ] == 0 && argindx [ 3 ] == 1 && argindx [ 4 ] == 1 )
                    {
                        pms . Add ( args [ 0 ]
                            , value: args [ 1 ]
                            , size: Convert . ToInt32 ( args [ 3 ] )
                            , direction: GetDirection ( args ) );
                        continue;
                    }
                }
                if ( argcount < argsbuffer . Count )
                    error = "One or more invalid arguments identified";
            }
            catch ( Exception ex )
            {
                Debug . WriteLine ( ex . Message );
            }
            return pms;
        }
        public static string [ ] PadArgsArray ( string [ ] content )
        {
            string [ ] tmp = new string [ 6 ];
            for ( int x = 0 ; x < 6 ; x++ )
            {
                if ( content [ x ] != null )
                    tmp [ x ] = content [ x ];
                else
                    tmp [ x ] = "";
            }
            return tmp;
        }

        static public DbType GetArgType ( string [ ] type )
        {
            if ( type [ 2 ] == "" ) return DbType . String;
            if ( type [ 2 ] . Contains ( "STR" ) || type [ 2 ] . Contains ( "STR" ) ) return DbType . String;
            if ( type [ 2 ] . Contains ( "INT" ) ) return DbType . Int32;
            if ( type [ 2 ] . Contains ( "FLOAT" ) ) return DbType . Double;
            if ( type [ 2 ] . Contains ( "VARCHAR" ) ) return DbType . String;
            if ( type [ 2 ] . Contains ( "VARBIN" ) ) return DbType . Binary;
            if ( type [ 2 ] . Contains ( "TEXT" ) ) return DbType . String;
            if ( type [ 2 ] . Contains ( "BIT" ) ) return DbType . Boolean;
            if ( type [ 2 ] . Contains ( "BOOL" ) ) return DbType . Boolean;
            if ( type [ 2 ] . Contains ( "SMALLINT" ) ) return DbType . Int16;
            if ( type [ 2 ] . Contains ( "BIGINT" ) ) return DbType . Int64;
            if ( type [ 2 ] . Contains ( "DOUBLE" ) ) return DbType . Double;
            if ( type [ 2 ] . Contains ( "DEC" ) ) return DbType . Decimal;
            if ( type [ 2 ] . Contains ( "CURR" ) ) return DbType . Currency;
            if ( type [ 2 ] . Contains ( "DATETIME" ) ) return DbType . DateTime;
            if ( type [ 2 ] . Contains ( "DATE" ) ) return DbType . Date;
            if ( type [ 2 ] . Contains ( "TIMESTAMP" ) ) return DbType . Time;
            if ( type [ 2 ] . Contains ( "TIME" ) ) return DbType . Time;

            return DbType . Object;
        }
        static public Size GetArgSize ( string [ ] args )
        {
            int size = 0;
            if ( args [ 3 ] != "" && args [ 3 ] != "MAX" )
            {
                size = Convert . ToInt32 ( args [ 3 ] );
                Size sz = Size . Parse ( size . ToString ( ) );
                return Size . Parse ( args [ 3 ] );
            }
            else if ( args [ 3 ] == "MAX" )
                return Size . Empty;
            return Size . Empty;
        }
        static public ParameterDirection GetDirection ( string [ ] args )
        {
            if ( args [ 4 ] == "" || args [ 4 ] . Contains ( "IN" ) )
                return ParameterDirection . Input;
            else if ( args [ 4 ] . Contains ( "OUT" ) )
                return ParameterDirection . Output;

            return ParameterDirection . Input;
        }
        static public DynamicParameters ParseSqlArgs ( DynamicParameters parameters , string [ ] args )
        {
            bool error = false;
            // WORKING CORRECTLY 6/11/2022 ?
            DynamicParameters pms = new DynamicParameters ( );
            if ( args != null && args . Length > 0 && args [ 0 ] != "-" )
            {
                // pms . AddDynamicParams ( args );
                int counter = 1;
                for ( int x = 0 ; x < args . Length ; x += 4 )
                {
                    if ( args [ x ] == "" ) break;

                    string name = "";
                    string type = "";
                    string size = "";
                    string returntype = "";
                    string valid = "0123456789";
                    string arg = "";
                    int index = 0;
                    if ( args [ 4 ] == "INPUT" )
                    {
                        if ( args [ 2 ] . ToUpper ( ) == "STRING" )
                        {
                            // WORKING 20/11/2022 !!
                            pms . Add ( $"{args [ 1 ]}" , args [ 0 ] ,
                             dbType: DbType . String ,
                            size: Convert . ToInt32 ( args [ 0 ] . Length ) ,
                            direction: ParameterDirection . Input );
                        }
                        else if ( args [ 1 ] == "INT" )
                        {
                            pms . Add ( $"{args [ 1 ]}" , args [ 0 ] ,
                             dbType: DbType . Int32 ,
                            size: Convert . ToInt32 ( args [ 0 ] . Length ) ,
                            direction: ParameterDirection . Input );
                        }
                    }
                    else
                    {
                        // Output args
                        if ( args [ 1 ] == "STRING" )
                        {
                            pms . Add ( $"{args [ 1 ]}" , args [ 0 ] ,
                             dbType: DbType . String ,
                            size: Convert . ToInt32 ( args [ 0 ] . Length ) ,
                            direction: ParameterDirection . Output );
                        }
                        else if ( args [ 1 ] == "INT" )
                        {
                            pms . Add ( $"{args [ 1 ]}" , args [ 0 ] ,
                             dbType: DbType . Int32 ,
                            size: Convert . ToInt32 ( args [ 0 ] . Length ) ,
                            direction: ParameterDirection . Output );
                        }
                    }
                    if ( error == true )
                        return parameters = null;
                }
                counter++;
            }
            return pms;
        }
    }
}