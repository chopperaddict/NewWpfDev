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
        static public DynamicParameters ParseSqlArgs ( DynamicParameters parameters , string [ ] args )
        {
            // WORKING CORRECTLY 6/11/2022 ?
            DynamicParameters pms = new DynamicParameters ( );
            //            DynamicParameters p = new DynamicParameters ( );
            if ( args != null && args . Length > 0 && args [ 0 ] != "-" )
            {
                pms . AddDynamicParams ( args );
                for ( int x = 0 ; x < args . Length ; x++ )
                {
                    // breakout on first unused array element
                    if ( args [ x ] == "" ) break;
                    if ( args [ x ] . ToUpper ( ) . Contains ( "OUTPUT" ) )
                    {
                        string [ ] splitter = args [ x ] . Split ( " " );
                        pms . Add ( $"{splitter [ 0 ]}" ,
                                           dbType: DbType . String ,
                                           direction: ParameterDirection . Output ,
                                           size: int . MaxValue );
 //                       var properties = args [x] . GetType ( ) . GetProperties ( );
                    }
                    else
                    {
                        pms . Add ( $"Arg{x + 1}" , args [ x ] ,
                       DbType . String );
                    }
                }
            }
            return pms;
        }
    }
}
