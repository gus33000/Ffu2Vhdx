using System.Runtime.InteropServices;

namespace Microsoft.WindowsPhone.Imaging
{
    [StructLayout(LayoutKind.Sequential)]
    public class ImageHeaderEx : ImageHeader
    {
        [MarshalAs(UnmanagedType.U4)]
        public uint NumberOfDeviceTargetingIds;
    }
}
