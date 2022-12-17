using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Data;
using System . Data . SqlClient;
using System . Diagnostics;
using System . Diagnostics . Metrics;
using System . DirectoryServices . ActiveDirectory;
using System . Globalization;
using System . Linq;
using System . Reflection;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Controls . Primitives;
using System . Windows . Data;
using System . Windows . Input;
using System . Windows . Media;

using DapperGenericsLib;

using GenericSqlLib . Models;

using NewWpfDev . Models;
using NewWpfDev . ViewModels;
using NewWpfDev . Views;

///using GenericClass = DapperGenericsLib . GenericClass;

namespace NewWpfDev . UserControls
{
    /// <summary>
    ///  This was  the Original control that uses ONLY my DapperGenLib to hndle all SQL Db handling
    ///   saving special code required to handle the requirement for loading ANY type of table into
    ///   a DataGrid fore any required CRUD operations
    /// </summary>
    public partial class GenericGridControl : UserControl
    {
        #region Testing
        [DefaultValue ( typeof ( double ) , "130" )]
        new public double Width { get; set; }

        [DefaultValue ( typeof ( double ) , "30" )]
        new public double Height { get; set; }

        public Size GenGrid1Size;
        public Size GenGrid2Size;
        public Size GenContentSize;

        public ExtendSplitter splithandler { set; get; }
        #endregion Testing

        public static event EventHandler<ListboxHostArgs> SetListboxHost;

        // Flowdoc file wide variables
        // Pointer to the special library FlowdocLib.cs 
        FlowdocLib fdl;
        private double XLeft = 0;
        private double YTop = 0;
        private bool UseFlowdoc = true;
        public static object MovingObject { get; set; }

        public UIElement UcHostElement;
        public Control UcHostControl;
        public string UcHostName;
        public string UcHostType;
        //public static MapperConfiguration MapperCfg;

        //#region Event Handling
        ////======================================================================================//
        //public static event EventHandler<StyleArgs> StylesSwitch;
        //// Diifferent Style  has been selected, notify UserControl to handle it
        //public void TriggerStyleSwitch ( object sender , StyleArgs e )
        //{
        //    if ( StylesSwitch != null )
        //    {
        //        StylesSwitch ( this , e );
        //    }
        //    //OnStylesSwitch ( e );
        //}

        ////======================================================================================//
        //#endregion Event Handling

        #region normal variable Declarations {get;set;}
        public static bool ShowColumnNames = true;
        public static string procname = "";
        public static bool IsMousedown = false;
        public static double MaxTopHeight = 0;
        public bool isInsertMode = false;
        public static int colcount = 0;
        public bool isBeingEdited = false;
        public static string CurrentTable1;
        public static string CurrentTable2;
        public static string Title1;
        public static string Title2;
        public static string Style1 = "Dark Mode";
        public static string Style2 = "Dark Mode";
        public static bool NoUpdate = false;
        public static bool Startup = true;
        public static int ActiveGrid { get; set; } = 1;
        public static bool ShowingHeaders1 { get; set; } = true;
        public static bool ShowingHeaders2 { get; set; } = true;

        public static DataTable dt = new DataTable ( );
        public static DataTable dt2 = new DataTable ( );
        public static List<DapperGenericsLib . GenericClass> list = new List<DapperGenericsLib . GenericClass> ( );
        public static BankAcHost Host { get; set; }
        public static GenericGridControl ThisWin { get; set; }

        #endregion normal File Declarations
        #region Data Models

        //Data Collections
        public static ObservableCollection<DapperGenericsLib . GenericClass> Gencollection1 = new ObservableCollection<DapperGenericsLib . GenericClass> ( );
        public static ObservableCollection<DapperGenericsLib . GenericClass> Gencollection2 = new ObservableCollection<DapperGenericsLib . GenericClass> ( );
        public static List<DapperGenericsLib . GenericClass> Listcollection1 { get; set; } = new List<DapperGenericsLib . GenericClass> ( );
        public static List<DapperGenericsLib . GenericClass> Listcollection2 { get; set; } = new List<DapperGenericsLib . GenericClass> ( );

        public static DapperGenericsLib . DataGridLayout? dglayout { get; set; } = new DapperGenericsLib . DataGridLayout ( );
        public static List<DapperGenericsLib . DataGridLayout> dglayoutlist1 = new List<DapperGenericsLib . DataGridLayout> ( );
        public static List<DapperGenericsLib . DataGridLayout> dglayoutlist2 = new List<DapperGenericsLib . DataGridLayout> ( );

        // Current Generic selection in use record
        public static new DapperGenericsLib . GenericClass GenClass = new DapperGenericsLib . GenericClass ( );
        #endregion Data Models

        #region Full Properties

        private static ObservableCollection<DapperGenericsLib . GenericClass> genclass;
        private static DataGrid genericgrid1;
        private static DataGrid genericgrid2;

        public static DataGrid GenericGrid1
        {
            get { return genericgrid1; }
            set { genericgrid1 = value; }
        }
        public static DataGrid GenericGrid2
        {
            get { return genericgrid2; }
            set { genericgrid2 = value; }
        }
        private double grid1Height;
        public double Grid1Height
        {
            get { return grid1Height; }
            set { grid1Height = value; }
        }
        private double grid2Height;
        public double Grid2Height
        {
            get { return grid2Height; }
            set { grid2Height = value; }
        }
        private double splitteroffset;
        public double Splitteroffset
        {
            get { return splitteroffset; }
            set { splitteroffset = value; }
        }
        private bool minSplitterHit;

        public bool MinSplitterHit
        {
            get { return minSplitterHit; }
            set { minSplitterHit = value; }
        }
        private DependencyPropertyDescriptor HeightDescriptor;

        public DependencyPropertyDescriptor heightDescriptor
        {
            get { return HeightDescriptor; }
            set { HeightDescriptor = value; }
        }

        #endregion Full Properties

        #region Behaviors

        // NOT in use
        public static DependencyProperty CreateAttachedProperty ( Type ctrl = null , string ApName = "" , Type ArgType = null , string namesuffix = "" , object DefValue = null )
        {
            //    if ( ctrl == null || ApName == "" )
            //        return null;
            //    if ( namesuffix == "" )
            //    {
            //        DependencyProperty DefProperty0 =
            //              DependencyProperty . RegisterAttached (
            //                  ApName ,
            //                ArgType ,
            //                ctrl as Type ,
            //                new PropertyMetadata ( DefValue ) );
            //        return DefProperty0;
            //    }
            return ( DependencyProperty ) null;
        }

        public static readonly DependencyProperty TextChangedProperty =
          DependencyProperty . Register ( "TextChanged" ,
          typeof ( bool ) , 
          typeof ( GenericGridControl ) ,
            typeMetadata: new FrameworkPropertyMetadata (
                defaultValue:(bool) true, 
                flags: FrameworkPropertyMetadataOptions .None) ,
            new ValidateValueCallback ( GetTextChanged ) );

        //public static readonly DependencyProperty TextChangedProperty =
        //  DependencyProperty . RegisterAttached ( "TextChanged" ,
        //  typeof ( bool ) , typeof ( GenericGridControl ) ,
        //  new PropertyMetadata ( true ) , new ValidateValueCallback ( GetTextChanged ) );

        private static TextChangedEventHandler OnTextChanged ( DependencyObject d , DependencyPropertyChangedEventArgs e )
        {
            return CtrlTextChanged;
        }
        private static void CtrlTextChanged ( object sender , RoutedEventArgs e )
        {
            // Set Background to Off whiite when empty, else light green
            // and toggle the caret color as well from Black to Red
           TextBox? tb = sender as TextBox;
            if ( tb == null || tb . GetType ( ) != typeof ( TextBox ) )
                return;
            if ( tb . Text . Length != 0 )
            {
                if ( tb . Background == Application . Current . FindResource ( "White5" ) as SolidColorBrush )
                {
                    tb . Background = Application . Current . FindResource ( "Green5" ) as SolidColorBrush;
                    tb . CaretBrush = Application . Current . FindResource ( "Black0" ) as SolidColorBrush;
                }
            }
            else
            {
                if ( tb . Background == Application . Current . FindResource ( "Green5" ) as SolidColorBrush ) ;
                {
                    tb . Background = Application . Current . FindResource ( "White5" ) as SolidColorBrush;
                    tb . CaretBrush = Application . Current . FindResource ( "Red0" ) as SolidColorBrush;
                }
            }
        }

        public bool TextChanged
        {
            get => ( bool ) GetValue ( TextChangedProperty );
            set => SetValue ( TextChangedProperty , value );
        }
        private static bool GetTextChanged ( object value )
        {
            bool val = ( bool ) value;
            if ( val )
                return true;

            return false;
        }
        private static bool SetTextChanged ( object value )
        {
            bool val = ( bool ) value;
            if ( val == true )
                return true;

            return false;
        }

        #endregion  Behaviors

        #region Constructor
        //Datacontext is set in XAML (to BankAccountVM), NOT in here
        public GenericGridControl ( BankAcHost host )
        {
            InitializeComponent ( );
            Utils . ClearAttachedProperties ( this );
            Mouse . OverrideCursor = Cursors . Wait;
            this . UpdateLayout ( );
            ThisWin = this;
            fdl = new FlowdocLib ( Flowdoc , canvas );
            if ( host != null ) Host = host;
            //DependencyPropertyChangedEventArgs dargs = new DependencyPropertyChangedEventArgs ( );
            //dargs . NewValue = FindResource ( "Red1" );
            splithandler = new ExtendSplitter ( this );
            // setup handler for textbox background color change on being Empty or having content
            clrtb . TextChanged += OnTextChanged ( clrtb , new DependencyPropertyChangedEventArgs ( ) );
        }

        private void GenGridControl_Loaded ( object sender , RoutedEventArgs e )
        {
            Mouse . OverrideCursor = Cursors . Wait;

            GenericGrid1 = datagrid1;
            Thickness th = new Thickness ( );
            th . Left = 10;
            // Set dapperlib scope flag to convert datetime to date string only for displqay usage inj datagrids etc.
            DapperGenLib . ConvertDateTimeToNvarchar = true;

            GenericGrid1 = datagrid1;
            GenericGrid2 = datagrid2;
            datagrid1 . Margin = th;
            datagrid2 . Margin = th;
            GenericGrid1 . Margin = th;
            GenericGrid2 . Margin = th;
            Mouse . SetCursor ( Cursors . Wait );
            CurrentTable1 = CurrentTable2 = "BANKACCOUNT";
            Mouse . SetCursor ( Cursors . Wait );
            int colcount = GetDbColumnCount ( CurrentTable1 );
            Togglegrid . Content = "Grid 2 >";
            Mouse . SetCursor ( Cursors . Arrow );
            PopupListBox . Stylechanged += GenericGridControl_Stylechanged;
            if ( Host != null )
            {
                GenGrid1Size . Height = Host . BankContent . Height;
                GenGrid1Size . Width = Host . BankContent . Width;
                GenGrid2Size . Height = Host . BankContent . Height;
                GenGrid2Size . Width = Host . BankContent . Width;
            }
            //           Splitter . SizeChanged += Splitter_SizeChanged;
            string ConString = "";
            if ( DapperLibSupport . CheckResetDbConnection ( "IAN1" , out string constring ) == true )
                Debug . WriteLine ( $"Db IAN1 is set and Connectionstring is loaded successfully" );
            else
                Debug . WriteLine ( $"Db IAN1 could not be set and Connectionstring has NOT been  loaded " );
            // This fails
            DapperLibSupport . CheckDbDomain ( "IAN1" );
            // This works in GenericDbUtilities
            ConString = DapperGenLib . CurrentConnectionString;

            // Set Horizontal Splitter FULLY DOWN at startup
            double Offset1 = SplitterGrid . RowDefinitions [ 0 ] . ActualHeight;
            double Offset2 = SplitterGrid . RowDefinitions [ 1 ] . ActualHeight;
            Togglegrid . Content = "Grid 2 >";
            //Maximize hook  +/- statements - dont forget to remove them (Unsubscribe on closing)
            Flowdoc . ExecuteFlowDocMaxmizeMethod += new EventHandler ( MaximizeFlowDoc );
            FlowDoc . FlowDocClosed += Flowdoc_FlowDocClosed;

            if ( GenericGridControl . SetListboxHost != null )
            {
                ListboxHostArgs args = new ListboxHostArgs ( );
                args . HostControl = this;
                args . HostName = "GenGridControl";
                args . HostType = "GenericGridControl";
                GenericGridControl . SetListboxHost . Invoke ( this , args );
            }
            // Set splitter height change monitor method up correctly
            if ( heightDescriptor == null )
            {
                heightDescriptor = DependencyPropertyDescriptor . FromProperty ( RowDefinition . HeightProperty , typeof ( ItemsControl ) );
                heightDescriptor . AddValueChanged ( SplitterGrid . RowDefinitions [ 0 ] , SplitterHeightChanged );
            }
            SplitterGrid . UpdateLayout ( );
            Mouse . OverrideCursor = Cursors . Arrow;
        }

        //private void Splitter_SizeChanged ( object sender , SizeChangedEventArgs e )
        //{
        //    //throw new NotImplementedException ( );
        //}

        #endregion Constructor
        public static void CheckDbDomain ( string DbDomain )
        {
            if ( DapperGenLib . ConnectionStringsDict == null || Flags . ConnectionStringsDict . Count == 0 )
                Utils . LoadConnectionStrings ( );
            DapperLibSupport . CheckResetDbConnection ( DbDomain , out string constring );
            Flags . CurrentConnectionString = constring;
        }

        public static GenericGridControl GetGenGridHandle ( )
        {
            return ThisWin;
        }
        public void ResizeControl ( double height , double width )
        {
            this . Height = height;
            this . Width = width;
            this . Refresh ( );
        }
        private void PopupListBox_StyleSizeChanged ( object sender , SizeChangedArgs e )
        {
            Debug . WriteLine ( $"Popup listbox height changed to {e . NewHeight}" );
        }
        private void GenericGridControl_Stylechanged ( object sender , StyleArgs e )
        {
            if ( NoUpdate )
                return;
            string str = e . style;
            if ( str == "Dark Mode" )
            {
                StyleArgs args = new StyleArgs ( );
                if ( Togglegrid . Content . ToString ( ) . Contains ( "Grid 2" ) )
                {
                    datagrid1 . CellStyle = null;
                    datagrid1 . Foreground = Brushes . White;
                    args . dgrid = datagrid1;
                    Style1 = str;
                }
                else
                {
                    datagrid2 . CellStyle = null;
                    datagrid2 . Foreground = Brushes . White;
                    args . dgrid = datagrid2;
                    Style2 = str;
                }
                args . style = str;
                args . sender = this;
                return;
            }
            else
            {
                StyleArgs args = new StyleArgs ( );
                Style style = FindResource ( str ) as Style;
                if ( Togglegrid . Content . ToString ( ) . Contains ( "Grid 2" ) )
                {
                    datagrid1 . CellStyle = style;
                    args . dgrid = datagrid1;
                    Style1 = str;
                }
                else
                {
                    datagrid2 . CellStyle = style;
                    args . dgrid = datagrid2;
                    Style2 = str;
                }
                args . style = str;
                args . sender = this;
                return;
            }
        }
        public void UpDateStyle ( string str )
        {
            if ( Togglegrid . Content . ToString ( ) . Contains ( "Grid 2" ) )
            {
                datagrid1 . CellStyle = null;
                datagrid1 . Foreground = Brushes . White;
                Style1 = str;
            }
            else
            {
                datagrid2 . CellStyle = null;
                datagrid2 . Foreground = Brushes . White;
                Style2 = str;
            }
        }
        public void CreateGridColumnHeaders ( int colcount , DataGrid grid )
        {
            grid . Columns . Clear ( );
            for ( int x = 0 ; x < colcount ; x++ )
            {
                // can  be any column type !
                DataGridTextColumn tc = new DataGridTextColumn ( );
                // column internal name
                Binding b = new Binding ( $"Col {x + 1}" );
                tc . Binding = b;
                // Column title
                tc . Header = $"Col {x + 1}";
                // Add to grid
                grid . Columns . Add ( tc );
            }
        }
        private int GetDbColumnCount ( string table )
        {
            int retval = 0;
            if ( table == "BANKACCOUNT" )
                retval = 8;
            else if ( table == "CUSTOMER" )
                retval = 14;
            else
                GenDapperQueries . GetColumnNamesAsDictionary ( table , out retval , "IAN1" );
            return retval;
        }
        /// <summary>
        /// Special method that loads GENERIC format data from Sql Table, & Loads it into specified DataGrid
        /// cleaning away unused columns and even loading the real Column names  for the selected table
        /// </summary>
        /// <param name="table">Sql Table name</param>
        /// <param name="grid">Datagrid to be loaded</param>
        /// <returns></returns>
        public async Task<bool> LoadGenericTable ( string table , string grid , bool LoadDataIntoGrid = true , ObservableCollection<DapperGenericsLib . GenericClass> GenTable = null )
        {
            int colcount = 0;
            bool success = true;
            // How to access Config Mgr strings
            // not very useful really as it is NOT dynamic....
            //var dbString = ConfigurationManager . ConnectionStrings [ "Ian1Db" ] . ConnectionString;
            //var AppPath = ConfigurationManager . AppSettings[ "NewWpfDev=C" ];
            DapperLibSupport . LoadConnectionStrings ( );
            string ConString = "";
            DapperLibSupport . CheckResetDbConnection ( "IAN1" , out ConString );
            string con = DapperLibSupport . GetDictionaryEntry ( DapperGenLib . ConnectionStringsDict , "IAN1" , out string dictvalue );
            Stopwatch sw = new Stopwatch ( );
            sw . Start ( );
            if ( ActiveGrid == 1 || Gencollection1 . Count == 0 )
            {
                List<Dictionary<string , string>> ColumntypesList = new List<Dictionary<string , string>> ( );
                ActiveGrid = 1;
                // perform ALL items as part of the async  task
                await Task . Run ( ( ) =>
                {
                    DapperGenLib . LoadTableGeneric ( $"Select * from {table}" , ref Gencollection1 );
                    if ( Gencollection1 == null )
                    {
                        $"FATAL ERROR - Unable to load Db table {table}" . cwerror ( );
                        success = false;
                    }
                    else
                    {
                        if ( LoadDataIntoGrid == false )
                        {
                            GenTable = Gencollection1;
                            success = true;
                        }
                        else
                        {
                            //CreateGenericColumns ( 0 , datagrid );
                            // load data for datagrid1
                            Application . Current . Dispatcher . Invoke ( ( ) =>
                            {
                                //CreateGenericColumns ( DapperLibSupport . GetGenericColumnCount ( Gencollection1 ) , datagrid1 );
                                Listcollection1 . Clear ( );
                                Listcollection2 . Clear ( );

                                foreach ( var item in Gencollection1 )
                                {
                                    Listcollection1 . Add ( item );
                                    Listcollection2 . Add ( item );
                                }
                                // datagrid1
                                // get column count for LIST
                                colcount = GetColumnsCount ( Listcollection1 );
                                datagrid1 . ItemsSource = Listcollection1;
                                DapperLibSupport . LoadActiveRowsOnlyInGrid ( datagrid1 , Listcollection1 , colcount );
                                if ( maskcols . Content . ToString ( ) == "Mask Columns" )
                                    ColumntypesList = DapperLibSupport . ReplaceDataGridFldNames ( table , ref datagrid1 , ref dglayoutlist1 , colcount );
                                Debug . WriteLine ( $"LOADGENERICTABLE : {Listcollection1 . Count} records in [{table}] Loaded to datagrid1 in Task " );
                                CurrentTable1 = table;
                                GenericTitle1 . Text = Title1 = $"{CurrentTable1 . ToUpper ( )}";
                                datagrid1 . Focus ( );
                                SelectCorrectTable ( CurrentTable1 . ToUpper ( ) );

                                // datagrid2
                                datagrid2 . ItemsSource = Listcollection2;
                                DapperLibSupport . LoadActiveRowsOnlyInGrid ( datagrid2 , Listcollection2 , colcount );
                                if ( maskcols . Content . ToString ( ) == "Mask Columns" )
                                    DapperLibSupport . ReplaceDataGridFldNames ( table , ref datagrid2 , ref dglayoutlist2 , colcount );
                                Debug . WriteLine ( $"LOADGENERICTABLE : {Listcollection2 . Count} records in [{table}] Loaded to datagrid2 in Task " );
                                CurrentTable2 = table;
                                GenericTitle2 . Text = Title2 = $"{CurrentTable2 . ToUpper ( )}";
                                datagrid2 . Focus ( );
                                SelectCorrectTable ( CurrentTable2 . ToUpper ( ) );
                            } );
                        }
                    }
                } );
                if ( success == false || Listcollection2 . Count == 0 || Listcollection1 . Count == 0 )
                {
                    Debug . WriteLine ( $"Data load Task Failed.........." );
                    sw . Stop ( );
                    return false;
                }
                sw . Stop ( );
                return true;
            }
            else if ( ActiveGrid == 2 && Gencollection2 . Count == 0 )
            {
                // Called toi load data by Combo
                await Task . Run ( ( ) =>
                {
                    DapperGenLib . LoadTableGeneric ( $"Select * from {table}" , ref Gencollection2 );
                    if ( Gencollection2 == null )
                    {
                        $"FATAL ERROR - Unable to load Db table {table}" . cwerror ( );
                        success = false;
                    }
                } );
                if ( success == false )
                {
                    sw . Stop ( );
                    return false;
                }
                await Task . Run ( ( ) =>
                {
                    Application . Current . Dispatcher . Invoke ( ( ) =>
                        {
                            // get column count for Obscollection , no list
                            colcount = DapperLibSupport . GetGenericColumnCount ( Gencollection2 );
                            DapperLibSupport . LoadActiveRowsOnlyInGrid ( datagrid2 , Gencollection2 , DapperLibSupport . GetGenericColumnCount ( Gencollection2 ) );
                            DapperLibSupport . LoadActiveRowsOnlyInGrid ( datagrid2 , Gencollection2 , colcount );
                            if ( maskcols . Content . ToString ( ) == "Mask Columns" )
                                DapperLibSupport . ReplaceDataGridFldNames ( table , ref datagrid2 , ref dglayoutlist2 , colcount );
                            Debug . WriteLine ( $"LOADGENERICTABLE : {Gencollection2 . Count} records in [{table}] Loaded to datagrid2 in Task " );
                            CurrentTable2 = table;
                            if ( Gencollection2 . Count == 0 )
                            {
                                Debug . WriteLine ( $"Data load Task Failed.........." );
                                sw . Stop ( );
                                return false;
                            }
                            Debug . WriteLine ( $"Total Task took {sw . ElapsedMilliseconds} msecs" );
                            GenericTitle2 . Text = Title2 = $"{CurrentTable2 . ToUpper ( )}";
                            datagrid2 . Focus ( );
                            SelectCorrectTable ( CurrentTable2 . ToUpper ( ) );
                            sw . Stop ( );
                            return true;
                        } );
                } );        // end task
                sw . Stop ( );
                return true;
            }   // outer else
            sw . Stop ( );
            return true;
        }

        public static int GetColumnsCount ( List<DapperGenericsLib . GenericClass> list )
        {
            int counter = 1;
            int maxcol = 0;
            foreach ( var item in list )
            {
                // We only ever do this for a single record !!!!  Not all records, so pretty fast
                GenClass = item;
                switch ( counter )
                {
                    case 1:
                        maxcol = GenClass . field1 != null ? 0 : counter;
                        break;
                    case 2:
                        maxcol = GenClass . field2 != null ? 0 : counter;
                        break;
                    case 3:
                        maxcol = GenClass . field3 != null ? 0 : counter;
                        break;
                    case 4:
                        maxcol = GenClass . field4 != null ? 0 : counter;
                        break;
                    case 5:
                        maxcol = GenClass . field5 != null ? 0 : counter;
                        break;
                    case 6:
                        maxcol = GenClass . field6 != null ? 0 : counter;
                        break;
                    case 7:
                        maxcol = GenClass . field7 != null ? 0 : counter;
                        break;
                    case 8:
                        maxcol = GenClass . field8 != null ? 0 : counter;
                        break;
                    case 9:
                        maxcol = GenClass . field9 != null ? 0 : counter;
                        break;
                    case 10:
                        maxcol = GenClass . field10 != null ? 0 : counter;
                        break;
                    case 11:
                        maxcol = GenClass . field11 != null ? 0 : counter;
                        break;
                    case 12:
                        maxcol = GenClass . field12 != null ? 0 : counter;
                        break;
                    case 13:
                        maxcol = GenClass . field13 != null ? 0 : counter;
                        break;
                    case 14:
                        maxcol = GenClass . field14 != null ? 0 : counter;
                        break;
                    case 15:
                        maxcol = GenClass . field15 != null ? 0 : counter;
                        break;
                    case 16:
                        maxcol = GenClass . field16 != null ? 0 : counter;
                        break;
                    case 17:
                        maxcol = GenClass . field17 != null ? 0 : counter;
                        break;
                    case 18:
                        maxcol = GenClass . field18 != null ? 0 : counter;
                        break;
                    case 19:
                        maxcol = GenClass . field19 != null ? 0 : counter;
                        break;
                    case 20:
                        maxcol = GenClass . field20 != null ? 0 : counter;
                        break;
                }
                counter++;
                if ( maxcol != 0 )
                    break;
            }
            colcount = maxcol - 1;
            // Adjust to actual columns count
            return colcount;
        }
        public static void SelectCorrectTable ( string table )
        {
            int indx = 0;
            try
            {
                for ( int x = 0 ; x < Host . combo . comboBox . Items . Count ; x++ )
                {
                    if ( Host . combo . comboBox . Items [ x ] . ToString ( ) . ToUpper ( ) == table )
                    {
                        ComboboxPlus cbp = ComboboxPlus . GetCBP ( );
                        cbp . ReloadDb = false;
                        Host . combo . SelectActive = true;
                        Host . combo . comboBox . SelectedIndex = indx;
                        Host . combo . comboBox . SelectedItem = table;
                        Host . combo . comboBox . UpdateLayout ( );
                        Host . combo . SelectActive = false;
                        cbp . ReloadDb = true;
                        break;
                    }
                    indx++;
                }
            }
            catch ( Exception ex ) { Debug . WriteLine ( $"Failed to set DbTable in Combo : {ex . Message}" ); }
        }

        static public List<string> GetDbTablesList ( string DbName )
        {
            List<string> TablesList = new List<string> ( );
            string SqlCommand = "";
            List<string> list = new List<string> ( );
            DbName = DbName . ToUpper ( );
            if ( DapperLibSupport . CheckResetDbConnection ( DbName , out string constr ) == false )
            {
                Debug . WriteLine ( $"Failed to set connection string for {DbName} Db" );
                return TablesList;
            }
            // All Db's have their own version of this SP.....
            SqlCommand = "spGetTablesList";

            Datagrids . CallStoredProcedure ( list , SqlCommand );
            //This call returns us a DataTable
            DataTable dt = DapperLibSupport . GetDataTable ( SqlCommand );
            // This how to access Row data from  a grid the easiest way.... parsed into a List <xxxxx>
            if ( dt != null )
            {
                TablesList = DapperLibSupport . GetDataDridRowsAsListOfStrings ( dt );
            }
            return TablesList;

        }
        private void genkeydown ( object sender , KeyEventArgs e )
        {
            if ( sender . GetType ( ) is PopupListBox )
            {
                PopupListBox plb = ( PopupListBox ) sender;
            }
            if ( e . Key == Key . F8 )
            {
                Debugger . Break ( );
                datagrid1 . BeginEdit ( );
                // This WORKS = returns all DataGRids in selected Parent (this)
                var result = WpfLib1 . Utils . FindVisualChildren<DataGrid> ( this );
            }
            if ( e . Key == Key . F11 )
            {
                if ( PopupListBox . ResetPopup )
                    PopupListBox . ResetPopup = true;
                else
                    PopupListBox . ResetPopup = false;
            }
            if ( e . Key == Key . Escape )
            {   // used by Flowdoc (only)
                if ( Flowdoc != null )
                {
                    Flowdoc . Visibility = Visibility . Collapsed;
                    canvas . Visibility = Visibility . Collapsed;
                }
            }

        }
        private double oldheight { get; set; }
        private double oldwidth { get; set; }

        #region Buttons handlers

        private void Button1_Click ( object sender , RoutedEventArgs e )
        {
            // clear grid
            datagrid1 . ItemsSource = null;
            datagrid1 . Items . Clear ( );
            datagrid1 . Refresh ( );
            GenericTitle1 . Text = "";
        }

        // Toggle columhn header content
        private void Button2_Click ( object sender , RoutedEventArgs e )
        {
            Mouse . SetCursor ( Cursors . Wait );

            if ( maskcols . Content . ToString ( ) == "Mask Columns" )
            {
                // Hiide generic column names
                int indexer = 1;
                // Set llags  for how  next time around
                ShowColumnNames = true;
                if ( ActiveGrid == 1 )
                {
                    if ( ShowingHeaders1 == true ) ;
                    SetDefColumnHeaderText ( datagrid1 , true );
                    datagrid1 . UpdateLayout ( );
                    ShowingHeaders1 = false;
                }
                else if ( ActiveGrid == 2 )
                {
                    if ( ShowingHeaders2 == true ) ;
                    SetDefColumnHeaderText ( datagrid2 , true );
                    datagrid2 . UpdateLayout ( );
                    ShowingHeaders2 = false;
                }
                maskcols . Content = "Show Columns";
            }
            else
            {
                // Show true column names
                DapperGenericsLib . DataGridLayout dg = new DapperGenericsLib . DataGridLayout ( );
                DapperGenericsLib . GenericClass gc = new DapperGenericsLib . GenericClass ( );
                if ( ActiveGrid == 1 )
                {
                    if ( ShowingHeaders1 == false ) ;
                    {
                        DataGrid tmpgrid = datagrid1;
                        ObservableCollection<DapperGenericsLib . GenericClass> tmp = new ObservableCollection<DapperGenericsLib . GenericClass> ( );
                        for ( int y = 0 ; y < datagrid1 . Items . Count ; y++ )
                        {
                            gc = datagrid1 . Items [ y ] as DapperGenericsLib . GenericClass;
                            tmp . Add ( gc );
                        }
                        colcount = DapperLibSupport . GetGenericColumnCount ( tmp , gc );
                        DapperLibSupport . ReplaceDataGridFldNames ( CurrentTable1 , ref tmpgrid , ref dglayoutlist1 , colcount );
                        datagrid1 . UpdateLayout ( );
                        ShowingHeaders1 = true;
                    }
                }
                else if ( ActiveGrid == 2 )
                {
                    DataGrid tmpgrid = datagrid2;
                    ObservableCollection<DapperGenericsLib . GenericClass> tmp = new ObservableCollection<DapperGenericsLib . GenericClass> ( );
                    LoadGenericTable ( CurrentTable2 , "datagrid2" , false , tmp );
                    //for ( int y = 0 ; y < datagrid2 . Items . Count ; y++ )
                    //{

                    //    //DataGridRow dgr = datagrid2 . Items [ y ];
                    //    DataGridRow dgr = Utils . GetRow ( datagrid2 , y ) ;
                    //    gc = ( DapperGenericsLib . GenericClass ) dgr.c ;
                    //    tmp . Add ( gc );
                    //}
                    colcount = DapperLibSupport . GetGenericColumnCount ( tmp , gc );
                    DapperLibSupport . ReplaceDataGridFldNames ( CurrentTable2 , ref tmpgrid , ref dglayoutlist2 , colcount );
                    datagrid2 . UpdateLayout ( );
                    ShowingHeaders2 = true;
                }
                maskcols . Content = "Mask Columns";
            }
            Mouse . SetCursor ( Cursors . Arrow );
        }
        public void SetDefColumnHeaderText ( DataGrid grid , bool IsCollapsed )
        {
            int indexer = 1;
            grid . Visibility = Visibility . Visible;
            foreach ( var item in grid . Columns )
            {
                DataGridColumn dgc = item;
                if ( indexer <= grid . Columns . Count )
                    dgc . Header = $"Field{indexer++}";
            }
            grid . UpdateLayout ( );
            grid . Refresh ( );
            //if ( IsCollapsed ) grid . Visibility = Visibility . Collapsed;
        }

        private void AddNew ( object sender , RoutedEventArgs e )
        {
            if ( addnewrow . Content . ToString ( ) == "Add New" )
            {
                canUserAddRows = true;
                if ( ActiveGrid == 1 )
                {
                    //datagrid1 . ItemsSource = null;
                    //datagrid1 . Items . Clear ( );
                    //datagrid1 . UpdateLayout ( );
                    //datagrid1 . Refresh ( );
                    Gencollection1 . Add ( new DapperGenericsLib . GenericClass ( ) );
                    datagrid1 . AutoGenerateColumns = true;
                    DapperLibSupport . LoadActiveRowsOnlyInGrid ( datagrid1 , Gencollection1 , DapperLibSupport . GetGenericColumnCount ( Gencollection1 ) );
                    colcount = DapperLibSupport . GetGenericColumnCount ( Gencollection1 );
                    //                 DapperLibSupport . LoadActiveRowsOnlyInGrid ( datagrid1 , Gencollection1 , colcount );
                    if ( ShowColumnNames )
                        DapperLibSupport . ReplaceDataGridFldNames ( CurrentTable1 , ref datagrid1 , ref dglayoutlist1 , colcount );
                    datagrid1 . SelectedIndex = datagrid1 . Items . Count - 1;
                    datagrid1 . Refresh ( );
                    Utils . ScrollRecordIntoView ( datagrid1 , datagrid1 . SelectedIndex );
                    datagrid1 . UpdateLayout ( );
                    datagrid1 . Refresh ( );
                    updaterow . IsEnabled = false;
                    addnewrow . Content = "Save Account";
                    datagrid1 . Focus ( );
                }
                else
                {
                    Gencollection2 . Add ( new DapperGenericsLib . GenericClass ( ) );
                    DapperLibSupport . LoadActiveRowsOnlyInGrid ( datagrid2 , Gencollection1 , DapperLibSupport . GetGenericColumnCount ( Gencollection2 ) );
                    colcount = DapperLibSupport . GetGenericColumnCount ( Gencollection2 );
                    //                  DapperLibSupport . LoadActiveRowsOnlyInGrid ( datagrid2 , Gencollection2 , colcount );
                    if ( ShowColumnNames )
                        DapperLibSupport . ReplaceDataGridFldNames ( CurrentTable2 , ref datagrid2 , ref dglayoutlist2 , colcount );
                    datagrid2 . SelectedIndex = datagrid2 . Items . Count - 1;
                    datagrid2 . Refresh ( );
                    Utils . ScrollRecordIntoView ( datagrid2 , datagrid2 . SelectedIndex );
                    datagrid2 . UpdateLayout ( );
                    datagrid2 . Refresh ( );
                    updaterow . IsEnabled = false;
                    addnewrow . Content = "Save Account";
                    datagrid2 . Focus ( );
                }
            }
            else
            {
                // Save the new row
                updaterow . IsEnabled = true;
                addnewrow . Content = "Add New";
                // stop adding new records as default policy
                canUserAddRows = false;
                UpdateRecord ( sender , e );
            }
        }
        #endregion Buttons handlers

        public void UpdateRecord ( object sender , RoutedEventArgs e )
        {
            // Update record
            int MaxCols = 0;
            int index = 0;
            if ( ActiveGrid == 1 )
            {
                MaxCols = datagrid1 . Columns . Count;
            }
            else
            {
                MaxCols = datagrid2 . Columns . Count;
            }
            string [ ] fieldnames = new string [ MaxCols ];
            if ( ActiveGrid == 1 )
            {
                for ( int x = 0 ; x < MaxCols ; x++ )
                { fieldnames [ x ] = ""; }
                DapperGenericsLib . GenericClass GenClass1 = new DapperGenericsLib . GenericClass ( );
                //var GenClass1 = datagrid1 . SelectedItem as GenericClass;
                GenClass1 = datagrid1 . SelectedItem as DapperGenericsLib . GenericClass;
                //GenClass = GenClass1;
                foreach ( DataGridColumn dgc in datagrid1 . Columns )
                {
                    if ( dgc . Header . ToString ( ) . Contains ( "Field" ) == true )
                        break;
                    fieldnames [ index++ ] = dgc . Header . ToString ( );
                }
            }
            else
            {
                for ( int x = 0 ; x < MaxCols ; x++ )
                { fieldnames [ x ] = ""; }
                GenClass = datagrid2 . SelectedItem as DapperGenericsLib . GenericClass;
                foreach ( DataGridColumn dgc in datagrid2 . Columns )
                {
                    if ( dgc . Header . ToString ( ) . Contains ( "Field" ) == true )
                        break;
                    fieldnames [ index++ ] = dgc . Header . ToString ( );
                }
            }
            if ( MaxCols == 0 )
            {
                // Try to get correct column count
                if ( ActiveGrid == 1 )
                    MaxCols = DapperLibSupport . GetGenericColumnCount ( Listcollection1 , GenClass );
                if ( ActiveGrid == 2 )
                    MaxCols = DapperLibSupport . GetGenericColumnCount ( Listcollection2 , GenClass );
                if ( MaxCols == 0 )
                {
                    MessageBox . Show ( $"You need to have the 'TRUE' Column Names being used in \nthe datagrid if you want to update it on the Server\nUse the 'Show Columns' button to achieve this and then try the update again" , "Data Format Error" );
                    return;
                }
            }
            index = 0;
            //string [ ] fieldnames = new string [ MaxCols ];

            for ( int x = 0 ; x < MaxCols ; x++ )
            {
                if ( fieldnames [ x ] == null )
                {
                    index++;
                    break;
                }
                if ( fieldnames [ x ] . Contains ( "field" ) )
                    index++;
            }
            if ( index >= 1 )
            {
                MessageBox . Show ( $"You need to have the 'TRUE' Column Names for the table in use in \nthe selected datagrid if you want to update it on the Server\n\nUse the 'Show Columns' button to achieve this and then try the update again" , "Data Format Error" );
                return;
            }
            //string [ ] fldnames = new string [ index ];
            //for ( int x = 0 ; x < index ; x++ )
            //    fldnames [ x ] = fieldnames [ x ];

            //for ( int x = 0 ; x < index ; x++ )
            //{
            //    fldnames [ x ] = fieldnames [ x ];
            //}
            //MaxCols--;
            string cmd = CreateSqlCommand ( fieldnames , MaxCols );
            if ( ActiveGrid == 1 )
                UpdateGenericTable ( cmd , fieldnames , dglayoutlist1 , GenClass );
            else
                UpdateGenericTable ( cmd , fieldnames , dglayoutlist2 , GenClass );
        }
        public string CreateSqlCommand ( string [ ] fldnames , int MaxCols )
        {
            // create an SQL command string with all relevant update data included
            int count = 0;
            string cmd = $"UPDATE {CurrentTable1} SET ";
            if ( count < MaxCols )
            {
                for ( int x = 0 ; x < MaxCols ; x++ )
                {
                    //if ( x > 0 )
                    cmd += CheckForDate ( x );
                }
            }
            //            cmd += $" where {fldnames [ 2 ]}={fldnames [ 2 ]}";
            //            Debug . WriteLine ( $"SqlCommand is {cmd}" );
            return cmd;
        }
        public string CheckForDate ( int count )
        {
            // called by CreateSqlCommand above to create an Update Sql Command
            string cmdline = "";
            cmdline = $"{CreateCmdLine ( ref count , GetFieldValueFromIndex ( count ) )}";
            //          Debug . WriteLine ( $"Line[{count}]={cmdline}" );
            return cmdline;
        }

        public string CreateCmdLine ( ref int count , string fieldname )
        {
            // called by Checkfodate above
            // We are creating an Update command in thiis function 
            string cmdline = "";
            if ( ActiveGrid == 1 )
            {
                // special handling to exlcude "Id" from Sql command Generation  cos we CANNOT usaully update Id in any table
                // as it is almost always a Sql Db generated value
                cmdline = $"{dglayoutlist1 [ count ] . Fieldname}=";
                if ( dglayoutlist1 [ count ] . Fieldname == "Id" )
                {
                    cmdline = "";
                    return cmdline;
                }
                //else
                //{
                //    cmdline = $"{dglayoutlist1 [ count ] . Fieldname}=";
                //}
                if ( count <= 1 )
                {
                    if ( dglayoutlist1 [ count ] . Fieldtype == "nvarchar" )
                        cmdline += $"{GetFieldValueFromIndex ( count )}";
                    else if ( dglayoutlist1 [ count ] . Fieldtype == "date" || dglayoutlist1 [ count ] . Fieldtype == "datetime" )
                        cmdline += $"{GetFieldValueFromIndex ( count )}";
                    else
                        cmdline += $"{GetFieldValueFromIndex ( count )}";
                }
                else
                {
                    cmdline = $", {dglayoutlist1 [ count ] . Fieldname}=";
                    if ( dglayoutlist1 [ count ] . Fieldtype == "nvarchar" )
                        cmdline += $"{GetFieldValueFromIndex ( count )}";
                    else if ( dglayoutlist1 [ count ] . Fieldtype == "datetime" || dglayoutlist1 [ count ] . Fieldtype == "date" )
                    {
                        string fulldate = GetFieldValueFromIndex ( count );
                        string str = RemoveTimeFromDate ( fulldate );
                        cmdline += $"'{str}'";
                    }
                    else
                        cmdline += $"{GetFieldValueFromIndex ( count )}";
                }
                dglayoutlist1 [ count ] . DataValue = GetFieldValueFromIndex ( count );
            }
            else
            {
                if ( dglayoutlist2 [ count ] == null ) return cmdline;

                cmdline = $"{dglayoutlist2 [ count ] . Fieldname}=";
                if ( dglayoutlist2 [ count ] . Fieldname == "Id" )
                {
                    cmdline = "";
                    return cmdline;
                }
                if ( count == 1 )
                {
                    if ( dglayoutlist2 [ count ] . Fieldtype == "nvarchar" )
                        cmdline += $"{GetFieldValueFromIndex ( count )}";
                    else if ( dglayoutlist2 [ count ] . Fieldtype == "date" || dglayoutlist2 [ count ] . Fieldtype == "datetime" )
                        cmdline += $"{GetFieldValueFromIndex ( count )}";
                    else
                        cmdline += $"{GetFieldValueFromIndex ( count )}";
                }
                else
                {
                    cmdline = $", {dglayoutlist2 [ count ] . Fieldname}=";
                    if ( dglayoutlist2 [ count ] . Fieldtype == "nvarchar" )
                        cmdline += $"{GetFieldValueFromIndex ( count )}";
                    else if ( dglayoutlist2 [ count ] . Fieldtype == "datetime" || dglayoutlist2 [ count ] . Fieldtype == "date" )
                    {
                        string fulldate = GetFieldValueFromIndex ( count );
                        string str = RemoveTimeFromDate ( fulldate );
                        cmdline += $"{str}";
                    }
                    else
                        cmdline += $"{GetFieldValueFromIndex ( count )}";
                }
                dglayoutlist2 [ count ] . DataValue = GetFieldValueFromIndex ( count );
            }
            return cmdline;
        }

        public static bool UpdateGenericTable ( string SqlCommand , string [ ] fldnames , List<DapperGenericsLib . DataGridLayout> dglayoutlist , DapperGenericsLib . GenericClass GenClass )
        {
            bool success = false;
            int index = 0;
            SqlConnection con;
            SqlCommand cmd = null;

            string ConString = GenericDbUtilities . CheckSetSqlDomain ( "" );
            if ( ConString == "" )
            {
                GenericDbUtilities . CheckDbDomain ( "IAN1" );
                ConString = MainWindow . CurrentSqlTableDomain;
            }

            //List<Tuple<string , string , object>> fielddata = new List<Tuple<string , string , object>> ( );
            //for ( int x = 0 ; x < dglayoutlist . Count ; x++ )

            {
                //    Tuple<string , string , object> tuple;
                //    if ( x == 0 )
                //        tuple = new Tuple<string , string , object> ( dglayoutlist [ x ] . Fieldname , dglayoutlist [ x ] . Fieldtype , dglayoutlist [ x ] . DataValue );
                //    else if ( x == 1 )
                //        tuple = new Tuple<string , string , object> ( dglayoutlist [ x ] . Fieldname , dglayoutlist [ x ] . Fieldtype , dglayoutlist [ x ] . DataValue );
                //    else if ( x == 2 )
                //        tuple = new Tuple<string , string , object> ( dglayoutlist [ x ] . Fieldname , dglayoutlist [ x ] . Fieldtype , dglayoutlist [ x ] . DataValue );
                //    else if ( x == 3 )
                //        tuple = new Tuple<string , string , object> ( dglayoutlist [ x ] . Fieldname , dglayoutlist [ x ] . Fieldtype , dglayoutlist [ x ] . DataValue );
                //    else if ( x == 4 )
                //        tuple = new Tuple<string , string , object> ( dglayoutlist [ x ] . Fieldname , dglayoutlist [ x ] . Fieldtype , dglayoutlist [ x ] . DataValue );
                //    else if ( x == 5 )
                //        tuple = new Tuple<string , string , object> ( dglayoutlist [ x ] . Fieldname , dglayoutlist [ x ] . Fieldtype , dglayoutlist [ x ] . DataValue );
                //    else if ( x == 6 )
                //        tuple = new Tuple<string , string , object> ( dglayoutlist [ x ] . Fieldname , dglayoutlist [ x ] . Fieldtype , dglayoutlist [ x ] . DataValue );
                //    else if ( x == 7 )
                //        tuple = new Tuple<string , string , object> ( dglayoutlist [ x ] . Fieldname , dglayoutlist [ x ] . Fieldtype , dglayoutlist [ x ] . DataValue );
                //    else if ( x == 8 )
                //        tuple = new Tuple<string , string , object> ( dglayoutlist [ x ] . Fieldname , dglayoutlist [ x ] . Fieldtype , dglayoutlist [ x ] . DataValue );
                //    else if ( x == 9 )
                //        tuple = new Tuple<string , string , object> ( dglayoutlist [ x ] . Fieldname , dglayoutlist [ x ] . Fieldtype , dglayoutlist [ x ] . DataValue );
                //    else if ( x == 10 )
                //        tuple = new Tuple<string , string , object> ( dglayoutlist [ x ] . Fieldname , dglayoutlist [ x ] . Fieldtype , dglayoutlist [ x ] . DataValue );
                //    else if ( x == 11 )
                //        tuple = new Tuple<string , string , object> ( dglayoutlist [ x ] . Fieldname , dglayoutlist [ x ] . Fieldtype , dglayoutlist [ x ] . DataValue );
                //    else if ( x == 12 )
                //        tuple = new Tuple<string , string , object> ( dglayoutlist [ x ] . Fieldname , dglayoutlist [ x ] . Fieldtype , dglayoutlist [ x ] . DataValue );
                //    else if ( x == 13 )
                //        tuple = new Tuple<string , string , object> ( dglayoutlist [ x ] . Fieldname , dglayoutlist [ x ] . Fieldtype , dglayoutlist [ x ] . DataValue );
                //    else if ( x == 14 )
                //        tuple = new Tuple<string , string , object> ( dglayoutlist [ x ] . Fieldname , dglayoutlist [ x ] . Fieldtype , dglayoutlist [ x ] . DataValue );
                //    else if ( x == 15 )
                //        tuple = new Tuple<string , string , object> ( dglayoutlist [ x ] . Fieldname , dglayoutlist [ x ] . Fieldtype , dglayoutlist [ x ] . DataValue );
                //    else if ( x == 16 )
                //        tuple = new Tuple<string , string , object> ( dglayoutlist [ x ] . Fieldname , dglayoutlist [ x ] . Fieldtype , dglayoutlist [ x ] . DataValue );
                //    else if ( x == 17 )
                //        tuple = new Tuple<string , string , object> ( dglayoutlist [ x ] . Fieldname , dglayoutlist [ x ] . Fieldtype , dglayoutlist [ x ] . DataValue );
                //    else if ( x == 18 )
                //        tuple = new Tuple<string , string , object> ( dglayoutlist [ x ] . Fieldname , dglayoutlist [ x ] . Fieldtype , dglayoutlist [ x ] . DataValue );
                //    else if ( x == 19 )
                //        tuple = new Tuple<string , string , object> ( dglayoutlist [ x ] . Fieldname , dglayoutlist [ x ] . Fieldtype , dglayoutlist [ x ] . DataValue );
                //    else
                //        tuple = new Tuple<string , string , object> ( "" , "" , null );

                //    fielddata . Add ( tuple );
            }

            con = new SqlConnection ( ConString );
            try
            {
                using ( con )
                {
                    string tmp = "";
                    int intval = 0;
                    con . Open ( );
                    cmd = new SqlCommand ( SqlCommand , con );
                    Debug . WriteLine ( "Parameters are :" );
                    for ( int x = 1 ; x < dglayoutlist . Count ; x++ )
                    {
                        if ( dglayoutlist [ x ] . Fieldtype == "int" || dglayoutlist [ x ] . Fieldtype == "smallint" || dglayoutlist [ x ] . Fieldtype == "tinyint" ||
                             dglayoutlist [ x ] . Fieldtype == "bigint" || dglayoutlist [ x ] . Fieldtype == "numeric" )
                        {   // int
                            cmd . Parameters . AddWithValue ( dglayoutlist [ x ] . Fieldname , Convert . ToInt32 ( dglayoutlist [ x ] . DataValue ) );
                        }
                        else if ( dglayoutlist [ x ] . Fieldtype == "decimal" )
                        {   // int
                            cmd . Parameters . AddWithValue ( dglayoutlist [ x ] . Fieldname , Convert . ToDecimal ( dglayoutlist [ x ] . DataValue ) );
                        }
                        else if ( dglayoutlist [ x ] . Fieldtype == "numeric" )
                        {   // int
                            cmd . Parameters . AddWithValue ( dglayoutlist [ x ] . Fieldname , Convert . ToUInt64 ( dglayoutlist [ x ] . DataValue ) );
                        }
                        else if ( dglayoutlist [ x ] . Fieldtype == "float" || dglayoutlist [ x ] . Fieldtype == "real" )
                        {   // int
                            cmd . Parameters . AddWithValue ( dglayoutlist [ x ] . Fieldname , Convert . ToDouble ( dglayoutlist [ x ] . DataValue ) );
                        }
                        else if ( dglayoutlist [ x ] . Fieldtype == "date" || dglayoutlist [ x ] . Fieldtype == "datetime" )
                        {   // DateTime
                            string str = RemoveTimeFromDate ( dglayoutlist [ x ] . DataValue );
                            dglayoutlist [ x ] . DataValue = str;
                            cmd . Parameters . AddWithValue ( dglayoutlist [ x ] . Fieldname , $"TRY_CONVERT(DATE, {dglayoutlist [ x ] . DataValue}, 105) as [date], TRY_CAST('00:00:00' as [TIME]" );
                            //cmd . Parameters . AddWithValue ( dglayoutlist [ x ] . Fieldname , Convert.ToDateTime(dglayoutlist [ x ] . DataValue));
                        }
                        else if ( dglayoutlist [ x ] . Fieldtype == "bool" )
                        {   // boolean
                            cmd . Parameters . AddWithValue ( dglayoutlist [ x ] . Fieldname , Convert . ToBoolean ( dglayoutlist [ x ] . DataValue ) );
                        }
                        else if ( dglayoutlist [ x ] . Fieldtype == "nvarchar" || dglayoutlist [ x ] . Fieldtype == "nchar" ||
                            dglayoutlist [ x ] . Fieldtype == "ntext" || dglayoutlist [ x ] . Fieldtype == "text" ||
                            dglayoutlist [ x ] . Fieldtype == "char" || dglayoutlist [ x ] . Fieldtype == "nvarchar" || dglayoutlist [ x ] . Fieldtype == "varchar" )
                        {   // strings
                            string str = $"{dglayoutlist [ x ] . DataValue}";
                            cmd . Parameters . AddWithValue ( dglayoutlist [ x ] . Fieldname , str );
                        }
                        else if ( dglayoutlist [ x ] . Fieldtype == "object" )
                        {  // objects
                            cmd . Parameters . AddWithValue ( dglayoutlist [ x ] . Fieldname , ( object ) dglayoutlist [ x ] . DataValue );
                        }
                        else if ( dglayoutlist [ x ] . Fieldtype == "image" )
                        {  // objects
                            cmd . Parameters . AddWithValue ( dglayoutlist [ x ] . Fieldname , ( object ) dglayoutlist [ x ] . DataValue );
                        }
                        else
                        {  // misc Blob etc (objects)
                            cmd . Parameters . AddWithValue ( dglayoutlist [ x ] . Fieldname , ( object ) dglayoutlist [ x ] . DataValue );
                        }

                        Debug . WriteLine ( $"{dglayoutlist [ x ] . Fieldname} : {dglayoutlist [ x ] . DataValue}" );
                        foreach ( var item in cmd . Parameters )
                        {
                            Debug . WriteLine ( item . ToString ( ) ); ;
                        }
                    }
                    Debug . WriteLine ( $"[{SqlCommand}]" );
                    cmd . ExecuteNonQuery ( );
                    Debug . WriteLine ( "SQL Update of All Db's successful..." );
                    success = true;
                }
            }
            catch ( Exception ex )
            {
                // TODO fails  when date fields are "date" or "datetime" !!!!
                Debug . WriteLine ( $"BANKACCOUNT Update FAILED : {ex . Message}, {ex . Data}" );
                success = false;
            }
            finally
            {
                con . Close ( );
            }
            return success;
        }

        #region SQL parsing utilitites

        //====================================================================//
        public static string LocalDateToISODate ( string _strDate )
        {
            //CultureInfo _cinLocInfo = getCulture ( _strLocalLanguage );
            if ( IsLocalDate ( _strDate ) )
            {
                return String . Format ( "{0:yyyy/MM/dd}" , DateTime . Parse ( _strDate , null ) );
            }
            else
            {
                return null;
            }
        }
        public static bool IsLocalDate ( string _strDate )
        {
            //            CultureInfo _cinLocInfo = getCulture ( _strLocalLanguage );
            DateTime dtOut = new DateTime ( );
            return DateTime . TryParse ( _strDate , null , DateTimeStyles . AdjustToUniversal , out dtOut );
        }
        public static CultureInfo getCulture ( string _strLocalLanguage )
        {
            return CultureInfo . CreateSpecificCulture ( _strLocalLanguage );
        }

        public static string RemoveTimeFromDate ( object date )
        {
            var str = date . ToString ( );
            string [ ] items;
            items = str . Split ( "00:" );
            return items [ 0 ] . Trim ( );
        }
        //====================================================================//
        public static bool IsValidSqlDatetime ( string someval )
        {
            bool valid = false;
            DateTime testDate = DateTime . MinValue;
            DateTime minDateTime = DateTime . MaxValue;
            DateTime maxDateTime = DateTime . MinValue;

            minDateTime = new DateTime ( 1753 , 1 , 1 );
            maxDateTime = new DateTime ( 9999 , 12 , 31 , 23 , 59 , 59 , 997 );

            if ( DateTime . TryParse ( someval , out testDate ) )
            {
                if ( testDate >= minDateTime && testDate <= maxDateTime )
                    valid = true;
            }
            return valid;
        }
        public string GetFieldValueFromIndex ( int index )
        {
            // Return valid data for specidic column of data in generic table (field1-19)
            string result = "";
            if ( index == 0 ) return GenClass?.field1?.ToString ( );  // returns ID - not wanted??
            if ( index == 1 ) return GenClass?.field2?.ToString ( );
            if ( index == 2 ) return GenClass?.field3?.ToString ( );
            if ( index == 3 ) return GenClass?.field4?.ToString ( );
            if ( index == 4 ) return GenClass?.field5?.ToString ( );
            if ( index == 5 ) return GenClass?.field6?.ToString ( );
            if ( index == 6 ) return GenClass?.field7?.ToString ( );
            if ( index == 7 ) return GenClass?.field8?.ToString ( );
            if ( index == 8 ) return GenClass?.field9?.ToString ( );
            if ( index == 9 ) return GenClass?.field10?.ToString ( );
            if ( index == 10 ) return GenClass?.field11?.ToString ( );
            if ( index == 11 ) return GenClass?.field12?.ToString ( );
            if ( index == 12 ) return GenClass?.field13?.ToString ( );
            if ( index == 13 ) return GenClass?.field14?.ToString ( );
            if ( index == 14 ) return GenClass?.field15?.ToString ( );
            if ( index == 15 ) return GenClass?.field16?.ToString ( );
            if ( index == 16 ) return GenClass?.field17?.ToString ( );
            if ( index == 17 ) return GenClass?.field18?.ToString ( );
            if ( index == 18 ) return GenClass?.field19?.ToString ( );
            if ( index == 19 ) return GenClass?.field20?.ToString ( );
            else return "";
        }

        public static int GetValueFromIndex ( int x , DapperGenericsLib . GenericClass GenClass , DapperGenericsLib . DataGridLayout dgfieldvalues )
        {
            int val = 0;
            if ( dgfieldvalues . Fieldtype == "int" )
            {
                if ( x == 1 )
                    val = Convert . ToInt32 ( GenClass . field1 );
                else if ( x == 2 )
                    val = Convert . ToInt32 ( GenClass . field2 );
                else if ( x == 3 )
                    val = Convert . ToInt32 ( GenClass . field3 );
                else if ( x == 4 )
                    val = Convert . ToInt32 ( GenClass . field4 );
                else if ( x == 5 )
                    val = Convert . ToInt32 ( GenClass . field5 );
                else if ( x == 6 )
                    val = Convert . ToInt32 ( GenClass . field6 );
                else if ( x == 7 )
                    val = Convert . ToInt32 ( GenClass . field7 );
                else if ( x == 8 )
                    val = Convert . ToInt32 ( GenClass . field8 );
                else if ( x == 9 )
                    val = Convert . ToInt32 ( GenClass . field9 );
                else if ( x == 10 )
                    val = Convert . ToInt32 ( GenClass . field10 );
                else if ( x == 11 )
                    val = Convert . ToInt32 ( GenClass . field11 );
                else if ( x == 12 )
                    val = Convert . ToInt32 ( GenClass . field12 );
                else if ( x == 13 )
                    val = Convert . ToInt32 ( GenClass . field13 );
                else if ( x == 14 )
                    val = Convert . ToInt32 ( GenClass . field14 );
                else if ( x == 15 )
                    val = Convert . ToInt32 ( GenClass . field15 );
                else if ( x == 16 )
                    val = Convert . ToInt32 ( GenClass . field16 );
                else if ( x == 17 )
                    val = Convert . ToInt32 ( GenClass . field17 );
                else if ( x == 18 )
                    val = Convert . ToInt32 ( GenClass . field18 );
                else if ( x == 19 )
                    val = Convert . ToInt32 ( GenClass . field19 );
                else if ( x == 20 )
                    val = Convert . ToInt32 ( GenClass . field20 );
            }
            return val;
        }
        #endregion SQL parsing utilitites



        public async void Togglegrid_Click ( object sender , RoutedEventArgs e )
        {
            StyleArgs args = new StyleArgs ( );
            Thickness th = new Thickness ( );
            Thickness thhost = new Thickness ( );
            thhost . Right = Host . Width;
            thhost . Bottom = Host . Height;

            if ( ActiveGrid == 1 )
            {
                //Switching to Grid 2
                GenericGrid2 = datagrid2;
                ActiveGrid = 2;
                Togglegrid . Content = " < Grid 1";
                datagrid2 . BringIntoView ( );
                datagrid2 . Focus ( );

                datagrid2 . UpdateLayout ( );
                if ( ShowingHeaders2 == true )
                    UpdateTitleAndStyle ( this , "BANKACCOUNT" , datagrid2 , thhost , Title2 , null );
            }
            else
            {
                //Switching to Grid 1
                //Identify current tab early on
                ActiveGrid = 1;
                GenericGrid1 = datagrid1;
                datagrid1 . BringIntoView ( );
                datagrid1 . Focus ( );
                Togglegrid . Content = "Grid 2 >";
                if ( ShowingHeaders1 == true )
                    UpdateTitleAndStyle ( this , "BANKACCOUNT" , datagrid1 , thhost , Title1 , null );
            }
        }

        public void UpdateTitleAndStyle ( GenericGridControl Ctrl , string table , DataGrid grid , Thickness thhost , string title , string style )
        {
            // Handle grid  switching, Title bar, Toggle button text, , Column names and grid Style in selected grid
            Thickness th = new Thickness ( );
            thhost . Right = Host . Width;
            thhost . Bottom = Host . Height;

            if ( maskcols . Content . ToString ( ) == "Mask Columns" )
            {
                foreach ( var item in grid . Columns )
                {
                    // don't bother if they are  already displayed
                    if ( item . Header == "field1" )
                        GenericDbUtilities . ReplaceDataGridFldNames ( table , ref grid );
                    break;
                }
            }
            if ( thhost . Right != 0 && thhost . Bottom != 0 )
            {
                grid . Height = Host . BankContent . Height > 0 ? Host . BankContent . Height - 90 : 0;
                //th = grid . Margin;
                //th. Top = 35;
                //grid . Margin = th;
                grid . Width = thhost . Right - 230;
            }
            else
            {
                th = grid . Margin;
                th . Bottom = 50;
                th . Right = 20;
                grid . Margin = th;
            }
            if ( title != null && title != "" )
            {
                if ( grid . Name == "datagrid1" )
                    GenericTitle1 . Text = title;
                else
                    GenericTitle2 . Text = title;
            }
            else
            {
                if ( grid . Name == "datagrid1" )
                    GenericTitle1 . Text = table;
                else
                    GenericTitle2 . Text = title;

            }
            //Handle Grid Style  here
            //if ( style == "Dark Mode" )
            //{
            //    grid . Foreground = FindResource ( "White0" ) as SolidColorBrush;
            //    grid . Background = FindResource ( "Transparent0" ) as SolidColorBrush;
            //    SelectCurrentStyle ( style );
            //}
            //else
            //{
            //    Style newstyle = FindResource ( style ) as Style;
            //    grid . CellStyle = newstyle;
            //    SelectCurrentStyle ( style );
            //}
            grid . UpdateLayout ( );
            grid . Refresh ( );
            ComboboxPlus cbp = ComboboxPlus . GetCBP ( );
            cbp . ReloadDb = false;
            //Update Sql Table combo to match this grids content
            if ( grid . Name == "datagrid1" )
                SelectCorrectTable ( GenericTitle1 . Text );
            else
                SelectCorrectTable ( GenericTitle2 . Text );
            cbp . ReloadDb = true;
        }


        public void SelectCurrentStyle ( string Style )
        {
            int indx = 0;

        }

        #region DP's

        public SelectionMode Selectionmode
        {
            get { return ( SelectionMode ) GetValue ( SelectionmodeProperty ); }
            set { SetValue ( SelectionmodeProperty , value ); }
        }
        public static readonly DependencyProperty SelectionmodeProperty =
            DependencyProperty . Register ( "Selectionmode" , typeof ( SelectionMode ) , typeof ( GenericGridControl ) , new PropertyMetadata ( SelectionMode . Multiple ) );

        public bool autoGenerateColumns
        {
            get { return ( bool ) GetValue ( autoGenerateColumnsProperty ); }
            set { SetValue ( autoGenerateColumnsProperty , value ); }
        }
        public static readonly DependencyProperty autoGenerateColumnsProperty =
            DependencyProperty . Register ( "autoGenerateColumns" , typeof ( bool ) , typeof ( GenericGridControl ) , new PropertyMetadata ( false ) );

        public bool canUserAddRows
        {
            get { return ( bool ) GetValue ( canUserAddRowsProperty ); }
            set { SetValue ( canUserAddRowsProperty , value ); }
        }
        public static readonly DependencyProperty canUserAddRowsProperty =
            DependencyProperty . Register ( "canUserAddRows" , typeof ( bool ) , typeof ( GenericGridControl ) , new PropertyMetadata ( false ) );

        public Brush Rowbackground
        {
            get { return ( Brush ) GetValue ( RowbackgroundProperty ); }
            set { SetValue ( RowbackgroundProperty , value ); }
        }
        public static readonly DependencyProperty RowbackgroundProperty =
           DependencyProperty . Register ( "Rowbackground" , typeof ( Brush ) , typeof ( GenericGridControl ) , new PropertyMetadata ( Brushes . Transparent ) );

        public Brush background
        {
            get { return ( Brush ) GetValue ( backgroundProperty ); }
            set { SetValue ( backgroundProperty , value ); }
        }
        public static readonly DependencyProperty backgroundProperty =
           DependencyProperty . Register ( "background" , typeof ( Brush ) , typeof ( GenericGridControl ) , new PropertyMetadata ( Brushes . Gray ) );

        public Brush foreground
        {
            get { return ( Brush ) GetValue ( foregroundProperty ); }
            set { SetValue ( foregroundProperty , value ); }
        }
        public static readonly DependencyProperty foregroundProperty =
           DependencyProperty . Register ( "foreground" , typeof ( Brush ) , typeof ( GenericGridControl ) , new PropertyMetadata ( Brushes . White ) );

        public Thickness margin
        {
            get { return ( Thickness ) GetValue ( marginProperty ); }
            set { SetValue ( marginProperty , value ); }
        }
        public static readonly DependencyProperty marginProperty =
           DependencyProperty . Register ( "margin" , typeof ( Thickness ) , typeof ( GenericGridControl ) , new PropertyMetadata ( new Thickness { Left = 10 , Top = 10 , Bottom = 10 , Right = 10 } ) );

        public double fontsize
        {
            get { return ( double ) GetValue ( fontsizeProperty ); }
            set { SetValue ( fontsizeProperty , value ); }
        }
        public static readonly DependencyProperty fontsizeProperty =
            DependencyProperty . Register ( "fontsize" , typeof ( double ) , typeof ( GenericGridControl ) , new PropertyMetadata ( ( double ) 15 ) );

        public ScrollBarVisibility vscrollBar
        {
            get { return ( ScrollBarVisibility ) GetValue ( vscrollBarProperty ); }
            set { SetValue ( vscrollBarProperty , value ); }
        }
        public static readonly DependencyProperty vscrollBarProperty =
            DependencyProperty . Register ( "vscrollBar" , typeof ( ScrollBarVisibility ) ,
                typeof ( GenericGridControl ) , new PropertyMetadata ( ( ScrollBarVisibility ) 1 ) );

        #endregion DP's

        #region Edit/Add/Update
        private void dgProducts_AddingNewItem ( object sender , AddingNewItemEventArgs e )
        { isInsertMode = true; }
        private void dgProducts_BeginningEdit ( object sender , DataGridBeginningEditEventArgs e )
        { isBeingEdited = true; }
        private void dgProducts_RowEditEnding ( object sender , DataGridRowEditEndingEventArgs e )
        { }
        private void dgProducts_PreviewKeyDown1 ( object sender , KeyEventArgs e )
        {
            ActiveGrid = 1;
            //datagrid1 . Focus ( );
        }
        private void dgProducts_PreviewKeyDown2 ( object sender , KeyEventArgs e )
        {
            datagrid2 . Focus ( );
            ActiveGrid = 2;
        }
        #endregion Edit/Add/Update

        #region UNUSED METHODS

        private void CreateGenericColumns ( int maxcols , DataGrid grid1 )
        {
            grid1 . Columns . Clear ( );
            //Debug . WriteLine ( $"CREATING CUSTOMER COLUMNS" );
            DataGridTextColumn c1 = new DataGridTextColumn ( );
            c1 . Header = "Field 1";
            c1 . Binding = new Binding ( "field1" );
            grid1 . Columns . Add ( c1 );
            if ( maxcols == 1 ) return;
            DataGridTextColumn c2 = new DataGridTextColumn ( );
            c2 . Header = "Field 2";
            c2 . Binding = new Binding ( "field2" );
            grid1 . Columns . Add ( c2 );
            if ( maxcols == 2 ) return;
            DataGridTextColumn c3 = new DataGridTextColumn ( );
            c3 . Header = "Field 3";
            c3 . Binding = new Binding ( "field3" );
            grid1 . Columns . Add ( c3 );
            if ( maxcols == 3 ) return;
            DataGridTextColumn c4 = new DataGridTextColumn ( );
            c4 . Header = "Field 4";
            c4 . Binding = new Binding ( "field4" );
            grid1 . Columns . Add ( c4 );
            if ( maxcols == 4 ) return;
            DataGridTextColumn c5 = new DataGridTextColumn ( );
            c5 . Header = "Field 5";
            c5 . Binding = new Binding ( "field5" );
            grid1 . Columns . Add ( c5 );
            if ( maxcols == 5 ) return;
            DataGridTextColumn c6 = new DataGridTextColumn ( );
            c6 . Header = "Field 6";
            c6 . Binding = new Binding ( "field6" );
            grid1 . Columns . Add ( c6 );
            if ( maxcols == 6 ) return;
            DataGridTextColumn c7 = new DataGridTextColumn ( );
            c7 . Header = "Field 7";
            c7 . Binding = new Binding ( "field7" );
            grid1 . Columns . Add ( c7 );
            if ( maxcols == 7 ) return;
            DataGridTextColumn c8 = new DataGridTextColumn ( );
            c8 . Header = "Field 8";
            c8 . Binding = new Binding ( "field8" );
            grid1 . Columns . Add ( c8 );
            if ( maxcols == 8 ) return;
            DataGridTextColumn c9 = new DataGridTextColumn ( );
            c9 . Header = "Field 9";
            c9 . Binding = new Binding ( "field9" );
            grid1 . Columns . Add ( c9 );
            if ( maxcols == 9 ) return;
            DataGridTextColumn c10 = new DataGridTextColumn ( );
            c10 . Header = "Field 10";
            c10 . Binding = new Binding ( "field10" );
            grid1 . Columns . Add ( c10 );
            if ( maxcols == 10 ) return;
            DataGridTextColumn c11 = new DataGridTextColumn ( );
            c11 . Header = "Field 11";
            c11 . Binding = new Binding ( "field11" );
            grid1 . Columns . Add ( c11 );
            if ( maxcols == 11 ) return;
            DataGridTextColumn c12 = new DataGridTextColumn ( );
            c12 . Header = "Field 12";
            c12 . Binding = new Binding ( "field12" );
            grid1 . Columns . Add ( c12 );
            if ( maxcols == 12 ) return;
            DataGridTextColumn c13 = new DataGridTextColumn ( );
            c13 . Header = "Field 13";
            c13 . Binding = new Binding ( "field13" );
            grid1 . Columns . Add ( c13 );
            if ( maxcols == 13 ) return;
            DataGridTextColumn c14 = new DataGridTextColumn ( );
            c14 . Header = "Field 14";
            c14 . Binding = new Binding ( "field14" );
            grid1 . Columns . Add ( c14 );
            if ( maxcols == 14 ) return;
            DataGridTextColumn c15 = new DataGridTextColumn ( );
            c15 . Header = "Field 15";
            c15 . Binding = new Binding ( "field15" );
            grid1 . Columns . Add ( c15 );
            if ( maxcols == 15 ) return;
            DataGridTextColumn c16 = new DataGridTextColumn ( );
            c16 . Header = "Field 16";
            c16 . Binding = new Binding ( "field16" );
            grid1 . Columns . Add ( c16 );
            if ( maxcols == 16 ) return;
            DataGridTextColumn c17 = new DataGridTextColumn ( );
            c17 . Header = "Field 17";
            c17 . Binding = new Binding ( "field17" );
            grid1 . Columns . Add ( c17 );
            if ( maxcols == 17 ) return;
            DataGridTextColumn c18 = new DataGridTextColumn ( );
            c18 . Header = "Field 18";
            c18 . Binding = new Binding ( "field18" );
            grid1 . Columns . Add ( c18 );
            if ( maxcols == 18 ) return;
            DataGridTextColumn c19 = new DataGridTextColumn ( );
            c19 . Header = "Field 19";
            c19 . Binding = new Binding ( "field19" );
            grid1 . Columns . Add ( c19 );
            if ( maxcols == 19 ) return;
            DataGridTextColumn c20 = new DataGridTextColumn ( );
            c20 . Header = "Field 20";
            c20 . Binding = new Binding ( "field20" );
            grid1 . Columns . Add ( c20 );
            if ( maxcols == 20 ) return;
            DataGridTextColumn c21 = new DataGridTextColumn ( );
            c21 . Header = "Field 21";
            c21 . Binding = new Binding ( "field21" );
            grid1 . Columns . Add ( c21 );
            if ( maxcols == 21 ) return;
            DataGridTextColumn c22 = new DataGridTextColumn ( );
            c21 . Header = "Field 22";
            c21 . Binding = new Binding ( "field22" );
            grid1 . Columns . Add ( c22 );
            if ( maxcols == 22 ) return;
            DataGridTextColumn c23 = new DataGridTextColumn ( );
            c21 . Header = "Field 23";
            c21 . Binding = new Binding ( "field23" );
            grid1 . Columns . Add ( c23 );
            if ( maxcols == 23 ) return;
            DataGridTextColumn c24 = new DataGridTextColumn ( );
            c21 . Header = "Field 24";
            c21 . Binding = new Binding ( "field24" );
            grid1 . Columns . Add ( c24 );
        }

        public DapperGenericsLib . GenericClass ConvertClass ( DapperGenericsLib . GenericClass GCollection , DapperGenericsLib . GenericClass collection )
        {
            collection . field1 = GCollection . field1;
            collection . field2 = GCollection . field2;
            collection . field3 = GCollection . field3;
            collection . field4 = GCollection . field4;
            collection . field5 = GCollection . field5;
            collection . field6 = GCollection . field6;
            collection . field7 = GCollection . field7;
            collection . field8 = GCollection . field8;
            collection . field9 = GCollection . field9;
            collection . field10 = GCollection . field10;
            collection . field11 = GCollection . field11;
            collection . field12 = GCollection . field12;
            collection . field13 = GCollection . field13;
            collection . field14 = GCollection . field14;
            collection . field15 = GCollection . field15;
            collection . field16 = GCollection . field16;
            collection . field17 = GCollection . field17;
            collection . field18 = GCollection . field18;
            collection . field19 = GCollection . field19;
            collection . field20 = GCollection . field20;
            return collection;
        }

        public static DataTable GetDataTableCount ( string Tablename , DataTable dt , out int count )
        {
            count = 0;
            //DataSet ds = new DataSet ( "GenericDataSet" );
            //ds . Tables . Add ( dt );
            //string ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
            //using ( SqlConnection conn = new SqlConnection ( ConString ) ) {
            //    SqlCommand cmd = conn . CreateCommand ( );
            //    cmd . CommandType = CommandType . Text;
            //    cmd . CommandText = $"SELECT * FROM {Tablename}";
            //    SqlDataAdapter da = new SqlDataAdapter ( cmd );
            //    da . Fill ( ds );
            //}
            //dt = CreateTableColumns ( ds . Tables [ 1 ] . Columns . Count );
            //count = dt . Columns . Count;
            return dt;
        }
        public static DataTable GetDataTable ( string Tablename , DataTable dt )
        {
            //DataSet ds = new DataSet ( "GenericDataSet" );
            //ds . Tables . Add ( dt );
            //string ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
            //using ( SqlConnection conn = new SqlConnection ( ConString ) ) {
            //    SqlCommand cmd = conn . CreateCommand ( );
            //    cmd . CommandType = CommandType . Text;
            //    cmd . CommandText = $"SELECT * FROM {Tablename}";
            //    SqlDataAdapter da = new SqlDataAdapter ( cmd );
            //    da . Fill ( ds );
            //}
            //return ds . Tables [ 1 ];
            return dt;
        }
        private static List<T> ConvertDataTableToList<T> ( DataTable dt , out int Columncount )
        {
            List<T> data = new List<T> ( );
            Columncount = 0;
            foreach ( DataRow row in dt . Rows )
            {
                T item = GetItem<T> ( row );
                data . Add ( item );
                Columncount++;
            }
            return data;
        }
        private static T GetItem<T> ( DataRow dr )
        {
            Type temp = typeof ( T );
            T obj = Activator . CreateInstance<T> ( );
            int index = 0;
            //foreach ( DataColumn column in dr . Table . Columns ) {
            for ( int x = 0 ; x < dr . Table . Columns . Count - 1 ; x++ )
            {
                index = 0;
                foreach ( PropertyInfo pro in temp . GetProperties ( ) )
                {
                    if ( index < dr . Table . Columns . Count )
                        pro . SetValue ( obj , dr [ index++ ] . ToString ( ) );
                    else break;
                    //else
                    //    continue;
                }
                //                Columncount = x;
                return obj;
            }
            return obj;
        }
        private static DataTable CreateTableColumns ( int max )
        {
            int counter = 0;
            dt = new DataTable ( );
            {   // unused
                //dt . Columns . Add ( "field1" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field2" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field3" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field4" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field5" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field6" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field7" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field8" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field9" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field10" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field11" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field12" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field13" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field14" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field15" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field16" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field17" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field18" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field19" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field20" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field21" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field22" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field23" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field24" , typeof ( string ) );
            }
            return dt;
        }
        private void CreateDgStyleslist ( )
        {
            datagrid1 . CellStyle = FindResource ( "xxxtypeGridStyle" ) as Style;

        }

        #endregion UNUSED METHODS 


        private void MouseLeftButtonDown1 ( object sender , MouseButtonEventArgs e )
        {
            // change focus of the grid
            datagrid1 . IsReadOnly = false;
            ActiveGrid = 1;
            datagrid1 . Focus ( );
            Togglegrid . Content = "Grid 2 >";
            string s = "";
            if ( GenericTitle1 . Text . Contains ( "Table = " ) )
                s = GenericTitle1 . Text . Substring ( 8 );
            else
                s = GenericTitle1 . Text;
            GenericGridControl . SelectCorrectTable ( s );
            SelectCorrectTable ( s );
        }
        private void MouseLeftButtonDown2 ( object sender , MouseButtonEventArgs e )
        {
            // change focus of the grid
            datagrid2 . IsReadOnly = false;
            ActiveGrid = 2;
            datagrid2 . Focus ( );
            Togglegrid . Content = "< Grid 1";
            string s = "";
            if ( GenericTitle2 . Text . Contains ( "Table = " ) )
                s = GenericTitle2 . Text . Substring ( 8 );
            else
                s = GenericTitle2 . Text;
            GenericGridControl . SelectCorrectTable ( s );
            //            Debug . WriteLine ( $"Edit mode = {result . ToString ( )}" );
            SelectCorrectTable ( s );
        }


        #region  Resizing handlers
        public void GenGridControl_SizeChanged ( object sender , SizeChangedEventArgs e )
        {
            //Working
            double splitterht = 0;
            //Width
            SplitterGrid . UpdateLayout ( );
            double Widthchange = e . NewSize . Width - e . PreviousSize . Width;
            SplitterGrid . Width = e . NewSize . Width;
            datagrid1 . Width = e . NewSize . Width - 30;
            datagrid2 . Width = datagrid1 . Width;// - 30;

            SplitterGrid . Height = e . NewSize . Height;
            SplitterGrid . UpdateLayout ( );
            splitterht = SplitterGrid . Height;
            double Offset1 = SplitterGrid . RowDefinitions [ 0 ] . ActualHeight;
            double Offset2 = SplitterGrid . RowDefinitions [ 1 ] . ActualHeight;

            //Height
            if ( Offset1 != 0 )
            {
                // Change container grid size 1st
                double newtotalheight = e . NewSize . Height;
                SplitterGrid . Height = newtotalheight;
                double dbl1 = SplitterGrid . RowDefinitions [ 0 ] . ActualHeight;
                double dbl2 = SplitterGrid . RowDefinitions [ 1 ] . ActualHeight;
                datagrid1 . Height = dbl1 - 25;
                if ( newtotalheight - Offset1 - 100 > 0 )
                    datagrid2 . Height = newtotalheight - Offset1 - 100;
            }
            datagrid1 . UpdateLayout ( );
            datagrid2 . UpdateLayout ( );
            Startup = false;
            //           Debug . WriteLine ( $"Resizing  dg1 of {splitterht} ={datagrid1 . Height} dg2={datagrid2 . Height} - {row1 . ActualHeight}" );
        }
        private void Splitter_DragStarted ( object sender , DragStartedEventArgs e )
        {
            //WORKING
            double ht = SplitterGrid . Height;
            double Offset1 = SplitterGrid . RowDefinitions [ 0 ] . ActualHeight;
            MaxTopHeight = ht - Offset1;
            double Offset2 = ht - Offset1;
            datagrid1 . Height = Offset1 - 20;
            datagrid2 . Height = Offset2 - 100;
        }
        private void Splitter_DragCompleted ( object sender , DragCompletedEventArgs e )
        {
            // This  works correctly
            double ht = SplitterGrid . Height;
            double Offset1 = SplitterGrid . RowDefinitions [ 0 ] . ActualHeight;
            double bottomheight = SplitterGrid . RowDefinitions [ 1 ] . ActualHeight;
            double Offset2 = ht - Offset1;
            if ( bottomheight > 130 )
            {
                try
                {
                    if ( e . VerticalChange > 0 )
                    {   // grid1 growing change PLUS
                        datagrid1 . Height = Offset1 - 20;
                        datagrid2 . Height = Offset2 - 100;
                    }
                    else
                    {   // grid2 growing, change MINUS
                        datagrid1 . Height = Offset1 - 20;
                        datagrid2 . Height = Offset2 - 100;
                    }
                }
                catch ( Exception ex ) { }
                double CtrlHeight = this . ActualHeight;
            }
            datagrid1 . UpdateLayout ( );
            datagrid2 . UpdateLayout ( );
            IsMousedown = false;
            e . Handled = true;
        }
        private void SplitterHeightChanged ( object sender , EventArgs e )
        {
            // Handles update of datagrid sizing (height) in real time as splitter is dragged up/down
            // WORKING
            double ht1 = SplitterGrid . RowDefinitions [ 0 ] . ActualHeight - 20;
            double ht2 = SplitterGrid . RowDefinitions [ 1 ] . ActualHeight;
            if ( ht2 < 100 )
            {
                if ( SplitterGrid . RowDefinitions [ 0 ] . ActualHeight - 20 > 0 )
                    datagrid1 . Height = ht1;
                if ( SplitterGrid . RowDefinitions [ 1 ] . ActualHeight - 110 > 0 )
                    datagrid2 . Height = ht2;// - 110;
            }
            else
            {
                if ( SplitterGrid . RowDefinitions [ 0 ] . ActualHeight - 30 > 0 )
                    datagrid1 . Height = SplitterGrid . RowDefinitions [ 0 ] . ActualHeight - 20;
                if ( SplitterGrid . RowDefinitions [ 1 ] . ActualHeight - 100 > 0 )
                    datagrid2 . Height = SplitterGrid . RowDefinitions [ 1 ] . ActualHeight - 100;
            }
            datagrid1 . UpdateLayout ( );
            datagrid2 . UpdateLayout ( );
        }
        private void Splitter_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
        {
            IsMousedown = true;
        }
        private void Splitter_PreviewMouseLeftButtonUp ( object sender , MouseButtonEventArgs e )
        {
            IsMousedown = false;

        }

        #endregion  Resizing handlers

        private void datagrid1_GotFocus ( object sender , RoutedEventArgs e )
        {
            GenericTitle1 . Background = FindResource ( "Green5" ) as SolidColorBrush;
            GenericTitle1 . Foreground = FindResource ( "Black0" ) as SolidColorBrush;
            GenericTitle2 . Background = FindResource ( "Red4" ) as SolidColorBrush;
            GenericTitle2 . Foreground = FindResource ( "White0" ) as SolidColorBrush;

        }
        private void datagrid2_GotFocus ( object sender , RoutedEventArgs e )
        {
            GenericTitle1 . Background = FindResource ( "Red4" ) as SolidColorBrush;
            GenericTitle1 . Foreground = FindResource ( "White0" ) as SolidColorBrush;
            GenericTitle2 . Background = FindResource ( "Green5" ) as SolidColorBrush;
            GenericTitle2 . Foreground = FindResource ( "Black0" ) as SolidColorBrush;
            e . Source = datagrid2;

            //Grid_Selected ( sender ,e );
        }
        private void Grid_Selected ( object sender , RoutedEventArgs e )
        {
            if ( e . OriginalSource . GetType ( ) == typeof ( DataGridRow ) )
            {
                DataGridCell cell = ( e . OriginalSource as DataGridCell );
                if ( cell . IsEditing == true )
                {
                    DataGrid grd1 = ( DataGrid ) sender;
                    dgProducts_RowEditEnding ( sender , null );
                }
                //if ( cell . IsReadOnly == true )
                //    cell . IsReadOnly = false;
                //// Starts the Edit on the row;
                DataGrid grd = ( DataGrid ) sender;
                grd . BeginEdit ( e );
                cell . IsEditing = true;
                string cellValue = GetSelectedCellValue ( );
            }
        }

        public string GetSelectedCellValue ( )
        {

            DataGridCellInfo cells = datagrid1 . SelectedCells [ 0 ];

            string columnName = cells . Column . SortMemberPath;

            var item = cells . Item;

            if ( item == null || columnName == null ) return null;

            object result = item . GetType ( ) . GetProperty ( columnName ) . GetValue ( item , null );

            if ( result == null ) return null;

            return result . ToString ( );
        }


        private void DeleteRecord ( object sender , RoutedEventArgs e )
        {
            DataGridCell dgc = new DataGridCell ( );
            DataGrid grid = new DataGrid ( );
            grid = datagrid1;
            //           grid . PreparingCellForEdit += DataGrid_PreparingCellForEdit;

            if ( grid . BeginEdit ( ) )
            {

                dgc = e . OriginalSource as DataGridCell;
            }
        }

        private void datagrid1_AutoGeneratingColumn ( object sender , DataGridAutoGeneratingColumnEventArgs e )
        {
            DataGridColumn dgc = e . Column;
            dgc . IsReadOnly = false;
        }

        private void datagrid1_PreviewMouseDoubleClick ( object sender , MouseButtonEventArgs e )
        {
            datagrid1 . BeginEdit ( );
        }

        private void datagrid2_PreviewMouseDoubleClick ( object sender , MouseButtonEventArgs e )
        {
            DataGrid dgrid = sender as DataGrid;
            DataGrid source = e . Source as DataGrid;
            int selindex = source . SelectedIndex;
            Type type = source . GetType ( );
            var dr = dgrid . CurrentItem as DataGridRow;
            string cellname = dgrid . CurrentCell . Column . Header . ToString ( );
            var currvalue = dgrid . CurrentCell . Column . GetValue ( ContentProperty );
            datagrid2 . BeginEdit ( );
        }

        bool ShowFullScript = false;
        bool showall = true;

        private void viewschema ( object sender , RoutedEventArgs e )
        {
            string str = "";
            if ( ActiveGrid == 1 )
                str = GetSpArgs ( CurrentTable1 );
            else
                str = GetSpArgs ( CurrentTable2 );
        }

        private string [ ] CreateColumnBuffer ( string [ ] input )
        {
            string [ ] outbuffer = new string [ 6 ];
            for ( int x = 0 ; x < 6 ; x++ )
            {
                outbuffer [ x ] = input [ x ];
            }
            return outbuffer;
        }
        public string GetSpArgs ( string spName , bool showfull = false )
        {
            string output = "";
            string errormsg = "";
            int columncount = 0;
            DataTable dt = new DataTable ( );
            ObservableCollection<DapperGenericsLib . GenericClass> Generics = new ObservableCollection<DapperGenericsLib . GenericClass> ( );
            //        ObservableCollection<BankAccountViewModel> bvmparam = new ObservableCollection<BankAccountViewModel> ( );
            List<string> genericlist = new List<string> ( );
            try
            {
                //Generics = DapperGenLib . CreateGenericCollection (
                //"spGetTableColumnWithSizes" ,
                //$"{spName}" ,
                //"" ,
                //"" ,
                // ref errormsg );

                string ConString = GenericDbUtilities . CheckSetSqlDomain ( "" );
                if ( ConString == "" )
                {
                    // Sanity check - set to our local definition
                    ConString = MainWindow . SqlCurrentConstring;
                }
                // call our generic support lib to get SQL data as DataTable
                dt = GenDapperQueries . GetSqlDataAsDataTable ( "spGetTableColumnWithSizes " + spName , ConString );
                if ( dt . Rows . Count == 0 )
                    columncount = 0;
                foreach ( var item in dt . Rows )
                {
                    string store = "";
                    DataRow dr = item as DataRow;
                    columncount = dr . ItemArray . Count ( );
                    if ( columncount == 0 )
                        columncount = 1;
                    // we only need to process max cols - 1 here !!!
                    for ( int x = 0 ; x < columncount ; x++ )
                        store += dr . ItemArray [ x ] . ToString ( ) + ",";
                    output += store;
                }
            }
            catch ( Exception ex )
            {
                MessageBox . Show ( $"SQL ERROR 1125 - {ex . Message}" );
                return "";
            }

            if ( showfull == false )
            {
                // we now have the result, so lets process them
                // data is fieldname, sql-datatype, size (where appropriate), decimal
                string [ ] lines = output . Split ( ',' );
                output = $"{lines [ 0 ] . ToUpper ( )} :\n";
                int counter = 1;
                int currindex = 1;
                string message = "";
                try
                {
                    string [ ] buffitem = new string [ 6 ];
                    while ( currindex < lines . Length )
                    {
                        buffitem [ 0 ] = "";
                        buffitem [ 1 ] = lines [ currindex++ ];
                        buffitem [ 2 ] = lines [ currindex++ ];
                        buffitem [ 3 ] = lines [ currindex++ ];
                        buffitem [ 4 ] = lines [ currindex++ ];
                        buffitem [ 5 ] = lines [ currindex++ ];
                        counter = 1;
                        foreach ( var item in buffitem )
                        {
                            if ( item != "" && item != "0" )
                                output += $"{item}, ";
                            message += output;
                            output = "";
                        }
                        message = message . Trim ( ) . Substring ( 0 , message . Length - 1 );
                        message += "\n";
                        currindex++;
                    }
                }
                catch ( Exception ex )
                {
                    MessageBox . Show ( $"SQL ERROR 1125 - {ex . Message}" );
                    return "";
                }

                // we now have a list of the Args for the selected SP in output
                // Show it in a TextBox if it takes 1 or more args
                // format is ("fielddname, fieldtype, size1, size2\n,")
                if ( message != "" )
                {
                    string fdinput = $"Procedure Name : {spName . ToUpper ( )}\n\n";
                    fdinput += message;
                    fdinput += $"\nPress ESCAPE to close this window...\n";
                    //                    FlowDoc fdl = new FlowDoc ();
                    if ( fdl == null )
                        fdl = new FlowdocLib ( Flowdoc , canvas );
                    fdl . ShowInfo ( Flowdoc , canvas , line1: fdinput , clr1: "Black0" , line2: "" , clr2: "Black0" , line3: "" , clr3: "Black0" , header: "" , clr4: "Black0" );
                    canvas . Visibility = Visibility . Visible;
                    Flowdoc . Visibility = Visibility . Visible;
                }
                else
                {
                    Mouse . OverrideCursor = Cursors . Arrow;
                    //Utils . Mbox ( this , string1: $"Procedure [{Storedprocs . SelectedItem . ToString ( ) . ToUpper ( )}] \ndoes not Support / Require any arguments" , string2: "" , caption: "" , iconstring: "\\icons\\Information.png" , Btn1: MB . OK , Btn2: MB . NNULL , defButton: MB . OK );
                    //               fdl . ShowInfo ( Flowdoc , canvas , line1: $"Procedure [{Storedprocs . SelectedItem . ToString ( ) . ToUpper ( )}] \ndoes not Support / Require any arguments" , clr1: "Black0" , line2: "" , clr2: "Black0" , line3: "" , clr3: "Black0" , header: "" , clr4: "Black0" );
                }
            }
            return output;
        }


        #region FlowDoc support
        /// <summary>
        ///  These are the only methods any window needs to provide support for my FlowDoc system.

        // This is triggered/Broadcast by FlowDoc so that the parent controller can Collapse the 
        // Canvas so it  does not BLOCK other controls after being closed.
        private void Flowdoc_FlowDocClosed ( object sender , EventArgs e )
        {
            canvas . Visibility = Visibility . Collapsed;
        }
        protected void MaximizeFlowDoc ( object sender , EventArgs e )
        {
            // Clever "Hook" method that Allows the flowdoc to be resized to fill window
            // or return to its original size and position courtesy of the Event declard in FlowDoc
            fdl . MaximizeFlowDoc ( Flowdoc , canvas , e );
        }
        private void Flowdoc_MouseLeftButtonUp ( object sender , MouseButtonEventArgs e )
        {
            // Window wide  !!
            // Called  when a Flowdoc MOVE has ended
            MovingObject = fdl . Flowdoc_MouseLeftButtonUp ( sender , Flowdoc , MovingObject , e );
            ReleaseMouseCapture ( );
        }
        // CALLED WHEN  LEFT BUTTON PRESSED
        private void Flowdoc_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
        {
            //In this event, we get current mouse position on the control to use it in the MouseMove event.
            MovingObject = fdl . Flowdoc_PreviewMouseLeftButtonDown ( sender , Flowdoc , e );
            Debug . WriteLine ( $"MvvmDataGrid Btn down {MovingObject}" );
        }
        private void Flowdoc_MouseMove ( object sender , MouseEventArgs e )
        {
            // We are Resizing the Flowdoc using the mouse on the border  (Border.Name=FdBorder)
            fdl . Flowdoc_MouseMove ( Flowdoc , canvas , MovingObject , e );
        }

        // Shortened version proxy call		
        private void Flowdoc_LostFocus ( object sender , RoutedEventArgs e )
        {
            Flowdoc . BorderClicked = false;
        }

        public void FlowDoc_ExecuteFlowDocBorderMethod ( object sender , EventArgs e )
        {
            // EVENTHANDLER to Handle resizing
            FlowDoc fd = sender as FlowDoc;
            Point pt = Mouse . GetPosition ( canvas );
            double dLeft = pt . X;
            double dTop = pt . Y;
        }

        private void LvFlowdoc_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
        {
            //In this event, we get current mouse position on the control to use it in the MouseMove event.
            MovingObject = fdl . Flowdoc_PreviewMouseLeftButtonDown ( sender , Flowdoc , e );
        }

        public void fdmsg ( string line1 , string line2 = "" , string line3 = "" )
        {
            //We have to pass the Flowdoc.Name, and Canvas.Name as well as up   to 3 strings of message
            //  you can  just provie one if required
            // eg fdmsg("message text");
            fdl . FdMsg ( Flowdoc , canvas , line1 , line2 , line3 );
        }

        #endregion Flowdoc support via library
        //=================================================================//

        private void Flowdoc_PreviewKeyDown ( object sender , KeyEventArgs e )
        {
            if ( e . Key == Key . Escape )
            {
                Flowdoc . Visibility = Visibility . Collapsed;
                canvas . Visibility = Visibility . Collapsed;
            }
        }

        private void datagrid1_PreviewMouseRightButtonUp ( object sender , MouseButtonEventArgs e )
        {
            string output = "", temp = "";
            string [ ] lines, vals;
            string colName = "";
            int maxCols = 0, index = 0;
            //MapperConfiguration cfg;
            // fetch records data anddisplayin FlowDoc

            var genrecord = datagrid1 . SelectedItem . ToString ( );
            maxCols = datagrid1 . Columns . Count;
            string [ ] names = new string [ maxCols ];
            foreach ( var item in datagrid1 . Columns )
            {
                temp = item . Header . ToString ( );
                names [ index++ ] = temp;
            }
            genrecord = genrecord . Substring ( 1 , genrecord . Length - 2 );
            lines = genrecord . Split ( "," );
            index = 0;
            foreach ( var item in lines )
            {
                if ( index == maxCols )
                    break;
                vals = item . Split ( "=" );
                output += names [ index ] . PadRight ( 20 - names [ index ] . Length ) + " = " + vals [ 1 ] + "\n";
                index++;
            }
            if ( fdl == null ) fdl = new FlowdocLib ( Flowdoc , canvas );
            fdl . FdMsg ( Flowdoc , canvas , line1: "Current Record Data is :\n" + output , line2: "" , line3: "" , true );
            canvas . Visibility = Visibility . Visible;
            Flowdoc . Visibility = Visibility . Visible;

            output = output . Substring ( 0 , output . Length - 3 ) + "\nPress Escape to close viewer...";
        }

        private void Splitter_MouseEnter ( object sender , MouseEventArgs e )
        {
            Mouse . OverrideCursor = Cursors . SizeNS;
        }

        private void Splitter_MouseLeave ( object sender , MouseEventArgs e )
        {
            Mouse . OverrideCursor = Cursors . Arrow;

        }

        public Point GetNewCanvasOffset ( object sender , Point pt )
        {
            pt = Mouse . GetPosition ( canvas );
            return pt;
        }
        public void canvas_MouseMove ( object sender , MouseEventArgs e )
        {
            //Debug . WriteLine ("HostCanvas move hit.....");
            Point pt = new Point ( );
            GetNewCanvasOffset ( sender , pt );
        }

        private void clrtb_GotKeyboardFocus ( object sender , KeyboardFocusChangedEventArgs e )
        {
            Background = FindResource ( "White0" ) as SolidColorBrush;
        }

        private void clrtb_LostKeyboardFocus ( object sender , KeyboardFocusChangedEventArgs e )
        {
            Background = FindResource ( "White5" ) as SolidColorBrush;

        }
    }
}
