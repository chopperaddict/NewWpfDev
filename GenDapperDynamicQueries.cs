using System;
using System . Collections;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . Data;
using System . Data . SqlClient;
using System . Diagnostics;
using System . Linq;
using System . Text;
using System . Windows;
using System . Windows . Documents;

using Dapper;

using GenericSqlLib . Models;

using NewWpfDev . Models;
using NewWpfDev . Views;

namespace NewWpfDev
{
    // Dynamic QUERY methods for main class GenDapperQueries
    // most supporting methods can be found in GenDapperQuerySupport.cs
    public static partial class GenDapperQueries
    {
        //#####################################################################################//
        /// <summary>
        /// Generic Method to implement dapper Queries  of all types
        /// it implements various methods access via the 'method=x' argument
        /// which basically controls  the return type exepcted from each call received
        /// 
        /// method  0 :  returns an IEnumerable<dynamic> INT commandType  is StoredProcedure
        /// method  1 :  returns an IEnumerable<dynamic> STRING where commandType  is TEXT (no args array received)
        /// method  2 :  returns an IEnumerable<dynamic> STRING FROM AN OUTPUT VARIABLE commandType  is StoredProcedure
        /// method  3 :  currenty unused
        /// method  4 :  returns an IEnumerable<dynamic> OBSERVABLECOLLECTION<GENERICclASS> commandType  is StoredProcedure
        /// </summary>
        /// <param name="SqlCommand"></param>
        /// <param name="args"></param>
        /// <param name="resultstring"></param>
        /// <param name="obj"></param>
        /// <param name="Objtype"></param>
        /// <param name="count"></param>
        /// <param name="error"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static dynamic Get_DynamicValue_ViaDapper (
            string SqlCommand ,
            string [ ] args ,
            ref string resultstring ,
            ref object obj ,
            ref Type Objtype ,
            ref int count ,
            ref string error ,
            int method = 0 )
        {
              string connectionString = MainWindow .SqlCurrentConstring;
            SqlConnection sqlCon = null;
            ObservableCollection<GenericClass> temp = new ObservableCollection<GenericClass> ( );
            List<Dictionary<string , string>> list = new List<Dictionary<string , string>> ( );

            // initialize ref variables
            Objtype = null;
            count = 0;

            List<string> stringlist = new List<string> ( );
            ObservableCollection<GenericClass> collection = new ObservableCollection<GenericClass> ( );

            IEnumerable<dynamic> intval;
            dynamic reslt;
            IEnumerable<dynamic> IEintval = null;
            string Con = CheckSetSqlDomain ( MainWindow . CurrentSqlTableDomain );
            if ( Con == "" )
            {
                // set to our local definition
                Con = MainWindow . CurrentSqlTableDomain;
                MessageBox . Show ( $"It was not possible to Identify a valid Sql Connection string for \nthe Database [ {MainWindow . CurrentSqlTableDomain . ToUpper ( )} ]\n\n Please report  this error to DB Technical Support" , "Connection Error" );
                return null;
            }

            try
            {
                using ( sqlCon = new SqlConnection ( Con ) )
                {
                    var parameters = new DynamicParameters ( );
                    parameters = ParseSqlArgs ( parameters , args );
                     $"{SqlCommand}" . DapperTrace ( );

                    //********************************************************************************************************//                    
                    if ( method == 0 )       // use StoredProcedure version  returning IEnumerable<dynamic>
                    {
                        //=================================================================================//
                        IEintval = sqlCon . Query ( SqlCommand , parameters , commandType: CommandType . StoredProcedure );
                        //=================================================================================//

                        int intresult = 0;
                        string stringresult = "";
                        List<string> listresult = new List<string> ( );
                        object objectresult = null;
                        // parse out the int value from the IEnumerable <dynamic> IEintval  variable returned to us
                        dynamic newcount = null;
                        // what are we  expecting back ??
                        if ( Objtype == typeof ( int ) )
                            intresult = parameters . Get<int> ( "result" );
                        else if ( Objtype == typeof ( string ) )
                            stringresult = parameters . Get<string> ( "result" );
                        else if ( Objtype == typeof ( object ) )
                            objectresult = parameters . Get<object> ( "result" );
                        else if ( Objtype == typeof ( List<string>) )
                            listresult = parameters . Get<List<string>> ( "result" );
                        
                        newcount = intresult;
                        foreach ( var item in IEintval )
                        {
                            // set (ref) parameters  to be  returned
                            GetReturnString (newcount, out count , out resultstring );
                            obj = ( object ) -1;
                            Objtype = typeof ( int );
                        }
                        return obj;
                    }
                    else if ( method == 1 )       
                    {
                        string [ ] splitter = args [ 1 ] . Split ( ' ' );
                        SqlCommand += $" {args [ 0 ]}, {splitter [ 1 ]}";

                        //**************************************************************************************************************************************************//
                        // using Text command version returning IEnumerable<dynamic>
                        //**************************************************************************************************************************************************//

                        //=================================================================================//
                        IEintval = sqlCon . Query ( SqlCommand , null , commandType: CommandType . Text );
                        //=================================================================================//

                        // parse out the int value from the IEnumerable <dynamic> IEintval  variable returned to us
                        dynamic newcount = null;

                        foreach ( var item in IEintval )
                        {
                            // set (ref) parameters  to be  returned
                            newcount = item . count;
                            Objtype = typeof ( string );
                            //GetReturnString ( newcount , out count , out resultstring );
                            if ( newcount > 0 )
                            {
                                obj = ( object ) newcount . ToString ( );
                                count = newcount;
                                resultstring = "SUCCESS";
                            }
                            else
                            {
                                obj = ( object ) "-1";
                                count = -1;
                                resultstring = "FAIL";
                            }
                            return obj;
                        }
                    }
                    else if ( method == 2 )
                    {
                        Debug . WriteLine ( $"ProcessUniversalQueryStoredProcedure() : [ {SqlCommand . ToUpper ( )} ]" );

                        //**************************************************************************************************************************************************//
                        // Getting a string from an output  variable '@result'
                        //**************************************************************************************************************************************************//

                        //=================================================================================//
                        IEnumerable<dynamic> result = sqlCon . Query ( SqlCommand , parameters , commandType: CommandType . StoredProcedure );
                        //=================================================================================//

                        Debug . WriteLine ( $"ProcessUniversalQueryStoredProcedure() : {SqlCommand} returned  RESULT = {result}" );
                        string restring = parameters . Get<string> ( "result" );
                        foreach ( var item in result )
                        {
                            // set (ref) parameters  to be  returned
                            Objtype = typeof ( string );
                            if ( result != null )
                            {
                                //int tryintresult = 0;
                                int . TryParse ( restring , out int tryintresult );
                                if ( tryintresult > 0 )
                                    count = tryintresult;
                                obj = ( object ) restring;
                                Objtype = typeof ( string );
                                resultstring = "SUCCESS";
                            }
                            else
                            {
                                obj = null;
                                count = -1;
                                resultstring = "FAIL";
                            }
                            if ( result . ToList ( ) . Count == 0 )
                                return null;
                            return obj;
                        }
                    }
                    else if ( method == 3 )
                    {
                    }
                    else if ( method == 4 )
                    {
                        Debug . WriteLine ( $"ProcessUniversalQueryStoredProcedure() : [ {SqlCommand . ToUpper ( )} ]" );

                        //**************************************************************************************************************************************************//
                        // Getting a generic <GenericClass>collection 
                        // SP should normally be [spLoadTableAsGeneric]
                        //**************************************************************************************************************************************************//

                        //=================================================================================//
                        IEnumerable<dynamic> table = sqlCon . Query<dynamic> ( SqlCommand , parameters , commandType: CommandType . StoredProcedure );
                        //=================================================================================//

                        Debug . WriteLine ( $"ProcessUniversalQueryStoredProcedure() (method=4): {SqlCommand} returned  RESULT = {table}" );
                        //string restring = parameters . Get<string> ( "result" );
                        ObservableCollection<GenericClass> newtable = new ObservableCollection<GenericClass> ( );
                        foreach ( var rows in table )
                        {
                            var fields = rows as IDictionary<string , object>;
                            var sum = fields [ "" ];
                            newtable . Add ( fields as GenericClass );
                            // ...
                        }
                        foreach ( var item in table )
                        {
                            // set (ref) parameters  to be  returned
                            Objtype = typeof ( Collection<GenericClass> );
                            if ( table != null )
                            {
                                //int tryintresult = 0;
                                count = newtable . Count;
                                obj = ( object ) newtable;
                                Objtype = typeof ( ObservableCollection<GenericClass> );
                                resultstring = "SUCCESS";
                            }
                            else
                            {
                                obj = null;
                                count = -1;
                                resultstring = "FAIL";
                            }
                            if ( table . ToList ( ) . Count == 0 )
                                return null;
                            return table;
                        }
                    }
                }
            }
            catch ( Exception ex )
            {
                Debug . WriteLine ( $"SQL error : [ {ex . Message} ]" );
                error = $"SQL error : [ {ex . Message} ]";
                $"{ex . Message} [ {ex . Data} ]" . err ( );
            }
            return null;
        }

        //#####################################################################################//

        public static int CreateGenericCollection (
                 ref ObservableCollection<GenericClass> collection ,
                 string SqlCommand ,
                 string ConnectionString ,
                 string Arguments ,
                 string [ ] args ,
                 string WhereClause ,
                 string OrderByClause ,
                 ref List<string> genericlist ,
                 ref string errormsg )
        {
            //====================================
            // Use DAPPER to run a Stored Procedure
            //====================================
            string result = "";
            errormsg = "";
            genericlist = new List<string> ( );
            string arg1 = "", arg2 = "", arg3 = "", arg4 = "";
            Dictionary<string , object> dict = new Dictionary<string , object> ( );

            string Con = GetCheckCurrentConnectionString ( MainWindow . CurrentSqlTableDomain );
            using ( IDbConnection db = new SqlConnection ( Con ) )
            {
                try
                {
                    // Use DAPPER to run  Stored Procedure
                    try
                    {
                        // Parse out the arguments and put them in correct order for all SP's
                        if ( Arguments . Length > 0 )
                        {
                            if ( Arguments . Contains ( "'" ) )
                            {
                                bool [ ] argsarray = { false , false , false , false };
                                int argscount = 0;
                                // we maybe have args in quotes
                                args = Arguments . Trim ( ) . Split ( '\'' );
                                for ( int x = 0 ; x < args . Length ; x++ )
                                {
                                    if ( args [ x ] . Trim ( ) . Contains ( "," ) )
                                    {
                                        string tmp = args [ x ] . Trim ( );
                                        if ( tmp . Substring ( tmp . Length - 1 , 1 ) == "," )
                                        {
                                            tmp = tmp . Substring ( 0 , tmp . Length - 1 );
                                            args [ x ] = tmp;
                                            argsarray [ x ] = true;
                                            argscount++;
                                        }
                                        else
                                        {
                                            if ( args [ x ] != "" )
                                            {
                                                argsarray [ x ] = true;
                                                argscount++;
                                            }
                                        }
                                    }
                                }
                                for ( int x = 0 ; x < argsarray . Length ; x++ )
                                {
                                    switch ( x )
                                    {
                                        case 0:
                                            if ( argsarray [ x ] == true )
                                                arg1 = args [ x ];
                                            break;
                                        case 1:
                                            if ( argsarray [ x ] == true )
                                                arg2 = args [ x ];
                                            break;
                                        case 2:
                                            if ( argsarray [ x ] == true )
                                                arg3 = args [ x ];
                                            break;
                                        case 3:
                                            if ( argsarray [ x ] == true )
                                                arg4 = args [ x ];
                                            break;
                                    }
                                }
                            }
                            else if ( Arguments . Contains ( "," ) )
                            {
                                args = Arguments . Trim ( ) . Split ( ',' );
                                //string[] args = DbName.Split(',');
                                for ( int x = 0 ; x < args . Length ; x++ )
                                {
                                    switch ( x )
                                    {
                                        case 0:
                                            arg1 = args [ x ];
                                            if ( arg1 . Contains ( "," ) )              // trim comma off
                                                arg1 = arg1 . Substring ( 0 , arg1 . Length - 1 );
                                            break;
                                        case 1:
                                            arg2 = args [ x ];
                                            if ( arg2 . Contains ( "," ) )              // trim comma off
                                                arg2 = arg2 . Substring ( 0 , arg2 . Length - 1 );
                                            break;
                                        case 2:
                                            arg3 = args [ x ];
                                            if ( arg3 . Contains ( "," ) )         // trim comma off
                                                arg3 = arg3 . Substring ( 0 , arg3 . Length - 1 );
                                            break;
                                        case 3:
                                            arg4 = args [ x ];
                                            if ( arg4 . Contains ( "," ) )         // trim comma off
                                                arg4 = arg4 . Substring ( 0 , arg4 . Length - 1 );
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                // One or No arguments
                                arg1 = Arguments;
                                if ( arg1 . Contains ( "," ) )              // trim comma off
                                    arg1 = arg1 . Substring ( 0 , arg1 . Length - 1 );
                            }
                            // Create our aguments using the Dynamic parameters provided by Dapper
                            var Params = new DynamicParameters ( );
                            if ( arg1 != "" )
                                Params . Add ( "Arg1" , arg1 , DbType . String , ParameterDirection . Input , arg1 . Length );
                            if ( arg2 != "" )
                                Params . Add ( "Arg2" , arg2 , DbType . String , ParameterDirection . Input , arg2 . Length );
                            if ( arg3 != "" )
                                Params . Add ( "Arg3" , arg3 , DbType . String , ParameterDirection . Input , arg3 . Length );
                            if ( arg4 != "" )
                                Params . Add ( "Arg4" , arg4 , DbType . String , ParameterDirection . Input , arg4 . Length );
                        }
                        else
                        {
                            // doing it the string[] way
                            var parameters = new DynamicParameters ( );
                            parameters = ParseSqlArgs ( parameters , args );
                        }
                        // Call Dapper to get results using it's StoredProcedures method which returns
                        // a Dynamic IEnumerable that we then parse via a dictionary into collection of GenericClass  records
                        int colcount = 0, maxcols = 0;

                        if ( SqlCommand . ToUpper ( ) . Contains ( "SELECT " ) )
                        {
                            //***************************************************************************************************************//
                            // Performing a standard SELECT command but returning the data in a GenericClass structure	  (Bank/Customer/Details/etc)
                            $"{SqlCommand}" . DapperTrace ( );
                            IEnumerable<dynamic> reslt = null;
                            if ( args . Length > 0 )
                                reslt = db . Query ( SqlCommand , args , commandType: CommandType . Text );
                            else
                                reslt = db . Query ( SqlCommand , commandType: CommandType . Text );
                            //***************************************************************************************************************//
                            if ( reslt == null )
                            {
                                errormsg = "DT";
                                return 0;
                            }
                            else
                            {
                                //Although this is duplicated  with the one below we CANNOT make it a method()
                                errormsg = "DYNAMIC";
                                int dictcount = 0;
                                int fldcount = 0;
                                try
                                {
                                    foreach ( var item in reslt )
                                    {
                                        GenericClass gc = new GenericClass ( );
                                        try
                                        {
                                            // we need to create a dictionary for each row of data then add it to a GenericClass row then add row to Generics Db
                                            string buffer = "";
                                            List<int> VarcharList = new List<int> ( );
                                            gc = ParseDapperRow ( item , dict , out colcount , ref VarcharList );
                                            dictcount = 1;
                                            fldcount = dict . Count;
                                            string tmp = "";
                                            foreach ( var pair in dict )
                                            {
                                                try
                                                {
                                                    if ( pair . Key != null && pair . Value != null )
                                                    {
                                                        AddDictPairToGeneric ( gc , pair , dictcount++ );
                                                        tmp = pair . Key . ToString ( ) + "=" + pair . Value . ToString ( );
                                                        buffer += tmp + ",";
                                                    }
                                                }
                                                catch ( Exception ex )
                                                {
                                                    Debug . WriteLine ( $"Dictionary ERROR : {ex . Message}" );
                                                    result = ex . Message;
                                                }
                                            }
                                            //remove trailing comma
                                            string s = buffer . Substring ( 0 , buffer . Length - 1 );
                                            buffer = s;
                                            genericlist . Add ( buffer );
                                        }
                                        catch ( Exception ex )
                                        {
                                            result = $"SQLERROR : {ex . Message}";
                                            errormsg = result;
                                            Debug . WriteLine ( result );
                                        }
                                        collection . Add ( gc );
                                        dict . Clear ( );
                                        dictcount = 1;
                                    }
                                }
                                catch ( Exception ex )
                                {
                                    Debug . WriteLine ( $"OUTER DICT/PROCEDURE ERROR : {ex . Message}" );
                                    result = ex . Message;
                                    errormsg = result;
                                }
                                if ( errormsg == "" )
                                    errormsg = $"DYNAMIC:{fldcount}";
                                return collection . Count;
                            }
                        }
                        else
                        {
                            // probably a stored procedure ?  							
                            bool IsSuccess = false;
                            int fldcount = 0;

                            //***************************************************************************************************************//
                            // This returns the data from SP commands (only) in a GenericClass Structured format
                            string argsbuff = "";
                            if ( arg1 != "" )
                                argsbuff += $"{arg1}";
                            if ( arg2 != "" )
                                argsbuff += $", {arg2}";
                            if ( arg3 != "" )
                                argsbuff += $", {arg3}";
                            if ( arg4 != "" )
                                argsbuff += $", {arg4}";
                            $"{SqlCommand} {argsbuff}" . DapperTrace ( );

                            var reslt = db . Query ( SqlCommand , null , commandType: CommandType . StoredProcedure );
                            //***************************************************************************************************************//

                            if ( reslt != null )
                            {
                                //Although this is duplicated  with the one above we CANNOT make it a method()
                                int dictcount = 0;
                                dict . Clear ( );
                                long zero = reslt . LongCount ( );
                                try
                                {
                                    foreach ( var item in reslt )
                                    {
                                        GenericClass gc = new GenericClass ( );
                                        try
                                        {
                                            //	Create a dictionary for each row of data then add it to a GenericClass row then add row to Generics Db
                                            List<int> VarcharList = new List<int> ( );
                                            gc = ParseDapperRow ( item , dict , out colcount , ref VarcharList );
                                            dictcount = 1;
                                            fldcount = dict . Count;
                                            if ( fldcount == 0 )
                                            {
                                                //no problem, we will get a Datatable anyway
                                                return 0;
                                            }
                                            string buffer = "", tmp = "";
                                            foreach ( var pair in dict )
                                            {
                                                try
                                                {
                                                    if ( pair . Key != null && pair . Value != null )
                                                    {
                                                        AddDictPairToGeneric ( gc , pair , dictcount++ );
                                                        tmp = pair . Key . ToString ( ) + "=" + pair . Value . ToString ( );
                                                        buffer += tmp + ",";
                                                    }
                                                }
                                                catch ( Exception ex )
                                                {
                                                    Debug . WriteLine ( $"Dictionary ERROR : {ex . Message}" );
                                                    result = ex . Message;
                                                }
                                            }
                                            IsSuccess = true;
                                            string s = buffer . Substring ( 0 , buffer . Length - 1 );
                                            buffer = s;
                                            genericlist . Add ( buffer );
                                        }
                                        catch ( Exception ex )
                                        {
                                            result = $"SQLERROR : {ex . Message}";
                                            Debug . WriteLine ( result );
                                            return 0;
                                        }
                                        //										gc . ActiveColumns = dict . Count;
                                        //ParseListToDbRecord ( genericlist , out gc );
                                        collection . Add ( gc );
                                        dict . Clear ( );
                                        dictcount = 1;
                                    }
                                }
                                catch ( Exception ex )
                                {
                                    Debug . WriteLine ( $"OUTER DICT/PROCEDURE ERROR : {ex . Message}" );
                                    if ( ex . Message . Contains ( "not find stored procedure" ) )
                                    {
                                        result = $"SQL PARSE ERROR - [{ex . Message}]";
                                        $"SQL PARSE ERROR - [{ex . Message}]" . err ( );
                                        errormsg = $"{result}";
                                        return 0;
                                    }
                                    else
                                    {
                                        long x = reslt . LongCount ( );
                                        if ( x == ( long ) 0 )
                                        {
                                            result = $"ERROR : [{SqlCommand}] returned ZERO records... ";
                                            $"ERROR : [{SqlCommand}] returned ZERO records... " . err ( );
                                            errormsg = $"DYNAMIC:0";
                                            return 0;
                                        }
                                        else
                                        {
                                            result = ex . Message;
                                            errormsg = $"UNKNOWN :{ex . Message}";
                                        }
                                        return 0;
                                    }
                                }
                            }
                            if ( IsSuccess == false )
                            {
                                errormsg = $"Dapper request returned zero results, maybe one or more arguments are required, or the Procedure does not return any values ?";
                                $"Dapper request returned zero results, maybe one or more arguments are required, or the Procedure does not return any values ?" . err ( );
                                Debug . WriteLine ( errormsg );
                            }
                            else
                                return fldcount;
                            //return 0;
                        }
                    }
                    catch ( Exception ex )
                    {
                        Debug . WriteLine ( $"STORED PROCEDURE ERROR : {ex . Message}" );
                        $"STORED PROCEDURE ERROR : {ex . Message}" . err ( );
                        result = ex . Message;
                        errormsg = $"SQLERROR : {result}";
                    }
                }
                catch ( Exception ex )
                {
                    Debug . WriteLine ( $"Sql Error, {ex . Message}" );
                    $"Sql Error, {ex . Message}" . err ( );
                    result = ex . Message;
                }
            }
            $"{SqlCommand} Loaded {dict . Count} records" . DapperTrace ( );

            return dict . Count;
        }
        private static void GetReturnString ( int newcount, out int count , out string resultstring )
        {
            if ( newcount > 0 )
            {
                count = newcount;
                resultstring = "SUCCESS";
            }
            else
            {
                count = 0;
                resultstring = "FAIL";
            }
            return;
        }
        //#####################################################################################//

    }
}
