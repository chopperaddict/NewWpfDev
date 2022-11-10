using System;
using System . Collections;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Data;
using System . Diagnostics;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Data;
using System . Windows . Input;


using NewWpfDev . SQL;
using NewWpfDev . Views;


using TextBox = System . Windows . Controls . TextBox;

namespace NewWpfDev . ViewModels {
    public class YieldWindowViewModel : INotifyPropertyChanged {

        #region OnPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged ( string propertyName ) {
            if ( PropertyChanged != null ) {
                PropertyChanged ( this , new PropertyChangedEventArgs ( propertyName ) );
            }
        }
        #endregion OnPropertyChanged

        #region ICommands
        public ICommand LoadStack1 { get;  }
        public ICommand LoadStack2 { get; }
        public ICommand LoadYield1 { get;  }
        public ICommand LoadYield2 { get; set; }
        public ICommand LoadNormal { get; }
        public ICommand Iterate { get; set; }
        public ICommand Iterate2 { get; set; }
        public ICommand CloseAllBtn { get; set; }
        public ICommand ClearGrid { get; set; }
        public ICommand ToggleStack{ get; set; }

        #endregion ICommands

        public int limit1 { get; set; }
        public int limit2 { get; set; }

        private int stackorder = 1;
        private bool usenew = false;

        #region Std Variables / Properties

        private YieldWindow win { get; set; }
        public GenericStack<BankAccountViewModel> gstack { get; set; }
        public Queue<BankAccountViewModel> queue { get; set; }

        public string SqlCommand = "Select  * from BankAccount";
        //       public CollectionView BankCollectionView { get; set; }
        public DataTable dt = new DataTable ( );
        public Stopwatch sw = new Stopwatch ( );
        public bool Grid1RowEdited { set; get; }
        public int EditedRow1 { get; set; }
        public int EditedRow2 { get; set; }
        public bool Grid2RowEdited { set; get; }
        private string duration { get; set; }

        // Nullable type declarations for testing
        public Nullable<int> nullint { get; set; }
        public int? nulltype { get; set; }

        #endregion Variables / Properties

        //public string Button1Text { get; } = "Load D'base";

        #region Full Properties

        private string counter1;
        private string counter2;
        private string duration1;
        private string duration2;

        private string customernum1;
        private string customernum2;
        private BankAccountViewModel selectedaccount1;
        private BankAccountViewModel selectedaccount2;
        private string Infopanel;

        private ObservableCollection<BankAccountViewModel> Bvm;
        public ObservableCollection<BankAccountViewModel> bvm {
            get { return Bvm; }
            set {
                Bvm = value;
                NotifyPropertyChanged ( "bvm" );
            }
        }
        private ObservableCollection<BankAccountViewModel> Bvm2 = new ObservableCollection<BankAccountViewModel> ( );
        public ObservableCollection<BankAccountViewModel> bvm2 {
            get { return Bvm2; }
            set {
                Bvm2 = value;
                NotifyPropertyChanged ( "bvm2" );
            }
        }

        public string InfoPanel {
            get { return Infopanel; }
            set { Infopanel = value; NotifyPropertyChanged ( "InfoPanel" ); }
        }
        public string Counter1 {
            get { return counter1; }
            set { counter1 = value; NotifyPropertyChanged ( nameof ( Counter1 ) ); }
        }
        public string Counter2 {
            get { return counter2; }
            set { counter2 = value; NotifyPropertyChanged ( nameof ( Counter2 ) ); }
        }
        public string Duration1 {
            get { return duration1; }
            set { duration1 = value; NotifyPropertyChanged ( nameof ( Duration1 ) ); }
        }
        public string Duration2 {
            get { return duration2; }
            set { duration2 = value; NotifyPropertyChanged ( nameof ( Duration2 ) ); }
        }

        public string CustomerNum1 {
            get { return customernum1; }
            set { customernum1 = value; NotifyPropertyChanged ( "CustomerNum1" ); }
        }
        public string CustomerNum2 {
            get { return customernum2; }
            set { customernum2 = value; NotifyPropertyChanged ( "CustomerNum2" ); }
        }
        public BankAccountViewModel SelectedAccount1 {
            get { return selectedaccount1; }
            set {
                selectedaccount1 = value;
                NotifyPropertyChanged ( "SelectedAccount1" );
                //                Debug. WriteLine (   $"{SelectedAccount1.ToString()}");
                CustomerNum1 = SelectedAccount1?.CustNo;
            }
        }
        public BankAccountViewModel SelectedAccount2 {
            get { return selectedaccount2; }
            set {
                if ( value != null ) {
                    selectedaccount2 = value;
                    NotifyPropertyChanged ( "SelectedAccount2" );
                    CustomerNum2 = SelectedAccount2?.CustNo;
                }
            }
        }
        #endregion Full Properties

        #region CanExecute
        private bool CanExecuteLoadstack1 ( object arg ) { return true; }
        private bool CanExecuteLoadstack2 ( object arg ) { return true; }
        private bool CanExecuteLoadYield1 ( object arg ) { return true; }
        private bool CanExecuteLoadYield2 ( object arg ) { return true; }
        private bool CanExecuteLoadNormal ( object arg ) { return true; }
        private bool CanExecuteIterate ( object arg ) { return true; }
        private bool CanExecuteIterate2 ( object arg ) { return true; }
        private bool CanExecuteCloseAllBtn ( object arg ) { return true; }
        private bool CanExecuteClearGrid ( object arg ) { return true; }
        private bool CanExecuteToggleStack ( object arg ) { return true; }

        #endregion CanExecute

        public YieldWindowViewModel ( ) {
            LoadStack1 = new RelayCommand ( ExecuteLoadstack1 , CanExecuteLoadstack1 );
            LoadStack2 = new RelayCommand ( ExecuteLoadstack2 , CanExecuteLoadstack2 );
            LoadYield1 = new RelayCommand ( ExecuteLoadYield1 , CanExecuteLoadYield1 );
            LoadYield2 = new RelayCommand ( ExecuteLoadYield2 , CanExecuteLoadYield2 );
            LoadNormal = new RelayCommand ( ExecuteLoadNormal , CanExecuteLoadNormal );
            ClearGrid = new RelayCommand ( ExecuteClearGrid , CanExecuteClearGrid );
            Iterate = new RelayCommand ( ExecuteLoadIterate , CanExecuteIterate );
            Iterate2 = new RelayCommand ( ExecuteLoadIterate2 , CanExecuteIterate2 );
            CloseAllBtn = new RelayCommand ( ExecuteCloseAllBtn , CanExecuteCloseAllBtn );
            ToggleStack = new RelayCommand ( ExecuteToggleStack , CanExecuteToggleStack );
            queue = new Queue<BankAccountViewModel> ( );
        }
        public void PassViewPointer ( YieldWindow yieldwin ) {
            sw . Start ( );
            sw . Stop ( );
            win = yieldwin;
            bvm = new ObservableCollection<BankAccountViewModel> ( );
            bvm2 = new ObservableCollection<BankAccountViewModel> ( );
            //            UpdateBankRecord += YieldWindow_UpdateBankRecord;
            Loadnormal (3 );    // ALL
            //            win . dgrid1 . ItemsSource = bvm;
        }

        #region Execcute Commands Handlers

        private void ExecuteCloseAllBtn ( object obj ) {
            Application . Current . Shutdown ( );
        }
        private void ExecuteClearGrid ( object obj ) {
            ClearRightGrid ( );
        }
        private void ExecuteLoadstack1 ( object obj ) {
            LoadStackQueue( );
        }
        public void ExecuteLoadstack2 ( object arg ) {
            //// load from stack
            Loadyield2 ( 1 );
            BankAccountViewModel model = win . dgrid2 . SelectedItem as BankAccountViewModel;
            int index = Utils . FindMatchingRecord ( model . CustNo , model . BankNo , win . dgrid1 , "BANKACCOUNT" );
            win . dgrid1 . SelectedIndex = index;
            WpfLib1 . Utils . ScrollRecordIntoView ( win . dgrid1 , index );
        }
        private void ExecuteLoadYield1 ( object obj ) {
            // load from queue
            Loadyield2 ( 2 );
            BankAccountViewModel model = win . dgrid2 . SelectedItem as BankAccountViewModel;
            int index = Utils . FindMatchingRecord ( model . CustNo , model . BankNo , win . dgrid1 , "BANKACCOUNT" );
            WpfLib1 . Utils . ScrollRecordIntoView ( win . dgrid1 , index );
        }
        private void ExecuteLoadYield2 ( object obj ) {
            // load from Database bvm - updates everything
            Loadyield2 ( 0 );
            BankAccountViewModel model = win . dgrid2 . SelectedItem as BankAccountViewModel;
            int index = Utils . FindMatchingRecord ( model . CustNo , model . BankNo , win . dgrid1 , "BANKACCOUNT" );
            WpfLib1 . Utils . ScrollRecordIntoView ( win . dgrid1 , index );
        }
        private void ExecuteLoadNormal ( object obj ) {
            // Load both grids
            Loadnormal ( 0 );
        }
        private void ExecuteToggleStack ( object obj ) {
            if ( stackorder == 1 )
                stackorder = 2;
            else
                stackorder = 1;
            stack2_Click ( );
            BankAccountViewModel model = win . dgrid2 . SelectedItem as BankAccountViewModel;
            int index = Utils . FindMatchingRecord ( model . CustNo , model . BankNo , win . dgrid1 , "BANKACCOUNT" );
            win . dgrid1 . SelectedIndex = index;
            WpfLib1 . Utils . ScrollRecordIntoView ( win . dgrid1 , index );
        }
        #endregion Execcute Commands Handlers

        public void Loadnormal ( int level ) {
            //Standard Db load- loads bvm or both wiith appropriate grids loaded & Stack/Queues reloaded
            // 1 = left only. 2 = right only + Stack/Queue
            // 3 = BOTH/ALL
            // clears both Db's and both grids
            ClearAll ( level );
            sw . Restart ( );
            SqlCommand = "Select * from BankAccount order by CustNo,BankNo";
            dt . Clear ( );
            dt = DataLoadControl . GetDataTable ( SqlCommand );

            if ( level == 1 || level == 3 )
                bvm = SqlSupport . LoadBankCollection ( dt , false );
            if ( level == 2 || level == 3 )
                bvm2 = SqlSupport . LoadBankCollection ( dt , false );
            duration = $"{sw . ElapsedMilliseconds} ms";
            win . duration1 . Content = duration;
            win . counter1 . Content = bvm . Count . ToString ( );
            win . counter2 . Content = bvm2 . Count . ToString ( );

            if ( level == 1 || level == 3 ) {
                win . dgrid1 . ItemsSource = bvm;
                win . dgrid1 . SelectedIndex = 0;
                win . dgrid1 . SelectedItem = 0;
                win . dgrid1 . UpdateLayout ( );
            }
            if ( level == 2 || level == 3 ) {
                win . dgrid2 . ItemsSource = bvm2;
                win . dgrid2 . SelectedIndex = 0;
                win . dgrid2 . SelectedItem = 0;
                win . dgrid2 . UpdateLayout ( );
            }
            if ( level == 2 || level == 3 ) {
                gstack = new GenericStack<BankAccountViewModel> ( bvm . Count + 1 );
                foreach ( BankAccountViewModel item in bvm ) {
                    gstack . Push ( item );
                    queue . Enqueue ( item );
                }
            }
            sw . Stop ( );
            InfoPanel = $"Loaded both Grids , Stack & Queue from Db Disk Data in {sw . ElapsedMilliseconds} ms";
            win.recordval . UpdateLayout ( );
        }
        public void LoadStackQueue ( ) {
            // load stack
            sw . Restart ( );
            if ( bvm2 . Count > 0 ) {
                // Create stack just largeenought for our data to save memory
                gstack = new GenericStack<BankAccountViewModel> ( bvm . Count + 1 );
                foreach ( BankAccountViewModel item in bvm2 ) {
                    gstack . Push ( item );
                    queue . Enqueue ( item );
                }
                sw . Stop ( );
                InfoPanel = $"Loaded Stack with {gstack . Count ( )} records from Right Grid in {sw . ElapsedMilliseconds} ms";
            }
            else
                InfoPanel = $"Table(bvm) needs to be reloaded before stack can be loaded";
        }
        public void Stack2_PreviewButton ( int i ) {
        }
        private void ExecuteStack2_Preview ( object obj ) {
            Stack2_PreviewButton ( 2 );
        }
        private void loadyield1 ( ) {
            usenew = false;
            ClearRightGrid ( );
            Loadyield2 ( 2 );
            usenew = true;
        }
        public void Loadyield2 ( int arg = 0 ) {
            // Db load using yield return
            string info = "";
            if ( bvm . Count == 0 && arg == 0 ) {
                Loadnormal ( 2 );
             }
            int lim1 = limit1 == 0 ? 1 : limit1;
            int lim2 = limit2 == 0 ? 4 : limit2;
            BankAccountViewModel bvrecord = new BankAccountViewModel ( );
            sw . Stop( );
            sw . Start ( );

            if ( usenew == false ) {
                //This only returns matching items to the ACTYPE(s) passed in
                // which SAVES us using loads of memory to get ALL records and
                //ONLY THEN choose what we want to display in our grid
                //**********************************************
                // Thiis uses Yield return to load BankAccount
                // by calling LoadSelectedBankYield which retuns a single bvm record
                // which we finally add  to the collections ItemsSource.
                //**********************************************
                if ( arg > 0 ) {
                    if ( arg == 1 ) {
                        if ( gstack != null ) {
                            int scount = gstack . Count ( );
                            if ( scount > 0 ) {
                                bvm2 . Clear ( );
                                foreach ( var item in gstack )
                                    bvm2 . Add ( item );
                                info = $"Stack";
                            }
                            else
                                Loadnormal ( 2 );   // Right + Stack/Queue
                        }
                        else
                            Loadnormal ( 2 );   // Right + Stack/Queue
                    }
                    else if ( arg == 2 ) {
                        if ( queue != null ) {
                            int qcount = queue . Count ( );
                            if ( qcount > 0 ) {
                                bvm2 . Clear ( );
                                foreach ( var item in queue )
                                    bvm2 . Add ( item );
                                info = $"Queue";
                            }
                            else
                                Loadnormal ( 2 );   // Right + Stack/Queue
                        }
                        else
                            Loadnormal ( 2 );   // Right + Stack/Queue
                    }
                    win . dgrid2 . ItemsSource = bvm2;
                }
                else if ( arg == 0 ) {
                    if ( bvm . Count == 0 ) {
                        bvm = LoadDbToCollections ( dt , lim1 , lim2 );
                        win . dgrid1 . ItemsSource = bvm;
                        win . dgrid2 . ItemsSource = bvm;
                    }
                    win . dgrid1 . SelectedIndex = 0;
                    win . dgrid2 . SelectedIndex = 0;
                    info = $"disk";
                }
                if ( win . dgrid1 . ItemsSource == null ) {
                    win . dgrid1 . ItemsSource = bvm;
                    win . dgrid1 . SelectedIndex = 0;
                }
                if ( win . dgrid2 . ItemsSource == null ) {
                    win . dgrid2 . ItemsSource = bvm;
                    win . dgrid2 . SelectedIndex = 0;
                }
            }
            else {
                Loadnormal ( 2 );
                //LoadStackQueue ( );
                //foreach ( BankAccountViewModel dtBank in Utils . ReadGenericCollection ( bvm ) ) {
                //    //    slightly faster way !!!!
                //    if ( dtBank . AcType >= lim1 && dtBank . AcType <= lim2 ) {
                //        bvm2 . Add ( dtBank );
                //        win . dgrid2 . Refresh ( );
                //    }
                //    win . dgrid2 . ItemsSource = bvm2;
                //}
            }
            sw . Stop ( );
            Counter1 = bvm . Count . ToString ( );
            Counter2 = bvm2 . Count . ToString ( );
            Duration1 = $"{sw . ElapsedMilliseconds} ms";
            Duration2 = $"{sw . ElapsedMilliseconds} ms";
            win . dgrid2 . SelectedIndex = 0;

            if(arg == 3)
                InfoPanel = $"Grid2 loaded from {info} in {sw . ElapsedMilliseconds} ms";
        }
        private void ExecuteLoadIterate ( object obj ) { iterate_Click ( ); }
        private void ExecuteLoadIterate2 ( object obj ) {
            //Iterate_Click2 ( );
        }
        

        // Utilities
        public IEnumerable<DataRow> LoadSelectedBankYield ( int lim1 , int lim2 ) {
            // Use Yield return for max efficiency
            //YIELD RETURN Method to return Bank records matching ACTYPE lowHigh values pased to it
            // Only  returns records that have a matching ACTYPE(s), saving memory
            // that would otherwise be used processing all the (nearly 5000 records in this Db
            // NB There s a time overhead for this.
            BankAccountViewModel bvrecord = new BankAccountViewModel ( );
            foreach ( DataRow item in dt . Rows ) {
                bvrecord . AcType = Convert . ToInt32 ( item [ 3 ] );
                if ( bvrecord . AcType >= lim1 && bvrecord . AcType <= lim2 )
                    yield return item;
            }
         }
        private void ClearRightGrid ( ) {
            win . dgrid2 . ItemsSource = null;
            win . dgrid2 . Items . Clear ( );
        }
        private void ClearAll ( int level ) {
            if ( level == 1 )
                bvm . Clear ( );
            if ( level == 2 || level == 3 )
                bvm2 . Clear ( );
            win . dgrid1 . ItemsSource = null;
            win . dgrid2 . ItemsSource = null;
            win . dgrid1 . Items . Clear ( );
            win . dgrid2 . Items . Clear ( );
            win . dgrid1 . Items . Clear ( );
            win . dgrid2 . Items . Clear ( );
            if(level == 3)
               queue . Clear ( );
        }
        private ObservableCollection<BankAccountViewModel> LoadDbToCollections ( DataTable dt , int lim1 , int lim2 ) {
            BankAccountViewModel bvrecord = new BankAccountViewModel ( );
            bvm . Clear ( );
            // Uses Iteration, & only returns items if they match our conditions
            foreach ( DataRow dtBank in LoadSelectedBankYield ( lim1 , lim2 ) ) {
                bvrecord = new BankAccountViewModel ( );
                bvrecord . Id = Convert . ToInt32 ( dtBank [ 0 ] );
                bvrecord . BankNo = dtBank [ 1 ] . ToString ( );
                bvrecord . CustNo = dtBank [ 2 ] . ToString ( );
                bvrecord . AcType = Convert . ToInt32 ( dtBank [ 3 ] );
                bvrecord . Balance = Convert . ToDecimal ( dtBank [ 4 ] );
                bvrecord . IntRate = Convert . ToDecimal ( dtBank [ 5 ] );
                bvrecord . ODate = Convert . ToDateTime ( dtBank [ 6 ] );
                bvrecord . CDate = Convert . ToDateTime ( dtBank [ 7 ] );
                bvm . Add ( bvrecord );
            }
            return bvm;
        }

        #region Iteration example

        public void iterate_Click ( ) {
            // This uses a Generic Iteration Method in UTILS.CS to read the data that handles any type of Collection
            // it generates & uses the IEnumerable Iterator method to access the collection and  returns them indiviually
            // because it uses the yield return  system, so it can be used to obtain any number of records
            // as individual items without having to load them all into memory first
            bvm2 . Clear ( );
            ClearRightGrid ( );
            sw . Restart ( );
            if ( bvm2 . Count == 0 ) {
                foreach ( BankAccountViewModel item in Utils . ReadGenericCollection ( bvm ) ) {
                    //bvm . Add ( item );
                    bvm2 . Add ( item );
                    //win . dgrid1 . Refresh ( );
                    //win . dgrid2 . Refresh ( );
                }
            }
            //win . dgrid1 . ItemsSource = bvm;
            //win . dgrid1 . SelectedIndex = 0;
            win . dgrid2 . ItemsSource = bvm2;
            win . dgrid2 . SelectedIndex = 0;
            sw . Stop ( );
            Duration2 = $"{sw . ElapsedMilliseconds} ms";
            InfoPanel = $"grid2 reloaded with Generic iteration valuesin {sw . ElapsedMilliseconds} ms...";
        }

        private void IterateCollection ( ) {
            // How to use the IEnumerable collecton's Iterator
            BankAccountViewModel bvrecord = new BankAccountViewModel ( );
            IEnumerator ie = bvm . GetEnumerator ( );
            while ( true ) {
                if ( ie . MoveNext ( ) ) {
                    bvrecord = ie . Current as BankAccountViewModel;
                    Debug . WriteLine ( bvrecord . CustNo );
                }
                else break;
            }
            InfoPanel = $"grid2 reloaded using Enumeration...";
        }
        #endregion Iteration example


        //private void Closebtn ( object sender , RoutedEventArgs e )
        //{
        //    this . Close ( );
        //}
        ////        private bool MatchonContent = false;

        #region Editing
        public void dgrid1_CellEditEnding ( ) {
            EditedRow1 = win . dgrid1 . SelectedIndex;
            SelectedAccount1 = win . dgrid1 . SelectedItem as BankAccountViewModel;
            Grid1RowEdited = true;
        }

        public void dgrid2_CellEditEnding ( ) {

            EditedRow2 = win . dgrid2 . SelectedIndex;
            SelectedAccount2 = win . dgrid2 . SelectedItem as BankAccountViewModel;
            Grid2RowEdited = true;
        }

        #endregion Editing

        //private int stackorder = 0;
        private void stack2_Click ( ) {
            // Show stack content Ascending
            if ( gstack . Count ( ) > 0 ) {
                ClearRightGrid ( );
                bvm2 . Clear ( );
                int lim1 = limit1 == 0 ? 1 : limit1;
                int lim2 = limit2 == 0 ? 4 : limit2;
                sw . Restart ( );
                if ( stackorder == 1 ) {
                    foreach ( BankAccountViewModel item in gstack . BottomToTop ) {
                        if ( item . AcType >= lim1 && item . AcType <= lim2 )
                            bvm2 . Add ( item );
                    }
                }
                else {
                    foreach ( BankAccountViewModel item in gstack . TopToBottom ) {
                        if ( item . AcType >= lim1 && item . AcType <= lim2 )
                            bvm2 . Add ( item );
                    }
                }
                //Reset sort order for stack
                string sort = stackorder == 1 ? "Ascending" : "Descending";
                win . dgrid2 . ItemsSource = bvm2;
                win . dgrid2 . SelectedIndex = 0;
                win . dgrid2 . SelectedItem = 0;
                win . dgrid2 . Refresh ( );
                sw . Stop ( );
                Counter1 = bvm2 . Count . ToString ( );
                Duration2 = $"{sw . ElapsedMilliseconds} ms";
                InfoPanel = $"Loaded grid {sort} from Stack in {sw . ElapsedMilliseconds} ms";
              }
            else {
                InfoPanel = $"Stack needs to be reLoaded ....";
            }
        }

        public void Dgrid1_SelectionChanged ( object sender , System . Windows . Controls . SelectionChangedEventArgs e ) {
            int counter = 0;
            if ( win . dgrid1 . Items . Count <= 1 )
                return;
            if ( Grid1RowEdited ) {
                BankAccountViewModel bv = SelectedAccount1;
                // Update Sql here !!!!
                // The flag tells us the Previously selected record was changed.
                BankCollection . UpdateBankDb ( bv , "" );
                Grid1RowEdited = false;
                SelectedAccount1 = win . dgrid1 . SelectedItem as BankAccountViewModel;
            }
            else {
                DataGrid dg = sender as DataGrid;
                SelectedAccount1 = win . dgrid1 . SelectedItem as BankAccountViewModel;
            }
            try {
                if ( SelectedAccount2 != null ) {
                    // select same record in Grid2
                    foreach ( BankAccountViewModel item in win . dgrid2 . Items ) {
                        if ( item?.CustNo == SelectedAccount1?.CustNo && item?.BankNo == SelectedAccount1?.BankNo ) {
                            win . dgrid2 . SelectedIndex = counter;
                            win . dgrid2 . SelectedItem = counter;
                            win . dgrid2 . BringIntoView ( );
                            Utils . ScrollRecordIntoView ( win . dgrid2 , counter );
                            break;
                        }
                        counter++;
                    }
                }
            }
            catch ( Exception ) { }
        }

        public void dgrid2_SelectionChanged ( object sender , System . Windows . Controls . SelectionChangedEventArgs e ) {
            if ( win . dgrid2 . Items . Count <= 1 )
                return;
            if ( Grid2RowEdited ) {
                BankAccountViewModel bv = SelectedAccount2;
                // Update Sql here !!!!
                // The flag tells us the Previously selected record was changed.
                BankCollection . UpdateBankDb ( bv , "" );
                Grid2RowEdited = false;
                SelectedAccount2 = win . dgrid2 . SelectedItem as BankAccountViewModel;
            }
            else {
                DataGrid dg = sender as DataGrid;
                SelectedAccount2 = win . dgrid2 . SelectedItem as BankAccountViewModel;
            }
            if ( SelectedAccount2 == null )
                return;
            int counter = 0;
            {
                foreach ( BankAccountViewModel item in win . dgrid1 . Items ) {
                    if ( item . CustNo == SelectedAccount2 . CustNo && item . BankNo == SelectedAccount2 . BankNo ) {
                        win . dgrid1 . SelectedIndex = counter;
                        win . dgrid1 . SelectedItem = counter;
                        win . dgrid1 . BringIntoView ( );
                        Utils . ScrollRecordIntoView ( win . dgrid1 , counter );
                        break;
                    }
                    counter++;
                }
            }
        }

        private void YieldWindow_UpdateBankRecord ( object sender , DbArgs args ) {
            //    //if ( args . UseMatch == false )
            //    //{
            //    dgrid1 . SelectedIndex = args . index;
            //    dgrid1 . SelectedItem = args . index;
            //    dgrid1 . BringIntoView ( );
            //    dgrid1 . Refresh ( );
            //    dgrid1 . BringIntoView ( );
            //    Utils . ScrollRecordIntoView ( dgrid1 , args . index );
            //    //}
            //    //else
            //    //{
            //    //    dgrid1 . SelectedIndex = BankAccountViewModel . FindMatchingIndex ( args . bvm , dgrid1 );
            //    //    dgrid1 . SelectedItem= dgrid1 . SelectedIndex;
            //    //    dgrid1 . BringIntoView ( );
            //    //    Utils . ScrollRecordIntoView ( dgrid1 , dgrid1 . SelectedIndex );
            //    //}
        }
        //#region Delegate Testing
        //public bool DoCompare ( BankAccountViewModel a , BankAccountViewModel b )
        //{
        //    if ( a == null || b == null )
        //        return false;
        //    if ( a . Equals ( b ) )
        //        return true;
        //    return false;
        //}
        //public int MyFunc2 ( int a , int b )
        //{
        //    if ( a > b )
        //        return 0;
        //    else
        //        return 1;
        //}
        //public bool test ( Delegates . MyFunc mf , int a , int b , int c )
        //{
        //    int x = mf ( 100 , b );
        //    return x > 0 ? false : true;
        //}
        //#endregion Delegate Testing

    }
}

