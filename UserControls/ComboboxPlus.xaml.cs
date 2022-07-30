using System;
using System . Collections;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Diagnostics;
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

using NewWpfDev . Dapper;
using NewWpfDev . Models;
using NewWpfDev . Sql;
using NewWpfDev . ViewModels;
using NewWpfDev . Views;

namespace NewWpfDev . UserControls
{
    /// <summary>
    /// Interaction logic for ComboboxPlus.xaml
    /// </summary>


    public enum Panels
    {
        BLANK = 0,
        GEN = 1,
        PANEL3,
        PANEL4,
        PANEL5
    };

    public class ComboChangedArgs
    {
        public ComboBox CBplus = null;
        public BankAcHost BHost = null;
        public BlankScreenUC BlankCtrl = null;
        public GenericGridControl GenCtrl = null;
        public DataGrid [ ] grids = { null , null , null , null , null };
        public string Activepanel = "";
        public string ActiveTablename = "";
    }
    public partial class ComboboxPlus : UserControl
    {

        static public event EventHandler<ComboChangedArgs> ComboboxChanged;

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

        #region declarations
        public static ObservableCollection<GenericClass> GenCollection = new ObservableCollection<GenericClass> ( );
        public static GenericClass GenClass = new GenericClass ( );

        // NB : 0=GenericGrid, 1 = BlankPanel grid
        public DataGrid [ ] datagrids = new DataGrid [ 5 ];
        public string [ ] gridtablenames = new string [ 5 ];
        static int datagridcount { get; set; } = 0;
        static public int blankindex { get; set; }
        static public int gridindex { get; set; }
        private List<string> ienum = new List<string> ( );
        static public BankAcHost Host { get; set; }

        #endregion declarations

        #region Attached Properties
        public static BankAcHost GetBankHost ( DependencyObject obj ) { return ( BankAcHost ) obj . GetValue ( BankHostProperty ); }
        public static void SetBankHost ( DependencyObject obj , BankAcHost value ) { obj . SetValue ( BankHostProperty , value ); }
        public static readonly DependencyProperty BankHostProperty =
            DependencyProperty . RegisterAttached ( "BankHost" , typeof ( BankAcHost ) , typeof ( ComboboxPlus ) , new PropertyMetadata ( default ) );

        #endregion Attached Properties

        #region Full properties
        // current;ly active panel
        private string currentPanel;
        public string CurrentPanel
        {
            get { return currentPanel; }
            set { currentPanel = value; NotifyPropertyChanged ( nameof ( CurrentPanel ) ); }
        }

        // Default binded value for tables combo
        private string currentcomboSelection;
        public string currentComboSelection
        {
            get { return currentcomboSelection; }
            set { currentcomboSelection = value; NotifyPropertyChanged ( nameof ( currentComboSelection ) ); }
        }
        private string comboSelection1;
        public string ComboSelection1
        {
            get { return comboSelection1; }
            set { comboSelection1 = value; NotifyPropertyChanged ( nameof ( ComboSelection1 ) ); }
        }
        private string comboSelection2;
        public string ComboSelection2
        {
            get { return comboSelection2; }
            set { comboSelection2 = value; NotifyPropertyChanged ( nameof ( ComboSelection2 ) ); }
        }
        private object selectedItem;
        public object SelectedItem
        {
            get { return selectedItem; }
            set { selectedItem = value; NotifyPropertyChanged ( nameof ( SelectedItem ) ); }
        }

        #endregion Full properties

        #region DP's
        //+++++++++++++++++++++
        public List<string> ItemsList
        {
            get { return ( List<string> ) GetValue ( ItemsListProperty ); }
            set
            {
                SetValue ( ItemsListProperty , value );
                this . MyItemsSource = value;
            }
        }
        public static readonly DependencyProperty ItemsListProperty =
            DependencyProperty . Register ( "ItemsList" , typeof ( List<string> ) , typeof ( ComboboxPlus ) , new PropertyMetadata ( default ) );
        //++++++++++++++++++++++++++++++
        public IEnumerable MyItemsSource
        {
            get { return ( IEnumerable ) GetValue ( MyItemsSourceProperty ); }
            set
            {
                SetValue ( MyItemsSourceProperty , value );
                comboBox . ItemsSource = null;
                comboBox . Items . Clear ( );
                comboBox . ItemsSource = ItemsList;
            }
        }
        public static readonly DependencyProperty MyItemsSourceProperty =
            DependencyProperty . Register ( "MyItemsSource" , typeof ( IEnumerable ) , typeof ( ComboboxPlus ) , new PropertyMetadata ( default ) );
        //++++++++++++++++++++++++++++++
        public Style ComboStyle
        {
            get { return ( Style ) GetValue ( ComboStyleProperty ); }
            set { SetValue ( ComboStyleProperty , value ); }
        }
        public static readonly DependencyProperty ComboStyleProperty =
            DependencyProperty . Register ( "ComboStyle" , typeof ( Style ) , typeof ( ComboboxPlus ) , new PropertyMetadata ( default ) );
        //++++++++++++++++++++++++++++++
        public string DefaultText
        {
            get { return ( string ) GetValue ( DefaultTextProperty ); }
            set { SetValue ( DefaultTextProperty , value ); }
        }
        public static readonly DependencyProperty DefaultTextProperty =
                DependencyProperty . Register ( "DefaultText" , typeof ( string ) , typeof ( ComboboxPlus ) , new PropertyMetadata ( "Select Item ..." ) );
        //++++++++++++++++++++++++++++++
        new public Brush Background
        {
            get { return ( Brush ) GetValue ( BackgroundProperty ); }
            set { SetValue ( BackgroundProperty , value ); }
        }
        new public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty . Register ( "Background" , typeof ( Brush ) , typeof ( ComboboxPlus ) , new PropertyMetadata ( Brushes . Transparent ) );
        //++++++++++++++++++++++++++++++
        public int ItemSelected
        {
            get { return ( int ) GetValue ( ItemSelectedProperty ); }
            set { SetValue ( ItemSelectedProperty , value ); }
        }
        public static readonly DependencyProperty ItemSelectedProperty =
            DependencyProperty . Register ( "ItemSelected" , typeof ( int ) , typeof ( ComboboxPlus ) , new PropertyMetadata ( 0 ) );
        //++++++++++++++++++++++++++++++



        //public object SelectedItem {
        //    get { return GetValue ( SelectedItemProperty ); }
        //    set { SetValue ( SelectedItemProperty , value ); }
        //}
        //public static DependencyProperty SelectedItemProperty =
        //    DependencyProperty . Register ( "SelectedItem" , typeof ( object ) , typeof ( ComboboxPlus ) , new PropertyMetadata ( default ) );
        //++++++++++++++++++++++++++++++
        public int selectioncount
        {
            get { return ( int ) GetValue ( selectioncountProperty ); }
            set { SetValue ( selectioncountProperty , value ); }
        }
        public static readonly DependencyProperty selectioncountProperty =
            DependencyProperty . Register ( "selectioncount" , typeof ( int ) , typeof ( ComboboxPlus ) , new PropertyMetadata ( -1 ) );
        //++++++++++++++++++++++++++++++

        #endregion DP's

        public bool SelectActive { get; set; } = false;

        public ComboboxPlus ( )
        {
            InitializeComponent ( );
            //DataContext = this;
            comboBox . SelectionChanged += ComboBox_SelectionChanged;
            // initialize dataggrids array
            for ( int i = 0 ; i < 5 ; i++ )
            {
                datagrids [ i ] = new DataGrid ( );
                gridtablenames [ i ] = "";
            }
            ItemSelected = 0;
        }
        public void SetHost ( BankAcHost host )
        {
            Host = host;
            SetBankHost ( ( DependencyObject ) host , host );
        }
        static public ComboChangedArgs SetSwitchArguments (
            ComboBox CBplus ,
            BankAcHost BHost ,
            BlankScreenUC BlankCtrl ,
            GenericGridControl GenCtrl ,
            DataGrid [ ] grids ,
            string Activepanel ,
            string ActiveTablename = "" )
        {
            ComboChangedArgs args = new ComboChangedArgs ( );
            args . CBplus = CBplus;
            args . BHost = BHost;
            args . BlankCtrl = BlankCtrl;
            args . GenCtrl = GenCtrl;
            args . Activepanel = Activepanel;
            args . ActiveTablename = ActiveTablename;
            return args;
        }
        private void ComboboxPlus_ComboboxChanged ( object sender , ComboChangedArgs e )
        {
            int index = 0;
            string tablename = e . ActiveTablename . ToUpper ( );
            //**************************
            // BLANKPANEL IS ACTIVE  
            //**************************
            if ( e . Activepanel == "BLANKSCREEN" )
            {
                index = 0;
                if ( e . BHost != null )
                    e . BHost . Title = "BLANK GRID VIEWER";

                if ( e . grids [ index ] . Items . Count == 0 )
                    LoadGenericTable ( tablename , e . grids [ index ] );

                if ( e . grids [ index ] . Items . Count == 0 )
                {
                    // load new data 
                    e . grids [ index ] . ItemsSource = null;
                    e . grids [ index ] . Items . Clear ( );
                    SqlServerCommands . LoadActiveRowsOnlyInGrid ( e . grids [ index ] , GenCollection , SqlServerCommands . GetGenericColumnCount ( GenCollection ) );
                    GenericDbUtilities . ReplaceDataGridFldNames ( tablename , ref e . grids [ index ] );
                    e . grids [ index ] . SelectedIndex = 0;
                }
                e . grids [ index ] . Refresh ( );
            }
            else if ( CurrentPanel == "GENERICGRID" )
            {
            }
        }

        public void LoadNewData ( object sender , string tablename , string activepane , DataGrid grid )
        {
            LoadGenericTable ( tablename , grid );

            //Pass data loading back to BankAcHost to process
            // Get BankAcHost to handle the data loading etc for the combo selection
            //ComboChangedArgs args = new ComboChangedArgs ( );
            //args . Itemselected = tablename;
            //args . CBplus = sender as ComboBox;
            //ComboboxChanged . Invoke ( this , args );

            //// Get pointer to BankAcHst - WORKS!!
            //BankAcHost bh = new BankAcHost ( );
            //bh = GetBankHost ( ( DependencyObject ) bh );
        }

        static public bool LoadGenericTable ( string table , DataGrid grid )
        {
            int DbCount = LoadTableGeneric ( $"Select * from {table}" , ref GenCollection );
            if ( DbCount > 0 )
            {
                SqlServerCommands . LoadActiveRowsOnlyInGrid ( grid , GenCollection , SqlServerCommands . GetGenericColumnCount ( GenCollection ) );
                GenericDbUtilities . ReplaceDataGridFldNames ( table , ref grid );
                Debug . WriteLine ( $"grid has {grid . Items . Count} items from {table}" );
                return true;
            }
            return false;
        }
        static private int LoadTableGeneric ( string SqlCommand , ref ObservableCollection<GenericClass> GenCollection )
        {
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
        public ComboChangedArgs CreateComboArgs ( )
        {
            ComboChangedArgs args = new ComboChangedArgs ( );
            args . CBplus = comboBox;
            args . BHost = null;
            args . BlankCtrl = null;
            args . GenCtrl = null;
            args . grids [ 0 ] = null;
            args . Activepanel = "";
            args . ActiveTablename = "";
            return args;
        }

        private void ComboBox_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {
            if ( SelectActive ) return;
            SelectedItem = ( object ) e . AddedItems [ 0 ];
            string activepanel = BankAcHost . CurrentPanel;
            if ( activepanel == "GENERICGRID" )
            {
                var args = new ComboChangedArgs ( );
                //********************************************************
                // Send data request to GenericGridControl to handle it
                //********************************************************
                if ( GenericGridControl . ActiveGrid == 1 )
                {
                    this . gridtablenames [ 0 ] = selectedItem . ToString ( ) . ToUpper ( );
                    comboSelection1 = SelectedItem . ToString ( );
                    currentcomboSelection = comboSelection1;
                    args . ActiveTablename = this . gridtablenames [ 0 ];
                }
                else
                {
                    this . gridtablenames [ 1 ] = selectedItem . ToString ( ) . ToUpper ( );
                    comboSelection2 = SelectedItem . ToString ( );
                    currentcomboSelection = comboSelection2;
                    args . ActiveTablename = this . gridtablenames [ 1 ];
                }
                args . Activepanel = activepanel;
                ComboboxChanged . Invoke ( this , args );
            }
            ItemSelected = comboBox . SelectedIndex;
        }


        private void Promptlabel_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
        {
            Promptlabel . Visibility = Visibility . Collapsed;
            comboBox . Visibility = Visibility . Visible;
        }
        //++++++++++++++++++++++++++++++
    }
}
