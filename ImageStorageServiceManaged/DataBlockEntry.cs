using System.Collections.Generic;
using System.IO;

namespace Microsoft.WindowsPhone.Imaging
{
    public class DataBlockEntry
	{
		public uint BytesPerBlock { get; private set; }

		public DataBlockEntry(uint A_1)
		{
			BytesPerBlock = A_1;
			DataSource = new DataBlockSource();
		}

		public List<DiskLocation> BlockLocationsOnDisk
		{
			get
			{
				return _blockLocations;
			}
		}

		public DataBlockSource DataSource
		{
			get; set;
		}

		public void ReadEntryFromStream(BinaryReader A_1, uint A_2)
		{
			int num = A_1.ReadInt32();
			if (A_1.ReadUInt32() != 1U)
			{
				throw new ImageStorageException("More than one block per data block entry is not currently supported.");
			}
			for (int i = 0; i < num; i++)
			{
				DiskLocation diskLocation = new DiskLocation();
				diskLocation.Read(A_1);
				_blockLocations.Add(diskLocation);
			}
			DataBlockSource dataSource = new DataBlockSource
			{
				Source = DataBlockSource.DataSource.Disk,
				StorageOffset = A_2 * BytesPerBlock
            };
			DataSource = dataSource;
		}

		private readonly List<DiskLocation> _blockLocations = new List<DiskLocation>();
	}
}
