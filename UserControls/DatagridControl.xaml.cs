using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . Text;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Data;
using System . Windows . Documents;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Media . Imaging;
using System . Windows . Navigation;
using System . Windows . Shapes;

using NewWpfDev;

namespace UserControls
{
    /// <summary>
    /// Interaction logic for DatagridControl.xaml
    /// </summary>
    public partial class DatagridControl : UserControl
    {
        public DatagridControl ( )
        {
            InitializeComponent ( );
        }


        public ObservableCollection<GenericClass> Data
        {
            get { return ( ObservableCollection<GenericClass> ) GetValue ( DataProperty ); }
            set { SetValue ( DataProperty , value ); }
        }
        public static readonly DependencyProperty DataProperty =
            DependencyProperty . Register ( "Data" ,
                typeof ( ObservableCollection<GenericClass> ) ,
                typeof ( DatagridControl ) ,
                new PropertyMetadata ( ( ObservableCollection<GenericClass> ) null ) );
        
        #region Dependency Properties
        public string Tablename
        {   get { return ( string ) GetValue ( TablenameProperty ); }
            set { SetValue ( TablenameProperty , value ); }}
        public static readonly DependencyProperty TablenameProperty =
            DependencyProperty . Register ( "Tablename" , typeof ( string ) , typeof ( DatagridControl ) , new PropertyMetadata ( "" ) );

        public int Selection
        {   get { return ( int ) GetValue ( SelectionProperty ); }
            set { SetValue ( SelectionProperty , value ); }}
        public static readonly DependencyProperty SelectionProperty =
            DependencyProperty . Register ( "Selection" , typeof ( int ) , typeof ( DatagridControl ) , new PropertyMetadata ( (int)0 ) );

        public DataTemplate GridDataTemplate
            {   get { return ( DataTemplate ) GetValue ( GridDataTemplateProperty ); }
            set { SetValue ( GridDataTemplateProperty , value ); }}
        public static readonly DependencyProperty GridDataTemplateProperty =
            DependencyProperty . Register ( "GridDataTemplate" , typeof ( DataTemplate ) , typeof ( DatagridControl ) , new PropertyMetadata ( default ) );

        public Style GridStyle
            {   get { return ( Style ) GetValue ( GridStyleProperty ); }
            set { SetValue ( GridStyleProperty , value ); }}
        public static readonly DependencyProperty GridStyleProperty =
            DependencyProperty . Register ( "GridStyle" , typeof ( Style ) , typeof ( DatagridControl ) , new PropertyMetadata ( 0 ) );

        #endregion Dependency Properties

        public static ObservableCollection<GenericClass> LoadGenericData ( ObservableCollection<GenericClass>  collection, string table )
        {
            return collection;
        }

    }
}
