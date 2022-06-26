using System;
using System . Collections;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Data;
using System . Diagnostics;
using System . Drawing . Printing;
using System . Globalization;
using System . Linq;
using System . Reflection;
using System . Security . Policy;
using System . Text;
using System . Threading;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Threading;
using System . Xml . Linq;


using NewWpfDev . Converts;
using NewWpfDev . UserControls;
using NewWpfDev . Views;



using static NewWpfDev . ViewModels . TabWinViewModel;
namespace NewWpfDev . ViewModels
{
    public class TabWinViewModel : INotifyPropertyChanged
    {
        #region Events
        public static event EventHandler<LoadDbArgs> LoadDb;
        public static event EventHandler<LinkChangedArgs> ViewersLinked;
        public static event EventHandler<DbCountArgs> SetDBCount;
        public static event EventHandler<DbTypeArgs> SetDBType;
        //public static event EventHandler<EventArgs> GridGotFocus;

        public static void OnSetDbType ()
        {
            DbTypeArgs args = new();
            if ( SetDBType != null )
                SetDBType?.Invoke(null , args);
        }
        protected virtual void OnViewersLinked (int index , object item , UIElement ctrl)
        {
            LinkChangedArgs args = new LinkChangedArgs();
            if ( ViewersLinked != null )
                ViewersLinked.Invoke(this , args);
        }
        protected virtual void OnLoadDb (string name)
        {
            LoadDbArgs args = new LoadDbArgs();
            args . dbname = name;

            if ( LoadDb != null )
                LoadDb.Invoke(this , args);
        }
        private void Tabview_SetDBType (object sender , DbTypeArgs e)
        {
            //            Debug. WriteLine ( $"{e . Dbname}" );
            DbType = e . Dbname;
        }
        public static void TriggerDbType (string dbname)
        {
            if ( SetDBType != null )
            {
                DbTypeArgs args = new DbTypeArgs();
                args . Dbname = dbname;
                SetDBType . Invoke(null , args);
            }
        }

        #endregion Events

        public Button button = new Button();
        private static Stopwatch timer = new Stopwatch();
        private bool SelectionInAction { get; set; } = false;
        static public IProgress<int> progress
        {
            get; set;
        }

        public bool USETASK { get; set; } = true;

        #region OnPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged (string propertyName)
        {
            if ( PropertyChanged != null )
            {
                PropertyChanged(this , new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion OnPropertyChanged

        public LbUserControl lbuser;
        public LvUserControl lvuser;
        public TvUserControl tvuser;
        public struct TabData
        {
            public LbUserControl lbctrl;
            public LvUserControl lvctrl;
            public TvUserControl tvctrl;
            public LogUserControl logctrl;
        }
        public Stopwatch watch = new Stopwatch();
        public object Viewmodel
        {
            get; set;
        }

        #region Properties

        public static TabWinViewModel ControllerVm
        {
            get; set;
        }
        public static TabControl Tabcontrol
        {
            get; set;
        }
        public static Tabview Tview
        {
            get; set;
        }

        public static string CurrentTabTextBlock
        {
            get; set;
        }
        public static string CurrentTabName
        {
            get; set;
        }
        public static int CurrentTabIndex
        {
            get; set;
        }
        public static DgUserControl dgUserctrl
        {
            get; set;
        }
        public static LbUserControl lbUserctrl
        {
            get; set;
        }
        public static LvUserControl lvUserctrl
        {
            get; set;
        }
        public static LogUserControl logUserctrl
        {
            get; set;
        }
        public static TvUserControl tvUserctrl
        {
            get; set;
        }
        public static Window Win
        {
            get; set;
        }
        private static TabWinViewModel ThisWin
        {
            get; set;
        }
        private static object TabContentObject
        {
            get; set;
        }
        public static bool IsLoadingDb { get; set; } = true;

        #endregion Poperties

        #region Full properties

        private DataGrid dgrid;
        private string dbCount;
        //        private bool areViewersLinked = false;
        private int progressValue;

        private TabItem tabitem;
        public TabItem Tabitem
        {
            get {return tabitem;}
            set {tabitem = value; NotifyPropertyChanged(nameof(Tabitem));}
        }
        public DataGrid dGrid
        {
            get
            {
                return dgrid;
            }
            set
            {
                dgrid = value; NotifyPropertyChanged(nameof(dGrid));
            }
        }
        public string DbCount
        {
            get
            {
                return dbCount;
            }
            set
            {
                dbCount = value;
            }
        }
        public int ProgressValue
        {
            get
            {
                return progressValue;
            }
            set
            {
                progressValue = value; NotifyPropertyChanged("ProgressValue");
            }
        }
        #endregion Full properties


        #region Commands
        public ICommand LoadBankBtn
        {
            get;
        }
        public ICommand LoadCustBtn
        {
            get;
        }
        public ICommand CloseAppBtn
        {
            get;
        }
        public ICommand CloseWinBtn
        {
            get;
        }
        public ICommand ShowInfo
        {
            get;
        }
        private string dbType;
        public string DbType
        {
            get { return dbType; }
            set { dbType = value; NotifyPropertyChanged(nameof(DbType));  }
        }
        #endregion Commands

        public TabWinViewModel ()
        {
            if ( ThisWin == null )
            {
                ThisWin = this;
                LoadBankBtn = new RelayCommand(LoadBankBtnExecute , LoadBankBtnCanExecute);
                LoadCustBtn = new RelayCommand(LoadCustBtnExecute , LoadCustBtnCanExecute);
                CloseAppBtn = new RelayCommand(CloseAppExecute , CloseAppCanExecute);
                CloseWinBtn = new RelayCommand(CloseWinExecute , CloseWinCanExecute);
                ShowInfo = new RelayCommand(ShowinfoExecute , ShowinfoCanExecute);
                // Give a pointer for 'this' to our usercontrols and get theirs back
                SetDBCount += Tabview_SetDBCount;
                SetDBType += Tabview_SetDBType;
                //LvUserControl.GetCurrentDbType += TabWinViewModel_GetCurrentDbType; 

                // set local public static pointer  to Tabview
                Tview = Tabview . GetTabview();

                if ( dgUserctrl == null )
                {
                    dgUserctrl = Tabview . GetTabview() . Dgusercontrol;
                    if ( dgUserctrl == null )
                    {
                        dgUserctrl = new DgUserControl();
                        Tview . Dgusercontrol = dgUserctrl;
                    }
                }
                lbUserctrl = Tabview . GetTabview() . Lbusercontrol;
                if ( lbUserctrl == null )
                {
                    lbUserctrl = new LbUserControl();
                    Tview . Lbusercontrol = lbUserctrl;
                }
                lvUserctrl = Tabview . GetTabview() . Lvusercontrol;
                if ( lvUserctrl == null )
                {
                    lvUserctrl = new LvUserControl();
                    Tview . Lvusercontrol = lvUserctrl;
                }
                if ( tvUserctrl == null )
                    tvUserctrl = new TvUserControl();
                if ( logUserctrl == null )
                    logUserctrl = new LogUserControl();

                //dgUserctrl = DgUserControl . SetController ( this );
                //lbUserctrl = LbUserControl . SetController ( this );
                //lvUserctrl = LvUserControl . SetController ( this );
                //tvUserctrl = TvUserControl . SetController ( this );
                //logUserctrl = LogUserControl . SetController ( this );

                // Save to our ViewModel repository
                Viewmodel = new ViewModel();
                Viewmodel = this;
                ViewModel . SaveViewmodel("TabWinViewModel" , Viewmodel);

                progress = new Progress<int>(count => ProgressValue = 0);
                Debug. WriteLine($"ctrls = {lbUserctrl?.listbox1?.Items . Count} ,  {lvUserctrl?.listview1?.Items . Count}");
            }
        }
   
        private void SetFocusToGrid (object sender , EventArgs e)
        {
            //DgUserControl . OnGridGotFocus ( );
        }

        private bool ShowinfoCanExecute (object arg)
        {
            return true;
        }

        private void ShowinfoExecute (object obj)
        {
            try
            {
                TabViewInfo tvvi = new TabViewInfo();
                tvvi . Show();
            }
            catch ( Exception ex ) { Debug. WriteLine($"Error loading Text file from disk : {ex . Message}"); }
        }

        public void Tabview_SetDBCount (object sender , DbCountArgs e)
        {
            DbCount = e . Dbcount . ToString();
        }
        public static void TriggerBankDbCount (object obj , DbCountArgs e)
        {
            if ( SetDBCount != null )
                SetDBCount?.Invoke(obj , e);
        }

        #region Button Handlers   for loading data
        public void LoadCustBtnExecute (object obj)
        {
            // Generic Load of Customer data for any control type
            if ( CurrentTabName == "DgridTab" )
            {
                dgUserctrl . Cvm?.Clear();
                dgUserctrl . CurrentType = "CUSTOMER";
                dgUserctrl . LoadCustomer();
                dgUserctrl . grid1 . ItemsSource = dgUserctrl . Cvm;
                dgUserctrl . grid1 . CellStyle = Application . Current . FindResource("MAINCustomerGridStyle") as Style;
                DataTemplate dt = Application . Current . FindResource("CustomersDbTemplate1") as DataTemplate;
                dgUserctrl . grid1 . ItemTemplate = dt;
                IsLoadingDb = true;
                dgUserctrl . grid1 . SelectedIndex = 0;
                dgUserctrl . grid1 . SelectedItem = 0;
                IsLoadingDb = false;
            }
            else if ( CurrentTabName == "ListboxTab" )
            {
                lbUserctrl . Cvm?.Clear();
                lbUserctrl . CurrentType = "CUSTOMER";
                lbUserctrl . LoadCustomer(true);
                lbUserctrl . listbox1 . ItemsSource = lbUserctrl . Cvm;
                DataTemplate dt = Application . Current . FindResource("CustomersDbTemplate1") as DataTemplate;
                lbUserctrl . listbox1 . ItemTemplate = dt;
                IsLoadingDb = true;
                lbUserctrl . listbox1 . SelectedIndex = 0;
                lbUserctrl . listbox1 . SelectedItem = 0;
                IsLoadingDb = false;
            }
            else if ( CurrentTabName == "ListviewTab" )
            {
                lvUserctrl . Cvm?.Clear();
                lvUserctrl . CurrentType = "CUSTOMER";
                lvUserctrl . LoadCustomer();
                lvUserctrl . listview1 . ItemsSource = lvUserctrl . Cvm;
                DataTemplate dt = Application . Current . FindResource("CustomersDbTemplate1") as DataTemplate;
                lvUserctrl . listview1 . ItemTemplate = dt;
                IsLoadingDb = true;
                lvUserctrl . listview1 . SelectedIndex = 0;
                lvUserctrl . listview1 . SelectedItem = 0;
                IsLoadingDb = false;
            }
            DbType = "CUSTOMER";
        }
        public void LoadBankBtnExecute (object obj)
        {
            // Generic Load of Bank data for any control type
            if ( CurrentTabName == "DgridTab" )
            {
                dgUserctrl . Bvm?.Clear();
                dgUserctrl . CurrentType = "BANK";
                dgUserctrl . LoadBank();
            }
            else if ( CurrentTabName == "ListboxTab" )
            {
                lbUserctrl . Bvm?.Clear();
                lbUserctrl . CurrentType = "BANK";
                lbUserctrl . LoadBank(true);
            }
            else if ( CurrentTabName == "ListviewTab" )
            {
                lvUserctrl . Bvm?.Clear();
                lvUserctrl . CurrentType = "BANK";
                lvUserctrl . LoadBank();
            }
            DbType = "BANK";
        }
        #endregion Button Handlers   for loading data

        #region CanExecute handlers
        private bool CloseAppCanExecute (object arg)
        {
            return true;
        }
        private bool CloseWinCanExecute (object arg)
        {
            return true;
        }
        private bool LoadCustBtnCanExecute (object arg)
        {
            //            if ( Cvm . Count > 0 ) return false;
            return true;
        }
        private bool LoadBankBtnCanExecute (object arg)
        {
            //           if ( Bvm . Count > 0 ) return false;
            return true;
        }

        #endregion CanExecute

        public static TabWinViewModel SetPointer (Tabview tview , string tabname)
        {
            // Get pointer to our view
            Tview = tview;
            Tabcontrol = Tview?.Tabctrl;
            //dgUserctrl = DgUserControl . SetController ( ThisWin );
            //lbUserctrl = LbUserControl . SetController ( ThisWin );
            lvUserctrl = LvUserControl . SetController(ThisWin);
            tvUserctrl = TvUserControl . SetController(ThisWin);
            CurrentTabIndex = 0;
            return ThisWin;
        }
        public static Tabview SendTabview ()
        {
            if ( Tview != null )
                return Tview;
            return null;
        }

        public async Task SetCurrentTab (Tabview tabview , string tab)
        {
            // Working well 2/6/22
            ProgressValue = 0;
            Tview . ProgressBar_Progress . UpdateLayout();
            //List<Dictionary<string, object>> obj =  ViewModel . GetAllViewModels ( );
            CurrentTabName = tab;
            if ( tab == "DgridTab" )
            {
                ProgressValue = 15;
                Tview . ProgressBar_Progress . UpdateLayout();

                Tview . LoadName . Text = "Loading Data Grid ...";
                if ( IsLoadingDb == true && CurrentTabName != tab ) return;
                IsLoadingDb = true;

                // setup the current tab Id
                Tabcontrol . SelectedIndex = 0;
                CurrentTabIndex = Tabcontrol . SelectedIndex;
                Debug. WriteLine($"Setting DataGrid as Active tab");
                SendWindowMessage($"Setting Datagrid as Active tab");
                CurrentTabTextBlock = "Tab1Header";
                TabItem TItem = Tabcontrol . Items [ 0 ] as TabItem;
                TItem . Content = dgUserctrl;
                ProgressValue = 25;
                Tview . ProgressBar_Progress . UpdateLayout();

                // Handle UserControl
                if ( dgUserctrl == null )
                {
                    dgUserctrl = ViewModel . GetViewmodel("DgUserControl") as DgUserControl;
                    if ( dgUserctrl == null )
                    {
                        dgUserctrl = new DgUserControl();
                        ViewModel . SaveViewmodel("DgUserControl" , dgUserctrl);
                    }
                    TItem . Content = dgUserctrl;
                    //dgUserctrl . DataContext = dgUserctrl;
                    ProgressValue = 55;
                    Tview . ProgressBar_Progress . UpdateLayout();
                }
                else
                {
                    DgUserControl test = ViewModel . GetViewmodel("DgUserControl") as DgUserControl;
                    if ( test == null )
                    {
                        ViewModel . SaveViewmodel("DgUserControl" , dgUserctrl);
                        test = ViewModel . GetViewmodel("DgUserControl") as DgUserControl;
                        if ( test == null )
                            test = new DgUserControl();
                        // reset to our saved control
                        dgUserctrl = test;
                        TItem . Content = test;
                        dgUserctrl . DataContext = test;
                        ProgressValue = 55;
                        Tview . ProgressBar_Progress . UpdateLayout();
                        Utils . ScrollRecordIntoView(dgUserctrl . grid1 , dgUserctrl . grid1 . SelectedIndex);
                    }
                    else
                    {
                        dgUserctrl = test;
                        TItem . Content = test;
                    }
                }
                if ( dgUserctrl . grid1 . Items . Count == 0 )
                {
                    if ( USETASK )
                    {
                        LoadDataGridAsync(dgUserctrl);
                    }
                    else
                    {
                        dgUserctrl = LoadDatagridInBackgroundTask(dgUserctrl);
                    }
                }

                ProgressValue = 100;
                Tview . ProgressBar_Progress . UpdateLayout();
                // clear all Tags to false;
                ClearTags();
                dgUserctrl . Tag = true;
                //                this.SelectionInAction = Tabview . GetTabview ( ) . ViewersLinked;
                //dgUserctrl . SelectionInAction = this . SelectionInAction;
                //Always do this close to end of method
                TItem . Content = dgUserctrl;
                //Tview . DgridTab . Content = dgUserctrl;

                // update Db Count for field on Tabview
                GotFocusArgs args = new GotFocusArgs();
                args . UseTask = USETASK;
                args . sender = this;
                args . caller = "TabWinViewModel";
                //allow other tabs  to load again
                IsLoadingDb = false;
                DbType = dgUserctrl?.CurrentType;

                //Tidy up screen
                Tview . LoadName . Text = "";
                ProgressValue = 0;
                Tview . ProgressBar_Progress . UpdateLayout();
                Utils . ScrollRecordIntoView(dgUserctrl . grid1 , dgUserctrl . grid1 . SelectedIndex);

            }
            else if ( tab == "ListboxTab" )
            {
                if ( IsLoadingDb == true && lbUserctrl != null ) return;
                IsLoadingDb = true;
                //CurrentTabName = tab;
                Tview . LoadName . Text = "List Box Loading";

                Mouse . OverrideCursor = Cursors . Wait;
                Debug. WriteLine($"Setting Listbox as Active tab");
                SendWindowMessage($"Setting Listbox as Active tab");
                Tabcontrol . SelectedIndex = 1;
                TabItem TItem = Tabcontrol . Items [ 1 ] as TabItem;
                CurrentTabIndex = Tabcontrol . SelectedIndex;

                if ( lbUserctrl == null )
                {
                    lbUserctrl = ViewModel . GetViewmodel("LbUserControl") as LbUserControl;
                    if ( lbUserctrl == null )
                    {
                        lbUserctrl = new LbUserControl();
                        ViewModel . SaveViewmodel("LbUserControl" , lbUserctrl);
                    }
                    TItem . Content = lbUserctrl;
                    lbUserctrl . DataContext = lbUserctrl;
                }
                else
                {
                    LbUserControl test = ViewModel . GetViewmodel("LbUserControl") as LbUserControl;
                    if ( test == null )
                    {
                        // save it to container viewmodel
                        ViewModel . SaveViewmodel("LbUserControl" , lbUserctrl);
                        //read it back and select it
                        test = ViewModel . GetViewmodel("LbUserControl") as LbUserControl;
                        // reset to our saved control
                        lbUserctrl = test;
                        TItem . Content = lbUserctrl;
                        lbUserctrl . DataContext = test;
                        Utils . ScrollLBRecordIntoView(lbUserctrl . listbox1 , lbUserctrl . listbox1 . SelectedIndex);
                    }
                    else
                    {
                        lbUserctrl = test;
                        TItem . Content = lbUserctrl;
                    }
                    lbUserctrl . DataContext = lbUserctrl;
                }

                ProgressValue = 5;
                Tview . ProgressBar_Progress . UpdateLayout();
                //Tview . ListboxTab . Content = lbUserctrl;
                CurrentTabTextBlock = "Tab2Header";
                CurrentTabName = tab;
                LbUserControl . lbParent = "LBVIEW";

                if ( lbUserctrl . listbox1 . Items . Count == 0 )
                {
                    if ( USETASK )
                    {
                        watch . Reset();
                        watch . Start();
                        ProgressValue = 15;
                        Tview . ProgressBar_Progress . UpdateLayout();
                        Debug. WriteLine($"Calling LoadListBoxAsync , UI Thread = {Thread . CurrentThread . ManagedThreadId}");
                        Debug. WriteLine($"Calling LoadListBoxAsync , Now Thread = {Thread . CurrentThread . ManagedThreadId}");
                        if ( lbUserctrl == null ) lbUserctrl = new LbUserControl();

                        Task task = new Task(() =>
                        {
                            LoadListBoxAsync();
                        });
                        task . Start();
                        ProgressValue = 55;
                        Tview . ProgressBar_Progress . UpdateLayout();
                        //                        TabItem TItem = Tabcontrol . Items [ 1 ] as TabItem;
                        //                        TItem . Content = lbUserctrl;
                        TItem . IsSelected = true;
                        //ListBox lb = lbUserctrl . listbox1;
                        //ReduceByParamValue converter = new ReduceByParamValue ( );
                        //lb . Height = lbUserctrl . Height = Convert . ToDouble ( converter . Convert ( Tabcontrol . ActualHeight , typeof ( double ) , 40 , CultureInfo . CurrentCulture ) );
                        //lb . Width = lbUserctrl . Width = Convert . ToDouble ( converter . Convert ( Tabcontrol . ActualWidth , typeof ( double ) , 10 , CultureInfo . CurrentCulture ) );
                        ProgressValue = 85;
                        Tview . ProgressBar_Progress . UpdateLayout();
                        watch . Stop();
                        Debug. WriteLine($"Loading via Task took {watch . ElapsedMilliseconds} msecs....");
                        watch . Reset();

                        Debug. WriteLine($"Data loaded so showing tab..");
                        //Tabcontrol . SelectedIndex = 1;
                        //CurrentTabIndex = Tabcontrol . SelectedIndex;
                        lbUserctrl . listbox1 . Focus();
                        DbType = lbUserctrl?.CurrentType;
                        lbUserctrl . UpdateLayout();
                        ProgressValue = 0;
                        Tview . ProgressBar_Progress . UpdateLayout();
                        Mouse . OverrideCursor = Cursors . Arrow;
                        ProgressValue = 90;
                        Tview . ProgressBar_Progress . UpdateLayout();
                    }
                    else
                    {
                        watch . Start();
                        lbUserctrl = LoadListboxInBackgroundTask(lbUserctrl);
                        watch . Stop();
                        Debug. WriteLine($"Loading via Background process took {watch . ElapsedMilliseconds} msecs....");
                        watch . Reset();
                    }
                    Debug. WriteLine("Loaded listbox - yeahhhhhhh........");
                    //                   EventControl . TriggerWindowMessage ( this , new InterWindowArgs { message = $"lbUserControl now Active..." , listbox = null } );
                }
                else
                {
                    //                    Tview . ListboxTab . Content = lbUserctrl;
                    TItem . IsSelected = true;
                    Debug. WriteLine("Already loaded....");
                    Mouse . OverrideCursor = Cursors . Arrow;
                }
                TItem . Content = lbUserctrl;
                DbType = lbUserctrl?.CurrentType;
                //                Tabcontrol . SelectedIndex = 1;
                CurrentTabIndex = Tabcontrol . SelectedIndex;
                //                Tview . ListboxTab . Opacity = 1.0;
                lbUserctrl?.UpdateLayout();
                //if ( lbUserctrl != null )
                //{
                //    ListBox lb = lbUserctrl . listbox1;
                //}
                Debug. WriteLine("Switched to  listbox - yeahhhhhhh........");
                //EventControl . TriggerWindowMessage ( this , new InterWindowArgs { message = $"lbUserControl now Active..." , listbox = null } );
                ClearTags();
                lbUserctrl . Tag = true;
                lbUserctrl . listbox1 . Focus();
                //lbUserctrl .
                //false;
                DbCountArgs cargs = new DbCountArgs();
                //                Tview . ListboxTab . Content = lbUserctrl;
                cargs . Dbcount = lbUserctrl . listbox1 . Items . Count;
                cargs . sender = "lbUserctrl";
                IsLoadingDb = false;
                TabWinViewModel . TriggerBankDbCount(this , cargs);
                Tview . LoadName . Text = "";
                ProgressValue = 0;
                Tview . ProgressBar_Progress . UpdateLayout();
                Utils . ScrollLBRecordIntoView(lbUserctrl . listbox1 , lbUserctrl . listbox1 . SelectedIndex);

            }
            else if ( tab == "ListviewTab" )
            {
                if ( IsLoadingDb == true && lvUserctrl != null ) return;
                IsLoadingDb = true;
                Debug. WriteLine($"Setting Listview as Active tab");
                //SendWindowMessage ( $"Setting Listview as Active tab" );
                Tabcontrol . SelectedIndex = 2;
                CurrentTabIndex = Tabcontrol . SelectedIndex;
                CurrentTabTextBlock = "Tab3Header";
                TabItem TItem = Tabcontrol . Items [ 2 ] as TabItem;

                Tview . LoadName . Text = "List View Loading";

                if ( lvUserctrl == null )
                {
                    lvUserctrl = ViewModel . GetViewmodel("LvUserControl") as LvUserControl;
                    if ( lvUserctrl == null )
                    {
                        lvUserctrl = new LvUserControl();
                        ViewModel . SaveViewmodel("LvUserControl" , lvUserctrl);
                    }
                    lvUserctrl . DataContext = lvUserctrl;
                }
                else
                {
                    LvUserControl test = ViewModel . GetViewmodel("LvUserControl") as LvUserControl;
                    if ( test == null )
                    {
                        // save it to container viewmodel
                        ViewModel . SaveViewmodel("LvUserControl" , lvUserctrl);
                        //read it back and select it
                        test = ViewModel . GetViewmodel("LvUserControl") as LvUserControl;
                        if ( test == null )
                        {
                            lvUserctrl = new LvUserControl();
                            ViewModel . SaveViewmodel("LvUserControl" , lvUserctrl);
                        }
                        else
                        {
                            // reset to our saved control
                            lvUserctrl = test;
                        }
                        TItem . Content = lvUserctrl;
                        Utils . ScrollLVRecordIntoView(lvUserctrl . listview1 , lvUserctrl . listview1 . SelectedIndex);
                    }
                    else
                    {
                        lvUserctrl = test;
                        TItem . Content = lvUserctrl;
                    }
                    lvUserctrl . DataContext = test;
                }

                LvUserControl . SetSelectionInAction(Tabview . GetTabview() . ViewersLinked);
                //lvUserctrl . SelectionInAction = false;

                if ( lvUserctrl . listview1 . Items . Count == 0 )
                {
                    ProgressValue = 15;
                    Tview . ProgressBar_Progress . UpdateLayout();

                    if ( USETASK )
                    {
                        Debug. WriteLine($"Calling LoadListBoxAsync , UI Thread = {Thread . CurrentThread . ManagedThreadId}");
                        //SendWindowMessage ( $"Calling LoadListBoxAsync , UI Thread = {Thread . CurrentThread . ManagedThreadId}" );
                        // We are already inside a Task !!!!
                        await Task . Run(async () =>
                        {
                            await LoadListViewAsync(lvUserctrl);
                        });

                        Debug. WriteLine($"RETURNED from Async Method");
                        // we do NOT have any data here as task is still running
                        //TabItem TItem = Tabcontrol . Items [ 2 ] as TabItem;
                        TItem . Content = lvUserctrl;
                        TItem . IsSelected = true;

                        //ListView lv = lvUserctrl . listview1;
                        //ReduceByParamValue converter = new ReduceByParamValue ( );
                        //lv . Height = lvUserctrl . Height = Convert . ToDouble ( converter . Convert ( Tabcontrol . ActualHeight , typeof ( double ) , 40 , CultureInfo . CurrentCulture ) );
                        //lv . Width = lvUserctrl . Width = Convert . ToDouble ( converter . Convert ( Tabcontrol . ActualWidth , typeof ( double ) , 10 , CultureInfo . CurrentCulture ) );

                        if ( lvUserctrl != null )
                            lvUserctrl?.UpdateLayout();
                        lvUserctrl . listview1 . Width = lvUserctrl . listview1 . Width + 1;
                        lvUserctrl . listview1 . Refresh();
                    }
                    else
                    {
                        ProgressValue = 15;
                        Tview . ProgressBar_Progress . UpdateLayout();
                        lvUserctrl = LoadListviewInBackgroundTask(lvUserctrl);

                    }
                    Debug. WriteLine("Loaded listview - yeahhhhhhh........");
                }
                IsLoadingDb = false;
                lvUserctrl . listview1 . Focus();
                TItem . Content = lvUserctrl;
                //Tview . ListviewTab . Content = lvUserctrl;
                if ( lvUserctrl != null )
                {
                    DbCountArgs cargs = new DbCountArgs();
                    cargs . Dbcount = lvUserctrl . listview1 . Items . Count;
                    cargs . sender = "lvUserctrl";
                    TabWinViewModel . TriggerBankDbCount(this , cargs);
                }
                Tview . LoadName . Text = "";
                ProgressValue = 0;
                Tview . ProgressBar_Progress . UpdateLayout();
                DbType = lvUserctrl?.CurrentType;
                //GetCurrentDbType .Invoke
            }
            else if ( tab == "LogviewTab" )
            {
                Tview . LoadName . Text = "";
                if ( IsLoadingDb == true && CurrentTabName != tab ) return;
                IsLoadingDb = true;

                Tabcontrol . SelectedIndex = 3;
                CurrentTabIndex = Tabcontrol . SelectedIndex;
                Mouse . OverrideCursor = Cursors . Wait;
                Debug. WriteLine($"Setting Logview as Active tab");
                CurrentTabTextBlock = "Tab4Header";
                TabItem TItem = Tabcontrol . Items [ 3 ] as TabItem;

                if ( logUserctrl == null )
                {
                    // get new usercontrol & add to tab content
                    logUserctrl = new LogUserControl();
                    TItem . Content = logUserctrl;
                    TItem . IsSelected = true;
                }
                ClearTags();
                logUserctrl . Tag = true;
                Tview . LogviewTab . Content = logUserctrl;
                //EventControl . TriggerWindowMessage ( this , new InterWindowArgs { message = $"LogUserControl now Active..." , listbox = null } );
                ProgressValue = 0;
                Tview . ProgressBar_Progress . UpdateLayout();
                Mouse . OverrideCursor = Cursors . Arrow;
                Debug. WriteLine($"Setting Logview as Active tab");
                IsLoadingDb = false;

                // update Db Count for field on Tabview
                DbCountArgs cargs = new DbCountArgs();
                cargs . Dbcount = logUserctrl . logview . Items . Count;
                cargs . sender = "logUserctrl";
                TabWinViewModel . TriggerBankDbCount(this , cargs);
            }
            else if ( tab == "TreeviewTab" )
            {
                Tview . LoadName . Text = "";
                if ( IsLoadingDb == true && CurrentTabName != tab ) return;
                IsLoadingDb = true;
                Tabcontrol . SelectedIndex = 4;
                CurrentTabIndex = Tabcontrol . SelectedIndex;
                Mouse . OverrideCursor = Cursors . Wait;
                Debug. WriteLine($"Setting Dummy Treeview as Active tab");
                ReduceByParamValue rbp = new ReduceByParamValue();
                CurrentTabTextBlock = "Tab5Header";

                if ( tvUserctrl == null )
                {
                    // get new usercontrol & add to tab content
                    tvUserctrl = new TvUserControl();
                    TabItem TItem = Tabcontrol . Items [ 4 ] as TabItem;
                    TItem . Content = tvUserctrl;
                    TItem . IsSelected = true;
                }
                ClearTags();
                tvUserctrl . Tag = true;
                Tabview . ResizeTreeviewTab();
                //                EventControl . TriggerWindowMessage ( this , new InterWindowArgs { message = $"tvUserControl now Active..." , listbox = null } );

                // update Db Count for field on Tabview
                DbCountArgs args = new DbCountArgs();
                args . Dbcount = tvUserctrl . treeview1 . Items . Count;
                args . sender = "tvUserctrl";
                TabWinViewModel . TriggerBankDbCount(this , args);
                ProgressValue = 0;
                Tview . TreeviewTab . Content = tvUserctrl;
                Tview . ProgressBar_Progress . UpdateLayout();
                tvUserctrl?.treeview1 . Focus();
                IsLoadingDb = false;
            }
            Mouse . OverrideCursor = Cursors . Arrow;
        }

        private void Grid1_PreviewGotKeyboardFocus (object sender , KeyboardFocusChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        #region ASYNC data load methods

        public TabData LoadListboxData (TabData tabdata)
        {
            //bool success = true;
            LbUserControl lbUserctrl;
            if ( tabdata . lbctrl == null )
            {
                lbUserctrl = new LbUserControl();
                Debug. WriteLine($"Loading Db.....");
                //                    DataTemplate dt = Application . Current . FindResource ( "CustomersDbTemplate1" ) as DataTemplate;
                DataTemplate dt = Application . Current . FindResource("BankDataTemplate1") as DataTemplate;
                lbUserctrl . listbox1 . ItemTemplate = dt;
                Thickness th = new Thickness();
                th . Left = 5;
                th . Top = 10;
                lbUserctrl . Margin = th;
                lbUserctrl . CurrentType = "BANK";
                DbType = lbUserctrl?.CurrentType;
                lbUserctrl . LoadBank(true);
                Mouse . OverrideCursor = Cursors . Arrow;
                lbUserctrl . UpdateLayout();
                tabdata . lbctrl = lbUserctrl;
                //lbUserctrl = lbUserctrl;
                //ListBox is populated when we reach here
            }
            //} );
            return tabdata;
        }
        private async Task LoadListBoxAsync ()
        {
            // We HAVE to  run most of this code inDispatcher as we are accessing UI elements
            Debug. WriteLine($"Running TASK for LbUserControl");
            Tview . Dispatcher . Invoke(DispatcherPriority . Normal , ( Action )( async () =>
            {
                ProgressValue = 25;
                Tview . ProgressBar_Progress . UpdateLayout();
                Mouse . OverrideCursor = Cursors . Wait;
                LbUserControl lbUser = new LbUserControl();
                lbUser . Name = "temporary";
                Debug. WriteLine($"Created new  Listbox in Task");
                lbUser . Visibility = Visibility . Visible;
                if ( lbUser . listbox1 . ItemsSource == null )
                {
                    Debug. WriteLine($"Loading Db data from disk via SQL.....");
                    ProgressValue = 45;
                    Tview . ProgressBar_Progress . UpdateLayout();
                    //                    DataTemplate dt = Application . Current . FindResource ( "CustomersDbTemplate1" ) as DataTemplate;
                    DataTemplate dt = Application . Current . FindResource("BankDataTemplate1") as DataTemplate;
                    lbUser . listbox1 . ItemTemplate = dt;
                    Tview . ListboxTab . Content = lbUser;
                    lbUser . CurrentType = "BANK";
                    DbType = lbUser?.CurrentType;
                    // I do NOT want to wait here, as we message when data has been loaded....
                    lbUser . LoadBank(true);
                    lbUser . listbox1 . ItemsSource = lbUser . Bvm;
                    if ( lbUserctrl != null )
                    {
                        Debug. WriteLine($"Before App : {lbUserctrl . Name} == Local : {lbUser . Name} ??");
                        SendWindowMessage($"Before App : {lbUserctrl . Name} == Local : {lbUser . Name} ??");
                    }
                    lbUserctrl = lbUser;
                    Debug. WriteLine($"After App : {lbUserctrl . Name} == Local : {lbUser . Name} ??");
                    SendWindowMessage($"After App : {lbUserctrl . Name} == Local :  {lbUser . Name} ??");
                    if ( lbUserctrl . Name != "" )
                    {
                        Debug. WriteLine($"App now has full copy of ListBoxUserControl from the Task thread");
                        SendWindowMessage($"App now has full copy of ListBoxUserControl from the Task thread");
                    }
                    ProgressValue = 75;
                    Tview . ProgressBar_Progress . UpdateLayout();
                }
                Debug. WriteLine($"Returning to main UI thread..");
            }
            ));
            ProgressValue = 100;
            Tview . ProgressBar_Progress . UpdateLayout();
            Mouse . OverrideCursor = Cursors . Arrow;
            return;
        }
        public async Task LoadListViewAsync (LvUserControl userctrl)
        {
            Debug. WriteLine($"Running TASK for LvUserControl");
            // We HAVE to  run most of this code inDispatcher as we are accessing UI elements
            //Mouse . OverrideCursor = Cursors . Wait;
            watch . Reset();
            watch . Start();
            Application . Current . Dispatcher . Invoke(DispatcherPriority . Normal , ( Action )( async () =>
            {
                Debug. WriteLine($"In Dispatcher.....");
                if ( userctrl == null )
                {
                    //Mouse . OverrideCursor = Cursors . Wait;
                    Debug. WriteLine($"Loading LvUserControl .....");
                    ProgressValue = 15;
                    Tview . ProgressBar_Progress . UpdateLayout();
                    // speed up this process as we do not need it just yet
                    Task loadcontrol = new Task(() =>
                    {
                        lvUserctrl = new LvUserControl();
                    });
                    loadcontrol . Start();
                }
                else
                    lvUserctrl = userctrl;
                Debug. WriteLine($"LvUserControl  LOADED .....");
                Debug. WriteLine($"Continuing processing in LoadListViewAsync()");
                Debug. WriteLine($"Manipulating Content/Control size in LoadListViewAsync()");
                TabItem TItem = Tabcontrol . Items [ 2 ] as TabItem;
                TItem . Content = lvUserctrl;
                Debug. WriteLine($"TIrem : {TItem . Height}, {TItem . Width}");
                ListView lb = lvUserctrl . listview1;
                ProgressValue = 35;
                Tview . ProgressBar_Progress . UpdateLayout();
                lb . HorizontalAlignment = HorizontalAlignment . Left;
                lvUserctrl . Width = TItem . ActualWidth - 5;
                lvUserctrl . Height = TItem . ActualHeight - 30;
                ProgressValue = 35;
                Tview . ProgressBar_Progress . UpdateLayout();

                // How to call a Converter from c#
                ReduceByParamValue converter = new ReduceByParamValue();
                lb . Height = lvUserctrl . Height = Convert . ToDouble(converter . Convert(Tabcontrol . ActualHeight , typeof(double) , 40 , CultureInfo . CurrentCulture));
                lb . Width = lvUserctrl . Width = Convert . ToDouble(converter . Convert(Tabcontrol . ActualWidth , typeof(double) , 10 , CultureInfo . CurrentCulture));
                lvUserctrl . Visibility = Visibility . Visible;
                TItem . IsSelected = true;
                Debug. WriteLine($"checking  for data loaded ");

                if ( lvUserctrl . listview1 . ItemsSource == null )
                {
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
                    Debug. WriteLine("Loading data - Calling multiple task system here......");

                    // First, declare the Main Task (task2() here) as a new task with the usual ( () => { setup
                    // DO NOT CLOSE IT ..... just wrap it all in { ......  }
                    Task maintask = new Task(() =>
                    {
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
                        Task child = new Task(() =>
                        {
                            Debug. WriteLine("parent task starting ......");
                            lvUserctrl . LoadBank(true);
                            Debug. WriteLine("parent task ended ......");

                        } , TaskCreationOptions . AttachedToParent
                       );

                        child . Start();
                        // Parent Task

                        // MAIN TASK HERE
                        Application . Current . Dispatcher . Invoke(() =>
                        {
                            Debug. WriteLine("child task starting ......");
                            ProgressValue = 55;
                            Tview . ProgressBar_Progress . UpdateLayout();
                            Debug. WriteLine($"Loading Db.....");
                            DataTemplate dt = Application . Current . FindResource("BankDataTemplate1") as DataTemplate;
                            lvUserctrl . listview1 . ItemTemplate = dt;
                            Tview . ListviewTab . Content = lvUserctrl;
                            TItem . Content = lvUserctrl;
                            lvUserctrl . CurrentType = "BANK";
                            DbType = lvUserctrl?.CurrentType;
                            //Mouse . OverrideCursor = Cursors . Arrow;
                            Debug. WriteLine("child task ended ......");
                            lvUserctrl . UpdateLayout();
                            ProgressValue = 75;
                            Tview . ProgressBar_Progress . UpdateLayout();
                        });
                        //Debug. WriteLine ( "parent task starting ......" );
                        //lvUserctrl . LoadBank ( true );
                        //Debug. WriteLine ( "parent task ended ......" );
                    }
                    );
                    Debug. WriteLine("calling maintask ......");
                    maintask . Start();
                    Debug. WriteLine("maintask ended ......");
                }
                Debug. WriteLine("multiple task system Ended ......");
                Debug. WriteLine("Following Code is being processed WHILE tadsks are running......");

                //---------------------------------------//
                Debug. WriteLine($"Data loaded so showing tab..");
                Tabcontrol . SelectedIndex = 2;
                CurrentTabIndex = Tabcontrol . SelectedIndex;
                DbType = lvUserctrl?.CurrentType;
                lvUserctrl . listview1 . ScrollIntoView(lvUserctrl . listview1 . SelectedItem);
                Utils . ScrollLBRecordIntoView(lvUserctrl . listview1 , lvUserctrl . listview1 . SelectedIndex);
                lvUserctrl . Visibility = Visibility . Visible;
                lvUserctrl . UpdateLayout();
                ProgressValue = 100;
                Tview . ProgressBar_Progress . UpdateLayout();
                Mouse . OverrideCursor = Cursors . Arrow;
            } ));
            Debug. WriteLine($"*** Exiting async method entirely .. NB : Data may still be loading in maintask ***");
            long msecs = watch . ElapsedMilliseconds;
            watch . Stop();
            Debug. WriteLine($"Elapsed msecs = {msecs}");
            return;
        }

        public async Task LoadDataGridAsync (DgUserControl userctrl)
        {
            Debug. WriteLine($"Running TASK for LvUserControl");
            // We HAVE to  run most of this code inDispatcher as we are accessing UI elements
            //Mouse . OverrideCursor = Cursors . Wait;
            watch . Reset();
            watch . Start();
            //Application . Current . Dispatcher . Invoke ( DispatcherPriority . Normal , ( Action ) ( async ( ) =>
            //{
            Debug. WriteLine($"In Dispatcher.....");
            if ( userctrl == null )
            {
                //Mouse . OverrideCursor = Cursors . Wait;
                Debug. WriteLine($"Loading DgUserControl .....");
                ProgressValue = 15;
                Tview . ProgressBar_Progress . UpdateLayout();
                // speed up this process as we do not need it just yet
                Task loadcontrol = new Task(() =>
                {
                    dgUserctrl = new DgUserControl();
                });
                loadcontrol . Start();
            }
            else
                dgUserctrl = userctrl;

            Debug. WriteLine($"DgUserControl  LOADED .....");
            Debug. WriteLine($"Continuing processing in LoadDataGridAsync()");
            Debug. WriteLine($"Manipulating Content/Control size in LoadDataGridAsync()");
            TabItem TItem = Tabcontrol . Items [ 2 ] as TabItem;
            TItem . Content = dgUserctrl;
            Debug. WriteLine($"TIrem : {TItem . Height}, {TItem . Width}");
            DataGrid dg = dgUserctrl . grid1;
            ProgressValue = 35;
            Tview . ProgressBar_Progress . UpdateLayout();
            dg . HorizontalAlignment = HorizontalAlignment . Left;
            dgUserctrl . Width = TItem . ActualWidth - 5;
            dgUserctrl . Height = TItem . ActualHeight - 30;
            ProgressValue = 35;
            Tview . ProgressBar_Progress . UpdateLayout();

            // How to call a Converter from c#
            ReduceByParamValue converter = new ReduceByParamValue();
            dg . Height = dgUserctrl . Height = Convert . ToDouble(converter . Convert(Tabcontrol . ActualHeight , typeof(double) , 40 , CultureInfo . CurrentCulture));
            dg . Width = dgUserctrl . Width = Convert . ToDouble(converter . Convert(Tabcontrol . ActualWidth , typeof(double) , 10 , CultureInfo . CurrentCulture));
            dgUserctrl . Visibility = Visibility . Visible;
            TItem . IsSelected = true;
            Debug. WriteLine($"checking  for data loaded ");

            if ( dgUserctrl . grid1 . ItemsSource == null )
            {
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
                Debug. WriteLine("Loading data - Calling multiple task system here......");

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
                Tview . ProgressBar_Progress . UpdateLayout();
                Debug. WriteLine($"Loading Db.....");
                DataTemplate dt = Application . Current . FindResource("BankDataTemplate1") as DataTemplate;
                dgUserctrl . grid1 . ItemTemplate = dt;
                Tview . DgridTab . Content = dgUserctrl;
                TItem . Content = dgUserctrl;
                dgUserctrl . CurrentType = "BANK";
                DbType = dgUserctrl?.CurrentType;
                //Mouse . OverrideCursor = Cursors . Arrow;
                Debug. WriteLine("child task ended ......");
                dgUserctrl . UpdateLayout();
                ProgressValue = 75;
                Tview . ProgressBar_Progress . UpdateLayout();
                //   } , TaskCreationOptions . AttachedToParent
                //);

                //child . Start ( );
                Debug. WriteLine("parent task starting ......");
                //dgUserctrl . LoadBank ( true );
                ObservableCollection<BankAccountViewModel> Bvm = new ObservableCollection<BankAccountViewModel>();
                UserControlDataAccess . GetBankObsCollection(true , "DgUserControl" , -1);
                //                    } );

                Debug. WriteLine("parent task ended ......");

                // Parent Task

                // MAIN TASK HERE
                //Debug. WriteLine ( "child task starting ......" );
                //ProgressValue = 55;
                //Tview . ProgressBar_Progress . UpdateLayout ( );
                //Debug. WriteLine ( $"Loading Db....." );
                //DataTemplate dt = Application . Current . FindResource ( "BankDataTemplate1" ) as DataTemplate;
                //dgUserctrl . grid1 . ItemTemplate = dt;
                //Tview . DgridTab . Content = dgUserctrl;
                //TItem . Content = dgUserctrl;
                //dgUserctrl . CurrentType = "BANK";
                //DbType = dgUserctrl?.CurrentType;
                ////Mouse . OverrideCursor = Cursors . Arrow;
                //Debug. WriteLine ( "child task ended ......" );
                //dgUserctrl . UpdateLayout ( );
                //ProgressValue = 75;
                //Tview . ProgressBar_Progress . UpdateLayout ( );

                ////Debug. WriteLine ( "parent task starting ......" );
                ////lvUserctrl . LoadBank ( true );
                ////Debug. WriteLine ( "parent task ended ......" );
                //}
                //);
                //Debug. WriteLine ( "calling maintask ......" );
                //maintask . Start ( );
                //Debug. WriteLine ( "maintask ended ......" );
                //}
                Debug. WriteLine("multiple task system Ended ......");
                Debug. WriteLine("Following Code is being processed WHILE tadsks are running......");

                //---------------------------------------//
                Debug. WriteLine($"Data loaded so showing tab..");
                Tabcontrol . SelectedIndex = 2;
                CurrentTabIndex = Tabcontrol . SelectedIndex;
                DbType = dgUserctrl?.CurrentType;
                //                dgUserctrl . grid1 . ScrollIntoView ( dgUserctrl . grid1 . SelectedItem );
                //                Utils . ScrollRecordIntoView ( dgUserctrl . grid1 , dgUserctrl . grid1 . SelectedIndex );
                dgUserctrl . Visibility = Visibility . Visible;
                dgUserctrl . UpdateLayout();
                ProgressValue = 100;
                Tview . ProgressBar_Progress . UpdateLayout();
                //            } ) );
                Debug. WriteLine($"*** Exiting async method entirely .. NB : Data may still be loading in maintask ***");
                long msecs = watch . ElapsedMilliseconds;
                watch . Stop();
                Debug. WriteLine($"Elapsed msecs = {msecs}");
                Mouse . OverrideCursor = Cursors . Arrow;
                return;
            }
        }
        #endregion ASYNC data load methods

        private void SendWindowMessage (string msg = "")
        {
            InterWindowArgs args = new InterWindowArgs();

            args . data = null;
            args . window = null;
            args . message = msg;
            EventControl . TriggerWindowMessage(this , args);
        }
        private void CloseAppExecute (object obj)
        {
            Application . Current . Shutdown();
        }
        private void CloseWinExecute (object obj)
        {
            if ( dgUserctrl != null )
            {
                dgUserctrl . grid1 . ItemsSource = null;
                dgUserctrl . grid1 . Items . Clear();
                dgUserctrl = null;
                Tview . DgridTab . Content = null;
            }
            if ( lbUserctrl != null )
            {
                lbUserctrl . listbox1 . ItemsSource = null;
                lbUserctrl . listbox1 . Items . Clear();
                lbUserctrl = null;
                Tview . ListboxTab . Content = null;
            }
            if ( lvUserctrl != null )
            {
                lvUserctrl . listview1 . ItemsSource = null;
                lvUserctrl . listview1 . Items . Clear();
                lvUserctrl = null;
                Tview . ListviewTab . Content = null;
            }
            if ( tvUserctrl != null )
            {
                tvUserctrl . treeview1 . ItemsSource = null;
                tvUserctrl . treeview1 . Items . Clear();
                tvUserctrl = null;
                Tview . TreeviewTab . Content = null;
            }
            if ( logUserctrl != null )
            {
                logUserctrl . logview . ItemsSource = null;
                logUserctrl . logview . Items . Clear();
                logUserctrl = null;
                Tview . LogviewTab . Content = null;
            }
            lbUserctrl = null;
            lvUserctrl = null;
            dgUserctrl = null;
            logUserctrl = null;
            tvUserctrl = null;
            ThisWin = null;
            Tview . Close();
        }
        public void Closedown ()
        {
            lbUserctrl = null;
            lvUserctrl = null;
            dgUserctrl = null;
            tvUserctrl = null;
            ThisWin = null;
        }
        private void ClearTags ()
        {
            if ( dgUserctrl != null )
                dgUserctrl . Tag = false;
            if ( lbUserctrl != null )
                lbUserctrl . Tag = false;
            if ( lvUserctrl != null )
                lvUserctrl . Tag = false;
            if ( logUserctrl != null )
                logUserctrl . Tag = false;
            if ( tvUserctrl != null )
                tvUserctrl . Tag = false;
        }

        #region NOT USED

        #region BACKGROUND THREADS

        //*************************************************
        #region Datagrid BackgrundWorker Loading
        public DgUserControl LoadDatagridInBackgroundTask (DgUserControl dguserctrl)
        {
            BackgroundWorker worker = new BackgroundWorker();
            Debug. WriteLine($"Calling DataGrid Background Worker setup");
            worker . WorkerSupportsCancellation = true;
            worker . WorkerReportsProgress = true;
            worker . ProgressChanged += DgWorker_ProgressChanged;
            worker . RunWorkerCompleted += DgWorker_RunWorkerCompleted;
            worker . DoWork += DgWorker_DoWork;
            Debug. WriteLine($"Running DataGrid Background Worker");
            ProgressValue = 15;
            Tview . ProgressBar_Progress . UpdateLayout();
            worker . RunWorkerAsync(dguserctrl);
            return dguserctrl;
        }
        private void DgWorker_DoWork (object sender , DoWorkEventArgs e)
        {
            Debug. WriteLine($"Calling DataGrid Background Worker ProgressChanged method");
            BackgroundWorker worker = sender as BackgroundWorker;
            worker . ReportProgress(0 , e . Argument as DgUserControl);
            Debug. WriteLine($"Returned from DataGrid Background Worker ProgressChanged method");
            Debug. WriteLine($"Cancelling thread of DataGrid ProgressChanged method");
            worker . CancelAsync();
        }
        public void DgWorker_RunWorkerCompleted (object sender , RunWorkerCompletedEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            if ( e . Cancelled )
                Debug. WriteLine($"DataGrid Worker Cancelled");
            else
                Debug. WriteLine($"DataGrid Worker completed");
            Tview . ProgressBar_Progress . Value = 0;
            Tview . ProgressBar_Progress . UpdateLayout();
            Mouse . OverrideCursor = Cursors . Arrow;
            worker . CancelAsync();
        }
        private void DgWorker_ProgressChanged (object sender , ProgressChangedEventArgs e)
        {
            long msecs = 0;
            BackgroundWorker worker = sender as BackgroundWorker;
            //DgUserControl dgUserctrl = e . UserState as DgUserControl;
            Stopwatch watch = new Stopwatch();
            ProgressValue = 25;
            Tview . ProgressBar_Progress . UpdateLayout();
            if ( dgUserctrl == null || dgUserctrl . grid1 . Items . Count == 0 )
            {
                if ( dgUserctrl == null )
                    dgUserctrl = new DgUserControl();

                ReduceByParamValue convert;
                watch . Start();
                Debug. WriteLine($"Creating new DgUserControl in Background Worker ProgressChanged method");
                EventControl . TriggerWindowMessage(this , new InterWindowArgs { message = $"Creating new DgUserControl..." , listbox = null });
                watch . Stop();
                msecs = watch . ElapsedMilliseconds;
                Debug. WriteLine($"new DgUserControl took {msecs}");

                if ( dgUserctrl . grid1 . ItemsSource == null )
                {
                    Tview . ProgressBar_Progress . Value = 35;
                    Tview . ProgressBar_Progress . UpdateLayout();
                    Debug. WriteLine($"Loading Db.....");
                    //                    DataTemplate dt = Application . Current . FindResource ( "CustomersDbTemplate1" ) as DataTemplate;
                    DataTemplate dt = Application . Current . FindResource("BankDataTemplate1") as DataTemplate;
                    dgUserctrl . grid1 . ItemTemplate = dt;
                    Tview . DgridTab . Content = dgUserctrl;
                    dgUserctrl . CurrentType = "BANK";
                    DbType = dgUserctrl?.CurrentType;
                    EventControl . TriggerWindowMessage(this , new InterWindowArgs { message = $"Loading Bank data in DgUserControl..." , listbox = null });

                    Tview . Dispatcher . Invoke(DispatcherPriority . Normal , ( Action )( async () =>
                    {
                        dgUserctrl . LoadBank(true);
                    } ));

                    Tview . ProgressBar_Progress . Value = 65;
                    Tview . ProgressBar_Progress . UpdateLayout();
                    if ( dgUserctrl . Bvm . Count > 0 )
                        dgUserctrl . grid1 . ItemsSource = dgUserctrl . Bvm;

                    Mouse . OverrideCursor = Cursors . Arrow;
                    dgUserctrl . UpdateLayout();
                }

                ProgressValue = 45;
                Tview . ProgressBar_Progress . UpdateLayout();
                //DataGrid lb = dgUserctrl . grid1;
                //convert = new ReduceByParamValue ( );
                //lb . Height = dgUserctrl . Height = Convert . ToDouble ( convert . Convert ( Tabcontrol . ActualHeight , typeof ( double ) , 40 , CultureInfo . CurrentCulture ) );
                //lb . Width = dgUserctrl . Width = Convert . ToDouble ( convert . Convert ( Tabcontrol . ActualWidth , typeof ( double ) , 10 , CultureInfo . CurrentCulture ) );
            }
            Tview . ProgressBar_Progress . Value = 75;
            Tview . ProgressBar_Progress . UpdateLayout();
            watch . Start();
            Mouse . OverrideCursor = Cursors . Wait;
            Debug. WriteLine($"Loading DataGrid in Background Worker ProgressChanged method");
            TabItem TItem = Tabcontrol . Items [ 0 ] as TabItem;
            TItem . Content = dgUserctrl;
            DataGrid dg = dgUserctrl . grid1;
            // How to call a Converter from c#
            ReduceByParamValue converter = new ReduceByParamValue();
            dg . Height = dgUserctrl . Height = Convert . ToDouble(converter . Convert(Tabcontrol . ActualHeight , typeof(double) , 40 , CultureInfo . CurrentCulture));
            dg . Width = dgUserctrl . Width = Convert . ToDouble(converter . Convert(Tabcontrol . ActualWidth , typeof(double) , 10 , CultureInfo . CurrentCulture));
            dgUserctrl . Visibility = Visibility . Visible;
            TItem . IsSelected = true;
            Debug. WriteLine($"DataGrid loaded by Background Worker ");
            Debug. WriteLine($"Data loaded so showing tab..");
            watch . Stop();
            Debug. WriteLine($"new DgUserControl took {watch . ElapsedMilliseconds}, Total = Create : {msecs} + Load : {watch . ElapsedMilliseconds} =  {msecs + watch . ElapsedMilliseconds}");
            Mouse . OverrideCursor = Cursors . Arrow;
            Tview . ProgressBar_Progress . Value = 100;
            Tview . ProgressBar_Progress . UpdateLayout();
            EventControl . TriggerWindowMessage(this , new InterWindowArgs { message = $"Loading completed DgUserControl..." , listbox = null });

        }
        #endregion Datagrid BackgrundWorker Loading

        //*************************************************
        #region ListBox BackgrundWorker Loading
        public LbUserControl LoadListboxInBackgroundTask (LbUserControl lbuserctrl)
        {
            int progress = 10;
            BackgroundWorker worker = new BackgroundWorker();
            Debug. WriteLine($"Calling Background Worker setup");
            worker . WorkerSupportsCancellation = true;
            worker . WorkerReportsProgress = true;
            worker . ProgressChanged += LbWorker_ProgressChanged;
            worker . RunWorkerCompleted += LbWorker_RunWorkerCompleted;
            worker . DoWork += LbWorker_DoWork;
            Debug. WriteLine($"Running Background Worker");
            worker . ReportProgress(progress , lbuserctrl);
            worker . RunWorkerAsync(lbuserctrl);
            return lbuserctrl;
        }
        private void LbWorker_DoWork (object sender , DoWorkEventArgs e)
        {
            Debug. WriteLine($"Calling ListBox Background Worker ProgressChanged method");
            BackgroundWorker worker = sender as BackgroundWorker;
            // How  to pass more than just percent  done by using e.Argument (object) which can be anyhing you want to unpack in progresschanged()
            worker . ReportProgress(10 , e . Argument as LbUserControl);
            Debug. WriteLine($"Cancelling thread of ProgressChanged method");
            worker . CancelAsync();
        }
        public void LbWorker_RunWorkerCompleted (object sender , RunWorkerCompletedEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            if ( e . Cancelled )
                Debug. WriteLine($"Worker Cancelled");
            else
                Debug. WriteLine($"Worker completed");
            Tview . ProgressBar_Progress . Value = 0;
            Tview . ProgressBar_Progress . UpdateLayout();
            Tview . ProgressBar_Progress . Value = 0;
            Tview . ProgressBar_Progress . UpdateLayout();
            Mouse . OverrideCursor = Cursors . Arrow;
            worker . CancelAsync();
        }
        private async void LbWorker_ProgressChanged (object sender , ProgressChangedEventArgs e)
        {
            long msecs = 0;
            int progress = e . ProgressPercentage;
            LbUserControl lbUserctrl = e . UserState as LbUserControl;
            Tview . ProgressBar_Progress . Value = progress;
            BackgroundWorker worker = sender as BackgroundWorker;
            Stopwatch watch = new Stopwatch();
            Debug. WriteLine($"Running BACKGROUND for LbUserControl");

            if ( lbUserctrl . listbox1 . Items . Count == 0 )
            {
                watch . Start();
                Debug. WriteLine($"Creating new LbUserControl in Background Worker ProgressChanged method");
                if ( lbUserctrl == null )
                    lbUserctrl = new LbUserControl();

                watch . Stop();
                msecs = watch . ElapsedMilliseconds;
                Debug. WriteLine($"new LbUserControl took {msecs}");
                ListBox lbx = lbUserctrl . listbox1;
                ReduceByParamValue convert = new ReduceByParamValue();
                lbx . Height = lbUserctrl . Height = Convert . ToDouble(convert . Convert(Tabcontrol . ActualHeight , typeof(double) , 40 , CultureInfo . CurrentCulture));
                lbx . Width = lbUserctrl . Width = Convert . ToDouble(convert . Convert(Tabcontrol . ActualWidth , typeof(double) , 10 , CultureInfo . CurrentCulture));
                Tview . ProgressBar_Progress . Value = 35;
                Tview . ProgressBar_Progress . UpdateLayout();

                if ( lbUserctrl . listbox1 . ItemsSource == null )
                {
                    Debug. WriteLine($"Loading Db.....");
                    DataTemplate dt = Application . Current . FindResource("BankDataTemplate1") as DataTemplate;
                    lbUserctrl . listbox1 . ItemTemplate = dt;
                    Thickness t1 = new Thickness();
                    t1 = new Thickness();
                    t1 . Left = 5;
                    t1 . Top = 10;
                    lbUserctrl . Margin = t1;
                    Tview . ListboxTab . Content = lbUserctrl;
                    lbUserctrl . CurrentType = "BANK";
                    DbType = lbUserctrl?.CurrentType;
                    //Tview . Dispatcher . Invoke ( DispatcherPriority . Normal , ( Action ) ( async ( ) =>
                    //{
                    lbUserctrl . LoadBank(true);
                    Tview . ProgressBar_Progress . Value = 65;
                    Tview . ProgressBar_Progress . UpdateLayout();
                    //} ) );

                    if ( lbUserctrl . Bvm . Count > 0 )
                        lbUserctrl . listbox1 . ItemsSource = lbUserctrl . Bvm;

                    Mouse . OverrideCursor = Cursors . Arrow;
                    lbUserctrl . UpdateLayout();
                    Tview . ProgressBar_Progress . Value = 85;
                    Tview . ProgressBar_Progress . UpdateLayout();
                    Debug. WriteLine($"Data loading is in process");
                }
                else
                {
                    lbUserctrl . listbox1 . ScrollIntoView(lbUserctrl . listbox1 . SelectedItem);
                    Utils . ScrollLBRecordIntoView(lbUserctrl . listbox1 , lbUserctrl . listbox1 . SelectedIndex);
                    Mouse . OverrideCursor = Cursors . Arrow;
                    watch . Stop();
                }
                Tview . ProgressBar_Progress . Value = 100;
                Tview . ProgressBar_Progress . UpdateLayout();
            }
            else
                lbUserctrl = e . UserState as LbUserControl;

            watch . Start();
            Mouse . OverrideCursor = Cursors . Wait;
            Debug. WriteLine($"Loading Listbox in Background Worker ProgressChanged method");
            TabItem TItem = Tabcontrol . Items [ 1 ] as TabItem;
            Thickness th = new Thickness();
            th . Left = 5;
            th . Top = 10;
            th . Right = 5;
            th . Bottom = 10;
            lbUserctrl . Margin = th;
            TItem . Content = lbUserctrl;
            ListBox lb = lbUserctrl . listbox1;
            Tview . ProgressBar_Progress . Value = 45;
            Tview . ProgressBar_Progress . UpdateLayout();
            // How to call a Converter from c#
            ReduceByParamValue converter = new ReduceByParamValue();
            lb . Height = lbUserctrl . Height = Convert . ToDouble(converter . Convert(Tabcontrol . ActualHeight , typeof(double) , 40 , CultureInfo . CurrentCulture));
            lb . Width = lbUserctrl . Width = Convert . ToDouble(converter . Convert(Tabcontrol . ActualWidth , typeof(double) , 10 , CultureInfo . CurrentCulture));
            lbUserctrl . Visibility = Visibility . Visible;
            TItem . IsSelected = true;
            Debug. WriteLine($"Listbox loaded by Background Worker ");

            // not in use
            if ( lbUserctrl . listbox1 . ItemsSource == null )
            {
                //    Debug. WriteLine ( $"Loading Db....." );
                //    DataTemplate dt = Application . Current . FindResource ( "BankDataTemplate1" ) as DataTemplate;
                //    lbUserctrl . listbox1 . ItemTemplate = dt;
                //    th = new Thickness ( );
                //    th . Left = 5;
                //    th . Top = 10;
                //    lbUserctrl . Margin = th;
                //    Tview . ListboxTab . Content = lbUserctrl;
                //     lbUserctrl . CurrentType = "BANK";
                //    DbType = lbUserctrl?.CurrentType;
                //    Application . Current . Dispatcher . Invoke ( DispatcherPriority . Normal , ( Action ) ( async ( ) =>
                //      {
                //          lbUserctrl . LoadBank ( true );
                //        }));
                //    Mouse . OverrideCursor = Cursors . Arrow;
                //    lbUserctrl . UpdateLayout ( );
                //    Tview . ProgressBar_Progress . Value = 75;
                //    Tview . ProgressBar_Progress . UpdateLayout ( );
                //    Debug. WriteLine ( $"Data loaded so showing tab.." );
                //}
                //else
                //{
                //    lbUserctrl . listbox1 . ScrollIntoView ( lbUserctrl . listbox1 . SelectedItem );
                //    Utils . ScrollLBRecordIntoView ( lbUserctrl . listbox1 , lbUserctrl . listbox1 . SelectedIndex );
                //    Mouse . OverrideCursor = Cursors . Arrow;
                //    watch . Stop ( );
            }

            Tabcontrol . SelectedIndex = 1;
            CurrentTabIndex = Tabcontrol . SelectedIndex;
            lbUserctrl . listbox1 . Focus();
            DbType = lbUserctrl?.CurrentType;
            Debug. WriteLine($"new LbUserControl took {watch . ElapsedMilliseconds}, Total = Create : {msecs} + Load : {watch . ElapsedMilliseconds} =  {msecs + watch . ElapsedMilliseconds}");
            Tview . ProgressBar_Progress . Value = 100;
            Tview . ProgressBar_Progress . UpdateLayout();
            Mouse . OverrideCursor = Cursors . Arrow;

            // This creates a new ListBox entirely in C# and displays it on the L
            // if(SHOWC#LISTBOX ist Box tab.....
            //however, the Style set in generator does NOT work correctly
            if ( SHOWCLBOX )
            {
                lb = lbUserctrl . CreateListBox();
                lb . Items . Add("aaaaaaaaaaaaaaaaaaaa");
                lb . Items . Add("bbbbbbbbbbbbbbbbbbbb");
                lb . Items . Add("cccccccccccccccccccc");
                lb . Items . Add("ddddddddddddddddddddddddddd");
                lb . Items . Add("eeeeeeeeeeeeeeeeeeeeee");
                lb . Items . Add("fffffffffffffffffffffffffffffffff");
                lb . Items . Add("gggggggggggggggggggggggggggggggg");
                lb . Items . Add("hhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh");
                lb . Items . Add("iiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiii");
                lb . Items . Add("jjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjj");
                lb . Items . Add("mmmmmmmmmmmmmmmmmmmmmmmmmmm");
                Tview . ListboxTab . Content = lb;
            }
        }

        public bool SHOWCLBOX = false;

        #endregion ListBox BackgrundWorker Loading

        //*************************************************
        #region ListView BackgrundWorker Loading
        public LvUserControl LoadListviewInBackgroundTask (LvUserControl lvuserctrl)
        {
            BackgroundWorker worker = new BackgroundWorker();
            Debug. WriteLine($"Calling Background Worker setup");
            worker . WorkerSupportsCancellation = true;
            worker . WorkerReportsProgress = true;
            worker . ProgressChanged += LvWorker_ProgressChanged;
            worker . RunWorkerCompleted += LvWorker_RunWorkerCompleted;
            worker . DoWork += LvWorker_DoWork;
            Debug. WriteLine($"Running Background Worker");
            ProgressValue = 15;
            Tview . ProgressBar_Progress . UpdateLayout();
            worker . RunWorkerAsync(lvuserctrl);
            return lvuserctrl;
        }
        private void LvWorker_DoWork (object sender , DoWorkEventArgs e)
        {
            Debug. WriteLine($"Calling ListView ackground Worker ProgressChanged method");
            BackgroundWorker worker = sender as BackgroundWorker;
            worker . ReportProgress(10 , e . Argument as LvUserControl);
            Debug. WriteLine($"Returned from Background Worker ProgressChanged method");
            Debug. WriteLine($"Cancelling thread of ProgressChanged method");
            worker . CancelAsync();
        }
        public void LvWorker_RunWorkerCompleted (object sender , RunWorkerCompletedEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            if ( e . Cancelled )
                Debug. WriteLine($"Worker Cancelled");
            else
                Debug. WriteLine($"Worker completed");
            Tview . ProgressBar_Progress . Value = 0;
            Tview . ProgressBar_Progress . UpdateLayout();
            Tview . ProgressBar_Progress . Value = 0;
            Tview . ProgressBar_Progress . UpdateLayout();
            Mouse . OverrideCursor = Cursors . Arrow;
            worker . CancelAsync();
        }
        private async void LvWorker_ProgressChanged (object sender , ProgressChangedEventArgs e)
        {
            long msecs = 0;
            int progress = e . ProgressPercentage;
            Tview . ProgressBar_Progress . Value = progress;
            Tview . ProgressBar_Progress . UpdateLayout();
            LvUserControl lvUserctrl = e . UserState as LvUserControl;
            BackgroundWorker worker = sender as BackgroundWorker;
            Stopwatch watch = new Stopwatch();
            Mouse . OverrideCursor = Cursors . Wait;
            if ( lvUserctrl == null || lvUserctrl . listview1 . Items . Count == 0 )
            {
            }
            if ( lvUserctrl == null )
            {
                watch . Start();
                Debug. WriteLine($"Creating new LvUserControl in Background Worker ProgressChanged method");
                lvUserctrl = new LvUserControl();
                watch . Stop();
                msecs = watch . ElapsedMilliseconds;
                Debug. WriteLine($"new LvUserControl ook {msecs}");
            }
            if ( lvUserctrl . listview1 . ItemsSource == null )
            {
                Debug. WriteLine($"Loading Db.....");
                //                    DataTemplate dt = Application . Current . FindResource ( "CustomersDbTemplate1" ) as DataTemplate;
                DataTemplate dt = Tview . FindResource("BankDataTemplate1") as DataTemplate;
                lvUserctrl . listview1 . ItemTemplate = dt;
                Thickness t2 = new Thickness();
                t2 . Left = 5;
                t2 . Top = 10;
                lvUserctrl . Margin = t2;
                Tview . ListviewTab . Content = lvUserctrl;
                lvUserctrl . CurrentType = "BANK";
                DbType = lvUserctrl?.CurrentType;
                Tview . ProgressBar_Progress . Value = 55;
                Tview . ProgressBar_Progress . UpdateLayout();

                //Data is Loaded via EventControl Mesage BankDataLoaded
                lvUserctrl . LoadBank();
                //if ( lvUserctrl . Bvm . Count > 0 && lvUserctrl . listview1 . Items . Count > 0 )
                //    lvUserctrl . listview1 . ItemsSource = lvUserctrl . Bvm;
                Mouse . OverrideCursor = Cursors . Arrow;
                lvUserctrl . UpdateLayout();
                Tview . ProgressBar_Progress . Value = 75;
                Tview . ProgressBar_Progress . UpdateLayout();
            }
            Debug. WriteLine($"Loading Listview in Background Worker ProgressChanged method");
            //            Debug. WriteLine ( $"ListView : {lvUserctrl . GetHashCode ( )}" );
            TabItem TItem = Tabcontrol . Items [ 2 ] as TabItem;
            Thickness th = new Thickness();
            th . Left = 5;
            th . Top = 10;
            th . Right = 5;
            th . Bottom = 10;
            lvUserctrl . Margin = th;
            TItem . Content = lvUserctrl;
            ListView lv = lvUserctrl . listview1;
            // How to call a Converter from c#
            ReduceByParamValue converter = new ReduceByParamValue();
            lv . Height = lvUserctrl . Height = Convert . ToDouble(converter . Convert(Tabcontrol . ActualHeight , typeof(double) , 40 , CultureInfo . CurrentCulture));
            lv . Width = lvUserctrl . Width = Convert . ToDouble(converter . Convert(Tabcontrol . ActualWidth , typeof(double) , 10 , CultureInfo . CurrentCulture));
            lvUserctrl . Visibility = Visibility . Visible;
            TItem . IsSelected = true;
            Debug. WriteLine($"Listview loaded by Background Worker ");
            Tabcontrol . SelectedIndex = 2;
            CurrentTabIndex = Tabcontrol . SelectedIndex;
            Tview . ProgressBar_Progress . Value = 100;
            Tview . ProgressBar_Progress . UpdateLayout();
            Debug. WriteLine($"Data loaded so showing tab..");
            lvUserctrl . listview1 . Focus();
            DbType = lvUserctrl?.CurrentType;
            lvUserctrl . listview1 . ScrollIntoView(lvUserctrl . listview1 . SelectedItem);
            Utils . ScrollLBRecordIntoView(lvUserctrl . listview1 , lvUserctrl . listview1 . SelectedIndex);
            Debug. WriteLine($"ListView : {lvUserctrl . GetHashCode()}");
            Tview . ProgressBar_Progress . Value = 100;
            Tview . ProgressBar_Progress . UpdateLayout();
        }
        #endregion Listview BackgrundWorker Loading

        #endregion BACKGROUND THREADS

        private void UpdateInfopanel (SelectionChangedArgs e)
        {
            if ( e . data == null )
                return;
            BankAccountViewModel bv = new BankAccountViewModel();
            CustomerViewModel cv = new CustomerViewModel();
            if ( e . sendername == "BANK" )
            {
                bv = e . data as BankAccountViewModel;
                if ( bv != null )
                    if ( Tview != null ) Tview . InfoPanel . Text = $"{bv . CustNo} : {bv . BankNo},  {bv . Balance}";
                DbType = "BANK";
            }
            else
            {
                cv = e . data as CustomerViewModel;
                if ( cv != null )
                    if ( Tview != null ) Tview . InfoPanel . Text = $"{cv . CustNo} : {cv . BankNo},  {cv . FName}";
                DbType = "CUSTOMER";
            }
        }
        private void FindMatch (string Custno , string Bankno , string type , object data)
        {
            int index = 0;
            if ( type == "LB" )
            {
                if ( lbUserctrl == null ) return;
                if ( lbUserctrl . listbox1 . ItemsSource == null ) return;
                if ( lbUserctrl . CurrentType == "BANK" )
                {

                    try
                    {
                        foreach ( BankAccountViewModel item in lbUserctrl . listbox1 . Items )
                        {
                            if ( item . CustNo == Custno && item . BankNo == Bankno )
                            {
                                IsLoadingDb = true;
                                lbUserctrl . listbox1 . SelectedIndex = index;
                                Utils . ScrollLBRecordIntoView(lbUserctrl . listbox1 , lbUserctrl . listbox1 . SelectedIndex);
                                IsLoadingDb = false;
                                break;
                            }
                            index++;
                        }
                    }
                    catch ( Exception ex ) { Debug. WriteLine($"ListBox Matching failed {ex . Message}, [{ex . Data}]"); }
                }
                else
                {
                    try
                    {
                        foreach ( CustomerViewModel item in lbUserctrl . listbox1 . Items )
                        {
                            if ( item . CustNo == Custno && item . BankNo == Bankno )
                            {
                                IsLoadingDb = true;
                                lbUserctrl . listbox1 . SelectedIndex = index;
                                Utils . ScrollLBRecordIntoView(lbUserctrl . listbox1 , lbUserctrl . listbox1 . SelectedIndex);
                                IsLoadingDb = false;
                                break;
                            }
                            index++;
                        }
                    }
                    catch ( Exception ex ) { Debug. WriteLine($"ListBox Matching failed {ex . Message}, [{ex . Data}]"); }
                }
            }
            else if ( type == "LV" )
            {
                //              if ( lvUserctrl . listview1 . SelectedItem == null ) return;
                //bv = lvUserctrl . listview1 . SelectedItem as BankAccountViewModel;
                //Custno = bv . CustNo;
                //Bankno = bv . BankNo;
                if ( lvUserctrl == null ) return;
                if ( lvUserctrl . listview1 . ItemsSource == null ) return;
                if ( lvUserctrl . CurrentType == "BANK" )
                {
                    try
                    {
                        foreach ( BankAccountViewModel item in lvUserctrl . listview1 . Items )
                        {
                            if ( item . CustNo == Custno && item . BankNo == Bankno )
                            {
                                IsLoadingDb = true;
                                lvUserctrl . listview1 . SelectedIndex = index;
                                Utils . ScrollLVRecordIntoView(lvUserctrl . listview1 , lvUserctrl . listview1 . SelectedIndex);
                                IsLoadingDb = false;
                                break;
                            }
                            index++;
                        }
                    }
                    catch ( Exception ex ) { Debug. WriteLine($"ListView Matching failed {ex . Message}, [{ex . Data}]"); }
                }
                else
                {
                    try
                    {
                        foreach ( CustomerViewModel item in lvUserctrl . listview1 . Items )
                        {
                            if ( item . CustNo == Custno && item . BankNo == Bankno )
                            {
                                IsLoadingDb = true;
                                lvUserctrl . listview1 . SelectedIndex = index;
                                Utils . ScrollLVRecordIntoView(lvUserctrl . listview1 , lvUserctrl . listview1 . SelectedIndex);
                                IsLoadingDb = false;
                                break;
                            }
                            index++;
                        }
                    }
                    catch ( Exception ex ) { Debug. WriteLine($"ListView Matching failed {ex . Message}, [{ex . Data}]"); }
                }
            }
            else if ( type == "DG" )
            {
                //                if ( dgUserctrl . grid1 . SelectedItem == null ) return;
                if ( dgUserctrl == null ) return;
                if ( dgUserctrl . grid1 . ItemsSource == null ) return;
                if ( dgUserctrl . CurrentType == "BANK" )
                {
                    try
                    {
                        foreach ( BankAccountViewModel item in dgUserctrl . grid1 . Items )
                        {
                            if ( item . CustNo == Custno && item . BankNo == Bankno )
                            {
                                IsLoadingDb = true;
                                dgUserctrl . grid1 . SelectedIndex = index;
                                dgUserctrl . grid1 . SelectedItem = index;
                                Utils . ScrollRowInGrid(dgUserctrl . grid1 , index);
                                IsLoadingDb = false;
                                break;
                            }
                            index++;
                        }
                    }
                    catch ( Exception ex ) { Debug. WriteLine($"DataGrid Matching failed {ex . Message}, [{ex . Data}]"); }
                }
                else
                {
                    // crashes here !!!!
                    try
                    {
                        foreach ( CustomerViewModel item in dgUserctrl . grid1 . Items )
                        {
                            if ( item . CustNo == Custno && item . BankNo == Bankno )
                            {
                                IsLoadingDb = true;
                                dgUserctrl . grid1 . SelectedIndex = index;
                                dgUserctrl . grid1 . SelectedItem = index;
                                Utils . ScrollRowInGrid(dgUserctrl . grid1 , dgUserctrl . grid1 . SelectedIndex);
                                IsLoadingDb = false;
                                break;
                            }
                            index++;
                        }
                    }
                    catch ( Exception ex ) { Debug. WriteLine($"DataGrid Matching failed {ex . Message}, [{ex . Data}]"); }
                }
            }
        }
        public async void wpfasyncTask ()
        {
            // Cannot access  UI thread
            Debug. WriteLine("Hellloo World");

            // This secton will run immediately & not block UI thread
            await Task . Run(async () =>
            {
                Debug. WriteLine("Starting task ");
                Debug. WriteLine($"Now on Thread {Thread . CurrentThread . ManagedThreadId}");
                for ( int i = 0 ; i < 100 ; i++ )
                {
                    Debug. WriteLine("looping");
                    Thread . Sleep(100);
                }
                Debug. WriteLine("Task ended ");
            });
        }
        public static void asynctask ()
        {
            Debug. WriteLine("Hellloo World");
            Debug. WriteLine($"Now on Thread {Thread . CurrentThread . ManagedThreadId}");

            // This secton will run immediately & not block UI thread
            // Cannot await it cos  methid is   not async - (see below)
            Task . Run(async () =>
            {
                Debug. WriteLine($"Now on Thread {Thread . CurrentThread . ManagedThreadId}");
                await Task . Delay(100);
                Debug. WriteLine("Starting task ");
                for ( int i = 0 ; i < 1000 ; i++ )
                {
                    Debug. WriteLine($"looping {i}");
                    Thread . Sleep(100);
                }
            });
            // This will NOT run unti task above has finshed
            Debug. WriteLine("Task ended ");

        }
        public static async void fullasynctask ()
        {
            Debug. WriteLine("Hellloo World");

            // This secton will run immediately & not block UI thread
            // this WILL await cos  method itself is async - (see above)
            await Task . Run(async () =>
            {
                Debug. WriteLine($"Now on Thread {Thread . CurrentThread . ManagedThreadId}");
                await Task . Delay(100);
                Debug. WriteLine("Starting task ");
                for ( int i = 0 ; i < 1000 ; i++ )
                {
                    Debug. WriteLine("looping");
                    Thread . Sleep(100);
                }
            });
            // This will NOT run unti task above has finshed
            Debug. WriteLine("Task ended ");

        }
        public void wpfasyncthread ()
        {
            // this works
            button . Content = "Helllo World";

            // This secton will run immediately & not block UI thread
            new Thread(() =>
            {
                Debug. WriteLine($"Now on Thread {Thread . CurrentThread . ManagedThreadId}");
                //This will Fail
                //button . Content = "Starting task ";
                for ( int i = 0 ; i < 100 ; i++ )
                {
                    // This will fail !
                    //          button . Content = "Starting task ";
                    Debug. WriteLine($"looping {i}");
                    Thread . Sleep(100);
                    //This will work
                    button . Dispatcher . Invoke(() =>
                    {
                        button . Content = "Starting task ";
                    });
                }
                Debug. WriteLine("Task ended ");
            }) . Start();

        }
        public async Task<bool> dolooping ()
        {
            for ( int i = 0 ; i < 100 ; i++ )
            {
                Debug. WriteLine("looping");
                Thread . Sleep(100);

                // this WILL fail without Dispatcher
                //button . Content = "dfdfdsg";
                // This will WORK & access button.Content
                button . Dispatcher . Invoke(() =>
                {
                    Debug. WriteLine($"Setting Button in Thread {Thread . CurrentThread . ManagedThreadId}");
                    button . Content = "dfdfdsg";
                });

                // This will WORK & access
                // button.Content on UI thread from here.....
                Application . Current . Dispatcher . Invoke(() =>
                {
                    Debug. WriteLine($"Now on Thread {Thread . CurrentThread . ManagedThreadId}");
                    button . Content = "dfdfdsg";
                });
            }
            return true;
        }
        //not    used
        private TabData loadothers (TabData tabdata)
        {
            var v = LoadListboxData(tabdata);
            return v;
        }
        //not    used
        private async Task<bool> LoadListviewData ()
        {
            bool success = true;
            //    await Task . Run ( ( ) =>
            //    {
            //        if ( lvUserctrl == null )
            //        {
            //            lvUserctrl = new LvUserControl ( );
            //            Debug. WriteLine ( $"Loading Db....." );
            //            //                    DataTemplate dt = Application . Current . FindResource ( "CustomersDbTemplate1" ) as DataTemplate;
            //            DataTemplate dt = Application . Current . FindResource ( "BankDataTemplate1" ) as DataTemplate;
            //            lvUserctrl . listview1 . ItemTemplate = dt;
            //            Thickness th = new Thickness ( );
            //            th = new Thickness ( );
            //            th . Left = 5;
            //            th . Top = 10;
            //            lvUserctrl . Margin = th;
            //            Tview . ListviewTab . Content = lvUserctrl;
            //            //lbUserctrl . UpdateLayout ( );
            //            lvUserctrl . CurrentType = "BANK";
            //            DbType = lvUserctrl?.CurrentType;
            //            lvUserctrl . LoadBank ( true );
            //            Mouse . OverrideCursor = Cursors . Arrow;
            //            lvUserctrl . UpdateLayout ( );
            //        }
            //    } );
            return success;
        }
        #endregion NOT USED
    }

}
