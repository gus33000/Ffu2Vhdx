using System;

namespace Microsoft.WindowsPhone.Imaging
{
    [Serializable]
    public class ImageStorageException : Exception
    {
        public ImageStorageException(string A_1) : base(A_1)
        {
        }

        public override string ToString()
        {
            string text = Message;
            if (base.InnerException != null)
            {
                text += base.InnerException.ToString();
            }
            return text;
        }
    }
}
