using System;
using System . Collections . Generic;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows . Media;
using System . Windows;
using NewWpfDev . Views;

namespace NewWpfDev . AttachedProperties
{
    public class DataGridColumnsColorAP : DependencyObject
    {

        #region UseAttProperties
        public static readonly DependencyProperty UseAttPropertiesProperty
              = DependencyProperty . RegisterAttached (
              "UseAttProperties" ,
              typeof ( bool ) ,
              typeof ( DataGridColumnsColorAP ) ,
              new PropertyMetadata ( false ) , OnUseAttPropertiesChanged );
        public static bool GetUseAttProperties ( DependencyObject d )
        {
            return ( bool ) d . GetValue ( UseAttPropertiesProperty );
        }
        public static void SetUseAttProperties ( DependencyObject d , bool value )
        {
            //Debug. WriteLine ( $"AP : setting Background to {value}" );
            d . SetValue ( UseAttPropertiesProperty , value );
        }
        private static bool OnUseAttPropertiesChanged ( object value )
        {
            //Debug. WriteLine ( $"AP : OnBackgroundchanged = {value}" );
            return true;
        }
        #endregion UseAttProperties

        #region HeaderBackground 
        public static Brush GetHeaderBackground ( DependencyObject obj )
        {
            if ( GetUseAttProperties ( obj ) )
                return ( Brush ) obj . GetValue ( HeaderBackgroundProperty );
            return null;
        }

        public static void SetHeaderBackground ( DependencyObject obj , Brush value )
        {
            if ( GetUseAttProperties ( obj ) )
                obj . SetValue ( HeaderBackgroundProperty , value );
        }

        // Using a DependencyProperty as the backing store for HeaderBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderBackgroundProperty =
                DependencyProperty . RegisterAttached ( "HeaderBackground" , typeof ( Brush ) , typeof ( DataGridColumnsColorAP ) , new PropertyMetadata ( Brushes . Transparent
                    ) );
        #endregion HeaderBackground 

        #region HeaderForeground 
        public static Brush GetHeaderForeground ( DependencyObject obj )
        {
            if ( GetUseAttProperties ( obj ) )
                return ( Brush ) obj . GetValue ( HeaderForegroundProperty );
            return null;
        }

        public static void SetHeaderForeground ( DependencyObject obj , Brush value )
        {
            if ( GetUseAttProperties ( obj ) )
                obj . SetValue ( HeaderForegroundProperty , value );
        }

        // Using a DependencyProperty as the backing store for HeaderForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderForegroundProperty =
                DependencyProperty . RegisterAttached ( "HeaderForeground" , typeof ( Brush ) , typeof ( DataGridColumnsColorAP ) , new PropertyMetadata ( Brushes . Black ) );
        #endregion HeaderForeground 

        public static string ActiveDragControl ( DependencyObject obj )
        { return ( string ) obj . GetValue ( ActiveDragControlProperty ); }

        public static void SetMyProperty ( DependencyObject obj , string value )
        { obj . SetValue ( ActiveDragControlProperty , value ); }

        public static readonly DependencyProperty ActiveDragControlProperty =
            DependencyProperty . RegisterAttached ( "ActiveDragControl" , typeof ( string ) , typeof ( DataGridColumnsColorAP ) , new PropertyMetadata ( "" ) );


    }
}
