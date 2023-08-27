using System;

namespace Microsoft.WindowsPhone.Imaging
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class StructVersionAttribute : Attribute
    {
        public ushort Version
        {
            get; set;
        }
    }
}
