using System;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Diagnostics;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;
using System . Windows . Media;

using NewWpfDev . Models;
using NewWpfDev . SQL;
using NewWpfDev . ViewModels;
using NewWpfDev . Views;

using ServiceStack;

namespace NewWpfDev . UserControls
{
    /// <summary>
    /// Interaction logic for BankAccountInfo.xaml
    /// that shows full details of a Bank Account
    /// // It hosts a hidden datagrid with Customer records for direct data access
    /// </summary>
    public partial class BankAccountInfo : UserControl
    {

        #region NotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged ( string propertyName )
        {
            if ( PropertyChanged != null )
            {
                PropertyChanged ( this , new PropertyChangedEventArgs ( propertyName ) );
            }
        }
        #endregion NotifyPropertyChanged

        #region Properties

        private static BankAcctVm BankAcctVm { get; set; }
        private static BankAccountVM BankAccountvm { get; set; }
        public static DataGrid datagrid { get; set; }
        public static bool loading { get; set; }

        #endregion Properties

        #region full properties

        private static  BankAccountInfo thiswin;
        public static  BankAccountInfo ThisWin
        {
            get { return ( BankAccountInfo ) thiswin; }
            set { thiswin = value; }
        }
        private TextBox lastEditField;

        public TextBox LastEditField
        {
            get { return lastEditField; }
            set { lastEditField = value; NotifyPropertyChanged ( nameof ( LastEditField ) ); }
        }

        #endregion full properties

        public static ObservableCollection<CustomerViewModel> Cvm = new ObservableCollection<CustomerViewModel> ( );

        #region DP's

        public int CurrentIndex
        {
            get { return ( int ) GetValue ( CurrentIndexProperty ); }
            set { SetValue ( CurrentIndexProperty , value ); }
        }
        public static readonly DependencyProperty CurrentIndexProperty =
            DependencyProperty . Register ( "CurrentIndex" , typeof ( int ) , typeof ( BankAccountInfo ) , new PropertyMetadata ( ( int ) 0 , SetSelectedItem ) );
        private static void SetSelectedItem ( DependencyObject d , DependencyPropertyChangedEventArgs e )
        {
            // Always set selected item as well as index
            datagrid . SelectedItem = e . NewValue;
        }

        public DataGrid Datagrid
        {
            get { return ( DataGrid ) GetValue ( DatagridProperty ); }
            set { SetValue ( DatagridProperty , value ); }
        }
        public static readonly DependencyProperty DatagridProperty =
            DependencyProperty . Register ( "Datagrid" , typeof ( DataGrid ) , typeof ( BankAccountInfo ) , new PropertyMetadata ( ( DataGrid ) null ) );

        public CustomerViewModel CustRecord
        {
            get { return ( CustomerViewModel ) GetValue ( CustRecordProperty ); }
            set { SetValue ( CustRecordProperty , value ); }
        }
        public static readonly DependencyProperty CustRecordProperty =
            DependencyProperty . Register ( "CustRecord" , typeof ( CustomerViewModel ) , typeof ( BankAccountInfo ) , new PropertyMetadata ( ( CustomerViewModel ) null ) );

        #endregion DP's

        public BankAccountInfo ( )
        {
            InitializeComponent ( );
            //viewmodel is BankAccountVM
            var viewmodel = new BankAccountVM ( );
            this . DataContext = viewmodel;
            ThisWin = this;
            BankAcctVm = BankAcctVm . Instances;
            BankAccountvm = viewmodel;
            // setup search system so Info panel shows selected record
            BankAcctVm . Matchfound += BankAccountList_Matchfound;
            BankAcctVm . BankSelChanged += BankAccountInfo_BankSelChanged;

            //How to add a generic handler (12+ Edit fields in this case) 
            RoutedEventHandler handler = new RoutedEventHandler ( FocusHandler );
            FocusManager . AddGotFocusHandler ( this , handler );
            loading = true;
            Datagrid = backinggrid;
            // Setup Dp;s
            Datagrid . SelectedIndex = 0;
            CustRecord = Datagrid . SelectedItem as CustomerViewModel;
            CurrentIndex = 0;
            //LoadCustomer ( );
            backinggrid . ItemsSource = Cvm;
            loading = false;
        }
        public static BankAccountInfo GetBankAccountInfoHandle ( )
        {
            return ThisWin;
        }

        private void Custnumber_LostKeyboardFocus ( object sender , KeyboardFocusChangedEventArgs e )
        {
            TextBox tb = sender as TextBox;
            tb . Background = Brushes . Transparent;
            tb . Foreground = Brushes . White;
            tb . FontWeight = FontWeight . FromOpenTypeWeight ( 200 );
        }

        //Generic handler for (TextBox) edit fields
        private void FocusHandler ( object sender , RoutedEventArgs e )
        {
            if ( LastEditField != null )
            {
                LastEditField . Background = Brushes . Transparent;
                LastEditField . Foreground = Brushes . White;
                Search. Background = Brushes . Transparent;
                Search . Foreground = Brushes . White;
                Search . Text = "Search ...";
                Search . FontWeight = FontWeight . FromOpenTypeWeight ( 200 );
            }
            TextBox tb = e . OriginalSource as TextBox;
            if ( tb == null )
            {
                LastEditField = null;
                return;
            }
            LastEditField = tb;
            tb . Background = tb . Name switch
            {
                "custnumber" => tb . Background = Brushes . White,
                "banknumber" => tb . Background = Brushes . White,
                "firstname" => tb . Background = Brushes . White,
                "lastname" => tb . Background = Brushes . White,
                "actype" => tb . Background = Brushes . White,
                "balance" => tb . Background = Brushes . White,
                "intrate" => tb . Background = Brushes . White,
                "addr1" => tb . Background = Brushes . White,
                "addr2" => tb . Background = Brushes . White,
                "town" => tb . Background = Brushes . White,
                "county" => tb . Background = Brushes . White,
                "pcode" => tb . Background = Brushes . White,
                "odate" => tb . Background = Brushes . White,
                "cdate" => tb . Background = Brushes . White,
                "Search" => Search .Background = Brushes . White
            };

            tb . Foreground = tb . Name switch
            {
                "custnumber" => tb . Foreground = Brushes . Red,
                "banknumber" => tb . Foreground = Brushes . Red,
                "firstname" => tb . Foreground = Brushes . Red,
                "lastname" => tb . Foreground = Brushes . Red,
                "actype" => tb . Foreground = Brushes . Red,
                "balance" => tb . Foreground = Brushes . Red,
                "intrate" => tb . Foreground = Brushes . Red,
                "addr1" => tb . Foreground = Brushes . Red,
                "addr2" => tb . Foreground = Brushes . Red,
                "town" => tb . Foreground = Brushes . Red,
                "county" => tb . Foreground = Brushes . Red,
                "pcode" => tb . Foreground = Brushes . Red,
                "odate" => tb . Foreground = Brushes . Red,
                "cdate" => tb . Foreground = Brushes . Red,
                "Search" => Search . Foreground = Brushes . Red
            };
            if ( tb . Name == "Search" && Search . Background == Brushes . White )
            {
                Search . Text = "105";
                Search . SelectionLength = Search . Text . Length;
                Search . SelectionStart= Search . Text . Length - 1;
                Search . FontWeight = FontWeight . FromOpenTypeWeight ( 400 );
            }
        }

        private void Custnumber_GotKeyboardFocus ( object sender , KeyboardFocusChangedEventArgs e )
        {
            TextBox tb = sender as TextBox;
            tb . Background = Brushes . White;
            tb . Foreground = Brushes . Red;
            tb . FontWeight = FontWeight . FromOpenTypeWeight ( 800 );
        }

        public static BankAcHost Host { get; set; }
        public static void SetHost ( BankAcHost host )
        {
            Host = host;
        }
        private bool IsActive { get; set; }

        #region Window linkage

        public void IsHost ( bool value )
        {
            IsActive = value;
            if ( value == true )
            {
            }
        }

        private void SelectDetails ( object sender , RoutedEventArgs e )
        {
            // send name of panel requested (Grid)
            Host . ClosePanel ( this , "BANKACCOUNTGRID" );
        }

        private void SelectAccounts ( object sender , RoutedEventArgs e )
        {
            // send name of panel requested (Info)
            Host . ClosePanel ( this , "BANKACCOUNTLIST" );
        }

        #endregion Window linkage

        #region Data grid handlers

        public async void LoadCustomer ( )
        {
            Cvm = new ObservableCollection<CustomerViewModel> ( );            
            Cvm = SqlSupport . LoadCustomer ( "Select * from Customer order by CustNo, BankNo" , 0 , false );
            Application . Current . Dispatcher . BeginInvoke ( ( ) =>
            {
                backinggrid . ItemsSource = Cvm;
                backinggrid . SelectedIndex = 0;
                backinggrid . SelectedItem = 0;
                CustomerViewModel cv = new CustomerViewModel ( );
                cv = backinggrid . SelectedItem as CustomerViewModel;
                ShowDataRecord ( cv , null );
                datagrid = backinggrid;
            } );
        }
        private void Customergrid_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {
            if ( loading ) return;
            CustomerViewModel cv = new CustomerViewModel ( );
            cv = backinggrid . SelectedItem as CustomerViewModel;
            if ( cv == null ) return;
            datagrid = backinggrid;
            ShowDataRecord ( cv , null );
        }

        #endregion Data grid handlers

        #region Support methods
        // Internal suporting method
        private CustomerViewModel FindCustRecord ( BankAccountViewModel bvm )
        {
            int count = 0;
            if ( bvm == null ) return null;
            CustomerViewModel cv = new CustomerViewModel ( );
            foreach ( var item in backinggrid . Items )
            {
                cv = item as CustomerViewModel;
                if ( cv == null )
                    break;
                if ( cv . CustNo == bvm . CustNo )
                { // && cv . BankNo == bvm . BankNo ) {
                    backinggrid . SelectedIndex = count;
                    break;
                }
                count++;
            }
            return backinggrid . SelectedItem as CustomerViewModel; ;
        }
        private void ShowDataRecord ( CustomerViewModel cv , BankAccountViewModel bv )
        {
            //Custno . Text = cv . CustNo + ", A'c # " + cv . BankNo;
            custnumber . Text = cv . CustNo;
            banknumber . Text = cv . BankNo;
            firstname . Text = cv . FName;
            lastname . Text = cv . LName;
            addr1 . Text = cv . Addr1;
            addr2 . Text = cv . Addr2;
            town . Text = cv . Town;
            county . Text = cv . County;
            pcode . Text = cv . PCode;
            if ( bv != null )
            {
                actype . Text = bv . AcType . ToString ( );
                balance . Text = bv . Balance . ToString ( );
                intrate . Text = bv . IntRate . ToString ( );
                odate . Text = bv . ODate . Date . ToShortDateString ( );
                cdate . Text = bv . CDate . Date . ToShortDateString ( );
                loading = false;
                if ( BankAccountvm .InfoText != null )
                    BankAccountvm . InfoText = $"Information panel updated for {custnumber . Text} ...";
            }
            //else {
            //    if ( BankAcHost . info != null )
            //        BankAcHost . info . Text = $"Information panel partially updated for {Custno . Text} ...";
            //}
        }
        #endregion Support methods

        #region Event handlers

        // Event handler returned from BankAccountList
        private void BankAccountInfo_BankSelChanged ( object sender , SelchangedArgs args )
        {
            CustomerViewModel cv = FindCustRecord ( args . bv );
            if ( cv != null )
            {
                ShowDataRecord ( cv , args . bv );
            }
        }

        // Event handler returned from BankAccountList
        private void BankAccountList_Matchfound ( object sender , SelchangedArgs args )
        {
            TextBox tb = sender as TextBox;
            if ( args . IsSearchResult == true )
            {
                ShowDataRecord ( args . cv , args . bv );
                return;
            }
            SelchangedArgs selchangedArgs = new SelchangedArgs ( );
            if ( args . FindResult )
            {
                if ( args . bv != null )
                {
                    selchangedArgs = args;
                    if ( tb != null )
                    {
                        selchangedArgs . CustNo = tb . Text;
                        BankAcctVm . TriggerFindMatch ( sender , selchangedArgs );
                    }
                }
                if ( BankAccountvm . InfoText != null )
                    BankAccountvm . InfoText = "Matching record identified...";
            }
            else
            {
                if ( BankAccountvm . InfoText != null )
                    BankAccountvm . InfoText = "Failed to identify any match .. Is DataGrid Loaded ?.";
            }
        }
        // Event trigger to BankAccountList
        #endregion Event handling

        private void custnumber_KeyDown ( object sender , KeyEventArgs e )
        {
            SelchangedArgs args = new SelchangedArgs ( );
            if ( e . Key == Key . Enter )
            {
                TextBox tb = sender as TextBox;
                if ( tb != null )
                    //Trigger message (to BANKACCOUNTLIST in ths case)
                    if ( tb . Text . Length == 7 )
                    {
                        args . CustNo = tb . Text;
                        BankAccountViewModel bv = new BankAccountViewModel ( );
                        bv . CustNo = tb . Text;
                        args . cv = FindCustRecord ( bv );
                        // send it the current CVM record as well
                        BankAcctVm . TriggerFindMatch ( sender , args );
                    }
            }
        }

        private void UpdateBankRecord ( object sender , RoutedEventArgs e )
        {
            try
            {
                BankAccountViewModel bv = new BankAccountViewModel ( );
                CustomerViewModel cv = new CustomerViewModel ( );
                cv = backinggrid . SelectedItem as CustomerViewModel;
                SelchangedArgs args = new SelchangedArgs ( );
                args . cv = backinggrid . SelectedItem as CustomerViewModel;
                bv . CustNo = custnumber . Text;
                bv . BankNo = banknumber . Text;
                bv . AcType = Convert . ToInt32 ( actype . Text );
                bv . Balance = Convert . ToDecimal ( balance . Text );
                bv . IntRate = Convert . ToDecimal ( intrate . Text );
                bv . ODate = Convert . ToDateTime ( odate . Text );
                bv . CDate = Convert . ToDateTime ( cdate . Text );
                cv . Addr1 = addr1 . Text;
                cv . Addr2 = addr2 . Text;
                cv . Town = town . Text;
                cv . County = county . Text;
                cv . PCode = pcode . Text;
                args . bv = bv;
                BankAcctVm . TriggerUpdate ( sender , args );
            }
            catch(FormatException  ex )
            {
                Debug . WriteLine ($"Failed to add record : {ex.Message}");
            }
        }

        private void BankAccountinfo_SizeChanged ( object sender , SizeChangedEventArgs e )
        {
            Debug . WriteLine ( "" );
        }
        public void ResizeControl ( double height , double width )
        {
            this . VerticalAlignment = VerticalAlignment . Top;
            this . HorizontalAlignment = HorizontalAlignment . Left;
            this . Height = height;
            this . Width = width;
            this . Refresh ( );
        }

        private void Createnew ( object sender , RoutedEventArgs e )
        {
            if ( makenew . Content != "Save New A/c" )
            {
                custnumber . Text = "105";
                banknumber . Text = "41";
                firstname . Text = "";
                lastname . Text = "";
                actype . Text = "1/2/3/4 ?";
                balance . Text = "";
                intrate . Text = "";
                addr1 . Text = "";
                addr2 . Text = "";
                town . Text = "";
                county . Text = "";
                pcode . Text = "";
                odate . Text = DateTime . Now . ToString ( );
                DateTime dt = DateTime . Now;
                dt = dt . AddYears ( 10 );
                cdate . Text = dt . ToString ( );
                custnumber . Focus ( );
                custnumber . SelectionLength = 3;
                makenew . Content = "Save New A/c";
            }
            else
            {
                makenew . Content = "Create New A/c";
            }
        }

        private void DeleteRecord ( object sender , RoutedEventArgs e )
        {
            GenericClass genclass = new GenericClass ( );
            genclass = backinggrid . SelectedItem as GenericClass;
        }

         private void prevrecord ( object sender , RoutedEventArgs e )
        {
            // = backinggrid
            if ( Datagrid . SelectedIndex > 0 )
            {
                Datagrid . SelectedIndex = Datagrid . SelectedIndex - 1;
                UpdateBankRecord ( sender , null );
                BankAccountvm . InfoText = "Previous Record is displayed ...";
            }
            else BankAccountvm . InfoText = "First Record is displayed ...";
            Host . Info . Refresh ( );
        }

        private void nextrecord ( object sender , RoutedEventArgs e )
        {
            // = backinggrid
            if ( Datagrid . SelectedIndex < Datagrid . Items . Count - 1 )
            {
                Datagrid . SelectedIndex = Datagrid . SelectedIndex + 1;
            UpdateBankRecord ( sender , null );
                BankAccountvm . InfoText = "Next Record is displayed ...";
            }
            else BankAccountvm . InfoText = "Last Record is displayed ...";
            Host . Info . Refresh ( );
        }

        private void Search_MouseEnter ( object sender , MouseEventArgs e )
        {
            //Search . Background = Brushes . White;
            //Search . Background = Brushes . Red;
            //Search . Text = "105";
            //Search .SelectionLength= Search .Text.Length;
            //Search . FontWeight = FontWeight . FromOpenTypeWeight ( 400 );
        }

        private void Search_MouseLeave ( object sender , MouseEventArgs e )
        {
            //Search . Background = Brushes . Transparent;
            //Search . Background = Brushes . White;
            //Search . Text = "Search ...";
            //Search . FontWeight = FontWeight . FromOpenTypeWeight ( 200 );
        }
    }
}

