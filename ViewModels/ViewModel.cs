using System;
using System . Collections;
using System . Collections . Generic;
using System . ComponentModel;
using System . Diagnostics;
using System . Linq;
using System . Runtime . CompilerServices;
using System . Text;
using System . Threading . Tasks;

namespace NewWpfDev. ViewModels
{
    public class ViewModel : INotifyPropertyChanged
    {
        private static Dictionary <string, object>VmDictionary  = new Dictionary<string, object> ();

        private static int MvCount { get; set; }
        public static ViewModel Viewmodel { get; set; }
        public ViewModel ( )
        {
            Viewmodel = this;
        }

        public static void SaveViewmodel ( string name, object viewmodel )
        {
            object obj = null;
            VmDictionary.TryGetValue( name . ToUpper ( ) , out obj);
            if ( obj == null )
            {
                VmDictionary . Add(name . ToUpper() , viewmodel);
                Debug. WriteLine($"[{name . ToUpper()}] added to dictionary");
                MvCount++;
            }
        }
        public static void ClearDictionary ( )
        {
            VmDictionary . Clear ( );
        }

        public static object GetViewmodel (string name)
        {
            object result = null;
            foreach ( KeyValuePair<string , object> item in VmDictionary )
            {
                if(item.Key.ToUpper() == name . ToUpper ( ) )
                {
                    result = item . Value;
                    break;
                }
            }
            return result;
        }
        public static Dictionary<string,object> GetAllViewModels()
        {
            return VmDictionary;
        }

        #region OnPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        //            [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged ( [CallerMemberName] string propertyName = null )
        {
            PropertyChanged?.Invoke ( this , new PropertyChangedEventArgs ( propertyName ) );
        }
        #endregion OnPropertyChanged

    }
}
