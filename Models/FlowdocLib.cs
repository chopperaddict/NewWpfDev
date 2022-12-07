using NewWpfDev . UserControls;
using NewWpfDev . ViewModels;
using NewWpfDev . Views;

using System;
using System . Collections . Generic;
using System . ComponentModel;
using System . Diagnostics;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;


namespace NewWpfDev . Models
{
    public class FlowdocLib
    {

        #region OnPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged ( string propertyName )
        {
            if ( PropertyChanged != null )
            {
                PropertyChanged ( this , new PropertyChangedEventArgs ( propertyName ) );
            }
        }
        #endregion OnPropertyChanged

        #region Flowdoc Full Properties
        private FlowDoc flowDoc;
        public FlowDoc Flowdoc
        {
            get { return flowDoc; }
            set { flowDoc = value; NotifyPropertyChanged ( Flowdoc . ToString ( ) ); }
        }

        private bool flowdocResizing;
        public bool FlowdocResizing
        {
            get { return flowdocResizing; }
            set { flowdocResizing = value; NotifyPropertyChanged ( FlowdocResizing . ToString ( ) ); }
        }
        private double flowdocFloatingTop;
        public double FlowdocFloatingTop
        {
            get { return flowdocFloatingTop; }
            set { flowdocFloatingTop = value; NotifyPropertyChanged ( FlowdocFloatingTop . ToString ( ) ); }
        }
        private double flowdocFloatingLeft;
        public double FlowdocFloatingLeft
        {
            get { return flowdocFloatingLeft; }
            set { flowdocFloatingLeft = value; NotifyPropertyChanged ( FlowdocFloatingLeft . ToString ( ) ); }
        }
        private double flowdocFloatingHeight;
        public double FlowdocFloatingHeight
        {
            get { return flowdocFloatingHeight; }
            set { flowdocFloatingHeight = value; NotifyPropertyChanged ( FlowdocFloatingHeight . ToString ( ) ); }
        }
        private double flowdocFloatingWidth;
        public double FlowdocFloatingWidth
        {
            get { return flowdocFloatingWidth; }
            set { flowdocFloatingWidth = value; NotifyPropertyChanged ( FlowdocFloatingWidth . ToString ( ) ); }
        }

        #endregion Flowdoc Full Properties

        #region Flowdoc Variables
        // These ARE USED
        public double flowdocHeight = 0;
        public double flowdocWidth = 0;
        public double flowdocTop = 0;
        public double flowdocLeft = 0;
        // This IS USED
        public bool TvMouseCaptured = false;
        public double newWidth = 0;
        public double newHeight = 0;
        public double YDiff = 0;
        public double XDiff = 0;
        public double FdLeft = 0;
        public double FdTop = 0;
        public double FdHeight = 0;
        public double FdFullCanvasHeight = 0;
        public double FdFullCanvasWidth= 0;
        public double FdWidth = 0;
        public double MLeft = 0;
        public double MTop = 0;
        public bool CornerDrag = false;
        public double FdBorderWidth = 0;
        public double FdBottom = 0;
        public double ValidTop = 0;
        public double ValidBottom = 0;
        public Thickness th = new Thickness ( 0 , 0 , 0 , 0 );
        public double CpFirstXPos = 0;
        public double CpFirstYPos = 0;
        public static bool IsScrollbarActive { get; set; }
        public static Canvas parentcanvas { get; set; }
        public static FlowDoc flowdoc { get; set; }



        #endregion Flowdoc Variables
        public FlowdocLib ( FlowDoc ctrl , Canvas Parentcanvas )
        {
            if ( ctrl != null )
                flowdoc = ctrl;
            if ( Parentcanvas == null )
                parentcanvas = Flowdoc . topcanvas;
            else
                parentcanvas = Parentcanvas;
            
        }
        #region Methods
        public void ShowInfo ( FlowDoc Flowdoc , Canvas canvas , string line1 = "" , string clr1 = "" , string line2 = "" , string clr2 = "" , string line3 = "" , string clr3 = "" , string header = "" , string clr4 = "" , bool beep = false )
        {
            Flowdoc . ShowInfo ( Flowdoc , canvas , line1 , clr1 , line2 , clr2 , line3 , clr3 , header , clr4 , beep );
            canvas . Visibility = Visibility . Visible;
            canvas . BringIntoView ( );
            Flowdoc . Visibility = Visibility . Visible;
            Flowdoc . BringIntoView ( );
            Flowdoc . Refresh ( );
            Flowdoc . UpdateLayout ( );
            //Flowdoc . TopMost = true;

            if ( Flags . PinToBorder == true )
            {
                ( Flowdoc as FrameworkElement ) . SetValue ( Canvas . LeftProperty , ( double ) 0 );
                ( Flowdoc as FrameworkElement ) . SetValue ( Canvas . TopProperty , ( double ) 0 );
            }
        }

        public void FdMsg ( FlowDoc Flowdoc , Canvas canvas , string line1 = "" , string line2 = "" , string line3 = "" , bool beep = false )
        {
            string clr1 = "Black0";
            string clr2 = "Blue0";
            string clr3 = "Green2";
            string clr4 = "Red3";

            Flowdoc . ShowInfo ( Flowdoc , canvas , line1 , clr1 , line2 , clr2 , "" , clr3 , "" , clr4 , beep );
            canvas . Visibility = Visibility . Visible;
            canvas . BringIntoView ( );
            Flowdoc . Visibility = Visibility . Visible;
            Flowdoc . BringIntoView ( );
            if ( Flags . PinToBorder == true )
            {
                ( Flowdoc as FrameworkElement ) . SetValue ( Canvas . LeftProperty , ( double ) 0 );
                ( Flowdoc as FrameworkElement ) . SetValue ( Canvas . TopProperty , ( double ) 0 );
            }
        }
        // THIS GETS CALLED WHENEVER A FLOWDOC IS MOVED !!
        private void DragTopLeft ( FlowDoc Flowdoc , Canvas canvas )
        {
            // move left side
            if ( FdLeft == 0 )
                FdLeft = Canvas . GetLeft ( Flowdoc );
            if ( FdTop == 0 )
                FdTop = Canvas . GetTop ( Flowdoc );
            FdHeight = Flowdoc . ActualHeight;
            FdWidth = Flowdoc . ActualWidth;
            //Get mouse cursor position
            Point pt = Mouse . GetPosition ( canvas );
            MLeft = pt . X;
            MTop = pt . Y;
            //				if ( th . Left == 0 )
            th = Flowdoc . FdBorder . BorderThickness;
            FdBorderWidth = th . Left * 2;
            FdBottom = FdTop + FdHeight;
            ValidTop = FdBottom - ( FdBorderWidth / 2 );
            ValidBottom = FdBottom + ( FdBorderWidth / 2 );

            Canvas . SetTop ( Flowdoc , MTop );
            YDiff = MTop - FdTop;
            FdTop = MTop;

            newHeight = FdHeight - YDiff;
            if ( newHeight < 200 )
                newHeight = 200;
            Flowdoc . Height = newHeight;

            // drag left as well
            XDiff = MLeft - FdLeft;
            newWidth = FdWidth - XDiff;
            if ( newWidth < 350 )
                newWidth = 350;
            Flowdoc . Width = newWidth;
            Canvas . SetLeft ( Flowdoc , MLeft );
            FdLeft = MLeft;
        }
        private void DragTopRight ( FlowDoc Flowdoc , Canvas canvas )
        {
            if ( FdLeft == 0 )
                FdLeft = Canvas . GetLeft ( Flowdoc );
            if ( FdTop == 0 )
                FdTop = Canvas . GetTop ( Flowdoc );
            FdHeight = Flowdoc . ActualHeight;
            FdWidth = Flowdoc . ActualWidth;
            //Get mouse cursor position
            Point pt = Mouse . GetPosition ( canvas );
            MLeft = pt . X;
            MTop = pt . Y;
            //				if ( th . Left == 0 )
            th = Flowdoc . FdBorder . BorderThickness;
            FdBorderWidth = th . Left * 2;
            FdBottom = FdTop + FdHeight;
            ValidTop = FdBottom - ( FdBorderWidth / 2 );
            ValidBottom = FdBottom + ( FdBorderWidth / 2 );

            YDiff = FdTop - MTop;
            FdTop = MTop;
            Canvas . SetTop ( Flowdoc , MTop );
            // Handle Height
            newHeight = FdHeight + YDiff;
            if ( newHeight < 200 )
                newHeight = 200;
            Flowdoc . Height = FdHeight;
            // handle width
            newWidth = MLeft - FdLeft;
            FdWidth = newWidth;
            Flowdoc . Width = FdWidth;
            Flowdoc . SetValue ( FlowDoc . HeightProperty , newHeight );
            Flowdoc . SetValue ( FlowDoc . WidthProperty , newWidth );

        }
        private void DragBottomLeft ( FlowDoc Flowdoc , Canvas canvas )
        {
            if ( FdLeft == 0 )
                FdLeft = Canvas . GetLeft ( Flowdoc );
            if ( FdTop == 0 )
                FdTop = Canvas . GetTop ( Flowdoc );
            FdHeight = Flowdoc . ActualHeight;
            FdWidth = Flowdoc . ActualWidth;
            //Get mouse cursor position
            Point pt = Mouse . GetPosition ( canvas );
            MLeft = pt . X;
            MTop = pt . Y;
            //				if ( th . Left == 0 )
            th = Flowdoc . FdBorder . BorderThickness;
            FdBorderWidth = th . Left * 2;
            FdBottom = FdTop + FdHeight;
            ValidTop = FdBottom - ( FdBorderWidth / 2 );
            ValidBottom = FdBottom + ( FdBorderWidth / 2 );

            newHeight = MTop - FdTop;
            Flowdoc . Height = newHeight;

            XDiff = MLeft - FdLeft;
            newWidth = FdWidth - XDiff;
            if ( newWidth < 350 )
                newWidth = 350;
            Flowdoc . Width = newWidth;
            Canvas . SetLeft ( Flowdoc , MLeft );
            FdLeft = MLeft;
        }
        private void DragBottomRight ( FlowDoc Flowdoc , Canvas canvas )
        {
            if ( FdLeft == 0 )
                FdLeft = Canvas . GetLeft ( Flowdoc );
            if ( FdTop == 0 )
                FdTop = Canvas . GetTop ( Flowdoc );
            FdHeight = Flowdoc . ActualHeight;
            FdWidth = Flowdoc . ActualWidth;
            //Get mouse cursor position
            Point pt = Mouse . GetPosition ( canvas );
            MLeft = pt . X;
            MTop = pt . Y;
            //				if ( th . Left == 0 )
            th = Flowdoc . FdBorder . BorderThickness;
            FdBorderWidth = th . Left * 2;
            FdBottom = FdTop + FdHeight;
            ValidTop = FdBottom - ( FdBorderWidth / 2 );
            ValidBottom = FdBottom + ( FdBorderWidth / 2 );

            // handle height
            newHeight = MTop - FdTop;
            Flowdoc . Height = newHeight;

            double mouseposY = FdTop + FdHeight;
            YDiff = MTop - FdTop - FdHeight;
            newHeight = FdHeight + YDiff;// - FdLeft;
            Flowdoc . Height = newHeight;
            if ( newHeight < 0 )
                return;
            //Reset width - WORKING
            newWidth = MLeft - FdLeft;
            Flowdoc . Width = newWidth;
            Flowdoc . SetValue ( FlowDoc . HeightProperty , newHeight );
            Flowdoc . SetValue ( FlowDoc . WidthProperty , newWidth );
        }

        // This IS CALLED by ListViews...
        // This IS CALLED by DataGrid...
        public void Flowdoc_MouseMove ( FlowDoc Flowdoc , Canvas canvas , object MovingObject , MouseEventArgs e )
        {
            //borderSelected = 5;      // TOP + LEFT
            //borderSelected = 6;       // BOTTOM + LEFT
            //borderSelected = 7;       // TOP + RIGHT
            //borderSelected = 8;       // BOTTOM + RIGHT
            bool IsCornerDragging = false;

            if ( IsScrollbarActive )
            {
                var v = e . OriginalSource . ToString ( );
                Flowdoc . ReleaseMouseCapture ( );
                return;
            }
            #region set up mouse cursor for Moving control around
            Border border = e . OriginalSource as Border;
            if ( border == null )
            {
                if ( e . LeftButton == MouseButtonState . Pressed )
                    Mouse . OverrideCursor = Cursors . SizeAll;
                else
                    Mouse . OverrideCursor = Cursors . Arrow;
                //return;
            }
            #endregion set up mouse cursor for sizing

            // We are Resizing the Flowdoc using the mouse on a corner of the border  (Border.Name=FdBorder)
            #region set up border that is under mouse cursor 
            if ( Flowdoc . BorderSelected == 5 )  // Top Left corner
            {
                if ( e . LeftButton == MouseButtonState . Pressed )
                    DragTopLeft ( Flowdoc , canvas );
            }
            else if ( Flowdoc . BorderSelected == 6 )  // boottom Left corner
            {
                if ( e . LeftButton == MouseButtonState . Pressed )
                    DragBottomLeft ( Flowdoc , canvas );
            }
            else if ( Flowdoc . BorderSelected == 7 )  // Top right corner
            {
                if ( e . LeftButton == MouseButtonState . Pressed )
                    DragTopRight ( Flowdoc , canvas );
            }
            else if ( Flowdoc . BorderSelected == 8 )  // Bottom Right corner
            {
                if ( e . LeftButton == MouseButtonState . Pressed )
                    DragBottomRight ( Flowdoc , canvas );
            }
            #endregion set up border that is under mouse cursor 

            border = e . OriginalSource as Border;

            #region Get border position info and set specific cursor to match border

            if ( border != null )
            {
                string res = e . LeftButton == MouseButtonState . Pressed ? "Yes" : "No";
                if ( e . LeftButton == MouseButtonState . Pressed )
                {
                    IsCornerDragging = true;
                    FlowdocResizing = true;
                }
                else
                {
                    IsCornerDragging = false;
                    FlowdocResizing = false;
                }
                flowdocLeft = e . GetPosition ( Flowdoc ) . X;
                flowdocTop = e . GetPosition ( Flowdoc ) . Y;
                flowdocHeight = Flowdoc . ActualHeight;
                flowdocWidth = Flowdoc . ActualWidth;

                // Handle relevant mouse pointers first
                if ( flowdocLeft <= 15 && flowdocTop <= 15 && ( flowdocTop <= 15 )
                    || ( flowdocTop <= 15 && flowdocLeft > flowdocWidth - 15 )
                    || ( flowdocLeft >= flowdocWidth - 15 && flowdocTop >= flowdocHeight - 15 )
                    || ( flowdocLeft <= 15 && flowdocTop >= flowdocHeight - 15 ) )
                {
                    // over any corner	    WORKING
                    Mouse . OverrideCursor = Cursors . SizeAll;
                }
                else
                {
                    if ( flowdocLeft >= border . ActualWidth - 15
                        && flowdocWidth >= border . ActualWidth - 15
                        && flowdocWidth <= border . ActualWidth )
                        // over right border
                        Mouse . OverrideCursor = Cursors . SizeWE;
                    else if ( flowdocLeft <= 10 && flowdocTop >= 11
                         && flowdocTop >= 15
                         && flowdocTop < flowdocHeight - 15 )
                        // Over left border
                        Mouse . OverrideCursor = Cursors . SizeWE;
                    else if ( flowdocTop <= 15
                        && flowdocLeft >= 15 )
                        // over  top  border
                        Mouse . OverrideCursor = Cursors . SizeNS;
                    else if ( flowdocTop >= flowdocHeight - 15 )
                        // over bottom border
                        Mouse . OverrideCursor = Cursors . SizeNS;
                }
            }
            #endregion Get border position info

            // Now handle resizing the top/bottom/left/right borders,but NOT corners
            #region Get resize control from border dragging

            if ( ( Flowdoc . BorderSelected < 5 && Flowdoc . BorderSelected != -1 )
                && ( FlowdocResizing || Flowdoc . BorderClicked && IsCornerDragging == false ) )
            {

                // Get current sizes and position of Flowdoc window to intialize our calculations
                if ( FdLeft == 0 )
                    FdLeft = Canvas . GetLeft ( Flowdoc );
                if ( FdTop == 0 )
                    FdTop = Canvas . GetTop ( Flowdoc );
                FdBottom = canvas . ActualHeight;

                FdFullCanvasHeight = FdBottom;
                FdFullCanvasWidth = canvas . ActualWidth;
                FdHeight = Flowdoc . ActualHeight;
                FdWidth = Flowdoc . ActualWidth;
                //Get mouse cursor position
                Point pt = Mouse . GetPosition ( canvas );
                MLeft = pt . X;
                MTop = pt . Y;
                //				if ( th . Left == 0 )
                th = Flowdoc . FdBorder . BorderThickness;
                FdBorderWidth = th . Left * 2;
                FdBottom = FdTop + FdHeight;

                // Block stretching control beyond right side
                if ( FdBottom > FdFullCanvasHeight )
                {
                   // Debug . WriteLine ( $"FDL Exc" +
                   //$"Fdl bottom exeeds Canvas Height={FdFullCanvasHeight} by {FdFullCanvasHeight - ( FdTop + FdHeight )}" );
                    Canvas . SetTop ( Flowdoc , FdTop - 1 );
                    Flowdoc_MouseLeftButtonUp ( null , Flowdoc , MovingObject , null );
                    return;
                }

                // Block stretching control below canvas bottom
                if ( FdLeft + FdWidth > FdFullCanvasWidth - 10 )
                {
                   // Debug . WriteLine ( $"FDL Exc" +
                   //$"Fdl bottom exeeds Canvas Height={FdFullCanvasWidth} by {FdFullCanvasWidth - ( FdTop + FdWidth )}" );
                    Canvas . SetLeft( Flowdoc , FdLeft - 1 );
                    Flowdoc_MouseLeftButtonUp ( null , Flowdoc , MovingObject , null );
                    return;
                }
                ValidTop = FdBottom - ( FdBorderWidth / 2 );
                ValidBottom = FdBottom + ( FdBorderWidth / 2 );

                if ( Flowdoc . BorderSelected == 1 )
                {
                    // Top border - WORKING CORRECTLY
                    Mouse . OverrideCursor = Cursors . SizeNS;
                    Canvas . SetTop ( Flowdoc , MTop );
                    YDiff = MTop - FdTop;
                    FdTop = MTop;

                    newHeight = FdHeight - YDiff;
                    if ( newHeight < 200 )
                        newHeight = 200;
                    Flowdoc . Height = newHeight;
                    if ( IsCornerDragging == true )
                    {
                        // drag left as well
                        XDiff = MLeft - FdLeft;
                        newWidth = FdWidth - XDiff;
                        if ( newWidth < 350 )
                            newWidth = 350;
                        Flowdoc . Width = newWidth;
                        Canvas . SetLeft ( Flowdoc , MLeft );
                        FdLeft = MLeft;
                    }
                    return;
                }
                else if ( Flowdoc . BorderSelected == 2 )
                {     // Bottom border
                    Mouse . OverrideCursor = Cursors . SizeNS;
                    newHeight = MTop - FdTop;
                    Flowdoc . Height = newHeight;
                    return;
                }
                else if ( Flowdoc . BorderSelected == 3 )
                {   // Left hand side border  - WORKING CORRECTLY
                    Mouse . OverrideCursor = Cursors . SizeWE;
                    XDiff = MLeft - FdLeft;
                    newWidth = FdWidth - XDiff;
                    if ( newWidth < 350 )
                        newWidth = 350;
                    Flowdoc . Width = newWidth;
                    Canvas . SetLeft ( Flowdoc , MLeft );
                    FdLeft = MLeft;
                    return;
                }
                else if ( Flowdoc . BorderSelected == 4 )
                {
                    // Right hand side border  OR Top Right Corner 
                    Mouse . SetCursor ( Cursors . SizeWE );
                    if ( CornerDrag || MTop - FdTop <= FdBorderWidth || FdTop - MTop <= -FdBorderWidth )
                    {
                        Mouse . SetCursor ( Cursors . SizeAll );
                        if ( FdTop - MTop >= -FdBorderWidth )
                        {
                            //  Right border clicked	- working very well 
                            YDiff = FdTop - MTop;
                            FdTop = MTop;
                            Canvas . SetTop ( Flowdoc , MTop );
                            // Handle Height
                            newHeight = FdHeight + YDiff;
                            if ( newHeight < 200 )
                                newHeight = 200;
                            Flowdoc . Height = FdHeight;
                            // handle width
                            newWidth = MLeft - FdLeft;
                            FdWidth = newWidth;
                            Flowdoc . Width = FdWidth;
                            Flowdoc . SetValue ( FlowDoc . HeightProperty , newHeight );
                            Flowdoc . SetValue ( FlowDoc . WidthProperty , newWidth );
                            return;
                        }
                        else
                        {
                            // just dragging right border WORKING CORRECTLY  for right border
                            newWidth = MLeft - FdLeft;
                            Flowdoc . Width = newWidth;
                            Flowdoc . SetValue ( FlowDoc . WidthProperty , newWidth );
                            CornerDrag = true;
                            return;
                        }
                    }
                }
                return;
            }
            #endregion Get resize control from border dragging

            #region Move control by dragging it

            else
            {
                if ( MovingObject != null && e . LeftButton == MouseButtonState . Pressed && Flowdoc . BorderClicked == false )
                {
                    // MOVING WINDOW around the Parent window (MDI ?)
                    // Get mouse position IN FlowDoc !!
                    //Mouse . SetCursor ( Cursors . SizeAll );
                    double left = e . GetPosition ( ( MovingObject as FrameworkElement ) . Parent as FrameworkElement ) . X - CpFirstXPos;
                    double top = e . GetPosition ( ( MovingObject as FrameworkElement ) . Parent as FrameworkElement ) . Y - CpFirstYPos;
                    double trueleft = left - CpFirstXPos;
                    double truetop = left - CpFirstYPos;
                    if ( left >= 0 ) // && left <= canvas.ActualWidth - Flowdoc.ActualWidth)
                        ( MovingObject as FrameworkElement ) . SetValue ( Canvas . LeftProperty , left );
                    if ( top >= 0 )
                    {
                        // Do NOT allow dialog to extend Below canvas limits  17/9/2022
                        if ( top + Flowdoc . Height < canvas . ActualHeight )
                            ( MovingObject as FrameworkElement ) . SetValue ( Canvas . TopProperty , top );
                    }
                }
            }
            #endregion Move control by dragging it

        }

        // Called by Datagrids	&& Listviews
        public void MaximizeFlowDoc ( FlowDoc Flowdoc , Canvas canvas , EventArgs e )
        {
            //Canvas CanVas = canvas;
            // Clever "Hook" method that Allows the flowdoc to be resized to fill window
            // or return to its original size and position courtesy of the Event declard in FlowDoc
            if ( Flowdoc . BorderClicked )
            {
                return;
            }
            double height = canvas . ActualHeight;
            double width = canvas . ActualWidth;
            if ( ( Flowdoc . Height < canvas . ActualHeight && Flowdoc . ActualWidth < canvas . ActualWidth ) )
            {
                // it is in NORMAL mode right now
                // Set flowdoc size into variables for later use
                FlowdocFloatingTop = Convert . ToDouble ( Flowdoc . GetValue ( Canvas . TopProperty ) );
                FlowdocFloatingLeft = Convert . ToDouble ( Flowdoc . GetValue ( Canvas . LeftProperty ) );
                //if ( Flowdoc . Height == 0)
                //            {
                //	Flowdoc . Height = Convert . ToDouble ( Flowdoc . GetValue ( Canvas . BottomProperty ) ) - FlowdocFloatingTop;
                //	Flowdoc . Width = Convert . ToDouble ( Flowdoc . GetValue ( Canvas . RightProperty ) ) - FlowdocFloatingLeft;

                //}
                FlowdocFloatingHeight = Flowdoc . ActualHeight;
                FlowdocFloatingWidth = Flowdoc . ActualWidth;
                flowdocHeight = Flowdoc . Height;
                flowdocWidth = Flowdoc . ActualWidth;
                flowdocTop = Convert . ToDouble ( Flowdoc . GetValue ( Canvas . LeftProperty ) );
                flowdocLeft = Convert . ToDouble ( Flowdoc . GetValue ( Canvas . TopProperty ) );
                ( Flowdoc as FrameworkElement ) . SetValue ( Canvas . LeftProperty , ( double ) 0 );
                ( Flowdoc as FrameworkElement ) . SetValue ( Canvas . TopProperty , ( double ) 0 );
                Flowdoc . Height = height;
                Flowdoc . Width = width;
                // save current size/position
            }
            else
            {
                // it is MAXIMIZED right now
                // We re returning it to normal position/Size
                Flowdoc . Height = FlowdocFloatingHeight;
                Flowdoc . Width = FlowdocFloatingWidth;
                if ( Flags . PinToBorder )
                {
                    ( Flowdoc as FrameworkElement ) . SetValue ( Canvas . LeftProperty , ( double ) 0 );
                    ( Flowdoc as FrameworkElement ) . SetValue ( Canvas . TopProperty , ( double ) 0 );
                }
                else
                {
                    ( Flowdoc as FrameworkElement ) . SetValue ( Canvas . LeftProperty , ( double ) FlowdocFloatingLeft );
                    ( Flowdoc as FrameworkElement ) . SetValue ( Canvas . TopProperty , ( double ) FlowdocFloatingTop );
                    ( Flowdoc as FrameworkElement ) . SetValue ( Canvas . HeightProperty , ( double ) FlowdocFloatingHeight );
                    ( Flowdoc as FrameworkElement ) . SetValue ( Canvas . WidthProperty , ( double ) FlowdocFloatingWidth );
                }
            }
        }
        // HIT IN MVVMDATAGRID

        // Called by ListViews	&& Datagrids
        public object Flowdoc_PreviewMouseLeftButtonDown ( object sender , FlowDoc Flowdoc , MouseButtonEventArgs e )
        {
            //In this event, we get current mouse position on the control to use it in the MouseMove event.
            // This IS CALLED by ListViews

            if ( WpfLib1 . Utils . HitTestScrollBar ( sender , e ) )
            {
                IsScrollbarActive = true;
            }

            Border border = e . OriginalSource as Border;
            if ( border != null )
            {
                FlowdocResizing = true;
                flowdocLeft = e . GetPosition ( Flowdoc ) . X;
                flowdocTop = e . GetPosition ( Flowdoc ) . Y;
                flowdocHeight = Flowdoc . ActualHeight;
                flowdocWidth = Flowdoc . ActualWidth;
                Flowdoc . CaptureMouse ( );
                return null;
            }
            else
            {
                Button btn = sender as Button;
                if ( btn != null )
                {
                    return null;
                }
                //               var str = sender as FlowDoc;
            }
            flowdocLeft = e . GetPosition ( Flowdoc ) . X;
            flowdocTop = e . GetPosition ( Flowdoc ) . Y;
            flowdocHeight = Flowdoc . ActualHeight;
            flowdocWidth = Flowdoc . ActualWidth;
            double currcursorH = e . GetPosition ( Flowdoc ) . Y;
            double currcursorW = e . GetPosition ( Flowdoc ) . X;
            CpFirstXPos = e . GetPosition ( sender as Control ) . X;
            CpFirstYPos = e . GetPosition ( sender as Control ) . Y;
            double FirstArrowXPos = e . GetPosition ( ( sender as Control ) . Parent as Control ) . X - CpFirstXPos;
            double FirstArrowYPos = e . GetPosition ( ( sender as Control ) . Parent as Control ) . Y - CpFirstYPos;
            Flowdoc . BorderClicked = false;
            Debug . WriteLine ( "Mouse is DOWN" );
            return sender;
        }
        // Called by BOTH WINDOWS
        public object Flowdoc_MouseLeftButtonUp ( object sender , FlowDoc Flowdoc , object MovingObject , MouseButtonEventArgs e )
        {
            // Window wide  !!
            // Called  when a Flowdoc MOVE has ended
            FlowdocResizing = false;
            Flowdoc . BorderClicked = false;
            Flowdoc . BorderSelected = -1;
            CornerDrag = false;
            TvMouseCaptured = false;
            FdLeft = FdTop = th . Left = 0;
            Mouse . OverrideCursor = Cursors . Arrow;
            //Flowdoc.ReleaseMouseCapture ( );
            Flowdoc . Focus ( );
            IsScrollbarActive = false;
            Debug . WriteLine ( "Mouse is DOWN" );
            return MovingObject = null;
        }

        #endregion Methods
    }
}
