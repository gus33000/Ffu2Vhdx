using System.IO;

namespace Microsoft.WindowsPhone.Imaging
{
    public class DiskLocation
	{
		public DiskLocation()
		{
			BlockIndex = 0U;
			AccessMethod = DiskLocation.DiskAccessMethod.DiskBegin;
		}

		public DiskAccessMethod AccessMethod { get; set; }

		public uint BlockIndex { get; set; }

		public void Read(BinaryReader A_1)
		{
			AccessMethod = (DiskAccessMethod)A_1.ReadUInt32();
			BlockIndex = A_1.ReadUInt32();
		}

		public enum DiskAccessMethod : uint
		{
			DiskBegin,
			DiskEnd = 2U
		}
	}
}
