using System;
using System . Collections . Generic;
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

using Microsoft . Xaml . Behaviors . Layout;

using NewWpfDev;

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
           this .Close ( );
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
                 } ));
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

    }
}

