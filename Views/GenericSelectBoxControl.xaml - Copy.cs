using System;
using System . Diagnostics;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;
using System . Windows . Media;


using NewWpfDev . UserControls;

using Canvas = System . Windows . Controls . Canvas;

namespace NewWpfDev . Views
{
    /// <summary>
    /// Interaction logic for GenericSelectBoxControl.xaml
    /// </summary>
    public partial class GenericSelectBoxControl : UserControl
    {
        public static event EventHandler<SelectionArgs> ListSelection;
        public static Thickness Ctrlstats = new Thickness ( );
        public static GenericSelectBoxControl GenSelBox { get; set; } = null;
        public static GenericGridControl gridctrl { get; set; } = null;
        public static Canvas GcCanvas { get; set; }
        public FlowDoc fdl { get; set; }
        public object callerobj { get; set; } = null;
        public static bool MouseCaptured = false;
        public bool ListMoving { get; set; } = false;
        public bool ListResizing { get; set; } = false;
        private static bool IsMouseIconNotArrow { get; set; } = false;
        public double left { get; set; } = 0;
        public double borderbottom { get; set; } = 0;
        public double borderheight { get; set; } = 0;
        //public double CtrlTop { get; set; } = 0;
        public double width { get; set; } = 0;
        public double borderThickness { get; set; } = 0;
        public double CpFirstXPos { get; set; } = 0;
        public double CpFirstYPos { get; set; } = 0;
        public double FdLeft { get; set; } = 0;
        public double FdTop { get; set; } = 0;
        public double FdBottom { get; set; } = 0;
        public double FdHeight { get; set; } = 0;
        public double FdWidth { get; set; } = 0;
        public int BorderSelected { get; set; } = 0;
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
        public double CtrlTop
        {
            get { return ( double ) GetValue ( CtrlTopProperty ); }
            set { SetValue ( CtrlTopProperty , value ); }
        }
        public static readonly DependencyProperty CtrlTopProperty =
            DependencyProperty . Register ( "CtrlTop" , typeof ( double ) , typeof ( GenericSelectBoxControl ) , new PropertyMetadata ( ( double ) 0 ) );
        public double CtrlLeft
        {
            get { return ( double ) GetValue ( CtrlLeftProperty ); }
            set { SetValue ( CtrlLeftProperty , value ); }
        }
        public static readonly DependencyProperty CtrlLeftProperty =
            DependencyProperty . Register ( "CtrlLeft" , typeof ( double ) , typeof ( GenericSelectBoxControl ) , new PropertyMetadata ( ( double ) 0 ) );
        #endregion Dependency Properties


        public GenericSelectBoxControl ( )
        {
            InitializeComponent ( );
            // this is LBSELECTOR (Name for the entire control)
            GenSelBox = this;
            ( GenSelBox as FrameworkElement ) . SetValue ( Canvas . LeftProperty , ( double ) 100 );
            ( GenSelBox as FrameworkElement ) . SetValue ( Canvas . TopProperty , ( double ) 50 );
            border . Width = 300;
            border . Height = 400;
            Ctrlstats . Top = 50;
            CtrlTop= 50;
            Ctrlstats . Bottom = border . Height;
            GenSelBox . Width = border . Width;
            GenSelBox . Height = border . Height;
            CtrlTop = 50;
            left = 100;
            //Debug . WriteLine ( $"\nINIT left= 100, top=50" );
            SetValue ( IsVisibleProperty , true );
            this . IsVisibleChanged += GenericSelectBoxControl_IsVisibleChanged;
            Isopen = true;
        }

        private void GenericSelectBoxControl_IsVisibleChanged ( object sender , DependencyPropertyChangedEventArgs e )
        {
            // try to center popup when making it visible AGAIN
            if ( this . Visibility == Visibility . Visible )
            {
                GenSelBox = sender as GenericSelectBoxControl;
                if ( ( left == 0 && CtrlTop == 0 ) || ( GenSelBox . Height < 350 || GenSelBox . Width <= 400 ) )
                {
                    ( GenSelBox as FrameworkElement ) . SetValue ( Canvas . LeftProperty , ( double ) 100 );
                    ( GenSelBox as FrameworkElement ) . SetValue ( Canvas . TopProperty , ( double ) 50 );
                    border . Width = 300;
                    border . Height = 400;
                    Ctrlstats . Top = 50;
                    CtrlTop = 50;
                    Ctrlstats . Bottom = border . Height;
                    GenSelBox . Width = border . Width;
                    GenSelBox . Height = border . Height;
                    CtrlTop = 50; left = 100;
                    borderheight = GenSelBox . Height;
                    width = GenSelBox . Width;
                    ////Mouse . OverrideCursor = Cursors . Arrow;
                }
            }
            else
            {
                if ( GenSelBox != null )
                {
                    double ourheight = this . ActualHeight;
                    double ourwidth = this . ActualWidth;
                    borderheight = ourheight;
                    width = ourwidth;

                    if ( GcCanvas == null ) GcCanvas = DapperGenericsLib . Utils . FindVisualParent<Canvas> ( sender as DependencyObject );
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
                    CtrlTop = vertposition;
                    Ctrlstats . Top = vertposition;
                    Ctrlstats . Bottom = vertposition + border . Height;
                    left = horposition;
                    ////Mouse . OverrideCursor = Cursors . Arrow;
                }
            }
        }
        private void IsLoaded ( object sender , RoutedEventArgs e )
        {
            Isopen = true;
            ( GenSelBox as FrameworkElement ) . SetValue ( Canvas . LeftProperty , ( double ) 100 );
            ( GenSelBox as FrameworkElement ) . SetValue ( Canvas . TopProperty , ( double ) 50 );
            border . Width = 300;
            border . Height = 400;
            Ctrlstats . Top = 50;
            CtrlTop = 50;
            Ctrlstats . Bottom = border . Height;
            GenSelBox . Width = border . Width;
            GenSelBox . Height = border . Height;
            // Get our parent Canvas
            GenCtrl = DapperGenericsLib . Utils . FindVisualParent<GenericGridControl> ( sender as DependencyObject );
            if ( GcCanvas == null ) GcCanvas = DapperGenericsLib . Utils . FindVisualParent<Canvas> ( sender as DependencyObject );
            Thickness th = new Thickness ( );
            th = border . BorderThickness;
            borderThickness = th . Left;
            CtrlTop = 50;
            ////Mouse . OverrideCursor = Cursors . Arrow;
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
            // update with selected fontfamily from listbox and collapse our listbox
            Canvas canvas = DapperGenericsLib . Utils . FindVisualParent<Canvas> ( this as DependencyObject );
            FlowDoc fdl = DapperGenericsLib . Utils . FindChild<FlowDoc> ( canvas , "Flowdoc" );
            // Update font textbox
            fdl . CurrentFont . Text = listbox . SelectedItem . ToString ( );
            fdl . fontFamily = new FontFamily ( listbox . SelectedItem . ToString ( ) );
            fdl . UpdateDisplay ( );
            this . Visibility = Visibility . Collapsed;
        }

        #region Font Listbox  move/sizing OUTER

        #region Font Listbox  (mainarea) move/sizing INNER

        private void titlebar_MouseEnter ( object sender , MouseEventArgs e )
        {
            //    if ( Mouse . SetCursor ( Cursors . SizeAll) )
            //    {
            //        titlebar . Cursor = Cursors . SizeAll;
            //        Mouse . OverrideCursor = titlebar. Cursor;
            //    }
            //   IsMouseIconNotArrow = true;
        }
        private void titlebar_MouseLeave ( object sender , MouseEventArgs e )
        {
            IsMouseIconNotArrow = false;
            if ( Mouse . SetCursor ( Cursors . Arrow ) )
            {
                titlebar . Cursor = Cursors . Arrow;
                Mouse . OverrideCursor = titlebar . Cursor;
            }
        }
        private void titlebar_MouseLeftButtonDown ( object sender , System . Windows . Input . MouseButtonEventArgs e )
        {
            // Working 19/8/22
            if ( MouseCaptured )
                return;
            if ( ListResizing == true )
                return;
            TextBlock Titlebar = e . OriginalSource as TextBlock;
            MouseCaptured = this . CaptureMouse ( );
            ListMoving = true;
            ListResizing = false;
            // save left/top  offset of the mouse in Titlebar
            CpFirstXPos = e . GetPosition ( Titlebar ) . X;
            CpFirstYPos = e . GetPosition ( Titlebar ) . Y;
            //Mouse . OverrideCursor = Cursors . SizeAll;
            IsMouseIconNotArrow = true;

            //Debug . WriteLine ( $"Tbar MouseDown : top = {top}, left = {left}, offset X= {CpFirstXPos}, CpFirstYPos= {CpFirstYPos}" );
            return;
        }
        private void titlebar_MouseMove ( object sender , MouseEventArgs e )
        {
            // font family popup mousemove
            // Working just great 19/8/2022
            double vertposition = 0;
            double horposition = 0;
            if ( MouseCaptured == false )
            {
                return;
            }
            double ourheight = this . ActualHeight;
            double ourwidth = this . ActualWidth;
            if ( border == null ) border = DapperGenericsLib . Utils . FindVisualParent<Border> ( sender as DependencyObject );
            Canvas canvas = DapperGenericsLib . Utils . FindVisualParent<Canvas> ( border as DependencyObject );
            GenericGridControl ggrid = DapperGenericsLib . Utils . FindVisualParent<GenericGridControl> ( canvas as DependencyObject );
            if ( ggrid != null )
            {
                ggrid . canvas . Height = ggrid . ActualHeight;
                ggrid . canvas . Width = ggrid . ActualWidth;
            }
            double newtop = e . GetPosition ( canvas ) . Y - CpFirstYPos;
            double newleft = e . GetPosition ( canvas ) . X - CpFirstXPos;
            //Debug . WriteLine ( $"MOVING ENTRY = top= {top}, lleft= {left}" );
            // All working  correctly
            vertposition = newtop;
            horposition = left;
            if ( vertposition < 5 )
                vertposition = 0;
            else if ( ( vertposition + ( ourheight ) ) > canvas . ActualHeight )
                vertposition = canvas . ActualHeight - ( ourheight );
            else if ( vertposition < 0 )
                vertposition = 0;
            else if ( ( vertposition + ( ourheight ) ) > canvas . ActualHeight )
                vertposition = canvas . ActualHeight - ( ourheight );

            horposition = newleft;
            if ( horposition < 10 )
                horposition = 10;
            else if ( ( horposition + ( ourwidth ) ) > canvas . ActualWidth - 20 )
                horposition = canvas . ActualWidth - ( ourwidth + 20 );
            else if ( horposition < 0 )
                horposition = 0;
            else if ( ( horposition + ( ourwidth ) ) > canvas . ActualWidth )
                horposition = canvas . ActualWidth - ( ourwidth );

            //reset our running total positions
            CtrlTop = vertposition;
            CtrlTop = CtrlTop;
            left = horposition;
              CtrlLeft = horposition;
            ( this as FrameworkElement ) . SetValue ( Canvas . TopProperty , vertposition );
            ( this as FrameworkElement ) . SetValue ( Canvas . LeftProperty , horposition );
            //            Debug . WriteLine ( $"EXITING ENTRY = top= {vertposition}, left = {horposition}" );
        }
        private void titlebar_MouseLeftButtonUp ( object sender , System . Windows . Input . MouseButtonEventArgs e )
        {
            //Mouse . OverrideCursor = Cursors . Arrow;
            // Working 19/8/22
            ListMoving = false;
            if ( MouseCaptured == false && ListMoving == false && ListResizing == false )
            {
                counter = 0;
                return;
            }
            //           Debug . WriteLine ( $"Tbar Mouseup  top={top}, left={left}" );
            Isopen = false;
            counter = 0;
            this . ReleaseMouseCapture ( );
            MouseCaptured = false;
            // Clear flags
            ListMoving = false;
            ListMoving = true;
        }
        #endregion Font Listbox  (mainarea) move/sizing INNER

        #region Font Listbox  (border) move/sizing INNER
        private void border_MouseEnter ( object sender , MouseEventArgs e )
        {
            Canvas canvas = DapperGenericsLib . Utils . FindVisualParent<Canvas> ( sender as DependencyObject );
            Thickness th = new Thickness ( );
            th = border . BorderThickness;
            double borderwidth = th . Bottom + th . Top;
            double xoffset = e . GetPosition ( canvas ) . X;
            double yoffset = e . GetPosition ( canvas ) . Y;
            BorderSelected = 0;
            //           Debug . WriteLine ( $"Border MouseEnter top = {top}. yoffset = {yoffset} left = {left}, xoffset = {xoffset}" );

            //Check Horizontal position, we onlly accpet if we are >= 15 pixels in from left and <= 5 pixels in from right
            if ( xoffset >= left + 55 && xoffset <= xoffset + ( border . ActualWidth - 55 ) )
            {   // working 19/8/22
                //Check Vertical position
                if ( yoffset >= CtrlTop && yoffset <= ( CtrlTop + borderwidth ) )
                    BorderSelected = 5;  //Top horizontal border
                else if ( yoffset >= CtrlTop + ( border . ActualHeight - borderwidth ) && yoffset <= ( CtrlTop + CtrlTop + border . ActualHeight ) )
                    BorderSelected = 6;  //Bottom horizontal border

                if ( BorderSelected == 5 || BorderSelected == 6 )
                {
                    if ( Mouse . SetCursor ( Cursors . SizeNS ) )
                    {
                        border . Cursor = Cursors . SizeNS;
                        Mouse . OverrideCursor = border . Cursor;
                    }
                    return;
                }
            }
            if ( yoffset >= CtrlTop + ( borderwidth + 15 ) && CtrlTop <= CtrlTop + ( border . ActualHeight - 15 ) )
            {   // working 19/8/22
                // Horizontal border ??
                if ( xoffset >= left && xoffset <= left + borderwidth )
                    BorderSelected = 7; // LEFT
                else if ( xoffset >= left + border . ActualWidth && xoffset <= left + border . ActualWidth + borderwidth )
                    BorderSelected = 8; // RIGHT
                if ( BorderSelected == 7 || BorderSelected == 8 )
                {
                    if ( Mouse . SetCursor ( Cursors . SizeWE ) )
                    {
                        border . Cursor = Cursors . SizeNS;
                        Mouse . OverrideCursor = border . Cursor;
                    }
                    return;
                }
            }
            if ( IsMouseIconNotArrow == false && Mouse . SetCursor ( Cursors . Arrow ) )
            {
                border . Cursor = Cursors . Arrow;
                Mouse . OverrideCursor = border . Cursor;
            }
            //            Debug . WriteLine ( $"Border MouseEnter top = {top}. left = {left}" );
        }
        private void border_MouseLeave ( object sender , MouseEventArgs e )
        {
            if ( IsMouseIconNotArrow == false )
                Mouse . OverrideCursor = null;
            //            else IsMouseIconNotArrow = false;

            if ( e . LeftButton != MouseButtonState . Pressed && ListResizing == false )
            {
                this . ReleaseMouseCapture ( );
                MouseCaptured = false;
                ListResizing = false;
                //               Debug . WriteLine ( $"Border MouseLeave top = {top}. left = {left}" );
            }
        }
        private void LbSelector_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
        {
            GenericSelectBoxControl newborder = null;
            Border bdr = null;
            object original = e . OriginalSource;
            if ( original . GetType ( ) . Equals ( typeof ( Border ) ) )
            {
                bdr = original as Border;
                Debug . WriteLine ( $"{bdr?.Name . ToString ( )}" );
            }
            Canvas canvas = DapperGenericsLib . Utils . FindVisualParent<Canvas> ( sender as DependencyObject );
            // This is the mouse postion inside the Canvas - no relationship to what object we are in (LbSelector in fact)
            Debug . WriteLine ( $"{e . GetPosition ( canvas ) . Y . ToString ( )}" );
            // Debug . WriteLine ( $"Type = {v . ToString ( )}, name={v . Name . ToString ( )}" );
            //if ( original . GetType ( ) . Equals ( typeof ( Border ) ) && v . Name . ToString ( ) == "Border" )
            //   bdr = Utils . FindChild<Border> ( LbSelector, "border" );
            //    try {
            //        newborder = ( GenericSelectBoxControl ) original;
            //    }
            //    catch (Exception ex) { }
   
        }

        private void border_MouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
        {
            //return;

            Border border = sender as Border;
            if ( border == null )
                return;
            if ( e . LeftButton == MouseButtonState . Pressed && ListResizing == false && ListMoving == false )
                 IsMouseIconNotArrow = true;
            else
            {
                ListResizing = false;
                return;
            }
            //            Debug . WriteLine ( $"ENTRY L Button down: CpFirstXPos= {CpFirstXPos}  CpFirstYPos={CpFirstYPos}" );

            // TODO  jups to topleft of the canvas ????
            TextBlock Titlebar = e . OriginalSource as TextBlock;
            MouseCaptured = border . CaptureMouse ( );
            Isopen = true;
            // save left/top  offset of the mouse in Titlebar
            CpFirstXPos = e . GetPosition ( border ) . X;
            CpFirstYPos = e . GetPosition ( border ) . Y;
            BorderSelected = 5;
            borderbottom = CtrlTop + this . ActualHeight;
            Debug . WriteLine ( $"EXIT L Button down: CpFirstXPos= {CpFirstXPos}  CpFirstYPos={CpFirstYPos}" );
            ListResizing = true;
            ListMoving = false;
            return;

            {
                //TODO  make  it  border
                if ( GcCanvas == null ) GcCanvas = DapperGenericsLib . Utils . FindVisualParent<Canvas> ( sender as DependencyObject );
                double Left = e . GetPosition ( ( GcCanvas as FrameworkElement ) as FrameworkElement ) . X;
                double Top = e . GetPosition ( ( GcCanvas as FrameworkElement ) as FrameworkElement ) . Y;
                double Height = GenSelBox . ActualHeight;
                double Width = GenSelBox . ActualWidth;
                {
                    if ( Top >= CtrlTop && Top <= CtrlTop + borderThickness
                       && Left > left + 10 && Left <= left + border . ActualWidth - 10 )
                    { // over top border
                        //Mouse . OverrideCursor = Cursors . SizeNS;
                        BorderSelected = 5;
                    }
                    else if ( Top - CpFirstYPos >= CtrlTop + ( borderheight - borderThickness ) && Top - CpFirstYPos <= CtrlTop + borderheight + borderThickness
                        && Left > left + 10 && Left <= left + border . ActualWidth - 10 )
                    {    // Over botom border
                        //Mouse . OverrideCursor = Cursors . SizeNS;
                        BorderSelected = 6;
                    }
                    else if ( Top >= CtrlTop + 10 && Top <= CtrlTop + borderheight
                        && Left >= left && Left <= left + borderThickness )
                    {   // over  left border
                        //Mouse . OverrideCursor = Cursors . SizeWE;
                        BorderSelected = 7;
                    }
                    else if ( Top >= CtrlTop + 10 && Top <= CtrlTop + borderheight - 10
                        && Left >= left && Left <= +left + Width )
                    {   // over right border
                        //Mouse . OverrideCursor = Cursors . SizeWE;
                        BorderSelected = 8;
                    }
                }
            }
            {
                // Now handle resizing the top border
                //if ( this . BorderSelected == 5 && this . BorderSelected != -1 )
                //// && ( FlowdocResizing || this . BorderClicked && IsCornerDragging == false ) )
                //{
                //    // Get current sizes and position of GenListbox  window to intilize our calculations
                //    if ( FdLeft == 0 )
                //        FdLeft = Canvas . GetLeft ( this );
                //    if ( FdTop == 0 )
                //        FdTop = Canvas . GetTop ( this );
                //    FdHeight = this . ActualHeight;
                //    FdWidth = this . ActualWidth;
                //    //Get mouse cursor position
                //    Point pt = Mouse . GetPosition ( GcCanvas );
                //    double MLeft = pt . X;
                //    // Border Top position
                //    FdTop = pt . Y;
                //    FdBottom = FdTop + FdHeight;
                //    double ValidTop = FdBottom - ( 2 );
                //}
                //if ( this . BorderSelected == 1 )
                //{
                //    // Top border - WORKING CORRECTLY
                //    ////Mouse . OverrideCursor = Cursors . SizeNS;
                //    //Canvas . SetTop ( this , MTop );
                //    //YDiff = MTop - FdTop;
                //    //FdTop = MTop;

                //    //newHeight = FdHeight - YDiff;
                //    //if ( newHeight < 200 )
                //    //    newHeight = 200;
                //    //this . Height = newHeight;
                //    //if ( IsCornerDragging == true )
                //    //{
                //    //    // drag left as well
                //    //    XDiff = MLeft - FdLeft;
                //    //    newWidth = FdWidth - XDiff;
                //    //    if ( newWidth < 350 )
                //    //        newWidth = 350;
                //    //    this . Width = newWidth;
                //    //    Canvas . SetLeft ( this , MLeft );
                //    //    FdLeft = MLeft;
                //    //}
                //    return;
                //}
                {
                    //    else if ( Flowdoc . BorderSelected == 2 )
                    //    {     // Bottom border
                    //        //Mouse . OverrideCursor = Cursors . SizeNS;
                    //        newHeight = MTop - FdTop;
                    //        Flowdoc . Height = newHeight;
                    //        return;
                    //    }
                    //    else if ( Flowdoc . BorderSelected == 3 )
                    //    {   // Left hand side border  - WORKING CORRECTLY
                    //        //Mouse . OverrideCursor = Cursors . SizeWE;
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
        }

        private void border_MouseMove ( object sender , MouseEventArgs e )
        {
            double btop = 0, bheight = 0;
            double diff = 0;
            Canvas canvas = null;
            if ( MouseCaptured == false || ListResizing == false )
                return;
            // Get entire listbox control
            GenericSelectBoxControl lbSelector = DapperGenericsLib . Utils . FindVisualParent<GenericSelectBoxControl> ( sender as DependencyObject );
            GenericGridControl GenGrid = DapperGenericsLib . Utils . FindVisualParent<GenericGridControl> ( sender as DependencyObject );
            // Get host canvas
            canvas = DapperGenericsLib . Utils . FindVisualParent<Canvas> ( sender as DependencyObject );
            canvas . Height = GenGrid . ActualHeight;
            try
            {
                if ( this . BorderSelected == 5 && e . LeftButton == MouseButtonState . Pressed )
                {
                    // Get new Top/ left  position, e refers  to border, not this !
                    //lbSelector . Height += 10;
                     double borderMTop = e . GetPosition ( canvas ) . X;
                     PointHitTestResult ptresult = new PointHitTestResult (lbSelector, Mouse . GetPosition ( canvas ) );
                    borderMTop = ptresult . PointHit . Y;
                    Debug . WriteLine ( $"MTop = {borderMTop}" );
                     /*
                     * MTop = border . Top
                    top = this . top,, NOT border . Top
                    borderbottom = border . bottom
                    bottom = this . bottom
                    moving TOP border
                    */
                    // set control height
                     borderbottom = CtrlTop + border . ActualHeight;
                    if ( borderMTop < CtrlTop)
                    {
                        Debug . WriteLine ( $"Cursor has moved UP" );
                        diff = ( borderMTop - CtrlTop );  // top going up  + diff
                        borderheight = border . ActualHeight;
                        //borderbottom = CtrlTop + borderheight;
                        borderheight -= diff;
                        border . Height = borderheight;
                        lbSelector . Height = borderheight;
                        CtrlTop = borderMTop;
                    }
                    else if ( borderMTop > CtrlTop )
                    {   // top going down -diff
                        Debug . WriteLine ( $"Cursor has moved DOWN" );
                        diff = ( borderMTop - CtrlTop );
                        lbSelector . Height = border . ActualHeight - diff;
                        border.Height = border . ActualHeight - diff;
                        //border . Height = borderheight;
                        borderbottom = borderMTop + border . Height + diff;
                        CtrlTop = borderMTop;
                    }
                    //CtrlTop = borderMTop;                       //bottom -= diff * -1;
                    if ( diff == 0 )
                        return;
                    Debug . WriteLine ( $"diff = {diff}, top = {CtrlTop}" );
                    //border . Height = borderheight;
                    //Canvas . SetTop ( lbSelector , CtrlTop );
                    var vv =(double) lbSelector . GetValue ( CtrlTopProperty);
                    CtrlTop = vv;
                   // Canvas . SetBottom ( lbSelector , borderbottom );
                    // top = MTop;
                    //Canvas . SetBottom ( this . border , MTop + height );
                    Debug . WriteLine ( $"Exit  diff = {diff},Top = {CtrlTop}, height={borderheight}, bottom = {borderbottom}" );
                    //e . Handled = true;
                    Debug . WriteLine ( $"Height= {lbSelector.Height} ........................." );
                    return;


                    {
                        if ( GcCanvas == null ) GcCanvas = DapperGenericsLib . Utils . FindVisualParent<Canvas> ( sender as DependencyObject );
                        btop = e . GetPosition ( ( GcCanvas as FrameworkElement ) as FrameworkElement ) . Y;
                        Debug . WriteLine ( $"ENTRY moving TOP border : CtrlTop = {CtrlTop},  btop = {btop},Height={borderheight}, borderHeight={border . ActualHeight}" );
                        diff = btop - CtrlTop;
                        // Get border's top position
                        if ( diff > 0 )
                        {
                            bheight = borderheight - diff;
                            btop += diff;
                        }
                        else
                        {
                            bheight = borderheight + diff;
                            btop -= diff;
                        }
                        CtrlTop = btop;
                        borderheight = bheight;
                        border . Height = bheight;
                        Debug . WriteLine ( $"EXIT moving TOP border : CtrlTop =  {CtrlTop},  Height={borderheight}, borderHeight={border . ActualHeight}" );
                        //                   ( this as FrameworkElement ) . SetValue ( Canvas . LeftProperty , ( double ) horposition );
                        ( border as FrameworkElement ) . SetValue ( Canvas . TopProperty , ( double ) CtrlTop );
                        //e . Handled = true;
                    }
                }
            }
            catch ( Exception ex ) { Debug . WriteLine ( $"{ex . Message}" ); }
            {
                //                bleft = e . GetPosition ( ( GcCanvas as FrameworkElement ) as FrameworkElement ) . X;
                //               bwidth = width;
                //               left = bleft;
                //width = bwidth;
                //border . Width = bwidth;


                //Debug . WriteLine ( $"ENTRY Diff = {diff}" );
                //border . SetValue ( Canvas . BottomProperty , ( double ) top + border . Height );
                //border . SetValue ( Canvas . TopProperty , ( double ) top );
                //var bot = border . GetValue ( Canvas.BottomProperty );
                //Debug . WriteLine ( $"EXIT Diff = {diff}, CtrlTop= {CtrlTop}, Height={border . Height}" );
                // ( border as FrameworkElement ) . SetValue ( Canvas . BottomProperty , ( double ) top + border.Height );
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
                // uppdate global top position
                //               top = pt . Y;
                //( this as FrameworkElement ) . SetValue ( Canvas . BottomProperty , ( double )FdBottom  - FdTop );
            }
        }

        private void border_MouseLeftButtonUp ( object sender , MouseButtonEventArgs e )
        {
            if ( ListResizing == false )
                return;
            if ( GcCanvas == null )
                GcCanvas = DapperGenericsLib . Utils . FindVisualParent<Canvas> ( sender as DependencyObject );
            this . BorderSelected = -1;
            border . ReleaseMouseCapture ( );
            MouseCaptured = false;
            ListResizing = false;
            ListMoving = false;

            Mouse . OverrideCursor = Cursors . Arrow;
            //            Debug . WriteLine ( $"border Button up ......................" );
        }
        #endregion Font Listbox  (border) move/sizing

        #endregion Font Listbox move/sizing OUTER

        #region utilities

        #endregion utilities

        private void listbox_MouseDoubleClick ( object sender , MouseButtonEventArgs e )
        {
            selectbtn ( sender , e );
        }

        private void listbox_MouseDoubleClick_1 ( object sender , MouseButtonEventArgs e )
        {

        }

        private void MousetoArrow ( object sender , MouseEventArgs e )
        {
            if ( Mouse . SetCursor ( Cursors . Arrow ) == false )
                Debug . WriteLine ( $"Cursor to arrow failed !!!!!!!!!!!!!!!" );
        }

        private void LbSelector_LostFocus ( object sender , RoutedEventArgs e )
        {
            //         Mouse . OverrideCursor =null;
        }

 
    }
}
