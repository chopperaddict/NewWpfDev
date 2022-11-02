using System;
using System . Collections . Generic;
using System . Text;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Data;
using System . Windows . Documents;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Media . Imaging;
using System . Windows . Shapes;

namespace Views
{
    /// <summary>
    /// Interaction logic for GengridExecutionResults.xaml
    /// </summary>
    public partial class GengridExecutionResults : Window
    {
        SpResultsViewer spResultsViewer;
        public GengridExecutionResults ( SpResultsViewer  parent)
        {
            InitializeComponent ( );
            spResultsViewer = parent;
            WpfLib1 . Utils . SetupWindowDrag ( this );

        }
        private void showExecuteresults_Click ( object sender , RoutedEventArgs e )
        {
            //spResultsViewer . Visibility = Visibility . Collapsed;
            spResultsViewer . OperationSelection . Visibility = Visibility . Collapsed;
            this . Close ( );
        }

        private void TextResult_PreviewMouseRightButtonDown ( object sender , MouseButtonEventArgs e )
        {
        }

        private void Arguments_KeyDown ( object sender , KeyEventArgs e )
        {
        }
    }
}
