using System . Collections . Generic;
using System . Collections . ObjectModel;

namespace NewWpfDev . ViewModels
{
    public class Database  : TreeViewItemBase
	{
		public Database ( )
		{ }
		public Database ( string dbname )
		{
			Databasename = dbname;
		}
		public string Databasename { get; set; }
		public List<SqlTable> Tables { get; set; }
//		public List<SqlProcedures> Procedures{ get; set; }
		public Database LoadData ( ObservableCollection<Database >sqldb )
		{
			Database db= new  Database ( "Ian1" );
			db . Tables = new List<SqlTable> ( );
			db . Tables . Add ( new SqlTable ( "BankAccount" ) );
			db . Tables . Add ( new SqlTable ( "SecAccounts" ) );
			db . Tables . Add ( new SqlTable ( "NewBank" ) );
			db . Tables . Add ( new SqlTable ( "Customer" ) );
			sqldb. Add (db);
			db= new  Database ( "Publishers" );
			db . Tables = new List<SqlTable> ( );
			db . Tables . Add ( new SqlTable ( "Orders" ) );
			db. Tables . Add ( new SqlTable ( "Titles" ) );
			db . Tables . Add ( new SqlTable ( "Publisher" ) );
			sqldb . Add ( db );
			return db;
		}
	}
}
