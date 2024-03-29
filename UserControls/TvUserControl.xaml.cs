﻿using System;
using System . Collections . Generic;
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

using DocumentFormat . OpenXml . Presentation;

using NewWpfDev . ViewModels;
using NewWpfDev . Views;

using NewWpfDev . Views;
using NewWpfDev . Views;

namespace NewWpfDev . UserControls {
    /// <summary>
    /// Interaction logic for TvUserControl.xaml
    /// </summary>
    public partial class TvUserControl : UserControl {
        #region NotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged ( string propertyName ) {
            if ( PropertyChanged != null ) {
                PropertyChanged ( this , new PropertyChangedEventArgs ( propertyName ) );
            }
        }
        #endregion NotifyPropertyChanged

        private static TabWinViewModel Controller {
            get; set;
        }
        public string CurrentType { set; get; } = "CUSTOMER";
        private static TvUserControl ThisWin {
            get; set;
        }
        public static Tabview tabviewWin {
            get; set;
        }
        private double fontsize;
        public double Fontsize {
            get { return fontsize; }
            set { fontsize = value; NotifyPropertyChanged ( nameof ( Fontsize ) ); }
        }

        new private bool IsLoaded { get; set; } = false;

        public TvUserControl ( ) {
            InitializeComponent ( );
            Debug . WriteLine ( $"TreeView Control Loading ......" );
            ThisWin = this;
            Fontsize = 16;
            treeview1 . FontSize = Fontsize;

            treeview1 . FontSize = Fontsize;
            EventControl . WindowMessage += InterWinComms_WindowMessage;
            EventControl . TriggerWindowMessage ( this , new InterWindowArgs { message = $"TvUserControl loaded..." , listbox = null } );
        }

        private void TreeviewViiewer_Loaded ( object sender , RoutedEventArgs e ) {
            if ( IsLoaded == true ) return;
            TabWinViewModel . LoadDb += DgLoadDb;
            EventControl . BankDataLoaded += EventControl_BankDataLoaded;
            EventControl . CustDataLoaded += EventControl_CustDataLoaded;
            if ( treeview1 == null )
                Debug . WriteLine ( "" );
            IsLoaded = true;
            tabviewWin . TabSizeChanged ( null , null );
            treeview1 . UpdateLayout ( );
        }
        public static TvUserControl SetController ( object ctrl ) {
            Controller = ctrl as TabWinViewModel;
            tabviewWin = TabWinViewModel . SendTabview ( );
            return ThisWin;
        }

        private void SelectionHasChanged ( object sender , SelectionChangedArgs e ) {
            //          throw new NotImplementedException ( );
        }

        private void EventControl_CustDataLoaded ( object sender , LoadedEventArgs e ) {
            //           throw new NotImplementedException ( );
        }

        private void EventControl_BankDataLoaded ( object sender , LoadedEventArgs e ) {
            //            throw new NotImplementedException ( );
        }

        private void DgLoadDb ( object sender , LoadDbArgs e ) {
            //          throw new NotImplementedException ( );
        }

        private void treeview1_PreviewMouseMove ( object sender , MouseEventArgs e ) {
            TreeView tvSender = sender as TreeView;
            if ( tvSender != null ) {
                if ( tvSender . Name == "treeview1" ) {
                    TabWinViewModel . CurrentTabIndex = 4;
                    TabWinViewModel . CurrentTabName = "TreeviewTab";
                    TabWinViewModel . CurrentTabTextBlock = "Tab5Header";
                }
            }

        }
        private void PART_MouseLeave ( object sender , MouseEventArgs e ) {
            var tabview = TabWinViewModel . Tview;
            if ( TabWinViewModel . CurrentTabTextBlock == "Tab5Header" ) {
                Tabview . TriggerStoryBoardOff ( 5 );
                tabview . Tab5Header . FontSize = 14;
                tabview . Tab5Header . Foreground = FindResource ( "Cyan0" ) as SolidColorBrush;
            }
        }
        private void PART_MouseEnter ( object sender , MouseEventArgs e ) {
            var tabview = TabWinViewModel . Tview;
            if ( TabWinViewModel . CurrentTabTextBlock == "Tab5Header" ) {
                Tabview . TriggerStoryBoardOn ( 5 );
                tabview . Tab5Header . Foreground = FindResource ( "Yellow0" ) as SolidColorBrush;
                tabview . Tab5Header . FontSize = 24;
            }
        }
        private void InterWinComms_WindowMessage ( object sender , InterWindowArgs e ) {
            string msg = e . message;
            this . treeview1 . Items . Add ( msg );
        }
        private void Magnifyplus2 ( object sender , RoutedEventArgs e ) {
            Fontsize += 2;
            treeview1 . FontSize = Fontsize;
            treeview1 . UpdateLayout ( );
        }
        private void Magnifyminus2 ( object sender , RoutedEventArgs e ) {
            Fontsize -= 2;
            treeview1 . FontSize = Fontsize;
            treeview1 . UpdateLayout ( );
        }

        private void TreeView_MouseLeftButtonUp ( object sender , MouseButtonEventArgs e ) {
            // Intercepts left mouse button click event in my TreeView
            // Can do whatever we like from here on......
            TreeViewItem tvi = ( TreeViewItem ) sender;
            e . Handled = true;
        }
    }
}
