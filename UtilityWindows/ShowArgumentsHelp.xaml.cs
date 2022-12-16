using System;
using System . Collections . Generic;
using System . IO;
using System . Text;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Data;
using System . Windows . Documents;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Media . Imaging;
using System . Windows . Shapes;



namespace NewWpfDev
{
    /// <summary>
    /// Interaction logic for ShowArgumentsHelp.xaml
    /// </summary>
    public partial class ShowArgumentsHelp : Window
    {
        public static string argsinfo = "";
        public int Fontsize = 14;
        static public Control parent = null;

        public ShowArgumentsHelp ( string textfile ,Control Parent=null)
        {
            InitializeComponent ( );
            this . DataContext = this;
            parent = Parent;
            argsinfo = File . ReadAllText ( textfile );
            WpfLib1 . Utils . SetupWindowDrag ( this );
        }

        private void Closebtn_Click ( object sender , RoutedEventArgs e )
        {
            if ( parent != null )
                parent . Focus ( );
                this . Close ( );
        }

        private void Window_Loaded ( object sender , RoutedEventArgs e )
        {
            Window win = sender as Window;
            if ( win . Title == "Stored Procedures Results Viewer" )
                win . Topmost = false;
            if( ArgInfo . Text =="")
                ArgInfo . Text = File . ReadAllText ( @$"C:\users\ianch\documents\argsinfo.txt" );
            if ( ScrollArginfo . Document == null )
            {
                FlowDocument Document = new FlowDocument ( );
                ScrollArginfo . Document = Document;
                ScrollArginfo . Document . Blocks . Clear ( );
                ScrollArginfo . Document = null;
                ScrollArginfo . Document = LoadFlowDoc ( ScrollArginfo , FindResource ( "Black0" ) as SolidColorBrush , ArgInfo . Text );
            }
            List<string> sizes = new List<string> ( );
            for ( int x = 11 ; x < 21 ; x++ )
            {
                sizes . Add ( $"{x}" );
            }
            CurrentFontSize . ItemsSource = sizes;
            CurrentFontSize . SelectedIndex = 3;
        }
        public FlowDocument LoadFlowDoc ( FlowDocumentScrollViewer ctrl , SolidColorBrush BkgrndColor = null , string item1 = "" , string clr1 = "" , string item2 = "" , string clr2 = "" , string item3 = "" , string clr3 = "" , string header = "" , string clr5 = "" )
        {
            FlowDocument myFlowDocument = new FlowDocument ( );
            myFlowDocument = CreateFlowDocumentScroll ( item1 , clr1 , item2 , clr2 , item3 , clr3 , header , clr5 );
            ctrl . Document = myFlowDocument;

            myFlowDocument . Background = BkgrndColor;
            ctrl . UpdateLayout ( );
            return myFlowDocument;
        }
        public FlowDocument CreateFlowDocumentScroll ( string line1 , string clr1 = "" , string line2 = "" , string clr2 = "" , string line3 = "" , string clr3 = "" , string header = "" , string clr4 = "" ,
            int fontsize = 0 , string fground = "" , string bground = "" )
        {
            FlowDocument myFlowDocument = new FlowDocument ( );
            Paragraph para1 = new Paragraph ( );
            //NORMAL
            // This is  the only paragraph that uses the user defined Font Size....
            para1 . FontSize = Fontsize;
            para1 . FontFamily = new FontFamily ( "Arial" );
            para1 . Foreground = FindResource ( "White0" ) as SolidColorBrush;
            // handle user defined optional parameters
            if ( fground != "" )
                para1 . Foreground = FindResource ( fground ) as SolidColorBrush;
            if ( bground != "" )
                para1 . Background = FindResource ( "Black3" ) as SolidColorBrush;
            Thickness th = new Thickness ( );
            //para1 . Background = bground as Brush;
            th . Top = 5;
            th . Left = 10;
            th . Right = 10;
            th . Bottom = 10;
            para1 . Padding = th;
            para1 . Inlines . Add ( new Run ( line1 ) );
            //Add paragraph to flowdocument
            myFlowDocument . Blocks . Clear ( );
            myFlowDocument . Blocks . Add ( para1 );
            return myFlowDocument;
        }

        private void CurrentFontSize_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {
            string newsize = "";
            string infotext = "";
            ComboBox cb = sender as ComboBox;
            if(cb != null)
            {
                newsize = cb . SelectedItem.ToString();
                Fontsize = Convert . ToInt32 ( newsize );
                FlowDocument doc = ScrollArginfo . Document;
                doc.Blocks . Clear ( );
                infotext = File . ReadAllText ( @$"C:\users\ianch\documents\argsinfo.txt" );
                SolidColorBrush newcolor = FindResource ( "Black4" ) as SolidColorBrush;
                doc = CreateFlowDocumentScroll ( infotext, bground:newcolor.ToString() );
                doc . Background = newcolor ;
                ScrollArginfo.Document= doc;
                ScrollArginfo . UpdateLayout ( );
            }
        }
    }
}
