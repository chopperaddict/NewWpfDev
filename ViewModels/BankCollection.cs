//using NewWpfDev. Properties;
using NewWpfDev. ViewModels;

using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . Data . SqlClient;
using System . Data;
using System . Diagnostics;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows;
using NewWpfDev. Views;
using NewWpfDev. Models;

namespace NewWpfDev. ViewModels
{
    // This is basically a wrapper that provides an Observablecollection<BankAccountViewModel> data set
    // To call it - use the Method :
    //async static Task<BankCollection> LoadBank ( BankCollection cc , string caller = "" , int ViewerType = 1 , bool NotifyAll = false )
    public class BankCollection : ObservableCollection<BankAccountViewModel>
    {
        //		//Declare a global pointer to Observable BankAccount Collection
        //		public static BankCollection Bankinternalcollection = new BankCollection ( );
        public static BankCollection temp
        {
            get; set;
        }
        private static ObservableCollection<BankAccountViewModel> collection
        {
            get; set;
        }
        private static DataTable dtBank = new DataTable("BankDataTable");
        private static bool USEFULLTASK = true;
        private static bool Notify { get; set; } = false;
        private static string Caller = "";

        #region (dummy) CONSTRUCTOR

        public BankCollection () : base()
        {
        }
        #endregion

        #region LoadBank()

        //*************************************//
        // MAIN LOADING METHOD CALL (wrappper)
        //*************************************//
        //public static BankCollection LoadBank ( BankCollection cc, string caller, int ViewerType = 1, bool NotifyAll = false )
        async public static Task<ObservableCollection<BankAccountViewModel>> LoadBank (ObservableCollection<BankAccountViewModel> bcollection , string caller = "" , int ViewerType = 1 , bool NotifyAll = false)
        {
            BankCollection . Notify = NotifyAll;
            BankCollection . Caller = caller;
            try
            {
                if ( USEFULLTASK )
                {
                    //Bankinternalcollection . Clear ( );
                    //Bankinternalcollection = new BankCollection ( );
                    ObservableCollection<BankAccountViewModel> collection = new ObservableCollection<BankAccountViewModel>();
                    bcollection = LoadBankTaskInSortOrderasync(collection , caller , Notify);
                    //bcollection = Bankinternalcollection;
                }
                bcollection = collection;
                return bcollection;
            }
            catch ( Exception ex )
            {
                Debug . WriteLine($"Bank Load Exception : {ex . Message}, {ex . Data}");
                return null;
            }
        }
        #endregion load  bank

        public static ObservableCollection<BankAccountViewModel> LoadBankTaskInSortOrderasync (ObservableCollection<BankAccountViewModel> bcollection , string Caller , bool notify , int i = -1)
        {
            if ( dtBank . Rows . Count > 0 )
                dtBank . Clear();

            if ( bcollection . Count > 0 )
                bcollection . Clear();
            #region process code to load data

            dtBank = LoadBankData();
            bcollection = LoadBankCollection(bcollection , notify , Caller , i);

            #endregion process code to load data

            // Finally fill and return The global Dataset
            return bcollection; ;
        }

        #region LOAD THE DATA

        /// Handles the actual conneciton ot SQL to load  Bank Db data required
        /// </summary>
        /// <returns></returns>
        public static DataTable LoadBankData (int max = 0 , string caller = "" , bool isMultiMode = false)
        {
            object bptr = new object();
            try
            {
                SqlConnection con;
                string ConString = "";
                ConString = Flags . CurrentConnectionString;
                ConString = ( string )Properties . Settings . Default [ "BankSysConnectionString" ];
                con = new SqlConnection(ConString);
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

                        //						commandline = WpfLib1 . Utils .GetDataSortOrder ( commandline );
                    }
                    else if ( Flags . FilterCommand != "" )
                    {
                        commandline = Flags . FilterCommand;
                    }
                    else if ( max != -1 )
                    {
                        commandline = $"Select Top ({max})  * from BankAccount order by Id";
                    }
                    else
                    {
                        // Create a valid Query Command string including any active sort ordering
                        commandline = "Select * from [BankAccount] order by ";
                        commandline = Utils .GetDataSortOrder(commandline);
                    }
                    SqlCommand cmd = new SqlCommand(commandline , con);
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    if ( dtBank == null )
                        dtBank = new DataTable();
                    sda . Fill(dtBank);
                }
            }
            catch ( Exception ex )
            {
                Debug . WriteLine($"Failed to load Bank Details - {ex . Message}, {ex . Data}");
                return null;
            }
            return dtBank;
        }

        // Main Data load method that handles trigger notification
        public static bool LoadBCollection (ObservableCollection<BankAccountViewModel> bcollection , DataTable dtBank)
        {
            int count = 0;
            try
            {
                object bptr = new object();
                for ( int i = 0 ; i < dtBank . Rows . Count ; i++ )
                {
                    bcollection . Add(new BankAccountViewModel
                    {
                        Id = Convert . ToInt32(dtBank . Rows [ i ] [ 0 ]) ,
                        BankNo = dtBank . Rows [ i ] [ 1 ] . ToString() ,
                        CustNo = dtBank . Rows [ i ] [ 2 ] . ToString() ,
                        AcType = Convert . ToInt32(dtBank . Rows [ i ] [ 3 ]) ,
                        Balance = Convert . ToDecimal(dtBank . Rows [ i ] [ 4 ]) ,
                        IntRate = Convert . ToDecimal(dtBank . Rows [ i ] [ 5 ]) ,
                        ODate = Convert . ToDateTime(dtBank . Rows [ i ] [ 6 ]) ,
                        CDate = Convert . ToDateTime(dtBank . Rows [ i ] [ 7 ]) ,
                    });
                    count = i;
                }
            }
            catch ( Exception ex )
            {
                Debug . WriteLine($"BANK : SQL Error in BankCollection(351) load function : {ex . Message}, {ex . Data}, count was {count}");
                //MessageBox . Show ( $"BANK : SQL Error in BankCollection (351) load function : {ex . Message}, {ex . Data}" );
            }
            finally
            {
                // This is ONLY called  if a requestor specifies the file wide variable argument as TRUE
                if ( BankCollection . Notify )
                {
                    Application . Current . Dispatcher . Invoke(() =>
                    EventControl . TriggerBankDataLoaded(null ,
                        new LoadedEventArgs
                        {
                            CallerType = BankCollection . Caller ,
                            CallerDb = Caller ,
                            DataSource = bcollection ,
                            RowCount = bcollection . Count
                        }));
                }
            }
            return true;
        }

        // MAIN CALLED METHOD - Redirector (wrapper method) that calls main method, but with far fewer parameters required
        // Returns colection in both  cases of notification requested
        public static ObservableCollection<BankAccountViewModel> LoadBankCollection (ObservableCollection<BankAccountViewModel> bcollection , bool NotifyAll , string caller , int max = -1)
        {
            BankCollection . Notify = NotifyAll;
            BankCollection . Caller = caller;
            bcollection = new ObservableCollection<BankAccountViewModel>();
            dtBank . Clear();

            try
            {
                dtBank = LoadBankData(max);
                collection = new ObservableCollection<BankAccountViewModel>();
                LoadBCollection(bcollection , dtBank);
                if ( BankCollection . Notify == false )
                    return bcollection;
            }
            catch ( Exception ex )
            {
                Debug. WriteLine($"Bank loading failed... : {ex . Message} : {ex . Data}");
            }
            return bcollection;
        }



        #region Specialist Load methods
        public static DataTable LoadSelectedBankData (string seltype = "" , int Min = -1 , int Max = -1 , int Tot = -1)
        {
            DataTable dt = new DataTable();
            SqlConnection con;
            string ConString = "";
            string commandline = "";
            string selection = seltype . ToUpper();
            ConString = Flags . CurrentConnectionString;
            if ( ConString == "" )
            {
                GenericDbUtilities . CheckDbDomain("IAN1");
                ConString = Flags . CurrentConnectionString;
            }
            ConString = ( string )Properties . Settings . Default [ "BankSysConnectionString" ];
            Debug . WriteLine($"Making new SQL connection in BANKCOLLECTION");
            con = new SqlConnection(ConString);
            try
            {
                Debug . WriteLine($"Using new SQL connection in BANKCOLLECTION");
                using ( con )
                {
                    if ( Min == -1 && Max == -1 && Tot != -1 )
                    {
                        // Create a valid Query Command string for a Maximum  # of records
                        commandline = $"Select Top {Tot} Id, BankNo, CustNo, AcType, Balance, IntRate, ODate, CDate  from BankAccount  " +
                            $"  order by CustNo + BankNo ";
                    }
                    else if ( Min == -1 && Max == -1 && Tot == -1 )
                    {
                        // Create a valid Query Command string for ALL records
                        commandline = $"Select  * from BankAccount  order by CustNo + BankNo ";
                    }
                    else
                    {
                        // Create a valid Query Command string for a Max # of records with start/End parameters for CustNo
                        if ( selection == "" || selection == "CUSTNO" )
                            commandline = $"Select Top {Tot} Id, BankNo, CustNo, AcType, Balance, IntRate, ODate, CDate  from BankAccount  " +
                                $" where CustNo > {Min} AND CustNo < {Max} order by CustNo + BankNo ";
                        else if ( selection == "BANKNO" )
                            commandline = $"Select Top {Tot} Id, BankNo, CustNo, AcType, Balance, IntRate, ODate, CDate  from BankAccount  " +
                                $" where BankNo > {Min} AND BankjNo < {Max} order by CustNo + BankNo ";
                        else if ( selection == "ACTYPE" )
                            commandline = $"Select Top {Tot} Id, BankNo, CustNo, AcType, Balance, IntRate, ODate, CDate  from BankAccount  " +
                                $" where AcType > {Min} AND AcType < {Max} order by CustNo + BankNo ";
                        else if ( selection == "INTRATE" )
                            commandline = $"Select Top {Tot} Id, BankNo, CustNo, AcType, Balance, IntRate, ODate, CDate  from BankAccount  " +
                                $" where IntRate > {Min} AND IntRate < {Max} order by CustNo + BankNo ";
                        else if ( selection == "BALANCE" )
                            commandline = $"Select Top {Tot} Id, BankNo, CustNo, AcType, Balance, IntRate, ODate, CDate  from BankAccount  " +
                                $" where Balance > {Min} AND Balance < {Max} order by CustNo + BankNo ";

                    }
                    SqlCommand cmd = new SqlCommand(commandline , con);
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    sda . Fill(dt);
                }
            }
            catch ( Exception ex )
            {
                Debug . WriteLine($"BANKACCOUNT : ERROR in LoadBankDataSql(): Failed to load Bank Details - {ex . Message}, {ex . Data}");
                MessageBox . Show($"BANKACCOUNT: ERROR in LoadBankDataSql(): Failed to load Bank Details - {ex . Message}, {ex . Data}");
            }
            finally
            {
                con . Close();
            }
            return dt;
        }

        /// <summary>
        /// A specialist version  to reload data from a provided DataTable
        ///  July 2021
        /// </summary>
        /// <returns></returns>
        public static BankCollection LoadSelectedCollection (BankCollection bankCollection , int max = -1 , DataTable dtBank = null , bool Notify = false)
        {
            int count = 0;
            try
            {
                //if max is not Received, set to total records in DtBank
                //				if ( max == -1 )
                //max = dtBank . Rows . Count;
                for ( int i = 0 ; i < dtBank . Rows . Count ; i++ )
                {
                    temp . Add(new BankAccountViewModel
                    {
                        Id = Convert . ToInt32(dtBank . Rows [ i ] [ 0 ]) ,
                        BankNo = dtBank . Rows [ i ] [ 1 ] . ToString() ,
                        CustNo = dtBank . Rows [ i ] [ 2 ] . ToString() ,
                        AcType = Convert . ToInt32(dtBank . Rows [ i ] [ 3 ]) ,
                        Balance = Convert . ToDecimal(dtBank . Rows [ i ] [ 4 ]) ,
                        IntRate = Convert . ToDecimal(dtBank . Rows [ i ] [ 5 ]) ,
                        ODate = Convert . ToDateTime(dtBank . Rows [ i ] [ 6 ]) ,
                        CDate = Convert . ToDateTime(dtBank . Rows [ i ] [ 7 ]) ,
                    });
                    count++;
                }

            }
            catch ( Exception ex )
            {
                Debug . WriteLine($"BANK : SQL Error in BankCollection load function (count = {count}) : {ex . Message}, {ex . Data}");
                //MessageBox . Show ( $"BANK : SQL Error in BankCollection load function : {ex . Message}, {ex . Data}" );
            }
            finally
            {
                //				Debug . WriteLine ( $"BANK : Completed load into Bankcollection :  {temp . Count} records loaded successfully ...." );
                // This is ONLY called  if a requestor specifies the argument as TRUE
                if ( Notify )
                {
                    Application . Current . Dispatcher . Invoke(() =>
                    EventControl . TriggerBankDataLoaded(null ,
                        new LoadedEventArgs
                        {
                            CallerType = "SELECTEDDATA" ,
                            CallerDb = Caller ,
                            DataSource = temp ,
                            RowCount = temp . Count
                        })
                    );
                    //					Debug . WriteLine ( $"DEBUG : In BankCollection : Sending  BankDataLoaded EVENT trigger" );
                }
            }
            if ( Notify == false )
                return temp;
            else
                return null;
        }
        #endregion Specialist Load methods

        #endregion LOAD THE DATA

        //public void ListBankInfo ( KeyboardDelegate KeyBoardDelegate )
        //{
        //	// Run a specified delegate sent by SqlDbViewer
        //	KeyBoardDelegate ( 1 );
        //}
        public static bool UpdateBankDb (BankAccountViewModel NewData , string CallerType)
        {
            SqlConnection con;
            SqlCommand cmd = null;
            string ConString = "";
            ConString = Flags . CurrentConnectionString;
            if ( ConString == "" )
            {
                GenericDbUtilities . CheckDbDomain("IAN1");
                ConString = Flags . CurrentConnectionString;
            }
            con = new SqlConnection(ConString);
            try
            {
                using ( con )
                {
                    con . Open();
                    if ( CallerType == "CUSTOMER" )
                    {
                        cmd = new SqlCommand("UPDATE BankAccount SET BANKNO=@bankno, CUSTNO=@custno, ACTYPE=@actype, " +
                            "ODATE=@odate, CDATE=@cdate where CUSTNO = @custno" , con);
                        cmd . Parameters . AddWithValue("@id" , Convert . ToInt32(NewData . Id));
                        cmd . Parameters . AddWithValue("@bankno" , NewData . BankNo . ToString());
                        cmd . Parameters . AddWithValue("@custno" , NewData . CustNo . ToString());
                        cmd . Parameters . AddWithValue("@actype" , Convert . ToInt32(NewData . AcType));
                        cmd . Parameters . AddWithValue("@odate" , Convert . ToDateTime(NewData . ODate));
                        cmd . Parameters . AddWithValue("@cdate" , Convert . ToDateTime(NewData . CDate));
                    }
                    else
                    {
                        cmd = new SqlCommand("UPDATE BankAccount SET BANKNO=@bankno, CUSTNO=@custno, ACTYPE=@actype, " +
                            "BALANCE=@balance, INTRATE=@intrate, ODATE=@odate, CDATE=@cdate where CUSTNO = @custno" , con);
                        cmd . Parameters . AddWithValue("@id" , Convert . ToInt32(NewData . Id));
                        cmd . Parameters . AddWithValue("@bankno" , NewData . BankNo . ToString());
                        cmd . Parameters . AddWithValue("@custno" , NewData . CustNo . ToString());
                        cmd . Parameters . AddWithValue("@actype" , Convert . ToInt32(NewData . AcType));
                        cmd . Parameters . AddWithValue("@balance" , Convert . ToDecimal(NewData . Balance));
                        cmd . Parameters . AddWithValue("@intrate" , Convert . ToDecimal(NewData . IntRate));
                        cmd . Parameters . AddWithValue("@odate" , Convert . ToDateTime(NewData . ODate));
                        cmd . Parameters . AddWithValue("@cdate" , Convert . ToDateTime(NewData . CDate));
                    }
                    cmd . ExecuteNonQuery();
                    Debug . WriteLine("SQL Update of All Db's successful...");
                }
            }
            catch ( Exception ex )
            {
                Debug. WriteLine($"BANKACCOUNT Update FAILED : {ex . Message}, {ex . Data}");
            }
            finally
            {
                con . Close();
            }
            return true;
        }

        #region update functionality

        /// <summary>
        /// Called to allow any method to load FULL Bank data directly 
        /// it returns a populated DataTable  that the caller can then "Load" by  calling LoadBankCollectionDirect ()
        /// </summary>
        /// <param name="dtBank"></param>
        /// <param name="Sqlcommand">Can be a fully qualified SQLcommand line</param>
        /// <returns></returns>
        public static DataTable LoadBankDirect (DataTable dt , string Sqlcommand = "" , int Max = -1)
        {
            SqlConnection con;
            string ConString = "";
            string commandline = "";
            dt . Clear();
            string sqlcommand = Sqlcommand;
            if ( sqlcommand == "" )
            {
                sqlcommand = "Select * from BankAccount ";
                if ( Max > 0 )
                    sqlcommand = $"Select top ({Max}) * from BankAccount ";
            }
            try
            {
                ConString = Flags . CurrentConnectionString;
                if ( ConString == "" )
                {
                    GenericDbUtilities . CheckDbDomain("IAN1");
                    ConString = Flags . CurrentConnectionString;
                }
                ConString = ( string )Properties . Settings . Default [ "BankSysConnectionString" ];
                Debug . WriteLine($"Making new SQL connection in BANKCOLLECTION");
                con = new SqlConnection(ConString);
                using ( con )
                {
                    Debug . WriteLine($"Using new SQL connection in BANKCOLLECTION");
                    //					if ( Sqlcommand != "" )
                    commandline = sqlcommand;
                    //					else
                    //						commandline = "Select * from BankAccount order by CustNo, BankNo";

                    SqlCommand cmd = new SqlCommand(commandline , con);
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    if ( dt == null )
                        dt = new DataTable();
                    sda . Fill(dt);
                }
            }
            catch ( Exception ex )
            {
                Debug . WriteLine($"Failed to load Bank Details - {ex . Message}, {ex . Data}");
                return null;
            }
            return dt;

        }
        /// <summary>
        /// A specialist version  to reload data WITHOUT changing global version
        /// </summary>
        /// <returns></returns>
        public static BankCollection LoadBankCollectionDirect (BankCollection temp , DataTable dtBank)
        {
            try
            {
                for ( int i = 0 ; i < dtBank . Rows . Count ; i++ )
                {
                    temp . Add(new BankAccountViewModel
                    {
                        Id = Convert . ToInt32(dtBank . Rows [ i ] [ 0 ]) ,
                        BankNo = dtBank . Rows [ i ] [ 1 ] . ToString() ,
                        CustNo = dtBank . Rows [ i ] [ 2 ] . ToString() ,
                        AcType = Convert . ToInt32(dtBank . Rows [ i ] [ 3 ]) ,
                        Balance = Convert . ToDecimal(dtBank . Rows [ i ] [ 4 ]) ,
                        IntRate = Convert . ToDecimal(dtBank . Rows [ i ] [ 5 ]) ,
                        ODate = Convert . ToDateTime(dtBank . Rows [ i ] [ 6 ]) ,
                        CDate = Convert . ToDateTime(dtBank . Rows [ i ] [ 7 ]) ,
                    });
                }
            }
            catch ( Exception ex )
            {
                Debug . WriteLine($"BANK : SQL Error in BankCollection load function : {ex . Message}, {ex . Data}");
                MessageBox . Show($"BANK : SQL Error in BankCollection load function : {ex . Message}, {ex . Data}");
            }
            finally
            {
                Debug . WriteLine($"BANK : Completed load into Bankcollection :  {temp . Count} records loaded successfully ....");
            }
            return temp;
        }

        #endregion update functionality


        #region EXPORT FUNCTIONS TO READ/WRITE CSV files for BANKACCOUNT DB

        /// <summary>
        /// Load the data into a DataTable for our export functions below here
        /// </summary>
        /// <returns></returns>
        public static DataTable LoadBankExportData ()
        {
            DataTable dt = new DataTable();
            SqlConnection con;
            string ConString = "";
            string commandline = "";
            ConString = Flags . CurrentConnectionString;
            if ( ConString == "" )
            {
                GenericDbUtilities . CheckDbDomain("IAN1");
                ConString = Flags . CurrentConnectionString;
            }
            ConString = ( string )Properties . Settings . Default [ "BankSysConnectionString" ];
            Debug . WriteLine($"Making new SQL connection in BANKCOLLECTION");
            con = new SqlConnection(ConString);
            try
            {
                Debug . WriteLine($"Using new SQL connection in BANKCOLLECTION");
                using ( con )
                {
                    //if ( Flags . IsMultiMode )
                    //{
                    //	// Create a valid Query Command string including any active sort ordering
                    //	commandline = $"SELECT * FROM SECACCOUNTS WHERE CUSTNO IN "
                    //		+ $"(SELECT CUSTNO FROM SECACCOUNTS  "
                    //		+ $" GROUP BY CUSTNO"
                    //		+ $" HAVING COUNT(*) > 1) ORDER BY ";
                    //	commandline = WpfLib1 . Utils .GetDataSortOrder ( commandline );
                    //}
                    //else if ( Flags . FilterCommand != "" )
                    //{
                    //	commandline = Flags . FilterCommand;
                    //}
                    //else
                    {
                        // Create a valid Query Command string including any active sort ordering
                        commandline = "Select * from BankAccount  order by ";
                        //						commandline = WpfLib1 . Utils .GetDataSortOrder ( commandline );
                    }
                    SqlCommand cmd = new SqlCommand(commandline , con);
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    sda . Fill(dt);
                }
            }
            catch ( Exception ex )
            {
                Debug . WriteLine($"BANKACCOUNT : ERROR in LoadBankDataSql(): Failed to load Bank Details - {ex . Message}, {ex . Data}");
                MessageBox . Show($"BANKACCOUNT: ERROR in LoadBankDataSql(): Failed to load Bank Details - {ex . Message}, {ex . Data}");
            }
            finally
            {
                con . Close();
            }
            return dt;
        }

        /// <summary>
        /// Writes the data directly from the Db vias Sqluery first, then output to disk file in CSV format
        /// Working well 7/6/21
        /// </summary>
        /// <param name="path"></param>
        /// <param name="dbType"></param>
        public static int ExportBankData (string path , string dbType)
        {
            int count = 0;
            string output = "";

            // Read data in from disk first as a DataTable dt
            DataSet ds = new DataSet();

            DataTable dt = new DataTable();
            dt = LoadBankExportData();
            ds . Tables . Add(dt);
            //£££££££££££££££££££££££££££££££££££££££££££
            // This works just fine with no external binding.
            // The data is  now accessible in ds.Tables[0].Rows
            // NB DATA ACCESS FORMAT IS [ $"{objRow["CustNo"]}"  ]
            //££££££££££££££££££££££££££££££££££££££££££££
            Debug. WriteLine($"Writing results of SQL enquiry to {path} ...");
            foreach ( DataRow objRow in ds . Tables [ 0 ] . Rows )
            {
                output += ParseDbRow("BANKACCOUNT" , objRow);
                count++;
            }
            if ( path == "" )
                path = @"C:\Users\ianch\Documents\Bank";
            string savepath = WpfLib1 . Utils .GetExportFileName(path);

            System . IO . File . WriteAllText(savepath , output);
            Debug. WriteLine($"Export of {count - 1} records from the [ {dbType} ] Db has been saved to {path} successfully.");
            return count;
        }


        //===============================================================================
        /// <summary>
        /// Special method to check the data format we are going to write to the CSV file 
        /// and creates the output line by line from a datarow of the DataTable we have just read in
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="objRow"></param>
        /// <returns></returns>
        public static string ParseDbRow (string dbType , DataRow objRow)
        {
            string tmp = "", s = "";
            string [ ] odat, cdat, revstr;
            if ( dbType == "BANKACCOUNT" )
            {
                char [ ] ch = { ' ' };
                char [ ] ch2 = { '/' };
                s = $"{objRow [ "Odate" ] . ToString()}', '";
                odat = s . Split(ch);
                string odate = odat [ 0 ];
                // now reverse it  to YYYY/MM/DD format as this is what SQL understands
                revstr = odate . Split(ch2);
                odate = revstr [ 2 ] + "/" + revstr [ 1 ] + "/" + revstr [ 0 ];
                // thats  the Open date handled - now do close data
                s = $"{objRow [ "cDate" ] . ToString()}', '";
                cdat = s . Split(ch);   // split date on '/'
                string cdate = cdat [ 0 ];
                // now reverse it  to YYYY/MM/DD format as this is what SQL understands
                revstr = cdate . Split(ch2);
                cdate = revstr [ 2 ] + "/" + revstr [ 1 ] + "/" + revstr [ 0 ];
                string acTypestr = objRow [ "AcType" ] . ToString() . Trim();

                //Creates the correct format for the CSV fle output, including adding single quotes to DATE fields
                // Tested and working 7/6/21
                tmp = $"{objRow [ "Id" ] . ToString()}, "
                    + $"{objRow [ "BankNo" ] . ToString()}, "
                    + $"{objRow [ "CustNo" ] . ToString()}, "
                    + $"{acTypestr}, "
                    + $"{objRow [ "Balance" ] . ToString()}, "
                    + $"{objRow [ "Intrate" ] . ToString()}, "
                    + $"'{odate}', '"
                    + $"{cdate}'\r\n";
            }
            return tmp;
        }
        #endregion EXPORT FUNCTIONS TO READ/WRITE CSV files


    }
}