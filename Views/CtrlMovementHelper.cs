using System;

using System . Diagnostics;

using System . Windows . Controls;
using System . Windows . Input;
using System . Windows;
using static Views . DragCtrlHelper;


namespace Views
{

    /// <summary>
    /// Class that provides built in support for dragging any control around inside a canvas
    /// NB the Control MUST be inside a Canvas control in the calling control...
    /// </summary>
    public partial class DragCtrlHelper
    {
        public Thickness GenTh = new ( 50.0 , 10.0 , 0 , 0 );
        public Thickness th = new ( );

        public struct DragData
        {
            public object MovingObject;
            public double currentleft;
            public double currenttop;
            public double FirstXPos;
            public double FirstYPos;
            public string CtrlName;
            public Control parent;
            public FrameworkElement CurrentControl;
            public double width;
            public double height;
            public double top;
            public double left;
            public bool Ismoving;
        }
        public  DragData CtrlDragData = new DragData ( );
 
        public object MovingObject = null;
        public double currentleft = 0.0;
        public double currenttop = 0.0;
        public double FirstXPos = 0.0;
        public double FirstYPos = 0.0;
        public string CtrlName = "";
        public Control parent = null;
        public FrameworkElement CurrentControl = null;
        public double width = 0.0, height = 0.0, top = 0.0, left = 0.0;
        public bool Ismoving = false;

        //***************************************************************************************************************//
        public void InitializeMovement ( FrameworkElement controlToMove , Window win )
        {
            CtrlDragData.CurrentControl = controlToMove;
            CtrlDragData . CurrentControl = controlToMove;
        }
        //***************************************************************************************************************//
        public void MovementStart ( object sender , MouseButtonEventArgs e , Window win = null )
        {
            if ( NewWpfDev . Utils . HitTestValidMoveType ( sender , e ) == true )
            {
                Ismoving = false; return;
            }
            if ( CtrlDragData . CurrentControl == null ) return;
             CtrlDragData . FirstXPos = e . GetPosition ( CtrlDragData . CurrentControl ) . X;
            CtrlDragData . FirstYPos = e . GetPosition ( CtrlDragData . CurrentControl ) . Y;
            CtrlDragData . left = e . GetPosition ( ( CtrlDragData . CurrentControl as FrameworkElement ) . Parent as FrameworkElement ) . X - CtrlDragData . FirstXPos;
            CtrlDragData . top = e . GetPosition ( ( CtrlDragData . CurrentControl as FrameworkElement ) . Parent as FrameworkElement ) . Y - CtrlDragData . FirstYPos;
            CtrlDragData . Ismoving = true;
            e . Handled = true;
        }
        //***************************************************************************************************************//
        public void CtrlMoving ( object sender , MouseEventArgs e , Window win =null)
        {
            //Debug . WriteLine ( $"{e . LeftButton == MouseButtonState . Pressed}" );
            if ( CtrlDragData . CurrentControl == null && e . LeftButton == MouseButtonState . Pressed ) return;
            Type type = sender . GetType ( );
            var child = GetChild ( ( DependencyObject ) sender , type );
            //Debug . WriteLine ( $"Moving a child = [ {child} ]" );
            if ( e . LeftButton == MouseButtonState . Pressed && CtrlDragData . Ismoving == true )
            {
                type = sender . GetType ( );
                CtrlDragData . height = CtrlDragData . CurrentControl . ActualHeight;
                CtrlDragData . width = CtrlDragData . CurrentControl . ActualWidth;
                CtrlDragData . left = e . GetPosition ( ( CtrlDragData . CurrentControl as FrameworkElement ) . Parent as FrameworkElement ) . X - CtrlDragData . FirstXPos;
                CtrlDragData . top = e . GetPosition ( ( CtrlDragData . CurrentControl as FrameworkElement ) . Parent as FrameworkElement ) . Y - CtrlDragData . FirstYPos;
                if ( CtrlDragData . left > 5 )
                    ( CtrlDragData . CurrentControl as FrameworkElement ) . SetValue ( Canvas . LeftProperty , CtrlDragData . left );
                if ( CtrlDragData . top > 0 )
                    ( CtrlDragData . CurrentControl as FrameworkElement ) . SetValue ( Canvas . TopProperty , CtrlDragData . top );
                //Debug . WriteLine($"Y top {Convert . ToDouble(top)} / {Convert . ToDouble(left)}");
            }
        }
        //***************************************************************************************************************//
        public void MovementEnd ( object sender , RoutedEventArgs e , Window win )
        {
            if ( CtrlDragData . CurrentControl == null ) return;
            CtrlDragData . Ismoving = false;
        }
        //***************************************************************************************************************//
        public static string GetChild ( DependencyObject obj , Type type )
        {
            var found = NewWpfDev . Utils . FindChild ( ( DependencyObject ) obj , type );
            type = found?.GetType ( );
            return type?.ToString ( );
        }
    }

}