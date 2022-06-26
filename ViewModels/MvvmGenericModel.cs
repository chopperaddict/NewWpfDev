using NewWpfDev . Views;

using System;
using System . Collections . Generic;
using System . ComponentModel;
using System . Linq;
using System . Security . Policy;
using System . Text;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Input;

namespace NewWpfDev . ViewModels
{
	/// <summary>
	/// Viewmodel for one section in the GENERICMVVM. window
	/// </summary>
	internal class MvvmGenericModel 
	{

		//..Viewmodel for just one (of four) grid sections in the GENERICMVVM.window
		#region NotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged ( string propertyName )
		{
			if ( PropertyChanged != null )
			{
				PropertyChanged ( this , new PropertyChangedEventArgs ( propertyName ) );
			}
		}
		#endregion NotifyPropertyChanged
		#region  Class Properties
		private string name;
		private string address1;
		private string address2;
		private string address3;
		public string Name
		{
			get { return name; }
			set { name = value; NotifyPropertyChanged ( Name ); }
		}
		public string Address1
		{
			get { return address1; }
			set { address1 = value; NotifyPropertyChanged ( Address1 ); }
		}
		public string Address2
		{
			get { return address2; }
			set { address2 = value; NotifyPropertyChanged ( Address2 ); }
		}
		public string Address3
		{
			get { return address3; }
			set { address3 = value; NotifyPropertyChanged ( Address3 ); }
		}

		public ICommand UpdateName
		{ get; set; }
		public ICommand UpdateAddr1
		{ get; set; }
		public ICommand UpdateAddr2
		{ get; set; }
		public ICommand CloseWindow
		{ get; set; }

		public MvvmGenericModel ( )
		{
			UpdateName = new RelayCommand ( ExecuteUpdateName , CanExecuteUpdateName );
			UpdateAddr1 = new RelayCommand ( ExecuteUpdateAddr1 , CanExecuteUpdateAddr1 );
			UpdateAddr2 = new RelayCommand ( ExecuteUpdateAddr2 , CanExecuteUpdateAddr2 );
			Name = "Ian";
			Address1 = "38 Liggard Court";
			Address2 = "Mythop Road";
			Address3 = "Lytham St Annes";

		}
		#endregion  Class Properties

		#region ICommand Can Execute

		public bool CanExecuteUpdateName ( object arg )
		{ return true; }
		public bool CanExecuteUpdateAddr1 ( object arg )
		{ return true; }
		public bool CanExecuteUpdateAddr2 ( object arg )
		{ return true; }

		#endregion ICommand Can Execute

		#region  ICommand Execute Methods
		public void ExecuteUpdateName ( object obj )
		{ Name = obj . ToString ( ); }
		public void ExecuteUpdateAddr1 ( object obj )
		{ Address1 = obj . ToString ( ); }
		public void ExecuteUpdateAddr2 ( object obj )
		{ Address2 = obj . ToString ( ); }

		#endregion ICommand CanExecute Methods

	}
}
