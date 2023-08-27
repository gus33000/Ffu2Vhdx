using Microsoft.Composition.ToolBox.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Microsoft.WindowsPhone.Imaging
{
    [DefaultMember("Item")]
    public class FullFlashUpdateImage
    {
        public FullFlashUpdateImage()
        {
            _ffuStoragePools = new List<FullFlashUpdateStoragePool>();
            _ffuStores = new List<FullFlashUpdateStore>();
        }

        public FullFlashUpdateImage(string A_1) : this()
        {
            if (!FileToolBox.Exists(A_1))
            {
                throw new ImageCommonException(CodeSite.StorageService, ErrorCategory.MissingArtifact, "The FFU file '" + A_1 + "' does not exist.");
            }
            _imagePath = PathToolBox.GetFullPath(A_1);
            using FileStream imageStream = GetImageStream();
            using BinaryReader binaryReader = new(imageStream);
            uint num = binaryReader.ReadUInt32();
            byte[] signature = binaryReader.ReadBytes(12);
            if (num != FullFlashUpdateHeaders.SecurityHeaderSize || !SecurityHeader.ValidateSignature(signature))
            {
                throw new ImageCommonException(CodeSite.StorageService, ErrorCategory.BadContent, "Unable to load image because the security header is invalid.");
            }
            _securityHeader.ByteCount = num;
            _securityHeader.ChunkSize = binaryReader.ReadUInt32();
            _securityHeader.HashAlgorithmID = binaryReader.ReadUInt32();
            _securityHeader.CatalogSize = binaryReader.ReadUInt32();
            _securityHeader.HashTableSize = binaryReader.ReadUInt32();
            CatalogData = binaryReader.ReadBytes((int)_securityHeader.CatalogSize);
            HashTableData = binaryReader.ReadBytes((int)_securityHeader.HashTableSize);
            _ = binaryReader.ReadBytes((int)SecurityPadding);
            num = binaryReader.ReadUInt32();
            signature = binaryReader.ReadBytes(12);
            if ((num != FullFlashUpdateHeaders.ImageHeaderSize && num != FullFlashUpdateHeaders.ImageHeaderExSize) || !ImageHeader.ValidateSignature(signature))
            {
                throw new ImageCommonException(CodeSite.StorageService, ErrorCategory.BadContent, "Unable to load image because the image header is invalid.");
            }
            _imageHeader.ByteCount = num;
            _imageHeader.ManifestLength = binaryReader.ReadUInt32();
            _imageHeader.ChunkSize = binaryReader.ReadUInt32();
            _imageHeader.NumberOfDeviceTargetingIds = num == FullFlashUpdateHeaders.ImageHeaderSize ? 0U : binaryReader.ReadUInt32();
            Manifest = new FullFlashUpdateManifest(this, binaryReader.ReadBytes((int)_imageHeader.ManifestLength));
            Manifest.Validate("2.0");
            if (_imageHeader.ChunkSize > 0U)
            {
                _ = binaryReader.ReadBytes((int)CalculateAlignment((uint)imageStream.Position, _imageHeader.ChunkSize * 1024U));
            }
            _payloadOffset = imageStream.Position;
        }

        public FileStream GetImageStream()
        {
            FileStream fileStream = FileToolBox.Stream(_imagePath);
            fileStream.Position = _payloadOffset;
            return fileStream;
        }

        internal void AddStoragePool(ManifestCategory A_1)
        {
            if (A_1 == null)
            {
                throw new ArgumentNullException("category");
            }
            _ffuStoragePools.Add(new FullFlashUpdateStoragePool(this, A_1));
        }

        internal void AddStore(ManifestCategory A_1)
        {
            if (A_1 == null)
            {
                throw new ArgumentNullException("category");
            }
            _ffuStores.Add(new FullFlashUpdateStore(this, null, A_1));
        }

        public static int ImageHeaderSize => Marshal.SizeOf(new ImageHeader());

        public static int ImageHeaderExSize => Marshal.SizeOf(new ImageHeaderEx());

        public byte[] CatalogData
        {
            get; set;
        }

        public byte[] HashTableData
        {
            get; set;
        }

        public List<FullFlashUpdateStoragePool> StoragePools => new(_ffuStoragePools);

        public List<FullFlashUpdateStore> Stores => new(_ffuStores);

        public int StoreCount => Stores.Count;

        public FullFlashUpdateManifest Manifest
        {
            get; private set;
        }

        internal uint SecurityPadding
        {
            get
            {
                uint num = 1024U;
                if (_imageHeader.ChunkSize != 0U)
                {
                    num *= _imageHeader.ChunkSize;
                }
                else
                {
                    if (_securityHeader.ChunkSize == 0U)
                    {
                        throw new ImageCommonException(CodeSite.StorageService, ErrorCategory.BadContent, "Neither the of the headers have been initialized with a chunk size.");
                    }
                    num *= _securityHeader.ChunkSize;
                }
                return CalculateAlignment(FullFlashUpdateHeaders.SecurityHeaderSize + (uint)(CatalogData != null ? CatalogData.Length : 0) + (uint)(HashTableData != null ? HashTableData.Length : 0), num);
            }
        }

        private uint CalculateAlignment(uint A_1, uint A_2)
        {
            uint result = 0U;
            uint num = A_1 % A_2;
            if (num > 0U)
            {
                result = A_2 - num;
            }
            return result;
        }

        public static readonly uint PartitionTypeMbr = 0U;

        public static readonly uint PartitionTypeGpt = 1U;

        private readonly List<FullFlashUpdateStoragePool> _ffuStoragePools;

        private readonly List<FullFlashUpdateStore> _ffuStores;

        private readonly string _imagePath;

        private readonly long _payloadOffset;

        private readonly ImageHeaderEx _imageHeader = new();

        private SecurityHeader _securityHeader;

        private readonly uint _defaultPartititionByteAlignment = 65536U;
    }
}
