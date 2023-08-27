using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Microsoft.WindowsPhone.Imaging
{
    [StructLayout(LayoutKind.Sequential)]
    public class ImageHeader
    {
        public static bool ValidateSignature(byte[] A_0)
        {
            return A_0.SequenceEqual(FullFlashUpdateHeaders.GetImageHeaderSignature());
        }

        [MarshalAs(UnmanagedType.U4)]
        public uint ByteCount;

        [MarshalAs(UnmanagedType.U4)]
        public uint ManifestLength;

        [MarshalAs(UnmanagedType.U4)]
        public uint ChunkSize;
    }
}
