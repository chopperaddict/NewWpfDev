using System . Windows . Controls;

namespace NewWpfDev . UserControls
{
    /// <summary>
    /// Interaction logic for PopupListBox.xaml
    /// </summary>
    public class StyleArgs
    {
        public string style { get; set; }
        public object sender { get; set; }
        public DataGrid dgrid { get; set; }
        public int ActiveGrid { get; set; }
    }
}
