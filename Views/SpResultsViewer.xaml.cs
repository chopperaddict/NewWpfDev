using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Configuration;
using System . Diagnostics;
using System . Linq;
using System . Reflection . PortableExecutable;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Documents;
using System . Windows . Input;
using System . Windows . Media;

using Azure . Core;

using Microsoft . VisualBasic;
using Microsoft . Xaml . Behaviors . Media;

using Newtonsoft . Json;

using NewWpfDev;
using NewWpfDev . UserControls;

using UserControls;

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
        public static FlowDocScrollViewerSupport FdSupport { get; set; }
        public static string [ ] arguments = new string [ 10 ];
        GengridExecutionResults GenResults { get; set; }

        ObservableCollection<GenericClass> DataGrid = new ObservableCollection<GenericClass> ( );

        public struct ExecutionResults
        {
            public int resultInt { get; set; }
            public string resultString { get; set; }
            public List<string> resultStringList { get; set; }
            public double resultDouble { get; set; }
            public ObservableCollection<GenericClass> resultCollection { get; set; }
        }
        public static ExecutionResults ExecResults;

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

        #region Dependecy properties

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
            Genericgrid . Resultsviewer = this;
            string spname = Gengrid . SpName . Text;
            Searchtext = spname;
            string [ ] args = new string [ 1 ];
            Mouse . OverrideCursor = Cursors . Wait;
            canvas . Visibility = Visibility . Visible;
            // setup a pointer to ourselves
            this . Topmost = true;
            Gengrid . ExecuteLoaded = true;
            hspltter . Cursor = Cursors . ScrollNS;
            //ExecResults = new ExecutionResults ( );
            //GengridExecutionResults ge = new GengridExecutionResults ( this , true );
            //ge . CollectionShortTextresults . Text = "dsggga dgg g g ag e ey bvc bcvn 54 7 3747 bvm  utr ";
            //ge . Show ( );

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

                //Reset arguments panel
                SPArguments . Text = "Argument(s) required ?";
                optype . SelectedIndex = -1;    // unselect selection of S.P listbox
                optype . SelectedItem = null;    // unselect method listbox
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
            List<string> results = DatagridControl . ProcessUniversalQueryStoredProcedure ( spname , args , Genericgrid . CurrentTableDomain , out string err );
            //            List<string> processResults = new List<string> ( );
            if ( results . Count > 0 )
            {
                //TextResult . Visibility = Visibility . Visible;
                //ListResults . Visibility = Visibility . Visible;
                //SpViewerResults . Visibility = Visibility . Visible;

                //string stringresult = "";
                //    Task . Run( ( ) =>
                //{
                ///              string stringresult = "";
                Task task = Task . Run ( async ( ) =>
                {
                    await fetchandloadAllSProcs ( results );
                    //                    sptext = Gengrid . FetchStoredProcedureCode ( Splist . SelectedItem . ToString ( ) , ref stringresult );
                } );
                // string line = "";
                // string sptext = "";
                //foreach ( string item in results )
                //{
                //   line = item . ToString ( );
                //    Gengrid . FetchStoredProcedureCode ( line , ref sptext );
                //    processResults . Add ( sptext );
                //}
                //List<string> reslt = processResults . Where (
                //matchtext => sptext . ToUpper ( ) . Contains ( Gengrid . selectSp . Text . ToUpper ( ) )
                //) . ToList ( );
                //ListResults . Items . Clear ( );
                //if ( reslt . Count > 0 )
                //{
                //    foreach ( string item in results )
                //    {
                //        ListResults . Items . Add ( item );
                //    }
                //    ListResults . SelectedIndex = 0;
                //     FlowDocument fd = new FlowDocument ( );
                //    fd . Blocks . Clear ( );

                //    // Store search term in our dialog for easier access
                //    SrchTerm . Text = Gengrid . selectSp . Text . ToUpper ( );

                //    fd = Gengrid . CreateBoldString ( fd , sptext , Genericgrid . SpSearchTerm . ToUpper ( ) );
                //    fd . Background = FindResource ( "Black3" ) as SolidColorBrush;
                //    TextResult . Document = fd;
                //    //RTBox . Document = myFlowDocument;
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
                return;
            }
            if ( dynvar == null || objtype == null )
            {
                WpfLib1 . Utils . DoErrorBeep ( );
                MessageBox . Show ( $"Your request failed with no error information being returned.\nTry a different Execution Method" , "SQL Error" );
            }
            if ( objtype == typeof ( Int32 ) )
            {
                testint = Convert . ToInt32 ( obj );
                newtype = objtype;
            }
            else if ( objtype == typeof ( string ) )
            {
                if ( objtype != null )
                    resultstring = ( string ) obj?.ToString ( );
                newtype = objtype;
            }
            else if ( objtype == typeof ( ObservableCollection<GenericClass> ) )
            {
                newtype = objtype;
            }
            else
            {
                // setup results dialog, but hide it
                newtype = objtype;
            }
            //GengridExecutionResults
            GenResults = new GengridExecutionResults ( this , this . Topmost );
            GenResults . CollectionGridresults . Visibility = Visibility . Collapsed;
            GenResults . CollectionTextresults . Visibility = Visibility . Collapsed;
            GenResults . ExecutionInfo . Visibility = Visibility . Visible;
            if ( err != "" )
            {
                GenResults . CollectionTextresults . Document =
                    Gengrid . LoadFlowDoc ( GenResults . CollectionTextresults ,
                     FindResource ( "Black3" ) as SolidColorBrush ,
                        $"Execution of S.P [{ListResults . SelectedItem . ToString ( )}] Encountered an SQL error.\n\n{err}" );
                GenResults . ExecutionInfo . Text = $"Execution of [ {optype . SelectedItem . ToString ( )} ] completed , but  with errors !!!!";
                GenResults . Show ( );
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
                        return;
                    //-------------------------------------------------------------------------------------------------//
                    if ( newtype == typeof ( string ) )
                    {
                        // return value from SP String return method
                        // Go   ahead & Show our results  dialog popup
                        if ( obj . ToString ( ) . Length == 0 )
                        {
                            GenResults . ExecutionInfo . Visibility = Visibility . Visible;
                            GenResults . innerresultscontainer . RowDefinitions [ 0 ] . Height = new GridLength ( 3.9 , GridUnitType . Star );
                            GenResults . CollectionTextresults . Document =
                                Gengrid . LoadFlowDoc ( GenResults . CollectionTextresults ,
                                 FindResource ( "Black3" ) as SolidColorBrush ,
                            $"Execution of S.P [ {ListResults . SelectedItem . ToString ( )} ] appears to have failed !\n\nThe data returned was {resultstring}.\n\nPerhaps using a different Execution Method will provide a better result ?" );
                            NewWpfDev . Utils . DoSuccessBeep ( );
                            GenResults . ShowDialog ( );
                            GenResults . Refresh ( );
                            Mouse . OverrideCursor = Cursors . Arrow;
                        }
                        else if ( obj . ToString ( ) . Length < 100 )
                        {
                            GenResults . innerresultscontainer . RowDefinitions [ 0 ] . Height = new GridLength ( 3.9 , GridUnitType . Star );
                            // show short result in TextBlock viewer
                            GenResults . CollectionTextresults . Visibility = Visibility . Visible;
                            GenResults . CollectionTextresults . Document =
                                Gengrid . LoadFlowDoc ( GenResults . CollectionTextresults ,
                                 FindResource ( "Black3" ) as SolidColorBrush ,
                                    $"Execution of S.P [{ListResults . SelectedItem . ToString ( )}] \ncompleted successfully. The string returned was [ \"{obj . ToString ( )}\" ]" );
                            GenResults . ExecutionInfo . Text = $"Execution of [ {optype . SelectedItem . ToString ( )} ] as string completed successfully, details shown above...";
                            NewWpfDev . Utils . DoSuccessBeep ( );
                            GenResults . ShowDialog ( );
                            GenResults . Refresh ( );
                            Mouse . OverrideCursor = Cursors . Arrow;
                        }
                        else
                        {
                            // show long result in Scrollviewer Document
                            GenResults . CollectionTextresults . Document = Gengrid . LoadFlowDoc ( GenResults . CollectionTextresults ,
                         FindResource ( "Black3" ) as SolidColorBrush ,
                          $"Execution of S.P [ {ListResults . SelectedItem . ToString ( )} ] \ncompleted successfully. The execution call and parameters were as shown below.\n\n{resultstring}" );
                        }
                        Mouse . OverrideCursor = Cursors . Arrow;
                        return;
                    }
                    //-------------------------------------------------------------------------------------------------//
                    else if ( newtype == typeof ( Int32 ) )
                    {
                        // Got an INT value in returned (ref value count ) variable
                        string argstring = SPArguments . Text == "Argument(s) required ?" ? "" : SPArguments . Text;
                        GenResults . CollectionTextresults . Visibility = Visibility . Visible;
                        GenResults . CollectionTextresults . Document =
                            Gengrid . LoadFlowDoc ( GenResults . CollectionTextresults ,
                             FindResource ( "Black3" ) as SolidColorBrush ,
                        $"The enquiry [ {ListResults . SelectedItem . ToString ( )} {argstring} ] \nreturned an Int return value of {count}..." );
                        GenResults . ExecutionInfo . Text = $"Execution of [ {optype . SelectedItem . ToString ( )} ] \ncompleted successfully. The details are shown above...";
                        // Squeeze unused row so buttons show in our 220 height
                        GenResults . Height = 220;
                        GenResults . innerresultscontainer . RowDefinitions [ 1 ] . Height = new GridLength ( 1 , GridUnitType . Pixel );
                        GenResults . UpdateLayout ( );
                        GenResults . Refresh ( );
                        NewWpfDev . Utils . DoSuccessBeep ( );
                        GenResults . ShowDialog ( );
                        Mouse . OverrideCursor = Cursors . Arrow;
                        return;
                    }
                    //-------------------------------------------------------------------------------------------------//
                    else if ( newtype . ToString ( ) . Contains ( "List" ) )
                    {
                        // Got a List<string>
                        // Go   ahead & Show our results  dialog popup
                        ObservableCollection<GenericClass> gengrid = new ObservableCollection<GenericClass> ( );
                        if ( dynvar . Count > 0 )
                        {
                            List<string> list = new List<string> ( );
                            foreach ( var item in dynvar )
                            {
                                list . Add ( item . ToString ( ) );
                            }
                            // got a list with at least one item/Row ??
                            //foreach ( string item in list )
                            //{
                            //    GenericClass gc = new GenericClass ( );
                            //    gc . field1 = item . ToString ( );
                            //    if ( gc != null )
                            //        gengrid . Add ( gc );
                            //}
                        }
                        if ( gengrid . Count > 0 )
                        {
                            GenResults . CollectionListboxresults . Items . Clear ( );
                            GenResults . CollectionListboxresults . ItemsSource = null;

                            int colcount = 0;
                            //colcount = DatagridControl . GetColumnsCount ( gengrid );
                            //DatagridControl . LoadActiveRowsOnlyInGrid ( GenResults . CollectionGridresults , gengrid , colcount );
                            GenResults . CollectionListboxresults . Visibility = Visibility . Visible;
                            GenResults . CollectionTextresults . Document = Gengrid . LoadFlowDoc ( GenResults . CollectionTextresults ,
                         FindResource ( "Black3" ) as SolidColorBrush ,
                           $"Execution of [ {optype . SelectedItem . ToString ( )} ] completed successfully. \nThe successfully with {gengrid . Count} items returned. \nThe requested information is shown below... " );
                            // Showing Scrollviewer Text AND DataGrid, so we stay full height
                            // Squeeze unused row so buttons show in our 220 height
                            GenResults . Height = 580;
                            NewWpfDev . Utils . DoSuccessBeep ( );
                            GenResults . ShowDialog ( );
                            GenResults . Refresh ( );
                            Mouse . OverrideCursor = Cursors . Arrow;
                        }
                        else
                        {
                            if ( resultstring != null && resultstring . Contains ( "System.Collections.Generic.List" ) == false )
                            {
                                string argstring = SPArguments . Text == "Argument(s) required ?" ? "" : SPArguments . Text;
                                GenResults . CollectionGridresults . ItemsSource = dynvar;
                                GenResults . CollectionTextresults . Document = Gengrid . LoadFlowDoc ( GenResults . CollectionTextresults ,
                             FindResource ( "Black3" ) as SolidColorBrush ,
                                    $"The enquiry [ {ListResults . SelectedItem . ToString ( )} {argstring} ] returned [ {resultstring} ]" );
                                GenResults . ExecutionInfo . Text = $"Execution of [ {optype . SelectedItem . ToString ( )} ]\ncompleted successfully, details shown above...";
                                // Showing Scrollviewer Text Only, so reduce height
                                // Squeeze unused row so buttons show in our 220 height
                                GenResults . Height = 280;
                                GenResults . innerresultscontainer . RowDefinitions [ 1 ] . Height = new GridLength ( 1 , GridUnitType . Pixel );
                                NewWpfDev . Utils . DoSuccessBeep ( );
                                GenResults . ShowDialog ( );
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
                                   $"The enquiry [ {ListResults . SelectedItem . ToString ( )} {argstring} ] responded with the following values\n\n[ {data} ]" );
                                GenResults . ExecutionInfo . Text = $"Execution of [{optype . SelectedItem . ToString ( )}] completed with the list containing the data shown above!";
                                // Showing Scrollviewer Text Only, so reduce height
                                // Squeeze unused row so buttons show in our 220 height
                                GenResults . Height = 280;
                                GenResults . innerresultscontainer . RowDefinitions [ 1 ] . Height = new GridLength ( 1 , GridUnitType . Pixel );
                                NewWpfDev . Utils . PlayErrorBeep ( );
                                GenResults . ShowDialog ( );
                                GenResults . Refresh ( );
                            }
                        }
                    }
                    //-------------------------------------------------------------------------------------------------//
                    else if ( newtype . ToString ( ) . ToUpper ( ) . Contains ( "OBSERVABLECOLLECTION" ) == true )
                    {
                        // got an observable collection
                        if ( count > 0 )
                        {
                            GenResults . CollectionTextresults . Visibility = Visibility . Visible;
                            GenResults . CollectionGridresults . Visibility = Visibility . Visible;
                            string argstring = SPArguments . Text == "Argument(s) required ?" ? "" : SPArguments . Text;
                            GenResults . CollectionGridresults . ItemsSource = dynvar;
                            GenResults . CollectionTextresults . Document = Gengrid . LoadFlowDoc ( GenResults . CollectionTextresults ,
                        FindResource ( "Black3" ) as SolidColorBrush ,
                                $"Execution Result : SUCCESS\n\nThe enquiry [ {ListResults . SelectedItem . ToString ( )} {argstring} ] \nwas processed successfully and returned a value of [ {count} ] records..." );
                            GenResults . ExecutionInfo . Text = $"Execution of [ {optype . SelectedItem . ToString ( )} ]\ncompleted successfully, details shown above...";
                            // Showing Scrollviewer Text Only, so reduce height
                            // Squeeze unused row so buttons show in our 220 height
                            //GenResults . Height = 280;
                            //GenResults . innerresultscontainer . RowDefinitions [ 1 ] . Height = new GridLength ( 1 , GridUnitType . Pixel );
                            NewWpfDev . Utils . DoSuccessBeep ( );
                            GenResults . ShowDialog ( );
                            GenResults . Refresh ( );
                        }
                        else if ( newtype . ToString ( ) != "" )
                        {
                            if ( newtype . Name . Contains ( "ObservableCollection" ) )
                            {
                                ObservableCollection<GenericClass> resultcollection = new ObservableCollection<GenericClass> ( );
                                resultcollection = NewWpfDev . Utils . CopyCollection ( dynvar , resultcollection );
                                if ( resultcollection . Count > 0 )
                                {
                                    string argstring = SPArguments . Text == "Argument(s) required ?" ? "" : SPArguments . Text;
                                    GenResults . CollectionTextresults . Visibility = Visibility . Visible;
                                    GenResults . CollectionGridresults . ItemsSource = resultcollection;
                                    GenResults . CollectionTextresults . Document = Gengrid . LoadFlowDoc ( GenResults . CollectionTextresults ,
                                    FindResource ( "Black3" ) as SolidColorBrush ,
                                           $"The enquiry [ {optype . SelectedItem . ToString ( )} {argstring} ] \nreturned a DataGrid as shown below" );
                                    GenResults . ExecutionInfo . Text = $"Execution of [ {optype . SelectedItem . ToString ( )} ] completed successfully, details shown above...";
                                    GenResults . CollectionGridresults . Visibility = Visibility . Visible;
                                    NewWpfDev . Utils . DoSuccessBeep ( );
                                    GenResults . ShowDialog ( );
                                    GenResults . Refresh ( );
                                }
                                else
                                {
                                    NewWpfDev . Utils . PlayErrorBeep ( );
                                }
                                Mouse . OverrideCursor = Cursors . Arrow;
                            }
                        }
                        Mouse . OverrideCursor = Cursors . Arrow;
                    }
                    else
                    {
                        // Unknown result
                        string argstring = SPArguments . Text == "Argument(s) required ?" ? "" : SPArguments . Text;
                        GenResults . CollectionTextresults . Visibility = Visibility . Visible;
                        GenResults . CollectionTextresults . Document = Gengrid . LoadFlowDoc ( GenResults . CollectionTextresults ,
                     FindResource ( "Black3" ) as SolidColorBrush ,
                   $"The enquiry [ {ListResults . SelectedItem . ToString ( )} {argstring} ] \ncompleted successfully but failed to return any type of value ...\n\nTry a different Method of Execution !" );
                        GenResults . ExecutionInfo . Text = $"Execution of [ {optype . SelectedItem . ToString ( )} ] completed successfully, but see notes above...";
                        // Showing Scrollviewer Text Only, so reduce height
                        // Squeeze unused row so buttons show in our 220 height
                        GenResults . Height = 280;
                        GenResults . innerresultscontainer . RowDefinitions [ 1 ] . Height = new GridLength ( 1 , GridUnitType . Pixel );
                        NewWpfDev . Utils . PlayErrorBeep ( );
                        GenResults . ShowDialog ( );
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
                NewWpfDev . Utils . PlayErrorBeep ( );
                Debug . WriteLine ( $"SQL error encountered ...\n {ex . Message}, [{ex . Data}]" );
            }
            Mouse . OverrideCursor = Cursors . Arrow;
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
            optype . Items . Add ( $"SP returning a Table as Collection<GenericTable>" );
            optype . Items . Add ( $"SP returning a 'Pot Luck' result" );
            optype . Items . Add ( $"SP returning No value" );
        }

        private string ReturnProcedureHeader ( string commandline , string Arguments )
        {
            //*********************************//
            // only called  by Resultsviewer
            //*********************************//
            Parameters . Text = GetSpHeaderBlock ( Arguments );
            DetailInfo . Visibility = Visibility . Visible;
            operationtype3 . Text = $"Stored Procedure {commandline . ToUpper ( )} Header Details :-\n\n{Parameters . Text}";
            return "Done";
        }
        /// <summary>
        /// Method to strip out the header block of any SP
        /// </summary>
        /// <param name="Arguments"></param>
        public string GetSpHeaderBlock ( string Arguments )
        {
            string output = "";
            string arguments = Arguments;
            int argcount = 0;
            string header = "";
            string buffer = "";
            string temp = Arguments;
          temp = temp . Trim ( ) . TrimStart ( );
            Arguments = temp . ToUpper ( );
            int count = -1;
            count = Arguments . IndexOf ( "CREATE PROCEDURE" );
            if ( count != -1 )
                buffer = Arguments . Substring ( count );
            string [ ] strings = new string [0];

            if ( buffer . ToUpper ( ) . Contains ( "PROCEDURE" ) )
            {
                header = buffer;

            }
            strings = header . ToUpper ( ) . Split ( "AS" );
            string [ ] parts = strings [ 0 ] . Split ( "\r\n" );
            argcount = 0;
            foreach ( var item in parts )
            {
                if ( argcount == 0 && item.Contains("PROCEDURE"))
                {
                    output = $"{item . Trim ( )}\n";
                    argcount++;
                    continue;
                }
                if ( item.Trim() == "" )
                    continue;
                if ( item . ToUpper ( ) . Contains ( "\r\nBEGIN" ) )
                    break;
                else
                {
                    if ( item . ToUpper ( ) . Contains ( "OUTPUT" ) )
                    {
                        if ( item . Contains ( '\t' ) )
                            output += $" Output : [{item . Trim ( ) . Substring ( 2 )}]";
                        else
                            output += $" Output : [{item . Trim ( )}]\n";
                    }
                    else
                    {
                        if ( argcount >= 1 )
                        {
                            if ( item . TrimStart ( ) . StartsWith ( "--" ) )
                            {
                                argcount++;
                                continue;
                            }
                            if ( item . Contains ( "--" ) )
                            {
                                string [ ] tmp = item . Split ( "--" );
                                if ( tmp . Length > 1 )
                                {
                                    tmp [ 0 ] = StripTabs ( tmp [ 0 ] );
                                    output += tmp [ 0 ];
                                }
                            }
                            else
                            {
                               string  tmp = StripTabs ( item );
                                output += $" Input : [{tmp. Trim ( )}]\n";
                            }
                        }
                    }
                }
                argcount++;
            }
            return output;
        }

        public string StripTabs ( string input )
        {
            string output = "";
            int offset = -1;
            string [ ] test = new string [ 0 ];
            if ( input . Contains ( '\t' ) )
            {
                offset = input . IndexOf ( '\t' );
                test = input . Split ( '\t' );
                for (int x = 0 ; x < test.Length ; x++ )
                {
                    if ( test [ x ] . StartsWith ( '\t' ) )
                        test [ x ] = $" {test [x] + 1}";
                    output += test [ x ];
                }
            }
            return output;
        }
        // called by Stub DoExecute_Click ( object sender , RoutedEventArgs e )
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

            // Now find out what method we are going to use
            if ( operationtype == "SP returning an INT value" )
            {
                string SqlCommand = $"{ListResults . SelectedItem . ToString ( )}";
                // Need to pass method=0 to a query as we are using an SP. to return an INT value
                SqlCommand = $"spgetcolscount";
                // tell method what we are expecting back
                Objtype = typeof ( int );
                //********************************************************************************//
                dynamic stringresult = GenDapperQueries . Get_DynamicValue_ViaDapper ( SqlCommand ,
                    args ,
                    ref innerresultstring ,
                    ref innerobj ,
                    ref Objtype ,
                    ref innercount ,
                    ref Err ,
                    0 );
                //********************************************************************************//

                if ( stringresult != null )
                {
                    // TODO Maybe wrong  8/11/2022
                    ResultString = innerresultstring;
                    Obj = ( object ) stringresult;
                    Objtype = typeof ( Int32 );
                    Count = innercount;
                    return ( dynamic ) innerobj;
                }
                if ( Err != "" && innerresultstring == "" )
                {
                    if ( ReturnProcedureHeader ( SqlCommand , ListResults . SelectedItem . ToString ( ) ) == "DONE" )
                        return ( dynamic ) null;
                    ShowError ( operationtype , Err );
                    return ( dynamic ) null;
                }
            }
            else if ( operationtype == "SP returning a String" )
            {
                //Use storedprocedure  version
                string SqlCommand = $"{ListResults . SelectedItem . ToString ( )}";

                // tell method what we are expecting back
                Objtype = typeof ( string );

                //********************************************************************************//
                dynamic stringresult = GenDapperQueries . Get_DynamicValue_ViaDapper ( SqlCommand , args , ref innerresultstring , ref innerobj , ref Objtype , ref innercount , ref Err , 2 );
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
                DatagridControl dgc = new ( );
                List<string> list = new List<string> ( );

                // tell method what we are expecting back
                Objtype = typeof ( List<string> );

                //********************************************************************************//
                list = StoredprocsProcessing . ProcessGenericDapperStoredProcedure (
                     ListResults . SelectedItem . ToString ( ) ,
                    args ,
                    Genericgrid . CurrentTableDomain ,
                    ref innerresultstring ,
                    ref innerobj ,
                    ref Objtype ,
                    ref innercount ,
                    ref Err );
                //********************************************************************************//

                ResultString = innerresultstring;
                Obj = ( object ) list;
                Objtype = typeof ( List<string> );
                Count = list . Count;

                if ( Objtype == typeof ( List<string> ) )
                    return ( dynamic ) list;
                else
                    return ( dynamic ) null;

                //if ( list . Count > 0 )
                //    return ( dynamic ) list;
                //else
                //    return ( dynamic ) null;
            }
            else if ( operationtype == "SP returning a Table as Collection<GenericTable>" )
            {
                DatagridControl dgc = new ( );

                // tell method what we are expecting back
                Objtype = typeof ( ObservableCollection<GenericClass> );

                //********************************************************************************//
                // Should be  '[spLoadTableAsGeneric]'
                dynamic tableresult = GenDapperQueries . Get_DynamicValue_ViaDapper (
                    ListResults . SelectedItem . ToString ( ) ,
                    args ,
                    ref innerresultstring ,
                    ref innerobj ,
                    ref Objtype ,
                    ref innercount ,
                    ref Err ,
                    4 );

                ResultString = innerresultstring;
                Obj = ( object ) innerobj;
                Objtype = typeof ( ObservableCollection<GenericClass> );
                Count = innercount;

                if ( Objtype == typeof ( ObservableCollection<GenericClass> ) )
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
            else if ( operationtype == "SP returning a 'Pot Luck' result" )
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
                    int returnedcount = rescollection . Count;
                    if ( returnedcount > 0 )
                    {
                        string output = $"Results of the {optype . SelectedItem . ToString ( )} request is shown below\n\n";
                        foreach ( var item in rescollection )
                        {
                            output += $"{item . field1 . ToString ( )}\n";
                        }
                        Obj = output;
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
                    var result = dg . GetDataFromStoredProcedure ( "Select columncount from countreturnvalue" , null , "" , out Err , out recordcount , 1 );
                    //********************************************************************************//

                    if ( result . Count == 0 )
                        MessageBox . Show ( $"No Error was encountered,  but the request did NOT return any type of value...\n\nPerhaps the processing method that you selected as shown below :-\n" +
                            $"[{optype . SelectedItem . ToString ( ) . ToUpper ( )}]\n was not the correct processing method type for this Stored.Procedure ?" , "SQL Error" );
                    if ( ReturnProcedureHeader ( "Select columncount from countreturnvalue" , "" ) == "DONE" )
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
                    string argstring = SPArguments . Text == "Argument(s) required ?" ? "" : SPArguments . Text;
                    GenResults = new ( this , this . Topmost );
                    GenResults . ExecutionInfo . Visibility = Visibility . Visible;
                    GenResults . CollectionTextresults . Document = Gengrid . LoadFlowDoc ( GenResults . CollectionTextresults ,
                   FindResource ( "Black3" ) as SolidColorBrush ,
                    $"The enquiry [ {ListResults . SelectedItem . ToString ( )} {argstring} ] did not respond with any return values.\n\nPerhaps using a different Execution method will resullt in a better result ??" );
                    GenResults . ExecutionInfo . Text = $"Execution of [ {optype . SelectedItem?.ToString ( )} ] completed but no value was returned...";
                    GenResults . Show ( );
                    GenResults . Refresh ( );
                    NewWpfDev . Utils . PlayErrorBeep ( );
                }
            }
            else
            {
                MessageBox . Show ( "You MUST select one of these options to proceed....." , "Selection Error" );
                return -9;
            }
            return ( dynamic ) null;
        }

        //private void Hidepanel_Click ( object sender , RoutedEventArgs e )
        //{
        //    OperationSelection . Visibility = Visibility . Collapsed;
        //}
        private void ShowError ( string optype , string err )
        {
            if ( err != "" )
                MessageBox . Show ( $"Error encountered .....error message was \n{err . ToUpper ( )}\n\nPerhaps the method that you selected as shown below :-\n" +
                    $"[{optype . ToUpper ( )}]\n was not the correct processing method type for this Stored.Procedure.\n\n" +
                    $"The help window just opened shows you the parameter types required by this S.P?" , "SQL Error" );
            else
                MessageBox . Show ( $"No Error was encountered,  but the request did NOT return any type of value...\n\n" +
                    $"Perhaps the processing method that you selected as shown below :-\n[{optype . ToUpper ( )}]\n" +
                    $"The help window just opened shows you the parameter types required by this S.P?" , "SQL Error" );

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
            // call a  context menu to refresh list of SP's

            ListBox lb = sender as ListBox;
            string srchtext = Gengrid . Searchtext;
            int currindex = lb . SelectedIndex;
            if ( currindex < 0 )
                currindex = 0;
            LoadSpList ( this , currindex , srchtext );
            e . Handled = true;
            lb . SelectedIndex = currindex;
        }

        private void Spresultsviewer_Closing ( object sender , System . ComponentModel . CancelEventArgs e )
        {
            Genericgrid . Resultsviewer = null;
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
            this . Cursor = Cursors . ScrollNS;
        }

        private void hSplitter_MouseLeave ( object sender , MouseEventArgs e )
        {
            this . Cursor = Cursors . Arrow;
        }
        private void Hsplitter_MouseMove ( object sender , MouseEventArgs e )
        {
            this . Cursor = Cursors . ScrollNS;
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
                GengridExecutionResults sh = new GengridExecutionResults ( this , false );
                sh . Show ( );
                e . Handled = true;
            }
            else if ( e . Key == Key . F3 )
            {
                SpArguments sh = new SpArguments ( SPArguments );
                sh . SPHeaderblock . Text = GetSpHeaderBlock ( Gengrid . SpTextBuffer );
                sh . Show ( );
                e . Handled = true;
            }
        }

    }

}

