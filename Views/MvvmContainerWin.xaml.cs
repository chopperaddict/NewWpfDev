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
using NewWpfDev . ViewModels;

using NewWpfDev . UserControls;
#pragma warning disable CS0105 // The using directive for 'NewWpfDev.ViewModels' appeared previously in this namespace
using NewWpfDev . ViewModels;
#pragma warning restore CS0105 // The using directive for 'NewWpfDev.ViewModels' appeared previously in this namespace

namespace NewWpfDev . Views
{    
    /// <summary>
    /// This is the overall controller for a set of UserControls that are
    /// displayed in 3 different ContentControls hosted in this window
    /// 
    /// This allows flexible use of this widow as a Host for any User Control required
    /// </summary>
    public partial class MvvmContainerWin : Window
    {
        public MvvmContainerViewModel dcvm;
        public static MvvmContainerWin ContainerWin;
        public ContentControl browserContentControl;
        public ContentControl imageContentControl;
        public ContentControl listboxContentControl;
        public MvvmContainerWin ( )
        {
            ContainerWin = this;
            InitializeComponent ( );
            dcvm = MvvmContainerViewModel . GetMvvmContainerViewModel ( );
        }
        public static MvvmContainerWin GetMvvmContainerWin( )
        {
            // Get pointer to Containers ViewModel for later use in ContainerWin
            return ContainerWin;
        }
        private void Window_Loaded ( object sender , RoutedEventArgs e )
        {
        }
    }
}
