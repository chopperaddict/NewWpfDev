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

using Microsoft . Web . WebView2 . Core;

namespace NewWpfDev. UserControls
{
    /// <summary>
    /// Interaction logic for WebViewer.xaml
    /// </summary>
    public partial class WebViewer : UserControl
    {
        public WebViewer ( )
        {
            InitializeComponent ( );
            //webView.NavigationStarting += EnsureHttps;
            InitializeAsync ( );
            initUrlList ( );
        }
        public WebViewer (string url )
        {
            InitializeComponent ( );
//            webView . NavigationStarting += EnsureHttps;
            InitializeAsync ( );
            initUrlList ( );
        }
        public string URL { get; set; }
        public string LastURL { get; set; }
        public string OriginalAddress { get; set; }
        async void InitializeAsync ( )
        {
//            var env = await CoreWebView2Environment . CreateAsync ( userDataFolder: Path . Combine ( Path . GetTempPath ( ) , "MarkdownMonster_Browser" ) );

            await webView . EnsureCoreWebView2Async ( null );
            webView . CoreWebView2 . WebMessageReceived += UpdateAddressBar;

            await webView . CoreWebView2 . AddScriptToExecuteOnDocumentCreatedAsync ( "window.chrome.webview.postMessage(window.document.URL);" );
            await webView . CoreWebView2 . AddScriptToExecuteOnDocumentCreatedAsync ( "window.chrome.webview.addEventListener(\'message\', event => alert(event.data));" );
        }
        void UpdateAddressBar ( object sender , CoreWebView2WebMessageReceivedEventArgs args )
        {
            String uri = args . TryGetWebMessageAsString ( );
            String ActualUrl = args . Source;
            if ( uri != ActualUrl )
            {
                webView . CoreWebView2 . PostWebMessageAsString ( uri );
                return;
            }
            addressBar . Text = ActualUrl;
            //if(uri != addressBar .Text)
            //    webView . CoreWebView2 . PostWebMessageAsString ( uri );

            if ( CheckForDuplicateUrl ( ActualUrl ) == false )
                addressBar . Items . Add ( ActualUrl );
            // Add it to  our URL combo
        }
        private void initUrlList ( )
        {
            addressBar . Items . Add ( "https://www.Morrisons.co.uk" );
            addressBar . Items . Add ( "https://www.Yahoo.com" );
            addressBar . Items . Add ( "https://www.github.com" );
            addressBar . Items . Add ( "https://www.codeproject.com" );
            addressBar . SelectedIndex = 0;
            addressBar . SelectedItem = addressBar . SelectedIndex;
            if ( addressBar . SelectedItem != null )
                OriginalAddress = addressBar . SelectedItem . ToString ( );
        }
        private string CheckForHttps ( string url )
        {
            if ( url . ToUpper ( ) . Contains ( "HTTP://" ) == false
                && url . ToUpper ( ) . Contains ( "HTTPS://" ) == false )
                return "http://" + url;
            else
                return url;
        }
        void EnsureHttps ( object sender , CoreWebView2NavigationStartingEventArgs args )
        {
            String uri = args . Uri;
            if ( !uri . StartsWith ( "https://" ) )
            {
                string [ ] url;
                char ch = ':';
                url = uri . Split ( ch );
                uri = "https:" + url [ 1 ];
                // this CANCELS the navigation - weird really!!
                //   args . Cancel = true;
            }
        }
        private void addressBar_PreviewKeyDown ( object sender , KeyEventArgs e )
        {
            // Enter key hit, so navigate
            if ( e . Key == Key . Enter )
            {
                string newurl = addressBar . Text . ToString ( );
                if ( CheckForDuplicateUrl ( newurl ) == false )
                    addressBar . Items . Add ( newurl );
                addressBar . SelectedIndex = addressBar . Items . Count - 1;
                //addressBar . SelectedItem = addressBar . SelectedIndex;
                ButtonGo_Click ( sender , null );
            }
        }
        private bool CheckForDuplicateUrl ( string newurl )
        {
            foreach ( string item in addressBar . Items )
            {

                if ( item == null )
                    return false;
                if ( item . ToUpper ( ) == newurl . ToUpper ( ) )
                    return true;
            }
            return false;
        }
        private void Window_Loaded ( object sender , RoutedEventArgs e )
        {
            OriginalAddress = "https://www.google.com";
            addressBar . Items . Add ( OriginalAddress );
            addressBar . SelectedIndex = addressBar . Items . Count - 1;
            addressBar . SelectedItem = addressBar . SelectedIndex;
            //ButtonGo_Click ( sender , null );
        }
        public  void LoadWebsite(int index)
        {
            addressBar . SelectedIndex = index;
            ButtonGo_Click ( this , null );
        }
        private void ButtonGo_Click ( object sender , RoutedEventArgs e )
        {
            if ( webView != null && webView . CoreWebView2 != null )
            {
                if ( addressBar . SelectedIndex == -1 )
                {
                    if ( (string)addressBar . SelectedItem != "" )
                    {
                        addressBar . SelectedIndex = 0;
                        URL = addressBar . SelectedItem . ToString ( );
                    }
                    else return;
                }
                else
                {
                    if ( addressBar . SelectedItem . ToString ( ) == "" )
                        return;
                    else
                    {
                        URL = addressBar . SelectedItem . ToString ( );
                        if ( URL == LastURL )
                            return;
                        OriginalAddress = URL;
                    }
                }
                OriginalAddress = CheckForHttps ( URL );
                webView . CoreWebView2 . Navigate ( OriginalAddress );
                LastURL = OriginalAddress;
            }
        }

        private void addressBar_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {
            if ( addressBar . SelectedItem == null )
                return;
            addressBar . Text = addressBar . SelectedItem . ToString ( );
            //addressBar . SelectedIndex = addressBar . SelectedItem;
            ButtonGo_Click ( sender , null );
        }

        private void ButtonFwd_Click ( object sender , RoutedEventArgs e )
        {
            //            Debug. WriteLine ( $"index = {addressBar . SelectedIndex }" );
            //            Debug. WriteLine ( $"item = {addressBar . SelectedItem}" );
            if ( addressBar . SelectedIndex < addressBar . Items . Count - 1 )
            {
                addressBar . SelectedIndex++;
                addressBar . SelectedItem = addressBar . SelectedIndex;
                //                Debug. WriteLine ( $"index = {addressBar . SelectedIndex }" );
            }
        }

        private void ButtonBack_Click ( object sender , RoutedEventArgs e )
        {
            //            Debug. WriteLine ( $"index = {addressBar . SelectedIndex }" );
            //            Debug. WriteLine ( $"item = {addressBar . SelectedItem}" );
            if ( addressBar . SelectedIndex >= 1 )
            {
                addressBar . SelectedIndex--;
                addressBar . SelectedItem = addressBar . SelectedIndex;
                //                Debug. WriteLine ( $"index = {addressBar . SelectedIndex }" );
            }
        }

        private void addressBar_DropDownOpened ( object sender , EventArgs e )
        {
            string current = addressBar . Text;
            int count = 0;
            foreach ( var item in addressBar . Items )
            {
                if ( item . ToString ( ) == current )
                {
                    addressBar . SelectedItem = count;
                    break;
                }
                count++;
            }
        }
    }
}

