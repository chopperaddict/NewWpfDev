using System;
using System . Windows;
using System . Windows . Controls;

namespace UserControls
{
    /// <summary>
    /// Interaction logic for GenericSelectBoxControl.xaml
    /// </summary>
    public partial class GenericSelectBoxControl : UserControl
    {
        public event EventHandler<SelectionArgs> ListSelection;
        public GenericSelectBoxControl ( )
        {
            InitializeComponent ( );
        }

        private void cancelbtn ( object sender , RoutedEventArgs e )
        {
            this . Visibility = Visibility . Collapsed;
        }

        private void selectbtn ( object sender , RoutedEventArgs e )
        {
            ListBox lb = sender as ListBox;
            SelectionArgs args = new SelectionArgs ( );
            args . selection = lb . SelectedItem . ToString ( );
            ListSelection . Invoke ( sender , args );
            this . Visibility = Visibility . Collapsed;
        }
    }
}
