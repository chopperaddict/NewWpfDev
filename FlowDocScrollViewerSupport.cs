using System;
using System . Diagnostics;
using System . Linq;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Documents;

using Views;

namespace NewWpfDev
{
    /// <summary>
    /// Support methods , mostly to suport finding selected text (highlighted as Bold) 
    /// for the std FlowDocumentScrllViewer, but should work with anny variety of this (RichTextBox etc > )
    /// as they all use FlowDocuent internally
    /// </summary>
    public class FlowDocScrollViewerSupport : FlowDocumentScrollViewer
    {
        public static bool USEMODIFIED { get; set; } = true;
        public static void OnFindCommand ( FlowDocumentScrollViewer fdsv , FlowDocument fd , string Searchtext )
        {
            double offset = ExtractStyleChanges ( fd , Genericgrid . SpSearchTerm );
            //Try to Scroll viewer to matching word position
            // It varies depending on wordwrap, viewer width etc etc
            // but it gets fairly close to it anyway
            ScrollViewer sv = fdsv . Template . FindName ( "PART_ContentHost" , ( FrameworkElement ) fdsv ) as ScrollViewer;
            if ( sv != null )
            {
                sv . ScrollToBottom ( );
                sv . ScrollToTop ( );
                sv . ScrollToVerticalOffset ( offset );
            }
        }

        public static double ExtractStyleChanges ( FlowDocument doc , string srchterm )
        {
            int offset = 0;
            string [ ] strings;
            if ( doc . Blocks . Count == 1 )
            {
                // only got a single block in our viewer
                var p = doc . Blocks . OfType<Paragraph> ( );
                var startpara = p . ElementAtOrDefault ( 0 );
                InlineCollection startinline = startpara . Inlines;
                int position = 0, linefeeds = 0, tabs = 0;
                int soffset = 0;
                // usually more than 1 Inlines exist in most paragraphs, so try to find our search term in them
                foreach ( var item in startinline )
                {
                    var line = item . ContentStart;

                    if ( USEMODIFIED )
                    {
                        TextPointer start = item . ContentStart;
                        TextPointer end = item . ContentEnd;
                        Paragraph startpar = start . Paragraph;
                        InlineCollection startinlin = startpara . Inlines;
                        TextRange tempRange = new TextRange ( start , end );
                        if ( tempRange . Text . Contains ( srchterm ) )
                        {
                            // Found it !!!!
                            position = soffset += tempRange . Text . IndexOf ( srchterm );
                            if ( soffset < 200 )
                                soffset = soffset - 100;
                            else if ( soffset < 300 )
                                soffset = ( int ) ( ( double ) soffset - ( ( double ) soffset / 4.5 ) );
                            else if ( soffset < 400 )
                                soffset = ( int ) ( ( double ) soffset - ( ( double ) soffset / 3.0) );
                            else if ( soffset < 500 )
                                soffset = ( int ) ( ( double ) soffset - ( ( double ) soffset / 2.0 ) );
                            else if ( soffset < 600 )
                                soffset = ( int ) ( ( double ) soffset - ( ( double ) soffset / 2.5 ) );
                            else if ( soffset <700 )
                                soffset = ( int ) ( ( double ) soffset - ( ( double ) soffset / 2.7 ) );
                            else if ( soffset < 800 )
                                soffset = ( int ) ( ( double ) soffset - ( ( double ) soffset /3.0 ) );
                            else if ( soffset < 900 )
                                soffset = ( int ) ( ( double ) soffset - ( ( double ) soffset / 2.0 ) );
                            else if ( soffset < 1000 )
                                soffset = ( int ) ( ( double ) soffset - ( ( double ) soffset / 6.0 ) );
                            else if ( soffset < 1200 )
                                soffset = ( int ) ( ( double ) soffset - ( ( double ) soffset / 2.5 ) );
 
                            // save to return value
                            offset = soffset;
                            Debug . WriteLine ($"TextSearch match found...");
                            break;
                        }
                        else
                        {
                            // none in this  block, so store length then count tabs 
                            // and linefeeds and finally carry on to next block (if any)
                            string blocktxt = tempRange . Text . ToString ( );
                            soffset += blocktxt . Length;
                            // count linefeeds
                            strings = blocktxt . Split ( "\r\n" );
                            linefeeds += strings . Length;   // there are 2 characters in Cr/Lf pair!!!!
                            // count tabs
                            strings = blocktxt . Split ( "\t" );
                            tabs = +strings . Length;
                            continue;
                        }
                    }
                    else
                    {
                        var actualtext = line . GetTextInRun ( LogicalDirection . Forward );
                        if ( actualtext . ToUpper ( ) == srchterm )
                        {
                            offset = ( int ) GetCharOffsetFromPointer ( line , doc );
                             break;
                        }
                    }
                }
                return offset;
            }
            else
            {
                foreach ( var p in doc . Blocks . OfType<Paragraph> ( ) )
                    foreach ( var i in p . Inlines )
                    {
                        TextPointer start = i . ContentStart;
                        TextPointer end = i . ContentEnd;
                        Paragraph startpara = start . Paragraph;
                        InlineCollection startinline = startpara . Inlines;

                        TextRange tempRange = new TextRange ( start , end );
                        int off = 0;
                        if ( tempRange . Text . Contains ( srchterm ) )
                            off = tempRange . Text . IndexOf ( srchterm );
                        string txt1 = "Selection starts at character #" + tempRange . Text . Length + Environment . NewLine;
  
                        foreach ( var item in startinline )
                        {
                            var line = item . ContentStart;
                             var actualtext = line . GetTextInRun ( LogicalDirection . Forward );
                            if ( actualtext . ToUpper ( ) == srchterm )
                            {
                                offset = GetCharOffsetFromPointer ( line , doc );
                                 break;
                            }
                        }
                        if ( offset > ( double ) 0 )
                            break;
                    }
            }
            return offset;
        }
        public static int GetCharOffsetFromPointer ( TextPointer pointer , FlowDocument document )
        {
            if ( document == null ) return 0;
            int counter = 0;
            while ( ( pointer != null ) && ( pointer . CompareTo ( document . ContentStart ) > 0 ) )
            {
                if ( pointer . GetPointerContext ( LogicalDirection . Backward ) == TextPointerContext . Text )
                {
                    String textRun = pointer . GetTextInRun ( LogicalDirection . Backward );
                    counter += textRun . Length;
                    pointer = pointer . GetPositionAtOffset ( -textRun . Length );
                }
                else
                {
                    pointer = pointer . GetNextInsertionPosition ( LogicalDirection . Backward );
                    counter++;
                }
            }
            return counter;
        }

        public TextPointer GetPointerFromCharOffset ( int charOffset , FlowDocument document )
        {
            int counter = 0;
            TextPointer result = document . ContentStart;
            while ( ( result != null ) && ( counter < charOffset ) )
            {
                if ( result . GetPointerContext ( LogicalDirection . Forward ) == TextPointerContext . Text )
                {
                    String textRun = result . GetTextInRun ( LogicalDirection . Forward );
                    counter += textRun . Length;
                    result = result . GetPositionAtOffset ( textRun . Length );
                }
                else
                {
                    result = result . GetNextInsertionPosition ( LogicalDirection . Forward );
                    counter++;
                }
            }
            if ( result == null ) result = document . ContentEnd;
            return result;
        }

    }
}
