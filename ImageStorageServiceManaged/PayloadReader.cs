using System;
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

        private static bool WriteDetourOk(long inputPosition, int inputCount, long position, int count)
        {
            long begin = Math.Max(inputPosition, position);
            long end = Math.Min(inputPosition + inputCount, position + count);

            return end > begin;
        }

        private static void WriteDetour(long inputPosition, byte[] inputBuffer, int inputCount, long position, byte[] buffer, int offset, int count)
        {
            long begin = Math.Max(inputPosition, position);
            long end = Math.Min(inputPosition + inputCount, position + count);

            if (end > begin)
            {
                int index = (int)(begin - position) + offset;
                int inputIndex = (int)(begin - inputPosition);

                using var strm = new MemoryStream(buffer);
                strm.Seek(index, SeekOrigin.Begin);
                strm.Write(inputBuffer, inputIndex, (int)(end - begin));
            }
        }

        public void WriteToStream(StorePayload storePayload, ulong sectorCount, uint sectorSize, long position, byte[] buffer, int offset, int count)
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
                    bool blockMatches = false;
                    byte[] ibuffer = new byte[bytesPerBlock];

                    for (int i = 0; i < dataBlockEntry.BlockLocationsOnDisk.Count; i++)
                    {
                        long ioffset = (long)(dataBlockEntry.BlockLocationsOnDisk[i].BlockIndex * (ulong)bytesPerBlock);
                        if (dataBlockEntry.BlockLocationsOnDisk[i].AccessMethod == DiskLocation.DiskAccessMethod.DiskEnd)
                        {
                            ioffset = totalSize - ioffset - (long)(ulong)bytesPerBlock;
                        }

                        if (blockMatches)
                        {
                            WriteDetour(ioffset, ibuffer, (int)bytesPerBlock, position, buffer, offset, count);
                        }
                        else
                        {
                            blockMatches = WriteDetourOk(ioffset, (int)bytesPerBlock, position, count);
                            if (blockMatches)
                            {
                                _payloadStream.Read(ibuffer, 0, (int)bytesPerBlock);
                                WriteDetour(ioffset, ibuffer, (int)bytesPerBlock, position, buffer, offset, count);
                            }
                        }
                    }

                    if (!blockMatches)
                    {
                        _payloadStream.Seek((int)bytesPerBlock, SeekOrigin.Current);
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
