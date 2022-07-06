using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Diagnostics;
using System . IO;
using System . Runtime . Serialization . Formatters . Binary;
using System . Text . Json;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Data;
using System . Windows . Input;
using System . Windows . Media;
using System . Xml . Serialization;

using DocumentFormat . OpenXml . Drawing;

using Newtonsoft . Json;
using Newtonsoft . Json . Linq;

using NewWpfDev . Sql;
using NewWpfDev . SQL;
using NewWpfDev . ViewModels;
using NewWpfDev . Views;

using static System . Net . WebRequestMethods;
using static NewWpfDev . Views . Tabview;

using Cursors = System . Windows . Input . Cursors;

namespace NewWpfDev . UserControls {

    public partial class DgUserControl : UserControl, ITabViewer {
        #region NotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged ( string propertyName ) {
            if ( PropertyChanged != null ) {
                PropertyChanged ( this , new PropertyChangedEventArgs ( propertyName ) );
            }
        }
        #endregion NotifyPropertyChanged

        const string FileName = @"JSONTEXT.json";
        DatagridUserControlViewModel dgridctrlvm {
            get; set;
        }
        private double fontsize;
        public double Fontsize {
            get { return fontsize; }
            set { fontsize = value; NotifyPropertyChanged ( nameof ( Fontsize ) ); }
        }

        #region header block
        public ObservableCollection<ViewModels . BankAccountViewModel> Bvm { get; private set; }
        public ObservableCollection<ViewModels . CustomerViewModel> Cvm { get; private set; }
        public ObservableCollection<GenericClass> Gvm { get; set; }
        public string CurrentType { set; get; } = "CUSTOMER";
        new private bool IsLoaded { get; set; } = false;
        public static bool TrackselectionChanges { get; set; } = false;
        #endregion header block

        #region Serialization
        public DgUserControl ReadSerializedObject ( ) {
            DgUserControl dg = new DgUserControl ( );
            XmlSerializer mySerializer = new XmlSerializer ( typeof ( DgUserControl ) );
            BinaryFormatter b = new BinaryFormatter ( );

            StreamReader reader = new StreamReader ( FileName );
            XmlSerializer serialObject = new XmlSerializer ( typeof ( DgUserControl ) );
            DgUserControl deSerialObject = ( DgUserControl ) serialObject . Deserialize ( reader );
            reader . Close ( );
            return deSerialObject;
        }
        public void WriteSerializedObjectXML ( ObservableCollection<BankAccountViewModel> dgobj ) {
            //Creates an XML file as output
            XmlSerializer mySerializer = new XmlSerializer ( typeof ( ObservableCollection<BankAccountViewModel> [ ] ) );
            // To write to a file, create a StreamWriter object.  
            StreamWriter myWriter = new StreamWriter ( FileName );
            mySerializer . Serialize ( myWriter , dgobj );
            myWriter . Close ( );
        }
        //  public bool WriteSerializedObjectJSON ( object obj , string file = "" ) {
        //    //Writes any linear style object as a JSON file (Observable collection works fine)
        //    // Doesnt handle Datagrids or UserControl etc
        //    //Create JSON String
        //    if ( file == "" )
        //        file = FileName;
        //    try {
        //        var options = new JsonSerializerOptions { WriteIndented = true , IncludeFields = true , MaxDepth = 12 };
        //        string jsonString = System . Text . Json . JsonSerializer . Serialize<object> ( obj , options );
        //        // Save JSON file to disk 
        //        XmlSerializer mySerializer = new XmlSerializer ( typeof ( string ) );
        //        StreamWriter myWriter = new StreamWriter ( file );
        //        mySerializer . Serialize ( myWriter , jsonString );
        //        myWriter . Close ( );
        //        return true;
        //    }
        //    catch ( Exception ex ) {
        //        Debug . WriteLine ( $"Serialization FAILED :[{ex . Message}]" );
        //    }
        //    return false;
        //}
        //public string ReadSerializedObjectJson ( string file ) {
        //    string fileName = file, output = "";
        //    JsonTextReader reader = new JsonTextReader ( new StringReader ( file ) );
        //    while ( reader . Read ( )){
        //        string strg = String . Format ( "{0}, {1}" , reader . TokenType , reader . Value );
        //        output += strg;
        //    }
        //    string str = reader . ReadAsString ( );
        //      return output;
        //}
        #endregion Serialization

        public DgUserControl ( ) {
            InitializeComponent ( );
            Debug . WriteLine ( $"DataGrid Control Loading ......" );
            // setup DP pointer in Tabview to DgUserControl using shortcut command line !
            Tabview . Tabcntrl . dgUserctrl = this;
            // setup local data collections
            Bvm = new ObservableCollection<BankAccountViewModel> ( );
            Cvm = new ObservableCollection<CustomerViewModel> ( );
            Fontsize = 14;
            grid1 . FontSize = Fontsize;

            //setup DataContext
            dgridctrlvm = new DatagridUserControlViewModel ( this );
            this . DataContext = dgridctrlvm;
            // setup required Hooks in our ViewModel
            this . grid1 . SelectionChanged += dgridctrlvm . grid1_SelectionChanged;
            // allow this  to broadcast
            EventControl . TriggerWindowMessage ( this , new InterWindowArgs { message = $"DgUIserControl loaded, ViewMode is DATAGRIDUSERCONTROLVIEWMODEL..." } );
            IsLoaded = false;
            Gvm = ITabViewer . Gvm;

        }
        private void ReceivedFocus ( GotFocusArgs args ) {
            // Handle focus being set to this user control
            this . Focus ( );
            //Debug . WriteLine ( $"Setting DataGrid as Active tab" );
            Mouse . OverrideCursor = Cursors . Wait;
            // setup the current tab Id

            if ( grid1?.Items . Count > 0 ) {
                grid1 . CancelEdit ( );
                Mouse . OverrideCursor = Cursors . Arrow;
            }
            else {
                Debug . WriteLine ( $"Loading DataGrid Control" );
                if ( args . UseTask ) {
                    BankCollection bnk = new BankCollection ( );
                    ObservableCollection<BankAccountViewModel> Bvm = new ObservableCollection<BankAccountViewModel> ( );
                    Task task = new Task ( ( ) => {
                        UserControlDataAccess . GetBankObsCollectionAsync ( Bvm , "" , true , "DgUserControl" );
                    } );
                    task . Start ( );
                }
            }
            Mouse . OverrideCursor = Cursors . Arrow;
            DbCountArgs cargs = new DbCountArgs ( );
            cargs . Dbcount = grid1 . Items . Count;
            cargs . sender = "dgUserctrl";
            TabWinViewModel . TriggerBankDbCount ( this , cargs );
            grid1 . CancelEdit ( );
            return;
        }

        public static void SetListSelectionChanged ( bool arg ) {
            TrackselectionChanges = arg;
        }

        public async Task LoadBank ( bool update = true ) {
            BankCollection bankcollection = new BankCollection ( );

            Tabview . Tabcntrl . DtTemplates . TemplateNameDg = "BANKACCOUNT";
            Tabview . Tabcntrl . CurrentTypeDg = "BANKACCOUNT";
            Tabview . Tabcntrl . ActiveControlType = Tabview . Tabcntrl . dgUserctrl;

            Application . Current . Dispatcher . Invoke ( ( ) => {
                Tabview . Tabcntrl . dgUserctrl . grid1 . ItemsSource = null;
                Tabview . Tabcntrl . dgUserctrl . grid1 . Items . Clear ( );
                Tabview . Tabcntrl . dgUserctrl . grid1 . CellStyle = FindResource ( "MAINCustomerGridStyle" ) as Style;

                if ( Bvm == null ) Bvm = new ObservableCollection<BankAccountViewModel> ( );
                CurrentType = "BANK";

                // Set colors of Indicator panels on Tabview
                Tabview . tabvw . DbTypeFld . Background = FindResource ( "Blue5" ) as SolidColorBrush;
                Tabview . tabvw . DbCount . Background = Application . Current . FindResource ( "Blue5" ) as SolidColorBrush;
            } );

            TabWinViewModel . TriggerDbType ( CurrentType );

            Task task = Task . Run ( ( ) => {
                // This is pretty fast - uses Dapper and Linq
                this . Dispatcher . Invoke ( ( ) => {
                    Bvm = ( ObservableCollection<BankAccountViewModel> ) UserControlDataAccess . GetBankObsCollectionAsync ( Bvm , "" , true , "DgUserControl" );
                } );
            } );
            return;
        }
        public async Task LoadCustomer ( bool update = true ) {
            this . Dispatcher . Invoke ( ( ) => {
                Tabview . Tabcntrl . DtTemplates . TemplateNameDg = "CUSTOMER";
                Tabview . Tabcntrl . CurrentTypeDg = "CUSTOMER";
                Tabview . Tabcntrl . ActiveControlType = Tabview . Tabcntrl . dgUserctrl;
                this . grid1 . ItemsSource = null;
                this . grid1 . Items . Clear ( );
                Tabview . Tabcntrl . dgUserctrl . grid1 . CellStyle = FindResource ( "MAINCustomerGridStyle" ) as Style;

                // Set colors of Indicator panels on Tabview
                Tabview . tabvw . DbTypeFld . Background = FindResource ( "Blue5" ) as SolidColorBrush;
                Tabview . tabvw . DbCount . Background = Application . Current . FindResource ( "Blue5" ) as SolidColorBrush;
                CurrentType = "CUSTOMER";

                TabWinViewModel . TriggerDbType ( CurrentType );
                if ( Cvm == null ) Cvm = new ObservableCollection<CustomerViewModel> ( );
                Task task = Task . Run ( ( ) => {
                    // This is pretty fast - uses Dapper and Linq
                    UserControlDataAccess . GetCustObsCollection ( Cvm , "" , true , "DgUserControl" );
                } );
            } );
        }

        public async Task<bool> LoadGeneric ( string tablename ) {
            string ResultString = "";
            this . Dispatcher . Invoke ( ( ) => {
                string SqlCommand = tablename != null ? $"Select * from {tablename}" : "Select * from Invoice";
                Tabview . Tabcntrl . ActiveControlType = Tabview . Tabcntrl . dgUserctrl;
                //Setup Templates list  as we are changing Db type
                Tabview . Tabcntrl . DtTemplates . TemplateNameDg = tablename . ToUpper ( );
                Tabview . Tabcntrl . CurrentTypeDg = "GEN";
                Tabview . SetDbType ( "GEN" );

                // Set colors of Indicator panels on Tabview
                Tabview . tabvw . DbTypeFld . Background = FindResource ( "Red5" ) as SolidColorBrush;
                Tabview . tabvw . DbCount . Background = Application . Current . FindResource ( "Red5" ) as SolidColorBrush;

                Tabview . Tabcntrl . dgUserctrl . grid1 . CellStyle = null;
                Tabview . Tabcntrl . dgUserctrl . grid1 . ItemsSource = null;
                Tabview . Tabcntrl . dgUserctrl . grid1 . Items . Clear ( );
                Tabview . Tabcntrl . dgUserctrl . grid1 . Foreground = FindResource ( "Red5" ) as SolidColorBrush;
                // Gvm = new ObservableCollection<GenericClass> ( );
                Gvm = SqlSupport . LoadGeneric ( SqlCommand , out ResultString , 0 , false );
                CreateGenericColumns ( SqlServerCommands . GetGenericColumnCount ( Gvm ) );
                SqlServerCommands . LoadActiveRowsOnlyInGrid ( Tabview . Tabcntrl . dgUserctrl . grid1 , Gvm , SqlServerCommands . GetGenericColumnCount ( Gvm ) );

                // Set Datagrid to the new Data Template
                Tabview . Tabcntrl . DtTemplates . TemplateNameDg = tablename . ToUpper ( );
                Tabview . Tabcntrl . twVModel . CheckActiveTemplate ( Tabview . Tabcntrl . dgUserctrl );
                FrameworkElement elemnt = Tabview . Tabcntrl . dgUserctrl . grid1 as FrameworkElement;
                DataTemplate dtemp = new DataTemplate ( );
                DbCountArgs args = new DbCountArgs ( );
                args . Dbcount = Gvm?.Count ?? -1;
                args . sender = "dgUserctrl";
                TabWinViewModel . TriggerBankDbCount ( this , args );

                // Lock template  - cannot be changed
                dtemp . Seal ( );
                dtemp = elemnt . FindResource ( Tabview . Tabcntrl . DtTemplates . TemplatesCombo . SelectedItem . ToString ( ) ) as DataTemplate;
                Tabview . Tabcntrl . dgUserctrl . grid1 . ItemTemplate = dtemp;
                Tabview . Tabcntrl . DtTemplates . TemplatesCombo . SelectedIndex = Tabview . Tabcntrl . DtTemplates . TemplateIndexDg;
                Tabview . Tabcntrl . dgUserctrl . grid1 . UpdateLayout ( );
            } );
            return Gvm . Count == 0 ? false : true;
            //return;
        }

        #region Hilite TabItem header on mouse Entry / Exit
        public void PART_MouseLeave ( object sender , MouseEventArgs e ) {
            var tabview = TabWinViewModel . Tview;
            if ( TabWinViewModel . CurrentTabTextBlock == "Tab1Header" ) {
                tabview . Tab1Header . FontSize = 14;
                Tabview . TriggerStoryBoardOff ( 1 );
                tabview . Tab1Header . Foreground = FindResource ( "Cyan0" ) as SolidColorBrush;
            }
        }

        public void PART_MouseEnter ( object sender , MouseEventArgs e ) {
            var tabview = TabWinViewModel . Tview;
            if ( TabWinViewModel . CurrentTabTextBlock == "Tab1Header" ) {
                tabview . Tab1Header . FontSize = 18;
                Tabview . TriggerStoryBoardOn ( 1 );
                tabview . Tab1Header . Foreground = FindResource ( "Yellow0" ) as SolidColorBrush;
            }
        }
        #endregion Hilite TabItem header on mouse Entry / Exit

        #region DataGrid columns creation
        private void CreateBankColumns ( ) {
            grid1 . Columns . Clear ( );
            //Debug . WriteLine ( $"CREATING BANK COLUMNS" );
            DataGridTextColumn c1 = new DataGridTextColumn ( );
            c1 . Header = "Id";
            c1 . Binding = new Binding ( "Id" );
            grid1 . Columns . Add ( c1 );
            DataGridTextColumn c2 = new DataGridTextColumn ( );
            c2 . Header = "Customer #";
            c2 . Binding = new Binding ( "CustNo" );
            grid1 . Columns . Add ( c2 );
            DataGridTextColumn c3 = new DataGridTextColumn ( );
            c3 . Header = "Bank #";
            c3 . Binding = new Binding ( "BankNo" );
            grid1 . Columns . Add ( c3 );
            DataGridTextColumn c4 = new DataGridTextColumn ( );
            c4 . Header = "A/c Type";
            c4 . Binding = new Binding ( "AcType" );
            grid1 . Columns . Add ( c4 );
            DataGridTextColumn c5 = new DataGridTextColumn ( );
            c5 . Header = "Balance";
            c5 . Binding = new Binding ( "Balance" );
            grid1 . Columns . Add ( c5 );
            DataGridTextColumn c6 = new DataGridTextColumn ( );
            c6 . Header = "Opened";
            c6 . Binding = new Binding ( "ODate" );
            grid1 . Columns . Add ( c6 );
            DataGridTextColumn c7 = new DataGridTextColumn ( );
            c7 . Header = "Closed";
            c7 . Binding = new Binding ( "CDate" );
            grid1 . Columns . Add ( c7 );
        }
        private void CreateCustomerColumns ( ) {
            grid1 . Columns . Clear ( );
            //Debug . WriteLine ( $"CREATING CUSTOMER COLUMNS" );
            DataGridTextColumn c1 = new DataGridTextColumn ( );
            c1 . Header = "Id";
            c1 . Binding = new Binding ( "Id" );
            grid1 . Columns . Add ( c1 );
            DataGridTextColumn c2 = new DataGridTextColumn ( );
            c2 . Header = "Customer #";
            c2 . Binding = new Binding ( "CustNo" );
            grid1 . Columns . Add ( c2 );
            DataGridTextColumn c3 = new DataGridTextColumn ( );
            c3 . Header = "Bank #";
            c3 . Binding = new Binding ( "BankNo" );
            grid1 . Columns . Add ( c3 );
            DataGridTextColumn c4 = new DataGridTextColumn ( );
            c4 . Header = "A/c Type";
            c4 . Binding = new Binding ( "AcType" );
            grid1 . Columns . Add ( c4 );
            DataGridTextColumn c5 = new DataGridTextColumn ( );
            c5 . Header = "Address1";
            c5 . Binding = new Binding ( "Addr1" );
            grid1 . Columns . Add ( c5 );
            DataGridTextColumn c6 = new DataGridTextColumn ( );
            c6 . Header = "Address2";
            c6 . Binding = new Binding ( "Addr2" );
            grid1 . Columns . Add ( c6 );
            DataGridTextColumn c7 = new DataGridTextColumn ( );
            c7 . Header = "Town";
            c7 . Binding = new Binding ( "Town" );
            grid1 . Columns . Add ( c7 );
            DataGridTextColumn c8 = new DataGridTextColumn ( );
            c8 . Header = "County";
            c8 . Binding = new Binding ( "County" );
            grid1 . Columns . Add ( c8 );
            DataGridTextColumn c9 = new DataGridTextColumn ( );
            c9 . Header = "Zip";
            c9 . Binding = new Binding ( "PCode" );
            grid1 . Columns . Add ( c9 );
            DataGridTextColumn c10 = new DataGridTextColumn ( );
            c10 . Header = "Opened";
            c10 . Binding = new Binding ( "ODate" );
            grid1 . Columns . Add ( c10 );
            DataGridTextColumn c11 = new DataGridTextColumn ( );
            c11 . Header = "Closed";
            c11 . Binding = new Binding ( "CDate" );
            grid1 . Columns . Add ( c11 );
        }

        private void CreateGenericColumns ( int maxcols ) {
            grid1 . Columns . Clear ( );
            //Debug . WriteLine ( $"CREATING CUSTOMER COLUMNS" );
            DataGridTextColumn c1 = new DataGridTextColumn ( );
            c1 . Header = "Col 1";
            c1 . Binding = new Binding ( "field1" );
            grid1 . Columns . Add ( c1 );
            if ( maxcols == 1 ) return;
            DataGridTextColumn c2 = new DataGridTextColumn ( );
            c2 . Header = "Col 2";
            c2 . Binding = new Binding ( "field2" );
            grid1 . Columns . Add ( c2 );
            if ( maxcols == 2 ) return;
            DataGridTextColumn c3 = new DataGridTextColumn ( );
            c3 . Header = "Col 3";
            c3 . Binding = new Binding ( "field3" );
            grid1 . Columns . Add ( c3 );
            if ( maxcols == 3 ) return;
            DataGridTextColumn c4 = new DataGridTextColumn ( );
            c4 . Header = "Col 4";
            c4 . Binding = new Binding ( "field4" );
            grid1 . Columns . Add ( c4 );
            if ( maxcols == 4 ) return;
            DataGridTextColumn c5 = new DataGridTextColumn ( );
            c5 . Header = "Col 5";
            c5 . Binding = new Binding ( "field5" );
            grid1 . Columns . Add ( c5 );
            if ( maxcols == 5 ) return;
            DataGridTextColumn c6 = new DataGridTextColumn ( );
            c6 . Header = "Col 6";
            c6 . Binding = new Binding ( "field6" );
            grid1 . Columns . Add ( c6 );
            if ( maxcols == 6 ) return;
            DataGridTextColumn c7 = new DataGridTextColumn ( );
            c7 . Header = "Col 7";
            c7 . Binding = new Binding ( "field7" );
            grid1 . Columns . Add ( c7 );
            if ( maxcols == 7 ) return;
            DataGridTextColumn c8 = new DataGridTextColumn ( );
            c8 . Header = "Col 8";
            c8 . Binding = new Binding ( "field8" );
            grid1 . Columns . Add ( c8 );
            if ( maxcols == 8 ) return;
            DataGridTextColumn c9 = new DataGridTextColumn ( );
            c9 . Header = "Col 9";
            c9 . Binding = new Binding ( "field9" );
            grid1 . Columns . Add ( c9 );
            if ( maxcols == 9 ) return;
            DataGridTextColumn c10 = new DataGridTextColumn ( );
            c10 . Header = "Col 10";
            c10 . Binding = new Binding ( "field10" );
            grid1 . Columns . Add ( c10 );
            if ( maxcols == 10 ) return;
            DataGridTextColumn c11 = new DataGridTextColumn ( );
            c11 . Header = "Col 11";
            c11 . Binding = new Binding ( "field11" );
            grid1 . Columns . Add ( c11 );
            if ( maxcols == 11 ) return;
            DataGridTextColumn c12 = new DataGridTextColumn ( );
            c12 . Header = "Col 12";
            c12 . Binding = new Binding ( "field12" );
            grid1 . Columns . Add ( c12 );
            if ( maxcols == 12 ) return;
            DataGridTextColumn c13 = new DataGridTextColumn ( );
            c13 . Header = "Col 13";
            c13 . Binding = new Binding ( "field13" );
            grid1 . Columns . Add ( c13 );
            if ( maxcols == 13 ) return;
            DataGridTextColumn c14 = new DataGridTextColumn ( );
            c14 . Header = "Col 14";
            c14 . Binding = new Binding ( "field14" );
            grid1 . Columns . Add ( c14 );
            if ( maxcols == 14 ) return;
            DataGridTextColumn c15 = new DataGridTextColumn ( );
            c15 . Header = "Col 15";
            c15 . Binding = new Binding ( "field15" );
            grid1 . Columns . Add ( c15 );
            if ( maxcols == 15 ) return;
            DataGridTextColumn c16 = new DataGridTextColumn ( );
            c16 . Header = "Col 16";
            c16 . Binding = new Binding ( "field16" );
            grid1 . Columns . Add ( c16 );
            if ( maxcols == 16 ) return;
            DataGridTextColumn c17 = new DataGridTextColumn ( );
            c17 . Header = "Col 17";
            c17 . Binding = new Binding ( "field17" );
            grid1 . Columns . Add ( c17 );
            if ( maxcols == 17 ) return;
            DataGridTextColumn c18 = new DataGridTextColumn ( );
            c18 . Header = "Col 18";
            c18 . Binding = new Binding ( "field18" );
            grid1 . Columns . Add ( c18 );
            if ( maxcols == 18 ) return;
            DataGridTextColumn c19 = new DataGridTextColumn ( );
            c19 . Header = "Col 19";
            c19 . Binding = new Binding ( "field19" );
            grid1 . Columns . Add ( c19 );
            if ( maxcols == 19 ) return;
            DataGridTextColumn c20 = new DataGridTextColumn ( );
            c20 . Header = "Col 20";
            c20 . Binding = new Binding ( "field20" );
            grid1 . Columns . Add ( c20 );
            if ( maxcols == 20 ) return;
            DataGridTextColumn c21 = new DataGridTextColumn ( );
            c21 . Header = "Col 21";
            c21 . Binding = new Binding ( "field21" );
            grid1 . Columns . Add ( c21 );
        }
        #endregion DataGrid columns creation

        //#endregion sundry grid focus stuff
        private void CommitTables ( DependencyObject control ) {
            if ( control is DataGrid ) {
                DataGrid grid = control as DataGrid;
                grid . CommitEdit ( DataGridEditingUnit . Row , true );
                return;
            }
            int childrenCount = VisualTreeHelper . GetChildrenCount ( control );
            for ( int childIndex = 0 ; childIndex < childrenCount ; childIndex++ )
                CommitTables ( VisualTreeHelper . GetChild ( control , childIndex ) );
        }
        private void Magnifyplus2 ( object sender , RoutedEventArgs e ) {
            Fontsize += 2;
            grid1 . FontSize = Fontsize;
            grid1 . UpdateLayout ( );
        }
        private void Magnifyminus2 ( object sender , RoutedEventArgs e ) {
            Fontsize -= 2;
            grid1 . FontSize = Fontsize;
            grid1 . UpdateLayout ( );
        }

        #region Interface methods
        public void TabLoadBank ( object HostControl , string DbType , bool update ) {
            throw new NotImplementedException ( );
        }
        #endregion Interface methods

        public static void WriteSerializedObject ( ) {
            Stream SaveFileStream = System . IO . File . Create ( FileName );
            BinaryFormatter serializer = new BinaryFormatter ( );
            serializer . Serialize ( SaveFileStream , Tabview . Tabcntrl . dgUserctrl );
            SaveFileStream . Close ( );
        }
        private void ReloadBank ( object sender , RoutedEventArgs e ) {
            Tabview . Tabcntrl . twVModel . TabLoadDb ( this , "BANKACCOUNT" , true );
        }

        private void ReloadCust ( object sender , RoutedEventArgs e ) {
            Tabview . Tabcntrl . twVModel . TabLoadDb ( this , "CUSTOMER" , true );
        }

        private void WriteBinarydata ( object sender , RoutedEventArgs e ) {
            bool result = Utils.WriteSerializedObjectJSON ( Tabview.Tabcntrl.twVModel , @"C:\users\ianch\TabWinViewModel.json" , 2 );
            return;

                 result =  Utils . WriteSerializedObject ( Tabview . Tabcntrl . lgUserctrl , @"C:\users\ianch\TabWinViewModel.bin" , "LbUserControl" );
            if ( result )
                Debug . WriteLine ( "Serilaization succeeded" );
            else {
                Debug . WriteLine ( "Serilaization Failed" );
                Utils . DoErrorBeep ( 280 , 100 , 1 );
                Utils . DoErrorBeep ( 180 , 400 , 1 );
            }
        }
    }
}
