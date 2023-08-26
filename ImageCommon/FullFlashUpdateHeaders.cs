using System.Runtime.InteropServices;
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

		public static uint SecurityHeaderSize
		{
			get
			{
				return (uint)(Marshal.SizeOf(default(SecurityHeader)) + FullFlashUpdateHeaders.GetSecurityHeaderSignature().Length);
			}
		}

		public static uint ImageHeaderSize
		{
			get
			{
				return (uint)(FullFlashUpdateImage.ImageHeaderSize + FullFlashUpdateHeaders.GetImageHeaderSignature().Length);
			}
		}

		public static uint ImageHeaderExSize
		{
			get
			{
				return (uint)(FullFlashUpdateImage.ImageHeaderExSize + FullFlashUpdateHeaders.GetImageHeaderSignature().Length);
			}
		}
	}
}
