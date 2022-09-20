using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . Data;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Controls . Primitives;
using System . Windows . Documents . DocumentStructures;
using System . Windows . Input;
using System . Windows . Media;

//using Dapper;
//using DapperGenericsLib;
//using NewWpfDev;
//using NewWpfDev . ViewModels;
using NewWpfDev . Views;

//using ServiceStack;

using UserControls;

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
        public static ObservableCollection<GenericClass> GridData = new ObservableCollection<GenericClass> ( );
        public DatagridControl dgControl;
        public DataGrid Dgrid;
        public int ColumnsCount = 0;
        public bool bStartup = true;
        public bool ShowColumnHeaders = false;
        public List<int> SelectedRows = new List<int> ( );
        //This is Updated by my Grid Control whenever it loads a different table
        // NB : Must be declared as shown, including it's name;
        public static string CurrentTable = "";

        public static string DbConnectionString = "Data Source=DINO-PC;Initial Catalog=\"IAN1\";Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public Genericgrid ( )
        {
            InitializeComponent ( );
            Mouse . SetCursor ( Cursors . Wait );
            CurrentTable = "BankAccount";
            dgControl = this . GenGridCtrl;
            Dgrid = dgControl . datagridControl;
            Task . Run ( ( ) => LoadDbTables ( "BankAccount" ) );
            SelectCurrentTable ( "BankAccount" );
            ToggleColumnHeaders . IsChecked = ShowColumnHeaders;
            ColumnsCount = Dgrid . Columns . Count;
            bStartup = false;
            DatagridControl . SetParent ( ( Control ) this );
            Flags . UseScrollView = false;
            Mouse . SetCursor ( Cursors . Arrow );
            // TODO  TEMP ON:Y
            NewTableName . Text = "qwerty";
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
        private async void SqlTables_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {
            string selection = e . AddedItems [ 0 ] . ToString ( );
            var result = dgControl . LoadGenericData ( $"{selection}" , ShowColumnHeaders , DbConnectionString );
            GridData = result;
        }
        private async void asyncSqlTables_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {
            string selection = e . AddedItems [ 0 ] . ToString ( );


            // call User Control to load the selected table from the current Sql Db
            var result = await dgControl . LoadData ( $"{selection}" , ShowColumnHeaders , DbConnectionString );

            //var result  = await Task.Run( () =>dgControl . LoadGenericData ( $"{selection}" , ShowColumnHeaders , DbConnectionString ));
            //            var result = dgControl . LoadData ( $"{selection}" , ShowColumnHeaders , DbConnectionString );
        }

        private async Task<bool> LoadDbTables ( string currentTable )
        {   //task to load list of Db Tables
            List<String> TablesList = GetDbTablesList ( "IAN1" );
            Application . Current . Dispatcher . BeginInvoke ( ( ) =>
            {
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
            //if ( CheckResetDbConnection ( DbName , out string constr ) == false )
            //{
            //    Debug . WriteLine ( $"Failed to set connection string for {DbName} Db" );
            //    return TablesList;
            //}
            //// All Db's have their own version of this SP.....
            SqlCommand = "spGetTablesList";

            CallStoredProcedure ( list , SqlCommand );
            //This call returns us a DataTable
            DataTable dt = dgControl . GetDataTable ( SqlCommand );
            // This how to access Row data from  a grid the easiest way.... parsed into a List <xxxxx>
            if ( dt != null )
            {
                TablesList = GetDataDridRowsAsListOfStrings ( dt );
            }
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

        //public DataTable GetDataTable ( string commandline )
        //{
        //    DataTable dt = new DataTable ( );
        //    try
        //    {
        //        SqlConnection con;
        //        string ConString = DbConnectionString;
        //        if ( ConString == "" )
        //        {
        //             //GenericDbUtilities<GenericClass> . CheckDbDomain ( "IAN1" );
        //            ConString = DbConnectionString;
        //        }
        //        con = new SqlConnection ( ConString );
        //        using ( con )
        //        {
        //            SqlCommand cmd = new SqlCommand ( commandline , con );
        //            SqlDataAdapter sda = new SqlDataAdapter ( cmd );
        //            sda . Fill ( dt );
        //        }
        //    }
        //    catch ( Exception ex )
        //    {
        //        Debug . WriteLine ( $"Failed to load Db - {ex . Message}, {ex . Data}" );
        //        return null;
        //    }
        //    return dt;
        //}

        private void ToggleColumnHeaders_Checked ( object sender , RoutedEventArgs e )
        {
            CheckBox cb = sender as CheckBox;
            if ( bStartup == false && cb . IsChecked == true )
                ShowColumnHeaders = true;
            else
                ShowColumnHeaders = false;

            int colcount = dgControl . datagridControl . Columns . Count;
            dgControl . ShowTrueColumns ( dgControl . datagridControl , CurrentTable , colcount , ShowColumnHeaders );
        }

        private void ToggleColumnHeaders_Click ( object sender , RoutedEventArgs e )
        {
            CheckBox cb = sender as CheckBox;
            if ( cb . IsChecked == true )
            {
                ShowColumnHeaders = true;
                int colcount = dgControl . datagridControl . Columns . Count;
                dgControl . ShowTrueColumns ( dgControl . datagridControl , CurrentTable , colcount , ShowColumnHeaders );
            }
            else
            {
                ShowColumnHeaders = false;
                dgControl . SetDefColumnHeaderText ( dgControl . datagridControl , false );
            }
        }

        private void ShowColumnInfo_Click ( object sender , RoutedEventArgs e )
        {
            String input = "";
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
            this . Close ( );
        }

        private void IsLoaded ( object sender , RoutedEventArgs e )
        {
            ToggleColumnHeaders . IsChecked = true;

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
        private int CreateNewTableAsync ( object sender , RoutedEventArgs e )
        {
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
                MessageBoxButton . YesNo ,
                MessageBoxImage . Question ,
                MessageBoxResult . No );

            //int x = 0;
            if ( mbresult == MessageBoxResult . Yes )
            {
                string [ ] args = new string [ 20 ];
                UseSelectedColumns = true;
                string Output = dgControl . GetFullColumnInfo ( DatagridControl . CurrentTable , DbConnectionString , false );
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
                    {
                        //Create collection of items
                            collection = new ObservableCollection<GenericClass> ( );
                        foreach ( string col in FldNames )
                        {
                            GenericClass tem = new GenericClass ( );
                            tem . field1 = col;
                            collection . Add ( tem );
                        }
                        ColNames . ItemsSource = collection;
                        // We now have a list of all Column names, so let user choose what to save to a new table!
                    }
                }
                    //SelectedRows . Clear ( );
                    //                    return -1;
   
                       FieldSelectionGrid . Visibility = Visibility . Visible;
            }
            else
            {

                      List<GenericToRealStructure> TableStruct = new List<GenericToRealStructure> ( );
                    if ( TableStruct.Count== 0 )
                        TableStruct = dgControl . CreateFullColumnInfo ( DatagridControl . CurrentTable , DbConnectionString );


                string [ ] args = new string [ 20 ];
                UseSelectedColumns = true;
                string Output = dgControl . GetFullColumnInfo ( DatagridControl . CurrentTable , DbConnectionString , false );
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
                CreateAsyncTable ( NewDbName , TableStruct );


                //GenericClass selColumns = new GenericClass ( );
                //List<GenericToRealStructure> grsList = new List<GenericToRealStructure> ( );
                //DataGrid grid = ColNames;
                //int selindex = 0;
                //int rowindex = 0;
                //bool startup = true;
                //// Create list of  items 
                //List<int> columnsindex = new List<int> ( );
                //foreach ( GenericClass item in GridData )
                //{
                //    GenericToRealStructure grs = new GenericToRealStructure ( );
                //    grs . colindex = selindex++;
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
                //    if ( startup )
                //    { grsList . Add ( grs ); startup = false; }
                //}
                //        CreateAsyncTable ( NewDbName , null );

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
            // Creating new table based on selected columns
            GenericClass selColumns = new GenericClass ( );
            ObservableCollection<GenericClass> collection = new ObservableCollection<GenericClass> ( );
            List<GenericToRealStructure> grsList = new List<GenericToRealStructure> ( );
            DataGrid grid = ColNames;
            int selindex = 0;
//            int rowindex = 0;
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
            }
            //    //    GenericClass tem = new GenericClass ( );
            //    //    tem = item;
            //    //    collection . Add ( tem );
            //    //}
            //    // Assign this collection to selection datagrid and show it
            //    ColNames . ItemsSource = collection;
            //FieldSelectionGrid . Visibility = Visibility . Visible;

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
            CreateAsyncTable ( CurrentTable , grsList );
        }
        public async Task<int> CreateAsyncTable ( string NewDbName , List<GenericToRealStructure> TableStruct )
        {
            int x = 0;
            dgControl = GenGridCtrl;
            int result = dgControl . CreateLimitedTableAsync ( NewDbName , TableStruct );
            x = Convert . ToInt32 ( result );
            if ( x == -1 )
                return -1;
            if ( x == -2 )
                NewTableName . Focus ( );
            if ( x == -3 )
                statusbar . Content = $"New Table creation failed, see error log file for more infomation....";
            if ( x == 1 )
            {
                await Task . Run ( async ( ) => LoadDbTables ( NewDbName ) );
                SelectCurrentTable ( NewDbName );
                statusbar . Content = $"New Table [{NewDbName}] Created successfully, data copied & {NewDbName} is now displayed in datagrid above....";
                DapperGenericsLib . Utils . DoErrorBeep ( 380 , 100 , 1 );
                DapperGenericsLib . Utils . DoErrorBeep ( 340 , 100 , 1 );
                NewTableName . Text = NewDbName;
            }
            // clear temporay grid data
            ColNames . ItemsSource = null;
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
            {
                TablesList = GetDataDridRowsAsListOfStrings ( dt );
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

        private void CreateNewTable ( object sender , RoutedEventArgs e )
        {
            CreateNewTableAsync ( sender , e );
        }


        private void stopBtn_Click ( object sender , RoutedEventArgs e )
        {
            // Aborting creation of new table based on selected columns
            FieldSelectionGrid . Visibility = Visibility . Collapsed;
        }


    }
}
