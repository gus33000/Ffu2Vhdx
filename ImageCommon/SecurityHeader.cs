namespace Microsoft.WindowsPhone.Imaging
{
    public struct SecurityHeader
    {
        public static bool ValidateSignature(byte[] A_0)
        {
            byte[] securityHeaderSignature = FullFlashUpdateHeaders.GetSecurityHeaderSignature();
            for (int i = 0; i < securityHeaderSignature.Length; i++)
            {
                if (A_0[i] != securityHeaderSignature[i])
                {
                    return false;
                }
            }
            return true;
        }

        public uint ByteCount
        {
            get; set;
        }

        public uint ChunkSize
        {
            readonly get; set;
        }

        public uint HashAlgorithmID
        {
            get; set;
        }

        public uint CatalogSize
        {
            readonly get; set;
        }

        public uint HashTableSize
        {
            readonly get; set;
        }
    }
}
