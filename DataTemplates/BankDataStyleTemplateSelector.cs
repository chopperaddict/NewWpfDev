using NewWpfDev. ViewModels;

using System;
using System . Collections . Generic;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows . Controls;
using System . Windows;

namespace NewWpfDev. DataTemplates
{
	public class BankDataStyleTemplateSelector : StyleSelector
	{
		// This needs to retun a style for specific datagrid Row type
		public override Style SelectStyle ( object item ,
			  DependencyObject container )
		{
			Style st = new Style();
			st . TargetType = typeof ( DataGridRow );
			Setter setter = new Setter();
			setter . Property = DataGridRow . StyleProperty;
			DataGrid listView = ItemsControl.ItemsControlFromItemContainer(container) as DataGrid;
			BankAccountViewModel bvm =  listView .SelectedItem as BankAccountViewModel ;
			if ( bvm . AcType == 2 )
			{
				//setter .Value = FindResource(BankAccountGridStyle)
			}
			else
			{
				//setter .Value = Brushes .Beige;
			}
			//		st .Setters .Add ( setter );
			return st;
		}
	}
}


