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



using NewWpfDev;
using NewWpfDev . UserControls;

using UserControls;

namespace Views
{
    /// <summary>
    /// Interaction logic for SelectDbWin.xaml
    /// </summary>
    public partial class SelectDbWin : Window
    {
        public Genericgrid gencontrol { get; set; }
        public string CurrentTableDomain { get; set; }
        public ObservableCollection<GenericClass> temp { get; set; }
        public ObservableCollection<GenericClass> output { get; set; }

        public SelectDbWin ( string CurrentTabledomain )
        {
            int colcount = 0;
            InitializeComponent ( );
            gencontrol = Genericgrid . GenControl;
            CurrentTableDomain = Genericgrid . CurrentTableDomain;
            DatagridControl dgctrl = new DatagridControl ( );
        }
        private void Window_Loaded ( object sender , RoutedEventArgs e )
        {
            string err = "";
            CurrentTableDomain = Genericgrid . CurrentTableDomain;
            temp = new ObservableCollection<GenericClass> ( );
            string [ ] args = new string[ 0 ];
            temp = gencontrol . LoadFullSqlTable ( "select * from sysdatabases where YEAR(crdate)>=2021" , args , out err , CurrentTableDomain );
            output = new ObservableCollection<GenericClass> ( );
            if ( temp . Count > 0 )
            {
                Dbgrid . Items . Clear ( );
                foreach ( var item in temp )
                {
                    GenericClass gc = new GenericClass ( );
                    gc . field1 = item . field1;
                    output . Add ( gc );
                }
                Dbgrid . Items . Clear ( );
                foreach ( var item in output )
                {
                    Dbgrid . Items . Add ( item . field1 );
                }
                for ( int x = 0 ; x < Dbgrid . Items . Count ; x++ )
                {
                    ListBoxItem li = Dbgrid . Items [ x ] as ListBoxItem;
                    string s = ( string ) Dbgrid . Items [ x ];
                    if ( s . ToUpper ( ) == CurrentTableDomain . ToUpper ( ) )
                    {
                        Dbgrid . SelectedIndex = x;
                        break;
                    }
                }
            }
        }

        private void CancelBtn_Click ( object sender , RoutedEventArgs e )
        {
            this . Close ( );
        }

        private void SelectBtn_Click ( object sender , RoutedEventArgs e )
        {
            // switch toa diferent database
            int offset = Dbgrid . SelectedIndex;
            DatagridControl . CurrentTableDomain = Genericgrid . CurrentTableDomain = Dbgrid . Items [ offset ] . ToString ( );
            gencontrol . RemoteReloadTables ( );
            this . Close ( );
        }

        private void Dbgrid_MouseDoubleClick ( object sender , MouseButtonEventArgs e )
        {
            Genericgrid . DomainChanged = true;
            SelectBtn_Click ( sender , null );
        }

        private void CancelBtn_MouseEnter ( object sender , MouseEventArgs e )
        {
            Button btn = sender as Button;
            // btn . Background = FindResource ( "Orange0" ) as SolidColorBrush;
        }

        private void OkBtn_MouseEnter ( object sender , MouseEventArgs e )
        {
            Button btn = sender as Button;
            // btn . Background = FindResource ( "Orange0" ) as SolidColorBrush;
        }
    }
}
