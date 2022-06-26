using NewWpfDev . Dapper;
using NewWpfDev . Models;
using NewWpfDev . Sql;
using NewWpfDev . UserControls;
using NewWpfDev . Views;

using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Data . SqlClient;
using System . Diagnostics;
using System . Linq;
using System . Security . Policy;
using System . Text;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Data;
using System . Windows . Input;
using System . Windows . Markup;
using System . Xml . Linq;

namespace NewWpfDev . ViewModels
{
	/// <summary>
	/// VIEWMODEL for MvvmDataGrid.
	///  Also sends lower level requests to MvvmGridView for data access etc
	///   This only handles ICommands and Binding variables for the View itself
	/// </summary>
	public class MvvmViewModel
	{
		#region NotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged ( string propertyName )
		{
			if ( PropertyChanged != null )
			{
				PropertyChanged ( this , new PropertyChangedEventArgs ( propertyName ) );
			}
		}
		#endregion NotifyPropertyChanged
		internal static MvvmGenericModel mvvm { get; set; }
		public static MvvmGridModel mvgm { get; set; }
		public static MvvmDataGrid ParentBGView { get; set; }
		public static FlowDoc Flowdoc { get; set; }
		public static FlowdocLib fdl { get; set; }
		public static Canvas canvas { get; set; }
		public static bool IsBankActive { get; set; } = true;

		public ICommand Debugger { get; set; }
		public ICommand CloseWindow { get; set; }
		public ICommand LoadData { get; set; }
		public ICommand GetColumnNames { get; set; }



		private  bool UseFlowdoc = true;
		public static object MovingObject { get; set; }

		#region Full Properties
		private string fillterlabel;
		public string FilterLabel
		{
			get { return fillterlabel; }
			set { fillterlabel = value; NotifyPropertyChanged ( FilterLabel ); }
		}
		private string  acFilterLabel;
		public string AcFilterLabel
		{
			get { return acFilterLabel; }
			set { acFilterLabel = value; NotifyPropertyChanged ( AcFilterLabel ); }
		}
		#region Filter TextBoxes
		private string  filtertextbox;
		public string FilterTextBox
		{
			get { return filtertextbox; }
			set { filtertextbox = value; NotifyPropertyChanged ( FilterTextBox ); }
		}
		private string  acFilterTextBox;
		public string ACFilterTextBox
		{
			get { return acFilterTextBox; }
			set { acFilterTextBox = value; NotifyPropertyChanged ( ACFilterTextBox ); }
		}
		#endregion Filter TextBoxes

		private string  loadbuttontext;
		public string LoadButtonText
		{
			get { return loadbuttontext; }
			set { loadbuttontext = value; NotifyPropertyChanged ( LoadButtonText ); }
		}
		private string activeTable;
		public string ActiveTable
		{
			get { return activeTable; }
			set { activeTable = value; NotifyPropertyChanged ( ActiveTable ); }
		}

		#endregion Full Properties
		public MvvmViewModel ( )
		{
			// Handle ICommands
			Debugger = new RelayCommand ( ExecuteDebugger , CanExecuteDebugger );
			LoadData = new RelayCommand ( ExecuteLoadData , CanExecuteLoadData );
			CloseWindow = new RelayCommand ( ExecuteCloseWindow , CanExecuteCloseWindow );
			GetColumnNames = new RelayCommand ( ExecuteGetColumnNames , CanExecuteGetColumnNames );
//			RefreshListbox = new RelayCommand ( ExecuteRefreshListbox , CanRefreshListbox );
			CloseWindow = new RelayCommand ( ExecuteCloseWindow , CanExecuteCloseWindow );
			LoadButtonText = "Load Customer Data";

		}
        #region FlowDoc methods
        // Allows this class to control maximizing FlowDoc window
        public event EventHandler ExecuteFlowDocMaxmizeMethod;
		protected virtual void OnExecuteMethod ( )
		{
			if ( ExecuteFlowDocMaxmizeMethod != null )
				ExecuteFlowDocMaxmizeMethod ( this , EventArgs . Empty );
		}
		private void Image_PreviewMouseLeftButtonUp ( object sender , MouseButtonEventArgs e )
		{
			//allows remote window to maximize /resize  this control ?
			OnExecuteMethod ( );
		}
		#endregion FlowDoc methods

		public MvvmViewModel ( object caller )
		{
			// Get pointers  to  View and Model Class
			ParentBGView = caller as MvvmDataGrid;
			mvgm = new MvvmGridModel ( this );

			//Setup Edit label annd button text as it switches from Bank to Customer
			FilterLabel = "Filter Bank A/c's Col : CustNo";
			LoadButtonText = "Load Customer A/cs";
			ActiveTable = "All Bank Accounts";
			// Handle  filtering by calling MvvmGridModel to process it
			ParentBGView . filtertext . TextChanged += mvgm . filter_TextChanged;
			ParentBGView . acfiltertext . TextChanged += mvgm . acfilter_TextChanged;

			fdl = new FlowdocLib ( );
		}

 		#region ICommand Methods 
		// ICommand CanExecute's
		private bool CanExecuteLoadData ( object arg )
		{ return true; }
		public bool CanExecuteDebugger ( object arg )
		{return true;}
		public bool CanExecuteCloseWindow ( object parameter )
		{ return true; }
		public  bool CanExecuteGetColumnNames ( object arg )
		{return true;}

		// IComand Handelrs
		public void ExecuteDebugger ( object obj )
		{ //
		  //mvgm . ExecuteDebugger ( null );
		 }

		public void ExecuteGetColumnNames ( object obj )
		{
			int indx = 0;
			List<string> list = new List<string>();
			ObservableCollection<GenericClass> GenericClass = new ObservableCollection<GenericClass>();
			Dictionary<string, string> dict = new Dictionary<string, string>();
			// This returns a Dictionary<sting,string> PLUS a collection  and a List<string> passed by ref....
			List<int> VarCharLength  = new List<int>();
//			IsBankActive = ( bool ) obj;
			if ( IsBankActive == false )
				dict = GenericDbUtilities . GetDbTableColumns ( ref GenericClass , ref list , "Customer" , "IAN1" , ref VarCharLength );
			else
				dict = GenericDbUtilities . GetDbTableColumns ( ref GenericClass , ref list , "BankAccount" , "IAN1" , ref VarCharLength );
			
			indx = 0;
			if ( VarCharLength . Count > 0 )
			{
				foreach ( var item in GenericClass )
				{
					item . field3 = VarCharLength [ indx++ ] . ToString ( );
				}
			}
			if ( ParentBGView != null )
			{
				SqlServerCommands . LoadActiveRowsOnlyInGrid ( ParentBGView . dataGrid2 , GenericClass , DapperSupport . GetGenericColumnCount ( GenericClass ) );
				indx = 0;
				foreach ( var col in ParentBGView . dataGrid2 . Columns )
				{
					if ( indx == 0 )
						col . Header = "Field Name";
					else if ( indx == 1 )
						col . Header = "SQL Field Type";
					else if ( indx == 2 )
					{
						col . Header = "NVarChar Length";
					}
					indx++;
				}
			}
			if ( VarCharLength . Count > 0 )
			{
				string output = "";
				indx = 0;
				foreach ( var item in GenericClass )
				{
					item . field3 = VarCharLength [ indx++ ] . ToString ( );
					output += item . field1 . ToString ( ) + ", " + item . field2 . ToString ( ) + ", " + item . field3 + "\n";
				}
				fdmsg ( output,"","");
			}
		}

		public void ExecuteCloseWindow ( object parameter )
		{
			//			Debug. WriteLine ( "We have Hit the close ICommand ..." );
			WindowCollection  v = Application .Current.Windows;
			foreach ( Window item in v )
			{
				if ( item . ToString ( ) . Contains ( "MvvmDataGrid" )
					|| item . ToString ( ) . Contains ( "GenericMvvmWindow" ) )
				{
					MessageBoxResult res = MessageBox . Show ( "Close App down entirely ?" , "Application Closedown Options" , MessageBoxButton . YesNoCancel , MessageBoxImage. Question , MessageBoxResult . Yes );
					if ( res == MessageBoxResult . Yes )
						Application . Current . Shutdown ( );
					else if ( res == MessageBoxResult . No )
						item . Close ( );

					break;
				}
			}
			//Application . Current . Shutdown ( );
		}

		private void ExecuteLoadData ( object IsBankactive )
		{
			ParentBGView . filtertext . Text = "";
			if ( ( bool ) IsBankActive == true )
			{
				// Call MvvmGridModel to actually get the Customer data from SQL Db
				IsBankActive = false;
				mvgm . LoadData ( IsBankActive );
				// Reset labels & button text in filter box
				FilterLabel = "Filter Customer A/c's Col : CustNo";
				LoadButtonText = "Load Bank A/cs";
				ActiveTable = "All Customer Details";
				IsBankActive = false;
			}
			else
			{
				// Call MvvmGridModel to actually get the Bank data from SQL Db
				IsBankActive = true;
				mvgm . LoadData ( IsBankActive );
				FilterLabel = "Filter Bank A/c's Col : CustNo";
				LoadButtonText = "Load Customer A/cs";
				ActiveTable = "All Bank Accounts";
				IsBankActive = true;
			}
		}
		#endregion ICommand Methods CANEXECUTExxxxx, EXECUTExxxxxx

		public void ShowRecordData ( DataGrid dgrid )
		{
			if ( IsBankActive )
			{
				BankAccountViewModel bvm = dgrid.SelectedItem as   BankAccountViewModel;
				string data = "Record Contents :-\n";
				data += "Customer # :	" + bvm . CustNo + "\n";
				data += "Bank A/C  # :	" + bvm . BankNo + "\n";
				data += "A/c Type :	" + bvm . AcType + "\n";
				data += "Balance :	" + bvm . Balance + "\n";
				data += "Interest  rate :	" + bvm . IntRate + "\n";
				data += "Date opened :	" + bvm . ODate + "\n";
				data += "Date Closed:	" + bvm . CDate + "\n";
				fdmsg ( "testing Short form of flowdoc (fdl.FdMsg) from MvvmViewModel" , data );
			}
            else
            {

            }

		}
		public void fdmsg ( string line1 , string line2 = "" , string line3 = "" )
		{
			//We have to pass the Flowdoc.Name, and Canvas.Name as well as up   to 3 strings of message
			//  you can  just provie one if required
			// eg fdmsg("message text");
			fdl . FdMsg ( Flowdoc , canvas , line1 , line2 , line3 );
		}

		private void ShowInfo ( FlowDoc Flowdoc , Canvas canvas , string line1 = "" , string clr1 = "" , string line2 = "" , string clr2 = "" , string line3 = "" , string clr3 = "" , string header = "" , string clr4 = "" , bool beep = false )
		{
			if ( UseFlowdoc == false )
				return;
			Flowdoc . ShowInfo ( Flowdoc , canvas , line1 , clr1 , line2 , clr2 , line3 , clr3 , header , clr4 , beep );

		}
	}
}
