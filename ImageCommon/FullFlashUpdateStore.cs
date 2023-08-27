using Microsoft.Composition.ToolBox.IO;
using System;
using System.Collections.Generic;

namespace Microsoft.WindowsPhone.Imaging
{
    public class FullFlashUpdateStore : IDisposable
    {
        public FullFlashUpdateStore()
        {
            _ffuPartitions = new List<FullFlashUpdatePartition>();
        }

        public FullFlashUpdateStore(FullFlashUpdateImage A_1, FullFlashUpdateStoragePool A_2, ManifestCategory A_3) : this()
        {
            Initialize(A_1, A_2, A_3.GetString("StoreId"), A_3.GetString("StoreType"), A_3.GetBool("IsSpace"), A_3.GetBool("IsMainOSStore", true), A_3.GetString("Provisioning"), A_3.GetString("DevicePath"), A_3.GetBool("OnlyAllocateDefinedGptEntries"), A_3.GetUInt32("MinSectorCount"), A_3.GetUInt32("SectorSize"));
        }

        private void Initialize(FullFlashUpdateImage A_1, FullFlashUpdateStoragePool A_2, string A_3, string A_4, bool A_5, bool A_6, string A_7, string A_8, bool A_9, uint A_10, uint A_11)
        {
            _tempBackingStorePath = PathToolBox.GetTemporaryPath();
            DirectoryToolBox.CreateDirectory(_tempBackingStorePath);
            _tempBackingStoreFile = PathToolBox.Combine(_tempBackingStorePath, "FFUBackingStore");
            Image = A_1;
            StoragePool = A_2;
            Id = A_3;
            Type = A_4;
            IsSpace = A_5;
            IsMainOSStore = A_6;
            Provisioning = A_7;
            DevicePath = A_8;
            OnlyAllocateDefinedGptEntries = A_9;
            MinSectorCount = A_10;
            SectorSize = A_11;
        }

        ~FullFlashUpdateStore()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool A_1)
        {
            if (_alreadyDisposed)
            {
                return;
            }
            if (A_1)
            {
                Partitions = null;
            }
            try
            {
                DirectoryToolBox.Delete(_tempBackingStorePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Warning: ImageCommon!Dispose: Failed to delete temporary backing store directory/file '" + _tempBackingStorePath + "' with exception: " + ex.Message);
            }
            _alreadyDisposed = true;
        }

        private FullFlashUpdateImage Image
        {
            get; set;
        }

        private FullFlashUpdateStoragePool StoragePool
        {
            get; set;
        }

        private string Id
        {
            get; set;
        }

        private string Type
        {
            get; set;
        }

        public bool IsMainOSStore
        {
            get; private set;
        }

        private string Provisioning
        {
            get; set;
        }

        private string DevicePath
        {
            get; set;
        }

        private bool OnlyAllocateDefinedGptEntries
        {
            get; set;
        }

        public uint MinSectorCount
        {
            get; set;
        }

        public uint SectorSize
        {
            get; set;
        }

        public List<FullFlashUpdatePartition> Partitions
        {
            get => new(_ffuPartitions);
            private set => _ffuPartitions = value;
        }

        public FullFlashUpdatePartition this[string A_1]
        {
            get
            {
                foreach (FullFlashUpdatePartition fullFlashUpdatePartition in Partitions)
                {
                    if (string.CompareOrdinal(fullFlashUpdatePartition.Name, A_1) == 0)
                    {
                        return fullFlashUpdatePartition;
                    }
                }
                return null;
            }
        }

        public bool IsSpace
        {
            get; private set;
        }

        internal void AddPartition(FullFlashUpdatePartition A_1)
        {
            if (this[A_1.Name] != null)
            {
                throw new ImageCommonException(CodeSite.StorageService, ErrorCategory.IncorrectUserInput, "Two partitions in a store have the same name (" + A_1.Name + ").");
            }
            if (!IsSpace && IsMainOSStore)
            {
                if (MinSectorCount != 0U && A_1.TotalSectors > MinSectorCount)
                {
                    throw new ImageCommonException(CodeSite.StorageService, ErrorCategory.IncorrectUserInput, "The partition " + A_1.Name + " is too large for the store.");
                }
                if (A_1.UseAllSpace)
                {
                    using (List<FullFlashUpdatePartition>.Enumerator enumerator = Partitions.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            if (enumerator.Current.UseAllSpace)
                            {
                                throw new ImageCommonException(CodeSite.StorageService, ErrorCategory.IncorrectUserInput, "Two partitions in the same store have the UseAllSpace flag set.");
                            }
                        }
                        goto IL_FA;
                    }
                }
                if (A_1.SectorsInUse > A_1.TotalSectors)
                {
                    throw new ImageCommonException(CodeSite.StorageService, ErrorCategory.IncorrectUserInput, string.Format("The partition data is invalid.  There are more used sectors ({0}) than total sectors ({1}) for partition:{2}", A_1.SectorsInUse, A_1.TotalSectors, A_1.Name));
                }
            IL_FA:
                if (MinSectorCount != 0U)
                {
                    if (A_1.UseAllSpace)
                    {
                        _sectorsUsed += 1U;
                    }
                    else
                    {
                        _sectorsUsed += A_1.TotalSectors;
                    }
                    if (_sectorsUsed > MinSectorCount)
                    {
                        throw new ImageCommonException(CodeSite.StorageService, ErrorCategory.IncorrectUserInput, string.Format("Partition ({0}) on the Store does not fit. SectorsUsed = {1} > MinSectorCount = {2}", A_1.Name, _sectorsUsed, MinSectorCount));
                    }
                }
            }
            _ffuPartitions.Add(A_1);
        }

        internal void AddPartition(ManifestCategory A_1)
        {
            uint sectorsInUse = 0U;
            uint num = 0U;
            bool flag = false;
            if (A_1 == null)
            {
                throw new ArgumentNullException("category");
            }
            if (IsMainOSStore)
            {
                sectorsInUse = A_1.GetUInt32("UsedSectors");
                num = A_1.GetUInt32("TotalSectors");
                flag = A_1.GetBool("UseAllSpace");
                if (!flag && num == 0U)
                {
                    throw new ImageCommonException(CodeSite.StorageService, ErrorCategory.IncorrectUserInput, "The partition category for partition " + A_1.GetString("Name") + " must contain either a 'TotalSectors' or 'UseAllSpace' key/value pair.");
                }
                if (flag && num > 0U)
                {
                    throw new ImageCommonException(CodeSite.StorageService, ErrorCategory.IncorrectUserInput, "The partition category for partition " + A_1.GetString("Name") + " cannot contain both a 'TotalSectors' and a 'UseAllSpace' key/value pair.");
                }
            }
            AddPartition(new FullFlashUpdatePartition(this, new InputPartition
            {
                Name = A_1.GetString("Name"),
                Type = A_1.GetString("Type"),
                Id = A_1.GetString("Id"),
                ReadOnly = A_1.GetBool("ReadOnly"),
                AttachDriveLetter = A_1.GetBool("AttachDriveLetter"),
                ServicePartition = A_1.GetBool("ServicePartition"),
                Hidden = A_1.GetBool("Hidden"),
                Bootable = A_1.GetBool("Bootable"),
                TotalSectors = num,
                UseAllSpace = flag,
                FileSystem = A_1.GetString("FileSystem"),
                PrimaryPartition = A_1.GetString("Primary"),
                RequiredToFlash = A_1.GetBool("RequiredToFlash"),
                ByteAlignment = A_1.GetUInt32("ByteAlignment"),
                ClusterSize = A_1.GetUInt32("ClusterSize"),
                OffsetInSectors = A_1.GetUInt64("OffsetInSectors"),
                PrepareFveMetadata = A_1.GetBool("PrepareFveMetadata")
            })
            {
                SectorsInUse = sectorsInUse,
                SectorAlignment = A_1.GetUInt32("SectorAlignment")
            });
        }

        private List<FullFlashUpdatePartition> _ffuPartitions;

        private uint _sectorsUsed;

        private string _tempBackingStoreFile = string.Empty;

        private string _tempBackingStorePath = string.Empty;

        private bool _alreadyDisposed;
    }
}
