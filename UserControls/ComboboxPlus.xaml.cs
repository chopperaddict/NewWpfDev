using System;
using System . Collections;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Data;
using System . Windows . Documents;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Media . Imaging;
using System . Windows . Navigation;
using System . Windows . Shapes;

using NewWpfDev . Views;

namespace NewWpfDev . UserControls {
    /// <summary>
    /// Interaction logic for ComboboxPlus.xaml
    /// </summary>
    public class ComboChangedArgs {
        public object Itemselected = null;
        public ComboboxPlus CBplus = null;
    }
    public partial class ComboboxPlus : UserControl {

        #region NotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged ( string propertyName ) {
            if ( PropertyChanged != null ) {
                PropertyChanged ( this , new PropertyChangedEventArgs ( propertyName ) );
            }
        }
        #endregion NotifyPropertyChanged

        static public event EventHandler<ComboChangedArgs> ComboboxChanged;
        static public int blankindex { get; set; }
        static public int gridindex { get; set; }
        private List<string> ienum = new List<string> ( );

        private BankAcHost bankAcHost;
        public BankAcHost Host {
            get { return ( BankAcHost ) bankAcHost; }
            set { bankAcHost = value; NotifyPropertyChanged ( nameof ( Host ) ); }
        }

        public ComboboxPlus ( ) {
            InitializeComponent ( );
            comboBox . SelectionChanged += ComboBox_SelectionChanged;
            //            Host = this.Parent as BankAcHost;   
        }
        public void SetHost ( BankAcHost host ) {
            Host = host;
        }
        private void ComboBox_SelectionChanged ( object sender , SelectionChangedEventArgs e ) {
            SelectedItem = e . AddedItems [ 0 ];
            if ( ComboboxChanged != null ) {
                ComboChangedArgs args = new ComboChangedArgs ( );
                args . Itemselected = SelectedItem;
                args . CBplus = sender as ComboboxPlus;
                ComboboxChanged . Invoke ( this , args );
                selectioncount++;
                if ( selectioncount > 0 ) {
                    selectioncount = 1;
                    comboBox . Visibility = Visibility . Visible;
                }
                else {
                    comboBox . Visibility = Visibility . Hidden;
                }
                //BankAcHost bh = new BankAcHost ( );
                //if ( bh .Name.ToUpper()  == "HOSTWIN" ) {
                //    if(bh.BankContent.ToString() == "") {

                //    }

                //}
            }
        }

        //+++++++++
        #region DP's
        //+++++++++++++++++++++
        public List<string> ItemsList {
            get { return ( List<string> ) GetValue ( ItemsListProperty ); }
            set {
                SetValue ( ItemsListProperty , value );
                this . MyItemsSource = value;
            }
        }
        public static readonly DependencyProperty ItemsListProperty =
            DependencyProperty . Register ( "ItemsList" , typeof ( List<string> ) , typeof ( ComboboxPlus ) , new PropertyMetadata ( default ) );
        //++++++++++++++++++++++++++++++
        public IEnumerable MyItemsSource {
            get { return ( IEnumerable ) GetValue ( MyItemsSourceProperty ); }
            set {
                SetValue ( MyItemsSourceProperty , value );
                comboBox . ItemsSource = null;
                comboBox . Items . Clear ( );
                comboBox . ItemsSource = ItemsList;
            }
        }
        public static DependencyProperty MyItemsSourceProperty =
            DependencyProperty . Register ( "MyItemsSource" , typeof ( IEnumerable ) , typeof ( ComboboxPlus ) , new PropertyMetadata ( default ) );
        //++++++++++++++++++++++++++++++
        public Style ComboStyle {
            get { return ( Style ) GetValue ( ComboStyleProperty ); }
            set { SetValue ( ComboStyleProperty , value ); }
        }
        public static readonly DependencyProperty ComboStyleProperty =
            DependencyProperty . Register ( "ComboStyle" , typeof ( Style ) , typeof ( ComboboxPlus ) , new PropertyMetadata ( default ) );
        //++++++++++++++++++++++++++++++
        public string DefaultText {
            get { return ( string ) GetValue ( DefaultTextProperty ); }
            set { SetValue ( DefaultTextProperty , value ); }
        }
        public static DependencyProperty DefaultTextProperty =
                DependencyProperty . Register ( "DefaultText" , typeof ( string ) , typeof ( ComboboxPlus ) , new PropertyMetadata ( "Select Item ..." ) );
        //++++++++++++++++++++++++++++++
        public object SelectedItem {
            get { return GetValue ( SelectedItemProperty ); }
            set { SetValue ( SelectedItemProperty , value ); }
        }
        public static DependencyProperty SelectedItemProperty =
            DependencyProperty . Register ( "SelectedItem" , typeof ( object ) , typeof ( ComboboxPlus ) , new PropertyMetadata ( default ) );
        //++++++++++++++++++++++++++++++
        public int selectioncount {
            get { return ( int ) GetValue ( selectioncountProperty ); }
            set { SetValue ( selectioncountProperty , value ); }
        }
        public static readonly DependencyProperty selectioncountProperty =
            DependencyProperty . Register ( "selectioncount" , typeof ( int ) , typeof ( ComboboxPlus ) , new PropertyMetadata ( -1 ) );

        //++++++++++++++++++++++++++++++

        #endregion DP's

        private void comboBox_SelectionChanged ( object sender , SelectionChangedEventArgs e ) {

        }

        private void Promptlabel_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e ) {
            Promptlabel . Visibility = Visibility . Collapsed;
            comboBox . Visibility = Visibility . Visible;
        }
        //++++++++++++++++++++++++++++++
    }
}
