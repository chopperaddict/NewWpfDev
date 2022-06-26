using NewWpfDev . ViewModels;
using NewWpfDev . Views;

using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . Diagnostics;
using System . Linq;
using System . Runtime . CompilerServices;
using System . Text;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Data;
using System . Windows . Documents;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Media . Imaging;
using System . Windows . Navigation;
using System . Windows . Shapes;

namespace NewWpfDev . UserControls
{
	/// <summary>
	/// Interaction logic for StdDataUserControl.xaml
	///  USERCONTROL VERSION
	/// </summary>
	public partial class StdDataUserControl 
	{
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
		private static Window ParentView;

		// Sundry variables used in processing
		private const string ValidNumbers="0123456789";
		private static int CurrentBankSelection { get; set; } = 0;
		private static int CurrentCustSelection { get; set; } = 0;
		private static bool IsBankActive { get; set; } = false;
		private static bool GridLoading { get; set; } = false;
//		public int dgHeight { get; set; }
//		public int dgWidth { get; set; }

		public static StdDataUserControl stdgrid { get; set; }
		#endregion Declarations

		#region  ICommand declarations
		//		public ICommand debugger { get; set; }
		//		public ICommand CloseWindow { get; set; }
		//		public ICommand LoadCustomers { get; set; }
		//		public ICommand LoadBank { get; set; }
		#endregion  ICommand declarations

		#region Setup
		public StdDataUserControl ( )
		{
			// WORKS WELL
			InitializeComponent ( );
			stdgrid = this;
			this . DataContext = this;
			Mouse . OverrideCursor = Cursors . Wait;
			// Flag so we can track which Db is being used in the main grid
			IsBankActive = true;
			// Hook into filter text change text fields to handle real time updating of Datagrid contents based on CustNo
			bankfilter . TextChanged += bankfilter_TextChanged;
			custfilter . TextChanged += custfilter_TextChanged;
			// Hook into datagrid selection changes
			dataGrid . SelectionChanged += dataGrid_SelectionChanged;
			Debug. WriteLine ( $"dataGrid IsVisible={dataGrid . IsVisible}" );
			// almost Finally, create pointer for Bank and Customer Records :  *needed for selection changes"
			bvm = new BankAccountViewModel ( );
			cvm = new CustomerViewModel ( );
			// Finally, Load Bank Account Data
			ExecuteLoadBank ( null );
			//			ExecuteLoadCustomers ( null );
			Mouse . OverrideCursor = Cursors . Arrow;
			//this . Visibility = Visibility . Visible;
			//this . Height = ControlHeight;
			//this . Width = ControlWidth;
			this . SizeChanged += StdDataControl_SizeChanged;
			this . Height += 1;
		}
		private void stddatagridControl_Loaded ( object sender , RoutedEventArgs e )
		{
		}

		private void StdDataControl_SizeChanged ( object sender , SizeChangedEventArgs e )
		{
			// This handles our data grid resizing
			// all 3 lines are needed !!
			// No width conversion is needed ?
//			double height = e.NewSize.Height;
//			MAINgrid_Grid1 . Height = height;
			//dataGrid . Height = dgcanvas . ActualHeight - 120;
			//(Top_rightcol as FrameworkElement ) . SetValue ( Canvas . LeftProperty , ( double ) dataGrid.ActualWidth );
//			Debug. WriteLine ( $"MvvmBGV Width = {dataGrid . ActualWidth} Canvas Width = {dgcanvas . ActualWidth}" );
		}							  

		public void SetParent ( Window parent )
		{
			ParentView = parent;
		}

		#endregion Setup

		#region ICommand CanExecute handlers
		private bool CanExecuteDebugger ( object arg )
		{
			return true;
		}
		public bool CanExecuteCloseWindow ( object arg ) { return true; }
		public void ExecuteCloseWindow ( object arg )
		{
			Debug. WriteLine ( "We have Hit the close ICommand ..." );
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
		private bool CanExecuteLoadBank ( object arg )
		{
			//var v =dataGrid . SelectedItem as BankAccountViewModel;
			//if ( v != null )
			//	return false;
			//else
			//	return true;
			//if ( IsBankActive == false )
			//{
			//	bankfilter . IsEnabled = false;
			//	custfilter . IsEnabled = true;
			//	if ( IsBankActive )
			//		return false;
			//	else
			//		return true;
			//}
			//else
			//{
			//	bankfilter . IsEnabled = true;
			//	custfilter . IsEnabled = false;
			//	return false;
			//}
			return true;
		}
		private bool CanExecuteLoadCustomers ( object arg )
		{
			//var v = dataGrid . SelectedItem as CustomerViewModel;
			//if ( v != null )
			//	return false;
			//else
			//	return true;

			//if ( IsBankActive == true )
			//{
			//	custfilter . IsEnabled = false;
			//	bankfilter . IsEnabled = true;
			//	if ( IsBankActive )
			//		return true;
			//	else
			//		return false;
			//}
			//else
			//{
			//	custfilter . IsEnabled = true;
			//	bankfilter . IsEnabled = false;
			//	return false;
			//}
			return true;
		}

		#endregion ICommand CanExecute handlers

		#region IComand Exection Handlers
		public void ExecuteDebugger ( object obj )
		{
			Debugger . Break ( );
		}
		//==============================//
		// ICommand method  to Load Bank Data  using our Root level Classes (BankAccountViewModel & CustomerViewModel)
		//==============================//
		private void ExecuteLoadBank ( object obj )
		{
			Mouse . OverrideCursor = Cursors . Wait;
			BankCollection . Clear ( );
			BankCollection = bvm . GetBankAccounts ( BankCollection , "Select * from BankAccount" , false , "BankGridView" );
			BankCollectionView = CollectionViewSource . GetDefaultView ( BankCollection ) as CollectionView;
			//Set the fillter we want up  This is called for each record in the collection
			BankCollectionView . Filter = new Predicate<object> ( ( flt ) => FilterBankData ( flt as BankAccountViewModel ) );
			dataGrid . ItemsSource = BankCollectionView;
			GridLoading = true;
			dataGrid . SelectedIndex = CurrentBankSelection;
			dataGrid . SelectedItem = CurrentBankSelection;

			// NB These need to be called in this order to get the record to be displayed in the DataGrid
			dataGrid . ScrollIntoView ( dataGrid . SelectedIndex );
			dataGrid . BringIntoView ( );
			GridLoading = false;
			IsBankActive = true;
			ToggleOptions ( "BANKACCOUNT" );
			Mouse . OverrideCursor = Cursors . Arrow;
		}
		//=================================//
		// ICommand method  to Load Customer Data
		//=================================//
		private void ExecuteLoadCustomers ( object obj )
		{
			Mouse . OverrideCursor = Cursors . Wait;
			CustCollection . Clear ( );
			CustCollection = cvm . GetCustObsCollection ( CustCollection , "Select * from Customer" );
			//CustCollection = SqlSupport . LoadCustomer ( "Select * from Customer" , 0 , false , false );
			CustCollectionView = CollectionViewSource . GetDefaultView ( CustCollection ) as CollectionView;
			//Set the fillter we want up  This is called for each record in the collection
			CustCollectionView . Filter = new Predicate<object> ( ( filt ) => FilterCustData ( filt as CustomerViewModel ) );
			//dataGrid2 . ItemsSource = CustCollectionView;
			GridLoading = true;

			// Update main grid as well
			dataGrid . ItemsSource = CustCollectionView;
			GridLoading = true;
			dataGrid . SelectedIndex = CurrentCustSelection;
			dataGrid . SelectedItem = CurrentCustSelection;

			// NB These need to be called in this order to get the record to be displayed in the DataGrid
			dataGrid . ScrollIntoView ( dataGrid . SelectedIndex );
			dataGrid . BringIntoView ( );
			IsBankActive = false;
			ToggleOptions ( "CUSTOMER" );
			Mouse . OverrideCursor = Cursors . Arrow;
		}
		#endregion IComand Exection Handlers

		#region DataGrid Filtering handlers
		private void bankfilter_TextChanged ( object sender , TextChangedEventArgs e )
		{
			// THIS WORKS CORRECTLY 9/5/22 8:27AM
			// It also works when the Db's are in different datagrids automatically!
			string validline = bankfilter . Text;
			int count=0;
			if ( IsBankActive )
			{
				foreach ( var item in bankfilter . Text )
				{
					if ( ValidNumbers . Contains ( item . ToString ( ) ) == false )
					{
						string newline="";
						newline = validline . Substring ( 0 , count );
						newline += validline . Substring ( count , validline . Length - count - 1 );
						MessageBox . Show ( "Invalid character was entered, only Numeric digits are valid  entries...\n\ninvalid character is being removed, " );
						bankfilter . Text = newline;
						bankfilter . SelectAll ( );
						return;
					}
					count++;
				}
				BankCollectionView?.Refresh ( );
				RecordCount . Text = dataGrid . Items . Count . ToString ( );
			}
			else
			{
				bankfilter . Text = "";
//				custfilter . IsEnabled = false;
			}
		}
		private void custfilter_TextChanged ( object sender , TextChangedEventArgs e )
		{
			// THIS WORKS CORRECTLY 9/5/22 8:27AM
			// It also works when the Db's are in different datagrids automatically!
			string validline = custfilter . Text;
			int count=0;
			if ( IsBankActive == false )
			{
				foreach ( var item in custfilter . Text )
				{
					if ( ValidNumbers . Contains ( item . ToString ( ) ) == false )
					{
						string newline="";
						newline = validline . Substring ( 0 , count );
						newline += validline . Substring ( count , validline . Length - count - 1 );
						MessageBox . Show ( "Invalid character was entered, only Numeric digits are valid  entries...\n\ninvalid character is being removed, " );
						custfilter . Text = newline;
						custfilter . SelectAll ( );
						return;
					}
				}
				CustCollectionView?.Refresh ( );
				RecordCount . Text = dataGrid . Items . Count . ToString ( );
			}
			else
			{
				custfilter . Text = "";
				//bankfilter . IsEnabled = false;
			}
		}
		private bool FilterBankData ( BankAccountViewModel bvm )
		{
			//BankAccountViewModel bvm = bvm;
			string input =bvm.CustNo.ToString();
			string  srchtxt = bankfilter.Text;
			if ( srchtxt == null )
				return false;
			if ( bvm == null )
				return false;
			bool b = input.Contains(srchtxt );
			return b;
		}
		private bool FilterCustData ( CustomerViewModel cvm )
		{
			string input = cvm.CustNo.ToString();
			string  srchtxt = custfilter .Text;
			if ( srchtxt == null )
				return false;
			if ( cvm == null )
				return false;
			bool b = input.Contains(srchtxt );
			return b;
		}

		#endregion DataGrid Filtering handlers

		private void dataGrid_SelectionChanged ( object sender , SelectionChangedEventArgs e )
		{
			BankAccountViewModel bvm;
			CustomerViewModel cvm;
			if ( GridLoading )
				return;

			bvm = dataGrid . SelectedItem as BankAccountViewModel;
			if ( bvm != null )
			{
				CurrentBankSelection = dataGrid . SelectedIndex;
				Debug. WriteLine ( $"{bvm . CustNo} selected in Bank Db...." );
			}
			else
			{
				cvm = dataGrid . SelectedItem as CustomerViewModel;
				if ( cvm != null )
				{
					CurrentCustSelection = dataGrid . SelectedIndex;
					Debug. WriteLine ( $"{cvm . CustNo} selected in Customer Db...." );
				}
			}
		}

		private void LoadCustomers ( object sender , RoutedEventArgs e )
		{
			ExecuteLoadCustomers ( null );

		}

		private void LoadBankaccts ( object sender , RoutedEventArgs e )
		{
			ExecuteLoadBank ( null );
		}

		private void CloseThisWindow ( object sender , RoutedEventArgs e )
		{
			//this . Visibility = Visibility . Collapsed;
			WindowCollection  v = Application .Current.Windows;
			MessageBoxResult res = MessageBox . Show ( "Yes - Close App down entirely, No - Close this Viewer Only ?" , "Close Options" ,
						MessageBoxButton .YesNoCancel, MessageBoxImage.Question, MessageBoxResult . Yes );
			foreach ( Window item in v )
			{
				if ( res == MessageBoxResult . Cancel )
					return;
				if ( res == MessageBoxResult . Yes )
				{
					Application . Current . Shutdown ( );
				}
				else if ( res == MessageBoxResult . No )
				{
					string parent = ParentView.ToString();
					if ( parent == item.ToString() )
					{
						stdgrid . Visibility = Visibility . Collapsed;
						break;
					}
				}
			}
		}

		// In the usercontrol, add this code :
		//		public event RoutedEventHandler Click;

		//// This passes the Click event (from this user control) on to user windows
		//private void OnButtonClick ( object sender , RoutedEventArgs e )
		//{
		//	if ( this . Click != null )
		//	{
		//		this . Click ( this , e );
		//	}
		//}
		#region Dependency properties


		public double ControlHeight
		{
			get { return ( double ) GetValue ( ControlHeightProperty ); }
			set { SetValue ( ControlHeightProperty , value ); }
		}
		public static readonly DependencyProperty ControlHeightProperty =
		    DependencyProperty.Register("ControlHeight", typeof(double), typeof(StdDataUserControl),
				  new PropertyMetadata((double)650,
							  new PropertyChangedCallback(OnHeightChanged)));

		private static void OnHeightChanged ( DependencyObject d , DependencyPropertyChangedEventArgs e )
		{
			Debug. WriteLine ( $"Height changed from {e . OldValue} to {e . NewValue}" );
			//			mvgrid . Height= ( double ) e . NewValue - 200;
			//StdDataUserControl. Height = mvgrid . Height;
			//			mvgrid . Refresh ( );
		}

		public double ControlWidth
		{
			get { return ( double ) GetValue ( ControlWidthProperty ); }
			set { SetValue ( ControlWidthProperty , value ); }
		}
		public static readonly DependencyProperty ControlWidthProperty =
			DependencyProperty.Register("ControlWidth", typeof(double), typeof(StdDataUserControl),
				   new PropertyMetadata((double)1100 ,
				new PropertyChangedCallback(OnWidthChanged)));

		private static void OnWidthChanged ( DependencyObject d , DependencyPropertyChangedEventArgs e )
		{
			//Debug. WriteLine ($"Width changed from {e.OldValue} to {e.NewValue}");
			//double newval = (double) e . NewValue -400;
			//if ( newval < 800 )
			//	newval = 800;
			//mvgrid . Width = newval;
		}
		private void ToggleOptions ( string type )
		{
			if ( type == "BANKACCOUNT" )
			{
				CustGroup . IsEnabled = false;
				//LoadCust . IsEnabled = true;
				custfilter . Opacity = 0.2;
				CustFilterlabel . Opacity = 0.3;
				
				BankGroup . IsEnabled = true;
				LoadBank . IsEnabled = false;
				bankfilter . Opacity = 1;
				BankFilterlabel . Opacity = 1.0; 				
				Currentdb . Text = "Bank Accounts Database";
				Currentdb . TextAlignment = TextAlignment . Center;
			}
			else
			{
				BankGroup . IsEnabled = false;
				LoadBank . IsEnabled = true;
				bankfilter . Opacity = 0.2;
				BankFilterlabel . Opacity = 0.3;
				
				CustGroup . IsEnabled = true;
//				LoadCust . IsEnabled = false;
				custfilter . Opacity = 1;
				CustFilterlabel . Opacity = 1.0;
				Currentdb . Text = "Customers Database";
				Currentdb . TextAlignment = TextAlignment . Center;
			}
			RecordCount . Text = dataGrid . Items . Count . ToString ( );
		}
		#endregion Dependency properties

		private void Loadbankdata ( object sender , RoutedEventArgs e )
		{

		}
	}
}
