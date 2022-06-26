using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . ComponentModel . Design;
using System . Diagnostics;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Input;
using System . Windows . Markup;

namespace NewWpfDev  . ViewModels
{
    //public delegate void RunMenuCommand ( string command , object data );
    public class MenuTestViewModel
    {
        private readonly ICommand _command;
//        public  delegate void UpdateBankAccountSelection ( object sender , DbArgs args );
        Delegates . RunMenuCommand rmc = null;
        public MenuTestViewModel ( )
        {
            _command = new CommandViewModel ( Execute );
        }

        public string Header { get; set; }

        public ObservableCollection<MenuTestViewModel> MenuItems { get; set; }

        public ICommand Command
        {
            get
            {
                return _command;
            }
        }

        private void Execute ( )
        {

            // (NOTE: This gets ALL input form the Obs Collection menu, so weneed to parse it here
            // and then call the relevant methods (Using a delegate (rmc=delegate RunMenuCommand(string, object))!)
            switch ( Header . ToString ( ) )
            {
                case "Alpha":
                     rmc = MenuCommand1;
                    rmc . Invoke (Header, null);
                    break;
                case "Gamma":
                     rmc = MenuCommand3;
                    rmc . Invoke ( Header , null );
                    break;
                default:
                    if ( Header . Contains ( "Beta" ) )
                        rmc = MenuCommand2;
                    if ( Header . Contains ( "Alpha" ) )
                        rmc = MenuCommand1;

                    rmc . Invoke ( Header , null );
                    break;
            }
        }
        public void MenuCommand1 ( string cmd , object data )
        {
            Debug. WriteLine ($"Menu item {cmd}  hit ...");
            return;
        }
        public void MenuCommand2 ( string cmd , object data )
        {
            if ( Header . Contains ( "Beta1" ) )
                ProcessBeta1 ( cmd, data);
            else if ( Header . Contains ( "Beta1a" ) )
                ProcessBeta1a ( cmd , data );
            else if ( Header . Contains ( "Beta3" ) )
                ProcessBeta1 ( cmd , data );
            Debug. WriteLine ( $"Menu item {cmd}  hit ..." );
            return;
        }
        public void MenuCommand3 ( string cmd , object data )
        {
            Debug. WriteLine ( $"Menu item {cmd}  hit ..." );
            return;
        }
        public void ProcessBeta1 ( string cmd , object data )
        {
            Debug. WriteLine ( $"Menu item {cmd}  hit ..." );
        }
        private void ProcessBeta1a ( string cmd , object data )
        {
            Debug. WriteLine ( $"Menu item {cmd}  hit ..." );
        }
    }
    public class CommandViewModel : ICommand
    {
        private readonly Action _action;

        public CommandViewModel ( Action action )
        {
            _action = action;
        }

        public void Execute ( object o )
        {
            _action ( );
        }

        public bool CanExecute ( object o )
        {
            return true;
        }

        public event EventHandler CanExecuteChanged
        {
            add { }
            remove { }
        }
    }
}
