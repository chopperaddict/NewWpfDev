using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . Text;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Data;
using System . Windows . Documents;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Media . Imaging;
using System . Windows . Shapes;

using static Azure . Core . HttpHeader;

namespace NewWpfDev
{
    /// <summary>
    /// Interaction logic for ColumnSelectionWiindow.xaml
    /// </summary>
    public partial class ColumnSelectionWindow : Window
    {
//        public List<string> columns;
            GenericClass selColumns = new GenericClass ( );
            ObservableCollection<GenericClass> collection = new ObservableCollection<GenericClass> ( );
        public ColumnSelectionWindow ()
        {
            InitializeComponent ( );
            // Create list of selected items
            //foreach ( string item in columnsrecvd )
            //{
            //    GenericClass tem = new GenericClass ( );
            //    tem . field1 = item; ;
            //    selColumns . field1 = tem . field1;
            //    collection . Add ( tem );
            //}
            //ColNames . ItemsSource = collection;
        }

        private void GoBtn_Click ( object sender , RoutedEventArgs e )
        {
              this . Close ( );
        }

        private void stopBtn_Click ( object sender , RoutedEventArgs e )
        {
            collection.Clear ();
            collection = null;
            this . Close ( );  
        }

        private void Window_Loaded ( object sender , RoutedEventArgs e )
        {
            int x = 0;
        }
    }
}
