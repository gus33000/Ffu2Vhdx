using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.Imaging
{
    public class InputPartition
	{
		public string Name { get; set; }

		public string Type { get; set; }

		public string Id { get; set; }

		public bool ReadOnly { get; set; }

		public bool AttachDriveLetter { get; set; }

		public bool Hidden { get; set; }

		public bool ServicePartition { get; set; }

		public bool Bootable { get; set; }

		public uint TotalSectors { get; set; }

		public bool UseAllSpace { get; set; }

		public string FileSystem { get; set; }

		public string PrimaryPartition
		{
			get
			{
				if (string.IsNullOrEmpty(_primaryPartition))
				{
					return Name;
				}
				return _primaryPartition;
			}
			set
			{
				_primaryPartition = value;
			}
		}

		public bool RequiredToFlash { get; set; }

		[XmlIgnore]
		public uint ByteAlignment { get; set; }

		[XmlIgnore]
		public uint ClusterSize { get; set; }

		[XmlIgnore]
		public ulong OffsetInSectors { get; set; }

		public bool PrepareFveMetadata { get; set; }

		private string _primaryPartition;
	}
}
