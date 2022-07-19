using Dapper;
using NewWpfDev. Dapper;
using NewWpfDev. ViewModels;

using System;
using System . Collections;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Data . SqlClient;
using System . Data;
using System . Diagnostics;
using System . IO;
using System . Linq;
using System . Net;
using System . Runtime . Serialization . Formatters . Binary;
using System . Text;
using System . Threading . Tasks;
using System . Windows . Controls;
using System . Windows . Input;
using System . Windows . Media . Imaging;
using System . Windows . Media;
using System . Windows;
using System . Xml;

namespace NewWpfDev. Sql
{

	public partial class SqlServerCommands : Window
	{
		ObservableCollection<GenericClass> Generics= new ObservableCollection<GenericClass>();
		//********************************************************************************************************************************************************************************//
		public ObservableCollection<GenericClass> CreateGenericDatabase ( DataGrid dgrid , List<string> ReceivedDbData , bool LoadGrid = true )
		//********************************************************************************************************************************************************************************//
		{
			string datain="";
			int totalfields = 0;
			// Post process data string received 
			//			ObservableCollection<GenericClass> genericcollection = new ObservableCollection<GenericClass>();
			Generics . Clear ( );
			for ( int x = 0 ; x < ReceivedDbData . Count ; x++ )
			{
				datain = ReceivedDbData [ x ];
				string[] fields = datain.Split(',');
				totalfields = fields . Length;
				GenericClass genclass = new GenericClass();
				for ( int z = 0 ; z < fields . Length ; z++ )
				{
					string[] inner = fields[z].Split('=');
					try
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
					{
						switch ( z + 1 )
						{
							case 1:
								genclass . field1 = inner [ 1 ];
								break;
							case 2:
								genclass . field2 = inner [ 1 ];
								break;
							case 3:
								genclass . field3 = inner [ 1 ];
								break;
							case 4:
								genclass . field4 = inner [ 1 ];
								break;
							case 5:
								genclass . field5 = inner [ 1 ];
								break;
							case 6:
								genclass . field6 = inner [ 1 ];
								break;
							case 7:
								genclass . field7 = inner [ 1 ];
								break;
							case 8:
								genclass . field8 = inner [ 1 ];
								break;
							case 9:
								genclass . field9 = inner [ 1 ];
								break;
							case 10:
								genclass . field10 = inner [ 1 ];
								break;
							case 11:
								genclass . field11 = inner [ 1 ];
								break;
							case 12:
								genclass . field12 = inner [ 1 ];
								break;
							case 13:
								genclass . field13 = inner [ 1 ];
								break;
							case 14:
								genclass . field14 = inner [ 1 ];
								break;
							case 15:
								genclass . field15 = inner [ 1 ];
								break;
							case 16:
								genclass . field16 = inner [ 1 ];
								break;
							case 17:
								genclass . field17 = inner [ 1 ];
								break;
							case 18:
								genclass . field18 = inner [ 1 ];
								break;
							case 19:
								genclass . field19 = inner [ 1 ];
								break;
							case 20:
								genclass . field20 = inner [ 1 ];
								break;
						}
					} catch ( Exception ex ) { }
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
				}
				Generics . Add ( genclass );
			}
			if ( LoadGrid == true )
			{
				LoadActiveRowsOnlyInGrid ( dgrid , Generics , totalfields );
//				UpdateUniversalViewer ( );
			}
			return Generics;
		}
		
		#region generic data formatting methods

		//********************************************************************************************************************************************************************************//
		// takes al 20 rows of data from GenericClass records and  then loads a datagrid with
		// //only the columns actualy used, dropping the unsed columns from the datagrid
		public static void LoadActiveRowsOnlyInGrid ( DataGrid Grid , ObservableCollection<GenericClass> genericcollection , int total )
		//********************************************************************************************************************************************************************************//
		{
			// filter data to remove all "extraneous" columns
			Grid . ItemsSource = null;
			Grid . Items . Clear ( );
			if ( total == 1 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1}).ToList();
				Grid . ItemsSource = res;
			}
			else if ( total == 2 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1,data.field2}).ToList();
				Grid . ItemsSource = res;
			}
			else if ( total == 3 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2,data.field3}).ToList();
				Grid . ItemsSource = res;
			}
			else if ( total == 4 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4}).ToList();
				Grid . ItemsSource = res;
			}
			else if ( total == 5 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5}).ToList();
				Grid . ItemsSource = res;
			}
			else if ( total == 6 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6}).ToList();
				Grid . ItemsSource = res;
			}
			else if ( total == 7 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7}).ToList();
				Grid . ItemsSource = res;
			}
			else if ( total == 8 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8}).ToList();
				Grid . ItemsSource = res;
			}
			else if ( total == 9 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9}).ToList();
				Grid . ItemsSource = res;
			}
			else if ( total == 10 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9 ,data.field10}).ToList();
				Grid . ItemsSource = res;
			}
			else if ( total == 11 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9,data.field10,data.field11}).ToList();
				Grid . ItemsSource = res;
			}
			else if ( total == 12 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9,data.field10,data.field11,data.field12}).ToList();
				Grid . ItemsSource = res;
			}
			else if ( total == 13 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9,data.field10,data.field11,data.field12,data.field13}).ToList();
				Grid . ItemsSource = res;
			}
			else if ( total == 14 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9,data.field10,data.field11,data.field12,data.field13,data.field14}).ToList();
				Grid . ItemsSource = res;
			}
			else if ( total == 15 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9,data.field10,data.field11,data.field12,data.field13,data.field14,data.field15}).ToList();
				Grid . ItemsSource = res;
			}
			else if ( total == 16 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9,data.field10,data.field11,data.field12,data.field13,data.field14,data.field15,data.field16}).ToList();
				Grid . ItemsSource = res;
			}
			else if ( total == 17 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9,data.field10,data.field11,data.field12,data.field13,data.field14,data.field15,data.field16,data.field17}).ToList();
				Grid . ItemsSource = res;
			}
			else if ( total == 18 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9,data.field10,data.field11,data.field12,data.field13,data.field14,data.field15,data.field16,data.field17,data.field18}).ToList();
				Grid . ItemsSource = res;
			}
			else if ( total == 19 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9,data.field10,data.field11,data.field12,data.field13,data.field14,data.field15,data.field16,data.field17,data.field18,data.field19}).ToList();
				Grid . ItemsSource = res;
			}
			else if ( total == 20 )
			{
				var res =
				   ( from data in genericcollection
				     select new
				     {data.field1, data.field2, data.field3,data.field4,data.field5,data.field6,data.field7,data.field8,data.field9,data.field10,data.field11,data.field12,data.field13,data.field14,data.field15,data.field16,data.field17,data.field18,data.field19,data.field20}).ToList();
				Grid . ItemsSource = res;
			}
			Grid . SelectedIndex = 0;
			Grid . Visibility = Visibility . Visible;
//			GridCount . Text = Grid . Items . Count . ToString ( );
			Grid . Refresh ( );
			Grid . Focus ( );
		}
		// support  the getgrenrows above by cunting the active rows
		public static int GetGenericColumnCount ( ObservableCollection<GenericClass> collection , GenericClass gcc = null )
		{
			GenericClass gc = new GenericClass();
			try
			{
				if ( gcc == null )
					gc = collection [ 0 ] as GenericClass;
				else
					gc = gcc;
                if ( gc.field1  == null )
                    return 0;
				if ( gc . field20 != null  && gc.field20 != "")
				{ return 20; }
				else if ( gc . field19 != null && gc . field19 != "" )
				{ return 19; }
				else if ( gc . field18 != null && gc . field18 != "" )
				{ return 18; }
				else if ( gc . field17 != null && gc . field17 != "" )
				{ return 17; }
				else if ( gc . field16 != null && gc . field16 != "" )
				{ return 16; }
				else if ( gc . field15 != null   && gc.field15 != "")
				{ return 15; }
				else if ( gc . field14 != null   && gc.field14 != "")
				{ return 14; }
				else if ( gc . field13 != null   && gc.field13 != "")
				{ return 13; }
				else if ( gc . field12 != null   && gc.field12 != "")
				{ return 12; }
				else if ( gc . field11 != null   && gc.field11 != "")
				{ return 11; }
				else if ( gc . field10 != null   && gc.field10 != "")
				{ return 10; }
				else if ( gc . field9 != null   && gc.field9 != "")
				{ return 9; }
				else if ( gc . field8 != null   && gc.field8 != "")
				{ return 8; }
				else if ( gc . field7 != null   && gc.field7!= "")
				{ return 7; }
				else if ( gc . field6 != null   && gc.field6!= "")
				{ return 6; }
				else if ( gc . field5 != null   && gc.field5 != "")
				{ return 5; }
				else if ( gc . field4 != null   && gc.field4 != "")
				{ return 4; }
				else if ( gc . field3 != null   && gc.field3 != "")
				{ return 3; }
				else if ( gc . field2 != null   && gc.field2 != "")
				{ return 2; }
				else if ( gc . field1 != null   && gc.field1 != "")
				{ return 1; }
				return 0;
			} catch ( Exception ex )
			{
				Debug. WriteLine ( $"Column count error '{ex . Message}'" );
			}
			return 0;
		}
		// support  the getgrenrows above by using the column by colunn dictionaries of the hetgenrows  method
		public static void AddDictPairToGeneric<T> ( T gc , KeyValuePair<string , object> dict , int dictcount ) where T : GenericClass
		{
			switch ( dictcount )
			{
				case 1:
					gc . field1 = dict . Value . ToString ( );
					break;
				case 2:
					gc . field2 = dict . Value . ToString ( );
					break;
				case 3:
					gc . field3 = dict . Value . ToString ( );
					break;
				case 4:
					gc . field4 = dict . Value . ToString ( );
					break;
				case 5:
					gc . field5 = dict . Value . ToString ( );
					break;
				case 6:
					gc . field6 = dict . Value . ToString ( );
					break;
				case 7:
					gc . field7 = dict . Value . ToString ( );
					break;
				case 8:
					gc . field8 = dict . Value . ToString ( );
					break;
				case 9:
					gc . field9 = dict . Value . ToString ( );
					break;
				case 10:
					gc . field10 = dict . Value . ToString ( );
					break;
				case 11:
					gc . field10 = dict . Value . ToString ( );
					break;
				case 12:
					gc . field12 = dict . Value . ToString ( );
					break;
				case 13:
					gc . field13 = dict . Value . ToString ( );
					break;
				case 14:
					gc . field14 = dict . Value . ToString ( );
					break;
				case 15:
					gc . field15 = dict . Value . ToString ( );
					break;
				case 16:
					gc . field16 = dict . Value . ToString ( );
					break;
				case 17:
					gc . field17 = dict . Value . ToString ( );
					break;
				case 18:
					gc . field18 = dict . Value . ToString ( );
					break;
				case 19:
					gc . field19 = dict . Value . ToString ( );
					break;
				case 20:
					gc . field20 = dict . Value . ToString ( );
					break;
			}
		}

		#endregion generic data formatting methods
		public static ObservableCollection<GenericClass> ParseDapperRow ( ObservableCollection<GenericClass>buff, Dictionary<string , object> dict , out int colcount )
		{
			//GenericClass GenRow = new GenericClass();
			//int index=0;
			colcount = 0;
			//foreach ( var item in dict )
			//{
			//	try
			//	{
			//		if ( item . Key == "" || item . Value == null )
			//			break;
			//		dict . Add ( item . Key , item . Value );
			//		index++;
			//	} catch ( Exception ex )
			//	{
			//		MessageBox . Show ( $"ParseDapper error was : \n{ex . Message}\nKey={item . Key} Value={item . Value . ToString ( )}" );
			//		break;
			//	}
			//}
			//colcount = index;
			return buff;
		}
		public static string GetStringFromGenericRow ( GenericClass GenRow )
		{
			// Create a string containg data from ALL non null fields  in a GenericClass record
			string output="";
			for ( int i = 0 ; i < 20 ; i++ )
			{
				if ( GenRow . field1 != "" )
					output += GenRow . field1 . Trim ( );
				if ( GenRow . field2 != "" )
					output += GenRow . field2 . Trim ( ) + ",";
				if ( GenRow . field3 != "" )
					output += GenRow . field3 . Trim ( ) + ",";
				if ( GenRow . field4 != "" )
					output += GenRow . field4 . Trim ( ) + ",";
				if ( GenRow . field5 != "" )
					output += GenRow . field5 . Trim ( ) + ",";
				if ( GenRow . field6 != "" )
					output += GenRow . field6 . Trim ( ) + ",";
				if ( GenRow . field7 != "" )
					output += GenRow . field7 . Trim ( ) + ",";
				if ( GenRow . field8 != "" )
					output += GenRow . field8 . Trim ( ) + ",";
				if ( GenRow . field9 != "" )
					output += GenRow . field9 . Trim ( ) + ",";
				if ( GenRow . field10 != "" )
					output += GenRow . field10 . Trim ( ) + ",";
				if ( GenRow . field11 != "" )
					output += GenRow . field11 . Trim ( ) + ",";
				if ( GenRow . field12 != "" )
					output += GenRow . field12 . Trim ( ) + ",";
				if ( GenRow . field13 != "" )
					output += GenRow . field13 . Trim ( ) + ",";
				if ( GenRow . field14 != "" )
					output += GenRow . field14 . Trim ( ) + ",";
				if ( GenRow . field15 != "" )
					output += GenRow . field15 . Trim ( ) + ",";
				if ( GenRow . field16 != "" )
					output += GenRow . field16 . Trim ( ) + ",";
				if ( GenRow . field17 != "" )
					output += GenRow . field17 . Trim ( ) + ",";
				if ( GenRow . field18 != "" )
					output += GenRow . field18 . Trim ( ) + ",";
				if ( GenRow . field19 != "" )
					output += GenRow . field19 . Trim ( ) + ",";
				if ( GenRow . field20 != "" )
					output += GenRow . field20 . Trim ( ) + ",";
			}
			output = output . Substring ( 0 , output . Length - 1 );
			return output;
		}
		
		public static GenericClass SaveToField ( GenericClass GenRow , int index , string outstr )
		{
			switch ( index )
			{
				case 0:
					GenRow . field1 = outstr;
					break;
				case 1:
					GenRow . field2 = outstr;
					break;
				case 2:
					GenRow . field3 = outstr;
					break;
				case 3:
					GenRow . field4 = outstr;
					break;
				case 4:
					GenRow . field5 = outstr;
					break;
				case 5:
					GenRow . field6 = outstr;
					break;
				case 6:
					GenRow . field7 = outstr;
					break;
				case 7:
					GenRow . field8 = outstr;
					break;
				case 8:
					GenRow . field9 = outstr;
					break;
				case 9:
					GenRow . field10 = outstr;
					break;
				case 10:
					GenRow . field11 = outstr;
					break;
				case 11:
					GenRow . field12 = outstr;
					break;
				case 12:
					GenRow . field13 = outstr;
					break;
				case 13:
					GenRow . field14 = outstr;
					break;
				case 14:
					GenRow . field15 = outstr;
					break;
				case 15:
					GenRow . field16 = outstr;
					break;
				case 16:
					GenRow . field17 = outstr;
					break;
				case 17:
					GenRow . field18 = outstr;
					break;
				case 18:
					GenRow . field19 = outstr;
					break;
				case 19:
					GenRow . field20 = outstr;
					break;
			}
			return GenRow;
		}
	}
}



