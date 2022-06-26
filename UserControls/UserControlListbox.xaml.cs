using System;
using System . Collections . Generic;
using System . Diagnostics;
using System . IO;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Data;
using System . Windows . Documents;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Media . Imaging;
using System . Windows . Navigation;
using System . Windows . Shapes;

namespace NewWpfDev . UserControls
{
    /// <summary>
    /// Interaction logic for UserControlListbox.xaml
    /// </summary>
    public partial class UserControlListbox : UserControl
    {
        public static List<string> UCNamesList = new List<string> ( );
        public static UserControlsViewer ucv { get; set; }
        public static UserControlListbox uclb { get; set; }
        public UserControlListbox ( )
        {
            InitializeComponent ( );
            string startpath = Properties . Settings . Default . AppRootPath;
            string [ ] str = startpath . Split ( '\\' );
            startpath = $"{str [ 0 ]}\\{str [ 1 ]}\\UserControls";
            LoadUCList ( startpath );
            ucv = UserControlsViewer . GetUCviewer ( );
            uclb = this;
        }
        public static UserControlListbox GetUCListBox ( )
        {
            return uclb;
        }

        private void UCtrllistbox_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {
        }

        private void UCtrlListbox_MouseDoubleClick ( object sender , MouseButtonEventArgs e )
        {
            ListBox lb = sender as ListBox;
            string controlname = lb . SelectedItem . ToString ( );
            ucv . Contentctrl . Content = null;
            if ( LoadSelectedUserControl ( controlname ) == false )
                ucv . Contentctrl . Content = UControlsListbox;

        }
        public void LoadUCList ( string path )
        {
            UCNamesList . Clear ( );
            var imagefiles = Directory . GetFiles ( path );
            foreach ( var imagefile in imagefiles )
            {
                if ( imagefile . ToUpper ( ) . Contains ( ".XAML" ) == false )
                {
                    try
                    {
                    }
                    catch ( Exception ex ) { Debug. WriteLine ( $"{ex . Message}" ); }
                }
                else if ( imagefile . ToUpper ( ) . Contains ( ".XAML.CS" ) == false )
                {
                    string [ ] fname = imagefile . Split ( '\\' );
                    int max = fname . Length;
                    string file = fname [ max - 1 ];
                    string str = file. Substring ( 0 , file . Length - 5 );
                    UCNamesList . Add ( str );
                }
            }
            UControlsListbox . ItemsSource = UCNamesList;
        }
        private bool  LoadSelectedUserControl ( string controlname )
        {
            bool success = true;
            switch ( controlname . ToUpper ( ) )
            {
                case "APTESTINGCONTROLAP":
                    ucv . Contentctrl . Content = new ApTestingControlAP ( );
                    break;
                case "COLORPICKER":
 //                   ucv . Contentctrl . Content = new Colorpicker ( );
                    break;
                case "FLOWDOC":
                    ucv . Contentctrl . Content = new FlowDoc( );
                    break;
                case "LISTBOXUSERCONTROL":
                    ucv . Contentctrl . Content = new ListBoxUserControl ( );
                    break;
                case "MULTIDBUSERCONTROL":
                    ucv . Contentctrl . Content = new MulltiDbUserControl ( );
                    break;
                case "MULTIIMAGEVIEWER":
                    ucv . Contentctrl . Content = new MultiImageViewer( );
                    break;
                case "MVVMBROWSERUC":
                    ucv . Contentctrl . Content = new MvvmBrowserUC ( );
                    break;
                case "MVVMIMAGEUC":
                    ucv . Contentctrl . Content = new MvvmImageUC ( );
                    break;
                case "MVVMLISTBOXUC":
                    ucv . Contentctrl . Content = new MvvmListboxUC ( );
                    break;
                case "STDDATAUSERCONTROL":
                    ucv . Contentctrl . Content = new StdDataUserControl ( );
                    break;
                case "UCLISTBOX":
                    ucv . Contentctrl . Content = new UCListBox( );
                    break;
                case "UCONTROL1":
                    ucv . Contentctrl . Content = new Ucontrol1 ( );
                    break;
                case "USERCONTROLLISTBOX":
                    MessageBox . Show ( $"Cannot load this control as it is the list you are using" );
                    success = false;
                    break;
                case "USERCONTROLSVIEWER":
                    MessageBox . Show ($"Cannot load this control as it is the current Host application" );
                    success = false;
                    break;
                case "WEBVIEWER":
                    ucv . Contentctrl . Content = new WebViewer ( );
                    break;
            }
            if(success)
                ucv . InfoPanel . Text = $"User Control : {controlname}";
            else
                ucv . InfoPanel . Text = $"User Control : Not Available";
            return success;
        }
    }
}

