#define USENEWARGS
#undef USENEWARGS
using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Data;
using System . Diagnostics;
using System . Diagnostics . Eventing . Reader;
using System . Drawing . Printing;
using System . Linq . Expressions;
using System . Numerics;
using System . Security . Cryptography;
using System . ServiceProcess;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Controls . Primitives;
using System . Windows . Documents;
using System . Windows . Input;
using System . Windows . Media;
using System . IO;
using System . Collections . Generic;

using NewWpfDev;

using UserControls;
using System . Printing;
using System . Threading;
using System . Windows . Threading;
using Microsoft . Data . SqlClient;
using System . Net;
using Microsoft . Xaml . Behaviors . Media;
using System . Reflection;
using NewWpfDev . Expandos;
using NewWpfDev . StoredProcs;

using UtilityWindows;
using System . Windows . Media . TextFormatting;
using System . Threading . Tasks;
using System . Security . RightsManagement;

namespace Views
{
    /// <summary>
    /// Interaction logic for SpResultsViewer.xaml
    /// </summary>
    public partial class SpResultsViewer : Window
    {
        #region declarations
        public SpResultsViewer spviewer;
        public ObservableCollection<string> Executedata = new ObservableCollection<string> ( );
        Genericgrid Gengrid { get; set; }
        public string Searchtext { get; set; }
        public string Searchterm { get; set; }
        public bool CloseArgsViewerOnPaste { get; set; }
        public bool ShowTypesInArgsViewer { get; set; } = true;
        public bool ShowParseDetails { get; set; }
        public static FlowDocScrollViewerSupport? FdSupport { get; set; }
        private static string [ ] arguments = new string [ DEFAULTARGSSIZE ];
        public GengridExecutionResults GenResults { get; set; }

        public static bool IsMoving = false;
        public ObservableCollection<GenericClass> DataGrid = new ObservableCollection<GenericClass> ( );
        static private DragCtrlHelper? DragCtrl;
        public FrameworkElement ActiveDragControl { get; set; }
        public static dynamic? spViewerexpobj { get; private set; }
        public bool IsLoading { get; set; } = true;
        public bool ShowOptionsPanel { get; set; } = true;
        public bool ShowCheckboxes { get; set; } = true;
        public bool IsFlashing { get; set; } = false;
        public static string SpListboxFontSize { get; set; } = "14";
        public static string ScrollViewerFontSize { get; set; } = "14";
        public bool IsDirty { get; set; } = false;

        #endregion declarations

        int Fontsize { set; get; } = 14;
        int SpListFontsize { set; get; } = 14;
 
        public const int DEFAULTARGSSIZE = 6;
        public static bool SHOWSIZEARG = false;

        #region full props

        private object movingobject;
        public object MovingObject
        {
            get { return movingobject; }
            set { movingobject = value; }
        }
        private object movingobject2;
        public object MovingObject2
        {
            get { return movingobject2; }
            set { movingobject2 = value; }
        }

        #endregion full props

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

        #region Dependency properties

        public int zOrder
        {
            get { return ( int ) GetValue ( zOrderProperty ); }
            set { SetValue ( zOrderProperty , value ); }
        }
        public static readonly DependencyProperty zOrderProperty =
            DependencyProperty . Register ( "zOrder" , typeof ( int ) , typeof ( SpResultsViewer ) , new PropertyMetadata ( 0 ) );
        public bool ShowingAllSPs
        {
            get { return ( bool ) GetValue ( ShowingAllSPsProperty ); }
            set
            {
                SetValue ( ShowingAllSPsProperty , value );
                Debug . WriteLine ( $"ShowingAllSps set to {value}" );
                OnPropertyChanged ( "ShowingAllSpsProperty" );
            }
        }
        public static readonly DependencyProperty ShowingAllSPsProperty =
            DependencyProperty . Register ( "ShowingAllSPs" , typeof ( bool ) , typeof ( Genericgrid ) , new PropertyMetadata ( ( bool ) true ) );
        public bool UsingMatches
        {
            get { return ( bool ) GetValue ( UsingMatchesProperty ); }
            set { SetValue ( UsingMatchesProperty , value ); }
        }
        public static readonly DependencyProperty UsingMatchesProperty =
            DependencyProperty . Register ( "UsingMatches" , typeof ( bool ) , typeof ( SpResultsViewer ) , new PropertyMetadata ( ( bool ) false ) );
        public string gm41Text
        {
            get { return ( string ) GetValue ( gm41TextProperty ); }
            set { SetValue ( gm41TextProperty , value ); }
        }
        public static readonly DependencyProperty gm41TextProperty =
            DependencyProperty . Register ( "gm41Text" , typeof ( string ) , typeof ( SpResultsViewer ) , new PropertyMetadata ( "Expand Arguments entry panel" ) );
        public string TooltipText
        {
            get { return ( string ) GetValue ( TooltipTextProperty ); }
            set { SetValue ( TooltipTextProperty , value ); }
        }
        public static readonly DependencyProperty TooltipTextProperty =
            DependencyProperty . Register ( "TooltipText" , typeof ( string ) , typeof ( SpResultsViewer ) , new PropertyMetadata ( "" ) );
        //public ToolTip ExecTooltip
        //{
        //    get { return ( ToolTip ) GetValue ( ExecTooltipProperty ); }
        //    set { SetValue ( ExecTooltipProperty , value ); }
        //}
        //public static readonly DependencyProperty ExecTooltipProperty =
        //    DependencyProperty . Register ( "ExecTooltip" , typeof ( ToolTip ) , typeof ( SpResultsViewer ) , new PropertyMetadata ( "" ) );

        #endregion Dependecy properties

        public SpResultsViewer ( Genericgrid genControl , string sproc , string searchterm )
        {
            InitializeComponent ( );
            WpfLib1 . Utils . SetupWindowDrag ( this );
            spviewer = this;
            DataContext = this;
            Gengrid = genControl;
            FdSupport = new FlowDocScrollViewerSupport ( );
            DragCtrl = new DragCtrlHelper ( SqlTablesViewer );
            Genericgrid . Resultsviewer = this;
            string spname = Gengrid . SpName . Text;
            Searchtext = Gengrid . Searchtext;
            string [ ] args = new string [ 1 ];
            Mouse . OverrideCursor = Cursors . Wait;
            canvas . Visibility = Visibility . Visible;
            // setup a pointer to ourselves
            this . Topmost = false;
            Gengrid . ExecuteLoaded = true;
            hspltter . Cursor = Cursors . ScrollNS;

            IsLoading = true;

            ShowTypesInArgsViewer = ( bool ) MainWindow . GetSystemSetting ( "ShowTypesInSpArgumentsString" );
            CloseArgsViewerOnPaste = ( bool ) MainWindow . GetSystemSetting ( "AutoCloseSpArgumentsViewer" );
             if ( ( bool ) MainWindow . GetSystemSetting ( "SpViewerUseDarkMode" ) == true )
            {
                SpViewerResults . Background = FindResource ( "Black3" ) as SolidColorBrush;
             prompter . Foreground = FindResource ( "White0" ) as SolidColorBrush;
                BannerGrid . Background = FindResource ( "Orange5" ) as SolidColorBrush;
                MovingObject = SqlTablesViewer;
            }
            if ( spViewerexpobj == null )
                spViewerexpobj = ExpandoClass . GetNewExpandoObject ( );

            var t = new Dictionary<string , object> ( );
            t . Add ( "asdda" , 4456 );
            IsLoading = false;
            int selitem = ListResults . SelectedIndex;
            selitem = selitem != -1 ? selitem : 0;
            ListResults . SelectedIndex = selitem;
            ListResults . SelectedItem = selitem;
            this . BringIntoView ( );
            this . Focus ( );
        }

        private void Spresultsviewer_Loaded ( object sender , RoutedEventArgs e )
        {
            if ( ShowingAllSPs == false )
                ShowingAllSPs = true;
            IsLoading = false;
            this . Focus ( );
            //            ScrollFontSize . Content = ListResults . FontSize; ;
            SPFontSize . Content = ScrollViewerFontSize;
            optype . SelectedIndex = 4;
            ExName . Text = optype . SelectedItem . ToString ( );
            this . Focus ( );
        }

        private void ListResults_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {
            // Load data into Scrollviewer
            string selname = "";
            ListBox lb = sender as ListBox;
            if ( lb . SelectedItem != null )
            {
                selname = lb . SelectedItem . ToString ( );
                // Store search term in our dialog for easier access

                if ( TextResult . Document != null )
                {
                    TextResult . Document . Blocks . Clear ( );
                    TextResult . Document = null;
                }
                string sptext = "";
                // Update cosmetics
                if ( IsLoading == false )
                {
                    bool result = Gengrid . LoadShowMatchingSproc ( this , TextResult , selname , ref sptext , Convert . ToInt32 ( ScrollViewerFontSize ) );
                    ShowParseDetails = true;
                    if ( ShowParseDetails )
                    {
                        string Arguments = SProcsDataHandling . GetSpHeaderBlock ( sptext , spviewer );
                        if ( Arguments . Length == 0 || Arguments . Contains ( "No valid Arguments were found" ) == true
                            || Arguments . Contains ( "Either the \"AS\" or \"BEGIN \" statements are missing" )
                            || Arguments . StartsWith ( "ERROR -" ) )
                        {
                            SPArguments . Text = "The Header Block or parameters in the S.Procedure appear to be invalid !";
                            SPArgumentsFull . Text = SPArguments . Text;
                            Parameterstop . Text = Arguments;
                        }
                        else
                        {
                            SPArguments . Text = Arguments;
                            SPArgumentsFull . Text = SPArguments . Text;
                        }
                    }
                    e . Handled = true;
                }
                else
                {
                    //Reset arguments panel
                    SPArguments . Text = "Argument(s) required ?";
                    SPArgumentsFull . Text = SPArguments . Text;
                    optype . SelectedIndex = -1;    // unselect selection of S.P listbox
                    optype . SelectedItem = null;    // unselect method listbox
                }
            }
            SPName . Text = selname;
        }

         //. NOT IN USE
        private async void Execsp_Click ( object sender , RoutedEventArgs e )
        {
            string spname = Gengrid . SpName . Text;
            string [ ] args = new string [ 1 ];
            args [ 0 ] = Genericgrid . SpSearchTerm;
            //Store search term for use  by later dialogs
            Genericgrid . SpSearchTerm = Gengrid . selectSp . Text . ToUpper ( );
            Debug . WriteLine ( $"Executing S.P {spname}" );
            List<string> results = GenDapperQueries . ProcessUniversalQueryStoredProcedure ( spname , args , Genericgrid . CurrentTableDomain , out string err );
            //            List<string> processResults = new List<string> ( );
            if ( results . Count > 0 )
            {
                Task task = Task . Run ( async ( ) =>
                {
                    await fetchandloadAllSProcs ( results );
                    //                    sptext = Gengrid . FetchStoredProcedureCode ( Splist . SelectedItem . ToString ( ) , ref stringresult );
                } );
            }
            e . Handled = true;
        }


        /// <summary>
        /// Clever method that loads/reloads ALL matching S.P's in Sql Server 
        /// </summary>
        /// <param name="selindex"></param>
        /// <param name="srchtext"></param>
        public void LoadSpList ( dynamic sender , int selindex , string srchtext )
        {
            //*********************************//
            //only called by Resultsviewer
            //*********************************//
            // load ALL Sp's into Execution Viwer
            int indx = 0;
            string curritem = "";
            curritem = ListResults . SelectedItem?.ToString ( );
            ListResults . ItemsSource = null;
            ListResults . Items . Clear ( );
            // Update cosmetics
            Gengrid . LoadMatchingStoredProcs ( ListResults , srchtext );
            // default to 1st  entry in list
            // reset selected item
            foreach ( var item in ListResults . Items )
            {
                if ( curritem != null && curritem != "" )
                {
                    if ( item . ToString ( ) == curritem )
                    {
                        ListResults . SelectedIndex = indx;
                        ListResults . ScrollIntoView ( item . ToString ( ) );
                        break;
                    }
                }
                else
                {
                    ListResults . SelectedIndex = 0;
                    ListResults . ScrollIntoView ( item . ToString ( ) );
                    break;
                }
                indx++;
            }
            Mouse . OverrideCursor = Cursors . Arrow;
        }
        public async Task fetchandloadAllSProcs ( List<string> results )
        {
            //*********************************//
            // only called  by Resultsviewer
            //*********************************//
            List<string> processResults = new List<string> ( );
            int selindex = 0;
            string line = "";
            string sptext = "";
            Gengrid . FetchStoredProcedureCode ( line , ref sptext );
            if ( ListResults . SelectedIndex >= 0 )
                selindex = ListResults . SelectedIndex;
            else
                selindex = 0;
            LoadSpList ( this , selindex , Searchtext );
            ListResults . Items . Clear ( );
            ListResults . SelectedIndex = 0;
            FlowDocument fd = new FlowDocument ( );
            fd . Blocks . Clear ( );

            // Store search term in our dialog for easier access
            Genericgrid . SpSearchTerm = SrchTerm . Text = Gengrid . selectSp . Text . ToUpper ( );

            fd = Gengrid . CreateBoldString ( fd , sptext , Genericgrid . SpSearchTerm . ToUpper ( ) );
            fd . Background = FindResource ( "Black3" ) as SolidColorBrush;
            TextResult . Document = fd;
            return;
        }

        private void closeresultsviewer_Click ( object sender , RoutedEventArgs e )
        {
            Gengrid . ExecuteLoaded = false;
            this . Close ( );
        }

        private void SpStrings_KeyDown ( object sender , KeyEventArgs e )
        {

        }

        private void ExecuteSp ( )
        {
            DoExecute_Click ( null , null );
        }

        private void DoExecute_Click ( object sender , RoutedEventArgs e )
        {
            Type newtype;
            string arguments = "";
            int v = 0;
            string err = "";
            bool IsCmd = false;
            bool IsSproc = false;

            if ( optype . SelectedItem == null )
            {
                MessageBox . Show ( "You MUST select an Execution Method before the selected S.P can be executed !" , "Execution processing error" );
                return;
            }
            ExName . Text = optype . SelectedItem . ToString ( );

            if ( SPArguments . Text != SPArgumentsFull . Text )
            {
                if ( SPArguments . Text . Contains ( "STRING" ) == true && SPArgumentsFull . Text . Contains ( "STRING" ) == false )
                    SPArguments = SPArgumentsFull;
                else if ( SPArguments . Text . Contains ( "STRING" ) == false && SPArgumentsFull . Text . Contains ( "STRING" ) == true )
                    SPArgumentsFull = SPArguments;
            }

            int count = 0;
            string ResultString = "";
            Type objtype = null;
            object obj = new object ( );
            int testint = 0;
            if ( SPArguments . Text . ToUpper ( ) . StartsWith ( "CMD" ) )
                IsCmd = true;
            else if ( SPArguments . Text . ToUpper ( ) . StartsWith ( "SP" ) )
                IsSproc = true;

            ///////==================================================================///////
            dynamic dynvar = Execute_click ( ref count , ref ResultString , ref objtype , ref obj , out err );
            ///////==================================================================///////

            // see if we have received a specific type (from objtype)
            string resultstring = ResultString;
            // Failures will always return NULL
            if ( err != "" )
            {
                if ( err == "SUCCESS" )
                {
                    Mouse . OverrideCursor = Cursors . Arrow;
                    MessageBox . Show ( err , $"SQL processing of the command execuuted has been reported as successful" );
                }
                else
                {
                    Mouse . OverrideCursor = Cursors . Arrow;
                    MessageBox . Show ( err , "SQL processing error encountered" );
                }
            }
            if ( dynvar == null || objtype == null )
            {
                Mouse . OverrideCursor = Cursors . Arrow;
                return;
            }
            try
            {
                if ( objtype == typeof ( Int32 ) )
                {
                    if ( count != 0 )
                        testint = count;
                    newtype = objtype;
                }
                else if ( objtype == typeof ( string ) )
                {
                    resultstring = ( string ) obj?.ToString ( );
                    newtype = objtype;
                }
                else if ( objtype == typeof ( IEnumerable<dynamic> ) )
                {
                    newtype = objtype;
                }
                else if ( objtype == typeof ( List<string> ) )
                {
                    newtype = objtype;
                }
                else
                {
                    // setup results dialog, but hide it
                    newtype = objtype;
                    if ( err == "" )
                    {
                        err = $"An undetermined error occured during Execution of [ {ListResults . SelectedItem . ToString ( )}.  Check that any required arguments were passed " +
                                $"correctly to the enquiry, and failing that try using a diferent Execution Method ";
                    }
                }
            }
            catch ( Exception ex )
            {
                Console . WriteLine ( $"Error occurred parsing return types from SLQ  Execution request\nError was {ex . Message}" );
                Mouse . OverrideCursor = Cursors . Arrow;
                NewWpfDev . Utils . DoErrorBeep ( );
                return;
            }
            //GengridExecutionResults
            GenResults = new GengridExecutionResults ( this , this . Topmost );
            GenResults . CollectionGridresults . Visibility = Visibility . Collapsed;
            GenResults . CollectionTextresults . Visibility = Visibility . Collapsed;
            GenResults . TextResultsDocument . Visibility = Visibility . Collapsed;
            GenResults . ExecutionInfo . Visibility = Visibility . Visible;
            if ( err != "" )
            {
                GenResults . IsUnknownError = true;
                GenResults . CollectionTextresults . Visibility = Visibility . Visible;
                GenResults . CollectionTextresults . Document =
                    Gengrid . LoadFlowDoc ( GenResults . CollectionTextresults ,
                     FindResource ( "Black3" ) as SolidColorBrush ,
                        $"Execution of [ {ListResults . SelectedItem . ToString ( )} using \n{optype . SelectedItem . ToString ( )}] Encountered an SQL error.\n\n{err}" );
                GenResults . ExecutionInfo . Text = $"Execution of [ {optype . SelectedItem . ToString ( )}] completed , but  with errors !!!!";
                GenResults . CountResult . Text = $"ZERO";
                // Showing Scrollviewer Text Only, so reduce height
                // Squeeze unused row so buttons show in our 220 height
                GenResults . Height = 280;
                GenResults . innerresultscontainer . RowDefinitions [ 1 ] . Height = new GridLength ( 1 , GridUnitType . Pixel );
                GenResults . Show ( );
                GenResults . Topmost = true;
                GenResults . Refresh ( );
                NewWpfDev . Utils . PlayErrorBeep ( );
                Mouse . OverrideCursor = Cursors . Arrow;
                return;
            }

            ProcessSprocExecutionResult ( dynvar , count , ResultString , objtype , obj , err , newtype );
            Mouse . OverrideCursor = Cursors . Arrow;
        }
        public static string [ ] PadArgsArray ( string [ ] content )
        {
            string [ ] tmp = new string [ DEFAULTARGSSIZE ];
            for ( int x = 0 ; x < DEFAULTARGSSIZE ; x++ )
            {
                if ( content . Length - 1 >= x )
                {
                    if ( content [ x ] != null )
                        tmp [ x ] = content [ x ];
                    else
                        tmp [ x ] = "";
                }
                else
                    tmp [ x ] = "";
            }
            return tmp;
        }
        static public bool CheckForArgType ( string type )
        {
            if ( type == "" ) return false;
            if ( type == "STR"
           || type == "INT"
           || type == "FLOAT"
           || type == "VARCHAR"
           || type == "VARBIN"
           || type == "TEXT"
           || type == "BIT"
           || type == "BOOL"
           || type == "SMALLINT"
           || type == "BIGINT"
           || type == "DOUBLE"
           || type == "DEC"
           || type == "CURR"
           || type == "DATETIME"
           || type == "DATE"
           || type == "TIMESTAMP"
           || type . Contains ( "TIME" ) )
                return true;
            else
                return false;
        }

        static public string GetArgSize ( string args )
        {
            int size = 0;
            bool success = true;
            string ch = "";
            string validnumbs = "()0123456789";
            for ( int x = 0 ; x < args . Length ; x++ )
            {
                ch = args . Substring ( x , 1 );
                if ( validnumbs . Contains ( ch ) == false )
                {
                    success = false;
                    break;
                }
                if ( success )
                    return args;
                else
                    return "";
            }
            if ( success )
            {
                if ( args != "" && args != "MAX" && args != "SYSNAME" )
                {
                    //return size of any not text variable type
                    size = Convert . ToInt32 ( args );
                    Size sz = Size . Parse ( size . ToString ( ) );
                    return sz . ToString ( );
                }
                else if ( args == "MAX" || args == "SYSNAME" )
                {
                    if ( SHOWSIZEARG )
                        return "32000";
                    else
                        return "";
                }
                else
                    return args;
            }
            else
                return "";
        }

        public int CheckForParameterArgCount ( string [ ] args )
        {
            int count = 0;
            for ( int x = 0 ; x < args . Length ; x++ )
            {
                if ( args [ x ] != "" ) count++;
            }
            return count; ;
        }

        private dynamic Execute_click ( ref int Count , ref string ResultString , ref Type Objtype , ref object Obj , out string Err )
        {
            // called when executing an SP
            //string operationtype = optype . SelectedItem as string;
            string [ ] args1 = null;
            string [ ] args = new string [ 0 ];
            bool UsingCmd = false;
            bool UsingSproc = false;
            // Initiaize ref variables

            Count = 0;
            string Resultstring = "";
            Objtype = null;
            Err = "";

            Searchtext = SPArguments . Text;
            if ( Searchtext == "Argument(s) required ?" )
            {
                MessageBox . Show ( "You MUST enter at least the name of the S.P to be processed before it can be executed !" , "Execution processing error" );
                return null;
            }
            else if ( Searchtext . Contains ( "Clear Prompt" ) || Searchtext . Contains ( "No parameters are required" ) )
            {
                Searchtext = "";
            }

            // PARSE THE ARGUMENTS ENTERED BY  OUR  USER
            int cnt = 0;
            string [ ] fullargs = new string [ DEFAULTARGSSIZE ];
            string [ ] argparts = new string [ DEFAULTARGSSIZE ];
            string [ ] parts = new string [ DEFAULTARGSSIZE ];
            string [ ] tempargs = new string [ 1 ];
            string [ ] testcontent = new string [ 1 ];
            string valid = "0123456789";
            string validstrings = "STRING NVARCHAR VARCHAR TEXT";
            string validnumerics = "INT DOUBLE FLOAT CURRENCY REAL DATE DATETIME ";
            bool GotCommas = false;
            List<string [ ]> argsbuffer = new List<string [ ]> ( );

            if ( Searchtext . ToUpper ( ) . StartsWith ( "CMD" ) )
            {
                // user entered Select statement (or similar)
                UsingCmd = true;
            }
            else if ( SPArguments . Text . ToUpper ( ) . StartsWith ( "SP" ) )
            {
                UsingSproc = true;
            }

            else if ( Searchtext != "" )
            {
                // splt mutliple args into individual strings
                tempargs = Searchtext . Trim ( ) . ToUpper ( ) . Split ( ':' );
                for ( int x = 0 ; x < tempargs . Length ; x++ )
                {
                    tempargs [ x ] = tempargs [ x ] . TrimStart ( ) . TrimEnd ( );
                }
            }
            argsbuffer . Clear ( );
            //Spllit testcontent [ ] string into seperate parts on either comma or space
            if ( UsingCmd == true )
            {
                argsbuffer . Clear ( );
                string [ ] argsx = new string [ 2 ];
                argsx [ 0 ] = "CMD";
                argsx [ 1 ] = SPArguments . Text . Substring ( 3 ) . Trim ( );
                argsbuffer . Add ( argsx );
                dynamic result = ExecuteArgument ( argsbuffer , ref Count , ref ResultString , ref Obj , ref Objtype , ref Err );
                return result;
            }
            else if ( UsingSproc == true )
            {
                argsbuffer . Clear ( );
                string [ ] sprocs = SPArguments . Text . ToUpper ( ) . Split ( " " );
                if ( sprocs . Length >= 1 )
                {
                    int max = 0;
                    string [ ] argsx = new string [ sprocs . Length ];
                    for ( int x = 0 ; x < sprocs . Length ; x++ )
                    {
                        if ( sprocs [ x ] != "" )
                            max++;
                    }
                    argsx = new string [ max ];
                    int index = 0;
                    for ( int x = 0 ; x < sprocs . Length ; x++ )
                    {
                        if ( sprocs [ x ] != "" )
                            argsx [ index++ ] = sprocs [ x ];
                    }
                    argsbuffer . Add ( argsx );
                }
                dynamic result = ExecuteArgument ( argsbuffer , ref Count , ref ResultString , ref Obj , ref Objtype , ref Err );
                return result;
            }
            else
            {
                for ( int y = 0 ; y < tempargs . Length ; y++ )
                {
                    try
                    {

                        for ( int z = 0 ; z < tempargs . Length ; z++ )
                        {
                            args = new string [ DEFAULTARGSSIZE ];
                            args = PadArgsArray ( args );
                            /* process each set of arguments we have in testcontent[]  and split to its constituent parts (name, value, type, size, direction)
                            based on spaces(or comas) between sections of the argument
                            fill parts  with the processed fields from the current arg string

                            structure used FOR ALL ENTRIES MUST ADHERE TO THESE 5 elements & field offset:-
                           0 - Target object (in first argument only)
                           1 - @arg (SP argument name)
                           2 - data type (Optional)
                           3 - size (if relevant) (Optional)
                           4 - direction (INPUT / OUTPUT/ RETURN)
                            */
                            if ( tempargs [ z ] != null && tempargs [ z ] . ToUpper ( ) . Contains ( "NO ARGUMENTS ARE REQUIRED" ) )
                                break;
                            if ( tempargs [ z ] == "" )
                                continue;
                            if ( tempargs . Length == 0 )
                                break;
                            parts = ProcessNextArgSet ( tempargs [ z ] , z , out Err );
                            // now put whatever args contains into our MAIN set (args[]) in correct position
                            // and finally add them to Argsbuffer list

                            for ( int x = 0 ; x < parts . Length ; x++ )
                            {
                                if ( parts [ x ] == "" )
                                    continue;
                                if ( CheckForArgType ( parts [ x ] ) )
                                {
                                    args [ 2 ] = parts [ x ];
                                    continue;
                                }
                                if ( x != 1 && GetArgSize ( parts [ x ] ) != "" )
                                {
                                    // it IS  a numeric or (xxx) string
                                    args [ 3 ] = parts [ x ];
                                    continue;
                                }
                                if ( x == 0 )
                                {
                                    args [ x ] = parts [ x ];
                                    if ( args [ x ] . StartsWith ( "@" ) == false )
                                    {
                                        args [ x ] = "";
                                    }
                                }
                                if ( x == 1 )
                                {
                                    if ( parts [ x ] != "" && ( parts [ x ] == "INPUT" || parts [ x ] == "OUTPUT" || parts [ x ] == "OUT" || parts [ x ] == "RETURN" ) )
                                        args [ 4 ] = parts [ x ];
                                    else
                                        args [ x ] = parts [ x ];
                                }
                                if ( x == 2 )
                                {
                                    if ( parts [ x ] != "" && ( parts [ x ] == "INPUT" || parts [ x ] == "OUTPUT" || parts [ x ] == "OUT" || parts [ x ] == "RETURN" ) )
                                        args [ 4 ] = parts [ x ];
                                    else
                                        args [ x ] = parts [ x ];
                                }
                                if ( x == 3 )
                                {
                                    if ( parts [ x ] != "" && ( parts [ x ] == "INPUT" || parts [ x ] == "OUTPUT" || parts [ x ] == "OUT" || parts [ x ] == "RETURN" ) )
                                        args [ 4 ] = parts [ x ];
                                    else
                                        args [ x ] = parts [ x ];
                                }

                                if ( args [ 3 ] != null && args [ 3 ] != "" )
                                {
                                    if ( parts [ x ] != "" && ( parts [ x ] == "INPUT" || parts [ x ] == "OUTPUT" || parts [ x ] == "OUT" || parts [ x ] == "RETURN" ) )
                                        args [ 4 ] = parts [ x ];
                                    else if ( args [ 1 ] != parts [ x ] )
                                    {
                                        if ( SHOWSIZEARG )
                                        {
                                            args [ 3 ] = parts [ x ];
                                            args [ 3 ] = ValidateSizeParam ( args [ 3 ] );
                                        }
                                        else
                                            args [ 3 ] = "";
                                    }
                                }
                                if ( x == 4 )
                                {
                                    if ( parts [ x ] != "" && ( parts [ x ] == "INPUT" || parts [ x ] == "OUTPUT" || parts [ x ] == "OUT" || parts [ x ] == "RETURN" ) )
                                        args [ 4 ] = parts [ x ];
                                    else
                                        args [ x ] = parts [ x ];
                                }
                            }
                            // Finally Add this set of now validated args to our outgoing arguments list to pass to the SQL Query
                            if ( CheckForParameterArgCount ( args ) > 0 )
                                argsbuffer . Add ( args );
                            PrintSPArgs ( args );
                        }
                    }
                    catch ( Exception ex )
                    {
                        Console . WriteLine ( $"Parsing error : {ex . Message}, {ex . Data}" );
                        NewWpfDev . Utils . DoErrorBeep ( );
                        return null;
                    }
                }
                dynamic result = ExecuteArgument ( argsbuffer , ref Count , ref ResultString , ref Obj , ref Objtype , ref Err );
                return result;
            }
        }

        public dynamic ExecuteArgument ( List<string [ ]> argsbuffer , ref int Count , ref string ResultString , ref object Obj , ref Type Objtype , ref string Err )
        {
            //*************************************************//
            // WE have now
            // finished parsing the FIRST argument
            // WE have @argname, data type, size as integer,
            // and direction as INPUT, OUTPUT or RETURN
            //*************************************************//
            int innercount = Count;
            string innerresultstring = ResultString;
            object innerobj = Obj;
            string innerrerr = Err;
            string operationtype = optype . SelectedItem as string;
            string SqlCommand = ListResults . SelectedItem . ToString ( );
            string [ ] sprocCmd = null;
            bool IsCmd = false;
            bool IsSproc = false;
            int indx = 0;
            foreach ( string [ ] item in argsbuffer )
            {
                if ( item [ 0 ] . Contains ( "CMD" ) )
                {
                    IsCmd = true;
                    SqlCommand = item [ 1 ];
                    break;
                }
                else if ( item [ 0 ] . ToUpper ( ) . StartsWith ( "SP" ) )
                {
                    IsSproc = true;
                    sprocCmd = new string [ item . Length ];
                    sprocCmd = item;
                    break;
                }
            }
            if ( IsSproc )
            {
                argsbuffer = new List<string [ ]> ( );
                string [ ] args = new string [ sprocCmd . Length - 1 ];
                for ( int x = 0 ; x < sprocCmd . Length ; x++ )
                {
                    if ( x == 0 )
                        SqlCommand = sprocCmd [ 0 ];
                    else if ( sprocCmd [ x ] != "" )
                        args [ x - 1 ] = sprocCmd [ x ];
                }
                argsbuffer . Add ( args );
            }
            if ( operationtype == null )
            {
                MessageBox . Show ( "You MUST select an Execution Method before the selected S.P can be executed !" , "Execution processing error" );
                return null;
            }

            try
            {
                string output = "";
                // Now find out what method we are going to use
                if ( operationtype == "SP Execute command or returning an INT value" )
                {
                    //METHOD 6
                    if ( IsCmd == false && IsSproc == false && SqlCommand == "" )
                        SqlCommand = $"{ListResults . SelectedItem . ToString ( )}";

                    //// tell method what we are expecting back
                    Objtype = typeof ( int );

                    //********************************************************************************//
                    dynamic intresult = GenDapperQueries . Get_DynamicValue_ViaDapper ( SqlCommand ,
                        argsbuffer ,
                        ref innerresultstring ,
                        ref innerobj ,
                        ref Objtype ,
                        ref innercount ,
                        ref Err ,
                        6 );
                    //********************************************************************************//

                    if ( intresult != null )
                    {
                        // TODO Maybe wrong  8/11/2022
                        ResultString = innerresultstring;
                        Obj = ( object ) intresult;
                        Objtype = typeof ( Int32 );
                        Count = innercount;
                        return ( dynamic ) innerobj;
                    }
                    if ( Err != "" && innerresultstring == "" )
                    {
                        //if ( ReturnProcedureHeader ( SqlCommand , ListResults . SelectedItem . ToString ( ) ) == "DONE" )
                        //    return ( dynamic ) null;
                        //ShowError ( operationtype , Err );
                        return ( dynamic ) null;
                    }
                }
                else if ( operationtype == "SP returning a String" )
                {
                    //METHOD 2

                    //Use storedprocedure  version
                    if ( IsCmd == false && IsSproc == false && SqlCommand == "" )
                        SqlCommand = $"{ListResults . SelectedItem . ToString ( )}";

                    // tell method what we are expecting back
                    Objtype = typeof ( string );

                    //********************************************************************************//
                    dynamic stringresult = GenDapperQueries . Get_DynamicValue_ViaDapper (
                        SqlCommand ,
                        argsbuffer ,
                        ref innerresultstring ,
                        ref innerobj ,
                        ref Objtype ,
                        ref innercount ,
                        ref Err ,
                        2 );
                    //********************************************************************************//
                    // Working 8/11/2022
                    if ( Err != "" )
                    {
                        if ( ReturnProcedureHeader ( SqlCommand , ListResults . SelectedItem . ToString ( ) ) == "DONE" )
                            return ( dynamic ) null;
                        ShowError ( operationtype , Err );
                    }
                    ResultString = innerresultstring;
                    Obj = ( object ) stringresult;
                    Objtype = typeof ( string );
                    Count = innercount;

                    if ( Objtype == typeof ( string ) )
                        return stringresult;
                    else
                        return stringresult . ToString ( );
                }
                else if ( operationtype == "SP returning a List<string>" )
                {
                    //METHOD 5
                    if ( IsCmd == false && IsSproc == false && SqlCommand == "" )
                        SqlCommand = $"{ListResults . SelectedItem . ToString ( )}";

                    // tell method what we are expecting back
                    Objtype = typeof ( List<string> );

                    //********************************************************************************//
                    dynamic stringlist = GenDapperQueries . Get_DynamicValue_ViaDapper (
                        SqlCommand ,
                        argsbuffer ,
                        ref innerresultstring ,
                        ref innerobj ,
                        ref Objtype ,
                        ref innercount ,
                        ref Err ,
                        5 );

                    ResultString = innerresultstring;
                    Obj = ( object ) stringlist;
                    Objtype = typeof ( List<string> );

                    if ( Objtype == typeof ( List<string> ) )
                        return ( dynamic ) stringlist;
                    else
                        return ( dynamic ) null;

                }
                else if ( operationtype == "SP returning a Table as ObservableCollection" )
                {
                    //METHOD 0
                    if ( IsCmd == false && IsSproc == false && SqlCommand == "" )
                        SqlCommand = $"{ListResults . SelectedItem . ToString ( )}";
                    DatagridControl dgc = new ( );

                    // tell method what we are expecting back
                    Objtype = typeof ( ObservableCollection<GenericClass> );

                    //********************************************************************************//
                    // Should normally  be  '[spLoadTableAsGeneric]' but can be any SP that wants a collection back
                    IEnumerable<dynamic> tableresult = GenDapperQueries . Get_DynamicValue_ViaDapper (
                     SqlCommand ,
                    argsbuffer ,
                     ref innerresultstring ,
                     ref innerobj ,
                     ref Objtype ,
                     ref innercount ,
                     ref Err ,
                     0 );

                    ResultString = innerresultstring;
                    Obj = ( object ) tableresult;
                    Objtype = typeof ( IEnumerable<dynamic> );

                    if ( Objtype == typeof ( IEnumerable<dynamic> ) )
                        return ( dynamic ) tableresult;
                    else
                        return ( dynamic ) null;


                    //********************************************************************************//

                    if ( Err != "" )
                    {
                        if ( ReturnProcedureHeader ( ListResults . SelectedItem . ToString ( ) , ListResults . SelectedItem . ToString ( ) ) == "DONE" )
                            return ( dynamic ) null;
                        ShowError ( operationtype , Err );
                    }
                    else return tableresult;
                }
                ////---------------------------------------------------------------------------------------------------//
                else if ( operationtype == $"Execute SQL (text) command with No return value" )
                //---------------------------------------------------------------------------------------------------//
                {
                    string error = "";
                    //string [ ] argsbuff = new string [ 0 ];
                    if ( IsCmd == false && IsSproc == false && SqlCommand == "" )
                        SqlCommand = SPArguments . Text . Trim ( );
                    //********************************************************************************//
                    // call Execute procedure for TEXT command (0)
                    GenDapperQueries . ExecuteSqlCommandWithNoReturnValue ( 0 , SqlCommand , argsbuffer , out error );
                    if ( error != "" && error != "SUCCESS" && error . Contains ( "has completed successfully" ) == false )
                        MessageBox . Show ( $"The command {SqlCommand} failed wiith  the following \nerror message\n[{error}]" , "SQL Execution error" );
                    else
                    {
                        if ( error == "SUCCESS" )
                            error = $"The command just executed [{SqlCommand . ToUpper ( )}] has been completed successfuly.";
                    }
                    Err = error;
                }
                ////---------------------------------------------------------------------------------------------------//
                else if ( operationtype == $"Execute (S.Proc) command with No return value" )
                //---------------------------------------------------------------------------------------------------//
                {
                    string error = "";
                    //string [ ] argsbuff = string [ 1 ];
                    //int [ ] args = new int [ 2 ];
                    //args [ 0] = 
                    //********************************************************************************//
                    // call Execute procedure for TEXT command (0)
                    GenDapperQueries . ExecuteSqlCommandWithNoReturnValue ( 1 , SqlCommand , argsbuffer , out error );
                    if ( error != "" )
                        MessageBox . Show ( $"The command {SqlCommand} failed wiith  the following error message\n[{error}]" , "SQL Execution error" );
                    else
                        error = "SUCCESS";
                    Err = error;
                }
                //else if ( rescollection . Count == 0 && Err == "" )
                //{
                //    DatagridControl dg = new DatagridControl ( );

                //    //********************************************************************************//
                //    //                        var result = dg . GetDataFromStoredProcedure ( "Select columncount from countreturnvalue" , null , "" , out Err , out recordcount , 1 );
                //    //********************************************************************************//

                //    //                     if ( result . Count == 0 )
                //    MessageBox . Show ( $"No Error was encountered,  but the request did NOT return any type of value...\n\nPerhaps the processing method that you selected as shown below :-\n" +
                //        $"[{optype . SelectedItem . ToString ( ) . ToUpper ( )}]\n was not the correct processing method type for this Stored.Procedure ?" , "SQL Error" );
                //    //if ( ReturnProcedureHeader ( "Select columncount from countreturnvalue" , "" ) == "DONE" )

                //    return ( dynamic ) null;
                //}
                else
                {
                    string errmsg = $"SQL Error encountered : The error message was \n{Err}\n\nPerhaps a  different Execution method would work more effectively for this Stored.Procedure.?";
                    Err = errmsg;
                    return ( dynamic ) null;
                }
                //}
                //else if ( operationtype == $"Execute SQL (text) command with No return value" )
                //{
                //    DatagridControl dgc = new ( );

                //    //********************************************************************************//
                //    var result = dgc . ExecuteDapperTextCommand ( ListResults . SelectedItem . ToString ( ) , args , out Err );
                //    //********************************************************************************//

                //    if ( Err != "" )
                //        ShowError ( operationtype , Err );
                //    else
                //    {
                //    }
                //}
                //else
                //{
                //    MessageBox . Show ( "You MUST select one of these options to proceed....." , "Selection Error" );
                //    return -9;
                //}
            }
            catch ( Exception ex )
            {
                Utils . DoErrorBeep ( );
                Debug . WriteLine ( $"Execute_Click ERROR : \n{ex . Message}\n{ex . Data}" );
            }
            return ( dynamic ) null;
        }

        public void PrintSPArgs ( string [ ] args )
        {
            Debug . WriteLine ( message: "\n" );
            for ( int x = 0 ; x < args . Length ; x++ )
            {
                Debug . WriteLine ( $"({x})  [{args [ x ]}]" );
            }
        }

        static public string [ ] ProcessNextArgSet ( string argstring , int index , out string Error )
        {
            Error = "";

            string [ ] argset = new string [ DEFAULTARGSSIZE ];
            string [ ] tmp = new string [ DEFAULTARGSSIZE ];
            for ( int x = 0 ; x < DEFAULTARGSSIZE ; x++ )
            {
                argset [ x ] = "";
            }
            if ( argstring == null )
                return argset;

            if ( argstring . Trim ( ) != "" )
            {
                argset = argstring . Split ( ' ' );
                argset = CleanArgumentblanks ( argset );
                //if ( argset . Length == 1 )
                //    return argset;
                //if ( argset . Length < 5 )
                //    argset = argstring . Split ( ' ' );
                if ( argset . Length < 2 )
                {
                    Error = $"Invalid  arguments set ({argset . Length}) received. There must be an @argument name + it's value";
                    return new string [ 0 ];
                }
                int blanks = 0;

                if ( index > 0 ) tmp [ 0 ] = "";
                int insertindx = 0;
                bool isnumeric = true;
                string validnumerics = "0123456789";

                if ( argset . Length >= 1 && argset [ 0 ] != "" )
                {
                    tmp [ 0 ] = argset [ 0 ];
                }

                if ( argset . Length >= 2 && argset [ 1 ] != "" )
                {
                    // arg name 
                    tmp [ 1 ] = argset [ 1 ];
                }

                if ( argset . Length >= 3 && argset [ 2 ] != "" )
                {
                    if ( argset [ 2 ] != "" )
                    {
                        // initial argument name
                        tmp [ 2 ] = argset [ 2 ];
                    }
                    else return null;
                }
                if ( argset . Length >= 4 && argset [ 3 ] != "" )
                {
                    // 3rd parameter value  (size)
                    if ( SHOWSIZEARG )
                    {
                        argset [ 3 ] = NewWpfDev . Utils . SpanTrim ( argset [ 3 ] , 1 , argset [ 3 ] . Length - 2 ) . ToString ( );
                        for ( int y = 0 ; y < argset [ 3 ] . Length ; y++ )
                        {
                            char ch = argset [ 3 ] [ y ];
                            char char2 = ' ';
                            for ( int t = 0 ; t < validnumerics . Length ; t++ )
                            {
                                char2 = ( char ) validnumerics [ t ];
                                if ( ch == char2 )
                                {
                                    isnumeric = false;
                                    break;
                                }
                                else
                                    isnumeric = false;
                                break;
                            }
                        }
                        if ( isnumeric == false && argset [ 3 ] . Contains ( "MAX" ) || argset [ 3 ] . Contains ( "SYSNAME" ) )
                        {
                            if ( SHOWSIZEARG )
                                tmp [ 3 ] = "32000";
                            else
                                tmp [ 3 ] = "";
                        }
                        else if ( isnumeric == true )
                        {
                            if ( SHOWSIZEARG )
                                tmp [ 3 ] = Convert . ToInt32 ( argset [ 3 ] ) . ToString ( );
                            else tmp [ 3 ] = "";
                        }
                    }
                    else
                        tmp [ 3 ] = "";
                }
                if ( argset . Length >= 4 && argset [ 3 ] != "" )
                {
                    // 2nd parameter value  (size)
                    tmp [ 3 ] = argset [ 3 ];
                }
                if ( argset . Length >= 5 && argset [ 4 ] != "" )
                {
                    // 1st parameter value  (Target/Argument)
                    try
                    {
                        if ( isnumeric )
                        {
                            if ( SHOWSIZEARG )
                                tmp [ ( insertindx ) - blanks ] = Convert . ToInt32 ( argset [ 3 ] ) . ToString ( );
                            else
                                tmp [ ( insertindx ) - blanks ] = "";
                        }
                        else
                        {
                            if ( argset [ 4 ] == "INPUT" || argset [ 4 ] == "OUTPUT" || argset [ 4 ] == "RETURN" )
                                tmp [ 4 ] = argset [ 4 ];
                            else
                            {
                                if ( argset [ 4 ] . Contains ( "INTEGER" ) )
                                    argset [ 4 ] = "INT";
                                //if ( argset [ z ] . Contains ( "STR" ) )
                                //    argset [ z ] = "STRING";

                                tmp [ ( insertindx ) - blanks ] = argset [ 4 ];
                            }
                        }
                        int val = Convert . ToInt32 ( argset [ 4 ] );
                    }
                    catch ( Exception ex )
                    {
                        NewWpfDev . Utils . DoErrorBeep ( );
                        isnumeric = false;
                    }
                }
            }
            if ( tmp [ 2 ] == "OUT" || tmp [ 2 ] == "OUTPUT" )
                tmp [ 3 ] = "OUTPUT";
            else if ( tmp [ 3 ] == null || tmp [ 3 ] == "" )
                tmp [ 3 ] = "INPUT";
            for ( int v = 0 ; v < tmp . Length ; v++ )
            {
                // ensure we have NO NULLS in structure
                if ( tmp [ v ] == null )
                    tmp [ v ] = "";
            }
            return tmp;
        }

        static public string [ ] CleanArgumentblanks ( string [ ] argset )
        {
            string [ ] data
        ; int rowcount = 0;
            for ( int x = 0 ; x < argset . Length ; x++ )
            {
                if ( argset [ x ] != "" )
                    rowcount++;
            }
            data = new string [ rowcount ];
            int indx = 0;
            for ( int x = 0 ; x < argset . Length ; x++ )
            {
                if ( argset [ x ] != "" )
                {
                    data [ indx ] = argset [ x ];
                    indx++;
                }
            }
            return data;
        }
   
        public void createoptypes ( )
        {
            optype . Items . Add ( $"SP Execute command or returning an INT value" );
            optype . Items . Add ( $"SP returning a String" );
            optype . Items . Add ( $"SP returning a List<string>" );
            optype . Items . Add ( $"SP returning a Table as ObservableCollection" );
            optype . Items . Add ( $"SP returning a 'Pot Luck' result" );
            optype . Items . Add ( $"Execute SQL (text) command with No return value" );
            optype . Items . Add ( $"Execute (S.Proc) command with No return value" );
            optype . SelectedIndex = 4;
            ExName . Text = optype . SelectedItem . ToString ( );

        }

        private string ReturnProcedureHeader ( string commandline , string Arguments )
        {
            //*********************************//
            // only called  by Resultsviewer
            //*********************************//
            Parameters . Text = SProcsDataHandling . GetSpHeaderBlock ( Arguments , spviewer );
            if ( Parameters . Text == "" )
                return "";
            //DetailInfo . Visibility = Visibility . Visible;
            operationtype3 . Text = $"Stored Procedure {commandline . ToUpper ( )} Header Details :-\n\n{Parameters . Text}";
            return "Done";
        }

        //private void Hidepanel_Click ( object sender , RoutedEventArgs e )
        //{
        //    OperationSelection . Visibility = Visibility . Collapsed;
        //}
        private void ShowError ( string optype , string err )
        {
            //if ( err != "" )
            //    MessageBox . Show ( $"Error encountered .....error message was \n{err . ToUpper ( )}\n\nPerhaps the method that you selected as shown below :-\n" +
            //        $"[{optype . ToUpper ( )}]\n was not the correct processing method type for this Stored.Procedure.\n\n" +
            //        $"The help window just opened shows you the parameter types required by this S.P?" , "SQL Error" );
            //else
            //    MessageBox . Show ( $"No Error was encountered,  but the request did NOT return any type of value...\n\n" +
            //        $"Perhaps the processing method that you selected as shown below :-\n[{optype . ToUpper ( )}]\n" +
            //        $"The help window just opened shows you the parameter types required by this S.P?" , "SQL Error" );

        }

        private void SPArguments_MouseEnter ( object sender , MouseEventArgs e )
        {
            SPArguments . SelectAll ( );
        }

        private void SPArguments_MouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
        {
            TextBox tb = sender as TextBox;
            if ( SPArguments . Text == "Argument(s) required ?" )
                SPArguments . Text = "";
            SPArgumentsFull . Text = SPArguments . Text;
        }

        private void SPArguments_GotFocus ( object sender , RoutedEventArgs e )
        {
            if ( SPArguments . Text == "Argument(s) required ?" )
                SPArguments . Text = "";
            SPArgumentsFull . Text = SPArguments . Text;
        }

        private void TextResult_PreviewKeyDown ( object sender , KeyEventArgs e )
        {

        }

        private void Closepanel_Click ( object sender , RoutedEventArgs e )
        {
            DetailInfo . Visibility = Visibility . Collapsed;
        }

        private void optype_MouseDoubleClick ( object sender , MouseButtonEventArgs e )
        {
            if ( SPArguments . Text != SPArgumentsFull . Text )
            {
                if ( SPArguments . Text . Contains ( "STRING" ) == true && SPArgumentsFull . Text . Contains ( "STRING" ) == false )
                    SPArguments = SPArgumentsFull;
                else if ( SPArguments . Text . Contains ( "STRING" ) == false && SPArgumentsFull . Text . Contains ( "STRING" ) == true )
                    SPArgumentsFull = SPArguments;
            }
            ExName . Text = optype . SelectedItem . ToString ( );
            DoExecute_Click ( null , null );
        }

        private void Exec_Click ( object sender , RoutedEventArgs e )
        {
            Gengrid . RunExecute_Click ( this );
            //ProcessExecResults ( );
        }

        //public void ProcessExecResults ( )
        //{
        //    if ( ExecResults . resultInt != 0 )
        //    { }
        //    if ( ExecResults . resultString != "" )
        //    { }
        //    if ( ExecResults . resultDouble != 0.0 )
        //    { }
        //    if ( ExecResults . resultCollection != null )
        //    { }
        //    if ( ExecResults . resultStringList . Count > 0 )
        //    { }

        //}

        private void ListResults_MouseRightButtonDown ( object sender , MouseButtonEventArgs e )
        {
            if ( e . Handled ) return;
            ContextMenu cm = FindResource ( "ResultsViewerContextMenu" ) as ContextMenu;
            Point pt = e . GetPosition ( sender as UIElement );
            // Hide relevant entries
            List<string> hideitems = new List<string> ( );
             //hideitems . Add ( "gm3" );
            //hideitems . Add ( "gm4" );
            // Hide close tables viewer as it is not open
            hideitems . Add ( "gm5" );
   
            ContextMenu menu = RemoveMenuItems ( "ResultsViewerContextMenu" , "" , hideitems );
            //forces menu to show immeduiately to right and below mouse pointer
            menu . PlacementTarget = sender as FrameworkElement;
            menu . PlacementRectangle = new Rect ( pt . X , pt . Y , 250 , 10 );
            menu . IsOpen = true;
            e . Handled = true;
        }

        private void Spresultsviewer_Closing ( object sender , System . ComponentModel . CancelEventArgs e )
        {
            Genericgrid . Resultsviewer = null;
            if ( IsDirty )
            {
                NewWpfDev . Properties . Settings . Default . SpResultLbFontSize = ListResults . FontSize . ToString ( );
                NewWpfDev . Properties . Settings . Default . SpResultScrollviewerFontSize = ScrollViewerFontSize . ToString ( );
                NewWpfDev . Properties . Settings . Default . Save ( );
            }
        }

        /// <summary>
        /// Stub to allow Genericgrid to call ShowAllSps (Private Click event handler)
        /// </summary>
        /// <param name="win"></param>
        public void LoadAllSps ( Window win , int currindex )
        {
            // stub to load main private method
            Gengrid . RunExecute_Click ( this );
        }

        /// <summary>
        /// Load ALL stored procedures into viewer wudow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowAllSps ( object sender , RoutedEventArgs e )
        {
            CheckBox cb = sender as CheckBox;
            List<string> SpList = new List<string> ( );
            string srchterm = "";
            srchterm = Searchterm;
            string curritem = "";
            int currindex = 0;
            if ( ListResults . SelectedIndex != -1 )
                currindex = ListResults . SelectedIndex;
            if ( ListResults . SelectedItem != null )
                curritem = ListResults . SelectedItem . ToString ( );
            else
                curritem = ListResults . Items [ 0 ] . ToString ( );
            SpResultsViewer Target = sender as SpResultsViewer;

              Mouse . OverrideCursor = Cursors . Arrow;
        }
        private void hSplitter_MouseEnter ( object sender , MouseEventArgs e )
        {
            GridSplitter gs = sender as GridSplitter;
            gs . Cursor = Cursors . ScrollNS;
            Mouse . OverrideCursor = Cursors . SizeNS;
        }

        private void hSplitter_MouseLeave ( object sender , MouseEventArgs e )
        {
            GridSplitter gs = sender as GridSplitter;
            gs . Cursor = Cursors . Arrow;
            Mouse . OverrideCursor = Cursors . Arrow;
        }

        private void Hsplitter_MouseMove ( object sender , MouseEventArgs e )
        {
            GridSplitter gs = sender as GridSplitter;
            gs . Cursor = Cursors . ScrollNS;
            Mouse . OverrideCursor = Cursors . SizeNS;
        }

        private void Spresultsviewer_FocusableChanged ( object sender , DependencyPropertyChangedEventArgs e )
        {
            Debug . WriteLine ( $"Ontop status = {this . Topmost}" );
        }

        private void Spresultsviewer_KeyDown ( object sender , KeyEventArgs e )
        {
            if ( e . Key == Key . F1 )
            {
                //if( ShowArgumentsHelp .IsHitTestVisibleProperty )
                ShowArgumentsHelp sh = new ShowArgumentsHelp ( @"C:\users\ianch\documents\spresultsviewer arguments layout.txt" , this );
                sh . header . Text = "IMPORTANT HELP : Entering Stored Procedure Arguments";
                sh . Show ( );
                sh . ArgInfo . UpdateLayout ( );
                sh . Topmost = true;
                e . Handled = true;
            }
            else if ( e . Key == Key . F2 )
            {
                GengridExecutionResults sh = new GengridExecutionResults ( this , false );
                sh . Show ( );
                e . Handled = true;
            }
            else if ( e . Key == Key . F3 )
            {
                SpArguments sh = new SpArguments ( SPArguments );
                sh . SPHeaderblock . Text = SProcsDataHandling . GetSpHeaderBlock ( Gengrid . SpTextBuffer , spviewer );

                if ( sh . SPHeaderblock . Text != "" )
                    sh . Show ( );
                e . Handled = true;
            }
        }

        private void ListResults_MouseDoubleClick ( object sender , MouseButtonEventArgs e )
        {
            SpArguments sh = new SpArguments ( SPArguments );
            sh . SPHeaderblock . Text = SProcsDataHandling . GetSpHeaderBlock ( Gengrid . SpTextBuffer , spviewer );
            if ( sh . SPHeaderblock . Text != "" )
                sh . Show ( );
            e . Handled = true;

        }

        #region SqlTablesViewer
        private void LoadViewerTables ( object sender , RoutedEventArgs e )
        {
            //List<string> data = new List<string> ( );
            //           data = Gengrid . Domethod ( );
            List<string> TablesList = Gengrid . GetDbTablesList ( MainWindow . CurrentSqlTableDomain );
            AllTables . ItemsSource = TablesList;
            //            AllTables . ItemsSource = data;
            SqlTablesViewer . Visibility = Visibility . Visible;
            reccount . Text = AllTables . Items . Count . ToString ( );
        }

        private void tableviewer_LButtonDn ( object sender , MouseButtonEventArgs e )
        {
            ListBox senderlb;
            Grid grid;
            string Sendername = "";
            Type type = sender . GetType ( );
            if ( type == typeof ( ListBox ) )
            {
                senderlb = sender as ListBox;
                Sendername = senderlb . Name;
            }
            else
            {
                grid = sender as Grid;
                Sendername = grid . Name;
            }
            ActiveDragControl = SqlTablesViewer;
            this . MovingObject = SqlTablesViewer;
            DragCtrl . InitializeMovement ( ( FrameworkElement ) SqlTablesViewer );
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
                    ActiveDragControl = SqlTablesViewer;
                    DragCtrl . Ismoving = true;
                    DragCtrl . CtrlMoving ( sender , e );
                    e . Handled = false;
                }
            }
        }

        private void tableviewer_Ending ( object sender , MouseButtonEventArgs e )
        {
            ActiveDragControl = SqlTablesViewer;
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
                SqlTablesViewer . Visibility = Visibility . Collapsed;
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
        #endregion SqlTablesViewer

        private void closeviewer_Click ( object sender , RoutedEventArgs e )
        {
            SqlTablesViewer . Visibility = Visibility . Collapsed;
        }

        private void closeviewer ( object sender , RoutedEventArgs e )
        {
            SqlTablesViewer . Visibility = Visibility . Collapsed;
        }
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
                    //if ( Splist . SelectedItem != null && Splist . SelectedItem . ToString ( ) != "" )
                    //    SPExecuteText = $"Show the S.P Execute Window with [ {Splist . SelectedItem . ToString ( )} ]";
                    //else
                    //{
                    //    Splist . SelectedIndex = 0;
                    //    SPExecuteText = $"Show the S.P Execute Window with [ {Splist . SelectedItem . ToString ( )} ]";
                    //}

                    //mi . Header = SPExecuteText;
                    //mi . Height = 25;
                    //mi . Tag = Splist . SelectedItem . ToString ( );
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
                // show menu item(s)
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
                // ??
                //foreach ( string delitem in delItems )
                //{
                //    foreach ( MenuItem menuitem in menu . Items )
                //    {
                //        //var v = mi . Items;
                //        if ( menuitem . Name == delitem )
                //            menuitem . Visibility = Visibility . Visible;
                //    }
                //}
            }
            return menu;
        }

        public ContextMenu UpdateMenuItem ( string menuname , string menuitemname )
        {
            var menu = FindResource ( menuname ) as ContextMenu;

            if ( menuitemname != "" )
            {
                // show menu item(s)
                foreach ( MenuItem item in menu . Items )
                {
                    if ( item . Name == menuitemname )
                    {
                        item . UpdateLayout ( );
                        break;
                    }
                }
            }
            return menu;
        }

        private void ContextMenu_Closed ( object sender , RoutedEventArgs e )
        {
            //Restore Popup to full contents
            ContextMenu cm = FindResource ( "ResultsViewerContextMenu" ) as ContextMenu;
            List<string> hideitems = new List<string> ( );
            hideitems . Add ( "gm1" );
            hideitems . Add ( "gm2" );
            hideitems . Add ( "gm3" );
            hideitems . Add ( "gm4" );
            hideitems . Add ( "gm5" );
            hideitems . Add ( "gm6" );
            hideitems . Add ( "gm7" );
            //hideitems . Add ( "gm8" );
            ContextMenu menu = ResetMenuItems ( "ResultsViewerContextMenu" , "" , hideitems );
            menu . IsOpen = false;
            e . Handled = true;
        }

        private void ContextMenu_Opened ( object sender , RoutedEventArgs e )
        {
            // cannot identify caller !!!!
            object [ ] objectarray;
            //var ret = Utils.GetChildControls ( (UIElement) sender , "Button");
            //e . Handled = true;
        }

        private void CloseTableviewer ( object sender , RoutedEventArgs e )
        {
            SqlTablesViewer . Visibility = Visibility . Collapsed;

        }

        private void showmatchesonly ( object sender , RoutedEventArgs e )
        {
            // Careful = calls from Context Menu will NOT find a window, so check it here
            Mouse . OverrideCursor = Cursors . Wait;
            ListBox lbox = null;
            string callertype = "";
            Window dgobj = null;
            dgobj = DapperGenericsLib . Utils . FindVisualParent<Window> ( e . OriginalSource as DependencyObject );
            if ( dgobj == null )
            {
                ContextMenu cmenu = null;
                cmenu = DapperGenericsLib . Utils . FindVisualParent<ContextMenu> ( e . OriginalSource as DependencyObject );
                if ( cmenu . GetType ( ) == typeof ( ContextMenu ) )
                    callertype = "CTXMENU";
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
            if ( callertype == "CTXMENU" || dgobj == null )
            {
                lbox = ListResults;
                // must be Generigrid if dgobk == null
                if ( ListResults . Items . Count > 0 )
                {
                    if ( ListResults . SelectedItem == null )
                        currItem = ListResults . Items [ 0 ] . ToString ( );
                    else
                        currItem = ListResults . SelectedItem . ToString ( );
                }
                else
                    return;
            }
            if ( Gengrid . Searchtext != "" )
            {
                string srchtext = Gengrid . Searchtext;
                // call sql to get SP list and load it into listbox
                List<string> list = null;
                if ( callertype == "CTXMENU" )
                    list = Gengrid . LoadMatchingStoredProcs ( ListResults , srchtext );
                //else
                //    list = Gengrid . LoadMatchingStoredProcs ( GenGrid . Splist , srchterm );
                if ( currItem != null && currItem != "" )
                    lbox . SelectedItem = currItem;

                // Update the resultsviewer as well (if open)
                lbox . ItemsSource = null;
                lbox . Items . Clear ( );
                lbox . ItemsSource = list;
                {
                    lbox . SelectedItem = currItem;
                    lbox . ScrollIntoView ( currItem );
                }
                ListResults . Refresh ( );
                Bannerline . Text = $"Stored Procedures Helper ({list . Count}) SP's Matching  [{srchtext}] for Db [{MainWindow . CurrentSqlTableDomain}] are shown)";
                // set generic flag to show we are no not showing ALL sp's
                ShowingAllSPs = false;
                Mouse . OverrideCursor = Cursors . Arrow;

            }
            Mouse . OverrideCursor = Cursors . Arrow;
        }

        private void ShowArgDetailsonly ( object sender , RoutedEventArgs e )
        {
            //bool sucess = false;
            string Output = "";
            string buffer = SProcsDataHandling . GetSpHeaderBlock ( Gengrid . SpTextBuffer , spviewer );
            AllArgs . Items . Clear ( );
            string [ ] items;
            string [ ] lines = buffer . Split ( '\n' );
            foreach ( var line in lines )
            {
                if ( line . Contains ( "CREATE PROC" ) == false )
                    Output += SProcsDataHandling . GetBareSProcHeader ( line , ListResults . SelectedItem . ToString ( ) , out bool success );
            }
            if ( Output . Length < 5 )
            {
                AllArgs . Items . Add ( "This Procedure does NOT appear to require any Arguments.,\nbut it may possibly contain a Syntax Error in the arguments section\nsuch as a defult value that only has [ \" ] a single quote mark\ninstead of the correct [ \" ..\" ] quote marks ?" );
                DetailedArgsViewer . Visibility = Visibility . Visible;
            }
            else
            {
                items = Output . Split ( '\n' );
                DetailedArgsViewer . Visibility = Visibility . Visible;
                foreach ( string item in items )
                {
                    if ( item . Length > 2 )
                        AllArgs . Items . Add ( item );
                }
            }
            AllArgs . SelectedIndex = 0;
            AllArgs . SelectedItem = 0;
            AllArgs . Focus ( );
            e . Handled = true;
        }

        private void ShowAllSprocs ( object sender , RoutedEventArgs e )
        {
            string currItem = "";
            if ( this . ListResults . Items . Count > 0 )
            {
                if ( this . ListResults . SelectedItem == null )
                    currItem = this . ListResults . Items [ 0 ] . ToString ( );
                else
                    currItem = this . ListResults . SelectedItem . ToString ( );
            }
            List<string> list = new List<string> ( );
            list = GenDapperQueries . CallStoredProcedure ( list , "spGetStoredProcs" );
            ListResults . ItemsSource = null;
            ListResults . Items . Clear ( );
            ListResults . ItemsSource = list;
            ListResults . SelectedItem = currItem;
            ListResults . ScrollIntoView ( currItem );
            Mouse . OverrideCursor = Cursors . Arrow;
            //SetStatusbarText ( $"All ({Splist . Items . Count}) S.P's loaded successfully..." );
            Bannerline . Text = $"All {ListResults . Items . Count} existing S.Procs for Db [{MainWindow . CurrentSqlTableDomain}] are displayed";
            //            reccount . Text = ListResults . Items . Count . ToString ( );
            ShowingAllSPs = true;
            //SpInfo2 . Text = $"{Splist . Items . Count} available...";
            //InfoHeaderPanel . Text = $"All ({Splist . Items . Count}) Stored Procedures are listed";
            //CurrentSpList = "ALL";

            // Context menu options - all correct  
            //LoadAllItems . IsEnabled = false;
            //LoadMatchingItems . IsEnabled = true;

            // Force list to show()
            //GridLength gl = new GridLength ( );
            //gl = ViewerGrid . ColumnDefinitions [ 0 ] . Width;
            //if ( gl . Value < 5 )
            //    ViewerGrid . ColumnDefinitions [ 0 ] . Width = new GridLength ( 200 , GridUnitType . Pixel );
        }
        private void TablesViewer_PreviewMouseRightButtonDown ( object sender , MouseButtonEventArgs e )
        {
            //control for ContextMenu = "{StaticResource TableViewerContextMenu}"
            ContextMenu cm = FindResource ( "TableViewerContextMenu" ) as ContextMenu;
            //forces menu to show immeduiately to right and below mouse pointer
            cm . PlacementTarget = sender as FrameworkElement;
            Point pt = e . GetPosition ( sender as UIElement );
            cm . PlacementRectangle = new Rect ( pt . X + 70 , pt . Y + 10 , 12 , 40 );
            cm . IsOpen = true;
            e . Handled = true;
        }
        private void closeargsviewer_Click ( object sender , RoutedEventArgs e )
        {
            DetailedArgsViewer . Visibility = Visibility . Collapsed;
        }

        private void DetailedArgsViewer_KeyDown ( object sender , KeyEventArgs e )
        {
            if ( e . Key == Key . Escape )
                DetailedArgsViewer . Visibility = Visibility . Collapsed;
        }
        private void detailsviewer_KeyDown ( object sender , KeyEventArgs e )
        {
            if ( e . Key == Key . Escape )
            {
                DetailedArgsViewer . Visibility = Visibility . Collapsed;
                e . Handled = true;
            }
        }
        public string ValidateSizeParam ( string sizearg )
        {
            if ( sizearg . Substring ( 0 , 1 ) == "(" )
            {
                // parse away any leading parenthesis
                string tmp = NewWpfDev . Utils . SpanTrim ( sizearg , 1 , sizearg . Length - 1 ) . ToString ( );
                //string tmp = sizearg . Substring ( 1 , sizearg . Length - 1 ) . Trim ( );
                sizearg = tmp;
            }
            // parse away any trailing parenthesis
            if ( sizearg . Contains ( ')' ) )
                sizearg = sizearg . Substring ( 0 , sizearg . Length - 1 ) . Trim ( );
            return sizearg;
        }
        public string ValidateDataType ( string datatype )
        {
            if ( datatype != "STRING" )
            {
                if ( datatype != "INT" && datatype != "DECIMAL" && datatype != "FLOAT" && datatype != "DOUBLE" && datatype != "CURRENCY"
                    && datatype != "BIT" && datatype != "CURRENCY" && datatype != "NUMERIC" && datatype != "REAL" && datatype != "SMALLINT" )
                {
                    datatype = "STRING";   // default to varchar
                }
                //    if ( datatype == "INT" )
                //        datatype = "10";
            }
            else
            {
                if ( datatype != "STRING" && datatype != "NVARCHAR" && datatype != "VARCHAR" && datatype != "DATE" && datatype != "DATETIME" && datatype != "TIMESTAMP"
                    && datatype != "YEAR" && datatype != "CHAR" && datatype != "TEXT" && datatype != "NCHAR" && datatype != "NTEXT"
                    && datatype != "BINARY" && datatype != "VARBINARY" && datatype != "IMAGE" && datatype != "BLOB" && datatype != "JSON" )
                {
                    datatype = "INT";
                }
            }
            return datatype;
        }

        public void LoadExecuteEditor ( )
        {
            //Executedata . Clear ( );
            // string argstring = SPArguments . Text;
            //string [ ] args = argstring . Split ( ' ' );
            //if ( args . Length < 4 )
            //{
            //    args = argstring . Split ( ',' );
            //}
            //int count = 0;
            //string outbuff = "";
            //foreach ( var item in args )
            //{
            //    string output = "";
            //    if ( item == "" ) continue;
            //    if ( count < 3 )
            //    {
            //         if ( item . Contains ( "**]" ) )
            //        {
            //            outbuff += $"{item . ToString ( ) . Trim ( )} ";
            //        }
            //        else
            //            outbuff += $"{item . ToString ( ) . Trim ( )} ";
            //        count++;
            //        continue;
            //    }
            //    else
            //        outbuff = item . ToString ( ) . Trim ( );
            //    output = outbuff;
            //    Executedata . Add ( output );
            //}
        }
        private void Arguments_KeyDown ( object sender , KeyEventArgs e )
        {
            //call method below to handle dbleCLick on SPlist
            if ( e . Key == Key . Enter )
                ExecuteSp ( );
            //else if ( e . Key == Key . F8 )
            //{
            //    ExecuteEdit . Visibility = Visibility . Visible;
            //    LoadExecuteEditor ( );
            //    ExecuteEdit . ItemsSource = null;
            //    ExecuteEdit . Items . Clear ( );
            //    ExecuteEdit . ItemsSource = Executedata;
            //    ExecuteEdit . SelectedIndex = 0;
            //    ExecuteEdit . SelectedItem = 0;
            //    CreateEditBox ( ExecuteEdit );
            //}
            //else if ( e . Key == Key . F9 )
            //    ExecuteEdit . Visibility = Visibility . Collapsed;
        }

        private void TextBlock_KeyDown ( object sender , KeyEventArgs e )
        {
            //if ( e . Key == Key . F9 )
            //    ExecuteEdit . Visibility = Visibility . Collapsed;
        }
  
        private void expandargsentry ( object sender , RoutedEventArgs e )
        {
            if ( SPArguments . Height == 40 )
            {
                // Expanding height
                SPArgumentsFull . Text = SPArguments . Text;
                SPArguments . Visibility = Visibility . Collapsed;
                SPArgumentsFull . Visibility = Visibility . Visible;
                Thickness th = new Thickness ( );
                th = SPArguments . Margin;
                th . Top = 0;
                th . Bottom = 0;
                SPArguments . Margin = th;
                SPArguments . Height = 90;
                TogglepromptPanel ( false );
            }
            else
            {
                // Reducing height
                SPArguments . Text = SPArgumentsFull . Text;
                SPArgumentsFull . Text = SPArguments . Text;
                SPArgumentsFull . Visibility = Visibility . Collapsed;
                SPArguments . Visibility = Visibility . Visible;
                Thickness th = new Thickness ( );
                th = SPArguments . Margin;
                th . Top = 50;
                th . Bottom = 0;
                SPArguments . Height = 40;
                TogglepromptPanel ( true );
            }
        }

        private void expandprompt ( object sender , RoutedEventArgs e )
        {
            // show hide blue prompt entry field  
            if ( Parameterstop . Visibility == Visibility . Visible )
            {
                ShowOptionsPanel = false;
                TogglepromptPanel ( false );
            }
            else
            {
                ShowOptionsPanel = true;
                TogglepromptPanel ( true );
            }
        }

        public void TogglepromptPanel ( bool show )
        {
            // show hide blue prompt entry field  
            if ( show )
            {
                // show blue panel
                if ( SPArguments . Height == 40 && ShowOptionsPanel == true )
                {
                    Parameterstop . Visibility = Visibility . Visible;
                    ShowCboxes ( false );
                }
                else if ( SPArguments . Height == 40 && ShowOptionsPanel == false )
                {
                    Parameterstop . Visibility = Visibility . Collapsed;
                    ShowCboxes ( true );
                }
                else if ( SPArguments . Height == 90 )
                {
                    Parameterstop . Visibility = Visibility . Collapsed;
                    ShowCboxes ( false );
                }
            }
            else
            {
                // Hide blue panel so show checkboxes
                if ( SPArguments . Height == 40 && ShowOptionsPanel == true )
                {
                    Parameterstop . Visibility = Visibility . Visible;
                    ShowCboxes ( false );
                }
                else if ( SPArguments . Height == 40 && ShowOptionsPanel == false )
                {
                    Parameterstop . Visibility = Visibility . Collapsed;
                    ShowCboxes ( true );
                }
                else if ( SPArguments . Height == 90 && ShowOptionsPanel == false )
                {
                    Parameterstop . Visibility = Visibility . Collapsed;
                    ShowCboxes ( false );
                }
                else if ( SPArguments . Height == 90 && ShowOptionsPanel == true )
                {
                    Parameterstop . Visibility = Visibility . Collapsed;
                    ShowCboxes ( false );
                }
            }
            if ( SPArguments . Height == 90 )
            {
                gm41Text = "Contract Arguments entry panel";
                UpdateMenuItem ( "ResultsViewerContextMenu" , "gm41" );
            }
            else
            {
                gm41Text = "Expand Arguments entry panel";
                UpdateMenuItem ( "ResultsViewerContextMenu" , "gm41" );
            }
        }

        private void ShowCboxes ( bool show )
        {
            // show  hide check boxes
            // Called  by other Methods that effect prompt area
            if ( show )
            {
                ShowCheckboxes = true;
            }
            else
            {
                  ShowCheckboxes = false;
            }
        }

        private void SPArguments_MouseDoubleClick ( object sender , MouseButtonEventArgs e )
        {
            try
            {
                //    TextBox tb = sender as TextBox;
                //    string line = tb . Text;
                //    string seltext = tb . SelectedText . ToString ( );
                //    int caretstart = tb . CaretIndex;

                //    char [ ] charbuff = new char [ line . Length ];
                //    for ( int x = 0 ; x < line . Length ; x++ )
                //    {
                //        charbuff [ x ] = line [ x ];
                //    }
                //    int offsetstart = line . IndexOf ( seltext );
                //    int offsetend = offsetstart + seltext . Length;
                //    int charsellength = 0;
                //    string newselstring = "";
                //    int selstart = offsetstart;
                //    // Check backwards for start of line or a space
                //    int backspaces = 0;
                //    for ( int x = offsetstart ; x < line . Length ; x-- )
                //    {
                //        if ( x < 0 )
                //        {
                //            selstart = 0;
                //            break;
                //        }
                //        if ( charbuff [ x ] == ' ' )
                //        {
                //            selstart = x;
                //            break;
                //        }
                //        else
                //            backspaces++;
                //    }
                //    Debug . WriteLine ( $"offsetstart = {offsetstart}, backspaces = {backspaces}" );
                //    Console . WriteLine ( $"offsetstart = {offsetstart}, backspaces = {backspaces}" );
                //    // Check forwards for end of line or a trailing space
                //    for ( int x = ( selstart >= backspaces ? selstart - backspaces : 0 ) + seltext . Length ; x < line . Length ; x++ )
                //    {
                //        if ( charbuff [ x ] == ' ' )
                //            break;
                //        else
                //        {
                //            newselstring += charbuff [ x ];
                //            charsellength++;
                //        }
                //    }
                //    Debug . WriteLine ( $"charsellength = {charsellength}" );
                //    string newselectedbuffer = line . Substring ( offsetstart - charsellength , offsetstart + charsellength );
                //    tb . Select ( caretstart - charsellength , newselectedbuffer . Length );
            }
            catch ( Exception ex )
            {
                NewWpfDev . Utils . DoErrorBeep ( );
                Debug . WriteLine ( $"{ex . Message}" );
            }
            e . Handled = true;
        }
  
        private void ClearPrompt ( object sender , RoutedEventArgs e )
        {
            //TextBox tb = SpArguments as TextBox;
            SPArguments . Text = "";
            SPArgumentsFull . Text = SPArguments . Text;
            SPArguments . Focus ( );
        }

        private void showtooltip ( object sender , RoutedEventArgs e )
        {
            ListBox lb = optype as ListBox;
            //ToolTip tt = lb . ToolTip as ToolTip;
            //tt. IsOpen = true;
        }

        public void ShowExecutionHints ( object sender , RoutedEventArgs e )
        {
            string infotext = @$"C:\users\ianch\documents\Execution Methods Info.Txt";
            ShowArgumentsHelp sh = new ShowArgumentsHelp ( infotext , this );
            sh . header . Text = "IMPORTANT HELP : How  to select an Execution Method";
            sh . Show ( );
            //execinf . Text = "";
            //execinf . Text = infotext;
            //execinf . UpdateLayout ( );
            //Executeinfo . Visibility = Visibility . Visible;

        }
 
        private void AllTables_MouseDoubleClick ( object sender , MouseButtonEventArgs e )
        {
            string selection = "";
            string newcommand = "";
            ListBox lb = sender as ListBox;
            if ( lb != null )
            {
                selection = lb . SelectedItem . ToString ( );
                if ( SPArgumentsFull . Text . Contains ( "STRING" ) )
                {
                    string [ ] words = SPArgumentsFull . Text . Split ( " " );
                    for ( int x = 0 ; x < words . Length ; x++ )
                    {
                        if ( words [ x ] == "STRING" ) newcommand += $"{selection} ";
                        else newcommand += $"{words [ x ]} ";
                    }
                    SPArgumentsFull . Text = newcommand . Trim ( );
                    SPArguments . Text = SPArgumentsFull . Text;
                }
                SPArguments . UpdateLayout ( );
                SPArgumentsFull . UpdateLayout ( );
            }
        }

        public void ProcessSprocExecutionResult ( dynamic dynvar , int count , string ResultString , Type objtype , object obj , string err , Type newtype )
        {
            try
            {
                if ( dynvar != null )
                {
                    if ( newtype == null )
                    {
                        Mouse . OverrideCursor = Cursors . Arrow;
                        return;
                    }
                    //-------------------------------------------------------------------------------------------------//
                    if ( newtype == typeof ( string ) )
                    //-------------------------------------------------------------------------------------------------//
                    {
                        int listcount = 0;
                        string outputstring = "";

                        // Create list of data to be displayed
                        List<string> list = new List<string> ( );
                        foreach ( var str in dynvar )
                        {
                            list . Add ( str );
                            outputstring += $"{str}, ";
                            listcount++;
                        }
                        if ( list . Count == 0 )
                            listcount = 0;
                        outputstring = outputstring . Trim ( );
                        //remove terminating comma from script
                        if ( outputstring != "" ) outputstring = outputstring . Substring ( 0 , outputstring . Length - 1 );
                        // return value from SP String return method
                        // Go ahead & Show our results  dialog popup
                        if ( listcount == 0 )
                        {
                            GenResults . ExecutionInfo . Visibility = Visibility . Visible;
                            GenResults . CollectionTextresults . Visibility = Visibility . Visible;
                            GenResults . innerresultscontainer . RowDefinitions [ 1 ] . Height = new GridLength ( 1 , GridUnitType . Star );
                            GenResults . CollectionTextresults . Document =
                                Gengrid . LoadFlowDoc ( GenResults . CollectionTextresults ,
                                 FindResource ( "Black3" ) as SolidColorBrush ,
                            $"Execution of S.P [{ListResults . SelectedItem . ToString ( )}] using [{optype . SelectedItem . ToString ( )}] appears to have failed !\n\nThe data returned was {ResultString}.\n\nPerhaps using a different Execution Method will provide a better result ?" );
                            GenResults . CollectionTextresults . Document . Blocks . FirstBlock . FontSize = 18;
                            GenResults . Height = 280;
                            GenResults . innerresultscontainer . RowDefinitions [ 1 ] . Height = new GridLength ( 1 , GridUnitType . Pixel );
                            GenResults . CountResult . Text = $"ZERO";
                            NewWpfDev . Utils . DoErrorBeep ( );
                            GenResults . ShowDialog ( );
                            GenResults . Refresh ( );
                            Mouse . OverrideCursor = Cursors . Arrow;
                        }
                        else if ( list . Count > 1 )
                        {
                            GenResults . CollectionListboxresults . Items . Clear ( );
                            GenResults . CollectionListboxresults . ItemsSource = null;
                            GenResults . CollectionListboxresults . ItemsSource = list;
                            GenResults . CollectionListboxresults . Visibility = Visibility . Visible;
                            GenResults . CollectionTextresults . Visibility = Visibility . Visible;
                            GenResults . OperationResults . Visibility = Visibility . Visible;
                            GenResults . CollectionTextresults . Document = Gengrid . LoadFlowDoc ( GenResults . CollectionTextresults ,
                                FindResource ( "Black3" ) as SolidColorBrush ,
                               $"Execution of [{ListResults . SelectedItem . ToString ( )}] using [{optype . SelectedItem . ToString ( )}] completed successfully. \nbut contains a total of {list . Count} rows being returned. \n" +
                               $"Due to more than one string being returned the results are displayed in the listbox below... " );
                            GenResults . CollectionTextresults . Document . Blocks . FirstBlock . FontSize = 18;
                            GenResults . ExecutionInfo . Text = $"Execution of [{optype . SelectedItem . ToString ( )}]\ncompleted successfully, the results are listed above...";
                            GenResults . CountResult . Text = $"{list . Count}";
                            GenResults . innerresultscontainer . RowDefinitions [ 1 ] . Height = new GridLength ( 1 , GridUnitType . Star );

                            // Showing Scrollviewer Text AND DataGrid, so we stay full height
                            GenResults . Refresh ( );
                            NewWpfDev . Utils . DoSuccessBeep ( );
                            Mouse . OverrideCursor = Cursors . Arrow;
                            GenResults . Show ( );
                            GenResults . Topmost = true;
                            GenResults . CollectionListboxresults . SelectedIndex = 0;
                            GenResults . CollectionListboxresults . Focus ( );
                        }
                        else if ( list . Count == 1 )
                        {
                            // show single string result in Scrollviewer Document
                            string outline = list [ 0 ] . ToString ( );

                            GenResults . CollectionTextresults . Visibility = Visibility . Visible;
                            GenResults . TextResultsDocument . Visibility = Visibility . Visible;
                            GenResults . innerresultscontainer . RowDefinitions [ 1 ] . Height = new GridLength ( 1 , GridUnitType . Star );
                            //                            GenResults . innerresultscontainer . RowDefinitions [ 0 ] . Height = new GridLength ( 3.9 , GridUnitType . Star );
                            GenResults . CollectionTextresults . Document = Gengrid . LoadFlowDoc ( GenResults . CollectionTextresults ,
                             FindResource ( "Black3" ) as SolidColorBrush ,
                            $"Execution of S.P [{ListResults . SelectedItem . ToString ( )}] using [{optype . SelectedItem . ToString ( )}] completed successfully. The results of the request is shown in the viewer below." );
                            GenResults . TextResultsDocument . Document =
                                Gengrid . LoadFlowDoc ( GenResults . TextResultsDocument ,
                                 FindResource ( "Black3" ) as SolidColorBrush ,
                                $"{outline}" );
                            GenResults . CollectionTextresults . Document . Blocks . FirstBlock . FontSize = 18;
                            GenResults . ExecutionInfo . Text = $"Execution of [{optype . SelectedItem . ToString ( )}]\ncompleted successfully, the result is listed above...";
                            GenResults . CountResult . Text = "1";
                            GenResults . Refresh ( );
                            NewWpfDev . Utils . DoSuccessBeep ( );
                            Mouse . OverrideCursor = Cursors . Arrow;
                            GenResults . Show ( );
                            GenResults . Topmost = true;
                            Mouse . OverrideCursor = Cursors . Arrow;
                            return;
                        }
                    }
                    //-------------------------------------------------------------------------------------------------//
                    else if ( newtype == typeof ( Int32 ) )
                    //-------------------------------------------------------------------------------------------------//
                    {
                        // Got an INT value in returned (ref value count ) variable
                        string argstring = SPArguments . Text == "Argument(s) required ?" ? "" : SPArguments . Text;
                        SPArgumentsFull . Text = SPArguments . Text;
                        GenResults . CollectionTextresults . Visibility = Visibility . Visible;
                        GenResults . CollectionTextresults . Document =
                            Gengrid . LoadFlowDoc ( GenResults . CollectionTextresults ,
                             FindResource ( "Black3" ) as SolidColorBrush ,
                        $"The enquiry [ {ListResults . SelectedItem . ToString ( )} {argstring}] \nsuccessfully returned a numeric value - shown below in the left hand panel..." );
                        GenResults . CountResult . Text = $"{count}";
                        GenResults . CountResult . Background = FindResource ( "Green5" ) as SolidColorBrush;
                        GenResults . CountResult . Foreground = FindResource ( "Red4" ) as SolidColorBrush;
                        GenResults . ExecutionInfo . Text = $"Execution of  [{ListResults . SelectedItem . ToString ( )}] using [{optype . SelectedItem . ToString ( )}] completed successfully. \nThe count you requested is shown at the left of this information panel...";
                        // Squeeze unused row so buttons show in our 220 height
                        GenResults . CollectionTextresults . Document . Blocks . FirstBlock . FontSize = 18;
                        GenResults . Height = 300;
                        GenResults . innerresultscontainer . RowDefinitions [ 1 ] . Height = new GridLength ( 1 , GridUnitType . Star );
                        GenResults . UpdateLayout ( );
                        GenResults . Refresh ( );
                        NewWpfDev . Utils . DoSuccessBeep ( );
                        GenResults . Show ( );
                        GenResults . Topmost = true;
                        Mouse . OverrideCursor = Cursors . Arrow;
                        GenResults . Focus ( );
                        return;
                    }
                    //-------------------------------------------------------------------------------------------------//
                    else if ( newtype . ToString ( ) . Contains ( "List" ) )
                    //-------------------------------------------------------------------------------------------------//
                    {
                        // Got a List<string>
                        // Go  ahead & Show our results  dialog popup
                        string outputstring = "";
                        string errormsg = "";
                        int dictcount = 0;
                        int recordcount = 0;
                        List<string> newlist;
                        ObservableCollection<GenericClass> gengrid = new ObservableCollection<GenericClass> ( );
                        if ( dynvar . Count > 1 )
                        {
                            int colcount = 0;
                            string result = "";
                           List<List<string>> varlist = null;
                              Dictionary<string , object> dict = new Dictionary<string , object> ( );
                            Dictionary<string , string> outdict = new Dictionary<string , string> ( );

                            foreach ( var item in dynvar )
                            {
                                try
                                {
                                    // we need to create a dictionary for each row of data then add it to a GenericClass row then add row to Generics Db
                                    string buffer = "";
                                    List<int> VarcharList = new List<int> ( );
                                    DatagridControl . ParseDapperRow ( item , dict , out colcount );
                                    GenericClass gc = new GenericClass ( );
                                    dictcount = 1;
                                    int index = 1;
                                    int fldcount = dict . Count;
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
                                            NewWpfDev . Utils . DoErrorBeep ( );
                                            result = ex . Message;
                                        }
                                    }

                                    //remove trailing comma
                                    string s = buffer . Substring ( 0 , buffer . Length - 1 );
                                    buffer = s;
                                    // We now  have ONE single record, but need to add this  to a GenericClass structure 
                                    int reccount = 1;
                                    NewWpfDev . Utils . ParseDictIntoGenericClass ( outdict , reccount , ref gc );
                                     gengrid . Add ( gc );
                                }
                                catch ( Exception ex )
                                {
                                    result = $"SQLERROR : {ex . Message}";
                                    errormsg = result;
                                    NewWpfDev . Utils . DoErrorBeep ( );
                                    result . cwerror ( );
                                }
                                dict . Clear ( );
                                outdict . Clear ( );
                                dictcount = 1;
                            }

                            // Massage  data from dynamic collection into list<string>
                            List<string> list = SProcsDataHandling . CreateListFromGenericClass ( gengrid );

                            GenResults . CollectionListboxresults . ItemsSource = null;
                            GenResults . CollectionListboxresults . Items . Clear ( );
                            GenResults . CollectionListboxresults . ItemsSource = list;
                            recordcount = gengrid . Count;
                            GenResults . CollectionListboxresults . Visibility = Visibility . Visible;
                            GenResults . CollectionTextresults . Visibility = Visibility . Visible;
                            GenResults . OperationResults . Visibility = Visibility . Visible;
                            GenResults . CollectionTextresults . Document = Gengrid . LoadFlowDoc ( GenResults . CollectionTextresults ,
                                FindResource ( "Black3" ) as SolidColorBrush ,
                               $"Execution of [{ListResults . SelectedItem . ToString ( )}] using [{optype . SelectedItem . ToString ( )}] completed successfully. \n" +
                               $"with a total of {recordcount} items returned. \nThe results are shown below... " );
                            GenResults . CollectionTextresults . Document . Blocks . FirstBlock . FontSize = 18;
                            GenResults . ExecutionInfo . Text = $"Execution of [{optype . SelectedItem . ToString ( )}]\ncompleted successfully, the results are listed above...";
                            GenResults . CountResult . Text = $"{recordcount}";
                            GenResults . innerresultscontainer . RowDefinitions [ 1 ] . Height = new GridLength ( 1 , GridUnitType . Star );

                            // Showing Scrollviewer Text AND DataGrid, so we stay full height
                            GenResults . Refresh ( );
                            NewWpfDev . Utils . DoSuccessBeep ( );
                            Mouse . OverrideCursor = Cursors . Arrow;
                            GenResults . Show ( );
                            GenResults . Topmost = true;
                            GenResults . CollectionListboxresults . SelectedIndex = 0;
                            GenResults . CollectionListboxresults . Focus ( );
                        }
                        else if ( dynvar . Count == 1 )
                        {
                            outputstring = dynvar [ 0 ] . ToString ( );
                            int stringlen = outputstring . Length;
                            string argstring = SPArguments . Text == "Argument(s) required ?" ? "" : SPArguments . Text;
                            SPArgumentsFull . Text = SPArguments . Text;

                            GenResults . innerresultscontainer . RowDefinitions [ 1 ] . Height = new GridLength ( 1 , GridUnitType . Star );
                            GenResults . TextResultsDocument . Visibility = Visibility . Visible;
                            GenResults . CollectionTextresults . Visibility = Visibility . Visible;

                            GenResults . CollectionTextresults . Document = Gengrid . LoadFlowDoc ( GenResults . CollectionTextresults ,
                                 FindResource ( "Black3" ) as SolidColorBrush ,
                                $"The enquiry [{ListResults . SelectedItem . ToString ( )} {argstring}] completed successfully, but it only returned a single row, so that row is being displayed in a text viewer for you to view more easily" );
                            GenResults . ExecutionInfo . Text = $"Execution of [{ListResults . SelectedItem . ToString ( )}] using [{optype . SelectedItem . ToString ( )}] completed successfully, details shown above...";
                            GenResults . CollectionTextresults . Document . Blocks . FirstBlock . FontSize = 18;

                            GenResults . TextResultsDocument . Document = Gengrid . LoadFlowDoc ( GenResults . TextResultsDocument ,
                                 FindResource ( "Black3" ) as SolidColorBrush ,
                                $"{outputstring}" );
                            GenResults . CountResult . Text = $"1";
                            NewWpfDev . Utils . DoSuccessBeep ( );
                            GenResults . Show ( );
                            GenResults . Topmost = true;
                            GenResults . Refresh ( );
                            Mouse . OverrideCursor = Cursors . Arrow;
                        }
                        else
                        {
                            GenResults . innerresultscontainer . RowDefinitions [ 0 ] . Height = new GridLength ( 3.9 , GridUnitType . Star );
                            string data = "";
                            foreach ( var item in dynvar )
                            {
                                data += $"{item . ToString ( )}\n";
                            }
                            string argstring = SPArguments . Text == "Argument(s) required ?" ? "" : SPArguments . Text;
                            SPArgumentsFull . Text = SPArguments . Text;
                            GenResults . CollectionTextresults . Document = Gengrid . LoadFlowDoc ( GenResults . CollectionTextresults ,
                                FindResource ( "Black3" ) as SolidColorBrush ,
                               $"The enquiry [{ListResults . SelectedItem . ToString ( )} {argstring}] responded with the following values\n\n[{data}]" );
                            GenResults . ExecutionInfo . Text = $"Execution of [{ListResults . SelectedItem . ToString ( )} using {optype . SelectedItem . ToString ( )}] completed with the list containing the data shown above!";
                            GenResults . CollectionTextresults . Document . Blocks . FirstBlock . FontSize = 18;
                            GenResults . CountResult . Text = $"{count}";
                            // Showing Scrollviewer Text Only, so reduce height
                            // Squeeze unused row so buttons show in our 220 height
                            GenResults . Height = 280;
                            GenResults . innerresultscontainer . RowDefinitions [ 1 ] . Height = new GridLength ( 1 , GridUnitType . Pixel );
                            NewWpfDev . Utils . PlayErrorBeep ( );
                            GenResults . Show ( );
                            GenResults . Topmost = true;
                            GenResults . Refresh ( );
                        }
                    }
                    //-------------------------------------------------------------------------------------------------//
                    else if ( newtype . ToString ( ) . ToUpper ( ) . Contains ( "IENUMERABLE" ) == true )
                    //-------------------------------------------------------------------------------------------------//
                    {
                        // WORKING WELL 19/11/2022
                        // got an observable collection in Dynamic collection, so display it
                        int colcount = 0;
                        string outline = "";
                        string argstring = SPArgumentsFull . Text;
                        ObservableCollection<GenericClass> genclass = new ObservableCollection<GenericClass> ( );
                        ObservableCollection<GenericClass> output = new ObservableCollection<GenericClass> ( );
                        genclass = GenDapperQueries . ParseDynamicToCollection (
                                     dynvar ,
                                     out string errormsg ,
                                     out int reccount ,
                                     out List<string> genericlist );
                        if ( genclass . Count == 1 )
                        {
                            GenericClass gc = new GenericClass ( );
                            gc = genclass [ 0 ];
                            outline = gc . field1 . ToString ( );
                            colcount = DatagridControl . GetColumnsCount ( genclass , null );
                        }
                        if ( colcount == 1 )
                        {
                            // show single string result in Scrollviewer Document

                            GenResults . CollectionTextresults . Visibility = Visibility . Visible;
                            GenResults . TextResultsDocument . Visibility = Visibility . Visible;
                            GenResults . innerresultscontainer . RowDefinitions [ 1 ] . Height = new GridLength ( 1 , GridUnitType . Star );
                            GenResults . CollectionTextresults . Document = Gengrid . LoadFlowDoc ( GenResults . CollectionTextresults ,
                             FindResource ( "Black3" ) as SolidColorBrush ,
                            $"Execution of S.P [{ListResults . SelectedItem . ToString ( )}] using [{optype . SelectedItem . ToString ( )}] completed successfully, BUT only returned " +
                            $"a single row with just one Column, so this column's content is shown in the text viewer below." );
                            GenResults . TextResultsDocument . Document =
                                Gengrid . LoadFlowDoc ( GenResults . TextResultsDocument ,
                                 FindResource ( "Black3" ) as SolidColorBrush ,
                                $"{outline}" );
                            GenResults . CollectionTextresults . Document . Blocks . FirstBlock . FontSize = 18;
                            GenResults . ExecutionInfo . Text = $"Execution of [{optype . SelectedItem . ToString ( )}]\ncompleted successfully, the result is listed above...";
                            GenResults . CountResult . Text = "1";
                            GenResults . Refresh ( );
                        }
                        else
                        {
                            GenResults . CollectionTextresults . Visibility = Visibility . Visible;
                            GenResults . CollectionGridresults . Visibility = Visibility . Visible;
                            GenResults . innerresultscontainer . RowDefinitions [ 1 ] . Height = new GridLength ( 1 , GridUnitType . Star );
                            argstring = SPArguments . Text == "Argument(s) required ?" ? "" : SPArguments . Text;
                            SPArgumentsFull . Text = SPArguments . Text;
                            string [ ] parts = SPArguments . Text . Split ( ',' );
                            count = genclass . Count;
                            GenResults . CountResult . Text = $"{count}";
                            GenResults . CollectionGridresults . ItemsSource = genclass;
                            GenResults . CollectionTextresults . Document = Gengrid . LoadFlowDoc ( GenResults . CollectionTextresults ,
                            FindResource ( "Black3" ) as SolidColorBrush ,
                                $"Execution of [{ListResults . SelectedItem . ToString ( )} using [{optype . SelectedItem . ToString ( )}] was processed successfully \nand returned a value of [{count}] records..." );
                            GenResults . ExecutionInfo . Text = $"Execution of [{optype . SelectedItem . ToString ( )}]\ncompleted successfully, details shown above...";
                            GenResults . CollectionTextresults . Document . Blocks . FirstBlock . FontSize = 18;
                        }
                        NewWpfDev . Utils . DoSuccessBeep ( );
                        GenResults . Show ( );
                        GenResults . Topmost = true;
                        // GenResults . Refresh ( );
                        SPArguments . Text = argstring;
                        Mouse . OverrideCursor = Cursors . Arrow;
                    }
                    else
                    {
                        // Unknown result
                        string argstring = SPArguments . Text == "Argument(s) required ?" ? "" : SPArguments . Text;
                        SPArgumentsFull . Text = SPArguments . Text;
                        GenResults . CollectionTextresults . Visibility = Visibility . Visible;
                        GenResults . CollectionTextresults . Document = Gengrid . LoadFlowDoc ( GenResults . CollectionTextresults ,
                             FindResource ( "Black3" ) as SolidColorBrush ,
                           $"Execution of [{ListResults . SelectedItem . ToString ( )} using {optype . SelectedItem . ToString ( )}] completed successfully \nbut failed to return any type of value ...\n\nTry a different Method of Execution !" );
                        GenResults . ExecutionInfo . Text = $"Execution of [{optype . SelectedItem . ToString ( )}] completed successfully, but see notes above...";
                        GenResults . CollectionTextresults . Document . Blocks . FirstBlock . FontSize = 18;
                        // Showing Scrollviewer Text Only, so reduce height
                        // Squeeze unused row so buttons show in our 220 height
                        GenResults . CountResult . Text = $"ZERO";
                        GenResults . Height = 280;
                        GenResults . innerresultscontainer . RowDefinitions [ 1 ] . Height = new GridLength ( 1 , GridUnitType . Pixel );
                        NewWpfDev . Utils . PlayErrorBeep ( );
                        GenResults . Show ( );
                        GenResults . Topmost = true;
                        GenResults . Refresh ( );
                        Mouse . OverrideCursor = Cursors . Arrow;
                        return;
                    }
                    Mouse . OverrideCursor = Cursors . Arrow;
                    return;
                }
            }
            catch ( Exception ex )
            {
                Utils . DoErrorBeep ( );
                Debug . WriteLine ( $"SQL error encountered ...\n {ex . Message}, [{ex . Data}]" );
            }
        }

        public static void ShowDataViewer ( string textline1 , string fontfamily = "Nirmala UI" , string fontstyle = "Normal" , string fontsize = "14" , string fontcolor = "Black0" , bool IsFixed = false )
        {
            // Display Popup widnow for messages of all types
            //            DataViewer dv = new DataViewer ( output , isFixed: IsFixed );
            DataViewer dv = new DataViewer ( );
            dv . ShowDataViewer ( textline1 , isFixed: IsFixed );
            dv . Show ( );
        }

        private void DisplayHeaderInfo ( object sender , RoutedEventArgs e )
        {
            // load headerblock and dissect it
            string sptext = "";
            string HeaderBlock = "";
            string output = "";
            bool result = Gengrid . LoadShowMatchingSproc ( this , TextResult , ListResults . SelectedItem . ToString ( ) , ref sptext , Convert . ToInt32 ( ScrollViewerFontSize ) , false );
            ShowParseDetails = true;
            ShowDataViewer ( sptext , fontfamily: "Nirmala UI" , fontstyle: "Normal" , fontsize: "14" , fontcolor: "Black0" , IsFixed: false );
          }

        private void tbarBtn2_Click ( object sender , RoutedEventArgs e )
        {

        }

        private void topmost_Click ( object sender , RoutedEventArgs e )
        {
            if ( this . Topmost == true )
            {
                this . Topmost = false;
                tbarBtn2 . Content = "Set as OnTop";
            }
            else
            {
                this . Topmost = true;
                tbarBtn2 . Content = "Cancel OnTop";
            }
        }

        #region Font size handlers

        private void CloseFont_Click ( object sender , RoutedEventArgs e )
        {
            FontSizeChanger . Visibility = Visibility . Collapsed;
        }

        private void FontSizeChanger_Loaded ( object sender , RoutedEventArgs e )
        {
            List<string> sizes = new List<string> ( );
            for ( int x = 11 ; x < 21 ; x++ )
            {
                sizes . Add ( $"{x}" );
            }
            SpListboxFontSize = NewWpfDev . Properties . Settings . Default . SpResultLbFontSize;
            ScrollViewerFontSize = NewWpfDev . Properties . Settings . Default . SpResultScrollviewerFontSize;
            TbFonts . ItemsSource = null;
            TbFonts . Items . Clear ( );
            TbFonts . ItemsSource = sizes;
            int index = 0;
            foreach ( string fsize in sizes )
            {
                if ( fsize == SpListboxFontSize )
                {
                    TbFonts . SelectedIndex = index;
                    break;
                }
                index++;
            }
            ScrollFontSize . ItemsSource = null;
            ScrollFontSize . Items . Clear ( );
            ScrollFontSize . ItemsSource = sizes;
            index = 0;
            foreach ( string fsize in sizes )
            {
                if ( fsize == ScrollViewerFontSize )
                {
                    ScrollFontSize . SelectedIndex = index;
                    break;
                }
                index++;
            }

        }

          private void Fonts_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {
            ComboBox cb = null;
            if ( IsLoading )
            {
                SpListboxFontSize = NewWpfDev . Properties . Settings . Default . SpResultLbFontSize;
                ScrollViewerFontSize = NewWpfDev . Properties . Settings . Default . SpResultScrollviewerFontSize;

                return;
            }
          string newsize = "";
            string infotext = "";
            ComboBox cbt = sender as ComboBox;
            if ( cbt . Name == "TbFonts" )
                cb = TbFonts;
            else if ( cbt . Name == "ScrollFontSize" )
                cb = ScrollFontSize;
            else
                return;
            if ( cb != null )
            {
                SolidColorBrush newcolor = FindResource ( "Black4" ) as SolidColorBrush;
                newsize = cb . SelectedItem . ToString ( );
                Fontsize = Convert . ToInt32 ( newsize );
                if ( cbt . Name == "TbFonts" )
                {
                    SpListFontsize = Fontsize;
                    ListResults . FontSize = SpListFontsize;
                    ListResults . UpdateLayout ( );
                }
                else if ( cbt . Name == "ScrollFontSize" )
                {
                    FlowDocument doc = TextResult . Document;
                    string sptext = "";

                     ScrollViewerFontSize = Fontsize . ToString ( );
                    doc . Blocks . Clear ( );
                    string spname = ListResults . SelectedItem . ToString ( );
                    bool result = Gengrid . LoadShowMatchingSproc ( this , TextResult , spname , ref sptext , Convert . ToInt32 ( ScrollViewerFontSize ) );
                    TextResult . UpdateLayout ( );
                }
                Fontsize = 0;
                IsDirty = true;
            }
        }

        #endregion Fonts size handlers

        private void ListResults_MouseRightButtonUp ( object sender , MouseButtonEventArgs e )
        {
            e . Handled = true;
        }

        private void optype_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {
                        ExName . Text = optype . SelectedItem . ToString ( );

        }
    }
}

