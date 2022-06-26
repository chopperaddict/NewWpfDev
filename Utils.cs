//using Microsoft . Win32;

using Microsoft . Win32;

using NewWpfDev. ViewModels;
using NewWpfDev. Views;

using System;
using System . Collections;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . Configuration;
using System . Data;
using System . Data . SqlClient;
using System . Diagnostics;
using System . IO;
//using System . Runtime . InteropServices . WindowsRuntime;
//using System . Security . Permissions;
using System . Text;
using System . Threading;
//using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Controls . Primitives;
using System . Windows . Data;
using System . Windows . Documents;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Media . Imaging;
using System . Windows . Shapes;

using static NewWpfDev. Views . DragDropClient;
//using System . Windows . Threading;



using Brush = System . Windows . Media . Brush;

namespace NewWpfDev
{
    /// <summary>
    /// Class to handle various utility functions such as fetching 
    /// Style/Templates/Brushes etc to Set/Reset control styles 
    /// from various Dictionary sources for use in "code behind"
    /// </summary>
    public class Utils
    {
        public static Action<DataGrid , int> GridInitialSetup = WpfLib1 . Utils .SetUpGridSelection;
        //		public static Func<bool, BankAccountViewModel, CustomerViewModel, DetailsViewModel> IsMatched = CheckRecordMatch; 
        public static Func<object , object , bool> IsRecordMatched = Utils .CompareDbRecords;

        // list each window that wants to support control capture needs to have so
        // mousemove can add current item under cursor to the list, and then F11 will display it.
        public static List<HitTestResult> ControlsHitList = new List<HitTestResult>();

        #region structures
        public struct bankrec
        {
            public string custno
            {
                get; set;
            }
            public string bankno
            {
                get; set;
            }
            public int actype
            {
                get; set;
            }
            public decimal intrate
            {
                get; set;
            }
            public decimal balance
            {
                get; set;
            }
            public DateTime odate
            {
                get; set;
            }
            public DateTime cdate
            {
                get; set;
            }
        }
        #endregion structures

        #region play tunes / sounds
        // Declare the first few notes of the song, "Mary Had A Little Lamb".
        // Define the frequencies of notes in an octave, as well as
        // silence (rest).
        protected enum Tone
        {
            REST = 0,
            GbelowC = 196,
            A = 220,
            Asharp = 233,
            B = 247,
            C = 262,
            Csharp = 277,
            D = 294,
            Dsharp = 311,
            E = 330,
            F = 349,
            Fsharp = 370,
            G = 392,
            Gsharp = 415,
        }

        // Define the duration of a note in units of milliseconds.
        protected enum Duration
        {
            WHOLE = 1600,
            HALF = WHOLE / 2,
            QUARTER = HALF / 2,
            EIGHTH = QUARTER / 2,
            SIXTEENTH = EIGHTH / 2,
        }

        protected struct Note
        {
            Tone toneVal;
            Duration durVal;

            // Define a constructor to create a specific note.
            public Note (Tone frequency , Duration time)
            {
                toneVal = frequency;
                durVal = time;
            }

            // Define properties to return the note's tone and duration.
            public Tone NoteTone
            {
                get
                {
                    return toneVal;
                }
            }
            public Duration NoteDuration
            {
                get
                {
                    return durVal;
                }
            }
        }
        public static void PlayMary ()
        {
            Note [ ] Mary =
                {
                            new Note(Tone.B, Duration.QUARTER),
                            new Note(Tone.A, Duration.QUARTER),
                            new Note(Tone.GbelowC, Duration.QUARTER),
                            new Note(Tone.A, Duration.QUARTER),
                            new Note(Tone.B, Duration.QUARTER),
                            new Note(Tone.B, Duration.QUARTER),
                            new Note(Tone.B, Duration.HALF),
                            new Note(Tone.A, Duration.QUARTER),
                            new Note(Tone.A, Duration.QUARTER),
                            new Note(Tone.A, Duration.HALF),
                            new Note(Tone.B, Duration.QUARTER),
                            new Note(Tone.D, Duration.QUARTER),
                            new Note(Tone.D, Duration.HALF)
                };
            // Play the song
            Play(Mary);
        }
        // Play the notes in a song.
        protected static void Play (Note [ ] tune)
        {
            foreach ( Note n in tune )
            {
                if ( n . NoteTone == Tone . REST )
                    Thread . Sleep(( int )n . NoteDuration);
                else
                    Console . Beep(( int )n . NoteTone , ( int )n . NoteDuration);
            }
        }
        // Define a note as a frequency (tone) and the amount of
        //// time (duration) the note plays.
        //public static Task DoBeep ( int freq = 180, int count = 300, bool swap = false )
        //{
        //	int tone = freq;
        //	int duration = count;
        //	int x = 0;
        //	Task t = new Task ( ( ) => x = 1 );
        //	if ( Flags . UseBeeps )
        //	{
        //		if ( swap )
        //		{
        //			tone = ( tone / 4 ) * 3;
        //			duration = ( count * 5 ) / 2;
        //			t = Task . Factory . StartNew ( ( ) => Console . Beep ( freq, count ) )
        //				. ContinueWith ( Action => Console . Beep ( tone, duration ) );
        //			Thread . Sleep ( 500 );
        //		}
        //		else
        //		{
        //			tone = ( tone / 4 ) * 3;
        //			duration = ( count * 5 ) / 2;
        //			t = Task . Factory . StartNew ( ( ) => Console . Beep ( tone, duration ) )
        //				. ContinueWith ( Action => Console . Beep ( freq, count ) );
        //			Thread . Sleep ( 500 );
        //		}
        //	}
        //	else
        //	{
        //		Task task = Task . Factory . StartNew ( ( ) => Debug. WriteLine ( ) );
        //		t = task ,TaskScheduler . FromCurrentSynchronizationContext ( );
        //			}

        //	TaskScheduler . FromCurrentSynchronizationContext ( ));
        //	return t;
        //}
        public static void DoSingleBeep (int freq = 280 , int count = 300 , int repeat = 1)
        {
            //			int x = 0;
            //			int i = 0;
            //Lambda test
            //			Task t = new Task ( ( ) => x = 1 );
            if ( Flags . UseBeeps )
            {
                for ( int i = 0 ; i < repeat ; i++ )
                {
                    Console . Beep(freq , count);
                    Thread . Sleep(200);
                }
                //else
                //	t = Task . Factory . StartNew ( ( ) => Debug. WriteLine ( ) );
                //			return t;
            }
        }
        public static void DoErrorBeep (int freq = 280 , int count = 100 , int repeat = 3)
        {
            //			int x = 0;
            //			Task t = new Task ( ( ) => x = 1 );
            if ( Flags . UseBeeps )
            {
                for ( int i = 0 ; i < repeat ; i++ )
                {
                    Console . Beep(freq , count);
                }
                Thread . Sleep(100);
            }
            return;
        }

        #endregion play tunes / sounds


        /// <summary>
        /// Simulate Application.DoEvents function of 
        /// <see cref=" System.Windows.Forms.Application"/> class.
        /// </summary>
        //[SecurityPermissionAttribute ( SecurityAction . Demand ,
        //    Flags = SecurityPermissionFlag . UnmanagedCode )]
        //public static void SetSynchforDbCollections ( object _lock ,
        //	ObservableCollection<BankAccountViewModel> bvmcollection ,
        //	ObservableCollection<CustomerViewModel> cvmcollection ,
        //	ObservableCollection<DetailsViewModel> dvmcollection
        //	)
        //{
        //	_lock = new object ( );
        //	BindingOperations . EnableCollectionSynchronization ( bvmcollection , _lock );
        //	_lock = new object ( );
        //	BindingOperations . EnableCollectionSynchronization ( cvmcollection , _lock );
        //	_lock = new object ( );
        //	BindingOperations . EnableCollectionSynchronization ( dvmcollection , _lock );
        //}

        #region Dictionary Handlers
        public static string GetDictionaryEntry (Dictionary<string , string> dict , string key , out string dictvalue)
        {
            string keyval = "";

            if ( dict . TryGetValue(key . ToUpper() , out keyval) == false )
            {
                Debug. WriteLine($"Unable to access Dictionary {dict} to identify key value [{key}]");
                key = key + "ConnectionString";
                //WpfLib1 . Utils .DoErrorBeep ( 250 , 50 , 1 );
            }
            dictvalue = keyval;
            return keyval;
        }
        public static string GetDictionaryEntry (Dictionary<int , string> dict , int key , out string dictvalue)
        {
            string keyval = "";

            if ( dict . TryGetValue(key , out keyval) == false )
            {
                Debug. WriteLine($"Unable to access Dictionary {dict} to identify key value [{key}]");
                WpfLib1 . Utils .DoErrorBeep(250 , 50 , 1);
            }
            dictvalue = keyval;
            return keyval;
        }
        public static int GetDictionaryEntry (Dictionary<int , int> dict , int key , out int dictvalue)
        {
            int keyval = 0;

            if ( dict . TryGetValue(key , out keyval) == false )
            {
                Debug. WriteLine($"Unable to access Dictionary {dict} to identify key value [{key}]");
                WpfLib1 . Utils .DoErrorBeep(250 , 50 , 1);
            }
            dictvalue = keyval;
            return keyval;
        }
        //public static bool AddDictionaryEntry ( Dictionary<string , string> dict , string key , string dictvalue )
        //{
        //	try
        //	{
        //		dict . Add ( key , dictvalue );
        //	}
        //	catch ( Exception ex )
        //	{
        //		Debug. WriteLine ( $"Unable to access Dictionary {dict} to identify key value [{key}]" );
        //		WpfLib1 . Utils .DoErrorBeep ( 250 , 50 , 1 );
        //		return false;
        //	}
        //	return true;
        //}
        public static bool DeleteDictionaryEntry (Dictionary<string , string> dict , string value)
        {
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            try
            {
                dict . Remove(value);
            }
            catch ( Exception ex )
            {
                Debug. WriteLine($"Unable to access Dictionary {dict} to delete key value [{value}]");
                WpfLib1 . Utils .DoErrorBeep(250 , 50 , 1);
                return false;
            }
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            return true;
        }
        public static bool DeleteDictionaryEntry (Dictionary<string , int> dict , string value)
        {
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            try
            {
                dict . Remove(value);
            }
            catch ( Exception ex )
            {
                Debug. WriteLine($"Unable to access Dictionary {dict} to delete key value [{value}]");
                WpfLib1 . Utils .DoErrorBeep(250 , 50 , 1);
                return false;
            }
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            return true;
        }
        public static bool DeleteDictionaryEntry (Dictionary<int , int> dict , int value)
        {
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            try
            {
                dict . Remove(value);
            }
            catch ( Exception ex )
            {
                Debug. WriteLine($"Unable to access Dictionary {dict} to delete key value [{value}]");
                WpfLib1 . Utils .DoErrorBeep(250 , 50 , 1);
                return false;
            }
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            return true;
        }
        #endregion Dictionary Handlers

        #region datagrid row  to List methods (string, int, double, decimal, DateTime)
        public static List<string> GetTableColumnsList (DataTable dt)
        {
            //Return a list of strings Containing table column info
            List<string> list = new List<string>();
            foreach ( DataRow row in dt . Rows )
            {
                string output = "";
                var colcount = row . ItemArray . Length;
                object type = null;
                switch ( colcount )
                {
                    case 1:
                        type = row . Field<object>(0);
                        output += row . Field<string>(0);
                        break;
                    case 2:
                        output += row . Field<string>(0) + ", ";
                        type = row . Field<object>(1);
                        output += row . Field<string>(1) + " ";
                        break;
                    case 3:
                        output += row . Field<string>(0) + ", ";
                        output += row . Field<string>(1) + ", ";
                        type = row . Field<object>(2);
                        if ( type != null )
                        {
                            if ( ( Type )type == typeof(int) )
                            {
                                output += row . Field<string>(2) . ToString() + "";
                            }
                        }
                        break;
                    case 4:
                        output += row . Field<string>(0) + ", ";
                        output += row . Field<string>(1) + ", ";
                        output += row . Field<string>(2) + ", ";
                        type = row . Field<object>(3);
                        output += row . Field<string>(3) + " ";
                        break;
                    case 5:
                        output += row . Field<string>(0) + ", ";
                        output += row . Field<string>(1) + ", ";
                        output += row . Field<string>(2) + ", ";
                        output += row . Field<string>(3) + ", ";
                        type = row . Field<object>(4);
                        output += row . Field<string>(4) + "";
                        break;
                }
                list . Add(output);
            }
            return list;
        }
        public static List<string> GetDataDridRowsAsListOfStrings (DataTable dt)
        {
            List<string> list = new List<string>();
            foreach ( DataRow row in dt . Rows )
            {
                var txt = row . Field<string>(0);
                list . Add(txt);
            }
            return list;
        }
        public static List<int> GetDataDridRowsAsListOfInts (DataTable dt)
        {
            List<int> list = new List<int>();
            foreach ( DataRow row in dt . Rows )
            {
                // ... Write value of first field as integer.
                list . Add(row . Field<int>(0));
            }
            return list;
        }
        public static List<double> GetDataDridRowsAsListOfDoubles (DataTable dt)
        {
            List<double> list = new List<double>();
            foreach ( DataRow row in dt . Rows )
            {
                // ... Write value of first field as integer.
                list . Add(row . Field<double>(0));
            }
            return list;
        }
        public static List<decimal> GetDataDridRowsAsListOfDecimals (DataTable dt)
        {
            List<decimal> list = new List<decimal>();
            foreach ( DataRow row in dt . Rows )
            {
                // ... Write value of first field as integer.
                list . Add(row . Field<decimal>(0));
            }
            return list;
        }
        public static List<DateTime> GetDataDridRowsAsListOfDateTime (DataTable dt)
        {
            List<DateTime> list = new List<DateTime>();
            foreach ( DataRow row in dt . Rows )
            {
                // ... Write value of first field as integer.
                list . Add(row . Field<DateTime>(0));
            }
            return list;
        }
        #endregion datagrid row  to List methods

        /// <summary>
        /// Parse Db Column information to display in FlowDoc
        /// </summary>
        /// <param name="fldnameslist"></param>
        /// <returns></returns>
                // Create dictionary of ALL Sql Connection strings we may want to use
        public static void LoadConnectionStrings ()
        {
            try
            {
                if ( Flags . ConnectionStringsDict . Count > 0 )
                    return;
                Flags . ConnectionStringsDict . Add("IAN1" , ( string )Properties . Settings . Default [ "BankSysConnectionString" ]);
                Flags . ConnectionStringsDict . Add("NORTHWIND" , ( string )Properties . Settings . Default [ "NorthwindConnectionString" ]);
                //Flags.ConnectionStringsDict . Add ( "NEWBANKACCOUNT" , ( string ) Properties . Settings . Default [ "NewBanksys" ] );
                Flags . ConnectionStringsDict . Add("PUBS" , ( string )Properties . Settings . Default [ "PubsConnectionString" ]);
            }
            catch ( NullReferenceException ex )
            {
                Debug. WriteLine($"Dictionary  entrry [{( string )Properties . Settings . Default [ "BankSysConnectionString" ]}] already exists");
            }
            finally
            {

            }
        }
        public static string ParseTableColumnData (List<string> fldnameslist)
        {
            int indx = 0;
            string entry = "", outp = "";
            int trimlen = 3;
            string output = "";
            foreach ( string row in fldnameslist )
            {
                if ( indx < 3 )
                {
                    entry = row;
                    if ( indx == 0 )
                        output += entry . ToUpper() + ",  ";
                    else if ( indx == 1 )
                        output += entry + ",  ";
                    else
                    {
                        if ( entry == "---" )
                        {
                            outp = output . Substring(0 , output . Length - trimlen);// += "\n";
                            output += "\n";
                            indx = 3;
                        }
                        else
                        {
                            //outp = output . Substring ( 0 , output . Length - trimlen );// += "\n";
                            output += entry + "\n";
                            indx = 3;
                        }
                    }
                    if ( indx < 3 )
                        indx++;
                    else
                        indx = 0;
                }
                else
                {
                    indx = 0;
                }
            }
            return output;
        }
        // Record the names of the method that called this one in an iterative tree.
        public static string trace (string prompt = "")
        {
            // logs all the calls made upwards in a tree
            string output = "", tmp = "";
            int indx = 1;
            var v = new StackTrace(0);
            if ( prompt != "" )
                output = prompt + $"\nStackTrace  for {prompt}:\n";
            while ( true )
            {
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
                try
                {

                    tmp = v . GetFrame(indx++) . GetMethod() . Name + '\n';
                    if ( tmp . Contains("Invoke") || tmp . Contains("RaiseEvent") )
                        break;
                    else
                        output += tmp;
                }
                catch ( Exception ex )
                {
                    Debug. WriteLine("trace() Crashed...\n");
                    output += "\ntrace() Crashed...\n";
                    break;
                }
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            }
            Debug. WriteLine($"\n{output}\n");
            return $"\n{output}\n";
        }
        //TODO
        //        public static void Mbox ( Window win , string string1 = "" , string string2 = "" , string caption = "" , string iconstring = "" , int Btn1 = 1 , int Btn2 = 0 , int Btn3 = 0 , int Btn4 = 0 , int defButton = 1 , bool minsize = false , bool modal = false )
        //        {
        //            // We NEED to remove any \r as part of \r\n as textboxes ONLY accept \n on its own for Cr/Lf
        //            string1 = ParseforCR ( string1 );
        ////TODO            Msgboxs m = new Msgboxs ( string1: string1 , string2: string2 , caption: caption , Btn1: Btn1 , Btn2: Btn2 , Btn3: Btn3 , Btn4: Btn4 , defButton: defButton , iconstring: iconstring , MinSize: minsize , modal: modal );
        //            //			m . Owner = win;

        //            if ( modal == false )
        //                m . Show ( );
        //            else
        //                m . ShowDialog ( );
        //        }
        //        public static void Mssg (
        //                        string caption = "" ,
        //                        string string1 = "" ,
        //                        string string2 = "" ,
        //                        string string3 = "" ,
        //                        string title = "" ,
        //                        string iconstring = "" ,
        //                        int defButton = 1 ,
        //                        int Btn1 = 1 ,
        //                        int Btn2 = 2 ,
        //                        int Btn3 = 3 ,
        //                        int Btn4 = 4 ,
        //                        string btn1Text = "" ,
        //                        string btn2Text = "" ,
        //                        string btn3Text = "" ,
        //                        string btn4Text = "" ,
        //                        bool usedialog = true
        //        )
        //        {
        //            Msgbox msg = new Msgbox (
        //                caption: caption ,
        //                string1: string1 ,
        //                string2: string2 ,
        //                string3: string3 ,
        //                title: title ,
        //                Btn1: Btn1 ,
        //                Btn2: Btn2 ,
        //                Btn3: Btn3 ,
        //                Btn4: Btn4 ,
        //                defButton: defButton ,
        //                iconstring: iconstring ,
        //                btn1Text: btn1Text ,
        //                btn2Text: btn2Text ,
        //                btn3Text: btn3Text ,
        //                btn4Text: btn4Text );
        //            //msg . Owner = win;
        //            if ( usedialog )
        //                msg . ShowDialog ( );
        //            else
        //                msg . Show ( );
        //        }
        public static Brush BrushFromColors (Color color)
        {
            Brush brush = new SolidColorBrush(color);
            return brush;
        }
        //Working well 4/8/21
        public static bool CompareDbRecords (object obj1 , object obj2)
        {
            bool result = false;
            BankAccountViewModel bvm = new BankAccountViewModel();
            CustomerViewModel cvm = new CustomerViewModel();
            DetailsViewModel dvm = new DetailsViewModel();
            //bvm = null;
            //cvm = null;
            //dvm = null;
            if ( obj1 == null || obj2 == null )
                return result;
            if ( obj1 . GetType() == bvm . GetType() )
                bvm = obj1 as BankAccountViewModel;
            if ( obj1 . GetType() == cvm . GetType() )
                cvm = obj1 as CustomerViewModel;
            if ( obj1 . GetType() == dvm . GetType() )
                dvm = obj1 as DetailsViewModel;

            if ( obj2 . GetType() == bvm . GetType() )
                bvm = obj2 as BankAccountViewModel;
            if ( obj2 . GetType() == cvm . GetType() )
                cvm = obj2 as CustomerViewModel;
            if ( obj2 . GetType() == dvm . GetType() )
                dvm = obj2 as DetailsViewModel;

            if ( bvm != null && cvm != null )
            {
                if ( bvm . CustNo == cvm . CustNo )
                    result = true;
            }
            else if ( bvm != null && dvm != null )
            {
                if ( bvm . CustNo == dvm . CustNo )
                    result = true;
            }
            else if ( cvm != null && dvm != null )
            {
                if ( cvm . CustNo == dvm . CustNo )
                    result = true;
            }
            result = false;
            return result;
        }
        public static string ConvertInputDate (string datein)
        {
            string YYYMMDD = "";
            string [ ] datebits;
            // This filter will strip off the "Time" section of an excel date
            // and return us a valid YYYY/MM/DD string
            char [ ] ch = { '/' , ' ' };
            datebits = datein . Split(ch);
            if ( datebits . Length < 3 )
                return datein;

            // check input to see if it needs reversing ?
            if ( datebits [ 0 ] . Length == 4 )
                YYYMMDD = datebits [ 0 ] + "/" + datebits [ 1 ] + "/" + datebits [ 2 ];
            else
                YYYMMDD = datebits [ 2 ] + "/" + datebits [ 1 ] + "/" + datebits [ 0 ];
            return YYYMMDD;
        }
        //TODO        public static BankDragviewModel CreateBankGridRecordFromString ( string input )
        //        {
        //            int index = 1;
        //            string type = "";
        //            //			BankAccountViewModel bvm = new BankAccountViewModel ( );
        //            BankDragviewModel bvm = new BankDragviewModel ( );
        //            CustomerDragviewModel cvm = new CustomerDragviewModel ( );

        //            char [ ] s = { ',' };
        //            string [ ] data = input . Split ( s );
        //            string donor = data [ 0 ];
        //            try
        //            {
        //                DateTime dt;
        //                type = data [ 0 ];
        //                if ( type == "BANKACCOUNT" || type == "BANK" || type == "DETAILS" )
        //                {
        //                    // This WORKS CORRECTLY 12/6/21 when called from n SQLDbViewer DETAILS grid entry && BANK grid entry					
        //                    // this test confirms the data layout by finding the Odate field correctly
        //                    // else it drops thru to the Catch branch
        //                    dt = Convert . ToDateTime ( data [ 7 ] );
        //                    //We can have any type of record in the string recvd
        //                    index = 1;  // jump the data type string
        //                    bvm . RecordType = type;
        //                    bvm . Id = int . Parse ( data [ index++ ] );
        //                    bvm . CustNo = data [ index++ ];
        //                    bvm . BankNo = data [ index++ ];
        //                    bvm . AcType = int . Parse ( data [ index++ ] );
        //                    bvm . IntRate = decimal . Parse ( data [ index++ ] );
        //                    bvm . Balance = decimal . Parse ( data [ index++ ] );
        //                    bvm . ODate = Convert . ToDateTime ( data [ index++ ] );
        //                    bvm . CDate = Convert . ToDateTime ( data [ index ] );
        //                    return bvm;
        //                }
        //            }
        //            catch
        //            {
        //                //Check to see if the data includes the data type in it
        //                //As we have to parse it diffrently if not - see index....
        //                index = 0;
        //#pragma warning disable CS0168 // The variable 'ex' is declared but never used
        //                try
        //                {
        //                    int x = int . Parse ( donor );
        //                    // if we get here, it IS a NUMERIC VALUE
        //                    index = 0;
        //                }
        //                catch ( Exception ex )
        //                {
        //                    //its probably the Data Type string, so ignore it for our Data creation processing
        //                    index = 1;
        //                }
        //#pragma warning restore CS0168 // The variable 'ex' is declared but never used
        //                //We have a CUSTOMER record
        //                bvm . RecordType = type;
        //                bvm . Id = int . Parse ( data [ index++ ] );
        //                bvm . CustNo = data [ index++ ];
        //                bvm . BankNo = data [ index++ ];
        //                bvm . AcType = int . Parse ( data [ index++ ] );
        //                bvm . ODate = Convert . ToDateTime ( data [ index++ ] );
        //                bvm . CDate = Convert . ToDateTime ( data [ index ] );
        //                return bvm;
        //            }
        //            return bvm;
        //        }
        public static RenderTargetBitmap CreateControlImage (FrameworkElement control , string filename = "" , bool savetodisk = false , GrabImageArgs ga = null)
        {
            if ( control == null )
                return null;
            // Get the Visual (Control) itself and the size of the Visual and its descendants.
            // This is the clever bit that gets the requested control, not the full window
            Rect rect = VisualTreeHelper . GetDescendantBounds(control);

            // Make a DrawingVisual to make a screen
            // representation of the control.
            DrawingVisual dv = new DrawingVisual();

            // Fill a rectangle the same size as the control
            // with a brush containing images of the control.
            using ( DrawingContext ctx = dv . RenderOpen() )
            {
                VisualBrush brush = new VisualBrush(control);
                ctx . DrawRectangle(brush , null , new Rect(rect . Size));
            }

            // Make a bitmap and draw on it.
            int width = ( int )control . ActualWidth;
            int height = ( int )control . ActualHeight;
            if ( height == 0 || width == 0 )
                return null;
            RenderTargetBitmap rtb = new RenderTargetBitmap(width , height , 96 , 96 , PixelFormats . Pbgra32);
            rtb . Render(dv);
            if ( savetodisk && filename != "" )
                SaveImageToFile(rtb , filename);
            return rtb;
        }
        public static CustomerViewModel CreateCustomerRecordFromString (string input)
        {
            int index = 1;
            CustomerViewModel cvm = new CustomerViewModel();
            char [ ] s = { ',' };
            string [ ] data = input . Split(s);
            string donor = data [ 0 ];
            //Check to see if the data includes the data type in it
            //As we have to parse it diffrently if not - see index....
            //if ( donor . Length > 3 )
            //        index =0 ;
            //We have the sender type in the string recvd
            cvm . Id = int . Parse(data [ index++ ]);
            cvm . CustNo = data [ index++ ];
            cvm . BankNo = data [ index++ ];
            cvm . AcType = int . Parse(data [ index++ ]);
            cvm . FName = data [ index++ ];
            cvm . LName = data [ index++ ];
            cvm . Addr1 = data [ index++ ];
            cvm . Addr2 = data [ index++ ];
            cvm . Town = data [ index++ ];
            cvm . County = data [ index++ ];
            cvm . PCode = data [ index++ ];
            cvm . Phone = data [ index++ ];
            cvm . Mobile = data [ index++ ];
            cvm . Dob = DateTime . Parse(data [ index++ ]);
            cvm . ODate = DateTime . Parse(data [ index++ ]);
            cvm . CDate = DateTime . Parse(data [ index ]);
            return cvm;
        }
        public static DetailsViewModel CreateDetailsRecordFromString (string input)
        {
            int index = 0;
            DetailsViewModel bvm = new DetailsViewModel();
            char [ ] s = { ',' };
            string [ ] data = input . Split(s);
            string donor = data [ 0 ];
            //Check to see if the data includes the data type in it
            //As we have to parse it diffrently if not - see index....
            if ( donor . Length > 3 )
                index = 1;
            bvm . Id = int . Parse(data [ index++ ]);
            bvm . CustNo = data [ index++ ];
            bvm . BankNo = data [ index++ ];
            bvm . AcType = int . Parse(data [ index++ ]);
            bvm . IntRate = decimal . Parse(data [ index++ ]);
            bvm . Balance = decimal . Parse(data [ index++ ]);
            bvm . ODate = DateTime . Parse(data [ index++ ]);
            bvm . CDate = DateTime . Parse(data [ index ]);
            return bvm;
        }


        public static void Magnify (List<object> list , bool magnify)
        {
            // lets other controls have magnification, providing other Templates do not overrule these.....
            for ( int i = 0 ; i < list . Count ; i++ )
            {
                var obj = list [ i ] as ListBox;
                if ( obj != null )
                {
                    obj . Style = magnify ? System . Windows . Application . Current . FindResource("ListBoxMagnifyAnimation4") as Style : null;
                    continue;
                }
                var lv = list [ i ] as ListView;
                if ( lv != null )
                {
                    lv . Style = magnify ? System . Windows . Application . Current . FindResource("ListViewMagnifyAnimation4") as Style : null;
                    continue;
                }
                var dg = list [ i ] as DataGrid;
                if ( dg != null )
                {
                    dg . Style = magnify ? System . Windows . Application . Current . FindResource("DatagridMagnifyAnimation4") as Style : null;
                    continue;
                }
                var bd = list [ i ] as Border;
                if ( bd != null )
                {
                    bd . Style = magnify ? System . Windows . Application . Current . FindResource("BorderMagnifyAnimation4") as Style : null;
                    continue;
                }
                var cb = list [ i ] as ComboBox;
                if ( cb != null )
                {
                    cb . Style = magnify ? System . Windows . Application . Current . FindResource("ComboBoxMagnifyAnimation4") as Style : null;
                    continue;
                }
                var btn = list [ i ] as Button;
                if ( btn != null )
                {
                    btn . Style = magnify ? System . Windows . Application . Current . FindResource("ButtonMagnifyAnimation4") as Style : null;
                    continue;
                }
                var rct = list [ i ] as Rectangle;
                if ( rct != null )
                {
                    rct . Style = magnify ? System . Windows . Application . Current . FindResource("RectangleMagnifyAnimation4") as Style : null;
                    continue;
                }
                var tb = list [ i ] as TextBlock;
                if ( tb != null )
                {
                    tb . Style = magnify ? System . Windows . Application . Current . FindResource("TextBlockMagnifyAnimation4") as Style : null;
                    continue;
                }
                var tbx = list [ i ] as TextBox;
                if ( tbx != null )
                {
                    tbx . Style = magnify ? System . Windows . Application . Current . FindResource("TextBoxMagnifyAnimation4") as Style : null;
                    continue;
                }
            }
        }
        public static bool HitTestScrollBar (object sender , MouseButtonEventArgs e)
        {
            //TODO
            //FlowDoc fd = new FlowDoc ( );

            //HitTestResult hit = VisualTreeHelper . HitTest ( ( Visual ) sender, e . GetPosition ( ( IInputElement ) sender ) );
            //string str = hit .VisualHit. ToString ( );
            //bool bl = str. Contains ( "Rectangle" );

            //RichTextBox rtb = sender as RichTextBox;
            //if ( rtb != null)
            //{
            //    fd.IsEnabled = true;
            //    return true;
            //}
            //fd = sender as FlowDoc;
            //if ( fd != null )
            //{
            //    fd.doc . IsEnabled = true;
            //    //fd.S
            //}
            //// Retrieve the coordinate of the mouse position.
            //Point pt = e . GetPosition ( fd );

            //PointHitTestParameters php = new PointHitTestParameters (pt );
            //Point pt2 = php . HitPoint;
            // pt & pt give same response
            // Perform the hit test against a given portion of the visual object tree.



            //HitTestResult result = VisualTreeHelper . HitTest (  pt2 , pt );
            //          double min = fd . ActualWidth - 30;
            //          double maxmin = fd . ActualWidth - 10;
            //{
            //    // Perform action on hit visual object.
            //    return true;
            //}
            ////			return hit . VisualHit . GetVisualAncestor<ScrollBar> ( ) != null;
            object original = e . OriginalSource;
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            try
            {
                var v = original . GetType();
                bool isScrollbar = original . GetType() . Equals(typeof(ScrollBar));
                //if ( bl == true )
                //{
                //    return true;
                //}
                if ( !isScrollbar . Equals(typeof(ScrollBar)) )
                {
                    if ( original . GetType() . Equals(typeof(DataGrid)) )
                    {
                        //						Debug. WriteLine ( "DataGrid is clicked" );
                        return false;
                    }
                    //else if ( original . GetType ( ) . Equals ( typeof ( Grid ) ) )
                    //{
                    //    //						Debug. WriteLine ( "Pararaph clicked" );
                    //    //return true;
                    //}
                    else if ( original . GetType() . Equals(typeof(Paragraph)) )
                    {
                        //						Debug. WriteLine ( "Pararaph clicked" );
                        return false;
                    }
                    else if ( original . GetType() . Equals(typeof(Border)) )
                    {
                        //						Debug. WriteLine ( "Border clicked" );
                        return false;
                    }
                    else if ( FindVisualParent<ScrollBar>(original as DependencyObject) != null )
                    {
                        //scroll bar is clicked
                        //						Debug. WriteLine ( "Calling FindVisualParent" );
                        return true;
                    }
                    return false;
                }
                else
                    return true;
            }
            catch ( Exception ex )
            {
                //               Debug . WriteLine ( $"Error in HitTest ScriollBar Function (Utils-1520({ex . Data}" );
            }
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            return true;
        }
        public static DependencyObject FindChild (DependencyObject o , Type childType)
        {
            DependencyObject foundChild = null;
            if ( o != null )
            {
                int childrenCount = VisualTreeHelper . GetChildrenCount(o);
                for ( int i = 0 ; i < childrenCount ; i++ )
                {
                    var child = VisualTreeHelper . GetChild(o , i);
                    if ( child . GetType() != childType )
                    {
                        foundChild = FindChild(child , childType);
                        //if(foundChild == null)
                        //        FindChild ( child, childType );
                    }
                    else
                    {
                        foundChild = child;
                        break;
                    }
                }
            }
            return foundChild;
        }
        //TODO
        //public static string CreateDragDataFromRecord ( BankDragviewModel bvm )
        //{
        //    if ( bvm == null )
        //        return "";
        //    string datastring = "";
        //    datastring = bvm . RecordType + ",";
        //    datastring += bvm . Id + ",";
        //    datastring += bvm . CustNo + ",";
        //    datastring += bvm . BankNo + ",";
        //    datastring += bvm . AcType . ToString ( ) + ",";
        //    datastring += bvm . IntRate . ToString ( ) + ",";
        //    datastring += bvm . Balance . ToString ( ) + ",";
        //    datastring += "'" + bvm . CDate . ToString ( ) + "',";
        //    datastring += "'" + bvm . ODate . ToString ( ) + "',";
        //    return datastring;
        //}
        public static string CreateFullCsvTextFromRecord (BankAccountViewModel bvm , DetailsViewModel dvm , CustomerViewModel cvm = null , bool IncludeType = true)
        {
            if ( bvm == null && cvm == null && dvm == null )
                return "";
            string datastring = "";
            if ( bvm != null )
            {
                // Handle a BANK Record
                if ( IncludeType )
                    datastring = "BANKACCOUNT";
                datastring += bvm . Id + ",";
                datastring += bvm . CustNo + ",";
                datastring += bvm . BankNo + ",";
                datastring += bvm . AcType . ToString() + ",";
                datastring += bvm . IntRate . ToString() + ",";
                datastring += bvm . Balance . ToString() + ",";
                datastring += "'" + bvm . CDate . ToString() + "',";
                datastring += "'" + bvm . ODate . ToString() + "',";
            }
            else if ( dvm != null )
            {
                if ( IncludeType )
                    datastring = "DETAILS,";
                datastring += dvm . Id + ",";
                datastring += dvm . CustNo + ",";
                datastring += dvm . BankNo + ",";
                datastring += dvm . AcType . ToString() + ",";
                datastring += dvm . IntRate . ToString() + ",";
                datastring += dvm . Balance . ToString() + ",";
                datastring += "'" + dvm . CDate . ToString() + "',";
                datastring += dvm . ODate . ToString() + ",";
            }
            else if ( cvm != null )
            {
                if ( IncludeType )
                    datastring = "CUSTOMER,";
                datastring += cvm . Id + ",";
                datastring += cvm . CustNo + ",";
                datastring += cvm . BankNo + ",";
                datastring += cvm . AcType . ToString() + ",";
                datastring += "'" + cvm . CDate . ToString() + "',";
                datastring += cvm . ODate . ToString() + ",";
            }
            return datastring;
        }
        public static T FindChild<T> (DependencyObject parent , string childName)
              where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if ( parent == null )
                return null;
            T foundChild = null;
            int childrenCount = VisualTreeHelper . GetChildrenCount(parent);
            for ( int i = 0 ; i < childrenCount ; i++ )
            {
                var child = VisualTreeHelper . GetChild(parent , i);
                // If the child is not of the request child type child
                T childType = child as T;
                if ( childType == null )
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child , childName);
                    // If the child is found, break so we do not overwrite the found child. 
                    if ( foundChild != null )
                        break;
                }
                else if ( !string . IsNullOrEmpty(childName) )
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if ( frameworkElement != null && frameworkElement . Name == childName )
                    {
                        // if the child's name is of the request name
                        foundChild = ( T )child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = ( T )child;
                    break;
                }
            }
            return foundChild;
        }
        public static int FindMatchingRecord (string Custno , string Bankno , DataGrid Grid , string currentDb = "")
        {
            int index = 0;
            bool success = false;
            if ( currentDb == "BANKACCOUNT" )
            {
                foreach ( var item in Grid . Items )
                {
                    BankAccountViewModel cvm = item as BankAccountViewModel;
                    if ( cvm == null )
                        break;
                    if ( cvm . CustNo == Custno && cvm . BankNo == Bankno )
                    {
                        success = true;
                        break;
                    }
                    index++;
                }
                if ( !success )
                    index = -1;
                return index;
            }
            else if ( currentDb == "CUSTOMER" )
            {
                foreach ( var item in Grid . Items )
                {
                    CustomerViewModel cvm = item as CustomerViewModel;
                    if ( cvm == null )
                        break;
                    if ( cvm . CustNo == Custno && cvm . BankNo == Bankno )
                    {
                        break;
                    }
                    index++;
                }
                if ( index == Grid . Items . Count )
                    index = -1;
                return index;
            }
            else if ( currentDb == "DETAILS" )
            {
                foreach ( var item in Grid . Items )
                {
                    DetailsViewModel dvm = item as DetailsViewModel;
                    if ( dvm == null )
                        break;
                    if ( dvm . CustNo == Custno && dvm . BankNo == Bankno )
                    {
                        break;
                    }
                    index++;
                }
                if ( index == Grid . Items . Count )
                    index = -1;
                return index;
            }
            return -1;
        }
        public static T FindVisualChildByName<T> (DependencyObject parent , string name) where T : DependencyObject
        {
            for ( int i = 0 ; i < VisualTreeHelper . GetChildrenCount(parent) ; i++ )
            {
                var child = VisualTreeHelper . GetChild(parent , i);
                string controlName = child . GetValue(Control . NameProperty) as string;
                if ( controlName == name )
                {
                    return child as T;
                }
                else
                {
                    T result = FindVisualChildByName<T>(child , name);
                    if ( result != null )
                        return result;
                }
            }
            return null;
        }
        public static T FindVisualParent<T> (UIElement element) where T : UIElement
        {
            UIElement parent = element;
            while ( parent != null )
            {
                var correctlyTyped = parent as T;
                if ( correctlyTyped != null )
                {
                    return correctlyTyped;
                }
                parent = VisualTreeHelper . GetParent(parent) as UIElement;
            }
            return null;
        }
        public static parentItem FindVisualParent<parentItem> (DependencyObject obj) where parentItem : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper . GetParent(obj);
            while ( parent != null && !parent . GetType() . Equals(typeof(parentItem)) )
            {
                parent = VisualTreeHelper . GetParent(parent);
            }
            return parent as parentItem;
        }
        public static string GetDataSortOrder (string commandline)
        {
            if ( Flags . SortOrderRequested == ( int )Flags . SortOrderEnum . DEFAULT )
                commandline += "Custno, BankNo";
            else if ( Flags . SortOrderRequested == ( int )Flags . SortOrderEnum . ID )
                commandline += "ID";
            else if ( Flags . SortOrderRequested == ( int )Flags . SortOrderEnum . BANKNO )
                commandline += "BankNo, CustNo";
            else if ( Flags . SortOrderRequested == ( int )Flags . SortOrderEnum . CUSTNO )
                commandline += "CustNo";
            else if ( Flags . SortOrderRequested == ( int )Flags . SortOrderEnum . ACTYPE )
                commandline += "AcType";
            else if ( Flags . SortOrderRequested == ( int )Flags . SortOrderEnum . DOB )
                commandline += "Dob";
            else if ( Flags . SortOrderRequested == ( int )Flags . SortOrderEnum . ODATE )
                commandline += "Odate";
            else if ( Flags . SortOrderRequested == ( int )Flags . SortOrderEnum . CDATE )
                commandline += "Cdate";
            return commandline;
        }
        public static Brush GetDictionaryBrush (string brushname)
        {
            Brush brs = null;
            try
            {
                brs = System . Windows . Application . Current . FindResource(brushname) as Brush;
            }
            catch
            {

            }
            return brs;
        }
        public static string GetExportFileName (string filespec = "")
        // opens  the common file open dialog
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd . InitialDirectory = @"C:\Users\ianch\Documents\";
            ofd . CheckFileExists = false;
            ofd . AddExtension = true;
            ofd . Title = "Select name for Exported data file.";
            if ( filespec . ToUpper() . Contains("XL") )
                ofd . Filter = "Excel Spreadsheets (*.xl*) | *.xl*";
            else if ( filespec . ToUpper() . Contains("CSV") )
                ofd . Filter = "Comma seperated data (*.csv) | *.csv";
            else if ( filespec . ToUpper() . Contains("*.*") )
                ofd . Filter = "All Files (*.*) | *.*";
            if ( filespec . ToUpper() . Contains("PNG") )
                ofd . Filter = "Image (*.png*) | *.pb*";
            else if ( filespec == "" )
            {
                ofd . Filter = "All Files (*.*) | *.*";
                ofd . DefaultExt = ".CSV";
            }
            ofd . FileName = filespec;
            ofd . ShowDialog();
            string fnameonly = ofd . SafeFileName;
            return ofd . FileName;
        }
        public static string GetImportFileName (string filespec)
        // opens  the common file open dialog
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd . InitialDirectory = @"C:\Users\ianch\Documents\";
            ofd . CheckFileExists = true;
            if ( filespec . ToUpper() . Contains("XL") )
                ofd . Filter = "Excel Spreadsheets (*.xl*) | *.xl*";
            else if ( filespec . ToUpper() . Contains("CSV") )
                ofd . Filter = "Comma seperated data (*.csv) | *.csv";
            else if ( filespec . ToUpper() . Contains("*.*") || filespec == "" )
                ofd . Filter = "All Files (*.*) | *.*";
            ofd . AddExtension = true;
            ofd . ShowDialog();
            return ofd . FileName;
        }
        public static Brush GetNewBrush (string color)
        {
            if ( color == "" || color == "#Unknown" )
                return null;
            else
            {
                if ( color [ 0 ] != '#' )
                    color = "#" + color;
                return ( Brush )new BrushConverter() . ConvertFrom(color);
            }
        }
        public static void GetWindowHandles ()
        {
#if SHOWWINDOWDATA
            Debug. WriteLine ( $"Current Windows\r\n" + "===============" );
            foreach ( Window window in System . Windows . Application . Current . Windows )
            {
                if ( ( string ) window . Title != "" && ( string ) window . Content != "" )
                {
                    Debug. WriteLine ( $"Title:  {window . Title },\r\nContent - {window . Content}" );
                    Debug. WriteLine ( $"Name = [{window . Name}]\r\n" );
                }
            }
#endif
        }
        public static void Grab_MouseMove (object sender , MouseEventArgs e)
        {
            Point pt = e . GetPosition(( UIElement )sender);
            HitTestResult hit = VisualTreeHelper . HitTest(( Visual )sender , pt);
            if ( hit?.VisualHit != null )
            {
                if ( ControlsHitList . Count != 0 )
                {
                    if ( hit . VisualHit == ControlsHitList [ 0 ] . VisualHit )
                        return;
                }
                ControlsHitList . Clear();
                ControlsHitList . Add(hit);
            }
        }
        public static void Grab_Object (object sender , Point pt)
        {
            //Point pt = e.GetPosition((UIElement)sender);
            HitTestResult hit = VisualTreeHelper . HitTest(( Visual )sender , pt);
            if ( hit?.VisualHit != null )
            {
                if ( ControlsHitList . Count != 0 )
                {
                    if ( hit . VisualHit == ControlsHitList [ 0 ] . VisualHit )
                        return;
                }
                ControlsHitList . Clear();
                ControlsHitList . Add(hit);
            }
        }
        public static void Grabscreen (Window parent , object obj , GrabImageArgs args , Control ctrl = null)
        {
            UIElement ui = obj as UIElement;
            UIElement OBJ = new UIElement();
            int indx = 0;
            bool success = false;
            // try to step up the visual tree ?
            do
            {
                indx++;
                if ( indx > 30 || indx < 0 )
                    break;
                switch ( indx )
                {
                    case 1:
                        OBJ = FindVisualParent<DataGrid>(ui);
                        if ( OBJ != null )
                            success = true;
                        break;
                    case 2:
                        OBJ = FindVisualParent<Button>(ui);
                        if ( OBJ != null )
                            success = true;
                        break;
                    case 3:
                        OBJ = FindVisualParent<Slider>(ui);
                        if ( OBJ != null )
                            success = true;
                        break;
                    case 4:
                        OBJ = FindVisualParent<ListView>(ui);
                        if ( OBJ != null )
                            success = true;
                        break;
                    case 5:
                        OBJ = FindVisualParent<ListBox>(ui);
                        if ( OBJ != null )
                            success = true;
                        break;
                    case 6:
                        OBJ = FindVisualParent<ComboBox>(ui);
                        if ( OBJ != null )
                            success = true;
                        break;
                    case 7:
                        OBJ = FindVisualParent<WrapPanel>(ui);
                        if ( OBJ != null )
                            success = true;
                        break;
                    case 8:
                        OBJ = FindVisualParent<CheckBox>(ui);
                        if ( OBJ != null )
                            success = true;
                        break;
                    case 9:
                        OBJ = FindVisualParent<DataGridRow>(ui);
                        if ( OBJ != null )
                            success = true;
                        break;
                    case 10:
                        OBJ = FindVisualParent<DataGridCell>(ui);
                        if ( OBJ != null )
                            success = true;
                        break;
                    case 11:
                        OBJ = FindVisualParent<Canvas>(ui);
                        if ( OBJ != null )
                            success = true;
                        break;
                    case 12:
                        OBJ = FindVisualParent<GroupBox>(ui);
                        if ( OBJ != null )
                            success = true;
                        break;
                    case 13:
                        OBJ = FindVisualParent<ProgressBar>(ui);
                        if ( OBJ != null )
                            success = true;
                        break;
                    case 14:
                        OBJ = FindVisualParent<Ellipse>(ui);
                        if ( OBJ != null )
                            success = true;
                        break;
                    case 15:
                        OBJ = FindVisualParent<RichTextBox>(ui);
                        if ( OBJ != null )
                            success = true;
                        break;
                    case 16:
                        OBJ = FindVisualParent<TextBlock>(ui);
                        if ( OBJ != null )
                            success = true;
                        break;
                    case 17:
                        //if(ui Equals TextBox)
                        if ( ui == null )
                            break;
                        DependencyObject v = new DependencyObject();
                        DependencyObject prev = new DependencyObject();
                        //OBJ = FindVisualParent<TextBox> ( ui );
                        do
                        {
                            v = VisualTreeHelper . GetParent(ui);
                            if ( v == null )
                                break;
                            prev = v;
                            TextBox tb = v as TextBox;
                            if ( tb != null && tb . Text . Length > 0 )
                            {
                                Debug. WriteLine($"UI = {tb . Text}");
                                OBJ = tb as UIElement;
                                success = true;
                                break;
                            }
                            Debug. WriteLine($"UI = {v . ToString()}");
                            ui = v as UIElement;
                        } while ( true );
                        break;
                    case 18:
                        OBJ = FindVisualParent<ContentPresenter>(ui);
                        if ( OBJ != null )
                            success = true;
                        break;
                    case 19:
                        OBJ = FindVisualParent<Grid>(ui);
                        if ( OBJ != null )
                            success = true;
                        break;
                    case 20:
                        OBJ = FindVisualParent<Window>(ui);
                        if ( OBJ != null )
                            success = true;
                        break;
                    case 21:
                        OBJ = FindVisualParent<ScrollContentPresenter>(ui);
                        if ( OBJ != null )
                            success = true;
                        break;
                    case 22:
                        OBJ = FindVisualParent<Rectangle>(ui);
                        if ( OBJ != null )
                            success = true;
                        break;
                    //case 23:
                    //	OBJ = FindVisualParent<TextBoxLineDrawingVisual> ( ui );
                    //	if ( OBJ != null )
                    //		success = true;
                    //	break;
                    default:
                        OBJ = ui;
                        success = false;
                        break;
                }
                if ( success == true && OBJ != null )
                    break;
            } while ( true );
            if ( success == false )
                return;
            Debug. WriteLine($"Element Identified for display = : [{OBJ . ToString()}]");
            //string str = OBJ . ToString ( );
            //if ( str . Contains ( ".TextBox" ) )
            //{
            //	var v  = OBJ.GetType();
            //	Debug. WriteLine ( $"Type is {v}" );
            //}
            ////OBJ = OBJ . Text;
            var bmp = Utils .CreateControlImage(OBJ as FrameworkElement);
            if ( bmp == null )
                return;
            WpfLib1 . Utils .SaveImageToFile(( RenderTargetBitmap )bmp , "C:\\Dev2\\Icons\\Grabimage.png" , "PNG");
            //TODO           Grabviewer gv = new Grabviewer ( parent , ctrl , bmp );
            //Setup the  image in our viewer
            //gv . Grabimage . Source = bmp;
            //gv . Title = "C:\\Dev2\\Icons\\Grabimage.png";
            //gv . Title += $"  :  RESIZE window to magnify the contents ...";
            //gv . Show ( );
            //// Save to disk file
        }
        public static bool HitTestBorder (object sender , MouseButtonEventArgs e)
        {
            object original = e . OriginalSource;
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            try
            {
                var v = original . GetType();
                if ( original . GetType() . Equals(typeof(Border)) )
                {
                    //                Debug. WriteLine ( "Border clicked" );
                    //					Mouse . SetCursor ( Cursors . SizeAll );
                    return true;
                }
                Type type = original . GetType();
                if ( type . Equals(typeof(TextBlock)) )
                {
                    //                    Debug. WriteLine ( "TextBlock clicked" );
                    return false;
                }
                if ( type . Equals(typeof(Grid)) )
                {
                    //                  Debug. WriteLine ( "Grid clicked" );
                    return false;
                }
                if ( type . Equals(typeof(TreeViewItem)) )
                {
                    //                  Debug. WriteLine ( "Grid clicked" );
                    return false;
                }
                else if ( FindVisualParent<Border>(original as DependencyObject) != null )
                {
                    //scroll bar is clicked
                    Debug. WriteLine("Calling FindVisualParent");
                    return true;
                }
                return false;
                //}
            }
            catch ( Exception ex )
            {
                //              Debug . WriteLine ( $"Error in HitTest ScriollBar Function (Utils-1520({ex . Data}" );
                return false;
            }
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            //		return true;
        }
        public static bool HitTestTreeViewItem (object sender , MouseButtonEventArgs e)
        {
            TreeView tv = sender as TreeView;
            object original = e . OriginalSource;
            var vv = e . Source;
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            try
            {
                var v = original . GetType();
                if ( original . GetType() . Equals(typeof(Border)) )
                {
                    //                Debug. WriteLine ( "Border clicked" );
                    //					Mouse . SetCursor ( Cursors . SizeAll );
                    return true;
                }
                Type type = original . GetType();
                if ( type . Equals(typeof(System . Windows . Shapes . Path)) )
                {
                    //                  Debug. WriteLine ( "Grid clicked" );
                    //IInputElement dropNode = tv . InputHitTest ( (Point)tv.GetPosition(tv).);
                    return false;
                }
                else if ( FindVisualParent<Border>(original as DependencyObject) != null )
                {
                    //scroll bar is clicked
                    Debug. WriteLine("Calling FindVisualParent");
                    return true;
                }
                return false;
                //}
            }
            catch ( Exception ex )
            {
                //              Debug . WriteLine ( $"Error in HitTest ScriollBar Function (Utils-1520({ex . Data}" );
                return false;
            }
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            //		return true;
        }
        private static string ParseforCR (string input)
        {
            string output = "";
            if ( input . Length == 0 )
                return input;
            if ( input . Contains("\r\n") )
            {
                do
                {
                    string [ ] fields = input . Split('\r');
                    foreach ( var item in fields )
                    {
                        output += item;
                    }
                    if ( output . Contains("\r") == false )
                        break;
                } while ( true );
            }
            else
                return input;
            return output;
        }
        // allows any image to be saved as PNG/GIF/JPG format, defaullt is PNG
        public static void SaveImageToFile (RenderTargetBitmap bmp , string file , string imagetype = "PNG")
        {
            string [ ] items;
            // Make a PNG encoder.
            if ( bmp == null )
                return;
            if ( file == "" && imagetype != "" )
                file = @"c:\users\ianch\pictures\defaultimage";
            items = file . Split('.');
            file = items [ 0 ];
            if ( imagetype == "PNG" )
                file += ".png";
            else if ( imagetype == "GIF" )
                file += ".gif";
            else if ( imagetype == "JPG" )
                file += ".jpg";
            else if ( imagetype == "BMP" )
                file += ".bmp";
            else if ( imagetype == "TIF" )
                file += ".tif";
            try
            {
                using ( FileStream fs = new FileStream(file ,
                            FileMode . Create , FileAccess . Write , FileShare . ReadWrite) )
                {
                    if ( imagetype == "PNG" )
                    {
                        PngBitmapEncoder encoder = new PngBitmapEncoder();
                        encoder . Frames . Add(BitmapFrame . Create(bmp));
                        encoder . Save(fs);
                    }
                    else if ( imagetype == "GIF" )
                    {
                        GifBitmapEncoder encoder = new GifBitmapEncoder();
                        encoder . Frames . Add(BitmapFrame . Create(bmp));
                        encoder . Save(fs);
                    }
                    else if ( imagetype == "JPG" || imagetype == "JPEG" )
                    {
                        JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                        encoder . Frames . Add(BitmapFrame . Create(bmp));
                        encoder . Save(fs);
                    }
                    else if ( imagetype == "BMP" )
                    {
                        BmpBitmapEncoder encoder = new BmpBitmapEncoder();
                        encoder . Frames . Add(BitmapFrame . Create(bmp));
                        encoder . Save(fs);
                    }
                    else if ( imagetype == "TIF" || imagetype == "TIFF" )
                    {
                        TiffBitmapEncoder encoder = new TiffBitmapEncoder();
                        encoder . Frames . Add(BitmapFrame . Create(bmp));
                        encoder . Save(fs);
                    }
                    fs . Close();
                }
            }
            catch ( Exception ex )
            {
                //TODO
                //    WpfLib1 . Utils .Mbox ( null , string1: "The image could not be saved for the following reason " , string2: $"{ex . Message}" , caption: "" , iconstring: "\\icons\\Information.png" , Btn1: MB . OK , Btn2: MB . NNULL , defButton: MB . OK );
            }
        }
        public static void ScrollLBRecordIntoView (ListBox lbox , int CurrentRecord)
        {
            // Works well 26/5/21

            //update and scroll to bottom first
            //lbox . SelectedIndex = ( int ) CurrentRecord;
            //lbox . SelectedItem = ( int ) CurrentRecord;
            //            lbox . UpdateLayout ( );
            lbox . ScrollIntoView(lbox . SelectedIndex);
            lbox . UpdateLayout();
            lbox . ScrollIntoView(lbox . SelectedItem);
            lbox . UpdateLayout();
        }
        public static void ScrollLVRecordIntoView (ListView Dgrid , int CurrentRecord)
        {
            // Works well 26/5/21

            //update and scroll to bottom first
            Dgrid . SelectedIndex = ( int )CurrentRecord;
            Dgrid . SelectedItem = ( int )CurrentRecord;
            //            Dgrid . UpdateLayout ( );
            Dgrid . ScrollIntoView(Dgrid . SelectedItem);
            Dgrid . UpdateLayout();
            //            Dgrid . ScrollIntoView ( Dgrid . SelectedItem );
            //            Dgrid . UpdateLayout ( );
        }
        public static void ScrollRecordIntoView (DataGrid Dgrid , int CurrentRecord , object row = null)
        {
            // Works well 26/5/21
            double currentTop = 0;
            double currentBottom = 0;
            if ( CurrentRecord == -1 || Dgrid == null )
                return;
            if ( Dgrid . Name == "CustomerGrid" || Dgrid . Name == "DataGrid1" )
            {
                currentTop = Flags . TopVisibleBankGridRow;
                currentBottom = Flags . BottomVisibleBankGridRow;
            }
            else if ( Dgrid . Name == "BankGrid" || Dgrid . Name == "DataGrid2" )
            {
                currentTop = Flags . TopVisibleCustGridRow;
                currentBottom = Flags . BottomVisibleCustGridRow;
            }
            else if ( Dgrid . Name == "DetailsGrid" || Dgrid . Name == "DetailsGrid" )
            {
                currentTop = Flags . TopVisibleDetGridRow;
                currentBottom = Flags . BottomVisibleDetGridRow;
            }     // Believe it or not, it takes all this to force a scrollinto view correctly

            if ( Dgrid == null || Dgrid . Items . Count == 0 )//|| Dgrid . SelectedItem == null )
                return;

            //update and scroll to bottom first
            //Dgrid . CurrentColumn = Dgrid . Columns [ 0 ];
            //Dgrid . Dispatcher . Invoke ( () =>
            //{
            //    //Dgrid . Focus ( );
            Dgrid . SelectedIndex = CurrentRecord;
            Dgrid . SelectedItem = CurrentRecord;
            //Dgrid . UpdateLayout ( );
            Dgrid . BringIntoView();
            //                Dgrid . ScrollIntoView ( row == null ? Dgrid . SelectedItem : row);
            Dgrid . UpdateLayout();
            //                Dgrid . ScrollIntoView ( row == null ? Dgrid . SelectedItem : row );
            //Dgrid . ScrollIntoView ( Dgrid . SelectedIndex );
            //ScrollRowInGrid ( Dgrid , Convert.ToInt16(row) );
            Dgrid . UpdateLayout();
            if ( CurrentRecord == 0 )
                Debug. WriteLine($"DataGrid Scroll is selecting record ZERO !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            //Dgrid . Refresh ( );
            //} );
            //TRANSFER			Flags . CurrentSqlViewer?.SetScrollVariables ( Dgrid );
        }
        static public DataGridRow GetRow (DataGrid dg , int index)
        {
            DataGridRow row = ( DataGridRow )dg . ItemContainerGenerator . ContainerFromIndex(index);
            if ( row == null )
            {
                // may be virtualized, bring into view and try again
                dg . ScrollIntoView(dg . Items [ index ]);
                row = ( DataGridRow )dg . ItemContainerGenerator . ContainerFromIndex(index);
            }
            return row;
        }
        //Generic form of Selection forcing code below
        public static void SetupWindowDrag (Window inst)
        {
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            try
            {
                //Handle the button NOT being the left mouse button
                // which will crash the DragMove Fn.....
                MouseButtonState mbs = Mouse . RightButton;
                //Debug. WriteLine ( $"{mbs . ToString ( )}" );
                if ( mbs == MouseButtonState . Pressed )
                    return;
                inst . MouseDown += delegate
                {
                    {
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
                        try
                        {
                            inst?.DragMove();
                        }
                        catch ( Exception ex )
                        {
                            return;
                        }
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
                    }
                };
            }
            catch ( Exception ex )
            {
                return;
            }
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
        }
        public static bool CheckResetDbConnection (string currdb , out string constring)
        {
            //string constring = "";
            currdb?.ToUpper();
            // This resets the current database connection to the one we re working with (currdb - in UPPER Case!)- should be used anywhere that We switch between databases in Sql Server
            // It also sets the Flags.CurrentConnectionString - Current Connectionstring  and local variable
            if ( WpfLib1 . Utils .GetDictionaryEntry(Flags . ConnectionStringsDict , currdb , out string connstring) != "" )
            {
                //				constring = connstring;
                if ( connstring != null )
                {
                    Flags . CurrentConnectionString = connstring;
                    SqlConnection con;
                    con = new SqlConnection(Flags . CurrentConnectionString);
                    if ( con != null )
                    {
                        constring = connstring;
                        con . Close();
                        return true;
                    }
                    else
                    {
                        constring = connstring;
                        return false;
                    }
                }
                {
                    constring = connstring;
                    return false;
                }
            }
            else
            {
                constring = connstring;
                return false;
            }

        }
        public static void SetUpGridSelection (DataGrid grid , int row = 0)
        {
            //			bool inprogress = false;
            //			int scrollrow = 0;
            if ( row == -1 )
                row = 0;
            // This triggers the selection changed event
            grid . SelectedIndex = row;
            grid . SelectedItem = row;
            //			grid . SetDetailsVisibilityForItem ( grid . SelectedItem, Visibility . Visible );
            grid . SelectedIndex = row;
            grid . SelectedItem = row;
            WpfLib1 . Utils .ScrollRecordIntoView(grid , row);
            grid . ScrollIntoView(grid . SelectedItem);
            grid . UpdateLayout();
            //grid . Refresh ( );
            //			var v = grid .VerticalAlignment;
        }
  
        /// <summary>
        /// A Generic data reader for any ObservableCollection&lt;T&gt; type
        /// </summary>
        ///<example>
        ///Call Method using loop such as :
        ///<code>
        ///<c>foreach ( int item in Utils. ReadGenericCollection ( <paramref name="collection"/> intcollection) )</c>
        ///</code>
        /// </example>
        /// or any similar looping construct
        /// <typeparam name="T">Any Generic Observable Collection</typeparam>
        /// <param name="collection"/>
        /// <returns>Individual records via yield return system to return items on demand, or NULL if collection cannot provide an Iterator 
        /// </returns>
        public static IEnumerable ReadGenericCollection<T> (ObservableCollection<T> collection , IEnumerator ie = null)
        {
            // Generic method to supply content of ANY Observablecollection<> type
            // Call it by a call such as  :-
            //  foreach ( BankCollection item in Utils.GenericRead ( BankCollection ) )
            //      {Debug. WriteLine ( item );}
            //or
            //  foreach ( int item in Utils.GenericRead ( integerCollection ) )
            //      {Debug. WriteLine ( item );}
            if ( collection . Count > 0 )
            {
                ie = collection . GetEnumerator();
                if ( ie != null )
                {
                    foreach ( var item in collection )
                    {

                        if ( ie . MoveNext() )
                            yield return item;
                    }
                }
            }
        }

        #region Nullable handlers
        public static bool? CompareNullable (int? a , int? b)
        {
            if ( Nullable . Compare<int>(a , b) == 0 )
            {
                $"Nullable int {a} is Equal to {b}" . cwinfo(); return true;
            }
            else
            {
                $"Nullable int {a} is NOT Equal to {b}" . cwinfo();
                return false;
            }
        }
        public static bool? CompareNullable (long? a , long? b)
        {
            if ( Nullable . Compare<long>(a , b) == 0 )
            {
                $"Nullable long {a} is Equal to {b}" . cwinfo();
                return true;
            }
            else
            {
                $"Nullable long {a} is NOT Equal to {b}" . cwinfo();
                return false;
            }
        }
        public static bool? CompareNullable (double? a , double? b)
        {
            if ( Nullable . Compare<double>(a , b) == 0 )
            {
                $"Nullable double {a} is Equal to {b}" . cwinfo();
                return true;
            }
            else
            {
                $"Nullable double int {a} is NOT Equal to {b}" . cwinfo();
                return false;
            }
        }
        public static bool? CompareNullable (float? a , float? b)
        {
            if ( Nullable . Compare<float>(a , b) == 0 )
            {
                $"Nullable float {a} is Equal to {b}" . cwinfo();
                return true;
            }
            else
            {
                $"Nullable float {a} is NOT Equal to {b}" . cwinfo();
                return false;
            }
        }
        public static bool? CompareNullable (decimal? a , decimal? b)
        {
            if ( Nullable . Compare<decimal>(a , b) == 0 )
            {
                $"Nullable decimal {a} is Equal to {b}" . cwinfo();
                return true;
            }
            else
            {
                $"Nullable decimal {a} is NOT Equal to {b}" . cwinfo();
                return false;
            }
        }

        #endregion Nullable handlers

        //public static string ? CompareNullable ( string? a , string? b )
        //{ return Nullable . Compare ( a , b ); }
        //public static BankAccountViewModel ? CompareNullable ( BankAccountViewModel ? a , BankAccountViewModel ? b )
        //{ return Nullable . Compare ( a , b ); }

        public static StringBuilder ReadFileGeneric (string path , ref StringBuilder sb)
        {
            string s = File . ReadAllText(path);
            sb . Append(s);
            return sb;
        }
        public static string ReadFileGeneric (string path , ref string sb)
        {
            sb = File . ReadAllText(path);
            return sb;
        }
        public static bool WriteFileGeneric (string path , string data)
        {
            File . WriteAllText(path , data);
            return true;
        }
        public static bool WriteFileGeneric (string path , StringBuilder data)
        {
            File . WriteAllText(path , data . ToString());
            return true;
        }

        public static List<object> GetChildControls (UIElement parent , string TypeRequired)
        {
            // this uses  the TabControlHelper class
            UIElement element = new UIElement();
            List<object> objects = new List<object>();
            IEnumerable alltabcontrols = null;
            //TODO
            //if ( TypeRequired == "*" )
            //    alltabcontrols = TabControlHelper . FindChildren<UIElement> ( parent );
            //else if ( TypeRequired == "Button" )
            //    alltabcontrols = TabControlHelper . FindChildren<Button> ( parent );
            //else if ( TypeRequired == "TextBlock" )
            //    alltabcontrols = TabControlHelper . FindChildren<TextBlock> ( parent );
            //else if ( TypeRequired == "DataGrid" )
            //    alltabcontrols = TabControlHelper . FindChildren<DataGrid> ( parent );
            //else if ( TypeRequired == "ListBox" )
            //    alltabcontrols = TabControlHelper . FindChildren<ListBox> ( parent );
            //else if ( TypeRequired == "ListView" )
            //    alltabcontrols = TabControlHelper . FindChildren<ListView> ( parent );
            //else if ( TypeRequired == "TextBox" )
            //    alltabcontrols = TabControlHelper . FindChildren<TextBox> ( parent );
            ////else if ( TypeRequired == "WrapPanel" )
            ////    alltabcontrols = TabControlHelper . FindChildren<WrapPanel> ( parent );
            //else if ( TypeRequired == "Border" )
            //    alltabcontrols = TabControlHelper . FindChildren<Border> ( parent );
            //else if ( TypeRequired == "Slider" )
            //    alltabcontrols = TabControlHelper . FindChildren<Slider> ( parent );
            //else if ( TypeRequired == "TabControl" )
            //    alltabcontrols = TabControlHelper . FindChildren<TabControl> ( parent );
            //else if ( TypeRequired == "TabItem" )
            //    alltabcontrols = TabControlHelper . FindChildren<TabItem> ( parent );
            //else if ( TypeRequired == "" )
            //    alltabcontrols = TabControlHelper . FindChildren<UIElement> ( parent );
            //if ( alltabcontrols != null )
            {
                int count = 0;
                IEnumerator enumerator = alltabcontrols . GetEnumerator();
                try
                {
                    while ( enumerator . MoveNext() )
                    {
                        count++;
                        var v = enumerator . Current;
                        objects . Add(v);
                    }
                }
                finally
                {
                    Debug. WriteLine($"Found {count} controls of  type {TypeRequired}");
                }
            }
            Debug. WriteLine("Finished FindChildren() 4\n");

            return objects;
        }
        #region ZERO referennces
        public static string convertToHex (double temp)
        {
            int intval = ( int )Convert . ToInt32(temp);
            string hexval = intval . ToString("X");
            return hexval;
        }
        //Working well 4/8/21
        /// <summary>
        /// Accepts color in Colors.xxxx format = "Blue" etc
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        /// <summary>
        /// Accpets string in "#XX00FF00" or similar
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Brush BrushFromHashString (string color)
        {
            //Must start with  '#'
            string s = color . ToString();
            if ( !s . Contains("#") )
                return WpfLib1 . Utils .BrushFromColors(Colors . Transparent);
            Brush brush = ( Brush )new BrushConverter() . ConvertFromString(color);
            return brush;
        }
        public static bool CheckForExistingGuid (Guid guid)
        {
            bool retval = false;
            for ( int x = 0 ; x < Flags . DbSelectorOpen . ViewersList . Items . Count ; x++ )
            {
                ListBoxItem lbi = new ListBoxItem();
                //lbi.Tag = viewer.Tag;
                lbi = Flags . DbSelectorOpen . ViewersList . Items [ x ] as ListBoxItem;
                if ( lbi . Tag == null )
                    return retval;
                Guid g = ( Guid )lbi . Tag;
                if ( g == guid )
                {
                    retval = true;
                    break;
                }
            }
            return retval;
        }
        public static bool CheckRecordMatch (BankAccountViewModel bvm , CustomerViewModel cvm , DetailsViewModel dvm)
        {
            bool result = false;
            if ( bvm != null && cvm != null )
            {
                if ( bvm . CustNo == cvm . CustNo )
                    result = true;
            }
            else if ( bvm != null && dvm != null )
            {
                if ( bvm . CustNo == dvm . CustNo )
                    result = true;
            }
            else if ( cvm != null && dvm != null )
            {
                if ( cvm . CustNo == dvm . CustNo )
                    result = true;
            }
            return result;
        }
        public static BankAccountViewModel CreateBankRecordFromString (string type , string input)
        {
            int index = 0;
            BankAccountViewModel bvm = new BankAccountViewModel();
            char [ ] s = { ',' };
            string [ ] data = input . Split(s);
            string donor = data [ 0 ];
            try
            {
                DateTime dt;
                if ( type == "BANKACCOUNT" || type == "SECACCOUNTS" )
                {
                    // This WORKS CORRECTLY 12/6/21 when called from n SQLDbViewer DETAILS grid entry && BANK grid entry					
                    // this test confirms the data layout by finding the Odate field correctly
                    // else it drops thru to the Catch branch
                    dt = Convert . ToDateTime(data [ 7 ]);
                    //We can have any type of record in the string recvd
                    index = 1;  // jump the data type string
                    bvm . Id = int . Parse(data [ index++ ]);
                    bvm . CustNo = data [ index++ ];
                    bvm . BankNo = data [ index++ ];
                    bvm . AcType = int . Parse(data [ index++ ]);
                    bvm . IntRate = decimal . Parse(data [ index++ ]);
                    bvm . Balance = decimal . Parse(data [ index++ ]);
                    bvm . ODate = Convert . ToDateTime(data [ index++ ]);
                    bvm . CDate = Convert . ToDateTime(data [ index ]);
                    return bvm;
                }
                else if ( type == "CUSTOMER" )
                {
                    // this test confirms the data layout by finding the Odate field correctly
                    // else it drops thru to the Catch branch
                    dt = Convert . ToDateTime(data [ 5 ]);
                    // We have a customer record !!
                    //Check to see if the data includes the data type in it
                    //As we have to parse it diffrently if not - see index....
                    index = 1;
                    bvm . Id = int . Parse(data [ index++ ]);
                    bvm . CustNo = data [ index++ ];
                    bvm . BankNo = data [ index++ ];
                    bvm . AcType = int . Parse(data [ index++ ]);
                    bvm . ODate = Convert . ToDateTime(data [ index++ ]);
                    bvm . CDate = Convert . ToDateTime(data [ index ]);
                }
                return bvm;
            }
            catch
            {
                //Check to see if the data includes the data type in it
                //As we have to parse it diffrently if not - see index....
                index = 0;
                try
                {
                    int x = int . Parse(donor);
                    // if we get here, it IS a NUMERIC VALUE
                    index = 0;
                }
                catch
                {
                    //its probably the Data Type string, so ignore it for our Data creation processing
                    index = 1;
                }
                //We have a CUSTOMER record
                bvm . Id = int . Parse(data [ index++ ]);
                bvm . CustNo = data [ index++ ];
                bvm . BankNo = data [ index++ ];
                bvm . AcType = int . Parse(data [ index++ ]);
                bvm . ODate = Convert . ToDateTime(data [ index++ ]);
                bvm . CDate = Convert . ToDateTime(data [ index ]);
                return bvm;
            }
        }
        public static CustomerDragviewModel CreateCustGridRecordFromString (string input)
        {
            int index = 0;
            string type = "";
            //			BankAccountViewModel bvm = new BankAccountViewModel ( );
            CustomerDragviewModel cvm = new CustomerDragviewModel();

            char [ ] s = { ',' };
            string [ ] data = input . Split(s);
            string donor = data [ 0 ];
            try
            {
                DateTime dt;
                type = data [ 0 ];
                // this test confirms the data layout by finding the Dob field correctly
                // else it drops thru to the Catch branch
                dt = Convert . ToDateTime(data [ 10 ]);
                // We have a customer record !!
                //Check to see if the data includes the data type in it
                //As we have to parse it diffrently if not - see index....
                index = 0;
                cvm . RecordType = type;
                cvm . Id = int . Parse(data [ index++ ]);
                cvm . CustNo = data [ index++ ];
                cvm . BankNo = data [ index++ ];
                cvm . AcType = int . Parse(data [ index++ ]);

                cvm . FName = data [ index++ ];
                cvm . LName = data [ index++ ];
                cvm . Town = data [ index++ ];
                cvm . County = data [ index++ ];
                cvm . PCode = data [ index++ ];

                cvm . Dob = Convert . ToDateTime(data [ index++ ]);
                cvm . ODate = Convert . ToDateTime(data [ index++ ]);
                cvm . CDate = Convert . ToDateTime(data [ index ]);
                return cvm;
            }
            catch
            {
                //Check to see if the data includes the data type in it
                //As we have to parse it diffrently if not - see index....
                index = 0;
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
                try
                {
                    int x = int . Parse(donor);
                    // if we get here, it IS a NUMERIC VALUE
                    index = 0;
                }
                catch ( Exception ex )
                {
                    //its probably the Data Type string, so ignore it for our Data creation processing
                    index = 1;
                }
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            }
            return cvm;
        }
        public static bool FindWindowFromTitle (string searchterm , ref Window handle)
        {
            bool result = false;
            foreach ( Window window in Application . Current . Windows )
            {
                if ( window . Title . ToUpper() . Contains(searchterm . ToUpper()) )
                {
                    handle = window;
                    result = true;
                    break;
                }
            }
            return result;
        }
        public static Brush GetBrush (string parameter)
        {
            if ( parameter == "BLUE" )
                return Brushes . Blue;
            else if ( parameter == "RED" )
                return Brushes . Red;
            else if ( parameter == "GREEN" )
                return Brushes . Green;
            else if ( parameter == "CYAN" )
                return Brushes . Cyan;
            else if ( parameter == "MAGENTA" )
                return Brushes . Magenta;
            else if ( parameter == "YELLOW" )
                return Brushes . Yellow;
            else if ( parameter == "WHITE" )
                return Brushes . White;
            else
            {
                //We appear to have received a Brushes Resource Name, so return that Brushes value
                Brush b = ( Brush )WpfLib1 . Utils .GetDictionaryBrush(parameter . ToString());
                return b;
            }
        }
        public static Brush GetBrushFromInt (int value)
        {
            switch ( value )
            {
                case 0:
                    return ( Brushes . White );
                case 1:
                    return ( Brushes . Yellow );
                case 2:
                    return ( Brushes . Orange );
                case 3:
                    return ( Brushes . Red );
                case 4:
                    return ( Brushes . Magenta );
                case 5:
                    return ( Brushes . Gray );
                case 6:
                    return ( Brushes . Aqua );
                case 7:
                    return ( Brushes . Azure );
                case 8:
                    return ( Brushes . Brown );
                case 9:
                    return ( Brushes . Crimson );
                case 10:
                    return ( Brushes . Transparent );
            }
            return ( Brush )null;
        }
        public static string GetPrettyGridStatistics (DataGrid Grid , int current)
        {
            string output = "";
            if ( current != -1 )
                output = $"{current} / {Grid . Items . Count}";
            else
                output = $"0 / {Grid . Items . Count}";
            return output;
        }
        public static ControlTemplate GetDictionaryControlTemplate (string tempname)
        {
            ControlTemplate ctmp = System . Windows . Application . Current . FindResource(tempname) as ControlTemplate;
            return ctmp;
        }
        public static Style GetDictionaryStyle (string tempname)
        {
            Style ctmp = System . Windows . Application . Current . FindResource(tempname) as Style;
            return ctmp;
        }
        public static object GetTemplateControl (Control RectBtn , string CtrlName)
        {
            var template = RectBtn . Template;
            object v = template . FindName(CtrlName , RectBtn) as object;
            return v;
        }
        public static void ReadAllConfigSettings ()
        {
            try
            {
                var appSettings = ConfigurationManager . AppSettings;

                if ( appSettings . Count == 0 )
                {
                    Debug. WriteLine("AppSettings is empty.");
                }
                else
                {
                    foreach ( var key in appSettings . AllKeys )
                    {
                        Debug. WriteLine("Key: {0} Value: {1}" , key , appSettings [ key ]);
                    }
                }
            }
            catch ( ConfigurationErrorsException )
            {
                Debug. WriteLine("Error reading app settings");
            }
        }
        public static void HandleCtrlFnKeys (bool key1 , KeyEventArgs e)
        {
            if ( key1 && e . Key == Key . F5 )
            {
                // list Flags in Console
                WpfLib1 . Utils .GetWindowHandles();
                e . Handled = true;
                key1 = false;
                return;
            }
            else if ( key1 && e . Key == Key . F6 )  // CTRL + F6
            {
                // list various Flags in Console
                //Debug . WriteLine ( $"\nCTRL + F6 pressed..." );
                Flags . UseBeeps = !Flags . UseBeeps;
                e . Handled = true;
                key1 = false;
                //Debug . WriteLine ( $"Flags.UseBeeps reset to  {Flags . UseBeeps }" );
                return;
            }
            else if ( key1 && e . Key == Key . F7 )  // CTRL + F7
            {
                // list various Flags in Console
                //Debug . WriteLine ( $"\nCTRL + F7 pressed..." );
                //TRANSFER				Flags . PrintDbInfo ( );
                e . Handled = true;
                key1 = false;
                return;
            }
            else if ( key1 && e . Key == Key . F8 )     // CTRL + F8
            {
                //Debug . WriteLine ( $"\nCTRL + F8 pressed..." );
                EventHandlers . ShowSubscribersCount();
                e . Handled = true;
                key1 = false;
                return;
            }
            else if ( key1 && e . Key == Key . F9 )     // CTRL + F9
            {
                //Debug . WriteLine ( "\nCtrl + F9 NOT Implemented" );
                key1 = false;
                return;

            }
            else if ( key1 && e . Key == Key . System )     // CTRL + F10
            {
                // Major  listof GV[] variables (Guids etc]
                //Debug . WriteLine ( $"\nCTRL + F10 pressed..." );
                //TRANSFER				Flags . ListGridviewControlFlags ( 1 );
                key1 = false;
                e . Handled = true;
                return;
            }
            else if ( key1 && e . Key == Key . F11 )  // CTRL + F11
            {
                // list various Flags in Console
                //Debug . WriteLine ( $"\nCTRL + F11 pressed..." );
                //				Flags . PrintSundryVariables ( );
                e . Handled = true;
                key1 = false;
                return;
            }
        }
        public static string ReadConfigSetting (string key)
        {
            string result = "";
            try
            {
                var appSettings = ConfigurationManager . AppSettings;
                result = appSettings [ key ] ?? "Not Found";
                Debug. WriteLine(result);
            }
            catch ( ConfigurationErrorsException )
            {
                Debug. WriteLine("Error reading app settings");
            }
            return result;
        }
        /// <summary>
        /// Creates a BMP from any control passed into it   ???
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static RenderTargetBitmap RenderBitmap (Visual element , double objwidth = 0 , double objheight = 0 , string filename = "" , bool savetodisk = false)
        {
            double topLeft = 0;
            double topRight = 0;
            int width = 0;
            int height = 0;

            if ( element == null )
                return null;
            Rect bounds = VisualTreeHelper . GetDescendantBounds(element);
            if ( objwidth == 0 )
                width = ( int )bounds . Width;
            if ( objheight == 0 )
                height = ( int )bounds . Height;
            double dpiX = 96; // this is the magic number
            double dpiY = 96; // this is the magic number

            PixelFormat pixelFormat = PixelFormats . Default;
            VisualBrush elementBrush = new VisualBrush(element);
            DrawingVisual visual = new DrawingVisual();
            DrawingContext dc = visual . RenderOpen();

            dc . DrawRectangle(elementBrush , null , new Rect(topLeft , topRight , width , height));
            dc . Close();
            RenderTargetBitmap rtb = new RenderTargetBitmap(( int )( bounds . Width * dpiX / 96.0 ) , ( int )( bounds . Height * dpiY / 96.0 ) , dpiX , dpiY , PixelFormats . Pbgra32);
            DrawingVisual dv = new DrawingVisual();
            using ( DrawingContext ctx = dv . RenderOpen() )
            {
                VisualBrush vb = new VisualBrush(element);
                ctx . DrawRectangle(vb , null , new Rect(new Point() , bounds . Size));
            }
            rtb . Render(dv);

            if ( savetodisk && filename != "" )
                SaveImageToFile(rtb , filename);
            return rtb;
        }
        public static void SaveProperty (string setting , string value)
        {
            try
            {
                if ( value . ToUpper() . Contains("TRUE") )
                    Properties.Settings . Default [ setting ] = true;
                else if ( value . ToUpper() . Contains("FALSE") )
                    Properties . Settings . Default [ setting ] = false;
                else
                    Properties . Settings . Default [ setting ] = value;
                Properties . Settings . Default . Save();
                Properties . Settings . Default . Upgrade();
                ConfigurationManager . RefreshSection(setting);
            }
            catch ( Exception ex )
            {
                Debug . WriteLine($"Unable to save property {setting} of [{value}]\nError was {ex . Data}, {ex . Message}, Stack trace = \n{ex . StackTrace}");
            }
        }
        public static void SelectTextBoxText (TextBox txtbox)
        {
            txtbox . SelectionLength = txtbox . Text . Length;
            txtbox . SelectionStart = 0;
            txtbox . SelectAll();
        }
        public static bool HitTestHeaderBar (object sender , MouseButtonEventArgs e)
        {
            //			HitTestResult hit = VisualTreeHelper . HitTest ( ( Visual ) sender, e . GetPosition ( ( IInputElement ) sender ) );
            //			return hit . VisualHit . GetVisualAncestor<ScrollBar> ( ) != null;
            object original = e . OriginalSource;

            if ( !original . GetType() . Equals(typeof(DataGridColumnHeader)) )
            {
                if ( original . GetType() . Equals(typeof(DataGrid)) )
                {
                    Debug. WriteLine("DataGrid is clicked");
                }
                else if ( FindVisualParent<DataGridColumnHeader>(original as DependencyObject) != null )
                {
                    //Header bar is clicked
                    return true;
                }
                return false;
            }
            return true;
        }
        public static void SetGridRowSelectionOn (DataGrid dgrid , int index)
        {
            if ( dgrid . Items . Count > 0 && index != -1 )
            {
                try
                {
                    //Setup new selected index
                    dgrid . SelectedIndex = index;
                    dgrid . SelectedItem = index;
                    dgrid . UpdateLayout();
                    dgrid . BringIntoView();
                    object obj = dgrid . Items [ index ];
                    if ( obj . GetType() == typeof(BankAccountViewModel) )
                    {
                        BankAccountViewModel item = dgrid . Items [ index ] as BankAccountViewModel;
                        dgrid . ScrollIntoView(item);
                    }
                    else if ( obj . GetType() == typeof(CustomerViewModel) )
                    {
                        CustomerViewModel item = dgrid . Items [ index ] as CustomerViewModel;
                        dgrid . ScrollIntoView(item);
                    }
                    else if ( obj . GetType() == typeof(GenericClass) )
                    {
                        GenericClass item = dgrid . Items [ index ] as GenericClass;
                        dgrid . ScrollIntoView(item);
                    }
                }
                catch ( Exception ex )
                {
                    Debug . WriteLine($"{ex . Message}, {ex . Data}");
                }
            }
        }
        public static void SetSelectedItemFirstRow (object dataGrid , object selectedItem)
        {
            //If target datagrid Empty, throw exception
            if ( dataGrid == null )
            {
                throw new ArgumentNullException("Target none" + dataGrid + "Cannot convert to DataGrid");
            }
            //Get target DataGrid，If it is empty, an exception will be thrown
            System . Windows . Controls . DataGrid dg = dataGrid as System . Windows . Controls . DataGrid;
            if ( dg == null )
            {
                throw new ArgumentNullException("Target none" + dataGrid + "Cannot convert to DataGrid");
            }
            //If the data source is empty, return
            if ( dg . Items == null || dg . Items . Count < 1 )
            {
                return;
            }

            dg . SelectedItem = selectedItem;
            dg . CurrentColumn = dg . Columns [ 0 ];
            dg . ScrollIntoView(dg . SelectedItem);
        }
        public static void SetUpGListboxSelection (ListBox grid , int row = 0)
        {
            if ( row == -1 )
                row = 0;
            // This triggers the selection changed event
            grid . SelectedIndex = row;
            grid . SelectedItem = row;
            WpfLib1 . Utils .ScrollLBRecordIntoView(grid , row);
            grid . UpdateLayout();
            grid . Refresh();
            //			var v = grid .VerticalAlignment;
        }
        /// <summary>
        /// MASTER UPDATE METHOD
        /// This handles repositioning of a selected item in any grid perfectly
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="row"></param>
        /// <summary>
        /// Metohd that almost GUARANTESS ot force a record into view in any DataGrid
        /// /// This is called by method above - MASTER Updater Method
        /// </summary>
        /// <param name="dGrid"></param>
        /// <param name="row"></param>
        public static void ScrollRowInGrid (DataGrid dGrid , int row)
        {
            if ( dGrid . SelectedItem == null ) return;

            dGrid . ScrollIntoView(dGrid . SelectedItem);
            dGrid . UpdateLayout();
            //           WpfLib1 . Utils .ScrollRecordIntoView ( dGrid , row );
            //            dGrid . UpdateLayout ( );
        }
        //********************************************************************************************************************************************************************************//
        public static void NewCookie_Click (object sender , RoutedEventArgs e)
        //********************************************************************************************************************************************************************************//
        {
            //NewCookie nc = new NewCookie(sender as Window);
            //nc . ShowDialog ( );
            //defvars . CookieAdded = false;
        }
        public static void LoadBankDbGeneric (BankCollection bvm , string caller = "" , bool Notify = false , int lowvalue = -1 , int highvalue = -1 , int maxrecords = -1)
        {
            if ( maxrecords == -1 )
            {
                DataTable dt = new DataTable();
                BankCollection . LoadBank(bvm , caller: caller , ViewerType: 99 , NotifyAll: Notify);
            }
            else
            {
                DataTable dtBank = new DataTable();
                dtBank = BankCollection . LoadSelectedBankData(Min: lowvalue , Max: highvalue , Tot: maxrecords);
                bvm = BankCollection . LoadSelectedCollection(bankCollection: bvm , max: -1 , dtBank: dtBank , Notify: Notify);
            }

        }
        public static bool IsMousRightBtnDn (object sender , MouseEventArgs e)
        {
            e . Handled = true;
            if ( e . RightButton == MouseButtonState . Pressed )
                return true;
            return false;
        }
        public static void TrackSplitterPosition (TextBlock Textblock , double MaxWidth , DragDeltaEventArgs e)
        {
            Thickness th = new Thickness(0 , 0 , 0 , 0);
            th = Textblock . Margin;
            if ( th . Left < MaxWidth )
            {
                th . Left += e . HorizontalChange;
                if ( th . Left > 10 )
                    Textblock . Margin = th;
            }
        }

        #endregion ZERO referennces
    }

}
