using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . ComponentModel . DataAnnotations;
using System . Diagnostics;
using System . Printing;
using System . Reflection . Metadata;
using System . Text . RegularExpressions;
using System . Threading;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Documents;
using System . Windows . Documents . DocumentStructures;
using System . Windows . Input;
using System . Windows . Media;

using Expandos;

using IronPython . Compiler . Ast;
using IronPython . Runtime;

using Microsoft . VisualBasic;

using NewWpfDev;

//using StoredProcs;

using UserControls;

using static Community . CsharpSqlite . Sqlite3;
using static IronPython . Modules . _ast;

namespace Views
{
    /// <summary>
    /// Interaction logic for SpResultsViewer.xaml
    /// </summary>
    public partial class SpResultsViewer : Window
    {
        Genericgrid Gengrid { get; set; }
        public string Searchtext { get; set; }
        public string Searchterm { get; set; }
        public bool CloseArgsViewerOnPaste { get; set; } = false;
        public bool ShowTypesInArgsViewer { get; set; } = true;
        public bool ShowParseDetails { get; set; } = false;
        public static FlowDocScrollViewerSupport FdSupport { get; set; }
        public static string [ ] arguments = new string [ 10 ];
        public GengridExecutionResults GenResults { get; set; }

        public static bool IsMoving = false;
        public ObservableCollection<GenericClass> DataGrid = new ObservableCollection<GenericClass> ( );
        static public DragCtrlHelper DragCtrl;
        public FrameworkElement ActiveDragControl { get; set; }
        public static dynamic spViewerexpobj { get; private set; }

        public struct ExecutionResults
        {
            public int resultInt { get; set; }
            public string resultString { get; set; }
            public List<string> resultStringList { get; set; }
            public double resultDouble { get; set; }
            public ObservableCollection<GenericClass> resultCollection { get; set; }
        }
        public static ExecutionResults ExecResults;
        public bool IsFlashing { get; set; } = false;

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

        #endregion Dependecy properties

        public SpResultsViewer ( Genericgrid genControl , string sproc , string searchterm )
        {
            InitializeComponent ( );
            WpfLib1 . Utils . SetupWindowDrag ( this );
            Gengrid = genControl;
            //nResults = Gengrid.
            FdSupport = new FlowDocScrollViewerSupport ( );
            DragCtrl = new DragCtrlHelper ( SqlTablesViewer );
            Genericgrid . Resultsviewer = this;
            string spname = Gengrid . SpName . Text;
            Searchtext = spname;
            string [ ] args = new string [ 1 ];
            Mouse . OverrideCursor = Cursors . Wait;
            canvas . Visibility = Visibility . Visible;
            // setup a pointer to ourselves
            this . Topmost = false;
            Gengrid . ExecuteLoaded = true;
            hspltter . Cursor = Cursors . ScrollNS;

            ShowTypesInArgsViewer = ( bool ) MainWindow . GetSystemSetting ( "ShowTypesInSpArgumentsString" );
            CloseArgsViewerOnPaste = ( bool ) MainWindow . GetSystemSetting ( "AutoCloseSpArgumentsViewer" );
            OntopCheck . IsChecked = ( bool ) MainWindow . GetSystemSetting ( "SpResultsViewerOnTop" );
            if ( ( bool ) MainWindow . GetSystemSetting ( "SpViewerUseDarkMode" ) == true )
            {
                SpViewerResults . Background = FindResource ( "Black3" ) as SolidColorBrush;
                ShowingAllSprocs . Foreground = FindResource ( "White0" ) as SolidColorBrush;
                prompter . Foreground = FindResource ( "White0" ) as SolidColorBrush;
                BannerGrid . Background = FindResource ( "Orange5" ) as SolidColorBrush;
                OntopCheck . Foreground = FindResource ( "White0" ) as SolidColorBrush;
                MovingObject = SqlTablesViewer;
            }
            if ( spViewerexpobj == null )
                spViewerexpobj = ExpandoClass . GetNewExpandoObject ( );

            var t = new Dictionary<string , object> ( );
            t . Add ( "asdda" , 4456 );


            return;
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
                bool result = Gengrid . LoadShowMatchingSproc ( this , TextResult , selname , ref sptext );
                ShowParseDetails = true;
                if ( ShowParseDetails )
                {
                    string Arguments = StoredProcs . SProcsDataHandling . GetSpHeaderBlock ( Gengrid . SpTextBuffer );
                    if ( Arguments . Contains ( "No  Arguments were found" ) == true )
                        SPArguments . Text = Arguments;
                    else
                    {
                        string argsline = StoredProcs . SProcsDataHandling . CreateSProcArgsList ( Arguments , selname , out bool success );
                        SPArguments . Text = argsline;
                    }
                }
                else
                {
                    //Reset arguments panel
                    SPArguments . Text = "Argument(s) required ?";
                    optype . SelectedIndex = -1;    // unselect selection of S.P listbox
                    optype . SelectedItem = null;    // unselect method listbox
                }
            }
        }

        private void TextResult_PreviewMouseRightButtonDown ( object sender , MouseButtonEventArgs e )
        {
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
        }

        private void closeresultsviewer_Click ( object sender , RoutedEventArgs e )
        {
            Gengrid . ExecuteLoaded = false;
            this . Close ( );
        }

        private void SpStrings_KeyDown ( object sender , KeyEventArgs e )
        {

        }

        private void Arguments_KeyDown ( object sender , KeyEventArgs e )
        {
            //call method below to handle dbleCLick on SPlist
            if ( e . Key == Key . Enter )
                ExecuteSp ( );
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
            //            string [ ] args = new string [ 1 ];
            string err = "";

            if ( optype . SelectedItem == null )
            {
                MessageBox . Show ( "You MUST select an Execution Method before the selected S.P can be executed !" , "Execution processing error" );
                return;
            }
            int count = 0;
            ///////============================///////
            string ResultString = "";
            Type objtype = null;
            object obj = new object ( );
            int testint = 0;

            dynamic dynvar = Execute_click ( ref count , ref ResultString , ref objtype , ref obj , out err );
            ///////===========================///////
            // see if we have received a specific type (from objtype)

            string resultstring = ResultString;
            // Failures will always return NULL
            if ( err != "" )
            {
                Mouse . OverrideCursor = Cursors . Arrow;
//                return;
            }
            if ( dynvar == null || objtype == null )
            {
                Mouse . OverrideCursor = Cursors . Arrow;
                //WpfLib1 . Utils . DoErrorBeep ( );
                //MessageBox . Show ( $"Your request failed with no error information being returned.\nTry a different Execution Method" , "SQL Error" );
                //return;
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
                    if( err=="")
                    err = $"An undetermined error occured during Execution of [ {ListResults . SelectedItem . ToString ( )}.  Check that any required arguments were passed "+
                            $"correctly to the enquiry, and failing that try using a diferent Execution Method ";
                }
            }catch(Exception ex)
            {
                Console . WriteLine ($"Error occurred parsing return types from SLQ  Execution request\nError was {ex. Message}");
                Mouse . OverrideCursor = Cursors . Arrow;
                return ;
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

            // find out what we have  got
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
                        if ( dynvar . Count == 0 )
                        {
                            NewWpfDev . Utils . DoErrorBeep ( );
                            Mouse . OverrideCursor = Cursors . Arrow;
                            MessageBox . Show ( "Your request for a string value completed, BUT it failed to return any value. \n\nCheck that you have passed any expected Arguments, and that they are correct, as this can often cause this type of error" , "S.P Execution system" );
                            return;
                        }

                        string outputstring = "";
                        List<string> list = new List<string> ( );

                        foreach ( var str in dynvar )
                        {
                            list . Add ( str );
                            outputstring += $"{str}, ";
                        }
                        outputstring = outputstring . Trim ( );
                        outputstring = outputstring . Substring ( 0 , outputstring . Length - 1 );
                        // return value from SP String return method
                        // Go   ahead & Show our results  dialog popup
                        if ( list . Count == 0 )
                        {
                            GenResults . ExecutionInfo . Visibility = Visibility . Visible;
                            GenResults . CollectionTextresults . Visibility = Visibility . Visible;
                            //GenResults . innerresultscontainer . RowDefinitions [ 0 ] . Height = new GridLength ( 3.9 , GridUnitType . Star );
                            GenResults . innerresultscontainer . RowDefinitions [ 1 ] . Height = new GridLength ( 1 , GridUnitType . Star );
                            GenResults . CollectionTextresults . Document =
                                Gengrid . LoadFlowDoc ( GenResults . CollectionTextresults ,
                                 FindResource ( "Black3" ) as SolidColorBrush ,
                            $"Execution of S.P [{ListResults . SelectedItem . ToString ( )}] using [{optype . SelectedItem . ToString ( )}] appears to have failed !\n\nThe data returned was {resultstring}.\n\nPerhaps using a different Execution Method will provide a better result ?" );
                            GenResults . CollectionTextresults . Document . Blocks . FirstBlock . FontSize = 18;
                            GenResults . CountResult . Text = $"ZERO";
                            NewWpfDev . Utils . DoSuccessBeep ( );
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
                        List<string> list = new List<string> ( );
                        ObservableCollection<GenericClass> gengrid = new ObservableCollection<GenericClass> ( );
                        if ( dynvar . Count > 1 )
                        {
                            list = dynvar;
                            GenResults . CollectionListboxresults . Items . Clear ( );
                            GenResults . CollectionListboxresults . ItemsSource = null;
                            GenResults . CollectionListboxresults . ItemsSource = list;
                            GenResults . CollectionListboxresults . Visibility = Visibility . Visible;
                            GenResults . CollectionTextresults . Visibility = Visibility . Visible;
                            GenResults . OperationResults . Visibility = Visibility . Visible;
                            GenResults . CollectionTextresults . Document = Gengrid . LoadFlowDoc ( GenResults . CollectionTextresults ,
                                FindResource ( "Black3" ) as SolidColorBrush ,
                               $"Execution of [{ListResults . SelectedItem . ToString ( )}] using [{optype . SelectedItem . ToString ( )}] completed successfully. \n" +
                               $"with a total of {list . Count} items returned. \nThe results are shown below... " );
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
                        else if ( dynvar . Count == 1 )
                        {
                            outputstring = dynvar [ 0 ] . ToString ( );
                            int stringlen = outputstring . Length;
                            string argstring = SPArguments . Text == "Argument(s) required ?" ? "" : SPArguments . Text;

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
                       ObservableCollection <GenericClass> genclass = new ObservableCollection<GenericClass> ( );
                        ObservableCollection<GenericClass> output = new ObservableCollection<GenericClass> ( );
                        genclass = GenDapperQueries . ParseDynamicToCollection (
                                     dynvar ,
                                     out string errormsg ,
                                     out int reccount ,
                                     out List<string> genericlist );
                        if( genclass .Count == 1)
                        {
                            GenericClass gc = new GenericClass ( );
                            gc = genclass [ 0 ];
                            outline = gc . field1 . ToString ( );
                            colcount = DatagridControl . GetColumnsCount ( genclass, null);
                        }
                        if ( colcount == 1 )
                        {
                            // show single string result in Scrollviewer Document
                           

                            GenResults . CollectionTextresults . Visibility = Visibility . Visible;
                            GenResults . TextResultsDocument . Visibility = Visibility . Visible;
                            GenResults . innerresultscontainer . RowDefinitions [ 1 ] . Height = new GridLength ( 1 , GridUnitType . Star );
                            //                            GenResults . innerresultscontainer . RowDefinitions [ 0 ] . Height = new GridLength ( 3.9 , GridUnitType . Star );
                            GenResults . CollectionTextresults . Document = Gengrid . LoadFlowDoc ( GenResults . CollectionTextresults ,
                             FindResource ( "Black3" ) as SolidColorBrush ,
                            $"Execution of S.P [{ListResults . SelectedItem . ToString ( )}] using [{optype . SelectedItem . ToString ( )}] completed successfully, BUT only returned "+
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
                            string argstring = SPArguments . Text == "Argument(s) required ?" ? "" : SPArguments . Text;
                            string [ ] parts = SPArguments . Text . Split ( ',' );
                            count = genclass . Count;
                            GenResults . CountResult . Text = $"{count}";
                            GenResults . CollectionGridresults . ItemsSource = genclass;
                            //GenResults . CollectionGridresults . ItemsSource = dynvar;
                            GenResults . CollectionTextresults . Document = Gengrid . LoadFlowDoc ( GenResults . CollectionTextresults ,
                            FindResource ( "Black3" ) as SolidColorBrush ,
                                $"Execution of [{ListResults . SelectedItem . ToString ( )} using [{optype . SelectedItem . ToString ( )}] was processed successfully \nand returned a value of [{count}] records..." );
                            GenResults . ExecutionInfo . Text = $"Execution of [{optype . SelectedItem . ToString ( )}]\ncompleted successfully, details shown above...";
                            GenResults . CollectionTextresults . Document . Blocks . FirstBlock . FontSize = 18;
                        }
                        NewWpfDev . Utils . DoSuccessBeep ( );
                        GenResults . Show ( );
                        GenResults . Topmost = true;
                        GenResults . Refresh ( );
                        Mouse . OverrideCursor = Cursors . Arrow;
                    }
                    else
                    {
                        // Unknown result
                        string argstring = SPArguments . Text == "Argument(s) required ?" ? "" : SPArguments . Text;
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

                    //              NewWpfDev . Utils . DoSuccessBeep ( );
                    Mouse . OverrideCursor = Cursors . Arrow;
                    return;

                }
            }
            catch ( Exception ex )
            {
                Utils . DoErrorBeep ( );
//                NewWpfDev . Utils . PlayErrorBeep ( );
                Debug . WriteLine ( $"SQL error encountered ...\n {ex . Message}, [{ex . Data}]" );
            }
            Mouse . OverrideCursor = Cursors . Arrow;
        }


        private dynamic Execute_click ( ref int Count , ref string ResultString , ref Type Objtype , ref object Obj , out string Err )
        {
            // called when executing an SP
            string operationtype = optype . SelectedItem as string;
            string [ ] args1 = null;
            string [ ] args = new string [ 0 ];

            // Initiaize ref variables

            {
                Count = 0;
                string Resultstring = "";
                Objtype = null;
                Err = "";

                if ( operationtype == null )
                {
                    MessageBox . Show ( "You MUST select an Execution Method before the selected S.P can be executed !" , "Execution processing error" );
                    return null;
                }

                Searchtext = SPArguments . Text;
                if ( Searchtext == "Argument(s) required ?" )
                    Searchtext = "";

                if ( Searchtext != "" )
                {
                    // sort out arguments   1st of all
                    args1 = Searchtext . Trim ( ) . Split ( ',' );
                }
                if ( args1 != null && args1 . Length > 0 )
                {
                    //Check for output args 1st
                    int cnt = args1 . Length;
                    if ( cnt > 0 )
                    {
                        args = new string [ cnt ];
                        for ( int x = 0 ; x < cnt ; x++ )
                        {
                            args [ x ] = args1 [ x ] . Trim ( );
                        }
                    }
                }
                else
                    args = null;
            }

            int innercount = Count;
            string innerresultstring = ResultString;
            object innerobj = Obj;
            string innerrerr = Err;

            string SqlCommand = $"{ListResults . SelectedItem . ToString ( )}";
            try
            {
                string output = "";
               // string SqlCommand = "";
                // Now find out what method we are going to use
                if ( operationtype == "SP returning an INT value" )
                {
                    if ( args == null || args . Length == 0 )
                    {
                        string buffer = "";
                        string [ ] buff;
                        string [ ] items;
                        int offset = 0;
                        string argstring = SPArguments . Text . Trim ( );
                        while ( true )
                        {
                            if ( argstring . Contains ( ' ' ) )
                            {
                                items = argstring . Split ( ' ' );
                                for ( int x = 0 ; x < items . Length ; x++ )
                                {
                                    buffer += $"{items [ x ] . TrimStart ( ) . TrimEnd ( )}/";
                                }
                                //buffer += $":{args [ 1 ]}";
                                argstring = buffer;
                                buffer = "";
                            }
                            buff = argstring . Split ( '/' );
                            items = null;
                            items = new string [ buff . Length ];
                            for ( int x = 0 ; x < buff . Length ; x++ )
                            {
                                if ( buff [ x ] != "=" )
                                    items [ offset++ ] = buff [ x ] . TrimStart ( ) . TrimEnd ( );
                            }
                            buffer = "";
                            for ( int x = 0 ; x < items . Length ; x++ )
                            {
                                if ( items [ x ] == "" || items [ x ] == null )
                                    break;
                                buffer += items [ x ];
                                if ( x % 2 == 0 )
                                {
                                    if ( items [ x ] . Contains ( '=' ) == false )
                                        buffer += "=";
                                }

                                if ( x % 2 == 1 )

                                    buffer += " ";
                            }
                            break;
                        }
                        output = buffer;
                        //SqlCommand = $"{ListResults . SelectedItem . ToString ( )} {output}";
                    }
                    //else
                    //    SpCommand = this . ListResults . SelectedItem . ToString ( );
                    // tell method what we are expecting back
                    Objtype = typeof ( int );

                    //********************************************************************************//
                    dynamic intresult = GenDapperQueries . Get_DynamicValue_ViaDapper ( SqlCommand ,
                        args ,
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
                    //Use storedprocedure  version
                    SqlCommand = $"{ListResults . SelectedItem . ToString ( )}";

                    // tell method what we are expecting back
                    Objtype = typeof ( string );

                    //********************************************************************************//
                    dynamic stringresult = GenDapperQueries . Get_DynamicValue_ViaDapper (
                        SqlCommand , args ,
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
                    SqlCommand = $"{ListResults . SelectedItem . ToString ( )}";

                    // tell method what we are expecting back
                    Objtype = typeof ( List<string> );

                    //********************************************************************************//
                    dynamic stringlist = GenDapperQueries . Get_DynamicValue_ViaDapper (
                        SqlCommand , args ,
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

                    ////********************************************************************************//
                    //list = StoredprocsProcessing . ProcessGenericDapperStoredProcedure (
                    //     ListResults . SelectedItem . ToString ( ) ,
                    //    args ,
                    //    Genericgrid . CurrentTableDomain ,
                    //    ref innerresultstring ,
                    //    ref innerobj ,
                    //    ref Objtype ,
                    //    ref innercount ,
                    //    ref Err );
                    ////********************************************************************************//

                    //ResultString = innerresultstring;
                    //Obj = ( object ) list;
                    //Objtype = typeof ( List<string> );
                    //if ( list != null )
                    //{
                    //    Count = list . Count;
                    //}
                    //if ( Objtype == typeof ( List<string> ) )
                    //    return ( dynamic ) list;
                    //else
                    //    return ( dynamic ) null;

                    ////    return ( dynamic ) null;
                }
                else if ( operationtype == "SP returning a Table as ObservableCollection" )
                {
                    DatagridControl dgc = new ( );

                    // tell method what we are expecting back
                    Objtype = typeof ( ObservableCollection<GenericClass> );

                    //********************************************************************************//
                    // Should normally  be  '[spLoadTableAsGeneric]' but can be any SP that wants a collection back
                    IEnumerable<dynamic> tableresult = GenDapperQueries . Get_DynamicValue_ViaDapper (
                     ListResults . SelectedItem . ToString ( ) ,
                     args ,
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
                //---------------------------------------------------------------------------------------------------//
                else if ( operationtype == "SP returning a 'Pot Luck' result" )
                //---------------------------------------------------------------------------------------------------//
                {
                    DatagridControl dgc = new ( );
                    int recordcount = 0;

                    // tell method what we are expecting back ??????
                    //Objtype = typeof ( ObservableCollection<GenericClass> );

                    //********************************************************************************//
                    ObservableCollection<GenericClass>
                       rescollection = dgc . GetDataFromStoredProcedure (
                         ListResults . SelectedItem . ToString ( ) ,
                        args ,
                        Genericgrid . CurrentTableDomain ,
                        out Err ,
                        out recordcount );
                    //********************************************************************************//

                    if ( Err == "" && rescollection . Count > 0 )
                    {
                        string output2 = "";
                        int returnedcount = rescollection . Count;
                        innercount = returnedcount;
                        if ( returnedcount > 0 )
                        {
                            output2 = $"Results of the {optype . SelectedItem . ToString ( )} request is shown below\n\n";
                            foreach ( var item in rescollection )
                            {
                                output2 += $"{item . field1 . ToString ( )}\n";
                            }
                            Obj = output2;
                            ResultString = "SUCCESS";
                            return ( dynamic ) Obj;
                        }

                        else
                        {
                            Err = $"No usable values were returned";
                            Obj = ( object ) Err;
                            ResultString = "FAILURE";
                            return ( dynamic ) Obj;
                        }
                    }
                    else if ( rescollection . Count == 0 && Err == "" )
                    {
                        DatagridControl dg = new DatagridControl ( );

                        //********************************************************************************//
//                        var result = dg . GetDataFromStoredProcedure ( "Select columncount from countreturnvalue" , null , "" , out Err , out recordcount , 1 );
                        //********************************************************************************//

   //                     if ( result . Count == 0 )
                            MessageBox . Show ( $"No Error was encountered,  but the request did NOT return any type of value...\n\nPerhaps the processing method that you selected as shown below :-\n" +
                                $"[{optype . SelectedItem . ToString ( ) . ToUpper ( )}]\n was not the correct processing method type for this Stored.Procedure ?" , "SQL Error" );
                        //if ( ReturnProcedureHeader ( "Select columncount from countreturnvalue" , "" ) == "DONE" )
                       
                        return ( dynamic ) null;
                    }
                    else
                    {
                        string errmsg = $"SQL Error encountered : The error message was \n{Err}\n\nPerhaps a  different Execution method would work more effectively for this Stored.Procedure.?";
                        Err = errmsg;
                        return ( dynamic ) null;
                    }
                }
                else if ( operationtype == "SP returning No value" )
                {
                    DatagridControl dgc = new ( );

                    //********************************************************************************//
                    var result = dgc . ExecuteDapperTextCommand ( ListResults . SelectedItem . ToString ( ) , args , out Err );
                    //********************************************************************************//

                    if ( Err != "" )
                        ShowError ( operationtype , Err );
                    else
                    {
                       // string argstring = SPArguments . Text == "Argument(s) required ?" ? "" : SPArguments . Text;
                       // GenResults = new ( this , this . Topmost );
                       // GenResults . ExecutionInfo . Visibility = Visibility . Visible;
                       // GenResults . CollectionTextresults . Visibility = Visibility . Visible;
                       // GenResults . CollectionTextresults . Document = Gengrid . LoadFlowDoc ( GenResults . CollectionTextresults ,
                       //FindResource ( "Black3" ) as SolidColorBrush ,
                       // $"The enquiry [{ListResults . SelectedItem . ToString ( )} {argstring}] did not respond with any return values.\n\nPerhaps using a different Execution method will resullt in a better result ??" );
                       // GenResults . ExecutionInfo . Text = $"Execution of [{optype . SelectedItem?.ToString ( )}] completed but no value was returned...";
                       // // Showing Scrollviewer Text Only, so reduce height
                       // // Squeeze unused row so buttons show in our 220 height
                       // GenResults . Height = 280;
                       // GenResults . innerresultscontainer . RowDefinitions [ 1 ] . Height = new GridLength ( 1 , GridUnitType . Pixel );
                       // GenResults . CountResult . Text = $"ZERO";
                       // GenResults . Show ( );
                       // GenResults . Refresh ( );
                       // NewWpfDev . Utils . PlayErrorBeep ( );
                       // return -9;
                    }
                }
                else
                {
                    MessageBox . Show ( "You MUST select one of these options to proceed....." , "Selection Error" );
                    return -9;
                }
            }
            catch ( Exception ex )
            {
                Utils . DoErrorBeep ( );
                Debug . WriteLine ( $"Execute_Click ERROR : \n{ex . Message}\n{ex . Data}" );
            }
            return ( dynamic ) null;
        }

        public static string CleanArgs ( string [ ] args )
        {
            int bufferindex = 0;
            int newargindex = 0;
            int alldone = 0;
            string output = "";
            string [ ] newarg = new string [ 4 ];
            string [ ] buffer = new string [ args . Length * 8 ];
            try
            {
                for ( int x = 0 ; x < args . Length ; x++ )
                {
                    if ( args [ x ] . Contains ( ' ' ) )
                    {
                        string [ ] item = args [ x ] . Split ( ' ' );
                        for ( int y = 0 ; y < item . Length ; y++ )
                        {
                            buffer [ bufferindex++ ] = item [ y ] . Trim ( );
                        }
                        continue;
                    }
                }
                for ( int x = 0 ; x < buffer . Length ; x++ )
                {
                    if ( buffer [ x ] == null )
                        break;
                    if ( buffer [ x ] . Contains ( "=" ) )
                        buffer [ x ] = null;
                    newargindex = x;
                }
                for ( int x = 0 ; x < newargindex ; x++ )
                {
                    if ( buffer [ x ] == null )
                        break;
                    if ( buffer [ x ] . Contains ( "," ) )
                        buffer [ x ] = null;
                    newargindex = x;
                }
                int offset = 0;
                for ( int x = 0 ; x < newargindex ; x++ )
                {
                    if ( buffer [ offset ] == null )
                        break;
                    if ( output == "" )
                        output += $"{buffer [ offset++ ]}={buffer [ offset++ ]}";
                    else
                    {
                        output += $", {buffer [ offset++ ]}";
                        if ( buffer [ offset++ ] != null && buffer [ offset++ ] . Length > 0 )
                            output += $"={buffer [ offset ]}";
                    }
                    //buffer [ x ] = null;
                    //newargindex = x;
                }
            }
            catch ( Exception ex )
            {
                //Debug . WriteLine ( $"{ex . Message}" );
            }
            //output = buffer;
            return output;
        }
        public string ExecuteSelectedStoredproc ( string spname , string Searchtext )
        {
            //Show popup optype selection dialog
            //string resulltstring = "";
            //string [ ] args1;
            //if ( Searchtext != null )
            //{
            //    args1 = Searchtext . Trim ( ) . Split ( ',' );
            //    int count = args1 . Length;
            //    string [ ] args = new string [ count ];
            //}
            //createoptypes ( );
            //optype . UpdateLayout ( );
            return "";
        }
        public void createoptypes ( )
        {
            optype . Items . Add ( $"SP returning an INT value" );
            optype . Items . Add ( $"SP returning a String" );
            optype . Items . Add ( $"SP returning a List<string>" );
            optype . Items . Add ( $"SP returning a Table as ObservableCollection" );
            optype . Items . Add ( $"SP returning a 'Pot Luck' result" );
            optype . Items . Add ( $"SP returning No value" );
        }

        private string ReturnProcedureHeader ( string commandline , string Arguments )
        {
            //*********************************//
            // only called  by Resultsviewer
            //*********************************//
            Parameters . Text = StoredProcs . SProcsDataHandling . GetSpHeaderBlock ( Arguments );
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
            if ( SPArguments . Text == "Argument(s) required ?" )
                SPArguments . Text = "";
        }
        private void SPArguments_GotFocus ( object sender , RoutedEventArgs e )
        {
            if ( SPArguments . Text == "Argument(s) required ?" )
                SPArguments . Text = "";
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
            DoExecute_Click ( null , null );
        }
        private void Exec_Click ( object sender , RoutedEventArgs e )
        {
            Gengrid . RunExecute_Click ( this );
            ProcessExecResults ( );
        }
        public void ProcessExecResults ( )
        {
            if ( ExecResults . resultInt != 0 )
            { }
            if ( ExecResults . resultString != "" )
            { }
            if ( ExecResults . resultDouble != 0.0 )
            { }
            if ( ExecResults . resultCollection != null )
            { }
            if ( ExecResults . resultStringList . Count > 0 )
            { }

        }
        private void ListResults_MouseRightButtonDown ( object sender , MouseButtonEventArgs e )
        {
            ContextMenu cm = FindResource ( "ResultsViewerContextMenu" ) as ContextMenu;
            Point pt = e . GetPosition ( sender as UIElement );
            // Hide relevant entries
            List<string> hideitems = new List<string> ( );
            if ( ShowingAllSPs == false )
            {   // Show show all as we showing matches only
                hideitems . Add ( "gm1" );
            }
            else
            {   // hide show all as we already are showing all
                hideitems . Add ( "gm2" );
            }
            //hideitems . Add ( "gm3" );
            //hideitems . Add ( "gm4" );
            // Hide close tables viewer as it is not open
            hideitems . Add ( "gm5" );
            //hideitems . Add ( "gm6" );
            //hideitems . Add ( "gm7" );
            //hideitems . Add ( "gm8" );

            ContextMenu menu = RemoveMenuItems ( "ResultsViewerContextMenu" , "" , hideitems );
            //forces menu to show immeduiately to right and below mouse pointer
            menu . PlacementTarget = sender as FrameworkElement;
            menu . PlacementRectangle = new Rect ( pt . X , pt . Y , 250 , 100 );
            e . Handled = true;
            menu . IsOpen = true;
        }
        private void Spresultsviewer_Closing ( object sender , System . ComponentModel . CancelEventArgs e )
        {
            Genericgrid . Resultsviewer = null;
            MainWindow . SaveSystemSetting ( "SpResultsViewerOnTop" , OntopCheck . IsChecked );
        }
        private void OntopCheck_Click ( object sender , RoutedEventArgs e )
        {
            CheckBox cb = sender as CheckBox; ;
            if ( cb . IsChecked == true )
            {
                zOrder = 100;
                Spresultsviewer . Topmost = true;
            }
            else
            {
                zOrder = 0;
                Spresultsviewer . Topmost = false;
            }
        }
        private void ShowingAllSps_Checked ( object sender , RoutedEventArgs e )
        {
            // Checkbox clicked to change SP's shown
            // toggle flag and load ALL SP's

            CheckBox cb = sender as CheckBox;
            if ( ShowingAllSPs == false )
            {
                ShowingAllSprocs . Content = $"Show ONLY Stored Procedures that contain [{Searchtext}].";
                ShowingAllSprocs . UpdateLayout ( );
            }
            else
            {
                ShowingAllSprocs . Content = $"Show ALL Stored Procedures.";
                ShowingAllSprocs . UpdateLayout ( );

                // LOAD ALL (or matching) SP'S depending on flag status
                // call stup method required because method is private
                LoadAllSps ( this , ListResults . SelectedIndex );
            }
            e . Handled = true;

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
            string srchterm = "";
            srchterm = Searchtext;
            string curritem = "";
            int currindex = 0;
            if ( ListResults . SelectedIndex != -1 )
                currindex = ListResults . SelectedIndex;
            if ( ListResults . SelectedItem != null )
                curritem = ListResults . SelectedItem . ToString ( );
            else
                curritem = ListResults . Items [ 0 ] . ToString ( );
            SpResultsViewer Target = sender as SpResultsViewer;

            // We get back here after loading matching SP's ?????????????????????????'
            if ( ShowingAllSPs == true )
            {
                ShowingAllSprocs . UpdateLayout ( );
                // Update cosmetics
                if ( Gengrid . LoadShowMatchingSproc ( this , TextResult , curritem , ref srchterm ) == false )
                {
                    Debug . WriteLine ( $"Failed  to load SP's" );
                    NewWpfDev . Utils . PlayErrorBeep ( );
                    return;
                }
                else
                {
                    // Update  checkbox prompt
                    ShowingAllSprocs . Content = $"Show ALL available Stored Procedures .";
                    ShowingAllSprocs . UpdateLayout ( );
                    // reset flag as we are now showing matches only
                    ShowingAllSPs = true;
                    UsingMatches = true;
                    // Load SP into ScrollViewer
                    Gengrid . SetSpWindowInfoText ( this , this , Gengrid . Searchtext );
                }
            }
            else
            {
                ShowingAllSprocs . UpdateLayout ( );
                // Update cosmetics
                if ( Gengrid . LoadShowMatchingSproc ( this , TextResult , curritem , ref srchterm ) == false )
                {
                    Debug . WriteLine ( $"Failed  to load SP's" );
                    NewWpfDev . Utils . PlayErrorBeep ( );
                    ShowingAllSPs = true;
                    return;
                }
                else
                {
                    ShowingAllSprocs . Content = $"Show ONLY Stored Procedures matching [{Searchtext}].";
                    ShowingAllSprocs . UpdateLayout ( );
                    // Load SP into ScrollViewer
                    Gengrid . SetSpWindowInfoText ( this , this , srchterm );
                    ShowingAllSPs = true;
                }
            }
            // sort out the layout, but pass  a blank search term as we are loading ALL SP's
            Mouse . OverrideCursor = Cursors . Arrow;
        }
        private void Spresultsviewer_Loaded ( object sender , RoutedEventArgs e )
        {
            if ( ShowingAllSPs == false )
                ShowingAllSPs = true;
            //           ShowingAllSprocs . IsChecked = true;
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
                ShowArgumentsHelp sh = new ShowArgumentsHelp ( );
                sh . Show ( );
                sh . ArgInfo . UpdateLayout ( );
                sh . Topmost = true;
                e . Handled = true;
            }
            else if ( e . Key == Key . F2 )
            {
                //IsFlashing = true;
                GengridExecutionResults sh = new GengridExecutionResults ( this , false );
                sh . Show ( );
                e . Handled = true;
                //IsFlashing = true;
            }
            else if ( e . Key == Key . F3 )
            {
                SpArguments sh = new SpArguments ( SPArguments );
                sh . SPHeaderblock . Text = StoredProcs . SProcsDataHandling . GetSpHeaderBlock ( Gengrid . SpTextBuffer );

                if ( sh . SPHeaderblock . Text != "" )
                    sh . Show ( );
                e . Handled = true;
            }
        }
        private void ListResults_MouseDoubleClick ( object sender , MouseButtonEventArgs e )
        {
            SpArguments sh = new SpArguments ( SPArguments );
            sh . SPHeaderblock . Text = StoredProcs . SProcsDataHandling . GetSpHeaderBlock ( Gengrid . SpTextBuffer );
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
        private void Viewer_PreviewMouseRightButtonDown ( object sender , MouseButtonEventArgs e )
        {
            ContextMenu cm = FindResource ( "ResultsViewerContextMenu" ) as ContextMenu;
            // Hide relevant entries
            List<string> hideitems = new List<string> ( );
            hideitems . Add ( "gm2" );
            hideitems . Add ( "gm6" );
            hideitems . Add ( "gm7" );

            ContextMenu menu = RemoveMenuItems ( "ResultsViewerContextMenu" , "" , hideitems );
            //forces menu to show immeduiately to right and below mouse pointer
            menu . PlacementTarget = sender as FrameworkElement;
            Point pt = e . GetPosition ( sender as UIElement );
            menu . PlacementRectangle = new Rect ( pt . X , pt . Y , 350 , 300 );
            menu . IsOpen = true;
            e . Handled = true;
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
                foreach ( var delitem in delItems )
                {
                    foreach ( MenuItem menuitem in menu . Items )
                    {
                        //var v = mi . Items;
                        if ( menuitem . Name == delitem )
                            menuitem . Visibility = Visibility . Visible;
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
            string buffer = StoredProcs . SProcsDataHandling . GetSpHeaderBlock ( Gengrid . SpTextBuffer );
            AllArgs . Items . Clear ( );
            string [ ] items;
            string [ ] lines = buffer . Split ( '\n' );
            foreach ( var line in lines )
            {
                if ( line . Contains ( "CREATE PROC" ) == false )
                    Output += StoredProcs . SProcsDataHandling . GetBareSProcHeader ( line , ListResults . SelectedItem . ToString ( ) , out bool success );
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

        private void ShowMatchingSprocs ( object sender , RoutedEventArgs e )
        {

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

        private void ListResults_MouseRightButtonUp ( object sender , MouseButtonEventArgs e )
        {

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

        private void detailsviewer_LButtonDn ( object sender , MouseButtonEventArgs e )
        {
        }

        private void detailsviewer_KeyDown ( object sender , KeyEventArgs e )
        {
            if ( e . Key == Key . Escape )
            {
                DetailedArgsViewer . Visibility = Visibility . Collapsed;
                e . Handled = true;
            }
        }
    }
}

