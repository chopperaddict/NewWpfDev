using System . Reflection;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;

//using NewWpfDev . UserControls;

using NewWpfDev . ViewModels;

namespace NewWpfDev . UserControls
{
    /// <summary>
    /// Interaction logic for MvvmBrowserUC.xaml
    /// </summary>
    public partial class MvvmBrowserUC 
    {
        //WebBrowser MyBrowser;
        public static string Uri { get; set; }
        public ICommand ForwardButton_cmd { get; set; }
        public ICommand BackwardButton_cmd { get; set; }
        public ICommand ClearCombo { get; set; }
        public ICommand NavigateFwd { get; set; }
        public ICommand NavigateBack { get; set; }
//        public MvvmBrowserUC BrowserClass { get; set; }
        public MvvmBrowserUC ( )
        {
            InitializeComponent ( );
            this.DataContext = this;
            Uri = "https://www.google.com";
            //UrlCombo . Items . Add ( Uri );
            //UrlCombo . Items . Add ( "https://Yahoo.com");
            //UrlCombo . Items . Add ( "https://Github.com" );
            //UrlCombo . Items . Add ( "https://Morrisons.co.uk" );
            //UrlCombo . Items . Add ( "https://Codeproject.com" );
            //UrlCombo . SelectedIndex = 0;
            //UrlCombo . SelectedItem = 0; 
            //wbSample . Navigate ( Uri );
            //this . navigationKeys ( );
            ForwardButton_cmd =  new RelayCommand ( ExecuteForwardButton , CanExecuteForwardButton );
            BackwardButton_cmd = new RelayCommand ( ExecuteBackwardButton , CanExecuteBackwardButton );
            ClearCombo = new RelayCommand ( ExecuteClearCombo , CanExecuteClearCombo );
            NavigateBack = new RelayCommand ( ExecuteNavigateBack , CanExecuteNavigateBack);
            NavigateFwd = new RelayCommand ( ExecuteNavigateFwd , CanExecuteNavigateFwd );
            //var wvc = new WebView2 ( );
            //Browsergrid . Children . Add ( wvc );
            //CoreWebView2CreationProperties cp = new CoreWebView2CreationProperties ( );
            //CreateAsync ( "C:\\aaa" , "https://www.google.com" , CoreWebView2EnvironmentOptions );
            //cp .
            //wvc . Source = new Uri( "https://www.google.com"  );
            //wvc . NavigateToString ( "https://www.google.com" );
        }

        private void ExecuteNavigateFwd ( object obj )
        {
            //if ( UrlCombo . SelectedIndex < UrlCombo . Items . Count - 1 )
            //    UrlCombo . SelectedIndex++;
            //UrlCombo . SelectedItem = UrlCombo . SelectedIndex;
            //Uri = UrlCombo . SelectedItem.ToString();
            //if ( Uri != null )
            //{
            //    if ( Uri . ToUpper ( ) . Contains ( "HTTP" ) == false )
            //        Uri = "Http://" + Uri;
            //    wbSample . Navigate ( Uri );
            //}
        }

    private void ExecuteNavigateBack ( object obj )
        {
            //if ( UrlCombo . SelectedIndex == 0 )
            //    return;
            //UrlCombo . SelectedIndex --;
            //UrlCombo . SelectedItem = UrlCombo . SelectedIndex;
            //Uri = UrlCombo . SelectedItem . ToString ( );
            //if ( Uri != null )
            //{
            //    if ( Uri . ToUpper ( ) . Contains ( "HTTP" ) == false )
            //        Uri = "Http://" + Uri;
            //    wbSample . Navigate ( Uri );
            //}
        }

        private void ExecuteClearCombo ( object obj )
        {
//            UrlCombo . Items . Clear ( );
        }


        private void txtUrl_KeyUp ( object sender , KeyEventArgs e )
        {
            //if ( e . Key == Key . Enter )
            //{
            //    if ( txtUrl . Text . ToUpper ( ) . Contains ( "HTTP" ) == false )
            //        txtUrl . Text = "Httbp://" + txtUrl . Text;
            //    UrlCombo . Items . Add ( txtUrl . Text );
            //    Uri = txtUrl . Text;
            //    wbSample . Navigate ( Uri );
            //}
        }

        #region  CanExecute handlers
        private bool CanExecuteNavigateFwd ( object arg )
        { return true; }
        private bool CanExecuteNavigateBack ( object arg )
        { return true; }
        private bool CanExecuteClearCombo ( object arg )
        { return true; }
        private bool CanExecuteBackwardButton ( object arg )
        { return true; }
        private bool CanExecuteForwardButton ( object arg )
        { return true; }
        private void BrowseBack_CanExecute ( object sender , CanExecuteRoutedEventArgs e )
        {/*e . CanExecute = ( ( wbSample != null ) && ( wbSample . CanGoBack ) );*/}
        private void BrowseForward_CanExecute ( object sender , CanExecuteRoutedEventArgs e )
        {/* e . CanExecute = ( ( wbSample != null ) && ( wbSample . CanGoForward ) );*/ }        
        private void BrowseForward_Executed ( object sender , ExecutedRoutedEventArgs e )
        {
            //webView . NavigationStarting += EnsureHttps;
            //InitializeAsync ( );
            //initUrlList ( );
        }
        private void GoToPage_CanExecute ( object sender , CanExecuteRoutedEventArgs e )
        {e . CanExecute = true;}
        
        #endregion  CanExecute handlers
        private void GoToPage_Executed ( object sender , ExecutedRoutedEventArgs e )
        {
//            wbSample . Navigate ( txtUrl . Text );
        }


        public void HideScriptErrors ( WebBrowser wb , bool hide )
        {
            var fiComWebBrowser = typeof ( WebBrowser ) . GetField ( "_axIWebBrowser2" , BindingFlags . Instance | BindingFlags . NonPublic );
            if ( fiComWebBrowser == null ) return;
            var objComWebBrowser = fiComWebBrowser . GetValue ( wb );
            if ( objComWebBrowser == null )
            {
                wb . Loaded += ( o , s ) => HideScriptErrors ( wb , hide ); //In case we are to early
                return;
            }
            objComWebBrowser . GetType ( ) . InvokeMember ( "Silent" , BindingFlags . SetProperty , null , objComWebBrowser , new object [ ] { hide } );
        }
        
        private void UserControl_Loaded ( object sender , RoutedEventArgs e )
        {
//            HideScriptErrors ( wbSample , true );
        }

        private void navigationKeys ( )
        {
            // if browser has a forward page
            //if ( !wbSample . CanGoForward )
            //{
            //    // enable button
            //    ForwardButton . IsEnabled = false;
            //}
            //else
            //{
            //    // disable it
            //    ForwardButton . IsEnabled = true;
            //}

            //// if browser has a back page
            //if ( !wbSample . CanGoBack )
            //{
            //    // enable button
            //    BackButton . IsEnabled = false;
            //}
            //else
            //{
            //    // disable button
            //    BackButton . IsEnabled = true;
            //}
        }
        private void myBrowser_KeyDown ( object sender , KeyEventArgs e )
        {
            // get the web browser
            WebBrowser myBrowser = sender as WebBrowser;

            // get if the key is BACKSPACE then go back!
            if ( e . Key == Key . Back )
            {
                if ( myBrowser . CanGoBack )
                {
                    myBrowser . GoBack ( );
                }
            }
            else if ( e . Key == Key . F5)
            {
                //wbSample . Navigate ( txtUrl . Text );
                BrowserRefresh_Click ( sender , e );
            }
        }
        // Reload the current page function.
        private void BrowserRefresh_Click ( object sender , RoutedEventArgs e )
        {
            //wbSample . Refresh ( );
            //txtUrl . Text = wbSample . Source . ToString ( );
            //wbSample . Navigate ( txtUrl . Text );
        }
        
        #region Navigation button handlers
        private void ExecuteBackwardButton ( object obj )
        {
            //if ( wbSample . CanGoBack )
            //{ wbSample . GoBack ( ); }
        }
        private void ExecuteForwardButton ( object obj )
        {
            //if ( wbSample . CanGoForward )
            //{ wbSample . GoForward ( ); }
        }
        private void wbSample_Navigating ( object sender , System . Windows . Navigation . NavigatingCancelEventArgs e )
        {
            //txtUrl . Text = e . Uri . OriginalString;
            //if ( txtUrl . Text . ToUpper ( ) . Contains ( "HTTP" ) == false )
            //    txtUrl . Text = "Httbp://" + txtUrl . Text;
        }

        #endregion Navigation button handlers

        private void UrlCombo_KeyDown ( object sender , KeyEventArgs e )
        {
            //if ( e . Key == Key . Enter )
            //{
            //    Uri = UrlCombo . Text;
            //    if ( Uri != null )
            //    {
            //        if ( Uri . ToUpper ( ) . Contains ( "HTTP" ) == false )
            //            Uri = "Http://" +Uri;
            //        wbSample . Navigate ( Uri );
            //        UrlCombo . Items . Add ( Uri);
            //    }
            //}
        }

        private void UrlCombo_PreviewMouseDoubleClick ( object sender , MouseButtonEventArgs e )
        {
            //UrlCombo . SelectedItem = UrlCombo . SelectedIndex;
            //ActionUrl ( );
        }

        private void UrlCombo_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {
            ActionUrl ( );
        }
        public void ActionUrl ( )
        {
            //if ( UrlCombo . Text == "" )
            //    return;
            //Uri = UrlCombo . Text;
            //if ( Uri != null )
            //{
            //    if ( Uri . ToUpper ( ) . Contains ( "HTTP" ) == false )
            //        Uri = "Http://" + Uri;
            //    wbSample . Navigate ( Uri );
            //}
        }
    }
}