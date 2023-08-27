using System;
using System.Text;

namespace Microsoft.WindowsPhone.Imaging
{
    [Serializable]
    public class ImageCommonException : Exception
    {
        public CodeSite CodeSite
        {
            get; set;
        } = CodeSite.Unknown;

        public ErrorCategory ErrorCategory
        {
            get; set;
        } = ErrorCategory.Unknown;

        public ImageCommonException(CodeSite A_1, ErrorCategory A_2, string A_3) : base(A_3)
        {
            CodeSite = A_1;
            ErrorCategory = A_2;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new();
            _ = stringBuilder.AppendLine(Message);
            if (InnerException != null)
            {
                _ = stringBuilder.Append(InnerException.ToString());
            }
            return stringBuilder.ToString();
        }
    }
}
