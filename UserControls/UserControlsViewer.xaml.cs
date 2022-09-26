using System;
using System . Collections . Generic;
using System . ComponentModel;
using System . Diagnostics;
using System . IO;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Media . Imaging;

using NewWpfDev . Views;


namespace NewWpfDev . UserControls
{
    /// <summary>
    /// Interaction logic for UserControlsViewer.xaml
    /// </summary>
    public partial class UserControlsViewer : IDataErrorInfo
    {
        // pointer to this class
        public static UserControlsViewer UCCtrlViewer { get; set; }
        public UCListBox uclistbox { get; set; }
        public UcHostWindow uhw { get; set; }
        public WebViewer webViewer { get; set; }
        public UserControlListbox uclb { get; set; }

        public MultiImageViewer miv { get; set; }
        public bool WrapPanelLoaded { get; set; } = false;

        public static List<Image> WPImages = new List<Image> ( );

        private double ucvheight { get; set; }
        private double ucvwidth { get; set; }

        #region IDataErrorInfo
        public string Error { get; }

        public string this [ string PropertyName ]
        {
            get
            {
                switch ( PropertyName . ToUpper ( ) )
                {
                    case "xx":
                        {
                            break;
                        }
#pragma warning disable CS0162 // Unreachable code detected
                        return null;
#pragma warning restore CS0162 // Unreachable code detected
                }
                return null;
            }
        }
        #endregion IDataErrorInfo

            //public ICommand ClearPanel;
            //public ICommand ShowListbox;
            //public ICommand LoadImage;
            //public ICommand CloseWindow;
            //public ICommand CloseApp;
        public UserControlsViewer ( )
        {
            InitializeComponent ( );
            uhw = UcHostWindow . GetUCHostWin ( );
            this . DataContext = this;
            LoadUserControlsViewer ( );
            this . Loaded += UcHostWindow_Loaded;
        }
        private void UcHostWindow_Loaded ( object sender , RoutedEventArgs e )
        {
            Window parentWin = Window . GetWindow ( this );
            parentWin . Closing += ParentWin_Closing;
        }
        private void ParentWin_Closing ( object sender , System . ComponentModel . CancelEventArgs e )
        {
            //UcHostWindow vm = this . DataContext as UcHostWindow;
            UCCtrlViewer = null;
            uclistbox = null;
            uhw = null;
            webViewer = null;
            uclb = null;
            miv = null;
            WrapPanelLoaded = false;
            WPImages = null; 
    }
        public void LoadUserControlsViewer ( )
        {
            UCCtrlViewer = this;
            DataContext = this;
            //ClearPanel = new RelayCommand ( ExecuteClearPanel , CanExecuteClearPanel );
            //ShowListbox = new RelayCommand ( ExecuteShowListbox , CanExecuteShowListbox );
            //LoadImage = new RelayCommand ( ExecuteLoadImage , CanExecuteLoadImage );
            //CloseWindow = new RelayCommand ( ExecuteCloseWindow , CanExecuteCloseWindow );
            //CloseApp = new RelayCommand ( ExecuteCloseApp , CanExecuteCloseApp );
            uclistbox = new UCListBox ( );
            Contentctrl . Content = uclistbox;
            uclb = new UserControlListbox ( );
            WrapPanelLoaded = false;
            this.WrapPanelLoaded = false;
            //           miv = new MultiImageViewer ( );
            //LoadMultiViewer ( WPImages );
            //            doload ( );
        }
        public static UserControlsViewer GetUCviewer ( )
        {
            return UCCtrlViewer;
        }
        private void ExecuteClearPanel ( object obj )
        {
            Contentctrl . Content = null;
        }
        private void ExecuteShowListbox ( object obj )
        {
            Contentctrl . Content = uclistbox;
            //Contentctrl . Visibility = Visibility . Visible;
        }
        private void ExecuteLoadImage ( object obj )
        {
            string currentctrl = "";
            ListBox lb = uclistbox . UClistbox as ListBox;
            if ( lb != null )
                return;
            currentctrl = lb . SelectedItem . ToString ( );
            Image img = new Image ( );
            img . Source = new BitmapImage ( new Uri ( currentctrl , UriKind . Relative ) );
            Contentctrl . Content = img;
        }
        private void ExecuteCloseApp ( object obj )
        {
            Application . Current . Shutdown ( );
        }
        private void ExecuteCloseWindow ( object obj )
        {
            uhw . Close ( );
        }

        #region CanExecute
        private bool CanExecuteCloseApp ( object arg )
        { throw new NotImplementedException ( ); }
        private bool CanExecuteCloseWindow ( object arg )
        { throw new NotImplementedException ( ); }
        private bool CanExecuteLoadImage ( object arg )
        { throw new NotImplementedException ( ); }
        private bool CanExecuteShowListbox ( object arg )
        { throw new NotImplementedException ( ); }
        private bool CanExecuteClearPanel ( object arg )
        { throw new NotImplementedException ( ); }
        #endregion CanExecute
        private void doload ( )
        {
            //            Task . Run ( ( ) => LoadImages( ) );
            //            WPImages = LoadImages ( WPImages );
            //            LoadMultiViewer ( WPImages );
        }

        private List<Image> LoadImages ( List<Image> ImageList )
        {
            int count = 0;
            if ( ImageList . Count > 0 )
                return ImageList;
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
                        ImageList . Add ( image );
                        count++;
                        if ( count > 100 )
                            break;
                    }
                    catch ( Exception ex ) { Debug. WriteLine ( $"{ex . Message}" ); }
                }
            }
            //            await Task . Run ( () => 
            //              LoadMultiViewer ( ) );
            //await Dispatcher . BeginInvoke ( DispatcherPriority . Normal , ( Action ) ( async ( ) =>
            //    await LoadMultiViewer ( )
            //) );

            //            Task . Run ( ( ) => LoadMultiViewer ( ) );

            return ImageList;
        }

        public bool LoadMultiViewer ( List<Image> ImageList )
        {
            //if ( ImageList . Count == 0 )
            //    LoadImages ( ImageList );
            //MultiImageViewer miv = new MultiImageViewer ( );
            //WrapPanelImages . Content = miv;
            //miv . sp1 . Children . Clear ( );
            //foreach ( var item in ImageList )
            //{
            //    miv . sp1 . Children . Add ( item );
            //    WrapPanelImages . UpdateLayout ( );
            //}
            return true;
        }

        private void Command1_Click ( object sender , RoutedEventArgs e )
        {
            // Clear Content Control
            WrapPanelImages . Opacity = 0;
            Contentctrl . Visibility = Visibility . Visible;
            Contentctrl . Content = null;
            InfoPanel . Text = $"";
        }
        private void Command2_Click ( object sender , RoutedEventArgs e )
        {
            if ( WPImages . Count == 0 )
            {
                //Loads images into ItemsSource and resets current index
                uclistbox . LoadListbox ( );
            }
            WrapPanelImages . Opacity = 0;
            Contentctrl . Visibility = Visibility . Visible;
            Contentctrl . Content = uclistbox;
            InfoPanel . Text = $"Images List";
        }
        private void Command3_Click ( object sender , RoutedEventArgs e )
        {
            //if ( uclistbox . UClistbox . SelectedItem == null )
            //    return;
            //WrapPanelImages . Opacity = 0;
            //Contentctrl . Visibility = Visibility . Visible;
            //string sel = uclistbox . UClistbox . SelectedItem . ToString ( );
            //Uri url = new Uri ( sel );
            //BitmapImage img = new BitmapImage ( url );
            //Image image = new Image ( );
            //image . Source = img;
            //Contentctrl . Content = image;
            //InfoPanel . Text = $"Image : {image}";
        }
        private void Command4_Click ( object sender , RoutedEventArgs e )
        {
            // Close window only
            WrapPanelImages . Content = null;
            Contentctrl  . Content = null;
            WPImages ?. Clear ( );
            uclistbox . UClistbox . ItemsSource = null;
            uclistbox.UClistbox.Items.Clear ( );
            miv ?. sp1 . Children . Clear ( );
            //WPImages . Clear ( );
            uclistbox = null;
            uclb = null;
            UCCtrlViewer = null;
            webViewer = null;
            uhw?.Close ( );
            uhw = null;
            //WrapPanelLoaded = false;
            this . WrapPanelLoaded = false;
            InfoPanel . Text = $"";
        }
        private void Command5_Click ( object sender , RoutedEventArgs e )
        {
            // Close application
            WrapPanelImages . Opacity = 0;
            Contentctrl . Visibility = Visibility . Visible;
            Application . Current . Shutdown ( );
        }
        public void ShowImage ( object sender , RoutedEventArgs e )
        {
            WrapPanelImages . Opacity = 0;
            Contentctrl . Visibility = Visibility . Visible;
            string sel = uclistbox . UClistbox . SelectedItem . ToString ( );
            LoadImageinControl ( sel );
            InfoPanel . Text = $"Image : {sel}";
        }

        private void LoadImageinControl ( string imagename )
        {
            WrapPanelImages . Opacity = 0;
            Contentctrl . Visibility = Visibility . Visible;
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            try
            {
                Uri url = new Uri ( imagename );
                BitmapImage img = new BitmapImage ( url );
                Image image = new Image ( );
                image . Source = img;
                Contentctrl . Content = image;
                InfoPanel . Text = $"Image : {image}";
            }
            catch ( Exception ex ) { }
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
        }
        private void ShowPrevious ( object sender , RoutedEventArgs e )
        {
            int selindx = uclistbox . UClistbox . SelectedIndex;
            if ( selindx == -1 )
            {
                selindx = 0;
                uclistbox . UClistbox . SelectedIndex = selindx;
                uclistbox . UClistbox . SelectedItem = selindx;
            }
            WrapPanelImages . Opacity = 0;
            Contentctrl . Visibility = Visibility . Visible;
            if ( selindx > 0 ) selindx--;
            uclistbox . UClistbox . SelectedIndex = selindx;
            uclistbox . UClistbox . SelectedItem = selindx;
            string sel = uclistbox . UClistbox . SelectedItem . ToString ( );
            LoadImageinControl ( sel );
            InfoPanel . Text = $"Image : {sel}";
        }

        private void ShowNextImage ( object sender , RoutedEventArgs e )
        {
            uclistbox . UClistbox . SelectedIndex = uclistbox . CurrentIndex == -1 ? 0 : uclistbox . CurrentIndex;
            int selindx = uclistbox . UClistbox . SelectedIndex;
            if ( selindx == -1 )
            {
                selindx = 0;
                uclistbox . UClistbox . SelectedIndex = selindx;
                uclistbox . UClistbox . SelectedItem = selindx;
            }
            if ( selindx == uclistbox . UClistbox . Items . Count - 1 )
                return;
            WrapPanelImages . Opacity = 0;
            Contentctrl . Visibility = Visibility . Visible;
            selindx++;
            uclistbox . UClistbox . SelectedIndex = selindx;
            uclistbox . UClistbox . SelectedItem = selindx;
            try
            {
                string sel = uclistbox . UClistbox . SelectedItem . ToString ( );
                LoadImageinControl ( sel );

                InfoPanel . Text = $"Image : {sel}";
            }
            catch ( Exception ex ) { Debug. WriteLine ($"{ex.Message}, selindx = {selindx}"); }
        }

        private void ShowBrowser ( object sender , RoutedEventArgs e )
        {
            if ( webViewer == null )
                webViewer = new WebViewer ( );
            WrapPanelImages . Opacity = 0;
            Contentctrl . Visibility = Visibility . Visible;
            Contentctrl . Content = webViewer;
            InfoPanel . Text = $"Web Browser Control";
        }

        private void ShowMultiView ( object sender , RoutedEventArgs e )
        {
            // Show image mulltiviewer
            if ( this.WrapPanelLoaded == false )
            //            if ( WPImages . Count == 0 )
            {
                //MessageBoxResult result = MessageBox . Show ( $"The images have to be rendered, which can take a period of time\n\nDo you want to  continue ?." , "DELAY WARNING !!" , MessageBoxButton . YesNo , MessageBoxImage . Question , MessageBoxResult . Yes );
                //if ( result == MessageBoxResult . No )
                //    return;
                //Mouse . OverrideCursor = Cursors . Wait;
                MultiView . Content = "You can continue working....";
                MultiView . UpdateLayout ( );
                Loadcounter .Background= FindResource("Green5") as SolidColorBrush;
                Loadcounter . UpdateLayout ( );
                miv = new MultiImageViewer ( );
                WrapPanelImages . Content = miv;
                Contentctrl . Visibility = Visibility . Collapsed;
                WrapPanelImages . Visibility = Visibility . Visible;
                WrapPanelImages . Opacity = 1;
                Mouse . OverrideCursor = Cursors . Arrow;
                Loadcounter . Background = FindResource ( "Orange5" ) as SolidColorBrush;
                Loadcounter . UpdateLayout ( );
                MultiView . Content = "Images are full loaded, Click here" +
                    "....";
                MultiView . UpdateLayout ( );

            }
            else
            {
                Contentctrl . Visibility = Visibility . Collapsed;
                WrapPanelImages . Content = miv;
                WrapPanelImages . Visibility = Visibility . Visible;
                WrapPanelImages . Opacity = 1;
                WrapPanelImages . BringIntoView ( );
                WrapPanelImages . Refresh ( );
                Mouse . OverrideCursor = Cursors . Arrow;
            }
            InfoPanel . Text = $"Multi Image Viewer";
        }
        private void UserControl_SizeChanged ( object sender , SizeChangedEventArgs e )
        {
            if ( miv == null )
                return;
            miv . ChangeSize ( );
        }

        private void ShowUserControls ( object sender , RoutedEventArgs e )
        {
            //int x = uclb.UControlsListbox. Items . Count;
            WrapPanelImages . Opacity = 0;
            Contentctrl . Visibility = Visibility . Visible;
            Contentctrl . Content = uclb;
            Contentctrl . Refresh ( );
            //uhw . Refresh ( );
            InfoPanel . Text = $"List of User Controls ";
        }
    }
}
