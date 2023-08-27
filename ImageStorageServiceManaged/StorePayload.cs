using System.Collections.Generic;
using System.IO;

namespace Microsoft.WindowsPhone.Imaging
{
    public class StorePayload
    {
        public ImageStoreHeader StoreHeader
        {
            get; set;
        }

        public List<DataBlockEntry> Phase1DataEntries
        {
            get; set;
        }

        public List<DataBlockEntry> Phase2DataEntries
        {
            get; set;
        }

        public List<DataBlockEntry> Phase3DataEntries
        {
            get; set;
        }

        public StorePayload(bool A_1, bool A_2)
        {
            _recovery = A_1;
            _useLegacyBehavior = A_2;
            StoreHeader = new ImageStoreHeader();
            Phase1DataEntries = new List<DataBlockEntry>();
            Phase2DataEntries = new List<DataBlockEntry>();
            Phase3DataEntries = new List<DataBlockEntry>();
        }

        public List<DataBlockEntry> GetPhaseEntries(BlockPhase A_1)
        {
            List<DataBlockEntry> result = null;
            switch (A_1)
            {
                case StorePayload.BlockPhase.Phase1:
                    result = Phase1DataEntries;
                    break;
                case StorePayload.BlockPhase.Phase2:
                    result = Phase2DataEntries;
                    break;
                case StorePayload.BlockPhase.Phase3:
                    result = Phase3DataEntries;
                    break;
            }
            return result;
        }

        public void ReadMetadataFromStream(Stream A_1)
        {
            StoreHeader = ImageStoreHeader.ReadFromStream(A_1);
            uint num = StoreHeader.InitialPartitionTableBlockIndex + StoreHeader.InitialPartitionTableBlockCount;
            uint num2 = StoreHeader.FlashOnlyPartitionTableBlockIndex + StoreHeader.FlashOnlyPartitionTableBlockCount;
            uint num3 = StoreHeader.FinalPartitionTableBlockIndex + StoreHeader.FinalPartitionTableBlockCount;
            BinaryReader reader = new(A_1);
            uint num4;
            for (num4 = 0U; num4 < num; num4 += 1U)
            {
                DataBlockEntry dataBlockEntry = new(StoreHeader.BytesPerBlock);
                dataBlockEntry.ReadEntryFromStream(reader, num4);
                Phase1DataEntries.Add(dataBlockEntry);
            }
            while (num4 < num2)
            {
                DataBlockEntry dataBlockEntry2 = new(StoreHeader.BytesPerBlock);
                dataBlockEntry2.ReadEntryFromStream(reader, num4);
                Phase2DataEntries.Add(dataBlockEntry2);
                num4 += 1U;
            }
            while (num4 < num3)
            {
                DataBlockEntry dataBlockEntry3 = new(StoreHeader.BytesPerBlock);
                dataBlockEntry3.ReadEntryFromStream(reader, num4);
                Phase3DataEntries.Add(dataBlockEntry3);
                num4 += 1U;
            }
        }

        private readonly bool _recovery;

        private readonly bool _useLegacyBehavior = true;

        public enum BlockPhase
        {
            Phase1,
            Phase2,
            Phase3,
            Invalid
        }
    }
}
