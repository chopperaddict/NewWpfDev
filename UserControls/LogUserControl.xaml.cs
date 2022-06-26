using System;
using System . Collections . Generic;
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

using NewWpfDev . ViewModels;
using NewWpfDev . Views;

using NewWpfDev . Views;

namespace NewWpfDev . UserControls
{
    /// <summary>
    /// Interaction logic for logUserControl.xaml
    /// </summary>
    public partial class LogUserControl : UserControl
    {
        public static LogUserControl ThisWin;
        public static TabWinViewModel Controller
        {
            get; set;
        }
        public static Tabview tabviewWin
        {
            get; set;
        }
        public static ListBox loglistbox
        {
            get; set;
        }
        public LogUserControl ()
        {
            InitializeComponent();
            ThisWin = this;
            // save our content's control as a pointer
            loglistbox = ThisWin . logview;
            tabviewWin = TabWinViewModel . SendTabview();

            //            tabviewWin . TabSizeChanged ( null , null );

        }
        public static LogUserControl SetController (object ctrl)
        {
            Controller = ctrl as TabWinViewModel;
            tabviewWin = TabWinViewModel . SendTabview();
            return ThisWin;
        }
        public void PART_MouseLeave (object sender , MouseEventArgs e)
        {
            var tabview = TabWinViewModel . Tview;
            //TabItem  item = TabWinViewModel . CurrentTabitem;
            //Controller . SetCurrentTab ( tabview , TabWinViewModel . CurrentTabName );
            if ( TabWinViewModel . CurrentTabTextBlock == "Tab4Header" )
            {
                tabview . Tab4Header . FontSize = 14;
                Tabview . TriggerStoryBoardOff(4);
                tabview . Tab4Header . Foreground = FindResource("Cyan0") as SolidColorBrush;
            }
        }

        public void PART_MouseEnter (object sender , MouseEventArgs e)
        {
            var tabview = TabWinViewModel . Tview;
            //Point pt = e . GetPosition ( ( UIElement ) sender );
            //HitTestResult hit = VisualTreeHelper . HitTest ( ( Visual ) sender , pt );
            if ( TabWinViewModel . CurrentTabTextBlock == "Tab4Header" )
            {
                tabview . Tab4Header . FontSize = 18;
                Tabview . TriggerStoryBoardOn(4);
                tabview . Tab4Header . Foreground = FindResource("Yellow0") as SolidColorBrush;
            }
        }
        private void logview_PreviewMouseMove (object sender , MouseEventArgs e)
        {
            ListBox logSender = sender as ListBox;
            if ( logSender != null )
            {
                logSender = sender as ListBox;
                if ( logSender . Name == "logview" )
                {
                    TabWinViewModel . CurrentTabIndex = 3;
                    TabWinViewModel . CurrentTabName = "LogviewTab";
                    TabWinViewModel . CurrentTabTextBlock = "Tab4Header";
                }
            }
        }
    }

}

