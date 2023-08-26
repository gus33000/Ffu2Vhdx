using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Microsoft.WindowsPhone.Imaging
{
    public class PayloadReader
	{
		public PayloadReader(FileStream A_1)
		{
			_payloadStream = A_1;
			_payloadOffsets = new List<PayloadOffset>();
			int num = 1;
			for (int i = 1; i <= num; i++)
			{
				StorePayload storePayload = new StorePayload(false, true);
				storePayload.ReadMetadataFromStream(A_1);
				long num2 = 0L;
				if (_payloadStream.Position % (long)((ulong)storePayload.StoreHeader.BytesPerBlock) != 0L)
				{
					num2 = (long)((ulong)(storePayload.StoreHeader.BytesPerBlock - (uint)(_payloadStream.Position % (long)((ulong)storePayload.StoreHeader.BytesPerBlock))));
				}
				_payloadStream.Position += num2;
				_payloadOffsets.Add(new PayloadOffset
                {
					Payload = storePayload
				});
				if (storePayload.StoreHeader.MajorVersion >= 2)
				{
					num = storePayload.StoreHeader.NumberOfStores;
				}
			}
			long num3 = _payloadStream.Position;
			for (int j = 0; j < num; j++)
			{
                PayloadOffset payloadOffset = _payloadOffsets[j];
				payloadOffset.Offset = num3;
				ImageStoreHeader storeHeader = payloadOffset.Payload.StoreHeader;
				num3 += (long)((ulong)(storeHeader.BytesPerBlock * storeHeader.StoreDataEntryCount));
			}
		}

		public void WriteToStream(Stream A_1, StorePayload A_2, ulong A_3, uint A_4)
		{
			uint bytesPerBlock = A_2.StoreHeader.BytesPerBlock;
			long num = (long)(A_3 * A_4);
            PayloadOffset payloadOffset = FindPayloadOffset(A_2);
			if (payloadOffset == null)
			{
				throw new ImageStorageException("Unable to find store payload.");
			}
			_payloadStream.Position = payloadOffset.Offset;
			for (StorePayload.BlockPhase blockPhase = StorePayload.BlockPhase.Phase1; blockPhase != StorePayload.BlockPhase.Invalid; blockPhase++)
			{
				foreach (DataBlockEntry dataBlockEntry in A_2.GetPhaseEntries(blockPhase))
				{
					byte[] buffer = new byte[bytesPerBlock];
					_payloadStream.Read(buffer, 0, (int)bytesPerBlock);
					for (int i = 0; i < dataBlockEntry.BlockLocationsOnDisk.Count; i++)
					{
						long num2 = (long)(dataBlockEntry.BlockLocationsOnDisk[i].BlockIndex * (ulong)bytesPerBlock);
						if (dataBlockEntry.BlockLocationsOnDisk[i].AccessMethod == DiskLocation.DiskAccessMethod.DiskEnd)
						{
							num2 = num - num2 - (long)((ulong)bytesPerBlock);
						}
						A_1.Seek(num2, SeekOrigin.Begin);
						A_1.Write(buffer, 0, (int)bytesPerBlock);
					}
				}
			}
        }

        public ReadOnlyCollection<StorePayload> Payloads
		{
			get
			{
				List<StorePayload> list = new List<StorePayload>();
				foreach (PayloadOffset payloadOffset in _payloadOffsets)
				{
					list.Add(payloadOffset.Payload);
				}
				return list.AsReadOnly();
			}
		}

		private PayloadOffset FindPayloadOffset(StorePayload A_1)
		{
			for (int i = 0; i < _payloadOffsets.Count; i++)
			{
                PayloadOffset payloadOffset = _payloadOffsets[i];
				if (payloadOffset.Payload == A_1)
				{
					return payloadOffset;
				}
			}
			return null;
		}

		private readonly List<PayloadOffset> _payloadOffsets;

		private readonly FileStream _payloadStream;

		private class PayloadOffset
		{
			public StorePayload Payload { get; set; }

			public long Offset { get; set; }
		}
	}
}
