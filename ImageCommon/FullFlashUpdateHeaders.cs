﻿using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.WindowsPhone.Imaging
{
    public static class FullFlashUpdateHeaders
    {
        public static byte[] GetSecurityHeaderSignature()
        {
            return Encoding.ASCII.GetBytes("SignedImage ");
        }

        public static byte[] GetImageHeaderSignature()
        {
            return Encoding.ASCII.GetBytes("ImageFlash  ");
        }

        public static uint SecurityHeaderSize => (uint)(Marshal.SizeOf(default(SecurityHeader)) + GetSecurityHeaderSignature().Length);

        public static uint ImageHeaderSize => (uint)(FullFlashUpdateImage.ImageHeaderSize + GetImageHeaderSignature().Length);

        public static uint ImageHeaderExSize => (uint)(FullFlashUpdateImage.ImageHeaderExSize + GetImageHeaderSignature().Length);
    }
}
