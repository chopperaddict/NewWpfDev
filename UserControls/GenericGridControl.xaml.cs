using System;
using System . Collections;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . Data;
using System . Data . SqlClient;
using System . Diagnostics;
using System . Linq;
using System . Reflection;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;
using System . Windows . Media;

using NewWpfDev . ViewModels;
using NewWpfDev . Views;

using ServiceStack;

namespace NewWpfDev . UserControls {

    public partial class GenericGridControl : UserControl {

        public static ObservableCollection<GenericClass> collection { get; set; }
        public bool isInsertMode = false;
        public static int colcount = 0;
        public bool isBeingEdited = false;
        public static DataTable dt { get; set; }
        public static DataTable dt2 =new DataTable();
        public static List<GenericClass> list = new List<GenericClass> ( );
        //            IEnumerator ie = DataClass .GetEnumerator ();

        public GenericGridControl ( ) {
            InitializeComponent ( );
            //          this . DataContext = this;  // DO NOT SET CONTEXT HERE !!!!!!  Do it in XAML code !
            GenCollection = new ObservableCollection<GenericClass> ( );
            collection = new ObservableCollection<GenericClass> ( );
            GenericGrid = datagrid1;
            Thickness th = new Thickness ( );
            th . Top = 130;
            th . Left = 10;
            GenericGrid . Margin = th; 
            //         LoadDataGrid ("Customers" , "Select top (200) from Customers" );
        }
        public static ObservableCollection<GenericClass> LoadDataGrid ( string filename , string SqlCommand = "" ) {
            DataTable dt = new DataTable ( );
            // Also creats a valid DT structure for correct # columns
           dt = CreateTableColumns ( 24 );
            // These next 2 methods combined  will return us a Datatable with correct total columns to match Db requested
            dt2 = GetDataTableCount ( "BankAccount", dt, out colcount );
            dt = GetDataTable ( "BankAccount" , dt2 );
            list = ConvertDataTableToList<GenericClass> ( dt,  out colcount );
            foreach ( var item in list ) {
                collection . Add ( item );
            }
            GenericGrid . ItemsSource = collection;
            GenericGrid . SelectedIndex = 0;
            return collection;
        }

        #region Full Properties

        private static DataGrid genericgrid;
        public static DataGrid GenericGrid {
            get { return genericgrid; }
            set { genericgrid = value; }
        }
        private static ObservableCollection<GenericClass> genclass;
        public static ObservableCollection<GenericClass> GenCollection {
            get { return ( ObservableCollection<GenericClass> ) genclass; }
            set {
                ObservableCollection<GenericClass> genclass = value;
                if ( GenericGrid != null ) {
                    GenericGrid . ItemsSource = null;
                    GenericGrid . Items . Clear ( );
                    GenericGrid . ItemsSource = value;
                }
            }
        }
        #endregion Full Properties

        #region DP's
        public SelectionMode Selectionmode {
            get { return ( SelectionMode ) GetValue ( SelectionmodeProperty ); }
            set { SetValue ( SelectionmodeProperty , value ); }
        }
        public static readonly DependencyProperty SelectionmodeProperty =
            DependencyProperty . Register ( "Selectionmode" , typeof ( SelectionMode ) , typeof ( GenericGridControl ) , new PropertyMetadata ( SelectionMode . Multiple ) );

        public bool autoGenerateColumns {
            get { return ( bool ) GetValue ( autoGenerateColumnsProperty ); }
            set { SetValue ( autoGenerateColumnsProperty , value ); }
        }
        public static readonly DependencyProperty autoGenerateColumnsProperty =
            DependencyProperty . Register ( "autoGenerateColumns" , typeof ( bool ) , typeof ( GenericGridControl ) , new PropertyMetadata ( false ) );

        public bool canUserAddRows {
            get { return ( bool ) GetValue ( canUserAddRowsProperty ); }
            set { SetValue ( canUserAddRowsProperty , value ); }
        }
        public static readonly DependencyProperty canUserAddRowsProperty =
            DependencyProperty . Register ( "canUserAddRows" , typeof ( bool ) , typeof ( GenericGridControl ) , new PropertyMetadata ( false ) );

        public Brush Rowbackground {
            get { return ( Brush ) GetValue ( RowbackgroundProperty ); }
            set { SetValue ( RowbackgroundProperty , value ); }
        }
        public static readonly DependencyProperty RowbackgroundProperty =
           DependencyProperty . Register ( "Rowbackground" , typeof ( Brush ) , typeof ( GenericGridControl ) , new PropertyMetadata ( Brushes . Yellow ) );

        public Brush background {
            get { return ( Brush ) GetValue ( backgroundProperty ); }
            set { SetValue ( backgroundProperty , value ); }
        }
        public static readonly DependencyProperty backgroundProperty =
           DependencyProperty . Register ( "background" , typeof ( Brush ) , typeof ( GenericGridControl ) , new PropertyMetadata ( Brushes . Gray ) );

        public Brush foreground {
            get { return ( Brush ) GetValue ( foregroundProperty ); }
            set { SetValue ( foregroundProperty , value ); }
        }
        public static readonly DependencyProperty foregroundProperty =
           DependencyProperty . Register ( "foreground" , typeof ( Brush ) , typeof ( GenericGridControl ) , new PropertyMetadata ( Brushes . Black ) );

        public Thickness margin {
            get { return ( Thickness ) GetValue ( marginProperty ); }
            set { SetValue ( marginProperty , value ); }
        }
        public static readonly DependencyProperty marginProperty =
           DependencyProperty . Register ( "margin" , typeof ( Thickness ) , typeof ( GenericGridControl ) , new PropertyMetadata ( new Thickness { Left = 10 , Top = 10 , Bottom = 10 , Right = 10 } ) );
        public double fontsize {
            get { return ( double ) GetValue ( fontsizeProperty ); }
            set { SetValue ( fontsizeProperty , value ); }
        }
        public static readonly DependencyProperty fontsizeProperty =
            DependencyProperty . Register ( "fontsize" , typeof ( double ) , typeof ( GenericGridControl ) , new PropertyMetadata ( ( double ) 15 ) );
        public ScrollBarVisibility vscrollBar {
            get { return ( ScrollBarVisibility ) GetValue ( vscrollBarProperty ); }
            set { SetValue ( vscrollBarProperty , value ); }
        }
        public static readonly DependencyProperty vscrollBarProperty =
            DependencyProperty . Register ( "vscrollBar" , typeof ( ScrollBarVisibility ) , typeof ( GenericGridControl ) , new PropertyMetadata ( default ) );

        #endregion DP's

        //private void createtype ( object data ) {
        //    //GenericClass X = 1;
        //    Type listype = typeof ( List<GenericClass> );
        //    Type constructed = listype . MakeGenericType ( data . GetType ( ) );
        //    object runtimeList = Activator . CreateInstance ( constructed );
        //}

        private static ObservableCollection<GenericClass> GetProductList ( DataTable dt ) {
            return collection;
        }

        public static DataTable GetDataTableCount ( string Tablename , DataTable dt , out int count) {
            count = 0;
            DataSet ds = new DataSet ( "GenericDataSet" );
            ds . Tables . Add ( dt );
            string ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
            using ( SqlConnection conn = new SqlConnection ( ConString ) ) {
                SqlCommand cmd = conn . CreateCommand ( );
                cmd . CommandType = CommandType . Text;
                cmd . CommandText = $"SELECT * FROM {Tablename}";
                SqlDataAdapter da = new SqlDataAdapter ( cmd );
                da . Fill ( ds );
            }
            dt = CreateTableColumns ( ds . Tables[1].Columns . Count );
            count = dt . Columns . Count;
            return dt;
        }
        public static DataTable GetDataTable ( string Tablename , DataTable dt ) {
            DataSet ds = new DataSet ( "GenericDataSet" );
            ds . Tables . Add ( dt );
            string ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
            using ( SqlConnection conn = new SqlConnection ( ConString ) ) {
                SqlCommand cmd = conn . CreateCommand ( );
                cmd . CommandType = CommandType . Text;
                cmd . CommandText = $"SELECT * FROM {Tablename}";
                SqlDataAdapter da = new SqlDataAdapter ( cmd );
                da . Fill ( ds );
            }
            return ds . Tables [ 1 ];
        }

        private static List<T> ConvertDataTableToList<T> ( DataTable dt , out int Columncount ) {
            List<T> data = new List<T> ( );
            Columncount = 0;
            foreach ( DataRow row in dt . Rows ) {
                T item = GetItem<T> ( row );
                data . Add ( item );
            }
            return data;
        }
        private static T GetItem<T> ( DataRow dr ) {
            Type temp = typeof ( T );
            T obj = Activator . CreateInstance<T> ( );
            int index = 0;
            //foreach ( DataColumn column in dr . Table . Columns ) {
            for ( int x = 0 ; x < dr . Table . Columns . Count - 1 ; x++ ) {
                index = 0;
                foreach ( PropertyInfo pro in temp . GetProperties ( ) ) {
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
        private static DataTable CreateTableColumns ( int max) {
            int counter = 0;
            dt = new DataTable ( );
            dt . Columns . Add ( "field1" , typeof ( string ) );
            if(++counter >= max)
                return dt;
            dt . Columns . Add ( "field2" , typeof ( string ) );
            if ( ++counter >= max )
                return dt;
            dt . Columns . Add ( "field3" , typeof ( string ) );
            if ( ++counter >= max )
                return dt;
            dt . Columns . Add ( "field4" , typeof ( string ) );
            if ( ++counter >= max )
                return dt;
            dt . Columns . Add ( "field5" , typeof ( string ) );
            if ( ++counter >= max )
                return dt;
            dt . Columns . Add ( "field6" , typeof ( string ) );
            if ( ++counter >= max )
                return dt;
            dt . Columns . Add ( "field7" , typeof ( string ) );
            if ( ++counter >= max )
                return dt;
            dt . Columns . Add ( "field8" , typeof ( string ) );
            if ( ++counter >= max )
                return dt;
            dt . Columns . Add ( "field9" , typeof ( string ) );
            if ( ++counter >= max )
                return dt;
            dt . Columns . Add ( "field10" , typeof ( string ) );
            if ( ++counter >= max )
                return dt;
            dt . Columns . Add ( "field11" , typeof ( string ) );
            if ( ++counter >= max )
                return dt;
            dt . Columns . Add ( "field12" , typeof ( string ) );
            if ( ++counter >= max )
                return dt;
            dt . Columns . Add ( "field13" , typeof ( string ) );
            if ( ++counter >= max )
                return dt;
            dt . Columns . Add ( "field14" , typeof ( string ) );
            if ( ++counter >= max )
                return dt;
            dt . Columns . Add ( "field15" , typeof ( string ) );
            if ( ++counter >= max )
                return dt;
            dt . Columns . Add ( "field16" , typeof ( string ) );
            if ( ++counter >= max )
                return dt;
            dt . Columns . Add ( "field17" , typeof ( string ) );
            if ( ++counter >= max )
                return dt;
            dt . Columns . Add ( "field18" , typeof ( string ) );
            if ( ++counter >= max )
                return dt;
            dt . Columns . Add ( "field19" , typeof ( string ) );
            if ( ++counter >= max )
                return dt;
            dt . Columns . Add ( "field20" , typeof ( string ) );
            if ( ++counter >= max )
                return dt;
            dt . Columns . Add ( "field21" , typeof ( string ) );
            if ( ++counter >= max )
                return dt;
            dt . Columns . Add ( "field22" , typeof ( string ) );
            if ( ++counter >= max )
                return dt;
            dt . Columns . Add ( "field23" , typeof ( string ) );
            if ( ++counter >= max )
                return dt;
            dt . Columns . Add ( "field24" , typeof ( string ) );
            return dt;
        }

        #region Edit/Add/Update
        private void dgProducts_AddingNewItem ( object sender , AddingNewItemEventArgs e ) {
            isInsertMode = true;
        }
        private void dgProducts_BeginningEdit ( object sender , DataGridBeginningEditEventArgs e ) {
            isBeingEdited = true;
        }

        private void dgProducts_RowEditEnding ( object sender , DataGridRowEditEndingEventArgs e ) {

        }

        private void dgProducts_PreviewKeyDown ( object sender , KeyEventArgs e ) {

        }

        private void dgProducts_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e ) {
            //Debugger . Break ( );
        }
        #endregion Edit/Add/Update

        static public void GetDbTablesList ( string DbName , out List<string> TablesList ) {
            int listindex = 0, count = 0;
            TablesList = new List<string>();
            string SqlCommand = "";
            List<string> list = new List<string> ( );
            DbName = DbName . ToUpper ( );
            if ( Utils . CheckResetDbConnection ( DbName , out string constr ) == false ) {
                Debug . WriteLine ( $"Failed to set connection string for {DbName} Db" );
                return;
            }
            // All Db's have their own version of this SP.....
            SqlCommand = "spGetTablesList";

            Datagrids . CallStoredProcedure ( list , SqlCommand );
            //This call returns us a DataTable
            DataTable dt = DataLoadControl . GetDataTable ( SqlCommand );
            // This how to access Row data from  a grid the easiest way.... parsed into a List <xxxxx>
            if ( dt != null ) {
                TablesList = Utils . GetDataDridRowsAsListOfStrings ( dt );

             }
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
    }
}