using System;
using System . Collections . Generic;
using System . IO;
using System . Text;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Data;
using System . Windows . Documents;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Media . Imaging;
using System . Windows . Shapes;

namespace Views
{
    /// <summary>
    /// Interaction logic for TraceViewer.xaml
    /// </summary>
    public partial class TraceViewer : Window
    {
        public TraceViewer ( )
        {
            InitializeComponent ( );
            try
            {
                string [ ] s = File . ReadAllLines ( $@"C:\users\ianch\Documents\NewWpfDev.Trace.log" );
                if ( s != null )
                {
                    TraceView . ItemsSource = s;
                    this . Show ( );
                }
                else
                    MessageBox . Show ( $"Sorry, the log file [@\"C:\\users\\ianch\\Documents\\NewWpfDev.Trace.log\"] could not be found\nPerhaps the #Define is not set ??" , "File Error" );
            }
            catch ( Exception ex ) { }
        }
    }
}
