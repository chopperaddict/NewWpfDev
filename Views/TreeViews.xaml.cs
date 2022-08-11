using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Diagnostics;
using System . IO;
using System . Linq;
using System . Threading;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Media . Imaging;
using System . Windows . Threading;

using NewWpfDev. Models;
using NewWpfDev. UserControls;
using NewWpfDev . ViewModels;

namespace NewWpfDev. Views
{
    public partial class TreeViews : Window, INotifyPropertyChanged
    {
        #region ALL Declarations

        #region OnPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public int SLEEPTIME { get; set; } = 100;

        public static object [ ] Args = new object [ ] { new object ( ) , new object ( ) , new object ( ) };
        public static object TreeViewObject;
        public static bool AbortExpand = false;
        public static bool ExpandLimited = false;
        protected void OnPropertyChanged ( string PropertyName )
        {
            if ( null != PropertyChanged )
            {
                PropertyChanged ( this ,
                      new PropertyChangedEventArgs ( PropertyName ) );
            }
        }
        #endregion OnPropertyChanged

        public static int PROGRESSWRAPVALUE = 24;
        public static int MAXSEARCHLEVELS = 99;

        #region Brushes
        public SolidColorBrush YellowBrush;
        public SolidColorBrush WhiteBrush;
        public SolidColorBrush BlackBrush;
        public SolidColorBrush RedBrush;
        public SolidColorBrush Brush0;
        public SolidColorBrush Brush1;
        public SolidColorBrush Brush2;
        public SolidColorBrush Brush3;
        public SolidColorBrush Brush4;
        public SolidColorBrush Brush5;
        public SolidColorBrush Brush6;
        public SolidColorBrush Brush7;
        public SolidColorBrush Brush8;
        #endregion Brushes

        #region Expansion Items
        public struct ExpandArgs
        {
            public TreeView tv;
            public TreeViewItem tvitem;
            public TreeViewItem SearchSuccessItem;
            public int ExpandLevels;
            public int MaxItems;
            public string SearchTerm;
            public bool SearchActive;
            public int Selection;
            public bool SearchSuccess;
            public bool ListResults;
            public bool IsFullExpand;
            public TreeViewItem Parent;
        };
        public static ExpandArgs ExpArgs = new ExpandArgs ( );

        public static Dictionary<string , string> VolumeLabelsDict = new Dictionary<string , string> ( );

        #endregion Expansion Items

        #region general variable declarations

        public ExplorerClass TvExplorer = null;
        public TreeView ActiveTree { get; set; }
        public struct lbitemtemplate
        {
            public string Colm1 { get; set; }
            public string Colm2 { get; set; }
            public string Colm3 { get; set; }
            public string Colm4 { get; set; }
            public string Colm5 { get; set; }
            public string Colm6 { get; set; }
        };

        public List<String> DirOptions { get; set; }

        public List<ComboBoxItem> DirectoryOptions2 = new List<ComboBoxItem> ( );
        //        public List<Family> families = new List<Family> ( );
        public static List<string> LbStrings = new List<string> ( );
        public static List<string> ValidFiles = new List<string> ( );
        public static List<TreeViewItem> AllCheckedFolders = new List<TreeViewItem> ( );
        public int ExpandSelection { get; set; } = -1;
        public bool TrackExpand { get; set; } = false;
        public TreeViewItem SelectedTVItem { get; set; }
        //        public bool ClosePreviousFolder { get; set; } = false;
        public Image tvimage = new Image ( );
        public static int iterations { get; set; } = 0;


        #endregion general variable declarations

        #region Full Properties

        private string fullDetail;
        public string FullDetail
        {
            get { return fullDetail; }
            set { fullDetail = value; OnPropertyChanged ( nameof ( FullDetail ) ); }
        }
        private bool refreshListbox;
        public bool RefreshListBox
        {
            get { return ( bool ) refreshListbox; }
            set { refreshListbox = value; OnPropertyChanged ( nameof ( RefreshListBox ) ); }
        }
        private int currentTree;
        public int CurrentTree
        {
            get { return currentTree; }
            set { currentTree = value; OnPropertyChanged ( nameof ( RefreshListBox ) ); }
        }
        private int currentLevel;
        public int CurrentLevel
        {
            get { return currentLevel; }
            set { currentLevel = value; OnPropertyChanged ( nameof ( CurrentLevel ) ); }
        }
        private string defaultDrive;
        public string DefaultDrive
        {
            get { return defaultDrive; }
            set { defaultDrive = value; OnPropertyChanged ( nameof ( DefaultDrive ) ); }
        }
        private TreeViews treeviewsclass;
        public TreeViews Treeviewsclass
        {
            get { return treeviewsclass; }
            set { treeviewsclass = value; OnPropertyChanged ( nameof ( Treeviewsclass ) ); }
        }
        private ExplorerClass explorer;
        public ExplorerClass Explorer
        {
            get { return explorer; }
            set { explorer = value; OnPropertyChanged ( nameof ( Explorer ) ); }
        }
        private bool exactmatch;
        public bool Exactmatch
        {
            get { return exactmatch; }
            set { exactmatch = value; OnPropertyChanged ( nameof ( Exactmatch ) ); }
        }
        private bool listresults;
        public bool LISTRESULTS
        {
            get { return listresults; }
            set { listresults = value; OnPropertyChanged ( nameof ( LISTRESULTS ) ); }
        }
        // Global flag to control auto closing of searched folders (only)
        private bool closePreviousNode;
        public bool ClosePreviousNode
        {
            get { return closePreviousNode; }
            set { closePreviousNode = value; OnPropertyChanged ( nameof ( ClosePreviousNode ) ); }
        }
        private bool showVolumeLabels;
        public bool ShowVolumeLabels
        {
            get { return showVolumeLabels; }
            set { showVolumeLabels = value; OnPropertyChanged ( nameof ( ShowVolumeLabels ) ); }
        }
        private bool showallfiles;
        public bool ShowAllFiles
        {
            get { return showallfiles; }
            set { showallfiles = value; OnPropertyChanged ( nameof ( ShowAllFiles ) ); }
        }
        private string expandDuration;
        public string ExpandDuration
        {
            get { return expandDuration; }
            set { expandDuration = value; OnPropertyChanged ( nameof ( ExpandDuration ) ); }
        }

        private int progressCount;
        public int ProgressCount
        {
            get { return progressCount; }
            set
            {
                if ( value != 0 && value % PROGRESSWRAPVALUE == 0 )
                {
                    if ( BusyLabel . Visibility == Visibility . Hidden )
                        BusyLabel . Visibility = Visibility . Visible;
                    progressCount = 0;
                    ProgressString = ".";
                    if ( BusyLabelColor == RedBrush )
                    {
                        BusyLabelColor = YellowBrush;
                        BusyLabelBkgrn = BlackBrush;
                    }
                    else
                    {
                        BusyLabelColor = RedBrush;
                        BusyLabelBkgrn = WhiteBrush;
                    }

                }
                else
                {
                    progressCount = value;
                    ProgressString = def . Substring ( 0 , value );
                    OnPropertyChanged ( ProgressCount . ToString ( ) );
                }
            }
        }
        private string progressString;
        public string ProgressString
        {
            get { return progressString; }
            set
            {
                progressString = value;
                OnPropertyChanged ( ProgressString );
            }
        }
        private int listboxtotal;
        public int Listboxtotal
        {
            get { return listboxtotal; }
            set { listboxtotal = value; OnPropertyChanged ( Listboxtotal . ToString ( ) ); }
        }
        private SolidColorBrush busyLabelColor;
        public SolidColorBrush BusyLabelColor
        {
            get { return busyLabelColor; }
            set { busyLabelColor = value; OnPropertyChanged ( BusyLabelColor . ToString ( ) ); }
        }
        private SolidColorBrush busyLabelBkgrn;
        public SolidColorBrush BusyLabelBkgrn
        {
            get { return busyLabelBkgrn; }
            set { busyLabelBkgrn = value; OnPropertyChanged ( BusyLabelBkgrn . ToString ( ) ); }
        }
        private int comboSelectedItem;

        public int ComboSelectedItem
        {
            get { return comboSelectedItem; }
            set { comboSelectedItem = value; OnPropertyChanged ( ComboSelectedItem . ToString ( ) ); }
        }

 
        #endregion Full Properties

        #region Dependency Properties
        public bool tv1SelectedItem
        {
            get { return ( bool ) GetValue ( tv1SelectedItemProperty ); }
            set { SetValue ( tv1SelectedItemProperty , value ); }
        }
        public static readonly DependencyProperty tv1SelectedItemProperty =
            DependencyProperty . Register ( "tv1SelectedItem" , typeof ( bool ) , typeof ( TreeViews ) , new PropertyMetadata ( false ) );
        public bool tv2SelectedItem
        {
            get { return ( bool ) GetValue ( tv2SelectedItemProperty ); }
            set { SetValue ( tv2SelectedItemProperty , value ); }
        }
        public static readonly DependencyProperty tv2SelectedItemProperty =
            DependencyProperty . Register ( "tv2SelectedItem" , typeof ( bool ) , typeof ( TreeViews ) , new PropertyMetadata ( false ) );
        public bool tv3SelectedItem
        {
            get { return ( bool ) GetValue ( tv3SelectedItemProperty ); }
            set { SetValue ( tv3SelectedItemProperty , value ); }
        }
        public static readonly DependencyProperty tv3SelectedItemProperty =
            DependencyProperty . Register ( "tv3SelectedItem" , typeof ( bool ) , typeof ( TreeViews ) , new PropertyMetadata ( false ) );
        public TreeViewItem tv4SelectedItem
        {
            get { return ( TreeViewItem ) GetValue ( tv4SelectedItemProperty ); }
            set { SetValue ( tv4SelectedItemProperty , value ); }
        }
        public static readonly DependencyProperty tv4SelectedItemProperty =
            DependencyProperty . Register ( "tv4SelectedItem" , typeof ( TreeViewItem ) , typeof ( TreeViews ) , new PropertyMetadata ( ( TreeViewItem ) null ) );
        public TreeViewItem SelectedItem
        {
            get { return ( TreeViewItem ) GetValue ( SelectedItemProperty ); }
            set { SetValue ( SelectedItemProperty , value ); }
        }
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty . Register ( "SelectedItem" , typeof ( TreeViewItem ) , typeof ( TreeViews ) , new PropertyMetadata ( ( TreeViewItem ) null ) );
        public double Fontsize
        {
            get { return ( double ) GetValue ( FontsizeProperty ); }
            set { SetValue ( FontsizeProperty , value ); }
        }
        public static readonly DependencyProperty FontsizeProperty =
            DependencyProperty . Register ( "Fontsize" , typeof ( double ) , typeof ( TreeViews ) , new PropertyMetadata ( ( double ) 12 ) );
        public BitmapImage LsplitterImage
        {
            get
            { return ( BitmapImage ) GetValue ( LsplitterImageProperty ); }
            set { SetValue ( LsplitterImageProperty , value ); }
        }
        public static readonly DependencyProperty LsplitterImageProperty =
            DependencyProperty . Register ( "LsplitterImage" , typeof ( BitmapImage ) , typeof ( TreeViews ) , new PropertyMetadata ( ( BitmapImage ) null ) );
        public BitmapImage VsplitterImage
        {
            get
            { return ( BitmapImage ) GetValue ( VsplitterImageProperty ); }
            set { SetValue ( VsplitterImageProperty , value ); }
        }
        public static readonly DependencyProperty VsplitterImageProperty =
            DependencyProperty . Register ( "VsplitterImage" , typeof ( BitmapImage ) , typeof ( TreeViews ) , new PropertyMetadata ( ( BitmapImage ) null ) );
        public string LeftSplitterText
        {
            get { return ( string ) GetValue ( LeftSplitterTextProperty ); }
            set { SetValue ( LeftSplitterTextProperty , value ); }
        }
        // Using a DependencyProperty as the backing store for LeftSplitterText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftSplitterTextProperty =
            DependencyProperty . Register ( "LeftSplitterText" , typeof ( string ) , typeof ( TreeViews ) , new PropertyMetadata ( ( string ) "Drag Up or Down" ) );
        public string RightSplitterText
        {
            get { return ( string ) GetValue ( RightSplitterTextProperty ); }
            set { SetValue ( RightSplitterTextProperty , value ); }
        }
        // Using a DependencyProperty as the backing store for LeftSplitterText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightSplitterTextProperty =
            DependencyProperty . Register ( "RightSplitterText" , typeof ( string ) , typeof ( TreeViews ) , new PropertyMetadata ( ( string ) "to View Directory Tree / Drive Technical Information." ) );


        public SolidColorBrush LbTextColor
        {
            get { return ( SolidColorBrush ) GetValue ( LbTextColorProperty ); }
            set { SetValue ( LbTextColorProperty , value ); }
        }
        public static readonly DependencyProperty LbTextColorProperty =
            DependencyProperty . Register ( "LbTextColor" , typeof ( SolidColorBrush ) , typeof ( TreeViews ) ,
                new PropertyMetadata ( new SolidColorBrush ( Colors . Black ) ) );

        #endregion Dependency Properties

        #region General  dclarations
        public string def = ".....................................................";
        public string SearchString { get; set; } = "";
        public string ExceptionMessage { get; set; }
        public static bool BreakExpand { get; set; } = false;
        private static bool isresettingSelection { get; set; } = false;
        public int maxlevels { get; set; }
        public int TotalItemsExpanded { get; set; }
        public static lbitemtemplate lbtmp { get; set; }
        public TextBlock LbTextblock { get; set; }
        public string TextToSearchFor { set; get; } = "";
        public static TreeViews treeViews { get; set; }
        private TreeViewItem startitem { get; set; }

        public bool HasHidden = false;
        private bool FullExpandinProgress = false;
        public bool Loading = true;
        private static double startmin = 0;
        private static double startsec = 0;
        private static double startmsec = 0;
        //		public static LazyLoading Lazytree = null;
        public Family family1 = new Family ( );
        public DirectoryInfo DirInfo = new DirectoryInfo ( @"C:\\" );
        private static DispatcherTimer sw = new DispatcherTimer ( );

        #endregion General declarations


        #region Attached Properties

        public static bool Gettvselection ( DependencyObject obj )
        {return ( bool ) obj . GetValue ( tvselectionProperty );}
        public static void Settvselection ( DependencyObject obj , bool value )
        {obj . SetValue ( tvselectionProperty , value );}
        public static readonly DependencyProperty tvselectionProperty =
            DependencyProperty . RegisterAttached ( "tvselection" , typeof ( bool ) , typeof ( TreeViews ) , new PropertyMetadata ( ( bool ) false ) );

        public static bool GetIsMouseDirectlyOverItem ( DependencyObject obj )
        {
            return ( bool ) obj . GetValue ( IsMouseDirectlyOverItemProperty );
        }
        public static void SetIsMouseDirectlyOverItem ( DependencyObject obj , bool value )
        {
            obj . SetValue ( IsMouseDirectlyOverItemProperty , value );
        }
        public static readonly DependencyProperty IsMouseDirectlyOverItemProperty =
            DependencyProperty . RegisterAttached ( "IsMouseDirectlyOverItem" , typeof ( bool ) , typeof ( TreeViews ) , new PropertyMetadata ( ( bool ) false ) );

        #endregion Attached Properties

        private static FlowdocLib fdl;

        #endregion ALL Declarations

        public List<string> EditContentMenuitems = new List<string> ( );
        public ObservableCollection<MenuTestViewModel> MenuItems = new ObservableCollection<MenuTestViewModel> ( );
        #region startup  items
        public TreeViews ( )
        {
#pragma warning disable CS0219 // The variable 'count' is assigned but its value is never used
            int count = 0;
#pragma warning restore CS0219 // The variable 'count' is assigned but its value is never used
            InitializeComponent ( );
            this . DataContext = this;
            ReadSettings ( );
            //tvitemclass = new TvItemClass ( );
            // Get ObsCollection
            //tvitems = TvItemClass . tvcollectionitems;
            // Cannot use  this with FlowDoc cos of dragging/Resizing
            //Utils . SetupWindowDrag ( this );
            ActiveTree = TestTree;
            OptionsPanel . Visibility = Visibility . Hidden;
            treeViews = this;
            listBox . Items . Clear ( );
            ActiveTree . Items . Clear ( );
            fdl = new FlowdocLib ( );
            FdMargin . Left = Flowdoc . Margin . Left;
            FdMargin . Top = Flowdoc . Margin . Top;
            TvExplorer = new ExplorerClass ( );
            TreeViewItem tvi = new TreeViewItem ( );
            tvi . Header = @"C:\";
            BusyLabelColor = FindResource ( "Yellow0" ) as SolidColorBrush;
            BusyLabelBkgrn = FindResource ( "Black0" ) as SolidColorBrush;

            YellowBrush = FindResource ( "Yellow0" ) as SolidColorBrush;
            BlackBrush = FindResource ( "Black0" ) as SolidColorBrush;
            RedBrush = FindResource ( "Red5" ) as SolidColorBrush;
            WhiteBrush = FindResource ( "White2" ) as SolidColorBrush;
            LoadSearchLevels ( );
            // Set Horizontal Splitter FULLY DOWN at startup
            TopGrid . RowDefinitions [ 2 ] . Height = new GridLength ( 0 , GridUnitType . Pixel );
            Col3 . Width = new GridLength ( 350 , GridUnitType . Pixel );
            testtreebanner . Text = "Manual Directories System, TestTree";

        }
        private void Window_Loaded ( object sender , RoutedEventArgs e )
        {
            string output = "";
            this . SetValue ( FontsizeProperty , InfoList . FontSize );
            canvas . Visibility = Visibility . Visible;
            CreateBrushes ( );
            VolumeLabelsDict . Clear ( );
            LoadDrives ( TestTree );
            LoadDrives ( TestTree2 );
            ExpArgs . SearchSuccessItem = new TreeViewItem ( );
            Flowdoc . ExecuteFlowDocMaxmizeMethod += new EventHandler ( MaximizeFlowDoc );
            Flowdoc . HandleKeyEvents += new KeyEventHandler ( Flowdoc_HandleKeyEvents );
            List<String> errors = new List<string> ( );
            //            LazyLoadingTreeview . LazyLoadTreeview ( treeViewModel , this , ref errors );
            if ( errors . Count > 0 )
            {
                foreach ( var item in errors )
                {

                    listBox . Items . Add ( "<-- " + item );
                    output += item + "\n";
                }
            }
            //Grid1 . RowDefinitions [ 1 ] . Height = new GridLength ( 3 , GridUnitType . Star );
            //Grid1 . RowDefinitions [ 3 ] . Height = new GridLength ( 155 , GridUnitType . Pixel );
            // orow2 . Height = new GridLength ( 0 , GridUnitType . Pixel ); 
            LsplitterImage = new BitmapImage ( new Uri ( @"\icons\Lrg updown arrow red copy.png" , UriKind . Relative ) );
            VsplitterImage = new BitmapImage ( new Uri ( @"\icons\Lrg ltrt arrow red copy.png" , UriKind . Relative ) );
            ShowDriveInfo ( sender , e );
            loadExpandOptions ( );
            ExpandSetup ( false );
            DrivesCombo . SelectedIndex = 0;
            maxlevels = 99;
            Loading = false;
            if ( LISTRESULTS )
                CurrentFolder . Text = "Information / Log  Panel : ENABLED";
            else
                CurrentFolder . Text = "Information / Log  Panel : DISABLED";
            Duration . Text = "";
            ProgressCount = 0;
            Expandcounter . Text = "";
            this . DataContext = this;
            ToolTip ttprogbar = new ToolTip ( );
            ttprogbar . Content = $"Dbl-Click to Expand / Collapse any selected item\nor Right Click to access various useful built-in\nTree View Expansion Options.\n    ";
            ttprogbar . Background = FindResource ( "White3" ) as SolidColorBrush;
            ttprogbar . Foreground = FindResource ( "Blue4" ) as SolidColorBrush;
            ttprogbar . FontWeight = FontWeights . SemiBold;

            TestTree . ToolTip = ttprogbar;

            //            OpenContextMenu ( TVContextMenu as FrameworkElement );
            CreateContextMenu ( );
            LoadMenu ( );
            //fdl . ShowInfo ( Flowdoc, canvas, "This Version is here to demonstrate the use of a Templated style that handles all coloring", "Black0", "Information Idiot !!", "Red5");
        }

        private void LoadMenu ( )
{
            MenuItems = new ObservableCollection<MenuTestViewModel>
            {
                new MenuTestViewModel { Header = "Alpha" },
                new MenuTestViewModel { Header = "Beta",
                    MenuItems = new ObservableCollection<MenuTestViewModel>
                        {
                            new MenuTestViewModel { Header = "Beta1" },
                            new MenuTestViewModel { Header = "Beta2",
                                MenuItems = new ObservableCollection<MenuTestViewModel>
                                {
                                    new MenuTestViewModel { Header = "Beta1a" },
                                    new MenuTestViewModel { Header = "Beta1b" },
                                    new MenuTestViewModel { Header = "Beta1c" }
                                }
                            },
                            new MenuTestViewModel { Header = "Beta3" }
                        }
                },
                new MenuTestViewModel { Header = "Gamma" }
            };
        }
        private void CreateContextMenu()
        {
            EditContentMenuitems . Clear ( );
            EditContentMenuitems . Add ( "_File" );
            EditContentMenuitems  . Add ( "_Edit" );
            EditContentMenuitems  . Add ( "_View" );
            EditContentMenuitems  . Add ( "_Window" );
            EditContentMenuitems . Add ( "_Help" );
            LbContext . ItemsSource = null;
            LbContext . Items . Clear ( );
            LbContext . ItemsSource = EditContentMenuitems;
        }
        private void CtxTest ( object sender , RoutedEventArgs e )
        {
            CreateContextMenu ( );
//            OpenContextMenu ( InfoList );
        }
        private void OpenContextMenu ( FrameworkElement element )
        {
            if ( element . ContextMenu != null )
            {
                element . ContextMenu . PlacementTarget = element;
                element . ContextMenu . IsOpen = true;
            }
        }

        public void HandleImageClick ( object sender , MouseButtonEventArgs e )
        {
        }
        private void TREEViews_Closing ( object sender , CancelEventArgs e )
        {
            Flowdoc . ExecuteFlowDocMaxmizeMethod -= new EventHandler ( MaximizeFlowDoc );
            Flowdoc . HandleKeyEvents -= new KeyEventHandler ( Flowdoc_HandleKeyEvents );
            SaveSettings ( );
        }
        private void ReadSettings ( )
        {
            string output = "";
            output = File . ReadAllText (@"C:\Users\Ianch\Documents\treeviewsettings.dat");
            string [ ] input = output . Split ( ',' );
            for ( int x = 0 ; x < input . Length ; x++ )
            {
                switch ( x )
                {
                    case 0:
                        Exactmatch = input [ x ] == "T" ? true : false;
                        break;
                    case 1:
                        LISTRESULTS = input [ x ] == "T" ? true : false;
                        break;
                    case 2:
                        ClosePreviousNode = input [ x ] == "T" ? true : false;
                        break;
                    case 3:
                        ShowVolumeLabels = input [ x ] == "T" ? true : false;
                        break;
                    case 4:
                        ShowAllFiles = input [ x ] == "T" ? true : false;
                        break;
                    case 5:
                        RefreshListBox = input [ x ] == "T" ? true : false;
                        break;
                }
            }
        }
        private void SaveSettings ( )
        {
            string output = "";
            output = Exactmatch ? "T," : "F,";
            output += LISTRESULTS ? "T," : "F,";
            output += ClosePreviousNode ? "T," : "F,";
            output += ShowVolumeLabels ? "T," : "F,";
            output += ShowVolumeLabels ? "T," : "F,";
            output += RefreshListBox ? "T," : "F,";
            output += "\n";
            File . WriteAllText (@"C:\Users\Ianch\Documents\treeviewsettings.dat" , output );
        }

        #endregion startup    items     

        #region close dwn
        private void App_Close ( object sender , RoutedEventArgs e )
        {
            sw . Stop ( );
            this . Close ( );
            Application . Current . Shutdown ( );
        }

        private void Close_Btn ( object sender , RoutedEventArgs e )
        {
            sw . Stop ( );
            this . Close ( );
        }
        #endregion close dwn

        #region Initialization methods

        private void TestViewModel ( object sender , RoutedEventArgs e )
        {
            List<string> dirs = new List<string> ( );
            List<string> files = new List<string> ( );
            string selecteddrive = DrivesCombo . SelectedItem . ToString ( );
            LoadDrives ( ActiveTree , selecteddrive );
        }

        public void LoadDrives ( TreeView tv , string drivetoload = "" )
        {
            bool ValidDrive = false;
            //            bool HasHiddenItems = false;
            string volabel = "";
            string DriveHeader = "";
            string Padding = "                 ";
            bool isvalid = false;
            tv . Items . Clear ( );
            //            listBox . Items . Clear ( );
            listBox . UpdateLayout ( );
            DrivesCombo . Items . Add ( "ALL" );
            VolumeLabelsDict . Clear ( );
            LoadValidFiles ( );
            if ( drivetoload == "ALL" )
                drivetoload = "";
            foreach ( var drive in Directory . GetLogicalDrives ( ) )
            {
                ValidDrive = false;
                DriveHeader = "";
                if ( drivetoload . ToUpper ( ) != "" )
                {
                    if ( drive . ToUpper ( ) != drivetoload . ToUpper ( ) )
                        continue;
                }
                //Add Drive to Treeview
                DriveInfo [ ] di = DriveInfo . GetDrives ( );
                foreach ( var item in di )
                {
                    if ( item . Name == drive )
                    {
                        if ( item . DriveType == DriveType . CDRom )
                        {
                            ValidDrive = true;
                            //isvalid = true;
                            DriveHeader = Padding . Substring ( 0 , 10 );
                            DriveHeader += "CdRom Drive";
                            string newlabel = " " + DriveHeader;
                            volabel = "   CdRom Drive (No CdRom)";
                            VolumeLabelsDict . Add ( drive , volabel );
                            if ( ShowVolumeLabels == true )
                            {
                                DriveHeader = $"    {volabel}";
                            }
                        }
                        else
                        {
                            List<string> directories = new List<string> ( );
                            GetDirectories ( item . ToString ( ) , out directories );
                            foreach ( var dir in directories )
                            {
                                //if ( CheckIsVisible ( dir . ToUpper ( ) , ShowAllFiles , out HasHidden ) == true )
                                //{
                                isvalid = true;
                                string newlabel = " " + item . VolumeLabel;
                                VolumeLabelsDict . Add ( drive , newlabel );
                                if ( ShowVolumeLabels == true )
                                {
                                    DriveHeader = $"    [{newlabel}]";
                                }
                                break;
                                //}
                            }
                            if ( isvalid )
                            {
                                if ( ShowVolumeLabels == true )
                                    DriveHeader = $"   [{item . VolumeLabel}]";
                                ValidDrive = true;
                            }
                            else
                                volabel = $"    [{item . VolumeLabel}]";
                        }
                        break;
                    }
                }
                if ( ValidDrive == true )
                {
                    var item = new TreeViewItem ( );
                    item . Header = drive + DriveHeader;
                    item . Tag = drive;
                    tv . Items . Add ( item );
                    // Add Dummy entry so we get an "Can be Opened" triangle icon
                    item . Items . Add ( "Loading" );
                    DrivesCombo . Items . Add ( drive . ToString ( ) );
                    //     tvitems . Add ( item );
                }
                else
                {
                    var item = new TreeViewItem ( );
                    if ( ShowVolumeLabels == true )
                        item . Header = drive + volabel;
                    else
                        item . Header = drive + DriveHeader;
                    item . Tag = drive;
                    tv . Items . Add ( item );
                    item . Items . Add ( "Loading" );
                    DrivesCombo . Items . Add ( drive . ToString ( ) );
                    //  tvitems . Add ( item );
                }
            }
            DrivesCombo . Items . Add ( "ALL" );
            DrivesCombo . SelectedIndex = 0;
            DrivesCombo . SelectedItem = 0;
            tv . UpdateLayout ( );
        }

        // Stored list of all Hidden/System file names so we can handle not showing the,
        public void LoadValidFiles ( )
        {
            ValidFiles . Add ( "BOOTMGR" );
            ValidFiles . Add ( "BOOTNXT" );
            ValidFiles . Add ( "BOOTSTAT" );
            ValidFiles . Add ( "RECOVERY" );
            ValidFiles . Add ( "BOOTNXT" );
            ValidFiles . Add ( "MEMTEST" );
            ValidFiles . Add ( "BOOTUWF" );
            ValidFiles . Add ( "BOOTVHD" );
            ValidFiles . Add ( "MEMTEST" );
            ValidFiles . Add ( "BOOT" );
            ValidFiles . Add ( "$GETCURRENT" );
            ValidFiles . Add ( "$WINDOWS" );
            ValidFiles . Add ( "$WINREAGENT" );
            ValidFiles . Add ( "CONFIG.MSI" );
            ValidFiles . Add ( "WINDOWS.OLD" );
            ValidFiles . Add ( ".BIN" );
            ValidFiles . Add ( "$WINRE_BACKUP" );
            ValidFiles . Add ( "RECYCLE" );
            ValidFiles . Add ( "SYSTEM VOLUME INFORMATION" );
            ValidFiles . Add ( "BACKUP_PARTITION" );
            ValidFiles . Add ( "BOOTSECT" );
        }

        #endregion Initialization methods
        private void Window_KeyDown ( object sender , KeyEventArgs e )
        {
            if ( e . Key == Key . F8 )
                Debugger . Break ( );
        }
        private void ShowallFiles_Click ( object sender , RoutedEventArgs e )
        {
            CheckBox cb = sender as CheckBox;
            if ( cb . IsChecked == true )
                ShowAllFiles = true;
            else
                ShowAllFiles = false;
            LoadDrives ( ActiveTree );
        }

        #region utilities
        public static string GetFileFolderName ( string path )
        {
            if ( string . IsNullOrEmpty ( path ) )
                return String . Empty;
            var normalizedPath = path . Replace ( '/' , '\\' );
            var lastindex = normalizedPath . LastIndexOf ( '\\' );
            if ( lastindex <= 0 )
                return path;
            return path . Substring ( lastindex + 1 );
        }
        private static T FindAnchestor<T> ( DependencyObject current )
         where T : DependencyObject
        {
            do
            {
                if ( current is T )
                {
                    return ( T ) current;
                }
                current = VisualTreeHelper . GetParent ( current );
            }
            while ( current != null );
            return null;
        }
        #endregion utilities

        #region Flowdoc support via library
        /// <summary>
        /// These methods are needed to allow FLowdoc  to work via FlowDocLib
        ///  We also Need to declare an object :
        ///  object MovingObject ;
        ///  in the heade area just worksj
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        // Variables Required for FlowDoc
        object MovingObject;
#pragma warning disable CS0414 // The field 'TreeViews.fdTop' is assigned but its value is never used
        private static double fdTop = 100;
#pragma warning restore CS0414 // The field 'TreeViews.fdTop' is assigned but its value is never used
#pragma warning disable CS0414 // The field 'TreeViews.fdLeft' is assigned but its value is never used
        private static double fdLeft = 100;
#pragma warning restore CS0414 // The field 'TreeViews.fdLeft' is assigned but its value is never used
        private static Thickness FdMargin = new Thickness ( );

        /*  
		 *  Add these  to the FlowDoc in XAML
  				PreviewMouseLeftButtonDown="Flowdoc_PreviewMouseLeftButtonDown"
				MouseLeftButtonUp="Flowdoc_MouseLeftButtonUp"
				MouseMove= "Flowdoc_MouseMove"
				LostFocus="Flowdoc_LostFocus"
*/

        // Add this startup :-			Flowdoc . ExecuteFlowDocMaxmizeMethod += new EventHandler ( MaximizeFlowDoc );
        // & of course  on closing :-	Flowdoc . ExecuteFlowDocMaxmizeMethod -= new EventHandler ( MaximizeFlowDoc );


        protected void MaximizeFlowDoc ( object sender , EventArgs e )
        {
            // Clever "Hook" method that Allows the flowdoc to be resized to fill window
            // or return to its original size and position courtesy of the Event declard in FlowDoc
            //Need to ensure the wrapping canvas is sized to its containing element (Wiindow outer Grid in this case)
            canvas . Height = Grid1 . ActualHeight;
            canvas . Width = Grid1 . ActualWidth;
            fdl . MaximizeFlowDoc ( Flowdoc , canvas , e );
        }
        // CALLED WHEN  LEFT BUTTON PRESSED
        private void Flowdoc_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
        {
            //In this event, we get current mouse position on the control to use it in the MouseMove event.
            MovingObject = fdl . Flowdoc_PreviewMouseLeftButtonDown ( sender , Flowdoc , e );
            Mouse . OverrideCursor = Cursors . Arrow;
            // NB Flowdoc remebers its last position automatically
        }
        private void Flowdoc_MouseLeftButtonUp ( object sender , MouseButtonEventArgs e )
        {
            // Window wide  !!
            // Called  when a Flowdoc MOVE has ended
            MovingObject = fdl . Flowdoc_MouseLeftButtonUp ( sender , Flowdoc , MovingObject , e );
            ReleaseMouseCapture ( );
            Mouse . OverrideCursor = Cursors . Arrow;
        }
        private void Flowdoc_MouseMove ( object sender , MouseEventArgs e )
        {
            FlowDoc fd = sender as FlowDoc;
            // We are Resizing the Flowdoc using the mouse on the border  (Border.Name=FdBorder)
            fdl . Flowdoc_MouseMove ( Flowdoc , canvas , MovingObject , e );
            fd . Focus ( );
        }
        // Shortened version proxy call		
        private void Flowdoc_LostFocus ( object sender , RoutedEventArgs e )
        {
            Flowdoc . BorderClicked = false;
            Mouse . OverrideCursor = Cursors . Arrow;
        }
        public void Flowdoc_HandleKeyEvents ( object sender , KeyEventArgs e )
        {
            if ( e . Key == Key . Escape )
                this . Visibility = Visibility . Hidden;
        }
        public void FlowDoc_ExecuteFlowDocBorderMethod ( object sender , EventArgs e )
        {
            // EVENTHANDLER to Handle resizing
            FlowDoc fd = sender as FlowDoc;
            Point pt = Mouse . GetPosition ( canvas );
            double dLeft = pt . X;
            double dTop = pt . Y;
        }
        public void fdmsg ( string line1 , string line2 = "" , string line3 = "" )
        {
            //We have to pass the Flowdoc.Name, and Canvas.Name as well as up   to 3 strings of message
            //  you can  just provie one if required
            // eg fdmsg("message text");
            fdl . FdMsg ( Flowdoc , canvas , line1 , line2 , line3 );
            Mouse . OverrideCursor = Cursors . Arrow;
        }


        #endregion Flowdoc support via library

        #region Treeview lower level  support methods
        private static bool CheckIsVisible ( string entry , bool showall , out bool HasHidden )
        {
            HasHidden = false;
            entry = entry . ToUpper ( );
            if ( showall == false )
            {
                foreach ( var item in ValidFiles )
                {
                    if ( entry . Contains ( item . ToUpper ( ) ) )
                    {
                        HasHidden = true;
                        return false;
                    }
                }
                return true;
            }
            return true;
        }
        public int AddDirectoriesToTestTree ( List<string> directories , TreeViewItem item , ListBox lBox = null , bool UseExpand = true )
        {
            // Called by basic TESTTREE_EXPANDED()
            int added = 0;
            int TotalDirs = 0;
#pragma warning disable CS0219 // The variable 'TotalFiles' is assigned but its value is never used
            int TotalFiles = 0;
#pragma warning restore CS0219 // The variable 'TotalFiles' is assigned but its value is never used
            item . Items . Clear ( );
            foreach ( var directoryPath in directories )
            {
                var dummy = new TreeViewItem ( );
                var subitem = new TreeViewItem ( );

                subitem . Header = GetFileFolderName ( directoryPath );
                subitem . Tag = directoryPath;
                UpdateListBox ( directoryPath . ToUpper ( ) );
                if ( CheckIsVisible ( directoryPath . ToUpper ( ) , ShowAllFiles , out HasHidden ) == true )
                {     // add the dummy entry to each of the subdirectories we are adding to the tree so we get the Expand icons
                    TotalDirs = GetDirectoryCount ( directoryPath );
  
                    item . Items . Add ( subitem );
                    //    // Add DUMMY entry as we have content in this folder
                    dummy . Header = "Loading";
                    subitem . Items . Add ( dummy );
                    item . IsExpanded = true;
                    ScrollCurrentTvItemIntoView ( subitem );
                    if ( FullExpandinProgress == false )
                        ActiveTree . Refresh ( );
                    added++;
                }
                ShowProgress ( );
            }
            // Folder now has a set of subdirs in it.
            return added;
        }
        public int AddDirectoriesToTestTreeview ( List<string> directories , TreeViewItem item , ListBox lBox = null , bool UseExpand = true )
        {
            int added = 0;
            if ( directories . Count == 0 )
                return 0;
            item . Items . Clear ( );
            foreach ( var dir in directories )
            {
                var subitem = new TreeViewItem ( );

                ShowProgress ( );

                if ( CheckIsVisible ( dir . ToUpper ( ) , ShowAllFiles , out HasHidden ) == true )
                {
                    try
                    {
                        subitem . Header = GetFileFolderName ( dir );
                        subitem . Tag = dir;
                        item . Items . Add ( subitem );
                        item . IsExpanded = true;
                        subitem . BringIntoView ( );
                        //ActiveTree . HorizontalAlignment = HorizontalAlignment . Left;
                        if ( TrackExpand )
                            ScrollTvItemIntoView ( subitem );
                        ActiveTree . Refresh ( );
                        //UpdateListBox ( subitem . Tag . ToString ( ) );

                        int count = GetDirectories ( dir , out directories );
                        if ( count > 0 )
                        {
                            var tv = new TreeViewItem ( );
                            tv . Header = "Loading";
                            //                      tv . Tag = "Loading";
                            subitem . Items . Add ( tv );
                            if ( TrackExpand )
                                ScrollTvItemIntoView ( subitem );

                            AddDirectoriesToTestTreeview ( directories , subitem , listBox );
                        }
                        else
                        {
                            var tv = new TreeViewItem ( );
                            tv = item;
                            if ( tv . Header . ToString ( ) == "Loading" )
                                item . Items . Clear ( );
                        }
                    }
                    //}
                    catch ( Exception ex )
                    {
                        Debug. WriteLine ( $"AddDirectoriesoTestTreeView : 903 ; Invalid  directory accessed {ex . Message}" );
                    }
                }
                ShowProgress ( );
                added++;
            }
            return added;
        }
        public int AddFilesToTreeview ( List<string> Allfiles , TreeViewItem item )
        {
            int count = 0;
            if ( item . Items . Count == 1 )
            {
                //var tmp = item . Items [ 0 ] . ToString ( );
                //                if ( tmp == "Loading" )
                item . Items . Clear ( );

            }
            item . IsSelected = false;
            //            item . IsExpanded = true;
            foreach ( var itm in Allfiles )
            {
                ShowProgress ( );
                if ( CheckIsVisible ( itm . ToUpper ( ) , ShowAllFiles , out HasHidden ) == true )
                {
                    var subitem = new TreeViewItem ( );
                    subitem . Header = GetFileFolderName ( itm );
                    subitem . Tag = itm;
                    subitem . IsExpanded = false;
                    if ( TrackExpand )
                        subitem . IsSelected = true;
                    subitem . Items . Clear ( );
                    item . Items . Add ( subitem );
                    //if ( item . Tag . ToString ( ) . ToUpper ( ) . Contains ( $"K:\\MY DRIVE" ) )
                    //    Debug. WriteLine ( );
                    //if ( subitem . Tag. ToString ( ) . ToUpper ( ) . Contains ( $"K:\\MY DRIVE" ) )
                    //    Debug. WriteLine ( );
                    // SetValue ( Expander . VisibilityProperty , Visibility.Hidden);
                    //ActiveTree . HorizontalAlignment = HorizontalAlignment . Left;
                    //ScrollCurrentTvItemIntoView ( subitem );
                    if ( TrackExpand )
                    {
                        ScrollTvItemIntoView ( subitem );
                        ActiveTree . Refresh ( );
                    }

                    count++;
                }
                ShowProgress ( );
                if ( FullExpandinProgress == false )
                    ActiveTree . Refresh ( );
            }
            return count;
        }
        public int GetDirectories ( string path , out List<string> dirs )
        {
            bool filterSysfiles = false;
            int count = 0;
            List<string> directories = new List<string> ( );
            try
            {
                string [ ] directs = Directory . GetDirectories ( path , "*.*" , SearchOption . TopDirectoryOnly );
                if ( directs . Length > 0 )
                {
                    foreach ( var item in directs )
                    {
                        try
                        {
                            if ( filterSysfiles )
                            {
                                ShowProgress ( );
                                if ( IsSystemFile ( item . ToUpper ( ) ) == true )
                                {
                                    continue;
                                }
                            }
                            directories . Add ( item );
                            ShowProgress ( );
                            if ( FullExpandinProgress == false )
                                ActiveTree . Refresh ( );
                            count++;
                        }
                        catch ( Exception ex ) { Debug. WriteLine ( $"GetDirectories : 980 : {ex . Message}" ); }
                    }
                    ShowExpandTime ( );
                }
            }
            catch ( Exception ex )
            {
                { Debug. WriteLine ( $"GetDirectories : 981 : {ex . Message}" ); }
            }
            dirs = directories;
            return count;
        }
        public int GetDirectoryCount ( string path )
        {
            int count = 0;
            List<string> directories = new List<string> ( );
            try
            {
                ShowProgress ( );
                string [ ] directs = Directory . GetDirectories ( path , "*.*" , SearchOption . TopDirectoryOnly );
                foreach ( var item in directs )
                {
                    if ( CheckIsVisible ( item . ToUpper ( ) , ShowAllFiles , out HasHidden ) == true )
                    {
                        count++;
                    }
                    //count = directs . Length;
                }
            }
            catch ( Exception ex )
            {
                { Debug. WriteLine ( $"GetDirectoryCount : 9968 : {ex . Message}" ); }
            }
            return count;
        }
        public int GetFiles ( string path , out List<string> allfiles )
        {
            int count = 0;
            var files = new List<string> ( );
            allfiles = new List<string> ( );
            // Get a list of all items in the current folder
            ShowProgress ( );
            if ( FullExpandinProgress == false )
                ActiveTree . Refresh ( );
            try
            {
                if ( GetFilesCount ( path ) <= 0 )
                    return 0;
                //var file = Directory . EnumerateFiles ( path , "*.*" );
                var filecount = Directory . GetFiles ( path , "*.*" , SearchOption . TopDirectoryOnly );
                if ( filecount . Count ( ) > 0 )
                {
                    foreach ( var item in filecount )
                    {
                        ShowProgress ( );
                        if ( CheckIsVisible ( item . ToUpper ( ) , ShowAllFiles , out HasHidden ) == true )
                        {
                            files . Add ( item );
                            //if(item. ToUpper().Contains("BCD.LOG") )
                            //        Debug. WriteLine ( );

                            //                              Debug. WriteLine ();
                            count++;
                            allfiles . Add ( item );
                            // working correctly
                            UpdateListBox ( item );
                        }

                        ShowProgress ( );
                        if ( FullExpandinProgress == false )
                            ActiveTree . Refresh ( );
                    }
                    ShowExpandTime ( );
                }
            }
            catch ( Exception ex )
            {
                Debug. WriteLine ( $"GetFiles : 1052 : {ex . Message}" );
                ExceptionMessage = $"{ex . Message}";
            }
            //allfiles = files;
            return count;
        }
        public int GetFilesCount ( string path )
        {
            int count = 0;
            bool result = true;
            var files = new List<string> ( );
            // Get a list of all items in the current folder
            try
            {
                //var file = Directory . EnumerateFiles ( path , "*.*" );
                var dirfile = Directory . GetFiles ( path , "*.*" , SearchOption . TopDirectoryOnly );
                count = ( int ) dirfile . Length;
                ShowProgress ( );
                //    //var file = Directory . GetFiles ( path , "*.*");
                //    if ( file . Count ( ) > 0 )
                //    {
                //        foreach ( var item in file )
                //        {
                //            if ( CheckIsVisible ( item . ToUpper ( ) , ShowAllfiles ) == true )
                //            {
                //                files . Add ( item );
                //                count++;
                //            }
                //        }
                //        //					files . AddRange ( file );
                //    }
            }
            catch ( Exception ex )
            {
                Debug. WriteLine ( $"GetFilesCount : 1081 : {ex . Message}" );
                result = false;
            }

            //allfiles = files;
            if ( result == false )
                return -1;

            return count;
        }

        #endregion Treeview support methods

        #region Splitter handlers
        private void LeftSplitter_DragStarted ( object sender , System . Windows . Controls . Primitives . DragStartedEventArgs e )
        {
            if ( row1 . ActualHeight <= 10 )
            {
                LeftSplitterText = "Drag Down";
                LsplitterImage = new BitmapImage ( new Uri ( @"\icons\down arroiw red.png" , UriKind . Relative ) );
                RightSplitterText = "to View Tree Directory  Information.";
            }
            else if ( row2 . ActualHeight <= 10 )
            {
                LeftSplitterText = "Drag Up";
                LsplitterImage = new BitmapImage ( new Uri ( @"\icons\up arroiw red.png" , UriKind . Relative ) );
                RightSplitterText = "to view detailed Drive technical Information.";
            }
            else
            {
                LeftSplitterText = "Drag Up or Down";
                LsplitterImage = new BitmapImage ( new Uri ( @"\icons\Lrg updown arrow red copy.png" , UriKind . Relative ) );
                RightSplitterText = "to view Directory Tree / Drive technical Information.";
            }
            //vsplitterleft. Cursor = Cursors . SizeWE;
        }

        private void LeftSplitter_DragCompleted ( object sender , System . Windows . Controls . Primitives . DragCompletedEventArgs e )
        {
            //double totalheight = ( row1 . ActualHeight + orow2 . ActualHeight ) - 20;
            //double maxheight = Grid1 . ActualHeight - btngrid . ActualHeight;
            //double topheight = row1 . ActualHeight - 10;
            //double lowerheight = row2 . ActualHeight - 10;
            //     innergrid . Height = row1.ActualHeight;
            //     InfoList . UpdateLayout ( );
            //            Debug. WriteLine ($"treeView4.ActualHeight = {treeView4 . ActualHeight }, row1.AH = {row1.ActualHeight}" );
            if ( row1 . ActualHeight <= 10 )
            {
                LeftSplitterText = "Drag Down";
                LsplitterImage = new BitmapImage ( new Uri ( @"\icons\down arroiw red.png" , UriKind . Relative ) );
                RightSplitterText = "to view Tree Directory information.";
            }
            else if ( row2 . ActualHeight <= 10 )
            {
                LeftSplitterText = "Drag Up";
                LsplitterImage = new BitmapImage ( new Uri ( @"\icons\up arroiw red.png" , UriKind . Relative ) );
                RightSplitterText = "to view detailed Drive technical Information.";
            }
            else
            {
                LeftSplitterText = "Drag Up or Down";
                LsplitterImage = new BitmapImage ( new Uri ( @"\icons\Lrg updown arrow red copy.png" , UriKind . Relative ) );
                RightSplitterText = "to view Directory Tree / Drive technical Information.";
            }
            vsplitterright . Cursor = Cursors . SizeWE;
        }
        private void Hsplitter_DragOver ( object sender , DragEventArgs e )
        {
            //double totalheight = ( row1 . ActualHeight + orow2 . ActualHeight ) - 20;
            //double maxheight = Grid1 . ActualHeight - btngrid . ActualHeight;
            //double topheight = row1 . ActualHeight - 10;
            //double lowerheight = row2 . ActualHeight - 10;
            ////innergrid . Height = row1 . ActualHeight;
            //InfoList . UpdateLayout ( );
            hsplitter . Cursor = Cursors . SizeNS;
        }
        private void hsplitter_DragDelta ( object sender , System . Windows . Controls . Primitives . DragDeltaEventArgs e )
        {
            //double totalheight = ( row1 . ActualHeight + orow2 . ActualHeight ) - 20;
            //double maxheight = Grid1 . ActualHeight - btngrid . ActualHeight;
            //double topheight = row1 . ActualHeight - 10;
            //double lowerheight = row2 . ActualHeight - 10;
            ////            innergrid . Height = row1 . ActualHeight;
            //            InfoList . UpdateLayout ( );
            hsplitter . Cursor = Cursors . SizeNS;

        }
        private void VRightSplitter_DragStarted ( object sender , System . Windows . Controls . Primitives . DragStartedEventArgs e )
        {
            vsplitterright . Cursor = Cursors . SizeWE;

        }

        private void VRightSplitter_DragCompleted ( object sender , System . Windows . Controls . Primitives . DragCompletedEventArgs e )
        {
            vsplitterright . Cursor = Cursors . SizeWE;
            //if(e.Canceled)
            //    e . Handled = true;

        }

        private void VRightSplitter_DragOver ( object sender , DragEventArgs e )
        {
            vsplitterright . Cursor = Cursors . SizeWE;

        }

        private void vRightSplitter_DragDelta ( object sender , System . Windows . Controls . Primitives . DragDeltaEventArgs e )
        {
            vsplitterright . Cursor = Cursors . SizeWE;
            //Debug. WriteLine ($"{ActiveTree .ActualWidth }" );
            //if ( ActiveTree .ActualWidth < 155 )
            //{
            //    DragCompletedEventArgs dce = new DragCompletedEventArgs ( 0 , 0 , true );
            //    VRightSplitter_DragCompleted ( sender , dce );
            //    e . Handled = true;
            //}
        }

        #endregion Splitter handlers

        #region TestTree Expanding Handling methods

        private bool ExpandAll3 ( TreeViewItem items , bool expand , int levels )
        {
            if ( items == null )
                return false;

            //          levels = ExpArgs . ExpandLevels;
            foreach ( object obj in items . Items )
            {
                if ( AbortExpand )
                    return false;
                //iterations++;
                ShowProgress ( );
                TreeViewItem childControl = obj as TreeViewItem;
                if ( childControl != null )
                {
                    UpdateExpandprogress ( );
                    if ( BreakExpand )
                        break;
                    if ( childControl . Items . Count > 0 )
                    {
                        try
                        {
                            childControl . IsExpanded = true;
                        }
                        catch ( Exception ex ) { Debug. WriteLine ( $"ExpandAll3: 1427 : {ex . Message}" ); }
                        if ( childControl . Header . ToString ( ) == "Loading" )
                            continue;

                        if ( CheckIsVisible ( childControl . Header . ToString ( ) . ToUpper ( ) , ShowAllFiles , out HasHidden ) == false )
                        {
                            continue;
                        }
                        if ( CheckSearchSuccess ( childControl . Tag . ToString ( ) ) == true )
                        {
                            UpdateListBox ( $"\nSearch for {Searchtext . Text} found  as [" + childControl . Header . ToString ( ) + $"]\nin {childControl . Tag . ToString ( )}" );
                            //ActiveTree . HorizontalAlignment = HorizontalAlignment . Left;
                            if ( TrackExpand )
                                ScrollCurrentTvItemIntoView ( childControl );
                            ExpArgs . SearchSuccessItem = childControl;
                            ExpArgs . SearchSuccess = true;
                            if ( TrackExpand )
                                childControl . IsSelected = true;
                            fdl . ShowInfo ( Flowdoc , canvas , "Match found !" );
                            return true;
                        }

                        ShowProgress ( );
                        ShowExpandTime ( );
                        if ( CalculateLevel ( childControl . Tag . ToString ( ) ) > levels )
                            break;

                        if ( FullExpandinProgress == false )
                            ActiveTree . Refresh ( );
                        if ( childControl . Items . Count > 1 )
                        {
                            UpdateListBox ( childControl . Tag . ToString ( ) );
                            TreeViewItem tmp = childControl . Items [ 0 ] as TreeViewItem;
                            if ( tmp . ToString ( ) != "Loading" )
                            {
                                if ( levels == 1 )
                                {
                                    ShowProgress ( );
                                    Selection . Text = $"Calling ExpandFolder for {childControl . Tag . ToString ( )}";
                                    //                                Debug. WriteLine ( Selection . Text );
                                    if ( ExpandFolder ( childControl , true ) == true ) // Expand ALL Contents (true)
                                    {
                                        //ActiveTree . HorizontalAlignment = HorizontalAlignment . Left;
                                        if ( TrackExpand )
                                            ScrollCurrentTvItemIntoView ( childControl );
                                        if ( TrackExpand )
                                            childControl . IsSelected = true;
                                        ExpArgs . SearchSuccess = true;
                                        return true;
                                    }
                                    ShowProgress ( );
                                    if ( FullExpandinProgress == false )
                                        ActiveTree . Refresh ( );
                                }
                                else
                                {
                                    ShowProgress ( );
                                    Selection . Text = $"Calling ExpandAll3 for {childControl . Tag . ToString ( )}";
                                    UpdateListBox ( childControl . Tag . ToString ( ) );
                                    ShowExpandTime ( );
                                    if ( ExpandAll3 ( childControl as TreeViewItem , expand , levels ) == true )
                                    {
                                        //ActiveTree . HorizontalContentAlignment = HorizontalAlignment . Left;
                                        //                                    ScrollCurrentTvItemIntoView ( childControl );
                                        if ( TrackExpand )
                                            childControl . IsSelected = true;
                                        //SearchSuccess = true;
                                        ExpArgs . SearchSuccess = true;
                                        return true;
                                    }
                                    ShowProgress ( );
                                    ShowExpandTime ( );
                                    if ( FullExpandinProgress == false )
                                        ActiveTree . Refresh ( );
                                }
                            }
                            else
                            {
                                UpdateListBox ( childControl . Tag . ToString ( ) );
                                ShowProgress ( );
                                try
                                {
                                    childControl . IsExpanded = true;
                                    //stack . Push ( childControl . Tag . ToString ( ) );
                                }
                                catch ( Exception ex ) { Debug. WriteLine ( $"ExpandAll3: 1503 : {ex . Message}" ); }
                                ShowProgress ( );
                                if ( FullExpandinProgress == false )
                                    ActiveTree . Refresh ( );
                            }
                        }
                        else
                        {
                            ShowProgress ( );
                            try
                            {
                                childControl . IsExpanded = true;
                                //stack . Push ( childControl . Tag . ToString ( ) );
                            }
                            catch ( Exception ex ) { Debug. WriteLine ( $"ExpandAll3: 1517 : {ex . Message}" ); }
                        }
                        ShowProgress ( );
                        if ( FullExpandinProgress == false )
                            ActiveTree . Refresh ( );
                        ShowExpandTime ( );
                    }
                }
                ShowProgress ( );
                if ( FullExpandinProgress == false )
                    ActiveTree . Refresh ( );
            }
            ShowProgress ( );
            ShowExpandTime ( );
            if ( FullExpandinProgress == false )
                ActiveTree . Refresh ( );
            return false;
        }
        private void ExpandAllDrivesBelowCurrent ( object [ ] Args )
        {
            // WORKING for TWO levels onlly
#pragma warning disable CS0219 // The variable 'go' is assigned but its value is never used
            bool go = false;
#pragma warning restore CS0219 // The variable 'go' is assigned but its value is never used
            int levels = ( int ) Args [ 1 ];
            TreeViewItem tv = Args [ 0 ] as TreeViewItem;
            if ( tv == null )
                return;
            List<TreeViewItem> allsubsequent = new List<TreeViewItem> ( );
            List<string> list = new List<string> ( );
            string [ ] drives = Directory . GetLogicalDrives ( );
            bool islaterdrive = false;
            //            ProgressCount = 0;
            ProgressString = "";
            foreach ( var validdrive in drives )
            {
                if ( islaterdrive == false && validdrive != tv . Header . ToString ( ) )
                {
                    ShowProgress ( );
                    continue;
                }
                islaterdrive = true;
                ShowProgress ( );
                ShowExpandTime ( );
                foreach ( TreeViewItem nextdrive in ActiveTree . Items )
                {
                    nextdrive . IsExpanded = true;
                    //stack . Push ( nextdrive . Tag . ToString ( ) );
                    if ( nextdrive . Tag . ToString ( ) == validdrive )
                    {
                        //nextdrive . IsExpanded = true;
                        //stack . Push ( nextdrive.Tag.ToString() );
                        ShowProgress ( );
                        foreach ( TreeViewItem item2 in nextdrive . Items )
                        {
                            item2 . IsExpanded = true;
                            //stack . Push ( item2 . Tag . ToString ( ) );
                            ShowProgress ( );
                            item2 . UpdateLayout ( );
                            ActiveTree . Refresh ( );
                            if ( item2 . Items . Count > 0 )
                            {
                                foreach ( TreeViewItem item3 in item2 . Items )
                                {
                                    item3 . IsExpanded = true;
                                    //stack . Push ( item3 . Tag . ToString ( ) );

                                    //item3 . UpdateLayout ( );
                                    ShowProgress ( );
                                    ActiveTree . Refresh ( );
                                }
                            }
                        }
                        ActiveTree . Refresh ( );
                        ShowExpandTime ( );
                    }
                }
            }
            ActiveTree . Refresh ( );
            //            ProgressCount = 0;
            ProgressString = "Done ...";
            ShowExpandTime ( );
            Mouse . OverrideCursor = Cursors . Arrow;
        }

        // ALL WORKING  REASONABLY CORRECTLY IT APPEARS 14/4/22

        async public void TriggerExpand0 ( object sender , RoutedEventArgs e )
        {
            if ( ActiveTree . SelectedItem == null )
            {
                MessageBox . Show ( $"Please select a drive or subfolder before using  these options...." , "No Drive Selected" );
                return;
            }
            object [ ] Args = { ActiveTree . SelectedItem as TreeViewItem , ( object ) 1 , null };
            startitem = ActiveTree . SelectedItem as TreeViewItem;
            //            ExpandSetup ( true );
            DirectoryOptions . Focus ( );
            ExpanderMenuOption . Text = "Expand Top Level only of Selected Item.";

            // ClearExpandArgs ( );
            ExpArgs . tvitem = ActiveTree . SelectedItem as TreeViewItem;
            ExpArgs . Selection = 0;
            //            ExpArgs . ExpandLevels = 1;
            Debug. WriteLine ( $"TriggerExpand0 :  Expanding {startitem . Tag . ToString ( )}" );
            await Dispatcher . BeginInvoke ( DispatcherPriority . Normal , ( Action ) ( async ( ) => await RunExpandSystem ( Args [ 0 ] , e ) ) );
//            RunExpandSystem ( null , null );
            Thread . Sleep ( 100 );
            return;
        }
        public async void TriggerExpand1 ( object sender , RoutedEventArgs e )
        {
            if ( ActiveTree . SelectedItem == null )
            {
                //MessageBox . Show ( $"Please select a drive or subfolder before using  these options...." , "No Drive Selected" );
                fdl . ShowInfo ( Flowdoc , canvas ,
                      $"Please select a drive or subfolder before using  this option...." ,
                      "Blue1" ,
                      "TreeView Search Sytem" );
                //fdl . SetFocus ( );
                return;
            }
            object [ ] Args = { ActiveTree . SelectedItem as TreeViewItem , ( object ) 2 , null };
            startitem = ActiveTree . SelectedItem as TreeViewItem;
            ExpandSetup ( true );
            DirectoryOptions . Focus ( );
            if ( ExpArgs . SearchActive == false )
            {
                ExpanderMenuOption . Text = "Fully Expand Selected Item 2 levels";
                ClearExpandArgs ( );
                ExpArgs . tvitem = ActiveTree . SelectedItem as TreeViewItem;
                ExpArgs . Selection = 1;
                ExpArgs . ExpandLevels = 2;
            }
            else
            {
                ExpanderMenuOption . Text = "Search for Item down up to 2 levels";
                ExpArgs . Selection = 1;
            }
            await Dispatcher . BeginInvoke ( DispatcherPriority . Normal , ( Action ) ( async ( ) => await RunExpandSystem ( Args [ 0 ] , e ) ) );

//            RunExpandSystem ( null , null );
            return;

            //if ( ExpandCurrentAllLevels ( Args ) == true && TextToSearchFor != "" )
            //    MessageBox . Show ( $"[{Searchtext . Text}] FOUND ...." , "Search System" );
            //Mouse . OverrideCursor = Cursors . Arrow;
            //  ActiveTree . HorizontalContentAlignment = HorizontalAlignment . Left;
            //ScrollCurrentTvItemIntoView ( startitem );
        }
        public async void TriggerExpand2 ( object sender , RoutedEventArgs e )
        {
            if ( ActiveTree . SelectedItem == null )
            {
                //                MessageBox . Show ( $"Please select a drive or subfolder before using  these options...." , "No Drive Selected" );
                fdl . ShowInfo ( Flowdoc , canvas ,
                      $"Please select a drive or subfolder before using  this option...." ,
                      "Blue1" ,
                      "TreeView Search Sytem" );
                return;
            }
            object [ ] Args = { ActiveTree . SelectedItem as TreeViewItem , ( object ) 3 , null };
            startitem = ActiveTree . SelectedItem as TreeViewItem;
            ExpandSetup ( true );
            DirectoryOptions . Focus ( );
            if ( ExpArgs . SearchActive == false )
            {
                ExpanderMenuOption . Text = "Fully Expand Selected Item 3 levels";
                ClearExpandArgs ( );
                ExpArgs . tvitem = ActiveTree . SelectedItem as TreeViewItem;
                ExpArgs . Selection = 2;
                ExpArgs . ExpandLevels = 3;
            }
            else
            {
                ExpanderMenuOption . Text = "Search for item down up to 3 levels";
                ExpArgs . Selection = 2;
            }
            await Dispatcher . BeginInvoke ( DispatcherPriority . Normal , ( Action ) ( async ( ) => await RunExpandSystem ( Args [ 0 ] , e ) ) );
            //RunExpandSystem ( null , null );
            return;
        }
        public async void TriggerExpand3 ( object sender , RoutedEventArgs e )
        {
            if ( ActiveTree . SelectedItem == null )
            {
                //                MessageBox . Show ( $"Please select a drive or subfolder before using  these options...." , "No Drive Selected" );
                fdl . ShowInfo ( Flowdoc , canvas ,
                      $"Please select a drive or subfolder before using  this option...." ,
                      "Blue1" ,
                      "TreeView Search Sytem" );
                return;
            }
            object [ ] Args = { ActiveTree . SelectedItem as TreeViewItem , ( object ) 4 , null };
            DirectoryOptions . Focus ( );
            if ( ExpArgs . SearchActive == false )
            {
                startitem = ActiveTree . SelectedItem as TreeViewItem;
                ExpandSetup ( true );
                ExpanderMenuOption . Text = "Fully Expand Selected Item 4 levels";
                ClearExpandArgs ( );
                ExpArgs . tvitem = ActiveTree . SelectedItem as TreeViewItem;
                ExpArgs . Selection = 3;
                ExpArgs . ExpandLevels = 4;
            }
            else
            {
                ExpanderMenuOption . Text = "Search for Item down to 4 levels";
                ExpArgs . Selection = 3;
            }
            await Dispatcher . BeginInvoke ( DispatcherPriority . Normal , ( Action ) ( async ( ) => await RunExpandSystem ( Args[0],e ) ) );

            //RunExpandSystem ( null , null );
            return;
        }
        public async void TriggerExpand4 ( object sender , RoutedEventArgs e )
        {

            WalkTestTree ( sender , e );
            return;
            // Open ALL levels
#pragma warning disable CS0162 // Unreachable code detected
            if ( ActiveTree . SelectedItem == null )
            {
                //MessageBox . Show ( $"Please select a drive or subfolder before using  these options...." , "No Drive Selected" );
                fdl . ShowInfo ( Flowdoc , canvas ,
                     $"Please select a drive or subfolder before using  this option...." ,
                     "Blue1" ,
                     "TreeView Search Sytem" );
                return;
            }
#pragma warning restore CS0162 // Unreachable code detected
            if ( ExpArgs . SearchActive == false )
            {
                if ( MessageBox . Show ( $"This  can take a *** considerable *** time to complete, and access to the application will not be available until it has completed"
                + ".\n\nAre you sure you want to fully expand the current item ?\n\n" , "Potentially Lengthy Expansion request !" , MessageBoxButton . YesNo , MessageBoxImage . Hand , MessageBoxResult . No ) == MessageBoxResult . No )
                    return;
            }
            object [ ] Args = { ActiveTree . SelectedItem as TreeViewItem , ( object ) 90 , null };
            startitem = ActiveTree . SelectedItem as TreeViewItem;
            ExpandSetup ( true );
            DirectoryOptions . Focus ( );
            if ( ExpArgs . SearchActive == false )
            {
                ExpanderMenuOption . Text = $"Fully Expand Selected Item down {ExpArgs . ExpandLevels} levels";
                ClearExpandArgs ( );
                ExpArgs . tvitem = ActiveTree . SelectedItem as TreeViewItem;
                ExpArgs . Selection = 4;
                ExpArgs . ExpandLevels = 90;
                ExpArgs . ListResults = false;
                LISTRESULTS = true;
            }
            else
            {
                ExpanderMenuOption . Text = $"Search for Item in {ExpArgs . ExpandLevels} levels";
                ExpArgs . Selection = 4;
            }
            await Dispatcher . BeginInvoke ( DispatcherPriority . Normal , ( Action ) ( async ( ) => await RunExpandSystem ( Args [ 0 ] , e ) ) );

//            RunExpandSystem ( null , null );
            return;

        }

        //******************************************************************************************************//
        // Utility method called recursively to open folders by  other expand methods
        //******************************************************************************************************//
        private bool ExpandFolder ( TreeViewItem item , bool ExpandContent = false )
        {
            string fullpath = "";
            if ( AbortExpand )
                return false;
            if ( item . Items . Count > 0 )
            {
                TreeViewItem tmp = item . Items [ 0 ] as TreeViewItem;
                if ( tmp . ToString ( ) != "Loading" )
                {
                    foreach ( TreeViewItem item2 in item . Items )
                    {
                        Thread . Sleep ( SLEEPTIME );

                        ShowProgress ( );
                        if ( CheckSearchSuccess ( item2 . Tag . ToString ( ) ) == true )
                        {
                            UpdateListBox ( $"Search for {Searchtext . Text} found  as [" + item2 . Header . ToString ( ) + $"]\nin {item2 . Tag . ToString ( )}" );
                            if ( TrackExpand )
                                ScrollCurrentTvItemIntoView ( item2 );
                            if ( TrackExpand )
                                item2 . IsSelected = true;
                            ExpArgs . SearchSuccessItem = item2;
                            //SearchSuccess = true;
                            ExpArgs . SearchSuccess = true;
                            return true;
                        }
                        fullpath = item2 . Tag . ToString ( ) . ToUpper ( );
                        try
                        {
                            item2 . IsExpanded = true;
                            //stack . Push ( item2 . Tag . ToString ( ) );
                        }
                        catch ( Exception ex ) { Debug. WriteLine ( $"ExpandFolder : 1797 : {ex . Message}" ); }
                        UpdateListBox ( item2 . Tag . ToString ( ) );
                        ShowProgress ( );
                        ShowExpandTime ( );
                        if ( FullExpandinProgress == false )
                            ActiveTree . Refresh ( );
                    }
                    UpdateExpandprogress ( );
                    ShowExpandTime ( );
                }
            }
            return false;
        }

        #region Expanding support methods
        private void UpdateExpandprogress ( )
        {
            if ( Expandprogress . Text . Length >= 12 )
                Expandprogress . Text = ".";
            else
                Expandprogress . Text += ".";
            Thread . Sleep ( 10 );
            Expandprogress . UpdateLayout ( );
            Thread . Sleep ( 10 );
        }
        private static bool IsSystemFile ( string entry )
        {
            if ( entry . Contains ( "BOOT" )
                || entry . Contains ( "SYSTEM VOLUME INFORMATION" )
                || entry . Contains ( "$WINDOWS" )
                || entry . Contains ( "PAGEFILE.SYS" )
                || entry . Contains ( "HIBERFIL.SYS" )
                || entry . Contains ( "DUMPSTACK" )
                || entry . Contains ( ".RND" )
                || entry . Contains ( "$GETCURRENT" )
                || entry . Contains ( "$WINREAGENT" )
                || entry . Contains ( "WINDOWS.OLD" )
                || entry . Contains ( "CONFIG.MSI" )
                || entry . Contains ( "RECOVERY.TXT" )
                || entry . Contains ( "$RECYCLE.BIN" ) == true )
            {
                return true;
            }
            else
                return false;
        }
        private void loadExpandOptions ( )
        {
            DirOptions = new List<string> ( );
            //           DirOptions . Add ( "Expand current Item down 2 levels \n-> Root \n->       Subfolders\n           ....  Subfolders\n           ....  Files\n-               ....  Subfolders\n                 ....Files\n        ....  Files\n (Reasonably Fast...)" );
            //            DirOptions . Add ( "Expand current Item down 3 levels\n-> Root \n->    SubFolders\n->       Subfolders\n             ....  Subfolders\n                   ....  Subfolders\n                   ....  Files\n-                 ....  Subfolders\n                   ....Files\n             ....  Files\n (Can take  while...)" );
            DirOptions . Add ( "Fully Expand Current Drive 2 Levels" );
            DirOptions . Add ( "Fully Expand Current Drive 3 Levels" );
            DirOptions . Add ( "Fully Expand Current Drive 4 Levels" );
            DirOptions . Add ( "Fully Expand Current Drive ALL Levels" );
            DirOptions . Add ( "Expand ALL Drives down 1 level\n-> Root \n->    SubFolders\n->       ....  Subfolders\n            ....  Files\n->.Files\n (Quite fast...)" );
            DirOptions . Add ( "Expand ALL Drives down 2 levels \n-> Root \n->    SubFolders\n         Subfolders\n            Files\n      ....  .Files \n (WARNIING - May take some time....)" );
            DirOptions . Add ( "Fully Expand All Drives below current ? levels\n  (WARNING - Can take quite some time !" );
            DirOptions . Add ( "Collapse All Drives" );
            DirectoryOptions . Items . Clear ( );
            DirectoryOptions . ItemsSource = DirOptions;
            DirectoryOptions . SelectedIndex = 0;
            DirectoryOptions . SelectedItem = 0;
            ComboSelectedItem = 0;
        }
        private void LoadSearchLevels ( )
        {
            LevelsCombo . Items . Add ( "2" );
            LevelsCombo . Items . Add ( "3" );
            LevelsCombo . Items . Add ( "4" );
            LevelsCombo . Items . Add ( "5" );
            LevelsCombo . Items . Add ( "90" );
        }

        #endregion Expanding support mmethods


        private void Sw_Tick ( object sender , EventArgs e )
        {
            //TimeElapsed = temp;
        }

        #endregion Expand // Collapse

        private void ShowDriveInfo ( object sender , RoutedEventArgs e )
        {
            string output = "";
            ExplorerClass Texplorer = new ExplorerClass ( );
            Texplorer . GetDrives ( "C:\\" );
            List<lbitemtemplate> lbtmplates = new List<lbitemtemplate> ( );
            InfoList . ItemsSource = null;
            InfoList . Items . Clear ( );

            // Create list of drive info in individual Templates & add to array of templates
            for ( int x = 0 ; x < Texplorer . Drives . Count ; x++ )
            {
                DriveInfo [ ] driveinfo = DriveInfo . GetDrives ( );
                List<string> drives = Texplorer . GetDrives ( Texplorer . Drives [ x ] );
                Texplorer . GetDirectories ( drives [ x ] );
                Texplorer . GetFiles ( drives [ x ] );
                lbitemtemplate lbtmp = new lbitemtemplate ( );
                if ( driveinfo [ x ] . IsReady == true )
                {
                    output += $"Drive [{Texplorer . Name}, Volume Label = {driveinfo [ x ] . VolumeLabel}, Type = {driveinfo [ x ] . DriveType}, Format = {driveinfo [ x ] . DriveFormat}, " +
                        $"Directories = {Texplorer . Directories . Count}, Files = {Texplorer . Files . Count}\n";
                    lbtmp . Colm1 = $"Drive [{Texplorer . Name}]";
                    lbtmp . Colm2 = $"Drive Type = [{driveinfo [ x ] . DriveType}]";
                    lbtmp . Colm3 = $"Volume label = [{driveinfo [ x ] . VolumeLabel}]";
                    lbtmp . Colm4 = $"Format = [{driveinfo [ x ] . DriveFormat}]";
                    lbtmp . Colm5 = $"Directories = [{Texplorer . Directories . Count}]";
                    lbtmp . Colm6 = $"Files[{Texplorer . Files . Count}]";
                }
                else
                {
                    output += $"NOT READY : Drive [{Texplorer . Name}, Type = {driveinfo [ x ] . DriveType}\n";
                    lbtmp . Colm1 = $"Drive [{Texplorer . Name} ";
                    lbtmp . Colm2 = $"Drive Type = {driveinfo [ x ] . DriveType}";
                    lbtmp . Colm3 = $" DRIVE NOT READY!!";
                }
                lbtmplates . Add ( lbtmp );
                continue;

            }
            InfoList . ItemsSource = null;
            InfoList . ItemsSource = lbtmplates;
        }
        private void TREEViews_KeyDown ( object sender , KeyEventArgs e )
        {
            if ( e . Key == Key . F5 )
                BreakExpand = true;
        }
        private void DirectoryOptions_Selected ( object sender , RoutedEventArgs e )
        {
            ComboBox cb = sender as ComboBox;
            ExpanderMenuOption . Text = cb . SelectedItem . ToString ( );
        }
        private void CreateTreeViewData ( string DriveToLoad , List<string> dirs , List<string> files )
        {
            List<string> currentdir = new List<string> ( );
            List<string> currentfile = new List<string> ( );
            int iterator = 0;
            ActiveTree . Items . Clear ( );

            foreach ( var drive in Directory . GetLogicalDrives ( ) )
            {
                if ( drive != DriveToLoad && DriveToLoad != "ALL" )
                    continue;
                iterator++;
                var item = new TreeViewItem ( );
                item . Header = drive;
                item . Tag = drive;

                // Add Dummy entry so we get an "Can be Opened" triangle icon
                int dircount = GetDirectories ( drive , out List<string> directories );
                if ( dircount > 0 )
                {
                    item . Items . Add ( "Loading" );

                    // Add Drive to Treeview with dummy "Loading" item
                    ActiveTree . Items . Add ( item );
                }
                continue;
            }
        }
        private void listBox_PreviewMouseRightButtonDown ( object sender , MouseButtonEventArgs e )
        {

            //ListBox tv = sender as ListBox;
            //if ( tv . Visibility == Visibility . Visible )
            //{
            //    tv . Visibility = Visibility . Hidden;
            //    xtreeView4 . Visibility = Visibility . Visible;
            //}
            //else
            //{
            //    tv . Visibility = Visibility . Visible;
            //    xtreeView4 . Visibility = Visibility . Hidden;
            //}
        }
        private void TestTree_PreviewMouseRightButtonDown ( object sender , MouseButtonEventArgs e )
        {
            return;
        }
        private void DirectoryOptions_Selected ( object sender , SelectionChangedEventArgs e )
        {
            ExpanderMenuOption . Text = $"{DirectoryOptions . SelectedItem . ToString ( )}";
            int current = DirectoryOptions . SelectedIndex;
            ComboBox cb = sender as ComboBox;
            ComboSelectedItem = current;
           // var  sp = cb . Items [ ComboSelectedItem ];
            //var v = cb . ItemsHost as WrapPanel;
            //ComboSelectedItem = e.OriginalSource;

        }
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        async private void WalkTestTree ( object sender , RoutedEventArgs e )
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            Args [ 0 ] = ActiveTree . SelectedItem as TreeViewItem;
            Args [ 1 ] = 90;
            //SelectedTVItem = ActiveTree . SelectedItem as TreeViewItem;
            SearchString = "";
            ExpandSelection = 4;
            ClearExpandArgs ( );
            ExpArgs . tvitem = ActiveTree . SelectedItem as TreeViewItem;
            ExpArgs . Selection = 4;
            ExpArgs . ExpandLevels = 90;
//          await Dispatcher . BeginInvoke ( DispatcherPriority . Normal , ( Action ) ( async ( ) => await RunExpandSystem ( Args [ 0 ] , e ) ) );
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
            RunExpandSystem ( sender , e );
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
        }

        #region Expand Utility methods
        public int GetCurrentLevel ( string currentpath )
        {
            int count = 0;
            string [ ] paths = currentpath . Split ( '\\' );
            count = paths . Length - 1;
            return count;
        }
        public void ShowProgress ( )
        {
            if ( ProgressCount < PROGRESSWRAPVALUE )
                ProgressCount++;
            else
                ProgressCount = 0;
        }
        public void StartTimer ( )
        {
            startmin = DateTime . Now . Minute;
            startsec = DateTime . Now . Second;
            startmsec = DateTime . Now . Millisecond;
        }
        public void StopTimer ( )
        {
            startmin = 0;
            startsec = 0;
            startmsec = 0;
        }
        public void ShowExpandTime ( )
        {
            //Working fine, updates an Att.Prop that is Binded  to display field
            double endmin = DateTime . Now . Minute;
            double endsec = DateTime . Now . Second;
            double endmsec = DateTime . Now . Millisecond;
            double total = 0;
            double rsecs = 0, rmin = 0, rmsecs = 0;
            //Debug. WriteLine ( $"{startmin}, {startsec}, {startmsec}" );
            //Debug. WriteLine ( $"{endmin}, {endsec}, {endmsec}" );
            if ( endmin >= startmin )
                rmin = endmin - startmin;
            else
                rmin = endmin + ( 60 - startmin );

            if ( endsec > startsec )
                rsecs = endsec - startsec;// > 0 ? ( endsec - startsec ) : 0;
            else
            {
                // end Seconds less then start secs
                rsecs = ( 60 - startsec ) + endsec;
                if ( rmin == ( double ) 1 )
                {
                    rmin = rmin - 1;
                    rsecs = ( rmin * 60 ) + rsecs;
                }
                else
                {
                    rsecs = endsec - startsec;
                }
            }
            if ( endmsec > startmsec )
                rmsecs = endmsec - startmsec; //> 0 ? ( endmsec - startmsec ) : 0;
            else
                rmsecs = ( 1000 - startmsec ) + endmsec;
            //            Debug. WriteLine ($"{rmin}, {rsecs}, {rmsecs}\n");
            BreakExpand = false;
            total = ( rmin * 60 );
            total += rsecs;
            string restime = String . Format ( "{0:0.00}" , ( ( total * 1000 ) + rmsecs ) / 1000 ) + " secs";
            //            SetExpandDuration ( this , restime );
            ExpandDuration = restime;
        }
        public int CalculateLevel ( string currentitem )
        {
            int len = 0;
            string [ ] levels = currentitem . Split ( '\\' );
            if ( levels [ 1 ] == "" )
                return 1;
            else len = levels . Length;
            //            else len = levels . Length - 1;
            return len;
        }
        private void ExpandSetup ( bool direction )
        {
            if ( direction )
            {
                //                ProgressCount = 0;
                Duration . Text = "";
                ProgressCount = 0;
                ProgressString = "";
                Listboxtotal = 0;
                StartTimer ( );
                //                listBox . Items . Clear ( );
                //                listBox . Refresh ( );
                Thickness th = new Thickness ( 2 , 2 , 2 , 2 );
                Expandprogress . Text = ".";
                Expandprogress . BorderThickness = th;
                Expandcounter . BorderThickness = th;
                Expandprogress . BorderBrush = FindResource ( "Red5" ) as SolidColorBrush;
                Expandcounter . Foreground = FindResource ( "Yellow1" ) as SolidColorBrush;
                Expandprogress . UpdateLayout ( );
                Expandcounter . BorderBrush = FindResource ( "Red5" ) as SolidColorBrush;
                Expandcounter . Text = "";
                Expandcounter . UpdateLayout ( );
                BusyLabel . Visibility = Visibility . Visible;
                treeViews . Cursor = Cursors . Wait;
            }
            else
            {
                Expandprogress . BorderBrush = FindResource ( "White0" ) as SolidColorBrush;
                Expandcounter . BorderBrush = FindResource ( "White0" ) as SolidColorBrush;
                Expandcounter . Foreground = FindResource ( "White4" ) as SolidColorBrush;
                Thickness th = new Thickness ( 0 , 0 , 0 , 0 );
                Expandprogress . BorderThickness = th;
                Expandcounter . BorderThickness = th;
                //                BusyLabel . Visibility = Visibility . Hidden;

                if ( ExpArgs . SearchSuccess == false )
                    UpdateListBox ( $"\nExpansion completed successfully ..." );
                TotalItemsExpanded = 0;
                ShowExpandTime ( );
                Expandprogress . Refresh ( );
                Expandcounter . Refresh ( );
                UpdateListBox ( "" );
                if ( ExpArgs . SearchSuccess )
                    UpdateListBox ( $"Around {Expandcounter . Text } objects have been\nOpened & Searched..." );
                else
                    UpdateListBox ( $"Around {Expandcounter . Text } objects have been Expanded..." );
                Expandprogress . Text = "Finished ...";
                vsplitterright . Cursor = Cursors . SizeWE;
                hsplitter . Cursor = Cursors . SizeNS;
                treeViews . Cursor = Cursors . Arrow;
            }
        }
        public void ScrollCurrentTvItemIntoView ( TreeViewItem item )
        {
            // Brings selected item  into view as selected item
            var count = VisualTreeHelper . GetChildrenCount ( item );

            for ( int i = count - 1 ; i >= 0 ; --i )
            {
                var childItem = VisualTreeHelper . GetChild ( item , i );
                if ( childItem != ( DependencyObject ) null )
                    ( ( FrameworkElement ) childItem ) . BringIntoView ( );
                //item.BringIntoView ( );
            }
        }
        public void ScrollTvItemIntoView ( TreeViewItem item )
        {
            item . BringIntoView ( );
        }

        #endregion Expand Utility methods

        #region Failed Search fom M$$$$$$

        /// <summary>
        /// Recursively search for an item in this subtree.
        /// </summary>
        /// <param name="container">
        /// The parent ItemsControl. This can be a TreeView or a TreeViewItem.
        /// </param>
        /// <param name="item">
        /// The item to search for.
        /// </param>
        /// <returns>
        /// The TreeViewItem that contains the specified item.
        /// </returns>
        private TreeViewItem GetTreeViewItem ( ItemsControl container , object item )
        {
            if ( container != null )
            {
                if ( container . DataContext == item )
                {
                    return container as TreeViewItem;
                }

                // Expand the current container
                if ( container is TreeViewItem && !( ( TreeViewItem ) container ) . IsExpanded )
                {
                    container . SetValue ( TreeViewItem . IsExpandedProperty , true );
                }

                // Try to generate the ItemsPresenter and the ItemsPanel.
                // by calling ApplyTemplate.  Note that in the
                // virtualizing case even if the item is marked
                // expanded we still need to do this step in order to
                // regenerate the visuals because they may have been virtualized away.

                container . ApplyTemplate ( );
                ItemsPresenter itemsPresenter =
                    ( ItemsPresenter ) container . Template . FindName ( "ItemsHost" , container );
                if ( itemsPresenter != null )
                {
                    itemsPresenter . ApplyTemplate ( );
                }
                else
                {
                    // The Tree template has not named the ItemsPresenter,
                    // so walk the descendents and find the child.
                    itemsPresenter = FindVisualChild<ItemsPresenter> ( container );
                    if ( itemsPresenter == null )
                    {
                        container . UpdateLayout ( );

                        itemsPresenter = FindVisualChild<ItemsPresenter> ( container );
                    }
                }

                Panel itemsHostPanel = ( Panel ) VisualTreeHelper . GetChild ( itemsPresenter , 0 );

                // Ensure that the generator for this panel has been created.
                UIElementCollection children = itemsHostPanel . Children;

                //                MyVirtualizingStackPanel virtualizingPanel =
                //                    itemsHostPanel as MyVirtualizingStackPanel;

                for ( int i = 0, count = container . Items . Count ; i < count ; i++ )
                {
                    TreeViewItem subContainer;
                    //if ( virtualizingPanel != null )
                    //{
                    //    // Bring the item into view so
                    //    // that the container will be generated.
                    //    virtualizingPanel . BringIntoView ( i );

                    //    subContainer =
                    //        ( TreeViewItem ) container . ItemContainerGenerator .
                    //        ContainerFromIndex ( i );
                    //}
                    //else
                    {
                        subContainer = ( TreeViewItem ) container . ItemContainerGenerator . ContainerFromIndex ( i );

                        // Bring the item into view to maintain the
                        // same behavior as with a virtualizing panel.
                        subContainer?.BringIntoView ( );
                    }

                    if ( subContainer != null )
                    {
                        // Search the next level for the object.
                        TreeViewItem resultContainer = GetTreeViewItem ( subContainer , item );
                        if ( resultContainer != null )
                        {
                            return resultContainer;
                        }
                        else
                        {
                            // The object is not under this TreeViewItem
                            // so collapse it.
                            if ( subContainer != null )
                                subContainer . IsExpanded = false;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Search for an element of a certain type in the visual tree.
        /// </summary>
        /// <typeparam name="T">The type of element to find.</typeparam>
        /// <param name="visual">The parent element.</param>
        /// <returns></returns>
        private T FindVisualChild<T> ( Visual visual ) where T : Visual
        {
            for ( int i = 0 ; i < VisualTreeHelper . GetChildrenCount ( visual ) ; i++ )
            {
                Visual child = ( Visual ) VisualTreeHelper . GetChild ( visual , i );
                if ( child != null )
                {
                    T correctlyTyped = child as T;
                    if ( correctlyTyped != null )
                    {
                        return correctlyTyped;
                    }

                    T descendent = FindVisualChild<T> ( child );
                    if ( descendent != null )
                    {
                        return descendent;
                    }
                }
            }

            return null;
        }
        #endregion Failed Search fom M$$$$$$

        private void listBox_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {
#pragma warning disable CS0219 // The variable 'str' is assigned but its value is never used
            string str = "";
#pragma warning restore CS0219 // The variable 'str' is assigned but its value is never used
#pragma warning disable CS0219 // The variable 'iterate' is assigned but its value is never used
            bool iterate = false;
#pragma warning restore CS0219 // The variable 'iterate' is assigned but its value is never used
            ListBox tb = sender as ListBox;
            tb . ScrollIntoView ( tb . SelectedItem );
            tb . HorizontalAlignment = HorizontalAlignment . Left;
            //foreach ( var item in tb.Items )
            //{
            //    if(item.)
            //}
            //if ( tb . SelectedIndex != -1 )
            //{ 
            //    if ( Selindex != -1 && iterate == false)
            //    {
            //        var  lastone = tb . Items [ Selindex ];
            //        iterate = true;
            //        tb . SelectedItem = lastone;
            //        iterate = false;
            //        (lastone as TextBlock).Background = FindResource ( "White3" ) as SolidColorBrush;
            //        Selindex = -1;
            //    }

            //    Selindex = tb . SelectedIndex;
            //    TextBlock lbi = tb . SelectedItem as TextBlock;
            //    lbi . Background = FindResource ( "Black0" ) as SolidColorBrush;
            //    return;
            //}
        }
        private bool CheckFileForMatch ( List<string> files , string upperstring , out string resstring )
        {
#pragma warning disable CS0219 // The variable 'result' is assigned but its value is never used
            bool result = false;
#pragma warning restore CS0219 // The variable 'result' is assigned but its value is never used
            resstring = "";
            foreach ( var filename in files )
            {
                Debug. WriteLine ( $"? FILE match[{filename . ToUpper ( )}]" );
                if ( filename . ToUpper ( ) . Contains ( upperstring ) == true )
                {
                    resstring = filename;
                    return true;
                }
            }
            return false;
        }
        private bool CheckFolderForMatch ( string folder , string upperstring , out string resultstring )
        {
            bool result = false;
            resultstring = "";
            List<string> subfolders = new List<string> ( );
            List<string> files = new List<string> ( );
#pragma warning disable CS0219 // The variable 'resstring' is assigned but its value is never used
            string resstring = "";
#pragma warning restore CS0219 // The variable 'resstring' is assigned but its value is never used

            Debug. WriteLine ( $"? FOLDER match [{folder}]" );
            TreeViewItem tvfound = new TreeViewItem ( );
            //tvfound . Tag = folder;
            //tvfound . IsExpanded = true;
            if ( folder . Contains ( upperstring ) == true )
            {
                tvfound . IsExpanded = true;
                return true;
            }
            GetFiles ( folder , out files );
            if ( CheckFileForMatch ( files , upperstring , out resultstring ) == true )
            {
                tvfound . Tag = resultstring;
                tvfound . IsSelected = true;
                return true;
            }
            GetDirectories ( folder , out subfolders );
            foreach ( var filename in subfolders )
            {
                Debug. WriteLine ( $"? ? FOLDER match [{filename}]" );
                if ( filename . ToUpper ( ) . Contains ( upperstring ) == true )
                {
                    result = true;
                    break;
                }
                else
                {
                    Debug. WriteLine ( $"? ? iterating inner FOLDER match [{filename}]" );
                    if ( CheckFolderForMatch ( filename , upperstring , out resultstring ) == true )
                    {
                        tvfound . Tag = resultstring;
                        tvfound . IsSelected = true;
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }
        private void Searchtext_PreviewMouseDown ( object sender , MouseButtonEventArgs e )
        {
            if ( Searchtext . Text == "Search for...." )
                Searchtext . Text = "";
        }
        private string GetParentPath ( string tag )
        {
            string [ ] items = tag . Split ( '\\' );
            string newpath = "";
            int max = items . Count ( ) - 1;
            for ( int x = 0 ; x <= max ; x++ )
            {
                if ( x < max )
                    newpath += items [ x ] + "\\";
                else if ( x == max - 1 )
                    newpath += items [ x ];
            }
            return items [ 0 ];
        }
        private void CollapseAllDrives ( )
        {
            Mouse . OverrideCursor = Cursors . Wait;
            TreeView tv = new TreeView ( );
            if ( ExpArgs . tv != null )
                tv = ExpArgs . tv;
            else
                tv = ActiveTree;
            //    TreeView tv = treeView4;
            foreach ( TreeViewItem item in tv . Items )
            {
                if ( item . IsExpanded )
                    item . IsExpanded = false;
                //ExpandAll3 ( rootTreeViewItem , false );
            }
            //ShowExpandTime ( );
            //            ActiveTree . HorizontalContentAlignment = HorizontalAlignment . Left;
            tv . Refresh ( );
            Mouse . OverrideCursor = Cursors . Arrow;
        }
        public bool CheckSearchSuccess ( string currentitem )
        {
            bool result = false;
            currentitem = currentitem . ToUpper ( );
            if ( ExpArgs . SearchSuccess == true || ExpArgs . SearchTerm == "" )
                return false;
            //if ( currentitem . Contains ( "BG-BG" ) )
            //    Debug. WriteLine ( $"" );
            if ( Exactmatch )
            {
                result = currentitem == ExpArgs . SearchTerm;
            }

            else
            {
                if ( currentitem . Contains ( ExpArgs . SearchTerm ) )
                    result = true;
                else
                    result = false;
            }

            return result;
        }
        private void ShowFullPath ( object sender , RoutedEventArgs e )
        {
            TreeViewItem tvi = ActiveTree . SelectedItem as TreeViewItem;
            MessageBoxResult result = MessageBox . Show ( $"{tvi . Tag . ToString ( )}\n\nClick YES to save to ClipBoard ..." , "Full path of current  item"
             , MessageBoxButton . YesNo , MessageBoxImage . Question , MessageBoxResult . No );
            if ( result == MessageBoxResult . Yes )
                Clipboard . SetText ( tvi . Tag . ToString ( ) );
        }
        private void CollapseTree ( object sender , RoutedEventArgs e )
        {
            object [ ] Args = { "C:\\" , null , null };
            Args [ 0 ] = ActiveTree as object;
            ExpArgs . tv = ActiveTree;
            ExpandSetup ( true );
            CollapseAllDrives ( );
            Mouse . OverrideCursor = Cursors . Arrow;
            ExpandSetup ( false );
        }
        private void InfoList_Scroll ( object sender , System . Windows . Controls . Primitives . ScrollEventArgs e )
        {
            ListBox lb = sender as ListBox;
            lb . HorizontalContentAlignment = HorizontalAlignment . Left;
        }

        private void CreateBrushes ( )
        {
            Brush0 = FindResource ( "White0" ) as SolidColorBrush;
            Brush1 = FindResource ( "White3" ) as SolidColorBrush;
            Brush2 = FindResource ( "Red6" ) as SolidColorBrush;
            Brush3 = FindResource ( "Black0" ) as SolidColorBrush;
            Brush4 = FindResource ( "Blue3" ) as SolidColorBrush;
            Brush5 = FindResource ( "Orange1" ) as SolidColorBrush;
            Brush6 = FindResource ( "Magenta3" ) as SolidColorBrush;
            Brush7 = FindResource ( "Black7" ) as SolidColorBrush;
            Brush8 = FindResource ( "Green2" ) as SolidColorBrush;
        }

        #region Search
        private TreeViewItem [ ] SearchSubDir ( TreeViewItem item , string SearchTerm , out TreeViewItem MatchingItem , bool AddFolders )
        {
            //    bool result = false;
            //    bool found = false;
            //    // Save main calling item so we can get back  to it later on
            //    TreeViewItem CallingItem = item;
            //    TreeViewItem MatchItem = null;
            MatchingItem = null;

            //    // see if the parent item matches ?
            //    if ( CheckForMatchingItem ( item , SearchTerm ) )
            //    {
            //        SearchSuccess = true;
            //        return null;
            //    }
            //    if ( AddFolders )
            //    {
            //        List<string> directories = new List<string> ( );
            //        int count = GetDirectories ( item . Tag . ToString ( ) , out directories );

            //        AddDirectoriesToTestTreeview ( directories , item , listBox );
            //        AddFilesToSubdirectory ( item );
            //        item . IsExpanded = true;
            //        ActiveTree .Refresh ( );
            //    }
            //    foreach ( TreeViewItem itms in item . Items )
            //    {
            //        if ( CheckForMatchingItem ( itms , SearchTerm ) )
            //        {
            //            SearchSuccess = true;
            //            return null;
            //        }
            //        if ( itms . HasItems )
            //        {
            //            itms . IsExpanded = true;
            //            AddFilesToSubdirectory ( itms );
            //            if ( SearchIterate ( itms , SearchTerm ) == null )
            //                continue;
            //        }
            //    }
            return null;
        }
        private void AddFilesToSubdirectory ( TreeViewItem item )
        {
            List<string> files = new List<string> ( );
            GetFiles ( item . Tag . ToString ( ) , out files );
            if ( item . Items [ 0 ] . ToString ( ) == "Loading" )
                item . Items . Clear ( );
            if ( files . Count > 0 )
            {
                for ( int y = 0 ; y < files . Count ; y++ )
                {
                    item . Items . Add ( files [ y ] );
                    item . IsExpanded = true;
                    ScrollCurrentTvItemIntoView ( item );
                    ActiveTree . Refresh ( );
                }
            }
            else
            {
                // No Directories or files, so remove "Loading" Dummy
                if ( item . Items . Count == 0 )
                    item . Items . Clear ( );
            }
        }
        // NOT USED
        private TreeViewItem SearchIterate ( TreeViewItem item , string SearchTerm )
        {
#pragma warning disable CS0219 // The variable 'result' is assigned but its value is never used
            bool result = false;
#pragma warning restore CS0219 // The variable 'result' is assigned but its value is never used
#pragma warning disable CS0219 // The variable 'found' is assigned but its value is never used
            bool found = false;
#pragma warning restore CS0219 // The variable 'found' is assigned but its value is never used
            // Save main calling item so we can get back  to it later on
            TreeViewItem CallingItem = item;
            TreeViewItem MatchItem = null;

            // Is this a match ?
            if ( CheckForMatchingItem ( item , SearchTerm ) )
            {
                //                SearchSuccess = true;
                ExpArgs . SearchSuccess = true;
                return null;
            }

            // no, so list all remaining subdirs
            foreach ( TreeViewItem itms in item . Items )
            {
                if ( itms . Header . ToString ( ) == "Loading" )
                    return null;
                if ( itms . Tag . ToString ( ) . ToUpper ( ) . Contains ( SearchTerm ) )
                {
                    //UpdateListBox ( $"\nSearch for {Searchtext . Text} found as [\" + { itms . Header . ToString ( ) }\"] \nin {itms . Tag . ToString ( )}" );
                    itms . IsSelected = true;
                    ScrollCurrentTvItemIntoView ( itms );
                    //                    SearchSuccess = true;
                    ExpArgs . SearchSuccess = true;
                    ActiveTree . Refresh ( );
                    return itms;
                }
                if ( itms . HasItems )
                {

                }
            }
            return MatchItem;
        }
        private bool CheckForMatchingItem ( TreeViewItem item , string SearchTerm )
        {
            if ( item . HasItems )
            {
                foreach ( TreeViewItem itm in item . Items )
                {
                    if ( itm . HasItems )
                    {
                        if ( itm . Tag . ToString ( ) . ToUpper ( ) . Contains ( SearchTerm ) )
                        {
                            //                            UpdateListBox ( $"Search for {Searchtext . Text} found  as [\" + { itm . Header . ToString ( ) }\"] \nin {itm . Tag . ToString ( )}" );
                            itm . IsSelected = true;
                            ScrollCurrentTvItemIntoView ( itm );
                            //SearchSuccess = true;
                            ExpArgs . SearchSuccess = true;
                            ActiveTree . Refresh ( );
                            return true;

                        }
                    }
                }
            }
            return false;
        }
        #endregion Search

        private int testcount = 0;
        public void UpdateListBox ( string entry )
        {
            testcount++;
            if ( testcount < 25 )
                Debug. WriteLine ( $"{entry}" );
            if ( LISTRESULTS == false )
            {
                if ( listBox . Items . Count == 0 )
                {
                    if ( ExpArgs . SearchActive )
                        listBox . Items . Add ( "Logging to this List is automatically disabled\nfor all Search operations to improve the\nspeed of  the search ..." );
                    else if ( LISTRESULTS == false )
                    {
                        listBox . Items . Add ( "Logging to this List is currently disabled" );
                        listBox . Items . Add ( "Right click to access TreeView Options to change  this" );
                        //                        listBox . Items . Add ( "from the TreeView Options to change  this..." );
                    }
                }
                return;
            }
            TextBlock tblk = new TextBlock ( );
            LbTextblock = tblk;
            string [ ] items;
            items = entry . Split ( '\\' );
            if ( entry . Contains ( "." ) && entry . Contains ( "All " ) == false )
            {
                //LIGHT Black on gray- Filles
                tblk . Foreground = Brush3;
                tblk . Background = Brush1;
                tblk . FontWeight = FontWeights . Normal;
                string [ ] dot = entry . Split ( '\\' );
                if ( dot . Length == 2 )
                    entry = dot [ dot . Length - 1 ];
                else if ( dot . Length >= 3 )
#pragma warning disable CS1717 // Assignment made to same variable; did you mean to assign something else?
                    entry = entry;
#pragma warning restore CS1717 // Assignment made to same variable; did you mean to assign something else?
                else
                    entry = dot [ dot . Length - 1 ];
            }
            else if ( items . Length == 2 && items [ 1 ] == "" )
            {
                //BOLD White on Red background - Drives
                tblk . Foreground = Brush0;
                tblk . Background = Brush2;
                tblk . FontWeight = FontWeights . DemiBold;
            }
            else if ( items . Length == 2 && items [ 1 ] != "" )
            {
                //NORMAL Red on Gray - C:\\xxxxx
                tblk . Foreground = Brush2;
                tblk . Background = Brush1;
                tblk . FontWeight = FontWeights . Normal;
            }
            else if ( items . Length == 3 )
            {
                //NORMAL Magenta on gray - C:\\xxxx\\xxxx
                tblk . Foreground = Brush6;
                tblk . Background = Brush1;
                tblk . FontWeight = FontWeights . Normal;
            }
            else if ( items . Length == 4 )
            {
                //LIGHTFONT - Orange on gray - C:\\xxx\\xxx\\xxx
                tblk . Foreground = Brush5;
                tblk . Background = Brush1;
                tblk . FontWeight = FontWeights . Light;
            }
            else if ( items . Length == 5 )
            {
                //NORMAL Orange on gray - C:\\xxx\\xxx\\xxx
                tblk . Foreground = Brush5;
                tblk . Background = Brush1;
                tblk . FontWeight = FontWeights . Normal;
                //Debug. WriteLine ( );
            }
            else
            {
                //NORMAL Blue on gray
                tblk . Foreground = Brush3;
                tblk . Background = Brush1;
                tblk . FontWeight = FontWeights . Normal;
            }

            tblk . Text = entry;
            // Doing it this way forces the  listbox  to remain LEFT Justified even with long lines
            ListBoxItem lbi = new ListBoxItem ( );
            lbi . Content = tblk;
            lbi . HorizontalAlignment = HorizontalAlignment . Left;
            int currindex = listBox . Items . Add ( lbi );

            //listBox . SelectedIndex = currindex;
            //listBox . SelectedItem = currindex;
            //// Bound in xaml
            Listboxtotal++;
            ShowExpandTime ( );
            if ( entry != "" && entry . Length > 1 )
            {
                if ( entry . Substring ( 1 , 1 ) == ":" )
                    TotalItemsExpanded++;
            }
            //ListBoxItem current = listBox . SelectedItem as ListBoxItem;
            //if ( current != null )
            //{
            //    ListBoxItem lbitem = listBox . Items [ currindex - 1 ] as ListBoxItem;
            //    listBox . Refresh ( );
            //}
            //listBox . HorizontalContentAlignment = HorizontalAlignment . Left;
            //listBox . UpdateLayout ( );
        }

        /// <summary>
        /// Main method for expanding  folders, or searching for items
        /// </summary>
        /// <param name="Args"></param>
        /// <returns></returns>
        async public Task<bool> ExpandCurrentAllLevels ( object [ ] Args )
        {

            await ExpandCurrentAllLevelsTask ( Args );
            return true;


#pragma warning disable CS0162 // Unreachable code detected
            bool Returnval = false;
#pragma warning restore CS0162 // Unreachable code detected
            bool IsComplete = false;
            int iterations = 0;
            int itemcount = 0;
            int levelscount = 0;
#pragma warning disable CS0219 // The variable 'fail' is assigned but its value is never used
            var fail = false;
#pragma warning restore CS0219 // The variable 'fail' is assigned but its value is never used
#pragma warning disable CS0219 // The variable 'success' is assigned but its value is never used
            var success = true;
#pragma warning restore CS0219 // The variable 'success' is assigned but its value is never used
            int levels = ( int ) ExpArgs . ExpandLevels;
            TreeViewItem items = ExpArgs . tvitem;
            startitem = items;
            if ( items == null )
                return false;
            //ProgressCount = 0;
            string TagString = items . Tag . ToString ( ) . ToUpper ( );
            if ( TagString . Contains ( items . Header . ToString ( ) . ToUpper ( ) ) == false )
                items . Header = items . Tag . ToString ( );
            try
            {
                items . IsExpanded = true;
            }
            catch ( Exception ex ) { Debug. WriteLine ( $"ExpandCurrentAllLevels: 2736 : {ex . Message}" ); }

            ShowProgress ( );
            if ( FullExpandinProgress == false )
                ActiveTree . Refresh ( );
            levelscount = CalculateLevel ( TagString );
            if ( levelscount >= ExpArgs . ExpandLevels )
            {
                ShowExpandTime ( );
                ExpandSetup ( false );
                Expandprogress . Refresh ( );
                return false;
            }
            //**************
            // Main LOOP
            //**************
            foreach ( object objct in items . Items )
            {
                int currentcount = 0;
                TreeViewItem obj = objct as TreeViewItem;
                currentcount = obj . Items . Count;
                //    TreeViewItem obj = objct as TreeViewItem;
                if ( obj == null || obj . ToString ( ) == "Loading" )
                    break;
                ShowProgress ( );
                //levelscount = CalculateLevel ( obj . Tag . ToString ( ) );
                if ( levelscount > ExpArgs . ExpandLevels )
                {
                    continue;
                    //                    Debug. WriteLine ( $"LEVELS 1 - Breaking out where level {levelscount} <= {levels}" );
                    //                    break;
                }
                Selection . Text = $"Expanding {obj . Tag . ToString ( )}";
                TreeViewItem childControl = obj as TreeViewItem;
                // working correctly
                //UpdateListBox ( childControl . Tag . ToString ( ) );
                ShowProgress ( );
                ShowExpandTime ( );
                if ( CheckSearchSuccess ( childControl . Tag . ToString ( ) ) == true )
                {
                    UpdateListBox ( $"Search for {Searchtext . Text} found  as [" + childControl . Header . ToString ( ) + $"]\nin {childControl . Tag . ToString ( )}" );
                    ScrollCurrentTvItemIntoView ( childControl );
                    childControl . IsSelected = true;
                    ExpArgs . SearchSuccessItem = childControl;
                    ExpArgs . SearchSuccess = true;
                    Returnval = true;
                    IsComplete = true;
                    ActiveTree . Refresh ( );
                    break;
                }

                try
                {
                    childControl . IsExpanded = true;
                }
                catch ( Exception ex ) { Debug. WriteLine ( $"ExpandCurrentAllLevels: 2790 : {ex . Message}" ); }

                if ( childControl . Items . Count == 1 && childControl . Header . ToString ( ) == "Loading" )
                    continue;
                //                Debug. WriteLine ( $"2 Level={levelscount}, Outer loop {childControl . Tag . ToString ( )}" );
                ShowProgress ( );
                levelscount = CalculateLevel ( childControl . Tag . ToString ( ) );
                if ( levelscount >= ExpArgs . ExpandLevels )
                    continue;

                if ( childControl != null )
                {
                    string entry = childControl . Header . ToString ( ) . ToString ( ) . ToUpper ( );
                    itemcount = childControl . Items . Count;
                    ShowProgress ( );
                    if ( FullExpandinProgress == false )
                        ActiveTree . Refresh ( );
                    iterations++;
                    if ( ExpArgs . ExpandLevels >= 3 )
                    {
                        //******************
                        // INNER LOOP
                        //******************
                        UpdateListBox ( childControl . Tag . ToString ( ) );
                        if ( childControl . Items . Count > 0 )
                        {
                            TreeViewItem tmp = childControl . Items [ 0 ] as TreeViewItem;
                            if ( tmp . ToString ( ) == "Loading" && childControl . Items . Count > 1 )
                            {
                                MessageBox . Show ( $"ERROR, {childControl . Tag . ToString ( )} has  a 'Loading' dummy entry" );
                                AbortExpand = true;
                                break;
                            }
                        }
                        foreach ( TreeViewItem nextitem in childControl . Items )
                        {
                            ShowProgress ( );
                            if ( CheckIsVisible ( nextitem . Tag . ToString ( ) . ToUpper ( ) , ShowAllFiles , out HasHidden ) == false )
                            {
                                Debug. WriteLine ( $"System file : {nextitem . Tag . ToString ( ) . ToUpper ( )}" );
                                continue;
                            }

                            if ( CheckSearchSuccess ( nextitem . Tag . ToString ( ) ) == true )
                            {
                                UpdateListBox ( $"Search for {Searchtext . Text} found  as [" + nextitem . Header . ToString ( ) + $"]\nin {nextitem . Tag . ToString ( )}" );
                                ScrollCurrentTvItemIntoView ( nextitem );
                                nextitem . IsSelected = true;
                                //SearchSuccess = true;
                                ExpArgs . SearchSuccess = true;
                                ExpArgs . SearchSuccessItem = nextitem;
                                Returnval = true;
                                ActiveTree . Refresh ( );
                                break;
                            }
                            try
                            {
                                nextitem . IsExpanded = true;
                                if ( AbortExpand == true )
                                    break;
                                if ( ExpandLimited == true )
                                    continue;
                            }
                            catch ( Exception ex ) { Debug. WriteLine ( $"ExpandCurrentAllLevels: 2853 : {ex . Message}" ); }
                            ShowProgress ( );
                            // working correctly
                            if ( FullExpandinProgress == false )
                                ActiveTree . Refresh ( );
                            //                            Debug. WriteLine ( Selection . Text );
                            levelscount = CalculateLevel ( nextitem . Tag . ToString ( ) );
                            if ( levelscount >= ExpArgs . ExpandLevels )
                            {
                                continue;
                            }
                            UpdateListBox ( nextitem . Tag . ToString ( ) );
                            if ( ExpArgs . ExpandLevels >= 4 )
                            {
                                if ( CheckIsVisible ( nextitem . Tag . ToString ( ) . ToUpper ( ) , ShowAllFiles , out HasHidden ) == false )
                                {
                                    Debug. WriteLine ( $"System file : {nextitem . Tag . ToString ( ) . ToUpper ( )}" );
                                    continue;
                                }
                                if ( ExpandAll3 ( nextitem , true , ExpArgs . ExpandLevels ) == true )
                                {
                                    //SearchSuccess = true;
                                    ExpArgs . SearchSuccess = true;
                                    Returnval = true;
                                    break;
                                }
                            }
                            ShowExpandTime ( );
                            ShowProgress ( );
                            if ( FullExpandinProgress == false )
                                ActiveTree . Refresh ( );
                        }   // End INNER FOREACH

                        if ( IsComplete )
                            break;
                        if ( AbortExpand == true )
                            break;
                        ShowExpandTime ( );
                    }
                    ShowExpandTime ( );
                }
                if ( FullExpandinProgress == false )
                    ActiveTree . Refresh ( );
                ShowProgress ( );
                if ( IsComplete )
                    break;
            }   // End FOREACH

            ShowExpandTime ( );
            ExpandSetup ( false );
            Expandprogress . Refresh ( );
            if ( AbortExpand == true )
            {
                UpdateListBox ( $"Expansion has been CANCELLED, \n\nNot all expected folders have been expanded..." );
                MessageBox . Show ( $"Current Expansion of {startitem} has been CANCELLED. \n\nNot all expected folders have been expanded..." , "System Information" );
            }
            Selection . Text = $"{startitem . Tag . ToString ( )} Expanded {ExpArgs . ExpandLevels} levels Successfully...";
            if ( ExpArgs . SearchSuccess == false )
            {
                startitem . IsSelected = true;
                ScrollCurrentTvItemIntoView ( Args [ 0 ] as TreeViewItem );
                ActiveTree . Refresh ( );
            }
            // Reset global flag for cancellation
            AbortExpand = false;
            ExpandLimited = false;
            return Returnval;
        }
        public Task<bool> ExpandCurrentAllLevelsTask ( object [ ] Args )
        {
            bool Returnval = false;
            bool IsComplete = false;
            int iterations = 0;
            int itemcount = 0;
            int levelscount = 0;
            var fail = false;
            var success = true;
            //            int levels = ( int ) Args [ 1 ];
            TreeViewItem items = Args [ 0 ] as TreeViewItem;
            startitem = items;
            fail = false;
            if ( items == null )
                return Task . FromResult ( fail );
            //ProgressCount = 0;
            if ( items . Tag . ToString ( ) . ToUpper ( ) . Contains ( items . Header . ToString ( ) . ToUpper ( ) ) == false )
                items . Header = items . Tag . ToString ( );
            // Essential to force root to be expanded, else nothing happens
            if ( CheckSearchSuccess ( items . Tag . ToString ( ) ) == true )
            {
                UpdateListBox ( $"Search for {Searchtext . Text} found  as [" + items . Header . ToString ( ) + $"]\nin {items . Tag . ToString ( )}" );
                ScrollCurrentTvItemIntoView ( items );
                items . IsSelected = true;
                ExpArgs . SearchSuccessItem = items;
                //SearchSuccess = true;
                ExpArgs . SearchSuccess = true;
                ActiveTree . Refresh ( );
                return Task . FromResult ( success );
            }

            try
            {
                items . IsExpanded = true;
            }
            catch ( Exception ex ) { Debug. WriteLine ( $"ExpandCurrentAllLevels: 2956 : {ex . Message}" ); }

            ShowProgress ( );
            if ( FullExpandinProgress == false )
                ActiveTree . Refresh ( );
            levelscount = CalculateLevel ( items . Tag . ToString ( ) );
            if ( levelscount >= ExpArgs . ExpandLevels )
            {
                ShowExpandTime ( );
                ExpandSetup ( false );
                Expandprogress . Refresh ( );
                return Task . FromResult ( fail );
            }
            //**************
            // Main LOOP
            //**************
            foreach ( TreeViewItem obj in items . Items )
            {
                ShowProgress ( );
                if ( levelscount > ExpArgs . ExpandLevels )
                {
                    continue;
                }
                Selection . Text = $"Expanding {obj . Tag . ToString ( )}";
                TreeViewItem childControl = obj as TreeViewItem;
                // working correctly
                ShowProgress ( );
                if ( CheckSearchSuccess ( childControl . Tag . ToString ( ) ) == true )
                {
                    UpdateListBox ( $"Search for {Searchtext . Text} found  as [" + childControl . Header . ToString ( ) + $"]\nin {childControl . Tag . ToString ( )}" );
                    ScrollCurrentTvItemIntoView ( childControl );
                    childControl . IsSelected = true;
                    ExpArgs . SearchSuccessItem = childControl;
                    //  SearchSuccess = true;
                    ExpArgs . SearchSuccess = true;
                    Returnval = true;
                    IsComplete = true;
                    break;
                }

                try
                {
                    childControl . IsExpanded = true;
                }
                catch ( Exception ex ) { Debug. WriteLine ( $"ExpandCurrentAllLevels: 3000 : {ex . Message}" ); }
                //                Debug. WriteLine ( $"2 Level={levelscount}, Outer loop {childControl . Tag . ToString ( )}" );
                ShowProgress ( );
                levelscount = CalculateLevel ( childControl . Tag . ToString ( ) );
                if ( levelscount >= ExpArgs . ExpandLevels )
                    continue;

                if ( childControl != null )//&& levels >= 2 )
                {
                    string entry = childControl . Header . ToString ( ) . ToString ( ) . ToUpper ( );
                    itemcount = childControl . Items . Count;

                    ShowProgress ( );
                    if ( FullExpandinProgress == false )
                        ActiveTree . Refresh ( );
                    iterations++;
                    if ( ExpArgs . ExpandLevels >= 3 )
                    {
                        //******************
                        // INNER LOOP
                        //******************
                        foreach ( TreeViewItem nextitem in childControl . Items )
                        {
                            ShowProgress ( );
                            if ( CheckIsVisible ( nextitem . Tag . ToString ( ) . ToUpper ( ) , ShowAllFiles , out HasHidden ) == false )
                            {
                                Debug. WriteLine ( $"System file : {nextitem . Tag . ToString ( ) . ToUpper ( )}" );
                                continue;
                            }

                            if ( CheckSearchSuccess ( nextitem . Tag . ToString ( ) ) == true )
                            {
                                UpdateListBox ( $"Search for {Searchtext . Text} found  as [" + nextitem . Header . ToString ( ) + $"]\nin {nextitem . Tag . ToString ( )}" );
                                ScrollCurrentTvItemIntoView ( nextitem );
                                nextitem . IsSelected = true;
                                ExpArgs . SearchSuccessItem = nextitem;
                                //    SearchSuccess = true;
                                ExpArgs . SearchSuccess = true;
                                Returnval = true;
                                break;
                            }

                            try
                            {
                                nextitem . IsExpanded = true;
                            }
                            catch ( Exception ex ) { Debug. WriteLine ( $"ExpandCurrentAllLevels: 3046 : {ex . Message}" ); }
                            ShowProgress ( );
                            if ( FullExpandinProgress == false )
                                ActiveTree . Refresh ( );

                            //                            Debug. WriteLine ( Selection . Text );
                            levelscount = CalculateLevel ( nextitem . Tag . ToString ( ) );
                            if ( levelscount >= ExpArgs . ExpandLevels )
                            {
                                continue;
                            }
                            UpdateListBox ( nextitem . Tag . ToString ( ) );
                            if ( ExpArgs . ExpandLevels >= 4 )
                            {
                                if ( CheckIsVisible ( nextitem . Tag . ToString ( ) . ToUpper ( ) , ShowAllFiles , out HasHidden ) == false )
                                {
                                    Debug. WriteLine ( $"System file : {nextitem . Tag . ToString ( ) . ToUpper ( )}" );
                                    continue;
                                }
                                if ( ExpandAll3 ( nextitem , true , ExpArgs . ExpandLevels ) == true )
                                {
                                    //SearchSuccess = true;
                                    ExpArgs . SearchSuccess = true;
                                    Returnval = true;
                                    IsComplete = true;
                                    break;
                                }
                            }
                            ShowExpandTime ( );
                            ShowProgress ( );
                            if ( FullExpandinProgress == false )
                                ActiveTree . Refresh ( );
                        }   // End INNER FOREACH

                        if ( IsComplete )
                            break;
                    }
                    ShowExpandTime ( );
                }
                if ( FullExpandinProgress == false )
                    ActiveTree . Refresh ( );
                ShowProgress ( );
                if ( IsComplete )
                    break;
            }   // End FOREACH

            ShowExpandTime ( );
            ExpandSetup ( false );
            Expandprogress . Refresh ( );
            Selection . Text = $"{startitem . Tag . ToString ( )} Expanded {ExpArgs . ExpandLevels} levels Successfully...";
            if ( ExpArgs . SearchSuccess == false )
            {
                startitem . IsSelected = true;
                ScrollCurrentTvItemIntoView ( Args [ 0 ] as TreeViewItem );
            }
            if ( Returnval )
                return Task . FromResult ( success );
            else
                return Task . FromResult ( fail );
        }
        private void TestTree_Collapsed ( object sender , RoutedEventArgs e )
        {
            Mouse . OverrideCursor = Cursors . Wait;
            if ( ActiveTree == TestTree )
            {
                TreeViewItem item = e . Source as TreeViewItem;
                item . Items . Clear ( );
                item . Items . Add ( "Loading" );
                //            ActiveTree . HorizontalContentAlignment = HorizontalAlignment . Left;
                item . IsSelected = true;
            }
            Mouse . OverrideCursor = Cursors . Arrow;
        }

        private void Stopthread ( object sender , RoutedEventArgs e )
        {
            //           Dispatcher . BeginInvokeShutdown ( DispatcherPriority . Normal );

        }

        #region BackgroundWorker
        private void worker_DoWork ( object sender , DoWorkEventArgs e )
        {
            int result = 0;
            BackgroundWorker worker = sender as BackgroundWorker;
            object [ ] Args = e . Argument as object [ ];
            Debug. WriteLine ( $"Calling BackGroundWorker Progress thread" );
            worker . ReportProgress ( 0 , ExpArgs );
            e . Result = result;
        }
        private void worker_RunWorkerCompleted ( object sender , RunWorkerCompletedEventArgs e )
        {
            if ( e . Cancelled )
            {
            }
        }
        private TreeViewItem GetParentNode ( TreeViewItem currentItem )
        {
            try
            {
                TreeViewItem tv2 = new TreeViewItem ( );
                tv2 = currentItem;
                if ( tv2 == null )
                    return null;
                string test = tv2 . Tag . ToString ( );
                if ( test . Length == 3 )
                    Debug. WriteLine ( $"Starting at parent node of {test}.." );
                else
                {
                    tv2 = currentItem . Parent as TreeViewItem;
                    test = tv2 . Tag . ToString ( );
                    if ( test . Length > 3 )
                    {
                        while ( test . Length > 3 )
                        {
                            TreeViewItem tv3 = new TreeViewItem ( );
                            tv3 = tv2 . Parent as TreeViewItem;
                            test = tv3 . Tag . ToString ( );
                            if ( test . Length > 3 )
                                tv2 = tv3;
                            else
                            {
                                ExpArgs . Parent = tv3;
                                tv2 = tv3;
                                break;
                            }
                        }
                    }
                }
                Debug. WriteLine ( $"Parent found is {test}" );
                return tv2;
            }
            catch ( Exception ex ) { Debug. WriteLine ( $"GetParentNode :3174 : {ex . Message}" ); }
            return null;
        }

        async private void worker_ProgressChanged ( object sender , ProgressChangedEventArgs e )
        {
            TreeViewItem tvnew = GetParentNode ( ExpArgs . tvitem );

            StartTimer ( );

            //// Run expansion as a task.....
            await RunRecurse ( ActiveTree , e );

            //// All done;
            Mouse . OverrideCursor = Cursors . Arrow;
            if ( ExpArgs . SearchActive && ExpArgs . SearchSuccess )
                //MessageBox . Show ( $"{ExpArgs . SearchTerm} has been identified" , "TreeView Search facilitiy" );
                fdl . ShowInfo ( Flowdoc , canvas , $"The 1st instance of the Search term shown below has been identified successfully, and is highlighted for you ..." , "Red5" , $"[{ExpArgs . SearchTerm}] " , "Black0" , "Match found !" , "Cyan5" , "TreeView Search Sytem" );
            else if ( ExpArgs . SearchActive )
            {
                if ( Exactmatch == false )
                    fdl . ShowInfo ( Flowdoc , canvas , $"Sorry, but the Search term of [{ExpArgs . SearchTerm}] could not be identified for you in the {ExpArgs . ExpandLevels - 1} Expansion levels below the initial point of [{ExpArgs . tvitem . Tag . ToString ( )}]..." , "Black0" , $"[{ExpArgs . SearchTerm}] " , "Red5" , "NO Match found !" , "Cyan5" , "TreeView Search Sytem" );
                else
                    fdl . ShowInfo ( Flowdoc , canvas , $"Sorry, but an EXACT match to the Search term of [{ExpArgs . SearchTerm}] could not be identified for you in the {ExpArgs . ExpandLevels - 1} Expansion levels below the initial point of [{ExpArgs . tvitem . Tag . ToString ( )}]..." , "Black0" , $"[{ExpArgs . SearchTerm}] " , "Red5" , "NO Match found !" , "Cyan5" , "TreeView Search Sytem" );
            }
        }
        public Task<bool> RunRecurse ( TreeView ActiveTree , ProgressChangedEventArgs e )
        {
            // e contains current treeviewitem !!!
            // All parameters are in ExpandArgs ExpArgs
#pragma warning disable CS0219 // The variable 'Returnval' is assigned but its value is never used
            bool Returnval = false;
#pragma warning restore CS0219 // The variable 'Returnval' is assigned but its value is never used
            bool IsComplete = false;
            int iterations = 0;

            int itemcount = 0;
            int levelscount = 0;
            var fail = false;
            var success = true;

            if ( ExpArgs . SearchTerm != null )
                // Force search to be upper case
                ExpArgs . SearchTerm = ExpArgs . SearchTerm . ToUpper ( );
            else
                ExpArgs . SearchTerm = "";
            //ExpandArgs eargs = e . UserState as ExpandArgs;
            //int levels = ( int ) Args [ 1 ];
            int levels = ExpArgs . ExpandLevels;

            TreeViewItem startup = new TreeViewItem ( );
            TreeViewItem items = ExpArgs . tvitem;
            startitem = items;
            fail = false;
            if ( items == null )
                return Task . FromResult ( success );
            //ProgressCount = 0;
            if ( items . Tag . ToString ( ) . ToUpper ( ) . Contains ( items . Header . ToString ( ) . ToUpper ( ) ) == false )
                items . Header = items . Tag . ToString ( );
            // Essential to force root to be expanded, else nothing happens
            if ( CheckSearchSuccess ( items . Tag . ToString ( ) ) == true )
            {
                UpdateListBox ( $"Search for {Searchtext . Text} found  as [" + items . Header . ToString ( ) + $"]\nin {items . Tag . ToString ( )}" );
                ScrollCurrentTvItemIntoView ( items );
                ExpArgs . SearchSuccessItem = items;
                items . IsSelected = true;
                //    SearchSuccess = true;
                ExpArgs . SearchSuccess = true;
                return Task . FromResult ( success );
            }

            try
            {
                items . IsSelected = true;
                items . IsExpanded = true;
                if ( AbortExpand )
                    return Task . FromResult ( fail );
                //stack . Push ( items . Tag . ToString ( ) );
            }
            catch ( Exception ex ) { Debug. WriteLine ( $"RunRecurse: 3304 : {ex . Message}" ); }
            Thread . Sleep ( SLEEPTIME );
            ShowProgress ( );
            if ( FullExpandinProgress == false )
                items . Refresh ( );
            levelscount = CalculateLevel ( items . Tag . ToString ( ) );
            if ( levelscount >= ExpArgs . ExpandLevels )
            {
                items . IsSelected = true;
                items . IsExpanded = true;
                Debug. WriteLine ( $"{items . Header . ToString ( )} Expanded.... ?" );
                ActiveTree . Refresh ( );
                ShowExpandTime ( );
                ExpandSetup ( false );
                Expandprogress . Refresh ( );
                return Task . FromResult ( success );
            }
            //**************
            // Main LOOP
            //**************
            UpdateListBox ( items . Tag . ToString ( ) );

            foreach ( var objct in items . Items )
            {
                if ( objct . ToString ( ) == "Loading" )
                    break;
                startup = objct as TreeViewItem;
                //Thread . Sleep ( SLEEPTIME );
                TreeViewItem obj = objct as TreeViewItem;
                ShowProgress ( );
                //levelscount = CalculateLevel ( obj . Tag . ToString ( ) );
                if ( levelscount > ExpArgs . ExpandLevels )
                {
                    continue;
                }
                Selection . Text = $"Expanding {obj . Tag . ToString ( )}";
                TreeViewItem childControl = obj as TreeViewItem;
                // working correctly
                ShowProgress ( );
                if ( CheckSearchSuccess ( childControl . Tag . ToString ( ) ) == true )
                {
                    UpdateListBox ( $"Search for {Searchtext . Text} found  as [" + childControl . Header . ToString ( ) + $"]\nin {childControl . Tag . ToString ( )}" );
                    ScrollCurrentTvItemIntoView ( childControl );
                    childControl . IsSelected = true;
                    ExpArgs . SearchSuccessItem = childControl;
                    ExpArgs . SearchSuccess = true;
                    Returnval = true;
                    IsComplete = true;
                    ActiveTree . Refresh ( );
                    break;
                }

                try
                {
                    childControl . IsExpanded = true;
                    if ( AbortExpand )
                        return Task . FromResult ( fail );
                }
                catch ( Exception ex ) { Debug. WriteLine ( $"RunRecurse: 3361 : {ex . Message}" ); }
                ShowProgress ( );

                //                if ( ClosePreviousNode )
                //                  Debug. WriteLine ( $"Closeprevious 1 : {childControl.Tag.ToString()}" );

                levelscount = CalculateLevel ( childControl . Tag . ToString ( ) );
                if ( levelscount >= ExpArgs . ExpandLevels )
                {
                    if ( ExpArgs . SearchTerm != "" && ClosePreviousNode )
                    {
                        childControl . IsExpanded = false;
                        ActiveTree . Refresh ( );
                    }
                    continue;
                }
                if ( childControl != null )//&& ExpArgs . ExpandLevels >= 2 )
                {
                    string entry = childControl . Header . ToString ( ) . ToString ( ) . ToUpper ( );
                    itemcount = childControl . Items . Count;
                    ShowProgress ( );
                    if ( FullExpandinProgress == false )
                        ActiveTree . Refresh ( );
                    iterations++;
                    if ( ExpArgs . ExpandLevels >= 3 )
                    {
                        bool nofault = false;
                        //******************
                        // INNER LOOP
                        //******************
                        //UpdateListBox ( childControl. Tag . ToString ( ) );

                        UpdateListBox ( childControl . Tag . ToString ( ) );
                        foreach ( var newentry in childControl . Items )
                        {
                            TreeViewItem nextitem = new TreeViewItem ( );
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
                            try
                            {
                                nextitem = newentry as TreeViewItem;
                            }
                            catch ( Exception ex )
                            {
                                Debug. WriteLine ( $"Unable to access {childControl . Tag . ToString ( )}" );
                                nofault = true;
                            }
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
                            if ( nofault || nextitem == null )
                            {
                                nofault = false;
                                continue;
                            }
                            ShowProgress ( );
                            if ( nextitem . Header . ToString ( ) == "Loading" )
                                continue;
                            if ( CheckIsVisible ( nextitem . Tag . ToString ( ) . ToUpper ( ) , ShowAllFiles , out HasHidden ) == false )
                            {
                                Debug. WriteLine ( $"System file : {nextitem . Tag . ToString ( ) . ToUpper ( )}" );
                                continue;
                            }
                            UpdateListBox ( nextitem . Tag . ToString ( ) );

                            if ( CheckSearchSuccess ( nextitem . Tag . ToString ( ) ) == true )
                            {
                                UpdateListBox ( $"Search for {Searchtext . Text} found  as [" + nextitem . Header . ToString ( ) + $"]\nin {nextitem . Tag . ToString ( )}" );
                                ScrollCurrentTvItemIntoView ( nextitem );
                                nextitem . IsSelected = true;
                                //SearchSuccess = true;
                                ExpArgs . SearchSuccess = true;
                                ExpArgs . SearchSuccessItem = nextitem;
                                Returnval = true;
                                IsComplete = true;
                                break;
                            }

                            try
                            {
                                iterations++;
                                nextitem . IsExpanded = true;
                                if ( nextitem . Items . Count == 0 || nextitem . Items [ 0 ] . ToString ( ) == "Loading" )
                                    continue;
                                if ( AbortExpand )
                                    return Task . FromResult ( fail );
                            }
                            catch ( Exception ex ) { Debug. WriteLine ( $"RunRecurse: 3424 : {ex . Message}" ); }
                            ShowProgress ( );
                            // working correctly
                            if ( FullExpandinProgress == false )
                                ActiveTree . Refresh ( );
                            //                            Debug. WriteLine ( Selection . Text );
                            levelscount = CalculateLevel ( nextitem . Tag . ToString ( ) );
                            if ( levelscount >= ExpArgs . ExpandLevels )
                            {
                                continue;
                            }
                            //if ( nextitem . Tag . ToString ( ) . Contains ( "-500" ) )
                            //    Returnval = Returnval;
                            if ( ExpArgs . ExpandLevels >= 4 )
                            {
                                //                                UpdateListBox ( nextitem . Tag . ToString ( ) );
                                if ( CheckIsVisible ( nextitem . Tag . ToString ( ) . ToUpper ( ) , ShowAllFiles , out HasHidden ) == false )
                                {
                                    Debug. WriteLine ( $"System file : {nextitem . Tag . ToString ( ) . ToUpper ( )}" );
                                    continue;
                                }
                                //Thread . Sleep ( SLEEPTIME );
                                if ( nextitem . HasItems )
                                {
                                    if ( ExpandAll3 ( nextitem , true , ExpArgs . ExpandLevels ) == true )
                                    {
                                        ExpArgs . SearchSuccess = true;
                                        Returnval = true;
                                        IsComplete = true;
                                        break;
                                    }
                                    else
                                    {
                                        if ( ClosePreviousNode && ExpArgs . SearchActive == true && ExpArgs . SearchSuccess == false )
                                        {
                                            // ONLY If Searching , Close the subdir we have just finished prcessing
                                            nextitem . IsExpanded = false;
                                            ActiveTree . Refresh ( );
                                        }
                                    }
                                }
                            }
                            else
                                UpdateListBox ( nextitem . Tag . ToString ( ) );

                            ShowExpandTime ( );
                            ShowProgress ( );
                            if ( FullExpandinProgress == false )
                                ActiveTree . Refresh ( );
                        }   // End INNER FOREACH

                        if ( ClosePreviousNode && ExpArgs . SearchActive == true && ExpArgs . SearchSuccess == false )
                        {
                            // ONLY If Searching , Close the subdir we have just finished prcessing
                            childControl . IsExpanded = false;
                            ActiveTree . Refresh ( );
                        }
                        if ( IsComplete )
                            break;
                    }
                    ShowExpandTime ( );
                }
                if ( FullExpandinProgress == false )
                    ActiveTree . Refresh ( );
                ShowProgress ( );
                if ( IsComplete )
                    break;
            }   // End FOREACH

            ShowExpandTime ( );
            ExpandSetup ( false );
            Expandprogress . Refresh ( );
            Selection . Text = $"{startitem . Tag . ToString ( )} Expanded {ExpArgs . ExpandLevels} levels Successfully...";
            if ( ExpArgs . SearchSuccess == false )
            {
                ScrollCurrentTvItemIntoView ( startup );
                startitem . IsSelected = true;
            }
            UpdateDriveHeader ( ShowVolumeLabels );
            return Task . FromResult ( success );
        }
        #endregion BackgroundWorker
        public int AddFilesToRecurse ( List<string> Allfiles , TreeViewItem item )
        {
            int count = 0;
            //            TreeViewItem tmp = item . Items [ 0 ] as TreeViewItem;
            var subitemctrl = new TreeViewItem ( );
            if ( item . Items [ 0 ] . ToString ( ) == "Loading" )
                item . Items . Clear ( );
            foreach ( var itm in Allfiles )
            {
                ShowProgress ( );
                var subitem = new TreeViewItem ( )
                {
                    Header = GetFileFolderName ( itm ) ,
                    Tag = itm
                };
                if ( CheckIsVisible ( itm . ToUpper ( ) , ShowAllFiles , out HasHidden ) == true )
                {
                    item . Items . Add ( subitem );
                    item . IsExpanded = true;
                    subitem . IsSelected = true;
                    ScrollCurrentTvItemIntoView ( subitem );
                    ActiveTree . Refresh ( );
                    subitemctrl = subitem;                    //item . IsExpanded = true;
                                                              //                    Debug. WriteLine ( $"3 - ADTR : Added {subitem . Tag . ToString ( )} to {item . Tag . ToString ( )} &  scrolled" );
                    count++;
                    if ( CheckSearchSuccess ( subitem . Tag . ToString ( ) ) == true )
                    {
                        UpdateListBox ( $"\nSearch for {Searchtext . Text} found  as [" + subitem . Header . ToString ( ) + $"]\nin {subitem . Tag . ToString ( )}" );
                        if ( subitem . IsSelected == false )
                            subitem . IsSelected = true;
                        ScrollCurrentTvItemIntoView ( subitem );
                        ActiveTree . Refresh ( );
                        ExpArgs . SearchSuccessItem = subitem;
                        //SearchSuccess = true;
                        ExpArgs . SearchSuccess = true;
                        Mouse . OverrideCursor = Cursors . Arrow;
                        break;
                    }

                }
                if ( Allfiles . Count > 0 )
                {
                    item . IsExpanded = true;
                    subitemctrl . IsSelected = true;
                    ScrollCurrentTvItemIntoView ( subitemctrl );
                    if ( FullExpandinProgress == false )
                        ActiveTree . Refresh ( );
                }
                ShowProgress ( );
            }
            return count;
        }
        private void SearchTree ( object sender , RoutedEventArgs e )
        {
            // This is the Search button  handler
            //SearchSuccess = false;
            ExpArgs . SearchSuccess = false;
            TreeViewItem tvi = new TreeViewItem ( );
            tvi = ActiveTree . SelectedItem as TreeViewItem;
            TreeViewItem rootitem = new TreeViewItem ( );
            TreeViewItem tvfound = new TreeViewItem ( );
            ExpandSetup ( true );
            TextToSearchFor = Searchtext . Text . ToUpper ( );
            if ( TextToSearchFor . ToUpper ( ) == "" || TextToSearchFor . ToUpper ( ) == "SEARCH FOR...." )
            {
                fdl . ShowInfo ( Flowdoc , canvas , "You have not entered a value to be searched for ? " );
                return;
            }
            SearchString = TextToSearchFor;
            // Set it as a default
            rootitem = tvi;
            if ( tvi != null && tvi . HasItems )
            {
                // call main recursive handler
                ExpandSelection = 4;
                maxlevels = 90;

                ClearExpandArgs ( );
                ExpArgs . tvitem = ActiveTree . SelectedItem as TreeViewItem;
                ExpArgs . Selection = 0;
                ExpArgs . ExpandLevels = LevelsCombo . SelectedIndex + 3;
                ExpArgs . SearchTerm = Searchtext . Text . ToUpper ( );
                ExpArgs . ListResults = false;
                ExpArgs . SearchActive = true;
                ExpanderMenuOption . Text = $"Search for Item down up to {ExpArgs . ExpandLevels - 1} levels";

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
                RunExpandSystem ( null , null );
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
                //RecurseItem ( ExpArgs . tvitem , ExpArgs . SearchTerm , ExpArgs . IsFullExpand );
            }
            Mouse . OverrideCursor = Cursors . Arrow;
            return;
        }
        private bool RecurseItem ( TreeViewItem tvitem , string SearchTerm , bool ClosePrevious = true )
        {

            /*
              * ExpandArgs
                       public TreeView tv;
                         public               TreeViewItem tvitem;
                         public  int         ExpandLevels;
                         public string     SearchTerm;
                         public bool        ClosePrevious; 
               */
            // Expand our structure to parameters
            //TreeView tv = ExpArgs . tv;
            SearchTerm = ExpArgs . SearchTerm;
            RunRecurseItem ( ExpArgs . tvitem , SearchTerm , ClosePrevious );
            SearchTerm = "";
            ExpArgs . SearchSuccess = false;
            return true;
        }
        // NOT USED
        // refs are internal (recursive) calls only
        private TreeViewItem RunRecurseItem ( TreeViewItem tvitem , string SearchTerm , bool ClosePrevious = true )
        {
            // MAIN SEARCH HANDLER
            // This recurses through all files and folders
            //and Scrolls to and highlights the item Found, if any!

            return null;


#pragma warning disable CS0162 // Unreachable code detected
            if ( ExpArgs . SearchTerm == "SEARCH FOR...." )
            {
                MessageBox . Show ( "No search term entered, so Search has been aborted" , "User Error" );
                return null;
            }
#pragma warning restore CS0162 // Unreachable code detected
            // Allow it  to unwind gracefully
            if ( ExpArgs . SearchSuccess )
                return ExpArgs . SearchSuccessItem;
            List<String> directories = new List<string> ( );
            List<String> AllFiles = new List<string> ( );
            TreeViewItem currentitem = new TreeViewItem ( );
            currentitem = tvitem;
            // Add all content to current folder
            // Get root level Subdirs next, it may provde a search match
            int count = GetDirectories ( currentitem . Tag . ToString ( ) , out directories );
            if ( count > 0 )
            {
                AddDirectoriesToTestTree ( directories , tvitem );
                if ( ExpArgs . SearchSuccess )
                    return ExpArgs . SearchSuccessItem;
                if ( ClosePreviousNode )
                {
                    //                    if ( ClosePreviousNode )
                    //                      Debug. WriteLine ( $"Closeprevious 7 CLOSING : {tvitem. Tag . ToString ( )}" );
                    tvitem . IsExpanded = false;
                }
            }
            // Get root level files 1st, it may provde a search match
            GetFiles ( currentitem . Tag . ToString ( ) , out AllFiles );
            if ( AllFiles . Count > 0 )
            {
                AddFilesToRecurse ( AllFiles , currentitem );
                if ( ExpArgs . SearchSuccess )
                {
                    currentitem . IsSelected = true;
                    return ExpArgs . SearchSuccessItem;
                }
            }
            //Finally, iterate thru subdirs 
            currentitem . IsExpanded = true;
            foreach ( var subItem in currentitem . Items )
            {
                TreeViewItem tvo = new TreeViewItem ( );
                tvo = subItem as TreeViewItem;
                if ( tvo . HasItems == true && CheckSearchSuccess ( tvo . Tag . ToString ( ) ) == true )
                {
                    UpdateListBox ( $"\"nSearch for {Searchtext . Text} found  as [" + tvo . Header . ToString ( ) + $"]\nin {tvo . Tag . ToString ( )}" );
                    tvo . IsSelected = true;
                    //    SearchSuccess = true;
                    ExpArgs . SearchSuccess = true;
                    ExpArgs . SearchSuccessItem = tvo;
                    ScrollCurrentTvItemIntoView ( tvo );
                    ActiveTree . Refresh ( );
                    Mouse . OverrideCursor = Cursors . Arrow;
                    break;
                }
                else
                {
                    if ( ClosePrevious )
                        tvo . IsExpanded = false;
                    if ( tvo . HasItems == false )
                        continue;
                    //                  Debug. WriteLine ( $"RI Expanded :{tvo . Tag . ToString ( )} - calling Ri for item {tvo . Header . ToString ( )}...." );

                    RunRecurseItem ( tvo , SearchTerm , ClosePrevious );
                    if ( ExpArgs . SearchSuccess == true )
                        return ExpArgs . SearchSuccessItem;
                    //                    AllCheckedFolders . Add ( tvo );
                }
            }
            if ( ExpArgs . SearchSuccess )
                return ExpArgs . SearchSuccessItem;
            // Close the subdirectory we have completed processing on - for neatness - Works well 25/4/22
            if ( ClosePrevious )
                currentitem . IsExpanded = false;
            return null;
        }
        private void Expand_Click ( object sender , RoutedEventArgs e )
        {
            // Called by Expand ALL drives one level
            TreeViewItem tvi;
            ClearExpandArgs ( );
            ExpArgs . Selection = DirectoryOptions . SelectedIndex;
            if ( ExpArgs . Selection == 3 )
                ExpArgs . Selection = 90;
            ExpArgs . ExpandLevels = Convert . ToInt16 ( LevelsCombo . SelectedItem );
            if ( ExpArgs . Selection == 0 )
                ExpArgs . ExpandLevels = 2;
            if ( ExpArgs . Selection == 2 )
                ExpArgs . ExpandLevels = 3;
            ExpArgs . tvitem = ActiveTree . SelectedItem as TreeViewItem;
            tvi = ExpArgs . tvitem;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
            RunExpandSystem ( sender , e );
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
            Utils . DoErrorBeep ( 300 , 60 , 1 );
            Utils . DoErrorBeep ( 250 , 70 , 1 );
            if ( tvi != null )
            {
                // Return to original selected item
                tvi . IsSelected = true;
                ScrollTvItemIntoView ( tvi );
                BringIntoView ( );
                //            ExpandSetup ( false );
            }
        }

        /// <summary>
        /// Process Expannd request
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        async private Task<bool> RunExpandSystem ( object sender , RoutedEventArgs e )
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            //called by Context menu and dropdown list options to handle individual and multiple drive expansions
            // This iterates internally to fully expand xx levels as per ExpArgs level setting
            // and visually track the items as they are expanded down the tree in the TreeView itself
            // and finally selects the original calling item that has been opened

#pragma warning disable CS0219 // The variable 'temp' is assigned but its value is never used
            double temp = 0;
#pragma warning restore CS0219 // The variable 'temp' is assigned but its value is never used
            ComboBox cb = DirectoryOptions;
            int selindex = ExpArgs . Selection;
            string original = "";
            //            int iterations = 0;
            listboxtotal = 0;
            TreeViewItem root = ActiveTree . SelectedItem as TreeViewItem;
            TreeViewItem caller = ActiveTree . SelectedItem as TreeViewItem;
            if ( caller == null )
                caller = root;
            if ( root != null )
            {
                if ( root . HasItems == false && ExpArgs . IsFullExpand == false )
                {
                    Mouse . OverrideCursor = Cursors . Arrow;
                    MessageBox . Show ( "The current item is only a file (or contains only Hidden items)\nand cannot threfore be expanded.\n\nPlease select a Valid Drive or Folder in the TreeView before using these options...." ,
                        "Invalid Current Selection" );
                    return false;
                }
                Args [ 0 ] = root as object;
                original = root . Header . ToString ( );
            }
            else
            {
                if ( Args [ 0 ] != null )
                    root = Args [ 0 ] as TreeViewItem;
            }

            // Prepare Arguments
            ExpArgs . tv = ActiveTree;
            ExpArgs . tvitem = root;
            ExpArgs . Selection = selindex;

            InfoList . ItemsSource = null;

            Debug. WriteLine ( $"User Selection in use = {ExpArgs . Selection }" );
            if ( ExpArgs . tvitem == null )
            {
                Mouse . OverrideCursor = Cursors . Arrow;
                fdl . ShowInfo ( Flowdoc , canvas ,
                      $"Please select a drive or subfolder before using  this option...." ,
                      "Blue1" ,
                      "TreeView Search Sytem" );
                BusyLabel . Visibility = Visibility . Hidden;
                return false;
            }

            #region  FALL THRU SELECTIONS

            if ( selindex == 0 )   // Expand  2  levels
            {
                if ( ExpArgs . ExpandLevels == 0 )
                    ExpArgs . ExpandLevels = 2;
                UpdateListBox ( $"Expanding {original} {ExpArgs . ExpandLevels} levels...." );
                //TestTree_Expanded ( sender , null );
                //Fall thru to main handler .......
            }
            else if ( selindex == 1 )    // Expand 2 levels ???
            {
                if ( ExpArgs . ExpandLevels == 0 )
                    ExpArgs . ExpandLevels = 3;
                UpdateListBox ( $"Expanding {original} {ExpArgs . ExpandLevels } levels...." );
                //Fall thru to main handler .......
            }
            else if ( selindex == 2 )    // Expand 3 levels ????
            {
                ExpArgs . ExpandLevels = 4;
                TreeViewItem tvi = ( TreeViewItem ) ExpArgs . tvitem;
                string str = tvi . Tag . ToString ( );
                //if ( ExpArgs . SearchActive == false )
                //{
                //    if ( MessageBox . Show ( $"Expanding {str} down {ExpArgs . ExpandLevels } levels may take some time, Are you  sure you want  to continue ?" , "Expansion System Warning !" ,
                //    MessageBoxButton . YesNo ) == MessageBoxResult . No )
                //    {
                //        Mouse . OverrideCursor = Cursors . Arrow;
                //        BusyLabel . Visibility = Visibility . Hidden;
                //        return false;
                //    }
                //}
                UpdateListBox ( $"Expanding {original} {Args [ 1 ]} levels...." );
                ExpArgs . ListResults = LISTRESULTS;
                //               Dispatcher . BeginInvoke ( DispatcherPriority . Normal , ( Action ) ( async ( ) => TestTree_Expanded ( tvi , e ) ) );
                Mouse . OverrideCursor = Cursors . Arrow;
                //               return true;
                //Fall thru to main handler .......
            }
            else if ( selindex == 3 )
            {
                if ( ExpArgs . ExpandLevels == 0 )
                    ExpArgs . ExpandLevels = 90;
                UpdateListBox ( $"Expanding {original} {Args [ 1 ]} levels...." );
                ExpArgs . ListResults = LISTRESULTS;
                //Fall thru to main handler .......
            }
            #endregion  FALL THRU SELECTIONS

            #region  SPECIFIC SELECTIONS (calls the various TriggerExpand(x) options)

            else if ( selindex == 4 )   //TriggerExpand0
            {
                // Expand All Drives 1 level 
                ExpArgs . tv = ActiveTree;
                ExpArgs . Selection = 0;
                ExpArgs . ExpandLevels = 1;
                ExpandSetup ( true );
                foreach ( TreeViewItem item in ActiveTree . Items )
                {
                    TreeViewItem tvi = new TreeViewItem ( );
                    tvi = item;
                    tvi . IsSelected = true;
                    ExpArgs . tvitem = tvi;
                    ExpArgs . IsFullExpand = true;
                    TriggerExpand0 ( sender , e );
                    Thread . Sleep ( 10 );
                }
                ExpandSetup ( false );
                ScrollCurrentTvItemIntoView ( ( TreeViewItem ) ActiveTree . Items [ 0 ] );
                return true;
            }
            else if ( selindex == 5 )   //TriggerExpand 0 multiple times
            {
                // Expand All Drives 2 levels 
                ExpArgs . tv = ActiveTree;
                ExpArgs . Selection = 0;
                ExpArgs . ExpandLevels = 2;
                ExpandSetup ( true );
                foreach ( TreeViewItem item in ActiveTree . Items )
                {
                    TreeViewItem tvi = new TreeViewItem ( );
                    tvi = item;
                    tvi . IsSelected = true;
                    ExpArgs . tvitem = tvi;
                    ExpArgs . IsFullExpand = true;
                    ExpArgs . ExpandLevels = 2;
                    TriggerExpand0 ( sender , e );
                    Thread . Sleep ( 10 );
                }
                ExpandSetup ( false );
                ScrollCurrentTvItemIntoView ( ( TreeViewItem ) ActiveTree . Items [ 0 ] );
                return true;
            }
            else if ( selindex == 6 )   //ExpandCurrentLevels()
            {
                bool IterateGo = false;
                // Expand ALL below  current drive
                ExpArgs . tvitem = ActiveTree . SelectedItem as TreeViewItem;
                if ( ExpArgs . ExpandLevels == 0 )
                    ExpArgs . ExpandLevels = 90;
                // inhibit listbox
                ExpArgs . ListResults = LISTRESULTS;
                UpdateListBox ( $"Expanding ALL items BELOW current...." );
                foreach ( TreeViewItem item in ActiveTree . Items )
                {
                    if ( IterateGo || item . Header . ToString ( ) == ExpArgs . tvitem . Header . ToString ( ) )
                    {
                        Task<bool> asyncresult;
                        ExpArgs . tvitem = item;
                        asyncresult = ExpandCurrentAllLevels ( Args );
                        if ( asyncresult . IsCompleted && TextToSearchFor != "" )
                            MessageBox . Show ( $"[{Searchtext . Text}] FOUND ...." , "Search System" );
                        IterateGo = true;
                    }
                }
                ExpandSetup ( false );
                ScrollCurrentTvItemIntoView ( ( TreeViewItem ) Args [ 0 ] );
                return true;
            }
            else if ( selindex == 7 )   //CollapseAllDrives
            {
                // Collapse All Drives
                ExpArgs . tv = ActiveTree;
                CollapseAllDrives ( );
                Mouse . OverrideCursor = Cursors . Arrow;
                ExpandSetup ( false );
                ScrollCurrentTvItemIntoView ( ( TreeViewItem ) ActiveTree . Items [ 0 ] );
                return true;
            }
            #endregion  SPECIFIC SELECTIONS

            #region generic handling

            // go ahead
            TreeViewItem tview = new TreeViewItem ( );
            tview = ExpArgs . tvitem;
            tview . IsSelected = true;
            ActiveTree . Refresh ( );
            if ( ExpArgs . IsFullExpand )
            {
                worker_ProgressChanged ( sender , new ProgressChangedEventArgs ( 100 , null ) );
            }
            else
            {
                worker_ProgressChanged ( sender , new ProgressChangedEventArgs ( 100 , null ) );
            }

            #endregion generic handling
            try
            {
                TreeViewItem tv2 = new TreeViewItem ( );
                tv2 = tview;
                string test = tv2 . Tag . ToString ( );
                if ( test . Length == 3 )
                    Debug. WriteLine ( $"Starting at parent node of {test}.." );
                else
                {
                    tv2 = tview . Parent as TreeViewItem;
                    test = tv2 . Tag . ToString ( );
                    if ( test . Length > 3 )
                    {
                        while ( test . Length > 3 )
                        {
                            TreeViewItem tv3 = new TreeViewItem ( );
                            tv3 = tv2 . Parent as TreeViewItem;
                            test = tv3 . Tag . ToString ( );
                            if ( test . Length > 3 )
                                tv2 = tv3;
                            else
                            {
                                ExpArgs . Parent = tv3;
                                break;
                            }
                        }
                    }
                }
                Debug. WriteLine ( $"Parent is {test}" );
            }
            catch ( Exception ex ) { Debug. WriteLine ( $"RunRecurse: 4020 : {ex . Message}" ); }
            return true;
        }
        public bool IsExpanding { get; set; } = false;
        public int currentexpandlevel { get; set; } = 1;
        private void TestTree_Expanded ( object sender , RoutedEventArgs e )
        {
            // All working when clicking on any folder !!!!
            // this gets callled iteratively  as it progress down a tree of subdirectories

            //            Dispatcher . BeginInvoke ( DispatcherPriority . Normal , ( Action ) ( async ( ) => await TestTree_ExpandedAsync ( tvi , ExpArgs ) ) );
            string currentHeader = "";
            int currentlevel = 0;
            BusyLabel . Text = "Busy  ...";
            if ( Expandprogress . Text == "Finished ..." || Expandprogress . Text == "" )
                StartTimer ( );
            // Needed to let us show the volume label if the option is checked
            TreeViewItem Caller = new TreeViewItem ( );
            TreeViewItem item = null;
            int itemscount = 0;
            if ( IsExpanding == false )
            {
                if ( e != null )
                    item = e . Source as TreeViewItem;
                else
                    item = sender as TreeViewItem;
                if ( item == null )
                {
                    if ( TestTree . SelectedItem != null )
                        item = TestTree . SelectedItem as TreeViewItem;
                    else
                    {
                        iterations = 0;
                        BusyLabel . Text = "";
                        return;
                    }
                }
            }
            else
            {
                item = e . Source as TreeViewItem;
                //ExpandSpecifiedLevels ( item , e );
            }
            Caller = item;
            currentlevel = GetCurrentLevel ( item . Tag . ToString ( ) );
            currentHeader = item . Header . ToString ( );
            Debug. WriteLine ( $"Level = {currentlevel} : {item . Header}  ||   {item . Tag}" );
            // This is CRITICAL to get any drive that is currently selected to open when the expand icon is clicked

            item . IsSelected = true;
            Selection . Text = $"{item . Tag . ToString ( )}";
            //            ActiveTree . HorizontalContentAlignment = HorizontalAlignment . Left;
            ScrollCurrentTvItemIntoView ( item );
            ActiveTree . Refresh ( );

            var directories = new List<string> ( );
            var Allfiles = new List<string> ( );
            string Fullpath = item . Tag . ToString ( ) . ToUpper ( );
            int DirectoryCount = 0, filescount = 0;
            itemscount = item . Items . Count;
            if ( itemscount == 0 )
            {
                iterations = 0;
                BusyLabel . Text = "";
                return;
            }
            var tvi = item as TreeViewItem;
            Caller . Header = currentHeader;
            var itemheader = item . Items [ 0 ] . ToString ( );
            // Get a list of all items in the current folder
            int dircount = GetDirectoryCount ( Fullpath );
            if ( dircount > 0 )
            {
                int count = GetDirectories ( Fullpath , out directories );
                if ( count > 250 )
                {
                    MessageBoxResult result = MessageBox . Show ( $"Directory {Fullpath} contains {count} Files\nExpanding these will take a considerable time...\n\nAre you sure you want to continue ?" ,
                     "Potential long delay" , MessageBoxButton . YesNoCancel , MessageBoxImage . Warning , MessageBoxResult . Cancel );
                    if ( result == MessageBoxResult . Yes )
                    {
                        // Remove DUMMY entry
                        if ( itemheader != null && itemheader == "Loading" )
                            item . Items . Clear ( );
                        DirectoryCount = count;
                        ShowProgress ( );
                        DirectoryCount = AddDirectoriesToTestTreeview ( directories , item , listBox );
                    }
                    else if ( result == MessageBoxResult . Cancel )
                    {
                        AbortExpand = true;
                        {
                            Caller . Header = currentHeader;
                            iterations = 0;
                            BusyLabel . Text = "";
                            return;
                        }
                    }
                    else
                    {
                        ExpandLimited = true;
                        {
                            Caller . Header = currentHeader;
                            iterations = 0;
                            BusyLabel . Text = "";
                            return;
                        }
                    }
                }
                else
                {
                    DirectoryCount = count;
                    ShowProgress ( );
                    if ( directories . Count > 0 )
                    {
                        if ( item . Items [ 0 ] . ToString ( ) == "Loading" )
                        {
                            item . Items . Clear ( );
                        }
                        iterations++;
                        // Expand folder so we can add Directories to it.
                        item . IsExpanded = true;
                        ScrollCurrentTvItemIntoView ( item );
                        ActiveTree . Refresh ( );
                        DirectoryCount = AddDirectoriesToTestTree ( directories , item , listBox );
                        //                        item . IsExpanded = true;
                    }
                }
            }
            else
            {
                DirectoryCount = 0;
                ShowProgress ( );
            }
            // Now Get FILES

            if ( GetFilesCount ( Fullpath ) > 0 )
            {
                GetFiles ( Fullpath , out Allfiles );
                filescount = Allfiles . Count;
                if ( filescount > 500 )
                {
                    MessageBoxResult result = MessageBox . Show ( $"Directory {Fullpath} contains {filescount} Files\nExpanding these will take a considerable time...\n\nAre you sure you want to expand  thiis  subdirectory?\n\n(Cancel to stop the entire Expansion immediately)" ,
                     "Potential long delay" , MessageBoxButton . YesNoCancel , MessageBoxImage . Warning , MessageBoxResult . Cancel );
                    if ( result == MessageBoxResult . Yes )
                    {
                        if ( item . Items [ 0 ] . ToString ( ) == "Loading" )
                        {
                            item . Items . Clear ( );
                        }
                        AddFilesToTreeview ( Allfiles , item );
                    }
                    else if ( result == MessageBoxResult . Cancel )
                    {
                        AbortExpand = true;
                        {
                            Caller . Header = currentHeader;
                            iterations = 0;
                            BusyLabel . Text = "";
                            return;
                        }
                    }
                    else
                    {
                        ExpandLimited = true;
                        {
                            Caller . Header = currentHeader;
                            iterations = 0;
                            BusyLabel . Text = "";
                            return;
                        }
                    }
                }
                else
                {
                    if ( filescount > 0 )
                        AddFilesToTreeview ( Allfiles , item );
                    else
                    {
                        if ( DirectoryCount == 0 )
                        {
                            if ( item . Items [ 0 ] . ToString ( ) == "Loading" )
                            {
                                //                            item . Items . Clear ( );
                            }
                            item . IsExpanded = false;
                        }
                    }
                    //ActiveTree . HorizontalAlignment = HorizontalAlignment . Left;
                    if ( TrackExpand )
                        ScrollCurrentTvItemIntoView ( item );
                    ActiveTree . Refresh ( );
                }
            }
            // item now has all its Subdirs and files
            if ( DirectoryCount == 0 && Allfiles . Count == 0 )
            {
                try
                {
                    if ( item . Items [ 0 ] . ToString ( ) == "Loading" )
                    {
                        //                       item . Items . Clear ( );
                        ActiveTree . Refresh ( );
                        ShowProgress ( );
                    }
                    item . IsExpanded = false;
                }
                catch { }
                finally
                {
                    item . IsSelected = true;
                }

                if ( ExceptionMessage != "" )
                {
                    Selection . Text = ExceptionMessage;
                    ExceptionMessage = "";
                }
                if ( item . Header . ToString ( ) . ToUpper ( ) . Contains ( "CDROM" ) )
                    Selection . Text = "This item is a CdRom, but No Media has been identified, Access is denied by Windows ...";
                else
                    Selection . Text = "This item does not contain any (Non System / Hidden files), or perhaps Access is denied by Windows ...";
                //Utils . DoErrorBeep ( 280 , 100 , 1 );
            }
            else
            {
                iterations++;
                //  item . IsExpanded = true;
                ActiveTree . UpdateLayout ( );
                ActiveTree . Refresh ( );
                Selection . Text = $"{item . Header . ToString ( )} SubDirectories = {DirectoryCount} , Files = {Allfiles . Count}";


                //if ( currentexpandlevel <= ExpArgs . ExpandLevels )
                //{
                //    e . Source = item . Items [ 0 ];
                //    IsExpanding = true;
                //    //foreach ( TreeViewItem subdir  in item . Items )
                //    //{
                //    //e . Source = subdir;
                //    Debug. WriteLine ( $"Expanding {item . Tag . ToString ( )}" );
                //    TestTree_Expanded ( sender , e );
                //    //}
                //    currentexpandlevel++;
                //}
            }

            ShowProgress ( );
            iterations = 0;
            //            ActiveTree . HorizontalContentAlignment = HorizontalAlignment . Left;
            //if ( Caller != null && Caller . IsExpanded == true )
            //{
            //    Caller . IsSelected = true;
            //    if ( TrackExpand )
            //        ScrollCurrentTvItemIntoView ( Caller );
            //    Caller . BringIntoView ( );
            //    ActiveTree . UpdateLayout ( );
            //    ActiveTree . Refresh ( );
            //}
            currentexpandlevel = 1;
            IsExpanding = false;

            return;
        }

        public bool ExpandSpecifiedLevels ( TreeViewItem tvitem , RoutedEventArgs e )
        {
#pragma warning disable CS0219 // The variable 'level' is assigned but its value is never used
            int level = 1;
#pragma warning restore CS0219 // The variable 'level' is assigned but its value is never used
            IsExpanding = false;
            tvitem . IsExpanded = true;
            foreach ( TreeViewItem subdir in tvitem . Items )
            {
                Debug. WriteLine ( $"Expanding {tvitem . Tag . ToString ( )}" );
                e . Source = tvitem;
                TestTree_Expanded ( this , e );
                IsExpanding = true;
            }
            IsExpanding = false;
            return true;
        }
        private void FlashInfopanel ( string text )
        {
            Selection . Text = "";
            Selection . UpdateLayout ( );
            Thread . Sleep ( 500 );
            Selection . Text = text;
            Selection . UpdateLayout ( );
            Thread . Sleep ( 500 );
            Selection . Text = "";
            Selection . UpdateLayout ( );
            Thread . Sleep ( 300 );
            Selection . Text = text; ;
            Selection . UpdateLayout ( );
        }
        private void ClearExpandArgs ( )
        {
            ExpArgs . tv = ActiveTree;
            ExpArgs . tvitem = null;
            ExpArgs . ExpandLevels = 0;
            ExpArgs . SearchTerm = "";
            ExpArgs . SearchActive = false;
            ExpArgs . Selection = 7;    // default to collapse
            ExpArgs . SearchSuccess = false;
            ExpArgs . MaxItems = 250;
            ExpArgs . ListResults = LISTRESULTS;
            ExpArgs . IsFullExpand = false;
            ExpArgs . Parent = null;
        }
        private void TreeOptions ( object sender , RoutedEventArgs e )
        {
            OptionsPanel . Visibility = Visibility . Visible;
            RefreshOptions ( );
        }
        private void Image_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
        {
            ClosePreviousNode = ( bool ) Opt1cbox . IsChecked;
            LISTRESULTS = ( bool ) Opt2cbox . IsChecked;
            Exactmatch = ( bool ) Opt4cbox . IsChecked;
            ShowVolumeLabels = ( bool ) Opt3cbox . IsChecked;
            ShowAllFiles = ( bool ) Opt5cbox . IsChecked;
            RefreshListBox = ( bool ) Opt6cbox . IsChecked;
            OptionsPanel . Visibility = Visibility . Hidden;
        }
        private string GetDriveInfo ( string arg )
        {
            string str = "";
            DriveInfo di = new DriveInfo ( arg );
            str = di . VolumeLabel;
            return str;
        }
        private void LevelsCombo_Selected ( object sender , SelectionChangedEventArgs e )
        {
            ExpArgs . ExpandLevels = LevelsCombo . SelectedIndex;
        }
        private void ShowallVolumes_Click ( object sender , RoutedEventArgs e )
        {
            ShowAllFiles = !ShowAllFiles;
        }
        private void MenuItem_Click ( object sender , RoutedEventArgs e )
        {
            OptionsPanel . Visibility = Visibility . Visible;
            RefreshOptions ( );
        }
        private void CollapseAll ( object sender , RoutedEventArgs e )
        {
            CollapseAllDrives ( );
        }
        private void CollapseCurrent ( object sender , RoutedEventArgs e )
        {
            TreeViewItem tv = sender as TreeViewItem;
            tv = ActiveTree . SelectedItem as TreeViewItem;
            tv = GetParentNode ( tv );
            if ( tv != null )
                tv . IsExpanded = false;
            //            ActiveTree . HorizontalContentAlignment = HorizontalAlignment . Left;
        }
        private void Close_Click ( object sender , RoutedEventArgs e )
        {
            this . Close ( );
        }

        private void CloseApp_Click ( object sender , RoutedEventArgs e )
        {
            Application . Current . Shutdown ( );
        }

        private void TreviewOptions_Click ( object sender , RoutedEventArgs e )
        {
            if ( OptionsPanel . Visibility == Visibility . Hidden )
                OptionsPanel . Visibility = Visibility . Visible;
            else
                OptionsPanel . Visibility = Visibility . Hidden;
            RefreshOptions ( );
        }

        private void searchcurrent_Click ( object sender , RoutedEventArgs e )
        {
            if ( Searchtext . Text == "" || Searchtext . Text == "Search for...." )
            {
                fdl . ShowInfo ( Flowdoc , canvas , "You need to enter a search term in the Search box below before using this option." , "Red0" );
                return;
            }
            TreeViewItem tv = new TreeViewItem ( );
            tv = ActiveTree . SelectedItem as TreeViewItem;
            if ( tv == null )
            {
                fdl . ShowInfo ( Flowdoc , canvas , "You need to select an item in the TreeView before using this option ..." , "Red5" );
                return;
            }
            tv . IsSelected = true;
            ExpArgs . tvitem = tv;
            ExpArgs . SearchActive = true;
            //ExpArgs . SearchTerm = Searchtext . Text . ToUpper ( );
            //ExpArgs . SearchSuccess = false;
            //ExpArgs .ExpandLevels = Convert.ToInt16(LevelsCombo.SelectedItem) ;
            SearchTree ( sender , e );
        }

        private void searchdrive_Click ( object sender , RoutedEventArgs e )
        {

            TreeViewItem tv = new TreeViewItem ( );
            tv = ActiveTree . SelectedItem as TreeViewItem;
            if ( tv != null )
            {
                tv = GetParentNode ( tv );
                tv . IsSelected = true;
                ExpArgs . tvitem = tv;
                ExpArgs . SearchActive = true;
                SearchTree ( sender , e );
            }
            else
            {
                fdl . ShowInfo ( Flowdoc , canvas , "You need to select an item in the TreeView before using this option ..." , "Red5" );
                return;
            }
        }

        private void CboxExactMatch_Click ( object sender , RoutedEventArgs e )
        {
            if ( ( bool ) CboxExactMatch . IsChecked == false )
                Exactmatch = ( bool ) true;
            else
                Exactmatch = ( bool ) false;
            CboxExactMatch . IsChecked = Exactmatch;
            CboxExactMatch . Refresh ( );
            this . Refresh ( );
            RefreshOptions ( );
        }
        private void ExactMatch_Click ( object sender , RoutedEventArgs e )
        {
            //called by main menu option
            if ( ( bool ) CboxExactMatch . IsChecked == false )
                Exactmatch = ( bool ) true;
            else
                Exactmatch = ( bool ) false;
            CboxExactMatch . IsChecked = Exactmatch;
            Opt4cbox_Click ( sender , e );
            this . Refresh ( );
            RefreshOptions ( );
        }

        private void Showlog_Click ( object sender , RoutedEventArgs e )
        {
            LISTRESULTS = !LISTRESULTS;
            UseListBox . IsChecked = LISTRESULTS;
            Opt2cbox_Click ( sender , e );
            this . Refresh ( );
            RefreshOptions ( );
        }

        private void Closenodes_Click ( object sender , RoutedEventArgs e )
        {
            ClosePreviousNode = !ClosePreviousNode;
            Opt1cbox . IsChecked = ClosePreviousNode;
            Opt1cbox_Click ( sender , e );
            this . Refresh ( );

            RefreshOptions ( );
        }
        private void ShowVolumes_Click ( object sender , RoutedEventArgs e )
        {
            ShowVolumeLabels = !ShowVolumeLabels;
            Opt3cbox . IsChecked = ShowVolumeLabels;
            ShowVolumes . IsChecked = ShowVolumeLabels;
            Opt3cbox_Click ( sender , e );
            this . Refresh ( );
            RefreshOptions ( );
            ShowallVolumes_Click ( sender , e );
        }
        private void ShowHidden_Click ( object sender , RoutedEventArgs e )
        {
            ShowAllFiles = !ShowAllFiles;
            Opt5cbox . IsChecked = ShowAllFiles;
            showallFiles . IsChecked = ShowAllFiles;
            Opt5cbox_Click ( sender , e );
            this . Refresh ( );
            RefreshOptions ( );
            LoadDrives ( ActiveTree );

        }
        private void MainTVMenu_MouseDown ( object sender , MouseButtonEventArgs e )
        {
            MenuItem_GotFocus ( sender , e );
        }

        private void MenuItem_GotFocus ( object sender , RoutedEventArgs e )
        {
            if ( ClosePreviousNode )
                Closenodes . Header = "Do NOT close Searched Nodes if no match";
            else
                Closenodes . Header = "Close Searched Nodes if no match";

            if ( LISTRESULTS )
                showlog . Header = "Do NOT log Search/Expand operations";
            else
                showlog . Header = "Log all Search/Expand operations";

            if ( Exactmatch == false )
                exactmatching . Header = "SEARCH : Require EXACT (full) Match for Success";
            else
                exactmatching . Header = "SEARCH : Allow Partial Matches for Success";

            if ( ShowVolumeLabels == true )
                showVolumes . Header = "Do NOT show Volume labels";
            else
                showVolumes . Header = "Show Volume labels";

            if ( ShowAllFiles == true )
                Showhidden . Header = "Do NOT show Hidden/System files";
            else
                Showhidden . Header = "Show Hidden/System files";
            //if ( RefreshListBox== true )
            //    RefreshListBox . Header = "Do NOT show Hidden/System files";
            //else
            //    RefreshListBox . Header = "Show Hidden/System files";
            this . Refresh ( );
            RefreshOptions ( );
        }
        private void RefreshOptions ( )
        {
            if ( OptionsPanel . Visibility == Visibility . Visible )
            {
                Opt1cbox . IsChecked = ClosePreviousNode;
                Opt1cbox . Foreground = ClosePreviousNode ? FindResource ( "Green3" ) as SolidColorBrush : FindResource ( "Red3" ) as SolidColorBrush;
                Opt2cbox . IsChecked = LISTRESULTS;
                Opt2cbox . Foreground = LISTRESULTS ? FindResource ( "Green3" ) as SolidColorBrush : FindResource ( "Red3" ) as SolidColorBrush;
                Opt3cbox . IsChecked = ShowVolumeLabels;
                Opt3cbox . Foreground = ShowVolumeLabels ? FindResource ( "Green3" ) as SolidColorBrush : FindResource ( "Red3" ) as SolidColorBrush;
                Opt4cbox . IsChecked = Exactmatch;
                Opt4cbox . Foreground = Exactmatch ? FindResource ( "Green3" ) as SolidColorBrush : FindResource ( "Red3" ) as SolidColorBrush;
                Opt5cbox . IsChecked = ShowAllFiles;
                Opt5cbox . Foreground = ShowAllFiles ? FindResource ( "Green3" ) as SolidColorBrush : FindResource ( "Red3" ) as SolidColorBrush;
                Opt6cbox . IsChecked = RefreshListBox;
                Opt6cbox . Foreground = RefreshListBox ? FindResource ( "Green3" ) as SolidColorBrush : FindResource ( "Red3" ) as SolidColorBrush;
                OptionsPanel . Refresh ( );
            }
        }

        private void TestTree_MouseEnter ( object sender , MouseEventArgs e )
        {
            ActiveTree . Refresh ( );
        }

        private void TREEViews_IsMouseDirectlyOverChanged ( object sender , DependencyPropertyChangedEventArgs e )
        {
            ActiveTree . Refresh ( );
        }

        private void TREEViews_MouseEnter ( object sender , MouseEventArgs e )
        {
            ActiveTree . Refresh ( );
            TreeViewItem tvi = e . Source as TreeViewItem;
            // TreeViewItem tvitem = ActiveTree . SelectedItem as TreeViewItem;
            string str = tvi != null ? tvi . Tag . ToString ( ) : "    None";
            //Debug. WriteLine ( $"MouseEnter {str}" );
            e . Handled = true;
        }

        private void TREEViews_MouseMove ( object sender , MouseEventArgs e )
        {
            Point pt = new Point ( );
            pt = e . GetPosition ( TestTree );
            IInputElement dropNode = TestTree . InputHitTest ( pt );
            IInputElement ie = ActiveTree . InputHitTest ( pt );
         }
        private void Opt1cbox_Click ( object sender , RoutedEventArgs e )
        {
            ClosePreviousNode = ( bool ) Opt1cbox . IsChecked;
            Opt1cbox . Content = ClosePreviousNode ? "Yes" : "No";
            if ( ClosePreviousNode )
                Opt1cbox . Foreground = FindResource ( "Green3" ) as SolidColorBrush;
            else
                Opt1cbox . Foreground = FindResource ( "Red3" ) as SolidColorBrush;
            if ( LISTRESULTS && ClosePreviousNode )
            {
                Selection . Text = "Previous Nodes will be closed during Searching....";
            }
            else
            {
                Selection . Text = "Previous Nodes will NOT be closed during Searching....";
            }
            OptionsPanel . Refresh ( );
        }

        private void Opt2cbox_Click ( object sender , RoutedEventArgs e )
        {
            LISTRESULTS = ( bool ) Opt2cbox . IsChecked;
            Opt2cbox . Content = LISTRESULTS ? "Yes" : "No";
            if ( LISTRESULTS )
                Opt2cbox . Foreground = FindResource ( "Green3" ) as SolidColorBrush;
            else
                Opt2cbox . Foreground = FindResource ( "Red3" ) as SolidColorBrush;
            if ( LISTRESULTS )
            {
                Selection . Text = "Search/Expansion information will be listed ....";
                listBox . Items . Clear ( );
                listBox . Items . Add ( "Logging of Expansion and Search\nactivity is current ENABLED." );
                CurrentFolder . Text = "Information / Log  Panel : ENABLED";
            }
            else
            {
                Selection . Text = "Listing Search/Expansion is Disabled ...";
                listBox . Items . Clear ( );
                listBox . Items . Add ( "Logging of Expansion and Search\nactivity is current DISABLED." );
                CurrentFolder . Text = "Information / Log  Panel : DISABLED";
            }
            OptionsPanel . Refresh ( );
        }

        private void Opt3cbox_Click ( object sender , RoutedEventArgs e )
        {
            ShowVolumeLabels = ( bool ) Opt3cbox . IsChecked;
            Opt3cbox . Content = ShowVolumeLabels ? "Yes" : "No";
            if ( ShowVolumeLabels )
                Opt3cbox . Foreground = FindResource ( "Green3" ) as SolidColorBrush;
            else
                Opt3cbox . Foreground = FindResource ( "Red3" ) as SolidColorBrush;
            if ( LISTRESULTS && ShowVolumeLabels )
            {
                Selection . Text = "Volume labels are being shown ....";
            }
            else
            {
                Selection . Text = "Volume labels are not being shown ...";
            }
            //LoadDrives ( ActiveTree );
            UpdateDriveHeader ( ShowVolumeLabels );
            OptionsPanel . Refresh ( );
        }
        private void UpdateDriveHeader ( bool ShowVolumeLabels )
        {
            string drivestring = "";
            string dictvalue = "";
            foreach ( TreeViewItem item in ActiveTree . Items )
            {
                drivestring = item . Tag . ToString ( );
                if ( ShowVolumeLabels )
                    item . Header = drivestring + "  " + Utils . GetDictionaryEntry ( VolumeLabelsDict , drivestring , out dictvalue );
                else
                    item . Header = drivestring;
            }
            ActiveTree . Refresh ( );
        }
        private void Opt4cbox_Click ( object sender , RoutedEventArgs e )
        {
            Exactmatch = ( bool ) Opt4cbox . IsChecked;
            Opt4cbox . Content = Exactmatch ? "Yes" : "No";
            if ( Exactmatch )
                Opt4cbox . Foreground = FindResource ( "Green3" ) as SolidColorBrush;
            else
                Opt4cbox . Foreground = FindResource ( "Red3" ) as SolidColorBrush;
            if ( LISTRESULTS && Exactmatch )
            {
                Selection . Text = "Searching will use EXACT matching....";
            }
            else
            {
                Selection . Text = "Searching will use Partial matching....";
            }
            OptionsPanel . Refresh ( );
        }

        private void Opt5cbox_Click ( object sender , RoutedEventArgs e )
        {
            ShowAllFiles = ( bool ) Opt5cbox . IsChecked;
            Opt5cbox . Content = ShowAllFiles ? "Yes" : "No";
            if ( ShowAllFiles )
                Opt5cbox . Foreground = FindResource ( "Green3" ) as SolidColorBrush;
            else
                Opt5cbox . Foreground = FindResource ( "Red3" ) as SolidColorBrush;
            if ( LISTRESULTS && ShowAllFiles )
            {
                Selection . Text = "Hidden/System files will be shown....";
            }
            else
            {
                Selection . Text = "Hidden/System files will NOT be shown....";
            }
            VolumeLabelsDict . Clear ( );
            LoadDrives ( ActiveTree );
            OptionsPanel . Refresh ( );
        }

        private void Opt6cbox_Click ( object sender , RoutedEventArgs e )
        {
            RefreshListBox = ( bool ) Opt6cbox . IsChecked;
            Opt6cbox . Content = RefreshListBox ? "Yes" : "No";
            if ( RefreshListBox )
                Opt6cbox . Foreground = FindResource ( "Green3" ) as SolidColorBrush;
            else
                Opt6cbox . Foreground = FindResource ( "Red3" ) as SolidColorBrush;
            if ( RefreshListBox )
            {
                Selection . Text = "Listbox will be cleared/reloaded for each new Expansion....";
            }
            else
            {
                Selection . Text = "Listbox will NOT be cleared/reloaded for each new Expansion....";
            }
            OptionsPanel . Refresh ( );

        }


        private void Opt1cbox_Click ( object sender , MouseButtonEventArgs e )
        {
            Opt1cbox . IsChecked = !Opt1cbox . IsChecked;
            Opt1cbox_Click ( sender , new RoutedEventArgs ( null ) );
        }
        private void Opt2cbox_Click ( object sender , MouseButtonEventArgs e )
        {
            Opt2cbox . IsChecked = !Opt2cbox . IsChecked;
            Opt2cbox_Click ( sender , new RoutedEventArgs ( null ) );
        }
        private void Opt3cbox_Click ( object sender , MouseButtonEventArgs e )
        {
            Opt3cbox . IsChecked = !Opt3cbox . IsChecked;
            Opt3cbox_Click ( sender , new RoutedEventArgs ( null ) );
        }
        private void Opt4cbox_Click ( object sender , MouseButtonEventArgs e )
        {
            Opt4cbox . IsChecked = !Opt4cbox . IsChecked;
            Opt4cbox_Click ( sender , new RoutedEventArgs ( null ) );
        }
        private void Opt5cbox_Click ( object sender , MouseButtonEventArgs e )
        {
            Opt5cbox . IsChecked = !Opt5cbox . IsChecked;
            Opt5cbox_Click ( sender , new RoutedEventArgs ( null ) );
        }
        private void Opt6cbox_Click ( object sender , MouseButtonEventArgs e )
        {
            Opt6cbox . IsChecked = !Opt6cbox . IsChecked;
            Opt6cbox_Click ( sender , new RoutedEventArgs ( null ) );
        }

        private void ExpandNode_Click ( object sender , RoutedEventArgs e )
        {
            TreeViewItem tv = new TreeViewItem ( );
            tv = ActiveTree . SelectedItem as TreeViewItem;
            tv . IsExpanded = !tv . IsExpanded;
        }

        private void TestTree_MouseDoubleClick ( object sender , MouseButtonEventArgs e )
        {
            // Handle dbl click on file type entries correctly
            bool result = false;
            TreeViewItem tvi = ActiveTree . SelectedItem as TreeViewItem;
            int files = GetFilesCount ( tvi . Tag . ToString ( ) );
            int dirs = GetDirectoryCount ( tvi . Tag . ToString ( ) );
            if ( dirs == 0 && files <= 0 && tvi . HasItems )
            {
                try
                {
                    if ( tvi . Items [ 0 ] . ToString ( ) == "Loading" )
                    {
                        tvi . Items . Clear ( );
                        result = true;
                    }
                }
                finally
                {
                    if ( result )
                    {
                        tvi . IsExpanded = false;
                        Selection . Text = $"Unable to access {tvi . Header . ToString ( )}";
                        e . Handled = true;
                    }
                }
            }
            else
            {
                if ( tvi . IsExpanded == false )
                {
                    // StartTimer ( );
                    TestTree_Expanded ( sender , null );
                }
            }
        }

        private void ToggleListbox ( object sender , MouseButtonEventArgs e )
        {

        }

        private void clearlog_Click ( object sender , RoutedEventArgs e )
        {
            listBox . Items . Clear ( );
        }

        private void TestTree_SelectedItemChanged ( object sender , RoutedPropertyChangedEventArgs<object> e )
        {
            TreeView tv = sender as TreeView;
            if ( tv == null )
                return;
            TreeViewItem item = tv . SelectedItem as TreeViewItem;
            if ( item != null )
            {
                item . BringIntoView ( );
                item . HorizontalAlignment = HorizontalAlignment . Left;
            }
        }

        private void TreeViewItem_RequestBringIntoView ( object sender , RequestBringIntoViewEventArgs e )
        {
            //stop horizontal scrolling when filling TV
            e . Handled = true;
        }

        private void ToggleTreeview ( object sender , MouseButtonEventArgs e )
        {
            if ( TestTree . Visibility == Visibility . Visible )
            {
                ActiveTree = TestTree;
                TestTree . Visibility = Visibility . Hidden;
                TestTree . Visibility = Visibility . Visible;
                //TestTree . ItemsSource = tvitems;
                TestTree . UpdateLayout ( );
                TestTree . Refresh ( );
                testtreebanner . Text = "Manual Directories System, TestTree";
                TestTree . BringIntoView ( );
            }
            else
            {
                ActiveTree = TestTree;
                TestTree . Visibility = Visibility . Hidden;
                TestTree . UpdateLayout ( );
                TestTree . Refresh ( );
                TestTree . ItemsSource = null;
                TestTree . Visibility = Visibility . Visible;
                TestTree . BringIntoView ( );
                testtreebanner . Text = "Manual Directories System, TestTree";
            }
        }

        private void TreeViewItem_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
        {

            return;

#pragma warning disable CS0162 // Unreachable code detected
            Point pt = new Point ( );
#pragma warning restore CS0162 // Unreachable code detected
            pt = e . GetPosition ( TestTree );
            IInputElement dropNode = TestTree . InputHitTest ( pt );
            Type type = dropNode . GetType ( );
            if ( type . Name == "Path" || type . Name == "ScrollViewer" || type . Name == "Grid" || type . Name == "Rectangle" )
                return;
            IInputElement ie = ActiveTree . InputHitTest ( pt );
            var offset = this . VisualOffset;
            TreeViewItem currentitem = TestTree . SelectedItem as TreeViewItem;
            if ( currentitem != null )
            {
                currentitem . IsSelected = false;
            }
            else
            {
                TreeViewItem selitem = GetPathAtPos ( pt );
                if ( selitem == null || selitem . Header == null )
                    return;
                selitem . IsSelected = true;
                TestTree . Refresh ( );
                Debug. WriteLine ( $"Selected item = ={selitem . Tag . ToString ( )}" );
                Selection . Text = $"{selitem . Header . ToString ( )} SubDirectories = {GetDirectoryCount ( selitem . Tag . ToString ( ) )} , Files = {GetFilesCount ( selitem . Tag . ToString ( ) )}";
            }

            TestTree_Expanded ( sender , null );
            return;
        }
        public TreeViewItem GetPathAtPos ( Point point )
        {
#pragma warning disable CS0219 // The variable 'pt' is assigned but its value is never used
            Point pt = new Point ( );
#pragma warning restore CS0219 // The variable 'pt' is assigned but its value is never used
#pragma warning disable CS0219 // The variable 'pt2' is assigned but its value is never used
            Point pt2 = new Point ( );
#pragma warning restore CS0219 // The variable 'pt2' is assigned but its value is never used
            Point pt3 = new Point ( );
            int index = 1;

            double itemht = 0;
            TreeViewItem path = new TreeViewItem ( );
            foreach ( TreeViewItem item in TestTree . Items )
            {
                itemht = item . ActualHeight;
                // pt = item . PointFromScreen ( point );
                pt3 = item . PointToScreen ( point );
                double diff = ( pt3 . Y / 10 );
                //int indx = ( int ) ( diff / 46 ) * 100;
                double match = ( index * itemht );
                Debug. WriteLine ( $"point = {point . Y}, pt3.Y = {pt3 . Y}, match =  {match}, diff = {diff}, result ={match >= diff && match <= diff + itemht} " );
                if ( match >= point . Y && match <= point . Y + itemht )
                {
                    path = item;
                    //break;
                }
                index++;
                int count = this . VisualChildrenCount;
            }
            return path;
        }

        private void TrackExpansion_Click ( object sender , RoutedEventArgs e )
        {
            TrackExpand = !TrackExpand;
            if ( TrackExpand )
                trackitemexpansion . Header = "Don't Track Expanded items";
            else
                trackitemexpansion . Header = "Track Expanded items";
        }

        private void StartStory ( object sender , RoutedEventArgs e )
        {

        }

        private void togglelog_Click ( object sender , RoutedEventArgs e )
        {

        }

        private void TVStoryboardstart_Click ( object sender , RoutedEventArgs e )
        {

        }

        private void TViiewert_Click ( object sender , RoutedEventArgs e )
        {
            TreeViewer tvr = new TreeViewer ("ALL" );
            tvr . Show ( );
        }

        private void ShowExpander ( object sender , RoutedEventArgs e )
        {
            TreeViewItem tvi = TestTree.SelectedItem as TreeViewItem;

            TreeViewer tvr = new TreeViewer ( tvi.Header.ToString());
            tvr . Show ( );
        }

        private void click0 ( object sender , RoutedEventArgs e )
        {

        }


        private void DirectoryOptions_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
        {
        }

        private void DirectoryOptions_DropDownOpened ( object sender , EventArgs e )
        {
            // System . Windows . Controls . MenuItem mi = sender as System . Windows . Controls . MenuItem;
            DirectoryOptions . SelectedItem = ComboSelectedItem;
        }

        private void ComboBoxItem_Selected ( object sender , RoutedEventArgs e )
        {
            var v = e . OriginalSource;
        }
    }
}
// End of CLASS TreeViews

//public class MyVirtualizingStackPanel : VirtualizingStackPanel
//{
//    /// <summary>
//    /// Publically expose BringIndexIntoView.
//    /// </summary>
//    public void BringIntoView ( int index )
//    {

//        this . BringIndexIntoView ( index );
//    }
//

public class TreeViewClass : TreeView
{
    TreeViewClass tvclass = new TreeViewClass ( );
    TreeViewClass ( )
    {
    }
}