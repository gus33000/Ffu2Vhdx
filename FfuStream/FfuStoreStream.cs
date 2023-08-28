using Microsoft.WindowsPhone.Imaging;

namespace FfuStream
{
    public partial class FfuStoreStream : Stream
    {
        private readonly PayloadReader payloadReader;
        private readonly FullFlashUpdateStore fullFlashUpdateStore;
        private readonly StorePayload storePayload;
        private long position = 0;

        public FfuStoreStream(PayloadReader payloadReader, FullFlashUpdateStore fullFlashUpdateStore, StorePayload storePayload)
        {
            this.payloadReader = payloadReader;
            this.fullFlashUpdateStore = fullFlashUpdateStore;
            this.storePayload = storePayload;
        }

        public string DevicePath => fullFlashUpdateStore.DevicePath;

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => false;

        public override long Length => ((long)fullFlashUpdateStore.MinSectorCount * fullFlashUpdateStore.SectorSize);

        public override long Position
        {
            get => position;
            set => position = value;
        }

        public override void Flush()
        {
            // Nothing to do!
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (position + count > Length)
            {
                count = (int)Length - (int)position;
            }

            payloadReader.WriteToStream(storePayload, fullFlashUpdateStore.MinSectorCount, fullFlashUpdateStore.SectorSize, position, buffer, offset, count);

            position += count;

            return count;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (offset > Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            switch (origin)
            {
                case SeekOrigin.Begin:
                    {
                        if (offset < 0)
                        {
                            throw new IOException();
                        }

                        position = offset;
                        break;
                    }
                case SeekOrigin.Current:
                    {
                        int tempPosition = (int)position + (int)offset;
                        if (tempPosition < 0)
                        {
                            throw new IOException();
                        }

                        position = tempPosition;
                        break;
                    }
                case SeekOrigin.End:
                    {
                        int tempPosition = (int)Length + (int)offset;
                        if (tempPosition < 0)
                        {
                            throw new IOException();
                        }

                        position = tempPosition;
                        break;
                    }
                default:
                    {
                        throw new ArgumentException(nameof(origin));
                    }
            }

            return position;
        }

        public override void SetLength(long value)
        {
            // Nothing to do!
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            // Nothing to do!
        }
    }
}