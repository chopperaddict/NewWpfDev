using System;
using System . Collections . Generic;
using System . ComponentModel;
using System . ComponentModel . Design;
using System . Diagnostics;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;

using NewWpfDev . UserControls;
using NewWpfDev . Views;

namespace NewWpfDev . ViewModels
{
    public class MvvmListboxUCViewModel : INotifyPropertyChanged
    {
        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged ( string propertyName )
        {
            if ( Flags . SqlBankActive == false )
                //				this . VerifyPropertyName ( propertyName );

                if ( this . PropertyChanged != null )
                {
                    var e = new PropertyChangedEventArgs ( propertyName );
                    this . PropertyChanged ( this , e );
                }
        }
        #endregion PropertyChanged

        public static List<string> TestData = new List<string> ( );
        static public string SelectedUCtrl { get; set; }
        public ICommand HideListbox { get; set; }

        public static MvvmListboxUC uc2 { get; set; }
        public static MvvmContainerViewModel mcvm { get; set; }
        private string closebtnText = "Hide List";
        public string ClosebtnText
        {
            get { return closebtnText; }
            set { closebtnText = value; OnPropertyChanged ( "ClosebtnText" ); }
        }

        public MvvmListboxUCViewModel ( )
        {
            mcvm = MvvmContainerViewModel . GetMvvmContainerViewModel ( );
            HideListbox = new RelayCommand ( ExecuteHideListbox , CanExecuteHideListbox );
            LoadControl2Listbox ( );
        }

        public static void LoadData ( MvvmListboxUC win , int mode = 0 )
        {
            uc2 = win;
            if ( mode == 0 )
                uc2 . _listbox . ItemsSource = TestData;
            else
                uc2 . _listbox . ItemsSource = MvvmContainerViewModel . UCList;
        }
        private void ExecuteHideListbox ( object obj )
        {
            
            Debug. WriteLine ( $"DataContext = {uc2 . DataContext}" );
            if ( uc2 . _listbox . Visibility == Visibility . Collapsed )
            {
                uc2 . _listbox . Visibility = Visibility . Visible;
                uc2 . HideBtn . Content = "Hide List Box";
            }
            else
            {
                uc2 . _listbox . Visibility = Visibility . Collapsed;
                uc2 . HideBtn . Content = "Show List Box";
            }
        }

        private bool CanExecuteHideListbox ( object arg )
        {
            return true;
        }

        public void LoadControl2Listbox ( )
        {
            TestData . Clear ( );
            TestData . Add ( "line 1" );
            TestData . Add ( "line 2" );
            TestData . Add ( "line 3" );
            TestData . Add ( "line 4" );
            TestData . Add ( "line 5" );
            TestData . Add ( "line 6" );
            TestData . Add ( "line 7" );
            TestData . Add ( "line 8" );
        }

        public static void _listbox_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {
            ListBox lb = sender as ListBox;
            SelectedUCtrl = lb . SelectedItem . ToString ( );
        }
    }

}
