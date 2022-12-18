using System;
using System . Collections . Generic;
using System . Reflection;
using System . Text;
using System . Windows . Controls . Primitives;

using Microsoft . Xaml . Behaviors;

namespace NewWpfDev
{
    public   class ScrollViewerBehavior : Behavior<System . Windows . Controls . ScrollViewer>
    {
        protected override void OnAttached ( )
        {
            base . OnAttached ( );
            AssociatedObject . Loaded += ScrollViewerLoaded;
        }

        private void ScrollViewerLoaded ( object sender , System . Windows . RoutedEventArgs e )
        {
            var property = AssociatedObject . GetType ( ) . GetProperty ( "ScrollInfo" , BindingFlags . NonPublic | BindingFlags . Instance );
            property . SetValue ( AssociatedObject , new ScrollInfoAdapter ( ( IScrollInfo ) property . GetValue ( AssociatedObject ) ) );
        }
    }
}
