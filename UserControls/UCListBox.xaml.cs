using System . Collections . ObjectModel;
using System . IO;
using System . Windows . Controls;
using System . Windows . Input;

namespace NewWpfDev . UserControls
{
    /// <summary>
    /// Interaction logic for UCListBox.xaml
    /// </summary>
    public partial class UCListBox 
    {
        public static UCListBox UclistBox { get; set; }
        ObservableCollection <string> ImageCollection = new ObservableCollection <string> ();
        public  int CurrentIndex { get; set; } = -1;
        public UCListBox ( )
        {
            InitializeComponent ( );
            LoadListbox ( );
            UclistBox = this;
            this . DataContext = this;
        }
        public static UCListBox GetUcLisbox ( )
        {
            return UclistBox;
        }
        public void LoadListbox ( )
        {
            // Load listbox with all file names
            LoadImagesCollection ( );
//            string path = @"C:\\Users\ianch\pictures\";
//            var imagefiles = Directory . GetFiles ( path );
//            List<string> images = new List<string> ( );
//            foreach ( var imagefile in imagefiles )
//            {
//                if ( imagefile . ToUpper ( ) . Contains ( ".PSD" ) == false )
//                    ImageCollection.Add ( imagefile );
////                images . Add ( imagefile );
//            }
            //imagefiles = images . ToArray<string> ( );
            // Get pointer to listbox Viewmodel
            //We have a list of all image files in UClistbox
//            UClistbox . ItemsSource = imagefiles;
            UClistbox . ItemsSource = ImageCollection;
            if ( CurrentIndex == -1 )
            {
                CurrentIndex = 0;
                UClistbox . SelectedIndex = 0;
                UClistbox . SelectedItem = 0;
            }
            else
            {
                UClistbox . SelectedIndex = CurrentIndex;
                UClistbox . SelectedItem = CurrentIndex;
                Utils . ScrollLBRecordIntoView ( UClistbox , CurrentIndex );
            }
        }
        public void LoadImagesCollection ( )
        {
            // Load listbox with all file names
            string path = @"C:\\Users\ianch\pictures\";
            var imagefiles = Directory . GetFiles ( path );
            //List<string> images = new List<string> ( );
            foreach ( var imagefile in imagefiles )
            {
                if ( imagefile . ToUpper ( ) . Contains ( ".PSD" ) == false )
                    ImageCollection . Add ( imagefile );
            }
        }
            private void UClistbox_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {
            ListBox lb = sender as ListBox;
            CurrentIndex = lb . SelectedIndex;
        }

        private void Command3_MouseDoubleClick ( object sender , MouseButtonEventArgs e )
        {
            //ListBox lb = sender as ListBox;
            //string sel = lb. SelectedItem . ToString ( );
            ////this . Visibility = Visibility . Collapsed;
            //Image img = new Image ( );
            //img . Source = new BitmapImage ( new Uri ( sel, UriKind . Relative ) );
            UserControlsViewer  ucv = UserControlsViewer . GetUCviewer ( );
            ucv . ShowImage ( sender,e);
            //ucv.Contentctrl . Content = img;
            //ucv . Visibility = Visibility . Visible;
            //ucv . Command3 ( sender , e );
            //ucv . Contentctrl . Refresh ( );
        }
    }
}
