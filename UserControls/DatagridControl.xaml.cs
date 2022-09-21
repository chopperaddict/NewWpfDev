using System;
using System . Collections;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Data;
using System . Data . SqlClient;
using System . Diagnostics;
using System . Linq;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;
using System . Windows . Media;
using NewWpfDev . Properties;
using Dapper;

using DapperGenericsLib;


using NewWpfDev;
using NewWpfDev . Models;
using NewWpfDev . UserControls;

using GenericClass = NewWpfDev . GenericClass;

namespace UserControls
{
    /// <summary>
    ///// DatagridControl.xaml
    ///// This is a generic DataGrid that supports loading virtually ANY SqlTable
    /////and also allows you to copy them (as the full real schema ) to a new table
    ////// Also provides my FlowDoc window for messaging
    /// </summary>
    public partial class DatagridControl : UserControl
    {
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

        #region Declarations
        public List<DataGridLayout> dglayoutlist = new List<DataGridLayout> ( );
        public DataGrid Dgrid;
        public static Window ParentWin { get; set; }
        public FlowdocLib fdl = new FlowdocLib ( );
        public static string CurrentTable { get; set; }
        public static Control ParentCtrl { get; set; }

        //Flowdoc declarations
        private double XLeft = 0;
        private double YTop = 0;
        private bool UseFlowdoc = true;
        public static object MovingObject { get; set; }

        public static bool ConvertDateTimeToNvarchar { get; set; } = false;
        public static string DbConnectionString = "Data Source=DINO-PC;Initial Catalog=\"IAN1\";Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        // collection identical to ObservableCollection<GenericClass>
        //        public ObsCollections collection = new ObsCollections ();
        public static ObservableCollection<GenericClass> TableColumnsCollection { get; set; }
        public static ObservableCollection<GenericClass> GridData { get; set; }
        #endregion

        public DatagridControl ( )
        {
            InitializeComponent ( );
            Dgrid = datagridControl;
            GridData = new ObservableCollection<GenericClass> ( );
            Dgrid . UpdateLayout ( );
            Flowdoc . ExecuteFlowDocMaxmizeMethod += new EventHandler ( MaximizeFlowDoc );
            FlowDoc . FlowDocClosed += Flowdoc_FlowDocClosed;
        }
        public static void SetParent ( Control parent )
        {
            ParentCtrl = parent;
            Type type = parent . GetType ( );
            if ( type == typeof ( Window ) )
                ParentWin = parent as Window;
        }

        /**************************************************************************************************************/
        #region Dependency Properties
        /**************************************************************************************************************/
        public Brush AlternateBackground
        {
            get { return ( Brush ) GetValue ( AlternateBackgroundProperty ); }
            set { SetValue ( AlternateBackgroundProperty , value ); }
        }
        public static readonly DependencyProperty AlternateBackgroundProperty =
            DependencyProperty . Register ( "AlternateBackground" , typeof ( Brush ) , typeof ( DatagridControl ) , new PropertyMetadata ( Brushes . Yellow ) );
        /**************************************************************************************************************/
        public Style Cellstyle
        {
            get { return ( Style ) GetValue ( CellstyleProperty ); }
            set { SetValue ( CellstyleProperty , value ); }
        }
        public static readonly DependencyProperty CellstyleProperty =
            DependencyProperty . Register ( "Cellstyle" , typeof ( Style ) , typeof ( DatagridControl ) , new PropertyMetadata ( default ) );
        /**************************************************************************************************************/
        public ObservableCollection<GenericClass> Data
        {
            get { return ( ObservableCollection<GenericClass> ) GetValue ( DataProperty ); }
            set { SetValue ( DataProperty , value ); }
        }
        public static readonly DependencyProperty DataProperty =
            DependencyProperty . Register ( "Data" ,
                typeof ( ObservableCollection<GenericClass> ) ,
                typeof ( DatagridControl ) ,
                new PropertyMetadata ( ( ObservableCollection<GenericClass> ) null ) );
        /**************************************************************************************************************/
        public string Tablename
        {
            get { return ( string ) GetValue ( TablenameProperty ); }
            set { SetValue ( TablenameProperty , value ); }
        }
        public static readonly DependencyProperty TablenameProperty =
            DependencyProperty . Register ( "Tablename" , typeof ( string ) , typeof ( DatagridControl ) , new PropertyMetadata ( "" ) );
        /**************************************************************************************************************/
        public int Selection
        {
            get { return ( int ) GetValue ( SelectionProperty ); }
            set { SetValue ( SelectionProperty , value ); }
        }
        public static readonly DependencyProperty SelectionProperty =
            DependencyProperty . Register ( "Selection" , typeof ( int ) , typeof ( DatagridControl ) , new PropertyMetadata ( ( int ) 0 ) );
        /**************************************************************************************************************/
        public DataTemplate GridDataTemplate
        {
            get { return ( DataTemplate ) GetValue ( GridDataTemplateProperty ); }
            set { SetValue ( GridDataTemplateProperty , value ); }
        }
        public static readonly DependencyProperty GridDataTemplateProperty =
            DependencyProperty . Register ( "GridDataTemplate" , typeof ( DataTemplate ) , typeof ( DatagridControl ) , new PropertyMetadata ( default ) );
        /**************************************************************************************************************/
        public Style GridStyle
        {
            get { return ( Style ) GetValue ( GridStyleProperty ); }
            set { SetValue ( GridStyleProperty , value ); }
        }
        public static readonly DependencyProperty GridStyleProperty =
            DependencyProperty . Register ( "GridStyle" , typeof ( Style ) , typeof ( DatagridControl ) , new PropertyMetadata ( ( Style ) null ) );
        /**************************************************************************************************************/
        public SolidColorBrush HeaderBackground
        {
            get { return ( SolidColorBrush ) GetValue ( HeaderBackgroundProperty ); }
            set { SetValue ( HeaderBackgroundProperty , value ); }
        }
        public static readonly DependencyProperty HeaderBackgroundProperty =
            DependencyProperty . Register ( "HeaderBackground" , typeof ( SolidColorBrush ) , typeof ( DatagridControl ) , new PropertyMetadata ( Brushes . Black ) );
        /**************************************************************************************************************/
        public SolidColorBrush HeaderForeground
        {
            get { return ( SolidColorBrush ) GetValue ( HeaderForegroundProperty ); }
            set { SetValue ( HeaderForegroundProperty , value ); }
        }
        public static readonly DependencyProperty HeaderForegroundProperty =
            DependencyProperty . Register ( "HeaderForeground" , typeof ( SolidColorBrush ) , typeof ( DatagridControl ) , new PropertyMetadata ( Brushes . Yellow ) );
        /**************************************************************************************************************/
        #endregion Dependency Properties

        public async Task<ObservableCollection<GenericClass>> LoadData ( string table , bool UseTrueColumns , string ConnectionString )
        {
            ObservableCollection<GenericClass> collection = new ObservableCollection<GenericClass> ( );
            datagridControl . ItemsSource = null;

            IEnumerable<ObservableCollection<GenericClass>> result;
            await Task . Run ( ( ) => GetSqlData<ObservableCollection<GenericClass>> ( table , ConnectionString ) );
            collection = Data as ObservableCollection<GenericClass>;
            // Create a completely new instance via seriazable Clone method stored in NewWpfDev.Utils (in ObjectCopier class file)
            GridData = NewWpfDev . Utils . CopyCollection ( collection , GridData );
            //ObservableCollection<GenericClass>  clonedGridData = collection .MakeClone (  );
            datagridControl . UpdateLayout ( );
            Data = collection;
            PostProcessData ( collection , datagridControl , table , UseTrueColumns );
            datagridControl . UpdateLayout ( );
            datagridControl . RefreshGrid ( );
            // grid IS LOADED by  here....
            datagridControl . SelectedIndex = 0;
            GenericClass gcc = datagridControl . SelectedItem as GenericClass;
            int colcount = GetGenericColumnCount ( collection , gcc );
            // Clear list f column info as we are loading a  different table
            dglayoutlist . Clear ( );
            CurrentTable = table;
            GetNewColumnsInfo ( collection , table );

            ShowTrueColumns ( Dgrid , table , colcount , UseTrueColumns );
            return GridData;

        }
        /**************************************************************************************************************/
        public ObservableCollection<GenericClass> LoadGenericData ( string table , bool UseTrueColumns , string ConnectionString )
        {
            ObservableCollection<GenericClass> collection = new ObservableCollection<GenericClass> ( );
            //dglayoutlist.Clear ();
            datagridControl . ItemsSource = null;
            string ResultString = "";

            collection = LoadGeneric ( $"SpLoadTableAsGeneric {table}" , ConnectionString , out ResultString );
            // Create a completely new instance via seriazable Clone method stored in NewWpfDev.Utils (in ObjectCopier class file)
            GridData = NewWpfDev . Utils . CopyCollection ( collection , GridData );
            //            GridData = collection .MakeClone (  );
            datagridControl . UpdateLayout ( );
            Data = collection;
            PostProcessData ( collection , datagridControl , table , UseTrueColumns );
            datagridControl . UpdateLayout ( );
            datagridControl . RefreshGrid ( );
            // grid IS LOADED by  here....
            datagridControl . SelectedIndex = 0;
            GenericClass gcc = datagridControl . SelectedItem as GenericClass;
            int colcount = GetGenericColumnCount ( collection , gcc );
            // Clear list f column info as we are loading a  different table
            dglayoutlist . Clear ( );
            CurrentTable = table;
            GetNewColumnsInfo ( collection , table );

            ShowTrueColumns ( Dgrid , table , colcount , UseTrueColumns );
            return GridData;
        }

        public static int GetGenericColumnCount ( ObservableCollection<GenericClass> collection , GenericClass gcc = null )
        {
            GenericClass gc = new GenericClass ( );
            if ( collection == null )
                return 0;
            try
            {
                if ( gcc == null )
                    gc = collection [ 0 ] as GenericClass;
                else
                    gc = gcc;

                if ( gc . field20 != null )
                {
                    return 20;
                }
                else if ( gc . field19 != null )
                {
                    return 19;
                }
                else if ( gc . field18 != null )
                {
                    return 18;
                }
                else if ( gc . field17 != null )
                {
                    return 17;
                }
                else if ( gc . field16 != null )
                {
                    return 16;
                }
                else if ( gc . field15 != null )
                {
                    return 15;
                }
                else if ( gc . field14 != null )
                {
                    return 14;
                }
                else if ( gc . field13 != null )
                {
                    return 13;
                }
                else if ( gc . field12 != null )
                {
                    return 12;
                }
                else if ( gc . field11 != null )
                {
                    return 11;
                }
                else if ( gc . field10 != null )
                {
                    return 10;
                }
                else if ( gc . field9 != null )
                {
                    return 9;
                }
                else if ( gc . field8 != null )
                {
                    return 8;
                }
                else if ( gc . field7 != null )
                {
                    return 7;
                }
                else if ( gc . field6 != null )
                {
                    return 6;
                }
                else if ( gc . field5 != null )
                {
                    return 5;
                }
                else if ( gc . field4 != null )
                {
                    return 4;
                }
                else if ( gc . field3 != null )
                {
                    return 3;
                }
                else if ( gc . field2 != null )
                {
                    return 2;
                }
                else if ( gc . field1 != null )
                {
                    return 1;
                }
                return 0;
            }
            catch ( Exception ex )
            {
                Debug . WriteLine ( $"Column count error '{ex . Message}, {ex . Data}'" );
            }
            return 0;
        }
        public static ObservableCollection<GenericClass> LoadGeneric ( string Sqlcommand , string ConnectionString , out string ResultString , int max = 0 , bool Notify = false , bool isMultiMode = false )
        {
            string argument = "";
            ObservableCollection<GenericClass> generics = new ObservableCollection<GenericClass> ( );
            if ( Sqlcommand . Contains ( " " ) )
            {
                string [ ] args = Sqlcommand . Split ( " " );
                argument = args [ 1 ];
                Sqlcommand = args [ 0 ] . Trim ( );
            }
            ExecuteStoredProcedure ( Sqlcommand ,
                 ConnectionString ,
                generics ,
                out ResultString ,
                "" ,
                argument ,
                null ,
                false );
            return generics;
        }
        public static ObservableCollection<GenericClass> ExecuteStoredProcedure (
        string SqlCommand ,
        string ConnectionString ,
        ObservableCollection<GenericClass> generics ,
        out string ResultString ,
        string DbName = "" ,
        string Arguments = "" ,
        RoutedEventArgs e = null ,
        bool displayData = false )
        {
            ResultString = "";
            string SavedValue = SqlCommand;
            string errormsg = "";
            int totalcolumns = 0;
            Dictionary<string , object> dict = new Dictionary<string , object> ( );
            //============
            // Sanity checks
            //============
            if ( SavedValue == "spGetFullSchema" && Arguments == "FULL" )
            {
                Arguments = "";
            }
            try
            {
                List<string> genericlist = new List<string> ( );
                bool usegeneric = false;

                generics . Clear ( );
                totalcolumns = CreateGenericCollection (
                    ref generics ,
                    SqlCommand ,
                        ConnectionString ,
                    Arguments ,
                    "" ,
                    "" ,
                    ref genericlist ,
                    ref errormsg );
                ResultString = errormsg;
                return generics;
            }
            catch ( Exception ex )
            {
                MessageBox . Show ( $"SQL ERROR 1125 - {ex . Message}" );
                return null;
            }
        }

        public static int CreateGenericCollection (
                 ref ObservableCollection<GenericClass> collection ,
                 string SqlCommand ,
                 string ConnectionString ,
            string Arguments ,
                 string WhereClause ,
                 string OrderByClause ,
                 ref List<string> genericlist ,
                 ref string errormsg )
        {
            //			out string DbToOpen ,
            //====================================
            // Use DAPPER to run a Stored Procedure
            //====================================
            string result = "";
            errormsg = "";
            genericlist = new List<string> ( );
            string arg1 = "", arg2 = "", arg3 = "", arg4 = "";
            Dictionary<string , object> dict = new Dictionary<string , object> ( );
            string ConString = ConnectionString;

            if ( ConString == "" )
            {
                CheckDbDomain ( "IAN1" );
                ConString = DapperGenLib . CurrentConnectionString;
            }
            using ( IDbConnection db = new SqlConnection ( ConString ) )
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
                        int colcount = 0, maxcols = 0;

                        if ( SqlCommand . ToUpper ( ) . Contains ( "SELECT " ) )
                        {
                            //***************************************************************************************************************//
                            // Performing a standard SELECT command but returning the data in a GenericClass structure	  (Bank/Customer/Details/etc)
                            var reslt = db . Query ( SqlCommand , CommandType . Text );
                            //***************************************************************************************************************//
                            if ( reslt == null )
                            {
                                errormsg = "DT";
                                return 0;
                            }
                            else
                            {
                                //Although this is duplicated  with the one below we CANNOT make it a method()
                                errormsg = "DYNAMIC";
                                int dictcount = 0;
                                int fldcount = 0;
                                try
                                {
                                    foreach ( var item in reslt )
                                    {
                                        GenericClass gc = new GenericClass ( );
                                        try
                                        {
                                            // we need to create a dictionary for each row of data then add it to a GenericClass row then add row to Generics Db
                                            string buffer = "";
                                            List<int> VarcharList = new List<int> ( );
                                            gc = ParseDapperRow ( item , dict , out colcount , ref VarcharList );
                                            dictcount = 1;
                                            fldcount = dict . Count;
                                            string tmp = "";
                                            foreach ( var pair in dict )
                                            {
                                                try
                                                {
                                                    if ( pair . Key != null && pair . Value != null )
                                                    {
                                                        AddDictPairToGeneric ( gc , pair , dictcount++ );
                                                        tmp = pair . Key . ToString ( ) + "=" + pair . Value . ToString ( );
                                                        buffer += tmp + ",";
                                                    }
                                                }
                                                catch ( Exception ex )
                                                {
                                                    Debug . WriteLine ( $"Dictionary ERROR : {ex . Message}" );
                                                    result = ex . Message;
                                                }
                                            }
                                            //remove trailing comma
                                            string s = buffer . Substring ( 0 , buffer . Length - 1 );
                                            buffer = s;
                                            genericlist . Add ( buffer );
                                        }
                                        catch ( Exception ex )
                                        {
                                            result = $"SQLERROR : {ex . Message}";
                                            errormsg = result;
                                            Debug . WriteLine ( result );
                                        }
                                        collection . Add ( gc );
                                        dict . Clear ( );
                                        dictcount = 1;
                                    }
                                }
                                catch ( Exception ex )
                                {
                                    Debug . WriteLine ( $"OUTER DICT/PROCEDURE ERROR : {ex . Message}" );
                                    result = ex . Message;
                                    errormsg = result;
                                }
                                if ( errormsg == "" )
                                    errormsg = $"DYNAMIC:{fldcount}";
                                return collection . Count;
                            }
                        }
                        else
                        {
                            // probably a stored procedure ?  							
                            bool IsSuccess = false;
                            int fldcount = 0;

                            //***************************************************************************************************************//
                            // This returns the data from SP commands (only) in a GenericClass Structured format
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
                                        GenericClass gc = new GenericClass ( );
                                        try
                                        {
                                            //	Create a dictionary for each row of data then add it to a GenericClass row then add row to Generics Db
                                            List<int> VarcharList = new List<int> ( );
                                            gc = ParseDapperRow ( item , dict , out colcount , ref VarcharList );
                                            dictcount = 1;
                                            fldcount = dict . Count;
                                            if ( fldcount == 0 )
                                            {
                                                //no problem, we will get a Datatable anyway
                                                return 0;
                                            }
                                            string buffer = "", tmp = "";
                                            foreach ( var pair in dict )
                                            {
                                                try
                                                {
                                                    if ( pair . Key != null && pair . Value != null )
                                                    {
                                                        AddDictPairToGeneric ( gc , pair , dictcount++ );
                                                        tmp = pair . Key . ToString ( ) + "=" + pair . Value . ToString ( );
                                                        buffer += tmp + ",";
                                                    }
                                                }
                                                catch ( Exception ex )
                                                {
                                                    Debug . WriteLine ( $"Dictionary ERROR : {ex . Message}" );
                                                    result = ex . Message;
                                                }
                                            }
                                            IsSuccess = true;
                                            string s = buffer . Substring ( 0 , buffer . Length - 1 );
                                            buffer = s;
                                            genericlist . Add ( buffer );
                                        }
                                        catch ( Exception ex )
                                        {
                                            result = $"SQLERROR : {ex . Message}";
                                            Debug . WriteLine ( result );
                                            return 0;
                                        }
                                        //										gc . ActiveColumns = dict . Count;
                                        //ParseListToDbRecord ( genericlist , out gc );
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
                                        return 0;
                                    }
                                    else
                                    {
                                        long x = reslt . LongCount ( );
                                        if ( x == ( long ) 0 )
                                        {
                                            result = $"ERROR : [{SqlCommand}] returned ZERO records... ";
                                            errormsg = $"DYNAMIC:0";
                                            return 0;
                                        }
                                        else
                                        {
                                            result = ex . Message;
                                            errormsg = $"UNKNOWN :{ex . Message}";
                                        }
                                        return 0;
                                    }
                                }
                            }
                            if ( IsSuccess == false )
                            {
                                errormsg = $"Dapper request returned zero results, maybe one or more arguments are required, or the Procedure does not return any values ?";
                                Debug . WriteLine ( errormsg );
                            }
                            else
                                return fldcount;
                            //return 0;
                        }
                    }
                    catch ( Exception ex )
                    {
                        Debug . WriteLine ( $"STORED PROCEDURE ERROR : {ex . Message}" );
                        result = ex . Message;
                        errormsg = $"SQLERROR : {result}";
                    }
                }
                catch ( Exception ex )
                {
                    Debug . WriteLine ( $"Sql Error, {ex . Message}, {ex . Data}" );
                    result = ex . Message;
                }
            }
            return dict . Count;
        }
        public static GenericClass ParseDapperRow ( dynamic buff , Dictionary<string , object> dict , out int colcount , ref List<int> varcharlen , bool GetLength = false )
        {
            GenericClass GenRow = new GenericClass ( );
            int index = 0;
            colcount = 0;
            foreach ( var item in buff )
            {
                try
                {
                    index += 1;
                    if ( item . Key == "" || ( item . Value == null && item . Key . Contains ( "character_maximum_length" ) == false ) )
                        break;
                    if ( GetLength && item . Key . Contains ( "character_maximum_length" ) )
                    {
                        varcharlen . Add ( item . Value == null ? 0 : item . Value );
                    }
                    else
                        dict . Add ( item . Key , item . Value );

                    //var v = buff .ToString();
                    //varcharlen.Add(item.Key, buff [ 2 ]);
                }
                catch ( Exception ex )
                {
                    MessageBox . Show ( $"ParseDapper error was : \n{ex . Message}\nKey={item . Key} Value={item . Value . ToString ( )}" );
                    break;
                }
            }
            colcount = index;
            return GenRow;
        }

        public static void AddDictPairToGeneric ( GenericClass gc , KeyValuePair<string , object> dict , int dictcount )
        {
            switch ( dictcount )
            {
                case 1:
                    gc . field1 = dict . Value . ToString ( );
                    break;
                case 2:
                    gc . field2 = dict . Value . ToString ( );
                    break;
                case 3:
                    gc . field3 = dict . Value . ToString ( );
                    break;
                case 4:
                    gc . field4 = dict . Value . ToString ( );
                    break;
                case 5:
                    gc . field5 = dict . Value . ToString ( );
                    break;
                case 6:
                    gc . field6 = dict . Value . ToString ( );
                    break;
                case 7:
                    gc . field7 = dict . Value . ToString ( );
                    break;
                case 8:
                    gc . field8 = dict . Value . ToString ( );
                    break;
                case 9:
                    gc . field9 = dict . Value . ToString ( );
                    break;
                case 10:
                    gc . field10 = dict . Value . ToString ( );
                    break;
                case 11:
                    gc . field10 = dict . Value . ToString ( );
                    break;
                case 12:
                    gc . field12 = dict . Value . ToString ( );
                    break;
                case 13:
                    gc . field13 = dict . Value . ToString ( );
                    break;
                case 14:
                    gc . field14 = dict . Value . ToString ( );
                    break;
                case 15:
                    gc . field15 = dict . Value . ToString ( );
                    break;
                case 16:
                    gc . field16 = dict . Value . ToString ( );
                    break;
                case 17:
                    gc . field17 = dict . Value . ToString ( );
                    break;
                case 18:
                    gc . field18 = dict . Value . ToString ( );
                    break;
                case 19:
                    gc . field19 = dict . Value . ToString ( );
                    break;
                case 20:
                    gc . field20 = dict . Value . ToString ( );
                    break;
            }
        }
        public void PostProcessData ( ObservableCollection<GenericClass> collection , DataGrid grid , string Table , bool UseTrueColumns )
        {
            List<GenericClass> Glist = new List<GenericClass> ( );
            Glist = collection . ToList ( );
            int colcount = GetColumnsCount ( Glist );
            LoadActiveRowsOnlyInGrid ( grid , collection , colcount );
        }
        public void ShowTrueColumns ( DataGrid grid , string Table , int colcount , bool Show )
        {
            if ( Show == false )
            {
                SetDefColumnHeaderText ( grid , false );
                return;
            }
            grid = datagridControl;
            List<Dictionary<string , string>> ColumnTypesList = new List<Dictionary<string , string>> ( );
            ColumnTypesList = ReplaceDataGridFldNames ( Table , ref grid , ref this . dglayoutlist , colcount );
            if ( ColumnTypesList . Count == 0 )
            {
                MessageBox . Show ( $"There are no column information available for {Table . ToUpper ( )}" , "Table Columns handler" );
            }
        }
        public static void LoadActiveRowsOnlyInGrid ( DataGrid Grid , ObservableCollection<GenericClass> genericcollection , int total )
        {
            //$"Entering " . cwinfo();

            // filter data to remove all "extraneous" columns
            Grid . ItemsSource = null;
            Grid . Items . Clear ( );

            #region getdata

            if ( total == 1 )
            {
                var res =
                       ( from data in genericcollection
                         select new
                         {
                             data . field1
                         } ) . ToList ( );
                Grid . ItemsSource = res;
            }
            else if ( total == 2 )
            {
                var res =
                       ( from data in genericcollection
                         select new
                         {
                             data . field1 ,
                             data . field2
                         } ) . ToList ( );
                Grid . ItemsSource = res;
            }
            else if ( total == 3 )
            {
                var res =
                       ( from data in genericcollection
                         select new
                         {
                             data . field1 ,
                             data . field2 ,
                             data . field3
                         } ) . ToList ( );
                Grid . ItemsSource = res;
            }
            else if ( total == 4 )
            {
                var res =
                       ( from data in genericcollection
                         select new
                         {
                             data . field1 ,
                             data . field2 ,
                             data . field3 ,
                             data . field4
                         } ) . ToList ( );
                Grid . ItemsSource = res;
            }
            else if ( total == 5 )
            {
                var res =
                       ( from data in genericcollection
                         select new
                         {
                             data . field1 ,
                             data . field2 ,
                             data . field3 ,
                             data . field4 ,
                             data . field5
                         } ) . ToList ( );
                Grid . ItemsSource = res;
            }
            else if ( total == 6 )
            {
                var res =
                       ( from data in genericcollection
                         select new
                         {
                             data . field1 ,
                             data . field2 ,
                             data . field3 ,
                             data . field4 ,
                             data . field5 ,
                             data . field6
                         } ) . ToList ( );
                Grid . ItemsSource = res;
            }
            else if ( total == 7 )
            {
                var res =
                       ( from data in genericcollection
                         select new
                         {
                             data . field1 ,
                             data . field2 ,
                             data . field3 ,
                             data . field4 ,
                             data . field5 ,
                             data . field6 ,
                             data . field7
                         } ) . ToList ( );
                Grid . ItemsSource = res;
            }
            else if ( total == 8 )
            {
                var res =
                       ( from data in genericcollection
                         select new
                         {
                             data . field1 ,
                             data . field2 ,
                             data . field3 ,
                             data . field4 ,
                             data . field5 ,
                             data . field6 ,
                             data . field7 ,
                             data . field8
                         } ) . ToList ( );
                Grid . ItemsSource = res;
            }
            else if ( total == 9 )
            {
                var res =
                       ( from data in genericcollection
                         select new
                         {
                             data . field1 ,
                             data . field2 ,
                             data . field3 ,
                             data . field4 ,
                             data . field5 ,
                             data . field6 ,
                             data . field7 ,
                             data . field8 ,
                             data . field9
                         } ) . ToList ( );
                Grid . ItemsSource = res;
            }
            else if ( total == 10 )
            {
                var res =
                       ( from data in genericcollection
                         select new
                         {
                             data . field1 ,
                             data . field2 ,
                             data . field3 ,
                             data . field4 ,
                             data . field5 ,
                             data . field6 ,
                             data . field7 ,
                             data . field8 ,
                             data . field9 ,
                             data . field10
                         } ) . ToList ( );
                Grid . ItemsSource = res;
            }
            else if ( total == 11 )
            {
                var res =
                       ( from data in genericcollection
                         select new
                         {
                             data . field1 ,
                             data . field2 ,
                             data . field3 ,
                             data . field4 ,
                             data . field5 ,
                             data . field6 ,
                             data . field7 ,
                             data . field8 ,
                             data . field9 ,
                             data . field10 ,
                             data . field11
                         } ) . ToList ( );
                Grid . ItemsSource = res;
            }
            else if ( total == 12 )
            {
                var res =
                       ( from data in genericcollection
                         select new
                         {
                             data . field1 ,
                             data . field2 ,
                             data . field3 ,
                             data . field4 ,
                             data . field5 ,
                             data . field6 ,
                             data . field7 ,
                             data . field8 ,
                             data . field9 ,
                             data . field10 ,
                             data . field11 ,
                             data . field12
                         } ) . ToList ( );
                Grid . ItemsSource = res;
            }
            else if ( total == 13 )
            {
                var res =
                       ( from data in genericcollection
                         select new
                         {
                             data . field1 ,
                             data . field2 ,
                             data . field3 ,
                             data . field4 ,
                             data . field5 ,
                             data . field6 ,
                             data . field7 ,
                             data . field8 ,
                             data . field9 ,
                             data . field10 ,
                             data . field11 ,
                             data . field12 ,
                             data . field13
                         } ) . ToList ( );
                Grid . ItemsSource = res;
            }
            else if ( total == 14 )
            {
                var res =
                       ( from data in genericcollection
                         select new
                         {
                             data . field1 ,
                             data . field2 ,
                             data . field3 ,
                             data . field4 ,
                             data . field5 ,
                             data . field6 ,
                             data . field7 ,
                             data . field8 ,
                             data . field9 ,
                             data . field10 ,
                             data . field11 ,
                             data . field12 ,
                             data . field13 ,
                             data . field14
                         } ) . ToList ( );
                Grid . ItemsSource = res;
            }
            else if ( total == 15 )
            {
                var res =
                       ( from data in genericcollection
                         select new
                         {
                             data . field1 ,
                             data . field2 ,
                             data . field3 ,
                             data . field4 ,
                             data . field5 ,
                             data . field6 ,
                             data . field7 ,
                             data . field8 ,
                             data . field9 ,
                             data . field10 ,
                             data . field11 ,
                             data . field12 ,
                             data . field13 ,
                             data . field14 ,
                             data . field15
                         } ) . ToList ( );
                Grid . ItemsSource = res;
            }
            else if ( total == 16 )
            {
                var res =
                       ( from data in genericcollection
                         select new
                         {
                             data . field1 ,
                             data . field2 ,
                             data . field3 ,
                             data . field4 ,
                             data . field5 ,
                             data . field6 ,
                             data . field7 ,
                             data . field8 ,
                             data . field9 ,
                             data . field10 ,
                             data . field11 ,
                             data . field12 ,
                             data . field13 ,
                             data . field14 ,
                             data . field15 ,
                             data . field16
                         } ) . ToList ( );
                Grid . ItemsSource = res;
            }
            else if ( total == 17 )
            {
                var res =
                       ( from data in genericcollection
                         select new
                         {
                             data . field1 ,
                             data . field2 ,
                             data . field3 ,
                             data . field4 ,
                             data . field5 ,
                             data . field6 ,
                             data . field7 ,
                             data . field8 ,
                             data . field9 ,
                             data . field10 ,
                             data . field11 ,
                             data . field12 ,
                             data . field13 ,
                             data . field14 ,
                             data . field15 ,
                             data . field16 ,
                             data . field17
                         } ) . ToList ( );
                Grid . ItemsSource = res;
            }
            else if ( total == 18 )
            {
                var res =
                       ( from data in genericcollection
                         select new
                         {
                             data . field1 ,
                             data . field2 ,
                             data . field3 ,
                             data . field4 ,
                             data . field5 ,
                             data . field6 ,
                             data . field7 ,
                             data . field8 ,
                             data . field9 ,
                             data . field10 ,
                             data . field11 ,
                             data . field12 ,
                             data . field13 ,
                             data . field14 ,
                             data . field15 ,
                             data . field16 ,
                             data . field17 ,
                             data . field18
                         } ) . ToList ( );
                Grid . ItemsSource = res;
            }
            else if ( total == 19 )
            {
                var res =
                       ( from data in genericcollection
                         select new
                         {
                             data . field1 ,
                             data . field2 ,
                             data . field3 ,
                             data . field4 ,
                             data . field5 ,
                             data . field6 ,
                             data . field7 ,
                             data . field8 ,
                             data . field9 ,
                             data . field10 ,
                             data . field11 ,
                             data . field12 ,
                             data . field13 ,
                             data . field14 ,
                             data . field15 ,
                             data . field16 ,
                             data . field17 ,
                             data . field18 ,
                             data . field19
                         } ) . ToList ( );
                Grid . ItemsSource = res;
            }
            else if ( total == 20 )
            {
                var res =
                       ( from data in genericcollection
                         select new
                         {
                             data . field1 ,
                             data . field2 ,
                             data . field3 ,
                             data . field4 ,
                             data . field5 ,
                             data . field6 ,
                             data . field7 ,
                             data . field8 ,
                             data . field9 ,
                             data . field10 ,
                             data . field11 ,
                             data . field12 ,
                             data . field13 ,
                             data . field14 ,
                             data . field15 ,
                             data . field16 ,
                             data . field17 ,
                             data . field18 ,
                             data . field19 ,
                             data . field20
                         } ) . ToList ( );
                Grid . ItemsSource = res;

            }
            #endregion

            Grid . SelectedIndex = 0;
            Grid . Visibility = Visibility . Visible;
            //			GridCount . Text = Grid . Items . Count . ToString ( );
            Grid . UpdateLayout ( );
            Grid . Focus ( );
            //        $"Exiting " . cwinfo();
        }
        public static int GetColumnsCount ( List<GenericClass> list )
        {
            int counter = 1;
            int maxcol = 0;
            foreach ( GenericClass item in list )
            {
                // We only ever do this for a single record !!!!  Not all records, so pretty fast
                GenericClass GenClass = item;
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
            // Adjust to actual columns count
            return maxcol - 1;
        }
        public static List<Dictionary<string , string>> ReplaceDataGridFldNames ( string CurrentType , ref DataGrid Grid1 , ref List<DataGridLayout> dglayoutlist , int colcount , string Domain = "IAN1" )
        {
            List<string> list = new List<string> ( );
            ObservableCollection<GenericClass> GenClass = new ObservableCollection<GenericClass> ( );
            Dictionary<string , string> dict = new Dictionary<string , string> ( );
            List<Dictionary<string , string>> ColumntypesList = new List<Dictionary<string , string>> ( );
            // pass down dictionary that will return with column names and SQL types
            Dictionary<string , string> Columntypes = new Dictionary<string , string> ( );
            List<Dictionary<string , string>> ColumnTypesList = new List<Dictionary<string , string>> ( );
            // returns list(Dict<str,str>>)
            // clear reference sturcture first off
            //dglayoutlist . Clear ( );
            //           if ( dglayoutlist . Count == 0 )
            dict = GetDbTableColumns ( ref GenClass , ref ColumnTypesList , ref list , CurrentType , Domain , ref dglayoutlist );
            // dglayoutlist is now fully populated        
            int index = 0;
            // Add data  for field size
            if ( GenClass . Count > 0 )
            {
                if ( list . Count > 0 )
                {
                    index = 0;
                    // use the list to get the correct column header info
                    foreach ( var item in Grid1 . Columns )
                    {
                        DataGridColumn dgc = item;
                        try
                        {
                            dgc . Header = "";
                            dgc . Header = list [ index++ ];
                            // Update  the datagrid's column header here...
                            item . Header = dgc . Header;
                            if ( index >= dict . Count )
                            {
                                break;
                            }
                        }
                        catch ( ArgumentOutOfRangeException ex ) { Debug . WriteLine ( $"TODO - BAD Columns - 300 GenericDbHandlers.cs" ); }
                    }
                }
                // Grid now has valid column names, but still got All 20 ??
                Grid1 . UpdateLayout ( );
            }
            return ColumnTypesList;
        }
        public static Dictionary<string , string> GetDbTableColumns ( ref ObservableCollection<GenericClass> Gencollection , ref List<Dictionary<string , string>> ColumntypesList ,
             ref List<string> list , string dbName , string DbDomain , ref List<DataGridLayout> dglayoutlist )
        {
            // This call CHANGES GridData content to columns data !!
            Dictionary<string , string> dict = GetSpArgs ( ref Gencollection , ref ColumntypesList , ref list , dbName , DbDomain , ref dglayoutlist );
            return dict;
        }
        public static Dictionary<string , string> GetSpArgs ( ref ObservableCollection<GenericClass> Gencollection , ref List<Dictionary<string , string>> ColumntypesList ,
            ref List<string> list , string dbName , string DbDomain , ref List<DataGridLayout> dglayoutlist )
        {
            //this is an obs-collection of dglayoutlist
            DataTable dt = new DataTable ( );
            GenericClass genclass = new GenericClass ( );
            Dictionary<string , string> dict = new Dictionary<string , string> ( );
            try
            {
                // only used by grid2 on initial load cos grid1 uses List for datasource & gets count diffrently.
                //called on initial load to get column name and type (not datagrid data)
                dglayoutlist . Clear ( );
                if ( dglayoutlist . Count == 0 )
                {
                    TableColumnsCollection = LoadDbAsGenericData ( "spGetTableColumnWithSize" , Gencollection , ref ColumntypesList , dbName , DbDomain , ref dglayoutlist , true );
                    Gencollection = GridData;
                }
            }
            catch ( Exception ex )
            {
                MessageBox . Show ( $"SQL ERROR 1125 - {ex . Message}" );
                dict . Clear ( );
                return dict;
            }
            dict . Clear ( );
            list . Clear ( );
            try
            {
                int charlenindex = 0;
                foreach ( var item in TableColumnsCollection )
                {
                    GenericClass gc = new GenericClass ( );
                    gc = item as GenericClass;
                    //                   gc . field3 = VarCharLength[charlenindex++] . ToString ( );
                    if ( dict . TryAdd ( gc . field1 , gc . field2 ) )
                        list . Add ( gc . field1 . ToString ( ) );
                }
            }
            catch ( Exception ex )
            {
                Debug . WriteLine ( ex . Message );
                //               "Exiting with error" . cwwarn ( );
            }
            return dict;
        }

        public static ObservableCollection<GenericClass> LoadDbAsGenericData (
             string SqlCommand ,
             ObservableCollection<GenericClass> collection ,
             ref List<Dictionary<string , string>> ColumntypesList ,
             string Arguments ,
             string DbDomain ,
             ref List<DataGridLayout> dglayoutlist ,
             bool GetLengths = false )
        {
            string result = "";
            string arg1 = "", arg2 = "", arg3 = "", arg4 = "";
            // provide a default connection string
            string ConString = "BankSysConnectionString";
            Dictionary<string , dynamic> dict = new Dictionary<string , object> ( );
            ObservableCollection<GenericClass> GenClass = new ObservableCollection<GenericClass> ( );

            dglayoutlist . Clear ( );
            // Ensure we have the correct connection string for the current Db Doman
            DapperLibSupport . CheckResetDbConnection ( DbDomain , out string constr );
            //CurrentConnectionString = constr;
            ConString = constr;
            collection . Clear ( );
            using ( IDbConnection db = new SqlConnection ( ConString ) )
            {
                try
                {
                    // Use DAPPER to run  Stored Procedure
                    // One or No arguments
                    arg1 = Arguments;
                    if ( arg1 . Contains ( "," ) )              // trim comma off
                        arg1 = arg1 . Substring ( 0 , arg1 . Length - 1 );
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

                    //***************************************************************************************************************//
                    // This returns the data from SP commands (only) in a GenericClass Structured format
                    // FAILS on parsedapper
                    var reslt = db . Query ( SqlCommand , Params , commandType: CommandType . StoredProcedure );
                    //***************************************************************************************************************//

                    if ( reslt != null )
                    {
                        //Although this is duplicated  with the one below we CANNOT make it a method()
                        string errormsg = "DYNAMIC";
                        int dictcount = 0;
                        int fldcount = 0;
                        int colcount = 0;
                        GenericClass gc = new GenericClass ( );

                        List<int> VarcharList = new List<int> ( );
                        Dictionary<string , string> outdict = new Dictionary<string , string> ( );
                        try
                        {
                            foreach ( var item in reslt )
                            {
                                DataGridLayout dglayout = new DataGridLayout ( );
                                try
                                {
                                    // we need to create a dictionary for each row of data then add it to a GenericClass row then add row to Generics Db
                                    string buffer = "";
                                    // WORKS OK
                                    ParseDapperRow ( item , dict , out colcount );
                                    gc = new GenericClass ( );
                                    dictcount = 1;
                                    int index = 1;
                                    fldcount = dict . Count;
                                    string tmp = "";

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
                                                    AddDictPairToGeneric ( gc , pair , dictcount++ );
                                                    tmp = $"field{index++} = {pair . Value . ToString ( )}";
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
                                                    if ( IsDuplicatecolumnname ( temp , Columntypes ) == false )
                                                        Columntypes . Add ( temp , item . data_type . ToString ( ) );
                                                    dglayout . Fieldname = item . column_name == null ? "UNSPECIFIED" : item . column_name;
                                                    // Add Dictionary <string,string> to List<Dictionary<string,string>
                                                    ColumntypesList . Add ( Columntypes );
                                                }
                                            }
                                        }
                                        catch ( Exception ex )
                                        {
                                            Debug . WriteLine ( $"Dictionary ERROR : {ex . Message}" );
                                            result = ex . Message;
                                        }
                                    }
                                    if ( IsDuplicateFieldname ( dglayout , dglayoutlist ) == false )
                                        dglayoutlist . Add ( dglayout );
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
                                    //genericlist.Add(buffer);
                                    collection . Add ( gc );
                                }
                                catch ( Exception ex )
                                {
                                    result = $"SQLERROR : {ex . Message}";
                                    errormsg = result;
                                    Debug . WriteLine ( result );
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
                            result = ex . Message;
                            errormsg = result;
                        }
                        if ( errormsg == "" )
                            errormsg = $"DYNAMIC:{fldcount}";
                        return collection; //collection.Count;
                    }
                }
                catch ( Exception ex )
                {
                    ///                    $"{ex . Message}" . cwerror ( );
                }
            }
            return GenClass;
        }
        public static void ParseDapperRow ( dynamic buff , Dictionary<string , object> dict , out int colcount , bool GetLength = false )
        {
            int index = 0;
            colcount = 0;
            foreach ( var item in buff )
            {
                try
                {
                    index += 1;
                    if ( item . Key == "" || ( item . Value == null && item . Key . Contains ( "character_maximum_length" ) == false ) )
                        break;
                    if ( GetLength && item . Key . Contains ( "character_maximum_length" ) )
                    {
                        //dglayoutlist.Add(item.Value == null ? 0 : item.Value);
                    }
                    else
                    {
                        if ( ConvertDateTimeToNvarchar && ( item . Key == "ODATE" || item . Key == "CDATE" ) )
                        {
                            string str = item . Value . ToString ( );
                            string [ ] items;
                            items = str . Split ( " " );
                            dict . Add ( item . Key , items [ 0 ] . Trim ( ) );
                        }
                        else
                            dict . Add ( item . Key , item . Value );
                    }
                    //var v = buff .ToString();
                    //varcharlen.Add(item.Key, buff [ 2 ]);
                }
                catch ( Exception ex )
                {
                    MessageBox . Show ( $"ParseDapper error was : \n{ex . Message}\nKey={item . Key} Value={item . Value . ToString ( )}" );
                    break;
                }
            }
            colcount = index;
            // $"Exiting " . cwinfo();
            //return GenRow;
        }
        private static bool IsDuplicateFieldname ( DataGridLayout dglayout , List<DataGridLayout> dglayoutlist )
        {
            bool success = false;
            int count = 0;
            foreach ( var item in dglayoutlist )
            {
                if ( item . Fieldname == dglayout . Fieldname )
                    return true;
            }
            return success;
        }
        private static bool IsDuplicatecolumnname ( string Columntypes , Dictionary<string , string> ColumntypesList )
        {
            bool success = false;
            int count = 0;

            foreach ( KeyValuePair<string , string> item in ColumntypesList )
            {
                if ( item . Key == Columntypes )
                {
                    success = true;
                    break;
                }
            }
            return success;
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
            //grid . Refresh ( );
            //if ( IsCollapsed ) grid . Visibility = Visibility . Collapsed;
        }
        public string GetFullColumnInfo ( string spName , string ConString , bool ShowFdl = true , bool ShowOutput = true )
        {
            string output = "";
            string errormsg = "";
            int columncount = 0;
            DataTable dt = new DataTable ( );
            ObservableCollection<GenericClass> Generics = new ObservableCollection<GenericClass> ( );
            //        ObservableCollection<BankAccountViewModel> bvmparam = new ObservableCollection<BankAccountViewModel> ( );
            List<string> genericlist = new List<string> ( );
            try
            {
                CreateGenericCollection (
                "spGetTableColumnWithSize" ,
                $"{spName}" ,
                "" ,
                "" ,
                 ref errormsg );
                dt = ProcessSqlCommand ( "spGetTableColumnWithSize  " + spName , ConString );
                if ( dt . Rows . Count == 0 )
                    columncount = 0;
                foreach ( var item in dt . Rows )
                {
                    DapperGenericsLib . GenericClass gc = new DapperGenericsLib . GenericClass ( );
                    string store = "";
                    DataRow dr = item as DataRow;
                    columncount = dr . ItemArray . Count ( );
                    if ( columncount == 0 )
                        columncount = 1;
                    // we only need max cols - 1 here !!!
                    for ( int x = 0 ; x < columncount ; x++ )
                        store += dr . ItemArray [ x ] . ToString ( ) + ",";
                    output += store;
                }
                // we now have the result, so lets process them
                // data is fieldname, sql-datatype, size (where appropriate)
                string buffer = output;
                string [ ] lines = buffer . Split ( ',' );
                output = "";
                int counter = 0;
                string type = "";
                string tmp = "";
                foreach ( var item in lines )
                {
                    switch ( counter )
                    {
                        //----------------------------------------//
                        case 0:     //field name
                            output += $"\n{item}  ";
                            break;
                        //----------------------------------------//
                        case 1: //field type
                            output += $"{item}  ";
                            type = item;
                            break;
                        //----------------------------------------//
                        case 2: // size (1)
                            if ( type == "int" || type == "decimal" )
                            {
                                output += $"  ";
                                break;
                            }
                            else
                                output += $"{item},";
                            break;
                        //----------------------------------------//
                        case 3: // Size 2 (Decimal root)
                            if ( type == "int" )
                            {
                                output += $" {item}  \n";
                                break;
                            }
                            else if ( type == "nvarchar" )
                            {
                                output += $" {item} \n";
                                break;
                            }
                            else if ( type == "decimal" )
                            {
                                output += $" {item} \n";
                                break;
                            }
                            else
                            {
                                if ( type != "int" && type != "nvarchar" && type != "decimal" )
                                    output += $"{item}\n";
                                else
                                    output += $"\n";
                                break;
                            }
                        //----------------------------------------//
                        case 4: // Size 3 (decimal Radix)
                            if ( type == "int" )
                            {
                                output += $"0,\n";
                                break;
                            }
                            else
                            {
                                if ( item != "" )
                                    output += $"{item}\n";
                                else
                                {
                                    if ( output . Substring ( 0 , output . Length - 3 ) == ",,," )
                                        output += $"\n";
                                }
                                break;
                            }
                        //----------------------------------------//
                        default:
                            counter = 0;
                            break;
                    }
                    if ( counter < 4 )
                        counter++;
                    else
                        counter = 0;

                    output = output . Substring ( 0 , output . Length - 1 );
                    // we now have a list of the Args for the selected SP in output
                    // Show it in a TextBox if it takes 1 or more args
                    // format is ("fielddname, fieldtype, size1, size2\n,")
                    if ( output != "" && ShowOutput == true && ShowFdl )
                    {
                        string fdinput = $"Procedure Name : {spName . ToUpper ( )}\n";
                        fdinput += output;
                        fdl . ShowInfo ( Flowdoc , canvas , line1: fdinput , clr1: "Black0" , line2: "" , clr2: "Black0" , line3: "" , clr3: "Black0" , header: "" , clr4: "Black0" );
                        canvas . Visibility = Visibility . Visible;
                        Flowdoc . Visibility = Visibility . Visible;
                    }
                    else
                    {
                        Mouse . OverrideCursor = Cursors . Arrow;
                    }
                }
                //       ShowLoadtime ( );
                return output;
                {
                    if ( errormsg == "" )
                        MessageBox . Show ( $"No Argument information is available" , $"[{spName}] SP Script Information" , MessageBoxButton . OK , MessageBoxImage . Warning );
                    return "";
                }
            }
            catch ( Exception ex )
            {
                MessageBox . Show ( $"SQL ERROR 1125 - {ex . Message}" );
                return "";
            }
        }
        public DataTable ProcessSqlCommand ( string SqlCommand , string connstring )
        {
            SqlConnection con;
            DataTable dt = new DataTable ( );
            string filterline = "";
            string ConString = connstring;
            //			string ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
            //Debug . WriteLine ( $"Making new SQL connection in DETAILSCOLLECTION,  Time elapsed = {timer . ElapsedMilliseconds}" );
            //SqlCommand += " TempDb";
            con = new SqlConnection ( ConString );
            try
            {
                Debug . WriteLine ( $"Using new SQL connection in PROCESSSQLCOMMAND" );
                using ( con )
                {
                    SqlCommand cmd = new SqlCommand ( SqlCommand , con );
                    SqlDataAdapter sda = new SqlDataAdapter ( cmd );
                    sda . Fill ( dt );
                }
            }
            catch ( Exception ex )
            {
                Debug . WriteLine ( $"ERROR in PROCESSSQLCOMMAND(): Failed to load Datatable :\n {ex . Message}, {ex . Data}" );
                MessageBox . Show ( $"ERROR in PROCESSSQLCOMMAND(): Failed to load datatable\n{ex . Message}" );
            }
            finally
            {
                Debug . WriteLine ( $" SQL data loaded from SQLCommand [{SqlCommand . ToUpper ( )}]" );
                con . Close ( );
            }
            return dt;
        }
        public void GetNewColumnsInfo ( ObservableCollection<GenericClass> collection , string Table )
        {
            List<string> list = new List<string> ( );
            //ObservableCollection<GenericClass> GenClass = new ObservableCollection<GenericClass> ( );
            Dictionary<string , string> dict = new Dictionary<string , string> ( );
            List<Dictionary<string , string>> ColumntypesList = new List<Dictionary<string , string>> ( );
            // pass down dictionary that will return with column names and SQL types
            Dictionary<string , string> Columntypes = new Dictionary<string , string> ( );
            List<Dictionary<string , string>> ColumnTypesList = new List<Dictionary<string , string>> ( );
            // returns list(Dict<str,str>>)
            // clear reference structure first off
            dglayoutlist . Clear ( );
            dict = GetDbTableColumns ( ref collection , ref ColumnTypesList , ref list , Table , "IAN1" , ref dglayoutlist );
            // dglayoutlist is now fully populated        
        }
        //=================================================================//
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

        private void UserControl_KeyDown ( object sender , KeyEventArgs e )
        {
            if ( e . Key == Key . F8 )
                Debugger . Break ( );
            if ( e . Key == Key . Escape )
            {
                if ( Flowdoc != null )
                {
                    Flowdoc . Visibility = Visibility . Collapsed;
                    canvas . Visibility = Visibility . Collapsed;
                }
            }
        }

        private void Flowdoc_PreviewKeyDown ( object sender , KeyEventArgs e )
        {
            if ( e . Key == Key . Escape )
            {
                if ( Flowdoc != null )
                {
                    Flowdoc . Visibility = Visibility . Collapsed;
                    canvas . Visibility = Visibility . Collapsed;
                }
            }
        }

        public static ObservableCollection<GenericClass> CreateGenericCollection (
            string SqlCommand ,
            string Arguments ,
            string WhereClause ,
            string OrderByClause ,
            ref string errormsg )
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
            string ConString = DbConnectionString;

            ObservableCollection<GenericClass> collection = new ObservableCollection<GenericClass> ( );

            if ( ConString == null || ConString == "" )
            {
                CheckDbDomain ( "IAN1" );
                ConString = DbConnectionString;
            }
            using ( IDbConnection db = new SqlConnection ( ConString ) )
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
                            //                           $"Entering for 'Sql Select'" . cwinfo();
                            //***************************************************************************************************************//
                            // Performing a standard SELECT command but returning the data in a GenericClass structure	  (Bank/Customer/Details/etc)
                            //WORKS JUST FINE
                            var reslt = db . Query ( SqlCommand , CommandType . Text );
                            //***************************************************************************************************************//
                            if ( reslt == null )
                            {
                                errormsg = "DT";
                                return null;
                            }
                            else
                            {
                                //Although this is duplicated  with the one below we CANNOT make it a method()
                                errormsg = "DYNAMIC";
                                int dictcount = 0;
                                int fldcount = 0;
                                GenericClass gc = new GenericClass ( );

                                Dictionary<string , string> outdict = new Dictionary<string , string> ( );
                                try
                                {

                                    foreach ( var item in reslt )
                                    {
                                        try
                                        {
                                            // we need to create a dictionary for each row of data then add it to a GenericClass row then add row to Generics Db
                                            string buffer = "";
                                            List<int> VarcharList = new List<int> ( );
                                            // WORKS OK
                                            //dynamic buff, Dictionary< string, object> dict, out int colcount, ref List<DataGridLayout> dglayoutlist, bool GetLength = false
                                            ParseDapperRow ( item , dict , out colcount );
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
                                                        AddDictPairToGeneric ( gc , pair , dictcount++ );
                                                        tmp = $"field{index++} = {pair . Value . ToString ( )}";
                                                        buffer += tmp + ",";
                                                        outdict . Add ( pair . Key , pair . Value . ToString ( ) );
                                                    }
                                                }
                                                catch ( Exception ex )
                                                {
                                                    //                                                   $"Dictionary ERROR : {ex . Message}" . cwerror ( );
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
                                            //genericlist.Add(buffer);
                                            collection . Add ( gc );
                                        }
                                        catch ( Exception ex )
                                        {
                                            result = $"SQLERROR : {ex . Message}";
                                            errormsg = result;
                                            //result . cwerror ( );
                                        }
                                        //collection.Add(gc);
                                        dict . Clear ( );
                                        outdict . Clear ( );
                                        dictcount = 1;
                                    }
                                }
                                catch ( Exception ex )
                                {
                                    //                                   $"OUTER DICT/PROCEDURE ERROR : {ex . Message}" . cwerror ( );
                                    result = ex . Message;
                                    errormsg = result;
                                }
                                if ( errormsg == "" )
                                    errormsg = $"DYNAMIC:{fldcount}";
                                //                             $"Exiting with VALID result" . cwinfo();
                                return collection; //collection.Count;
                            }
                        }
                        else
                        {
                            // probably a stored procedure ?  							
                            bool IsSuccess = false;
                            int fldcount = 0;
                            GenericClass gc = new GenericClass ( );

                            //$"Entering for 'Stored Procedure'" . cwinfo ( );
                            //***************************************************************************************************************//
                            // This returns the data from SP commands (only) in a GenericClass Structured format
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
                                            // List<int> VarcharList = new List<int>();
                                            ParseDapperRow ( item , dict , out colcount );
                                            dictcount = 1;
                                            fldcount = dict . Count;
                                            if ( fldcount == 0 )
                                            {
                                                //no problem, we will get a Datatable anyway
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
                                                        //AddDictPairToGeneric(gc, pair, dictcount++);
                                                        tmp = pair . Key . ToString ( ) + $"= Field{index++}";// + pair.Value.ToString();
                                                        //tmp = pair.Key.ToString() + "=" + pair.Value.ToString();
                                                        buffer += tmp + ",";
                                                    }
                                                }
                                                catch ( Exception ex )
                                                {
                                                    //                                                $"Dictionary ERROR : {ex . Message}" . cwerror ( );
                                                    result = ex . Message;
                                                }
                                            }
                                            IsSuccess = true;
                                            string s = buffer . Substring ( 0 , buffer . Length - 1 );
                                            //                                            $"buffer = {s}" . CW ( );
                                            buffer = s;
                                            //genericlist.Add(buffer);
                                        }
                                        catch ( Exception ex )
                                        {
                                            //                                            $"SQLERROR : {ex . Message}" . cwerror ( );
                                            //$"Exiting with null" . cwwarn ( );
                                            return null;
                                        }
                                        //										gc . ActiveColumns = dict . Count;
                                        //ParseListToDbRecord ( genericlist , out gc );
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
                                        //                                      $"Exiting with null" . cwwarn ( );
                                        return null;
                                    }
                                    else
                                    {
                                        long x = reslt . LongCount ( );
                                        if ( x == ( long ) 0 )
                                        {
                                            result = $"ERROR : [{SqlCommand}] returned ZERO records... ";
                                            errormsg = $"DYNAMIC:0";
                                            //                                           $"Exiting with null" . cwwarn ( );
                                            return null;
                                        }
                                        else
                                        {
                                            result = ex . Message;
                                            errormsg = $"UNKNOWN :{ex . Message}";
                                        }
                                        //                                        $"Exiting with null" . cwwarn ( );
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
                                //$"Exiting with null" . cwwarn ( );
                                //return null;
                            }
                            //return 0;
                        }
                    }
                    catch ( Exception ex )
                    {
                        Debug . WriteLine ( $"STORED PROCEDURE ERROR : {ex . Message}" );
                        //$"STORED PROCEDURE ERROR : {ex . Message}" . cwerror ( );
                        result = ex . Message;
                        errormsg = $"SQLERROR : {result}";
                        //return null;
                    }
                }
                catch ( Exception ex )
                {
                    //                   $"Sql Error, {ex . Message}, {ex . Data}" . cwerror ( );
                    result = ex . Message;
                    //                   $"STORED PROCEDURE ERROR : {ex . Message}" . cwerror ( );
                }
            } // end using {} - MUST get here  to close connection correctly
              //            $"Exiting with null" . cwwarn ( );
            return null;
            //            return dict.Count;
        }

        public void BuildTableWithValidTypes ( )
        {
            string fname = "", ftype = "", intstring = "";
            string [ ] temp;
            int decsize = 0, decimalsize = 0;

            foreach ( var item in dglayoutlist )
            {
                fname = item . Fieldname;
                ftype = item . Fieldtype;
            }
        }

        public string [ ] CreateSqlSPArgs ( string commandline , string newdbname = "" )
        {
            string [ ] strings;
            strings = commandline . Split ( "," );
            //if(newdbname != "")
            //{
            string [ ] newstring = new string [ strings . Length + 2 ];
            newstring [ 0 ] = newdbname;
            foreach ( string item in strings )
            {
                Debug . WriteLine ( $"{item}" );
            }
            strings = newstring;
            //}
            Debug . WriteLine ( $"{strings}" );
            return strings;
        }
        //--------------------------------------------------------------------------------------------------------------------------------------------------------
        public int CreateGenericDbStoredProcedure ( string SqlCommand , string [ ] args , string ConString , out string err )
        //--------------------------------------------------------------------------------------------------------------------------------------------------------
        {
            //===============================================//
            // This version successfully creates a new Db as specified by the args[] string array
            //===============================================//
            int gresult = -1;
            string Con = ConString;
            SqlConnection sqlCon = null;
            err = "";

            try
            {
                using ( sqlCon = new SqlConnection ( Con ) )
                {
                    sqlCon . Open ( );
                    string [ ] str = SqlCommand . Split ( ' ' );
                    using ( var tran = sqlCon . BeginTransaction ( ) )
                    {
                        var parameters = new DynamicParameters ( );
                        if ( args . Length > 0 )
                        {
                            //Add the new table name as 1st argument parameter
                            string s = str [ 1 ];
                            parameters . Add ( $"Arg1" , s ,
                                DbType . String ,
                                ParameterDirection . Input ,
                                s . Length );
                            // add rest of arguments for fields in new table
                            for ( int x = 0 ; x < args . Length ; x++ )
                            {
                                string [ ] fields;
                                string fld = "", type = "";
                                fields = args [ x ] . Split ( ' ' );
                                fld = fields [ 0 ];
                                for ( int y = 0 ; y < fields . Length - 1 ; y++ )
                                    type += fields [ y ] + " ";
                                parameters . Add ( $"fld{x + 1}" , $"{type}" , DbType . String , ParameterDirection . Input , type . Length );
                            }
                        }
                        //Create the new table as requested
                        gresult = sqlCon . Execute ( "spCreateNewDbTable" , parameters , commandType: CommandType . StoredProcedure , transaction: tran );
                        // Commit the transaction
                        tran . Commit ( );
                        gresult = 1;
                    }
                }
            }
            catch ( Exception ex )
            {
                Debug . WriteLine ( $"Error {ex . Message}, {ex . Data}" );
                DapperGenericsLib . Utils . DoErrorBeep ( 230 , 100 , 1 );
                DapperGenericsLib . Utils . DoErrorBeep ( 160 , 200 , 1 );
                err = $"Error {ex . Message}";
            }
            //            WpfLib1 . Utils . trace ("CreateGenericDbStoredProcedure" );
            return gresult;
        }

        #region create newsmart Sql tables

        public List<GenericToRealStructure> CreateFullColumnInfo ( string spName , string ConString )
        {
            string output = "";
            string errormsg = "";
            int columncount = 0;
            string fname = "", ftype = "", intstring = "";
            List<GenericToRealStructure> newstructure = new List<GenericToRealStructure> ( );
            DataTable dt = new DataTable ( );
            ObservableCollection<GenericClass> Generics = new ObservableCollection<GenericClass> ( );
            List<string> genericlist = new List<string> ( );
            try
            {
                CreateGenericCollection (
                "spGetTableColumnWithSize" ,
                $"{spName}" ,
                "" ,
                "" ,
                 ref errormsg );
                dt = ProcessSqlCommand ( "spGetTableColumnWithSize  " + spName , ConString );
                if ( dt . Rows . Count == 0 )
                    columncount = 0;

                int decsize = 0, decpart = 0, decroot = 0;

                foreach ( var item in dt . Rows )
                {
                    GenericClass gc = new GenericClass ( );
                    string store = "";
                    DataRow dr = item as DataRow;
                    columncount = dr . ItemArray . Count ( );
                    if ( columncount == 0 )
                        columncount = 1;
                    // we only need max cols - 1 here !!!
                    for ( int x = 0 ; x < columncount ; x++ )
                        store += dr . ItemArray [ x ] . ToString ( ) + ",";
                    output += store;
                }
                // we now have the result, so lets process them
                // data is fieldname, sql-datatype, size (where appropriate)
                string buffer = output;
                string [ ] lines = buffer . Split ( ',' );
                output = "";
                int counter = 0;
                string type = "";
                string tmp = "";
                foreach ( var item in lines )
                {
                    switch ( counter )
                    {
                        //----------------------------------------//
                        case 0:     //field name
                            fname = $"{item}";
                            break;
                        //----------------------------------------//
                        case 1: //field type
                            ftype = $"{item}";
                            break;
                        //----------------------------------------//
                        case 2: // size (1)
                            if ( ftype == "varchar" || ftype == "nvarchar" )
                            {
                                decroot = Convert . ToInt16 ( item );
                            }
                            break;
                        //----------------------------------------//
                        case 3: // Size 2 (Decimal root)
                            if ( ftype == "int" )
                            {
                                if ( item != "" ) decroot = Convert . ToInt16 ( item );
                                else decroot = 0;
                            }
                            else if ( ftype == "decimal" )
                            {
                                if ( item != "" )
                                    decroot = Convert . ToInt16 ( item );
                                else
                                    decroot = 0;
                            }
                            break;
                        case 4: // Size 3 (decimal Radix)
                            if ( ftype == "decimal" || ftype == "float" )
                            {
                                decpart = Convert . ToInt32 ( item );
                            }
                            // else decimalsize = 0;
                            // We now have all the data types for this column
                            // Add data  to our storage structure
                            GenericToRealStructure schema = new GenericToRealStructure ( );
                            schema . fname = fname;
                            schema . ftype = ftype;
                            if ( decpart != 0 )
                                schema . decpart = decpart;
                            if ( decroot != 0 )
                                schema . decroot = decroot;

                            newstructure . Add ( schema );
                            // cleanup ready for next column
                            fname = "";
                            ftype = "";
                            decsize = 0;
                            decroot = 0;
                            decpart = 0;
                            break;

                        //----------------------------------------//
                        default:
                            counter = 0;
                            break;
                    }
                    if ( counter < 4 )
                        counter++;
                    else
                        counter = 0;

                    //                   output = output . Substring ( 0 , output . Length - 1 );
                    // we now have a list of the Args for the selected SP in output
                    // Show it in a TextBox if it takes 1 or more args
                    // format is ("fielddname, fieldtype, size1, size2\n,")
                }
                //       ShowLoadtime ( );
                return newstructure;
            }
            catch ( Exception ex )
            {
                MessageBox . Show ( $"SQL ERROR 1125 - {ex . Message}" );
                return null;
            }
        }
        public static string CreateSqlCommand ( List<GenericToRealStructure> TableStructure , string newDbName , out string [ ] args )
        {
            string line = "";
            string [ ] strings = new string [ TableStructure . Count * 20 ];
            string output = "";
            string fields = "";
            int index = 1;
            int arrayindex = 0;
            output = $" {newDbName} (";
            // strings [ arrayindex++ ] = $"{newDbName}";
            //foreach ( var item in TableStructure )
            //{
            //    fields += $"{item . fname}, ";
            //    strings [ arrayindex++ ] = item . fname;
            //}
            //output += fields . Substring ( 0 , fields . Length - 2 );
            foreach ( var item in TableStructure )
            {
                if ( item . fname . ToUpper ( ) == "ID" )
                {
                    line += $"{item . fname} {item . ftype} IDENTITY(1,1) NOT NULL ";
                    strings [ arrayindex++ ] = line;
                }
                else
                {
                    line += $"{item . fname} {item . ftype}";
                    if ( item . ftype == "int" )
                        line += $" NULL ";
                    else if ( item . ftype == "decimal" || item . ftype == "double" )
                        line += $"({item . decroot},{item . decpart}) NULL ";
                    else if ( item . ftype == "nvarchar" )
                        line += $"({item . decroot}) NULL ";
                    else
                        line += $" NULL ";
                    strings [ arrayindex++ ] = line;
                }
                output += line;
                index++;
                line = "";
            }
            output = output . Trim ( );
            output += ")";
            int validargs = 0;
            foreach ( string item in strings )
            {
                if ( item == null )
                    break;
                validargs++;
            }
            string [ ] newargs = new string [ validargs ];

            for ( int x = 0 ; x < validargs ; x++ )
            {
                newargs [ x ] = strings [ x ];
            }
            args = newargs;
            return output;
        }

        public static DataTable GetDataTable ( string commandline , string connstring )
        {
            //        $"Entering " . cwinfo();
            DataTable dt = new DataTable ( );
            try
            {
                SqlConnection con;
                string ConString = connstring;
                if ( ConString == "" )
                {
                    CheckDbDomain ( "IAN1" );
                    ConString = connstring;
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
            //        $"Exiting " . cwinfo();
            return dt;
        }

        #endregion

        public void GetSqlData<T> ( string table , string constring )
        {
            DataTable dt = GetDataTable ( $@"SELECT * FROM {table}" , constring );

            ObservableCollection<T> data2 = new ObservableCollection<T> ( );
            // process dt to our collection
            foreach ( DataRow row in dt . Rows )
            {
                GenericClass clas = new GenericClass ( );
            }
            //connection . Open ( );
            //var query = $@"SELECT * FROM {table}";
            ////                IEnumerable enumItem = data . GetEnumerator ( );
            ////IEnumerable<DataRow> data = from BankAccount in ;
            //dynamic data = connection . Query<GenericClass> ( @"Select * from bankaccount");
            //IEnumerator enumItem = data. GetEnumerator ( );
            //while ( enumItem . MoveNext ( ) )
            //{
            //    var  dat = enumItem . Current as GenericClass;
            //    data2 . Add ( dat);
            //    Debug . WriteLine ( $"{ enumItem . Current}");
            //}
            //<GenericClass> data2 = new ObservableCollection<ObservableCollection<GenericClass>> ( data );
            // int count = data . Count ( );
            //while (MoveNext())
            //{
            //    data2 . Add ( data[x] as ObservableCollection<GenericClass> );
            //}
            //            }
            //return data;
        }

        //*********************************//        
        #region table creation        //*********************************//        
        public int CreateTableAsync ( string NewTableName , List<GenericToRealStructure> TableStruct )
        {
            string NewDbName = NewTableName . Trim ( );
            List<GenericToRealStructure> TableStructure = new List<GenericToRealStructure> ( );
            if ( NewDbName == "Enter New Table Name ...." )
            {
                MessageBox . Show ( "Please provide a name for the new table in the field provided..." , "New Table name required" );
                //NewTableName . Focus ( );
                return -2;
            }
            if ( NewDbName == "" )
            {
                MessageBox . Show ( "Please enter a suitable name for the table you want to create !" , "Naming Error" );
                return -2;
            }

            GetFullColumnInfo ( DatagridControl . CurrentTable , DbConnectionString , false );
            if ( TableStruct == null )
                TableStructure = CreateFullColumnInfo ( DatagridControl . CurrentTable , DbConnectionString );
            else
            {
                string error = "";
                CreateLimitedTableAsync ( NewTableName , TableStruct, out error );
                if(error != "")
                    Debug . WriteLine ($"ERROR : {error}");
                return 1;
            }
            // We now have a full SQl Structure for the current table in TableStructure
            // Sort out  our  new table structure
            string [ ] Sqlargs;
            int gresult = -1;
            string commandline = "";
            commandline = CreateSqlCommand ( TableStructure , NewDbName , out Sqlargs );
            // Now we have got a fully formatted SqlCommand and the necessary arguments using the special CreateGenericDbStoredProcedure S.P.
            try
            {
                // Create the new table in current Db
                if ( CreateGenericDbStoredProcedure ( $"spCREATENEWDBTABLE {NewDbName} " , Sqlargs , DbConnectionString , out string err ) == 1 )
                {
                    //Table creates successfuilly, so Copy data to new table
                    string temp = "";
                    List<string> datavalues = new List<string> ( );
                    int rangecount = TableStructure . Count;
                    int datastartvalue = 0, y = 0, x = 0, itemscount = 0;
                    string Con = DbConnectionString;
                    err = "";
                    gresult = -1;
                    SqlConnection sqlCon = null;
                    Mouse . OverrideCursor = Cursors . Wait;
                    Debug . WriteLine ( "running Task" );
                    //                    Task . Run ( ( ) =>
                    //                {
                    using ( sqlCon = new SqlConnection ( Con ) )
                    {
                        sqlCon . Open ( );

                        foreach ( GenericClass item in GridData )
                        {
                            itemscount = 0;
                            string SqlInsertCommand = $"Insert into {NewDbName} (";
                            Console . Write ( "." );
                            for ( x = 0 ; x < TableStructure . Count ; x++ )
                            {
                                switch ( x + 1 )
                                {
                                    case 1:
                                        if ( TableStructure [ x ] . fname . ToUpper ( ) != "ID" )
                                        {
                                            if ( TableStructure [ x ] . ftype . ToUpper ( ) == "DATETIME" )
                                                datavalues . Add ( ConvertToUsSqlDate ( item . field1 ) );
                                            else
                                                datavalues . Add ( item . field1 );
                                            itemscount++;
                                        }
                                        break;
                                    case 2:
                                        if ( TableStructure [ x ] . ftype . ToUpper ( ) == "DATETIME" )
                                            datavalues . Add ( ConvertToUsSqlDate ( item . field2 ) );
                                        else
                                            datavalues . Add ( item . field2 );
                                        itemscount++;
                                        break;
                                    case 3:
                                        if ( TableStructure [ x ] . ftype . ToUpper ( ) == "DATETIME" )
                                            datavalues . Add ( ConvertToUsSqlDate ( item . field3 ) );
                                        else
                                            datavalues . Add ( item . field3 );
                                        itemscount++;
                                        break;
                                    case 4:
                                        if ( TableStructure [ x ] . ftype . ToUpper ( ) == "DATETIME" )
                                            datavalues . Add ( ConvertToUsSqlDate ( item . field4 ) );
                                        else
                                            datavalues . Add ( item . field4 );
                                        itemscount++;
                                        break;
                                    case 5:
                                        if ( TableStructure [ x ] . ftype . ToUpper ( ) == "DATETIME" )
                                            datavalues . Add ( ConvertToUsSqlDate ( item . field5 ) );
                                        else
                                            datavalues . Add ( item . field5 );
                                        itemscount++;
                                        break;
                                    case 6:
                                        if ( TableStructure [ x ] . ftype . ToUpper ( ) == "DATETIME" )
                                            datavalues . Add ( ConvertToUsSqlDate ( item . field6 ) );
                                        else
                                            datavalues . Add ( item . field6 );
                                        itemscount++;
                                        break;
                                    case 7:
                                        if ( TableStructure [ x ] . ftype . ToUpper ( ) == "DATETIME" )
                                            datavalues . Add ( ConvertToUsSqlDate ( item . field7 ) );
                                        else
                                            datavalues . Add ( item . field7 );
                                        itemscount++;
                                        break;
                                    case 8:
                                        if ( TableStructure [ x ] . ftype . ToUpper ( ) == "DATETIME" )
                                            datavalues . Add ( ConvertToUsSqlDate ( item . field8 ) );
                                        else
                                            datavalues . Add ( item . field8 );
                                        itemscount++;
                                        break;
                                    case 9:
                                        if ( TableStructure [ x ] . ftype . ToUpper ( ) == "DATETIME" )
                                            datavalues . Add ( ConvertToUsSqlDate ( item . field9 ) );
                                        else
                                            datavalues . Add ( item . field9 );
                                        itemscount++;
                                        break;
                                    case 10:
                                        if ( TableStructure [ x ] . ftype . ToUpper ( ) == "DATETIME" )
                                            datavalues . Add ( ConvertToUsSqlDate ( item . field10 ) );
                                        else
                                            datavalues . Add ( item . field10 );
                                        itemscount++;
                                        break;
                                    case 11:
                                        if ( TableStructure [ x ] . ftype . ToUpper ( ) == "DATETIME" )
                                            datavalues . Add ( ConvertToUsSqlDate ( item . field11 ) );
                                        else
                                            datavalues . Add ( item . field11 );
                                        itemscount++;
                                        break;
                                    case 12:
                                        if ( TableStructure [ x ] . ftype . ToUpper ( ) == "DATETIME" )
                                            datavalues . Add ( ConvertToUsSqlDate ( item . field12 ) );
                                        else
                                            datavalues . Add ( item . field12 );
                                        itemscount++;
                                        break;
                                    case 13:
                                        if ( TableStructure [ x ] . ftype . ToUpper ( ) == "DATETIME" )
                                            datavalues . Add ( ConvertToUsSqlDate ( item . field13 ) );
                                        else
                                            datavalues . Add ( item . field13 );
                                        itemscount++;
                                        break;
                                    case 14:
                                        if ( TableStructure [ x ] . ftype . ToUpper ( ) == "DATETIME" )
                                            datavalues . Add ( ConvertToUsSqlDate ( item . field14 ) );
                                        else
                                            datavalues . Add ( item . field14 );
                                        itemscount++;
                                        break;
                                    case 15:
                                        if ( TableStructure [ x ] . ftype . ToUpper ( ) == "DATETIME" )
                                            datavalues . Add ( ConvertToUsSqlDate ( item . field15 ) );
                                        else
                                            datavalues . Add ( item . field15 );
                                        itemscount++;
                                        break;
                                    case 16:
                                        if ( TableStructure [ x ] . ftype . ToUpper ( ) == "DATETIME" )
                                            datavalues . Add ( ConvertToUsSqlDate ( item . field16 ) );
                                        else
                                            datavalues . Add ( item . field16 );
                                        itemscount++;
                                        break;
                                    case 17:
                                        if ( TableStructure [ x ] . ftype . ToUpper ( ) == "DATETIME" )
                                            datavalues . Add ( ConvertToUsSqlDate ( item . field17 ) );
                                        else
                                            datavalues . Add ( item . field17 );
                                        itemscount++;
                                        break;
                                    case 18:
                                        if ( TableStructure [ x ] . ftype . ToUpper ( ) == "DATETIME" )
                                            datavalues . Add ( ConvertToUsSqlDate ( item . field18 ) );
                                        else
                                            datavalues . Add ( item . field18 );
                                        itemscount++;
                                        break;
                                    case 19:
                                        if ( TableStructure [ x ] . ftype . ToUpper ( ) == "DATETIME" )
                                            datavalues . Add ( ConvertToUsSqlDate ( item . field19 ) );
                                        else
                                            datavalues . Add ( item . field19 );
                                        itemscount++;
                                        break;
                                    case 20:
                                        if ( TableStructure [ x ] . ftype . ToUpper ( ) == "DATETIME" )
                                            datavalues . Add ( ConvertToUsSqlDate ( item . field20 ) );
                                        else
                                            datavalues . Add ( item . field20 );
                                        itemscount++;
                                        break;
                                    default:
                                        break;

                                }
                                if ( TableStructure [ x ] . fname . ToUpper ( ) != "ID" )
                                {
                                    if ( TableStructure [ x ] . ftype . ToUpper ( ) == "DATETIME" )
                                        SqlInsertCommand += $"{TableStructure [ x ] . fname}, ";
                                    else
                                        SqlInsertCommand += $"{TableStructure [ x ] . fname}, ";
                                }
                            }
                            SqlInsertCommand = SqlInsertCommand . Trim ( );
                            SqlInsertCommand = NewWpfDev . Utils . ReverseString ( SqlInsertCommand );
                            if ( SqlInsertCommand [ 0 ] == ',' )
                            {
                                SqlInsertCommand = SqlInsertCommand . Substring ( 1 ) . Trim ( );
                                SqlInsertCommand = NewWpfDev . Utils . ReverseString ( SqlInsertCommand );
                            }
                            SqlInsertCommand += $") values (";
                            for ( y = datastartvalue ; y < datastartvalue + itemscount ; y++ )
                            {
                                SqlInsertCommand += $" {datavalues [ y ]}, ";
                            }
                            datastartvalue = y;
                            SqlInsertCommand = SqlInsertCommand . Trim ( );
                            SqlInsertCommand = SqlInsertCommand . Substring ( 0 , SqlInsertCommand . Length - 1 ) . Trim ( );
                            SqlInsertCommand += $" ) ";
                            itemscount = 0;

                            // Now add record  to SQL table
                            var parameters = new DynamicParameters ( );
                            parameters . Add ( $"Arg1" , SqlInsertCommand ,
                                DbType . String ,
                                ParameterDirection . Input ,
                                SqlInsertCommand . Length );
                            string cmd = $"spExecuteFullStoredProcedureCommand";
                            gresult = sqlCon . Execute ( cmd , parameters , commandType: CommandType . StoredProcedure );
                            gresult = 1; SqlInsertCommand = "";
                        }   //foreach
                    }   // using
                        //                } );
                }   // if
            }   // try
            catch ( Exception ex )
            {
                Debug . WriteLine ( $"Error {ex . Message}, {ex . Data}" );
                $" {ex . Message}, {ex . Data}" . dcwerror ( );
                DapperGenericsLib . Utils . DoErrorBeep ( 280 , 100 , 1 );
                DapperGenericsLib . Utils . DoErrorBeep ( 190 , 200 , 1 );
                gresult = -3;
            }
            Mouse . OverrideCursor = Cursors . Arrow;
            return gresult;
        }
        public ObservableCollection<GenericClass> CreateLimitedTableAsync ( string NewTableName , List<GenericToRealStructure> TableStruct, out string error )
        {
            // We now have a full SQl Structure for the current table in TableStructure
            // Sort out  our  new table structure
            string [ ] Sqlargs;
            int gresult = -1;
            bool ColumnMissed = false;
            string commandline = "";
            error = "";
            ObservableCollection<GenericClass> LimitedColumnTable = new ObservableCollection<GenericClass> ( );
            commandline = CreateSqlCommand ( TableStruct , NewTableName , out Sqlargs );
            // Now we have got a fully formatted SqlCommand and the necessary arguments using the special CreateGenericDbStoredProcedure S.P.
            try
            {
                // Create the new table in current Db
                if ( CreateGenericDbStoredProcedure ( $"spCREATENEWDBTABLE {NewTableName} " , Sqlargs , DbConnectionString , out string err ) == 1 )
                {
                    //Table creates successfuilly, so Copy data  from selected columns only to new table
                    string temp = "";
                    List<string> datavalues = new List<string> ( );
                    int rangecount = TableStruct . Count;
                    int datastartvalue = 0, y = 0, x = 0, itemscount = 0;
                    string Con = DbConnectionString;                   
                    gresult = -1;
                    SqlConnection sqlCon = null;
                    Mouse . OverrideCursor = Cursors . Wait;
                    Debug . WriteLine ( "Loading data from current table to new 'Columns only' table" );
                    // We have a new EMPTY table, so add data from correct columns only
                    foreach ( GenericClass item in GridData )
                    {
                        try
                        {
                            while ( true )
                            {
                                if ( y >= TableStruct . Count )
                                {
                                    // Add new "selected columns"  record to temp table
                                    AddColumnToTempTable ( datavalues , LimitedColumnTable );
                                    datavalues . Clear ( );
                                    y = 0;
                                    break;
                                }
                                if ( TableStruct [ y ] . colindex > 19 )
                                {
                                    y++;
                                    ColumnMissed = true;
                                    error = $"Unable to handle column {TableStruct [ y ] . colindex}, Max selected column # is 19";
                                    continue;
                                }//Debug . WriteLine ( $"y={y} : {TableStruct [y].fname}, {TableStruct [y].ftype}, {TableStruct [y].decroot},{TableStruct [y].decpart}, {TableStruct [ y ].colindex}" );
                               // Debug . WriteLine ( $"data = {item . field1}, {item . field2}, {item . field3}, {item . field4} " );
                                if ( TableStruct [ y ] . colindex == 0 )
                                {
                                    datavalues . Add ( item . field1 ); y++; continue;
                                }
                                if ( TableStruct [ y ] . colindex == 1 )
                                {
                                    datavalues . Add ( item . field2 ); y++; continue;
                                }
                                if ( TableStruct [ y ] . colindex == 2 )
                                {
                                    datavalues . Add ( item . field3 ); y++; continue;
                                }
                                if ( TableStruct [ y ] . colindex == 3 )
                                {
                                    datavalues . Add ( item . field4 ); y++; continue;
                                }
                                if ( TableStruct [ y ] . colindex == 4 )
                                {
                                    datavalues . Add ( item . field5 ); y++; continue;
                                }
                                if ( TableStruct [ y ] . colindex == 5 )
                                {
                                    datavalues . Add ( item . field6 ); y++; continue;
                                }
                                if ( TableStruct [ y ] . colindex == 6 )
                                {
                                    datavalues . Add ( item . field7 ); y++; continue;
                                }
                                if ( TableStruct [ y ] . colindex == 7 )
                                {
                                    datavalues . Add ( item . field8 ); y++; continue;
                                }
                                if ( TableStruct [ y ] . colindex == 8 )
                                {
                                    datavalues . Add ( item . field9 ); y++; continue;
                                }
                                if ( TableStruct [ y ] . colindex == 9 )
                                {
                                    datavalues . Add ( item . field10 ); y++; continue;
                                }
                                if ( TableStruct [ y ] . colindex == 10 )
                                {
                                    datavalues . Add ( item . field11 ); y++; continue;
                                }
                                if ( TableStruct [ y ] . colindex == 11 )
                                {
                                    datavalues . Add ( item . field12 ); y++; continue;
                                }
                                if ( TableStruct [ y ] . colindex == 12 )
                                {
                                    datavalues . Add ( item . field13 ); y++; continue;
                                }
                                if ( TableStruct [ y ] . colindex == 13 )
                                {
                                    datavalues . Add ( item . field14 ); y++; continue;
                                }
                                if ( TableStruct [ y ] . colindex == 14 )
                                {
                                    datavalues . Add ( item . field15 ); y++; continue;
                                }
                                if ( TableStruct [ y ] . colindex == 15 )
                                {
                                    datavalues . Add ( item . field16 ); y++; continue;
                                }
                                if ( TableStruct [ y ] . colindex == 16 )
                                {
                                    datavalues . Add ( item . field17 ); y++; continue;
                                }
                                if ( TableStruct [ y ] . colindex == 17 )
                                {
                                    datavalues . Add ( item . field18 ); y++; continue;
                                }
                                if ( TableStruct [ y ] . colindex == 18 )
                                {
                                    datavalues . Add ( item . field19 ); y++; continue;
                                }
                                if ( TableStruct [ y ] . colindex == 19 )
                                {
                                    datavalues . Add ( item . field20 ); y++; continue;
                                }
                            }
                        }
                        catch ( Exception ex )
                        {
                            Debug . WriteLine ( $"{ex . Message}, {ex . Data}, y={y}, columns count={TableStruct . Count}" );
                            error = $"Table creation failed... Reason = [{ex.Message}]";
                            return null;
                        }
                    }
                    Debug . WriteLine ($"Loaded {LimitedColumnTable.Count} records into LimitedColumnTable." );
                    return LimitedColumnTable;
                }
                error = "Table creation failed...";
                return null;
            }
            catch ( Exception ex )
            {
                Debug . WriteLine ( $"{ex . Message}, {ex . Data}" );
                Mouse . OverrideCursor = Cursors . Arrow;
                return null;
            }
        }
        public void AddColumnToTempTable ( List<string> datavalues , ObservableCollection<GenericClass> LimitedColumnTable )
        {
            GenericClass tempclass = new GenericClass ( );

            if ( datavalues . Count >= 20 )
                tempclass . field20 = datavalues [ 19 ];
            if ( datavalues . Count >= 19 )
                tempclass . field19 = datavalues [ 18 ];
            if ( datavalues . Count >= 18 )
                tempclass . field18 = datavalues [ 17 ];
            if ( datavalues . Count >= 17 )
                tempclass . field17 = datavalues [ 16 ];
            if ( datavalues . Count >= 16 )
                tempclass . field16 = datavalues [ 15 ];
            if ( datavalues . Count >= 15 )
                tempclass . field15 = datavalues [ 14 ];
            if ( datavalues . Count >= 14 )
                tempclass . field14 = datavalues [ 13 ];
            if ( datavalues . Count >= 13 )
                tempclass . field13 = datavalues [ 12 ];
            if ( datavalues . Count >= 12 )
                tempclass . field11 = datavalues [ 10 ];
            if ( datavalues . Count >= 11 )
                tempclass . field12 = datavalues [ 10 ];
            if ( datavalues . Count >= 10 )
                tempclass . field10 = datavalues [ 9 ];
            if ( datavalues . Count >= 9 )
                tempclass . field9 = datavalues [ 8 ];
            if ( datavalues . Count >= 8 )
                tempclass . field8 = datavalues [ 7 ];
            if ( datavalues . Count >= 7 )
                tempclass . field7 = datavalues [ 6 ];
            if ( datavalues . Count >= 6 )
                tempclass . field6 = datavalues [ 5 ];
            if ( datavalues . Count >= 5 )
                tempclass . field5 = datavalues [ 4 ];
            if ( datavalues . Count >= 4 )
                tempclass . field4 = datavalues [ 3 ];
            if ( datavalues . Count >= 3 )
                tempclass . field3 = datavalues [ 2 ];
            if ( datavalues . Count >= 2 )
                tempclass . field2 = datavalues [ 1 ];
            if ( datavalues . Count >= 1 )
                tempclass . field1 = datavalues [ 0 ];



            LimitedColumnTable . Add ( tempclass );
        }
        //*********************************//        
        #endregion table creation        //*********************************//


        //*********************************//        
        #region utility support
        //*********************************//        

        public static string ConvertToUsSqlDate ( string dateToConvert )
        {
            string output = "";
            string [ ] items = dateToConvert . Split ( '/' );
            output = $"'{items [ 1 ]}/{items [ 0 ]}/{items [ 2 ]}'";
            return output;
        }
        public static void CheckDbDomain ( string DbDomain )
        {
            $"Entering " . cwinfo ( 0 );
            if ( DapperGenLib . ConnectionStringsDict == null || DapperGenLib . ConnectionStringsDict . Count == 0 )
                LoadConnectionStrings ( );
            CheckResetDbConnection ( DbDomain , out string constring );
            DapperGenLib . CurrentConnectionString = constring;
            $"Exiting " . cwinfo ( 0 );
        }

        public static void LoadConnectionStrings ( )
        {
            // This one works just fine - its in NewWpfDev
            $"Entering " . cwinfo ( 0 );
            Settings defaultInstance = ( ( Settings ) ( global::System . Configuration . ApplicationSettingsBase . Synchronized ( new Settings ( ) ) ) );
            try
            {
                if ( DapperGenLib . ConnectionStringsDict . Count > 0 )
                    return;
                DapperGenLib . ConnectionStringsDict . Add ( "IAN1" , ( string ) Settings . Default [ "BankSysConnectionString" ] );
                DapperGenLib . ConnectionStringsDict . Add ( "NORTHWIND" , ( string ) Settings . Default [ "NorthwindConnectionString" ] );
                DapperGenLib . ConnectionStringsDict . Add ( "PUBS" , ( string ) Settings . Default [ "PubsConnectionString" ] );
                // TODO
                //WpfLib1.Utils.WriteSerializedCollectionJSON(Flags.ConnectionStringsDict, @"C:\users\ianch\DbConnectionstrings.dat");
            }
            catch ( NullReferenceException ex )
            {
                Debug . WriteLine ( $"Dictionary  entrry [{( string ) Settings . Default [ "BankSysConnectionString" ]}] already exists" );
            }
            finally
            {

            }
            $"Exiting " . cwinfo ( 0 );
        }
        public static bool CheckResetDbConnection ( string currdb , out string constring )
        {
            //string constring = "";
            $"Entering " . cwinfo ( 0 );
            currdb?.ToUpper ( );
            // This resets the current database connection to the one we re working with (currdb - in UPPER Case!)- should be used anywhere that We switch between databases in Sql Server
            // It also sets the Flags.CurrentConnectionString - Current Connectionstring  and local variable
            if ( GetDictionaryEntry ( DapperGenLib . ConnectionStringsDict , currdb , out string connstring ) != "" )
            {
                if ( connstring != null )
                {
                    DapperGenLib . CurrentConnectionString = connstring;
                    SqlConnection con;
                    con = new SqlConnection ( DapperGenLib . CurrentConnectionString );
                    if ( con != null )
                    {
                        //test it
                        constring = connstring;
                        con . Close ( );
                        $"Exiting " . cwinfo ( 0 );
                        return true;
                    }
                    else
                    {
                        constring = connstring;
                        $"Exiting with error" . cwwarn ( );
                        return false;
                    }
                }
                else
                {
                    constring = "";
                    $"Exiting with error " . cwwarn ( );
                    return false;
                }
            }
            else
            {
                constring = "";
                $"Exiting with error" . cwwarn ( );
                return false;
            }
        }
        public static string GetDictionaryEntry ( Dictionary<string , string> dict , string key , out string dictvalue )
        {
            string keyval = "";

            if ( dict . Count == 0 )
                NewWpfDev . Utils . LoadConnectionStrings ( );
            if ( dict . TryGetValue ( key . ToUpper ( ) , out keyval ) == false )
            {
                if ( dict . TryGetValue ( key , out keyval ) == false )
                {
                    Debug . WriteLine ( $"Unable to access Dictionary {dict} to identify key value [{key}]" );
                    key = key + "ConnectionString";
                }
            }
            dictvalue = keyval;
            return keyval;
        }

        //*********************************//        
        #endregion utility support
        //*********************************//        

        public DataTable GetDataTable ( string commandline )
        {
            DataTable dt = new DataTable ( );
            try
            {
                SqlConnection con;
                string ConString = DbConnectionString;
                if ( ConString == "" )
                {
                    //GenericDbUtilities<GenericClass> . CheckDbDomain ( "IAN1" );
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

        public int ProcessUniversalStoredProcedure ( string spCommand , string [ ] args , out string err )
        {
            int result = -1;
            string Con = DbConnectionString;
            SqlConnection sqlCon = null;

           //RunStoredProc ( );


            Mouse . OverrideCursor = Cursors . Wait;
            Debug . WriteLine ( $"Running Stored Procedure {spCommand}" );
            using ( sqlCon = new SqlConnection ( Con ) )
            {
                string cmd = "";
                sqlCon . Open ( );
                // Now add record  to SQL table
                var parameters = new DynamicParameters ( );
                if ( args . Length > 0 && args [ 0 ] != "-" )
                {
                    for ( int x = 0 ; x < args . Length ; x++ )
                    {
                        // breakout on first unused array element
                        if ( args [ x ] == "-" ) break;

                        parameters . Add ( $"Arg{x + 1}" , args [ x ] ,
                        DbType . String ,
                        ParameterDirection . Input ,
                        args [ x ] . Length );
                    }
                }
                err = "";
                try
                {
                    result = sqlCon . Execute ( spCommand , parameters , commandType: CommandType . StoredProcedure );
                    Debug . WriteLine ( $"RESULT = {result}" );
                }
                catch ( Exception ex )
                {
                    Debug . WriteLine ( $"{ex . Message}, {ex . Data}" );
                    result = -1;
                    err = ex . Message;
                }
            }
            Mouse . OverrideCursor = Cursors . Arrow;
            return result;
        }
        //****************************************************************//
        public int RunStoredProc ( )
        {
            string connectionString = DbConnectionString;
            using ( SqlConnection conn = new SqlConnection ( connectionString ) )
            {
                using ( SqlCommand cmd = new SqlCommand ( "GetTableFieldsList" ) )
                {
                    cmd . CommandType = CommandType . StoredProcedure;
                    cmd . Parameters . Add ( new SqlParameter ( "@Arg1" , "BANKACCOUNT" ) );
                    SqlParameter Data = new SqlParameter ( "@Output" , DbType . String );
                    Data . Direction = ParameterDirection . Output;
                    cmd . Parameters . Add ( Data );

                    conn . Open ( );
                    cmd . Connection = conn;

                    int res = cmd . ExecuteNonQuery ( );
                    string output = cmd . Parameters [ "@Output" ] . Value . ToString ( );
                    Debug . WriteLine ( $"{output}" );
                    conn . Close ( );
                }
            }
            return 0;
        }


    }
}
