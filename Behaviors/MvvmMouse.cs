using System;
using System . Collections . Generic;
using System . Diagnostics;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;

using NewWpfDev . ViewModels;


namespace NewWpfDev . Behaviors
{
    public class MvvmMouse
    {
        /// <summary>
        /// 
        /// Class to provide ICommand support for ALL mouse event handling
        /// called by GetMouseCommands(string mouse event type)
        /// RETURNS a RelayCommand for each type requested
        /// </summary>
        public MvvmMouse ( )
        {
        }
        public RelayCommand GetMouseCommands ( string mousetype )
        {
            switch ( mousetype )
            {
                case "UP":
                    return ( RelayCommand ) new RelayCommand ( ExecuteMouseUpCommand , CanExecuteMouseUpCommand );
#pragma warning disable CS0162 // Unreachable code detected
                    break;
#pragma warning restore CS0162 // Unreachable code detected
                case "DOWN":
                    return ( RelayCommand ) new RelayCommand ( ExecuteMouseDownCommand , CanExecuteMouseDownCommand );
#pragma warning disable CS0162 // Unreachable code detected
                    break;
#pragma warning restore CS0162 // Unreachable code detected
                case "ENTER":
                    return ( RelayCommand ) new RelayCommand ( ExecuteMouseEnterCommand , CanExecuteMouseEnterCommand );
#pragma warning disable CS0162 // Unreachable code detected
                    break;
#pragma warning restore CS0162 // Unreachable code detected
                case "LEAVE":
                    return ( RelayCommand ) new RelayCommand ( ExecuteMouseLeave , CanExecuteMouseLeaveCommand );
#pragma warning disable CS0162 // Unreachable code detected
                    break;
#pragma warning restore CS0162 // Unreachable code detected
                case "LEFTUP":
                    return ( RelayCommand ) new RelayCommand ( ExecuteMouseLeftButtonUpCommand , CanExcuteMouseLeftButtonUpCommand );
#pragma warning disable CS0162 // Unreachable code detected
                    break;
#pragma warning restore CS0162 // Unreachable code detected
                case "LEFTDOWN":
                    return ( RelayCommand ) new RelayCommand ( ExecuteMouseLeftButtonDownCommand , CanExecuteMouseLeftButtonDownCommand );
#pragma warning disable CS0162 // Unreachable code detected
                    break;
#pragma warning restore CS0162 // Unreachable code detected
                case "RIGHTUP":
                    return ( RelayCommand ) new RelayCommand ( ExecuteMouseRightButtonUpCommand , CanMouseRightButtonUpCommand );
#pragma warning disable CS0162 // Unreachable code detected
                    break;
#pragma warning restore CS0162 // Unreachable code detected
                case "RIGHTDOWN":
                    return ( RelayCommand ) new RelayCommand ( ExecuteMouseRightButtonDownCommand , CanMouseRightButtonDownCommand );
#pragma warning disable CS0162 // Unreachable code detected
                    break;
#pragma warning restore CS0162 // Unreachable code detected
                case "MOVE":
                    return ( RelayCommand ) new RelayCommand ( ExecuteMouseMoveCommand , CanExecuteMouseMoveCommand );
#pragma warning disable CS0162 // Unreachable code detected
                    break;
#pragma warning restore CS0162 // Unreachable code detected
                case "WHEEL":
                    return ( RelayCommand ) new RelayCommand ( ExecuteMouseWheelCommand , CanExecuteMouseWheelCommand );
#pragma warning disable CS0162 // Unreachable code detected
                    break;
#pragma warning restore CS0162 // Unreachable code detected
            }
            return null;
        }
        #region Mouse commands
        private void ExecuteMouseUpCommand ( object obj )
        {
            MouseButtonEventArgs args = obj as MouseButtonEventArgs;
            Debug. WriteLine ( $"Mouse Up hit ({args . ChangedButton}  )....." );
        }
        private void ExecuteMouseDownCommand ( object obj )
        {
            MouseButtonEventArgs args = obj as MouseButtonEventArgs;
            Debug. WriteLine ( $"Mouse Down hit ({args . ChangedButton}  )....." );
        }
        private void ExecuteMouseLeftButtonUpCommand ( object obj )
        {
            Debug. WriteLine ( $"Mouse Left Button Up hit....." );
        }
        private void ExecuteMouseLeftButtonDownCommand ( object obj )
        {
            Debug. WriteLine ( $"Mouse Left Button Down hit....." );
        }
        private void ExecuteMouseRightButtonUpCommand ( object obj )
        {
            MouseButtonEventArgs args = obj as MouseButtonEventArgs;
            Debug. WriteLine ( $"Mouse Right button Up hit....." );
        }
        private void ExecuteMouseRightButtonDownCommand ( object obj )
        {
            MouseButtonEventArgs args = obj as MouseButtonEventArgs;
            Debug. WriteLine ( $"Mouse Right button Down hit....." );
        }
        private void ExecuteMouseEnterCommand ( object obj )
        {
            MouseEventArgs args = obj as MouseEventArgs;
            Debug. WriteLine ( $"Mouse Enter hit ....." );
        }
        private void ExecuteMouseLeave ( object obj )
        {
            Debug. WriteLine ( $"Mouse Leave hit....." );
        }
        private void ExecuteMouseWheelCommand ( object obj )
        {
            MouseWheelEventArgs args = obj as MouseWheelEventArgs;
            Debug. WriteLine ( $"Mouse Wheel hit. {args . Delta}...." );
        }
        private void ExecuteMouseMoveCommand ( object obj )
        {
            MouseEventArgs args = obj as MouseEventArgs;
            //Debug. WriteLine ( $"Mouse Move identified  )....." );
        }


    private Point GetMousePosition ( object window , string mode = "SCREEN" )
    {
        var position = new Point ( );
        Window win = new Window ( );
        // Position of the mouse relative to the Screen
        // and allows for the window being moved around as well
        if ( mode . ToUpper ( ) == "SCREEN" )
        {
            win = window as Window;
            position = new Point ( Mouse . GetPosition ( win ) . X + win . Left , Mouse . GetPosition ( win ) . Y + win . Top );
            Debug. WriteLine ( $"Mouse to Screen X = {position . X}, Y = {position . Y}" );
        }
        // Position of the mouse relative to the window
        else if ( mode . ToUpper ( ) == "WINDOW" )
        {
            win = window as Window;
            position = Mouse . GetPosition ( win );
            Debug. WriteLine ( $"Mouse to Window  {win . ToString ( )} X = {position . X}, Y = {position . Y}" );
        }
        else if ( mode . ToUpper ( ) == "IMAGE" )
        {
            Image ctrl = window as Image;
            position = Mouse . GetPosition ( ctrl );
            Debug. WriteLine ( $"Mouse to Image {ctrl . ToString ( )} X = {position . X}, Y = {position . Y}" );
        }
        else
        {
            win = window as Window;
            // converts to screen position of specified window
            position = win . PointToScreen ( Mouse . GetPosition ( win ) );
            Debug. WriteLine ( $"Screen from Window {win . ToString ( )} X = {position . X}, Y = {position . Y}" );
        }
        // Add the window position
        return position;
        //            return new Point ( position . X + win . Left , position . Y + win . Top );
    }



    #endregion
    #region Mouse CanExecute
    private bool CanExecuteMouseUpCommand ( object arg )
    { return true; }
    private bool CanExecuteMouseDownCommand ( object arg )
    { return true; }
    private bool CanExcuteMouseLeftButtonUpCommand ( object arg )
    { return true; }
    private bool CanExecuteMouseLeftButtonDownCommand ( object arg )
    { return true; }
    private bool CanMouseRightButtonUpCommand ( object arg )
    { return true; }
    private bool CanMouseRightButtonDownCommand ( object arg )
    { return true; }
    private bool CanExecuteMouseEnterCommand ( object arg )
    { return true; }
    private bool CanExecuteMouseLeaveCommand ( object arg )
    { return true; }
    private bool CanExecuteMouseWheelCommand ( object arg )
    { return true; }
    private bool CanExecuteMouseMoveCommand ( object arg )
    { return true; }

    #endregion

}
}
