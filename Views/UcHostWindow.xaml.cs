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
using System . Windows . Shapes;

using NewWpfDev . UserControls;

namespace NewWpfDev . Views
{
    /// <summary>
    /// Interaction logic for UcHostWindow.xaml
    /// </summary>
    public partial class UcHostWindow : Window
    {
        private double uchheight { get; set; }
        private double uchwidth { get; set; }

     public static UserControlsViewer ucv { get; set; }
    public static UcHostWindow uchw { get; set; }
        public UcHostWindow ( )
        {
            InitializeComponent ( );
            uchw = this;

        }
        public void LoadHostWindow()
        {
             ucv = new UserControlsViewer ( );
            UCHostContent . Content = ucv;
            this . Refresh ( );

        }
        public static UcHostWindow GetUCHostWin ( )
        {
            //return a pointer to this host !
            return uchw;
        }
        private void Window_SizeChanged ( object sender , SizeChangedEventArgs e )
        {
            if ( ucv == null )
                return;
            ucv.Height = UCHostContent . Height;
            uchwidth= UCHostContent . Width;
        }
        private void Window_Closing ( object sender , System . ComponentModel . CancelEventArgs e )
        {
            ucv = null;
            uchw = null;
        }
        public void CloseThis(object obj)
        {
            obj = null;
        }
        private void UcHostWindow_Loaded ( object sender , RoutedEventArgs e )
        {

        }

        private void Window_Loaded ( object sender , RoutedEventArgs e )
        {

        }
    }
}
