using NewWpfDev. ViewModels;
using NewWpfDev. Views;

using NewWpfDev . Views;

using System;
using System . Collections . Generic;
using System . IO;
using System . Linq;
using System . Net;
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

namespace NewWpfDev
{

    #region Generic System wide structures and Definitions
    // All System wide Delegates are declared here...
    #region DELEGATE DECLARATIONS
 //   public delegate void LoadTableDelegate ( string Sqlcommand , string TableType , object bvm );
	//public delegate void LoadTableWithDapperDelegate ( string Sqlcommand , string TableType , object bvm , object Args);
	#endregion DELEGATE DECLARATIONS

	#region dummy classes
	public class EditDb
	{
	}
	public class SqlDbViewer
	{
	}
	public class DbSelector
	{
		public ListBox ViewersList;
	}
	public class BankDbView
	{
	}
	public class CustDbView
	{
	}
	public class DetailsDbView
	{
	}
	public class DragDropClient
	{
	}
	public class MultiViewer
	{
	}
	public class AllCustomers
	{
	}

	#endregion dummy classes

	#region My MessageBox Definitions

	public struct mb
	{
		static public int nnull = 0;
		static public int NNULL=0;
		static public int ok=1;
		static public int OK=1;
		static public int yes=2;
		static public int YES=2;
		static public int no=3;
		static public int NO=3;
		static public int cancel=4;
		static public int CANCEL=4;
		static public int iconexclm=5;
		static public int ICONEXCLM=5;
		static public int iconwarn=6;
		static public int ICONWARN=6;
		static public int iconerr=7;
		static public int ICONERR=7;
		static public int iconinfo=8;
		static public int ICONINFO=8;
	}

	public struct MB
	{
		static public int nnull = 0;
		static public int NNULL=0;
		static public int ok=1;
		static public int OK=1;
		static public int yes=2;
		static public int YES=2;
		static public int no=3;
		static public int NO=3;
		static public int cancel=4;
		static public int CANCEL=4;
		static public int iconexclm=5;
		static public int ICONEXCLM=5;
		static public int iconwarn=6;
		static public int ICONWARN=6;
		static public int iconerr=7;
		static public int ICONERR=7;
		static public int iconinfo=8;
		static public int ICONINFO=8;
	}

	/// <summary>
	/// output parameters (return values) for my Message Box
	/// </summary>
	public struct Dlgresult
	{
		static public bool result;
		static public int returnint;
		static public string returnstring;
		static public string returnerror;
		static public object obj;
	}

	#endregion My MessageBox Definitions

	#region My MessageBox argument structuress
	/// <summary>
	/// Input parameters for my Message Box
	/// </summary>
	public struct DlgInput
	{
		//static public Msgbox MsgboxWin;
		//static public Msgboxs MsgboxSmallWin;
		//static public Msgboxs MsgboxMinWin;
		//		public static SysMenu sysmenu;
		static public bool isClean;
		static public bool resultboolin;
		static public bool UseDarkMode;
		static public bool resetdata;
		static public bool UseIcon;
		static public int intin;
		static public int returnint;
		static public string stringin;
		static public object obj;
		static public string iconstring;
		static public Thickness thickness;
		static public Image image;
		static public Brush dlgbackground;
		static public Brush dlgforeground;
		static public Brush btnbackground;
		static public Brush btnforeground;
		static public Brush Btnborder;
		static public Brush Btnmousebackground;
		static public Brush Btnmouseforeground;
		static public Brush defbtnbackground;
		static public Brush defbtnforeground;
		// Dark mode
		static public Brush BtnborderDark;
		static public Brush btnforegroundDark;
		static public Brush btnbackgroundDark;
		static public Brush defbtnforegroundDark;
		static public Brush defbtnbackgroundDark;
		static public Brush mouseborderDark;
		static public Brush mousebackgroundDark;
		static public Brush mouseforegroundDark;
		static public bool ShowButtonHitMaster;
		static public bool ShowButtonHit;
		static public Thickness BorderSizeNormal;                    // Normal display shadow
		static public Thickness BorderSizeDefault;            // Mouse over / (current Default) display
	}
	#endregion My MessageBox arguments

	#region  Cookies handling
	public struct defvars
	{
		public static Uri  cookierootname=new Uri(@"C:\Cookie");
		public static String CookieDictionarypath=@"J:\users\ianch\documents\CookieDictionary.ser";
		public static String CookieCollectionpath=@"J:\users\ianch\documents\CookieCollection.ser";
		public static Dictionary<string , string> Cookiedictionary;
		public static CookieCollection  Cookiecollection;
		public static  int NextCookieIndex = 0;
		public static bool CookieAdded=false;
		public static bool FullViewer=false;
	}
	#endregion  Cookies handling

	//public struct TreeExplorer
	//{
	//	public static ExplorerClass Explorer = new ExplorerClass();
	//	public static DirectoryInfo DirInfo = new DirectoryInfo(@"C:\\");
	//}
	#region My GridColors arguments
	public struct GridControl
	{
		public string Controller { get; set; }
		public Brush transparency { get; set; }
		public Brush normalBackground { get; set; }
		public Brush normalForeground { get; set; }
		public Brush selectedBackground { get; set; }
		public Brush selectedForeground { get; set; }
		public Brush normalMouseBackground { get; set; }
		public Brush normalMouseForeground { get; set; }
		public Brush selectedMouseBackground { get; set; }
		public Brush selectedMouseForeground { get; set; }
		public double fontsize { get; set; }
	}

	#endregion My GridColors arguments

	// structure to hold all arguments required by DapperSuport data loading calls
	public struct DbLoadArgs
	{
		public string dbname;
		public string Orderby;
		public string Conditions;
		public bool wantSort;
		public bool wantDictionary;
		public bool Notify;
		public string Caller;
		public int [ ] args;
	}
	#endregion Generic System wide structures and Definitions

	//=======================//
	// Start of Mainwindow class
	//=======================//
	public partial class MainWindow : Window
	{
        //public static GridViewer gv = new GridViewer ( );
        //// Global pointers to Viewmodel classes
        public static BankAccountViewModel bvm = null;
        public static CustomerViewModel cvm = null;
        public static DetailsViewModel dvm = null;

        //public static ExplorerClass Txplorer;

        public MainWindow ( )
		{
			InitializeComponent ( );
			this . Top = 100;
			this . Left = 100;
			WpfLib1 . Utils .SetupWindowDrag ( this );
            Flags . CurrentConnectionString = ( string )Properties . Settings . Default [ "BankSysConnectionString" ];
            Flags . FlowdocCrMultplier = 3.0;
            Flags . UseFlowdoc = Properties . Settings . Default . UseFlowDoc . ToUpper() == "TRUE" ? true : false;
            Properties . Settings . Default . Save();
            Flags . UseScrollView = Properties . Settings . Default . UseScrollViewer . ToUpper() == "TRUE" ? true : false;
            Properties . Settings . Default . Save();
            Flags . ReplaceFldNames = Properties . Settings . Default . ReplaceFldNames . ToUpper() == "TRUE" ? true : false;
            Properties . Settings . Default . Save();
            Flags . UseMagnify = Properties . Settings . Default . UseMagnify;
            Properties . Settings . Default . Save();

            string startpath = Properties . Settings . Default . AppRootPath;
            if ( startpath == "" )
            {
                startpath = SupportMethods . GetCurrentApplicationFullPath();
                Properties . Settings . Default . AppRootPath = startpath;
                Properties . Settings . Default . Save();
                WpfLib1 . Utils .SaveProperty("AppRootPath" , startpath);
            }

        }

//		private void button1_Click ( object sender , RoutedEventArgs e )
//		{
//			Datagrids dg = new Datagrids();
//			dg . Show ( );
//		}

//		private void button2_Click ( object sender , RoutedEventArgs e )
//		{
//			Listviews lv = new Listviews();
//			lv. Show ( );
//		}

//		private void button3_Click ( object sender , RoutedEventArgs e )
//		{
//			TreeViews tv = new TreeViews();
//#pragma warning disable CS0168 // The variable 'ex' is declared but never used
//			try
//			{
//				tv . Show ( );
//			}
//            catch ( Exception ex ) { Debug. WriteLine ("TreeViews already open"); }
//#pragma warning restore CS0168 // The variable 'ex' is declared but never used
//		}

//		private void button4_Click ( object sender , RoutedEventArgs e )
//		{
//			VmTest vmt = new VmTest();
//			vmt . Show ( );
//		}

//		private void button5_Click ( object sender , RoutedEventArgs e )
//		{
//			GenericMvvmWindow gmvvm = new GenericMvvmWindow();
//			gmvvm . Show ( );
//		}

//		private void button6_Click ( object sender , RoutedEventArgs e )
//		{
//			MvvmDataGrid bg = new MvvmDataGrid();
//			bg . Show ( );
//		}

//		private void button7_Click ( object sender , RoutedEventArgs e )
//		{
//			MvvmUserTest mut = new MvvmUserTest();
//			mut . Show ( );
//		}

//		private void button8_Click ( object sender , RoutedEventArgs e )
//		{
//			ModernViews ga = new ModernViews ( );
//			ga . Show ( );
//		}

		//private void button9_Click ( object sender , RoutedEventArgs e )
		//{
		//	SysConfig scfg = new         SysConfig();
		//	scfg . Show ( );

		//}

		private void button10_Click ( object sender , RoutedEventArgs e )
		{
			this . Close ( );
			Application . Current . Shutdown ( );
		}

		private void Window_PreviewKeyDown ( object sender , KeyEventArgs e )
		{
			//if ( e . Key == Key . D )
			//	button1_Click ( sender , null );
			//else if ( e . Key == Key . L )
			//	button3_Click ( sender , null );
			//else if ( e . Key == Key . T )
			//	button3_Click ( sender , null );
			//else if ( e . Key == Key . V )
			//	button4_Click ( sender , null );
			//else if ( e . Key == Key . M )
			//	button5_Click ( sender , null );
			//else if ( e . Key == Key . G )
			//	button6_Click ( sender , null );
			//else if ( e . Key == Key . U )
			//	button7_Click ( sender , null );
			//else if ( e . Key == Key . Enter )
			//	button7_Click ( sender , null );
			//else if ( e . Key == Key . Escape )
			//	Application . Current . Shutdown ( );
		}

        private void Button10_Click (object sender , RoutedEventArgs e)
        {
            Application . Current . Shutdown();
        }

        private void Button16_Click (object sender , RoutedEventArgs e)
        {
            //UserCtrlHost uch = new UserCtrlHost();
            //uch . Show();
            //TextBoxwithDataError tbwe = newTextBoxwithDataError();
        }

        private void Button1_Click (object sender , RoutedEventArgs e)
        {
            Datagrids dg = new Datagrids();
            dg . Show();
        }

        private void Button2_Click (object sender , RoutedEventArgs e)
        {
            Listviews lv = new Listviews();
            lv . Show();
        }

        private void Button3_Click (object sender , RoutedEventArgs e)
        {
            TreeViews tvs = new TreeViews();
            tvs . Show();
        }

        private void Button6_Click (object sender , RoutedEventArgs e)
        {
            MvvmContainerWin mvvm = new MvvmContainerWin();
            mvvm . Show();
        }

        private void Button7_Click (object sender , RoutedEventArgs e)
        {
            MvvmUserTest mut = new MvvmUserTest();
            mut . Show();
        }

        private void Button13_Click (object sender , RoutedEventArgs e)
        {
            FourwaySplitViewer fv = new FourwaySplitViewer();
            fv . Show();
        }

        private void Button21_Click (object sender , RoutedEventArgs e)
        {
            YieldWindow yw = new YieldWindow();
            yw . Show(); 
        }

        private void Button20_Click (object sender , RoutedEventArgs e)
        {
            Tabview ss = new Tabview();
            ss . Show();

        }

        //        private void button11_Click ( object sender, RoutedEventArgs e )
        //        {
        //			ExpanderTest et = new ExpanderTest ( );
        //			et. Show ( );
        //        }

        //        private void button12_Click ( object sender, RoutedEventArgs e )
        //        {
        //			SplitViewer sv = new SplitViewer ( );
        //			sv. Show ( );
        //        }

        //        private void button13_Click ( object sender, RoutedEventArgs e )
        //        {
        //			FourwaySplitViewer st = new FourwaySplitViewer( );
        //			st . Show ( );
        //        }

        //        private void button14_Click ( object sender, RoutedEventArgs e )
        //        {
        //			Mouse . OverrideCursor = Cursors . Wait;
        //			InterWinComms iw= new InterWinComms ( );
        //            iw . Show ( );
        //		}

        //		private void button15_Click ( object sender, RoutedEventArgs e )
        //        {
        //			Menutest mt = new Menutest ( );
        //			mt . Show ( );
        //        }

        //        private void button16_Click ( object sender , RoutedEventArgs e )
        //        {
        //			DataErrorWindow hit = new DataErrorWindow ( );
        //			hit . Show ( );
        //        }

        //        private void button17_Click ( object sender , RoutedEventArgs e )
        //        {
        //			MvvmContainerWin ucm = new MvvmContainerWin ( );
        //			ucm . Show ( );
        //        }

        //        private void button19_Click ( object sender , RoutedEventArgs e )
        //        {
        //			//get a pointer to class view mdel
        //			//UserCtrlViewModel ucvm = new UserCtrlViewModel ( );			
        //			UcHostWindow uch = new UcHostWindow ( );
        //			uch . Show ( );
        //			uch . LoadHostWindow ( );
        //		}

        //        private void button18_Click ( object sender , RoutedEventArgs e )
        //        {
        //			TabViewInfo tvvi = new TabViewInfo ("NewWPfDevInfo.txt" );
        //			tvvi . Show ( );
        //		}

        //		private void button20_Click ( object sender , RoutedEventArgs e )
        //        {
        ////			LoadPopup pu= new LoadPopup ( );
        ////			pu . Show ( );

        //			Tabview ss = new Tabview( );
        //			ss . Show ( );

        //		}

        //        private void button21_Click ( object sender , RoutedEventArgs e )
        //        {
        //			YieldWindow yw = new YieldWindow ( );
        //			yw . Show ( );
        //        }
    }
}
