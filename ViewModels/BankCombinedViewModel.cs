using NewWpfDev. Views;

using System;
using System . Collections . Generic;
using System . ComponentModel;
using System . Linq;
using System . Text;
using System . Threading . Tasks;

namespace NewWpfDev. ViewModels
{
	// combination of BankAccount with personal Customer details added
	public class BankCombinedViewModel : BankAccountViewModel 
	{

		#region PropertyChanged

		new public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged ( string propertyName )
		{
			if ( Flags . SqlBankActive == false )
				//				this . VerifyPropertyName ( propertyName );

				if ( this . PropertyChanged != null )
				{
					var e = new PropertyChangedEventArgs ( propertyName );
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

		#endregion PropertyChanged
		public BankCombinedViewModel ( ) { }

		private string lName;
		public string LName
		{
			get { return lName; }
			set { lName = value; OnPropertyChanged ( LName . ToString ( ) ); }
		}
		private string fName;
		public string FName
		{
			get { return fName; }
			set { fName = value; OnPropertyChanged ( FName . ToString ( ) ); }
		}
		private string addr1;
		public string Addr1
		{
			get { return addr1; }
			set { addr1 = value; OnPropertyChanged ( Addr1 . ToString ( ) ); }
		}
		private string addr2;
		public string Addr2
		{
			get { return addr2; }
			set { addr2 = value; OnPropertyChanged ( Addr2 . ToString ( ) ); }
		}
		private string town;
		public string Town
		{
			get { return town; }
			set { town = value; OnPropertyChanged ( Town . ToString ( ) ); }
		}
		private string county;
		public string County
		{
			get { return county; }
			set { county = value; OnPropertyChanged ( County . ToString ( ) ); }
		}
		private string pcode;
		public string PCode
		{
			get { return pcode; }
			set { pcode = value; OnPropertyChanged ( PCode . ToString ( ) ); }
		}
		private string phone;
		public string Phone
		{
			get { return phone; }
			set { phone = value; OnPropertyChanged ( Phone . ToString ( ) ); }
		}

	}
}


