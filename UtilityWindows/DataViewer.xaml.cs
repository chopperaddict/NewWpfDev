using System;
using System . Collections . Generic;
using System . Configuration;
using System . Security . Policy;
using System . Text;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Data;
using System . Windows . Documents;
using System . Windows . Input;
using System . Windows . Media;

using ConfigSettings;

using NewWpfDev;

namespace UtilityWindows
{
    /// <summary>
    /// Interaction logic for DataViewer.xaml
    /// </summary>
    public partial class DataViewer : Window
    {
        FontFamily fontFamily = new FontFamily ( "Arial" );
        public static int Fontsize { set; get; } = 14;
        public static string viewertext { get; set; } = "";
        public static int DefFontSize { get; set; } = 14;
        public static string DefFontStyle { get; set; } = "Normal";
        private  static string DefFontFamily { get; set; } = "Arial";
        public static string DefFontColor { get; set; } = "Black0";
        public static int DefFontSize2 { get; set; } = 14;
        public static string DefFontStyle2 { get; set; } = "Normal";
        public static string DefFontFamily2 { get; set; } = "Arial";
        public static string DefFontColor2 { get; set; } = "Black0";
        public bool IsDirty { get; set; } = false;
        public bool IsLoadiing{ get; set; } = true;
        /**************************************************************************************************************************************************/
        public DataViewer ( string textline1 , string fontstyle1 , string fontfamily1 , int fontsize1 , string fontcolor1 ,
                                        string textline2 = "" , string fontstyle2 = "Normal" , string fontfamily2 = "" , int fontsize2 = 0 , string fontcolor2 = "" )
        /**************************************************************************************************************************************************/
        {
            InitializeComponent ( );
            ConfigurationManager . OpenExeConfiguration ( ConfigurationUserLevel . PerUserRoamingAndLocal );
            // setup font family combo
            LoadFontsList ( );
            // setup font size combo
            Utils . SetDefaultFontSizes ( Fonts , 11 , 10 , fontsize1 );

            // Save our fonts based on args received
               // Read them back into our local variables
            LoadFontDefaults ( );

            if ( fontfamily1 == "" )
                fontfamily1 = DefFontFamily;

            LoadData ( textline1 , fontstyle1, 
                fontsize1 , fontfamily1 , fontcolor1 ,
                textline2 , fontstyle2=="" ? DefFontStyle2 : fontstyle2, 
                fontfamily2 == "" ? DefFontFamily2 : fontfamily2, 
                fontsize2==0 ? DefFontSize2 : fontsize2, 
                fontcolor2=="" ? DefFontColor2  : fontcolor2);
        }

        public void LoadData ( string line1 , string style1 , int fontsize1 , string fontfamily , string color1 ,
                                            string line2 , string style2 , string fontfamily2 , int fontsize2 , string color2 )
        {
            TextRange textRange;
            FlowDocument myFlowDocument = new FlowDocument ( );
            DefFontFamily = fontfamily;
            DefFontStyle = style1;
            DefFontSize = fontsize1;
            DefFontColor = color1;
            SolidColorBrush brush = FindResource ( DefFontColor ) as SolidColorBrush;
            DefFontFamily = AppSettingsHandler . ReadSetting ( fontfamily );
            myFlowDocument = CreateFlowDocumentScroll ( line1 , style1 , fontfamily , fontsize1 , color1 , line2 , style2 , fontfamily2 , fontsize2 , color2 );
            fdviewer . Document = myFlowDocument;
            textRange = new TextRange ( fdviewer . Document . ContentStart , fdviewer . Document . ContentEnd );
            textRange . Text = line1;
            viewertext = line1;
        }

        public FlowDocument CreateFlowDocumentScroll ( string textline1 , string fontstyle1 , string fontfamily , int fontsize1 , string color1 , string
                                                                                            textline2 = "" , string fontstyle2 = "" , string fontfamily2 = "" , int fontsize2 = 13 , string color2 = "Black0" )
        {
            FlowDocument myFlowDocument = new FlowDocument ( );
            if ( textline1 != "" )
            {
                Paragraph para2 = null;
                //NORMAL
                Paragraph para1 = CreateNewParagraph ( textline1 , fontstyle1 , fontfamily , fontsize1 , color1 );
                myFlowDocument . Blocks . Add ( para1 );
                if ( textline2 != "" )
                {
                    if ( fontstyle2 == "" ) fontstyle2 = DefFontStyle2;
                    if ( fontfamily2 == "" ) fontfamily2 = DefFontFamily2;
                    if ( fontsize2 == 0 ) fontsize2 = DefFontSize2;
                    if ( color2 == "" ) color2 = DefFontColor2;
                    para2 = CreateNewParagraph ( textline2 , fontstyle2 , fontfamily2 , fontsize2 , color2 );
                    myFlowDocument . Blocks . Add ( para2 );
                }
            }
            //Add paragraph to flowdocument
            return myFlowDocument;
        }

        public   Paragraph CreateNewParagraph ( string textline , string fontstyle , string fontfamily , int fontsize , string color )
        {
            // called for each para independently
            Paragraph para1 = new Paragraph ( );
            para1 . FontSize = 14;
            para1 . FontSize = fontsize;

            if ( fontfamily == "" && DefFontFamily != "" )
                para1 . FontFamily = ( FontFamily ) FindResource ( DefFontFamily ) as FontFamily ;
            else if ( fontfamily == "" && DefFontFamily == "" )
                para1 . FontFamily =(FontFamily) FindResource("Nirmala UI") as FontFamily;
            else
                para1 . FontFamily = FindResource(fontfamily) as FontFamily;

            if ( color != "" ) para1 . Foreground = FindResource ( color . Trim ( ) ) as SolidColorBrush;
            else para1 . Foreground = FindResource ( color ) as SolidColorBrush;

            if ( fontstyle == "Normal" ) para1 . Inlines . Add ( new Run ( textline ) );
            else if ( fontstyle == "Bold" ) para1 . Inlines . Add ( new Bold ( new Run ( textline ) ) );
            else if ( fontstyle == "Italic" ) para1 . Inlines . Add ( new Italic ( new Run ( textline ) ) );

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

        private void Button_Click ( object sender , RoutedEventArgs e )
        {

        }

        private void Close_Click ( object sender , RoutedEventArgs e )
        {
            this . Close ( );
        }

        private void Fonts_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {
            string newsize = "";
            string infotext = "";
            ComboBox cb = sender as ComboBox;
            
            if ( IsLoadiing == true ) return;

            if ( cb != null )
            {
                if ( fdviewer . Document == null ) return; ;
                newsize = cb . SelectedItem . ToString ( );
                Fontsize = Convert . ToInt32 ( newsize );
                FlowDocument doc = fdviewer . Document;
                doc . Blocks . Clear ( );
                doc = CreateFlowDocumentScroll ( viewertext , DefFontStyle , DefFontFamily , Fontsize , DefFontColor );
                fdviewer . Document = doc;
                fdviewer . UpdateLayout ( );
                IsDirty = true;
            }

        }
        public void LoadFontsList ( )
        {
            List<string> fonts = DapperGenericsLib . Utils . GetFontsList ( );
            fontslist . ItemsSource = fonts;
            string match = DefFontColor;
            ;
            int index = 0;
            // Highlight current font in use
            foreach ( string item in fontslist . Items )
            {
                if ( item == DefFontFamily )
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

        private void Fontslist_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {
            if ( IsLoadiing ) return;
            // Select font family
            AppSettingsHandler . AddUpdateAppSettings ( "FlowDocFontFamily" , fontslist . SelectedItem . ToString ( ) );
            DefFontFamily = fontslist . SelectedItem . ToString ( );
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
            if ( e . Key == Key . F8 )
            {
                AppSettingsHandler . AddUpdateAppSettings ( "FlowDocFontFamily" , "Nirmala UI" );
                AppSettingsHandler . AddUpdateAppSettings ( "FlowDocFontStyle" , "Normal" );
                AppSettingsHandler . AddUpdateAppSettings ( "FlowDocFontSize" , "13" );
                AppSettingsHandler . AddUpdateAppSettings ( "FlowDocFontColor" , "Black0" );
                DefFontFamily = "Nirmala UI";
                DefFontStyle = "Normal";
                DefFontSize = 13;
                DefFontColor = "Black0";
                FlowDocument doc = fdviewer . Document;
                if ( doc == null ) return;
                doc . Blocks . Clear ( );
                doc = CreateFlowDocumentScroll ( viewertext , DefFontStyle , DefFontFamily , DefFontSize , DefFontColor );
                fdviewer . Document = doc;
                fdviewer . UpdateLayout ( );
            }
        }
        private void SaveFontInfo ( string fontstyle1 , string fontfamily1 , int fontsize1 , string fontcolor1 ,
                                                 string fontstyle2 = "Normal" , string fontfamily2 = "" , int fontsize2 = 14 , string fontcolor2 = "" )
        {
            if ( IsDirty == false ) return;
            DefFontFamily = fontfamily1;
            DefFontSize = fontsize1;
            DefFontStyle = fontstyle1;
            DefFontColor = fontcolor1;

            AppSettingsHandler . AddUpdateAppSettings ( "FlowDocFontFamily" , DefFontFamily );
            AppSettingsHandler . AddUpdateAppSettings ( "FlowDocFontFamily" , DefFontFamily );
            AppSettingsHandler . AddUpdateAppSettings ( "FlowDocFontStyle" , DefFontStyle );
            AppSettingsHandler . AddUpdateAppSettings ( "FlowDocFontSize" , DefFontSize . ToString ( ) );
            AppSettingsHandler . AddUpdateAppSettings ( "FlowDocFontColor" , DefFontColor );

            if ( fontfamily2 != "" )
            {
                AppSettingsHandler . AddUpdateAppSettings ( "FlowDocFontFamily2" , fontfamily2 );
                DefFontFamily2 = fontfamily2;
            }
            if ( fontstyle2 != "" )
            {
                AppSettingsHandler . AddUpdateAppSettings ( "FlowDocFontStyle2" , fontstyle2);
                DefFontStyle2 = fontstyle2;
            }
            if ( fontsize2 != 14 )
            {
                AppSettingsHandler . AddUpdateAppSettings ( "FlowDocFontSize2" , fontsize2 . ToString ( ) );
                DefFontSize2 = fontsize2;
            }
            if ( fontcolor2 != "" )
            {
                AppSettingsHandler . AddUpdateAppSettings ( "FlowDocFontColor2" , fontcolor2 );
                DefFontColor2 = fontcolor2;
            }
        }
        private void LoadFontDefaults ( )
        {
            DefFontFamily = ( string ) AppSettingsHandler . ReadSetting ( "FlowDocFontFamily" );
            DefFontStyle = ( string )  AppSettingsHandler . ReadSetting ( "FlowDocFontStyle" );
            DefFontSize = ( int ) Convert . ToInt32 ( AppSettingsHandler . ReadSetting ( "FlowDocFontSize" ) );
            DefFontColor = ( string ) AppSettingsHandler . ReadSetting ( "FlowDocFontColor" );

            DefFontFamily2 = ( string ) AppSettingsHandler . ReadSetting ( "FlowDocFontFamily2" );
            DefFontStyle2 = ( string ) AppSettingsHandler . ReadSetting ( "FlowDocFontStyle2" );
            //DefFontSize2 = ( int ) Convert . ToInt32 ( AppSettingsHandler . ReadSetting ( "FlowDocFontSize2" ) );
            DefFontColor2 = ( string ) AppSettingsHandler . ReadSetting ( "FlowDocFontColor2" );
            IsDirty = false;
        }

        private void Dataviewer_Closing ( object sender , System . ComponentModel . CancelEventArgs e )
        {
            SaveFontInfo ( DefFontStyle , DefFontFamily , DefFontSize , DefFontColor ,
                                          DefFontStyle2 ,DefFontFamily2, DefFontSize2 , DefFontColor2);
        }
    }
}
