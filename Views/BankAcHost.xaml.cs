
using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Diagnostics;
using System . Windows;
using System . Windows . Controls;

using NewWpfDev . Dapper;
using NewWpfDev . Models;
using NewWpfDev . Sql;
using NewWpfDev . UserControls;
using NewWpfDev . ViewModels;

namespace NewWpfDev . Views {
    /// <summary>
    ///********************
    /// MVVM system
    ///********************
    /// Interaction logic for BankAcHost.xaml
    /// Processing is handled by BANKACCOUNTVM.CS
    /// </summary>

    public partial class BankAcHost : Window {

        #region NotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged ( string propertyName ) {
            if ( PropertyChanged != null ) {
                PropertyChanged ( this , new PropertyChangedEventArgs ( propertyName ) );
            }
        }
        #endregion NotifyPropertyChanged

        public static ObservableCollection<GenericClass> GenCollection = new ObservableCollection<GenericClass> ( );
        public static GenericClass GenClass = new GenericClass ( );
        public List<string> TablesList = new List<string> ( );
        public string SelectedTable { get; set; }

        #region properties
        private static BankAcctVm BankAcctVm { get; set; }
        public static BankAcHost ThisWin { get; set; }
        public static BankAccountVM BankVm { get; set; }
        public static BankAccountInfo BankAcDetails { get; set; }
        public static BankAccountGrid BankAcctGrid { get; set; }
        public static BlankScreenUC BlankScreen { get; set; }
        public static GenericGridControl GenericGrid { get; set; }
        public static ComboboxPlus comboPlus { get; set; }
        public static TextBlock info { get; set; }
        public static DataGrid custgrid { get; set; }
        public static double ContentHeight { get; set; }
        public static double ContentWidth { get; set; }
        public static string CurrentPanel { get; set; }

        #endregion properties

        #region Full Properties

        private int Currentposition;
        public int currentposition {
            get { return Currentposition; }
            set { Currentposition = value; NotifyPropertyChanged ( nameof ( currentposition ) ); }
        }

        private string genericComboEntry;
        public string GenericComboEntry {
            get { return ( string ) genericComboEntry; }
            set { genericComboEntry = value; NotifyPropertyChanged ( nameof ( GenericComboEntry ) ); }
        }

        private string blankComboEntry;
        public string BlankComboEntry {
            get { return ( string ) blankComboEntry; }
            set { blankComboEntry = value; NotifyPropertyChanged ( nameof ( BlankComboEntry ) ); }
        }
        #endregion Full Properties

        public BankAcHost ( ) {
            InitializeComponent ( );
            BankAcctVm = BankAcctVm . Instances;
            //viewmodel is BankAccountVM
            var viewmodel = new BankAccountVM ( );
            //this.DataContext = viewmodel;
            viewmodel . GetHost ( );
            ThisWin = this;
            Currentposition = 0;
            info = Info;
            BankAcDetails = new BankAccountInfo ( );
            BankAcctGrid = new BankAccountGrid ( );
            BlankScreen = new BlankScreenUC ( );
            GenericGrid = new GenericGridControl ( );
            comboPlus = new ComboboxPlus ( );
            comboPlus . SetHost ( ThisWin );
            comboPlus . DataContext = ThisWin;
            BankAcctVm . DoClosePanel += BankAcctVm_DoClosePanel;
            custgrid = BankAccountGrid . datagrid;

            GenClass = new GenericClass ( );
            GenericGridControl . GetDbTablesList ( "IAN1" , out TablesList );
            combo . ItemsList = TablesList;
            comboPlus . Visibility = Visibility . Visible;
            // subscribe to Events ( Combobox selection)
            ComboboxPlus . ComboboxChanged += ComboboxPlus_ComboboxChanged;
        }
        static public void LoadGenericTable ( ) {
            int DbCount = LoadTableGeneric ( "Select * from BankAccount" , ref GenCollection );
            if ( DbCount > 0 ) {
                SqlServerCommands . LoadActiveRowsOnlyInGrid ( GenericGrid . datagrid1 , GenCollection , SqlServerCommands . GetGenericColumnCount ( GenCollection ) );
                Debug . WriteLine ( $"grid has {GenericGrid . datagrid1 . Items . Count} items" );
            }
        }

        #region Data Loading
        static private int LoadTableGeneric ( string SqlCommand , ref ObservableCollection<GenericClass> GenCollection ) {
            List<string> list2 = new ( );
            GenCollection . Clear ( );
            string errormsg = "";
            int DbCount = 0;
            DbCount = DapperSupport . CreateGenericCollection (
            ref GenCollection ,
           SqlCommand ,
            "" ,
            "" ,
            "" ,
            ref list2 ,
            ref errormsg );

            return DbCount;
        }

        #endregion Data Loading

        private void ComboboxPlus_ComboboxChanged ( object sender , ComboChangedArgs e ) {
            SelectedTable = e . Itemselected . ToString ( );
            int DbCount = LoadTableGeneric ( $"Select * from {SelectedTable}" , ref GenCollection );
            if ( CurrentPanel == "BLANKSCREEN" ) {
                if ( BlankScreen . dgrid2 . datagrid1 . Items . Count == 0 || blankComboEntry == null || SelectedTable . ToUpper ( ) != blankComboEntry . ToUpper ( ) ) {
                    if ( blankComboEntry != null && SelectedTable . ToUpper ( ) != blankComboEntry . ToUpper ( ) ) {
                        foreach ( string item in e . CBplus . comboBox . Items ) {
                            if ( item . ToUpper ( ) == blankComboEntry ) {
                                BlankScreen . dgrid2 . datagrid1 . SelectedItem = blankComboEntry;
                                break;
                            }
                        }
                    }
                    else {
                        // load new data 
                        BlankScreen . dgrid2 . datagrid1 . ItemsSource = null;
                        BlankScreen . dgrid2 . datagrid1 . Items . Clear ( );
                        SqlServerCommands . LoadActiveRowsOnlyInGrid ( BlankScreen . dgrid2 . datagrid1 , GenCollection , SqlServerCommands . GetGenericColumnCount ( GenCollection ) );
                        GenericDbUtilities . ReplaceDataGridFldNames ( SelectedTable , ref BlankScreen . dgrid2 . datagrid1 );
                        BlankScreen . dgrid2 . datagrid1 . SelectedIndex = 0;
                        blankComboEntry = SelectedTable . ToUpper ( );
                    }
                }
                else {
                    //still on same combo entry ?
                    BlankScreen . dgrid2 . datagrid1 . SelectedItem = blankComboEntry;
                }
                    //comboPlus.bl
                    BlankScreen . dgrid2 . datagrid1 . Refresh ( );
                }
                else if ( CurrentPanel == "GENERICGRID" ) {
                    if ( GenericGrid . datagrid1 . Items . Count == 0 || genericComboEntry == null || SelectedTable . ToUpper ( ) != genericComboEntry . ToUpper ( ) ) {
                        //if ( genericComboEntry  != null && SelectedTable . ToUpper ( ) != genericComboEntry . ToUpper ( ) ) {
                        //    foreach ( string item in e . CBplus . comboBox . Items ) {
                        //        if ( item . ToUpper ( ) == genericComboEntry ) {
                        //            GenericGrid . datagrid1 . SelectedItem = genericComboEntry;
                        //            break;
                        //        }
                        //    }
                        //}
                        //else {
                            // load new data 
                            GenericGrid . datagrid1 . ItemsSource = null;
                            GenericGrid . datagrid1 . Items . Clear ( );
                            SqlServerCommands . LoadActiveRowsOnlyInGrid ( GenericGrid . datagrid1 , GenCollection , SqlServerCommands . GetGenericColumnCount ( GenCollection ) );
                            GenericDbUtilities . ReplaceDataGridFldNames ( SelectedTable , ref GenericGrid . datagrid1 );
                            GenericGrid . datagrid1 . SelectedIndex = 0;
                            genericComboEntry = SelectedTable . ToUpper ( );
                        //}
                    }
                    else {
                        //still on same combo entry ?
                        GenericGrid . datagrid1 . SelectedItem = genericComboEntry;
                    }
                    GenericGrid . datagrid1 . Refresh ( );
                }
            }


            static public void CenterGrid ( DataGrid grid ) {
                Thickness th = new Thickness ( );
                double paddingtop = 130.0;
                double paddingleft = 10;
                // th = grid . Margin;
                double panelheight = ThisWin . ActualHeight - 130;
                double panelwidth = ThisWin . ActualWidth - 260;
                double gridheight = grid . ActualHeight;
                double gridwidth = grid . ActualWidth;
                Debug . WriteLine ( $"Entry margin {th . ToString ( )}" );
                Debug . WriteLine ( $"area {ThisWin . ActualHeight} / {ThisWin . ActualWidth}" );
                //            if ( gridheight < panelheight ) {
                if ( gridheight > panelheight ) {
                    // Grid HIGHER than panel height
                    grid . Height = panelheight;
                    th . Top = paddingtop;
                    th . Left = paddingleft;
                    grid . Margin = th;
                    GenericGrid . datagrid1 . Refresh ( );
                    GenericGrid . datagrid1 . UpdateLayout ( );
                    th . Bottom = 0;
                    grid . Margin = th;
                    GenericGrid . datagrid1 . Refresh ( );
                    GenericGrid . datagrid1 . UpdateLayout ( );
                }
                else if ( panelheight >= gridheight ) {
                    //grid height less than panel height
                    double diff = ( panelheight - gridheight );
                    th . Top = paddingtop;
                    th . Left = paddingleft;
                    grid . Margin = th;
                    GenericGrid . datagrid1 . Refresh ( );
                    GenericGrid . datagrid1 . UpdateLayout ( );
                }
            }

            public Dictionary<string , string> GetColumnNames ( string tablename , string domain = "IAN1" ) {
                int indx = 0;
                List<string> list = new List<string> ( );
                ObservableCollection<GenericClass> GenericClass = new ObservableCollection<GenericClass> ( );
                Dictionary<string , string> dict = new Dictionary<string , string> ( );
                // This returns a Dictionary<sting,string> PLUS a collection  and a List<string> passed by ref....
                List<int> VarCharLength = new List<int> ( );
                //			IsBankActive = ( bool ) obj;
                //if ( IsBankActive == false )
                //    dict = GenericDbUtilities . GetDbTableColumns ( ref GenericClass , ref list , "Customer" , "IAN1" , ref VarCharLength );
                //else
                dict = GenericDbUtilities . GetDbTableColumns ( ref GenericClass , ref list , tablename , domain , ref VarCharLength );

                indx = 0;
                if ( VarCharLength . Count > 0 ) {
                    foreach ( var item in GenericClass ) {
                        item . field3 = VarCharLength [ indx++ ] . ToString ( );
                    }
                }
                return dict;
                //if ( ParentBGView != null ) {
                //    SqlServerCommands . LoadActiveRowsOnlyInGrid ( ParentBGView . dataGrid2 , GenericClass , DapperSupport . GetGenericColumnCount ( GenericClass ) );
                //    indx = 0;
                //    foreach ( var col in ParentBGView . dataGrid2 . Columns ) {
                //        if ( indx == 0 )
                //            col . Header = "Field Name";
                //        else if ( indx == 1 )
                //            col . Header = "SQL Field Type";
                //        else if ( indx == 2 ) {
                //            col . Header = "NVarChar Length";
                //        }
                //        indx++;
                //    }
                //}
                //if ( VarCharLength . Count > 0 ) {
                //    string output = "";
                //    indx = 0;
                //    foreach ( var item in GenericClass ) {
                //        item . field3 = VarCharLength [ indx++ ] . ToString ( );
                //        output += item . field1 . ToString ( ) + ", " + item . field2 . ToString ( ) + ", " + item . field3 + "\n";
                //    }
                //    //fdmsg ( output , "" , "" );
            }
            public static BankAcHost GetHost ( ) {
                return ThisWin;
            }
            private void HostLoaded ( object sender , RoutedEventArgs e ) {
                BankAccountInfo . SetHost ( this );
                BankAccountGrid . SetHost ( this );
                BlankScreenUC . SetHost ( this );
                BankAcctGrid . IsHost ( true );
                BankAcDetails . LoadCustomer ( );
                //            SetActivePanel ( "" );
                SetActivePanel ( "BANKACCOUNTLIST" );
                CurrentPanel = "BANKACCOUNTLIST";
            }

            private void SetVisibility ( string newpanel ) {
                BankAcctGrid . Visibility = Visibility . Collapsed;
                BlankScreen . Visibility = Visibility . Collapsed;
                BankAcDetails . Visibility = Visibility . Collapsed;
                BankDetails . IsEnabled = true;
                updatebtn . IsEnabled = true;
                AllAccounts . IsEnabled = true;
                GenericBtn . IsEnabled = true;
                HidePanel . IsEnabled = true;
            if ( newpanel == "BANKACCOUNTLIST" ) {
                    BankAcDetails . Visibility = Visibility . Visible;
                    Debug . WriteLine ( $"{newpanel} Width={BankAcDetails . Width} Height={BankAcDetails . Height}\n{BankAcDetails . ActualWidth}  {BankAcDetails . ActualHeight}" );
                    CurrentPanel = "BANKACCOUNTLIST";
                    comboPlus . Promptlabel . Opacity = 0.3;
                    combo . IsEnabled = false;
                    combo . Opacity = 0.1;
                    BankDetails . IsEnabled = false;
                    updatebtn . IsEnabled = false;
            }
            else if ( newpanel == "BANKACCOUNTGRID" ) {
                    BankAcctGrid . Visibility = Visibility . Visible;
                    Debug . WriteLine ( $"{newpanel} Width={BankAcctGrid . Width} Height={BankAcctGrid . Height}\n{BankAcctGrid . ActualWidth}  {BankAcctGrid . ActualHeight}" );
                    CurrentPanel = "BANKACCOUNTGRID";
                    comboPlus . Promptlabel . Opacity = 0.3;
                    combo . IsEnabled = false;
                    combo . Opacity = 0.3;
                    AllAccounts . IsEnabled = false;
            }
            else if ( newpanel == "GENERICGRID" ) {
                    GenericGrid . Visibility = Visibility . Visible;
                    Debug . WriteLine ( $"{newpanel} Width={GenericGrid . Width} Height={GenericGrid . Height}\n{GenericGrid . ActualWidth}  {GenericGrid . ActualHeight}" );
                    CurrentPanel = "GENERICGRID";
                    if ( GenCollection == null )
                        GenCollection = new ObservableCollection<GenericClass> ( );
                    if ( GenCollection . Count == 0 )
                        LoadGenericTable ( );
                    UpdateCombo ( "BANKACCOUNT" );
                    comboPlus . Promptlabel . Opacity = 1.0;
                    combo . IsEnabled = true;
                    combo . Opacity = 1.0;
                    GenericBtn . IsEnabled = false;
            }
            else if ( newpanel == "BLANKSCREEN" ) {
                    BlankScreen . Visibility = Visibility . Visible;
                    Debug . WriteLine ( $"{newpanel} Width={BlankScreen . Width} Height={BlankScreen . Height}\n{BlankScreen . ActualWidth}  {BlankScreen . ActualHeight}" );
                    CurrentPanel = "BLANKSCREEN";
                    HidePanel . IsEnabled = false;
            }
            this . BankContent . Refresh ( );
                string name = newpanel == null ? "BlankPanel" : newpanel;
                Debug . WriteLine ( $"{name} set as Visible panel" );
                //comboPlus . Promptlabel . Opacity = 1.0;
                //combo . IsEnabled = true;
                //combo . Opacity = 1.0;
            }
            // Find currently selected tablle in combo and selexct it (without loosing the prompt)
            private void UpdateCombo ( string table ) {
                int count = 0;
                ComboBox cb = this . combo . comboBox as ComboBox;
                foreach ( var item in cb . Items ) {
                    if ( item . ToString ( ) . ToUpper ( ) == table ) {
                        cb . SelectedIndex = count;
                        break;
                    }
                    count++;
                }
            }

            public void SetActivePanel ( string newpanel ) {
                SetVisibility ( newpanel );
                if ( newpanel == "BANKACCOUNTLIST" ) {
                    if ( BankAcDetails == null )
                        BankAcDetails = new BankAccountInfo ( );
                    this . BankContent . Content = BankAcDetails;
                    comboPlus . Promptlabel . Opacity = 0.2;
                    combo . IsEnabled = false;
                    combo . Opacity = 0.2;
                }
                else if ( newpanel == "BANKACCOUNTGRID" ) {
                    if ( BankAcctGrid == null )
                        BankAcctGrid = new BankAccountGrid ( );
                    this . BankContent . Content = BankAcctGrid;
                    comboPlus . Promptlabel . Opacity = 0.2;
                    combo . IsEnabled = false;
                    combo . Opacity = 0.2;
                }
                else if ( newpanel == "GENERICGRID" ) {
                    this . BankContent . Content = GenericGrid;

                    if ( GenericGrid . datagrid1 . Items . Count == 0 )
                        GenericGridControl . LoadDataGrid ( "BankAccount" , "Select * from BankAccount" );
                    this . BankContent . Refresh ( );
                    comboPlus . Promptlabel . Opacity = 1.0;
                    combo . IsEnabled = true;
                    combo . Opacity = 1.0;
                }
                else if ( newpanel == "BLANKSCREEN" ) {
                    this . BankContent . Content = BlankScreen;
                    this . BankContent . Refresh ( );
                    comboPlus . Promptlabel . Opacity = 1.0;
                    combo . IsEnabled = true;
                    combo . Opacity = 1.0;
                }
                Debug . WriteLine ( $"SETACTIVEPANEL : Content Height {BankContent . Height} / {BankContent . Width}" );
                this . BankContent . Refresh ( );
            }
            private void UpdateBankRecord ( object sender , RoutedEventArgs e ) {
                BankAccountViewModel bv = new BankAccountViewModel ( );
                CustomerViewModel cv = new CustomerViewModel ( );
                cv = BankAcDetails . customergrid . SelectedItem as CustomerViewModel;
                SelchangedArgs args = new SelchangedArgs ( );
                args . cv = cv;
                bv . CustNo = BankAcDetails . custnumber . Text;
                bv . BankNo = BankAcDetails . banknumber . Text;
                bv . AcType = Convert . ToInt32 ( BankAcDetails . actype . Text );
                bv . Balance = Convert . ToDecimal ( BankAcDetails . balance . Text );
                bv . IntRate = Convert . ToDecimal ( BankAcDetails . intrate . Text );
                bv . ODate = Convert . ToDateTime ( BankAcDetails . odate . Text );
                bv . CDate = Convert . ToDateTime ( BankAcDetails . cdate . Text );
                cv . Addr1 = BankAcDetails . addr1 . Text;
                cv . Addr2 = BankAcDetails . addr2 . Text;
                cv . Town = BankAcDetails . town . Text;
                cv . County = BankAcDetails . county . Text;
                cv . PCode = BankAcDetails . pcode . Text;
                args . bv = bv;
                BankAcctVm . TriggerUpdate ( sender , args );

            }

            private void BankAcctVm_DoClosePanel ( object sender , SelchangedArgs args ) {
                SetActivePanel ( "" );
            }

            public BankAcHost GetHostContext ( object sender ) {
                if ( sender . GetType ( ) == typeof ( BankAccountInfo ) ) {
                    BankAcDetails = ( BankAccountInfo ) sender;
                }
                else if ( sender . GetType ( ) == typeof ( BankAccountGrid ) ) {
                    BankAcctGrid = ( BankAccountGrid ) sender;
                }
                return this;
            }
            public void ClosePanel ( object sender , string newpanel ) {
                // called by all UC panels  to switch panes
                SetActivePanel ( newpanel );
                return;
            }

            private void DoClosePanel ( object sender , string newpanel ) {
                // open specified panel
                SetActivePanel ( newpanel );
                return;
            }

            private void ClosePanel ( object sender ) {
                if ( sender == null ) {
                    // hide all panes
                    BankAcctGrid . Visibility = Visibility . Collapsed;
                    BankAcDetails . Visibility = Visibility . Collapsed;
                    BlankScreen = new BlankScreenUC ( );
                    BlankScreen . Height = 800;
                    BlankScreen . Width = 800;
                    BankContent . Content = BlankScreen;
                    ButtonPanel . Height = 800;
                    ButtonPanel . Width = 800;
                    ButtonPanel . Visibility = Visibility . Visible;
                    BankContent . Refresh ( );
                }
                else if ( sender != null || sender . GetType ( ) == typeof ( BankAccountInfo ) ) {
                    // toggle Info visibility
                    if ( BankAcDetails . Visibility == Visibility . Collapsed )
                        BankAcDetails . Visibility = Visibility . Visible;
                    else
                        BankAcDetails . Visibility = Visibility . Collapsed;
                }
                else if ( sender != null || sender . GetType ( ) == typeof ( BankAccountGrid ) ) {
                    // toggle grid visibility
                    if ( BankAcctGrid . Visibility == Visibility . Collapsed )
                        BankAcctGrid . Visibility = Visibility . Visible;
                    else
                        BankAcctGrid . Visibility = Visibility . Collapsed;
                }
                //ButtonPanel . Visibility = Visibility . Collapsed;
                BankContent . Refresh ( );
            }

            // NOT USED
            private void SetNewPanel ( string newpanel ) {
                if ( newpanel == "BANKACCOUNTLIST" ) {
                    // Show Account details
                    if ( BankAcDetails == null )
                        BankAcDetails = new BankAccountInfo ( );
                    BankContent . Content = BankAcDetails;
                    BankAcDetails . IsHost ( true );
                    if ( BankAcDetails . Visibility == Visibility . Collapsed )
                        BankAcDetails . Visibility = Visibility . Visible;
                    // Utils . ScrollRecordIntoView ( BankAcDetails . customergrid , BankAcDetails . customergrid . SelectedIndex );
                    BankAcctGrid . grid1 . SelectedItem = BankAcctGrid . grid1 . SelectedIndex;
                    BankAcctGrid . grid1 . UpdateLayout ( );
                    CurrentPanel = "BANKACCOUNTLIST";
                    combo . IsEnabled = false;
                    combo . Opacity = 0.3;
                }
                else if ( newpanel == "BANKACCOUNTGRID" ) {
                    // Show Bank Datagrid
                    if ( BankAcctGrid == null )
                        BankAcctGrid = new BankAccountGrid ( );
                    if ( BankAcctGrid . Visibility == Visibility . Collapsed )
                        BankAcctGrid . Visibility = Visibility . Visible;
                    BankContent . Content = BankAcctGrid;
                    BankAcctGrid . Height = ThisWin . Height;
                    BankAcctGrid . Width = ThisWin . Width;

                    BankContent . Refresh ( );
                    BankAcctGrid . IsHost ( true );
                    // Set DP in Grid viewer, also sets SelItem
                    BankAcctGrid . CurrentIndex = BankAcctGrid . grid1 . SelectedIndex;
                    BankAcctGrid . grid1 . SelectedItem = BankAcctGrid . grid1 . SelectedIndex;
                    Utils . ScrollRecordIntoView ( BankAcctGrid . grid1 , BankAcctGrid . grid1 . SelectedIndex );
                    BankAcctGrid . grid1 . UpdateLayout ( );
                    CurrentPanel = "BANKACCOUNTGRID";
                    combo . IsEnabled = false;
                    combo . Opacity = 0.3;
                }
                else if ( newpanel == "BLANKSCREEN" ) {
                    if ( BlankScreen == null )
                        BlankScreen = new BlankScreenUC ( );
                    if ( BlankScreen . Visibility == Visibility . Collapsed )
                        BlankScreen . Visibility = Visibility . Visible;
                    BankContent . Content = BlankScreen;
                    BlankScreen . Height = ThisWin . Height;
                    BlankScreen . Width = ThisWin . Width;

                    BankContent . Refresh ( );
                    CurrentPanel = "BLANKSCREEN";
                    combo . IsEnabled = true;
                    combo . Opacity = 1;
                }
                BankContent . Refresh ( );
            }

            private void Window_SizeChanged ( object sender , SizeChangedEventArgs e ) {
                var size = e . NewSize;
                this . BankContent . Height = size . Height;
                this . BankContent . Width = size . Width;
                BankAcctGrid . ResizeControl ( size . Height , size . Width );
                BankAcDetails . ResizeControl ( size . Height , size . Width );
                BlankScreen . ResizeControl ( size . Height , size . Width );
            }

            private void Window_Closed ( object sender , EventArgs e ) {
                BankAcctVm . DoClosePanel -= BankAcctVm_DoClosePanel;
                BlankScreen = null;
                BankAcctGrid = null;
                BankAcDetails = null;
                this . Close ( );
            }

            private void Closepane_Click ( object sender , RoutedEventArgs e ) {
                BankAcDetails = null;
                BankAcctGrid = null;
                BlankScreen = null;
                GenericGrid = null;
                comboPlus = null;
                GenCollection = null;
                GenClass = null;
                this . Close ( );
            }
        }
    }
