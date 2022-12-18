using System;
using System . Diagnostics . Eventing . Reader;
using System . Linq;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;
using System . Windows . Media;

using Microsoft . Xaml . Behaviors;

using Views;

namespace NewWpfDev
{

    public class TextChangeBehavior : Behavior<TextBox>
    {

        public static Brush backgrnd = null;
        public static Brush foregrnd = null;

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

        public static TextChangedEventHandler OnTextChanged ( DependencyObject d , DependencyPropertyChangedEventArgs e )
        {
            return CtrlTextChanged;
        }
        private  static void CtrlTextChanged ( object sender , RoutedEventArgs e )
        {
            // handles various different TextBoxes, based on their name.
            // Set Background to Off whiite when empty, else light green
            // and toggle the caret color as well from Black to Red
            TextBox? tb = sender as TextBox;
            if ( tb == null || tb . GetType ( ) != typeof ( TextBox ) )
                return;
            
            if ( tb . Name == "ExName" )
            {
                if ( backgrnd != null )
                    tb . Background = backgrnd as SolidColorBrush;
                if ( foregrnd != null )
                    tb . Foreground = foregrnd as SolidColorBrush;
                tb . UpdateLayout ( );
            }
            if ( tb . Name == "SPName" )
            {
                if ( backgrnd != null )
                    tb . Background = backgrnd as SolidColorBrush;
                if ( foregrnd != null )
                    tb . Foreground = foregrnd as SolidColorBrush;
                tb . UpdateLayout ( );
            }
            if ( tb . Name == "Parameterstop" )
            {
               if ( tb . Text == "[No Parameters/Arguments required]"
                    || tb . Text == "[No parameters required (or allowed)]" )
                {
                    tb . Background = Application . Current . FindResource ( "Green5" ) as SolidColorBrush;
                    tb . Foreground = Application . Current . FindResource ( "Red3" ) as SolidColorBrush;
                }
                else if ( tb . Text . Contains ( " or multiple inputs]" ) )
                {
                    tb . Background = Application . Current . FindResource ( "Purple2" ) as SolidColorBrush;
                    tb . Foreground = Application . Current . FindResource ( "White0" ) as SolidColorBrush;
                }
                else if ( tb . Text . Contains ( " Output " ) )
                {
                    tb . Background = Application . Current . FindResource ( "Red2" ) as SolidColorBrush;
                    tb . Foreground = Application . Current . FindResource ( "White0" ) as SolidColorBrush;
                }
                else
                {
                    tb . Background = Application . Current . FindResource ( "Cyan2" ) as SolidColorBrush;
                    tb . Foreground = Application . Current . FindResource ( "Black0" ) as SolidColorBrush;
                }
                backgrnd = tb . Background;
                foregrnd = tb . Foreground;
                SpResultsViewer spv = SpResultsViewer . GetViewerPointer ( );
                TextBox tbx = new TextBox( );
                tbx = spv . ExName;
                spv . UpdateExecuteBackground (tbx,  backgrnd , foregrnd );
            }
            else if ( tb . Name . Contains ( "Nobehavior" ) == false )
            {
                if ( tb . Text . Length != 0 )
                {
                    if ( tb . Background == Application . Current . FindResource ( "White5" ) as SolidColorBrush )
                    {
                        tb . Background = Application . Current . FindResource ( "Green5" ) as SolidColorBrush;
                        tb . CaretBrush = Application . Current . FindResource ( "Black0" ) as SolidColorBrush;
                        tb . Foreground = Application . Current . FindResource ( "Black0" ) as SolidColorBrush;
                    }
                }
                else
                {
                    if ( tb . Background == Application . Current . FindResource ( "Green5" ) as SolidColorBrush ) ;
                    {
                        tb . Background = Application . Current . FindResource ( "White5" ) as SolidColorBrush;
                        tb . CaretBrush = Application . Current . FindResource ( "Red0" ) as SolidColorBrush;
                    }
                }
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
