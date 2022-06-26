using System;
using System . Collections . Generic;
using System . Linq;
using System . Text;
using System . Threading . Tasks;

using NewWpfDev. UserControls;
using NewWpfDev. ViewModels;

namespace NewWpfDev
{
    /// <summary>
    /// A class to hold all generic Delegate methods
    /// </summary>
    public class Delegates
    {
        // int = (int, int)
        public delegate int  MyFunc(int arg, int arg2);

        public delegate bool  CompareBankRecords( BankAccountViewModel arg1 , BankAccountViewModel arg2 );

        // MainWindow.xaml.cs
        public delegate void LoadTableDelegate ( string Sqlcommand , string TableType , object bvm );
        public delegate void LoadTableWithDapperDelegate ( string Sqlcommand , string TableType , object bvm , object Args );

        // DargDropClient.xaml.cs
        public delegate string QualifyingFileLocations ( string filename );

        // MenutestViewModel.xaml.cs
        public delegate void RunMenuCommand ( string command , object data );

        
        // TextBoxWiithDataError.xaml.cs
        public delegate void SendUserHandler ( object sender , MessageEventArgs args );
        public event SendUserHandler SendUser;
        //YieldWindow.xaml.cs
        public delegate void UpdateBankAccountSelection ( object sender , DbArgs args );


    }
}
