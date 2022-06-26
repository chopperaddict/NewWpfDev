using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . Data;
using System . Diagnostics;
using System . Net . Http;
//using System . Net . Http;
using System . Threading;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;

using Newtonsoft . Json . Linq;

using NewWpfDev . SQL;
using NewWpfDev . ViewModels;

namespace NewWpfDev . Views
{
    public partial class InterWinComms : Window
    {
        public static event EventHandler<TooltipArgs> Tooltipshown;

        private CancellationTokenSource currentCancellationSource;
        ObservableCollection<GenericClass> Gvm = new ObservableCollection<GenericClass>();
        private int currentlines { get; set; } = 1;
        private List<string> TemplateList { get; set; } = new List<string>();
        private List<string> LbStyleList { get; set; } = new List<string>();
        public InterWinComms ()
        {
            InitializeComponent();
            //Set up  to receive broadcast progress messages from anywhere that has a  reason to send them
            EventControl . WindowMessage += InterWinComms_WindowMessage;
            // Load Generic SQL data
            EventControl . GenDataLoaded += EventControl_GenDataLoaded;
            Mouse . OverrideCursor = Cursors . Arrow;
            LbStyleList . Add("_ColumnsListBoxItemStyle");
            LbStyleList . Add("GenericListBoxItemStyle1");
            LbStyleList . Add("_ListBoxItemStyle1");
            LbStyleList . Add("GrpAcctsListBoxItemStyle");
            LbStyleList . Add("ListBoxItemMagnifyAnimation");
            StyleTemplates . ItemsSource = LbStyleList;
            StyleTemplates . SelectedIndex = 0;
            StyleTemplates . UpdateLayout();
            TemplateList . Add("GenericTemplate");
            TemplateList . Add("GenDataTemplate1");
            TemplateList . Add("GenDataTemplate2");
            ItemTemplates . ItemsSource = TemplateList;
            ItemTemplates . SelectedIndex = 0;
            ItemTemplates . UpdateLayout();
        }

        private void EventControl_GenDataLoaded (object sender , LoadedEventArgs e)
        {
            Application . Current . Dispatcher . Invoke(() =>
            {
                ListBox lb = lbcontrol . listbox1;
                lbcontrol . listbox1 . ItemsSource = e . DataSource as ObservableCollection<GenericClass>;
                // NB do not have DataTemplate, but rather ItemTemplate for listbox
                // This is in the Resources in this file for portability
                lbcontrol . listbox1 . ItemTemplate = FindResource("GenericTemplate") as DataTemplate;
            });
        }

        private void InterWinComms_WindowMessage (object sender , InterWindowArgs e)
        {
            Debug. WriteLine($"Window message received in InterWinComms......");
            WinMsgs . Items . Add(e . message);
            currentlines++;
            if ( currentlines > 12 ) currentlines = 1;
            Utils . ScrollLBRecordIntoView(WinMsgs , WinMsgs . Items . Count - 1);
            //            ListBox lb = lbcontrol . listbox1;
            WinMsgs . SelectedIndex = WinMsgs . Items . Count - 1;
            Utils . ScrollLBRecordIntoView(WinMsgs , WinMsgs . SelectedIndex);
            WinMsgs . UpdateLayout();
            WinMsgs . SelectedIndex = WinMsgs . Items . Count - 1;
        }

        //========================================================================//
        // Demo of a working async task - works perfectly, allows cancel & shows progress bar & counter
        //========================================================================//
        private async void Button_Click (object sender , RoutedEventArgs e)
        {
            // Enable/disabled buttons so that only one counting task runs at a time.
            this . Button_Start . IsEnabled = false;
            this . Button_Cancel . IsEnabled = true;
            try
            {
                // Set up the progress event handler - this instance automatically invokes to the UI for UI updates
                // this.ProgressBar_Progress is the progress bar control
                IProgress<int> progress = new Progress<int>(count => this . ProgressBar_Progress . Value = count);

                // Note the Dispose() further down !!!!
                currentCancellationSource = new CancellationTokenSource();
                await CountToOneHundredAsync(progress , this . currentCancellationSource . Token);
                // Operation was successful. Let the user know!
                MessageBox . Show("Done counting!");
                percent . Text = "";
}
            catch ( OperationCanceledException )
            {
                // Operation was cancelled. Let the user know!
                MessageBox . Show("Operation cancelled.");
                percent . Text = "";
            }
            finally
            {
                // Reset controls in a finally block so that they ALWAYS go 
                // back to the correct state once the counting ends, 
                // regardless of any exceptions
                this . Button_Start . IsEnabled = true;
                this . Button_Cancel . IsEnabled = false;
                this . ProgressBar_Progress . Value = 0;

                // Dispose of the cancellation source as it is no longer needed
                this . currentCancellationSource . Dispose();
                this . currentCancellationSource = null;
            }
        }

        private async Task CountToOneHundredAsync (IProgress<int> progress , CancellationToken cancellationToken)
        {
            for ( int i = 1 ; i <= 100 ; i++ )
            {
                // This is where the 'work' is performed. 
                // Feel free to swap out Task.Delay for your own Task-returning code! 
                // You can even await many tasks here

                // ConfigureAwait(false) tells the task that we dont need to come back to the UI after awaiting
                // This is a good read on the subject - https://blog.stephencleary.com/2012/07/dont-block-on-async-code.html
                await Task . Delay(100 , cancellationToken) . ConfigureAwait(false);

// If cancelled, an exception will be thrown by the call the task.Delay
// and will bubble up to the calling method because we used await!

// Report progress with the current number
                progress . Report(i);
                Application . Current . Dispatcher . Invoke(() =>
                {
                    percent . Text = i . ToString() + " %";
                    percent . UpdateLayout();
                });
            }
        }
        private async Task<JObject> Http1Async (Uri uri , CancellationToken cancellationToken)
        {
            using ( var client = new HttpClient() )
            {
                var jsonString = await client . GetStringAsync(uri);
                return JObject . Parse(jsonString);
            }
            for ( int i = 1 ; i <= 100 ; i++ )
            {
// You can even await many tasks here
// ConfigureAwait(false) tells the task that we dont need to come back to the UI after awaiting
// This is a good read on the subject - https://blog.stephencleary.com/2012/07/dont-block-on-async-code.html
                await Task . Delay(100 , cancellationToken) . ConfigureAwait(false);
                percent . Text = lbcontrol . ToString();
                percent . UpdateLayout();
                //                progress . Report ( i );
            }
        }

        private void Button_Cancel_Click (object sender , RoutedEventArgs e)
        {
// Cancel the cancellation token
            this . currentCancellationSource . Cancel();
            percent . Text = "";
        }

        private void Window_SizeChanged (object sender , SizeChangedEventArgs e)
        {
            WinMsgs . Height = this . Height - 82;
        }

        private void CloseAppBtn (object sender , RoutedEventArgs e)
        {
            Application . Current . Shutdown();
        }

        private void CloseWin (object sender , RoutedEventArgs e)
        {
            this . Close();
        }

        private void ShowWebviewer (object sender , RoutedEventArgs e)
        {
//            int indx = 0;
//            lbcontrol . Visibility = Visibility . Collapsed;
//            WinMsgs . Visibility = Visibility . Collapsed;
//            WebViewer . Visibility = Visibility . Visible;
//            string site = @"www..codeproject.com" . ToLower();
//            foreach ( string item in WebView2. addressBar . Items )
//            {
//                if ( item . ToLower() == site )
//break;
//indx++;
//            }
//            if ( indx == Webviewer . addressBar . Items . Count )
//{
//                indx = Webviewer . addressBar . Items . Add(site);
//                Webviewer . LoadWebsite(indx);
//}
//else
//                Webviewer . LoadWebsite(indx);
        }

        private void ClearPanel (object sender , RoutedEventArgs e)
        {
            //lbcontrol . Visibility = Visibility . Collapsed;
            //WinMsgs . Visibility = Visibility . Collapsed;
            //Webviewer . Visibility = Visibility . Collapsed;
        }

        private void ShowLog (object sender , RoutedEventArgs e)
        {
            //lbcontrol . Visibility = Visibility . Collapsed;
            //Webviewer . Visibility = Visibility . Collapsed;
            //WinMsgs . Visibility = Visibility . Visible;
        }

        private async void ShowLbCtrl (object sender , RoutedEventArgs e)
        {
            string Sqlcommand = "";
            //Webviewer . Visibility = Visibility . Collapsed;
            WinMsgs . Visibility = Visibility . Collapsed;
            lbcontrol . Visibility = Visibility . Visible;
            Utils . LoadConnectionStrings();
            string addr = sqlcommand . Text;
            DataTable dt = new DataTable();
            if ( addr != "" )
            {
                Sqlcommand = "Select * from " + addr;
                await Task . Run(() =>
                {
                    dt = SqlSupport . LoadGenericData(Sqlcommand , addr , 0 , true);
                }) . ConfigureAwait(false);
                if ( dt . Rows . Count > 0 )
                {
                    await Task . Run(() =>
                    {
                        SqlSupport . LoadGenericCollection(dt , true);
                    }) . ConfigureAwait(false);
                }
            }
        }

        private void ItemTemplates_SelectionChanged (object sender , SelectionChangedEventArgs e)
        {
            lbcontrol . listbox1 . ItemTemplate = FindResource(ItemTemplates . SelectedItem . ToString()) as DataTemplate;
            lbcontrol . listbox1 . UpdateLayout();
            //WinMsgs.  ItemTemplate = FindResource ( ItemTemplates . SelectedItem . ToString ( ) ) as DataTemplate;
            //WinMsgs . UpdateLayout ( );
        }

        private void StyleTemplates_SelectionChanged (object sender , SelectionChangedEventArgs e)
        {
            string style = StyleTemplates . SelectedItem . ToString();
            lbcontrol . listbox1 . ItemContainerStyle = FindResource(style) as Style;
            lbcontrol . listbox1 . UpdateLayout();
            //WinMsgs . ItemContainerStyle = FindResource ( style ) as Style;
            //WinMsgs . UpdateLayout ( );
        }

        private void control_MouseEnter (object sender , MouseEventArgs e)
        {
        }

        private void control_MouseLeave (object sender , MouseEventArgs e)
        {

        }
    }
}
