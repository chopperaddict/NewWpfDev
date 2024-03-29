﻿
using System;
using System . Collections;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Diagnostics;
using System . Threading . Tasks;
using System . Windows . Controls;
using System . Windows . Data;


using NewWpfDev. Views;

namespace NewWpfDev. ViewModels
{
	[Serializable]
	public partial class DetailsViewModel// : INotifyPropertyChanged
	{

		#region PropertyChanged

		 public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged ( string propertyName )
		{
			//PropertyChanged?.Invoke ( this, new PropertyChangedEventArgs ( propertyName ) );
			if ( Flags . SqlDetActive == false )
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
		[Conditional ( "DEBUG" )]
		[DebuggerStepThrough]
		public virtual void VerifyPropertyName ( string propertyName )
		{
			// Verify that the property name matches a real,
			// public, instance property on this object.
			if ( TypeDescriptor . GetProperties ( this ) [ propertyName ] == null )
			{
				string msg = "Invalid property name: " + propertyName;

				if ( this . ThrowOnInvalidPropertyName )
					throw new Exception ( msg );
				else
					Debug . Fail ( msg );
			}
		}

		/// <summary>
		/// Returns whether an exception is thrown, or if a Debug.Fail() is used
		/// when an invalid property name is passed to the VerifyPropertyName method.
		/// The default value is false, but subclasses used by unit tests might
		/// override this property's getter to return true.
		/// </summary>
		protected virtual bool ThrowOnInvalidPropertyName
		{
			get; private set;
		}

		#endregion PropertyChanged

		// Create a Collection that can be added to View Collection ?
		//		public static ObservableCollection<DetailsViewModel> DetailsViewObservableCollection { get; set; }
		//		public static ICollectionView DetailsCollectionView;

		#region CONSTRUCTORS
		public DetailsViewModel ( )
		{
			//			DetailsCollectionView = CollectionViewSource . GetDefaultView ( DetailsViewObservableCollection );
		}

		#endregion CONSTRUCTORS
		public static bool SqlUpdating = false;
		public static int CurrentSelectedIndex = 0;

		#region properties

		private int id;
		private string bankno;
		private string custno;
		private int actype;
		private decimal balance;
		private decimal intrate;
		private DateTime odate;
		private DateTime cdate;
		//private Internalclass integervalues;

		//public Internalclass Integervalues
		//{
		//	get { return integervalues; }
		//	set { integervalues = value; OnPropertyChanged( Integervalues . ToString()); }
		//}
		public int Id
		{
			get { return id; }
			set { id = value; OnPropertyChanged ( Id . ToString ( ) ); }
		}

		public string CustNo
		{
			get { return custno; }
			set { custno = value; OnPropertyChanged ( CustNo . ToString ( ) ); }
		}

		public string BankNo
		{
			get { return bankno; }
			set { bankno = value; OnPropertyChanged ( BankNo . ToString ( ) ); }
		}

		public int AcType
		{
			get { return actype; }
			set { actype = value; OnPropertyChanged ( AcType . ToString ( ) ); }
		}

		public decimal Balance
		{
			get { return balance; }
			set { balance = value; OnPropertyChanged ( Balance . ToString ( ) ); }
		}

		public decimal IntRate
		{
			get { return intrate; }
			set { intrate = value; OnPropertyChanged ( IntRate . ToString ( ) ); }
		}

		public DateTime ODate
		{
			get { return odate; }
			set { odate = value; OnPropertyChanged ( ODate . ToString ( ) ); }
		}

		public DateTime CDate
		{
			get { return cdate; }
			set { cdate = value; OnPropertyChanged ( CDate . ToString ( ) ); }
		}

		#endregion properties

	}
}


//**************************************************************************************************************************************************************//

/*
 *
#if USETASK
{
			try
			{
			// THIS ALL WORKS PERFECTLY - THANKS TO VIDEO BY JEREMY CLARKE OF JEREMYBYTES YOUTUBE CHANNEL
				int? taskid = Task.CurrentId;
				Task<DataTable> DataLoader = LoadSqlData ();
				DataLoader.ContinueWith
				(
					task =>
					{
						LoadDetailsObsCollection();
					},
					TaskScheduler.FromCurrentSynchronizationContext ()
				);
				Console.WriteLine ($"Completed AWAITED task to load Details Data via Sql\n" +
					$"task =Id is [{taskid}], Completed status  [{DataLoader.IsCompleted}] in {(DateTime.Now - start).Ticks} ticks]\n");
			}
			catch (Exception ex)
			{ Console.WriteLine ($"Task error {ex.Data},\n{ex.Message}"); }
			Mouse.OverrideCursor = Cursors.Arrow;
			// WE NOW HAVE OUR DATA HERE - fully loaded into Obs >?
}
#else * */


