//using NewWpfDev. Dapper;
using NewWpfDev . ViewModels;

using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . Data;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows . Input;
using System . Windows;
using System . Collections . Specialized;
using NewWpfDev . Views;
using System . Data . SqlClient;
using System . Diagnostics;
using Dapper;
using System . Collections;
using System . Linq . Expressions;
using System . Windows . Documents;
using System . Windows . Controls;
using NewWpfDev . Dapper;
using NewWpfDev . Sql;

namespace NewWpfDev . Models
{
    public static class GenericDbUtilities
    {
        public static Dictionary<string , string> dict = new Dictionary<string , string> ( );
        private static string ConnString { get; set; }

        /// <summary>
        /// Used when a query returns zero records to provide feedback in any datagrid
        /// </summary>
        /// <param name="Grid"></param>
        public static void SetNullRecords ( ObservableCollection<GenericClass> genaccts , DataGrid Grid , string dbname )
        {
            GenericClass gc = new GenericClass ( );
            gc . field1 = "     ";
            genaccts . Add ( gc );
            GenericClass gc2 = new GenericClass ( );
            gc2 . field1 = $"The database Query was completed successfully";
            genaccts . Add ( gc2 );
            GenericClass gc3 = new GenericClass ( );
            gc3 . field1 = $"but no records were returned for the Database Table  {dbname} ...";
            genaccts . Add ( gc3 );
            SqlServerCommands . LoadActiveRowsOnlyInGrid ( Grid , genaccts , SqlServerCommands . GetGenericColumnCount ( genaccts ) );
            Grid . Columns [ 0 ] . Header = "Query result Information";
        }
        public static void CheckDbDomain ( string DbDomain )
        {
            if ( Flags . ConnectionStringsDict == null || Flags . ConnectionStringsDict . Count == 0 )
                Utils . LoadConnectionStrings ( );
            Utils . CheckResetDbConnection ( DbDomain , out string constring );
            ConnString = constring;
            Flags . CurrentConnectionString = constring;
        }

        public static Dictionary<string , string> GetDbTableColumns ( ref ObservableCollection<ViewModels . GenericClass> Gencollection , ref List<string> list , string dbName , string DbDomain , ref List<int> VarCharLength )
        {
            // Make sure we are accessing the correct Db Domain
            CheckDbDomain ( DbDomain );
            dict = GetSpArgs ( ref Gencollection , ref list , dbName , DbDomain , ref VarCharLength );
            return dict;
        }
        private static Dictionary<string , string> GetSpArgs ( ref ObservableCollection<GenericClass> Gencollection , ref List<string> list , string dbName , string DbDomain , ref List<int> VarCharLength )
        {
//#pragma warning disable CS0219 // The variable 'output' is assigned but its value is never used
//            string output = "";
//#pragma warning restore CS0219 // The variable 'output' is assigned but its value is never used
//#pragma warning disable CS0219 // The variable 'errormsg' is assigned but its value is never used
//            string errormsg = "";
//#pragma warning restore CS0219 // The variable 'errormsg' is assigned but its value is never used
//#pragma warning disable CS0219 // The variable 'columncount' is assigned but its value is never used
//            int columncount = 0;
//#pragma warning restore CS0219 // The variable 'columncount' is assigned but its value is never used
//#pragma warning disable CS0219 // The variable 'IsSuccess' is assigned but its value is never used
//            bool IsSuccess = false;
//#pragma warning restore CS0219 // The variable 'IsSuccess' is assigned but its value is never used
//            DataTable dt = new DataTable ( );
            GenericClass genclass = new GenericClass ( );
            Dictionary<string , string> dict = new Dictionary<string , string> ( );
            try
            {
                // also get a List<int> of (nvarchar) string lengths
                //Gencollection = LoadDbAsGenericData ( ref list , "spGetTableColumnWithSize" , dbName , DbDomain , ref VarCharLength , true );
                Gencollection = LoadDbAsGenericData ( ref list , "spGetTableColumns" , dbName , DbDomain , ref VarCharLength , true );
            }
            catch ( Exception ex )
            {
                MessageBox . Show ( $"SQL ERROR 1125 - {ex . Message}" );
                dict . Clear ( );
                return dict;
            }

            dict . Clear ( );
            list . Clear ( );
            //			int charlenindex=0;
            try
            {
                foreach ( var item in Gencollection )
                {
                    GenericClass gc = new GenericClass ( );
                    gc = item as GenericClass;
                    //					gc . field3 = VarCharLength[charlenindex++] . ToString ( );
                    dict . Add ( gc . field1 , gc . field2 );
                    list . Add ( gc . field1 . ToString ( ) );
                }
            }
            catch ( Exception ex )
            {
                Debug . WriteLine ( ex . Message );
            }
            return dict;
        }
        /// <summary>
        /// Returns  a GENERIC collection, plus a List<string> using an SP
        /// </summary>
        /// <param name="GenClass"></param>
        /// <param name="list"></param>
        /// <param name="SqlCommand"></param>
        /// <param name="Arguments"></param>
        /// <param name="DbDomain"></param>
        /// <returns></returns>
        public static ObservableCollection<GenericClass> LoadDbAsGenericData (
            ref List<string> list ,
            string SqlCommand ,
            string Arguments ,
            string DbDomain ,
            ref List<int> VarCharLength ,
            bool GetLengths = false )
        {
            string result = "";
#pragma warning disable CS0219 // The variable 'IsSuccess' is assigned but its value is never used
            bool IsSuccess = false;
#pragma warning restore CS0219 // The variable 'IsSuccess' is assigned but its value is never used
            string arg1 = "", arg2 = "", arg3 = "", arg4 = "";
            // provide a default connection string
            string ConString = "BankSysConnectionString";
            Dictionary<string , object> dict = new Dictionary<string , object> ( );
            ObservableCollection<GenericClass> GenClass = new ObservableCollection<GenericClass> ( );
            // Ensure we have the correct connection string for the current Db Doman
            Utils . CheckResetDbConnection ( DbDomain , out string constr );
            Flags . CurrentConnectionString = constr;
            ConString = constr;

            using ( IDbConnection db = new SqlConnection ( ConString ) )
            {
                try
                {
                    // Use DAPPER to run  Stored Procedure
                    // One or No arguments
                    arg1 = Arguments;
                    if ( arg1 . Contains ( "," ) )              // trim comma off
                        arg1 = arg1 . Substring ( 0 , arg1 . Length - 1 );
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

                    //***************************************************************************************************************//
                    // This returns the data from SP commands (only) in a GenericClass Structured format
                    var reslt = db . Query ( SqlCommand , Params , commandType: CommandType . StoredProcedure );
                    //***************************************************************************************************************//

                    if ( reslt != null )
                    {
                        //Although this is duplicated  with the one above we CANNOT make it a method()
                        int dictcount = 0;
                        dict . Clear ( );
                        long zero = reslt . LongCount ( );
                        try
                        {
                            int colcount = 0, fldcount = 0;
                            foreach ( var item in reslt )
                            {
                                GenericClass gc = new GenericClass ( );
                                try
                                {
                                    //	Create a dictionary for each row of data then add it to a GenericClass row then add row to Generics Db
                                    gc = DapperSupport . ParseDapperRow ( item , dict , out colcount , ref VarCharLength , GetLengths );
                                    //VarcharList . Add ( VarCharLength );
                                    dictcount = 1;
                                    fldcount = dict . Count;
                                    if ( fldcount == 0 )
                                    {
                                        //no problem, we will get a Datatable anyway
                                        return GenClass;
                                    }
                                    string buffer = "", tmp = "";
                                    foreach ( var pair in dict )
                                    {
                                        try
                                        {
                                            if ( pair . Key != null && pair . Value != null )
                                            {
                                                DapperSupport . AddDictPairToGeneric ( gc , pair , dictcount++ );
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
                                    //string s = buffer . Substring (0, buffer . Length - 1 );
                                    //buffer = s;
                                    //genericlist . Add ( buffer );
                                }
                                catch ( Exception ex )
                                {
                                    result = $"SQLERROR : {ex . Message}";
                                    Debug . WriteLine ( result );
                                    return GenClass;
                                }
                                //										gc . ActiveColumns = dict . Count;
                                //ParseListToDbRecord ( genericlist , out gc );
                                GenClass . Add ( gc );
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
                                //								errormsg = $"{result}";
                                return GenClass;
                            }
                            else
                            {
                                long x = reslt . LongCount ( );
                                if ( x == ( long ) 0 )
                                {
                                    result = $"ERROR : [{SqlCommand}] returned ZERO records... ";
                                    //									errormsg = $"DYNAMIC:0";
                                    return GenClass;
                                }
                                else
                                {
                                    result = ex . Message;
                                    //									errormsg = $"UNKNOWN :{ex . Message}";
                                }
                                return GenClass;
                            }
                        }
                    }
                }
                catch ( Exception ex )
                { Debug . WriteLine ( $"{ex . Message}" ); }
            }
            return GenClass;
        }
        /// <summary>
        /// CLEVER METHOD
        /// This Method recieves a datagrid prefilled with GENERIC STYLE data (field1, field2, etc)
        /// and then uses the spGetTableNames S.Proc toi get the real field names
        /// and replaces the column headers with them instead, assuming they are retrieved successfully
        /// </summary>
        /// <param name="CurrentType"></param>
        /// <param name="Grid1"></param>
        public static void ReplaceDataGridFldNames ( string CurrentType , ref DataGrid Grid1 , string Domain = "IAN1" )
        {
            List<string> list = new List<string> ( );
            ObservableCollection<GenericClass> GenericClass = new ObservableCollection<GenericClass> ( );
            Dictionary<string , string> dict = new Dictionary<string , string> ( );
            // This returns a Dictionary<sting,string> PLUS a collection  and a List<string> passed by ref....
            List<int> VarCharLength = new List<int> ( );
            dict = GenericDbUtilities . GetDbTableColumns ( ref GenericClass , ref list , CurrentType , Domain , ref VarCharLength );
            int index = 0;
            if ( list . Count > 0 )
            {
                index = 0;
                // use the list to get the correct column header info
                foreach ( var item in Grid1 . Columns )
                {
                    DataGridColumn dgc = item;
                     try
                    {
                        dgc . Header = "";
                        dgc . Header = list [ index++ ];
                        if ( index >= dict . Count )
                        {
                           break;
                        }
                    }
                    catch ( ArgumentOutOfRangeException  ex ) { Debug . WriteLine ( $"TODO - BAD Columns - 300 GenericDbHandlers.cs" ); }
                }
            }
            Grid1 . UpdateLayout ( );
        }

        public static List<string> GetDataDridRowsWithSizes ( DataTable dt )
        {
            List<string> list = new List<string> ( );
            foreach ( DataRow row in dt . Rows )
            {
                var txt = row . Field<string> ( 0 );
                list . Add ( txt );
                txt = row . Field<string> ( 1 );
                list . Add ( txt );
                if ( row . Field<object> ( 2 ) != null )
                {
                    txt = row . Field<object> ( 2 ) . ToString ( );
                    list . Add ( txt );
                }
                else
                    list . Add ( "---" );
                //txt = row . Field<string> ( 1 );
                //list . Add ( txt);
                //object obj = row . Field<object>(0);
                //if( obj == typeof ( Int16 ) || obj == typeof ( Int32 ) || obj == typeof ( int ) )
                //	list . Add ( obj.ToString() );
            }
            return list;
        }

        #region Support methods
        private static DataTable ProcessSqlCommand ( string SqlCommand )
        {
            SqlConnection con;
            DataTable dt = new DataTable ( );
#pragma warning disable CS0219 // The variable 'filterline' is assigned but its value is never used
            string filterline = "";
#pragma warning restore CS0219 // The variable 'filterline' is assigned but its value is never used
            //string ConString = Flags . CurrentConnectionString;
            string ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
            //Debug . WriteLine ( $"Making new SQL connection in DETAILSCOLLECTION,  Time elapsed = {timer . ElapsedMilliseconds}" );
            //SqlCommand += " TempDb";
            con = new SqlConnection ( ConString );
            try
            {
                Debug . WriteLine ( $"Using new SQL connection in PROCESSSQLCOMMAND" );
                using ( con )
                {
                    SqlCommand cmd = new SqlCommand ( SqlCommand , con );
                    SqlDataAdapter sda = new SqlDataAdapter ( cmd );
                    sda . Fill ( dt );
                }
            }
            catch ( Exception ex )
            {
                Debug . WriteLine ( $"ERROR in PROCESSSQLCOMMAND(): Failed to load Datatable :\n {ex . Message}, {ex . Data}" );
                MessageBox . Show ( $"ERROR in PROCESSSQLCOMMAND(): Failed to load datatable\n{ex . Message}" );
            }
            finally
            {
                Debug . WriteLine ( $" SQL data loaded from SQLCommand [{SqlCommand . ToUpper ( )}]" );
                con . Close ( );
            }
            return dt;
        }

        #endregion Support methods
    }
}
