using System;
using System . Collections . Generic;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows . Media;
using System . Windows;
using Newtonsoft . Json . Linq;

namespace NewWpfDev. Views
{
    public class ListboxColorCtrlAP : DependencyObject
    {
        #region Attached Properties


        #region Background 
        public static readonly DependencyProperty BackgroundProperty
              = DependencyProperty . RegisterAttached(
              "Background" ,
              typeof(Brush) ,
              typeof(ListboxColorCtrlAP) ,
              new PropertyMetadata(Brushes . Aquamarine) , OnBackgroundChanged);
        public static Brush GetBackground (DependencyObject d)
        {
            return ( Brush )d . GetValue(BackgroundProperty);
        }
        public static void SetBackground (DependencyObject d , Brush value)
        {
            //Debug. WriteLine ( $"AP : setting Background to {value}" );
            d . SetValue(BackgroundProperty , value);
        }
        private static bool OnBackgroundChanged (object value)
        {
            //Debug. WriteLine ( $"AP : OnBackgroundchanged = {value}" );
            return true;
        }
        #endregion Background

        #region BackgroundColor AP
        public static readonly DependencyProperty BackgroundColorProperty
             = DependencyProperty . RegisterAttached(
             "BackgroundColor" ,
             typeof(Brush) ,
             typeof(ListboxColorCtrlAP) ,
             new PropertyMetadata(Brushes . Aquamarine) , OnBackgroundColorChanged);
        public static Brush GetBackgroundColor (DependencyObject d)
        {
            return ( Brush )d . GetValue(BackgroundColorProperty);
        }
        public static void SetBackgroundColor (DependencyObject d , Brush value)
        {
            //Debug. WriteLine ( $"AP : setting Background to {value}" );
            d . SetValue(BackgroundColorProperty , value);
        }
        private static bool OnBackgroundColorChanged (object value)
        {// Debug. WriteLine ( $"AP : OnBackgroundColorchanged = {value}" );
            return true;
        }
        #endregion BackgroundColor

        #region BorderBrush
        public static Brush GetBorderBrush (DependencyObject obj)
        {
            return ( Brush )obj . GetValue(BorderBrushProperty);
        }

        public static void SetBorderBrush (DependencyObject obj , Brush value)
        {
            obj . SetValue(BorderBrushProperty , value);
        }

        // Using a DependencyProperty as the backing store for BorderBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BorderBrushProperty =
                DependencyProperty . RegisterAttached("BorderBrush" , typeof(Brush) , typeof(ListboxColorCtrlAP) , new PropertyMetadata(Brushes . Transparent));
        #endregion BorderBrush

        #region BorderThickness
        public static Thickness GetBorderThickness (DependencyObject obj)
        {
            return ( Thickness )obj . GetValue(BorderThicknessProperty);
        }

        public static void SetBorderThickness (DependencyObject obj , Thickness value)
        {
            obj . SetValue(BorderThicknessProperty , value);
        }

        // Using a DependencyProperty as the backing store for BorderThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BorderThicknessProperty =
                DependencyProperty . RegisterAttached("BorderThickness" , typeof(Thickness) , typeof(ListboxColorCtrlAP) , new PropertyMetadata(default));
        #endregion BorderThickness

        #region FontSize
        public static double GetFontSize (DependencyObject obj)
        {
            return ( double )obj . GetValue(FontSizeProperty);
        }

        public static void SetFontSize (DependencyObject obj , double value)
        {
            obj . SetValue(FontSizeProperty , value);
        }

        // Using a DependencyProperty as the backing store for FontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FontSizeProperty =
                DependencyProperty . RegisterAttached("FontSize" , typeof(double) , typeof(ListboxColorCtrlAP) , new PropertyMetadata(( double )14));
        #endregion FontSize

        #region FontWeight
        public static FontWeight GetFontWeight (DependencyObject obj)
        {
            return ( FontWeight )obj . GetValue(FontWeightProperty);
        }

        public static void SetFontWeight (DependencyObject obj , FontWeight value)
        {
            obj . SetValue(FontWeightProperty , value);
        }

        // Using a DependencyProperty as the backing store for FontWeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FontWeightProperty =
                DependencyProperty . RegisterAttached("FontWeight" , typeof(FontWeight) , typeof(ListboxColorCtrlAP) , new PropertyMetadata(default));
        #endregion FontWeight

        #region FontWeightSelected
        public static FontWeight GetFontWeightSelected (DependencyObject obj)
        {
            return ( FontWeight )obj . GetValue(FontWeightSelectedProperty);
        }

        public static void SetFontWeightSelected (DependencyObject obj , FontWeight value)
        {
            obj . SetValue(FontWeightSelectedProperty , value);
        }

        // Using a DependencyProperty as the backing store for FontWeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FontWeightSelectedProperty =
                DependencyProperty . RegisterAttached("FontWeightSelected" , typeof(FontWeight) , typeof(ListboxColorCtrlAP) , new PropertyMetadata(( FontWeight )FontWeight . FromOpenTypeWeight(400)) , OnFontWeightSelectedChanged);

        private static bool OnFontWeightSelectedChanged (object value)
        {
            //Debug. WriteLine ( $"FontWeightSelected has been reset to {value}" );
            return true;
        }
        #endregion FontWeight

        #region Foreground
        public static readonly DependencyProperty ForegroundProperty
             = DependencyProperty . RegisterAttached(
             "Foreground" ,
             typeof(Brush) ,
             typeof(ListboxColorCtrlAP) ,
             new PropertyMetadata(Brushes . Black) , OnForegroundChanged);

        public static Brush GetForeground (DependencyObject d)
        {
            return ( Brush )d . GetValue(ForegroundProperty);
        }
        public static void SetForeground (DependencyObject d , Brush value)
        {
            d . SetValue(ForegroundProperty , value);
        }
        private static bool OnForegroundChanged (object value)
        {
            //Debug. WriteLine ( $"AP : OnForegroundchanged = {value}" );
            return true;
        }
        #endregion Foreground

        #region ItemHeight
        public static readonly DependencyProperty ItemHeightProperty
                    = DependencyProperty . RegisterAttached(
                    "ItemHeight" ,
                    typeof(double) ,
                    typeof(ListboxColorCtrlAP) ,
                    new PropertyMetadata(( double )20) , OnItemheightChanged);

        public static double GetItemHeight (DependencyObject d)
        {
            return ( double )d . GetValue(ItemHeightProperty);
        }
        public static void SetItemHeight (DependencyObject d , double value)
        {
            d . SetValue(ItemHeightProperty , value);
        }
        private static bool OnItemheightChanged (object value)
        {
            //Debug. WriteLine ( $"AP : ONItemHeightchanged = {value}" );

            return true;
        }
        #endregion ItemHeight

        #region SelectionBackground 
        public static Brush GetSelectionBackground (DependencyObject obj)
        {
            return ( Brush )obj . GetValue(SelectionBackgroundProperty);
        }

        public static void SetSelectionBackground (DependencyObject obj , Brush value)
        {
            obj . SetValue(SelectionBackgroundProperty , value);
        }

        // Using a DependencyProperty as the backing store for SelectionBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectionBackgroundProperty =
                DependencyProperty . RegisterAttached("SelectionBackground" , typeof(Brush) , typeof(ListboxColorCtrlAP) , new PropertyMetadata(Brushes . Blue));

        #endregion SelectionBackground

        #region SelectionForeground
        public static Brush GetSelectionForeground (DependencyObject obj)
        {
            return ( Brush )obj . GetValue(SelectionForegroundProperty);
        }
        public static void SetSelectionForeground (DependencyObject obj , Brush value)
        {
            obj . SetValue(SelectionForegroundProperty , value);
        }
        // Using a DependencyProperty as the backing store for SelectionForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectionForegroundProperty =
                DependencyProperty . RegisterAttached("SelectionForeground" , typeof(Brush) , typeof(ListboxColorCtrlAP) , new PropertyMetadata(Brushes . White));

        #endregion SelectionForeground   

        #region MouseoverForeground
        public static Brush GetMouseoverForeground (DependencyObject obj)
        {
            return ( Brush )obj . GetValue(MouseoverForegroundProperty);
        }

        public static void SetMouseoverForeground (DependencyObject obj , Brush value)
        {
            obj . SetValue(MouseoverForegroundProperty , value);
        }

        // Using a DependencyProperty as the backing store for MouseoverForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MouseoverForegroundProperty =
                DependencyProperty . RegisterAttached("MouseoverForeground" , typeof(Brush) , typeof(ListboxColorCtrlAP) , new PropertyMetadata(Brushes . Black));
        #endregion MouseoverForeground

        #region MouseoverBackground
        public static Brush GetMouseoverBackground (DependencyObject obj)
        {
            return ( Brush )obj . GetValue(MouseoverBackgroundProperty);
        }

        public static void SetMouseoverBackground (DependencyObject obj , Brush value)
        {
            obj . SetValue(MouseoverBackgroundProperty , value);
        }

        // Using a DependencyProperty as the backing store for MouseoverBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MouseoverBackgroundProperty =
                DependencyProperty . RegisterAttached("MouseoverBackground" , typeof(Brush) , typeof(ListboxColorCtrlAP) , new PropertyMetadata(Brushes . LightGray));
        #endregion MouseoverBackground 

        #region MouseoverSelectedForeground 
        public static Brush GetMouseoverSelectedForeground (DependencyObject obj)
        {
            return ( Brush )obj . GetValue(MouseoverSelectedForegroundProperty);
        }
        public static void SetMouseoverSelectedForeground (DependencyObject obj , Brush value)
        {
            obj . SetValue(MouseoverSelectedForegroundProperty , value);
        }
        public static readonly DependencyProperty MouseoverSelectedForegroundProperty =
                DependencyProperty . RegisterAttached("MouseoverSelectedForeground" , typeof(Brush) , typeof(ListboxColorCtrlAP) , new PropertyMetadata(Brushes . White));
        #endregion MouseoverSelectedForeground 

        #region MouseoverSelectedBackground 
        public static Brush GetMouseoverSelectedBackground (DependencyObject obj)
        {
            return ( Brush )obj . GetValue(MouseoverSelectedBackgroundProperty);
        }
        public static void SetMouseoverSelectedBackground (DependencyObject obj , Brush value)
        {
            obj . SetValue(MouseoverSelectedBackgroundProperty , value);
        }
        public static readonly DependencyProperty MouseoverSelectedBackgroundProperty =
                DependencyProperty . RegisterAttached("MouseoverSelectedBackground" , typeof(Brush) , typeof(ListboxColorCtrlAP) , new PropertyMetadata(Brushes . Red));
        #endregion MouseoverSelectedBackground




        #region dumyAPstring
        public static readonly DependencyProperty dummyAPstringProperty =
                    DependencyProperty . RegisterAttached("dummyAPstring" ,
                            typeof(string) , typeof(ListboxColorCtrlAP) ,
                            new PropertyMetadata(( string )"DummyAPstring from AP ! ") , OnstringSet);
        public static string GetdummyAPstring (DependencyObject obj)
        {
            return ( string )obj . GetValue(dummyAPstringProperty);
        }
        public static void SetdummyAPstring (DependencyObject obj , string value)
        {
            obj . SetValue(dummyAPstringProperty , value);
        }
        private static bool OnstringSet (object value)
        {
            // Debug. WriteLine ( $"AP.dummyAPstring set to : {value}" );
            return true;
        }
        #endregion dumyAPstring

        #region test2 AP
        public static readonly DependencyProperty test2Property =
                DependencyProperty . RegisterAttached("test2" ,
                    typeof(string) , typeof(ListboxColorCtrlAP) , new PropertyMetadata(( string )"99"));
        public static string Gettest2 (DependencyObject obj)
        {
            return ( string )obj . GetValue(test2Property);
        }
        public static void Settest2 (DependencyObject obj , string value)
        {
            obj . SetValue(test2Property , value);
        }
        #endregion test2 AP

        #region test AP
        public static readonly DependencyProperty testProperty =
                DependencyProperty . RegisterAttached("test" ,
                    typeof(int) , typeof(ListboxColorCtrlAP) , new PropertyMetadata(32767) , Ontestchanged);

        public static int Gettest (DependencyObject obj)
        {
            return ( int )obj . GetValue(testProperty);
        }
        public static void Settest (DependencyObject obj , int value)
        {
            obj . SetValue(testProperty , value);
        }


        private static bool Ontestchanged (object value)
        {
            //Debug. WriteLine ( $"AP : test value changed to {value}" );
            return true;
        }
        #endregion test AP

        #region dblvalue
        public static double Getdblvalue (DependencyObject obj)
        {
            return ( double )obj . GetValue(dblvalueProperty);
        }

        public static void Setdblvalue (DependencyObject obj , double value)
        {
            obj . SetValue(dblvalueProperty , value);
        }

        // Using a DependencyProperty as the backing store for dblvalue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty dblvalueProperty =
                DependencyProperty . RegisterAttached("dblvalue" , typeof(double) , typeof(ListboxColorCtrlAP) , new PropertyMetadata(( double )23.864) , Ondblvaluechanged);


        private static bool Ondblvaluechanged (object value)
        {
            //Debug. WriteLine ( $"dblvaluechanged = {value}" );
            return true;
        }
        #endregion dblvalue

        #endregion Attached Properties

    }
}