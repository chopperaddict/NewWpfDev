using System;
using System . Collections . Generic;
using System . ComponentModel;
using System . IO;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Data;
using System . Windows . Documents;
using System . Windows . Input;
using System . Windows . Interop;
using System . Windows . Media;
using System . Windows . Media . Imaging;
using System . Windows . Shapes;

namespace NewWpfDev. Views
{
    /// <summary>
    /// Interaction logic for TabViewInfo.xaml
    /// </summary>
    public partial class TabViewInfo : Window, System . ComponentModel . INotifyPropertyChanged
    {

        private string tabbedWindowText;

        #region OnPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged ( string propertyName )
        {
            if ( PropertyChanged != null )
            {
                PropertyChanged ( this , new PropertyChangedEventArgs ( propertyName ) );
            }
        }
        #endregion OnPropertyChanged

        public string TabbedWindowText
        {
            get { return tabbedWindowText; }
            set
            {
                tabbedWindowText = value;
                NotifyPropertyChanged ( nameof ( TabbedWindowText ) );
            }
        }
        protected void CloseControl(object sender, RoutedEventArgs e)
        {
            //CustCtrl . Visibility = Visibility.Collapsed;
        }
        public TabViewInfo (string datafile= "TabbedDataInfo.txt" )
        {
            InitializeComponent ( );
            this . DataContext = this;
            if ( File . Exists ( datafile ) == false )
            {
                MessageBox . Show ( $"Sorry, but the Information file [{datafile}] cannot be found" , "Missing information file" );
                ShowOption ( null , null );
                return;
            }

            TabbedWindowText = File . ReadAllText ( datafile );
            if ( TabbedWindowText . Length == 0 )
                MessageBox . Show ( "Could  not find the source text file...." , "Missing Data" );
            ShowOption ( null , null );
        }

        private void Closewin ( object sender , RoutedEventArgs e )
        {
            this . Close ( );
        }

        private void ShowOption ( object sender , RoutedEventArgs e )
        {
            List<string> filelist = new List<string> ( );
            string [ ] files;
            string tmp = "";
            files = Directory . GetFiles ( "." );
            foreach ( var item in files )
            {
                if ( item . ToUpper ( ) . Contains ( ".TXT" ) )
                {
                    if ( item . Contains ( ".\\" ) )
                        tmp = item . Substring ( 2 );
                    else tmp = item;
                    filelist . Add ( tmp );
                }
            }

            DataFiles . ItemsSource = filelist;
            DataFiles . SelectedIndex = 0;
            DataFiles . SelectedItem = 0;

        }

        private void DataFiles_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {
            TabbedWindowText = "";
            dataBlock.UpdateLayout ( );
            TabbedWindowText = File . ReadAllText ( $"{DataFiles.SelectedItem}" );
            dataBlock . UpdateLayout ( );
        }
    }
}
