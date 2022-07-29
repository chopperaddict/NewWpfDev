//#define SHOWFLAGS

//using System;

//using NewWpfDev. ViewModels;

using System;
using System . Collections . Generic;
using System . Data . SqlClient;
using System . Diagnostics;
using System . Threading;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Controls . Primitives;
using System . Windows . Media;



namespace NewWpfDev. Views
{
    public static class Flags
    {
        // NewWpfDev version 
        public static Dictionary<string , string> ConnectionStringsDict = new Dictionary<string , string>();

        public static bool USESDAPPERSTDPROCEDURES = false;
        public static bool USEADOWITHSTOREDPROCEDURES = true;
        public static bool USEDAPPERWITHSTOREDPROCEDURE = false;
        public static bool GETMULTIACCOUNTS = false;
        public static bool USECOPYDATA = false;
        public static string COPYBANKDATANAME = "NewBank";
        public static string COPYCUSTDATANAME = "NewCust";
        public static string COPYDETDATANAME = "NewDet";
        // Controls whether we use the comon Collection View or not in data viewers of all types
        public static bool UseSharedView
        {
            get; set;
        }

        public static bool SqlGridSwitchingActive = false;
        public static bool SqlBankActive = false;
        public static bool SqlCustActive = false;
        public static bool SqlDetActive = false;
        public static DataGrid SqlBankGrid;// = null;
        public static SqlDbViewer SqlBankViewer;// = null;
        public static SqlDbViewer SqlCustViewer;// = null;
        public static SqlDbViewer SqlDetViewer;// = null;
        public static MultiViewer SqlMultiViewer;// = null;
        public static DataGrid CurrentEditDbViewerBankGrid;// = null;
        public static Window NwSelectionWindow;

        //public static Datagrids dataGrids;
        //// Pointers to our data collections
        //public static DetCollection DetCollection = null;
        //public static AllCustomers CustCollection = null;
        public static object DbData = null;
        public static string DbSaveJsonPath = "";
        public static string CurrentConnectionString
        {
            get; set;
        }

        public static bool UseMagnify
        {
            get; set;
        }

        //-------------------------------------------------------------------
        // FlowDoc flags
        public static bool UseFlowdoc
        {
            get; set;
        }
        public static bool UseScrollView
        {
            get; set;
        }
        public static bool UseFlowScrollbar
        {
            get; set;
        }
        public static double FlowdocCrMultplier
        {
            get; set;
        }
        public static bool PinToBorder
        {
            get; set;
        }
        public static bool IsFlowDocActive
        {
            get; set;
        }
        //-------------------------------------------------------------------
        // Datagrid Options (Generic  types)
        public static bool ReplaceFldNames = true;
        //-------------------------------------------------------------------

        public static DataGrid SqlCustGrid;// = null;
        public static DataGrid CurrentEditDbViewerCustomerGrid;//= null;

        public static DataGrid SqlDetGrid = null;
        public static DataGrid CurrentEditDbViewerDetailsGrid = null;

        public static List<DataGrid> CurrentEditDbViewerBankGridList;
        public static List<DataGrid> CurrentEditDbViewerCustomerGridList;
        public static List<DataGrid> CurrentEditDbViewerDetailsGridList;

        // Current active Grid pointer and Name - Used as a pointer to the currently active DataGrid
        public static DataGrid ActiveSqlGrid = null;

        // current SelectedIndex for each grid type in SqlDbViewers
        //Updated whenever the selection changes in any of the grids
        public static int SqlBankCurrentIndex = 0;
        public static int SqlCustCurrentIndex = 0;
        public static int SqlDetCurrentIndex = 0;

        //EditDb Grid info
        public static EditDb ActiveEditGrid = null;

        // Flag ot control Multi account data loading
        public static bool isMultiMode = false;
        public static string MultiAccountCommandString = "";

        public static bool isEditDbCaller = false;
        public static bool SqlDataChanged = false;
        public static bool EditDbDataChanged = false;
        // system wide flags to avoid selection change processing when we are loading/Reloading FULL DATA in SqlDbViewer
        public static bool DataLoadIngInProgress = false;
        public static bool UpdateInProgress = false;

        public static EditDb BankEditDb;//= null;
        public static EditDb CustEditDb;// = null;
        public static EditDb DetEditDb;// = null;

        //Flags to hold pointers to current DbSelector & SqlViewer Windows
        // Needed to avoi dInstance issues when calling methods from inside Static methods
        // that are needed to handle the interprocess messaging system I have designed for this thing
        public static DbSelector DbSelectorOpen;// = null;
        public static EditDb CurrentEditDbViewer;// = new EditDb ( );
        public static SqlDbViewer CurrentSqlViewer;// = new SqlDbViewer();
        public static DragDropClient DragDropViewer = null;

        // pointers  to any open Viewers
        public static SqlDbViewer CurrentBankViewer;
        public static SqlDbViewer CurrentCustomerViewer;
        public static SqlDbViewer CurrentDetailsViewer;

        public static MultiViewer MultiViewer;

        public static BankDbView BankDbEditor;
        public static CustDbView CustDbEditor;
        public static DetailsDbView DetDbEditor;

        //public static RunSearchPaths ExecuteViewer
        //{
        //    get; set;
        //}
        public static string SingleSearchPath
        {
            get; set;
        }

        public static bool EditDbChangeHandled = false;

        public static bool IsFiltered = false;
        public static string FilterCommand = "";

        //Control CW output of event handlers
        public static bool EventHandlerDebug = false;
        public static bool IsMultiMode = false;

        public static bool LinkviewerRecords = false;

        /// <summary>
        ///  Holds the DataGrid pointer fort each open SqlDbViewer Window as they
        ///  can each have diffrent datasets in use at any one time
        /// </summary>
        public static SqlDbViewer ActiveSqlViewer;//= null;

        /// <summary>
        ///  Used to  control the initial load of Viewer windows to avoid 
        ///  mutliple additions to DbSelector's viewer  listbox
        /// </summary>
        public static bool SqlViewerIsLoading = false;
        public static bool SqlViewerIndexIsChanging = false;
        public static bool EditDbIndexIsChanging = false;
        public static bool EditDbDataChange = false;
        public static bool ViewerDataChange = false;
        public static bool UseBeeps = true;


        public static double TopVisibleBankGridRow = 0;
        public static double BottomVisibleBankGridRow = 0;
        public static double TopVisibleCustGridRow = 0;
        public static double BottomVisibleCustGridRow = 0;
        public static double TopVisibleDetGridRow = 0;
        public static double BottomVisibleDetGridRow = 0;
        public static double ViewPortHeight = 0;

        // Set default sort to Custno + Bankno
        public static int SortOrderRequested = 0;
        public enum SortOrderEnum
        {
            DEFAULT = 0,
            ID,
            BANKNO,
            CUSTNO,
            ACTYPE,
            DOB,
            ODATE,
            CDATE
        }
        public static bool AddDictionaryEntry<TKey, TValue> (this IDictionary<TKey , TValue> dictionary , TKey key , TValue value)
        {

            if ( dictionary == null )
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            if ( !dictionary . ContainsKey(key) )
            {
                dictionary . Add(key , value);
                return true;
            }

            return false;
        }

        public static T GetChildOfType<T> (this DependencyObject depObj) where T : DependencyObject
        {
            if ( depObj == null )
                return null;

            for ( int i = 0 ; i < VisualTreeHelper . GetChildrenCount(depObj) ; i++ )
            {
                var child = VisualTreeHelper . GetChild(depObj , i);

                var result = ( child as T ) ?? GetChildOfType<T>(child);
                if ( result != null )
                    return result;
            }
            return null;
        }


        //public static void ShowAllFlags ( )
        //{
        //	//Debug . WriteLine (
        //	//$"\nbool EditDbDataChanged						: { Flags . EditDbDataChanged}" +
        //	//$"\nbool EditDbChangeHandled					: { Flags . EditDbChangeHandled}" +
        //	//$"\nbool EventHandlerDebug						: { Flags . EventHandlerDebug}" +
        //	//$"\nbool isEditDbCaller							: { Flags . isEditDbCaller}" +
        //	//$"\nbool isMultiMode							: { Flags . isMultiMode}" +
        //	//$"\nbool IsFiltered								: { Flags . IsFiltered}" +
        //	//$"\nbool IsMultiMode							: { Flags . IsMultiMode}" +
        //	//$"\nbool SqlViewerIsLoading						: { Flags . SqlViewerIsLoading}" +
        //	//$"\nbool  SqlViewerIndexIsChanging				: { Flags . SqlViewerIndexIsChanging}" +
        //	//"\n" +
        //	//$"\nDataGrid ActiveSqlGrid						: { Flags . ActiveSqlGrid?.Name}" +
        //	//$"\nDataGrid CurrentEditDbViewerBankGrid		: { Flags . CurrentEditDbViewerBankGrid?.Name}" +
        //	//$"\nDataGrid CurrentEditDbViewerCustomerGrid: { Flags . CurrentEditDbViewerCustomerGrid?.Name}" +
        //	//$"\nDataGrid CurrentEditDbViewerDetailsGrid		: { Flags . CurrentEditDbViewerDetailsGrid?.Name}" +
        //	//$"\nDataGrid SqlBankGrid						: { Flags . SqlBankGrid?.Name}" +
        //	//$"\nDataGrid SqlCustGrid						: { Flags . SqlCustGrid?.Name}" +
        //	//$"\nDataGrid SqlDetGrid							: { Flags . SqlDetGrid?.Name}" +
        //	//"\n" +
        //	//$"\nDbSelector DbSelectorOpen					: { Flags . DbSelectorOpen}" +
        //	//"\n" +
        //	//$"\nEditDb ActiveEditGrid						: { Flags . ActiveEditGrid?.Name}" +
        //	//$"\nEditDb BankEditDb							: { Flags . BankEditDb?.Name}" +
        //	//$"\nEditDb CustEditDb							: { Flags . CustEditDb?.Name}" +
        //	//$"\nEditDb DetEditDb							: { Flags . DetEditDb?.Name}" +
        //	//$"\nEditDb CurrentEditDbViewer					: { Flags . CurrentEditDbViewer?.Name}" +
        //	//"\n" +
        //	//$"\nint SqlBankCurrentIndex						: { Flags . SqlBankCurrentIndex}" +
        //	//$"\nint SqlCustCurrentIndex						: { Flags . SqlCustCurrentIndex}" +
        //	//$"\nint SqlDetCurrentIndex						: { Flags . SqlDetCurrentIndex}" +
        //	//"\n" +
        //	//$"\nSqlDbViewer ActiveSqlViewer					: { Flags . ActiveSqlViewer?.CurrentDb}" +
        //	//$"\nSqlDbViewer CurrentSqlViewer				: { Flags . CurrentSqlViewer?.CurrentDb}" +
        //	//$"\nSqlDbViewer SqlBankViewer					: { Flags . SqlBankViewer} + {Flags . SqlBankViewer?.BankGrid?.Name}" +
        //	//$"\nSqlDbViewer SqlCustViewer					: { Flags . SqlCustViewer} + {Flags . SqlCustViewer?.CustomerGrid?.Name}" +
        //	//$"\nSqlDbViewer SqlDetViewer					: { Flags . SqlDetViewer} + {Flags . SqlDetViewer?.DetailsGrid?.Name}" +
        //	////			$"\nSqlDbViewer SqlUpdateOriginatorViewer					: { Flags .SqlUpdateOriginatorViewer?.Name}" +
        //	//"\n" +
        //	//$"\nstring FilterCommand						: { Flags . FilterCommand}" +
        //	//$"\nstring MultiAccountCommandString			: { Flags . MultiAccountCommandString}" +
        //	//$"\nCurrentThread								: {Thread . CurrentThread . ManagedThreadId}\n" +
        //	//$"\nSQL Database pointers\n" +
        //	//$"Bank : Bankinternalcollection				: { BankCollection . Bankinternalcollection . Count}\n" +
        //	////$"Bank : SqlViewerBankcollection				: { BankCollection . SqlViewerBankcollection . Count}\n" +
        //	////$"Bank : EditDbViewercollection 				: { BankCollection . EditDbBankcollection . Count}\n" +
        //	////$"Bank : MultiBankcollection					: { BankCollection . MultiBankcollection . Count}\n" +
        //	////$"Bank : BankViewerDbcollection				: { BankCollection . BankViewerDbcollection . Count}\n" +

        //	////			$"\nCustcollection								: { CustCollection . Custcollection . Count}\n" +
        //	////			$"CustViewerDbcollection						: { CustCollection . CustViewDbcollection . Count}\n" +
        //	////			$"SqlViewerCustcollection	  					: { CustCollection . SqlViewerCustcollection . Count}\n" +
        //	////			$"EditDbCustcollection 						: { CustCollection . EditDbCustcollection . Count}\n" +
        //	////			$"MultiCustcollection							: { CustCollection . MultiCustcollection . Count}\n" +

        //	//$"\nDetcollection								: { DetCollection . Detcollection . Count}\n" +
        //	//$"DetViewerDbcollection						: { DetCollection . DetViewerDbcollection . Count}\n" +
        //	//$"SqlViewerDetcollection	  					: { DetCollection . SqlViewerDetcollection . Count}\n" +
        //	//$"EditDbDetcollection 						: { DetCollection . EditDbDetcollection . Count}\n" +
        //	//$"MultiDetcollection							: { DetCollection . MultiDetcollection . Count}\n\n"
        //	//);
        //}
        ////public static void PrintDbInfo ( )
        //{
        //	BankAccountViewModel bvm = SqlBankGrid?.SelectedItem as BankAccountViewModel;
        //	CustomerViewModel cvm = SqlBankGrid?.SelectedItem as CustomerViewModel;
        //	DetailsViewModel dvm = SqlBankGrid?.SelectedItem as DetailsViewModel;
        //	Debug. WriteLine ( $"\nDatabase information" );
        //	bvm = SqlBankGrid?.SelectedItem as BankAccountViewModel;
        //	Debug. WriteLine ( $"SqlBankGrid		: {SqlBankGrid?.Items . Count} : {SqlBankGrid?.SelectedIndex}" );
        //	Debug. WriteLine ( $"				: CustNo = {bvm?.CustNo}, BankNo = {bvm?.BankNo}" );
        //	cvm = SqlCustGrid?.SelectedItem as CustomerViewModel;
        //	Debug. WriteLine ( $"SqlCustGrid		: {SqlCustGrid?.Items . Count} : {SqlCustGrid?.SelectedIndex}" );
        //	Debug. WriteLine ( $"				: CustNo = {cvm?.CustNo}, BankNo = {cvm?.BankNo}" );
        //	dvm = SqlDetGrid?.SelectedItem as DetailsViewModel;
        //	Debug. WriteLine ( $"SqlDetGrid		: {SqlDetGrid?.Items . Count} : {SqlDetGrid?.SelectedIndex}" );
        //	Debug. WriteLine ( $"				: CustNo = {dvm?.CustNo}, BankNo = {dvm?.BankNo}" );
        //	bvm = MultiViewer?.BankGrid?.SelectedItem as BankAccountViewModel;
        //	Debug. WriteLine ( $"Multi.BankGrid	: {MultiViewer?.BankGrid . Items . Count} : {MultiViewer?.BankGrid?.SelectedIndex}" );
        //	Debug. WriteLine ( $"				: CustNo = {bvm?.CustNo}, BankNo = {bvm?.BankNo}" );
        //	cvm = MultiViewer?.CustomerGrid?.SelectedItem as CustomerViewModel;
        //	Debug. WriteLine ( $"Multi. CustGrid	: {MultiViewer?.CustomerGrid . Items . Count} : {MultiViewer?.CustomerGrid?.SelectedIndex}" );
        //	Debug. WriteLine ( $"				: CustNo = {cvm?.CustNo}, BankNo = {cvm?.BankNo}" );
        //	dvm = MultiViewer?.DetailsGrid?.SelectedItem as DetailsViewModel;
        //	Debug. WriteLine ( $"Multi. DetGrid		: {MultiViewer?.DetailsGrid . Items . Count} : {MultiViewer?.DetailsGrid . SelectedIndex}" );
        //	Debug. WriteLine ( $"				: CustNo = {dvm?.CustNo}, BankNo = {dvm?.BankNo}" );
        //}

        //public static void PrintSundryVariables ( string comment = "" )
        //{
        //	//	if ( comment . Length > 0 )
        //	//		Debug . WriteLine ( $"\n COMMENT : {comment}" );
        //	//	else
        //	//		Debug . WriteLine ( "" );
        //	//	if ( Flags . CurrentSqlViewer != null && Flags . SqlBankGrid != null )
        //	//		Debug . WriteLine ( $" Current Viewer : ItemsSource :		{ Flags . SqlBankGrid . Name}" );
        //	//	if ( Flags . CurrentSqlViewer != null && Flags . SqlCustGrid != null )
        //	//		Debug . WriteLine ( $" Current Viewer : ItemsSource :		{ Flags . SqlCustGrid . Name}" );
        //	//	if ( Flags . CurrentSqlViewer != null && Flags . SqlDetGrid != null )
        //	//		Debug . WriteLine ( $" Current Viewer : ItemsSource :		{ Flags . SqlDetGrid . Name}" );
        //	//	Debug . WriteLine ( $" Flags . TopVisibleBankGridRow		= { Flags . TopVisibleBankGridRow }" );
        //	//	Debug . WriteLine ( $" Flags . BottomVisibleBankGridRow	= {Flags . BottomVisibleBankGridRow}" );
        //	//	Debug . WriteLine ( $" Flags . TopVisibleCustGridRow		= { Flags . TopVisibleCustGridRow }" );
        //	//	Debug . WriteLine ( $" Flags . BottomVisibleCustGridRow	= { Flags . BottomVisibleCustGridRow}" );
        //	//	Debug . WriteLine ( $" Flags . TopVisibleDetGridRow		= { Flags . TopVisibleDetGridRow}" );
        //	//	Debug . WriteLine ( $" Flags . BottomVisibleDetGridRow	= { Flags . BottomVisibleDetGridRow}" );
        //	//	Debug . WriteLine ( $" Flags . ViewPortHeight				= { Flags . ViewPortHeight } rows visible" );
        //	//	if ( Flags . ActiveSqlViewer?.CurrentDb == "BANKACCOUNT" )
        //	//		Debug . WriteLine ( $" BANK record's offset (from top)	= { ( Flags . SqlBankCurrentIndex - Flags . TopVisibleDetGridRow ) + 1}" );
        //	//	else if ( Flags . ActiveSqlViewer?.CurrentDb == "CUSTOMER" )
        //	//		Debug . WriteLine ( $" CUST record's offset (from top)	= { ( Flags . SqlCustCurrentIndex - Flags . TopVisibleDetGridRow ) + 1}" );
        //	//	else if ( Flags . ActiveSqlViewer?.CurrentDb == "DETAILS" )
        //	//		Debug . WriteLine ( $" DETAILS record offset (from top)	= { ( Flags . SqlDetCurrentIndex - Flags . TopVisibleDetGridRow ) + 1}" );
        //	//	Debug . WriteLine ( $"\n Flags . SqlBankCurrentIndex		= { Flags . SqlBankCurrentIndex}" );
        //	//	Debug . WriteLine ( $" Flags . SqlCustCurrentIndex		= { Flags . SqlCustCurrentIndex}" );
        //	//	Debug . WriteLine ( $" Flags . SqlDetCurrentIndex			= { Flags . SqlDetCurrentIndex}" );

        //	//	string buffer = "\n Multi Grid Info :-";
        //	//	if ( Flags . SqlBankGrid != null )
        //	//		buffer += $"\n Flags.SqlBankGrid					= { Flags . SqlBankGrid?.Items . Count} / {Flags . SqlBankGrid?.SelectedIndex}";
        //	//	if ( Flags . SqlCustGrid != null )
        //	//		buffer += $"\n Flags.SqlCustGrid					= { Flags . SqlCustGrid?.Items . Count} / {Flags . SqlCustGrid?.SelectedIndex}";
        //	//	if ( Flags . SqlDetGrid != null )
        //	//		buffer += $"\n Flags.SqlDetGrid					= { Flags . SqlDetGrid?.Items . Count} / {Flags . SqlDetGrid?.SelectedIndex}";
        //	//	if ( buffer . Length > 18 )
        //	//	{
        //	//		Debug . WriteLine ( buffer );
        //	//		Debug . WriteLine ( "\n" );
        //	//	}

        //	//	buffer = "Sql Viewer Info :-";
        //	//	if ( Flags . CurrentBankViewer?.BankGrid != null )
        //	//		buffer += $" \n Flags.CurrentBankViewer.BankGrid							= { Flags . CurrentBankViewer . BankGrid?.Items . Count} / {Flags . CurrentBankViewer . BankGrid?.SelectedIndex}";
        //	//	//				Debug . WriteLine ( $" Flags.CurrentBankViewer.BankGrid				= { Flags .CurrentBankViewer.BankGrid?.Items . Count} / {Flags . CurrentBankViewer . BankGrid? . SelectedIndex}" );
        //	//	if ( Flags . CurrentCustomerViewer?.BankGrid != null )
        //	//		buffer += $"\n Flags.CurrentCustomerViewer . CustomerGrid			= { Flags . CurrentCustomerViewer . CustomerGrid?.Items . Count} / {Flags . CurrentCustomerViewer . CustomerGrid?.SelectedIndex}";
        //	//	//					Debug . WriteLine ( $" Flags.CurrentCustomerViewer . CustomerGrid	= { Flags . CurrentCustomerViewer . CustomerGrid? . Items . Count} / {Flags . CurrentCustomerViewer . CustomerGrid? . SelectedIndex}" );
        //	//	if ( Flags . CurrentDetailsViewer?.BankGrid != null )
        //	//		buffer += $"\n Flags.CurrentDetailsViewer . DetailsGrid					= { Flags . CurrentDetailsViewer . DetailsGrid?.Items . Count} / {Flags . CurrentDetailsViewer . DetailsGrid?.SelectedIndex}";
        //	//	//					Debug . WriteLine ( $" Flags.CurrentDetailsViewer . DetailsGrid		= { Flags . CurrentDetailsViewer . DetailsGrid? . Items . Count} / {Flags . CurrentDetailsViewer . DetailsGrid? . SelectedIndex}" );
        //	//	if ( buffer . Length > 18 )
        //	//	{
        //	//		Debug . WriteLine ( buffer );
        //	//		Debug . WriteLine ( "\n" );
        //	//	}
        //	//	if ( MultiViewer != null )
        //	//	{
        //	//		Debug . WriteLine ( $" MultiViewer Scroll settings" );
        //	//		Debug . WriteLine ( $" ScrollData.Banktop		= { ( int ) MultiViewer?.ScrollData . Banktop}" );
        //	//		Debug . WriteLine ( $" ScrollData.Bankbottom	= { ( int ) MultiViewer?.ScrollData . Bankbottom}" );
        //	//		Debug . WriteLine ( $" ScrollData.Bankvisible	= { ( int ) MultiViewer?.ScrollData . BankVisible}" );
        //	//		Debug . WriteLine ( $" ScrollData.Custtop		= { ( int ) MultiViewer?.ScrollData . Custtop}" );
        //	//		Debug . WriteLine ( $" ScrollData.Custbottom	= { ( int ) MultiViewer?.ScrollData . Custbottom}" );
        //	//		Debug . WriteLine ( $" ScrollData.Custvisible	= { ( int ) MultiViewer?.ScrollData . CustVisible}" );
        //	//		Debug . WriteLine ( $" ScrollData.Dettop		= { ( int ) MultiViewer?.ScrollData . Dettop}" );
        //	//		Debug . WriteLine ( $" ScrollData.Detbottom	= { ( int ) MultiViewer?.ScrollData . Detbottom}" );
        //	//		Debug . WriteLine ( $" ScrollData.Detvisible	= { ( int ) MultiViewer?.ScrollData . DetVisible}\n" );
        //	//	}
        //	//	if ( CurrentSqlViewer != null )
        //	//	{
        //	//		Debug . WriteLine ( $" SqlViewer Scroll settings" );
        //	//		Debug . WriteLine ( $" SqlViewer.Banktop		= { ( int ) SqlDbViewer . ScrollData . Banktop}" );
        //	//		Debug . WriteLine ( $" SqlViewer.Bankbottom	= { ( int ) SqlDbViewer . ScrollData . Bankbottom}" );
        //	//		Debug . WriteLine ( $" SqlViewer.Bankvisible	= { ( int ) SqlDbViewer . ScrollData . BankVisible}" );
        //	//		Debug . WriteLine ( $" SqlViewer.Custtop		= { ( int ) SqlDbViewer . ScrollData . Custtop}" );
        //	//		Debug . WriteLine ( $" SqlViewer.Custbottom	= { ( int ) SqlDbViewer . ScrollData . Custbottom}" );
        //	//		Debug . WriteLine ( $" SqlViewer.Custvisible	= { ( int ) SqlDbViewer . ScrollData . CustVisible}" );
        //	//		Debug . WriteLine ( $" SqlViewer.Dettop		= { ( int ) SqlDbViewer . ScrollData . Dettop}" );
        //	//		Debug . WriteLine ( $" SqlViewer.Detbottom	= { ( int ) SqlDbViewer . ScrollData . Detbottom}" );
        //	//		Debug . WriteLine ( $" SqlViewer.Detvisible	= { ( int ) SqlDbViewer . ScrollData . DetVisible}\n" );
        //	//	}
        //}
    }

}
