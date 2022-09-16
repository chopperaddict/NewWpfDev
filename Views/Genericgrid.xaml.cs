using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . Data;
using System . Data . SqlClient;
using System . Diagnostics;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;

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
            Mouse . SetCursor ( Cursors . Arrow );
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
        private async void SqlTables_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {
            string selection = e . AddedItems [ 0 ] . ToString ( );
            //var result = dgControl . LoadData ( $"{selection}" , ShowColumnHeaders , DbConnectionString );
            var result = dgControl.LoadGenericData ( $"{selection}" , ShowColumnHeaders , DbConnectionString );
            //            Task  res 
            //                = await Task.Run ( () => dgControl . LoadData ( $"{selection}" , ShowColumnHeaders , DbConnectionString ) );
            /////           GridData = dgControl . LoadGenericData ( $"{selection}" , ShowColumnHeaders , DbConnectionString );
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
            DataTable dt = GetDataTable ( SqlCommand );
            // This how to access Row data from  a grid the easiest way.... parsed into a List <xxxxx>
            if ( dt != null )
            {
                TablesList = GetDataDridRowsAsListOfStrings ( dt );
            }
            return TablesList;
        }
        public List<string> CallStoredProcedure ( List<string> list , string sqlcommand )
        {
            //This call returns us a DataTable
            DataTable dt = GetDataTable ( sqlcommand );
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

        public DataTable GetDataTable ( string commandline )
        {
            DataTable dt = new DataTable ( );
            try
            {
                SqlConnection con;
                string ConString = DbConnectionString;
                if ( ConString == "" )
                {
                    // GenericDbUtilities<GenericClass> . CheckDbDomain ( "IAN1" );
                    ConString = DbConnectionString;
                }
                con = new SqlConnection ( ConString );
                using ( con )
                {
                    SqlCommand cmd = new SqlCommand ( commandline , con );
                    SqlDataAdapter sda = new SqlDataAdapter ( cmd );
                    sda . Fill ( dt );
                }
            }
            catch ( Exception ex )
            {
                Debug . WriteLine ( $"Failed to load Db - {ex . Message}, {ex . Data}" );
                return null;
            }
            return dt;
        }

        private void ToggleColumnHeaders_Checked ( object sender , RoutedEventArgs e )
        {
            CheckBox cb = sender as CheckBox;
            if ( bStartup== false  && cb . IsChecked == true )
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

            List<GenericToRealStructure> TableStructure = dgControl . CreateFullColumnInfo ( DatagridControl . CurrentTable , DbConnectionString );
            // We now have a full SQl Structure for the current table in TableStructure
            string NewDbName = NewTableName . Text . Trim ( ); 
            if( NewDbName == "")
            {
                MessageBox . Show ( "Please enter a suitable name for the table you want to create !","Naming Error");
                return;
            }
            // Sort out  our  new table structure
            string [ ] Sqlargs;
            string commandline = dgControl.CreateSqlCommand ( TableStructure , NewDbName , out Sqlargs);
            // Now we have got a fully formatted SqlCommand and the necessary arguments using the special CreateGenericDbStoredProcedure S.P.
            if(dgControl . CreateGenericDbStoredProcedure ( $"spCREATENEWDBTABLE {NewDbName} " , Sqlargs , DbConnectionString , out string err ) == 1)
            {
                //Table creates successfuilly, so Copy data to new table
                foreach ( var item in GenGridCtrl. datagridControl.Items )
                {


                }

            }
          }
      private void Close_Click ( object sender , RoutedEventArgs e )
        {
            this . Close ( );
        }

        private void IsLoaded ( object sender , RoutedEventArgs e )
        {
            ToggleColumnHeaders . IsChecked = true;

        }

        #endregion local control support
        //**********************************//
    }
}
