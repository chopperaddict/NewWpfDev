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

using NewWpfDev . ViewModels;


namespace NewWpfDev . UserControls
{
    /// <summary>
    /// Interaction logic for MvvmImageUC.xaml
    /// </summary>
    public partial class MvvmImageUC
    {

        public Image CurrentImage
        {
            get; set;
        }
        public bool LeftMouseDown
        {
            get; set;
        }
        public static MvvmContainerViewModel mcvm
        {
            set; get;
        }
        public static MvvmImageUC mvvmImageuc
        {
            get; set;
        }
        public MvvmImageUC ()
        {
            InitializeComponent();
            mcvm = MvvmContainerViewModel . GetMvvmContainerViewModel();
            GetListOfImages();
            mvvmImageuc = this;
        }

        private void Image_PreviewMouseLeftButtonDown (object sender , MouseButtonEventArgs e)
        {

        }
        public void GetListOfImages ()
        {
            string path = @"C:\\Users\ianch\pictures\";
            var imagefiles = Directory . GetFiles(path);
            // Get pointer to listbox Viewmodel
            MvvmListboxUC listboxuc = MvvmListboxUC . GetListBoxUc();
        }
        private void Grid_PreviewMouseMove (object sender , MouseEventArgs e)
        {
            //pt = GetMousePosition ( Utils . FindVisualParent<Window> ( this ) , "WINDOW" );

            //var ctrl = Utils . FindVisualParent<Window> ( sender as UIElement );

            ////pt = GetMousePosition ( sender as UIElement , "CONTROL" );

            string [ ] arr = new string [ 20 ];
            Image img = Utils . FindVisualParent<Image>(sender as UIElement , out arr);
            Thickness th = new Thickness();
            th = img . Margin;
            if ( LeftMouseDown )
            {
                string [ ] strarr = new string [ 20 ];
                Point pt = GetMousePosition(Utils . FindVisualParent<Window>(sender as Image , out strarr) , "IMAGE");
                th . Left = pt . X;
                th . Top = pt . Y;
                Debug . WriteLine($"Mouse mmmmmmmove {pt . X} , {pt . Y}. th={th . Left},{th . Top}, {th . Right}, {th . Bottom}..........");
                img . Margin = th;
            }
        }
        private Point GetMousePosition (object window , string mode = "SCREEN")
        {
            var position = new Point();
            Window win = new Window();
            // Position of the mouse relative to the Screen
            // and allows for the window being moved around as well
            if ( mode . ToUpper() == "SCREEN" )
            {
                win = window as Window;
                position = new Point(Mouse . GetPosition(win) . X + win . Left , Mouse . GetPosition(win) . Y + win . Top);
                Debug . WriteLine($"Mouse to Screen X = {position . X}, Y = {position . Y}");
            }
            // Position of the mouse relative to the window
            else if ( mode . ToUpper() == "WINDOW" )
            {
                win = window as Window;
                position = Mouse . GetPosition(win);
                Debug . WriteLine($"Mouse to Window  {win . ToString()} X = {position . X}, Y = {position . Y}");
            }
            else if ( mode . ToUpper() == "IMAGE" )
            {
                Image ctrl = window as Image;
                position = Mouse . GetPosition(ctrl);
                //Debug. WriteLine ( $"Mouse to Image {ctrl . ToString ( )} X = {position . X}, Y = {position . Y}" );
            }
            else
            {
                win = window as Window;
                // converts to screen position of specified window
                position = win . PointToScreen(Mouse . GetPosition(win));
                Debug . WriteLine($"Screen from Window {win . ToString()} X = {position . X}, Y = {position . Y}");
            }
            // Add the window position
            return position;
            //            return new Point ( position . X + win . Left , position . Y + win . Top );
        }

        private void Image_IsMouseDirectlyOverChanged (object sender , DependencyPropertyChangedEventArgs e)
        {
            var ctrl = sender as UIElement;
            string objstring = ctrl . ToString();
            if ( objstring . Contains(".Image") )
            {
                Image img = ctrl as Image;
                string name = img . Name;
                if ( img . GetType() . Equals(typeof(Image)) )
                {
                    GetMousePosition(img , "IMAGE");
                }
                //                UIElement CurrentImage = Utils . FindVisualParent<UIElement> ( ctrl ) as UIElement;
                //                GetMousePosition ( img , "CONTROL" );

            }
            else if ( objstring . Contains("pack://") )
            {
                string [ ] arr = new string [ 20 ];
                Image CurrentImage = Utils . FindVisualParent<UIElement>(ctrl, out arr) as Image;
                GetMousePosition(ctrl , "CONTROL");
            }
            else
                GetMousePosition(ctrl , "CONTROL");
        }

        private void Image1_PreviewMouseLeftButtonDown (object sender , MouseButtonEventArgs e)
        {
            LeftMouseDown = true;
        }

        private void Image1_PreviewMouseLeftButtonUp (object sender , MouseButtonEventArgs e)
        {
            LeftMouseDown = false;
        }
    }
}
