﻿using System;
using System . Diagnostics;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;
using System . Windows . Media;

using NewWpfDev . Models;
using NewWpfDev . UserControls;

using Canvas = System . Windows . Controls . Canvas;

namespace NewWpfDev . Views
{
    /// <summary>
    /// Interaction logic for GenericSelectBoxControl.xaml
    /// </summary>
    public partial class GenericSelectBoxControl : UserControl
    {
        #region Properties

        public static event EventHandler<SelectionArgs> ListSelection;

        public static Thickness Ctrlstats = new Thickness ( );
        //public static GenericSelectBoxControl GenSelectBox { get; set; } = null;
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
        //public double BorderSizingTop { get; set; } = 0;
        public double FdTop { get; set; } = 0;
        public double FdBottom { get; set; } = 0;
        public double FdHeight { get; set; } = 0;
        public double FdWidth { get; set; } = 0;
        public int BorderSelected { get; set; } = 0;
        public static bool Isopen { get; set; } = false;
        public UIElement UcHostElement;
        public Control UcHostControl;
        public string UcHostName;
        public string UcHostType;
        public Canvas fdlcanvas;
        int counter { get; set; } = 0;

        #endregion Properties
        public void SetHost ( Canvas Fdlcanvas = null )
        {
            fdlcanvas = Fdlcanvas;
        }
        public GenericSelectBoxControl ( Control fdparentl , Canvas canvas )
        {
            InitializeComponent ( );
            fdlcanvas = canvas;
            // this is the DP for LBSELECTOR (Name for the entire control)
            GenericSelectBoxControl gsb = this;
            if ( fdlcanvas == null )
            {
                ( gsb as FrameworkElement ) . SetValue ( Canvas . LeftProperty , ( double ) 100 );
                ( gsb as FrameworkElement ) . SetValue ( Canvas . TopProperty , ( double ) 150 );
                this . SetValue ( GenSelectBoxProperty , gsb );
            }
            else
            {
                ( fdlcanvas as FrameworkElement ) . SetValue ( Canvas . LeftProperty , ( double ) 10 );
                ( fdlcanvas as FrameworkElement ) . SetValue ( Canvas . TopProperty , ( double ) 15 );
                // this . SetValue ( GenSelectBoxProperty , fdlcanvas );
            }
            BorderMLeft = 100;
            border . Width = 300;
            border . Height = 400;
            Ctrlstats . Top = 150;
            CtrlTop = 150;

            // set our controlling DP's
            BorderMTop = CtrlTop;
            BorderMHeight = border . Height;
            BorderMWidth = 300;
            BorderMBottom = CtrlTop + border . Height;

            Ctrlstats . Bottom = border . Height;
            if ( fdlcanvas == null )
            {
                GenSelectBox . Width = border . Width;
                GenSelectBox . Height = border . Height;
                BorderSizingTop = CtrlTop;
                BorderSizingBottom = Ctrlstats . Top + GenSelectBox . Height;
                gsb . SetValue ( IsVisibleProperty , true );
                Listviews . SetListboxHost += Listbox_SetListboxHost;
                GenericGridControl . SetListboxHost += Listbox_SetListboxHost;
                FourwaySplitViewer . SetListboxHost += Listbox_SetListboxHost;
            }
            Isopen = true;
            this . Focus ( );
            // Configure ALL subscriiptions to our popup Event
        }

        public void Listbox_SetListboxHost ( object sender , ListboxHostArgs e )
        {
            UcHostElement = e . HostUIElement;
            UcHostControl = e . HostControl;
            UcHostName = e . HostName;
            UcHostType = e . HostType;
        }

        private void GenericSelectBoxControl_IsVisibleChanged ( object sender , DependencyPropertyChangedEventArgs e )
        {
            return;
        }
        private void IsLoaded ( object sender , RoutedEventArgs e )
        {
            Isopen = true;
            ( fdlcanvas as FrameworkElement ) . SetValue ( Canvas . LeftProperty , ( double ) 10 );
            ( fdlcanvas as FrameworkElement ) . SetValue ( Canvas . TopProperty , ( double ) 10 );
            //( GenSelectBox as FrameworkElement ) . SetValue ( Canvas . LeftProperty , ( double ) 100 );
            //( GenSelectBox as FrameworkElement ) . SetValue ( Canvas . TopProperty , ( double ) 150 );
            border . Width = 300;
            border . Height = 400;
            Ctrlstats . Top = 150;
            CtrlTop = 150;

            // set our controlling DP's
            BorderMTop = CtrlTop;
            BorderMHeight = border . Height;
            BorderMWidth = 300;
            BorderMBottom = CtrlTop + border . Height;

            Ctrlstats . Bottom = border . Height;
            GenSelectBox . Width = border . Width;
            GenSelectBox . Height = border . Height;
            BorderSizingTop = Ctrlstats . Top;
            BorderSizingBottom = Ctrlstats . Top + GenSelectBox . Height;
            // Get our parent Canvas
            //fdlcanvas
            GenCtrl = DapperGenericsLib . Utils . FindVisualParent<GenericGridControl> ( sender as DependencyObject );
            if ( GcCanvas == null )
                GcCanvas = DapperGenericsLib . Utils . FindVisualParent<Canvas> ( sender as DependencyObject );
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
            string newfont = listbox . SelectedItem . ToString ( );
            Properties . Settings . Default . FlowDocFont = newfont;
            Properties . Settings . Default . Save ( );

            fdl . fontFamily = new FontFamily ( listbox . SelectedItem . ToString ( ) );
            fdl . UpdateFontDisplay ( );

            this . Visibility = Visibility . Collapsed;
        }

        #region Font Listbox  move/sizing OUTER

        #region Font Listbox  (mainarea) move/sizing INNER

        private void titlebar_MouseEnter ( object sender , MouseEventArgs e )
        {
            if ( Mouse . SetCursor ( Cursors . SizeAll ) )
            {
                titlebar . Cursor = Cursors . SizeAll;
                Mouse . OverrideCursor = titlebar . Cursor;
            }
            IsMouseIconNotArrow = true;
            this . BorderSelected = 0;

        }
        private void titlebar_MouseLeave ( object sender , MouseEventArgs e )
        {
            IsMouseIconNotArrow = false;
            if ( Mouse . SetCursor ( Cursors . Arrow ) )
            {
                titlebar . Cursor = Cursors . Arrow;
                Mouse . OverrideCursor = titlebar . Cursor;
            }
            this . BorderSelected = 0;
        }
        private void titlebar_MouseLeftButtonDown ( object sender , System . Windows . Input . MouseButtonEventArgs e )
        {
            // Working 19/8/22
            if ( MouseCaptured )
                return;
            if ( ListResizing == true )
                return;
            TextBlock Titlebar = e . OriginalSource as TextBlock;
            MouseCaptured = Titlebar . CaptureMouse ( );
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
            left = horposition;
            BorderMLeft = horposition;
            ( this as FrameworkElement ) . SetValue ( Canvas . TopProperty , vertposition );
            ( this as FrameworkElement ) . SetValue ( Canvas . LeftProperty , horposition );
            CtrlTop = vertposition - ( border . BorderThickness . Top / 2 );
            BorderSizingTop = CtrlTop;
            BorderSizingBottom = CtrlTop + ourheight;

            // set our controlling DP's after moving
            BorderMTop = vertposition;
            BorderMBottom = BorderMTop + this . Height;
        }

        private void titlebar_MouseLeftButtonUp ( object sender , System . Windows . Input . MouseButtonEventArgs e )
        {
            if ( Mouse . SetCursor ( Cursors . Arrow ) )
            {
                titlebar . Cursor = Cursors . Arrow;
                Mouse . OverrideCursor = titlebar . Cursor;
            }
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
            TextBlock Titlebar = e . OriginalSource as TextBlock;
            if ( Titlebar == null )
            {
                Border border = e . OriginalSource as Border;
                if ( border != null )
                    border . ReleaseMouseCapture ( );
            }
            else
                Titlebar . ReleaseMouseCapture ( );
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
            double borderwidth = th . Top;
            double xoffset = e . GetPosition ( canvas ) . X;
            double yoffset = e . GetPosition ( canvas ) . Y;
            this . BorderSelected = 0;
            //Check Horizontal position, we onlly accpet if we are >= 15 pixels in from left and <= 5 pixels in from right
            if ( xoffset >= BorderMLeft + 15 && xoffset <= BorderMLeft + ( border . ActualWidth - 15 ) )
            {   // working 19/8/22
                //   check for Top Horizontal border 1st
                //Check Vertical position
                if ( yoffset >= BorderMTop && yoffset <= ( BorderMTop + borderwidth ) )
                    this . BorderSelected = 5;  //Top horizontal border
                else if ( yoffset >= BorderMTop + ( border . ActualHeight - borderwidth ) && yoffset <= ( BorderMTop + BorderMTop + border . ActualHeight ) )
                    this . BorderSelected = 6;  //Bottom horizontal border

                //              Debug . WriteLine ( $"selection is {this . BorderSelected}" );
                if ( this . BorderSelected == 5 || this . BorderSelected == 6 )
                {
                    if ( Mouse . SetCursor ( Cursors . SizeNS ) )
                    {
                        border . Cursor = Cursors . SizeNS;
                        Mouse . OverrideCursor = border . Cursor;
                    }
                    //this . Focus ( );
                    return;
                }
            }
            // Finished testing for the horizontal borders, so Check vertical borders position next
            if ( yoffset >= BorderMTop + ( 10 ) && yoffset <= BorderMTop + ( border . ActualHeight - 10 ) )
            {
                if ( xoffset >= BorderMLeft && xoffset <= BorderMLeft + borderwidth )
                    this . BorderSelected = 7; // LEFT
                else if ( xoffset > BorderMLeft + BorderMWidth - borderwidth && xoffset <= BorderMLeft + BorderMWidth )
                    this . BorderSelected = 8; // RIGHT
                Debug . WriteLine ( $"selection is {this . BorderSelected}" );
                if ( this . BorderSelected == 7 || this . BorderSelected == 8 )
                {
                    if ( Mouse . SetCursor ( Cursors . SizeWE ) )
                    {
                        border . Cursor = Cursors . SizeWE;
                        Mouse . OverrideCursor = border . Cursor;
                    }
                    this . Focus ( );
                    return;
                }
            }
            //if ( IsMouseIconNotArrow == false && Mouse . SetCursor ( Cursors . Arrow ) )
            //{
            //    border . Cursor = Cursors . Arrow;
            //    Mouse . OverrideCursor = border . Cursor;
            //}
            //            Debug . WriteLine ( $"Border MouseEnter top = {top}. left = {left}" );
        }
        private void border_MouseLeave ( object sender , MouseEventArgs e )
        {
            //            if ( IsMouseIconNotArrow == false )
            if ( Mouse . SetCursor ( Cursors . Arrow ) )
            {
                border . Cursor = Cursors . Arrow;
                Mouse . OverrideCursor = border . Cursor;
            }
            MouseCaptured = false;
            ListResizing = false;
            this . BorderSelected = 0;
            //            else IsMouseIconNotArrow = false;

            if ( e . LeftButton != MouseButtonState . Pressed && ListResizing == false )
            {
                if ( this . IsMouseCaptured == true )
                    this . ReleaseMouseCapture ( );
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
        }

        private void border_MouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
        {
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
            this . Focus ( );
            Thickness th = ( Thickness ) border . GetValue ( BorderThicknessProperty );
            borderThickness = th . Top;

            TextBlock Titlebar = e . OriginalSource as TextBlock;
            GenericSelectBoxControl lbSelector = DapperGenericsLib . Utils . FindVisualParent<GenericSelectBoxControl> ( sender as DependencyObject );
            Canvas canvas = DapperGenericsLib . Utils . FindVisualParent<Canvas> ( sender as DependencyObject );
            BorderSizingTop = e . GetPosition ( canvas ) . Y;
            BorderSizingBottom = BorderSizingTop + lbSelector . Height;
            MouseCaptured = border . CaptureMouse ( );
            Isopen = true;
            // save left/top  offset of the mouse in Titlebar
            CpFirstXPos = e . GetPosition ( border ) . X;
            CpFirstYPos = e . GetPosition ( border ) . Y;
            Debug . WriteLine ( $"EXIT L Button down: CpFirstXPos= {CpFirstXPos}  CpFirstYPos={CpFirstYPos}" );
            ListResizing = true;
            ListMoving = false;
            //TODO  make  it  border
            if ( GcCanvas == null )
                GcCanvas = DapperGenericsLib . Utils . FindVisualParent<Canvas> ( sender as DependencyObject );
            double Left = e . GetPosition ( ( GcCanvas as FrameworkElement ) as FrameworkElement ) . X;
            double Top = e . GetPosition ( ( GcCanvas as FrameworkElement ) as FrameworkElement ) . Y;
            double Height = GenSelectBox . ActualHeight;
            double Width = GenSelectBox . ActualWidth;
            {
                if ( Top >= BorderMTop && Top <= BorderMTop + borderThickness
                   && Left > BorderMLeft + 10 && Left <= left + border . ActualWidth - 10 )
                { // over top border
                    this . BorderSelected = 5;
                    return;
                }
                else if ( Top - CpFirstYPos >= BorderMTop + ( borderheight - borderThickness ) && Top - CpFirstYPos <= BorderMTop + borderheight + borderThickness
                    && Left > BorderMLeft + 10 && Left <= BorderMLeft + border . ActualWidth - 10 )
                {    // Over bottom border
                    this . BorderSelected = 6;
                    return;
                }
                else if ( Top >= BorderMTop + 15 && Top <= BorderMTop + BorderMHeight + borderThickness
                    && Left >= BorderMLeft && Left <= BorderMLeft + borderThickness )
                {   // over  left border
                    this . BorderSelected = 7;
                    return;
                }
                else if ( Top >= BorderMTop + 15 && Top <= BorderMTop + BorderMHeight + borderThickness
                    && Left >= BorderMLeft + ( BorderMWidth - borderThickness ) && Left <= BorderMLeft + BorderMWidth )
                {   // over right border
                    this . BorderSelected = 8;
                    return;
                }
            }
        }

        private void GenLbControl_MouseMove ( object sender , MouseEventArgs e )
        {
            //Type type = sender . GetType ( );
            //if ( typeof ( Border ) == type )
            //{
            //    Border bdr = e . OriginalSource as Border;
            //    if ( bdr . Name == "border" )
            //        Debug . WriteLine ( $"GenLbControl_MouseMove  triggered {type . Name . ToString ( )}........" );
            //}
        }
        private void border_MouseMove ( object sender , MouseEventArgs e )
        {
            //  FINALLY ALL WORKING 21/8/22
            double diff = 0;
            Canvas canvas = null;
            // Get entire listbox control
            GenericSelectBoxControl lbSelector = DapperGenericsLib . Utils . FindVisualParent<GenericSelectBoxControl> ( sender as DependencyObject );
            canvas = DapperGenericsLib . Utils . FindVisualParent<Canvas> ( sender as DependencyObject );
            //GenericGridControl GenGrid = DapperGenericsLib . Utils . FindVisualParent<GenericGridControl> ( sender as DependencyObject );
            // Get host canvas
            //var Host = DapperGenericsLib . Utils . FindVisualParent<Control> ( UcHostWindow as DependencyObject );
            if ( UcHostControl != null )
                canvas . Height = ( double ) UcHostControl . GetValue ( HeightProperty );
            if ( UcHostControl == null ) return;
            if ( this . IsFocused == false )
                this . Focus ( );
            try
            {   // WORKING 21/8/22
                if ( this . BorderSelected == 5 && e . LeftButton == MouseButtonState . Pressed )
                {
                    // Get new Top/ left  position, e refers  to border, not this !
                    double borderMTop = e . GetPosition ( canvas ) . Y;
                    PointHitTestResult ptresult = new PointHitTestResult ( lbSelector , Mouse . GetPosition ( canvas ) );
                    borderMTop = ptresult . PointHit . Y;
                    Debug . WriteLine ( $"\n(1) -- > BorderMTop= {BorderMTop} -> current top = {borderMTop}" );
                    if ( BorderMTop > borderMTop )
                    {// Top moving UP - WORKING 21 / 8 / 22
                        Debug . WriteLine ( $"(2) Cursor has moved UP" );
                        diff = ( BorderMTop - borderMTop );
                        if ( BorderMHeight - diff > 150 )
                        {
                            lbSelector . Height = lbSelector . ActualHeight + diff;
                            border . Height = lbSelector . Height;
                        }
                    }
                    else if ( borderMTop > BorderMTop )
                    {   // top going down -diff - WORKING 21/8/22
                        Debug . WriteLine ( $"(2) Cursor has moved DOWN" );
                        diff = ( borderMTop - BorderMTop );
                        if ( BorderMHeight - diff > 250 )
                        {
                            lbSelector . Height = lbSelector . ActualHeight - diff;
                            border . Height = lbSelector . Height;
                        }
                        else return;
                    }
                    if ( diff == 0 )
                        return;

                    // WORKING 21 / 8 / 22
                    Canvas . SetTop ( lbSelector , borderMTop );

                    Debug . WriteLine ( $"(3) diff = {diff}" );
                    Debug . WriteLine ( $"(4) -- > new Top = {borderMTop} :  BorderMTop= {BorderMTop} -> Ctrl. Height = {lbSelector . Height}" );
                    if ( MouseCaptured == true || ListResizing == true )
                    {
                        // set our controlling DP's
                        BorderMTop = borderMTop;
                        BorderMHeight = lbSelector . Height;
                        BorderMBottom = BorderMTop + BorderMHeight;
                    }
                    return;
                }
                else if ( this . BorderSelected == 6 && e . LeftButton == MouseButtonState . Pressed )
                {
                    // Bottom border moving
                    double borderMBottom = e . GetPosition ( canvas ) . Y;
                    PointHitTestResult ptresult = new PointHitTestResult ( lbSelector , Mouse . GetPosition ( canvas ) );
                    borderMBottom = ptresult . PointHit . Y;
                    //Debug . WriteLine ( $"\n(1) -- > BorderSizingBottom = {BorderSizingBottom} :  CtrlBottom= {CtrlBottom} -> BorderMBottom: Ctrl. Height = {CtrlBottom + lbSelector . Height}" );
                    if ( borderMBottom > BorderMBottom )
                    {   // bottom going down -diff - Height increasing
                        if ( this . IsFocused == false )
                            this . Focus ( );
                        Debug . WriteLine ( $"(2) Cursor has moved DOWN" );
                        diff = ( borderMBottom - BorderMBottom );  // top going up  + diff
                        lbSelector . Height += diff;
                        border . Height += diff;
                        //CtrlBottom = BorderMBottom;
                    }
                    else if ( BorderMBottom > borderMBottom )
                    {   // bottom going up +diff Height decreasing
                        Debug . WriteLine ( $"(2) Cursor has moved UP" );
                        diff = BorderMBottom - borderMBottom;
                        if ( BorderMHeight - diff > 250 )
                        {
                            lbSelector . Height -= diff;
                            border . Height -= diff;
                        }
                    }
                    if ( diff == 0 )
                        return;

                    Canvas . SetTop ( lbSelector , BorderMTop );
                    Debug . WriteLine ( $"(3) diff = {diff}" );
                    Canvas . SetBottom ( lbSelector , BorderMTop + lbSelector . Height );
                    if ( MouseCaptured == true || ListResizing == true )
                    {   // set our controlling DP's
                        BorderMHeight = lbSelector . Height;
                        BorderMBottom = BorderMTop + lbSelector . Height;
                    }
                    Debug . WriteLine ( $"(4) -- > BordeMTop = {BorderMTop} :  BorderMBottom: {BorderMBottom}, BorderMHeight : {BorderMHeight}" );
                    this . Focus ( );
                    return;
                }
                else if ( this . BorderSelected == 7 && e . LeftButton == MouseButtonState . Pressed )
                {
                    // LEFT  border moving
                    double borderLeft = e . GetPosition ( canvas ) . X;
                    PointHitTestResult ptresult = new PointHitTestResult ( lbSelector , Mouse . GetPosition ( canvas ) );
                    borderLeft = ptresult . PointHit . X;
                    if ( borderLeft > BorderMLeft )
                    {   // left going right -diff - Width decreasing
                        if ( this . IsFocused == false )
                            this . Focus ( );
                        Debug . WriteLine ( $"(2) Cursor has moved LEFT" );
                        diff = ( borderLeft - BorderMLeft );  // left going right  - Width decreasing 
                        if ( border . Width - diff > 300 )
                        {
                            lbSelector . Width -= diff;
                            border . Width -= diff;
                            BorderMLeft = borderLeft;
                        }
                    }
                    else if ( BorderMLeft > borderLeft )
                    {   // // left going left -diff - Width increasing
                        Debug . WriteLine ( $"(2) Cursor has moved RIGHT" );
                        diff = BorderMLeft - borderLeft;
                        if ( borderLeft > 0 )
                        {
                            BorderMLeft = borderLeft;
                            lbSelector . Width += diff;
                            border . Width += diff;
                        }
                    }
                    if ( diff == 0 )
                        return;

                    Canvas . SetLeft ( lbSelector , BorderMLeft );

                    Debug . WriteLine ( $"(3) diff = {diff}" );
                    Canvas . SetRight ( lbSelector , BorderMLeft + lbSelector . Width );
                    BorderMWidth = lbSelector . Width;
                    Debug . WriteLine ( $"(4) -- > BorderMLeft = {BorderMLeft} :  BorderMWidth : {BorderMWidth}" );
                    this . Focus ( );
                    return;
                }
                else if ( this . BorderSelected == 8 && e . LeftButton == MouseButtonState . Pressed )
                {
                    // RIGHT  border moving
                    Thickness th = border . BorderThickness;
                    double borderwidth = th . Right;
                    double borderRight = e . GetPosition ( canvas ) . X;
                    PointHitTestResult ptresult = new PointHitTestResult ( lbSelector , Mouse . GetPosition ( canvas ) );
                    borderRight = ptresult . PointHit . X;
                    double rightborderleft = BorderMLeft + ( BorderMWidth - borderwidth );
                    double rightborderright = BorderMLeft + BorderMWidth;
                    if ( borderRight >= rightborderleft && borderRight <= rightborderright )
                        return; /// only in the border itself, so abort as we do not know which way to move
                    if ( borderRight > rightborderright )
                    {   // right going right -diff - Width increasing
                        if ( this . IsFocused == false )
                            this . Focus ( );
                        Debug . WriteLine ( $"(2) Cursor has moved LEFT" );
                        diff = ( borderRight - ( BorderMLeft + BorderMWidth ) );  // top going up  + diff
                        lbSelector . Width += diff;
                        border . Width += diff;
                        BorderMWidth += diff;
                    }
                    else if ( borderRight < rightborderleft )
                    {   // // right going left -diff - Width decreasing
                        Debug . WriteLine ( $"(2) Cursor has moved RIGHT" );
                        diff = ( BorderMLeft + BorderMWidth ) - borderRight;
                        if ( borderRight > BorderMLeft + 300 )
                        {   //BorderMLeft = borderRight;
                            lbSelector . Width -= diff;
                            border . Width -= diff;
                            BorderMWidth -= diff;
                        }
                    }
                    if ( diff == 0 )
                        return;

                    Canvas . SetLeft ( lbSelector , BorderMLeft );

                    Debug . WriteLine ( $"(3) diff = {diff}" );
                    Canvas . SetRight ( lbSelector , borderRight );
                    BorderMWidth = lbSelector . Width;
                    Debug . WriteLine ( $"(4) -- > BorderMLeft = {BorderMLeft} :  BorderMWidth : {BorderMWidth}" );
                    this . Focus ( );
                    return;
                }
            }
            catch ( Exception ex ) { Debug . WriteLine ( $"{ex . Message}" ); }
            {
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
            Control elm = sender as Control;
            if ( Mouse . SetCursor ( Cursors . Arrow ) )
            {
                this . Cursor = Cursors . Arrow;
                Mouse . OverrideCursor = this . Cursor;
            }
        }

        private void LbSelector_LostFocus ( object sender , RoutedEventArgs e )
        {
        }

        #region Dependency Properties

        //*******************************************************************************************//
        new public bool IsVisible
        {
            get { return ( bool ) GetValue ( IsVisibleProperty ); }
            set { SetValue ( IsVisibleProperty , value ); }
        }
        new public static readonly DependencyProperty IsVisibleProperty =
            DependencyProperty . Register ( "IsVisible" , typeof ( bool ) , typeof ( GenericSelectBoxControl ) , new PropertyMetadata ( false ) );
        //*******************************************************************************************//
        public GenericSelectBoxControl GenSelectBox
        {
            get { return ( GenericSelectBoxControl ) GetValue ( GenSelectBoxProperty ); }
            set { SetValue ( GenSelectBoxProperty , value ); }
        }
        public static readonly DependencyProperty GenSelectBoxProperty =
            DependencyProperty . Register ( "GenSelectBox" , typeof ( GenericSelectBoxControl ) , typeof ( GenericSelectBoxControl ) , new PropertyMetadata ( default ) );
        //*******************************************************************************************//
        public GenericGridControl GenCtrl
        {
            get { return ( GenericGridControl ) GetValue ( GenCtrlProperty ); }
            set { SetValue ( GenCtrlProperty , value ); }
        }
        public static readonly DependencyProperty GenCtrlProperty =
            DependencyProperty . Register ( "GenCtrl" , typeof ( GenericGridControl ) , typeof ( GenericSelectBoxControl ) , new PropertyMetadata ( ( GenericGridControl ) null ) );
        //*******************************************************************************************//
        public double CtrlTop
        {
            get { return ( double ) GetValue ( CtrlTopProperty ); }
            set { SetValue ( CtrlTopProperty , value ); }
        }
        public static readonly DependencyProperty CtrlTopProperty =
            DependencyProperty . Register ( "CtrlTop" , typeof ( double ) , typeof ( GenericSelectBoxControl ) , new PropertyMetadata ( ( double ) 150 ) );
        //*******************************************************************************************//
        public double CtrlBottom
        {
            get { return ( double ) GetValue ( CtrlBottomProperty ); }
            set { SetValue ( CtrlBottomProperty , value ); }
        }
        public static readonly DependencyProperty CtrlBottomProperty =
            DependencyProperty . Register ( "CtrlBottom" , typeof ( double ) , typeof ( GenericSelectBoxControl ) , new PropertyMetadata ( ( double ) 450 ) );
        //*******************************************************************************************//
        public double BorderMLeft
        {
            get { return ( double ) GetValue ( CtrlLeftProperty ); }
            set { SetValue ( CtrlLeftProperty , value ); }
        }
        public static readonly DependencyProperty CtrlLeftProperty =
            DependencyProperty . Register ( "BorderMLeft" , typeof ( double ) , typeof ( GenericSelectBoxControl ) , new PropertyMetadata ( ( double ) 150 ) );
        //*******************************************************************************************//
        public double BorderSizingTop
        {
            get { return ( double ) GetValue ( BorderSizingTopProperty ); }
            set { SetValue ( BorderSizingTopProperty , value ); }
        }
        public static readonly DependencyProperty BorderSizingTopProperty =
            DependencyProperty . Register ( "BorderSizingTop" , typeof ( double ) , typeof ( GenericSelectBoxControl ) , new PropertyMetadata ( ( double ) 150 ) );
        //*******************************************************************************************//
        public double BorderSizingBottom
        {
            get { return ( double ) GetValue ( BorderSizingBottomProperty ); }
            set { SetValue ( BorderSizingBottomProperty , value ); }
        }
        public static readonly DependencyProperty BorderSizingBottomProperty =
            DependencyProperty . Register ( "BorderSizingBottom" , typeof ( double ) , typeof ( GenericSelectBoxControl ) , new PropertyMetadata ( ( double ) 450 ) );
        //*******************************************************************************************//
        public double BorderMTop
        {
            get { return ( double ) GetValue ( BorderMTopProperty ); }
            set { SetValue ( BorderMTopProperty , value ); }
        }
        public static readonly DependencyProperty BorderMTopProperty =
            DependencyProperty . Register ( "BorderMTop" , typeof ( double ) , typeof ( GenericSelectBoxControl ) , new PropertyMetadata ( ( double ) 150 ) );
        //*******************************************************************************************//
        public double BorderMBottom
        {
            get { return ( double ) GetValue ( BorderMBottomProperty ); }
            set { SetValue ( BorderMBottomProperty , value ); }
        }
        public static readonly DependencyProperty BorderMBottomProperty =
            DependencyProperty . Register ( "BorderMBottom" , typeof ( double ) , typeof ( GenericSelectBoxControl ) , new PropertyMetadata ( ( double ) 450 ) );
        //*******************************************************************************************//
        public double BorderMHeight
        {
            get { return ( double ) GetValue ( BorderMHeightProperty ); }
            set { SetValue ( BorderMHeightProperty , value ); }
        }
        public static readonly DependencyProperty BorderMHeightProperty =
            DependencyProperty . Register ( "BorderMHeight" , typeof ( double ) , typeof ( GenericSelectBoxControl ) , new PropertyMetadata ( ( double ) 300 ) );
        //*******************************************************************************************//
        public double BorderMWidth
        {
            get { return ( double ) GetValue ( BorderMWidthProperty ); }
            set { SetValue ( BorderMWidthProperty , value ); }
        }
        public static readonly DependencyProperty BorderMWidthProperty =
            DependencyProperty . Register ( "BorderMWidth" , typeof ( double ) , typeof ( GenericSelectBoxControl ) , new PropertyMetadata ( ( double ) 300 ) );
        //*******************************************************************************************//

        #endregion Dependency Properties

        private void bordergrid_MouseLeave ( object sender , MouseEventArgs e )
        {
            this . ReleaseMouseCapture ( );
            MouseCaptured = false;
            ListMoving = false;
        }
    }
}
//public class ListboxHostArgs : EventArgs
//{
//    public UIElement HostUIElement;
//    public Control HostControl;
//    public string HostName;
//    public string HostType;
//}