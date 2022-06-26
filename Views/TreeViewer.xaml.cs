using System;
using System . Collections . Generic;
using System . ComponentModel;
using System . Diagnostics;
using System . IO;
using System . Linq;
using System . Threading;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Threading;

namespace NewWpfDev . Views
{
    /// <summary>
    /// Interaction logic for TreeViewer.xaml
    /// </summary>
    public partial class TreeViewer : Window
    {
        TreeViews treeviews = new TreeViews ( );
        #region OnPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public int SLEEPTIME { get; set; } = 100;

        public static object [ ] Args = new object [ ] { new object ( ) , new object ( ) , new object ( ) };
        public static object TreeViewObject;
        
         public static bool ExpandLimited = false;
        protected void OnPropertyChanged ( string PropertyName )
        {
            if ( null != PropertyChanged )
            {
                PropertyChanged ( this ,
                      new PropertyChangedEventArgs ( PropertyName ) );
            }
        }
        #endregion OnPropertyChanged

        public struct ExpandArgs
        {
            public TreeView tv;
            public TreeViewItem tvitem;
            public TreeViewItem SearchSuccessItem;
            public int ExpandLevels;
            public int CurrentLevel;
            public int MaxItems;
            public string SearchTerm;
            public bool SearchActive;
            public int Selection;
            public bool SearchSuccess;
            public bool ListResults;
            public bool IsFullExpand;
            public TreeViewItem Parent;
        };
        public ExpandArgs ExpArgs = new ExpandArgs ( );
        public static Dictionary<string , string> VolumeLabelsDict = new Dictionary<string , string> ( );
        public static List<string> ValidFiles = new List<string> ( );
        public bool ShowVolumeLabels { get; set; }
        public string LoadDrive = "";
        Stack<TreeViewItem> stack0 = new Stack<TreeViewItem> ( );
        List<TreeViewItem> slist1 = new List<TreeViewItem> ( );
        Stack<TreeViewItem> stack2 = new Stack<TreeViewItem> ( );
        Stack<TreeViewItem> stack3 = new Stack<TreeViewItem> ( );
        Stack<TreeViewItem> stack4 = new Stack<TreeViewItem> ( );
        
        #region Dependency Props
        
        public bool CancelExpand
        {
            get { return ( bool ) GetValue ( CancelExpandProperty ); }
            set { SetValue ( CancelExpandProperty , value ); }
        }
        public static readonly DependencyProperty CancelExpandProperty =
            DependencyProperty . Register ( "CancelExpand" , typeof ( bool ) , typeof ( TreeViewer ) , new PropertyMetadata ( false ) );

        #endregion Dependency Props

        #region Full Props

        private bool _IsSelected;
        private bool _IsExpanded;
        private TreeViewItem _CurrentBaseItem;
        private TreeViewItem _CurrentActiveItem;         
          public bool IsSelected
        {
            get { return _IsSelected; }
            set { _IsSelected = value; OnPropertyChanged ( IsSelected . ToString ( ) ); }
        }
        public bool IsExpanded
        {
            get { return _IsExpanded; }
            set { _IsExpanded = value; OnPropertyChanged ( IsExpanded . ToString ( ) ); }
        }
        public TreeViewItem CurrentBaseItem
        {
            get { return _CurrentBaseItem; }
            set { _CurrentBaseItem = value; OnPropertyChanged ( CurrentBaseItem . ToString ( ) ); }
        }
        public TreeViewItem CurrentActiveItem
        {
            get { return _CurrentActiveItem; }
            set { _CurrentActiveItem = value; OnPropertyChanged ( CurrentActiveItem . ToString ( ) ); }
        }

        #endregion Full Props

        //public TreeViewer ( )
        //{
        //    InitializeComponent ( );
        //}
        public TreeViewer ( string drive)
        {
            InitializeComponent ( );
            LoadDrive = drive;
        }
        private void Window_Loaded ( object sender , RoutedEventArgs e )
        {
            treeviews = new TreeViews ( );
            if ( LoadDrive != "" )
                LoadDrives (TestTree, LoadDrive );
            else
            LoadDrives ( TestTree );
        }

        #region methods
        private void TestTree_SelectedItemChanged ( object sender , RoutedPropertyChangedEventArgs<object> e )
        {
        }
        private void TreeViewItem_RequestBringIntoView ( object sender , RequestBringIntoViewEventArgs e )
        {
            BringIntoView ( );
        }
        private void TreeViewItem_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
        {
        }
        #endregion methods

        #region PROCESSING METHODS
        private void TestTree_Collapsed ( object sender , RoutedEventArgs e )
        {
            IsExpanded = false;
        }
        private bool usetask = true;
        private void OnTreeExpanded ( object sender , RoutedEventArgs e )
        {
            var tvi = ( TreeViewItem ) TestTree . SelectedItem;
            if ( tvi == null )
                return;
            // Expand the directoryD/rive first so wehave the first level of items
            tvi . IsExpanded = true;
            IsExpanded = true;
            IsSelected = true;
            CurrentBaseItem = tvi;
            CurrentActiveItem = tvi;
            ExpArgs . ExpandLevels = 3;
            ExpArgs . tvitem = tvi;
            if ( usetask == false )
            {
                // Works beautifuly = FAST too
                Dispatcher . BeginInvoke ( DispatcherPriority . Normal , ( Action ) ( async ( ) => await TestTree_ExpandedAsync ( tvi , ExpArgs ) ) );
            }
            else
                TestTree_Expanded ( tvi );
            //e . Handled = true;
        }

        public bool IsExpanding { get; set; } = false;
        public int currentexpandlevel { get; set; } = 1;

        private Task<bool> TestTree_ExpandedAsync ( TreeViewItem item , ExpandArgs args )
        {
            bool success = true;
            bool fail = false;
            // All working when clicking on any folder !!!!
            // this gets callled iteratively  as it progress down a tree of subdirectories
            TreeView tv = TestTree as TreeView;
            //TreeViewItem item = tv . SelectedItem as TreeViewItem;
            string currentHeader = "";
            int currentlevel = 0;
            

            if ( item == null )
            {
                //if ( TestTree . SelectedItem != null )
                //    item = TestTree . SelectedItem as TreeViewItem;
                //else
                //{
                return Task . FromResult ( fail );
                //}
            }
            CurrentActiveItem = item;
            currentlevel = GetCurrentLevel ( item . Tag . ToString ( ) );
            currentHeader = item . Header . ToString ( );
            //  item . Header = item . Tag . ToString ( );
            Debug. WriteLine ( $"Async Current Level 1 = {currentlevel} : Max : {ExpArgs . ExpandLevels}  :   {item . Tag}" );
            item . IsSelected = true;
            //            ActiveTree . HorizontalContentAlignment = HorizontalAlignment . Left;
            ScrollCurrentTvItemIntoView ( item );
            TestTree . Refresh ( );
            InfoPanel . Text = $"Expanding {CurrentActiveItem . Tag . ToString ( )}";
            var directories = new List<string> ( );
            var Allfiles = new List<string> ( );
            string Fullpath = item . Tag . ToString ( ) . ToUpper ( );
            int DirectoryCount = 0, filescount = 0;
            int itemscount = item . Items . Count;
            if ( itemscount == 0 )
            {
                return Task . FromResult ( success );
            }
            var tvi = item as TreeViewItem;
            var itemheader = item . Items [ 0 ] . ToString ( );
            //  UpdateListBox ( $"{item . Tag . ToString ( )}" );
            // Get a list of all items in the current folder
            int dircount = GetDirectoryCount ( Fullpath );
            if ( dircount > 0 )
            {
                int count = GetDirectories ( Fullpath , out directories );
                if ( count > 250 )
                {
                    MessageBoxResult result = MessageBox . Show ( $"Directory {Fullpath} contains {count} Files\nExpanding these will take a considerable time...\n\nAre you sure you want to continue ?" ,
                     "Potential long delay" , MessageBoxButton . YesNoCancel , MessageBoxImage . Warning , MessageBoxResult . Cancel );
                    if ( result == MessageBoxResult . Yes )
                    {
                        // Remove DUMMY entry
                        if ( itemheader != null && itemheader == "Loading" )
                            item . Items . Clear ( );
                        DirectoryCount = count;
                        DirectoryCount = AddDirectoriesToTestTree( directories , item , null );
//                        DirectoryCount = AddDirectoriesToTestTreeview ( directories , item , null );
                    }
                    else if ( result == MessageBoxResult . Cancel )
                    {
                        CancelExpand = true;
                        {
                            //                           Caller . Header = currentHeader;
                        }
                    }
                    else
                    {
                        {
                            //                           Caller . Header = currentHeader;
                            return Task . FromResult ( success );
                        }
                    }
                }
                else
                {
                    DirectoryCount = count;
                    if ( directories . Count > 0 )
                    {
                        if ( item . Items [ 0 ] . ToString ( ) == "Loading" )
                        {
                            item . Items . Clear ( );
                        }
                        item . IsExpanded = true;
                        ScrollCurrentTvItemIntoView ( item );
                        TestTree . Refresh ( );
                        DirectoryCount = AddDirectoriesToTestTree ( directories , item , null );
                    }
                }
            }
            else
            {
                DirectoryCount = 0;
            }
            // Now Get FILES

            if ( GetFilesCount ( Fullpath ) > 0 )
            {
                GetFiles ( Fullpath , out Allfiles );
                filescount = Allfiles . Count;
                if ( filescount > 500 )
                {
                    MessageBoxResult result = MessageBox . Show ( $"Directory {Fullpath} contains {filescount} Files\nExpanding these will take a considerable time...\n\nAre you sure you want to expand  thiis  subdirectory?\n\n(Cancel to stop the entire Expansion immediately)" ,
                     "Potential long delay" , MessageBoxButton . YesNoCancel , MessageBoxImage . Warning , MessageBoxResult . Cancel );
                    if ( result == MessageBoxResult . Yes )
                    {
                        if ( item . Items [ 0 ] . ToString ( ) == "Loading" )
                        {
                            item . Items . Clear ( );
                        }
                        AddFilesToTreeview ( Allfiles , item );
                    }
                    else if ( result == MessageBoxResult . Cancel )
                    {
                        {
                            CancelExpand = true;
                            return Task . FromResult ( success );
                        }
                    }
                    else
                    {
                        {
                            return Task . FromResult ( success );
                        }
                    }
                }
                else
                {
                    if ( filescount > 0 )
                        AddFilesToTreeview ( Allfiles , item );
                    else
                    {
                        if ( DirectoryCount == 0 )
                        {
                            if ( item . Items [ 0 ] . ToString ( ) == "Loading" )
                            {
                                //                            item . Items . Clear ( );
                            }
                            item . IsExpanded = false;
                        }
                    }
                    TestTree . Refresh ( );
                }
            }

            if ( DirectoryCount == 0 && Allfiles . Count == 0 )
            {
                try
                {
                    if ( item . Items [ 0 ] . ToString ( ) == "Loading" )
                    {
                        //                       item . Items . Clear ( );
                        TestTree . Refresh ( );
                    }
                    item . IsExpanded = false;
                }
                catch { }
                finally
                {
                    item . IsSelected = true;
                }
            }
            else
            {
                TestTree . Refresh ( );
            }
            return Task . FromResult ( success );
        }

        #endregion PROCESSING METHODS

        #region general handlers
        private void TestTree_MouseDoubleClick ( object sender , MouseButtonEventArgs e )
        {

        }

        private void TestTree_MouseEnter ( object sender , MouseEventArgs e )
        {

        }

        private void TREEViews_MouseMove ( object sender , MouseEventArgs e )
        {

        }

        private void TREEViews_IsMouseDirectlyOverChanged ( object sender , DependencyPropertyChangedEventArgs e )
        {

        }

        #endregion general handlers


        #region ADD Treeview Item Methods
        public int AddFilesToTreeview ( List<string> Allfiles , TreeViewItem item )
        {
            int count = 0;
            if ( item . Items . Count == 1 )
            {
                item . Items . Clear ( );
            }
            item . IsSelected = false;
            foreach ( var itm in Allfiles )
            {
                //if ( CheckIsVisible ( itm . ToUpper ( ) , ShowAllFiles , out HasHidden ) == true )
                //{
                var subitem = new TreeViewItem ( );
                subitem . Header = GetFileFolderName ( itm );
                subitem . Tag = itm;
                subitem . IsExpanded = false;
                //if ( TrackExpand )
                //    subitem . IsSelected = true;
                subitem . Items . Clear ( );
                item . Items . Add ( subitem );
                count++;
            }
            //}
            return count;
        }
        public int AddDirectoriesToTestTree ( List<string> directories , TreeViewItem item , ListBox lBox = null , bool UseExpand = true )
        {
            // Adds a series of EMPTY folders  to parent item
            int added = 0;
            int TotalDirs = 0;
#pragma warning disable CS0219 // The variable 'TotalFiles' is assigned but its value is never used
            int TotalFiles = 0;
#pragma warning restore CS0219 // The variable 'TotalFiles' is assigned but its value is never used
            item . Items . Clear ( );
            foreach ( var directoryPath in directories )
            {
                var dummy = new TreeViewItem ( );
                var subitem = new TreeViewItem ( );

                subitem . Header = GetFileFolderName ( directoryPath );
                subitem . Tag = directoryPath;
                TotalDirs = GetDirectoryCount ( directoryPath );
//                TotalFiles = GetFilesCount ( directoryPath );
//                if ( TotalFiles == -1 )
//                    TotalFiles = 0;
                item . Items . Add ( subitem );
                //    // Add DUMMY entry as we have content in this folder
                dummy . Header = "Loading";
                subitem . Items . Add ( dummy );
                item . IsExpanded = true;
                ScrollCurrentTvItemIntoView ( subitem );
               added++;
           }
            return added;
        }
        public int AddDirectoriesToTestTreeview ( List<string> directories , TreeViewItem item , ListBox lBox = null , bool UseExpand = true )
        {
            int added = 0;
            if ( directories . Count == 0 )
                return 0;
            item . Items . Clear ( );
            foreach ( var dir in directories )
            {
                var subitem = new TreeViewItem ( );
                try
                {
                    subitem . Header = GetFileFolderName ( dir );
                    subitem . Tag = dir;
                    item . Items . Add ( subitem );
                    item . IsExpanded = true;
                    subitem . BringIntoView ( );
                    int count = GetDirectories ( dir , out directories );
                    if ( count > 0 )
                    {
                        var tv = new TreeViewItem ( );
                        tv . Header = "Loading";
                        subitem . Items . Add ( tv );
                        AddDirectoriesToTestTreeview ( directories , subitem , null );
                    }
                    else
                    {
                        var tv = new TreeViewItem ( );
                        tv = item;
                        if ( tv . Header . ToString ( ) == "Loading" )
                            item . Items . Clear ( );
                    }
                }
                //}
                catch ( Exception ex )
                {
                    Debug. WriteLine ( $"AddDirectoriesoTestTreeView : 903 ; Invalid  directory accessed {ex . Message}" );
                }
            }
            added++;
            return added;
        }

        #endregion ADD Treeview Item Methods


        #region GET Treeview Item Info Methods

        public int GetCurrentLevel ( string currentpath )
        {
            int count = 0;
            string [ ] paths = currentpath . Split ( '\\' );
            count = paths . Length - 1;
            return count;
        }
        public int GetFiles ( string path , out List<string> allfiles )
        {
            int count = 0;
            var files = new List<string> ( );
            allfiles = new List<string> ( );
            // Get a list of all items in the current folder
            try
            {
                if ( GetFilesCount ( path ) <= 0 )
                    return 0;
                //var file = Directory . EnumerateFiles ( path , "*.*" );
                var filecount = Directory . GetFiles ( path , "*.*" , SearchOption . TopDirectoryOnly );
                if ( filecount . Count ( ) > 0 )
                {
                    foreach ( var item in filecount )
                    {
                        //if ( CheckIsVisible ( item . ToUpper ( ) , ShowAllFiles , out HasHidden ) == true )
                        //{
                        files . Add ( item );
                        //if(item. ToUpper().Contains("BCD.LOG") )
                        //        Debug. WriteLine ( );

                        //                              Debug. WriteLine ();
                        count++;
                        allfiles . Add ( item );
                        // working correctly
                        //}

                    }
                }
            }
            catch ( Exception ex )
            {
                Debug. WriteLine ( $"GetFiles : 1052 : {ex . Message}" );
            }
            //allfiles = files;
            return count;
        }
        public int GetFilesCount ( string path )
        {
            int count = 0;
            bool result = true;
            var files = new List<string> ( );
            // Get a list of all items in the current folder
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            try
            {
                //var file = Directory . EnumerateFiles ( path , "*.*" );
                var dirfile = Directory . GetFiles ( path , "*.*" , SearchOption . TopDirectoryOnly );
                count = ( int ) dirfile . Length;
            }
            catch ( Exception ex )
            {
                //Debug. WriteLine ( $"GetFilesCount : 1081 : {ex . Message}" );
                result = false;
            }
#pragma warning restore CS0168 // The variable 'ex' is declared but never used

            //allfiles = files;
            if ( result == false )
                return -1;

            return count;
        }
        public int GetDirectories ( string path , out List<string> dirs )
        {
            bool filterSysfiles = false;
            int count = 0;
            List<string> directories = new List<string> ( );
            try
            {
                string [ ] directs = Directory . GetDirectories ( path , "*.*" , SearchOption . TopDirectoryOnly );
                if ( directs . Length > 0 )
                {
                    foreach ( var item in directs )
                    {
                        try
                        {
                            if ( filterSysfiles )
                            {
                                //if ( IsSystemFile ( item . ToUpper ( ) ) == true )
                                //{
                                //    continue;
                                //}
                            }
                            directories . Add ( item );
                            count++;
                        }
                        catch ( Exception ex ) { Debug. WriteLine ( $"GetDirectories : 980 : {ex . Message}" ); }
                    }
                }
            }
            catch ( Exception ex )
            {
                { Debug. WriteLine ( $"GetDirectories : 981 : {ex . Message}" ); }
            }
            dirs = directories;
            return count;
        }
        public int GetDirectoryCount ( string path )
        {
            int count = 0;
            List<string> directories = new List<string> ( );
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            try
            {
                string [ ] directs = Directory . GetDirectories ( path , "*.*" , SearchOption . TopDirectoryOnly );
                foreach ( var item in directs )
                {
                    //if ( CheckIsVisible ( item . ToUpper ( ) , ShowAllFiles , out HasHidden ) == true )
                    //{
                    count++;
                    //}
                    //count = directs . Length;
                }
            }
            catch ( Exception ex )
            {
                { //Debug. WriteLine ( $"GetDirectoryCount : 9968 : {ex . Message}" );
                }
            }
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            return count;
        }
        public static string GetFileFolderName ( string path )
        {
            if ( string . IsNullOrEmpty ( path ) )
                return String . Empty;
            var normalizedPath = path . Replace ( '/' , '\\' );
            var lastindex = normalizedPath . LastIndexOf ( '\\' );
            if ( lastindex <= 0 )
                return path;
            return path . Substring ( lastindex + 1 );
        }

        #endregion GET Treeview Item Info Methods


        #region utility methods        
        public void ScrollCurrentTvItemIntoView ( TreeViewItem item )
        {
            // Brings selected item  into view as selected item
            var count = VisualTreeHelper . GetChildrenCount ( item );

            for ( int i = count - 1 ; i >= 0 ; --i )
            {
                var childItem = VisualTreeHelper . GetChild ( item , i );
                if ( childItem != ( DependencyObject ) null )
                    ( ( FrameworkElement ) childItem ) . BringIntoView ( );
                //item.BringIntoView ( );
            }
        }

        #endregion utility methods        

        private void CboxExactMatch_Click ( object sender , RoutedEventArgs e )
        {

        }

        
        
        
        #region NON threaded methods
        private bool TestTree_Expanded ( TreeViewItem item )
        {
            string currentHeader = "";
            int currentlevel = 0;
            // Needed to let us show the volume label if the option is checked
            TreeViewItem Caller = new TreeViewItem ( );
            //TreeViewItem item = null;
            int itemscount = 0;
            //if ( IsExpanding == false )
            //{
            //    if ( e != null )
            //        item = e . Source as TreeViewItem;
            //    else
            //        item = sender as TreeViewItem;
            //    if ( item == null )
            //    {
            //        if ( TestTree . SelectedItem != null )
            //            item = TestTree . SelectedItem as TreeViewItem;
            //        else
            //        {
            //            iterations = 0;
            //            BusyLabel . Text = "";
            //            return;
            //        }
            //    }
            //}
            //else
            //{
            //    item = e . Source as TreeViewItem;
            //    //ExpandSpecifiedLevels ( item , e );
            //}
            Caller = item;
            currentlevel = GetCurrentLevel ( item . Tag . ToString ( ) );
            currentHeader = item . Header . ToString ( );
            Debug. WriteLine ( $"Level = {currentlevel} : {item . Header}  ||   {item . Tag}" );
            // This is CRITICAL to get any drive that is currently selected to open when the expand icon is clicked

            item . IsSelected = true;
//            Selection . Text = $"{item . Tag . ToString ( )}";
            //            ActiveTree . HorizontalContentAlignment = HorizontalAlignment . Left;
            ScrollCurrentTvItemIntoView ( item );
            TestTree. Refresh ( );

            var directories = new List<string> ( );
            var Allfiles = new List<string> ( );
            string Fullpath = item . Tag . ToString ( ) . ToUpper ( );
            int DirectoryCount = 0, filescount = 0;
            itemscount = item . Items . Count;
            if ( itemscount == 0 )
            {
                return true;
            }
            var tvi = item as TreeViewItem;
            Caller . Header = currentHeader;
            var itemheader = item . Items [ 0 ] . ToString ( );
            // Get a list of all items in the current folder
            int dircount = GetDirectoryCount ( Fullpath );
            if ( dircount > 0 )
            {
                int count = GetDirectories ( Fullpath , out directories );
                if ( count > 250 )
                {
                    MessageBoxResult result = System . Windows . MessageBox . Show ( $"Directory {Fullpath} contains {count} Files\nExpanding these will take a considerable time...\n\nAre you sure you want to continue ?" ,
                     "Potential long delay" , MessageBoxButton . YesNoCancel , MessageBoxImage . Warning , MessageBoxResult . Cancel );
                    if ( result == MessageBoxResult . Yes )
                    {
                        // Remove DUMMY entry
                        if ( itemheader != null && itemheader == "Loading" )
                            item . Items . Clear ( );
                        DirectoryCount = count;
                        DirectoryCount = AddDirectoriesToTestTree( directories , item , null );
//                        DirectoryCount = AddDirectoriesToTestTreeview ( directories , item , null );
                    }
                    else if ( result == MessageBoxResult . Cancel )
                    {
                        CancelExpand = true;
                        {
                            Caller . Header = currentHeader;
                            return true;
                        }
                    }
                    else
                    {
                        ExpandLimited = true;
                        {
                            Caller . Header = currentHeader;
                            return true;
                        }
                    }
                }
                else
                {
                    DirectoryCount = count;
                    if ( directories . Count > 0 )
                    {
                        if ( item . Items [ 0 ] . ToString ( ) == "Loading" )
                        {
                            item . Items . Clear ( );
                        }
                        // Expand folder so we can add Directories to it.
                        item . IsExpanded = true;
                        ScrollCurrentTvItemIntoView ( item );
                        TestTree . Refresh ( );
                        DirectoryCount = AddDirectoriesToTestTree ( directories , item , null );
                        //                        item . IsExpanded = true;
                    }
                }
            }
            else
            {
                DirectoryCount = 0;
            }
            // Now Get FILES

            if ( GetFilesCount ( Fullpath ) > 0 )
            {
                GetFiles ( Fullpath , out Allfiles );
                filescount = Allfiles . Count;
                if ( filescount > 500 )
                {
                    MessageBoxResult result = System . Windows . MessageBox . Show ( $"Directory {Fullpath} contains {filescount} Files\nExpanding these will take a considerable time...\n\nAre you sure you want to expand  thiis  subdirectory?\n\n(Cancel to stop the entire Expansion immediately)" ,
                     "Potential long delay" , MessageBoxButton . YesNoCancel , MessageBoxImage . Warning , MessageBoxResult . Cancel );
                    if ( result == MessageBoxResult . Yes )
                    {
                        if ( item . Items [ 0 ] . ToString ( ) == "Loading" )
                        {
                            item . Items . Clear ( );
                        }
                        AddFilesToTreeview ( Allfiles , item );
                    }
                    else if ( result == MessageBoxResult . Cancel )
                    {
                        CancelExpand = true;
                        {
                            Caller . Header = currentHeader;
                            return true; ;
                        }
                    }
                    else
                    {
                        ExpandLimited = true;
                        {
                            Caller . Header = currentHeader;
                            return true;
                        }
                    }
                }
                else
                {
                    if ( filescount > 0 )
                        AddFilesToTreeview ( Allfiles , item );
                    else
                    {
                        if ( DirectoryCount == 0 )
                        {
                            if ( item . Items [ 0 ] . ToString ( ) == "Loading" )
                            {
                                //                            item . Items . Clear ( );
                            }
                            item . IsExpanded = false;
                        }
                    }
                    //ActiveTree . HorizontalAlignment = HorizontalAlignment . Left;
                    if ( CancelExpand)
                        ScrollCurrentTvItemIntoView ( item );
                    TestTree . Refresh ( );
                }
            }
            // item now has all its Subdirs and files
            if ( DirectoryCount == 0 && Allfiles . Count == 0 )
            {
                try
                {
                    if ( item . Items [ 0 ] . ToString ( ) == "Loading" )
                    {
                        //                       item . Items . Clear ( );
                        TestTree . Refresh ( );
                    }
                    item . IsExpanded = false;
                }
                catch { }
                finally
                {
                    item . IsSelected = true;
                }

                if ( item . Header . ToString ( ) . ToUpper ( ) . Contains ( "CDROM" ) )
                    InfoPanel. Text = "This item is a CdRom, but No Media has been identified, Access is denied by Windows ...";
                else
                    InfoPanel . Text = "This item does not contain any (Non System / Hidden files), or perhaps Access is denied by Windows ...";
                //Utils . DoErrorBeep ( 280 , 100 , 1 );
            }
            else
            {
                //  item . IsExpanded = true;
                TestTree . UpdateLayout ( );
                TestTree . Refresh ( );
//                Selection . Text = $"{item . Header . ToString ( )} SubDirectories = {DirectoryCount} , Files = {Allfiles . Count}";


                //if ( currentexpandlevel <= ExpArgs . ExpandLevels )
                //{
                //    e . Source = item . Items [ 0 ];
                //    IsExpanding = true;
                //    //foreach ( TreeViewItem subdir  in item . Items )
                //    //{
                //    //e . Source = subdir;
                //    Debug. WriteLine ( $"Expanding {item . Tag . ToString ( )}" );
                //    TestTree_Expanded ( sender , e );
                //    //}
                //    currentexpandlevel++;
                //}
            }

            //            ActiveTree . HorizontalContentAlignment = HorizontalAlignment . Left;
            //if ( Caller != null && Caller . IsExpanded == true )
            //{
            //    Caller . IsSelected = true;
            //    if ( TrackExpand )
            //        ScrollCurrentTvItemIntoView ( Caller );
            //    Caller . BringIntoView ( );
            //    ActiveTree . UpdateLayout ( );
            //    ActiveTree . Refresh ( );
            //}
            currentexpandlevel = 1;
            IsExpanding = false;

            return true;
        }

        #endregion NON threaded methods


        #region Methods tohandle Context Menu selections
        async private void TriggerExpand0 ( object sender , RoutedEventArgs e )
        {
              var tvi = ( TreeViewItem ) TestTree . SelectedItem;
            if ( tvi == null )
                return;
            tvi . IsExpanded = true;
            IsExpanded = true;
            IsSelected = true;
            CurrentBaseItem = tvi;
            CurrentActiveItem = tvi;
            level = 1;
            ExpArgs . ExpandLevels = Convert.ToInt16(Expandcount.Text);
            if ( ExpArgs . ExpandLevels < 0 || ExpArgs . ExpandLevels > 9 )
                return;
            ExpArgs . ExpandLevels -= 1;
            ExpArgs . CurrentLevel = 1;
            stack0 . Clear ( );
            slist1 . Clear ( );
            Mouse . OverrideCursor = Cursors . Wait;

            await Dispatcher . BeginInvoke ( DispatcherPriority . Normal , ( Action ) ( async ( ) => await RunExpandSystem ( tvi )  ) );
            //RunExpandSystem ( tvi ) ;
            InfoPanel . Text = $"{tvi . Tag . ToString ( )} has been expanded {ExpArgs . ExpandLevels - 1}";
            Mouse . OverrideCursor = Cursors . Arrow;
        }
         public Task<bool> RunExpandSystem ( TreeViewItem tvitem )
        {
            bool success = true;
#pragma warning disable CS0219 // The variable 'fail' is assigned but its value is never used
            bool fail = false;
#pragma warning restore CS0219 // The variable 'fail' is assigned but its value is never used
            ExpandSpecifiedLevels ( tvitem , null );
            TestTree . Refresh ( );
            if ( ExpArgs . ExpandLevels > 1 )
            {
                while ( ExpArgs . CurrentLevel < ExpArgs . ExpandLevels )
                {
                    Debug. WriteLine ( $"Looping : Current level is {ExpArgs . CurrentLevel}, Max = {ExpArgs . ExpandLevels}" );
                    slist1 . Clear ( );
                    foreach ( var item in stack0 )
                    {
                        slist1 . Add ( item );
                    }
                    stack0 . Clear ( );
                    foreach ( var item in slist1 )
                    {
                        Debug. WriteLine ( $"{item . Tag . ToString ( )}" );
                        ExpandSpecifiedLevels ( item , null );
                        item . IsSelected = true;
                        ScrollCurrentTvItemIntoView ( item );
                        TestTree . Refresh ( );
                        if ( CancelExpand )
                            break;
                        //stack0 . Push( item );
                    }
                    ExpArgs . CurrentLevel++;
                    TestTree . Refresh ( );
                    if ( CancelExpand )
                        break;
                }
            }

            return Task.FromResult(success);
        }
        int level { get; set; }
        public bool ExpandSpecifiedLevels ( TreeViewItem tvitem , RoutedEventArgs e )
        {
            IsExpanding = false;
            //tvitem . IsExpanded = true;
            TestTree_Expanded ( tvitem );
            tvitem . IsSelected = true;
            TestTree . Refresh ( );
            if ( tvitem . Items [ 0 ].ToString() == "Loading" )
            {
                tvitem . IsExpanded = false;
                return false;
            }
            if ( ExpArgs . ExpandLevels >= 1 )
            {

                Debug. WriteLine ( $"Expanding {tvitem . Tag . ToString ( )} with {tvitem . Items . Count} items" );
                try
                {
                    foreach ( TreeViewItem subdir in tvitem . Items )
                    {
                        if ( subdir . Items . Count == 0 )
                            return false;
                        TestTree_Expanded ( subdir );
                        if ( CancelExpand )
                            break;
                        subdir . IsSelected = true;
                        TestTree . Refresh ( );
                        if ( subdir . Items . Count > 0 )
                        {
                            subdir . IsExpanded = true;
                            if ( subdir . Items [ 0 ] . ToString ( ) == "Loading" )
                                subdir . IsExpanded = false;
                            else
                                Debug. WriteLine ( $"Subdir {subdir . Tag . ToString ( )} with {subdir . Items . Count} items Expanded" );
                            stack0 . Push ( subdir );
                            subdir . IsSelected = true;
                        }
                        else
                            subdir . IsExpanded = false;
                    }
                }
                catch ( Exception ex )
                { Debug. WriteLine ( $"{tvitem . Header . ToString ( )} caused error\n{ex . Message}" ); }
                //ExpArgs . CurrentLevel++;
                IsExpanding = false;
            }
            level = 1;
            return true;
        }
  
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        async private void TriggerExpand1 ( object sender , RoutedEventArgs e )
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            if ( TestTree . SelectedItem == null )
            {
                //MessageBox . Show ( $"Please select a drive or subfolder before using  these options...." , "No Drive Selected" );
                //fdl . ShowInfo ( Flowdoc , canvas ,
                //      $"Please select a drive or subfolder before using  this option...." ,
                //      "Blue1" ,
                //      "TreeView Search Sytem" );
                //fdl . SetFocus ( );
                return;
            }
            object [ ] Args = { TestTree . SelectedItem as TreeViewItem , ( object ) 2 , null };
            //            startitem = TestTree . SelectedItem as TreeViewItem;
            if ( ExpArgs . SearchActive == false )
            {
                ExpArgs . tvitem = TestTree . SelectedItem as TreeViewItem;
                ExpArgs . Selection = 1;
                ExpArgs . ExpandLevels = 4;
            }
            else
            {
                ExpArgs . Selection = 1;
            }
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
            RunRecurse ( TestTree , null );
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
            //Dispatcher . BeginInvoke ( DispatcherPriority . Normal , ( Action ) ( async ( ) => await RunRecurse ( TestTree , null ) ) );

            //RunExpandSystem ( null , null );
            return;
        }

#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        async private void TriggerExpand2 ( object sender , RoutedEventArgs e )
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {

        }

#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        async private void TriggerExpand3 ( object sender , RoutedEventArgs e )
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {

        }

#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        async private void TriggerExpand4 ( object sender , RoutedEventArgs e )
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {

        }

        private void ShowFullPath ( object sender , RoutedEventArgs e )
        {

        }

        private void CollapseCurrent ( object sender , RoutedEventArgs e )
        {
            TreeViewItem tv = sender as TreeViewItem;
            tv = TestTree . SelectedItem as TreeViewItem;
            if ( tv != null )
                tv . IsExpanded = false;
        }

        private void CollapseAll ( object sender , RoutedEventArgs e )
        {
            CollapseAllDrives ( );

        }

        #endregion Methods tohandle Context Menu selections

        // NON threaded methods

        private void App_Close ( object sender , RoutedEventArgs e )
        {
            this . Close ( );
            Application . Current . Shutdown ( );

        }
        private void CollapseAllDrives ( )
        {
            Mouse . OverrideCursor = Cursors . Wait;
            TreeView tv = new TreeView ( );
            if ( ExpArgs . tv != null )
                tv = ExpArgs . tv;
            else
                tv = TestTree;
            foreach ( TreeViewItem item in tv . Items )
            {
                if ( item . IsExpanded )
                    item . IsExpanded = false;
            }
            tv . Refresh ( );
            Mouse . OverrideCursor = Cursors . Arrow;
        }

        public Task<bool> RunRecurse ( TreeView ActiveTree , ProgressChangedEventArgs e )
        {
            // e contains current treeviewitem !!!
            // All parameters are in ExpandArgs ExpArgs
#pragma warning disable CS0219 // The variable 'Returnval' is assigned but its value is never used
            bool Returnval = false;
#pragma warning restore CS0219 // The variable 'Returnval' is assigned but its value is never used
            bool IsComplete = false;
            int iterations = 0;

            int itemcount = 0;
            int levelscount = 0;
            var fail = false;
            var success = true;

            if ( ExpArgs . SearchTerm != null )
                // Force search to be upper case
                ExpArgs . SearchTerm = ExpArgs . SearchTerm . ToUpper ( );
            else
                ExpArgs . SearchTerm = "";
            //ExpandArgs eargs = e . UserState as ExpandArgs;
            //int levels = ( int ) Args [ 1 ];
            int levels = ExpArgs . ExpandLevels;

            TreeViewItem startup = new TreeViewItem ( );
            TreeViewItem startitem = new TreeViewItem ( );
            TreeViewItem items = ExpArgs . tvitem;
            startitem = items;
            fail = false;
            if ( items == null )
                return Task . FromResult ( success );
            //ProgressCount = 0;
            if ( items . Tag . ToString ( ) . ToUpper ( ) . Contains ( items . Header . ToString ( ) . ToUpper ( ) ) == false )
                items . Header = items . Tag . ToString ( );
            // Essential to force root to be expanded, else nothing happens
            if ( CheckSearchSuccess ( items . Tag . ToString ( ) ) == true )
            {
                ScrollCurrentTvItemIntoView ( items );
                ExpArgs . SearchSuccessItem = items;
                items . IsSelected = true;
                //    SearchSuccess = true;
                ExpArgs . SearchSuccess = true;
                return Task . FromResult ( success );
            }

            try
            {
                items . IsSelected = true;
                items . IsExpanded = true;
                if ( CancelExpand )
                    return Task . FromResult ( fail );
                //stack . Push ( items . Tag . ToString ( ) );
            }
            catch ( Exception ex ) { Debug. WriteLine ( $"RunRecurse: 3304 : {ex . Message}" ); }
            Thread . Sleep ( SLEEPTIME );
            //if ( FullExpandinProgress == false )
            //    items . Refresh ( );
            levelscount = CalculateLevel ( items . Tag . ToString ( ) );
            if ( levelscount >= ExpArgs . ExpandLevels )
            {
                items . IsSelected = true;
                items . IsExpanded = true;
                Debug. WriteLine ( $"{items . Header . ToString ( )} Expanded.... ?" );
                ActiveTree . Refresh ( );
                return Task . FromResult ( success );
            }
            //**************
            // Main LOOP
            //**************

            foreach ( var objct in items . Items )
            {
                if ( objct . ToString ( ) == "Loading" )
                    break;
                startup = objct as TreeViewItem;
                //Thread . Sleep ( SLEEPTIME );
                TreeViewItem obj = objct as TreeViewItem;
                //levelscount = CalculateLevel ( obj . Tag . ToString ( ) );
                if ( levelscount > ExpArgs . ExpandLevels )
                {
                    continue;
                }
                InfoPanel . Text = $"Expanding {obj . Tag . ToString ( )}";
                TreeViewItem childControl = obj as TreeViewItem;
                // working correctly
                if ( CheckSearchSuccess ( childControl . Tag . ToString ( ) ) == true )
                {
                    ScrollCurrentTvItemIntoView ( childControl );
                    childControl . IsSelected = true;
                    ExpArgs . SearchSuccessItem = childControl;
                    ExpArgs . SearchSuccess = true;
                    Returnval = true;
                    IsComplete = true;
                    ActiveTree . Refresh ( );
                    break;
                }

                try
                {
                    if ( childControl . Items . Count == 1 )
                    {
                        TreeViewItem test = childControl . Items [ 0 ] as TreeViewItem;
#pragma warning disable CS0252 // Possible unintended reference comparison; to get a value comparison, cast the left hand side to type 'string'
                        if ( test . Header == "Loading" )
                            continue;
#pragma warning restore CS0252 // Possible unintended reference comparison; to get a value comparison, cast the left hand side to type 'string'
                    }
                    //                    ExpandFolder ( childControl );
                    childControl . IsExpanded = true;
                    if ( CancelExpand )
                        return Task . FromResult ( fail );
                }
                catch ( Exception ex ) { Debug. WriteLine ( $"RunRecurse: 3361 : {ex . Message}" ); }

                //                if ( ClosePreviousNode )
                //                  Debug. WriteLine ( $"Closeprevious 1 : {childControl.Tag.ToString()}" );

                levelscount = CalculateLevel ( childControl . Tag . ToString ( ) );
                if ( levelscount >= ExpArgs . ExpandLevels )
                {
                    if ( ExpArgs . SearchTerm != "" )
                    {
                        childControl . IsExpanded = false;
                        ActiveTree . Refresh ( );
                    }
                    continue;
                }
                if ( childControl != null )//&& ExpArgs . ExpandLevels >= 2 )
                {
                    string entry = childControl . Header . ToString ( ) . ToString ( ) . ToUpper ( );
                    itemcount = childControl . Items . Count;
                    if ( itemcount == 0 )
                        continue;
                    iterations++;
                    if ( ExpArgs . ExpandLevels >= 3 )
                    {
                        bool nofault = false;
                        //******************
                        // INNER LOOP
                        //******************
                        //UpdateListBox ( childControl. Tag . ToString ( ) );

                        foreach ( var newentry in childControl . Items )
                        {
                            TreeViewItem nextitem = new TreeViewItem ( );
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
                            try
                            {
                                nextitem = newentry as TreeViewItem;
                            }
                            catch ( Exception ex )
                            {
                                Debug. WriteLine ( $"Unable to access {childControl . Tag . ToString ( )}" );
                                nofault = true;
                            }
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
                            if ( nofault || nextitem == null )
                            {
                                nofault = false;
                                continue;
                            }
                            if ( nextitem . Header . ToString ( ) == "Loading" )
                                continue;
                            //if ( CheckIsVisible ( nextitem . Tag . ToString ( ) . ToUpper ( ) , ShowAllFiles , out HasHidden ) == false )
                            //{
                            //    Debug. WriteLine ( $"System file : {nextitem . Tag . ToString ( ) . ToUpper ( )}" );
                            //    continue;
                            //}

                            if ( CheckSearchSuccess ( nextitem . Tag . ToString ( ) ) == true )
                            {
                                ScrollCurrentTvItemIntoView ( nextitem );
                                nextitem . IsSelected = true;
                                //SearchSuccess = true;
                                ExpArgs . SearchSuccess = true;
                                ExpArgs . SearchSuccessItem = nextitem;
                                Returnval = true;
                                IsComplete = true;
                                break;
                            }

                            try
                            {
                                iterations++;
                                nextitem . IsExpanded = true;
                                if ( nextitem . Items . Count == 0 || nextitem . Items [ 0 ] . ToString ( ) == "Loading" )
                                    continue;
                                if ( CancelExpand )
                                    return Task . FromResult ( fail );
                            }
                            catch ( Exception ex ) { Debug. WriteLine ( $"RunRecurse: 3424 : {ex . Message}" ); }
                            // working correctly
                            //                            Debug. WriteLine ( Selection . Text );
                            levelscount = CalculateLevel ( nextitem . Tag . ToString ( ) );
                            if ( levelscount >= ExpArgs . ExpandLevels )
                            {
                                continue;
                            }
                            //if ( nextitem . Tag . ToString ( ) . Contains ( "-500" ) )
                            //    Returnval = Returnval;
                            if ( ExpArgs . ExpandLevels >= 4 )
                            {
                                bool HasHidden = false;
                                //                                UpdateListBox ( nextitem . Tag . ToString ( ) );
                                if ( CheckIsVisible ( nextitem . Tag . ToString ( ) . ToUpper ( ) , true , out HasHidden ) == false )
                                {
                                    Debug. WriteLine ( $"System file : {nextitem . Tag . ToString ( ) . ToUpper ( )}" );
                                    continue;
                                }
                                //Thread . Sleep ( SLEEPTIME );
                                if ( nextitem . HasItems )
                                {
                                    if ( ExpandAll3 ( nextitem , true , ExpArgs . ExpandLevels ) == true )
                                    {
                                        ExpArgs . SearchSuccess = true;
                                        Returnval = true;
                                        IsComplete = true;
                                        break;
                                    }
                                    else
                                    {
                                        if ( ExpArgs . SearchActive == true && ExpArgs . SearchSuccess == false )
                                        {
                                            // ONLY If Searching , Close the subdir we have just finished prcessing
                                            nextitem . IsExpanded = false;
                                            ActiveTree . Refresh ( );
                                        }
                                    }
                                }
                            }

                        }   // End INNER FOREACH

                        if ( ExpArgs . SearchActive == true && ExpArgs . SearchSuccess == false )
                        {
                            // ONLY If Searching , Close the subdir we have just finished prcessing
                            childControl . IsExpanded = false;
                            ActiveTree . Refresh ( );
                        }
                        if ( IsComplete )
                            break;
                    }
                }
                if ( IsComplete )
                    break;
            }   // End FOREACH

            if ( ExpArgs . SearchSuccess == false )
            {
                ScrollCurrentTvItemIntoView ( startup );
                startitem . IsSelected = true;
            }
            UpdateDriveHeader ( ShowVolumeLabels );
            return Task . FromResult ( success );
        }
        public int CalculateLevel ( string currentitem )
        {
            int len = 0;
            string [ ] levels = currentitem . Split ( '\\' );
            if ( levels [ 1 ] == "" )
                return 1;
            else len = levels . Length;
            return len;
        }
        private bool ExpandAll3 ( TreeViewItem items , bool expand , int levels )
        {
            if ( items == null )
                return false;

            //          levels = ExpArgs . ExpandLevels;
            foreach ( object obj in items . Items )
            {
                if ( CancelExpand )
                    return false;
                //iterations++;
                TreeViewItem childControl = obj as TreeViewItem;
                if ( childControl != null )
                {
                    try
                    {
                        childControl . IsExpanded = true;
                    }
                    catch ( Exception ex ) { Debug. WriteLine ( $"ExpandAll3: 1427 : {ex . Message}" ); }
                    if ( childControl . Header . ToString ( ) == "Loading" )
                        continue;

                    //if ( CheckIsVisible ( childControl . Header . ToString ( ) . ToUpper ( ) , ShowAllFiles , out HasHidden ) == false )
                    //{
                    //    continue;
                    //}
                    //if ( CheckSearchSuccess ( childControl . Tag . ToString ( ) ) == true )
                    //{
                    //    UpdateListBox ( $"\nSearch for {Searchtext . Text} found  as [" + childControl . Header . ToString ( ) + $"]\nin {childControl . Tag . ToString ( )}" );
                    //    //ActiveTree . HorizontalAlignment = HorizontalAlignment . Left;
                    //    if ( TrackExpand )
                    //        ScrollCurrentTvItemIntoView ( childControl );
                    //    ExpArgs . SearchSuccessItem = childControl;
                    //    ExpArgs . SearchSuccess = true;
                    //    if ( TrackExpand )
                    //        childControl . IsSelected = true;
                    //    fdl . ShowInfo ( Flowdoc , canvas , "Match found !" );
                    //    return true;
                    //}

                    if ( CalculateLevel ( childControl . Tag . ToString ( ) ) > levels )
                        break;

                    if ( childControl . Items . Count > 1 )
                    {
                        TreeViewItem tmp = childControl . Items [ 0 ] as TreeViewItem;
                        if ( tmp . ToString ( ) != "Loading" )
                        {
                            if ( levels == 1 )
                            {
                                InfoPanel . Text = $"Calling ExpandFolder for {childControl . Tag . ToString ( )}";
                                //                                Debug. WriteLine ( Selection . Text );
                                if ( ExpandFolder ( childControl , true ) == true ) // Expand ALL Contents (true)
                                {
                                    //ActiveTree . HorizontalAlignment = HorizontalAlignment . Left;
                                    ExpArgs . SearchSuccess = true;
                                    return true;
                                }
                            }
                            else
                            {
                                InfoPanel . Text = $"Calling ExpandAll3 for {childControl . Tag . ToString ( )}";
                                if ( ExpandAll3 ( childControl as TreeViewItem , expand , levels ) == true )
                                {
                                    //ActiveTree . HorizontalContentAlignment = HorizontalAlignment . Left;
                                    //                                    ScrollCurrentTvItemIntoView ( childControl );
                                    //SearchSuccess = true;
                                    ExpArgs . SearchSuccess = true;
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            try
                            {
                                childControl . IsExpanded = true;
                                //stack . Push ( childControl . Tag . ToString ( ) );
                            }
                            catch ( Exception ex ) { Debug. WriteLine ( $"ExpandAll3: 1503 : {ex . Message}" ); }
                        }
                    }
                    else
                    {
                        try
                        {
                            childControl . IsExpanded = true;
                            //stack . Push ( childControl . Tag . ToString ( ) );
                        }
                        catch ( Exception ex ) { Debug. WriteLine ( $"ExpandAll3: 1517 : {ex . Message}" ); }
                    }
                }
            }
            return false;
        }
        private bool ExpandFolder ( TreeViewItem item , bool ExpandContent = false )
        {
            string fullpath = "";
            if ( CancelExpand )
                return false;
            if ( item . Items . Count > 0 )
            {
                string tmp = item . Header . ToString ( );
                if ( tmp . ToString ( ) != "Loading" )
                {
                    foreach ( TreeViewItem item2 in item . Items )
                    {
                        Thread . Sleep ( SLEEPTIME );

                        //if ( CheckSearchSuccess ( item2 . Tag . ToString ( ) ) == true )
                        //{
                        //    ExpArgs . SearchSuccessItem = item2;
                        //    //SearchSuccess = true;
                        //    ExpArgs . SearchSuccess = true;
                        //    return true;
                        //}
                        fullpath = item2 . Tag . ToString ( ) . ToUpper ( );
                        try
                        {
                            item2 . IsExpanded = true;
                            //stack . Push ( item2 . Tag . ToString ( ) );
                        }
                        catch ( Exception ex ) { Debug. WriteLine ( $"ExpandFolder : 1797 : {ex . Message}" ); }
                    }
                }
            }
            return false;
        }
        private void UpdateDriveHeader ( bool ShowVolumeLabels )
        {
            string drivestring = "";
            string dictvalue = "";
            foreach ( TreeViewItem item in TestTree . Items )
            {
                drivestring = item . Tag . ToString ( );
                if ( ShowVolumeLabels )
                    item . Header = drivestring + "  " + Utils . GetDictionaryEntry ( VolumeLabelsDict , drivestring , out dictvalue );
                else
                    item . Header = drivestring;
            }
            TestTree . Refresh ( );
        }
        public bool CheckSearchSuccess ( string currentitem )
        {
            bool result = false;
            currentitem = currentitem . ToUpper ( );
            if ( ExpArgs . SearchSuccess == true || ExpArgs . SearchTerm == "" )
                return false;
            //if ( currentitem . Contains ( "BG-BG" ) )
            //    Debug. WriteLine ( $"" );
            //if ( Exactmatch )
            //{
            //    result = currentitem == ExpArgs . SearchTerm;
            //}

            //else
            //{
            if ( currentitem . Contains ( ExpArgs . SearchTerm ) )
                result = true;
            else
                result = false;
            //}

            return result;
        }
        private static bool CheckIsVisible ( string entry , bool showall , out bool HasHidden )
        {
            HasHidden = false;
            entry = entry . ToUpper ( );
            if ( showall == false )
            {
                foreach ( var item in ValidFiles )
                {
                    if ( entry . Contains ( item . ToUpper ( ) ) )
                    {
                        HasHidden = true;
                        return false;
                    }
                }
                return true;
            }
            return true;
        }

        public void LoadDrives ( TreeView tv , string drivetoload = "" )
        {
            bool ValidDrive = false;
            //            bool HasHiddenItems = false;
            string volabel = "";
            string DriveHeader = "";
            string Padding = "                 ";
            bool isvalid = false;
            tv . Items . Clear ( );
            //            listBox . Items . Clear ( );
            VolumeLabelsDict . Clear ( );
            LoadValidFiles ( );
            if ( drivetoload == "ALL" )
                drivetoload = "";
            foreach ( var drive in Directory . GetLogicalDrives ( ) )
            {
                ValidDrive = false;
                DriveHeader = "";
                if ( drivetoload . ToUpper ( ) != "" )
                {
                    if ( drivetoload . ToUpper ( ).Contains(drive . ToUpper ( ) ) == false)
                        continue;
                }
                //Add Drive to Treeview
                DriveInfo [ ] di = DriveInfo . GetDrives ( );
                foreach ( var item in di )
                {
                    if ( item . Name == drive )
                    {
                        if ( item . DriveType == DriveType . CDRom )
                        {
                            ValidDrive = true;
                            //isvalid = true;
                            DriveHeader = Padding . Substring ( 0 , 10 );
                            DriveHeader += "CdRom Drive";
                            string newlabel = " " + DriveHeader;
                            volabel = "   CdRom Drive (No CdRom)";
                            VolumeLabelsDict . Add ( drive , volabel );
                            if ( ShowVolumeLabels == true )
                            {
                                DriveHeader = $"    {volabel}";
                            }
                        }
                        else
                        {
                            List<string> directories = new List<string> ( );
                            GetDirectories ( item . ToString ( ) , out directories );
                            foreach ( var dir in directories )
                            {
                                //if ( CheckIsVisible ( dir . ToUpper ( ) , ShowAllFiles , out HasHidden ) == true )
                                //{
                                isvalid = true;
                                string newlabel = " " + item . VolumeLabel;
                                VolumeLabelsDict . Add ( drive , newlabel );
                                if ( ShowVolumeLabels == true )
                                {
                                    DriveHeader = $"    [{newlabel}]";
                                }
                                break;
                                //}
                            }
                            if ( isvalid )
                            {
                                if ( ShowVolumeLabels == true )
                                    DriveHeader = $"   [{item . VolumeLabel}]";
                                ValidDrive = true;
                            }
                            else
                                volabel = $"    [{item . VolumeLabel}]";
                        }
                        break;
                    }
                }
                if ( ValidDrive == true )
                {
                    var item = new TreeViewItem ( );
                    item . Header = drive + DriveHeader;
                    item . Tag = drive;
                    tv . Items . Add ( item );
                    // Add Dummy entry so we get an "Can be Opened" triangle icon
                    item . Items . Add ( "Loading" );
                    //     tvitems . Add ( item );
                }
                else
                {
                    var item = new TreeViewItem ( );
                    if ( ShowVolumeLabels == true )
                        item . Header = drive + volabel;
                    else
                        item . Header = drive + DriveHeader;
                    item . Tag = drive;
                    tv . Items . Add ( item );
                    item . Items . Add ( "Loading" );
                    //  tvitems . Add ( item );
                }
            }
            tv . UpdateLayout ( );
        }

        // Stored list of all Hidden/System file names so we can handle not showing the,
        public void LoadValidFiles ( )
        {
            ValidFiles . Add ( "BOOTMGR" );
            ValidFiles . Add ( "BOOTNXT" );
            ValidFiles . Add ( "BOOTSTAT" );
            ValidFiles . Add ( "RECOVERY" );
            ValidFiles . Add ( "BOOTNXT" );
            ValidFiles . Add ( "MEMTEST" );
            ValidFiles . Add ( "BOOTUWF" );
            ValidFiles . Add ( "BOOTVHD" );
            ValidFiles . Add ( "MEMTEST" );
            ValidFiles . Add ( "BOOT" );
            ValidFiles . Add ( "$GETCURRENT" );
            ValidFiles . Add ( "$WINDOWS" );
            ValidFiles . Add ( "$WINREAGENT" );
            ValidFiles . Add ( "CONFIG.MSI" );
            ValidFiles . Add ( "WINDOWS.OLD" );
            ValidFiles . Add ( ".BIN" );
            ValidFiles . Add ( "$WINRE_BACKUP" );
            ValidFiles . Add ( "RECYCLE" );
            ValidFiles . Add ( "SYSTEM VOLUME INFORMATION" );
            ValidFiles . Add ( "BACKUP_PARTITION" );
            ValidFiles . Add ( "BOOTSECT" );
        }
        private void ClearExpandArgs ( )
        {
            ExpArgs . tv = TestTree;
            ExpArgs . tvitem = null;
            ExpArgs . ExpandLevels = 0;
            ExpArgs . CurrentLevel = 1;
            ExpArgs . SearchTerm = "";
            ExpArgs . SearchActive = false;
            ExpArgs . Selection = 7;    // default to collapse
            ExpArgs . SearchSuccess = false;
            ExpArgs . MaxItems = 250;
            //            ExpArgs . ListResults = LISTRESULTS;
            ExpArgs . IsFullExpand = false;
            ExpArgs . Parent = null;
        }
        private void Close_Btn ( object sender , RoutedEventArgs e )
        {
            this . Close ( );
        }

        private void TestTree_Expanded ( object sender , RoutedEventArgs e )
        {
            TestTree_Expanded ( e . Source as TreeViewItem );
            if ( CancelExpand )
                return;
        }
    }
}
