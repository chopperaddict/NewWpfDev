using System;
using System . Diagnostics;
using System . Reflection;
using System . Runtime . InteropServices;

namespace NewWpfDev {
    /// <summary>
    /// Special class to allow access to the default Windows MessageBox
    /// </summary>
    public static class MBox {
        [DllImport ( "user32.dll" , EntryPoint = "MessageBox" )]

        /////1 = Ok/Cancel, 2=Abort/Retry/Ignore, 3=Yes/No/Cancel, 4=Yes/No, 5=Retry/Cancel, 6=Cancel/Try again/Continue
        public static extern int ShowMessageBox ( int hWnd , string text , string caption , uint type );

        private static void GetLibmethods ( Type t ) {
            Debug . WriteLine ( "*********Methods*********" );
            MethodInfo [ ] mth = t . GetMethods ( );
            foreach ( MethodInfo m in mth ) {
                Debug . WriteLine ( "-->{0}" , m . Name );
            }
        }
        public static void ListMethods (string  obj ) {
            Type t = Type . GetType ( obj );
            if(t != null)
                GetLibmethods ( t );
        }
    }
 
}
