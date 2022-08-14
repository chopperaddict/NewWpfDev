using  NewWpfDev. ViewModels;
using  NewWpfDev. Views;

using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Data . SqlClient;
using System . Data;
using System . Diagnostics;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows;

namespace  NewWpfDev. ViewModels
{
	public class PubAuthors: INotifyPropertyChanged
	{
		#region OnPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged ( string PropertyName )
		{
			if ( this . PropertyChanged != null )
			{
				var e =  new PropertyChangedEventArgs ( PropertyName );
				this . PropertyChanged ( this , e );
			}
		}
		/// <summary>
		/// Warns the developer if this object does not have
		/// a public property with the specified name. This
		/// method does not exist in a Release build.
		/// </summary>
		//[Conditional ( "DEBUG" )]
		//[DebuggerStepThrough]
		//public virtual void VerifyPropertyName ( string propertyName )
		//{
		//	// Verify that the property name matches a real,
		//	// public, instance property on this object.
		//	if ( TypeDescriptor . GetProperties ( this ) [ propertyName ] == null )
		//	{
		//		string msg = "Invalid property name: " + propertyName;

		//		if ( this . ThrowOnInvalidPropertyName )
		//			throw new Exception ( msg );
		//		else
		//			Debug . Fail ( msg );
		//	}
		//}

		/// <summary>
		/// Returns whether an exception is thrown, or if a Debug.Fail() is used
		/// when an invalid property name is passed to the VerifyPropertyName method.
		/// The default value is false, but subclasses used by unit tests might
		/// override this property's getter to return true.
		/// </summary>
		//protected virtual bool ThrowOnInvalidPropertyName
		//{
		//	get; private set;
		//}

		#endregion OnPropertyChanged

		#region public table structure properties
		private  int id;
		public int Id
		{
			get { return id; }
			set { id= value; OnPropertyChanged ( id.ToString()); }
		}
		private string  fname;
		public string FName
		{
			get { return fname; }
			set { fname = value; OnPropertyChanged ( FName ); }
		}
		private string lname;
		public string LName
		{
			get { return lname; }
			set { lname = value; OnPropertyChanged ( FName ); }
		}
		private string address;
		public string Address
		{
			get { return address; }
			set { address = value; OnPropertyChanged ( Address ); }
		}
		private string city;
		public string City
		{
			get { return city; }
			set { city = value; OnPropertyChanged ( City ); }
		}
		private string state;
		public string State
		{
			get { return state; }
			set { state = value; OnPropertyChanged ( State ); }
		}
		private string zip;
		public string Zip
		{
			get { return zip; }
			set { zip = value; OnPropertyChanged ( Zip ); }
		}
		private string phone;
		public string Phone
		{
			get { return phone; }
			set { phone = value; OnPropertyChanged ( Zip ); }
		}

		#endregion public table structure properties

		private static bool Notify = false;
		static public ObservableCollection<PubAuthors> pubauthoraccts = new   ObservableCollection<PubAuthors>();
		public static DataTable dt = new DataTable();

		// Constructor
		public PubAuthors ( ) { }

		public static ObservableCollection<PubAuthors> LoadPubAuthors( ObservableCollection<PubAuthors> pubauthoraccts , bool NotifyAll = false )
		{
			Notify = NotifyAll;
			// Want to see if we ever get here
			//Debugger . Break ( );
			try
			{

				pubauthoraccts = new ObservableCollection<PubAuthors> ( );
				GetPubAuthors( dt);
				pubauthoraccts = LoadAuthors(pubauthoraccts ,  dt);
				return ( pubauthoraccts );
			} catch ( Exception ex )
			{
				Debug. WriteLine ( $"Details  Load Exception : {ex . Message}, {ex . Data}" );
				WpfLib1.Utils . DoErrorBeep ( 290 , 100 , 1 );
				return null;
			}
		}
		public static DataTable GetPubAuthors ( DataTable dt )
		{
			try
			{
				SqlConnection con;
				string commandline = "";
				//string ConString = Flags . CurrentConnectionString;
				string ConString = ( string )NewWpfDev. Properties . Settings . Default [ "BankSysConnectionString" ];
				con = new SqlConnection ( ConString );
				using ( con )
				{
					commandline = "Select * from Authors order by au_LName";
					SqlCommand cmd = new SqlCommand ( commandline, con );
					SqlDataAdapter sda = new SqlDataAdapter ( cmd );
					sda . Fill ( dt );
				}
			} catch ( Exception ex )
			{
				Debug . WriteLine ( $"AWorks : ERROR in GetAsPersons(): Failed to load Data in DataTable - {ex . Message}, {ex . Data}" );
				MessageBox . Show ( $"AWorks: ERROR in GetAsPersons(): Failed to load DataTable- {ex . Message}, {ex . Data}" );
				return ( DataTable ) null;				
			}
			return dt;
		}

		public static ObservableCollection<PubAuthors> LoadAuthors( ObservableCollection<PubAuthors> authors, DataTable dt)
		{
			int count = 0;
			try
			{
				for ( int i = 0 ; i < dt. Rows . Count ; i++ )
				{
					authors . Add ( new PubAuthors
					{
						FName = dt . Rows [ i ] [ 0 ] . ToString ( ) ,
						LName = dt . Rows [ i ] [ 1 ] . ToString ( ) ,
						City = dt . Rows [ i ] [ 2 ] . ToString ( ) ,
						State = dt . Rows [ i ] [ 3 ] . ToString ( ) ,
						Zip = dt . Rows [ i ] [ 4 ] . ToString ( ),
						Phone = dt . Rows [ i ] [ 5 ] . ToString ( )
					} );
					count = i;
				}
				Debug . WriteLine ( $"DETAILS : Sql data loaded into Details ObservableCollection \"Detinternalcollection\" [{count}] ...." );
			} catch ( Exception ex )
			{
				Debug . WriteLine ( $"DETAILS : ERROR in  LoadDetCollection() : loading Details into ObservableCollection \"DetCollection\" : [{ex . Message}] : {ex . Data} ...." );
				MessageBox . Show ( $"DETAILS : ERROR in  LoadDetCollection() : loading Details into ObservableCollection \"DetCollection\" : [{ex . Message}] : {ex . Data} ...." );
				return null;
			} finally
			{
				if ( Notify )
				{
					EventControl . TriggerDetDataLoaded ( null ,
						new LoadedEventArgs
						{
							CallerType = "AUTHORSSERVER" ,
							CallerDb = "AUTHORS" ,
							DataSource = authors,
							RowCount = authors . Count
						} );
				}
			}
			return authors;
		}


	}
}
