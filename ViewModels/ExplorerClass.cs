using System;
using System . Collections . Generic;
using System . IO;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows . Documents;
using System . Windows . Shapes;

namespace NewWpfDev . ViewModels
{
	public class ExplorerClass
	{
		private static ExplorerClass instance = null;
		private static readonly object padlock = new object();


		private DirectoryInfo _directory;
		private List<DirectoryInfo> _directories;
		private List<FileInfo> _files;
#pragma warning disable CS0169 // The field 'ExplorerClass._drives' is never used
		private List<string> _drives;
#pragma warning restore CS0169 // The field 'ExplorerClass._drives' is never used
		public ExplorerClass ( ) { }
		public static ExplorerClass Instance
		{
			get
			{
				lock ( padlock )
				{
					if ( instance == null )
					{
						instance = new ExplorerClass ( );
						instance . FullPath = @"C:\\";
					}
					return Instance;
				}
			}
		}

		#region declarations
		public string CurrentDrive {get; set;}

		public string Name
		{
			get { return _directory? . Name; }
			set {  }
		}
		public string FullPath
		{
			get
			{
				return _directory . FullName;
			}
			set
			{
				if ( Directory . Exists ( value ))
				{
					_directory = new DirectoryInfo ( value );
					CurrentDrive = value . ToString ( );
				}
				else
				{
				}
			}
		}

		public List<string> Drives
		{
			get
			{
				List<string > _drives = new List<string>();
				DriveInfo [] drives = DriveInfo.GetDrives();
				drives = DriveInfo . GetDrives ( );
				for ( int drive = 0 ; drive < drives . Length ; drive++ )
					_drives . Add ( drives [ drive ] . ToString ( ) );
				return _drives;
			}
		}
		public List<DirectoryInfo> Directories
		{
			get
			{
				_directories = new List<DirectoryInfo> ( );
				if ( _directory == null )
					_directory = new DirectoryInfo (@"C:\\" );
				DirectoryInfo[] di = _directory.GetDirectories();
				// Load  all folders in drive
				for ( int i = 0 ; i < di . Length ; i++ )
				{
					DirectoryInfo newFolder = di[i];
					_directories . Add ( newFolder );

				}
				return _directories;
			}
		}

		//public List<ExplorerClass> Folders
		//{
		//	get
		//	{
		//		if ( _Folders == null )
		//		{
		//			_Folders = new List<ExplorerClass> ( );
		//			DirectoryInfo[] di = this._directory.GetDirectories();
		//			for ( int i = 0 ; i < di . Length ; i++ )
		//			{
		//				ExplorerClass newFolder = new ExplorerClass();
		//				newFolder . FullPath = di [ i ] . FullName;
		//				_Folders . Add ( newFolder );

		//			}
		//		}
		//		return _Folders;
		//	}
		//}
		public List<FileInfo> Files
		{
			get
			{
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
				try
				{
					_files = new List<FileInfo> ( );
					FileInfo [ ] fi = this . _directory? . GetFiles ( );
					for ( int i = 0 ; i < fi ?. Length ; i++ )
					{

						FileAttributes fa = fi [ i ] . Attributes;
						string s = fa . ToString ( );
						if (
								( s . Contains ( "Hidden" )
								|| s . ToUpper ( ) . Contains ( "BOOTMGR" )
								|| s . ToUpper ( ) . Contains ( "BOOTNXT" )
								|| s . ToUpper ( ) . Contains ( "BOOTSECT" ) ) == false )
							_files . Add ( fi [ i ] );
					}
				}
				catch (Exception ex)
				{; }
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
				//}
				return _files;
			}
		}
		#endregion declarations

		public List<string> GetDrives ( string path = "" )
		{
			FullPath = path;
			return Drives;
		}
		public List<DirectoryInfo> GetDirectories ( string newpath = "" )
		{
			//FullPath =  newpath;
			//Name = newpath;
			return Directories;
		}

		//public List<Folder> GetFolders ( )
		//{
		//	return Folders;
		//}
		public List<FileInfo> GetFiles ( string path = "" )
		{
			//			FullPath =  path;
			return Files;

		}
	}
}
