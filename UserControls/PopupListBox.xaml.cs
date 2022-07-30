using System;
using System . Collections;
using System . Collections . Generic;
using System . ComponentModel;
using System . Diagnostics;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Controls . Primitives;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Media;

using NewWpfDev . Views;

namespace NewWpfDev . UserControls
{
    public partial class PopupListBox : UserControl
    {
        //======================================================================================//
        // A different style has been selected
        public event EventHandler<StyleArgs> Stylechanged;
        //ListBox is being expanded/contracted
        public static event EventHandler<SizeChangedArgs> StyleSizeChanged;

        //[DefaultValue ( typeof ( Size ) , "300,30" )]
        //public Size Size { get; set; }

        //[DefaultValue ( typeof ( double ) , "130" )]
        //new public double Width { get; set; }

        //[DefaultValue ( typeof ( double ) , "30" )]
        //new public double Height{ get; set; }

        protected virtual void OnStylechanged ( StyleArgs e )
        {       // A different style has been selected
            if ( Stylechanged != null )
            {
                Stylechanged ( this , e );
                SetShadowTextbox ( e . style );
            }
        }
        public void TriggerStyleChanged ( object sender , StyleArgs e )
        { OnStylechanged ( e ); }
        public void SetShadowTextbox ( string style )
        {
            ShadowText . Text = style;
            ShadowText . UpdateLayout ( );
        }
        //======================================================================================//
        protected virtual void OnStyleSizeChanged ( SizeChangedArgs e )
        {           //ListBox is being expanded/contracted

            if ( StyleSizeChanged != null )
                StyleSizeChanged ( this , e );
        }
        //======================================================================================//

        public Control HostWin { get; set; }
        public bool NoUpdate { get; set; } = false;
        public PopupListBox ( )
        {
            InitializeComponent ( );
            // DataContext set to ourselves
            DataContext = this;
            // listen for style changes
            GenericGridControl . StylesSwitch += GenericGridControl_StylesSwitch;
        }
        public void SetHost ( object host )
        {
            Type type = host . GetType ( );
            HostWin = ( Control ) host;
        }
        private void GenericGridControl_StylesSwitch ( object sender , StyleArgs e )
        {     // Handle style changes
            ShadowText . Text = e . style;
            ShadowText . UpdateLayout ( );
            if ( StylesCombo . SelectedItem == null ) StylesCombo . SelectedIndex = 0;
            for ( int x = 0 ; x < Stylescount ; x++ )
            {
                //if ( StylesCombo . SelectedItem . ToString ( ) == e . style )
                //{
                //    StylesCombo . SelectedIndex = x;
                //    break;
                //}
            }
        }

        private void StylesCombo_MouseEnter ( object sender , MouseEventArgs e )
        {
            //return;
            ListBox lb = sender as ListBox;
            SizeChangedArgs args = new SizeChangedArgs ( );
            // Hide blocking TextBlock
            this . ShadowText . Visibility = Visibility . Hidden;
            this . ShadowText . UpdateLayout ( );
            this . ShadowText . Refresh ( );
            // 0=PopUP, 1=PopDN
            args . GrowDirection = GrowDirection;
            //Ensure ListBox is Visible, and at its normal (specified) height
            StylesCombo . Visibility = Visibility . Visible;
            StylesCombo . Height = MaxListHeight;
            StylesCombo . UpdateLayout ( );
            StylesCombo . Refresh ( );
            // force the parent (Canvas ) to be full height
            Thickness th2 = new Thickness ( );
            th2 = Stylesgrid . Margin;
            Stylesgrid . UpdateLayout ( );
            this . StylesCombo . Visibility = Visibility . Visible;
            if ( GrowDirection == 0 )
            {
                // slide listbox UP - Working !!
                // Move top of canvas UP first
                CanvasTop = -( MaxListHeight );
                 Stylesgrid . Margin = th2;
                Stylesgrid . Height = MaxListHeight;
                Stylesgrid . UpdateLayout ( );
                Stylesgrid . Refresh ( );
                args . NewHeight = MaxListHeight;
                OnStyleSizeChanged ( args );
            }
            else
            {
                // slide listbox DOWN - Working !!!
                CanvasTop = 0;
                th2 . Bottom =  ((double)MaxListHeight /0.8);
                Stylesgrid . Margin = th2;
                //Set Canvas Container to full Height
                Stylesgrid . Height = MaxListHeight;
                StylesCombo.Height= MaxListHeight;
                Stylesgrid . UpdateLayout ( );
                Stylesgrid . Refresh ( );
                args . NewHeight = MaxListHeight;
                OnStyleSizeChanged ( args );
                Debug . WriteLine ("");
            }
            ShadowText . Visibility = Visibility . Hidden;
            e . Handled = true;
        }

        private void StylesCombo_MouseLeave ( object sender , MouseEventArgs e )
        {
           //return;
            StylesCombo . Height = DefaultHeight;
            Thickness th2 = new Thickness ( );
            th2 = Stylesgrid . Margin;
            th2 . Top = 0;
            Stylesgrid . Margin = th2;
            this . Stylesgrid . Height = DefaultHeight;
            Stylesgrid . Refresh ( );
            Stylesgrid . Refresh ( );
            StylesCombo . Refresh ( );
            SizeChangedArgs args = new SizeChangedArgs ( );
            args . NewHeight = DefaultHeight;
            OnStyleSizeChanged ( args );
            ShadowText . Visibility = Visibility . Visible;
            Stylesgrid . UpdateLayout ( );
            StylesCombo . UpdateLayout ( );
            e . Handled = true;
            return;
        }

        public void Clear ( object sender , SelectionChangedEventArgs e )
        {
            try
            {
                StylesCombo . Items . Clear ( );
            }
            catch ( Exception ex ) { }
        }

        private void StylesCombo_SelectionChanged ( object sender , SelectionChangedEventArgs e )
        {
            if ( NoUpdate )
                return;
            StyleArgs args = new StyleArgs ( );
            Control ctrl = sender as Control;
            string str = e . AddedItems [ 0 ] . ToString ( );
            args . style = str;
            args . sender = this;
            TriggerStyleChanged ( this , args );
        }
        public void AddItems ( List<string> all )
        {
            StylesCombo . ItemsSource = all;
            Stylescount = all . Count;
            SetShadowTextbox ( all [ 0 ] );
        }
        public void ClearSource ( )
        {
            StylesCombo . ItemsSource = null;
        }

        #region Dependency properties
        //======================================================================================//
          public string Text
        {
            get { return ( string ) GetValue ( TextProperty ); }
            set { SetValue ( TextProperty , value ); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty . Register ( "Text" , typeof ( string ) , typeof ( PopupListBox ) , new PropertyMetadata ( ( string ) "" ) );
        //======================================================================================//
        new public Brush Background
        {
            get { return ( Brush ) GetValue ( BackgroundProperty ); }
            set { SetValue ( BackgroundProperty , value ); }
        }
        new public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty . Register ( "Background" , typeof ( Brush ) , typeof ( PopupListBox ) , new PropertyMetadata ( ( Brush ) Brushes . White ) );
        //======================================================================================//
        new public Brush Foreground
        {
            get { return ( Brush ) GetValue ( ForegroundProperty ); }
            set { SetValue ( ForegroundProperty , value ); }
        }
        new public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty . Register ( "Foreground" , typeof ( Brush ) , typeof ( PopupListBox ) , new PropertyMetadata ( Brushes . Black ) );
        //======================================================================================//
        public double MaxListHeight
        {
            get { return ( double ) GetValue ( MaxListHeightProperty ); }
            set { SetValue ( MaxListHeightProperty , value ); }
        }
        public static readonly DependencyProperty MaxListHeightProperty =
            DependencyProperty . Register ( "MaxListHeight" , typeof ( double ) , typeof ( PopupListBox ) , new PropertyMetadata ( ( double ) 0 ) );
        //======================================================================================//
        public int GrowDirection
        {            // 0 == UP ,1 == down
            get { return ( int ) GetValue ( GrowDirectionProperty ); }
            set { SetValue ( GrowDirectionProperty , value ); }
        }
        public static readonly DependencyProperty GrowDirectionProperty =
            DependencyProperty . Register ( "GrowDirection" , typeof ( int ) , typeof ( PopupListBox ) , new PropertyMetadata ( ( int ) 0 ) , SetVerticalAlignment );
        private static bool SetVerticalAlignment ( object value )
        {
            return true;
        }
        //======================================================================================//
        new public VerticalAlignment VerticalAlignment
        {
            get { return ( VerticalAlignment ) GetValue ( VerticalAlignmentProperty ); }
            set { SetValue ( VerticalAlignmentProperty , value ); }
        }
        new public static readonly DependencyProperty VerticalAlignmentProperty =
            DependencyProperty . Register ( "VerticalAlignment" , typeof ( VerticalAlignment ) , typeof ( Popup ) , new PropertyMetadata ( default ) );//, SetGrowDirection);
        private static bool SetGrowDirection ( object value )
        {            // Top grows UP
            PopupListBox sc = new PopupListBox ( );
            if ( ( int ) sc . GetValue ( GrowDirectionProperty ) == 0 )
                sc . VerticalAlignment = VerticalAlignment . Bottom;
            else
                sc . VerticalAlignment = VerticalAlignment . Top;
            return true;
        }
        //======================================================================================//
        new public Thickness Margin
        {
            get { return ( Thickness ) GetValue ( MarginProperty ); }
            set { SetValue ( MarginProperty , value ); }
        }
        new public static readonly DependencyProperty MarginProperty =
            DependencyProperty . Register ( "Margin" , typeof ( Thickness ) , typeof ( Popup ) , new PropertyMetadata ( ( default ) ) );
        //======================================================================================//
        public double DefaultHeight
        {
            get { return ( double ) GetValue ( DefaultHeightProperty ); }
            set { SetValue ( DefaultHeightProperty , value ); }
        }
        public static readonly DependencyProperty DefaultHeightProperty =
            DependencyProperty . Register ( "DefaultHeight" , typeof ( double ) , typeof ( PopupListBox ) , new PropertyMetadata ( ( double ) 30 ) );
        //======================================================================================//
        public string SelectedItem
        {
            get { return ( string ) GetValue ( SelectedItemProperty ); }
            set { SetValue ( SelectedItemProperty , value ); }
        }
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty . Register ( "SelectedItem" , typeof ( string ) , typeof ( PopupListBox ) , new PropertyMetadata ( ( string ) "" ) );
        //======================================================================================//
        public int SelectedIndex
        {
            get { return ( int ) GetValue ( SelectedIndexProperty ); }
            set { SetValue ( SelectedIndexProperty , value ); }
        }
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty . Register ( "SelectedIndex" , typeof ( int ) , typeof ( PopupListBox ) , new PropertyMetadata ( ( int ) 0 ) );
        //======================================================================================//
        public IEnumerable ItemsSource
        {
            get { return ( IEnumerable ) GetValue ( ItemsSourceProperty ); }
            set { SetValue ( ItemsSourceProperty , value ); }
        }
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty . Register ( "ItemsSource" , typeof ( IEnumerable ) , typeof ( PopupListBox ) , new PropertyMetadata ( default ) );
        //======================================================================================//
        public IEnumerable Items
        {
            get { return ( IEnumerable ) GetValue ( MyPropertyProperty ); }
            set { SetValue ( MyPropertyProperty , value ); }
        }
        public static readonly DependencyProperty MyPropertyProperty =
            DependencyProperty . Register ( "Items" , typeof ( IEnumerable ) , typeof ( PopupListBox ) , new PropertyMetadata ( default ) );
        //======================================================================================//
        public bool HasFocus
        {
            get { return ( bool ) GetValue ( HasFocusProperty ); }
            set { SetValue ( HasFocusProperty , value ); }
        }
        public static readonly DependencyProperty HasFocusProperty =
            DependencyProperty . Register ( "HasFocus" , typeof ( bool ) , typeof ( PopupListBox ) , new PropertyMetadata ( ( bool ) false ) );
        //======================================================================================//
        public int Stylescount
        {
            get { return ( int ) GetValue ( StylescountProperty ); }
            set { SetValue ( StylescountProperty , value ); }
        }
        public static readonly DependencyProperty StylescountProperty =
            DependencyProperty . Register ( "Stylescount" , typeof ( int ) , typeof ( PopupListBox ) , new PropertyMetadata ( default ) );
        //======================================================================================//
        public double CanvasTop
        {
            get { return ( double ) GetValue ( CanvasTopProperty ); }
            set { SetValue ( CanvasTopProperty , value ); }
        }
        public static readonly DependencyProperty CanvasTopProperty =
            DependencyProperty . Register ( "CanvasTop" , typeof ( double ) , typeof ( PopupListBox ) , new PropertyMetadata ( ( double ) default ) );
        //======================================================================================//
        public double CanvasHeight
        {
            get { return ( double ) GetValue ( CanvasHeightProperty ); }
            set { SetValue ( CanvasHeightProperty , value ); }
        }
        public static readonly DependencyProperty CanvasHeightProperty =
            DependencyProperty . Register ( "CanvasHeight" , typeof ( double ) , typeof ( PopupListBox ) , new PropertyMetadata ( ( double ) 30 ) );
        //======================================================================================//
        public double CanvasWidth
        {
            get { return ( double ) GetValue ( CanvasWidthProperty ); }
            set { SetValue ( CanvasWidthProperty , value ); }
        }
        public static readonly DependencyProperty CanvasWidthProperty =
            DependencyProperty . Register ( "CanvasWidth" , typeof ( double ) , typeof ( PopupListBox ) , new PropertyMetadata ( ( double ) 150 ) );
        //======================================================================================//
        public double  CtrlHeight
        {
            get { return ( double  ) GetValue ( CtrlHeightProperty ); }
            set { SetValue ( CtrlHeightProperty , value ); }
        }
        public static readonly DependencyProperty CtrlHeightProperty =
            DependencyProperty . Register ( "CtrlHeight" , typeof ( double  ) , typeof ( PopupListBox ) , new PropertyMetadata ( (double)30 ) );
        //======================================================================================//
        public double CtrlWidth
        {
            get { return ( double ) GetValue ( CtrlWidthProperty ); }
            set { SetValue ( CtrlWidthProperty , value ); }
        }
        public static readonly DependencyProperty CtrlWidthProperty =
            DependencyProperty . Register ( "CtrlWidth" , typeof ( double ) , typeof ( PopupListBox ) , new PropertyMetadata ( (double)120 ) );
        //======================================================================================//


        #endregion Dependency properties

    }
}
