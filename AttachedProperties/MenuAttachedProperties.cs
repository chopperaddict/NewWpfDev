using System;
using System . Collections . Generic;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows . Media;
using System . Windows;
using Newtonsoft . Json . Linq;

namespace NewWpfDev. AttachedProperties
{
	public class MenuAttachedProperties : DependencyObject
	{
		#region NormalBackground 
		public static Brush NormalBackground ( DependencyObject obj )
		{
			return ( Brush ) obj . GetValue ( NormalBackgroundProperty );
		}
		public static void SetNormalBackground ( DependencyObject obj , Brush value )
		{
			obj . SetValue ( NormalBackgroundProperty , value );
		}
		public static readonly DependencyProperty NormalBackgroundProperty =
			  DependencyProperty . RegisterAttached ( "NormalBackground", typeof ( Brush ), typeof ( MenuAttachedProperties ), new PropertyMetadata ( Brushes.LightGray) );
		#endregion HeaderBackground 

		#region NormallForeground 
		public static Brush NormalForeground ( DependencyObject obj )
		{
			return ( Brush ) obj . GetValue ( NormalForegroundProperty );
		}
		public static void SetNormalForeground ( DependencyObject obj , Brush value )
		{
			obj . SetValue ( NormalForegroundProperty , value );
		}
		public static readonly DependencyProperty NormalForegroundProperty =
			    DependencyProperty.RegisterAttached("NormalForeground", typeof(Brush), typeof(MenuAttachedProperties), new PropertyMetadata(Brushes.Black));
		#endregion NormallForeground 

		#region MouseoverBackground 
		public static Brush GetMouseoverBackground ( DependencyObject obj )
		{
			return ( Brush ) obj . GetValue ( MouseoverBackgroundProperty );
		}

		public static void SetMouseoverBackground ( DependencyObject obj , Brush value )
		{
			obj . SetValue ( MouseoverBackgroundProperty , value );
		}
		public static readonly DependencyProperty MouseoverBackgroundProperty =
			    DependencyProperty.RegisterAttached("MouseoverBackground", typeof(Brush), typeof(MenuAttachedProperties), new PropertyMetadata(Brushes.DarkGray));
		#endregion MouseoverBackground 

		#region MouseoverForeground 
		public static Brush MousoverForeground ( DependencyObject obj )
		{
			return ( Brush ) obj . GetValue ( MousoverForegroundProperty );
		}
		public static void SetMousoverForeground ( DependencyObject obj , Brush value )
		{
			obj . SetValue ( MousoverForegroundProperty , value );
		}
		public static readonly DependencyProperty MousoverForegroundProperty =
			DependencyProperty.RegisterAttached("MousoverForeground", typeof(Brush), typeof(MenuAttachedProperties), new PropertyMetadata(Brushes.White));
		#endregion MouseoverForeground 



	}
}
