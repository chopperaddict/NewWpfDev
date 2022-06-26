using System;
using System . Collections . Generic;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows . Input;
using System . Windows . Navigation;

namespace NewWpfDev. ViewModels
{
      /// <summary>
      /// Basic class to run any action
      /// </summary>
      public class RelayCommand : ICommand
      {
            Action < object > _executeMethod;
            Func <object, bool> _canexecuteMethod;

            public RelayCommand ( Action<object> executeMethod , Func<object , bool> canexecuteMethod )
            {
                  _executeMethod = executeMethod;
                  _canexecuteMethod = canexecuteMethod;
            }

        public RelayCommand ( )
        {
        }

        public bool CanExecute ( object parameter )
            {
                  if ( _canexecuteMethod != null )
                  {
                        return _canexecuteMethod ( parameter );
                  }
                  else
                  {
                        return false;
                  }
            }

            public event EventHandler CanExecuteChanged {
                  add
                  {
                        CommandManager.RequerySuggested += value;
                  }
                  remove
                  {
                        CommandManager.RequerySuggested -= value;
                  }
            }

            public void Execute ( object parameter )
            {
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            try
            {
                _executeMethod ( parameter );
            }
            catch ( Exception ex ) { }
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            }
      }
}