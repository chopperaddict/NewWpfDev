using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Data;
using System . Diagnostics;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Documents;
using System . Windows . Input;
using System . Windows . Media;
using File = System . IO . File;

using DapperGenericsLib;
using NewWpfDev;
using NewWpfDev . Views;
using GenericClass = NewWpfDev . GenericClass;
using UserControls;
using System . Linq;
using System . CodeDom;
using GenericSqlLib . Models;
using NewWpfDev . ViewModels;
using Microsoft . VisualBasic;
using DocumentFormat . OpenXml . Bibliography;
using System . Net;
using ServiceStack . Redis;
using Microsoft . Xaml . Behaviors . Media;

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
        public static ObservableCollection<GenericClass> GridData = new ObservableCollection<GenericClass> ( );
        public static ObservableCollection<GenericClass> ColumnsData = new ObservableCollection<GenericClass> ( );
        static public DragCtrlHelper DragCtrl = new DragCtrlHelper ( );
        static public DatagridControl dgControl;
        static public Genericgrid GenControl;
        public DataGrid Dgrid;
        public int ColumnsCount = 0;
        public bool bStartup = true;
        public bool ShowColumnHeaders = false;
        public List<int> SelectedRows = new List<int> ( );
        //This is Updated by my Grid Control whenever it loads a different table
        // NB : Must be declared as shown, including it's name;
        public static string CurrentTableDomain = "IAN1";
        public static string LastActiveFillter = "";
        public static string LastActiveTable = "";
        public static string NewTableSelection = "";
        public bool TableIsEmpty = false;

        public static string NewSelectedTableName = "";
        public bool InfoViewerShown = false;
        public static bool USERRTBOX = true;
        public static bool IsMoving = false;
        public FrameworkElement ActiveDragControl { get; set; }
        public static string DbConnectionString = "Data Source=DINO-PC;Initial Catalog=\"IAN1\";Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public double MAXLISTWIDTH = 250;
        public object MovingObject;

        private bool dataLoaded;
        public bool DataLoaded
        {
            get { return dataLoaded; }
            set { dataLoaded = value; }
        }

        private string currentTable;
        public string CurrentTable
        {
            get { return currentTable; }
            set { currentTable = value; }
        }



        // Flowdoc file wide variables
        // Pointer to the special library FlowdocLib.cs 
        //FlowdocLib fdl = new FlowdocLib();
        //private double XLeft = 0;
        //private double YTop = 0;
        //private bool UseFlowdoc = true;
        //public static object MovingObject2
        //{
        //    get; set;
        //}
        #region Attached properties

        public static int GetToolTipDelayBetweenShow ( DependencyObject obj )
        { return ( int ) obj . GetValue ( ToolTipDelayBetweenShow ); }
        public static void SetToolTipDelayBetweenShow ( DependencyObject obj , int value )
        { obj . SetValue ( ToolTipDelayBetweenShow , value ); }

        public static readonly DependencyProperty ToolTipDelayBetweenShow =
            DependencyProperty . Register ( "ToolTipDelayBetweenShow" , typeof ( int ) , typeof ( Genericgrid ) , new PropertyMetadata ( 5000 ) );



        //public static Control GetActiveDragControl ( DependencyObject obj )
        //{
        //    return ( Control ) obj . GetValue ( ActiveDragControlProperty );
        //}

        //public static void SetActiveDragControl ( DependencyObject obj , Control value )
        //{
        //    obj . SetValue ( ActiveDragControlProperty , value );
        //}
        //public static readonly DependencyProperty ActiveDragControlProperty =
        //    DependencyProperty . RegisterAttached ( "ActiveDragControl" , typeof ( Control ) , typeof ( Genericgrid ) , new PropertyMetadata ( (Control)null ) );


        #endregion Attached properties

        #region Dependency properties
        public bool IsLoading
        {
            get { return ( bool ) GetValue ( IsLoadingProperty ); }
            set { SetValue ( IsLoadingProperty , value ); }
        }
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty . Register ( "IsLoading" , typeof ( bool ) , typeof ( Genericgrid ) , new PropertyMetadata ( true ) );

        public bool ListReloading
        {
            get
            {
                return ( bool ) GetValue ( ListReloadingProperty );
            }
            set
            {
                SetValue ( ListReloadingProperty , value );
            }
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


        //-----------------------------------------------------------//
        #endregion Dependency properties
        //-----------------------------------------------------------//

        public Genericgrid ( )
        {
            InitializeComponent ( );
            this . DataContext = this;
            GenericGridSupport . SetPointers ( null , this );
            GenControl = this;
            Mouse . OverrideCursor = Cursors . Wait;
            CurrentTable = "BankAccount";
            dgControl = this . GenGridCtrl;
            Dgrid = dgControl . datagridControl;
            Task . Run ( ( ) => LoadDbTables ( "BankAccount" ) );
            GenericGridSupport . SelectCurrentTable ( "BankAccount" );
            ToggleColumnHeaders . IsChecked = ShowColumnHeaders;
            ColumnsCount = Dgrid . Columns . Count;
            bStartup = false;
            DatagridControl . SetParent ( ( Control ) this );
            Flags . UseScrollView = false;
            Mouse . OverrideCursor = Cursors . Arrow;
            // TODO  TEMP ON:Y
            // Dummy entry in save field
            NewTableName . Text = "qwerty";
            OptionsList . SelectedIndex = 0;
            ViewerGrid . ColumnDefinitions [ 0 ] . Width = new GridLength ( 1 , GridUnitType . Pixel );
            ViewerGrid . ColumnDefinitions [ 2 ] . Width = new GridLength ( 1 , GridUnitType . Star );
            // Ensure GridViewer panel is hidden on startup
            InfoGrid . Visibility = Visibility . Collapsed;
            dgControl . datagridControl . Visibility = Visibility . Visible;
            ToolTipService . SetBetweenShowDelay ( Vsplitter , 5000 );
            int myInt = ToolTipService . GetBetweenShowDelay ( ( DependencyObject ) FindName ( "Vsplitter" ) );
            Vsplitter . SetValue ( ToolTipDelayBetweenShow , myInt );
            MovingObject = Filtering;
        }



        //**********************************//
        #region local control support
        //**********************************//
        private void SqlTables_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {
            if ( ListReloading == false && e . AddedItems . Count > 0 )
            {
                 string selection = e . AddedItems [ 0 ] . ToString ( );
                NewTableSelection = selection;
                LastActiveTable = CurrentTable;
                string [ ] args = { "" };
                args [ 0 ] = selection;
                // This value is necessary for an SQL Output value to be accessed
                int recordcount = 0;
                string Tablename = "";
                int exist = dgControl . ProcessUniversalStoredProcedure ( "spCheckTableExists" , args , out string err , out recordcount , out Tablename );
                if ( exist == -1 && err == "" )
                {
                    if ( DataLoaded == false && TableIsEmpty == false)
                    {
                        if ( recordcount > 0 )
                        {
                            var GridData = dgControl . LoadGenericData ( $"{selection}" , ShowColumnHeaders , DbConnectionString );
                            //LastActiveTable = selection;
                            //DataLoaded = true;
                            //SqlTables . SelectedItem = LastActiveTable;
                            //CurrentTable = selection;
                            //// GridData = result;

                            int colcount = dgControl . datagridControl . Columns . Count;
                            DatagridControl . LoadActiveRowsOnlyInGrid ( dgControl . datagridControl , GridData , colcount );
                            List<DapperGenericsLib . DataGridLayout> dglayoutlist = new List<DapperGenericsLib . DataGridLayout> ( );
                            DapperLibSupport . ReplaceDataGridFldNames ( CurrentTable , ref dgControl . datagridControl , ref dglayoutlist , colcount );
                            Reccount . Text = GridData . Count . ToString ( );
                            if ( GridData . Count > 0 )
                                statusbar . Text = $"The data for {selection . ToUpper ( )} was loaded successfully and is shown in the viewer above...";
                            LastActiveTable = selection;
//                            CurrentTable = selection;
                        }
                        else
                        {
                            statusbar . Text = $"A check made for the requested table [ {selection} ]showed that although it does exist, \nit does NOT contain any records and therefore it has NOT been loaded";
                            DapperGenericsLib . Utils . DoErrorBeep ( 400 , 100 , 1 );
                            DapperGenericsLib . Utils . DoErrorBeep ( 300 , 400 , 1 );
                            TableIsEmpty = true;
                            //DataLoaded = true;
                            SqlTables . SelectedItem = LastActiveTable;
                            //DataLoaded = false;
                            //CurrentTable = LastActiveTable;
                        }
                    }
                    else if ( recordcount == 0 )
                    {
                        statusbar.Text= $"A check made for the requested table [ {selection} ]showed that although it does exist, \nit does NOT contain any records and therefore it has NOT been loaded";
                        DapperGenericsLib . Utils . DoErrorBeep ( 400 , 100 , 1 );
                        DapperGenericsLib . Utils . DoErrorBeep ( 300 , 400 , 1 );
                        TableIsEmpty = true;
                        //DataLoaded = true;
                        SqlTables . SelectedItem = LastActiveTable;
                        ////CurrentTable = LastActiveTable;
                    }
                    else if ( GridData . Count > 0 )
                    {
                        if ( TableIsEmpty == false )
                        {
                            GenericClass gc = new GenericClass ( );
                            gc = GridData [ 0 ];
                            int count = NewWpfDev . Utils . GetCollectionColumnCount ( gc );
                            int colcount = dgControl . datagridControl . Columns . Count;
                            if ( count != colcount || colcount == 0 )
                                DatagridControl . LoadActiveRowsOnlyInGrid ( dgControl . datagridControl , GridData , colcount );
                            List<DapperGenericsLib . DataGridLayout> dglayoutlist = new List<DapperGenericsLib . DataGridLayout> ( );
                            DapperLibSupport . ReplaceDataGridFldNames ( CurrentTable , ref dgControl . datagridControl , ref dglayoutlist , colcount );
                            Reccount . Text = GridData . Count . ToString ( );
                            statusbar . Text = $"The data for {selection . ToUpper ( )} was loaded successfully  and is shown above...";
                            CurrentTable = selection;
                        }
                        else
                            TableIsEmpty = false;

                    }
                    else
                        ResetColumnHeaderToTrueNames ( selection , dgControl . datagridControl );
                }
                else
                {
                    MessageBox . Show ( "It appears that the selected Table no longer exists.\nThe Tables list will now be reloaded for you to try this option again." , "Combo Box Error" );
                }
                //LoadDbTables ( CurrentTable );
                //SqlTables . SelectedItem = CurrentTable;
                ////GenericGridSupport . SelectCurrentTable ( CurrentTable );
            }
        }
        public void ResetColumnHeaderToTrueNames ( string CurrentTable , DataGrid Grid )
        {
            //Update Column headers to original column names, so we need to create dummy list just to call Replace headers method
            int colcount = dgControl . datagridControl . Columns . Count;
            List<DapperGenericsLib . DataGridLayout> dglayoutlist = new List<DapperGenericsLib . DataGridLayout> ( );
            DapperLibSupport . ReplaceDataGridFldNames ( CurrentTable , ref Grid , ref dglayoutlist , colcount );
        }

        public async Task<bool> LoadDbTables ( string currentTable )
        {   //task to load list of Db Tables
            CurrentTable = currentTable;
            List<string> TablesList = GetDbTablesList ( "IAN1" );
            Application . Current . Dispatcher . BeginInvoke ( ( ) =>
            {
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
                                SqlTables . SelectedIndex = index;
                            DataLoaded = false;
                            break;
                        }
                        index++;
                    }
                }
            } );
            return true;
        }
        public List<string> GetDbTablesList ( string DbName )
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
            //// All Db's have their own version of this SP.....
            SqlCommand = "spGetTablesList";

            TablesList = GenericGridSupport . CallStoredProcedure ( list , SqlCommand );
            //// return list of all current SQL tables in current Db
            return TablesList;
        }
        public List<string> CallStoredProcedure ( List<string> list , string sqlcommand )
        {
            //This call returns us a DataTable
            DataTable dt = dgControl . GetDataTable ( sqlcommand );
            if ( dt != null )
                list = GenericGridSupport . GetDataDridRowsAsListOfStrings ( dt );
            return list;
        }
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
            string input = "";
            string errormsg = "";
            string output = "";
            int columncount = 0;
            dgControl . GetFullColumnInfo ( CurrentTable , CurrentTable , DbConnectionString );
            return;
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
        //-----------------------------------------------------------//
        private void Grid_Loaded ( object sender , RoutedEventArgs e )
        {
            List<DataGridLayout> dglayoutlist = new List<DataGridLayout> ( );
            List<Dictionary<string , string>> ColumntypesList = new List<Dictionary<string , string>> ( );
            File . Delete ( @"c:\users\ianch\documents\CW.log" );
            $"calling LoadDbAsGenericData" . CW ( );
            string error = "";
            GridData = DatagridControl . LoadDbAsGenericData ( "spGetTableColumnWithSize" , GridData ,
               ref ColumntypesList , CurrentTable , "IAN1" , ref dglayoutlist , ref error , true );
            if ( error != "" )
                Debug . WriteLine ( $"Data Load failed : [ {error} ]" );
            ToggleColumnHeaders . IsChecked = true;
            GridData = dgControl . LoadGenericData ( CurrentTable , true , DbConnectionString );
            //LastActiveTable = CurrentTable;
            //DataLoaded = true;
            //SqlTables . SelectedItem = LastActiveTable;
            Reccount . Text = GridData . Count . ToString ( );
           statusbar . Text = $"The data for {CurrentTable . ToUpper ( )} was loaded successfully  and is shown above...";
            IsLoading = true;
            LastActiveTable = CurrentTable;
        }
        //**********************************//
        #region local control support
        //**********************************//
        private async void asyncSqlTables_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {
            string selection = e . AddedItems [ 0 ] . ToString ( );
            // call User Control to load the selected table from the current Sql Db
            // TODO   NOT WORKING 3/10/2022
            var result = await dgControl . LoadData ( $"{selection}" , ShowColumnHeaders , DbConnectionString );
        }
        private void IsLoaded ( object sender , RoutedEventArgs e )
        {
            ToggleColumnHeaders . IsChecked = true;
        }
        //-----------------------------------------------------------//
        #endregion local control support
        //-----------------------------------------------------------//

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
                string Output = dgControl . GetFullColumnInfo ( CurrentTable , CurrentTable , DbConnectionString , false );
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
                // Show dialog
                FieldSelectionGrid . Visibility = Visibility . Visible;
            }
            else
            {
                // just  do a direct copy
                string [ ] args = { $"{SqlTables . SelectedItem . ToString ( )}" , $"{NewDbName}" , "" , "" };
                int recordcount = 0;
                string Tablename = "";
                dgControl . ProcessUniversalStoredProcedure ( "spCopyDb" , args , out string err , out recordcount , out Tablename );
                // make deep copy of table else it gets cleared elsewhere
                // Create a completely new instance via seriazable Clone method stored in NewWpfDev.Utils (in ObjectCopier class file)
                ObservableCollection<GenericClass> deepcopy = new ObservableCollection<GenericClass> ( );
                string originalname = $"{SqlTables . SelectedItem . ToString ( )}";
                deepcopy = NewWpfDev . Utils . CopyCollection ( GridData , deepcopy );
                GridData = deepcopy;
                string [ ] args1 = { $"{NewDbName}" };
                int colcount = dgControl . datagridControl . Columns . Count;
                DatagridControl . LoadActiveRowsOnlyInGrid ( dgControl . datagridControl , GridData , colcount );
                List<DapperGenericsLib . DataGridLayout> dglayoutlist = new List<DapperGenericsLib . DataGridLayout> ( );
                DapperLibSupport . ReplaceDataGridFldNames ( NewDbName , ref dgControl . datagridControl , ref dglayoutlist , colcount );
                LoadDbTables ( NewDbName );
                SqlTables . SelectedItem = CurrentTable;

                //GenericGridSupport . SelectCurrentTable ( NewDbName );

                if ( dgControl . datagridControl . Items . Count > 0 )
                {
                    statusbar . Text = $"New Table [{NewDbName}] Created successfully, {dgControl . datagridControl . Items . Count} records inserted & Table is now shown in datagrid above....";
                    DapperGenericsLib . Utils . DoErrorBeep ( 400 , 100 , 1 );
                    DapperGenericsLib . Utils . DoErrorBeep ( 300 , 400 , 1 );
                }
                else
                {
                    statusbar . Text = $"New Table [{NewDbName}] could NOT be Created. Error was [{err}] ";
                    DapperGenericsLib . Utils . DoErrorBeep ( 320 , 100 , 1 );
                    DapperGenericsLib . Utils . DoErrorBeep ( 260 , 200 , 1 );
                }
                NewTableName . Text = NewDbName;

            }
            Mouse . OverrideCursor = Cursors . Arrow;
            return 1;
        }
        private void ColNames_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {
            // Working  successfully 19/9/22
            DataGrid grid = sender as DataGrid;
            int index = -1;
            bool ismatched = false;
            // Get Index of selected row in datagrid
            if ( grid . CurrentItem != null )
                index = grid . Items . IndexOf ( grid . CurrentItem );

            // Save selected row to list
            foreach ( var item in SelectedRows )
            {
                if ( item == index )
                {
                    ismatched = true;
                    break;
                }
            }
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
            if ( ismatched == false )
                SelectedRows . Add ( index );
        }
        private void GoBtn_Click ( object sender , RoutedEventArgs e )
        {
            "" . Track ( );
            // Creating new table based on selected columns
            DapperGenericsLib . GenericClass selColumns = new DapperGenericsLib . GenericClass ( );
            ObservableCollection<DapperGenericsLib . GenericClass> collection = new ObservableCollection<DapperGenericsLib . GenericClass> ( );
            List<GenericToRealStructure> grsList = new List<GenericToRealStructure> ( );
            string [ ] cols = new string [ ColNames . SelectedItems . Count ];
            string Columnsinfo = dgControl . GetFullColumnInfo ( "'test" , CurrentTable , Genericgrid . DbConnectionString , false , false );
            string [ ] colnames = Columnsinfo . Split ( "\n" );
            DataGrid grid = ColNames;
            int selindex = 0;
            // Create list of selected items only
            List<string> flddescription = new List<string> ( );

            foreach ( DapperGenericsLib . GenericClass item in ColNames . SelectedItems )
            {
                //NOT Working 11/10/22
                GenericToRealStructure grs = new GenericToRealStructure ( );
                if ( selindex > ColNames . SelectedItems . Count - 1 )
                    break;
                bool indexfound = false;

                // Get column name from full set of columns
                string tmp = item . field1;
                string [ ] tmparray = tmp . Split ( "," );
                string setfield = tmparray [ 0 ];

                for ( int y = 0 ; y < SelectedRows . Count - 1 ; y++ )
                {
                    // iterate thru all selected columns identifying the column position of all selected fields
                    for ( int p = 0 ; p < tmparray . Length - 1 ; p++ )
                    {
                        string tmp2 = colnames [ p ];
                        string [ ] tmparray2 = tmp2 . Split ( "," );
                        // Get field name
                        string fldname = tmparray2 [ 0 ] . ToUpper ( );
                        // does selected field name match current table column ?
                        if ( fldname == setfield )
                        {
                            grs . colindex = SelectedRows [ y ];
                            indexfound = true;
                            break;
                        }
                    }
                    grs . fname = item . field1;
                    grs . ftype = item . field2;
                    if ( item . field3 != "" )
                        grs . decroot = Convert . ToInt32 ( item . field3 );
                    if ( item . field4 != null && item . field4 != "" )
                    {
                        if ( item . field4 . Contains ( "," ) )
                        {
                            int indx = 0;
                            for ( int entry = 0 ; entry < item . field4 . Length ; entry++ )
                            {
                                if ( item . field4 [ entry ] == ',' )
                                {
                                    item . field4 = item . field4 . Substring ( 0 , entry );
                                    break;
                                }
                            }
                            grs . decpart = Convert . ToInt32 ( item . field4 );
                        }
                    }
                    grsList . Add ( grs );
                    selindex++;
                    Debug . WriteLine ( $"GrsList DATA : {grs . fname} {grs . colindex}" );
                    break;
                }
            }
            // We now have all selected columns in grslist List, so hide selection dialog again
            FieldSelectionGrid . Visibility = Visibility . Collapsed;

            // Needed ????
            {
                //GenericClass tem = new GenericClass ( );
                //tem = item;
                //collection . Add ( tem );
                //}
                // Assign this collection to selection datagrid and show it
                //ColNames . ItemsSource = collection;

                //List<GenericToRealStructure> grsList = new List<GenericToRealStructure> ( );
                //foreach ( var item in collection )
                //{
                //    GenericToRealStructure grs = new GenericToRealStructure ( );
                //    grs . fname = item . field1;
                //    grs . ftype = item . field2;
                //    if ( item . field3 != "" )
                //        grs . decroot = Convert . ToInt32 ( item . field3 );
                //    if ( item . field4 != "" )
                //    {
                //        if ( item . field4 . Contains ( "," ) )
                //        {
                //            int indx = 0;
                //            for ( int entry = 0 ; entry < item . field4 . Length ; entry++ )
                //            {
                //                if ( item . field4 [ entry ] == ',' )
                //                {
                //                    item . field4 = item . field4 . Substring ( 0 , entry );
                //                    break;
                //                }
                //            }
                //        }
                //        grs . decpart = Convert . ToInt32 ( item . field4 );
                //    }
                //    grsList . Add ( grs );
                //}
            }
            string err = "";
            //            CreateAsyncTable ( CurrentTable , flddescription, out err );
            CreateAsyncTable ( NewTableName . Text , grsList , out err );
            Mouse . OverrideCursor = Cursors . Arrow;
            e . Handled = true;
        }
        public int CreateAsyncTable ( string NewDbName , List<GenericToRealStructure> TableStruct , out string err )
        {
            int x = 0;
            dgControl = GenGridCtrl;
            string error = "";
            err = "";
            "" . Track ( );
            // Assign new collection to special collection for now
            ColumnsData = dgControl . CreateLimitedTableAsync ( NewDbName , TableStruct , out error );
            if ( error != "" ) err = error;
            // We should now have all the data in our new columns only table
            if ( GridData == null )
                return -1;
            if ( x == -2 )
                NewTableName . Focus ( );
            if ( x == -3 )
                statusbar . Text = $"New Table creation failed, see error log file for more infomation....";
            try
            {
                if ( GridData != null )
                {
                    // reload list of tables so new one is shown as well
                    LoadDbTables ( NewDbName );
                    // select new table in dropdown list only
                    DataLoaded = true;
                    SqlTables . SelectedItem = NewDbName;
                    DataLoaded = false;

                    if ( ColumnsData . Count > 0 )
                    {
                        //Load new columns only data into datarid
                        DatagridControl . LoadActiveRowsOnlyInGrid ( dgControl . datagridControl , ColumnsData , TableStruct . Count );
                    }
                    else
                    {
                        // Load ALL records into datagrid
                        DatagridControl . LoadActiveRowsOnlyInGrid ( dgControl . datagridControl , GridData , TableStruct . Count );
                    }
                    //Update Column headers to original column names, so we need to create dummy list just to call Replace headers method
                    List<DapperGenericsLib . DataGridLayout> dglayoutlist = new List<DapperGenericsLib . DataGridLayout> ( );

                    DapperLibSupport . ReplaceDataGridFldNames ( NewDbName , ref dgControl . datagridControl , ref dglayoutlist , TableStruct . Count );

                    // make deep copy of table else it gets cleared elsewhere
                    // Create a completely new instance via seriazable Clone method stored in NewWpfDev.Utils (in ObjectCopier class file)
                    ObservableCollection<GenericClass> deepcopy = new ObservableCollection<GenericClass> ( );
                    deepcopy = NewWpfDev . Utils . CopyCollection ( ColumnsData , deepcopy );
                    GridData = deepcopy;
                    if ( dgControl . datagridControl . Items . Count > 0 )
                    {
                        statusbar . Text = $"New Table [{NewDbName}] Created successfully, {dgControl . datagridControl . Items . Count} records inserted & Table is now shown in datagrid above....";
                        DapperGenericsLib . Utils . DoErrorBeep ( 450 , 200 , 1 );
                        DapperGenericsLib . Utils . DoErrorBeep ( 700 , 300 , 1 );
                        DapperGenericsLib . Utils . DoErrorBeep ( 500 , 300 , 1 );
                    }
                    else
                    {
                        statusbar . Text = $"New Table [{NewDbName}] could NOT be Created. Error was [{err}] ";
                        DapperGenericsLib . Utils . DoErrorBeep ( 320 , 100 , 1 );
                        DapperGenericsLib . Utils . DoErrorBeep ( 260 , 200 , 1 );
                    }
                    NewTableName . Text = NewDbName;
                }
            }
            catch ( Exception ex )
            {
                Debug . WriteLine ( $"Data Loading Failed !!! {ex . Message}" );
            }
            // clear temporay grid data
            ColNames . ItemsSource = null;
            Mouse . OverrideCursor = Cursors . Arrow;
            // hide selection dialog
            FieldSelectionGrid . Visibility = Visibility . Collapsed;
            return 1;
        }

        #endregion Select columns for new table support
        //-----------------------------------------------------------//

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

        private void statusbar_KeyDown ( object sender , KeyEventArgs e )
        {
            if ( e . Key == Key . Enter )
                CreateNewTableAsync ( sender , e );
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
        private void stopBtn_Click ( object sender , RoutedEventArgs e )
        {
            // Aborting creation of new table based on selected columns
            FieldSelectionGrid . Visibility = Visibility . Collapsed;
        }
        private void ShowInfo ( object sender , RoutedEventArgs e )
        {
            // Load boilerplate text describing ths control
            infotext = File . ReadAllText ( @$"C:\users\ianch\Documents\GenericGridInfo.Txt" );
            DisplayInformationViewer ( true );
        }
        public void DisplayInformationViewer ( bool showSPlist = false )
        {
            FlowDocument myFlowDocument = new FlowDocument ( );

            if ( showSPlist && ( InfoGrid . Visibility == Visibility . Visible ) )
            {
                // Load the list of SP's for the InfoGriid viewer
                List<string> SpList = new List<string> ( );
                SpList = CallStoredProcedure ( SpList , "spGetStoredProcs" );

                Splist . ItemsSource = null;
                Splist . ItemsSource = SpList;
                Splist . Items . SortDescriptions . Add ( new SortDescription ( "" , ListSortDirection . Ascending ) );
                SpInfo . Text = $"All S.P's {Splist . Items . Count} available...";
                //PromptLine . Text = $"Listbox in left Column contains ALL accessible User owned Stored Procedures";
                ViewerGrid . ColumnDefinitions [ 1 ] . Width = new GridLength ( 250 , GridUnitType . Pixel );
                Splist . Visibility = Visibility . Visible;
            }
            else if ( Splist . Items . Count == 0 )
            {
                // Load the list of SP's for the InfoGriid viewer
                List<string> SpList = new List<string> ( );
                SpList = CallStoredProcedure ( SpList , "spGetStoredProcs" );

                Splist . ItemsSource = null;
                Splist . ItemsSource = SpList;
                Splist . Items . SortDescriptions . Add ( new SortDescription ( "" , ListSortDirection . Ascending ) );
                SpInfo . Text = $"All S.P's {Splist . Items . Count} available...";
                //PromptLine . Text = $"Listbox in left Column contains ALL accessible User owned Stored Procedures";
                //ViewerGrid . ColumnDefinitions [ 1 ] . Width = new GridLength ( 250 , GridUnitType . Pixel );
                //Splist . Visibility = Visibility . Visible;
            }
            // load text from previosly specified file
            myFlowDocument = LoadFlowDoc ( RTBox , FindResource ( "Black3" ) as SolidColorBrush , infotext );
            RTBox . Visibility = Visibility . Visible;
            InfoGrid . Visibility = Visibility . Visible;
            dgControl . datagridControl . Visibility = Visibility . Collapsed;

            Mouse . OverrideCursor = Cursors . Arrow;
            InfoViewerShown = true;
        }
        private void InfoGrid_KeyDown ( object sender , KeyEventArgs e )
        {
            if ( e . Key == Key . Escape )
            {
                InfoGrid . Visibility = Visibility . Collapsed;
                if ( SpStringsSelector . Visibility == Visibility . Visible )
                    SpStringsSelector . Visibility = Visibility . Collapsed;
                dgControl . datagridControl . Visibility = Visibility . Visible;
                InfoViewerShown = false;
                if ( SpStringsSelector . Visibility == Visibility . Visible )
                    ProcNames . Focus ( );
                Splist . ItemsSource = null;
            }
        }
        private void RTBox_MouseRightButtonDown ( object sender , MouseButtonEventArgs e )
        {

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
                statusbar . Text = $"It appears that the data save to a new table {NewSelectedTableName . ToUpper ( )} FAILED \nand therefore this process has been cancelled !";
            }
            else if ( retval >= -2 )
                statusbar . Text = $"A total of {recsinserted} items have been used to create a new table {NewSelectedTableName . ToUpper ( )} in the \'.IAN1\' Database successfully";
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
        private void OptionsList_Selected ( object sender , SelectionChangedEventArgs e )
        {
            ComboBox cb = sender as ComboBox;
            string selection = e . OriginalSource . ToString ( );
            if ( cb . SelectedIndex == 1 )
                ShowColumnInfo_Click ( sender , e );
            else if ( cb . SelectedIndex == 2 )
                ReloadTables ( sender , e );
            else if ( cb . SelectedIndex == 3 )
                SaveAsNewTable ( sender , e );
            else if ( cb . SelectedIndex == 4 )
                ShowFilter_Click ( sender , e );
            else if ( cb . SelectedIndex == 5 )
                SaveSelectedOnly ( sender , e );
            else if ( cb . SelectedIndex == 6 )
                SearchStoredProc ( sender , e );
            //Reset it to top (non active option so we can select any valid option next time
            cb . SelectedIndex = 0;
        }
        private void SaveAsNewTable ( object sender , RoutedEventArgs e )
        {
            string curtbl = CurrentTable;
            string err = "";
            int result = GenericGridSupport . SaveAsNewTable ( curtbl, NewTableName.Text , out err);
            if ( result == -9 )
            {
                DapperGenericsLib . Utils . DoErrorBeep ( 400 , 100 , 1 );
                DapperGenericsLib . Utils . DoErrorBeep ( 300 , 400 , 1 );
                SqlTables . SelectedItem = LastActiveTable;
                statusbar . Text = $"The Copy of { SqlTables . SelectedItem . ToString ( )} to {curtbl} FAILED.  The reason was {err}";
            }
        }
        private void ReloadTables ( object sender , RoutedEventArgs e )
        {
            if ( CurrentTable == "" )
                CurrentTable = SqlTables . SelectedItem . ToString ( );
            LoadDbTables ( CurrentTable );
            SetValue ( ListReloadingProperty , true );
            SqlTables . SelectedItem = CurrentTable;
            //GenericGridSupport . SelectCurrentTable ( CurrentTable );
            statusbar . Text = "List of Tables reloaded successfully...";
            SetValue ( ListReloadingProperty , false );
            MessageBox . Show ( "List of Sql Tables has been refreshed successfully" , "SQL Tables Utility" );
        }

        private void Gengrid_MouseDoubleClick ( object sender , MouseButtonEventArgs e )
        {
            var obj = e . OriginalSource;
            if ( InfoViewerShown == true && InfoGrid . Visibility == Visibility . Collapsed )
            {
                InfoGrid . Visibility = Visibility . Collapsed;
                if ( SpStringsSelector . Visibility == Visibility . Visible )
                    SpStringsSelector . Visibility = Visibility . Collapsed;
                dgControl . datagridControl . Visibility = Visibility . Visible;
                InfoViewerShown = false;
                Splist . ItemsSource = null;
                //ListCounter . Text = $"Content has been Cleared ...";
                //PromptLine . Text = $"Listbox in left Column has been cleared of All Stored Procedures";
            }
            else if ( InfoViewerShown == true && InfoGrid . Visibility == Visibility . Visible )
            {
                InfoGrid . Visibility = Visibility . Collapsed;
                dgControl . datagridControl . Visibility = Visibility . Visible;
                InfoViewerShown = false;
                Splist . ItemsSource = null;
                if ( SpStringsSelector . Visibility == Visibility . Visible )
                    SpStringsSelector . Visibility = Visibility . Collapsed;
                //                PromptLine . Text = $"Listbox in left Column has been cleared of All Stored Procedures";
                //                ListCounter . Text = $"Content has been Cleared ...";
            }
            else if ( InfoViewerShown == false && InfoGrid . Visibility == Visibility . Visible )
            {
                InfoGrid . Visibility = Visibility . Visible;
                dgControl . datagridControl . Visibility = Visibility . Collapsed;
                InfoViewerShown = true;
            }
            else if ( InfoViewerShown == false && InfoGrid . Visibility == Visibility . Collapsed )
            {
                InfoGrid . Visibility = Visibility . Collapsed;
                dgControl . datagridControl . Visibility = Visibility . Visible;
                InfoViewerShown = true;
                if ( SpStringsSelector . Visibility == Visibility . Visible )
                    SpStringsSelector . Visibility = Visibility . Collapsed;
            }
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
            DragCtrl . InitializeMovement ( Filtering as FrameworkElement , this );
            if ( LastActiveFillter != "" )
                filtertext . Text = LastActiveFillter;
        }
        //**********************************//
        #region Control movement  support
        //**********************************//

        // Finds the parent immediately above the Canvas,
        // which is what we are dragging
        public object GetControlDragParent ( object sender )
        {
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
            object parent = null;

            // Finds the parent immediately above the Canvas,
            // which is what we are dragging
            parent = ( Grid ) GetControlDragParent ( sender );
            Grid filtergrid = parent as Grid;
            DragCtrl . parent = parent as Control;
            DragCtrl . InitializeMovement ( ActiveDragControl , this );
            DragCtrl . MovementStart ( sender , e );
        }

        private void DragDialog_Moving ( object sender , MouseEventArgs e )
        {
            // Working 3/10/22
            if ( MovingObject != null && e . LeftButton == MouseButtonState . Pressed )
            {
                if ( sender . GetType ( ) == typeof ( TextBox ) )
                    return;
                DragCtrl . CtrlMoving ( sender , e );
            }
        }
        private void DragDialog_Ending ( object sender , MouseButtonEventArgs e )
        {
            // Working 3/10/22
            DragCtrl . MovementEnd ( sender , e , this );
        }
        private void FieldSelectionGrid_LostFocus ( object sender , RoutedEventArgs e )
        {
        }

        #endregion Control movement  support

        private void Filtering_KeyDown ( object sender , KeyEventArgs e )
        {
            if ( e . Key == Key . Enter )
                Filter_Click ( sender , null );
            if ( e . Key == Key . Escape )
            {
                Filtering . Visibility = Visibility . Collapsed;
                DragCtrl . MovementEnd ( sender , e , this );
            }
        }
        private void closeFilter ( object sender , RoutedEventArgs e )
        {
            Filtering . Visibility = Visibility . Collapsed;
            DragCtrl . MovementEnd ( sender , e , this );
            GenGridCtrl . Visibility = Visibility . Visible;
        }
        //****************************************************//
        private void Filter_Click ( object sender , RoutedEventArgs e )
        {
            string temp = filtertext . Text . Trim ( );
            string filtercmd = $"{temp}";
            string err = "";
            int recordcount = 0;
            string Tablename = "";
            string originaltable = CurrentTable;
            LastActiveFillter = filtercmd;

            string [ ] args = new string [ 3 ];
            args [ 0 ] = CurrentTable;
            args [ 1 ] = LastActiveFillter;

            string replacementtable = "";
            // GenericClass gc = new GenericClass ( );
            if ( CurrentTable != "FILTEREDDATA" && CurrentTable != "FILTEREDDATA2" )
            {
                string [ ] args2 = new string [ 1 ];
                args2 [ 0 ] = "FILTEREDDATA";
                int result4 = dgControl . ProcessUniversalStoredProcedure ( "spCheckTableExists" , args2 , out err , out recordcount , out Tablename );
                if ( result4 == -1 && recordcount == 0 )
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
                        return;
                    }
                    else
                        replacementtable = "FILTEREDDATA";
                }
            }
            else if ( CurrentTable == "FILTEREDDATA" )
                replacementtable = "FILTEREDDATA2";
            else if ( CurrentTable == "FILTEREDDATA2" )
            {
                replacementtable = "FILTEREDDATA";
            }

            args [ 2 ] = replacementtable;

            int result = dgControl . ProcessUniversalStoredProcedure ( "spSetFilter" , args , out err , out recordcount , out Tablename );
            if ( result == -1 )
            {
                if ( recordcount == 0 )
                {
                    MessageBoxResult result3 = MessageBox . Show ( "The table was filtered successfully, but it resulted in a new table containing ZERO Records.\n\nDo you still want to load this empty table in the viewer ?" , "Sql filtering error" ,
                        MessageBoxButton . YesNo , MessageBoxImage . Question , MessageBoxResult . No );
                    if ( result3 == MessageBoxResult . No )
                    {
                        Filtering . Visibility = Visibility . Collapsed;
                    }
                    return;
                }
                // Filter has worked successflly, so load table 'FilteredData'
                List<DataGridLayout> dglayoutlist = new List<DataGridLayout> ( );
                List<Dictionary<string , string>> ColumntypesList = new List<Dictionary<string , string>> ( );
                //if ( replacementtable != "FILTEREDDATA" && replacementtable != "FILTEREDDATA2" )
                CurrentTable = replacementtable;
                //else
                //{
                //    CurrentTable = "FILTEREDDATA";
                //}

                //ToggleColumnHeaders . IsChecked = true;
                //GridData = dgControl . LoadGenericData ( CurrentTable , true , DbConnectionString );
                //Reccount . Text = GridData . Count . ToString ( );
                //if ( GridData . Count == 0 )
                //{
                //    //Reload original table
                //    MsgBoxArgs msgargs = new MsgBoxArgs ( );
                //    msgargs . title = "Stored Procedure Search System";
                //    msgargs . msg1 = $"Sorry, it does not appear that your filter term [ '{filtercmd . ToUpper ( )}' ] has resulted in NO RECORDS being found in the current Table";
                //    msgargs . msg2 = $"Please try a different Filter term taht is more likely to result in more records being identified. ";
                //    msgargs . msg3 = $"Therefore the original table [{CurrentTable}] is still displayed in the datagrid for you to try again.";
                //    CustomMsgBox cmb = new CustomMsgBox ( );
                //    closeFilter ( sender , e );
                //    cmb . Focus ( );
                //    CurrentTable = originaltable;
                //    GridData = dgControl . LoadGenericData ( CurrentTable , true , DbConnectionString );
                //    Reccount . Text = GridData . Count . ToString ( );
                //    cmb . Show ( msgargs );
                //}
                //ReLoad tables list to include our new temporary table, and select it
                LoadDbTables ( CurrentTable );
                SqlTables . SelectedItem = CurrentTable;
                //if ( replacementtable == "FILTEREDDATA" )
                //    statusbar . Text = $"The Filter completed successfully and was placed into new Table '{replacementtable}' which is displayed in the table viewer ";
                //else if ( replacementtable == "FILTEREDDATA2" )
                //    statusbar . Text = $"The Filter completed successful, but because you the orignal Filtered table has been filtered a 2nd (or subsequent time) it has been saved to a seconndary 'Search results' Table named 'FilteredData2' which is shown above...";

                closeFilter ( sender , e );
            }
            else if ( result == -9 )
            {
                statusbar . Text = $"The Filter did NOT complete successfully, so the original table remains shown above..\nError Mesage returned was {err}....";
                Debug . WriteLine ( err );
                closeFilter ( sender , e );
            }
        }
        #endregion FILTERING SUPPORT incl moving it around
        //-----------------------------------------------------------//

        //****************************************************//
        #region Search SP's for specific text
        //****************************************************//

        private void SpStringsSelection_KeyDown ( object sender , KeyEventArgs e )
        {
            if ( e . Key == Key . Enter )
                GoBtn1_Click ( null , null );
            else if ( e . Key == Key . Escape )
                SpStringsSelection . Visibility = Visibility . Collapsed;
        }

        private void stopBtn2_Click ( object sender , RoutedEventArgs e )
        {
            // close small dialog wih Text Entry field
            SpStringsSelector . Visibility = Visibility . Collapsed;
            DragCtrl . MovementEnd ( sender , e , this );
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
            if ( Searchtext != "" )
            {
                string sptext = FetchStoredProcedureCode ( ProcNames . SelectedItem . ToString ( ) );
                if ( sptext == "" )
                {
                    Debug . WriteLine ( $"ERROR - no SP file   was returned ????" );
                    Mouse . OverrideCursor = Cursors . Arrow;
                    return;
                }
                infotext = sptext;
                // now display the full content of the seleted S.P that has  the search text in it
                FlowDocument myFlowDocument = new FlowDocument ( );
                // Highlight  the search ext in SP file
                myFlowDocument = CreateBoldString ( myFlowDocument , sptext , Searchtext );
                myFlowDocument . Background = FindResource ( "Black3" ) as SolidColorBrush;
                RTBox . Document = myFlowDocument;
                RTBox . Visibility = Visibility . Visible;
                InfoGrid . Visibility = Visibility . Visible;
                dgControl . datagridControl . Visibility = Visibility . Collapsed;
                // Load FULL list of ALL SP's that match searchterm 
                // into left column of  our viewer panel of our Viewergrid
                if ( Splist . Items . Count == 0 )
                {
                    list = LoadMatchingStoredProcs ( Searchtext );
                    Splist . ItemsSource = null;
                    Splist . ItemsSource = list;
                    //                    ListCounter . Text = $"{Splist . Items . Count} Files available...";
                    // default to 1st  entry in list
                    Splist . SelectedIndex = 0;
                    SpInfo . Text = $"All  S.P's Matching [{Searchtext}] ({Splist . Items . Count}) ...";

                    //                    PromptLine . Text = $"Listbox in left Column contains ALL Stored Procedures matching current search term [ {Searchtext}.]";
                }
                //                else
                //                    PromptLine . Text = $"Listbox in left Column contains ALL Stored Procedures matching current search term [ {Searchtext}.]";
            }
            else
            {
                // No search term  exists, so get one now
            }
            // hide main grid - just for neatness
            dgControl . datagridControl . Visibility = Visibility . Collapsed;
            Mouse . OverrideCursor = Cursors . Arrow;
        }
        public string FetchStoredProcedureCode ( string spName )
        {
            // Load a specified SP file
            DataTable dt = new ( );
            string output = "";
            dt = DatagridControl . ProcessSqlCommand ( $"spGetSpecificSchema  {spName}" , Flags . CurrentConnectionString );
            List<string> list = new List<string> ( );
            foreach ( DataRow row in dt . Rows )
            {
                list . Add ( row . Field<string> ( 0 ) );
            }
            // now display  thefull content of the seleted S.P
            if ( list . Count > 0 )
                output = list [ 0 ];
            // infotext = output;
            return output;
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
        private void LoadRTbox ( )
        {
            FlowDocument myFlowDocument = new FlowDocument ( );
            infotext = File . ReadAllText ( @$"C:\users\ianch\documents\GenericGridInfo.Txt" );
            myFlowDocument = LoadFlowDoc ( RTBox , FindResource ( "Black3" ) as SolidColorBrush , infotext );
        }
        private FlowDocument CreateFlowDocumentScroll ( string line1 , string clr1 , string line2 = "" , string clr2 = "" , string line3 = "" , string clr3 = "" , string header = "" , string clr4 = "" )
        {
            FlowDocument myFlowDocument = new FlowDocument ( );
            //NORMAL
            Paragraph para1 = new Paragraph ( );
            // This is  the only paragraph that uses the user defined Font Size....
            para1 . FontSize = 14;
            para1 . FontFamily = new FontFamily ( "Arial" );
            if ( USERRTBOX )
                para1 . Foreground = FindResource ( "White0" ) as SolidColorBrush;
            else
                para1 . Foreground = FindResource ( "Black0" ) as SolidColorBrush;
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
            Paragraph para = new Paragraph ( );

            for ( int x = 0 ; x < NonCapitlisedString . Count ; x++ )
            {
                Run run1 = AddStdNewDocumentParagraph ( NonCapitlisedString [ x ] , SrchTerm );
                para . Inlines . Add ( run1 );
                Run run2 = AddDecoratedNewDocumentParagraph ( NonCapitlisedString [ x ] , SrchTerm );

                if ( x < NonCapitlisedString . Count - 1 )
                    para . Inlines . Add ( run2 );
            }
            // build  document by adding all blocks to Document
            myFlowDocument . Blocks . Add ( para );
            return myFlowDocument;
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

        public Run AddStdNewDocumentParagraph ( string textstring , string SearchText )
        {
            // Main text
            Run run1 = new Run ( textstring );
            run1 . FontSize = 16;
            run1 . FontFamily = new FontFamily ( "Arial" );
            run1 . FontWeight = FontWeights . Normal;
            run1 . Background = FindResource ( "Black4" ) as SolidColorBrush;
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
            run2 . Background = FindResource ( "Black4" ) as SolidColorBrush;
            return run2;
        }
        public List<string> LoadMatchingStoredProcs ( string Searchtext )
        {
            DataTable dt = new DataTable ( );
            string SqlCommand = $"spFindTextInSProc '{Searchtext}'";
            Mouse . OverrideCursor = Cursors . Wait;
            dt = DatagridControl . ProcessSqlCommand ( SqlCommand , Flags . CurrentConnectionString );
            List<string> list = GetDataDridRowsAsListOfStrings ( dt );
            ProcNames . ItemsSource = null;
            ProcNames . Items . Clear ( );
            if ( list . Count > 0 )
            {
                foreach ( var item in list )
                {
                    ProcNames . Items . Add ( item as string );
                }
                ProcNames . SelectedIndex = 0;
                // show sp listbox dialog
            }
            return list;
        }
        private void GoBtn1_Click ( object sender , RoutedEventArgs e )
        {
            // load all SP's that contain the specified search term
            // and add them to listbox in larger selection dialog
            Searchtext = selectedSp . Text;
            List<string> list = LoadMatchingStoredProcs ( selectedSp . Text );
            DragCtrl . InitializeMovement ( SpStringsSelector as FrameworkElement , this );
            if ( ProcNames . Items . Count > 0 )
            {
                SpStringsSelector . Visibility = Visibility . Visible;
                ProcNames . Focus ( );
                if ( Autoclose . IsChecked == true )
                    SpStringsSelection . Visibility = Visibility . Collapsed;
            }
            else
            {
                MsgBoxArgs msgargs = new MsgBoxArgs ( );
                msgargs . title = "Stored Procedure Search System";
                msgargs . msg1 = $"Sorry, it does not appear that the search term [ '{Searchtext . ToUpper ( )}' ]\nhas been found in any of the Stored Procedures in the current SQL Server";
                msgargs . msg2 = $"Please enter a search item likely to be found in a Stored Procedure . ";
                CustomMsgBox cmb = new CustomMsgBox ( );
                cmb . Show ( msgargs );
            }
            SpStringsSelector . Visibility = Visibility . Visible;
            Mouse . OverrideCursor = Cursors . Arrow;
        }

        private void ProcNames_MouseDoubleClick ( object sender , MouseButtonEventArgs e )
        {
            //Load Listbox in larger SP selection dialog with matching files
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
        }

        private void MenuItem_Click ( object sender , RoutedEventArgs e )
        {
            InfoGrid . Visibility = Visibility . Collapsed;
            dgControl . datagridControl . Visibility = Visibility . Visible;
            if ( SpStringsSelector . Visibility == Visibility . Visible )
                SpStringsSelector . Visibility = Visibility . Collapsed;
        }

        private void SearchStoredProc ( object sender , RoutedEventArgs e )
        {
            SpStringsSelection . Visibility = Visibility . Visible;
            selectedSp . Focus ( );
            DragCtrl . InitializeMovement ( SpStringsSelection as FrameworkElement , this );
            selectedSp . SelectAll ( );
        }

        private void stopBtn1_Click ( object sender , RoutedEventArgs e )
        {
            SpStringsSelection . Visibility = Visibility . Collapsed;
        }

        private void Button_KeyDown ( object sender , KeyEventArgs e )
        {
            if ( e . Key == Key . Escape )
            {
                Filtering . Visibility = Visibility . Collapsed;
                DragCtrl . MovementEnd ( sender , e , this );
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
        //-----------------------------------------------------------//

        #region FlowDoc support
        /// <summary>
        ///  These are the only methods any window needs to provide support for my FlowDoc system.

        // This is triggered/Broadcast by FlowDoc so that the parent controller can Collapse the 
        // Canvas so it  does not BLOCK other controls after being closed.
        //private void Flowdoc_FlowDocClosed (object sender , EventArgs e)
        //{
        //    Filtercanvas . Visibility = Visibility . Collapsed;
        //}

        //protected void MaximizeFlowDoc (object sender , EventArgs e)
        //{
        //    // Clever "Hook" method that Allows the flowdoc to be resized to fill window
        //    // or return to its original size and position courtesy of the Event declard in FlowDoc
        //    fdl . MaximizeFlowDoc(Flowdoc , Filtercanvas , e);
        //}

        //private void Flowdoc_MouseLeftButtonUp (object sender , MouseButtonEventArgs e)
        //{
        //    // Window wide  !!
        //    // Called  when a Flowdoc MOVE has ended
        //    MovingObject2 = fdl . Flowdoc_MouseLeftButtonUp(sender , Flowdoc , MovingObject2 , e);
        //    // TODO ?????
        //    //MovingObject2 . ReleaseMouseCapture();
        //}

        //// CALLED WHEN  LEFT BUTTON PRESSED
        //private void Flowdoc_PreviewMouseLeftButtonDown (object sender , MouseButtonEventArgs e)
        //{
        //    //In this event, we get current mouse position on the control to use it in the MouseMove event.
        //    MovingObject2 = fdl . Flowdoc_PreviewMouseLeftButtonDown(sender , Flowdoc , e);
        //    Debug . WriteLine($"MvvmDataGrid Btn down {MovingObject2}");
        //}

        //private void Flowdoc_MouseMove (object sender , MouseEventArgs e)
        //{
        //    // We are Resizing the Flowdoc using the mouse on the border  (Border.Name=FdBorder)
        //    fdl . Flowdoc_MouseMove(Flowdoc , Filtercanvas , MovingObject , e);
        //}

        //// Shortened version proxy call		
        //private void Flowdoc_LostFocus (object sender , RoutedEventArgs e)
        //{
        //    Flowdoc . BorderClicked = false;
        //}

        //public void FlowDoc_ExecuteFlowDocBorderMethod (object sender , EventArgs e)
        //{
        //    // EVENTHANDLER to Handle resizing
        //    FlowDoc fd = sender as FlowDoc;
        //    Point pt = Mouse . GetPosition(Filtercanvas);
        //    double dLeft = pt . X;
        //    double dTop = pt . Y;
        //}

        //private void LvFlowdoc_PreviewMouseLeftButtonDown (object sender , MouseButtonEventArgs e)
        //{
        //    //In this event, we get current mouse position on the control to use it in the MouseMove event.
        //    MovingObject2 = fdl . Flowdoc_PreviewMouseLeftButtonDown(sender , Flowdoc , e);
        //}

        //public void fdmsg (string line1 , string line2 = "" , string line3 = "")
        //{
        //    //We have to pass the Flowdoc.Name, and Canvas.Name as well as up   to 3 strings of message
        //    //  you can  just provie one if required
        //    // eg fdmsg("message text");
        //    fdl . FdMsg(Flowdoc , Filtercanvas , line1 , line2 , line3);
        //}

        //-----------------------------------------------------------//
        #endregion Flowdoc support via library
        //-----------------------------------------------------------//
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

        private void Splist_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {
            //info viewer listbox for SP's
            FlowDocument myFlowDocument = new FlowDocument ( );
            if ( Splist . SelectedItem! != null )
            {
                string sptext = FetchStoredProcedureCode ( Splist . SelectedItem . ToString ( ) );
                infotext = sptext;
                myFlowDocument = CreateBoldString ( myFlowDocument , sptext , Searchtext );
                RTBox . Document = myFlowDocument;
                myFlowDocument . Background = FindResource ( "Black3" ) as SolidColorBrush;
                Mouse . OverrideCursor = Cursors . Arrow;
                //PromptLine . Text = $"Stored Procedure [ {Splist . SelectedItem . ToString ( ) . ToUpper ( )}] now shown in Right hand panel of the viewer ]";
            }
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
            SpList = CallStoredProcedure ( SpList , "spGetStoredProcs" );
            Splist . ItemsSource = null;
            Splist . Items . Clear ( );
            Splist . ItemsSource = SpList;
            SpInfo . Text = $"All S.P's ({Splist . Items . Count}) available...";
            //ListCounter . Text = $"{Splist . Items . Count} Files available...";
            //Splist . Items . SortDescriptions . Add ( new SortDescription ( "" , ListSortDirection . Ascending ) );
            //PromptLine . Text = $"Listbox in left Column loaded wiith ALL {SpList . Count} user owned Stored Procedures.";
            Mouse . OverrideCursor = Cursors . Arrow;
        }

        private void ShowMatchingSPsClick ( object sender , RoutedEventArgs e )
        {
            if ( Searchtext != "" )
            {
                List<string> list = LoadMatchingStoredProcs ( Searchtext );
                Splist . ItemsSource = null;
                Splist . ItemsSource = list;
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


        private void CloseApp_Click ( object sender , RoutedEventArgs e )
        {
            Application . Current . Shutdown ( );
        }


        private void LoadMatchingSPs_Click ( object sender , RoutedEventArgs e )
        {
            string currItem = "";
            if ( Splist . SelectedItem == null )
                currItem = Splist . Items [ 0 ] . ToString ( );
            else
                currItem = Splist . SelectedItem . ToString ( );
            if ( Searchtext != "" )
            {
                List<string> list = LoadMatchingStoredProcs ( Searchtext );
                Splist . ItemsSource = null;
                Splist . Items . Clear ( );
                Splist . ItemsSource = list;
                Splist . SelectedItem = currItem;
                if ( Splist . SelectedItem == null )
                {
                    RTBox . Document = null;
                    Splist . ScrollIntoView ( Splist . Items [ 0 ] . ToString ( ) );
                    statusbar . Text = $"({Splist . Items . Count}) S.P's matching [{Searchtext}] loaded successfully.\nSorry, but the previous S.P is not in the new (filtered) list";
                }
                else
                    statusbar . Text = $"({Splist . Items . Count}) S.P's matching [{Searchtext}] loaded successfully...";
            }
            else
                SpStringsSelection . Visibility = Visibility . Visible;
            Mouse . OverrideCursor = Cursors . Arrow;
        }
        private void LoadAllSPs_Click ( object sender , RoutedEventArgs e )
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
            SpList = CallStoredProcedure ( SpList , "spGetStoredProcs" );
            Splist . ItemsSource = null;
            Splist . Items . Clear ( );
            Splist . ItemsSource = SpList;
            Splist . SelectedItem = currItem;
            Splist . ScrollIntoView ( currItem );
            Mouse . OverrideCursor = Cursors . Arrow;
            statusbar . Text = $"All S.P's ({Splist . Items . Count}) loaded successfully...";
        }
        private void ReloadInfo ( object sender , RoutedEventArgs e )
        {
            FlowDocument myFlowDocument = new FlowDocument ( );
            infotext = File . ReadAllText ( @$"C:\users\ianch\documents\GenericGridInfo.Txt" );
            myFlowDocument = LoadFlowDoc ( RTBox , FindResource ( "Black3" ) as SolidColorBrush , infotext );
            RTBox . UpdateLayout ( );
        }
        private void ReloadCurrentSP ( object sender , RoutedEventArgs e )
        {
            if ( Splist . SelectedItem != null )
            {
                FlowDocument myFlowDocument = new FlowDocument ( );
                string sptext = FetchStoredProcedureCode ( Splist . SelectedItem . ToString ( ) );
                infotext = sptext;
                myFlowDocument = CreateBoldString ( myFlowDocument , sptext , Searchtext );
                myFlowDocument . Background = FindResource ( "Black3" ) as SolidColorBrush;
                RTBox . Document = myFlowDocument;
                RTBox . UpdateLayout ( );
            }
            else
            {
                MessageBox . Show ( "It appears that you do not have any S.P file selected at this time ?" , "Request cannot be performed" );
            }
        }

        public FlowDocument LoadFlowDoc ( FlowDocumentScrollViewer ctrl , SolidColorBrush BkgrndColor = null , string item1 = "" , string clr1 = "" , string item2 = "" , string clr2 = "" , string item3 = "" , string clr3 = "" , string header = "" , string clr5 = "" )
        {
            FlowDocument myFlowDocument = new FlowDocument ( );
            myFlowDocument = CreateFlowDocumentScroll ( item1 , clr1 , item2 , clr2 , item3 , clr3 , header , clr5 );
            ctrl . Document = myFlowDocument;

            myFlowDocument . Background = BkgrndColor;
            ctrl . UpdateLayout ( );
            return myFlowDocument;
        }
        //*************************//
        #region Splitter Cursor handling
        //*************************// 
        private void GridSplitter_MouseEnter ( object sender , MouseEventArgs e )
        {
            if ( this . Cursor != Cursors . Wait )
                Mouse . OverrideCursor = Cursors . SizeWE;
        }

        private void GridSplitter_MouseLeave ( object sender , MouseEventArgs e )
        {
            if ( this . Cursor != Cursors . Wait )
                Mouse . OverrideCursor = Cursors . Arrow;
        }
        #endregion Splitter Cursor handling
        //-------------------------//

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
            ActiveDragControl = FieldSelectionGrid;
            DragDialog_LButtonDn ( sender , e );
        }

        private void ColSelect_DragDialog_Ending ( object sender , MouseButtonEventArgs e )
        {
            ActiveDragControl = FieldSelectionGrid;
            DragDialog_Ending ( sender , e );
        }

        private void ColSelect_DragDialog_Moving ( object sender , MouseEventArgs e )
        {
            ActiveDragControl = FieldSelectionGrid;
            DragDialog_Moving ( sender , e );
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
            DragCtrl . InitializeMovement ( SpStringsSelection as FrameworkElement , this );
            selectedSp . SelectAll ( );
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
    }
}
