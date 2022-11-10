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
     /// part of  GenDapperQueries (Partial class - 3 parts)
    /// </summary>
    public partial class GengridExecutionResults : Window
    {
        SpResultsViewer spResultsViewer;

        public bool IsHostTopmost { get; set; }
        public GengridExecutionResults ( SpResultsViewer  parent, bool istopmost)
        {
            InitializeComponent ( );
            spResultsViewer = parent;
            WpfLib1 . Utils . SetupWindowDrag ( this );
            IsHostTopmost = istopmost;
        }
        private void showExecuteresults_Click ( object sender , RoutedEventArgs e )
        {
            if ( IsHostTopmost == true )
            {
                if( this . Topmost == true)
                this . Topmost = false;
            }
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
