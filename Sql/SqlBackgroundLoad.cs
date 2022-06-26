using NewWpfDev. ViewModels;

using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . Data . SqlClient;
using System . Data;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows;
using System . ComponentModel;
using NewWpfDev. Views;
using NewWpfDev. Dapper;
using System . Diagnostics;

namespace NewWpfDev. SQL
{
	public class SqlBackgroundLoad : INotifyPropertyChanged
	{
		#region PropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged ( string propertyName )
		{
			if ( Flags . SqlBankActive == false )
				//				this . VerifyPropertyName ( propertyName );

				if ( this . PropertyChanged != null )
				{
					var e = new PropertyChangedEventArgs ( propertyName );
					this . PropertyChanged ( this , e );
				}
		}
        #endregion PropertyChanged


#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        async public Task <ObservableCollection<BankAccountViewModel>> LoadBackground_BankAsync (
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
            ObservableCollection<BankAccountViewModel> bvmcollection ,
            string SqlCommand = "" ,
            string DbNameToLoad = "" ,
            string Orderby = "" ,
            string Conditions = "" ,
            bool wantSort = false ,
            bool wantDictionary = false ,
            bool Notify = false ,
            string Caller = "" ,
            int [ ] args = null )
        {
            //string ConString = Flags . CurrentConnectionString;
            string ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
            string [ ] ValidFields =
            {
                "ID",
                "CUSTNO",
                "BANKNO",
                "ACTYPE",
                "INTRATE" ,
                "BALANCE" ,
                "ODATE" ,
                "CDATE"
                };
            string [ ] errorcolumns;
            int [ ] dummyargs = { 0 , 0 , 0 };

            // Use defaullt Db if none received frm caller
            if ( DbNameToLoad == "" )
                DbNameToLoad = "BankAccount";

            // Utility Support Methods to validate data
            if ( DapperSupport . ValidateSortConditionColumns ( ValidFields , "Bank" , Orderby , Conditions , out errorcolumns ) == false )
            {
                if ( Orderby . ToUpper ( ) . Contains ( errorcolumns [ 0 ] ) )
                {
                    MessageBox . Show ( $"BANKACCOUNT dB\nSorry, but an invalid Column name has been \nidentified in the Sorting Clause provided.\n\nThe Invalid Column identified was :\n{errorcolumns [ 0 ]}.\n\nTherefore No Sort will be performed for this Db" );
                    Orderby = "";
                }
                else if ( Conditions . ToUpper ( ) . Contains ( errorcolumns [ 0 ] ) )
                {
                    MessageBox . Show ( $"BANKACCOUNT dB\nSorry, but an invalid Match clause or Column Name \nhas been identified in the Data Selection Clause.\n\nThe Invalid item identified was :\n\t{errorcolumns [ 0 ]}\n\nTherefore No Data Matching will be performed for this Db" );
                    Conditions = "";
                }
                else
                {
                    MessageBox . Show ( $"BANKACCOUNT dB\nSorry, but the Loading of the BankAccount Db is being aborted due to \na non existent Column name.\nThe Invalid Column identified was :\n{errorcolumns [ 0 ]}" );
                    return null;
                }
            }
            //====================================================
            // Use standard ADO.Net to to load Bank data to run Stored Procedure
            //====================================================
            //BankAccountViewModel bvm = new BankAccountViewModel ( );
            //string Con = Flags . CurrentConnectionString;
            string Con = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
            SqlConnection sqlCon = null;

            // Works with default command 31/10/21
            // works with Records limited 31/10/21
            // works with Selection conditions limited 31/10/21
            // works with Sort conditions 31/10/21
            try
            {
                using ( sqlCon = new SqlConnection ( Con ) )
                {
                    SqlCommand sql_cmnd;
                    sqlCon . Open ( );
                    Orderby = Orderby . Contains ( "Order by" ) ? Orderby . Substring ( 9 ) : Orderby;
                    Conditions = Conditions . Contains ( "where " ) ? Conditions . Substring ( 6 ) : Conditions;
                    if ( SqlCommand != "" )
                        sql_cmnd = new SqlCommand ( SqlCommand , sqlCon );
                    else
                    {
                        sql_cmnd = new SqlCommand ( "dbo.spLoadBankAccountComplex " , sqlCon );
                        sql_cmnd . CommandType = CommandType . StoredProcedure;
                        // Now handle parameters
                        sql_cmnd . Parameters . AddWithValue ( "@Arg1" , SqlDbType . NVarChar ) . Value = DbNameToLoad;
                        if ( args == null )
                            args = dummyargs;
                        if ( args . Length > 0 )
                        {
                            if ( args [ 2 ] > 0 )
                            {
                                string limits = $" Top ({args [ 2 ]}) ";
                                sql_cmnd . Parameters . AddWithValue ( "@Arg2" , SqlDbType . NVarChar ) . Value = limits;
                            }
                        }
                        Orderby = Orderby . Contains ( "Order by" ) ? Orderby . Substring ( 9 ) : Orderby;
                        if ( Conditions != "" )
                            sql_cmnd . Parameters . AddWithValue ( "@Arg3" , SqlDbType . NVarChar ) . Value = Conditions;
                        if ( Orderby != "" )
                            sql_cmnd . Parameters . AddWithValue ( "@Arg4" , SqlDbType . NVarChar ) . Value = Orderby . Trim ( );
                    }
                    var sqlDr = sql_cmnd . ExecuteReader ( );
                    while ( sqlDr . Read ( ) )
                    {
                        BankAccountViewModel  bvm = new BankAccountViewModel ( );
                        bvm . Id = int . Parse ( sqlDr [ "ID" ] . ToString ( ) );
                        bvm . CustNo = sqlDr [ "CustNo" ] . ToString ( );
                        bvm . BankNo = sqlDr [ "BankNo" ] . ToString ( );
                        bvm . AcType = int . Parse ( sqlDr [ "ACTYPE" ] . ToString ( ) );
                        bvm . Balance = Decimal . Parse ( sqlDr [ "BALANCE" ] . ToString ( ) );
                        bvm . IntRate = Decimal . Parse ( sqlDr [ "INTRATE" ] . ToString ( ) );
                        bvm . ODate = DateTime . Parse ( sqlDr [ "ODATE" ] . ToString ( ) );
                        bvm . CDate = DateTime . Parse ( sqlDr [ "CDATE" ] . ToString ( ) );
                        bvmcollection . Add ( bvm );
                    }
                    sqlDr . Close ( );
                    Debug. WriteLine ( $"SQL DAPPER {DbNameToLoad}  loaded : {bvmcollection . Count} records successfuly" );
                }
                sqlCon . Close ( );
                if ( Notify )
                {
					Application . Current . Dispatcher . Invoke ( ( ) =>
					EventControl . TriggerBankDataLoaded ( null ,
                        new LoadedEventArgs
                        {
                            CallerType = "DAPPERSUPPORT" ,
                            CallerDb = Caller ,
                            DataSource = bvmcollection ,
                            RowCount = bvmcollection . Count
                        } )
					);
                    return bvmcollection;
                }
                else
                    return bvmcollection;
            }
            catch ( Exception ex )
            {
                Debug. WriteLine ( $"Sql Error, {ex . Message}, {ex . Data}" );
                //	MessageBox . Show ( $"Db Load Failed\n{ex . Message}, {ex . Data}" );
            }
            return null;
        }

        public static ObservableCollection<BankAccountViewModel> LoadBackground_Bank (
			ObservableCollection<BankAccountViewModel> bvmcollection ,
			string SqlCommand = "" ,
			string DbNameToLoad = "" ,
			string Orderby = "" ,
			string Conditions = "" ,
			bool wantSort = false ,
			bool wantDictionary = false ,
			bool Notify = false ,
			string Caller = "" ,
			int [ ] args = null )
		{
			//string ConString = Flags . CurrentConnectionString;
			//			string ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
			string[] ValidFields=
			{
				"ID",
				"CUSTNO",
				"BANKNO",
				"ACTYPE",
				"INTRATE" ,
				"BALANCE" ,
				"ODATE" ,
				"CDATE"
				};
			string[] errorcolumns;
			int [ ] dummyargs= {0,0,0};

			// Use defaullt Db if none received frm caller
			if ( DbNameToLoad == "" )
				DbNameToLoad = "BankAccount";

			// Utility Support Methods to validate data
			if ( DapperSupport . ValidateSortConditionColumns ( ValidFields , "Bank" , Orderby , Conditions , out errorcolumns ) == false )
			{
				if ( Orderby . ToUpper ( ) . Contains ( errorcolumns [ 0 ] ) )
				{
					MessageBox . Show ( $"BANKACCOUNT dB\nSorry, but an invalid Column name has been \nidentified in the Sorting Clause provided.\n\nThe Invalid Column identified was :\n{errorcolumns [ 0 ]}.\n\nTherefore No Sort will be performed for this Db" );
					Orderby = "";
				}
				else if ( Conditions . ToUpper ( ) . Contains ( errorcolumns [ 0 ] ) )
				{
					MessageBox . Show ( $"BANKACCOUNT dB\nSorry, but an invalid Match clause or Column Name \nhas been identified in the Data Selection Clause.\n\nThe Invalid item identified was :\n\t{errorcolumns [ 0 ]}\n\nTherefore No Data Matching will be performed for this Db" );
					Conditions = "";
				}
				else
				{
					MessageBox . Show ( $"BANKACCOUNT dB\nSorry, but the Loading of the BankAccount Db is being aborted due to \na non existent Column name.\nThe Invalid Column identified was :\n{errorcolumns [ 0 ]}" );
					return null;
				}
			}
			//====================================================
			// Use standard ADO.Net to to load Bank data to run Stored Procedure
			//====================================================
			BankAccountViewModel bvm= new BankAccountViewModel();
			//string Con= Flags . CurrentConnectionString;
			string Con = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
			SqlConnection sqlCon=null;

			// Works with default command 31/10/21
			// works with Records limited 31/10/21
			// works with Selection conditions limited 31/10/21
			// works with Sort conditions 31/10/21
			try
			{
				using ( sqlCon = new SqlConnection ( Con ) )
				{
					SqlCommand sql_cmnd;
					sqlCon . Open ( );
					Orderby = Orderby . Contains ( "Order by" ) ? Orderby . Substring ( 9 ) : Orderby;
					Conditions = Conditions . Contains ( "where " ) ? Conditions . Substring ( 6 ) : Conditions;
					if ( SqlCommand != "" )
						sql_cmnd = new SqlCommand ( SqlCommand , sqlCon );
					else
					{
						sql_cmnd = new SqlCommand ( "dbo.spLoadBankAccountComplex " , sqlCon );
						sql_cmnd . CommandType = CommandType . StoredProcedure;
						// Now handle parameters
						sql_cmnd . Parameters . AddWithValue ( "@Arg1" , SqlDbType . NVarChar ) . Value = DbNameToLoad;
						if ( args == null )
							args = dummyargs;
						if ( args . Length > 0 )
						{
							if ( args [ 2 ] > 0 )
							{
								string limits = $" Top ({args[2]}) ";
								sql_cmnd . Parameters . AddWithValue ( "@Arg2" , SqlDbType . NVarChar ) . Value = limits;
							}
						}
						Orderby = Orderby . Contains ( "Order by" ) ? Orderby . Substring ( 9 ) : Orderby;
						if ( Conditions != "" )
							sql_cmnd . Parameters . AddWithValue ( "@Arg3" , SqlDbType . NVarChar ) . Value = Conditions;
						if ( Orderby != "" )
							sql_cmnd . Parameters . AddWithValue ( "@Arg4" , SqlDbType . NVarChar ) . Value = Orderby . Trim ( );
					}
					var sqlDr = sql_cmnd . ExecuteReader ( );
					while ( sqlDr . Read ( ) )
					{
						bvm . Id = int . Parse ( sqlDr [ "ID" ] . ToString ( ) );
						bvm . CustNo = sqlDr [ "CustNo" ] . ToString ( );
						bvm . BankNo = sqlDr [ "BankNo" ] . ToString ( );
						bvm . AcType = int . Parse ( sqlDr [ "ACTYPE" ] . ToString ( ) );
						bvm . Balance = Decimal . Parse ( sqlDr [ "BALANCE" ] . ToString ( ) );
						bvm . IntRate = Decimal . Parse ( sqlDr [ "INTRATE" ] . ToString ( ) );
						bvm . ODate = DateTime . Parse ( sqlDr [ "ODATE" ] . ToString ( ) );
						bvm . CDate = DateTime . Parse ( sqlDr [ "CDATE" ] . ToString ( ) );
						bvmcollection . Add ( bvm );
						bvm = new BankAccountViewModel ( );
					}
					sqlDr . Close ( );
					Debug. WriteLine ( $"SQL DAPPER {DbNameToLoad}  loaded : {bvmcollection . Count} records successfuly" );
				}
				sqlCon . Close ( );
				if ( Notify )
				{
					Application . Current . Dispatcher . Invoke ( ( ) =>
					EventControl . TriggerBankDataLoaded ( null ,
						new LoadedEventArgs
						{
							CallerType = "DAPPERSUPPORT" ,
							CallerDb = Caller ,
							DataSource = bvmcollection ,
							RowCount = bvmcollection . Count
						} )
					);
					return bvmcollection;
				}
				else
					return bvmcollection;
			}
			catch ( Exception ex )
			{
				Debug. WriteLine ( $"Sql Error, {ex . Message}, {ex . Data}" );
			//	MessageBox . Show ( $"Db Load Failed\n{ex . Message}, {ex . Data}" );
			}
			return null;
		}

		public static ObservableCollection<CustomerViewModel> LoadBackground_Customer (
			ObservableCollection<CustomerViewModel> cvmcollection ,
			string SqlCommand = "" ,
			string DbNameToLoad = "" ,
			string Orderby = "" ,
			string Conditions = "" ,
			bool wantSort = false ,
			bool wantDictionary = false ,
			bool Notify = false ,
			string Caller = "" ,
			int [ ] args = null
			)
		{
			int[] dummyargs = {0,0,0,0 };
			//			IEnumerable<CustomerViewModel> cvm ;
			string[] ValidFields=
			{
				"ID",
				"CUSTNO",
				"BANKNO",
				"ACTYPE",
				"FNAME" ,
				"LNAME" ,
				"ADDR1" ,
				"ADDR2" ,
				"TOWN" ,
				"COUNTY",
				"PCODE" ,
				"PHONE" ,
				"MOBILE",
				"ODATE" ,
				"CDATE"
				};
			string[] errorcolumns;
			//string ConString = Flags . CurrentConnectionString;
			string ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];

			if ( DbNameToLoad == "" )
				DbNameToLoad = "Customer";

			// Utility Support Methods to validate data
			if ( DapperSupport . ValidateSortConditionColumns ( ValidFields , "Bank" , Orderby , Conditions , out errorcolumns ) == false )
			{
				if ( Orderby . ToUpper ( ) . Contains ( errorcolumns [ 0 ] ) )
				{
					MessageBox . Show ( $"CUSTOMER dB\nSorry, but an invalid Column name has been \nidentified in the Sorting Clause provided.\n\nThe Invalid Column identified was :\n{errorcolumns [ 0 ]}.\n\nTherefore No Sort will be performed for this Db" );
					Orderby = "";
				}
				else if ( Conditions . ToUpper ( ) . Contains ( errorcolumns [ 0 ] ) )
				{
					MessageBox . Show ( $"CUSTOMER dB\nSorry, but an invalid Match clause or Column Name \nhas been identified in the Data Selection Clause.\n\nThe Invalid item identified was :\n\t{errorcolumns [ 0 ]}\n\nTherefore No Data Matching will be performed for this Db" );
					Conditions = "";
				}
				else
				{
					MessageBox . Show ( $"CUSTOMER dB\nSorry, but the Loading of the BankAccount Db is being aborted due to \na non existent Column name.\nThe Invalid Column identified was :\n{errorcolumns [ 0 ]}" );
					return null;
				}
			}

			//====================================================
			// Use standard ADO.Net to to load Bank data to run Stored Procedure
			//====================================================
			CustomerViewModel cvmi = new CustomerViewModel ( );
			//string Con= Flags . CurrentConnectionString;
			string Con = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
			SqlConnection sqlCon=null;
			Orderby = Orderby != "" ? Orderby: "";
			Conditions = Conditions . Contains ( "where " ) ? Conditions . Substring ( 6 ) : Conditions;

			// Works with default command 31/10/21
			// works with Records limited 31/10/21
			// works with Selection conditions limited 31/10/21
			// works with Sort conditions 31/10/21
			try
			{
				using ( sqlCon = new SqlConnection ( Con ) )
				{
					SqlCommand sql_cmnd;
					sqlCon . Open ( );
					if ( SqlCommand != "" )
						sql_cmnd = new SqlCommand ( SqlCommand , sqlCon );
					else
					{
						sql_cmnd = new SqlCommand ( "dbo.spLoadCustomersComplex " , sqlCon );
						sql_cmnd . CommandType = CommandType . StoredProcedure;
						sql_cmnd . Parameters . AddWithValue ( "@Arg1" , SqlDbType . NVarChar ) . Value = DbNameToLoad;
						if ( args == null )
							args = dummyargs;
						if ( args . Length > 0 )
						{
							if ( args [ 2 ] > 0 )
							{
								string limits = $" Top ({args[2]}) ";
								sql_cmnd . Parameters . AddWithValue ( "@Arg2" , SqlDbType . NVarChar ) . Value = limits;
							}
							//else
							//	sql_cmnd . Parameters . AddWithValue ( "@Arg2" , SqlDbType . NVarChar ) . Value = " * ";
						}
						//else
						//	sql_cmnd . Parameters . AddWithValue ( "@Arg2" , SqlDbType . NVarChar ) . Value = " * ";
						sql_cmnd . Parameters . AddWithValue ( "@Arg3" , SqlDbType . NVarChar ) . Value = Conditions;
						sql_cmnd . Parameters . AddWithValue ( "@Arg4" , SqlDbType . NVarChar ) . Value = Orderby;
					}
					// Handle  max records, if any
					var sqlDr = sql_cmnd . ExecuteReader ( );
					while ( sqlDr . Read ( ) )
					{
						cvmi . Id = int . Parse ( sqlDr [ "ID" ] . ToString ( ) );
						cvmi . CustNo = sqlDr [ "CUSTNO" ] . ToString ( );
						cvmi . BankNo = sqlDr [ "BANKNO" ] . ToString ( );
						cvmi . AcType = int . Parse ( sqlDr [ "ACTYPE" ] . ToString ( ) );
						cvmi . FName = sqlDr [ "FNAME" ] . ToString ( );
						cvmi . LName = sqlDr [ "LNAME" ] . ToString ( );
						cvmi . Addr1 = sqlDr [ "ADDR1" ] . ToString ( );
						cvmi . Addr2 = sqlDr [ "ADDR2" ] . ToString ( );
						cvmi . Town = sqlDr [ "TOWN" ] . ToString ( );
						cvmi . County = sqlDr [ "COUNTY" ] . ToString ( );
						cvmi . PCode = sqlDr [ "PCODE" ] . ToString ( );
						cvmi . Phone = sqlDr [ "PHONE" ] . ToString ( );
						cvmi . Mobile = sqlDr [ "MOBILE" ] . ToString ( );
						cvmi . ODate = DateTime . Parse ( sqlDr [ "ODATE" ] . ToString ( ) );
						cvmi . CDate = DateTime . Parse ( sqlDr [ "CDATE" ] . ToString ( ) );
						cvmcollection . Add ( cvmi );
						cvmi = new CustomerViewModel ( );
					}
					sqlDr . Close ( );
					Debug. WriteLine ( $"SQL DAPPER {DbNameToLoad}  loaded : {cvmcollection . Count} records successfuly" );
				}
				sqlCon . Close ( );
				return cvmcollection;
			}
			catch ( Exception ex )
			{
				Debug. WriteLine ( $"Sql Error, {ex . Message}, {ex . Data}" );
			}
			return null;
		}


		public static ObservableCollection<DetailsViewModel> LoadBackground_Details (
			ObservableCollection<DetailsViewModel> dvmcollection ,
			string SqlCommand = "" ,
			string DbNameToLoad = "" ,
			string Orderby = "" ,
			string Conditions = "" ,
			bool wantSort = false ,
			bool wantDictionary = false ,
			bool Notify = false ,
			string Caller = "" ,
			int [ ] args = null )
		{

			int[] dummyargs = {0,0,0,0};
#pragma warning disable CS0168 // The variable 'dvm' is declared but never used
			IEnumerable<DetailsViewModel> dvm ;
#pragma warning restore CS0168 // The variable 'dvm' is declared but never used
			//string ConString = Flags . CurrentConnectionString;
			string ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];

			string[] ValidFields=
			{
				"ID",
				"CUSTNO",
				"BANKNO",
				"ACTYPE",
				"INTRATE" ,
				"BALANCE" ,
				"ODATE" ,
				"CDATE"
				};
			string[] errorcolumns;

			// Use defaullt Db if none received frm caller
			if ( DbNameToLoad == "" )
				DbNameToLoad = "SecAccounts";


			// Utility Support Methods to validate data
			if ( DapperSupport . ValidateSortConditionColumns ( ValidFields , "Bank" , Orderby , Conditions , out errorcolumns ) == false )
			{
				if ( Orderby . ToUpper ( ) . Contains ( errorcolumns [ 0 ] ) )
				{
					MessageBox . Show ( $"DETAILS  dB\nSorry, but an invalid Column name has been \nidentified in the Sorting Clause provided.\n\nThe Invalid Column identified was :\n{errorcolumns [ 0 ]}.\n\nTherefore No Sort will be performed for this Db" );
					Orderby = "";
				}
				else if ( Conditions . ToUpper ( ) . Contains ( errorcolumns [ 0 ] ) )
				{
					MessageBox . Show ( $"DETAILS dB\nSorry, but an invalid Match clause or Column Name \nhas been identified in the Data Selection Clause.\n\nThe Invalid item identified was :\n\t{errorcolumns [ 0 ]}\n\nTherefore No Data Matching will be performed for this Db" );
					Conditions = "";
				}
				else
				{
					MessageBox . Show ( $"DETAILS dB\nSorry, but the Loading of the BankAccount Db is being aborted due to \na non existent Column name.\nThe Invalid Column identified was :\n{errorcolumns [ 0 ]}" );
					return null;
				}
			}
			if ( DbNameToLoad == "" )
				DbNameToLoad = "SecAccounts";

			Orderby = Orderby . Contains ( "Order by" ) ? Orderby . Substring ( 9 ) : Orderby;
			Conditions = Conditions . Contains ( "where " ) ? Conditions . Substring ( 6 ) : Conditions;
			//====================================================
			// Use standard ADO.Net to to load Bank data to run Stored Procedure
			//====================================================
			DetailsViewModel dvmi = new DetailsViewModel();
			//string Con= Flags . CurrentConnectionString;
			string Con = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
			SqlConnection sqlCon=null;

			// Works with default command 31/10/21
			// works with Records limited 31/10/21
			// works with Selection conditions limited 31/10/21
			// works with Sort conditions 31/10/21
			try
			{
				using ( sqlCon = new SqlConnection ( Con ) )
				{
					SqlCommand sql_cmnd;
					sqlCon . Open ( );
					if ( SqlCommand != "" )
						sql_cmnd = new SqlCommand ( SqlCommand , sqlCon );
					else
					{
						sql_cmnd = new SqlCommand ( "dbo.spLoadDetailsComplex " , sqlCon );
						sql_cmnd . CommandType = CommandType . StoredProcedure;
						sql_cmnd . Parameters . AddWithValue ( "@Arg1" , SqlDbType . NVarChar ) . Value = DbNameToLoad;
						if ( args == null )
							args = dummyargs;
						if ( args . Length > 0 )
						{
							if ( args [ 2 ] > 0 )
							{
								string limits = $" Top ({args[2]}) ";
								sql_cmnd . Parameters . AddWithValue ( "@Arg2" , SqlDbType . NVarChar ) . Value = limits;
							}
							else
								sql_cmnd . Parameters . AddWithValue ( "@Arg2" , SqlDbType . NVarChar ) . Value = " * ";
						}
						else
							sql_cmnd . Parameters . AddWithValue ( "@Arg2" , SqlDbType . NVarChar ) . Value = " * ";

						sql_cmnd . Parameters . AddWithValue ( "@Arg3" , SqlDbType . NVarChar ) . Value = Conditions;
						sql_cmnd . Parameters . AddWithValue ( "@Arg4" , SqlDbType . NVarChar ) . Value = Orderby;
					}
					// Handle  max records, if any
					var sqlDr = sql_cmnd . ExecuteReader ( );
					while ( sqlDr . Read ( ) )
					{
						dvmi . Id = int . Parse ( sqlDr [ "ID" ] . ToString ( ) );
						dvmi . CustNo = sqlDr [ "CustNo" ] . ToString ( );
						dvmi . BankNo = sqlDr [ "BankNo" ] . ToString ( );
						dvmi . AcType = int . Parse ( sqlDr [ "ACTYPE" ] . ToString ( ) );
						dvmi . Balance = Decimal . Parse ( sqlDr [ "BALANCE" ] . ToString ( ) );
						dvmi . IntRate = Decimal . Parse ( sqlDr [ "INTRATE" ] . ToString ( ) );
						dvmi . ODate = DateTime . Parse ( sqlDr [ "ODATE" ] . ToString ( ) );
						dvmi . CDate = DateTime . Parse ( sqlDr [ "CDATE" ] . ToString ( ) );
						dvmcollection . Add ( dvmi );
						dvmi = new DetailsViewModel ( );
					}
					sqlDr . Close ( );
					Debug. WriteLine ( $"SQL DAPPER {DbNameToLoad}  loaded : {dvmcollection . Count} records successfuly" );
				}
				sqlCon . Close ( );
				return dvmcollection;
			}
			catch ( Exception ex )

			{
				Debug. WriteLine ( $"Sql Error, {ex . Message}, {ex . Data}" );
			}
			return null;
		}

	}
}
