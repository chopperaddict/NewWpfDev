using System;
using System . Collections;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Diagnostics;
using System . Text;
using System . Threading;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Data;
using System . Windows . Documents;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Media . Imaging;
using System . Windows . Shapes;
using NewWpfDev . StoredProcs;
using Microsoft . Xaml . Behaviors . Layout;

using NewWpfDev;

using UserControls;
using UtilityWindows;
using SProcsProcessing;
using System . Windows . Controls . Primitives;
using Microsoft . Identity . Client;

using System . Data . SqlTypes;

namespace Views
{
    /// <summary>
    /// part of  GenDapperQueries (Partial class - 3 parts)
    /// </summary>
    public partial class GengridExecutionResults : Window
    {
        SpResultsViewer spResultsViewer;

        public bool IsHostTopmost { get; set; }
        public bool IsFlashing { get; set; }
        public bool IsUnknownError { get; set; } = false;
        BackgroundWorker worker { get; set; }
        public GengridExecutionResults ( SpResultsViewer parent , bool istopmost )
        {
            InitializeComponent ( );
            spResultsViewer = parent;
            WpfLib1 . Utils . SetupWindowDrag ( this );
            IsHostTopmost = istopmost;
        }
        private void ExecuteResults_Loaded ( object sender , RoutedEventArgs e )
        {
            this . Show ( );
            worker = new BackgroundWorker ( );
            worker . WorkerReportsProgress = true;
            worker . DoWork += worker_DoWork;
            worker . ProgressChanged += worker_ProgressChanged;
            worker . RunWorkerCompleted += worker_RunWorkerCompleted;
            worker . RunWorkerAsync ( 1000 );
        }
        private void worker_RunWorkerCompleted ( object sender , RunWorkerCompletedEventArgs e )
        {

        }

        private void showExecuteresults_Click ( object sender , RoutedEventArgs e )
        {
            if ( IsHostTopmost == true )
            {
                if ( this . Topmost == true )
                    this . Topmost = false;
            }
            // stop the flashing
            IsFlashing = false;
            this . Close ( );
        }

        private void worker_DoWork ( object sender , DoWorkEventArgs e )
        {
            IsFlashing = true;
            Debug . WriteLine ( $"*******************************************Starting Task ************************************************" );
            while ( IsFlashing )
            {
                Dispatcher . BeginInvoke ( new Action ( ( ) =>
                {

                    if ( IsUnknownError == true )
                    {
                        CountResult . Background = FindResource ( "Red3" ) as SolidColorBrush;
                        CountResult . UpdateLayout ( );
                        CountResult . Foreground = FindResource ( "White0" ) as SolidColorBrush;
                    }
                    else
                    {
                        CountResult . Background = FindResource ( "Green5" ) as SolidColorBrush;
                        CountResult . UpdateLayout ( );
                        CountResult . Foreground = FindResource ( "Red4" ) as SolidColorBrush;
                    }
                    CountResult . FontWeight = FontWeights . Normal;
                    CountResult . UpdateLayout ( );
                    CountResult . Refresh ( );
                } ) );
                Thread . Sleep ( 500 );

                Dispatcher . BeginInvoke ( new Action ( ( ) =>
                {
                    CountResult . Background = FindResource ( "Yellow0" ) as SolidColorBrush;
                    CountResult . UpdateLayout ( );
                    CountResult . Foreground = FindResource ( "Black1" ) as SolidColorBrush;
                    CountResult . FontWeight = FontWeights . Bold;
                    CountResult . UpdateLayout ( );
                    CountResult . Refresh ( );
                } ) );
                Thread . Sleep ( 500 );
            }
            // reset coloring  flag
            IsUnknownError = false;
        }
        private void worker_ProgressChanged ( object sender , ProgressChangedEventArgs e )
        {
            //while ( true )
            //{
            //    Thread . Sleep ( 750 );
            //    CountResult . Background = FindResource ( "Blue4" ) as SolidColorBrush;
            //    CountResult . Foreground = FindResource ( "White0" ) as SolidColorBrush;
            //    CountResult . UpdateLayout ( );
            //    Thread . Sleep ( 750 );
            //    CountResult . Background = FindResource ( "Green5" ) as SolidColorBrush;
            //    CountResult . Foreground = FindResource ( "Red4" ) as SolidColorBrush;
            //    CountResult . UpdateLayout ( );
            //}
        }

        private void TextResult_PreviewMouseRightButtonDown ( object sender , MouseButtonEventArgs e )
        {
        }

        private void Arguments_KeyDown ( object sender , KeyEventArgs e )
        {
        }

        private void ExecuteResults_KeyDown ( object sender , KeyEventArgs e )
        {
            if ( e . Key == Key . F2 )
            {
                worker = new BackgroundWorker ( );
                worker . WorkerReportsProgress = true;
                worker . DoWork += worker_DoWork;
                worker . ProgressChanged += worker_ProgressChanged;
                worker . RunWorkerCompleted += worker_RunWorkerCompleted;
                worker . RunWorkerAsync ( 1000 );
            }
        }

        private void CollectionGridresults_PreviewMouseRightButtonDown ( object sender , MouseButtonEventArgs e )
        {
            // Create report of schema for selected table
            //Mouse . OverrideCursor = Cursors . Wait;
            //var menu = FindResource ( "ExecutionMenu" ) as ContextMenu;
            //menu . IsOpen = true;
            return;
        }

        private void ShowTableSchemaFromDatagrid ( object sender , RoutedEventArgs e )
        {
            string outputstring = "";
            string lines = "";
            string table = "";
            string tname = "";
            int index = 0;
            GengridExecutionResults ger = this;
            DataGrid dgrid = ger . CollectionGridresults as DataGrid;
            GenericClass gc = new GenericClass ( );
            Type type = sender . GetType ( );
            if ( type != typeof ( DataGrid ) )
            {
                Mouse . OverrideCursor = Cursors . Arrow;
                MessageBox . Show ( $"You cannot access column\ninformation from a ListBox display.\n\nTry again using the ObservableCllection Execution Method ? \n\n" +
                    $"Please reselect only columns from a single table & try again." , "Show  Schema Details ?" , MessageBoxButton . OK );
                return;
            }
            //selstrings . Add ( gc );
            index++;

            Mouse . OverrideCursor = Cursors . Wait;
            // create List of all selected GenericClass objects
            List<GenericClass> selstrings = new List<GenericClass> ( );
            string tabname = "";
            foreach ( var item in dgrid . SelectedItems )
            {
                gc = item as GenericClass;
                if ( index == 0 )
                    tabname = gc . field1;

                if ( tabname != gc . field1
                || ( index >= 0
                    && ( gc . field2 == null
                    || gc . field2 == ""
                    || gc . field2 . Length < 2 )
                    ) )
                {
                    // got  2 fields,but they do not look to be correct ......
                    //index++;
                    Mouse . OverrideCursor = Cursors . Arrow;
                    MessageBox . Show ( $"You do not appear to have selected Rows containing only column\ninformation from the selected SQL table {gc . field1 . ToUpper ( )}\n\n" +
                        $"Please reselect only columns from a single table & try again." , "Show  Schema Details ?" , MessageBoxButton . OK );
                    return;
                }
                selstrings . Add ( gc );
                index++;
            }


            List<string> fullschema = new List<string> ( );
            lines = "";
            foreach ( var item in selstrings )
            {
                if ( gc . field1 != "" )
                    lines += $"{gc . field1}";
                if ( gc . field2 != "" )
                    lines += $",{gc . field2}";

                fullschema . Add ( lines );
            }
            lines = "";
            index = 0;
            if ( selstrings . Count >= 1 )
            {
                // Get full columns info for selected table
                string line = fullschema [ index++ ];
                table = line . Substring ( 0 , line . IndexOf ( "," ) );

                // create arguments for sql enquiry
                string sprocname = "spGetTableColumnsSchema";
                List<string [ ]> argsbuffer = new List<string [ ]> ( );
                string resultstring = "";
                object obj = null;
                Type Objtype = null;
                int count = 0; ;
                string error = "";
                string [ ] args = new string [ 2 ];
                args [ 0 ] = "@Arg1";
                args [ 1 ] = table;
                argsbuffer . Add ( args );
                try
                {
                    // Get Table results in dynamic collection
                    dynamic result = GenDapperQueries . Get_DynamicValue_ViaDapper (
                        sprocname ,
                        argsbuffer ,
                        ref resultstring ,
                        ref obj ,
                        ref Objtype ,
                        ref count ,
                        ref error ,
                        0 );

                    // Parse dynamic table results
                    int colcount = 0;
                    ObservableCollection<GenericClass> genclass = new ObservableCollection<GenericClass> ( );

                    genclass = GenDapperQueries . ParseDynamicToCollection (
                            result ,
                            out string errormsg ,
                            out int reccount ,
                            out List<string> genericlist );

                    // Create text report of table schema ready for display
                    outputstring = SpProcessingSupport . CreateSchemaReportText ( genclass );
                    // Finally Show the formatted results
                    DataViewer dv = new DataViewer ( );
                    dv . ShowDataViewer ( outputstring , IsExecutionHelp: false );
                    dv . Show ( );
                    Mouse . OverrideCursor = Cursors . Arrow;
                }
                catch ( Exception ex )
                {
                    Debug . WriteLine ( $"Processing error : [{ex . Message}]" );
                }
            }
            Mouse . OverrideCursor = Cursors . Arrow;
            return;
        }


        private void ShowTableSchemaDetails ( object sender , RoutedEventArgs e )
        {
            //Display Full Schema in DataViewer

        }

        private void ShowContextMenu ( object sender , MouseButtonEventArgs e )
        {
            Mouse . OverrideCursor = Cursors . Arrow;
            //        ContextMenu menu = FindResource ( "ExecutionMenu" ) as ContextMenu;
            //        menu . PlacementTarget = sender as FrameworkElement;

            ////        menu . IsOpen = true;
        }

        private void ContextMenu_Closed ( object sender , RoutedEventArgs e )
        {

        }

        private void ContextMenu_Opened ( object sender , RoutedEventArgs e )
        {

        }

        private void ShowSpHeader ( object sender , RoutedEventArgs e )
        {

        }

        private void ShowTableRowAsList ( object sender , RoutedEventArgs e )
        {

        }

    }
}

