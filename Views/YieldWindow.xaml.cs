using System;
using System . Collections . Generic;
using System . Data . SqlClient;
using System . Data;
using System . Linq;
using System . Text;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;
using NewWpfDev . SQL;
using NewWpfDev . ViewModels;
using System . Collections . ObjectModel;
using System . Collections;
using System . Diagnostics;
using System . ComponentModel;
using System . Runtime . InteropServices;
using static NewWpfDev . Delegates;
using System . Windows . Data;

namespace NewWpfDev . Views
{
     public partial class YieldWindow : Window//, INotifyPropertyChanged
    {
        public  YieldWindowViewModel yvm { get; set; }

        private ObservableCollection<BankAccountViewModel> bvm { get; set; }

        private bool usenew = true;

          #region OnPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged ( string propertyName )
        {
            if ( PropertyChanged != null )
            {
                PropertyChanged ( this , new PropertyChangedEventArgs ( propertyName ) );
            }
        }
        #endregion OnPropertyChanged

        public YieldWindow ( )
        {
            InitializeComponent ( );
            yvm = new YieldWindowViewModel ( );
            // Give viewmodel a pointer to us for updating the view's labels etc
            yvm.PassViewPointer( this );
            // Need to set this so our calls to methods in ViewModel get valid data, (such as bvm)
            // without this, bvm becomes null on a call into the viewmodel !
            this.DataContext = yvm;
            this . Show ( );

            // Setup handlers  in ViewModel for various events in our window
            dgrid1 . SelectionChanged += yvm . Dgrid1_SelectionChanged;
            dgrid2 . SelectionChanged += yvm . dgrid2_SelectionChanged;

            duration1 . Content = $"{yvm.sw . ElapsedMilliseconds} ms";
            counter2 . Content = yvm . bvm . Count . ToString ( );
            yvm.InfoPanel = $"Loaded grid1 from Db data in {yvm . sw . ElapsedMilliseconds} ms";
            bvm = yvm . bvm; 
            yvm . Loadyield2 ( );
            yvm.Loadstack1( );
          }
        private void YieldWindow_UpdateBankRecord ( object sender , DbArgs args )
        {
            //if ( args . UseMatch == false )
            //{
            dgrid1 . SelectedIndex = args . index;
            dgrid1 . SelectedItem = args . index;
            dgrid1 . BringIntoView ( );
            dgrid1 . Refresh ( );
            dgrid1 . BringIntoView ( );
            Utils . ScrollRecordIntoView ( dgrid1 , args . index );
        }

        private void Closebtn ( object sender , RoutedEventArgs e )
        {
            this . Close ( );
        }
        private void dgrid1_CellEditEnding ( object sender , DataGridCellEditEndingEventArgs e )
        {
            yvm . EditedRow1 = dgrid1 . SelectedIndex;
            yvm . SelectedAccount1 = dgrid1 . SelectedItem as BankAccountViewModel;
            yvm . Grid1RowEdited = true;
        }
        private void dgrid2_CellEditEnding ( object sender , DataGridCellEditEndingEventArgs e )
        {
            yvm . EditedRow2 = dgrid2 . SelectedIndex;
            yvm . SelectedAccount2 = dgrid2 . SelectedItem as BankAccountViewModel;
            yvm . Grid2RowEdited = true;
        }  
        private void Stack2_PreviewButton ( object sender , MouseButtonEventArgs e )
        {
            yvm . ExecuteLoadstack2 ( 2 );
        }
    }
}


//public class DbArgs
//{
//    public int index { get; set; }
//    public BankAccountViewModel bvm { get; set; }
//    public bool DoUpdate { get; set; }
//    public bool UseMatch { get; set; }
//}