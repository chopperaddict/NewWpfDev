using Dapper;

using NewWpfDev . Dapper;
using NewWpfDev . ViewModels;
using NewWpfDev . Views;

using System;
using System . Collections;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . Data;
using System . Data . SqlClient;
using System . Diagnostics;
using System . Linq;
using System . Windows;
using System . Windows . Controls;

namespace NewWpfDev . SQL
{
    public class SqlSupport
	{
		//********************************************************************************************************************************************************************************//
		#region Wrapper methods to Fetch data & return an Observablecollection
		public static ObservableCollection<BankAccountViewModel> LoadBank ( string Sqlcommand , int max = 0 , bool Notify = false , bool isMultiMode = false )
		{
			DataTable dt = LoadBankData( Sqlcommand ,max, isMultiMode);
			ObservableCollection<BankAccountViewModel> bvm = LoadBankCollection ( dt , Notify);
			return bvm;
		}
		public static ObservableCollection<CustomerViewModel> LoadCustomer ( string Sqlcommand , int max = 0 , bool Notify = false , bool isMultiMode = false )
		{
			DataTable dt = LoadCustData( Sqlcommand ,max, isMultiMode);
			ObservableCollection< CustomerViewModel > cvm = LoadCustomerCollection ( dt , Notify);
			return cvm;
		}
		public static ObservableCollection<DetailsViewModel> LoadDetails ( string Sqlcommand , int max = 0 , bool Notify = false , bool isMultiMode = false )
		{
			DataTable dt = LoadDetailsData( Sqlcommand ,max, isMultiMode);
			ObservableCollection<DetailsViewModel> dvm = LoadDetailsCollection ( dt , Notify);
			return dvm;
		}
		/// <summary>
        /// loads table data by using an S.P
        /// </summary>
        /// <param name="Sqlcommand"></param>
        /// <param name="ResultString"></param>
        /// <param name="max"></param>
        /// <param name="Notify"></param>
        /// <param name="isMultiMode"></param>
        /// <returns></returns>
        public static ObservableCollection<GenericClass> LoadGeneric ( string Sqlcommand , out string ResultString , int max = 0 , bool Notify = false , bool isMultiMode = false )
		{
            string argument = "";
			ObservableCollection<GenericClass> generics = new ObservableCollection<GenericClass>();
            if(Sqlcommand.Contains(" "))
            {
                string [ ] args = Sqlcommand . Split ( " " );
                argument = args [ 1 ];
                Sqlcommand = args [ 0 ] . Trim ( );
            }
			ExecuteStoredProcedure ( Sqlcommand ,
			generics ,
			out ResultString ,
			"" ,
			argument ,
			null ,
			false );
            //No data available ...
//            if ( Notify )

            return generics;
		}
		#endregion Wrapper methods to Fetch data & load into collections

		//********************************************************************************************************************************************************************************//
		// DataTable loading methods
		#region Data loading to DataTable via Sql
		public static DataTable LoadBankData ( string Sqlcommand , int max = 0 , bool isMultiMode = false )
		//Load data from Sql Server
		{
			DataTable dtBank = new DataTable();
			try
			{
				SqlConnection con;
				string commandline="";
				string ConString = Flags . CurrentConnectionString;
				ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
				con = new SqlConnection ( ConString );
				using ( con )
				{
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
					else
					{
						// Create a valid Query Command string including any active sort ordering
						commandline = Sqlcommand;
					}
					SqlCommand cmd = new SqlCommand ( commandline, con );
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

		public static DataTable LoadCustData ( string Sqlcommand , int max = 0 , bool isMultiMode = false )
		//Load data from Sql Server
		{
			SqlConnection con;
			DataTable dtCust = new DataTable();
			string ConString = Flags . CurrentConnectionString;
			ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
			con = new SqlConnection ( ConString );
			try
			{
				using ( con )
				{
					string commandline = "";

					if ( Flags . IsMultiMode )
					{
						// Create a valid Query Command string including any active sort ordering
						commandline = $"SELECT * FROM CUSTOMER WHERE CUSTNO IN "
							  + $"(SELECT CUSTNO FROM CUSTOMER  "
							  + $" GROUP BY CUSTNO"
							  + $" HAVING COUNT(*) > 1) ORDER BY ";
					}
					else if ( Flags . FilterCommand != "" )
					{
						commandline = Flags . FilterCommand;
					}
					else
					{
						// Create a valid Query Command string including any active sort ordering
						if ( max == 0 ) //&& bottomrec == 0 && toprec == 0 )
						{
							commandline = Sqlcommand;
							//							commandline = WpfLib1 . Utils .GetDataSortOrder ( commandline );
						}
					}
					SqlCommand cmd = new SqlCommand ( commandline, con );
					SqlDataAdapter sda = new SqlDataAdapter ( cmd );
					sda . Fill ( dtCust );
				}
			}
			catch ( Exception ex )
			{
				Debug . WriteLine ( $"Failed to load Customer Details - {ex . Message}, {ex . Data}" );
				return dtCust;
			}
			finally
			{
				con . Close ( );
			}

			return dtCust;
		}

		public static DataTable LoadDetailsData ( string Sqlcommand , int max = 0 , bool isMultiMode = false )
		{
			SqlConnection con;
			string filterline = "";
			DataTable dtDetails = new DataTable();
			string ConString = Flags . CurrentConnectionString;
			ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];

			con = new SqlConnection ( ConString );
			try
			{
				Debug . WriteLine ( $"Using new SQL connection in DETAILSCOLLECTION" );
				using ( con )
				{
					if ( Flags . IsMultiMode )
					{
						// Create a valid Query Command string including any active sort ordering
						filterline = $"SELECT * FROM SECACCOUNTS WHERE CUSTNO IN "
							+ $"(SELECT CUSTNO FROM SECACCOUNTS  "
							+ $" GROUP BY CUSTNO"
							+ $" HAVING COUNT(*) > 1) ORDER BY ";
						//						filterline = WpfLib1 . Utils .GetDataSortOrder ( filterline );
					}
					else if ( Flags . FilterCommand != "" )
					{
						filterline = Flags . FilterCommand;
					}
					else
					{
						// Create a valid Query Command string including any active sort ordering
						filterline = Sqlcommand;
						//						filterline = WpfLib1 . Utils .GetDataSortOrder ( filterline );
					}
					SqlCommand cmd = new SqlCommand ( filterline , con );
					SqlDataAdapter sda = new SqlDataAdapter ( cmd );
					sda . Fill ( dtDetails );
				}
			}
			catch ( Exception ex )
			{
				Debug . WriteLine ( $"DETAILS : ERROR in LoadDetailsDataSql(): Failed to load Details Details - {ex . Message}, {ex . Data}" );
				MessageBox . Show ( $"DETAILS : ERROR in LoadDetailsDataSql(): Failed to load Details Details - {ex . Message}, {ex . Data}" );
			}
			finally
			{
					con . Close ( );
			}
			return dtDetails;
		}

		public static DataTable LoadGenericData ( string Sqlcommand , string DbName="", int max = 0 , bool isMultiMode = false )
		{
			SqlConnection con;
			string filterline = "";
			DataTable dtGeneric= new DataTable();

			// This resets the current database connection - should be used anywhere that We switch between databases in Sql Server
			if ( Utils .CheckResetDbConnection ( "IAN1" , out string constring ) == false )
			{
				Debug. WriteLine ( $"Failed to set connection string for {DbName . ToUpper ( )} Db" );
				return null;
			}
			filterline = Sqlcommand;
			string ConString = Flags . CurrentConnectionString;

			con = new SqlConnection ( ConString );
			try
			{
				Debug . WriteLine ( $"Using new SQL connection in LOADGENERICDATA" );
				using ( con )
				{
					SqlCommand cmd = new SqlCommand ( filterline , con );
					SqlDataAdapter sda = new SqlDataAdapter ( cmd );
					sda . Fill ( dtGeneric );
				}
			}
			catch ( Exception ex )
			{
				Debug . WriteLine ( $"GENERIC : ERROR in LoadGenericData(): Failed to load Generic Data :  {ex . Message}, {ex . Data}" );
				MessageBox . Show ( $"GENERIC: ERROR in LoadGenericData(): Failed to load Generic Data : {ex . Message}, {ex . Data}" );
			}
			finally
			{
					con . Close ( );
			}
			return dtGeneric;
		}
		#endregion Data loading to DataTable
		//********************************************************************************************************************************************************************************//
		#region Datatable loading from Datatables to collections

		public static ObservableCollection<BankAccountViewModel> LoadBankCollection ( DataTable dtBank , bool Notify = false )
		{
			int count = 0;
			ObservableCollection < BankAccountViewModel >     bvm = new ObservableCollection<BankAccountViewModel>();
			try
			{
				for ( int i = 0 ; i < dtBank . Rows . Count ; i++ )
				{
					bvm . Add ( new BankAccountViewModel
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
				Debug . WriteLine ( $"BANK : SQL Error in BankCollection(351) load function : {ex . Message}, {ex . Data}" );
				MessageBox . Show ( $"BANK : SQL Error in BankCollection (351) load function : {ex . Message}, {ex . Data}" );
			}
			finally
			{
				// This is ONLY called  if a requestor specifies the argument as TRUE
				if ( Notify )
				{
					Application . Current . Dispatcher . Invoke ( ( ) =>
					EventControl . TriggerBankDataLoaded ( null ,
						new LoadedEventArgs
						{
							CallerType = "SQLSUPPORT" ,
							DataSource = bvm ,
							RowCount = bvm . Count
						} )
					);
				}
			}
			return bvm;
		}
		public static ObservableCollection<CustomerViewModel> LoadCustomerCollection ( DataTable dtCust , bool Notify = false )
		{
			int count = 0;
			ObservableCollection<CustomerViewModel> cvm = new ObservableCollection<CustomerViewModel>();
			try
			{
				for ( int i = 0 ; i < dtCust . Rows . Count ; i++ )
				{
					cvm . Add ( new CustomerViewModel
					{
						Id = Convert . ToInt32 ( dtCust . Rows [ i ] [ 0 ] ) ,
						CustNo = dtCust . Rows [ i ] [ 1 ] . ToString ( ) ,
						BankNo = dtCust . Rows [ i ] [ 2 ] . ToString ( ) ,
						AcType = Convert . ToInt32 ( dtCust . Rows [ i ] [ 3 ] ) ,
						FName = dtCust . Rows [ i ] [ 4 ] . ToString ( ) ,
						LName = dtCust . Rows [ i ] [ 5 ] . ToString ( ) ,
						Addr1 = dtCust . Rows [ i ] [ 6 ] . ToString ( ) ,
						Addr2 = dtCust . Rows [ i ] [ 7 ] . ToString ( ) ,
						Town = dtCust . Rows [ i ] [ 8 ] . ToString ( ) ,
						County = dtCust . Rows [ i ] [ 9 ] . ToString ( ) ,
						PCode = dtCust . Rows [ i ] [ 10 ] . ToString ( ) ,
						Phone = dtCust . Rows [ i ] [ 11 ] . ToString ( ) ,
						Mobile = dtCust . Rows [ i ] [ 12 ] . ToString ( ) ,
						Dob = Convert . ToDateTime ( dtCust . Rows [ i ] [ 13 ] ) ,
						ODate = Convert . ToDateTime ( dtCust . Rows [ i ] [ 14 ] ) ,
						CDate = Convert . ToDateTime ( dtCust . Rows [ i ] [ 15 ] )
					} );
					count = i;
				}
			}
			catch ( Exception ex )
			{
				Debug . WriteLine ( $"CUSTOMERS : ERROR {ex . Message} + {ex . Data} ...." );
				cvm = null;
			}
			finally
			{
				if ( Notify && count > 0 )
				{
					Debug. WriteLine ( $"Triggering event CustDataLoaded with {cvm . Count}" );
					Application . Current . Dispatcher . Invoke ( ( ) =>
					EventControl . TriggerCustDataLoaded ( null ,
						  new LoadedEventArgs
						  {
							  CallerType = "SQLSUPPPORT" ,
							  DataSource = cvm ,
							  RowCount = cvm . Count
						  } )
					);
				}
			}
			Debug. WriteLine ( $"Customers Db Total = {cvm?.Count}" );
			return cvm;
		}
		public static ObservableCollection<DetailsViewModel> LoadDetailsCollection ( DataTable dtDetails , bool Notify = false )
		{
			int count = 0;
			ObservableCollection < DetailsViewModel > dvm = new ObservableCollection<DetailsViewModel>();
			try
			{
				Debug. WriteLine ( $" Loading Datable with {dtDetails . Rows . Count} records" );
				dvm . Clear ( );
				for ( int i = 0 ; i < dtDetails . Rows . Count ; i++ )
				{
					dvm . Add ( new DetailsViewModel
					{
						Id = Convert . ToInt32 ( dtDetails . Rows [ i ] [ 0 ] ) ,
						BankNo = dtDetails . Rows [ i ] [ 1 ] . ToString ( ) ,
						CustNo = dtDetails . Rows [ i ] [ 2 ] . ToString ( ) ,
						AcType = Convert . ToInt32 ( dtDetails . Rows [ i ] [ 3 ] ) ,
						Balance = Convert . ToDecimal ( dtDetails . Rows [ i ] [ 4 ] ) ,
						IntRate = Convert . ToDecimal ( dtDetails . Rows [ i ] [ 5 ] ) ,
						ODate = Convert . ToDateTime ( dtDetails . Rows [ i ] [ 6 ] ) ,
						CDate = Convert . ToDateTime ( dtDetails . Rows [ i ] [ 7 ] ) ,
					} );
					count = i;
				}
			}
			catch ( Exception ex )
			{
				Debug . WriteLine ( $"DETAILS : ERROR in  LoadDetCollection() : loading Details into ObservableCollection \"DetCollection\" : [{ex . Message}] : {ex . Data} ...." );
				MessageBox . Show ( $"DETAILS : ERROR in  LoadDetCollection() : loading Details into ObservableCollection \"DetCollection\" : [{ex . Message}] : {ex . Data} ...." );
				return null;
			}
			finally
			{
				if ( Notify )
				{
					EventControl . TriggerDetDataLoaded ( null ,
						new LoadedEventArgs
						{
							CallerType = "SQLSERVER" ,
							DataSource = ( object ) dvm ,
							RowCount = dvm . Count
						} );
				}
			}
			Debug. WriteLine ( $" DETAILS DB Loading () ALL FINISHED :  Records = [{dvm . Count}]" );
			return dvm;
		}
		public static ObservableCollection<GenericClass> LoadGenericCollection ( DataTable dtgeneric , bool Notify = false )
		{
			int count = 0;
			ObservableCollection < GenericClass > gvm = new ObservableCollection<GenericClass>();
			try
			{
				Debug. WriteLine ( $" Loading Datable with {dtgeneric . Rows . Count} records" );
				gvm . Clear ( );
				int colcount = dtgeneric.Columns.Count;
				if ( colcount > 20 )
					colcount = 20;
				for ( int i = 0 ; i < dtgeneric . Rows . Count ; i++ )
				{
					switch ( colcount )
					{
						case 20:
							gvm . Add ( new GenericClass
							{
								field1 = dtgeneric?.Rows [ i ] [ 0 ] . ToString ( ) ,
								field2 = dtgeneric?.Rows [ i ] [ 1 ] . ToString ( ) ,
								field3 = dtgeneric?.Rows [ i ] [ 2 ] . ToString ( ) ,
								field4 = dtgeneric?.Rows [ i ] [ 3 ] . ToString ( ) ,
								field5 = dtgeneric?.Rows [ i ] [ 4 ] . ToString ( ) ,
								field6 = dtgeneric?.Rows [ i ] [ 5 ] . ToString ( ) ,
								field7 = dtgeneric?.Rows [ i ] [ 6 ] . ToString ( ) ,
								field8 = dtgeneric?.Rows [ i ] [ 7 ] . ToString ( ) ,
								field9 = dtgeneric?.Rows [ i ] [ 8 ] . ToString ( ) ,
								field10 = dtgeneric?.Rows [ i ] [ 9 ] . ToString ( ) ,
								field11 = dtgeneric?.Rows [ i ] [ 10 ] . ToString ( ) ,
								field12 = dtgeneric?.Rows [ i ] [ 11 ] . ToString ( ) ,
								field13 = dtgeneric?.Rows [ i ] [ 12 ] . ToString ( ) ,
								field14 = dtgeneric?.Rows [ i ] [ 13 ] . ToString ( ) ,
								field15 = dtgeneric?.Rows [ i ] [ 14 ] . ToString ( ) ,
								field16 = dtgeneric?.Rows [ i ] [ 15 ] . ToString ( ) ,
								field17 = dtgeneric?.Rows [ i ] [ 16 ] . ToString ( ) ,
								field18 = dtgeneric?.Rows [ i ] [ 17 ] . ToString ( ) ,
								field19 = dtgeneric?.Rows [ i ] [ 18 ] . ToString ( ) ,
								field20 = dtgeneric?.Rows [ i ] [ 19 ] . ToString ( )
							} );
							break;
						case 19:
							gvm . Add ( new GenericClass
							{
								field1 = dtgeneric?.Rows [ i ] [ 0 ] . ToString ( ) ,
								field2 = dtgeneric?.Rows [ i ] [ 1 ] . ToString ( ) ,
								field3 = dtgeneric?.Rows [ i ] [ 2 ] . ToString ( ) ,
								field4 = dtgeneric?.Rows [ i ] [ 3 ] . ToString ( ) ,
								field5 = dtgeneric?.Rows [ i ] [ 4 ] . ToString ( ) ,
								field6 = dtgeneric?.Rows [ i ] [ 5 ] . ToString ( ) ,
								field7 = dtgeneric?.Rows [ i ] [ 6 ] . ToString ( ) ,
								field8 = dtgeneric?.Rows [ i ] [ 7 ] . ToString ( ) ,
								field9 = dtgeneric?.Rows [ i ] [ 8 ] . ToString ( ) ,
								field10 = dtgeneric?.Rows [ i ] [ 9 ] . ToString ( ) ,
								field11 = dtgeneric?.Rows [ i ] [ 10 ] . ToString ( ) ,
								field12 = dtgeneric?.Rows [ i ] [ 11 ] . ToString ( ) ,
								field13 = dtgeneric?.Rows [ i ] [ 12 ] . ToString ( ) ,
								field14 = dtgeneric?.Rows [ i ] [ 13 ] . ToString ( ) ,
								field15 = dtgeneric?.Rows [ i ] [ 14 ] . ToString ( ) ,
								field16 = dtgeneric?.Rows [ i ] [ 15 ] . ToString ( ) ,
								field17 = dtgeneric?.Rows [ i ] [ 16 ] . ToString ( ) ,
								field18 = dtgeneric?.Rows [ i ] [ 17 ] . ToString ( ) ,
								field19 = dtgeneric?.Rows [ i ] [ 18 ] . ToString ( ) ,
							} );
							break;
						case 18:
							gvm . Add ( new GenericClass
							{
								field1 = dtgeneric?.Rows [ i ] [ 0 ] . ToString ( ) ,
								field2 = dtgeneric?.Rows [ i ] [ 1 ] . ToString ( ) ,
								field3 = dtgeneric?.Rows [ i ] [ 2 ] . ToString ( ) ,
								field4 = dtgeneric?.Rows [ i ] [ 3 ] . ToString ( ) ,
								field5 = dtgeneric?.Rows [ i ] [ 4 ] . ToString ( ) ,
								field6 = dtgeneric?.Rows [ i ] [ 5 ] . ToString ( ) ,
								field7 = dtgeneric?.Rows [ i ] [ 6 ] . ToString ( ) ,
								field8 = dtgeneric?.Rows [ i ] [ 7 ] . ToString ( ) ,
								field9 = dtgeneric?.Rows [ i ] [ 8 ] . ToString ( ) ,
								field10 = dtgeneric?.Rows [ i ] [ 9 ] . ToString ( ) ,
								field11 = dtgeneric?.Rows [ i ] [ 10 ] . ToString ( ) ,
								field12 = dtgeneric?.Rows [ i ] [ 11 ] . ToString ( ) ,
								field13 = dtgeneric?.Rows [ i ] [ 12 ] . ToString ( ) ,
								field14 = dtgeneric?.Rows [ i ] [ 13 ] . ToString ( ) ,
								field15 = dtgeneric?.Rows [ i ] [ 14 ] . ToString ( ) ,
								field16 = dtgeneric?.Rows [ i ] [ 15 ] . ToString ( ) ,
								field17 = dtgeneric?.Rows [ i ] [ 16 ] . ToString ( ) ,
								field18 = dtgeneric?.Rows [ i ] [ 17 ] . ToString ( ) ,
							} );
							break;
						case 17:
							gvm . Add ( new GenericClass
							{
								field1 = dtgeneric?.Rows [ i ] [ 0 ] . ToString ( ) ,
								field2 = dtgeneric?.Rows [ i ] [ 1 ] . ToString ( ) ,
								field3 = dtgeneric?.Rows [ i ] [ 2 ] . ToString ( ) ,
								field4 = dtgeneric?.Rows [ i ] [ 3 ] . ToString ( ) ,
								field5 = dtgeneric?.Rows [ i ] [ 4 ] . ToString ( ) ,
								field6 = dtgeneric?.Rows [ i ] [ 5 ] . ToString ( ) ,
								field7 = dtgeneric?.Rows [ i ] [ 6 ] . ToString ( ) ,
								field8 = dtgeneric?.Rows [ i ] [ 7 ] . ToString ( ) ,
								field9 = dtgeneric?.Rows [ i ] [ 8 ] . ToString ( ) ,
								field10 = dtgeneric?.Rows [ i ] [ 9 ] . ToString ( ) ,
								field11 = dtgeneric?.Rows [ i ] [ 10 ] . ToString ( ) ,
								field12 = dtgeneric?.Rows [ i ] [ 11 ] . ToString ( ) ,
								field13 = dtgeneric?.Rows [ i ] [ 12 ] . ToString ( ) ,
								field14 = dtgeneric?.Rows [ i ] [ 13 ] . ToString ( ) ,
								field15 = dtgeneric?.Rows [ i ] [ 14 ] . ToString ( ) ,
								field16 = dtgeneric?.Rows [ i ] [ 15 ] . ToString ( ) ,
								field17 = dtgeneric?.Rows [ i ] [ 16 ] . ToString ( ) ,
							} );
							break;
						case 16:
							gvm . Add ( new GenericClass
							{
								field1 = dtgeneric?.Rows [ i ] [ 0 ] . ToString ( ) ,
								field2 = dtgeneric?.Rows [ i ] [ 1 ] . ToString ( ) ,
								field3 = dtgeneric?.Rows [ i ] [ 2 ] . ToString ( ) ,
								field4 = dtgeneric?.Rows [ i ] [ 3 ] . ToString ( ) ,
								field5 = dtgeneric?.Rows [ i ] [ 4 ] . ToString ( ) ,
								field6 = dtgeneric?.Rows [ i ] [ 5 ] . ToString ( ) ,
								field7 = dtgeneric?.Rows [ i ] [ 6 ] . ToString ( ) ,
								field8 = dtgeneric?.Rows [ i ] [ 7 ] . ToString ( ) ,
								field9 = dtgeneric?.Rows [ i ] [ 8 ] . ToString ( ) ,
								field10 = dtgeneric?.Rows [ i ] [ 9 ] . ToString ( ) ,
								field11 = dtgeneric?.Rows [ i ] [ 10 ] . ToString ( ) ,
								field12 = dtgeneric?.Rows [ i ] [ 11 ] . ToString ( ) ,
								field13 = dtgeneric?.Rows [ i ] [ 12 ] . ToString ( ) ,
								field14 = dtgeneric?.Rows [ i ] [ 13 ] . ToString ( ) ,
								field15 = dtgeneric?.Rows [ i ] [ 14 ] . ToString ( ) ,
								field16 = dtgeneric?.Rows [ i ] [ 15 ] . ToString ( ) ,
							} );
							break;
						case 15:
							gvm . Add ( new GenericClass
							{
								field1 = dtgeneric?.Rows [ i ] [ 0 ] . ToString ( ) ,
								field2 = dtgeneric?.Rows [ i ] [ 1 ] . ToString ( ) ,
								field3 = dtgeneric?.Rows [ i ] [ 2 ] . ToString ( ) ,
								field4 = dtgeneric?.Rows [ i ] [ 3 ] . ToString ( ) ,
								field5 = dtgeneric?.Rows [ i ] [ 4 ] . ToString ( ) ,
								field6 = dtgeneric?.Rows [ i ] [ 5 ] . ToString ( ) ,
								field7 = dtgeneric?.Rows [ i ] [ 6 ] . ToString ( ) ,
								field8 = dtgeneric?.Rows [ i ] [ 7 ] . ToString ( ) ,
								field9 = dtgeneric?.Rows [ i ] [ 8 ] . ToString ( ) ,
								field10 = dtgeneric?.Rows [ i ] [ 9 ] . ToString ( ) ,
								field11 = dtgeneric?.Rows [ i ] [ 10 ] . ToString ( ) ,
								field12 = dtgeneric?.Rows [ i ] [ 11 ] . ToString ( ) ,
								field13 = dtgeneric?.Rows [ i ] [ 12 ] . ToString ( ) ,
								field14 = dtgeneric?.Rows [ i ] [ 13 ] . ToString ( ) ,
								field15 = dtgeneric?.Rows [ i ] [ 14 ] . ToString ( ) ,
							} );
							break;
						case 14:
							gvm . Add ( new GenericClass
							{
								field1 = dtgeneric?.Rows [ i ] [ 0 ] . ToString ( ) ,
								field2 = dtgeneric?.Rows [ i ] [ 1 ] . ToString ( ) ,
								field3 = dtgeneric?.Rows [ i ] [ 2 ] . ToString ( ) ,
								field4 = dtgeneric?.Rows [ i ] [ 3 ] . ToString ( ) ,
								field5 = dtgeneric?.Rows [ i ] [ 4 ] . ToString ( ) ,
								field6 = dtgeneric?.Rows [ i ] [ 5 ] . ToString ( ) ,
								field7 = dtgeneric?.Rows [ i ] [ 6 ] . ToString ( ) ,
								field8 = dtgeneric?.Rows [ i ] [ 7 ] . ToString ( ) ,
								field9 = dtgeneric?.Rows [ i ] [ 8 ] . ToString ( ) ,
								field10 = dtgeneric?.Rows [ i ] [ 9 ] . ToString ( ) ,
								field11 = dtgeneric?.Rows [ i ] [ 10 ] . ToString ( ) ,
								field12 = dtgeneric?.Rows [ i ] [ 11 ] . ToString ( ) ,
								field13 = dtgeneric?.Rows [ i ] [ 12 ] . ToString ( ) ,
								field14 = dtgeneric?.Rows [ i ] [ 13 ] . ToString ( ) ,
							} );
							break;
						case 13:
							gvm . Add ( new GenericClass
							{
								field1 = dtgeneric?.Rows [ i ] [ 0 ] . ToString ( ) ,
								field2 = dtgeneric?.Rows [ i ] [ 1 ] . ToString ( ) ,
								field3 = dtgeneric?.Rows [ i ] [ 2 ] . ToString ( ) ,
								field4 = dtgeneric?.Rows [ i ] [ 3 ] . ToString ( ) ,
								field5 = dtgeneric?.Rows [ i ] [ 4 ] . ToString ( ) ,
								field6 = dtgeneric?.Rows [ i ] [ 5 ] . ToString ( ) ,
								field7 = dtgeneric?.Rows [ i ] [ 6 ] . ToString ( ) ,
								field8 = dtgeneric?.Rows [ i ] [ 7 ] . ToString ( ) ,
								field9 = dtgeneric?.Rows [ i ] [ 8 ] . ToString ( ) ,
								field10 = dtgeneric?.Rows [ i ] [ 9 ] . ToString ( ) ,
								field11 = dtgeneric?.Rows [ i ] [ 10 ] . ToString ( ) ,
								field12 = dtgeneric?.Rows [ i ] [ 11 ] . ToString ( ) ,
								field13 = dtgeneric?.Rows [ i ] [ 12 ] . ToString ( ) ,
							} );
							break;
						case 12:
							gvm . Add ( new GenericClass
							{
								field1 = dtgeneric?.Rows [ i ] [ 0 ] . ToString ( ) ,
								field2 = dtgeneric?.Rows [ i ] [ 1 ] . ToString ( ) ,
								field3 = dtgeneric?.Rows [ i ] [ 2 ] . ToString ( ) ,
								field4 = dtgeneric?.Rows [ i ] [ 3 ] . ToString ( ) ,
								field5 = dtgeneric?.Rows [ i ] [ 4 ] . ToString ( ) ,
								field6 = dtgeneric?.Rows [ i ] [ 5 ] . ToString ( ) ,
								field7 = dtgeneric?.Rows [ i ] [ 6 ] . ToString ( ) ,
								field8 = dtgeneric?.Rows [ i ] [ 7 ] . ToString ( ) ,
								field9 = dtgeneric?.Rows [ i ] [ 8 ] . ToString ( ) ,
								field10 = dtgeneric?.Rows [ i ] [ 9 ] . ToString ( ) ,
								field11 = dtgeneric?.Rows [ i ] [ 10 ] . ToString ( ) ,
								field12 = dtgeneric?.Rows [ i ] [ 11 ] . ToString ( ) ,
							} );
							break;
						case 11:
							gvm . Add ( new GenericClass
							{
								field1 = dtgeneric?.Rows [ i ] [ 0 ] . ToString ( ) ,
								field2 = dtgeneric?.Rows [ i ] [ 1 ] . ToString ( ) ,
								field3 = dtgeneric?.Rows [ i ] [ 2 ] . ToString ( ) ,
								field4 = dtgeneric?.Rows [ i ] [ 3 ] . ToString ( ) ,
								field5 = dtgeneric?.Rows [ i ] [ 4 ] . ToString ( ) ,
								field6 = dtgeneric?.Rows [ i ] [ 5 ] . ToString ( ) ,
								field7 = dtgeneric?.Rows [ i ] [ 6 ] . ToString ( ) ,
								field8 = dtgeneric?.Rows [ i ] [ 7 ] . ToString ( ) ,
								field9 = dtgeneric?.Rows [ i ] [ 8 ] . ToString ( ) ,
								field10 = dtgeneric?.Rows [ i ] [ 9 ] . ToString ( ) ,
								field11 = dtgeneric?.Rows [ i ] [ 10 ] . ToString ( ) ,
							} );
							break;
						case 10:
							gvm . Add ( new GenericClass
							{
								field1 = dtgeneric?.Rows [ i ] [ 0 ] . ToString ( ) ,
								field2 = dtgeneric?.Rows [ i ] [ 1 ] . ToString ( ) ,
								field3 = dtgeneric?.Rows [ i ] [ 2 ] . ToString ( ) ,
								field4 = dtgeneric?.Rows [ i ] [ 3 ] . ToString ( ) ,
								field5 = dtgeneric?.Rows [ i ] [ 4 ] . ToString ( ) ,
								field6 = dtgeneric?.Rows [ i ] [ 5 ] . ToString ( ) ,
								field7 = dtgeneric?.Rows [ i ] [ 6 ] . ToString ( ) ,
								field8 = dtgeneric?.Rows [ i ] [ 7 ] . ToString ( ) ,
								field9 = dtgeneric?.Rows [ i ] [ 8 ] . ToString ( ) ,
								field10 = dtgeneric?.Rows [ i ] [ 9 ] . ToString ( ) ,
							} );
							break;
						case 9:
							gvm . Add ( new GenericClass
							{
								field1 = dtgeneric?.Rows [ i ] [ 0 ] . ToString ( ) ,
								field2 = dtgeneric?.Rows [ i ] [ 1 ] . ToString ( ) ,
								field3 = dtgeneric?.Rows [ i ] [ 2 ] . ToString ( ) ,
								field4 = dtgeneric?.Rows [ i ] [ 3 ] . ToString ( ) ,
								field5 = dtgeneric?.Rows [ i ] [ 4 ] . ToString ( ) ,
								field6 = dtgeneric?.Rows [ i ] [ 5 ] . ToString ( ) ,
								field7 = dtgeneric?.Rows [ i ] [ 6 ] . ToString ( ) ,
								field8 = dtgeneric?.Rows [ i ] [ 7 ] . ToString ( ) ,
								field9 = dtgeneric?.Rows [ i ] [ 8 ] . ToString ( ) ,
							} );
							break;
						case 8:
							gvm . Add ( new GenericClass
							{
								field1 = dtgeneric?.Rows [ i ] [ 0 ] . ToString ( ) ,
								field2 = dtgeneric?.Rows [ i ] [ 1 ] . ToString ( ) ,
								field3 = dtgeneric?.Rows [ i ] [ 2 ] . ToString ( ) ,
								field4 = dtgeneric?.Rows [ i ] [ 3 ] . ToString ( ) ,
								field5 = dtgeneric?.Rows [ i ] [ 4 ] . ToString ( ) ,
								field6 = dtgeneric?.Rows [ i ] [ 5 ] . ToString ( ) ,
								field7 = dtgeneric?.Rows [ i ] [ 6 ] . ToString ( ) ,
								field8 = dtgeneric?.Rows [ i ] [ 7 ] . ToString ( ) ,
							} );
							break;
						case 7:
							gvm . Add ( new GenericClass
							{
								field1 = dtgeneric?.Rows [ i ] [ 0 ] . ToString ( ) ,
								field2 = dtgeneric?.Rows [ i ] [ 1 ] . ToString ( ) ,
								field3 = dtgeneric?.Rows [ i ] [ 2 ] . ToString ( ) ,
								field4 = dtgeneric?.Rows [ i ] [ 3 ] . ToString ( ) ,
								field5 = dtgeneric?.Rows [ i ] [ 4 ] . ToString ( ) ,
								field6 = dtgeneric?.Rows [ i ] [ 5 ] . ToString ( ) ,
								field7 = dtgeneric?.Rows [ i ] [ 6 ] . ToString ( ) ,
							} );
							break;
						case 6:
							gvm . Add ( new GenericClass
							{
								field1 = dtgeneric?.Rows [ i ] [ 0 ] . ToString ( ) ,
								field2 = dtgeneric?.Rows [ i ] [ 1 ] . ToString ( ) ,
								field3 = dtgeneric?.Rows [ i ] [ 2 ] . ToString ( ) ,
								field4 = dtgeneric?.Rows [ i ] [ 3 ] . ToString ( ) ,
								field5 = dtgeneric?.Rows [ i ] [ 4 ] . ToString ( ) ,
								field6 = dtgeneric?.Rows [ i ] [ 5 ] . ToString ( ) ,
							} );
							break;
						case 5:
							gvm . Add ( new GenericClass
							{
								field1 = dtgeneric?.Rows [ i ] [ 0 ] . ToString ( ) ,
								field2 = dtgeneric?.Rows [ i ] [ 1 ] . ToString ( ) ,
								field3 = dtgeneric?.Rows [ i ] [ 2 ] . ToString ( ) ,
								field4 = dtgeneric?.Rows [ i ] [ 3 ] . ToString ( ) ,
								field5 = dtgeneric?.Rows [ i ] [ 4 ] . ToString ( ) ,
							} );
							break;
						case 4:
							gvm . Add ( new GenericClass
							{
								field1 = dtgeneric?.Rows [ i ] [ 0 ] . ToString ( ) ,
								field2 = dtgeneric?.Rows [ i ] [ 1 ] . ToString ( ) ,
								field3 = dtgeneric?.Rows [ i ] [ 2 ] . ToString ( ) ,
								field4 = dtgeneric?.Rows [ i ] [ 3 ] . ToString ( ) ,
							} );
							break;
						case 3:
							gvm . Add ( new GenericClass
							{
								field1 = dtgeneric?.Rows [ i ] [ 0 ] . ToString ( ) ,
								field2 = dtgeneric?.Rows [ i ] [ 1 ] . ToString ( ) ,
								field3 = dtgeneric?.Rows [ i ] [ 2 ] . ToString ( ) ,
							} );
							break;
						case 2:
							gvm . Add ( new GenericClass
							{
								field1 = dtgeneric?.Rows [ i ] [ 0 ] . ToString ( ) ,
								field2 = dtgeneric?.Rows [ i ] [ 1 ] . ToString ( ) ,
							} );
							break;
						case 1:
							gvm . Add ( new GenericClass
							{
								field1 = dtgeneric?.Rows [ i ] [ 0 ] . ToString ( ) ,
							} );
							break;
					}
					count = i;
				}
			}
			catch ( Exception ex )
			{
				Debug . WriteLine ( $"GENERICS : ERROR in  LoadGenCollection() : loading Generic into ObservableCollection \"GenCollection\" : [{ex . Message}] : {ex . Data} ...." );
				//MessageBox . Show ( $"DETAILS : ERROR in  LoadDetCollection() : loading Details into ObservableCollection \"DetCollection\" : [{ex . Message}] : {ex . Data} ...." );
				//return null;
			}
			finally
			{
				if ( Notify )
				{
					EventControl . TriggerGenDataLoaded ( null ,
						new LoadedEventArgs
						{
							CallerType = "SQLSERVER" ,
							DataSource = ( object ) gvm ,
							RowCount = gvm . Count
						} );
				}
			}
			Debug. WriteLine ( $" DETAILS DB Loading () ALL FINISHED :  Records = [{gvm . Count}]" );
			return gvm;
		}

		#endregion Datatable loading from tables via SQL
		//********************************************************************************************************************************************************************************//
		#region	PERFORMSQLEXECUTECOMMAND

		#region Stored Procedures execution
		public static int PerformSqlExecuteCommand ( string SqlCommand , string [ ] args , out string err )
		//--------------------------------------------------------------------------------------------------------------------------------------------------------
		{
			//####################################################################################//
			// Handles running a dapper stored procedure call with transaction support & thrws exceptions back to caller
			//####################################################################################//
			int gresult = -1;
			//string Con= Flags . CurrentConnectionString;
			string Con = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
			SqlConnection sqlCon=null;
			err = "";

			try
			{
				using ( sqlCon = new SqlConnection ( Con ) )
				{
					var parameters = new DynamicParameters();
					sqlCon . Open ( );
					using ( var tran = sqlCon . BeginTransaction ( ) )
					{
						if ( ( SqlCommand . ToUpper ( ) == "SPINSERTSPECIFIEDROW" || SqlCommand . ToUpper ( ) == "SPCREATETABLE" || SqlCommand . ToUpper ( ) == "SPDROPTABLE" ) && args . Length > 0 )
						{
							if ( args [ 0 ] != "" )
								parameters . Add ( "Tablename" , args [ 0 ] , DbType . String , ParameterDirection . Input , args [ 0 ] . Length );
							if ( args [ 1 ] != "" )
								parameters . Add ( "cmd" , args [ 1 ] , DbType . String , ParameterDirection . Input , args [ 1 ] . Length );
							if ( args [ 2 ] != "" )
								parameters . Add ( "Values" , args [ 2 ] , DbType . String , ParameterDirection . Input , args [ 2 ] . Length );

							gresult = sqlCon . Execute ( @SqlCommand , parameters , commandType: CommandType . StoredProcedure , transaction: tran );
						}
						else
						{
							// Perform the sql command requested
							//							var parameters = "";
							gresult = sqlCon . Execute ( @SqlCommand , parameters , commandType:
                                 CommandType . StoredProcedure , transaction: tran );// as IEnumerable<GenericClass>;
																											 //var result  = sqlCon . Query( SqlCommand ,
																											 //  args,null,false, null,
																											 //   CommandType.StoredProcedure).ToList();
						}
						// Commit the transaction
						tran . Commit ( );
					}
				}
			}
			catch ( Exception ex )
			{
				Debug. WriteLine ( $"Error {ex . Message}, {ex . Data}" );
				err = $"Error {ex . Message}";
			}

			WpfLib1 . Utils .trace ( );

			return gresult;
		}
		#endregion Stored Procedures execution
		#endregion
		//********************************************************************************************************************************************************************************//

		public static ObservableCollection<GenericClass> ExecuteStoredProcedure ( string SqlCommand ,
			ObservableCollection<GenericClass> generics ,
			out string ResultString ,
			string DbName = "" ,
			string Arguments = "" ,
			RoutedEventArgs e = null ,
			bool displayData = false )

		{
			ResultString = "";
			string SavedValue = SqlCommand;
#pragma warning disable CS0219 // The variable 'dbnametoopen' is assigned but its value is never used
#pragma warning disable CS0219 // The variable 'command' is assigned but its value is never used
			string command = "", dbnametoopen = "";
#pragma warning restore CS0219 // The variable 'command' is assigned but its value is never used
#pragma warning restore CS0219 // The variable 'dbnametoopen' is assigned but its value is never used
			string errormsg="";
#pragma warning disable CS0219 // The variable 'WhereClause' is assigned but its value is never used
#pragma warning disable CS0219 // The variable 'OrderByClause' is assigned but its value is never used
			string  WhereClause="", OrderByClause="";
#pragma warning restore CS0219 // The variable 'OrderByClause' is assigned but its value is never used
#pragma warning restore CS0219 // The variable 'WhereClause' is assigned but its value is never used
#pragma warning disable CS0219 // The variable 'CheckingArgsOnly' is assigned but its value is never used
			bool CheckingArgsOnly = false;
#pragma warning restore CS0219 // The variable 'CheckingArgsOnly' is assigned but its value is never used
			int totalcolumns = 0;
			ObservableCollection<BankAccountViewModel> bvmparam = new ObservableCollection<BankAccountViewModel>();
			Dictionary <string, object>dict = new Dictionary<string, object>();
#pragma warning disable CS0219 // The variable 'DbResult' is assigned but its value is never used
			IEnumerable DbResult=null;
#pragma warning restore CS0219 // The variable 'DbResult' is assigned but its value is never used
			//============
			// Sanity checks
			//============
			// If it is a CopyDb Procedure, bale out, use the Copy button
			if ( SqlCommand . ToUpper ( ) . Contains ( "SPCOPYDB" ) )
			{
				MessageBox . Show ( $"Please use the 'Copy Db' button at top right to perform this operation.." , "Input error" , MessageBoxButton . OK );
				return null;
			}
			if ( SavedValue == "spGetFullSchema" && Arguments == "FULL" )
			{
				Arguments = "";
			}
		
            {
				}
			try
			{
				List<string> genericlist = new List<string>();
				bool usegeneric = false;
#pragma warning disable CS0219 // The variable 'outbuffer' is assigned but its value is never used
				string outbuffer="";
#pragma warning restore CS0219 // The variable 'outbuffer' is assigned but its value is never used

				if ( usegeneric )
				{
					GenericClass gc = new GenericClass ( );
					List<GenericClass> genlist = new List<GenericClass>();
					DapperGeneric<GenericClass , ObservableCollection<GenericClass> , List<GenericClass>> . ExecuteSPFullGenericClass (
						ref generics ,
						false ,
						ref generics ,
						SqlCommand ,
						Arguments ,
						"" ,
						"" ,
						ref genlist ,
						 out errormsg );
					ResultString = errormsg;
				}
				else
				{
					generics . Clear ( );
					totalcolumns = DapperSupport . CreateGenericCollection (
						ref generics ,
						SqlCommand ,
						Arguments ,
						"" ,
						"" ,
						ref genericlist ,
						ref errormsg );
					ResultString = errormsg;
				}
				return generics;
			}
			catch ( Exception ex )
			{
				MessageBox . Show ( $"SQL ERROR 1125 - {ex . Message}" );
				return null;
			}
		}

		private static object LoadListData ( object grid , ObservableCollection<GenericClass> genericcollection , int total )
		{
			if ( total == 1 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1}).ToList();
				return res as object;
			}
			else if ( total == 2 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1,data.field2}).ToList();
				return res as object;
			}
			else if ( total == 3 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2,data.field3}).ToList();
				return res as object;
			}
			else if ( total == 4 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4}).ToList();
				return res as object;
			}
			else if ( total == 5 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5}).ToList();
				return res as object;
		}
			else if ( total == 6 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6}).ToList();
				return res as object;
			}
			else if ( total == 7 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7}).ToList();
				return res as object;
			}
			else if ( total == 8 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8}).ToList();
				return res as object;
			}
			else if ( total == 9 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9}).ToList();
				return res as object;
			}
			else if ( total == 10 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9 ,data.field10}).ToList();
				return res as object;
			}
			else if ( total == 11 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9,data.field10,data.field11}).ToList();
				return res as object;
			}
			else if ( total == 12 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9,data.field10,data.field11,data.field12}).ToList();
				return res as object;
				}
			else if ( total == 13 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9,data.field10,data.field11,data.field12,data.field13}).ToList();
				return res as object;
			}
			else if ( total == 14 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9,data.field10,data.field11,data.field12,data.field13,data.field14}).ToList();
				return res as object;
			}
			else if ( total == 15 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9,data.field10,data.field11,data.field12,data.field13,data.field14,data.field15}).ToList();
				return res as object;
			}
			else if ( total == 16 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9,data.field10,data.field11,data.field12,data.field13,data.field14,data.field15,data.field16}).ToList();
				return res as object;
			}
			else if ( total == 17 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9,data.field10,data.field11,data.field12,data.field13,data.field14,data.field15,data.field16,data.field17}).ToList();
				return res as object;
			}
			else if ( total == 18 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9,data.field10,data.field11,data.field12,data.field13,data.field14,data.field15,data.field16,data.field17,data.field18}).ToList();
				return res as object;
			}
			else if ( total == 19 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9,data.field10,data.field11,data.field12,data.field13,data.field14,data.field15,data.field16,data.field17,data.field18,data.field19}).ToList();
				return res as object;
			}
			else if ( total == 20 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9,data.field10,data.field11,data.field12,data.field13,data.field14,data.field15,data.field16,data.field17,data.field18,data.field19,data.field20}).ToList();
				return res as object;
			}
			return null;
		}
		public static void LoadActiveRowsOnlyInLView ( ListView grid , ObservableCollection<GenericClass> genericcollection , int total )
		//********************************************************************************************************************************************************************************//
		{
			ListView Grid = grid ;
			// filter data to remove all "extraneous" columns
			Grid . ItemsSource = null;
			Grid . Items . Clear ( );
			LoadListData ( Grid , genericcollection , total );
			Grid . SelectedIndex = 0;
			Grid . Visibility = Visibility . Visible;
			//Grid . Refresh ( );
			Grid . Focus ( );
		}
		public static bool Executestoredproc (string SqlCommand, string ConString )
		{
			using ( IDbConnection db = new SqlConnection ( ConString ) )
			{
				try
				{
					// Use DAPPER to to load Bank data using Stored Procedure
					// This syntax WORKS CORRECTLY
					//******************************************************************************************************//
					var result  = db . Query( SqlCommand , null,null,false, null,CommandType.StoredProcedure);
					//******************************************************************************************************//

					var arry =result .ToArray();
					}
				catch ( Exception ex )
				{
					Debug. WriteLine ( $"SQL DAPPER error : {ex . Message}, {ex . Data}" );
					return false;
				}
				finally
				{
					Debug. WriteLine ( $"SQL DAPPER command [ {SqlCommand}] completed successfuly" );
				}
				return true;
			}
		}
	}
}
