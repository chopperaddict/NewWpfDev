using System;
using System . Linq;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;
using System . Windows . Media;

using Microsoft . Xaml . Behaviors;

namespace NewWpfDev
{

    public class TextChangeBehavior : Behavior<TextBox>
    {
        protected override void OnAttached ( )
        {
            base . OnAttached ( );
            TextBox txtBox = AssociatedObject as TextBox;
            txtBox . TextChanged += txt_TextChanged;
            if ( string . IsNullOrEmpty ( txtBox . Text ) )
            {
                txtBox . Background = new SolidColorBrush ( Colors . Green );
            }
            else
            {
                txtBox . Background = new SolidColorBrush ( Colors . Red );
            }

        }
        void txt_TextChanged ( object sender , TextChangedEventArgs e )
        {
            TextBox txtBox = AssociatedObject as TextBox;
            txtBox . TextChanged += txt_TextChanged;
            if ( string . IsNullOrEmpty ( txtBox . Text ) )
            {
                txtBox . Background = new SolidColorBrush ( Colors . Green );
            }
            else
            {
                txtBox . Background = new SolidColorBrush ( Colors . Red );
            }
        }
        protected override void OnDetaching ( )
        {
            base . OnDetaching ( );
            TextBox txtBox = AssociatedObject as TextBox;
            txtBox . TextChanged -= txt_TextChanged;

        }
    }

    //public abstract class DigitsOnlyBehavior :  Behavior 
    //{
    //    //public DigitsOnlyBehavior ( DependencyObject obj )
    //    //{
    //    //    IsDigitOnly = obj;
    //    //}
    //    public DigitsOnlyBehavior ( )
    //    {
    //        //IsDigitOnly = obj;
    //    }
    //public static bool GetIsDigitOnly ( DependencyObject obj )
    //    {
    //        return ( bool ) obj . GetValue ( IsDigitOnlyProperty );
    //    }
    //    protected override void OnAttached ( )
    //    {
    //        base . OnAttached ( );
    //        TextBox txtBox = AssociatedObject as TextBox;
    //        txtBox . TextChanged += txt_TextChanged;
    //        if ( string . IsNullOrEmpty ( txtBox . Text ) )
    //        {
    //            txtBox . Background = new SolidColorBrush ( Colors . Green );
    //        }
    //        else
    //        {
    //            txtBox . Background = new SolidColorBrush ( Colors . Red );
    //        }

    //    }
    //    protected override void OnDetaching ( )
    //    {
    //        base . OnDetaching ( );
    //        TextBox txtBox = AssociatedObject as TextBox;
    //        txtBox . TextChanged -= txt_TextChanged;

    //    }
    //    public static void SetIsDigitOnly ( DependencyObject obj , bool value )
    //    {
    //        obj . SetValue ( IsDigitOnlyProperty , value );
    //    }

    //    void txt_TextChanged ( object sender , TextChangedEventArgs e )
    //    {
    //        TextBox txtBox = AssociatedObject as TextBox;
    //        txtBox . TextChanged += txt_TextChanged;
    //        if ( string . IsNullOrEmpty ( txtBox . Text ) )
    //        {
    //            txtBox . Background = new SolidColorBrush ( Colors . Green );
    //        }
    //        else
    //        {
    //            txtBox . Background = new SolidColorBrush ( Colors . Red );
    //        }
    //    }

    //    public static readonly DependencyProperty IsDigitOnlyProperty =
    //      DependencyProperty . RegisterAttached ( "IsDigitOnly" ,
    //      typeof ( bool ) , typeof ( DigitsOnlyBehavior ) ,
    //      new PropertyMetadata ( false , OnIsDigitOnlyChanged ) );

    //    private static void OnIsDigitOnlyChanged ( object sender , DependencyPropertyChangedEventArgs e )
    //    {
    //        // ignoring error checking
    //        var textBox = ( TextBox ) sender;
    //        var isDigitOnly = ( bool ) e . NewValue;

    //        if ( isDigitOnly )
    //            textBox . PreviewTextInput += BlockNonDigitCharacters;
    //        else
    //            textBox . PreviewTextInput -= BlockNonDigitCharacters;
    //    }

    //    private static void BlockNonDigitCharacters ( object sender , TextCompositionEventArgs e )
    //    {
    //        e . Handled = e . Text . Any ( ch => !char . IsDigit ( ch ) );
    //    }

    //    public override string? ToString ( )
    //    {
    //        return base . ToString ( );
    //    }

    //    protected override bool ShouldSerializeProperty ( DependencyProperty dp )
    //    {
    //        return base . ShouldSerializeProperty ( dp );
    //    }

    //    protected override void CloneCore ( Freezable sourceFreezable )
    //    {
    //        base . CloneCore ( sourceFreezable );
    //    }

    //    protected override void CloneCurrentValueCore ( Freezable sourceFreezable )
    //    {
    //        base . CloneCurrentValueCore ( sourceFreezable );
    //    }

    //    protected override void GetAsFrozenCore ( Freezable sourceFreezable )
    //    {
    //        base . GetAsFrozenCore ( sourceFreezable );
    //    }

    //    protected override void GetCurrentValueAsFrozenCore ( Freezable sourceFreezable )
    //    {
    //        base . GetCurrentValueAsFrozenCore ( sourceFreezable );
    //    }

    //    protected override void OnChanged ( )
    //    {
    //        base . OnChanged ( );
    //    }

    //    protected override void OnPropertyChanged ( DependencyPropertyChangedEventArgs e )
    //    {
    //        base . OnPropertyChanged ( e );
    //    }

    //    protected override bool FreezeCore ( bool isChecking )
    //    {
    //        return base . FreezeCore ( isChecking );
    //    }

    //    protected override Freezable CreateInstanceCore ( )
    //    {
    //        return base . CreateInstanceCore ( );
    //    }
    //}
}
