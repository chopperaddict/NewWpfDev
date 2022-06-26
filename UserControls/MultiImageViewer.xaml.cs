using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Diagnostics;
using System . IO;
using System . Linq;
using System . Text;
using System . Threading;
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
using System . Windows . Threading;

namespace NewWpfDev . UserControls
{
    /// <summary>
    /// Interaction logic for MultiImageViewer.xaml
    /// </summary>
    public partial class MultiImageViewer : UserControl, IDataErrorInfo
    {
        public static ObservableCollection<Image> MVImages = new ObservableCollection<Image> ( );
        public bool WrapPanelLoaded { get; set; } = false;
        public int WPLoadCount { get; set; } = 0;
        private UserControlsViewer ucv { get; set; }
        public string Error { get; }
        string IDataErrorInfo.Error
        {
            get
            {
                if ( img == null )
                {
                    return "Image cannot be null.";
                }
                return null;
            }
        }
        
        // Required by IDataError  above!!!!
        public string this [ string PropertyName ]
        {
            get
            {
                if ( PropertyName == "img" )
                {
                    if ( img . Source == null )
                        return null;
                }
                return null;
            }
        }

        //BackgroundWorker worker = new BackgroundWorker ( );
        public MultiImageViewer ( )
        {
            InitializeComponent ( );
            
            // set up for closedown cleanup as UserControls do NOT have Closing/Closed methods
            this . Loaded += UcHostWindow_Loaded;

            // get handlle  to our parent window
            ucv = UserControlsViewer . GetUCviewer ( );

            if (  MVImages . Count == 0 )
            {
                Mouse . OverrideCursor = Cursors . Wait;
                Dispatcher . Invoke ( ( ) =>
                {
                    LoadImagesAsync ( MVImages );
                } );
                Mouse . OverrideCursor = Cursors . Arrow;
                Task . Run ( ( ) => WrapPanelSetup ( ) );
                this . Refresh ( );
                sp1 . Children . Clear ( );
                this . Loaded += UcHostWindow_Loaded;
            }
            else
            {
                if(sp1.Children . Count <= 1)
                {
                    sp1.Children . Clear ( );
                    Task . Run ( ( ) => WrapPanelSetup ( ) );
                }
                sp1 . UpdateLayout ( );
                ucv . InfoPanel . Text = $"Multi Image Viewer";
                ucv . WrapPanelLoaded = true;
            }
        }

        private void UcHostWindow_Loaded ( object sender , RoutedEventArgs e )
        {
            Window parentWin = Window . GetWindow ( this );
            parentWin . Closing += ParentWin_Closing;
        }
        private void ParentWin_Closing ( object sender , System . ComponentModel . CancelEventArgs e )
        {
            //UcHostWindow vm = this . DataContext as UcHostWindow;
            MVImages = null;
            ucv = null;            
        }
        private async void LoadImagesAsync ( ObservableCollection<Image> Mvimages )
        {
            int count = 0;
            if ( Mvimages . Count == 0 )
            {
                //               Mouse . OverrideCursor = Cursors . Wait;
                string path = @"C:\\Users\ianch\pictures\";
                var imagefiles = Directory . GetFiles ( path );
                foreach ( var imagefile in imagefiles )
                {
                    if ( imagefile . ToUpper ( ) . Contains ( ".PSD" ) == false )
                    {
                        try
                        {
                            Uri url = new Uri ( imagefile );
                            BitmapImage img = new BitmapImage ( url );
                            Image image = new Image ( );
                            image . Source = img;
                            image . Height = 100;
                            image . Width = 100;
                            image . Tag = url;
                            Mvimages . Add ( image );
                            count++;
                            Console . Write ( "." );
                        }
                        catch ( Exception ex ) { Debug. WriteLine ( $"{ex . Message}" ); }
                    }
                }
            }
            Debug. WriteLine ( $"{count} images loaded into ObservableCollection MvImages" );
            Mouse . OverrideCursor = Cursors . Arrow;
            return;
        }
        async private Task WrapPanelSetup ( )
        {
            {
                // This loads ALL the images into the Wrappanel, and can be quite slow
                Dispatcher . BeginInvoke ( DispatcherPriority . Normal , ( Action ) LoadMultiViewImages );

            }
            //if ( ucv . WrapPanelLoaded == false )
            //{
            //    //await Dispatcher . BeginInvoke ( new Action ( ( ) =>
            //    // LoadMultiViewImages ( )
            //    //) , DispatcherPriority . ApplicationIdle );
            //}
            //else
            //{
            //}

            //{
            //    return;

            //worker = new BackgroundWorker ( );
            //worker . RunWorkerCompleted += new RunWorkerCompletedEventHandler ( worker_RunWorkerCompleted );
            //worker . DoWork += new DoWorkEventHandler ( worker_DoWork );
            //worker . WorkerReportsProgress = true;
            //worker . ProgressChanged += worker_ProgressChanged;
            //worker . RunWorkerAsync ( 10000 );
            //          Mouse . OverrideCursor = Cursors . Arrow;
            //}
        }

        private async void LoadMultiViewImages ( )
        {
            WPLoadCount = 0;
            sp1 . Children . Clear ( );
            Debug. WriteLine ( $"Loading Multiple Images " );
            for ( int x = WPLoadCount ; x < MVImages . Count ; x++ )
            {
                sp1 . Children . Add ( MVImages [ x ] );
                WPLoadCount++;
                Console . Write ( "." );
                Dispatcher . Invoke ( ( ) =>
                {
                    string msg = $"{WPLoadCount}/{MVImages . Count} images loaded successfully.....";
                    ucv . Loadcounter . Text = msg;
                } , DispatcherPriority . Background );

                if ( WPLoadCount >= 50 )
                {
                    Debug. WriteLine ( "." );
                    break;
                }
            }
            // Only now we set Images Loaded into WrapPanel flags
            //         WrapPanelLoaded = true;
            ucv . WrapPanelLoaded = true;
        }

        // not in use
        private void LoadImages ( )
        {
            int count = 0;
            if ( MVImages . Count == 0 )
            {
                Mouse . OverrideCursor = Cursors . Wait;
                string path = @"C:\\Users\ianch\pictures\";
                var imagefiles = Directory . GetFiles ( path );
                foreach ( var imagefile in imagefiles )
                {
                    if ( imagefile . ToUpper ( ) . Contains ( ".PSD" ) == false )
                    {
                        try
                        {
                            Uri url = new Uri ( imagefile );
                            BitmapImage img = new BitmapImage ( url );
                            Image image = new Image ( );
                            image . Source = img;
                            image . Height = 100;
                            image . Width = 100;
                            image . Tag = url;
                            MVImages . Add ( image );
                            count++;
                        }
                        catch ( Exception ex ) { Debug. WriteLine ( $"{ex . Message}" ); }
                    }
                }
            }
            Debug. WriteLine ( $"{count} images loaded into ObservableCollection MvImages" );
            Mouse . OverrideCursor = Cursors . Arrow;
        }

        // IS in use

        private void img_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
        {
            // Show image in standard viewer & set Images listbox pointer to this image as well !!
            int index = 0;
            WrapPanel img = sender as WrapPanel;
            Image imagename = e . OriginalSource as Image;
            string s = imagename . Tag . ToString ( );
            s = s . Substring ( 8 );
            Uri url = new Uri ( s );
            BitmapImage img2 = new BitmapImage ( url );
            Image image = new Image ( );
            image . Source = img2;
            ucv . Contentctrl . Content = image;
            ucv . Contentctrl . Refresh ( );
            ucv . WrapPanelImages . Visibility = Visibility . Collapsed;
            ucv . Contentctrl . Visibility = Visibility . Visible;

            UCListBox ucl = UCListBox . GetUcLisbox ( );
            foreach ( var item in MVImages )
            {
                string str = item . Tag . ToString ( ) . Substring ( 8 );
                if ( str == s )
                {
                    ucl . CurrentIndex = index;
                    ucl . UClistbox . SelectedIndex = index;
                    ucl . UClistbox . SelectedItem = index;
                    break;
                }
                index++;
            }

        }
        public void ChangeSize ( )
        {
            UserControl_SizeChanged ( null , null );
        }
        private void UserControl_SizeChanged ( object sender , SizeChangedEventArgs e )
        {
            sv1 . Width = this . Width;
            sv1 . Height = this . Height;
        }

        private void sp1_ScrollChanged ( object sender , ScrollChangedEventArgs e )
        {
            var v = e . VerticalChange;
            Debug. WriteLine ( $"{v}" );
        }

        private void sp1_PreviewMouseWheel ( object sender , MouseWheelEventArgs e )
        {
            var v = e . Delta;
            Debug. WriteLine ( $"Wheel Delta = {v}" );

        }

        private void sp1_MouseWheel ( object sender , MouseWheelEventArgs e )
        {
            var v = e . GetPosition ( sp1 );
            Debug. WriteLine ( $"oisition = {v}" );

        }

        private void img_PreviewMouseRightButtonDown ( object sender , MouseButtonEventArgs e )
        {
#pragma warning disable CS0219 // The variable 'index' is assigned but its value is never used
            int index = 0;
#pragma warning restore CS0219 // The variable 'index' is assigned but its value is never used
            WrapPanel img = sender as WrapPanel;
            Image imagename = e . OriginalSource as Image;
            string s = imagename . Tag . ToString ( );
            s = s . Substring ( 8 );
            for ( int x = 0 ; x < MVImages . Count ; x++ )
            {
                if ( MVImages [ x ] . Name == s )
                {
                    break;
                }
            }
        }
        //    private async void worker_DoWork ( object sender , DoWorkEventArgs e )
        //    {
        //        // do the work required here.... it  shoulld return  the data set, whatever that is ?
        //        //Dispatcher . BeginInvoke ( DispatcherPriority . Normal , ( Action ) ( async ( ) =>
        //        //    Task.Run(worker . ReportProgress ( 0 , null ));
        //        //) );
        //        //await  Task . Run ( () => worker . ReportProgress ( 0 , null ) );

        //        //worker . ReportProgress ( 0 , null );
        //        // Dispatcher . Invoke ( ( ) =>
        //        //{
        //        //    Task . Run ( ( ) => LoadMultiViewImages ( Images , sp1 ) )

        //        //} , System . Windows . Threading . DispatcherPriority . Background; 
        //    }
        //    void worker_RunWorkerCompleted ( object sender , RunWorkerCompletedEventArgs e )
        //    {
        //        Debug. WriteLine ( "Background worker completed...." );
        //        // handle the data once it has been loaded here ....
        //        // Typically something like 
        //        // datagrid . ItemsSource = "Data received object" from above
        //    }

        //    async public void worker_ProgressChanged ( object sender , ProgressChangedEventArgs e )
        //    {

        //        Debug. WriteLine ( "in progress changed...." );
        //        return;
        //        sp1 . Children . Clear ( );
        //        if ( WrapPanelLoaded == false )
        //        {
        //            ucv . uclistbox . UClistbox . ItemsSource = null;
        //            //ucv . uclistbox . UClistbox . Items . Clear ( );

        //            foreach ( var item in MVImages )
        //            {
        //                sp1 . Children . Add ( item );
        //                //                    miv . sp1 . UpdateLayout ( );
        //            }
        //            Mouse . OverrideCursor = Cursors . Wait;
        //            ucv . Contentctrl . Content = this;
        //            ucv . Contentctrl . Refresh ( );
        //            //WrapPanelImages . Visibility = Visibility . Visible;
        //            //WrapPanelImages . Opacity = 1;
        //            ucv . WrapPanelImages . Content = this;
        //            Mouse . OverrideCursor = Cursors . Arrow;
        //        }
        //        else
        //        {
        //            Mouse . OverrideCursor = Cursors . Wait;
        //            ucv . Contentctrl . Visibility = Visibility . Collapsed;
        //            ucv . WrapPanelImages . Visibility = Visibility . Visible;
        //            ucv . WrapPanelImages . Opacity = 1;
        //            Mouse . OverrideCursor = Cursors . Arrow;
        //        }
        //        ucv . InfoPanel . Text = $"Multi Image Viewer";
        //        WrapPanelLoaded = true;
        //        ucv . WrapPanelLoaded = true;
        //        return;
        //    }
    }

}
