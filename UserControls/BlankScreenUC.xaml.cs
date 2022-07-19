using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . Diagnostics;
using System . DirectoryServices . ActiveDirectory;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Data;
using System . Windows . Documents;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Media . Imaging;
using System . Windows . Navigation;
using System . Windows . Shapes;

using DocumentFormat . OpenXml . Presentation;

using Microsoft . VisualBasic;

using NewWpfDev . Dapper;
using NewWpfDev . Models;
using NewWpfDev . Sql;
using NewWpfDev . SQL;
using NewWpfDev . ViewModels;
using NewWpfDev . Views;

namespace NewWpfDev . UserControls {

    public partial class BlankScreenUC : UserControl {
        public static BankAcHost Host { get; set; }
        public static ObservableCollection<string> Comboitems = new ObservableCollection<string> ( );
        //public static ObservableCollection<GenericClass> GenCollection = new ObservableCollection<GenericClass> ( );
        //public static GenericClass GenClass = new GenericClass ( );
        public string col1 = "wwwwwwwwwwwwww", col2 = "oooooooooooo", col3 = "zzzzzzzzzzzzzzzz";
//        public List<string> TablesList = new List<string> ( );
//        public string SelectedTable { get; set; }
        public DataGrid gengrid { get; set; }

        #region DP's

        public Brush background {
            get { return ( Brush ) GetValue ( backgroundProperty ); }
            set { SetValue ( backgroundProperty , value ); }
        }
        public static readonly DependencyProperty backgroundProperty =
            DependencyProperty . Register ( "background" , typeof ( Brush ) , typeof ( BlankScreenUC ) , new PropertyMetadata ( ( Brush ) Brushes . Cyan ) );

        #endregion DP's

        public BlankScreenUC ( ) {
            InitializeComponent ( );
            DataContext = this;
            this . Background = Brushes . Cyan;
//            combo . DataContext = combo;
//            GenClass = new GenericClass ( );

            // local grid
            //var v = new { col1 = Comboitems [ 0 ] , col2 = Comboitems [ 1 ] , col3 = Comboitems [ 2 ] };
            //stdgrid . Items . Add ( v );
            //v = new { col1 = Comboitems [ 1 ] , col2 = Comboitems [ 2 ] , col3 = Comboitems [ 3 ] };
            //stdgrid . Items . Add ( v );
            //v = new { col1 = Comboitems [ 2 ] , col2 = Comboitems [ 3 ] , col3 = Comboitems [ 4 ] };
            //stdgrid . Items . Add ( v );

            // Get list of Tables from SQL source
            //GenericGridControl . GetDbTablesList ( "IAN1" , out TablesList );
            //combo . ItemsList = TablesList;

            //// subscribe to Events
            //ComboboxPlus . ComboboxChanged += ComboboxPlus_ComboboxChanged;

            ////Load default Db
            //int DbCount = LoadTableGeneric ( "Select * from BankAccount" , ref GenCollection );
            //if ( DbCount > 0 ) {
            //    SqlServerCommands . LoadActiveRowsOnlyInGrid ( dgrid2 . datagrid1 , GenCollection , SqlServerCommands . GetGenericColumnCount ( GenCollection ) );
            //    Debug . WriteLine ( $"grid has {dgrid2 . datagrid1 . Items . Count} items" );
            //    GenericGridControl . GetDbTablesList ( "IAN1" , out TablesList );
//                combo . ItemsList = TablesList;
   
        }

 
        //private void ComboboxPlus_ComboboxChanged ( object sender , ComboChangedArgs e ) {
        //    SelectedTable = e . Itemselected . ToString ( );
        //    int DbCount = LoadTableGeneric ( $"Select * from {SelectedTable}" , ref GenCollection );
        //    if ( DbCount > 0 ) {
        //        dgrid2 . datagrid1 . ItemsSource = null;
        //        dgrid2 . datagrid1 . Items . Clear ( );
        //        //Dictionary<string , string> dict = new Dictionary<string , string> ( );
        //        //dict = GetColumnNames ( SelectedTable , "IAN1" );
        //        SqlServerCommands . LoadActiveRowsOnlyInGrid ( dgrid2 . datagrid1 , GenCollection , SqlServerCommands . GetGenericColumnCount ( GenCollection ) );
        //        Debug . WriteLine ( $"grid has {dgrid2 . datagrid1 . Items . Count} items" );

        //        GenericDbUtilities . ReplaceDataGridFldNames ( SelectedTable , ref dgrid2 . datagrid1 );
        //        dgrid2 . datagrid1 . Refresh ( );
        //    }
        //}

        //public Dictionary<string , string> GetColumnNames ( string tablename , string domain = "IAN1" ) {
        //    int indx = 0;
        //    List<string> list = new List<string> ( );
        //    ObservableCollection<GenericClass> GenericClass = new ObservableCollection<GenericClass> ( );
        //    Dictionary<string , string> dict = new Dictionary<string , string> ( );
        //    // This returns a Dictionary<sting,string> PLUS a collection  and a List<string> passed by ref....
        //    List<int> VarCharLength = new List<int> ( );
        //    //			IsBankActive = ( bool ) obj;
        //    //if ( IsBankActive == false )
        //    //    dict = GenericDbUtilities . GetDbTableColumns ( ref GenericClass , ref list , "Customer" , "IAN1" , ref VarCharLength );
        //    //else
        //    dict = GenericDbUtilities . GetDbTableColumns ( ref GenericClass , ref list , tablename , domain , ref VarCharLength );

        //    indx = 0;
        //    if ( VarCharLength . Count > 0 ) {
        //        foreach ( var item in GenericClass ) {
        //            item . field3 = VarCharLength [ indx++ ] . ToString ( );
        //        }
        //    }
        //    return dict;
        //    //if ( ParentBGView != null ) {
        //    //    SqlServerCommands . LoadActiveRowsOnlyInGrid ( ParentBGView . dataGrid2 , GenericClass , DapperSupport . GetGenericColumnCount ( GenericClass ) );
        //    //    indx = 0;
        //    //    foreach ( var col in ParentBGView . dataGrid2 . Columns ) {
        //    //        if ( indx == 0 )
        //    //            col . Header = "Field Name";
        //    //        else if ( indx == 1 )
        //    //            col . Header = "SQL Field Type";
        //    //        else if ( indx == 2 ) {
        //    //            col . Header = "NVarChar Length";
        //    //        }
        //    //        indx++;
        //    //    }
        //    //}
        //    //if ( VarCharLength . Count > 0 ) {
        //    //    string output = "";
        //    //    indx = 0;
        //    //    foreach ( var item in GenericClass ) {
        //    //        item . field3 = VarCharLength [ indx++ ] . ToString ( );
        //    //        output += item . field1 . ToString ( ) + ", " + item . field2 . ToString ( ) + ", " + item . field3 + "\n";
        //    //    }
        //    //    //fdmsg ( output , "" , "" );
        //}

        public static void SetHost ( BankAcHost host ) {
            Host = host;
        }
        private void SelectDetails ( object sender , RoutedEventArgs e ) {
            // send name of panel requested (Grid)
            Host . ClosePanel ( this , "BANKACCOUNTGRID" );
        }

        private void SelectAccounts ( object sender , RoutedEventArgs e ) {
            // send name of panel requested (Info)
            Host . ClosePanel ( this , "BANKACCOUNTLIST" );
        }
        public void ResizeControl ( double height , double width ) {
            this . Height = height;
            this . Width = width;
            //if( (width / 2 - ( image . Width ) / 1.5 ) < 0)
            //    image . Width = (width/2 - ( image . Width ) / 1.5);
            //image . Height= width/2 - ( image . Height) / 1.5;
            //Thickness th = new Thickness ( );
            //th = image . Margin;
            //th . Right = 220;
            //th . Top = 100;
            //image . Margin = th;
            //this . Refresh ( );
        }


        private void UpdateBankRecord ( object sender , RoutedEventArgs e ) {

        }

        private void dgrid_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e ) {
            GenericGridControl . GenericGrid . RowHeight = 25;
            GenericGridControl . GenericGrid . SelectedIndex = 2;
            GenericGridControl . GenericGrid . SelectedItem = 2;
            Debug . WriteLine ( $"{GenericGridControl . GenericGrid . RowHeight}, {GenericGridControl . GenericGrid . Items . Count}, {GenericGridControl . GenericGrid . FontSize}, {GenericGridControl . GenericGrid . Foreground}, {GenericGridControl . GenericGrid . Background}" );
        }
    }
}
