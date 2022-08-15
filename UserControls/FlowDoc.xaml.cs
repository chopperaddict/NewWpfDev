using System;
using System . ComponentModel;
using System . Diagnostics;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Documents;
using System . Windows . Input;
using System . Windows . Media;



using NewWpfDev . Views;
//using DocumentFormat . OpenXml . Presentation;
using System . Collections . Generic;

namespace NewWpfDev . UserControls;

/// <summary>
/// Interaction logic for FlowDoc.xaml
/// </summary>
public partial class FlowDoc : INotifyPropertyChanged
{

    public static event EventHandler<EventArgs> FlowDocClosed;

    #region Properties
    private bool mouseCaptured;
    public bool MouseCaptured
    {
        get { return mouseCaptured; }
        set { mouseCaptured = value; }
    }
    private static double docHeight;
    public static double DocHeight
    {
        get { return docHeight; }
        set { docHeight = value; }
    }
    private static double docWidth;
    public static double DocWidth
    {
        get { return docWidth; }
        set { docWidth = value; }
    }
    private bool borderClicked;
    public bool BorderClicked
    {
        get { return borderClicked; }
        set { borderClicked = value; }
    }
    private int borderSelected;
    public int BorderSelected
    {
        get { return borderSelected; }
        set { borderSelected = value; }
    }
    private bool keepSize;
    public bool KeepSize
    {
        get { return keepSize; }
        set { keepSize = value; }
    }
    private string keepSizeIcon1;
    public string KeepSizeIcon1
    {
        get { return keepSizeIcon1; }
        set { keepSizeIcon1 = value; }
    }
    private string keepSizeIcon2;
    public string KeepSizeIcon2
    {
        get { return keepSizeIcon2; }
        set { keepSizeIcon2 = value; }
    }

    #region OnPropertyChanged
    //        public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged ( string PropertyName )
    {
        if ( this . PropertyChanged != null )
        {
            var e = new PropertyChangedEventArgs ( PropertyName );
            this . PropertyChanged ( this , e );
        }
    }
    #endregion OnPropertyChanged

    private bool useScrollviewer;
    public bool UseScrollviewer
    {
        get { return useScrollviewer; }
        set { useScrollviewer = value; OnPropertyChanged ( "UseScrollviewer" ); }
    }
    private bool useRichTextBox;
    public bool UseRichTextBox
    {
        get { return useRichTextBox; }
        set { useRichTextBox = value; OnPropertyChanged ( "UseRichTextBox" ); }
    }

    #endregion Properties

    // Implement INameScope similar to this:

    public FlowDoc ( )
    {
        InitializeComponent ( );
        FontFamily FontFamily = new FontFamily ( "Rockwell Extra Bold" );
        fontFamily = FontFamily;
        doc . FontSize = ( double ) 16;
        Fontsize = doc . FontSize;
        //Font fontfamily = new Font ();
        //fontfamily . Typeface = "Arial";
        //fdviewer . FontFamily = new FontFamily ( );
        //var fonts = Fonts . GetFontFamilies ("@C:\\Boot" );
        //FlowDocumentScrollViewer . FontFamilyProperty . OverrideMetadata (
        //typeof ( FlowDoc ) ,
        //new FrameworkPropertyMetadata (
        //    new FontFamily ( "Comic Sans MS" ) ) );
    }

    private void flowdoc_Loaded ( object sender , RoutedEventArgs e )
    {
        this . DataContext = this;
        Fontsize = this . Fontsize;
        DocHeight = this . ActualHeight;
        if ( DocHeight == 0 )
            DocHeight = 250;
        DocWidth = this . ActualWidth;
        if ( DocWidth == 0 )
            DocWidth = 450;
        if ( Flags . UseScrollView )
        {
            UseScrollviewer = true;
            fdviewer . Visibility = Visibility . Visible;
            doc . Visibility = Visibility . Collapsed;
            BorderSelected = -1;
            UseScrollviewer = true;
            UseRichTextBox = false;
        }
        else
        {
            UseScrollviewer = false;
            fdviewer . Visibility = Visibility . Collapsed;
            doc . Visibility = Visibility . Visible;
            BorderSelected = -1;
            UseScrollviewer = false;
            UseRichTextBox = true;
        }
        KeepSizeIcon1 = "/Icons/down arroiw red.png";
        KeepSizeIcon2 = "/Icons/up arroiw red.png";

        Thickness th = new Thickness ( );
        th . Left = th . Right = th . Top = th . Bottom = 10;
        FdBorder . BorderThickness = th;
    }
    public void ShowInfo (
        FlowDoc Flowdoc ,
        Canvas canvas ,
        string line1 = "" ,
        string clr1 = "Black0" ,
        string line2 = "" ,
        string clr2 = "Blue0" ,
        string line3 = "" ,
        string clr3 = "Green2" ,
        string header = "" ,
        string clr4 = "Red3" ,
        bool beep = false )
    {
        TextRange textRange;
        FlowDocumentScrollViewer myFlowDocumentScrollViewer = new FlowDocumentScrollViewer ( );
        FlowDocument myFlowDocument = new FlowDocument ( );
        FlowDocument myFlowDocument2 = new FlowDocument ( );

        flowdoc . FontSize = this . Fontsize;
        if ( Flags . UseScrollView )
        {
            fdviewer . Visibility = Visibility . Visible;
            fdviewer . FontFamily = fontFamily;
            doc . Visibility = Visibility . Collapsed;
            doc. FontFamily = fontFamily;
            myFlowDocument2 = CreateFlowDocumentScroll ( line1 , clr1 , line2 , clr2 , line3 , clr3 , header , clr4 );
            fdviewer . Document = myFlowDocument2;
            textRange = new TextRange ( fdviewer . Document . ContentStart , fdviewer . Document . ContentEnd );
        }
        else
        {
            FontFamily FontFamily = new FontFamily ( "Script MT Bold" );
            fontFamily = FontFamily;

            fdviewer . Visibility = Visibility . Collapsed;
            fdviewer . FontFamily = fontFamily;
            doc . Visibility = Visibility . Visible;
            doc . FontFamily = fontFamily;
            myFlowDocument = CreateFlowDocument ( line1 , clr1 , line2 , clr2 , line3 , clr3 , header , clr4 );
            doc . Document = myFlowDocument;
            textRange = new TextRange ( doc . Document . ContentStart , doc . Document . ContentEnd );
        }
        // Get length of the controls content so we can resize as needed			
        int retcount = 0;
        for ( int x = 0 ; x < textRange . Text . Length - 1 ; x++ )
        {
            if ( textRange . Text [ x ] == '\n' )
                retcount++;
        }
        if ( flowdoc . Width == 0 )
            flowdoc . Width = 520;
        if ( flowdoc . Height == 0 )
            flowdoc . Height = 300;
        var v1 = Convert . ToDouble ( flowdoc . GetValue ( HeightProperty ) );
        var v2 = Convert . ToDouble ( flowdoc . GetValue ( WidthProperty ) );
        flowdoc . SetValue ( HeightProperty , DocHeight );
        flowdoc . SetValue ( WidthProperty , DocWidth );
        //			Debug. WriteLine ( $"{textRange . Text }\n" );
        //Debug. WriteLine ( $"Text Length in Flowdoc = {textRange . Text . Length }" );
        if ( textRange . Text . Length < 100 )
            flowdoc . SetValue ( HeightProperty , ( double ) 180 + retcount * Flags . FlowdocCrMultplier );
        else if ( textRange . Text . Length < 150 )
            flowdoc . SetValue ( HeightProperty , ( double ) 210 + retcount * Flags . FlowdocCrMultplier );
        else if ( textRange . Text . Length < 200 )
            flowdoc . SetValue ( HeightProperty , ( double ) 235 + retcount * Flags . FlowdocCrMultplier );
        else if ( textRange . Text . Length < 250 )
            flowdoc . SetValue ( HeightProperty , ( double ) 255 + retcount * Flags . FlowdocCrMultplier );
        else if ( textRange . Text . Length < 300 )
            flowdoc . SetValue ( HeightProperty , ( double ) 275 + retcount * Flags . FlowdocCrMultplier );
        else if ( textRange . Text . Length < 350 )
            flowdoc . SetValue ( HeightProperty , ( double ) 285 + retcount * Flags . FlowdocCrMultplier );
        else if ( textRange . Text . Length < 400 )
            flowdoc . SetValue ( HeightProperty , ( double ) 320 + retcount * Flags . FlowdocCrMultplier );
        else if ( textRange . Text . Length < 450 )
            flowdoc . SetValue ( HeightProperty , ( double ) 340 + retcount * Flags . FlowdocCrMultplier );
        else if ( textRange . Text . Length < 500 )
        {
            flowdoc . SetValue ( HeightProperty , ( double ) 360 + retcount * Flags . FlowdocCrMultplier );
            flowdoc . SetValue ( WidthProperty , ( double ) flowdoc . Width + 20 );
        }
        else if ( textRange . Text . Length < 600 )
        {
            flowdoc . SetValue ( HeightProperty , ( double ) 380 + retcount * Flags . FlowdocCrMultplier );
            flowdoc . SetValue ( WidthProperty , ( double ) flowdoc . Width + 30 );
        }
        else if ( textRange . Text . Length < 700 )
        {
            flowdoc . SetValue ( HeightProperty , ( double ) 400 + retcount * Flags . FlowdocCrMultplier );
            flowdoc . SetValue ( WidthProperty , ( double ) flowdoc . Width + 40 );
        }
        else if ( textRange . Text . Length < 800 )
        {
            flowdoc . SetValue ( HeightProperty , ( double ) 450 + retcount * Flags . FlowdocCrMultplier );
            flowdoc . SetValue ( WidthProperty , ( double ) flowdoc . Width + 50 );
        }
        else if ( textRange . Text . Length < 900 )
        {
            flowdoc . SetValue ( HeightProperty , ( double ) 500 + retcount * Flags . FlowdocCrMultplier );
            flowdoc . SetValue ( WidthProperty , ( double ) flowdoc . Width + 60 );
        }
        else
        {
            Flags . UseFlowScrollbar = true;
            flowdoc . SetValue ( HeightProperty , ( double ) 550 + retcount * Flags . FlowdocCrMultplier );
            flowdoc . SetValue ( WidthProperty , ( double ) flowdoc . Width + 100 );
        }
        flowdoc . Height = Convert . ToDouble ( flowdoc . GetValue ( HeightProperty ) );
        flowdoc . Width = Convert . ToDouble ( flowdoc . GetValue ( WidthProperty ) );
        DocHeight = flowdoc . Height;
        if ( flowdoc . Height == 0 )
            flowdoc . Height = v1;

        flowdoc . SetValue ( HeightProperty , ( double ) flowdoc . Height );
        //FlowDoc.DocHeight = 
        if ( flowdoc . Width == 0 )
            flowdoc . Width = v2;
        flowdoc . SetValue ( WidthProperty , ( double ) flowdoc . Width );

        if ( this . Visibility == Visibility . Collapsed )
        {
            if ( Flags . UseFlowScrollbar )
            {
                // text is too long for window, so handle scrollbar
                if ( Flags . UseScrollView )
                {
                    fdviewer . VerticalScrollBarVisibility = ScrollBarVisibility . Visible;
                }
                else
                {
                    doc . VerticalScrollBarVisibility = ScrollBarVisibility . Visible;
                }
            }
            this . Visibility = Visibility . Visible;
            this . BringIntoView ( );
            if ( beep )
                WpfLib1 . Utils . DoErrorBeep ( 300 , 50 , 1 );
        }
        else
        {
            this . Visibility = Visibility . Visible;
            this . BringIntoView ( );
        }
    }

    #region FlowDoc helpers
    private FlowDocument CreateFlowDocument ( string line1 , string clr1 , string line2 , string clr2 , string line3 , string clr3 , string header , string clr4 )
    {
        // Create new FlowDocument to be used by our RichTextBox
        FlowDocument myFlowDocument = new FlowDocument ( );

        if ( header != "" )
        {
            // BOLD + UNDERLINED
            Paragraph para2 = new Paragraph ( );
            // how to concatenate attributes on a paragraph
            para2 . FontSize = 16;
            para2 . FontFamily = new FontFamily ( "Arial" );
            if ( clr4 != "" )
                para2 . Foreground = FindResource ( clr4 . Trim ( ) ) as SolidColorBrush;
            else
                para2 . Foreground = FindResource ( "Black0" ) as SolidColorBrush;
            // Add some Bold text to the paragraph
            para2 . Inlines . Add ( new Underline ( new Bold ( new Run ( header . Trim ( ) ) ) ) );
            //Add paragraph to flowdocument
            myFlowDocument . Blocks . Add ( para2 );
        }

        if ( line1 != "" )
        {
            //NORMAL
            Paragraph para1 = new Paragraph ( );
            para1 . FontFamily = fontFamily;
//            para1 . FontFamily = new FontFamily ( "Arial" );
            if ( clr1 != "" )
                para1 . Foreground = FindResource ( clr1 . Trim ( ) ) as SolidColorBrush;
            else
                para1 . Foreground = FindResource ( "Black0" ) as SolidColorBrush;
            para1 . Inlines . Add ( new Run ( line1 ) );
            //Add paragraph to flowdocument
            myFlowDocument . Blocks . Add ( para1 );
        }
        if ( line2 != "" )
        {
            //\NORMAL
            Paragraph para2 = new Paragraph ( );
            para2 . FontFamily = new FontFamily ( "Arial" );
            para2 . FontSize = 14;
            if ( clr2 != "" )
                para2 . Foreground = FindResource ( clr2 . Trim ( ) ) as SolidColorBrush;
            else
                para2 . Foreground = FindResource ( "Black2" ) as SolidColorBrush;
            para2 . Inlines . Add ( new Run ( line2 ) );
            //Add paragraph to flowdocument
            myFlowDocument . Blocks . Add ( para2 );
        }
        if ( line3 != "" )
        {
            //ITALIC
            Paragraph para3 = new Paragraph ( );
            para3 . FontFamily = new FontFamily ( "Arial" );
            if ( clr3 != "" )
                para3 . Foreground = FindResource ( clr3 . Trim ( ) ) as SolidColorBrush;
            else
                para3 . Foreground = FindResource ( "Black3" ) as SolidColorBrush;
            para3 . Inlines . Add ( new Italic ( new Run ( line3 ) ) );
            //Add paragraph to flowdocument
            myFlowDocument . Blocks . Add ( para3 );
        }
        return myFlowDocument;
    }
    private FlowDocument CreateFlowDocumentScroll ( string line1 , string clr1 , string line2 , string clr2 , string line3 , string clr3 , string header , string clr4 )
    {
        // Create new FlowDocument to be used by our RichTextBox
        FlowDocument myFlowDocument = new FlowDocument ( );

        if ( header != "" )
        {
            // BOLD + UNDERLINED
            Paragraph para2 = new Paragraph ( );
            // how to concatenate attributes on a paragraph
            para2 . FontSize = 16;
            para2 . FontFamily = new FontFamily ( "Arial" );
            if ( clr4 != "" )
                para2 . Foreground = FindResource ( clr4 . Trim ( ) ) as SolidColorBrush;
            else
                para2 . Foreground = FindResource ( "Black0" ) as SolidColorBrush;
            // Add some Bold text to the paragraph
            para2 . Inlines . Add ( new Underline ( new Bold ( new Run ( header . Trim ( ) ) ) ) );
            //Add paragraph to flowdocument
            myFlowDocument . Blocks . Add ( para2 );
        }

        if ( line1 != "" )
        {
            //NORMAL
            Paragraph para1 = new Paragraph ( );
            // This is  the only paragraph that uses the user defined Font Size....
            para1 . FontSize = this . Fontsize;
            para1 . FontFamily = new FontFamily ( "Arial" );
            if ( clr1 != "" )
                para1 . Foreground = FindResource ( clr1 . Trim ( ) ) as SolidColorBrush;
            else
                para1 . Foreground = FindResource ( "Black0" ) as SolidColorBrush;
            para1 . Inlines . Add ( new Run ( line1 ) );
            //Add paragraph to flowdocument
            myFlowDocument . Blocks . Add ( para1 );
        }
        if ( line2 != "" )
        {
            // BOLD
            Paragraph para2 = new Paragraph ( );
            para2 . FontFamily = new FontFamily ( "Arial" );
            //				para2 . FontSize = 14;
            para2 . FontSize = this . Fontsize;
            if ( clr2 != "" )
                para2 . Foreground = FindResource ( clr2 . Trim ( ) ) as SolidColorBrush;
            else
                para2 . Foreground = FindResource ( "Black2" ) as SolidColorBrush;
            para2 . Inlines . Add ( new Run ( line2 ) );
            //Add paragraph to flowdocument
            myFlowDocument . Blocks . Add ( para2 );
        }
        if ( line3 != "" )
        {
            //ITALIC
            Paragraph para3 = new Paragraph ( );
            para3 . FontFamily = new FontFamily ( "Arial" );
            para3 . FontSize = 14;
            if ( clr3 != "" )
                para3 . Foreground = FindResource ( clr3 . Trim ( ) ) as SolidColorBrush;
            else
                para3 . Foreground = FindResource ( "Black3" ) as SolidColorBrush;
            para3 . Inlines . Add ( new Italic ( new Run ( line3 ) ) );
            //Add paragraph to flowdocument
            myFlowDocument . Blocks . Add ( para3 );
        }
        //myFlowDocumentScrollViewer = myFlowDocument;
        return myFlowDocument;
    }
    private void doc_GotFocus ( object sender , RoutedEventArgs e )
    {
        //e . Handled = true;
    }
    private void Border_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
    {
        //	BorderClicked = false;
        Border bd = sender as Border;
        if ( WpfLib1 . Utils . HitTestBorder ( bd , e ) )
        {
            string selections = "";
            // Over the Border, so let user resize contents
            this . BorderClicked = true;

            // Mouse Horizontal (X) position
            double left = e . GetPosition ( ( FdBorder as FrameworkElement ) . Parent as FrameworkElement ) . X;
            double height = this . ActualHeight;
            // Mouse Vertical (Y) position
            double MTop = e . GetPosition ( ( FdBorder as FrameworkElement ) . Parent as FrameworkElement ) . Y;
            double MBottom = MTop + this . ActualHeight;

            //Debug. WriteLine ( $"Border Hit : Left {left}, Top {MTop}\nWidth {this . ActualWidth}, Height {this . ActualHeight}" );
            double ValidTopT = FdBorder . BorderThickness . Left;
            double ValidBottomT = this . ActualHeight + FdBorder . BorderThickness . Left;
            double ValidTopB = MBottom - ( FdBorder . BorderThickness . Left * 2 );
            double ValidBottomB = MBottom + ( FdBorder . BorderThickness . Left * 2 );

            if ( MTop <= ValidTopT && MTop >= 0 )
            {
                // Top
                this . BorderSelected = 1;
                //					if  ( this . ActualWidth - left < 10 )
                //						this . BorderSelected = 4;
                Mouse . SetCursor ( Cursors . SizeNS );
                selections = $"{this . borderSelected}";

            }
            if ( MBottom >= ValidTopB && MBottom <= ValidBottomB && MTop > height - 20 )
            {
                // Bottom
                Mouse . SetCursor ( Cursors . SizeNS );
                BorderSelected = 2;
                //					if ( this . ActualWidth - left < 10 )
                //						this . BorderSelected = 4;
                selections += $"{this . borderSelected}";
            }
            if ( left < 10 )
            {
                // Left
                Mouse . SetCursor ( Cursors . SizeWE );
                this . BorderSelected = 3;
                selections += $"{this . borderSelected}";
            }
            if ( this . ActualWidth - left < 10 )
            {
                //Right
                Mouse . SetCursor ( Cursors . SizeWE );
                this . BorderSelected = 4;
                selections += $"{this . borderSelected}";
            }
            if ( selections . Contains ( "1" ) && selections . Contains ( "3" ) )
                borderSelected = 5;      // TOP + LEFT
            else if ( selections . Contains ( "2" ) && selections . Contains ( "3" ) )
                borderSelected = 6;       // BOTTOM + LEFT
            else if ( selections . Contains ( "1" ) && selections . Contains ( "4" ) )
                borderSelected = 7;       // TOP + RIGHT
            else if ( selections . Contains ( "2" ) && selections . Contains ( "4" ) )
                borderSelected = 8;       // BOTTOM + RIGHT
        }
        else
            Mouse . SetCursor ( Cursors . SizeAll );

    }

    //private void KeepSize_PreviewMouseLeftButtonUp ( object sender , MouseButtonEventArgs e )
    //{
    //	KeepSize = !KeepSize;
    //	if ( KeepSize == true )
    //	{
    //		KeepIcon . Source = new BitmapImage ( new Uri ( KeepSizeIcon1 , UriKind . Relative ) );
    //		SaveLabel . Content = "Using Saved Height =";
    //	}
    //	else
    //	{
    //		KeepIcon . Source = new BitmapImage ( new Uri ( KeepSizeIcon2 , UriKind . Relative ) );
    //		SaveLabel . Content = "Using Auto Height =";
    //	}
    //}

    private void Border_PreviewMouseLeftButtonUp ( object sender , MouseButtonEventArgs e )
    {
        BorderClicked = false;
    }

    private void FdBorder_MouseMove ( object sender , MouseEventArgs e )
    {
        // Flowdoc is being resized
        if ( BorderClicked )
        {
        }
        this . Focus ( );
    }

    private void FlowdocBorder_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
    {
        Border border = sender as Border;
        if ( border . Name == "FdBorder" )
            BorderClicked = true;
    }

    #endregion FlowDoc helpers
    private void Button_Click ( object sender , RoutedEventArgs e )
    {
        this . Visibility = Visibility . Collapsed;
        Mouse . OverrideCursor = Cursors . Arrow;
        Mouse . OverrideCursor = Cursors . Arrow;
        BorderSelected = -1;
    }

    #region Dependency properties
    public Brush borderColor
    {
        get { return ( Brush ) GetValue ( borderColorProperty ); }
        set { SetValue ( borderColorProperty , value ); }
    }
    public static readonly DependencyProperty borderColorProperty =
        DependencyProperty . Register ( "borderColor" , typeof ( Brush ) , typeof ( FlowDoc ) , new PropertyMetadata ( Brushes . Red ) );

    public Brush backGround
    {
        get { return ( Brush ) GetValue ( backGroundProperty ); }
        set { SetValue ( backGroundProperty , value ); }
    }
    public static readonly DependencyProperty backGroundProperty =
        DependencyProperty . Register ( "backGround" , typeof ( Brush ) , typeof ( FlowDoc ) , new PropertyMetadata ( Brushes . LightGray ) );

    public Brush btnBkGround
    {
        get { return ( Brush ) GetValue ( btnBkGroundProperty ); }
        set { SetValue ( btnBkGroundProperty , value ); }
    }
    public static readonly DependencyProperty btnBkGroundProperty =
        DependencyProperty . Register ( "btnBkGround" , typeof ( Brush ) , typeof ( FlowDoc ) , new PropertyMetadata ( Brushes . Red ) );

    public Brush btnForeGround
    {
        get { return ( Brush ) GetValue ( btnForeGroundProperty ); }
        set { SetValue ( btnForeGroundProperty , value ); }
    }
    public static readonly DependencyProperty btnForeGroundProperty =
        DependencyProperty . Register ( "btnForeGround" , typeof ( Brush ) , typeof ( FlowDoc ) , new PropertyMetadata ( Brushes . White ) );
    public double Fontsize
    {
        get { return ( double ) GetValue ( FontsizeProperty ); }
        set { SetValue ( FontsizeProperty , value ); }
    }
    public static readonly DependencyProperty FontsizeProperty =
        DependencyProperty . Register ( "Fontsize" , typeof ( double ) , typeof ( FlowDoc ) , new PropertyMetadata ( ( double ) 12 ) );
    public FontFamily fontFamily
    {
        get { return ( FontFamily ) GetValue ( fontFamilyProperty ); }
        set { SetValue ( fontFamilyProperty , value ); }
    }
    public static readonly DependencyProperty fontFamilyProperty =
        DependencyProperty . Register ( "fontFamily" , typeof ( FontFamily ) , typeof ( FlowDoc ) , new PropertyMetadata ( default ) );


    #endregion Dependency properties

    public static void SetFocus ( )
    {
        //			this . Focus ( );
    }
    #region keyboard handlers
    private void flowdoc_PreviewKeyDown ( object sender , KeyEventArgs e )
    {
        OnHandleKeyEvents ( e );


        if ( e . Key == Key . Escape )
        {
            e . Handled = true;
            {
                this . Visibility = Visibility . Collapsed;
                Mouse . OverrideCursor = Cursors . Arrow;
                Mouse . OverrideCursor = Cursors . Arrow;
            }
            BorderSelected = -1;
        }
        else if ( e . Key == Key . F8 )
        {
            fdviewer . ReleaseMouseCapture ( );
            flowdoc . ReleaseMouseCapture ( );
        }
    }
    #endregion keyboard handlers

    #region Mouse handlers
    // ONLY Called when flowdoc Document  or Scrollviewer area  is clicked on
    // NOT called by Listviews
    // This  IS CALLED by DataGrid
    private void flowdoc_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
    {
        if ( WpfLib1 . Utils . HitTestScrollBar ( sender , e ) )
        {
            // Over the Scrollbar so let user scroll contents
            if ( e . OriginalSource . ToString ( ) . Contains ( ".Run" ) )
            {
                // Not needed !! 8/4/22 - effect V.Scrollbar badly
                //if ( Flags . UseScrollView )
                //	fdviewer . IsEnabled = true;
                //else
                //	doc . IsEnabled = true;
            }
            else
            {
                if ( Flags . UseScrollView )
                {
                    // We get here when clicking on scrollbar
                    fdviewer . IsEnabled = false;
                    if ( fdviewer . VerticalScrollBarVisibility == ScrollBarVisibility . Visible )
                    {
                        fdviewer . IsEnabled = true;
                        fdviewer . ReleaseMouseCapture ( );
                        flowdoc . ReleaseMouseCapture ( );
                        return;
                    }
                }
                else
                {
                    // Not needed !! 8/4/22 - effect V.Scrollbar badly
                    //doc . IsEnabled = false;
                    if ( doc . VerticalScrollBarVisibility == ScrollBarVisibility . Visible )
                    {
                        // Not needed !! 8/4/22 - effect V.Scrollbar badly
                        //doc . IsEnabled = true;
                        fdviewer . ReleaseMouseCapture ( );
                        flowdoc . ReleaseMouseCapture ( );
                        return;
                    }
                }
            }
        }
        else
        {
            // Not needed !! 8/4/22 - effect V.Scrollbar badly
            // NOT over scrollbar, so only allow drag
            //if ( Flags . UseScrollView )
            //{
            //	fdviewer . IsEnabled = true;
            //}
            //else
            //{
            //	doc . IsEnabled = true;
            //}
        }
        Button btn = sender as Button;
        string str = e . OriginalSource . ToString ( ) . ToUpper ( );
        if ( str . Contains ( "BORDER" ) == true )
        {
            Button_Click ( null , null );
            fdviewer . ReleaseMouseCapture ( );
            flowdoc . ReleaseMouseCapture ( );
            return;
        }
        if ( Flags . UseScrollView )
            MouseCaptured = fdviewer . CaptureMouse ( );
        else
            MouseCaptured = flowdoc . CaptureMouse ( );
        // Not needed !! 8/4/22 - effect V.Scrollbar badly
        //if ( Flags . UseScrollView )
        //	fdviewer . IsEnabled = true;
        //else
        //	doc . IsEnabled = true;
        //e . Handled = true;
    }

    // This  IS CALLED by DataGrid
    // This  IS CALLED by ListViews
    private void flowdoc_PreviewMouseLeftButtonUp ( object sender , MouseButtonEventArgs e )
    {
        fdviewer . ReleaseMouseCapture ( );
        flowdoc . ReleaseMouseCapture ( );
        BorderClicked = false;
    }
    private void scrollviewer_PreviewMouseLeftButtonUp ( object sender , MouseButtonEventArgs e )
    {
        fdviewer . ReleaseMouseCapture ( );
        flowdoc . ReleaseMouseCapture ( );
        BorderClicked = false;
    }

    private void scrollviewer_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
    {
        if ( WpfLib1 . Utils . HitTestScrollBar ( sender , e ) )
        {
            fdviewer . IsEnabled = true;
            return;
        }
        else
            fdviewer . IsEnabled = false;

        MouseCaptured = flowdoc . CaptureMouse ( );
        Debug . WriteLine ( "Mouse CAPTURED...scrollviewer_PreviewMouseLeftButtonDown()" );
        e . Handled = true;
    }
    private void scrollviewer_MouseDown ( object sender , MouseButtonEventArgs e )
    {
        if ( WpfLib1 . Utils . HitTestScrollBar ( sender , e ) )
        {
            fdviewer . IsEnabled = true;
            return;
        }
        else
            fdviewer . IsEnabled = false;
        MouseCaptured = flowdoc . CaptureMouse ( );
        Debug . WriteLine ( "Mouse CAPTURED...scrollviewer_PreviewMouseLeftButtonDown()" );
        e . Handled = true;
    }

    #endregion Mouse handlers

    private void Closebtn_PreviewMouseLeftButtonUp ( object sender , MouseButtonEventArgs e )
    {
        flowdoc . ReleaseMouseCapture ( );
        BorderSelected = -1;
        this . Visibility = Visibility . Collapsed;
        FlowDocClosed?.Invoke ( this , null );
        Mouse . OverrideCursor = Cursors . Arrow;
    }

    private void dummy_Click ( object sender , RoutedEventArgs e )
    {
        ;     // context menu click
    }

    private void Exit_PreviewMouseLeftButtonUp ( object sender , MouseButtonEventArgs e )
    {
        this . Visibility = Visibility . Collapsed;
        flowdoc . ReleaseMouseCapture ( );
        BorderSelected = -1;
        Mouse . OverrideCursor = Cursors . Arrow;
    }

    #region External Hook
    //code to allow this action in flowdoc to allow smart resizing
    // Clever stuff really
    public event EventHandler ExecuteFlowDocBorderMethod;
    protected virtual void OnExecuteFlowDocBorderMethod ( EventArgs e )
    {
        if ( ExecuteFlowDocBorderMethod != null )
            ExecuteFlowDocBorderMethod ( this , e );
    }

    //code to allow this action in flowdocto be andled by an external window
    // Clever stuff really
    public event EventHandler<FlowArgs> ExecuteFlowDocResizeMethod;
    protected virtual void OnExecuteResizeMethod ( FlowArgs e )
    {
        if ( ExecuteFlowDocResizeMethod != null )
            ExecuteFlowDocResizeMethod ( this , e );
    }

    // Allows any other (External) window to control this via  the control 
    public event EventHandler ExecuteFlowDocMaxmizeMethod;
    protected virtual void OnExecuteMethod ( )
    {
        if ( ExecuteFlowDocMaxmizeMethod != null )
            ExecuteFlowDocMaxmizeMethod ( this , EventArgs . Empty );
    }

    public event KeyEventHandler HandleKeyEvents;
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnHandleKeyEvents ( KeyEventArgs e )
    {
        if ( HandleKeyEvents != null )
            HandleKeyEvents ( this , e );
    }
    private void Image_PreviewMouseLeftButtonUp ( object sender , MouseButtonEventArgs e )
    {
        //allows remote window to maximize /resize  this control ?
        OnExecuteMethod ( );
    }

    #endregion External Hook

    // Called by ListViews	&& Datagrids
    private void FdBorder_MouseEnter ( object sender , MouseEventArgs e )
    {
        Mouse . SetCursor ( Cursors . Hand );
        //MouseCaptured = true;
        //CaptureMouse ( );
        this . Focus ( );
    }

    // Called by ListViews	&& Datagrids
    private void FdBorder_MouseLeave ( object sender , MouseEventArgs e )
    {
        Mouse . OverrideCursor = Cursors . Arrow;
        //MouseCaptured = false;				
    }

    private void UseRTB_Click ( object sender , RoutedEventArgs e )
    {
        if ( UseRichTextBox == false )
        {
            UseScrollviewer = false;
            UseRichTextBox = true;
        }
        else
        {
            UseScrollviewer = true;
            UseRichTextBox = false;
        }
        Flags . UseScrollView = UseScrollviewer;
    }

    private void UseSViewer_Click ( object sender , RoutedEventArgs e )
    {
        if ( UseScrollviewer == false )
        {
            UseScrollviewer = true;
            UseRichTextBox = false;
        }
        else
        {
            UseScrollviewer = false;
            UseRichTextBox = true;
        }
        Flags . UseScrollView = UseScrollviewer;
    }

    private void SelectFont_Click ( object sender , RoutedEventArgs e )
    {
        List<string> fonts = DapperGenericsLib . Utils . GetFontsList ( );
    }
}
public class FlowArgs : EventArgs
{
    public double Height
    {
        get; set;
    }
    public double Width
    {
        get; set;
    }
    public double CTop
    {
        get; set;
    }
    public double CLeft
    {
        get; set;
    }
    public double Xpos
    {
        get; set;
    }
    public double Ypos
    {
        get; set;
    }
    public bool BorderClicked
    {
        get; set;
    }
}
