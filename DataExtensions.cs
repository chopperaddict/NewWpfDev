using DapperGenericsLib;

using NewWpfDev . Properties;
using NewWpfDev . UserControls;
using NewWpfDev . ViewModels;

using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . Data . SqlClient;
using System . Diagnostics;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Media;

using GenericClass = DapperGenericsLib . GenericClass;
//using DapperGenericsLib;
namespace NewWpfDev
{
    public static class DataExtensions
    {
        //Extension Method
        /// <summary>
        /// Special method that loads GENERIC format data from Sql Table, & Loads it into specified DataGrid
        /// cleaning away unused columns and even loading the real Column names  for the selected table
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="Tablename"></param>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static ObservableCollection<DapperGenericsLib . GenericClass> LoadGenData (
            this ObservableCollection<DapperGenericsLib . GenericClass> collection ,
            string Tablename ,
            DataGrid grid )
        {
            GenericGridControl genctrl = new GenericGridControl ( null );
//            DataLoad . LoadGenericTable ( Tablename , grid , collection );
            DapperGenLib . LoadTableGeneric ( $"Select * from {Tablename}" , ref collection );
            //            Task . Run ( ( ) => DataLoad . LoadGenericTable ( Tablename , grid , collection ) );
            return collection;
        }

        //Extension Method
        public static List<Dictionary<string , string>> ReplaceDataGridFldNames (
                    this DataGrid Grid1 ,
                    string CurrentType ,
                    ref DataGrid Grid ,
                    ref List<DapperGenericsLib . DataGridLayout> dglayoutlist ,
                    int colcount ,
                    string Domain = "IAN1" )
        {
            List<string> list = new List<string> ( );
            ObservableCollection<DapperGenericsLib.GenericClass> GenClass = new ObservableCollection<DapperGenericsLib . GenericClass> ( );
            Dictionary<string , string> dict = new Dictionary<string , string> ( );
            List<Dictionary<string , string>> ColumntypesList = new List<Dictionary<string , string>> ( );

            // pass down dictionary that will return with column names and SQL types
            Dictionary<string , string> Columntypes = new Dictionary<string , string> ( );
            List<Dictionary<string , string>> ColumnTypesList = new List<Dictionary<string , string>> ( );
            //"ENTERING : " . cwinfo ( );
            // clear reference sturcture first off
            dglayoutlist . Clear ( );

            GenClass = DapperGenLib . GetDbTableColumns ( ref GenClass , ref ColumnTypesList , ref list , CurrentType , Domain , ref dglayoutlist );
            // dglayoutlist is now fully populated        
            int index = 0;
            // Add data  for field size
            if ( GenClass . Count > 0 )
            {
                if ( list . Count > 0 )
                {
                    index = 0;
                    // use the list to get the correct column header info
                    foreach ( var item in Grid . Columns )
                    {
                        DataGridColumn dgc = item;
                        try
                        {
                            dgc . Header = "";
                            dgc . Header = list [ index++ ];
                            item . Header = dgc . Header;
                            if ( index >= dict . Count )
                                break;
                        }
                        catch ( ArgumentOutOfRangeException ex ) { Debug . WriteLine ( $"TODO - BAD Columns - 300 GenericDbHandlers.cs" ); }
                    }
                }
                // Grid now has valid column names, but still got All 20 ??
                Grid . UpdateLayout ( );
            }
            //"EXITING : " . cwinfo ( );
            return ColumnTypesList;
        }

        //Extension Method
        //public static Dictionary<string , string> GetDbTableColumns (
        //    this ObservableCollection<DapperGenericsLib . GenericClass> Gencollection ,
        //    ref List<Dictionary<string , string>> ColumntypesList ,
        //     ref List<string> list ,
        //     string dbName ,
        //     string DbDomain ,
        //     ref List<DataTableLayout> dglayoutlist )
        //{
        //    //$"Entering " . cwinfo ( );
        //    // Make sure we are accessing the correct Db Domain
        //    DapperLibSupport . CheckDbDomain ( DbDomain );
        //    Dictionary<string , string> dict = DapperGenLib . GetSpArgs ( ref Gencollection , ref ColumntypesList , ref list , dbName , DbDomain , ref dglayoutlist );
        //    return dict;
        //}

        #region non Extension support methods
        public static void CheckDbDomain ( string DbDomain )
        {
            //$"Entering " . cwinfo ( 0 );
            if ( DapperGenLib . ConnectionStringsDict == null || DapperGenLib . ConnectionStringsDict . Count == 0 )
                LoadConnectionStrings ( );
            CheckResetDbConnection ( DbDomain , out string constring );
            DapperGenLib . CurrentConnectionString = constring;
            //$"Exiting " . cwinfo ( 0 );
        }
        public static void LoadConnectionStrings ( )
        {
            // This one works just fine - its in NewWpfDev
            //$"Entering " . cwinfo ( 0 );
            Settings defaultInstance = ( ( Settings ) ( global::System . Configuration . ApplicationSettingsBase . Synchronized ( new Settings ( ) ) ) );
            try
            {
                if ( DapperGenLib . ConnectionStringsDict . Count > 0 )
                    return;
                DapperGenLib . ConnectionStringsDict . Add ( "IAN1" , ( string ) Settings . Default [ "BankSysConnectionString" ] );
                DapperGenLib . ConnectionStringsDict . Add ( "NORTHWIND" , ( string ) Settings . Default [ "NorthwindConnectionString" ] );
                DapperGenLib . ConnectionStringsDict . Add ( "PUBS" , ( string ) Settings . Default [ "PubsConnectionString" ] );
                // TODO
                //WpfLib1.Utils.WriteSerializedCollectionJSON(Flags.ConnectionStringsDict, @"C:\users\ianch\DbConnectionstrings.dat");
            }
            catch ( NullReferenceException ex )
            {
                Debug . WriteLine ( $"Dictionary  entrry [{( string ) Settings . Default [ "BankSysConnectionString" ]}] already exists" );
            }
            finally
            {

            }
            //$"Exiting " . cwinfo ( 0 );
        }
        public static bool CheckResetDbConnection ( string currdb , out string constring )
        {
            //string constring = "";
            //$"Entering " . cwinfo ( 0 );
            currdb?.ToUpper ( );
            // This resets the current database connection to the one we re working with (currdb - in UPPER Case!)- should be used anywhere that We switch between databases in Sql Server
            // It also sets the Flags.CurrentConnectionString - Current Connectionstring  and local variable
            if ( GetDictionaryEntry ( DapperGenLib . ConnectionStringsDict , currdb , out string connstring ) != "" )
            {
                if ( connstring != null )
                {
                    DapperGenLib . CurrentConnectionString = connstring;
                    SqlConnection con;
                    con = new SqlConnection ( DapperGenLib . CurrentConnectionString );
                    if ( con != null )
                    {
                        //test it
                        constring = connstring;
                        con . Close ( );
                        //$"Exiting " . cwinfo ( 0 );
                        return true;
                    }
                    else
                    {
                        constring = connstring;
                        $"Exiting with error" . cwwarn ( );
                        return false;
                    }
                }
                else
                {
                    constring = "";
                    $"Exiting with error " . cwwarn ( );
                    return false;
                }
            }
            else
            {
                constring = "";
                $"Exiting with error" . cwwarn ( );
                return false;
            }
        }
        public static string GetDictionaryEntry ( Dictionary<string , string> dict , string key , out string dictvalue )
        {
            string keyval = "";
            //$"Entering " . cwinfo ( 0 );

            if ( dict . Count == 0 )
                LoadConnectionStrings ( );
            if ( dict . TryGetValue ( key . ToUpper ( ) , out keyval ) == false )
            {
                if ( dict . TryGetValue ( key , out keyval ) == false )
                {
                    Debug . WriteLine ( $"Unable to access Dictionary {dict} to identify key value [{key}]" );
                    key = key + "ConnectionString";
                }
            }
            dictvalue = keyval;
            if ( keyval . Contains ( "Data Source" ) == false )
                $"Exiting with No connection string" . cwwarn ( );
           // else
                //$"Exiting " . cwinfo ( 0 );
            return keyval;
        }
        #endregion support methods

    }

}
