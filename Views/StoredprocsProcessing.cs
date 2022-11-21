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
            bool error = false;
            // WORKING CORRECTLY 6/11/2022 ?
            DynamicParameters pms = new DynamicParameters ( );
            if ( args != null && args . Length > 0 && args [ 0 ] != "-" )
            {
                // pms . AddDynamicParams ( args );
                for ( int x = 0 ; x < args . Length ; x++ )
                {
                    if ( args [ x ] == "" ) break;

                    string valid = "0123456789";
                    string [ ] splitter = args [ x ] . Split ( " " );
                    string name = splitter [ 0 ] . Trim ( ) . ToUpper ( );
                    string type = splitter [ 1 ] . Trim ( ) . ToUpper ( );
                    string size = splitter [ 2 ] . Trim ( ) . ToUpper ( );
                    string returntype = splitter [ 3 ] . Trim ( ) . ToUpper ( );

                    if ( name == "" )
                    { error = true; break; }
                    if ( type == "" )
                    { error = true; break; }
                    if ( size == "" )
                    { error = true; break; }
                    if ( returntype != "" && ( returntype != "OUTPUT" && returntype != "RETURN" && returntype != "OUT" ) )
                    { error = true; break; }

                    // process size buffer in case there are () around it
                    string size2 = "";
                    for ( int z = 0 ; z < size . Length ; z++ )
                    {
                        string test = size [ z ] . ToString ( );
                        if ( size . Contains ( test ) )
                            size2 += size [ z ];
                    }
                    int intsize = Convert . ToInt32 ( size2 );
                    if ( intsize <= 0 )
                    { error = true; break; }

                    // Now set them to values passed to us
                    var fulltype = DbType . String;
 
                    if ( splitter [ 3 ] != "" )
                    {
                        returntype = splitter [ 3 ] . Trim ( ) . ToUpper ( );
                        // breakout on first unused array element
                        if ( returntype == "OUTPUT" )
                            returntype = "OUT";
                        else if ( returntype == "RETURN" )
                            returntype = "RETURN";
                    }
                    if ( returntype == "" )
                    {
                        pms . Add ( name ,
                            dbType: fulltype ,
                            size: intsize ,
                            direction: ParameterDirection . Input );
                    }
                    else
                    {
                        pms . Add ( name ,
                            dbType: fulltype ,
                            size: intsize ,
                            direction: ParameterDirection . Output );
                    }
                }
                if ( error == true )
                    return parameters = null;

            }
            return pms;
        }
    }
}