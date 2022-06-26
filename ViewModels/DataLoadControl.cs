 using NewWpfDev. Dapper;
using NewWpfDev. SQL;
using NewWpfDev. Views;

using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . Data;
using System . Data . SqlClient;
using System . Diagnostics;
using System . Linq;
using System . Security . AccessControl;
using System . Text;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Threading;

using static NewWpfDev. MainWindow;

namespace NewWpfDev. ViewModels
{
	public class DataLoadControl
	{
		ObservableCollection<BankAccountViewModel> bankcollection ;
		ObservableCollection<CustomerViewModel> custcollection ;
		ObservableCollection<DetailsViewModel> detcollection ;
		ObservableCollection<GenericClass> gencollection ;


		//****************************//
		// DIRECT access Method
		//****************************//
		// Simplest data load method - return DataTable of any type
		//Accepts just a fully qualified SQL command string
		#region Generic Sql Execute method - all data types
		public static DataTable GetDataTable ( string commandline )
		{
			DataTable dt = new DataTable();
			try
			{
				SqlConnection con;
				string ConString = Flags . CurrentConnectionString;
				con = new SqlConnection ( ConString );
				using ( con )
				{
					SqlCommand cmd = new SqlCommand ( commandline, con );
					SqlDataAdapter sda = new SqlDataAdapter ( cmd );
					sda . Fill ( dt );
				}
			}
			catch ( Exception ex )
			{
				Debug . WriteLine ( $"Failed to load Db - {ex . Message}, {ex . Data}" );
				return null;
			}
			//Utils . trace ( "RunSqlCommand" );

			return dt;
		}
		#endregion Generic Sql Execute method - all data types

		//***************************************//
		// BACKGROUND Worker  Methods
		//***************************************//
		// METHODS CALLED VIA THE DELEGATE declared in MainWindow (All Using M$ BackgroundWorker System)
		#region LoadBackground class methods
		//******************************************//
		//Called by a Delegate to load SQL data (in a BackgroundWorker thread)
		// Working Well 6/2/22
		//******************************************//
		 public void LoadTableInBackground ( string Sqlcommand , string tabletype, object obj )
		{
			int[] args={0,0,0,0 };
			if ( tabletype == "BANKACCOUNT" )
			{
				bankcollection = obj as ObservableCollection<BankAccountViewModel>;
				// We are running a BackGroundWorker thread, so
		
				Application . Current . Dispatcher . Invoke ( ( ) =>
				{
					obj = SqlBackgroundLoad . LoadBackground_Bank (
				     bankcollection ,
				     Sqlcommand ,
					"" ,
					"" ,
					"" ,
					false ,
					false ,
					false ,
					"" ,
					args );
				} );
			}
			else if ( tabletype == "CUSTOMER" )
			{
				custcollection = obj as ObservableCollection<CustomerViewModel>;
				// We are running a BackGroundWorker thread, so
				// We MUST use the Dispatcher because the ObservableCollections will not allow changes on a different thread
				Application . Current . Dispatcher . Invoke ( ( ) =>
				{
					obj = SqlBackgroundLoad . LoadBackground_Customer (
				     custcollection ,
				     Sqlcommand ,
					"" ,
					"" ,
					"" ,
					false ,
					false ,
					false ,
					"" ,
					args );
				} );
			}
			else if ( tabletype == "SECACCOUNTS" )
			{
				detcollection = obj as ObservableCollection<DetailsViewModel>;
				// We are running a BackGroundWorker thread, so
				// We MUST use the Dispatcher because the ObservableCollections will not allow changes on a different thread
				Application . Current . Dispatcher . Invoke ( ( ) =>
				{
					obj = SqlBackgroundLoad . LoadBackground_Details (
				     detcollection ,
				     Sqlcommand ,
					"" ,
					"" ,
					"" ,
					false ,
					false ,
					false ,
					"" ,
					args );
				} );
			}
			else if ( tabletype == "*" )
			{
				gencollection = obj as ObservableCollection<GenericClass>;
				Application . Current . Dispatcher . Invoke ( ( ) =>
				{
					string rslt="";
				ObservableCollection<GenericClass> gencollection = new ObservableCollection<GenericClass>();
				SqlSupport . ExecuteStoredProcedure ( Sqlcommand , gencollection , ResultString: out rslt );
				obj = gencollection;
				} );
			}
		}
		#endregion LoadBackground class methods

		#region Normal load class methods
		public void LoadTablewithDapper( string Sqlcommand , string tabletype , object obj , object DbLoadArgs)
		{
			int[] args={0,0,0,0 };
			if ( tabletype == "BANKACCOUNT" )
			{
				bankcollection = obj as ObservableCollection<BankAccountViewModel>;
				Application . Current . Dispatcher . Invoke ( ( ) =>
				{
					DapperSupport . GetBankObsCollection (
						bankcollection ,
						Sqlcommand ,
						"" ,
						"" ,
						"" ,
						false ,
						false ,
						true ,
						"" ,
						null );                    
					} );
			}
			else if ( tabletype == "CUSTOMER" )
			{
				custcollection = obj  as ObservableCollection<CustomerViewModel>;
				Application . Current . Dispatcher . Invoke ( ( ) =>
				{
					DapperSupport . GetCustObsCollection (
						custcollection ,
						Sqlcommand ,
						"" ,
						"" ,
						"" ,
						false ,
						false ,
						true ,
						"" ,
						null );
				} );
			}
			else if ( tabletype == "SECACCOUNTS" )
			{
				detcollection = obj as ObservableCollection<DetailsViewModel>;
				Application . Current . Dispatcher . Invoke ( ( ) =>
				{
					DapperSupport . GetDetailsObsCollection (
						detcollection ,
						Sqlcommand ,
						"" ,
						"" ,
						"" ,
						false ,
						false ,
						true ,
						"" ,
						null );
				} );
			}
			else if ( tabletype == "*"  || tabletype=="")
			{
				gencollection = obj as ObservableCollection<GenericClass>;
				Application . Current . Dispatcher . Invoke ( ( ) =>
				{
					DapperSupport . GetDetailsObsCollection (
						detcollection ,
						Sqlcommand ,
						"" ,
						"" ,
						"" ,
						false ,
						false ,
						true ,
						"" ,
						null );
				} );
			}
		}
		#endregion Normal load class methods

	}
}
