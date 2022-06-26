using NewWpfDev . UserControls;
using NewWpfDev . ViewModels;

using System;
using System . Collections . Generic;
using System . Data;
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
using System . Windows . Shapes;



namespace NewWpfDev . Views
{
	/// <summary>
	/// Interaction logic for MvvmUserTest.xaml
	/// </summary>
	public partial class MvvmUserTest : Window
	{
		// Host window
		// Must declare the event
#pragma warning disable CS0067 // The event 'MvvmUserTest.Click' is never used
		public event RoutedEventHandler Click;
#pragma warning restore CS0067 // The event 'MvvmUserTest.Click' is never used
		StdDataUserControl SduCtrl { get; set; }
		MulltiDbUserControl MduCtrl { get; set; }
		ListBoxUserControl LbuCtrl { get; set; }
		Ucontrol1 U1ctrl { get; set; }
		public double Col1width=260;
#pragma warning disable CS0414 // The field 'MvvmUserTest.ButtonPanelMaxOffset' is assigned but its value is never used
		private double ButtonPanelMaxOffset = 155;
#pragma warning restore CS0414 // The field 'MvvmUserTest.ButtonPanelMaxOffset' is assigned but its value is never used
		private double ButtonPanelLeftOffset = 265;
#pragma warning disable CS0414 // The field 'MvvmUserTest.GridTopOffset' is assigned but its value is never used
		private double GridTopOffset = 300;
#pragma warning restore CS0414 // The field 'MvvmUserTest.GridTopOffset' is assigned but its value is never used
		string CurrentClient = "STD";
		//private double ButtonPanelLeftOffset = 245;
		public MvvmUserTest ( )
		{
			InitializeComponent ( );
			Utils . SetupWindowDrag ( this );

			this . DataContext = this;
			SduCtrl = stddatagrid;
			SduCtrl . SetParent ( this );
			MduCtrl = Multigrid;
			U1ctrl = Ucontrol1;
			LbuCtrl = listboxCtrl;
			this . DataContext = this;
			// setup the user control
			//SduCtrl . Visibility = Visibility . Visible;
			OpenStdControl ( this , null );
			//this . SizeChanged += MvvmUserTest_SizeChanged;
			UserTestWindow . SizeChanged += MvvmUserTest_SizeChanged;
			//this . Height += 1;
			//this . Width += 10;
			//bguv . canvas . Width = canvas . Width;
			//Setup handler to handle click event  from UserControl
			//			Click += new RoutedEventHandler ( CloseThisWindow );
			this . Title = "MVVM User Control Host Demonstration System - Standard Db's Viewer";
		}

		private void UserTestWindow_Loaded ( object sender , RoutedEventArgs e )
		{
			//Mouse . SetCursor ( Cursors . Wait);
			//OpenMultiDbControl ( null, null);
			//Mouse . OverrideCursor = Cursors . Arrow;
			}

		private void MvvmUserTest_SizeChanged ( object sender , SizeChangedEventArgs e )
		{
			// Handlle resizing of client user controls
			double height = 0;
			double width = 0;
			if ( e == null )
			{
				height = this . Height;
				width = this.Width;
			}
			else
			{
				height = e . NewSize . Height;
				width=(double) e . NewSize . Width;
			}
			this . Grid1 . Height = height;
			this . canvas . Height=height;
			this . canvas . Width = width;

			// Std Viewer - HEIGHT - All Working
			if ( CurrentClient == "STD")
			{
				SduCtrl . Height = height - 1;
				SduCtrl . MAINgrid_Grid1 . Height = height;
				SduCtrl . dgcanvas . Height = height - 20;
				SduCtrl . dataGrid . Height = height - 100;
				// Std Viewer - WIDTH
				SduCtrl . Width = width - (ButtonPanelLeftOffset - 10 );
				SduCtrl . dgcanvas . Width = SduCtrl . Width + 60;// - ButtonPanelLeftOffset;
				SduCtrl . MAINgrid_Grid1 . Width = this . canvas . Width +160;
				SduCtrl . dataGrid . Width = SduCtrl . dgcanvas . Width - ButtonPanelLeftOffset;
				( SduCtrl . Top_rightcol as FrameworkElement ) . SetValue ( Canvas . LeftProperty , ( double ) SduCtrl . dataGrid . Width);
				SduCtrl . Currentdb . Width = SduCtrl . dgcanvas . Width - ButtonPanelLeftOffset;
			}
			else if ( CurrentClient == "MULTI" )
			{
				////Handle Multi grid positioning
				Multigrid . Height = height - 35;
				Multigrid . MAIN_Grid1 . Height = height;
				Multigrid . bgcanvas . Height = height - 30;
				Multigrid . BankDataGrid . Height = height - 50;
				( Multigrid . BankDataGrid as FrameworkElement ) . SetValue ( Canvas . TopProperty , ( double ) 0 );
				// Std Viewer - WIDTH
				Multigrid . Width = width - ( ButtonPanelLeftOffset - 10 );
				Multigrid . bgcanvas . Width = Multigrid . Width;// - ButtonPanelLeftOffset;
				Multigrid . MAIN_Grid1 . Width = this . canvas . Width - ButtonPanelLeftOffset;
				Multigrid . BankDataGrid . Width = Multigrid . bgcanvas . Width - ButtonPanelLeftOffset;
				( Multigrid . bgcanvas as FrameworkElement ) . SetValue ( Canvas . LeftProperty , ( double ) 0 );
				( Multigrid . BankDataGrid as FrameworkElement ) . SetValue ( Canvas . LeftProperty , ( double ) 0 );
				( Multigrid . ButtonGroup as FrameworkElement ) . SetValue ( Canvas . LeftProperty , ( double ) Multigrid . BankDataGrid . Width );
				Debug. WriteLine ( $"MG Cv.Width {Multigrid . bgcanvas . Width },  " );
			}
			else if ( CurrentClient == "LISTBOX" )
			{
				LbuCtrl . listbox1 . Visibility = Visibility . Visible;
				////Handle Multi grid positioning
				double val = 40;
				( LbuCtrl . listbox1 as FrameworkElement ) . SetValue ( Canvas . TopProperty , ( double ) -10 );
				LbuCtrl . Height = height-val;
				double newheight = height-val;
				//( LbuCtrl . MainGrid as FrameworkElement ) . SetValue ( Canvas . TopProperty , ( double ) 0 );
				//( LbuCtrl . ListboxGroup as FrameworkElement ) . SetValue ( Canvas . TopProperty , ( double ) 0 );
				//( LbuCtrl . listbox1 as FrameworkElement ) . SetValue ( Canvas . TopProperty , ( double ) -30 );
				LbuCtrl . MainGrid . Height = newheight +10;
				LbuCtrl . ListboxGroup . Height = newheight + 10;
				LbuCtrl . listbox1 . Height = newheight - 10;
				( LbuCtrl  as FrameworkElement ) . SetValue ( Canvas . TopProperty, ( double ) 0 );
				// Std Viewer - WIDTH
				LbuCtrl . Width = width - ( ButtonPanelLeftOffset - 10 );
				LbuCtrl . MainGrid . Width = this . canvas . Width - ButtonPanelLeftOffset;
				( LbuCtrl . listbox1 as FrameworkElement ) . SetValue ( Canvas . LeftProperty , ( double )0 );
			}
			else
			{
				// Height is working 18/3/22
				//Dummy
				Thickness th = new Thickness();
				( U1ctrl . Caption as FrameworkElement ) . SetValue ( Canvas . TopProperty , ( double ) 10 );
				( U1ctrl . listbox1 as FrameworkElement ) . SetValue ( Canvas . TopProperty , ( double ) 35 );
				( U1ctrl . UiButtons as FrameworkElement ) . SetValue ( Canvas . TopProperty , ( double ) 35 );
				//( U1ctrl . listbox1 as FrameworkElement ) . SetValue ( Canvas . LeftProperty , ( double ) 10 );
				U1ctrl . Height = height -50;//+ 25;
				U1ctrl . MainGrid . Height = height - 65;
				U1ctrl . uccanvas . Height = height - 5;
				U1ctrl . listbox1 . Height = height - 75;
				U1ctrl . UiButtons . Height = height - 55;
				th = U1ctrl . Margin;
				th . Top = 0;
				U1ctrl . Margin= th;
				// Reset Image Hieght as image  file size may be massive !!!
				U1ctrl . image1 . Height = U1ctrl . listbox1 . Height - 10;
				U1ctrl . image1.Margin = th;
				(U1ctrl.image1 as FrameworkElement).SetValue(Canvas.TopProperty, (double)36);

				//( U1ctrl . image1 as FrameworkElement ) . SetValue ( Canvas. TopProperty , ( double ) 0 );
				// Std Viewer - WIDTH
				U1ctrl. uccanvas . Width = width + 35;// - ButtonPanelLeftOffset;
				U1ctrl . Width = width - 10;// + U1ctrl . UiButtons.Width;
				U1ctrl . listbox1 . Width = U1ctrl . Width-485;
				U1ctrl . UiButtons . Margin = th;
				// Reset Image Width as image  file size may be massive !!!
				U1ctrl . image1 . Width = U1ctrl . listbox1 . Width;
			}
			// Handle the windows own buttons
			ButtonPanel . VerticalAlignment = VerticalAlignment . Top;
			if ( this . WindowState == WindowState . Maximized )
			{
				Debug. WriteLine ( $"Canvas Max panel={canvas . Width}, Button AWidth={ButtonPanel . ActualWidth}" );
				canvas . Width = this . Width;
				double cheight = canvas.ActualHeight;
				(ButtonPanel as FrameworkElement ) . SetValue ( Canvas . TopProperty , ( double ) 0 );
//				( ButtonPanel as FrameworkElement ) . SetValue ( Canvas . TopProperty , ( double ) cheight +( ButtonPanel.Height - ButtonPanelMaxOffset ) );
				( ButtonPanel as FrameworkElement ) . SetValue ( Canvas . LeftProperty , ( double ) this . ActualWidth - ButtonPanelLeftOffset );
			}
			else
			{
				// Handle button panel resizing, including buttons positioning
				(ButtonPanel as FrameworkElement ) . SetValue ( Canvas . TopProperty , ( double ) 0 );
				//				( ButtonPanel as FrameworkElement ) . SetValue ( Canvas . WidthProperty , ( double ) Width - ButtonPanelLeftOffset - 0 );
				( ButtonPanel as FrameworkElement ) . SetValue ( Canvas . HeightProperty , ( double ) Height - 37 );
				( ButtonPanel as FrameworkElement ) . SetValue ( Canvas . LeftProperty , ( double ) Width - ButtonPanelLeftOffset +10 );
				
				BtnPanelGrid .Width = ButtonPanel.Width + 20;
				CloseBtn . Width = BtnPanelGrid . Width + 0;
				Thickness th = new Thickness();
				th . Top =Height - 95;// - (CloseBtn . Height + 50);
				th . Left = th . Right = th.Bottom = 0;
				CloseBtn . Margin = th;
			}

		}

		//private void OnClick ( object sender , MouseButtonEventArgs e )
		//{
		//	if ( this . Click != null )
		//	{
		//		this . Click ( this , e );
		//	}
		//}


		private void OpenStdControl ( object sender , RoutedEventArgs e )
		{
			CurrentClient = "STD";
			if ( canvas . Height == 0 )
				return;
			MduCtrl . Visibility = Visibility . Hidden;
			U1ctrl . Visibility = Visibility . Hidden;
			LbuCtrl . Visibility = Visibility . Hidden;
			this . Height += 1;
			MvvmUserTest_SizeChanged ( sender , null );
			this . UpdateLayout ( );
			this. Refresh ( );
			if ( SduCtrl . Visibility == Visibility . Hidden )
			{
				SduCtrl . Visibility = Visibility . Visible;
				SduCtrl . Refresh ( );
				SduCtrl . Width -= 1;
				SduCtrl . UpdateLayout ( );
				//SduCtrl . Height = this . canvas . Height - 70;//= canvas . Width;
				//SduCtrl . Width = this . canvas . Width - 300;
				//SduCtrl . UpdateLayout ( );
				this . Title = "MVVM User Control Host Demonstration System - Standard Db's Viewer";
			}
			else
				SduCtrl . Visibility = Visibility . Hidden;
		}

		private void OpenMultiDbControl( object sender , RoutedEventArgs e )
		{
			CurrentClient = "MULTI";
			U1ctrl . Visibility = Visibility . Hidden;
			SduCtrl . Visibility = Visibility . Hidden;
			LbuCtrl . Visibility = Visibility . Hidden;
			MduCtrl . Width = 860;
			this . Height += 1;
			this . UpdateLayout ( );
			if ( MduCtrl . Visibility == Visibility . Hidden )
			{
				MvvmUserTest_SizeChanged ( sender , null );
				MduCtrl . Visibility = Visibility . Visible;
				MduCtrl . Width -= 1;
				MduCtrl . UpdateLayout ( );
				MduCtrl . Refresh ( );
				//MduCtrl . Height= this . canvas . Height - 70;
				//MduCtrl . Width = this . canvas . Width - 300;
				if ( MduCtrl . DbMain . Items . Count == 0 )
					MduCtrl . InitialLoad ( );
				//MduCtrl . Width += 1;
				MduCtrl . UpdateLayout();
				this . Title = "MVVM User Control Host Demonstration System - Multi Db Viewer";
			}
			else
				MduCtrl . Visibility = Visibility . Hidden;
		}

		private void OpenDummy ( object sender , RoutedEventArgs e )
		{
			CurrentClient = "DUMMY";
			MduCtrl . Visibility = Visibility . Hidden;
			SduCtrl . Visibility = Visibility . Hidden;
			LbuCtrl . Visibility = Visibility . Hidden;
			U1ctrl . Width = 860;
			this . Height += 1;
			this . UpdateLayout ( );
			if ( U1ctrl . Visibility == Visibility . Hidden )
			{
				U1ctrl . Visibility = Visibility . Visible;
				U1ctrl . Width -= 1;
				MvvmUserTest_SizeChanged ( sender , null );
				U1ctrl . UpdateLayout ( );
				U1ctrl . Refresh();
				//U1ctrl . Height = this . canvas . Height - 70;//= canvas . Width;
				//U1ctrl . Width = canvas . Width - 300;
				//				U1ctrl . uccanvas . Width = this.canvas.Width - 220;
				//UiButtons.
				U1ctrl . UpdateLayout ( );
				this . Title = "MVVM User Control Host Demonstration System - Dummy Grid Viewer";
			}
			else
			{
				U1ctrl . Visibility = Visibility . Hidden;
			}
		}


		private void OpenListboxControl ( object sender , RoutedEventArgs e )
		{
			MduCtrl . Visibility = Visibility . Hidden;
			SduCtrl . Visibility = Visibility . Hidden;
			U1ctrl . Visibility = Visibility . Hidden;
			if ( LbuCtrl . Visibility == Visibility . Visible )
				LbuCtrl . Visibility = Visibility . Hidden;
			else
			{
				listboxCtrl . Visibility = Visibility . Visible;
				LbuCtrl . Width += 1;
				CurrentClient = "LISTBOX";
				MvvmUserTest_SizeChanged ( sender , null);
				LbuCtrl . UpdateLayout ( );
				LbuCtrl . Refresh ( );
			}
			CurrentClient = "LISTBOX";
			if ( LbuCtrl . listbox1 . Items . Count == 0 )
				LbuCtrl . InitialLoad ( );
			this . Title = "MVVM User Control Host Demonstration System - Listbox Db contents Viewer";

		}
		private void CloseThisWindow ( object sender , RoutedEventArgs e )
		{
			WindowCollection  v = Application .Current.Windows;
			foreach ( Window item in v )
			{
				if ( item . ToString ( ) . Contains ( "MvvmUserTest" ) )
				{
					MessageBoxResult res = MessageBox . Show ( "Close App down entirely ?" , "Application Closedown Options" ,
						MessageBoxButton .YesNoCancel, MessageBoxImage.Question, MessageBoxResult . Yes );
					if ( res == MessageBoxResult . Cancel )
						return;
					if ( res == MessageBoxResult . Yes )
						Application . Current . Shutdown ( );
					else
						item . Close ( );
					break;
				}
			}
		}
		private void UserTestWindow_ContentRendered ( object sender , EventArgs e )
		{
		}

	}
}
