﻿#define SHOWSPS
using System;
using System . Collections;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Data;
using System . Diagnostics;
using System . Dynamic;
using System . Linq;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Controls . Primitives;
using System . Windows . Documents;
using System . Windows . Input;
using System . Windows . Media;


using Dapper;

using Expandos;

using NewWpfDev;
using NewWpfDev . Dapper;
using NewWpfDev . Models;
using NewWpfDev . UserControls;
using NewWpfDev . ViewModels;

using StoredProcs;

//using StoredProcs;

//using StoredProcs;

using UserControls;

using File = System . IO . File;
using Flags = NewWpfDev . Views . Flags;
using GenericClass = NewWpfDev . GenericClass;
using Point = System . Windows . Point;
using SqlConnection = Microsoft . Data . SqlClient . SqlConnection;

namespace Views
{
    /// <summary>
    ///Genericgrid.xaml provides a host Window to demonstate my DataGrid UserControl "DatagridControl"
    /// the UserControl supports Toggling Grid Column headers between Generic "field1..2..3.." and the True Field names 
    /// by simply calling the method GetFullColumnInfo ( string Current Table name, string Sql DbConnectionString );
    /// The UserControl also provides my FlowDoc UserControl that displays all types of Text Messages in  a highly configrable Message box
    /// </summary>
    public partial class Genericgrid : Window
    {
        //public static string DbConnectionString = "Data Source=DINO-PC;Initial Catalog=\"IAN1\";Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        #region OnPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged ( string PropertyName )
        {
            if ( this . PropertyChanged != null )
            {
                var e = new PropertyChangedEventArgs ( PropertyName );
                this . PropertyChanged ( this , e );
            }
        }

        #endregion OnPropertyChanged

        #region global object initialization 

        //public ObservableCollection<GenericClass> GridData = new ObservableCollection< GenericClass> ( );
        public ObservableCollection<GenericClass> GridData = new ObservableCollection<GenericClass> ( );
        public ObservableCollection<GenericClass> collection = new ObservableCollection<GenericClass> ( );
        public ObservableCollection<GenericClass> ColumnsData = new ObservableCollection<GenericClass> ( );
        static public DragCtrlHelper DragCtrl;
        public static FlowDocument myFlowDocument = new FlowDocument ( );
        //public static DragCtrlHelper dch;
        public static Paragraph para1 = new Paragraph ( );
        public List<Dictionary<string , string>> ColumntypesList = new List<Dictionary<string , string>> ( );
        public List<DapperGenericsLib . DataGridLayout> dglayoutlist = new List<DapperGenericsLib . DataGridLayout> ( );
        public List<DapperGenericsLib . DataGridLayout> DGLAYOUTLIST = new List<DapperGenericsLib . DataGridLayout> ( );
        public ObservableCollection<Database> DatabasesCollection = new ObservableCollection<Database> ( );
        public static dynamic Gengridexpobj { get; set; }
        public Dictionary<string , List<DapperGenericsLib . DataGridLayout>> TableLayoutDict = new Dictionary<string , List<DapperGenericsLib . DataGridLayout>> ( );
        #endregion global object initialization 

        #region static Domain variables        

        // NB : Must be declared as shown, including it's name;
        public static string CurrentTableDomain = "IAN1";
        public static string DBprefix { get; set; } = "IAN1.DBO.";
        // set by SelectDbWin when a domain switch occurs
        public static bool DomainChanged { get; set; } = false;

        #endregion static Domain variables        

        #region  Properties 

        public static MouseButtonState mbs { get; set; }
        public static string SpSearchTerm { get; set; } = "";
        public static string CurrentSpSelection { get; set; }
        public static SpResultsViewer spviewer { get; set; }
        public static Genericgrid GenGrid { get; set; }
        public bool ShowColumnHeaders { get; set; } = true;
        public List<int> SelectedRows = new List<int> ( );
        public int FlowdocVerticalpos { get; set; } = 0;
        public double Splitterleftpos { get; set; }
        public double Splitterlastpos { get; set; }
        public bool SPViewerOpen { get; set; } = false;
        public bool DatabaseChanged { get; set; } = false;
        bool SplistRightclick { get; set; } = false;
        string SpLastSelection { get; set; } = "";
        public FrameworkElement ActiveDragControl { get; set; }
        static public SpResultsViewer Resultsviewer { get; set; }
        static public bool UsingMatches { get; set; }
        static public string ResultsListLoadType { get; set; }
        double RTwidth { get; set; }
        #endregion Properties

        //This is Updated by my Grid Control whenever it loads a different table
        #region Flags initialization 

        public static string LastActiveFillter = "";
        public static string LastActiveTable = "";
        public static string NewTableSelection = "";

        public static string CurrentSpList = "ALL";
        public static string Currentpanel = "GRID";
        public bool TableIsEmpty = false;
        public bool LOCALFILTER { get; set; } = false;

        #endregion flags initialization 

        #region full properties
        public static bool UseFlowdoc { get; set; }
        public static FlowdocLib fdl { get; set; }

        //store for the full SP text
        private string spTextBuffer;
        public string SpTextBuffer
        {
            get { return spTextBuffer; }
            set { spTextBuffer = value; }
        }

        // store  for SP arguments alone
        private string spArgstext;
        public string SpArgsText
        {
            get { return spArgstext; }
            set { spArgstext = value; }
        }

        private object movingobject;
        public object MovingObject
        {
            get { return movingobject; }
            set
            { movingobject = value; }
        }
        private object movingobject2;
        public object MovingObject2
        {
            get { return movingobject2; }
            set { movingobject2 = value; }
        }
        #endregion full properties

        // HSPLITTER stuff
        #region Splitter properties

        double hSplitterlastpos { get; set; }
        double hSplitterbottompos { get; set; }
        double ViewerHeight { get; set; }
        double RTHeight { get; set; }

        #endregion Splitter properties

        #region variables initialization 

        static public DatagridControl dgControl;
        static public Genericgrid GenControl;
        public DataGrid Dgrid;
        public int ColumnsCount = 0;
        public bool bStartup = true;
        public static string NewSelectedTableName = "";
        public bool InfoViewerShown = false;
        public static bool USERRTBOX = true;
        public static bool IsMoving = false;
        public double MAXLISTWIDTH = 250;

        #endregion variables initialization 

        #region FULL Bindable  properties

        private bool dataLoaded;
        public bool DataLoaded
        {
            get { return dataLoaded; }
            set { dataLoaded = value; OnPropertyChanged ( "DataLoaded" ); }
        }

        private string currentTable;
        public string CurrentTable
        {
            get { return currentTable; }
            set { currentTable = value; OnPropertyChanged ( "CurrentTable" ); }
        }

        #endregion FULL Bindable properties

        #region Attached properties

        public static int GetToolTipDelayBetweenShow ( DependencyObject obj )
        { return ( int ) obj . GetValue ( ToolTipDelayBetweenShow ); }
        public static void SetToolTipDelayBetweenShow ( DependencyObject obj , int value )
        { obj . SetValue ( ToolTipDelayBetweenShow , value ); }
        public static readonly DependencyProperty ToolTipDelayBetweenShow =
            DependencyProperty . Register ( "ToolTipDelayBetweenShow" , typeof ( int ) , typeof ( Genericgrid ) , new PropertyMetadata ( 5000 ) );

        #endregion Attached properties

        #region Dependency properties

        public string SPExecPrompt
        {
            get { return ( string ) GetValue ( SPExecPromptProperty ); }
            set { SetValue ( SPExecPromptProperty , value ); }
        }
        public static readonly DependencyProperty SPExecPromptProperty =
            DependencyProperty . Register (
                "SPExecPrompt" ,
                typeof ( string ) ,
                typeof ( Genericgrid ) ,
                new PropertyMetadata ( "Enter any arguments that the Stored Procedure requires here and Click Execute" ) );

        public string SPExecuteText
        {
            get { return ( string ) GetValue ( SPExecuteTextProperty ); }
            set { SetValue ( SPExecuteTextProperty , value ); }
        }
        public static readonly DependencyProperty SPExecuteTextProperty =
            DependencyProperty . Register ( "SPExecuteText" , typeof ( string ) , typeof ( Genericgrid ) , new PropertyMetadata ( "Execute currently selected S.P" ) );

        public bool IsLoading
        {
            get { return ( bool ) GetValue ( IsLoadingProperty ); }
            set { SetValue ( IsLoadingProperty , value ); }
        }
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty . Register ( "IsLoading" , typeof ( bool ) , typeof ( Genericgrid ) , new PropertyMetadata ( true ) );

        public bool ListReloading
        {
            get { return ( bool ) GetValue ( ListReloadingProperty ); }
            set { SetValue ( ListReloadingProperty , value ); }
        }
        public static readonly DependencyProperty ListReloadingProperty =
            DependencyProperty . Register ( "ListReloading" , typeof ( bool ) , typeof ( Genericgrid ) , new PropertyMetadata ( false ) );
        public string infotext
        {
            get { return ( string ) GetValue ( infotextProperty ); }
            set { SetValue ( infotextProperty , value ); }
        }
        public static readonly DependencyProperty infotextProperty =
            DependencyProperty . Register ( "infotext" , typeof ( string ) , typeof ( Genericgrid ) , new PropertyMetadata ( "Information text goes here ...." ) );
        public string Searchtext
        {
            get { return ( string ) GetValue ( SearchtextProperty ); }
            set { SetValue ( SearchtextProperty , value ); }
        }
        public static readonly DependencyProperty SearchtextProperty =
            DependencyProperty . Register ( "Searchtext" , typeof ( string ) , typeof ( Genericgrid ) , new PropertyMetadata ( "@Arg" ) );
        public bool ExecuteLoaded
        {
            get { return ( bool ) GetValue ( ExecuteLoadedProperty ); }
            set { SetValue ( ExecuteLoadedProperty , value ); }
        }
        public static readonly DependencyProperty ExecuteLoadedProperty =
            DependencyProperty . Register ( "ExecuteLoaded" , typeof ( bool ) , typeof ( Genericgrid ) , new PropertyMetadata ( ( bool ) false ) );

        //-----------------------------------------------------------//
        #endregion Dependency properties
        //-----------------------------------------------------------//

        #region COMMANDS
        public ICommand FilterStoredprocs;
        public ICommand CloseFilterStoredprocs;
        #endregion COMMANDS

        //========================================================================================//
        public Genericgrid ( )
        //========================================================================================//
        {
            Mouse . OverrideCursor = Cursors . Wait;
            InitializeComponent ( );
            this . DataContext = this;

            WpfLib1 . Utils . SetupWindowDrag ( this );

            GenericGridSupport . SetPointers ( null , this );
            GenControl = this;
            CurrentTable = "BankAccount";
            // Sort out domain and default table
            CurrentTableDomain = "IAN1";
            DatagridControl . CurrentTableDomain = CurrentTableDomain;
            MainWindow . CurrentActiveTable = "BANKACCOUNT";
            string Con = NewWpfDev . Utils . GetCheckCurrentConnectionString ( CurrentTableDomain );
            DragCtrl = new DragCtrlHelper ( this );
            //dch = new DragCtrlHelper ( this);
            dgControl = this . GenGridCtrl;
            LastActiveTable = "";
            Dgrid = dgControl . datagridControl;
            ShowColumnHeaders = true;
            //// This call only loads the list of database Tables 
            Mouse . OverrideCursor = Cursors . Wait;
            ToggleColumnHeaders . IsChecked = ShowColumnHeaders;
            ColumnsCount = Dgrid . Columns . Count;
            bStartup = false;
            DatagridControl . SetParent ( ( Control ) this );
            Flags . UseScrollView = false;
            //            fdl = new FlowdocLib ( Flowdoc , Filtercanvas );
            NewTableName . Text = "qwerty";
            OptionsList . SelectedIndex = 0;
            ViewerGrid . ColumnDefinitions [ 0 ] . Width = new GridLength ( 1 , GridUnitType . Pixel );
            ViewerGrid . ColumnDefinitions [ 2 ] . Width = new GridLength ( 1 , GridUnitType . Star );

            // Ensure GridViewer panel is hidden on startup
            dgControl . datagridControl . Visibility = Visibility . Visible;
            ToolTipService . SetBetweenShowDelay ( Vsplitter , 5000 );
            int myInt = ToolTipService . GetBetweenShowDelay ( ( DependencyObject ) FindName ( "Vsplitter" ) );
            Vsplitter . SetValue ( ToolTipDelayBetweenShow , myInt );
            MovingObject = Filtering;

            // This call only loads the list of database Tables 
            LoadDbTables ( MainWindow . CurrentActiveTable );
            Mouse . OverrideCursor = Cursors . Wait;
            // setup various controls  visibility
            Filtercanvas . Visibility = Visibility . Visible;
            this . UpdateLayout ( );
            Mouse . OverrideCursor = Cursors . Wait;

            FilterStoredprocs = new RelayCommand ( ExecuteFilterStoredprocs , CanExecuteFilterStoredprocs );
            CloseFilterStoredprocs = new RelayCommand ( ExecuteCloseFilterStoredprocs , CanExecuteCloseFilterStoredprocs );
            this . Title = $"Generic SQL Tables - Active Database = {CurrentTableDomain . ToUpper ( )}";
            caption . Text = $"Current Table Status / Processing information : Current Database = {CurrentTableDomain . ToUpper ( )}";
            ///Setup default SP match term
            Searchtext = "@Arg";

            // Finally we load the tables info into the datagrid
            LoadInfo ( );
            //LoadInitialData ( );
            //test ExpandoObject  
            //ExpandoObject expobj2 = new ExpandoObject ( );
            //bool success = false;
            //expobj2 = ExpandoClass . expobjAdd ( expobj2 , "Phone" , "0757 9062440" , out success );
            //expobj2 = ExpandoClass . expobjFind ( expobj2 , "Phone" );
            //ExpandoClass . expobj = expobj2;
            //if ( ExpandoClass . expobj != null )
            //{
            //    // returns just the found item
            //    expobj2 = ExpandoClass . expobjFind ( ExpandoClass . expobj , "Phone" );
            //    if ( expobj2 != null )
            //    {
            //        Dictionary<string , object> dict = new Dictionary<string , object> ( );
            //        //var v = expobj2 . ToString ( );
            //        foreach ( KeyValuePair<string , object> item in expobj2 )
            //        {
            //            if ( item . Key == "Phone" )
            //            {
            //                Debug . WriteLine ( $"Expando : Phone =  {item . Value} " );
            //                break;
            //            }
            //        }
            //    }
            //}
            this . Show ( );
            Mouse . OverrideCursor = Cursors . Arrow;
            //            this . Refresh ( );
        }

        public async Task LoadInfo ( )
        {
            Task . Factory . StartNew ( async ( ) => LoadInitialData ( ) );
        }
        private async void LoadInitialData ( )
        {
            Task . Run ( ( ) =>
            {
                Application . Current . Dispatcher . Invoke ( ( ) =>
              {
                  // load db table data
                  Reccount . Text = GridData . Count . ToString ( );
                  int colcount = 0;
                  colcount = DatagridControl . GetColumnsCount ( GridData );
                  DatagridControl . LoadActiveRowsOnlyInGrid ( dgControl . datagridControl , GridData , colcount );
                  // Force column renaming on initial loading - Honestly !!
                  DatagridControl . ReplaceDataGridFldNames ( GridData , ref dgControl . datagridControl , ref dglayoutlist , colcount );
                  GenGrid = this;
                  dgControl . datagridControl . UpdateLayout ( );
              } );
            } );

        }
        private async void Grid_Loaded ( object sender , RoutedEventArgs e )
        {
            // Show main datagrid and info viewer
            Mouse . OverrideCursor = Cursors . Wait;
            InfoGrid . Visibility = Visibility . Visible;
            InfoBorder . Visibility = Visibility . Visible;
            RTBox . Visibility = Visibility . Visible;
            dgControl . datagridControl . Visibility = Visibility . Visible;
            majorgrid . RowDefinitions [ 0 ] . Height = new GridLength ( 2 , GridUnitType . Star );
            majorgrid . RowDefinitions [ 2 ] . Height = new GridLength ( 0 , GridUnitType . Star );
            GenGridCtrl . Height = majorgrid . ActualHeight;
            Gengrid_SizeChanged ( null , null );
            //this . Show ( );
            //this . Refresh ( );

            //            // This call only loads the list of database Tables 
            //            LoadDbTables ( MainWindow . CurrentActiveTable );

#if SHOWSPS
            ShowInfo ( sender , e );
#endif

            if ( Splist . SelectedIndex == -1 )
                Splist . SelectedIndex = 0;

            fdl = new FlowdocLib ( Flowdoc , Filtercanvas );

            //TestExpando ( );

            return;
        }

        public static dynamic GetStringFromDynamic ( IEnumerable<dynamic> dynovalue )
        {
            dynamic newresults = "";
            string output = "";
            foreach ( IDictionary<string , object> kvp in dynovalue )
            {
                foreach ( var item in kvp )
                {
                    Debug . WriteLine ( item . Key + ": " + item . Value );
                    if ( item . Value != null && item . ToString ( ) . Length > 0 )
                    {
                        output += $"{item . Value as dynamic},";

                    }
                }
                output += "\r\n";
            }
            newresults = output;
            return newresults;
        }

        public List<DapperGenericsLib . DataGridLayout> GetNewColumnsLayout ( string tablename , ObservableCollection<GenericClass> griddata , out int recordcount )
        {
            // All working as defined 1/11/22
            File . Delete ( @"c:\users\ianch\documents\CW.log" );
            //$"calling LoadDbAsGenericData" . CW ( );
            string error = "";
            recordcount = 0;

            if ( griddata == null )
            {
                Debug . WriteLine ( $"Data Load failed : [ {error} ]" );
                "" . Track ( 1 );
                return null;
            }
            ToggleColumnHeaders . IsChecked = true;
            if ( tablename != "" )
                UpdateSqlTableList ( tablename );
            int result = 0;
            string Tablename = "";
            string err = "";
            string command = "";

            GetValidDomain ( );
            command = $"drop table if exists {DBprefix}x_1";
            dgControl . ExecuteDapperCommand ( command , null , out err );
            //            var dapperresult = dgControl . ExecuteDapperTextCommand ( command , null , out err );
            if ( err != "" )
            {
                Debug . WriteLine ( $"MAJOR PROBLEM - Drop table failed.\nAborting load, Error = [{err}]" );
                return null;
            }
            command = $"select column_name, data_type, character_maximum_length, numeric_precision, numeric_scale into {DBprefix}x_1 from information_schema.columns where table_name='{tablename}'";
            ObservableCollection<GenericClass> columnsinfo = new ObservableCollection<GenericClass> ( );
            dgControl . ExecuteDapperCommand ( command , null , out err );
            //dgControl . ExecuteDapperTextCommand ( command , null , out err );
            if ( err == "" )
            {
                command = $"Select * from {DBprefix}{tablename}";
                //command = $"spLoadTableAsGeneric {tablename}";
                string [ ] args = new string [ 1 ];
                args [ 0 ] = tablename;
                dynamic dynresult = dgControl . ExecuteDapperCommand ( command , args , out err );
                //                dynamic dynresult = dgControl . ExecuteDapperTextCommand ( command , args , out err );
            }
            else
            {
                Debug . WriteLine ( $"MAJOR PROBLEM - Data selection 1 failed \nAborting load, Error = [{err}]" );
                return null;
            }

            // Finally Get columns info data
            command = $"select column_name, data_type, character_maximum_length, numeric_precision, numeric_scale from {DBprefix}x_1";
            columnsinfo = dgControl . GetDataFromStoredProcedure ( command , null , CurrentTableDomain , out err , out recordcount , 1 );
            if ( columnsinfo == null )
            {
                Debug . WriteLine ( $"MAJOR PROBLEM - Final Data selection failed \nAborting load, Error = [{err}]" );
                return null;
            }
            else
            {
                dglayoutlist = CreateColumnsLayout ( columnsinfo );
            }
            command = $"drop table if exists {DBprefix}x_1";
            dgControl . ExecuteDapperCommand ( command , null , out err );
            //            var res = dgControl . ExecuteDapperTextCommand ( command , null , out err );
            if ( err != "" )
            {
                Debug . WriteLine ( $"MAJOR PROBLEM - Drop table failed.\nAborting load, Error = [{err}]" );
                return null;
            }
            return dglayoutlist;
        }

        public List<DapperGenericsLib . DataGridLayout> CreateColumnsLayout ( ObservableCollection<GenericClass> columnsinfo )
        {
            List<DapperGenericsLib . DataGridLayout> dglayoutlist = new List<DapperGenericsLib . DataGridLayout> ( );

            foreach ( var item in columnsinfo )
            {
                DapperGenericsLib . DataGridLayout dglayout = new DapperGenericsLib . DataGridLayout ( );
                GenericClass gc = new GenericClass ( );
                gc = item;
                dglayout . Fieldname = item . field1 . Trim ( );
                dglayout . Fieldtype = item . field2 . Trim ( );
                if ( item . field3 != null )
                    dglayout . Fieldlength = Convert . ToInt32 ( item . field3 . Trim ( ) );
                if ( item . field4 != null )
                    dglayout . Fielddec = item . field4 != null ? Convert . ToInt32 ( item . field4 . Trim ( ) ) : 0;
                if ( item . field5 != null )
                    dglayout . Fieldpart = item . field5 != null ? Convert . ToInt32 ( item . field5 . Trim ( ) ) : 0;
                dglayoutlist . Add ( dglayout );
            }
            return dglayoutlist;
        }

        //**********************************//
        #region local control support
        //**********************************//
        private void SqlTables_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {
            //ProcessTableChanged ( e);

            string selection = "", prevselection = "";
            int itemscount = 0;
            if ( SqlTables . Items . Count == 0 )
                return;
            "" . Track ( );
            Mouse . OverrideCursor = Cursors . Wait;

            string previousSelection = LastActiveTable;
            if ( e == null )
            {
                selection = SqlTables . SelectedItem . ToString ( );
                itemscount = SqlTables . Items . Count;
                selection = $"{DBprefix}{SqlTables . SelectedItem . ToString ( )}";
            }
            else
            {
                itemscount = e . AddedItems . Count;
                selection = $"{e . AddedItems [ 0 ] . ToString ( )}";
            }
            if ( ListReloading == false && itemscount > 0 )
            {
                NewTableSelection = selection;
                if ( selection == LastActiveTable )
                {
                    "" . Track ( 1 );
                    e . Handled = true;
                    return;
                }
                LastActiveTable = CurrentTable;
                // This value is necessary for an SQL Output value to be accessed
                int colcount = 0;

                // Returns count of COLUMNS in table specified
                int existval = CheckTableExists ( selection , CurrentTableDomain );
                if ( existval > 0 )
                {
                    string error = "";
                    // returned count > 0 , so it DOES exist, so go get data from table
                    ObservableCollection<GenericClass> temp = new ObservableCollection<GenericClass> ( );

                    //returns the same record time over time 
                    $"Calling LoadGenericDb() {selection}" . Track ( );
                    string [ ] args = new string [ 1 ];
                    args [ 0 ] = selection;

                    //***********************************************************************//
                    // THIS IS THE CALL THAT LOADS THE INITIAL DATA FOR GENERICGRID
                    //***********************************************************************//
                    temp = dgControl . LoadGenericData ( $"spLoadTableAsGeneric" , args , true , MainWindow . SqlCurrentConstring );

                    // TEMP ONLY
                    Debug . WriteLine ( $"TEMP ONLY : {args [ 0 ]} loaded {temp . Count} records...." );
                    "Leaving LoadGenericDb()" . Track ( 1 );

                    if ( temp == null || temp . Count == 0 )
                    {
                        TableIsEmpty = true;
                        DataLoaded = true;
                        SqlTables . SelectedItem = LastActiveTable;
                        DataLoaded = false;
                        if ( LOCALFILTER == false )
                            SetStatusbarText ( $"Although the requested table [ {selection} ] is in the  current Database, it does NOT contain any records \n" +
                            $"and therefore it has NOT been loaded, and the previous table {previousSelection} is still displayed" , 1 );
                        for ( int x = 0 ; x < Splist . Items . Count ; x++ )
                        {
                            if ( Splist . Items [ x ] == previousSelection )
                            {
                                Splist . SelectedIndex = x;
                                break;
                            }
                        }
                        CurrentTable = LastActiveTable;
                        //Mouse . OverrideCursor = Cursors . Arrow;
                        if ( e != null )
                            e . Handled = true;
                        "" . Track ( 1 );
                        return;
                    }
                    else
                        GridData = temp;

                    if ( LOCALFILTER == true && selection . ToUpper ( ) != "LOCALFILTERTABLE" )
                    {
                        // remove temp entry for display only filter result
                        List<string> newlist = new List<string> ( );
                        foreach ( var item in SqlTables . Items )
                        {
                            if ( item . ToString ( ) . ToUpper ( ) != "LOCALFILTERTABLE" )
                            {
                                newlist . Add ( item . ToString ( ) );
                            }
                            else
                                Debug . WriteLine ( $"found and removed LOCALFILTERTABLE!" );
                        }
                        SqlTables . ItemsSource = null;
                        SqlTables . Items . Clear ( );
                        SqlTables . ItemsSource = newlist;
                        ListReloading = true;
                        SqlTables . SelectedItem = selection;
                        ListReloading = false;
                        // clear flag so we do not do this every time 
                        LOCALFILTER = false;
                    }
                    else
                        SqlTables . SelectedItem = LastActiveTable;

                    LastActiveTable = selection;
                    DataLoaded = true;
                    CurrentTable = selection;

                    //*****************************************************************************************************************************************//
                    // Processes a hard coded enquiry using table (selection in this case))and returns ObsColl into GridData
                    // and also returns  a (global var) of type [List<DataTableLayout>] containing the full table columns specification
                    // These data  items are specific  to currentTable, and can there be used anywhere else without needing to update them
                    //*****************************************************************************************************************************************//

                    if ( TableLayoutDict . Count == 0 || TableLayoutDict . TryGetValue ( CurrentTable , out dglayoutlist ) == false )
                    {
                        // save structure for general access when this table is still the active table
                        // & Create new entry in control dictionary 
                        DGLAYOUTLIST = dglayoutlist;
                        int columncount = 0;
                        dglayoutlist = GetNewColumnsLayout ( CurrentTable , GridData , out columncount );
                        DGLAYOUTLIST = dglayoutlist;
                        TableLayoutDict . Clear ( );
                        TableLayoutDict . Add ( CurrentTable , DGLAYOUTLIST );
                    }

                    if ( dglayoutlist . Count > 20 )
                    {
                        StdError ( );
                        TableIsEmpty = true;
                        DataLoaded = true;
                        DataLoaded = false;
                        SetStatusbarText ( $"The requested table [ {selection} ] structure has {dglayoutlist . Count} columns, which exceeds the Total Colums supported \nf 20 columns,and therefore cannot be loaded, so the original table [ {previousSelection . ToUpper ( )}]is still displayed" , 1 );
                        for ( int x = 0 ; x < SqlTables . Items . Count ; x++ )
                        {
                            string upperstring = SqlTables . Items [ x ] . ToString ( ) . ToUpper ( );
                            if ( upperstring == previousSelection . ToUpper ( ) )
                            {
                                ListReloading = true;
                                SqlTables . SelectedIndex = x;
                                ListReloading = false;
                                break;
                            }
                        }
                        CurrentTable = previousSelection;
                        //Mouse . OverrideCursor = Cursors . Arrow;
                        e . Handled = true;
                        "" . Track ( 1 );
                        return;
                    }
                    // TODO URGENT These are  SLOW SLOW SLOW

                    colcount = DatagridControl . GetColumnsCount ( GridData );
                    DatagridControl . LoadActiveRowsOnlyInGrid ( dgControl . datagridControl , GridData , colcount );
                    DatagridControl . ReplaceDataGridFldNames ( GridData , ref dgControl . datagridControl , ref dglayoutlist , colcount );
                    Reccount . Text = GridData . Count . ToString ( );
                    if ( GridData . Count > 0 )
                    {
                        SetStatusbarText ( $"The data for {selection . ToUpper ( )} was loaded successfully and is shown in the viewer above..." );
                        GenGridCtrl . AlternateBackground = FindResource ( "Cyan0" ) as SolidColorBrush;
                        GenGridCtrl . Refresh ( );
                    }
                    LastActiveTable = selection;
                    CurrentTable = selection;
                    this . UpdateLayout ( );
                }
                else if ( existval == 0 )
                {
                    StdError ( );
                    TableIsEmpty = true;
                    DataLoaded = true;
                    SqlTables . SelectedItem = LastActiveTable;
                    DataLoaded = false;
                    CurrentTable = LastActiveTable;
                    SetStatusbarText ( $"A check made for the requested table [ {selection} ]showed that although it does exist, \nit does NOT contain any records and therefore it has NOT been loaded" , 1 );
                }
                else if ( existval == -1 )
                {
                    StdError ( );
                    TableIsEmpty = true;
                    DataLoaded = true;
                    SqlTables . SelectedItem = LastActiveTable;
                    DataLoaded = false;
                    CurrentTable = LastActiveTable;
                    SetStatusbarText ( $"FATAL ERROR : The requested table [ {selection} ] does NOT exist" , 1 );
                }
            }
            Mouse . OverrideCursor = Cursors . Arrow;
            if ( e != null ) e . Handled = true;
            "" . Track ( 1 );
            this . Refresh ( );
        }

        public int CheckTableExists ( string name , string domain = "IAN1" )
        {
            bool exist = false;
            int result = -1;
            IEnumerable<dynamic> reslt = null;

            string Con = NewWpfDev . Utils . GetCheckCurrentConnectionString ( CurrentTableDomain );
            using ( IDbConnection db = new SqlConnection ( Con ) )
            {
                try
                {
                    db . Open ( );
                    DynamicParameters parameters = new DynamicParameters ( );
                    // cannot add domain prefix due to potential functionality in SP
                    parameters . Add ( "Arg1" , name , dbType: DbType . String , direction: ParameterDirection . Input );
                    // get count of records in specified table
                    //*****************************************************************************************************************************//
                    GetValidDomain ( );
                    //$"{DBprefix}spCheckTableExists2" . DapperTrace ( );
                    reslt = db . Query ( $"{DBprefix}spCheckTableExists2" , parameters , commandType: CommandType . StoredProcedure );
                    //*****************************************************************************************************************************//
                    result = reslt . Count ( );
                    $"{DBprefix}spCheckTableExists2 returned {result}" . DapperTrace ( );
                }
                catch ( Exception ex )
                {
                    Debug . WriteLine ( $"CheckTableExists failed, error was {ex . Message}" );
                    db . Close ( );
                }
            }
            return result;
        }

        public dynamic LoadFullSqlTable ( string SqlCommand , string [ ] args , out string err , string domain = "IAN1" , int method = 0 )
        {
            // USAGE :
            //if(method == 0 ) Sqlcommand=TEXT, returns an int
            //if(method == 1 ) Sqlcommand=S.P name, returns a text string
            //if(method == 2 ) Sqlcommand=S.P name, returns a List
            // We use dynamic  types for return value in here
            IEnumerable<dynamic> reslt = null;
            IEnumerable<dynamic> str = null;
            string countstr = "";

            ObservableCollection<GenericClass> tmp = new ObservableCollection<GenericClass> ( );
            err = "";
            collection = new ObservableCollection<GenericClass> ( );
            string Con = NewWpfDev . Utils . GetCheckCurrentConnectionString ( CurrentTableDomain );
            try
            {
                using ( IDbConnection db = new SqlConnection ( Con ) )
                {
                    $"{SqlCommand}" . DapperTrace ( );
                    bool hasoutput = false;
                    bool hasretval = false;
                    DynamicParameters parameters = new DynamicParameters ( );
                    parameters = SProcsSupport . ParseSqlArguments ( parameters , args , ref hasoutput , ref hasretval );
                    Dictionary<string , List<dynamic>> dict = new Dictionary<string , List<dynamic>> ( );

                    if ( method == 0 )
                        reslt = db . Query ( SqlCommand , CommandType . Text );
                    else if ( method == 1 )
                    {
                        int colcount = 0;
                        Dictionary<string , object> dict2 = new Dictionary<string , object> ( );

                        SqlCommand = "Drop table if exists retvalues";
                        var spr2 = db . Query<dynamic> ( SqlCommand , parameters , commandType: CommandType . Text );
                        SqlCommand = "select ROUTINE_DEFINITION into retvalues from SpFullSchema where UPPER(ROUTINE_NAME)=UPPER(@Arg1) order by ROUTINE_NAME";
                        var spr = db . Query<dynamic> ( SqlCommand , parameters , commandType: CommandType . Text );
                        SqlCommand = "select * from retvalues";
                        var spr3 = db . Query<dynamic> ( SqlCommand , parameters , commandType: CommandType . Text );

                        string newresults = GetStringFromDynamic ( spr3 );
                        return newresults;
                    }
                    else if ( method == 2 )
                    {
                        str = db . Query<dynamic> ( SqlCommand , param: parameters , commandType: CommandType . StoredProcedure );
                        dynamic newcount = null;
                        foreach ( var item in str )
                        {
                            newcount = item . count;

                            // set (outint count) parameter
                            countstr = newcount . ToString ( );
                        }
                        // return it as a string
                        return newcount;
                    }
                    else
                    { }
                }
                if ( reslt == null )
                    return null;
                else
                {
                    string errormsg = "";
                    int dictcount = 0;
                    int fldcount = 0;
                    int colcount = 0;
                    GenericClass gc = new GenericClass ( );
                    // int  cnt = Convert.ToInt32(reslt);
                    List<int> VarcharList = new List<int> ( );
                    Dictionary<string , string> outdict = new Dictionary<string , string> ( );
                    Dictionary<string , object> dict = new Dictionary<string , object> ( );
                    try
                    {
                        foreach ( var item in reslt )
                        {
                            DapperGenericsLib . DataGridLayout dglayout = new DapperGenericsLib . DataGridLayout ( );
                            try
                            {
                                // we need to create a dictionary for each row of data then add it to a GenericClass row then add row to Generics Db
                                string buffer = "";
                                // WORKS OK
                                DatagridControl . ParseDapperRow ( item , dict , out colcount );
                                gc = new GenericClass ( );
                                dictcount = 1;
                                fldcount = dict . Count;
                                string tmp3 = "";

                                int index = 1;
                                // Parse reslt.item into  single dglayout Dictionary record
                                foreach ( var pair in dict )
                                {
                                    Dictionary<string , string> Columntypes = new Dictionary<string , string> ( );
                                    try
                                    {
                                        if ( pair . Key != null )   //l && pair.Value != null)
                                        {
                                            if ( pair . Value != null )
                                            {
                                                DatagridControl . AddDictPairToGeneric ( gc , pair , dictcount++ );
                                                tmp3 = $"field{index++} = {pair . Value . ToString ( )}";
                                                outdict . Add ( pair . Key , pair . Value . ToString ( ) );
                                                buffer += tmp + ",";
                                            }
                                            //List<int>
                                            if ( pair . Key == "character_maximum_length" )
                                                dglayout . Fieldlength = item . character_maximum_length == null ? 0 : item . character_maximum_length;
                                            if ( pair . Key == "data_type" )
                                                dglayout . Fieldtype = item . data_type == null ? 0 : item . data_type;
                                            if ( pair . Key == "column_name" )
                                            {
                                                string temp = item . column_name . ToString ( );
                                                if ( DatagridControl . IsDuplicatecolumnname ( temp , Columntypes ) == false )
                                                    Columntypes . Add ( temp , item . data_type . ToString ( ) );
                                                dglayout . Fieldname = item . column_name == null ? "UNSPECIFIED" : item . column_name;

                                                //dglayout . Colindex = index - 1;

                                                // Add Dictionary <string,string> to List<Dictionary<string,string>
                                                ColumntypesList . Add ( Columntypes );
                                            }
                                        }
                                    }
                                    catch ( Exception ex )
                                    {
                                        Debug . WriteLine ( $"Dictionary ERROR : {ex . Message}" );
                                        err = ex . Message;
                                    }
                                }
                                if ( dict . Count == 0 )
                                    errormsg = $"No records were retuned for {SqlCommand}";
                                if ( DatagridControl . IsDuplicateFieldname ( dglayout , dglayoutlist ) == false )
                                {
                                    //dglayout += index . ToString ( );
                                    dglayoutlist . Add ( dglayout );
                                }//remove trailing comma
                                string s = buffer?.Substring ( 0 , buffer . Length - 1 );
                                buffer = s;
                                // We now  have ONE sinlge record, but need to add this  to a GenericClass structure 
                                int reccount = 1;
                                foreach ( KeyValuePair<string , string> val in outdict )
                                {  //
                                    switch ( reccount )
                                    {
                                        case 1:
                                            gc . field1 = val . Value . ToString ( );
                                            break;
                                        case 2:
                                            gc . field2 = val . Value . ToString ( );
                                            break;
                                        case 3:
                                            gc . field3 = val . Value . ToString ( );
                                            break;
                                        case 4:
                                            gc . field4 = val . Value . ToString ( );
                                            break;
                                        case 5:
                                            gc . field5 = val . Value . ToString ( );
                                            break;
                                        case 6:
                                            gc . field6 = val . Value . ToString ( );
                                            break;
                                        case 7:
                                            gc . field7 = val . Value . ToString ( );
                                            break;
                                        case 8:
                                            gc . field8 = val . Value . ToString ( );
                                            break;
                                        case 9:
                                            gc . field9 = val . Value . ToString ( );
                                            break;
                                        case 10:
                                            gc . field10 = val . Value . ToString ( );
                                            break;
                                        case 11:
                                            gc . field11 = val . Value . ToString ( );
                                            break;
                                        case 12:
                                            gc . field12 = val . Value . ToString ( );
                                            break;
                                        case 13:
                                            gc . field13 = val . Value . ToString ( );
                                            break;
                                        case 14:
                                            gc . field14 = val . Value . ToString ( );
                                            break;
                                        case 15:
                                            gc . field15 = val . Value . ToString ( );
                                            break;
                                        case 16:
                                            gc . field16 = val . Value . ToString ( );
                                            break;
                                        case 17:
                                            gc . field17 = val . Value . ToString ( );
                                            break;
                                        case 18:
                                            gc . field18 = val . Value . ToString ( );
                                            break;
                                        case 19:
                                            gc . field19 = val . Value . ToString ( );
                                            break;
                                        case 20:
                                            gc . field20 = val . Value . ToString ( );
                                            break;
                                    }
                                    reccount += 1;
                                }
                                //genericlist.Add(buffer);
                                collection . Add ( gc );
                            }
                            catch ( Exception ex )
                            {
                                err = $"SQLERROR : {ex . Message}";
                                errormsg = err;
                                Debug . WriteLine ( err );
                            }
                            //collection.Add(gc);
                            dict . Clear ( );
                            outdict . Clear ( );
                            dictcount = 1;
                        }
                    }
                    catch ( Exception ex )
                    {
                        Debug . WriteLine ( $"OUTER DICT/PROCEDURE ERROR : {ex . Message}" );
                        err = ex . Message;
                        errormsg = err;
                    }
                    if ( errormsg == "" )
                        errormsg = $"DYNAMIC:{fldcount}";
                    return collection; //collection.Count;
                }
            }
            catch ( Exception ex )
            {
                Debug . WriteLine ( $"Error : {ex . Message}" );
            }
            return tmp;
        }

        public void ResetColumnHeaderToTrueNames ( ObservableCollection<GenericClass> collection , string CurrentTable , DataGrid Grid )
        {
            //Update Column headers to original column names, so we need to create dummy list just to call Replace headers method
            int colcount = dgControl . datagridControl . Columns . Count;
            List<DapperGenericsLib . DataGridLayout> dglayoutlist = new List<DapperGenericsLib . DataGridLayout> ( );
            ReplaceDataGridFldNames ( collection , ref Grid , ref dglayoutlist , colcount );
        }

        public bool LoadDbTables ( string currentTable )
        {   //task to load list of Db Tables
            CurrentTable = currentTable;
            // Add domain to command
  
            List<string> TablesList = GetDbTablesList ( CurrentTableDomain );
              SqlTables . ItemsSource = null;
            SqlTables . Items . Clear ( );
            SqlTables . ItemsSource = TablesList;
            int index = 0;
            if ( currentTable != "" )
            {
                currentTable = currentTable . ToUpper ( );
                foreach ( string item in SqlTables . Items )
                {
                    if ( currentTable == item . ToUpper ( ) )
                    {
                        DataLoaded = true;
                        if ( SqlTables . SelectedIndex != -1 )
                            SqlTables . SelectedItem = SqlTables . Items [ SqlTables . SelectedIndex ];
                        else
                            SqlTables . SelectedIndex = index;  // Triggers  a Database read
                        DataLoaded = false;
                        break;
                    }
                    index++;
                }
            }
            //} );
            return true;
        }
        public List<string> GetDbTablesList ( string DbName )
        {
            List<string> TablesList = new List<string> ( );
            string SqlCommand = "";
            List<string> list = new List<string> ( );
            DbName = DbName . ToUpper ( );

            string Con = NewWpfDev . Utils . GetCheckCurrentConnectionString ( CurrentTableDomain );

            if ( Con == "" )
            {
                Debug . WriteLine ( $"Failed to set connection string for {DbName} Db" );
                return TablesList;
            }
            //// All Db's have their own version of this SP.....
            GetValidDomain ( );
            SqlCommand = $"{DBprefix}spGetTablesList";

            TablesList = SProcsDataHandling . CallStoredProcedure ( list , SqlCommand );
            if ( CurrentTableDomain . ToUpper ( ) == "ADVENTUREWORKS2019" )
                TablesList = ConvertTableNames ( TablesList );
            //// return list of all current SQL tables in current Db
            return TablesList;
        }
        //public List<string> CallStoredProcedure ( List<string> list , string sqlcommand , string [ ] args = null )
        //{
        //    List<string> splist = new List<string> ( );
        //    splist = GenDapperQueries . ProcessUniversalQueryStoredProcedure ( "spGetStoredProcs" , args , CurrentTableDomain , out string err );
        //    //This call returns us a List<string>
        //    // This method is NOT a dynamic method
        //    return splist;
        //}
        public static List<string> GetDataDridRowsAsListOfStrings ( DataTable dt )
        {
            List<string> list = new List<string> ( );
            foreach ( DataRow row in dt . Rows )
            {
                var txt = row . Field<string> ( 0 );
                list . Add ( txt );
            }
            return list;
        }

        private void ToggleColumnHeaders_Checked ( object sender , RoutedEventArgs e )
        {
            CheckBox cb = sender as CheckBox;
            if ( bStartup == false && cb . IsChecked == true )
                ShowColumnHeaders = true;
            else
                ShowColumnHeaders = false;

            int colcount = dgControl . datagridControl . Columns . Count;
            if ( colcount > 0 )
            {
                dgControl . ShowTrueColumns ( dgControl . datagridControl , CurrentTable , colcount , ShowColumnHeaders , IsLoading );
                ShowColumnHeaders = false;
            }
        }
        private void ToggleColumnHeaders_Click ( object sender , RoutedEventArgs e )
        {
            CheckBox cb = sender as CheckBox;
            if ( cb . IsChecked == true )
            {
                ShowColumnHeaders = true;
                int colcount = dgControl . datagridControl . Columns . Count;
                dgControl . ShowTrueColumns ( dgControl . datagridControl , CurrentTable , colcount , ShowColumnHeaders , IsLoading );
            }
            else
            {
                ShowColumnHeaders = false;
                dgControl . SetDefColumnHeaderText ( dgControl . datagridControl , false );
            }
        }
        private void ShowColumnInfo_Click ( object sender , RoutedEventArgs e )
        {
            //popup the column info in a FlowDoc viewer
            int columncount = 0;
            if ( TableLayoutDict . ContainsKey ( CurrentTable ) == false )
            {
                dglayoutlist = GetNewColumnsLayout ( CurrentTable , GridData , out columncount );
            }
            else
            {
                if ( TableLayoutDict . TryGetValue ( CurrentTable , out dglayoutlist ) == false )
                {
                    // nope, so load it and save it to  control dict
                    dglayoutlist = GetNewColumnsLayout ( CurrentTable , GridData , out columncount );
                    DGLAYOUTLIST = dglayoutlist;
                    TableLayoutDict . Clear ( );
                    TableLayoutDict . Add ( CurrentTable , DGLAYOUTLIST );
                }
            }

            string ConString = GenericDbUtilities . CheckSetSqlDomain ( "" );
            if ( ConString == "" )
            {
                GenericDbUtilities . CheckDbDomain ( "" );
                ConString = MainWindow . CurrentSqlTableDomain;
            }
            GridData = DapperSupport . CreateFullColumnInfo ( CurrentTable , ConString , true );
            string output = GenerateTableStructuretext ( GridData );
            Mouse . OverrideCursor = Cursors . Arrow;
            fdl . ShowInfo ( Flowdoc , Filtercanvas , line1: output , clr1: "Black0" , line2: "" , clr2: "Black0" , line3: "" , clr3: "Black0" , header: "" , clr4: "Black0" );

            return;
        }
        public static string GenerateTableStructuretext ( ObservableCollection<GenericClass> GridData )
        {
            string output = "";
            foreach ( var item in GridData )
            {
                output += $"{item . field1.ToString().PadRight(25)}{item.field2}\n";
            }
            return output;
        }
        private void Close_Click ( object sender , RoutedEventArgs e )
        {
            Close ( );
        }
        private void NewTableName_GotKeyboardFocus ( object sender , KeyboardFocusChangedEventArgs e )
        {
            if ( NewTableName . Text . Contains ( "Enter New Table Name " ) )
                NewTableName . Text = "";
        }

        #endregion local control support

        //-----------------------------------------------------------//flags .conn
        public void UpdateSqlTableList ( string seltext )
        {
            "" . Track ( );
            int index = 0;
            string srchname = seltext . ToUpper ( );
            if ( SqlTables . Items . Count == 0 )
            {
                "" . Track ( 1 );
                return;
            }
            foreach ( string item in SqlTables . Items )
            {
                if ( srchname == item . ToUpper ( ) )
                {
                    SqlTables . SelectedItem = seltext;
                    break;
                }
                index++;
            }
            "" . Track ( 1 );
        }


        //**********************************************//
        #region Select columns for new table support
        //**********************************************//
        public int CreateNewTableAsync ( object sender , RoutedEventArgs e )
        {

            SaveAsNewTable ( sender , e );
            return 1;
            bool UseSelectedColumns = false;
            List<string> FldNames = new List<string> ( );
            ObservableCollection<GenericClass> collection = new ObservableCollection<GenericClass> ( );
            string NewDbName = NewTableName . Text . Trim ( );
            if ( NewDbName == "Enter New Table Name ...." )
            {
                MessageBox . Show ( "Please provide a name for the new table in the field provided..." , "New Table name required" );
                NewTableName . Focus ( );
                return -1;
            }
            if ( NewDbName == "" )
            {
                MessageBox . Show ( "Please enter a suitable name for the table you want to create !" , "Naming Error" );
                return -1;
            }
            CurrentTable = NewDbName;
            MessageBoxResult mbresult = MessageBox . Show ( "If you want to select only certain columns from the current table to be saved, Click YES, else click No" , "Data Formatting ?" ,
                MessageBoxButton . YesNoCancel ,
                MessageBoxImage . Question ,
                MessageBoxResult . No );

            if ( mbresult == MessageBoxResult . Cancel )
                return -1;
            if ( mbresult == MessageBoxResult . Yes )
            {
                // Save a set with only user selected columns
                string [ ] args = new string [ 20 ];
                UseSelectedColumns = true;
                string ConString = GenericDbUtilities . CheckSetSqlDomain ( "" );
                if ( ConString == "" )
                {
                    GenericDbUtilities . CheckDbDomain ( "" );
                    ConString = MainWindow . CurrentSqlTableDomain;
                }
                string Output = dgControl . GetFullColumnInfo ( CurrentTable , CurrentTable , ConString , false );

                string buffer = "";
                int index = 0;
                args = Output . Split ( '\n' );
                foreach ( var item in args )
                {
                    if ( item != null && item . Trim ( ) != "" )
                    {
                        string [ ] RawFldNames = item . Split ( ' ' );
                        string [ ] flds = { "" , "" , "" , "" };
                        int y = 0;
                        for ( int x = 0 ; x < RawFldNames . Length ; x++ )
                        {
                            if ( RawFldNames [ x ] . Length > 0 )
                                flds [ y++ ] = ( RawFldNames [ x ] );
                        }
                        buffer = flds [ 0 ];
                        if ( buffer != null && buffer . Trim ( ) != "" )
                        {
                            FldNames . Add ( buffer . ToUpper ( ) );
                            GenericClass tem = new GenericClass ( );
                            tem . field1 = buffer . ToUpper ( );    // fname
                            tem . field2 = flds [ 1 ];   //ftype
                            tem . field4 = flds [ 3 ];   // decroot
                            tem . field3 = flds [ 2 ];   // decpart
                            collection . Add ( tem );
                        }
                    }
                }
                //ALL WORKING  20/9/2022 - We now have a list of all Column names with
                //column type & size data, so let user choose what to save to a new table!
                SelectedRows . Clear ( );
                // load selection dialog with available coumns
                ColNames . ItemsSource = collection;
                // clear our reference structure
                SelectedRows . Clear ( );
                // Show dialog
                FieldSelectionGrid . Visibility = Visibility . Visible;
            }
            else
            {
                // just  do a direct copy
                string [ ] args = { $"{SqlTables . SelectedItem . ToString ( )}" , $"{NewDbName}" , "" , "" };
                int recordcount = 0;
                string Tablename = "";
                string err;
                string [ ] outputs;
                recordcount = dgControl . ExecuteStoredProcedure ( "spCopyDb" , args , out err );
                // make deep copy of table else it gets cleared elsewhere
                // Create a completely new instance via seriazable Clone method stored in NewWpfDev.Utils (in ObjectCopier class file)
                ObservableCollection<GenericClass> deepcopy = new ObservableCollection<GenericClass> ( );
                string originalname = $"{SqlTables . SelectedItem . ToString ( )}";
                deepcopy = NewWpfDev . Utils . CopyCollection ( GridData , deepcopy );
                GridData = deepcopy;
                string [ ] args1 = { $"{NewDbName}" };
                int colcount = dgControl . datagridControl . Columns . Count;
                DatagridControl . LoadActiveRowsOnlyInGrid ( dgControl . datagridControl , GridData , colcount );

                ReplaceDataGridFldNames ( GridData , ref dgControl . datagridControl , ref dglayoutlist , colcount );

                //DapperLibSupport . ReplaceDataGridFldNames ( NewDbName , ref dgControl . datagridControl , ref dglayoutlist , colcount );
                LoadDbTables ( NewDbName );
                SqlTables . SelectedItem = CurrentTable;

                //GenericGridSupport . SelectCurrentTable ( NewDbName );

                if ( dgControl . datagridControl . Items . Count > 0 )
                {
                    SetStatusbarText ( $"New Table [{NewDbName}] Created successfully, {dgControl . datagridControl . Items . Count} records inserted & Table is now shown in datagrid above...." , 1 );
                    StdError ( );
                }
                else
                {
                    SetStatusbarText ( $"New Table [{NewDbName}] could NOT be Created. Error was [{err}] " , 1 );
                    StdError ( );
                }
                NewTableName . Text = NewDbName;

            }
            //Mouse . OverrideCursor = Cursors . Arrow;
            return 1;
        }
        private void ColNames_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {
            // Working  successfully 19/9/22
            DataGrid grid = sender as DataGrid;
            int index = -1;
            bool ismatched = false;
            var newitem = e . AddedItems;// as GenericClass;
                                         // Get Index of selected row in datagrid
                                         //           if ( grid . CurrentItem != null )
            if ( newitem . Count == 0 )
                return;
            var newitem2 = newitem [ 0 ];
            index = grid . Items . IndexOf ( newitem2 );
            // Add zero based offset to selected columns
            SelectedRows . Add ( index );
            return;
            // Save selected row to list
            //foreach ( var item in SelectedRows )
            //{
            //    if ( item == index )
            //    {
            //        ismatched = true;
            //        break;
            //    }
            //}
            //Check and remove any possible duplicate index
            if ( ismatched )
            {
                List<int> temp = new List<int> ( );
                foreach ( var item in SelectedRows )
                {
                    if ( item != index )
                        temp . Add ( item );
                }
                // copy list back to original
                SelectedRows = temp;
            }
            if ( ismatched != false )
                SelectedRows . Add ( index );
        }
        private void GoBtn_Click ( object sender , RoutedEventArgs e )
        {
            "" . Track ( );
            // Creating new table based on selected columns
            DapperGenericsLib . GenericClass selColumns = new DapperGenericsLib . GenericClass ( );
            ObservableCollection<GenericClass> collection = new ObservableCollection<GenericClass> ( );
            List<GenericToRealStructure> grsList = new List<GenericToRealStructure> ( );
            string [ ] cols = new string [ ColNames . SelectedItems . Count ];

            string ConString = GenericDbUtilities . CheckSetSqlDomain ( "" );
            if ( ConString == "" )
            {
                // set to our local definition
                ConString = MainWindow . SqlCurrentConstring;
            }

            // get a string containing  of ALL of  the SELECTED columns in current table
            string Columnsinfo = dgControl . GetFullColumnInfo ( NewTableName . Text , CurrentTable , ConString , false , false );
            string [ ] colnames = Columnsinfo . Split ( "\n" );
            int selindex = 0;
            // Create list of selected items only
            List<string> flddescription = new List<string> ( );
            int coloffset = 0;
            int offset = 0;
            int selrowscount = 0;


            // We now have all selected columns in grslist List, so hide selection dialog again
            FieldSelectionGrid . Visibility = Visibility . Collapsed;
            FieldSelectionGrid . UpdateLayout ( );
            foreach ( DapperGenericsLib . GenericClass item in ColNames . SelectedItems )
            {
                GenericToRealStructure grs = new GenericToRealStructure ( );
                // Get column name from full set of columns
                string tmp = item . field1;
                string [ ] tmparray = colnames [ SelectedRows [ selrowscount++ ] ] . Split ( ":" );
                string setfield = tmparray [ 0 ] . ToUpper ( );
                for ( int p = 0 ; p < tmparray . Length - 1 ; p++ )
                {
                    if ( tmparray [ p ] . ToUpper ( ) == setfield )
                    {
                        grs . fname = item . field1;
                        grs . ftype = item . field2;
                        if ( item . field3 != "" )
                            grs . decroot = Convert . ToInt32 ( item . field3 );
                        if ( item . field4 != null && item . field4 != "" )
                            grs . decpart = Convert . ToInt32 ( item . field4 );
                        grs . colindex = SelectedRows [ offset++ ];
                        grsList . Add ( grs );
                        break;
                    }
                }
            }
            string err = "";
            string [ ] SqlArgs = new string [ 20 ];
            //var v = SelectedRows;
            CreateAsyncTable ( NewTableName . Text , grsList , out SqlArgs , out err );

            // finally load and show in datagrid

            int colcount = DatagridControl . GetColumnsCount ( ColumnsData );
            DatagridControl . LoadActiveRowsOnlyInGrid ( dgControl . datagridControl , ColumnsData , colcount );
            if ( ShowColumnHeaders == true )
                ResetColumnHeaderToTrueNames ( collection , NewTableName . Text , dgControl . datagridControl );
            else
            {
                //List<DataTableLayout> dapperdglayoutlist = new List<DataTableLayout> ( );
                DatagridControl . ReplaceDataGridFldNames ( GridData , ref dgControl . datagridControl , ref dglayoutlist , colcount );
            }
            Reccount . Text = GridData . Count . ToString ( );
            //ReLoad tables list to include our new temporary table, and select it
            LoadDbTables ( NewTableName . Text );
            SqlTables . SelectedItem = NewTableName . Text;
            if ( ColumnsData . Count > 0 )
                SetStatusbarText ( $"The table [{CurrentTable}] has been copied successfully, returning a total of {ColumnsData . Count ( )} records \nin new Table {NewTableName . Text} with {colcount} columns selected, and these are shown in the viewer above..." , 2 );
            StdSuccess ( );
            CurrentTable = NewTableName . Text;
            Mouse . OverrideCursor = Cursors . Arrow;
            SelectedRows . Clear ( );
            e . Handled = true;
            "" . Track ( 1 );
        }
        public int CreateAsyncTable ( string NewDbName , List<GenericToRealStructure> TableStruct , out string [ ] SqlArgs , out string err )
        {
            int x = 0;
            dgControl = GenGridCtrl;
            string error = "";
            SqlArgs = new string [ 20 ];
            err = "";
            "" . Track ( );

            // hide selection dialog
            FieldSelectionGrid . Visibility = Visibility . Collapsed;

            // Assign current data collection to a new special collection for selected columns only for now
            int [ ] columnoffsets = new int [ TableStruct . Count ];
            string SqlCommandstring = "";
            ColumnsData = dgControl . CreateLimitedTableAsync ( NewDbName , TableStruct , out SqlCommandstring , out SqlArgs , out columnoffsets , out error );
            if ( ColumnsData == null || ColumnsData . Count == 0 )
            {
                StdError ( );
                Debug . WriteLine ( $"ERROR Table creation failed" );
                return -1;
            }
            if ( error != "" ) err = error;
            // We should now have all the data in our new columns only table
            if ( GridData == null )
                return -1;
            if ( x == -2 )
                NewTableName . Focus ( );
            if ( x == -3 )
                SetStatusbarText ( $"New Table creation failed, see error log file for more infomation...." , 1 );
            // clear temporay grid data
            //ColNames . ItemsSource = null;
            // Save the new data t the sql table itself
            SaveDataToNewTable ( GridData , NewDbName , SqlArgs , SqlCommandstring , columnoffsets , out err );
            //            SaveDataToNewTable ( GridData , NewDbName , ColumnsData , SqlArgs , SqlCommandstring , columnoffsets , out err );
            Mouse . OverrideCursor = Cursors . Arrow;
            "" . Track ( 1 );
            return 1;
        }

        #endregion Select columns for new table support
        //-----------------------------------------------------------//

        private void stopBtn_Click ( object sender , RoutedEventArgs e )
        {
            // Aborting creation of new table based on selected columns
            FieldSelectionGrid . Visibility = Visibility . Collapsed;
        }
        private void ShowInfo ( object sender , RoutedEventArgs e )
        {
            // Load boilerplate text describing ths control
            infotext = File . ReadAllText ( @$"C:\users\ianch\Documents\GenericGridInfo.Txt" );
            if ( Splist . Items . Count > 0 )
                DisplayInformationViewer ( false ); // dont reload list of SPs
            else
                DisplayInformationViewer ( true );  // we DO need to reload list of SPs

            majorgrid . RowDefinitions [ 0 ] . Height = new GridLength ( 1 , GridUnitType . Pixel );
            majorgrid . RowDefinitions [ 2 ] . Height = new GridLength ( majorgrid . ActualHeight - 10 , GridUnitType . Star );
            RTBox . Refresh ( );
            Splist . Refresh ( );
            majorgrid . UpdateLayout ( );
            majorgrid . Refresh ( );
            Gengrid_SizeChanged ( null , null );
        }
        private void InfoGrid_KeyDown ( object sender , KeyEventArgs e )
        {
            if ( e . Key == Key . Escape )
            {
                // InfoGrid . Visibility = Visibility . Collapsed;
                if ( SpStringsSelector . Visibility == Visibility . Visible )
                    SpStringsSelector . Visibility = Visibility . Collapsed;
                // dgControl . datagridControl . Visibility = Visibility . Visible;
                InfoViewerShown = false;
                if ( SpStringsSelector . Visibility == Visibility . Visible )
                    ProcNames . Focus ( );
                // Splist . ItemsSource = null;
            }
        }
        private void statusbar_KeyDown ( object sender , KeyEventArgs e )
        {
            if ( e . Key == Key . Enter )
                CreateNewTableAsync ( sender , e );
        }
        private void SaveSelectedOnly ( object sender , RoutedEventArgs e )
        {
            int recsinserted = 0;
            NewSelectedTableName = NewTableName . Text;
            GenericGridSupport gensupt = new GenericGridSupport ( );
            int retval = gensupt . ProcessSelectedRows ( out recsinserted , this . CurrentTable , false );
            if ( retval == -9 )
            {
                // error - handle it here 
                MessageBoxResult res2 = MessageBox . Show ( $"It appears that the data saving process to {NewSelectedTableName . ToUpper ( )} FAILED \nso the current process has been cancelled !\n\nThe new Table still exists, do you want to delete it now" , "Fatal problem encountered" ,
                     MessageBoxButton . YesNo , MessageBoxImage . Question , MessageBoxResult . No );
                if ( res2 == MessageBoxResult . Yes )
                {
                    string err = "";
                    int retv = GenericGridSupport . DropTable ( NewSelectedTableName , CurrentTable , out err );
                    if ( err != "" )
                    {
                        Debug . WriteLine ( $"{err}" );
                    }
                    else
                        Debug . WriteLine ( $"{NewSelectedTableName} does not exist. so no action has occurred" );
                }
                SetStatusbarText ( $"It appears that the data save to a new table {NewSelectedTableName . ToUpper ( )} FAILED \nand therefore this process has been cancelled !" , 1 );
            }
            else if ( retval >= -2 )
                SetStatusbarText ( $"A total of {recsinserted} items have been used to create a new table {NewSelectedTableName . ToUpper ( )} in the \'.IAN1\' Database successfully" );
        }
        private void OptionsList_DropDownOpened ( object sender , EventArgs e )
        {
            // Lower Options combo(below) is being opened, so perform selective setup
            comboText1 . Text = "Close DropDown...";
        }
        private void OptionsList_DropDownClosed ( object sender , EventArgs e )
        {
            // Lower Options combo(below) is being closed, so perform selective setup
            comboText1 . Text = "Processing Options";
        }
        private void OptionsList_Selected ( object sender , SelectionChangedEventArgs e )
        {
            ComboBox cb = sender as ComboBox;
            string selection = e . OriginalSource . ToString ( );
            if ( cb . SelectedIndex == 1 )
                ShowColumnInfo_Click ( sender , e );    /// Show full schema
            else if ( cb . SelectedIndex == 2 )
                ReloadTables ( sender , e );                    // Reload list of Sql Tables
            else if ( cb . SelectedIndex == 3 )
                SaveAsNewTable ( sender , e );              // Save table as new table with selective columns
            else if ( cb . SelectedIndex == 4 )
                ShowFilter_Click ( sender , e );                //Filter current table contents
            else if ( cb . SelectedIndex == 5 )
                SaveSelectedOnly ( sender , e );            //Save only selected rows of table  to new table
            else if ( cb . SelectedIndex == 6 )
                SearchStoredProc ( sender , e );            // Open Search text Dialog
                                                            //Reset it to top (non active option so we can select any valid option next time
            cb . SelectedIndex = 0;
        }
        private void SaveAsNewTable ( object sender , RoutedEventArgs e )
        {
            string curtbl = CurrentTable;
            string err = "";
            string donorTable = SqlTables . SelectedItem . ToString ( );
            // Add DBprefix to command

            GetValidDomain ( );
            int result = GenericGridSupport . SaveAsNewTable ( donorTable , DBprefix + NewTableName . Text , out err );
            if ( result == -9 )
            {
                StdError ( );
                SqlTables . SelectedItem = LastActiveTable;
                SetStatusbarText ( $"The Copy of {SqlTables . SelectedItem . ToString ( )} to {curtbl} FAILED.  The reason was {err}" , 1 );
            }
        }
        public void RemoteReloadTables ( )
        {
            DatabaseChanged = true;
            ReloadTables ( null , null );
            Window handle = null;
            handle = Utils . FindWindowFromClass ( "Spresultsviewer" , ref handle );
            if ( handle != null )
            {
                MessageBox . Show ( "You already have the Results Viewer open, but it canot be updated on the fly to reflect this change to it's Data Source\n\nTherefore it will now be closed ..." , "Database being changed" );
                handle . Close ( );
            }
        }
        private void ReloadTables ( object sender , RoutedEventArgs e )
        {
            bool stillexists = false;
            if ( CurrentTable == "" )
                CurrentTable = SqlTables . SelectedItem . ToString ( );
            LoadDbTables ( CurrentTable );
            SetValue ( ListReloadingProperty , true );
            SqlTables . SelectedItem = CurrentTable;
            //GenericGridSupport . SelectCurrentTable ( CurrentTable );
            SetStatusbarText ( "List of Tables reloaded successfully..." );
            SetValue ( ListReloadingProperty , false );

            foreach ( string item in SqlTables . Items )
            {
                if ( item . ToUpper ( ) == CurrentTable )
                {
                    stillexists = true;
                    break;
                }
            }
            if ( stillexists == false )
            {
                dgControl . datagridControl . ItemsSource = null;
                dgControl . datagridControl . Items . Clear ( );
                dgControl . datagridControl . Refresh ( );
                //SqlTables . SelectedIndex = 0;
                //SqlTables . SelectedIndex = 0;
                //SqlTables . UpdateLayout ( );
                if ( DomainChanged )
                {
                    if ( SqlTables . SelectedItem != null )
                        SqlTables_SelectionChanged ( sender , null );
                    else
                    {
                        SqlTables . SelectedIndex = 0;
                        SqlTables . SelectedItem = 0;
                        SqlTables_SelectionChanged ( sender , null );
                    }
                }
                else
                    SetStatusbarText ( "List of Sql Tables has been refreshed successfully, but the previously displayed table no longer exists, \nso the 1st table in the new list of tables has been selected for you" , 1 );
                DomainChanged = false;
                dgControl . datagridControl . Refresh ( );
            }
            else
                MessageBox . Show ( "List of Sql Tables has been refreshed successfully" , "SQL Tables Utility" );

            if ( DisplayInformationViewer ( true , true ) == true )
            {
                this . Title = $"Generic SQL Tables - Active Database = {CurrentTableDomain . ToUpper ( )}";
                caption . Text = $"Current Table Status / Processing information : Current Database = {CurrentTableDomain . ToUpper ( )}";
            }
            else
                SetStatusbarText ( "An error occured when trying to reload data, so the Stored Procedures list may not  be populated ..." , 1 );
            Mouse . OverrideCursor = Cursors . Arrow;
        }
        private void Gengrid_MouseDoubleClick ( object sender , MouseButtonEventArgs e )
        {
            var obj = e . OriginalSource;
            //if ( InfoViewerShown == true && InfoGrid . Visibility == Visibility . Collapsed )
            //{
            //    RTBox . Visibility = Visibility . Collapsed;
            //    //InfoGrid . Visibility = Visibility . Collapsed;
            //    if ( SpStringsSelector . Visibility == Visibility . Visible )
            //        SpStringsSelector . Visibility = Visibility . Collapsed;
            //    dgControl . datagridControl . Visibility = Visibility . Visible;
            //    InfoViewerShown = false;
            //    Splist . ItemsSource = null;
            //    GenGridCtrl . Visibility = Visibility . Visible;
            //    //ListCounter . Text = $"Content has been Cleared ...";
            //    //PromptLine . Text = $"Listbox in left Column has been cleared of All Stored Procedures";
            //}
            //else if ( InfoViewerShown == true && InfoGrid . Visibility == Visibility . Visible )
            //{
            //    //RTBox . Visibility = Visibility . Collapsed; 
            //    InfoGrid . Visibility = Visibility . Collapsed;
            //    dgControl . datagridControl . Visibility = Visibility . Visible;
            //    InfoViewerShown = false;
            //    Splist . ItemsSource = null;
            //    if ( SpStringsSelector . Visibility == Visibility . Visible )
            //        SpStringsSelector . Visibility = Visibility . Collapsed;
            //}
            //else if ( InfoViewerShown == false && InfoGrid . Visibility == Visibility . Visible )
            //{
            //    InfoGrid . Visibility = Visibility . Visible;
            //    //dgControl . datagridControl . Visibility = Visibility . Collapsed;
            //    InfoViewerShown = true;
            //}
            //else if ( InfoViewerShown == false && InfoGrid . Visibility == Visibility . Collapsed )
            //{
            //    RTBox . Visibility = Visibility . Collapsed;
            //    //InfoGrid . Visibility = Visibility . Collapsed;
            //    dgControl . datagridControl . Visibility = Visibility . Visible;
            //    InfoViewerShown = true;
            //    if ( SpStringsSelector . Visibility == Visibility . Visible )
            //        SpStringsSelector . Visibility = Visibility . Collapsed;
            //}
        }

        //****************************************************//
        #region FILTERING SUPPORT incl moving it around
        //****************************************************//

        private void ShowFilter_Click ( object sender , RoutedEventArgs e )
        {
            // Working 3/10/22
            // Open the 'Floating' dialog
            Filtering . Visibility = Visibility . Visible;
            Filtering . UpdateLayout ( );
            filtertext . Text = "";
            filtertext . Focus ( );
            Type type = sender . GetType ( );
            Debug . WriteLine ( $"Show Type is {type}" );
            // Initialize our 'dialog' dragging system
            DragCtrl . InitializeMovement ( Filtering as FrameworkElement );
            if ( LastActiveFillter != "" )
                filtertext . Text = LastActiveFillter;
        }

        //**********************************//
        #region Control Drag/Movement  support        
        //**********************************//

        // Finds the parent immediately above the Canvas,
        // which is what we are dragging
        public object GetControlDragParent ( object sender , string target )
        {
            Grid grid = null;
            object parent = null;
            Type type = sender . GetType ( );
            string str = "";
            NewWpfDev . Utils . FindVisualParent ( sender as UIElement , out object [ ] array );
            for ( int x = 0 ; x < array . Length ; x++ )
            {
                if ( array [ x ] == null )
                    continue;
                Type type2 = array [ x ] . GetType ( );
                string str2 = type2 . ToString ( );
                if ( str2 . ToString ( ) . Contains ( "Border" ) )
                {
                    // make sure it is not US that is top in hierarchy
                    if ( x == 0 )
                    {
                        List<object> list = new List<object> ( );
                        parent = sender;    //Yes it is
                                            // Check to see if 1st child is a grid.  If so, we have a border AROUND the ctrl to 
                        list = NewWpfDev . Utils . GetChildControls ( parent as UIElement , "Grid" );
                        parent = list [ 0 ];
                    }
                    else
                        parent = array [ x - 1 ];
                    break;
                }
                if ( str2 . ToString ( ) . Contains ( "Canvas" ) )
                {
                    // make sure it is not US that is top in hierarchy
                    if ( x == 1 )
                        parent = sender;
                    else
                        parent = array [ x - 1 ];
                    break;
                }
            }
            return parent;
        }

        private void DragDialog_LButtonDn ( object sender , MouseButtonEventArgs e )
        {
            // Working 3/10/22
            Grid sendergrid = sender as Grid;
            string Sendername = sendergrid . Name;
            DoPanelDragInit ( sender , e , Sendername );

            Control activegrid = null;
            Grid filtergrid = new Grid ( );
            FrameworkElement parent = null;
            FrameworkElement fwelement = new FrameworkElement ( );
            Type type = sender . GetType ( );
            // Finds the parent immediately above the Canvas,
            // which is what we are dragging
            if ( type == typeof ( Button ) )
            {
                return;
            }
            if ( type != typeof ( Grid ) )
            {
                parent = ( Grid ) GetControlDragParent ( sender , "Execsp" );
                type = parent . GetType ( );
                if ( parent == null )
                    fwelement = ( FrameworkElement ) parent;
                else
                {
                    DragCtrl . InitializeMovement ( parent );
                    DragCtrl . MovementStart ( parent , e );
                    DragDialog_Moving ( parent , e );
                }
            }
            else
            {
                DragCtrl . InitializeMovement ( ( FrameworkElement ) sender );
                DragCtrl . MovementStart ( sender , e );
                DragDialog_Moving ( sender , e );
            }
        }

        private void DragDialog_Moving ( object sender , MouseEventArgs e )
        {
            // Working 3/10/22
            if ( MovingObject != null && e . LeftButton == MouseButtonState . Pressed )
            {
                Type type = sender . GetType ( );
                if ( type == typeof ( Button ) )
                    return;
                if ( type == typeof ( TextBox ) )
                    return;
                DragCtrl . CtrlMoving ( sender , e );
            }
        }
        private void DragDialog_Ending ( object sender , MouseButtonEventArgs e )
        {
            // Working 3/10/22
            DragCtrl . MovementEnd ( sender , e );
        }
        private void FieldSelectionGrid_LostFocus ( object sender , RoutedEventArgs e )
        {
        }

        #endregion Control movement  support

        #region Filtering

        private void Filtering_KeyDown ( object sender , KeyEventArgs e )
        {
            if ( e . Key == Key . Enter )
                Filter_Click ( sender , null );
            if ( e . Key == Key . Escape )
            {
                Filtering . Visibility = Visibility . Collapsed;
                DragCtrl . MovementEnd ( sender , e );
            }
        }
        private void closeFilter ( object sender , RoutedEventArgs e )
        {
            Filtering . Visibility = Visibility . Collapsed;
            DragCtrl . MovementEnd ( sender , e );
            GenGridCtrl . Visibility = Visibility . Visible;
            GenGridCtrl . Refresh ( );
        }
        //****************************************************//
        private void Filter_Click ( object sender , RoutedEventArgs e )
        {
            string temp = filtertext . Text . Trim ( );
            if ( temp . Contains ( "\"" ) )
            {
                int index = 0;
                string newstring = "";
                string [ ] bakup;
                //replace double quotes
                for ( int x = 0 ; x < temp . Length ; x++ )
                {
                    char ch = temp [ x ];
                    if ( ch != '\"' )
                        newstring += ch;
                    else
                        newstring += "'";
                }
                temp = newstring;
            }
            string filtercmd = $"{temp}";
            string err = "";
            int recordcount = 0;
            string Tablename = "";
            string originaltable = CurrentTable;
            LastActiveFillter = filtercmd;

            string [ ] filterargs = new string [ 3 ];
            filterargs [ 0 ] = $"{CurrentTable}";
            filterargs [ 1 ] = LastActiveFillter . ToUpper ( );

            string replacementtable = "";

            "" . Track ( );

            // GenericClass gc = new GenericClass ( );
            if ( LOCALFILTER == false )
            {
                if ( CurrentTable != "FILTEREDDATA" && CurrentTable != "FILTEREDDATA2" )
                {

                    int existval = CheckTableExists ( "'FILTEREDDATA2'" , CurrentTableDomain );
                    //string [ ] args2 = new string [ 1 ];
                    //args2 [ 0 ] = "'FILTEREDDATA2'";
                    //ObservableCollection<GenericClass> tempdb = new ObservableCollection<GenericClass> ( );
                    //recordcount = dgControl . GetCountFromStoredProc ( "spCheckTableExists" , args2 );
                    ////tempdb = dgControl . GetDataFromStoredProcedure ( "spCheckTableExists" , args2 , out err , out recordcount ,1);
                    //recordcount = tempdb . Count;
                    ////dgControl . ExecuteStoredProcedure ( "spCheckTableExists" , args2 , out err );
                    if ( existval != -1 )
                        replacementtable = "FILTEREDDATA";
                    else
                    {
                        MessageBoxResult result3 = MessageBox . Show ( "The normal Filterered results table 'FILTEREDATA' already exists !.\n\nSelect YES to overwrite it or NO to save the results to 'FILTEREDDATA2" ,
                            "Sql filtering Table Duplication" ,
                                 MessageBoxButton . YesNoCancel , MessageBoxImage . Question , MessageBoxResult . Yes );
                        if ( result3 == MessageBoxResult . No )
                            replacementtable = "FILTEREDDATA2";
                        else if ( result3 == MessageBoxResult . Cancel )
                        {
                            Filtering . Visibility = Visibility . Collapsed;
                            "" . Track ( 1 );
                            return;
                        }
                        else
                            replacementtable = "FILTEREDDATA";
                    }
                }
                else if ( CurrentTable == "FILTEREDDATA" )
                    replacementtable = "FILTEREDDATA2";
                else if ( CurrentTable == "FILTEREDDATA2" )
                    replacementtable = "FILTEREDDATA";
                filterargs [ 2 ] = $"{replacementtable}";

                int exists = CheckTableExists ( replacementtable , CurrentTableDomain );
                if ( exists > 0 )
                {
                    MessageBoxResult result3 = MessageBox . Show ( "The normal table for Filterered results of  'FILTEREDATA' already exists !.\n\nSelect YES to overwrite it or NO to save the results to 'FILTEREDDATA2" ,
                          "Sql filtering Table Duplication" ,
                               MessageBoxButton . YesNoCancel , MessageBoxImage . Question , MessageBoxResult . Yes );
                    if ( result3 == MessageBoxResult . No )
                        replacementtable = "FILTEREDDATA2";
                    else if ( result3 == MessageBoxResult . Cancel )
                    {
                        Filtering . Visibility = Visibility . Collapsed;
                        "" . Track ( 1 );
                        return;
                    }
                    else
                        replacementtable = "FILTEREDDATA";
                }
            }
            else
            {
                filterargs [ 2 ] = $"LocalFilterTable";
            }
            string spCommand = $"drop table if exists {filterargs [ 2 ]}";
            dgControl . ExecuteDapperTextCommand ( spCommand , null , out err );
            spCommand = $"Select * into {filterargs [ 2 ]} from {filterargs [ 0 ]} where {filterargs [ 1 ]}";
            var result = dgControl . ExecuteDapperTextCommand ( spCommand , null , out err );
            if ( err != "" )
            {
                spCommand = $"drop table if exists Temp";
                dgControl . ExecuteDapperTextCommand ( spCommand , null , out err );
                spCommand = $"Select * from {filterargs [ 2 ]}";
                string [ ] args = new string [ 0 ];
                GridData = LoadFullSqlTable ( spCommand , args , out err , CurrentTableDomain );
            }
            //else if ( result == -9 )
            //{
            //    SetStatusbarText ( $"The Filter did NOT complete successfully, so the original table remains shown above..\nError Mesage returned was {err}...." , 1 );
            //    Debug . WriteLine ( err );
            //    closeFilter ( sender , e );
            //    Mouse . OverrideCursor = Cursors . Arrow;
            //}
            if ( GridData . Count ( ) == 0 )
            {
                MsgBoxArgs msgargs = new MsgBoxArgs ( );
                msgargs . title = "Stored Procedure Search System";
                msgargs . msg1 = $"Sorry, it does not appear that your filter term [ '{filtercmd . ToUpper ( )}' ] has resulted in NO RECORDS being found in the current Table";
                msgargs . msg2 = $"Please try a different Filter term that is more likely to result in more records being identified. ";
                msgargs . msg3 = $"Therefore the original table [{CurrentTable}] is still displayed in the datagrid for you to try again.";
                CustomMsgBox cmb = new CustomMsgBox ( );
                closeFilter ( sender , e );
                cmb . Focus ( );
                CurrentTable = originaltable;
                string ConString = GenericDbUtilities . CheckSetSqlDomain ( "" );
                if ( ConString == "" )
                {
                    GenericDbUtilities . CheckDbDomain ( "" );
                    ConString = MainWindow . CurrentSqlTableDomain;
                }
                string Resultstring = "";

                string [ ] args = new string [ 0 ];
                GridData = dgControl . LoadGenericData ( CurrentTable , args , true , ConString );
                Reccount . Text = GridData . Count . ToString ( );
                cmb . Show ( msgargs );
                Mouse . OverrideCursor = Cursors . Arrow;
            }
            else
            {
                int colcount = DatagridControl . GetColumnsCount ( GridData );
                DatagridControl . LoadActiveRowsOnlyInGrid ( dgControl . datagridControl , GridData , colcount );
                if ( ShowColumnHeaders == true )
                    ResetColumnHeaderToTrueNames ( GridData , CurrentTable , dgControl . datagridControl );
                else
                {
                    //List<DataTableLayout> dapperdglayoutlist = new List<DataTableLayout> ( );
                    DatagridControl . ReplaceDataGridFldNames ( GridData , ref dgControl . datagridControl , ref dglayoutlist , colcount );
                }
                Reccount . Text = GridData . Count . ToString ( );

                CurrentTable = filterargs [ 2 ];
                //ReLoad tables list to include our new temporary table, and select it
                LoadDbTables ( CurrentTable );
                if ( LOCALFILTER == true && CurrentTable == "LOCALFILTERTABLE" )
                {
                    List<string> newlist = new List<string> ( );
                    foreach ( var item in SqlTables . Items )
                    {
                        if ( item . ToString ( ) . ToUpper ( ) != "LOCALFILTERTABLE" )
                            newlist . Add ( item . ToString ( ) );
                        else
                            Debug . WriteLine ( $"found and removed LOCALFILTERTABLE!)" );
                    }
                    newlist . Add ( "DISPLAY ONLY ! " );
                    SqlTables . ItemsSource = null;
                    SqlTables . Items . Clear ( );
                    SqlTables . ItemsSource = newlist;

                    CurrentTable = "DISPLAY ONLY ! ";
                    if ( GridData . Count > 0 )
                        SetStatusbarText ( $"The TEMPORARY filter [{LastActiveFillter}] on {originaltable . ToUpper ( )} returned a total of {GridData . Count ( )} records displayed above" +
                            $"\nWARNING : As this is a TEMPORARY FILTER, the data will be lost as soon as you change the table viewed, unless you save it irst" , 3 );
                    GenGridCtrl . AlternateBackground = FindResource ( "Orange4" ) as SolidColorBrush;
                    GenGridCtrl . Refresh ( );

                }
                else
                {
                    if ( GridData . Count > 0 )
                        SetStatusbarText ( $"The filter [{LastActiveFillter}] on {originaltable . ToUpper ( )} resulted in a total of {GridData . Count ( )} records being found and loaded\n" +
                            $"into {CurrentTable} and these are shown in the viewer above..." , 2 );
                    GenGridCtrl . AlternateBackground = FindResource ( "Cyan4" ) as SolidColorBrush;
                    GenGridCtrl . Refresh ( );
                }
                ListReloading = true;
                SqlTables . SelectedItem = CurrentTable;
                ListReloading = false;
                StdSuccess ( );
                //if ( replacementtable == "FILTEREDDATA" )
                //    statusbar . Text = $"The Filter completed successfully and was placed into new Table '{replacementtable}' which is displayed in the table viewer ";
                //else if ( replacementtable == "FILTEREDDATA2" )
                //    statusbar . Text = $"The Filter completed successful, but because you the orignal Filtered table has been filtered a 2nd (or subsequent time) it has been saved to a seconndary 'Search results' Table named 'FilteredData2' which is shown above...";
                SqlTables . UpdateLayout ( );
                closeFilter ( sender , e );
                if ( LOCALFILTER == true )
                {
                    // drop table immediately as it is a display table only
                    spCommand = $"drop table localfiltertable";
                    //string [ ] args = new string [ 1 ];
                    //args [ 0 ] = "localfiltertable";
                    dgControl . ExecuteDapperTextCommand ( spCommand , null , out err );
                }
                Mouse . OverrideCursor = Cursors . Arrow;
            }
        }

        #endregion FILTERING SUPPORT incl moving it around

        //****************************************************//
        #region Search SP's for specific text
        //****************************************************//

        private void stopBtn2_Click ( object sender , RoutedEventArgs e )
        {
            // close small dialog wih Text Entry field
            SpStringsSelector . Visibility = Visibility . Collapsed;
            DragCtrl . MovementEnd ( sender , e );
            selectedSp . Focus ( );
        }

        private void GoBtn2_Click ( object sender , RoutedEventArgs e )
        {
            //************************************************************************//
            // load SP that matches that selected in the Larger dialog listbox
            //************************************************************************//
            DataTable dt = new DataTable ( );
            List<string> list = new ( );
            Mouse . OverrideCursor = Cursors . Wait;
            // if we have a search term, open the ppecific SP file
            "" . Track ( );
            if ( Searchtext != "" )
            {
                string sptext = "";
                //Task . Run ( async ( ) =>
                //{
                string stringresult = "";
                Gengrid . FetchStoredProcedureCode ( Splist . SelectedItem . ToString ( ) , ref sptext );
                //} );
                //string stringresult = "";
                //string sptext = FetchStoredProcedureCode ( ProcNames . SelectedItem . ToString ( ) , ref stringresult );
                if ( sptext == "" )
                {
                    Debug . WriteLine ( $"ERROR - no SP file   was returned ????" );
                    Mouse . OverrideCursor = Cursors . Arrow;
                    "" . Track ( 1 );
                    return;
                }
                infotext = sptext;
                // now display the full content of the seleted S.P that has  the search text in it
                //FlowDocument myFlowDocument = new FlowDocument ( );
                // Highlight  the search ext in SP file
                RTBox . Document = null;
                myFlowDocument . Blocks . Clear ( );

                myFlowDocument = CreateBoldString ( myFlowDocument , sptext , Searchtext );
                myFlowDocument . Background = FindResource ( "Black3" ) as SolidColorBrush;
                RTBox . Document = myFlowDocument;
                RTBox . Visibility = Visibility . Visible;
                InfoGrid . Visibility = Visibility . Visible;

                //                dgControl . datagridControl . Visibility = Visibility . Collapsed;

                // Load FULL list of ALL SP's that match searchterm 
                // into left column of  our viewer panel of our Viewergrid
                if ( Splist . Items . Count == 0 )
                {
                    list = LoadMatchingStoredProcs ( Splist , Searchtext );
                    //Splist . ItemsSource = null;
                    //Splist . ItemsSource = list;
                    // // default to 1st  entry in list
                    //Splist . SelectedIndex = 0;
                    //InfoHeaderPanel . Text = SpInfo . Text;
                    //SpInfo . Text = $"All  Matching S.P's";
                    //SpInfo2 . Text = $"{Splist . Items . Count} match [{Searchtext}]";
                    //InfoHeaderPanel . Text = $"All ({Splist . Items . Count}) Stored Procedures matching Search Term [ {Searchtext} ] are displayed";
                    CurrentSpList = "MATCH";

                    // open splitter
                    ViewerGrid . ColumnDefinitions [ 0 ] . Width = new GridLength ( 200 , GridUnitType . Pixel );
                }
            }
            else
            {
                // No search term  exists, so get one now
            }
            // hide main grid - just for neatness
            dgControl . datagridControl . Visibility = Visibility . Visible;
            Mouse . OverrideCursor = Cursors . Arrow;
            "" . Track ( 1 );
        }
        public string FetchStoredProcedureCode ( string spName , ref string stringresult , bool HeaderOnly = false )
        {
            // Load a specified SP file annd show in Scrollviewer
            stringresult = "";
            DataTable dt = new ( );
            string output = "";
            if ( spName == null )
            {
                stringresult = output;
                return stringresult;
            }
            dt = DatagridControl . ProcessSqlCommand ( $"spGetSpecificScript  '{spName}'" , Flags . CurrentConnectionString );
            List<string> list = new List<string> ( );
            List<string> headeronlylist = new List<string> ( );
            foreach ( DataRow row in dt . Rows )
            {
                list . Add ( row . Field<string> ( 0 ) );
            }
            if ( HeaderOnly )
            {
                list [ 0 ] = GetSpHeaderTextOnly ( list [ 0 ] );
                // now display the full content of the seleted S.P
                if ( list . Count > 0 )
                    output = list [ 0 ];
                // infotext = output;
                stringresult = output;
                return stringresult;
                //return output;
            }
            // now display the full content of the seleted S.P
            if ( list . Count > 0 )
                output = list [ 0 ];
            stringresult = output;
            return stringresult;
        }
        public string GetSpHeaderTextOnly ( string spText )
        {
            // strip the header block only out of any Stored Procedure, (ending after the AS line)
            int counter = 0;
            int line1 = 0;
            int line2 = 0;
            string testline = "";
            string [ ] lines = spText . Split ( "\r\n" );

            foreach ( string item in lines )
            {
                testline = item . Trim ( ) . ToUpper ( );
                if ( testline == "" )
                { counter++; continue; }
                if ( testline . Length >= 2
                    && ( testline . Substring ( 0 , 2 ) == "AS"
                    || testline . Length >= 4 && testline . Substring ( 0 , 4 ) == "--AS" ) )
                {
                    line1 = counter;
                    //continue;
                }
                else if ( testline . Length >= 5
                    && ( testline . Substring ( 0 , 5 ) == "BEGIN"
                    || testline . Length >= 7 && testline . Substring ( 0 , 7 ) == "--BEGIN" ) )
                {
                    line2 = counter;
                }
                counter++;
                if ( line1 > 0 && line2 > 0 )
                {
                    for ( int x = 0 ; x < counter - 1 ; x++ )
                        testline += lines [ x ] + "\r\n";
                    spText = testline;
                    break;
                }
            }
            return spText;
        }
        private void LoadRTbox ( )
        {
            //FlowDocument myFlowDocument = new FlowDocument ( );
            infotext = File . ReadAllText ( @$"C:\users\ianch\documents\GenericGridInfo.Txt" );
            myFlowDocument = LoadFlowDoc ( RTBox , FindResource ( "Black3" ) as SolidColorBrush , infotext );
        }
        public FlowDocument CreateFlowDocumentScroll ( string line1 , string clr1 = "" , string line2 = "" , string clr2 = "" , string line3 = "" , string clr3 = "" , string header = "" , string clr4 = "" ,
            int fontsize = 0 , string fground = "" , string bground = "" )
        {
            FlowDocument myFlowDocument = new FlowDocument ( );
            Paragraph para1 = new Paragraph ( );
            //NORMAL
            // This is  the only paragraph that uses the user defined Font Size....
            if ( fontsize == 0 )
                para1 . FontSize = 14;
            else
                para1 . FontSize = fontsize;
            para1 . FontFamily = new FontFamily ( "Arial" );
            if ( USERRTBOX )
                para1 . Foreground = FindResource ( "White0" ) as SolidColorBrush;
            else
                para1 . Foreground = FindResource ( "Black0" ) as SolidColorBrush;
            // handle user defined optional parameters
            if ( fground != "" )
                para1 . Foreground = FindResource ( fground ) as SolidColorBrush;
            if ( bground != "" )
                para1 . Background = FindResource ( bground ) as SolidColorBrush;
            Thickness th = new Thickness ( );
            th . Top = 10;
            th . Left = 10;
            th . Right = 10;
            th . Bottom = 10;
            para1 . Padding = th;
            para1 . Inlines . Add ( new Run ( line1 ) );
            //Add paragraph to flowdocument
            myFlowDocument . Blocks . Clear ( );
            myFlowDocument . Blocks . Add ( para1 );
            return myFlowDocument;
        }
        public FlowDocument CreateBoldString ( FlowDocument myFlowDocument , string SpText , string SrchTerm )
        {
            string original = SpText;
            string originalSearchterm = "";
            original = NewWpfDev . Utils . CopyCollection ( SpText , original );
            string input = SpText . ToUpper ( );
            string [ ] NonSearchText;
            List<int> NonSearchTextlength = new List<int> ( );
            List<string> NonCapitlisedString = new List<string> ( );
            originalSearchterm = SrchTerm;
            int newpos = 0;
            SrchTerm = SrchTerm . ToUpper ( );

            // split source down based on searchterm (using non capitalised version
            // // Only searchterm is capitalised !!!!))
            NonSearchText = input . Split ( $"{SrchTerm}" );
            foreach ( var item in NonSearchText )
            {
                NonSearchTextlength . Add ( item . Length );
            }
            for ( int x = 0 ; x < NonSearchTextlength . Count ; x++ )
            {
                string temp = original . Substring ( newpos , NonSearchTextlength [ x ] );
                NonCapitlisedString . Add ( temp );
                newpos += NonSearchTextlength [ x ] + SrchTerm . Length;
            }
            // Now create a (formatted) list of lines from all  paragraphs identified previously
            // using temp paragraph so I can access it from my public para variable
            Paragraph tmppara = new Paragraph ( );

            for ( int x = 0 ; x < NonCapitlisedString . Count ; x++ )
            {
                Run run1 = AddStdNewDocumentParagraph ( NonCapitlisedString [ x ] , SrchTerm );
                tmppara . Inlines . Add ( run1 );
                Run run2 = AddDecoratedNewDocumentParagraph ( NonCapitlisedString [ x ] , SrchTerm );

                if ( x < NonCapitlisedString . Count - 1 )
                    tmppara . Inlines . Add ( run2 );
            }
            para1 = tmppara;
            // build  document by adding all blocks to Document
            myFlowDocument . Blocks . Add ( para1 );
            return myFlowDocument;
        }

        public Run AddStdNewDocumentParagraph ( string textstring , string SearchText )
        {
            // Main text
            Run run1 = new Run ( textstring );
            run1 . FontSize = 16;
            run1 . FontFamily = new FontFamily ( "Arial" );
            run1 . FontWeight = FontWeights . Normal;
            run1 . Background = FindResource ( "Black3" ) as SolidColorBrush;
            run1 . Foreground = FindResource ( "White0" ) as SolidColorBrush;
            return run1;
        }
        public Run AddDecoratedNewDocumentParagraph ( string textstring , string SearchText )
        {
            Run run2 = new Run ( SearchText );
            run2 . FontSize = 18;
            run2 . FontFamily = new FontFamily ( "Arial" );
            run2 . FontWeight = FontWeights . Bold;
            run2 . Foreground = FindResource ( "Green5" ) as SolidColorBrush;
            run2 . Background = FindResource ( "Black3" ) as SolidColorBrush;
            return run2;
        }
        public List<string> LoadMatchingStoredProcs ( ListBox lbox , string Searchtext )
        {
            // This  takes a while
            DataTable dt = new DataTable ( );
            string SqlCommand = $"spGetAllSprocsMatchingSearchterm";
            string currselection = "";
            Mouse . OverrideCursor = Cursors . Wait;
            string [ ] args = new string [ 1 ];
            args [ 0 ] = Searchtext;
            dt = DatagridControl . ProcessSqlCommand ( SqlCommand , Flags . CurrentConnectionString , args );
            // parse Datatablle into list<string>
            List<string> list = GetDataDridRowsAsListOfStrings ( dt );
            lbox . ItemsSource = null;
            lbox . Items . Clear ( );
            if ( list . Count > 0 )
            {
                foreach ( var item in list )
                {
                    lbox . Items . Add ( item as string );
                }
                lbox . SelectedIndex = 0;
                // show sp listbox dialog
            }
            Mouse . OverrideCursor = Cursors . Arrow;
            // return the list in case we need it later on
            return list;
        }
        private void GoBtn1_Click ( object sender , RoutedEventArgs e )
        {
            // load all SP's that contain the specified search term
            // and add them to listbox in larger selection dialog
            Searchtext = selectedSp . Text;
            List<string> list = LoadMatchingStoredProcs ( Splist , selectedSp . Text );
            if ( list . Count == 0 )
            {
                MsgBoxArgs msgargs = new MsgBoxArgs ( );
                msgargs . title = "Stored Procedure Search System";
                msgargs . msg1 = $"Sorry, it does not appear that the search term [ '{Searchtext . ToUpper ( )}' ]\nhas been found in any of the Stored Procedures in the current SQL Server\n\nTherefore the list still shows ALL SP's....";
                msgargs . msg2 = $"Please enter a search item likely to be found in a Stored Procedure . ";
                CustomMsgBox cmb = new CustomMsgBox ( );
                cmb . Show ( msgargs );
                return;
            }
            DragCtrl . InitializeMovement ( SpStringsSelector as FrameworkElement );
            Splist . ItemsSource = null;
            Splist . Items . Clear ( );
            Splist . ItemsSource = list;
            if ( Resultsviewer != null )
                SetSpWindowInfoText ( this , this , Searchtext );

            CurrentSpList = "MATCH";

            ViewerGrid . ColumnDefinitions [ 0 ] . Width = new GridLength ( 200 , GridUnitType . Pixel );
            ViewerGrid . Visibility = Visibility . Visible;
            DisplayInformationViewer ( true );
            SpStringsSelection . Visibility = Visibility . Collapsed;
            Splist . SelectedIndex = 0;
            ReloadInfo ( sender , e );
            Mouse . OverrideCursor = Cursors . Arrow;
        }

        private void ProcNames_MouseDoubleClick ( object sender , MouseButtonEventArgs e )
        {
            //Load Execution window
            GoBtn2_Click ( null , null );
            e . Handled = true;
        }

        private void SpStrings_KeyDown ( object sender , KeyEventArgs e )
        {
            // Called by Large dialog to show Matching SP
            if ( e . Key == Key . Enter )
                GoBtn2_Click ( null , null );
            if ( e . Key == Key . Escape )
            {
                SpStringsSelector . Visibility = Visibility . Collapsed;
                selectedSp . Focus ( );
            }
        }

        private void InfoGrid_IsVisibleChanged ( object sender , DependencyPropertyChangedEventArgs e )
        {
            Grid grid = sender as Grid;
            Visibility visibility = new Visibility ( );
        }

        private void CloseIcon_MouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
        {
            InfoGrid . Visibility = Visibility . Collapsed;
            dgControl . datagridControl . Visibility = Visibility . Visible;
            Currentpanel = "GRID";
            //GridVisible . IsEnabled = false;
            //ViewerVisible . IsEnabled = true;
        }

        private void MenuItem_Click ( object sender , RoutedEventArgs e )
        {
            //**********************************//
            // Switching Datagrid to full screen
            //**********************************//
            majorgrid . RowDefinitions [ 0 ] . Height = new GridLength ( 2 , GridUnitType . Star );
            majorgrid . RowDefinitions [ 2 ] . Height = new GridLength ( 5 , GridUnitType . Pixel );
            double row0height = dgrow . ActualHeight + vwrow . ActualHeight;
            SPViewerOpen = false;
            GenGridCtrl . Height = row0height;    // WORKING
            Gengrid_SizeChanged ( null , null );
            GenGridCtrl . UpdateLayout ( );
            Gengrid_SizeChanged ( null , null );
            majorgrid . UpdateLayout ( );
            RTBox . Width = ViewerGrid . Width - Splist . ActualWidth - Vsplitter . ActualWidth;
        }
        private void ReloadInfo ( object sender , RoutedEventArgs e )
        {
            //****************************************//
            //switching back to info panel full screen
            //****************************************//
            Currentpanel = "INFO";

            majorgrid . RowDefinitions [ 0 ] . Height = new GridLength ( 5 , GridUnitType . Pixel );
            majorgrid . RowDefinitions [ 2 ] . Height = new GridLength ( 2 , GridUnitType . Star );
            double row2height = dgrow . ActualHeight + vwrow . ActualHeight;
            // Set global flag to indicate which viewer is visible
            SPViewerOpen = true;

            InfoGrid . Height = row2height + 40;
            InfoBorder . Height = InfoGrid . Height - 50;
            RTBox . Height = InfoGrid . Height - 120;
            InfoGrid . UpdateLayout ( );
            InfoBorder . UpdateLayout ( );
            RTBox . UpdateLayout ( );
            Gengrid_SizeChanged ( null , null );
            majorgrid . UpdateLayout ( );
            RTBox . Width = ViewerGrid . Width - Splist . ActualWidth - Vsplitter . ActualWidth;

        }

        private void SearchStoredProc ( object sender , RoutedEventArgs e )
        {
            SpStringsSelection . Visibility = Visibility . Visible;
            selectedSp . Focus ( );
            DragCtrl . InitializeMovement ( SpStringsSelection as FrameworkElement );
            selectedSp . SelectAll ( );
        }

        private void Button_KeyDown ( object sender , KeyEventArgs e )
        {
            if ( e . Key == Key . Escape )
            {
                Filtering . Visibility = Visibility . Collapsed;
                DragCtrl . MovementEnd ( sender , e );
            }
        }

        private void stopBtn1_KeyDown ( object sender , KeyEventArgs e )
        {
            if ( e . Key == Key . Escape )
                SpStringsSelection . Visibility = Visibility . Collapsed;
        }
        private void RTBox_MouseDblClick ( object sender , MouseButtonEventArgs e )
        {
            InfoGrid . Visibility = Visibility . Collapsed;
            dgControl . datagridControl . Visibility = Visibility . Visible;
            if ( SpStringsSelector . Visibility == Visibility . Visible )
                SpStringsSelector . Visibility = Visibility . Collapsed;
        }

        #endregion Search SP's for specific text                

        #region SpList handlers
        private void Splist_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {
            //info viewer listbox for SP's
            string sptext = "";
            string spfilename = "";
            if ( Splist . Items . Count == 0 )
                return;
            //FlowDocument myFlowDocument = new FlowDocument ( );
            if ( Splist . Items . Count > 0 && Splist . SelectedItem == null )
                Splist . SelectedIndex = 0;
            if ( Splist . SelectedItem != null )
            {
                spfilename = Splist . SelectedItem . ToString ( );
                // set global pointer to Genericgrid's selecteditem
                CurrentSpSelection = Splist . SelectedItem . ToString ( );
            }
            else
            {
                Splist . SelectedIndex = 0;
                Splist . SelectedItem = 0;
                spfilename = Splist . SelectedItem . ToString ( );
            }
            SpLastSelection = spfilename;
            //string stringresult = "";
            SplistRightclick = false;


            // load a matching SP and show it in ScrolFlowDocument
            bool result = LoadShowMatchingSproc ( this , RTBox , spfilename , ref sptext );
            {
                //Gengrid . FetchStoredProcedureCode ( spfilename , ref sptext );
                //if ( sptext == "" )
                //{
                //    StdError ( );
                //    SetStatusbarText ( $"Failed to read the Stored Procedure {Splist . SelectedItem . ToString ( )}" , 1 );
                //    return;
                //}
                //infotext = sptext;
                //RTBox . Document = null;
                //myFlowDocument = new FlowDocument ( );
                //myFlowDocument . Blocks . Clear ( );
                //myFlowDocument = CreateBoldString ( myFlowDocument , sptext , Searchtext );
                //myFlowDocument . Background = FindResource ( "Black3" ) as SolidColorBrush;
                //RTBox . Document = myFlowDocument;
            }

            // open list cos we are opening SP viewer panel   annnd set splitter   up as needed
            GridLength gl = new GridLength ( );
            gl = ViewerGrid . ColumnDefinitions [ 0 ] . Width;
            if ( gl . Value < 5 )
                ViewerGrid . ColumnDefinitions [ 0 ] . Width = new GridLength ( 200 , GridUnitType . Pixel );

            ViewerGrid . Visibility = Visibility . Visible;
            RTBox . UpdateLayout ( );
            SetStatusbarText ( $"Stored Procedure [ {Splist . SelectedItem . ToString ( ) . ToUpper ( )}] matches Search Term {Searchtext} now shown in Right hand panel of the viewer ]" );
            Mouse . OverrideCursor = Cursors . Arrow;
        }
        private void Splist_PreviewMouseRightButtonDown ( object sender , MouseButtonEventArgs e )
        {
            SplistRightclick = true;
            List<string> deleteitems = new List<string> ( );
            deleteitems . Add ( "cm2" );
            deleteitems . Add ( "cm6" );
            deleteitems . Add ( "cm7" );
            deleteitems . Add ( "cm8" );
            deleteitems . Add ( "cm9" );
            deleteitems . Add ( "cm11" );

            ContextMenu menu = RemoveMenuItems ( "PopupMenu" , "" , deleteitems );
            "aplist_previewMouseRightButtonDown" . cwinfo ( );
            menu = AddMenuItem ( "PopupMenu" , "cm15" );
            menu . IsOpen = true;
            //e . Handled = true;
        }

        private void Splist_PreviewMouseRightButtonUp ( object sender , MouseButtonEventArgs e )
        {
            SplistRightclick = false;
        }

        #endregion SpList handlers

        private void CloseApp_Click ( object sender , RoutedEventArgs e )
        {
            Application . Current . Shutdown ( );
        }

        /// <summary>
        /// A clever method that loads/reloads the list of SP's in the left hand listboxes
        /// of Genericgrid and SpResultsViewer windows that loads ONLY those SP's that
        /// contain the current search term.
        /// 
        /// It has to do some clever stuff because Genericgrid calls it from a Context menu, so we have to 
        /// discover who the caller is and GENERECISE it.  See Below how I did that !!!!!!!!
        /// </summary>
        /// <param name="sender">Calling window</param>
        /// <param name="e">Std arguments</param>
        private void LoadMatchingSPs_Click ( object sender , RoutedEventArgs e )
        {
            // Careful = calls from Context Menu will NOT find a window, so check it here 1st
            ListBox lbox = null;
            string callertype = "";
            Window dgobj = null;
            dgobj = DapperGenericsLib . Utils . FindVisualParent<Window> ( e . OriginalSource as DependencyObject );
            if ( dgobj == null )
            {
                ContextMenu cmenu = null;
                cmenu = DapperGenericsLib . Utils . FindVisualParent<ContextMenu> ( e . OriginalSource as DependencyObject );
                if ( cmenu . GetType ( ) == typeof ( ContextMenu ) )
                    callertype = "GENGRID";
                Debug . WriteLine ( "Context Menu is the caller" );
            }
            if ( dgobj != null )
            {
                if ( dgobj . GetType ( ) == typeof ( Genericgrid ) )
                    callertype = "GENGRID";
                else if ( dgobj . GetType ( ) == typeof ( SpResultsViewer ) )
                    callertype = "RESVIEW";
            }
            // Dbl click or context mnu click in SP list, so load only matching SP's
            string currItem = "";
            string srchterm = Searchtext;
            if ( callertype == "GENGRID" || dgobj == null )
            {
                lbox = Splist;
                // must be Generigrid if dgobk == null
                if ( Splist . Items . Count > 0 )
                {
                    if ( Splist . SelectedItem == null )
                        currItem = Splist . Items [ 0 ] . ToString ( );
                    else
                        currItem = Splist . SelectedItem . ToString ( );
                }
                else
                    return;
            }
            else if ( callertype == "RESVIEW" )
            {
                lbox = spviewer . ListResults;

                if ( lbox . Items . Count > 0 )
                {
                    if ( lbox . SelectedItem == null )
                        currItem = lbox . Items [ 0 ] . ToString ( );
                    else
                        currItem = lbox . SelectedItem . ToString ( );
                }
                else
                    return;
            }
            if ( Searchtext != "" )
            {
                string srchtext = Searchtext;
                // call sql to get SP list and load it into listbox
                List<string> list = null;
                if ( callertype == "RESVIEW" )
                    list = LoadMatchingStoredProcs ( spviewer . ListResults , srchterm );
                else
                    list = LoadMatchingStoredProcs ( GenGrid . Splist , srchterm );
                if ( currItem != null && currItem != "" )
                    lbox . SelectedItem = currItem;

                // Update the resultsviewer as well (if open)
                lbox . ItemsSource = null;
                lbox . Items . Clear ( );
                lbox . ItemsSource = list;
                if ( currItem != null && currItem != "" )
                {
                    lbox . SelectedItem = currItem;
                    lbox . ScrollIntoView ( currItem );
                }
                if ( callertype == "RESVIEW" )
                    spviewer . Bannerline . Text = $"Stored Procedures Helper ({list . Count}) SP's Matching  [{srchterm}] shown)";
                // not  used
                if ( Resultsviewer != null )
                {
                    //    Genericgrid . Resultsviewer . ListResults . ItemsSource = null;
                    //    Genericgrid . Resultsviewer . ListResults . Items . Clear ( );
                    //    Genericgrid . Resultsviewer . ListResults . ItemsSource = list;
                    //    if ( currItem != null && currItem != "" )
                    //    {
                    //        Genericgrid . Resultsviewer . ListResults . SelectedItem = currItem;
                    //        Genericgrid . Resultsviewer . ListResults . ScrollIntoView ( currItem );
                    //    }
                    //    Resultsviewer . Bannerline . Text = $"Stored Procedures Helper ({list . Count}) SP's Matching  [{srchterm}] shown)";
                }
                if ( lbox . SelectedItem == null )
                {
                    // No selection in listbox, so tidy up scroll viewer
                    if ( callertype == "GENGRID" )
                    {
                        RTBox . Document = null;
                        myFlowDocument . Blocks . Clear ( );
                        if ( Splist . Items . Count > 0 )
                        {
                            lbox . ScrollIntoView ( lbox . Items [ 0 ] . ToString ( ) );
                            SetStatusbarText ( $"({lbox . Items . Count}) S.P's matching [{Searchtext}] loaded successfully, but the previous S.P [ {currItem} ] is not in\nthe new (filtered) list which is why the viewer is empty" , 1 );
                            StdError ( );
                        }
                        else
                        {
                            SetStatusbarText ( $"(The system did not return ANY S.P's matching [{Searchtext}] which is why the List and viewer are empty" , 1 );
                            StdError ( );
                        }
                    }
                    else if ( callertype == "RESVIEW" )
                    {
                        spviewer . TextResult . Document = null;
                        myFlowDocument . Blocks . Clear ( );
                        if ( lbox . Items . Count > 0 )
                        {
                            lbox . ScrollIntoView ( lbox . Items [ 0 ] . ToString ( ) );
                            //SetStatusbarText ( $"({lbox . Items . Count}) S.P's matching [{Searchtext}] loaded successfully, but the previous S.P [ {currItem} ] is not in\nthe new (filtered) list which is why the viewer is empty" , 1 );
                            StdError ( );
                        }
                        else
                        {
                            SetStatusbarText ( $"(The system did not return ANY S.P's matching [{Searchtext}] which is why the List and viewer are empty" , 1 );
                            StdError ( );
                        }
                        SetStatusbarText ( $"({lbox . Items . Count}) S.P's matching [{Searchtext}] loaded successfully..." );
                    }
                }

                // udate cosmetics
                if ( callertype == "GENGRID" )
                {
                    Genericgrid . UsingMatches = true;
                    SetSpWindowInfoText ( Gengrid , Gengrid , Searchtext );
                    // update Genericgrid's Scroll viewer panel and headers with SP contents
                    bool success = LoadShowMatchingSproc ( GenGrid , RTBox , lbox . SelectedItem . ToString ( ) , ref srchterm );
                }
                else
                {
                    spviewer . UsingMatches = true;
                    SetSpWindowInfoText ( spviewer , spviewer , Searchtext );
                    // update Genericgrid's Scroll viewer panel and headers with SP contents
                    bool success = LoadShowMatchingSproc ( spviewer , spviewer . TextResult , lbox . SelectedItem . ToString ( ) , ref srchterm );
                }
                CurrentSpList = "MATCH";
            }
            else
                SpStringsSelection . Visibility = Visibility . Visible;

            GridLength gl = new GridLength ( );
            gl = ViewerGrid . ColumnDefinitions [ 0 ] . Width;
            if ( gl . Value < 5 )
                ViewerGrid . ColumnDefinitions [ 0 ] . Width = new GridLength ( 200 , GridUnitType . Pixel );
            Mouse . OverrideCursor = Cursors . Arrow;
        }

        private void LoadAllSPs_Click ( object sender , RoutedEventArgs e )
        {
            LoadAllSPs ( );
        }
        public void  LoadAllSPs ( )
        {
            string currItem = "";
            if ( Splist . Items . Count > 0 )
            {
                if ( Splist . SelectedItem == null )
                    currItem = Splist . Items [ 0 ] . ToString ( );
                else
                    currItem = Splist . SelectedItem . ToString ( );
            }
            List<string> SpList = new List<string> ( );
            SpList = SProcsDataHandling . CallStoredProcedure ( SpList , "spGetStoredProcs" );
            Splist . ItemsSource = null;
            Splist . Items . Clear ( );
            Splist . ItemsSource = SpList;
            Splist . SelectedItem = currItem;
            Splist . ScrollIntoView ( currItem );
            Mouse . OverrideCursor = Cursors . Arrow;
            SetStatusbarText ( $"All ({Splist . Items . Count}) S.P's loaded successfully..." );
            SpInfo . Text = SpInfo . Text = $"All S.Procs ";
            SpInfo2 . Text = $"{Splist . Items . Count} available...";
            InfoHeaderPanel . Text = $"All ({Splist . Items . Count}) Stored Procedures are listed";
            CurrentSpList = "ALL";

            // Context menu options - all correct  
            //LoadAllItems . IsEnabled = false;
            //LoadMatchingItems . IsEnabled = true;

            // Force list to show()
            GridLength gl = new GridLength ( );
            gl = ViewerGrid . ColumnDefinitions [ 0 ] . Width;
            if ( gl . Value < 5 )
                ViewerGrid . ColumnDefinitions [ 0 ] . Width = new GridLength ( 200 , GridUnitType . Pixel );
        }
            private void Fontsizeup_Click ( object sender , RoutedEventArgs e )
        {
            para1 . FontSize += 1;
        }

        private void Fontsizedn_Click ( object sender , RoutedEventArgs e )
        {
            para1 . FontSize -= 1;
        }

        public FlowDocument LoadFlowDoc ( FlowDocumentScrollViewer ctrl , SolidColorBrush BkgrndColor = null , string item1 = "" , string clr1 = "" , string item2 = "" , string clr2 = "" , string item3 = "" , string clr3 = "" , string header = "" , string clr5 = "" )
        {
            //FlowDocument myFlowDocument = new FlowDocument ( );
            myFlowDocument = CreateFlowDocumentScroll ( item1 , clr1 , item2 , clr2 , item3 , clr3 , header , clr5 );
            ctrl . Document = myFlowDocument;

            myFlowDocument . Background = BkgrndColor;
            ctrl . UpdateLayout ( );
            return myFlowDocument;
        }

        //********************//
        #region ActiiveDrag helpers
        private void Filtering_DragDialog_LButtonDn ( object sender , MouseButtonEventArgs e )
        {
            ActiveDragControl = Filtering;
            DragDialog_LButtonDn ( sender , e );
        }

        private void Filtering_DragDialog_Ending ( object sender , MouseButtonEventArgs e )
        {
            ActiveDragControl = Filtering;
            DragDialog_Ending ( sender , e );
        }

        private void Filtering_DragDialog_Moving ( object sender , MouseEventArgs e )
        {
            ActiveDragControl = Filtering;
            DragDialog_Moving ( sender , e );
        }

        private void ColSelect_DragDialog_LButtonDn ( object sender , MouseButtonEventArgs e )
        {
            Type type = e . OriginalSource . GetType ( );
            Type type2 = sender . GetType ( );
            var dgobj = DapperGenericsLib . Utils . FindVisualParent<DataGrid> ( e . OriginalSource as DependencyObject );
            var obj = DapperGenericsLib . Utils . FindVisualParent<Button> ( e . OriginalSource as DependencyObject );
            Debug . WriteLine ( $"Btn Dn : obj={obj}, dgobj={dgobj}, type={type}, type2={type2}" );
            if ( dgobj != null )
                return;
            else if ( obj != null )
            {
                if ( obj . Name == "stopBtn" )
                    stopBtn_Click ( sender , e );
                if ( obj . Name == "GoBtn" )
                    GoBtn_Click ( sender , e );
                return;
            }
            else if ( type != typeof ( ScrollViewer ) && type != typeof ( Button ) )
            {
                ActiveDragControl = FieldSelectionGrid;
                this . MovingObject = ActiveDragControl;
                Debug . WriteLine ( $"Calling _Move" );

                //DragCtrlHelper dch = new DragCtrlHelper ( );
                DragCtrl . InitializeMovement ( FieldSelectionGrid );
                DragCtrl . MovementStart ( sender , e );
                DragCtrl . CtrlMoving ( FieldSelectionGrid , e );

                //ColSelect_DragDialog_Moving ( sender , e );
                //               DragDialog_LButtonDn ( sender , e );
            }
        }

        private void ColSelect_DragDialog_Moving ( object sender , MouseEventArgs e )
        {
            Type type = e . OriginalSource . GetType ( );
            Type type2 = sender . GetType ( );
            if ( MovingObject == null || ActiveDragControl == null )
                return;

            var dgobj = DapperGenericsLib . Utils . FindVisualParent<DataGrid> ( e . OriginalSource as DependencyObject );
            var obj = DapperGenericsLib . Utils . FindVisualParent<Button> ( e . OriginalSource as DependencyObject );
            if ( MovingObject != null && ActiveDragControl != null && dgobj != null )
            {
                //DragCtrlHelper dch = new DragCtrlHelper ( );
                DragCtrl . CtrlMoving ( FieldSelectionGrid , e );

                //                Debug . WriteLine ( $"Moving : {dgobj . Name}, {obj . Name}" );
                return;
            }
            else if ( MovingObject == null && ActiveDragControl == null && obj != null )
            {
                if ( obj . Name == "stopBtn" )
                    stopBtn_Click ( sender , e );
                if ( obj . Name == "GoBtn" )
                    GoBtn_Click ( sender , e );
                return;
            }
            else if ( type != typeof ( ScrollViewer ) && type != typeof ( Button ) )
            {
                ActiveDragControl = FieldSelectionGrid;
                //MovingObject = ActiveDragControl;
                Debug . WriteLine ( $"In Moving !!!! movingobject = {MovingObject . GetType ( )}, {ActiveDragControl . Name}" );
                //DragCtrlHelper dch = new DragCtrlHelper ( );
                DragCtrl . CtrlMoving ( FieldSelectionGrid , e );

                ////DragDialog_LButtonDn ( sender , e );
            }
        }

        private void ColSelect_DragDialog_Ending ( object sender , MouseButtonEventArgs e )
        {
            //Type type = e . OriginalSource . GetType ( );
            //Type type2 = sender . GetType ( );
            //var dgobj = DapperGenericsLib . Utils . FindVisualParent<DataGrid> ( e . OriginalSource as DependencyObject );
            //var obj = DapperGenericsLib . Utils . FindVisualParent<Button> ( e . OriginalSource as DependencyObject );
            //if ( dgobj != null )
            //    return;
            //else if ( obj != null )
            //{
            //    if ( obj . Name == "stopBtn" )
            //        stopBtn_Click ( sender , e );
            //    if ( obj . Name == "GoBtn" )
            //        GoBtn_Click ( sender , e );
            //    return;
            //}
            //else if ( type != typeof ( ScrollViewer ) && type != typeof ( Button ) )
            //{
            if ( ActiveDragControl != null )
                ActiveDragControl = null;
            //                DragDialog_LButtonDn ( sender , e );
            //}
        }

        private void Search2_DragDialog_LButtonDn ( object sender , MouseButtonEventArgs e )
        {
            ActiveDragControl = SpStringsSelector;
            DragDialog_LButtonDn ( sender , e );
        }

        private void Search2_DragDialog_Ending ( object sender , MouseButtonEventArgs e )
        {
            ActiveDragControl = SpStringsSelector;
            DragDialog_Ending ( sender , e );
        }

        private void Search2_DragDialog_Moving ( object sender , MouseEventArgs e )
        {
            //if ( sender . GetType ( ) == typeof ( Grid ) )
            //{
            //    e . Handled = true;
            //    return;
            //}
            ActiveDragControl = SpStringsSelector;
            DragDialog_Moving ( sender , e );
        }

        private void ChangeMatchingSPs_Click ( object sender , RoutedEventArgs e )
        {
            SpStringsSelection . Visibility = Visibility . Visible;
            selectedSp . Focus ( );
            selectedSp . Text = Searchtext;
            DragCtrl . InitializeMovement ( SpStringsSelection as FrameworkElement );
            selectedSp . SelectAll ( );
            CurrentSpList = "MATCH";
        }


        private void selectedSp_TextChanged ( object sender , TextChangedEventArgs e )
        {

        }

        //private void Search1_BlockFiltering_Moving ( object sender , MouseButtonEventArgs e )
        //{
        //    e . Handled = true;
        //    selectedSp . Focus ( );
        //}

        #endregion ActiiveDrag helpers
        //--------------------//

        public void SetStatusbarText ( string text , int isError = 0 )
        {
            Thickness th = new Thickness ( );
            th = statusbar . Padding;
            statusbar . Text = text;
            if ( text . Length < 200 && text . Contains ( "\n" ) == false )
            {
                th . Top = 15;
                statusbar . Padding = th;
            }
            else
            {
                th . Top = 5;
                statusbar . Padding = th;
            }
            statusbar . Foreground = FindResource ( "White0" ) as SolidColorBrush;
            if ( isError == 0 )
                statusbar . Background = FindResource ( "White7" ) as SolidColorBrush;
            if ( isError == 1 )
                statusbar . Background = FindResource ( "Red2" ) as SolidColorBrush;
            if ( isError == 2 )
            {
                statusbar . Background = FindResource ( "Green5" ) as SolidColorBrush;
                statusbar . Foreground = FindResource ( "Black0" ) as SolidColorBrush;
            }
            if ( isError == 3 )
            {
                statusbar . Background = FindResource ( "Orange4" ) as SolidColorBrush;
                statusbar . Foreground = FindResource ( "Black0" ) as SolidColorBrush;
            }
        }

        public static ObservableCollection<GenericClass> CreateGenericCollection (
            //ref ObservableCollection<DapperGenLib.GenericClass> collection,
            string SqlCommand ,
            string Arguments ,
            string WhereClause ,
            string OrderByClause ,
            ref string errormsg ,
            string domain = "IAN1" )
        {
            //====================================
            // Use DAPPER to run a Stored Procedure
            //====================================
            string result = "";
            bool HasArgs = false;
            int argcount = 0;
            //DbToOpen = "";
            errormsg = "";
            IEnumerable resultDb;
            //genericlist = new List<string>();
            string arg1 = "", arg2 = "", arg3 = "", arg4 = "";
            Dictionary<string , object> dict = new Dictionary<string , object> ( );

            //$"Entering " . dcwinfo();
            ObservableCollection<GenericClass> collection = new ObservableCollection<GenericClass> ( );

            string Con = NewWpfDev . Utils . GetCheckCurrentConnectionString ( CurrentTableDomain );
            using ( IDbConnection db = new SqlConnection ( Con ) )
            {
                try
                {
                    // Use DAPPER to run  Stored Procedure
                    try
                    {
                        // Parse out the arguments and put them in correct order for all SP's
                        if ( Arguments . Contains ( "'" ) )
                        {
                            bool [ ] argsarray = { false , false , false , false };
                            int argscount = 0;
                            //  int adownrgscount = 0;
                            // we maybe have args in quotes
                            string [ ] args = Arguments . Trim ( ) . Split ( '\'' );
                            for ( int x = 0 ; x < args . Length ; x++ )
                            {
                                if ( args [ x ] . Trim ( ) . Contains ( "," ) )
                                {
                                    string tmp = args [ x ] . Trim ( );
                                    if ( tmp . Substring ( tmp . Length - 1 , 1 ) == "," )
                                    {
                                        tmp = tmp . Substring ( 0 , tmp . Length - 1 );
                                        args [ x ] = tmp;
                                        argsarray [ x ] = true;
                                        argscount++;
                                    }
                                    else
                                    {
                                        if ( args [ x ] != "" )
                                        {
                                            argsarray [ x ] = true;
                                            argscount++;
                                        }
                                    }
                                }
                            }
                            for ( int x = 0 ; x < argsarray . Length ; x++ )
                            {
                                switch ( x )
                                {
                                    case 0:
                                        if ( argsarray [ x ] == true )
                                            arg1 = args [ x ];
                                        break;
                                    case 1:
                                        if ( argsarray [ x ] == true )
                                            arg2 = args [ x ];
                                        break;
                                    case 2:
                                        if ( argsarray [ x ] == true )
                                            arg3 = args [ x ];
                                        break;
                                    case 3:
                                        if ( argsarray [ x ] == true )
                                            arg4 = args [ x ];
                                        break;
                                }
                            }
                        }
                        else if ( Arguments . Contains ( "," ) )
                        {
                            string [ ] args = Arguments . Trim ( ) . Split ( ',' );
                            //string[] args = DbName.Split(',');
                            for ( int x = 0 ; x < args . Length ; x++ )
                            {
                                switch ( x )
                                {
                                    case 0:
                                        arg1 = args [ x ];
                                        if ( arg1 . Contains ( "," ) )              // trim comma off
                                            arg1 = arg1 . Substring ( 0 , arg1 . Length - 1 );
                                        break;
                                    case 1:
                                        arg2 = args [ x ];
                                        if ( arg2 . Contains ( "," ) )              // trim comma off
                                            arg2 = arg2 . Substring ( 0 , arg2 . Length - 1 );
                                        break;
                                    case 2:
                                        arg3 = args [ x ];
                                        if ( arg3 . Contains ( "," ) )         // trim comma off
                                            arg3 = arg3 . Substring ( 0 , arg3 . Length - 1 );
                                        break;
                                    case 3:
                                        arg4 = args [ x ];
                                        if ( arg4 . Contains ( "," ) )         // trim comma off
                                            arg4 = arg4 . Substring ( 0 , arg4 . Length - 1 );
                                        break;
                                }
                            }
                        }
                        else
                        {
                            // One or No arguments
                            arg1 = Arguments;
                            if ( arg1 . Contains ( "," ) )              // trim comma off
                                arg1 = arg1 . Substring ( 0 , arg1 . Length - 1 );
                        }
                        // Create our aguments using the Dynamic parameters provided by Dapper
                        var Params = new DynamicParameters ( );
                        if ( arg1 != "" )
                            Params . Add ( "Arg1" , arg1 , DbType . String , ParameterDirection . Input , arg1 . Length );
                        if ( arg2 != "" )
                            Params . Add ( "Arg2" , arg2 , DbType . String , ParameterDirection . Input , arg2 . Length );
                        if ( arg3 != "" )
                            Params . Add ( "Arg3" , arg3 , DbType . String , ParameterDirection . Input , arg3 . Length );
                        if ( arg4 != "" )
                            Params . Add ( "Arg4" , arg4 , DbType . String , ParameterDirection . Input , arg4 . Length );
                        // Call Dapper to get results using it's StoredProcedures method which returns
                        // a Dynamic IEnumerable that we then parse via a dictionary into collection of GenericClass  records
                        int colcount = 0;

                        if ( SqlCommand . ToUpper ( ) . Contains ( "SELECT " ) )
                        {
                            //                           $"Entering for 'Sql Select'" . dcwinfo();
                            //***************************************************************************************************************//
                            // Performing a standard SELECT command but returning the data in a GenericClass structure	  (Bank/Customer/Details/etc)
                            //WORKS JUST FINE
                            $"{SqlCommand}" . DapperTrace ( );
                            var reslt = db . Query ( SqlCommand , CommandType . Text );
                            //***************************************************************************************************************//
                            if ( reslt == null )
                            {
                                errormsg = "Resullt was null";
                                return null;
                            }
                            else
                            {
                                //Although this is duplicated  with the one below we CANNOT make it a method()
                                //errormsg = "DYNAMIC";
                                int dictcount = 0;
                                int fldcount = 0;
                                GenericClass gc = new GenericClass ( );

                                Dictionary<string , string> outdict = new Dictionary<string , string> ( );
                                //try
                                //{
                                foreach ( var item in reslt )
                                {
                                    try
                                    {
                                        // we need to create a dictionary for each row of data then add it to a GenericClass row then add row to Generics Db
                                        string buffer = "";
                                        List<int> VarcharList = new List<int> ( );
                                        DatagridControl . ParseDapperRow ( item , dict , out colcount );
                                        gc = new GenericClass ( );
                                        dictcount = 1;
                                        int index = 1;
                                        fldcount = dict . Count;
                                        string tmp = "";

                                        // Parse reslt.item into  single Dictionary record
                                        foreach ( var pair in dict )
                                        {
                                            try
                                            {
                                                if ( pair . Key != null && pair . Value != null )
                                                {
                                                    DatagridControl . AddDictPairToGeneric ( gc , pair , dictcount++ );
                                                    tmp = $"field{index++} = {pair . Value . ToString ( )}";
                                                    buffer += tmp + ",";
                                                    outdict . Add ( pair . Key , pair . Value . ToString ( ) );
                                                }
                                            }
                                            catch ( Exception ex )
                                            {
                                                $"Dictionary ERROR : {ex . Message}" . cwerror ( );
                                                result = ex . Message;
                                            }
                                        }

                                        //remove trailing comma
                                        string s = buffer . Substring ( 0 , buffer . Length - 1 );
                                        buffer = s;
                                        // We now  have ONE sinlge record, but need to add this  to a GenericClass structure 
                                        int reccount = 1;
                                        foreach ( KeyValuePair<string , string> val in outdict )
                                        {  //
                                            switch ( reccount )
                                            {
                                                case 1:
                                                    gc . field1 = val . Value . ToString ( );
                                                    break;
                                                case 2:
                                                    gc . field2 = val . Value . ToString ( );
                                                    break;
                                                case 3:
                                                    gc . field3 = val . Value . ToString ( );
                                                    break;
                                                case 4:
                                                    gc . field4 = val . Value . ToString ( );
                                                    break;
                                                case 5:
                                                    gc . field5 = val . Value . ToString ( );
                                                    break;
                                                case 6:
                                                    gc . field6 = val . Value . ToString ( );
                                                    break;
                                                case 7:
                                                    gc . field7 = val . Value . ToString ( );
                                                    break;
                                                case 8:
                                                    gc . field8 = val . Value . ToString ( );
                                                    break;
                                                case 9:
                                                    gc . field9 = val . Value . ToString ( );
                                                    break;
                                                case 10:
                                                    gc . field10 = val . Value . ToString ( );
                                                    break;
                                                case 11:
                                                    gc . field11 = val . Value . ToString ( );
                                                    break;
                                                case 12:
                                                    gc . field12 = val . Value . ToString ( );
                                                    break;
                                                case 13:
                                                    gc . field13 = val . Value . ToString ( );
                                                    break;
                                                case 14:
                                                    gc . field14 = val . Value . ToString ( );
                                                    break;
                                                case 15:
                                                    gc . field15 = val . Value . ToString ( );
                                                    break;
                                                case 16:
                                                    gc . field16 = val . Value . ToString ( );
                                                    break;
                                                case 17:
                                                    gc . field17 = val . Value . ToString ( );
                                                    break;
                                                case 18:
                                                    gc . field18 = val . Value . ToString ( );
                                                    break;
                                                case 19:
                                                    gc . field19 = val . Value . ToString ( );
                                                    break;
                                                case 20:
                                                    gc . field20 = val . Value . ToString ( );
                                                    break;
                                            }
                                            reccount += 1;
                                        }
                                        collection . Add ( gc );
                                    }
                                    catch ( Exception ex )
                                    {
                                        result = $"SQLERROR : {ex . Message}";
                                        errormsg = result;
                                        result . cwerror ( );
                                    }
                                    dict . Clear ( );
                                    outdict . Clear ( );
                                    dictcount = 1;
                                }
                                //}
                                //catch ( Exception ex )
                                //{
                                //    $"OUTER DICT/PROCEDURE ERROR : {ex . Message}" . cwerror ( );
                                //    result = ex . Message;
                                //    errormsg = result;
                                //}
                                //if ( errormsg == "" )
                                errormsg = $"DYNAMIC:{fldcount}";
                                return collection;
                            }
                        }
                        else
                        {
                            // probably a stored procedure ?  							
                            bool IsSuccess = false;
                            int fldcount = 0;
                            GenericClass gc = new GenericClass ( );

                            //***************************************************************************************************************//
                            // This returns the data from SP commands (only) in a GenericClass Structured format
                            $"{SqlCommand}" . DapperTrace ( );
                            var reslt = db . Query ( SqlCommand , Params , commandType: CommandType . StoredProcedure );
                            //***************************************************************************************************************//

                            if ( reslt != null )
                            {
                                //Although this is duplicated  with the one above we CANNOT make it a method()
                                int dictcount = 0;
                                dict . Clear ( );
                                long zero = reslt . LongCount ( );
                                try
                                {
                                    foreach ( var item in reslt )
                                    {
                                        try
                                        {
                                            //	Create a dictionary for each row of data then add it to a GenericClass row then add row to Generics Db
                                            DatagridControl . ParseDapperRow ( item , dict , out colcount );
                                            dictcount = 1;
                                            fldcount = dict . Count;
                                            if ( fldcount == 0 )
                                            {
                                                //no problem, we will get a collection anyway
                                                return null;
                                            }
                                            string buffer = "", tmp = "";
                                            int index = 0;
                                            foreach ( var pair in dict )
                                            {
                                                try
                                                {
                                                    if ( pair . Key != null && pair . Value != null )
                                                    {
                                                        tmp = pair . Key . ToString ( ) + $"= Field{index++}";
                                                        buffer += tmp + ",";
                                                    }
                                                }
                                                catch ( Exception ex )
                                                {
                                                    $"Dictionary ERROR : {ex . Message}" . cwerror ( );
                                                    result = ex . Message;
                                                }
                                            }
                                            IsSuccess = true;
                                            string s = buffer . Substring ( 0 , buffer . Length - 1 );
                                            $"buffer = {s}" . cwerror ( );
                                            buffer = s;
                                        }
                                        catch ( Exception ex )
                                        {
                                            $"SQLERROR : {ex . Message}" . cwerror ( );
                                            $"Exiting with null" . cwwarn ( );
                                            return null;
                                        }
                                        collection . Add ( gc );
                                        dict . Clear ( );
                                        dictcount = 1;
                                    }
                                }
                                catch ( Exception ex )
                                {
                                    Debug . WriteLine ( $"OUTER DICT/PROCEDURE ERROR : {ex . Message}" );
                                    if ( ex . Message . Contains ( "not find stored procedure" ) )
                                    {
                                        result = $"SQL PARSE ERROR - [{ex . Message}]";
                                        errormsg = $"{result}";
                                        $"Exiting with null" . cwwarn ( );
                                        return null;
                                    }
                                    else
                                    {
                                        long x = reslt . LongCount ( );
                                        if ( x == ( long ) 0 )
                                        {
                                            result = $"ERROR : [{SqlCommand}] returned ZERO records... ";
                                            errormsg = $"DYNAMIC:0";
                                            $"Exiting with null" . cwwarn ( );
                                            return null;
                                        }
                                        else
                                        {
                                            result = ex . Message;
                                            errormsg = $"UNKNOWN :{ex . Message}";
                                        }
                                        $"Exiting with null" . cwwarn ( );
                                        return null;
                                    }
                                }
                            }
                            if ( IsSuccess == false )
                            {
                                errormsg = $"Dapper request returned zero results, maybe one or more arguments are required, or the Procedure does not return any values ?";
                                Debug . WriteLine ( errormsg );
                            }
                            else
                            {
                                $"Exiting with null" . cwwarn ( );
                            }
                        }
                    }
                    catch ( Exception ex )
                    {
                        Debug . WriteLine ( $"STORED PROCEDURE ERROR : {ex . Message}" );
                        $"STORED PROCEDURE ERROR : {ex . Message}" . cwerror ( );
                        result = ex . Message;
                        errormsg = $"SQLERROR : {result}";
                    }
                }
                catch ( Exception ex )
                {
                    $"Sql Error, {ex . Message}, {ex . Data}" . cwerror ( );
                    result = ex . Message;
                    $"STORED PROCEDURE ERROR : {ex . Message}" . cwerror ( );
                }
            } // end using {} - MUST get here  to close connection correctly
            $"Exiting with null" . cwwarn ( );
            return null;
        }

        private void LoadAllDatabases ( object sender , RoutedEventArgs e )
        {
            SelectDbWin dbwin = new SelectDbWin ( CurrentTableDomain );
            dbwin . Show ( );
        }

        //*************************//
        #region resizing (all types)
        //*************************//
        private void Gengrid_SizeChanged ( object sender , SizeChangedEventArgs e )
        {
            double dbl = row2 . ActualHeight;
            //if ( dbl >= 10 )
            //    GenGridCtrl . Height = dbl;     // WORKING
            //else
            GenGridCtrl . Height = dbl;    // WORKING
            try
            {
                dbl = majorgrid . ActualHeight - 20;
                InfoGrid . Visibility = Visibility . Visible;
                double row0height = majorgrid . RowDefinitions [ 0 ] . ActualHeight;
                double row2height = majorgrid . RowDefinitions [ 2 ] . ActualHeight;

                GenGridCtrl . Height = row0height;// - 10;    // WORKING

                double viewerHeight = row2height;//>= 10 ? row2height - 10 : row2height;
                InfoGrid . Height = viewerHeight - 3; // -27;
                InfoBorder . Height = InfoGrid . Height - 10;
                ViewerGrid . Height = InfoBorder . Height - 10;
                RTBox . Height = ViewerGrid . Height - 45;

                // Width - working
                InfoGrid . Width = GenGridCtrl . ActualWidth;// +10;
                InfoBorder . Width = InfoGrid . Width - 10;
                // All ok so far
                InfoGrid . Refresh ( );
                ViewerGrid . Width = InfoBorder . Width;
                RTBox . Width = ( ViewerGrid . Width - Splist . ActualWidth - Vsplitter . ActualWidth ) - 10;
            }
            catch ( Exception ex ) { }
        }
        //*************************//
        #endregion

        #region Splitter Cursor handling
        //*************************// 
        private void GridSplitter_MouseEnter ( object sender , MouseEventArgs e )
        {
            if ( this . Cursor != Cursors . Wait )
                Mouse . OverrideCursor = Cursors . SizeWE;
            mbs = e . LeftButton;
            if ( mbs == MouseButtonState . Pressed )
            {
                Splitterlastpos = 0;
                Point pt = e . GetPosition ( ViewerGrid );
                RTwidth -= pt . X - Splitterlastpos;
            }
        }
        private void hGridSplitter_MouseEnter ( object sender , MouseEventArgs e )
        {
            Mouse . OverrideCursor = Cursors . SizeNS;
            if ( this . Cursor != Cursors . Wait )
                mbs = e . LeftButton;
            if ( mbs == MouseButtonState . Pressed )
            {
                hSplitterlastpos = 0;
                Point pt = e . GetPosition ( maingrid );
                RTHeight -= pt . Y - hSplitterlastpos;
                //Debug . WriteLine ( $"Entering => {pt . Y}, RTHeight ={RTHeight}" );
            }
        }

        private void GridSplitter_MouseLeave ( object sender , MouseEventArgs e )
        {
            if ( this . Cursor != Cursors . Wait )
                Mouse . OverrideCursor = Cursors . Arrow;
            mbs = e . LeftButton;
            if ( mbs == MouseButtonState . Pressed )
            {
                Point pt = e . GetPosition ( ViewerGrid );
                hSplitterlastpos = hSplitterbottompos;
                //Debug . WriteLine ( $"Exiting => {pt . Y}, RTHeight ={RTHeight}" );
            }
        }
        private void hGridSplitter_MouseLeave ( object sender , MouseEventArgs e )
        {
            //            if ( this . Cursor != Cursors . Wait )
            Mouse . OverrideCursor = Cursors . Arrow;
            mbs = e . LeftButton;
            if ( mbs == MouseButtonState . Pressed )
            {
                Point pt = e . GetPosition ( maingrid );
                hSplitterlastpos = hSplitterbottompos;
            }
        }
        private void Vsplitter_MouseMove ( object sender , MouseEventArgs e )
        {
            // get X position
            mbs = e . LeftButton;
            if ( mbs == MouseButtonState . Pressed )
            {
                try
                {
                    Point pt = e . GetPosition ( ViewerGrid );
                    Splitterleftpos = pt . X;
                    if ( Splitterleftpos > 300 )
                    {
                        // ViewerGrid . ColumnDefinitions [ 0 ] . Width = new GridLength ( 290 , GridUnitType . Pixel );
                        //Splitterlastpos = 290;
                        //RTwidth = Splitterleftpos ;
                        //ViewerGrid . ColumnDefinitions [ 2 ] . Width = new GridLength ( ViewerGrid.ActualWidth -310 , GridUnitType . Pixel );
                        RTBox . UpdateLayout ( );
                        e . Handled = true;
                        return;
                    }
                    RTwidth = Splitterleftpos;
                    //Debug . WriteLine ( $"{Splitterleftpos}, => {RTwidth}" );
                    InfoGrid . Width = ViewerGrid . ColumnDefinitions [ 2 ] . ActualWidth;
                    InfoGrid . Width = maingrid . ActualWidth - 5;
                    //                    InfoGrid . UpdateLayout ( );
                    InfoBorder . Width = InfoGrid . Width;// - Splitterleftpos;
                                                          //                    InfoBorder . UpdateLayout ( );

                    ViewerGrid . Width = InfoGrid . Width - 20;
                    ViewerGrid . UpdateLayout ( );
                    RTBox . Width = InfoGrid . Width - ( tempgrid . ActualWidth + 35 );
                    RTBox . UpdateLayout ( );
                    //RTBox. Width = ViewerGrid . Width;
                    Splitterlastpos = Splitterleftpos;
                    //ViewerGrid . ColumnDefinitions [ 0 ] . Width = new GridLength ( 1 , GridUnitType . Pixel );
                }
                catch ( Exception ex )
                {
                    Debug . WriteLine ( ex . Message );
                }
            }
            #endregion Splitter Cursor handling
        }

        private void Hsplitter_MouseMove ( object sender , MouseEventArgs e )
        {
            // get Y position
            mbs = e . LeftButton;
            double mainheight = majorgrid . ActualHeight;
            double gridheight = dgrow . ActualHeight + vwrow . ActualHeight + 10;
            double infoheight = vwrow . ActualHeight;
            double splitpos = 0;
            double newpos = 0;
            double row0height = majorgrid . RowDefinitions [ 0 ] . ActualHeight;
            double row2height = majorgrid . RowDefinitions [ 2 ] . ActualHeight;

            if ( e . LeftButton == MouseButtonState . Pressed )
            {
                try
                {
                    Point pt = e . GetPosition ( maingrid );
                    newpos = pt . Y;
                    GenGridCtrl . Height = row0height - 10;    // WORKING

                    double viewerHeight = row2height;//>= 10 ? row2height - 10 : row2height;
                    InfoGrid . Height = viewerHeight - 3; // -27;
                    InfoBorder . Height = InfoGrid . Height - 10;
                    ViewerGrid . Height = InfoBorder . Height - 10;
                    RTBox . Height = ViewerGrid . Height - 45;
                    //    DoubleValue viewerHeight = row2height;//>= 10 ? row2height - 10 : row2height;
                    //  InfoGrid . Height = viewerHeight+40 ;
                    //  InfoBorder . Height = InfoGrid . Height -50;
                    //  RTBox . Height = InfoGrid . Height - 120;
                    ////  Debug . WriteLine ( $"{row0height} - {row2height}, Gengrid={GenGridCtrl . Height}, Infogrid = {InfoGrid . Height}, canvas={Filtercanvas . Height}" );
                }
                catch ( Exception ex )
                {
                    //GenGridCtrl . Height = row0height;    // WORKING
                    //InfoGrid . Height = row2height;
                    //InfoBorder . Height = row2height;
                    //RTBox . Height = row2height;
                    Debug . WriteLine ( $"Drag error => {ex . Message}" );
                    e . Handled = true;
                }
            }
            else
                e . Handled = true;
        }

        //-------------------------//
        #endregion Splitter cursor handlers
        //-------------------------//

        public void StdError ( )
        {
            DapperGenericsLib . Utils . DoErrorBeep ( 400 , 100 , 1 );
            DapperGenericsLib . Utils . DoErrorBeep ( 300 , 400 , 1 );
        }
        public void StdSuccess ( )
        {
            DapperGenericsLib . Utils . DoErrorBeep ( 400 , 100 , 1 );
            DapperGenericsLib . Utils . DoErrorBeep ( 350 , 200 , 2 );
            DapperGenericsLib . Utils . DoErrorBeep ( 470 , 150 , 1 );
            DapperGenericsLib . Utils . DoErrorBeep ( 410 , 200 , 1 );
        }
        public static void ReplaceDataGridFldNames ( ObservableCollection<GenericClass> datagrid , ref DataGrid Grid1 , ref List<DapperGenericsLib . DataGridLayout> dglayoutlist , int colcount )
        {
            List<string> list = new List<string> ( );
            ObservableCollection<GenericClass> GenClass = new ObservableCollection<GenericClass> ( );
            Dictionary<string , string> dict = new Dictionary<string , string> ( );
            List<Dictionary<string , string>> ColumnTypesList = new List<Dictionary<string , string>> ( );
            // pass down dictionary that will return with column names and SQL types
            Dictionary<string , string> Columntypes = new Dictionary<string , string> ( );

            "" . Track ( );
            int index = 0;
            // Add data  for field size
            if ( datagrid . Count > 0 )
            {
                if ( dglayoutlist . Count > 0 )
                {
                    index = 0;
                    // use the list to get the correct column header info
                    // and replace the column headers in our grid
                    foreach ( var item in Grid1 . Columns )
                    {
                        //                      DataGridColumn dgc = item;
                        try
                        {
                            item . Header = "";
                            item . Header = dglayoutlist [ index ] . Fieldname;
                            // Update  the datagrid's column header here...
                            //item . Header = dgc . Header;
                            Grid1 . Columns [ index ] . Header = item . Header;
                            if ( index++ >= dglayoutlist . Count )
                            {
                                break;
                            }
                        }
                        catch ( ArgumentOutOfRangeException ex ) { Debug . WriteLine ( $"TODO - BAD Columns - 300 GenericDbHandlers.cs" ); }
                    }
                }
                // Grid now has valid column names, but still got All 20 ??
                //Grid1 . UpdateLayout ( );
            }
            "" . Track ( 1 );
            // TODO not filled correctly
            return;
        }
        private void RTBox_PreviewKeyDown ( object sender , KeyEventArgs e )
        {
            Type type = sender . GetType ( );
            FlowDocumentScrollViewer fdv = sender as FlowDocumentScrollViewer;
            Paragraph p = RTBox . Selection . Start . Paragraph;
            double fsize = fdv . FontSize;

            if ( e . Key == Key . Add )
            {
                para1 . FontSize += 1;
            }
            else if ( e . Key == Key . Subtract )
            {
                para1 . FontSize -= 1;
            }
            else if ( e . Key == Key . Down )
            {
                FlowDocumentScrollViewer fdsv = sender as FlowDocumentScrollViewer;
                ScrollViewer sv = fdsv . Template . FindName ( "PART_ContentHost" , fdsv ) as ScrollViewer;
                FlowdocVerticalpos += 13;
                if ( FlowdocVerticalpos > sv . ScrollableHeight )
                    FlowdocVerticalpos = Convert . ToInt32 ( sv . ScrollableHeight );
                sv . ScrollToVerticalOffset ( FlowdocVerticalpos );
                Debug . WriteLine ( FlowdocVerticalpos );
            }
            else if ( e . Key == Key . Up )
            {
                FlowDocumentScrollViewer fdsv = sender as FlowDocumentScrollViewer;
                ScrollViewer sv = fdsv . Template . FindName ( "PART_ContentHost" , fdsv ) as ScrollViewer;
                if ( FlowdocVerticalpos > 13 )
                    FlowdocVerticalpos -= 13;
                else if ( FlowdocVerticalpos <= 13 )
                    FlowdocVerticalpos -= 0;
                sv . ScrollToVerticalOffset ( FlowdocVerticalpos );
            }
            else if ( e . Key == Key . Home )
            {
                FlowDocumentScrollViewer fdsv = sender as FlowDocumentScrollViewer;
                ScrollViewer sv = fdsv . Template . FindName ( "PART_ContentHost" , fdsv ) as ScrollViewer;
                sv . ScrollToHome ( );
                FlowdocVerticalpos = 0;
            }
            else if ( e . Key == Key . End )
            {
                FlowDocumentScrollViewer fdsv = sender as FlowDocumentScrollViewer;
                ScrollViewer sv = fdsv . Template . FindName ( "PART_ContentHost" , fdsv ) as ScrollViewer;
                sv . ScrollToEnd ( );
                double dbl = sv . ActualHeight;
                //FlowdocVerticalpos =Convert.ToInt32( sv . ViewportHeight);
                //FlowdocVerticalpos = Convert . ToInt32 ( sv . ScrollToEnd));
                //sv . ScrollToVerticalOffset ( FlowdocVerticalpos );
            }
            else if ( e . Key == Key . PageUp )
            {
                FlowDocumentScrollViewer fdsv = sender as FlowDocumentScrollViewer;
                ScrollViewer sv = fdsv . Template . FindName ( "PART_ContentHost" , fdsv ) as ScrollViewer;
                FlowdocVerticalpos -= Convert . ToInt32 ( sv . ViewportHeight );
                if ( FlowdocVerticalpos < 0 )
                    FlowdocVerticalpos = 0;
                //else
                //{
                //        FlowdocVerticalpos = Convert . ToInt32 ( sv . ViewportHeight );
                //}
                sv . ScrollToVerticalOffset ( FlowdocVerticalpos );
            }
            else if ( e . Key == Key . PageDown )
            {
                FlowDocumentScrollViewer fdsv = sender as FlowDocumentScrollViewer;
                ScrollViewer sv = fdsv . Template . FindName ( "PART_ContentHost" , fdsv ) as ScrollViewer;
                if ( FlowdocVerticalpos >= Convert . ToInt32 ( sv . ExtentHeight ) )
                    FlowdocVerticalpos += Convert . ToInt32 ( sv . ViewportHeight );
                else
                    FlowdocVerticalpos += Convert . ToInt32 ( sv . ViewportHeight );
                sv . ScrollToVerticalOffset ( FlowdocVerticalpos );
            }
            fdv . UpdateLayout ( );
        }
        private void RTBox_Scroll ( object sender , ScrollEventArgs e )
        {
            Type type = sender . GetType ( );
            FlowDocumentScrollViewer sb = sender as FlowDocumentScrollViewer;
            //var height = sb .m;

        }

        /// <summary>
        /// Collapses 1 or more Context menu entries and returns a ContextMenu pointer
        /// </summary>
        /// <param name="menuname"></param>
        /// <param name="singleton"></param>
        /// <param name="delItems"></param>
        /// <returns></returns>
        public ContextMenu RemoveMenuItems ( string menuname , string singleton = "" , List<string> delItems = null )
        {
            // Collapse visibility on one or more context menu items
            int listcount = 0;
            if ( delItems != null )
                listcount = delItems . Count;
            MenuItem mi = new MenuItem ( );

            var menu = FindResource ( menuname ) as ContextMenu;
            List<MenuItem> items = new List<MenuItem> ( );

            if ( singleton != "" )
            {
                foreach ( var item in menu . Items )
                {
                    mi = item as MenuItem;
                    if ( mi . Name == singleton )
                    {
                        mi . Visibility = Visibility . Collapsed;
                        break;
                    }
                }
            }
            else
            {
                {
                    foreach ( var menuitem in delItems )
                    {
                        foreach ( var item in menu . Items )
                        {
                            mi = item as MenuItem;
                            if ( mi == null )
                                continue;
                            if ( mi . Name == menuitem )
                            {
                                mi . Visibility = Visibility . Collapsed;
                                //items . Add ( mi );
                            }
                        }
                    }
                }
            }
            return menu;
        }

        public ContextMenu AddMenuItem ( string menuname , string entry )
        {
            SolidColorBrush sbrush = null;
            var menu = FindResource ( menuname ) as ContextMenu;
            MenuItem mi = new MenuItem ( );
            foreach ( MenuItem item in menu . Items )
            {
                if ( item . Name == entry )
                {
                    //PopupMenu.cm15
                    mi = item;
                    if ( Splist . SelectedItem != null && Splist . SelectedItem . ToString ( ) != "" )
                        SPExecuteText = $"Show the S.P Execute Window with [ {Splist . SelectedItem . ToString ( )} ]";
                    else
                    {
                        Splist . SelectedIndex = 0;
                        SPExecuteText = $"Show the S.P Execute Window with [ {Splist . SelectedItem . ToString ( )} ]";
                    }
                    mi . Header = SPExecuteText;
                    mi . Height = 25;
                    mi . Tag = Splist . SelectedItem . ToString ( );
                    break;
                }
            }
            return menu;
        }

        /// <summary>
        /// Resets Visibility of 1 or more Context menu entries and returns a ContextMenu pointer
        /// </summary>
        /// <param name="menuname"></param>
        /// <param name="singleton"></param>
        /// <param name="delItems"></param>
        /// <returns></returns>
        public ContextMenu ResetMenuItems ( string menuname , string singleton = "" , List<string> delItems = null )
        {
            int listcount = 0;
            // reset visibility on one or more previously collapsed context menu items
            if ( delItems != null )
                listcount = delItems . Count;

            var menu = FindResource ( menuname ) as ContextMenu;

            if ( singleton != "" )
            {
                foreach ( MenuItem item in menu . Items )
                {
                    if ( item . Name == singleton )
                    {
                        item . Visibility = Visibility . Visible;
                        break;
                    }
                }
            }
            else
            {
                foreach ( var delitem in delItems )
                {
                    foreach ( MenuItem menuitem in menu . Items )
                    {
                        //var v = mi . Items;
                        if ( menuitem . Name == delitem )
                            menuitem . Visibility = Visibility . Visible;
                    }
                }
            }
            return menu;
        }


        /// <summary>
        /// Hide various Context menu items from Infomation/SP viewer panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RTBox_PreviewMouseRightButtonDown ( object sender , MouseButtonEventArgs e )
        {
            List<string> hideitems = new List<string> ( );
            hideitems . Add ( "cm2" );
            hideitems . Add ( "cm6" );
            hideitems . Add ( "cm7" );
            hideitems . Add ( "cm8" );
            //hideitems . Add ( "cm11" );
            if ( CurrentSpList == "ALL" )
                hideitems . Add ( "cm4" );
            else if ( CurrentSpList == "MATCH" )
                hideitems . Add ( "cm5" );

            ContextMenu menu = RemoveMenuItems ( "PopupMenu" , "" , hideitems );
            "RTBox_PreviewMouseRightButtonDown " . cwinfo ( );
            menu = AddMenuItem ( "PopupMenu" , "cm15" );

            menu . IsOpen = true;
            //e . Handled = true;
        }

        private void LoadTechInfo ( object sender , RoutedEventArgs e )
        {
            LoadRTbox ( );
            RTBox . Refresh ( );
        }

        private void ContextMenu_Closed ( object sender , RoutedEventArgs e )
        {
            ContextMenu cm = FindResource ( "GenGridContextMenu" ) as ContextMenu;
            List<string> hideitems = new List<string> ( );
            hideitems . Add ( "gm1" );
            hideitems . Add ( "gm2" );
            hideitems . Add ( "gm3" );
            ContextMenu menu = ResetMenuItems ( "GenGridContextMenu" , "" , hideitems );
            menu . IsOpen = false;
            e . Handled = true;
        }
        private void SpStringsSelection_KeyDown ( object sender , KeyEventArgs e )
        {
            if ( e . Key == Key . Enter )
                GoBtn1_Click ( null , null );
            else if ( e . Key == Key . Escape )
                SpStringsSelection . Visibility = Visibility . Collapsed;
        }

        public bool SaveDataToNewTable ( ObservableCollection<GenericClass> GridData ,
            string NewDbName ,
            string [ ] SqlArgs ,
            string SqlCommandstring ,
            int [ ] columnoffsets ,
            out string err )
        {
            string temp = "";
            bool retval = true;
            err = "";
            int gresult = -1;
            SqlConnection sqlCon = null;
            string SqlInsertCommand = "";
            // create array of field  types for use when creating values clause
            string [ ] dataTypes = new string [ columnoffsets . Length ];
            for ( int x = 0 ; x < columnoffsets . Length ; x++ )
            {
                dataTypes [ x ] = dglayoutlist [ columnoffsets [ x ] ] . Fieldtype . ToUpper ( );
                if ( x > columnoffsets . Length )
                    break;
            }
            // Create the new table in current Db
            string ConString = GenericDbUtilities . CheckSetSqlDomain ( "" );
            if ( ConString == "" )
            {
                GenericDbUtilities . CheckDbDomain ( "" );
                ConString = MainWindow . CurrentSqlTableDomain;
            }
            Mouse . OverrideCursor = Cursors . Wait;

            //Copy data to new table
            try
            {
                using ( sqlCon = new SqlConnection ( ConString ) )
                {
                    List<string> datavalues = new List<string> ( );
                    //                   int rangecount = TableStructure . Count;
                    int datastartvalue = 0, y = 0, x = 0, itemscount = 0;
                    string newdataitem = "";
                    string [ ] parts;

                    //sqlCon . Open ( );

                    foreach ( GenericClass item in GridData )
                    {
                        itemscount = 0;

                        // Get current command string from parameters
                        SqlInsertCommand = SqlCommandstring;

                        for ( int q = 0 ; q < columnoffsets . Length ; q++ )
                        {
                            string [ ] datecheck, datacheck2;
                            string datastring = AddDataToList ( item , columnoffsets [ q ] );
                            datecheck = datastring . Split ( ' ' );
                            if ( datecheck . Length >= 2 )
                            {
                                string date = datecheck [ 0 ];
                                datacheck2 = date . Split ( '/' );
                                if ( datacheck2 . Length >= 3 )
                                    datastring = ConvertToUsSqlDate ( date . Trim ( ) );
                            }
                            SqlInsertCommand += $"'{datastring}', ";
                        }
                        SqlInsertCommand = SqlInsertCommand . Substring ( 0 , SqlInsertCommand . Length - 2 );
                        SqlInsertCommand += ") ";
                        itemscount = 0;
                        // Now add record  to SQL table
                        var parameters = new DynamicParameters ( );
                        parameters . Add ( $"Arg1" , $"{SqlInsertCommand}" ,
                            DbType . String ,
                            ParameterDirection . Input ,
                            SqlInsertCommand . Length );
                        string cmd = $"spExecuteStoredProcedureCommand";
                        Debug . WriteLine ( $"Processing SQL command\n{SqlInsertCommand}" );
                        // save data (Insert)
                        //*********************************************************************************************************************************//
                        gresult = sqlCon . Execute ( cmd , parameters , commandType: CommandType . StoredProcedure );
                        //*********************************************************************************************************************************//
                        Debug . WriteLine ( "Insert succeeded...." );
                        gresult = 1;
                    }   //foreach
                }   // using
            }   // try
            catch ( Exception ex )
            {
                Debug . WriteLine ( $"Error {ex . Message}, {ex . Data}" );
                $" {SqlInsertCommand}" . cwerror ( );
                StdError ( );
                ////sqlCon . close ( );
                gresult = -3;
                retval = false;
            }
            finally
            {
                //sqlCon . close ( );
            }
            return retval;
        }
        public static string AddDataToList ( GenericClass item , int index )
        {
            string output = "";
            index++;
            switch ( index )
            {
                case 1: output = item . field1; break;
                case 2: output = item . field2; break;
                case 3: output = item . field3; break;
                case 4: output = item . field4; break;
                case 5: output = item . field5; break;
                case 6: output = item . field6; break;
                case 7: output = item . field7; break;
                case 8: output = item . field8; break;
                case 9: output = item . field9; break;
                case 10: output = item . field10; break;
                case 11: output = item . field11; break;
                case 12: output = item . field12; break;
                case 13: output = item . field13; break;
                case 14: output = item . field14; break;
                case 15: output = item . field15; break;
                case 16: output = item . field16; break;
                case 17: output = item . field17; break;
                case 18: output = item . field18; break;
                case 19: output = item . field19; break;
                case 20: output = item . field20; break;
            }
            return output;
        }
        public static string ConvertToUsSqlDate ( string dateToConvert )
        {
            string output = "";
            string [ ] items = dateToConvert . Split ( '/' );
            output = $"{items [ 2 ]}/{items [ 1 ]}/{items [ 0 ]}";
            return output;
        }


        #region ICOMMANDS
        private void ExecuteFilterStoredprocs ( object obj )
        {
            SpStringsSelection . Visibility = Visibility . Collapsed;
            GoBtn1_Click ( null , null );
        }
        private void ExecuteCloseFilterStoredprocs ( object obj )
        { SpStringsSelection . Visibility = Visibility . Collapsed; }

        private bool CanExecuteFilterStoredprocs ( object arg )
        { return true; }
        private bool CanExecuteCloseFilterStoredprocs ( object arg )
        { return true; }
        #endregion ICOMMANDS

        public static Grid FindOuterParent ( object sender , RoutedEventArgs e , string target = "" )
        {
            UIElement returnval = null;
            Grid dgobj = null;
            Border dgobj2 = null;
            string currgrid = "", lastgrid = "";
            int gridcount = 0;
            UIElement fe = sender as UIElement;
            Type type = sender . GetType ( );

            if ( target != "" && type == typeof ( Grid ) )
            {
                dgobj = sender as Grid;
                if ( dgobj . Name == target )
                    return dgobj;
            }
            dgobj = DapperGenericsLib . Utils . FindVisualParent<Grid> ( e . OriginalSource as DependencyObject );

            if ( dgobj != null )
            {
                if ( target != "" )
                {
                    if ( dgobj . Name == target )
                        return dgobj;
                }
                while ( true )
                {
                    if ( dgobj . GetType ( ) == typeof ( Grid ) && currgrid == "" )
                    {
                        currgrid = dgobj . Name;
                        return dgobj;
                    }
                    if ( dgobj == null )
                        dgobj = DapperGenericsLib . Utils . FindVisualParent<Grid> ( dgobj2 as DependencyObject );
                    //else 
                    //    dgobj = DapperGenericsLib . Utils . FindVisualParent<Grid> ( e . OriginalSource as DependencyObject );

                    if ( dgobj == null ) break;
                    if ( target != "" && dgobj . Name == target )
                    {
                        dgobj . Visibility = Visibility . Collapsed;
                        break;
                    }
                    else lastgrid = dgobj . Name;

                    if ( currgrid == lastgrid )
                    {
                        dgobj2 = DapperGenericsLib . Utils . FindVisualParent<Border> ( dgobj as DependencyObject );
                    }
                }   // End While (true)

            }
            return dgobj;
        }

        private void stopBtn1_Click ( object sender , RoutedEventArgs e )
        {
            Grid dgobj = null;
            Border dgobj2 = null;
            string currgrid = "", lastgrid = "";
            int gridcount = 0;
            UIElement fe = sender as UIElement;
            Type type = sender . GetType ( );
            if ( type == typeof ( Button ) )
            {
                dgobj = FindOuterParent ( sender , e );
                if ( dgobj != null )
                    dgobj . Visibility = Visibility . Collapsed;
                {
                    //dgobj = DapperGenericsLib . Utils . FindVisualParent<Grid> ( e . OriginalSource as DependencyObject );

                    //if ( dgobj != null )
                    //{
                    //    while ( true )
                    //    {
                    //        if ( dgobj . GetType ( ) == typeof ( Grid ) && currgrid == "" )
                    //            currgrid = dgobj . Name;
                    //        if ( dgobj2 != null ) dgobj = DapperGenericsLib . Utils . FindVisualParent<Grid> ( dgobj2 as DependencyObject );
                    //        else dgobj = DapperGenericsLib . Utils . FindVisualParent<Grid> ( e . OriginalSource as DependencyObject );

                    //        if ( dgobj == null ) break;
                    //        if ( dgobj . Name == "Execsp" )
                    //        {
                    //            dgobj . Visibility = Visibility . Collapsed;
                    //            break;
                    //        }
                    //        else lastgrid = dgobj . Name;

                    //        if ( currgrid == lastgrid )
                    //        {
                    //            dgobj2 = DapperGenericsLib . Utils . FindVisualParent<Border> ( dgobj as DependencyObject );
                    //        }
                    //    }   // End While (true)
                    //    return;
                }
            }
            else
            {
                UIElement uie = e . OriginalSource as UIElement;
                uie . Visibility = Visibility . Collapsed;
                return;
            }
        }

        private void closeSearchDlg ( object sender , MouseButtonEventArgs e )
        {
            SpStringsSelection . Visibility = Visibility . Collapsed;
        }


        private void ExecDragDialog_LButtonDn ( object sender , MouseButtonEventArgs e )
        {
            Control activegrid = null;
            Grid grid = new Grid ( );
            FrameworkElement parent = null;
            FrameworkElement fwelement = new FrameworkElement ( );
            Type type = sender . GetType ( );
            Debug . WriteLine ( $"Type={type}" );
            // Finds the parent immediately above the Canvas,
            // which is what we are dragging
            if ( type == typeof ( Button ) )
                return;
            else if ( type == typeof ( TextBox ) )
                return;
            else if ( type == typeof ( Border ) )
            {
                parent = NewWpfDev . Utils . FindVisualParent<Grid> ( ( DependencyObject ) sender , out string [ ] objectarray );
                if ( parent != null )
                {
                    if ( parent . Name == "Execsp" )
                    {
                        DragCtrl . InitializeMovement ( ( FrameworkElement ) parent );
                        DragCtrl . MovementStart ( parent , e );
                        e . Handled = true;
                        return;
                    }
                    else
                    {
                        if ( type == typeof ( Grid ) )
                        {
                            grid = sender as Grid;
                            if ( grid . Name == "Execsp" )
                            {
                                DragCtrl . InitializeMovement ( ( FrameworkElement ) grid );
                                DragCtrl . MovementStart ( grid , e );
                                e . Handled = true;
                                return;
                            }
                        }
                    }
                }
                else
                {
                    parent = ( Grid ) FindOuterParent ( sender , e );
                    if ( parent . GetType ( ) == typeof ( Grid ) && parent . Name == "Execsp" )
                    {
                        DragCtrl . InitializeMovement ( parent );
                        DragCtrl . MovementStart ( parent , e );
                        e . Handled = true;
                        return;
                    }
                }
            }
            else if ( type == typeof ( Grid ) )
            {
                grid = sender as Grid;
                if ( grid . Name == "Execsp" )
                {
                    DragCtrl . InitializeMovement ( grid );
                    DragCtrl . MovementStart ( grid , e );
                    e . Handled = true;
                    return;
                }
                else
                {
                    parent = ( Grid ) FindOuterParent ( sender , e );
                    if ( parent . GetType ( ) == typeof ( Grid ) && parent . Name == "Execsp" )
                    {
                        DragCtrl . InitializeMovement ( parent );
                        DragCtrl . MovementStart ( parent , e );
                        e . Handled = true;
                        return;
                    }
                }
            }
            else
            {
                parent = ( Grid ) FindOuterParent ( sender , e );
                if ( parent . GetType ( ) == typeof ( Grid ) && parent . Name == "Execsp" )
                {
                    DragCtrl . InitializeMovement ( parent );
                    DragCtrl . MovementStart ( parent , e );
                    e . Handled = true;
                    return;
                }
            }
        }
        private void ExecDragDialog_Moving ( object sender , MouseEventArgs e )
        {
            if ( MovingObject != null && e . LeftButton == MouseButtonState . Pressed )
            {
                Type type = sender . GetType ( );
                if ( type == typeof ( Button ) )
                {
                    e . Handled = true;
                    return;
                }
                else if ( type == typeof ( TextBox ) )
                {
                    e . Handled = true;
                    return;
                }
                else
                {
                    DragCtrl . CtrlMoving ( sender , e );
                    e . Handled = false;
                }
            }
        }
        private void ExecDragDialog_Ending ( object sender , MouseButtonEventArgs e )
        {
            DragCtrl . MovementEnd ( sender , e );
            e . Handled = true;
        }

        private void Execsp_KeyDown ( object sender , KeyEventArgs e )
        {
            //Execsp . Visibility = Visibility . Collapsed;
            if ( e . Key == Key . Enter )
            {
                CurrentSpSelection = Splist . SelectedItem . ToString ( );
                ExecBtn . RaiseEvent ( new RoutedEventArgs ( System . Windows . Controls . Primitives . ButtonBase . ClickEvent ) );
                e . Handled = true;
            }
            else if ( e . Key == Key . Escape )
            {
                Execsp . Visibility = Visibility . Collapsed;
                e . Handled = true;
            }
            e . Handled = false;
        }

        private void selectSp_MouseLeave ( object sender , MouseEventArgs e )
        {
            if ( selectSp . Text == "" )
                selectSp . Text = "Enter Arguments here ...";
            selectSp . SelectionLength = 0;
        }
        private void selectSp_MouseEnter ( object sender , MouseEventArgs e )
        {
            if ( selectSp . Text == "Enter Arguments here ..." )
            {
                selectSp . SelectAll ( );
                selectSp . Focus ( );
            }
            selectSp . UpdateLayout ( );
        }

        private void selectSp_MouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
        {
            if ( selectSp . SelectionLength > 0 && selectSp . Text == "Enter Arguments here ..." )
            {
                // selectSp . SelectionLength = 0;
                //selectSp . Text = "Enter Arguments here ..";
                selectSp . SelectAll ( );
                selectSp . Focus ( );
                selectSp . SelectionStart = 0;
                selectSp . CaretIndex = 0;
                e . Handled = false;
            }
            else
                selectSp . Select ( 0 , 0 );
        }

        private void ContextMenu_Opened ( object sender , RoutedEventArgs e )
        {
        }
        /// <summary>
        /// Auto routine called when the specified context menu closes
        /// and resets all options to visible
        /// </summary>
        /// <param name="sender">Any Control</param>
        /// <param name="e"></param>
        private void InfoContextMenu_Closed ( object sender , RoutedEventArgs e )
        {
            List<string> hideitems = new List<string> ( );
            hideitems . Add ( "cm1" );
            hideitems . Add ( "cm2" );
            hideitems . Add ( "cm3" );
            hideitems . Add ( "cm4" );
            hideitems . Add ( "cm5" );
            hideitems . Add ( "cm6" );
            hideitems . Add ( "cm7" );
            hideitems . Add ( "cm8" );
            hideitems . Add ( "cm9" );
            hideitems . Add ( "cm10" );
            hideitems . Add ( "cm11" );
            e . Handled = true;
            ContextMenu menu = ResetMenuItems ( "PopupMenu" , "" , hideitems );
            "InfoContextMenu_Closed " . cwinfo ( );
            menu . IsOpen = false;
        }
        public List<string> ConvertTableNames ( List<string> TablesList )
        {
            List<string> newNamesList = new List<string> ( );
            return newNamesList = ValidTableNames . ConvertTableName ( TablesList );
        }
        public static void GetValidDomain ( )
        {
            if ( Genericgrid . CurrentTableDomain == "AdventureWorks2019" )
                DBprefix = $"{CurrentTableDomain . ToUpper ( )}.";
            else
                DBprefix = $"{CurrentTableDomain . ToUpper ( )}.DBO.";
        }

        private void Splist_PreviewMouseDoubleClick ( object sender , MouseButtonEventArgs e )
        {
            // user double clicked in Splist to load Results viewer
            ExecuteSP_Click ( sender , null );
        }


        #region TREEVIEW code

        //private void LoadSpView ( object sender , RoutedEventArgs e )
        //{
        //    // Load Stored procedures Tree viewer

        //    if ( TreeviewBorder . Visibility == Visibility . Visible )
        //    {
        //        TreeviewBorder . Visibility = Visibility . Hidden;
        //        if ( DbTablesTree . Visibility == Visibility . Visible )
        //        {
        //            LoadTreeData ( );
        //            SpLabel . Visibility = Visibility . Visible;
        //            DbProcsTree . Visibility = Visibility . Visible;
        //            SpWrappanel . Visibility = Visibility . Visible;
        //            TablesWrappanel . Visibility = Visibility . Hidden;
        //            TablesLabel . Visibility = Visibility . Hidden;
        //            DbTablesTree . Visibility = Visibility . Hidden;
        //            TreeviewBorder . Visibility = Visibility . Visible;
        //        }
        //    }
        //    else
        //    {
        //        LoadTreeData ( );
        //        SpLabel . Visibility = Visibility . Visible;
        //        DbProcsTree . Visibility = Visibility . Visible;
        //        SpWrappanel . Visibility = Visibility . Visible;
        //        TablesWrappanel . Visibility = Visibility . Hidden;
        //        TablesLabel . Visibility = Visibility . Hidden;
        //        DbTablesTree . Visibility = Visibility . Hidden;
        //        TreeviewBorder . Visibility = Visibility . Visible;
        //    }
        //}
        //private void LoadTreeData ( )
        //{
        //    //SqlDatabases sqldb = new SqlDatabases();
        //    DatabasesCollection . Clear ( );
        //    DbTablesTree . ItemsSource = null;
        //    DbProcsTree . ItemsSource = null;
        //    DbTablesTree . Items . Clear ( );
        //    DbProcsTree . Items . Clear ( );
        //    TvSqlCommand  = "spGetAllDatabaseNames";
        //    List<string> dblist = new List<string> ( );
        //    Datagrids . CallStoredProcedure ( dblist , TvSqlCommand  );
        //    //This call returns us a DataTable
        //    DataTable dt = DapperGenericsLib . DataLoadControl . GetDataTable ( TvSqlCommand  );
        //    // This how to access  Row data from  a grid the easiest way.... parsed into a List <xxxxx>
        //    dblist = WpfLib1 . Utils . GetDataDridRowsAsListOfStrings ( dt );

        //    var collection = DatabasesCollection;

        //    foreach ( string row in dblist )
        //    {
        //        //List<SqlTable> sqltable = new List<SqlTable>();
        //        // Now Handle list of tablenames
        //        if ( NewWpfDev . Utils . CheckResetDbConnection ( row , out string constr ) == false )
        //        {
        //            Debug . WriteLine ( $"Failed to set connection string for {row . ToUpper ( )} Db" );
        //            continue;
        //        }
        //        // All Db's have their own version of this SP.....
        //        TvSqlCommand  = "spGetTablesList";

        //        List<string> tableslist = new List<string> ( );
        //        Datagrids . CallStoredProcedure ( tableslist , TvSqlCommand  );
        //        //This call returns us a DataTable
        //        dt = DapperGenericsLib.DataLoadControl . GetDataTable ( TvSqlCommand  );

        //        Database db = new Database ( );
        //        // This how to access Row data from  a grid the easiest way.... parsed into a List <xxxxx>
        //        if ( dt != null )
        //        {
        //            db . Tables = new List<SqlTable> ( );
        //            tableslist = WpfLib1 . Utils . GetDataDridRowsAsListOfStrings ( dt );
        //            foreach ( string item in tableslist )
        //            {
        //                SqlTable sqlt = new SqlTable ( item );
        //                sqlt . Tablename = item;
        //                db . Tables . Add ( sqlt );
        //                db . Databasename = row;
        //            }
        //            DatabasesCollection . Add ( db );
        //        }

        //        // All Db's have their own version of this SP.....
        //        TvSqlCommand  = "spGetStoredProcs";

        //        List<string> procslist = new List<string> ( );
        //        Datagrids . CallStoredProcedure ( procslist , TvSqlCommand  );
        //        //This call returns us a DataTable
        //        dt = DapperGenericsLib.DataLoadControl . GetDataTable ( TvSqlCommand  );
        //        // This how to access Row data from  a grid the easiest way.... parsed into a List <xxxxx>
        //        if ( dt != null )
        //        {
        //            //Database db = new Database();
        //            SqlProcedures sp = new SqlProcedures ( ); 
        //            sp. Procedures = new List<SqlProcedures> ( );
        //            procslist = WpfLib1 . Utils . GetDataDridRowsAsListOfStrings ( dt );
        //            foreach ( string item in procslist )
        //            {
        //                SqlProcedures sqlprocs = new SqlProcedures ( item );
        //                sqlprocs . Procname = item;
        //                sp . Procedures . Add ( sqlprocs );
        //            }
        //            // Duplicates all entries !!!
        //            //DatabasesCollection . Add ( db );
        //        }

        //    }
        //    DbTablesTree . ItemsSource = DatabasesCollection;
        //    DbProcsTree . ItemsSource = DatabasesCollection;

        //}

        //private void TreeviewBorder_LostFocus ( object sender , RoutedEventArgs e )
        //{
        //    ReleaseMouseCapture ( );
        //    //Debug. WriteLine ( "Mouse released 5" );
        //    TvMouseCaptured = false;
        //}

        //private void TreeviewBorder_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
        //{
        //    /// treeviewer item selected with mouse
        //    if ( e . LeftButton == MouseButtonState . Pressed )
        //    {
        //        Label tv = sender as Label;
        //        if ( tv == null )
        //        {
        //            ReleaseMouseCapture ( );
        //            return;
        //        }
        //        TvFirstXPos = e . GetPosition ( tv ) . X;
        //        TvFirstYPos = e . GetPosition ( tv ) . Y;
        //        TvMouseCaptured = true;
        //    }
        //    else
        //    {
        //        ReleaseMouseCapture ( );
        //        TvMouseCaptured = false;
        //    }
        //}

        //private void TreeviewBorder_PreviewMouseLeftButtonUp ( object sender , MouseButtonEventArgs e )
        //{
        //    /// stop treeviewer Move 
        //    ReleaseMouseCapture ( );
        //    //Debug. WriteLine ( "Mouse released 4" );
        //    TvMouseCaptured = false;
        //}
        //private void TreeviewBorder_MouseMove ( object sender , MouseEventArgs e )
        //{
        //    if ( TvMouseCaptured )
        //    {
        //        //Label  tv = sender  as Label ;
        //        //if ( tv == null )
        //        //	return;
        //        double left = e . GetPosition ( ( TreeviewBorder as FrameworkElement ) . Parent as FrameworkElement ) . X - TvFirstXPos;
        //        double top = e . GetPosition ( ( TreeviewBorder as FrameworkElement ) . Parent as FrameworkElement ) . Y - TvFirstYPos;
        //        double trueleft = left - CpFirstXPos;
        //        double truetop = left - CpFirstYPos;
        //        if ( left >= 0 ) // && left <= canvas.ActualWidth - Flowdoc.ActualWidth)
        //            ( TreeviewBorder as FrameworkElement ) . SetValue ( Canvas . LeftProperty , left );
        //        if ( top >= 0 ) //&& top <= canvas . ActualHeight- Flowdoc. ActualHeight)
        //            ( TreeviewBorder as FrameworkElement ) . SetValue ( Canvas . TopProperty , top );
        //        ReleaseMouseCapture ( );
        //    }
        //    else
        //        ReleaseMouseCapture ( );
        //}
        //private void SpImage_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
        //{
        //}

        //private void Image_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
        //{
        //}

        //private void DbTablesTree_SelectedItemChanged ( object sender , RoutedPropertyChangedEventArgs<object> e )
        //{

        //}

        //        private void DbProcsTree_Collapsed ( object sender , RoutedEventArgs e )
        //        {
        //#pragma warning disable CS0219 // The variable 'x' is assigned but its value is never used
        //            int x = 0;
        //#pragma warning restore CS0219 // The variable 'x' is assigned but its value is never used
        //        }

        //        private void TextBlock_PreviewMouseRightButtonUp ( object sender , MouseButtonEventArgs e )
        //        {
        //            // right click for S.P script
        //            TextBlock tb = sender as TextBlock;
        //            string selection = tb . Text;
        //#pragma warning disable CS0219 // The variable 'index' is assigned but its value is never used
        //            int index = 0;
        //#pragma warning restore CS0219 // The variable 'index' is assigned but its value is never used
        //            foreach ( var item in ProcsCollection )
        //            {
        //                if ( item . Procname == selection )
        //                {
        //                    item . IsSelected = true;
        //                    break;
        //                }
        //            }
        //        }

        //        private void DbProcsTree_PreviewMouseRightButtonUp ( object sender , MouseButtonEventArgs e )
        //        {
        //            // process right click to show the  full script in a FlowDoc viewer 
        //            if ( SqlSpCommand != "" && SqlSpCommand != null )
        //            {
        //                DataTable dt = new DataTable ( );
        //                string [ ] args = { "" , "" , "" , "" };
        //#pragma warning disable CS0219 // The variable 'err' is assigned but its value is never used
        //                string err = "", errormsg = "";
        //#pragma warning restore CS0219 // The variable 'err' is assigned but its value is never used
        //                List<string> list = new List<string> ( );
        //                ObservableCollection<GenericClass> Generics = new ObservableCollection<GenericClass> ( );
        //                foreach ( var item in DatabasesCollection )
        //                {
        //                    CurrentSPDb = item . Databasename;
        //                    if ( NewWpfDev.Utils . CheckResetDbConnection ( CurrentSPDb , out string constring ) == false )
        //                        return;

        //                    List<string> procslist = new List<string> ( );
        //                    ObservableCollection<BankAccountViewModel> bvmparam = new ObservableCollection<BankAccountViewModel> ( );
        //                    List<string> genericlist = new List<string> ( );
        //#pragma warning disable CS0168 // The variable 'ex' is declared but never used
        //                    try
        //                    {
        //                        DapperSupport . CreateGenericCollection (
        //                            ref Generics ,
        //                            "spGetSpecificSchema  " ,
        //                            SqlSpCommand ,
        //                            "" ,
        //                            "" ,
        //                            ref genericlist ,
        //                            ref errormsg );
        //                        if ( Generics . Count > 0 )
        //                        {
        //                            break;
        //                        }
        //                    }
        //                    catch ( Exception ex )
        //                    {
        //                    }
        //#pragma warning restore CS0168 // The variable 'ex' is declared but never used

        //                }
        //                if ( Generics . Count == 0 )
        //                {
        //                    if ( errormsg != "" )
        //                        MessageBox . Show ( $"No Argument information is available. \nError message = [{errormsg}]" , $"[{SqlSpCommand}] SP Script Information" , MessageBoxButton . OK , MessageBoxImage . Warning );
        //                    return;
        //                }
        //                string output = "NB: You can select a different S.P & right click it WITHOUT closing this viewer window...\nThe new Script will replace the current contents of the viewer\n\n";
        //                foreach ( var item in Generics )
        //                {
        //                    string store = "";
        //                    store = item . field1 + ",";
        //                    output += store;
        //                }
        //                // Display the script in whatever chsen container is relevant
        //                bool resetUse = false;
        //                if ( UseFlowdoc == false )
        //                {
        //                    UseFlowdoc = true;
        //                    resetUse = true;
        //                }
        //                if ( output != "" && UseFlowdoc )
        //                {
        //                    string fdinput = $"Procedure Name : {SqlSpCommand . ToUpper ( )}\n\n";
        //                    fdinput += output;
        //                    fdinput += $"\n\nPress ESCAPE to close this window...\n";
        //                    fdl.ShowInfo ( Flowdoc, Filtercanvas , line1: fdinput , clr1: "Black0" , line2: "" , clr2: "Black0" , line3: "" , clr3: "Black0" , header: "" , clr4: "Black0" );
        //                }
        //                else
        //                {
        //                    Mouse . OverrideCursor = Cursors . Arrow;
        //                    if ( UseFlowdoc )
        //                        fdl . ShowInfo ( Flowdoc , Filtercanvas , line1: $"Procedure [{SqlSpCommand . ToUpper ( )}] \ndoes not Support / Require any arguments" , clr1: "Black0" , line2: "" , clr2: "Black0" , line3: "" , clr3: "Black0" , header: "" , clr4: "Black0" );
        //                }
        //                if ( resetUse )
        //                    UseFlowdoc = false;
        //            }
        //        }

        //        private void DbProcsTree_SelectedItemChanged ( object sender , RoutedPropertyChangedEventArgs<object> e )
        //        {
        //            if ( e . NewValue == null )
        //                return;
        //            //var  v = SqlProcedures . IsSelected as Procname;
        //            var tablename = e . NewValue as Database;
        //            if ( tablename == null )
        //            {
        //                if ( e . NewValue == null )
        //                    return;
        //                var tvi = e . NewValue as SqlProcedures;
        //                SqlSpCommand = tvi . Procname;
        //                // Noow get  nmme  of the Db we are in 
        //                var items = DbProcsTree . Items;
        //                if ( items . CurrentItem != null )
        //                {
        //                    var db = items . CurrentItem as Database;
        //                    CurrentSPDb = db . Databasename;
        //                }
        //                else
        //                {
        //                    var v = sender as ItemsControl;
        //                    //foreach ( var item in v . Items )
        //                    //{
        //                    //	Debug. WriteLine ( item . ToString ( ) );
        //                    //}
        //                    var treeItems = WpfLib1 . Utils . FindVisualParent<TextBlock> ( this );
        //                    //treeItems . ForEach ( I => i . IsExpanded = false );
        //                }
        //            }
        //            else
        //            {
        //                var tvi = e . NewValue as Database;
        //                CurrentSPDb = tvi . Databasename;
        //            }

        //        }

        //        private void DbProcsTree_Expanded ( object sender , RoutedEventArgs e )
        //        {

        //        }

        #endregion  Treeview stuff

        #region Execution Window methods
        //****************************************************//

        /// <summary>
        /// Clever method that loads any selected  Stored Procedure into a ScrollViewer
        /// in Genericgrid and SpResultsViewer widows independently of each other.
        /// The Document viewer higlights the current Search term in the SP loaded.
        /// </summary>
        /// <param name="win">Caller window</param>
        /// <param name="spfilename">SP to be loaded</param>
        /// <param name="sptext">Search Text to be highlighted</param>
        /// <returns></returns>
        public bool LoadShowMatchingSproc ( Window win , FlowDocumentScrollViewer flowdocsv , string spfilename , ref string sptext )
        {
            // Read an SP into memory and display it inFlowdocscrollviewer received
            // This reads the SP into memory in sptext  and displays it in the SpResultsViewer Scrollviewer
            this . FetchStoredProcedureCode ( spfilename , ref sptext );
            SplistRightclick = false;
            if ( sptext == "" )
            {
                StdError ( );
                SetStatusbarText ( $"Failed to read the Stored Procedure {Splist . SelectedItem . ToString ( )}" , 1 );
                return false;
            }
            else spTextBuffer = sptext;     // store full sp text in window Property

            // This ensures that both widnows are updated independently
            // depending on which list triggers the reload of the SP.
            if ( win . Name == "Gengrid" )
            {
                /// It is Genericgrid that has triggered thiis data load, so put the SP details into the ScrollDooc
                infotext = sptext;
                flowdocsv . Document = null;
                myFlowDocument = new FlowDocument ( );
                myFlowDocument . Blocks . Clear ( );
                myFlowDocument = CreateBoldString ( myFlowDocument , sptext , Searchtext );
                myFlowDocument . Background = FindResource ( "Black3" ) as SolidColorBrush;
                flowdocsv . Document = myFlowDocument;
            }
            if ( Resultsviewer != null )
            {
                // has the Results viewer gotany items in it's listbox ?
                if ( win . Name == "Spresultsviewer" )
                {
                    Resultsviewer . TextResult . Document = null;
                    FlowDocument myFlowDocument = new FlowDocument ( );

                    Resultsviewer . TextResult . Document = myFlowDocument;
                    Resultsviewer . TextResult . Document . Blocks . Clear ( );
                    Resultsviewer . TextResult . Document = CreateBoldString ( myFlowDocument , sptext , Searchtext );
                    Resultsviewer . TextResult . Document . Background = FindResource ( "Black3" ) as SolidColorBrush;
                }
            }
            return true;
        }

        /// <summary>
        /// Handle cosmetics fr execution window and GenericGrid
        /// </summary>
        /// <param name="srchtext"></param>
        public void SetSpWindowInfoText ( object Sender , object target , string srchtext )
        {
            dynamic Caller = null;
            dynamic Target = null;
            string currentSelection = "";
            Type type = Sender . GetType ( );
            if ( type == typeof ( Genericgrid ) )
                Caller = Sender as Genericgrid;
            else if ( type == typeof ( SpResultsViewer ) )
                Caller = Sender as SpResultsViewer;

            type = target . GetType ( );
            if ( type == typeof ( Genericgrid ) )
            {
                Target = target as Genericgrid;
                Gengrid = Target as Genericgrid;
            }
            else if ( type == typeof ( SpResultsViewer ) )
            {
                Target = target as SpResultsViewer;
                spviewer = Target as SpResultsViewer;
            }
            if ( Splist . SelectedItem == null )
                Splist . SelectedIndex = 0;
            if ( Splist . SelectedItem . ToString ( ) == "" )
            {
                Splist . SelectedIndex = 0;
                currentSelection = Splist . SelectedItem . ToString ( );
            }
            else
                currentSelection = Splist . SelectedItem . ToString ( );

            // create a default SP search item in case it is empty
            if ( srchtext == "" )
                srchtext = "SELECT";

            if ( Target . GetType ( ) == typeof ( Genericgrid ) )
            {
                // Handle GenericGrid Window
                if ( srchtext != "" && Genericgrid . UsingMatches == true )
                {
                    // there is a search text, but showing matches
                    Gengrid . SpInfo . Text = Gengrid . SpInfo2 . Text = $"All SP's matching [{srchtext}]";
                    Gengrid . InfoHeaderPanel . Text = $"All ({Gengrid . Splist . Items . Count}) Stored Procedures are displayed";
                }
                else if ( Genericgrid . UsingMatches == false )
                {
                    // there is a searchtext, but we are  showing all
                    Gengrid . SpInfo2 . Text = $"All available SP's";
                    Gengrid . InfoHeaderPanel . Text = $"All ({Gengrid . Splist . Items . Count}) Stored Procedures are displayed";
                }
            }
            else if ( Target . GetType ( ) == typeof ( SpResultsViewer ) )
            {
                // Handle SpResultsViewer Window
                if ( srchtext != "" && Resultsviewer . UsingMatches == true )
                {
                    /// got searchtext and we are using matches
                    spviewer . Bannerline . Text = $"Stored Procedures Helper : ({spviewer . ListResults . Items . Count}) SP's matching  [{Searchtext}] is  shown)";
                    spviewer . ShowingAllSprocs . Content = $"Show All available SP's. ";
                }
                else if ( Resultsviewer . UsingMatches == false )
                {
                    // there is a search text, but showing all
                    spviewer . Bannerline . Text = $"Stored Procedures Helper : ALL ({spviewer . ListResults . Items . Count}) available SP's  shown)";
                    spviewer . ShowingAllSprocs . Content = $"Show SP's matching [{Gengrid . Searchtext}]. ";
                }
            }
            //spviewer . Bannerline . Text = $"Stored Procedures Helper ({Gengrid . Splist . Items . Count}) SP's Matching  [{Searchtext}] shown)";
            //    spviewer . ShowingAllSprocs . Content = $"Show ALL available SP's. ";
            //}
            //if ( Genericgrid . Resultsviewer != null )
            //{
            //    spviewer . Bannerline . Text = Gengrid . SpInfo2 . Text;
            //    Gengrid . SpInfo2 . Text = $"{Gengrid . Splist . Items . Count} match [{Searchtext}]";
            //    Gengrid . InfoHeaderPanel . Text = $"All ({Gengrid . Splist . Items . Count}) Stored Procedures matching Search Term [ {Searchtext} ] are displayed";
            //    spviewer . Bannerline . Text = $"Stored Procedures Helper ({Gengrid . Splist . Items . Count}) SP's Matching  [{Searchtext}] shown)";
            //    spviewer . ShowingAllSprocs . Content = $"Show ALL available SP's. ";
            //}
            //}
            //else if ( srchtext != "" && Genericgrid . UsingMatches == false )
            //{
            //    if ( Target . GetType ( ) == typeof ( Genericgrid ) )
            //    {
            //        Gengrid . SpInfo . Text = $"All  Matching S.P's";
            //        Gengrid . SpInfo . Text = $"All  S.P's";
            //        Gengrid . SpInfo2 . Text = $"All ({Gengrid . Splist . Items . Count}) available SP's";
            //        Gengrid . InfoHeaderPanel . Text = $"All ({Gengrid . Splist . Items . Count}) Stored Procedures are displayed";
            //    }
            //    else if ( srchtext == "" && Genericgrid . UsingMatches == false )
            //    {
            //        Gengrid . SpInfo . Text = $"All  S.P's";
            //        Gengrid . SpInfo2 . Text = $"All ({Gengrid . Splist . Items . Count}) available SP's";
            //        Gengrid . InfoHeaderPanel . Text = $"All ({Gengrid . Splist . Items . Count})available Stored Procedures are displayed";
            //    }
        }

        public void RunExecute_Click ( Window win )
        {
            // Call method to load/reload SpResultsviewer listbox
            Execsp_Click ( ( object ) win , null );
        }

        public static void GetSystemVariables ( object sender , out string currentselection , out string srchtext )
        {
            Genericgrid grid = null;
            SpResultsViewer spviewer = null;
            srchtext = "";
            currentselection = "";
            if ( sender . GetType ( ) == typeof ( Genericgrid ) )
            {
                grid = GenGrid;
                if ( GenGrid . Splist . SelectedItem . ToString ( ) == "" )
                {
                    GenGrid . Splist . SelectedIndex = 0;
                    currentselection = GenGrid . Splist . SelectedItem . ToString ( );
                }
                else
                    currentselection = GenGrid . Splist . SelectedItem . ToString ( );

                // create a default SP search item (Property) in case it is empty
                if ( srchtext == "" )
                {
                    GenGrid . Searchtext = "SELECT";
                    srchtext = GenGrid . Searchtext;
                }
            }
        }

        private void ExecuteSP_Click ( object sender , RoutedEventArgs e )
        {
            // user has elected to show the SpResultsViewer Execute dialog for Sp's via dblclick in listbox,
            // so show the popup dialog select SP's and enter arguments then try to execute  it.
            string tagstring = "";
            SpResultsViewer spviewer = Resultsviewer;
            Type type = sender . GetType ( );
            if ( type == typeof ( MenuItem ) )
            {
                MenuItem cm = sender as MenuItem;
                if ( cm . Tag != null )
                {
                    tagstring = cm . Tag . ToString ( );
                }
            }
            // std preamble
            string currentselection = "";
            string srchtext = Searchtext;
            GetSystemVariables ( sender , out currentselection , out srchtext );
            // end of preamble

            Mouse . OverrideCursor = Cursors . Wait;
            if ( Resultsviewer == null )
            {
                // SpResultsViewer NOT open, so open it here 
                if ( Splist . SelectedItem != null )
                    SpName . Text = Splist . SelectedItem . ToString ( );
                else
                    SpName . Text = "";
                // make sure our menu tag doesn't give us a different selection item
                if ( tagstring != "" )
                    SpName . Text = tagstring;

                if ( tagstring != "" )
                    currentselection = tagstring;
                else
                    currentselection = SpName . Text;
                // Get a new instance and load the Resultsviewer here 
                spviewer = new SpResultsViewer ( this , currentselection , srchtext );
                // set global pointer to SpResultsViewer
                Resultsviewer = spviewer;
                spviewer . UsingMatches = false;
                // // load methods listbox
                spviewer . createoptypes ( );
                spviewer . optype . UpdateLayout ( );
                //               spviewer . Show ( );
            }
            else
            {
                if ( Splist . SelectedItem != null )
                    currentselection = Splist . SelectedItem . ToString ( );
            }
            // clear Viewer listbox
            spviewer . ListResults . ItemsSource = null;
            spviewer . ListResults . Items . Clear ( );
            if ( UsingMatches == false && ResultsListLoadType == "ALL" )
            {
                // we have a FULL list, so copy them to resultsviewer
                // Load Sp list from our own list which has All SP's in it
                foreach ( var item in Splist . Items )
                {
                    spviewer . ListResults . Items . Add ( item );
                }
                for ( int x = 0 ; x < spviewer . ListResults . Items . Count ; x++ )
                {
                    string str = spviewer . ListResults . Items [ x ] . ToString ( );
                    if ( currentselection != "" )
                    {
                        if ( str == currentselection )
                        {
                            spviewer . ListResults . SelectedIndex = x;
                            break;
                        }
                    }
                    else
                    {
                        if ( str == Splist . SelectedItem . ToString ( ) )
                        {
                            spviewer . ListResults . SelectedIndex = x;
                            break;
                        }
                    }
                }
                if ( spviewer . ListResults . SelectedItem != null )
                    spviewer . ListResults . ScrollIntoView ( spviewer . ListResults . SelectedItem );
                spviewer . ListResults . Refresh ( );
            }
            else if ( UsingMatches == true && ResultsListLoadType == "MATCH" )
            {
                // we need to load matching SP's only
                // Load Sp list from our own list which has matching SP's in it
                //    // our list is matches only, so copy it
                foreach ( var item in Splist . Items )
                {
                    spviewer . ListResults . Items . Add ( item );
                }
                for ( int x = 0 ; x < spviewer . ListResults . Items . Count ; x++ )
                {
                    string str = spviewer . ListResults . Items [ x ] . ToString ( );
                    if ( str == spviewer . ListResults . Items [ x ] . ToString ( ) )
                    {
                        spviewer . ListResults . SelectedIndex = x;
                        break;
                    }
                }
                if ( spviewer . ListResults . SelectedItem != null )
                    spviewer . ListResults . ScrollIntoView ( spviewer . ListResults . SelectedItem );
            }
            else if ( UsingMatches == false && ResultsListLoadType == "MATCH" )
            {
                // we need to load matching SP's only
                // Load Sp list fromSql
                //    // our list is all sp's, so cannot copy it
                string [ ] args = new string [ 1 ];
                args [ 0 ] = Searchtext;
                List<string> list = NewWpfDev . GenDapperQueries . ProcessUniversalQueryStoredProcedure ( "spGetAllSprocsMatchingsearchterm" , args , CurrentTableDomain , out string err );
                foreach ( var item in list )
                {
                    spviewer . ListResults . Items . Add ( item );
                }
                for ( int y = 0 ; y < spviewer . ListResults . Items . Count ; y++ )
                {
                    if ( spviewer . ListResults . Items [ y ] . ToString ( ) == currentselection )
                    {
                        spviewer . ListResults . SelectedIndex = y;
                        break;
                    }
                }
                if ( spviewer . ListResults . SelectedItem != null )
                    spviewer . ListResults . ScrollIntoView ( spviewer . ListResults . SelectedItem );
                // load selected Sp into Scrollviewer
                bool result = LoadShowMatchingSproc ( this , spviewer . TextResult , spviewer . ListResults . SelectedItem . ToString ( ) , ref srchtext );
                // set glboal so cosmetics  work correctly
                Resultsviewer . UsingMatches = true;
            }
            else if ( UsingMatches == false )
            {
                // load matching SP's into ResultsViewer
                //string [ ] args = new string [ 1 ];
                //args [ 0 ] = Searchtext;
                List<string> list = NewWpfDev . GenDapperQueries . ProcessUniversalQueryStoredProcedure ( "spGetStoredProcs" , null , CurrentTableDomain , out string err );
                //foreach ( var item in list )
                //{
                //    spviewer . ListResults . Items . Add ( item );
                //}
                spviewer . ListResults . ItemsSource = list;
                spviewer . ListResults . SelectedIndex = 0;
                for ( int y = 0 ; y < spviewer . ListResults . Items . Count ; y++ )
                {
                    if ( spviewer . ListResults . Items [ y ] . ToString ( ) == currentselection )
                    {
                        spviewer . ListResults . SelectedIndex = y;
                        break;
                    }
                }
                if ( spviewer . ListResults . SelectedItem != null )
                    spviewer . ListResults . ScrollIntoView ( spviewer . ListResults . SelectedItem );
                bool result = LoadShowMatchingSproc ( this , spviewer . TextResult , spviewer . ListResults . SelectedItem . ToString ( ) , ref srchtext );
            }
            else
            {
                // gotta reload from disk, cos we only have matching SP's in our list
                //string [ ] args = new string [ 1 ];
                //args [ 0] = 
                List<string> list = NewWpfDev . GenDapperQueries . ProcessUniversalQueryStoredProcedure ( "spGetStoredProcs" , null , CurrentTableDomain , out string err );
                foreach ( var item in list )
                {
                    spviewer . ListResults . Items . Add ( item );
                }
                for ( int y = 0 ; y < spviewer . ListResults . Items . Count ; y++ )
                {
                    if ( spviewer . ListResults . Items [ y ] . ToString ( ) == currentselection )
                    {
                        spviewer . ListResults . SelectedIndex = y;
                        break;
                    }
                }
                spviewer . ListResults . Refresh ( );
                spviewer . ListResults . SelectedItem = Splist . SelectedItem;
                spviewer . ListResults . ScrollIntoView ( spviewer . ListResults . SelectedItem );
                //Load method selection listbox
                spviewer . createoptypes ( );
                spviewer . optype . UpdateLayout ( );
                /// load and show data in Scrollviewer
                bool result = LoadShowMatchingSproc ( this , spviewer . TextResult , spviewer . ListResults . SelectedItem . ToString ( ) , ref srchtext );
            }
            // resultsviewer is definitely opened, so just reload it with ALL SP's ?
            // toggle its status to Show ALL SP's
            if ( ResultsListLoadType == "MATCH" )
            {
                spviewer . UsingMatches = true;
                spviewer . ShowingAllSPs = false;
            }
            else
            {
                spviewer . UsingMatches = false;
                spviewer . ShowingAllSPs = true;
            }
            if ( spviewer . ListResults . SelectedIndex == -1 )
                spviewer . ListResults . SelectedIndex = 0;
            SetSpWindowInfoText ( this , spviewer , Searchtext );
            spviewer . Show ( );
            Mouse . OverrideCursor = Cursors . Arrow;
        }

        /// <summary>
        ///  Load Execution Window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Execsp_Click ( object sender , RoutedEventArgs e )
        {
            // We are trying to load the SP viewer ( SpResultsViewer ) so need a list of all currently listed SP's
            // plus the content oof the current SP
            string spname = "spGetAllSprocsMatchingsearchterm";  // SpName . Text;
            string [ ] args = new string [ 1 ];
            //Store SP contents search term for use  by later dialogs
            string srchterm = Searchtext;

            if ( Resultsviewer == null )
            {
                //Need to open SpResultsViewer
                spviewer = new SpResultsViewer ( this , spname , srchterm );
                spviewer . Show ( );
                // load listbox in secondary viewer
                Execsp . Visibility = Visibility . Collapsed;
                Execsp . UpdateLayout ( );
                Debug . WriteLine ( $"Executing S.P {spname}" );
            }
            else
                spviewer = Resultsviewer;

            args [ 0 ] = srchterm;
            //call SQL method using SP to get all MATCHING SP's (if srchterm != "")
            List<string> contents = null; ;
            if ( spviewer . ListResults . Items . Count == 0 )
                contents = NewWpfDev . GenDapperQueries . ProcessUniversalQueryStoredProcedure ( spname , args , CurrentTableDomain , out string err );

            if ( contents != null && contents . Count > 0 )
            {
                string line = "";
                string sptext = "";

                // Store search term into our Property for easier access
                SpSearchTerm = selectSp . Text . ToUpper ( );

                // Load listbox with SP's matching searc term
                spviewer . ListResults . ItemsSource = null;
                spviewer . ListResults . Items . Clear ( );
                int selindex = 0, indx = 0;
                if ( contents . Count > 0 )
                {
                    ListBox lb = spviewer . ListResults;
                    // load all sprocs into listbox in our full viewer window
                    foreach ( string item in contents )
                    {
                        lb . Items . Add ( item );
                        if ( CurrentSpSelection != null )
                        {
                            if ( item . ToUpper ( ) == CurrentSpSelection . ToUpper ( ) )
                                selindex = indx;
                        }
                        indx++;
                    }
                    lb . SelectedIndex = selindex;
                    lb . Refresh ( );
                    lb . ScrollIntoView ( lb . SelectedItem );
                    FlowDocument fd = new FlowDocument ( );
                    fd . Blocks . Clear ( );
                    // Get content of 1st sproc (selected above) and load into scrollviewer
                    Gengrid . FetchStoredProcedureCode ( Splist . SelectedItem . ToString ( ) , ref sptext );
                    fd = CreateBoldString ( fd , sptext , SpSearchTerm . ToUpper ( ) );
                    fd . Background = FindResource ( "Black3" ) as SolidColorBrush;
                    spviewer . TextResult . Document = fd;
                }
            }
        }

        public bool DisplayInformationViewer ( bool showSPlist = false , bool reload = true )
        {
            List<string> SpList = new List<string> ( );

            if ( showSPlist && ( InfoGrid . Visibility == Visibility . Visible ) )
            {
                // Load the list of SP's for the InfoGrid viewer
                if ( reload )
                {
                    int count = LoadStoredProcedures ( "spGetStoredProcs" , "" );
                    if ( count == 0 )
                        return false;
                    ViewerGrid . ColumnDefinitions [ 0 ] . Width = new GridLength ( 250 , GridUnitType . Pixel );
                }
                Splist . Visibility = Visibility . Visible;
            }
            else if ( Splist . Items . Count == 0 )
            {
                // Load the list of SP's for the InfoGrid viewer
                int count = LoadStoredProcedures ( "spGetStoredProcs" , "" );
                if ( count == 0 )
                    return false;
            }
            // load text from previously specified file
            myFlowDocument . Blocks . Clear ( );
            if ( myFlowDocument . Blocks . Count == 0 )
            {
                if ( infotext . Contains ( "Information text goes here ...." ) )
                    infotext = File . ReadAllText ( @$"C:\users\ianch\Documents\GenericGridInfo.Txt" );
                myFlowDocument = LoadFlowDoc ( RTBox , FindResource ( "Black3" ) as SolidColorBrush , infotext );
            }
            // Show complete container
            InfoGrid . Visibility = Visibility . Visible;
            RTBox . Visibility = Visibility . Visible;
            RTBox . Refresh ( );
            Mouse . OverrideCursor = Cursors . Arrow;
            InfoViewerShown = true;
            return true;
        }

        /// <summary>
        /// A clever method that loads/reloads the list of SP's in the left hand listboxes
        /// of Genericgrid and SpResultsViewer windows that loads ALL SP's that
        /// are currently in the SQL SP cache
        /// </summary>
        /// <param name="sender">Calling window</param>
        /// <param name="e">Std arguments</param>
        public int LoadStoredProcedures ( string spCommand , string srchterm )
        {
            List<string> NewSplist = new List<string> ( );
            string previousSelection = "";
            if ( Splist . SelectedItem != null )
                previousSelection = Splist . SelectedItem . ToString ( );
            string [ ] args = new string [ 1 ];
            if ( srchterm != "" )
                args [ 0 ] = srchterm;
            else
                args = null;
            // load list of all sp's calling dapper to run an SP that returns  a List<string>
            NewSplist = SProcsDataHandling . CallStoredProcedure ( NewSplist , spCommand , args );
            Splist . ItemsSource = null;
            Splist . UpdateLayout ( );
            // load list into listbox
            Splist . ItemsSource = NewSplist;
            Splist . Items . SortDescriptions . Add ( new SortDescription ( "" , ListSortDirection . Ascending ) );
            SpInfo . Text = SpInfo . Text = $"All S.Procs ";
            SpInfo2 . Text = $"{Splist . Items . Count} available...";
            InfoHeaderPanel . Text = $"All {Splist . Items . Count} Stored Procedures are displayed";
            CurrentSpList = "ALL";
            if ( previousSelection != "" )
            {
                int indx = 0;
                foreach ( var item in Splist . Items )
                {
                    if ( item == previousSelection )
                    {
                        Splist . SelectedIndex = indx;
                        break;
                    }
                }
            }
            Splist . UpdateLayout ( );
            if ( NewSplist != null )
                return NewSplist . Count;
            else return 0;
        }

        private void LoadSpResultsMatch ( object sender , RoutedEventArgs e )
        {
            // user selected option in Context menu to load Results viewer
            if ( Resultsviewer == null )
            {
                ResultsListLoadType = "MATCH";
                ExecuteSP_Click ( sender , null );
            }
            else
                Resultsviewer . Focus ( );
        }

        private void LoadSpResultsAll ( object sender , RoutedEventArgs e )
        {
            // user double clicked in Splist to load Results viewer
            if ( Resultsviewer == null )
            {
                ResultsListLoadType = "ALL";
                ExecuteSP_Click ( sender , null );
            }
            else
                Resultsviewer . Focus ( );
        }

        //****************************************************//
        #endregion Execution Window methods
        //****************************************************//

        //public static void setforegrnd ( string wintoactivate )
        //{
        //    //var hwnd = FindWindow ( IntPtr . Zero , wintoactivate );
        //    //           hwnd . Dump ( );  // make sure we got a match
        //    //           SetForegroundWindow ( hwnd );
        //}
        //[DllImport ( "user32.dll" , SetLastError = true )]
        //public static extern IntPtr SetForegroundWindow ( IntPtr hWnd );

        //[DllImport ( "user32.dll" , EntryPoint = "FindWindow" , SetLastError = true )]
        //public static extern IntPtr FindWindow ( IntPtr ZeroOnly , string lpWindowName );


        //****************************************************//

        #region UNUSED METHODS

        public ObservableCollection<GenericClass> GetFullColumnData ( string table )
        {
            ObservableCollection<GenericClass> returndata = new ObservableCollection<GenericClass> ( );
            string [ ] args = new string [ 1 ];
            string err = "";
            int recordcount = 0;
            args [ 0 ] = $"'{table}'";
            string command = "drop table if exists x_1";
            dgControl . ExecuteDapperTextCommand ( command , args , out err );
            command = $"select table_name column_name, data_type, character_maximum_length, numeric_precision, numeric_scale into x_1 from information_schema.columns where table_name='{args [ 0 ]}'";
            returndata = dgControl . GetDataFromStoredProcedure ( command , args , CurrentTableDomain , out err , out recordcount );
            return returndata;
        }


        #region treeview  properties
        public static string TvSqlCommand { get; set; }
        public static bool TvMouseCaptured { get; set; }
        public static double TvFirstXPos { get; set; }
        public static double TvFirstYPos { get; set; }
        public static double CpFirstXPos { get; set; }
        public static double CpFirstYPos { get; set; }
        public static string SqlSpCommand { get; set; }
        public static string CurrentSPDb { get; set; }

        #endregion  TreeviewProperties 

        public List<string> GetDataTableAsList ( )
        {
            List<string> TablesList = new List<string> ( );
            List<string> list = new List<string> ( );
            string SqlCommand = "spGetTablesList";
            //// All Db's have their own version of this SP.....
            GenericGridSupport . CallStoredProcedure ( list , SqlCommand );
            //This call returns us a DataTable
            DataTable dt = dgControl . GetDataTable ( SqlCommand );
            // This how to access Row data from  a grid the easiest way.... parsed into a List <xxxxx>
            if ( dt != null )
            {
                TablesList = GenericGridSupport . GetDataDridRowsAsListOfStrings ( dt );
            }
            return TablesList;
        }
        private void Button_MouseEnter ( object sender , MouseEventArgs e )
        {
            Button btn = sender as Button;
            LinearGradientBrush brsh = FindResource ( "Black2OrangeSlant" ) as LinearGradientBrush;
            btn . Background = brsh;
            btn . UpdateLayout ( );
        }
        private void CloseBtn_GotFocus ( object sender , RoutedEventArgs e )
        {
            Button btn = sender as Button;
            LinearGradientBrush brsh = FindResource ( "Black2OrangeSlant" ) as LinearGradientBrush;
            btn . Background = brsh;
            btn . UpdateLayout ( );
        }
        private void CloseBtn_IsMouseDirectlyOverChanged ( object sender , DependencyPropertyChangedEventArgs e )
        {
            Button btn = sender as Button;
            LinearGradientBrush brsh = FindResource ( "Black2OrangeSlant" ) as LinearGradientBrush;
            btn . Background = brsh;
            btn . UpdateLayout ( );
        }
        static public string RemoveTrailingChars ( string processQuery )
        {
            if ( processQuery . Trim ( ) . Contains ( "}," ) )
            {
                processQuery = NewWpfDev . Utils . ReverseString ( processQuery );
                processQuery = processQuery . Substring ( 2 );
                processQuery = NewWpfDev . Utils . ReverseString ( processQuery );
            }
            return processQuery;
        }

        //**********************************//
        #region local control support
        //**********************************//
        private async void asyncSqlTables_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {
            string selection = e . AddedItems [ 0 ] . ToString ( );
            int columncount = 0;
            // call User Control to load the selected table from the current Sql Db
            // TODO   NOT WORKING 3/10/2022
            var result = dgControl . LoadData ( $"{selection}" , ShowColumnHeaders , Flags . CurrentConnectionString , out columncount );
        }
        private void IsLoaded ( object sender , RoutedEventArgs e )
        {
            ToggleColumnHeaders . IsChecked = true;
        }
        //-----------------------------------------------------------//
        #endregion local control support

        private void RTBox_MouseRightButtonDown ( object sender , MouseButtonEventArgs e )
        {

        }

        // NOT USED
        public static FlowDocument ProcessRTBParagraph ( FlowDocument document , string SearchTerm )
        {
            //Paragraph p = ( Paragraph )document . Blocks . FirstBlock;
            //string originalRunText = ( ( Run )p . Inlines . FirstInline ) . Text;
            //String word = SearchTerm;

            //var textSearchRange = new TextRange(p . ContentStart , p . ContentEnd);
            //Int32 position = textSearchRange . Text . IndexOf(word , StringComparison . OrdinalIgnoreCase);
            //if ( position < 0 ) return document;

            //TextPointer start;
            //start = textSearchRange . Start . GetPositionAtOffset(position);
            //var end = textSearchRange . Start . GetPositionAtOffset(position + word . Length);

            //var textR = new TextRange(start , end);
            //textR . Text = "";

            //ToolTip tt = new ToolTip();

            //tt . Background = Brushes . LightYellow;
            //tt . Content = new Label() { Content = "Tooltip of HighLighted word" };

            //Run newRun = new Run(word , start);
            //newRun . FontSize = 30;
            //newRun . ToolTip = tt;
            return document;
        }
        public List<string> LoadSPList ( )
        {
            // Load ALL S.Procedures into List<string>
            DataTable dt = new ( );
            // Load list of SP's for viewer panel'
            List<string> list = new List<string> ( );
            dt = DatagridControl . ProcessSqlCommand ( $"spGetStoredProcs" , Flags . CurrentConnectionString );
            foreach ( DataRow row in dt . Rows )
            {
                list . Add ( row . Field<string> ( 0 ) );
            }
            return list;
        }

        #region FlowDoc support

        /// <summary>
        ///  These are the only methods any window needs to provide support for my FlowDoc system.

        // This is triggered/Broadcast by FlowDoc so that the parent controller can Collapse the 
        // Canvas so it  does not BLOCK other controls after being closed.
        private void Flowdoc_FlowDocClosed ( object sender , EventArgs e )
        {
            Filtercanvas . Visibility = Visibility . Collapsed;
        }

        protected void MaximizeFlowDoc ( object sender , EventArgs e )
        {
            // Clever "Hook" method that Allows the flowdoc to be resized to fill window
            // or return to its original size and position courtesy of the Event declard in FlowDoc
            fdl . MaximizeFlowDoc ( this . Flowdoc , Filtercanvas , e );
        }

        private void Flowdoc_MouseLeftButtonUp ( object sender , MouseButtonEventArgs e )
        {
            // Window wide  !!
            // Called  when a Flowdoc MOVE has ended
            MovingObject2 = fdl . Flowdoc_MouseLeftButtonUp ( sender , Flowdoc , MovingObject2 , e );
            // TODO ?????
            //MovingObject2 . ReleaseMouseCapture();
        }

        // CALLED WHEN  LEFT BUTTON PRESSED
        private void Flowdoc_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
        {
            //In this event, we get current mouse position on the control to use it in the MouseMove event.
            MovingObject2 = fdl . Flowdoc_PreviewMouseLeftButtonDown ( sender , Flowdoc , e );
            Debug . WriteLine ( $"MvvmDataGrid Btn down {MovingObject2}" );
        }

        private void Flowdoc_MouseMove ( object sender , MouseEventArgs e )
        {
            // We are Resizing the Flowdoc using the mouse on the border  (Border.Name=FdBorder)
            fdl . Flowdoc_MouseMove ( Flowdoc , Filtercanvas , MovingObject , e );
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
            Point pt = Mouse . GetPosition ( Filtercanvas );
            double dLeft = pt . X;
            double dTop = pt . Y;
        }

        private void LvFlowdoc_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
        {
            //In this event, we get current mouse position on the control to use it in the MouseMove event.
            MovingObject2 = fdl . Flowdoc_PreviewMouseLeftButtonDown ( sender , Flowdoc , e );
        }

        public void fdmsg ( string line1 , string line2 = "" , string line3 = "" )
        {
            //We have to pass the Flowdoc.Name, and Canvas.Name as well as up   to 3 strings of message
            //  you can  just provide one if required
            // eg fdmsg("message text");
            fdl . FdMsg ( Flowdoc , Filtercanvas , line1 , line2 , line3 );
        }

        //-----------------------------------------------------------//
        #endregion Flowdoc support via library

        private void ShowMoveInfo ( object sender , RoutedEventArgs e )
        {
            infotext = File . ReadAllText ( @$"C:\users\ianch\documents\Universal Control Moving Info.Txt" );
            DisplayInformationViewer ( );
        }
        private void Lostfocus ( object sender , RoutedEventArgs e )
        {
            //DragCtrl . MovementEnd(sender , e , this);
        }

        private void CloseAll_Click ( object sender , RoutedEventArgs e )
        {
            Application . Current . Shutdown ( );
        }

        public bool LoadRtDocument ( string spfilename )
        {
            string sptext = "";
            string stringresult = "";

            if ( SplistRightclick == false )
            {
                //  Task . Run ( async ( ) =>
                //{
                //string stringresult = "";
                Gengrid . FetchStoredProcedureCode ( Splist . SelectedItem . ToString ( ) , ref sptext );
                //} );
            }
            else
            {
                // Task . Run ( async ( ) =>
                //{
                Gengrid . FetchStoredProcedureCode ( null , ref sptext );
                //} );
                //sptext = FetchStoredProcedureCode ( null, ref stringresult );
            }
            SplistRightclick = false;
            if ( sptext == "" )
            {
                StdError ( );
                SetStatusbarText ( $"Failed to read the Stored Procedure {Splist . SelectedItem . ToString ( )}" , 1 );
                return false;
            }
            infotext = sptext;
            RTBox . Document = null;
            myFlowDocument . Blocks . Clear ( );
            myFlowDocument = CreateBoldString ( myFlowDocument , sptext , Searchtext );
            myFlowDocument . Background = FindResource ( "Black3" ) as SolidColorBrush;
            RTBox . Document = myFlowDocument;

            //myFlowDocument = CreateBoldString ( myFlowDocument , sptext , Searchtext );
            //RTBox . Document = myFlowDocument;
            //myFlowDocument . Background = FindResource ( "Black3" ) as SolidColorBrush;
            return true;
        }

        private void SrchtextKeyDown ( object sender , KeyEventArgs e )
        {
            //if ( e . Key == Key . Enter )
            //    SPTextset_Click ( sender , null );
            //else if ( e . Key == Key . Escape )
            //    SPtextbox . Visibility = Visibility . Collapsed;
        }

        private void ShowAllSPsClick ( object sender , RoutedEventArgs e )
        {
            // load full list of SP.s
            List<string> SpList = new List<string> ( );
            SpList = SProcsDataHandling . CallStoredProcedure ( SpList , "spGetStoredProcs" );
            Splist . ItemsSource = null;
            Splist . Items . Clear ( );
            Splist . ItemsSource = SpList;
            SpInfo . Text = SpInfo . Text = $"All S.Procs ";
            SpInfo2 . Text = $"{Splist . Items . Count} available...";
            InfoHeaderPanel . Text = $"All ({Splist . Items . Count}) Stored Procedures are displayed";
            CurrentSpList = "ALL";

            //ListCounter . Text = $"{Splist . Items . Count} Files available...";
            //Splist . Items . SortDescriptions . Add ( new SortDescription ( "" , ListSortDirection . Ascending ) );
            //PromptLine . Text = $"Listbox in left Column loaded wiith ALL {SpList . Count} user owned Stored Procedures.";
            Mouse . OverrideCursor = Cursors . Arrow;
        }

        private void ShowMatchingSPsClick ( object sender , RoutedEventArgs e )
        {
            if ( Splist . SelectedItem == null ) return;
            string currentselection = Splist . SelectedItem . ToString ( );
            if ( Searchtext != "" )
            {
                Mouse . OverrideCursor = Cursors . Wait;
                List<string> list = LoadMatchingStoredProcs ( Splist , Searchtext );
                Splist . ItemsSource = null;
                Splist . ItemsSource = list;
                SpInfo . Text = $"All  Matching S.P's";
                SpInfo2 . Text = $"{Splist . Items . Count} match [{Searchtext}]";
                InfoHeaderPanel . Text = $"All ({Splist . Items . Count}) Stored Procedures matching Search Term [ {Searchtext} ] are displayed";
                CurrentSpList = "MATCH";

                bool success = false;
                if ( currentselection != null )
                {
                    foreach ( string item in Splist . Items )
                    {
                        if ( item == currentselection )
                        {
                            success = true;
                            break;
                        }
                    }
                    string sptext = "";
                    string stringresult = "";
                    if ( success )
                    {
                        //Task . Run ( async ( ) =>
                        //{
                        //    string stringresult = "";
                        Gengrid . FetchStoredProcedureCode ( currentselection , ref sptext );
                        //} );
                        //   sptext = FetchStoredProcedureCode ( currentselection , ref stringresult );
                    }
                    else
                    {
                        sptext = "";
                        SetStatusbarText ( $"({Splist . Items . Count}) S.P's matching [{Searchtext}] loaded successfully.\nSorry, but the previous S.P is not in \nthe new (filtered) list which is why the viewer is empty" , 1 );
                        RTBox . Document = null;
                        myFlowDocument . Blocks . Clear ( );
                        StdError ( );

                    }
                }
                //if ( list . Count > 0 )
                //    PromptLine . Text = $"Listbox in left Column loaded wiith ALL {list . Count} user owned Stored Procedures matching Search Term [ {Searchtext}]";
                //else
                //    PromptLine . Text = $"No user owned Stored Procedures matching Search Term [ {Searchtext} ]can be found ???.";
                //ListCounter . Text = $"{Splist . Items . Count} Files available...";
                Mouse . OverrideCursor = Cursors . Arrow;
            }
            //else
            //{
            //    SPtextbox . Visibility = Visibility . Visible;
            //}
        }

        private void RTBox_Loaded ( object sender , RoutedEventArgs e )
        {
            this . Width += 1;
        }

        private void ReloadCurrentSP ( object sender , RoutedEventArgs e )
        {
            // all working, but no longer needed 23/10/22
            //if ( Splist . SelectedItem == null )
            //{
            //    Splist . SelectedIndex = 0;
            //}
            // string sptext = FetchStoredProcedureCode ( Splist . SelectedItem . ToString ( ) );
            //infotext = sptext;
            //myFlowDocument = CreateBoldString ( myFlowDocument , sptext , Searchtext );
            //myFlowDocument . Background = FindResource ( "Black3" ) as SolidColorBrush;
            //RTBox . Document = myFlowDocument;
            //RTBox . UpdateLayout ( );
            //if ( CurrentSpList == "ALL" )
            //    LoadMatchingItems . IsEnabled = false;
            //GridVisible . IsEnabled = true;
            //// open list cos we are opening SP viewer panel
            //GridLength gl = new GridLength ( );
            //gl = ViewerGrid . ColumnDefinitions [ 0 ] . Width;
            //if ( gl . Value < 5 )
            //    ViewerGrid . ColumnDefinitions [ 0 ] . Width = new GridLength ( 200 , GridUnitType . Pixel );
            //Currentpanel = "DATA";
            //if ( Splist . SelectedItem != null )
            //    Splist . SelectedIndex = 0;
            ////all correct
            //LoadAllItems . IsEnabled = false;
            //LoadMatchingItems . IsEnabled = true;
            //InfoVisible . IsEnabled = true;

            //RTBox . Visibility = Visibility . Visible;
            //RTBox . Document . BringIntoView ( );
        }

        private void Search1__DragDialog_LButtonDn ( object sender , MouseButtonEventArgs e )
        {
            ActiveDragControl = SpStringsSelection;
            DragDialog_LButtonDn ( sender , e );
        }

        private void Search1_Filtering_DragDialog_Ending ( object sender , MouseButtonEventArgs e )
        {
            ActiveDragControl = SpStringsSelection;
            DragDialog_Ending ( sender , e );
        }

        private void Search1_Filtering_DragDialog_Moving ( object sender , MouseEventArgs e )
        {
            ActiveDragControl = SpStringsSelection;
            DragDialog_Moving ( sender , e );
        }

        private void Search1_BlockFiltering_Moving ( object sender , MouseButtonEventArgs e )
        {
            e . Handled = true;
            selectedSp . Focus ( );
        }

        private void Search1_BlockFiltering_Moving ( object sender , MouseEventArgs e )
        {
            e . Handled = true;
            selectedSp . Focus ( );
        }

        public static ObservableCollection<GenericClass> LoadTableGeneric (
               string SqlCommand , ref ObservableCollection<GenericClass> GenCollection , out string error )
        {
            GenCollection = null;
            string errormsg = "";
            error = "";
            int DbCount = 0;

            //            $"Entering " . dcwinfo();
            // Set dapperlib scope flag to convert datetime to date string only for displqay usage inj datagrids etc.
            // ConvertDateTimeToNvarchar = true;

            GenCollection = CreateGenericCollection (
            SqlCommand ,
            "" ,
            "" ,
            "" ,
             ref errormsg ,
             CurrentTableDomain );

            //            $"Exiting " . dcwinfo();
            error = errormsg;
            return GenCollection;
        }

        private void TestLinq ( )
        {
            string sentence = "the quick brown fox jumps over the lazy dog";
            // Split the string into individual words to create a collection.  
            string [ ] words = sentence . Split ( ' ' );

            List<GenericClass> list = GridData . Where ( row => Convert . ToInt16 ( row . field1 ) < 25 ) . ToList ( );
            foreach ( GenericClass item in list )
                Debug . WriteLine ( $"{item . field1} = {item . field3}" );

            //    redisClient . SetAdd<string> ( "Qualification:" + setName , list . Select ( x => x . ProductID ) . ToString ( ) );
            /////          }
            //list = from GridData where  field1 != "25"select
            // Using query expression syntax.  
            Debug . WriteLine ( $"\n" );
            // WORKS
            IEnumerable<GenericClass> data = from x in GridData
                                             where Convert . ToInt16 ( x . field1 ) < 10
                                             select x;
            foreach ( GenericClass item in data )
                Debug . WriteLine ( $"{item . field1} = {item . field3}" );
            return;
        }

        private void Magnifier_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
        {
            //if ( InfoGrid . Visibility == Visibility . Visible )
            //{
            //    // Hide complete info viewer container
            //    InfoGrid . Visibility = Visibility . Visible;
            //    RTBox . Visibility = Visibility . Visible;
            //    // Show main datagrid
            //    InfoGrid . Visibility = Visibility . Collapsed;
            //    maingrid . RowDefinitions [ 0 ] . Height = new GridLength ( InfoGrid . ActualHeight + 10 , GridUnitType . Pixel );
            //}
            //else
            //{
            //    // Show info viewer panel
            //    DisplayInformationViewer ( reload: false );
            //    InfoGrid . Visibility = Visibility . Visible;
            //    //GridLength gl = new GridLength ( );
            //    //gl = ViewerGrid . ColumnDefinitions [ 0 ] . Width;
            //    //if ( gl . Value <= 4 )
            //    maingrid . RowDefinitions [ 0 ] . Height = new GridLength ( InfoGrid . Height , GridUnitType . Pixel );
            //}
        }

        private void GenGridCtrl_PreviewMouseRightButtonDown ( object sender , MouseButtonEventArgs e )
        {
            ContextMenu cm = FindResource ( "GenGridContextMenu" ) as ContextMenu;
            // Hide relevant entries
            List<string> hideitems = new List<string> ( );
            hideitems . Add ( "gm1" );
            hideitems . Add ( "gm2" );
            hideitems . Add ( "gm3" );
            hideitems . Add ( "gm4" );

            ContextMenu menu = RemoveMenuItems ( "GenGridContextMenu" , "" , hideitems );
            //forces menu to show immeduiately to right and below mouse pointer
            menu . PlacementTarget = sender as FrameworkElement;
            Point pt = e . GetPosition ( sender as UIElement );
            menu . PlacementRectangle = new Rect ( pt . X , pt . Y , 350 , 300 );
            menu . IsOpen = true;
            e . Handled = true;
        }

        private string ReturnProcedureHeader ( )
        {
            string output = "";
            int recordcount = 0;
            string err = "";
            string [ ] args = new string [ 0 ];
            string [ ] outputs = new string [ 0 ];
            List<string> list = new List<string> ( );
            string arguments = "";
            DatagridControl . CreateGenericCollection ( ref GridData , $"Select * from {Splist . SelectedItem}" , "" , "" , "" , "" , ref list , ref err );
            //            dgControl . GetDataFromStoredProcedure ( $"Select * from {Splist. SelectedItem}", args, CurrentTableDomain,out err, out outputs, out recordcount);
            return output;
        }

        #endregion UNUSED METHODS

        private void ShowlocalFilter_Click ( object sender , RoutedEventArgs e )
        {
            // Working 3/10/22
            // Open the 'Floating' dialog
            LOCALFILTER = true;
            Filtering . Visibility = Visibility . Visible;
            Filtering . UpdateLayout ( );
            filtertext . Text = "";
            filtertext . Focus ( );
            Type type = sender . GetType ( );
            Debug . WriteLine ( $"Show Type is {type}" );
            // Initialize our 'dialog' dragging system
            DragCtrl . InitializeMovement ( Filtering as FrameworkElement );
            if ( LastActiveFillter != "" )
                filtertext . Text = LastActiveFillter;
        }
        public void LoadViewerTables ( object sender , RoutedEventArgs e )
        {
            // Need to do this so we can call functonality from other controls
            Domethod ( );
        }
        public List<string> Domethod ( )
        {
            List<string> TablesList = GetDbTablesList ( CurrentTableDomain );
            AllTables . ItemsSource = TablesList;
            SpTableViewer . Visibility = Visibility . Visible;
            return TablesList;
        }

        private void closeviewer_Click ( object sender , RoutedEventArgs e )
        {
            SpTableViewer . Visibility = Visibility . Collapsed;
        }
        private void tableviewer_LButtonDn ( object sender , MouseButtonEventArgs e )
        {

            Grid sendergrid = sender as Grid;
            string Sendername = sendergrid . Name;
            DoPanelDragInit ( sender , e , Sendername );

        }

        private void tableviewer_Moving ( object sender , MouseEventArgs e )
        {
            if ( MovingObject != null && e . LeftButton == MouseButtonState . Pressed )
            {
                Type type = sender . GetType ( );
                if ( type == typeof ( Button ) )
                {
                    e . Handled = true;
                    return;
                }
                else if ( type == typeof ( TextBox ) )
                {
                    e . Handled = true;
                    return;
                }
                else
                {
                    DragCtrl . CtrlMoving ( sender , e );
                    e . Handled = false;
                }
            }
        }

        private void tableviewer_Ending ( object sender , MouseButtonEventArgs e )
        {
            DragCtrl . MovementEnd ( sender , e );
            e . Handled = true;
        }

        private void tableviewer_KeyDown ( object sender , KeyEventArgs e )
        {
            if ( e . Key == Key . Enter )
            {
                //CurrentSpSelection = Splist . SelectedItem . ToString ( );
                //ExecBtn . RaiseEvent ( new RoutedEventArgs ( System . Windows . Controls . Primitives . ButtonBase . ClickEvent ) );
                e . Handled = true;
            }
            else if ( e . Key == Key . Escape )
            {
                SpTableViewer . Visibility = Visibility . Collapsed;
                e . Handled = true;
            }
            e . Handled = false;
        }
        public static void DoPanelDragInit ( object sender , MouseButtonEventArgs e , string Sendername )
        {
            Control activegrid = null;
            Grid grid = new Grid ( );
            FrameworkElement parent = null;
            FrameworkElement fwelement = new FrameworkElement ( );
            Type type = sender . GetType ( );
            Debug . WriteLine ( $"Type={type}" );
            // Finds the parent immediately above the Canvas,
            // which is what we are dragging
            if ( type == typeof ( Button ) )
                return;
            else if ( type == typeof ( TextBox ) )
                return;
            else if ( type == typeof ( Border ) )
            {
                parent = NewWpfDev . Utils . FindVisualParent<Grid> ( ( DependencyObject ) sender , out string [ ] objectarray );
                if ( parent != null )
                {
                    if ( parent . Name == Sendername )
                    {
                        DragCtrl . InitializeMovement ( ( FrameworkElement ) parent );
                        DragCtrl . MovementStart ( parent , e );
                        e . Handled = true;
                        return;
                    }
                    else
                    {
                        if ( type == typeof ( Grid ) )
                        {
                            grid = sender as Grid;
                            if ( grid . Name == Sendername )
                            {
                                DragCtrl . InitializeMovement ( ( FrameworkElement ) grid );
                                DragCtrl . MovementStart ( grid , e );
                                e . Handled = true;
                                return;
                            }
                        }
                    }
                }
                else
                {
                    parent = ( Grid ) FindOuterParent ( sender , e );
                    if ( parent . GetType ( ) == typeof ( Grid ) && parent . Name == Sendername )
                    {
                        DragCtrl . InitializeMovement ( parent );
                        DragCtrl . MovementStart ( parent , e );
                        e . Handled = true;
                        return;
                    }
                }
            }
            else if ( type == typeof ( Grid ) )
            {
                grid = sender as Grid;
                if ( grid . Name == Sendername )
                {
                    DragCtrl . InitializeMovement ( grid );
                    DragCtrl . MovementStart ( grid , e );
                    e . Handled = true;
                    return;
                }
                else
                {
                    parent = ( Grid ) FindOuterParent ( sender , e );
                    if ( parent . GetType ( ) == typeof ( Grid ) && parent . Name == Sendername )
                    {
                        DragCtrl . InitializeMovement ( parent );
                        DragCtrl . MovementStart ( parent , e );
                        e . Handled = true;
                        return;
                    }
                }
            }
            else
            {
                parent = ( Grid ) FindOuterParent ( sender , e );
                if ( parent . GetType ( ) == typeof ( Grid ) && parent . Name == Sendername )
                {
                    DragCtrl . InitializeMovement ( parent );
                    DragCtrl . MovementStart ( parent , e );
                    e . Handled = true;
                    return;
                }
            }

        }
    }
}
