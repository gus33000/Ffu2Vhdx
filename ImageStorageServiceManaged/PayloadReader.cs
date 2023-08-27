using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Microsoft.WindowsPhone.Imaging
{
    public class PayloadReader
    {
        public PayloadReader(Stream fileStream)
        {
            _payloadStream = fileStream;
            _payloadOffsets = new List<PayloadOffset>();

            int numberOfStores = 1;

            for (int i = 1; i <= numberOfStores; i++)
            {
                StorePayload storePayload = new(false, true);
                storePayload.ReadMetadataFromStream(fileStream);

                long position = 0;
                if (_payloadStream.Position % (long)(ulong)storePayload.StoreHeader.BytesPerBlock != 0)
                {
                    position = (long)(ulong)(storePayload.StoreHeader.BytesPerBlock - (uint)(_payloadStream.Position % (long)(ulong)storePayload.StoreHeader.BytesPerBlock));
                }

                _payloadStream.Position += position;

                _payloadOffsets.Add(new PayloadOffset
                {
                    Payload = storePayload
                });

                if (storePayload.StoreHeader.MajorVersion >= 2)
                {
                    numberOfStores = storePayload.StoreHeader.NumberOfStores;
                }
            }

            long streamPosition = _payloadStream.Position;

            for (int j = 0; j < numberOfStores; j++)
            {
                PayloadOffset payloadOffset = _payloadOffsets[j];
                payloadOffset.Offset = streamPosition;
                ImageStoreHeader storeHeader = payloadOffset.Payload.StoreHeader;
                streamPosition += (long)(ulong)(storeHeader.BytesPerBlock * storeHeader.StoreDataEntryCount);
            }
        }

        public void WriteToStream(Stream outputStream, StorePayload storePayload, ulong sectorCount, uint sectorSize)
        {
            uint bytesPerBlock = storePayload.StoreHeader.BytesPerBlock;
            long totalSize = (long)(sectorCount * sectorSize);

            PayloadOffset payloadOffset = FindPayloadOffset(storePayload);
            if (payloadOffset == null)
            {
                throw new ImageStorageException("Unable to find store payload.");
            }

            _payloadStream.Position = payloadOffset.Offset;

            for (StorePayload.BlockPhase blockPhase = StorePayload.BlockPhase.Phase1; blockPhase != StorePayload.BlockPhase.Invalid; blockPhase++)
            {
                foreach (DataBlockEntry dataBlockEntry in storePayload.GetPhaseEntries(blockPhase))
                {
                    byte[] buffer = new byte[bytesPerBlock];
                    _ = _payloadStream.Read(buffer, 0, (int)bytesPerBlock);

                    for (int i = 0; i < dataBlockEntry.BlockLocationsOnDisk.Count; i++)
                    {
                        long offset = (long)(dataBlockEntry.BlockLocationsOnDisk[i].BlockIndex * (ulong)bytesPerBlock);
                        if (dataBlockEntry.BlockLocationsOnDisk[i].AccessMethod == DiskLocation.DiskAccessMethod.DiskEnd)
                        {
                            offset = totalSize - offset - (long)(ulong)bytesPerBlock;
                        }

                        _ = outputStream.Seek(offset, SeekOrigin.Begin);
                        outputStream.Write(buffer, 0, (int)bytesPerBlock);
                    }
                }
            }
        }

        public ReadOnlyCollection<StorePayload> Payloads
        {
            get
            {
                List<StorePayload> list = new();

                foreach (PayloadOffset payloadOffset in _payloadOffsets)
                {
                    list.Add(payloadOffset.Payload);
                }

                return list.AsReadOnly();
            }
        }

        private PayloadOffset FindPayloadOffset(StorePayload storePayload)
        {
            for (int i = 0; i < _payloadOffsets.Count; i++)
            {
                PayloadOffset payloadOffset = _payloadOffsets[i];

                if (payloadOffset.Payload == storePayload)
                {
                    return payloadOffset;
                }
            }

            return null;
        }

        private readonly List<PayloadOffset> _payloadOffsets;

        private readonly Stream _payloadStream;

        private class PayloadOffset
        {
            public StorePayload Payload
            {
                get; set;
            }

            public long Offset
            {
                get; set;
            }
        }
    }
}
