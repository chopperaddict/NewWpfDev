using Microsoft . Win32;

using NewWpfDev.Views;

using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . IO;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Data;
using System . Windows . Documents;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Media . Animation;
using System . Windows . Media . Imaging;
using System . Windows . Navigation;
using System . Windows . Shapes;
using System . Xml . Linq;

using static System . Net . Mime . MediaTypeNames;

namespace NewWpfDev . UserControls
{
	/// <summary>
	/// Interaction logic for Ucontrol1.xaml
	/// </summary>
	public partial class Ucontrol1 : UserControl
	{
		ObservableCollection<string> Strings = new ObservableCollection<string>();
		string[] strings={
		"asdsafdafda",
		"hjhkghkg",
		"hjkjkhgkgk"};

		public Ucontrol1 ( )
		{
			InitializeComponent ( );
			this . SizeChanged += Ucontrol1_SizeChanged;
			this . DataContext = this;
			string str;
			// This points to  Drive W !!!!!!!   Who knows what is going on ???
			//			string file =$@"{Environment.SpecialFolder.MyDocuments}" + @"\library1 functions.txt";
			string file =$@"C:\users\ianch\documents\library1 functions.txt";
#pragma warning disable CS0168 // The variable 'buffer' is declared but never used
			string[] buffer;
#pragma warning restore CS0168 // The variable 'buffer' is declared but never used
			//StringBuilder sb = new StringBuilder();
#pragma warning disable CS0219 // The variable 'indx' is assigned but its value is never used
#pragma warning disable CS0219 // The variable 'offset' is assigned but its value is never used
			int indx=0, offset=0;
#pragma warning restore CS0219 // The variable 'offset' is assigned but its value is never used
#pragma warning restore CS0219 // The variable 'indx' is assigned but its value is never used
			str = File . ReadAllText ( file );
			LoadListboxWithText ( str );
			str = $"Listbox Text file viewer - {file}";
			Caption . Text = str;
		}

		private void Ucontrol1_SizeChanged ( object sender , SizeChangedEventArgs e )
		{
		}

		private void U1Ctrl_Loaded ( object sender , RoutedEventArgs e )
		{
		}

		private void Btn1_Click ( object sender , RoutedEventArgs e )
		{
			listbox1 . Items . Clear ( );
		}

		private void Btn3_Click ( object sender , RoutedEventArgs e )
		{
			listbox1 . Items . Clear ( );
			U1Ctrl_Loaded ( sender , e );
		}

		private void Btn4_Click ( object sender , RoutedEventArgs e )
		{
			string  filetoopen="", tmp="";
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog . InitialDirectory = @"C:\users\ianch\Documents";
			//openFileDialog . DefaultExt = ".txt";
			openFileDialog . Filter = "Text files (*.txt)|*.txt" +
				"| PNG (*.png)|*.png" +
				"| GIF (*. gif)|*.gif "	 +
				"| Bitmap ( *.bmp)|*.gif"  +
				"| Icon (*.ico)|*.ico" +
				"| JPEG (*.jpg)|*.jpg" +
				"| All files (*.*)|*.*";
			if ( openFileDialog . ShowDialog ( ) == true )
			{
				filetoopen = openFileDialog . FileName;
				tmp = filetoopen . ToUpper ( );
				if ( tmp . Contains ( ".BMP" )
					|| tmp . Contains ( ".JPG" )
					|| tmp . Contains ( ".GIF" )
					|| tmp . Contains ( ".PNG" )
					|| tmp . Contains ( ".ICO" ) )
				{

					Uri res= new Uri ( tmp , UriKind .RelativeOrAbsolute);
					BitmapImage bmp = new BitmapImage (  res);
					this.image1 . Source = bmp;
					listbox1 . Visibility = Visibility . Hidden;
					this.image1 . Visibility = Visibility . Visible;
					(image1 as FrameworkElement).SetValue(Canvas.TopProperty, (double)36);
					this.image1 . Height = listbox1 . Height ;
					this.image1 . Width = listbox1 . Width;
					this.image1 . UpdateLayout ( );
				}
				else
				{
					filetoopen = File . ReadAllText ( openFileDialog . FileName );
					LoadListboxWithText ( filetoopen );
					image1 . Visibility = Visibility . Hidden;
					listbox1 . Visibility = Visibility . Visible;
				}
			}
			string str = $"Listbox Text fle viewer - {openFileDialog . FileName }";
			Caption . Text = str;
		}

		private void LoadListboxWithText ( string file )
		{
			string[] buffer;
#pragma warning disable CS0219 // The variable 'line3' is assigned but its value is never used
			string line1="",line2="", line3 ="", templine="";
#pragma warning restore CS0219 // The variable 'line3' is assigned but its value is never used
			char[] ch ={ ',','.'};
			buffer = file . Split ( '\n' );
			listbox1 . Items . Clear ( );
			foreach ( var item in buffer )
			{
				if ( item . Length > 0 )
				{
					if ( item . Length > 150 )
					{
						templine = item;
						while ( true )
						{
							int commapos = templine . IndexOfAny (ch );
							if ( commapos > 0 )
							{
								line1 = templine . Substring ( 0 , commapos + 1 );
								if ( line1 [ 0 ] == '\t' )
								{
									line1 = line1 . Substring ( 1 , line1 . Length - 2 );
									listbox1 . Items . Add ( line1 );
									line2 = templine . Substring ( commapos + 1 , ( templine . Length - ( line1 . Length + 2 ) ) );
								}
								else
								{
									listbox1 . Items . Add ( line1 );
									line2 = templine . Substring ( commapos + 1 , templine . Length - ( line1 . Length ) );
								}
								commapos = line2 . IndexOfAny ( ch );
								if ( commapos > 0 )
								{
									templine = line2;
								}
								else
								{
									listbox1 . Items . Add ( line2 );
									break;
								}
							}
							else if ( commapos == -1 )
							{
								break;
							}
							else
							{
								line1 = templine . Substring ( 0 , templine . Length / 2 );
								listbox1 . Items . Add ( line1 );
								line2 = templine . Substring ( line1 . Length , ( templine . Length - line1 . Length ) );
								listbox1 . Items . Add ( line2 );

							}
						}
					}
					else
						listbox1 . Items . Add ( item . Substring ( 0 , item . Length - 1 ) );
				}
			}
		}

		private void Btn5_Click ( object sender , RoutedEventArgs e )
		{
			if ( image1 . Visibility == Visibility . Visible )
			{
				listbox1 . Visibility = Visibility . Visible;
				image1 . Visibility = Visibility . Hidden;
			}
			else
			{
				image1 . Visibility = Visibility . Visible;
				listbox1 . Visibility = Visibility . Hidden;
			}
		}
	}
}
