namespace NewWpfDev . ViewModels
{
    using System;
    using System . Collections . Generic;
    using System . Linq;
    using System . Security . AccessControl;
    using System . Security . Policy;
    using System . Text;
    using System . Threading . Tasks;
    using System . Windows;
    using System . Windows . Controls;
    using System . Windows . Input;
    using System . Xml . Linq;

    using NewWpfDev . UserControls;
    using NewWpfDev . Views;
    public class MvvmContainerViewModel 
    {
        // Get a pointer to the ContentControl in MvvmContainerWin Window 
        public static ContentControl cctrl1;
        public static ContentControl cctrl2;
        public static ContentControl cctrl3;
        // Public pointerd
        public MvvmContainerWin ContainerWin;
        public static MvvmContainerViewModel mvvmContainerVm;

        #region UICommand declarations

        //private ICommand command1;
        //private ICommand command2;
        //private ICommand command3;
        public ICommand Command1 { get; set; }
        public ICommand Command2 { get; set; }
        public ICommand Command3 { get; set; }
        public ICommand CommandClose { get; set; }
        public ICommand CommandCloseAll { get; set; }
        public ICommand HideListbox { get; set; }
        public ICommand BrowserExpand { get; set; }
        public ICommand HostWindow { get; set; }
        public ICommand LoadControlInContentCtrl { get; set; }

        public ICommand ShowUCList { get; set; }
        #endregion UICommand declarations


           static public List<string> UCList = new List<string> ( );

        public string Button1Content
        { get { return "Selection Listbox"; } }
        public string Button2Content
        { get { return "Images Viewer"; } }
        public string Button3Content
        { get { return "Web Browser"; } }
        public string Button4Content
        { get { return "Close Window"; } }
        public string Button5Content
        { get { return "User Controls"; } }


        public MvvmContainerViewModel ( )
        {
            //Setup the ICommands
            Command1 = new RelayCommand ( ExecuteCommand1 , CanExecuteCommand1 );
            Command2 = new RelayCommand ( ExecuteCommand2 , CanExecuteCommand2 );
            Command3 = new RelayCommand ( ExecuteCommand3 , CanExecuteCommand3 );
            CommandClose = new RelayCommand ( ExecuteCommandClose , CanExecuteCommandClose );
            CommandCloseAll = new RelayCommand ( ExecuteCommandCloseAll , CanExecuteCommandCloseAll );
            BrowserExpand = new RelayCommand ( ExecuteBrowserCommand , CanExecuteBrowserCommand );
            HostWindow = new RelayCommand ( ExecuteHostWindow , CanExecuteHostWindow );
            HideListbox = new RelayCommand ( ExecuteHideListbox , CanExecuteHideListbox );
            LoadControlInContentCtrl = new RelayCommand ( ExecuteLoadControlInContentCtrl , CanExecuteLoadControlInContentCtrl );
            ShowUCList = new RelayCommand ( ExecuteShowUCList , CanExecuteShowUCList );
            //Save static pointer to this class
            mvvmContainerVm = this;

            // Get Pointer to Container Window
            ContainerWin = MvvmContainerWin . GetMvvmContainerWin ( );
            CreateUCList ( );
        }

        private bool CanExecuteCommandCloseAll ( object arg )
        {
            return true;
        }

        private void ExecuteCommandCloseAll ( object obj )
        {
            Application . Current . Shutdown ( );
        }

        public static MvvmContainerViewModel GetMvvmContainerViewModel ( )
        {
            return mvvmContainerVm;
        }

        private void ExecuteShowUCList ( object obj )
        {
            if ( cctrl3 == null )
                cctrl3 = ContainerWin . RightContentControl;
            if ( cctrl3 . Content != null )
            {
                cctrl3 . Content = null;
                ContainerWin . RightContentControl . Content = null;
            }
            ContainerWin . RightContentControl . Content = new MvvmListboxUC ( 1 );
            cctrl3 = ContainerWin . RightContentControl;
        }

        static public void CreateUCList ( )
        {
            UCList . Clear ( );
            UCList . Add ( "Colorpicker" );
            UCList . Add ( "FlowDoc" );
            UCList . Add ( "StdDataUserControl" );
            UCList . Add ( "Ucontrol1" );
            UCList . Add ( "MvvmImasgeUC" );
            UCList . Add ( "MultiDbUserControl" );
            UCList . Add ( "WebViewer" );
            UCList . Add ( "ApTestigControlAP" );
            UCList . Add ( "ListboxUserControl" );
            UCList . Add ( "MutliDbUserControl" );
            UCList . Add ( "MvvmImageUC" );
            UCList . Add ( "PropertyChanged" );
        }
        public void ExecuteLoadControlInContentCtrl ( object Args )
        {
            //string str = ContainerWin . objectname.Text;
            //CtrlArgs args = Args as CtrlArgs;
            //int x = args . CtrlToUse;
            //if ( x < 0 || x > 3 )
            //    return;
            //object y = args . ObjToLoad;
            //x = 0;
            //y=
            //if ( y == null )
            //    return;
            //switch ( x )
            //{
            //    case 1:
            //        cctrl1 . Content = y;
            //        break;
            //    case 2:
            //        cctrl2 . Content = y;
            //        break;
            //    case 3:
            //        cctrl3 . Content = y;
            //        break;
            //}
        }

        private bool CanExecuteLoadControlInContentCtrl ( object arg )
        { return true; }
        private bool CanExecuteShowUCList ( object arg )
        { return true; }

        private void ExecuteHideListbox ( object obj )
        {

        }

        #region Attached Properties
        public static int GetBrowserColumn ( DependencyObject obj )
        {
            return ( int ) obj . GetValue ( BrowserColumnProperty );
        }
        public static void SetBrowserColumn ( DependencyObject obj , int value )
        {
            obj . SetValue ( BrowserColumnProperty , value );
        }
        public static readonly DependencyProperty BrowserColumnProperty =
            DependencyProperty . RegisterAttached ( "BrowserColumn" , typeof ( int ) , typeof ( MvvmContainerViewModel ) , new PropertyMetadata ( 1 ) );
        public static int GetBrowserColumnSpan ( DependencyObject obj )
        {
            return ( int ) obj . GetValue ( BrowserColumnSpanProperty );
        }
        public static void SetColumnSpan ( DependencyObject obj , int value )
        {
            obj . SetValue ( BrowserColumnSpanProperty , value );
        }
        public static readonly DependencyProperty BrowserColumnSpanProperty =
            DependencyProperty . RegisterAttached ( "BrowserColumnSpan " , typeof ( int ) , typeof ( MvvmContainerViewModel ) , new PropertyMetadata ( 1 ) );

        #endregion Attached Properties

        public void SetContainerPointer ( MvvmContainerWin win )
        {
            ContainerWin = win;
        }

        #region Command Handlers
        private void ExecuteBrowserCommand ( object obj )
        {
            Grid grid = ContainerWin . BrowserGrid as Grid;
        }
        // Finally, these ALL work as expected
        public void ExecuteCommand1 ( object obj )
        {
            // Right hand column - Red button = Listbox
            if ( cctrl3 == null )
                cctrl3 = ContainerWin . RightContentControl;
            if ( cctrl3 . Content == null )
            {
                ContainerWin . RightContentControl . Content = new MvvmListboxUC ( 1 );
                cctrl3 = ContainerWin . RightContentControl;
            }
            else
            {
                cctrl3 . Content = null;
            }
        }
        public void ExecuteCommand2 ( object obj )
        {   //Green button - image in left column
            if ( cctrl1 == null )
                cctrl1 = ContainerWin . LeftContentControl;
            if ( cctrl1 . Content == null )
            {
                ContainerWin . LeftContentControl . Content = null;
                ContainerWin . LeftContentControl . Content = new MvvmImageUC ( );
                cctrl1 = ContainerWin . LeftContentControl;
                ContainerWin . LeftContentControl . UpdateLayout ( );
            }
            else
            {
                ContainerWin . LeftContentControl . Content = null;
            }
        }
        public void ExecuteCommand3 ( object obj )
        {   // blue button - web browser in center column
            if ( cctrl2 == null )
                cctrl2 = ContainerWin . CenterContentControl;
            if ( cctrl2 . Content == null )
            {
                ContainerWin . CenterContentControl . Content = new MvvmBrowserUC ( );
                ContainerWin . BrowserButton . Content = "Close Browser Window";
            }
            else
            {
                ContainerWin . CenterContentControl . Content = null;
                ContainerWin . BrowserButton . Content = "Show Browser Window";
            }
            cctrl2 = ContainerWin . CenterContentControl;
        }
        private void ExecuteHostWindow ( object obj )
        {// orange button - Mvvm window in center column
            if ( cctrl2 == null )
                cctrl2 = ContainerWin . CenterContentControl;
            if ( cctrl2 . Content != null )
            {
                ContainerWin . CenterContentControl . Content = null;
                ContainerWin . CenterContentControl . UpdateLayout ( );
            }
            if ( MvvmListboxUCViewModel . SelectedUCtrl == null )
            {
                MessageBox . Show ( "Please select a User Control to be shown ....");
                return;
            }
            ContainerWin . CenterContentControl . Content = null;
            var v = MvvmListboxUCViewModel . SelectedUCtrl;
            //UserControl uctrl = GetUserControl ( v );
//            ContainerWin . CenterContentControl . Content = uctrl;
            cctrl2 = ContainerWin . CenterContentControl;
            ContainerWin . CenterContentControl . UpdateLayout ( );
            cctrl2. UpdateLayout ( );
        }
        public  void GetUserControl ( string ctrlname )
        {
#pragma warning disable CS0219 // The variable 'selctrl' is assigned but its value is never used
            UserControl selctrl = null;
#pragma warning restore CS0219 // The variable 'selctrl' is assigned but its value is never used
            //    switch ( ctrlname . ToUpper ( ) )
            //    {
            //        case "COLORPICKER":
            //            ContainerWin . UCName . Text = "ColorPcker";
            //            selctrl = new Colorpicker ( );
            //            break;
            //        case "UCONTROL1":
            //            selctrl = new Ucontrol1 ( );
            //            break;
            //        case "WEBVIEWER":
            //            selctrl = new WebViewer ( );
            //            break;
            //        case "APTESTINGCONTROLAP":
            //            selctrl = new ApTestingControlAP ( );
            //            break;
            //        case "LISTBOXUSERCONTROL":
            //            selctrl = new ListBoxUserControl( );
            //            break;
            //        case "MULTIDBUSERCONTROL":
            //            selctrl = new MulltiDbUserControl( );
            //            break;
            //        case "MVVMIMAGEUC":
            //            selctrl = new MvvmImageUC ( );
            //            break;
            //        case "MVVMLISTBOXUC":
            //            selctrl = new MvvmListboxUC( );
            //            break;
            //        case "STDDATAUSERCONTROL":
            //            selctrl = new StdDataUserControl ( );
            //            break;
            //        case "FLOWDOC":
            //            selctrl = new FlowDoc( );
            //            break;
            //    }
            return;
        }
        public void DisplayInContainer( object obj , int column)
        {
            // called by others to load something in our container window 
            if ( column == 1 )
            {
                if ( cctrl1 == null )
                    cctrl1 = ContainerWin . LeftContentControl;
            }
            else if ( column == 2 )
            {
                if ( cctrl2 == null )
                    cctrl2 = ContainerWin . CenterContentControl;
                if ( cctrl2 . Content != null )
                {
                    ContainerWin . CenterContentControl . Content = null;
                    ContainerWin . CenterContentControl . UpdateLayout ( );
                }
            }
            else if ( column == 3 )
            {
                if ( cctrl3 == null )
                    cctrl3 = ContainerWin . RightContentControl;
            }
            if ( obj == null )
            {
                MessageBox . Show ( "Please select an item to be shown ...." );
                return;
            }
            //ContainerWin . CenterContentControl . Content = null;
            //var v = MvvmListboxUCViewModel . SelectedUCtrl;
            //UserControl uctrl = GetUserControl ( v );
            //ContainerWin . CenterContentControl . Content = uctrl;
            //cctrl2 = ContainerWin . CenterContentControl;
            //ContainerWin . CenterContentControl . UpdateLayout ( );
            //cctrl2 . UpdateLayout ( );
        }

        public void ExecuteCommandClose ( object obj )
        {
            string[] arr = new string [ 20 ];
            var parent = Utils . FindVisualParent<Window> ( ContainerWin, out arr );
            parent . Close ( );
        }
        #endregion Command Handlers

        #region CanExecute handlers
        private bool CanExecuteCommand1 ( object arg )
        { return true; }
        private bool CanExecuteCommand2 ( object arg )
        { return true; }
        private bool CanExecuteCommand3 ( object arg )
        { return true; }
        private bool CanExecuteCommandClose ( object arg )
        { return true; }
        private bool CanExecuteBrowserCommand ( object arg )
        { return true; }
        private bool CanExecuteHostWindow ( object arg )
        { return true; }
        private bool CanExecuteHideListbox ( object arg )
        { return true; }

        #endregion CanExecute handlers

    }

    public class CtrlArgs
    {
        public int CtrlToUse { get; set; }
        public object ObjToLoad { get; set; }
    }
}