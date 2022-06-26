using System;
using System . Collections;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . Globalization;
using System . IO;
using System . Linq;
using System . Runtime . Serialization . Formatters . Binary;
using System . Text;
using System . Threading;
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

using NewWpfDev . AttachedProperties;
using NewWpfDev . Converts;
using NewWpfDev . ViewModels;
using NewWpfDev . Views;

using NewWpfDev . Converts;
using NewWpfDev . UserControls;
using NewWpfDev . ViewModels;
using NewWpfDev . Views;
using System . Diagnostics;

namespace NewWpfDev . UserControls
{
    /// <summary>
    /// Interaction logic for LvUserControl.xaml
    /// </summary>
    [Serializable()]
    public partial class LvUserControl : UserControl
    {
        const string FileName = @"LvUserControl.bin";

        public object Viewmodel
        {
            get; set;
        }

        public ObservableCollection<BankAccountViewModel> Bvm
        {
            get; private set;
        }
        public ObservableCollection<CustomerViewModel> Cvm
        {
            get; private set;
        }
        public string CurrentType
        {
            get; set;
        }
        public TabControl tabControl
        {
            get; set;
        }
        private static LvUserControl ThisWin
        {
            get; set;
        }
        private static Tabview tabviewWin
        {
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
        protected virtual void OnListSelectionChanged (int index , object item , UIElement ctrl)
        {
            SelectionChangedArgs args = new SelectionChangedArgs();
            args . index = index;
            args . data = item;
            args . element = ctrl;
            args . sendername = CurrentType;
            if ( ListSelectionChanged != null )
                ListSelectionChanged(this , args);
        }

        #endregion events
        public LvUserControl ()
        {
            InitializeComponent();
            Debug. WriteLine($"Listview Control Loading ......");
            ThisWin = this;

            // setup DP pointer in Tabview to LvUserControl using shortcut command line !
            Tabview . GetTabview() . Lvusercontrol = this;

            //Set ListView AP pointer in Tabview
            Tabview . SetListView(this , listview1);

            // setup local data collections
            Bvm = new ObservableCollection<BankAccountViewModel>();
            Cvm = new ObservableCollection<CustomerViewModel>();
            //LbUserControl . LbSelectionChanged += SelectionHasChanged;
            //DgUserControl . DgSelectionChanged += SelectionHasChanged;
            EventControl . ListSelectionChanged += SelectionHasChanged;
            EventControl . BankDataLoaded += EventControl_BankDataLoaded;
            EventControl . CustDataLoaded += EventControl_CustDataLoaded;
            EventControl . TriggerWindowMessage(this , new InterWindowArgs { message = "LvUserControl  loaded..." });
            tabControl = Tabview . currenttab;
        }

        private void UserControl_Loaded (object sender , RoutedEventArgs e)
        {
        }
        public static void SetListSelectionChanged (bool arg)
        {
            TrackselectionChanges = arg;
        }
        private void ReadSerializedObject ()
        {
            Debug. WriteLine("Reading saved file");
            Stream openFileStream = File . OpenRead(FileName);
            BinaryFormatter deserializer = new BinaryFormatter();
            TabWinViewModel . dgUserctrl = ( DgUserControl )deserializer . Deserialize(openFileStream);
            //TestLoan . TimeLastLoaded = DateTime . Now;
            openFileStream . Close();
        }
        public static void WriteSerializedObject ()
        {
            Stream SaveFileStream = File . Create(FileName);
            BinaryFormatter serializer = new BinaryFormatter();
            serializer . Serialize(SaveFileStream , TabWinViewModel . dgUserctrl);
            SaveFileStream . Close();
        }

        private void EventControl_BankDataLoaded (object sender , LoadedEventArgs e)
        {
            //         if ( listview1 . Items . Count > 0 && CurrentType != "BANK") return;
            if ( e . CallerType != "LvUserControl" ) return;

            Bvm = e . DataSource as ObservableCollection<BankAccountViewModel>;
            if ( Bvm . Count == 0 ) return;
            // NB WE MUST reference  the usercontrol listview by using :-
            // listview1 , not via this.listview1
            listview1 . ItemsSource = Bvm;
            listview1 . SelectedIndex = 0;
            CurrentType = "BANK";
            TabWinViewModel . TriggerDbType(CurrentType);
            DataTemplate dt = FindResource("BankDataTemplate1") as DataTemplate;
            listview1 . ItemTemplate = dt;
            listview1 . SelectedIndex = 0;
            listview1 . SelectedItem = 0;
            //            Controller . DbCount = this . listview1 . Items . Count . ToString ( );
            listview1 . UpdateLayout();
            Mouse . OverrideCursor = Cursors . Arrow;
            Debug. WriteLine($"Bank data load via eventcontrol message !!!!!!");
            ListView lv = TabWinViewModel . lvUserctrl . listview1;
            ReduceByParamValue converter = new ReduceByParamValue();
            if ( tabControl != null )
            {
                listview1 . Height = TabWinViewModel . lvUserctrl . Height = Convert . ToDouble(converter . Convert(tabControl?.ActualHeight , typeof(double) , 40 , CultureInfo . CurrentCulture));
                listview1 . Width = TabWinViewModel . lvUserctrl . Width = Convert . ToDouble(converter . Convert(tabControl?.ActualWidth , typeof(double) , 10 , CultureInfo . CurrentCulture));
            }
            Utils . ScrollLVRecordIntoView(listview1 , 0);
            listview1 . Refresh();
            DbCountArgs args = new DbCountArgs();
            args . Dbcount = Bvm?.Count ?? -1;
            args . sender = "dgUserctrl";
            TabWinViewModel . TriggerBankDbCount(this , args);
            TabWinViewModel . TriggerDbType(CurrentType);
        }
        private void EventControl_CustDataLoaded (object sender , LoadedEventArgs e)
        {
            //            if ( listview1 . Items . Count > 0 && CurrentType != "CUSTOMER" ) return;
            if ( e . CallerType != "LvUserControl" ) return;

            Cvm = e . DataSource as ObservableCollection<CustomerViewModel>;
            this . listview1 . ItemsSource = Cvm;
            this . listview1 . SelectedIndex = 0;
            this . listview1 . ItemsSource = Cvm;
            CurrentType = "CUSTOMER";
            TabWinViewModel . TriggerDbType(CurrentType);
            DataTemplate dt = FindResource("CustomersDbTemplate1") as DataTemplate;
            this . listview1 . ItemTemplate = dt;
            this . listview1 . SelectedIndex = 0;
            this . listview1 . SelectedItem = 0;
            Utils . ScrollLVRecordIntoView(listview1 , 0);
            //Controller . DbCount = this . listview1 . Items . Count . ToString ( );
            tabviewWin . TabSizeChanged(null , null);
            Mouse . OverrideCursor = Cursors . Arrow;
            DbCountArgs args = new DbCountArgs();
            args . Dbcount = Cvm?.Count ?? -1;
            args . sender = "dgUserctrl";
            TabWinViewModel . TriggerBankDbCount(this , args);
            TabWinViewModel . TriggerDbType(CurrentType);
        }

        private void TabWinViewModel_SelectionHasChanged (object sender , SelectionChangedArgs e)
        {
            throw new NotImplementedException();
        }

        //private void LoadDb ( object sender , LoadDbArgs e )
        //{
        //    if ( e . dbname == "BANK" )
        //        LoadBank ( );
        //    else
        //        LoadCustomer ( );
        //}

        public static LvUserControl SetController (object ctrl)
        {
            //Controller = ctrl as TabWinViewModel;
            tabviewWin = TabWinViewModel . SendTabview();
            return ThisWin;
        }
        public Task LoadBank (bool arg = true)
        {
            //Task . Run ( ( ) => 
            LoadBankDb(arg);
            //LoadBankDb ( arg );
            if ( listview1 . ItemsSource == null )
            {
                if ( Bvm . Count > 0 )
                    listview1 . ItemsSource = Bvm;
            }
            return null;
            //ThreadPool . QueueUserWorkItem ( LoadBankDb );
        }

        public void LoadCustomer ()
        {
            LoadCustomerDb(true);
            //ThreadPool . QueueUserWorkItem ( LoadCustomerDb );
        }
        public void LoadCustomerDb (object data)
        {
            //            popup . IsOpen = true;
            this . listview1 . ItemsSource = null;
            this . listview1 . Items . Clear();
            if ( Cvm == null ) Cvm = new ObservableCollection<ViewModels . CustomerViewModel>();
            if ( Cvm . Count == 0 )
            {
                //                Cvm = ViewModels . CustomerViewModel . GetCustObsCollectionWithDict ( Cvm , "" , true );
                Task task = Task . Run(() =>
                {
                    // This is pretty fast - uses Dapper and Linq
                    UserControlDataAccess . GetCustObsCollection(Cvm , "" , true , "LvUserControl");
                });
            }
            else
            {
                Cvm . Clear();
                Task task = Task . Run(() =>
                {
                    // This is pretty fast - uses Dapper and Linq
                    UserControlDataAccess . GetCustObsCollection(Cvm , "" , true , "LvUserControl");
                });
                //                Cvm = ViewModels . CustomerViewModel . GetCustObsCollectionWithDict ( Cvm , "" , true );
            }
        }
        public async Task LoadBankDb (object data)
        {
            //Async workks well !!
            if ( Bvm == null || Bvm . Count == 0 )
            {
                Bvm = new ObservableCollection<ViewModels . BankAccountViewModel>();
                Task . Run(async () =>
                    Application . Current . Dispatcher . Invoke(( Action )( async () =>
                        Bvm = UserControlDataAccess . GetBankObsCollection(true , "LvUserControl")
                 )
                ));
            }
            else
            {
                Bvm . Clear();
                Task . Run(async () =>
                    Application . Current . Dispatcher . Invoke(( Action )( async () =>
                        Bvm = UserControlDataAccess . GetBankObsCollection(true , "LvUserControl")
                 )
                ));
            }
            return;
        }

        private void Loadcust (object sender , System . Windows . RoutedEventArgs e)
        {
            this . listview1 . ItemsSource = null;
            this . listview1 . Items . Clear();
            this . listview1 . ItemsSource = Cvm;
            this . listview1 . SelectedIndex = 0;
            this . listview1 . SelectedItem = 0;
            Utils . ScrollLVRecordIntoView(listview1 , 0);
        }
        private void listview1_GotFocus (object sender , RoutedEventArgs e)
        {
        }
        private void listview1_LostFocus (object sender , RoutedEventArgs e)
        {
        }

        public static void SetSelectionInAction (bool arg)
        {
            //         SelectionInAction = arg;
        }
        private void listview1_SelectionChanged (object sender , SelectionChangedEventArgs e)
        {
            if ( SelectionInAction == true )
                return;

            ListView lv = sender as ListView;
            if ( lv == null ) return;
            if ( lv . Items . Count == 0 ) return;
            if ( lv . SelectedIndex == -1 )
            {
                //make sure we have something selected !
                this . listview1 . SelectedIndex = 0;
                this . listview1 . SelectedItem = 0;
            }
            lv = e . OriginalSource as ListView;
            if ( lv . SelectedIndex != CurrentIndex )
            {
                SelectionInAction = true;
                this . listview1 . SelectedIndex = lv . SelectedIndex;
                CurrentIndex = lv . SelectedIndex;
                Utils . ScrollLVRecordIntoView(listview1 , CurrentIndex);
                if ( tabviewWin . ViewersLinked )
                {
                    SelectionChangedArgs args = new SelectionChangedArgs();
                    args . data = this . listview1 . SelectedItem;
                    args . sendername = "listview1";
                    args . sendertype = CurrentType;
                    args . index = CurrentIndex;
                    Debug. WriteLine($"ListView broadcasting selection set to  {args . index}");
                    SelectionInAction = false;
                    EventControl . TriggerListSelectionChanged(sender , args);
                }
            }

            SelectionInAction = false;
            Mouse . OverrideCursor = Cursors . Arrow;
            CurrentIndex = this . listview1 . SelectedIndex;
            e . Handled = true;
        }
        private void SelectionHasChanged (object sender , SelectionChangedArgs e)
        {
            bool success = false;
            if ( LvUserControl . TrackselectionChanges == false ) return;
            // Another viewer has changed selection
            int newindex = 0;
            if ( this . listview1 . ItemsSource == null ) return;

            if ( sender . GetType() == typeof(LvUserControl) )
                return;

            if ( e . sendername != "listview1" )
            {
                string custno = "", bankno = "";
                if ( e . sendertype == "BANK" )
                {
                    // Sender is a BANK
                    BankAccountViewModel sourcerecord = new BankAccountViewModel();
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
                    CustomerViewModel sourcerecord = new CustomerViewModel();
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
                        foreach ( CustomerViewModel item in this . listview1 . Items )
                        {
                            if ( item . CustNo == custno && item . BankNo == bankno )
                            {
                                SelectionInAction = true;
                                this . listview1 . SelectedIndex = newindex;
                                this . listview1 . SelectedItem = newindex;
                                Debug . WriteLine($"ListView selection in Customers matched on {custno}:{bankno}, index {newindex}");
                                Utils . ScrollLVRecordIntoView(listview1 , newindex);
                                tabviewWin . LoadName . Foreground = FindResource("Black0") as SolidColorBrush;
                                success = true;
                                break;
                            }
                            newindex++;
                        }
                    }
                    catch ( Exception ex )
                    {
                        Debug . WriteLine($"ListView failed search in Customer for match to {custno} : {bankno}");
                     }
                }
                else
                {
                    try
                    {
                        foreach ( BankAccountViewModel item in this . listview1 . Items )
                        {
                            if ( item . CustNo == custno && item . BankNo == bankno )
                            {
                                SelectionInAction = true;
                                this . listview1 . SelectedIndex = newindex;
                                this . listview1 . SelectedItem = newindex;
                                Debug . WriteLine($"ListView selection in BankAccount matched on {custno}:{bankno}, index {newindex}");
                                Utils . ScrollLVRecordIntoView(listview1 , newindex);
                                tabviewWin . LoadName . Foreground = FindResource("Black0") as SolidColorBrush;
                                success = true;
                                break;
                            }
                            newindex++;
                        }
                    }
                    catch ( Exception ex )
                    {
                        Debug . WriteLine($"XXXXXXListView failed search in Bank for match to {custno} : {bankno}");
                       }
                }
                if ( success == false )
                {
                    Debug . WriteLine($"ListView failed search for match to {custno} : {bankno}");
                    tabviewWin . LoadName . Text = $"ListView failed search for match to {custno} : {bankno}";
                    tabviewWin . LoadName . Foreground = FindResource("Red5") as SolidColorBrush;
                }
                else {
                    tabviewWin . LoadName . Text = $"ListView search for match to {custno} : {bankno} Succeeded";
                    tabviewWin . LoadName . Foreground = FindResource("Black0") as SolidColorBrush;
                }
            }
            if ( success )
                Utils . ScrollLVRecordIntoView(this . listview1 , newindex);
            SelectionInAction = false;
            this . listview1 . UpdateLayout();
        }
        public void PART_MouseLeave (object sender , MouseEventArgs e)
        {
            var tabview = TabWinViewModel . Tview;
            //TabItem  item = TabWinViewModel . CurrentTabitem;
            //Controller . SetCurrentTab ( tabview , TabWinViewModel . CurrentTabName );
            if ( TabWinViewModel . CurrentTabTextBlock == "Tab3Header" )
            {
                tabview . Tab3Header . FontSize = 14;
                Tabview . TriggerStoryBoardOff(3);
                tabview . Tab3Header . Foreground = FindResource("Cyan0") as SolidColorBrush;
            }
        }

        public void PART_MouseEnter (object sender , MouseEventArgs e)
        {
            var tabview = TabWinViewModel . Tview;
            //Point pt = e . GetPosition ( ( UIElement ) sender );
            //HitTestResult hit = VisualTreeHelper . HitTest ( ( Visual ) sender , pt );
            if ( TabWinViewModel . CurrentTabTextBlock == "Tab3Header" )
            {
                tabview . Tab3Header . FontSize = 18;
                Tabview . TriggerStoryBoardOn(3);
                tabview . Tab3Header . Foreground = FindResource("Yellow0") as SolidColorBrush;
            }
            this . listview1 . Focus();
        }
        private void listview1_PreviewMouseMove (object sender , MouseEventArgs e)
        {
            ListView lvSender = sender as ListView;
            if ( lvSender != null )
            {
                lvSender = sender as ListView;
                if ( lvSender . Name == "listview1" )
                {
                    TabWinViewModel . CurrentTabIndex = 2;
                    TabWinViewModel . CurrentTabName = "ListviewTab";
                    TabWinViewModel . CurrentTabTextBlock = "Tab3Header";
                }
            }
            listview1 . Focus();
        }

        private void listview1_PreviewMouseLeftButtonUp (object sender , MouseButtonEventArgs e)
        {
        }
    }
}
