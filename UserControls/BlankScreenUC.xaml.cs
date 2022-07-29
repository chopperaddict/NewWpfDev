using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . Diagnostics;
using System . DirectoryServices . ActiveDirectory;
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

using DocumentFormat . OpenXml . Presentation;

using Microsoft . VisualBasic;

using NewWpfDev . Dapper;
using NewWpfDev . Models;
using NewWpfDev . Sql;
using NewWpfDev . SQL;
using NewWpfDev . ViewModels;
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
            GenericGridControl . GenericGrid . RowHeight = 25;
            GenericGridControl . GenericGrid . SelectedIndex = 2;
            GenericGridControl . GenericGrid . SelectedItem = 2;
            Debug . WriteLine ( $"{GenericGridControl . GenericGrid . RowHeight}, {GenericGridControl . GenericGrid . Items . Count}, {GenericGridControl . GenericGrid . FontSize}, {GenericGridControl . GenericGrid . Foreground}, {GenericGridControl . GenericGrid . Background}" );
        }
    }
}
