using System;
using System . Collections . Generic;
using System . ComponentModel;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows . Input;

using NewWpfDev . UserControls;
using NewWpfDev . Views;

namespace NewWpfDev . ViewModels
{
    public class xBrowserUCViewModel : INotifyPropertyChanged
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

        //        public static List<string> TestData = new List<string> ( );
        //public ICommand CloseBrowserUC { get; set; }
        //public ICommand NavigateFwd { get; set; }
        //public ICommand NavigateBack { get; set; }
        //public ICommand ClearCombo { get; set; }
        //public static string Uri { get; set; }
        public static xBrowserUCViewModel browserVm;

        public static MvvmBrowserUC uc3 { get; set; }
        public string Uri { get; set; }
        public xBrowserUCViewModel ( )
        {
            //NavigateBack = new RelayCommand ( ExecuteNavigateBack , CanExecuteNavigateBack );
            //NavigateFwd = new RelayCommand ( ExecuteNavigateFwd , CanExecuteNavigateFwd );
            //CloseBrowserUC = new RelayCommand ( ExecuteCloseBrowserUC , CanExecuteCloseBrowserUC );
            //ClearCombo = new RelayCommand ( ExecuteClearCombo , CanExecuteClearCombo );
            browserVm = this;
        }

        public static xBrowserUCViewModel GetBrowserVm ( MvvmBrowserUC uc )
        {
            // give a pointer to  this class and set pointer for MvvmBrowserUC  - all in one go !
            uc3 = uc;
            return browserVm;
        }
        public static void LoadUcontrol3 ( MvvmBrowserUC win )
        {
            uc3 = win;
        }
    }
}

