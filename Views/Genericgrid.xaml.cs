using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . Data;
using System . Diagnostics;
using System . IO;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Documents;
using System . Windows . Input;
using System . Windows . Media;

using DapperGenericsLib;

//using DocumentFormat . OpenXml . Drawing . Charts;

using NewWpfDev;
using NewWpfDev . Views;

using UserControls;

using Canvas = System . Windows . Controls . Canvas;
using GenericClass = NewWpfDev . GenericClass;

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
        public static ObservableCollection<GenericClass> GridData = new ObservableCollection<GenericClass>();
        public static ObservableCollection<GenericClass> ColumnsData = new ObservableCollection<GenericClass>();
        //        public static FlowDocument myFlowDocument = new FlowDocument();
        static public DatagridControl dgControl;
        static public Genericgrid GenControl;
        public DataGrid Dgrid;
        public int ColumnsCount = 0;
        public bool bStartup = true;
        public bool ShowColumnHeaders = false;
        public List<int> SelectedRows = new List<int>();
        //This is Updated by my Grid Control whenever it loads a different table
        // NB : Must be declared as shown, including it's name;
        public static string CurrentTableDomain = "IAN1";
        public static string CurrentTable = "";
        public static string NewSelectedTableName = "";
        public bool InfoViewerShown = false;
        public static bool USERRTBOX = true;

        public static string DbConnectionString = "Data Source=DINO-PC;Initial Catalog=\"IAN1\";Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";


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

        #region Dependency properties
        public bool ListReloading
        {
            get
            {
                return ( bool )GetValue(ListReloadingProperty);
            }
            set
            {
                SetValue(ListReloadingProperty , value);
            }
        }
        public static readonly DependencyProperty ListReloadingProperty =
            DependencyProperty . Register("ListReloading" , typeof(bool) , typeof(Genericgrid) , new PropertyMetadata(false));
        public string infotext
        {
            get
            {
                return ( string )GetValue(infotextProperty);
            }
            set
            {
                SetValue(infotextProperty , value);
            }
        }
        public static readonly DependencyProperty infotextProperty =
            DependencyProperty . Register("infotext" , typeof(string) , typeof(Genericgrid) , new PropertyMetadata("Information text goes here ...."));
        public string Searchtext
        {
            get
            {
                return ( string )GetValue(SearchtextProperty);
            }
            set
            {
                SetValue(SearchtextProperty , value);
            }
        }
        public static readonly DependencyProperty SearchtextProperty =
            DependencyProperty . Register("Searchtext" , typeof(string) , typeof(Genericgrid) , new PropertyMetadata(""));

        //-----------------------------------------------------------//
        #endregion Dependency properties
        //-----------------------------------------------------------//

        public Genericgrid ()
        {
            InitializeComponent();
            this . DataContext = this;
            GenericGridSupport . SetPointers(null , this);
            GenControl = this;
            Mouse . OverrideCursor = Cursors . Wait;
            CurrentTable = "BankAccount";
            dgControl = this . GenGridCtrl;
            Dgrid = dgControl . datagridControl;
            Task . Run(() => LoadDbTables("BankAccount"));
            GenericGridSupport . SelectCurrentTable("BankAccount");
            ToggleColumnHeaders . IsChecked = ShowColumnHeaders;
            ColumnsCount = Dgrid . Columns . Count;
            bStartup = false;
            DatagridControl . SetParent(( Control )this);
            Flags . UseScrollView = false;
            Mouse . OverrideCursor = Cursors . Arrow;
            // TODO  TEMP ON:Y
            // Dummy entry in save field
            NewTableName . Text = "qwerty";
            //          LoadRTbox();
            OptionsList . SelectedIndex = 0;

            //Flowdoc . ExecuteFlowDocMaxmizeMethod += new EventHandler(MaximizeFlowDoc);
            //Flowdoc . ExecuteFlowDocMaxmizeMethod -= new EventHandler(MaximizeFlowDoc);
            //FlowDoc . FlowDocClosed += Flowdoc_FlowDocClosed;

            string [ ] args = { "" , "" };
            args [ 0 ] = "Customer";
            args [ 1 ] = "@OUTPUT OUTPUT";
            //            int exist = dgControl . ProcessUniversalStoredProcedure ( "getColumnscount" , args , out string err );
            List<string> list = new List<string>();
            //            list = DatagridControl . GetSqlData<ObservableCollection<GenericClass>>(ColumnsData , "use ian1; Select * from schema");
        }


        private void LoadRTbox ()
        {
            //if ( USERRTBOX == false )
            //{
            FlowDocument myFlowDocument = new FlowDocument();
            infotext = File . ReadAllText(@$"C:\users\ianch\documents\GenericGridInfo.Txt");
            myFlowDocument = CreateFlowDocumentScroll(infotext , "Black0");
            RTBox . Document = myFlowDocument;
            //}
        }

        #region local control support
        //**********************************//
        private void SqlTables_SelectionChanged (object sender , SelectionChangedEventArgs e)
        {
            if ( ListReloading == false && e . AddedItems . Count > 0 )
            {
                //string err = "";
                string selection = e . AddedItems [ 0 ] . ToString();
                string [ ] args = { "" };
                args [ 0 ] = selection;
                int exist = dgControl . ProcessUniversalStoredProcedure("spCheckTableExists" , args , out string err);
                if ( exist == -1 && err == "" )
                {
                    var result = dgControl . LoadGenericData($"{selection}" , ShowColumnHeaders , DbConnectionString);
                    GridData = result;
                    Reccount . Text = GridData . Count . ToString();
                    if ( result . Count > 0 )
                        statusbar . Text = $"The data for {selection . ToUpper()} was loaded successfully  and is shown above...";
                    ResetColumnHeaderToTrueNames(selection , dgControl . datagridControl);
                    CurrentTable = selection;
                }
                else
                {
                    MessageBox . Show("It appears that the selected Table no longer exists.\nThe Tables list will now be reloaded for you to try this option again." , "Combo Box Error");
                    LoadDbTables(CurrentTable);
                    GenericGridSupport . SelectCurrentTable(CurrentTable);
                }
            }
        }
        public void ResetColumnHeaderToTrueNames (string CurrentTable , DataGrid Grid)
        {
            //Update Column headers to original column names, so we need to create dummy list just to call Replace headers method
            int colcount = dgControl . datagridControl . Columns . Count;
            List<DapperGenericsLib . DataGridLayout> dglayoutlist = new List<DapperGenericsLib . DataGridLayout>();
            DapperLibSupport . ReplaceDataGridFldNames(CurrentTable , ref Grid , ref dglayoutlist , colcount);
        }

        public async Task<bool> LoadDbTables (string currentTable)
        {   //task to load list of Db Tables
            List<string> TablesList = GetDbTablesList("IAN1");
            Application . Current . Dispatcher . BeginInvoke(() =>
            {
                SqlTables . ItemsSource = null;
                SqlTables . Items . Clear();
                SqlTables . ItemsSource = TablesList;
                int index = 0;
                if ( currentTable != "" )
                {
                    currentTable = currentTable . ToUpper();
                    foreach ( string item in SqlTables . Items )
                    {
                        if ( currentTable == item . ToUpper() )
                        {
                            SqlTables . SelectedIndex = index;
                            break;
                        }
                        index++;
                    }
                }
            });
            return true;
        }
        public List<string> GetDbTablesList (string DbName)
        {
            List<string> TablesList = new List<string>();
            string SqlCommand = "";
            List<string> list = new List<string>();
            DbName = DbName . ToUpper();
            if ( DapperLibSupport . CheckResetDbConnection(DbName , out string constr) == false )
            {
                Debug . WriteLine($"Failed to set connection string for {DbName} Db");
                return TablesList;
            }
            //// All Db's have their own version of this SP.....
            SqlCommand = "spGetTablesList";

            TablesList = GenericGridSupport . CallStoredProcedure(list , SqlCommand);
            //This call returns us a DataTable
            //DataTable dt = dgControl . GetDataTable ( SqlCommand );
            //// This how to access Row data from  a grid the easiest way.... parsed into a List <xxxxx>
            //if ( dt != null )
            //    TablesList =  GetDataDridRowsAsListOfStrings ( dt );
            //// return list of all current SQL tables in current Db
            return TablesList;
        }
        public List<string> CallStoredProcedure (List<string> list , string sqlcommand)
        {
            //This call returns us a DataTable
            DataTable dt = dgControl . GetDataTable(sqlcommand);
            if ( dt != null )
                //				list = GenericDbHandlers.GetDataDridRowsWithSizes ( dt );
                list = GenericGridSupport . GetDataDridRowsAsListOfStrings(dt);
            return list;
        }
        public static List<string> GetDataDridRowsAsListOfStrings (DataTable dt)
        {
            List<string> list = new List<string>();
            foreach ( DataRow row in dt . Rows )
            {
                var txt = row . Field<string>(0);
                list . Add(txt);
            }
            return list;
        }

        private void ToggleColumnHeaders_Checked (object sender , RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if ( bStartup == false && cb . IsChecked == true )
                ShowColumnHeaders = true;
            else
                ShowColumnHeaders = false;

            int colcount = dgControl . datagridControl . Columns . Count;
            dgControl . ShowTrueColumns(dgControl . datagridControl , CurrentTable , colcount , ShowColumnHeaders);
            ShowColumnHeaders = false;
        }
        private void ToggleColumnHeaders_Click (object sender , RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if ( cb . IsChecked == true )
            {
                ShowColumnHeaders = true;
                int colcount = dgControl . datagridControl . Columns . Count;
                dgControl . ShowTrueColumns(dgControl . datagridControl , CurrentTable , colcount , ShowColumnHeaders);
            }
            else
            {
                ShowColumnHeaders = false;
                dgControl . SetDefColumnHeaderText(dgControl . datagridControl , false);
            }
        }
        private void ShowColumnInfo_Click (object sender , RoutedEventArgs e)
        {
            string input = "";
            string errormsg = "";
            string output = "";
            int columncount = 0;
            dgControl . GetFullColumnInfo(CurrentTable , CurrentTable , DbConnectionString);
            return;
        }
        private void Close_Click (object sender , RoutedEventArgs e)
        {
            Close();
        }
        private void NewTableName_GotKeyboardFocus (object sender , KeyboardFocusChangedEventArgs e)
        {
            if ( NewTableName . Text . Contains("Enter New Table Name ") )
                NewTableName . Text = "";
        }

        //-----------------------------------------------------------//
        #endregion local control support
        //-----------------------------------------------------------//
        private void Grid_Loaded (object sender , RoutedEventArgs e)
        {
            List<DataGridLayout> dglayoutlist = new List<DataGridLayout>();
            List<Dictionary<string , string>> ColumntypesList = new List<Dictionary<string , string>>();
            File . Delete(@"c:\users\ianch\documents\CW.log");
            $"calling LoadDbAsGenericData" . CW();
            string error = "";
            GridData = DatagridControl . LoadDbAsGenericData("spGetTableColumnWithSize" , GridData ,
               ref ColumntypesList , CurrentTable , "IAN1" , ref dglayoutlist , ref error , true);
            if ( error != "" )
                Debug . WriteLine($"Data Load failed : [ {error} ]");
            ToggleColumnHeaders . IsChecked = true;
            GridData = dgControl . LoadGenericData(CurrentTable , true , DbConnectionString);
            Reccount . Text = GridData . Count . ToString();
            statusbar . Text = $"The data for {CurrentTable . ToUpper()} was loaded successfully  and is shown above...";
        }
        //**********************************//
        #region local control support
        //**********************************//
        private async void asyncSqlTables_SelectionChanged (object sender , SelectionChangedEventArgs e)
        {
            string selection = e . AddedItems [ 0 ] . ToString();
            // call User Control to load the selected table from the current Sql Db
            // TODO   NOT WORKING 3/10/2022
            var result = await dgControl . LoadData($"{selection}" , ShowColumnHeaders , DbConnectionString);
        }
        private void IsLoaded (object sender , RoutedEventArgs e)
        {
            ToggleColumnHeaders . IsChecked = true;
        }
        //-----------------------------------------------------------//
        #endregion local control support
        //-----------------------------------------------------------//

        //**********************************************//
        #region Select columns for new table support
        //**********************************************//
        public int CreateNewTableAsync (object sender , RoutedEventArgs e)
        {

            SaveAsNewTable(sender , e);
            return 1;
            bool UseSelectedColumns = false;
            List<string> FldNames = new List<string>();
            ObservableCollection<GenericClass> collection = new ObservableCollection<GenericClass>();
            string NewDbName = NewTableName . Text . Trim();
            if ( NewDbName == "Enter New Table Name ...." )
            {
                MessageBox . Show("Please provide a name for the new table in the field provided..." , "New Table name required");
                NewTableName . Focus();
                return -1;
            }
            if ( NewDbName == "" )
            {
                MessageBox . Show("Please enter a suitable name for the table you want to create !" , "Naming Error");
                return -1;
            }
            CurrentTable = NewDbName;
            MessageBoxResult mbresult = MessageBox . Show("If you want to select only certain columns from the current table to be saved, Click YES, else click No" , "Data Formatting ?" ,
                MessageBoxButton . YesNoCancel ,
                MessageBoxImage . Question ,
                MessageBoxResult . No);

            //int x = 0;
            if ( mbresult == MessageBoxResult . Cancel )
                return -1;
            if ( mbresult == MessageBoxResult . Yes )
            {
                // Save a set with only user selected columns
                string [ ] args = new string [ 20 ];
                UseSelectedColumns = true;
                string Output = dgControl . GetFullColumnInfo(CurrentTable , CurrentTable , DbConnectionString , false);
                string buffer = "";
                int index = 0;
                args = Output . Split('\n');
                foreach ( var item in args )
                {
                    if ( item != null && item . Trim() != "" )
                    {
                        string [ ] RawFldNames = item . Split(' ');
                        string [ ] flds = { "" , "" , "" , "" };
                        int y = 0;
                        for ( int x = 0 ; x < RawFldNames . Length ; x++ )
                        {
                            if ( RawFldNames [ x ] . Length > 0 )
                                flds [ y++ ] = ( RawFldNames [ x ] );
                        }
                        buffer = flds [ 0 ];
                        if ( buffer != null && buffer . Trim() != "" )
                        {
                            FldNames . Add(buffer . ToUpper());
                            GenericClass tem = new GenericClass();
                            tem . field1 = buffer . ToUpper();    // fname
                            tem . field2 = flds [ 1 ];   //ftype
                            tem . field4 = flds [ 3 ];   // decroot
                            tem . field3 = flds [ 2 ];   // decpart
                            collection . Add(tem);
                        }
                    }
                }
                //ALL WORKING  20/9/2022 - We now have a list of all Column names with
                //column type & size data, so let user choose what to save to a new table!
                SelectedRows . Clear();
                // load selection dialog with available clumns
                ColNames . ItemsSource = collection;
                // Show dialog
                FieldSelectionGrid . Visibility = Visibility . Visible;
            }
            else
            {
                // just  do a direct copy
                string [ ] args = { $"{SqlTables . SelectedItem . ToString()}" , $"{NewDbName}" , "" , "" };
                dgControl . ProcessUniversalStoredProcedure("spCopyDb" , args , out string err);
                // make deep copy of table else it gets cleared elsewhere
                // Create a completely new instance via seriazable Clone method stored in NewWpfDev.Utils (in ObjectCopier class file)
                ObservableCollection<GenericClass> deepcopy = new ObservableCollection<GenericClass>();
                string originalname = $"{SqlTables . SelectedItem . ToString()}";
                deepcopy = NewWpfDev . Utils . CopyCollection(GridData , deepcopy);
                GridData = deepcopy;
                string [ ] args1 = { $"{NewDbName}" };
                int colcount = dgControl . datagridControl . Columns . Count;
                DatagridControl . LoadActiveRowsOnlyInGrid(dgControl . datagridControl , GridData , colcount);
                List<DapperGenericsLib . DataGridLayout> dglayoutlist = new List<DapperGenericsLib . DataGridLayout>();
                DapperLibSupport . ReplaceDataGridFldNames(NewDbName , ref dgControl . datagridControl , ref dglayoutlist , colcount);
                LoadDbTables(NewDbName);
                GenericGridSupport . SelectCurrentTable(NewDbName);

                if ( dgControl . datagridControl . Items . Count > 0 )
                {
                    statusbar . Text = $"New Table [{NewDbName}] Created successfully, {dgControl . datagridControl . Items . Count} records inserted & Table is now shown in datagrid above....";
                    DapperGenericsLib . Utils . DoErrorBeep(400 , 100 , 1);
                    DapperGenericsLib . Utils . DoErrorBeep(300 , 400 , 1);
                }
                else
                {
                    statusbar . Text = $"New Table [{NewDbName}] could NOT be Created. Error was [{err}] ";
                    DapperGenericsLib . Utils . DoErrorBeep(320 , 100 , 1);
                    DapperGenericsLib . Utils . DoErrorBeep(260 , 200 , 1);
                }
                NewTableName . Text = NewDbName;

            }
            Mouse . OverrideCursor = Cursors . Arrow;
            return 1;
        }
        private void ColNames_SelectionChanged (object sender , SelectionChangedEventArgs e)
        {
            // Working  successfully 19/9/22
            DataGrid grid = sender as DataGrid;
            int index = -1;
            bool ismatched = false;
            // Get Index of selected row in datagrid
            if ( grid . CurrentItem != null )
                index = grid . Items . IndexOf(grid . CurrentItem);

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
                List<int> temp = new List<int>();
                foreach ( var item in SelectedRows )
                {
                    if ( item != index )
                        temp . Add(item);
                }
                // copy list back to original
                SelectedRows = temp;
            }
            if ( ismatched == false )
                SelectedRows . Add(index);
        }
        private void GoBtn_Click (object sender , RoutedEventArgs e)
        {
            // Creating new table based on selected columns
            GenericClass selColumns = new GenericClass();
            ObservableCollection<GenericClass> collection = new ObservableCollection<GenericClass>();
            List<GenericToRealStructure> grsList = new List<GenericToRealStructure>();
            DataGrid grid = ColNames;
            int selindex = 0;
            // Create list of selected items only
            List<int> columnsindex = new List<int>();
            foreach ( GenericClass item in ColNames . SelectedItems )
            {
                GenericToRealStructure grs = new GenericToRealStructure();
                grs . colindex = SelectedRows [ selindex++ ];
                grs . fname = item . field1;
                grs . ftype = item . field2;
                if ( item . field3 != "" )
                    grs . decroot = Convert . ToInt32(item . field3);
                if ( item . field4 != null && item . field4 != "" )
                {
                    if ( item . field4 . Contains(",") )
                    {
                        int indx = 0;
                        for ( int entry = 0 ; entry < item . field4 . Length ; entry++ )
                        {
                            if ( item . field4 [ entry ] == ',' )
                            {
                                item . field4 = item . field4 . Substring(0 , entry);
                                break;
                            }
                        }
                        grs . decpart = Convert . ToInt32(item . field4);
                    }
                }
                grsList . Add(grs);
                Debug . WriteLine($"GrsList DATA : {grs . fname}, {grs . ftype}, {grs . decroot}, {grs . decpart}, {grs . colindex}");
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
            CreateAsyncTable(CurrentTable , grsList , out err);
            Mouse . OverrideCursor = Cursors . Arrow;
            e . Handled = true;
        }
        public int CreateAsyncTable (string NewDbName , List<GenericToRealStructure> TableStruct , out string err)
        {
            int x = 0;
            dgControl = GenGridCtrl;
            string error = "";
            err = "";
            // Assign new collection to special collection for now
            ColumnsData = dgControl . CreateLimitedTableAsync(NewDbName , TableStruct , out error);
            if ( error != "" ) err = error;
            // We should now have all the data in our new columns only table
            if ( GridData == null )
                return -1;
            if ( x == -2 )
                NewTableName . Focus();
            if ( x == -3 )
                statusbar . Text = $"New Table creation failed, see error log file for more infomation....";
            if ( GridData != null )
            {
                // reload list of tables so new one is shown as well
                LoadDbTables(NewDbName);
                // select new table in dropdown list only
                GenericGridSupport . SelectCurrentTable(NewDbName);
                // clear display grid contents
                dgControl . datagridControl . ItemsSource = null;
                if ( ColumnsData . Count > 0 )
                {
                    //Load new columns only data into datarid
                    DatagridControl . LoadActiveRowsOnlyInGrid(dgControl . datagridControl , ColumnsData , TableStruct . Count);
                }
                else
                {
                    // Load ALL records into datagrid
                    DatagridControl . LoadActiveRowsOnlyInGrid(dgControl . datagridControl , GridData , TableStruct . Count);
                }
                //Update Column headers to original column names, so we need to create dummy list just to call Replace headers method
                List<DapperGenericsLib . DataGridLayout> dglayoutlist = new List<DapperGenericsLib . DataGridLayout>();
                DapperLibSupport . ReplaceDataGridFldNames(NewDbName , ref dgControl . datagridControl , ref dglayoutlist , TableStruct . Count);

                // make deep copy of table else it gets cleared elsewhere
                // Create a completely new instance via seriazable Clone method stored in NewWpfDev.Utils (in ObjectCopier class file)
                ObservableCollection<GenericClass> deepcopy = new ObservableCollection<GenericClass>();
                deepcopy = NewWpfDev . Utils . CopyCollection(ColumnsData , deepcopy);
                GridData = deepcopy;

                if ( dgControl . datagridControl . Items . Count > 0 )
                {
                    statusbar . Text = $"New Table [{NewDbName}] Created successfully, {dgControl . datagridControl . Items . Count} records inserted & Table is now shown in datagrid above....";
                    DapperGenericsLib . Utils . DoErrorBeep(400 , 100 , 1);
                    DapperGenericsLib . Utils . DoErrorBeep(300 , 400 , 1);
                }
                else
                {
                    statusbar . Text = $"New Table [{NewDbName}] could NOT be Created. Error was [{err}] ";
                    DapperGenericsLib . Utils . DoErrorBeep(320 , 100 , 1);
                    DapperGenericsLib . Utils . DoErrorBeep(260 , 200 , 1);
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

        //-----------------------------------------------------------//
        #endregion Select columns for new table support
        //-----------------------------------------------------------//

        public List<string> GetDataTableAsList ()
        {
            List<string> TablesList = new List<string>();
            List<string> list = new List<string>();
            string SqlCommand = "spGetTablesList";
            //// All Db's have their own version of this SP.....
            GenericGridSupport . CallStoredProcedure(list , SqlCommand);
            //This call returns us a DataTable
            DataTable dt = dgControl . GetDataTable(SqlCommand);
            // This how to access Row data from  a grid the easiest way.... parsed into a List <xxxxx>
            if ( dt != null )
            {
                TablesList = GenericGridSupport . GetDataDridRowsAsListOfStrings(dt);
            }
            return TablesList;
        }

        private void statusbar_KeyDown (object sender , KeyEventArgs e)
        {
            if ( e . Key == Key . Enter )
                CreateNewTableAsync(sender , e);
        }
        private void Button_MouseEnter (object sender , MouseEventArgs e)
        {
            Button btn = sender as Button;
            LinearGradientBrush brsh = FindResource("Black2OrangeSlant") as LinearGradientBrush;
            btn . Background = brsh;
            btn . UpdateLayout();
            //btn . Refresh ( );
        }
        private void CloseBtn_GotFocus (object sender , RoutedEventArgs e)
        {
            Button btn = sender as Button;
            LinearGradientBrush brsh = FindResource("Black2OrangeSlant") as LinearGradientBrush;
            btn . Background = brsh;
            btn . UpdateLayout();
        }
        private void CloseBtn_IsMouseDirectlyOverChanged (object sender , DependencyPropertyChangedEventArgs e)
        {
            Button btn = sender as Button;
            LinearGradientBrush brsh = FindResource("Black2OrangeSlant") as LinearGradientBrush;
            btn . Background = brsh;
            btn . UpdateLayout();
        }
        private void CreateNewTable (object sender , RoutedEventArgs e)
        {
            CreateNewTableAsync(sender , e);
        }
        private void stopBtn_Click (object sender , RoutedEventArgs e)
        {
            // Aborting creation of new table based on selected columns
            FieldSelectionGrid . Visibility = Visibility . Collapsed;
        }
        private void ShowInfo (object sender , RoutedEventArgs e)
        {
            // Load boilerplate text describing ths control
            infotext = File . ReadAllText(@$"C:\users\ianch\documents\GenericGridInfo.Txt");
            FlowDocument myFlowDocument = new FlowDocument();
            myFlowDocument . Blocks . Clear();
            myFlowDocument = CreateFlowDocumentScroll(infotext , "Black0");
            RTBox . Document = myFlowDocument;
            RTBox . Visibility = Visibility . Visible;
            InfoGrid . Visibility = Visibility . Visible;
            InfoViewerShown = true;
        }
        private void InfoGrid_KeyDown (object sender , KeyEventArgs e)
        {
            if ( e . Key == Key . Escape )
            {
                InfoGrid . Visibility = Visibility . Collapsed;
                dgControl . datagridControl . Visibility = Visibility . Visible;
                InfoViewerShown = false;
                if ( SpStringsSelector . Visibility == Visibility . Visible )
                    ProcNames . Focus();
                //myFlowDocument . Blocks . Clear();
            }
            // finally add this  to your Form/Window/UserControl keyboard handler method so ESCworks to close Flowdoc   
            //if ( e . Key == Key . Escape )
            //{
            //    if ( Flowdoc != null )
            //    {
            //        Flowdoc . Visibility = Visibility . Collapsed;
            //        Filtercanvas . Visibility = Visibility . Collapsed;
            //    }
            //}
        }
        private void RTBox_MouseRightButtonDown (object sender , MouseButtonEventArgs e)
        {

        }
        private void SaveSelectedOnly (object sender , RoutedEventArgs e)
        {
            int recsinserted = 0;
            NewSelectedTableName = NewTableName . Text;
            int retval = GenericGridSupport . ProcessSelectedRows(out recsinserted , Genericgrid . CurrentTable , false);
            if ( retval == -9 )
            {
                // error - handle it here 
                MessageBoxResult res2 = MessageBox . Show($"It appears that the data saving process to {NewSelectedTableName . ToUpper()} FAILED \nso the current process has been cancelled !\n\nThe new Table still exists, do you want to delete it now" , "Fatal problem encountered" ,
                     MessageBoxButton . YesNo , MessageBoxImage . Question , MessageBoxResult . No);
                if ( res2 == MessageBoxResult . Yes )
                {
                    string err = "";
                    int retv = GenericGridSupport . DropTable(NewSelectedTableName , CurrentTable , out err);
                    if ( err != "" )
                    {
                        Debug . WriteLine($"{err}");
                    }
                    else
                        Debug . WriteLine($"{NewSelectedTableName} does not exist. so no action has occurred");
                }
                statusbar . Text = $"It appears that the data save to a new table {NewSelectedTableName . ToUpper()} FAILED \nand therefore this process has been cancelled !";
            }
            else if ( retval >= -2 )
                statusbar . Text = $"A total of {recsinserted} items have been used to create a new table {NewSelectedTableName . ToUpper()} in the \'.IAN1\' Database successfully";
        }

        static public string RemoveTrailingChars (string processQuery)
        {
            if ( processQuery . Trim() . Contains("},") )
            {
                processQuery = NewWpfDev . Utils . ReverseString(processQuery);
                processQuery = processQuery . Substring(2);
                processQuery = NewWpfDev . Utils . ReverseString(processQuery);
            }
            return processQuery;
        }
        private void OptionsList_Selected (object sender , SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            string selection = e . OriginalSource . ToString();
            if ( cb . SelectedIndex == 1 )
                ShowColumnInfo_Click(sender , e);
            else if ( cb . SelectedIndex == 2 )
                ReloadTables(sender , e);
            else if ( cb . SelectedIndex == 3 )
                SaveAsNewTable(sender , e);
            else if ( cb . SelectedIndex == 4 )
                ShowFilter_Click(sender , e);
            else if ( cb . SelectedIndex == 5 )
                SaveSelectedOnly(sender , e);
            else if ( cb . SelectedIndex == 6 )
                SearchStoredProc(sender , e);
            //Reset it to top (non active option so we can select any valid option next time
            cb . SelectedIndex = 0;
        }
        private void SaveAsNewTable (object sender , RoutedEventArgs e)
        {
            int result = GenericGridSupport . SaveAsNewTable();
        }
        private void ReloadTables (object sender , RoutedEventArgs e)
        {
            if ( CurrentTable == "" )
                CurrentTable = SqlTables . SelectedItem . ToString();
            LoadDbTables(CurrentTable);
            SetValue(ListReloadingProperty , true);
            GenericGridSupport . SelectCurrentTable(CurrentTable);
            statusbar . Text = "List of Tables reloaded successfully...";
            SetValue(ListReloadingProperty , false);
        }

        private void Gengrid_MouseDoubleClick (object sender , MouseButtonEventArgs e)
        {
            var obj = e . OriginalSource;
            if ( InfoViewerShown == true && InfoGrid . Visibility == Visibility . Collapsed )
            {
                InfoGrid . Visibility = Visibility . Collapsed;
                dgControl . datagridControl . Visibility = Visibility . Visible;

                InfoViewerShown = false;
            }
            else if ( InfoViewerShown == true && InfoGrid . Visibility == Visibility . Visible )
            {
                InfoGrid . Visibility = Visibility . Collapsed;
                dgControl . datagridControl . Visibility = Visibility . Visible;
                InfoViewerShown = false;
            }
            else if ( InfoViewerShown == false && InfoGrid . Visibility == Visibility . Visible )
            {
                InfoGrid . Visibility = Visibility . Visible;
                InfoViewerShown = true;
            }
            else if ( InfoViewerShown == false && InfoGrid . Visibility == Visibility . Collapsed )
            {
                InfoGrid . Visibility = Visibility . Collapsed;
                dgControl . datagridControl . Visibility = Visibility . Visible;
                InfoViewerShown = true;
            }
        }

        //****************************************************//
        #region FILTERING SUPPORT incl movinng it around
        //****************************************************//

        public object MovingObject;
        public Thickness GenTh = new(50.0 , 10.0 , 0 , 0);
        public Thickness th = new();
        public double currentleft = 0.0;
        public double currenttop = 0.0;
        public double FirstXPos = 0.0;
        public double FirstYPos = 0.0;
        public Type type;
        public double width = 0.0, height = 0.0, top = 0.0, left = 0.0;

        private void ShowFilter_Click (object sender , RoutedEventArgs e)
        {
            // Working 3/10/22
            Filtering . Visibility = Visibility . Visible;
            Filtering . UpdateLayout();
            filtertext . Text = "";
            filtertext . Focus();
            type = sender . GetType();
            Debug . WriteLine($"Show Type is {type}");
            width = Filtering . ActualWidth;
            height = Filtering . ActualHeight;
            Debug . WriteLine($"Show - left {left} top {top}");
            MovingObject = Filtering;
            if ( MovingObject != null )
            {
                ( MovingObject as FrameworkElement ) . SetValue(Canvas . LeftProperty , 100.0);
                ( MovingObject as FrameworkElement ) . SetValue(Canvas . TopProperty , 50.0);
            }
        }
        private void Filtering_PreviewMouseLeftButtonDown (object sender , MouseButtonEventArgs e)
        {
            // Working 3/10/22
            type = sender . GetType();
            Grid filtergrid = sender as Grid;
            MovingObject = Filtering;
            FirstXPos = e . GetPosition(filtergrid) . X;
            FirstYPos = e . GetPosition(filtergrid) . Y;
            left = e . GetPosition(( MovingObject as FrameworkElement ) . Parent as FrameworkElement) . X - FirstXPos;
            top = e . GetPosition(( MovingObject as FrameworkElement ) . Parent as FrameworkElement) . Y - FirstYPos;
        }
        private void Filtering_MouseMove (object sender , MouseEventArgs e)
        {
            // Working 3/10/22
            Grid filtergrid = sender as Grid;
            if ( MovingObject != null && filtergrid != null && e . LeftButton == MouseButtonState . Pressed )
            {
                type = sender . GetType();
                height = filtergrid . ActualHeight;
                width = filtergrid . ActualWidth;
                left = e . GetPosition(( MovingObject as FrameworkElement ) . Parent as FrameworkElement) . X - FirstXPos;
                top = e . GetPosition(( MovingObject as FrameworkElement ) . Parent as FrameworkElement) . Y - FirstYPos;
                //Debug . WriteLine($"Move Filtering [{Filtering . Name}] , {top} / {left}, Ht {height}, Wd {width}");
                //Debug . WriteLine($"New Pos top {Convert . ToInt32(top)} / {Convert . ToInt32(left)}");
                if ( left > 5 ) //&& left < ( Filtercanvas . Width - left ) ) 
                    ( MovingObject as FrameworkElement ) . SetValue(Canvas . LeftProperty , left);
                if ( top > 0 )//&& top < (Filtercanvas.Height - top ))
                    ( MovingObject as FrameworkElement ) . SetValue(Canvas . TopProperty , top);
                Debug . WriteLine($"Y top {Convert . ToDouble(top)} / {Convert . ToDouble(left)}");
            }
        }
        private void Filtering_KeyDown (object sender , KeyEventArgs e)
        {
            if ( e . Key == Key . Enter )
                Filter_Click(sender , null);
            if ( e . Key == Key . Escape )
                Filtering . Visibility = Visibility . Collapsed;
        }
        private void Filtering_MouseLeftButtonUp (object sender , MouseButtonEventArgs e)
        {
            // Working 3/10/22
            //Filtering . ReleaseMouseCapture();
        }
        private void FieldSelectionGrid_LostFocus (object sender , RoutedEventArgs e)
        {
            //Filtering . ReleaseMouseCapture();
        }
        private void closeFilter (object sender , RoutedEventArgs e)
        {
            Filtering . Visibility = Visibility . Collapsed;
            GenGridCtrl . Visibility = Visibility . Visible;
            //Filtering . ReleaseMouseCapture();
        }
        //****************************************************//
        private void Filter_Click (object sender , RoutedEventArgs e)
        {
            Filtering . Visibility = Visibility . Visible;
        }
        //-----------------------------------------------------------//
        #endregion FILTERING SUPPORT incl moving it around
        //-----------------------------------------------------------//

        //****************************************************//
        #region Search SP's for specific text
        //****************************************************//

        private void SpStringsSelection_KeyDown (object sender , KeyEventArgs e)
        {
            if ( e . Key == Key . Enter )
                GoBtn1_Click(null , null);
            else if ( e . Key == Key . Escape )
                SpStringsSelection . Visibility = Visibility . Collapsed;
        }

        private void stopBtn2_Click (object sender , RoutedEventArgs e)
        {
            // close small dialog wih Text Entry field
            SpStringsSelector . Visibility = Visibility . Collapsed;
            selectedSp . Focus();
        }

        private void GoBtn2_Click (object sender , RoutedEventArgs e)
        {
            // load  SP's and show in Flowdoc
            DataTable dt = new DataTable();
            string sprocedure = ProcNames . SelectedItem . ToString();
            Mouse . OverrideCursor = Cursors . Wait;
            dt = DatagridControl . ProcessSqlCommand($"spGetSpecificSchema  {sprocedure}" , Flags . CurrentConnectionString);
            List<string> list = new List<string>();
            foreach ( DataRow row in dt . Rows )
            {
                list . Add(row . Field<string>(0));
            }
            // now display  thefull content of the seleted S.P
            infotext = list [ 0 ];

            FlowDocument myFlowDocument = new FlowDocument();
            myFlowDocument = CreateBoldString(myFlowDocument , list [ 0 ] , Searchtext);
            myFlowDocument . Background = FindResource("Black3") as SolidColorBrush;
            RTBox . Document = myFlowDocument;
            RTBox . Background = FindResource("Black3") as SolidColorBrush;
            RTBox . Foreground = FindResource("White0") as SolidColorBrush;
            RTBox . VerticalScrollBarVisibility = ScrollBarVisibility . Visible;
            RTBox . HorizontalScrollBarVisibility = ScrollBarVisibility . Visible;
            RTBox . Visibility = Visibility . Visible;
            InfoGrid . Visibility = Visibility . Visible;
            dgControl . datagridControl . Visibility = Visibility . Collapsed;
            Mouse . OverrideCursor = Cursors . Arrow;
        }
        private FlowDocument CreateFlowDocumentScroll (string line1 , string clr1 , string line2 = "" , string clr2 = "" , string line3 = "" , string clr3 = "" , string header = "" , string clr4 = "")
        {
            FlowDocument myFlowDocument = new FlowDocument();
            //NORMAL
            Paragraph para1 = new Paragraph();
            // This is  the only paragraph that uses the user defined Font Size....
            para1 . FontSize = 14;
            para1 . FontFamily = new FontFamily("Arial");
            if ( USERRTBOX )
                para1 . Foreground = FindResource("White0") as SolidColorBrush;
            else
                para1 . Foreground = FindResource("Black0") as SolidColorBrush;
            Thickness th = new Thickness();
            th . Top = 10;
            th . Left = 10;
            th . Right = 10;
            th . Bottom = 10;
            para1 . Padding = th;
            para1 . Inlines . Add(new Run(line1));
            //Add paragraph to flowdocument
            myFlowDocument . Blocks . Clear();
            myFlowDocument . Blocks . Add(para1);
            return myFlowDocument;
        }


        public FlowDocument CreateBoldString (FlowDocument myFlowDocument , string SpText , string SrchTerm)
        {
            string original = SpText;
            string originalSearchterm = "";
            original = NewWpfDev . Utils . CopyCollection(SpText , original);
            string input = SpText . ToUpper();
            string [ ] NonSearchText;
            List<int> NonSearchTextlength = new List<int>();
            List<string> NonCapitlisedString = new List<string>();
            originalSearchterm = SrchTerm;
            int newpos = 0;
            SrchTerm = SrchTerm . ToUpper();

            // split source down based on searchterm (using non capitalised version
            // // Only searchterm is capitalised !!!!))
            NonSearchText = input . Split($"{SrchTerm}");
            foreach ( var item in NonSearchText )
            {
                NonSearchTextlength . Add(item . Length);
            }
            for ( int x = 0 ; x < NonSearchTextlength . Count ; x++ )
            {
                string temp = original . Substring(newpos , NonSearchTextlength [ x ]);
                NonCapitlisedString . Add(temp);
                newpos += NonSearchTextlength [ x ] + SrchTerm . Length;
            }
            // Now create a (formatted) list of lines from all  paragraphs identified previously
            Paragraph para = new Paragraph();

            for ( int x = 0 ; x < NonCapitlisedString . Count ; x++ )
            {
                Run run1 = AddStdNewDocumentParagraph(NonCapitlisedString [ x ] , SrchTerm);
                para . Inlines . Add(run1);
                Run run2 = AddDecoratedNewDocumentParagraph(NonCapitlisedString [ x ] , SrchTerm);

                if ( x < NonCapitlisedString . Count- 1 )
                    para . Inlines . Add(run2);
            }
            // build  document by adding all blocks to Document
            myFlowDocument . Blocks . Add(para);
            return myFlowDocument;
        }
        // NOT USED
        public static FlowDocument ProcessRTBParagraph (FlowDocument document , string SearchTerm)
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

        public Run AddStdNewDocumentParagraph (string textstring , string SearchText)
        {
            // Main text
            Run run1 = new Run(textstring);
            run1 . FontSize = 16;
            run1 . FontFamily = new FontFamily("Arial");
            run1 . FontWeight = FontWeights . Normal;
            run1 . Background = FindResource("Black4") as SolidColorBrush;
            run1 . Foreground = FindResource("White0") as SolidColorBrush;
            return run1;
        }
        public Run AddDecoratedNewDocumentParagraph (string textstring , string SearchText)
        {
            Run run2 = new Run(SearchText);
            run2 . FontSize = 18;
            run2 . FontFamily = new FontFamily("Arial");
            run2 . FontWeight = FontWeights . Bold;
            run2 . Foreground = FindResource("Green5") as SolidColorBrush;
            run2 . Background = FindResource("Black4") as SolidColorBrush;
            return run2;
        }

        private void GoBtn1_Click (object sender , RoutedEventArgs e)
        {
            DataTable dt = new DataTable();
            string SqlCommand = $"spFindTextInSProc '{selectedSp . Text}'";
            Searchtext = selectedSp . Text;
            Mouse . OverrideCursor = Cursors . Wait;
            dt = DatagridControl . ProcessSqlCommand(SqlCommand , Flags . CurrentConnectionString);
            List<string> list = GetDataDridRowsAsListOfStrings(dt);
            ProcNames . ItemsSource = null;
            ProcNames . Items . Clear();
            if ( list . Count > 0 )
            {
                SpStringsSelector . Visibility = Visibility . Visible;
                foreach ( var item in list )
                {
                    ProcNames . Items . Add(item as string);
                }
                ProcNames . SelectedIndex = 0;
                // show sp listbox dialog
                SpStringsSelector . Visibility = Visibility . Visible;
                ProcNames . Focus();
                if(Autoclose.IsChecked == true)
                    SpStringsSelection.Visibility = Visibility . Collapsed;
            }
            else
                MessageBox . Show($"Sorry, it does not appear that the search term [ '{Searchtext . ToUpper()}' ]\nhas been found in any of the Stored Procedures in the current SQL Server\n\nPlease enter a search item likely to be found in a Stored Procedure. " , "Stored Procedure Search System");
            Mouse . OverrideCursor = Cursors . Arrow;
        }

        private void ProcNames_MouseDoubleClick (object sender , MouseButtonEventArgs e)
        {
            GoBtn2_Click(null , null);
            e . Handled = true;
        }

        private void SpStrings_KeyDown (object sender , KeyEventArgs e)
        {
            if ( e . Key == Key . Enter )
                GoBtn2_Click(null , null);
            if ( e . Key == Key . Escape )
            {
                SpStringsSelector . Visibility = Visibility . Collapsed;
                selectedSp . Focus();
            }
        }

        private void InfoGrid_IsVisibleChanged (object sender , DependencyPropertyChangedEventArgs e)
        {
            Grid grid = sender as Grid;
            Visibility visibility = new Visibility();
        }

        private void CloseIcon_MouseLeftButtonDown (object sender , MouseButtonEventArgs e)
        {
            InfoGrid . Visibility = Visibility . Collapsed;
            dgControl . datagridControl . Visibility = Visibility . Visible;
        }

        private void MenuItem_Click (object sender , RoutedEventArgs e)
        {
            InfoGrid . Visibility = Visibility . Collapsed;
        }

        private void SearchStoredProc (object sender , RoutedEventArgs e)
        {
            SpStringsSelection . Visibility = Visibility . Visible;
            selectedSp . Focus();
            selectedSp . SelectAll();
        }

        private void stopBtn1_Click (object sender , RoutedEventArgs e)
        {
            SpStringsSelection . Visibility = Visibility . Collapsed;
        }

        private void Button_KeyDown (object sender , KeyEventArgs e)
        {
            if ( e . Key == Key . Escape )
                Filtering . Visibility = Visibility . Collapsed;
        }

        private void stopBtn1_KeyDown (object sender , KeyEventArgs e)
        {
            if ( e . Key == Key . Escape )
                SpStringsSelection . Visibility = Visibility . Collapsed;
        }

        //-----------------------------------------------------------//
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

        // EOF  EOF  EOF  EOF  EOF  EOF  EOF  EOF  EOF  EOF  EOF  EOF  EOF  EOF  EOF  EOF  
    }
}
