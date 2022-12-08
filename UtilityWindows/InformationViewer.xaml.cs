using System;
using System . Collections . Generic;
using System . ComponentModel;
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
    /// Interaction logic for InformationViewer.xaml
    /// </summary>
    public partial class InformationViewer : Window
    {
          public InformationViewer ( )
        {
            InitializeComponent ( );
            this . DataContext = this;
        }

        private void Window_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
        {
        }
       #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged ( string property )
        {
            if ( PropertyChanged != null )
                PropertyChanged ( this , new PropertyChangedEventArgs ( property ) );
        }
        #endregion PropertyChanged

        #region full properties
            private string message1;
        private string message2;
        private string message3;
        // public static CustomMessageBox Instance ;
        public string Message1
        {
            get { return message1; }
            set { message1 = value; RaisePropertyChanged ( "Message1" ); }
        }
        public string Message2
        {
            get { return message1; }
            set { message2 = value; RaisePropertyChanged ( "Message2" ); }
        }
        public string Message3
        {
            get { return message1; }
            set { message3 = value; RaisePropertyChanged ( "Message3" ); }
        }
        #endregion full properties

        public void Show ( MsgBoxArgs args )
        {
            //string spaces1 = "                                                                                   \n";
            //string spaces2 = "                                               \n";
            //Thickness th = new ( );
            //th . Left = 0;
            //th . Top = 0;
            //th . Right = 0;
            //th . Bottom = 0;
            //Title . Text = args . title;
            //Msg1 . Text = args . msg1;

            //if ( args . msg2 . Length > 0 )
            //    Msg1 . Text += $".\n\n{args . msg2}";
            //// pad text out vertically
            //if ( args . msg1 . Length < 300 )
            //    th . Top = 15;
            //else if ( args . msg1 . Length < 200 )
            //    th . Top = 20;
            //else if ( args . msg1 . Length < 1 )
            //    th . Top = 25;
            //Msg1 . Margin = th;

            this . Show ( );
        }

        private void BtnOk ( object sender , RoutedEventArgs e )
        {
            this . Close ( );
        }

        private void BtnCancel ( object sender , RoutedEventArgs e )
        {
            this . Close ( );
        }

        private void BtnCloseapp ( object sender , RoutedEventArgs e )
        {
            Application . Current . Shutdown ( );
        }

        private void Window_Loaded ( object sender , RoutedEventArgs e )
        {
            if ( Msg1 . Text . Length > 150 )
            {
                this . Height += 80;
                this . Width += 100;
            }
            else if ( Msg1 . Text . Length > 100 )
            {
                this . Height += 80;
                this . Width += 80;
            }
            else if ( Msg1 . Text . Length > 50 )
            {
                this . Height += 60;
                this . Width += 60;
            }
            e . Handled = true;
        }

        private void Window_SizeChanged ( object sender , SizeChangedEventArgs e )
        {

        }
    }
}
