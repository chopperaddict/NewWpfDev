//#define SHOWFLAGS

//using System;

//using NewWpfDev. ViewModels;

using System;
using System . Collections . Generic;
using System . Diagnostics;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Media;

namespace NewWpfDev . Views
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
                return null; try
            {
                for ( int i = 0 ; i < VisualTreeHelper . GetChildrenCount ( depObj ) ; i++ )
                {
                    var child = VisualTreeHelper . GetChild ( depObj , i );

                    var result = ( child as T ) ?? GetChildOfType<T> ( child );
                    if ( result != null )
                        return result;
                }
            } catch (Exception ex) { }
            return null;
        }

        public static string GetConnectionStringForTable ( string table , string domain="IAN1")
        {
            Debug . WriteLine ($"SQL connection chack for {domain} resulted in [{NewWpfDev. Utils.CheckResetDbConnection ( domain , out string constring)}] ");
            NewWpfDev . Utils . GetDictionaryEntry ( Flags . ConnectionStringsDict , domain , out string connstring );
            if ( connstring != null && connstring != "" )
                Flags . CurrentConnectionString = connstring;
            return Flags . CurrentConnectionString;
        }
    }

}
