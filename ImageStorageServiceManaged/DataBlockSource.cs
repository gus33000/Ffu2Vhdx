namespace Microsoft.WindowsPhone.Imaging
{
    public class DataBlockSource
	{
		public DataSource Source
		{
			get; set;
		}

		public ulong StorageOffset
		{
			get; set;
		}

		public enum DataSource
		{
			Zero,
			Disk,
			Memory
		}
	}
}
