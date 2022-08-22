
using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Data;
using System . Diagnostics;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Media . Animation;

using NewWpfDev . AttachedProperties;
using NewWpfDev . Dapper;
using NewWpfDev . Models;
using NewWpfDev . Sql;
using NewWpfDev . SQL;
using NewWpfDev . UserControls;
using NewWpfDev . ViewModels;

namespace NewWpfDev . Views
{
    public partial class Listviews : Window, INotifyPropertyChanged
    {
        public static event EventHandler<ListboxHostArgs> SetListboxHost;

        #region OnPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged ( string PropertyName )
        {
            if ( this . PropertyChanged != null )
            {
                var e = new PropertyChangedEventArgs ( PropertyName );
                this . PropertyChanged ( this , e );
            }
        }
        /// <summary>
        /// Warns the developer if this object does not have
        /// a public property with the specified name. This
        /// method does not exist in a Release build.
        /// </summary>
        //[Conditional("DEBUG")]
        //[DebuggerStepThrough]
        //public virtual void VerifyPropertyName (string propertyName)
        //{
        //    // Verify that the property name matches a real,
        //    // public, instance property on this object.
        //    if ( TypeDescriptor . GetProperties(this) [ propertyName ] == null )
        //    {
        //        string msg = "Invalid property name: " + propertyName;

        //        if ( this . ThrowOnInvalidPropertyName )
        //            throw new Exception(msg);
        //        else
        //            Debug . Fail(msg);
        //    }
        //}

        /// <summary>
        /// Returns whether an exception is thrown, or if a Debug.Fail() is used
        /// when an invalid property name is passed to the VerifyPropertyName method.
        /// The default value is false, but subclasses used by unit tests might
        /// override this property's getter to return true.
        /// </summary>
        //protected virtual bool ThrowOnInvalidPropertyName
        //{
        //    get; private set;
        //}

        #endregion OnPropertyChanged

        #region  Public variables
        // Set  up our data collections

        // Individual records
        public BankAccountViewModel bvm = new BankAccountViewModel ( );
        public CustomerViewModel cvm = new CustomerViewModel ( );
        public DetailsViewModel dvm = new DetailsViewModel ( );
        public GenericClass gvm = new GenericClass ( );

        // Collections
        public ObservableCollection<BankAccountViewModel> bankaccts = new ObservableCollection<BankAccountViewModel> ( );
        public ObservableCollection<CustomerViewModel> custaccts = new ObservableCollection<CustomerViewModel> ( );
        public ObservableCollection<DetailsViewModel> detaccts = new ObservableCollection<DetailsViewModel> ( );
        public ObservableCollection<GenericClass> genaccts = new ObservableCollection<GenericClass> ( );

        // Northwind Records / collections
        public nwcustomer nwc = new nwcustomer ( );
        public NwOrderCollection nwo = new NwOrderCollection ( );
        public ObservableCollection<nwcustomer> nwcustomeraccts = new ObservableCollection<nwcustomer> ( );
        public ObservableCollection<nworder> nworderaccts = new ObservableCollection<nworder> ( );

        // Pubs
        public PubAuthors pubAuthor = new PubAuthors ( );
        static public ObservableCollection<PubAuthors> pubauthors = new ObservableCollection<PubAuthors> ( );

        // supporting sources
        public List<string> TablesList = new List<string> ( );

        private string SqlSpCommand
        {
            get; set;
        }
        private string CurrentSPDb
        {
            get; set;
        }
        private string SqlCommand = "";
        private string DefaultSqlCommand = "Select * from BankAccount";
        string CurrentDbName = "IAN1";
        string CurrentTableName = "BANKACCOUNT";
        string CurrentDataTable = "";

        public static string CurrentSqlConnection = "BankSysConnectionString";
        public static Dictionary<string , string> DefaultSqlCommands = new Dictionary<string , string> ( );
        public FlowdocLib fdl = new FlowdocLib ( );
        #endregion  Public variables

        #region private variables
        private bool UseDirectLoad = true;
        //private bool LoadDirect=false;
        private bool alldone = false;
        // pro temp variables
        // Flowdoc flags
        private bool UseFlowdoc = false;
#pragma warning disable CS0414 // The field 'Listviews.UseFlowdocBeep' is assigned but its value is never used
        //       private bool UseFlowdocBeep = false;
#pragma warning restore CS0414 // The field 'Listviews.UseFlowdocBeep' is assigned but its value is never used
        private bool UseScrollViewer = false;
        private bool TvMouseCaptured = false;

        // These ARE used
        //private  double flowdocHeight=0;
        //private  double flowdocWidth=0;
        //private  double flowdocTop=0;
        //private  double flowdocLeft=0;
#pragma warning disable CS0414 // The field 'Listviews.XLeft' is assigned but its value is never used
        //       private double XLeft = 0;
#pragma warning restore CS0414 // The field 'Listviews.XLeft' is assigned but its value is never used
#pragma warning disable CS0414 // The field 'Listviews.YTop' is assigned but its value is never used
        //       private double YTop = 0;
        private bool Startup = false;
        private bool LoadAll = true;
        private bool Usetimer = true;
        private bool ComboSelectionActive = false;
        private static Stopwatch timer = new Stopwatch ( );
        //public Colorpicker ColorPickerObject = new Colorpicker ( );

        //Colorpicker ColorpickerObject = new Colorpicker ( );
        private double TvFirstXPos;
        private double TvFirstYPos;

        //Variables  for resizing FlowDoc
        // Needed to be global to improve performance!
        //private double newWidth=0;
        //private double newHeight =0;
        //private double YDiff = 0;
        //private double XDiff = 0;
        //private double FdLeft = 0;
        //private double FdTop =0;
        //private double FdHeight=0;
        //private double FdWidth=0;
        //private double MLeft=0;
        //private double MTop=0;
        //private bool CornerDrag = false;
        //private double FdBorderWidth=0;
        //private double FdBottom =0;
        //private double ValidTop = 0;
        //private double ValidBottom =0;
        //Thickness th = new Thickness(0,0,0,0);
        #endregion private variables

        #region Full Properties

        // Full properties used in Binding to I/f objects
        private int dbCountlb;
        public int DbCountlb
        {
            get
            {
                return dbCountlb;
            }
            set
            {
                dbCountlb = value; OnPropertyChanged ( "DbCountlb" );
            } //Debug. WriteLine ( $"DbCountlb set to {value}" ); }
        }
        private int dbCountlv;
        public int DbCountlv
        {
            get
            {
                return dbCountlv;
            }
            set
            {
                dbCountlv = value; OnPropertyChanged ( "DbCountlv" );
            } //Debug. WriteLine ( $"DbCountlv set to {value}" ); }
        }

        private bool ismouseDown;
        public bool isMouseDown
        {
            get
            {
                return ismouseDown;
            }
            set
            {
                ismouseDown = value;
            }
        }
        private object movingobject;
        public object MovingObject
        {
            get
            {
                return movingobject;
            }
            set
            {
                movingobject = value;
            }
        }

        private double CpFirstXPos = 0;
        private double CpFirstYPos = 0;

        //private double TvFirstXPos=0;
        //private double TvFirstYPos = 0;
        #endregion Full Properties

        //Data  for font size/rowheight
        private List<int> fontsizes = new List<int> ( );
        private List<int> rowsizes = new List<int> ( );
        private Style BtnStyle
        {
            get; set;
        }

        private bool CanMagnify
        {
            get; set;
        }
        #region Treeview declarations
        public ObservableCollection<Database> DatabasesCollection = new ObservableCollection<Database> ( );
        public ObservableCollection<SqlTable> TablesCollection = new ObservableCollection<SqlTable> ( );
        public ObservableCollection<SqlProcedures> ProcsCollection = new ObservableCollection<SqlProcedures> ( );
        #endregion Treeview declarations

        #region Startup / close
        public Listviews ( )
        {
            InitializeComponent ( );
            this . DataContext = this;
            Startup = true;
            UseFlowdoc = false;
            UseScrollViewer = false;
            Usetimer = true;
            Flags . UseScrollView = false;
            // Set flags so we get the right SQL command method used...
            UseDirectLoad = true;
            SqlCommand = DefaultSqlCommand;
            canvas . Visibility = Visibility . Visible;
            TreeviewBorder . Visibility = Visibility . Hidden;
            IsExpanded . SetIsExpand ( this , false );

            // Handle the magnify sytem to handle global flag
            Flags . UseMagnify = ( bool ) Properties . Settings . Default [ "UseMagnify" ];
            if ( Flags . UseMagnify == false )
            {
                dGrid . Style = ( Style ) FindResource ( "DatagridMagnifyAnimation0" );
                listBox . Style = ( Style ) FindResource ( "ListBoxMagnifyAnimation0" );
                listView . Style = ( Style ) FindResource ( "ListViewMagnifyAnimation0" );
                DataTemplatesLb . Style = ( Style ) FindResource ( "ComboBoxMagnifyAnimation0" );
                DataTemplatesLv . Style = ( Style ) FindResource ( "ComboBoxMagnifyAnimation0" );
                DbMain . Style = ( Style ) FindResource ( "ComboBoxMagnifyAnimation0" );
                dbNameLv . Style = ( Style ) FindResource ( "ComboBoxMagnifyAnimation0" );
                fontSize . Style = ( Style ) FindResource ( "ComboBoxMagnifyAnimation0" );
                rowheight . Style = ( Style ) FindResource ( "ComboBoxMagnifyAnimation0" );
                Magnifyrate . Text = "0";
            }
            else
            {
                dGrid . Style = ( Style ) FindResource ( "DatagridMagnifyAnimation4" );
                listBox . Style = ( Style ) FindResource ( "ListBoxMagnifyAnimation4" );
                listView . Style = ( Style ) FindResource ( "ListViewMagnifyAnimation4" );
                DataTemplatesLb . Style = ( Style ) FindResource ( "ComboBoxMagnifyAnimation4" );
                DataTemplatesLv . Style = ( Style ) FindResource ( "ComboBoxMagnifyAnimation4" );
                DbMain . Style = ( Style ) FindResource ( "ComboBoxMagnifyAnimation4" );
                dbNameLv . Style = ( Style ) FindResource ( "ComboBoxMagnifyAnimation4" );
                fontSize . Style = ( Style ) FindResource ( "ComboBoxMagnifyAnimation4" );
                rowheight . Style = ( Style ) FindResource ( "ComboBoxMagnifyAnimation4" );
                Magnifyrate . Text = "+4";
            }
            Flags . UseScrollView = false;
            if ( Listviews . SetListboxHost != null )
            {
                ListboxHostArgs args = new ListboxHostArgs ( );
                args . HostControl = this;
                args . HostName = "ListViewWindow";
                args . HostType = "Listviews";
                Listviews . SetListboxHost . Invoke ( this , args );
            }
        }

        private void Rectangle_Loaded ( object sender , RoutedEventArgs e )
        {
            //Makes a rectangle height increase/decrease on startup over 0.5 seconds
            Storyboard MyStoryboard = new Storyboard ( );
            DoubleAnimation myDoubleAnimation = new DoubleAnimation ( );
            myDoubleAnimation . From = 35.0;
            myDoubleAnimation . To = 45.0;
            myDoubleAnimation . Duration = new Duration ( TimeSpan . FromSeconds ( 0.5 ) );
            myDoubleAnimation . AutoReverse = true;
            Storyboard . SetTargetProperty ( myDoubleAnimation , new PropertyPath ( Image . HeightProperty ) );
            myDoubleAnimation . RepeatBehavior = RepeatBehavior . Forever;
            MyStoryboard . Children . Add ( myDoubleAnimation );

            DoubleAnimation myDoubleAnimation2 = new DoubleAnimation ( );
            myDoubleAnimation2 . From = 35.0;
            myDoubleAnimation2 . To = 45.0;
            myDoubleAnimation2 . Duration = new Duration ( TimeSpan . FromSeconds ( 0.5 ) );
            myDoubleAnimation2 . AutoReverse = true;
            Storyboard . SetTargetProperty ( myDoubleAnimation2 , new PropertyPath ( Image . WidthProperty ) );
            myDoubleAnimation2 . RepeatBehavior = RepeatBehavior . Forever;
            MyStoryboard . Children . Add ( myDoubleAnimation2 );

            //			Storyboard . SetTargetName ( myDoubleAnimation , "MyRectangle");
            MyStoryboard . Begin ( magnifyimage );
        }

        //*************************************************************//
        // Initial startup - load Db = Ian1 / Table = Bankaccount 
        //*************************************************************//
        private void ListViewWindow_Loaded ( object sender , RoutedEventArgs e )
        {
            this . DataContext = this;
            // Initialize all connection strings  in Flags
            Utils . LoadConnectionStrings ( );

            // Get list of Dbs (just 3 right now)
            LoadAllDbNames ( );

            // Initialize all default Sql command strings
            LoadDefaultSqlCommands ( );
            DefaultSqlCommand = GetDefaultSqlCommand ( "BANKACCOUNT" );
            if ( DefaultSqlCommand == "" )
            {
                Debug . WriteLine ( "Unable to load default Select string for BANKACOUNT Db " );
                WpfLib1 . Utils . DoErrorBeep ( 250 , 50 , 1 );
            }
            else
                SqlCommand = DefaultSqlCommand;
            if ( Utils . CheckResetDbConnection ( "IAN1" , out string constr ) == false )
            {
                Debug . WriteLine ( "Unable to load connection string for BANKACOUNT Db from System Properties" );
                WpfLib1 . Utils . DoErrorBeep ( 250 , 50 , 1 );
            }
            //Default to Bankaccount as we are strting up
            CurrentDbName = "IAN1";
            CurrentTableName = "BANKACCOUNT";

            // Now open Ian1, load data and display in both viewers
            // 1st = connect to IAN1.MDF
            // this also loads list of tables in Ian1/mdf
            OpenIan1Db ( );
            // used to access Dictionary of DataTemplates  - load into both Combos and selects 1st entry

            // now load list of tables in IAN1 Db
            LoadDbTables ( "IAN1" );
            SelectCurrentDbInCombo ( "BANKACCOUNT" , "" );

            // Load Bankaccount into both viewers
            CurrentTableName = "BANKACCOUNT";
            LoadData_Ian1 ( "" );
            // Set selection of datatemplate to 1st one (selection  used in load method)
            CurrentDataTable = DataTemplatesLv . Items [ 0 ] . ToString ( );

            // Hook into our Flowdoc so we can resize it in  the canvas !!!
            // Flowdoc has an Event declared (ExecuteFlowDocReSizeMethod ) that we are  hooking into
            Flowdoc . ExecuteFlowDocMaxmizeMethod += new EventHandler ( MaximizeFlowDoc );
            Flowdoc . ExecuteFlowDocBorderMethod += FlowDoc_ExecuteFlowDocBorderMethod;
            //            Colorpicker . ExecuteSaveToClipboardMethod += Colorpicker_ExecuteSaveToClipboardMethod;

            // LOAD BOTH VIEWERS (NO PARAMETER)
            LoadGrid_IAN1 ( );
            Startup = false;
            if ( UseFlowdoc )
                ShowInfo ( Flowdoc , canvas , header: "Load completed successfully" , clr4: "Red5" ,
                line1: $"Request made was completed succesfully!" , clr1: "Red3" ,
                line2: $"" ,
                line3: $"{SqlCommand . ToUpper ( )}" , clr3: "Blue4"
                );
            LoadFontsizes ( );
            LoadRowsizes ( );
            //PickColors . Fontsize = 12;
            Fontsize = 12;
            ItemBackground = Brushes . LightBlue;
            ItemForeground = Brushes . Black;
            SelectedBackground = Brushes . Red;
            SelectedForeground = Brushes . White;
            MouseoverBackground = Brushes . Blue;
            MouseoverForeground = Brushes . White;
            MouseoverSelectedBackground = Brushes . Black;
            canvas . Visibility = Visibility . Visible;
            MouseoverSelectedForeground = Brushes . White;
        }

        private void ListViewWindow_PreviewKeyDown ( object sender , KeyEventArgs e )
        {
            if ( e . Key == Key . F8 )
                Debugger . Break ( );
            ReleaseMouseCapture ( );
            //			Debug. WriteLine ( "Mouse released 1" );
            TvMouseCaptured = false;
        }
        private void LoadRowsizes ( )
        {
            rowsizes . Add ( 10 );
            rowsizes . Add ( 12 );
            rowsizes . Add ( 14 );
            rowsizes . Add ( 15 );
            rowsizes . Add ( 16 );
            rowsizes . Add ( 17 );
            rowsizes . Add ( 18 );
            rowsizes . Add ( 20 );
            rowsizes . Add ( 22 );
            rowsizes . Add ( 24 );
            rowsizes . Add ( 26 );
            rowsizes . Add ( 28 );
            rowsizes . Add ( 30 );
            rowsizes . Add ( 34 );
            rowsizes . Add ( 36 );
            rowsizes . Add ( 38 );
            rowsizes . Add ( 40 );
            rowsizes . Add ( 45 );
            rowheight . ItemsSource = rowsizes;
        }
        private void LoadFontsizes ( )
        {
            fontsizes . Add ( 11 );
            fontsizes . Add ( 12 );
            fontsizes . Add ( 13 );
            fontsizes . Add ( 14 );
            fontsizes . Add ( 15 );
            fontsizes . Add ( 16 );
            fontsizes . Add ( 17 );
            fontsizes . Add ( 18 );
            fontsizes . Add ( 19 );
            fontsizes . Add ( 20 );
            fontSize . ItemsSource = fontsizes;
        }
        private void Close_Btn ( object sender , RoutedEventArgs e )
        {
            this . Close ( );
        }

        private void App_Close ( object sender , RoutedEventArgs e )
        {
            this . Close ( );
            Application . Current . Shutdown ( );
        }
        #endregion Startup / close

        #region Load data for the 3 different Db's
        private void LoadData_Ian1 ( string viewertype )
        {
            if ( Usetimer )
                timer . Start ( );
            if ( CurrentTableName == "BANKACCOUNT" )
            {
                //// Looad available DataTemplates into combo(s) ??
                LoadDataTemplates_Ian1 ( "BANKACCOUNT" , viewertype );
                bankaccts = new ObservableCollection<BankAccountViewModel> ( );
                SqlCommand = GetDefaultSqlCommand ( CurrentTableName );
                // need to do this cos the SQL command is changed to load the tables list....
                if ( Startup )
                    SqlCommand = DefaultSqlCommand;
                bankaccts = SqlSupport . LoadBank ( SqlCommand , 0 , true );
            }
            else if ( CurrentTableName == "CUSTOMER" )
            {
                LoadDataTemplates_Ian1 ( "CUSTOMER" , viewertype );
                custaccts = new ObservableCollection<CustomerViewModel> ( );
                SqlCommand = GetDefaultSqlCommand ( CurrentTableName );
                // need to do this cos the SQL command is changed to load the tables list....
                if ( Startup )
                    SqlCommand = DefaultSqlCommand;
                custaccts = SqlSupport . LoadCustomer ( SqlCommand , 0 , true );
            }
            else if ( CurrentTableName == "SECACCOUNTS" )
            {
                LoadDataTemplates_Ian1 ( "SECACCOUNTS" , viewertype );
                detaccts = new ObservableCollection<DetailsViewModel> ( );
                SqlCommand = GetDefaultSqlCommand ( CurrentTableName );
                // need to do this cos the SQL command is changed to load the tables list....
                if ( Startup )
                    SqlCommand = DefaultSqlCommand;
                detaccts = SqlSupport . LoadDetails ( SqlCommand , 0 , true );
            }
            else
            {
                string tablename = "";
                LoadDataTemplates_Ian1 ( "GENERIC" , viewertype );
                genaccts = new ObservableCollection<GenericClass> ( );
                // need to do this cos the SQL command is changed to load the tables list....
                if ( viewertype == "VIEW" )
                    tablename = dbNameLv . SelectedItem . ToString ( );
                else
                    tablename = dbNameLb . SelectedItem . ToString ( );
                SqlCommand = $"Select *from {tablename}";
                SqlCommand = GetDefaultSqlCommand ( CurrentTableName );
                if ( SqlCommand == "" )
                    SqlCommand = $"Select * from {tablename}";
                // need to do this cos the SQL command is changed to load the tables list....
                if ( Startup )
                    SqlCommand = DefaultSqlCommand;
                string ResultString = "";
                genaccts = SqlSupport . LoadGeneric ( SqlCommand , out ResultString , 0 , true );
                if ( genaccts . Count > 0 )
                {
                    if ( UseFlowdoc )
                        ShowInfo ( Flowdoc , canvas , line1: $"The requested table [{CurrentTableName}] was loaded successfully, and {genaccts . Count} records were returned,\nThe data is shown in  the viewer below" , clr1: "Black0" ,
                        line2: $"The command line used was" , clr2: "Red2" ,
                        line3: $"{SqlCommand . ToUpper ( )}" , clr3: "Blue4" ,
                        header: "Generic style data table" , clr4: "Red5" );
                }
                else
                {
                    if ( UseFlowdoc )
                        ShowInfo ( Flowdoc , canvas , line1: $"The requested table [{CurrentTableName}] was loaded successfully, but ZERO records were returned,\nThe Table is  " , clr1: "Black0" ,
                        line2: $"The command line used was" , clr2: "Red2" ,
                        line3: $"{SqlCommand . ToUpper ( )}" , clr3: "Blue4" ,
                        header: "Generic style data table" , clr4: "Red5" );


                }
                //else
                //{
                //	// MORE COMPLEX METHODS !!
                //	if ( Usetimer )
                //		timer . Start ( );
                //	if ( CurrentType == "BANKACCOUNT" )
                //		DapperSupport . GetBankObsCollection ( bankaccts , DbNameToLoad: "BankAccount" , Orderby: "Custno, BankNo" , wantSort: true , Caller: "DATAGRIDS" , Notify: true );
                //	else if ( CurrentType == "CUSTOMER" )
                //		DapperSupport . GetCustObsCollection ( custaccts , DbNameToLoad: "Customer" , Orderby: "Custno, BankNo" , wantSort: true , Caller: "DATAGRIDS" , Notify: true );
                //	else if ( CurrentType == "SECACCOUNTS" )
                //		DapperSupport . GetDetailsObsCollection ( detaccts , DbNameToLoad: "SecAccounts" , Orderby: "Custno, BankNo" , wantSort: true , Caller: "DATAGRIDS" , Notify: true );
                //}
            }
        }
        public void LoadData_NorthWind ( string viewertype )
        {
            if ( Usetimer )
                timer . Start ( );
            if ( CurrentTableName == "CUSTOMERS" )
            {

                LoadDataTemplates_NorthWind ( "CUSTOMERS" , viewertype );
                nwcustomeraccts = new ObservableCollection<nwcustomer> ( );
                nwcustomeraccts = nwc . GetNwCustomers ( );
                // need to do this cos the SQL command is changed to load the tables list....
                if ( Startup )
                    SqlCommand = DefaultSqlCommand;
            }
            else if ( CurrentTableName == "ORDERS" )
            {

                LoadDataTemplates_NorthWind ( "ORDERS" , viewertype );
                nworderaccts = new ObservableCollection<nworder> ( );
                nworderaccts = nwo . LoadOrders ( );
                // need to do this cos the SQL command is changed to load the tables list....
                if ( Startup )
                    SqlCommand = DefaultSqlCommand;
            }
            else
            {
                string tablename = "";
                LoadDataTemplates_Ian1 ( "GENERIC" , viewertype );
                genaccts = new ObservableCollection<GenericClass> ( );
                // need to do this cos the SQL command is changed to load the tables list....
                if ( viewertype == "VIEW" )
                    tablename = dbNameLv . SelectedItem . ToString ( );
                else
                    tablename = dbNameLb . SelectedItem . ToString ( );
                SqlCommand = GetDefaultSqlCommand ( CurrentTableName );
                if ( SqlCommand == "" )
                    SqlCommand = $"Select * from {tablename}";
                // need to do this cos the SQL command is changed to load the tables list....
                if ( Startup )
                    SqlCommand = DefaultSqlCommand;
                string ResultString = "";
                genaccts = SqlSupport . LoadGeneric ( SqlCommand , out ResultString , 0 , true );
                if ( genaccts . Count > 0 )
                {
                    if ( UseFlowdoc )
                        ShowInfo ( Flowdoc , canvas , line1: $"The requested table [{CurrentTableName}] was loaded successfully, and {genaccts . Count} records were returned,\nThe data is shown in  the viewer below" , clr1: "Black0" ,
                        line2: $"The command line used was" , clr2: "Red2" ,
                        line3: $"{SqlCommand . ToUpper ( )}" , clr3: "Blue4" ,
                        header: "Generic style data table" , clr4: "Red5" );
                }
                else
                {
                    if ( UseFlowdoc )
                        ShowInfo ( Flowdoc , canvas , line1: $"The requested table [{CurrentTableName}] was loaded successfully, but ZERO records were returned,\nThe Table is  " , clr1: "Black0" ,
                        line2: $"The command line used was" , clr2: "Red2" ,
                        line3: $"{SqlCommand . ToUpper ( )}" , clr3: "Blue4" ,
                        header: "Generic style data table" , clr4: "Red5" );
                }
            }
        }
        public void LoadData_Publishers ( string viewertype , out ObservableCollection<GenericClass> generics )
        {
            generics = null;
            //ObservableCollection<GenericClass> generics = new ObservableCollection<GenericClass>();
            if ( Usetimer )
                timer . Start ( );
            if ( CurrentTableName == "AUTHORS" )
            {
                LoadDataTemplates_PubAuthors ( "AUTHORS" , viewertype );
                pubauthors = new ObservableCollection<PubAuthors> ( );
                // need to do this cos the SQL command is changed to load the tables list....
                if ( Startup )
                    SqlCommand = DefaultSqlCommand;
                //				nwcustomeraccts = SqlSupport . LoadBank ( SqlCommand , 0 , true );
            }
            else if ( CurrentTableName == "ORDERS" )
            {

                LoadDataTemplates_NorthWind ( "ORDERS" , viewertype );
                nworderaccts = new ObservableCollection<nworder> ( );
                // need to do this cos the SQL command is changed to load the tables list....
                if ( Startup )
                    SqlCommand = DefaultSqlCommand;
                //				nwcustomeraccts = SqlSupport . LoadBank ( SqlCommand , 0 , true );
            }
            else
            {
                string tablename = "";
                LoadDataTemplates_Ian1 ( "GENERIC" , viewertype );
                genaccts = new ObservableCollection<GenericClass> ( );
                // need to do this cos the SQL command is changed to load the tables list....
                if ( viewertype == "VIEW" )
                    tablename = dbNameLv . SelectedItem . ToString ( );
                else
                    tablename = dbNameLb . SelectedItem . ToString ( );
                SqlCommand = $"Select *from {tablename}";
                SqlCommand = GetDefaultSqlCommand ( CurrentTableName );
                if ( SqlCommand == "" )
                    SqlCommand = $"Select * from {tablename}";
                // need to do this cos the SQL command is changed to load the tables list....
                if ( Startup )
                    SqlCommand = DefaultSqlCommand;
                string ResultString = "";
                genaccts = SqlSupport . LoadGeneric ( SqlCommand , out ResultString , 0 , true );
                if ( genaccts . Count > 0 )
                {
                    if ( UseFlowdoc )
                        ShowInfo ( Flowdoc , canvas , line1: $"The requested table [{CurrentTableName}] was loaded successfully, and {genaccts . Count} records were returned,\nThe data is shown in  the viewer below" , clr1: "Black0" ,
                        line2: $"The command line used was" , clr2: "Red2" ,
                        line3: $"{SqlCommand . ToUpper ( )}" , clr3: "Blue4" ,
                        header: "Generic style data table" , clr4: "Red5" );
                }
                else
                {
                    if ( UseFlowdoc )
                        ShowInfo ( Flowdoc , canvas , line1: $"The requested table [{CurrentTableName}] was loaded successfully, but ZERO records were returned,\nThe Table is  " , clr1: "Black0" ,
                        line2: $"The command line used was" , clr2: "Red2" ,
                        line3: $"{SqlCommand . ToUpper ( )}" , clr3: "Blue4" ,
                        header: "Generic style data table" , clr4: "Red5" );
                }
                generics = genaccts;
            }
        }

        #endregion Load data for the 3 different Db's

        // Just Assign data to grids to display it
        #region Load Viewer for the 3 different Db's
        private void LoadGrid_IAN1 ( string type = "" )
        {
            // Load whatever data we have received into DataGrid
            if ( CurrentTableName . ToUpper ( ) == "BANKACCOUNT" )
            {
                if ( bankaccts == null )
                    return;
                if ( type == "VIEW" )
                {
                    listView . ItemsSource = bankaccts;
                    dGrid . ItemsSource = bankaccts;
                    dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                    HandleCaption ( "VIEW" , bankaccts . Count );
                    listView . Focus ( );
                }
                else if ( type == "BOX" )
                {
                    listBox . ItemsSource = bankaccts;
                    dGrid . ItemsSource = bankaccts;
                    dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                    HandleCaption ( "BOX" , bankaccts . Count );
                    listBox . Focus ( );
                }
                else
                {
                    listView . ItemsSource = bankaccts;
                    listBox . ItemsSource = bankaccts;
                    dGrid . ItemsSource = bankaccts;
                    dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                    HandleCaption ( "" , bankaccts . Count );
                    listBox . Focus ( );
                    listView . Focus ( );
                }
                if ( UseFlowdoc )
                    ShowInfo ( Flowdoc , canvas , line1: $"The requested table [{CurrentTableName}] was loaded successfully, and the {DbCountlb} records returned are displayed in the table below" , clr1: "Black0" ,
                    line2: $"The command line used was" , clr2: "Red2" ,
                    line3: $"{SqlCommand . ToUpper ( )}" , clr3: "Blue4" ,
                    header: "Bank Accounts data table" , clr4: "Red5" );
            }
            else if ( CurrentTableName . ToUpper ( ) == "CUSTOMER" )
            {
                if ( custaccts == null )
                    return;
                if ( type == "VIEW" )
                {
                    listView . ItemsSource = custaccts;
                    dGrid . ItemsSource = custaccts;
                    dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                    HandleCaption ( "VIEW" , custaccts . Count );
                    listView . SelectedIndex = 0;
                    listView . Focus ( );
                }
                else if ( type == "BOX" )
                {
                    listBox . ItemsSource = custaccts;
                    dGrid . ItemsSource = custaccts;
                    dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                    HandleCaption ( "BOX" , custaccts . Count );
                    listBox . SelectedIndex = 0;
                }
                else
                {
                    listView . ItemsSource = custaccts;
                    listBox . ItemsSource = custaccts;
                    dGrid . ItemsSource = custaccts;
                    dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                    HandleCaption ( "" , custaccts . Count );
                    listBox . SelectedIndex = 0;
                    listView . SelectedIndex = 0;
                }
                listBox . SelectedIndex = 0;
                listBox . Focus ( );
            }
            else if ( CurrentTableName . ToUpper ( ) == "SECACCOUNTS" )
            {
                if ( detaccts == null )
                    return;
                if ( type == "VIEW" )
                {
                    listView . ItemsSource = detaccts;
                    dGrid . ItemsSource = detaccts;
                    dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                    HandleCaption ( "VIEW" , detaccts . Count );
                }
                else if ( type == "BOX" )
                {
                    listBox . ItemsSource = detaccts;
                    dGrid . ItemsSource = detaccts;
                    dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                    HandleCaption ( "BOX" , detaccts . Count );
                    listBox . SelectedIndex = 0;
                }
                else
                {
                    listView . ItemsSource = detaccts;
                    listBox . ItemsSource = detaccts;
                    dGrid . ItemsSource = detaccts;
                    dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                    HandleCaption ( "" , detaccts . Count );
                    listBox . SelectedIndex = 0;
                    listView . SelectedIndex = 0;
                    listView . Focus ( );
                }
                if ( UseFlowdoc )
                    ShowInfo ( Flowdoc , canvas , line1: $"The requested table [{CurrentTableName}] was loaded successfully, and the {DbCountlb} records returned are displayed in the table below" , clr1: "Black0" ,
                    line2: $"The command line used was" , clr2: "Red2" ,
                    line3: $"{SqlCommand . ToUpper ( )}" , clr3: "Blue4" ,
                    header: "Secondary Accounts data table" );
                listBox . Focus ( );
            }
            else
            {
                if ( genaccts . Count > 0 )
                {
                    //Generic with >= 1 records
                    if ( type == "VIEW" )
                    {
                        listView . ItemsSource = genaccts;
                        SqlServerCommands . LoadActiveRowsOnlyInGrid ( dGrid , genaccts , SqlServerCommands . GetGenericColumnCount ( genaccts ) );
                        if ( Flags . ReplaceFldNames )
                        {
                            GenericDbUtilities . ReplaceDataGridFldNames ( CurrentTableName , ref dGrid );
                        }
                        dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                        HandleCaption ( "VIEW" , genaccts . Count );
                        listView . SelectedIndex = 0;
                    }
                    else if ( type == "BOX" )
                    {
                        listBox . ItemsSource = genaccts;
                        SqlServerCommands . LoadActiveRowsOnlyInGrid ( dGrid , genaccts , SqlServerCommands . GetGenericColumnCount ( genaccts ) );
                        if ( Flags . ReplaceFldNames )
                        {
                            GenericDbUtilities . ReplaceDataGridFldNames ( CurrentTableName , ref dGrid );
                        }
                        dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                        HandleCaption ( "BOX" , genaccts . Count );
                        listBox . SelectedIndex = 0;
                        listBox . Focus ( );
                    }
                    else
                    {
                        listView . ItemsSource = genaccts;
                        listBox . ItemsSource = genaccts;
                        SqlServerCommands . LoadActiveRowsOnlyInGrid ( dGrid , genaccts , SqlServerCommands . GetGenericColumnCount ( genaccts ) );
                        if ( Flags . ReplaceFldNames )
                        {
                            GenericDbUtilities . ReplaceDataGridFldNames ( CurrentTableName , ref dGrid );
                        }
                        dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                        HandleCaption ( "" , genaccts . Count );
                        listBox . SelectedIndex = 0;
                        listView . SelectedIndex = 0;
                    }
                    listBox . Refresh ( );
                    return;
                }
                else
                {
                    // Empty  Generic table
                    if ( type == "VIEW" )
                    {
                        listView . ItemsSource = genaccts;
                        SqlServerCommands . LoadActiveRowsOnlyInGrid ( dGrid , genaccts , SqlServerCommands . GetGenericColumnCount ( genaccts ) );
                        if ( Flags . ReplaceFldNames )
                        {
                            GenericDbUtilities . ReplaceDataGridFldNames ( CurrentTableName , ref dGrid );
                        }
                        dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                        HandleCaption ( "VIEW" , genaccts . Count );
                    }
                    else if ( type == "BOX" )
                    {
                        listBox . ItemsSource = genaccts;
                        SqlServerCommands . LoadActiveRowsOnlyInGrid ( dGrid , genaccts , SqlServerCommands . GetGenericColumnCount ( genaccts ) );
                        if ( Flags . ReplaceFldNames )
                        {
                            GenericDbUtilities . ReplaceDataGridFldNames ( CurrentTableName , ref dGrid );
                        }
                        dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                        HandleCaption ( "BOX" , genaccts . Count );
                    }
                    else
                    {
                        //Generic table type
                        listView . ItemsSource = genaccts;
                        listBox . ItemsSource = genaccts;
                        SqlServerCommands . LoadActiveRowsOnlyInGrid ( dGrid , genaccts , SqlServerCommands . GetGenericColumnCount ( genaccts ) );
                        if ( Flags . ReplaceFldNames )
                        {
                            GenericDbUtilities . ReplaceDataGridFldNames ( CurrentTableName , ref dGrid );
                        }
                        dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                        HandleCaption ( "" , genaccts . Count );
                    }
                    if ( UseFlowdoc )
                        fdl . ShowInfo ( Flowdoc , canvas , header: "Unrecognised table accessed successfully" , clr4: "Red5" ,
                        line1: $"Request made was completed succesfully!" , clr1: "Red3" ,
                        line2: $"the table [{CurrentTableName}] that was queried returned a record count of {genaccts . Count}.\nThe structure of this data is not recognised, so a generic structure has been used..." ,
                        line3: $"{SqlCommand . ToUpper ( )}" , clr3: "Blue4"
                        );
                }
            }
            ShowLoadtime ( );
        }
        private void LoadGrid_NORTHWIND ( string type = "" )
        {
            // Load whatever data we have received into DataGrid
            if ( CurrentTableName . ToUpper ( ) == "CUSTOMERS" )
            {
                if ( nwcustomeraccts == null )
                    return;
                if ( type == "VIEW" )
                {
                    listView . ItemsSource = nwcustomeraccts;
                    if ( nwcustomeraccts . Count == 0 )
                        return;
                    dGrid . ItemsSource = nwcustomeraccts;
                    dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                    HandleCaption ( "VIEW" , nwcustomeraccts . Count );
                    listView . SelectedIndex = 0;
                }
                else if ( type == "BOX" )
                {
                    listBox . ItemsSource = nwcustomeraccts;
                    if ( nwcustomeraccts . Count == 0 )
                        return;
                    dGrid . ItemsSource = nwcustomeraccts;
                    dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                    HandleCaption ( "BOX" , nwcustomeraccts . Count );
                    listBox . SelectedIndex = 0;
                }
                else
                {
                    listView . ItemsSource = nwcustomeraccts;
                    if ( nwcustomeraccts . Count == 0 )
                        return;
                    listBox . ItemsSource = nwcustomeraccts;
                    dGrid . ItemsSource = nwcustomeraccts;
                    dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                    HandleCaption ( "" , nwcustomeraccts . Count );
                    listBox . SelectedIndex = 0;
                    listView . SelectedIndex = 0;
                }
                if ( UseFlowdoc )
                    ShowInfo ( Flowdoc , canvas , line1: $"The requested table [{CurrentTableName}] was loaded successfully, and the {nwcustomeraccts . Count} records returned are displayed in the table below" , clr1: "Black0" ,
                    line2: $"The command line used was" , clr2: "Red2" ,
                    line3: $"{SqlCommand . ToUpper ( )}" , clr3: "Blue4" ,
                    header: "Bank Accounts data table" , clr4: "Red5" );
            }
            else if ( CurrentTableName . ToUpper ( ) == "ORDERS" )
            {
                if ( nworderaccts == null )
                    return;
                if ( type == "VIEW" )
                {
                    listView . ItemsSource = nworderaccts;
                    if ( nworderaccts . Count == 0 )
                        return;
                    dGrid . ItemsSource = nworderaccts;
                    dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                    HandleCaption ( "VIEW" , nworderaccts . Count );
                    listView . SelectedIndex = 0;
                }
                else if ( type == "BOX" )
                {
                    listBox . ItemsSource = nworderaccts;
                    if ( nworderaccts . Count == 0 )
                        return;
                    dGrid . ItemsSource = nworderaccts;
                    dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                    HandleCaption ( "BOX" , nworderaccts . Count );
                    listBox . SelectedIndex = 0;
                }
                else
                {
                    listView . ItemsSource = nworderaccts;
                    if ( nworderaccts . Count == 0 )
                        return;
                    listBox . ItemsSource = nworderaccts;
                    dGrid . ItemsSource = nworderaccts;
                    dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                    HandleCaption ( "" , nworderaccts . Count );
                    listBox . SelectedIndex = 0;
                    listView . SelectedIndex = 0;
                }
                listBox . SelectedIndex = 0;
                listBox . Focus ( );
            }
            else
            {
                if ( genaccts . Count > 0 )
                {
                    if ( type == "VIEW" )
                    {
                        listView . ItemsSource = genaccts;
                        SqlServerCommands . LoadActiveRowsOnlyInGrid ( dGrid , genaccts , SqlServerCommands . GetGenericColumnCount ( genaccts ) );
                        if ( Flags . ReplaceFldNames )
                        {
                            GenericDbUtilities . ReplaceDataGridFldNames ( CurrentTableName , ref dGrid );
                        }
                        dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                        HandleCaption ( "VIEW" , genaccts . Count );
                        listView . SelectedIndex = 0;
                    }
                    else if ( type == "BOX" )
                    {
                        listBox . ItemsSource = genaccts;
                        SqlServerCommands . LoadActiveRowsOnlyInGrid ( dGrid , genaccts , SqlServerCommands . GetGenericColumnCount ( genaccts ) );
                        if ( Flags . ReplaceFldNames )
                        {
                            GenericDbUtilities . ReplaceDataGridFldNames ( CurrentTableName , ref dGrid );
                        }
                        dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                        HandleCaption ( "BOX" , genaccts . Count );
                        listBox . SelectedIndex = 0;
                    }
                    else
                    {
                        listView . ItemsSource = genaccts;
                        listBox . ItemsSource = genaccts;
                        SqlServerCommands . LoadActiveRowsOnlyInGrid ( dGrid , genaccts , SqlServerCommands . GetGenericColumnCount ( genaccts ) );
                        if ( Flags . ReplaceFldNames )
                        {
                            GenericDbUtilities . ReplaceDataGridFldNames ( CurrentTableName , ref dGrid );
                        }
                        dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                        HandleCaption ( "" , genaccts . Count );
                        listBox . SelectedIndex = 0;
                        listView . SelectedIndex = 0;
                    }
                    listBox . Refresh ( );
                    return;
                }
                else
                {
                    if ( type == "VIEW" )
                    {
                        // Caution : This loads the data into the DataGrid with only the selected rows
                        // //visible in the grid so do NOT repopulate the grid after making this call
                        //SqlSupport . LoadActiveRowsOnlyInLView ( listView , genaccts , DapperSupport . GetGenericColumnCount ( genaccts ) );
                        listView . ItemsSource = genaccts;
                        SqlServerCommands . LoadActiveRowsOnlyInGrid ( dGrid , genaccts , SqlServerCommands . GetGenericColumnCount ( genaccts ) );
                        if ( Flags . ReplaceFldNames )
                        {
                            GenericDbUtilities . ReplaceDataGridFldNames ( CurrentTableName , ref dGrid );
                        }
                        dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                        if ( genaccts . Count == 0 )
                            return;
                        listView . SelectedIndex = 0;
                        listView . Focus ( );
                        HandleCaption ( "VIEW" , genaccts . Count );
                        listView . SelectedIndex = 0;
                    }
                    else if ( type == "BOX" )
                    {
                        listBox . ItemsSource = genaccts;
                        if ( genaccts . Count == 0 )
                            return;


                        // Caution : This loads the data into the Datarid with only the selected rows
                        // //visible in the grid so do NOT repopulate the grid after making this call
                        SqlServerCommands . LoadActiveRowsOnlyInGrid ( dGrid , genaccts , SqlServerCommands . GetGenericColumnCount ( genaccts ) );
                        if ( Flags . ReplaceFldNames )
                        {
                            GenericDbUtilities . ReplaceDataGridFldNames ( CurrentTableName , ref dGrid );
                        }
                        dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                        listBox . SelectedIndex = 0;
                        HandleCaption ( "BOX" , genaccts . Count );
                    }
                    else
                    {
                        //SqlSupport . LoadActiveRowsOnlyInLView ( listView , genaccts , DapperSupport . GetGenericColumnCount ( genaccts ) );
                        listView . ItemsSource = genaccts;
                        if ( genaccts . Count == 0 )
                            return;
                        listBox . ItemsSource = genaccts;
                        SqlServerCommands . LoadActiveRowsOnlyInGrid ( dGrid , genaccts , SqlServerCommands . GetGenericColumnCount ( genaccts ) );
                        if ( Flags . ReplaceFldNames )
                        {
                            GenericDbUtilities . ReplaceDataGridFldNames ( CurrentTableName , ref dGrid );
                        }
                        dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                        listView . SelectedIndex = 0;
                        HandleCaption ( "" , genaccts . Count );
                        listBox . SelectedIndex = 0;
                    }
                }
                if ( UseFlowdoc )
                    ShowInfo ( Flowdoc , canvas , header: "Unrecognised table accessed successfully" , clr4: "Red5" ,
                        line1: $"Request made was completed succesfully!" , clr1: "Red3" ,
                        line2: $"the table [{CurrentTableName}] that was queried returned a record count of {genaccts . Count}.\nThe structure of this data is not recognised, so a generic structure has been used..." ,
                        line3: $"{SqlCommand . ToUpper ( )}" , clr3: "Blue4"
                        );

                ShowLoadtime ( );
            }
        }
        private void LoadGrid_PUBS ( string type = "" )
        {
            // Load whatever data we have received into DataGrid
            if ( CurrentTableName . ToUpper ( ) == "AUTHORS" )
            {
                if ( pubauthors == null )
                    return;
                if ( type == "VIEW" )
                {
                    listView . ItemsSource = pubauthors;
                    if ( pubauthors . Count == 0 )
                        return;
                    dGrid . ItemsSource = pubauthors;
                    dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                    HandleCaption ( "VIEW" , pubauthors . Count );
                    FrameworkElement elemnt = listView as FrameworkElement;
                }
                else if ( type == "BOX" )
                {
                    listBox . ItemsSource = pubauthors;
                    if ( pubauthors . Count == 0 )
                        return;
                    dGrid . ItemsSource = pubauthors;
                    dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                    HandleCaption ( "BOX" , pubauthors . Count );
                    listBox . SelectedIndex = 0;
                }
                else
                {
                    listView . ItemsSource = pubauthors;
                    if ( pubauthors . Count == 0 )
                        return;
                    listBox . ItemsSource = pubauthors;
                    dGrid . ItemsSource = pubauthors;
                    dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                    HandleCaption ( "" , pubauthors . Count );
                    listBox . SelectedIndex = 0;
                    listView . SelectedIndex = 0;
                }
                if ( UseFlowdoc )
                    ShowInfo ( Flowdoc , canvas , line1: $"The requested table [{CurrentTableName}] was loaded successfully, and the {pubauthors . Count} records returned are displayed in the table below" , clr1: "Black0" ,
                    line2: $"The command line used was" , clr2: "Red2" ,
                    line3: $"{SqlCommand . ToUpper ( )}" , clr3: "Blue4" ,
                    header: "Bank Accounts data table" , clr4: "Red5" );
            }
            else if ( CurrentTableName . ToUpper ( ) == "ORDERS" )
            {
                if ( nworderaccts == null )
                    return;
                if ( type == "VIEW" )
                {
                    listView . ItemsSource = pubauthors;
                    if ( pubauthors . Count == 0 )
                        return;
                    HandleCaption ( "VIEW" , pubauthors . Count );
                    listView . SelectedIndex = 0;
                }
                else if ( type == "BOX" )
                {
                    listBox . ItemsSource = pubauthors;
                    if ( pubauthors . Count == 0 )
                        return;
                    dGrid . ItemsSource = pubauthors;
                    dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                    FrameworkElement elemnt = listBox as FrameworkElement;
                    HandleCaption ( "BOX" , pubauthors . Count );
                    listBox . SelectedIndex = 0;
                }
                else
                {
                    listView . ItemsSource = pubauthors;
                    if ( pubauthors . Count == 0 )
                        return;
                    listBox . ItemsSource = pubauthors;
                    dGrid . ItemsSource = pubauthors;
                    dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                    HandleCaption ( "" , pubauthors . Count );
                    listBox . SelectedIndex = 0;
                    listView . SelectedIndex = 0;
                }
                listBox . SelectedIndex = 0;
                listBox . Focus ( );
            }
            else
            {
                if ( genaccts . Count > 0 )
                {
                    if ( type == "VIEW" )
                    {
                        listView . ItemsSource = genaccts;
                        if ( genaccts . Count == 0 )
                            return;
                        SqlServerCommands . LoadActiveRowsOnlyInGrid ( dGrid , genaccts , SqlServerCommands . GetGenericColumnCount ( genaccts ) );
                        if ( Flags . ReplaceFldNames )
                        {
                            GenericDbUtilities . ReplaceDataGridFldNames ( CurrentTableName , ref dGrid );
                        }
                        dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                        HandleCaption ( "VIEW" , genaccts . Count );
                        listView . SelectedIndex = 0;
                    }
                    else if ( type == "BOX" )
                    {
                        listBox . ItemsSource = genaccts;
                        if ( genaccts . Count == 0 )
                            return;
                        SqlServerCommands . LoadActiveRowsOnlyInGrid ( dGrid , genaccts , SqlServerCommands . GetGenericColumnCount ( genaccts ) );
                        if ( Flags . ReplaceFldNames )
                        {
                            GenericDbUtilities . ReplaceDataGridFldNames ( CurrentTableName , ref dGrid );
                        }
                        dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                        HandleCaption ( "BOX" , genaccts . Count );
                        listBox . SelectedIndex = 0;
                    }
                    else
                    {
                        if ( type == "VIEW" )
                        {
                            // Caution : This loads the data into the DataGrid with only the selected rows
                            // //visible in the grid so do NOT repopulate the grid after making this call
                            //SqlSupport . LoadActiveRowsOnlyInLView ( listView , genaccts , DapperSupport . GetGenericColumnCount ( genaccts ) );
                            listView . ItemsSource = genaccts;
                            if ( genaccts . Count == 0 )
                                return;
                            SqlServerCommands . LoadActiveRowsOnlyInGrid ( dGrid , genaccts , SqlServerCommands . GetGenericColumnCount ( genaccts ) );
                            if ( Flags . ReplaceFldNames )
                            {
                                GenericDbUtilities . ReplaceDataGridFldNames ( CurrentTableName , ref dGrid );
                            }
                            dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                            listView . SelectedIndex = 0;
                            HandleCaption ( "" , genaccts . Count );
                        }
                        else if ( type == "BOX" )
                        {
                            // Caution : This loads the data into the Datarid with only the selected rows
                            // //visible in the grid so do NOT repopulate the grid after making this call
                            listBox . ItemsSource = genaccts;
                            if ( genaccts . Count == 0 )
                                return;
                            SqlServerCommands . LoadActiveRowsOnlyInGrid ( dGrid , genaccts , SqlServerCommands . GetGenericColumnCount ( genaccts ) );
                            if ( Flags . ReplaceFldNames )
                            {
                                GenericDbUtilities . ReplaceDataGridFldNames ( CurrentTableName , ref dGrid );
                            }
                            dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                            listBox . SelectedIndex = 0;
                            HandleCaption ( "BOX" , genaccts . Count );
                        }
                        else
                        {
                            //SqlSupport . LoadActiveRowsOnlyInLView ( listView , genaccts , DapperSupport . GetGenericColumnCount ( genaccts ) );
                            listView . ItemsSource = genaccts;
                            if ( genaccts . Count == 0 )
                                return;
                            listBox . ItemsSource = genaccts;
                            SqlServerCommands . LoadActiveRowsOnlyInGrid ( dGrid , genaccts , SqlServerCommands . GetGenericColumnCount ( genaccts ) );
                            if ( Flags . ReplaceFldNames )
                            {
                                GenericDbUtilities . ReplaceDataGridFldNames ( CurrentTableName , ref dGrid );
                            }
                            dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                            listView . SelectedIndex = 0;
                            HandleCaption ( "" , genaccts . Count );
                            listBox . SelectedIndex = 0;
                        }
                        if ( UseFlowdoc )
                            ShowInfo ( Flowdoc , canvas , header: "Unrecognised table accessed successfully" , clr4: "Red5" ,
                            line1: $"Request made was completed succesfully!" , clr1: "Red3" ,
                            line2: $"the table [{CurrentTableName}] that was queried returned a record count of {genaccts . Count}.\nThe structure of this data is not recognised, so a generic structure has been used..." ,
                            line3: $"{SqlCommand . ToUpper ( )}" , clr3: "Blue4"
                            );
                    }
                    ShowLoadtime ( );
                }
                else
                {
                    // no genaccts = its empty
                    DbCountlv = genaccts . Count;
                    if ( type == "VIEW" )
                    {
                        listView . ItemsSource = genaccts;
                        SqlServerCommands . LoadActiveRowsOnlyInGrid ( dGrid , genaccts , SqlServerCommands . GetGenericColumnCount ( genaccts ) );
                        if ( Flags . ReplaceFldNames )
                        {
                            GenericDbUtilities . ReplaceDataGridFldNames ( CurrentTableName , ref dGrid );
                        }
                        dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                        lvHeader . Text = $"List View Display :  No records returned...";
                    }
                    else if ( type == "BOX" )
                    {
                        listBox . ItemsSource = genaccts;
                        SqlServerCommands . LoadActiveRowsOnlyInGrid ( dGrid , genaccts , SqlServerCommands . GetGenericColumnCount ( genaccts ) );
                        if ( Flags . ReplaceFldNames )
                        {
                            GenericDbUtilities . ReplaceDataGridFldNames ( CurrentTableName , ref dGrid );
                        }
                        dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                        lbHeader . Text = $"List Box Display :  No records returned...";
                        dGridHeader . Text = $"List Box Display :  No records returned...";
                    }
                    else
                    {
                        listView . ItemsSource = genaccts;
                        listBox . ItemsSource = genaccts;
                        SqlServerCommands . LoadActiveRowsOnlyInGrid ( dGrid , genaccts , SqlServerCommands . GetGenericColumnCount ( genaccts ) );
                        if ( Flags . ReplaceFldNames )
                        {
                            GenericDbUtilities . ReplaceDataGridFldNames ( CurrentTableName , ref dGrid );
                        }
                        dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                        lvHeader . Text = $"List View Display :  No records returned...";
                        listBox . ItemsSource = genaccts;
                        lbHeader . Text = $"List Box Display :  No records returned...";
                        dGridHeader . Text = $"List Box Display :  No records returned...";
                    }
                }
            }
        }
        #endregion Load Viewer for the 3 different Db's

        // Load list of databases in MSSQL
        private void LoadAllDbNames ( )
        {
            int bankindex = 0, count = 0;
            List<string> list = new List<string> ( );
            SqlCommand = "spGetAllDatabaseNames";
            Datagrids . CallStoredProcedure ( list , SqlCommand );
            //This call returns us a DataTable
            DataTable dt = DataLoadControl . GetDataTable ( SqlCommand );
            // This how to access  Row data from  a grid the easiest way.... parsed into a List <xxxxx>
            list = WpfLib1 . Utils . GetDataDridRowsAsListOfStrings ( dt );
            foreach ( string row in list )
            {
                string entry = row . ToUpper ( );
                // ONLY ALLOW THE TRHEE MAIN dB'S WE ARE LIKELY TO USE.
                if ( entry . Contains ( "IAN" ) || entry . Contains ( "NORTHWIND" ) || entry . Contains ( "PUBS" ) )
                {
                    DbMain . Items . Add ( row );
                    if ( row . ToUpper ( ) . Contains ( "IAN1" ) )
                        bankindex = count;
                    count++;
                }
            }
            // how to Sort Combo/Listbox contents
            DbMain . Items . SortDescriptions . Add ( new SortDescription ( "" , ListSortDirection . Ascending ) );
            Startup = true;
            DbMain . SelectedIndex = bankindex;
            Startup = false;
            SqlCommand = DefaultSqlCommand;
            CanMagnify = true;
            // Save a List of ALL DBs so we can add  to our treeview 
            //databaseList = list;
        }

        //Reload  contents of table names on right click
        private void dbName_PreviewMouseRightButtonUp ( object sender , MouseButtonEventArgs e )
        {
            if ( LoadDbTables ( DbMain . SelectedItem . ToString ( ) ) == true )
            {
                if ( UseFlowdoc )
                    ShowInfo ( Flowdoc , canvas , header: "List of Tables in current Db reloaded successfully" , clr4: "Red5" ,
                    line1: $"Request made was completed succesfully!" , clr1: "Red3"
                    );
            }
            else
            {
                if ( UseFlowdoc )
                    ShowInfo ( Flowdoc , canvas , header: "List of Tables in current Db could not be reloaded" , clr4: "Red5" ,
                    line1: $"Request failed...!" , clr1: "Red3"
                    );
            }
        }

        private void ResetViewers ( string type )
        {
            // clear data collections
            if ( CurrentTableName == "BANKACCOUNT" )
                bankaccts = null;
            else if ( CurrentTableName == "CUSTOMER" )
                custaccts = null;
            else if ( CurrentTableName == "SECACCOUNTS" )
                detaccts = null;
            else if ( CurrentTableName == "GENERICTABLE" )
                genaccts = null;
        }
        private string GetCurrentDatabase ( )
        {
            string result = "";
            result = DbMain . SelectedItem . ToString ( );
            return result . ToUpper ( );
        }

        #region Load data templates
        private void LoadDataTemplates_Ian1 ( string type , string viewertype )
        {
            if ( viewertype == "VIEW" )
            {
                DataTemplatesLv . Items . Clear ( );
            }
            else if ( viewertype == "BOX" )
            {
                DataTemplatesLb . Items . Clear ( );
            }
            else
            {
                DataTemplatesLv . Items . Clear ( );
                DataTemplatesLb . Items . Clear ( );
            }

            if ( type == "BANKACCOUNT" )
            {
                if ( viewertype == "VIEW" )
                {
                    DataTemplatesLv . Items . Add ( "BankDataTemplate1" );
                    DataTemplatesLv . Items . Add ( "BankDataTemplate2" );
                    DataTemplatesLv . SelectedIndex = 0;
                    DataTemplatesLv . SelectedItem = 0;
                }
                else if ( viewertype == "BOX" )
                {
                    DataTemplatesLb . Items . Add ( "BankDataTemplate1" );
                    DataTemplatesLb . Items . Add ( "BankDataTemplate2" );
                    DataTemplatesLb . SelectedIndex = 0;
                    DataTemplatesLb . SelectedItem = 0;
                }
                else
                {
                    DataTemplatesLv . Items . Add ( "BankDataTemplate1" );
                    DataTemplatesLv . Items . Add ( "BankDataTemplate2" );
                    DataTemplatesLv . SelectedIndex = 0;
                    DataTemplatesLv . SelectedItem = 0;
                    DataTemplatesLb . Items . Add ( "BankDataTemplate1" );
                    DataTemplatesLb . Items . Add ( "BankDataTemplate2" );
                    DataTemplatesLb . SelectedIndex = 0;
                    DataTemplatesLb . SelectedItem = 0;
                }

            }
            else if ( type == "CUSTOMER" )
            {
                if ( viewertype == "VIEW" )
                {
                    DataTemplatesLv . Items . Add ( "CustomersDbTemplate1" );
                    DataTemplatesLv . Items . Add ( "CustomersDbTemplate2" );
                    DataTemplatesLv . SelectedIndex = 0;
                    DataTemplatesLv . SelectedItem = 0;
                }
                else if ( viewertype == "BOX" )
                {
                    DataTemplatesLb . Items . Add ( "CustomersDbTemplate1" );
                    DataTemplatesLb . Items . Add ( "CustomersDbTemplate2" );
                    DataTemplatesLb . SelectedIndex = 0;
                    DataTemplatesLb . SelectedItem = 0;
                }
                else
                {
                    DataTemplatesLv . Items . Add ( "CustomersDbTemplate1" );
                    DataTemplatesLv . Items . Add ( "CustomersDbTemplate2" );
                    DataTemplatesLv . SelectedIndex = 0;
                    DataTemplatesLv . SelectedItem = 0;
                    DataTemplatesLb . Items . Add ( "CustomersDbTemplate1" );
                    DataTemplatesLb . Items . Add ( "CustomersDbTemplate2" );
                    DataTemplatesLb . SelectedIndex = 0;
                    DataTemplatesLb . SelectedItem = 0;
                }
            }
            else if ( type == "SECACCOUNTS" )
            {
                DataTemplatesLv . Items . Add ( "DetailsDataTemplate1" );
                DataTemplatesLv . Items . Add ( "DetailsDataTemplate2" );
                DataTemplatesLv . SelectedIndex = 0;
                DataTemplatesLv . SelectedItem = 0;
                DataTemplatesLb . Items . Add ( "DetailsDataTemplate1" );
                DataTemplatesLb . Items . Add ( "DetailsDataTemplate2" );
                DataTemplatesLb . SelectedIndex = 0;
                DataTemplatesLb . SelectedItem = 0;
            }
            else
            {
                if ( viewertype == "VIEW" )
                {
                    DataTemplatesLv . Items . Add ( "GenDataTemplate1" );
                    DataTemplatesLv . Items . Add ( "GenDataTemplate2" );
                    DataTemplatesLv . SelectedIndex = 0;
                    DataTemplatesLv . SelectedItem = 0;
                }
                else if ( viewertype == "BOX" )
                {
                    DataTemplatesLb . Items . Add ( "GenDataTemplate1" );
                    DataTemplatesLb . Items . Add ( "GenDataTemplate2" );
                    DataTemplatesLb . SelectedIndex = 0;
                    DataTemplatesLb . SelectedItem = 0;
                }
                else
                {
                    DataTemplatesLv . Items . Add ( "GenDataTemplate1" );
                    DataTemplatesLv . Items . Add ( "GenDataTemplate2" );
                    DataTemplatesLv . SelectedIndex = 0;
                    DataTemplatesLv . SelectedItem = 0;
                    DataTemplatesLb . Items . Add ( "GenDataTemplate1" );
                    DataTemplatesLb . Items . Add ( "GenDataTemplate2" );
                    DataTemplatesLb . SelectedIndex = 0;
                    DataTemplatesLb . SelectedItem = 0;
                }
            }
        }
        private void LoadDataTemplates_NorthWind ( string type , string viewertype )
        {
            if ( viewertype == "VIEW" )
            {
                DataTemplatesLv . Items . Clear ( );
            }
            else if ( viewertype == "BOX" )
            {
                DataTemplatesLb . Items . Clear ( );
            }
            else
            {
                DataTemplatesLv . Items . Clear ( );
                DataTemplatesLb . Items . Clear ( );
            }
            if ( type == "CUSTOMERS" )
            {
                if ( viewertype == "VIEW" )
                {
                    DataTemplatesLv . Items . Add ( "NwCustomersDataTemplate1" );
                    DataTemplatesLv . Items . Add ( "NwCustomersDataTemplate3" );
                    DataTemplatesLv . Items . Add ( "NwCustomersDataTemplate5" );
                    DataTemplatesLv . Items . Add ( "NwCustomersLVDataTemplate5" );
                }
                else if ( viewertype == "BOX" )
                {
                    DataTemplatesLv . Items . Add ( "NwCustomersDataTemplate1" );
                    DataTemplatesLv . Items . Add ( "NwCustomersDataTemplate3" );
                    DataTemplatesLv . Items . Add ( "NwCustomersDataTemplate5" );
                    DataTemplatesLv . Items . Add ( "NwCustomersLVDataTemplate5" );
                }
                else
                {
                    DataTemplatesLv . Items . Add ( "NwCustomersDataTemplate1" );
                    DataTemplatesLv . Items . Add ( "NwCustomersDataTemplate3" );
                    DataTemplatesLv . Items . Add ( "NwCustomersDataTemplate5" );
                    DataTemplatesLv . Items . Add ( "NwCustomersLVDataTemplate5" );
                    DataTemplatesLb . Items . Add ( "NwCustomersDataTemplate1" );
                    DataTemplatesLb . Items . Add ( "NwCustomersDataTemplate3" );
                    DataTemplatesLb . Items . Add ( "NwCustomersDataTemplate5" );
                    DataTemplatesLb . Items . Add ( "NwCustomersLVDataTemplate5" );
                }
            }
            else if ( type == "ORDERS" )
            {
                if ( viewertype == "VIEW" )
                {
                    DataTemplatesLv . Items . Add ( "NwordersComplexTemplate1" );
                    DataTemplatesLv . Items . Add ( "NwordersDataTemplate1" );
                    DataTemplatesLv . Items . Add ( "NwordersDataTemplate2" );
                    DataTemplatesLv . Items . Add ( "NwordersDataTemplate4" );
                    DataTemplatesLv . Items . Add ( "NwOrdersDataGridTemplate1" );
                }
                else if ( viewertype == "BOX" )
                {
                    DataTemplatesLb . Items . Add ( "NwordersComplexTemplate1" );
                    DataTemplatesLb . Items . Add ( "NwordersDataTemplate1" );
                    DataTemplatesLb . Items . Add ( "NwordersDataTemplate2" );
                    DataTemplatesLb . Items . Add ( "NwordersDataTemplate4" );
                    DataTemplatesLb . Items . Add ( "NwOrdersDataGridTemplate1" );
                }
                else
                {
                    DataTemplatesLv . Items . Add ( "NwordersComplexTemplate1" );
                    DataTemplatesLv . Items . Add ( "NwordersDataTemplate1" );
                    DataTemplatesLv . Items . Add ( "NwordersDataTemplate2" );
                    DataTemplatesLv . Items . Add ( "NwordersDataTemplate4" );
                    DataTemplatesLv . Items . Add ( "NwOrdersDataGridTemplate1" );
                    DataTemplatesLb . Items . Add ( "NwordersComplexTemplate1" );
                    DataTemplatesLb . Items . Add ( "NwordersDataTemplate1" );
                    DataTemplatesLb . Items . Add ( "NwordersDataTemplate2" );
                    DataTemplatesLb . Items . Add ( "NwordersDataTemplate4" );
                    DataTemplatesLb . Items . Add ( "NwOrdersDataGridTemplate1" );
                }
            }
            else
            {
                //Generic   type
                if ( viewertype == "VIEW" )
                {
                    DataTemplatesLv . Items . Add ( "GenDataTemplate1" );
                    DataTemplatesLv . Items . Add ( "GenDataTemplate2" );
                }
                else if ( viewertype == "BOX" )
                {
                    DataTemplatesLb . Items . Add ( "GenDataTemplate1" );
                    DataTemplatesLb . Items . Add ( "GenDataTemplate2" );
                }
                else
                {
                    DataTemplatesLv . Items . Add ( "GenDataTemplate1" );
                    DataTemplatesLv . Items . Add ( "GenDataTemplate2" );
                    DataTemplatesLb . Items . Add ( "GenDataTemplate1" );
                    DataTemplatesLb . Items . Add ( "GenDataTemplate2" );
                }
            }
            if ( viewertype == "VIEW" )
            {
                DataTemplatesLv . SelectedIndex = 0;
                DataTemplatesLv . SelectedItem = 0;
            }
            else if ( viewertype == "BOX" )
            {
                DataTemplatesLb . SelectedIndex = 0;
                DataTemplatesLb . SelectedItem = 0;
            }
            else
            {
                DataTemplatesLv . SelectedIndex = 0;
                DataTemplatesLv . SelectedItem = 0;
                DataTemplatesLb . SelectedIndex = 0;
                DataTemplatesLb . SelectedItem = 0;
            }
        }
        private void LoadDataTemplates_PubAuthors ( string type , string viewertype )
        {
            if ( viewertype == "VIEW" )
            {
                DataTemplatesLv . Items . Clear ( );
            }
            else if ( viewertype == "BOX" )
            {
                DataTemplatesLb . Items . Clear ( );
            }
            else
            {
                DataTemplatesLv . Items . Clear ( );
                DataTemplatesLb . Items . Clear ( );
            }
            if ( type == "AUTHORS" )
            {
                if ( viewertype == "VIEW" )
                {
                    DataTemplatesLv . Items . Add ( "PubsAuthorTemplate1" );
                    DataTemplatesLv . Items . Add ( "PubsAuthorTemplate2" );
                }
                else if ( viewertype == "BOX" )
                {
                    DataTemplatesLb . Items . Add ( "PubsAuthorTemplate1" );
                    DataTemplatesLb . Items . Add ( "PubsAuthorTemplate2" );
                }
                else
                {
                    DataTemplatesLv . Items . Add ( "PubsAuthorTemplate1" );
                    DataTemplatesLv . Items . Add ( "PubsAuthorTemplate2" );
                    DataTemplatesLb . Items . Add ( "PubsAuthorTemplate1" );
                    DataTemplatesLb . Items . Add ( "PubsAuthorTemplate2" );
                }
            }
            else
            {
                //Generic   type
                if ( viewertype == "VIEW" )
                {
                    DataTemplatesLv . Items . Add ( "GenDataTemplate1" );
                    DataTemplatesLv . Items . Add ( "GenDataTemplate2" );
                }
                else if ( viewertype == "BOX" )
                {
                    DataTemplatesLb . Items . Add ( "GenDataTemplate1" );
                    DataTemplatesLb . Items . Add ( "GenDataTemplate2" );
                }
                else
                {
                    DataTemplatesLv . Items . Add ( "GenDataTemplate1" );
                    DataTemplatesLv . Items . Add ( "GenDataTemplate2" );
                    DataTemplatesLb . Items . Add ( "GenDataTemplate1" );
                    DataTemplatesLb . Items . Add ( "GenDataTemplate2" );
                }
            }
            if ( viewertype == "VIEW" )
            {
                DataTemplatesLv . SelectedIndex = 0;
                DataTemplatesLv . SelectedItem = 0;
            }
            else if ( viewertype == "BOX" )
            {
                DataTemplatesLb . SelectedIndex = 0;
                DataTemplatesLb . SelectedItem = 0;
            }
            else
            {
                DataTemplatesLv . SelectedIndex = 0;
                DataTemplatesLv . SelectedItem = 0;
                DataTemplatesLb . SelectedIndex = 0;
                DataTemplatesLb . SelectedItem = 0;
            }
        }
        #endregion Load data templates

        #region ALL Combo box handlers
        private void LVDataTemplate_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {
            if ( DataTemplatesLv . Items . Count == 0 )
                return;
            string selection = DataTemplatesLv . SelectedItem . ToString ( );
            FrameworkElement elemnt = listView as FrameworkElement;
            listView . ItemTemplate = elemnt . FindResource ( selection ) as DataTemplate;
            listView . Refresh ( );
        }
        private void LBDataTemplate_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {
            if ( DataTemplatesLb . Items . Count == 0 )
                return;
            ComboBox cb = sender as ComboBox;
            string selection = cb . SelectedItem . ToString ( );
            FrameworkElement elemnt = listBox as FrameworkElement;
            listBox . ItemTemplate = elemnt . FindResource ( selection ) as DataTemplate;
            listBox . Refresh ( );
        }

        private void fontsize_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {
            // Font size changing
            ComboBox cb = sender as ComboBox;
            ComboBox cbrow = rowheight as ComboBox;
            if ( cb . SelectedItem == null )
                cb . SelectedIndex = 0;
            double fontsze = 0;
            fontsze = Convert . ToDouble ( cb . SelectedItem );
            double newitemheightrequired = 0;
            double currentItemHeight = 0;
            newitemheightrequired = Fontsize + 6;
            currentItemHeight = Convert . ToDouble ( GetValue ( ItemsHeightProperty ) );
            SetValue ( FontsizeProperty , fontsze );
            dGrid . FontSize = fontsze;
        }
        private void rowheight_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {
            // Row Height changing
            ComboBox cb = sender as ComboBox;
            if ( cb . SelectedItem == null )
                cb . SelectedIndex = 0;
            double rwheight = 0;
            rwheight = Convert . ToDouble ( cb . SelectedItem );
            ItemsHeight = rwheight;
            SetValue ( ItemsHeightProperty , rwheight );
            dGrid . RowHeight = rwheight;
        }

        private void SelectCurrentDbInCombo ( string dbname , string viewertype )
        {
            ComboBoxItem cbi = new ComboBoxItem ( );
            string entry = "";
            int indx = 0;
            for ( int x = 0 ; x < dbNameLv . Items . Count ; x++ )
            {
                entry = dbNameLv . Items [ x ] . ToString ( );
                if ( entry . ToUpper ( ) == dbname )
                    break;
                indx++;
            }
            ComboSelectionActive = true;
            if ( viewertype == "VIEW" )
                dbNameLv . SelectedIndex = indx;
            if ( viewertype == "BOX" )
                dbNameLb . SelectedIndex = indx;
            else
            {
                dbNameLb . SelectedIndex = indx;
                dbNameLv . SelectedIndex = indx;
            }

            ComboSelectionActive = false;
        }
        private void dbNameLb_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {
#pragma warning disable CS0219 // The variable 'ResultString' is assigned but its value is never used
            string ResultString = "";
#pragma warning restore CS0219 // The variable 'ResultString' is assigned but its value is never used
            if ( alldone )
                return;
            if ( ComboSelectionActive || dbNameLb . Items . Count == 0 )
                return;
            string tablename = dbNameLb . SelectedItem . ToString ( );
            SqlCommand = $"Select *from {tablename}";
            CurrentTableName = tablename . ToUpper ( );
        }
        private void dbNameLv_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {
#pragma warning disable CS0219 // The variable 'ResultString' is assigned but its value is never used
            string ResultString = "";
#pragma warning restore CS0219 // The variable 'ResultString' is assigned but its value is never used
            if ( alldone )
                return;
            if ( ComboSelectionActive || dbNameLv . Items . Count == 0 )
                return;
            string tablename = dbNameLv . SelectedItem . ToString ( );
            SqlCommand = $"Select *from {tablename}";
            CurrentTableName = tablename . ToUpper ( );
        }

        #endregion ALL Combo box handlers

        #region NorthWind	 UNUSED customer load
        //NorthWind methods    - UNUSED
        private ObservableCollection<nwcustomer> LoadNwCustomers ( string type )
        {
            if ( Startup )
                return null;
            // Set correct connection string
            if ( Utils . CheckResetDbConnection ( "NORTHWIND" , out string constr ) == false )
            {
                Debug . WriteLine ( "Failed to set connection string for NorthWind Db" );
                return null;
            }
            {
                OpenNorthWindDb ( );
                nwcustomeraccts = nwc . GetNwCustomers ( );
                listView . ItemsSource = nwcustomeraccts;
                dGrid . ItemsSource = nwcustomeraccts;
                FrameworkElement elemnt = listView as FrameworkElement;
                listView . ItemTemplate = elemnt . FindResource ( "NwCustomersDataTemplate1" ) as DataTemplate;
                dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                lvHeader . Text = $"List View Display : {dbNameLv?.SelectedItem?.ToString ( ) . ToUpper ( )}";
                listView . SelectedIndex = 0;
                listView . Focus ( );
                listBox . ItemTemplate = elemnt . FindResource ( "NwCustomersDataTemplate1" ) as DataTemplate;
                listBox . ItemsSource = nwcustomeraccts;
                lbHeader . Text = $"List Box Display : {dbNameLb?.SelectedItem?.ToString ( ) . ToUpper ( )}";
                dGridHeader . Text = $"DataGrid Display : {dbNameLb?.SelectedItem?.ToString ( ) . ToUpper ( )} Records = {nwcustomeraccts . Count}";
                listBox . SelectedIndex = 0;
                listBox . Focus ( );
                DbCountlb = nwcustomeraccts . Count;
                DbCountlv = nwcustomeraccts . Count;
                return nwcustomeraccts;
            }
        }
        #endregion NorthWind

        #region Make main Db Connectiions
        private bool OpenIan1Db ( )
        {
            //Set Sql Connection string up first
            if ( Utils . CheckResetDbConnection ( "IAN1" , out string constr ) == false )
            {
                Debug . WriteLine ( "Failed to set connection string for Ian1 Db" );
                return false;
            }
            // Open the Ian1 Db first off
            SqlCommand = "spOpenDb_Ian1";
            if ( SqlSupport . Executestoredproc ( SqlCommand , Flags . CurrentConnectionString ) == false )
            {
                Debug . WriteLine ( $"Stored procedure {SqlCommand} Failed to open IAN1.MDF" );
                WpfLib1 . Utils . DoErrorBeep ( 250 , 75 , 1 );
                SqlCommand = DefaultSqlCommand;
                return false;
            }
            SqlCommand = DefaultSqlCommand;
            return true;
        }
        private void OpenNorthWindDb ( )
        {
            //Set Sql Connectoin string up first
            if ( Utils . CheckResetDbConnection ( "NORTHWIND" , out string constr ) == false )
            {
                Debug . WriteLine ( "Failed to set connection string for NorthWind Db" );
                return;
            }
            // Open the NorthWind Db first off
            SqlCommand = "spOpenDb_NorthWind";
            SqlSupport . Executestoredproc ( SqlCommand , Flags . CurrentConnectionString );
            // now load list of tabels in Northwind Db into dbMain Combo
            LoadDbTables ( "NORTHWIND" );
            SqlCommand = DefaultSqlCommand;
        }
        private void OpenPublishers ( )
        {
            //Set Sql Connectoin string up first
            if ( Utils . CheckResetDbConnection ( "PUBS" , out string constr ) == false )
            {
                Debug . WriteLine ( "Failed to set connection string for Adventure WorksDb" );
                return;
            }
            // Open the Adventureworks Db first off
            SqlCommand = "spOpenDb_Publishers";
            SqlSupport . Executestoredproc ( SqlCommand , Flags . CurrentConnectionString );
            // now load list of tabels in Northwind Db
            LoadDbTables ( "PUBS" );
            SqlCommand = "Select * fom Authors order by au_fname";
        }
        #endregion Make main Db Connectiions

        #region  Load Current Db's Tables lists
        //Get list of all Tables in our Db (Ian1.MDF)
        public void LoadTablesList_Ian1 ( )
        {
            int bankindex = 0, count = 0;
            List<string> list = new List<string> ( );
            dbNameLv . Items . Clear ( );
            dbNameLb . Items . Clear ( );
            SqlCommand = "spGetTablesList";
            Datagrids . CallStoredProcedure ( list , SqlCommand );
            //This call returns us a DataTable
            DataTable dt = DataLoadControl . GetDataTable ( SqlCommand );
            // This how to access  Row data from  a grid the easiest way.... parsed into a List <xxxxx>
            list = WpfLib1 . Utils . GetDataDridRowsAsListOfStrings ( dt );
            foreach ( string row in list )
            {
                dbNameLb . Items . Add ( row );
                dbNameLv . Items . Add ( row );
                if ( row . ToUpper ( ) == "BANKACCOUNT" )
                    bankindex = count;
                count++;
            }
            // how to Sort Combo/Listbox contents
            dbNameLb . Items . SortDescriptions . Add ( new SortDescription ( "" , ListSortDirection . Ascending ) );
            dbNameLb . SelectedIndex = bankindex;
            dbNameLv . Items . SortDescriptions . Add ( new SortDescription ( "" , ListSortDirection . Ascending ) );
            dbNameLv . SelectedIndex = bankindex;
            SqlCommand = DefaultSqlCommand;
        }
        #endregion  Load Current Db's Tables lists

        #region Load List of Tables in current Db

        //Get list of all Tables in currently selected Db 
        public bool LoadDbTables ( string DbName )
        {
            int listindex = 0, count = 0;
            List<string> list = new List<string> ( );
            DbName = DbName . ToUpper ( );
            if ( Utils . CheckResetDbConnection ( DbName , out string constr ) == false )
            {
                Debug . WriteLine ( $"Failed to set connection string for {DbName} Db" );
                return false;
            }
            // All Db's have their own version of this SP.....
            SqlCommand = "spGetTablesList";

            Datagrids . CallStoredProcedure ( list , SqlCommand );
            //This call returns us a DataTable
            DataTable dt = DataLoadControl . GetDataTable ( SqlCommand );
            // This how to access Row data from  a grid the easiest way.... parsed into a List <xxxxx>
            if ( dt != null )
            {
                dbNameLb . Items . Clear ( );
                dbNameLv . Items . Clear ( );
                list = WpfLib1 . Utils . GetDataDridRowsAsListOfStrings ( dt );
                if ( DbName == "NORTHWIND" )
                {
                    foreach ( string row in list )
                    {
                        dbNameLb . Items . Add ( row );
                        dbNameLv . Items . Add ( row );
                        if ( row . ToUpper ( ) == CurrentTableName )
                            listindex = count;
                        count++;
                    }
                }
                else if ( DbName == "IAN1" )
                {
                    foreach ( string row in list )
                    {
                        dbNameLb . Items . Add ( row );
                        dbNameLv . Items . Add ( row );
                        if ( row . ToUpper ( ) == CurrentTableName )
                            listindex = count;
                        count++;
                    }
                }
                else if ( DbName == "PUBS" )
                {
                    foreach ( string row in list )
                    {
                        dbNameLb . Items . Add ( row );
                        dbNameLv . Items . Add ( row );
                        if ( row . ToUpper ( ) == CurrentTableName )
                            listindex = count;
                        count++;
                    }
                }

                //// add ALL DBs to our treeview list of databases
                ////SqlTables sqlt = new SqlTables();
                //foreach ( string row in list )
                //{
                //	SqlTables sqlt= new SqlTables();
                //	sqlt . tablename = row;
                //	SqlTableCollection . Add ( sqlt );
                //}
                //DbTablesTree . ItemsSource = SqlTableCollection;


                // how to Sort Combo/Listbox contents
                //dbNameLv . Items . SortDescriptions . Add ( new SortDescription ( "" , ListSortDirection . Ascending ) );
                alldone = true;
                dbNameLb . SelectedIndex = listindex;
                dbNameLv . SelectedIndex = listindex;
                alldone = false;
                if ( count > 0 )
                    return true;
                else
                    return false;
            }
            else
            {
                MessageBox . Show ( $"SQL comand {SqlCommand} Failed..." );
                WpfLib1 . Utils . DoErrorBeep ( 125 , 55 , 1 );
                return false;
            }
#pragma warning disable CS0162 // Unreachable code detected
            return true;
#pragma warning restore CS0162 // Unreachable code detected
            //SqlCommand = DefaultSqlCommand;
        }

        #endregion Load List of Tables in current Db

        #region FlowDoc support
        private void ShowInfo ( FlowDoc Flowdoc , Canvas canvas , string line1 = "" , string clr1 = "" , string line2 = "" , string clr2 = "" , string line3 = "" , string clr3 = "" , string header = "" , string clr4 = "" , bool beep = false )
        {
            Flowdoc . ShowInfo ( Flowdoc , canvas , line1 , clr1 , line2 , clr2 , line3 , clr3 , header , clr4 , beep );
            //if ( UseFlowdoc == false )
            //	return;
            //if ( UseFlowdocBeep == false )
            //	beep = false;
            //Flowdoc . ShowInfo ( line1 , clr1 , line2 , clr2 , line3 , clr3 , header , clr4 , beep );
            //canvas . Visibility = Visibility . Visible;
            //canvas . BringIntoView ( );
            //Flowdoc . Visibility = Visibility . Visible;

            //if ( Flowdoc . KeepSize == false )
            //{
            //	if ( Flowdoc . Height != flowdocFloatingHeight )
            //		flowdocFloatingHeight = Flowdoc . Height;
            //}
            //var docheight = Convert.ToDouble ( flowdocFloatingHeight == 0 ? 250 : flowdocFloatingHeight);
            //var docwidth = Convert.ToDouble ( flowdocFloatingWidth == 0 ? 450 : flowdocFloatingWidth);
            //// Save properties
            //flowdocFloatingHeight = docheight;
            //flowdocFloatingWidth = docwidth;
            //flowdocFloatingTop = Convert . ToDouble ( Flowdoc . GetValue ( TopProperty ) );
            //flowdocFloatingLeft = Convert . ToDouble ( Flowdoc . GetValue ( LeftProperty ) );
            ////Position Control on Canvas
            //double canvasheight = canvas.ActualHeight;
            //if ( docheight >= canvasheight )
            //	Flowdoc . Height = canvasheight - 20;
            //// Set size of Control
            //Flowdoc . Width = flowdocFloatingWidth;
            //Flowdoc . Height = flowdocFloatingHeight;

            //Flowdoc . BringIntoView ( );
            //if ( Flags . PinToBorder == true )
            //{
            //	( Flowdoc as FrameworkElement ) . SetValue ( Canvas . LeftProperty , ( double ) 0 );
            //	( Flowdoc as FrameworkElement ) . SetValue ( Canvas . TopProperty , ( double ) 0 );
            //}
        }

        #region  Dependency properties
        public double Fontsize
        {
            get
            {
                return ( double ) GetValue ( FontsizeProperty );
            }
            set
            {
                SetValue ( FontsizeProperty , value );
            }
        }
        public static readonly DependencyProperty FontsizeProperty =
            DependencyProperty . Register ( "Fontsize" , typeof ( double ) , typeof ( Listviews ) , new PropertyMetadata ( ( double ) 12 ) );

        public double ItemsHeight
        {
            get
            {
                return ( double ) GetValue ( ItemsHeightProperty );
            }
            set
            {
                SetValue ( ItemsHeightProperty , value );
            }
        }
        public static readonly DependencyProperty ItemsHeightProperty =
            DependencyProperty . Register ( "ItemsHeight" , typeof ( double ) , typeof ( Listviews ) , new PropertyMetadata ( ( double ) 20 ) );

        #endregion  Dependency properties


        #endregion FlowDoc support

        #region  initialize data
        public void LoadDefaultSqlCommands ( )
        {
            if ( DefaultSqlCommands . Count > 0 )
                return;
            DefaultSqlCommands . Add ( "BANKACCOUNT" , "Select * from BankAccount" );
            DefaultSqlCommands . Add ( "CUSTOMER" , "Select * from Customer" );
            DefaultSqlCommands . Add ( "SECACCOUNTS" , "Select * from secAccounts" );
            DefaultSqlCommands . Add ( "NORTHWIND" , "Select * from Customers" );
            DefaultSqlCommands . Add ( "AUTHORS" , "Select * from Authors" );
            //DefaultSqlCommands . Add ( "AW.SALES.CREDITCARD" , "Select * from Sales.CreditCard" );
            //DefaultSqlCommands . Add ( "AW.SALES.SALESPERSON" , "Select * from Sales.Salesperson" );
            //DefaultSqlCommands . Add ( "AW.SALES.SALESTERRITORY" , "Select * from Sales.SalesTerritory" );
            //DefaultSqlCommands . Add ( "AW.PRODUCTION.PRODUCTREVIEW" , "Select * from Production.ProductReview" );
            //DefaultSqlCommands . Add ( "AW.PRODUCTION.PRODUCTPHOTO" , "Select * from Production.ProductPhoto" );
        }                 // Set up the connection string fo rthe approriate Db

        #endregion  initialize data

        #region utility  support methods
        private void ShowLoadtime ( )
        {
            if ( Usetimer )
            {
                timer . Stop ( );
                if ( timer . ElapsedMilliseconds != 0 )
                    LoadTime . Text = timer . ElapsedMilliseconds . ToString ( ) + " m/secs";
                timer . Reset ( );
            }
        }
        #endregion utility  support methods

        #region Database switching
        /// <summary>
        ///----------------------------------------------------------------------------------//
        ///  Switching DataBases, so gotta change all other lookup tables
        ///----------------------------------------------------------------------------------//
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dbMain_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {
            if ( Startup )
                return;
            if ( DbMain . Items . Count == 0 )
                return;
            // clear down viewers 1st
            listView . ItemsSource = null;
            listBox . ItemsSource = null;
            listView . Refresh ( );
            listBox . Refresh ( );
            ComboBox cb = sender as ComboBox;
            string selection = cb . SelectedItem . ToString ( );
            if ( selection . ToUpper ( ) == "IAN1" )
            {
                // initial access of this Db, so load BankAccount table
                if ( Utils . CheckResetDbConnection ( "IAN1" , out string constr ) == false )
                {
                    Debug . WriteLine ( "Failed to set connection string for IAN1 Db" );
                    return;
                }
                // used to access Dictionary of DataTemplates
                OpenIan1Db ( );
                CurrentDbName = "IAN1";
                LoadDbTables ( CurrentDbName );
                CurrentTableName = "BANKACCOUNT";
                SelectCurrentDbInCombo ( "BANKACCOUNT" , "" );
                LoadDataTemplates_Ian1 ( CurrentTableName , "VIEW" );
                DefaultSqlCommand = "Select * from Bankaccount";
                SqlCommand = DefaultSqlCommand;
                CurrentDataTable = DataTemplatesLv . Items [ 0 ] . ToString ( );

                //Now setup the UI as needed
                listView . ItemsSource = null;
                // Load  the data from the Table via SQL
                LoadData_Ian1 ( "VIEW" );

                FrameworkElement elemnt = listView as FrameworkElement;
                listView . ItemTemplate = elemnt . FindResource ( CurrentDataTable ) as DataTemplate;
                listView . ItemsSource = bankaccts;
                dGrid . ItemsSource = bankaccts;
                dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                lvHeader . Text = $"List View Display : {DbMain . SelectedItem . ToString ( )} : {dbNameLv?.SelectedItem?.ToString ( ) . ToUpper ( )}";
                listView . SelectedIndex = 0;
                listView . Focus ( );
                listBox . ItemTemplate = elemnt . FindResource ( CurrentDataTable ) as DataTemplate;
                listBox . ItemsSource = bankaccts;
                lbHeader . Text = $"List Box Display : {DbMain . SelectedItem . ToString ( )} : {dbNameLb?.SelectedItem?.ToString ( ) . ToUpper ( )}";
                dGridHeader . Text = $"DataGrid Display : {DbMain . SelectedItem . ToString ( )} : {dbNameLb?.SelectedItem?.ToString ( ) . ToUpper ( )} Records = {bankaccts . Count}";
                listBox . SelectedIndex = 0;
                DbCountlb = bankaccts . Count;
                DbCountlv = bankaccts . Count;
            }
            else if ( selection . ToUpper ( ) == "NORTHWIND" )
            {
                // Just open the New DB
                if ( Utils . CheckResetDbConnection ( "NORTHWIND" , out string constr ) == false )
                {
                    Debug . WriteLine ( "Failed to set connection string for NorthWind Db" );
                    return;
                }
                OpenNorthWindDb ( );
                CurrentDbName = "NORTHWIND";
                CurrentTableName = "CUSTOMERS";
                LoadDbTables ( CurrentDbName );

                LoadDataTemplates_NorthWind ( CurrentTableName , "" );
                SelectCurrentDbInCombo ( CurrentTableName , "" );
                DefaultSqlCommand = "Select * from Customers";
                SqlCommand = DefaultSqlCommand;
                listView . ItemsSource = null;
                // Load  the data from the Table via SQL
                nwcustomeraccts = nwc . GetNwCustomers ( );

                //Now setup the UI as needed
                FrameworkElement elemnt = listView as FrameworkElement;
                listView . ItemTemplate = elemnt . FindResource ( "NwCustomersDataTemplate1" ) as DataTemplate;
                listView . ItemsSource = nwcustomeraccts;
                dGrid . ItemsSource = nwcustomeraccts;
                dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                lvHeader . Text = $"List View Display : {DbMain . SelectedItem . ToString ( )} : {dbNameLv?.SelectedItem?.ToString ( ) . ToUpper ( )}";
                listView . SelectedIndex = 0;
                listView . Focus ( );
                listBox . ItemTemplate = elemnt . FindResource ( "NwCustomersDataTemplate1" ) as DataTemplate;
                listBox . ItemsSource = nwcustomeraccts;
                lbHeader . Text = $"List Box Display : {DbMain . SelectedItem . ToString ( )} : {dbNameLb?.SelectedItem?.ToString ( ) . ToUpper ( )}";
                dGridHeader . Text = $"DataGrid Display : {DbMain . SelectedItem . ToString ( )} : {dbNameLb?.SelectedItem?.ToString ( ) . ToUpper ( )} Records = {nwcustomeraccts . Count}";
                listBox . SelectedIndex = 0;
                listBox . Focus ( );
                DbCountlb = nwcustomeraccts . Count;
                DbCountlv = nwcustomeraccts . Count;
            }
            else if ( selection . ToUpper ( ) == "PUBS" )
            {
                listView . ItemsSource = null;
                OpenPublishers ( );
                CurrentDbName = "PUBS";
                CurrentTableName = "AUTHORS";

                LoadDataTemplates_PubAuthors ( CurrentTableName , "" );

                SelectCurrentDbInCombo ( CurrentTableName , "" );
                DefaultSqlCommand = "Select * from Authors ";
                SqlCommand = DefaultSqlCommand;
                listView . ItemsSource = null;
                // Load  the data from the Table via SQL
                pubauthors = PubAuthors . LoadPubAuthors ( pubauthors , false );

                //Now setup the UI as needed
                FrameworkElement elemnt = listView as FrameworkElement;
                listView . ItemTemplate = elemnt . FindResource ( "PubsAuthorTemplate1" ) as DataTemplate;
                listView . ItemsSource = pubauthors;
                dGrid . ItemsSource = pubauthors;
                dGrid . SelectedItem = dGrid . SelectedIndex = 0;

                lvHeader . Text = $"List View Display : {DbMain . SelectedItem . ToString ( )} : {dbNameLv?.SelectedItem?.ToString ( ) . ToUpper ( )}";
                listView . SelectedIndex = 0;
                listView . Focus ( );
                listBox . ItemTemplate = elemnt . FindResource ( "PubsAuthorTemplate1" ) as DataTemplate;
                listBox . ItemsSource = pubauthors;
                lbHeader . Text = $"List Box Display : {DbMain . SelectedItem . ToString ( )} : {dbNameLb?.SelectedItem?.ToString ( ) . ToUpper ( )}";
                dGridHeader . Text = $"DataGrid Display : {DbMain . SelectedItem . ToString ( )} : {dbNameLb?.SelectedItem?.ToString ( ) . ToUpper ( )} Records = {pubauthors . Count}";
                listBox . SelectedIndex = 0;
                listBox . Focus ( );
                DbCountlb = pubauthors . Count;
                DbCountlv = pubauthors . Count;
            }
        }

        #endregion Databse switching

        #region  Reload Data viewers
        private void ReloadListview ( object sender , RoutedEventArgs e )
        {
            ResetViewers ( "VIEW" );
            listView . ItemsSource = null;
            DbCountlv = 0;
            listView . Refresh ( );
            // Set flag  to ignore limits check
            LoadAll = true;
            string currdb = GetCurrentDatabase ( );
            CurrentTableName = dbNameLv . SelectedItem . ToString ( ) . ToUpper ( );
            // This resets the current database connection - should be used anywhere that We switch between databases in Sql Server
            if ( Utils . CheckResetDbConnection ( DbMain . SelectedItem . ToString ( ) , out string constring ) == false )
            {
                Debug . WriteLine ( $"Failed to set connection string for {CurrentTableName . ToUpper ( )} Db" );
                return;
            }
            if ( currdb == "IAN1" )
            {
                LoadData_Ian1 ( "VIEW" );
                LoadGrid_IAN1 ( "VIEW" );
            }
            else if ( currdb == "NORTHWIND" )
            {
                LoadData_NorthWind ( "VIEW" );
                LoadGrid_NORTHWIND ( "VIEW" );
            }
            else if ( currdb == "PUBS" )
            {
                genaccts = null;
                LoadData_Publishers ( "VIEW" , out genaccts );
                if ( genaccts != null )
                {
                    listView . ItemsSource = genaccts;
                    SqlServerCommands . LoadActiveRowsOnlyInGrid ( dGrid , genaccts , SqlServerCommands . GetGenericColumnCount ( genaccts ) );
                    if ( Flags . ReplaceFldNames )
                    {
                        GenericDbUtilities . ReplaceDataGridFldNames ( CurrentTableName , ref dGrid );
                    }
                    dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                    DbCountlv = genaccts . Count;
                }
                else
                {
                    pubauthors = PubAuthors . LoadPubAuthors ( pubauthors , false );
                    listView . ItemsSource = pubauthors;
                    dGrid . ItemsSource = pubauthors;
                    dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                    DbCountlv = pubauthors . Count;
                }
                LoadGrid_PUBS ( "VIEW" );
            }
            listView . SelectedIndex = 0;
            listView . SelectedItem = 0;
            // Clear flag again
            LoadAll = false;
        }

        private void ReloadListbox ( object sender , RoutedEventArgs e )
        {
            ResetViewers ( "BOX" );
            listBox . ItemsSource = null;
            listBox . Refresh ( );
            DbCountlb = 0;
            // Set flag  to ignore limits check
            LoadAll = true;
            string currdb = GetCurrentDatabase ( );
            CurrentTableName = dbNameLb . SelectedItem . ToString ( ) . ToUpper ( );
            if ( Utils . CheckResetDbConnection ( DbMain . SelectedItem . ToString ( ) , out string constr ) == false )
            {
                Debug . WriteLine ( $"Failed to set connection string for {CurrentTableName . ToUpper ( )} Db" );
                return;
            }

            if ( currdb == "IAN1" )
            {
                LoadData_Ian1 ( "BOX" );
                LoadGrid_IAN1 ( "BOX" );
            }
            else if ( currdb == "NORTHWIND" )
            {
                LoadData_NorthWind ( "BOX" );
                LoadGrid_NORTHWIND ( "BOX" );
            }
            else if ( currdb == "PUBS" )
            {
                genaccts = null;
                LoadData_Publishers ( "BOX" , out genaccts );
                if ( genaccts != null )
                {
                    listBox . ItemsSource = genaccts;
                    SqlServerCommands . LoadActiveRowsOnlyInGrid ( dGrid , genaccts , SqlServerCommands . GetGenericColumnCount ( genaccts ) );
                    if ( Flags . ReplaceFldNames )
                    {
                        GenericDbUtilities . ReplaceDataGridFldNames ( CurrentTableName , ref dGrid );
                    }
                    dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                    DbCountlb = genaccts . Count;
                }
                else
                {
                    pubauthors = PubAuthors . LoadPubAuthors ( pubauthors , false );
                    listBox . ItemsSource = pubauthors;
                    dGrid . ItemsSource = pubauthors;
                    dGrid . SelectedItem = dGrid . SelectedIndex = 0;
                    DbCountlb = pubauthors . Count;
                }
                LoadGrid_PUBS ( "BOX" );
            }
            listBox . SelectedIndex = 0;
            listBox . SelectedItem = 0;
            LoadAll = false;
        }
        #endregion  Reload Data viewers
        private string GetDefaultSqlCommand ( string CurrentType )
        {
            string result = "";
            foreach ( KeyValuePair<string , string> pair in DefaultSqlCommands )
            {
                if ( pair . Key == CurrentType . ToUpper ( ) )
                {
                    result = pair . Value;
                    SqlCommand = result;
                    break;
                }
            }
            return result;
        }

        #region Checkboxes
        private void ShowInfo_Click ( object sender , RoutedEventArgs e )
        {
            UseFlowdoc = !UseFlowdoc;
        }

        private void scrollview_Click ( object sender , RoutedEventArgs e )
        {
            UseScrollViewer = !UseScrollViewer;
            Flags . UseScrollView = UseScrollViewer;
        }
        #endregion Checkboxes

        private void ViewTableColumnsLb ( object sender , RoutedEventArgs e )
        {
            bool flowdocswitch = false;
#pragma warning disable CS0219 // The variable 'count' is assigned but its value is never used
            int count = 0;
#pragma warning restore CS0219 // The variable 'count' is assigned but its value is never used
            List<string> list = new List<string> ( );
            List<string> fldnameslist = new List<string> ( );
            string output = "";
            SqlCommand = $"spGetTableColumnWithSize {dbNameLb . SelectedItem . ToString ( )}";
            //SqlCommand = SqlCommand = $"spGetTableColumns";
            fldnameslist = Datagrids . CallStoredProcedureWithSizes ( list , SqlCommand );

            output = WpfLib1 . Utils . ParseTableColumnData ( fldnameslist );

            // Fiddle  to allow Flowdoc  to show Field info even though Flowdoc use is disabled
            if ( UseFlowdoc == false )
            {
                flowdocswitch = true;
                UseFlowdoc = true;
            }
            //Debug. WriteLine ( $"loaded {count} records for table columns" );
            if ( UseFlowdoc )
                ShowInfo ( Flowdoc , canvas , header: "Table Columns informaton accessed successfully" , clr4: "Red5" ,
                line1: $"Request made was completed succesfully!" , clr1: "Red3" ,
                line2: $"the structure of the table [{dbNameLb . SelectedItem . ToString ( )}] is listed below : \n{output}" ,
                line3: $"Results created by Stored Procedure : \n({SqlCommand . ToUpper ( )})" , clr3: "Blue4"
                );
            if ( flowdocswitch == true )
            {
                flowdocswitch = false;
                UseFlowdoc = false;
            }
        }
        private void ViewTableColumnsLv ( object sender , RoutedEventArgs e )
        {
            bool flowdocswitch = false;
#pragma warning disable CS0219 // The variable 'count' is assigned but its value is never used
            int count = 0;
#pragma warning restore CS0219 // The variable 'count' is assigned but its value is never used
            List<string> list = new List<string> ( );
            string output = "";
            SqlCommand = $"spGetTableColumnWithSize {dbNameLv . SelectedItem . ToString ( )}";
            list = Datagrids . CallStoredProcedureWithSizes ( list , SqlCommand );
            output = WpfLib1 . Utils . ParseTableColumnData ( list );

            //This call returns us a DataTable
            DataTable dt = DataLoadControl . GetDataTable ( SqlCommand );
            // Fiddle  to allow Flowdoc  to show Field info even though Flowdoc use is disabled
            if ( UseFlowdoc == false )
            {
                flowdocswitch = true;
                UseFlowdoc = true;
            }
            if ( UseFlowdoc )
                ShowInfo ( Flowdoc , canvas , header: "Table Columns informaton accessed successfully" , clr4: "Red5" ,
                line1: $"Request made was completed succesfully!" , clr1: "Red3" ,
                line2: $"the structure of the table [{dbNameLv . SelectedItem . ToString ( )}] is listed below : \n{output}" ,
                line3: $"Results created by Stored Procedure : \n({SqlCommand . ToUpper ( )})" , clr3: "Blue4"
                );
            if ( flowdocswitch == true )
            {
                flowdocswitch = false;
                UseFlowdoc = false;
            }
        }

        // display relevant info when a different table is selected
        private void HandleCaption ( string viewertype , int reccount )
        {
            FrameworkElement elemnt;
            if ( viewertype == "VIEW" )
            {
                elemnt = listView as FrameworkElement;
                listView . ItemTemplate = elemnt . FindResource ( DataTemplatesLv . SelectedItem ) as DataTemplate;
                lvHeader . Text = $"List View Display : {DbMain . SelectedItem . ToString ( )} : {dbNameLv?.SelectedItem?.ToString ( ) . ToUpper ( )}";
                dGridHeader . Text = $"DataGrid Display : {DbMain . SelectedItem . ToString ( )} : {dbNameLv?.SelectedItem?.ToString ( ) . ToUpper ( )} Records = {reccount}";
                DbCountlv = reccount;
            }
            else if ( viewertype == "BOX" )
            {
                elemnt = listBox as FrameworkElement;
                listBox . ItemTemplate = elemnt . FindResource ( DataTemplatesLb?.SelectedItem ) as DataTemplate;
                lbHeader . Text = $"List Box Display : {DbMain . SelectedItem . ToString ( )} : {dbNameLb?.SelectedItem?.ToString ( ) . ToUpper ( )}";
                dGridHeader . Text = $"DataGrid Display : {DbMain . SelectedItem . ToString ( )} : {dbNameLb?.SelectedItem?.ToString ( ) . ToUpper ( )} Records = {reccount}";
                DbCountlb = reccount;
            }
            else
            {
                elemnt = listView as FrameworkElement;
                listView . ItemTemplate = elemnt . FindResource ( DataTemplatesLv?.SelectedItem ) as DataTemplate;
                lvHeader . Text = $"List View Display : {DbMain . SelectedItem . ToString ( )} : {dbNameLv?.SelectedItem?.ToString ( ) . ToUpper ( )}";
                dGridHeader . Text = $"DataGrid Display : {DbMain . SelectedItem . ToString ( )} : {dbNameLv?.SelectedItem?.ToString ( ) . ToUpper ( )} Records = {reccount}";
                DbCountlv = reccount;
                elemnt = listBox as FrameworkElement;
                listBox . ItemTemplate = elemnt . FindResource ( DataTemplatesLb?.SelectedItem ) as DataTemplate;
                lbHeader . Text = $"List Box Display : {DbMain . SelectedItem . ToString ( )} : {dbNameLb?.SelectedItem?.ToString ( ) . ToUpper ( )}";
                dGridHeader . Text = $"DataGrid Display : {DbMain . SelectedItem . ToString ( )} : {dbNameLb?.SelectedItem?.ToString ( ) . ToUpper ( )} Records = {reccount}";
                DbCountlb = reccount;
            }
        }


        private void Park_Click ( object sender , RoutedEventArgs e )
        {
            Flags . PinToBorder = !Flags . PinToBorder;
        }

        #region Event handler rom ColorPicker button - save to clipboard
        //private void Colorpicker_ExecuteSaveToClipboardMethod ( object sender , ColorpickerArgs e )
        //{
        //    Clipboard . SetText ( e . RgbString );
        //}
        #endregion Event handler from ColorPicker button - save to clipboard

        #region Move ColorPicker
        private void PickColors_MouseLeftButtonUp ( object sender , MouseButtonEventArgs e )
        {
            //ColorpickerObject = null;
        }
        private void PickColors_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
        {
            //In this event, we get current mouse position on the control to use it in the MouseMove event.
            //if ( PickColors . RedSlider . IsMouseOver == true
            //    || PickColors . GreenSlider . IsMouseOver == true
            //    || PickColors . BlueSlider . IsMouseOver == true
            //    || PickColors . OpacitySlider . IsMouseOver == true
            //    || PickColors . ClipboardSave . IsMouseOver == true
            //    || PickColors . listbox . IsMouseOver == true )
            //{
            //    // Dont capture mouse in our cotrols
            //    return;
            //}
            //CpFirstXPos = e . GetPosition ( sender as Control ) . X;
            //CpFirstYPos = e . GetPosition ( sender as Control ) . Y;
            //double FirstArrowXPos = e . GetPosition ( ( sender as Control ) . Parent as Control ) . X - CpFirstXPos;
            //double FirstArrowYPos = e . GetPosition ( ( sender as Control ) . Parent as Control ) . Y - CpFirstYPos;
            ////ColorpickerObject = sender as Colorpicker;
        }
        private void PickColors_MouseMove ( object sender , MouseEventArgs e )
        {
            //// ColorpickerObject is an object pointer to the ColorPicker object so this only
            //// works if the mouse pointer is over the ColorPcker
            //if ( ColorpickerObject != null && e . LeftButton == MouseButtonState . Pressed )
            //{
            //    ///var v = e . Source;
            //    // Get mouse position IN PickColor !!
            //    double left = e . GetPosition ( ( ColorpickerObject as FrameworkElement ) . Parent as FrameworkElement ) . X - CpFirstXPos;
            //    double top = e . GetPosition ( ( ColorpickerObject as FrameworkElement ) . Parent as FrameworkElement ) . Y - CpFirstYPos;
            //    double trueleft = left - CpFirstXPos;
            //    double truetop = left - CpFirstYPos;
            //    if ( left >= 0 ) // && left <= canvas.ActualWidth - Flowdoc.ActualWidth)
            //        ( ColorpickerObject as FrameworkElement ) . SetValue ( Canvas . LeftProperty , left );
            //    if ( top >= 0 ) //&& top <= canvas . ActualHeight- Flowdoc. ActualHeight)
            //        ( ColorpickerObject as FrameworkElement ) . SetValue ( Canvas . TopProperty , top );
            //    //Debug. WriteLine ( $"left={left}, Top = {top}" );
            // }
        }
        #endregion Move ColorPicker
        private void ListViewWindow_Closed ( object sender , EventArgs e )
        {
            //Colorpicker . ExecuteSaveToClipboardMethod -= Colorpicker_ExecuteSaveToClipboardMethod;
            Flowdoc . ExecuteFlowDocMaxmizeMethod -= new EventHandler ( MaximizeFlowDoc );
            //			Flowdoc . ExecuteFlowDocResizeMethod -= Flowdoc_ExecuteFlowDocResizeMethod;
            Flowdoc . ExecuteFlowDocBorderMethod -= FlowDoc_ExecuteFlowDocBorderMethod;
            MainWindow.RemoveGenericlistboxcontrol (canvas );
        }
         private void LoadTreeData ( )
        {
            //SqlDatabases sqldb = new SqlDatabases();
            DatabasesCollection . Clear ( );
            DbTablesTree . ItemsSource = null;
            DbProcsTree . ItemsSource = null;
            DbTablesTree . Items . Clear ( );
            DbProcsTree . Items . Clear ( );
            SqlCommand = "spGetAllDatabaseNames";
            List<string> dblist = new List<string> ( );
            Datagrids . CallStoredProcedure ( dblist , SqlCommand );
            //This call returns us a DataTable
            DataTable dt = DataLoadControl . GetDataTable ( SqlCommand );
            // This how to access  Row data from  a grid the easiest way.... parsed into a List <xxxxx>
            dblist = WpfLib1 . Utils . GetDataDridRowsAsListOfStrings ( dt );

            var collection = DatabasesCollection;

            foreach ( string row in dblist )
            {
                //List<SqlTable> sqltable = new List<SqlTable>();
                // Now Handle list of tablenames
                if ( Utils . CheckResetDbConnection ( row , out string constr ) == false )
                {
                    Debug . WriteLine ( $"Failed to set connection string for {row . ToUpper ( )} Db" );
                    continue;
                }
                // All Db's have their own version of this SP.....
                SqlCommand = "spGetTablesList";

                List<string> tableslist = new List<string> ( );
                Datagrids . CallStoredProcedure ( tableslist , SqlCommand );
                //This call returns us a DataTable
                dt = DataLoadControl . GetDataTable ( SqlCommand );

                Database db = new Database ( );
                // This how to access Row data from  a grid the easiest way.... parsed into a List <xxxxx>
                if ( dt != null )
                {
                    db . Tables = new List<SqlTable> ( );
                    tableslist = WpfLib1 . Utils . GetDataDridRowsAsListOfStrings ( dt );
                    foreach ( string item in tableslist )
                    {
                        SqlTable sqlt = new SqlTable ( item );
                        sqlt . Tablename = item;
                        db . Tables . Add ( sqlt );
                        db . Databasename = row;
                    }
                    DatabasesCollection . Add ( db );
                }

                // All Db's have their own version of this SP.....
                SqlCommand = "spGetStoredProcs";

                List<string> procslist = new List<string> ( );
                Datagrids . CallStoredProcedure ( procslist , SqlCommand );
                //This call returns us a DataTable
                dt = DataLoadControl . GetDataTable ( SqlCommand );
                // This how to access Row data from  a grid the easiest way.... parsed into a List <xxxxx>
                if ( dt != null )
                {
                    //Database db = new Database();
                    db . Procedures = new List<SqlProcedures> ( );
                    procslist = WpfLib1 . Utils . GetDataDridRowsAsListOfStrings ( dt );
                    foreach ( string item in procslist )
                    {
                        SqlProcedures sqlprocs = new SqlProcedures ( item );
                        sqlprocs . Procname = item;
                        db . Procedures . Add ( sqlprocs );
                    }
                    // Duplicates all entries !!!
                    //DatabasesCollection . Add ( db );
                }

            }
            DbTablesTree . ItemsSource = DatabasesCollection;
            DbProcsTree . ItemsSource = DatabasesCollection;

        }

        #region  Dependency properties for listbox

        #region ItemBackground
        public Brush ItemBackground
        {
            get
            {
                return ( Brush ) GetValue ( ItemBackgroundProperty );
            }
            set
            {
                SetValue ( ItemBackgroundProperty , value );
            }
        }

        public static readonly DependencyProperty ItemBackgroundProperty =
                DependencyProperty . Register ( "ItemBackground" , typeof ( Brush ) , typeof ( Listviews ) , new PropertyMetadata ( Brushes . LightBlue ) );
        #endregion ItemBackground

        #region ItemForeground
        public Brush ItemForeground
        {
            get
            {
                return ( Brush ) GetValue ( ItemForegroundProperty );
            }
            set
            {
                SetValue ( ItemForegroundProperty , value );
            }
        }

        public static readonly DependencyProperty ItemForegroundProperty =
                DependencyProperty . Register ( "ItemForeground" , typeof ( Brush ) , typeof ( Listviews ) , new PropertyMetadata ( Brushes . Black ) );
        #endregion ItemForeground

        #region SelectedBackground
        public Brush SelectedBackground
        {
            get
            {
                return ( Brush ) GetValue ( SelectedBackgroundProperty );
            }
            set
            {
                SetValue ( SelectedBackgroundProperty , value );
            }
        }
        public static readonly DependencyProperty SelectedBackgroundProperty =
                DependencyProperty . Register ( "SelectedBackground" , typeof ( Brush ) , typeof ( Listviews ) , new PropertyMetadata ( Brushes . Red ) );
        #endregion SelectedBackground

        #region SelectedForeground
        public Brush SelectedForeground
        {
            get
            {
                return ( Brush ) GetValue ( SelectedForegroundProperty );
            }
            set
            {
                SetValue ( SelectedForegroundProperty , value );
            }
        }
        public static readonly DependencyProperty SelectedForegroundProperty =
            DependencyProperty . Register ( "SelectedForeground" , typeof ( Brush ) , typeof ( Listviews ) , new PropertyMetadata ( Brushes . White ) );
        #endregion SelectedForeground

        #region MouseoverBackground
        public Brush MouseoverBackground
        {
            get
            {
                return ( Brush ) GetValue ( MouseoverBackgroundProperty );
            }
            set
            {
                SetValue ( MouseoverBackgroundProperty , value );
            }
        }
        public static readonly DependencyProperty MouseoverBackgroundProperty =
                DependencyProperty . Register ( "MouseoverBackground" , typeof ( Brush ) , typeof ( Listviews ) , new PropertyMetadata ( Brushes . Blue ) );
        #endregion MouseoverBackground

        #region MouseoverForeground
        public Brush MouseoverForeground
        {
            get
            {
                return ( Brush ) GetValue ( MouseoverForegroundProperty );
            }
            set
            {
                SetValue ( MouseoverForegroundProperty , value );
            }
        }
        public static readonly DependencyProperty MouseoverForegroundProperty =
                DependencyProperty . Register ( "MouseoverForeground" , typeof ( Brush ) , typeof ( Listviews ) , new PropertyMetadata ( Brushes . White ) );
        #endregion MouseoverForeground

        #region MouseoverSelectedBackground
        public Brush MouseoverSelectedBackground
        {
            get
            {
                return ( Brush ) GetValue ( MouseoverSelectedBackgroundProperty );
            }
            set
            {
                SetValue ( MouseoverSelectedBackgroundProperty , value );
            }
        }
        public static readonly DependencyProperty MouseoverSelectedBackgroundProperty =
                DependencyProperty . Register ( "MouseoverSelectedBackground" , typeof ( Brush ) , typeof ( Listviews ) , new PropertyMetadata ( Brushes . Black ) );
        #endregion MouseoverSelectedBackground

        #region MouseoverSelectedForeground
        public Brush MouseoverSelectedForeground
        {
            get
            {
                return ( Brush ) GetValue ( MouseoverSelectedForegroundProperty );
            }
            set
            {
                SetValue ( MouseoverSelectedForegroundProperty , value );
            }
        }
        public static readonly DependencyProperty MouseoverSelectedForegroundProperty =
                DependencyProperty . Register ( "MouseoverSelectedForeground" , typeof ( Brush ) , typeof ( Listviews ) , new PropertyMetadata ( Brushes . White ) );

        #endregion MouseoverSelectedForeground

        public static bool GetIsNodeExpanded ( DependencyObject obj )
        {
            return ( bool ) obj . GetValue ( IsNodeExpandedProperty );
        }
        public static void SetIsNodeExpanded ( DependencyObject obj , bool value )
        {
            obj . SetValue ( IsNodeExpandedProperty , value );
        }
        public static readonly DependencyProperty IsNodeExpandedProperty =
            DependencyProperty . RegisterAttached ( "IsNodeExpanded" , typeof ( bool ) , typeof ( Listviews ) , new PropertyMetadata ( false ) );

        #endregion  Dependency properties for listbox

        private void DbMain_PreviewMouseRightButtonUp ( object sender , MouseButtonEventArgs e )
        {

        }

        private void TextBlock_PreviewMouseRightButtonUp ( object sender , MouseButtonEventArgs e )
        {
            // right click for S.P script
            TextBlock tb = sender as TextBlock;
            string selection = tb . Text;
#pragma warning disable CS0219 // The variable 'index' is assigned but its value is never used
            int index = 0;
#pragma warning restore CS0219 // The variable 'index' is assigned but its value is never used
            foreach ( var item in ProcsCollection )
            {
                if ( item . Procname == selection )
                {
                    item . IsSelected = true;
                    break;
                }
            }
        }

        private void DbProcsTree_PreviewMouseRightButtonUp ( object sender , MouseButtonEventArgs e )
        {
            // process right click to show the  full script in a FlowDoc viewer 
            if ( SqlSpCommand != "" && SqlSpCommand != null )
            {
                DataTable dt = new DataTable ( );
                string [ ] args = { "" , "" , "" , "" };
#pragma warning disable CS0219 // The variable 'err' is assigned but its value is never used
                string err = "", errormsg = "";
#pragma warning restore CS0219 // The variable 'err' is assigned but its value is never used
                List<string> list = new List<string> ( );
                ObservableCollection<GenericClass> Generics = new ObservableCollection<GenericClass> ( );
                foreach ( var item in DatabasesCollection )
                {
                    CurrentSPDb = item . Databasename;
                    if ( Utils . CheckResetDbConnection ( CurrentSPDb , out string constring ) == false )
                        return;

                    List<string> procslist = new List<string> ( );
                    ObservableCollection<BankAccountViewModel> bvmparam = new ObservableCollection<BankAccountViewModel> ( );
                    List<string> genericlist = new List<string> ( );
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
                    try
                    {
                        DapperSupport . CreateGenericCollection (
                            ref Generics ,
                            "spGetSpecificSchema  " ,
                            SqlSpCommand ,
                            "" ,
                            "" ,
                            ref genericlist ,
                            ref errormsg );
                        if ( Generics . Count > 0 )
                        {
                            break;
                        }
                    }
                    catch ( Exception ex )
                    {
                    }
#pragma warning restore CS0168 // The variable 'ex' is declared but never used

                }
                if ( Generics . Count == 0 )
                {
                    if ( errormsg != "" )
                        MessageBox . Show ( $"No Argument information is available. \nError message = [{errormsg}]" , $"[{SqlSpCommand}] SP Script Information" , MessageBoxButton . OK , MessageBoxImage . Warning );
                    return;
                }
                string output = "NB: You can select a different S.P & right click it WITHOUT closing this viewer window...\nThe new Script will replace the current contents of the viewer\n\n";
                foreach ( var item in Generics )
                {
                    string store = "";
                    store = item . field1 + ",";
                    output += store;
                }
                // Display the script in whatever chsen container is relevant
                bool resetUse = false;
                if ( UseFlowdoc == false )
                {
                    UseFlowdoc = true;
                    resetUse = true;
                }
                if ( output != "" && UseFlowdoc )
                {
                    string fdinput = $"Procedure Name : {SqlSpCommand . ToUpper ( )}\n\n";
                    fdinput += output;
                    fdinput += $"\n\nPress ESCAPE to close this window...\n";
                    ShowInfo ( Flowdoc , canvas , line1: fdinput , clr1: "Black0" , line2: "" , clr2: "Black0" , line3: "" , clr3: "Black0" , header: "" , clr4: "Black0" );
                }
                else
                {
                    Mouse . OverrideCursor = Cursors . Arrow;
                    if ( UseFlowdoc )
                        ShowInfo ( Flowdoc , canvas , line1: $"Procedure [{SqlSpCommand . ToUpper ( )}] \ndoes not Support / Require any arguments" , clr1: "Black0" , line2: "" , clr2: "Black0" , line3: "" , clr3: "Black0" , header: "" , clr4: "Black0" );
                }
                if ( resetUse )
                    UseFlowdoc = false;

            }
        }

        private void DbProcsTree_SelectedItemChanged ( object sender , RoutedPropertyChangedEventArgs<object> e )
        {
            if ( e . NewValue == null )
                return;
            //var  v = SqlProcedures . IsSelected as Procname;
            var tablename = e . NewValue as Database;
            if ( tablename == null )
            {
                if ( e . NewValue == null )
                    return;
                var tvi = e . NewValue as SqlProcedures;
                SqlSpCommand = tvi . Procname;
                // Noow get  nmme  of the Db we are in 
                var items = DbProcsTree . Items;
                if ( items . CurrentItem != null )
                {
                    var db = items . CurrentItem as Database;
                    CurrentSPDb = db . Databasename;
                }
                else
                {
                    var v = sender as ItemsControl;
                    //foreach ( var item in v . Items )
                    //{
                    //	Debug. WriteLine ( item . ToString ( ) );
                    //}
                    var treeItems = WpfLib1 . Utils . FindVisualParent<TextBlock> ( this );
                    //treeItems . ForEach ( I => i . IsExpanded = false );
                }
            }
            else
            {
                var tvi = e . NewValue as Database;
                CurrentSPDb = tvi . Databasename;
            }

        }

        #region Table/SP Tree Handlers

        private void DbProcsTree_Expanded ( object sender , RoutedEventArgs e )
        {
#pragma warning disable CS0219 // The variable 'x' is assigned but its value is never used
            int x = 0;
#pragma warning restore CS0219 // The variable 'x' is assigned but its value is never used
        }
        private void LoadTreeView ( object sender , RoutedEventArgs e )
        {
            // Load Db Tables Tree viewer
            if ( TreeviewBorder . Visibility == Visibility . Visible )
            {
                TreeviewBorder . Visibility = Visibility . Hidden;
                if ( DbProcsTree . Visibility == Visibility . Visible )
                {
                    LoadTreeData ( );
                    SpLabel . Visibility = Visibility . Hidden;
                    DbProcsTree . Visibility = Visibility . Hidden;
                    SpWrappanel . Visibility = Visibility . Hidden;
                    TablesWrappanel . Visibility = Visibility . Visible;
                    TablesLabel . Visibility = Visibility . Visible;
                    DbTablesTree . Visibility = Visibility . Visible;
                    TreeviewBorder . Visibility = Visibility . Visible;
                }
            }
            else
            {
                LoadTreeData ( );
                SpLabel . Visibility = Visibility . Hidden;
                DbProcsTree . Visibility = Visibility . Hidden;
                SpWrappanel . Visibility = Visibility . Hidden;
                TablesWrappanel . Visibility = Visibility . Visible;
                TablesLabel . Visibility = Visibility . Visible;
                DbTablesTree . Visibility = Visibility . Visible;
                TreeviewBorder . Visibility = Visibility . Visible;
            }
        }
        private void LoadSpView ( object sender , RoutedEventArgs e )
        {
            // Load Stored procedures Tree viewer

            if ( TreeviewBorder . Visibility == Visibility . Visible )
            {
                TreeviewBorder . Visibility = Visibility . Hidden;
                if ( DbTablesTree . Visibility == Visibility . Visible )
                {
                    LoadTreeData ( );
                    SpLabel . Visibility = Visibility . Visible;
                    DbProcsTree . Visibility = Visibility . Visible;
                    SpWrappanel . Visibility = Visibility . Visible;
                    TablesWrappanel . Visibility = Visibility . Hidden;
                    TablesLabel . Visibility = Visibility . Hidden;
                    DbTablesTree . Visibility = Visibility . Hidden;
                    TreeviewBorder . Visibility = Visibility . Visible;
                }
            }
            else
            {
                LoadTreeData ( );
                SpLabel . Visibility = Visibility . Visible;
                DbProcsTree . Visibility = Visibility . Visible;
                SpWrappanel . Visibility = Visibility . Visible;
                TablesWrappanel . Visibility = Visibility . Hidden;
                TablesLabel . Visibility = Visibility . Hidden;
                DbTablesTree . Visibility = Visibility . Hidden;
                TreeviewBorder . Visibility = Visibility . Visible;
            }
        }

        private void Image_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
        {
            TreeviewBorder . Visibility = Visibility . Hidden;
        }

        private void DbProcsTree_Expanded_1 ( object sender , RoutedEventArgs e )
        {

        }

        private void DbProcsTree_Collapsed ( object sender , RoutedEventArgs e )
        {
#pragma warning disable CS0219 // The variable 'x' is assigned but its value is never used
            int x = 0;
#pragma warning restore CS0219 // The variable 'x' is assigned but its value is never used
        }

        private void TablesLabel_PreviewMouseRightButtonUp ( object sender , MouseButtonEventArgs e )
        {
            bool flowdocswitch = false;
            int count = 0;
            List<string> list = new List<string> ( );
            string output = "";

            foreach ( var item in DatabasesCollection )
            {
                CurrentSPDb = item . Databasename;
                if ( Utils . CheckResetDbConnection ( CurrentSPDb , out string constring ) == false )
                    return;
                SqlCommand = $"spGetTableColumns {SqlSpCommand}";
                Datagrids . CallStoredProcedure ( list , SqlCommand );
                //This call returns us a DataTable
                DataTable dt = DataLoadControl . GetDataTable ( SqlCommand );
                // This how to access  Row data from  a grid the easiest way.... parsed into a List <xxxxx>
                list = WpfLib1 . Utils . GetTableColumnsList ( dt );
                if ( dt . Rows . Count > 0 )
                    break;
            }
            if ( list . Count > 0 )
            {
                foreach ( string row in list )
                {
                    string entry = row . ToUpper ( );
                    output += row + "\n";
                    count++;
                }
                // Fiddle  to allow Flowdoc  to show Field info even though Flowdoc use is disabled
                if ( UseFlowdoc == false )
                {
                    flowdocswitch = true;
                    UseFlowdoc = true;
                }
                Debug . WriteLine ( $"loaded {count} records for table columns" );
                if ( UseFlowdoc )
                    ShowInfo ( Flowdoc , canvas , header: "Table Columns informaton accessed successfully" , clr4: "Red5" ,
                    line1: $"Request made was completed succesfully!" , clr1: "Red3" ,
                    line2: $"the structure of the table [{SqlSpCommand}] is listed below : \n{output}" ,
                    line3: $"Results created by Stored Procedure : \n({SqlCommand . ToUpper ( )})" , clr3: "Blue4"
                    );
                if ( flowdocswitch == true )
                {
                    flowdocswitch = false;
                    UseFlowdoc = false;
                }
            }
            else
            {
                if ( UseFlowdoc )
                    ShowInfo ( Flowdoc , canvas , header: "Table Columns informaton accessed successfully" , clr4: "Red5" ,
                    line1: $"The request was made succesfully, but no Table Fields were returned..." , clr1: "Red3" ,
                    line2: $"The table queried was [{SqlSpCommand}]" ,
                    line3: $"Results created by Stored Procedure : \n({SqlCommand . ToUpper ( )})" , clr3: "Blue4"
                    );
            }
        }
        #endregion Table/SP Tree Handlers

        private void DbTablesTree_SelectedItemChanged ( object sender , RoutedPropertyChangedEventArgs<object> e )
        {
            if ( e . NewValue == null )
                return;
            //var  v = SqlProcedures . IsSelected as Procname;
            var tablename = e . NewValue as Database;
            if ( tablename == null )
            {
                if ( e . NewValue == null )
                    return;
                var tvi = e . NewValue as SqlTable;
                SqlSpCommand = tvi . Tablename;
                // Now get  nmme  of the Db we are in 
                var parentitem = WpfLib1 . Utils . FindVisualParent<TextBlock> ( sender as UIElement );
                var items = DbProcsTree . Items;
                if ( items . CurrentItem != null )
                {
                    var db = items . CurrentItem as Database;
                    CurrentSPDb = db . Databasename;
                }
                else
                {
                    var v = sender as ItemsControl;
                    //foreach ( var item in v . Items )
                    //{
                    //	Debug. WriteLine ( item . ToString ( ) );
                    //}
                    var treeItems = WpfLib1 . Utils . FindVisualParent<TextBlock> ( this );
                    //treeItems . ForEach ( I => i . IsExpanded = false );
                }
            }
            else
            {
                var tvi = e . NewValue as Database;
                CurrentSPDb = tvi . Databasename;
            }

        }


        //private void ListViewWindow_PreviewMouseLeftButtonUp ( object sender , MouseButtonEventArgs e )
        //{
        //	MovingObject = null;
        //	fdl.FlowdocResizing = false;
        //	Flowdoc . BorderClicked = false;
        //	ReleaseMouseCapture ( );
        //	TvMouseCaptured = false;

        //}

        private void TreeviewBorder_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
        {
            /// treeviewer item selected with mouse
            if ( e . LeftButton == MouseButtonState . Pressed )
            {
                Label tv = sender as Label;
                if ( tv == null )
                {
                    ReleaseMouseCapture ( );
                    return;
                }
                TvFirstXPos = e . GetPosition ( tv ) . X;
                TvFirstYPos = e . GetPosition ( tv ) . Y;
                TvMouseCaptured = true;
            }
            else
            {
                ReleaseMouseCapture ( );
                TvMouseCaptured = false;
            }
        }

        #region Treeview  handlers
        private void TreeviewBorder_PreviewMouseLeftButtonUp ( object sender , MouseButtonEventArgs e )
        {
            /// stop treeviewer Move 
            ReleaseMouseCapture ( );
            //Debug. WriteLine ( "Mouse released 4" );
            TvMouseCaptured = false;
        }
        private void TreeviewBorder_MouseMove ( object sender , MouseEventArgs e )
        {
            if ( TvMouseCaptured )
            {
                //Label  tv = sender  as Label ;
                //if ( tv == null )
                //	return;
                double left = e . GetPosition ( ( TreeviewBorder as FrameworkElement ) . Parent as FrameworkElement ) . X - TvFirstXPos;
                double top = e . GetPosition ( ( TreeviewBorder as FrameworkElement ) . Parent as FrameworkElement ) . Y - TvFirstYPos;
                double trueleft = left - CpFirstXPos;
                double truetop = left - CpFirstYPos;
                if ( left >= 0 ) // && left <= canvas.ActualWidth - Flowdoc.ActualWidth)
                    ( TreeviewBorder as FrameworkElement ) . SetValue ( Canvas . LeftProperty , left );
                if ( top >= 0 ) //&& top <= canvas . ActualHeight- Flowdoc. ActualHeight)
                    ( TreeviewBorder as FrameworkElement ) . SetValue ( Canvas . TopProperty , top );
                ReleaseMouseCapture ( );
            }
            else
                ReleaseMouseCapture ( );
        }
        private void TreeviewBorder_LostFocus ( object sender , RoutedEventArgs e )
        {
            ReleaseMouseCapture ( );
            //Debug. WriteLine ( "Mouse released 5" );
            TvMouseCaptured = false;
        }
        private void TextBlock_PreviewMouseLeftButtonUp ( object sender , MouseButtonEventArgs e )
        {
        }
        private void SpTreeviewBorder_PreviewMouseLeftButtonUp ( object sender , MouseButtonEventArgs e )
        {
        }
        private void SpTreeviewBorder_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
        {
        }
        private void SpImage_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
        {
        }
        private void SpTextBlock_PreviewMouseRightButtonUp ( object sender , MouseButtonEventArgs e )
        {
        }
        private void SpTreeviewBorder_LostFocus ( object sender , RoutedEventArgs e )
        {
        }
        private void SpTreeviewBorder_MouseMove ( object sender , MouseEventArgs e )
        {
        }
        #endregion Treeview  handlers

        #region Flowdoc support via library
        private void MaximizeFlowDoc ( object sender , EventArgs e )
        {
            // Clever "Hook" method that Allows the flowdoc to be resized to fill window
            // or return to its original size and position courtesy of the Event declard in FlowDoc
            fdl . MaximizeFlowDoc ( Flowdoc , canvas , e );
        }
        private void Flowdoc_MouseLeftButtonUp ( object sender , MouseButtonEventArgs e )
        {
            // Window wide  !!
            // Called  when a Flowdoc MOVE has ended
            MovingObject = fdl . Flowdoc_MouseLeftButtonUp ( sender , Flowdoc , MovingObject , e );
            ReleaseMouseCapture ( );
        }
        private void Flowdoc_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
        {
            //In this event, we get current mouse position on the control to use it in the MouseMove event.
            MovingObject = fdl . Flowdoc_PreviewMouseLeftButtonDown ( sender , Flowdoc , e );
        }
        private void Flowdoc_MouseMove ( object sender , MouseEventArgs e )
        {
            // We are Resizing the Flowdoc using the mouse on the border  (Border.Name=FdBorder)
            fdl . Flowdoc_MouseMove ( Flowdoc , canvas , MovingObject , e );
        }
        // Shortened version proxy call		
        private void Flowdoc_LostFocus ( object sender , RoutedEventArgs e )
        {
            Flowdoc . BorderClicked = false;
        }
        public void FlowDoc_ExecuteFlowDocBorderMethod ( object sender , EventArgs e )
        {
            // EVENTHANDLER to Handle resizing
            Point pt = Mouse . GetPosition ( canvas );
            double dLeft = pt . X;
            double dTop = pt . Y;
        }
        public void fdmsg ( string line1 , string line2 = "" , string line3 = "" )
        {
            //We have to pass the Flowdoc.Name, and Canvas.Name as well as up   to 3 strings of message
            //  you can  just provie one if required
            // eg fdmsg("message text");
            fdl . FdMsg ( Flowdoc , canvas , line1 , line2 , line3 );
        }
        #endregion Flowdoc support via library

        //private void Image_MouseEnter( object sender , MouseEventArgs e )
        //{
        //	Storyboard s = ( Storyboard ) FindResource ( "ButtonExpand" );
        //	s . Begin ( ViewColumnsLb );
        //}
        public static bool GetCanExpand ( DependencyObject obj )
        {
            return ( bool ) obj . GetValue ( CanExpandProperty );
        }

        public static void SetCanExpand ( DependencyObject obj , bool value )
        {
            obj . SetValue ( CanExpandProperty , value );
        }

        // Using a DependencyProperty as the backing store for CanExpand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanExpandProperty =
            DependencyProperty . RegisterAttached ( "CanExpand" , typeof ( bool ) , typeof ( Listviews ) , new PropertyMetadata ( false ) );

        private void ControlTemplateFindElement ( object sender , RoutedEventArgs e )
        {
            // Finding the Border  that is generated by the ControlTemplate of the (Default : UnNamed) Button Style 
            var gridInTemplate = ( Grid ) clrviewer . Template . FindName ( "grid" , clrviewer );

            // Do something to the ControlTemplate-generated grid
            MessageBox . Show ( "The actual width of the grid in the ControlTemplate: "
                            + gridInTemplate . GetValue ( ActualWidthProperty ) );
        }
        private void DataTemplateFindElement ( object sender , RoutedEventArgs e )
        {
            // Getting the currently selected ListBoxItem
            // Note that the ListBox must have
            // IsSynchronizedWithCurrentItem set to True for this to work
            var myListBoxItem =
            ( ListBoxItem ) ( listBox . ItemContainerGenerator . ContainerFromItem ( listBox . Items . CurrentItem ) );

            // Getting the ContentPresenter of myListBoxItem- WORKS!
            var myContentPresenter = FindVisualChild<ContentPresenter> ( myListBoxItem );

            // Finding textBlock from the DataTemplate that is set on that ContentPresenter- WORKS!
            var myDataTemplate = myContentPresenter . ContentTemplate;
            //- returns NULL
            var myTextBlock = ( TextBlock ) myDataTemplate . FindName ( "textBlock" , myContentPresenter );


            // Do something with the DataTemplate-generated Border control - WORKS!
            var myBorder = FindVisualChild<Border> ( listBox );
            MessageBox . Show ( "The width of the Border of the  item: "
                            + myBorder . ActualWidth );
        }
        private TChildItem FindVisualChild<TChildItem> ( DependencyObject obj )
              where TChildItem : DependencyObject
        {
            for ( var i = 0 ; i < VisualTreeHelper . GetChildrenCount ( obj ) ; i++ )
            {
                var child = VisualTreeHelper . GetChild ( obj , i );
                if ( child is TChildItem )
                    return ( TChildItem ) child;
                var childOfChild = FindVisualChild<TChildItem> ( child );
                if ( childOfChild != null )
                    return childOfChild;
            }
            return null;
        }

        private void magnifyimage_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
        {
            // Rotates DataGrid row magnification down from 4 > 1 and then straight back < to 4 again
            WpfLib1 . Utils . SwitchMagnifyStyle ( dGrid , ref Magnifyrate );
            WpfLib1 . Utils . SwitchMagnifyStyle ( listBox , ref Magnifyrate , false );
            WpfLib1 . Utils . SwitchMagnifyStyle ( listView , ref Magnifyrate , false );
            WpfLib1 . Utils . SwitchMagnifyStyle ( DataTemplatesLb , ref Magnifyrate , false );
            WpfLib1 . Utils . SwitchMagnifyStyle ( DataTemplatesLv , ref Magnifyrate , false );
            WpfLib1 . Utils . SwitchMagnifyStyle ( DbMain , ref Magnifyrate , false );
            WpfLib1 . Utils . SwitchMagnifyStyle ( dbNameLv , ref Magnifyrate , false );
            WpfLib1 . Utils . SwitchMagnifyStyle ( fontSize , ref Magnifyrate , false );
            WpfLib1 . Utils . SwitchMagnifyStyle ( rowheight , ref Magnifyrate , false );
            dGrid . UpdateLayout ( );
            listBox . UpdateLayout ( );
            listView . UpdateLayout ( );
            DataTemplatesLb . UpdateLayout ( );
            DataTemplatesLv . UpdateLayout ( );
            DbMain . UpdateLayout ( );
            dbNameLv . UpdateLayout ( );
            fontSize . UpdateLayout ( );
            rowheight . UpdateLayout ( );


        }
    }
}


