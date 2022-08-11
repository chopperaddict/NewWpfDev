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
        public static  event EventHandler<StyleArgs> Stylechanged;
        //ListBox is being expanded/contracted
        public static event EventHandler<SizeChangedArgs> StyleSizeChanged;
        public static PopupListBox ThisWin { get; set; }
        public BankAcHost HostWin { get; set; }
        public GenericGridControl ggc { get; set; }
        public bool NoUpdate { get; set; } = false;
        public double Ctrlwidth { get; set; } = 0;
        public static bool ResetPopup { get; set; }
        private double maxListHeight;
        public double MaxListHeight
        {
            get { return maxListHeight; }
            set { maxListHeight = value; }
        }
        private double defaultHeight;
        public double DefaultHeight
        {
            get { return defaultHeight; }
            set { defaultHeight = value; }
        }
        private int growDirection;
        public int GrowDirection
        {
            get { return growDirection; ; }
            set { growDirection = value; }
        }


        public PopupListBox ( )
        {
            InitializeComponent ( );
            // DataContext set to ourselves
            DataContext = this;
            ThisWin = this;
            // listen for style changes
            BankAcHost . StylesSwitch += GenericGridControl_StylesSwitch;
            ggc = GenericGridControl . GetGenGridHandle ( );
            //StartMargin = this . Margin;
        }
        public static PopupListBox GetPopupLbHandle ( )
        { return ThisWin; }
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


        public void SetHost ( object host )
        {
            Type type = host . GetType ( );
            HostWin = host as BankAcHost;
            //if ( type is GenericGridControl )
            //    ggc = GenericGridControl . GetGenGridHandle ( );
        }
        private void GenericGridControl_StylesSwitch ( object sender , StyleArgs e )
        {     // Handle style changes
            ShadowText . Text = e . style;
            ShadowText . UpdateLayout ( );
            if ( StylesCombo . SelectedItem == null ) StylesCombo . SelectedIndex = 0;
            for ( int x = 0 ; x < Stylescount ; x++ )
            {
                if ( StylesCombo . SelectedItem . ToString ( ) == e . style )
                {
                    StylesCombo . SelectedIndex = x;
                    break;
                }
            }
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
            args . ActiveGrid = GenericGridControl . ActiveGrid;
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
        private void StylesCombo_MouseEnter ( object sender , MouseEventArgs e )
        {
            if ( IsOpen == true )
                return;
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
            this . Height = MaxListHeight;
            StylesCombo . UpdateLayout ( );
            StylesCombo . Refresh ( );
            Panel . SetZIndex ( this , 5 );
            // force the parent (Canvas ) to be full height
            if ( GrowDirection == 0 )
            {
                // slide listbox UP -  !!
                // WORKING
                Thickness th2 = this . Margin;
                th2 . Top = -( MaxListHeight - th2 . Top - DefaultHeight );
                this . Margin = th2;
                this . UpdateLayout ( );
                Stylesgrid . Height = MaxListHeight;
                StylesCombo . Height = MaxListHeight;
                Stylesgrid . UpdateLayout ( );
                Stylesgrid . Refresh ( );
            }
            else
            {
                //Slide Listbox DOWN
                //WORKING correctly
                Stylesgrid . Height = MaxListHeight;
                StylesCombo . Height = MaxListHeight;
                Stylesgrid . UpdateLayout ( );
                Stylesgrid . Refresh ( );
            }
            ShadowText . Visibility = Visibility . Hidden;
            StylesCombo . Visibility = Visibility . Visible;
            Stylesgrid . Visibility = Visibility . Visible;
            Panel . SetZIndex ( this , 5 );
            IsOpen = true;
        }

        private void StylesCombo_MouseLeave ( object sender , MouseEventArgs e )
        {
            // Working OK - resets popup position after sliding UP or DOWN
            if ( IsOpen == false )
                return;
            IsOpen = false;
            // ButtonPanel
            Grid grid = this . Parent as Grid;
            HostWin . Refresh ( );
           if ( GrowDirection == 0 )
            {
                Thickness th2 = this . Margin;
                th2 . Top = 10;
                th2 . Bottom = 0;
                this . Margin = th2;
            }
            //ShadowText . Text = "RERERlhhhjRRRW";
            this . Height = DefaultHeight;
            Stylesgrid . Height = DefaultHeight;
            StylesCombo . Height = DefaultHeight;
            ShadowText . Visibility = Visibility . Visible;
            ShadowText . UpdateLayout ( );
            Stylesgrid . Refresh ( );
            StylesCombo . Refresh ( );
            SizeChangedArgs args = new SizeChangedArgs ( );
            ShadowText . UpdateLayout ( );
            Panel . SetZIndex ( this , 0 );
            ShadowText . UpdateLayout ( );
            IsOpen = false;
            return;
        }

        #region Dependency properties

        #region Popup settings
        //======================================================================================//
        public double PopupOpenHeight
        {
            get { return ( double ) GetValue ( PopupOpenHeightProperty ); }
            set { SetValue ( PopupOpenHeightProperty , value ); }
        }
        public static readonly DependencyProperty PopupOpenHeightProperty =
            DependencyProperty . Register ( "PopupOpenHeight" , typeof ( double ) , typeof ( PopupListBox ) , new PropertyMetadata ( ( double ) default ) );
        //======================================================================================//
        public bool IsOpen
        {
            get { return ( bool ) GetValue ( IsOpenProperty ); }
            set { SetValue ( IsOpenProperty , value ); }
        }
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty . Register ( "IsOpen" , typeof ( bool ) , typeof ( PopupListBox ) , new PropertyMetadata ( ( bool ) false ) );
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
        new public VerticalAlignment VerticalAlignment
        {
            get { return ( VerticalAlignment ) GetValue ( VerticalAlignmentProperty ); }
            set { SetValue ( VerticalAlignmentProperty , value ); }
        }
        new public static readonly DependencyProperty VerticalAlignmentProperty =
            DependencyProperty . Register ( "VerticalAlignment" , typeof ( VerticalAlignment ) , typeof ( Popup ) , new PropertyMetadata ( default ) );//, SetGrowDirection);
        private static bool SetGrowDirection ( object value )
        {            // Top grows UP
            return true;
        }
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
        public double RowHeight
        {
            get { return ( double ) GetValue ( RowHeightProperty ); }
            set { SetValue ( RowHeightProperty , value ); }
        }
        public static readonly DependencyProperty RowHeightProperty =
            DependencyProperty . Register ( "RowHeight" , typeof ( double ) , typeof ( PopupListBox ) , new PropertyMetadata ( (double)25 ) );
        //======================================================================================//


        #endregion Popup settings



        #endregion Dependency properties

    }
}
