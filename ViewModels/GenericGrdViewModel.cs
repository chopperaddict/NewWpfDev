using System;
using System . Collections . Generic;
using System . Text;
using System . Windows;
using System . Windows . Input;

using NewWpfDev . ViewModels;

namespace ViewModels
{
    public  class GenericGrdViewModel
    {
        private ICommand _command;
        //public ICommand Command
        //{
        //    get
        //    {
        //        return _command ?? ( _command = new RelayCommand (
        //           x =>
        //           {
        //               DoStuff ( );
        //           } ) );
        //    }

        //}

        private static void DoStuff (object obj, Func<object, bool> func )
        {
            MessageBox . Show ( "Responding to button click event..." );
        }
    }
}
