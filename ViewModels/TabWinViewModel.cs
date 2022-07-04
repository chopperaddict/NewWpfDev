using System;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Diagnostics;
using System . Globalization;
using System . Linq;
using System . Text;
using System . Threading;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Threading;

using NewWpfDev . Converts;
using NewWpfDev . DataTemplates;
using NewWpfDev . UserControls;
using NewWpfDev . Views;

//using static NewWpfDev . Views . Tabview;

namespace NewWpfDev . ViewModels {
    /// <summary>
    ///  Thiis  file is used to control  a large part of the functionality of the entire Tabview System.
    ///  it handles switching between  the 5 tabs tht compprises the entire Tabcontrol system, and each
    ///  tabs Content is in fact an individual  UserControl , which caused me to have to create a Control STRUCT
    ///  access  using "Tabview . Tanbcntrl . xxxxxxxxxxxxxxx....." to allow access to the relevant DataContexts used 
    ///  by  the  UserControls due to implleentin a "sort" of MVMM control system for this  system
    /// 
    /// </summary>
    public class TabWinViewModel : INotifyPropertyChanged, ITabViewer {

        dynamic Ctrlptr = Tabview . Tabcntrl . dgUserctrl;

        #region Events
        public static event EventHandler<LoadDbArgs> LoadDb;
        public static event EventHandler<LinkChangedArgs> ViewersLinked;
        public static event EventHandler<DbCountArgs> SetDBCount;
        public static event EventHandler<DbTypeArgs> SetDBType;

        public static void OnSetDbType ( ) {
            DbTypeArgs args = new ( );
            if ( SetDBType != null )
                SetDBType?.Invoke ( null , args );
        }
        protected virtual void OnViewersLinked ( int index , object item , UIElement ctrl ) {
            LinkChangedArgs args = new LinkChangedArgs ( );
            if ( ViewersLinked != null )
                ViewersLinked . Invoke ( this , args );
        }
        protected virtual void OnLoadDb ( string name ) {
            LoadDbArgs args = new LoadDbArgs ( );
            args . dbname = name;

            if ( LoadDb != null )
                LoadDb . Invoke ( this , args );
        }
        private void Tabview_SetDBType ( object sender , DbTypeArgs e ) {
            //            Debug. WriteLine ( $"{e . Dbname}" );
            DbType = e . Dbname;
        }
        public static void TriggerDbType ( string dbname ) {
            if ( SetDBType != null ) {
                DbTypeArgs args = new DbTypeArgs ( );
                args . Dbname = dbname;
                SetDBType . Invoke ( null , args );
            }
        }

        #endregion Events

        #region NotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged ( string propertyName ) {
            if ( PropertyChanged != null ) {
                PropertyChanged ( this , new PropertyChangedEventArgs ( propertyName ) );
            }
        }
        #endregion NotifyPropertyChanged

        //        public static TabController Tabctrl;
        public Button button = new Button ( );
        private static Stopwatch timer = new Stopwatch ( );
        private bool SelectionInAction { get; set; } = false;
        static public IProgress<int> progress {
            get; set;
        }

        public bool USETASK { get; set; } = true;


        public LbUserControl lbuser;
        public LvUserControl lvuser;
        public TvUserControl tvuser;
        public struct TabData {
            public LbUserControl lbctrl;
            public LvUserControl lvctrl;
            public TvUserControl tvctrl;
            public LogUserControl logctrl;
        }
        public Stopwatch watch = new Stopwatch ( );
        public object Viewmodel {
            get; set;
        }

        #region Properties

        public static TabWinViewModel ControllerVm {
            get; set;
        }
        public static TabControl Tabcontrol {
            get; set;
        }
        public static Tabview Tview {
            get; set;
        }

        public static string CurrentTabTextBlock {
            get; set;
        }
        public static string CurrentTabName {
            get; set;
        }
        public static int CurrentTabIndex {
            get; set;
        }
        //public static DgUserControl Tabview . Tabcntrl. dgUserctrl {
        //    get; set;
        //}
        //public static LbUserControl Tabview . Tabcntrl. lbUserctrl {
        //    get; set;
        //}
        //public static LvUserControl Tabview . Tabcntrl. lvUserctrl {
        //    get; set;
        //}
        public static LogUserControl logUserctrl {
            get; set;
        }
        public static TvUserControl tvUserctrl {
            get; set;
        }
        public static Window Win {
            get; set;
        }
        private static TabWinViewModel ThisWin {
            get; set;
        }
        //private static object TabContentObject {
        //    get; set;
        //}
        public static bool IsLoadingDb { get; set; } = true;

        #endregion Poperties

        #region Full properties

        // Backing variables
        private DataGrid dgrid;
        private string dbCount;
        private int progressValue;
        private TabItem tabitem;
        private string dbType;

        // Full properties for above....
        public TabItem Tabitem {
            get { return tabitem; }
            set { tabitem = value; NotifyPropertyChanged ( nameof ( Tabitem ) ); }
        }
        public DataGrid dGrid {
            get {
                return dgrid;
            }
            set {
                dgrid = value; NotifyPropertyChanged ( nameof ( dGrid ) );
            }
        }
        public string DbCount {
            get {
                return dbCount;
            }
            set {
                dbCount = value; ; NotifyPropertyChanged ( nameof ( DbCount ) );
            }
        }
        public string DbType {
            get { return dbType; }
            set { dbType = value; NotifyPropertyChanged ( nameof ( DbType ) ); }
        }
        public int ProgressValue {
            get {
                return progressValue;
            }
            set {
                progressValue = value; NotifyPropertyChanged ( nameof ( ProgressValue ) );
            }
        }
        #endregion Full properties


        private bool canExpand;
        public bool CanExpand {
            get { return canExpand; }
            set {
                canExpand = value; NotifyPropertyChanged ( nameof ( CanExpand ) );
            }
        }


        #region Commands
        public ICommand LoadBankBtn {
            get;
        }
        public ICommand LoadCustBtn {
            get;
        }
        public ICommand LoadGenBtn {
            get;
        }
        public ICommand CloseAppBtn {
            get;
        }
        public ICommand CloseWinBtn {
            get;
        }
        public ICommand ShowInfo {
            get;
        }
        public ObservableCollection<BankAccountViewModel> Bvm { get; set; }
        public ObservableCollection<CustomerViewModel> Cvm { get; set; }
        #endregion Commands

        #region Interface items
        public void TabLoadBank ( object HostControl , string DbType , bool Notify ) {
            //            CurrentType = DbType;
            TabWinViewModel . TriggerDbType ( DbType );
            if ( HostControl . GetType ( ) == typeof ( DgUserControl ) ) {
                Tabview . Tabcntrl . ActiveControlType = Tabview . Tabcntrl . dgUserctrl;
                Tabview . Tabcntrl . DtTemplates . TemplateNameDg = "BANKACCOUNT";
                Tabview . Tabcntrl . CurrentTypeDg = "BANKACCOUNT";
            }
            if ( HostControl . GetType ( ) == typeof ( LbUserControl ) ) {
                Tabview . Tabcntrl . ActiveControlType = Tabview . Tabcntrl . lbUserctrl;
                Tabview . Tabcntrl . DtTemplates . TemplateNameLb = "BANKACCOUNT";
                Tabview . Tabcntrl . CurrentTypeLb = "BANKACCOUNT";
            }
            if ( HostControl . GetType ( ) == typeof ( LvUserControl ) ) {
                Tabview . Tabcntrl . ActiveControlType = Tabview . Tabcntrl . lvUserctrl;
                Tabview . Tabcntrl . DtTemplates . TemplateNameLv = "BANKACCOUNT";
                Tabview . Tabcntrl . CurrentTypeLv = "BANKACCOUNT";
            }
            Tabview . SetDbType ( "BANK" );
            // now load the data
            Task . Run ( async ( ) => {
                Application . Current . Dispatcher . Invoke ( ( Action ) ( async ( ) => {
                    // does NOT use BankDataLoaded messaging
                    await LoadBankDb ( HostControl , Notify );
                } ) );
            } );
            Mouse . OverrideCursor = Cursors . Arrow;
            return;
        }
        DgUserControl dgUserctrl;
        LbUserControl lbUserctrl;
        LvUserControl lvUserctrl;
        private async Task LoadBankDb ( object HostControl , bool Notify ) {
            Bvm = new ObservableCollection<BankAccountViewModel> ( );
            if ( HostControl . GetType ( ) == typeof ( DgUserControl ) ) {
                dgUserctrl = ( DgUserControl ) HostControl;
                dgUserctrl . grid1 . ItemsSource = null;
                dgUserctrl . grid1 . Items . Clear ( );
                await Task . Run ( ( ) => {
                    Application . Current . Dispatcher . Invoke ( ( Action ) ( ( ) => {
                        Bvm = UserControlDataAccess . GetBankObsCollection ( true , "dgUserControl" );
                    } ) );
                } );
            }
            else if ( HostControl . GetType ( ) == typeof ( LbUserControl ) ) {
                lbUserctrl = ( LbUserControl ) HostControl;
                lbUserctrl . listbox1 . ItemsSource = null;
                lbUserctrl . listbox1 . Items . Clear ( );
                await Task . Run ( ( ) => {
                    Application . Current . Dispatcher . Invoke ( ( Action ) ( ( ) => {
                        Bvm = UserControlDataAccess . GetBankObsCollection ( true , "lbUserControl" );
                    } ) );
                } );
            }
            else if ( HostControl . GetType ( ) == typeof ( LvUserControl ) ) {
                lvUserctrl = ( LvUserControl ) HostControl;
                lvUserctrl . listview1 . ItemsSource = null;
                lvUserctrl . listview1 . Items . Clear ( );
                await Task . Run ( ( ) => {
                    Application . Current . Dispatcher . Invoke ( ( Action ) ( ( ) => {
                        Bvm = UserControlDataAccess . GetBankObsCollection ( true , "lvUserControl" );
                    } ) );
                } );
            }
            return;
        }

        #endregion Interface items

        public TabWinViewModel ( ) {
            if ( ThisWin == null ) {
                ThisWin = this;
                LoadBankBtn = new RelayCommand ( LoadBankBtnExecute , LoadBankBtnCanExecute );
                LoadCustBtn = new RelayCommand ( LoadCustBtnExecute , LoadCustBtnCanExecute );
                LoadGenBtn = new RelayCommand ( LoadGenBtnExecute , LoadGenBtnCanExecute );
                CloseAppBtn = new RelayCommand ( CloseAppExecute , CloseAppCanExecute );
                CloseWinBtn = new RelayCommand ( CloseWinExecute , CloseWinCanExecute );
                ShowInfo = new RelayCommand ( ShowinfoExecute , ShowinfoCanExecute );
                // Give a pointer for 'this' to our usercontrols and get theirs back
                SetDBCount += Tabview_SetDBCount;
                SetDBType += Tabview_SetDBType;

                // get our control structure
                //Tabcntrl= Tabview . Tabctrl;
                // Add this class to control struct
                Tabview . Tabcntrl . twVModel = this;
                // set local public static pointer  to Tabview
                //Tview = Tabview . GetTabview ( );

                if ( Tabview . Tabcntrl . dgUserctrl == null ) {
                    Tabview . Tabcntrl . dgUserctrl = new DgUserControl ( );
                }
                if ( Tabview . Tabcntrl . lbUserctrl == null ) {
                    Tabview . Tabcntrl . lbUserctrl = new LbUserControl ( );
                }
                if ( Tabview . Tabcntrl . lvUserctrl == null ) {
                    Tabview . Tabcntrl . lvUserctrl = new LvUserControl ( );
                }
                if ( tvUserctrl == null )
                    tvUserctrl = new TvUserControl ( );
                if ( logUserctrl == null )
                    logUserctrl = new LogUserControl ( );

                CanExpand = true;

                //Tabview . Tabcntrl. dgUserctrl = DgUserControl . SetController ( this );
                //Tabview . Tabcntrl. lbUserctrl = LbUserControl . SetController ( this );
                //Tabview . Tabcntrl. lvUserctrl = LvUserControl . SetController ( this );
                //tvUserctrl = TvUserControl . SetController ( this );
                //logUserctrl = LogUserControl . SetController ( this );

                // Save to our ViewModel repository
                //Viewmodel = new ViewModel ( );
                //Viewmodel = this;
                //ViewModel . SaveViewmodel ( "TabWinViewModel" , Viewmodel );
                progress = new Progress<int> ( count => ProgressValue = 0 );
                //Debug . WriteLine ( $"ctrls = {Tabview . Tabcntrl . lbUserctrl?.listbox1?.Items . Count} ,  {Tabview . Tabcntrl . lvUserctrl?.listview1?.Items . Count}" );
            }
        }

        private void LoadGenBtnExecute ( object obj ) {
            string target = Tabview . tabvw . DbnamesCb . SelectedItem . ToString ( );
            if ( target == "" ) target = "invoice";
            if ( Tabview . Tabcntrl . ActiveControlType . GetType ( ) == typeof ( DgUserControl ) )
                Tabview . Tabcntrl . dgUserctrl . LoadGeneric ( target );
            if ( Tabview . Tabcntrl . ActiveControlType . GetType ( ) == typeof ( LbUserControl ) )
                Tabview . Tabcntrl . lbUserctrl . LoadGeneric ( target );
            else if ( Tabview . Tabcntrl . ActiveControlType . GetType ( ) == typeof ( LvUserControl ) )
                Tabview . Tabcntrl . lvUserctrl . LoadGeneric ( target );
        }

        private bool LoadGenBtnCanExecute ( object arg ) {
            if ( Tabview . Tabcntrl . ActiveControlType == Tabview . Tabcntrl . dgUserctrl ||
                Tabview . Tabcntrl . ActiveControlType == Tabview . Tabcntrl . lbUserctrl ||
                Tabview . Tabcntrl . ActiveControlType == Tabview . Tabcntrl . lvUserctrl )
                return true;
            else return false;
        }
        private void SetFocusToGrid ( object sender , EventArgs e ) {
            //DgUserControl . OnGridGotFocus ( );
        }

        private bool ShowinfoCanExecute ( object arg ) {
            return true;
        }

        private void ShowinfoExecute ( object obj ) {
            try {
                TabViewInfo tvvi = new TabViewInfo ( "TabbedDataInfo.Txt" );
                tvvi . Show ( );
            }
            catch ( Exception ex ) { Debug . WriteLine ( $"Error loading Text file from disk : {ex . Message}" ); }
        }

        public void Tabview_SetDBCount ( object sender , DbCountArgs e ) {
            DbCount = e . Dbcount . ToString ( );

        }
        public static void TriggerBankDbCount ( object obj , DbCountArgs e ) {
            if ( SetDBCount != null )
                SetDBCount?.Invoke ( obj , e );
        }

        #region Button Handlers   for loading data
        public async void LoadCustBtnExecute ( object obj ) {
            // Generic Load of Customer data for any control type
            // called by a Command for a  button on top row
            if ( CurrentTabName == "DgridTab" ) {
                Tabview . Tabcntrl . dgUserctrl . Cvm?.Clear ( );
                Tabview . Tabcntrl . dgUserctrl . CurrentType = "CUSTOMER";
                await Task . Run ( ( ) => Tabview . Tabcntrl . dgUserctrl . LoadCustomer ( ) );
                Tabview . Tabcntrl . dgUserctrl . grid1 . ItemsSource = Tabview . Tabcntrl . dgUserctrl . Cvm;
                Tabview . Tabcntrl . dgUserctrl . grid1 . CellStyle = Application . Current . FindResource ( "MAINCustomerGridStyle" ) as Style;
                DataTemplate dt = Application . Current . FindResource ( "CustomersDbTemplate1" ) as DataTemplate;
                Tabview . Tabcntrl . dgUserctrl . grid1 . ItemTemplate = dt;
                IsLoadingDb = true;
                Tabview . Tabcntrl . dgUserctrl . grid1 . SelectedIndex = 0;
                Tabview . Tabcntrl . dgUserctrl . grid1 . SelectedItem = 0;
                IsLoadingDb = false;
            }
            else if ( CurrentTabName == "ListboxTab" ) {
                Tabview . Tabcntrl . lbUserctrl . Cvm?.Clear ( );
                Tabview . Tabcntrl . lbUserctrl . CurrentType = "CUSTOMER";
                Tabview . Tabcntrl . lbUserctrl . LoadCustomer ( true );
                //await Task . Run ( ( ) => Tabview . Tabcntrl . lbUserctrl . LoadCustomer ( true ) );
                Tabview . Tabcntrl . lbUserctrl . listbox1 . ItemsSource = Tabview . Tabcntrl . lbUserctrl . Cvm;
                DataTemplate dt = Application . Current . FindResource ( "CustomersDbTemplate1" ) as DataTemplate;
                Tabview . Tabcntrl . lbUserctrl . listbox1 . ItemTemplate = dt;
                IsLoadingDb = true;
                Tabview . Tabcntrl . lbUserctrl . listbox1 . SelectedIndex = 0;
                Tabview . Tabcntrl . lbUserctrl . listbox1 . SelectedItem = 0;
                IsLoadingDb = false;
            }
            else if ( CurrentTabName == "ListviewTab" ) {
                Tabview . Tabcntrl . lvUserctrl . Cvm?.Clear ( );
                Tabview . Tabcntrl . lvUserctrl . CurrentType = "CUSTOMER";
                //await Task . Run ( async ( ) => {
                //Application . Current . Dispatcher . Invoke (() =>
                Tabview . Tabcntrl . lvUserctrl . LoadCustomer ( );


                //        await Task . Run ( ( ) => Tabview . Tabcntrl . lvUserctrl . LoadCustomer ( ) );
                Tabview . Tabcntrl . lvUserctrl . listview1 . ItemsSource = Tabview . Tabcntrl . lvUserctrl . Cvm;
                DataTemplate dt = Application . Current . FindResource ( "CustomersDbTemplate1" ) as DataTemplate;
                Tabview . Tabcntrl . lvUserctrl . listview1 . ItemTemplate = dt;
                IsLoadingDb = true;
                Tabview . Tabcntrl . lvUserctrl . listview1 . SelectedIndex = 0;
                Tabview . Tabcntrl . lvUserctrl . listview1 . SelectedItem = 0;
                IsLoadingDb = false;
            }
            DbType = "CUSTOMER";
        }
        public async void LoadBankBtnExecute ( object obj ) {
            // Generic Load of Bank data for any control type
            if ( CurrentTabName == "DgridTab" ) {
                Tabview . Tabcntrl . dgUserctrl . Bvm?.Clear ( );
                Tabview . Tabcntrl . dgUserctrl . CurrentType = "BANK";
                Application . Current . Dispatcher . Invoke ( async ( ) =>
                    await Task . Run ( ( ) => Tabview . Tabcntrl . dgUserctrl . LoadBank ( ) )
                 );

            }
            else if ( CurrentTabName == "ListboxTab" ) {
                Tabview . Tabcntrl . lbUserctrl . Bvm?.Clear ( );
                Tabview . Tabcntrl . lbUserctrl . CurrentType = "BANK";
                Tabview . Tabcntrl . lbUserctrl . LoadBank ( true );
                //              await Task . Run ( ( ) => Tabview . Tabcntrl . lbUserctrl . LoadBank ( true ) );
            }
            else if ( CurrentTabName == "ListviewTab" ) {
                Tabview . Tabcntrl . lvUserctrl . Bvm?.Clear ( );
                Tabview . Tabcntrl . lvUserctrl . CurrentType = "BANK";
                await Task . Run ( ( ) => Tabview . Tabcntrl . lvUserctrl . LoadBank ( ) );
            }
            DbType = "BANK";
        }
        #endregion Button Handlers   for loading data

        #region CanExecute handlers
        private bool CloseAppCanExecute ( object arg ) {
            return true;
        }
        private bool CloseWinCanExecute ( object arg ) {
            return true;
        }
        private bool LoadCustBtnCanExecute ( object arg ) {
            if ( Tabview . Tabcntrl . ActiveControlType == Tabview . Tabcntrl . dgUserctrl ||
                Tabview . Tabcntrl . ActiveControlType == Tabview . Tabcntrl . lbUserctrl ||
                Tabview . Tabcntrl . ActiveControlType == Tabview . Tabcntrl . lvUserctrl )
                return true;
            else return false;
        }
        private bool LoadBankBtnCanExecute ( object arg ) {
            if ( Tabview . Tabcntrl . ActiveControlType == Tabview . Tabcntrl . dgUserctrl ||
                Tabview . Tabcntrl . ActiveControlType == Tabview . Tabcntrl . lbUserctrl ||
                Tabview . Tabcntrl . ActiveControlType == Tabview . Tabcntrl . lvUserctrl )
                return true;
            else return false;
        }

        #endregion CanExecute

        public static TabWinViewModel SetPointer ( Tabview tview , string tabname ) {
            // Get pointer to our view
            Tview = tview;
            Tabcontrol = Tview?.Tabctrl;
            //Tabview . Tabcntrl. dgUserctrl = DgUserControl . SetController ( ThisWin );
            //Tabview . Tabcntrl. lbUserctrl = LbUserControl . SetController ( ThisWin );
            //Tabview . Tabcntrl . lvUserctrl = LvUserControl . SetController ( ThisWin );
            tvUserctrl = TvUserControl . SetController ( ThisWin );
            CurrentTabIndex = 0;
            return ThisWin;
        }
        public static Tabview SendTabview ( ) {
            if ( Tview != null )
                return Tview;
            return null;
        }

       
        public async Task SetCurrentTab ( Tabview tabview , string tab ) {
            // Working well 2/6/22
            ProgressValue = 0;
            Tview . ProgressBar_Progress . UpdateLayout ( );
            CurrentTabName = tab;

            if ( tab == "DgridTab" ) {
                // ************//
                // DATAGRID
                // ************//
                //Tabview . Tabcntrl . twVModel . CanExpand = true;
                // Test of Dynamic value
                ProgressValue = 15;
                Tview . ProgressBar_Progress . UpdateLayout ( );
                Tview . LoadName . Text = "Loading Data Grid ...";
                if ( IsLoadingDb == true && CurrentTabName != tab ) return;
                IsLoadingDb = true;
                // setup the current tab Id
                Tabcontrol . SelectedIndex = 0;
                CurrentTabIndex = Tabcontrol . SelectedIndex;
                Debug . WriteLine ( $"Setting DataGrid as Active tab" );
                SendWindowMessage ( $"Setting Datagrid as Active tab" );
                CurrentTabTextBlock = "Tab1Header";
                Tabview . Tabcntrl . tabView . Btn1 . IsEnabled = true;
                Tabview . Tabcntrl . tabView . Btn2 . IsEnabled = true;
                // Setup control struct
                Tabview . Tabcntrl . tabItem = Tabcontrol . Items [ 0 ] as TabItem;
                Tabview . Tabcntrl . tabItem . Content = Tabview . Tabcntrl . dgUserctrl;
                //Set currently active tab to Datagrid
                Tabview . Tabcntrl . ActiveControlType = Tabview . Tabcntrl . dgUserctrl;
                Tabitem = Tabcontrol . Items [ 0 ] as TabItem;

                // Setup Templates to match this tab
                CheckActiveTemplate ( Tabview . Tabcntrl . dgUserctrl );

                ProgressValue = 25;
                Tview . ProgressBar_Progress . UpdateLayout ( );

                if ( Tabview . Tabcntrl . dgUserctrl == null )
                    Tabview . Tabcntrl . dgUserctrl = new DgUserControl ( );

                if ( Tabview . Tabcntrl . dgUserctrl . grid1 . Items . Count == 0 ) {
                    if ( USETASK ) {
                        LoadDataGridAsync ( Tabview . Tabcntrl . dgUserctrl );
                    }
                    else {
                        Tabview . Tabcntrl . dgUserctrl = LoadDatagridInBackgroundTask ( Tabview . Tabcntrl . dgUserctrl );
                    }
                    Tabview . tabvw . DbnamesCb . SelectedIndex = Tabview . Tabcntrl . tabView . BankIndex;
                    Tabview . Tabcntrl . DbNameIndexDg = Tabview . Tabcntrl . tabView . BankIndex;
                    //Tabview . Tabcntrl . DbNameDg = Tabview . Tabcntrl . tabView . DbnamesCb . SelectedItem . ToString ( ) . ToUpper ( );
                    //// Setup Templates to match this tab
                    //CheckActiveTemplate ( Tabview . Tabcntrl . dgUserctrl );
                }
                ProgressValue = 100;
                Tview . ProgressBar_Progress . UpdateLayout ( );
                // clear all Tags to false;
                ClearTags ( );
                Tabview . Tabcntrl . dgUserctrl . Tag = true;
                //Always do this close to end of method
                Tabview . Tabcntrl . tabItem . Content = Tabview . Tabcntrl . dgUserctrl;

                // update Db Count for field on Tabview
                GotFocusArgs args = new GotFocusArgs ( );
                args . UseTask = USETASK;
                args . sender = this;
                args . caller = "TabWinViewModel";
                //allow other tabs  to load again
                IsLoadingDb = false;
                DbType = Tabview . Tabcntrl . CurrentTypeDg;

                //Tidy up screen
                // Set Correct Combobox selections as we have switched tabs
                Tabview . tabvw . DbnamesCb . SelectedIndex = Tabview . Tabcntrl . DbNameIndexDg;
                Tabview . tabvw . TemplatesCb . SelectedIndex = Tabview . Tabcntrl . DtTemplates . TemplateIndexDg;

                if ( Tabview . Tabcntrl . DtTemplates . TemplateNameDg == "BANKACCOUNT"
                    || Tabview . Tabcntrl . DtTemplates . TemplateNameDg == "CUSTOMER" )
                    SetDefaultComboColors ( true );
                else
                    SetDefaultComboColors ( false );

                Tabview . tabvw . DbnamesCb . IsEnabled = true;
                Tabview . tabvw . TemplatesCb . IsEnabled = true;
                Tabview . tabvw . DbnamesCb . UpdateLayout ( );
                Tabview . Tabcntrl . dgUserctrl . grid1 . CommitEdit ( 0 , false );
                Tview . LoadName . Text = "";
                ProgressValue = 0;
                Utils . ScrollRecordIntoView ( Tabview . Tabcntrl . dgUserctrl . grid1 , Tabview . Tabcntrl . dgUserctrl . grid1 . SelectedIndex );
                Tview . ProgressBar_Progress . UpdateLayout ( );
                // This NOW scrolls into view correctly !!!!!
                Utils . ScrollRecordIntoView ( Tabview . Tabcntrl . dgUserctrl . grid1 , Tabview . Tabcntrl . dgUserctrl . grid1 . SelectedIndex , Tabview . Tabcntrl . dgUserctrl . grid1 . SelectedItem );
                Tabview . Tabcntrl . dgUserctrl . grid1 . ScrollIntoView ( Tabview . Tabcntrl . dgUserctrl . grid1 . SelectedItem );
                Ctrlptr = Tabview . Tabcntrl . dgUserctrl;
                string [ ] ctrl = Utils . GetDynamicVarType ( Ctrlptr );
                Debug . WriteLine ( $"Dynamic type is Datagrid  : \n\"{ctrl [ 0 ]}\"\n\"{ctrl [ 1 ]}\"\n\"{ctrl [ 2 ]}\"" );
            }
            else if ( tab == "ListboxTab" ) {
                // ************//
                // LISTBOX
                // ************//
                if ( IsLoadingDb == true && Tabview . Tabcntrl . lbUserctrl != null ) return;
                Mouse . OverrideCursor = Cursors . Wait;
                //CurrentTabName = tab;
                Tview . LoadName . Text = "List Box Loading";
                IsLoadingDb = true;
                Tabcontrol . SelectedIndex = 1;
                CurrentTabIndex = Tabcontrol . SelectedIndex;
                Debug . WriteLine ( $"Setting Listbox as Active tab" );
                Tabview . Tabcntrl . tabView . Btn1 . IsEnabled = true;
                Tabview . Tabcntrl . tabView . Btn2 . IsEnabled = true;
                //Debug . WriteLine ( $"Setting Listbox as Active tab" );
                SendWindowMessage ( $"Setting Listbox as Active tab" );
                Tabview . Tabcntrl . tabItem = Tabcontrol . Items [ 1 ] as TabItem;
                Tabview . Tabcntrl . tabItem . Content = Tabview . Tabcntrl . lbUserctrl;
                Tabview . Tabcntrl . ActiveControlType = Tabview . Tabcntrl . lbUserctrl;
                Tabitem = Tabcontrol . Items [ 1 ] as TabItem;
                //                Tabview . Tabcntrl . CurrentTypeLb = Tabview . Tabcntrl . CurrentTypeLb == null ? "BANK" : Tabview . Tabcntrl . CurrentTypeLb;

                // Setup Templates to match this tab
                CheckActiveTemplate ( Tabview . Tabcntrl . lbUserctrl );

                ProgressValue = 5;
                Tview . ProgressBar_Progress . UpdateLayout ( );
                CurrentTabTextBlock = "Tab2Header";
                CurrentTabName = tab;
                Tabview . Tabcntrl . CurrentTabName = tab;
                LbUserControl . lbParent = "LBVIEW";            // ??????

                if ( Tabview . Tabcntrl . lbUserctrl . listbox1 . Items . Count == 0 ) {
                    if ( USETASK ) {
                        watch . Reset ( );
                        watch . Start ( );
                        ProgressValue = 15;
                        Tview . ProgressBar_Progress . UpdateLayout ( );
                        if ( Tabview . Tabcntrl . lbUserctrl == null ) Tabview . Tabcntrl . lbUserctrl = new LbUserControl ( );

                        Task task = new Task ( async ( ) => {
                            Application . Current . Dispatcher . Invoke ( DispatcherPriority . Normal , ( Action ) ( async ( ) => {
                                await LoadListBoxAsync ( );
                            } ) );
                        } );
                        task . Start ( );

                        ProgressValue = 55;
                        Tview . ProgressBar_Progress . UpdateLayout ( );
                        //Tabitem . IsSelected = true;
                        Tabview . Tabcntrl . tabItem . IsSelected = true;
                        ProgressValue = 85;
                        Tview . ProgressBar_Progress . UpdateLayout ( );
                        watch . Stop ( );
                        watch . Reset ( );
                        Tabview . Tabcntrl . lbUserctrl . listbox1 . Focus ( );
                        DbType = Tabview . Tabcntrl . lbUserctrl?.CurrentType;
                        Tabview . Tabcntrl . lbUserctrl . UpdateLayout ( );
                        ProgressValue = 0;
                        Tview . ProgressBar_Progress . UpdateLayout ( );
                        Mouse . OverrideCursor = Cursors . Arrow;
                        ProgressValue = 90;
                        Tview . ProgressBar_Progress . UpdateLayout ( );
                    }
                    else {
                        watch . Start ( );
                        Tabview . Tabcntrl . lbUserctrl = LoadListboxInBackgroundTask ( Tabview . Tabcntrl . lbUserctrl );
                        watch . Stop ( );
                        watch . Reset ( );
                    }
                    Tabview . tabvw . DbnamesCb . SelectedIndex = Tabview . Tabcntrl . tabView . BankIndex;
                    Tabview . Tabcntrl . DbNameIndexLb = Tabview . Tabcntrl . tabView . BankIndex;
                    Tabview . Tabcntrl . DbNameLb = Tabview . Tabcntrl . tabView . DbnamesCb . SelectedItem . ToString ( ) . ToUpper ( );
                }
                else {
                    Tabview . Tabcntrl . tabItem . IsSelected = true;
                    // Debug . WriteLine ( "Already loaded...." );
                    Mouse . OverrideCursor = Cursors . Arrow;
                }
                Tabview . Tabcntrl . tabItem . Content = Tabview . Tabcntrl . lbUserctrl;
                //Tabitem . Content = Tabview . Tabcntrl. lbUserctrl;

                // Set Correct Combobox selections as we have switched tabs
                Tabview . tabvw . DbnamesCb . SelectedIndex = Tabview . Tabcntrl . DbNameIndexLb;
                Tabview . tabvw . TemplatesCb . SelectedIndex = Tabview . Tabcntrl . DtTemplates . TemplateIndexLb;

                if ( Tabview . Tabcntrl . DtTemplates . TemplateNameLb == "BANKACCOUNT"
                    || Tabview . Tabcntrl . DtTemplates . TemplateNameLb == "CUSTOMER" )
                    SetDefaultComboColors ( true );
                else
                    SetDefaultComboColors ( false );

                Tabview . tabvw . DbnamesCb . IsEnabled = true;
                Tabview . tabvw . TemplatesCb . IsEnabled = true;
                Tabview . tabvw . DbnamesCb . UpdateLayout ( );

                DbType = Tabview . Tabcntrl . CurrentTypeLb;
                CurrentTabIndex = Tabcontrol . SelectedIndex;
                Tabview . Tabcntrl . lbUserctrl?.UpdateLayout ( );
                //Debug . WriteLine ( "Switched to  listbox - yeahhhhhhh........" );
                ClearTags ( );
                Tabview . Tabcntrl . lbUserctrl . Tag = true;
                Tabview . Tabcntrl . lbUserctrl . listbox1 . Focus ( );
                DbCountArgs cargs = new DbCountArgs ( );
                cargs . Dbcount = Tabview . Tabcntrl . lbUserctrl . listbox1 . Items . Count;
                cargs . sender = "Tabview . Tabcntrl. lbUserctrl";
                IsLoadingDb = false;
                TabWinViewModel . TriggerBankDbCount ( this , cargs );
                Tview . LoadName . Text = "";
                ProgressValue = 0;
                Tview . ProgressBar_Progress . UpdateLayout ( );
                Utils . ScrollLBRecordIntoView ( Tabview . Tabcntrl . lbUserctrl . listbox1 , Tabview . Tabcntrl . lbUserctrl . listbox1 . SelectedIndex );
                Ctrlptr = Tabview . Tabcntrl . lbUserctrl;
                string [ ] ctrl = Utils . GetDynamicVarType ( Ctrlptr );
                Debug . WriteLine ( $"Dynamic type is ListBox : \n\"{ctrl [ 0 ]}\"\n\"{ctrl [ 1 ]}\"\n\"{ctrl [ 2 ]}\"" );
            }
            else if ( tab == "ListviewTab" ) {
                // ************//
                // LISTVIEW
                // ************//
                if ( IsLoadingDb == true && Tabview . Tabcntrl . lvUserctrl != null ) return;

                IsLoadingDb = true;
                Debug . WriteLine ( $"Setting Listview as Active tab" );
                Tabcontrol . SelectedIndex = 2;
                CurrentTabIndex = Tabcontrol . SelectedIndex;
                CurrentTabTextBlock = "Tab3Header";
                Tview . LoadName . Text = "List View Loading";

                //Set currently active tab to ListView
                Tabview . Tabcntrl . ActiveControlType = Tabview . Tabcntrl . lvUserctrl;

                Tabview . Tabcntrl . tabItem = Tabcontrol . Items [ 2 ] as TabItem;

                CheckActiveTemplate ( Tabview . Tabcntrl . lvUserctrl );

                Tabview . Tabcntrl . tabView . Btn1 . IsEnabled = true;
                Tabview . Tabcntrl . tabView . Btn2 . IsEnabled = true;

                if ( Tabview . Tabcntrl . lvUserctrl == null ) {
                    Tabview . Tabcntrl . lvUserctrl = new LvUserControl ( );
                }
                Tabview . Tabcntrl . CurrentTypeLv = Tabview . Tabcntrl . CurrentTypeLv == null ? "BANKACCOUNT" : Tabview . Tabcntrl . CurrentTypeLv;

                Tabitem . Content = Tabview . Tabcntrl . lvUserctrl;

                Tabview . Tabcntrl . tabItem = Tabcontrol . Items [ 2 ] as TabItem;
                //Tabitem = Tabcontrol . Items [ 2 ] as TabItem;

                LvUserControl . SetSelectionInAction ( Tabview . GetTabview ( ) . ViewersLinked );

                if ( Tabview . Tabcntrl . lvUserctrl . listview1 . Items . Count == 0 ) {
                    // Need to load data here
                    ProgressValue = 15;
                    Tview . ProgressBar_Progress . UpdateLayout ( );

                    if ( USETASK ) {
                        await Task . Run ( async ( ) => {
                            await LoadListViewAsync ( Tabview . Tabcntrl . lvUserctrl );
                        } );
                        // we do NOT have any data here as task is still running
                        Tabitem . Content = Tabview . Tabcntrl . lvUserctrl;
                        Tabitem . IsSelected = true;

                        if ( Tabview . Tabcntrl . lvUserctrl != null )
                            Tabview . Tabcntrl . lvUserctrl?.UpdateLayout ( );
                        Tabview . Tabcntrl . lvUserctrl . listview1 . Width = Tabview . Tabcntrl . lvUserctrl . listview1 . Width + 1;
                        Tabview . Tabcntrl . lvUserctrl . listview1 . Refresh ( );
                    }
                    else {
                        ProgressValue = 15;
                        Tview . ProgressBar_Progress . UpdateLayout ( );
                        Tabview . Tabcntrl . lvUserctrl = LoadListviewInBackgroundTask ( Tabview . Tabcntrl . lvUserctrl );
                    }
                    Tabview . tabvw . DbnamesCb . SelectedIndex = Tabview . Tabcntrl . tabView . BankIndex;
                    Tabview . Tabcntrl . DbNameIndexLv = Tabview . Tabcntrl . tabView . BankIndex;
                }

                IsLoadingDb = false;
                Tabview . Tabcntrl . lvUserctrl . listview1 . Focus ( );
                if ( Tabview . Tabcntrl . lvUserctrl != null ) {
                    DbCountArgs cargs = new DbCountArgs ( );
                    cargs . Dbcount = Tabview . Tabcntrl . lvUserctrl . listview1 . Items . Count;
                    cargs . sender = "Tabview . Tabcntrl. lvUserctrl";
                    TabWinViewModel . TriggerBankDbCount ( this , cargs );
                }

                // Set Correct Combobox selections as we have switched tabs
                Tabview . tabvw . DbnamesCb . SelectedIndex = Tabview . Tabcntrl . DbNameIndexLv;
                Tabview . tabvw . TemplatesCb . SelectedIndex = Tabview . Tabcntrl . DtTemplates . TemplateIndexLv;

                if ( Tabview . Tabcntrl . DtTemplates . TemplateNameLv == "BANKACCOUNT"
                        || Tabview . Tabcntrl . DtTemplates . TemplateNameLv == "CUSTOMER" )
                    SetDefaultComboColors ( true );
                else
                    SetDefaultComboColors ( false );

                Tabview . tabvw . DbnamesCb . IsEnabled = true;
                Tabview . tabvw . TemplatesCb . IsEnabled = true;

                Tabview . tabvw . DbnamesCb . UpdateLayout ( );

                Tview . LoadName . Text = "";
                ProgressValue = 0;
                Tview . ProgressBar_Progress . UpdateLayout ( );
                DbType = Tabview . Tabcntrl . CurrentTypeLv;

                Ctrlptr = Tabview . Tabcntrl . lvUserctrl;
                string [ ] ctrl = Utils . GetDynamicVarType ( Ctrlptr );
                Debug . WriteLine ( $"Dynamic type is ListView : \n\"{ctrl [ 0 ]}\"\n\"{ctrl [ 1 ]}\"\n\"{ctrl [ 2 ]}\"" );
            }
            else if ( tab == "LogviewTab" ) {
                // ************//
                // LOGVIEW
                // ************//
                Tview . LoadName . Text = "";
                if ( IsLoadingDb == true && CurrentTabName != tab ) return;

                Tabview . Tabcntrl . twVModel . CanExpand = true;

                if ( Tabview . Tabcntrl . lgUserctrl == null ) {
                    Tabview . Tabcntrl . lgUserctrl = new LogUserControl ( );
                }

                IsLoadingDb = true;
                Tabview . Tabcntrl . tabView . Btn1 . IsEnabled = false;
                Tabview . Tabcntrl . tabView . Btn2 . IsEnabled = false;
                //Tabview . Tabcntrl . tabView . Btn6 . IsEnabled = false;

                //Set currently active tab to ListView (log)
                Tabview . Tabcntrl . ActiveControlType = Tabview . Tabcntrl . lgUserctrl;

                Tabcontrol . SelectedIndex = 3;
                CurrentTabIndex = Tabcontrol . SelectedIndex;
                Mouse . OverrideCursor = Cursors . Wait;
                Debug . WriteLine ( $"Setting Logview as Active tab" );
                CurrentTabTextBlock = "Tab4Header";
                Tabitem = Tabcontrol . Items [ 3 ] as TabItem;

                if ( logUserctrl == null ) {
                    // get new usercontrol & add to tab content
                    logUserctrl = new LogUserControl ( );
                    Tabitem . Content = logUserctrl;
                    Tabitem . IsSelected = true;
                }
                ClearTags ( );
                logUserctrl . Tag = true;
                Tview . LogviewTab . Content = logUserctrl;
                //EventControl . TriggerWindowMessage ( this , new InterWindowArgs { message = $"LogUserControl now Active..." , listbox = null } );
                ProgressValue = 0;
                Tview . ProgressBar_Progress . UpdateLayout ( );
                Mouse . OverrideCursor = Cursors . Arrow;
                //Debug . WriteLine ( $"Setting Logview as Active tab" );
                IsLoadingDb = false;
                DbType = "N/A";
                Tabview . tabvw . DbTypeFld . Background = Application . Current . FindResource ( "Gray5" ) as SolidColorBrush;
                Tabview . tabvw . DbCount . Background = Application . Current . FindResource ( "Gray5" ) as SolidColorBrush;
                Tabview . tabvw . DbnamesCb . Foreground = Application . Current . FindResource ( "Gray4" ) as SolidColorBrush;
                Tabview . tabvw . TemplatesCb . Foreground = Application . Current . FindResource ( "Gray4" ) as SolidColorBrush;
                Tabview . tabvw . DbnamesCb . IsEnabled = false;
                Tabview . tabvw . TemplatesCb . IsEnabled = false;

                // update Db Count for field on Tabview
                DbCountArgs cargs = new DbCountArgs ( );
                cargs . Dbcount = logUserctrl . logview . Items . Count;
                cargs . sender = "logUserctrl";
                TabWinViewModel . TriggerBankDbCount ( this , cargs );
            }
            else if ( tab == "TreeviewTab" ) {
                // ************//
                // TREEVIEW
                // ************//
                Tview . LoadName . Text = "";

                Tabview . Tabcntrl . twVModel . CanExpand = true;

                if ( IsLoadingDb == true && CurrentTabName != tab ) return;
                IsLoadingDb = true;
                ClearTags ( );
                Tabcontrol . SelectedIndex = 4;
                Tabview . Tabcntrl . tabItem = Tabcontrol . Items [ 4 ] as TabItem;
                //Tabitem . Content = Tabview . Tabcntrl . tvUserctrl;
                CurrentTabIndex = Tabcontrol . SelectedIndex;
                Mouse . OverrideCursor = Cursors . Wait;
                //Debug . WriteLine ( $"Setting Dummy Treeview as Active tab" );
                //ReduceByParamValue rbp = new ReduceByParamValue ( );
                CurrentTabTextBlock = "Tab5Header";
                Tabview . Tabcntrl . tabView . Btn1 . IsEnabled = false;
                Tabview . Tabcntrl . tabView . Btn2 . IsEnabled = false;
                //Tabview . Tabcntrl . tabView . Btn6 . IsEnabled = false;


                if ( Tabview . Tabcntrl . tvUserctrl == null ) {
                    // get new usercontrol & add to tab content
                    Tabview . Tabcntrl . tvUserctrl = new TvUserControl ( );
                }
                Tabview . Tabcntrl . tabItem = Tabcontrol . Items [ 4 ] as TabItem;
                Tabitem . Content = Tabview . Tabcntrl . tvUserctrl;
                Tabitem . IsSelected = true;
                Tabview . Tabcntrl . tabView . TreeviewTab . Content = tvUserctrl;

                //Set currently active tab to TreeView
                Tabview . Tabcntrl . ActiveControlType = Tabview . Tabcntrl . tvUserctrl;

                Tabview . Tabcntrl . tvUserctrl . Tag = true;
                Tabview . ResizeTreeviewTab ( );
                //                EventControl . TriggerWindowMessage ( this , new InterWindowArgs { message = $"tvUserControl now Active..." , listbox = null } );

                // update Db Count for field on Tabview
                DbType = "N/A";
                Tabview . tabvw . DbCount . Background = Application . Current . FindResource ( "Gray5" ) as SolidColorBrush;
                Tabview . tabvw . DbTypeFld . Background = Application . Current . FindResource ( "Gray5" ) as SolidColorBrush;
                Tabview . tabvw . DbnamesCb . Foreground = Application . Current . FindResource ( "Gray4" ) as SolidColorBrush;
                Tabview . tabvw . TemplatesCb . Foreground = Application . Current . FindResource ( "Gray4" ) as SolidColorBrush;
                Tabview . tabvw . DbnamesCb . IsEnabled = false;
                Tabview . tabvw . TemplatesCb . IsEnabled = false;
                DbCountArgs args = new DbCountArgs ( );
                args . Dbcount = Tabview . Tabcntrl . tvUserctrl . treeview1 . Items . Count;
                args . sender = "tvUserctrl";
                TabWinViewModel . TriggerBankDbCount ( this , args );
                ProgressValue = 0;
                Tview . ProgressBar_Progress . UpdateLayout ( );
                Tabview . Tabcntrl . tvUserctrl . treeview1 . Focus ( );
                IsLoadingDb = false;
            }
            Mouse . OverrideCursor = Cursors . Arrow;
        }

        private void SetDefaultComboColors ( bool state ) {
            if ( state ) {
                Tabview . tabvw . DbTypeFld . Background = Application . Current . FindResource ( "Blue5" ) as SolidColorBrush;
                Tabview . tabvw . DbCount . Background = Application . Current . FindResource ( "Blue5" ) as SolidColorBrush;
                Tabview . tabvw . DbnamesCb . Foreground = Application . Current . FindResource ( "Black0" ) as SolidColorBrush;
                Tabview . tabvw . TemplatesCb . Foreground = Application . Current . FindResource ( "Black0" ) as SolidColorBrush;
            }
            else {
                Tabview . tabvw . DbTypeFld . Background = Application . Current . FindResource ( "Red5" ) as SolidColorBrush;
                Tabview . tabvw . DbCount . Background = Application . Current . FindResource ( "Red5" ) as SolidColorBrush;
                Tabview . tabvw . DbnamesCb . Foreground = Application . Current . FindResource ( "Red5" ) as SolidColorBrush;
                Tabview . tabvw . TemplatesCb . Foreground = Application . Current . FindResource ( "Red5" ) as SolidColorBrush;
            }
        }
        public LbUserControl GetCurrentListbox ( LbUserControl lbUserctrl ) {
            //if ( Tabview . Tabcntrl. lbUserctrl == null ) {
            //    Debug . WriteLine ( $"We do NOT HAVE LbUserctrl in ViewModel Template selection" );
            //    if ( Tabview . Tabcntrl. lbUserctrl == null ) {
            //        Tabview . Tabcntrl. lbUserctrl = new LbUserControl ( );
            //      Debug . WriteLine ( $"Saved & using a NEW LbUserctrl to our ViewModel Dictionary " );
            //    }
            //    else
            //        Debug . WriteLine ( $"LbUserctrl was null, but we are now using our ViewModel Dictionary''s version " );
            //    Tabitem . Content = Tabview . Tabcntrl. lbUserctrl;
            //    Tabview . Tabcntrl. lbUserctrl . DataContext = Tabview . Tabcntrl. lbUserctrl;
            //}
            //else {
            //    Debug . WriteLine ( $"We do ALREADY HAVE LbUserctrl in ViewModel Template selection" );

            //    LbUserControl test = Tabview . Tabcntrl . lbUserctrl;
            //    if ( test == null ) {
            //        test = new LbUserControl();
            //        // reset to our saved control
            //        if ( test != null ) {
            //            Tabview . Tabcntrl. lbUserctrl = test;
            //        }
            //        else
            //            Debug . WriteLine ( $"Saviing of LbUserctrl to our ViewModel Dictionary FAILED..... " );

            //        Tabview.Tabcntrl.tabItem . Content = Tabview . Tabcntrl. lbUserctrl;
            //        Tabview . Tabcntrl. lbUserctrl . DataContext = test;
            //        Utils . ScrollLBRecordIntoView ( Tabview . Tabcntrl. lbUserctrl . listbox1 , Tabview . Tabcntrl. lbUserctrl . listbox1 . SelectedIndex );
            //    }
            //    else {
            //        Debug . WriteLine ( $"We ARE NOW using our ViewModel Dictionary LbUserctrl" );
            //        Tabview . Tabcntrl. lbUserctrl = test;
            //        Tabview . Tabcntrl . tabItem . Content = Tabview . Tabcntrl. lbUserctrl;
            //    }
            //    Tabview . Tabcntrl. lbUserctrl . DataContext = Tabview . Tabcntrl. lbUserctrl;
            //}
            return null;
        }
        public LvUserControl GetCurrentListview ( LvUserControl lvUserctrl ) {
            //Tabview . Tabcntrl. lvUserctrl = ViewModel . GetViewmodel ( "LvUserControl" ) as LvUserControl;
            if ( Tabview . Tabcntrl . lvUserctrl == null ) {
                Debug . WriteLine ( $"We do NOT HAVE vbUserctrl in ViewModel Template selection" );
                //Tabview . Tabcntrl. lvUserctrl = ViewModel . GetViewmodel ( "LvUserControl" ) as LvUserControl;
                if ( Tabview . Tabcntrl . lvUserctrl == null ) {
                    Tabview . Tabcntrl . lvUserctrl = new LvUserControl ( );
                    ViewModel . SaveViewmodel ( "LbUserControl" , Tabview . Tabcntrl . lvUserctrl );
                    Debug . WriteLine ( $"Saved & using a NEW LvUserctrl to our ViewModel Dictionary " );
                }
                else
                    Debug . WriteLine ( $"LvUserctrl was null, but we are now using our ViewModel Dictionary''s version " );
                Tabitem . Content = Tabview . Tabcntrl . lvUserctrl;
                Tabview . Tabcntrl . lvUserctrl . DataContext = Tabview . Tabcntrl . lvUserctrl;
            }
            else {
                Debug . WriteLine ( $"We do ALREADY HAVE LvUserctrl in ViewModel Template selection" );

                LvUserControl test = Tabview . Tabcntrl . lvUserctrl;
                if ( Tabview . Tabcntrl . lvUserctrl == null ) {
                    Debug . WriteLine ( $"We do NOT have an LvUserctrl in our ViewModel Dictionary " );
                    test = new LvUserControl ( );
                    if ( test != null ) {
                        Tabview . Tabcntrl . lvUserctrl = test;
                    }
                    else
                        Debug . WriteLine ( $"Saving of LvUserctrl to our ViewModel Dictionary FAILED..... " );

                    Tabitem . Content = Tabview . Tabcntrl . lvUserctrl;
                    Tabview . Tabcntrl . lvUserctrl . DataContext = test;
                    Utils . ScrollLBRecordIntoView ( Tabview . Tabcntrl . lvUserctrl . listview1 , Tabview . Tabcntrl . lvUserctrl . listview1 . SelectedIndex );
                }
                else {
                    Debug . WriteLine ( $"We ARE NOW using our ViewModel Dictionary LvUserctrl" );
                    Tabview . Tabcntrl . lvUserctrl = test;
                    Tabitem . Content = Tabview . Tabcntrl . lvUserctrl;
                }
                Tabview . Tabcntrl . lvUserctrl . DataContext = Tabview . Tabcntrl . lvUserctrl;
            }
            return Tabview . Tabcntrl . lvUserctrl;
        }
        private void Grid1_PreviewGotKeyboardFocus ( object sender , KeyboardFocusChangedEventArgs e ) {
            throw new NotImplementedException ( );
        }

        #region ASYNC data load methods

        //      public async Task TabData LoadListboxData ( TabData tabdata ) {
        public async Task LoadListboxData ( TabData tabdata ) {
            //bool success = true;
            //LbUserControl Tabview . Tabcntrl. lbUserctrl;
            if ( tabdata . lbctrl == null ) {
                Tabview . Tabcntrl . lbUserctrl = new LbUserControl ( );
                Debug . WriteLine ( $"Loading Db....." );
                //                    DataTemplate dt = Application . Current . FindResource ( "CustomersDbTemplate1" ) as DataTemplate;
                DataTemplate dt = Application . Current . FindResource ( "BankDataTemplate1" ) as DataTemplate;
                Tabview . Tabcntrl . lbUserctrl . listbox1 . ItemTemplate = dt;
                Thickness th = new Thickness ( );
                th . Left = 5;
                th . Top = 10;
                Tabview . Tabcntrl . lbUserctrl . Margin = th;
                Tabview . Tabcntrl . lbUserctrl . CurrentType = "BANK";
                DbType = Tabview . Tabcntrl . lbUserctrl?.CurrentType;
                await Task . Run ( ( ) => Tabview . Tabcntrl . lbUserctrl . LoadBank ( true ) );
                Mouse . OverrideCursor = Cursors . Arrow;
                Tabview . Tabcntrl . lbUserctrl . UpdateLayout ( );
                tabdata . lbctrl = Tabview . Tabcntrl . lbUserctrl;
                //Tabview . Tabcntrl. lbUserctrl = Tabview . Tabcntrl. lbUserctrl;
                //ListBox is populated when we reach here
            }
            //} );
            return;
            //            return tabdata;
        }
        private async Task LoadListBoxAsync ( ) {
            // We HAVE to  run most of this code inDispatcher as we are accessing UI elements
            // Debug . WriteLine ( $"Running TASK for LbUserControl" );
            Tview . Dispatcher . Invoke ( DispatcherPriority . Normal , ( Action ) ( async ( ) => {
                ProgressValue = 25;
                Tview . ProgressBar_Progress . UpdateLayout ( );
                Mouse . OverrideCursor = Cursors . Wait;
                LbUserControl lbUser = new LbUserControl ( );
                lbUser . Name = "temporary";
                // Debug . WriteLine ( $"Created new  Listbox in Task" );
                lbUser . Visibility = Visibility . Visible;
                if ( lbUser . listbox1 . ItemsSource == null ) {
                    // Debug . WriteLine ( $"Loading Db data from disk via SQL....." );
                    ProgressValue = 45;
                    Tview . ProgressBar_Progress . UpdateLayout ( );
                    //                    DataTemplate dt = Application . Current . FindResource ( "CustomersDbTemplate1" ) as DataTemplate;
                    DataTemplate dt = Application . Current . FindResource ( "BankDataTemplate1" ) as DataTemplate;
                    lbUser . listbox1 . ItemTemplate = dt;
                    Tview . ListboxTab . Content = lbUser;
                    lbUser . CurrentType = "BANK";
                    DbType = lbUser?.CurrentType;
                    // I do NOT want to wait here, as we message when data has been loaded....

                    await Task . Run ( async ( ) => {
                        Application . Current . Dispatcher . Invoke (
                                        ( ) => lbUser . LoadBank ( true )
                                );
                    } );

                    //                    await Task . Run ( ( ) => lbUser . LoadBank ( true ));
                    lbUser . listbox1 . ItemsSource = lbUser . Bvm;
                    if ( Tabview . Tabcntrl . lbUserctrl != null ) {
                        //                    Debug . WriteLine ( $"Before App : {Tabview . Tabcntrl . lbUserctrl . Name} == Local : {lbUser . Name} ??" );
                        SendWindowMessage ( $"Before App : {Tabview . Tabcntrl . lbUserctrl . Name} == Local : {lbUser . Name} ??" );
                    }
                    Tabview . Tabcntrl . lbUserctrl = lbUser;
                    //                   Debug . WriteLine ( $"After App : {Tabview . Tabcntrl . lbUserctrl . Name} == Local : {lbUser . Name} ??" );
                    SendWindowMessage ( $"After App : {Tabview . Tabcntrl . lbUserctrl . Name} == Local :  {lbUser . Name} ??" );
                    if ( Tabview . Tabcntrl . lbUserctrl . Name != "" ) {
                        //                       Debug . WriteLine ( $"App now has full copy of ListBoxUserControl from the Task thread" );
                        SendWindowMessage ( $"App now has full copy of ListBoxUserControl from the Task thread" );
                    }
                    ProgressValue = 75;
                    Tview . ProgressBar_Progress . UpdateLayout ( );
                }
                //                Debug . WriteLine ( $"Returning to main UI thread.." );
            }
            ) );
            ProgressValue = 100;
            //Tview . ProgressBar_Progress . UpdateLayout ( );
            Mouse . OverrideCursor = Cursors . Arrow;
            return;
        }
        public async Task LoadListViewAsync ( LvUserControl userctrl ) {
            //Debug . WriteLine ( $"Running TASK for LvUserControl" );
            // We HAVE to  run most of this code inDispatcher as we are accessing UI elements
            //Mouse . OverrideCursor = Cursors . Wait;
            watch . Reset ( );
            watch . Start ( );
            Application . Current . Dispatcher . Invoke ( DispatcherPriority . Normal , ( Action ) ( async ( ) => {
                //Debug . WriteLine ( $"In Dispatcher....." );
                if ( userctrl == null ) {
                    //Mouse . OverrideCursor = Cursors . Wait;
                    //Debug . WriteLine ( $"Loading LvUserControl ....." );
                    ProgressValue = 15;
                    Tview . ProgressBar_Progress . UpdateLayout ( );
                    // speed up this process as we do not need it just yet
                    Task loadcontrol = new Task ( ( ) => {
                        Tabview . Tabcntrl . lvUserctrl = new LvUserControl ( );
                    } );
                    loadcontrol . Start ( );
                }
                else
                    Tabview . Tabcntrl . lvUserctrl = userctrl;
                //Debug . WriteLine ( $"LvUserControl  LOADED ....." );
                //Debug . WriteLine ( $"Continuing processing in LoadListViewAsync()" );
                //Debug . WriteLine ( $"Manipulating Content/Control size in LoadListViewAsync()" );
                //Tabitem = Tabcontrol . Items [ 2 ] as TabItem;
                Tabitem . Content = Tabview . Tabcntrl . lvUserctrl;
                //Debug . WriteLine ( $"TIrem : {Tabitem . Height}, {Tabitem . Width}" );
                ListView lb = Tabview . Tabcntrl . lvUserctrl . listview1;
                ProgressValue = 35;
                Tview . ProgressBar_Progress . UpdateLayout ( );
                lb . HorizontalAlignment = HorizontalAlignment . Left;
                Tabview . Tabcntrl . lvUserctrl . Width = Tabitem . ActualWidth - 5;
                Tabview . Tabcntrl . lvUserctrl . Height = Tabitem . ActualHeight - 30;
                ProgressValue = 35;
                Tview . ProgressBar_Progress . UpdateLayout ( );

                // How to call a Converter from c#
                ReduceByParamValue converter = new ReduceByParamValue ( );
                lb . Height = Tabview . Tabcntrl . lvUserctrl . Height = Convert . ToDouble ( converter . Convert ( Tabcontrol . ActualHeight , typeof ( double ) , 40 , CultureInfo . CurrentCulture ) );
                lb . Width = Tabview . Tabcntrl . lvUserctrl . Width = Convert . ToDouble ( converter . Convert ( Tabcontrol . ActualWidth , typeof ( double ) , 10 , CultureInfo . CurrentCulture ) );
                Tabview . Tabcntrl . lvUserctrl . Visibility = Visibility . Visible;
                Tabitem . IsSelected = true;
                //Debug . WriteLine ( $"checking  for data loaded " );

                if ( Tabview . Tabcntrl . lvUserctrl . listview1 . ItemsSource == null ) {
                    // Largish comment here .....
                    {
                        //---------------------------------------//
                        // This is some clever shit really !!!
                        // I am creating 2 tasks here, one  as a
                        // child of the other, BUT having them
                        // BOTH run in parallel for max efficiency
                        // It actually Works too !!
                        //---------------------------------------//
                    }
                    // Debug . WriteLine ( "Loading data - Calling multiple task system here......" );

                    // First, declare the Main Task (task2() here) as a new task with the usual ( () => { setup
                    // DO NOT CLOSE IT ..... just wrap it all in { ......  }
                    Task maintask = new Task ( ( ) => {
                        // Large comment here .....
                        {
                            // Thereafter the basic logic is this :-
                            // Next, without closing the main task2 with a ); we declare the secondary Task
                            //(child() here) as a new task with the usual ' ( () => { '  setup and
                            //place the relevant code in there, finally closing it withthe usual  '}'
                            // add the  ,TaskCreationOptions. AttachedToParent statement to the CHILD after it's closing ' }
                            // '(Note the comma at the front..) and *** without*** any terminating semi colon.
                            // Then close the child Task with the usual   ' ); '
                            // Run it with ' child . Start(); ' immediately after its declaration
                            // Next, while stil inside outer ' { '  declare the Parent task as another new task directly below the ' child.Start(); '
                            // finally Close the outer (main) Task OFF and finally :
                            // Call ' task2 . Start(); ' to run the entire thing......
                        }

                        //The outer Task is running the innermost Task(s), but not waiting for it . This is the beauty of an asynchronous call .
                        Task child = new Task ( ( ) => {
                            //                          Debug . WriteLine ( "parent task starting ......" );
                            Tabview . Tabcntrl . lvUserctrl . LoadBank ( true );
                            //                            Debug . WriteLine ( "parent task ended ......" );

                        } , TaskCreationOptions . AttachedToParent
                       );

                        child . Start ( );
                        // Parent Task

                        // MAIN TASK HERE
                        Application . Current . Dispatcher . Invoke ( ( ) => {
                            //                           Debug . WriteLine ( "child task starting ......" );
                            ProgressValue = 55;
                            Tview . ProgressBar_Progress . UpdateLayout ( );
                            //Debug . WriteLine ( $"Loading Db....." );
                            DataTemplate dt = Application . Current . FindResource ( "BankDataTemplate1" ) as DataTemplate;
                            Tabview . Tabcntrl . lvUserctrl . listview1 . ItemTemplate = dt;
                            Tview . ListviewTab . Content = Tabview . Tabcntrl . lvUserctrl;
                            Tabitem . Content = Tabview . Tabcntrl . lvUserctrl;
                            Tabview . Tabcntrl . lvUserctrl . CurrentType = "BANK";
                            DbType = Tabview . Tabcntrl . lvUserctrl?.CurrentType;
                            //Mouse . OverrideCursor = Cursors . Arrow;
                            //Debug . WriteLine ( "child task ended ......" );
                            Tabview . Tabcntrl . lvUserctrl . UpdateLayout ( );
                            ProgressValue = 75;
                            Tview . ProgressBar_Progress . UpdateLayout ( );
                        } );
                        //Debug. WriteLine ( "parent task starting ......" );
                        //Tabview . Tabcntrl. lvUserctrl . LoadBank ( true );
                        //Debug. WriteLine ( "parent task ended ......" );
                    }
                    );
                    //                    Debug . WriteLine ( "calling maintask ......" );
                    maintask . Start ( );
                    //                    Debug . WriteLine ( "maintask ended ......" );
                }
                //                Debug . WriteLine ( "multiple task system Ended ......" );
                //                Debug . WriteLine ( "Following Code is being processed WHILE tadsks are running......" );

                //---------------------------------------//
                //                Debug . WriteLine ( $"Data loaded so showing tab.." );
                Tabcontrol . SelectedIndex = 2;
                CurrentTabIndex = Tabcontrol . SelectedIndex;
                DbType = Tabview . Tabcntrl . lvUserctrl?.CurrentType;
                Tabview . Tabcntrl . lvUserctrl . listview1 . ScrollIntoView ( Tabview . Tabcntrl . lvUserctrl . listview1 . SelectedItem );
                Utils . ScrollLBRecordIntoView ( Tabview . Tabcntrl . lvUserctrl . listview1 , Tabview . Tabcntrl . lvUserctrl . listview1 . SelectedIndex );
                Tabview . Tabcntrl . lvUserctrl . Visibility = Visibility . Visible;
                Tabview . Tabcntrl . lvUserctrl . UpdateLayout ( );
                ProgressValue = 100;
                Tview . ProgressBar_Progress . UpdateLayout ( );
                Mouse . OverrideCursor = Cursors . Arrow;
            } ) );
            //Debug . WriteLine ( $"*** Exiting async method entirely .. NB : Data may still be loading in maintask ***" );
            long msecs = watch . ElapsedMilliseconds;
            watch . Stop ( );
            //Debug . WriteLine ( $"Elapsed msecs = {msecs}" );
            return;
        }

        public async Task LoadDataGridAsync ( DgUserControl userctrl ) {
            //            Debug . WriteLine ( $"Running TASK for LvUserControl" );
            // We HAVE to  run most of this code inDispatcher as we are accessing UI elements
            //Mouse . OverrideCursor = Cursors . Wait;
            watch . Reset ( );
            watch . Start ( );
            //Application . Current . Dispatcher . Invoke ( DispatcherPriority . Normal , ( Action ) ( async ( ) =>
            //{
            //            Debug . WriteLine ( $"In Dispatcher....." );
            if ( userctrl == null ) {
                //Mouse . OverrideCursor = Cursors . Wait;
                //                Debug . WriteLine ( $"Loading DgUserControl ....." );
                ProgressValue = 15;
                Tview . ProgressBar_Progress . UpdateLayout ( );
                // speed up this process as we do not need it just yet
                Task loadcontrol = new Task ( ( ) => {
                    Tabview . Tabcntrl . dgUserctrl = new DgUserControl ( );
                } );
                loadcontrol . Start ( );
            }
            else
                Tabview . Tabcntrl . dgUserctrl = userctrl;

            //Debug . WriteLine ( $"DgUserControl  LOADED ....." );
            //Debug . WriteLine ( $"Continuing processing in LoadDataGridAsync()" );
            //Debug . WriteLine ( $"Manipulating Content/Control size in LoadDataGridAsync()" );
            Tabitem = Tabcontrol . Items [ 2 ] as TabItem;
            Tabitem . Content = Tabview . Tabcntrl . dgUserctrl;
            //Debug . WriteLine ( $"TIrem : {Tabitem . Height}, {Tabitem . Width}" );
            DataGrid dg = Tabview . Tabcntrl . dgUserctrl . grid1;
            ProgressValue = 35;
            Tview . ProgressBar_Progress . UpdateLayout ( );
            dg . HorizontalAlignment = HorizontalAlignment . Left;
            Tabview . Tabcntrl . dgUserctrl . Width = Tabitem . ActualWidth - 5;
            Tabview . Tabcntrl . dgUserctrl . Height = Tabitem . ActualHeight - 30;
            ProgressValue = 35;
            Tview . ProgressBar_Progress . UpdateLayout ( );

            // How to call a Converter from c#
            ReduceByParamValue converter = new ReduceByParamValue ( );
            dg . Height = Tabview . Tabcntrl . dgUserctrl . Height = Convert . ToDouble ( converter . Convert ( Tabcontrol . ActualHeight , typeof ( double ) , 40 , CultureInfo . CurrentCulture ) );
            dg . Width = Tabview . Tabcntrl . dgUserctrl . Width = Convert . ToDouble ( converter . Convert ( Tabcontrol . ActualWidth , typeof ( double ) , 10 , CultureInfo . CurrentCulture ) );
            Tabview . Tabcntrl . dgUserctrl . Visibility = Visibility . Visible;
            Tabitem . IsSelected = true;
            //Debug . WriteLine ( $"checking  for data loaded " );

            if ( Tabview . Tabcntrl . dgUserctrl . grid1 . ItemsSource == null ) {
                // Largish comment here .....
                {
                    //---------------------------------------//
                    // This is some clever shit really !!!
                    // I am creating 2 tasks here, one  as a
                    // child of the other, BUT having them
                    // BOTH run in parallel for max efficiency
                    // It actually Works too !!
                    //---------------------------------------//
                }
                //Debug . WriteLine ( "Loading data - Calling multiple task system here......" );

                // First, declare the Main Task (task2() here) as a new task with the usual ( () => { setup
                // DO NOT CLOSE IT ..... just wrap it all in { ......  }
                //Task maintask = new Task ( ( ) =>
                //{
                // Large comment here .....
                {
                    // Thereafter the basic logic is this :-
                    // Next, without closing the main task2 with a ); we declare the secondary Task
                    //(child() here) as a new task with the usual ' ( () => { '  setup and
                    //place the relevant code in there, finally closing it withthe usual  '}'
                    // add the  ,TaskCreationOptions. AttachedToParent statement to the CHILD after it's closing ' }
                    // '(Note the comma at the front..) and *** without*** any terminating semi colon.
                    // Then close the child Task with the usual   ' ); '
                    // Run it with ' child . Start(); ' immediately after its declaration
                    // Next, while stil inside outer ' { '  declare the Parent task as another new task directly below the ' child.Start(); '
                    // finally Close the outer (main) Task OFF and finally :
                    // Call ' task2 . Start(); ' to run the entire thing......
                }

                //The outer Task is running the innermost Task(s), but not waiting for it . This is the beauty of an asynchronous call .
                //Task child = new Task ( ( ) =>
                //{
                //Application . Current . Dispatcher . Invoke ( DispatcherPriority . Normal , ( Action ) (async ( ) =>
                //{
                //    Debug. WriteLine ( "child task starting ......" );
                ProgressValue = 55;
                Tview . ProgressBar_Progress . UpdateLayout ( );
                //Debug . WriteLine ( $"Loading Db....." );
                DataTemplate dt = Application . Current . FindResource ( "BankDataTemplate1" ) as DataTemplate;
                Tabview . Tabcntrl . dgUserctrl . grid1 . ItemTemplate = dt;
                Tview . DgridTab . Content = Tabview . Tabcntrl . dgUserctrl;
                Tabitem . Content = Tabview . Tabcntrl . dgUserctrl;
                Tabview . Tabcntrl . dgUserctrl . CurrentType = "BANK";
                DbType = Tabview . Tabcntrl . dgUserctrl?.CurrentType;
                //Mouse . OverrideCursor = Cursors . Arrow;
                //  Debug . WriteLine ( "child task ended ......" );
                Tabview . Tabcntrl . dgUserctrl . UpdateLayout ( );
                ProgressValue = 75;
                Tview . ProgressBar_Progress . UpdateLayout ( );
                //   } , TaskCreationOptions . AttachedToParent
                //);

                //child . Start ( );
                //Debug . WriteLine ( "parent task starting ......" );
                //Tabview . Tabcntrl. dgUserctrl . LoadBank ( true );
                ObservableCollection<BankAccountViewModel> Bvm = new ObservableCollection<BankAccountViewModel> ( );
                UserControlDataAccess . GetBankObsCollection ( true , "DgUserControl" , -1 );
                //                    } );

                //Debug . WriteLine ( "parent task ended ......" );

                // Parent Task

                // MAIN TASK HERE
                //Debug. WriteLine ( "child task starting ......" );
                //ProgressValue = 55;
                //Tview . ProgressBar_Progress . UpdateLayout ( );
                //Debug. WriteLine ( $"Loading Db....." );
                //DataTemplate dt = Application . Current . FindResource ( "BankDataTemplate1" ) as DataTemplate;
                //Tabview . Tabcntrl. dgUserctrl . grid1 . ItemTemplate = dt;
                //Tview . DgridTab . Content = Tabview . Tabcntrl. dgUserctrl;
                //TItem . Content = Tabview . Tabcntrl. dgUserctrl;
                //Tabview . Tabcntrl. dgUserctrl . CurrentType = "BANK";
                //DbType = Tabview . Tabcntrl. dgUserctrl?.CurrentType;
                ////Mouse . OverrideCursor = Cursors . Arrow;
                //Debug. WriteLine ( "child task ended ......" );
                //Tabview . Tabcntrl. dgUserctrl . UpdateLayout ( );
                //ProgressValue = 75;
                //Tview . ProgressBar_Progress . UpdateLayout ( );

                ////Debug. WriteLine ( "parent task starting ......" );
                ////Tabview . Tabcntrl. lvUserctrl . LoadBank ( true );
                ////Debug. WriteLine ( "parent task ended ......" );
                //}
                //);
                //Debug. WriteLine ( "calling maintask ......" );
                //maintask . Start ( );
                //Debug. WriteLine ( "maintask ended ......" );
                //}
                //Debug . WriteLine ( "multiple task system Ended ......" );
                //Debug . WriteLine ( "Following Code is being processed WHILE tadsks are running......" );

                //---------------------------------------//
                //Debug . WriteLine ( $"Data loaded so showing tab.." );
                Tabcontrol . SelectedIndex = 2;
                CurrentTabIndex = Tabcontrol . SelectedIndex;
                DbType = Tabview . Tabcntrl . dgUserctrl?.CurrentType;
                //                Tabview . Tabcntrl. dgUserctrl . grid1 . ScrollIntoView ( Tabview . Tabcntrl. dgUserctrl . grid1 . SelectedItem );
                //                Utils . ScrollRecordIntoView ( Tabview . Tabcntrl. dgUserctrl . grid1 , Tabview . Tabcntrl. dgUserctrl . grid1 . SelectedIndex );
                Tabview . Tabcntrl . dgUserctrl . Visibility = Visibility . Visible;
                Tabview . Tabcntrl . dgUserctrl . UpdateLayout ( );
                ProgressValue = 100;
                Tview . ProgressBar_Progress . UpdateLayout ( );
                //            } ) );
                //Debug . WriteLine ( $"*** Exiting async method entirely .. NB : Data may still be loading in maintask ***" );
                long msecs = watch . ElapsedMilliseconds;
                watch . Stop ( );
                //Debug . WriteLine ( $"Elapsed msecs = {msecs}" );
                Mouse . OverrideCursor = Cursors . Arrow;
                return;
            }
        }
        #endregion ASYNC data load methods

        private void SendWindowMessage ( string msg = "" ) {
            InterWindowArgs args = new InterWindowArgs ( );

            args . data = null;
            args . window = null;
            args . message = msg;
            EventControl . TriggerWindowMessage ( this , args );
        }
        private void CloseAppExecute ( object obj ) {
            Application . Current . Shutdown ( );
        }
        private void CloseWinExecute ( object obj ) {
            if ( Tabview . Tabcntrl . dgUserctrl != null ) {
                Tabview . Tabcntrl . dgUserctrl . grid1 . ItemsSource = null;
                Tabview . Tabcntrl . dgUserctrl . grid1 . Items . Clear ( );
                Tabview . Tabcntrl . dgUserctrl = null;
                Tview . DgridTab . Content = null;
            }
            if ( Tabview . Tabcntrl . lbUserctrl != null ) {
                Tabview . Tabcntrl . lbUserctrl . listbox1 . ItemsSource = null;
                Tabview . Tabcntrl . lbUserctrl . listbox1 . Items . Clear ( );
                Tabview . Tabcntrl . lbUserctrl = null;
                Tview . ListboxTab . Content = null;
            }
            if ( Tabview . Tabcntrl . lvUserctrl != null ) {
                Tabview . Tabcntrl . lvUserctrl . listview1 . ItemsSource = null;
                Tabview . Tabcntrl . lvUserctrl . listview1 . Items . Clear ( );
                Tabview . Tabcntrl . lvUserctrl = null;
                Tview . ListviewTab . Content = null;
            }
            if ( tvUserctrl != null ) {
                tvUserctrl . treeview1 . ItemsSource = null;
                tvUserctrl . treeview1 . Items . Clear ( );
                tvUserctrl = null;
                Tview . TreeviewTab . Content = null;
            }
            if ( logUserctrl != null ) {
                logUserctrl . logview . ItemsSource = null;
                logUserctrl . logview . Items . Clear ( );
                logUserctrl = null;
                Tview . LogviewTab . Content = null;
            }
            Tabview . Tabcntrl . lbUserctrl = null;
            Tabview . Tabcntrl . lvUserctrl = null;
            Tabview . Tabcntrl . dgUserctrl = null;
            logUserctrl = null;
            tvUserctrl = null;
            ThisWin = null;
            Tview . Close ( );
        }
        public void Closedown ( ) {
            Tabview . Tabcntrl . lbUserctrl = null;
            Tabview . Tabcntrl . lvUserctrl = null;
            Tabview . Tabcntrl . dgUserctrl = null;
            tvUserctrl = null;
            ThisWin = null;
        }
        private void ClearTags ( ) {
            if ( Tabview . Tabcntrl . dgUserctrl != null )
                Tabview . Tabcntrl . dgUserctrl . Tag = false;
            if ( Tabview . Tabcntrl . lbUserctrl != null )
                Tabview . Tabcntrl . lbUserctrl . Tag = false;
            if ( Tabview . Tabcntrl . lvUserctrl != null )
                Tabview . Tabcntrl . lvUserctrl . Tag = false;
            if ( Tabview . Tabcntrl . lgUserctrl != null )
                Tabview . Tabcntrl . lgUserctrl . Tag = false;
            if ( Tabview . Tabcntrl . tvUserctrl != null )
                Tabview . Tabcntrl . tvUserctrl . Tag = false;
        }
        private void TemplatesCb_SelectionChanged ( object sender , SelectionChangedEventArgs e ) {
            //DataTemplateSelector Bs = new DataTemplateSelector ( );
            //DataTemplateSelectors . SelectDataTemplate ( Tview . TemplatesCb . SelectedItem . ToString ( ) , Tabview . Tabcntrl . dgUserctrl . grid1 );
        }
        private void ProcessDgTemplate ( ) {
            Tabview . Tabcntrl . ActiveControlType = Tabview . Tabcntrl . dgUserctrl;
            Tabview . Tabcntrl . tabView . TemplatesCb . SelectedIndex = Tabview . Tabcntrl . DtTemplates . TemplateIndexDg;
            Tabview . Tabcntrl . dgUserctrl . grid1 . ScrollIntoView ( Tabview . Tabcntrl . dgUserctrl . grid1 . SelectedIndex );
            Tabview . Tabcntrl . tabView . IsLoading = true;
            Tabview . Tabcntrl . tabView . DbnamesCb . SelectedIndex = Tabview . Tabcntrl . DbNameIndexDg;
            Tabview . Tabcntrl . tabView . IsLoading = false;
        }
        private void ProcessLbTemplate ( ) {
            Tabview . Tabcntrl . ActiveControlType = Tabview . Tabcntrl . lbUserctrl;
            Tabview . Tabcntrl . tabView . TemplatesCb . SelectedIndex = Tabview . Tabcntrl . DtTemplates . TemplateIndexLb;
            Tabview . Tabcntrl . tabView . IsLoading = true;
            Tabview . Tabcntrl . tabView . DbnamesCb . SelectedIndex = Tabview . Tabcntrl . DbNameIndexLb;
            Tabview . Tabcntrl . tabView . IsLoading = false;
        }
        private void ProcessLvTemplate ( ) {
            Tabview . Tabcntrl . ActiveControlType = Tabview . Tabcntrl . lvUserctrl;
            Tabview . Tabcntrl . tabView . TemplatesCb . SelectedIndex = Tabview . Tabcntrl . DtTemplates . TemplateIndexLv;
            Tabview . Tabcntrl . tabView . IsLoading = true;
            Tabview . Tabcntrl . tabView . DbnamesCb . SelectedIndex = Tabview . Tabcntrl . DbNameIndexLv;
            Tabview . Tabcntrl . tabView . IsLoading = false;
        }
        public void CheckActiveTemplate ( object control ) {
            if ( Tabview . Tabcntrl . ActiveControlType == Tabview . Tabcntrl . dgUserctrl ) {
                // DATAGRID ACTIONS
                if ( Tabview . Tabcntrl . DtTemplates . TemplateNameDg == null || Tabview . Tabcntrl . DtTemplates . TemplateNameDg == "BANKACCOUNT" ) {
                    Tabview . Tabcntrl . DtTemplates . TemplateNameDg = "BANKACCOUNT";
                    Tabview . Tabcntrl . DtTemplates . TemplatesCombo . ItemsSource = Tabview . DataTemplatesBank;
                    ProcessDgTemplate ( );
                    //Tabview . Tabcntrl . ActiveControlType = Tabview . Tabcntrl . dgUserctrl;
                    //Tabview . Tabcntrl . tabView . TemplatesCb . SelectedIndex = Tabview . Tabcntrl . DtTemplates . TemplateIndexDg;
                    //Tabview . Tabcntrl . dgUserctrl . grid1 . ScrollIntoView ( Tabview . Tabcntrl . dgUserctrl . grid1 . SelectedIndex );
                    //Tabview . Tabcntrl . tabView . IsLoading = true;
                    //Tabview . Tabcntrl . tabView . DbnamesCb . SelectedIndex = Tabview . Tabcntrl . DbNameIndexDg;
                    //Tabview . Tabcntrl . tabView . IsLoading = false;
                }
                else if ( Tabview . Tabcntrl . DtTemplates . TemplateNameDg == null || Tabview . Tabcntrl . CurrentTypeDg == "CUSTOMER" ) {
                    // DATAGRID ACTIONS
                    Tabview . Tabcntrl . DtTemplates . TemplateNameDg = "CUSTOMER";
                    Tabview . Tabcntrl . DtTemplates . TemplatesCombo . ItemsSource = Tabview . DataTemplatesCust;
                    ProcessDgTemplate ( );
                    //Tabview . Tabcntrl . ActiveControlType = Tabview . Tabcntrl . dgUserctrl;
                    //Tabview . Tabcntrl . tabView . TemplatesCb . SelectedIndex = Tabview . Tabcntrl . DtTemplates . TemplateIndexDg;
                    //Tabview . Tabcntrl . dgUserctrl . grid1 . ScrollIntoView ( Tabview . Tabcntrl . dgUserctrl . grid1 . SelectedIndex );
                    //Tabview . Tabcntrl . tabView . IsLoading = true;
                    //Tabview . Tabcntrl . tabView . DbnamesCb . SelectedIndex = Tabview . Tabcntrl . DbNameIndexDg;
                    //Tabview . Tabcntrl . tabView . IsLoading = false;
                }
                else if ( Tabview . Tabcntrl . DtTemplates . TemplateNameDg != "BANKACCOUNT"
                    && Tabview . Tabcntrl . DtTemplates . TemplateNameDg != "CUSTOMER" ) {
                    // DATAGRID ACTIONS
                    Tabview . Tabcntrl . DtTemplates . TemplateNameDg = "GEN";
                    Tabview . Tabcntrl . DtTemplates . TemplatesCombo . ItemsSource = Tabview . DataTemplatesGen;
                    ProcessDgTemplate ( );
                    //Tabview . Tabcntrl . ActiveControlType = Tabview . Tabcntrl . dgUserctrl;
                    //Tabview . Tabcntrl . tabView . TemplatesCb . SelectedIndex = Tabview . Tabcntrl . DtTemplates . TemplateIndexDg;
                    //Tabview . Tabcntrl . tabView . IsLoading = true;
                    //Tabview . Tabcntrl . tabView . DbnamesCb . SelectedIndex = Tabview . Tabcntrl . DbNameIndexDg;
                    //Tabview . Tabcntrl . tabView . IsLoading = false;
                }
                Tabview . Tabcntrl . tabView . DbnamesCb . UpdateLayout ( );
                TabWinViewModel . TriggerDbType ( Tabview . Tabcntrl . DtTemplates . TemplateNameDg );
                WpfLib1 . Utils . ScrollRecordIntoView ( Tabview . Tabcntrl . dgUserctrl . grid1 , Tabview . Tabcntrl . dgUserctrl . grid1 . SelectedIndex );
            }
            else if ( Tabview . Tabcntrl . ActiveControlType == Tabview . Tabcntrl . lbUserctrl ) {
                // LISTBOX ACTIONS
                if ( Tabview . Tabcntrl . DtTemplates . TemplateNameLb == null || Tabview . Tabcntrl . DtTemplates . TemplateNameLb == "BANKACCOUNT" ) {
                    Tabview . Tabcntrl . DtTemplates . TemplateNameLb = "BANKACCOUNT";
                    Tabview . Tabcntrl . DtTemplates . TemplatesCombo . ItemsSource = Tabview . DataTemplatesBank;
                    ProcessLbTemplate ( );
                    //Tabview . Tabcntrl . ActiveControlType = Tabview . Tabcntrl . lbUserctrl;
                    //Tabview . Tabcntrl . tabView . TemplatesCb . SelectedIndex = Tabview . Tabcntrl . DtTemplates . TemplateIndexLb;
                    //Tabview . Tabcntrl . tabView . IsLoading = true;
                    //Tabview . Tabcntrl . tabView . DbnamesCb . SelectedIndex = Tabview . Tabcntrl . DbNameIndexLb;
                    //Tabview . Tabcntrl . tabView . IsLoading = false;
                }
                else if ( Tabview . Tabcntrl . DtTemplates . TemplateNameLb == null || Tabview . Tabcntrl . DtTemplates . TemplateNameLb == "CUSTOMER" ) {
                    // LISTBOX ACTIONS
                    Tabview . Tabcntrl . DtTemplates . TemplateNameLb = "CUSTOMER";
                    Tabview . Tabcntrl . DtTemplates . TemplatesCombo . ItemsSource = Tabview . DataTemplatesCust;
                    ProcessLbTemplate ( );
                    //Tabview . Tabcntrl . ActiveControlType = Tabview . Tabcntrl . lbUserctrl;
                    //Tabview . Tabcntrl . tabView . TemplatesCb . SelectedIndex = Tabview . Tabcntrl . DtTemplates . TemplateIndexLb;
                    //Tabview . Tabcntrl . tabView . IsLoading = true;
                    //Tabview . Tabcntrl . tabView . DbnamesCb . SelectedIndex = Tabview . Tabcntrl . DbNameIndexLb;
                    //Tabview . Tabcntrl . tabView . IsLoading = false;
                }
                else if ( Tabview . Tabcntrl . DtTemplates . TemplateNameLb != "BANKACCOUNT"
                    && Tabview . Tabcntrl . DtTemplates . TemplateNameLb != "CUSTOMER" ) {
                    // LISTBOX ACTIONS
                    Tabview . Tabcntrl . DtTemplates . TemplateNameLb = "GEN";
                    Tabview . Tabcntrl . DtTemplates . TemplatesCombo . ItemsSource = Tabview . DataTemplatesGen;
                    ProcessLbTemplate ( );
                    //Tabview . Tabcntrl . ActiveControlType = Tabview . Tabcntrl . lbUserctrl;
                    //Tabview . Tabcntrl . tabView . TemplatesCb . SelectedIndex = Tabview . Tabcntrl . DtTemplates . TemplateIndexLb;
                    //Tabview . Tabcntrl . tabView . IsLoading = true;
                    //Tabview . Tabcntrl . tabView . DbnamesCb . SelectedIndex = Tabview . Tabcntrl . DbNameIndexLb;
                    //Tabview . Tabcntrl . tabView . IsLoading = false;
                }
                Tabview . Tabcntrl . tabView . DbnamesCb . UpdateLayout ( );
                TabWinViewModel . TriggerDbType ( Tabview . Tabcntrl . DtTemplates . TemplateNameLb );
                Tabview . Tabcntrl . lbUserctrl . listbox1 . ScrollIntoView ( Tabview . Tabcntrl . lbUserctrl . listbox1 . SelectedIndex );
            }
            else if ( Tabview . Tabcntrl . ActiveControlType == Tabview . Tabcntrl . lvUserctrl ) {
                // LISTVIEW ACTIONS
                if ( Tabview . Tabcntrl . DtTemplates . TemplateNameLv == null || Tabview . Tabcntrl . CurrentTypeLv == "BANKACCOUNT" ) {
                    Tabview . Tabcntrl . DtTemplates . TemplateNameLv = "BANKACCOUNT";
                    Tabview . Tabcntrl . DtTemplates . TemplatesCombo . ItemsSource = Tabview . DataTemplatesBank;
                    ProcessLvTemplate ( );
                    //Tabview . Tabcntrl . ActiveControlType = Tabview . Tabcntrl . lvUserctrl;
                    //Tabview . Tabcntrl . tabView . TemplatesCb . SelectedIndex = Tabview . Tabcntrl . DtTemplates . TemplateIndexLv;
                    //Tabview . Tabcntrl . tabView . IsLoading = true;
                    //Tabview . Tabcntrl . tabView . DbnamesCb . SelectedIndex = Tabview . Tabcntrl . DbNameIndexLv;
                    //Tabview . Tabcntrl . tabView . IsLoading = false;
                }
                else if ( Tabview . Tabcntrl . DtTemplates . TemplateNameLv == null || Tabview . Tabcntrl . CurrentTypeLv == "CUSTOMER" ) {
                    // LISTVIEW ACTIONS
                    Tabview . Tabcntrl . DtTemplates . TemplateNameLv = "CUSTOMER";
                    Tabview . Tabcntrl . DtTemplates . TemplatesCombo . ItemsSource = Tabview . DataTemplatesCust;
                    ProcessLvTemplate ( );
                    //Tabview . Tabcntrl . ActiveControlType = Tabview . Tabcntrl . lvUserctrl;
                    //Tabview . Tabcntrl . tabView . TemplatesCb . SelectedIndex = Tabview . Tabcntrl . DtTemplates . TemplateIndexLv;
                    //Tabview . Tabcntrl . tabView . IsLoading = true;
                    //Tabview . Tabcntrl . tabView . DbnamesCb . SelectedIndex = Tabview . Tabcntrl . DbNameIndexLv;
                    //Tabview . Tabcntrl . tabView . IsLoading = false;
                }
                else if ( Tabview . Tabcntrl . DtTemplates . TemplateNameLv != "BANKACCOUNT"
                    && Tabview . Tabcntrl . DtTemplates . TemplateNameLv != "CUSTOMER" ) {
                    // LISTVIEW ACTIONS
                    Tabview . Tabcntrl . DtTemplates . TemplateNameLv = "GEN";
                    Tabview . Tabcntrl . DtTemplates . TemplatesCombo . ItemsSource = Tabview . DataTemplatesGen;
                    ProcessLvTemplate ( );
                    //Tabview . Tabcntrl . ActiveControlType = Tabview . Tabcntrl . lvUserctrl;
                    //Tabview . Tabcntrl . tabView . TemplatesCb . SelectedIndex = Tabview . Tabcntrl . DtTemplates . TemplateIndexLv;
                    //Tabview . Tabcntrl . tabView . IsLoading = true;
                    //Tabview . Tabcntrl . tabView . DbnamesCb . SelectedIndex = Tabview . Tabcntrl . DbNameIndexLv;
                    //Tabview . Tabcntrl . tabView . IsLoading = false;
                }
                Tabview . Tabcntrl . tabView . DbnamesCb . UpdateLayout ( );
                TabWinViewModel . TriggerDbType ( Tabview . Tabcntrl . DtTemplates . TemplateNameLv );
                Tabview . Tabcntrl . lvUserctrl . listview1 . ScrollIntoView ( Tabview . Tabcntrl . lvUserctrl . listview1 . SelectedIndex );
            }
        }
        public static int FindActiveTable ( string activeDbTable ) {
            int index = 0;
            if ( activeDbTable == null ) return 0;
            foreach ( string item in Tabview . Tabcntrl . tabView . DbnamesCb . Items ) {
                if ( item . ToUpper ( ) == activeDbTable . ToUpper ( ) )
                    break;
                index++;
            }
            return index;
        }
        #region NOT USED
        #region BACKGROUND THREADS

        //*************************************************
        #region Datagrid BackgrundWorker Loading
        public DgUserControl LoadDatagridInBackgroundTask ( DgUserControl dguserctrl ) {
            BackgroundWorker worker = new BackgroundWorker ( );
            Debug . WriteLine ( $"Calling DataGrid Background Worker setup" );
            worker . WorkerSupportsCancellation = true;
            worker . WorkerReportsProgress = true;
            worker . ProgressChanged += DgWorker_ProgressChanged;
            worker . RunWorkerCompleted += DgWorker_RunWorkerCompleted;
            worker . DoWork += DgWorker_DoWork;
            Debug . WriteLine ( $"Running DataGrid Background Worker" );
            ProgressValue = 15;
            Tview . ProgressBar_Progress . UpdateLayout ( );
            worker . RunWorkerAsync ( dguserctrl );
            return dguserctrl;
        }
        private void DgWorker_DoWork ( object sender , DoWorkEventArgs e ) {
            Debug . WriteLine ( $"Calling DataGrid Background Worker ProgressChanged method" );
            BackgroundWorker worker = sender as BackgroundWorker;
            worker . ReportProgress ( 0 , e . Argument as DgUserControl );
            Debug . WriteLine ( $"Returned from DataGrid Background Worker ProgressChanged method" );
            Debug . WriteLine ( $"Cancelling thread of DataGrid ProgressChanged method" );
            worker . CancelAsync ( );
        }
        public void DgWorker_RunWorkerCompleted ( object sender , RunWorkerCompletedEventArgs e ) {
            BackgroundWorker worker = sender as BackgroundWorker;
            if ( e . Cancelled )
                Debug . WriteLine ( $"DataGrid Worker Cancelled" );
            else
                Debug . WriteLine ( $"DataGrid Worker completed" );
            Tview . ProgressBar_Progress . Value = 0;
            Tview . ProgressBar_Progress . UpdateLayout ( );
            Mouse . OverrideCursor = Cursors . Arrow;
            worker . CancelAsync ( );
        }
        private void DgWorker_ProgressChanged ( object sender , ProgressChangedEventArgs e ) {
            long msecs = 0;
            BackgroundWorker worker = sender as BackgroundWorker;
            //DgUserControl Tabview . Tabcntrl. dgUserctrl = e . UserState as DgUserControl;
            Stopwatch watch = new Stopwatch ( );
            ProgressValue = 25;
            Tview . ProgressBar_Progress . UpdateLayout ( );
            if ( Tabview . Tabcntrl . dgUserctrl == null || Tabview . Tabcntrl . dgUserctrl . grid1 . Items . Count == 0 ) {
                if ( Tabview . Tabcntrl . dgUserctrl == null )
                    Tabview . Tabcntrl . dgUserctrl = new DgUserControl ( );

                ReduceByParamValue convert;
                watch . Start ( );
                Debug . WriteLine ( $"Creating new DgUserControl in Background Worker ProgressChanged method" );
                EventControl . TriggerWindowMessage ( this , new InterWindowArgs { message = $"Creating new DgUserControl..." , listbox = null } );
                watch . Stop ( );
                msecs = watch . ElapsedMilliseconds;
                Debug . WriteLine ( $"new DgUserControl took {msecs}" );

                if ( Tabview . Tabcntrl . dgUserctrl . grid1 . ItemsSource == null ) {
                    Tview . ProgressBar_Progress . Value = 35;
                    Tview . ProgressBar_Progress . UpdateLayout ( );
                    Debug . WriteLine ( $"Loading Db....." );
                    //                    DataTemplate dt = Application . Current . FindResource ( "CustomersDbTemplate1" ) as DataTemplate;
                    DataTemplate dt = Application . Current . FindResource ( "BankDataTemplate1" ) as DataTemplate;
                    Tabview . Tabcntrl . dgUserctrl . grid1 . ItemTemplate = dt;
                    Tview . DgridTab . Content = Tabview . Tabcntrl . dgUserctrl;
                    Tabview . Tabcntrl . dgUserctrl . CurrentType = "BANK";
                    DbType = Tabview . Tabcntrl . dgUserctrl?.CurrentType;
                    EventControl . TriggerWindowMessage ( this , new InterWindowArgs { message = $"Loading Bank data in DgUserControl..." , listbox = null } );

                    Tview . Dispatcher . Invoke ( DispatcherPriority . Normal , ( Action ) ( async ( ) => {
                        await Task . Run ( ( ) => Tabview . Tabcntrl . dgUserctrl . LoadBank ( true ) );
                    } ) );

                    Tview . ProgressBar_Progress . Value = 65;
                    Tview . ProgressBar_Progress . UpdateLayout ( );
                    if ( Tabview . Tabcntrl . dgUserctrl . Bvm . Count > 0 )
                        Tabview . Tabcntrl . dgUserctrl . grid1 . ItemsSource = Tabview . Tabcntrl . dgUserctrl . Bvm;

                    Mouse . OverrideCursor = Cursors . Arrow;
                    Tabview . Tabcntrl . dgUserctrl . UpdateLayout ( );
                }

                ProgressValue = 45;
                Tview . ProgressBar_Progress . UpdateLayout ( );
                //DataGrid lb = Tabview . Tabcntrl. dgUserctrl . grid1;
                //convert = new ReduceByParamValue ( );
                //lb . Height = Tabview . Tabcntrl. dgUserctrl . Height = Convert . ToDouble ( convert . Convert ( Tabcontrol . ActualHeight , typeof ( double ) , 40 , CultureInfo . CurrentCulture ) );
                //lb . Width = Tabview . Tabcntrl. dgUserctrl . Width = Convert . ToDouble ( convert . Convert ( Tabcontrol . ActualWidth , typeof ( double ) , 10 , CultureInfo . CurrentCulture ) );
            }
            Tview . ProgressBar_Progress . Value = 75;
            Tview . ProgressBar_Progress . UpdateLayout ( );
            watch . Start ( );
            Mouse . OverrideCursor = Cursors . Wait;
            Debug . WriteLine ( $"Loading DataGrid in Background Worker ProgressChanged method" );
            Tabitem = Tabcontrol . Items [ 0 ] as TabItem;
            Tabitem . Content = Tabview . Tabcntrl . dgUserctrl;
            DataGrid dg = Tabview . Tabcntrl . dgUserctrl . grid1;
            // How to call a Converter from c#
            ReduceByParamValue converter = new ReduceByParamValue ( );
            dg . Height = Tabview . Tabcntrl . dgUserctrl . Height = Convert . ToDouble ( converter . Convert ( Tabcontrol . ActualHeight , typeof ( double ) , 40 , CultureInfo . CurrentCulture ) );
            dg . Width = Tabview . Tabcntrl . dgUserctrl . Width = Convert . ToDouble ( converter . Convert ( Tabcontrol . ActualWidth , typeof ( double ) , 10 , CultureInfo . CurrentCulture ) );
            Tabview . Tabcntrl . dgUserctrl . Visibility = Visibility . Visible;
            Tabitem . IsSelected = true;
            //Debug . WriteLine ( $"DataGrid loaded by Background Worker " );
            //Debug . WriteLine ( $"Data loaded so showing tab.." );
            watch . Stop ( );
            Debug . WriteLine ( $"new DgUserControl took {watch . ElapsedMilliseconds}, Total = Create : {msecs} + Load : {watch . ElapsedMilliseconds} =  {msecs + watch . ElapsedMilliseconds}" );
            Mouse . OverrideCursor = Cursors . Arrow;
            Tview . ProgressBar_Progress . Value = 100;
            Tview . ProgressBar_Progress . UpdateLayout ( );
            EventControl . TriggerWindowMessage ( this , new InterWindowArgs { message = $"Loading completed DgUserControl..." , listbox = null } );

        }
        #endregion Datagrid BackgrundWorker Loading

        //*************************************************
        #region ListBox BackgrundWorker Loading
        public LbUserControl LoadListboxInBackgroundTask ( LbUserControl lbuserctrl ) {
            int progress = 10;
            BackgroundWorker worker = new BackgroundWorker ( );
            Debug . WriteLine ( $"Calling Background Worker setup" );
            worker . WorkerSupportsCancellation = true;
            worker . WorkerReportsProgress = true;
            worker . ProgressChanged += LbWorker_ProgressChanged;
            worker . RunWorkerCompleted += LbWorker_RunWorkerCompleted;
            worker . DoWork += LbWorker_DoWork;
            Debug . WriteLine ( $"Running Background Worker" );
            worker . ReportProgress ( progress , lbuserctrl );
            worker . RunWorkerAsync ( lbuserctrl );
            return lbuserctrl;
        }
        private void LbWorker_DoWork ( object sender , DoWorkEventArgs e ) {
            Debug . WriteLine ( $"Calling ListBox Background Worker ProgressChanged method" );
            BackgroundWorker worker = sender as BackgroundWorker;
            // How  to pass more than just percent  done by using e.Argument (object) which can be anyhing you want to unpack in progresschanged()
            worker . ReportProgress ( 10 , e . Argument as LbUserControl );
            Debug . WriteLine ( $"Cancelling thread of ProgressChanged method" );
            worker . CancelAsync ( );
        }
        public void LbWorker_RunWorkerCompleted ( object sender , RunWorkerCompletedEventArgs e ) {
            BackgroundWorker worker = sender as BackgroundWorker;
            if ( e . Cancelled )
                Debug . WriteLine ( $"Worker Cancelled" );
            else
                Debug . WriteLine ( $"Worker completed" );
            Tview . ProgressBar_Progress . Value = 0;
            Tview . ProgressBar_Progress . UpdateLayout ( );
            Tview . ProgressBar_Progress . Value = 0;
            Tview . ProgressBar_Progress . UpdateLayout ( );
            Mouse . OverrideCursor = Cursors . Arrow;
            worker . CancelAsync ( );
        }
        private async void LbWorker_ProgressChanged ( object sender , ProgressChangedEventArgs e ) {
            long msecs = 0;
            int progress = e . ProgressPercentage;
            Tabview . Tabcntrl . lbUserctrl = e . UserState as LbUserControl;
            Tview . ProgressBar_Progress . Value = progress;
            BackgroundWorker worker = sender as BackgroundWorker;
            Stopwatch watch = new Stopwatch ( );
            Debug . WriteLine ( $"Running BACKGROUND for LbUserControl" );

            if ( Tabview . Tabcntrl . lbUserctrl . listbox1 . Items . Count == 0 ) {
                watch . Start ( );
                Debug . WriteLine ( $"Creating new LbUserControl in Background Worker ProgressChanged method" );
                if ( Tabview . Tabcntrl . lbUserctrl == null )
                    Tabview . Tabcntrl . lbUserctrl = new LbUserControl ( );

                watch . Stop ( );
                msecs = watch . ElapsedMilliseconds;
                Debug . WriteLine ( $"new LbUserControl took {msecs}" );
                ListBox lbx = Tabview . Tabcntrl . lbUserctrl . listbox1;
                ReduceByParamValue convert = new ReduceByParamValue ( );
                lbx . Height = Tabview . Tabcntrl . lbUserctrl . Height = Convert . ToDouble ( convert . Convert ( Tabcontrol . ActualHeight , typeof ( double ) , 40 , CultureInfo . CurrentCulture ) );
                lbx . Width = Tabview . Tabcntrl . lbUserctrl . Width = Convert . ToDouble ( convert . Convert ( Tabcontrol . ActualWidth , typeof ( double ) , 10 , CultureInfo . CurrentCulture ) );
                Tview . ProgressBar_Progress . Value = 35;
                Tview . ProgressBar_Progress . UpdateLayout ( );

                if ( Tabview . Tabcntrl . lbUserctrl . listbox1 . ItemsSource == null ) {
                    Debug . WriteLine ( $"Loading Db....." );
                    DataTemplate dt = Application . Current . FindResource ( "BankDataTemplate1" ) as DataTemplate;
                    Tabview . Tabcntrl . lbUserctrl . listbox1 . ItemTemplate = dt;
                    Thickness t1 = new Thickness ( );
                    t1 = new Thickness ( );
                    t1 . Left = 5;
                    t1 . Top = 10;
                    Tabview . Tabcntrl . lbUserctrl . Margin = t1;
                    Tview . ListboxTab . Content = Tabview . Tabcntrl . lbUserctrl;
                    Tabview . Tabcntrl . lbUserctrl . CurrentType = "BANK";
                    DbType = Tabview . Tabcntrl . lbUserctrl?.CurrentType;
                    //Tview . Dispatcher . Invoke ( DispatcherPriority . Normal , ( Action ) ( async ( ) =>
                    //{
                    await Task . Run ( ( ) => Tabview . Tabcntrl . lbUserctrl . LoadBank ( true ) );
                    Tview . ProgressBar_Progress . Value = 65;
                    Tview . ProgressBar_Progress . UpdateLayout ( );
                    //} ) );

                    if ( Tabview . Tabcntrl . lbUserctrl . Bvm . Count > 0 )
                        Tabview . Tabcntrl . lbUserctrl . listbox1 . ItemsSource = Tabview . Tabcntrl . lbUserctrl . Bvm;

                    Mouse . OverrideCursor = Cursors . Arrow;
                    Tabview . Tabcntrl . lbUserctrl . UpdateLayout ( );
                    Tview . ProgressBar_Progress . Value = 85;
                    Tview . ProgressBar_Progress . UpdateLayout ( );
                    Debug . WriteLine ( $"Data loading is in process" );
                }
                else {
                    Tabview . Tabcntrl . lbUserctrl . listbox1 . ScrollIntoView ( Tabview . Tabcntrl . lbUserctrl . listbox1 . SelectedItem );
                    Utils . ScrollLBRecordIntoView ( Tabview . Tabcntrl . lbUserctrl . listbox1 , Tabview . Tabcntrl . lbUserctrl . listbox1 . SelectedIndex );
                    Mouse . OverrideCursor = Cursors . Arrow;
                    watch . Stop ( );
                }
                Tview . ProgressBar_Progress . Value = 100;
                Tview . ProgressBar_Progress . UpdateLayout ( );
            }
            else
                Tabview . Tabcntrl . lbUserctrl = e . UserState as LbUserControl;

            watch . Start ( );
            Mouse . OverrideCursor = Cursors . Wait;
            Debug . WriteLine ( $"Loading Listbox in Background Worker ProgressChanged method" );
            Tabitem = Tabcontrol . Items [ 1 ] as TabItem;
            Thickness th = new Thickness ( );
            th . Left = 5;
            th . Top = 10;
            th . Right = 5;
            th . Bottom = 10;
            Tabview . Tabcntrl . lbUserctrl . Margin = th;
            Tabitem . Content = Tabview . Tabcntrl . lbUserctrl;
            ListBox lb = Tabview . Tabcntrl . lbUserctrl . listbox1;
            Tview . ProgressBar_Progress . Value = 45;
            Tview . ProgressBar_Progress . UpdateLayout ( );
            // How to call a Converter from c#
            ReduceByParamValue converter = new ReduceByParamValue ( );
            lb . Height = Tabview . Tabcntrl . lbUserctrl . Height = Convert . ToDouble ( converter . Convert ( Tabcontrol . ActualHeight , typeof ( double ) , 40 , CultureInfo . CurrentCulture ) );
            lb . Width = Tabview . Tabcntrl . lbUserctrl . Width = Convert . ToDouble ( converter . Convert ( Tabcontrol . ActualWidth , typeof ( double ) , 10 , CultureInfo . CurrentCulture ) );
            Tabview . Tabcntrl . lbUserctrl . Visibility = Visibility . Visible;
            Tabitem . IsSelected = true;
            Debug . WriteLine ( $"Listbox loaded by Background Worker " );

            Tabcontrol . SelectedIndex = 1;
            CurrentTabIndex = Tabcontrol . SelectedIndex;
            Tabview . Tabcntrl . lbUserctrl . listbox1 . Focus ( );
            DbType = Tabview . Tabcntrl . lbUserctrl?.CurrentType;
            Debug . WriteLine ( $"new LbUserControl took {watch . ElapsedMilliseconds}, Total = Create : {msecs} + Load : {watch . ElapsedMilliseconds} =  {msecs + watch . ElapsedMilliseconds}" );
            Tview . ProgressBar_Progress . Value = 100;
            Tview . ProgressBar_Progress . UpdateLayout ( );
            Mouse . OverrideCursor = Cursors . Arrow;

            // This creates a new ListBox entirely in C# and displays it on the L
            //however, the Style set in generator does NOT work correctly
            if ( SHOWCLBOX ) {
                lb = Tabview . Tabcntrl . lbUserctrl . CreateListBox ( );
                lb . Items . Add ( "aaaaaaaaaaaaaaaaaaaa" );
                lb . Items . Add ( "bbbbbbbbbbbbbbbbbbbb" );
                lb . Items . Add ( "cccccccccccccccccccc" );
                lb . Items . Add ( "ddddddddddddddddddddddddddd" );
                lb . Items . Add ( "eeeeeeeeeeeeeeeeeeeeee" );
                lb . Items . Add ( "fffffffffffffffffffffffffffffffff" );
                lb . Items . Add ( "gggggggggggggggggggggggggggggggg" );
                lb . Items . Add ( "hhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh" );
                lb . Items . Add ( "iiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiii" );
                lb . Items . Add ( "jjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjj" );
                lb . Items . Add ( "mmmmmmmmmmmmmmmmmmmmmmmmmmm" );
                Tview . ListboxTab . Content = lb;
            }
        }

        public bool SHOWCLBOX = false;

        #endregion ListBox BackgrundWorker Loading

        //*************************************************
        #region ListView BackgrundWorker Loading
        public LvUserControl LoadListviewInBackgroundTask ( LvUserControl lvuserctrl ) {
            BackgroundWorker worker = new BackgroundWorker ( );
            Debug . WriteLine ( $"Calling Background Worker setup" );
            worker . WorkerSupportsCancellation = true;
            worker . WorkerReportsProgress = true;
            worker . ProgressChanged += LvWorker_ProgressChanged;
            worker . RunWorkerCompleted += LvWorker_RunWorkerCompleted;
            worker . DoWork += LvWorker_DoWork;
            Debug . WriteLine ( $"Running Background Worker" );
            ProgressValue = 15;
            Tview . ProgressBar_Progress . UpdateLayout ( );
            worker . RunWorkerAsync ( lvuserctrl );
            return lvuserctrl;
        }
        private void LvWorker_DoWork ( object sender , DoWorkEventArgs e ) {
            Debug . WriteLine ( $"Calling ListView ackground Worker ProgressChanged method" );
            BackgroundWorker worker = sender as BackgroundWorker;
            worker . ReportProgress ( 10 , e . Argument as LvUserControl );
            Debug . WriteLine ( $"Returned from Background Worker ProgressChanged method" );
            Debug . WriteLine ( $"Cancelling thread of ProgressChanged method" );
            worker . CancelAsync ( );
        }
        public void LvWorker_RunWorkerCompleted ( object sender , RunWorkerCompletedEventArgs e ) {
            BackgroundWorker worker = sender as BackgroundWorker;
            if ( e . Cancelled )
                Debug . WriteLine ( $"Worker Cancelled" );
            else
                Debug . WriteLine ( $"Worker completed" );
            Tview . ProgressBar_Progress . Value = 0;
            Tview . ProgressBar_Progress . UpdateLayout ( );
            Tview . ProgressBar_Progress . Value = 0;
            Tview . ProgressBar_Progress . UpdateLayout ( );
            Mouse . OverrideCursor = Cursors . Arrow;
            worker . CancelAsync ( );
        }
        private async void LvWorker_ProgressChanged ( object sender , ProgressChangedEventArgs e ) {
            long msecs = 0;
            int progress = e . ProgressPercentage;
            Tview . ProgressBar_Progress . Value = progress;
            Tview . ProgressBar_Progress . UpdateLayout ( );
            Tabview . Tabcntrl . lvUserctrl = e . UserState as LvUserControl;
            BackgroundWorker worker = sender as BackgroundWorker;
            Stopwatch watch = new Stopwatch ( );
            Mouse . OverrideCursor = Cursors . Wait;
            if ( Tabview . Tabcntrl . lvUserctrl == null || Tabview . Tabcntrl . lvUserctrl . listview1 . Items . Count == 0 ) {
            }
            if ( Tabview . Tabcntrl . lvUserctrl == null ) {
                watch . Start ( );
                Debug . WriteLine ( $"Creating new LvUserControl in Background Worker ProgressChanged method" );
                Tabview . Tabcntrl . lvUserctrl = new LvUserControl ( );
                watch . Stop ( );
                msecs = watch . ElapsedMilliseconds;
                Debug . WriteLine ( $"new LvUserControl ook {msecs}" );
            }
            if ( Tabview . Tabcntrl . lvUserctrl . listview1 . ItemsSource == null ) {
                Debug . WriteLine ( $"Loading Db....." );
                //                    DataTemplate dt = Application . Current . FindResource ( "CustomersDbTemplate1" ) as DataTemplate;
                DataTemplate dt = Tview . FindResource ( "BankDataTemplate1" ) as DataTemplate;
                Tabview . Tabcntrl . lvUserctrl . listview1 . ItemTemplate = dt;
                Thickness t2 = new Thickness ( );
                t2 . Left = 5;
                t2 . Top = 10;
                Tabview . Tabcntrl . lvUserctrl . Margin = t2;
                Tview . ListviewTab . Content = Tabview . Tabcntrl . lvUserctrl;
                Tabview . Tabcntrl . lvUserctrl . CurrentType = "BANK";
                DbType = Tabview . Tabcntrl . lvUserctrl?.CurrentType;
                Tview . ProgressBar_Progress . Value = 55;
                Tview . ProgressBar_Progress . UpdateLayout ( );

                //Data is Loaded via EventControl Mesage BankDataLoaded
                await Task . Run ( ( ) => Tabview . Tabcntrl . lvUserctrl . LoadBank ( ) );
                //if ( Tabview . Tabcntrl. lvUserctrl . Bvm . Count > 0 && Tabview . Tabcntrl. lvUserctrl . listview1 . Items . Count > 0 )
                //    Tabview . Tabcntrl. lvUserctrl . listview1 . ItemsSource = Tabview . Tabcntrl. lvUserctrl . Bvm;
                Mouse . OverrideCursor = Cursors . Arrow;
                Tabview . Tabcntrl . lvUserctrl . UpdateLayout ( );
                Tview . ProgressBar_Progress . Value = 75;
                Tview . ProgressBar_Progress . UpdateLayout ( );
            }
            Debug . WriteLine ( $"Loading Listview in Background Worker ProgressChanged method" );
            //            Debug. WriteLine ( $"ListView : {Tabview . Tabcntrl. lvUserctrl . GetHashCode ( )}" );
            Tabitem = Tabcontrol . Items [ 2 ] as TabItem;
            Thickness th = new Thickness ( );
            th . Left = 5;
            th . Top = 10;
            th . Right = 5;
            th . Bottom = 10;
            Tabview . Tabcntrl . lvUserctrl . Margin = th;
            Tabitem . Content = Tabview . Tabcntrl . lvUserctrl;
            ListView lv = Tabview . Tabcntrl . lvUserctrl . listview1;
            // How to call a Converter from c#
            ReduceByParamValue converter = new ReduceByParamValue ( );
            lv . Height = Tabview . Tabcntrl . lvUserctrl . Height = Convert . ToDouble ( converter . Convert ( Tabcontrol . ActualHeight , typeof ( double ) , 40 , CultureInfo . CurrentCulture ) );
            lv . Width = Tabview . Tabcntrl . lvUserctrl . Width = Convert . ToDouble ( converter . Convert ( Tabcontrol . ActualWidth , typeof ( double ) , 10 , CultureInfo . CurrentCulture ) );
            Tabview . Tabcntrl . lvUserctrl . Visibility = Visibility . Visible;
            Tabitem . IsSelected = true;
            Debug . WriteLine ( $"Listview loaded by Background Worker " );
            Tabcontrol . SelectedIndex = 2;
            CurrentTabIndex = Tabcontrol . SelectedIndex;
            Tview . ProgressBar_Progress . Value = 100;
            Tview . ProgressBar_Progress . UpdateLayout ( );
            //Debug . WriteLine ( $"Data loaded so showing tab.." );
            Tabview . Tabcntrl . lvUserctrl . listview1 . Focus ( );
            DbType = Tabview . Tabcntrl . lvUserctrl?.CurrentType;
            Tabview . Tabcntrl . lvUserctrl . listview1 . ScrollIntoView ( Tabview . Tabcntrl . lvUserctrl . listview1 . SelectedItem );
            Utils . ScrollLBRecordIntoView ( Tabview . Tabcntrl . lvUserctrl . listview1 , Tabview . Tabcntrl . lvUserctrl . listview1 . SelectedIndex );
            Debug . WriteLine ( $"ListView : {Tabview . Tabcntrl . lvUserctrl . GetHashCode ( )}" );
            Tview . ProgressBar_Progress . Value = 100;
            Tview . ProgressBar_Progress . UpdateLayout ( );
        }
        #endregion Listview BackgrundWorker Loading

        #endregion BACKGROUND THREADS

        private void UpdateInfopanel ( SelectionChangedArgs e ) {
            if ( e . data == null )
                return;
            BankAccountViewModel bv = new BankAccountViewModel ( );
            CustomerViewModel cv = new CustomerViewModel ( );
            if ( e . sendername == "BANK" ) {
                bv = e . data as BankAccountViewModel;
                if ( bv != null )
                    if ( Tview != null ) Tview . InfoPanel . Text = $"{bv . CustNo} : {bv . BankNo},  {bv . Balance}";
                DbType = "BANK";
            }
            else {
                cv = e . data as CustomerViewModel;
                if ( cv != null )
                    if ( Tview != null ) Tview . InfoPanel . Text = $"{cv . CustNo} : {cv . BankNo},  {cv . FName}";
                DbType = "CUSTOMER";
            }
        }
        private void FindMatch ( string Custno , string Bankno , string type , object data ) {
            int index = 0;
            if ( type == "LB" ) {
                if ( Tabview . Tabcntrl . lbUserctrl == null ) return;
                if ( Tabview . Tabcntrl . lbUserctrl . listbox1 . ItemsSource == null ) return;
                if ( Tabview . Tabcntrl . lbUserctrl . CurrentType == "BANK" ) {

                    try {
                        foreach ( BankAccountViewModel item in Tabview . Tabcntrl . lbUserctrl . listbox1 . Items ) {
                            if ( item . CustNo == Custno && item . BankNo == Bankno ) {
                                IsLoadingDb = true;
                                Tabview . Tabcntrl . lbUserctrl . listbox1 . SelectedIndex = index;
                                Utils . ScrollLBRecordIntoView ( Tabview . Tabcntrl . lbUserctrl . listbox1 , Tabview . Tabcntrl . lbUserctrl . listbox1 . SelectedIndex );
                                IsLoadingDb = false;
                                break;
                            }
                            index++;
                        }
                    }
                    catch ( Exception ex ) { Debug . WriteLine ( $"ListBox Matching failed {ex . Message}, [{ex . Data}]" ); }
                }
                else {
                    try {
                        foreach ( CustomerViewModel item in Tabview . Tabcntrl . lbUserctrl . listbox1 . Items ) {
                            if ( item . CustNo == Custno && item . BankNo == Bankno ) {
                                IsLoadingDb = true;
                                Tabview . Tabcntrl . lbUserctrl . listbox1 . SelectedIndex = index;
                                Utils . ScrollLBRecordIntoView ( Tabview . Tabcntrl . lbUserctrl . listbox1 , Tabview . Tabcntrl . lbUserctrl . listbox1 . SelectedIndex );
                                IsLoadingDb = false;
                                break;
                            }
                            index++;
                        }
                    }
                    catch ( Exception ex ) { Debug . WriteLine ( $"ListBox Matching failed {ex . Message}, [{ex . Data}]" ); }
                }
            }
            else if ( type == "LV" ) {
                //              if ( Tabview . Tabcntrl. lvUserctrl . listview1 . SelectedItem == null ) return;
                //bv = Tabview . Tabcntrl. lvUserctrl . listview1 . SelectedItem as BankAccountViewModel;
                //Custno = bv . CustNo;
                //Bankno = bv . BankNo;
                if ( Tabview . Tabcntrl . lvUserctrl == null ) return;
                if ( Tabview . Tabcntrl . lvUserctrl . listview1 . ItemsSource == null ) return;
                if ( Tabview . Tabcntrl . lvUserctrl . CurrentType == "BANK" ) {
                    try {
                        foreach ( BankAccountViewModel item in Tabview . Tabcntrl . lvUserctrl . listview1 . Items ) {
                            if ( item . CustNo == Custno && item . BankNo == Bankno ) {
                                IsLoadingDb = true;
                                Tabview . Tabcntrl . lvUserctrl . listview1 . SelectedIndex = index;
                                Utils . ScrollLVRecordIntoView ( Tabview . Tabcntrl . lvUserctrl . listview1 , Tabview . Tabcntrl . lvUserctrl . listview1 . SelectedIndex );
                                IsLoadingDb = false;
                                break;
                            }
                            index++;
                        }
                    }
                    catch ( Exception ex ) { Debug . WriteLine ( $"ListView Matching failed {ex . Message}, [{ex . Data}]" ); }
                }
                else {
                    try {
                        foreach ( CustomerViewModel item in Tabview . Tabcntrl . lvUserctrl . listview1 . Items ) {
                            if ( item . CustNo == Custno && item . BankNo == Bankno ) {
                                IsLoadingDb = true;
                                Tabview . Tabcntrl . lvUserctrl . listview1 . SelectedIndex = index;
                                Utils . ScrollLVRecordIntoView ( Tabview . Tabcntrl . lvUserctrl . listview1 , Tabview . Tabcntrl . lvUserctrl . listview1 . SelectedIndex );
                                IsLoadingDb = false;
                                break;
                            }
                            index++;
                        }
                    }
                    catch ( Exception ex ) { Debug . WriteLine ( $"ListView Matching failed {ex . Message}, [{ex . Data}]" ); }
                }
            }
            else if ( type == "DG" ) {
                //                if ( Tabview . Tabcntrl. dgUserctrl . grid1 . SelectedItem == null ) return;
                if ( Tabview . Tabcntrl . dgUserctrl == null ) return;
                if ( Tabview . Tabcntrl . dgUserctrl . grid1 . ItemsSource == null ) return;
                if ( Tabview . Tabcntrl . dgUserctrl . CurrentType == "BANK" ) {
                    try {
                        foreach ( BankAccountViewModel item in Tabview . Tabcntrl . dgUserctrl . grid1 . Items ) {
                            if ( item . CustNo == Custno && item . BankNo == Bankno ) {
                                IsLoadingDb = true;
                                Tabview . Tabcntrl . dgUserctrl . grid1 . SelectedIndex = index;
                                Tabview . Tabcntrl . dgUserctrl . grid1 . SelectedItem = index;
                                Utils . ScrollRowInGrid ( Tabview . Tabcntrl . dgUserctrl . grid1 , index );
                                IsLoadingDb = false;
                                break;
                            }
                            index++;
                        }
                    }
                    catch ( Exception ex ) { Debug . WriteLine ( $"DataGrid Matching failed {ex . Message}, [{ex . Data}]" ); }
                }
                else {
                    // crashes here !!!!
                    try {
                        foreach ( CustomerViewModel item in Tabview . Tabcntrl . dgUserctrl . grid1 . Items ) {
                            if ( item . CustNo == Custno && item . BankNo == Bankno ) {
                                IsLoadingDb = true;
                                Tabview . Tabcntrl . dgUserctrl . grid1 . SelectedIndex = index;
                                Tabview . Tabcntrl . dgUserctrl . grid1 . SelectedItem = index;
                                Utils . ScrollRowInGrid ( Tabview . Tabcntrl . dgUserctrl . grid1 , Tabview . Tabcntrl . dgUserctrl . grid1 . SelectedIndex );
                                IsLoadingDb = false;
                                break;
                            }
                            index++;
                        }
                    }
                    catch ( Exception ex ) { Debug . WriteLine ( $"DataGrid Matching failed {ex . Message}, [{ex . Data}]" ); }
                }
            }
        }
        public async void wpfasyncTask ( ) {
            // Cannot access  UI thread
            Debug . WriteLine ( "Hellloo World" );

            // This secton will run immediately & not block UI thread
            await Task . Run ( async ( ) => {
                Debug . WriteLine ( "Starting task " );
                Debug . WriteLine ( $"Now on Thread {Thread . CurrentThread . ManagedThreadId}" );
                for ( int i = 0 ; i < 100 ; i++ ) {
                    Debug . WriteLine ( "looping" );
                    Thread . Sleep ( 100 );
                }
                Debug . WriteLine ( "Task ended " );
            } );
        }
        public static void asynctask ( ) {
            Debug . WriteLine ( "Hellloo World" );
            Debug . WriteLine ( $"Now on Thread {Thread . CurrentThread . ManagedThreadId}" );

            // This secton will run immediately & not block UI thread
            // Cannot await it cos  methid is   not async - (see below)
            Task . Run ( async ( ) => {
                Debug . WriteLine ( $"Now on Thread {Thread . CurrentThread . ManagedThreadId}" );
                await Task . Delay ( 100 );
                Debug . WriteLine ( "Starting task " );
                for ( int i = 0 ; i < 1000 ; i++ ) {
                    Debug . WriteLine ( $"looping {i}" );
                    Thread . Sleep ( 100 );
                }
            } );
            // This will NOT run unti task above has finshed
            Debug . WriteLine ( "Task ended " );

        }
        public static async void fullasynctask ( ) {
            Debug . WriteLine ( "Hellloo World" );

            // This secton will run immediately & not block UI thread
            // this WILL await cos  method itself is async - (see above)
            await Task . Run ( async ( ) => {
                Debug . WriteLine ( $"Now on Thread {Thread . CurrentThread . ManagedThreadId}" );
                await Task . Delay ( 100 );
                Debug . WriteLine ( "Starting task " );
                for ( int i = 0 ; i < 1000 ; i++ ) {
                    Debug . WriteLine ( "looping" );
                    Thread . Sleep ( 100 );
                }
            } );
            // This will NOT run unti task above has finshed
            Debug . WriteLine ( "Task ended " );

        }
        public void wpfasyncthread ( ) {
            // this works
            button . Content = "Helllo World";

            // This secton will run immediately & not block UI thread
            new Thread ( ( ) => {
                Debug . WriteLine ( $"Now on Thread {Thread . CurrentThread . ManagedThreadId}" );
                //This will Fail
                //button . Content = "Starting task ";
                for ( int i = 0 ; i < 100 ; i++ ) {
                    // This will fail !
                    //          button . Content = "Starting task ";
                    Debug . WriteLine ( $"looping {i}" );
                    Thread . Sleep ( 100 );
                    //This will work
                    button . Dispatcher . Invoke ( ( ) => {
                        button . Content = "Starting task ";
                    } );
                }
                Debug . WriteLine ( "Task ended " );
            } ) . Start ( );

        }
        public async Task<bool> dolooping ( ) {
            for ( int i = 0 ; i < 100 ; i++ ) {
                Debug . WriteLine ( "looping" );
                Thread . Sleep ( 100 );

                // this WILL fail without Dispatcher
                //button . Content = "dfdfdsg";
                // This will WORK & access button.Content
                button . Dispatcher . Invoke ( ( ) => {
                    Debug . WriteLine ( $"Setting Button in Thread {Thread . CurrentThread . ManagedThreadId}" );
                    button . Content = "dfdfdsg";
                } );

                // This will WORK & access
                // button.Content on UI thread from here.....
                Application . Current . Dispatcher . Invoke ( ( ) => {
                    Debug . WriteLine ( $"Now on Thread {Thread . CurrentThread . ManagedThreadId}" );
                    button . Content = "dfdfdsg";
                } );
            }
            return true;
        }
        //not    used
        private async Task<bool> LoadListviewData ( ) {
            bool success = true;
            //    await Task . Run ( ( ) =>
            //    {
            //        if ( Tabview . Tabcntrl. lvUserctrl == null )
            //        {
            //            Tabview . Tabcntrl. lvUserctrl = new LvUserControl ( );
            //            Debug. WriteLine ( $"Loading Db....." );
            //            //                    DataTemplate dt = Application . Current . FindResource ( "CustomersDbTemplate1" ) as DataTemplate;
            //            DataTemplate dt = Application . Current . FindResource ( "BankDataTemplate1" ) as DataTemplate;
            //            Tabview . Tabcntrl. lvUserctrl . listview1 . ItemTemplate = dt;
            //            Thickness th = new Thickness ( );
            //            th = new Thickness ( );
            //            th . Left = 5;
            //            th . Top = 10;
            //            Tabview . Tabcntrl. lvUserctrl . Margin = th;
            //            Tview . ListviewTab . Content = Tabview . Tabcntrl. lvUserctrl;
            //            //Tabview . Tabcntrl. lbUserctrl . UpdateLayout ( );
            //            Tabview . Tabcntrl. lvUserctrl . CurrentType = "BANK";
            //            DbType = Tabview . Tabcntrl. lvUserctrl?.CurrentType;
            //            Tabview . Tabcntrl. lvUserctrl . LoadBank ( true );
            //            Mouse . OverrideCursor = Cursors . Arrow;
            //            Tabview . Tabcntrl. lvUserctrl . UpdateLayout ( );
            //        }
            //    } );
            return success;
        }

        #endregion NOT USED
    }

}
