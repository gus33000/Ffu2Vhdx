using System;
using System.IO;

namespace Microsoft.WindowsPhone.Imaging
{
    public class ImageStoreHeader
    {
        [StructVersion(Version = 1)]
        public FullFlashUpdateType UpdateType
        {
            get; set;
        }

        [StructVersion(Version = 1)]
        public ushort MajorVersion
        {
            get; set;
        }

        [StructVersion(Version = 1)]
        public ushort MinorVersion
        {
            get; set;
        }

        [StructVersion(Version = 1)]
        public ushort FullFlashMajorVersion
        {
            get; set;
        }

        [StructVersion(Version = 1)]
        public ushort FullFlashMinorVersion
        {
            get; set;
        }

        [StructVersion(Version = 1)]
        public byte[] PlatformIdentifier
        {
            set => _platformId = value;
        }

        [StructVersion(Version = 1)]
        public uint BytesPerBlock
        {
            get; set;
        }

        [StructVersion(Version = 1)]
        public uint StoreDataEntryCount
        {
            get; set;
        }

        [StructVersion(Version = 1)]
        public uint StoreDataSizeInBytes
        {
            get; set;
        }

        [StructVersion(Version = 1)]
        public uint ValidationEntryCount
        {
            get; set;
        }

        [StructVersion(Version = 1)]
        public uint ValidationDataSizeInBytes
        {
            get; set;
        }

        [StructVersion(Version = 1)]
        public uint InitialPartitionTableBlockIndex
        {
            get; set;
        }

        [StructVersion(Version = 1)]
        public uint InitialPartitionTableBlockCount
        {
            get; set;
        }

        [StructVersion(Version = 1)]
        public uint FlashOnlyPartitionTableBlockIndex
        {
            get; set;
        }

        [StructVersion(Version = 1)]
        public uint FlashOnlyPartitionTableBlockCount
        {
            get; set;
        }

        [StructVersion(Version = 1)]
        public uint FinalPartitionTableBlockIndex
        {
            get; set;
        }

        [StructVersion(Version = 1)]
        public uint FinalPartitionTableBlockCount
        {
            get; set;
        }

        [StructVersion(Version = 2)]
        public ushort NumberOfStores
        {
            get => MajorVersion < 2 ? throw new NotImplementedException("NumberOfStores") : _numberOfStores;
            set => _numberOfStores = value;
        }

        [StructVersion(Version = 2)]
        public ushort StoreIndex
        {
            set => _storeIndex = value;
        }

        [StructVersion(Version = 2)]
        public ulong StorePayloadSize
        {
            set => _storePayloadSize = value;
        }

        [StructVersion(Version = 2)]
        public ushort DevicePathLength
        {
            get => MajorVersion < 2 ? throw new NotImplementedException("DevicePathLength") : _devicePathLength;
            set => _devicePathLength = value;
        }

        [StructVersion(Version = 2)]
        public byte[] DevicePath
        {
            set => _devicePath = value;
        }

        public static ImageStoreHeader ReadFromStream(Stream A_0)
        {
            BinaryReader binaryReader = new(A_0);
            ImageStoreHeader imageStoreHeader = new()
            {
                UpdateType = (FullFlashUpdateType)binaryReader.ReadUInt32(),
                MajorVersion = binaryReader.ReadUInt16(),
                MinorVersion = binaryReader.ReadUInt16(),
                FullFlashMajorVersion = binaryReader.ReadUInt16(),
                FullFlashMinorVersion = binaryReader.ReadUInt16(),
                PlatformIdentifier = binaryReader.ReadBytes(ImageStoreHeader.PlatformIdSizeInBytes),
                BytesPerBlock = binaryReader.ReadUInt32(),
                StoreDataEntryCount = binaryReader.ReadUInt32(),
                StoreDataSizeInBytes = binaryReader.ReadUInt32(),
                ValidationEntryCount = binaryReader.ReadUInt32(),
                ValidationDataSizeInBytes = binaryReader.ReadUInt32(),
                InitialPartitionTableBlockIndex = binaryReader.ReadUInt32(),
                InitialPartitionTableBlockCount = binaryReader.ReadUInt32(),
                FlashOnlyPartitionTableBlockIndex = binaryReader.ReadUInt32(),
                FlashOnlyPartitionTableBlockCount = binaryReader.ReadUInt32(),
                FinalPartitionTableBlockIndex = binaryReader.ReadUInt32(),
                FinalPartitionTableBlockCount = binaryReader.ReadUInt32()
            };
            if (imageStoreHeader.MajorVersion >= 2)
            {
                imageStoreHeader.NumberOfStores = binaryReader.ReadUInt16();
                imageStoreHeader.StoreIndex = binaryReader.ReadUInt16();
                imageStoreHeader.StorePayloadSize = binaryReader.ReadUInt64();
                imageStoreHeader.DevicePathLength = binaryReader.ReadUInt16();
                imageStoreHeader.DevicePath = binaryReader.ReadBytes(imageStoreHeader.DevicePathLength * 2);
            }
            return imageStoreHeader;
        }

        public static readonly int PlatformIdSizeInBytes = 192;

        private byte[] _platformId = new byte[ImageStoreHeader.PlatformIdSizeInBytes];

        private ushort _numberOfStores = 1;

        private ushort _storeIndex = 1;

        private ulong _storePayloadSize;

        private ushort _devicePathLength;

        private byte[] _devicePath;
    }
}
