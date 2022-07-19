using System;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Diagnostics;
using System . IO;
using System . Runtime . Serialization;
using System . Runtime . Serialization . Formatters . Binary;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Data;
using System . Windows . Documents;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Threading;

using NewWpfDev . Dapper;
using NewWpfDev . SQL;
using NewWpfDev . ViewModels;
using NewWpfDev . Views;

namespace NewWpfDev . UserControls {

    [Serializable()]
    public partial class LbUserControl : UserControl, ITabViewer {

        const string FileName = @"LbUserControl.bin";

        public object Viewmodel {
            get; set;
        }

        #region Data structure declarations
        public ObservableCollection<BankAccountViewModel> Bvm {
            get; private set;
        }
        public ObservableCollection<CustomerViewModel> Cvm {
            get; private set;
        }
        public ObservableCollection<GenericClass> Gvm {
            get; private set;
        }
        #endregion Data structure declarations

        #region OnPropertyChanged
        [field: NonSerialized ( )]
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged ( string propertyName ) {
            if ( PropertyChanged != null ) {
                PropertyChanged ( this , new PropertyChangedEventArgs ( propertyName ) );
            }
        }
        #endregion OnPropertyChanged

        #region variable declarations
        public string CurrentType {
            get; set;
        }
        //        public static TabWinViewModel Controller { get; set; }
        public static LbUserControl ThisWin {
            get; set;
        }
        private static Tabview tabviewWin {
            get; set;
        }
        public static string lbParent {
            get; set;
        }
        public static bool TrackselectionChanges { get; set; } = false;
        private int CurrentIndex { get; set; } = 0;
        public int TipElapsedTime {
            get; set;
        }
        public string underlinetext { get; set; } = "Right Click " + "here" + " to view the Properties available";

        private const int MAXTOOLTIPSECS = 5;
        #endregion declarations

        #region Full Properties
        protected bool SelectionInAction;
        private bool isToolTipOpen;
        private bool isToolTipClosed;
        private bool useToolTip;
        public bool IsToolTipOpen {
            get {
                return isToolTipOpen;
            }
            set {
                isToolTipOpen = value;
            }
        }
        public bool IsToolTipClosed {
            get {
                return isToolTipClosed;
            }
            set {
                isToolTipClosed = value; OnPropertyChanged ( nameof ( IsToolTipClosed ) );
            }
        }
        public bool UseToolTip {
            get => useToolTip;
            set => useToolTip = value;
        }
        #endregion Full Properties

        #region  Dependency View properties for listbox


        public double Fontsize {
            get => ( double ) GetValue ( FontsizeProperty );
            set => SetValue ( FontsizeProperty , value );
        }
        public static readonly DependencyProperty FontsizeProperty =
            DependencyProperty . Register ( "Fontsize" , typeof ( double ) , typeof ( LbUserControl ) , new PropertyMetadata ( ( double ) 16 ) );

        #region ItemBackground
        public Brush ItemBackground {
            get {
                return ( Brush ) GetValue ( ItemBackgroundProperty );
            }
            set {
                SetValue ( ItemBackgroundProperty , value );
            }
        }
        public static readonly DependencyProperty ItemBackgroundProperty =
                DependencyProperty . Register ( "ItemBackground" , typeof ( Brush ) , typeof ( LbUserControl ) , new PropertyMetadata ( Brushes . LightBlue ) );
        #endregion ItemBackground

        #region ItemForeground
        public Brush ItemForeground {
            get {
                return ( Brush ) GetValue ( ItemForegroundProperty );
            }
            set {
                SetValue ( ItemForegroundProperty , value );
            }
        }

        public static readonly DependencyProperty ItemForegroundProperty =
                DependencyProperty . Register ( "ItemForeground" , typeof ( Brush ) , typeof ( LbUserControl ) , new PropertyMetadata ( Brushes . Black ) );
        #endregion ItemForeground

        #region SelectedBackground
        public Brush SelectedBackground {
            get {
                return ( Brush ) GetValue ( SelectedBackgroundProperty );
            }
            set {
                SetValue ( SelectedBackgroundProperty , value );
            }
        }
        public static readonly DependencyProperty SelectedBackgroundProperty =
                DependencyProperty . Register ( "SelectedBackground" , typeof ( Brush ) , typeof ( LbUserControl ) , new PropertyMetadata ( Brushes . Red ) );
        #endregion SelectedBackground

        #region SelectedForeground
        public Brush SelectedForeground {
            get {
                return ( Brush ) GetValue ( SelectedForegroundProperty );
            }
            set {
                SetValue ( SelectedForegroundProperty , value );
            }
        }
        public static readonly DependencyProperty SelectedForegroundProperty =
            DependencyProperty . Register ( "SelectedForeground" , typeof ( Brush ) , typeof ( LbUserControl ) , new PropertyMetadata ( Brushes . White ) );
        #endregion SelectedForeground

        #region MouseoverBackground
        public Brush MouseoverBackground {
            get {
                return ( Brush ) GetValue ( MouseoverBackgroundProperty );
            }
            set {
                SetValue ( MouseoverBackgroundProperty , value );
            }
        }
        public static readonly DependencyProperty MouseoverBackgroundProperty =
                DependencyProperty . Register ( "MouseoverBackground" , typeof ( Brush ) , typeof ( LbUserControl ) , new PropertyMetadata ( Brushes . Blue ) );
        #endregion MouseoverBackground

        #region MouseoverForeground
        public Brush MouseoverForeground {
            get {
                return ( Brush ) GetValue ( MouseoverForegroundProperty );
            }
            set {
                SetValue ( MouseoverForegroundProperty , value );
            }
        }
        public static readonly DependencyProperty MouseoverForegroundProperty =
                DependencyProperty . Register ( "MouseoverForeground" , typeof ( Brush ) , typeof ( LbUserControl ) , new PropertyMetadata ( Brushes . White ) );
        #endregion MouseoverForeground

        #region MouseoverSelectedBackground
        public Brush MouseoverSelectedBackground {
            get {
                return ( Brush ) GetValue ( MouseoverSelectedBackgroundProperty );
            }
            set {
                SetValue ( MouseoverSelectedBackgroundProperty , value );
            }
        }
        public static readonly DependencyProperty MouseoverSelectedBackgroundProperty =
                DependencyProperty . Register ( "MouseoverSelectedBackground" , typeof ( Brush ) , typeof ( LbUserControl ) , new PropertyMetadata ( Brushes . Black ) );
        #endregion MouseoverSelectedBackground

        #region MouseoverSelectedForeground
        public Brush MouseoverSelectedForeground {
            get {
                return ( Brush ) GetValue ( MouseoverSelectedForegroundProperty );
            }
            set {
                SetValue ( MouseoverSelectedForegroundProperty , value );
            }
        }
        public static readonly DependencyProperty MouseoverSelectedForegroundProperty =
                DependencyProperty . Register ( "MouseoverSelectedForeground" , typeof ( Brush ) , typeof ( LbUserControl ) , new PropertyMetadata ( Brushes . White ) );

        #endregion MouseoverSelectedForeground

        #endregion  Dependency properties for listbox

        public DispatcherTimer timer = new DispatcherTimer ( );

        private void LoadDb ( object sender , LoadDbArgs e ) {
            if ( e . dbname == "BANK" ) {
                Task . Run ( async ( ) => await LoadBank ( true ) );
            }
            else
                Task . Run ( async ( ) => await LoadCustomer ( true ) );
        }

        //Constructor
        public LbUserControl ( ) {
            InitializeComponent ( );
            ThisWin = this;
            Tabview . Tabcntrl . lbUserctrl = this;
            SelectionInAction = false;
            // setup DP pointer in Tabview to LbUserControl using shortcut command line !
            Tabview . GetTabview ( ) . Lbusercontrol = this;
            Fontsize = 14;
            listbox1 . FontSize = Fontsize; ;
            // setup local data collections
            Bvm = new ObservableCollection<BankAccountViewModel> ( );
            Cvm = new ObservableCollection<CustomerViewModel> ( );
            EventControl . BankDataLoaded += EventControl_BankDataLoaded;
            EventControl . CustDataLoaded += EventControl_CustDataLoaded;
            EventControl . GenDataLoaded += EventControl_GenericDataLoaded;
            EventControl . ListSelectionChanged += SelectionHasChanged;
            TabWinViewModel . LoadDb += LoadDb;
            InterWinComms . Tooltipshown += LbUserControl_Tooltipshown;            
            timer . Interval = new TimeSpan ( 0 , 0 , 1 );
            timer . Tick += Timer_Tick;
            this . DataContext = this;
            Gvm = ITabViewer . Gvm;
             
        }
        public static void SetListSelectionChanged ( bool arg ) {
            TrackselectionChanges = arg;
        }
        private void ReadSerializedObject ( ) {
            Debug . WriteLine ( "Reading saved file" );
            Stream openFileStream = File . OpenRead ( FileName );
            BinaryFormatter deserializer = new BinaryFormatter ( );
            Tabview . Tabcntrl . lbUserctrl = ( LbUserControl ) deserializer . Deserialize ( openFileStream );
            openFileStream . Close ( );
        }
        //public static void WriteSerializedObject ( ) {
        //    Stream SaveFileStream = File . Create ( FileName );
        //    BinaryFormatter serializer = new BinaryFormatter ( );
        //    serializer . Serialize ( SaveFileStream , Tabview . Tabcntrl . lbUserctrl . listbox1 );
        //    SaveFileStream . Close ( );
        //}

        private void UserControl_Loaded ( object sender , RoutedEventArgs e ) {
            //These get called EVERY time the control gets focus,
            //so need to be quite careful what is done here to avoid endless duplications.
        }
        public ListBox CreateListBox ( ) {
            // create list ot be shown on screen as  tooltip
            ListBox lb = new ListBox ( );
            lb . Name = "listbx1";
            lb . IsSynchronizedWithCurrentItem = true;
            Thickness th = new Thickness ( );
            th . Left = 5;
            th . Top = 5;
            //lb . Margin = "5 5 0 0"
            lb . Margin = th;
            lb . SelectionChanged += listbox1_SelectionChanged;
            lb . Background = FindResource ( "Cyan1" ) as SolidColorBrush;
            lb . Foreground = FindResource ( "Black0" ) as SolidColorBrush;
            lb . Height = LbGrid . ActualHeight;

            // Make a new Binding source.
            Binding binding = new Binding ( "Height" );
            binding . Source = LbGrid . ActualHeight;
            // Bind the new data source to the myText TextBlock control's Text dependency property.
            lb . SetBinding ( ListBox . HeightProperty , binding );

            // How  to set Dependecy Properties from c# = actually works to0
            DependencyObject dobj = new DependencyObject ( );
            dobj = new ListboxColorCtrlAP ( );
            dobj . SetValue ( ListboxColorCtrlAP . ItemHeightProperty , ( double ) 29.0 );
            ListboxColorCtrlAP . SetItemHeight ( dobj , ( double ) 28.0 );

            //How to set up triggers in c#
            Style styleListBoxItem = new Style ( typeof ( ListBoxItem ) );
            styleListBoxItem . Setters . Add ( new Setter {
                Property = TextElement . FontSizeProperty ,
                Value = 18.0
            } );
            styleListBoxItem . Setters . Add ( new Setter {
                Property = TextElement . BackgroundProperty ,
                Value = FindResource ( "Blue2" ) as SolidColorBrush
            } );

            styleListBoxItem . Setters . Add ( new Setter {
                Property = TextElement . ForegroundProperty ,
                Value = Brushes . Black
            } );
            lb . ItemContainerStyle = styleListBoxItem;

            lb . UpdateLayout ( );
            lb . Height = 200;
            lb . Width = 800;
            this . listbox1 . Visibility = Visibility . Visible;
            return lb;
        }

        #region Bank Data Loading methods
        public async Task LoadBank ( bool update = true ) {
            //Bank button handler
            //            WpfLib1 . Utils . IsReferenceEqual ( Tabview . Tabcntrl . lbUserctrl. listbox1, this.listbox1, "Tabview . Tabcntrl . lbUserctrl. listbox1" , "this.listbox1" , true );
            //WpfLib1 . Utils . IsHashEqual ( Tabview . Tabcntrl . lbUserctrl . listbox1 , this . listbox1 , "Tabview . Tabcntrl . lbUserctrl. listbox1" , "this.listbox1" , true ); 
            Application . Current . Dispatcher . Invoke ( async ( ) => {
                await DispatcherExtns . SwitchToUi ( Application . Current . Dispatcher );
                Debug . WriteLine ( $"\nInside Dispatcher" );
                $"Dispatcher on UI thread =  {Application . Current . Dispatcher . CheckAccess ( )}" . CW ( );
                CurrentType = "BANK";
                TabWinViewModel . TriggerDbType ( CurrentType );
                Tabview . Tabcntrl . ActiveControlType = Tabview . Tabcntrl . lbUserctrl;
                Tabview . Tabcntrl . DtTemplates . TemplateNameLb = "BANKACCOUNT";
                Tabview . Tabcntrl . CurrentTypeLb = "BANKACCOUNT";
                Tabview . SetDbType ( "BANK" );
                // now load the data
                await Task . Run ( async ( ) => {
                    // does NOT use BankDataLoaded messaging
                    await LoadBankDb ( update );
                } );
                Mouse . OverrideCursor = Cursors . Arrow;
                return;
            } );
        }
        private async Task LoadBankDb ( bool notify ) {
            //Works  well - Fast too 1//7/2022
            Application . Current . Dispatcher . Invoke ( async ( ) => {
                await DispatcherExtns . SwitchToUi ( Application . Current . Dispatcher );
                Debug . WriteLine ( $"\nInside Dispatcher" );
                $"Dispatcher on UI thread =  {Application . Current . Dispatcher . CheckAccess ( )}" . CW ( );
                Bvm = new ObservableCollection<BankAccountViewModel> ( );
            this . listbox1 . ItemsSource = null;
            this . listbox1 . Items . Clear ( );
            CurrentType = "BANK";
                
                // Set colors of Indicator panels on Tabview
                Tabview . tabvw . DbTypeFld . Background = FindResource ( "Blue5" ) as SolidColorBrush;
            Tabview . tabvw . DbCount . Background = Application . Current . FindResource ( "Blue5" ) as SolidColorBrush;
            
                TabWinViewModel . TriggerDbType ( CurrentType );
            // uses BankDataLoadedto load data indirectly
            int [ ] args = { 0 , 0 , 0 };
            await Task . Run (async  ( ) => {
                DapperSupport . GetBankObsCollectionAsync ( Bvm , "" , "BankAccount" , "" , "" , false , false , notify , "LbUserControl" , args );
            } );
            } );
            return;
        }

        private void EventControl_BankDataLoaded ( object sender , LoadedEventArgs e ) {
            if ( e . CallerDb != "LbUserControl" ) return;
            Application . Current . Dispatcher . Invoke ( async ( ) => {
                await DispatcherExtns . SwitchToUi ( Application . Current . Dispatcher );
                Debug . WriteLine ( $"\nInside Dispatcher" );
                $"Dispatcher on UI thread =  {Application . Current . Dispatcher . CheckAccess ( )}" . CW ( );
                this . listbox1 . ItemsSource = null;
                this . listbox1 . Items . Clear ( );
                Bvm = e . DataSource as ObservableCollection<BankAccountViewModel>;
                this . listbox1 . ItemsSource = Bvm;
                Debug . WriteLine ( ( Bvm . Count > 0 ? $"Received {Bvm . Count} " : "Failed to receive " ) + "BankAccount Table Records in LbUserControl" );
                CurrentType = "BANK";
                TabWinViewModel . TriggerDbType ( CurrentType );
                // load / Reload theTemplates Combo & set to index 0
                Tabview . Tabcntrl . DtTemplates . TemplatesCombo . ItemsSource = Tabview . DataTemplatesBank;
                Tabview . Tabcntrl . DtTemplates . TemplateIndexLb = 0;
                Tabview . Tabcntrl . tabView . IsLoading = true;
                Tabview . Tabcntrl . tabView . DbnamesCb . SelectedIndex = Tabview . FindDbName ( "BANKACCOUNT" );
                Tabview . Tabcntrl . DbNameIndexLb = Tabview . Tabcntrl . tabView . DbnamesCb . SelectedIndex;
                Tabview . Tabcntrl . DbNameLb = Tabview . Tabcntrl . tabView . DbnamesCb . SelectedItem . ToString ( ) . ToUpper ( );
                Tabview . Tabcntrl . tabView . IsLoading = false;
                Tabview . Tabcntrl . DtTemplates . TemplatesCombo . SelectedIndex = 0;
                // Set ListBox to the new Data Template
                FrameworkElement elemnt = Tabview . Tabcntrl . lbUserctrl . listbox1 as FrameworkElement;
                DataTemplate dtemp = new DataTemplate ( );
                // Lock template  - cannot be changed
                dtemp . Seal ( );
                dtemp = elemnt . FindResource ( Tabview . Tabcntrl . DtTemplates . TemplatesCombo . SelectedItem . ToString ( ) ) as DataTemplate;
                Tabview . Tabcntrl . lbUserctrl . listbox1 . ItemTemplate = dtemp;
                Tabview . Tabcntrl . DtTemplates . TemplatesCombo . SelectedIndex = Tabview . Tabcntrl . DtTemplates . TemplateIndexLb;
                Tabview . Tabcntrl . lbUserctrl . listbox1 . UpdateLayout ( );

                Tabview . Tabcntrl . tabView . TemplatesCb . Foreground = FindResource ( "Black0" ) as SolidColorBrush;
                Tabview . Tabcntrl . tabView . TemplatesCb . UpdateLayout ( );
                Tabview . Tabcntrl . tabView . DbnamesCb . Foreground = FindResource ( "Black0" ) as SolidColorBrush;
                Tabview . Tabcntrl . tabView . DbnamesCb . UpdateLayout ( );

                Tabview . Tabcntrl . twVModel . ProgressValue = 0;
                Tabview . Tabcntrl . tabView . ProgressBar_Progress . UpdateLayout ( );
                Utils . ScrollLBRecordIntoView ( this . listbox1 , 0 );
                this . listbox1 . Refresh ( );
                Tabview . GetTabview ( ) . TabSizeChanged ( null , null );
                Mouse . OverrideCursor = Cursors . Arrow;
                DbCountArgs args = new DbCountArgs ( );
                args . Dbcount = Bvm?.Count ?? -1;
                args . sender = "lbUserctrl";

                TabWinViewModel . TriggerBankDbCount ( this , args );
            } );
            Debug . WriteLine ( $"Exited Dispatcher\n" );
        }

        #endregion Bank Data Loading methods

        private void EventControl_CustDataLoaded ( object sender , LoadedEventArgs e ) {
            if ( e . CallerType != "LbUserControl" ) return;
            Application . Current . Dispatcher . Invoke ( async ( ) => {
                await DispatcherExtns . SwitchToUi ( Application . Current . Dispatcher );
                Debug . WriteLine ( $"\nInside Dispatcher" );
                $"Dispatcher on UI thread =  {Application . Current . Dispatcher . CheckAccess ( )}" . CW ( );
                this . listbox1 . ItemsSource = null;
                this . listbox1 . Items . Clear ( );
                Cvm = e . DataSource as ObservableCollection<CustomerViewModel>;
                this . listbox1 . ItemsSource = Cvm;
                this . listbox1 . SelectedIndex = 0;
                CurrentType = "CUSTOMER";
                TabWinViewModel . TriggerDbType ( CurrentType );
                Debug . WriteLine ( ( Bvm . Count > 0 ? $"Received {Cvm . Count} " : "Failed to receive " ) + "Cuustomer Table Records in LbUserControl" );

                // load / Reload theTemplates Combo & set to index 0
                Tabview . Tabcntrl . DtTemplates . TemplatesCombo . ItemsSource = Tabview . DataTemplatesCust;
                Tabview . Tabcntrl . DtTemplates . TemplateIndexLb = 0;
                Tabview . Tabcntrl . tabView . IsLoading = true;
                Tabview . Tabcntrl . tabView . DbnamesCb . SelectedIndex = Tabview . FindDbName ( "CUSTOMER" );
                Tabview . Tabcntrl . DbNameIndexLb = Tabview . Tabcntrl . tabView . DbnamesCb . SelectedIndex;
                Tabview . Tabcntrl . DbNameLb = Tabview . Tabcntrl . tabView . DbnamesCb . SelectedItem . ToString ( ) . ToUpper ( );
                Tabview . Tabcntrl . tabView . IsLoading = false;
                Tabview . Tabcntrl . DtTemplates . TemplatesCombo . SelectedIndex = 0;
                // Set Data Template
                FrameworkElement elemnt = Tabview . Tabcntrl . lbUserctrl . listbox1 as FrameworkElement;
                DataTemplate dtemp = new DataTemplate ( );
                // Lock template  - cannot be changed
                dtemp . Seal ( );
                dtemp = elemnt . FindResource ( Tabview . Tabcntrl . DtTemplates . TemplatesCombo . SelectedItem . ToString ( ) ) as DataTemplate;
                Tabview . Tabcntrl . lbUserctrl . listbox1 . ItemTemplate = dtemp;
                Tabview . Tabcntrl . DtTemplates . TemplatesCombo . SelectedIndex = 0;
                Tabview . Tabcntrl . lbUserctrl . listbox1 . UpdateLayout ( );

                Tabview . Tabcntrl . tabView . TemplatesCb . Foreground = FindResource ( "Black0" ) as SolidColorBrush;
                Tabview . Tabcntrl . tabView . TemplatesCb . UpdateLayout ( );
                Tabview . Tabcntrl . tabView . DbnamesCb . Foreground = FindResource ( "Black0" ) as SolidColorBrush;
                Tabview . Tabcntrl . tabView . DbnamesCb . UpdateLayout ( );
                Utils . ScrollLBRecordIntoView ( this . listbox1 , 0 );
                Tabview . GetTabview ( ) . TabSizeChanged ( null , null );
                Mouse . OverrideCursor = Cursors . Arrow;
                DbCountArgs args = new DbCountArgs ( );
                args . Dbcount = Cvm?.Count ?? -1;
                args . sender = "lbUserctrl";
                TabWinViewModel . TriggerBankDbCount ( this , args );
            } );
            Debug . WriteLine ( $"Exited Dispatcher\n" );
        }

        private void EventControl_GenericDataLoaded ( object sender , LoadedEventArgs e ) {
            if ( e . CallerType != "GENERICDATA" ) return;
            this . listbox1 . ItemsSource = null;
            Grid v = this . listbox1 . Parent as Grid;
            TabItem y = v . Parent as TabItem;
            this . listbox1 . Items . Clear ( );
            Gvm = e . DataSource as ObservableCollection<GenericClass>;
            this . listbox1 . ItemsSource = Gvm;
            CurrentType = "GENERIC";
            TabWinViewModel . TriggerDbType ( CurrentType );
            DataTemplate dt = FindResource ( "GenericTemplate" ) as DataTemplate;
            this . listbox1 . ItemTemplate = dt;
            this . listbox1 . SelectedIndex = 0;
            this . listbox1 . SelectedItem = 0;
            this . listbox1 . UpdateLayout ( );
            Tabview . Tabcntrl . tabView . DbnamesCb . SelectedIndex = Tabview . FindDbName ( Tabview . Tabcntrl . tabView . DbnamesCb . SelectedItem . ToString ( ) );
            Tabview . Tabcntrl . DbNameIndexLb = Tabview . Tabcntrl . tabView . DbnamesCb . SelectedIndex;
            Tabview . Tabcntrl . DbNameLb = Tabview . Tabcntrl . tabView . DbnamesCb . SelectedItem . ToString ( ) . ToUpper ( );
            
            // Set colors of Indicator panels on Tabview
            Tabview . tabvw . DbTypeFld . Background = FindResource ( "Red5" ) as SolidColorBrush;
            Tabview . tabvw . DbCount . Background = Application . Current . FindResource ( "Red5" ) as SolidColorBrush;
            Tabview . tabvw . DbTypeFld . UpdateLayout ( );
            Tabview . Tabcntrl . tabView . TemplatesCb . Foreground = FindResource ( "Red5" ) as SolidColorBrush;
            Tabview . Tabcntrl . tabView . TemplatesCb . UpdateLayout ( );
            Tabview . Tabcntrl . tabView . DbnamesCb . Foreground = FindResource ( "Red5" ) as SolidColorBrush;
            Tabview . Tabcntrl . tabView . DbnamesCb . UpdateLayout ( );
            
            Utils . ScrollLBRecordIntoView ( this . listbox1 , 0 );
            this . listbox1 . Refresh ( );
            Mouse . OverrideCursor = Cursors . Arrow;
        }
        public static LbUserControl SetController ( object ctrl ) {
            tabviewWin = TabWinViewModel . SendTabview ( );
            return ThisWin;
        }

        public async Task LoadCustomer ( bool update = true ) {
            CurrentType = "CUSTOMER";
            TabWinViewModel . TriggerDbType ( CurrentType );
            Tabview . Tabcntrl . ActiveControlType = Tabview . Tabcntrl . lbUserctrl;
            Tabview . Tabcntrl . DtTemplates . TemplateNameLb = "CUSTOMER";
            Tabview . Tabcntrl . CurrentTypeLb = "CUSTOMER";
            Tabview . SetDbType ( "CUSTOMER" );
            
            // Set colors of Indicator panels on Tabview
            Tabview . tabvw . DbTypeFld . Background = FindResource ( "Blue5" ) as SolidColorBrush;
            Tabview . tabvw . DbCount . Background = Application . Current . FindResource ( "Blue5" ) as SolidColorBrush;
            
            await LoadCustomerDb ( update );
            Mouse . OverrideCursor = Cursors . Arrow;
            return;
        }

        public int LoadGeneric ( string tablename ) {
            string ResultString = "";
            string SqlCommand = tablename != null ? $"Select * from {tablename}" : "Select * from Invoice";
            Tabview . Tabcntrl . ActiveControlType = Tabview . Tabcntrl . lbUserctrl;
            //Setup Templates list  as we are changing Db type
            Tabview . Tabcntrl . DtTemplates . TemplateNameLb = "GEN";
            Tabview . Tabcntrl . CurrentTypeLb = "GEN";
            Tabview . SetDbType ( "GEN" );

            Tabview . Tabcntrl . lbUserctrl . listbox1 . ItemsSource = null;
            Tabview . Tabcntrl . lbUserctrl . listbox1 . Items . Clear ( );
            //Gvm = new ObservableCollection<GenericClass> ( );
            Gvm = SqlSupport . LoadGeneric ( SqlCommand , out ResultString , 0 , false );
            Tabview . Tabcntrl . lbUserctrl . listbox1 . ItemsSource = Gvm;
            DbCountArgs args = new DbCountArgs ( );
            args . Dbcount = Gvm?.Count ?? -1;
            args . sender = "lbUserctrl";
            TabWinViewModel . TriggerBankDbCount ( this , args );

            // Set ListBox to the new Data Template
            Tabview . Tabcntrl . twVModel . CheckActiveTemplate ( Tabview . Tabcntrl . lbUserctrl );
            FrameworkElement elemnt = Tabview . Tabcntrl . lbUserctrl . listbox1 as FrameworkElement;
            DataTemplate dtemp = new DataTemplate ( );
            // Lock template  - cannot be changed
            dtemp . Seal ( );
            dtemp = elemnt . FindResource ( Tabview . Tabcntrl . DtTemplates . TemplatesCombo . SelectedItem . ToString ( ) ) as DataTemplate;
            Tabview . Tabcntrl . lbUserctrl . listbox1 . ItemTemplate = dtemp;
            Tabview . Tabcntrl . DtTemplates . TemplatesCombo . SelectedIndex = Tabview . Tabcntrl . DtTemplates . TemplateIndexLb;
            Tabview . Tabcntrl . lbUserctrl . listbox1 . UpdateLayout ( );
            
            // Set colors of Indicator panels on Tabview
            Tabview . tabvw . DbTypeFld . Background = FindResource ( "Red5" ) as SolidColorBrush;
            Tabview . tabvw . DbCount . Background = Application . Current . FindResource ( "Red5" ) as SolidColorBrush;
            
            return Gvm . Count;
        }


        private async Task LoadCustomerDb ( object state ) {
            if ( Cvm == null ) Cvm = new ObservableCollection<CustomerViewModel> ( );
            if ( Cvm . Count == 0 ) {
                Task task = Task . Run ( async ( ) => {
                    // This is pretty fast - uses Dapper and Linq
                    Application . Current . Dispatcher . Invoke ( ( Action ) ( async ( ) => {
                        UserControlDataAccess . GetCustObsCollection ( Cvm , "" , true , "LbUserControl" );
                    } ) );
                } );
            }
            else {
                this . listbox1 . ItemsSource = null;
                this . listbox1 . Items . Clear ( );
                Cvm . Clear ( );
                Task task = Task . Run ( async ( ) => {
                    // This is pretty fast - uses Dapper and Linq
                    UserControlDataAccess . GetCustObsCollection ( Cvm , "" , true , "LbUserControl" );
                } );
            }
            if ( ( bool ) state == true ) {
                this . listbox1 . ItemsSource = null;
                this . listbox1 . Items . Clear ( );
                this . listbox1 . ItemsSource = Cvm;
                CurrentType = "CUSTOMER";
                DataTemplate dt = FindResource ( "CustomersDbTemplate1" ) as DataTemplate;
                this . listbox1 . ItemTemplate = dt;
            }
            this . listbox1 . SelectedIndex = 0;
            this . listbox1 . SelectedItem = 0;
            Utils . ScrollLBRecordIntoView ( this . listbox1 , 0 );
        }
        private void listbox1_GotFocus ( object sender , RoutedEventArgs e ) {
        }
        private void listbox1_LostFocus ( object sender , RoutedEventArgs e ) {
        }
        private void listbox1_SelectionChanged ( object sender , SelectionChangedEventArgs e ) {
            ListBox lb = sender as ListBox;

            if ( Tabview . Tabcntrl . Selectionchanged == true ) return;
            if ( lb == null ) return;
            if ( lb . Items . Count == 0 ) return;
            if ( this . listbox1 . SelectedIndex == -1 ) {
                //make sure we have something selected !
                this . listbox1 . SelectedIndex = 0;
                this . listbox1 . SelectedItem = 0;
            }
            ListBox v = e . OriginalSource as ListBox;
            if ( v?.SelectedIndex != CurrentIndex && Tabview . Tabcntrl . Selectionchanged == false ) {
                if ( Tabview . Tabcntrl . CurrentTypeLb != "GEN" ) {
                    CurrentIndex = v . SelectedIndex;
                    if ( Tabview . Tabcntrl . tabView . ViewersLinked ) {
                        if ( Tabview . Tabcntrl . tabView . CheckAllControlIndexes ( CurrentIndex ) == false ) {
                            Tabview . Tabcntrl . Selectionchanged = true;
                            SelectionChangedArgs args = new SelectionChangedArgs ( );
                            args . data = this . listbox1 . SelectedItem;
                            args . sendername = "listbox1";
                            args . sendertype = CurrentType;
                            args . index = this . listbox1 . SelectedIndex;
                            Debug . WriteLine ( $"ListBox broadcasting selection to set to  {args . index}" );
                            Task task = Task . Run ( async ( ) => {
                                Application . Current . Dispatcher . Invoke ( ( ) => {
                                    EventControl . TriggerListSelectionChanged ( sender , args );
                                } );
                            } );
                            //                           e . Handled = true;
                        }
                    }
                }
            }
            Mouse . OverrideCursor = Cursors . Arrow;
            Tabview . Tabcntrl . Selectionchanged = false;
            e . Handled = true;
        }

        private void SelectionHasChanged ( object sender , SelectionChangedArgs e ) {
            bool success = false;
            if ( SelectionInAction == true )
                return;
            if ( LbUserControl . TrackselectionChanges == false ) return;
            // Another viewer has changed selection
            int newindex = 0;
            if ( this . listbox1 . ItemsSource == null ) return;

            if ( sender . GetType ( ) == typeof ( LbUserControl ) )
                return;
            if ( Tabview . Tabcntrl . DtTemplates . TemplateNameLb == "GEN" ) {
                Debug . WriteLine ( $"ListBox  : Cannot match : List  contains Generic data" );
                return;
            }

            if ( e . sendername != "listbox1" ) {
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
                else if ( e . sendertype == "GENERIC" ) {
                    // Sender is a GENERIC
                    //Cant notify other type of our selection
                    //GenericClass sourcerecord = new GenericClass ( );
                    //sourcerecord = e . data as GenericClass;
                    //custno = sourcerecord . CustNo;
                    //bankno = sourcerecord . BankNo;
                }
                if ( this . CurrentType == "CUSTOMER" ) {
                    try {
                        foreach ( CustomerViewModel item in this . listbox1 . Items ) {
                            if ( item . CustNo == custno ) { //&& item . BankNo == bankno ) {
                                Tabview . Tabcntrl . Selectionchanged = true;
                                this . listbox1 . SelectedIndex = newindex;
                                this . listbox1 . SelectedItem = newindex;
                                Debug . WriteLine ( $"Listbox selection in Customers matched on {custno}:{bankno}, index {newindex}" );
                                Utils . ScrollLBRecordIntoView ( this . listbox1 , newindex );
                                success = true;
                                break;
                            }
                            newindex++;
                        }
                    }
                    catch ( Exception ex ) { Debug . WriteLine ( $"Listbox failed search in Customer for match to {custno} : {bankno}" ); }
                }
                else if ( this . CurrentType == "BANK" ) {
                    //                  int indx = 0;
                    try {
                        foreach ( BankAccountViewModel item in this . listbox1 . Items ) {
                            if ( item . CustNo == custno ) { //} && item . BankNo == bankno ) {
                                Tabview . Tabcntrl . Selectionchanged = true;
                                this . listbox1 . SelectedIndex = newindex;
                                this . listbox1 . SelectedItem = newindex;
                                Debug . WriteLine ( $"Listbox selection in BankAccount matched on {custno}:{bankno}, index {newindex}" );
                                Utils . ScrollLBRecordIntoView ( this . listbox1 , newindex );
                                success = true;
                                break;
                            }
                            newindex++;
                        }
                    }
                    catch ( Exception ex ) { Debug . WriteLine ( $"Listbox failed search in Bank for match to {custno} : {bankno}\n{ex . Message}" ); }
                }
                if ( success == false ) {
                    if ( this . CurrentType == "BANK" )
                        Debug . WriteLine ( $"Listbox failed search in Bank for match to {custno} : {bankno}" );
                    else
                        Debug . WriteLine ( $"Listbox failed search in Customer for match to {custno} : {bankno}" );
                }
                else if ( success ) {
                    Utils . ScrollLBRecordIntoView ( this . listbox1 , newindex );
                }
                this . listbox1 . UpdateLayout ( );
            }
            Tabview . Tabcntrl . Selectionchanged = false;
        }

        private void Button_Click ( object sender , RoutedEventArgs e ) {
            Clipboard . SetText (
                     $"You can set any or all of these properties to\n" +
                     $"Customise the listbox's overall appearance.\n\n" +
                 $"ItemBackground = (SolidColorBrush)\n" +
                 $"ItemForeground = (SolidColorBrush)\n" +
                 $"SelectedBackground = (SolidColorBrush)\n" +
                 $"SelectedForeground = (SolidColorBrush)\n" +
                 $"MouseoverBackground = (SolidColorBrush)\n" +
                 $"MouseoverForeground = (SolidColorBrush)\n" +
                 $"MouseoverSelectedBackground = SolidColorBrush\n" +
                 $"MouseoverSelectedForeground = SolidColorBrush\n" );
            //          TooltipPopup . IsOpen = false;
        }

        private void ttSp_PreviewMouseRightButtonDown ( object sender , MouseButtonEventArgs e ) {
            //            e . Handled = true;
        }
        private void Hidepopup_Click ( object sender , RoutedEventArgs e ) {
            TooltipPopup . Visibility = Visibility . Collapsed;
            timer . Stop ( );
            TipElapsedTime = 0;
        }
        private void listbox1_MouseLeave ( object sender , MouseEventArgs e ) {
        }
        private void LbUserControl_Tooltipshown ( object sender , TooltipArgs e ) {
        }
        private void Timer_Tick ( object sender , EventArgs e ) {
            if ( TooltipPopup . Visibility == Visibility . Visible ) {
                Debug . WriteLine ( TipElapsedTime );
                if ( TipElapsedTime > MAXTOOLTIPSECS ) {
                    TooltipPopup . Visibility = Visibility . Collapsed;
                    timer . Stop ( );
                    TipElapsedTime = 0;
                }
                else
                    TipElapsedTime++;
            }
        }

        public void PART_MouseLeave ( object sender , MouseEventArgs e ) {
            var tabview = TabWinViewModel . Tview;
            if ( TabWinViewModel . CurrentTabTextBlock == "Tab2Header" ) {
                Tabview . TriggerStoryBoardOff ( 2 );
                tabview . Tab2Header . FontSize = 14;
                tabview . Tab2Header . Foreground = FindResource ( "Cyan0" ) as SolidColorBrush;
            }
        }

        public void PART_MouseEnter ( object sender , MouseEventArgs e ) {
            var tabview = TabWinViewModel . Tview;
            Tabview . TriggerStoryBoardOn ( 2 );
            tabview . Tab2Header . FontSize = 18;
            tabview . Tab2Header . Foreground = FindResource ( "Yellow0" ) as SolidColorBrush;
        }
        private void listbox1_PreviewMouseMove ( object sender , MouseEventArgs e ) {
            ListBox lbSender = sender as ListBox;
            if ( lbSender != null ) {
                lbSender = sender as ListBox;
                if ( lbSender . Name == "listbox1" ) {
                    TabWinViewModel . CurrentTabIndex = 1;
                    TabWinViewModel . CurrentTabName = "ListboxTab";
                    TabWinViewModel . CurrentTabTextBlock = "Tab2Header";
                }
            }
        }
        private void Magnifyplus2 ( object sender , RoutedEventArgs e ) {
            // set Listbox font size
            Fontsize += 2;
            listbox1 . FontSize = Fontsize; ;
            listbox1 . UpdateLayout ( );
        }
        private void Magnifyminus2 ( object sender , RoutedEventArgs e ) {
            Fontsize -= 2;
            listbox1 . FontSize = Fontsize; ;
            listbox1 . UpdateLayout ( );
        }

        #region unused
        private void ttloaded ( object sender , RoutedEventArgs e ) {
            //ListBox lb = sender as ListBox;
            //StackPanel ttpi = lb . ToolTip as StackPanel;
            //if ( this . ttSp == null ) return;
            //if ( ShowToolTip == false )
            //    ttSp . Visibility = Visibility . Collapsed;
            //else
            //    ttSp . Visibility = Visibility . Visible;
        }

        public void GetObjectData ( SerializationInfo info , StreamingContext context ) {
            throw new NotImplementedException ( );
        }
        #endregion unused

        private void ReloadBank ( object sender , RoutedEventArgs e ) {
            Tabview . Tabcntrl . twVModel . TabLoadDb ( this , "BANKACCOUNT" , true ); ;

        }

        private void ReloadCust ( object sender , RoutedEventArgs e ) {
            Tabview . Tabcntrl . twVModel . TabLoadDb ( this , "CUSTOMER" , true );
        }
    }
}

