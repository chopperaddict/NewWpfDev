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

using DocumentFormat . OpenXml . Wordprocessing;

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
        public SelectDbWin ( string CurrentTableDomain )
        {
            string err = "";
            int colcount = 0;
            InitializeComponent ( );
            gencontrol = Genericgrid.GenControl;
            DatagridControl dgctrl = new DatagridControl ( );
            ObservableCollection<GenericClass> temp = gencontrol . LoadFullSqlTable ( "select * from sysdatabases where YEAR(crdate)>=2021" , out err , CurrentTableDomain );
            ObservableCollection<GenericClass> output = new ObservableCollection<GenericClass> ( );
            if ( temp . Count > 0 )
            {
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
            }
        }

        private void CancelBtn_Click ( object sender , RoutedEventArgs e )
        {
            this . Close ( );
        }

        private void SelectBtn_Click ( object sender , RoutedEventArgs e )
        {
             int offset = Dbgrid . SelectedIndex;
            DatagridControl . CurrentTableDomain = Genericgrid . CurrentTableDomain = Dbgrid . Items [ offset ] . ToString ( );
            gencontrol . RemoteReloadTables ( );
             this . Close ( );
        }
    }
}
