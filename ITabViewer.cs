using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . Linq;
using System . Text;
using System . Threading . Tasks;

using NewWpfDev . ViewModels;

namespace NewWpfDev {
    interface ITabViewer {
        // Data collections
        static ObservableCollection<BankAccountViewModel> Bvm { get; set; }
        static ObservableCollection<CustomerViewModel> Cvm { get; set; }
        static ObservableCollection<GenericClass> Gvm { get; set; }

    }
}
