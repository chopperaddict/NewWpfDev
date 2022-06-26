using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Data . SqlClient;
using System . IO;
using System . Linq;
using System . Runtime . Serialization;
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
using System . Windows . Threading;

using NewWpfDev . SQL;
using NewWpfDev . ViewModels;
using NewWpfDev . Views;

using Newtonsoft . Json . Linq;

using NewWpfDev . SQL;
using NewWpfDev . ViewModels;
using NewWpfDev . Views;
using System . Diagnostics;

namespace NewWpfDev . UserControls
{
    /// <summary>
    /// Interaction logic for LbUserControl.xaml
    /// </summary>

    public partial class LbUserControl : UserControl
    {
        const string FileName = @"LbUserControl.bin";

        public object Viewmodel
        {
            get; set;
        }

        #region Data structure declarations
        public ObservableCollection<BankAccountViewModel> Bvm
        {
            get; private set;
        }
        public ObservableCollection<CustomerViewModel> Cvm
        {
            get; private set;
        }
        public ObservableCollection<GenericClass> Gvm
        {
            get; private set;
        }
        #endregion Data structure declarations

        #region variable declarations
        public string CurrentType
        {
            get; set;
        }
        //        public static TabWinViewModel Controller { get; set; }
        public static LbUserControl ThisWin
        {
            get; set;
        }
        private static Tabview tabviewWin
        {
            get; set;
        }
        public static string lbParent
        {
            get; set;
        }
        public bool SelectionInAction { get; set; } = false;
        public static bool TrackselectionChanges { get; set; } = false;
        private int CurrentIndex { get; set; } = 0;
        public int TipElapsedTime
        {
            get; set;
        }
        public string underlinetext { get; set; } = "Right Click " + "here" + " to view the Properties available";

        private const int MAXTOOLTIPSECS = 5;
        #endregion declarations

        #region Full Properties
        private bool isToolTipOpen;
        public bool IsToolTipOpen
        {
            get
            {
                return isToolTipOpen;
            }
            set
            {
                isToolTipOpen = value;
            }
        }
        private bool isToolTipClosed;
        public bool IsToolTipClosed
        {
            get
            {
                return isToolTipClosed;
            }
            set
            {
                isToolTipClosed = value;
            }
        }
        private bool useToolTip;
        public bool UseToolTip
        {
            get
            {
                return useToolTip;
            }
            set
            {
                useToolTip = value;
            }
        }
        #endregion Full Properties

        #region  Dependency properties for listbox

        #region ItemBackground
        public Brush ItemBackground
        {
            get
            {
                return ( Brush )GetValue(ItemBackgroundProperty);
            }
            set
            {
                SetValue(ItemBackgroundProperty , value);
            }
        }

        public static readonly DependencyProperty ItemBackgroundProperty =
                DependencyProperty . Register("ItemBackground" , typeof(Brush) , typeof(LbUserControl) , new PropertyMetadata(Brushes . LightBlue));
        #endregion ItemBackground

        #region ItemForeground
        public Brush ItemForeground
        {
            get
            {
                return ( Brush )GetValue(ItemForegroundProperty);
            }
            set
            {
                SetValue(ItemForegroundProperty , value);
            }
        }

        public static readonly DependencyProperty ItemForegroundProperty =
                DependencyProperty . Register("ItemForeground" , typeof(Brush) , typeof(LbUserControl) , new PropertyMetadata(Brushes . Black));
        #endregion ItemForeground

        #region SelectedBackground
        public Brush SelectedBackground
        {
            get
            {
                return ( Brush )GetValue(SelectedBackgroundProperty);
            }
            set
            {
                SetValue(SelectedBackgroundProperty , value);
            }
        }
        public static readonly DependencyProperty SelectedBackgroundProperty =
                DependencyProperty . Register("SelectedBackground" , typeof(Brush) , typeof(LbUserControl) , new PropertyMetadata(Brushes . Red));
        #endregion SelectedBackground

        #region SelectedForeground
        public Brush SelectedForeground
        {
            get
            {
                return ( Brush )GetValue(SelectedForegroundProperty);
            }
            set
            {
                SetValue(SelectedForegroundProperty , value);
            }
        }
        public static readonly DependencyProperty SelectedForegroundProperty =
            DependencyProperty . Register("SelectedForeground" , typeof(Brush) , typeof(LbUserControl) , new PropertyMetadata(Brushes . White));
        #endregion SelectedForeground

        #region MouseoverBackground
        public Brush MouseoverBackground
        {
            get
            {
                return ( Brush )GetValue(MouseoverBackgroundProperty);
            }
            set
            {
                SetValue(MouseoverBackgroundProperty , value);
            }
        }
        public static readonly DependencyProperty MouseoverBackgroundProperty =
                DependencyProperty . Register("MouseoverBackground" , typeof(Brush) , typeof(LbUserControl) , new PropertyMetadata(Brushes . Blue));
        #endregion MouseoverBackground

        #region MouseoverForeground
        public Brush MouseoverForeground
        {
            get
            {
                return ( Brush )GetValue(MouseoverForegroundProperty);
            }
            set
            {
                SetValue(MouseoverForegroundProperty , value);
            }
        }
        public static readonly DependencyProperty MouseoverForegroundProperty =
                DependencyProperty . Register("MouseoverForeground" , typeof(Brush) , typeof(LbUserControl) , new PropertyMetadata(Brushes . White));
        #endregion MouseoverForeground

        #region MouseoverSelectedBackground
        public Brush MouseoverSelectedBackground
        {
            get
            {
                return ( Brush )GetValue(MouseoverSelectedBackgroundProperty);
            }
            set
            {
                SetValue(MouseoverSelectedBackgroundProperty , value);
            }
        }
        public static readonly DependencyProperty MouseoverSelectedBackgroundProperty =
                DependencyProperty . Register("MouseoverSelectedBackground" , typeof(Brush) , typeof(LbUserControl) , new PropertyMetadata(Brushes . Black));
        #endregion MouseoverSelectedBackground

        #region MouseoverSelectedForeground
        public Brush MouseoverSelectedForeground
        {
            get
            {
                return ( Brush )GetValue(MouseoverSelectedForegroundProperty);
            }
            set
            {
                SetValue(MouseoverSelectedForegroundProperty , value);
            }
        }
        public static readonly DependencyProperty MouseoverSelectedForegroundProperty =
                DependencyProperty . Register("MouseoverSelectedForeground" , typeof(Brush) , typeof(LbUserControl) , new PropertyMetadata(Brushes . White));

        #endregion MouseoverSelectedForeground

        #endregion  Dependency properties for listbox

        public DispatcherTimer timer = new DispatcherTimer();

        private void LoadDb (object sender , LoadDbArgs e)
        {
            if ( e . dbname == "BANK" )
            {
                Task . Run(async () => await LoadBank(true));
            }
            else
                Task . Run(() => LoadCustomer(true));
        }

        //Constructor
        public LbUserControl ()
        {
            InitializeComponent();
            Debug. WriteLine($"'Creating 'LbUserControl' User Control  ......");
            ThisWin = this;

            // setup DP pointer in Tabview to LbUserControl using shortcut command line !
            Tabview . GetTabview() . Lbusercontrol = this;

            //Set ListBox  AP pointer in Tabview
            Tabview . SetListBox(this , listbox1);

            // setup local data collections
            Bvm = new ObservableCollection<BankAccountViewModel>();
            Cvm = new ObservableCollection<CustomerViewModel>();
            EventControl . BankDataLoaded += EventControl_BankDataLoaded;
            EventControl . CustDataLoaded += EventControl_CustDataLoaded;
            EventControl . ListSelectionChanged += SelectionHasChanged;
            TabWinViewModel . LoadDb += LoadDb;
            InterWinComms . Tooltipshown += LbUserControl_Tooltipshown;
            timer . Interval = new TimeSpan(0 , 0 , 1);
            timer . Tick += Timer_Tick;
            EventControl . TriggerWindowMessage(this , new InterWindowArgs { message = "LbUserControl  loaded..." , listbox = listbox1 });
            Debug. WriteLine("LbUserControl  loaded...");

            // Save to our ViewModel repository
            Viewmodel = new ViewModel();
            Viewmodel = this;
            ViewModel . SaveViewmodel("LbUserControl" , Viewmodel);
            //            listbox1.
            this . DataContext = this;
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
            TabWinViewModel . lbUserctrl = ( LbUserControl )deserializer . Deserialize(openFileStream);
            //TestLoan . TimeLastLoaded = DateTime . Now;
            openFileStream . Close();
        }
        public static void WriteSerializedObject ()
        {
            Stream SaveFileStream = File . Create(FileName);
            BinaryFormatter serializer = new BinaryFormatter();
            serializer . Serialize(SaveFileStream , TabWinViewModel . lbUserctrl . listbox1);
            SaveFileStream . Close();
        }

        private void UserControl_Loaded (object sender , RoutedEventArgs e)
        {
            //These get called EVERY time the control gets focus,
            //so need to be quite careful what is done here to avoid endless duplications.
        }
        public ListBox CreateListBox ()
        {
            // create list ot be shown on screen as  tooltip
            ListBox lb = new ListBox();
            lb . Name = "listbx1";
            lb . IsSynchronizedWithCurrentItem = true;
            Thickness th = new Thickness();
            th . Left = 5;
            th . Top = 5;
            //lb . Margin = "5 5 0 0"
            lb . Margin = th;
            lb . SelectionChanged += listbox1_SelectionChanged;
            lb . Background = FindResource("Cyan1") as SolidColorBrush;
            lb . Foreground = FindResource("Black0") as SolidColorBrush;
            lb . Height = LbGrid . ActualHeight;

            // Make a new Binding source.
            Binding binding = new Binding("Height");
            binding . Source = LbGrid . ActualHeight;
            // Bind the new data source to the myText TextBlock control's Text dependency property.
            lb . SetBinding(ListBox . HeightProperty , binding);

            // How  to set Dependecy Properties from c# = actually works to0
            DependencyObject dobj = new DependencyObject();
            dobj = new ListboxColorCtrlAP();
            dobj . SetValue(ListboxColorCtrlAP . ItemHeightProperty , ( double )29.0);
            ListboxColorCtrlAP . SetItemHeight(dobj , ( double )28.0);

            //How to set up triggers in c#
            Style styleListBoxItem = new Style(typeof(ListBoxItem));
            styleListBoxItem . Setters . Add(new Setter
            {
                Property = TextElement . FontSizeProperty ,
                Value = 18.0
            });
            styleListBoxItem . Setters . Add(new Setter
            {
                Property = TextElement . BackgroundProperty ,
                Value = FindResource("Blue2") as SolidColorBrush
            });

            styleListBoxItem . Setters . Add(new Setter
            {
                Property = TextElement . ForegroundProperty ,
                Value = Brushes . Black
            });
            lb . ItemContainerStyle = styleListBoxItem;

            lb . UpdateLayout();
            lb . Height = 200;
            lb . Width = 800;
            this . listbox1 . Visibility = Visibility . Visible;
            return lb;
        }

        private void EventControl_BankDataLoaded (object sender , LoadedEventArgs e)
        {
            if ( e . CallerType != "LbUserControl" ) return;
            this . listbox1 . ItemsSource = null;
            Grid v = listbox1 . Parent as Grid;
            TabItem y = v . Parent as TabItem;
            this . listbox1 . Items . Clear();
            Bvm = e . DataSource as ObservableCollection<BankAccountViewModel>;
            this . listbox1 . ItemsSource = Bvm;
            Debug. WriteLine($"Loaded Bank data in LbUserControl, REcords = {Bvm . Count}");
            CurrentType = "BANK";
            TabWinViewModel . TriggerDbType(CurrentType);
            DataTemplate dt = FindResource("BankDataTemplate1") as DataTemplate;
            this . listbox1 . ItemTemplate = dt;
            this . listbox1 . SelectedIndex = 0;
            this . listbox1 . SelectedItem = 0;
            this . listbox1 . UpdateLayout();

            Utils . ScrollLBRecordIntoView(this . listbox1 , 0);
            this . listbox1 . Refresh();
            Tabview . GetTabview() . TabSizeChanged(null , null);
            Mouse . OverrideCursor = Cursors . Arrow;
            DbCountArgs args = new DbCountArgs();
            args . Dbcount = Bvm?.Count ?? -1;
            args . sender = "dgUserctrl";
            TabWinViewModel . TriggerBankDbCount(this , args);
        }
        private void EventControl_CustDataLoaded (object sender , LoadedEventArgs e)
        {
            if ( e . CallerType != "LbUserControl" ) return;
            this . listbox1 . ItemsSource = null;
            this . listbox1 . Items . Clear();
            Cvm = e . DataSource as ObservableCollection<CustomerViewModel>;
            this . listbox1 . ItemsSource = Cvm;
            this . listbox1 . SelectedIndex = 0;
            CurrentType = "CUSTOMER";
            TabWinViewModel . TriggerDbType(CurrentType);
            DataTemplate dt = FindResource("CustomersDbTemplate1") as DataTemplate;
            this . listbox1 . ItemTemplate = dt;
            Utils . ScrollLBRecordIntoView(this . listbox1 , 0);
            Tabview . GetTabview() . TabSizeChanged(null , null);
            Mouse . OverrideCursor = Cursors . Arrow;
            DbCountArgs args = new DbCountArgs();
            args . Dbcount = Cvm?.Count ?? -1;
            args . sender = "dgUserctrl";
            TabWinViewModel . TriggerBankDbCount(this , args);
        }

        private void EventControl_GenericDataLoaded (object sender , LoadedEventArgs e)
        {
            if ( e . CallerType != "LISTBOX" ) return;
            this . listbox1 . ItemsSource = null;
            Grid v = this . listbox1 . Parent as Grid;
            TabItem y = v . Parent as TabItem;
            this . listbox1 . Items . Clear();
            Gvm = e . DataSource as ObservableCollection<GenericClass>;
            this . listbox1 . ItemsSource = Gvm;
            CurrentType = "GENERIC";
            TabWinViewModel . TriggerDbType(CurrentType);
            DataTemplate dt = FindResource("GenericTemplate") as DataTemplate;
            this . listbox1 . ItemTemplate = dt;
            this . listbox1 . SelectedIndex = 0;
            this . listbox1 . SelectedItem = 0;
            this . listbox1 . UpdateLayout();

            Utils . ScrollLBRecordIntoView(this . listbox1 , 0);
            this . listbox1 . Refresh();
            Mouse . OverrideCursor = Cursors . Arrow;
        }
        public static LbUserControl SetController (object ctrl)
        {
            tabviewWin = TabWinViewModel . SendTabview();
            return ThisWin;
        }

        public Task LoadBank (bool update)
        {
            CurrentType = "BANK";
            TabWinViewModel . TriggerDbType(CurrentType);
            Application . Current . Dispatcher . Invoke(( Action )( () =>
            {
                // does NOT use BankDataLoaded messaging
                Bvm = LoadBankDb(update);
                if ( Bvm != null && Bvm . Count > 0 )
                    listbox1 . ItemsSource = Bvm;
                Mouse . OverrideCursor = Cursors . Arrow;
            } ));
            return null;
            //ThreadPool . QueueUserWorkItem ( LoadBankDb, update);
        }
        public void LoadCustomer (bool update)
        {
            CurrentType = "CUSTOMER";
            TabWinViewModel . TriggerDbType(CurrentType);
            LoadCustomerDb(update);
        }

        public void LoadGeneric (string SqlCommand , bool update)
        {
            string ResultString = "";
            CurrentType = "GENERIC";
            TabWinViewModel . TriggerDbType(CurrentType);
            Gvm = SqlSupport . LoadGeneric(SqlCommand , out ResultString , 0 , true);
        }

        private ObservableCollection<BankAccountViewModel> LoadBankDb (object state)
        {
            Bvm = new ObservableCollection<BankAccountViewModel>();
            this . listbox1 . ItemsSource = null;
            this . listbox1 . Items . Clear();
            //Task task = Task . Run ( ( ) =>
            //{
            CurrentType = "BANK";
            TabWinViewModel . TriggerDbType(CurrentType);
            // uses BankDataLoadedto load data indirectly
            Bvm = UserControlDataAccess . GetBankObsCollection(true , "LbUserControl");
            //} );
            return Bvm;
        }
        private async void LoadCustomerDb (object state)
        {
            if ( Cvm == null ) Cvm = new ObservableCollection<CustomerViewModel>();
            if ( Cvm . Count == 0 )
            {
                Task task = Task . Run(async () =>
                {
                    // This is pretty fast - uses Dapper and Linq
                     UserControlDataAccess . GetCustObsCollection(Cvm , "" , true , "LbUserControl");
                });
            }
            else
            {
                this . listbox1 . ItemsSource = null;
                this . listbox1 . Items . Clear();
                Cvm . Clear();
                Task task = Task . Run(async () =>
                {
                    // This is pretty fast - uses Dapper and Linq
                     UserControlDataAccess . GetCustObsCollection(Cvm , "" , true , "LbUserControl");
                });
            }
            if ( ( bool )state == true )
            {
                this . listbox1 . ItemsSource = null;
                this . listbox1 . Items . Clear();
                this . listbox1 . ItemsSource = Cvm;
                CurrentType = "CUSTOMER";
                DataTemplate dt = FindResource("CustomersDbTemplate1") as DataTemplate;
                this . listbox1 . ItemTemplate = dt;
            }
            this . listbox1 . SelectedIndex = 0;
            this . listbox1 . SelectedItem = 0;
            Utils . ScrollLBRecordIntoView(this . listbox1 , 0);
        }
        private void listbox1_GotFocus (object sender , RoutedEventArgs e)
        {
        }
        private void listbox1_LostFocus (object sender , RoutedEventArgs e)
        {
        }
        private void listbox1_SelectionChanged (object sender , SelectionChangedEventArgs e)
        {
            if ( this . SelectionInAction )
            {
                Mouse . OverrideCursor = Cursors . Arrow;
                return;
            }
            ListBox lb = sender as ListBox;
            if ( lb == null ) return;
            if ( lb . Items . Count == 0 ) return;
            if ( this . listbox1 . SelectedIndex == -1 )
            {
                //make sure we have something selected !
                this . listbox1 . SelectedIndex = 0;
                this . listbox1 . SelectedItem = 0;
            }
            ListBox v = e . OriginalSource as ListBox;
            if ( v?.SelectedIndex != CurrentIndex )
            {
                CurrentIndex = v . SelectedIndex;
                if ( Tabview . GetTabview() . ViewersLinked )
                {
                    this . SelectionInAction = true;
                    SelectionChangedArgs args = new SelectionChangedArgs();
                    args . data = this . listbox1 . SelectedItem;
                    args . sendername = "listbox1";
                    args . sendertype = CurrentType;
                    args . index = this . listbox1 . SelectedIndex;
                    Debug. WriteLine($"ListBox broadcasting selection set to  {args . index}");
                    EventControl . TriggerListSelectionChanged(sender , args);
                    //                    this . SelectionInAction = false;
                }
                //                else this . SelectionInAction = false;
            }

            this . SelectionInAction = false;
            Mouse . OverrideCursor = Cursors . Arrow;
            e . Handled = true;
        }
        private void SelectionHasChanged (object sender , SelectionChangedArgs e)
        {
            bool success = false;
            if ( this . SelectionInAction == true )
                return;

            if ( LbUserControl . TrackselectionChanges == false ) return;
            // Another viewer has changed selection
            int newindex = 0;
            if ( this . listbox1 . ItemsSource == null ) return;

            if ( sender . GetType() == typeof(LbUserControl) )
                return;

            if ( e . sendername != "listbox1" )
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
                else if ( e . sendertype == "GENERIC" )
                {
                    // Sender is a GENERIC
                    //Cant notify other type of our selection
                    //GenericClass sourcerecord = new GenericClass ( );
                    //sourcerecord = e . data as GenericClass;
                    //custno = sourcerecord . CustNo;
                    //bankno = sourcerecord . BankNo;
                }
                if ( this . CurrentType == "CUSTOMER" )
                {
                    try
                    {
                        foreach ( CustomerViewModel item in this . listbox1 . Items )
                        {
                            if ( item . CustNo == custno && item . BankNo == bankno )
                            {
                                this . SelectionInAction = true;
                                this . listbox1 . SelectedIndex = newindex;
                                this . listbox1 . SelectedItem = newindex;
                                Debug. WriteLine($"Listbox selection in Customers matched on {custno}:{bankno}, index {newindex}");
                                Utils . ScrollLBRecordIntoView(this . listbox1 , newindex);
                                success = true;
                                break;
                            }
                            newindex++;
                        }
                    }
                    catch ( Exception ex ) { Debug. WriteLine($"Listbox failed search in Customer for match to {custno} : {bankno}"); }
                }
                else
                {
                    try
                    {
                        foreach ( BankAccountViewModel item in this . listbox1 . Items )
                        {
                            if ( item . CustNo == custno && item . BankNo == bankno )
                            {
                                this . SelectionInAction = true;
                                this . listbox1 . SelectedIndex = newindex;
                                this . listbox1 . SelectedItem = newindex;
                                Debug. WriteLine($"Listbox selection in BankAccount matched on {custno}:{bankno}, index {newindex}");
                                Utils . ScrollLBRecordIntoView(this . listbox1 , newindex);
                                success = true;
                                break;
                            }
                            newindex++;
                        }
                    }
                    catch ( Exception ex ) { Debug. WriteLine($"Listbox failed search in Bank for match to {custno} : {bankno}"); }
                }
                if ( success == false )
                    Debug. WriteLine($"Listbox failed search in Bank for match to {custno} : {bankno}");
                SelectionInAction = false;

                if ( success )
                    Utils . ScrollLBRecordIntoView(this . listbox1 , newindex);
                this . listbox1 . UpdateLayout();
            }
        }

        private void Button_Click (object sender , RoutedEventArgs e)
        {
            Clipboard . SetText(
                     $"You can set any or all of these properties to\n" +
                     $"Customise the listbox's overall appearance.\n\n" +
                 $"ItemBackground = (SolidColorBrush)\n" +
                 $"ItemForeground = (SolidColorBrush)\n" +
                 $"SelectedBackground = (SolidColorBrush)\n" +
                 $"SelectedForeground = (SolidColorBrush)\n" +
                 $"MouseoverBackground = (SolidColorBrush)\n" +
                 $"MouseoverForeground = (SolidColorBrush)\n" +
                 $"MouseoverSelectedBackground = SolidColorBrush\n" +
                 $"MouseoverSelectedForeground = SolidColorBrush\n");
            //          TooltipPopup . IsOpen = false;
        }

        private void ttSp_PreviewMouseRightButtonDown (object sender , MouseButtonEventArgs e)
        {
            if ( TipElapsedTime == 0 )
            {
                TooltipPopup . Visibility = Visibility . Visible;
                timer . Start();
            }
        }
        private void Hidepopup_Click (object sender , RoutedEventArgs e)
        {
            TooltipPopup . Visibility = Visibility . Collapsed;
            timer . Stop();
            TipElapsedTime = 0;
        }
        private void listbox1_MouseLeave (object sender , MouseEventArgs e)
        {
            if ( TipElapsedTime >= MAXTOOLTIPSECS )
            {
                TooltipPopup . Visibility = Visibility . Collapsed;
                timer . Stop();
                TipElapsedTime = 0;
            }
        }
        private void LbUserControl_Tooltipshown (object sender , TooltipArgs e)
        {
            //bool IsShown = e . IsOpen;
            //IsToolTipOpen = e . isShow & IsShown ;
            //TooltipPopup . IsOpen = IsToolTipOpen;
        }
        private void Timer_Tick (object sender , EventArgs e)
        {
            if ( TooltipPopup . Visibility == Visibility . Visible )
            {
                Debug. WriteLine(TipElapsedTime);
                if ( TipElapsedTime > MAXTOOLTIPSECS )
                {
                    TooltipPopup . Visibility = Visibility . Collapsed;
                    timer . Stop();
                    TipElapsedTime = 0;
                }
                else
                    TipElapsedTime++;
            }
        }

        public void PART_MouseLeave (object sender , MouseEventArgs e)
        {
            var tabview = TabWinViewModel . Tview;
            if ( TabWinViewModel . CurrentTabTextBlock == "Tab2Header" )
            {
                Tabview . TriggerStoryBoardOff(2);
                tabview . Tab2Header . FontSize = 14;
                tabview . Tab2Header . Foreground = FindResource("Cyan0") as SolidColorBrush;
            }
        }

        public void PART_MouseEnter (object sender , MouseEventArgs e)
        {
            var tabview = TabWinViewModel . Tview;
            Tabview . TriggerStoryBoardOn(2);
            tabview . Tab2Header . FontSize = 18;
            tabview . Tab2Header . Foreground = FindResource("Yellow0") as SolidColorBrush;
        }
        private void listbox1_PreviewMouseMove (object sender , MouseEventArgs e)
        {
            ListBox lbSender = sender as ListBox;
            if ( lbSender != null )
            {
                lbSender = sender as ListBox;
                if ( lbSender . Name == "listbox1" )
                {
                    TabWinViewModel . CurrentTabIndex = 1;
                    TabWinViewModel . CurrentTabName = "ListboxTab";
                    TabWinViewModel . CurrentTabTextBlock = "Tab2Header";
                }
            }
        }

        #region unused
        private void ttloaded (object sender , RoutedEventArgs e)
        {
            //ListBox lb = sender as ListBox;
            //StackPanel ttpi = lb . ToolTip as StackPanel;
            //if ( this . ttSp == null ) return;
            //if ( ShowToolTip == false )
            //    ttSp . Visibility = Visibility . Collapsed;
            //else
            //    ttSp . Visibility = Visibility . Visible;
        }

        public void GetObjectData (SerializationInfo info , StreamingContext context)
        {
            throw new NotImplementedException();
        }
        #endregion unused

    }
}

