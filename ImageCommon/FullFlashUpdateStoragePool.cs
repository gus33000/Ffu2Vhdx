using System;
using System.Collections.Generic;

namespace Microsoft.WindowsPhone.Imaging
{
    public class FullFlashUpdateStoragePool
    {
        private FullFlashUpdateStoragePool()
        {
            _ffuStores = new List<FullFlashUpdateStore>();
        }

        public FullFlashUpdateStoragePool(FullFlashUpdateImage A_1, ManifestCategory A_2) : this()
        {
            Image = A_1;
            Name = A_2.GetString("Name");
        }

        public FullFlashUpdateImage Image
        {
            get; private set;
        }

        private string Name
        {
            get; set;
        }

        public List<FullFlashUpdateStore> Stores => new(_ffuStores);

        internal void AddStore(ManifestCategory A_1)
        {
            if (A_1 == null)
            {
                throw new ArgumentNullException("category");
            }
            _ffuStores.Add(new FullFlashUpdateStore(Image, this, A_1));
        }

        private readonly List<FullFlashUpdateStore> _ffuStores;
    }
}
