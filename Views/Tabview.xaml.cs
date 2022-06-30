using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Diagnostics;
using System . Threading;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Media . Animation;
using System . Windows . Media . Effects;

//using DocumentFormat . OpenXml . Wordprocessing;

using NewWpfDev . Converts;
using NewWpfDev . Models;
using NewWpfDev . UserControls;
using NewWpfDev . ViewModels;

using static NewWpfDev . Views . Tabview;

namespace NewWpfDev . Views {
    /// <summary>
    /// Interaction logic for Tabview.xaml
    /// </summary>
    public partial class Tabview : Window {

        #region NotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged ( string propertyName ) {
            if ( PropertyChanged != null ) {
                PropertyChanged ( this , new PropertyChangedEventArgs ( propertyName ) );
            }
        }
        #endregion OnPropertyChanged

        #region Declarations

        DatagridUserControlViewModel dgvm {
            get; set;
        }
        #region Db Setup
        public ObservableCollection<BankAccountViewModel> Bvm {
            get; private set;
        }
        public ObservableCollection<CustomerViewModel> Cvm {
            get; private set;
        }
        #endregion Db Setup

        //Lists  for datatemplates
        public static List<string> DataTemplatesBank = new List<string> ( );
        public static List<string> DataTemplatesCust = new List<string> ( );
        public static List<string> DataTemplatesGen = new List<string> ( );

        // Pointer to the special library FlowdocLib.cs 
        FlowdocLib fdl = new FlowdocLib ( );

        #region Tab CONTROL STRUCTURES

        /*
         * struct Tabcntrl :
            Tabview . Tabcntrl .  tabView
            Tabview . Tabcntrl .  twVModel
            Tabview . Tabcntrl . CurrentTypeDg
            Tabview . Tabcntrl . CurrentTypeLb
            Tabview . Tabcntrl . CurrentTypeLv 
            Tabview . Tabcntrl . dguserctrl
            Tabview . Tabcntrl . lbuserctrl 
            Tabview . Tabcntrl . lvuserctrl 
            Tabview . Tabcntrl . DtTemplates
            DtTemplates . TemplateIndexDg 
            DtTemplates . TemplateIndexLb
            DtTemplates . TemplateIndexLv 
            DtTemplates . TemplateListDg 
            DtTemplates . TemplateListLb
            DtTemplates . TemplateListLb
            DtTemplates . TemplatesCombo 
        * */
        public struct DataTemplates {
            // A sole instance of this Structure is contained as DtTemplates in TabController struct - as Tabcntrl
            public ComboBox TemplatesCombo { get; set; }
            public int TemplateIndexDg { get; set; }
            public int TemplateIndexLb { get; set; }
            public int TemplateIndexLv { get; set; }
            public string TemplateListDg { get; set; }
            public string TemplateListLb { get; set; }
            public string TemplateListLv { get; set; }
        }
        public struct TabController {
            //
            // Main Control structure for the TabView Tab Controls windows
            //
            public DataTemplates DtTemplates;
            public object ActiveControlType { get; set; }
            public DgUserControl dgUserctrl { get; set; }
            public LbUserControl lbUserctrl { get; set; }
            public LvUserControl lvUserctrl { get; set; }
            public string CurrentTypeDg { get; set; }
            public string CurrentTypeLb { get; set; }
            public string CurrentTypeLv { get; set; }
            public string CurrentTabName { get; set; }
            public TabControl tabControl { get; set; }
            public Tabview tabView { get; set; }
            public TabItem tabItem { get; set; }
            public TabWinViewModel twVModel { get; set; }
            public bool Selectionchanged { get; set; }
        }

        //Structure to control Templates for all tabs
        public static DataTemplates DtTemplates = new DataTemplates ( );
        public static TabController Tabcntrl = new TabController ( );

        #endregion Tab CONTROL STRUCTURES

        #region general properties
        public static object MovingObject {
            get; set;
        }
        private CancellationTokenSource currentCancellationSource;
        private int msgcounter { get; set; } = 1;
        public static Tabview tabvw {
            get; set;
        }
        public static TabItem tabitem {
            get; set;
        }
        public static TabWinViewModel ControllerVm {
            get; set;
        }
        public static TabControl currenttab {
            get; set;
        }
        #endregion properties

        #region ALL Dependency Properties


        public ComboBox Combobox {
            get { return ( ComboBox ) GetValue ( ComboboxProperty ); }
            set { SetValue ( ComboboxProperty , value ); }
        }
        public static readonly DependencyProperty ComboboxProperty =
            DependencyProperty . Register ( "Combobox" , typeof ( ComboBox ) , typeof ( Tabview ) , new PropertyMetadata ( ( ComboBox ) null ) );


        public bool ViewersLinked {   // DP VIEWERSLINKED
            get {
                return ( bool ) GetValue ( ViewersLinkedProperty );
            }
            set {
                SetValue ( ViewersLinkedProperty , value );
                /////Debug . WriteLine ( "Viewer Linkage changed" );
            }
        }
        public static readonly DependencyProperty ViewersLinkedProperty =
            DependencyProperty . Register ( "ViewersLinked" , typeof ( bool ) ,
                typeof ( Tabview ) , new PropertyMetadata ( ( bool ) false ) );

        public string DbType {
            get {
                return ( string ) GetValue ( DbTypeProperty );
            }
            set {
                SetValue ( DbTypeProperty , value );
                NotifyPropertyChanged ( nameof ( DbType ) );
                SetDbType ( value );
            }
        }
        public static readonly DependencyProperty DbTypeProperty =
            DependencyProperty . Register ( "DbType" , typeof ( string ) , typeof ( Tabview ) , new PropertyMetadata ( "BANK" ) );

        public Tabview TabViewWin {// DP TABVIEWWIN
            get {
                return ( Tabview ) GetValue ( TabViewWinProperty );
            }
            set {
                SetValue ( TabViewWinProperty , value );
            }
        }
        public static readonly DependencyProperty TabViewWinProperty =
            DependencyProperty . Register ( "TabViewWin" , typeof ( Tabview ) ,
            typeof ( Tabview ) , new PropertyMetadata ( ( Tabview ) null ) );

        #region User Control DP;s
        public DgUserControl Dgusercontrol {   // DP DGUSERCTRL
            get {
                return ( DgUserControl ) GetValue ( DgusercontrolProperty );
            }
            set {
                SetValue ( DgusercontrolProperty , value );
            }
        }
        public static readonly DependencyProperty DgusercontrolProperty =
            DependencyProperty . Register ( "Dgusercontrol" , typeof ( DgUserControl ) ,
                typeof ( Tabview ) , new PropertyMetadata ( ( DgUserControl ) null ) );

        public LbUserControl Lbusercontrol {   // DP LBUSERCTRL
            get {
                return ( LbUserControl ) GetValue ( LbUserControlProperty );
            }
            set {
                SetValue ( LbUserControlProperty , value );
            }
        }
        public static readonly DependencyProperty LbUserControlProperty =
            DependencyProperty . Register ( "Lbusercontrol" , typeof ( LbUserControl ) ,
                typeof ( Tabview ) , new PropertyMetadata ( ( LbUserControl ) null ) );

        public LvUserControl Lvusercontrol {   // DP LVUSERCTRL
            get {
                return ( LvUserControl ) GetValue ( LvUserControlProperty );
            }
            set {
                SetValue ( LvUserControlProperty , value );
            }
        }
        public static readonly DependencyProperty LvUserControlProperty =
            DependencyProperty . Register ( "Lvusercontrol" , typeof ( LvUserControl ) ,
                typeof ( Tabview ) , new PropertyMetadata ( ( LvUserControl ) null ) );

        #endregion User Control DP;s
        #endregion ALL Dependency Properties

        #region Attached properties

        // GLOBAL DATAGRID
        public static DataGrid GetDataGrid ( DependencyObject obj ) {
            if ( DGControlProperty == null ) return null;
            return ( DataGrid ) obj . GetValue ( DGControlProperty );
        }
        public static void SetDGControl ( DependencyObject obj , DataGrid value ) {
            obj . SetValue ( DGControlProperty , value );
        }
        public static readonly DependencyProperty DGControlProperty =
            DependencyProperty . RegisterAttached ( "DGControl" , typeof ( DataGrid ) ,
            typeof ( Tabview ) , new PropertyMetadata ( ( DataGrid ) null , OnDataGridSet ) );
        private static void OnDataGridSet ( DependencyObject d , DependencyPropertyChangedEventArgs e ) {
            //Debug . WriteLine ( $"DGControl set to {e . NewValue}" );
        }

        // GLOBAL LISTBOX
        public static ListBox GetListBox ( DependencyObject obj ) {
            return ( ListBox ) obj . GetValue ( LBControlProperty );
        }
        public static void SetListBox ( DependencyObject obj , ListBox value ) {
            obj . SetValue ( LBControlProperty , value );
        }
        public static readonly DependencyProperty LBControlProperty =
            DependencyProperty . RegisterAttached ( "LBControl" , typeof ( ListBox ) , typeof ( Tabview ) , new PropertyMetadata ( ( ListBox ) null ) );

        // GLOBAL LISTVIEW`
        public static ListView GetListView ( DependencyObject obj ) {
            return ( ListView ) obj . GetValue ( LVControlProperty );
        }
        public static void SetListView ( DependencyObject obj , ListView value ) {
            obj . SetValue ( LVControlProperty , value );
        }
        public static readonly DependencyProperty LVControlProperty =
            DependencyProperty . RegisterAttached ( "LVControl" , typeof ( ListView ) , typeof ( Tabview ) , new PropertyMetadata ( ( ListView ) null ) );



        #endregion Attached properties

        #endregion Declarations

        #region Startup
        public Tabview ( ) {
            Mouse . OverrideCursor = Cursors . Wait;
            tabvw = this;
            TabViewWin = this;
            CreateControlStructs ( );
            InitializeComponent ( );
            Combobox = this . TemplatesCb;
            Tabview . Tabcntrl . tabControl = this . Tabctrl;
            Tabview . Tabcntrl . tabView = this;
            Tabview . Tabcntrl . tabItem = Tabview . Tabcntrl . tabControl . Items [ 0 ] as TabItem;
            Tabview . Tabcntrl . DtTemplates . TemplatesCombo = Combobox;

            this . Left = 50;
            this . Top = 100;
            SizeChanged += Tabview_SizeChanged;
            ControllerVm = TabWinViewModel . SetPointer ( this , "DgridTab" );
            this . Show ( );
            this . DataContext = ControllerVm;
            TabWinViewModel . CurrentTabIndex = 0;
            if ( TabWinViewModel . Tabcontrol != null )
                currenttab = TabWinViewModel . Tabcontrol;
            EventControl . WindowMessage += InterWinComms_WindowMessage;
            UseTask . IsChecked = ControllerVm . USETASK;
            UseWorker . IsChecked = !ControllerVm . USETASK;
            LoadTemplates ( );
            //Maximize hook  +/- statements
            Flowdoc . ExecuteFlowDocMaxmizeMethod += new EventHandler ( MaximizeFlowDoc );
            FlowDoc . FlowDocClosed += Flowdoc_FlowDocClosed;
            TemplatesCb . SelectionChanged += TemplatesCb_SelectionChanged;
            TemplatesCb . UpdateLayout ( );
            dgvm = new DatagridUserControlViewModel ( );
            Bvm = dgvm . Bvm;
            Cvm = dgvm . Cvm;
            Tabview . Tabcntrl . DtTemplates . TemplatesCombo . ItemsSource = DataTemplatesBank;
            Tabview . Tabcntrl . DtTemplates . TemplatesCombo . SelectedIndex = 0;
            Mouse . OverrideCursor = Cursors . Arrow;
        }
        public static Tabview GetTabview ( ) {   // Return  pointer to ourselves (TABVIEW)
            return tabvw;
        }
        public void SetViewerLinkage ( bool Islinked ) {   //set our DP for viewer linkage
            ViewersLinked = Islinked;
        }
        public static void SetDbType ( string dbname ) {   //set current Db Type
            TabWinViewModel . TriggerDbType ( dbname );
        }
        private void Linkall ( object sender , RoutedEventArgs e ) {   // Set linkage  for control indexes
            bool val = ( bool ) linkViewers . IsChecked;
            SetViewerLinkage ( val );
            LvUserControl . SetListSelectionChanged ( val );
            LbUserControl . SetListSelectionChanged ( val );
            DgUserControl . SetListSelectionChanged ( val );
            linkViewers . IsChecked = val;
            ViewersLinked = val;
        }
        private async void Window_Loaded ( object sender , RoutedEventArgs e ) {
            Tabctrl . SelectedIndex = 0;
            TabWinViewModel . IsLoadingDb = false;
            await ControllerVm . SetCurrentTab ( this , "DgridTab" );
        }
        #endregion Startup

        private void Flowdoc_FlowDocClosed ( object sender , EventArgs e ) {
            canvas . Visibility = Visibility . Collapsed;
        }

        #region Intra window messaging
        private void SendWindowMessage ( string msg = "" ) {
            //Send a broadcast message out
            InterWindowArgs args = new InterWindowArgs ( );
            args . data = null;
            args . window = this;
            if ( msg == "" )
                args . message = $"Broadcast Message from Tabview :- {msgcounter++} ";
            else
                args . message = msg;
            EventControl . TriggerWindowMessage ( this , args );
        }

        private void InterWinComms_WindowMessage ( object sender , InterWindowArgs e ) {
            // Recieve a message and display it in listbox
            string msg = e . message;
            //Debug. WriteLine ( $"Tabview : System data transmission system message received : Sender was  Message : {msg}" );
            if ( TabWinViewModel . logUserctrl == null ) return;
            TabWinViewModel . logUserctrl?.logview?.Items . Add ( msg );
            TabWinViewModel . logUserctrl?.logview . ScrollIntoView ( TabWinViewModel . logUserctrl . logview . Items . Count - 1 );
            Utils . ScrollLBRecordIntoView ( TabWinViewModel . logUserctrl?.logview , TabWinViewModel . logUserctrl . logview . Items . Count - 1 );
            TabWinViewModel . logUserctrl?.logview?.UpdateLayout ( );
        }
        private void SendWinMsg ( object sender , RoutedEventArgs e ) {
            // Broadcast information
            SendWindowMessage ( );
        }

        #endregion Intra window messaging

        public void TabSizeChanged ( object sender , SizeChangedEventArgs e ) {
            // Helper called by other UserControls to resize their content before viewing
            Tabview_SizeChanged ( sender , e );
        }

        #region window resizing
        private void Tabview_SizeChanged ( object sender , SizeChangedEventArgs e ) {
            ReduceByParamValue rbp = new ReduceByParamValue ( );
            var v = currenttab?.ActualWidth;
            if ( Tabview . Tabcntrl . dgUserctrl != null ) {
                ResizeDatagridTab ( );
            }
            if ( Tabview . Tabcntrl . lbUserctrl != null ) {
                ResizeListboxTab ( );
            }
            if ( Tabview . Tabcntrl . lvUserctrl != null ) {
                ResizeListviewTab ( );
            }
            if ( TabWinViewModel . tvUserctrl != null ) {
                ResizeTreeviewTab ( );
            }
        }

        public static void ResizeDatagridTab ( ) {
            if ( currenttab == null ) return;
            Tabview . Tabcntrl . dgUserctrl . Width = currenttab . ActualWidth;// - 5;
            Tabview . Tabcntrl . dgUserctrl . Height = currenttab . ActualHeight;// - 30;
            Thickness th = new Thickness ( 0 , 0 , 0 , 0 );
            th = Tabview . Tabcntrl . dgUserctrl . grid1 . Margin;
            th . Left = 0;
            Tabview . Tabcntrl . dgUserctrl . grid1 . Margin = th;
            Tabview . Tabcntrl . dgUserctrl . grid1 . Height = Tabview . Tabcntrl . dgUserctrl . Height - 55;
            Tabview . Tabcntrl . dgUserctrl . grid1 . Width = Tabview . Tabcntrl . dgUserctrl . Width - 20;
            Tabview . Tabcntrl . dgUserctrl . grid1 . VerticalAlignment = VerticalAlignment . Top;
            Tabview . Tabcntrl . dgUserctrl . grid1 . UpdateLayout ( );
        }
        public static void ResizeListboxTab ( ) {
            if ( currenttab == null ) return;
            var width = TabWinViewModel . Tview . Width;
            Tabview . Tabcntrl . lbUserctrl . Width = currenttab . ActualWidth;// - 5;
            Tabview . Tabcntrl . lbUserctrl . Height = currenttab . ActualHeight - 30;
            Thickness th = new Thickness ( 0 , 0 , 0 , 0 );
            th = Tabview . Tabcntrl . lbUserctrl . Margin;
            th . Left = 0;
            th . Right = 0;
            th . Top = 5;
            Tabview . Tabcntrl . lbUserctrl . Margin = th;

            Tabview . Tabcntrl . lbUserctrl . listbox1 . Height = Tabview . Tabcntrl . lbUserctrl . Height - 30;
            Tabview . Tabcntrl . lbUserctrl . listbox1 . Width = Tabview . Tabcntrl . lbUserctrl . Width - 20;
            Tabview . Tabcntrl . lbUserctrl . listbox1 . VerticalAlignment = VerticalAlignment . Top;
            Tabview . Tabcntrl . lbUserctrl . listbox1 . UpdateLayout ( );
        }
        public static void ResizeListviewTab ( ) {
            if ( currenttab == null ) return;
            Tabview . Tabcntrl . lvUserctrl . Width = currenttab . ActualWidth;// - 5;
            Tabview . Tabcntrl . lvUserctrl . Height = currenttab . ActualHeight;// - 20;
            Thickness th = new Thickness ( 0 , 0 , 0 , 0 );
            th = Tabview . Tabcntrl . lvUserctrl . Margin;
            th . Left = 0;
            th . Right = 0;
            th . Top = 0;
            Tabview . Tabcntrl . lvUserctrl . Margin = th;
            th = Tabview . Tabcntrl . lvUserctrl . listview1 . Margin;
            th . Left = 5;
            th . Top = 5;
            th . Right = 20;
            th . Bottom = 0;
            Tabview . Tabcntrl . lvUserctrl . listview1 . Margin = th;
            Tabview . Tabcntrl . lvUserctrl . listview1 . Height = Tabview . Tabcntrl . lvUserctrl . Height - 55;
            Tabview . Tabcntrl . lvUserctrl . listview1 . Width = Tabview . Tabcntrl . lvUserctrl . Width - 25;
            Tabview . Tabcntrl . lvUserctrl . listview1 . VerticalAlignment = VerticalAlignment . Top;
            Tabview . Tabcntrl . lvUserctrl . listview1 . UpdateLayout ( );
        }
        public static void ResizeTreeviewTab ( ) {
            TabWinViewModel . tvUserctrl . Width = TabWinViewModel . Tabcontrol . ActualWidth;// - 5;
            TabWinViewModel . tvUserctrl . Height = TabWinViewModel . Tabcontrol . ActualHeight - 20;
            Thickness th = new Thickness ( 0 , 0 , 0 , 0 );
            th = TabWinViewModel . tvUserctrl . Margin;
            th . Left = 0;
            th . Right = 10;
            th . Top = 0;
            TabWinViewModel . tvUserctrl . Margin = th;
            th = TabWinViewModel . tvUserctrl . treeview1 . Margin;
            th . Left = 5;
            th . Top = 5;
            th . Right = 20;
            th . Bottom = 0;
            TabWinViewModel . tvUserctrl . treeview1 . Margin = th;
            TabWinViewModel . tvUserctrl . treeview1 . Height = TabWinViewModel . tvUserctrl . Height - 35;
            TabWinViewModel . tvUserctrl . treeview1 . Width = TabWinViewModel . tvUserctrl . Width - 25;
            TabWinViewModel . tvUserctrl . treeview1 . VerticalAlignment = VerticalAlignment . Top;
            TabWinViewModel . tvUserctrl . treeview1 . UpdateLayout ( );
        }

        #endregion resizing

        private void tabview_Closed ( object sender , EventArgs e ) {
            // cleanup FLowDoc before closing down
            Flowdoc . ExecuteFlowDocMaxmizeMethod -= new EventHandler ( MaximizeFlowDoc );
            FlowDoc . FlowDocClosed -= Flowdoc_FlowDocClosed;
            // Close App
            ControllerVm . Closedown ( );
        }

        #region Left Mouse Ckick on tabs Trigger Methods
        private void GridMouseLeftButtonDown ( object sender , MouseButtonEventArgs e ) {
            ControllerVm . SetCurrentTab ( this , "DgridTab" );
        }
        private async void ListboxMouseLeftButtonDown ( object sender , MouseButtonEventArgs e ) {
            await ControllerVm . SetCurrentTab ( this , "ListboxTab" );
        }
        private async void ListviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e ) {
            await ControllerVm . SetCurrentTab ( this , "ListviewTab" );
        }
        private async void LogviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e ) {
            await ControllerVm . SetCurrentTab ( this , "LogviewTab" );
        }
        private async void TreeviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e ) {
            await ControllerVm . SetCurrentTab ( this , "TreeviewTab" );
        }

        #endregion Left Mouse Ckick on Tabs
        public bool CheckAllControlIndexes ( int index ) {
            if ( Tabview . Tabcntrl . DtTemplates . TemplateIndexDg == index
                && Tabview . Tabcntrl . DtTemplates . TemplateIndexLb == index
                && Tabview . Tabcntrl . DtTemplates . TemplateIndexLv == index )
                return true;
            return false;
        }

        #region Tab Cleanup
        public void ClearTab ( UIElement element ) {
            Type type = element . GetType ( );
            if ( type . Name == "DgUserControl" ) {
                Tabview . Tabcntrl . dgUserctrl . grid1 . ItemsSource = null;
                Tabview . Tabcntrl . dgUserctrl . grid1 . Items . Clear ( );
                Tabview . Tabcntrl . dgUserctrl . UpdateLayout ( );
                //DgridTab . Content = null;
                //Tabview . Tabcntrl . dgUserctrl = null;
                //Tabview tview = TabWinViewModel . SendTabview ( );
                //ControllerVm . SetCurrentTab ( tview , "DgridTab" );
            }
            else if ( type . Name == "LbUserControl" ) {
                Tabview . Tabcntrl . lbUserctrl . listbox1 . ItemsSource = null;
                Tabview . Tabcntrl . lbUserctrl . listbox1 . Items . Clear ( );
                Tabview . Tabcntrl . lbUserctrl . UpdateLayout ( );
                //ListboxTab . Content = null;
                //                Tabview . Tabcntrl . lbUserctrl = null;
                //Tabview tview = TabWinViewModel . SendTabview ( );
                //ControllerVm . SetCurrentTab ( tview , "ListboxTab" );
            }
            else if ( type . Name == "LvUserControl" ) {
                Tabview . Tabcntrl . lvUserctrl . listview1 . ItemsSource = null;
                Tabview . Tabcntrl . lvUserctrl . listview1 . Items . Clear ( );
                Tabview . Tabcntrl . lvUserctrl . UpdateLayout ( );
                //ListviewTab . Content = null;
                //             Tabview . Tabcntrl . lvUserctrl = null;
                //Tabview tview = TabWinViewModel . SendTabview ( );
                //ControllerVm . SetCurrentTab ( tview , "ListviewTab" );
            }
        }
        private void clearTabs ( object sender , RoutedEventArgs e ) {
            if ( Tabview . Tabcntrl . lbUserctrl != null ) {
                Tabview . Tabcntrl . lbUserctrl . listbox1 . ItemsSource = null;
                Tabview . Tabcntrl . lbUserctrl . listbox1 . Items . Clear ( );
                //ListboxTab . Content = null;
                //Tabview . Tabcntrl . lbUserctrl = null;
            }
            if ( Tabview . Tabcntrl . lvUserctrl != null ) {
                Tabview . Tabcntrl . lvUserctrl . listview1 . ItemsSource = null;
                Tabview . Tabcntrl . lvUserctrl . listview1 . Items . Clear ( );
                //ListviewTab . Content = null;
                //Tabview . Tabcntrl . lvUserctrl = null;
            }
            if ( Tabview . Tabcntrl . dgUserctrl != null ) {
                Tabview . Tabcntrl . dgUserctrl . grid1 . ItemsSource = null;
                Tabview . Tabcntrl . dgUserctrl?.grid1 . Items . Clear ( );
                //DgridTab . Content = null;
                //Tabview . Tabcntrl . dgUserctrl = null;
            }
            if ( TabWinViewModel . logUserctrl != null ) {
                TabWinViewModel . logUserctrl?.logview . Items . Clear ( );
                TabWinViewModel . logUserctrl?.logview . UpdateLayout ( );
                //LogviewTab . Content = null;
                //TabWinViewModel . logUserctrl = null;
            }
            if ( TabWinViewModel . tvUserctrl != null ) {
                TabWinViewModel . tvUserctrl . treeview1 . ItemsSource = null;
                TabWinViewModel . tvUserctrl?.treeview1 . Items . Clear ( );
                TreeviewTab . Content = null;
                TabWinViewModel . tvUserctrl = null;
            }
            DgridTab . Refresh ( );
            ListboxTab . Refresh ( );
            ListviewTab . Refresh ( );
            TreeviewTab . Refresh ( );
            LogviewTab . Refresh ( );
            DbCountArgs args = new DbCountArgs ( );
            args . Dbcount = 0;
            TabWinViewModel . TriggerBankDbCount ( this , args );
            //var v = sender . GetType ( );
            //if ( v == typeof ( Button ) ) {
            //    Dictionary<string , object> dict = ViewModel . GetAllViewModels ( );
            //    if ( Viewmodels . Visibility == Visibility . Visible ) {
            //        Viewmodels . Visibility = Visibility . Collapsed;
            //        ViewModel . ClearDictionary ( );
            //    }
            //    else {
            //        Viewmodels . Items . Clear ( );
            //        foreach ( KeyValuePair<string , object> entry in dict ) {
            //            Viewmodels . Items . Add ( entry . Key );
            //            var type = entry . Value . GetType ( );
            //            switch ( type . Name ) {
            //                case "DGUSERCONTROL":
            //                    break;
            //                case "LBUSERCONTROL":
            //                    break;
            //                case "LVUSERCONTROL":
            //                    break;
            //                case "LOGUSERCONTROL":
            //                    break;
            //                case "TVUSERCONTROL":
            //                    break;
            //            }
            //        }
            //        Viewmodels . Visibility = Visibility . Visible;
            //    }
            //}
        }

        #endregion Tab Cleanup
        private async void Button_Click ( object sender , RoutedEventArgs e ) {
        }
        private async Task CountToOneHundredAsync ( IProgress<int> progress , CancellationToken cancellationToken ) {
            for ( int i = 1 ; i <= 100 ; i++ ) {
                // This is where the 'work' is performed. 
                // Feel free to swap out Task.Delay for your own Task-returning code! 
                // You can even await many tasks here

                // ConfigureAwait(false) tells the task that we dont need to come back to the UI after awaiting
                // This is a good read on the subject - https://blog.stephencleary.com/2012/07/dont-block-on-async-code.html
                await Task . Delay ( 100 , cancellationToken ) . ConfigureAwait ( true );

                // If cancelled, an exception will be thrown by the call the task.Delay
                // and will bubble up to the calling method because we used await!

                // Report progress with the current number
                //progress . Report ( i );
                ControllerVm . ProgressValue = i;
            }
        }
        private void Button_Cancel_Click ( object sender , RoutedEventArgs e ) {
            // Cancel the cancellation token
            this . currentCancellationSource . Cancel ( );
        }

        private void usetask ( object sender , RoutedEventArgs e ) {
            ControllerVm . USETASK = ( bool ) UseTask . IsChecked;
            CheckBox cb = sender as CheckBox;
            if ( cb . Name == "UseTask" ) {
                if ( ( bool ) cb . IsChecked == true ) {
                    ControllerVm . USETASK = true;
                    UseWorker . IsChecked = false;
                    UseTask . IsChecked = true;
                }
                else {
                    ControllerVm . USETASK = false;
                    UseWorker . IsChecked = true;
                    UseTask . IsChecked = false;
                }
            }
            else {
                if ( ( bool ) cb . IsChecked == true ) {
                    ControllerVm . USETASK = false;
                    UseWorker . IsChecked = true;
                    UseTask . IsChecked = false;
                }
                else {
                    ControllerVm . USETASK = true;
                    UseTask . IsChecked = true;
                    UseWorker . IsChecked = false;
                }
            }
            if ( Tabview . Tabcntrl . dgUserctrl != null ) {
                Tabview . Tabcntrl . dgUserctrl . grid1 . ItemsSource = null;
                Tabview . Tabcntrl . lbUserctrl . listbox1 . ItemsSource = null;
                Tabview . Tabcntrl . lvUserctrl . listview1 . ItemsSource = null;
                TabWinViewModel . logUserctrl . logview . ItemsSource = null;
                TabWinViewModel . tvUserctrl . treeview1 . ItemsSource = null;
                Tabview . Tabcntrl . dgUserctrl . grid1 . Items . Clear ( );
                Tabview . Tabcntrl . lbUserctrl . listbox1 . Items . Clear ( );
                Tabview . Tabcntrl . lvUserctrl . listview1 . Items . Clear ( );
                TabWinViewModel . logUserctrl . logview . Items . Clear ( );
                TabWinViewModel . tvUserctrl . treeview1 . Items . Clear ( );
            }
            //if ( sender . GetType ( ) == typeof ( CheckBox ) )
            //    Debug . WriteLine ( "" );
            clearTabs ( this , null );
        }

        #region Remote triggers  for mouseover events
        public static void TriggerStoryBoardOn ( int Id ) {
            Storyboard sb;
            switch ( Id ) {
                case 1:
                    sb = tabvw . FindResource ( "TabAnimationOn1" ) as Storyboard;
                    sb . Begin ( );
                    break;
                case 2:
                    sb = tabvw . FindResource ( "TabAnimationOn2" ) as Storyboard;
                    sb . Begin ( );
                    break;
                case 3:
                    sb = tabvw . FindResource ( "TabAnimationOn3" ) as Storyboard;
                    sb . Begin ( );
                    break;
                case 4:
                    sb = tabvw . FindResource ( "TabAnimationOn4" ) as Storyboard;
                    sb . Begin ( );
                    break;
                case 5:
                    sb = tabvw . FindResource ( "TabAnimationOn5" ) as Storyboard;
                    sb . Begin ( );
                    break;
            }
        }
        public static void TriggerStoryBoardOff ( int Id ) {
            Storyboard sb;
            switch ( Id ) {
                case 1:
                    sb = tabvw . FindResource ( "TabAnimationOff1" ) as Storyboard;
                    sb . Begin ( );
                    break;
                case 2:
                    sb = tabvw . FindResource ( "TabAnimationOff2" ) as Storyboard;
                    sb . Begin ( );
                    break;
                case 3:
                    sb = tabvw . FindResource ( "TabAnimationOff3" ) as Storyboard;
                    sb . Begin ( );
                    break;
                case 4:
                    sb = tabvw . FindResource ( "TabAnimationOff4" ) as Storyboard;
                    sb . Begin ( );
                    break;
                case 5:
                    sb = tabvw . FindResource ( "TabAnimationOff5" ) as Storyboard;
                    sb . Begin ( );
                    break;
            }
        }
        #endregion Remote triggers  for mouseover events

        #region UNUSED
        public void PART_MouseLeave ( object sender , MouseEventArgs e ) {
            var tabview = TabWinViewModel . Tview;
            //TabItem  item = TabWinViewModel . CurrentTabitem;
            //Controller . SetCurrentTab ( tabview , TabWinViewModel . CurrentTabName );
            if ( TabWinViewModel . CurrentTabTextBlock == "Tab4Header" ) {
                Tabview . TriggerStoryBoardOff ( 4 );
                tabview . Tab4Header . FontSize = 14;
                tabview . Tab4Header . Foreground = FindResource ( "Cyan0" ) as SolidColorBrush;
            }
        }
        public void PART_MouseEnter ( object sender , MouseEventArgs e ) {
            var tabview = TabWinViewModel . Tview;
            Point pt = e . GetPosition ( ( UIElement ) sender );
            HitTestResult hit = VisualTreeHelper . HitTest ( ( Visual ) sender , pt );
            if ( TabWinViewModel . CurrentTabTextBlock == "Tab4Header" ) {
                Tabview . TriggerStoryBoardOn ( 4 );
                tabview . Tab4Header . FontSize = 18;
                tabview . Tab4Header . Foreground = FindResource ( "Yellow0" ) as SolidColorBrush;
            }
        }

        private void logview_PreviewMouseMove ( object sender , MouseEventArgs e ) {
            //ListBox lbSender = sender as ListBox;
            //if ( lbSender != null )
            //{
            //    if ( lbSender . Name == "listbox1" )
            //    {
            //        TabWinViewModel . CurrentTabIndex = 4;
            //        TabWinViewModel . CurrentTabName = "LogviewTab";
            //        TabWinViewModel . CurrentTabTextBlock = "Tab4Header";
            //    }
            //}
        }
        private void Tab1Mouseoover ( object sender , MouseEventArgs e ) {
            //Debug. WriteLine ("Mousemove....");
            Storyboard MyStoryboard = new Storyboard ( );
            Storyboard s = FindResource ( "TabAnimationTest1" ) as Storyboard;
            MyStoryboard . Children . Add ( s );
            MyStoryboard . Begin ( );
        }

        #endregion UNUSED

        #region Right Clickmethods
        private void Dg_PreviewMouseRightButtonUp ( object sender , MouseButtonEventArgs e ) {
            if ( TabWinViewModel . CurrentTabIndex == 0 ) {
                //Datagrid
                if ( Tabview . Tabcntrl . dgUserctrl == null ) return;
                if ( Tabview . Tabcntrl . dgUserctrl?.Tag == null || ( bool ) Tabview . Tabcntrl . dgUserctrl?.Tag == true )
                    ClearTab ( ( UIElement ) Tabview . Tabcntrl . dgUserctrl );
            }
            else
                MessageBox . Show ( "You need to SELECT the tab before reloading it" , "Reload Error" );
        }

        private void Lb_PreviewMouseRightButtonUp ( object sender , MouseButtonEventArgs e ) {
            if ( TabWinViewModel . CurrentTabIndex == 1 ) {
                //Listbox
                if ( Tabview . Tabcntrl . lbUserctrl?.Tag == null ) return;
                if ( Tabview . Tabcntrl . lbUserctrl?.Tag == null || ( bool ) Tabview . Tabcntrl . lbUserctrl?.Tag == true )
                    ClearTab ( ( UIElement ) Tabview . Tabcntrl . lbUserctrl );
            }
            else
                MessageBox . Show ( "You need to SELECT the tab before reloading it" , "Reload Error" );
        }

        private void Lv_PreviewMouseRightButtonUp ( object sender , MouseButtonEventArgs e ) {
            if ( TabWinViewModel . CurrentTabIndex == 2 ) {
                //listview
                if ( Tabview . Tabcntrl . lvUserctrl == null ) return;
                if ( Tabview . Tabcntrl . lvUserctrl . Tag == null || ( bool ) Tabview . Tabcntrl . lvUserctrl . Tag == true ) {
                    ClearTab ( ( UIElement ) Tabview . Tabcntrl . lvUserctrl );
                    ControllerVm . SetCurrentTab ( TabWinViewModel . Tview , "ListviewTab" );
                }
            }
            else
                MessageBox . Show ( "You need to SELECT the tab before reloading it" , "Reload Error" );
        }

        private void Log_PreviewMouseRightButtonUp ( object sender , MouseButtonEventArgs e ) {
            if ( TabWinViewModel . CurrentTabIndex == 3 ) {
                // LOG VIEW
                if ( TabWinViewModel . logUserctrl == null ) return;
                if ( TabWinViewModel . logUserctrl . Tag == null || ( bool ) TabWinViewModel . logUserctrl . Tag == true ) {
                    var con = Tabctrl . SelectedContent;//as TreeView;
                    if ( con == null ) return;
                    ClearTab ( ( UIElement ) TabWinViewModel . logUserctrl );
                    //                    Type type = con . GetType ( );
                    //                    if ( type . Name != "LogUserControl" ) return;
                    ListBox lb = TabWinViewModel . logUserctrl . logview;
                    lb . Items . Clear ( );
                }
            }
            else {
                MessageBox . Show ( "You need to SELECT the tab before reloading it" , "Reload Error" );
                return;
            }
        }

        private void Tv_PreviewMouseRightButtonUp ( object sender , MouseButtonEventArgs e ) {
            if ( TabWinViewModel . CurrentTabIndex == 4 ) {
                // TREEVIEW
                // Demonstrates how to access whatever control is inside the tab's usercontrol
                if ( TabWinViewModel . tvUserctrl == null ) return;
                if ( TabWinViewModel . tvUserctrl . Tag == null || ( bool ) TabWinViewModel . tvUserctrl . Tag == true ) {
                    var con = Tabctrl . SelectedContent;//as TreeView;
                    Type type = con . GetType ( );
                    if ( type . Name != "TvUserControl" ) return;
                    TreeView tw = TabWinViewModel . tvUserctrl . treeview1;
                    tw . Items . Clear ( );
                }
            }
            else {
                MessageBox . Show ( "You need to SELECT the tab before reloading it" , "Reload Error" );
                return;
            }
        }
        #endregion Right Clickmethods

        private void Btn1_MouseEnter ( object sender , MouseEventArgs e ) {
            Tabview . TriggerStoryBoardOn ( 4 );
            tabview . Btn1 . FontSize = 18;
            tabview . Btn1 . Content = "LOAD";
            tabview . Btn1 . Foreground = FindResource ( "Red5" ) as SolidColorBrush;
            tabview . Btn1 . ToolTip = "Load/Reload Bank Account Data";
            DropShadowEffect DsEffect = new DropShadowEffect ( );
            DsEffect . ShadowDepth = 2;
            DsEffect . BlurRadius = 0.5;
            DsEffect . Direction = 310;
            tabview . Btn1 . Effect = DsEffect;
        }

        private void Btn1_MouseLeave ( object sender , MouseEventArgs e ) {
            Tabview . TriggerStoryBoardOff ( 4 );
            tabview . Btn1 . FontSize = 14;
            tabview . Btn1 . Content = "Bank";
            tabview . Btn1 . Foreground = FindResource ( "Black0" ) as SolidColorBrush;

        }

        private void Btn2_MouseEnter ( object sender , MouseEventArgs e ) {
            Tabview . TriggerStoryBoardOn ( 4 );
            tabview . Btn2 . FontSize = 18;
            tabview . Btn2 . Content = "LOAD";
            tabview . Btn2 . Foreground = FindResource ( "Green5" ) as SolidColorBrush;
            tabview . Btn2 . ToolTip = "Load/Reload Customers Account Data";

        }
        private void Btn2_MouseLeave ( object sender , MouseEventArgs e ) {
            Tabview . TriggerStoryBoardOn ( 4 );
            tabview . Btn2 . FontSize = 14;
            tabview . Btn2 . Content = "Customer";
            tabview . Btn2 . Foreground = FindResource ( "Black0" ) as SolidColorBrush;
            tabview . Btn2 . ToolTip = "Load/Reload Customers Account Data";

        }

        #region FlowDoc support
        /// <summary>
        ///  These are the only methods any window needs to provide support for my FlowDoc system.

        protected void MaximizeFlowDoc ( object sender , EventArgs e ) {
            // Clever "Hook" method that Allows the flowdoc to be resized to fill window
            // or return to its original size and position courtesy of the Event declard in FlowDoc
            fdl . MaximizeFlowDoc ( Flowdoc , canvas , e );
        }
        private void Flowdoc_MouseLeftButtonUp ( object sender , MouseButtonEventArgs e ) {
            // Window wide  !!
            // Called  when a Flowdoc MOVE has ended
            MovingObject = fdl . Flowdoc_MouseLeftButtonUp ( sender , Flowdoc , MovingObject , e );
            ReleaseMouseCapture ( );
        }
        // CALLED WHEN  LEFT BUTTON PRESSED
        private void Flowdoc_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e ) {
            //In this event, we get current mouse position on the control to use it in the MouseMove event.
            MovingObject = fdl . Flowdoc_PreviewMouseLeftButtonDown ( sender , Flowdoc , e );
            //Debug . WriteLine ( $"MvvmDataGrid Btn down {MovingObject}" );
        }
        private void Flowdoc_MouseMove ( object sender , MouseEventArgs e ) {
            // We are Resizing the Flowdoc using the mouse on the border  (Border.Name=FdBorder)
            fdl . Flowdoc_MouseMove ( Flowdoc , canvas , MovingObject , e );
        }
        // Shortened version proxy call		
        private void Flowdoc_LostFocus ( object sender , RoutedEventArgs e ) {
            Flowdoc . BorderClicked = false;
        }
        public void FlowDoc_ExecuteFlowDocBorderMethod ( object sender , EventArgs e ) {
            // EVENTHANDLER to Handle resizing
            FlowDoc fd = sender as FlowDoc;
            Point pt = Mouse . GetPosition ( canvas );
            double dLeft = pt . X;
            double dTop = pt . Y;
        }
        public void fdmsg ( string line1 , string line2 = "" , string line3 = "" ) {
            //We have to pass the Flowdoc.Name, and Canvas.Name as well as up   to 3 strings of message
            //  you can  just provie one if required
            // eg fdmsg("message text");
            canvas . Visibility = Visibility . Visible;
            fdl . FdMsg ( Flowdoc , canvas , line1 , line2 , line3 );
        }


        private void LvFlowdoc_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e ) {
            //In this event, we get current mouse position on the control to use it in the MouseMove event.
            MovingObject = fdl . Flowdoc_PreviewMouseLeftButtonDown ( sender , Flowdoc , e );
        }
        #endregion Flowdoc support via library

        #region  Doesnt help datagrid going into edit mode
        private void TabControl_PreviewMouseDown ( object sender , MouseButtonEventArgs e ) {
            //if ( IsUnderTabHeader ( e . OriginalSource as DependencyObject ) )
            //    CommitTables ( Tabctrl );
        }
        private bool IsUnderTabHeader ( DependencyObject control ) {
            //if ( control is TabItem )
            //    return true;
            //DependencyObject parent = VisualTreeHelper . GetParent ( control );
            //if ( parent == null ) return false;
            //return IsUnderTabHeader ( parent );
            return false;
        }
        private void CommitTables ( DependencyObject control ) {
            if ( control is DataGrid ) {
                DataGrid grid = control as DataGrid;
                grid . CommitEdit ( DataGridEditingUnit . Row , true );
                return;
            }
            int childrenCount = VisualTreeHelper . GetChildrenCount ( control );
            for ( int childIndex = 0 ; childIndex < childrenCount ; childIndex++ )
                CommitTables ( VisualTreeHelper . GetChild ( control , childIndex ) );
        }
        #endregion  Doesnt help

        #region Serialization
        private void Serialize_LvUserControl ( object sender , RoutedEventArgs e ) {
            LvUserControl . WriteSerializedObject ( );

        }

        private void Serialize_LbUserControl ( object sender , RoutedEventArgs e ) {
            LbUserControl . WriteSerializedObject ( );

        }

        private void Serialize_DgUserControl ( object sender , RoutedEventArgs e ) {
            Tabview . Tabcntrl . dgUserctrl . WriteSerializedObject ( );
        }

        private void DeSerialize_DgUserControl ( object sender , RoutedEventArgs e ) {

        }
        #endregion Serializatio

        public void LoadTemplates ( ) {
            DataTemplatesBank . Add ( "BankDataTemplate1" );
            DataTemplatesBank . Add ( "BankDataTemplate2" );
            DataTemplatesBank . Add ( "BankDataTemplateComplex" );
            DataTemplatesCust . Add ( "CustomersDbTemplate1" );
            DataTemplatesCust . Add ( "CustomersDbTemplate2" );
            DataTemplatesCust . Add ( "CustomersDbTemplateComplex" );
            DataTemplatesGen . Add ( "GenericTemplate" );
            DataTemplatesGen . Add ( "GenericTemplate1" );
            DataTemplatesGen . Add ( "GenDataTemplate1" );
            DataTemplatesGen . Add ( "GenDataTemplate2" );
        }
        public void CreateControlStructs ( ) {
            DtTemplates . TemplatesCombo = Combobox;
            DtTemplates . TemplateIndexDg = 0;
            DtTemplates . TemplateIndexLb = 0;
            DtTemplates . TemplateIndexLv = 0;
            DtTemplates . TemplateListDg = "BANK";
            DtTemplates . TemplateListLb = "BANK";
            DtTemplates . TemplateListLv = "BANK";
            DtTemplates . TemplatesCombo = TemplatesCb;
            Tabview . Tabcntrl . CurrentTypeDg = "BANK";
            Tabview . Tabcntrl . CurrentTypeLb = "BANK";
            Tabview . Tabcntrl . CurrentTypeLv = "BANK";
            Tabview . Tabcntrl . CurrentTabName = "DgridTab";
            Tabview . Tabcntrl . dgUserctrl = new DgUserControl ( );
            Tabview . Tabcntrl . lbUserctrl = new LbUserControl ( );
            Tabview . Tabcntrl . lvUserctrl = new LvUserControl ( );
            Tabview . Tabcntrl . DtTemplates = DtTemplates;
            Tabview . Tabcntrl . twVModel = ControllerVm;
        }

        private void TemplatesCb_SelectionChanged ( object sender , SelectionChangedEventArgs e ) {
            // called to Switch between templates as tabs are swtched or loaded
            ComboBox cb = Tabview . Tabcntrl . DtTemplates . TemplatesCombo;
            if ( cb == null || cb . SelectedValue == null ) return;
            //save our data - find out what the current item is ?
            Type type = Tabview . Tabcntrl . ActiveControlType . GetType ( );

            if ( type == typeof ( DgUserControl ) ) {
                Tabview . Tabcntrl . ActiveControlType = Tabview . Tabcntrl . dgUserctrl;
                Tabview . Tabcntrl . DtTemplates . TemplateIndexDg = cb . SelectedIndex;
                DataTemplate dtemp = new DataTemplate ( );
                dtemp . Seal ( );
                FrameworkElement elemnt = Tabview . Tabcntrl . dgUserctrl . grid1 as FrameworkElement;
                dtemp = elemnt . FindResource ( e . AddedItems [ 0 ] . ToString ( ) ) as DataTemplate;
                Tabview . Tabcntrl . dgUserctrl . grid1 . ItemTemplate = dtemp;
                Tabview . Tabcntrl . dgUserctrl . grid1 . UpdateLayout ( );
                //Debug . WriteLine ( $"swiitched DgUserctrl ItemTemplate to  {e . AddedItems [ 0 ] . ToString ( )}" );
            }
            else if ( type == typeof ( LbUserControl ) ) {
                Tabview . Tabcntrl . ActiveControlType = Tabview . Tabcntrl . lbUserctrl;
                Tabview . Tabcntrl . DtTemplates . TemplateIndexLb = cb . SelectedIndex;
                FrameworkElement elemnt = Tabview . Tabcntrl . lbUserctrl . listbox1 as FrameworkElement;
                DataTemplate dtemp = new DataTemplate ( );
                dtemp . Seal ( );
                dtemp = elemnt . FindResource ( e . AddedItems [ 0 ] . ToString ( ) ) as DataTemplate;
                Tabview . Tabcntrl . lbUserctrl . listbox1 . ItemTemplate = dtemp;
                Tabview . Tabcntrl . lbUserctrl . listbox1 . UpdateLayout ( );
            }
            else if ( type == typeof ( LvUserControl ) ) {
                if ( Tabview . Tabcntrl . lvUserctrl != null && Tabview . Tabcntrl . tabItem . Content . GetType ( ) == typeof ( LvUserControl ) ) {
                    Tabview . Tabcntrl . ActiveControlType = Tabview . Tabcntrl . lvUserctrl;
                    Tabview . Tabcntrl . DtTemplates . TemplateIndexLv = cb . SelectedIndex;
                    FrameworkElement elemnt = Tabview . Tabcntrl . lvUserctrl . listview1 as FrameworkElement;
                    DataTemplate dtemp = new DataTemplate ( );
                    dtemp . Seal ( );
                    dtemp = elemnt . FindResource ( e . AddedItems [ 0 ] . ToString ( ) ) as DataTemplate;
                    Tabview . Tabcntrl . lvUserctrl . listview1 . ItemTemplate = dtemp;
                    Tabview . Tabcntrl . lvUserctrl . listview1 . UpdateLayout ( );
                }
            }
            TemplatesCb . UpdateLayout ( );
        }

        private void Btn6_MouseEnter ( object sender , MouseEventArgs e ) {

        }

        private void Btn6_MouseLeave ( object sender , MouseEventArgs e ) {

        }
    }
}