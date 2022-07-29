using System;
using System . Collections . Generic;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows . Input;

namespace NewWpfDev . Commands
{
    /// <summary>
    /// Helper Extension class for ICommands
    /// Allows any ICommand to be executed really easily by simply calling this method from c#:
    ///             viewmodel.SelectGrid . CheckBeginExecute ([parameter string Args );
    /// where viewmodel = DataContext Class for the ICommand = SelectGrid 
    /// </summary>
    public static class ICommandHelper
        {
       
            public static bool ExecuteCommand ( this ICommand command, object[] args)
            {
                return CheckBeginExecuteCommand ( command ,args);
            }

            public static bool CheckBeginExecuteCommand ( ICommand command , object [ ] args )
            {
                var canExecute = false;
                lock ( command )
                {
                    canExecute = command . CanExecute ( args );
                    if ( canExecute )
                    {
                        command . Execute ( args);
                    }
                }
                return canExecute;
            }
        }
    }