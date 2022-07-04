using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . Data;
using System . Data . SqlClient;
using System . Diagnostics;
using System . Linq;
using System . Windows;
using System . Windows . Threading;

using Dapper;

using NewWpfDev. Models;
using NewWpfDev. Views;

namespace NewWpfDev. ViewModels
{
    /// <summary>
    /// Class to handle data access to BankAccount /Customer for ## MY ## UserControls
    /// </summary>
    public class UserControlDataAccess
    {
        private static ObservableCollection<BankAccountViewModel> bcollection { get; set; }
        private static DataTable dtBank;
        private static bool USEFULLTASK = true;
        private static bool Notify { get; set; } = false;
        private static string Caller = "";

        #region BANKACCOUNT Data Access
        //*************************************//
        // MAIN LOADING METHOD CALL (wrappper)
        //*************************************//
        // MAIN STANDARD CALLED METHOD - Redirector (wrapper method) that calls main method, but with far fewer parameters required
        // Returns colection in both  cases of notification requested
        public static ObservableCollection<BankAccountViewModel> GetBankObsCollection ( bool NotifyAll , string caller , int max = -1 )
        {
            Notify = NotifyAll;
            Caller = caller;
            bcollection = new ObservableCollection<BankAccountViewModel> ( );
            dtBank = new DataTable ( );

            try
            {
                Debug . WriteLine ( "Loading BankAccount table in \"GetBankObsCollection\"!!!" );
                dtBank = ReadBankData ( max );
                if ( dtBank == null ) {
                    Debug . WriteLine ("BankAccount failed  to load from table !!!");
                    return null;
                }
                // collection = new ObservableCollection<BankAccountViewModel> ( );
                Debug . WriteLine ( "Loading BankAccount into Bvm in \"GetBankObsCollection\"!!!" );
                LoadBankCollection ( bcollection , dtBank );
            }
            catch ( Exception ex )
            {
                Debug. WriteLine ( $"Bank loading failed... : {ex . Message} : {ex . Data}" );
            }
            return bcollection;
        }


        #region LOAD THE DATA INTO COLLECTION

        /// Handles the actual conneciton ot SQL to load  Bank Db data required
        /// </summary>
        /// <returns></returns>
        public static DataTable ReadBankData ( int max = 0 , string caller = "" , bool isMultiMode = false )
        {
            object bptr = new object ( );
            try
            {
                SqlConnection con;
                string ConString = "";
                ConString = Flags . CurrentConnectionString;
                if ( ConString == "" )
                {
                    GenericDbUtilities . CheckDbDomain ( "IAN1" );
                    ConString = Flags . CurrentConnectionString;
                    if ( ConString == "" )
                        MessageBox . Show (
                            $"The system could not find the Flags.CurrentConnectionString collection\nPlease ensure you have the FLAGS.CS file \nand/or the GENERICDBHANDLERS.CS in this project" ,
                            "Data Access Error" );
                    return null;
                }
                con = new SqlConnection ( ConString );
                using ( con )
                {
                    string commandline = "";
                    if ( Flags . IsMultiMode )
                    {
                        // Create a valid Query Command string including any active sort ordering
                        commandline = $"SELECT * FROM BANKACCOUNT WHERE CUSTNO IN "
                            + $"(SELECT CUSTNO FROM BANKACCOUNT "
                            + $" GROUP BY CUSTNO"
                            + $" HAVING COUNT(*) > 1) ORDER BY ";
                    }
                    else if ( Flags . FilterCommand != "" )
                    {
                        commandline = Flags . FilterCommand;
                    }
                    else if ( max != -1 )
                    {
                        commandline = $"Select Top ({max})  * from BankAccount order by CustNo";
                    }
                    else
                    {
                        // Create a valid Query Command string including any active sort ordering
                        commandline = "Select * from [BankAccount] order by ";
                        commandline = Utils . GetDataSortOrder ( commandline );
                    }
                    SqlCommand cmd = new SqlCommand ( commandline , con );
                    SqlDataAdapter sda = new SqlDataAdapter ( cmd );
                    if ( dtBank == null )
                        dtBank = new DataTable ( );
                    sda . Fill ( dtBank );
                }
            }
            catch ( Exception ex )
            {
                Debug . WriteLine ( $"Failed to load Bank Details - {ex . Message}, {ex . Data}" );
                return null;
            }
            return dtBank;
        }

        // Main Data load method that handles trigger notification
        public static bool LoadBankCollection ( ObservableCollection<BankAccountViewModel> bcollection , DataTable dtBank )
        {
            int count = 0;
            try
            {
                object bptr = new object ( );
                for ( int i = 0 ; i < dtBank . Rows . Count ; i++ )
                {
                    bcollection . Add ( new BankAccountViewModel
                    {
                        Id = Convert . ToInt32 ( dtBank . Rows [ i ] [ 0 ] ) ,
                        BankNo = dtBank . Rows [ i ] [ 1 ] . ToString ( ) ,
                        CustNo = dtBank . Rows [ i ] [ 2 ] . ToString ( ) ,
                        AcType = Convert . ToInt32 ( dtBank . Rows [ i ] [ 3 ] ) ,
                        Balance = Convert . ToDecimal ( dtBank . Rows [ i ] [ 4 ] ) ,
                        IntRate = Convert . ToDecimal ( dtBank . Rows [ i ] [ 5 ] ) ,
                        ODate = Convert . ToDateTime ( dtBank . Rows [ i ] [ 6 ] ) ,
                        CDate = Convert . ToDateTime ( dtBank . Rows [ i ] [ 7 ] ) ,
                    } );
                    count = i;
                }
            }
            catch ( Exception ex )
            {
                Debug . WriteLine ( $"BANK : SQL Error in BankCollection(351) load function : {ex . Message}, {ex . Data}, count was {count}" );
            }
            finally
            {
                // This is ONLY called  if a requestor specifies the file wide variable argument as TRUE
                if ( UserControlDataAccess . Notify )
                {
                    // $"Dispatcher on UI thread =  {Application . Current . Dispatcher . CheckAccess ( )}" . CW ( );
                    //if ( Application . Current . Dispatcher . CheckAccess ( ) == false )
                    //{
                    //"Triggeriing BankDataLoaded inside a new App Dispatcher..." . CW ( );
                    //Application . Current . Dispatcher . Invoke ( ( ) =>
                    Debug . WriteLine ( $"Calling TriggerBankDataLoaded with {bcollection.Count} records in \"LoadBankCollection\" for {Caller}!!!" );
                    EventControl . TriggerBankDataLoaded ( null ,
                        new LoadedEventArgs
                        {
                            CallerType = Caller ,
                            CallerDb = Caller ,
                            DataSource = bcollection ,
                            RowCount = bcollection . Count
                        });
                        //} ) );
                }
                //else
                //{
                //    "Triggeriing BankDataLoaded in current UI Dispatcher..." . CW ( );
                //    EventControl . TriggerBankDataLoaded ( null ,
                //    new LoadedEventArgs
                //    {
                //        CallerType = Caller ,
                //        CallerDb = Caller ,
                //        DataSource = bcollection ,
                //        RowCount = bcollection . Count
                //    //} );
                //}
            }
            //Debug. WriteLine ( $"Data Loaded for: [{Caller}], Records = {bcollection . Count}" );

            return true;
        }
        #endregion


        // MAIN "ASYNC" DIECT CALL METHOD - 
        public static ObservableCollection<BankAccountViewModel> GetBankObsCollectionAsync ( ObservableCollection<BankAccountViewModel> collection , string SqlCommand = "" , bool Notify = false , string Caller = "" )
        {
            //This is a clever variation that uses Dapper and LINQ to load Data via SQL directly into a List<>
            //that is then converted into the requested Obs.Collection directly within this method
            // sort of one stop shop !
            ObservableCollection<BankAccountViewModel> bvmcollection = new ObservableCollection<BankAccountViewModel> ( );
            //bvmcollection = collection;
            List<BankAccountViewModel> bvmlist = new List<BankAccountViewModel> ( );
            string ConString = Flags . CurrentConnectionString;
            if ( ConString == "" )
            {
                GenericDbUtilities . CheckDbDomain ( "IAN1" );
                ConString = Flags . CurrentConnectionString;
                if ( ConString == "" )
                    MessageBox . Show (
                        $"The system could not find the Flags.CurrentConnectionString collection\nPlease ensure you have the FLAGS.CS file \nand/or the GENERICDBHANDLERS.CS in this project" ,
                        "Data Access Error" );
                return null;
            }
            Application . Current . Dispatcher . Invoke ( ( ) =>
            {
                using ( IDbConnection db = new SqlConnection ( ConString ) )
                {
                    try
                    {
                        if ( SqlCommand == "" )
                            bvmlist = db . Query<BankAccountViewModel> ( "Select * From BankAccount order by CustNo" ) . ToList ( );
                        else
                            bvmlist = db . Query<BankAccountViewModel> ( SqlCommand ) . ToList ( );

                        if ( bvmlist . Count > 0 )
                        {
                            foreach ( var item in bvmlist )
                            {
                                bvmcollection . Add ( item );
                            }
                        }
                    }
                    catch ( Exception ex )
                    {
                        Debug. WriteLine ( $"SQL DAPPER error : {ex . Message}, {ex . Data}" );
                    }
                }
                if ( Notify )
                {
                    //collection = bvmcollection;
                    Application . Current . Dispatcher . Invoke ( ( ) =>
                       EventControl . TriggerBankDataLoaded ( null ,
                       new LoadedEventArgs
                       {
                           CallerType = Caller ,
                           CallerDb = Caller ,
                           DataSource = bvmcollection ,
                           RowCount = bvmcollection . Count
                       } )
                       );
                }
            } );
            return null;
        }
        #endregion BANKACCOUNT Data Access

        #region CUSTOMER Data Access
        public static ObservableCollection<CustomerViewModel> GetCustObsCollection ( ObservableCollection<CustomerViewModel> collection , string SqlCommand = "" , bool Notify = false , string Caller = "" )
        {
            //This is a clever variation that uses Dapper and LINQ to load Data via SQL directly into a List<>
            //that is then converted into the requested Obs.Collection directly within this method
            // sort of one stop shop !
            ObservableCollection<CustomerViewModel> cvmcollection = new ObservableCollection<CustomerViewModel> ( );
            cvmcollection = collection;
            List<CustomerViewModel> cvmlist = new List<CustomerViewModel> ( );
            string ConString = Flags . CurrentConnectionString;
            if ( ConString == "" )
            {
                GenericDbUtilities . CheckDbDomain ( "IAN1" );
                ConString = Flags . CurrentConnectionString;
                if ( ConString == "" )
                    MessageBox . Show (
                        $"The system could not find the Flags.CurrentConnectionString collection\nPlease ensure you have the FLAGS.CS file \nand/or the GENERICDBHANDLERS.CS in this project" ,
                        "Data Access Error" );
                return null;
            }
            Application . Current . Dispatcher . Invoke ( ( ) =>
            {
                using ( IDbConnection db = new SqlConnection ( ConString ) )
                {
                    try
                    {
                        if ( SqlCommand == "" )
                            cvmlist = db . Query<CustomerViewModel> ( "Select * From Customer order  by CustNo, BankNo" ) . ToList ( );
                        else
                            cvmlist = db . Query<CustomerViewModel> ( SqlCommand ) . ToList ( );

                        if ( cvmlist . Count > 0 )
                        {
                            foreach ( var item in cvmlist )
                            {
                                cvmcollection . Add ( item );
                            }
                        }
                    }
                    catch ( Exception ex )
                    {
                        Debug. WriteLine ( $"SQL DAPPER error : {ex . Message}, {ex . Data}" );
                    }
                }
                if ( Notify )
                {
                    collection = cvmcollection;
                    Application . Current . Dispatcher . Invoke ( ( ) =>
                       EventControl . TriggerCustDataLoaded ( null ,
                       new LoadedEventArgs
                       {
                           CallerType = Caller ,
                           CallerDb = Caller ,
                           DataSource = collection ,
                           RowCount = collection . Count
                       } )
                       );
                }
            } );
            return cvmcollection;
        }
        #endregion CUSTOMER Data Access

    }
}
