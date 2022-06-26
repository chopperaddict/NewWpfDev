using NewWpfDev . ViewModels;

using System;
using System . Collections . Generic;
using System . ComponentModel;
using System . Diagnostics;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Data;
using System . Windows . Documents;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Media . Imaging;
using System . Windows . Navigation;
using System . Windows . Shapes;

namespace NewWpfDev . UserControls
{
	/// <summary>
	/// Interaction logic for ApTestingControlAP.xaml
	/// </summary>
	public partial class ApTestingControlAP
	{
		#region OnPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged ( string PropertyName )
		{
			if ( null != PropertyChanged )
			{
				PropertyChanged ( this ,
					  new PropertyChangedEventArgs ( PropertyName ) );
			}
		}
		#endregion OnPropertyChanged
		public ApTestingControlAP ( )
		{
			InitializeComponent ( );
			this . DataContext = this;
			//TRANSFER			stdproperty = "12345 m67890";
		}
		public  string another = "fdshldgsjhf";

		private string _stdproperty;

		[DefaultValue ( "Hi there from Ian" )]
		public string stdproperty
		{
			get
			{
				return _stdproperty;
			}
			set
			{
				_stdproperty = value;
				OnPropertyChanged ( stdproperty . ToString ( ) );
				Debug. WriteLine ( $"APTesting : stdproperty set to  {value}" );
			}
		}

		private void Button_Click ( object sender , RoutedEventArgs e )
		{
			if ( tb3 . Text == "Hey ho, here we go !" )
				tb3 . Text = stdproperty;
			else
				tb3 . Text = "Hey ho, here we go !";
		}

		public string tempdata
		{
			get
			{
				return ( string ) GetValue ( tempdataProperty );
			}
			set
			{
				SetValue ( tempdataProperty , value );
			}
		}

		// Using a DependencyProperty as the backing store for tempdata.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty tempdataProperty =
			  DependencyProperty . Register ( "tempdata", typeof ( string ), typeof ( ApTestingControlAP ), new PropertyMetadata ( "1234567890" ) );


	}
}





