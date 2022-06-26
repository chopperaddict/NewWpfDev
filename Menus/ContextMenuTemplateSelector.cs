using System;
using System . Collections . Generic;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows . Controls;
using System . Windows;

namespace NewWpfDev  . Menus
{
    public class ContextMenuItemContainerTemplateSelector : ItemContainerTemplateSelector
    {
        private static ResourceDictionary _dictionary;
        static ContextMenuItemContainerTemplateSelector ( )
        {
            _dictionary = new ResourceDictionary ( );
            _dictionary . Source = new Uri ( @"pack://application:,,,/ContextMenu.Framework;component/ContextMenuResources.xaml" );
        }
        public override DataTemplate SelectTemplate ( object item , ItemsControl parentItemsControl )
        {
            var name = item == null ? null : item . GetType ( ) . Name;
            if ( name != null && _dictionary . Contains ( name ) )
            {
                return ( DataTemplate ) _dictionary [ name ];
            }
            return null;
        }
    }
}

