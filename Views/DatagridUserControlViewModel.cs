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

using static NewWpfDev . Views . Tabview;

namespace NewWpfDev . ViewModels {
    [Serializable]
    public class DatagridUserControlViewModel {
        #region declarations
        //public object Viewmodel {
        //    get; set;
        //}
        private DataGrid grid1 {
            get; set;
        }

        public ObservableCollection<BankAccountViewModel> Bvm {
            get; private set;
        }
        public ObservableCollection<CustomerViewModel> Cvm {
            get; private set;
        }
        private static TabWinViewModel Controller {
            get; set;
        }
        private static DataGrid dgrid1 {
            get; set;
        }
        //public static DgUserControl dgUserctrl {
        //    get; set;
        //}
        public static object HostViewModel {
            get; set;
        }
        //public static Tabview tabview {
        //    get; set;
        //}
        public string CurrentType { set; get; } = "BANK";
        private static DgUserControl ThisWin {
            get; set;
        }
        private bool IsLoaded { get; set; } = false;
        public bool SelectionInAction { get; set; } = false;
        public static bool TrackselectionChanges { get; set; } = false;
        private int CurrentIndex { get; set; } = 0;

        #endregion declarations

        #region OnPropertyChanged
        [field: NonSerialized ( )]
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged ( string propertyName ) {
            if ( PropertyChanged != null ) {
                PropertyChanged ( this , new PropertyChangedEventArgs ( propertyName ) );
            }
        }
        #endregion OnPropertyChanged

        // Constructor
        public DatagridUserControlViewModel ( ) {
        }
        public DatagridUserControlViewModel ( DgUserControl ctrl ) {
 //           Debug . WriteLine ( $"DataGrid ViewModel (DATAGRIDUSERCONTROLVIEWMODEL(DgUserctrl) Loading ......" );
            // Setup pointer to our user control (DgUserComtrol)
            Tabview . Tabcntrl . dgUserctrl = ctrl;
            //            tabview = Tabview . GetTabview ( );
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
        }

        public void DgLoadDb ( object sender , LoadDbArgs e ) {
            if ( e . dbname == "BANK" )
                LoadBank ( );
            else
                LoadCustomer ( );
        }

        private void CreateBankColumns ( ) {
            Tabview . Tabcntrl . dgUserctrl . grid1 . Columns . Clear ( );
            //Debug . WriteLine ( $"CREATING BANK COLUMNS" );
            DataGridTextColumn c1 = new DataGridTextColumn ( );
            c1 . Header = "Id";
            c1 . Binding = new Binding ( "Id" );
            Tabview . Tabcntrl . dgUserctrl . grid1 . Columns . Add ( c1 );
            DataGridTextColumn c2 = new DataGridTextColumn ( );
            c2 . Header = "Customer #";
            c2 . Binding = new Binding ( "CustNo" );
            Tabview . Tabcntrl . dgUserctrl . grid1 . Columns . Add ( c2 );
            DataGridTextColumn c3 = new DataGridTextColumn ( );
            c3 . Header = "Bank #";
            c3 . Binding = new Binding ( "BankNo" );
            Tabview . Tabcntrl . dgUserctrl . grid1 . Columns . Add ( c3 );
            DataGridTextColumn c4 = new DataGridTextColumn ( );
            c4 . Header = "A/c Type";
            c4 . Binding = new Binding ( "AcType" );
            Tabview . Tabcntrl . dgUserctrl . grid1 . Columns . Add ( c4 );
            DataGridTextColumn c5 = new DataGridTextColumn ( );
            c5 . Header = "Balance";
            c5 . Binding = new Binding ( "Balance" );
            Tabview . Tabcntrl . dgUserctrl . grid1 . Columns . Add ( c5 );
            DataGridTextColumn c6 = new DataGridTextColumn ( );
            c6 . Header = "Opened";
            c6 . Binding = new Binding ( "ODate" );
            Tabview . Tabcntrl . dgUserctrl . grid1 . Columns . Add ( c6 );
            DataGridTextColumn c7 = new DataGridTextColumn ( );
            c7 . Header = "Closed";
            c7 . Binding = new Binding ( "CDate" );
            Tabview . Tabcntrl . dgUserctrl . grid1 . Columns . Add ( c7 );
        }
        private void CreateCustomerColumns ( ) {
            Tabview . Tabcntrl . dgUserctrl . grid1 . Columns . Clear ( );
            //Debug . WriteLine ( $"CREATING CUSTOMER COLUMNS" );
            DataGridTextColumn c1 = new DataGridTextColumn ( );
            c1 . Header = "Id";
            c1 . Binding = new Binding ( "Id" );
            Tabview . Tabcntrl . dgUserctrl . grid1 . Columns . Add ( c1 );
            DataGridTextColumn c2 = new DataGridTextColumn ( );
            c2 . Header = "Customer #";
            c2 . Binding = new Binding ( "CustNo" );
            Tabview . Tabcntrl . dgUserctrl . grid1 . Columns . Add ( c2 );
            DataGridTextColumn c3 = new DataGridTextColumn ( );
            c3 . Header = "Bank #";
            c3 . Binding = new Binding ( "BankNo" );
            Tabview . Tabcntrl . dgUserctrl . grid1 . Columns . Add ( c3 );
            DataGridTextColumn c4 = new DataGridTextColumn ( );
            c4 . Header = "A/c Type";
            c4 . Binding = new Binding ( "AcType" );
            Tabview . Tabcntrl . dgUserctrl . grid1 . Columns . Add ( c4 );
            DataGridTextColumn c5 = new DataGridTextColumn ( );
            c5 . Header = "Address1";
            c5 . Binding = new Binding ( "Addr1" );
            Tabview . Tabcntrl . dgUserctrl . grid1 . Columns . Add ( c5 );
            DataGridTextColumn c6 = new DataGridTextColumn ( );
            c6 . Header = "Address2";
            c6 . Binding = new Binding ( "Addr2" );
            Tabview . Tabcntrl . dgUserctrl . grid1 . Columns . Add ( c6 );
            DataGridTextColumn c7 = new DataGridTextColumn ( );
            c7 . Header = "Town";
            c7 . Binding = new Binding ( "Town" );
            Tabview . Tabcntrl . dgUserctrl . grid1 . Columns . Add ( c7 );
            DataGridTextColumn c8 = new DataGridTextColumn ( );
            c8 . Header = "County";
            c8 . Binding = new Binding ( "County" );
            Tabview . Tabcntrl . dgUserctrl . grid1 . Columns . Add ( c8 );
            DataGridTextColumn c9 = new DataGridTextColumn ( );
            c9 . Header = "Zip";
            c9 . Binding = new Binding ( "PCode" );
            Tabview . Tabcntrl . dgUserctrl . grid1 . Columns . Add ( c9 );
            DataGridTextColumn c10 = new DataGridTextColumn ( );
            c10 . Header = "Opened";
            c10 . Binding = new Binding ( "ODate" );
            Tabview . Tabcntrl . dgUserctrl . grid1 . Columns . Add ( c10 );
            DataGridTextColumn c11 = new DataGridTextColumn ( );
            c11 . Header = "Closed";
            c11 . Binding = new Binding ( "CDate" );
            Tabview . Tabcntrl . dgUserctrl . grid1 . Columns . Add ( c11 );
        }
        public void LoadBank ( bool update = true ) {
            BankCollection bankcollection = new BankCollection ( );
            Tabview . Tabcntrl . dgUserctrl . grid1 . ItemsSource = null;
            Tabview . Tabcntrl . dgUserctrl . grid1 . Items . Clear ( );
            if ( Bvm == null ) Bvm = new ObservableCollection<BankAccountViewModel> ( );
            CurrentType = "BANK";
            Tabview . SetDbType ( "BANK" );
            TabWinViewModel . TriggerDbType ( CurrentType );

            Tabview . Tabcntrl . ActiveControlType = Tabview . Tabcntrl . dgUserctrl;
            Tabview . Tabcntrl . DtTemplates . TemplateListLb = "BANK";
            Tabview . Tabcntrl . CurrentTypeDg = "BANK";
            if ( Tabview . Tabcntrl . DtTemplates . TemplatesCombo != null ) {
                Tabview . Tabcntrl . DtTemplates . TemplatesCombo . ItemsSource = Tabview . DataTemplatesCust;
                Tabview . Tabcntrl . DtTemplates . TemplatesCombo . SelectedIndex = 0;
            }

            Task task = Task . Run ( ( ) => {
                // This is pretty fast - uses Dapper and Linq
                Application . Current . Dispatcher . Invoke ( ( ) => {
                    UserControlDataAccess . GetBankObsCollectionAsync ( Bvm , "" , true , "DgUserControl" );
                } );
                return Bvm;
            } );

        }
        public void LoadCustomer ( bool update = true ) {
            Tabview . Tabcntrl . dgUserctrl . grid1 . ItemsSource = null;
            Tabview . Tabcntrl . dgUserctrl . grid1 . Items . Clear ( );
            CurrentType = "CUSTOMER";
            Tabview . SetDbType ( "CUSTOMER" );
            TabWinViewModel . TriggerDbType ( CurrentType );

            Tabview . Tabcntrl . ActiveControlType = Tabview . Tabcntrl . dgUserctrl;

            if ( Cvm == null ) Cvm = new ObservableCollection<CustomerViewModel> ( );
            Tabview . Tabcntrl . DtTemplates . TemplateListDg = "CUSTOMER";
            Tabview . Tabcntrl . CurrentTypeDg = "CUSTOMER";
            Tabview . Tabcntrl . DtTemplates . TemplatesCombo . ItemsSource = Tabview . DataTemplatesBank;
            Tabview . Tabcntrl . DtTemplates . TemplatesCombo . SelectedIndex = 0;
            Tabview . Tabcntrl . DtTemplates . TemplateIndexDg = 0;
            //}

            Task task = Task . Run ( ( ) => {
                // This is pretty fast - uses Dapper and Linq
                Application . Current . Dispatcher . Invoke ( ( ) => {
                    Cvm = ( ObservableCollection<CustomerViewModel> ) UserControlDataAccess . GetCustObsCollection ( Cvm , "" , true , "DgUserControl" );
                } );
            } );
        }
        public void EventControl_BankDataLoaded ( object sender , LoadedEventArgs e ) {
            if ( e . CallerType != "DgUserControl" ) return;
            //if ( dgUserctrl. grid1 .  Items . Count > 0 && CurrentType != "BANK" ) return;
            CurrentType = "BANK";
            TabWinViewModel . TriggerDbType ( CurrentType );
            //await Dispatcher . SwitchToUi ( );
            //$"Dispatcher on UI thread =  {Application . Current . Dispatcher . CheckAccess ( )}" . CW ( );
            //Debug . WriteLine ( $"Data requested by : [{e . CallerDb}]" );
            Tabview . Tabcntrl . dgUserctrl . grid1 . ItemsSource = null;
            Tabview . Tabcntrl . dgUserctrl . grid1 . Items . Clear ( );
            CreateBankColumns ( );
            Bvm = e . DataSource as ObservableCollection<ViewModels . BankAccountViewModel>;
            Tabview . Tabcntrl . dgUserctrl . grid1 . ItemsSource = Bvm;
            Tabview . Tabcntrl . dgUserctrl . grid1 . SelectedIndex = 0;
            Tabview . Tabcntrl . dgUserctrl . grid1 . SelectedItem = 0;

            Tabview . Tabcntrl . DtTemplates . TemplateListDg = "BANK";
            Tabview . Tabcntrl . CurrentTypeDg = "BANK";
            //            Application . Current . Dispatcher . Invoke ( ( ) => {
            Tabview . Tabcntrl . DtTemplates . TemplatesCombo . ItemsSource = Tabview . DataTemplatesBank;
            Tabview . Tabcntrl . DtTemplates . TemplatesCombo . SelectedIndex = 0;
            Tabview . Tabcntrl . ActiveControlType = Tabview . Tabcntrl . dgUserctrl;
            Tabview . Tabcntrl . DtTemplates . TemplateIndexDg = 0;
            //            } );
            Tabview . Tabcntrl . dgUserctrl . grid1 . UpdateLayout ( );
            Utils . ScrollRecordIntoView ( Tabview . Tabcntrl . dgUserctrl . grid1 , 0 );
            //Debug . WriteLine ( $"Data Loaded for: [{e . CallerDb}], Records = {Tabview . Tabcntrl . dgUserctrl . grid1 . Items . Count}" );
            DbCountArgs args = new DbCountArgs ( );
            args . Dbcount = Bvm?.Count ?? -1;
            args . sender = "dgUserctrl";
            TabWinViewModel . TriggerBankDbCount ( this , args );
            //DataTemplate dt = Application . Current . FindResource ( "BankDataTemplate1" ) as DataTemplate;
            //Tabview . Tabcntrl . dgUserctrl . grid1 . ItemTemplate = dt;

        }

        public void EventControl_CustDataLoaded ( object sender , LoadedEventArgs e ) {
            if ( e . CallerType != "DgUserControl" ) return;
            //            if ( dgUserctrl. grid1 .  Items . Count > 0 && CurrentType != "CUSTOMER" ) return;

            CurrentType = "CUSTOMER";
            TabWinViewModel . TriggerDbType ( CurrentType );
            Tabview . Tabcntrl . dgUserctrl . grid1 . ItemsSource = null;
            Tabview . Tabcntrl . dgUserctrl . grid1 . Items . Clear ( );
            CreateCustomerColumns ( );
            Cvm = new ObservableCollection<ViewModels . CustomerViewModel> ( );
            Cvm = e . DataSource as ObservableCollection<ViewModels . CustomerViewModel>;
            Tabview . Tabcntrl . dgUserctrl . grid1 . ItemsSource = Cvm;
            Tabview . Tabcntrl . dgUserctrl . grid1 . CellStyle = Application . Current . FindResource ( "MAINCustomerGridStyle" ) as Style;

            DataTemplate dt = Application . Current . FindResource ( "CustomersDbTemplate1" ) as DataTemplate;
            Tabview . Tabcntrl . dgUserctrl . grid1 . ItemTemplate = dt;
            Tabview . Tabcntrl . dgUserctrl . grid1 . SelectedIndex = 0;
            Tabview . Tabcntrl . dgUserctrl . grid1 . SelectedItem = 0;

            Tabview . Tabcntrl . DtTemplates . TemplateListDg = "CUSTOMER";
            Tabview . Tabcntrl . CurrentTypeDg = "CUSTOMER";
            Application . Current . Dispatcher . Invoke ( ( ) => {
                Tabview . Tabcntrl . DtTemplates . TemplatesCombo . ItemsSource = Tabview . DataTemplatesCust;
                Tabview . Tabcntrl . DtTemplates . TemplatesCombo . SelectedIndex = 0;
                Tabview . Tabcntrl . ActiveControlType = Tabview . Tabcntrl . dgUserctrl;
                Tabview . Tabcntrl . ActiveControlType = Tabview . Tabcntrl . dgUserctrl;
                Tabview . Tabcntrl . DtTemplates . TemplateIndexDg = 0;
            } );

            Utils . ScrollRecordIntoView ( grid1 , 0 );
            DbCountArgs args = new DbCountArgs ( );
            args . Dbcount = Cvm?.Count ?? -1;
            args . sender = "dgUserctrl";
            TabWinViewModel . TriggerBankDbCount ( this , args );
        }

        public void grid1_SelectionChanged ( object sender , SelectionChangedEventArgs e ) {
            if ( Tabview . Tabcntrl . Selectionchanged == true ) return;
            if ( Tabview . Tabcntrl . dgUserctrl . grid1 . Items . Count == 0 ) return;
            if ( Tabview . Tabcntrl . dgUserctrl . grid1 . SelectedIndex == -1 ) {
                //make sure we have something selected !
                Tabview . Tabcntrl . dgUserctrl . grid1 . SelectedIndex = 0;
                Tabview . Tabcntrl . dgUserctrl . grid1 . SelectedItem = 0;
            }
            //            DataGrid v = e . OriginalSource as DataGrid;
            DataGrid v = e . OriginalSource as DataGrid;
            if ( v?.SelectedIndex != CurrentIndex ) {
                (double, int) t1 = (4.5, 3);
                CurrentIndex = v . SelectedIndex;

                if ( Tabview . Tabcntrl . tabView . ViewersLinked && Tabview . Tabcntrl . Selectionchanged == false ) {
                    if ( Tabview . Tabcntrl . tabView . CheckAllControlIndexes ( CurrentIndex ) == false ) {
                        Tabview . Tabcntrl . Selectionchanged = true;
                        SelectionChangedArgs args = new SelectionChangedArgs ( );
                        args . index = Tabview . Tabcntrl . dgUserctrl . grid1 . SelectedIndex;
                        args . data = Tabview . Tabcntrl . dgUserctrl . grid1 . SelectedItem;
                        args . sendertype = CurrentType;
                        args . sendername = "grid1";
                        Debug . WriteLine ( $"DataGrid broadcasting selection set to  {args . index}" );
                        //                 this . SelectionInAction = false;
                        Task task = Task . Run ( ( ) => {
                            Application . Current . Dispatcher . Invoke ( ( ) => {
                                EventControl . TriggerListSelectionChanged ( sender , args );
                            } );
                        } );
                        e . Handled = true;
                    }
                }
            }

            Mouse . OverrideCursor = Cursors . Arrow;
            CurrentIndex = Tabview . Tabcntrl . dgUserctrl . grid1 . SelectedIndex;
//            Tabview . Tabcntrl . DtTemplates . TemplateIndexDg = Tabview . Tabcntrl . dgUserctrl . grid1 . SelectedIndex;
            //Debug . WriteLine ( $"{CurrentIndex} ...." );
            Tabview . Tabcntrl . Selectionchanged = false;
        }

        public void SelectionHasChanged ( object sender , SelectionChangedArgs e ) {
            bool success = false;
            object row = null;
            string custno = "", bankno = "";
            if ( DgUserControl . TrackselectionChanges == false ) return;

            //Debug . WriteLine ( $"dgUsercontrol : {e . sendername}" );

            // Another viewer has changed selection
            if ( Tabview . Tabcntrl . dgUserctrl . grid1 . ItemsSource == null ) return;
            if ( sender . GetType ( ) == typeof ( DgUserControl ) ) return;

            int newindex = 0;
            if ( e . sendername != "grid1" ) {
                if ( e . sendertype == "BANK" ) {
                    // Sender is a BANK
                    BankAccountViewModel sourcerecord = new BankAccountViewModel ( );
                    sourcerecord = e . data as BankAccountViewModel;
                    if ( sourcerecord != null ) {
                        custno = sourcerecord . CustNo;
                        bankno = sourcerecord . BankNo;
                    }
                    else return;
                }
                else if ( e . sendertype == "CUSTOMER" ) {
                    // Sender is a CUSTOMER
                    CustomerViewModel sourcerecord = new CustomerViewModel ( );
                    sourcerecord = e . data as CustomerViewModel;
                    if ( sourcerecord != null ) {
                        custno = sourcerecord . CustNo;
                        bankno = sourcerecord . BankNo;
                    }
                    else return;
                }
                if ( this . CurrentType == "CUSTOMER" ) {
                    try {
                        foreach ( CustomerViewModel item in Tabview . Tabcntrl . dgUserctrl . grid1 . Items ) {
                            if ( item . CustNo == custno ) { //} && item . BankNo == bankno ) {
                                Tabview . Tabcntrl . Selectionchanged = true;
                                Tabview . Tabcntrl . dgUserctrl . grid1 . SelectedIndex = newindex;
                                Tabview . Tabcntrl . dgUserctrl . grid1 . SelectedItem = newindex;
                                CurrentIndex = newindex;
                                Debug . WriteLine ( $"DataGrid selection in Customers matched on {custno}:{bankno}, index {newindex}" );
                                Utils . ScrollRecordIntoView ( grid1 , newindex );
                                success = true;
                                break;
                            }
                            newindex++;
                        }
                    }
                    catch ( Exception ex ) { Debug . WriteLine ( $"DataGrid failed search in Customer for match to {custno} : {bankno} : {ex . Message}" ); }
                }
                else {
                    try {
                        foreach ( BankAccountViewModel item in Tabview . Tabcntrl . dgUserctrl . grid1 . Items ) {
                            if ( item . CustNo == custno ) { //} && item . BankNo == bankno ) {
                                Tabview . Tabcntrl . Selectionchanged = true;
                                Tabview . Tabcntrl . dgUserctrl . grid1 . SelectedIndex = newindex;
                                Tabview . Tabcntrl . dgUserctrl . grid1 . SelectedItem = newindex;
                                CurrentIndex = newindex;
                                Debug . WriteLine ( $"DataGrid selection in BankAccount matched on {custno}:{bankno}, index {newindex}" );
                                //row = Utils . GetRow ( grid1 , newindex );
                                Utils . ScrollRecordIntoView ( Tabview . Tabcntrl . dgUserctrl . grid1 , newindex );
                                Tabview . Tabcntrl . dgUserctrl . grid1 . UpdateLayout ( );
                                success = true;
                                break;
                            }
                            newindex++;
                        }
                    }
                    catch ( Exception ex ) { Debug . WriteLine ( $"DataGrid failed search in Bank for match to {custno} : {bankno}" ); }
                }
                if ( success == false ) {
                    if ( this . CurrentType == "BANK" )
                        Debug . WriteLine ( $"Datagrid failed search in Bank for match to {custno} : {bankno}" );
                    else
                        Debug . WriteLine ( $"Datagrid failed search in Customer for match to {custno} : {bankno}" );
                }
            }
            this . SelectionInAction = false;
            if ( success ) {
                Utils . ScrollRecordIntoView ( Tabview . Tabcntrl . dgUserctrl . grid1 , newindex );
                //Tabview . Tabcntrl . DtTemplates . TemplateIndexDg = newindex;
            }
            Tabview . Tabcntrl . Selectionchanged = false;
        }

    }
}
