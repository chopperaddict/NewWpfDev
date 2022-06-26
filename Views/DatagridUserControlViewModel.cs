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
using System . Windows . Input;
using System . Windows . Threading;

using NewWpfDev . UserControls;

using NewWpfDev . Views;

namespace NewWpfDev . ViewModels
{
    [Serializable]
    public class DatagridUserControlViewModel 
    {
        #region declarations
        public object Viewmodel { get; set; }
        private DataGrid grid1 { get; set; }

        public ObservableCollection<ViewModels . BankAccountViewModel> Bvm { get; private set; }
        public ObservableCollection<ViewModels . CustomerViewModel> Cvm { get; private set; }
        private static TabWinViewModel Controller { get; set; }
        private static DataGrid dgrid1 { get; set; }
        public static DgUserControl dgUserctrl{ get; set; }
        public static object HostViewModel { get; set; }
        public static Tabview tabview { get; set; }
        public string CurrentType { set; get; } = "BANK";
        private static DgUserControl ThisWin { get; set; }
        private bool IsLoaded { get; set; } = false;
        public bool SelectionInAction { get; set; } = false;
        public static bool TrackselectionChanges { get; set; } = false;
        private int CurrentIndex { get; set; } = 0;
        
        #endregion declarations

        #region OnPropertyChanged
        [field: NonSerialized ( )]
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged ( string propertyName )
        {
            if ( PropertyChanged != null )
            {
                PropertyChanged ( this , new PropertyChangedEventArgs ( propertyName ) );
            }
        }
        #endregion OnPropertyChanged
        
        // Constructor
        public  DatagridUserControlViewModel ( DgUserControl ctrl )
        {
            Debug. WriteLine ( $"DataGrid ViewModel (DATAGRIDUSERCONTROLVIEWMODEL(DgUserctrl) Loading ......" );
            // Setup pointer to our user control (DgUserComtrol)
            dgUserctrl = ctrl;
            tabview = Tabview . GetTabview ( );
            dgrid1 = new DataGrid ( );
            dgrid1 = Tabview . GetDataGrid ( dgrid1 );
            TabWinViewModel . LoadDb += DgLoadDb;
            EventControl . BankDataLoaded += EventControl_BankDataLoaded;
            EventControl . CustDataLoaded += EventControl_CustDataLoaded;
            EventControl . ListSelectionChanged += SelectionHasChanged;
            // allow this  to broadcast
            EventControl . TriggerWindowMessage ( this , new InterWindowArgs { message = $"DgUIserControl loaded..." } );
            CreateBankColumns ( );
            LoadBank ( );
            IsLoaded = false;
            // Save to our ViewModel repository
            Viewmodel = new ViewModel ( );
            Viewmodel = dgUserctrl;
            ViewModel . SaveViewmodel ( "DgUsercontrol" , Viewmodel );
        }

        public  void DgLoadDb ( object sender , LoadDbArgs e )
        {
            if ( e . dbname == "BANK" )
                LoadBank ( );
            else
                LoadCustomer ( );
        }

        public static DgUserControl SetController ( object ctrl )
        {
            HostViewModel = ctrl;
            tabview = TabWinViewModel . SendTabview ( );
            return ThisWin;
        }
        private void CreateBankColumns ( )
        {
            dgUserctrl. grid1 .Columns . Clear ( );
            Debug. WriteLine ( $"CREATING BANK COLUMNS" );
            DataGridTextColumn c1 = new DataGridTextColumn ( );
            c1 . Header = "Id";
            c1 . Binding = new Binding ( "Id" );
            dgUserctrl. grid1 .Columns . Add ( c1 );
            DataGridTextColumn c2 = new DataGridTextColumn ( );
            c2 . Header = "Customer #";
            c2 . Binding = new Binding ( "CustNo" );
            dgUserctrl. grid1 .  Columns . Add ( c2 );
            DataGridTextColumn c3 = new DataGridTextColumn ( );
            c3 . Header = "Bank #";
            c3 . Binding = new Binding ( "BankNo" );
            dgUserctrl. grid1 .  Columns . Add ( c3 );
            DataGridTextColumn c4 = new DataGridTextColumn ( );
            c4 . Header = "A/c Type";
            c4 . Binding = new Binding ( "AcType" );
            dgUserctrl. grid1 .  Columns . Add ( c4 );
            DataGridTextColumn c5 = new DataGridTextColumn ( );
            c5 . Header = "Balance";
            c5 . Binding = new Binding ( "Balance" );
            dgUserctrl. grid1 .  Columns . Add ( c5 );
            DataGridTextColumn c6 = new DataGridTextColumn ( );
            c6 . Header = "Opened";
            c6 . Binding = new Binding ( "ODate" );
            dgUserctrl. grid1 .  Columns . Add ( c6 );
            DataGridTextColumn c7 = new DataGridTextColumn ( );
            c7 . Header = "Closed";
            c7 . Binding = new Binding ( "CDate" );
            dgUserctrl. grid1 .  Columns . Add ( c7 );
        }
        private void CreateCustomerColumns ( )
        {
            dgUserctrl. grid1 .  Columns . Clear ( );
            Debug. WriteLine ( $"CREATING CUSTOMER COLUMNS" );
            DataGridTextColumn c1 = new DataGridTextColumn ( );
            c1 . Header = "Id";
            c1 . Binding = new Binding ( "Id" );
            dgUserctrl. grid1 .  Columns . Add ( c1 );
            DataGridTextColumn c2 = new DataGridTextColumn ( );
            c2 . Header = "Customer #";
            c2 . Binding = new Binding ( "CustNo" );
            dgUserctrl. grid1 .  Columns . Add ( c2 );
            DataGridTextColumn c3 = new DataGridTextColumn ( );
            c3 . Header = "Bank #";
            c3 . Binding = new Binding ( "BankNo" );
            dgUserctrl. grid1 .  Columns . Add ( c3 );
            DataGridTextColumn c4 = new DataGridTextColumn ( );
            c4 . Header = "A/c Type";
            c4 . Binding = new Binding ( "AcType" );
            dgUserctrl. grid1 .  Columns . Add ( c4 );
            DataGridTextColumn c5 = new DataGridTextColumn ( );
            c5 . Header = "Address1";
            c5 . Binding = new Binding ( "Addr1" );
            dgUserctrl. grid1 .  Columns . Add ( c5 );
            DataGridTextColumn c6 = new DataGridTextColumn ( );
            c6 . Header = "Address2";
            c6 . Binding = new Binding ( "Addr2" );
            dgUserctrl. grid1 .  Columns . Add ( c6 );
            DataGridTextColumn c7 = new DataGridTextColumn ( );
            c7 . Header = "Town";
            c7 . Binding = new Binding ( "Town" );
            dgUserctrl. grid1 .  Columns . Add ( c7 );
            DataGridTextColumn c8 = new DataGridTextColumn ( );
            c8 . Header = "County";
            c8 . Binding = new Binding ( "County" );
            dgUserctrl. grid1 .  Columns . Add ( c8 );
            DataGridTextColumn c9 = new DataGridTextColumn ( );
            c9 . Header = "Zip";
            c9 . Binding = new Binding ( "PCode" );
            dgUserctrl. grid1 .  Columns . Add ( c9 );
            DataGridTextColumn c10 = new DataGridTextColumn ( );
            c10 . Header = "Opened";
            c10 . Binding = new Binding ( "ODate" );
            dgUserctrl. grid1 .  Columns . Add ( c10 );
            DataGridTextColumn c11 = new DataGridTextColumn ( );
            c11 . Header = "Closed";
            c11 . Binding = new Binding ( "CDate" );
            dgUserctrl. grid1 .  Columns . Add ( c11 );
        }
        public  void  LoadBank ( bool update = true )
        {
            BankCollection bankcollection = new BankCollection ( );
            dgUserctrl. grid1 .  ItemsSource = null;
            dgUserctrl. grid1 .  Items . Clear ( );
            if ( Bvm == null ) Bvm = new ObservableCollection<BankAccountViewModel> ( );
            CurrentType = "BANK";
            TabWinViewModel . TriggerDbType ( CurrentType );

            Task task = Task . Run (  ( ) =>
            {
                // This is pretty fast - uses Dapper and Linq
               Application . Current . Dispatcher . Invoke ( ( ) =>
                {
                   UserControlDataAccess . GetBankObsCollectionAsync ( Bvm , "" , true , "DgUserControl" );
                } );
                return Bvm;
            } );
            // This workks  too, but above call is faster
            //Task . Run (async ( ) => {
            //     this . Dispatcher . Invoke ( ( ) =>
            //     {
            //        UserControlDataAccess . GetBankObsCollection ( Bvm , true , "DgUserControl" );
            //    } );
            //} );
            //return Bvm;
        }
        public void LoadCustomer ( bool update = true )
        {
            dgUserctrl. grid1 .ItemsSource = null;
            dgUserctrl. grid1 .Items . Clear ( );
            CurrentType = "CUSTOMER";
            TabWinViewModel . TriggerDbType ( CurrentType );
            if ( Cvm == null ) Cvm = new ObservableCollection<CustomerViewModel> ( );
            //            CustomerViewModel . GetCustObsCollectionWithDict ( Cvm );
            Task task = Task . Run ( ( ) =>
            {
                // This is pretty fast - uses Dapper and Linq
                Application.Current . Dispatcher . Invoke ( ( ) =>
                {
                    Cvm = ( ObservableCollection<CustomerViewModel> ) UserControlDataAccess . GetCustObsCollection ( Cvm , "" , true , "DgUserControl" );
                } );
            } );
        }
        public void EventControl_BankDataLoaded ( object sender , LoadedEventArgs e )
        {
            if ( e . CallerType != "DgUserControl" ) return;
            //if ( dgUserctrl. grid1 .  Items . Count > 0 && CurrentType != "BANK" ) return;
            CurrentType = "BANK";
            TabWinViewModel . TriggerDbType ( CurrentType );
            //await Dispatcher . SwitchToUi ( );
            $"Dispatcher on UI thread =  {Application . Current . Dispatcher . CheckAccess ( )}" . CW ( );
            Debug. WriteLine ( $"Data requested by : [{e . CallerDb}]" );
            dgUserctrl. grid1 .  ItemsSource = null;
            dgUserctrl. grid1 .  Items . Clear ( );
            CreateBankColumns ( );
            Bvm = e . DataSource as ObservableCollection<ViewModels . BankAccountViewModel>;
            DataTemplate dt = Application . Current . FindResource ( "BankDataTemplate1" ) as DataTemplate;
            dgUserctrl. grid1 .  ItemTemplate = dt;
            dgUserctrl. grid1 .  ItemsSource = Bvm;
            dgUserctrl. grid1 .SelectedIndex = 0;
            dgUserctrl. grid1 .SelectedItem = 0;
            Utils . ScrollRecordIntoView ( dgUserctrl . grid1 ,  0 );
//            tabview . DgridTab . Content = null;
//            tabview . DgridTab . Content = dgUserctrl . grid1;
            //dgUserctrl . grid1 .UpdateLayout ( );
            Debug. WriteLine ( $"Data Loaded for: [{e . CallerDb}], Records = {dgUserctrl. grid1 .  Items . Count}" );
            //TabWinViewModel . dgUserctrl . Refresh ( );
            DbCountArgs args = new DbCountArgs ( );
            args . Dbcount = Bvm?.Count ?? -1;
            args . sender = "dgUserctrl";
            TabWinViewModel . TriggerBankDbCount ( this , args );

        }

        public void EventControl_CustDataLoaded ( object sender , LoadedEventArgs e )
        {
            if ( e . CallerType != "DgUserControl" ) return;
            //            if ( dgUserctrl. grid1 .  Items . Count > 0 && CurrentType != "CUSTOMER" ) return;

            CurrentType = "CUSTOMER";
            TabWinViewModel . TriggerDbType ( CurrentType );
            dgUserctrl. grid1 .  ItemsSource = null;
            dgUserctrl. grid1 .  Items . Clear ( );
            CreateCustomerColumns ( );
            Cvm = new ObservableCollection<ViewModels . CustomerViewModel> ( );
            Cvm = e . DataSource as ObservableCollection<ViewModels . CustomerViewModel>;
            dgUserctrl. grid1 .  ItemsSource = Cvm;
            dgUserctrl. grid1 .CellStyle = Application . Current . FindResource ( "MAINCustomerGridStyle" ) as Style;
            DataTemplate dt = Application.Current.FindResource ( "CustomersDbTemplate1" ) as DataTemplate;
            dgUserctrl. grid1 .  ItemTemplate = dt;
            dgUserctrl. grid1 . SelectedIndex = 0;
            dgUserctrl. grid1 . SelectedItem = 0;
            Utils . ScrollRecordIntoView ( grid1 , 0 );
            DbCountArgs args = new DbCountArgs ( );
            args . Dbcount = Cvm?.Count ?? -1;
            args . sender = "dgUserctrl";
            TabWinViewModel . TriggerBankDbCount ( this , args );
        }

        public void grid1_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {
            if ( this . SelectionInAction )
            {
                this . SelectionInAction = false;
                return;
            }
            //DataGrid dg = sender as DataGrid;
            //if ( dg == null ) return;
            if ( dgUserctrl.grid1. Items . Count == 0 ) return;
            if ( dgUserctrl.grid1. SelectedIndex == -1 )
            {
                //make sure we have something selected !
                dgUserctrl.grid1. SelectedIndex = 0;
                dgUserctrl.grid1. SelectedItem = 0;
            }
            //            DataGrid v = e . OriginalSource as DataGrid;
            DataGrid v = e . OriginalSource as DataGrid;
            if ( v?.SelectedIndex != CurrentIndex )
            {
                (double, int) t1 = (4.5, 3);
                CurrentIndex = v . SelectedIndex;

                if ( tabview. ViewersLinked)
                {
                    this . SelectionInAction = true;
                    SelectionChangedArgs args = new SelectionChangedArgs ( );
                    args . index = dgUserctrl.grid1. SelectedIndex;
                    args . data = dgUserctrl.grid1. SelectedItem;
                    args . sendertype = CurrentType;
                    args . sendername = "grid1";
                    Debug. WriteLine ( $"DataGrid broadcasting selection set to  {args . index}" );
   //                 this . SelectionInAction = false;
                    EventControl . TriggerListSelectionChanged ( sender , args );
                }
           }
            this.SelectionInAction = false;

            Mouse . OverrideCursor = Cursors . Arrow;
            CurrentIndex = dgUserctrl.grid1. SelectedIndex;
            Debug. WriteLine ( $"{CurrentIndex} ...." );
        }

        public  void SelectionHasChanged ( object sender , SelectionChangedArgs e )
        {
            bool success = false;
            object row = null;
            string custno = "", bankno = "";
            if ( DgUserControl . TrackselectionChanges == false ) return;
            // Another viewer has changed selection
            if ( dgUserctrl. grid1 .  ItemsSource == null ) return;
            if ( sender . GetType ( ) == typeof ( DgUserControl ) ) return;

            int newindex = 0;
            if ( e . sendername != "grid1" )
            {
                if ( e . sendertype == "BANK" )
                {
                    // Sender is a BANK
                    BankAccountViewModel sourcerecord = new BankAccountViewModel ( );
                    sourcerecord = e . data as BankAccountViewModel;
                    if ( sourcerecord != null )
                    {
                        custno = sourcerecord . CustNo;
                        bankno = sourcerecord . BankNo;
                    }
                    else return;
                }
                else if ( e . sendertype == "CUSTOMER" )
                {
                    // Sender is a CUSTOMER
                    CustomerViewModel sourcerecord = new CustomerViewModel ( );
                    sourcerecord = e . data as CustomerViewModel;
                    if ( sourcerecord != null )
                    {
                        custno = sourcerecord . CustNo;
                        bankno = sourcerecord . BankNo;
                    }
                    else return;
                }
                if ( this . CurrentType == "CUSTOMER" )
                {
                    try
                    {
                        foreach ( CustomerViewModel item in dgUserctrl. grid1 .  Items )
                        {
                            if ( item . CustNo == custno && item . BankNo == bankno )
                            {
                                this . SelectionInAction = true;
                                dgUserctrl. grid1 . SelectedIndex = newindex;
                                dgUserctrl. grid1 . SelectedItem = newindex;
                                CurrentIndex = newindex;

                                Debug. WriteLine ( $"DataGrid selection in Customers matched on {custno}:{bankno}, index {newindex}" );
                                //
                                //row = Utils . GetRow ( grid1 , newindex );
                                Utils . ScrollRecordIntoView ( grid1 , newindex );
                                success = true;
                                break;
                            }
                            newindex++;
                        }
                    }
                    catch ( Exception ex ) { Debug. WriteLine ( $"DataGrid failed search in Customer for match to {custno} : {bankno} : {ex . Message}" ); }
                }
                else
                {
                    try
                    {
                        foreach ( BankAccountViewModel item in dgUserctrl. grid1 .  Items )
                        {
                            if ( item . CustNo == custno && item . BankNo == bankno )
                            {
                                this . SelectionInAction = true;
                                dgUserctrl. grid1 . SelectedIndex = newindex;
                                dgUserctrl. grid1 . SelectedItem = newindex;
                                CurrentIndex = newindex;
                                Debug. WriteLine ( $"DataGrid selection in BankAccount matched on {custno}:{bankno}, index {newindex}" );
                                //row = Utils . GetRow ( grid1 , newindex );
                                Utils . ScrollRecordIntoView ( dgUserctrl.grid1 , newindex );
                                dgUserctrl. grid1 . UpdateLayout ( );
                                success = true;
                                break;
                            }
                            newindex++;
                        }
                    }
                    catch ( Exception ex ) { Debug. WriteLine ( $"DataGrid failed search in Bank for match to {custno} : {bankno}" ); }
                }
                if ( success == false )
                    Debug. WriteLine ( $"DataGrid failed search in Bank for match to {custno} : {bankno}" );
            }
            this . SelectionInAction = false;
            if ( success )
                Utils . ScrollRecordIntoView ( dgUserctrl.grid1  , newindex );
            Debug. WriteLine ( $"DataGrid : {CurrentIndex} ...." );
        }
   
    }
}
