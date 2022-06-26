using System;
using System . Collections . Generic;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows . Media;
using System . Windows;

namespace NewWpfDev. AttachedProperties
{
      public class DataGridColumnsColorAP : DependencyObject
      {


            #region HeaderBackground 
            public static Brush GetHeaderBackground ( DependencyObject obj )
            {
                  return ( Brush ) obj . GetValue ( HeaderBackgroundProperty );
            }

            public static void SetHeaderBackground ( DependencyObject obj , Brush value )
            {
                  obj . SetValue ( HeaderBackgroundProperty , value );
            }

            // Using a DependencyProperty as the backing store for HeaderBackground.  This enables animation, styling, binding, etc...
            public static readonly DependencyProperty HeaderBackgroundProperty =
                    DependencyProperty . RegisterAttached ( "HeaderBackground", typeof ( Brush ), typeof ( DataGridColumnsColorAP ), new PropertyMetadata ( Brushes.LightGray) );
            #endregion HeaderBackground 

            #region HeaderForeground 
            public static Brush GetHeaderForeground ( DependencyObject obj )
            {
                  return ( Brush ) obj . GetValue ( HeaderForegroundProperty );
            }

            public static void SetHeaderForeground ( DependencyObject obj , Brush value )
            {
                  obj . SetValue ( HeaderForegroundProperty , value );
            }

            // Using a DependencyProperty as the backing store for HeaderForeground.  This enables animation, styling, binding, etc...
            public static readonly DependencyProperty HeaderForegroundProperty =
                    DependencyProperty . RegisterAttached ( "HeaderForeground", typeof ( Brush ), typeof ( DataGridColumnsColorAP ), new PropertyMetadata ( Brushes.Black ) );
            #endregion HeaderForeground 

      }
}
