using System;
using System . Collections . Generic;
using System . ComponentModel . DataAnnotations;
using System . Configuration;
using System . Diagnostics;
using System . Globalization;
using System . Security . Policy;
using System . Text;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Data;
using System . Windows . Documents;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Media . TextFormatting;

using NewWpfDev;
//using NewWpfDev . ConfigSettings;

namespace UtilityWindows
{
    /// <summary>
    /// Interaction logic for DataViewer.xaml
    /// </summary>
    public partial class DataViewer : Window
    {
        const double LINEHEIGHT = 1.4;
        FontFamily fontFamily = new FontFamily ( "Arial" );
        public static int Fontsize { set; get; } = 14;
        public static string viewertext { get; set; } = "";
        public static string DefFontSize { get; set; } = "13";
        public static string DefFontStyle { get; set; } = "Normal";
        private static string DefFontFamily { get; set; } = "Arial";
        public static string DefFontColor { get; set; } = "Black0";
        public static string DefFontSize2 { get; set; } = "14";
        public static string DefFontStyle2 { get; set; } = "Normal";
        public static string DefFontFamily2 { get; set; } = "Arial";
        public static string DefFontColor2 { get; set; } = "Black0";
        public bool IsDirty { get; set; } = false;
        public bool IsLoadiing { get; set; } = true;
        public bool IsFixed { get; set; } = true;
        public bool SetupCompleted { get; set; } = false;
        public bool IsIntialized { get; set; } = false;
        public int SelectedFont { get; set; } = -1;

        public DataViewer ( )
        {
            InitializeComponent ( );
            IsIntialized = true;
        }
        private void Setup ( )
        {
            if ( IsIntialized == false )
            {
                InitializeComponent ( );
                IsIntialized = true;
            }
            IsIntialized = true;
            ConfigurationManager . OpenExeConfiguration ( ConfigurationUserLevel . PerUserRoamingAndLocal );
            // setup font family combo
            LoadFontsList ( );

            // Save our fonts based on args received
            // Read them back into our local variables
            LoadFontDefaults ( );
            DefFontFamily = NewWpfDev . Properties . Settings . Default . DataViewerFontFamily;
            DefFontSize = NewWpfDev . Properties . Settings . Default . DataViewerFontSize;
            DefFontStyle = NewWpfDev . Properties . Settings . Default . DataViewerFontStyle;
            DefFontColor = NewWpfDev . Properties . Settings . Default . DataViewerFontColor;
            SetupCompleted = true;
        }
        /**************************************************************************************************************************************************/
        public DataViewer ( string textline1 , string fontstyle1 = "Normal" , string fontfamily1 = "Nirmala UI" , string fontsize1 = "14" , string fontcolor1 = "Black1" ,
                                        string textline2 = "" , string fontstyle2 = "Normal" , string fontfamily2 = "" , string fontsize2 = "0" , string fontcolor2 = "" , bool isFixed = false )
        /**************************************************************************************************************************************************/
        {
            string family = "";
            string originalInput = textline1;
            Mouse . OverrideCursor = Cursors . Wait;

            if ( IsIntialized == false )
            {
                InitializeComponent ( );
                IsIntialized = true;
            }
            ConfigurationManager . OpenExeConfiguration ( ConfigurationUserLevel . PerUserRoamingAndLocal );
            if ( SetupCompleted == false )
                Setup ( );
            // setup font size combo
            Utils . SetComboDefaultFontSizes ( Fonts , 11 , 15 , Convert . ToInt32 ( fontsize1 ) );

            if ( fontfamily1 == "" )
                fontfamily1 = DefFontFamily;
            else
                family = fontfamily1;
            Mouse . OverrideCursor = Cursors . Arrow;

        }

        public void ShowDataViewer ( string textline1 , string fontstyle1 = "" , string fontfamily1 = "" , string fontsize1 = "" , string fontcolor1 = "" ,
                                        string textline2 = "" , string fontstyle2 = "Normal" , string fontfamily2 = "Lucida Console" , string fontsize2 = "14" , string fontcolor2 = "Black1" , bool isFixed = false , bool IsExecutionHelp =true)
        {
            Mouse . OverrideCursor = Cursors . Wait;

            if ( IsIntialized == false )
            {
                InitializeComponent ( );
                IsIntialized = true;
            }

            string originalInput = textline1;

            if ( SetupCompleted == false )
                Setup ( );

            //textline1 = originalInput;
            if ( fontfamily1 == "" )
                DefFontFamily = NewWpfDev . Properties . Settings . Default . DataViewerFontFamily;
            if ( fontsize1 == "" )
                DefFontSize = NewWpfDev . Properties . Settings . Default . DataViewerFontSize;
            if ( fontstyle1 == "" )
                DefFontStyle = NewWpfDev . Properties . Settings . Default . DataViewerFontStyle;
            if ( fontcolor1 == "" )
                DefFontColor = NewWpfDev . Properties . Settings . Default . DataViewerFontColor;

            if ( DefFontFamily == "" )
            {
                DefFontFamily = "Nirmala UI";
                NewWpfDev . Properties . Settings . Default . DataViewerFontFamily = DefFontFamily;
            }
            if ( DefFontSize == "" )
            {
                DefFontSize = "14";
                NewWpfDev . Properties . Settings . Default . DataViewerFontSize = DefFontSize;
            }
            if ( DefFontStyle == "" )
            {
                DefFontStyle = "Normal";
                NewWpfDev . Properties . Settings . Default . DataViewerFontStyle = DefFontStyle;
            }
            if ( DefFontColor == "" )
            {
                DefFontColor = "Black0";
                NewWpfDev . Properties . Settings . Default . DataViewerFontColor = DefFontColor;
            }

            fontfamily1 = DefFontFamily;
            fontsize1 = DefFontSize;
            fontstyle1 = DefFontStyle;
            fontcolor1 = DefFontColor;

            if ( Fonts . Items . Count <= 0 )
            {
                // setup font size combo
                Utils . SetComboDefaultFontSizes ( Fonts , 11 , 15 , Convert . ToInt32 ( fontsize1 ) );
            }
            SelectCurrentFont ( fontfamily1 );

            IsFixed = isFixed;
            if ( isFixed )
            {
                textline1 = SetFixedFont ( textline1 , Convert . ToInt32 ( fontsize1 ) , fontcolor1 );
                fontfamily1 = "Lucida Console";
                fontsize1 = "15";
                SelectCurrentFontSize ( fontsize1 );
            }

            if ( IsExecutionHelp )
            {
                textline1 = ReturnHeaderOutputString ( textline1 );
                textline1 = CleanHeader ( textline1 );
                // OK to here
                textline1 = ParseHeaderToAddInfo ( textline1 );
            }
            LoadData ( textline1 , fontstyle1 ,
            fontsize1 , fontfamily1 , fontcolor1 ,
            textline2 , fontstyle2 == "" ? DefFontStyle2 : fontstyle2 ,
            fontfamily2 == "" ? DefFontFamily2 : fontfamily2 ,
            fontsize2 == "0" ? DefFontSize2 : fontsize2 ,
            fontcolor2 == "" ? DefFontColor2 : fontcolor2 );


            int winwidth = SetTextWidth ( textline1 , fontsize1 );
            this . Width = winwidth;
            int winheight = SetDefaultHeight ( textline1 , fontsize1 );

            this . Height = winheight;
            this . Topmost = true;
            Mouse . OverrideCursor = Cursors . Arrow;

        }
        public void LoadData ( string line1 , string style1 , string fontsize1 , string fontfamily , string color1 ,
                                            string line2 , string style2 , string fontfamily2 , string fontsize2 , string color2 )
        {
            Mouse . OverrideCursor = Cursors . Wait;

            FlowDocument myFlowDocument = new FlowDocument ( );
            if ( IsFixed == false )
                DefFontFamily = fontfamily;
            DefFontStyle = style1;
            DefFontSize = fontsize1 . ToString ( );
            DefFontColor = color1;
            myFlowDocument = CreateFlowDocumentScroll ( line1 , style1 , fontfamily , fontsize1 , color1 , line2 , style2 , fontfamily2 , fontsize2 , color2 );
            fdviewer . Document = myFlowDocument;
            //textRange = new TextRange ( fdviewer . Document . ContentStart , fdviewer . Document . ContentEnd );
            //textRange . Text = line1;
            viewertext = line1;
            fdviewer . UpdateLayout ( );
            // Set current font size in dropdown toforce update of viewer text font size correctly
            SelectCurrentFontSize ( fontsize1 );
            Mouse . OverrideCursor = Cursors . Arrow;
        }

        public FlowDocument CreateFlowDocumentScroll ( string textline1 , string fontstyle1 , string fontfamily , string fontsize1 , string color1 ,
                                                                                        string textline2 = "" , string fontstyle2 = "" , string fontfamily2 = "" , string fontsize2 = "13" , string color2 = "Black0" )
        {
            FlowDocument myFlowDocument = new FlowDocument ( );
            if ( textline1 != "" )
            {
                //NORMAL
                Paragraph para1 = CreateNewParagraph ( textline1 , fontstyle1 , fontfamily , fontsize1 , color1 );
                para1 . FontSize = Convert . ToInt32 ( fontsize1 );
                myFlowDocument . Blocks . Add ( para1 );
                if ( textline2 != "" )
                {
                    Paragraph para2 = null;
                    if ( fontstyle2 == "" ) fontstyle2 = DefFontStyle2;
                    if ( fontfamily2 == "" ) fontfamily2 = DefFontFamily2;
                    if ( fontsize2 == "0" ) fontsize2 = DefFontSize2;
                    if ( color2 == "" ) color2 = DefFontColor2;
                    para2 = CreateNewParagraph ( textline2 , fontstyle2 , fontfamily2 , fontsize2 , color2 );
                    myFlowDocument . Blocks . Add ( para2 );
                }
            }
            //Add paragraph to flowdocument
            return myFlowDocument;
        }

        public Paragraph CreateNewParagraph ( string textline , string fontstyle , string fontfamily , string fontsize , string color )
        {
            // called for each para independently
            Paragraph para1 = new Paragraph ( );
            FontFamily newFamily = new FontFamily ( "Nirmala UI" );
            para1 . FontFamily = newFamily;
            para1 . FontSize = 14;
            try
            {
                if ( fontfamily == "" && DefFontFamily != "" )
                    newFamily = new FontFamily ( DefFontFamily );
                else if ( fontfamily == "" && DefFontFamily == "" )
                    newFamily = ( FontFamily ) FindResource ( "Nirmala UI" ) as FontFamily;
                else
                    newFamily = new FontFamily ( fontfamily );
                para1 . TextIndent = 2;
                para1 . FontFamily = newFamily;
                para1 . FontSize = Convert . ToInt32 ( fontsize );
                SolidColorBrush sbrush = new SolidColorBrush ( );
                sbrush = new SolidColorBrush ( );
                if ( color != "" )
                {
                    sbrush = FindResource ( color ) as SolidColorBrush;
                }
                else
                {
                    sbrush = FindResource ( DefFontColor ) as SolidColorBrush;
                }
                para1 . Foreground = sbrush;
            }
            catch ( Exception ex_ ) { Debug . WriteLine ( $"Fonts problem : {ex_ . Message}" ); }

            //    string [ ] tweaktabs = textline . Split ( " " );

            if ( fontstyle == "Normal" )
                para1 . Inlines . Add ( new Run ( textline ) );
            else if ( fontstyle == "Bold" )
                para1 . Inlines . Add ( new Bold ( new Run ( textline ) ) );
            else if ( fontstyle == "Italic" )
                para1 . Inlines . Add ( new Italic ( new Run ( textline ) ) );
            // set family last of all to ensure it "sticks"
            para1 . FontFamily = newFamily;
            para1 . FontSize = Convert . ToInt32 ( fontsize );
            return para1;
        }

        private void Grid_Loaded ( object sender , RoutedEventArgs e )
        {
            string [ ] split = viewertext . Split ( "\n" );
            int linecount = split . Length;
            if ( linecount > 5 )
                this . Height += 30;
            else if ( linecount > 10 )
                this . Height += 50;
            else if ( linecount > 15 )
                this . Height += 70;
            IsLoadiing = false;
        }

        private void Close_Click ( object sender , RoutedEventArgs e )
        {
            this . Close ( );
        }

        private void Fonts_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {
            // handle switching of font size in the window's scrollviewer
            string newsize = "";
            string infotext = "";
            ComboBox cb = sender as ComboBox;

            if ( IsLoadiing == true ) return;

            if ( cb != null )
            {
                if ( fdviewer == null ) return;
                newsize = cb . SelectedItem . ToString ( );
                FlowDocument doc = fdviewer . Document;
                doc . Blocks . Clear ( );
                doc = CreateFlowDocumentScroll ( viewertext , DefFontStyle , DefFontFamily , newsize , DefFontColor );
                fdviewer . Document = doc;
                fdviewer . UpdateLayout ( );
                DefFontSize = newsize;
                // save new size to user defaults
                NewWpfDev . Properties . Settings . Default . DataViewerFontSize = newsize;
                NewWpfDev . Properties . Settings . Default . Save ( );
                AppSettingsHandler . AddUpdateAppSettings ( "DataViewerFontSize" , newsize );

                IsDirty = true;
            }
            SelectCurrentFontSize ( newsize );
        }
        public void LoadFontsList ( )
        {
            List<string> fonts = DapperGenericsLib . Utils . GetFontsList ( );
            fontslist . ItemsSource = fonts;
            string match = DefFontColor;
            int index = 0;
            // Highlight current font in use
            SelectCurrentFont ( DefFontFamily );
        }
        private void Fontslist_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {
            // Select font family in Windoe's ScrollViewer
            AppSettingsHandler . AddUpdateAppSettings ( "DataViewerFontFamily" , fontslist . SelectedItem . ToString ( ) );
            string selfont = "";
            if ( SelectedFont != -1 )
            {
                selfont = fontslist . SelectedItem . ToString ( );
                SelectedFont = -1;
            }
            else
                selfont = fontslist . SelectedItem . ToString ( );
            DefFontFamily = selfont;
            FlowDocument doc = fdviewer . Document;
            if ( doc == null ) return;
            doc . Blocks . Clear ( );
            doc = CreateFlowDocumentScroll ( viewertext , DefFontStyle , DefFontFamily , DefFontSize , DefFontColor );
            fdviewer . Document = doc;
            fdviewer . UpdateLayout ( );
            IsDirty = true;
        }

        private void Dataviewer_KeyDown ( object sender , KeyEventArgs e )
        {
           if ( e . Key == Key . Escape )
            {
                // close window if Escape hit
                this . Close ( );
            }
            else if ( e . Key == Key . F8 )
            {
                AppSettingsHandler . AddUpdateAppSettings ( "DataViewerFontFamily" , "Nirmala UI Bold" );
                AppSettingsHandler . AddUpdateAppSettings ( "DataViewerFontStyle" , "Normal" );
                AppSettingsHandler . AddUpdateAppSettings ( "DataViewerFontSize" , "13" );
                AppSettingsHandler . AddUpdateAppSettings ( "DataViewerFontColor" , "Black0" );
                DefFontFamily = NewWpfDev . Properties . Settings . Default . DataViewerFontFamily;
                DefFontSize = NewWpfDev . Properties . Settings . Default . DataViewerFontSize;
                DefFontStyle = NewWpfDev . Properties . Settings . Default . DataViewerFontStyle;
                DefFontColor = NewWpfDev . Properties . Settings . Default . DataViewerFontColor;
                if ( DefFontFamily != "Nirmala UI" )
                    NewWpfDev . Properties . Settings . Default . DataViewerFontFamily = "Nirmala UI";
                if ( DefFontSize != "13" )
                    NewWpfDev . Properties . Settings . Default . DataViewerFontSize = "13";
                if ( DefFontStyle != "Normal" )
                    NewWpfDev . Properties . Settings . Default . DataViewerFontStyle = "Normal";
                if ( DefFontColor != "Black0" )
                    NewWpfDev . Properties . Settings . Default . DataViewerFontColor = "Black0";
                NewWpfDev . Properties . Settings . Default . Save ( );

                DefFontFamily = "Nirmala UI";
                DefFontStyle = "Normal";
                DefFontSize = "13";
                DefFontColor = "Black0";

                FlowDocument doc = fdviewer . Document;
                if ( doc == null ) return;
                doc . Blocks . Clear ( );
                doc = CreateFlowDocumentScroll ( viewertext , DefFontStyle , DefFontFamily , DefFontSize , DefFontColor );
                fdviewer . Document = doc;
                fdviewer . UpdateLayout ( );
            }
            else if ( e . Key == Key . F7 )
            {
                string family = NewWpfDev . Properties . Settings . Default . DataViewerFontFamily;
                string fsize = NewWpfDev . Properties . Settings . Default . DataViewerFontSize;
                string style = NewWpfDev . Properties . Settings . Default . DataViewerFontStyle;
                string color = NewWpfDev . Properties . Settings . Default . DataViewerFontColor;
                viewertext = $"Current settings in Config.Config\n" +
                                        "======================\n" +
                    $"{family}\n{fsize}\n{style}\n{color}";
                FlowDocument doc = fdviewer . Document;
                if ( doc == null ) return;
                doc . Blocks . Clear ( );
                //                int fsizenum = Convert . ToInt32 ( fsize );
                doc = CreateFlowDocumentScroll ( viewertext , style , family , fsize , color );
                fdviewer . Document = doc;
                fdviewer . UpdateLayout ( );
            }
            else if ( e . Key == Key . F9 )
            {
                fdviewer . UpdateLayout ( );
                fdviewer . Refresh ( );
            }
        }

        private void SaveSingleFontInfo ( string fontstyle1 , string fontfamily1 , string fontsize1 , string fontcolor1 ,
                                                 string fontstyle2 = "Normal" , string fontfamily2 = "" , string fontsize2 = "14" , string fontcolor2 = "" )
        {
            if ( IsDirty == false ) return;
            DefFontFamily = fontfamily1;
            DefFontSize = fontsize1;
            DefFontStyle = fontstyle1;
            DefFontColor = fontcolor1;

            AppSettingsHandler . AddUpdateAppSettings ( "DataViewerFontFamily" , DefFontFamily );
            AppSettingsHandler . AddUpdateAppSettings ( "DataViewerFontStyle" , DefFontStyle );
            AppSettingsHandler . AddUpdateAppSettings ( "DataViewerFontSize" , DefFontSize . ToString ( ) );
            AppSettingsHandler . AddUpdateAppSettings ( "DataViewerFontColor" , DefFontColor );

            //if ( fontfamily2 != "" )
            //{
            //    AppSettingsHandler . AddUpdateAppSettings ( "DataViewerFontFamily2" , fontfamily2 );
            //    DefFontFamily2 = fontfamily2;
            //}
            //if ( fontstyle2 != "" )
            //{
            //    AppSettingsHandler . AddUpdateAppSettings ( "DataViewerFontStyle2" , fontstyle2 );
            //    DefFontStyle2 = fontstyle2;
            //}
            //if ( fontsize2 != 14 )
            //{
            //    AppSettingsHandler . AddUpdateAppSettings ( "DataViewerFontSize2" , fontsize2 . ToString ( ) );
            //    DefFontSize2 = fontsize2;
            //}
            //if ( fontcolor2 != "" )
            //{
            //    AppSettingsHandler . AddUpdateAppSettings ( "DataViewerFontColor2" , fontcolor2 );
            //    DefFontColor2 = fontcolor2;
            //}
            IsDirty = false;
        }
        private void SaveAllFontInfo ( string fontstyle1 , string fontfamily1 , string fontsize1 , string fontcolor1 ,
                                                 string fontstyle2 = "Normal" , string fontfamily2 = "" , string fontsize2 = "14" , string fontcolor2 = "" )
        {
            if ( IsDirty == false ) return;
            DefFontFamily = fontfamily1;
            DefFontSize = fontsize1;
            DefFontStyle = fontstyle1;
            DefFontColor = fontcolor1;

            AppSettingsHandler . AddUpdateAppSettings ( "DataViewerFontFamily" , DefFontFamily );
            AppSettingsHandler . AddUpdateAppSettings ( "DataViewerFontStyle" , DefFontStyle );
            AppSettingsHandler . AddUpdateAppSettings ( "DataViewerFontSize" , DefFontSize . ToString ( ) );
            AppSettingsHandler . AddUpdateAppSettings ( "DataViewerFontColor" , DefFontColor );

            if ( fontfamily2 != "" )
            {
                AppSettingsHandler . AddUpdateAppSettings ( "DataViewerFontFamily2" , fontfamily2 );
                DefFontFamily2 = fontfamily2;
            }
            if ( fontstyle2 != "" )
            {
                AppSettingsHandler . AddUpdateAppSettings ( "DataViewerFontStyle2" , fontstyle2 );
                DefFontStyle2 = fontstyle2;
            }
            if ( fontsize2 != "14" )
            {
                AppSettingsHandler . AddUpdateAppSettings ( "DataViewerFontSize2" , fontsize2 . ToString ( ) );
                DefFontSize2 = fontsize2 . ToString ( );
            }
            if ( fontcolor2 != "" )
            {
                AppSettingsHandler . AddUpdateAppSettings ( "DataViewerFontColor2" , fontcolor2 );
                DefFontColor2 = fontcolor2;
            }
        }
        private void LoadFontDefaults ( )
        {
            DefFontFamily = NewWpfDev . Properties . Settings . Default . DataViewerFontFamily;
            DefFontSize = NewWpfDev . Properties . Settings . Default . DataViewerFontSize;
            DefFontStyle = NewWpfDev . Properties . Settings . Default . DataViewerFontStyle;
            DefFontColor = NewWpfDev . Properties . Settings . Default . DataViewerFontColor;

            DefFontFamily2 = NewWpfDev . Properties . Settings . Default . DataViewerFontFamily2;
            DefFontSize2 = NewWpfDev . Properties . Settings . Default . SettingDataViewerFontSize2;
            DefFontStyle2 = NewWpfDev . Properties . Settings . Default . DataViewerFontStyle2;
            DefFontColor2 = NewWpfDev . Properties . Settings . Default . DataViewerFontColor2;
        }

        private void Dataviewer_Closing ( object sender , System . ComponentModel . CancelEventArgs e )
        {
            if ( IsDirty )
            {
                //    SaveAllFontInfo ( DefFontStyle , DefFontFamily , DefFontSize , DefFontColor ,
                //                              DefFontStyle2 , DefFontFamily2 , Convert . ToInt32 ( DefFontSize2 ) , DefFontColor2 );
            }
        }
        private void SelectCurrentFont ( string fontname )
        {
            int index = 0;
            // Highlight current font in use
            foreach ( string item in fontslist . Items )
            {
                if ( item == fontname )
                {
                    fontslist . SelectedIndex = index;
                    fontslist . SelectedItem = item;
                    break;
                }
                index++;
            }
            fontslist . SelectedIndex = index;
            NewWpfDev . Utils . ScrollCBRecordIntoView ( fontslist , index );
        }
        public void SelectCurrentFontSize ( string fontsize1 )
        {
            int index = 0;
            // Highlight current font size in use
            foreach ( string item in Fonts . Items )
            {
                if ( item == fontsize1 )
                {
                    Fonts . SelectedIndex = index;
                    Fonts . SelectedItem = item;
                    break;
                }
                index++;
            }
            Fonts . SelectedIndex = index;
            NewWpfDev . Utils . ScrollCBRecordIntoView ( Fonts , index );
        }

        public void RefreshDataViewerDisplay ( )
        {
            //We have to force the dumb scrollviewer to show correct settings
            //DefFontFamily = fontslist . SelectedItem . ToString ( );
            FlowDocument doc = fdviewer . Document;
            if ( doc == null ) return;
            doc . Blocks . Clear ( );
            doc = CreateFlowDocumentScroll ( viewertext , DefFontStyle , DefFontFamily , DefFontSize , DefFontColor );
            fdviewer . Document = doc;
            fdviewer . UpdateLayout ( );
            IsDirty = true;

        }

        private void fontslist_PreviewKeyDown ( object sender , KeyEventArgs e )
        {
            ComboBox cb = sender as ComboBox;
            if ( cb == null ) return;
            int selection = cb . SelectedIndex;
            SelectedFont = selection;
            Fontslist_SelectionChanged ( null , null );
        }
        public string SetFixedFont ( string input , int size , string color = "" )
        {
            // Returns a string formatter into Lucida Concole fixed width font of specified size and color
            FormattedText formattedText = null;
            Brush brush = Brushes . Black;
            if ( color != "" )
                brush = FindResource ( color ) as SolidColorBrush;
            formattedText = new FormattedText ( input , CultureInfo . GetCultureInfo ( "en-us" ) ,
               FlowDirection . LeftToRight , new Typeface ( "Lucida Console" ) , size , Brushes . Black );
            return formattedText . Text;
        }
         public static string CleanHeader ( string textline )
        {
            string [ ] lines = textline . Split ( "\n" );
            string output = "";
            string currentline = "";
            bool QuoteStart = false;
            foreach ( string line in lines )
            {
                if ( line . Trim ( ) == "" )
                    continue;
                // Handle removal of quoted text blocks
                if ( line . Trim ( ) . StartsWith ( "/*" ) )
                {
                    QuoteStart = true;
                    continue;
                }
                if ( QuoteStart == true && line . Contains ( "*/" ) == false )
                    continue;
                else if ( QuoteStart == true && line . Contains ( "*/" ) == true )
                {
                    QuoteStart = false;
                    continue;
                }
                if ( line . Trim ( ) . EndsWith ( "*/" ) )
                    continue;
                // end of Handling of removal of quoted text blocks

                if ( line . Trim ( ) . StartsWith ( "--" ) )
                    continue;
                if ( line . Trim ( ) . StartsWith ( "\t" ) )
                {
                    currentline = line . Substring ( 1 );
                    //continue;
                }
                if ( line . Trim ( ) . StartsWith ( "," ) )
                    currentline = line . Substring ( 1 );
                else
                    currentline = line;
                if ( currentline . Trim ( ) . Contains ( "\t" ) )
                {
                    currentline = currentline . Substring ( 0 , currentline . IndexOf ( "\t" ) + 1 );
                    //continue;
                }
                if ( currentline . Trim ( ) . Contains ( "--" ) )
                {
                    currentline = currentline . Substring ( 0 , currentline . IndexOf ( "--" ) );
                    //continue;
                }
                //if ( currentline . EndsWith ( "\r" ) )
                //    currentline = currentline . Substring (0, currentline . Length - 1 );

                output += currentline . TrimStart ( ) . TrimEnd ( ) + "\n";
            }
            return output;
        }
        public static string ReturnHeaderOutputString ( string Output )
        {
            int bindex = 0;
            bool success = false;

            if ( Output . Contains ( "BEGIN" ) )
            {
                bindex = Output . IndexOf ( "BEGIN" );
                if ( bindex == 0 )
                    return "BEGIN not found....";
                if ( Output . Contains ( "AS" ) )
                {
                    while ( true )
                    {
                        string tmpoutput1 = "";
                        tmpoutput1 = Output . Substring ( bindex - 12 );
                        int asindex = Output . IndexOf ( "AS" );
                        if ( tmpoutput1 == null )
                            break;
                        if ( asindex == -1 )
                            break;
                        if ( bindex - asindex < 12 )
                        {
                            tmpoutput1 = Output . Substring ( asindex );
                            if ( Output . Contains ( "AS\rBEGIN" ) || Output . Contains ( "AS\nBEGIN" ) || Output . Contains ( "AS\r\nBEGIN" ) )
                            {
                                if ( Output . IndexOf ( "AS\rBEGIN" ) != -1 )
                                {
                                    asindex = Output . IndexOf ( "AS\rBEGIN" );
                                    Output = Output . Substring ( 0 , asindex );
                                    break;
                                }
                                else if ( Output . IndexOf ( "AS\nBEGIN" ) != -1 )
                                {
                                    asindex = Output . IndexOf ( "AS\nBEGIN" );
                                    Output = Output . Substring ( 0 , asindex );
                                    break;
                                }
                                else if ( Output . IndexOf ( "AS\r\nBEGIN" ) != -1 )
                                {
                                    asindex = Output . IndexOf ( "AS\r\nBEGIN" );
                                    Output = Output . Substring ( 0 , asindex );
                                    break;
                                }
                            }
                            else
                                bindex -= 12;
                        }
                        if ( tmpoutput1 . Contains ( "BEGIN" ) )
                        {
                            bindex = tmpoutput1 . IndexOf ( "BEGIN" );
                            if ( bindex - asindex < 10 )
                                Output = Output . Substring ( 0 , Output . IndexOf ( "AS" ) );
                            else
                            {
                                if ( Output . Substring ( bindex - 10 ) . Contains ( "AS" ) )
                                {
                                    asindex = Output . Substring ( 0 , bindex - 10 ) . IndexOf ( "AS" );
                                }
                            }
                            int aspos = Output . IndexOf ( "AS" );
                            if ( aspos >= 0 )
                                Output = Output . Substring ( 0 , Output . IndexOf ( "AS" ) );
                        }
                    }
                }
            }
            else
            {
                Output = $"Header Block appears to be  Invalid...\n{Output}";
                success = false;
                return Output;
            }
            success = true;
            return Output;

        }
        public static string ParseHeaderToAddInfo ( string textline )
        {
            string output = "";
            string [ ] lines = textline . Split ( "\n" );
            string [ ] mlines = null;
            string newoutput = "";
            bool success = true;
            foreach ( string line in lines )
            {
                int mcount = 1;
                bool isoutput = false;
                string [ ] newwords = new string [ 5 ];
                newwords = PadArgsArray ( newwords );
                if ( line . Contains ( "," ) )
                {
                    mlines = line . Split ( "," );
                    mcount++;
                }
                newoutput += ParseArgWordsToMessage ( line );
            }
            string [ ] checker = newoutput . Split ( "\n" );
            if ( checker . Length == 2 && checker [ 1 ] == "" )
                newoutput += "\nNo arguments are specified for this procedure.... ";
            return newoutput;
            if ( success == true )
                return newoutput;
            else
                return output;
        }

        public static string ParseArgWordsToMessage ( string input )
        {
            string argname = "";
            string infoline = "";
            string newoutput = "";
            string DefaultValue = "";
            int wordsindex = 0;
            bool success = true;
            string currentline = input;
            string [ ] newwords = new string [ 5 ];
            string dummytab = "     ";
            newwords = PadArgsArray ( newwords );
            currentline = CleanHeader ( currentline );

            string [ ] words = currentline . Split ( " " );
            try
            {
                bool isoutput = false;
                if ( words . Length == 1 )
                    success = false;
                else
                {
                    if ( currentline . Contains ( "CREATE PROCEDURE" ) == true )
                        return currentline; ;
                    foreach ( string word in words )
                    {
                        string nextword = word;
                        nextword = nextword . Trim ( ) . ToUpper ( );

                        if ( nextword . Contains ( "=" ) )
                        {
                            // Check for default value
                            string [ ] defvalue = nextword . Split ( "=" );
                            DefaultValue = defvalue [ 1 ];
                        }

                        if ( nextword . StartsWith ( "@" ) )
                        {
                            newwords [ wordsindex++ ] = nextword;
                            continue;
                        }
                        else if ( nextword . Contains ( "INT" ) )
                        {
                            newwords [ wordsindex++ ] = $"INTEGER";
                            continue;
                        }
                        else if ( nextword . Contains ( "MAX" ) || nextword . Contains ( "SYSNAME" ) )
                        {
                            newwords [ wordsindex++ ] = $" Size is 32000)";
                            continue;
                        }
                        else if ( nextword . Contains ( "VARCHAR" ) )
                        {
                            newwords [ wordsindex++ ] = "STRING";
                            //continue;
                        }
                        if ( nextword . Contains ( "(" ) && nextword . Contains ( ")" ) )
                        {
                            string defsize = nextword . Substring ( nextword . IndexOf ( "(" ) );
                            if ( defsize . Contains ( "((" ) )
                                defsize = defsize . Substring ( 0 , defsize . IndexOf ( "))" ) +1);
                            if ( defsize . Contains ( "," ) )
                                    defsize = defsize.Substring(0, defsize.IndexOf ( "," ));
                            if ( defsize . Contains ( "\t" ) )
                                defsize = defsize . Substring ( 0 , defsize . IndexOf ( "\t" ));
                            if ( defsize . Contains ( "--" ) )
                                defsize = defsize . Substring ( 0 , defsize . IndexOf ( "--" ));

                            if ( defsize . Contains ( "=" )== true )
                                defsize = defsize . Substring ( 0 , defsize . IndexOf ( "=" ) );

                            newwords [ wordsindex++ ] = $"\n{dummytab}*** Max Size is {defsize}) ***";
                            continue;
                        }
                        else if ( nextword . Contains ( "OUTPUT" ) )
                        {
                            newwords [ wordsindex++ ] = $"OUTPUT";
                            isoutput = true;
                            continue;
                        }
                    }
                    if ( wordsindex > 0 )
                    {
                        newoutput = $"{currentline}{dummytab}";
                        //if ( newoutput . Contains ( "OUTPUT" ))
                        //            isoutput = true;
                        for ( int x = 0 ; x < wordsindex ; x++ )
                        {
                            if ( newwords [ x ] == "" )
                                continue;
                            else
                            {
                                if ( isoutput && x == 1 )
                                    newoutput += $"is an OUTPUT";
                                else if ( x == 1 )
                                    newoutput += $"is an INPUT";
                                if ( newwords [ x ] != "OUTPUT" )
                                    newoutput += $" {newwords [ x ]} ";
                            }
                        }
                        if ( DefaultValue != "" )
                        {
                            newoutput += $"\n{dummytab}Argument provides default of {DefaultValue . ToUpper ( )}";
                            newoutput += $" & so this is\n{dummytab}an OPTIONAL argument\n";
                        }
                        if ( DefaultValue . Contains ( "NULL" ) )
                            newoutput += $"{dummytab}& is therefore an optional argument\n";
                        newoutput += "\n";
                    }
                    success = true;
                }
            }
            catch ( Exception ex ) { }

            return newoutput;
        }
        public static string [ ] PadArgsArray ( string [ ] content )
        {
            // replaces all NULL lines with ""
            string [ ] tmp = new string [ Views . SpResultsViewer . DEFAULTARGSSIZE ];
            for ( int x = 0 ; x < Views . SpResultsViewer . DEFAULTARGSSIZE ; x++ )
            {
                if ( content . Length - 1 >= x )
                {
                    if ( content [ x ] != null )
                        tmp [ x ] = content [ x ];
                    else
                        tmp [ x ] = "";
                }
                else
                    tmp [ x ] = "";
            }
            return tmp;
        }

        public static int SetDefaultHeight ( string textin , string fontsize )
        {
            string [ ] lines;
            int maxlines = 0;
            lines = textin . Split ( "\n" );
            if ( lines . Length <= 1 )
            {
                lines = textin . Split ( "\r" );
                if ( lines . Length == 1 )
                {
                    lines = textin . Split ( "\f\n" );
                    maxlines = lines . Length;
                }
                else
                    maxlines = lines . Length;
            }
            else
                maxlines = lines . Length;

            int fheight = Convert . ToInt32 ( fontsize );
            double heightfiddle = 0;
            switch ( fheight )
            {
                case <= 12:
                    heightfiddle = 2.7;
                    break;
                case <= 15:
                    heightfiddle = 2.95;
                    break;
                case <= 18:
                    heightfiddle = 2.8;
                    break;
                case <= 21:
                    heightfiddle = 2.5;
                    break;
                case <= 23:
                    heightfiddle = 2.5;
                    break;
                case <= 25:
                    heightfiddle = 2.6;
                    break;
                case > 25:
                    heightfiddle = 2.4;
                    break;
            }
            int heightconvert = Convert . ToInt32 ( fheight *( heightfiddle  / 1.4));
            if ( maxlines > 0 )
            {
                int newheight = Convert . ToInt32 ( heightconvert  );
                int totlheight = ( ( maxlines * newheight ) );
                return Convert . ToInt32 ( totlheight );
                //                return Convert . ToInt32 ( totlheight * LINEHEIGHT );
            }
            return fheight * 2;
        }
        public static int SetTextWidth ( string textin , string fontsize )
        {
            double charwidth = 2.3;
            string [ ] lines;
            int maxlength = 0;
            int fwidth = Convert . ToInt32 ( fontsize );
            charwidth = charwidth * ( Convert . ToInt32 ( fontsize ) / 4.6 );
            lines = textin . Split ( "\n" );
            foreach ( var line in lines )
            {
                if ( line . Length > maxlength )
                    maxlength = Convert . ToInt32 ( line . Length * ( charwidth ) );
            }
            int newwidth = maxlength;
            double widthfiddle = 0;
            switch ( Convert . ToInt32 ( Convert . ToInt32 ( fontsize ) ) )
            {
                case < 12:
                    widthfiddle = 0.9;
                    break;
                case < 15:
                    widthfiddle = 1.1;
                    break;
                case < 18:
                    widthfiddle = 1.2;
                    break;
                case < 21:
                    widthfiddle = 1.3;
                    break;
                case < 23:
                    widthfiddle = 1.3;
                    break;
                case < 25:
                    widthfiddle = 1.5;
                    break;
                case >= 25:
                    widthfiddle = 1.25;
                    break;
            }
            int totwidth = Convert . ToInt32 ( ( newwidth * widthfiddle ) );
            return totwidth;
        }
    }
}

