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
using System . Windows . Navigation;
using System . Windows . Shapes;

using NewWpfDev . ViewModels;

namespace NewWpfDev . UserControls
{
    /// <summary>
    /// Interaction logic for MvvmListboxUC.xaml
    /// </summary>
    /// 
    public partial class MvvmListboxUC 
    {
#pragma warning disable CS0169 // The field 'MvvmListboxUC.ucvm' is never used
        MvvmListboxUCViewModel ucvm;
#pragma warning restore CS0169 // The field 'MvvmListboxUC.ucvm' is never used
        MvvmContainerViewModel mcvm { set; get; }
        public static MvvmListboxUC mvvmlistboxuc { get; set; }

        public MvvmListboxUC ( int mode = 0 )
        {
            InitializeComponent ( );
            mvvmlistboxuc = this;
            mcvm = MvvmContainerViewModel . GetMvvmContainerViewModel ( );
            if ( mode == 1 )
            {
                MvvmContainerViewModel . CreateUCList ( );
                MvvmListboxUCViewModel . LoadData ( this , 1 );
                _listbox . ItemsSource = MvvmContainerViewModel . UCList;
            }
            else
            {
                MvvmListboxUCViewModel . LoadData ( this , 0 );
                _listbox . ItemsSource = MvvmListboxUCViewModel . TestData;
            }
        }
        public static MvvmListboxUC GetListBoxUc ( )
        { return mvvmlistboxuc; }
        private void _listbox_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {
            //ListBox lb = sender as ListBox;
            MvvmListboxUCViewModel . _listbox_SelectionChanged ( sender , e );

        }
    }
}
