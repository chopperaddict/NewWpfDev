using System;
using System . Collections . Generic;
using System . Text;

namespace NewWpfDev . Views
{
    public interface IStoredProcsString
    {
        void SelectSqlPProcessor ( );

        public dynamic ProcessGenericDapperStoredProcedure (
            string spCommand ,
            string [ ] args ,
            string CurrDomain ,
            ref string ResultString ,
            ref object Obj ,
            ref Type Objtype ,
            ref int Count ,
            ref string Error );
    }
}
