namespace Microsoft.WindowsPhone.Imaging
{
    public class FullFlashUpdatePartition
	{
		public FullFlashUpdatePartition()
		{
		}

		public FullFlashUpdatePartition(FullFlashUpdateStore A_1, InputPartition A_2) : this()
		{
			Store = A_1;
			SectorsInUse = 0U;
			TotalSectors = A_2.TotalSectors;
			PartitionType = A_2.Type;
			PartitionId = A_2.Id;
			Name = A_2.Name;
			UseAllSpace = A_2.UseAllSpace;
			FileSystem = A_2.FileSystem;
			Bootable = A_2.Bootable;
			ReadOnly = A_2.ReadOnly;
			Hidden = A_2.Hidden;
			AttachDriveLetter = A_2.AttachDriveLetter;
			ServicePartition = A_2.ServicePartition;
			PrimaryPartition = A_2.PrimaryPartition;
			RequiredToFlash = A_2.RequiredToFlash;
			ByteAlignment = A_2.ByteAlignment;
			ClusterSize = A_2.ClusterSize;
			OffsetInSectors = A_2.OffsetInSectors;
			PrepareFveMetadata = A_2.PrepareFveMetadata;
		}

		public override string ToString()
		{
			return Name;
		}

		private FullFlashUpdateStore Store
		{
            get; set;
        }

		public string Name { get; set; }

		public uint TotalSectors { get; set; }

		public string PartitionType
		{
            get; set;
        }

		public string PartitionId
		{
            get; set;
        }

		public bool Bootable
		{
            get; set;
        }

		public bool ReadOnly
		{
            get; set;
        }

		public bool Hidden
		{
            get; set;
        }

		public bool AttachDriveLetter
		{
            get; set;
        }

		public bool ServicePartition
		{
            get; set;
        }

		public string PrimaryPartition
		{
            get; set;
        }

		public string FileSystem
		{
            get; set;
        }

		public uint ByteAlignment
		{
            get; set;
        }

		public uint ClusterSize
		{
            get; set;
        }

		public uint SectorsInUse { get; set; }

		public bool UseAllSpace { get; set; }

		public bool RequiredToFlash
		{
            get; set;
        }

		public uint SectorAlignment
		{
            get; set;
        }

		public ulong OffsetInSectors
		{
            get; set;
        }

		public bool PrepareFveMetadata
		{
			get; set;
		}
	}
}
