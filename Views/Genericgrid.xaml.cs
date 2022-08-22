using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . Text;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Data;
using System . Windows . Documents;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Media . Imaging;
using System . Windows . Shapes;

using NewWpfDev;

using UserControls;

namespace Views
{
    /// <summary>
    ///Genericgrid.xaml host for my DataGrid UserControl
    /// </summary>
    public partial class Genericgrid : Window
    {
        ObservableCollection<GenericClass> GridData = new ObservableCollection<GenericClass>();
        public Genericgrid ( )
        {
            InitializeComponent ( );
           GridData =  DatagridControl . LoadGenericData( GridData , "BankAccount");
        }
    }
}
