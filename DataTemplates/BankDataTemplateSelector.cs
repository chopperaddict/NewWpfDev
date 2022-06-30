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
	public class BankDataTemplateSelector : DataTemplateSelector
	{
		public override DataTemplate SelectTemplate ( object item , DependencyObject container )
		{
			FrameworkElement element = container as FrameworkElement;

            // Task is the particular value to let you select the requiired Template element 
            if ( element != null && item != null && item is BankAccountViewModel )
            {
                BankAccountViewModel dg = item as BankAccountViewModel;

                if ( dg . AcType == 2 )
                    return element . FindResource("BankDataTemplate1") as DataTemplate;
                else
                    return element . FindResource("BankDataTemplate2") as DataTemplate;
            }
            else if ( element != null && item != null && item is CustomerViewModel )
            {
                CustomerViewModel dg = item as CustomerViewModel;

                if ( dg . AcType == 2 )
                    return element . FindResource("CustomersDbTemplate1") as DataTemplate;
                else
                    return element . FindResource("CustomersDbTemplate2") as DataTemplate;
            }
            else if ( element != null && item != null && item is GenericClass )
            {
                CustomerViewModel dg = item as CustomerViewModel;

                if ( dg . AcType == 2 )
                    return element . FindResource("GenericTemplate") as DataTemplate;
                else
                    return element . FindResource("GenericTemplate1") as DataTemplate;
            }
            
            return null;
		}
	}
}


