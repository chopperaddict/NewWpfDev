using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . Diagnostics;
using System . Linq;
using System . Reflection . Metadata . Ecma335;
using System . Runtime . InteropServices;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Documents;
using System . Windows . Input;
using System . Windows . Media;
using System . Xml . Linq;

using Dapper;

using DocumentFormat . OpenXml . Drawing . Charts;
using DocumentFormat . OpenXml . Wordprocessing;

using Microsoft . Win32;

using NewWpfDev;

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


        public SpResultsViewer ( Genericgrid genControl , string sproc , string searchterm )
        {
            InitializeComponent ( );
            WpfLib1 . Utils . SetupWindowDrag ( this );
            Gengrid = genControl;
            //nResults = Gengrid.
            FdSupport = new FlowDocScrollViewerSupport ( );

            string spname = Gengrid . SpName . Text;
            Searchtext = spname;
            string [ ] args = new string [ 1 ];
            //Store search term for use  by later dialogs
            //            Genericgrid . SpSearchTerm = Gengrid . selectSp . Text . ToUpper ( );
            //            SrchTerm . Text = Gengrid . selectSp . Text . ToUpper ( );
            //            args [ 0 ] = SrchTerm . Text;
            Mouse . OverrideCursor = Cursors . Wait;
            canvas . Visibility = Visibility . Visible;
            //            LoadShowData ( sproc,args);
            return;
        }
        public void LoadSpList (int selindex, string srchtext )
        {
            int indx = 0;
            ListResults . ItemsSource = null;
            ListResults . Items . Clear ( );
            Gengrid . LoadMatchingStoredProcs ( ListResults , srchtext );
            // load our listbox from parents list
            foreach (var item in ListResults . Items )
            {
                if ( indx== selindex )
                { ListResults . SelectedIndex = selindex; break; }
                indx++;
            }
            Mouse . OverrideCursor = Cursors.Arrow;
        }

        public void LoadShowData ( string spName , string [ ] args )
        {
            string line = "", sptext = "", currentselection = "";
            if ( ListResults . Items . Count == 0 )
            {
                Mouse . OverrideCursor = Cursors . Wait;
                Debug . WriteLine ( $"Executing S.P {spName}" );
                //string [ ] args = new string [ 1 ];
                //args [ 0 ] = Genericgrid . SpSearchTerm;
                currentselection = spName;
                List<string> results = DatagridControl . ProcessUniversalQueryStoredProcedure ( spName , args , Genericgrid . CurrentTableDomain , out string err );
                List<string> processResults = new List<string> ( );
                ListResults . Items . Clear ( );
                foreach ( string item in results )
                {
                    line = item . ToString ( );
                    sptext = Gengrid . FetchStoredProcedureCode ( line );
                    if ( sptext . ToUpper ( ) . Contains ( SrchTerm . Text . ToUpper ( ) ) )
                        ListResults . Items . Add ( item );
                    //                  processResults . Add ( sptext );
                }

                if ( ListResults . Items . Count > 0 )
                {
                    ListResults . SelectedIndex = 0;
                    FlowDocument fd = new FlowDocument ( );
                    sptext = Gengrid . FetchStoredProcedureCode ( ListResults . Items [ 0 ] . ToString ( ) );
                    fd . Blocks . Clear ( );
                    fd = Gengrid . CreateBoldString ( fd , sptext , SrchTerm . Text . ToUpper ( ) );
                    fd . Background = FindResource ( "Black3" ) as SolidColorBrush;
                    Block block = fd . Blocks . FirstBlock;
                    TextResult . Document = fd;
                }
                createoptypes ( );
                optype . UpdateLayout ( );
            }
            SPArguments . Text = SrchTerm . Text . ToUpper ( );
            Mouse . OverrideCursor = Cursors . Arrow;
        }


        private void ListResults_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {
            string selname = "";
            ListBox lb = sender as ListBox;
            if ( lb . SelectedItem != null )
            {
                selname = lb . SelectedItem . ToString ( );
                // Store search term in our dialog for easier access

                //SrchTerm . Text = selname;
                if ( TextResult . Document != null )
                {
                    TextResult . Document . Blocks . Clear ( );
                    TextResult . Document = null;
                }

                string sptext = Gengrid . FetchStoredProcedureCode ( selname );
                FlowDocument fd = new FlowDocument ( );
                fd = Gengrid . CreateBoldString ( fd , sptext , Genericgrid . SpSearchTerm );
                if ( TextResult . Document == null )
                {
                    fd . Background = FindResource ( "Black3" ) as SolidColorBrush;
                    TextResult . Document = fd;
                    TextResult . Refresh ( );
                }
                FlowDocScrollViewerSupport . OnFindCommand ( TextResult , fd , Searchtext );

                //Reset arguments panel
                SPArguments . Text = "Argument(s) required ?";
                optype . SelectedIndex = -1;    // unselect selection of S.P listbox
                optype . SelectedItem = null;    // unselect method listbox

                //double offset = ExtractStyleChanges ( fd , Genericgrid . SpSearchTerm );
                ////Scroll viewer to matching word position
                //FlowDocumentScrollViewer fdsv = TextResult;
                //ScrollViewer sv = TextResult . Template . FindName ( "PART_ContentHost" , ( FrameworkElement ) TextResult ) as ScrollViewer;
                //if ( sv != null )
                //{
                //    sv . ScrollToBottom ( );
                //    sv . ScrollToTop( );
                //    sv . ScrollToVerticalOffset ( offset -15);
                //}
            }
        }


        private void TextResult_PreviewMouseRightButtonDown ( object sender , MouseButtonEventArgs e )
        {
        }
        private void Execsp_Click ( object sender , RoutedEventArgs e )
        {


            string spname = Gengrid . SpName . Text;
            string [ ] args = new string [ 1 ];
            args [ 0 ] = Genericgrid . SpSearchTerm;
            //Store search term for use  by later dialogs
            Genericgrid . SpSearchTerm = Gengrid . selectSp . Text . ToUpper ( );
            // string err = "";
            //Execsp . Visibility = Visibility . Collapsed;
            Debug . WriteLine ( $"Executing S.P {spname}" );
            //dgControl.ExecuteDapperCommand ( spname , args , out string err );
            List<string> results = DatagridControl . ProcessUniversalQueryStoredProcedure ( spname , args , Genericgrid . CurrentTableDomain , out string err );
            List<string> processResults = new List<string> ( );
            if ( results . Count > 0 )
            {
                //TextResult . Visibility = Visibility . Visible;
                //ListResults . Visibility = Visibility . Visible;
                //SpViewerResults . Visibility = Visibility . Visible;
                string line = "";
                string sptext = "";

                foreach ( string item in results )
                {
                    line = item . ToString ( );
                    sptext = Gengrid . FetchStoredProcedureCode ( line );
                    processResults . Add ( sptext );
                }
                List<string> reslt = processResults . Where (
                matchtext => sptext . ToUpper ( ) . Contains ( Gengrid . selectSp . Text . ToUpper ( ) )
                ) . ToList ( );
                ListResults . Items . Clear ( );
                if ( reslt . Count > 0 )
                {
                    foreach ( string item in results )
                    {
                        ListResults . Items . Add ( item );
                    }
                    ListResults . SelectedIndex = 0;
                    // ListResults . Refresh ( );
                    //TextResult . Text = reslt [ 0 ];
                    //                    TextResult . Document = null;
                    FlowDocument fd = new FlowDocument ( );
                    fd . Blocks . Clear ( );

                    // Store search term in our dialog for easier access
                    SrchTerm . Text = Gengrid . selectSp . Text . ToUpper ( );

                    fd = Gengrid . CreateBoldString ( fd , sptext , Genericgrid . SpSearchTerm . ToUpper ( ) );
                    fd . Background = FindResource ( "Black3" ) as SolidColorBrush;
                    TextResult . Document = fd;
                    //RTBox . Document = myFlowDocument;
                }
            }
            e . Handled = true;
        }

        private void closeresultsviewer_Click ( object sender , RoutedEventArgs e )
        {
            this . Close ( );
        }

        private void FieldSelectionGrid_LostFocus ( object sender , RoutedEventArgs e )
        {

        }

        private void SpStrings_KeyDown ( object sender , KeyEventArgs e )
        {

        }

        private void Arguments_KeyDown ( object sender , KeyEventArgs e )
        {
            if ( e . Key == Key . Enter )
                ExecuteSp ( );
        }

        private void ExecuteSp ( )
        {
            //SrchTerm . Text = SPArguments . Text;
            //Genericgrid . SpSearchTerm = Searchtext . Text; ;
            //Execute selected Sp
            //ExecuteSelectedStoredproc ( ListResults . SelectedItem . ToString ( ) , Searchtext );
            //string err = "";
            DoExecute_Click ( null , null );
            // Gengrid . LoadStoredProcedures ( "spReturnSprocscontainingArg" , Searchtext . Text . ToUpper ( ) );
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
            optype . Items . Add ( $"SP returning a List<GenericTable>" );
            optype . Items . Add ( $"SP returning a 'Pot Luck' result" );
            optype . Items . Add ( $"SP returning No value" );
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
            dynamic dynvar = Execute_click ( ref count , out err );
            ///////===========================///////
            string resultstring = dynvar?.ToString ( );
            // Failures will always return NULL
            //if ( dynvar == null && err == "")
            //{
            //    Mouse . OverrideCursor = Cursors . Arrow;
            //    return;
            //}
            // find out what we have  got
            newtype = dynvar? . GetType ( );

            GenResults = new ( this );
            GenResults . CollectionGridresults . Visibility = Visibility . Collapsed;
            GenResults . CollectionTextresults . Visibility = Visibility . Collapsed;
            GenResults . ExecutionInfo . Visibility = Visibility . Collapsed;
            GenResults . CollectionShortTextresults . Visibility = Visibility . Collapsed;
            //var islist = newtype? . Name . Contains ( "List" );
            if ( ( dynvar == null || dynvar == "") && err == "")
            {
                GenResults . CollectionShortTextresults . Visibility = Visibility . Visible;
                GenResults . ExecutionInfo . Visibility = Visibility . Visible;
                GenResults . CollectionShortTextresults . Text = $"The enquiry [ {ListResults . SelectedItem . ToString ( )} {SPArguments . Text} ] \ncompleted successfully but failed to return any type of value ...\n\nTry a different Method of Execution !";
                GenResults . ExecutionInfo . Text = $"Execution of [ {optype . SelectedItem . ToString ( )} ] completed successfully, but see notes above...";
                GenResults . Show ( );
                GenResults . Refresh ( );
                NewWpfDev . Utils . PlayErrorBeep ( );
                Mouse . OverrideCursor = Cursors . Arrow;
                return;
            }
            if (err != "")
            {
                GenResults . CollectionTextresults . Visibility = Visibility . Visible;
                GenResults . ExecutionInfo . Visibility = Visibility . Visible;
                GenResults . CollectionTextresults . Document =
                    Gengrid . LoadFlowDoc ( GenResults . CollectionTextresults ,
                     FindResource ( "Black3" ) as SolidColorBrush ,
                        $"Execution of S.P [{ListResults . SelectedItem . ToString ( )}] Encountered an SQL error.\n\n{err}" );
                GenResults . ExecutionInfo . Text = $"Execution[ {optype . SelectedItem . ToString ( )} ] completed , but  with errors !!!!";
                GenResults . Show ( );
                GenResults . Refresh ( );
                NewWpfDev . Utils . PlayErrorBeep ( );
                Mouse . OverrideCursor = Cursors . Arrow;
                return;
            }
            if ( newtype == typeof ( string ) )
            {
                // return value from SP String return method
                // Go   ahead & Show our results  dialog popup
                GenResults . CollectionGridresults . Visibility = Visibility . Collapsed;
                GenResults . CollectionTextresults . Visibility = Visibility . Collapsed;
                if ( resultstring . Length > 50 )
                {
                    if ( resultstring . Contains ( "SUCCESS" ) || resultstring . Length > 60 )
                    {
                        // show short result in TextBlock viewer
                        GenResults . CollectionTextresults . Visibility = Visibility . Visible;
                        GenResults . ExecutionInfo . Visibility = Visibility . Visible;
                        GenResults . CollectionTextresults . Document =
                            Gengrid . LoadFlowDoc ( GenResults . CollectionTextresults ,
                             FindResource ( "Black3" ) as SolidColorBrush ,
                                $"Execution of S.P [{ListResults . SelectedItem . ToString ( )}] completed successfully\n\nThe results are shown below.\n\n{resultstring}" );
                        GenResults . ExecutionInfo . Text = $"Execution[ {optype . SelectedItem . ToString ( )} ] completed successfully, details shown above...";
                        GenResults . Show ( );
                        GenResults . Refresh ( );
                        NewWpfDev . Utils . DoSuccessBeep ( );
                        Mouse . OverrideCursor = Cursors . Arrow;
                    }
                    else
                    {
                        GenResults . ExecutionInfo . Visibility = Visibility . Visible;
                        GenResults . ExecutionInfo . Text = $"Execution of S.P [ {ListResults . SelectedItem . ToString ( )} ] appears to have failed !\n\nThe data returend was {resultstring}.\n\nPerhaps using a different Execution Method will provide a better result ?";
                        GenResults . Show ( );
                        GenResults . Refresh ( );
                        NewWpfDev . Utils . DoSuccessBeep ( );
                        Mouse . OverrideCursor = Cursors . Arrow;
                    }
                }
                else
                {
                    // show long result in Scrollviewer Document
                    GenResults . CollectionTextresults . Document = Gengrid . LoadFlowDoc ( GenResults . CollectionTextresults ,
                    FindResource ( "Black3" ) as SolidColorBrush ,
                        $"Execution of S.P [ {ListResults . SelectedItem . ToString ( )} ] completed successfully\n\nThe execution call and parameters were as shown below.\n\n{resultstring}" );
                }
                // show short prompt in lower info panel
                //GenResults . ExecutionInfo . Visibility = Visibility . Visible;
                //GenResults . ExecutionInfo . Text = $"Execution[ {optype . SelectedItem . ToString ( )} ] completed successfully, details shown above...";
                //GenResults . Show ( );
                //NewWpfDev . Utils . DoSuccessBeep ( );
                Mouse . OverrideCursor = Cursors . Arrow;
                return;

            }
            else if ( newtype == typeof ( Int32 ) )
            {
                // Go   ahead & Show our results  dialog popup
                GenResults . CollectionShortTextresults . Visibility = Visibility . Visible;
                GenResults . ExecutionInfo . Visibility = Visibility . Visible;
                GenResults . CollectionShortTextresults . Text = $"The enquiry [ {ListResults . SelectedItem . ToString ( )} {SPArguments . Text} ] \\nreturned a the value of{dynvar}...\";";
                GenResults . ExecutionInfo . Text = $"Execution[ {optype . SelectedItem . ToString ( )} ] completed successfully, details shown above...";
                GenResults . Show ( );
                GenResults . Refresh ( );
                NewWpfDev . Utils . DoSuccessBeep ( );
                Mouse . OverrideCursor = Cursors . Arrow;
                return;
            }
            else if ( newtype . ToString ( ) . Contains ( "List" ) )
            {
                // Go   ahead & Show our results  dialog popup
                if ( dynvar . Count > 0 )
                {
                    // got a list with at least one item/Row ??
                    foreach ( var item in dynvar )
                    {
                        GenericClass gc = new GenericClass ( );
                        gc = item as GenericClass;
                        if ( gc != null )
                            GenResults . CollectionGridresults . Items . Add ( gc );
                    }
                }
                if ( GenResults . CollectionGridresults . Items . Count > 0 )
                {
                   GenResults . CollectionGridresults . Visibility = Visibility . Visible;
                    GenResults . ExecutionInfo . Visibility = Visibility . Visible;
                    GenResults . ExecutionInfo . Text = $"Execution[ {optype . SelectedItem . ToString ( )} ] completed successfully, details shown above...";
                    GenResults . Show ( );
                    GenResults . Refresh ( );
                    NewWpfDev . Utils . DoSuccessBeep ( );
                    Mouse . OverrideCursor = Cursors . Arrow;
                }
                else
                {
                    if ( resultstring != null && resultstring . Contains ( "System.Collections.Generic.List" ) == false )
                    {
                        GenResults . CollectionShortTextresults . Visibility = Visibility . Visible;
                        GenResults . ExecutionInfo . Visibility = Visibility . Visible;
                        GenResults . CollectionShortTextresults . Text = $"The enquiry [ {ListResults . SelectedItem . ToString ( )} {SPArguments . Text} ] returned [ {resultstring} ]";
                        GenResults . ExecutionInfo . Text = $"Execution[ {optype . SelectedItem . ToString ( )} ] completed successfully, details shown above...";
                        GenResults . Show ( );
                        GenResults . Refresh ( );
                        NewWpfDev . Utils . DoSuccessBeep ( );
                        Mouse . OverrideCursor = Cursors . Arrow;
                    }
                    else
                    {
                         GenResults . CollectionShortTextresults . Visibility = Visibility . Visible;
                        GenResults . ExecutionInfo . Visibility = Visibility . Visible;
                        GenResults . CollectionShortTextresults . Text = $"The enquiry [ {ListResults . SelectedItem . ToString ( )} {SPArguments . Text} ] responded with the following values\n\n";
                        foreach ( var item in dynvar )
                        {
                            GenResults . CollectionShortTextresults . Text += $"{item . ToString ( )}\n";
                        }
                        GenResults . ExecutionInfo . Text = $"Execution of [{optype . SelectedItem . ToString ( )}] completed with the list containing the data shown above!";
                        GenResults . Show ( );
                        GenResults . Refresh ( );
                        NewWpfDev . Utils . PlayErrorBeep ( );
                    }
                }
                //              NewWpfDev . Utils . DoSuccessBeep ( );
                Mouse . OverrideCursor = Cursors . Arrow;
                return;
            }
            if ( dynvar != null && newtype . ToString ( ) . ToUpper ( ) . Contains ( "ObservableCollection" ) == false )
            {
                if ( count > 0 )
                {
                    GenResults . CollectionShortTextresults . Visibility = Visibility . Visible;
                    GenResults . ExecutionInfo . Visibility = Visibility . Visible;
                    GenResults . CollectionShortTextresults . Text =
                        $"Execution Result : SUCCESS\n\nThe enquiry [ {ListResults . SelectedItem . ToString ( )} {SPArguments . Text} ] \nwas processed successfully and returned a value of [ {count} ]...";
                    GenResults . ExecutionInfo . Text = $"Execution[ {optype . SelectedItem . ToString ( )} ] completed successfully, details shown above...";
                    GenResults . Show ( );
                    GenResults . Refresh ( );
                    NewWpfDev . Utils . DoSuccessBeep ( );
                }
                else if ( newtype . ToString ( ) . ToUpper ( ) . Contains ( "ObservableCollection" ) == true )
                {
                    newtype = dynvar? . GetType ( );
                    string typestring = newtype . Name;
                    if ( typestring . Contains ( "ObservableCollection" ) )
                    {
                        ObservableCollection<GenericClass> resultlist = new ObservableCollection<GenericClass> ( );
                        resultlist = NewWpfDev . Utils . CopyCollection ( dynvar , resultlist );
                        if ( resultlist . Count > 0 )
                        {
                             GenResults . CollectionGridresults . Visibility = Visibility . Visible;
                            GenResults . ExecutionInfo . Visibility = Visibility . Visible;
                            GenResults . CollectionGridresults . ItemsSource = resultlist;
                            GenResults . ExecutionInfo . Text = $"The enquiry [ {optype . SelectedItem . ToString ( )} {SPArguments . Text} ] \nreturned a Generic Collection  containing {dynvar . Count} records...";
                            GenResults . ExecutionInfo . Text = $"Execution[ {optype . SelectedItem . ToString ( )} ] completed successfully, details shown above...";
                            GenResults . Show ( );
                            GenResults . Refresh ( );
                            NewWpfDev . Utils . DoSuccessBeep ( );
                        }
                        else
                        {
                            NewWpfDev . Utils . PlayErrorBeep ( );
                        }
                        Mouse . OverrideCursor = Cursors . Arrow;
                    }
                    else if ( typestring . Contains ( "string" ) )
                    {
                         GenResults . ExecutionInfo . Visibility = Visibility . Visible;
                        GenResults . ExecutionInfo . Text = dynvar . ToString ( );
                        GenResults . Show ( );
                        GenResults . Refresh ( );
                        NewWpfDev . Utils . DoSuccessBeep ( );
                    }
                    else
                    {
                        NewWpfDev . Utils . PlayErrorBeep ( );
                    }
                }
                Mouse . OverrideCursor = Cursors . Arrow;
            }
            Mouse . OverrideCursor = Cursors . Arrow;
        }
        private string ReturnProcedureHeader ( string Arguments )
        {
            string output = "";
            int recordcount = 0;
            string err = "";
            string [ ] args = new string [ 0 ];
            string [ ] outputs = new string [ 0 ];
            List<string> list = new List<string> ( );
            string arguments = Arguments;
            DataGrid . Clear ( );

            DatagridControl . CreateGenericCollection ( ref DataGrid , $"SpGetFullScript" , "" , arguments , "" , "" , ref list , ref err );
            //            dgControl . GetDataFromStoredProcedure ( $"Select * from {Splist. SelectedItem}", args, CurrentTableDomain,out err, out outputs, out recordcount);
            if ( DataGrid . Count > 0 )
            {
                string intro = "";
                string header = "";
                string rest = "";
                string buffer = "";
                GenericClass gc = new GenericClass ( );
                gc = DataGrid [ 0 ];
                buffer = gc . field1 . ToUpper ( );
                string [ ] strings = buffer . Trim ( ) . Split ( "CREATE" );
                foreach ( var item in strings )
                {
                    if ( item . Contains ( "PROCEDURE" ) )
                    {
                        header = item;
                        break;
                    }
                }
                strings = header . Split ( "AS" );
                foreach ( var item in strings )
                {
                    if ( item . Contains ( "\r\nBEGIN" ) )
                        break;
                    else
                        output += item;
                }
                //output = buffer;
            }
            DetailInfo . Visibility = Visibility . Visible;
            Parameters . Text = arguments;
            operationtype3 . Text = $"Selected Stored Procedure Header Details :-\n\n{output}";
            return "Done";
        }

        private dynamic Execute_click ( ref int count , out string err )
        {
            string operationtype = optype . SelectedItem as string;
            // get args
            string [ ] args1;
            string [ ] args = new string [ 0 ];
            string [ ] outputs;
            dynamic dynval = null;
            //          List<string> outputstrings = new List<string> ( );
            //           Dictionary<string , object> dict = new Dictionary<string , object> ( );
            err = "";
            //string error = "";

            if ( operationtype == null )
            {
                MessageBox . Show ( "You MUST select an Execution Method before the selected S.P can be executed !" , "Execution processing error" );
                return null;
            }

            Searchtext = SPArguments . Text;
            if ( Searchtext == "Argument(s) required ?" )
                Searchtext = "";

            // sort out arguments   1st of all
            args1 = Searchtext . Trim ( ) . Split ( ',' );
            int cnt = args1 . Length;
            args = new string [ cnt ];
            string resultstring = "";
            //Check for output args 1st
            for ( int x = 0 ; x < cnt ; x++ )
            {
                args [ x ] = args1 [ x ];
            }
            arguments = args1;

            // Now find out what method we are going to use
            if ( operationtype == "SP returning an INT value" )
            {
                DatagridControl dgc = new ( );
                // Need to pass method=1 as we are using an SP.
                var intresult = dgc . ExecuteDapperCommand ( ListResults . SelectedItem . ToString ( ) , args , out err , 1 );
                if ( intresult == null )
                {
                    MessageBox . Show ( $"the Execution [ {optype . SelectedItem . ToString ( )} ] appears to have failed, please check the parameters entered and the Execution method that was chosen !." , "Execution Error" );
                    return null;
                }
                resultstring = intresult . ToString ( );
                if ( err != "" && resultstring == "" )
                {
                    if ( ReturnProcedureHeader ( ListResults . SelectedItem . ToString ( ) ) == "DONE" )
                        return ( dynamic ) null;
                    ShowError ( operationtype , err );
                }
                else if ( resultstring . Contains ( "SUCCESS" ) )
                {
                    return ( dynamic ) resultstring;
                }
                else
                    return ( dynamic ) null;
            }
            else if ( operationtype == "SP returning a String" )
            {
                dynamic returnvalDynamic = Gengrid . LoadFullSqlTable ( ListResults . SelectedItem . ToString ( ) , args , out err , method: 1 );
                if ( err != "" )
                {
                    if ( ReturnProcedureHeader ( ListResults . SelectedItem . ToString ( ) ) == "DONE" )
                        return ( dynamic ) null;
                    ShowError ( operationtype , err );
                }
                //else
                //{
                //    // I convert it in the previous method   to a string, & then into a dynamic just ot make the retval generic
                //    // so we can get the data using ToString();
                //    string resultstring = returnvalDynamic . ToString ( );
                //    //Display the results dialog
                //    DetailInfo . Visibility = Visibility . Visible;
                //    operationtype3 . Text = resultstring;
                //}
                return returnvalDynamic;
            }
            else if ( operationtype == "SP returning a List<string>" )
            {
                DatagridControl dgc = new ( );
                string error = "";
                List<string> list = new List<string> ( );
                list = DatagridControl . ProcessUniversalQueryStoredProcedure ( ListResults . SelectedItem . ToString ( ) , args , Genericgrid . CurrentTableDomain , out error );
                if ( error != "" )
                {
                    //if ( ReturnProcedureHeader ( ListResults . SelectedItem . ToString ( ) ) == "DONE" )
                    //    return ( dynamic ) null;
                    //ShowError ( operationtype , err );
                    err = error;
                }
                else
                {
                    if ( list . Count > 0 )
                        return ( dynamic ) list;
                    else
                        return ( dynamic ) null;
                }
            }
            else if ( operationtype == "SP returning a List<GenericTable>" )
            {
                DatagridControl dgc = new ( );
                string error = "";
                dynamic returnvalDynamic = DatagridControl . ProcessUniversalQueryStoredProcedure ( ListResults . SelectedItem . ToString ( ) , args , Genericgrid . CurrentTableDomain , out error );
                if ( error != "" )
                {
                    if ( ReturnProcedureHeader ( ListResults . SelectedItem . ToString ( ) ) == "DONE" )
                        return ( dynamic ) null;
                    ShowError ( operationtype , err );
                }
                else return returnvalDynamic;
            }
            else if ( operationtype == "SP returning a 'Pot Luck' result" )
            {
                DatagridControl dgc = new ( );
                int recordcount = 0;
                string error = "", str = "";
                ObservableCollection<GenericClass>
                    rescollection = dgc . GetDataFromStoredProcedure ( ListResults . SelectedItem . ToString ( ) ,
                    args ,
                    Genericgrid . CurrentTableDomain ,
                    out error ,
                    out recordcount );
                if ( error == "" && rescollection . Count > 0 )
                {
                    int returnedcount = rescollection . Count;
                    if ( returnedcount > 0 )
                    {
                        string output = $"Results of the {optype . SelectedItem . ToString ( )} request is shown below\n\n";
                        foreach ( var item in rescollection )
                        {
                            output += $"{item . field1 . ToString ( )}\n";
                        }
                        return ( dynamic ) output;
                    }
                    else
                    {
                        return ( dynamic ) $"No usable values were returned";
                    }
                }
                else if ( rescollection . Count == 0 && error == "" )
                {
                    DatagridControl dg = new DatagridControl ( );
                    var result = dg . GetDataFromStoredProcedure ( "Select columncount from countreturnvalue" , null , "" , out err , out recordcount , 1 );
                    if ( result . Count == 0 )
                        MessageBox . Show ( $"No Error was encountered,  but the request did NOT return any type of value...\n\nPerhaps the processing method that you selected as shown below :-\n" +
                            $"[{optype . SelectedItem . ToString ( ) . ToUpper ( )}]\n was not the correct processing method type for this Stored.Procedure ?" , "SQL Error" );
                    if ( ReturnProcedureHeader ( "" ) == "DONE" )
                        return ( dynamic ) null;
                }
                else
                {
                    string   errmsg = $"SQL Error encountered : The error message was \n{error}\n\nPerhaps a  different Execution method would work more effectively for this Stored.Procedure.?";
                    err = errmsg;
                    return ( dynamic ) null;
                    //ShowError ( operationtype , error );
                 }
            }
            else if ( operationtype == "SP returning No value" )
            {
                DatagridControl dgc = new ( );
                string error = "";
                var result = dgc . ExecuteDapperCommand ( ListResults . SelectedItem . ToString ( ) , args , out error );
                if ( error != "" )
                {
                    //if ( ReturnProcedureHeader ( ListResults . SelectedItem . ToString ( ) ) == "DONE" )
                    //    return ( dynamic ) null;
                    ShowError ( operationtype , err );
                }
                else
                {
                    GenResults = new ( this );
                    GenResults . CollectionShortTextresults . Visibility = Visibility . Visible;
                    GenResults . ExecutionInfo . Visibility = Visibility . Visible;
                    GenResults . CollectionShortTextresults . Text = $"The enquiry [ {ListResults . SelectedItem . ToString ( )} {SPArguments . Text} ] did not respond with any return values.\n\nPerhaps using a different Execution method will resullt in a better result ??";
                    GenResults . ExecutionInfo . Text = $"Execution[ {optype . SelectedItem?.ToString ( )} ] completed but no value was returned...";
                    GenResults . Show ( );
                    GenResults . Refresh ( );
                    NewWpfDev . Utils . PlayErrorBeep ( );
                }
            }
            else
            {
                MessageBox . Show ( "You MUST select one of these options to proceed....." , "Selection Error" );
                return - 9;
            }
            return ( dynamic ) null;
        }

        private void Hidepanel_Click ( object sender , RoutedEventArgs e )
        {
            OperationSelection . Visibility = Visibility . Collapsed;
        }
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
            Execsp_Click ( null , null );
        }

        private void ListResults_MouseRightButtonDown ( object sender , MouseButtonEventArgs e )
        {
            // call a  context menu to refresh list of SP's

            ListBox lb = sender as ListBox;
            int currindex = lb.SelectedIndex;
            LoadSpList (currindex,""  );
            e . Handled = true;
            //lb . SelectedIndex = currindex;
        }
    }
}


