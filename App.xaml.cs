using System;
using System . Diagnostics;
using System . Threading . Tasks;
using System . Windows;

namespace NewWpfDev
{
    public partial class App : Application
    {
        public App ( ) : base ( )
        {
            SetupUnhandledExceptionHandling ( );
        }

        private void SetupUnhandledExceptionHandling ( )
        {
            // Catch exceptions from all threads in the AppDomain.
            AppDomain . CurrentDomain . UnhandledException += ( sender , args ) =>
                ShowUnhandledException ( args . ExceptionObject as Exception , "AppDomain.CurrentDomain.UnhandledException" , false );

            // Catch exceptions from each AppDomain that uses a task scheduler for async operations.
            TaskScheduler . UnobservedTaskException += ( sender , args ) =>
                ShowUnhandledException ( args . Exception , "TaskScheduler.UnobservedTaskException" , false );

            // Catch exceptions from a single specific UI dispatcher thread.
            Dispatcher . UnhandledException += ( sender , args ) =>
            {
                // If we are debugging, let Visual Studio handle the exception and take us to the code that threw it.
                if ( !Debugger . IsAttached )
                {
                    args . Handled = true;
                    ShowUnhandledException ( args . Exception , "Dispatcher.UnhandledException" , true );
                }
            };

            // Catch exceptions from the main UI dispatcher thread.
            // Typically we only need to catch this OR the Dispatcher.UnhandledException.
            // Handling both can result in the exception getting handled twice.
            Application . Current . DispatcherUnhandledException += ( sender , args ) =>
            {
                // If we are debugging, let Visual Studio handle the exception and take us to the code that threw it.
                if ( !Debugger . IsAttached )
                {
                    args . Handled = true;
                    ShowUnhandledException ( args . Exception , "Application.Current.DispatcherUnhandledException" , true );
                }
            };
        }

        void ShowUnhandledException ( Exception e , string unhandledExceptionType , bool promptUserForShutdown )
        {
            var messageBoxTitle = $"Unexpected Error Occurred: {unhandledExceptionType}";
            var messageBoxMessage = $"The following exception occurred:\n\n{e}";
            var messageBoxButtons = MessageBoxButton . OK;

            //if ( promptUserForShutdown )
            //{
            //    messageBoxMessage += "\n\nNormally the app would die now. Should we let it die?";
            //    messageBoxButtons = MessageBoxButton . YesNo;
            //}

            Debug . WriteLine ( messageBoxMessage );
            //{
            //    Application . Current . Shutdown ( );
            //}
        }
    }
}

//protected override void OnStartup ( StartupEventArgs e ) {
//    //ConsoleHelper . AllocConsole ( );
//    //Console . WriteLine ( "Start" );
//    //System . Threading . Thread . Sleep ( 1000 );
//    //Console . WriteLine ( "Stop" );
//    //ConsoleHelper . FreeConsole ( );
//    ////Shutdown ( 0 );
//}
//public   void XChangeSkin ( Skin newSkin ) {
//    Skin = newSkin;
//    foreach ( ResourceDictionary dict in Resources . MergedDictionaries ) {
//        if ( dict is SkinResourceDictionary skinDict )
//            skinDict . UpdateSource ( );
//        else
//            dict . Source = dict . Source;
//    }
//}
