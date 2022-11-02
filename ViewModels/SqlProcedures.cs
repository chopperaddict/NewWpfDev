using System . Collections . Generic;

namespace NewWpfDev . ViewModels
{
    public class SqlProcedures : TreeViewItemBase
	{
        public List<SqlProcedures> Procedures { get; set; }

        public SqlProcedures ( )
        {

        }
		public SqlProcedures ( string DbName )
		{
			Procname = DbName;
			NotifyPropertyChanged ( "DbName" );
		}
		public string Procname { get; set; }
	}
}
