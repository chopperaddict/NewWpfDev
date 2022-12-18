using System;
using System . Collections;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Data;
using System . Data . SqlClient;
using System . Diagnostics;
using System . Linq;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;
using System . Windows . Media;

using Dapper;

using DapperGenericsLib;

//using GenericSqlLib . Dapper;

using NewWpfDev;
using NewWpfDev . Dapper;
using NewWpfDev . Models;
using NewWpfDev . UserControls;
using NewWpfDev . Views;

using Views;

using static Dapper . SqlMapper;

using GenericClass = NewWpfDev . GenericClass;
using Utils = DapperGenericsLib . Utils;

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
        public List<DapperGenericsLib . DataGridLayout> dglayoutlist = new List<DapperGenericsLib . DataGridLayout> ( );
        public DataGrid Dgrid;
        public static string CurrentTableDomain { get; set; }
        public static string SqlOperationString { get; set; }

        public static Window ParentWin
        {
            get; set;
        }
        public FlowdocLib fdl;
        public static string CurrentTable
        {
            get; set;
        }
        public static Control ParentCtrl
        {
            get; set;
        }

        private bool dataLoaded;
        public bool DataLoaded
        {
            get { return dataLoaded; }
            set { dataLoaded = value; }
        }
        private static string domain;
        public string Domain
        {
            get { return domain; }
            set { domain = value; }
        }

        //        public static string staticDomain = domain;

        //Flowdoc declarations
        private double XLeft = 0;
        private double YTop = 0;
        private bool UseFlowdoc = true;
        public static object MovingObject
        {
            get; set;
        }

        public static bool ConvertDateTimeToNvarchar { get; set; } = false;
        //        public static string DbConnectionString = "Data Source=DINO-PC;Initial Catalog=\"IAN1\";Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public static string DbConnectionString = "";

        // collection identical to ObservableCollection<GenericClass>
        //        public ObsCollections collection = new ObsCollections ();
        public static ObservableCollection<GenericClass> TableColumnsCollection
        {
            get; set;
        }
        public static ObservableCollection<GenericClass> GridData
        {
            get; set;
        }
        #endregion

        public DatagridControl ( )
        {
            InitializeComponent ( );
            Dgrid = datagridControl;
            CurrentTableDomain = "IAN1";
            DatagridControl . CurrentTableDomain = CurrentTableDomain;
            GridData = new ObservableCollection<GenericClass> ( );
            MainWindow . SqlCurrentConstring = CurrentTableDomain;
            fdl = new FlowdocLib ( Flowdoc , canvas );
            Flowdoc . ExecuteFlowDocMaxmizeMethod += new EventHandler ( MaximizeFlowDoc );
            FlowDoc . FlowDocClosed += Flowdoc_FlowDocClosed;
            GenericGridSupport . SetPointers ( this , null );
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
            get
            {
                return ( Brush ) GetValue ( AlternateBackgroundProperty );
            }
            set
            {
                SetValue ( AlternateBackgroundProperty , value );
            }
        }
        public static readonly DependencyProperty AlternateBackgroundProperty =
            DependencyProperty . Register ( "AlternateBackground" , typeof ( Brush ) , typeof ( DatagridControl ) , new PropertyMetadata ( Brushes . Yellow ) );
        /**************************************************************************************************************/
        public Style Cellstyle
        {
            get
            {
                return ( Style ) GetValue ( CellstyleProperty );
            }
            set
            {
                SetValue ( CellstyleProperty , value );
            }
        }
        public static readonly DependencyProperty CellstyleProperty =
            DependencyProperty . Register ( "Cellstyle" , typeof ( Style ) , typeof ( DatagridControl ) , new PropertyMetadata ( default ) );
        /**************************************************************************************************************/
        public ObservableCollection<GenericClass> Data
        {
            get
            {
                return ( ObservableCollection<GenericClass> ) GetValue ( DataProperty );
            }
            set
            {
                SetValue ( DataProperty , value );
            }
        }
        public static readonly DependencyProperty DataProperty =
            DependencyProperty . Register ( "Data" ,
                typeof ( ObservableCollection<GenericClass> ) ,
                typeof ( DatagridControl ) ,
                new PropertyMetadata ( ( ObservableCollection<GenericClass> ) null ) );
        /**************************************************************************************************************/
        public string Tablename
        {
            get
            {
                return ( string ) GetValue ( TablenameProperty );
            }
            set
            {
                SetValue ( TablenameProperty , value );
            }
        }
        public static readonly DependencyProperty TablenameProperty =
            DependencyProperty . Register ( "Tablename" , typeof ( string ) , typeof ( DatagridControl ) , new PropertyMetadata ( "" ) );
        /**************************************************************************************************************/
        public int Selection
        {
            get
            {
                return ( int ) GetValue ( SelectionProperty );
            }
            set
            {
                SetValue ( SelectionProperty , value );
            }
        }
        public static readonly DependencyProperty SelectionProperty =
            DependencyProperty . Register ( "Selection" , typeof ( int ) , typeof ( DatagridControl ) , new PropertyMetadata ( ( int ) 0 ) );
        /**************************************************************************************************************/
        public DataTemplate GridDataTemplate
        {
            get
            {
                return ( DataTemplate ) GetValue ( GridDataTemplateProperty );
            }
            set
            {
                SetValue ( GridDataTemplateProperty , value );
            }
        }
        public static readonly DependencyProperty GridDataTemplateProperty =
            DependencyProperty . Register ( "GridDataTemplate" , typeof ( DataTemplate ) , typeof ( DatagridControl ) , new PropertyMetadata ( default ) );
        /**************************************************************************************************************/
        public Style GridStyle
        {
            get
            {
                return ( Style ) GetValue ( GridStyleProperty );
            }
            set
            {
                SetValue ( GridStyleProperty , value );
            }
        }
        public static readonly DependencyProperty GridStyleProperty =
            DependencyProperty . Register ( "GridStyle" , typeof ( Style ) , typeof ( DatagridControl ) , new PropertyMetadata ( ( Style ) null ) );
        /**************************************************************************************************************/
        public SolidColorBrush HeaderBackground
        {
            get
            {
                return ( SolidColorBrush ) GetValue ( HeaderBackgroundProperty );
            }
            set
            {
                SetValue ( HeaderBackgroundProperty , value );
            }
        }
        public static readonly DependencyProperty HeaderBackgroundProperty =
            DependencyProperty . Register ( "HeaderBackground" , typeof ( SolidColorBrush ) , typeof ( DatagridControl ) , new PropertyMetadata ( Brushes . Black ) );
        /**************************************************************************************************************/
        public SolidColorBrush HeaderForeground
        {
            get
            {
                return ( SolidColorBrush ) GetValue ( HeaderForegroundProperty );
            }
            set
            {
                SetValue ( HeaderForegroundProperty , value );
            }
        }
        public static readonly DependencyProperty HeaderForegroundProperty =
            DependencyProperty . Register ( "HeaderForeground" , typeof ( SolidColorBrush ) , typeof ( DatagridControl ) , new PropertyMetadata ( Brushes . Yellow ) );
        /**************************************************************************************************************/
        #endregion Dependency Properties

        public IEnumerable<T> GetSqlData<T> ( string table , string constring )
        {
            IEnumerable<T> data;

            using ( var connection = new SqlConnection ( constring ) )
            {
                //               connection . Open ( );
                string domain = $"{Domain}.dbo.";
                var query = $@"SELECT * FROM {domain}{table}";
                data = connection . Query<T> ( query );
                //                connection . Close ( );
            }
            return data;
        }

        public dynamic ExecuteDapperTextCommand ( string command , string [ ] args , out string err )
        {
            err = "";
            dynamic dynretval = null;
            dynretval = ExecuteDapperScalar ( command , args , out err , 0 );
            return dynretval;
        }

        //public dynamic GetGenericFromEnumerable ( IEnumerable result )
        //{
        //    IEnumerator enummer = result . GetEnumerator ( );
        //    while ( result . MoveNext ( ) )
        //    {
        //         = enummer . Current;
        //    }
        //}
        public ObservableCollection<GenericClass> LoadData ( string table , bool UseTrueColumns , string ConnectionString , out int colcount )
        {
            ObservableCollection<GenericClass> collection = new ObservableCollection<GenericClass> ( );
            datagridControl . ItemsSource = null;
            IEnumerable<ObservableCollection<GenericClass>> result;
            // TODO - sort this out 
            result = GetSqlData<ObservableCollection<GenericClass>> ( table , ConnectionString );
            collection = result as ObservableCollection<GenericClass>;
            // Create a completely new instance via seriazable Clone method stored in NewWpfDev.Utils (in ObjectCopier class file)
            GridData = NewWpfDev . Utils . CopyCollection ( collection , GridData );
            Data = collection;
            PostProcessData ( GridData , datagridControl , table , UseTrueColumns , CurrentTableDomain );
            // grid IS LOADED by  here....
            datagridControl . SelectedIndex = 0;
            GenericClass gcc = datagridControl . SelectedItem as GenericClass;
            colcount = GetGenericColumnCount ( collection , gcc );
            // Clear list f column info as we are loading a  different table
            dglayoutlist . Clear ( );
            CurrentTable = table;
            GetNewColumnsInfo ( collection , table );

            ShowTrueColumns ( Dgrid , table , colcount , UseTrueColumns );
            return GridData;

        }
        /**************************************************************************************************************/
        public ObservableCollection<GenericClass> LoadGenericData ( string table , string [ ] args , bool UseTrueColumns , string ConnectionString )
        {
            string SqlCommand = "";
            ObservableCollection<GenericClass> collection = new ObservableCollection<GenericClass> ( );
            //dglayoutlist.Clear ();
            datagridControl . ItemsSource = null;
            string ResultString = "";
            if ( args . Length == 0 )
            {
                args = new string [ 1 ];
                args [ 0 ] = table;
            }
            SqlCommand = "spLoadTableAsGeneric";
            // WORKING 9/11/2022
            collection = LoadGeneric ( SqlCommand , args , ConnectionString , out ResultString );
            // Create a completely new instance via seriazable Clone method stored in NewWpfDev.Utils (in ObjectCopier class file)
            GridData = NewWpfDev . Utils . CopyCollection ( collection , GridData );
            //            GridData = collection .MakeClone (  );
            datagridControl . UpdateLayout ( );
            Data = collection;
            // handles loading visible rows only etc
            PostProcessData ( collection , datagridControl , table , UseTrueColumns , CurrentTableDomain );
            //datagridControl . UpdateLayout ( );
            //datagridControl . RefreshGrid ( );
            // grid IS LOADED by  here....
            DataLoaded = true;
            datagridControl . SelectedIndex = 0;
            DataLoaded = false;
            //           GenericClass gcc = datagridControl . SelectedItem as GenericClass;
            // Clear list f column info as we are loading a  different table
            //           dglayoutlist . Clear ( );
            if ( CurrentTable != table )
                CurrentTable = table;
            //GetNewColumnsInfo ( collection , table );
            //           int colcount = GetGenericColumnCount ( collection , gcc );

            //ShowTrueColumns ( Dgrid , table , colcount , UseTrueColumns );
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
                NewWpfDev . Utils . DoErrorBeep ( );
                Debug . WriteLine ( $"Column count error '{ex . Message}'" );
            }
            return 0;
        }
        public static ObservableCollection<GenericClass> LoadGeneric ( string Sqlcommand , string [ ] args , string ConnectionString , out string ResultString , int max = 0 , bool Notify = false , bool isMultiMode = false )
        {
            string argument = "";
            ObservableCollection<GenericClass> generics = new ObservableCollection<GenericClass> ( );
            if ( args . Length == 0 )
            {
                if ( Sqlcommand . Contains ( " " ) )
                {
                    args = Sqlcommand . Split ( " " );
                    argument = args [ 1 ];
                    Sqlcommand = args [ 0 ] . Trim ( );
                }
            }
            ExecuteStoredProcedure (
                Sqlcommand ,
                args ,
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
        string [ ] args ,
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
                    ref errormsg ,
                    args );
                ResultString = errormsg;
                return generics;
            }
            catch ( Exception ex )
            {
                NewWpfDev . Utils . DoErrorBeep ( );
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
                 ref string errormsg ,
                 string [ ] args = null
                  )
        {
            //			out string DbToOpen ,
            //==============================================//
            // Use DAPPER to run a Stored Procedure or using a text command version
            //==============================================//
            string result = "";
            errormsg = "";
            genericlist = new List<string> ( );
            string arg1 = "", arg2 = "", arg3 = "", arg4 = "";
            Dictionary<string , object> dict = new Dictionary<string , object> ( );

            string Con = NewWpfDev . Utils . GetCheckCurrentConnectionString ( CurrentTableDomain );
            using ( IDbConnection db = new SqlConnection ( Con ) )
            {
                try
                {
                    // Use DAPPER to run  Stored Procedure
                    try
                    {
                        // Parse out the arguments and put them in correct order for all SP's
                        if ( args == null || args?.Length == 0 )
                        {
                            string [ ] parts = Arguments . Split ( ',' );
                            if ( parts . Length > 1 )
                            {
                                if ( Arguments . Contains ( "\"" ) == true )
                                {
                                    bool [ ] argsarray = { false , false , false , false };
                                    int argscount = 0;
                                    // we maybe have args in quotes
                                    args = Arguments . Trim ( ) . Split ( '\'' );
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
                                    args = Arguments . Trim ( ) . Split ( ',' );
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
                            }
                        }
                        else
                        {
                            DynamicParameters parameters = new DynamicParameters ( );
                            ParseSqlArgs ( parameters , args );
                        }
                        // Call Dapper to get results using it's StoredProcedures method which returns
                        // a Dynamic IEnumerable that we then parse via a dictionary into collection of GenericClass  records
                        int colcount = 0, maxcols = 0;

                        if ( SqlCommand . ToUpper ( ) . Contains ( "SELECT " ) )
                        {
                            //***************************************************************************************************************//
                            // Performing a standard SELECT command but returning the data in a GenericClass structure	  (Bank/Customer/Details/etc)
                            $"{SqlCommand}" . DapperTrace ( );
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
                                            gc = ParseDapperRow ( item , out dict , out colcount , ref VarcharList );
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
                                                    NewWpfDev . Utils . DoErrorBeep ( );
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
                                            NewWpfDev . Utils . DoErrorBeep ( );
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
                                    NewWpfDev . Utils . DoErrorBeep ( );
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
                            DynamicParameters parameters = null;
                            //***************************************************************************************************************//
                            // This returns the data from SP commands (only) in a GenericClass Structured format
                            string argsbuff = "";
                            if ( arg1 != "" )
                                argsbuff += $"{arg1}";
                            if ( arg2 != "" )
                                argsbuff += $", {arg2}";
                            if ( arg3 != "" )
                                argsbuff += $", {arg3}";
                            if ( arg4 != "" )
                                argsbuff += $", {arg4}";
                            $"{SqlCommand} {argsbuff}" . DapperTrace ( );
                            if ( args != null && args . Length > 0 )
                            {
                                parameters = new DynamicParameters ( );
                                ParseSqlArgs ( parameters , args );
                            }

                            //***************************************************************************************************************//
                            // load an entire table from current database
                            // WORKING 9/11/2022
                            IEnumerable<dynamic> reslt = db . Query ( SqlCommand , parameters , commandType: CommandType . StoredProcedure );
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
                                            gc = ParseDapperRow ( item , out dict , out colcount , ref VarcharList );
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
                                                    NewWpfDev . Utils . DoErrorBeep ( );
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
                                            NewWpfDev . Utils . DoErrorBeep ( );
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
                                    NewWpfDev . Utils . DoErrorBeep ( );
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
                        NewWpfDev . Utils . DoErrorBeep ( );
                        Debug . WriteLine ( $"STORED PROCEDURE ERROR : {ex . Message}" );
                        $"STORED PROCEDURE ERROR : {ex . Message}" . err ( );
                        result = ex . Message;
                        errormsg = $"SQLERROR : {result}";
                    }
                }
                catch ( Exception ex )
                {
                    NewWpfDev . Utils . DoErrorBeep ( );
                    Debug . WriteLine ( $"Sql Error, {ex . Message}" );
                    result = ex . Message;
                }
            }
            $"{SqlCommand} Loaded {dict . Count} records" . DapperTrace ( );

            return dict . Count;
        }

        public static GenericClass ParseDapperRow ( dynamic buff , out Dictionary<string , object> dict , out int colcount , ref List<int> varcharlen , bool GetLength = false )
        {
            GenericClass GenRow = new GenericClass ( );
            int index = 0;
            colcount = 0;
            dict = new Dictionary<string , object> ( );

            try
            {
                foreach ( var item in buff )
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
            }
            catch ( Exception ex )
            {
                NewWpfDev . Utils . DoErrorBeep ( );
                Debug . WriteLine ( $"ParseDapperRow failed [{ex . Message}]" );
                return null;
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
        static public void PostProcessData ( ObservableCollection<GenericClass> collection , DataGrid grid , string Table , bool UseTrueColumns , string CurrentTableDomain )
        {
            //List<GenericClass> Glist = new List<GenericClass> ( );
            //Glist = collection . ToList ( );
            //GenericClass gc = new GenericClass ();
            //gc = collection [ 0 ];
            //int count = NewWpfDev . Utils.GetCollectionColumnCount ( gc );
            // int colcount = GenericGridSupport . GetTableColumnsCount ( Table , null , CurrentTableDomain );
            //         if(colcount != count )
            //          LoadActiveRowsOnlyInGrid ( grid , collection , colcount );
        }
        public List<Dictionary<string , string>> ShowTrueColumns ( DataGrid grid , string Table , int colcount , bool Show , bool Isloading = false )
        {
            List<Dictionary<string , string>> ColumnTypesList = new List<Dictionary<string , string>> ( );
            if ( Show == false )
            {
                SetDefColumnHeaderText ( grid , false );
                return ColumnTypesList;
            }
            grid = datagridControl;
            colcount = datagridControl . Columns . Count;
            if ( colcount == 0 && Isloading == false )
            {
                MessageBox . Show ( $"No records matched your filter term of \n[ {Genericgrid . LastActiveFillter} ] for {Table . ToUpper ( )}\n\nOriginal table [ {CurrentTable} ] is still displayed..." , "Table Columns handler" );
                return ColumnTypesList;
            }
            ReplaceDataGridFldNames ( GridData , ref grid , ref dglayoutlist , colcount );
            //if ( ColumnTypesList . Count == 0 )
            //{
            //    MessageBox . Show ( $"There is no column information available for {Table . ToUpper ( )}" , "Table Columns handler" );
            //}
            return ColumnTypesList;
        }
        public static void LoadActiveRowsOnlyInGrid ( DataGrid Grid , ObservableCollection<GenericClass> genericcollection , int total )
        {
            // Working 11/10/22
            "" . Track ( );
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
            // Grid . Visibility = Visibility . Visible;
            //Grid . UpdateLayout ( );
            //Grid . Focus ( );
            "" . Track ( 1 );
        }
        public static int GetColumnsCount ( ObservableCollection<GenericClass> collection = null , List<GenericClass> list = null )
        {
            int counter = 1;
            int maxcol = 20;
            dynamic source = null;
            "" . Track ( );
            if ( list != null )
                source = list;
            else if ( collection! != null )
                source = collection;
            foreach ( GenericClass item in source )
            {
                // We only ever do this for a single record !!!!  Not all records, so pretty fast
                GenericClass GenClass = item;
                switch ( counter )
                {
                    case 1:
                        maxcol = GenClass . field1 == null ? 0 : counter;
                        break;
                    case 2:
                        maxcol = GenClass . field2 == null ? 1 : counter;
                        break;
                    case 3:
                        maxcol = GenClass . field3 == null ? 2 : counter;
                        break;
                    case 4:
                        maxcol = GenClass . field4 == null ? 3 : counter;
                        break;
                    case 5:
                        maxcol = GenClass . field5 == null ? 4 : counter;
                        break;
                    case 6:
                        maxcol = GenClass . field6 == null ? 5 : counter;
                        break;
                    case 7:
                        maxcol = GenClass . field7 == null ? 6 : counter;
                        break;
                    case 8:
                        maxcol = GenClass . field8 == null ? 7 : counter;
                        break;
                    case 9:
                        maxcol = GenClass . field9 == null ? 8 : counter - 1;
                        break;
                    case 10:
                        maxcol = GenClass . field10 == null ? 9 : counter;
                        break;
                    case 11:
                        maxcol = GenClass . field11 == null ? 10 : counter;
                        break;
                    case 12:
                        maxcol = GenClass . field12 == null ? 11 : counter;
                        break;
                    case 13:
                        maxcol = GenClass . field13 == null ? 12 : counter;
                        break;
                    case 14:
                        maxcol = GenClass . field14 == null ? 13 : counter;
                        break;
                    case 15:
                        maxcol = GenClass . field15 == null ? 14 : counter;
                        break;
                    case 16:
                        maxcol = GenClass . field16 == null ? 15 : counter;
                        break;
                    case 17:
                        maxcol = GenClass . field17 == null ? 16 : counter;
                        break;
                    case 18:
                        maxcol = GenClass . field18 == null ? 17 : counter;
                        break;
                    case 19:
                        maxcol = GenClass . field19 == null ? 18 : counter;
                        break;
                    case 20:
                        maxcol = GenClass . field20 == null ? 19 : counter;
                        break;
                }
                if ( maxcol != counter )
                    break;
                counter++;
            }
            "" . Track ( 1 );
            // Adjust to actual columns count
            return maxcol;
        }
        public static void ReplaceDataGridFldNames ( ObservableCollection<GenericClass> datagrid , ref DataGrid Grid1 , ref List<DapperGenericsLib . DataGridLayout> dglayoutlist , int colcount )
        {
            //Stopwatch sw = new Stopwatch ( );
            //sw . Start ( );
            "" . Track ( );
            int index = 0;
            // Add data  for field size
            if ( datagrid . Count > 0 )
            {
                if ( dglayoutlist . Count > 0 )
                {
                    int max = dglayoutlist . Count;
                    // use the list to get the correct column header info
                    // and replace the column headers in our grid
                    string [ ] colnames = new string [ max ];
                    //More efficient than accessing dglayoutlist every column
                    foreach ( var item in dglayoutlist )
                    {
                        if ( index >= dglayoutlist . Count )
                            break;
                        colnames [ index++ ] = item . Fieldname;
                    }
                    try
                    {
                        if ( Grid1 . Columns . Count > 0 )
                        {
                            "Replacing column names" . Track ( 0 );
                            //Actually Change the column headers
                            index = 0;
                            foreach ( var item in Grid1 . Columns )
                            {
                                Grid1 . Columns [ index ] . Header = colnames [ index ];
                                if ( index >= max )
                                {
                                    break;
                                }
                                index++;
                            }
                        }
                    "Finished replacing column names" . Track ( 1 );
                    }
                    catch ( ArgumentOutOfRangeException ex ) { Debug . WriteLine ( $"TODO - BAD Columns - 300 GenericDbHandlers.cs" ); }
                }
                // Grid now has valid column names
            }
             return;
        }
        public static Dictionary<string , string> GetDbTableColumns ( ref ObservableCollection<GenericClass> Gencollection , ref List<Dictionary<string , string>> ColumntypesList ,
             ref List<string> list , string dbName , string DbDomain , ref List<DapperGenericsLib . DataGridLayout> dglayoutlist )
        {
            "" . Track ( );
            // This call CHANGES GridData content to columns data !!
            $"Calling GetSpArgs ( )  !!!!" . CW ( );
            Dictionary<string , string> dict = GetSpArgs ( ref Gencollection , ref ColumntypesList , ref list , dbName , DbDomain , ref dglayoutlist );
            "" . Track ( 1 );
            return dict;
        }
        public static Dictionary<string , string> GetSpArgs ( ref ObservableCollection<GenericClass> Gencollection , ref List<Dictionary<string , string>> ColumntypesList ,
            ref List<string> list , string dbName , string DbDomain , ref List<DapperGenericsLib . DataGridLayout> dglayoutlist )
        {
            string err = "";
            //this is an obs-collection of dglayoutlist
            DataTable dt = new DataTable ( );
            GenericClass genclass = new GenericClass ( );
            Dictionary<string , string> dict = new Dictionary<string , string> ( );
            "" . Track ( );
            try
            {
                //called on initial load to get column name and type (not datagrid data)
                dglayoutlist . Clear ( );
                if ( dglayoutlist . Count == 0 )
                {
                    string [ ] args = new string [ 1 ];
                    args [ 0 ] = $"'{dbName}'";
                    $"Calling SPGETTABLECOLUMNWITHSIZES()" . CW ( );
                    TableColumnsCollection = LoadDbAsGenericData ( $"spGetTableColumnWithSizes" , Gencollection , ref ColumntypesList , dbName , DbDomain , ref dglayoutlist , ref err , true );
                    // we now have a collection containing ALL column info

                }
            }
            catch ( Exception ex )
            {
                NewWpfDev . Utils . DoErrorBeep ( );
                MessageBox . Show ( $"SQL ERROR 1125 - {ex . Message}" );
                dict . Clear ( );
                "" . Track ( 1 );
                return dict;
            }

            dict . Clear ( );
            list . Clear ( );
            if ( err != "" )
            {
                err = $"Data load failed : Reason [ {err} ]";
                "" . Track ( 1 );
                return dict;
            }
            //try
            //{
            //    //int charlenindex = 0;
            //    foreach ( var item in TableColumnsCollection )
            //    {
            //        GenericClass gc = new GenericClass ( );
            //        gc = item as GenericClass;
            //        if ( dict . TryAdd ( gc . field1 , gc . field2 ) )
            //            list . Add ( gc . field1 . ToString ( ) );
            //    }
            //}
            //catch ( Exception ex )
            //{
            //    Debug . WriteLine ( ex . Message );
            //}
            "" . Track ( 1 );
            return dict;
        }

        public static ObservableCollection<GenericClass> LoadDbAsGenericData (
             string SqlCommand ,
             ObservableCollection<GenericClass> collection ,
             ref List<Dictionary<string , string>> ColumntypesList ,
             string Arguments ,
             string DbDomain ,
             ref List<DapperGenericsLib . DataGridLayout> dglayoutlist ,
             ref string errors ,
             bool GetLengths = false
              )
        {
            string result = "";
            string arg1 = "", arg2 = "", arg3 = "", arg4 = "";
            // provide a default connection string
            string ConString = "BankSysConnectionString";
            errors = "";
            Dictionary<string , dynamic> dict = new Dictionary<string , object> ( );
            ObservableCollection<GenericClass> GenClass = new ObservableCollection<GenericClass> ( );

            "" . Track ( );
            dglayoutlist . Clear ( );
            // Ensure we have the correct connection string for the current Db Doman
            if ( DbDomain == null )
                DbDomain = "IAN1";
            ConString = GenericDbUtilities . CheckSetSqlDomain ( CurrentTableDomain );
            if ( ConString == "" )
            {
                // set to our local definition
                ConString = MainWindow . CurrentSqlTableDomain;
            }
            else
            {
                DapperLibSupport . CheckResetDbConnection ( DbDomain , out string constr );
                ConString = constr;
            }
            collection . Clear ( );
            using ( IDbConnection db = new SqlConnection ( ConString ) )
            {
                try
                {
                    db . Open ( );
                    // Use DAPPER to run  Stored Procedure
                    // One or No arguments
                    arg1 = Arguments;
                    if ( arg1 . Contains ( "," ) )              // trim comma off
                        arg1 = arg1 . Substring ( 0 , arg1 . Length - 1 );
                    // Create our aguments using the Dynamic parameters provided by Dapper
                    var Params = new DynamicParameters ( );
                    if ( arg1 != "" )
                        Params . Add ( "@Arg1" , $"'{arg1}'" , DbType . String , ParameterDirection . Input , arg1 . Length + 2 );
                    if ( arg2 != "" )
                        Params . Add ( "Arg2" , arg2 , DbType . String , ParameterDirection . Input , arg2 . Length );
                    if ( arg3 != "" )
                        Params . Add ( "Arg3" , arg3 , DbType . String , ParameterDirection . Input , arg3 . Length );
                    if ( arg4 != "" )
                        Params . Add ( "Arg4" , arg4 , DbType . String , ParameterDirection . Input , arg4 . Length );

                    //***************************************************************************************************************//
                    // This returns the data from SP commands (only) in a GenericClass Structured format
                    $"Calling {SqlCommand . ToUpper ( )}( ) f" . CW ( );
                    var reslt = db . Execute ( SqlCommand , Params , commandType: CommandType . StoredProcedure );
                    //***************************************************************************************************************//

                    if ( reslt != null )
                    {
                        var Params2 = new DynamicParameters ( );

                        $"Select * from ian1 . DBO . x_2" . DapperTrace ( );
                        var reslt2 = db . Query ( $"Select * from ian1 . DBO . x_2" , Params2 , commandType: CommandType . Text );

                        //Although this is duplicated  with the one below we CANNOT make it a method()
                        string errormsg = "DYNAMIC";
                        int dictcount = 0;
                        int fldcount = 0;
                        int colcount = 0;
                        GenericClass gc = new GenericClass ( );

                        List<int> VarcharList = new List<int> ( );
                        List<string> datacolumninfo = new List<string> ( );
                        Dictionary<string , string> outdict = new Dictionary<string , string> ( );
                        try
                        {
                            foreach ( var item in reslt2 )
                            {
                                string colinfo = "";
                                DapperGenericsLib . DataGridLayout dglayout = new DapperGenericsLib . DataGridLayout ( );
                                try
                                {
                                    // we need to create a dictionary for each row of data then add it to a GenericClass row then add row to Generics Db
                                    string buffer = "";
                                    // WORKS OK
                                    ParseDapperRow ( item , dict , out colcount );
                                    gc = new GenericClass ( );
                                    dictcount = 1;
                                    fldcount = dict . Count;
                                    string tmp = "";

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

                                                    //dglayout . Colindex = index - 1;

                                                    // Add Dictionary <string,string> to List<Dictionary<string,string>
                                                    ColumntypesList . Add ( Columntypes );
                                                }
                                                //colinfo += dglayout . DataValue . ToString ( );
                                            }
                                        }
                                        catch ( Exception ex )
                                        {
                                            NewWpfDev . Utils . DoErrorBeep ( );
                                            Debug . WriteLine ( $"Dictionary ERROR : {ex . Message}" );
                                            result = ex . Message;
                                        }
                                    }
                                    if ( dict . Count == 0 )
                                        errormsg = $"No records were retuned for {SqlCommand}";
                                    if ( IsDuplicateFieldname ( dglayout , dglayoutlist ) == false )
                                    {
                                        //dglayout += index . ToString ( );
                                        dglayoutlist . Add ( dglayout );
                                    }//remove trailing comma
                                    string s = buffer?.Substring ( 0 , buffer . Length - 1 );
                                    buffer = s;
                                    // We now  have ONE sinlge record, but need to add this  to a GenericClass structure 
                                    int reccount = 1;
                                    NewWpfDev . Utils . ParseDictIntoGenericClass ( outdict , reccount , ref gc );
                                    //try
                                    //{
                                    //    foreach ( KeyValuePair<string , string> val in outdict )
                                    //    {  //
                                    //        switch ( reccount )
                                    //        {
                                    //            case 1:
                                    //                gc . field1 = val . Value . ToString ( );
                                    //                break;
                                    //            case 2:
                                    //                gc . field2 = val . Value . ToString ( );
                                    //                break;
                                    //            case 3:
                                    //                gc . field3 = val . Value . ToString ( );
                                    //                break;
                                    //            case 4:
                                    //                gc . field4 = val . Value . ToString ( );
                                    //                break;
                                    //            case 5:
                                    //                gc . field5 = val . Value . ToString ( );
                                    //                break;
                                    //            case 6:
                                    //                gc . field6 = val . Value . ToString ( );
                                    //                break;
                                    //            case 7:
                                    //                gc . field7 = val . Value . ToString ( );
                                    //                break;
                                    //            case 8:
                                    //                gc . field8 = val . Value . ToString ( );
                                    //                break;
                                    //            case 9:
                                    //                gc . field9 = val . Value . ToString ( );
                                    //                break;
                                    //            case 10:
                                    //                gc . field10 = val . Value . ToString ( );
                                    //                break;
                                    //            case 11:
                                    //                gc . field11 = val . Value . ToString ( );
                                    //                break;
                                    //            case 12:
                                    //                gc . field12 = val . Value . ToString ( );
                                    //                break;
                                    //            case 13:
                                    //                gc . field13 = val . Value . ToString ( );
                                    //                break;
                                    //            case 14:
                                    //                gc . field14 = val . Value . ToString ( );
                                    //                break;
                                    //            case 15:
                                    //                gc . field15 = val . Value . ToString ( );
                                    //                break;
                                    //            case 16:
                                    //                gc . field16 = val . Value . ToString ( );
                                    //                break;
                                    //            case 17:
                                    //                gc . field17 = val . Value . ToString ( );
                                    //                break;
                                    //            case 18:
                                    //                gc . field18 = val . Value . ToString ( );
                                    //                break;
                                    //            case 19:
                                    //                gc . field19 = val . Value . ToString ( );
                                    //                break;
                                    //            case 20:
                                    //                gc . field20 = val . Value . ToString ( );
                                    //                break;
                                    //        }
                                    //        reccount += 1;
                                    //    }
                                    //}
                                    //catch ( Exception ex )
                                    //{
                                    //    NewWpfDev . Utils . DoErrorBeep ( );
                                    //    Debug . WriteLine ( $"INNER DICT ERROR : {ex . Message}" );
                                    //    result = ex . Message;
                                    //    errormsg = result;
                                    //}
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
                        db . Close ( );
                        return collection; //collection.Count;
                    }
                }
                catch ( Exception ex )
                {
                    $"{ex . Message}" . cwerror ( );
                }
                finally
                {
                    db . Close ( );
                }
            }
            "" . Track ( 1 );
            return GenClass;
        }

        public ObservableCollection<GenericClass> CreateLayoutlist ( IEnumerable<dynamic> reslt2 , out List<Dictionary<string , string>> ColumntypesList , out string err )
        {
            //Although this is duplicated  with the one below we CANNOT make it a method()
            List<Dictionary<string , string>> list = new List<Dictionary<string , string>> ( );
            ColumntypesList = list;
            string errormsg = "DYNAMIC";
            int dictcount = 0;
            int fldcount = 0;
            int colcount = 0;
            GenericClass gc = new GenericClass ( );
            GenericClass genclass = new GenericClass ( );
            Dictionary<string , object> dict = new Dictionary<string , object> ( );

            List<int> VarcharList = new List<int> ( );
            List<string> datacolumninfo = new List<string> ( );
            Dictionary<string , string> outdict = new Dictionary<string , string> ( );
            DapperGenericsLib . DataGridLayout dglayout = new DapperGenericsLib . DataGridLayout ( );
            ObservableCollection<GenericClass> collection = new ObservableCollection<GenericClass> ( );

            err = "";

            try
            {
                foreach ( var item in reslt2 )
                {
                    try
                    {
                        // we need to create a dictionary for each row of data then add it to a GenericClass row then add row to Generics Db
                        string buffer = "";
                        // WORKS OK
                        ParseDapperRow ( item , dict , out colcount );
                        gc = new GenericClass ( );
                        dictcount = 1;
                        fldcount = dict . Count;
                        string tmp = "";

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

                                        //dglayout . Colindex = index - 1;

                                        // Add Dictionary <string,string> to List<Dictionary<string,string>
                                        ColumntypesList . Add ( Columntypes );
                                    }
                                    //colinfo += dglayout . DataValue . ToString ( );
                                }
                            }
                            catch ( Exception ex )
                            {
                                Debug . WriteLine ( $"Dictionary ERROR : {ex . Message}" );
                                err = ex . Message;
                            }
                        }
                        //if ( dict . Count == 0 )
                        //    errormsg = $"No records were retuned for {spCommand}";
                        if ( IsDuplicateFieldname ( dglayout , dglayoutlist ) == false )
                        {
                            //dglayout += index . ToString ( );
                            dglayoutlist . Add ( dglayout );
                        }//remove trailing comma
                        string s = buffer?.Substring ( 0 , buffer . Length - 1 );
                        buffer = s;
//                        genericlist . Add ( buffer );
                        // We now  have ONE sinlge record, but need to add this  to a GenericClass structure 
                        int reccount = 1;
                        NewWpfDev . Utils . ParseDictIntoGenericClass ( outdict , reccount , ref gc );
                        //foreach ( KeyValuePair<string , string> val in outdict )
                        //{  //
                        //    switch ( reccount )
                        //    {
                        //        case 1:
                        //            gc . field1 = val . Value . ToString ( );
                        //            break;
                        //        case 2:
                        //            gc . field2 = val . Value . ToString ( );
                        //            break;
                        //        case 3:
                        //            gc . field3 = val . Value . ToString ( );
                        //            break;
                        //        case 4:
                        //            gc . field4 = val . Value . ToString ( );
                        //            break;
                        //        case 5:
                        //            gc . field5 = val . Value . ToString ( );
                        //            break;
                        //        case 6:
                        //            gc . field6 = val . Value . ToString ( );
                        //            break;
                        //        case 7:
                        //            gc . field7 = val . Value . ToString ( );
                        //            break;
                        //        case 8:
                        //            gc . field8 = val . Value . ToString ( );
                        //            break;
                        //        case 9:
                        //            gc . field9 = val . Value . ToString ( );
                        //            break;
                        //        case 10:
                        //            gc . field10 = val . Value . ToString ( );
                        //            break;
                        //        case 11:
                        //            gc . field11 = val . Value . ToString ( );
                        //            break;
                        //        case 12:
                        //            gc . field12 = val . Value . ToString ( );
                        //            break;
                        //        case 13:
                        //            gc . field13 = val . Value . ToString ( );
                        //            break;
                        //        case 14:
                        //            gc . field14 = val . Value . ToString ( );
                        //            break;
                        //        case 15:
                        //            gc . field15 = val . Value . ToString ( );
                        //            break;
                        //        case 16:
                        //            gc . field16 = val . Value . ToString ( );
                        //            break;
                        //        case 17:
                        //            gc . field17 = val . Value . ToString ( );
                        //            break;
                        //        case 18:
                        //            gc . field18 = val . Value . ToString ( );
                        //            break;
                        //        case 19:
                        //            gc . field19 = val . Value . ToString ( );
                        //            break;
                        //        case 20:
                        //            gc . field20 = val . Value . ToString ( );
                        //            break;
                        //    }
                        //    reccount += 1;
                        //}
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
            // this contains full column info incl sizes
            return collection;
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
        public static bool IsDuplicateFieldname ( DapperGenericsLib . DataGridLayout dglayout , List<DapperGenericsLib . DataGridLayout> dglayoutlist )
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
        public static bool IsDuplicatecolumnname ( string Columntypes , Dictionary<string , string> ColumntypesList )
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
        }
        /// <summary>
        /// method using std SQL to return a Datatable containing requested data
        /// </summary>
        /// <param name="SqlCommand"></param>
        /// <param name="connstring"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        static public DataTable ProcessSqlCommand ( string SqlCommand , string connstring , string [ ] args = null )
        {
            SqlConnection con;
            DataTable dt = new DataTable ( );
            string filterline = "";
            string ConString = connstring;
            "" . Track ( 0 );
            if ( connstring == "" )
                ConString = "Data Source=DINO-PC;Initial Catalog=\"IAN1\";Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            con = new SqlConnection ( ConString );
            try
            {
                using ( con )
                {
                    if ( args != null )
                        SqlCommand = SqlCommand + $" '{args [ 0 ]}'";
                    SqlCommand cmd = new SqlCommand ( SqlCommand , con );
                    SqlDataAdapter sda = new SqlDataAdapter ( cmd );
                    sda . Fill ( dt );
                }
            }
            catch ( Exception ex )
            {
                Debug . WriteLine ( $"ERROR in PROCESSSQLCOMMAND(): Failed to load Datatable :\n {ex . Message}" );
                MessageBox . Show ( $"ERROR in PROCESSSQLCOMMAND(): Failed to load datatable\n{ex . Message}" );
            }
            finally
            {
                $" SQL data loaded from SQLCommand [{SqlCommand . ToUpper ( )}]" . cwinfo ( );
//                Debug . WriteLine ( $" SQL data loaded from SQLCommand [{SqlCommand . ToUpper ( )}]" );
                con . Close ( );
                "" . Track ( 1 );
            }
            return dt;
        }
        public void GetNewColumnsInfo ( ObservableCollection<GenericClass> collection , string Table )
        {
            List<string> list = new List<string> ( );
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
            Debug . WriteLine ( $"Finished collecting column header info to update Datagrid columns" );
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

        #region Keyboard handlers
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
        #endregion Keyboard handlers

        public static ObservableCollection<GenericClass> CreateGenericCollection (
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
                            $"{SqlCommand}" . DapperTrace ( );
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
                                            NewWpfDev . Utils . ParseDictIntoGenericClass ( outdict , reccount , ref gc );
                                            //foreach ( KeyValuePair<string , string> val in outdict )
                                            //{  //
                                            //    switch ( reccount )
                                            //    {
                                            //        case 1:
                                            //            gc . field1 = val . Value . ToString ( );
                                            //            break;
                                            //        case 2:
                                            //            gc . field2 = val . Value . ToString ( );
                                            //            break;
                                            //        case 3:
                                            //            gc . field3 = val . Value . ToString ( );
                                            //            break;
                                            //        case 4:
                                            //            gc . field4 = val . Value . ToString ( );
                                            //            break;
                                            //        case 5:
                                            //            gc . field5 = val . Value . ToString ( );
                                            //            break;
                                            //        case 6:
                                            //            gc . field6 = val . Value . ToString ( );
                                            //            break;
                                            //        case 7:
                                            //            gc . field7 = val . Value . ToString ( );
                                            //            break;
                                            //        case 8:
                                            //            gc . field8 = val . Value . ToString ( );
                                            //            break;
                                            //        case 9:
                                            //            gc . field9 = val . Value . ToString ( );
                                            //            break;
                                            //        case 10:
                                            //            gc . field10 = val . Value . ToString ( );
                                            //            break;
                                            //        case 11:
                                            //            gc . field11 = val . Value . ToString ( );
                                            //            break;
                                            //        case 12:
                                            //            gc . field12 = val . Value . ToString ( );
                                            //            break;
                                            //        case 13:
                                            //            gc . field13 = val . Value . ToString ( );
                                            //            break;
                                            //        case 14:
                                            //            gc . field14 = val . Value . ToString ( );
                                            //            break;
                                            //        case 15:
                                            //            gc . field15 = val . Value . ToString ( );
                                            //            break;
                                            //        case 16:
                                            //            gc . field16 = val . Value . ToString ( );
                                            //            break;
                                            //        case 17:
                                            //            gc . field17 = val . Value . ToString ( );
                                            //            break;
                                            //        case 18:
                                            //            gc . field18 = val . Value . ToString ( );
                                            //            break;
                                            //        case 19:
                                            //            gc . field19 = val . Value . ToString ( );
                                            //            break;
                                            //        case 20:
                                            //            gc . field20 = val . Value . ToString ( );
                                            //            break;
                                            //    }
                                            //    reccount += 1;
                                            //}
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
                    //                   $"Sql Error, {ex . Message}" . cwerror ( );
                    result = ex . Message;
                    //                   $"STORED PROCEDURE ERROR : {ex . Message}" . cwerror ( );
                }
            } // end using {} - MUST get here  to close connection correctly
              //            $"Exiting with null" . cwwarn ( );
            return collection;
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
        public int CreateGenericDbStoredProcedure ( string SqlCommand , string [ ] args , string ConString , out string err , string Domain = "IAN1" )
        //--------------------------------------------------------------------------------------------------------------------------------------------------------
        {
            //===============================================//
            // This version successfully creates a new Db as specified by the args[] string array
            //===============================================//
            int gresult = -1;
            string Con = ConString;
            SqlConnection sqlCon = null;
            err = "";
            //           List<string> SqlCommandList= new List<string> ( );
            "" . Track ( );

            try
            {
                using ( sqlCon = new SqlConnection ( Con ) )
                {
                    sqlCon . Open ( );
                    //                 string [ ] str = SqlCommand . Split ( ' ' );
                    using ( var tran = sqlCon . BeginTransaction ( ) )
                    {
                        var parameters = new DynamicParameters ( );
                        parameters = ParseSqlArgs ( parameters , args );
                        //if ( args . Length > 0 )
                        //{
                        //    //Add the new table name as 1st argument parameter
                        //    //                           string s = str [ 1 ];
                        //    parameters . Add ( $"Arg1" , args [ 0 ] ,
                        //        DbType . String ,
                        //        ParameterDirection . Input ,
                        //        args [ 0 ] . Length );
                        //    parameters . Add ( $"Domain" , args [ 1 ] ,
                        //          DbType . String ,
                        //          ParameterDirection . Input ,
                        //          args [ 1 ] . Length );
                        //    // add rest of arguments for fields in new table
                        //    for ( int x = 2 ; x < args . Length ; x++ )
                        //    {
                        //        //string [ ] fields;
                        //        //string fld = "", type = "";
                        //        //fields = args [ x ] . Split ( ' ' );
                        //        ////fld = fields [ 0 ];
                        //        //for ( int y = 0 ; y < fields . Length - 1 ; y++ )
                        //        //    type += fields [ y ] + " ";
                        //        parameters . Add ( $"fld{x - 1}" , $"{args [ x ] . Trim ( )}" , DbType . String , ParameterDirection . Input , args [ x ] . Trim ( ) . Length );
                        //    }
                        //}
                        //Create the new table as requested
                        //************************************************************************************************************************************************************//
                        gresult = sqlCon . Execute ( "spCreateTableFromSchema" , parameters , commandType: CommandType . StoredProcedure , transaction: tran );
                        //************************************************************************************************************************************************************//
                        // Commit the transaction
                        tran . Commit ( );
                        gresult = 1;
                        // We now have a newly created  empty table
                    }
                }
            }
            catch ( Exception ex )
            {
                Debug . WriteLine ( $"Error {ex . Message}" );
                StdError ( );
                err = $"Error {ex . Message}";
            }
            //            WpfLib1 . Utils . trace ("CreateGenericDbStoredProcedure" );
            "" . Track ( 1 );
            return gresult;
        }

        #region create newsmart Sql tables

        public ObservableCollection<GenericClass> CreateFullColumnInfo ( string spName , string ConString , bool showinfo = true )
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
                string err = "";
                int recordcount = 0;
                string [ ] args = new string [ 1 ];
                args [ 0 ] = spName;
                string resultstring = "";
                string error = "";
                object obj = null;
                Type Objtype = null;
                int count = 0;
                List<Dictionary<string , string>> ColumnTypesList = new List<Dictionary<string , string>> ( );
                List<string [ ]> argsbuffer = new List<string [ ]> ( );
                argsbuffer . Add ( args );
                string SqlCommand = $"select column_name, data_type, character_maximum_length,numeric_precision, numeric_scale from information_schema . columns WHERE upper ( table_name ) = '{spName . ToUpper ( )}'";
                IEnumerable<dynamic> DynList = GenDapperQueries . Get_DynamicValue_ViaDapper (
                   SqlCommand ,
                   argsbuffer ,
                  ref resultstring ,
                  ref obj ,
                  ref Objtype ,
                  ref count ,
                  ref error ,
                   3 );
                //               dynamic dynreslt = GetDataViaDapper ( SqlCommand , args , out ColumnTypesList , out recordcount );
                //GetDataFromStoredProcedure ( SqlCommand , null , out err , out recordcount );
                //we  now have a table (Generics) holding all column info
                Dictionary<string , object> dict = new Dictionary<string , object> ( );
                List<int> varcharlen = new List<int> ( );
                if ( DynList != null )

                    foreach ( var item in DynList )
                    {
                        int colcount = 0;
                        GenericClass gc = new GenericClass ( );
                        //  Dictionary<string, object> dict, out int colcount, ref List<int> varcharlen, bool GetLength = false
                        gc = DapperSupport . ParseDapperRow ( item , out dict , out colcount , ref varcharlen , true );
                        Generics . Add ( gc );
                    }
                //if ( DynList == null )
                //{
                //    columncount = 0;
                //    return null;
                //}
                //int decsize = 0, decpart = 0, decroot = 0;
                if ( showinfo )
                {
                    string buffer = "";
                    foreach ( var item in Generics )
                    {
                        buffer += $"{item . field1 . Trim ( )}:";    // fname
                        if ( item . field2 != null )
                            buffer += $" {item . field2 . Trim ( )}:";
                        if ( item . field3 != null )
                            buffer += $" {item . field3 . Trim ( )}:";
                        if ( item . field4 != null )
                            buffer += $" {item . field4 . Trim ( )}";
                        buffer += "\n";
                    }

                    string fdinput = $"Procedure Name : {spName . ToUpper ( )}\n";
                    fdinput += buffer;
                    fdl . ShowInfo ( Flowdoc , canvas , line1: fdinput , clr1: "Black0" , line2: "" , clr2: "Black0" , line3: "" , clr3: "Black0" , header: "" , clr4: "Black0" );
                    canvas . Visibility = Visibility . Visible;
                    Flowdoc . Visibility = Visibility . Visible;
                    return null;
                }
                else
                    return Generics;    // Collection of donor tables structure elements
            }
            catch ( Exception ex )
            {
                MessageBox . Show ( $"SQL ERROR 1125 - {ex . Message}, {ex . Data}" );
            }
            return null;
        }

        public static string [ ] CreateSqlCommand ( List<GenericToRealStructure> TableStructure , string newDbName , string [ ] args , List<string> SqlArgs , string Domain = "IAN1" )
        {
            string line = "";
            string [ ] strings = new string [ TableStructure . Count + 2 ];
            string output = $"{newDbName}, {Domain}, ";
            string fields = "";
            int index = 1;
            int arrayindex = 2;
            "" . Track ( );
            // args = strings;
            args = new string [ TableStructure . Count + 2 ];
            args [ 0 ] = newDbName;
            args [ 1 ] = Domain;
            output = $" {newDbName} (";
            List<string> flddescription = new List<string> ( );
            flddescription . Add ( newDbName );
            flddescription . Add ( Domain );
            // we need to create a layout of fields :
            // fieldname type (size [,size]) [INT IDENTITY] [NULL / NOT NULL],
            foreach ( var item in TableStructure )
            {
                //string [ ] field = item . fname . Split ( ',' );
                string newline = $"{item . fname} {item . ftype} ";

                if ( item . ftype . ToUpper ( ) == "INT" )
                {
                    //if ( item . decroot!= 0 )
                    //    newline += $"({item . decroot})";

                    if ( MainWindow . USE_ID_IDENTITY )
                        newline += $" IDENTITY(1,1) NOT NULL ";
                    else
                        newline += $" NOT NULL ";
                }
                else if ( item . ftype . ToUpper ( ) == "DECIMAL" )
                {
                    if ( item . decroot != 0 )
                        newline += $" ({item . decroot}";
                    if ( item . decpart != 0 )
                        newline += $", {item . decpart}";
                    newline += $")";
                }
                else if ( item . ftype . ToUpper ( ) . Contains ( "VARCHAR" )
                    || item . ftype . ToUpper ( ) . Contains ( "NCHAR" ) )
                {
                    if ( item . decroot != 0 )
                        newline += $" ({item . decroot}";
                    if ( item . decpart != 0 )
                        newline += $", {item . decpart}";
                    newline += $")";

                }
                else if ( item . ftype . ToUpper ( ) == "DATETIME"
                    || item . ftype . ToUpper ( ) == "DATE" )
                {
                }
                args [ arrayindex++ ] = newline . Trim ( );
                flddescription . Add ( newline . Trim ( ) );
                output += $"{newline}:";
                index++;
                line = "";
            }
            output = output . Trim ( );
            output = output . Substring ( 0 , output . Length - 1 ) + ")";
            // string [] newargs = new string [ args.Length -1];
            //output now has all data seperatd by Colons
            //for (int y = 0 ; y < args.Length ; y++)
            //{
            //    if ( y == 0 )
            //        newargs [ 0 ] = args [ y ];
            //    if ( y == 1 )
            //        newargs [ 1 ] = args [ y ];
            //   //newargs = 
            //}

            //string newoutput = $"{newDbName} ";
            ////newoutputlines . Add ( newoutput );
            //int p = 2;
            ////strings [ p++ ] = newoutput;
            //string tmps = "";
            //for ( int y = 0; y < flddescription.Count ; y++ )
            //{
            //    //tmps = line1 . Trim ( );
            //    //tmps = tmps . Substring ( 0 , tmps . Length - 1 );
            //    //newoutputlines . Add ( tmps );
            //    //strings [ p ] = tmps;
            //    //if ( p < flddescription.l
            //    //p++;
            //    args [ y ] = flddescription [ y ];
            //}
            //args = strings;
            "" . Track ( 1 );
            return args;
        }

        #endregion

        static public List<string> GetSqlData<T> ( T Collection , string SqlCommand )
        {
            List<string> list = new List<string> ( );
            DataTable dt = new DataTable ( );
            //                dt = GetDataTable (SqlCommand ,constring );

            try
            {
                //SqlConnection con;
                //CheckDbDomain ( Genericgrid . CurrentTableDomain );
                //string DbConnectionString = "Data Source=DINO-PC;Initial Catalog=\"IAN1\";Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                //ring constring = ( string )Properties . Settings . Default [ "BankSysConnectionString" ];
                //string constring = ConfigurationManager . ConnectionStrings [ "Ian1" ] . ConnectionString;

                SqlConnection con;
                string ConString = GenericDbUtilities . CheckSetSqlDomain ( domain );
                if ( ConString == "" )
                {
                    ConString = MainWindow . SqlCurrentConstring;
                }

                //    CheckDbDomain ( Genericgrid . CurrentTableDomain );
                ////string DbConnectionString = "Data Source=DINO-PC;Initial Catalog=\"IAN1\";Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                //ring constring = ( string )Properties . Settings . Default [ "BankSysConnectionString" ];
                //string constring = ConfigurationManager . ConnectionStrings [ "Ian1" ] . ConnectionString;
                con = new SqlConnection ( ConString );
                //using ( con )
                //{
                //    con = new SqlConnection ( DbConnectionString );
                using ( con )
                {
                    SqlCommand cmd = new SqlCommand ( SqlCommand , con );
                    SqlDataAdapter sda = new SqlDataAdapter ( cmd );
                    sda . Fill ( dt );
                }
                foreach ( DataRow row in dt . Rows )
                {
                    // ... add string value
                    list . Add ( row . Field<string> ( 0 ) );
                }
            }
            catch ( Exception ex )
            {
                Debug . WriteLine ( $"Failed to load Db - {ex . Message}" );
                return null;
            }
            return list;
        }

        //*********************************//        
        #region table creation       
        //*********************************//        
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
            //TODO   temp commenting
            //GetFullColumnInfo ( DatagridControl . CurrentTable , DatagridControl . CurrentTable , DbConnectionString , false );
            //if ( TableStruct == null )
            //    TableStructure = CreateFullColumnInfo ( DatagridControl . CurrentTable , DbConnectionString );
            //else
            //{
            //    string error = "";
            //    CreateLimitedTableAsync ( NewTableName , TableStruct , out error );
            //    if ( error != "" )
            //        Debug . WriteLine ( $"ERROR : {error}" );
            //    return 1;
            //}
            // We now have a full SQl Structure for the current table in TableStructure
            // Sort out  our  new table structure
            string [ ] Sqlargs = new string [ 20 ];
            int gresult = -1;
            string commandline = "";
            List<string> newlines = new List<string> ( );
            Sqlargs = CreateSqlCommand ( TableStructure , NewDbName , Sqlargs , newlines , Domain );
            // Now we have got a fully formatted SqlCommand and the necessary arguments using the special CreateGenericDbStoredProcedure S.P.
            try
            {
                // Create the new table in current Db
                if ( CreateGenericDbStoredProcedure ( $"spCREATENEWDBTABLE" , Sqlargs , DbConnectionString , out string err ) == 1 )
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
                                            if ( TableStructure [ x ] . ftype . ToUpper ( ) == "DATETIME" || TableStructure [ x ] . ftype . ToUpper ( ) == "DATE" )
                                                datavalues . Add ( ConvertToUsSqlDate ( item . field1 ) );
                                            else
                                                datavalues . Add ( item . field1 );
                                            itemscount++;
                                        }
                                        break;
                                    case 2:
                                        if ( TableStructure [ x ] . ftype . ToUpper ( ) == "DATETIME" || TableStructure [ x ] . ftype . ToUpper ( ) == "DATE" )
                                            datavalues . Add ( ConvertToUsSqlDate ( item . field2 ) );
                                        else
                                            datavalues . Add ( item . field2 );
                                        itemscount++;
                                        break;
                                    case 3:
                                        if ( TableStructure [ x ] . ftype . ToUpper ( ) == "DATETIME" || TableStructure [ x ] . ftype . ToUpper ( ) == "DATE" )
                                            datavalues . Add ( ConvertToUsSqlDate ( item . field3 ) );
                                        else
                                            datavalues . Add ( item . field3 );
                                        itemscount++;
                                        break;
                                    case 4:
                                        if ( TableStructure [ x ] . ftype . ToUpper ( ) == "DATETIME" || TableStructure [ x ] . ftype . ToUpper ( ) == "DATE" )
                                            datavalues . Add ( ConvertToUsSqlDate ( item . field4 ) );
                                        else
                                            datavalues . Add ( item . field4 );
                                        itemscount++;
                                        break;
                                    case 5:
                                        if ( TableStructure [ x ] . ftype . ToUpper ( ) == "DATETIME" || TableStructure [ x ] . ftype . ToUpper ( ) == "DATE" )
                                            datavalues . Add ( ConvertToUsSqlDate ( item . field5 ) );
                                        else
                                            datavalues . Add ( item . field5 );
                                        itemscount++;
                                        break;
                                    case 6:
                                        if ( TableStructure [ x ] . ftype . ToUpper ( ) == "DATETIME" || TableStructure [ x ] . ftype . ToUpper ( ) == "DATE" )
                                            datavalues . Add ( ConvertToUsSqlDate ( item . field6 ) );
                                        else
                                            datavalues . Add ( item . field6 );
                                        itemscount++;
                                        break;
                                    case 7:
                                        if ( TableStructure [ x ] . ftype . ToUpper ( ) == "DATETIME" || TableStructure [ x ] . ftype . ToUpper ( ) == "DATE" )
                                            datavalues . Add ( ConvertToUsSqlDate ( item . field7 ) );
                                        else
                                            datavalues . Add ( item . field7 );
                                        itemscount++;
                                        break;
                                    case 8:
                                        if ( TableStructure [ x ] . ftype . ToUpper ( ) == "DATETIME" || TableStructure [ x ] . ftype . ToUpper ( ) == "DATE" )
                                            datavalues . Add ( ConvertToUsSqlDate ( item . field8 ) );
                                        else
                                            datavalues . Add ( item . field8 );
                                        itemscount++;
                                        break;
                                    case 9:
                                        if ( TableStructure [ x ] . ftype . ToUpper ( ) == "DATETIME" || TableStructure [ x ] . ftype . ToUpper ( ) == "DATE" )
                                            datavalues . Add ( ConvertToUsSqlDate ( item . field9 ) );
                                        else
                                            datavalues . Add ( item . field9 );
                                        itemscount++;
                                        break;
                                    case 10:
                                        if ( TableStructure [ x ] . ftype . ToUpper ( ) == "DATETIME" || TableStructure [ x ] . ftype . ToUpper ( ) == "DATE" )
                                            datavalues . Add ( ConvertToUsSqlDate ( item . field10 ) );
                                        else
                                            datavalues . Add ( item . field10 );
                                        itemscount++;
                                        break;
                                    case 11:
                                        if ( TableStructure [ x ] . ftype . ToUpper ( ) == "DATETIME" || TableStructure [ x ] . ftype . ToUpper ( ) == "DATE" )
                                            datavalues . Add ( ConvertToUsSqlDate ( item . field11 ) );
                                        else
                                            datavalues . Add ( item . field11 );
                                        itemscount++;
                                        break;
                                    case 12:
                                        if ( TableStructure [ x ] . ftype . ToUpper ( ) == "DATETIME" || TableStructure [ x ] . ftype . ToUpper ( ) == "DATE" )
                                            datavalues . Add ( ConvertToUsSqlDate ( item . field12 ) );
                                        else
                                            datavalues . Add ( item . field12 );
                                        itemscount++;
                                        break;
                                    case 13:
                                        if ( TableStructure [ x ] . ftype . ToUpper ( ) == "DATETIME" || TableStructure [ x ] . ftype . ToUpper ( ) == "DATE" )
                                            datavalues . Add ( ConvertToUsSqlDate ( item . field13 ) );
                                        else
                                            datavalues . Add ( item . field13 );
                                        itemscount++;
                                        break;
                                    case 14:
                                        if ( TableStructure [ x ] . ftype . ToUpper ( ) == "DATETIME" || TableStructure [ x ] . ftype . ToUpper ( ) == "DATE" )
                                            datavalues . Add ( ConvertToUsSqlDate ( item . field14 ) );
                                        else
                                            datavalues . Add ( item . field14 );
                                        itemscount++;
                                        break;
                                    case 15:
                                        if ( TableStructure [ x ] . ftype . ToUpper ( ) == "DATETIME" || TableStructure [ x ] . ftype . ToUpper ( ) == "DATE" )
                                            datavalues . Add ( ConvertToUsSqlDate ( item . field15 ) );
                                        else
                                            datavalues . Add ( item . field15 );
                                        itemscount++;
                                        break;
                                    case 16:
                                        if ( TableStructure [ x ] . ftype . ToUpper ( ) == "DATETIME" || TableStructure [ x ] . ftype . ToUpper ( ) == "DATE" )
                                            datavalues . Add ( ConvertToUsSqlDate ( item . field16 ) );
                                        else
                                            datavalues . Add ( item . field16 );
                                        itemscount++;
                                        break;
                                    case 17:
                                        if ( TableStructure [ x ] . ftype . ToUpper ( ) == "DATETIME" || TableStructure [ x ] . ftype . ToUpper ( ) == "DATE" )
                                            datavalues . Add ( ConvertToUsSqlDate ( item . field17 ) );
                                        else
                                            datavalues . Add ( item . field17 );
                                        itemscount++;
                                        break;
                                    case 18:
                                        if ( TableStructure [ x ] . ftype . ToUpper ( ) == "DATETIME" || TableStructure [ x ] . ftype . ToUpper ( ) == "DATE" )
                                            datavalues . Add ( ConvertToUsSqlDate ( item . field18 ) );
                                        else
                                            datavalues . Add ( item . field18 );
                                        itemscount++;
                                        break;
                                    case 19:
                                        if ( TableStructure [ x ] . ftype . ToUpper ( ) == "DATETIME" || TableStructure [ x ] . ftype . ToUpper ( ) == "DATE" )
                                            datavalues . Add ( ConvertToUsSqlDate ( item . field19 ) );
                                        else
                                            datavalues . Add ( item . field19 );
                                        itemscount++;
                                        break;
                                    case 20:
                                        if ( TableStructure [ x ] . ftype . ToUpper ( ) == "DATETIME" || TableStructure [ x ] . ftype . ToUpper ( ) == "DATE" )
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
                                    if ( TableStructure [ x ] . ftype . ToUpper ( ) == "DATETIME" || TableStructure [ x ] . ftype . ToUpper ( ) == "DATE" )
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
                            string cmd = $"spExecuteStoredProcedureCommand";

                            //****************************************************************************************************************//
                            gresult = sqlCon . Execute ( cmd , parameters , commandType: CommandType . StoredProcedure );
                            gresult = 1;
                            SqlInsertCommand = "";
                        }   //foreach
                    }   // using
                        //                } );
                }   // if
            }   // try
            catch ( Exception ex )
            {
                Debug . WriteLine ( $"Error {ex . Message}" );
                $" {ex . Message}" . dcwerror ( );
                StdError ( );
                gresult = -3;
            }
            Mouse . OverrideCursor = Cursors . Arrow;
            return gresult;
        }
        public ObservableCollection<GenericClass> CreateLimitedTableAsync ( string NewTableName , List<GenericToRealStructure> TableStruct , out string SqlCommandstring , out string [ ] Sqlargs , out int [ ] columnoffsets , out string error )
        {
            // We now have a full SQl Structure for the current table in TableStructure
            // Sort out  our  new table structure
            Sqlargs = new string [ 20 ];
            int gresult = -1;
            bool ColumnMissed = false;
            string commandline = "";
            List<string> newlines = new List<string> ( );

            SqlCommandstring = "";
            columnoffsets = new int [ TableStruct . Count ];
            error = "";
            "" . Track ( );

            Sqlargs = CreateSqlCommand ( TableStruct , NewTableName , Sqlargs , newlines );
            // Now we have got the necessary arguments to let us create a new table structure using the special CreateGenericDbStoredProcedure S.P.
            try
            {
                // Create the new table in current Db - this is absolutely correct, based on data from SqlArgs data....
                if ( CreateGenericDbStoredProcedure ( $"spCreateTableFromSchema" , Sqlargs , DbConnectionString , out string err ) == 1 )
                {
                    //Table created successfully with new schema, so Copy all data, but from selected columns only to our new created table
                    // Problem is to get the data  in same order as the fields are defined

                    // 1st create the basic Insert Command string for thew fields in correct order as selected                    
                    SqlOperationString = $"Insert into {NewTableName} (";
                    for ( int a = 2 ; a < Sqlargs . Length ; a++ )
                    {
                        string [ ] temp = Sqlargs [ a ] . Split ( ' ' );
                        SqlOperationString += $"{temp [ 0 ]}, ";
                    }
                    SqlOperationString = SqlOperationString . Substring ( 0 , SqlOperationString . Length - 2 );
                    SqlOperationString += ") Values (";

                    // save to our out string parameter
                    SqlCommandstring = SqlOperationString;
                    // save column index  to int array out parameter
                    for ( int b = 0 ; b < TableStruct . Count ; b++ )
                        columnoffsets [ b ] = TableStruct [ b ] . colindex;

                    ObservableCollection<GenericClass> LimitedColumnTable = new ObservableCollection<GenericClass> ( );
                    List<string> datavalues = new List<string> ( );
                    int rangecount = TableStruct . Count;
                    //string Con = DbConnectionString;
                    gresult = -1;
                    //SqlConnection sqlCon = null;
                    Mouse . OverrideCursor = Cursors . Wait;
                    Debug . WriteLine ( $"Loading {GridData . Count} records from current table to new 'Columns only' table {NewTableName}" );
                    // We have a new EMPTY table, so add data from correct columns only

                    // get all the columns into a list
                    List<DataGridColumn> colnames = new List<DataGridColumn> ( );
                    colnames = datagridControl . Columns . ToList ( );
                    int index = 0;
                    int outcoloffset = 0;
                    foreach ( GenericClass item in Genericgrid . GenControl . GridData )
                    {
                        try
                        {
                            GenericClass row = new GenericClass ( );
                            outcoloffset = 0;
                            //for ( int y = 0 ; y < TableStruct . Count ; y++ )
                            for ( int z = 0 ; z < columnoffsets . Length ; z++ )
                            {
                                int offset = 0;
                                string tmp1 = TableStruct [ z ] . fname;
                                outcoloffset = columnoffsets [ z ];
                                string fld = GetFieldDatabyOffset ( item , outcoloffset );
                                SaveDataToRow ( fld . Trim ( ) , z , ref row );
                                //             string [ ] splitter = tmp1 . Split ( "," );
                                //             string colname = splitter [ 0 ].ToUpper();
                                //index = 0;
                                // foreach ( var col in colnames )
                                //{
                                //       if ( col . Header . ToString ( ) . Contains ( "field" ) )
                                //        break;
                                //  if ( col . Header . ToString ( ) . ToUpper ( ) == colname )
                                //    {
                                //        offset = TableStruct [ y ] . colindex;
                                //        string fld = GetFieldDatabyOffset ( item , index );
                                //       SaveDataToRow ( fld.Trim() , outcoloffset++ , ref row );
                                //        break;
                                //    }
                                //    else
                                //        index++;
                                //}
                            }
                            LimitedColumnTable . Add ( row );
                        }
                        catch ( Exception ex )
                        {
                            Debug . WriteLine ( $"{ex . Message}, columns count={TableStruct . Count}" );
                            error = $"Table creation failed... Reason = [{ex . Message}]";
                            return null;
                        }
                    }   // end  foreach

                    // Return the collection includng all selected columns only
                    return LimitedColumnTable;
                }       // end if
            }
            catch ( Exception ex )
            {
                Debug . WriteLine ( $"{ex . Message}, columns count={TableStruct . Count}" );
                error = $"Table creation failed... Reason = [{ex . Message}]";
                "" . Track ( 1 );
                return null;
            }
            "" . Track ( 1 );
            return null;
        }
        public string GetFieldDatabyOffset ( GenericClass datarec , int offset )
        {
            //offset--;
            if ( offset == 0 ) return datarec . field1;
            if ( offset == 1 ) return datarec . field2;
            if ( offset == 2 ) return datarec . field3;
            if ( offset == 3 ) return datarec . field4;
            if ( offset == 4 ) return datarec . field5;
            if ( offset == 5 ) return datarec . field6;
            if ( offset == 6 ) return datarec . field7;
            if ( offset == 7 ) return datarec . field8;
            if ( offset == 8 ) return datarec . field9;
            if ( offset == 9 ) return datarec . field10;
            if ( offset == 10 ) return datarec . field11;
            if ( offset == 11 ) return datarec . field12;
            if ( offset == 12 ) return datarec . field13;
            if ( offset == 13 ) return datarec . field14;
            if ( offset == 14 ) return datarec . field15;
            if ( offset == 15 ) return datarec . field16;
            if ( offset == 16 ) return datarec . field17;
            if ( offset == 17 ) return datarec . field18;
            if ( offset == 18 ) return datarec . field19;
            return "";
        }
        public void SaveDataToRow ( string fld , int outcoloffset , ref GenericClass row )
        {
            if ( outcoloffset == 0 ) { row . field1 = fld; return; }
            if ( outcoloffset == 1 ) { row . field2 = fld; return; }
            if ( outcoloffset == 2 ) { row . field3 = fld; return; }
            if ( outcoloffset == 3 ) { row . field4 = fld; return; }
            if ( outcoloffset == 4 ) { row . field5 = fld; return; }
            if ( outcoloffset == 5 ) { row . field6 = fld; return; }
            if ( outcoloffset == 6 ) { row . field7 = fld; return; }
            if ( outcoloffset == 7 ) { row . field8 = fld; return; }
            if ( outcoloffset == 8 ) { row . field9 = fld; return; }
            if ( outcoloffset == 9 ) { row . field10 = fld; return; }
            if ( outcoloffset == 10 ) { row . field11 = fld; return; }
            if ( outcoloffset == 11 ) { row . field12 = fld; return; }
            if ( outcoloffset == 12 ) { row . field13 = fld; return; }
            if ( outcoloffset == 13 ) { row . field14 = fld; return; }
            if ( outcoloffset == 14 ) { row . field15 = fld; return; }
            if ( outcoloffset == 15 ) { row . field16 = fld; return; }
            if ( outcoloffset == 16 ) { row . field17 = fld; return; }
            if ( outcoloffset == 17 ) { row . field18 = fld; return; }
            if ( outcoloffset == 18 ) { row . field19 = fld; return; }

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
        public static string CheckDbDomain ( string DbDomain )
        {
            if ( DapperGenLib . ConnectionStringsDict == null || DapperGenLib . ConnectionStringsDict . Count == 0 )
                LoadConnectionStrings ( );
            CheckResetDbConnection ( DbDomain , out string constring );
            DapperGenLib . CurrentConnectionString = constring;
            return constring;
        }

        public static void LoadConnectionStrings ( )
        {
            // This one works just fine - its in NewWpfDev
            //$"Entering " . cwinfo ( 0 );
            //Settings defaultInstance = ( ( Settings ) ( global::System . Configuration . ApplicationSettingsBase . Synchronized ( new Settings ( ) ) ) );
            //try
            //{
            //    if ( DapperGenLib . ConnectionStringsDict . Count > 0 )
            //        return;
            //    DapperGenLib . ConnectionStringsDict . Add ( "IAN1" , ( string ) Settings . Default [ "BankSysConnectionString" ] );
            //    DapperGenLib . ConnectionStringsDict . Add ( "NORTHWIND" , ( string ) Settings . Default [ "NorthwindConnectionString" ] );
            //    DapperGenLib . ConnectionStringsDict . Add ( "PUBS" , ( string ) Settings . Default [ "PubsConnectionString" ] );
            //    // TODO
            //    //WpfLib1.Utils.WriteSerializedCollectionJSON(Flags.ConnectionStringsDict, @"C:\users\ianch\DbConnectionstrings.dat");
            //}
            //catch ( NullReferenceException ex )
            //{
            //    Debug . WriteLine ( $"Dictionary  entrry [{( string ) Settings . Default [ "BankSysConnectionString" ]}] already exists" );
            //}
            //finally
            //{

            //}
            //$"Exiting " . cwinfo ( 0 );
        }
        public static bool CheckResetDbConnection ( string currdb , out string constring )
        {
            //string constring = "";
            //$"Entering " . cwinfo ( 0 );
            currdb?.ToUpper ( );
            // This resets the current database connection to the one we re working with (currdb - in UPPER Case!)- should be used anywhere that We switch between databases in Sql Server
            // It also sets the Flags.CurrentConnectionString - Current Connectionstring  and local variable
            if ( NewWpfDev . Utils . GetDictionaryEntry ( Flags . ConnectionStringsDict , currdb , out string connstring ) != "" )
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
                        //$"Exiting " . cwinfo ( 0 );
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
        //public static string GetDictionaryEntry ( Dictionary<string , string> dict , string key , out string dictvalue )
        //{
        //    string keyval = "";

        //    if ( dict . Count == 0 )
        //        NewWpfDev . Utils . LoadConnectionStrings ( );
        //    if ( dict . TryGetValue ( key . ToUpper ( ) , out keyval ) == false )
        //    {
        //        if ( dict . TryGetValue ( key , out keyval ) == false )
        //        {
        //            Debug . WriteLine ( $"Unable to access Dictionary {dict} to identify key value [{key}]" );
        //            NewWpfDev . Utils . DoErrorBeep ( 370 , 100 , 1 );
        //            NewWpfDev . Utils . DoErrorBeep ( 270 , 400 , 1 );
        //            key = key + "ConnectionString";
        //        }
        //    }
        //    dictvalue = keyval;
        //    return keyval;
        //}

        //*********************************//        
        #endregion utility support
        //*********************************//        

        public DataTable GetDataTable ( string commandline , string domain = "IAN1" )
        {
            DataTable dt = new DataTable ( );
            SqlConnection con;
            try
            {
                string ConString = CheckDbDomain ( CurrentTableDomain );
                if ( ConString == "" )
                {
                    // set to our lcal definition
                    ConString = MainWindow . SqlCurrentConstring;
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
                Debug . WriteLine ( $"Failed to load Db - {ex . Message}" );
                return null;
            }
            return dt;
        }

        public int ExecuteDapperCommand ( string spCommand , string [ ] args , out string err , int method = 0 )
        {
            int res = -1;
            err = "";
            Tablename = "";
            string Con = GenericDbUtilities . CheckSetSqlDomain ( CurrentTableDomain );
            if ( Con == "" )
            {
                Con = MainWindow . SqlCurrentConstring;
            }

            SqlConnection sqlCon = null;

            Mouse . OverrideCursor = Cursors . Wait;
            using ( sqlCon = new SqlConnection ( Con ) )
            {
                try
                {
                    //sqlCon . Open ( );
                    DynamicParameters parameters = new DynamicParameters ( );
                    parameters . AddDynamicParams ( parameters );

                    if ( args != null && args . Length > 0 && args [ 0 ] != "-" )
                    {
                        for ( int x = 0 ; x < args . Length ; x++ )
                        {
                            // breakout on first unused array element
                            if ( args [ x ] == "" ) break;

                            if ( args [ x ] != null )
                                parameters . Add ( $"Arg{x + 1}" , args [ x ] , DbType . String , ParameterDirection . Input , args [ x ] . Length );
                        }
                    }
                    //Debug . WriteLine ( $"ExecuteDapperCommand() : [ {spCommand . ToUpper ( )} ]" );
                    //$"{spCommand}" . DapperTrace ( );
                    res = sqlCon . Execute ( spCommand , args , commandType: CommandType . Text );
                }
                catch ( Exception ex )
                {
                    Debug . WriteLine ( $"{ex . Message}" );
                    err = ex . Message;
                }
            }
            return res;
        }

        public int ExecuteStoredProcedure ( string spCommand , string [ ] args , out string err )
        {
            int res = -9;
            err = "";
            Tablename = "";
            string Con = DbConnectionString;
            string cmddebug = "";
            SqlConnection sqlCon = null;

            Mouse . OverrideCursor = Cursors . Wait;
            try
            {
                using ( sqlCon = new SqlConnection ( Con ) )
                {
                    //sqlCon . Open ( );
                    DynamicParameters parameters = new DynamicParameters ( );
                    parameters . AddDynamicParams ( parameters );

                    if ( args . Length > 0 && args [ 0 ] != "-" )
                    {
                        for ( int x = 0 ; x < args . Length ; x++ )
                        {
                            // breakout on first unused array element
                            if ( args [ x ] == "" ) break;

                            if ( args [ x ] != null )
                            {
                                parameters . Add ( $"@Arg{x + 1}" , args [ x ] , DbType . String , ParameterDirection . Input , args [ x ] . Length );
                                cmddebug += $"Arg{x + 1} : [ {args [ x ]} ]\n";
                            }
                        }
                    }
                    //Debug . WriteLine ( $"[ {spCommand . ToUpper ( )} ]\n{cmddebug}" );
                    //                    Debug . WriteLine ( $"ExecuteStoredProcedure() : [ {spCommand . ToUpper ( )} ]" );
                    $"[ {spCommand . ToUpper ( )} ]\n{cmddebug}" . DapperTrace ( );
                    res = sqlCon . Execute ( spCommand , parameters , commandType: CommandType . StoredProcedure );

                    //    spCommand = $"drop table if exists {args [ 2 ]}";
                    //    ExecuteDapperCommand ( spCommand , null , out err );
                    //    spCommand = $"Select * into {args [ 2 ]} from {args [ 0 ]} where {args [ 1 ]}";
                    //    int result = ExecuteDapperCommand ( spCommand , null , out err );
                    //    if ( result > 0 )
                    //    {
                    //        spCommand = $"Select * from {args [ 2 ]} ";
                    //        GetDataViaDapper ( spCommand , args , out err , out result );
                    //    }
                    //    else
                    //    {
                    //        // failed or  no matches
                    //    }
                }
            }
            catch ( Exception ex )
            {
                Debug . WriteLine ( $"{ex . Message}" );
                err = ex . Message;
            }
            return res;
        }
        public int ExecuteDapperScalar ( string spCommand , string [ ] args , out string err , int method = 0 )
        {
            int res = -9;
            err = "";
            Tablename = "";
            string Con = DbConnectionString;
            string cmddebug = "";
            SqlConnection sqlCon = null;
            Con = GenericDbUtilities . CheckSetSqlDomain ( CurrentTableDomain );
            if ( Con == "" )
            {
                // set to our local definition
                Con = MainWindow . CurrentSqlTableDomain;
            }
            Mouse . OverrideCursor = Cursors . Wait;
            try
            {
                using ( sqlCon = new SqlConnection ( Con ) )
                {
                    //sqlCon . Open ( );
                    DynamicParameters parameters = new DynamicParameters ( );
                    parameters . AddDynamicParams ( parameters );

                    if ( args != null && args . Length > 0 && args [ 0 ] != "-" )
                    {
                        for ( int x = 0 ; x < args . Length ; x++ )
                        {
                            // breakout on first unused array element
                            if ( args [ x ] == "" ) break;

                            if ( args [ x ] != null )
                            {
                                parameters . Add ( $"@Arg{x + 1}" , args [ x ] , DbType . String , ParameterDirection . Input , args [ x ] . Length );
                                cmddebug += $"Arg{x + 1} : [ {args [ x ]} ]\n";
                            }
                        }
                    }
                    dynamic counter = 0;
                    Debug . WriteLine ( $"[ {spCommand . ToUpper ( )} ]\n{cmddebug}" );
                    //                    Debug . WriteLine ( $"ExecuteStoredProcedure() : [ {spCommand . ToUpper ( )} ]" );
                    $"[ {spCommand . ToUpper ( )} ]" . DapperTrace ( );

                    if ( method == 0 ) // execute code on server
                    {
                        //**************************************************************************************************************//
                        res = sqlCon . Execute ( spCommand , parameters , commandType: CommandType . Text );
                        //**************************************************************************************************************//
                    }
                    else if ( method == 1 ) // Get int return value
                    {
                        //**************************************************************************************************************//
                        counter = sqlCon . Query ( spCommand , parameters , commandType: CommandType . Text );
                        //**************************************************************************************************************//

                        dynamic str9 = Genericgrid . GetStringFromDynamic ( counter );
                        string stdstring = str9 . ToString ( );
                        string [ ] splitter = stdstring . Split ( "," );
                        res = Convert . ToInt32 ( splitter [ 0 ] . ToString ( ) );
                        // IEnumerable< dynamic > dynres = Convert . ToInt32 ( Genericgrid . GetStringFromDynamic ( counter ) );
                    }
                    else if ( method == 2 ) // execute a Stored procedure
                    {
                        //**************************************************************************************************************//
                        res = sqlCon . Execute ( spCommand , parameters , commandType: CommandType . StoredProcedure );
                        //**************************************************************************************************************//
                    }
                    else if ( method == 3 )
                    {

                        //**************************************************************************************************************//
                        res = sqlCon . Execute ( spCommand , parameters , commandType: CommandType . TableDirect );
                        //**************************************************************************************************************//
                    }
                    //    spCommand = $"drop table if exists {args [ 2 ]}";
                    //    ExecuteDapperCommand ( spCommand , null , out err );
                    //    spCommand = $"Select * into {args [ 2 ]} from {args [ 0 ]} where {args [ 1 ]}";
                    //    int result = ExecuteDapperCommand ( spCommand , null , out err );
                    //    if ( result > 0 )
                    //    {
                    //        spCommand = $"Select * from {args [ 2 ]} ";
                    //        GetDataViaDapper ( spCommand , args , out err , out result );
                    //    }
                    //    else
                    //    {
                    //        // failed or  no matches
                    //    }
                }
            }
            catch ( Exception ex )
            {
                NewWpfDev . Utils . DoErrorBeep ( );
                Debug . WriteLine ( $"{ex . Message}" );
                err = ex . Message;
            }
            return res;
        }
        public ObservableCollection<GenericClass> GetDataFromStoredProcedure ( string spCommand , string [ ] args , string CurrentTableDomain , out string err , out int recordcount , int method = 0 )
        {
            IEnumerable result2 = null;
            recordcount = 0;
            Tablename = "";
            string Con = DbConnectionString;
            SqlConnection sqlCon = null;
            err = "";
            int res = -8;
            "" . Track ( );

            ObservableCollection<GenericClass> GenClass = new ObservableCollection<GenericClass> ( );

            Mouse . OverrideCursor = Cursors . Wait;
            try
            {
                Con = GenericDbUtilities . CheckSetSqlDomain ( CurrentTableDomain );
                if ( Con == "" )
                {
                    // set to our local definition
                    Con = MainWindow . CurrentSqlTableDomain;
                }
                //                DatagridControl . CurrentTableDomain = CurrentTableDomain;
                using ( sqlCon = new SqlConnection ( Con ) )
                {
                    //sqlCon . Open ( );
                    // Now add record  to SQL table
                    DynamicParameters parameters = new DynamicParameters ( );
                    parameters . AddDynamicParams ( parameters );

                    if ( args != null && args . Length > 0 && args [ 0 ] != "-" )
                    {

                        for ( int x = 0 ; x < args . Length ; x++ )
                        {
                            // breakout on first unused array element
                            if ( args [ x ] == "" ) break;

                            if ( args [ x ] != null )
                            {
                                parameters . Add ( $"Arg{x + 1}" , args [ x ] ,
                                DbType . String ,
                                ParameterDirection . Input ,
                                args [ x ] . Length );
                            }
                        }
                    }
                    IEnumerable<dynamic> res3 = null;


                    if ( method == 0 )
                    {
                        // Executing SQL via Stored procedure
                        //$"{spCommand}" . DapperTrace ( );
                        res3 = sqlCon . Query ( spCommand , parameters , commandType: CommandType . StoredProcedure );
                        //                         var reslt = res3.Count ( );
                        //recordcount = res3 . Get<int> ("@arg1" );
                    }
                    else
                    {
                        // Executing SQL code  directly
                        //$"{spCommand}" . DapperTrace ( );
                        res3 = sqlCon . Query ( spCommand , parameters , commandType: CommandType . Text );
                    }
                    recordcount = res3 . Count ( );
                    $"{spCommand} Record count returned  = {recordcount}" . DapperTrace ( );
                    recordcount = res3 . Count ( );

                    if ( res3 != null )
                    {
                        // Process data returned (in dynamic DapperRows)
                        Dictionary<string , object> dict = new Dictionary<string , object> ( );
                        List<int> VarCharLength = new List<int> ( );
                        bool GetLengths = false;
                        //Although this is duplicated  with the one above we CANNOT make it a method()
                        int dictcount = 0;
                        dict . Clear ( );
                        long zero = res3 . LongCount ( );
                        try
                        {
                            int colcount = 0, fldcount = 0;
                            foreach ( var item in res3 )
                            {
                                GenericClass gc = new GenericClass ( );
                                try
                                {
                                    //	Create a dictionary for each row of data then add it to a GenericClass row then add row to Generics Db
                                    gc = ParseDapperRow ( item , out dict , out colcount , ref VarCharLength , GetLengths );
                                    dictcount = 1;
                                    fldcount = dict . Count;
                                    if ( fldcount == 0 )
                                    {
                                        //no problem, we will get an Obs collection anyway
                                        return GenClass;
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
                                            NewWpfDev . Utils . DoErrorBeep ( );
                                            Debug . WriteLine ( $"Dictionary ERROR : {ex . Message}" );
                                            err = ex . Message;
                                        }
                                    }
                                }
                                catch ( Exception ex )
                                {
                                    err = $"SQLERROR : {ex . Message}";
                                    NewWpfDev . Utils . DoErrorBeep ( );
                                    Debug . WriteLine ( err );
                                    return GenClass;
                                }
                                GenClass . Add ( gc );
                                dict . Clear ( );
                                dictcount = 1;
                            }
                        }
                        catch ( Exception ex )
                        {
                            Debug . WriteLine ( $"OUTER DICT/PROCEDURE ERROR : {ex . Message}" );
                            NewWpfDev . Utils . DoErrorBeep ( );
                            if ( ex . Message . Contains ( "not find stored procedure" ) )
                            {
                                err = $"SQL PARSE ERROR - [{ex . Message}]";
                                return GenClass;
                            }
                            else
                            {
                                long x = res3 . LongCount ( );
                                if ( x == ( long ) 0 )
                                {
                                    err = $"ERROR : [{spCommand}] returned ZERO records... ";
                                    return GenClass;
                                }
                                else
                                {
                                    err = ex . Message;
                                }
                                return GenClass;
                            }
                        }
                    }
                }
            }
            catch ( Exception ex )
            {
                Debug . WriteLine ( $"{ex . Message}" );
                Utils . DoErrorBeep ( );
                err = ex . Message;
            }
            Mouse . OverrideCursor = Cursors . Arrow;
            "" . Track ( 1 );
            return GenClass;
        }

        static public DynamicParameters ParseSqlArgs ( DynamicParameters parameters , string [ ] args )
        {
            if ( args != null && args . Length > 0 && args [ 0 ] != "-" )
            {
                for ( int x = 0 ; x < args . Length ; x++ )
                {
                    // breakout on first unused array element
                    if ( args [ x ] == "" ) break;
                    if ( args [ x ] . ToUpper ( ) . Contains ( "OUTPUT" ) )
                    {
                        string [ ] splitter = args [ x ] . Split ( " " );
                        parameters . Add ( $"{splitter [ 1 ]}" , splitter [ 1 ] ,
                                           DbType . String ,
                                           ParameterDirection . Output ,
                                           splitter [ 1 ] . Length );
                    }
                    else
                    {
                        parameters . Add ( $"Arg{x + 1}" , args [ x ] ,
                       DbType . String ,
                       ParameterDirection . Input ,
                       args [ x ] . Length );
                    }
                }
            }
            return parameters;
        }
        //****************************************************************//
        public dynamic GetDataViaDapper ( string SqlCommand , string [ ] args , out List<Dictionary<string , string>> ColumnTypesList , out int count , int method = 0 )
        {
            string connectionString = MainWindow . SqlCurrentConstring;
            // ColumnTypesList . Clear ( ); 
            SqlConnection sqlCon = null;
            ObservableCollection<GenericClass> temp = new ObservableCollection<GenericClass> ( );
            dynamic res = null;
            List<Dictionary<string , string>> list = new List<Dictionary<string , string>> ( );
            ColumnTypesList = list;
            count = 0;

            string Con = GenericDbUtilities . CheckSetSqlDomain ( CurrentTableDomain );
            if ( Con == "" )
            {
                // set to our local definition
                Con = MainWindow . SqlCurrentConstring;
            }
            try
            {
                using ( sqlCon = new SqlConnection ( Con ) )
                {

                    var parameters = new DynamicParameters ( );
                    parameters = ParseSqlArgs ( parameters , args );
                    //Debug . WriteLine ( $"GetCountFromStoredProc() : [ {SqlCommand . ToUpper ( )} ]" );
                    $"{SqlCommand}" . DapperTrace ( );

                    //********************************************************************************************************//
                    //var thisres =  sqlCon . Query ( SqlCommand , parameters , commandType: CommandType . StoredProcedure );
                    IEnumerable<dynamic> intval = sqlCon . Query ( SqlCommand , parameters , commandType: CommandType . StoredProcedure );
                    //Dictionary<string , object> dyndict = new Dictionary<string , object> ( );
                    //Dictionary<string , object> objdict = new Dictionary<string , object> ( );
                    //dyndict . Add ( "result" , intval );
                    dynamic newcount = null;
                    foreach ( var item in intval )
                    {
                        newcount = item . count;

                        // set (outint count) parameter
                        count = newcount;
                    }
                    return newcount;
                }
            }
            catch ( Exception ex )
            {
                Utils . DoErrorBeep ( );
                Debug . WriteLine ( $"SQL error : [ {ex . Message} ]" );
            }
            return null;
        }

        public int GetCountFromStoredProc ( string SqlCommand , string [ ] args )
        {
            string connectionString = DbConnectionString;
            SqlConnection sqlCon = null;

            using ( sqlCon = new SqlConnection ( connectionString ) )
            {
                sqlCon . Open ( );

                var parameters = new DynamicParameters ( );
                parameters = ParseSqlArgs ( parameters , args );

                Debug . WriteLine ( $"GetCountFromStoredProc() : [ {SqlCommand . ToUpper ( )} ]" );
                int outval = sqlCon . Execute ( SqlCommand , parameters , commandType: CommandType . StoredProcedure );
                return outval;
            }
            return -1;
        }

        static public int TestIfFileExists ( string tablename , out string err )
        {
            err = "";
            int output = 0; SqlConnection conn = new SqlConnection ( );
            SqlCommand cmd = new SqlCommand ( );
            conn . ConnectionString = DbConnectionString;
            //conn . ConnectionString = ConfigurationManager . ConnectionStrings [ "CS" ] . ConnectionString;
            cmd . Connection = conn;
            cmd . CommandType = CommandType . StoredProcedure;
            cmd . CommandText = "spcheckforTable";

            ///*The 3 output parameters etc*/
            #region info code
            {
                //cmd . Parameters . Add ( "@tablename" , SqlDbType . VarChar , 100 );
                //cmd . Parameters [ "@tablename" ] . Direction = ParameterDirection . Input;
                //////cmd . Parameters [ "@ProductName" ] . Direction = ParameterDirection . Input;
                // cmd . Parameters . Add ( "@Output" , SqlDbType . VarChar , ParameterDirection . Output;);
                //cmd . Parameters . Add ( "@QuantityPerUnit" , SqlDbType . VarChar , 20 );
                //cmd . Parameters [ "@QuantityPerUnit" ] . Direction = ParameterDirection . Output;
                // cmd . Parameters . Add ( "asd" , SqlDbType . VarChar , 100 );
                // cmd . Parameters [ "@Arg1" ] . Direction = ParameterDirection . Input;
                #endregion info code

            }
            cmd . Parameters . AddWithValue ( "@arg1" , tablename );
            cmd . Parameters . Add ( "@Output" , SqlDbType . VarChar , 20 );
            cmd . Parameters [ "@Output" ] . Direction = ParameterDirection . Output;
            try
            {
                conn . Open ( );
                //Executing the SP

                int i = cmd . ExecuteNonQuery ( );
                //Storing the output parameters value in 3 different variables.
                output = Convert . ToInt32 ( cmd . Parameters [ "@Output" ] . Value );
                if ( output > 0 )
                    err = "success";
                else err = "NOEXIST";
            }
            catch ( Exception ex )
            {
                NewWpfDev . Utils . DoErrorBeep ( );
                err = "EXISTS : {ex.Message}, {ex.Data}";
            }
            finally
            {
                conn . Close ( );
            }
            return output;
        }
        public void StdError ( )
        {
            DapperGenericsLib . Utils . DoErrorBeep ( 400 , 100 , 1 );
            DapperGenericsLib . Utils . DoErrorBeep ( 300 , 400 , 1 );
        }

        public string GetFullColumnInfo ( string spName , string CurrentTable , string ConString , bool ShowFdl = true , bool ShowOutput = true )
        {
            string output = "";
            string errormsg = "";
            int columncount = 0;
            DataTable dt = new DataTable ( );
            ObservableCollection<GenericClass> Generics = new ObservableCollection<GenericClass> ( );
            List<string> genericlist = new List<string> ( );
            try
            {
                List<GenericToRealStructure> list = new List<GenericToRealStructure> ( );
                // Generate a new table structure by passing CURRENT table name
                // so we get a buffer containing the column info 
                Generics = CreateFullColumnInfo ( CurrentTable , ConString , false );
                string buffer = "";
                if ( Generics . Count ( ) > 0 )
                {
                    // process donor table structure
                    foreach ( var item in Generics )
                    {
                        buffer += $"{item . field1 . Trim ( )}";    // fname
                        buffer += $":{item . field2 . Trim ( )}";
                        if ( item . field3 != null && item . field3 != "0" )
                            buffer += $":{item . field3 . Trim ( )}";
                        if ( item . field4 != null && item . field4 != "0" )
                            buffer += $":{item . field4 . Trim ( )}\n";
                        else
                            buffer += $"\n";
                    }
                    if ( ShowFdl == false )
                        return buffer;  // contains fields :fields :fields :fields\n:  we need to create a new table structure eg: Create new Table (......)
                }
                string [ ] args = new string [ 1 ];
                args [ 0 ] = spName;
                dt = ProcessSqlCommand ( "spGetTableColumnWithSizes" , ConString , args );
            }
            catch ( Exception ex )
            {
                MessageBox . Show ( $"SQL ERROR 1125 - {ex . Message}" );
                return "";
            }
            if ( dt . Rows . Count == 0 )
            {
                columncount = 0;
                return "";
            }
            if ( ShowOutput == false )
                output = "";
            foreach ( var item in dt . Rows )
            {
                string store = "";
                DataRow dr = item as DataRow;
                columncount = dr . ItemArray . Count ( );
                if ( columncount == 0 )
                    columncount = 1;
                // we only need max cols - 1 here !!!
                for ( int x = 0 ; x < columncount ; x++ )
                {
                    if ( dr . ItemArray [ x ] . ToString ( ) != "{}" )
                    {
                        store = dr . ItemArray [ x ] . ToString ( ) . ToUpper ( );
                        if ( store != CurrentTable )
                            continue;

                        store = store . Substring ( 0 , store . Length - 1 );
                        store += dr . ItemArray [ x ] . ToString ( ) + ",";
                    }
                    else store = store . Substring ( 0 , store . Length - 1 );
                }
                if ( store . Contains ( ",," ) ) store = store . Substring ( 0 , store . Length - 2 );
                store = NewWpfDev . Utils . ReverseString ( store );
                if ( store [ 0 ] == ',' )
                {
                    store = store . Substring ( 1 );
                    store = NewWpfDev . Utils . ReverseString ( store );
                }
                else
                    store = NewWpfDev . Utils . ReverseString ( store );
                output += store + ":";
                output = output . Substring ( 0 , output . Length - 1 );
                // output string are seperated by COLONs
                // we now have the err, so lets process them
                // data is fieldname, sql-datatype, size (where appropriate)
                string buffer = output;
                string [ ] lines = buffer . Split ( ':' );

                string tmp = "";
                if ( output != "" ) { output = output . Trim ( ) + "\n"; } //output += $"\n{item}  "; }
                else output += $"{item} \n";
            }
            // we now have a list of the Args for the selected SP in output terminated by \n
            // Show it in a TextBox if it takes 1 or more args
            // format is ("fielddname, fieldtype, size1, size2\n,")
            //           return output;

            output = DapperSupport . GetFullColumnsStructure ( spName , CurrentTable , ConString , ShowFdl = true , ShowOutput = true );
            if ( output != "" && ShowFdl )
            {
                string fdinput = $"Procedure Name : {spName . ToUpper ( )}\n";
                fdinput += output;
                fdl . ShowInfo ( Flowdoc , canvas , line1: fdinput , clr1: "Black0" , line2: "" , clr2: "Black0" , line3: "" , clr3: "Black0" , header: "" , clr4: "Black0" );
                canvas . Visibility = Visibility . Visible;
                Flowdoc . Visibility = Visibility . Visible;
            }
            if ( ShowFdl == false )
            {
                if ( errormsg == "" )
                {
                    MessageBox . Show ( $"No Argument information is available" , $"[{spName}] SP Script Information" , MessageBoxButton . OK , MessageBoxImage . Warning );
                    return "";
                }
            }
            return output;
        }

    }
}