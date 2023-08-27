using Microsoft.WindowsPhone.Imaging;

namespace Ffu2Vhdx
{
    internal class FfuFile
    {
        private readonly FullFlashUpdateImage ffuImage;

        private readonly PayloadReader payloadReader;

        public FfuFile(Stream stream)
        {
            ffuImage = new(stream);
            Stream imageStream = ffuImage.GetImageStream();
            payloadReader = new(imageStream);

            if (payloadReader.Payloads.Count() != ffuImage.StoreCount)
            {
                throw new ImageStorageException("Store counts in metadata and store header do not match.");
            }
        }

        public int StoreCount => ffuImage.StoreCount;

        public void WriteStoreToStream(int StoreIndex, Stream outputStream)
        {
            if (StoreIndex >= StoreCount || StoreIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(StoreIndex));
            }

            FullFlashUpdateStore fullFlashUpdateStore = ffuImage.Stores[StoreIndex];
            StorePayload storePayload = payloadReader.Payloads[StoreIndex];

            payloadReader.WriteToStream(outputStream, storePayload, fullFlashUpdateStore.MinSectorCount, fullFlashUpdateStore.SectorSize);
        }

        public uint GetMinSectorCount(int StoreIndex)
        {
            if (StoreIndex >= StoreCount || StoreIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(StoreIndex));
            }

            FullFlashUpdateStore fullFlashUpdateStore = ffuImage.Stores[StoreIndex];
            return fullFlashUpdateStore.MinSectorCount;
        }

        public uint GetSectorSize(int StoreIndex)
        {
            if (StoreIndex >= StoreCount || StoreIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(StoreIndex));
            }

            FullFlashUpdateStore fullFlashUpdateStore = ffuImage.Stores[StoreIndex];
            return fullFlashUpdateStore.SectorSize;
        }
    }
}