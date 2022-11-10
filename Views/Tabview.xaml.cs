using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Data;
using System . Diagnostics;
using System . Text;
using System . Threading;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Media . Animation;
using System . Windows . Media . Effects;
using System . Windows . Threading;

using NewWpfDev . Converts;
using NewWpfDev . Models;
using NewWpfDev . UserControls;
using NewWpfDev . ViewModels;



using static NewWpfDev . Views . Tabview;

namespace NewWpfDev . Views {

    public partial class Tabview : Window {

        #region NotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged ( string propertyName ) {
            if ( PropertyChanged != null ) {
                PropertyChanged ( this , new PropertyChangedEventArgs ( propertyName ) );
            }
        }
        #endregion OnPropertyChanged

//        public static dynamic Tctrl;

        #region Tab CONTROL STRUCTURES
        public struct DataTemplates {
            // A sole instance of this Structure is contained as DtTemplates in TabController struct - as Tabcntrl
            public ComboBox TemplatesCombo { get; set; }
            public int TemplateIndexDg { get; set; }
            public int TemplateIndexLb { get; set; }
            public int TemplateIndexLv { get; set; }
            public string TemplateNameDg { get; set; }
            public string TemplateNameLb { get; set; }
            public string TemplateNameLv { get; set; }
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
            public LogUserControl lgUserctrl { get; set; }
            public TvUserControl tvUserctrl { get; set; }
            public string CurrentTypeDg { get; set; }
            public string CurrentTypeLb { get; set; }
            public string CurrentTypeLv { get; set; }
            public string CurrentTabName { get; set; }
            public string DbNameDg { get; set; }
            public string DbNameLb { get; set; }
            public string DbNameLv { get; set; }
            public int DbNameIndexDg { get; set; }
            public int DbNameIndexLb { get; set; }
            public int DbNameIndexLv { get; set; }
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

        // Serializes any Observablecollection correctly to JSON file
        //            SerializeTestBank ( ); // works  well

        #region Declarations

        // Critical objects variable
        //public static dynamic CtrlPtr;
        //public static dynamic Infopanelptr;

        public bool IsLoading { get; set; } = true;
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
        public FlowdocLib fdl;


        #region general properties
        public static object MovingObject {
            get; set;
        }
        private CancellationTokenSource currentCancellationSource;
        private int msgcounter { get; set; } = 1;
        public static Tabview tabvw { get; set; }
        public static TabItem tabitem { get; set; }
        public static TabWinViewModel ControllerVm { get; set; }
        public static TabControl currenttab { get; set; }

        private int bankindex;
        public int BankIndex {
            get => bankindex;
            set { bankindex = value; NotifyPropertyChanged ( nameof ( BankIndex ) ); }
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
            get {return ( bool ) GetValue ( ViewersLinkedProperty );}
            set {SetValue ( ViewersLinkedProperty , value );}
        }
        public static readonly DependencyProperty ViewersLinkedProperty =
            DependencyProperty . Register ( "ViewersLinked" , typeof ( bool ) ,
                typeof ( Tabview ) , new PropertyMetadata ( ( bool ) false ) );

        public string DbType {
            get {return ( string ) GetValue ( DbTypeProperty );}
            set {SetValue ( DbTypeProperty , value );
                NotifyPropertyChanged ( nameof ( DbType ) );
                Tabview . SetDbType ( value );
            }
        }
        public static readonly DependencyProperty DbTypeProperty =
            DependencyProperty . Register ( "DbType" , typeof ( string ) , typeof ( Tabview ) , new PropertyMetadata ( "BANK" ) );

        public Tabview TabViewWin {// DP TABVIEWWIN
            get {return ( Tabview ) GetValue ( TabViewWinProperty );}
            set {SetValue ( TabViewWinProperty , value );}
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

        // public string TabviewInfoString     {
        //     get { return ( string  ) GetValue ( TabviewInfoStringProperty ); }
        //     set { SetValue ( TabviewInfoStringProperty , value ); }
        // }
        //public static readonly DependencyProperty TabviewInfoStringProperty =
        //     DependencyProperty . Register ( "TabviewInfoString" , typeof ( string  ) , typeof ( Tabview) , new PropertyMetadata ( "" ) );


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

        // public static string TabviewInfoString    ( DependencyObject obj ) {
        //     return ( string)  obj . GetValue ( TabviewInfoStringProperty );
        // }
        // public static void SetMyProperty ( DependencyObject obj , string  value ) {
        //     obj . SetValue ( TabviewInfoStringProperty , value );
        // }
        //public static readonly DependencyProperty TabviewInfoStringProperty =
        //     DependencyProperty . RegisterAttached ( "TabviewInfoString" , typeof ( string  ) , typeof ( Tabview ) , new PropertyMetadata ( "" ) );


        #endregion Attached properties

        #endregion Declarations

        #region Startup
        public Tabview ( ) {
            Mouse . OverrideCursor = Cursors . Wait;
            tabvw = this;
            TabViewWin = this;
            //Tctrl = Tabcntrl;
            //Debug . WriteLine ($"{ Tctrl.GetType()}");
            //FlowdocLib fdl = new FlowdocLib ( this.Flowdoc , this.canvas );

            CreateControlStructs ( );
            InitializeComponent ( );
            LoadDbTables ( null );
            Combobox = this . TemplatesCb;
            Tabview . Tabcntrl . tabControl = this . Tabctrl;
            Tabview . Tabcntrl . tabView = this;
            Tabview . Tabcntrl . tabItem = Tabview . Tabcntrl . tabControl . Items [ 0 ] as TabItem;
            Tabview . Tabcntrl . DtTemplates . TemplatesCombo = Combobox;

            this . Left = 50;
            this . Top = 100;
            SizeChanged += Tabview_SizeChanged;

            // Setup pointers In TabWinViewWindow
            ControllerVm = TabWinViewModel . SetPointer ( this , "DgridTab" );

            this . Show ( );
            this . DataContext = ControllerVm;
            TabWinViewModel . CurrentTabIndex = 0;
            if ( TabWinViewModel . Tabcontrol != null )
                currenttab = TabWinViewModel . Tabcontrol;
            UseTask . IsChecked = ControllerVm . USETASK;
            UseWorker . IsChecked = !ControllerVm . USETASK;
            LoadTemplates ( );
            //Maximize hook  +/- statements
            Flowdoc . ExecuteFlowDocMaxmizeMethod += new EventHandler ( MaximizeFlowDoc );
            FlowDoc . FlowDocClosed += Flowdoc_FlowDocClosed;
            EventControl . WindowMessage += InterWinComms_WindowMessage;
            TemplatesCb . SelectionChanged += TemplatesCb_SelectionChanged;
            DbnamesCb . SelectionChanged += DbNamesCb_SelectionChanged;
            TemplatesCb . UpdateLayout ( );
            //dgvm = new DatagridUserControlViewModel ( );
            Bvm = DatagridUserControlViewModel . Bvm;
            Cvm = DatagridUserControlViewModel . Cvm;
            Tabview . Tabcntrl . DtTemplates . TemplatesCombo . ItemsSource = DataTemplatesBank;
            Tabview . Tabcntrl . DtTemplates . TemplatesCombo . SelectedIndex = 0;
            Mouse . OverrideCursor = Cursors . Arrow;
            Utils . SetupWindowDrag ( this );
                // How to use "Public Static Dynamic" pointer to access Infopanel TextBlock Text from anywhere in Tabview
            LoadName . Text = "Tabview loaded successfully";
            IsLoading = false;

            Console . WriteLine ("HOOOORAH, CONSOLE WRITELINE IS WORKING !!!!!!");
        }

        private void Window_Loaded ( object sender , RoutedEventArgs e )
        {
            Tabctrl . SelectedIndex = 0;
            FlowdocLib fdl = new FlowdocLib ( this . Flowdoc , this . canvas );
            TabWinViewModel . IsLoadingDb = false;

            Application . Current . Dispatcher . Invoke ( async ( ) =>
                ControllerVm . SetCurrentTab ( this , "DgridTab" )
            );
        }

        public static int FindDbName ( string dbname ) {
            int count = 0;
            foreach ( string item in Tabview . Tabcntrl . tabView . DbnamesCb . Items ) {
                if ( item . ToUpper ( ) == dbname . ToUpper ( ) )
                    break;
                count++;
            }
            return count;
        }
        private async void DbNamesCb_SelectionChanged ( object sender , SelectionChangedEventArgs e ) {
            if ( IsLoading ) return;
            string selitem = DbnamesCb . SelectedItem . ToString ( );
            if ( selitem . ToUpper ( ) == "BANKACCOUNT" ) {
                if ( Tabview . Tabcntrl . ActiveControlType . GetType ( ) == typeof ( DgUserControl ) ) {
                    Tabview . Tabcntrl . DtTemplates . TemplateNameDg = "BANKACCOUNT";
                    Tabview . Tabcntrl . CurrentTypeDg = "BANKACCOUNT";
                    Tabview . Tabcntrl . DbNameIndexDg = DbnamesCb . SelectedIndex;
                    Tabview . Tabcntrl . DbNameDg = DbnamesCb . SelectedItem . ToString ( ) . ToUpper ( );
                    // Allows Callee to update interface
                    Application . Current . Dispatcher . Invoke ( async ( ) =>
                        await Task . Run ( async ( ) => await Tabview . Tabcntrl . dgUserctrl . LoadBank ( ) )
                    );
                    //                  await Task . Run (async  ( ) => await Tabview . Tabcntrl . dgUserctrl . LoadBank ( ) );
                }
                else if ( Tabview . Tabcntrl . ActiveControlType . GetType ( ) == typeof ( LbUserControl ) ) {
                    Tabview . Tabcntrl . DtTemplates . TemplateNameLb = "BANKACCOUNT";
                    Tabview . Tabcntrl . CurrentTypeLb = "BANKACCOUNT";
                    Tabview . Tabcntrl . DbNameIndexLb = DbnamesCb . SelectedIndex;
                    Tabview . Tabcntrl . DbNameLb = DbnamesCb . SelectedItem . ToString ( ) . ToUpper ( );
                    // Allows Callee to update interface
                    Application . Current . Dispatcher . Invoke ( async ( ) =>
                    // works well, & fast
                        await Task . Run ( async ( ) => await Tabview . Tabcntrl . lbUserctrl . LoadBank ( ) )
                    );
                }
                else if ( Tabview . Tabcntrl . ActiveControlType . GetType ( ) == typeof ( LvUserControl ) ) {
                    Tabview . Tabcntrl . DtTemplates . TemplateNameLv = "BANKACCOUNT";
                    Tabview . Tabcntrl . CurrentTypeLv = "BANKACCOUNT";
                    Tabview . Tabcntrl . DbNameIndexLv = DbnamesCb . SelectedIndex;
                    Tabview . Tabcntrl . DbNameLv = DbnamesCb . SelectedItem . ToString ( ) . ToUpper ( );
                    // Allows Callee to update interface
                    Application . Current . Dispatcher . Invoke ( async ( ) =>
                        await Task . Run ( async ( ) => await Tabview . Tabcntrl . lvUserctrl . LoadBank ( ) )
                    );
                    //                    Tabview . Tabcntrl . lvUserctrl . LoadBank ( );
                }
                Tabview . SetDbType ( "BANK" );
            }
            else if ( selitem . ToUpper ( ) == "CUSTOMER" ) {
                if ( Tabview . Tabcntrl . ActiveControlType . GetType ( ) == typeof ( DgUserControl ) ) {
                    Tabview . Tabcntrl . DtTemplates . TemplateNameDg = "CUSTOMER";
                    Tabview . Tabcntrl . CurrentTypeDg = "CUSTOMER";
                    Tabview . Tabcntrl . DbNameIndexDg = DbnamesCb . SelectedIndex;
                    Tabview . Tabcntrl . DbNameDg = DbnamesCb . SelectedItem . ToString ( ) . ToUpper ( );
                    Tabview . Tabcntrl . dgUserctrl . LoadCustomer ( );

                    //await Application . Current . Dispatcher . Invoke ( async ( ) => {
                    //    await Task . Run ( ( ) => Tabview . Tabcntrl . dgUserctrl . LoadCustomer ( ) );
                    //} );
                }
                else if ( Tabview . Tabcntrl . ActiveControlType . GetType ( ) == typeof ( LbUserControl ) ) {
                    Tabview . Tabcntrl . DtTemplates . TemplateNameLb = "CUSTOMER";
                    Tabview . Tabcntrl . CurrentTypeLb = "CUSTOMER";
                    Tabview . Tabcntrl . DbNameIndexLb = DbnamesCb . SelectedIndex;
                    Tabview . Tabcntrl . DbNameLb = Tabview . Tabcntrl . tabView . DbnamesCb . SelectedItem . ToString ( ) . ToUpper ( );
                    Tabview . Tabcntrl . lbUserctrl . LoadCustomer ( ) ;
//                    await Task . Run ( async ( ) => await Tabview . Tabcntrl . lbUserctrl . LoadCustomer ( ) );
                    //await Application . Current . Dispatcher . Invoke ( async ( ) => {
                    //    await Task . Run ( ( ) => Tabview . Tabcntrl . lbUserctrl . LoadCustomer ( ) );
                    //} );
                }
                else if ( Tabview . Tabcntrl . ActiveControlType . GetType ( ) == typeof ( LvUserControl ) ) {
                    Tabview . Tabcntrl . DtTemplates . TemplateNameLv = "CUSTOMER";
                    Tabview . Tabcntrl . CurrentTypeLv = "CUSTOMER";
                    Tabview . Tabcntrl . DbNameIndexLv = DbnamesCb . SelectedIndex;
                    Tabview . Tabcntrl . DbNameLv = Tabview . Tabcntrl . tabView . DbnamesCb . SelectedItem . ToString ( ) . ToUpper ( );
                    await Task . Run ( async ( ) => await Tabview . Tabcntrl . lvUserctrl . LoadCustomer ( ) );
                    //await Application . Current . Dispatcher . Invoke ( async ( ) => {
                    //    await Task . Run ( ( ) => Tabview . Tabcntrl . lvUserctrl . LoadCustomer ( ) );
                    //} );
                }
                Tabview . SetDbType ( "CUSTOMER" );
            }
            else
            {
                // use GenericClass
                if ( Tabview . Tabcntrl . ActiveControlType . GetType ( ) == typeof ( DgUserControl ) ) {
                    // a GENERIC table  has been selected in Datagrid
                    Tabview . Tabcntrl . CurrentTypeDg = "GEN";
                    Tabview . Tabcntrl . DbNameIndexDg = DbnamesCb . SelectedIndex;
                    Tabview . Tabcntrl . DtTemplates . TemplateNameDg = "GEN";
                    Tabview . Tabcntrl . DbNameDg = DbnamesCb . SelectedItem . ToString ( ) . ToUpper ( );
                    var Task = Tabview . Tabcntrl . dgUserctrl . LoadGeneric ( e . AddedItems [ 0 ] . ToString ( ) );
                }
                else if ( Tabview . Tabcntrl . ActiveControlType . GetType ( ) == typeof ( LbUserControl ) ) {
                    // a GENERIC table  has been selected in ListBox
                    Tabview . Tabcntrl . CurrentTypeLb = "GEN";
                    Tabview . Tabcntrl . DbNameIndexLb = DbnamesCb . SelectedIndex;
                    Tabview . Tabcntrl . DtTemplates . TemplateNameLb = DbnamesCb . SelectedItem . ToString ( ) . ToUpper ( );
                    Tabview . Tabcntrl . DtTemplates . TemplateNameLb = "GEN";
                    Tabview . Tabcntrl . DbNameLb = Tabview . Tabcntrl . tabView . DbnamesCb . SelectedItem . ToString ( ) . ToUpper ( );
                    int count = Tabview . Tabcntrl . lbUserctrl . LoadGeneric ( e . AddedItems [ 0 ] . ToString ( ) );
                    if ( count == 0 ) {
                        Debug . WriteLine ( $"No records returned for {DbnamesCb . SelectedItem . ToString ( ) . ToUpper ( )}\nso BankAccount will be (re)loaded  by default" );
                        MessageBox . Show ( $"the Db [{DbnamesCb . SelectedItem . ToString ( ) . ToUpper ( )}] returned ZERO records.\n\nTherefore the ListBox will now reload the \n\"default\" BankAccount Table for you. " , "Table requested returned No Data " );
                        // Reload Bankaccount as default
                        Tabview . Tabcntrl . lbUserctrl . LoadBank ( );
                    }
                }
                else if ( Tabview . Tabcntrl . ActiveControlType . GetType ( ) == typeof ( LvUserControl ) ) {
                    // a GENERIC table  has been selected in Listview
                    Tabview . Tabcntrl . CurrentTypeLv = "GEN";
                    Tabview . Tabcntrl . DbNameIndexLv = DbnamesCb . SelectedIndex;
                    Tabview . Tabcntrl . DtTemplates . TemplateNameLv = DbnamesCb . SelectedItem . ToString ( ) . ToUpper ( );
                    Tabview . Tabcntrl . DbNameLv = Tabview . Tabcntrl . tabView . DbnamesCb . SelectedItem . ToString ( ) . ToUpper ( );
                    // load sql data using spLoadTagbleAsGeneric Stored procedure (command created in method itself)
                    int count = Tabview . Tabcntrl . lvUserctrl . LoadGeneric ( e . AddedItems [ 0 ] . ToString ( ) );
                    if ( count == 0 ) {
                        Debug . WriteLine ( $"No records returned for {DbnamesCb . SelectedItem . ToString ( ) . ToUpper ( )}\nso BankAccount will be (re)loaded  by default" );
                        MessageBox . Show ( $"the Db [{DbnamesCb . SelectedItem . ToString ( ) . ToUpper ( )}] returned ZERO records.\n\nTherefore the ListView will now reload the \n\"default\" BankAccount Table for you. " , "Table requested returned No Data " );
                        // Reload Bankaccount as default
                        Thread . Sleep ( 250 );
                        Debug . WriteLine ( $"Reloading Bank Data because previous Generic type failed to load any records" );
                        Application . Current . Dispatcher . Invoke ( async ( ) =>
                            Task . Run ( ( ) => Tabview . Tabcntrl . lvUserctrl . LoadBank ( ) )
                            );
                    }
                }
                Tabview . SetDbType ( "GEN" );
            }
        }

        //Get list of all Tables in currently selected Db 
        public bool LoadDbTables ( string DbName ) {
            int listindex = 0, count = 0;
            List<string> list = new List<string> ( );
            DbName = DbName == null ? "Ian1" : DbName;
            DbName = DbName . ToUpper ( );
            if ( Utils . CheckResetDbConnection ( DbName , out string constr ) == false )
                Debug . WriteLine ( $"Failed to set connection string for {DbName} Db" );
            // All Db's have their own version of this SP.....
            string SqlCommand = "spGetTablesList";

            Datagrids . CallStoredProcedure ( list , SqlCommand );
            //This call returns us a DataTable
            DataTable dt = DataLoadControl . GetDataTable ( SqlCommand );
            // This how to access Row data from  a grid the easiest way.... parsed into a List <xxxxx>
            if ( dt != null ) {
                DbnamesCb . Items . Clear ( );
                list = WpfLib1 . Utils . GetDataDridRowsAsListOfStrings ( dt );
                if ( DbName == "NORTHWIND" ) {
                    foreach ( string row in list ) {
                        DbnamesCb . Items . Add ( row );
                        count++;
                    }
                }
                else if ( DbName == "IAN1" ) {
                    foreach ( string row in list ) {
                        DbnamesCb . Items . Add ( row );
                        if ( row . ToUpper ( ) == "BANKACCOUNT" )
                            BankIndex = count;
                        count++;
                    }
                    DbnamesCb . SelectedIndex = bankindex;
                }
                else if ( DbName == "PUBS" ) {
                    foreach ( string row in list ) {
                        DbnamesCb . Items . Add ( row );
                        count++;
                    }
                }
                DbnamesCb . SelectedIndex = listindex;
                if ( count > 0 )
                    return true;
                else
                    return false;
            }
            else {
                MessageBox . Show ( $"SQL comand {SqlCommand} Failed..." );
                WpfLib1 . Utils . DoErrorBeep ( 125 , 55 , 1 );
                return false;
            }
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
            if ( Tabview . Tabcntrl . tvUserctrl == null ) return;
            Tabview . Tabcntrl . tvUserctrl . Width = TabWinViewModel . Tabcontrol . ActualWidth;// - 5;
            Tabview . Tabcntrl . tvUserctrl . Height = TabWinViewModel . Tabcontrol . ActualHeight - 20;
            Thickness th = new Thickness ( 0 , 0 , 0 , 0 );
            th = Tabview . Tabcntrl . tvUserctrl . Margin;
            th . Left = 0;
            th . Right = 10;
            th . Top = 0;
            Tabview . Tabcntrl . tvUserctrl . Margin = th;
            th = Tabview . Tabcntrl . tvUserctrl . treeview1 . Margin;
            th . Left = 5;
            th . Top = 5;
            th . Right = 20;
            th . Bottom = 0;
            Tabview . Tabcntrl . tvUserctrl . treeview1 . Margin = th;
            Tabview . Tabcntrl . tvUserctrl . treeview1 . Height = Tabview . Tabcntrl . tvUserctrl . Height - 35;
            Tabview . Tabcntrl . tvUserctrl . treeview1 . Width = Tabview . Tabcntrl . tvUserctrl . Width - 25;
            Tabview . Tabcntrl . tvUserctrl . treeview1 . VerticalAlignment = VerticalAlignment . Top;
            Tabview . Tabcntrl . tvUserctrl . treeview1 . UpdateLayout ( );
        }

        #endregion resizing

        private void tabview_Closed ( object sender , EventArgs e ) {
            // cleanup FLowDoc before closing down
            Flowdoc . ExecuteFlowDocMaxmizeMethod -= new EventHandler ( MaximizeFlowDoc );
            FlowDoc . FlowDocClosed -= Flowdoc_FlowDocClosed;
            EventControl . WindowMessage -= InterWinComms_WindowMessage;
            TemplatesCb . SelectionChanged -= TemplatesCb_SelectionChanged;
            DbnamesCb . SelectionChanged -= DbNamesCb_SelectionChanged;
            FlowDoc . FlowDocClosed -= Flowdoc_FlowDocClosed;
            SizeChanged -= Tabview_SizeChanged;
            // Close App
            ControllerVm . Closedown ( );
        }

        #region Left Mouse Ckick on tabs Trigger Methods
        private void GridMouseLeftButtonDown ( object sender , MouseButtonEventArgs e ) {
            Application . Current . Dispatcher . Invoke ( ( ) => ControllerVm . SetCurrentTab ( this , "DgridTab" ) );
            // ControllerVm . SetCurrentTab ( this , "DgridTab" );
        }
        private async void ListboxMouseLeftButtonDown ( object sender , MouseButtonEventArgs e ) {
            Application . Current . Dispatcher . Invoke ( ( ) => ControllerVm . SetCurrentTab ( this , "ListboxTab" ) );
        }
        private async void ListviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e ) {
            Application . Current . Dispatcher . Invoke ( ( ) => ControllerVm . SetCurrentTab ( this , "ListviewTab" ) );
        }
        private async void LogviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e ) {
            Application . Current . Dispatcher . Invoke ( ( ) => ControllerVm . SetCurrentTab ( this , "LogviewTab" ) );
        }
        private async void TreeviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e ) {
            Application . Current . Dispatcher . Invoke ( ( ) => ControllerVm . SetCurrentTab ( this , "TreeviewTab" ) );
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
            }
            else if ( type . Name == "LbUserControl" ) {
                Tabview . Tabcntrl . lbUserctrl . listbox1 . ItemsSource = null;
                Tabview . Tabcntrl . lbUserctrl . listbox1 . Items . Clear ( );
                Tabview . Tabcntrl . lbUserctrl . UpdateLayout ( );
            }
            else if ( type . Name == "LvUserControl" ) {
                Tabview . Tabcntrl . lvUserctrl . listview1 . ItemsSource = null;
                Tabview . Tabcntrl . lvUserctrl . listview1 . Items . Clear ( );
                Tabview . Tabcntrl . lvUserctrl . UpdateLayout ( );
            }
        }
        private void clearTabs ( object sender , RoutedEventArgs e ) {
            if ( Tabview . Tabcntrl . lbUserctrl != null ) {
                Tabview . Tabcntrl . lbUserctrl . listbox1 . ItemsSource = null;
                Tabview . Tabcntrl . lbUserctrl . listbox1 . Items . Clear ( );
            }
            if ( Tabview . Tabcntrl . lvUserctrl != null ) {
                Tabview . Tabcntrl . lvUserctrl . listview1 . ItemsSource = null;
                Tabview . Tabcntrl . lvUserctrl . listview1 . Items . Clear ( );
            }
            if ( Tabview . Tabcntrl . dgUserctrl != null ) {
                Tabview . Tabcntrl . dgUserctrl . grid1 . ItemsSource = null;
                Tabview . Tabcntrl . dgUserctrl?.grid1 . Items . Clear ( );
            }
            if ( TabWinViewModel . logUserctrl != null ) {
                TabWinViewModel . logUserctrl?.logview . Items . Clear ( );
                TabWinViewModel . logUserctrl?.logview . UpdateLayout ( );
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
            Tabview . Tabcntrl . twVModel . USETASK = ( bool ) UseTask . IsChecked;
            CheckBox cb = sender as CheckBox;
            if ( cb . Name == "UseTask" ) {
                if ( ( bool ) cb . IsChecked == true ) {
                    Tabview . Tabcntrl . twVModel . USETASK = true;
                    UseWorker . IsChecked = false;
                    UseTask . IsChecked = true;
                }
                else {
                    Tabview . Tabcntrl . twVModel . USETASK = false;
                    UseWorker . IsChecked = true;
                    UseTask . IsChecked = false;
                }
            }
            else {
                if ( ( bool ) cb . IsChecked == true ) {
                    Tabview . Tabcntrl . twVModel . USETASK = false;
                    UseWorker . IsChecked = true;
                    UseTask . IsChecked = false;
                }
                else {
                    Tabview . Tabcntrl . twVModel . USETASK = true;
                    UseTask . IsChecked = true;
                    UseWorker . IsChecked = false;
                }
            }
            if ( Tabview . Tabcntrl . dgUserctrl != null )
                Tabview . Tabcntrl . dgUserctrl . grid1 . ItemsSource = null;
            if ( Tabview . Tabcntrl . lbUserctrl != null )
                Tabview . Tabcntrl . lbUserctrl . listbox1 . ItemsSource = null;
            if ( Tabview . Tabcntrl . lvUserctrl != null )
                Tabview . Tabcntrl . lvUserctrl . listview1 . ItemsSource = null;
            if ( Tabview . Tabcntrl . lgUserctrl != null )
                Tabview . Tabcntrl . lgUserctrl . logview . ItemsSource = null;
            if ( Tabview . Tabcntrl . tvUserctrl . treeview1 != null )
                Tabview . Tabcntrl . tvUserctrl . treeview1 . ItemsSource = null;
            if ( Tabview . Tabcntrl . dgUserctrl . grid1 != null )
                Tabview . Tabcntrl . dgUserctrl . grid1 . Items . Clear ( );
            if ( Tabview . Tabcntrl . lbUserctrl . listbox1 != null )
                Tabview . Tabcntrl . lbUserctrl . listbox1 . Items . Clear ( );
            if ( Tabview . Tabcntrl . lvUserctrl . listview1 != null )
                Tabview . Tabcntrl . lvUserctrl . listview1 . Items . Clear ( );
            if ( Tabview . Tabcntrl . lgUserctrl . logview != null )
                Tabview . Tabcntrl . lgUserctrl . logview . Items . Clear ( );

            clearTabs ( this , null );
        }

        #region Remote triggers  for mouseover events
        public static void TriggerStoryBoardOn ( int Id ) {
            Storyboard sb;

            sb = Id  switch
            {
               1  => tabvw . FindResource ( "TabAnimationOn1" ) as Storyboard,
                2 => tabvw . FindResource ( "TabAnimationOn2" ) as Storyboard,
                3 => tabvw . FindResource ( "TabAnimationOn3" ) as Storyboard,
                4 => tabvw . FindResource ( "TabAnimationOn4" ) as Storyboard,
                5 => tabvw . FindResource ( "TabAnimationOn5" ) as Storyboard,
                _ => null
            };
            if(sb != null) sb . Begin ( );
            return;

            switch ( Id ) {
            //    case 1:
            //        sb = tabvw . FindResource ( "TabAnimationOn1" ) as Storyboard;
            //        sb . Begin ( );
            //        break;
            //    case 2:
            //        sb = tabvw . FindResource ( "TabAnimationOn2" ) as Storyboard;
            //        sb . Begin ( );
            //        break;
            //    case 3:
            //        sb = tabvw . FindResource ( "TabAnimationOn3" ) as Storyboard;
            //        sb . Begin ( );
            //        break;
            //    case 4:
            //        sb = tabvw . FindResource ( "TabAnimationOn4" ) as Storyboard;
            //        sb . Begin ( );
            //        break;
            //    case 5:
            //        sb = tabvw . FindResource ( "TabAnimationOn5" ) as Storyboard;
            //        sb . Begin ( );
            //        break;
            }
        }
        public static void TriggerStoryBoardOff ( int Id ) {
            Storyboard sb;

            sb = Id switch
            {
                1 => tabvw . FindResource ( "TabAnimationOff1" ) as Storyboard,
                2 => tabvw . FindResource ( "TabAnimationOff2" ) as Storyboard,
                3 => tabvw . FindResource ( "TabAnimationOff3" ) as Storyboard,
                4 => tabvw . FindResource ( "TabAnimationOff4" ) as Storyboard,
                5 => tabvw . FindResource ( "TabAnimationOff5" ) as Storyboard,
                _ => null
            };
            if ( sb != null ) sb . Begin ( );
            return;

            switch ( Id ) {
                //case 1:
                //    sb = tabvw . FindResource ( "TabAnimationOff1" ) as Storyboard;
                //    sb . Begin ( );
                //    break;
                //case 2:
                //    sb = tabvw . FindResource ( "TabAnimationOff2" ) as Storyboard;
                //    sb . Begin ( );
                //    break;
                //case 3:
                //    sb = tabvw . FindResource ( "TabAnimationOff3" ) as Storyboard;
                //    sb . Begin ( );
                //    break;
                //case 4:
                //    sb = tabvw . FindResource ( "TabAnimationOff4" ) as Storyboard;
                //    sb . Begin ( );
                //    break;
                //case 5:
                //    sb = tabvw . FindResource ( "TabAnimationOff5" ) as Storyboard;
                //    sb . Begin ( );
                //    break;
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
        }
        private bool IsUnderTabHeader ( DependencyObject control ) {
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
         private string SerializeTestBank ( ) {
            //This Works !!!! - XAML file created from Db data as a collection
            //creates a JSON output file as JSONTEXT.json of entire BANKACCOUNT Db contents
            if ( DatagridUserControlViewModel . Bvm != null && DatagridUserControlViewModel . Bvm . Count > 0 ) {
                WpfLib1 . Utils . WriteSerializedCollectionJSON ( DatagridUserControlViewModel . Bvm , @"C:\users\ianch\BankAccountCollection.json" );
                // Read it back in to check....
                string str = WpfLib1 . Utils . ReadSerializedCollectionJson ( @"C:\users\ianch\BankAccountCollection.json" );
                // returns as a string formatted as "xxxxx,yyyy\n" so we need to convert it back to our collection type
                // Thiis creates a new Bvm correctly from our JSON output
                Bvm = Utils . CreateBankAccountFromJson ( str );
                return str;
            }
            return "";
        }

        #endregion Serialization

        public void LoadTemplates ( ) {
            DataTemplatesBank . Clear ( );
            DataTemplatesCust . Clear ( );
            DataTemplatesGen . Clear ( );
            DataTemplatesBank . Add ( "BankDataTemplate1" );
            DataTemplatesBank . Add ( "BankDataTemplate2" );
            DataTemplatesBank . Add ( "BankDataTemplateComplex" );
            DataTemplatesCust . Add ( "CustomersDbTemplate1" );
            DataTemplatesCust . Add ( "CustomersDbTemplate2" );
            DataTemplatesCust . Add ( "CustomersDbTemplateComplex" );
            DataTemplatesGen . Add ( "GenericTemplate" );
            DataTemplatesGen . Add ( "GenDataTemplate1" );
            DataTemplatesGen . Add ( "GenDataTemplate2" );
            DataTemplatesGen . Add ( "GenDataTemplateReversed" );
        }
        public void CreateControlStructs ( ) {
            DtTemplates . TemplatesCombo = Combobox;
            DtTemplates . TemplateIndexDg = 0;
            DtTemplates . TemplateIndexLb = 0;
            DtTemplates . TemplateIndexLv = 0;
            DtTemplates . TemplateNameDg = "BANKACCOUNT";
            DtTemplates . TemplateNameLb = "BANKACCOUNT";
            DtTemplates . TemplateNameLv = "BANKACCOUNT";
            DtTemplates . TemplatesCombo = TemplatesCb;
            Tabview . Tabcntrl . CurrentTypeDg = "BANKACCOUNT";
            Tabview . Tabcntrl . CurrentTypeLb = "BANKACCOUNT";
            Tabview . Tabcntrl . CurrentTypeLv = "BANKACCOUNT";
            Tabview . Tabcntrl . CurrentTabName = "DgridTab";
            Tabview . Tabcntrl . dgUserctrl = new DgUserControl ( );
            Tabview . Tabcntrl . lbUserctrl = new LbUserControl ( );
            Tabview . Tabcntrl . lvUserctrl = new LvUserControl ( );
            Tabview . Tabcntrl . lgUserctrl = new LogUserControl ( );
            Tabview . Tabcntrl . tvUserctrl = new TvUserControl ( );
            Tabview . Tabcntrl . DtTemplates = DtTemplates;
            Tabview . Tabcntrl . twVModel = ControllerVm;
            Console . WriteLine ("gfdfs");
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
                string dtemplate = TemplatesCb . SelectedItem . ToString ( );
                FrameworkElement elemnt = Tabview . Tabcntrl . dgUserctrl . grid1 as FrameworkElement;
                var el =  elemnt . FindResource ( dtemplate ) as DataTemplate;
                 if(el != null)
                    dtemp = elemnt . FindResource ( dtemplate ) as DataTemplate;
                Tabview . Tabcntrl . dgUserctrl . grid1 . ItemTemplate = dtemp;
                Tabview . Tabcntrl . dgUserctrl . grid1 . UpdateLayout ( );
            }
            else if ( type == typeof ( LbUserControl ) ) {
                Tabview . Tabcntrl . ActiveControlType = Tabview . Tabcntrl . lbUserctrl;
                Tabview . Tabcntrl . DtTemplates . TemplateIndexLb = cb . SelectedIndex;
                DataTemplate dtemp = new DataTemplate ( );
                dtemp . Seal ( );
                FrameworkElement elemnt = Tabview . Tabcntrl . lbUserctrl . listbox1 as FrameworkElement;
                dtemp = elemnt . FindResource ( e . AddedItems [ 0 ] . ToString ( ) ) as DataTemplate;
                Tabview . Tabcntrl . lbUserctrl . listbox1 . ItemTemplate = dtemp;
                Tabview . Tabcntrl . lbUserctrl . listbox1 . UpdateLayout ( );
            }
            else if ( type == typeof ( LvUserControl ) ) {
                //if ( Tabview . Tabcntrl . lvUserctrl != null && Tabview . Tabcntrl . tabItem . Content . GetType ( ) == typeof ( LvUserControl ) ) {
                Tabview . Tabcntrl . ActiveControlType = Tabview . Tabcntrl . lvUserctrl;
                Tabview . Tabcntrl . DtTemplates . TemplateIndexLv = cb . SelectedIndex;
                DataTemplate dtemp = new DataTemplate ( );
                dtemp . Seal ( );
                FrameworkElement elemnt = Tabview . Tabcntrl . lvUserctrl . listview1 as FrameworkElement;
                dtemp = elemnt . FindResource ( e . AddedItems [ 0 ] . ToString ( ) ) as DataTemplate;
                Tabview . Tabcntrl . lvUserctrl . listview1 . ItemTemplate = dtemp;
                Tabview . Tabcntrl . lvUserctrl . listview1 . UpdateLayout ( );
                // }
            }
            TemplatesCb . UpdateLayout ( );
        }

        private void Btn6_MouseEnter ( object sender , MouseEventArgs e ) {
        }

        private void Btn6_MouseLeave ( object sender , MouseEventArgs e ) {
        }

        private void Magnifyplus2 ( object sender , RoutedEventArgs e ) {
            if ( Tabview . Tabcntrl . ActiveControlType . GetType ( ) == typeof ( DgUserControl ) ) {
                Tabview . Tabcntrl . dgUserctrl . grid1 . FontSize += 2;
                Tabview . Tabcntrl . dgUserctrl . UpdateLayout ( );
            }
            else if ( Tabview . Tabcntrl . ActiveControlType . GetType ( ) == typeof ( LbUserControl ) ) {
                // This will NOT WORK if the listbox has an ItemContainerStyle that specifies a Fontsize in it,
                // otherwise it does work
                // set Listbox font size
                Tabview . Tabcntrl . lbUserctrl . listbox1 . FontSize += 2;
                Tabview . Tabcntrl . lbUserctrl . listbox1 . UpdateLayout ( );
            }
            else if ( Tabview . Tabcntrl . ActiveControlType . GetType ( ) == typeof ( LvUserControl ) ) {
                Tabview . Tabcntrl . lvUserctrl . listview1 . FontSize += 2;
                Tabview . Tabcntrl . tabView . ListviewTab . FontSize += 2;
                Tabview . Tabcntrl . lvUserctrl . UpdateLayout ( );
            }
            else if ( Tabview . Tabcntrl . ActiveControlType . GetType ( ) == typeof ( LogUserControl ) ) {
                //Tabview . Tabcntrl . lgUserctrl . SetFontSize ( ( int ) Tabview . Tabcntrl . lgUserctrl . logview . FontSize + 2 );
                Tabview . Tabcntrl . tabView . LogviewTab . FontSize += 2;
                Tabview . Tabcntrl . lgUserctrl . UpdateLayout ( );
            }
            else if ( Tabview . Tabcntrl . ActiveControlType . GetType ( ) == typeof ( TvUserControl ) ) {
                Tabview . Tabcntrl . tvUserctrl . Fontsize += 2;
                //Tabview . Tabcntrl . tvUserctrl . treeview1 . FontSize = Tabview . Tabcntrl . tvUserctrl . Fontsize;
                //tabview . TreeviewTab . FontSize += Tabview . Tabcntrl . tvUserctrl . Fontsize;
                tabview . TreeviewTab . UpdateLayout ( );
                Tabview . Tabcntrl . tvUserctrl . treeview1 . UpdateLayout ( );
            }
        }
        private void Magnifyminus2 ( object sender , RoutedEventArgs e ) {
            if ( Tabview . Tabcntrl . ActiveControlType . GetType ( ) == typeof ( DgUserControl ) ) {
                Tabview . Tabcntrl . dgUserctrl . grid1 . FontSize = Tabview . Tabcntrl . dgUserctrl . grid1 . FontSize - 2;
            }
            else if ( Tabview . Tabcntrl . ActiveControlType . GetType ( ) == typeof ( LbUserControl ) ) {
                Tabview . Tabcntrl . lbUserctrl . listbox1 . FontSize -= 2;
                var props = Tabview . Tabcntrl . lbUserctrl . listbox1 . GetType ( ) . GetProperties ( );
                //                props . SetValue ( item , operation , Item . PropertyType );
            }
            else if ( Tabview . Tabcntrl . ActiveControlType . GetType ( ) == typeof ( LvUserControl ) ) {
                Tabview . Tabcntrl . lvUserctrl . listview1 . FontSize -= 2;
            }
            else if ( Tabview . Tabcntrl . ActiveControlType . GetType ( ) == typeof ( LogUserControl ) ) {
                Tabview . Tabcntrl . lgUserctrl . logview . FontSize -= 2;
                tabview . LogviewTab . FontSize -= 2;
                Tabview . Tabcntrl . lgUserctrl . UpdateLayout ( );
            }
            else if ( Tabview . Tabcntrl . ActiveControlType . GetType ( ) == typeof ( TvUserControl ) ) {
                Tabview . Tabcntrl . tvUserctrl . treeview1 . FontSize -= 2;
                tabview . TreeviewTab . FontSize -= 2;
                Tabview . Tabcntrl . tvUserctrl . UpdateLayout ( );
            }
        }
    }
}