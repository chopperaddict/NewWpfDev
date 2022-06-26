using NewWpfDev. ViewModels;

namespace NewWpfDev. Views
{
	public partial class DragDropClient
	{
		public class CustomerDragviewModel : CustomerViewModel
		{
			public string RecordType
			{
				get; set;
			}
			public override string ToString ( )
			{
				return base . ToString ( );
			}
			public CustomerDragviewModel ( )
			{
 
            }
        }
	}
}


