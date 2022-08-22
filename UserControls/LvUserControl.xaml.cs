using System;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Diagnostics;
using System . Globalization;
using System . IO;
using System . Runtime . Serialization . Formatters . Binary;
using System . Threading;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;
using System . Windows . Media;

using NewWpfDev . Converts;
using NewWpfDev . SQL;
using NewWpfDev . ViewModels;
using NewWpfDev . Views;

using static NewWpfDev . Views . Tabview;

namespace NewWpfDev . UserControls {
    [Serializable ( )]
    public partial class LvUserControl : UserControl, ITabViewer {

        #region NotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged ( string propertyName ) {
            if ( PropertyChanged != null ) {
                PropertyChanged ( this , new PropertyChangedEventArgs ( propertyName ) );
            }
        }
        #endregion NotifyPropertyChanged

        const string FileName = @"LvUserControl.bin";

        public object Viewmodel {
            get; set;
        }
        private double fontsize;
        public double Fontsize {
            get { return fontsize; }
            set { fontsize = value; NotifyPropertyChanged ( nameof ( Fontsize ) ); }
        }

        public ObservableCollection<BankAccountViewModel> Bvm {
            get; private set;
        }
        public ObservableCollection<CustomerViewModel> Cvm { get; private set; }
        public ObservableCollection<GenericClass> Gvm { get; private set; }
        public string CurrentType {
            get; set;
        }
        public TabControl tabControl {
            get; set;
        }
        private static LvUserControl ThisWin {
            get; set;
        }
        private static Tabview tabviewWin {
            get; set;
        }
        private static bool SelectionInAction { get; set; } = false;
        public static bool TrackselectionChanges { get; set; } = false;
        private int CurrentIndex { get; set; } = 0;

        #region Events
        public static event EventHandler<SelectionChangedArgs> ListSelectionChanged;
        //public static event EventHandler<DbTypeArgs> GetCurrentDbType;
        //protected virtual void onGetCurrentDbType ()
        //{
        //    DbTypeArgs args = new DbTypeArgs();
        //    args . Dbname = CurrentType;
        //    if ( GetCurrentDbType != null )
        //        GetCurrentDbType . Invoke(this , args);
        //}
        protected virtual void OnListSelectionChanged ( int index , object item , UIElement ctrl ) {
            SelectionChangedArgs args = new SelectionChangedArgs ( );
            args . index = index;
            args . data = item;
            args . element = ctrl;
            args . sendername = CurrentType;
            if ( ListSelectionChanged != null )
                ListSelectionChanged ( this , args );
        }

        #endregion events
        public LvUserControl ( ) {
            InitializeComponent ( );
            Debug . WriteLine ( $"LvUserControl Loading ......" );
            ThisWin = this;
            Fontsize = 14;
            listview1 . FontSize = Fontsize;
            // setup DP pointer in Tabview to LvUserControl using shortcut command line !
            Tabview . GetTabview ( ) . Lvusercontrol = this;
            Tabview . Tabcntrl . lvUserctrl = this;
            // setup local data collections
            Bvm = new ObservableCollection<BankAccountViewModel> ( );
            Cvm = new ObservableCollection<CustomerViewModel> ( );
            EventControl . ListSelectionChanged += SelectionHasChanged;
            EventControl . BankDataLoaded += EventControl_BankDataLoaded;
            EventControl . CustDataLoaded += EventControl_CustDataLoaded;
            EventControl . GenDataLoaded += EventControl_GenericDataLoaded;
            EventControl . TriggerWindowMessage ( this , new InterWindowArgs { message = "LvUserControl  loaded..." } );
            Gvm = ITabViewer . Gvm;
        }

        private void UserControl_Loaded ( object sender , RoutedEventArgs e ) {
        }
        public static void SetListSelectionChanged ( bool arg ) {
            TrackselectionChanges = arg;
        }
        private void ReadSerializedObject ( ) {
            //Debug . WriteLine ( "Reading saved file" );
            Stream openFileStream = File . OpenRead ( FileName );
            BinaryFormatter deserializer = new BinaryFormatter ( );
            Tabview . Tabcntrl . dgUserctrl = ( DgUserControl ) deserializer . Deserialize ( openFileStream );
            //TestLoan . TimeLastLoaded = DateTime . Now;
            openFileStream . Close ( );
        }
        //public static void WriteSerializedObject ( ) {
        //    Stream SaveFileStream = File . Create ( FileName );
        //    BinaryFormatter serializer = new BinaryFormatter ( );
        //    serializer . Serialize ( SaveFileStream , Tabview . Tabcntrl . dgUserctrl );
        //    SaveFileStream . Close ( );
        //}
        private void EventControl_BankDataLoaded ( object sender , LoadedEventArgs e ) {
            if ( e . CallerType != "LvUserControl" ) return;
            Application . Current . Dispatcher . Invoke ( async ( ) => {
                await DispatcherExtns . SwitchToUi ( Application . Current . Dispatcher );
               // Debug . WriteLine ( $"\nInside Dispatcher" );
                //$"Dispatcher on UI thread =  {Application . Current . Dispatcher . CheckAccess ( )}" . CW ( );
               Bvm = e . DataSource as ObservableCollection<BankAccountViewModel>;
                //if ( Bvm . Count == 0 ) {
                //    Thread . Sleep ( 250 );
                //    while ( true ) {
                //        Bvm = e . DataSource as ObservableCollection<BankAccountViewModel>;
                //        if ( Bvm . Count == 0 )
                //            Thread . Sleep ( 250 );
                //        else
                //            break;
                //    }
                //}
                //return;
                // NB WE MUST reference  the usercontrol listview by using :-
                // listview1 , not via this.listview1
                //Debug . WriteLine ( $"Listview received {Bvm . Count} records " );
                listview1 . ItemsSource = null;
                listview1 . ItemsSource = Bvm;
                listview1 . SelectedIndex = 0;
                CurrentType = "BANK";
                TabWinViewModel . TriggerDbType ( CurrentType );
                Tabview . Tabcntrl . DtTemplates . TemplatesCombo . ItemsSource = Tabview . DataTemplatesBank;
                Tabview . Tabcntrl . DtTemplates . TemplatesCombo . SelectedIndex = 0;
                Tabview . Tabcntrl . tabView . IsLoading = true;
                Tabview . Tabcntrl . tabView . DbnamesCb . SelectedIndex = FindDbName ( "BANKACCOUNT" );
                Tabview . Tabcntrl . DbNameIndexLv = Tabview . Tabcntrl . tabView . DbnamesCb . SelectedIndex;
                Tabview . Tabcntrl . tabView . IsLoading = false;
                Tabview . Tabcntrl . ActiveControlType = Tabview . Tabcntrl . lvUserctrl;
                Tabview . Tabcntrl . ActiveControlType = Tabview . Tabcntrl . lvUserctrl;
                Tabview . Tabcntrl . DtTemplates . TemplateIndexLv = 0;
                Tabview . Tabcntrl . tabView . TemplatesCb . Foreground = FindResource ( "Black0" ) as SolidColorBrush;
                Tabview . Tabcntrl . tabView . TemplatesCb . UpdateLayout ( );
                Tabview . Tabcntrl . tabView . DbnamesCb . Foreground = FindResource ( "Black0" ) as SolidColorBrush;
                Tabview . Tabcntrl . tabView . DbnamesCb . UpdateLayout ( );

                FrameworkElement elemnt = Tabview . Tabcntrl . lvUserctrl . listview1 as FrameworkElement;
                DataTemplate dtemp = new DataTemplate ( );
                dtemp . Seal ( );
                dtemp = elemnt . FindResource ( Tabview . Tabcntrl . DtTemplates . TemplatesCombo . SelectedItem . ToString ( ) ) as DataTemplate;
                Tabview . Tabcntrl . lvUserctrl . listview1 . ItemTemplate = dtemp;
                Tabview . Tabcntrl . lvUserctrl . listview1 . UpdateLayout ( );
                listview1 . UpdateLayout ( );

                Mouse . OverrideCursor = Cursors . Arrow;
                //Debug . WriteLine ( $"Bank data load via eventcontrol message !!!!!!" );
                //            ListView lv = Tabview . Tabcntrl . lvUserctrl . listview1;
                ReduceByParamValue converter = new ReduceByParamValue ( );
                if ( tabControl != null ) {
                    listview1 . Height = Tabview . Tabcntrl . lvUserctrl . Height = Convert . ToDouble ( converter . Convert ( tabControl?.ActualHeight , typeof ( double ) , 40 , CultureInfo . CurrentCulture ) );
                    listview1 . Width = Tabview . Tabcntrl . lvUserctrl . Width = Convert . ToDouble ( converter . Convert ( tabControl?.ActualWidth , typeof ( double ) , 10 , CultureInfo . CurrentCulture ) );
                }
                //Debug . WriteLine ( $"Exited Dispatcher\n" );
                Utils . ScrollLVRecordIntoView ( listview1 , 0 );
                listview1 . Refresh ( );
                DbCountArgs args = new DbCountArgs ( );
                args . Dbcount = Bvm?.Count ?? -1;
                args . sender = "dgUserctrl";
                TabWinViewModel . TriggerBankDbCount ( this , args );
                TabWinViewModel . TriggerDbType ( CurrentType );
            } );
            //Debug . WriteLine ( $"Exited Dispatcher\n" );
        }
        private void EventControl_CustDataLoaded ( object sender , LoadedEventArgs e ) {
            //            if ( listview1 . Items . Count > 0 && CurrentType != "CUSTOMER" ) return;
            if ( e . CallerType != "LvUserControl" ) return;

            Application . Current . Dispatcher . Invoke ( async ( ) => {
                await DispatcherExtns . SwitchToUi ( Application . Current . Dispatcher );
                //Debug . WriteLine ( $"\nInside Dispatcher" );
                $"Dispatcher on UI thread =  {Application . Current . Dispatcher . CheckAccess ( )}" . CW ( );
                Cvm = e . DataSource as ObservableCollection<CustomerViewModel>;
                this . listview1 . ItemsSource = Cvm;
                this . listview1 . SelectedIndex = 0;
                this . listview1 . ItemsSource = Cvm;
                CurrentType = "CUSTOMER";
                TabWinViewModel . TriggerDbType ( CurrentType );
                Tabview . Tabcntrl . DtTemplates . TemplatesCombo . ItemsSource = Tabview . DataTemplatesCust;
                Tabview . Tabcntrl . DtTemplates . TemplatesCombo . SelectedIndex = 0;
                Tabview . Tabcntrl . tabView . IsLoading = true;
                Tabview . Tabcntrl . tabView . DbnamesCb . SelectedIndex = FindDbName ( "CUSTOMER" );
                Tabview . Tabcntrl . DbNameIndexLv = Tabview . Tabcntrl . tabView . DbnamesCb . SelectedIndex;
                Tabview . Tabcntrl . tabView . IsLoading = false;
                Tabview . Tabcntrl . ActiveControlType = Tabview . Tabcntrl . lvUserctrl;

                Tabview . Tabcntrl . tabView . TemplatesCb . Foreground = FindResource ( "Black0" ) as SolidColorBrush;
                Tabview . Tabcntrl . tabView . TemplatesCb . UpdateLayout ( );
                Tabview . Tabcntrl . tabView . DbnamesCb . Foreground = FindResource ( "Black0" ) as SolidColorBrush;
                Tabview . Tabcntrl . tabView . DbnamesCb . UpdateLayout ( );

                Tabview . Tabcntrl . DtTemplates . TemplateIndexLv = 0;
                FrameworkElement elemnt = Tabview . Tabcntrl . lvUserctrl . listview1 as FrameworkElement;
                DataTemplate dtemp = new DataTemplate ( );
                dtemp . Seal ( );
                dtemp = elemnt . FindResource ( Tabview . Tabcntrl . DtTemplates . TemplatesCombo . SelectedItem . ToString ( ) ) as DataTemplate;
                Tabview . Tabcntrl . lvUserctrl . listview1 . ItemTemplate = dtemp;
                Tabview . Tabcntrl . lvUserctrl . listview1 . UpdateLayout ( );
                Utils . ScrollLVRecordIntoView ( listview1 , 0 );
                Mouse . OverrideCursor = Cursors . Arrow;
                DbCountArgs args = new DbCountArgs ( );
                args . Dbcount = Cvm?.Count ?? -1;
                args . sender = "dgUserctrl";
                TabWinViewModel . TriggerBankDbCount ( this , args );
                TabWinViewModel . TriggerDbType ( CurrentType );
            } );
            //Debug . WriteLine ( $"Exited Dispatcher\n" );
        }

        private void EventControl_GenericDataLoaded ( object sender , LoadedEventArgs e ) {
            if ( e . CallerType != "GENERICDATA" ) return;
            this . Dispatcher . Invoke ( ( ) => {
                this . listview1 . ItemsSource = null;
                Grid v = this . listview1 . Parent as Grid;
                TabItem y = v . Parent as TabItem;
                this . listview1 . Items . Clear ( );
                Gvm = e . DataSource as ObservableCollection<GenericClass>;
                this . listview1 . ItemsSource = Gvm;
                CurrentType = "GENERIC";
                TabWinViewModel . TriggerDbType ( CurrentType );
                DataTemplate dt = FindResource ( "GenericTemplate" ) as DataTemplate;
                this . listview1 . ItemTemplate = dt;
                this . listview1 . SelectedIndex = 0;
                this . listview1 . SelectedItem = 0;
                this . listview1 . UpdateLayout ( );
                Tabview . Tabcntrl . tabView . DbnamesCb . SelectedIndex = Tabview . FindDbName ( Tabview . Tabcntrl . tabView . DbnamesCb . SelectedItem . ToString ( ) );
                Tabview . Tabcntrl . DbNameIndexLv = Tabview . Tabcntrl . tabView . DbnamesCb . SelectedIndex;

                // Set colors of Indicator panels on Tabview
                Tabview . tabvw . DbTypeFld . Background = FindResource ( "Red5" ) as SolidColorBrush;
                Tabview . tabvw . DbCount . Background = Application . Current . FindResource ( "Red5" ) as SolidColorBrush;
                Tabview . tabvw . DbTypeFld . UpdateLayout ( );
                Tabview . Tabcntrl . tabView . TemplatesCb . Foreground = FindResource ( "Red5" ) as SolidColorBrush;
                Tabview . Tabcntrl . tabView . TemplatesCb . UpdateLayout ( );
                Tabview . Tabcntrl . tabView . DbnamesCb . Foreground = FindResource ( "Red5" ) as SolidColorBrush;
                Tabview . Tabcntrl . tabView . DbnamesCb . UpdateLayout ( );

                Utils . ScrollLBRecordIntoView ( this . listview1 , 0 );
                this . listview1 . Refresh ( );
            } );
            Mouse . OverrideCursor = Cursors . Arrow;
        }

        public async Task LoadBank ( bool arg = true ) {
            Tabview . Tabcntrl . ActiveControlType = Tabview . Tabcntrl . lvUserctrl;
            //Setup Templates list  as we are changing Db type
            Application . Current . Dispatcher . Invoke ( async ( ) => {
                Tabview . Tabcntrl . DtTemplates . TemplateNameLv = "BANKACCOUNT";
                Tabview . Tabcntrl . CurrentTypeLv = "BANKACCOUNT";
                Task . Run ( ( ) => SetDbType ( "BANK" ) );
                Task . Run ( async ( ) => await LoadBankDb ( arg ) );
            } );
            //Application . Current . Dispatcher . Invoke ( async ( ) =>
            //    await Task . Run ( async ( ) => await LoadBankDb ( arg ) )
            //    );

            if ( listview1 . ItemsSource == null ) {
                if ( Bvm . Count > 0 )
                    listview1 . ItemsSource = Bvm;
            }
            return;
        }
        public async Task LoadCustomerDb ( bool data = true ) {
            Tabview . Tabcntrl . ActiveControlType = Tabview . Tabcntrl . lvUserctrl;

            Tabview . Tabcntrl . DtTemplates . TemplateNameLv = "CUSTOMER";
            Tabview . Tabcntrl . CurrentTypeLv = "CUSTOMER";
            SetDbType ( "CUSTOMER" );
            Application . Current . Dispatcher . Invoke ( ( Action ) ( ( ) => {

                // Set colors of Indicator panels on Tabview
                Tabview . tabvw . DbTypeFld . Background = FindResource ( "Blue5" ) as SolidColorBrush;
                Tabview . tabvw . DbCount . Background = Application . Current . FindResource ( "Blue5" ) as SolidColorBrush;

                this . listview1 . ItemsSource = null;
                this . listview1 . Items . Clear ( );
            } ) );
            if ( Cvm == null ) Cvm = new ObservableCollection<ViewModels . CustomerViewModel> ( );
            if ( Cvm . Count == 0 ) {
                await Task . Run ( ( ) => {
                    // This is pretty fast - uses Dapper and Linq
                    UserControlDataAccess . GetCustObsCollection ( Cvm , "" , true , "LvUserControl" );
                } );
            }
            else {
                Application . Current . Dispatcher . Invoke ( ( Action ) ( ( ) =>
                    Cvm . Clear ( )
                ) );
                await Task . Run ( ( ) => {
                    // This is pretty fast - uses Dapper and Linq
                    UserControlDataAccess . GetCustObsCollection ( Cvm , "" , true , "LvUserControl" );
                } );
            }
        }
        public Task<string> LoadCustomer ( ) {
            //Setup Templates list  as we are changing Db type
            Task . Run ( async ( ) => await LoadCustomerDb ( true ) );
            return Task . FromResult ( "" );
            //ThreadPool . QueueUserWorkItem ( LoadCustomerDb );
        }
        public int LoadGeneric ( string tablename ) {
            string ResultString = "";
            this . Dispatcher . Invoke ( ( ) => {
                string SqlCommand = tablename != null ? $"Select * from {tablename}" : "Select * from Invoice";
                SqlCommand = $"SpLoadTableAsGeneric {tablename}";
                Tabview . Tabcntrl . ActiveControlType = Tabview . Tabcntrl . lvUserctrl;
                //Setup Templates list  as we are changing Db type
                Tabview . Tabcntrl . DtTemplates . TemplateNameLv = tablename . ToUpper ( );
                Tabview . Tabcntrl . CurrentTypeLv = "GEN";
                Tabview . SetDbType ( "GEN" );

                // Set colors of Indicator panels on Tabview
                Tabview . tabvw . DbTypeFld . Background = FindResource ( "Red5" ) as SolidColorBrush;
                Tabview . tabvw . DbCount . Background = Application . Current . FindResource ( "Red5" ) as SolidColorBrush;

                Tabview . Tabcntrl . lvUserctrl . listview1 . ItemsSource = null;
                Tabview . Tabcntrl . lvUserctrl . listview1 . Items . Clear ( );
                //Gvm = new ObservableCollection<GenericClass> ( );
                Gvm = SqlSupport . LoadGeneric ( SqlCommand , out ResultString , 0 , true );
                Tabview . Tabcntrl . lvUserctrl . listview1 . ItemsSource = Gvm;

                // Set ListBox to the new Data Template
                Tabview . Tabcntrl . DbNameIndexLv = Tabview . Tabcntrl . DbNameIndexLv;
                Tabview . Tabcntrl . twVModel . CheckActiveTemplate ( Tabview . Tabcntrl . lvUserctrl );
                FrameworkElement elemnt = Tabview . Tabcntrl . lvUserctrl . listview1 as FrameworkElement;
                DataTemplate dtemp = new DataTemplate ( );
                // Lock template  - cannot be changed
                dtemp . Seal ( );
                dtemp = elemnt . FindResource ( Tabview . Tabcntrl . DtTemplates . TemplatesCombo . SelectedItem . ToString ( ) ) as DataTemplate;
                Tabview . Tabcntrl . lvUserctrl . listview1 . ItemTemplate = dtemp;
                Tabview . Tabcntrl . DtTemplates . TemplatesCombo . SelectedIndex = Tabview . Tabcntrl . DtTemplates . TemplateIndexLv;
                Tabview . Tabcntrl . lvUserctrl . listview1 . UpdateLayout ( );

                Tabview . Tabcntrl . lvUserctrl . listview1 . UpdateLayout ( );
                DbCountArgs args = new DbCountArgs ( );
                args . Dbcount = Gvm?.Count ?? -1;
                args . sender = "lvUserctrl";
                TabWinViewModel . TriggerBankDbCount ( this , args );
            } );
            return Gvm . Count;
        }

        public async Task LoadBankDb ( object data ) {
            //Async workks well !!
            if ( Bvm == null || Bvm . Count == 0 ) {
                Bvm = new ObservableCollection<ViewModels . BankAccountViewModel> ( );
                //Debug . WriteLine ( $"Calling \"UserControlDataAccess . GetBankObsCollection()\"" );
                // Allows Callee to update interface
                Application . Current . Dispatcher . Invoke ( async ( ) => {
                    await DispatcherExtns . SwitchToUi ( Application . Current . Dispatcher );
//                    Debug . WriteLine ( $"\nInside lvUserctrl Dispatcher" );
                    $"Dispatcher on UI thread =  {Application . Current . Dispatcher . CheckAccess ( )}" . CW ( );
                    await Task . Run ( async ( ) => UserControlDataAccess . GetBankObsCollection ( true , "LvUserControl" ) );
                } );
   //             Debug . WriteLine ( $"Exited lvUserctrl Dispatcher" );


                //Application . Current . Dispatcher . Invoke ( async ( ) =>
                //    await Task . Run ( async ( ) =>
                //     await UserControlDataAccess . GetBankObsCollection ( true , "LvUserControl" )
                //  )
                // ) );
            }
            else {
                await Task . Run ( async ( ) =>
                     Application . Current . Dispatcher . Invoke ( ( Action ) ( ( ) => {
                         Bvm . Clear ( );
                         Bvm = UserControlDataAccess . GetBankObsCollection ( true , "LvUserControl" );
                     }
                  )
                 ) );
            }
            return;
        }

        public static void SetSelectionInAction ( bool arg ) {
            //         SelectionInAction = arg;
        }
        private void listview1_SelectionChanged ( object sender , SelectionChangedEventArgs e ) {
            ListView lv = sender as ListView;
            if ( Tabview . Tabcntrl . Selectionchanged == true ) return;
            if ( lv == null ) return;
            if ( lv . Items . Count == 0 ) return;
            if ( lv . SelectedIndex == -1 ) {
                //make sure we have something selected !
                this . listview1 . SelectedIndex = 0;
                this . listview1 . SelectedItem = 0;
            }
            lv = e . OriginalSource as ListView;
            if ( lv . SelectedIndex != CurrentIndex ) {
                SelectionInAction = true;
                this . listview1 . SelectedIndex = lv . SelectedIndex;
                CurrentIndex = lv . SelectedIndex;
                Utils . ScrollLVRecordIntoView ( listview1 , CurrentIndex );
                if ( Tabview . Tabcntrl . tabView . ViewersLinked && Tabview . Tabcntrl . Selectionchanged == false ) {
                    if ( Tabview . Tabcntrl . CurrentTypeLv != "GEN" ) {
                        if ( Tabview . Tabcntrl . tabView . CheckAllControlIndexes ( CurrentIndex ) == false ) {
                            Tabview . Tabcntrl . Selectionchanged = true;
                            SelectionChangedArgs args = new SelectionChangedArgs ( );
                            args . data = this . listview1 . SelectedItem;
                            args . sendername = "listview1";
                            args . sendertype = CurrentType;
                            args . index = CurrentIndex;
                            Debug . WriteLine ( $"ListView broadcasting selection set to  {args . index}" );
                            SelectionInAction = false;
                            Task task = Task . Run ( ( ) => {
                                Application . Current . Dispatcher . Invoke ( ( ) => {
                                    EventControl . TriggerListSelectionChanged ( sender , args );
                                } );
                            } );
                            //                         e . Handled = true;
                        }
                    }
                }
            }

            SelectionInAction = false;
            Mouse . OverrideCursor = Cursors . Arrow;
            CurrentIndex = this . listview1 . SelectedIndex;
            Tabview . Tabcntrl . Selectionchanged = false;
            e . Handled = true;
        }

        private void SelectionHasChanged ( object sender , SelectionChangedArgs e ) {
            bool success = false;
            if ( LvUserControl . TrackselectionChanges == false ) return;
            // Another viewer has changed selection
            int newindex = 0;
            if ( this . listview1 . ItemsSource == null ) return;

            if ( Tabview . Tabcntrl . DtTemplates . TemplateNameLv != "BANKACCOUNT" && Tabview . Tabcntrl . DtTemplates . TemplateNameLv != "CUSTOMER" ) {
                Debug . WriteLine ( $"ListView  : Cannot match : View  contains Generic data" );
                return;
            }


            if ( sender . GetType ( ) == typeof ( LvUserControl ) )
                return;

            if ( e . sendername != "listview1" ) {
                string custno = "", bankno = "";
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
                        foreach ( CustomerViewModel item in this . listview1 . Items ) {
                            if ( item . CustNo == custno ) { //} && item . BankNo == bankno ) {
                                Tabview . Tabcntrl . Selectionchanged = true;
                                this . listview1 . SelectedIndex = newindex;
                                this . listview1 . SelectedItem = newindex;
                                Debug . WriteLine ( $"ListView selection in Customers matched on {custno}:{bankno}, index {newindex}" );
                                Utils . ScrollLVRecordIntoView ( listview1 , newindex );
                                Tabview . Tabcntrl . tabView . LoadName . Foreground = FindResource ( "Black0" ) as SolidColorBrush;
                                success = true;
                                break;
                            }
                            newindex++;
                        }
                    }
                    catch ( Exception ex ) {
                        Debug . WriteLine ( $"ListView failed search in Customer for match to {custno} : {bankno}" );
                    }
                }
                else {
                    try {
                        foreach ( BankAccountViewModel item in this . listview1 . Items ) {
                            if ( item . CustNo == custno ) { //} && item . BankNo == bankno ) {
                                Tabview . Tabcntrl . Selectionchanged = true;
                                this . listview1 . SelectedIndex = newindex;
                                this . listview1 . SelectedItem = newindex;
                                Debug . WriteLine ( $"ListView selection in BankAccount matched on {custno}:{bankno}, index {newindex}" );
                                Utils . ScrollLVRecordIntoView ( listview1 , newindex );
                                Tabview . Tabcntrl . tabView . LoadName . Foreground = FindResource ( "Black0" ) as SolidColorBrush;
                                success = true;
                                break;
                            }
                            newindex++;
                        }
                    }
                    catch ( Exception ex ) {
                        Debug . WriteLine ( $"XXXXXXListView failed search in Bank for match to {custno} : {bankno}" );
                    }
                }
                if ( success == false ) {
                    if ( this . CurrentType == "BANK" )
                        Debug . WriteLine ( $"Listview failed search in Bank for match to {custno} : {bankno}" );
                    else
                        Debug . WriteLine ( $"Listview failed search in Customer for match to {custno} : {bankno}" );
                    TabWinViewModel . SetInfoString ( $"ListView failed search for match to {custno} : {bankno}" );
                    Tabview . Tabcntrl . tabView . LoadName . Foreground = FindResource ( "Red5" ) as SolidColorBrush;
                }
                else {
                    TabWinViewModel . SetInfoString ( $"ListView search for match to {custno} : {bankno} Succeeded" );
                    Tabview . Tabcntrl . tabView . LoadName . Foreground = FindResource ( "Black0" ) as SolidColorBrush;
                }
            }
            if ( success ) {
                Utils . ScrollLVRecordIntoView ( this . listview1 , newindex );
            }
            this . listview1 . UpdateLayout ( );
            Tabview . Tabcntrl . Selectionchanged = false;
        }
        public void PART_MouseLeave ( object sender , MouseEventArgs e ) {
            var tabview = TabWinViewModel . Tview;
            if ( TabWinViewModel . CurrentTabTextBlock == "Tab3Header" ) {
                tabview . Tab3Header . FontSize = 14;
                Tabview . TriggerStoryBoardOff ( 3 );
                tabview . Tab3Header . Foreground = FindResource ( "Cyan0" ) as SolidColorBrush;
            }
        }

        public void PART_MouseEnter ( object sender , MouseEventArgs e ) {
            var tabview = TabWinViewModel . Tview;
            if ( TabWinViewModel . CurrentTabTextBlock == "Tab3Header" ) {
                tabview . Tab3Header . FontSize = 18;
                Tabview . TriggerStoryBoardOn ( 3 );
                tabview . Tab3Header . Foreground = FindResource ( "Yellow0" ) as SolidColorBrush;
            }
            this . listview1 . Focus ( );
        }
        private void listview1_PreviewMouseMove ( object sender , MouseEventArgs e ) {
            ListView lvSender = sender as ListView;
        }

        private void listview1_PreviewMouseLeftButtonUp ( object sender , MouseButtonEventArgs e ) {
        }

        private void listview1_KeyDown ( object sender , KeyEventArgs e ) {
            if ( e . Key == Key . Down ||
                e . Key == Key . Up ) {
                Debug . WriteLine ( $"KeyDown ={e . Key}" );
            }
        }
        private void Magnifyplus2 ( object sender , RoutedEventArgs e ) {
            Fontsize += 2;
            listview1 . FontSize = Fontsize; ;
            listview1 . UpdateLayout ( );
        }
        private void Magnifyminus2 ( object sender , RoutedEventArgs e ) {
            Fontsize -= 2;
            listview1 . FontSize = Fontsize; ;
            listview1 . UpdateLayout ( );
        }

        private void ReloadBank ( object sender , RoutedEventArgs e ) {
            Tabview . Tabcntrl . twVModel . TabLoadDb ( this , "BANKACCOUNT" , true ); ;
        }

        private void ReloadCust ( object sender , RoutedEventArgs e ) {
            Tabview . Tabcntrl . twVModel . TabLoadDb ( this , "CUSTOMER" , true );
        }
    }
}
