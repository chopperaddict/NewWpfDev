using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Diagnostics;
using System . Linq;
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

using DocumentFormat . OpenXml . Presentation;

using NewWpfDev . Models;
using NewWpfDev . ViewModels;
using NewWpfDev . Views;

using static NewWpfDev . UserControls . BankAccountInfo;

namespace NewWpfDev . UserControls
{
    /// <summary>
    /// Interaction logic for BANKACCOUNTLIST.xaml
    /// hols a bankaccount Datagrid
    /// </summary>
    public partial class BankAccountGrid : UserControl
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

        #region Properties
        public BankAccountGrid ThisWin { get; set; }

        private static BankAcctVm BankAcctVm { get; set; }
        private static BankAccountVM BankAccountvm { get; set; }
        public static ObservableCollection<BankAccountViewModel> Bvm { get; set; }
        public static BankAccountViewModel bv { get; set; }
        public static DataGrid datagrid { get; set; }
        private bool loading { get; set; }
        #endregion Properties

        #region DP's

        public int CurrentIndex
        {
            get { return ( int ) GetValue ( CurrentIndexProperty ); }
            set { SetValue ( CurrentIndexProperty , value ); }
        }
        public static readonly DependencyProperty CurrentIndexProperty =
            DependencyProperty . Register ( "CurrentIndex" , typeof ( int ) , typeof ( BankAccountGrid ) , new PropertyMetadata ( ( int ) 0 , SetSelectedItem ) );
        private static void SetSelectedItem ( DependencyObject d , DependencyPropertyChangedEventArgs e )
        {
            // Set selected item as well
            datagrid . SelectedItem = e . NewValue;
        }

        public DataGrid Datagrid
        {
            get { return ( DataGrid ) GetValue ( DatagridProperty ); }
            set { SetValue ( DatagridProperty , value ); }
        }
        public static readonly DependencyProperty DatagridProperty =
            DependencyProperty . Register ( "Datagrid" , typeof ( DataGrid ) , typeof ( BankAccountGrid ) , new PropertyMetadata ( ( DataGrid ) null ) );

        public BankAccountViewModel BankRecord
        {
            get { return ( BankAccountViewModel ) GetValue ( BankRecordProperty ); }
            set { SetValue ( BankRecordProperty , value ); }
        }
        public static readonly DependencyProperty BankRecordProperty =
            DependencyProperty . Register ( "BankRecord" , typeof ( BankAccountViewModel ) , typeof ( BankAccountGrid ) , new PropertyMetadata ( ( BankAccountViewModel ) null ) );

        #endregion DP's

        public BankAccountGrid ( )
        {
            InitializeComponent ( );
            //viewmodel is BankAccountVM
            var viewmodel = new BankAccountVM ( );
            BankAccountvm = viewmodel;
            this . DataContext = viewmodel;
            BankAcctVm = BankAcctVm . Instances;
            ThisWin = this;
            EventControl . BankDataLoaded += EventControl_BankDataLoaded;
            BankAcctVm . Findmatch += BANKACCOUNTLIST_Findmatch;
            BankAcctVm . DoUpdate += BankAccountList_DoUpdate;
            datagrid = grid1;
            loading = false;
        }

        private void BankAccountList_DoUpdate ( object sender , SelchangedArgs args )
        {
            bool success = true, error = false;
            //get current view of record based on CustNo
            BankAccountViewModel bv = GetSpecifiedRecord ( args . bv . CustNo );

            if ( bv == null ) return;
            // Modify t with data from edit screen
            bv . CustNo = args . bv . CustNo;
            bv . BankNo = args . bv . BankNo;
            bv . AcType = args . bv . AcType;
            bv . IntRate = args . bv . IntRate;
            bv . ODate = args . bv . ODate;
            DateTime dtime = DateTime . Now;
            dtime = args . bv . ODate;
            DateTime dtime2 = DateTime . Now;
            bv . CDate = dtime2;
            success = BankCollection . UpdateBankDb ( bv , "BANKACCOUNT" );
            if ( success == false ) error = true;
            success = BankCollection . UpdateBankDb ( bv , "CUSTOMER" );
            if ( !success || error )
                MessageBox . Show ( "An error occured during the update processing...." , "Database update system" );
            else BankAccountvm.InfoText = "Database Table updated successfully ...";
            
            Host . Info . UpdateLayout ( );
            grid1 . Refresh ( );
            grid1 . UpdateLayout ( );
        }

        #region Event Handlers

        // Event handler from BankAccountList
        public void BANKACCOUNTLIST_Findmatch ( object sender , SelchangedArgs args )
        {
            bool success = false;
            SelchangedArgs selchangedArgs = new SelchangedArgs ( );
            if ( grid1 . Items . Count == 0 )
            {
                selchangedArgs = args;
                selchangedArgs . Index = -1;
                selchangedArgs . FindResult = false;
                BankAcctVm . TriggerMatchfound ( selchangedArgs );
                return;
            }

            for ( int x = 0 ; x < grid1 . Items . Count ; x++ )
            {
                loading = true;
                grid1 . SelectedIndex = x;
                loading = false;
                if ( grid1 . SelectedItem == null ) return;
                BankAccountViewModel Items = grid1 . SelectedItem as BankAccountViewModel;
                if ( Items == null ) return;
                if ( Items . CustNo == args . CustNo )
                {
                    selchangedArgs = args;
                    selchangedArgs . Index = x;
                    selchangedArgs . bv = grid1 . SelectedItem as BankAccountViewModel;
                    selchangedArgs . FindResult = true;
                    selchangedArgs . IsSearchResult = true;
                    BankAcctVm . TriggerMatchfound ( selchangedArgs );
                    success = true;
                    loading = true;
                    Utils . ScrollRecordIntoView ( grid1 , x );
                    Debug . WriteLine ( $"DEBUG : Found match to {Items . CustNo}" );
                    BankAccountvm . InfoText = $"Matching record for Customer # [{selchangedArgs . bv . CustNo}] found ...";
                    loading = false;
                    break;
                }
                loading = false;
            }
            if ( success == false )
            {
                selchangedArgs . Index = -1;
                selchangedArgs . FindResult = false;
                BankAcctVm . TriggerMatchfound ( selchangedArgs );
                BankAccountvm . InfoText = $"Failed to find Matching record for Customer # [{selchangedArgs . bv . CustNo}] ...";
            }
        }
        #endregion Event Handlers
        private BankAccountViewModel GetSpecifiedRecord ( string Custno )
        {
            BankAccountViewModel Items = null;
            for ( int x = 0 ; x < grid1 . Items . Count ; x++ )
            {
                loading = true;
                grid1 . SelectedIndex = x;
                loading = false;
                if ( grid1 . SelectedItem == null ) return null;
                Items = grid1 . SelectedItem as BankAccountViewModel;
                if ( Items == null ) return null;
                if ( Items . CustNo == Custno )
                {
                    Debug . WriteLine ( $"DEBUG : Found match to {Items . CustNo}" );
                    loading = false;
                    break;
                }
                loading = false;
            }
            return Items;
        }
        public static BankAcHost Host { get; set; }
        private bool IsActive { get; set; }
        public void IsHost ( bool value )
        {
            IsActive = value;
            if ( value == true )
            {
                if ( grid1 == null || grid1 . Items . Count == 0 )
                    Task . Run ( ( ) => LoadBank ( true ) );
            }
        }
        public static void SetHost ( BankAcHost host )
        {
            Host = host;
        }
        private void SelectDetails ( object sender , RoutedEventArgs e )
        {
            //send name of panel required to be opened (info)
            Host . ClosePanel ( this , "BANKACCOUNTGRID" );
        }
        private void SelectAccounts ( object sender , RoutedEventArgs e )
        {
            //send name of panel required to be opened (Grid)
            Host . ClosePanel ( this , "BANKACCOUNTLIST" );
        }

        private void grid1_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {
            int count = 0;
            if ( loading ) return;
            BankAccountViewModel bv = new BankAccountViewModel ( );
            bv = grid1 . SelectedItem as BankAccountViewModel;
            SelchangedArgs args = new SelchangedArgs ( );
            args . Index = grid1 . SelectedIndex;
            args . bv = grid1 . SelectedItem as BankAccountViewModel;
            // Set our DP selectedIndex
            CurrentIndex = grid1 . SelectedIndex;
            BankAcctVm . TriggerSelChange ( args );
            loading = false;
        }
        public void ResizeControl ( double height , double width )
        {
            this . Height = height;
            this . Width = width;
            this . Refresh ( );
        }

        #region Data Loading
        public async Task LoadBank ( bool update = true )
        {
            Application . Current . Dispatcher . Invoke ( ( ) =>
            {
                if ( Bvm == null ) Bvm = new ObservableCollection<BankAccountViewModel> ( );
                grid1 . ItemsSource = null;
                grid1 . Items . Clear ( );
                //grid1 . CellStyle = FindResource ( "MAINCustomerGridStyle" ) as Style;
            } );

            Bvm = ( ObservableCollection<BankAccountViewModel> ) UserControlDataAccess . GetBankObsCollectionAsync ( Bvm , "" , true , "BANKACCOUNTLIST" );
            return;
        }

        public void EventControl_BankDataLoaded ( object sender , LoadedEventArgs e )
        {
            Application . Current . Dispatcher . Invoke ( async ( ) =>
            {
                await DispatcherExtns . SwitchToUi ( Application . Current . Dispatcher );
                Bvm = e . DataSource as ObservableCollection<ViewModels . BankAccountViewModel>;
                grid1 . ItemsSource = Bvm;
                DataTemplate dt = Application . Current . FindResource ( "BankDataTemplate1" ) as DataTemplate;
                grid1 . ItemTemplate = dt;
                grid1 . UpdateLayout ( );
                grid1 . SelectedIndex = 0;
                grid1 . SelectedItem = grid1 . SelectedIndex;
                Utils . ScrollRecordIntoView ( grid1 , 0 );
                datagrid = grid1;
            } );
        }

        #endregion Data Loading

        private void grid1_CellEditEnding ( object sender , DataGridCellEditEndingEventArgs e )
        {
        }
        private void ResizeControl ( object sender , SizeChangedEventArgs e )
        {
        }

        private void prevrecord ( object sender , RoutedEventArgs e )
        {
            grid1 . SelectedIndex--;
        }

        private void nextrecord ( object sender , RoutedEventArgs e )
        {
            grid1 . SelectedIndex++;
        }
    }
}
