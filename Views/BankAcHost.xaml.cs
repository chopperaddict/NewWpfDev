#define SIZEING 
using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Diagnostics;
using System . DirectoryServices . ActiveDirectory;
using System . Runtime . InteropServices;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;
using System . Windows . Media;

using DapperGenericsLib;

using NewWpfDev . Models;
using NewWpfDev . UserControls;
using NewWpfDev . ViewModels;

// This is used as a subset of the standard GenericClass specified in NewWpfDev
// Because this uses the DapperGenericsLib library
using GenericClass = DapperGenericsLib . GenericClass;


namespace NewWpfDev . Views
{

    /// <summary>
    ///********************
    /// MVVM system
    ///********************
    /// Interaction logic for BankAcHost.xaml
    /// Processing is handled by BANKACCOUNTVM.CS
    ///  This was  one of the Original control that uses ONLY my DapperGenLib to handle all SQL Db handling
    ///   saving special code required to handle the requirement for loading ANY type of table into
    ///   a DataGrid fore any required CRUD operations
    /// </summary>
    /// </summary>

    public partial class BankAcHost : Window
    {

        //static public event EventHandler<ComboChangedArgs> ComboboxChanged;
        #region Event Handling

        //======================================================================================//
        public static event EventHandler<StyleArgs> StylesSwitch;
        // Diifferent Style  has been selected, notify UserControl to handle it
        public void TriggerStyleSwitch ( object sender , StyleArgs e )
        {
            if ( StylesSwitch != null )
            {
                StylesSwitch ( this , e );
            }
            //OnStylesSwitch ( e );
        }
        //======================================================================================//
        #endregion Event Handling

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

        //public static ObservableCollection<GenericClass> GGenCollection;
        //public static ObservableCollection<GenericClass> Gencollection1;
        //public static ObservableCollection<GenericClass> Gencllection2;
        public static ObservableCollection<DapperGenericsLib . GenericClass> Gencollection1 = new ObservableCollection<DapperGenericsLib . GenericClass> ( );
        public static ObservableCollection<DapperGenericsLib . GenericClass> Gencollection2 = new ObservableCollection<DapperGenericsLib . GenericClass> ( );
        public static ObservableCollection<GenericClass> vmGencollection1 = new ObservableCollection<GenericClass> ( );
        public static ObservableCollection<GenericClass> vmGencollection2 = new ObservableCollection<GenericClass> ( );
        public static ObservableCollection<DapperGenericsLib . GenericClass> LibGencollection = new ObservableCollection<DapperGenericsLib . GenericClass> ( );
        public static ObservableCollection<GenericClass> GenClass = new ObservableCollection<GenericClass> ( );
        //public static GenericClass GenClass = new GenericClass ( );
        public List<string> TablesList = new List<string> ( );
        public string SelectedTable { get; set; }
        public Size GenGrid1Size;
        public Size GenGrid2Size;
        public Size GenContentSize;
        public bool IsStartup = true;

        #region properties
        //Singleton control class ?
        public static BankAcHost ThisWin { get; set; }
        private static BankAcctVm BankAcctVm { get; set; }
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
        public List<string> AllStyles { get; set; } = new List<string> ( );


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
            var style = res . Values;
            //Gencollection1 = BankAccountVM . Gencollection1;
            //Gencllection2 = BankAccountVM . Gencollection2;
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
            info . Text = "gdd dgd     s gdsggg";
            ComboboxPlus . ComboboxChanged += ComboboxPlus_ComboboxChanged;
            // Create a generic handler to handle the ContentControl Buttons
            RoutedEventHandler handler = new RoutedEventHandler ( ContentController );
            FocusManager . AddGotFocusHandler ( this , handler );

            // Setting Doesnt "Stick" in ComboboxPlus !!!!
            comboPlus . SetHost ( ThisWin );
            BankAcctVm . DoClosePanel += BankAcctVm_DoClosePanel;
            custgrid = BankAccountGrid . datagrid;

            //GenClass = new GenericClass ( );
            LoadDbTables ( );
            //loads the data using existing ICommand in BankAccountVM.CS
            //         viewmodel . SelectGrid . Execute ( "BANKACCOUNT" );
            // or use this  Extension method if you want to pass up to 3 args (as objects)
            object [ ] args = new object [ 3 ];
            args [ 0 ] = "GENERICGRID";
            //  viewmodel . SelectGrid . ExecuteCommand ( args );

            // loads the data from SQL for Generic  grid
            //     GenericGrid . LoadGenericTable ( "Bankaccount" , "datagrid1" );
            SetActivePanel ( "GENERICGRID" );
            //SetVisibility ( "GENERICGRID" , "GRID1" );
            //    MessageBox . Show ( "Failed to find VM collection" , "SQL data error" );

            // Trigger ViewModel to load al pinters to other controls
            BankAccountVM . TriggerGetControlsPointers ( );

            // SPEED UP ??
            AllStyles = Utils . GetAllDgStyles ( );
            //How to set ItemsSource of remote control
            SetValue ( PopupListBox . ItemsSourceProperty , AllStyles );
            SetValue ( PopupListBox . StylescountProperty , AllStyles . Count );
            StylesList . AddItems ( AllStyles );
            StylesList . SelectedIndex = 0;

            StylesList . SetHost ( this );
            Mouse . SetCursor ( Cursors . Arrow );
        }
        public void ResetSize ( )
        {
            this . Height += 1;
            Window_SizeChanged ( null , null );
        }
        private void SetUpViewmodelPointers ( )
        {

        }
        public static BankAcHost GetBankHostHandle ( )
        {
            return ThisWin;
        }

        private async Task LoadDbTables ( )
        {//dummy caller for task to load list of Db Tables
         //            await DoLoadDbTablesAsync ( );
            TablesList = GenericGridControl . GetDbTablesList ( "IAN1" );
            combo . ItemsList = TablesList;
            comboPlus . Visibility = Visibility . Visible;
            return;
        }
        //private async Task DoLoadDbTablesAsync ( )
        //{   //  load list of Db Tables asynchronously
        //    //List<string> TablesList = Task . Run ( ( ) => GenericGridControl . GetDbTablesList ( "IAN1" ) );
        //    GenericGridControl . GetDbTablesList ( "IAN1" );
        //    combo . ItemsList = TablesList;
        //    comboPlus . Visibility = Visibility . Visible;
        //    return;
        //}
        private void ComboboxPlus_ComboboxChanged ( object sender , ComboChangedArgs e )
        {
            // called when combobox selection changes
            int DbCount = 0, index = 0;
            string tablename = e . ActiveTablename . ToUpper ( );
            //************************************
            // GENERICGRID MUST BE  ACTIVE  
            //************************************
            if ( e . Activepanel == "GENERICGRID" )
            {
                index = 0;
                // Load//Reload Db data
                ComboboxPlus cbp = ComboboxPlus . GetCBP ( );
                if ( GenericGridControl . ActiveGrid == 1 )
                {
                    GenericGrid . datagrid1 . ItemsSource = null;
                    GenericGrid . datagrid1 . Items . Clear ( );
                    if ( cbp . ReloadDb == true )
                    {
                        GenericGridControl . dglayoutlist1 . Clear ( );
                        Task . Run ( ( ) =>
                        {
                            DapperGenLib . LoadTableGeneric ( $"Select * from {tablename}" , ref Gencollection1 );
                            Application . Current . Dispatcher . Invoke ( ( ) =>
                            {   // load grid 1
                                int colcount = DapperLibSupport . GetGenericColumnCount ( Gencollection1 );
                                DapperLibSupport . LoadActiveRowsOnlyInGrid ( GenericGrid . datagrid1 , Gencollection1 , colcount );

                                GenericGridControl . CurrentTable1 = tablename;
                                GenericGridControl . Title1 = tablename;
                                //st<DapperGenericsLib . DataGridLayout> dglayoutlist = new List<DapperGenericsLib . DataGridLayout> ( );
                                if ( GenericGrid . maskcols . Content . ToString ( ) == "Mask Columns" )
                                    DapperLibSupport . ReplaceDataGridFldNames ( tablename , ref GenericGrid . datagrid1 , ref GenericGridControl. dglayoutlist1 , colcount );
                                else
                                    GenericGrid . SetDefColumnHeaderText ( GenericGrid . datagrid1 , false );
                                // Setup new label and default table name
                                GenericGrid . GenericTitle1 . Text = $"{tablename . ToUpper ( )}";
                                GenericGridControl . CurrentTable1 = tablename;
                                GenericGridControl . Title1 = tablename;
                                Debug . WriteLine ( $"[{tablename}] Data  for datagrid1 Loaded in Task and Datagrid fully updated" );
                                GenericGrid . datagrid1 . SelectedIndex = 0;
                                GenericGrid . datagrid1 . Refresh ( );
                                GenericGridControl . SelectCorrectTable ( tablename );
                            } );
                        } );
                    }
                }
                else if ( GenericGridControl . ActiveGrid == 2 )
                {
                    GenericGrid . datagrid2 . ItemsSource = null;
                    GenericGrid . datagrid2 . Items . Clear ( );
                    if ( cbp . ReloadDb == true )
                    {
                        GenericGridControl . dglayoutlist2 . Clear ( );
                        Task . Run ( ( ) =>
                        {
                            DapperGenLib . LoadTableGeneric ( $"Select * from {tablename}" , ref Gencollection2 );
                            Application . Current . Dispatcher . Invoke ( ( ) =>
                            {
                                int colcount = DapperLibSupport . GetGenericColumnCount ( Gencollection2 );
                                DapperLibSupport . LoadActiveRowsOnlyInGrid ( GenericGrid . datagrid2 , Gencollection2 , colcount );

                                GenericGridControl . CurrentTable2 = tablename;
                                GenericGridControl . Title2 = tablename;
                                //st<DapperGenericsLib . DataGridLayout> dglayoutlist = new List<DapperGenericsLib . DataGridLayout> ( );
                                if ( GenericGrid . maskcols . Content . ToString ( ) == "Mask Columns" )
                                    DapperLibSupport . ReplaceDataGridFldNames ( tablename , ref GenericGrid . datagrid2 , ref GenericGridControl. dglayoutlist2 , colcount );
                                else
                                    GenericGrid . SetDefColumnHeaderText ( GenericGrid . datagrid2 , false );
                                // Setup new label and default table name
                                GenericGrid . GenericTitle2 . Text = $"{tablename . ToUpper ( )}";
                                GenericGridControl . CurrentTable2 = tablename;
                                GenericGridControl . Title2 = tablename;
                                Debug . WriteLine ( $"[{tablename}] Data  for datagrid1 Loaded in Task and Datagrid fully updated" );
                                GenericGrid . datagrid1 . SelectedIndex = 0;
                                GenericGrid . datagrid1 . Refresh ( );
                                GenericGridControl . SelectCorrectTable ( tablename );
                            } );
                        } );
                    }
                    // Setup new label and default table name
                    // Reset flag so it will load data unless something else toggles it off !!
                }
                cbp . ReloadDb = true;
            }
        }
        private void ResetStyleSelection ( int index )
        {
            //if ( index == 1 )
            //    GenericGrid . StylesList . SelectedItem = GenericGridControl . Style1;
            //else
            //    GenericGrid . StylesList . SelectedItem = GenericGridControl . Style2;
            //GenericGrid . StylesList . Refresh ( );

        }
        public static GenericClass ConvertLibToGeneric ( GenericClass gcc , DapperGenericsLib . GenericClass DapperGen )
        {
            gcc . field1 = DapperGen . field1;
            gcc . field2 = DapperGen . field2;
            gcc . field3 = DapperGen . field3;
            gcc . field4 = DapperGen . field4;
            gcc . field5 = DapperGen . field5;
            gcc . field6 = DapperGen . field6;
            gcc . field7 = DapperGen . field7;
            gcc . field8 = DapperGen . field8;
            gcc . field9 = DapperGen . field9;
            gcc . field10 = DapperGen . field10;
            gcc . field11 = DapperGen . field11;
            gcc . field12 = DapperGen . field12;
            gcc . field13 = DapperGen . field13;
            gcc . field14 = DapperGen . field14;
            gcc . field15 = DapperGen . field15;
            gcc . field16 = DapperGen . field16;
            gcc . field17 = DapperGen . field17;
            gcc . field18 = DapperGen . field18;
            gcc . field19 = DapperGen . field19;
            gcc . field20 = DapperGen . field20;
            return gcc;
        }

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
            // local Collection only
            ObservableCollection<DapperGenericsLib.GenericClass> GenericClass = new ObservableCollection<DapperGenericsLib . GenericClass> ( );
            Dictionary<string , string> dict = new Dictionary<string , string> ( );
            List<Dictionary<string , string>> ColumntypesList = new List<Dictionary<string , string>> ( );
            // This returns a Dictionary<sting,string> PLUS a collection  and a List<string> passed by ref....
            Dictionary<string , string> Columntypes = new Dictionary<string , string> ( );
            List<DapperGenericsLib . DataGridLayout> dglayoutlist = new List<DapperGenericsLib . DataGridLayout> ( );
            dict = DapperGenLib . GetDbTableColumns ( ref GenericClass , ref ColumntypesList , ref list , tablename , domain , ref dglayoutlist );

            indx = 0;
            if ( dglayoutlist . Count > 0 )
            {
                foreach ( var item in GenericClass )
                {
                    item . field3 = dglayoutlist [ indx++ ] . Fieldlength . ToString ( );
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
            Task . Run ( ( ) => BankAcDetails . LoadCustomer ( ) );
            // SetActivePanel ( "GENERICGRID" );
            CurrentPanel = "GENERICGRID";
        }

        private bool SetVisibility ( string newpanel , string arg = "" )
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
                if ( IsStartup )
                    SetActivePanel ( "GENERICGRID" );
                else
                {
                    if ( Gencollection1 == null || Gencollection1 . Count == 0 )
                    {
                        if ( Gencollection1 == null ) return false;//GenCollection1 = new ObservableCollection<GenericClass> ( );

                        if ( Gencollection1 . Count == 0 )
                        {
                            combo . ComboSelection1 = "BANKACCOUNT";
                            combo . currentComboSelection = combo . ComboSelection1;
                            combo . gridtablenames [ 0 ] = "BANKACCOUNT";
                            // preload data if needed by calling method in GenericGridControl itself
                            if ( GenericGrid . datagrid1 . Items . Count == 0 )
                                GenericGrid . LoadGenericTable ( "BANKACCOUNT" , "datagrid1" );
                        }
                        comboPlus . ComboSelection1 = comboPlus . SelectedItem?.ToString ( ) . ToUpper ( );
                    }
                    if ( Gencollection2 == null || Gencollection2 . Count == 0 )
                    {
                        combo . ComboSelection2 = "BANKACCOUNT";
                        combo . currentComboSelection = combo . ComboSelection2;
                        combo . gridtablenames [ 1 ] = "BANKACCOUNT";
                        // preload data if needed (runs as a task & loads & formats Grid)
                        // Intial startup of system
                        //List<Dictionary<string , string>> ColumntypesList = new List<Dictionary<string , string>> ( );
                        //List<DapperGenericsLib . DataGridLayout> dglayoutlist = new List<DapperGenericsLib . DataGridLayout> ( );
                        //if ( GenericGrid . datagrid2 . Items . Count == 0 )
                        //    Gencollection2 = DapperGenLib . LoadDbAsGenericData (
                        //        "Select * from BankAccount" ,
                        //       Gencollection2 ,
                        //        ref ColumntypesList ,
                        //        "" ,
                        //        "IAN1" ,
                        //        ref dglayoutlist );
                        //if ( GenericGrid . datagrid2 . Items . Count == 0 )
                        //    Gencollection2 = Gencollection1;
                        //int colcount = DapperLibSupport . GetGenericColumnCount ( Gencollection2 );
                        //DapperLibSupport . LoadActiveRowsOnlyInGrid ( GenericGrid . datagrid1 , Gencollection2 , colcount );
                        //colcount = DapperLibSupport . GetGenericColumnCount ( Gencollection2 );
                        //DapperLibSupport . LoadActiveRowsOnlyInGrid ( GenericGrid . datagrid2 , Gencollection2 , colcount );
                        //GenericGridControl . SelectCorrectTable ( "BANKACCOUNT" );
                        // preload data if needed by calling method in GenericGridControl itself
                        if ( GenericGrid . datagrid1 . Items . Count == 0 )
                            GenericGrid . LoadGenericTable ( "BANKACCOUNT" , "datagrid1" );
                        GenericGridControl . Title2 = "BANKACCOUNT";
                        Debug . WriteLine ( $"[BANKACCOUNT] Data  for datagrid2 Loaded in Task and Datagrid fully updated" );
                        comboPlus . ComboSelection2 = comboPlus . SelectedItem?.ToString ( ) . ToUpper ( );
                    }
                    comboPlus . Promptlabel . Opacity = 1.0;
                    combo . IsEnabled = true;
                    combo . Opacity = 1.0;
                    GenericBtn . IsEnabled = true;
                    BankVm . InfoText = "Use Combo at right to select any Db Table you want to view... ";
                    //Default to Grid 1
                    //if ( ( string ) GenericGrid . Togglegrid . Content == "< Grid 1" )
                    //{
                    //    GenericGrid . datagrid2 . Visibility = Visibility . Collapsed;
                    //    GenericGrid . datagrid1 . Visibility = Visibility . Visible;
                    //}
                    //else
                    //{
                    //    GenericGrid . datagrid1 . Visibility = Visibility . Collapsed;
                    //    GenericGrid . datagrid2 . Visibility = Visibility . Visible;
                    //}
                    GenericGrid . UpdateLayout ( );
                }
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
            return true;
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
                default:
                    break;
            }
        }
        // Find currently selected table in combo and selexct it (without loosing the prompt)

        public void UpdateCombo ( string table )
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

        public async Task SetActivePanel ( string newpanel )
        {
            try
            {
                // Set dapperlib scope flag to convert datetime to date string only for display usage in datagrids etc.
                // used by GenericGrid only right now
                DapperGenLib . ConvertDateTimeToNvarchar = false;

                if ( BankContent == null )
                {
                    GenContentSize . Height = BankContent . Height - 65;
                    GenContentSize . Width = BankContent . Width - 220;
                }
                GenGrid1Size . Height = GenContentSize . Height;
                GenGrid1Size . Width = GenContentSize . Width;
                GenGrid2Size . Height = GenContentSize . Height;
                GenGrid2Size . Width = GenContentSize . Width;
            }
            catch ( Exception ex ) { Debug . WriteLine ( $"Bypassing top sizing in SetActivePanel()" ); }
            if ( newpanel == "BANKACCOUNTLIST" )
            {
                if ( BankAcDetails == null )
                    BankAcDetails = new BankAccountInfo ( );
                this . BankContent . Content = BankAcDetails;
                BankAcDetails . HorizontalAlignment = HorizontalAlignment . Left;
                BankAcDetails . VerticalAlignment = VerticalAlignment . Top;
                comboPlus . Promptlabel . Opacity = 0.3;
                combo . IsEnabled = false;
                combo . Opacity = 0.3;
                BankAcDetails . Visibility = Visibility . Visible;
            }
            else if ( newpanel == "BANKACCOUNTGRID" )
            {
                if ( BankAcctGrid == null )
                    BankAcctGrid = new BankAccountGrid ( );
                this . BankContent . Content = BankAcctGrid;
                comboPlus . Promptlabel . Opacity = 0.3;
                combo . IsEnabled = false;
                combo . Opacity = 0.3;
                BankAcctGrid . Visibility = Visibility . Visible;
            }
            else if ( newpanel == "GENERICGRID" )
            {
                this . BankContent . Content = GenericGrid;
                GenericGrid . Height = BankContent . Height;
                // Set dapperlib scope flag to convert datetime to date string only for displqay usage inj datagrids etc.
                DapperGenLib . ConvertDateTimeToNvarchar = true;

                if ( this . BankContent . Width != 0 && this . BankContent . Height != 0 )
                {
                    GenericGrid . datagrid1 . Width = BankContent . Width - 20;
                    GenericGrid . datagrid1 . Height = BankContent . Height - 90;
                    GenericGrid . datagrid2 . Width = BankContent . Width - 20;
                    GenericGrid . datagrid2 . Height = BankContent . Height - 90;
                }
                GenericGrid . UpdateLayout ( );
                GenericGridControl . Host = this;
                if ( IsStartup )
                {
                    bool useExtensions = false;
                    // Intial startup of system
                    if ( GenericGrid . datagrid1 . Items . Count == 0 )
                    {
                        Dictionary<string , string> dict = new Dictionary<string , string> ( );
                        List<Dictionary<string , string>> ColumntypesList = new List<Dictionary<string , string>> ( );
                        List<string> list = new List<string> ( );
                        DataGrid [ ] grids = new DataGrid [ 4 ];
                        if ( useExtensions == false )
                        {
                            // load more than 1 grid from same data <= 4, +  get the obs.collection used back as well;
                            grids [ 0 ] = GenericGrid . datagrid1;
                            grids [ 1 ] = GenericGrid . datagrid2;
                            // let it get on with loading data and populating grids while we carry on loading the window
                            Task . Run ( ( ) => DataLoad . LoadGenericTable ( "BankAccount" ,
                                grids ,
                                Gencollection1 ,
                                GenericGrid . GenericTitle1 ,
                                GenericGrid . GenericTitle2 ) );
                        }
                        else
                        {
                            // Gencollection2 = Gencollection2 . LoadGenData ( "BANKACCOUNT" , GenericGrid . datagrid2 );
                            //DapperGenLib . LoadTableGeneric ( $"Select * from BANKACCOUNT" , ref Gencollection1 );
                            //Gencollection1 = Gencollection1 . LoadGenData ( "BANKACCOUNT" , GenericGrid . datagrid1 );
                            // Testing extension method
                            // using my Data Extensions library class in DataExtensions.cs
                            dict = GetColumnNames ( "BankAccount" , out int count , "IAN1" );
                        }
                        //                        await Task . Run ( ( ) => GenericGrid . LoadGenericTable ( "BankAccount" , "datagrid1" ));
                    }
                }
                //GenericGrid . Refresh ( );
                //this . BankContent . Refresh ( );
                comboPlus . Promptlabel . Opacity = 1.0;
                // Setup the banner title string
                SetGenGridTitleBar ( );
                combo . IsEnabled = true;
                combo . Opacity = 1.0;
                GenericGrid . Height += 1;
                GenericGridControl . SelectCorrectTable ( "BANKACCOUNT" );

                GenericGrid . UpdateLayout ( );
                //GenericGrid . Refresh ( );
            }
            else if ( newpanel == "BLANKSCREEN" )
            {
                this . BankContent . Content = BlankScreen;

                this . BankContent . Refresh ( );
                comboPlus . Promptlabel . Opacity = 0.3;
                combo . IsEnabled = false;
                combo . Opacity = 0.3;
            }
            BlankScreen . Visibility = Visibility . Visible;
            this . BankContent . Refresh ( );
        }
        public static void SetGenGridTitleBar ( )
        {
            if ( GenericGrid . datagrid1 . Visibility == Visibility . Visible )
            {
                if ( GenericGridControl . Title1 != "" )
                    GenericGrid . GenericTitle1 . Text = GenericGridControl . Title1;
                else
                    GenericGrid . GenericTitle2 . Text = "BANKACCOUNT";
            }
            else if ( GenericGrid . datagrid2 . Visibility == Visibility . Visible )
            {
                if ( GenericGridControl . Title2 != "" )
                    GenericGrid . GenericTitle2 . Text = GenericGridControl . Title2;
                else
                    GenericGrid . GenericTitle2 . Text = "BANKACCOUNT";
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
            // Set dapperlib scope flag to convert datetime to date string only for displqay usage inj datagrids etc.
            DapperGenLib . ConvertDateTimeToNvarchar = false;
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
            // Set dapperlib scope flag to convert datetime to date string only for displqay usage inj datagrids etc.
            DapperGenLib . ConvertDateTimeToNvarchar = false;
            SetActivePanel ( newpanel );
            return;
        }

        private void DoClosePanel ( object sender , string newpanel )
        {
            // open specified panel
            // Set dapperlib scope flag to convert datetime to date string only for displqay usage inj datagrids etc.
            DapperGenLib . ConvertDateTimeToNvarchar = false;
            SetActivePanel ( newpanel );
            return;
        }

        private void ClosePanel ( object sender )
        {
            if ( sender == null )
            {
                // hide all panes
                // Set dapperlib scope flag to convert datetime to date string only for displqay usage inj datagrids etc.
                DapperGenLib . ConvertDateTimeToNvarchar = false;
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
            BankContent . Refresh ( );
        }
        private void Window_SizeChanged ( object sender , SizeChangedEventArgs e )
        {
            Size oldsize;
            Size newsize;
            if ( e != null )
            {
                oldsize = e . PreviousSize;
                newsize = e . NewSize;
            }
            ///// Allow for right and bottom margins
            if ( newsize . Height > 0 )
            {
                this . BankContent . Height = newsize . Height > 0 ? newsize . Height - 80 : 400;
                this . BankContent . Width = newsize . Width - 200;
                BankAcctGrid . ResizeControl ( newsize . Height - 110 , newsize . Width - 240 );
                BankAcDetails . ResizeControl ( newsize . Height - 190 , newsize . Width - 230 );
                GenericGrid . ResizeControl ( newsize . Height - 90 , newsize . Width - 250 );
                if ( setSplitter )
                {
                    double genctrlheight = newsize . Height - 90;
                    double topgrid = genctrlheight;
                    double Offset1 = GenericGrid . SplitterGrid . RowDefinitions [ 0 ] . ActualHeight;
                    double Offset2 = GenericGrid . SplitterGrid . RowDefinitions [ 1 ] . ActualHeight;
                    GenericGrid . datagrid1 . Height = Offset1 - 230;
                    GenericGrid . datagrid1 . Height = Offset2 - 330;
                }
            }
        }
        private bool setSplitter = false;
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
            MainWindow . RemoveGenericlistboxcontrol ( GenericGrid.canvas );
            GenericGrid = null;
            comboPlus = null;
            Gencollection1 = null;
            Gencollection2 = null;
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
            }
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

        private void StylesCombo_MouseEnter ( object sender , MouseEventArgs e )
        {

        }

        private void StylesCombo_MouseLeave ( object sender , MouseEventArgs e )
        {

        }
    }
}
