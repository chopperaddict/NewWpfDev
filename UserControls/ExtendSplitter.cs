using System . Windows;
using System . Windows . Controls;
using System . Windows . Controls . Primitives;
//using NewWpfDev . ViewModels;
public class ExtendSplitter : GridSplitter
{
    NewWpfDev . UserControls . GenericGridControl GGC;
    public ExtendSplitter ( NewWpfDev . UserControls . GenericGridControl gcc )
    {
        GGC = gcc;
        EventManager . RegisterClassHandler ( typeof ( ExtendSplitter ) , Thumb . DragDeltaEvent , new DragDeltaEventHandler ( OnDragDelta ) );
    }

    public void OnDragDelta ( object sender , DragDeltaEventArgs e )
    {
        if ( GGC . SplitterGrid . RowDefinitions [ 1 ] . ActualHeight < 100 )
            e . Handled = true;
        else e . Handled = false;
    }

}