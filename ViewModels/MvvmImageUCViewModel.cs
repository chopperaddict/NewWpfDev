using System;
using System . Collections . Generic;
using System . Linq;
using System . Runtime . InteropServices;
using System . Text;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;

using NewWpfDev . Behaviors;
using NewWpfDev . UserControls;



namespace NewWpfDev . ViewModels
{
    public class MvvmImageUCViewModel
    {
#pragma warning disable CS0169 // The field 'MvvmImageUCViewModel.obj' is never used
        private EventHandler<Image> obj;
#pragma warning restore CS0169 // The field 'MvvmImageUCViewModel.obj' is never used

        public ICommand CloseUControl4 { get; set; }
        public ICommand MouseUpCommand { get; set; }
        public ICommand MouseDownCommand { get; set; }
        public ICommand MouseEnterCommand { get; set; }
        public ICommand MouseLeaveCommand { get; set; }
        public ICommand MouseLeftButtonDownCommand { get; set; }
        public ICommand MouseLeftButtonUpCommand { get; set; }
        public ICommand MouseMoveCommand { get; set; }
        public ICommand MouseRightButtonUpCommand { get; set; }
        public ICommand MouseRightButtonDownCommand { get; set; }
        public ICommand MouseWheelCommand { get; set; }
        public static MvvmBrowserUC uc4 { get; set; }
        public static MvvmContainerViewModel mcvm { set; get; }
        public MvvmImageUCViewModel ( )
        {
            MvvmMouse mousehandler = new MvvmMouse ( );
            mcvm = MvvmContainerViewModel . GetMvvmContainerViewModel ( );

            // get all mouse handlers
            MouseUpCommand = ( RelayCommand ) mousehandler . GetMouseCommands ( "UP" );
            MouseDownCommand = ( RelayCommand ) mousehandler . GetMouseCommands ( "DOWN" );
            MouseEnterCommand = ( RelayCommand ) mousehandler . GetMouseCommands ( "ENTER" );
            MouseLeaveCommand = ( RelayCommand ) mousehandler . GetMouseCommands ( "LEAVE" );
            MouseLeftButtonUpCommand = ( RelayCommand ) mousehandler . GetMouseCommands ( "LEFTUP" );
            MouseLeftButtonDownCommand = ( RelayCommand ) mousehandler . GetMouseCommands ( "LEFTDOWN" );
            MouseRightButtonUpCommand = ( RelayCommand ) mousehandler . GetMouseCommands ( "RIGHTUP" );
            MouseRightButtonDownCommand = ( RelayCommand ) mousehandler . GetMouseCommands ( "RIGHTDOWN" );
            MouseMoveCommand = ( RelayCommand ) mousehandler . GetMouseCommands ( "MOVE" );
            MouseWheelCommand = ( RelayCommand ) mousehandler . GetMouseCommands ( "WHEEL" );

            CloseUControl4 = new RelayCommand ( ExecuteCloseUControl4 , CanExecuteCloseUControl4 );
        }
        private bool CanExecuteCloseUControl4 ( object arg )
        { return true; }

        private void ExecuteCloseUControl4 ( object obj )
        {
            uc4 . Visibility = Visibility . Hidden;
        }

        public static void LoadUcontrol4 ( MvvmBrowserUC win )
        {
            uc4 = win;
        }
     }

}