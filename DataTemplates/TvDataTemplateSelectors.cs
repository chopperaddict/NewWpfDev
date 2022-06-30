using System . Windows;
using System . Windows . Controls;

namespace NewWpfDev . DataTemplates
{
    class TvDataTemplateSelectors : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate (object item , DependencyObject container)
        {
            FrameworkElement elemnt = container as FrameworkElement;
            return elemnt . FindResource(item) as DataTemplate;
            return null;
        }
        public static DataTemplate SelectDataTemplate (string item , DependencyObject container)
        {
            FrameworkElement elemnt = container as FrameworkElement;
            DataTemplate dt =  elemnt?.FindResource(item) as DataTemplate;
            return dt;
        }

    }
}
