using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
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

        public static ObservableCollection<GenericClass> Gencollection1 { get; set; } = new ObservableCollection<GenericClass> ( );
        public static ObservableCollection<GenericClass> Gencollection2 { get; set; } = new ObservableCollection<GenericClass> ( );

        public static BankAcHost BankHost { get; set; }
        public static GenericGridControl GenericGridHost { get; set; }
        public static BankAccountInfo BankAcctInfoHost { get; set; }
        public static BankAccountGrid BankAcctGridHost { get; set; }
        public static PopupListBox PopupListCtlr { get; set; }
        public static ComboboxPlus ComboPlusCtrl  {get; set; }

        public BankAccountVM ( ) {
            if ( Host == null )
                Host = BankAcHost . GetHost ( );
        }
        public static BankAccountGrid BANKACCOUNTLIST = new BankAccountGrid ( );
  
        public static BankAcHost Host { get; set; }

        #region Full Props
        private string infotxt = "";
        public string InfoText
        {
            get { return infotxt; }
            set { infotxt = value; NotifyPropertyChanged ( nameof ( InfoText ) ); }
        }
        #endregion Full Props


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
                        (object  parameter ) => ExecuteselectGrid ( parameter ) ,
                        ( object  parameter ) => CanExecuteselectGrid ( parameter )
                   );
                }
                Debug . WriteLine ( $"SelectGrid hit" );
                return selectGrid;
            }
        }
        private void ExecuteselectGrid ( object obj ) {
            //Host . SetActivePanel ( obj .ToString() );
            SelectAcctGrid ( obj );
        }
        private bool CanExecuteselectGrid ( object arg ) { return true; }

        private void SelectAcctGrid ( object obj ) {
            // How to parse out arguments (usually created by a Converter (single or MultiBinding) of some form)
            if ( obj is object [ ] parameters )
            {
                // This receives [3] arguments created by AddTwoValuesConverter
                // I define these to be string, calling class, generic object
                if ( parameters == null ) return;
                if ( parameters . Length >= 1 )
                    Debug . WriteLine ( $"{parameters [ 0 ]?.GetType ( ) . ToString ( )}" );
                if ( parameters . Length >= 2 )
                    Debug . WriteLine ( $"{parameters [ 1 ]?.GetType ( ) . ToString ( )}" );
                if(parameters.Length >= 3)
                Debug . WriteLine ( $"{parameters [ 2 ]?.GetType ( ) . ToString ( )}" );
                if ( parameters [0] != null)
                    Host . SetActivePanel ( parameters [0] . ToString ( ) );
            }
            else
            Host . SetActivePanel ( obj.ToString() );
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

        public  static void TriggerGetControlsPointers ( )
        {
            // Only  called by BankAcHost at end of its Constructor
            // Setup Viewmodel pointers to all relevant cobntrols used in  thiis area
            BankAcctInfoHost = BankAccountInfo . GetBankAccountInfoHandle ( );
            BankAcctGridHost = BankAccountGrid . GetBankAccountGridHandle ( );
            GenericGridHost = GenericGridControl . GetGenGridHandle ( );
            PopupListCtlr = PopupListBox . GetPopupLbHandle ( );
            ComboPlusCtrl = ComboboxPlus . GetComboPlusHandle ( );
        }
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