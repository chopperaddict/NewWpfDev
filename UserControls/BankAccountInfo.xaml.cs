using System;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Diagnostics;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;

using NewWpfDev . Models;
using NewWpfDev . SQL;
using NewWpfDev . ViewModels;
using NewWpfDev . Views;

using ServiceStack;

namespace NewWpfDev . UserControls {
    /// <summary>
    /// Interaction logic for BankAccountInfo.xaml
    /// that shows full details of a Bank Account
    /// // It hosts a hidden datagrid with Customer records for direct data access
    /// </summary>
    public partial class BankAccountInfo : UserControl {

        #region NotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged ( string propertyName ) {
            if ( PropertyChanged != null ) {
                PropertyChanged ( this , new PropertyChangedEventArgs ( propertyName ) );
            }
        }
        #endregion NotifyPropertyChanged

        #region Properties

        private static BankAcctVm BankAcctVm { get; set; }
        public static DataGrid datagrid { get; set; }
        public static bool loading { get; set; }

        #endregion Properties

        #region full properties

        private BankAccountInfo thiswin;
        public BankAccountInfo ThisWin {
            get { return ( BankAccountInfo ) thiswin; }
            set { thiswin = value; }
        }
        #endregion full properties

        public static ObservableCollection<CustomerViewModel> Cvm = new ObservableCollection<CustomerViewModel> ( );

        #region DP's

        public int CurrentIndex {
            get { return ( int ) GetValue ( CurrentIndexProperty ); }
            set { SetValue ( CurrentIndexProperty , value ); }
        }
        public static readonly DependencyProperty CurrentIndexProperty =
            DependencyProperty . Register ( "CurrentIndex" , typeof ( int ) , typeof ( BankAccountInfo ) , new PropertyMetadata ( ( int ) 0 , SetSelectedItem ) );
        private static void SetSelectedItem ( DependencyObject d , DependencyPropertyChangedEventArgs e ) {
            // Always set selected item as well as index
            datagrid . SelectedItem = e . NewValue;
        }

        public DataGrid Datagrid {
            get { return ( DataGrid ) GetValue ( DatagridProperty ); }
            set { SetValue ( DatagridProperty , value ); }
        }
        public static readonly DependencyProperty DatagridProperty =
            DependencyProperty . Register ( "Datagrid" , typeof ( DataGrid ) , typeof ( BankAccountInfo ) , new PropertyMetadata ( ( DataGrid ) null ) );

        public CustomerViewModel CustRecord {
            get { return ( CustomerViewModel ) GetValue ( CustRecordProperty ); }
            set { SetValue ( CustRecordProperty , value ); }
        }
        public static readonly DependencyProperty CustRecordProperty =
            DependencyProperty . Register ( "CustRecord" , typeof ( CustomerViewModel ) , typeof ( BankAccountInfo ) , new PropertyMetadata ( ( CustomerViewModel ) null ) );

        #endregion DP's

        public BankAccountInfo ( ) {
            InitializeComponent ( );
            //viewmodel is BankAccountVM
            var viewmodel = new BankAccountVM ( );
            this . DataContext = viewmodel;
            ThisWin = this;
            BankAcctVm = BankAcctVm . Instances;

            // setup search system so Info panel shows selected record
            BankAcctVm . Matchfound += BankAccountList_Matchfound;
            BankAcctVm . BankSelChanged += BankAccountInfo_BankSelChanged;
            loading = true;
            Datagrid = customergrid;
            // Setup Dp;s
            Datagrid . SelectedIndex = 0;
            CustRecord = Datagrid . SelectedItem as CustomerViewModel;
            CurrentIndex = 0;
            //LoadCustomer ( );
            customergrid . ItemsSource = Cvm;
            loading = false;
        }

        public static BankAcHost Host { get; set; }
        public static void SetHost ( BankAcHost host ) {
            Host = host;
        }
        private bool IsActive { get; set; }

        #region Window linkage

        public void IsHost ( bool value ) {
            IsActive = value;
            if ( value == true ) {
            }
        }

        private void SelectDetails ( object sender , RoutedEventArgs e ) {
            // send name of panel requested (Grid)
            Host . ClosePanel ( this , "BANKACCOUNTGRID" );
        }

        private void SelectAccounts ( object sender , RoutedEventArgs e ) {
            // send name of panel requested (Info)
            Host . ClosePanel ( this , "BANKACCOUNTLIST" );
        }

        #endregion Window linkage

        #region Data grid handlers

        public async  void LoadCustomer ( ) {
            Cvm = new ObservableCollection<CustomerViewModel> ( );
            Cvm = SqlSupport . LoadCustomer ( "Select * from Customer order by CustNo, BankNo" , 0 , false );
            customergrid . ItemsSource = Cvm;
            customergrid . SelectedIndex = 0;
            customergrid . SelectedItem = 0;
            CustomerViewModel cv = new CustomerViewModel ( );
            cv = customergrid . SelectedItem as CustomerViewModel;
            ShowDataRecord ( cv , null );
            datagrid = customergrid;
        }
        private void Customergrid_SelectionChanged ( object sender , SelectionChangedEventArgs e ) {
            if ( loading ) return;
            CustomerViewModel cv = new CustomerViewModel ( );
            cv = customergrid . SelectedItem as CustomerViewModel;
            if ( cv == null ) return;
            datagrid = customergrid;
            ShowDataRecord ( cv , null );
        }

        #endregion Data grid handlers

        #region Support methods
        // Internal suporting method
        private CustomerViewModel FindCustRecord ( BankAccountViewModel bvm ) {
            int count = 0;
            if ( bvm == null ) return null;
            CustomerViewModel cv = new CustomerViewModel ( );
            foreach ( var item in customergrid . Items ) {
                cv = item as CustomerViewModel;
                if ( cv == null )
                    break;
                if ( cv . CustNo == bvm . CustNo ) { // && cv . BankNo == bvm . BankNo ) {
                    customergrid . SelectedIndex = count;
                    break;
                }
                count++;
            }
            return customergrid . SelectedItem as CustomerViewModel; ;
        }
        private void ShowDataRecord ( CustomerViewModel cv , BankAccountViewModel bv ) {
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
            if ( bv != null ) {
                actype . Text = bv . AcType . ToString ( );
                balance . Text = bv . Balance . ToString ( );
                intrate . Text = bv . IntRate . ToString ( );
                odate . Text = bv . ODate . Date . ToShortDateString ( );
                cdate . Text = bv . CDate . Date . ToShortDateString ( );
                loading = false;
                if ( BankAcHost . info != null )
                    BankAcHost . info . Text = $"Information panel updated for {custnumber . Text} ...";
            }
            //else {
            //    if ( BankAcHost . info != null )
            //        BankAcHost . info . Text = $"Information panel partially updated for {Custno . Text} ...";
            //}
        }
        #endregion Support methods

        #region Event handlers

        // Event handler returned from BankAccountList
        private void BankAccountInfo_BankSelChanged ( object sender , SelchangedArgs args ) {
            CustomerViewModel cv = FindCustRecord ( args . bv );
            if ( cv != null ) {
                ShowDataRecord ( cv , args . bv );
            }
        }

        // Event handler returned from BankAccountList
        private void BankAccountList_Matchfound ( object sender , SelchangedArgs args ) {
            TextBox tb = sender as TextBox;
            if ( args . IsSearchResult == true ) {
                ShowDataRecord ( args . cv , args . bv );
                return;
            }
            SelchangedArgs selchangedArgs = new SelchangedArgs ( );
            if ( args . FindResult ) {
                if ( args . bv != null ) {
                    selchangedArgs = args;
                    if ( tb != null ) {
                        selchangedArgs . CustNo = tb . Text;
                        BankAcctVm . TriggerFindMatch ( sender , selchangedArgs );
                    }
                }
                if ( BankAcHost . info != null )
                    BankAcHost . info . Text = "Matching record identified...";
            }
            else {
                if ( BankAcHost . info != null )
                    BankAcHost . info . Text = "Failed to identify any match .. Is DataGrid Loaded ?.";
            }
        }
        // Event trigger to BankAccountList
        #endregion Event handling

        private void custnumber_KeyDown ( object sender , KeyEventArgs e ) {
            SelchangedArgs args = new SelchangedArgs ( );
            if ( e . Key == Key . Enter ) {
                TextBox tb = sender as TextBox;
                if ( tb != null )
                    //Trigger message (to BANKACCOUNTLIST in ths case)
                    if ( tb . Text . Length == 7 ) {
                        args . CustNo = tb . Text;
                        BankAccountViewModel bv = new BankAccountViewModel ( );
                        bv . CustNo = tb . Text;
                        args . cv = FindCustRecord ( bv );
                        // send it the current CVM record as well
                        BankAcctVm . TriggerFindMatch ( sender , args );
                    }
            }
        }

        private void UpdateBankRecord ( object sender , RoutedEventArgs e ) {
            BankAccountViewModel bv = new BankAccountViewModel ( );
            CustomerViewModel cv = new CustomerViewModel ( );
            cv = customergrid . SelectedItem as CustomerViewModel;
            SelchangedArgs args = new SelchangedArgs ( );
            args . cv = customergrid . SelectedItem as CustomerViewModel;
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

        private void BankAccountinfo_SizeChanged ( object sender , SizeChangedEventArgs e ) {
            Debug . WriteLine ( "" );
        }
        public void ResizeControl ( double height , double width ) {
            this . Height = height;
            this . Width = width;
            this . Refresh ( );
        }
    }
}
