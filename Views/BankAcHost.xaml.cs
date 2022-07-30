#define SIZEING 
using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Diagnostics;
using System . Runtime . InteropServices;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;
using System . Windows . Media;

using DocumentFormat . OpenXml . Spreadsheet;

using GenericSqlLib . Models;

using NewWpfDev . Commands;
using NewWpfDev . Dapper;
using NewWpfDev . Dicts;
using NewWpfDev . Models;
using NewWpfDev . Sql;
using NewWpfDev . UserControls;
using NewWpfDev . ViewModels;

using ServiceStack;

namespace NewWpfDev . Views
{

    /// <summary>
    ///********************
    /// MVVM system
    ///********************
    /// Interaction logic for BankAcHost.xaml
    /// Processing is handled by BANKACCOUNTVM.CS
    /// </summary>

    public partial class BankAcHost : Window
    {

        //static public event EventHandler<ComboChangedArgs> ComboboxChanged;

        #region NotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged ( string propertyName )
        {
            if ( PropertyChanged != null )
            {
                PropertyChanged ( this , new PropertyChangedEventArgs ( propertyName ) );
            }
        }
        #endregion NotifyPropertyChanged

        public static ObservableCollection<GGenericClass> GGenCollection;
        public static ObservableCollection<ViewModels . GenericClass> GenCollection1 = new ObservableCollection<ViewModels . GenericClass> ( );
        public static ObservableCollection<ViewModels . GenericClass> GenCollection2 = new ObservableCollection<ViewModels . GenericClass> ( );
        public static ObservableCollection<ViewModels . GenericClass> BlankCollection = new ObservableCollection<ViewModels . GenericClass> ( );
        public static ViewModels . GenericClass GenClass = new ViewModels . GenericClass ( );
        public List<string> TablesList = new List<string> ( );
        public string SelectedTable { get; set; }
        public Size GenGrid1Size;
        public Size GenGrid2Size;
        public Size GenContentSize;


        #region properties
        //Singleton control class ?
        private static BankAcctVm BankAcctVm { get; set; }
        public static BankAcHost ThisWin { get; set; }
        public static BankAccountVM BankVm { get; set; }
        public static BankAccountInfo BankAcDetails { get; set; }
        public static BankAccountGrid BankAcctGrid { get; set; }
        public static BlankScreenUC BlankScreen { get; set; }
        public static GenericGridControl GenericGrid { get; set; }
        public static ComboboxPlus comboPlus { get; set; }
        public static TextBlock info { get; set; }
        public static DataGrid custgrid { get; set; }
        public static double ContentHeight { get; set; }
        public static double ContentWidth { get; set; }
        public static string CurrentPanel { get; set; }

        #endregion properties

        [DllImport ( "UXTheme.dll" , SetLastError = true , EntryPoint = "#138" )]
        public static extern bool ShouldSystemUseDarkMode ( );

        #region Full Properties

        private int Currentposition;
        public int currentposition
        {
            get { return Currentposition; }
            set { Currentposition = value; NotifyPropertyChanged ( nameof ( currentposition ) ); }
        }

        private string genericComboEntry;
        public string GenericComboEntry
        {
            get { return ( string ) genericComboEntry; }
            set { genericComboEntry = value; NotifyPropertyChanged ( nameof ( GenericComboEntry ) ); }
        }

        private string blankComboEntry;
        public string BlankComboEntry
        {
            get { return ( string ) blankComboEntry; }
            set { blankComboEntry = value; NotifyPropertyChanged ( nameof ( BlankComboEntry ) ); }
        }

        #endregion Full Properties

        public BankAcHost ( )
        {
            Mouse . SetCursor ( Cursors . Wait );
            InitializeComponent ( );
            bool bRet = ShouldSystemUseDarkMode ( );
            this . Title = "sdfafd";
            //this proves that only BooltoVisibilityConverter is in these resources
            var res = Application . Current . Resources;

            ComboboxPlus . ComboboxChanged += ComboboxPlus_ComboboxChanged;

            var style = res . Values;
            BankAcctVm = BankAcctVm . Instances;
            var viewmodel = new BankAccountVM ( );
            BankVm = viewmodel;
            viewmodel . GetHost ( );
            ThisWin = this;
            Currentposition = 0;
            info = Info;
            GenericGrid = new GenericGridControl ( this );
            //GenericGrid . UpdateLayout ( );
            combo . datagrids [ 0 ] = GenericGrid . datagrid1;
            combo . datagrids [ 1 ] = GenericGrid . datagrid2;
            BlankScreen = new BlankScreenUC ( );
            BankAcDetails = new BankAccountInfo ( );
            BankAcctGrid = new BankAccountGrid ( );
            comboPlus = new ComboboxPlus ( );

            // Create a generic handler to handle the ContentControl Buttons
            RoutedEventHandler handler = new RoutedEventHandler ( ContentController );
            FocusManager . AddGotFocusHandler ( this , handler );

            // Setting Doesnt "Stick" in ComboboxPlus !!!!
            comboPlus . SetHost ( ThisWin );
            BankAcctVm . DoClosePanel += BankAcctVm_DoClosePanel;
            custgrid = BankAccountGrid . datagrid;

            GenClass = new ViewModels . GenericClass ( );
            LoadDbTables ( );
            //loads the data using existing ICommand in BankAccountVM.CS
            viewmodel . SelectGrid . Execute ( "BANKACCOUNT" );
            // or use this  Extension method if you want to pass up to 3 args (as objects)
            object [ ] args = new object [ 3 ];
            args [ 0 ] = "GENERICGRID";
            //  viewmodel . SelectGrid . ExecuteCommand ( args );
            SetVisibility ( "GENERICGRID" , "GRID1" );
            Mouse . SetCursor ( Cursors . Arrow );
        }
        private async Task LoadDbTables ( )
        {//dummy caller for task to load list of Db Tables
            await DoLoadDbTablesAsync ( );
        }
        private async Task DoLoadDbTablesAsync ( )
        {   //  load list of Db Tables asynchronously
            List<string> TablesList = await Task . Run ( ( ) => GenericGridControl . GetDbTablesList ( "IAN1" )
              );
            combo . ItemsList = TablesList;
            comboPlus . Visibility = Visibility . Visible;
            return;
        }
        private void ComboboxPlus_ComboboxChanged ( object sender , ComboChangedArgs e )
        {
            int DbCount = 0, index = 0;
            string tablename = e . ActiveTablename . ToUpper ( );
            //************************************
            // GENERICGRID MUST BE  ACTIVE  
            //************************************
            if ( e . Activepanel == "GENERICGRID" )
            {
                index = 0;
                LoadGenericTable ( tablename , GenericGrid . datagrid1 );
                // load new data 
                //string curr = GenericGrid . Togglegrid . Content.ToString();
                if ( GenericGrid . Togglegrid . Content . ToString ( ) . Contains ( "Grid 1" ) )
                {
                    GenericGrid . datagrid1 . ItemsSource = null;
                    GenericGrid . datagrid1 . Items . Clear ( );

                    SqlServerCommands . LoadActiveRowsOnlyInGrid ( GenericGrid . datagrid1 , GenCollection1 , SqlServerCommands . GetGenericColumnCount ( GenCollection1 ) );
                    GenericDbUtilities . ReplaceDataGridFldNames ( tablename , ref GenericGrid . datagrid1 );
                    GenericGrid . datagrid1 . SelectedIndex = 0;
                    GenericGrid . datagrid1 . Refresh ( );
                    GenericGridControl . CurrentTable = tablename;
                    GenericGridControl . Title1 = tablename;
                    //                    ResetStyleSelection ( 1 );
                }
                else if ( GenericGrid . Togglegrid . Content . ToString ( ) . Contains ( "Grid 2" ) )
                {
                    GenericGrid . datagrid2 . ItemsSource = null;
                    GenericGrid . datagrid2 . Items . Clear ( );
                    SqlServerCommands .
                        LoadActiveRowsOnlyInGrid ( GenericGrid . datagrid2 , GenCollection2 , SqlServerCommands . GetGenericColumnCount ( GenCollection2 ) );
                    GenericDbUtilities . ReplaceDataGridFldNames ( tablename , ref GenericGrid . datagrid2 );
                    GenericGrid . datagrid2 . SelectedIndex = 0;
                    GenericGrid . datagrid2 . Refresh ( );
                    GenericGridControl . CurrentTable = tablename;
                    GenericGridControl . Title2 = tablename;
                    //                    ResetStyleSelection ( 2 );
                }
                // Setup new label and default table name
                GenericGrid . GenericTitle . Text = $"Table = {tablename . ToUpper ( )}";
            }
        }
        private void ResetStyleSelection ( int index )
        {
            if ( index == 1 )
                GenericGrid . StylesList . SelectedItem = GenericGridControl . Style1;
            else
                GenericGrid . StylesList . SelectedItem = GenericGridControl . Style2;
            GenericGrid . StylesList . Refresh ( );

        }

        static public bool LoadGenericTable ( string table , DataGrid grid )
        {
            int DbCount = 0;
            if ( grid . Name == "datagrid1" )
            {
                DbCount = LoadTableGeneric ( $"Select * from {table}" , ref GenCollection1 );
                if ( DbCount > 0 )
                    SqlServerCommands . LoadActiveRowsOnlyInGrid ( grid , GenCollection1 , SqlServerCommands . GetGenericColumnCount ( GenCollection1 ) );
            }
            else
            {
                DbCount = LoadTableGeneric ( $"Select * from {table}" , ref GenCollection2 );
                if ( DbCount > 0 )
                    SqlServerCommands . LoadActiveRowsOnlyInGrid ( grid , GenCollection2 , SqlServerCommands . GetGenericColumnCount ( GenCollection2 ) );
            }
            GenericDbUtilities . ReplaceDataGridFldNames ( table , ref grid );
            Debug . WriteLine ( $"grid has {grid . Items . Count} items from {table}" );
            return true;
        }

        #region Data Loading
        static private int LoadTableGeneric ( string SqlCommand , ref ObservableCollection<ViewModels . GenericClass> GenCollection )
        {
            List<string> list2 = new ( );
            GenCollection . Clear ( );
            string errormsg = "";
            int DbCount = 0;
            DbCount = DapperSupport . CreateGenericCollection (
            ref GenCollection ,
           SqlCommand ,
            "" , "" , "" ,
            ref list2 ,
            ref errormsg );
            return DbCount;
        }

        #endregion Data Loading

        //public void ChangeSkin ( Skin newSkin )
        //{
        //    Skin skin = newSkin;
        //    foreach ( ResourceDictionary dict in Resources . MergedDictionaries )
        //    {
        //        if ( dict is SkinResourceDictionary skinDict )
        //            skinDict . UpdateSource ( );
        //        else
        //            dict . Source = dict . Source;
        //    }
        //}

        static public void CenterGrid ( DataGrid grid )
        {
            Thickness th = new Thickness ( );
            double paddingtop = 130.0;
            double paddingleft = 10;
            // th = grid . Margin;
            double panelheight = ThisWin . ActualHeight - 130;
            double panelwidth = ThisWin . ActualWidth - 260;
            double gridheight = grid . ActualHeight;
            double gridwidth = grid . ActualWidth;
            Debug . WriteLine ( $"Entry margin {th . ToString ( )}" );
            Debug . WriteLine ( $"area {ThisWin . ActualHeight} / {ThisWin . ActualWidth}" );
            //            if ( gridheight < panelheight ) {
            if ( gridheight > panelheight )
            {
                // Grid HIGHER than panel height
                grid . Height = panelheight;
                th . Top = paddingtop;
                th . Left = paddingleft;
                grid . Margin = th;
                GenericGrid . datagrid1 . Refresh ( );
                GenericGrid . datagrid1 . UpdateLayout ( );
                th . Bottom = 0;
                grid . Margin = th;
                GenericGrid . datagrid1 . Refresh ( );
                GenericGrid . datagrid1 . UpdateLayout ( );
                GenericGrid . datagrid2 . Refresh ( );
                GenericGrid . datagrid2 . UpdateLayout ( );
            }
            else if ( panelheight >= gridheight )
            {
                //grid height less than panel height
                double diff = ( panelheight - gridheight );
                th . Top = paddingtop;
                th . Left = paddingleft;
                grid . Margin = th;
                GenericGrid . datagrid1 . Refresh ( );
                GenericGrid . datagrid1 . UpdateLayout ( );
                GenericGrid . datagrid2 . Refresh ( );
                GenericGrid . datagrid2 . UpdateLayout ( );
            }
        }

        public static Dictionary<string , string> GetColumnNames ( string tablename , out int count , string domain = "IAN1" )
        {
            int indx = 0;
            List<string> list = new List<string> ( );
            ObservableCollection<ViewModels . GenericClass> GenericClass = new ObservableCollection<ViewModels . GenericClass> ( );
            Dictionary<string , string> dict = new Dictionary<string , string> ( );
            // This returns a Dictionary<sting,string> PLUS a collection  and a List<string> passed by ref....
            List<int> VarCharLength = new List<int> ( );
            dict = GenericDbUtilities . GetDbTableColumns ( ref GenericClass , ref list , tablename , domain , ref VarCharLength );

            indx = 0;
            if ( VarCharLength . Count > 0 )
            {
                foreach ( var item in GenericClass )
                {
                    item . field3 = VarCharLength [ indx++ ] . ToString ( );
                }
            }
            count = indx - 1;
            return dict;
        }
        public static BankAcHost GetHost ( )
        {
            return ThisWin;
        }
        private void HostLoaded ( object sender , RoutedEventArgs e )
        {
            BankAccountInfo . SetHost ( this );
            BankAccountGrid . SetHost ( this );
            BlankScreenUC . SetHost ( this );
            BankAcctGrid . IsHost ( true );
            BankAcDetails . LoadCustomer ( );
            // SetActivePanel ( "GENERICGRID" );
            CurrentPanel = "GENERICGRID";
        }

        private void SetVisibility ( string newpanel , string arg = "" )
        {
            BankDetails . IsEnabled = true;
            updatebtn . IsEnabled = true;
            AllAccounts . IsEnabled = true;
            GenericBtn . IsEnabled = true;
            HidePanel . IsEnabled = true;
            BankAcctGrid . Visibility = Visibility . Collapsed;
            if ( BlankScreen != null ) BlankScreen . Visibility = Visibility . Collapsed;
            if ( BankAcDetails != null ) BankAcDetails . Visibility = Visibility . Collapsed;

            if ( newpanel == "BANKACCOUNTLIST" )
            {
                BankAcDetails . Visibility = Visibility . Visible;
                BankAcDetails . Refresh ( );
                this . Title = "BANK ACCOUNT DETAILS VIEWER";
                CurrentPanel = "BANKACCOUNTLIST";
                comboPlus . Promptlabel . Opacity = 0.3;
                combo . IsEnabled = false;
                combo . Opacity = 0.3;
                BankDetails . IsEnabled = false;
                updatebtn . IsEnabled = false;
                comboPlus . ComboSelection2 = comboPlus . SelectedItem?.ToString ( ) . ToUpper ( );
            }
            else if ( newpanel == "BANKACCOUNTGRID" )
            {
                BankAcctGrid . Visibility = Visibility . Visible;
                BankAcctGrid . Refresh ( );
                this . Title = "BANK ACCOUNTS GRID VIEWER";
                CurrentPanel = "BANKACCOUNTGRID";
                comboPlus . Promptlabel . Opacity = 0.3;
                combo . IsEnabled = false;
                combo . Opacity = 0.3;
                AllAccounts . IsEnabled = true;
            }
            else if ( newpanel == "GENERICGRID" )
            {
                GenericGrid . Visibility = Visibility . Visible;
                //GenericGrid . Refresh ( );
                this . Title = "GENERIC GRID VIEWER";
                CurrentPanel = "GENERICGRID";
                if ( GenCollection1 == null )
                {
                    GenCollection1 = new ObservableCollection<ViewModels . GenericClass> ( );

                    if ( GenCollection1 . Count == 0 )
                    {
                        combo . ComboSelection1 = "BANKACCOUNT";
                        combo . currentComboSelection = combo . ComboSelection1;
                        combo . gridtablenames [ 0 ] = "BANKACCOUNT";
                        // preload data if needed
                        if ( GenericGrid . datagrid1 . Items . Count == 0 )
                            GenericGrid . LoadGenericTable ( "BANKACCOUNT" );
                    }
                    comboPlus . ComboSelection1 = comboPlus . SelectedItem?.ToString ( ) . ToUpper ( );
                }
                else if ( GenCollection2 == null )
                {
                    GenCollection2 = new ObservableCollection<ViewModels . GenericClass> ( );
                    if ( GenCollection2 . Count == 0 )
                    {
                        combo . ComboSelection2 = "BANKACCOUNT";
                        combo . currentComboSelection = combo . ComboSelection2;
                        combo . gridtablenames [ 0 ] = "BANKACCOUNT";
                        // preload data if needed
                        if ( GenericGrid . datagrid2 . Items . Count == 0 )
                            GenericGrid . LoadGenericTable ( "BANKACCOUNT" );
                    }
                    comboPlus . ComboSelection2 = comboPlus . SelectedItem?.ToString ( ) . ToUpper ( );
                }
                comboPlus . Promptlabel . Opacity = 1.0;
                combo . IsEnabled = true;
                combo . Opacity = 1.0;
                GenericBtn . IsEnabled = true;
                BankVm . InfoText = "Use Combo at right to select any Db Table you want to view... ";
                //Default to Grid 1
                if ( (string)GenericGrid . Togglegrid . Content == "= Grid 1" )
                {
                    GenericGrid . datagrid2 . Visibility = Visibility . Collapsed;
                    GenericGrid . datagrid1 . Visibility = Visibility . Visible;
                }
                else
                {
                    GenericGrid . datagrid1 . Visibility = Visibility . Collapsed;
                    GenericGrid . datagrid2 . Visibility = Visibility . Visible;
                }
                //RoutedEventArgs args = new RoutedEventArgs ( );
                //args . Source = 1;
                GenericGrid . Togglegrid_Click ( null , null );
                GenericGrid . UpdateLayout ( );
            }
            else if ( newpanel == "BLANKSCREEN" )
            {
                this . Title = "BLANK GRID VIEWER";
                BlankScreen . Visibility = Visibility . Visible;
                CurrentPanel = "BLANKSCREEN";
                updatebtn . IsEnabled = false;
            }
            this . BankContent . Refresh ( );
            string name = newpanel == null ? "BlankPanel" : newpanel;
            //                Debug . WriteLine ( $"{name} set as Visible panel" );
        }

        #region ComboBoxPlus support
        private void SetComboSelection ( string panel , string setting , int index , bool def = false )
        {
            //Set ComboBox selected item (1/2/3) &/or binded default (ComboSelection)
            ComboboxPlus cbp = comboPlus as ComboboxPlus;
            cbp . CurrentPanel = panel;
            if ( setting == "" ) return;

            switch ( index )
            {
                case 0:
                    cbp . ComboSelection1 = setting;
                    cbp . currentComboSelection = setting;
                    break;
                case 1:
                    cbp . ComboSelection2 = setting;
                    cbp . currentComboSelection = setting;
                    break;
                //case 2:
                //    cbp . ComboSelection2 = setting;
                //    cbp . currentComboSelection = setting;
                //    break;
                //case 3:
                //    cbp . ComboSelection3 = setting;
                //    cbp . currentComboSelection = setting;
                //    break;
                //case 4:
                //    cbp . ComboSelection4 = setting;
                //    cbp . currentComboSelection = setting;
                //    break;
                //case 5:
                //    cbp . ComboSelection5 = setting;
                //    cbp . currentComboSelection = setting;
                //    break;
                default:
                    break;
            }
        }
        // Find currently selected table in combo and selexct it (without loosing the prompt)

        private void UpdateCombo ( string table )
        {
            int count = 0;
            ComboBox cb = this . combo . comboBox as ComboBox;
            foreach ( var item in cb . Items )
            {
                if ( item . ToString ( ) . ToUpper ( ) == table )
                {
                    cb . SelectedIndex = count;
                    break;
                }
                count++;
            }
        }
        #endregion ComboBoxPlus support

        public void SetActivePanel ( string newpanel )
        {
            GenContentSize . Height = BankContent . Height - 65;
            GenContentSize . Width = BankContent . Width - 220;
            GenGrid1Size . Height = GenContentSize . Height;
            GenGrid1Size . Width = GenContentSize . Width;
            GenGrid2Size . Height = GenContentSize . Height;
            GenGrid2Size . Width = GenContentSize . Width;

            SetVisibility ( newpanel );
            if ( newpanel == "BANKACCOUNTLIST" )
            {
                if ( BankAcDetails == null )
                    BankAcDetails = new BankAccountInfo ( );
                this . BankContent . Content = BankAcDetails;
                comboPlus . Promptlabel . Opacity = 0.3;
                combo . IsEnabled = false;
                combo . Opacity = 0.3;
            }
            else if ( newpanel == "BANKACCOUNTGRID" )
            {
                if ( BankAcctGrid == null )
                    BankAcctGrid = new BankAccountGrid ( );
                this . BankContent . Content = BankAcctGrid;
                comboPlus . Promptlabel . Opacity = 0.3;
                combo . IsEnabled = false;
                combo . Opacity = 0.3;
            }
            else if ( newpanel == "GENERICGRID" )
            {
                //var v = ShowColumnNames;
                this . BankContent . Content = GenericGrid;
                GenericGridControl . Host = this;
                if ( GenericGrid . datagrid1 . Items . Count == 0
                        || GenericGrid . datagrid2 . Items . Count == 0 )
                {
                    GenericGrid . LoadGenericTable ( "BankAccount" );
                    GenericGrid . GenericTitle . Text = "BANKACCOUNT";
                }
                if ( this . BankContent . Width != 0 && this . BankContent . Height != 0 )
                {
                    GenericGrid . datagrid1 . Width = BankContent . Width - 20;
                    GenericGrid . datagrid1 . Height = BankContent . Height - 80;
                    GenericGrid . datagrid2 . Width = BankContent . Width - 20;
                    GenericGrid . datagrid2 . Height = BankContent . Height - 80;
                }
                else
                {
                    GenericGrid . datagrid1 . Width = 675;// GenGrid1Size . Width;
                    GenericGrid . datagrid1 . Height = 300;// GenGrid1Size . Height;
                    GenericGrid . datagrid2 . Width = 750;// GenGrid2Size . Width;
                    GenericGrid . datagrid2 . Height = 300;// GenGrid2Size . Height;
                    GenericGridControl . ShowColumnNames = true;
                }
                GenericGrid . Refresh ( );
                this . BankContent . Refresh ( );
                comboPlus . Promptlabel . Opacity = 1.0;
                // Setup the banner title string
                SetGenGridTitleBar ( );
                //if ( GenericGrid . datagrid1 . Visibility == Visibility . Visible )
                //{
                //    if ( GenericGridControl . Title1 != "" )
                //        GenericGrid . GenericTitle . Text = GenericGridControl . Title1;
                //    else
                //        GenericGrid . GenericTitle . Text = "BANKACCOUNT";
                //}
                //else if ( GenericGrid . datagrid2 . Visibility == Visibility . Visible )
                //{
                //    if ( GenericGridControl . Title2 != "" )
                //        GenericGrid . GenericTitle . Text = GenericGridControl . Title2;
                //    else
                //        GenericGrid . GenericTitle . Text = "BANKACCOUNT";
                //}
                combo . IsEnabled = true;
                combo . Opacity = 1.0;
            }
            else if ( newpanel == "BLANKSCREEN" )
            {
                this . BankContent . Content = BlankScreen;
                this . BankContent . Refresh ( );
                comboPlus . Promptlabel . Opacity = 0.3;
                combo . IsEnabled = false;
                combo . Opacity = 0.3;
            }
            this . BankContent . Refresh ( );
        }
        public static void SetGenGridTitleBar ( )
        {
            if ( GenericGrid . datagrid1 . Visibility == Visibility . Visible )
            {
                if ( GenericGridControl . Title1 != "" )
                    GenericGrid . GenericTitle . Text = GenericGridControl . Title1;
                else
                    GenericGrid . GenericTitle . Text = "BANKACCOUNT";
            }
            else if ( GenericGrid . datagrid2 . Visibility == Visibility . Visible )
            {
                if ( GenericGridControl . Title2 != "" )
                    GenericGrid . GenericTitle . Text = GenericGridControl . Title2;
                else
                    GenericGrid . GenericTitle . Text = "BANKACCOUNT";
            }
        }
        private void UpdateBankRecord ( object sender , RoutedEventArgs e )
        {
            BankAccountViewModel bv = new BankAccountViewModel ( );
            CustomerViewModel cv = new CustomerViewModel ( );
            cv = BankAcDetails . backinggrid . SelectedItem as CustomerViewModel;
            SelchangedArgs args = new SelchangedArgs ( );
            args . cv = cv;
            bv . CustNo = BankAcDetails . custnumber . Text;
            bv . BankNo = BankAcDetails . banknumber . Text;
            bv . AcType = Convert . ToInt32 ( BankAcDetails . actype . Text );
            bv . Balance = Convert . ToDecimal ( BankAcDetails . balance . Text );
            bv . IntRate = Convert . ToDecimal ( BankAcDetails . intrate . Text );
            bv . ODate = Convert . ToDateTime ( BankAcDetails . odate . Text );
            bv . CDate = Convert . ToDateTime ( BankAcDetails . cdate . Text );
            cv . Addr1 = BankAcDetails . addr1 . Text;
            cv . Addr2 = BankAcDetails . addr2 . Text;
            cv . Town = BankAcDetails . town . Text;
            cv . County = BankAcDetails . county . Text;
            cv . PCode = BankAcDetails . pcode . Text;
            args . bv = bv;
            args . cv = cv;
            BankAcctVm . TriggerUpdate ( sender , args );

        }

        private void BankAcctVm_DoClosePanel ( object sender , SelchangedArgs args )
        {
            SetActivePanel ( "" );
        }

        public BankAcHost GetHostContext ( object sender )
        {
            if ( sender . GetType ( ) == typeof ( BankAccountInfo ) )
            {
                BankAcDetails = ( BankAccountInfo ) sender;
            }
            else if ( sender . GetType ( ) == typeof ( BankAccountGrid ) )
            {
                BankAcctGrid = ( BankAccountGrid ) sender;
            }
            return this;
        }
        public void ClosePanel ( object sender , string newpanel )
        {
            // called by all UC panels  to switch panes
            SetActivePanel ( newpanel );
            return;
        }

        private void DoClosePanel ( object sender , string newpanel )
        {
            // open specified panel
            SetActivePanel ( newpanel );
            return;
        }

        private void ClosePanel ( object sender )
        {
            if ( sender == null )
            {
                // hide all panes
                BankAcctGrid . Visibility = Visibility . Collapsed;
                BankAcDetails . Visibility = Visibility . Collapsed;
                BlankScreen = new BlankScreenUC ( );
                BlankScreen . Height = 800;
                BlankScreen . Width = 800;
                BankContent . Content = BlankScreen;
                ButtonPanel . Height = 800;
                ButtonPanel . Width = 800;
                ButtonPanel . Visibility = Visibility . Visible;
                BankContent . Refresh ( );
            }
            else if ( sender != null || sender . GetType ( ) == typeof ( BankAccountInfo ) )
            {
                // toggle Info visibility
                if ( BankAcDetails . Visibility == Visibility . Collapsed )
                    BankAcDetails . Visibility = Visibility . Visible;
                else
                    BankAcDetails . Visibility = Visibility . Collapsed;
            }
            else if ( sender != null || sender . GetType ( ) == typeof ( BankAccountGrid ) )
            {
                // toggle grid visibility
                if ( BankAcctGrid . Visibility == Visibility . Collapsed )
                    BankAcctGrid . Visibility = Visibility . Visible;
                else
                    BankAcctGrid . Visibility = Visibility . Collapsed;
            }
            //ButtonPanel . Visibility = Visibility . Collapsed;
            BankContent . Refresh ( );
        }
        private void Window_SizeChanged ( object sender , SizeChangedEventArgs e )
        {
            var size = e . NewSize;
            /// Allow for right and bottom margins
            this . BankContent . Height = size . Height - 120;
            this . BankContent . Width = size . Width - 200;
            BankAcctGrid . ResizeControl ( size . Height - 130 , size . Width - 230 );
            GenericGrid . ResizeControl ( size . Height - 130 , size . Width - 230 );
            GenericGrid . datagrid1 . Width = this . Width;
            GenericGrid . datagrid1 . Height = this . Height;
            //           BankAcDetails . ResizeControl ( size . Height , size . Width );
            Thickness th = new Thickness ( );
            th . Top = 0;
            th . Left = 0;
            BankAcDetails . Margin = th;
            GenericGrid . Margin = th;
            //            BlankScreen . ResizeControl ( size . Height , size . Width );
        }

        private void Window_Closed ( object sender , EventArgs e )
        {
            BankAcctVm . DoClosePanel -= BankAcctVm_DoClosePanel;
            BlankScreen = null;
            BankAcctGrid = null;
            BankAcDetails = null;
            this . Close ( );
        }

        private void Closepane_Click ( object sender , RoutedEventArgs e )
        {
            BankAcDetails = null;
            GenericGrid . datagrid1 . ItemsSource = null;
            GenericGrid . datagrid2 . ItemsSource = null;
            GenericGrid . datagrid1 . Items . Clear ( );
            GenericGrid . datagrid2 . Items . Clear ( );
            BankAcctGrid = null;
            BlankScreen = null;
            GenericGrid = null;
            comboPlus = null;
            GenCollection1 = null;
            GenCollection2 = null;
            GenClass = null;
            this . Close ( );
        }
        public ComboChangedArgs CreateComboArgs ( )
        {
            ComboChangedArgs args = new ComboChangedArgs ( );
            args . CBplus = comboPlus . comboBox;
            args . BHost = this;
            args . BlankCtrl = BlankScreen;
            args . GenCtrl = GenericGrid;
            if ( GenericGrid . datagrid1 . Visibility == Visibility . Visible )
            {
                GenericGrid . datagrid1 . UpdateLayout ( );
                args . grids [ 0 ] = GenericGrid . datagrid1;
            }
            else
            {
                GenericGrid . datagrid2 . UpdateLayout ( );
                args . grids [ 0 ] = GenericGrid . datagrid2;
            }//args . grids [ 1 ] = BlankScreen . dgrid2 . datagrid1;
            args . Activepanel = "";
            args . ActiveTablename = "";
            return args;
        }
        // Generic handler for toggling panels in our ContentControl
        private void ContentController ( object sender , RoutedEventArgs e )
        {
            Button btn = e . OriginalSource as Button;
            if ( btn is null ) return;
            if ( btn . Name == "BankDetails" )
                SetActivePanel ( "BANKACCOUNTLIST" );
            else if ( btn . Name == "AllAccounts" )
                SetActivePanel ( "BANKACCOUNTGRID" );
            else if ( btn . Name == "GenericBtn" )
                SetActivePanel ( "GENERICGRID" );
            else if ( btn . Name == "HidePanel" )
                SetActivePanel ( "BLANKSCREEN" );
            else if ( btn . Name == "HidePanel" )
                BankAcctVm_DoClosePanel ( null , null );
        }

        private void Settings_Click ( object sender , RoutedEventArgs e )
        {
        }

        private void StylesCombo_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {
            //return;
            try
            {
                string str = e . AddedItems [ 0 ] . ToString ( );
                if ( str == "Dark Mode" )
                {
                    GenericGrid . datagrid1 . CellStyle = null;
                    GenericGrid . datagrid1 . Foreground = Brushes . White;
                    GenericGrid . datagrid2 . CellStyle = null;
                    GenericGrid . datagrid2 . Foreground = Brushes . White;
                    return;
                }
                Style dt = Application . Current . FindResource ( str ) as Style;
                if ( dt != null )
                {
                    GenericGrid . datagrid1 . CellStyle = dt;
                    GenericGrid . datagrid1 . Refresh ( );
                    GenericGrid . datagrid2 . CellStyle = dt;
                    GenericGrid . datagrid2 . Refresh ( );
                }
            }
            catch ( Exception ex )
            {
                Debug . WriteLine ( $"{ex . Message}" );
            }
        }
    }
}
