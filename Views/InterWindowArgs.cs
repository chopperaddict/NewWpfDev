using System;
using System . Windows;
using System . Windows . Controls;

public class InterWindowArgs : EventArgs
{
    public Window window;
    public UIElement uielement;
    public ListBox listbox;
    public object data;
    public string message;
    public int intvalue;
    public long longvalue;
    public double dblvalue;
    public float floatvalue;
    public decimal  decimalvalue;
}