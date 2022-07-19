namespace NewWpfDev . Models {
    
    public  sealed class BankAcctVm {
        /// <summary>
        ///  Implementation of a (Working) Singleton class
        /// </summary>
        BankAcctVm ( ) {
        }
        private static BankAcctVm instances = null;
        private static readonly object padlock = new object ( );
        public static BankAcctVm Instances {
            get {
                lock ( padlock ) {
                    if ( instances == null )
                        instances = new BankAcctVm ( );
                    return instances;
                }
            }
        }

        #region Event declarations

        public delegate void ConfirmMatchFound ( object sender , SelchangedArgs args );
        public static event ConfirmMatchFound Matchfound;

        public delegate void BankSelectionChanged ( object sender , SelchangedArgs args );
        public static event BankSelectionChanged BankSelChanged;

        public delegate void FindMatch ( object sender , SelchangedArgs args );
        public static event FindMatch Findmatch;

        public delegate void UpdateRecord ( object sender , SelchangedArgs args );
        public static event UpdateRecord DoUpdate;

        public delegate void ClosePanel( object sender , SelchangedArgs args );
        public static event ClosePanel DoClosePanel;

        #endregion Event declarations

        #region Event Triggers
        public void TriggerClosePanel( SelchangedArgs args ) {
            if ( DoClosePanel != null )
                DoClosePanel ( this , args );
        }
        public void TriggerMatchfound ( SelchangedArgs args ) {
            if ( Matchfound != null )
                Matchfound ( this , args );
        }
        // Trigger event to BankAccountInfo
        public void TriggerSelChange ( SelchangedArgs args ) {
            if (  BankSelChanged != null ) {
                 BankSelChanged ( this , args );
            }
        }
        public bool TriggerUpdate ( object sender , SelchangedArgs args ) {
            if ( DoUpdate != null ) {
                DoUpdate ( sender , args );
                return true;
            }
            return false;
        }
        public void TriggerFindMatch ( object sender , SelchangedArgs args ) {
            if ( Findmatch != null ) {
                SelchangedArgs sa = new SelchangedArgs ( );
                sa = args;
                sa . CustNo = args . CustNo;
                Findmatch ( sender , sa );
            }
        }
        #endregion Event Triggers
    }
}   // namespace