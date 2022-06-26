using System;
using System . Collections . Generic;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;

namespace NewWpfDev. AttachedProperties
{
    public  class IsExpanded
    {
        public static bool GetIsExpand ( DependencyObject obj )
        {
            return ( bool ) obj . GetValue ( IsExpandProperty );
        }

        public static void SetIsExpand ( DependencyObject obj , bool value )
        {
            obj . SetValue ( IsExpandProperty , value );
        }

        // Using a DependencyProperty as the backing store for IsExpand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsExpandProperty =
            DependencyProperty . RegisterAttached ( "IsExpand" , typeof ( bool ) , 
                typeof ( IsExpanded ) , new PropertyMetadata ( false ) );

    }
}
