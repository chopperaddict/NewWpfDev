namespace NewWpfDev . ViewModels
{
    public class SqlTable : TreeViewItemBase
	{
		public SqlTable ( string TableName )
		{
			Tablename = TableName;
			NotifyPropertyChanged ( "Tablename" );
		}
		public string Tablename { get; set; }
	}
}
