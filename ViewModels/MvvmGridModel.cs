using System;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Diagnostics;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Data;
using System . Windows . Input;

using NewWpfDev . Views;

using Application = System . Windows . Application;
using Cursors = System . Windows . Input . Cursors;
using MessageBox = System . Windows . MessageBox;

namespace NewWpfDev . ViewModels
{
    /// <summary>
    /// This is the main MODEL for BankGridView.XAML 
    /// It handles all requests for processing from the VIEWMODEL MvvmViewModel
    /// In out turn, retun the data to the VIEWMODEL
    ///		BankAccountViewModel and CustomerViewModel
    ///	that provide SQL support for Bank and Customer data retrieval
    /// </summary>
    public class MvvmGridModel 
	{
		#region OnPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged ( string propertyName )
		{
			if ( PropertyChanged != null )
			{
				PropertyChanged ( this , new PropertyChangedEventArgs ( propertyName ) );
			}
		}
		#endregion OnPropertyChanged

		#region Declarations
		// Db SQL handles
		public static BankAccountViewModel bvm { get; set; }
		public static CustomerViewModel cvm { get; set; }
		// Db Observable Collections
		public static ObservableCollection<BankAccountViewModel> BankCollection { get; set; } = new ObservableCollection<BankAccountViewModel> ( );
		public static ObservableCollection<CustomerViewModel> CustCollection { get; set; } = new ObservableCollection<CustomerViewModel> ( );

		// Db Collection Viewss
		public static CollectionView BankCollectionView { get; set; }
		public static CollectionView CustCollectionView { get; set; }

		// Pointer to the parent window
		private MvvmDataGrid ParentBGView;
		private MvvmViewModel mvvm;

		// Sundry variables used in processing
		private const string ValidNumbers="0123456789";
		private static int CurrentBankSelection { get; set; } = 0;
		private static int CurrentCustSelection { get; set; } = 0;
		//private static bool IsBankActive { get; set; } = false;
		private static bool GridLoading { get; set; } = false;
		private bool IsBankActive { get; set; }

		#endregion Declarations

		#region  ICommand declarations
		public ICommand debugger { get; set; }

		#endregion  ICommand declarations

		#region Setup
		public MvvmGridModel ( object caller )
		{
			Mouse . OverrideCursor = Cursors . Wait;

			mvvm = caller as MvvmViewModel;
			ParentBGView = MvvmViewModel . ParentBGView;
			IsBankActive = false;
			// Hook into datagrid selection changes
			ParentBGView . dataGrid . SelectionChanged += dataGrid_SelectionChanged;

			// almost Finally, create pointer for Bank and Customer Records :  *needed for selection changes"
			bvm = new BankAccountViewModel ( );
			cvm = new CustomerViewModel ( );
			// Finally, Load Bank Account Data
			IsBankActive = true;
			LoadData ( IsBankActive );
			Mouse . OverrideCursor = Cursors . Arrow;
		}

		#endregion Setup

		#region ICommand CanExecute handlers
		private bool CanExecuteDebugger ( object arg )
		{ return true; }
		public bool CanExecuteCloseWindow ( object arg )
		{ return true; }
		public void ExecuteCloseWindow ( object arg )
		{
			Debug. WriteLine ( "We have Hit the close ICommand in MvvmGridModel..." );
			WindowCollection  v = Application .Current.Windows;
			foreach ( Window item in v )
			{
				if ( item . Name == "BankGV" )
				{
					MessageBoxResult res = MessageBox . Show ( "Close App down entirely ?" , "Application Closedown Options" ,
						MessageBoxButton .YesNoCancel, MessageBoxImage.Question, MessageBoxResult . Yes );
					if ( res == MessageBoxResult . Cancel )
						return;
					if ( res == MessageBoxResult . Yes )
						Application . Current . Shutdown ( );
					else
						item . Close ( );
					break;
				}
			}
		}
		private bool CanExecuteLoadData ( object arg )
		{ return true; }
		private bool CanExecuteLoadBank ( object arg )
		{
			var v =ParentBGView. dataGrid . SelectedItem as BankAccountViewModel;
			if ( v != null )
				return false;
			else
				return true;
			
#pragma warning disable CS0162 // Unreachable code detected
			if ( IsBankActive == false )
			{
				ParentBGView . filtertext . IsEnabled = true;
				if ( IsBankActive )
					return false;
				else
					return true;
			}
			else
			{
				return false;
			}
#pragma warning restore CS0162 // Unreachable code detected
		}
		private bool CanExecuteLoadCustomers ( object arg )
		{
			var v = ParentBGView.dataGrid . SelectedItem as CustomerViewModel;
			if ( v != null )
				return false;
			else
				return true;

#pragma warning disable CS0162 // Unreachable code detected
			if ( IsBankActive == true )
			{
				if ( IsBankActive )
					return true;
				else
					return false;
			}
			else
			{
				ParentBGView . filtertext . IsEnabled = true;
				return false;
			}
#pragma warning restore CS0162 // Unreachable code detected
			return false;
		}

		#endregion ICommand CanExecute handlers

		#region IComand full Exection Handlers
		public void ExecuteDebugger ( object obj )
		{
			Debugger . Break ( );
		}
		//==============================//
		// ICommand method  to Load Bank Data  using our Root level Classes (BankAccountViewModel & CustomerViewModel)
		//==============================//
		public string  LoadData ( bool obj )
		{
			Mouse . OverrideCursor = Cursors . Wait;
			IsBankActive =  obj;
			if ( IsBankActive == true )
			{
				BankCollection . Clear ( );
				BankCollection = bvm . GetBankAccounts ( BankCollection , "Select * from BankAccount" , false , "BankGridView" );
				BankCollectionView = CollectionViewSource . GetDefaultView ( BankCollection ) as CollectionView;
				//Set the fillter we want up  This is called for each record in the collection
				//2 ways of doing these
				//BankCollectionView . Filter = new Predicate<object> ( ( fltac ) => FilterBankACData ( fltac as BankAccountViewModel ) );
				BankCollectionView . Filter += new Predicate<object> ( ( flt ) => FilterBankACData ( flt as BankAccountViewModel , EventArgs . Empty ) );
				
				ParentBGView . dataGrid . ItemsSource = BankCollectionView;
				GridLoading = true;
				ParentBGView . dataGrid . SelectedIndex = CurrentBankSelection;
				ParentBGView . dataGrid . SelectedItem = CurrentBankSelection;

				// NB These need to be called in this order to get the record to be displayed in the DataGrid
				ParentBGView . dataGrid . ScrollIntoView ( ParentBGView . dataGrid . SelectedIndex );
				ParentBGView . dataGrid . BringIntoView ( );
				GridLoading = false;
				IsBankActive = true;
				ParentBGView . RecordCount . Text = ParentBGView . dataGrid . Items . Count . ToString ( );
				Mouse . OverrideCursor = Cursors . Arrow;
				return "BANKACCOUNT";
			}
			else
			{
				CustCollection . Clear ( );
				CustCollection = cvm . GetCustObsCollection ( CustCollection , "Select * from Customer" );
				CustCollectionView = CollectionViewSource . GetDefaultView ( CustCollection ) as CollectionView;
				CustCollectionView . Filter += new Predicate<object> ( ( flt ) => FilterCustACData ( flt as CustomerViewModel , EventArgs . Empty ) );
				GridLoading = true;
				GridLoading = false;
				IsBankActive = false;

				// Update main grid as well
				ParentBGView . dataGrid . ItemsSource = CustCollectionView;
				GridLoading = true;
				ParentBGView . dataGrid . SelectedIndex = CurrentCustSelection;
				ParentBGView . dataGrid . SelectedItem = CurrentCustSelection;

				// NB These need to be called in this order to get the record to be displayed in the DataGrid
				ParentBGView . dataGrid . ScrollIntoView ( ParentBGView . dataGrid . SelectedIndex );
				ParentBGView . dataGrid . BringIntoView ( );
				Mouse . OverrideCursor = Cursors . Arrow;
				return "CUSTOMER";
			}
		}

		#endregion IComand Exection Handlers

		#region DataGrid Filtering handlers
		private bool FilterBankACData ( object sender , EventArgs e )
		{
			var bvm = sender as BankAccountViewModel;
			string input = bvm.CustNo.ToString();
			string  srchtxt = ParentBGView.filtertext .Text;
			bool b = false, b2=false;
			if ( bvm == null )
				return false;
			if ( srchtxt != null )
			{
				b = input . Contains ( srchtxt );
			}
			srchtxt = ParentBGView . acfiltertext . Text;
			if ( srchtxt != "" )
			{
				input = bvm . AcType . ToString ( );
				srchtxt = ParentBGView . acfiltertext . Text;
				if ( srchtxt == null )
					return b;
				if ( bvm == null )
					return b;
				b2 = input . Contains ( srchtxt );
				b &= b2;
			}
			return ( bool ) b;
		}
		private bool FilterCustACData ( object sender , EventArgs e )
		{
			var cvm = sender as CustomerViewModel;
			string input = cvm.CustNo.ToString();
			string  srchtxt = ParentBGView.filtertext .Text;
			bool b = false, b2=false;
			if ( cvm == null )
				return false;
			if ( srchtxt != null )
			{
				b = input . Contains ( srchtxt );
			}
			srchtxt = ParentBGView . acfiltertext . Text;
			if ( srchtxt != "" )
			{
				input = cvm . AcType . ToString ( );
				srchtxt = ParentBGView . acfiltertext . Text;
				if ( srchtxt == null )
					return b;
				if ( cvm == null )
					return b;
				b2 = input . Contains ( srchtxt );
				b &= b2;
			}
			return ( bool ) b;
		}

		public void filter_TextChanged ( object sender , TextChangedEventArgs e )
		{
			// THIS WORKS CORRECTLY 9/5/22 8:27AM
			string validline = ParentBGView . filtertext . Text;
			int count=0;
			foreach ( var item in validline )
			{
				if ( ValidNumbers . Contains ( item . ToString ( ) ) == false )
				{
					string newline="";
					newline = validline . Substring ( 0 , count );
					newline += validline . Substring ( count , validline . Length - count - 1 );
					MessageBox . Show ( "Invalid character was entered, only Numeric digits are valid  entries...\n\ninvalid character is being removed, " );
					ParentBGView . filtertext . Text = newline;
					ParentBGView . filtertext . SelectAll ( );
					return;
				}
			}
			if ( IsBankActive == true )
			{
				//				foreach ( var item in ParentBGView . custfilter . Text )
				BankCollectionView?.Refresh ( );
				ParentBGView . RecordCount . Text = ParentBGView . dataGrid . Items . Count . ToString ( );
			}
			else
			{
				CustCollectionView?.Refresh ( );
				ParentBGView . RecordCount . Text = ParentBGView . dataGrid . Items . Count . ToString ( );
			}
		}
		public void acfilter_TextChanged ( object sender , TextChangedEventArgs e )
		{
			// THIS WORKS CORRECTLY 9/5/22 8:27AM
			// It also works when the Db's are in different datagrids automatically!
			string validline = ParentBGView . acfiltertext . Text;
			if ( validline . Length == 0 )
				return;
				if( validline . Length > 1 || Convert . ToInt32(validline) < 1 || Convert . ToInt32 ( validline ) > 4 )
			{
				MessageBox . Show ( "You cannot enter more than a SINGLE digit to filter the data on the Account Type.\n\nValid values are between 1 to 4 inclusive" );
				return;
			}
			int count=0;
			foreach ( var item in validline )
			{
				if ( ValidNumbers . Contains ( item . ToString ( ) ) == false )
				{
					string newline="";
					newline = validline . Substring ( 0 , count );
					newline += validline . Substring ( count , validline . Length - count - 1 );
					MessageBox . Show ( "Invalid character was entered, only Numeric digits are valid  entries...\n\ninvalid character is being removed, " );
					ParentBGView . filtertext . Text = newline;
					ParentBGView . filtertext . SelectAll ( );
					return;
				}
			}
			if ( IsBankActive == true )
			{
				//				foreach ( var item in ParentBGView . custfilter . Text )
				BankCollectionView?.Refresh ( );
				ParentBGView . RecordCount . Text = ParentBGView . dataGrid . Items . Count . ToString ( );
			}
			else
			{
				CustCollectionView?.Refresh ( );
				ParentBGView . RecordCount . Text = ParentBGView . dataGrid . Items . Count . ToString ( );
			}
		}

		#endregion DataGrid Filtering handlers

		private void dataGrid_SelectionChanged ( object sender , SelectionChangedEventArgs e )
		{
			BankAccountViewModel bvm;
			CustomerViewModel cvm;
			if ( GridLoading )
				return;

			bvm = ParentBGView . dataGrid . SelectedItem as BankAccountViewModel;
			if ( bvm != null )
			{
				CurrentBankSelection = ParentBGView . dataGrid . SelectedIndex;
				Debug. WriteLine ( $"{bvm . CustNo} selected in Bank Db...." );
			}
			else
			{
				cvm = ParentBGView . dataGrid . SelectedItem as CustomerViewModel;
				if ( cvm != null )
				{
					CurrentCustSelection = ParentBGView . dataGrid . SelectedIndex;
					Debug. WriteLine ( $"{cvm . CustNo} selected in Customer Db...." );
				}
			}
		}
	}
}
