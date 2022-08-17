using System;
using System . ComponentModel . DataAnnotations;
using System . Diagnostics;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;

using DocumentFormat . OpenXml . Drawing . Charts;

using NewWpfDev . UserControls;

namespace NewWpfDev . Views
{
    /// <summary>
    /// Interaction logic for GenericSelectBoxControl.xaml
    /// </summary>
    public partial class GenericSelectBoxControl : UserControl
    {
        public static event EventHandler<SelectionArgs> ListSelection;

        public static Canvas GcCanvas;
        public static GenericSelectBoxControl ourlist { get; set; } = null;
        public static GenericGridControl gridctrl { get; set; } = null;

        public static bool MouseCaptured = false;
        public static bool ListResizing = false;
        public double left = 0, top = 0, bottom = 0, height = 0, width = 0;

        public double CpFirstXPos = 0;
        public double CpFirstYPos = 0;
        public double FdLeft = 0;
        public double FdTop = 0;
        public double FdBottom = 0;
        public double FdHeight = 0;
        public double FdWidth = 0;
        public int BorderSelected = 0;
        public static bool Isopen { get; set; } = false;
        int counter { get; set; } = 0;

        #region Dependency Properties
        new public bool IsVisible
        {
            get { return ( bool ) GetValue ( IsVisibleProperty ); }
            set { SetValue ( IsVisibleProperty , value ); }
        }
        new public static readonly DependencyProperty IsVisibleProperty =
            DependencyProperty . Register ( "IsVisible" , typeof ( bool ) , typeof ( GenericSelectBoxControl ) , new PropertyMetadata ( false ) );
        public GenericGridControl GenCtrl
        {
            get { return ( GenericGridControl ) GetValue ( GenCtrlProperty ); }
            set { SetValue ( GenCtrlProperty , value ); }
        }
        public static readonly DependencyProperty GenCtrlProperty =
            DependencyProperty . Register ( "GenCtrl" , typeof ( GenericGridControl ) , typeof ( GenericSelectBoxControl ) , new PropertyMetadata ( ( GenericGridControl ) null ) );

        #endregion Dependency Properties


        public GenericSelectBoxControl ( )
        {
            InitializeComponent ( );
            // this is LBSELECTOR (Name for the entire control)
            ourlist = this;
            ( ourlist as FrameworkElement ) . SetValue ( Canvas . LeftProperty , ( double ) 20 );
            ( ourlist as FrameworkElement ) . SetValue ( Canvas . TopProperty , ( double ) 20 );
            border . Width = 400;
            border . Height = 400;
            ourlist . Width = border . Width;
            ourlist . Height = border . Height;
            top = left = 20;
            //Debug . WriteLine ( $"\nINIT left= 50, top=20" );
            SetValue ( IsVisibleProperty , true );
            this . IsVisibleChanged += GenericSelectBoxControl_IsVisibleChanged;
            Isopen = true;
        }

        private void GenericSelectBoxControl_IsVisibleChanged ( object sender , DependencyPropertyChangedEventArgs e )
        {
            // try to center popup when making it visible AGAIN
            if ( this . Visibility == Visibility . Visible )
            {
                ourlist = sender as GenericSelectBoxControl;
                if ( ( left == 0 && top == 0 ) || ( ourlist . Height < 350 || ourlist . Width <= 400 ) )
                {
                    ( ourlist as FrameworkElement ) . SetValue ( Canvas . LeftProperty , ( double ) 20 );
                    ( ourlist as FrameworkElement ) . SetValue ( Canvas . TopProperty , ( double ) 20 );
                    border . Width = 400;
                    border . Height = 400;
                    ourlist . Width = border . Width;
                    ourlist . Height = border . Height;
                    top = left = 20;
                    height = ourlist . Height;
                    width = ourlist . Width;
                }
            }
            else
            {
                if ( ourlist != null )
                {
                    double ourheight = this . ActualHeight;
                    double ourwidth = this . ActualWidth;
                    height = ourheight;
                    width = ourwidth;

                    double horposition = ( GcCanvas . ActualWidth / 2 ) - ( ourwidth / 2 );
                    double vertposition = ( GcCanvas . ActualHeight / 2 ) - ( ourheight / 2 );
                    if ( horposition < 20 )
                        horposition = 20;
                    if ( horposition + ( ourwidth + 40 ) > GcCanvas . ActualWidth )
                        horposition = GcCanvas . ActualWidth - ( ourwidth + 40 );
                    if ( vertposition < 5 )
                        vertposition = 5;
                    if ( ( vertposition + ( ourheight ) ) > GcCanvas . ActualHeight )
                        vertposition = GcCanvas . ActualHeight - ( ourheight );
                    ( border as FrameworkElement ) . SetValue ( Canvas . LeftProperty , ( double ) horposition );
                    ( border as FrameworkElement ) . SetValue ( Canvas . TopProperty , ( double ) vertposition );
                    top = vertposition;
                    left = horposition;
                }
            }
        }
        private void IsLoaded ( object sender , RoutedEventArgs e )
        {
            Isopen = true;
            ( ourlist as FrameworkElement ) . SetValue ( Canvas . LeftProperty , ( double ) 20 );
            ( ourlist as FrameworkElement ) . SetValue ( Canvas . TopProperty , ( double ) 20 );
            border . Width = 400;
            border . Height = 400;
            ourlist . Width = border . Width;
            ourlist . Height = border . Height;
            // Get our parent Canvas
            GenCtrl = DapperGenericsLib . Utils . FindVisualParent<GenericGridControl> ( sender as DependencyObject );
            GcCanvas = this . Parent as Canvas;
            Debug . WriteLine ( $"\n *** LOADED *** left= 20, top=20" );
            Mouse . OverrideCursor = Cursors . Arrow;

        }
        private void cancelbtn ( object sender , RoutedEventArgs e )
        {
            // Cancel fontfamily listbox
            this . Visibility = Visibility . Collapsed;
            SetValue ( IsVisibleProperty , false );
            Isopen = false;
            this . ReleaseMouseCapture ( );
            MouseCaptured = false;
            counter = 0;
        }
        private void selectbtn ( object sender , RoutedEventArgs e )
        {
            // select fontfamily from listbox and collapse our listbox
            ListBox lb = sender as ListBox;
            SelectionArgs args = new SelectionArgs ( );
            args . selection = listbox . SelectedItem . ToString ( );
            ListSelection . Invoke ( sender , args );
            this . Visibility = Visibility . Collapsed;
            SetValue ( IsVisibleProperty , false );
        }

        #region Font Listbox  move/sizing OUTER
        #region Font Listbox  (mainarea) move/sizing INNER

        private void titlebar_PreviewMouseLeftButtonDown ( object sender , System . Windows . Input . MouseButtonEventArgs e )
        {
            TextBlock Titlebar = e . OriginalSource as TextBlock;
            if ( Titlebar == null )
                return;
            GcCanvas = this . Parent as Canvas;
            ourlist = GetThisControl ( sender );
            //we can use this to position the control in the parent canvas
            ourlist = DapperGenericsLib . Utils . FindVisualParent<GenericSelectBoxControl> ( sender as DependencyObject );

            // getb our enclosing canvas
            GcCanvas . Height = GenCtrl . ActualHeight;
            // save left/top  offset of the mouse in Titlebar
            CpFirstXPos = e . GetPosition ( ( sender as FrameworkElement ) as FrameworkElement ) . X;
            CpFirstYPos = e . GetPosition ( ( sender as FrameworkElement ) as FrameworkElement ) . Y;
            // get current posiion (of titlebar really)
            left = e . GetPosition ( ( GcCanvas as FrameworkElement ) as FrameworkElement ) . X;
            top = e . GetPosition ( ( GcCanvas as FrameworkElement ) as FrameworkElement ) . Y;
            bottom = top + border . ActualHeight;
            counter = 0;
            MouseCaptured = this . CaptureMouse ( );
            Debug . WriteLine ( $"Left Button Down top={top}, Height={ourlist . ActualHeight}" );
            Isopen = true;
        }
        private void titlebar_PreviewMouseLeftButtonUp ( object sender , System . Windows . Input . MouseButtonEventArgs e )
        {
            if ( MouseCaptured == false )
            {
                counter = 0;
                Mouse . OverrideCursor = Cursors . Arrow;
                return;
            }
            Isopen = false;
            counter = 0;
            this . ReleaseMouseCapture ( );
            MouseCaptured = false;
            Mouse . OverrideCursor = Cursors . Arrow;
        }
        private void titlebar_MouseEnter ( object sender , MouseEventArgs e )
        {
            Mouse . OverrideCursor = Cursors . Hand;
        }
          private void titlebar_MouseLeave ( object sender , MouseEventArgs e )
        {
            this . ReleaseMouseCapture ( );
            MouseCaptured = false;
            Mouse . OverrideCursor = Cursors . Arrow;
        }
        private void titlebar_MouseMove( object sender , MouseEventArgs e )
        {
            // font family popup mousemove
            if ( MouseCaptured == false || Isopen == false ) return;
            if ( e . LeftButton == MouseButtonState . Pressed )
                Mouse . OverrideCursor = Cursors . SizeAll;
            else
            {
                Mouse . OverrideCursor = Cursors . Arrow;
                return;
            }
            if ( GcCanvas == null )
                GcCanvas = this . Parent as Canvas;
            double ourheight = this . ActualHeight;
            double ourwidth = this . ActualWidth;
            height = ourheight;
            width = ourwidth;
            left = e . GetPosition ( ( GcCanvas as FrameworkElement ) . Parent as FrameworkElement ) . X;
            top = e . GetPosition ( ( GcCanvas as FrameworkElement ) . Parent as FrameworkElement ) . Y;
            double trueleft = CpFirstXPos;
            double truetop = CpFirstYPos;
            Debug . WriteLine ( $"MouseMove top={top}, Height={ourheight}" );

            // dont let it slide out of view
            double horposition = ( left ) - ( CpFirstXPos );
            double vertposition = ( top - 5 ) - ( CpFirstYPos );

            if ( horposition < 20 )
                horposition = 20;
            if ( horposition + ( ourwidth + 40 ) > GcCanvas . ActualWidth )
                horposition = GcCanvas . ActualWidth - ( ourwidth + 40 );
            if ( vertposition < 5 )
                vertposition = 5;
            if ( ( vertposition + ( ourheight ) ) > GcCanvas . ActualHeight )
                vertposition = GcCanvas . ActualHeight - ( ourheight );

            ( this as FrameworkElement ) . SetValue ( Canvas . LeftProperty , ( double ) horposition );
            ( this as FrameworkElement ) . SetValue ( Canvas . TopProperty , ( double ) vertposition );
            top = vertposition;
            bottom = vertposition + height;
            //Debug . WriteLine ( $"MOVE EXIT = {left - CpFirstXPos}, YPos= {top - CpFirstYPos}" );
            counter++;
        }

        #endregion Font Listbox  (mainarea) move/sizing INNER
        private void border_MouseEnter ( object sender , MouseEventArgs e )
        {
            Mouse . OverrideCursor = Cursors . SizeAll;
        }
        private void border_MouseLeave ( object sender , MouseEventArgs e )
        {
            Mouse . OverrideCursor = Cursors . Arrow;
        }

        #region Font Listbox  (border) move/sizing INNER
        private void border_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
        {
            Border border = e . OriginalSource as Border;
            if ( border == null )
                return;

            if ( e . LeftButton == MouseButtonState . Pressed )
                Mouse . OverrideCursor = Cursors . SizeAll;
            else
            {
                Mouse . OverrideCursor = Cursors . Arrow;
                return;
            }
            ourlist = DapperGenericsLib . Utils . FindVisualParent<GenericSelectBoxControl> ( sender as DependencyObject );
            border = e . OriginalSource as Border;
            //TODO  make  it  border
            double Left = e . GetPosition ( ourlist ) . X;
            double Top = e . GetPosition ( ourlist ) . Y;
            double Height = ourlist . ActualHeight;
            double Width = ourlist . ActualWidth;

            // Handle relevant mouse pointers first
            if ( Left <= 15 && Top <= 15 && ( Top <= 15 )
                || ( Top <= 15 && Left > Width - 15 )
                || ( Left >= Width - 15 && Top >= Height - 15 )
                || ( Left <= 15 && Top >= Height - 15 ) )
            {
                // over any corner	    WORKING
                Mouse . OverrideCursor = Cursors . SizeAll;
            }
            else
            {
                // setup the correct cursor to match size direction we need to perform
                //borderSelected = 5;      // TOP
                //borderSelected = 6;       // BOTTOM 
                //borderSelected = 7;       // LEFT
                //borderSelected = 8;       // RIGHT

                if ( Left >= border . ActualWidth - 15
                    && Width >= border . ActualWidth - 15
                    && Width <= border . ActualWidth )
                { // over right border
                    Mouse . OverrideCursor = Cursors . SizeWE;
                    BorderSelected = 8;
                }
                else if ( Left <= 10 && Top >= 11
                     && Top >= 15
                     && Top < Height - 15 )
                {    // Over left border
                    Mouse . OverrideCursor = Cursors . SizeWE;
                    BorderSelected = 7;
                }
                else if ( Top <= 15
                    && Left >= 15 )
                {   // over  top  border
                    Mouse . OverrideCursor = Cursors . SizeNS;
                    BorderSelected = 5;
                }
                else if ( Top >= Height - 15 )
                {   // over bottom border
                    Mouse . OverrideCursor = Cursors . SizeNS;
                    BorderSelected = 6;
                }
            }
            //			Debug. WriteLine ( $"In Mousemove, at stage 2" );

            // Now handle resizing the top border
            if ( this . BorderSelected == 5 && this . BorderSelected != -1 )
            // && ( FlowdocResizing || this . BorderClicked && IsCornerDragging == false ) )
            {
                // Get current sizes and position of GenListbox  window to intilize our calculations
                if ( FdLeft == 0 )
                    FdLeft = Canvas . GetLeft ( this );
                if ( FdTop == 0 )
                    FdTop = Canvas . GetTop ( this );
                FdHeight = this . ActualHeight;
                FdWidth = this . ActualWidth;
                //Get mouse cursor position
                Point pt = Mouse . GetPosition ( GcCanvas );
                double MLeft = pt . X;
                // Border Top position
                FdTop = pt . Y;
                FdBottom = FdTop + FdHeight;
                double ValidTop = FdBottom - ( 2 );
            }
            if ( this . BorderSelected == 1 )
            {
                // Top border - WORKING CORRECTLY
                //Mouse . OverrideCursor = Cursors . SizeNS;
                //Canvas . SetTop ( this , MTop );
                //YDiff = MTop - FdTop;
                //FdTop = MTop;

                //newHeight = FdHeight - YDiff;
                //if ( newHeight < 200 )
                //    newHeight = 200;
                //this . Height = newHeight;
                //if ( IsCornerDragging == true )
                //{
                //    // drag left as well
                //    XDiff = MLeft - FdLeft;
                //    newWidth = FdWidth - XDiff;
                //    if ( newWidth < 350 )
                //        newWidth = 350;
                //    this . Width = newWidth;
                //    Canvas . SetLeft ( this , MLeft );
                //    FdLeft = MLeft;
                //}
                return;
            }
            {
                //    else if ( Flowdoc . BorderSelected == 2 )
                //    {     // Bottom border
                //        Mouse . OverrideCursor = Cursors . SizeNS;
                //        newHeight = MTop - FdTop;
                //        Flowdoc . Height = newHeight;
                //        return;
                //    }
                //    else if ( Flowdoc . BorderSelected == 3 )
                //    {   // Left hand side border  - WORKING CORRECTLY
                //        Mouse . OverrideCursor = Cursors . SizeWE;
                //        XDiff = MLeft - FdLeft;
                //        newWidth = FdWidth - XDiff;
                //        if ( newWidth < 350 )
                //            newWidth = 350;
                //        Flowdoc . Width = newWidth;
                //        Canvas . SetLeft ( Flowdoc , MLeft );
                //        FdLeft = MLeft;
                //        return;
            }
        }

        private void border_PreviewMouseMove ( object sender , MouseEventArgs e )
        {
            // sender = border !!
            double diff = 0;
            // Border ourborder = sender as Border;
            // Debug . WriteLine ($"{this.BorderSelected}");
            if ( this . BorderSelected == 5 && this . BorderSelected != -1
             && e . LeftButton == MouseButtonState . Pressed )
            {
                if ( ourlist == null )
                    return;
                //FdTop = top;

                Debug . WriteLine ( $"ENTRY top = {top},  Height={border . Height}" );
                // Get border's top position
                double newFdTop = e . GetPosition ( ( GcCanvas as FrameworkElement ) as FrameworkElement ) . Y;
                // FdTop = new positon
                if ( newFdTop > top )
                {
                    diff = newFdTop - top;    // Positive value, reducing height
                    border . Height -= diff;
                    top += diff;
                }
                else
                {
                    diff = top - newFdTop; //  Negatiive value, increasing height
                    border . Height += diff;
                    top -= diff;
                }
                Debug . WriteLine ( $"ENTRY Diff = {diff}" );
                //Point pt = Mouse . GetPosition ( GcCanvas );
                //Debug . WriteLine ( $"ENTRY pt X= {pt . X}, pt.Y={pt . Y}" );
                // if ( FdTop >= pt . Y )
                //{
                //    diff = FdTop + pt . Y;
                //    FdBottom = FdTop + FdHeight;
                //}
                //else
                //{
                //    diff = pt . Y - FdTop;
                //    FdBottom = FdTop - FdHeight;
                //}
                //top += diff;
                //               //double ValidTop = FdBottom - ( 2 );
                //double ValidBottom = FdBottom + ( 2 );
                //border = ourborder;
                ( border as FrameworkElement ) . SetValue ( Canvas . TopProperty , ( double ) top );
                ( border as FrameworkElement ) . SetValue ( Canvas . BottomProperty , ( double ) top + border . Height );
                // ( border as FrameworkElement ) . SetValue ( Canvas . BottomProperty , ( double ) top + border.Height );
                // uppdate global top position
                //               top = pt . Y;
                //( this as FrameworkElement ) . SetValue ( Canvas . BottomProperty , ( double )FdBottom  - FdTop );
                Debug . WriteLine ( $"EXIT Diff = {diff}, top= {top}, Height={border . Height}" );

            }

        }

        private void border_PreviewMouseLeftButtonUp ( object sender , MouseButtonEventArgs e )
        {
            top = ( double ) border . GetValue ( Canvas . TopProperty );
            left = ( double ) border . GetValue ( Canvas . BottomProperty );
            this . BorderSelected = -1;
            Mouse . OverrideCursor = Cursors . Arrow;
            Debug . WriteLine ( $"Button up : top= {top}, Height={border . Height}" );
        }
        #endregion Font Listbox  (border) move/sizing

        #endregion Font Listbox move/sizing OUTER

        #region utilities
        public static GenericSelectBoxControl GetThisControl ( object sender )
        {
            TextBlock tb = sender as TextBlock;
            Grid grid = tb . Parent as Grid;
            GenericSelectBoxControl ourlist = grid . Parent as GenericSelectBoxControl;
            return ourlist;
        }
        #endregion utilities
    }
}
