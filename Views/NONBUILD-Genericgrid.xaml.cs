using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . Data;
using System . Data . SqlClient;
using System . Diagnostics;
using System . IO;
using System . Linq;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Documents;

using System . Windows . Input;
using System . Windows . Media;

using Dapper;

// 77,824  bytess
// C: drive

using DapperGenericsLib;

using UserControls;

namespace NewWpfDev . Views
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
        public static DatagridControl dgControl;
        public DataGrid Dgrid;
        public int ColumnsCount = 0;
        public bool bStartup = true;
        public bool ShowColumnHeaders = false;
        public List<int> SelectedRows = new List<int> ( );
        //This is Updated by my Grid Control whenever it loads a different table
        // NB : Must be declared as shown, including it's name;
        public static string CurrentTable = "";

        public static string DbConnectionString = "Data Source=DINO-PC;Initial Catalog=\"IAN1\";Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        #region Dependency properties

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

        #endregion Dependency properties

        public Genericgrid ( )
        {
            InitializeComponent ( );
            DataContext = this;
            Mouse . SetCursor ( Cursors . Wait );
            CurrentTable = "BankAccount";
            dgControl = GenGridCtrl;
            Dgrid = dgControl . datagridControl;
            LoadDbTables ( "BankAccount" );
            SelectCurrentTable ( "BankAccount" );
            ToggleColumnHeaders . IsChecked = ShowColumnHeaders;
            ColumnsCount = Dgrid . Columns . Count;
            bStartup = false;
            DatagridControl . SetParent ( this );
            Flags . UseScrollView = false;
            Mouse . SetCursor ( Cursors . Arrow );
            // TODO  TEMP ON:Y
            infotext = File . ReadAllText ( @$"C:\users\ianch\documents\GenericGridInfo.Txt" );
            //infotext = File . ReadAllText ( @$"C:\users\ianch\documents\GenericGridInfo.rtf" );
            LoadRTbox ( );
            NewTableName . Text = "qwerty";
            OptionsList . SelectedIndex = 0;
        }

        private void LoadRTbox ( )
        {
            FlowDocument myFlowDocument = new FlowDocument ( );
            myFlowDocument = CreateFlowDocumentScroll ( infotext , "Black0" );
            RTBox . Document = myFlowDocument;
        }
        private FlowDocument CreateFlowDocumentScroll ( string line1 , string clr1 , string line2 = "" , string clr2 = "" , string line3 = "" , string clr3 = "" , string header = "" , string clr4 = "" )
        {
            FlowDocument myFlowDocument = new FlowDocument ( );
            //NORMAL
            Paragraph para1 = new Paragraph ( );
            // This is  the only paragraph that uses the user defined Font Size....
            para1 . FontSize = 14;
            para1 . FontFamily = new FontFamily ( "Arial" );
            //para1 . Background = FindResource ( "Black3" ) as SolidColorBrush;
            para1 . Foreground = FindResource ( "White0" ) as SolidColorBrush;
            Thickness th = new Thickness ( );
            th . Top = 10;
            th . Left = 10;
            th . Right = 10;
            th . Bottom = 10;
            para1 . Padding = th;
            para1 . Inlines . Add ( new Run ( line1 ) );
            //Add paragraph to flowdocument
            myFlowDocument . Blocks . Add ( para1 );
            return myFlowDocument;

        }
        private void Grid_Loaded ( object sender , RoutedEventArgs e )
        {
            List<DataGridLayout> dglayoutlist = new List<DataGridLayout> ( );
            List<Dictionary<string , string>> ColumntypesList = new List<Dictionary<string , string>> ( );
            GridData = DatagridControl . LoadDbAsGenericData ( "spGetTableColumnWithSize" , GridData ,
               ref ColumntypesList , CurrentTable , "IAN1" , ref dglayoutlist , true );

            ToggleColumnHeaders . IsChecked = true;
            GridData = dgControl . LoadGenericData ( CurrentTable , true , DbConnectionString );
            Reccount . Text = GridData . Count . ToString ( );
            statusbar . Text = $"The data for {CurrentTable . ToUpper ( )} was loaded successfully  and is shown above...";
        }

        public void SelectCurrentTable ( string table )
        {
            int index = 0;
            string currentTable = table . ToUpper ( );
            foreach ( string item in SqlTables . Items )
            {
                if ( currentTable == item . ToUpper ( ) )
                {
                    SqlTables . SelectedIndex = index;
                    break;
                }
                index++;
            }
        }

        //**********************************//
        #region local control support
        //**********************************//
        private void SqlTables_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {
            if ( ListReloading == false && e . AddedItems . Count > 0 )
            {
                //string err = "";
                string selection = e . AddedItems [ 0 ] . ToString ( );
                string [ ] args = { "" , "" };
                args [ 0 ] = selection;
                int exist = dgControl . ProcessUniversalStoredProcedure ( "spCheckTableExists" , args , out string err );
                if ( exist == 0 )
                {
                    var result = dgControl . LoadGenericData ( $"{selection}" , ShowColumnHeaders , DbConnectionString );
                    GridData = result;
                    Reccount . Text = GridData . Count . ToString ( );
                    statusbar . Text = $"The data for {selection . ToUpper ( )} was loaded successfully  and is shown above...";
                    ResetColumnHeaderToTrueNames ( selection , dgControl . datagridControl );
                    CurrentTable = selection;
                }
                else
                {
                    MessageBox . Show ( "It appears that the selected Table no longer exists.\nThe Tables list will now be reloaded for you to try this option again." , "Combo Box Error" );
                    LoadDbTables ( CurrentTable );
                    SelectCurrentTable ( CurrentTable );
                }
            }
        }
        public void ResetColumnHeaderToTrueNames ( string CurrentTable , DataGrid Grid )
        {
            //Update Column headers to original column names, so we need to create dummy list just to call Replace headers method
            int colcount = dgControl . datagridControl . Columns . Count;
            List<DapperGenericsLib . DataGridLayout> dglayoutlist = new List<DapperGenericsLib . DataGridLayout> ( );
            DapperLibSupport . ReplaceDataGridFldNames ( CurrentTable , ref Grid , ref dglayoutlist , colcount );
        }
        private async void asyncSqlTables_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {
            string selection = e . AddedItems [ 0 ] . ToString ( );


            // call User Control to load the selected table from the current Sql Db
            var result = await dgControl . LoadData ( $"{selection}" , ShowColumnHeaders , DbConnectionString );
            CurrentTable = selection;

            //var result  = await Task.Run( () =>dgControl . LoadGenericData ( $"{selection}" , ShowColumnHeaders , DbConnectionString ));
            //            var result = dgControl . LoadData ( $"{selection}" , ShowColumnHeaders , DbConnectionString );
        }

        private async Task<bool> LoadDbTables ( string currentTable )
        {   //task to load list of Db Tables
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
                            SqlTables . SelectedIndex = index;
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

            CallStoredProcedure ( list , SqlCommand );
            //This call returns us a DataTable
            DataTable dt = dgControl . GetDataTable ( SqlCommand );
            // This how to access Row data from  a grid the easiest way.... parsed into a List <xxxxx>
            if ( dt != null )
                TablesList = GetDataDridRowsAsListOfStrings ( dt );
            // return list of all current SQL tables in current Db
            return TablesList;
        }
        public List<string> CallStoredProcedure ( List<string> list , string sqlcommand )
        {
            //This call returns us a DataTable
            DataTable dt = dgControl . GetDataTable ( sqlcommand );
            if ( dt != null )
                //				list = GenericDbHandlers.GetDataDridRowsWithSizes ( dt );
                list = GetDataDridRowsAsListOfStrings ( dt );
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
            dgControl . ShowTrueColumns ( dgControl . datagridControl , CurrentTable , colcount , ShowColumnHeaders );
            ShowColumnHeaders = false;
        }
     
        private void ShowColumnInfo_Click ( object sender , RoutedEventArgs e )
        {
            string input = "";
            string errormsg = "";
            string output = "";
            int columncount = 0;
            if ( dgControl . dglayoutlist . Count == 0 )
            {
                MessageBox . Show ( "To view the Tables Data Structure you need to use the Toggle Column Headers 1st..." , "No Data Available" );
                return;
            }
            dgControl . GetFullColumnInfo ( DatagridControl . CurrentTable , DbConnectionString );
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

        //**********************************//
        #endregion local control support
        //**********************************//

        //**********************************************//
        #region Select columns for new table support
        //**********************************************//
        private void SaveAsNewTable ( object sender , RoutedEventArgs e )
        {
            int result = SaveAsNewTable ( );
        }
        private int SaveAsNewTable ( )
        {
            bool UseSelectedColumns = false;
            List<string> FldNames = new List<string> ( );
            ObservableCollection<GenericClass> collection = new ObservableCollection<GenericClass> ( );
            string NewDbName = NewTableName . Text . Trim ( );
            CurrentTable = NewDbName;
            MessageBoxResult mbresult = MessageBox . Show ( "If you want to select only certain columns from the current table to be saved, Click YES, else click No" , "Data Formatting ?" ,
                MessageBoxButton . YesNoCancel ,
                MessageBoxImage . Question ,
                MessageBoxResult . No );

            //int x = 0;
            if ( mbresult == MessageBoxResult . Cancel )
                return -1;
            if ( mbresult == MessageBoxResult . Yes )
            {
                // Save a set with only user selected columns
                string [ ] args = new string [ 20 ];
                UseSelectedColumns = true;
                string Output = dgControl . GetFullColumnInfo ( DatagridControl . CurrentTable , DbConnectionString , false );
                string buffer = "";
                int index = 0;

                if ( NewDbName == "" )  // Sanity check
                {
                    MessageBox . Show ( "Please enter a suitable name for the table you want to create !" , "Naming Error" );
                    return -1;
                }

                args = Output . Split ( '\n' );
                foreach ( var item in args )
                    if ( item != null && item . Trim ( ) != "" )
                    {
                        string [ ] RawFldNames = item . Split ( ' ' );
                        string [ ] flds = { "" , "" , "" , "" };
                        int y = 0;
                        for ( int x = 0 ; x < RawFldNames . Length ; x++ )
                            if ( RawFldNames [ x ] . Length > 0 )
                                flds [ y++ ] = RawFldNames [ x ];
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
                //ALL WORKING  20/9/2022 - We now have a list of all Column names with
                //column type & size data, so let user choose what to save to a new table!
                SelectedRows . Clear ( );
                // load selection dialog with available clumns
                ColNames . ItemsSource = collection;
                // Show dialog
                FieldSelectionGrid . Visibility = Visibility . Visible;
            }
            else
            {
                // just  do a direct copy
                if ( NewDbName == "" )  // Sanity check
                {
                    MessageBox . Show ( "Please enter a suitable name for the table you want to create !" , "Naming Error" );
                    return -1;
                }
                string [ ] args = { $"{SqlTables . SelectedItem . ToString ( )}" , $"{NewDbName}" };
                dgControl . ProcessUniversalStoredProcedure ( "spCopyDb" , args , out string err );
                // make deep copy of table else it gets cleared elsewhere
                // Create a completely new instance via seriazable Clone method stored in NewWpfDev.Utils (in ObjectCopier class file)
                string originalname = $"{SqlTables . SelectedItem . ToString ( )}";
                ObservableCollection<GenericClass> deepcopy = new ObservableCollection<GenericClass> ( );
                deepcopy = Utils . CopyCollection ( GridData , deepcopy );
                GridData = deepcopy;
                string [ ] args1 = { $"{NewDbName}" };
                int colcount = dgControl . datagridControl . Columns . Count;
                DatagridControl . LoadActiveRowsOnlyInGrid ( dgControl . datagridControl , GridData , colcount );
                ResetColumnHeaderToTrueNames ( NewDbName , dgControl . datagridControl );
                //List<DapperGenericsLib . DataGridLayout> dglayoutlist = new List<DapperGenericsLib . DataGridLayout> ( );
                //DapperLibSupport . ReplaceDataGridFldNames ( NewDbName , ref dgControl . datagridControl , ref dglayoutlist , colcount );
                LoadDbTables ( NewDbName );
                SelectCurrentTable ( NewDbName );

                if ( dgControl . datagridControl . Items . Count > 0 )
                {
                    statusbar . Text = $"Table [{NewDbName}] Created successfully & has  {dgControl . datagridControl . Items . Count} records whicch are shown in datagrid above....";
                    DapperGenericsLib . Utils . DoErrorBeep ( 500 , 100 , 2 );
                }
                else
                {
                    statusbar . Text = $"New Table [{NewDbName}] could NOT be Created. Error was [{err}] ";
                    DapperGenericsLib . Utils . DoErrorBeep ( 320 , 100 , 1 );
                    DapperGenericsLib . Utils . DoErrorBeep ( 260 , 300 , 1 );
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
                if ( item == index )
                {
                    ismatched = true;
                    break;
                }
            //Check and remove any possible duplicate index
            if ( ismatched )
            {
                List<int> temp = new List<int> ( );
                foreach ( var item in SelectedRows )
                    if ( item != index )
                        temp . Add ( item );
                // copy list back to original
                SelectedRows = temp;
            }
            if ( ismatched == false )
                SelectedRows . Add ( index );
        }
        private void GoBtn_Click ( object sender , RoutedEventArgs e )
        {
            // Creating new table based on selected columns
            GenericClass selColumns = new GenericClass ( );
            ObservableCollection<GenericClass> collection = new ObservableCollection<GenericClass> ( );
            List<GenericToRealStructure> grsList = new List<GenericToRealStructure> ( );
            DataGrid grid = ColNames;
            int selindex = 0;
            // Create list of selected items only
            List<int> columnsindex = new List<int> ( );
            foreach ( GenericClass item in ColNames . SelectedItems )
            {
                GenericToRealStructure grs = new GenericToRealStructure ( );
                grs . colindex = SelectedRows [ selindex++ ];
                grs . fname = item . field1;
                grs . ftype = item . field2;
                if ( item . field3 != "" )
                    grs . decroot = Convert . ToInt32 ( item . field3 );
                if ( item . field4 != null && item . field4 != "" )
                    if ( item . field4 . Contains ( "," ) )
                    {
                        int indx = 0;
                        for ( int entry = 0 ; entry < item . field4 . Length ; entry++ )
                            if ( item . field4 [ entry ] == ',' )
                            {
                                item . field4 = item . field4 . Substring ( 0 , entry );
                                break;
                            }
                        grs . decpart = Convert . ToInt32 ( item . field4 );
                    }
                grsList . Add ( grs );
                Debug . WriteLine ( $"GrsList DATA : {grs . fname}, {grs . ftype}, {grs . decroot}, {grs . decpart}, {grs . colindex}" );
                {

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
            CreateAsyncTable ( CurrentTable , grsList , out err );
            Mouse . OverrideCursor = Cursors . Arrow;
            e . Handled = true;
        }
        public int CreateAsyncTable ( string NewDbName , List<GenericToRealStructure> TableStruct , out string err )
        {
            int x = 0;
            dgControl = GenGridCtrl;
            string error = "";
            err = "";
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
            if ( GridData != null )
            {
                // reload list of tables so new one is shown as well
                LoadDbTables ( NewDbName );
                // select new table in dropdown list only
                SelectCurrentTable ( NewDbName );
                // clear display grid contents
                dgControl . datagridControl . ItemsSource = null;
                if ( ColumnsData . Count > 0 )
                    //Load new columns only data into datarid
                    DatagridControl . LoadActiveRowsOnlyInGrid ( dgControl . datagridControl , ColumnsData , TableStruct . Count );
                else
                    // Load ALL records into datagrid
                    DatagridControl . LoadActiveRowsOnlyInGrid ( dgControl . datagridControl , GridData , TableStruct . Count );
                ResetColumnHeaderToTrueNames ( NewDbName , dgControl . datagridControl );

                ////Update Column headers to original column names, so we need to create dummy list just to call Replace headers method
                //List<DapperGenericsLib . DataGridLayout> dglayoutlist = new List<DapperGenericsLib . DataGridLayout> ( );
                //DapperLibSupport . ReplaceDataGridFldNames ( NewDbName , ref dgControl . datagridControl , ref dglayoutlist , TableStruct . Count );

                // make deep copy of table else it gets cleared elsewhere
                // Create a completely new instance via seriazable Clone method stored in NewWpfDev.Utils (in ObjectCopier class file)
                ObservableCollection<GenericClass> deepcopy = new ObservableCollection<GenericClass> ( );
                deepcopy = Utils . CopyCollection ( ColumnsData , deepcopy );
                GridData = deepcopy;

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
            // clear temporay grid data
            ColNames . ItemsSource = null;
            Mouse . OverrideCursor = Cursors . Arrow;
            // hide selection dialog
            FieldSelectionGrid . Visibility = Visibility . Collapsed;
            return 1;
        }

        //***************************************************//
        #endregion Select columns for new table support
        //***************************************************//

        public List<string> GetDataTableAsList ( )
        {
            List<string> TablesList = new List<string> ( );
            List<string> list = new List<string> ( );
            string SqlCommand = "spGetTablesList";
            //// All Db's have their own version of this SP.....
            CallStoredProcedure ( list , SqlCommand );
            //This call returns us a DataTable
            DataTable dt = dgControl . GetDataTable ( SqlCommand );
            // This how to access Row data from  a grid the easiest way.... parsed into a List <xxxxx>
            if ( dt != null )
                TablesList = GetDataDridRowsAsListOfStrings ( dt );
            return TablesList;
        }

        private void statusbar_KeyDown ( object sender , KeyEventArgs e )
        {
            if ( e . Key == Key . Enter )
                SaveAsNewTable ( sender , e );
        }

        private void Button_MouseEnter ( object sender , MouseEventArgs e )
        {
            Button btn = sender as Button;
            LinearGradientBrush brsh = FindResource ( "Black2OrangeSlant" ) as LinearGradientBrush;
            btn . Background = brsh;
            btn . UpdateLayout ( );
            //btn . Refresh ( );
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
        private void ReloadTables ( object sender , RoutedEventArgs e )
        {
            if ( CurrentTable == "" )
                CurrentTable = SqlTables . SelectedItem . ToString ( );
            LoadDbTables ( CurrentTable );
            SetValue ( ListReloadingProperty , true );
            SelectCurrentTable ( CurrentTable );
            statusbar . Text = "List of Tables reloaded successfully...";
            SetValue ( ListReloadingProperty , false );
        }


        private void stopBtn_Click ( object sender , RoutedEventArgs e )
        {
            // Aborting creation of new table based on selected columns
            FieldSelectionGrid . Visibility = Visibility . Collapsed;
        }

        private void ShowInfo ( object sender , RoutedEventArgs e )
        {
            InfoGrid . Visibility = Visibility . Visible;
            GenGridCtrl . Visibility = Visibility . Collapsed;
        }

        private void InfoGrid_KeyDown ( object sender , KeyEventArgs e )
        {
            if ( e . Key == Key . Escape )
            {
                InfoGrid . Visibility = Visibility . Collapsed;
                GenGridCtrl . Visibility = Visibility . Visible;
            }
        }

        private void Filter_Click ( object sender , RoutedEventArgs e )
        {
            string temp = filtertext . Text . Trim ( );
            string filtercmd = $"{temp}";
            string err = "";
            string [ ] args = { $"{CurrentTable}" , $"{filtercmd}" };
            int result = dgControl . ProcessUniversalStoredProcedure ( "spSetFilter" , args , out err );
            if ( result == -1 )
            {
                // Filter has worked successflly, so load table 'FilteredData'
                List<DataGridLayout> dglayoutlist = new List<DataGridLayout> ( );
                List<Dictionary<string , string>> ColumntypesList = new List<Dictionary<string , string>> ( );
                CurrentTable = "FilteredData";
                ToggleColumnHeaders . IsChecked = true;
                GridData = dgControl . LoadGenericData ( CurrentTable , true , DbConnectionString );
                Reccount . Text = GridData . Count . ToString ( );
                //ReLoad tables list to include our new temporary table, and select it
                LoadDbTables ( CurrentTable );
                SelectCurrentTable ( CurrentTable );
                statusbar . Text = $"The Filter completed successfully and was placed into new Table 'FilteredData' which is shown above...";
            }
            closeFilter ( sender , e );
        }

        private void closeFilter ( object sender , RoutedEventArgs e )
        {
            Filtering . Visibility = Visibility . Collapsed;
            GenGridCtrl . Visibility = Visibility . Visible;
        }

        private void ShowFilter_Click ( object sender , RoutedEventArgs e )
        {
            Filtering . Visibility = Visibility . Visible;
            filtertext . Text = "";
            filtertext . Focus ( );
            e . Handled = true;
        }

        private void RTBox_MouseRightButtonDown ( object sender , MouseButtonEventArgs e )
        {
            InfoGrid . Visibility = Visibility . Collapsed;
            GenGridCtrl . Visibility = Visibility . Visible;
        }

        private void SaveSelectedOnly ( object sender , RoutedEventArgs e )
        {
            int retval = ProcessSelectedRows ( false );
            if ( retval != -9 )
            {
                // error - handle it here 
                MessageBox . Show ( $"It appears that the new table structure for [ {NewTableName . Text . Trim ( ) . ToUpper ( )} ] \nwas NOT created successfully, so this process has been cancelled !" , "Fatal problem encountered" , MessageBoxButton . OK );
                statusbar . Text = $"It appears that the new table structure for [ {NewTableName . Text . Trim ( ) . ToUpper ( )} ] \nwas NOT created successfully, so this process has been cancelled !";
            }
        }
        //****************************************//
        // Main method  to save selected rows
        //****************************************//
        public int ProcessSelectedRows ( bool ShowDuplicateErrors = true )
        {
            bool UseSelectedColumns = false;
            List<int> selindex = new List<int> ( );
            List<string> FldNames = new List<string> ( );
            List<string> selrecords = new List<string> ( );
            List<string [ ]> selectedrecords = new List<string [ ]> ( );
            //int selectedRecordCount = 0;
            ObservableCollection<GenericClass> collection = new ObservableCollection<GenericClass> ( );
            string NewDbName = NewTableName . Text . Trim ( );
            CurrentTable = NewDbName;
            // Save a set with only selected rows included
            UseSelectedColumns = true;
            string Output = "";// dgControl . GetFullColumnInfo ( DatagridControl . CurrentTable , DbConnectionString , false );
            string buffer = "";
            int index = -1;

            if ( dgControl . datagridControl . SelectedItems . Count == 1 )
            {
                MessageBoxResult res = MessageBox . Show ( $"Only a single record is selected ?  \nDo you really want to save it to a new and seperate SQL Table ?" , "Selected Record(s) Question ?" ,
                    MessageBoxButton . YesNo , MessageBoxImage . Question , MessageBoxResult . No );
                if ( res == MessageBoxResult . No ) return -1;
            }

            if ( NewDbName == "" )  // Sanity check
            {
                MessageBox . Show ( "Please enter a suitable name for the table you want to create !" , "Table name required" );
                return -1;
            }

            string [ ] flds = { "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" };
            foreach ( object selitem in dgControl . datagridControl . SelectedItems )
            {
                flds = selitem . ToString ( ) . Split ( ',' );
                selrecords . Add ( flds [ 0 ] );
            }
            // check to ensure we do have the new  table structure
            string err = "";
            string [ ] args = { $"{NewDbName}" };

            //**********************************//
            // Ckeck if new table already exists
            //**********************************//
            int result = dgControl . ProcessUniversalStoredProcedure ( "spCheckTableExists" , args , out err , true );

            // Check result
            if ( err . ToUpper ( ) . Contains ( "DOES NOT EXIST" ) )
            {
                //************************************************//
                //  All well, table does not exist, so create it here
                //************************************************//
                CreateNewSqlTableStructure ( NewDbName , collection , out err );
                if ( err != "" )
                {
                    Debug . WriteLine ( $"ERROR : {err}" );
                    return -9;
                }
            }
            else if ( ShowDuplicateErrors && result == -1 )
            {

                MessageBoxResult res = MessageBox . Show ( $"The table named [{NewDbName . ToUpper ( )}] does not exist\n Processing will now be cancelled?" , "File does not Exist" ,
                MessageBoxButton . OK , MessageBoxImage . Warning , MessageBoxResult . OK );
                return -0;
            }

            //***************************//
            // now save data for new table 
            //***************************//
            statusbar . Text = $"Saving Selected Rows only to new  table named [{NewDbName . ToUpper ( )}]....";
            CreateSqlInsertCommand ( args );


            int selectedcount = selrecords . Count;

            //****************************//
            // Work thru all selected iitems
            //****************************//
            //foreach ( var item in dgControl . datagridControl . Items )
            //{
            GenericClass rec = new GenericClass ( );
            // flds = { "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" };
            string [ ] d = { "" , "" };
            //flds = item . ToString ( ) . Split ( "=" );
            int max = 0;
            //Create string array containing row data
            foreach ( var row in GenGridCtrl . SelectedItems )
            {
                string [ ] data = { "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" };
                string tmpflds = row . ToString ( );
                flds = tmpflds . ToString ( ) . Split ( "=" );
                for ( int y = 0 ; y < flds . Length ; y++ )
                {
                    max++;
                    string temp = flds [ y ];
                    d = temp . Split ( ',' );
                    data [ y ] = d [ 0 ] . Trim ( );
                    if ( data [ y ] . Contains ( " " ) )
                        data [ y ] = Utils . ConvertInputDate ( data [ y ] . Trim ( ) );
                }
                // null out unused field elments
                for ( int y = flds . Length - 1 ; y < 20 ; y++ )
                    data [ y ] = null;
                // Add populated array to list
                selectedrecords . Add ( data );
                //}
                //We now have all selected rows in List<string[]> 'selectedrecords'
                //selectedrecords . Clear ( );
                collection . Clear ( );
                //string [ ] data = { "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "", };
                //for ( int z = 0 ; z < selectedrecords . Count ; z++ )
                int selcount = dgControl . datagridControl . SelectedItems . Count;
                for ( int z = 0 ; z < selectedrecords . Count ; z++ )
                {
                    GenericClass record = new GenericClass ( );
                    data = selectedrecords [ z ];
                    for ( int v = 0 ; v < flds . Length - 1 ; v++ )
                    {
                        if ( data [ v ] == "" )
                            break;
                        switch ( v )
                        {
                            case 0:
                                record . field1 = data [ v ] . Contains ( "}" ) ? data [ v ] . Substring ( 0 , data [ v ] . Length - 1 ) . Trim ( ) : data [ v ] . Trim ( );
                                break;
                            case 1:
                                record . field2 = data [ v ] . Contains ( "}" ) ? data [ v ] . Substring ( 0 , data [ v ] . Length - 1 ) . Trim ( ) : data [ v ] . Trim ( );
                                break;
                            case 2:
                                record . field3 = data [ v ] . Contains ( "}" ) ? data [ v ] . Substring ( 0 , data [ v ] . Length - 1 ) . Trim ( ) : data [ v ] . Trim ( );
                                break;
                            case 3:
                                record . field4 = data [ v ] . Contains ( "}" ) ? data [ v ] . Substring ( 0 , data [ v ] . Length - 1 ) . Trim ( ) : data [ v ] . Trim ( );
                                break;
                            case 4:
                                record . field5 = data [ v ] . Contains ( "}" ) ? data [ v ] . Substring ( 0 , data [ v ] . Length - 1 ) . Trim ( ) : data [ v ] . Trim ( );
                                break;
                            case 5:
                                record . field6 = data [ v ] . Contains ( "}" ) ? data [ v ] . Substring ( 0 , data [ v ] . Length - 1 ) . Trim ( ) : data [ v ] . Trim ( );
                                break;
                            case 6:
                                record . field7 = data [ v ] . Contains ( "}" ) ? data [ v ] . Substring ( 0 , data [ v ] . Length - 1 ) . Trim ( ) : data [ v ] . Trim ( );
                                break;
                            case 7:
                                record . field8 = data [ v ] . Contains ( "}" ) ? data [ v ] . Substring ( 0 , data [ v ] . Length - 1 ) . Trim ( ) : data [ v ] . Trim ( );
                                break;
                            case 8:
                                record . field9 = data [ v ] . Contains ( "}" ) ? data [ v ] . Substring ( 0 , data [ v ] . Length - 1 ) . Trim ( ) : data [ v ] . Trim ( );
                                break;
                            case 9:
                                record . field10 = data [ v ] . Contains ( "}" ) ? data [ v ] . Substring ( 0 , data [ v ] . Length - 1 ) . Trim ( ) : data [ v ] . Trim ( );
                                break;
                            case 10:
                                record . field11 = data [ v ] . Contains ( "}" ) ? data [ v ] . Substring ( 0 , data [ v ] . Length - 1 ) . Trim ( ) : data [ v ] . Trim ( );
                                break;
                            case 11:
                                record . field12 = data [ v ] . Contains ( "}" ) ? data [ v ] . Substring ( 0 , data [ v ] . Length - 1 ) . Trim ( ) : data [ v ] . Trim ( );
                                break;
                            case 12:
                                record . field13 = data [ v ] . Contains ( "}" ) ? data [ v ] . Substring ( 0 , data [ v ] . Length - 1 ) . Trim ( ) : data [ v ] . Trim ( );
                                break;
                            case 13:
                                record . field14 = data [ v ] . Contains ( "}" ) ? data [ v ] . Substring ( 0 , data [ v ] . Length - 1 ) . Trim ( ) : data [ v ] . Trim ( );
                                break;
                            case 14:
                                record . field15 = data [ v ] . Contains ( "}" ) ? data [ v ] . Substring ( 0 , data [ v ] . Length - 1 ) . Trim ( ) : data [ v ] . Trim ( );
                                break;
                            case 15:
                                record . field16 = data [ v ] . Contains ( "}" ) ? data [ v ] . Substring ( 0 , data [ v ] . Length - 1 ) . Trim ( ) : data [ v ] . Trim ( );
                                break;
                            case 16:
                                record . field17 = data [ v ] . Contains ( "}" ) ? data [ v ] . Substring ( 0 , data [ v ] . Length - 1 ) . Trim ( ) : data [ v ] . Trim ( );
                                break;
                            case 17:
                                record . field18 = data [ v ] . Contains ( "}" ) ? data [ v ] . Substring ( 0 , data [ v ] . Length - 1 ) . Trim ( ) : data [ v ] . Trim ( );
                                break;
                            case 18:
                                record . field19 = data [ v ] . Contains ( "}" ) ? data [ v ] . Substring ( 0 , data [ v ] . Length - 1 ) . Trim ( ) : data [ v ] . Trim ( );
                                break;
                            case 19:
                                record . field20 = data [ v ] . Contains ( "}" ) ? data [ v ] . Substring ( 0 , data [ v ] . Length - 1 ) . Trim ( ) : data [ v ] . Trim ( );
                                break;
                            default:
                                break;
                        }
                    }
                    collection . Add ( record );

                }
            }
            // Fnally, saved all selected data in an Obs collection 'collection'
            //We now have all data in the selected rows as observableccolllection<genclass>records to save to a new table!

            // 1st, create arguments in an array
            string [ ] array = new string [ 20 ];
            int colcount = 0;
            int activerow = 0;
            string processQuery = "";
            string insertstring = "";
            bool goahead = true;

            //**************************************************************************//
            // Cycle thru each record creating data values for SQL insert command row 
            //**************************************************************************//
            foreach ( var newrow in collection )
            {
                if ( goahead )
                {
                    if ( newrow . field1 != "" ) { array [ 0 ] = newrow . field1 . ToString ( ); colcount++; RemoveTrailingChars ( array [ 0 ] ); if ( array [ 0 ] . Contains ( "/" ) ) array [ 0 ] = Utils . ConvertInputDate ( array [ 7 ] ); } else goahead = false;
                    if ( newrow . field2 != null ) { array [ 1 ] = newrow . field2 . ToString ( ); colcount++; RemoveTrailingChars ( array [ 1 ] ); if ( array [ 1 ] . Contains ( "/" ) ) array [ 1 ] = Utils . ConvertInputDate ( array [ 1 ] ); else goahead = false; }
                    if ( newrow . field3 != null ) { array [ 2 ] = newrow . field3 . ToString ( ); colcount++; RemoveTrailingChars ( array [ 2 ] ); if ( array [ 2 ] . Contains ( "/" ) ) array [ 2 ] = Utils . ConvertInputDate ( array [ 2 ] ); else goahead = false; }
                    if ( newrow . field4 != null ) { array [ 3 ] = newrow . field4 . ToString ( ); colcount++; RemoveTrailingChars ( array [ 3 ] ); if ( array [ 3 ] . Contains ( "/" ) ) array [ 3 ] = Utils . ConvertInputDate ( array [ 3 ] ); else goahead = false; }
                    if ( newrow . field5 != null ) { array [ 4 ] = newrow . field5 . ToString ( ); colcount++; RemoveTrailingChars ( array [ 4 ] ); if ( array [ 4 ] . Contains ( "/" ) ) array [ 4 ] = Utils . ConvertInputDate ( array [ 4 ] ); else goahead = false; }
                    if ( newrow . field6 != null ) { array [ 5 ] = newrow . field6 . ToString ( ); colcount++; RemoveTrailingChars ( array [ 5 ] ); if ( array [ 5 ] . Contains ( "/" ) ) array [ 5 ] = Utils . ConvertInputDate ( array [ 5 ] ); else goahead = false; }
                    if ( newrow . field7 != null ) { array [ 6 ] = newrow . field7 . ToString ( ); colcount++; RemoveTrailingChars ( array [ 6 ] ); if ( array [ 6 ] . Contains ( "/" ) ) array [ 6 ] = Utils . ConvertInputDate ( array [ 6 ] ); else goahead = false; }
                    if ( newrow . field8 != null )
                    {
                        array [ 7 ] = newrow . field8 . ToString ( ); colcount++; RemoveTrailingChars ( array [ 7 ] ); if ( array [ 7 ] . Contains ( "/" ) ) array [ 7 ] = Utils . ConvertInputDate ( array [ 7 ] ); else goahead = false;

                        if ( newrow . field9 != null ) { array [ 8 ] = newrow . field9 . ToString ( ); colcount++; RemoveTrailingChars ( array [ 8 ] ); if ( array [ 8 ] . Contains ( "/" ) ) array [ 8 ] = Utils . ConvertInputDate ( array [ 8 ] ); }
                        if ( newrow . field10 != null ) { array [ 9 ] = newrow . field10 . ToString ( ); colcount++; RemoveTrailingChars ( array [ 9 ] ); if ( array [ 9 ] . Contains ( "/" ) ) array [ 9 ] = Utils . ConvertInputDate ( array [ 9 ] ); }
                        if ( newrow . field11 != null ) { array [ 10 ] = newrow . field11 . ToString ( ); colcount++; RemoveTrailingChars ( array [ 10 ] ); if ( array [ 10 ] . Contains ( "/" ) ) array [ 10 ] = Utils . ConvertInputDate ( array [ 10 ] ); }
                        if ( newrow . field12 != null ) { array [ 11 ] = newrow . field12 . ToString ( ); colcount++; RemoveTrailingChars ( array [ 11 ] ); if ( array [ 11 ] . Contains ( "/" ) ) array [ 11 ] = Utils . ConvertInputDate ( array [ 11 ] ); }
                        if ( newrow . field13 != null ) { array [ 12 ] = newrow . field13 . ToString ( ); colcount++; RemoveTrailingChars ( array [ 12 ] ); if ( array [ 12 ] . Contains ( "/" ) ) array [ 12 ] = Utils . ConvertInputDate ( array [ 12 ] ); }
                        if ( newrow . field14 != null ) { array [ 13 ] = newrow . field14 . ToString ( ); colcount++; RemoveTrailingChars ( array [ 13 ] ); if ( array [ 13 ] . Contains ( "/" ) ) array [ 13 ] = Utils . ConvertInputDate ( array [ 13 ] ); }
                        if ( newrow . field15 != null ) { array [ 14 ] = newrow . field15 . ToString ( ); colcount++; RemoveTrailingChars ( array [ 14 ] ); if ( array [ 14 ] . Contains ( "/" ) ) array [ 14 ] = Utils . ConvertInputDate ( array [ 14 ] ); }
                        if ( newrow . field16 != null ) { array [ 15 ] = newrow . field16 . ToString ( ); colcount++; RemoveTrailingChars ( array [ 15 ] ); if ( array [ 15 ] . Contains ( "/" ) ) array [ 15 ] = Utils . ConvertInputDate ( array [ 15 ] ); }
                        if ( newrow . field17 != null ) { array [ 16 ] = newrow . field17 . ToString ( ); colcount++; RemoveTrailingChars ( array [ 16 ] ); if ( array [ 16 ] . Contains ( "/" ) ) array [ 16 ] = Utils . ConvertInputDate ( array [ 16 ] ); }
                        if ( newrow . field18 != null ) { array [ 17 ] = newrow . field18 . ToString ( ); colcount++; RemoveTrailingChars ( array [ 17 ] ); if ( array [ 17 ] . Contains ( "/" ) ) array [ 17 ] = Utils . ConvertInputDate ( array [ 17 ] ); }
                        if ( newrow . field19 != null ) { array [ 18 ] = newrow . field19 . ToString ( ); colcount++; RemoveTrailingChars ( array [ 18 ] ); if ( array [ 18 ] . Contains ( "/" ) ) array [ 18 ] = Utils . ConvertInputDate ( array [ 18 ] ); }
                        if ( newrow . field20 != null ) { array [ 19 ] = newrow . field20 . ToString ( ); colcount++; RemoveTrailingChars ( array [ 19 ] ); if ( array [ 19 ] . Contains ( "/" ) ) array [ 19 ] = Utils . ConvertInputDate ( array [ 19 ] ); }
                    }


                    string fieldnames = dgControl . GetFullColumnInfo ( SqlTables . SelectedItem . ToString ( ) , DbConnectionString , true , false );
                    string [ ] tmp1 = Output . Split ( "\n" );
                    insertstring = " (";
                    for ( int t = 1 ; t < tmp1 . Length ; t++ )
                    {
                        string [ ] s = tmp1 [ t ] . Split ( ' ' );
                        if ( s [ 0 ] . ToUpper ( ) != "ID" )
                            insertstring += $"{s [ 0 ]}, ";
                    }
                    //********************************************************************//
                    // create the 1st part of the INSERT SQL command (field  names)
                    //*********************************************************************//
                    insertstring = insertstring . Substring ( 0 , insertstring . Trim ( ) . Length - 2 ) + ") ";
                    GenericClass record = new GenericClass ( );
                    colcount = 0;
                    bool alldone = false;

                    //now add the data for this insertion from List<genericClass> 'selectedrecords'
                    // 1st we need each record in a usable format (string[] array)
                    //******************************************************************//
                    // create the 2nd part of the INSERT SQL command (values (....))
                    //*******************************************************************//
                    for ( int q = 0 ; q < selectedrecords . Count ; q++ )
                    {
                        int counter = 1;
                        array = new string [ 20 ];
                        string [ ] str = selectedrecords [ q ];
                        foreach ( var itm in str )
                            if ( itm != null && str [ counter ] != null )
                            {
                                array [ counter - 1 ] = str [ counter ] . ToString ( );
                                colcount = counter++;
                            }
                            else
                            {
                                alldone = true; colcount = counter; break;
                            }
                        if ( alldone ) break;
                    }   //For ()
                }   //  if ( goahead )
                    //                }   // foreach)()
                    //Got  all basic Sql command data by here ... ??

                //**********************************************************************************************//
                //create 2nd part of Insert Command (Value(...)and insert selected records into NewDbTable)
                //**********************************************************************************************//
                int selcounter = dgControl . datagridControl . SelectedItems . Count;
                for ( int recscount = 0 ; recscount < selcounter ; recscount++ )
                {
                    processQuery = "";
                    // finally, create 2nd part of the Insert command (Values(....)) & combine it into full command string 'processQuery '
                    if ( colcount == 19 ) processQuery += $"  {insertstring} VALUES ({array [ 0 ]}, {array [ 1 ]}, {array [ 2 ]}, {array [ 3 ]}, {array [ 4 ]}, {array [ 5 ]}, {array [ 6 ]}, {array [ 7 ]}, {array [ 8 ]}, {array [ 9 ]}, {array [ 10 ]}, {array [ 11 ]}, {array [ 12 ]}, {array [ 13 ]},  {array [ 14 ]}, {array [ 15 ]},  {array [ 16 ]} {array [ 17 ]}, {array [ 18 ]}, {array [ 19 ]} ";
                    else if ( colcount == 18 ) processQuery += $"   {insertstring} VALUES ({array [ 0 ]}, {array [ 1 ]}, {array [ 2 ]}, {array [ 3 ]}, {array [ 4 ]}, {array [ 5 ]}, {array [ 6 ]}, {array [ 7 ]}, {array [ 8 ]}, {array [ 9 ]}, {array [ 10 ]}, {array [ 11 ]}, {array [ 12 ]}, {array [ 13 ]},  {array [ 14 ]}, {array [ 15 ]},  {array [ 16 ]}, {array [ 17 ]}, {array [ 18 ]} ";
                    else if ( colcount == 17 ) processQuery += $"   {insertstring} VALUES ({array [ 0 ]}, {array [ 1 ]}, {array [ 2 ]}, {array [ 3 ]}, {array [ 4 ]}, {array [ 5 ]}, {array [ 6 ]}, {array [ 7 ]}, {array [ 8 ]}, {array [ 9 ]}, {array [ 10 ]}, {array [ 11 ]}, {array [ 12 ]}, {array [ 13 ]},  {array [ 14 ]}, {array [ 15 ]},  {array [ 16 ]}, {array [ 17 ]} ";
                    else if ( colcount == 16 ) processQuery += $"   {insertstring} VALUES ({array [ 0 ]}, {array [ 1 ]}, {array [ 2 ]}, {array [ 3 ]}, {array [ 4 ]}, {array [ 5 ]}, {array [ 6 ]}, {array [ 7 ]}, {array [ 8 ]}, {array [ 9 ]}, {array [ 10 ]}, {array [ 11 ]}, {array [ 12 ]}, {array [ 13 ]},  {array [ 14 ]}, {array [ 15 ]},  {array [ 16 ]}";
                    else if ( colcount == 15 ) processQuery += $"   {insertstring} VALUES ({array [ 0 ]}, {array [ 1 ]}, {array [ 2 ]}, {array [ 3 ]}, {array [ 4 ]}, {array [ 5 ]}, {array [ 6 ]}, {array [ 7 ]}, {array [ 8 ]}, {array [ 9 ]}, {array [ 10 ]}, {array [ 11 ]}, {array [ 12 ]}, {array [ 13 ]},  {array [ 14 ]}, {array [ 15 ]} ";
                    else if ( colcount == 14 ) processQuery += $"   {insertstring} VALUES ({array [ 0 ]}, {array [ 1 ]}, {array [ 2 ]}, {array [ 3 ]}, {array [ 4 ]}, {array [ 5 ]}, {array [ 6 ]}, {array [ 7 ]}, {array [ 8 ]}, {array [ 9 ]}, {array [ 10 ]}, {array [ 11 ]}, {array [ 12 ]}, {array [ 13 ]},  {array [ 14 ]}";
                    else if ( colcount == 13 ) processQuery += $"   {insertstring} VALUES ({array [ 0 ]}, {array [ 1 ]}, {array [ 2 ]}, {array [ 3 ]}, {array [ 4 ]}, {array [ 5 ]}, {array [ 6 ]}, {array [ 7 ]}, {array [ 8 ]}, {array [ 9 ]}, {array [ 10 ]}, {array [ 11 ]}, {array [ 12 ]}, {array [ 13 ]}";
                    else if ( colcount == 12 ) processQuery += $"   {insertstring} VALUES ({array [ 0 ]}, {array [ 1 ]}, {array [ 2 ]}, {array [ 3 ]}, {array [ 4 ]}, {array [ 5 ]}, {array [ 6 ]}, {array [ 7 ]}, {array [ 8 ]}, {array [ 9 ]}, {array [ 10 ]}, {array [ 11 ]}, {array [ 12 ]}";
                    else if ( colcount == 11 ) processQuery += $"   {insertstring} VALUES ({array [ 0 ]}, {array [ 1 ]}, {array [ 2 ]}, {array [ 3 ]}, {array [ 4 ]}, {array [ 5 ]}, {array [ 6 ]}, {array [ 7 ]}, {array [ 8 ]}, {array [ 9 ]}, {array [ 10 ]}, {array [ 11 ]}";
                    else if ( colcount == 10 ) processQuery += $"   {insertstring} VALUES ({array [ 0 ]}, {array [ 1 ]}, {array [ 2 ]}, {array [ 3 ]}, {array [ 4 ]}, {array [ 5 ]}, {array [ 6 ]}, {array [ 7 ]}, {array [ 8 ]}, {array [ 9 ]}, {array [ 10 ]}";
                    else if ( colcount == 9 ) processQuery += $"   {insertstring} VALUES ({array [ 0 ]}, {array [ 1 ]}, {array [ 2 ]}, {array [ 3 ]}, {array [ 4 ]}, {array [ 5 ]}, {array [ 6 ]}, {array [ 7 ]}, {array [ 8 ]}, {array [ 9 ]}";
                    else if ( colcount == 8 ) processQuery += $"   {insertstring} VALUES ({array [ 0 ]}, {array [ 1 ]}, {array [ 2 ]}, {array [ 3 ]}, {array [ 4 ]}, {array [ 5 ]}, {array [ 6 ]}, {array [ 7 ]}, {array [ 8 ]}";
                    else if ( colcount == 7 ) processQuery += $"  {insertstring} VALUES ({array [ 0 ]}, {array [ 1 ]}, {array [ 2 ]}, {array [ 3 ]}, {array [ 4 ]}, {array [ 5 ]}, {array [ 6 ]}, {array [ 7 ]}";
                    else if ( colcount == 6 ) processQuery += $"  {insertstring} VALUES ({array [ 0 ]}, {array [ 1 ]}, {array [ 2 ]}, {array [ 3 ]}, {array [ 4 ]}, {array [ 5 ]}, {array [ 6 ]}";
                    else if ( colcount == 5 ) processQuery += $"  {insertstring} VALUES ({array [ 0 ]}, {array [ 1 ]}, {array [ 2 ]}, {array [ 3 ]}, {array [ 4 ]}, {array [ 5 ]}";
                    else if ( colcount == 4 ) processQuery += $"  {insertstring} VALUES ({array [ 0 ]}, {array [ 1 ]}, {array [ 2 ]}, {array [ 3 ]}, {array [ 4 ]}";
                    else if ( colcount == 3 ) processQuery += $"  {insertstring} VALUES ({array [ 0 ]}, {array [ 1 ]}, {array [ 2 ]}, {array [ 3 ]}";
                    else if ( colcount == 2 ) processQuery += $"  {insertstring} VALUES ({array [ 0 ]}, {array [ 1 ]}, {array [ 2 ]}";
                    else if ( colcount == 1 ) processQuery += $"  {insertstring} VALUES ({array [ 0 ]}, {array [ 1 ]}";
                    else if ( colcount == 0 ) processQuery += $"  {insertstring} VALUES ({array [ 0 ]}";
                    processQuery = processQuery . Trim ( );
                    processQuery = processQuery . Substring ( 0 , processQuery . Length - 1 ) . Trim ( ) + ")";
                    if ( processQuery . Contains ( ",)" ) )
                    {
                        processQuery = processQuery . Trim ( );
                        processQuery = processQuery . Substring ( 0 , processQuery . Length - 2 ) . Trim ( ) + ")";
                    }
                    //Reset continue flag
                    goahead = true;
                    //************************************//
                    // now  perform the actual insertion
                    //************************************//
                    if ( SqlInsertGenericRecord ( processQuery , NewDbName , out err ) == false )
                        Debug . WriteLine ( $"Record insertion FAILED...\n[ {err} ]" );
                    else
                        Debug . WriteLine ( $"Record Inserted SUCCESSFULLY..." );
//                    Utils . ForceUIToUpdate ( );
                    // End of insert loop
                }
            }
            return 1;
        }

        //***************************************************//
        // support method  for  saving selected items onlly
        //***************************************************//
        public int CreateNewTable ( string NewDbName , ObservableCollection<GenericClass> collection , out string err )
        {
            err = "";
            string [ ] args2 = { "" , "" };
            args2 [ 0 ] = $"{NewDbName}";
            string flddata = "";
            try
            {
                foreach ( GenericClass gcrow in collection )
                {
                    string [ ] parts = { "" , "" , "" , "" };
                    flddata += $"{gcrow . field1}";
                    flddata += $" {gcrow . field2}";
                    if ( gcrow . field1 . ToUpper ( ) == "ID" )
                    {
                        flddata += $" IDENTITY(1,1) NOT NULL,";
                        continue;
                    }
                    if ( gcrow . field3 != "" && gcrow . field2 != "int" )
                        flddata += $"({gcrow . field3}";
                    if ( gcrow . field4 != "" && gcrow . field3 != "" && gcrow . field2 != "int" )
                        if ( gcrow . field4 . Contains ( "," ) )
                            flddata += $", {gcrow . field4 . Substring ( 0 , gcrow . field4 . Length - 1 )}) ";
                        else
                            flddata += $", {gcrow . field4}), ";
                    else if ( gcrow . field4 == "" && gcrow . field3 != "" && gcrow . field2 != "int" )
                        flddata += $"), ";
                    else
                        flddata += $", ";

                    args2 [ 1 ] = $"{flddata}, ";
                    if ( args2 [ 1 ] . Contains ( ", ," ) ) args2 [ 1 ] = args2 [ 1 ] . Substring ( 0 , args2 [ 1 ] . Length - 4 );
                    else args2 [ 1 ] = args2 [ 1 ] . Substring ( 0 , args2 [ 1 ] . Length - 1 );
                }
                // now working 24/9/2022
                //**********************************//
                // now Create new table structure
                //**********************************//
                int result = dgControl . ProcessUniversalStoredProcedure ( "CreateTableTest" , args2 , out err );
                if ( err != "" && err . ToUpper ( ) . Contains ( "PROBLEM-FILE  EXISTS" ) == false )
                {
                    Debug . WriteLine ( $"Table creation failed [{err}]" );
                    MessageBoxResult result2 = MessageBox . Show ( "A table with the selected name already exists in the database.\nDo you want to overwrite it with the newly selected data ?" , "" ,
                        MessageBoxButton . YesNoCancel , MessageBoxImage . Question , MessageBoxResult . No );
                    if ( result2 == MessageBoxResult . Yes )
                    {
                        string [ ] args3 = { "" };
                        err = "";
                        args3 [ 0 ] = NewDbName;
                        result = dgControl . ProcessUniversalStoredProcedure ( "spDropTable" , args3 , out err );
                        if ( err . ToUpper ( ) . Contains ( "PROBLEM-FILE  EXISTS" ) == false )
                        {
                            // ????????????????
                        }
                        if ( result2 == MessageBoxResult . No )
                            return -9;
                        if ( result2 == MessageBoxResult . Cancel )
                            return -9;
                        CreateNewTable ( NewDbName , collection , out err );
                    }
                    else
                        return -9;
                }
                else { statusbar . Text = $"New table named [{NewDbName . ToUpper ( )}] was created succesfully...."; Debug . WriteLine ( $"{statusbar . Text}" ); }
            }
            catch ( Exception ex )
            {
                Debug . WriteLine ( $"{ex . Message}" );
            }
            return -1;
        }
        // saveselected support methods
        public bool SqlInsertGenericRecord ( string processQuery , string NewDbName , out string err )
        {
            err = "";
            // Insert  a single record into SQL table 'NewDbName'
            Dictionary<string , string> dict = new Dictionary<string , string> ( );
            string err2 = "";

            Debug . WriteLine ( $"Inserting new record containing\n[ {processQuery} ]" );
            SqlConnection sqlCon = null;
            string Con = DbConnectionString;
            using ( sqlCon = new SqlConnection ( Con ) )
            {
                string cmd = "";
                sqlCon . Open ( );
                // Now add argument(s)  to SQL command
                var parameters = new DynamicParameters ( );
                parameters . Add ( $"Arg1" , $"{NewDbName}" ,
               DbType . String ,
               ParameterDirection . Input ,
               NewDbName . Length );
                parameters . Add ( $"Arg2" , $"{processQuery}" ,
               DbType . String ,
               ParameterDirection . Input ,
               processQuery . Length );

                try
                {
                    int res3 = sqlCon . Execute ( "spInsertGenericRecord" , parameters , commandType: CommandType . StoredProcedure );
                    Debug . WriteLine ( $"RESULT of  [{processQuery}] = {res3} (Success)" );
                }
                catch ( Exception ex )
                {
                    // Debug . WriteLine ( $"Record Insertion failed = {ex . Message}, {ex . Data}" );
                    err = $"Record Insertion failed = {ex . Message}, {ex . Data}";
                    return false;
                }
                return true;
            }
        }
        public bool PerformSingleRecInsert ( string processQuery , Dictionary<string , string> dict , out string err )
        {
            string Con = DbConnectionString;
            int index = 0;
            Mouse . OverrideCursor = Cursors . Wait;
            SqlConnection sqlCon = null;
            Debug . WriteLine ( $"Running Stored Procedure {processQuery}" );
            using ( sqlCon = new SqlConnection ( Con ) )
            {
                string cmd = "";
                sqlCon . Open ( );
                // Now add argument(s)  to SQL command
                var parameters = new DynamicParameters ( );
                foreach ( var key in dict )
                    if ( key . Key . Length > 0 && key . Key != "-" )
                    {
                        string fldname = key . Key . ToString ( );
                        parameters . Add ( $"Arg[{index++}]," , $"{fldname}" ,
                       DbType . String ,
                       ParameterDirection . Input ,
                       fldname . Length );
                    }
                err = "";
                try
                {
                    int result = sqlCon . Execute ( processQuery , parameters , commandType: CommandType . Text );
                    Debug . WriteLine ( $"RESULT of  [{processQuery}] = {result} (Success)" );
                }
                catch ( Exception ex )
                {
                    Debug . WriteLine ( $"{ex . Message}, {ex . Data}" );
                }
            }
            return true;
        }
        public int CreateNewSqlTableStructure ( string NewDbName , ObservableCollection<GenericClass> collection , out string err )
        {
            bool GetFullDetails = true;
            string Output = "";
            err = "";
            if ( GetFullDetails )
            {
                collection . Clear ( );

                //*****************************//
                // Get full column info with sizes
                //*****************************//
                Output = dgControl . GetFullColumnInfo ( SqlTables . SelectedItem . ToString ( ) , DbConnectionString , false , false );

                string [ ] args = Output . Split ( '\n' );
                List<string> FldNames = new List<string> ( );
                string buffer = "";
                try
                {
                    //**********************************************//
                    // create new collection of all structure info
                    //**********************************************//
                    foreach ( string item2 in args )
                        if ( item2 != null && item2 . Trim ( ) != "" )
                        {
                            string [ ] RawFldNames = item2 . Split ( ' ' );
                            string [ ] flds2 = { "" , "" , "" , "" };
                            int y = 0;
                            for ( int x = 0 ; x < RawFldNames . Length ; x++ )
                                if ( RawFldNames [ x ] . Length > 0 )
                                    flds2 [ y++ ] = RawFldNames [ x ];
                            buffer = flds2 [ 0 ];
                            if ( buffer != null && buffer . Trim ( ) != "" )
                            {
                                FldNames . Add ( buffer . ToUpper ( ) );
                                GenericClass tem = new GenericClass ( );
                                tem . field1 = buffer . ToUpper ( );    // fname
                                tem . field2 = flds2 [ 1 ];   //ftype
                                tem . field4 = flds2 [ 3 ];   // decroot
                                tem . field3 = flds2 [ 2 ];   // decpart
                                collection . Add ( tem );
                            }
                        }
                }
                catch ( Exception ex ) { Debug . WriteLine ( $"{ex . Message}" ); }
            }

            //**************************************************************************************//
            // collection is obscollection<GenericClass> contains true field names + types + sizes
            // so lets create table structure here !
            //**************************************************************************************//
            int res = CreateNewTable ( NewDbName , collection , out err );
            return res;  // success
        }

        public int CreateSqlInsertCommand ( string [ ] args )
        {
            return -1;  // default success
        }


        public string RemoveTrailingChars ( string processQuery )
        {
            if ( processQuery . Trim ( ) . Contains ( "}," ) )
            {
                processQuery = Utils . ReverseString ( processQuery );
                processQuery = processQuery . Substring ( 2 );
                processQuery = Utils . ReverseString ( processQuery );
            }
            return processQuery;
        }

        private void SaveSelected_MouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
        {
            //            MyGrid . Visibility = Visibility . Visible;
        }

        private void Border_MouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
        {
            //         MyGrid . Visibility = Visibility . Visible;
        }

        private void Option_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {

        }


    
    }
}

