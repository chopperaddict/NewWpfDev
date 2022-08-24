using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . Data;
using System . Data . SqlClient;
using System . Diagnostics;
using System . Linq;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;

using DapperGenericsLib;

using DocumentFormat . OpenXml . InkML;
using DocumentFormat . OpenXml . Office2010 . Drawing;

using NewWpfDev;
using NewWpfDev . UserControls;

using ServiceStack;

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
        public string CurrentTable = "";

        string DbConnectionString = "Data Source=DINO-PC;Initial Catalog=\"IAN1\";Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
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

        private void SqlTables_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {
            string selection = e . AddedItems [ 0 ] . ToString ( );
            // call User Control to load the selected table from the current Sql Db
            GridData = dgControl . LoadGenericData ( $"{selection}" , ShowColumnHeaders , DbConnectionString );
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
            if ( cb . IsChecked == true )
                ShowColumnHeaders = true;
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
            dgControl.GetFullColumnInfo ( CurrentTable, DbConnectionString );
        }

         private void Close_Click ( object sender , RoutedEventArgs e )
        {
            this . Close ( );
        }

        #endregion local control support
        //**********************************//
    }
}
