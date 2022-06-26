using System . Collections . ObjectModel;

namespace NewWpfDev . Views
{
	public class Family : FamilyMember
	{
		public Family ( )
		{
			this . Members = new ObservableCollection<FamilyMember> ( );
		}

//		public string Name { get; set; }

		public ObservableCollection<FamilyMember> Members { get; set; }
	}
	public class FamilyMember
	{
		public string Name { get; set; }

		public int Age { get; set; }
		public string Gender { get; set; }
		public bool Employed { get; set; }

	}

}
