using System;
using System . Collections;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Data;
using System . Diagnostics;
using System . Reflection;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Data;
using System . Windows . Input;
using System . Windows . Media;

using GenericSqlLib . Models;

using NewWpfDev . Models;
using NewWpfDev . ViewModels;
using NewWpfDev . Views;

using GenericClass = NewWpfDev . ViewModels . GenericClass;

namespace NewWpfDev . UserControls
{

    public partial class GenericGridControl : UserControl
    {

        [DefaultValue ( typeof ( double ) , "130" )]
        new public double Width { get; set; }

        [DefaultValue ( typeof ( double ) , "30" )]
        new public double Height { get; set; }

        public Size GenGrid1Size;
        public Size GenGrid2Size;
        public Size GenContentSize;


        #region Event Handling
        //======================================================================================//
        public static event EventHandler<StyleArgs> StylesSwitch;
        // Diifferent Style  has been selected, notify UserControl to handle it
        public void TriggerStyleSwitch ( object sender , StyleArgs e )
        {
            if ( StylesSwitch != null )
            {
                StylesSwitch ( this , e );
            }
            //OnStylesSwitch ( e );
        }
        //======================================================================================//
        #endregion Event Handling

        public List<string> AllStyles { get; set; } = new List<string> ( );

        #region normal File Declarations

        private static bool ShowColumnNames { get; set; } = true;
        public bool isInsertMode = false;
        public static int colcount = 0;
        public bool isBeingEdited = false;
        public static string CurrentTable { get; set; }
        public static string Title1 { get; set; }
        public static string Title2 { get; set; }
        public static string Style1 { get; set; } = "Dark Mode";
        public static string Style2 { get; set; } = "Dark Mode";
        public static bool NoUpdate { get; set; } = false;
        public static DataTable dt { get; set; }
        public static DataTable dt2 = new DataTable ( );
        public static List<GenericClass> list = new List<GenericClass> ( );
        //            IEnumerator ie = DataClass .GetEnumerator ();
        public static BankAcHost Host { get; set; }
        #endregion normal File Declarations

        //Data Collections
        public static ObservableCollection<GenericSqlLib . Models . GenericClass> Gencollection
        { get; set; } = new ObservableCollection<GenericSqlLib . Models . GenericClass> ( );
        public static ObservableCollection<GGenericClass> gGencollection
        { get; set; } = new ObservableCollection<GenericSqlLib . Models . GGenericClass> ( );
        // Class records
        public static GenericSqlLib . Models . GenericClass GenClass
        { get; set; } = new GenericSqlLib . Models . GenericClass ( );
        public static GenericSqlLib . Models . GGenericClass gGenClass
        { get; set; } = new GenericSqlLib . Models . GGenericClass ( );

        #region Full Properties

        private static DataGrid genericgrid;
        private static ObservableCollection<GenericClass> genclass;

        public static DataGrid GenericGrid
        {
            get { return genericgrid; }
            set { genericgrid = value; }
        }
        public ObservableCollection<GenericClass> GenCollection
        {
            get { return ( ObservableCollection<GenericClass> ) genclass; }
            set
            {
                ObservableCollection<GenericClass> genclass = value;
                if ( GenericGrid != null )
                {
                    GenericGrid . ItemsSource = null;
                    GenericGrid . Items . Clear ( );
                    GenericGrid . ItemsSource = value;
                    StylesList . ItemsSource = null;
                    StylesList . ItemsSource = value;
                    StylesList . Items = null;
                    StylesList . Items = value;
                }
            }
        }
        private void UpdateListbox ( )
        {
            StylesList . ItemsSource = null;
        }

        #endregion Full Properties

        public GenericGridControl (BankAcHost host )
        {
            InitializeComponent ( );
            Utils . ClearAttachedProperties ( this );
            Mouse . SetCursor ( Cursors . Wait );
            this . UpdateLayout ( );
            //          this . DataContext = this;  // DO NOT SET CONTEXT HERE !!!!!!  Do it in XAML code !
            GenericGrid = datagrid1;
            Thickness th = new Thickness ( );
            Host = host;
            th . Left = 10;
            datagrid1 . Margin = th;
            datagrid2 . Margin = th;
            GenericGrid . Margin = th;
            Mouse . SetCursor ( Cursors . Wait );
            AllStyles = Utils . GetAllDgStyles ( );
            // StylesList.DataContext = this;
            StylesList . ClearSource ( );
            StylesList . SetHost ( this );
            StylesList . Clear ( this , null );
            SetValue ( PopupListBox . ItemsSourceProperty , AllStyles );
            SetValue ( PopupListBox . StylescountProperty , AllStyles . Count );
            StylesList . AddItems ( AllStyles );
            StylesList . SelectedIndex = 0;
            CurrentTable = "BANKACCOUNT";
            Mouse . SetCursor ( Cursors . Wait );
            int colcount = GetDbColumnCount ( CurrentTable );
            //CreateGenericColumns ( colcount, datagrid2 );
            //CreateGridColumnHeaders ( colcount , datagrid2 );
            GenericGrid = datagrid1;
            datagrid2 . Visibility = Visibility . Collapsed;
            datagrid1 . Visibility = Visibility . Visible;
            Togglegrid . Content = "= Grid 1";
            Mouse . SetCursor ( Cursors . Arrow );
            StylesList . Stylechanged += GenericGridControl_Stylechanged;
            PopupListBox . StyleSizeChanged += PopupListBox_StyleSizeChanged;
            if ( Host != null )
            {
                GenGrid1Size . Height = Host . BankContent . Height;
                GenGrid1Size . Width = Host . BankContent . Width;
                GenGrid2Size . Height = Host . BankContent . Height;
                GenGrid2Size . Width = Host . BankContent . Width;
            }
        }
        public void ResizeControl ( double height , double width )
        {
            this . Height = height- 10;
            this . Width = width;
            this . Refresh ( );
        }

        private void PopupListBox_StyleSizeChanged ( object sender , SizeChangedArgs e )
        {    //return;
            Debug . WriteLine ( $"{e . NewHeight}" );
            StylesList . Height = ( double ) e . NewHeight;
            StylesList . UpdateLayout ( );
            StylesList . Refresh ( );
        }

        private void GenericGridControl_Stylechanged ( object sender , StyleArgs e )
        {
            if ( NoUpdate )
                return;
            string str = e . style;
            if ( str == "Dark Mode" )
            {
                if ( Togglegrid . Content . ToString ( ) . Contains ( "Grid 1" ) )
                {
                    datagrid1 . CellStyle = null;
                    datagrid1 . Foreground = Brushes . White;
                    Style1 = str;
                }
                else
                {
                    datagrid2 . CellStyle = null;
                    datagrid2 . Foreground = Brushes . White;
                    Style2 = str;
                }
                StyleArgs args = new StyleArgs ( );
                args . style = str;
                args . sender = this;
                // Notify control itself
                TriggerStyleSwitch ( this , args );
                return;
            }
            else
            {
                Style style = FindResource ( str ) as Style;
                if ( Togglegrid . Content . ToString ( ) . Contains ( "Grid 1" ) )
                {
                    datagrid1 . CellStyle = style;
                    Style1 = str;
                }
                else
                {
                    datagrid2 . CellStyle = style;
                    Style2 = str;
                }
                StyleArgs args = new StyleArgs ( );
                args . style = str;
                args . sender = this;
                // Notify control itself
                TriggerStyleSwitch ( this , args );
                return;
            }
        }
        public void UpDateStyle ( string str )
        {
            if ( Togglegrid . Content . ToString ( ) . Contains ( "Grid 1" ) )
            {
                datagrid1 . CellStyle = null;
                datagrid1 . Foreground = Brushes . White;
                Style1 = str;
            }
            else
            {
                datagrid2 . CellStyle = null;
                datagrid2 . Foreground = Brushes . White;
                Style2 = str;
            }

        }
        public void CreateGridColumnHeaders ( int colcount , DataGrid grid )
        {
            grid . Columns . Clear ( );
            for ( int x = 0 ; x < colcount ; x++ )
            {
                // can  be any column type !
                DataGridTextColumn tc = new DataGridTextColumn ( );
                // column internal name
                Binding b = new Binding ( $"Col {x + 1}" );
                tc . Binding = b;
                // Column title
                tc . Header = $"Col {x + 1}";
                // Add to grid
                grid . Columns . Add ( tc );
            }
        }
        private int GetDbColumnCount ( string table )
        {
            int retval = 0;
            if ( table == "BANKACCOUNT" )
                retval = 8;
            else if ( table == "CUSTOMER" )
                retval = 14;
            else
                BankAcHost . GetColumnNames ( table , out retval , "IAN1" );
            return retval;
        }

        public async Task<bool> LoadGenericTable ( string table )
        {
            //string con = $"Data Source=DINO-PC;Initial Catalog=\"IAN1\";Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            Utils . LoadConnectionStrings ( );
            string ConString = "";
            Utils . CheckResetDbConnection ( "IAN1" , out ConString );
            string con = Utils . GetDictionaryEntry ( Flags . ConnectionStringsDict , "IAN1" , out string dictvalue );
            Stopwatch sw = new Stopwatch();
            sw . Start ( );
            // ObservableCollection<GenericClass> LoadGeneric = 
            //ObservableCollection<GenericSqlLib . Models . GenericClass> Gencoll =
                Gencollection= await Task . Run ( ( ) => GenericSqlLib . LoadSql . LoadGeneric ( $"Select * from {table}" , out string ResultString , 0 , false , false , con )
                );
            sw . Stop ( );
            Debug . WriteLine ($"Data load Task took {sw.ElapsedMilliseconds} msecs");
           //Task . WaitAll ( );     
              //Gencollection = GenericSqlLib . LoadSql .
            //   LoadGeneric ( $"Select * from {table}" , out string ResultString , 0 , false , false , con ); 
            
            GenericSqlLib . SqlServerCommands . LoadActiveRowsOnlyInGrid (
                datagrid1 , Gencollection ,
                GenericSqlLib . SqlServerCommands . GetGenericColumnCount ( Gencollection ) );
            int colcount = GenericSqlLib . SqlServerCommands . GetGenericColumnCount ( Gencollection );
            GenericSqlLib . SqlServerCommands . LoadActiveRowsOnlyInGrid (
                datagrid2 , Gencollection , colcount );
            if ( ShowColumnNames )
            {
                GenericDbUtilities . ReplaceDataGridFldNames ( table , ref datagrid1 );
                GenericDbUtilities . ReplaceDataGridFldNames ( table , ref datagrid2 );
            }
            Debug . WriteLine ( $"Both grids have {datagrid1 . Items . Count} items from {table}" );
            CurrentTable = table;
            GenericTitle . Text = $"Table = {table . ToUpper ( )}";
            return true;
        }
        public GenericClass ConvertClass ( GGenericClass GCollection , GenericClass collection )
        {
            collection . field1 = GCollection . field1;
            collection . field2 = GCollection . field2;
            collection . field3 = GCollection . field3;
            collection . field4 = GCollection . field4;
            collection . field5 = GCollection . field5;
            collection . field6 = GCollection . field6;
            collection . field7 = GCollection . field7;
            collection . field8 = GCollection . field8;
            collection . field9 = GCollection . field9;
            collection . field10 = GCollection . field10;
            collection . field11 = GCollection . field11;
            collection . field12 = GCollection . field12;
            collection . field13 = GCollection . field13;
            collection . field14 = GCollection . field14;
            collection . field15 = GCollection . field15;
            collection . field16 = GCollection . field16;
            collection . field17 = GCollection . field17;
            collection . field18 = GCollection . field18;
            collection . field19 = GCollection . field19;
            collection . field20 = GCollection . field20;
            return collection;
        }

        static public void GetDbTablesList ( string DbName , out List<string> TablesList )
        {
            TablesList = new List<string> ( );
            string SqlCommand = "";
            List<string> list = new List<string> ( );
            DbName = DbName . ToUpper ( );
            if ( Utils . CheckResetDbConnection ( DbName , out string constr ) == false )
            {
                Debug . WriteLine ( $"Failed to set connection string for {DbName} Db" );
                return;
            }
            // All Db's have their own version of this SP.....
            SqlCommand = "spGetTablesList";

            Datagrids . CallStoredProcedure ( list , SqlCommand );
            //This call returns us a DataTable
            DataTable dt = DataLoadControl . GetDataTable ( SqlCommand );
            // This how to access Row data from  a grid the easiest way.... parsed into a List <xxxxx>
            if ( dt != null )
            { TablesList = Utils . GetDataDridRowsAsListOfStrings ( dt ); }
        }

        private void genkeydown ( object sender , KeyEventArgs e )
        {
            if ( e . Key == Key . F8 )
                Debugger . Break ( );
        }

        private void GenGridControl_SizeChanged ( object sender , SizeChangedEventArgs e )
        {
            datagrid1 . Width = this . ActualWidth - 30;
            datagrid1 . Height = this . ActualHeight - 85;
            datagrid2 . Width = this . ActualWidth - 30;
            datagrid2 . Height = this . ActualHeight - 80;
        }

        #region Buttons handlers

        private void Button1_Click ( object sender , RoutedEventArgs e )
        {
            // clear grid
            datagrid1 . ItemsSource = null;
            datagrid1 . Items . Clear ( );
            datagrid1 . Refresh ( );
            GenericTitle . Text = "";
        }

        // Toggle columhn header content
        private void Button2_Click ( object sender , RoutedEventArgs e )
        {
            if ( ShowColumnNames )
            {
                int indexer = 1;
                ShowColumnNames = false;
                maskcols . Content = "Show Columns";
                foreach ( var item in datagrid1 . Columns )
                {
                    DataGridColumn dgc = item;
                    if ( indexer <= datagrid1 . Columns . Count )
                        dgc . Header = $"Field{indexer++}";
                }
                indexer = 1;
                foreach ( var item in datagrid2 . Columns )
                {
                    DataGridColumn dgc = item;
                    if ( indexer <= datagrid2 . Columns . Count )
                        dgc . Header = $"Field{indexer++}";
                }
                datagrid1 . Refresh ( );
                datagrid2 . Refresh ( );
            }
            else
            {
                ShowColumnNames = true;
                maskcols . Content = "Mask Columns";
                DataGrid tmpgrid = new DataGrid ( );
                tmpgrid = datagrid1;
                GenericDbUtilities . ReplaceDataGridFldNames ( CurrentTable , ref tmpgrid );
                tmpgrid = datagrid2;
                GenericDbUtilities . ReplaceDataGridFldNames ( CurrentTable , ref tmpgrid );
            }
        }

        private void AddNew ( object sender , RoutedEventArgs e )
        {
            if ( addnewrow . Content . ToString ( ) == "Add New" )
            {
                canUserAddRows = true;
                Gencollection . Add ( new GenericSqlLib . Models . GenericClass ( ) );
                datagrid1 . ItemsSource = Gencollection;
                datagrid1 . SelectedIndex = datagrid1 . Items . Count;
                datagrid1 . Refresh ( );
                datagrid1 . UpdateLayout ( );
                updaterow . IsEnabled = false;
                Utils . ScrollRecordIntoView ( datagrid1 , datagrid1 . SelectedIndex );
                addnewrow . Content = "Save Account";
                datagrid1 . Focus ( );
            }
            else
            {
                // Save the new row
                updaterow . IsEnabled = true;
                addnewrow . Content = "Add New";
                // stop adding new records as default policy
                canUserAddRows = false;
                UpdateRecord ( sender , e );
            }
        }

        private void UpdateRecord ( object sender , RoutedEventArgs e )
        {
            // Update record
            string [ ] fieldnames = new string [ 24 ];
            int index = 0;
            GenClass = datagrid1 . SelectedItem as GenericSqlLib . Models . GenericClass;
            for ( int x = 0 ; x < fieldnames . Length ; x++ )
            { fieldnames [ x ] = ""; }

            foreach ( DataGridColumn dgc in datagrid1 . Columns )
            {
                fieldnames [ index++ ] = dgc . Header . ToString ( );
            }
            string SqlCommand = $"UPDATE {CurrentTable} SET BANKNO=@bankno, CUSTNO=@custno, ACTYPE=@actype, " +
                            "INTRATE=@intrate, ODATE=@odate, CDATE=@cdate where CUSTNO = @custno";
        }

        #endregion Buttons handlers
        private void dgProducts_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
        {
            //Debugger . Break ( );
        }

        private void Togglegrid_Click ( object sender , RoutedEventArgs e )
        {
            if ( datagrid1 . Visibility == Visibility . Visible )
            {
                datagrid1 . Visibility = Visibility . Collapsed;
                datagrid2 . Visibility = Visibility . Visible;
                GenericGrid = datagrid2;
                Togglegrid . Content = "= Grid 2";
                //datagrid2 . Width -= 10;
                Thickness th = datagrid2 . Margin;
                th . Bottom = 20;
                datagrid2 . Margin = th;
                datagrid2 . Refresh ( );
                datagrid2 . UpdateLayout ( );
                GenericTitle . Text = Title2;
                StyleArgs args = new StyleArgs ( );
                args . style = Style2;
                TriggerStyleSwitch ( this , args );
            }
            else
            {
                datagrid2 . Visibility = Visibility . Collapsed;
                datagrid1 . Visibility = Visibility . Visible;
                GenericGrid = datagrid1;
                Togglegrid . Content = "= Grid 1";
                Thickness th = datagrid1 . Margin;
                th . Bottom =50;
                th . Right =20;
                datagrid1 . Margin = th;
                //datagrid1 . Width -= 10;
                //datagrid1. Height -= 10;
                datagrid1 . Refresh ( );
                GenericTitle . Text = Title1;
                StyleArgs args = new StyleArgs ( );
                args . style = Style1;
                TriggerStyleSwitch ( this , args );
            }
        }

        private void CreateGenericColumns ( int maxcols , DataGrid grid1 )
        {
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
            if ( maxcols == 21 ) return;
            DataGridTextColumn c22 = new DataGridTextColumn ( );
            c21 . Header = "Col 22";
            c21 . Binding = new Binding ( "field22" );
            grid1 . Columns . Add ( c22 );
            if ( maxcols == 22 ) return;
            DataGridTextColumn c23 = new DataGridTextColumn ( );
            c21 . Header = "Col 23";
            c21 . Binding = new Binding ( "field23" );
            grid1 . Columns . Add ( c23 );
            if ( maxcols == 23 ) return;
            DataGridTextColumn c24 = new DataGridTextColumn ( );
            c21 . Header = "Col 24";
            c21 . Binding = new Binding ( "field24" );
            grid1 . Columns . Add ( c24 );
        }

        private void StylesCombo_MouseEnter ( object sender , MouseEventArgs e )
        {
            StylesList . Opacity = 0.7;
            return;
        }

        private void StylesCombo_MouseLeave ( object sender , MouseEventArgs e )
        {
            StylesList . Opacity = 1.0;
            return;
        }

        #region DP's
        public SelectionMode Selectionmode
        {
            get { return ( SelectionMode ) GetValue ( SelectionmodeProperty ); }
            set { SetValue ( SelectionmodeProperty , value ); }
        }
        public static readonly DependencyProperty SelectionmodeProperty =
            DependencyProperty . Register ( "Selectionmode" , typeof ( SelectionMode ) , typeof ( GenericGridControl ) , new PropertyMetadata ( SelectionMode . Multiple ) );

        public bool autoGenerateColumns
        {
            get { return ( bool ) GetValue ( autoGenerateColumnsProperty ); }
            set { SetValue ( autoGenerateColumnsProperty , value ); }
        }
        public static readonly DependencyProperty autoGenerateColumnsProperty =
            DependencyProperty . Register ( "autoGenerateColumns" , typeof ( bool ) , typeof ( GenericGridControl ) , new PropertyMetadata ( false ) );

        public bool canUserAddRows
        {
            get { return ( bool ) GetValue ( canUserAddRowsProperty ); }
            set { SetValue ( canUserAddRowsProperty , value ); }
        }
        public static readonly DependencyProperty canUserAddRowsProperty =
            DependencyProperty . Register ( "canUserAddRows" , typeof ( bool ) , typeof ( GenericGridControl ) , new PropertyMetadata ( false ) );

        public Brush Rowbackground
        {
            get { return ( Brush ) GetValue ( RowbackgroundProperty ); }
            set { SetValue ( RowbackgroundProperty , value ); }
        }
        public static readonly DependencyProperty RowbackgroundProperty =
           DependencyProperty . Register ( "Rowbackground" , typeof ( Brush ) , typeof ( GenericGridControl ) , new PropertyMetadata ( Brushes . Transparent ) );

        public Brush background
        {
            get { return ( Brush ) GetValue ( backgroundProperty ); }
            set { SetValue ( backgroundProperty , value ); }
        }
        public static readonly DependencyProperty backgroundProperty =
           DependencyProperty . Register ( "background" , typeof ( Brush ) , typeof ( GenericGridControl ) , new PropertyMetadata ( Brushes . Gray ) );

        public Brush foreground
        {
            get { return ( Brush ) GetValue ( foregroundProperty ); }
            set { SetValue ( foregroundProperty , value ); }
        }
        public static readonly DependencyProperty foregroundProperty =
           DependencyProperty . Register ( "foreground" , typeof ( Brush ) , typeof ( GenericGridControl ) , new PropertyMetadata ( Brushes . White ) );

        public Thickness margin
        {
            get { return ( Thickness ) GetValue ( marginProperty ); }
            set { SetValue ( marginProperty , value ); }
        }
        public static readonly DependencyProperty marginProperty =
           DependencyProperty . Register ( "margin" , typeof ( Thickness ) , typeof ( GenericGridControl ) , new PropertyMetadata ( new Thickness { Left = 10 , Top = 10 , Bottom = 10 , Right = 10 } ) );

        public double fontsize
        {
            get { return ( double ) GetValue ( fontsizeProperty ); }
            set { SetValue ( fontsizeProperty , value ); }
        }
        public static readonly DependencyProperty fontsizeProperty =
            DependencyProperty . Register ( "fontsize" , typeof ( double ) , typeof ( GenericGridControl ) , new PropertyMetadata ( ( double ) 15 ) );

        public ScrollBarVisibility vscrollBar
        {
            get { return ( ScrollBarVisibility ) GetValue ( vscrollBarProperty ); }
            set { SetValue ( vscrollBarProperty , value ); }
        }
        public static readonly DependencyProperty vscrollBarProperty =
            DependencyProperty . Register ( "vscrollBar" , typeof ( ScrollBarVisibility ) ,
                typeof ( GenericGridControl ) , new PropertyMetadata ( ( ScrollBarVisibility ) 1 ) );

        #endregion DP's

        #region UNUSED METHODS

        static private ObservableCollection<GenericSqlLib . Models . GenericClass> LoadTableGeneric (
            string SqlCommand , ObservableCollection<GenericSqlLib . Models . GenericClass> GenCollection )
        {
            List<string> list2 = new ( );
            GenCollection . Clear ( );
            string errormsg = "";
            int DbCount = 0;
            DbCount = GenericSqlLib . Dapper . DapperSupport . CreateGenericCollection (
            ref GenCollection ,
           SqlCommand ,
            "" ,
            "" ,
            "" ,
            ref list2 ,
            ref errormsg );

            return GenCollection;
        }

        #region Edit/Add/Update
        private void dgProducts_AddingNewItem ( object sender , AddingNewItemEventArgs e )
        { isInsertMode = true; }
        private void dgProducts_BeginningEdit ( object sender , DataGridBeginningEditEventArgs e )
        { isBeingEdited = true; }
        private void dgProducts_RowEditEnding ( object sender , DataGridRowEditEndingEventArgs e )
        { }
        private void dgProducts_PreviewKeyDown ( object sender , KeyEventArgs e )
        { }
        #endregion Edit/Add/Update

        public void LoadGenericTableStatic ( string table )
        {
            LoadGenericTable ( table );
        }
        public static DataTable GetDataTableCount ( string Tablename , DataTable dt , out int count )
        {
            count = 0;
            //DataSet ds = new DataSet ( "GenericDataSet" );
            //ds . Tables . Add ( dt );
            //string ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
            //using ( SqlConnection conn = new SqlConnection ( ConString ) ) {
            //    SqlCommand cmd = conn . CreateCommand ( );
            //    cmd . CommandType = CommandType . Text;
            //    cmd . CommandText = $"SELECT * FROM {Tablename}";
            //    SqlDataAdapter da = new SqlDataAdapter ( cmd );
            //    da . Fill ( ds );
            //}
            //dt = CreateTableColumns ( ds . Tables [ 1 ] . Columns . Count );
            //count = dt . Columns . Count;
            return dt;
        }
        public static DataTable GetDataTable ( string Tablename , DataTable dt )
        {
            //DataSet ds = new DataSet ( "GenericDataSet" );
            //ds . Tables . Add ( dt );
            //string ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
            //using ( SqlConnection conn = new SqlConnection ( ConString ) ) {
            //    SqlCommand cmd = conn . CreateCommand ( );
            //    cmd . CommandType = CommandType . Text;
            //    cmd . CommandText = $"SELECT * FROM {Tablename}";
            //    SqlDataAdapter da = new SqlDataAdapter ( cmd );
            //    da . Fill ( ds );
            //}
            //return ds . Tables [ 1 ];
            return dt;
        }
        private static List<T> ConvertDataTableToList<T> ( DataTable dt , out int Columncount )
        {
            List<T> data = new List<T> ( );
            Columncount = 0;
            foreach ( DataRow row in dt . Rows )
            {
                T item = GetItem<T> ( row );
                data . Add ( item );
                Columncount++;
            }
            return data;
        }
        private static T GetItem<T> ( DataRow dr )
        {
            Type temp = typeof ( T );
            T obj = Activator . CreateInstance<T> ( );
            int index = 0;
            //foreach ( DataColumn column in dr . Table . Columns ) {
            for ( int x = 0 ; x < dr . Table . Columns . Count - 1 ; x++ )
            {
                index = 0;
                foreach ( PropertyInfo pro in temp . GetProperties ( ) )
                {
                    if ( index < dr . Table . Columns . Count )
                        pro . SetValue ( obj , dr [ index++ ] . ToString ( ) );
                    else break;
                    //else
                    //    continue;
                }
                //                Columncount = x;
                return obj;
            }
            return obj;
        }
        private static DataTable CreateTableColumns ( int max )
        {
            int counter = 0;
            dt = new DataTable ( );
            {   // unused
                //dt . Columns . Add ( "field1" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field2" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field3" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field4" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field5" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field6" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field7" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field8" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field9" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field10" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field11" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field12" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field13" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field14" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field15" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field16" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field17" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field18" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field19" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field20" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field21" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field22" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field23" , typeof ( string ) );
                //if ( ++counter >= max )
                //    return dt;
                //dt . Columns . Add ( "field24" , typeof ( string ) );
            }
            return dt;
        }
        private void CreateDgStyleslist ( )
        {
            datagrid1 . CellStyle = FindResource ( "xxxtypeGridStyle" ) as Style;

        }

        #region UNUSED
        // private List<T> GetListFromDt ( DataTable dt ) {

        //    var objType = typeof(T);
        //    IDictionary<Type, ICollection<PropertyInfo>> _Properties = new Dictionary<Type, ICollection<PropertyInfo>>();
        //    ICollection<PropertyInfo> properties;
        //    lock (_Properties)
        //    {
        //        if (!_Properties.TryGetValue(objType, out properties))
        //        {
        //            properties = objType.GetProperties().Where(property => property.CanWrite).ToList();
        //            _Properties.Add(objType, properties);
        //        }
        //    }

        //    var list = new List<T>(dt.Rows.Count);
        //    var obj = new T();

        //    foreach (var item in dt.Rows)
        //    {

        //        foreach (var prop in properties)
        //        {
        //            try
        //            {
        //                var propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
        //                var safeValue = row[prop.Name] == null ? null : Convert.ChangeType(item.row[prop.Name], propType);
        //                prop.SetValue(obj, safeValue, null);
        //            }
        //            catch
        //            {
        //                continue;
        //            }
        //        }
        //        list.Add(obj); return
        //}
        //}
        //public ObservableCollection<T> GetData(string tablename)
        //{
        //    list = ReadSqlData(tablename);
        //    foreach (var item in list)
        //    {
        //        collection.Add(item);
        //    }
        //    return collection;
        //}


        //public List<DataClass> ReadSqlData(string Tablename)
        //{
        //    DataSet ds = new DataSet("MyDataSet");
        //    ///string ConString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        //    var ConString = @"Data Source = (localdb)\MSSQLLocalDB; Initial Catalog = C:\USERS\IANCH\DOCUMENTS\IAN1.MDF; Integrated Security = True; Connect Timeout = 30; Encrypt = False; TrustServerCertificate = False; ApplicationIntent = ReadWrite; MultiSubnetFailover = False";
        //    using (SqlConnection conn = new SqlConnection(ConString))
        //    {
        //        SqlCommand cmd = conn.CreateCommand();
        //        cmd.CommandType = CommandType.Text;
        //        cmd.CommandText = "SELECT * FROM Products";
        //        SqlDataAdapter da = new SqlDataAdapter(cmd);
        //        da.Fill(ds);
        //    }

        //    Type classtype = typeof(T);
        //    DataTable dataview = ds.Tables[0];
        //    IEnumerable<DataClass> prods = dataview.DataTableToList<DataClass>();
        //    //            var categoryList = new List<Category>(table.Rows.Count);
        //    foreach (var row in prods)
        //    {
        //        list.Add(row);
        //        {

        //            //var values = row.ItemArray;
        //            //var product = new Products()
        //            //{
        //            //    Id = Convert.ToInt32(values[0]),
        //            //    ProductCode = values[1].ToString(),
        //            //    ProductDescription = values[2].ToString(),
        //            //    ProductPrice = (float)Convert.ToDecimal(values[3]),
        //            //    ProductExpirationDate = Convert.ToDateTime(values[4]),
        //            //    ProductStockQuantity = Convert.ToInt32(values[5]),
        //            //    ProducVatRate = (float)Convert.ToDecimal(values[6]),
        //            //    IsBio = Convert.ToByte(values[7])
        //            //};
        //        }
        //    }
        //    return list;


        //    DataTable dt = GetDataTable(Tablename);
        //    GetDataToList(T, dt);
        //    foreach (var row in dt)
        //    {
        //        list.Add(row);
        //    }
        //    return list;
        //}
        //private List<T> GetDataToList(DataTable dataview)
        //{
        //    IEnumerable<T> prods = dataview.DataTableToList<dataview>();
        //    foreach (var row in prods)
        //    {
        //        list.Add(row);
        //    }
        //    return list;
        //}


        //private void dgProducts_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        //{
        //    Products product = new Products();
        //    Products curProd = e.Row.DataContext as Products;
        //    if (isInsertMode)
        //    {
        //        var InsertRecord = MessageBox.Show("Do you want to add " + curProd.ProductCode + " as a new product?", "Confirm", MessageBoxButton.YesNo,

        //            MessageBoxImage.Question);
        //        if (InsertRecord == MessageBoxResult.Yes)
        //        {
        //            product.ProductCode = curProd.ProductCode;
        //            product.ProductDescription = curProd.ProductDescription;
        //            product.ProductPrice = curProd.ProductPrice;
        //            product.ProductExpirationDate = curProd.ProductExpirationDate;
        //            products.Add(product);
        //            context.SaveChanges();
        //            dgProducts.ItemsSource = GetProductList();
        //            MessageBox.Show(product.ProductCode + " " + product.ProductDescription + " has being added!", "Add product", MessageBoxButton.OK, MessageBoxImage.Information);
        //            isInsertMode = false;

        //        }
        //        else
        //            dgProducts.ItemsSource = GetProductList();
        //    }
        //    context.SaveChanges();
        //}

        #endregion UNUSED

        #endregion UNUSED METHODS 

    }
}