using System;
using System . Collections . Generic;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Data;
using System . Windows . Documents;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Media . Imaging;
using System . Windows . Shapes;

using NewWpfDev . Models;
using NewWpfDev . UserControls;
using NewWpfDev . ViewModels;

namespace NewWpfDev . Views {
    /// <summary>
    /// Interaction logic for BankAccountHost.xaml
    /// </summary>
    public partial class BankAccountHost : Window {
        private readonly BankAccountVM _viewModel;
        public BankAccountHost ( ) {
            _viewModel = new BankAccountVM ( );
            DataContext =_viewModel;
            InitializeComponent ( );
            //WpfMVVMSample . MainWindow window = new MainWindow ( );
            //UserViewModel VM = new UserViewModel ( );
            //window . DataContext = VM;
            //window . Show ( );
        }
    }
}
