using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . IO . Packaging;
using System . Linq;
using System . Text;
using System . Threading . Tasks;

namespace NewWpfDev.ViewModels
{
	public class SqlDatabases 
	{
		    public SqlDatabases ()
		{
			var collection = new ObservableCollection<Database>();
		}
	}
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
		public List<SqlProcedures> Procedures{ get; set; }
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
	public class SqlTable : TreeViewItemBase
	{
		public SqlTable ( string TableName )
		{
			Tablename = TableName;
			NotifyPropertyChanged ( "Tablename" );
		}
		public string Tablename { get; set; }
	}
	public class SqlProcedures : TreeViewItemBase
	{
		public SqlProcedures ( string DbName )
		{
			Procname = DbName;
			NotifyPropertyChanged ( "DbName" );
		}
		public string Procname { get; set; }
	}

	public class TreeViewItemBase : INotifyPropertyChanged
	{
		private bool isSelected;
		public bool IsSelected
		{
			get { return this . isSelected; }
			set
			{
				if ( value != this . isSelected )
				{
					this . isSelected = value;
					NotifyPropertyChanged ( "IsSelected" );
				}
			}
		}

		private bool isExpanded;
		public bool IsExpanded
		{
			get { return this . isExpanded; }
			set
			{
				if ( value != this . isExpanded )
				{
					this . isExpanded = value;
					NotifyPropertyChanged ( "IsExpanded" );
				}
			}
		}


		public event PropertyChangedEventHandler PropertyChanged;

		public void NotifyPropertyChanged ( string propName )
		{
			if ( this . PropertyChanged != null )
				this . PropertyChanged ( this , new PropertyChangedEventArgs ( propName ) );
		}
	}
}
