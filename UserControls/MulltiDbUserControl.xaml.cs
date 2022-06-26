using NewWpfDev . SQL;
using NewWpfDev . ViewModels;
using NewWpfDev . Views;
using NewWpfDev . UserControls;

using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . Data . SqlClient;
using System . Diagnostics;
using System . Linq;
using System . Text;
using System . Threading;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;

using static System . Net . WebRequestMethods;
using System . ComponentModel;
using System . Data;
using NewWpfDev . Dapper;
using System . Xml . Linq;
using NewWpfDev . Sql;
using NewWpfDev . Models;

namespace NewWpfDev . UserControls
{
	/// <summary>
	/// Interaction logic for BankGridUserControl.xaml
	/// </summary>
	public partial class MulltiDbUserControl 
	{
		// Individual records
		public BankAccountViewModel bvm = new BankAccountViewModel();
		public CustomerViewModel cvm = new CustomerViewModel ();
		public DetailsViewModel dvm = new DetailsViewModel();
		public GenericClass  gvm = new GenericClass  ();

		// Collections
		public ObservableCollection<BankAccountViewModel> bankaccts = new ObservableCollection<BankAccountViewModel>();
		public ObservableCollection<CustomerViewModel> custaccts = new ObservableCollection<CustomerViewModel>();
		public ObservableCollection<DetailsViewModel> detaccts = new ObservableCollection<DetailsViewModel>();
		public ObservableCollection<GenericClass> genaccts = new ObservableCollection<GenericClass>();

		// Northwind Records / collections
		public nwcustomer nwc = new nwcustomer();
		public NwOrderCollection  nwo = new NwOrderCollection( );
		public ObservableCollection<nwcustomer> nwcustomeraccts = new ObservableCollection<nwcustomer>();
		public ObservableCollection<nworder> nworderaccts = new ObservableCollection<nworder>();

		// Pubs
		public PubAuthors pubAuthor= new PubAuthors ();
		static public ObservableCollection<PubAuthors > pubauthors= new   ObservableCollection<PubAuthors >();

		// supporting sources
		public List<string> TablesList = new List<string>();
		private string SqlSpCommand { get; set; }
		private string CurrentSPDb { get; set; }
		private string SqlCommand="";
		private string DefaultSqlCommand="Select * from BankAccount";
		private string CurrentType= "BANKACCOUNT";

#pragma warning disable CS0414 // The field 'MulltiDbUserControl.CurrentDbName' is assigned but its value is never used
		string CurrentDbName = "IAN1";
#pragma warning restore CS0414 // The field 'MulltiDbUserControl.CurrentDbName' is assigned but its value is never used
		string CurrentTableName="BANKACCOUNT";
#pragma warning disable CS0414 // The field 'MulltiDbUserControl.CurrentDataTable' is assigned but its value is never used
		string CurrentDataTable = "";
#pragma warning restore CS0414 // The field 'MulltiDbUserControl.CurrentDataTable' is assigned but its value is never used
		private bool IsSelectionChanged=false;
		public static string CurrentSqlConnection = "BankSysConnectionString";
		public static Dictionary <string, string> DefaultSqlCommands = new Dictionary<string, string>();

#pragma warning disable CS0414 // The field 'MulltiDbUserControl.alldone' is assigned but its value is never used
		private bool alldone=false;
#pragma warning restore CS0414 // The field 'MulltiDbUserControl.alldone' is assigned but its value is never used
		// pro temp variables
		// Flowdoc flags
#pragma warning disable CS0414 // The field 'MulltiDbUserControl.UseFlowdoc' is assigned but its value is never used
		private bool UseFlowdoc=false;
#pragma warning restore CS0414 // The field 'MulltiDbUserControl.UseFlowdoc' is assigned but its value is never used
#pragma warning disable CS0414 // The field 'MulltiDbUserControl.UseFlowdocBeep' is assigned but its value is never used
		private bool UseFlowdocBeep=false;
#pragma warning restore CS0414 // The field 'MulltiDbUserControl.UseFlowdocBeep' is assigned but its value is never used
#pragma warning disable CS0414 // The field 'MulltiDbUserControl.UseScrollViewer' is assigned but its value is never used
		private bool UseScrollViewer= false;
#pragma warning restore CS0414 // The field 'MulltiDbUserControl.UseScrollViewer' is assigned but its value is never used
#pragma warning disable CS0414 // The field 'MulltiDbUserControl.TvMouseCaptured' is assigned but its value is never used
		private bool  TvMouseCaptured = false;
#pragma warning restore CS0414 // The field 'MulltiDbUserControl.TvMouseCaptured' is assigned but its value is never used

		private Window win { get; set; }

		private bool Usetimer = true;
#pragma warning disable CS0414 // The field 'MulltiDbUserControl.ComboSelectionActive' is assigned but its value is never used
		private bool ComboSelectionActive = false;
#pragma warning restore CS0414 // The field 'MulltiDbUserControl.ComboSelectionActive' is assigned but its value is never used
		private static Stopwatch timer = new Stopwatch();
		public void SetParent ( Window parent )
		{
			win = parent;
		}
		public MulltiDbUserControl ( )
		{
			InitializeComponent ( );
			this . DataContext = this;
			//Utils . SetupWindowDrag ( this );
		}
		public void InitialLoad ( )
		{
			Utils . LoadConnectionStrings ( );
			// Get list of Dbs (just 3 right now)
			LoadAllDbNames ( );
			LoadTablesList ( );
			// Initialize all default Sql command strings
			LoadDefaultSqlCommands ( );
			DefaultSqlCommand = GetDefaultSqlCommand ( "BANKACCOUNT" );
			if ( DefaultSqlCommand == "" )
			{
				Debug. WriteLine ( "Unable to load default Select string for BANKACOUNT Db " );
				Utils . DoErrorBeep ( 250 , 50 , 1 );
			}
			else
				SqlCommand = DefaultSqlCommand;
			if ( Utils . CheckResetDbConnection ( "IAN1" , out string constr ) == false )
			{
				Debug. WriteLine ( "Unable to load connection string for BANKACOUNT Db from System Properties" );
				Utils . DoErrorBeep ( 250 , 50 , 1 );
			}
			//Default to Bankaccount as we are strting up
			CurrentDbName = "IAN1";
			CurrentTableName = "BANKACCOUNT";

			// Now open Ian1, load data and display in both viewers
			// 1st = connect to IAN1.MDF
			// this also loads list of tables in Ian1/mdf
			OpenIan1Db ( );
			// used to access Dictionary of DataTemplates  - load into both Combos and selects 1st entry

			// now load list of tables in IAN1 Db
			LoadDbTables ( "IAN1" );
			this . UpdateLayout ( );
			this . SizeChanged += BankGridUserControl_SizeChanged;
			CurrentType = "BANKACCOUNT";
			SqlCommand = GetSqlCommand ( 0, DbListbox . SelectedIndex , "" , "" );
			LoadData ( );
			LoadGrid ( );
		}
		private void BankGridUserControl_SizeChanged ( object sender , SizeChangedEventArgs e )
		{
			Debug. WriteLine ( $"bgcanvas width={bgcanvas . Width}, grid" );
		}
		private void ReloadDatagrids ( object sender , RoutedEventArgs e )
		{
			int max=0;
			//ShowInfo ( "" );
			// Load Db based on Parameters entered by user
			//var result = int . TryParse ( RecCount . Text , out max);
			if ( DbMain . Items . Count == 0 )
				InitialLoad ( );
			bankaccts = new ObservableCollection<BankAccountViewModel> ( );
			BankDataGrid . ItemsSource = null;
			BankDataGrid . Refresh ( );
			SqlCommand = GetSqlCommand ( max , DbListbox . SelectedIndex , "" , "" );
			if ( SqlCommand == "" )
				SqlCommand = $"";
			//ShowInfo ( info: $"Processing command [{SqlCommand}] ..." );
			LoadData ( );
			LoadGrid ( );
		}
		public string GetSqlCommand ( int count = 0 , int table = 0 , string condition = "" , string sortorder = "" )
		{
			// Parse fields into a valid SQL Command string
			if ( DbListbox . SelectedItem == null )
				return "";
			string output = "Select  ";
			output += count == 0 ? " * From " : $"top ({count}) * From ";
			output += DbListbox . SelectedItem . ToString ( );
			output += condition . Trim ( ) != "" ? " Where " + condition + " " : "";
			output += sortorder . Trim ( ) != "" ? " Order by " + sortorder . Trim ( ) : "";
			CurrentType = DbListbox . Items [ table ] . ToString ( ) . ToUpper ( );
			return output;
		}
		private void LoadData ( )
		{
			// This method is called on the UI Thread, and require the Events system to be notified when data is ready for us
			// It accepts a fully qualified Sql Command line string to process, a maximum # of recrods to load, and a Notify Event completed flag
			// SIMPLER METHODS !!
			if ( Usetimer )
				timer . Start ( );
			if ( CurrentType == "BANKACCOUNT" )
			{
				bankaccts = new ObservableCollection<BankAccountViewModel> ( );
				bankaccts = SqlSupport . LoadBank ( SqlCommand , 0 , true );
			}
			else if ( CurrentType == "CUSTOMER" )
			{
				custaccts = new ObservableCollection<CustomerViewModel> ( );
				custaccts = SqlSupport . LoadCustomer ( SqlCommand , 0 , true );
			}
			else if ( CurrentType == "SECACCOUNTS" )
			{
				detaccts = new ObservableCollection<DetailsViewModel> ( );
				detaccts = SqlSupport . LoadDetails ( SqlCommand , 0 , true );
			}
			else
			{
				// WORKING 5.2.22
				// This creates and loads a GenericClass table if data is found in the selected table
				string ResultString="";
				CurrentTableName = DbListbox . SelectedItem . ToString ( ) . ToUpper ( );
				string tablename = CurrentTableName ;
				if ( Utils . CheckResetDbConnection ( DbMain . SelectedItem . ToString ( ) , out string constr ) == false )
				{
					Debug. WriteLine ( $"Failed to set connection string for {CurrentTableName . ToUpper ( )} Db" );
					return;
				}
				SqlCommand = $"Select *from {tablename}";
				genaccts = SqlSupport . LoadGeneric ( SqlCommand , out ResultString , 0 , true );
				if ( genaccts . Count > 0 )
				{
					LoadGrid ( genaccts );
				}
			}
		}
		private void LoadGrid ( object obj = null )
		{

			//			ShowLoadtime ( );

			// Load whatever data we have received into DataGrid
			if ( CurrentType . ToUpper ( ) == "BANKACCOUNT" )
			{
				if ( bankaccts == null )
					return;
				BankDataGrid . ItemsSource = bankaccts;
				//DbCount = bankaccts . Count;
				//ShowInfo ( line1: $"The requested table [{CurrentType}] was loaded successfully, and the {DbCount} records returned are displayed in the table below" , clr1: "Black0" ,
				//	line2: $"The command line used was" , clr2: "Red2" ,
				//	line3: $"{SqlCommand . ToUpper ( )}" , clr3: "Blue4" ,
				//	header: "Bank Accounts data table" , clr4: "Red5" );
				BankDataGrid . SelectedIndex = 0;
				BankDataGrid . Focus ( );
				RecordsCount = BankDataGrid . Items . Count;
			}
			else if ( CurrentType . ToUpper ( ) == "CUSTOMER" )
			{
				if ( custaccts == null )
					return;
				BankDataGrid . ItemsSource = custaccts;
				//DbCount = custaccts . Count;
				//ShowInfo ( line1: $"The requested table [{CurrentType}] was loaded successfully, and the {DbCount} records returned are displayed in the table below" , clr1: "Black0" ,
				//	line2: $"The command line used was" , clr2: "Red2" ,
				//	line3: $"{SqlCommand . ToUpper ( )}" , clr3: "Blue4" ,
				//	header: "All Customers data table" , clr4: "Red5" );
				BankDataGrid . SelectedIndex = 0;
				BankDataGrid . Focus ( );
				RecordsCount = BankDataGrid . Items . Count;
			}
			else if ( CurrentType . ToUpper ( ) == "SECACCOUNTS" )
			{
				if ( detaccts == null )
					return;
				BankDataGrid . ItemsSource = detaccts;
				//				DbCount = detaccts . Count;
				//				ShowInfo ( line1: $"The requested table [{CurrentType}] was loaded successfully, and the {DbCount} records returned are displayed in the table below" , clr1: "Black0" ,
				//					line2: $"The command line used was" , clr2: "Red2" ,
				//					line3: $"{SqlCommand . ToUpper ( )}" , clr3: "Blue4" ,
				//					header: "Secondary Accounts data table" );
				BankDataGrid . SelectedIndex = 0;
				BankDataGrid . Focus ( );
				RecordsCount = BankDataGrid . Items . Count;
			}
			else
			{
				if ( genaccts . Count == 0 )
				{
					//ShowInfo ( line1: $"The requested table [ {CurrentType} ] succeeded, but returned Zero rows of data." , clr1: "Green5" , header: "It is quite likely that the table is actually empty !" , clr4: "Cyan1" );
					//GenericClass gc = new GenericClass();
					//gc . field1 = "The database request succeeded but did not return any records for the selected table";
					//genaccts . Add ( gc );
					//SqlServerCommands . LoadActiveRowsOnlyInGrid ( BankDataGrid , genaccts , SqlServerCommands . GetGenericColumnCount ( genaccts ) );
					GenericDbUtilities . SetNullRecords (genaccts, BankDataGrid , CurrentType);
					//BankDataGrid . Columns [ 0 ] . Header = "Query result Information";
					RecordsCount = BankDataGrid . Items . Count - 1;
					return;
				}
				// Caution : This loads the data into the Datarid with only the selected rows
				// //visible in the grid so do NOT repopulate the grid after making this call
				//				SqlServerCommands sqlc = new SqlServerCommands();
				SqlServerCommands . LoadActiveRowsOnlyInGrid ( BankDataGrid , genaccts , SqlServerCommands . GetGenericColumnCount ( genaccts ) );
				if ( Flags . ReplaceFldNames )
				{
					string CurrentDomain = DbMain. SelectedItem . ToString ( ) . ToUpper ( );
					GenericDbUtilities . ReplaceDataGridFldNames ( CurrentType, ref BankDataGrid , CurrentDomain);
				}
				//				Grid1 . ItemsSource = genaccts;
				//				DbCount = genaccts . Count;
				//				ShowInfo ( header: "Unrecognised table accessed successfully" , clr4: "Red5" ,
				//					line1: $"Request made was completed succesfully!" , clr1: "Red3" ,
				//					line2: $"the table [{CurrentType}] that was queried returned a record count of {DbCount}.\nThe structure of this data is not recognised, so a generic structure has been used..." ,
				//					line3: $"{SqlCommand . ToUpper ( )}" , clr3: "Blue4"
				//					);
				BankDataGrid . SelectedIndex = 0;
				BankDataGrid . Focus ( );
				RecordsCount = BankDataGrid . Items . Count;
			}
		}
		private void LoadData_Ian1 ( string viewertype )
		{
			//if ( Usetimer )
			//	Timer . Start ( );
			if ( CurrentTableName == "BANKACCOUNT" )
			{
				//// Looad available DataTemplates into combo(s) ??
				//				LoadDataTemplates_Ian1 ( "BANKACCOUNT" , viewertype );
				bankaccts = new ObservableCollection<BankAccountViewModel> ( );
				//				SqlCommand = GetDefaultSqlCommand ( CurrentTableName );
				// need to do this cos the SQL command is changed to load the tables list....
				//				if ( Startup )
				//					SqlCommand = DefaultSqlCommand;
				bankaccts = SqlSupport . LoadBank ( SqlCommand , 0 , true );
				BankDataGrid . ItemsSource = bankaccts;
			}
			else if ( CurrentTableName == "CUSTOMER" )
			{
				//				LoadDataTemplates_Ian1 ( "CUSTOMER" , viewertype );
				custaccts = new ObservableCollection<CustomerViewModel> ( );
				//				SqlCommand = GetDefaultSqlCommand ( CurrentTableName );
				// need to do this cos the SQL command is changed to load the tables list....
				//				if ( Startup )
				//					SqlCommand = DefaultSqlCommand;
				custaccts = SqlSupport . LoadCustomer ( SqlCommand , 0 , true );
				BankDataGrid . ItemsSource = custaccts;
			}
			else if ( CurrentTableName == "SECACCOUNTS" )
			{
				//				LoadDataTemplates_Ian1 ( "SECACCOUNTS" , viewertype );
				detaccts = new ObservableCollection<DetailsViewModel> ( );
				//				SqlCommand = GetDefaultSqlCommand ( CurrentTableName );
				// need to do this cos the SQL command is changed to load the tables list....
				//				if ( Startup )
				//					SqlCommand = DefaultSqlCommand;
				detaccts = SqlSupport . LoadDetails ( SqlCommand , 0 , true );
				BankDataGrid . ItemsSource = detaccts;
			}
			else
			{
				string tablename="";
				////				LoadDataTemplates_Ian1 ( "GENERIC" , viewertype );
				genaccts = new ObservableCollection<GenericClass> ( );
				// need to do this cos the SQL command is changed to load the tables list....
				//if ( viewertype == "VIEW" )
				//	tablename = dbNameLv . SelectedItem . ToString ( );
				//else
				//	tablename = dbNameLb . SelectedItem . ToString ( );
				SqlCommand = $"Select *from {tablename}";
				SqlCommand = GetDefaultSqlCommand ( CurrentTableName );
				if ( SqlCommand == "" )
					SqlCommand = $"Select * from {tablename}";
				// need to do this cos the SQL command is changed to load the tables list....
				//				if ( Startup )
				//					SqlCommand = DefaultSqlCommand;
				string ResultString="";
				genaccts = SqlSupport . LoadGeneric ( SqlCommand , out ResultString , 0 , true );
				BankDataGrid . ItemsSource = genaccts;

				//if ( genaccts . Count > 0 )
				//{
				//	ShowInfo ( line1: $"The requested table [{CurrentTableName }] was loaded successfully, and {genaccts . Count} records were returned,\nThe data is shown in  the viewer below" , clr1: "Black0" ,
				//		line2: $"The command line used was" , clr2: "Red2" ,
				//		line3: $"{SqlCommand . ToUpper ( )}" , clr3: "Blue4" ,
				//		header: "Generic style data table" , clr4: "Red5" );
				//}
				//else
				//{
				//	ShowInfo ( line1: $"The requested table [{CurrentTableName }] was loaded successfully, but ZERO records were returned,\nThe Table is  " , clr1: "Black0" ,
				//		line2: $"The command line used was" , clr2: "Red2" ,
				//		line3: $"{SqlCommand . ToUpper ( )}" , clr3: "Blue4" ,
				//		header: "Generic style data table" , clr4: "Red5" );


				//}
				//else
				{
					// MORE COMPLEX METHODS !!
					if ( Usetimer )
						timer . Start ( );
					if ( CurrentType == "BANKACCOUNT" )
						DapperSupport . GetBankObsCollection ( bankaccts , DbNameToLoad: "BankAccount" , Orderby: "Custno, BankNo" , wantSort: true , Caller: "DATAGRIDS" , Notify: true );
					else if ( CurrentType == "CUSTOMER" )
						DapperSupport . GetCustObsCollection ( custaccts , DbNameToLoad: "Customer" , Orderby: "Custno, BankNo" , wantSort: true , Caller: "DATAGRIDS" , Notify: true );
					else if ( CurrentType == "SECACCOUNTS" )
						DapperSupport . GetDetailsObsCollection ( detaccts , DbNameToLoad: "SecAccounts" , Orderby: "Custno, BankNo" , wantSort: true , Caller: "DATAGRIDS" , Notify: true );
				}
			}
		}

		// Load list of databases in MSSQL
		private void LoadAllDbNames ( )
		{
			int bankindex = 0, count=0;
			List<string> list = new List<string>      ();
			SqlCommand = "spGetAllDatabaseNames";
			Datagrids . CallStoredProcedure ( list , SqlCommand );
			//This call returns us a DataTable
			DataTable dt = DataLoadControl . GetDataTable ( SqlCommand );
			// This how to access  Row data from  a grid the easiest way.... parsed into a List <xxxxx>
			list = Utils . GetDataDridRowsAsListOfStrings ( dt );
			foreach ( string row in list )
			{
				string entry = row.ToUpper();
				// ONLY ALLOW THE TRHEE MAIN dB'S WE ARE LIKELY TO USE.
				if ( entry . Contains ( "IAN" ) || entry . Contains ( "NORTHWIND" ) || entry . Contains ( "PUBS" ) )
				{
					DbMain . Items . Add ( row );
					if ( row . ToUpper ( ) . Contains ( "IAN1" ) )
						bankindex = count;
					count++;
				}
			}
			// how to Sort Combo/Listbox contents
			DbMain . Items . SortDescriptions . Add ( new SortDescription ( "" , ListSortDirection . Ascending ) );
			DbMain . SelectedIndex = bankindex;
			SqlCommand = DefaultSqlCommand;
		}
		public void LoadDefaultSqlCommands ( )
		{
			if ( DefaultSqlCommands . Count > 0 )
				return;
			DefaultSqlCommands . Add ( "BANKACCOUNT" , "Select * from BankAccount" );
			DefaultSqlCommands . Add ( "CUSTOMER" , "Select * from Customer" );
			DefaultSqlCommands . Add ( "SECACCOUNTS" , "Select * from secAccounts" );
			DefaultSqlCommands . Add ( "NORTHWIND" , "Select * from Customers" );
			DefaultSqlCommands . Add ( "AUTHORS" , "Select * from Authors" );
			//DefaultSqlCommands . Add ( "AW.SALES.CREDITCARD" , "Select * from Sales.CreditCard" );
			//DefaultSqlCommands . Add ( "AW.SALES.SALESPERSON" , "Select * from Sales.Salesperson" );
			//DefaultSqlCommands . Add ( "AW.SALES.SALESTERRITORY" , "Select * from Sales.SalesTerritory" );
			//DefaultSqlCommands . Add ( "AW.PRODUCTION.PRODUCTREVIEW" , "Select * from Production.ProductReview" );
			//DefaultSqlCommands . Add ( "AW.PRODUCTION.PRODUCTPHOTO" , "Select * from Production.ProductPhoto" );
		}                 // Set up the connection string fo rthe approriate Db
		private string GetDefaultSqlCommand ( string CurrentType )
		{
			string result="";
			foreach ( KeyValuePair<string , string> pair in DefaultSqlCommands )
			{
				if ( pair . Key == CurrentType . ToUpper ( ) )
				{
					result = pair . Value;
					SqlCommand = result;
					break;
				}
			}
			return result;
		}
		#region Make main Db Connectiions
		private bool OpenIan1Db ( )
		{
			//Set Sql Connection string up first
			if ( Utils . CheckResetDbConnection ( "IAN1" , out string constr ) == false )
			{
				Debug. WriteLine ( "Failed to set connection string for Ian1 Db" );
				return false;
			}
			// Open the Ian1 Db first off
			SqlCommand = "spOpenDb_Ian1";
			if ( SqlSupport . Executestoredproc ( SqlCommand , Flags . CurrentConnectionString ) == false )
			{
				Debug. WriteLine ( $"Stored procedure {SqlCommand} Failed to open IAN1.MDF" );
				Utils . DoErrorBeep ( 250 , 75 , 1 );
				SqlCommand = DefaultSqlCommand;
				return false;
			}
			SqlCommand = DefaultSqlCommand;
			return true;
		}
		private void OpenNorthWindDb ( )
		{
			//Set Sql Connectoin string up first
			if ( Utils . CheckResetDbConnection ( "NORTHWIND" , out string constr ) == false )
			{
				Debug. WriteLine ( "Failed to set connection string for NorthWind Db" );
				return;
			}
			// Open the NorthWind Db first off
			SqlCommand = "spOpenDb_NorthWind";
			SqlSupport . Executestoredproc ( SqlCommand , Flags . CurrentConnectionString );
			// now load list of tabels in Northwind Db into dbMain Combo
			LoadDbTables ( "NORTHWIND" );
			SqlCommand = DefaultSqlCommand;
		}
		private void OpenPublishers ( )
		{
			//Set Sql Connectoin string up first
			if ( Utils . CheckResetDbConnection ( "PUBS" , out string constr ) == false )
			{
				Debug. WriteLine ( "Failed to set connection string for Adventure WorksDb" );
				return;
			}
			// Open the Adventureworks Db first off
			SqlCommand = "spOpenDb_Publishers";
			SqlSupport . Executestoredproc ( SqlCommand , Flags . CurrentConnectionString );
			// now load list of tabels in Northwind Db
			LoadDbTables ( "PUBS" );
			SqlCommand = "Select * fom Authors order by au_fname";
		}
		#endregion Make main Db Connectiions

		//Get list of all Tables in our Db (Ian1.MDF)
		public void LoadTablesList ( string domain = "" )
		{
			int bankindex = 0, count=0;
			List<string> list = new List<string>      ();
			DbListbox . Items . Clear ( );
			SqlCommand = $"spGetTablesList";
			CallStoredProcedure ( list , SqlCommand );
			//This call returns us a DataTable
			DataTable dt = DataLoadControl . GetDataTable ( SqlCommand );
			//			Grid2 . ItemsSource = dt . DefaultView;
			//			Grid2 . Refresh ( );
			// This how to access  Row data from  a grid the easiest way.... parsed into a List <xxxxx>
			list = Utils . GetDataDridRowsAsListOfStrings ( dt );
			foreach ( string row in list )
			{
				DbListbox . Items . Add ( row );
				if ( row . ToUpper ( ) == "BANKACCOUNT" )
					bankindex = count;
				count++;
			}
			// how to Sort Combo/Listbox contents
			DbListbox . Items . SortDescriptions . Add ( new SortDescription ( "" , ListSortDirection . Ascending ) );
			DbListbox . SelectedIndex = bankindex;
		}
		private void ReloadListbox ( object sender , RoutedEventArgs e )
		{
			//ResetViewers ( "BOX" );
			//listBox . ItemsSource = null;
			//listBox . Refresh ( );
			//DbCountlb = 0;
			//// Set flag  to ignore limits check
			//LoadAll = true;
			//string currdb = GetCurrentDatabase ( );
			CurrentTableName = DbListbox . SelectedItem . ToString ( ) . ToUpper ( );
			if ( Utils . CheckResetDbConnection ( DbMain . SelectedItem . ToString ( ) , out string constr ) == false )
			{
				Debug. WriteLine ( $"Failed to set connection string for {CurrentTableName . ToUpper ( )} Db" );
				return;
			}

			if ( CurrentTableName == "IAN1" )
			{
				LoadData_Ian1 ( "BOX" );
				//				LoadGrid_IAN1 ( "BOX" );
			}
			else if ( CurrentTableName == "NORTHWIND" )
			{
				LoadData_NorthWind ( "BOX" );
				//				LoadGrid_NORTHWIND ( "BOX" );
			}
			else if ( CurrentTableName == "PUBS" )
			{
				genaccts = null;
				//LoadData_Publishers ( "BOX" , out genaccts );
				//if ( genaccts != null )
				//{
				//	listBox . ItemsSource = genaccts;
				//	SqlServerCommands . LoadActiveRowsOnlyInGrid ( dGrid , genaccts , SqlServerCommands . GetGenericColumnCount ( genaccts ) );
				//	dGrid . SelectedItem = dGrid . SelectedIndex = 0;
				//	DbCountlb = genaccts . Count;
				//}
				//else
				//{
				//	pubauthors = PubAuthors . LoadPubAuthors ( pubauthors , false );
				//	listBox . ItemsSource = pubauthors;
				//	dGrid . ItemsSource = pubauthors;
				//	dGrid . SelectedItem = dGrid . SelectedIndex = 0;
				//	DbCountlb = pubauthors . Count;
				//}
				//LoadGrid_PUBS ( "BOX" );
			}
			BankDataGrid . SelectedIndex = 0;
			BankDataGrid . SelectedItem = 0;
			//LoadAll = false;
		}

		public void LoadData_NorthWind ( string viewertype )
		{
			if ( Usetimer )
				timer . Start ( );
			if ( CurrentTableName == "CUSTOMERS" )
			{

				//				LoadDataTemplates_NorthWind ( "CUSTOMERS" , viewertype );
				nwcustomeraccts = new ObservableCollection<nwcustomer> ( );
				nwcustomeraccts = nwc . GetNwCustomers ( );
				// need to do this cos the SQL command is changed to load the tables list....
				//if ( Startup )
				//	SqlCommand = DefaultSqlCommand;
			}
			else if ( CurrentTableName == "ORDERS" )
			{

				//				LoadDataTemplates_NorthWind ( "ORDERS" , viewertype );
				nworderaccts = new ObservableCollection<nworder> ( );
				nworderaccts = nwo . LoadOrders ( );
				// need to do this cos the SQL command is changed to load the tables list....
				//if ( Startup )
				//	SqlCommand = DefaultSqlCommand;
			}
			else
			{
				string tablename="";
				//				LoadDataTemplates_Ian1 ( "GENERIC" , viewertype );
				genaccts = new ObservableCollection<GenericClass> ( );
				// need to do this cos the SQL command is changed to load the tables list....
				//if ( viewertype == "VIEW" )
				//	tablename = dbNameLv . SelectedItem . ToString ( );
				//else
				tablename = DbListbox . SelectedItem . ToString ( );
				SqlCommand = GetDefaultSqlCommand ( CurrentTableName );
				if ( SqlCommand == "" )
					SqlCommand = $"Select * from {tablename}";
				// need to do this cos the SQL command is changed to load the tables list....
				//if ( Startup )
				//	SqlCommand = DefaultSqlCommand;
				string ResultString="";
				genaccts = SqlSupport . LoadGeneric ( SqlCommand , out ResultString , 0 , true );
			}
		}
		public void LoadData_Publishers ( string viewertype , out ObservableCollection<GenericClass> generics )
		{
			generics = null;
			//ObservableCollection<GenericClass> generics = new ObservableCollection<GenericClass>();
			//			if ( Usetimer )
			//				timer . Start ( );
			if ( CurrentTableName == "AUTHORS" )
			{
				//				LoadDataTemplates_PubAuthors ( "AUTHORS" , viewertype );
				pubauthors = new ObservableCollection<PubAuthors> ( );
				// need to do this cos the SQL command is changed to load the tables list....
				//				if ( Startup )
				//					SqlCommand = DefaultSqlCommand;
				//				nwcustomeraccts = SqlSupport . LoadBank ( SqlCommand , 0 , true );
			}
			else if ( CurrentTableName == "ORDERS" )
			{

				//				LoadDataTemplates_NorthWind ( "ORDERS" , viewertype );
				nworderaccts = new ObservableCollection<nworder> ( );
				// need to do this cos the SQL command is changed to load the tables list....
				//				if ( Startup )
				//					SqlCommand = DefaultSqlCommand;
				//				nwcustomeraccts = SqlSupport . LoadBank ( SqlCommand , 0 , true );
			}
			else
			{
				string tablename="";
				//				LoadDataTemplates_Ian1 ( "GENERIC" , viewertype );
				genaccts = new ObservableCollection<GenericClass> ( );
				// need to do this cos the SQL command is changed to load the tables list....
				//if ( viewertype == "VIEW" )
				//	tablename = dbNameLv . SelectedItem . ToString ( );
				//else
				tablename = DbListbox . SelectedItem . ToString ( );
				SqlCommand = $"Select *from {tablename}";
				SqlCommand = GetDefaultSqlCommand ( CurrentTableName );
				if ( SqlCommand == "" )
					SqlCommand = $"Select * from {tablename}";
				// need to do this cos the SQL command is changed to load the tables list....
				//if ( Startup )
				//	SqlCommand = DefaultSqlCommand;
				string ResultString="";
				genaccts = SqlSupport . LoadGeneric ( SqlCommand , out ResultString , 0 , true );
				if ( genaccts . Count > 0 )
				{
					//ShowInfo ( line1: $"The requested table [{CurrentTableName }] was loaded successfully, and {genaccts . Count} records were returned,\nThe data is shown in  the viewer below" , clr1: "Black0" ,
					//	line2: $"The command line used was" , clr2: "Red2" ,
					//	line3: $"{SqlCommand . ToUpper ( )}" , clr3: "Blue4" ,
					//	header: "Generic style data table" , clr4: "Red5" );
				}
				else
				{
					//ShowInfo ( line1: $"The requested table [{CurrentTableName }] was loaded successfully, but ZERO records were returned,\nThe Table is  " , clr1: "Black0" ,
					//	line2: $"The command line used was" , clr2: "Red2" ,
					//	line3: $"{SqlCommand . ToUpper ( )}" , clr3: "Blue4" ,
					//	header: "Generic style data table" , clr4: "Red5" );
				}
				generics = genaccts;
			}
		}
		// load a list of all SP's
		private void LoadSpList ( )
		{
			List<string> SpList = new List<string>();
			SpList = CallStoredProcedure ( SpList , "spGetStoredProcs" );
			//Storedprocs . ItemsSource = SpList;
			//Storedprocs . Items . SortDescriptions . Add ( new SortDescription ( "" , ListSortDirection . Ascending ) );
			//Storedprocs . SelectedIndex = 0;
			//Storedprocs . SelectedItem = 0;
			//Storedprocs . Refresh ( );

		}



		#region Trigger methods  for Stored Procedures (string, Int, Double, Decimal) that return a List<xxxxx>
		// These all return just a single column from any table by calling a Stored Procedure  in MSSQL Server
		public static List<string> CallStoredProcedure ( List<string> list , string sqlcommand )
		{
			//This call returns us a DataTable
			DataTable dt = DataLoadControl . GetDataTable ( sqlcommand );
			if ( dt != null )
				list = Utils . GetDataDridRowsAsListOfStrings ( dt );
			return list;
		}
		public static List<int> CallStoredProcedure ( List<int> list , string sqlcommand )
		{
			//This call returns us a DataTable
			DataTable dt = DataLoadControl . GetDataTable ( sqlcommand );
			list = Utils . GetDataDridRowsAsListOfInts ( dt );
			return list;
		}
		public static List<double> CallStoredProcedure ( List<double> list , string sqlcommand )
		{
			//This call returns us a DataTable
			DataTable dt = DataLoadControl . GetDataTable ( sqlcommand );
			list = Utils . GetDataDridRowsAsListOfDoubles ( dt );
			return list;
		}
		public static List<decimal> CallStoredProcedure ( List<decimal> list , string sqlcommand )
		{
			//This call returns us a DataTable
			DataTable dt = DataLoadControl . GetDataTable ( sqlcommand );
			list = Utils . GetDataDridRowsAsListOfDecimals ( dt );
			return list;
		}
		public static List<DateTime> CallStoredProcedure ( List<DateTime> list , string sqlcommand )
		{
			//This call returns us a DataTable
			DataTable dt = DataLoadControl . GetDataTable ( sqlcommand );
			list = Utils . GetDataDridRowsAsListOfDateTime ( dt );
			return list;
		}
		#endregion Trigger methods  for Stored Procedures

		#region Load List of Tables in current Db

		//Get list of all Tables in currently selected Db 
		public bool LoadDbTables ( string DbName )
		{
			int listindex = 0, count=0;
			List<string> list = new List<string>      ();
			DbName = DbName . ToUpper ( );
			if ( Utils . CheckResetDbConnection ( DbName , out string constr ) == false )
			{
				Debug. WriteLine ( $"Failed to set connection string for {DbName} Db" );
				return false;
			}
			// All Db's have their own version of this SP.....
			SqlCommand = "spGetTablesList";

			Datagrids . CallStoredProcedure ( list , SqlCommand );
			//This call returns us a DataTable
			DataTable dt = DataLoadControl . GetDataTable ( SqlCommand );
			// This how to access Row data from  a grid the easiest way.... parsed into a List <xxxxx>
			if ( dt != null )
			{
				//				dbNameLb . Items . Clear ( );
				//				dbNameLv . Items . Clear ( );
				list = Utils . GetDataDridRowsAsListOfStrings ( dt );
				if ( DbName == "NORTHWIND" )
				{
					foreach ( string row in list )
					{
						//						dbNameLb . Items . Add ( row );
						//						dbNameLv . Items . Add ( row );
						if ( row . ToUpper ( ) == CurrentTableName )
							listindex = count;
						count++;
					}
				}
				else if ( DbName == "IAN1" )
				{
					foreach ( string row in list )
					{
						//						dbNameLb . Items . Add ( row );
						//						dbNameLv . Items . Add ( row );
						if ( row . ToUpper ( ) == CurrentTableName )
							listindex = count;
						count++;
					}
				}
				else if ( DbName == "PUBS" )
				{
					foreach ( string row in list )
					{
						//						dbNameLb . Items . Add ( row );
						//						dbNameLv . Items . Add ( row );
						if ( row . ToUpper ( ) == CurrentTableName )
							listindex = count;
						count++;
					}
				}
			}
			//// add ALL DBs to our treeview list of databases
			////SqlTables sqlt = new SqlTables();
			//				foreach ( string row in list )
			//				{
			////					SqlTables sqlt= new SqlTables();
			////					sqlt . tablename = row;
			//					SqlTableCollection . Add ( sqlt );
			//				}
			//				BankDataGrid . ItemsSource = SqlTableCollection;
			//DbTablesTree . ItemsSource = SqlTableCollection;


			// how to Sort Combo/Listbox contents
			//dbNameLv . Items . SortDescriptions . Add ( new SortDescription ( "" , ListSortDirection . Ascending ) );
			//	alldone = true;
			//	//dbNameLb . SelectedIndex = listindex;
			//	//dbNameLv . SelectedIndex = listindex;
			//	alldone = false;
			//	if ( count > 0 )
			//		return true;
			//	else
			//		return false;
			//}
			//else
			//{
			//	MessageBox . Show ( $"SQL comand {SqlCommand} Failed..." );
			//	Utils . DoErrorBeep ( 125 , 55 , 1 );
			//	return false;
			//}
			return true;
			//SqlCommand = DefaultSqlCommand;
		}

		#endregion Load List of Tables in current Db


		//#region FlowDoc support
		//private void ShowInfo ( string line1 = "" , string clr1 = "" , string line2 = "" , string clr2 = "" , string line3 = "" , string clr3 = "" , string header = "" , string clr4 = "" , bool beep = false )
		//{
		//	if ( UseFlowdoc == false )
		//		return;
		//	if ( UseFlowdocBeep == false )
		//		beep = false;
		//	Flowdoc . ShowInfo ( line1 , clr1 , line2 , clr2 , line3 , clr3 , header , clr4 , beep );
		//	canvas . Visibility = Visibility . Visible;
		//	canvas . BringIntoView ( );
		//	Flowdoc . Visibility = Visibility . Visible;

		//	if ( Flowdoc . KeepSize == false )
		//	{
		//		if ( Flowdoc . Height != flowdocFloatingHeight )
		//			flowdocFloatingHeight = Flowdoc . Height;
		//	}
		//	var docheight = Convert.ToDouble ( flowdocFloatingHeight == 0 ? 250 : flowdocFloatingHeight);
		//	var docwidth = Convert.ToDouble ( flowdocFloatingWidth == 0 ? 450 : flowdocFloatingWidth);
		//	// Save properties
		//	flowdocFloatingHeight = docheight;
		//	flowdocFloatingWidth = docwidth;
		//	flowdocFloatingTop = Convert . ToDouble ( Flowdoc . GetValue ( TopProperty ) );
		//	flowdocFloatingLeft = Convert . ToDouble ( Flowdoc . GetValue ( LeftProperty ) );
		//	//Position Control on Canvas
		//	double canvasheight = canvas.ActualHeight;
		//	if ( docheight >= canvasheight )
		//		Flowdoc . Height = canvasheight - 20;
		//	// Set size of Control
		//	Flowdoc . Width = flowdocFloatingWidth;
		//	Flowdoc . Height = flowdocFloatingHeight;

		//	Flowdoc . BringIntoView ( );
		//	if ( Flags . PinToBorder == true )
		//	{
		//		( Flowdoc as FrameworkElement ) . SetValue ( Canvas . LeftProperty , ( double ) 0 );
		//		( Flowdoc as FrameworkElement ) . SetValue ( Canvas . TopProperty , ( double ) 0 );
		//	}
		//}

		#region  Dependency properties
		public double Fontsize
		{
			get { return ( double ) GetValue ( FontsizeProperty ); }
			set { SetValue ( FontsizeProperty , value ); }
		}
		public static readonly DependencyProperty FontsizeProperty =
			DependencyProperty.Register("Fontsize", typeof(double), typeof(MulltiDbUserControl), new PropertyMetadata((double)12));

		public double ItemsHeight
		{
			get { return ( double ) GetValue ( ItemsHeightProperty ); }
			set { SetValue ( ItemsHeightProperty , value ); }
		}
		public static readonly DependencyProperty ItemsHeightProperty =
			DependencyProperty.Register("ItemsHeight", typeof(double), typeof(MulltiDbUserControl), new PropertyMetadata((double)20));


		public int RecordsCount
		{
			get { return ( int ) GetValue ( RecordsCountProperty ); }
			set { SetValue ( RecordsCountProperty , value ); }
		}

		// Using a DependencyProperty as the backing store for RecordsCount.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty RecordsCountProperty =
		    DependencyProperty.Register("RecordsCount", typeof(int), typeof(MulltiDbUserControl), new PropertyMetadata(0));


		#endregion  Dependency properties
		private void DbMain_PreviewMouseRightButtonUp ( object sender , MouseButtonEventArgs e )
		{

		}

		private void ReloadFromDomain ( object sender , RoutedEventArgs e )
		{
			if ( IsSelectionChanged == true )
				return;

			var  domain = DbMain.SelectedItem;
			if ( Utils . CheckResetDbConnection ( domain . ToString ( ) , out string constr ) == false )
			{
				Debug. WriteLine ( $"Failed to set connection string for {domain}" );
				return;
			}
			DbListbox . Items . Clear ( );
			LoadTablesList ( domain . ToString ( ) );
			
			//IsSelectionChanged = true;
		}

		private void DbMain_SelectionChanged ( object sender , SelectionChangedEventArgs e )
		{
			IsSelectionChanged = false;
			ReloadFromDomain ( sender , null );
			IsSelectionChanged = true;
		}

		private void MutliDbCtrl_IsVisibleChanged ( object sender , DependencyPropertyChangedEventArgs e )
		{
			this . Width += 1;
			this . UpdateLayout ( );
			this . Refresh ( );
		}
	}
}
