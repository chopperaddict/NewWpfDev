using System;
using System . Collections . Generic;
using System . ComponentModel;
using System . Diagnostics;
using System . Linq;
using System . Security . Cryptography;
using System . Text;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Input;

using NewWpfDev . UserControls;
using NewWpfDev . ViewModels;
using NewWpfDev . Views;

namespace NewWpfDev . Models {

    public class BankAccountVM {

        #region NotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged ( string propertyName ) {
            if ( PropertyChanged != null ) {
                PropertyChanged ( this , new PropertyChangedEventArgs ( propertyName ) );
            }
        }
        #endregion NotifyPropertyChanged

        public BankAccountVM ( ) {
            if ( Host == null )
                Host = BankAcHost . GetHost ( );
        }
        public static BankAccountGrid BANKACCOUNTLIST = new BankAccountGrid ( );
        // How to create working commands
        // All working 8/6/22
        // This is the ViewModel for BankAcHost

        public static BankAcHost Host { get; set; }
 
        #region Exit ICommand

        private ICommand exitFullSystem;
        public ICommand ExitFullSystem {
            get {
                if ( exitFullSystem == null )
                    exitFullSystem = new RelayCommand ( Executeclose , CanExecuteclose );
                return exitFullSystem;
            }
        }
        public bool CanExecuteclose ( object arg ) {
            if ( arg == null || ( bool ) arg == true ) return true;
            else return false;
        }
        public void Executeclose ( object obj ) {
            if ( obj == null || ( bool ) obj == true ) Application . Current . Shutdown ( );
        }
        #endregion Exit ICommand

        #region LoadSetup ICommand

        private ICommand loadSetup;
        public ICommand LoadSetup {
            get {
                if ( loadSetup == null ) loadSetup = new RelayCommand ( ExecuteLoad , CanExecuteLoad ); return loadSetup;
            }
        }
        public bool CanExecuteLoad ( object arg ) {
            if ( arg == null || ( bool ) arg == true ) return true;
            else return false;
        }
        public void ExecuteLoad ( object obj ) {
            if ( obj == null || ( bool ) obj == true ) LoadConfig ( );
        }
        #endregion LoadSetup ICommand

        #region SaveSetup ICommand

        private ICommand saveSetup;
        public ICommand SaveSetup {
            get {
                if ( saveSetup == null ) saveSetup = new RelayCommand ( ExecuteSave , CanExecuteSave ); return saveSetup;
            }
        }
        public bool CanExecuteSave ( object arg ) {
            if ( arg == null || ( bool ) arg == true ) return true;
            else return false;
        }
        public void ExecuteSave ( object obj ) {
            if ( obj == null || ( bool ) obj == true ) SaveConfig ( );
        }
        #endregion SaveSetup ICommand

        #region SelectDetails ICommand

        private ICommand selectDetails;
        public ICommand SelectDetails {
            get {
                if ( selectDetails == null )
                    selectDetails = new RelayCommand ( ExecuteSelectAcctDetails , CanExecuteSelectAcctDetails );
                return selectDetails;
            }
        }
        private void ExecuteSelectAcctDetails ( object obj ) {
            Debug . WriteLine ( $"" );
            SelectAcctDetails ( obj );
        }
        private bool CanExecuteSelectAcctDetails ( object arg ) { return true; }
        private void SelectAcctDetails ( object obj ) {
            // How to parse out arguments (usually created by a Converter (single or MultiBinding) of some form)
            if ( obj is object [ ] parameters ) {
                // This receives 3 arguments created by AddTwoValuesConverter
                // these are Int32, string, BankAccountVm class
                if ( parameters == null ) return;
                Debug . WriteLine ( $"{parameters [ 0 ]?. GetType ( ) . ToString ( )}" );
                Debug . WriteLine ( $"{parameters [ 1 ]?.GetType ( ) . ToString ( )}" );
                Debug . WriteLine ( $"{parameters [ 2 ]?.GetType ( ) . ToString ( )}" );
            }
            Host . SetActivePanel ( "BANKACCOUNTLIST" );
        }

        #endregion SelectDetails ICommand

        #region SelectGrid ICommand
        private ICommand selectGrid;
        public ICommand SelectGrid {
            get {
                if ( selectGrid == null ) {
                    selectGrid = new RelayCommand (
                        ( parameter ) => ExecuteselectGrid ( parameter ) ,
                        ( parameter ) => CanExecuteselectGrid ( parameter )
                   );
                }
                Debug . WriteLine ( $"SelectGrid hit" );
                return selectGrid;
            }
        }
        private void ExecuteselectGrid ( object obj ) {
            SelectAcctGrid ( obj );
        }
        private bool CanExecuteselectGrid ( object arg ) { return true; }

        private void SelectAcctGrid ( object obj ) {
            if ( obj is object [ ] parameters ) {
                if ( parameters == null ) return;
                Debug . WriteLine ( $"{parameters [ 0 ]? . GetType ( ) . ToString ( )}" );
                Debug . WriteLine ( $"{parameters [ 1 ] ?. GetType ( ) . ToString ( )}" );
            }
            Host . SetActivePanel ( "BANKACCOUNTGRID" );
        }
        #endregion SelectGrid

        #region Closepanel ICommand

        private ICommand closePanel;
        public ICommand ClosePanel {
            get {
                if ( closePanel == null )
                    closePanel = new RelayCommand ( ExecuteClosePanel , CanExecuteClosePanel );
                return closePanel;
            }
        }
        private bool CanExecuteClosePanel ( object arg ) { return true; }
        private void ExecuteClosePanel ( object obj ) {
            if ( obj == null || ( bool ) obj == true ) {
                if ( Host == null )
                    Host = new BankAcHost ( );
                Host . ClosePanel ( null , "" );
            }
        }
        #endregion Closepanel

        #region ShowBlank
        private ICommand showBlank;
        public ICommand ShowBlank {
            get {
                if ( showBlank == null )
                    showBlank = new RelayCommand ( ExecuteShowBlank , CanExecuteShowBlank );
                return showBlank;
            }
        }      
        private void ExecuteShowBlank ( object obj ) {
            if ( obj  == null) {
                if ( Host == null )
                    Host = new BankAcHost ( );
            }
            Host . SetActivePanel ( "BLANKSCREEN" );
        }


        private ICommand selectGenericGrid;
        public ICommand SelectGenericGrid {
            get {
                if ( selectGenericGrid == null )
                    selectGenericGrid = new RelayCommand ( ExecuteGenericGrid , CanExecuteGenericGrid );
                return selectGenericGrid;
            }
        }

        private bool CanExecuteGenericGrid ( object arg ) { return true; }

        private void ExecuteGenericGrid ( object obj ) {
            if ( obj == null ) {
                if ( selectGenericGrid == null )
                    Host = new BankAcHost ( );
            }
            Host . SetActivePanel ( "GENERICGRID" );
        }

        private bool CanExecuteShowBlank ( object arg ) { return true; }
        #endregion ShowBlank




        public BankAccountVM GetHost ( ) {
            return this;
        }
        private void LoadConfig ( ) {

        }
        private void SaveConfig ( ) {

        }
        public static void SetHost ( BankAcHost host ) {
            Host = host;
        }

    }   // Class
}   // namespace