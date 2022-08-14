using System . Collections . ObjectModel;
using System . Diagnostics;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;
using System . Windows . Media;

using NewWpfDev . Views;

namespace NewWpfDev . UserControls
{

    public partial class BlankScreenUC : UserControl
    {
        public static BankAcHost Host { get; set; }
        public static ObservableCollection<string> Comboitems = new ObservableCollection<string> ( );
         public string col1 = "wwwwwwwwwwwwww", col2 = "oooooooooooo", col3 = "zzzzzzzzzzzzzzzz";
        public DataGrid gengrid { get; set; }

        #region DP's

        public Brush background
        {
            get { return ( Brush ) GetValue ( backgroundProperty ); }
            set { SetValue ( backgroundProperty , value ); }
        }
        public static readonly DependencyProperty backgroundProperty =
            DependencyProperty . Register ( "background" , typeof ( Brush ) , typeof ( BlankScreenUC ) , new PropertyMetadata ( ( Brush ) Brushes . Cyan ) );

        #endregion DP's

        public BlankScreenUC ( )
        {
            InitializeComponent ( );
            DataContext = this;
            this . Background = Brushes . Cyan;
        }

        public static void SetHost ( BankAcHost host )
        {
            Host = host;
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
        public void ResizeControl ( double height , double width )
        {
            this . Height = height;
            this . Width = width;
         }

        private void UpdateBankRecord ( object sender , RoutedEventArgs e )
        {

        }

        private void dgrid_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
        {
            GenericGridControl . GenericGrid1 . RowHeight = 25;
            GenericGridControl . GenericGrid1 . SelectedIndex = 2;
            GenericGridControl . GenericGrid1 . SelectedItem = 2;
            Debug . WriteLine ( $"{GenericGridControl . GenericGrid1 . RowHeight}, {GenericGridControl . GenericGrid1 . Items . Count}, {GenericGridControl . GenericGrid1 . FontSize}, {GenericGridControl . GenericGrid1 . Foreground}, {GenericGridControl . GenericGrid1 . Background}" );
        }
    }
}
