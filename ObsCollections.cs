using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . Text;

namespace NewWpfDev
{
    [Serializable]
    public class ObsCollections
    {
        public static ObservableCollection<GenericClass> GenericCollection{ get; set; }

        public ObsCollections ( )
        {
            GenericCollection = new ObservableCollection<GenericClass> ();
        }
    }
}
