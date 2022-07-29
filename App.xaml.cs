using System;
using System . Collections . Generic;
using System . Configuration;
using System . Data;
using System . Linq;
using System . Threading . Tasks;
using System . Windows;

using NewWpfDev . Dicts;

namespace NewWpfDev {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
//public enum Skin { Red, Blue }
    public partial class App : Application {
  //      public static Skin Skin { get; set; } = Skin . Blue;
        protected override void OnStartup ( StartupEventArgs e ) {
            //ConsoleHelper . AllocConsole ( );
            //Console . WriteLine ( "Start" );
            //System . Threading . Thread . Sleep ( 1000 );
            //Console . WriteLine ( "Stop" );
            //ConsoleHelper . FreeConsole ( );
            ////Shutdown ( 0 );
        }
        //public   void XChangeSkin ( Skin newSkin ) {
        //    Skin = newSkin;
        //    foreach ( ResourceDictionary dict in Resources . MergedDictionaries ) {
        //        if ( dict is SkinResourceDictionary skinDict )
        //            skinDict . UpdateSource ( );
        //        else
        //            dict . Source = dict . Source;
        //    }
        //}
    }
}
